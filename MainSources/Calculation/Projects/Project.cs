using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Interop.Access.Dao;
using VersionSynch;

namespace Calculation
{
    //Проект
    public class Project
    {
        //Создание проекта по указанному файлу file в потоке thread
        public Project(string file, ThreadCalc thread)
        {
            ThreadCalc = thread;
            File = file;
            Otm = true;
            //Проверка правильности проекта
            if (!DbVersion.IsProject(File))
            {
                thread.AddError("Указан недопустимый файл проекта", null, "Путь=" + File);
                return;
            }
            if (thread.ApplicationType == ApplicationType.Controller)
            {//Обновление версии файла проекта
                var vsyn = new DbVersion();
                vsyn.UpdateProjectVersion(File, false);
            }
            ReadSysTabl(false);
        }

        //Загрузка проекта из Projects
        public Project(IRecordRead rec, ThreadCalc thread) : this (rec.GetString("ProjectFile"), thread)
        {
            Id = rec.GetInt("ProjectId");
            Otm = rec.GetBool("Otm");
        }

        //Записывет данные в таблицу Projects в ControllerData, возвращает Id
        public int ToRecordset(RecDao rec, bool addnew=false)
        {
            if (addnew) rec.AddNew();
            rec.Put("ThreadId", ThreadCalc.Id);
            rec.Put("Project", Code);
            rec.Put("ProjectFile", File);
            rec.Put("Otm", Otm);
            Id = rec.GetInt("ProjectId");
            return Id;
        }

        //Открывает рекордсет Projects, записывает туда проект и закрывает
        public void AddToDb()
        {
            try
            {
                using ( var rec = new RecDao(General.ControllerFile, "SELECT * FROM Projects WHERE (ThreadId=" + Id + ")"))
                {
                    if (!rec.FindFirst("Project", Code)) rec.AddNew();
                    ToRecordset(rec);    
                }
            }
            catch(Exception ex)
            {
                ThreadCalc.AddError("Ошибка при записи проекта в ControllerData", ex, Code);
            }
        }

        //Открывает рекордсет Projects, удаляет оттуда проект и закрывает
        public void DeleteFromDb()
        {
            try
            {
                DaoDb.Execute(General.ControllerFile, "DELETE * FROM Projects WHERE (ThreadId=" + Id + ") And (Project='" + Code + "')");
            }
            catch (Exception ex)
            {
                ThreadCalc.AddError("Ошибка при удалении проекта из ControllerData", ex, Code);
            }
        }
        
        //Поток, содержащий данный проект
        public ThreadCalc ThreadCalc { get; set; }
        //Методы, перенесенные из ThreadCalc
        private Command Start(double start, double finish = 100)
        {
            return ThreadCalc.Start(start, finish);
        }
        private Command Start()
        {
            return ThreadCalc.Start();
        }
        private bool Start(Action action, double start, double finish = 100)
        {
            using (Start(start, finish))
            {
                action();
                return !IsError;
            }
        }
        private bool Start(Action action)
        {
            using (Start())
            {
                action();
                return !IsError;
            }
        }
        private void AddEvent(string description, string pars = "")
        {
            ThreadCalc.AddEvent(description, pars);
        }
        private void AddError(string text, Exception ex = null, string pars = "")
        {
            ThreadCalc.AddError(text, ex, pars, "Проект=" + Code);
        }
        private void AddWarning(string text, Exception ex = null, string pars = "")
        {
            ThreadCalc.AddError(text, ex, pars, "Проект=" + Code, false);
        }
        private bool IsError { get { return ThreadCalc.Command.IsError; }}
        private double Procent { set { ThreadCalc.Procent = value; }}
        
        //Код проекта
        public string Code { get; private set; }
        //Имя проекта
        public string Name { get; private set; }
        //Описание проекта
        public string Description { get; private set; }
        //Разные настройки для ведомости анализатора
        public string VedTag { get; private set; }
        //Файл .accdb с проектом
        public string File { get; private set; }
        //True, если проект включен в расчет
        public bool Otm { get; set; }
        //Id проекта в ControllerData
        public int Id { get; private set; }

        //Ссылка на имитатор
        public Imitator Imitator { get; set; }
        
