using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Базовый класс для потоков
    public abstract class ThreadCalc : Logger, INotifyPropertyChanged
    {
        protected ThreadCalc()
        {
            try
            {
                IsClosed = true;
                CalcMode = "Остановлен";
                Funs = new Funs();
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка создания потока расчетов", "Системная ошибка контроллера расчетов");
            }
        }

        //Продготовка файла Result 
        protected void PrepareResultFile()
        {
            ResultFile = General.ControllerDir + @"Result\Result" + Id + ".accdb";
            if (!DaoDb.FromTemplate(General.ControllerDir + "ResultTemplate.accdb", ResultFile, ApplicationType == ApplicationType.Controller ? ReplaceByTemplate.IfNewVersion : ReplaceByTemplate.Always))
                DaoDb.Compress(ResultFile, 400000000, General.TmpDir, 3000);
        }

        //Id потока
        public int Id { get; protected set; }
        //Приложение, запустившее поток
        public ApplicationType ApplicationType { get; protected set; }
        
        //Комментарий
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (value == _comment) return;
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }

        //Словарь проектов, ключ - code
        private readonly DicS<Project> _projects = new DicS<Project>();
        public DicS<Project> Projects { get { return _projects; }}

        //Список всех функций
        public DicS<Fun> FunsDic { get; private set; }
        //Класс вычисления значений функций
        public Funs Funs { get; private set; }

        //Состояние данных потока, true - если не загружен или закрыт
        private bool _isClosed;
        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                _isClosed = value;
                if (_isClosed) IsSourcesRead = false;
            }
        }
        
        //Состояние интерфейса потока
        public abstract State State { get; set; }
        //Поток расчета 
        public Thread Task { get; set; }

        //Файл Result
        public string ResultFile { get; private set; }
        //Объект для соранения данных в ведомость РАС
        public VedSaver VedSaver { get; protected set; }

        //Архив для накопления  и его провайдер
        public IArchive Archive { get; protected set; }
        public Provider ArchivePr { get; protected set; }
        //Словарь источников, ключи Name
        private readonly DicS<ISource> _sources = new DicS<ISource>();
        public DicS<ISource> Sources { get { return _sources; }}
        //Словарь источников, ключи Name
        private readonly DicS<Imitator> _imitators = new DicS<Imitator>();
        public DicS<Imitator> Imitators { get { return _imitators; } }
        //Словарь приемниеков, ключи Name
        private readonly DicS<IReceiver> _receivers = new DicS<IReceiver>();
        public DicS<IReceiver> Receivers { get { return _receivers; } }

        //Начало текущего периода обработки
        private DateTime _periodBegin = Different.MinDate;
        private readonly object _periodBeginLock = new object();
        public DateTime PeriodBegin
        {
            get { lock (_periodBeginLock) return _periodBegin; } 
            set
            {
                lock (_periodBeginLock)
                {
                    if (value == _periodBegin) return;
                    _periodBegin = value;
                    OnPropertyChanged("PeriodBegin");    
                }
                if (IsPeriodic) PeriodEnd = PeriodBegin.AddMinutes(PeriodLength);
            }
        }
        //Конец текущего периода обработки
        private DateTime _periodEnd = Different.MinDate;
        private readonly object _periodEndLock = new object();
        public DateTime PeriodEnd
        {
            get { lock (_periodEndLock) return _periodEnd; }
            set
            {
                lock (_periodEndLock)
                {
                    if (value == _periodEnd) return;
                    _periodEnd = value;
                    OnPropertyChanged("PeriodEnd");    
                }
            }
        }
        //Имя расчета
        private string _calcName;
        public string CalcName
        {
            get { return _calcName; }
            set
            {
                if (value == _calcName) return;
                _calcName = value;
                OnPropertyChanged("CalcName");
            }
        }

        //Длина периода обработки в минутах
        private double _periodLength;
        public double PeriodLength
        {
            get { return _periodLength; }
            set
            {
                if (value == _periodLength) return;
                _periodLength = value;
                OnPropertyChanged("PeriodLength");
                if (IsPeriodic) PeriodEnd = PeriodBegin.AddMinutes(_periodLength);
            }
        }

        //Признак включения имитации и режим имитации, берутся из ControllerData
        private bool _isImit;
        public bool IsImit
        {
            get { return _isImit; }
            set
            {
                if (value == _isImit) return;
                _isImit = value;
                IsSourcesRead = false;
                MakeProviders();
                OnPropertyChanged("IsImit");
            }
        }
        private string _imitModeStr;
        public string ImitModeStr
        {
            get { return _imitModeStr; }
            set
            {
                if (value == _imitModeStr) return;
                _imitModeStr = value;
                IsSourcesRead = false;
                OnPropertyChanged("ImitModeStr");
            }
        }

        //Режим имитации передавыемый в Calc для ThreadApp
        protected ImitMode ImitModeCalc = ImitMode.Default;

        //Режим имитации при расчете
        public ImitMode ImitMode
        {
            get
            {
                if (ImitModeCalc != ImitMode.Default)
                    return ImitModeCalc;
                if (!IsImit) return ImitMode.NoImit;
                return ImitModeStr.ToImitMode();
            }
        }

        //True, если расчет периодический
        private bool _isPeriodic;
        public bool IsPeriodic
        {
            get { return _isPeriodic; }
            set
            {
                if (value == _isPeriodic) return;
                IsRepeat = _isPeriodic = value;
                if (ApplicationType == ApplicationType.Controller)
                    CalcType = value ? "Периодический" : "Разовый";
                OnPropertyChanged("IsPeriodic");
            }
        }

        //Начало диапазона источников
        private DateTime _sourcesBegin;
        public DateTime SourcesBegin
        {
            get { return _sourcesBegin; }
            set
            {
                if (value == _sourcesBegin) return;
                _sourcesBegin = value;
                OnPropertyChanged("SourcesBegin");
            }
        }
        //Конец диапазона источников
        private DateTime _sourcesEnd;
        public DateTime SourcesEnd
        {
            get { return _sourcesEnd; }
            set
            {
                if (value == _sourcesEnd) return;
                _sourcesEnd = value;
                OnPropertyChanged("SourcesEnd");
            }
        }

        //Строка содержащая времена всех источников в формате Имя Провайдера;Начало периода;Конец периода;
        private string _sourcesTimeString;
        public string SourcesTimeString() { return _sourcesTimeString;}

        //Запуск атомарной комманды
        public Command StartAtom(Atom atom, double start = 0, double finish = 100, string context = "", string objectName = "")
        {
            CurrentOperation = atom.ToRussian();
            if (!objectName.IsEmpty()) CurrentOperation += " (" + objectName + ")";
            return StartLog(start, finish, atom.ToRussian(), "", context, objectName);
        }
        //Запуск атомарной комманды и выполнение действия action, возвращает false, если ошибка
        public bool StartAtom(Atom atom, Action action, double start = 0, double finish = 100, string context = "", string objectName = null)
        {
            using (var c = StartAtom(atom, start, finish, context, objectName))
            {
                action();
                return !c.IsError;
            }
        }
        //Запуск комманды отображения
        public Command StartView(ViewAtom viewAtom, bool useSubLog)
        {
            if (!useSubLog) return Start(true);
            return StartSubLog(viewAtom.ToRussian(), PeriodBegin, PeriodEnd, CalcMode, CalcName, true);
        }
        //Запуск комманды отображения и выполнение действия action, возвращает false, если ошибка
        public bool StartView(ViewAtom viewAtom, Action action, bool useSubLog)
        {
            using (var c = StartView(viewAtom, useSubLog))
            {
                action();
                return !c.IsError;    
            }
        }

        //Время завершения комманды отображения
        public DateTime FinishCycleTime { get; set; }

        //Чтение списка функций из SprFunctions и SprFunctionsTypes
        protected void ReadFunctions()
        {
            FunsDic = new DicS<Fun>();
            const string stSql = "SELECT Functions.Name, Functions.Synonym, Functions.Code, Functions.CodeType, FunctionsOverloads.* " +
                                          "FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId WHERE Functions.NotLoad = False ORDER BY Functions.Name;";
            using (var rec = new ReaderAdo(General.GeneralDir + "General.accdb", stSql))
                while (rec.Read())
                    new Fun(rec, this);
        }

        //Операции, выполняемые во время расчета
        private bool _isReadSources = true;
        public bool IsReadSources { get { return Sources.Count != 0 && (_isReadSources || IsPeriodic || !IsSourcesRead || PeriodBegin != LastPeriodBegin || PeriodEnd != LastPeriodEnd) ; } }
        private bool _isWriteReceivers = true;
        public bool IsWriteReceivers { get { return Receivers.Count != 0 && _isWriteReceivers; }}
        private bool _isWriteArchives = true;
        public bool IsWriteArchives { get { return Archive != null && _isWriteArchives; }}

        //Свойства ведомости анализатора
        public bool IsWriteVed { get; protected set; }
        public string VedTask { get; protected set; }
        public string VedFile { get; protected set; }

        public void SetCalcOperations(bool readSources, bool writeArchives, bool writeReceivers)
        {
            using (StartAtom(Atom.SetCalcOperations))
            {
                _isReadSources = readSources;
                string s = _isReadSources ? "ReadSources;" : "";
                _isWriteReceivers = writeReceivers;
                if (_isWriteReceivers) s += "WriteReceivers;";
                _isWriteArchives = writeArchives;
                if (_isWriteArchives) s += "WriteArchives;";
                AddEvent("Выполняемые операции заданы", s);
            }
        }

        protected bool IsSourcesRead;//True, если значения с источников были прочитаны
        protected DateTime LastPeriodBegin;//Начало и конец предыдущего периода
        protected DateTime LastPeriodEnd;
        public IntervalType WriteArchiveType { get; protected set; }//Тип записи в архив
        //Id интервала архива последнего расчета
        protected int IdInterval { get; set; }
        public int IntervalId() { return IdInterval;}

        //Параметры отладочного сохранения
        public bool IsSaveSignals {get; private set;}
        public bool IsSaveParams { get; private set; }
        public bool IsSaveProperties { get; private set; }
        public bool IsSaveVariables { get; private set; }
        public bool IsSaveReceiversSignals { get; private set; }
        public bool IsSaveValues { get; private set; } //Сохранять значения в DebugValues, иначе сохранчется только в DebugParams
        public void SetDebugOperations(bool saveSignals, bool saveParams, bool saveMethods, bool saveVariables, bool saveReceiversSignals, bool saveValues)
        {
            using (StartAtom(Atom.SetDebugOperations))
            {
                IsSaveSignals = saveSignals;
                string s = IsSaveSignals ? "SaveSignals;" : "";
                IsSaveParams = saveParams;
                if (IsSaveParams) s += "SaveParams;";
                IsSaveProperties = saveMethods;
                if (IsSaveProperties) s += "SaveProperties;";
                IsSaveVariables = saveVariables;
                if (IsSaveVariables) s += "SaveVariables;";
                IsSaveReceiversSignals = saveReceiversSignals;
                if (IsSaveReceiversSignals) s += "SaveReceiversSignals;";
                IsSaveValues = saveValues;
                if (IsSaveValues) s += "SaveValues;";
                AddEvent("Отладочные операции заданы", s);
            }
        }

        //Загружет списки проектов и провайдеров для потока
        public void ReadSetup()
        {
            try
            {
                Projects.Clear();
                ProjectsList.Clear();
                foreach (var provider in ProvidersDic.Values)
                {
                    var pi = provider.ProviderInstance;
                    if (pi != null)
                    {
                        if (ApplicationType == ApplicationType.Controller && General.ProvidersQueues.ContainsKey(pi.Hash))
                            General.ProvidersQueues.Remove(pi.Hash);
                        pi.Dispose();
                    }
                }
                        
                ProvidersDic.Clear();
                Sources.Clear();
                Receivers.Clear();
                Imitators.Clear();
                Archive = null;
                ArchivePr = null;

                using ( var rec = new RecDao(General.ControllerFile, "SELECT * FROM Projects WHERE ThreadId =" + Id + " ORDER BY Project"))
                    while (rec.Read())
                    {
                        var proj = new Project(rec, this);
                        if (!Command.IsError && !Projects.ContainsKey(proj.Code))
                        {
                            Projects.Add(proj.Code, proj);
                            ProjectsList.Add(proj);
                            AddEvent("Добавлен проект", proj.Code + ", " + proj.File);
                            proj.ReadProviders();//Чтение списка провайдеров
                        }
                    }
                MakeProviders();
                MakeProjectString();
            }
            catch(Exception ex)
            {
                AddError("Ошибка загрузки списка проектов. Необходимо повторить настройку", ex);
            }
            Procent = 70;

            try //Список провайдеров
            {
                AddEvent("Чтение настроек провайдеров");
                using (var rec = new RecDao(General.ControllerFile, "SELECT * FROM Providers WHERE ThreadId =" + Id))
                    while (rec.Read())
                    {
                        var name = rec.GetString("ProviderName");
                        if (ProvidersDic.ContainsKey(name))
                        {
                            var prov = ProvidersDic[name];
                            prov.Code = rec.GetString("ProviderCode");
                            prov.Inf = rec.GetString("ProviderInf");
                            prov.Otm = rec.GetBool("Otm");
                        }
                    }
                foreach (var prov in ProvidersDic.Values)
                    if (prov.Otm)
                    {
                        prov.ProviderInstance = General.RunProvider(prov.Code, prov.Name, prov.Inf, this);
                        if (prov.ProviderInstance != null)
                        {
                            switch (prov.Type.ToProviderType())
                            {
                                case ProviderType.Archive:
                                    Archive = (IArchive)prov.ProviderInstance;
                                    ArchivePr = prov;
                                    break;
                                case ProviderType.Source:
                                    Sources.Add(prov.Name, (ISource)prov.ProviderInstance);
                                    break;
                                case ProviderType.Imitator:
                                    var ims = (Imitator)prov.ProviderInstance;
                                    Imitators.Add(prov.Name, ims);
                                    var pname = prov.Name.Substring(0, prov.Name.Length - 4);
                                    if (Projects.ContainsKey(pname)) Projects[pname].Imitator = ims;
                                    else AddError("Недопустимое имя провайдера имитатора", null, prov.Name);
                                    break;
                                case ProviderType.Receiver:
                                    Receivers.Add(prov.Name, (IReceiver)prov.ProviderInstance);
                                    break;
                            }
                            if (ApplicationType == ApplicationType.Controller)
                            {
                                string hash = prov.ProviderInstance.Hash;
                                if (!General.ProvidersQueues.ContainsKey(hash))
                                    General.ProvidersQueues.Add(hash, new Queue<int>());
                                prov.ProviderQueue = General.ProvidersQueues[hash];
                            }    
                        }
                    }
                MakeProviders();
            }
            catch(Exception ex)
            {
                AddError("Ошибка при чтении настроек провайдеров", ex);
            }
        }

        //Формирует список провайдеров для отображения
        protected void MakeProviders()
        {
            if (ApplicationType != ApplicationType.Controller) return;
            Providers.Clear();
            foreach (var p in ProvidersDic.Values)
                if (p.Type.ToProviderType() != ProviderType.Imitator || ImitMode != ImitMode.NoImit)
                    Providers.Add(p);    
        }

        //Формирует строку ProjectsString
        protected string MakeProjectString()
        {
            string s = "";
            foreach (var proj in ProjectsList)
            {
                if (s != "") s += ", ";
                s += proj.Code;    
            }
            return ProjectsString = s;
        }

        //Проверка соединения со всеми провайдерами потока
        protected void CheckProviders()
        {
            foreach (var pr in ProvidersDic.Values)
            {
                try
                {
                    var pi = pr.ProviderInstance;
                    if (pi != null && (pi.Type != ProviderType.Imitator || (ImitMode != ImitMode.NoImit && (ApplicationType == ApplicationType.Controller || ApplicationType.IsReport()))))
                        pi.CheckConnection();
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при проверке соединения с провайдером", ex, pr.ProviderInstance == null ? "" : (pr.ProviderInstance.Code + ", " + pr.ProviderInstance.Name));
                }
            }
        }

        //Получение общего диапазона всех источников
        public void ReadTime()
        {
            DateTime beg = Different.MaxDate, en = Different.MinDate;
            string st = "";
            try
            {
                if (Sources.Count > 0)
                {
                    double i = 0, d = 100.0 / Sources.Count;
                    foreach (var s in Sources.Values)
                        if (s.Type != ProviderType.Imitator)
                        {
                            AddEvent("Запрос времени источника");
                            using (Start(i, i += d))
                            {
                                TimeInterval interval = s.GetTime();
                                if (interval != null)
                                {
                                    AddEvent("Время источника определено", interval.Begin + " - " + interval.End);
                                    if (interval.Begin < beg && interval.Begin != Different.MinDate) beg = interval.Begin;
                                    if (interval.End > en) en = interval.End;
                                    st += s.Name + "=" + interval.Begin + "-" + interval.End + ";";
                                }
                                else
                                {
                                    AddEvent("Время источника не определено");
                                    st += s.Name + "=" + Different.MinDate + "-" + Different.MaxDate + ";";
                                }
                            }    
                        }
                }
            }
            catch(Exception ex)
            {
                AddError("Ошибка при определении времени источников", ex);
            }
            SourcesBegin = beg != Different.MaxDate ? beg : Different.MinDate;
            SourcesEnd = en != Different.MinDate ? en : Different.MaxDate;
            _sourcesTimeString = st;
        }

        //Подготовка данных для расчета
        protected void PrepareCalc()
        {
            try
            {
                StartAtom(Atom.PrepareResult, PrepareResult, 0, 18);
                if (IsReadSources) StartAtom(Atom.DeleteItems, DeleteItems, 18, 20);
                double procent = 20;
                var prsotm = Projects.Values.Where(x => x.Otm).ToList();
                
                double d = 30.0 / prsotm.Count;
                foreach (var p in prsotm)
                    using (StartAtom(Atom.ReadProject, procent, procent += d * 0.7, "Проект " + p.Code, p.Code))
                    {
                        p.ReadProject();
                        if (Command.IsError) return;
                    }

                if (IsReadSources || IsWriteReceivers)
                    StartAtom(Atom.PrepareProviders, PrepareProviders, 50, procent = 70);
                if (IsWriteArchives)
                    using (StartAtom(Atom.PrepareArchive, procent, 90, Archive.Context, Archive.Name))
                    {
                        var pa = prsotm.Where(x => x.Providers.ContainsKey(Archive.Name)).ToList();
                        double i = 0;
                        foreach (var p in pa)
                        {
                            Start(p.PrepareArchive, i, i += 40.0 / pa.Count);
                            if (IsPeriodic)
                            {
                                Start(p.ReadArchiveForPeriodic, i, i += 40.0 / pa.Count);
                                Start(p.ReadPrev, i, i += 20.0 / pa.Count);
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке расчета. Возможно следует обновить список сигналов в проекте и скомпилировать проект", ex);
            }
        }

        //Удаляет сигналы, параметры и т.д. ис провайдеров и проектов
        protected void DeleteItems()
        {
            if (IsReadSources)
                foreach (var source in Sources.Values)
                    source.ClearSignals();
        }

        //Очистка и загрузка и файла Result
        protected void PrepareResult()
        {
            PrepareResultFile();
        }

        //Подготовка провайдеров
        protected void PrepareProviders()
        {
            try
            {
                var prov = ProvidersDic.Values.Where(x => x.Otm).ToList();
                double n = 0;
                foreach (var pr in prov)
                {
                    if (pr.ProviderType == ProviderType.Source && IsReadSources)
                        using (Start(n, n += 100.0/prov.Count))
                        {
                            AddEvent("Подготовка иcточника");
                            var s = Sources[pr.Name];
                            if (s.Signals != null && s.Signals.Count > 0)
                                s.Prepare();
                        }

                    if (pr.ProviderType == ProviderType.Receiver && IsWriteReceivers)
                        using (Start(n, n += 100.0/prov.Count))
                        {
                            AddEvent("Подготовка приемника");
                            var r = Receivers[pr.Name];
                            if (r.Signals != null && r.Signals.Count > 0)
                                r.Prepare();
                        }
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке провайдеров", ex);
            }
        }
        
        private bool IsLastCycle()
        {
            if (StopTime == null) return false;
            return PeriodEnd >= ((DateTime) StopTime).AddSeconds(-1);
        }

        // Чтение данных из всех источников
        protected void ReadSources()
        {
            double i = 0, d = 100.0 / (Sources.Count + Imitators.Count);
            foreach (var p in ProvidersDic.Values)
                if (p.ProviderType.IsProviderSource() && p.Otm)
                {
                    if (p.ProviderType == ProviderType.Imitator) 
                        ((Imitator) p.ProviderInstance).ImitMode = ImitMode;
                    var s = (IProviderSource)p.ProviderInstance;
                    if (s.Signals != null && s.Signals.Count > 0)
                    {
                        using (StartAtom(Atom.ReadSource, i, i += d * 0.75, s.Context, s.Name))
                        {
                            s.GetValues(PeriodBegin, PeriodEnd);
                            AddEvent("Объем используемой памяти", GC.GetTotalMemory(false).ToString());
                            GC.Collect();
                        }
                        if (IsSaveSignals && IsLastCycle())
                            using (StartAtom(Atom.SaveSourceDebug, i, i += d * 0.25, s.Context, s.Name))
                                SaveSourceDebug(s);
                    }
                }
        }
        
        //Расчет по всем проектам
        protected void Calculate()
        {
            if (Projects.Count > 0)
            {
                double i = 0, d = 100.0 / Projects.Count;
                foreach (var p in Projects.Values)
                {
                    StartAtom(Atom.CalculateProject, p.Calculate, i, i += d * 0.7, "Проект " + p.Code, p.Code);
                    if (Archive != null && p.ArchiveProject != null && WriteArchiveType != IntervalType.Empty)
                        StartAtom(Atom.AccumulateProject, p.Accumulate, i, i += d * 0.1, "Проект " + p.Code, p.Code);
                    if (IsSaveParams && IsLastCycle())
                        StartAtom(Atom.SaveCalcDebug, p.SaveCalcDebug, i, i += d * 0.2, "Проект " + p.Code, p.Code);
                }
            }
        }
        
        //Запись результатов в приемники
        protected void WriteReceivers()
        {
            if (Receivers.Count > 0)
            {
                double i = 0, d = 100.0 / Receivers.Count;
                foreach (var p in ProvidersDic.Values)
                {
                    if (p.ProviderType == ProviderType.Receiver && p.Otm)
                    {
                        var r = (IReceiver)p.ProviderInstance;
                        if (r.Signals != null && r.Signals.Count > 0)
                        {
                            using (StartAtom(Atom.WriteReceiver, i, i += d*0.8, r.Context, r.Name))
                                r.WriteValues();
                            if (IsSaveReceiversSignals)
                                using (StartAtom(Atom.SaveReceiverDebug, i, i += d*0.2, r.Context, r.Name))
                                    SaveReceiverDebug(r);
                        }
                    }
                }
            }
        }

        //Запись результатов в архивы
        protected void WriteArchives()
        {
            if (Projects.Count > 0 && Archive != null && WriteArchiveType != IntervalType.Empty)
            {
                double i = 0, d = 100.0 / Projects.Count;
                foreach (var p in Projects.Values)
                    if (p.ArchiveProject != null)
                        using (StartAtom(Atom.WriteArchive, i, i += d, "Проект " + p.Code, p.Code))
                        {
                            Archive.WriteProject(p.ArchiveProject, PeriodBegin, PeriodEnd);
                            var t = p.ArchiveProject.IntervalsForWrite;
                            if (t != null && t.Count == 1) IdInterval = t[0].Id;
                        }    
            }
        }

        //Комманда закрытие потока
        public void CloseThread()
        {
            try
            {
                using (StartAtom(Atom.CloseThread))
                    foreach (var pr in ProvidersDic.Values)
                        if (pr.ProviderInstance != null) 
                            pr.ProviderInstance.Dispose();
                IsClosed = true;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при закрытии файла истории", ex);
            }
            UpdateHistory(false);
            CloseHistory();
        }

        //Сохраняет мгновенные значения сигналов одного источника в таблицу
        private void SaveSourceDebug(IProviderSource source)
        {
            try
            {
                AddEvent("Сохранение значений сигналов в таблицу");
                using (var sav = new ResultSaver(source.Name, this))
                {
                    Procent = 10;
                    int i = 0;
                    foreach (var sig in source.Signals.Values)
                    {
                        sav.ValuesToRec(sig.Code, "Сигнал", 0, null, sig.Value);
                        if (++i % 20 == 0) Procent = 10 + 90.0 * i / (source.Signals.Count);
                    }    
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка подготовки отладочного файла Result", ex);
            }
        }

        //Сохраняет значения сигналов одного приемника в таблицу
        private void SaveReceiverDebug(IReceiver receiver)
        {
            try
            {
                AddEvent("Сохранение значений сигналов приемника в таблицу");
                using (var sav = new ResultSaver(receiver.Name, this))
                {
                    Procent = 10;
                    int i = 0;
                    foreach (var sig in receiver.Signals.Values)
                    {
                        sav.ValuesToRec(sig.Code, "СигналПриемника", 0, null, receiver.AllowListValues ? sig.Value : new SingleValue(sig.Value.LastMoment));
                        if (++i % 20 == 0) Procent = 10 + 90.0 * i / (receiver.Signals.Count);
                    }    
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка подготовки отладочного файла Result", ex);
            }
        }

        //Удаление ссылок на мгновенные значения
        protected void ClearMemory()
        {
            foreach (var source in Sources.Values)
                foreach (var signal in source.Signals.Values)
                    if (signal.Value != null && signal.Value.Moments != null)
                        signal.Value.Moments.Clear();
            foreach (var project in ProjectsList)
                project.ClearMemory();
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Логирование
        //////////////////////////////////////////////////////////////////////////////////////////

       //Завершить комманду логирования
        protected override void FinishLogCommand()
        {
            CurrentOperation = "";
        }

        protected override void FinishSubLogCommand() {}

        //Завершить комманду отображения индикатора
        protected override void FinishProgressCommand()
        {
            CurrentOperation = "";
            IndicatorProcent = 0;
            IndicatorText = "";
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //ViewModel
        //////////////////////////////////////////////////////////////////////////////////////////

        //True, если поток контроллера
        public bool IsController { get { return !(this is ThreadApp); } }

        //Подпись к форме
        private string _formCaption;
        public string FormCaption
        {
            get { return _formCaption; }
            set
            {
                if (value == _formCaption) return;
                _formCaption = value;
                OnPropertyChanged("FormCaption");
            }
        }

        //Режим (Разовый, Периодический, Конструктор, Excel)
        private string _calcType;
        public string CalcType
        {
            get { return _calcType; }
            set
            {
                if (value == _calcType) return;
                _calcType = value;
                FormCaption = "Поток " + Id + " (" + _calcType + ", " + CalcMode + ")";
                OnPropertyChanged("CalcType");
            }
        }
        //Режим (Остановлен, Запущен, Синхронный, Выравнивание)
        private string _calcMode;
        public string CalcMode
        {
            get { return _calcMode; }
            set
            {
                if (value == _calcMode) return;
                _calcMode = value;
                FormCaption = "Поток " + Id + " (" + CalcType + ", " + _calcMode + ")";
                OnPropertyChanged("CalcMode");
            }
        }
        //Список проектов для формы списка потоков
        private string _projectsString;
        public string ProjectsString
        {
            get { return _projectsString; }
            set
            {
                if (value == _projectsString) return;
                _projectsString = value;
                OnPropertyChanged("ProjectsString");
            }
        }

        //Текущая операция
        private string _currentOperation;
        public string CurrentOperation
        {
            get { return _currentOperation; }
            set
            {
                if (value == _currentOperation) return;
                _currentOperation = value;
                if (this is ThreadApp)
                {
                    var f = ((ThreadApp) this).Form;
                    if (f != null)
                    {
                        f.CurrentOperation.Text = value;
                        f.Refresh();
                    }
                }
                else OnPropertyChanged("CurrentOperation");
            }
        }
        //Процент индикатора
        private double _indicatorProcent;
        public double IndicatorProcent
        {
            get { return _indicatorProcent; }
            set
            {
                if (value == _indicatorProcent) return;
                _indicatorProcent = value;
                OnPropertyChanged("IndicatorProcent");
            }
        }
        //Текст над индикатором, процент или сколько осталось времени
        private string _indicatorText;
        public string IndicatorText
        {
            get { return _indicatorText; }
            set
            {
                if (value == _indicatorText) return;
                _indicatorText = value;
                OnPropertyChanged("IndicatorText");
            }
        }

        //Конец диапазона текущего цикла расчета
        private DateTime? _stopTime;
        public DateTime? StopTime
        {
            get { return _stopTime; }
            set
            {
                if (value == _stopTime) return;
                _stopTime = value;
                OnPropertyChanged("StopTime");
            }
        }

        //Список провайдеров для настройки
        private readonly ObservableCollection<Provider> _providers = new ObservableCollection<Provider>();
        public ObservableCollection<Provider> Providers { get { return _providers; }} 
        //Словарь провайдеров, ключи - имена
        private readonly DicS<Provider> _providersDic = new DicS<Provider>();
        public DicS<Provider> ProvidersDic { get { return _providersDic; } } 

        //Колекция проектов
        private readonly ObservableCollection<Project> _projectsList = new ObservableCollection<Project>();
        public ObservableCollection<Project> ProjectsList { get { return _projectsList; }}

        //Для обновления строк в форме списка потоков
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}