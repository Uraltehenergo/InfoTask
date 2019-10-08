using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для всех источников
    public abstract class SourceBase : ProviderBase
    {
        protected SourceBase() { }
        protected SourceBase(string name, Logger logger) : base(name, logger) { }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Source; } }
        
        //Начало диапазона источника
        public DateTime BeginTime { get; protected set; }
        //Конец диапазона источника
        public DateTime EndTime { get; protected set; }
        //Список временных интервалов диапазона источника
        private readonly List<TimeInterval> _timeIntervals = new List<TimeInterval>();
        public List<TimeInterval> TimeIntervals { get { return _timeIntervals; } }

        //Начало и конец текущего периода получения данных
        public DateTime BeginRead { get; private set; }
        public DateTime EndRead { get; private set; }
        //Рекордсет таблицы значений клона
        public RecDao CloneRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        public RecDao CloneErrorRec { get; private set; }

        //Список сигналов, содержащих возвращаемые значения
        protected DicS<ProviderSignal> ProviderSignals = new DicS<ProviderSignal>();
        public IDicSForRead<ProviderSignal> Signals { get { return ProviderSignals; } }
        
        //Чтение данных из архива
        public void GetValues(DateTime beginRead, DateTime endRead)
        {
            BeginRead = beginRead;
            EndRead = endRead;
            GetValues();
            foreach (var signal in Signals.Values)
                signal.CalcAverage();
        }
        public abstract void GetValues();

        //True, если соединение прошло успешно, становится False, если произошла ошибка
        protected bool IsConnected { get; set; }
        //Открытие соединения
        protected abstract bool Connect();
        //Общее количество прочитанных и сформированных значений
        protected int NumRead { get; set; }
        protected int NumWrite { get; set; }

        //Чтение значений по блокам сигналов
        #region
        //Чтение данных из архива по набору блоков синалов parts, можно использовать для вызова в GetValues у наследников
        //funReadValues - функция подготовки и выполнения запроса к источнику по одному блоку сигналов
        //funFormValues - функция формирования значений сигналов на основе прочитанных значений
        //errorWaiting - Время ожидания после ошибки считывания в мс
        //maxErrorCount - Количество блоков, которое нужно считать, чтобы понять, что связи нет, 0 - читать до конца
        //maxErrorDepth - Глубина, до которой нужно дробить первый блок, если так и не было успешного считывания
        public void ReadValuesByParts(Func<List<ProviderObject>, bool> funReadValues, Func<KeyValuePair<int, int>> funFormValues, List<List<ProviderObject>> parts, int reconnectsCount = 2, int reconnectWaiting = 500, int maxErrorCount = 3, int maxErrorDepth = 3, int errorWaiting = 100)
        {
            using (Logger.Start(0))
            {
                ErrorObjects.Clear();
                if (!IsConnected && !Connect()) return;
                _funReadValues = funReadValues;
                _funFormValues = funFormValues;
                _errorWaiting = errorWaiting;
                _maxErrorDepth = maxErrorDepth;
                _successRead = false;
                NumRead = NumWrite = 0;
                double d = 100.0/parts.Count;
                for (int i = 0; i < parts.Count; i++)
                    using (Logger.Start(i*d, i*d + d, false, true))
                    {
                        if (i < reconnectsCount)
                        {
                            IsConnected = _successRead = ReadPart(parts[i]);
                            if (!_successRead)
                            {
                                Logger.Command.IsRepeated = true;
                                Thread.Sleep(_errorWaiting);
                                _successRead |= ReadPartRecursive(parts[i], true, 1);
                            }
                        }
                        else if (i < maxErrorCount || _successRead)
                            _successRead |= ReadPartRecursive(parts[i], _successRead, 1);    
                    }
                IsConnected &= _successRead;
                if (ErrorObjects.Count > 0)
                {
                    int i = 0;
                    string s = "";
                    foreach (var ob in ErrorObjects)
                    {
                        if (++i < 10) s += ob.Key + ", ";
                        if (CloneErrorRec != null)
                        {
                            CloneErrorRec.AddNew();
                            CloneErrorRec.Put("Signal", ob.Key);
                            CloneErrorRec.Put("ErrorDescription", ob.Value);
                            CloneErrorRec.Update();
                        }
                    }
                    Logger.AddError("Не удалось прочитать значения по некоторым объектам", null, s + (ErrorObjects.Count > 10 ? " и др." : "") + "всего " + ErrorObjects.Count + " объектов не удалось прочитать", "", false);
                }
                if (_successRead)
                    Logger.AddEvent("Значения из источника прочитаны", NumRead + " значений прочитано, " + NumWrite + " значений сформировано");    
            }
        }

        //Чтение значений по одному блоку
        private bool ReadPart(List<ProviderObject> part)
        {
            using (Logger.Start(0, 50))
            {
                try
                {
                    Logger.AddEvent("Чтение значений блока объектов", part.Count + " объектов");
                    if (!_funReadValues(part)) 
                        return IsConnected = false;
                }
                catch (Exception ex)
                {
                    Logger.AddError("Ошибка при запросе данных из источника", ex);
                    return IsConnected = false;
                }
            }
            try
            {
                Logger.AddEvent("Распределение данных по сигналам", part.Count + " объектов");
                KeyValuePair<int, int> pair = _funFormValues();
                NumRead += pair.Key;
                NumWrite += pair.Value;
                Logger.AddEvent("Значения блока объектов прочитаны", pair.Key + " значений прочитано, " + pair.Value + " значений сформировано");
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при формировании значений", ex);
            }
            return true;
        }

        //Параметры рекурсивного вызова
        private Func<List<ProviderObject>, bool> _funReadValues;
        private Func<KeyValuePair<int, int>> _funFormValues;
        private int _errorWaiting;
        private int _maxErrorDepth;
        //Хотя бы одно из чтений значений было успешным
        private bool _successRead;

        //Считывает значения по блоку сигналов part, используя функцию _funReadValues, в случае ошибки читает блок по частям
        //useRecursion - использовать рекурсивный вызов, depth - глубина в дереве вызовов, начиная с 1
        private bool ReadPartRecursive(List<ProviderObject> part, bool useRecursion, int depth)
        {
            if (!IsConnected && !Connect()) return false;
            bool b = ReadPart(part);
            if (b) return true;
            if (part.Count == 1 || !useRecursion || (!_successRead && depth >= _maxErrorDepth))
            {
                foreach (var ob in part)
                    AddErrorObject(ob.Inf, Logger.Command.ErrorMessage(false, true, false));                
                return false;
            }
            Thread.Sleep(_errorWaiting);
            int m = part.Count / 2;
            bool b1 = ReadPartRecursive(part.GetRange(0, m), true, depth + 1);
            _successRead |= b1;
            if (!b1) Thread.Sleep(_errorWaiting);
            bool b2 = ReadPartRecursive(part.GetRange(m, part.Count - m), true, depth + 1);
            if (!b2) Thread.Sleep(_errorWaiting);
            return b1 || b2;
        }
        #endregion

        //Создание клона
        //Настройки создания клона
        protected DicS<string> CloneInf { get; private set; }
        //Словарь ошибочных объектов, ключи - коды объектов
        protected Dictionary<string, string> ErrorObjects = new Dictionary<string, string>();
        //Добавляет объект в ErrorObjects, inf - описание сигнала, errText - сообщение ошибки, ex - исключение
        protected void AddErrorObject(string inf, string errText, Exception ex = null)
        {
            if (!ErrorObjects.ContainsKey(inf))
                ErrorObjects.Add(inf, errText + (ex == null ? "" : (". " + ex.Message + ". " + ex.StackTrace)));    
        }

        //Создание клона архива
        public void MakeClone(DateTime beginRead, DateTime endRead, string cloneFile = "", string cloneProps = "")
        {
            try
            {
                using (var db = new DaoDb(cloneFile))
                {
                    using (var sys = new SysTabl(db))
                        CloneInf = (sys.Value("CloneInf") ?? "").ToPropertyDicS();
                    using (CloneRec = new RecDao(db, "SELECT * FROM MomentsValues"))
                        using (CloneErrorRec = new RecDao(db, "SELECT * FROM ErrorsList"))
                            GetValues(beginRead, endRead);
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при создании клона", ex);
            }
            CloneRec = null;
        }
    }
}