        //Тип интерполяции
        public InterpolationType Interpolation { get; private set; }
        //Дата последней компиляции
        public DateTime LastChange { get; private set; }
        //Словарь провайдеров проекта, ключи - имена
        private readonly DicS<Provider> _providers = new DicS<Provider>();
        public DicS<Provider> Providers { get { return _providers; }}
        //Источник для ручного ввода
        public Provider HandInputProvider { get; private set; }

        //Словари сигналов из SignalsInUse для источников и для приемников
        private readonly DicS<CalcUnit> _signalsSources = new DicS<CalcUnit>();
        public DicS<CalcUnit> SignalsSources { get { return _signalsSources; }}
        private readonly DicS<CalcUnit> _signalsReceivers = new DicS<CalcUnit>();
        public DicS<CalcUnit> SignalsReceivers { get { return _signalsReceivers; } }
        //Набор используемых сигналов
        private readonly DicS<DataType> _signalsCodes = new DicS<DataType>();
        //Словарь объектов
        private readonly DicS<CalcObject> _objects = new DicS<CalcObject>();
        public DicS<CalcObject> Objects { get { return _objects; } }

        //Словари расчетных параметров, ключи - коды и id
        private readonly DicS<CalcParam> _calcParamsCode = new DicS<CalcParam>();
        public DicS<CalcParam> CalcParamsCode { get { return _calcParamsCode; } }
        private readonly DicI<CalcParam> _calcParamsId = new DicI<CalcParam>();
        public DicI<CalcParam> CalcParamsId { get { return _calcParamsId; } }
        //Словарь подпараметров, ключи - Id
        private readonly DicI<CalcParam> _calcSubParamsId = new DicI<CalcParam>();
        public DicI<CalcParam> CalcSubParamsId { get { return _calcSubParamsId; } }
        //Словарь графиков
        private readonly DicS<Grafic> _grafics = new DicS<Grafic>();
        public DicS<Grafic> Grafics { get { return _grafics; } }
        //Словарь таблиц, код - номер
        private readonly DicI<Tabl> _tabls = new DicI<Tabl>();
        public DicI<Tabl> Tabls { get { return _tabls; }}

        //Виртуальный корневой параметр проекта
        public CalcParamRun RootParam { get; private set; }
        //Проект архива
        public ArchiveProject ArchiveProject { get; set; }
        //Словарь расчетных параметров для накопления
        private readonly DicS<CalcParamArchive> _archiveParams = new DicS<CalcParamArchive>();
        public DicS<CalcParamArchive> ArchiveParams { get { return _archiveParams; }}
        //Архивный отчет для чтения последних данных по функциям Пред
        public ArchiveReport PrevLastReport { get; set; }
        //Архивный отчет для чтения данных по функциям Пред за период
        public ArchiveReport PrevManyReport { get; set; }
        //Словарь параметров функций Пред
        private readonly DicS<Prev> _prevParams = new DicS<Prev>();
        public DicS<Prev> PrevParams { get { return _prevParams; } }
        //Список ошибок последнего расчета
        private readonly List<ErrorCalc> _calcErrors = new List<ErrorCalc>();
        public List<ErrorCalc> CalcErrors { get { return _calcErrors; } }

        //Признаки наличия значений для записи в архив
        public bool HasHourValues { get; set; }
        public bool HasDayValues { get; set; }
        public bool HasAbsoluteValues { get; set; }
        //Признаки сохранения в архив
        public bool IsMoments { get; private set; }
        public bool IsPeriodic { get; private set; }
        public bool IsAbsolute { get; private set; }
        //Признаки использования функций Пред
        public bool IsPrevAbs { get; private set; }
        public bool IsLastBase { get; private set; }
        public bool IsLastHour { get; private set; }
        public bool IsLastDay { get; private set; }
        public bool IsManyBase { get; private set; }
        public bool IsManyHour { get; private set; }
        public bool IsManyDay { get; private set; }
        public bool IsManyMoments { get; private set; }

        //Интервалы для записи в архив
        public ArchiveInterval SingleInterval { get; private set; }
        public ArchiveInterval BaseInterval { get; private set; }
        public ArchiveInterval HourInterval { get; private set; }
        public ArchiveInterval DayInterval { get; private set; }
        public ArchiveInterval AbsoluteInterval { get; private set; }
        public ArchiveInterval AbsoluteDayInterval { get; private set; }
        public ArchiveInterval MomentsInterval { get; private set; }
        
