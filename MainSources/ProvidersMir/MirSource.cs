using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "MirSource")]
    public class MirSource : SourceBase, ISource 
    {
        //Код провайдера
        public override string Code { get { return "MirSource"; } }
        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set 
            { 
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                bool e = dic.Get("IndentType", "").ToLower() != "windows";
                string server = dic["SQLServer"], db = dic["Database"];
                SqlProps = new SqlProps(server, db, e, dic["Login"], dic["Password"]);
                Hash = "SQLServer=" + server + ";Database=" + db;
            }
        }

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

        //Все соединения и так закрыты
        public void Dispose() { }

        //Настройки SQL Server
        internal SqlProps SqlProps { get; set; }

        //Проверка соединения
        public bool Check()
        {
            return Logger.Danger(TryCheck, 2, 500, "Не удалось соединиться с SQL-сервером");
        }
        private bool TryCheck()
        {
            try
            {
                using (SqlDb.Connect(SqlProps))
                    return true;
            }
            catch (Exception ex)
            {
                Logger.AddError("Не удалось соединиться с SQL-сервером", ex, "", Context);
                return false;
            }
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера" + Different.NewLine;
            if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации" + Different.NewLine;
            if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин" + Different.NewLine;
            if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных" + Different.NewLine;
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            Logger.AddError(CheckConnectionMessage = "Не удалось соединиться с SQL-сервером");
            return false;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        //Открытие соединения
        protected override bool Connect()
        {
            return true;
        }

        //Добавить сигнал в провайдер
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            return ProviderSignals.Add(code, new SignalMir(signalInf, code, dataType, this, idInClone));
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
        }

        //Словари сигналов Unit и Indcation, ключи - IdChannel
        private readonly DicI<SignalMir> _signalsIndication = new DicI<SignalMir>();
        private readonly DicI<SignalMir> _signalsUnit = new DicI<SignalMir>();

        //Подготовка провайдера, чтение значений IDCHANNEL
        public void Prepare()
        {
            _signalsIndication.Clear();
            _signalsUnit.Clear();
            using (var rec = new ReaderAdo(SqlProps, "SELECT OBJECTS.NAME_OBJECT, DEVICES.NAME_DEVICE, LIB_CHANNELS.NAME_TYPE, LIB_CHANNELS.UNIT, CHANNELS.IDCHANNEL, LIB_CHANNELS.TABLE_NAME " +
            "FROM CHANNELS INNER JOIN DEVICES ON CHANNELS.IDDEVICE = DEVICES.IDDEVICE INNER JOIN " +
            "LIB_CHANNELS ON dbo.CHANNELS.IDTYPE_CHANNEL = dbo.LIB_CHANNELS.IDTYPE_CHANNEL INNER JOIN " +
            "POINT_DEVICES ON dbo.DEVICES.IDDEVICE = dbo.POINT_DEVICES.IDDEVICE INNER JOIN " +
            "POINT_CONNECTIONS ON dbo.POINT_DEVICES.IDPOINT_CONNECTION = dbo.POINT_CONNECTIONS.IDPOINT_CONNECTION INNER JOIN " +
            "POINT_OBJ ON dbo.POINT_CONNECTIONS.IDPOINT_CONNECTION = dbo.POINT_OBJ.IDPOINT_CONNECTION INNER JOIN " +
            "OBJECTS ON dbo.POINT_OBJ.IDOBJECT = dbo.OBJECTS.IDOBJECT"))
                while (rec.Read())
                {
                    string code = rec.GetString("NAME_OBJECT") + "." + rec.GetString("NAME_DEVICE") + "." + rec.GetString("NAME_TYPE");
                    if (ProviderSignals.ContainsKey(code + ".Indication"))
                    {
                        var sig = (SignalMir)ProviderSignals[code + ".Indication"];
                        sig.IdChannel = rec.GetInt("IDCHANNEL");
                        _signalsIndication.Add(sig.IdChannel, sig);
                    }
                    if (ProviderSignals.ContainsKey(code + ".Unit"))
                    {
                        var sig = (SignalMir)ProviderSignals[code + ".Unit"];
                        sig.IdChannel = rec.GetInt("IDCHANNEL");
                        _signalsUnit.Add(sig.IdChannel, sig);
                    }
                }
        }

        //Получение диапазона архива по блокам истории
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            var ti = new TimeInterval(Different.MinDate, Different.MaxDate);
            TimeIntervals.Add(ti);
            return ti;
        }

        //Чтение данных из Historian за период
        public override void GetValues()
        {
            foreach (var sig in ProviderSignals.Values)
                sig.Value.Moments.Clear();
            using (Logger.Start(0, 40))
                Logger.Danger(() => GetVals(BeginRead.AddMinutes(-30), BeginRead, true), 3, 30000, "Ошибка при чтении из базы данных МИР");
            using (Logger.Start(40))
                Logger.Danger(() => GetVals(BeginRead, EndRead, false), 3, 30000, "Ошибка при чтении из базы данных МИР");
        }

        //Чтение среза или изменений
        private bool GetVals(DateTime beg, DateTime en, bool forBegin)
        {
            string s = forBegin ? "Срез" : "Изменения";
            string queryString = "SELECT IDCHANNEL, TIME, VALUE, VALUE_UNIT, VALUE_INDICATION FROM IZM_TII" +
                                    " WHERE (TIME >= " + beg.ToSqlString() + ") AND (TIME <=" + en.ToSqlString() +
                                    ") ORDER BY TIME";
            Logger.AddEvent("Запрос значений из базы", s);
            using (var rec = new ReaderAdo(SqlProps, queryString, 1000))
            {
                Logger.Procent = 30;
                Logger.AddEvent("Чтение списка значений", s);
                while (rec.Read())
                {
                    int id = rec.GetInt("IDCHANNEL");
                    if (_signalsIndication.ContainsKey(id))
                    {
                        var sig = _signalsIndication[id];
                        sig.AddMoment(rec.GetTime("TIME"), rec.GetDouble("VALUE_INDICATION"), 0, forBegin);
                    }
                    if (_signalsUnit.ContainsKey(id))
                    {
                        var sig = _signalsUnit[id];
                        sig.AddMoment(rec.GetTime("TIME"), rec.GetDouble("VALUE_UNIT"), 0, forBegin);
                    }
                }
                if (forBegin)
                {
                    foreach (var sig in _signalsIndication.Values)
                        sig.AddBegin(BeginRead); //ab Было без параметра
                    foreach (var sig in _signalsUnit.Values)
                        sig.AddBegin(BeginRead); //ab Было без параметра
                }
            }
            return true;
        }
    }
}
