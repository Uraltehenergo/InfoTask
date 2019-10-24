using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал 
    public class ProviderSignal
    {
        public ProviderSignal(string signalInf, string code, DataType dataType, IProvider provider, int idInClone = 0)
        {
            Code = code;
            DataType = dataType;
            Inf = signalInf.ToPropertyDicS();
            _averageTime = Inf.Get("AverageTime", "0").ToDouble();
            Provider = provider;
            IdInClone = idInClone;
        }

        //Код
        public string Code { get; private set; }
        //Тип данных 
        private DataType _dataType;
        public DataType DataType 
        { 
            get { return _dataType; }
            protected set 
            { 
                _dataType = value;
                _isReal = DataType.LessOrEquals(DataType.Real);
            } 
        }
        //Тип данных можно записать как число
        private bool _isReal;
        //Словарь свойств
        public DicS<string> Inf { get; private set; }
        //Ссылка на провайдер
        public IProvider Provider { get; private set; }
        //Значение
        public SingleValue Value { get; set; }
        //Id в файле клона и т.п.
        public int IdInClone { get; private set; }

        //Длительность периода усреднения в секундах
        private double _averageTime;

        //Значение среза на начало периода
        public Moment BeginMoment { get; private set; }

        //Добавка мгновенных значений в список или клон
        //Возвращает количество реально добавленных значений
        //rec - рекордсет клона, если не задан, то добавляется в список
        //если forBegin, то значение не пишется в список сразу, т.к. оно предназначено для формирования среза
        public int AddMoment(Moment mom, bool forBegin = false, bool skipEqual = true, bool add10Min = true)
        {
            if (mom == null) return 0;
            if (forBegin) BeginMoment = mom.Clone();
            else
            {
                var rec = ((SourceBase)Provider).CloneRec;
                if (rec == null)
                {
                    BeginMoment = mom.Clone();
                    return MomentToList(mom, skipEqual);
                }
                if (_isReal) return MomentToClone(rec, mom.Time, mom.Real, mom.Nd, false, add10Min);    
            }
            return 0;
        }

        //Добавка мгновенных значений разного типа в список или клон
        //Возвращает количество добавленных значений
        public int AddMoment(DateTime time, bool v, int nd = 0, bool forBegin = false)
        {
            return AddMoment(new Moment(time, v, null, nd), forBegin);
        }
        public int AddMoment(DateTime time, int v, int nd = 0, bool forBegin = false)
        {
            return AddMoment(new Moment(time, v, null, nd), forBegin);
        }
        public int AddMoment(DateTime time, double v, int nd = 0, bool forBegin = false)
        {
            return AddMoment(new Moment(time, v, null, nd), forBegin);
        }
        public int AddMoment(DateTime time, DateTime v, int nd = 0, bool forBegin = false)
        {
            return AddMoment(new Moment(time, v, null, nd), forBegin);
        }
        public int AddMoment(DateTime time, string v, int nd = 0, bool forBegin = false)
        {
            return AddMoment(new Moment(time, v, null, nd), forBegin);
        }
        //Добавка мгновенных значений из объекта с указанием типа
        public int AddMoment(DataType dtype, DateTime time, object ob, int nd = 0, bool forBegin = false)
        {
            return AddMoment(new Moment(dtype, ob, time, nd), forBegin);
        }

        //Добавляет мгновенное значение в список, возвращает количество добавленных значений
        //Если skipEqual, то пропускает значения, равные предыдущим 
        private int MomentToList(Moment mv, bool skipEqual = true)
        {
            if (Value == null) Value = new SingleValue(SingleType.List);
            var list = Value.Moments;
            var last = list.Count == 0 ? null : list[list.Count - 1];
            if (!skipEqual || last == null || (last.Time < mv.Time && (last != mv || last.Nd != mv.Nd)))
            {
                list.Add(mv);
                return 1;
            }
            return 0;
        }

        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        //public int AddBegin(DateTime beginTime)//Установка среза 
        public int AddBegin()
        {
            if (BeginMoment == null) return 0;
            var rec = ((SourceBase)Provider).CloneRec;
            //if (rec == null) return MomentToList(BeginMoment.Clone(beginTime));//Установка среза 
            if (rec == null) return MomentToList(BeginMoment.Clone());
            if (_isReal) return MomentToClone(rec, BeginMoment.Time, BeginMoment.Real, BeginMoment.Nd);
            return 0;
        }

        //ab
        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        //если cloneOnly = true, то начальная точка = beginTime (а не последнему значению до beginTime) только для клона
        public int AddBegin(DateTime beginTime, bool cloneOnly = true) //Установка среза
        {
            if (BeginMoment == null) return 0;
            var rec = ((SourceBase)Provider).CloneRec;
            if (rec == null)
                if (cloneOnly) return MomentToList(BeginMoment.Clone());//Установка среза
                else return MomentToList(BeginMoment.Clone(beginTime));
            if (_isReal) return MomentToClone(rec, beginTime, BeginMoment.Real, BeginMoment.Nd);
            return 0;
        }
        //\ab

        //Формирует значение на конец периода и дополняет значения в клоне до конца периода
        public int MakeEnd(DateTime endTime)
        {
            //if (BeginMoment != null) BeginMoment = BeginMoment.Clone(endTime); //Установка среза
            var rec = ((SourceBase)Provider).CloneRec;
            if (rec != null && _time != Different.MinDate && _isReal)
                return MomentToClone(rec, endTime, _val, _nd, true);
            return 0;
        }

        //Текущие время, значение и недостоверность записи в клон
        private DateTime _time = Different.MinDate;
        private double _val = Double.NaN;
        private int _nd;

        //Добавляет мгновенное значение в клон, возвращает количество добавленных значений
        //Если withoutLast, то не добавляет само значение (только предыдущие раз в 10 минут), если add10Min, то добавляет значения не реже чем раз в 10 минут
        private int MomentToClone(RecDao rec, DateTime time, double val, int nd, bool withoutLast = false, bool add10Min = true)
        {
            int n = 0;
            if (_time != Different.MinDate && add10Min)
                while (time.Subtract(_time).TotalMinutes > 10)
                {
                    _time = _time.AddMinutes(10);
                    ToClone(rec);
                    n++;
                }
            if (_time < time && (val != _val || nd != _nd) && !withoutLast)
            {
                _time = time;
                _val = val;
                _nd = nd;
                ToClone(rec);
                n++;
            }
            return n;
        }

        private void ToClone(RecDao rec)
        {
            rec.AddNew();
            rec.Put("SignalId", IdInClone);
            rec.Put("Time", _time);
            rec.Put("Value", _val);
            rec.Put("Nd", _nd);
            rec.Update();
        }

        //Усреднение значений по сегментам (ели указано AverageTime)
        internal void CalcAverage()
        {
            if (_averageTime > 0.0 && Value != null && Value.Moments != null && Value.Moments.Count > 0)
            {
                var moms = Value.Moments;
                var res = new List<Moment>();
                var t = ((SourceBase)Provider).BeginRead;
                var endt  = ((SourceBase)Provider).EndRead;
                int i = 0;
                while (i < moms.Count && moms[i].Time <= t) i++;
                var cmom = i > 0 ? moms[i - 1].Clone(t) : moms[0].Clone(t);
                while (t < endt)
                {
                    double sum = 0;
                    int nd = 0;
                    var nextt = t.AddSeconds(_averageTime);
                    if (nextt > endt.AddMilliseconds(-1)) nextt = endt;
                    while (i < moms.Count && nextt.Subtract(moms[i].Time).TotalSeconds >= 0)
                    {
                        sum += cmom.Real * (moms[i].Time.Subtract(cmom.Time).TotalSeconds);
                        nd = Math.Max(nd, cmom.Nd);
                        cmom = moms[i++];
                    }
                    sum += moms[i-1].Real * Math.Min(nextt.Subtract(t).TotalSeconds, nextt.Subtract(moms[i-1].Time).TotalSeconds);
                    nd = Math.Max(nd, moms[i-1].Nd);
                    var tlen = nextt.Subtract(t).TotalSeconds;
                    if (tlen > 0) res.Add(new Moment(t.AddSeconds(tlen/2), sum/tlen , null, nd));
                    cmom = moms[i-1].Clone(t = nextt);
                }
                Value.Moments = res;
            }
        }
    }
}