        //Чтение иформации по проекту из файла проекта
        public void ReadProject()
        {
            ReadSysTabl(true);
            Procent = 5;
            if (IsError) return;
            if (ThreadCalc.IsReadSources || ThreadCalc.IsWriteReceivers)
            {
                Start(ReadSignals, 5, 15);
                if (IsError) return;
                if (ThreadCalc.ImitMode != ImitMode.NoImit && Imitator != null)
                    using (Start(15, 25)) Imitator.PrepareSignals(_signalsCodes);
                if (IsError) return;
                Start(JoinSignals, 25, 30);
            }
            if (!Start(ReadCalcParams, 30, 75)) return;
            //if (ThreadCalc.IsWriteArchives || ThreadCalc.IsWriteVed)
                ThreadCalc.Start(ReadArchiveParams, 75);
        }

        //Загрузка SysTabl проекта, needCompilation = true - ругается, если проект не скомпилирован
        public void ReadSysTabl(bool needCompilation)
        {
            try
            {
                using (var sys = new SysTabl(File))
                {
                    Code = sys.SubValue("ProjectInfo", "Project");
                    Name = sys.SubValue("ProjectInfo", "ProjectName");
                    Description = sys.SubValue("ProjectInfo", "ProjectDescription");
                    VedTag = sys.Tag("VedTag");
                    if (needCompilation)
                    {
                        Interpolation = sys.SubValue("ProjectInfo", "Interpolation").ToInterpolation();
                        LastChange = DateTime.Parse(sys.SubValue("CompileStatus", "LastTimeCompile"));
                    }
                    IsMoments = sys.SubValue("ArchiveOptions", "IsMoments") == "True";
                    IsPeriodic = sys.SubValue("ArchiveOptions", "IsPeriodic") == "True";
                    IsAbsolute = sys.SubValue("ArchiveOptions", "IsAbsolute") == "True";
                    IsPrevAbs = sys.SubValue("ArchiveOptions", "IsPrevAbs") == "True";
                    IsLastBase = sys.SubValue("ArchiveOptions", "IsLastBase") == "True";
                    IsLastHour = sys.SubValue("ArchiveOptions", "IsLastHour") == "True";
                    IsLastDay = sys.SubValue("ArchiveOptions", "IsLastDay") == "True";
                    IsManyBase = sys.SubValue("ArchiveOptions", "IsManyBase") == "True";
                    IsManyHour = sys.SubValue("ArchiveOptions", "IsManyHour") == "True";
                    IsManyDay = sys.SubValue("ArchiveOptions", "IsManyDay") == "True";
                    IsManyMoments = sys.SubValue("ArchiveOptions", "IsManyMoments") == "True";
                }
            }
            catch (Exception ex)
            {
                ThreadCalc.AddError("Недопустимые настройки в файле проекта (SysTabl) или проект никогда не копилировался", ex, "Путь=" + File);
            }
        }

        //Загрузка списка провайдеров
        public void ReadProviders()
        {
            try
            {
                Providers.Clear();
                using (var rec = new ReaderAdo(File, "SELECT * FROM Providers WHERE (ProviderType <> '" + "Коммуникатор" + "')"))
                {
                    while (rec.Read())
                    {
                        var p = new Provider(rec, ThreadCalc);
                        try { p.Codes = General.ProviderConfigs[p.Code].JointProviders;}
                        catch (Exception ex) { AddError("Не установлен провайдер", ex, p.Code); }

                        if (!ThreadCalc.ProvidersDic.ContainsKey(p.Name))
                        {
                            Providers.Add(p.Name, p);
                            ThreadCalc.ProvidersDic.Add(p.Name, p);
                            p.Projects.Add(Code);
                            if (p.Code == "HandInputSource" || p.Code == "HandInputSqlSource")
                                HandInputProvider = p;
                            AddEvent("Загружен провайдер");
                        }
                        else
                        {
                            var pr = ThreadCalc.ProvidersDic[p.Name];
                            Providers.Add(p.Name, pr);
                            pr.Projects.Add(Code);
                        }
                    }    
                }
            }
            catch (Exception ex)
            {
                ThreadCalc.AddError("Ошибка чтения списка провайдеров из проекта", ex, "Проект=" + Code);
            }
        }
        
