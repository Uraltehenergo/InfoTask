using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "WonderwareHistorianSource")]
    public class WonderwareHistorianSource : SourceBase, ISource 
    {
        //public WonderwareHistorianSource(string name, string inf, Logger logger) : base(name, logger)
        //{
        //    Inf = inf;
        //    IsConnected = true;
        //}

        //Код провайдера
        public override string Code { get { return "WonderwareHistorianSource"; } }
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

        //Словарь объектов по TagName
        private readonly Dictionary<string, ObjectWonderware> _objects = new Dictionary<string, ObjectWonderware>();
        //Список объектов, разбитый на блоки
        private readonly List<List<ProviderObject>> _parts = new List<List<ProviderObject>>();

        //Подготовка строки запроса
        public void Prepare() { }

        //Добавить сигнал в провайдер
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var sig = new SignalWonderware(signalInf, code, dataType, this, idInClone);
            if (!_objects.ContainsKey(sig.TagName))
            {
                var ob = new ObjectWonderware(sig.TagName);
                _objects.Add(sig.TagName, ob);
                if (_parts.Count == 0 || _parts[_parts.Count - 1].Count == 500)
                    _parts.Add(new List<ProviderObject>());
                _parts[_parts.Count - 1].Add(ob);
            }
            var ret = _objects[sig.TagName].AddSignal(sig);
            if (ret == sig) ProviderSignals.Add(sig.Code, sig);
            return ret;
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _objects.Clear();
        }
        
        //Открытие соединения
        protected override bool Connect()
        {
            return true;
        }

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

        //Получение диапазона архива по блокам истории
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            DateTime mind = Different.MaxDate, maxd = Different.MinDate;
            DateTime mint = Different.MaxDate, maxt = Different.MinDate;
            try
            {
                using (var rec = new ReaderAdo(SqlProps, "SELECT FromDate, ToDate FROM v_HistoryBlock ORDER BY FromDate, ToDate DESC"))
                    while (rec.Read())
                    {
                        var fromd = rec.GetTime("FromDate");
                        var tod = rec.GetTime("ToDate");
                        if (fromd < mind) mind = fromd;
                        if (fromd.Subtract(maxt).TotalMinutes > 1)
                        {
                            if (maxt != Different.MinDate) TimeIntervals.Add(new TimeInterval(mint, maxt));
                            mint = fromd;
                        }
                        if (maxd < tod) maxd = tod;
                        if (maxt < tod) maxt = tod;
                    }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при получении диапазона источника", ex);
            }
            if (mind == Different.MaxDate && maxd == Different.MinDate)
                return new TimeInterval(Different.MinDate, Different.MaxDate);
            Logger.AddEvent("Диапазон источника определен", mind + " - " + maxd);
            return new TimeInterval(mind, maxd);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //Рекордсет со значениями
        private ReaderAdo _rec;

        //Запрос значений по одному блоку сигналов
        private bool ReadPartValues(List<ProviderObject> part)
        {
            string queryString = "SELECT TagName, DateTime = convert(nvarchar, DateTime, 21), Value, vValue, Quality, QualityDetail FROM History WHERE  TagName IN (";
            for (var n = 0; n < part.Count; n++)
            {
                if (n != 0) queryString += ", ";
                var ob = (ObjectWonderware) part[n];
                queryString += "'" + ob.TagName + "'";
            }
            queryString += ") AND wwRetrievalMode = 'Delta'";
            bool isCut = BeginRead.ToSqlString() == EndRead.ToSqlString();
            queryString += " AND DateTime >= " + BeginRead.ToSqlString() + " AND DateTime " + (isCut ? "<=" : "<") + EndRead.ToSqlString() + " ORDER BY DateTime";
            Logger.AddEvent("Запрос значений из Historian", part.Count + " тегов");
            _rec = new ReaderAdo(SqlProps, queryString, 10000);
            return true;
        }

        //Формирование значений по одному блоку сигналов
        private KeyValuePair<int, int> FormPartValues()
        {
            int nread = 0, nwrite = 0;
            using (_rec)
                while (_rec.Read())
                {
                    string code = "";
                    try
                    {
                        code = _rec.GetString("TagName");
                        if (_objects.ContainsKey(code))
                        {
                            var ob = _objects[code];
                            DateTime time = _rec.GetTime("DateTime");
                            int nd = _rec.GetInt("QualityDetail") == 192 ? 0 : 1;
                            //int nd = rec.GetInt("Quality");
                            var d = _rec.GetDouble("Value");
                            nread++;
                            if (ob.ValueSignal != null)
                            {
                                switch (ob.DataType)
                                {
                                    case DataType.Boolean:
                                        nwrite += ob.ValueSignal.AddMoment(time, d != 0, nd);
                                        break;
                                    case DataType.Real:
                                        nwrite += ob.ValueSignal.AddMoment(time, d, nd);
                                        break;
                                    case DataType.Integer:
                                        nwrite += ob.ValueSignal.AddMoment(time, Convert.ToInt32(d), nd);
                                        break;
                                    default: //String
                                        nwrite += ob.ValueSignal.AddMoment(new Moment(time, _rec.GetString("vValue"), null, nd));
                                        break;
                                }
                            }
                            else
                            {
                                int i = Convert.ToInt32(d);
                                foreach (var b in ob.BitSignals.Values)
                                    nwrite += b.AddMoment(time, i.GetBit(b.Bit), nd);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddErrorObject(code, "Ошибка при чтении значений из рекордсета", ex);
                    }
                }
            return new KeyValuePair<int, int>(nread, nwrite);
        }

        //Чтение данных из Historian за период
        public override void GetValues()
        {
            using (Logger.Start(0))
            {
                foreach (var ps in ProviderSignals.Values)
                    ps.Value.Moments.Clear();
                ReadValuesByParts(ReadPartValues, FormPartValues, _parts, 0, 0);
            }
        }
    }
}
