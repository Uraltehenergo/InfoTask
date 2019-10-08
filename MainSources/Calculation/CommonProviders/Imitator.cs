using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using VersionSynch;

namespace Calculation
{
    public class Imitator : ProviderBase, IProviderSource
    {
        public Imitator(string name, string inf, Logger logger) : base(name, logger)
        {
            Inf = inf;
            ImitMode = ImitMode.FromBegin;
        }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Imitator; } }

        //Код провайдера
        public override string Code { get { return "Imitator"; } }
        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                ImitDataFile = dic.Get("ImitDataFile", "");
                Hash = "AccessDb=" + ImitDataFile;
                new DbVersion().UpdateImitVersion(ImitDataFile, false);
            }
        }

        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }

        protected override void AddMenuCommands()
        {
            var m = new Dictionary<string, IMenuCommand>();
            m.Add("Выбрать файл", new DialogCommand(DialogType.OpenFile)
            {
                DialogTitle = "Файл имитационных данных",
                ErrorMessage = "Указан недопустимый файл имитационных данных",
                FileTables = new[] { "SignalsValues", "SignalsBehavior" }
            });
            MenuCommands.Add("ImitDataFile", m);
        }

        //Момент, от кототого отсчитывается относительное время
        public DateTime ZeroTime = new DateTime(2000, 1, 1);

        //База данных имитационных значений
        internal string ImitDataFile { get; set; }
        //True, если соединение прошло успешно, становится False, если произошла ошибка
        internal bool IsConnected { get; private set; }

        //Чтение значений из таблиц, usedSignals - набор кодов используемых сигналов
        public void PrepareSignals(DicS<DataType> usedSignals)
        {
            if (!IsConnected && !Check()) return;
            Logger.AddEvent("Чтение списка имитируемых сигналов");
            _signals.Clear();
            _signalsId.Clear();
            try
            {
                using (var rec = new ReaderAdo(ImitDataFile, "SELECT * FROM SignalsBehavior WHERE ImitFlag=True ORDER BY ImitId"))
                    while (rec.Read())
                    {
                        var sig = new ImitSignal(rec, this);
                        if (usedSignals.ContainsKey(sig.Code))
                        {
                            sig.SetDataType(usedSignals[sig.Code]);
                            _signals.Add(sig.Code, sig);
                            _signalsId.Add(sig.ImitId, sig);    
                        }
                    }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка чтения списков имитированных сигналов", ex);
            }

            Logger.AddEvent("Чтение списка имитируемых значений, " + _signalsId.Count + " сигналов");
            try
            {
                using (var rec = new ReaderAdo(ImitDataFile, "SELECT SignalsValues.ImitId AS ImitId, SignalsValues.Time, SignalsValues.RelativeTime, SignalsValues.Value, SignalsValues.Nd FROM SignalsValues INNER JOIN SignalsBehavior ON SignalsBehavior.ImitId = SignalsValues.ImitId " +
                                                                "WHERE SignalsBehavior.ImitFlag=True ORDER BY SignalsValues.ImitId, SignalsValues.RelativeTime"))
                {
                    rec.Read();
                    while (!rec.EOF)
                    {
                        int id = rec.GetInt("ImitId");
                        if (_signalsId.ContainsKey(id)) 
                            _signalsId[id].ReadMoments(rec);
                        else while (!rec.EOF && rec.GetInt("ImitId") == id)
                            rec.Read();    
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка чтения списков имитируемых значений", ex);
            }
        }

        //Вызов окна наcтройки
        public bool Check()
        {
            if (!DbVersion.IsImitFile(ImitDataFile))
            {
                Logger.AddError("Не найден или неправильный файл имитационных данных", null, "", Context);
                return IsConnected = false;
            }
            return IsConnected = true;
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return inf["ImitDataFile"].IsEmpty() ? "Не задан файл имитационных данных" : "";
        }
        
        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            Logger.AddError(CheckConnectionMessage = "Не найден или неправильный файл имитационных данных");
            return false;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        public void Dispose()
        {
        }

        //Словари сигналов, ключи - коды и Id
        private readonly DicI<ImitSignal> _signalsId = new DicI<ImitSignal>();
        private readonly DicS<ProviderSignal> _signals = new DicS<ProviderSignal>();
        public IDicSForRead<ProviderSignal> Signals { get { return _signals; }}

        //Режим формирования имитационных значений
        public ImitMode ImitMode { get; set; }

        //Чтение данных из источника
        public void GetValues(DateTime beginRead, DateTime endRead)
        {
            if (!IsConnected && !Check()) return;
            try
            {
                Logger.AddEvent("Формирование значений");
                int n = 0, m = 0;
                foreach (ImitSignal sig in _signalsId.Values)
                {
                    sig.MakeMoments(beginRead, endRead);
                    n += sig.Value.Type == SingleType.List ? sig.Value.Moments.Count : 1;
                    m++;
                    if (m % 20 == 0) Logger.Procent = m * 100.0 / _signals.Count;
                }
                Logger.AddEvent("Значения сформированы", n + " значений");
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка формирования имитационных зачений", ex);
                IsConnected = false;
            }
        }
    }
}