        //Чтение списка сигналов
        private void ReadSignals()
        {
            AddEvent("Чтение списка сигналов");
            SignalsReceivers.Clear();
            if (ThreadCalc.IsReadSources)
            {
                SignalsSources.Clear();
                _signalsCodes.Clear();    
            }

            using (var rec = new ReaderAdo(File, "SELECT * FROM SignalsInUse"))
            {
                int i = 0, n = rec.RecordCount("SELECT Count(*) FROM SignalsInUse");
                while (rec.Read())
                {
                    var sig = new CalcUnit(rec, this, true);
                    if (IsError) return;
                    if (ThreadCalc.IsReadSources && !sig.SourceName.IsEmpty())
                    {
                        _signalsCodes.Add(sig.FullCode, sig.DataType);
                        SignalsSources.Add(sig.FullCode, sig);
                        if (sig.CodeObject != null)//null для сигналов ручного ввода
                        {
                            if (!Objects.ContainsKey(sig.CodeObject))
                                Objects.Add(sig.CodeObject, new CalcObject(sig.CodeObject));
                            var ob = Objects[sig.CodeObject];
                            ob.Signals.Add(sig.CodeSignal, sig);
                            sig.Object = ob;
                            if (sig.Default) ob.DefaultSignal = sig;
                        }
                    }
                    if (ThreadCalc.IsWriteReceivers && !sig.ReceiverName.IsEmpty()) 
                        SignalsReceivers.Add(sig.FullCode, sig);
                    if (++i % 50 == 0) Procent = i * 100.0 / n;
                }    
            }
        }

        //Присоединение к юнитам сигналов из провайдеров
        private void JoinSignals()
        {
            AddEvent("Присоединение сигналов из источников");
            foreach (var unit in SignalsSources.Values)
                unit.JoinSourceSignal();
            foreach (var unit in SignalsReceivers.Values)
                unit.JoinReceiverSignal();
        }

