using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;


namespace ProvidersLogika
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "PrologSource")]
    public class PrologSource : SourceBase, ISource
    {
        //Код провайдера
        public override string Code { get { return "PrologSource"; } }

        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                _db = dic["Db"] ?? "";
                Hash = "AccessDb=" + _db;
            }
        }

        //База данных архива
        private string _db;

        protected override bool Connect()
        {
            return true;
        }

        //Проверка соединения при ошибке
        public bool Check()
        {
            if (!DaoDb.Check(_db, new[] { "NODES", "ABONENTS" }))
            {
                Logger.AddError("Не найден или неправильный файл архива программы \"Пролог\"", null, "", Context);
                return IsConnected = false;
            }
            return IsConnected = true;
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return inf["Db"].IsEmpty() ? "Не задан файл архива программы \"Пролог\"" : "";
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            CheckConnectionMessage = "";
            if (Check()) return true;
            CheckConnectionMessage = "Не найден или неправильный файл архива программы \"Пролог\"";
            return false;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        public void Dispose() {}

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
                DialogTitle = "Выбрать файл архива программы \"Пролог\"",
                ErrorMessage = "Указан недопустимый файл архива программы \"Пролог\"",
                FileTables = new[] { "NODES", "ABONENTS" }
            });
            MenuCommands.Add("Db", m);
        }

        //Получение диапазона источника
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            var ti = new TimeInterval(Different.MinDate, Different.MaxDate);
            TimeIntervals.Add(ti);
            return ti;
        }
        //Подготовка
        public void Prepare() { }

        //Словари объектов текущих и итоговых, первый ключ - код таблицы, второй ключ - id объекта
        private readonly DicS<DicI<ObjectProlog>> _objects = new DicS<DicI<ObjectProlog>>();
        private readonly DicS<DicI<ObjectProlog>> _totals = new DicS<DicI<ObjectProlog>>();
        //Словари объектов текущих и итоговых, ключ - id объекта
        private readonly DicI<ObjectProlog> _objectsId = new DicI<ObjectProlog>();
        private readonly DicI<ObjectProlog> _totalsId = new DicI<ObjectProlog>();
        
        //Добавить сигнал в источник
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var sig = new ProviderSignal(signalInf, code, dataType, this, idInClone);
            ProviderSignals.Add(code, sig);
            string tableName = sig.Inf["TableName"];
            int nodeId = sig.Inf.GetInt("NodeId");
            string prop = sig.Inf["Prop"];
            if (prop.IsEmpty())
            {
                if (!_objects.ContainsKey(tableName))
                    _objects.Add(tableName, new DicI<ObjectProlog>());
                var ob = _objects[tableName].Add(nodeId, new ObjectProlog(tableName, nodeId, prop));
                _objectsId.Add(nodeId, ob);
                return ob.AddSignal(sig);
            }
            if (!_totals.ContainsKey(tableName))
                _totals.Add(tableName, new DicI<ObjectProlog>());
            var obt = _totals[tableName].Add(nodeId, new ObjectProlog(tableName, nodeId, prop));
            _totalsId.Add(nodeId, obt);
            return obt.AddSignal(sig);
        }

        //Очистить список сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _objects.Clear();
            _totals.Clear();
            _objectsId.Clear();
            _totalsId.Clear();
        }

        //Чтение значений
        public override void GetValues()
        {
            try
            {
                DateTime beg = BeginRead.AddMinutes(-BeginRead.Minute).AddSeconds(-BeginRead.Second-1);
                DateTime en = EndRead.AddSeconds(1);
                using (var db = new DaoDb(_db))
                {
                    foreach (var tableName in _objects.Keys)
                    {
                        Logger.AddEvent("Чтение значений из таблицы " + tableName + "_ARCHIVE");
                        using (var rec = new RecDao(db, "SELECT * FROM " + tableName + "_ARCHIVE " +
                                                        "WHERE (TYPE = 0) AND (Время >= " + beg.ToAccessString() + ") AND (Время <= " + en.ToAccessString() + ")"))
                            while (rec.Read())
                            {
                                int id = rec.GetInt("PARENT_ID");
                                if (_objectsId.ContainsKey(id))
                                {
                                    var ob = _objectsId[id];
                                    foreach (var sigCode in ob.Signals.Keys)
                                        ob.Signals[sigCode].AddMoment(rec.GetTime("Время"), rec.GetDouble(sigCode));
                                }
                            }
                    }
                    foreach (var tableName in _totals.Keys)
                    {
                        Logger.AddEvent("Чтение значений из таблицы " + tableName + "_TOTALS");
                        using (var rec = new RecDao(db, "SELECT * FROM " + tableName + "_TOTALS " +
                                                        "WHERE (Время >= " + beg.ToAccessString() + ") AND (Время <= " + en.ToAccessString() + ")"))
                            while (rec.Read())
                            {
                                int id = rec.GetInt("PARENT_ID");
                                if (_totalsId.ContainsKey(id))
                                {
                                    var ob = _totalsId[id];
                                    foreach (var sigCode in ob.Signals.Keys)
                                        ob.Signals[sigCode].AddMoment(rec.GetTime("Время"), rec.GetDouble(sigCode));
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при чтении данных из файла программы Пролог", ex);
            }
        }
    }
}
