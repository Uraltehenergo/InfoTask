using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "OvationSource")]
    public class OvationSource : SourceBase, ISource
    {
        //name - имя провайдера, inf - строка свойств-
        //public OvationSource(string name, string inf, Logger logger) : base(name, logger)
        //{
        //    Inf = inf;
        //}

        //Код провайдера
        public override string Code { get { return "OvationSource"; } }
        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                DataSource = dic["DataSource"];
                Hash = "OvationHistorian=" + DataSource;
            }
        }

        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        protected override void AddMenuCommands() { }
        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }
        
        //Имя дропа
        internal string DataSource;
        //Соединение с провайдером Historian
        private OleDbConnection _connection;
        //Рекордсет получения данных
        private OleDbDataReader _histReader;

        //Открытие соединения
        protected override bool Connect()
        {
            try
            {
                try { if (_histReader != null) _histReader.Close(); } catch { }
                _connection.Close();
                _connection.Dispose();
            }
            catch { }
            try
            {
                Logger.AddEvent("Соединение с Historian");
                _connection = new OleDbConnection("Provider=OvHOleDbProv.OvHOleDbProv.1;Persist Security Info=True;User ID='';Data Source=" + DataSource + ";Location='';Mode=ReadWrite;Extended Properties=''");
                _connection.Open();
                return IsConnected = _connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка соединения с Historian", ex, "", Context);
                return IsConnected = false;
            }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public bool Check()
        {
            return Logger.Danger(Connect, 2, 500, "Не удалось соединиться с Historian");
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return "";
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение с Historian";
                return true;
            }
            Logger.AddError(CheckConnectionMessage = "Ошибка соединения с Historian");
            return false;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        //Освобождение ресурсов, занятых провайдером
        public void Dispose()
        {
            try { if (_histReader != null) _histReader.Close(); } catch { }
            try { _connection.Close(); } catch { }
        }
       
        //Словарь объектов по Id
        private readonly DicI<ObjectOvation> _objectsId = new DicI<ObjectOvation>();
        //Список объектов по блокам, в каждом не более 200
        private readonly List<List<ProviderObject>> _parts = new List<List<ProviderObject>>();

        //Добавить сигнал
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone)
        {
            var sig = new SignalOvation(signalInf, code, dataType, this, idInClone);
            ObjectOvation ob;
            if (!_objectsId.ContainsKey(sig.Id))
            {
                ob = new ObjectOvation(sig.Id, code);
                _objectsId.Add(sig.Id, ob);
                if (_parts.Count == 0 || _parts[_parts.Count - 1].Count == 200)
                    _parts.Add(new List<ProviderObject>());
                _parts[_parts.Count - 1].Add(_objectsId[sig.Id]);
            }
            ob = _objectsId[sig.Id];
            if (sig.IsState) //Слово состояния
            {
                if (ob.StateSignal == null) ProviderSignals.Add(sig.Code, sig);
                return ob.StateSignal ?? (ob.StateSignal = sig);
            }
            if (sig.Bit == -1)//Аналоговый или дискретный
            {
                if (ob.ValueSignal == null) ProviderSignals.Add(sig.Code, sig);
                return ob.ValueSignal ?? (ob.ValueSignal = sig);
            }
            if (!ob.BitSignals.ContainsKey(sig.Bit))//Бит упакованного 
            {
                ProviderSignals.Add(sig.Code, sig);
                ob.BitSignals.Add(sig.Bit, sig);
            }
            return ob.BitSignals[sig.Bit];
        }

        //Удалить все сигналы
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _objectsId.Clear();
            _parts.Clear();
        }

        //Подготовка источника перед расчетом
        public void Prepare() { }

        //Получение времени источника
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            var t = new TimeInterval(Different.MinDate.AddYears(1), DateTime.Now);
            TimeIntervals.Add(t);
            return t;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Чтение значений

        //Параметры текущего запроса
        private DateTime _b, _e;
        
        //Получает значение из текущей строчки рекордсета
        private double RMean(IDataReader rec)
        {
            if (!DBNull.Value.Equals(rec["F_VALUE"]))
                return Convert.ToDouble(rec["F_VALUE"]);
            return IMean(rec);
        }
        private int IMean(IDataReader rec)
        {
            if (!DBNull.Value.Equals(rec["RAW_VALUE"]))
                return Convert.ToInt32(rec["RAW_VALUE"]);
            return 0;
        }

        //Получает слово состяния из текущей строчки рекордсета
        private int Stat(IDataReader rec)
        {
            if (DBNull.Value.Equals(rec["STS"])) return 0;
            return Convert.ToInt32(rec["STS"]);
        }

        //Получает недостоверность из текущей строчки рекордсета
        private int Nd(IDataReader rec)
        {
            //Недостоверность 8 и 9 бит, 00 - good, 01 - fair(имитация), 10 - poor(зашкал), 11 - bad
            if (DBNull.Value.Equals(rec["STS"]) || (DBNull.Value.Equals(rec["F_VALUE"]) && DBNull.Value.Equals(rec["RAW_VALUE"])))
                return 1;
            int state = Convert.ToInt32(rec["STS"]);
            return state.GetBit(8) && state.GetBit(9) ? 1 : 0;
        }

        //Получает время из текущей строчки рекордсета
        private DateTime Time(IDataReader rec)
        {
            var time = Convert.ToDateTime(rec["TIMESTAMP"]);
            if (!DBNull.Value.Equals(rec["TIME_NSEC"]))
                time = time.AddMilliseconds(Convert.ToInt32(rec["TIME_NSEC"]) / 1000000.0);
            time = time.ToLocalTime();
            return time;
        }

        //Запрос значений из Historian по списку сигналов и интервалу
        private bool ReadPart(IEnumerable<ProviderObject> objects, DateTime beg, DateTime en)
        {
            string s = "";
            foreach (ObjectOvation ob in objects)
            {
                if (s != "") s += " or ";
                s += "(ID=" + ob.Id + ")";
            }
            s = "(" + s + ") and ";
            var cmd = new OleDbCommand
            {
                Connection = _connection,
                CommandText = "select ID, TIMESTAMP, TIME_NSEC, F_VALUE, RAW_VALUE, STS from PT_HF_HIST " +
                                "where " + s + " (TIMESTAMP >= " + beg.ToOvationString() + ") and (TIMESTAMP <= " + en.ToOvationString() + ") order by TIMESTAMP, TIME_NSEC",
                CommandType = CommandType.Text
            };
            _histReader = cmd.ExecuteReader();
            if (en.Subtract(beg).TotalMinutes > 59 && !_histReader.HasRows)
            {
                Logger.AddError("Значения из источника не получены", null, beg + " - " + en +"; " + objects.First().Inf + " и др.", "", false);
                return IsConnected = false;
            }
            return true;
        }

        //Считывает значения из рекордсета _histReader, forBegin - значения для среза 
        //Возвращает количество прочитанных значений и сформированных значений
        private KeyValuePair<int, int> FormPart(bool forBegin)
        {
            int nread = 0, nwrite = 0;
            while (_histReader.Read())
            {
                ObjectOvation ob = null;
                try
                {
                    nread++;
                    var id = Convert.ToInt32(_histReader["Id"]);
                    if (_objectsId.ContainsKey(id))
                    {
                        ob = _objectsId[id];
                        //DateTime t = forBegin ? BeginRead : Time(_histReader); //Установка среза
                        DateTime t = Time(_histReader); 
                        int nd = Nd(_histReader);
                        if (ob.StateSignal != null)
                            nwrite += ob.StateSignal.AddMoment(t, Stat(_histReader), 0, forBegin);
                        if (ob.ValueSignal != null)
                            //правка(28.09.2018, добавлено преобразование к булевскому типу )
                            if (ob.DataType == DataType.Boolean)
                                nwrite += ob.ValueSignal.AddMoment(t, Convert.ToBoolean(RMean(_histReader)), nd, forBegin);
                            else if (ob.DataType == DataType.Integer)
                                nwrite += ob.ValueSignal.AddMoment(t, Convert.ToInt32(RMean(_histReader)), nd, forBegin);
                            else
                                nwrite += ob.ValueSignal.AddMoment(t, RMean(_histReader), nd, forBegin);
                        if (ob.BitSignals != null && ob.BitSignals.Count != 0)
                        {
                            int im = IMean(_histReader);
                            foreach (var b in ob.BitSignals.Keys)
                                nwrite += ob.BitSignals[b].AddMoment(t, im.GetBit(b), nd, forBegin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddErrorObject(ob == null ? "" : ob.Inf, "Ошибка при чтении значений из рекордсета. " + _b + " - " + _e, ex);
                }
            }
            return new KeyValuePair<int, int>(nread, nwrite);
        }

        //Вызовы функция для чтения и формирования значений среза и изменений
        private bool GetValuesRecordset(List<ProviderObject> part)
        {
            return ReadPart(part, _b, _e);
        }
        private KeyValuePair<int, int> FormObjectBegin()
        {
            return FormPart(true);
        }
        private KeyValuePair<int, int> FormObjectChanges()
        {
            return FormPart(false);
        }

        //Чтение значений из источника
        public override void GetValues()
        {
            try
            {
                if (!IsConnected && !Connect()) return;
                Logger.AddEvent("Чтение значений из Historian", _objectsId.Count + " объектов");
                foreach (var ps in Signals.Values)
                    ps.Value.Moments.Clear();
                var lens = new[] { 4, 61 };
                for (int i = 0; i < lens.Length; i++)
                {
                    var bparts = new List<List<ProviderObject>>();
                    foreach (var ob in _objectsId.Values)
                        if (!ob.HasBegin(BeginRead))
                        {
                            if (bparts.Count == 0 || bparts[bparts.Count-1].Count == 200)
                                bparts.Add(new List<ProviderObject>());
                            bparts[bparts.Count-1].Add(ob);
                        }
                    if (bparts.Count > 0)
                    {
                        Logger.AddEvent("Получение среза значений по " + lens[i] + " минутам");
                        _b = BeginRead.AddMinutes(-lens[i]);
                        _e = BeginRead;
                        using (Logger.Start(i*20-20, i*20))
                            ReadValuesByParts(GetValuesRecordset, FormObjectBegin, bparts);
                    }
                }
                foreach (var sig in ProviderSignals.Values)
                    if (sig.BeginMoment != null)
                        NumWrite += sig.AddBegin(BeginRead); //ab Добавлен параметр BeginRead

                Logger.AddEvent("Получение изменений значений", _objectsId.Values.Count + " объектов");
                _b = BeginRead;
                _e = EndRead;
                using (Logger.Start(40, 90))
                    ReadValuesByParts(GetValuesRecordset, FormObjectChanges, _parts);
                int w = 0;
                foreach (var sig in ProviderSignals.Values)
                    w += sig.MakeEnd(EndRead);
                int r = _objectsId.Values.Count(ob => !ob.HasBegin(EndRead));
                NumWrite += w;
                Logger.AddEvent("Чтение значений из Historian завершено", "Добавлено " + w + " значений в конец. " + r + " неопределенных срезов");
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при получении значений из Historian", ex);
                IsConnected = false;
            }
        }
    }
}