        //Чтение списка расчетных параметров
        private void ReadCalcParams()
        {
            CalcParamsCode.Clear();
            CalcParamsId.Clear();
            CalcSubParamsId.Clear();
            Grafics.Clear();
            AddEvent("Загрузка графиков");
            using (var db = new DaoDb(File))
            {
                try
                {
                    const string stSql =
                        "SELECT GraficsList.Code, GraficsList.Dimension, GraficsList.GraficType, GraficsValues.X1, GraficsValues.X2, GraficsValues.X3, GraficsValues.X4, GraficsValues.X5, GraficsValues.X6, GraficsValues.X7, GraficsValues.X8 " +
                        "FROM GraficsList INNER JOIN GraficsValues ON GraficsList.GraficId = GraficsValues.GraficId " +
                        "ORDER BY GraficsList.Code, GraficsValues.X8, GraficsValues.X7, GraficsValues.X6, GraficsValues.X5, GraficsValues.X4, GraficsValues.X3, GraficsValues.X2, GraficsValues.X1;";
                    using (var recg = new ReaderAdo(db, stSql))
                    {
                        //Считывание графиков
                        recg.Read();
                        while (!recg.EOF)
                        {
                            var gr = new Grafic(recg, ThreadCalc);
                            Grafics.Add(gr.Code, gr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка загрузки графика", ex);
                }
                Procent = 10;

                AddEvent("Загрузка параметров");
                try
                {
                    const string stSql = "SELECT * FROM CalcParams WHERE (TaskOn = True) AND (CalcOn = True)";
                    using (var rec = new ReaderAdo(db, stSql))
                        while (rec.Read())
                        {
                            var calc = new CalcParam(this, rec, false);
                            calc.FullCode = calc.Code;
                            CalcParamsId.Add(calc.Id, calc);
                            CalcParamsCode.Add(calc.Code, calc);
                            if (IsError) return;
                        }
                }
                catch (Exception ex)
                {
                    AddError("Список расчетных параметров загружен с ошибками, необходима повторная компиляция расчета", ex);
                }
                Procent = 40;

                AddEvent("Загрузка подпараметров");
                try
                {
                    const string stSql =
                        "SELECT CalcSubParams.* FROM CalcParams INNER JOIN CalcSubParams ON CalcParams.CalcParamId = CalcSubParams.OwnerId" +
                        " WHERE (CalcParams.TaskOn=True) AND (CalcParams.CalcOn=True) AND (CalcSubParams.CalcOn=True)";
                    using (var recp = new ReaderAdo(db, stSql))
                        while (recp.Read())
                        {
                            var calc = new CalcParam(this, recp, true);
                            CalcSubParamsId.Add(calc.Id, calc);
                            calc.Owner = CalcParamsId[recp.GetInt("OwnerId")];
                            calc.Owner.Methods.Add(calc.Code, calc);
                            calc.FullCode = calc.Owner.FullCode + "." + calc.Code;
                            if (IsError) return;
                        }
                }
                catch (Exception ex)
                {
                    AddError("Список расчетных параметров загружен с ошибками, необходима повторная компиляция расчета", ex);
                }
                Procent = 60;
            }

            AddEvent("Загрузка справочных таблиц");
            Tabls.Clear();
            using (var db = new DaoDb(File).ConnectDao())
                foreach (TableDef t in db.Database.TableDefs)
                    if (t.Name.StartsWith("Tabl_"))
                    {
                        var tabl = new Tabl(int.Parse(t.Name.Substring(5)));
                        Tabls.Add(tabl.Num, tabl);
                        tabl.FieldsCount = 0;
                        foreach (Field f in t.Fields)
                            if (f.Name.StartsWith("Val_"))
                            {
                                int fnum = int.Parse(f.Name.Substring(4));
                                if (fnum >= tabl.FieldsCount) tabl.FieldsCount = fnum + 1;
                            }
                        TableDef st = db.Database.TableDefs["Sub" + t.Name];
                        tabl.SubFieldsCount = 0;
                        foreach (Field f in st.Fields)
                            if (f.Name.StartsWith("SubVal_"))
                            {
                                int fnum = int.Parse(f.Name.Substring(7));
                                if (fnum >= tabl.SubFieldsCount) tabl.SubFieldsCount = fnum + 1;
                            }
                    }
            
            if (Tabls.Count > 0)
            {
                using (var db = new DaoDb(File))
                    foreach (var t in Tabls.Values)
                    {
                       using (var rect = new ReaderAdo(db, "SELECT * FROM Tabl_" + t.Num))
                            while (rect.Read())
                                new TablParam().ParamFromRec(t, rect);
                        using (var rect = new ReaderAdo(db, "SELECT * FROM SubTabl_" + t.Num))
                            while (rect.Read())
                                new TablParam().SubParamFromRec(t, rect);
                    }    
            }
            Procent = 75;

            AddEvent("Разбор выражений");
            try
            {
                foreach (var cp in CalcParamsId.Values)
                    if (!Start(cp.Parse)) break;
            }
            catch (Exception ex)
            {
                ThreadCalc.AddError("Список расчетных параметров загружен с ошибками, необходима повторная компиляция расчета", ex);
            }
        }

        //Чтение списка параметров для записи в архив
        public void ReadArchiveParams()
        {
            try
            {
                AddEvent("Загрузка архивных параметров");
                ArchiveProject = new ArchiveProject(Code, Name, ReportType.Calc, LastChange);
                ArchiveParams.Clear();
                using (var db = new DaoDb(File))
                {
                    using (var rec = new ReaderAdo(db, "SELECT CalcParamsArchive.* FROM CalcParamsArchive INNER JOIN CalcParams ON CalcParams.CalcParamId = CalcParamsArchive.CalcParamId WHERE CalcParams.CalcOn=True"))
                        while (rec.Read())
                        {
                            var par = new CalcParamArchive(rec, this);
                            ArchiveProject.AddParam(par.ArchiveParam);
                            ArchiveParams.Add(par.ArchiveParam.FullCode, par);
                        }

                    PrevParams.Clear();
                    if (IsLastBase || IsLastHour || IsLastDay)
                        PrevLastReport = new ArchiveReport(Code + "_Last_" + ThreadCalc.FullThreadName, Name, ReportType.Calc, LastChange);
                    if (IsManyBase || IsManyHour || IsManyDay || IsManyMoments)
                        PrevManyReport = new ArchiveReport(Code + "_Many_" + ThreadCalc.FullThreadName, Name, ReportType.Calc, LastChange);
                    using (var rec = new ReaderAdo(db, "SELECT * FROM PrevParams"))
                        while (rec.Read())
                        {
                            var p = new Prev(rec, this);
                            PrevParams.Add(p.Code, p);
                            if (p.LastReportParam != null) PrevLastReport.AddParam(p.LastReportParam);
                            if (p.ManyReportParam != null) PrevManyReport.AddParam(p.ManyReportParam);
                        }       
                }
            }
            catch(Exception ex)
            {
                AddError("Ошибка при чтении списка архивных параметров", ex);
            }
        }

        //Чтение значений из архива результатов
        public void PrepareArchive()
        {
            try
            {
                AddEvent("Подготовка списка параметров архива");
                using (Start(0, 60))
                    ThreadCalc.Archive.PrepareProject(ArchiveProject);
                if (PrevLastReport != null)
                {
                    AddEvent("Подготовка списка параметров запроса функций предыдущих последних значений");
                    using (Start(60, 80))
                        ThreadCalc.Archive.PrepareReport(PrevLastReport);
                }
                if (PrevManyReport != null)
                {
                    AddEvent("Подготовка списка параметров запроса функций предыдущих значений за период");
                    using (Start(80))
                        ThreadCalc.Archive.PrepareReport(PrevManyReport);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке или чтении архива результатов", ex);
            }
        }

        //Получение уже накопленных данных из архива
        public void ReadArchiveForPeriodic()
        {
            if (!IsPeriodic && !IsAbsolute) return;
            AddEvent("Чтение из архива результатов предыдущих расчетов");
            var intervals = ArchiveProject.IntervalsForRead;
            intervals.Clear();
            AbsoluteInterval = null;
            var beg = ThreadCalc.PeriodBegin;
            var en = ThreadCalc.PeriodEnd;
            var bhour = beg.AddMinutes(-beg.Minute);
            var bday = bhour.AddHours(-bhour.Hour);
            var aday = bday == beg ? bday : bday.AddDays(1);
            //Добавление интервалов для чтения из архива
            if (IsPeriodic)
            {
                if (beg.Minute != 0)
                    intervals.Add(new ArchiveInterval(IntervalType.Base, bhour, beg));
                if (beg.Hour != 0)
                    intervals.Add(new ArchiveInterval(IntervalType.Hour, bday, bhour));
            }
            if (IsAbsolute)
            {
                intervals.Add(new ArchiveInterval(IntervalType.Absolute, Different.MinDate, Different.MaxDate));
                intervals.Add(new ArchiveInterval(IntervalType.AbsoluteDay, bday, aday));
                //Абсолютные отредактированные значения
                if (ThreadCalc.Archive.IsAbsoluteEdited(Code))
                    foreach (var hip in ThreadCalc.Archive.ReadAbsoluteEdit(Code, true).Values)
                        if (ArchiveParams.ContainsKey(hip.Code))
                        {
                            var ap = ArchiveParams[hip.Code];
                            ap.AbsoluteEditValue = new Moment(ap.ArchiveParam.DataType, hip.Value, hip.Time);
                        }
            }

            //Чтение из архива
            using (Start(10, 70))
                ThreadCalc.Archive.ReadProject(ArchiveProject);
            foreach (var ap in ArchiveParams.Values)
                ap.FromArchiveParam(beg, en);
            
            if (IsAbsolute)
            {
                //Определение начала абсолютного интервала
                var abegin = Different.MaxDate;
                foreach (var ap in ArchiveParams.Values)
                    if (ap.AbsoluteValue != null && ap.AbsoluteValue.Time < abegin) 
                        abegin = ap.AbsoluteValue.Time;  
                AbsoluteInterval = new ArchiveInterval(IntervalType.Absolute, abegin != Different.MaxDate ? abegin : beg, beg);
            }
        }

        //Чтение предыдущих значений из архива
        public void ReadPrev()
        {
            if (PrevLastReport != null)
            {
                AddEvent("Чтение последних предыдущих значений");
                var intervals = PrevLastReport.IntervalsForRead;
                intervals.Clear();
                var en = ThreadCalc.PeriodBegin;
                var enhour = en.AddMinutes(-en.Minute);
                var enday = enhour.AddHours(-enhour.Hour);
                if (IsLastBase)  
                    intervals.Add(new ArchiveInterval(IntervalType.Base, en.AddMinutes(-ThreadCalc.PeriodLength), en, ThreadCalc.CalcName));
                if (IsLastHour)
                    intervals.Add(new ArchiveInterval(IntervalType.Hour, enhour.AddHours(-1), enhour, ThreadCalc.CalcName));
                if (IsLastDay)
                    intervals.Add(new ArchiveInterval(IntervalType.Day, enday.AddDays(-1), enday, ThreadCalc.CalcName));
                using (Start(5, 40))
                    ThreadCalc.Archive.ReadReport(PrevLastReport);
            }
            if (PrevManyReport != null)
            {
                AddEvent("Чтение предыдущих значений за период");
                var intervals = PrevManyReport.IntervalsForRead;
                intervals.Clear();
                var en = ThreadCalc.PeriodBegin;
                if (IsManyMoments)
                    intervals.Add(new ArchiveInterval(IntervalType.Moments, en.AddDays(-1).AddHours(-1), en, ThreadCalc.CalcName));
                if (IsManyBase)
                    intervals.Add(new ArchiveInterval(IntervalType.Base, en.AddHours(-6), en, ThreadCalc.CalcName));
                if (IsManyHour)
                    intervals.Add(new ArchiveInterval(IntervalType.Hour, en.AddDays(-1).AddHours(-1), en, ThreadCalc.CalcName));
                if (IsManyDay)
                    intervals.Add(new ArchiveInterval(IntervalType.Day, en.AddDays(-12), en, ThreadCalc.CalcName));
                using (Start(5, 40))
                    ThreadCalc.Archive.ReadReport(PrevManyReport);
            }
            if (PrevLastReport != null || PrevManyReport != null)
            {
                AddEvent("Получение данных из архивных параметров");
                foreach (var p in PrevParams.Values)
                    p.FromArchiveParam();   
            }
        }

        //Расчет по всем формулам
        public void Calculate()
        {
            try
            {
                CalcErrors.Clear();
                RootParam = new CalcParamRun();
                int n = CalcParamsId.Count, i = 0;
                foreach (var c in CalcParamsId.Values)
                {
                    if (c == null)
                        AddError("Список расчетных параметров загружен с ошибками, необходима повторная компиляция расчета");
                    else
                    {
                        c.RunParam = null;
                        foreach (var m in c.Methods.Values)
                        {
                            if (m == null)
                                AddError("Список расчетных параметров загружен с ошибками, необходима повторная компиляция расчета");
                            else m.RunParam = null;
                        }
                    }
                }
                if (IsError) return;

                foreach (var c in CalcParamsId.Values)
                {
                    if (c.IsNotObject)
                    {
                        c.Calculate();
                        foreach (var m in c.Methods.Values)
                            if (m.IsNotObject)
                                m.Calculate();
                    }
                    if (IsError) return;
                    if (n > 40 && ++i%(n/20) == 0) Procent = 80.0*i/n;
                }

                //Запись ошибок расчета в лог
                var count = CalcErrors.Count;
                if (count > 0)
                {
                    string s = "";
                    int j = 0;
                    while (j < 10 && j < count)
                        s += (s == "" ? "" : ", ") + CalcErrors[j++].Address;
                    string p = count%10 == 1 ? "параметр" : (count%10 == 2 || count%10 == 3 || count%10 == 4 ? "параметра" : "параметров");
                    AddWarning("При расчете произошли ошибки", null, count + " " + p + " с ошибками: " + s + (j < count ? " и др." : ""));
                }
                AddEvent("Объем используемой памяти", GC.GetTotalMemory(false).ToString());
            }
            catch (OutOfMemoryException ex)
            {
                AddEvent("Объем используемой памяти", GC.GetTotalMemory(false).ToString());
                AddError("Ошибка при расчете", ex);
                throw;
            }
            catch(Exception ex)
            {
                AddError("Ошибка при расчете", ex);
            }
        }

        //Сохраняет результаты расчета в таблицы
        public void SaveCalcDebug()
        {
            if (!ThreadCalc.IsSaveParams && !ThreadCalc.IsSaveProperties && !ThreadCalc.IsSaveVariables) return;
            try
            {
                AddEvent("Сохранение значений в таблицу");
                using (var sav = new ResultSaver(Code, ThreadCalc))
                {
                    Procent = 10;
                    int i = 0;
                    foreach (var c in CalcParamsId.Values)
                        if (c.Inputs.Count == 0)
                        {
                            sav.SaveValues(c.FullCode, "Параметр", c.Id, c.RunParam, null, 0);
                            if (++i % 20 == 0) Procent = 10 + 89.0 * i / CalcParamsId.Count;
                        }
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка подготовки отладочного файла Result.accdb", ex);
            }
        }

        //Получение данных для записи в архив
        public void Accumulate()
        {
            try
            {
                var forWrite = ArchiveProject.IntervalsForWrite;
                forWrite.Clear();
                var en = ThreadCalc.PeriodEnd;
                var beg = ThreadCalc.PeriodBegin;
                if (!ThreadCalc.IsPeriodic)
                {
                    SingleInterval = new ArchiveInterval(ThreadCalc.WriteArchiveType, beg, en, ThreadCalc.CalcName);
                    forWrite.Add(SingleInterval);
                }
                else
                {
                    if (IsMoments)
                    {
                        MomentsInterval = new ArchiveInterval(IntervalType.Moments, beg, en, ThreadCalc.CalcName);
                        forWrite.Add(MomentsInterval);
                    }
                    if (IsPeriodic)
                    {
                        BaseInterval = new ArchiveInterval(IntervalType.Base, beg, en, ThreadCalc.CalcName);
                        forWrite.Add(BaseInterval);
                        if (en.Minute == 0)
                        {
                            HourInterval = new ArchiveInterval(IntervalType.Hour, en.AddHours(-1), en, ThreadCalc.CalcName);
                            forWrite.Add(HourInterval);
                        }
                        if (en.Hour == 0 && en.Minute == 0)
                        {
                            DayInterval = new ArchiveInterval(IntervalType.Day, en.AddDays(-1), en, ThreadCalc.CalcName);      
                            forWrite.Add(DayInterval);
                        }
                        HasHourValues = false;
                        HasDayValues = false;
                    }
                    if (IsAbsolute)
                    {
                        AbsoluteInterval = new ArchiveInterval(IntervalType.Absolute, AbsoluteInterval.Begin, en, ThreadCalc.CalcName);
                        forWrite.Add(AbsoluteInterval);
                        if (en.Hour == 0 && en.Minute == 0)
                        {
                            AbsoluteDayInterval = new ArchiveInterval(IntervalType.AbsoluteDay, AbsoluteInterval.Begin, en, ThreadCalc.CalcName);      
                            forWrite.Add(AbsoluteDayInterval);
                        }
                        HasAbsoluteValues = false;
                    }
                }
                Procent = 10;

                foreach (var c in ArchiveParams.Values)
                    c.Accumulate();

                if (IsPeriodic)
                {
                    if (!HasHourValues) forWrite.Remove(HourInterval);
                    if (!HasDayValues) forWrite.Remove(DayInterval);
                }
                if (IsAbsolute && !HasAbsoluteValues)
                {
                    forWrite.Remove(AbsoluteInterval);
                    if (en.Hour == 0 && en.Minute == 0) forWrite.Remove(AbsoluteDayInterval);
                }

                if (PrevLastReport != null || PrevManyReport != null)
                {
                    Procent = 70;
                    AddEvent("Обновление списков предыдущих значений");
                    foreach (var p in PrevParams.Values)
                        p.Accumulate();
                }
            }
            catch(Exception ex)
            {
                AddEvent("Объем используемой памяти", GC.GetTotalMemory(false).ToString()); 
                AddError("Ошибка при накоплении", ex);
            }
        }

        //Освобождает всю память, занятую мгновенными данными проекта
        public void ClearMemory()
        {
            AddEvent("Объем используемой памяти", GC.GetTotalMemory(false).ToString());
            RootParam = null;
            foreach (var par in CalcParamsId.Values)
            {
                par.RunParam = null;
                foreach (var sub in par.Methods.Values)
                    sub.RunParam = null;
            }
            foreach (var ap in ArchiveParams.Values)
                ap.RunParam = null;
            GC.Collect();
        }
    }
}