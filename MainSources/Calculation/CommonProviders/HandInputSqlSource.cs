using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class HandInputSqlSource : SourceBase, ISource
    {
        public HandInputSqlSource(string name, string inf, Logger logger)
            : base(name, logger)
        {
            Inf = inf;
        }
        
        //Код провайдера
        public override string Code { get { return "HandInputSqlSource"; }}

        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                bool e = dic.Get("IndentType", "").ToLower() != "windows";
                string server = dic["SQLServer"], db = dic["Database"];
                SqlProps = new SqlProps(server, db, e, dic["Login"], dic["Password"]);
                Hash = "SQLServer=" + server + ";Database=" + db;
            }
        }

        //Настройки на базу ручного ввода SqlServer
        public SqlProps SqlProps { get; private set; }
        
        //Задание комманд, вызываемых из меню
        protected override void AddMenuCommands() { }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            try
            {
                if (propname == "Database" && props.ContainsKey("SQLServer") && !props["SQLServer"].IsEmpty() && (props["IndentType"].ToLower() == "windows" || (props.ContainsKey("Login") && !props["Login"].IsEmpty())))
                    return SqlDb.SqlDatabasesList(props["SQLServer"], props["IndentType"].ToLower() != "windows", props["Login"], props["Password"]);
            }
            catch { }
            return new List<string>();
        }

        protected override bool Connect()
        {
            return true;
        }

        //Проверка соединения при ошибке
        public bool Check()
        {
            try
            {
                using (SqlDb.Connect(SqlProps))
                    return IsConnected = true;
            }
            catch (Exception ex)
            {
                Logger.AddError("Не удалось соединиться с SQL-сервером", ex, "", Context);
                return IsConnected = false;
            }
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера\n";
            if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации\n";
            if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин\n";
            if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных\n";
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (!Check())
            {
                Logger.AddError(CheckConnectionMessage = "Не удалось соединиться с SQL-сервером");
                return false;
            }
            CheckConnectionMessage = "Успешное соединение";
            return true;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        public void Dispose()
        {
        }

        //Получение диапазона клона
        public TimeInterval GetTime()
        {
            try
            {
                using (var rec = new DataSetSql(SqlProps, "SysTabl"))
                {
                    rec.FindFirst("ParamName", "BeginInterval");
                    TimeIntervals.Clear();
                    var t = new TimeInterval(rec.GetString("ParamValue").ToDateTime(), DateTime.Now);
                    TimeIntervals.Add(t);
                    return t;
                }
            }
            catch { return new TimeInterval(Different.MinDate, Different.MaxDate); }
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
                Logger.AddEvent("Подготовка сигналов");
                _signalsId.Clear();
                _signalsStrId.Clear();
                using (var rec = new DataSetSql(SqlProps, "SELECT SignalId, FullCode, Otm FROM Signals"))
                    while (rec.Read())
                    {
                        string code = rec.GetString("FullCode");
                        if (ProviderSignals.ContainsKey(code))
                            (ProviderSignals[code].DataType.LessOrEquals(DataType.Real) ? _signalsId : _signalsStrId)
                                .Add(rec.GetInt("SignalId"), ProviderSignals[code]);
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
                bool needCut = false;
                foreach (var sig in ProviderSignals.Values)
                {
                    sig.Value.Moments.Clear();
                    needCut |= sig.BeginMoment == null || Math.Abs(sig.BeginMoment.Time.Subtract(BeginRead).TotalSeconds) > 0.5;
                }
                DateTime beg = needCut ? Different.MinDate : BeginRead;
                int nread = 0, nwrite = 0;
                if (_signalsId.Count > 0)
                {
                    Logger.AddEvent("Открытие рекордсета значений");
                    using (var rec = new ReaderAdo(SqlProps, "SELECT MomentsValues.SignalId as SignalId, Value, Time, Nd FROM MomentsValues INNER JOIN Signals ON Signals.SignalId = MomentsValues.SignalId " +
                                                              "WHERE (Time >= " + beg.ToSqlString() + ") AND (Time <= " + EndRead.ToSqlString() + ") ORDER BY Time, MomentsValues.SignalId"))
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
                    using (var rec = new ReaderAdo(SqlProps, "SELECT MomentsStrValues.SignalId as SignalId, StrValue, TimeValue, Time, Nd FROM MomentsStrValues INNER JOIN Signals ON Signals.SignalId = MomentsStrValues.SignalId " +
                                                              "WHERE (Time >= " + beg.ToSqlString() + ") AND (Time < " + EndRead.ToSqlString() + ") ORDER BY Time, MomentsStrValues.SignalId"))
                        if (rec.HasRows())
                        {
                            Logger.AddEvent("Чтение строковых значений из рекордсета", _signalsStrId.Count + " сигналов");
                            while (rec.Read() && rec.GetTime("Time") <= BeginRead)
                            {
                                var sigId = rec.GetInt("SignalId");
                                if (!_signalsStrId.ContainsKey(sigId))
                                    Logger.AddEvent("Не понятный SignalId", sigId.ToString());
                                var sig = _signalsStrId[sigId];
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
                Logger.AddError("Ошибка при попытке прочитать значения ручного ввода", ex);
            }
        }
    }
}