using System;
using System.Collections.Generic;
using System.Net;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public abstract class ArchiveSourceBase : SourceBase, ISource
    {
        protected ArchiveSourceBase(string name, string inf, Logger logger) : base(name, logger)
        {
            Inf = inf;
        }

        //Провайдер архива
        protected IArchive Archive { get; set; }

        public abstract string Inf { get; set; }

        protected override bool Connect()
        {
            return true;
        }

        //Проверка соединения при ошибке
        public bool Check()
        {
            return Archive.Check();
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            bool e = Archive.CheckConnection();
            CheckConnectionMessage = Archive.CheckConnectionMessage;
            if (e)
            {
                var t = GetTime();
                CheckConnectionMessage = "Успешное соединение";
                if (t.Begin != Different.MaxDate && t.End != Different.MinDate)
                    CheckConnectionMessage += ". Диапазон источника: " + t.Begin + " - " + t.End;
            }
            return e;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        public abstract string CheckSettings(Dictionary<string, string> infDic, Dictionary<string, string> nameDic);
        public abstract List<string> ComboBoxList(Dictionary<string, string> props, string propname);

        public void Dispose()
        {
            if (Archive != null) Archive.Dispose();
        }

        //Получение диапазона архива 
        public TimeInterval GetTime()
        {
            var pdic = Archive.ReadProjects(ReportType.Calc);
            var t = new TimeInterval(Different.MaxDate, Different.MinDate);
            foreach (var pr in pdic.Values)
            {
                var dic = Archive.ReadRanges(pr.Code, ReportType.Calc);
                AddIntervalTime(dic, IntervalType.Day, t);
                AddIntervalTime(dic, IntervalType.Hour, t);
                AddIntervalTime(dic, IntervalType.Base, t);
                AddIntervalTime(dic, IntervalType.Moments, t);
                AddIntervalTime(dic, IntervalType.AbsoluteDay, t);    
            }
            TimeIntervals.Clear();
            TimeIntervals.Add(t);
            BeginTime = t.Begin;
            EndTime = t.End;
            return t;
        }

        //Добавляет интервал в вычисление диапазона источника
        private void AddIntervalTime(Dictionary<IntervalType, TimeInterval> dic, IntervalType type, TimeInterval t)
        {
            if (dic.ContainsKey(type))
            {
                if (dic[type].Begin < t.Begin) t.Begin = dic[type].Begin;
                if (dic[type].End > t.End) t.End = dic[type].End;
            }
        }

        //Отчет для запроса данных
        private ArchiveReport _report;
        //Архивные параметры отчета для получения данных, ключи - код проекта и код параметра
        private readonly DicS<DicS<ArchiveReportParam>> _reportParams = new DicS<DicS<ArchiveReportParam>>();
        //Словарь сигналов ключи - код проекта, код параметра и тип интервала
        private readonly DicS<DicS<Dictionary<IntervalType, ProviderSignal>>> _signalsDic = new DicS<DicS<Dictionary<IntervalType, ProviderSignal>>>();
        //Используемые типы интервалов
        private readonly HashSet<IntervalType> _intervalTypes = new HashSet<IntervalType>();

        //Получение диапазона клона
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _reportParams.Clear();
            _signalsDic.Clear();
            _intervalTypes.Clear();
        }

        //Добавить сигнал
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var dic = signalInf.ToPropertyDicS();
            var proj = dic["Project"];
            var ocode = dic["CodeObject"];
            var itype = dic["IntervalType"].ToIntervalType();
            var sp = dic["SuperProcessType"].ToSuperProcess();
            if (!_intervalTypes.Contains(itype)) _intervalTypes.Add(itype);
            if (!_signalsDic.ContainsKey(proj))
            {
                _signalsDic.Add(proj, new DicS<Dictionary<IntervalType, ProviderSignal>>());
                _reportParams.Add(proj, new DicS<ArchiveReportParam>());
            }
            var projdic = _signalsDic[proj];
            var repdic = _reportParams[proj];
            if (!projdic.ContainsKey(ocode))
            {
                projdic.Add(ocode, new Dictionary<IntervalType, ProviderSignal>());
                repdic.Add(ocode, new ArchiveReportParam(ocode, proj, dataType, sp, CalcParamType.Class, new List<IntervalType>()));
            }
            if (projdic[ocode].ContainsKey(itype)) return projdic[ocode][itype];
            var sig = new ProviderSignal(signalInf, code, dataType, this, idInClone) {Value = new SingleValue(SingleType.List)};
            projdic[ocode].Add(itype, sig);
            ProviderSignals.Add(code, sig);
            repdic[ocode].Queries.Add(itype, new ArchiveQueryValues());
            return sig;
        }

        //Подготовка сигналов
        public void Prepare()
        {
            try
            {
                string reportCode = "Source_" + Dns.GetHostName();
                _report = new ArchiveReport("Source_" + Logger.FullThreadName, reportCode, ReportType.Source, DateTime.Now);
                foreach (var proj in _reportParams.Keys)
                    _report.Projects.Add(proj, _reportParams[proj]);    
                Archive.PrepareReport(_report);
                _endPrevious = Different.MinDate;
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при подготовке параметров для чтения из архива", ex);
            }
        }

        //Конец периода предыдущего чтения
        private DateTime _endPrevious;

        //Очистка значений
        private void ClearValues()
        {
            foreach (var sig in ProviderSignals.Values)
                sig.Value.Moments.Clear();
            _report.IntervalsForRead.Clear();
            foreach (var proj in _reportParams.Keys)
                foreach (var code in _reportParams[proj].Keys)
                    foreach (var q in _reportParams[proj][code].Queries.Values)
                    {
                        if (q.Intervals != null) q.Intervals.Clear();
                        q.Interval = null;
                        if (q.SingleValue != null)
                        {
                            if (q.SingleValue.Moments != null) q.SingleValue.Moments.Clear();
                            q.SingleValue.Moment = null;
                        }
                    }
        }

        //Чтение значений
        public override void GetValues()
        {
            try
            {
                ClearValues();
                foreach (var it in _intervalTypes)
                    _report.IntervalsForRead.Add(new ArchiveInterval(it, it != IntervalType.Moments || BeginRead == _endPrevious ? BeginRead : BeginRead.AddHours(-1), EndRead));
                Archive.ReadReport(_report);
                foreach (var proj in _reportParams.Keys)
                    foreach (var code in _reportParams[proj].Keys)
                        foreach (var q in _reportParams[proj][code].Queries)
                        {
                            var sv = q.Value.SingleValue;
                            var sig = _signalsDic[proj][code][q.Key];
                            if (sv.Moment != null)
                                sig.AddMoment(sv.Moment, false, false, false);
                            else if ((q.Value.Interval == null || q.Value.Interval.Type != IntervalType.Moments) && sv.Moments != null)
                                for (int i = 0; i < sv.Moments.Count; i++ )
                                    sig.AddMoment(q.Key != IntervalType.AbsoluteDay ? sv.Moments[i] : sv.Moments[i].Clone(q.Value.Intervals[i].End), false, false, false);
                            else
                            {
                                if (sv.Moments != null && sv.Moments.Count > 0)
                                {
                                    int i = 0;
                                    while (sv.Moments[i].Time <= BeginRead)
                                        sig.AddMoment(sv.Moments[i++], true);
                                    sig.AddBegin();
                                    while (i < sv.Moments.Count)
                                        sig.AddMoment(sv.Moments[i++]);
                                    sig.MakeEnd(EndRead);
                                }
                            }
                        }
                _endPrevious = EndRead;
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при получении данных из архива", ex);
            }
        }
    }
}
