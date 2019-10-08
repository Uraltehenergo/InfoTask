using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class CloneSource : SourceBase, ISource
    {
        public CloneSource(string name, string inf, Logger logger, bool isHandInput) : base(name, logger)
        {
            Inf = inf;
            IsHandInput = isHandInput;
             _cloneFileType = IsHandInput ? " файл значений ручного ввода" : " файл клона или архива мгновенных значений";
        }
        
        //Код провайдера
        public override string Code { get { return IsHandInput ? "HandInputSource" : "CloneSource"; } }

        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                CloneFile = dic["CloneFile"] ?? "";
                Hash = "AccessDb=" + CloneFile;
            }
        }

        //Является источником ручного ввода
        public bool IsHandInput { get; private set; }
        // Строка для формирования сообщений об ошибках
        private readonly string _cloneFileType;

        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }
        
        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        protected override void AddMenuCommands()
        {
            var m = new Dictionary<string, IMenuCommand>();
            m.Add("Выбрать файл", new DialogCommand(DialogType.OpenFile)
            {
                DialogTitle = "Выбрать" + _cloneFileType,
                ErrorMessage = "Указан недопустимый" + _cloneFileType,
                FileTables = new[] { "Objects", "Signals", "MomentsValues" }
            });
            if (IsHandInput)
                m.Add("Создать файл", new DialogCommand(DialogType.CreateFile)
                    {
                        DialogTitle = "Создание файла значений ручного ввода",
                        TemplateFile = General.GeneralDir + "CloneTemplate.accdb"
                    });
            MenuCommands.Add("CloneFile", m);
        }

        //Файл базы данных
        public string CloneFile { get; private set; }

        protected override bool Connect()
        {
            return true;
        }

        //Проверка соединения при ошибке
        public bool Check()
        {
            if (!DaoDb.Check(CloneFile, new[] {"Objects", "Signals", "MomentsValues"}))
            {
                Logger.AddError("Не найден или неправильный" + _cloneFileType, null, "", Context);
                return IsConnected = false;
            }
            return IsConnected = true;
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return inf["CloneFile"].IsEmpty() ? ("Не задан" + _cloneFileType) : "";
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            CheckConnectionMessage = "Не найден или неправильный" + _cloneFileType;
            if (!Check()) return false;
            try
            {
                var t = GetTime();
                CheckConnectionMessage = "Успешное соединение. Диапазон источника: " + t.Begin + " - " + t.End;
                return true;
            }
            catch
            {
                Logger.AddError(CheckConnectionMessage);
                return false;
            }
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        public void Dispose()
        {
        }

        //Получение диапазона клона
        public TimeInterval GetTime()
        {
            if (CloneFile.IsEmpty()) return new TimeInterval(Different.MinDate, Different.MaxDate);
            using (var sys = new SysTabl(CloneFile))
            {
                TimeIntervals.Clear();
                var t = new TimeInterval(sys.Value("BeginInterval").ToDateTime(), IsHandInput ? DateTime.Now : sys.Value("EndInterval").ToDateTime());
                TimeIntervals.Add(t);
                return t;
            }     
        }

        //Словари сигналов, ключи - Id в таблицах MomentsValues и MomentsStrValues
        private readonly DicI<ProviderSignal> _signalsId = new DicI<ProviderSignal>();
        private readonly DicI<ProviderSignal> _signalsStrId = new DicI<ProviderSignal>();

        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _signalsId.Clear();
            _signalsStrId.Clear();
        }

        //Добавление сигнала
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            if (!ProviderSignals.ContainsKey(code))
            {
                var sig = new ProviderSignal(signalInf, code, dataType, this, idInClone) {Value = new SingleValue(SingleType.List)};
                ProviderSignals.Add(code, sig);
                return sig;
            }
            return ProviderSignals[code];
        }

        //Подготовка сигналов
        public void Prepare()
        {
            try
            {
                if (CloneFile.IsEmpty()) return;
                Logger.AddEvent("Установка отметок для считываемых сигналов");
                _signalsId.Clear();
                _signalsStrId.Clear();
                using (var rec = new RecDao(CloneFile, "SELECT SignalId, FullCode, Otm FROM Signals"))
                    while (rec.Read())
                    {
                        string code = rec.GetString("FullCode");
                        if (!ProviderSignals.ContainsKey(code))
                            rec.Put("Otm", false);
                        else
                        {
                            rec.Put("Otm", true);
                            (ProviderSignals[code].DataType.LessOrEquals(DataType.Real) ? _signalsId : _signalsStrId)
                                .Add(rec.GetInt("SignalId"), ProviderSignals[code]);
                        }
                    }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при подготовке сигналов", ex);
            }
        }

        //Чтение значений
        public override void GetValues()
        {
            try
            {
                if (CloneFile.IsEmpty()) return;
                bool needCut = false;
                foreach (var sig in ProviderSignals.Values)
                {
                    sig.Value.Moments.Clear();
                    needCut |= sig.BeginMoment == null || Math.Abs(sig.BeginMoment.Time.Subtract(BeginRead).TotalSeconds) > 0.5;
                }
                DateTime beg = BeginRead;
                var t = GetTime();
                if (needCut) beg = IsHandInput ? Different.MinDate : (beg.AddMinutes(-10) <= t.Begin ? t.Begin.AddSeconds(-1) : beg.AddMinutes(-10));
                int nread = 0, nwrite = 0;
                if (_signalsId.Count > 0)
                {
                    Logger.AddEvent("Открытие рекордсета значений");
                    using (var rec = new ReaderAdo(CloneFile, "SELECT MomentsValues.SignalId as SignalId, Value, Time, Nd FROM MomentsValues INNER JOIN Signals ON Signals.SignalId = MomentsValues.SignalId " +
                                                              "WHERE (Signals.Otm=True) AND (Time >= " + beg.ToAccessString() + ") AND (Time <= " + EndRead.ToAccessString() + ") ORDER BY Time, MomentsValues.SignalId"))
                    {
                        Logger.AddEvent("Чтение значений из рекордсета", _signalsId.Count + " сигналов");
                        while (rec.Read() && rec.GetTime("Time") <= BeginRead)
                        {
                            var sig = _signalsId[rec.GetInt("SignalId")];
                            sig.AddMoment(new Moment(sig.DataType, rec.GetDouble("Value"), rec.GetTime("Time"), rec.GetInt("Nd")), true); 
                            nread++;
                        }
                        foreach (var sig in _signalsId.Values) 
                            nwrite += sig.AddBegin();
                        while (!rec.EOF)
                        {
                            nread++;
                            var sig = _signalsId[rec.GetInt("SignalId")];
                            nwrite += sig.AddMoment(new Moment(sig.DataType, rec.GetDouble("Value"), rec.GetTime("Time"), rec.GetInt("Nd")));
                            rec.Read();
                        }
                    }    
                }

                if (_signalsStrId.Count > 0)
                {
                    Logger.AddEvent("Открытие рекордсета строковых значений");
                    using (var rec = new ReaderAdo(CloneFile, "SELECT MomentsStrValues.SignalId as SignalId, StrValue, TimeValue, Time, Nd FROM MomentsStrValues INNER JOIN Signals ON Signals.SignalId = MomentsStrValues.SignalId " +
                                                              "WHERE (Signals.Otm=True) AND (Time >= " + beg.ToAccessString() + ") AND (Time < " + EndRead.ToAccessString() + ") ORDER BY Time, MomentsStrValues.SignalId"))
                        if (rec.HasRows())
                        {
                            Logger.AddEvent("Чтение строковых значений из рекордсета", _signalsStrId.Count + " сигналов");
                            while (rec.Read() && rec.GetTime("Time") <= BeginRead)
                            {
                                var sig = _signalsStrId[rec.GetInt("SignalId")];
                                if (sig.DataType == DataType.Time)
                                    sig.AddMoment(rec.GetTime("Time"), rec.GetTime("TimeValue"), rec.GetInt("Nd"), true);
                                else sig.AddMoment(rec.GetTime("Time"), rec.GetString("StrValue"), rec.GetInt("Nd"), true);
                                nread++;
                            }

                            foreach (var sig in _signalsStrId.Values)
                                nwrite += sig.AddBegin();
                            while (!rec.EOF)
                            {
                                nread++;
                                var sig = _signalsStrId[rec.GetInt("SignalId")];
                                if (sig.DataType == DataType.Time)
                                    nwrite += sig.AddMoment(rec.GetTime("Time"), rec.GetTime("TimeValue"), rec.GetInt("Nd"));
                                else nwrite += sig.AddMoment(rec.GetTime("Time"), rec.GetString("StrValue"), rec.GetInt("Nd"));
                                rec.Read();
                            }
                        }    
                }
                if (CloneRec != null)
                    foreach (var sig in ProviderSignals.Values)
                        nwrite += sig.MakeEnd(EndRead);

                Logger.AddEvent("Чтение значений завершено", nread + " значений прочитано, " + nwrite + " значений сформировано");
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при попытке прочитать" + _cloneFileType, ex);
            }
        }
    }
}
