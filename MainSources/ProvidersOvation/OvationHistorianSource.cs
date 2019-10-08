using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "OvationHistorianSource")]
    public class OvationHistorianSource : ISource
    {
        //name - имя провайдера, inf - строка свойств
        //public OvationHistorianSource(string name, string inf, Logger logger)
        //{
        //    Logger = logger;
        //    Name = name;
        //    Inf = inf;
        //}

        //Ссылка на потоок
        public Logger Logger { get; set; }
        //Тип провайдера
        public ProviderType Type { get { return ProviderType.Source; } }
        //Код провайдера
        public string Code { get { return "OvationHistorianSource"; } }
        //Имя провайдера
        public string Name { get; set; }
        //Кэш для идентификации соединения
        public string Hash { get; private set; }
        //Полное описание настроек провайдера для истории
        public string Context { get; private set; }
        //Настройки провайдера
        private string _inf;
        public string Inf
        {
            get { return _inf; }
            set
            {
                _inf = value;
                var dic = _inf.ToPropertyDicS();
                dic.DefVal = "";
                TypeOfData = dic["TypeOfData"];
                if (IsOriginal)
                {
                    DataSource = dic["DataSource"];
                    Hash = "OvationHistorian=" + DataSource;
                }
                else
                {
                    DatabaseFile = dic["DatabaseFile"];
                    Hash = "AccessDb=" + DatabaseFile;
                }
                Context = Name + ";" + Code + ";" + Hash;
            }
        }
        //True, пока идет настройка
        public bool IsSetup { get; set; }
        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        public DicS<Dictionary<string, IMenuCommand>> MenuCommands { get; private set; }
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname) { return new List<string>();}
        public bool CheckConnection() { return true; }
        public string CheckSettings(Dictionary<string, string> infDic, Dictionary<string, string> nameDic) { return ""; }
        public string CheckConnectionMessage { get; private set; }

        public string Setup()
        {
            IsSetup = true;
            var setup = new SetupForm { Provider = this };
            setup.ShowDialog();
            while (IsSetup) Thread.Sleep(500);
            return Inf;
        }

        //Тип источника (Ovation Historian или Файл бэкапа)
        internal string TypeOfData;
        //True, если данные бурутся из Historian
        private bool IsOriginal { get { return TypeOfData == "Оригинал"; } }
        //Имя дропа
        internal string DataSource;
        //Путь к файлу с клоном архива
        internal string DatabaseFile;
        //Соединение с провайдером Historian или файлом Access
        private OleDbConnection _connection;
        //Начало и конец текущего считывания
        private DateTime _begin;
        private DateTime _end;
        //Рекордсет получения данных
        private IRecordRead _histReader;

        //Добавление ошибки в логгер
        private void AddError(string message, Exception ex = null)
        {
            Logger.AddError(message, ex, "Источник:" + Name + "; " + (IsOriginal ? DataSource : ("Клон:" + (DatabaseFile ?? ""))));
        }

        //True, если соединение прошло успешно, становится False, если произошла ошибка
        internal bool IsConnected { get; private set; }
        //Открытие соединения
        private bool Connect()
        {
            try
            {
                try { if (_histReader != null) _histReader.Dispose(); } catch { }
                IsConnected = false;
                _connection.Close();
                _connection.Dispose();
            }
            catch { }
            try
            {
                Logger.AddEvent("Соединение с Historian");
                if (IsOriginal)
                    _connection = new OleDbConnection("Provider=OvHOleDbProv.OvHOleDbProv.1;Persist Security Info=True;User ID='';Data Source=" +
                        DataSource + ";Location='';Mode=ReadWrite;Extended Properties=''");
                else _connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DatabaseFile);
                _connection.Open();
                return IsConnected = _connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                AddError(IsOriginal ? "Ошибка соединения с Historian" : "Не найден файл клона архива", ex);
                return IsConnected = false;
            }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public bool Check()
        {
            return Logger.Danger(Connect, 2, 500, "Не удалось соединиться с Historian");
        }

        //Освобождение ресурсов, занятых провайдером
        public void Dispose()
        {
            try { if (_histReader != null) _histReader.Dispose(); } catch { }
            try { _connection.Close(); } catch { }
        }
       
        //Список сигналов 
        private readonly DicS<ProviderSignal> _signals = new DicS<ProviderSignal>();
        public IDicSForRead<ProviderSignal> Signals { get { return _signals; } }
        //Словарь объектов по Id
        private readonly DicI<ObjectOvation> _objectsId = new DicI<ObjectOvation>();
        //Список списков объектов, в каждом не более 200
        private readonly List<List<ObjectOvation>> _objectsParts = new List<List<ObjectOvation>>();

        //Добавить сигнал
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int id = 0)
        {
            var sig = new SignalOvation(signalInf, code, dataType, this, 0);
            if (!_objectsId.ContainsKey(sig.Id))
            {
                _objectsId.Add(sig.Id, new ObjectOvation(sig.Id, code));    
                if (IsOriginal)
                {
                    if (_objectsId.Count % 200 == 1) _objectsParts.Add(new List<ObjectOvation>());
                    _objectsParts[_objectsParts.Count - 1].Add(_objectsId[sig.Id]);
                }
            }
            var ob = _objectsId[sig.Id];
            if (sig.IsState) //Слово состояния
            {
                if (ob.StateSignal == null) _signals.Add(sig.Code, sig);
                return ob.StateSignal ?? (ob.StateSignal = sig);
            }
            if (sig.Bit == -1)//Аналоговый или дискретный
            {
                ob.IsValue = true;
                if (ob.ValueSignal == null) _signals.Add(sig.Code, sig);
                return ob.ValueSignal ?? (ob.ValueSignal = sig);
            }
            if (!ob.BitSignals.ContainsKey(sig.Bit))//Бит упакованного 
            {
                ob.IsValue = true;
                _signals.Add(sig.Code, sig);
                ob.BitSignals.Add(sig.Bit, sig);
            }
            return ob.BitSignals[sig.Bit];
        }

        //Удалить все сигналы
        public void ClearSignals()
        {
            _signals.Clear();
            _objectsId.Clear();
            _objectsParts.Clear();
        }

        //Подготовка источника перед расчетом
        public void Prepare() { }

        //Получение времени источника
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            TimeInterval t = null;
            try
            {
                if (IsOriginal) t = new TimeInterval(Different.MinDate.AddYears(1), DateTime.Now);
                else
                {
                    using (var sys = new SysTabl(DatabaseFile, false))
                        t = new TimeInterval(DateTime.Parse(sys.Value("BeginInterval")), DateTime.Parse(sys.Value("EndInterval")));
                }
                TimeIntervals.Add(t);
            }
            catch (Exception ex)
            {
                AddError("Ошибка определения диапазона источника", ex);
                IsConnected = false;
            }                                
            return t;
        }
        
        private readonly List<TimeInterval> _timeIntervals = new List<TimeInterval>();
        public List<TimeInterval> TimeIntervals { get { return _timeIntervals; } }

        private static bool TimeEquals(Moment mv, DateTime t)
        {
            if (mv == null) return false;
            return Math.Abs(mv.Time.Subtract(t).TotalSeconds) < 0.5;
        }

        //Параметры текущего запроса
        private IEnumerable<ObjectOvation> _objs;
        private DateTime _b, _e;
        //Попытка запроса
        private bool TryRunCommand()
        {
            try
            {
                var begs = _b.ToOvationString();
                var ens = _e.ToOvationString();
                string s = "";
                foreach (var ob in _objs)
                {
                    if (s != "") s += " or ";
                    s += "(ID=" + ob.Id + ")";
                }
                s = "(" + s + ") and ";

                _histReader = new ReaderAdo(_connection, "select ID, TIMESTAMP, TIME_NSEC, F_VALUE, RAW_VALUE, STS from PT_HF_HIST " +
                                      "where " + s + " (TIMESTAMP >= " + begs + ") and (TIMESTAMP <= " + ens + ") order by TIMESTAMP, TIME_NSEC");
            }
            catch (Exception ex)
            {
                AddError("Ошибка выполнения запроса к Historian", ex);
                return false;
            }
            return true;
        }

        //Запрос значений из Historian по списку сигналов и интервалу
        private bool RunCommand(IEnumerable<ObjectOvation> objects, DateTime beg, DateTime en)
        {
            _objs = objects;
            _b = beg;
            _e = en;
            return Logger.Danger(TryRunCommand, 2, 200, "Не удалось получить данные из Historian", Connect);
        }

        //Получает значение из текущей строчки рекордсета
        private double Mean(IRecordRead rec)
        {
            if (!rec.IsNull("F_VALUE"))
                return rec.GetDouble("F_VALUE");
            return rec.GetInt("RAW_VALUE");
        }

        //Получает слово состяния из текущей строчки рекордсета
        private int Stat(IRecordRead rec)
        {
            return rec.GetInt("STS");
        }

        //Получает недостоверность из текущей строчки рекордсета
        private int Nd(IRecordRead rec)
        {
            //Недостоверность 8 и 9 бит, 00 - good, 01 - fair(имитация), 10 - poor(зашкал), 11 - bad
            if (rec.IsNull("STS") || (rec.IsNull("F_VALUE") && rec.IsNull("RAW_VALUE")))
                return 1;
            int state = rec.GetInt("STS");
            return state.GetBit(8) && state.GetBit(9) ? 1 : 0;
        }

        //Получает время из текущей строчки рекордсета
        private DateTime Time(IRecordRead rec)
        {
            var time = rec.GetTime("TIMESTAMP");
            time = time.AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0);
            if (IsOriginal) time = time.ToLocalTime();
            if (time < _begin) time = _begin;
            return time;
        }

        //Возвращает количество значений, не имеющих среза
        private int ParamsWithCut()
        {
            int n = 0;
            foreach (var ob in _objectsId.Values)
            {
                if (ob.IsValue)
                {
                    if (TimeEquals(ob.ValueEnd, _begin))
                        ob.ValueBegin = ob.ValueEnd.Clone(_begin);
                    else
                    {
                        if (!TimeEquals(ob.ValueBegin, _begin))
                        {
                            ob.ValueBegin = null;
                            n++;
                        }
                    }    
                }
                if (ob.StateSignal != null)
                {
                    if (TimeEquals(ob.StatEnd, _begin))
                        ob.StatBegin = ob.StatEnd.Clone(_begin);
                    else
                    {
                        if (!TimeEquals(ob.StatBegin, _begin))
                        {
                            ob.StatBegin = null;
                            n++;
                        }
                    }
                }
            }
            return n;
        }

        //Получение среза значений
        private void GetBeginValues()
        {
            try
            {
                int d = 4;
                int n = ParamsWithCut(), m = 0;
                while (d <= 60 && n > 0)
                {
                    DateTime beg = _begin.AddMinutes(-d);
                    foreach (List<ObjectOvation> part in _objectsParts)
                    {
                        var list = part.Where(ob => ob.ValueBegin == null).ToList();
                        if (list.Count > 0)
                        {
                            Logger.AddEvent("Чтение среза значений по " + d + " минутам", list.Count + " сигналов");
                            if (RunCommand(list, beg, _begin))
                            {
                                Logger.Procent += 15.0 / _objectsParts.Count;
                                Logger.AddEvent("Распределение значений по сигналам");
                                while (_histReader.Read())
                                {
                                    m++;
                                    int id = _histReader.GetInt("ID");
                                    if (_objectsId.ContainsKey(id))
                                    {
                                        var ob = _objectsId[id];
                                        if (ob.IsValue)
                                        {
                                            if (ob.ValueBegin == null) n--;
                                            ob.ValueBegin = new Moment(ob.DataType, Mean(_histReader), _begin, Nd(_histReader));
                                            ob.ValueEnd = ob.ValueBegin;    
                                        }
                                        if (ob.StateSignal != null)
                                        {
                                            if (ob.StatBegin == null) n--;
                                            ob.StatBegin = new Moment(DataType.Integer, Stat(_histReader));
                                            ob.StatEnd = ob.StatBegin;
                                        }
                                    }
                                }
                                _histReader.Dispose();
                            }
                        }
                        Logger.Procent += 15.0 / _objectsParts.Count;
                    }
                    d *= 15;
                }
                Logger.AddEvent("Значения прочитаны", m + " значений, " + n + " неопределенных срезов");
            }
            catch( Exception ex)
            {
                AddError("Ошибка при чтении из Historian", ex);
                IsConnected = false;
            }
        }

        //Чтение данных из клона
        private void GetCloneValues()
        {
            if (!IsConnected && !Connect()) return;
            try
            {
                using (var db = new DaoDb(DatabaseFile))
                {
                    db.Execute("DELETE * FROM SignalsForRead");
                    Logger.Procent = 10;
                    using (var rs = new RecDao(db, "SignalsForRead"))
                        foreach (int id in _objectsId.Keys)
                        {
                            rs.AddNew();
                            rs.Put("Id", id);
                        }   
                }
                Logger.Procent = 20;
                int n = ParamsWithCut();
                DateTime beg = n == 0 ? _begin : _begin.AddHours(-1);
                Logger.AddEvent("Получение значений", _objectsId.Count + " сигналов, " + n + " без среза, время: " + beg + " - " + _end);
                var stSql = "SELECT SignalsForRead.Id AS ID, TIMESTAMP, TIME_NSEC, F_VALUE, RAW_VALUE, STS " + "FROM PT_HF_HIST INNER JOIN SignalsForRead ON PT_HF_HIST.ID = SignalsForRead.Id " 
                    + "WHERE (TIMESTAMP >= " + beg.ToAccessString() + ") AND (TIMESTAMP < " + _end.ToAccessString() + ") " + "ORDER BY TIMESTAMP, TIME_NSEC";
                using (var rec = new ReaderAdo(DatabaseFile, stSql))
                {
                    Logger.Procent = 40;
                    Logger.AddEvent("Распределение значений по сигналам");
                    var m = ReadValuesList(rec, _objectsId.Values);
                    Logger.AddEvent("Значения прочитаны", m + " значений, " + _objectsId.Count + " сигналов");        
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении клона архива", ex);
                IsConnected = false;
            }
        }

        //Чтение значений из одного рекордсета оригинала или клона по списку сигналов
        private int ReadValuesList(IRecordRead rec, IEnumerable<ObjectOvation> objects)
        {
            int m = 0;
            while (rec.Read())
            {
                var id = rec.GetInt("Id");
                if (_objectsId.ContainsKey(id))
                {
                    m++;
                    var ob = _objectsId[id];
                    if (ob.StateSignal != null)
                    {
                        var mv = new Moment(DataType.Integer, Stat(rec), Time(rec));
                        if (mv.Time <= _begin) ob.StatBegin = mv;
                        else AddValueToList(ob.StateSignal.Value.Moments, mv);
                        ob.StatEnd = mv;
                    }
                    if (ob.IsValue)
                    {
                        var mv = new Moment(ob.DataType, Mean(rec), Time(rec), Nd(rec));
                        if (mv.Time <= _begin) ob.ValueBegin = mv;
                        else
                        {
                            if (ob.ValueSignal != null)
                                AddValueToList(ob.ValueSignal.Value.Moments, mv);
                            if (ob.BitSignals != null && ob.BitSignals.Count != 0)
                                foreach (var b in ob.BitSignals.Keys)
                                    AddValueToList(ob.BitSignals[b].Value.Moments, new Moment(mv.Time, (mv.Integer & (1 << b)) != 0, null, mv.Nd));
                        }
                        ob.ValueEnd = mv;    
                    }
                }
            }
            foreach (var ob in objects)
            {
                if (ob.StatBegin != null)
                    ob.StateSignal.Value.Moments.Insert(0, ob.StatBegin);
                if (ob.StatEnd != null)
                    ob.StatEnd = ob.StatEnd.Clone(_end);
                if (ob.ValueBegin != null)
                {
                    if (ob.ValueSignal != null)
                        ob.ValueSignal.Value.Moments.Insert(0, ob.ValueBegin);
                    if (ob.BitSignals.Count != 0)
                        foreach (var b in ob.BitSignals.Keys)
                        {
                            var mb = new Moment(_begin, (ob.ValueBegin.Integer & (1 << b)) != 0, null, ob.ValueBegin.Nd);
                            ob.BitSignals[b].Value.Moments.Insert(0, mb);
                        }
                }
                if (ob.ValueEnd != null)
                    ob.ValueEnd = ob.ValueEnd.Clone(_end);
            }
            return m;
        }

        //Добавляет значение в список значений
        private void AddValueToList(List<Moment> list, Moment mv)
        {
            var last = list.Count == 0 ? null : list[list.Count - 1];
            if (mv.Time <= _end && (last == null || (last.Time < mv.Time && (last != mv || last.Nd != mv.Nd))))
                list.Add(mv);   
        }

        //Попытка прочитать значения
        private bool TryGetValues()
        {
            try
            {
                if (!IsConnected && !Connect()) return false;
                Logger.Start(GetBeginValues, 0, 40);
                int m = 0;
                foreach (List<ObjectOvation> sigList in _objectsParts)
                {
                    Logger.AddEvent("Получение значений", sigList.Count + " сигналов");
                    if (RunCommand(sigList, _begin, _end))
                    {
                        Logger.Procent += 20.0 / _objectsParts.Count;
                        Logger.AddEvent("Распределение значений по сигналам");
                        m += ReadValuesList(_histReader, sigList);
                        Logger.Procent += 20.0 / _objectsParts.Count;
                        _histReader.Dispose();
                    }
                }
                Logger.AddEvent("Значения прочитаны", m + " значений, " + _objectsId.Count + " сигналов");
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при получении значений из Historian", ex);
                IsConnected = false;
            }
            return !Logger.Command.IsError;
        }

        //Чтение значений из источника
        public void GetValues(DateTime beginRead, DateTime endRead)
        {
            _begin = beginRead;
            _end = endRead;
            foreach (var ps in Signals.Values)
                ps.Value.Moments.Clear();
            if (!IsOriginal) GetCloneValues();
            else
            {
                if (!Logger.IsRepeat) TryGetValues();
                else Logger.Danger(TryGetValues, 3, 60000, "Не удалось получить данные из Historian", Connect);
            }
        }

        public void MakeClone(DateTime beginRead, DateTime endRead, string cloneFile, string cloneInf)
        {
        }
    }
}
