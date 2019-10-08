using System;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один параметр для записи в архив
    public class CalcParamArchive
    {
        public CalcParamArchive(IRecordRead rec, Project project)
        {
            FirstParam = project.CalcParamsId[rec.GetInt("CalcParamId")];
            int sid = rec.GetInt("CalcSubParamId");
            LastParam = project.CalcSubParamsId[sid];
            DataType = rec.GetString("DataType").ToDataType();
            ArchiveParam = new ArchiveParam(rec.GetString("FullCode"), DataType, rec.GetString("Units"), FirstParam, LastParam,
                                                                    rec.GetString("SuperProcessType").ToSuperProcess(), rec.GetInt("DecPlaces", -1), rec.GetDouble("Min"), rec.GetDouble("Max"));
            _saveAbsolute = ArchiveParam.SuperProcess.IsAbsolute();
            _savePeriodic = ArchiveParam.SuperProcess.IsPeriodic();
        }
        
        //Первый и последний расчетный параметр в цепочке
        public CalcParam FirstParam { get; private set; }
        public CalcParam LastParam { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }
        //Ссылка на ArchiveParam
        public ArchiveParam ArchiveParam { get; private set; }
        //Ссылка на CalcParamRun
        public CalcParamRun RunParam { get; set; }

        //Сохранять переиодические и абсолютные значения
        private readonly bool _savePeriodic;
        private readonly bool _saveAbsolute;

        //Базовые значения за текущий час
        private Moment[] _baseValues;
        //Количество базовых значений
        private int _baseNum;
        //Часовые значения за текущие сутки
        private Moment[] _hourValues;
        //Количество часовых значений
        private int _hourNum;
        //Текущие абсолютное значение или абсолютное значение до которого надо дойти, чтоб начать абсолютное накопление
        //и конец его интервала
        public Moment AbsoluteValue { get; set; }
        public DateTime AbsoluteEnd { get; set; }
        //Отредактированное абсолютное значение, если нет то null
        public Moment AbsoluteEditValue { get; set; }
        
        //Загружает считанные из данные из архивного параметра
        //beg - начало периода расчета, en - конец периода расчета
        public void FromArchiveParam(DateTime beg, DateTime en)
        {
            double len = en.Subtract(beg).TotalMinutes;
            _baseNum = 0; _hourNum = 0;
            if (_savePeriodic)
            {
                _baseValues = new Moment[Convert.ToInt32(60/len)];
                _hourValues = new Moment[24];
            }
            var abTimeBefore = Different.MinDate;
            var abTimeAfter = Different.MaxDate;
            Moment abValueBefore = null, abValueAfter = null;
            foreach (var t in ArchiveParam.Intervals)
            {
                switch (t.Key.Type)
                {
                    case IntervalType.Absolute:
                    case IntervalType.AbsoluteDay:
                        if (t.Key.End <= beg && t.Key.End > abTimeBefore)
                        {
                            abTimeBefore = t.Key.End;
                            abValueBefore = t.Value.Moment;
                        }
                        if (t.Key.Begin <= beg && t.Key.Begin > abTimeBefore && abValueBefore == null)
                        {
                            abTimeBefore = t.Key.Begin;
                            abValueBefore = null;
                        }
                        if (t.Key.End >= beg && t.Key.End < abTimeAfter)
                        {
                            abTimeAfter = t.Key.End;
                            abValueAfter = t.Value.Moment;
                        }
                        break;
                    case IntervalType.Base:
                        int n;
                        if (Math.Abs(t.Key.End.Subtract(t.Key.Begin).TotalMinutes - len) < 0.01)
                        {
                            n = Convert.ToInt32(t.Key.Begin.Minute/len);
                            if (_baseValues[n] == null) _baseNum++;
                            _baseValues[n] = t.Value.Moment;
                        }
                        break;
                    case IntervalType.Hour:
                        n = t.Key.Begin.Hour;
                        if (_hourValues[n] == null) _hourNum++;
                        _hourValues[t.Key.Begin.Hour] = t.Value.Moment;
                        break;
                }
            }
            if (_saveAbsolute)
            {
                if (AbsoluteEditValue != null)
                {
                    AbsoluteEnd = beg;
                    AbsoluteValue = AbsoluteEditValue;
                    AbsoluteEditValue = null;
                }
                else if (abTimeBefore.EqulasToSeconds(beg))
                {
                    AbsoluteEnd = beg;
                    AbsoluteValue = abValueBefore;
                }
                else if (abTimeBefore == Different.MinDate)
                {
                    AbsoluteEnd = beg;
                    AbsoluteValue = null;
                }
                else if (abTimeAfter == Different.MaxDate)
                {
                    AbsoluteEnd = beg;
                    AbsoluteValue = abValueBefore;
                }
                else //abValueAfter != Different.MaxDate
                {
                    AbsoluteEnd = abTimeAfter;
                    AbsoluteValue = abValueAfter;
                }
            }
        }

        //Накопление данных по параметру
        public void Accumulate()
        {
            try
            {
                var process = ArchiveParam.SuperProcess.ToProcess();
                if (ArchiveParam == null || process.IsNone()) return; 
                var ivs = ArchiveParam.Intervals;
                ivs.Clear();
                var p = RunParam.CalcParam.Project;
                var v = RunParam.CalcValue.SingleValue;
                //if (v.Type == SingleType.List && v.Moments.Count == 0) return;
                var beg = p.ThreadCalc.PeriodBegin;
                var en = p.ThreadCalc.PeriodEnd;
                if (!p.ThreadCalc.IsPeriodic) //Разовый
                    ivs.Add(p.SingleInterval, v);
                else 
                {
                    if (process == SuperProcess.Moment)//Мгновенные
                        ivs.Add(p.MomentsInterval, v);
                    else
                    {
                        var m = v.LastMoment;
                        if (_savePeriodic)//Периодические
                        {
                            ivs.Add(p.BaseInterval, new SingleValue(m));
                            _baseValues[Convert.ToInt32(beg.Minute/p.ThreadCalc.PeriodLength)] = m;
                            _baseNum++;
                            var e = process == SuperProcess.AverageP || process == SuperProcess.AvNonZeroP || process == SuperProcess.SummP;
                            if (_baseNum == _baseValues.Length)
                            {
                                p.HasHourValues = true;
                                var mv = ConnectIntervals(_baseValues);
                                if (e) mv.Time = p.HourInterval.Begin;
                                _hourValues[beg.Hour] = mv;
                                ivs.Add(p.HourInterval, new SingleValue(mv));
                                _hourNum++;
                            }
                            if (en.Minute == 0) _baseNum = 0;
                            if (_hourNum == _hourValues.Length)
                            {
                                p.HasDayValues = true;
                                var mv = ConnectIntervals(_hourValues);
                                if (e) mv.Time = p.DayInterval.Begin;
                                ivs.Add(p.DayInterval, new SingleValue(mv));
                            }
                            if (en.Minute == 0 && en.Hour == 0) _hourNum = 0;
                        }

                        if (_saveAbsolute )//Абсолютные
                        {
                            if (AbsoluteEnd == beg)
                            {
                                p.HasAbsoluteValues = true;
                                AbsoluteValue = ConnectAbsolute(beg, AbsoluteValue, new TimeInterval(beg, en), m);
                                AbsoluteEnd = en;
                                var sv = new SingleValue(AbsoluteValue);
                                ivs.Add(p.AbsoluteInterval, sv);
                                if (en.Minute == 0 && en.Hour == 0)
                                    ivs.Add(p.AbsoluteDayInterval, sv);       
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                if (RunParam != null) 
                    RunParam.ThreadCalc.AddError("Ошибка накопления по параметру", ex, ArchiveParam == null ? "" : ArchiveParam.FullCode);
            }
        }

        //Накапливает час по базовым или сутки по часовым
        private Moment ConnectIntervals(Moment[] intervals)
        {
            var process = ArchiveParam.SuperProcess;
            if (process == SuperProcess.FirstP || process == SuperProcess.FirstPA) 
                return intervals[0];
            if (process == SuperProcess.LastP || process == SuperProcess.LastPA) 
                return intervals[intervals.Length-1];
            Moment res = intervals[0].Clone(DataType);
            for(int i = 1; i < intervals.Length; i++)
            {
                switch (process)
                {
                    case SuperProcess.AverageP:
                    case SuperProcess.AveragePA:
                        res.Real = res.Real + intervals[i].Real;
                        res.Nd |= intervals[i].Nd;
                        break;
                    case SuperProcess.SummP:
                    case SuperProcess.SummPA:
                        if (DataType.LessOrEquals(DataType.Integer))
                            res.Integer += intervals[i].Integer;
                        else if (DataType == DataType.Real)
                            res.Real += intervals[i].Real;
                        else if (DataType == DataType.String) 
                            res.String = StringSummConnect(res.String, intervals[i].String);
                        res.Nd |= intervals[i].Nd;
                        break;
                    case SuperProcess.MaxP:
                    case SuperProcess.MaxPA:
                        if (intervals[i] > res) res = intervals[i].Clone(DataType);
                        break;
                    case SuperProcess.MinP:
                    case SuperProcess.MinPA:
                        if (intervals[i] < res) res = intervals[i].Clone(DataType);
                        break;
                }   
            }
            if (process == SuperProcess.AverageP || process == SuperProcess.AveragePA)
                res.Real /= intervals.Length;
            return res;
        }

        //Накапливает абсолютное значение
        private Moment ConnectAbsolute(DateTime absEnd, Moment absVal, TimeInterval baseInt, Moment baseVal)
        {
            if (absVal == null) return baseVal.Clone(baseInt.Begin);
            var absInt = new TimeInterval(absVal.Time, absEnd);
            switch (ArchiveParam.SuperProcess)
            {
                case SuperProcess.AverageA:
                case SuperProcess.AveragePA:
                    var m = absVal.Clone(DataType);
                    m.Nd |= baseVal.Nd;
                    m.Real = (m.Real * absInt.Length() + baseVal.Real * baseInt.Length()) / (absInt.Length() + baseInt.Length());
                    return m;
                case SuperProcess.SummA:
                case SuperProcess.SummPA:
                    m = absVal.Clone(DataType);
                    m.Nd |= baseVal.Nd;
                    if (m.DataType.LessOrEquals(DataType.Integer))
                        m.Integer += baseVal.Integer;
                    else if (m.DataType == DataType.Real)
                        m.Real += baseVal.Real;
                    else m.String = StringSummConnect(m.String, baseVal.String);
                    return m;
                case SuperProcess.MinA:
                case SuperProcess.MinPA:
                    return baseVal > absVal ? absVal : baseVal.Clone(absInt.Begin);
                case SuperProcess.MaxA:
                case SuperProcess.MaxPA:
                    return baseVal < absVal ? absVal : baseVal.Clone(absInt.Begin);
                case SuperProcess.FirstA:
                case SuperProcess.FirstPA:
                    return absVal;
                case SuperProcess.LastA:
                case SuperProcess.LastPA:
                    return baseVal.Clone(absInt.Begin);
            }
            return null;
        }

        //Накопление Сумма для двух строк
        private string StringSummConnect(string s1, string s2)
        {
            var set1 = s1.ToPropertyHashSet();
            var set2 = s2.ToPropertyHashSet();
            foreach (var s in set2.Values)
                set1.Add(s);
            return set1.Values.ToPropertyString();
        }
    }    
}