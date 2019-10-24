using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BaseLibrary;
using System.Linq;
using System.Threading;
using CommonTypes;
using Microsoft.Office.Interop.Access.Dao;

namespace Tablik
{
    //Работа с одним конкретным проектом
    public class TablikCompiler : Logger
    {
        //Конструктор для работы из других приложений
        public TablikCompiler()
        {
            try
            {
                ThreadName = "Tablik";
                State = State.Empty;
                ShowIndicator = true;
                var screen = Screen.PrimaryScreen;
                Form = new TablikForm();
                Form.Location = new Point(screen.WorkingArea.Width - Form.Width - 1, screen.WorkingArea.Height - Form.Height - 2);
                _infoTaskDir = Different.GetInfoTaskDir();
                _tmpDir = _infoTaskDir + @"\Tmp";
                var d = new DirectoryInfo(_tmpDir);
                if (!d.Exists) d.Create();
                LoadFunctions();
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка загрузки копилятора", "Системная ошибка компилятора");
            }
        }

        //True, если нужно отображать окно с индикатором
        public bool ShowIndicator { get; set; }

        //Путь к каталогу InfoTask 
        private readonly string _infoTaskDir;
        //Путь к каталогу Tmp
        private readonly string _tmpDir;

        //Файл проекта
        private string _projectFile;
        public string ProjectFile { get { return _projectFile; }}
        //Рабочий файл проекта
        private string _workFile;
        public string WorkFile { get { return _workFile; }}
        //Код и имя проекта
        private string _code;
        public string ProjectCode { get { return _code; } }
        private string _name;
        public string ProjectName { get { return _name; } }
        //Имя источника ручного ввода
        public string HandInputSource { get; private set; }

        //Количество сигналов в проекте
        public int SignalsCount
        {
            get { return SignalsList.Count; }
        }
        //Количество используемых сигналов
        public int SignalsInUseCount { get; private set; }

        //Количество расчитываемых расчетных параметров
        public int CalcParamsCount
        {
            get { return CalcParamsId.Count; }
        }
        //Количество расчитываемых архивных параметров
        public int ArchiveParamsCount { get; internal set; }
        //Количество параметров с ошибками
        public int ErrorsCount { get; internal set; }
        //Количество сигналов и объектов с ошибками
        public int SignalsErrorsCount { get; private set; }
        public int ObjectsErrorsCount { get; private set; }

        //Компилировать только отмеченные формулы
        public bool OnlyOn { get; set; }

        //Словари всех функций ключи - коды или Id
        private readonly DicS<FunClass> _funs = new DicS<FunClass>();
        internal DicS<FunClass> Funs { get { return _funs; } }
        private readonly DicI<FunClass> _funsId = new DicI<FunClass>();
        internal DicI<FunClass> FunsId { get { return _funsId; } }

        //Словарь объектов
        private readonly DicS<ObjectSignal> _objects = new DicS<ObjectSignal>();
        internal DicS<ObjectSignal> Objects { get { return _objects; }}
        //Список сигналов
        private readonly List<Signal> _signalsList = new List<Signal>();
        internal List<Signal> SignalsList { get { return _signalsList; } }
        //Словарь сигналов
        private readonly DicS<Signal> _signals = new DicS<Signal>();
        internal DicS<Signal> Signals { get { return _signals; } }
        //Словарь сигнал ручного ввода
        private readonly DicS<Signal> _handSignals = new DicS<Signal>();
        internal DicS<Signal> HandSignals { get { return _handSignals; } }

        //Словарь расчетных параметров, ключи - коды, содержит только отмеченные и без грубых ошибок
        private readonly DicS<CalcParam> _calcParams = new DicS<CalcParam>();
        internal DicS<CalcParam> CalcParams { get { return _calcParams; } }
        //Словарь всех расчетных параметров, ключи - коды
        private readonly DicS<CalcParam> _calcParamsAll = new DicS<CalcParam>();
        internal DicS<CalcParam> CalcParamsAll { get { return _calcParamsAll; } }
        //Словарь расчетных параметров, ключи - Id, содержит все параметры
        private readonly DicI<CalcParam> _calcParamsId = new DicI<CalcParam>();
        internal DicI<CalcParam> CalcParamsId { get { return _calcParamsId; } }
        //Словарь графиков, ключ - код графика в нижнем регистре
        private readonly DicS<Grafic> _grafics = new DicS<Grafic>();
        internal DicS<Grafic> Grafics { get { return _grafics; } }
        //Словарь архивных параметров
        private readonly DicS<ParamArchive> _archiveParams = new DicS<ParamArchive>();
        internal DicS<ParamArchive> ArchiveParams { get { return _archiveParams; }}
        //Функции Пред, используемые в параметрах, ключи - коды параметров в нижнем регистре
        private readonly DicS<Prev> _prevs = new DicS<Prev>();
        internal DicS<Prev> Prevs { get { return _prevs; } }

        //Текущий номер расчетного параметра
        internal int CalcParamNumber { get; set; }
        //Используемые провайдеры
        internal SetS UsedProviders { get; private set; }
        //Признаки использования архивных интервалов
        internal bool IsMoments;
        internal bool IsAbsolute;
        internal bool IsPeriodic;
        //Признаки использования функций Пред
        internal bool IsPrevAbs;
        internal bool IsLastBase;
        internal bool IsLastHour;
        internal bool IsLastDay;
        internal bool IsManyBase;
        internal bool IsManyHour;
        internal bool IsManyDay;
        internal bool IsManyMoments;

        //ab
        //Файл списка объекта
        private string _objectsFile;
        public string ObjectsFile { get { return _objectsFile; } }
        //\ab
        
        //Загрузка списка функций
        public void LoadFunctions()
        {
            try
            {
                Funs.Clear();
                using (var g = new DaoDb(_infoTaskDir + @"\General\General.accdb"))
                {
                    using (var rec = new ReaderAdo(g, "SELECT * FROM Functions WHERE NotLoad = False"))
                        while (rec.Read())
                        {
                            //Сначала читаем сами функции
                            var f = new FunClass(rec);
                            Funs.Add(f.Name, f);
                            if (f.Synonym != null) Funs.Add(f.Synonym, f);
                            FunsId.Add(rec.GetInt("Id"), f);
                        }
                    
                    using (var rec = new ReaderAdo(g, "SELECT FunctionsOverloads.* FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId " +
                                "WHERE Functions.NotLoad = False ORDER BY FunctionsOverloads.FunctionId, FunctionsOverloads.RunNumber"))
                    {
                        rec.Read();
                        while (!rec.EOF)
                        {  //Потом их перегрузки
                            var id = rec.GetInt("FunctionId");
                            var fun = _funsId[id];
                            while (rec.GetInt("FunctionId") == id)
                            {
                                fun.Overloads.Add(new FunOverload(rec, fun));
                                if (!rec.Read()) break;
                            }
                        }
                    }    
                }
            }
            catch (Exception ex)
            {
                ex.MessageError("Системная ошибка компилятора. Ошибка загрузки функций");
            }
        }

        //Все функции возвращают ошибку или ""
        //Задает путь к файлу HistoryTablik.accdb
        public string SetHistoryFile(string historyFile)
        {
            using (Start(true))
            {
                if (State == State.Closed)
                    AddError("Копилятор уже был закрыт");
                else
                {
                    try
                    {
                        if (historyFile.IsEmpty())
                            throw new NullReferenceException("Путь к файлу не может быть пустой строкой или null");
                        OpenHistory(historyFile, _infoTaskDir + @"General\HistoryTemplate.accdb");
                    }
                    catch (Exception ex)
                    {
                        AddError("Ошибка открытия таблицы истории", ex);
                    }
                }
                return Command.ErrorMessage();
            }
        }

        //Задает путь к каталогу рабочих файлов
        private string _compiledDir;
        public string SetCompiledDir(string compiledDir)
        {
            using (var c = StartAtom(Atom.SetCompiled, false, compiledDir))
            {
                if (State == State.Closed)
                    AddError("Копилятор уже был закрыт");
                else
                {
                    try
                    {
                        var d = new DirectoryInfo(compiledDir);
                        if (!d.Exists) d.Create();
                        _compiledDir = compiledDir;
                    }
                    catch (Exception ex)
                    {
                        AddError("Недопустимый каталог скомпилированных проектов", ex, compiledDir);
                    }
                }
                return c.ErrorMessage();
            }
        }

        //Обнуление количеств ошибок
        private void CountsToZero()
        {
            ObjectsErrorsCount = 0;
            SignalsErrorsCount = 0;
            ErrorsCount = 0;
        }

        //Загрузить проект projectFile, compileDir - каталог для компиляции
        public string LoadProject(string projectFile)
        {
            StartAtom(Atom.LoadProject, true, projectFile);
            CountsToZero();
            if (State == State.Closed)
                AddError("Копилятор уже был закрыт");
            else
            {
                try
                {
                    if (!DaoDb.Check(projectFile, new[] { "CalcParams", "CalcSubParams", "CalcParamsArchive", "Objects", "Signals", "SignalsInUse", "GraficsList", "GraficsValues" }))
                        AddError("Указан неправильный файл проекта", null, projectFile);
                    else
                    {
                        using (var db = new DaoDb(projectFile))
                        {
                            using (var rec = new RecDao(db, "SELECT * FROM Providers WHERE (ProviderCode='HandInputSource') OR (ProviderCode='HandInputSqlSource')"))
                                if (rec.HasRows())
                                    HandInputSource = rec.GetString("ProviderName");
                            using (var sys = new SysTabl(db))
                            {
                                _code = sys.SubValue("ProjectInfo", "Project");
                                _name = sys.SubValue("ProjectInfo", "ProjectName");
                                sys.PutSubValue("ProjectInfo", "HandInputSource", HandInputSource);

                                //ab
                                _objectsFile = sys.Value("ObjectsFile");
                                if (string.IsNullOrEmpty(_objectsFile)) _objectsFile = projectFile;
                                if (_objectsFile != projectFile)
                                    if (!DaoDb.Check(projectFile, new[] { "CalcParams", "CalcSubParams", "CalcParamsArchive", "Objects", "Signals", "SignalsInUse", "GraficsList", "GraficsValues" }))
                                    {
                                        AddError("В проекте указан неправильный файл списка объектов", null, projectFile);
                                        _objectsFile = projectFile;
                                    }
                                //\ab
                            }
                        }
                        _projectFile = projectFile;
                    }
                }
                catch (Exception ex)
                {
                    AddError("Указан неправильный файл проекта", ex, projectFile);
                }   
            }
            return FinishAtom(State.Project, State.Empty, "Проект:" + _code + ", " + _name);
        }

        //Проверка таблицы сигналов
        public string CheckSignals(bool onlyInUse = false)
        {
            StartAtom(Atom.CheckSignals);
            CountsToZero();
            var objectsId = new DicI<ObjectSignal>();
            var signals = new DicS<Signal>();
            _objects.Clear();
            try
            {
                if (State == State.Closed)
                    AddError("Копилятор уже был закрыт");
                else if (State == State.Empty)
                    AddError("Проект не загружен");
                else
                {
                    //using (var reco = new RecDao(_projectFile, "SELECT CodeObject, NameObject, TagObject, ObjectId, CommName, ErrMess FROM Objects ORDER BY ObjectId"))
                    using (var reco = new RecDao(_objectsFile, "SELECT CodeObject, NameObject, TagObject, ObjectId, CommName, ErrMess FROM Objects ORDER BY ObjectId")) //ab\
                    {
                        Procent = 10;
                        while (reco.Read())
                        {
                            var ob = new ObjectSignal(reco, true);
                            objectsId.Add(ob.Id, ob);
                            if (!_objects.ContainsKey(ob.Code))
                                _objects.Add(ob.Code, ob);
                            else
                            {
                                ob.ErrMess += "Повтор кода объекта (" + ob.Code + "); ";
                                reco.Put("ErrMess", ob.ErrMess);
                                _objects[ob.Code].ErrMess += "Повтор кода объекта (" + ob.Code + "); ";
                            }
                        }
                        Procent = 25;
                        using (var recs = new RecDao(reco.DaoDb, "SELECT ObjectId, Signals.Default, CodeSignal, NameSignal, Units, Signals.DataType, ConstValue, Signals.SourceName, Signals.ReceiverName, Signals.Inf, Signals.FullCode AS FullCode, Signals.ErrMess" + (!onlyInUse ? "" : ", SignalsInUse.FullCode AS FullCodeInUse") 
                            + " FROM Signals" + (!onlyInUse ? "" : " LEFT JOIN SignalsInUse ON Signals.FullCode=SignalsInUse.FullCode ")))
                        {
                            Procent = 35;
                            while (recs.Read())
                            {
                                var sig = new Signal(recs, objectsId, true);
                                if (onlyInUse) sig.InUse = !recs.GetString("FullCodeInUse").IsEmpty();
                                objectsId[sig.ObjectId].InUse |= sig.InUse;
                                if (signals.ContainsKey(sig.FullCode))
                                    sig.ErrMess += "Повтор полного кода сигнала (" + sig.FullCode + "); ";
                                else signals.Add(sig.FullCode, sig);
                                if (sig.ErrMess != "")
                                {
                                    recs.Put("ErrMess", sig.ErrMess, true);
                                    if (sig.InUse || !onlyInUse) SignalsErrorsCount++;
                                    objectsId[sig.ObjectId].ErrorInSignals = true;
                                }
                            }
                        }
                        Procent = 80;
                        reco.MoveFirst();
                        while (!reco.EOF)
                        {
                            var ob = objectsId[reco.GetInt("ObjectId")];
                            if (ob.DefalutsCount == 0) ob.ErrMess += "Объект не содержит сигналов по умолчанию; ";
                            if (ob.DefalutsCount >= 2) ob.ErrMess += "Объект содержит более одного сигнала по умолчанию; ";
                            if (ob.ErrorInSignals) ob.ErrMess += "Ошибки в сигналах; ";
                            if (ob.ErrMess != "")
                            {
                                reco.Put("ErrMess", ob.ErrMess);
                                if (!onlyInUse || ob.InUse) ObjectsErrorsCount++;
                            }
                            reco.MoveNext();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return ex.MessageError("Ошибка при проверке сигналов");
            }
            return FinishAtom(State.Project, State.Project, "Проект: " + _code + "; Объектов с ошибками: " + ObjectsErrorsCount + "; Сигналов с ошибками: " + SignalsErrorsCount);
        }

        //Загрузить список сигналов, возвращает ошибку или ""
        public string LoadSignals()
        {
            StartAtom(Atom.LoadSignals);
            CountsToZero();
            if (State == State.Closed)
                AddError("Копилятор уже был закрыт");
            else if (State == State.Empty)
                AddError("Проект не загружен");
            else
            {
                try
                {
                    var objectsId = new DicI<ObjectSignal>();
                    Signals.Clear();
                    SignalsList.Clear();
                    //using (var db = new DaoDb(_projectFile))
                    using (var db = new DaoDb(_objectsFile)) //ab\
                    {
                        using (var reco = new ReaderAdo(db, "SELECT CodeObject, NameObject, TagObject, ObjectId, CommName FROM Objects ORDER BY ObjectId"))
                        {
                            AddEvent("Открыт рекордсет объектов");
                            Procent = 5;
                            while (reco.Read())
                            {
                                var curo = new ObjectSignal(reco, false);
                                Objects.Add(curo.Code, curo);
                                objectsId.Add(curo.Id, curo);
                            }
                            AddEvent("Объекты загружены");
                        }
                        Thread.Sleep(50);

                        Procent = 20;
                        using (var recs = new ReaderAdo(db, "SELECT ObjectId, Default, CodeSignal, NameSignal, Units, DataType, ConstValue, SourceName, ReceiverName, Inf, FullCode FROM Signals ORDER BY ObjectId, SignalId"))
                        {
                            int i = 0, n = recs.RecordCount("SELECT Count(*) FROM Signals");
                            Procent = 30;
                            AddEvent("Открыт рекордсет сигналов");
                            if (n != 0)
                                while (recs.Read())
                                {
                                    var sig = new Signal(recs, objectsId, false);
                                    SignalsList.Add(sig);
                                    Signals.Add(sig.FullCode, sig);
                                    if (sig.Default && objectsId.ContainsKey(sig.ObjectId))
                                        Signals.Add(objectsId[sig.ObjectId].Code, sig);
                                    if (n > 20 && i % (n / 15) == 0) Procent = (i++ * 70) / n + 30;
                                }
                        }
                        AddEvent("Сигналы загружены");
                    }    
                }
                catch (Exception ex)
                {
                    //ab
                    //AddError("Ошибка загрузки сигналов", ex);
                    AddError("Ошибка загрузки сигналов: " + ex.Message, ex);
                    //\ab
                }    
            }
            return FinishAtom(State.Signals, State.Project, "Проект: " + _code + @";  Сигналов: " + SignalsList.Count);
        }

        //Компиляция проекта, возвращает количество ошибок компиляции
        public string CompileProject()
        {
            //OnlyOn = true;
            CountsToZero();
            if (State == State.Project || State == State.Empty)//Если не загружены, то загружаем сигналы
            {
                string s = LoadSignals();
                if (s != "") return s;
            }
            else
            {
                foreach (var sig in SignalsList)
                {
                    sig.InUse = false;
                    sig.InUseSource = false;
                    sig.InUseReceiver = false;
                }
            }
            StartAtom(Atom.CompileProject);
            if (State == State.Closed)
                AddError("Копилятор уже был закрыт");
            else
            {
                try
                {
                    //Обнуления поля Expr и т.п.
                    using (var daodb = new DaoDb(_projectFile))
                    {
                        //daodb.Execute("UPDATE CalcParams SET CalcParams.Expr = Null, CalcParams.ErrMess = Null, CalcParams.UsedUnits = Null, CalcParams.CalcNumber = 0;");
                        foreach (string tab in new[] {"CalcParams", "CalcSubParams"})
                            using (var rec = new RecDao(daodb, tab))
                                while (rec.Read())
                                {
                                    rec.Put("Expr", (string) null);
                                    try { rec.Put("UsedUnits", (string) null); } catch { }
                                    rec.Put("CalcNumber", 0);
                                    rec.Put("ErrMess", (string) null);
                                    rec.Update();
                                }
                        CalcParams.Clear();
                        CalcParamsId.Clear();
                        CalcParamsAll.Clear();
                        Grafics.Clear();
                        ArchiveParams.Clear();
                        Prevs.Clear();
                        HandSignals.Clear();
                        CalcParamNumber = 0;

                        Procent = 10;
                        AddEvent("Загрузка графиков");
                        using (var rec = new RecDao(daodb, "SELECT GraficsList.Code, GraficsList.Dimension FROM GraficsList"))
                            while (rec.Read())
                            {
                                var gr = new Grafic(rec.GetString("Code"), rec.GetInt("Dimension"));
                                Grafics.Add(gr.Code, gr);
                            }

                        Procent = 15;
                        AddEvent("Загрузка параметров");
                        string spar = OnlyOn ? "WHERE (CalcOn = True) And (TaskOn = True)" : "";
                        using (var rec = new RecDao(daodb, "SELECT CalcParams.* FROM CalcParams " + spar + " ORDER BY CalcParamId"))
                            while (rec.Read())
                                new CalcParam(rec, this);

                        Procent = 25;
                        AddEvent("Загрузка подпараметров");
                        string ssub = OnlyOn ? "WHERE (CalcParams.CalcOn = True) And (CalcParams.TaskOn = True) And (CalcSubParams.CalcOn = True)" : "";
                        using (var rec = new RecDao(daodb, "SELECT CalcSubParams.* FROM CalcParams INNER JOIN CalcSubParams ON CalcParams.CalcParamId = CalcSubParams.OwnerId " + ssub))
                            while (rec.Read())
                                new CalcParam(rec, this, true);

                        Procent = 35;
                        AddEvent("Разбор выражений");
                        foreach (var cp in CalcParamsId.Values)
                            cp.Parse();

                        IsPrevAbs = false;
                        IsLastBase = false;
                        IsLastHour = false;
                        IsLastDay = false;
                        IsManyBase = false;
                        IsManyHour = false;
                        IsManyDay = false;
                        IsManyMoments = false;

                        Procent = 45;
                        AddEvent("Компиляция выражений");
                        foreach (var cp in CalcParamsId.Values)
                        {
                            if (cp.Stage == CompileStage.NotStarted)
                                cp.Compile(null);
                            /*foreach (var d in cp.MethodsId.Values)
                                if (d.Stage == CompileStage.NotStarted)
                                    d.Compile(null);*/
                        }

                        FindCycleLinks();

                        ErrorsCount = 0;
                        UsedProviders = new SetS();
                        IsAbsolute = false;
                        IsPeriodic = false;
                        IsMoments = false;

                        Procent = 60;
                        AddEvent("Сохранение результатов компиляции параметров");
                        using (var rec = new RecDao(daodb, "SELECT CalcParams.* FROM CalcParams " + spar + " ORDER BY CalcParamId"))
                            SaveCompile(rec, false);

                        Procent = 70;
                        AddEvent("Сохранение результатов компиляции подпараметров");
                        using (var rec = new RecDao(daodb, "SELECT CalcSubParams.* FROM CalcParams INNER JOIN CalcSubParams ON CalcParams.CalcParamId = CalcSubParams.OwnerId " + ssub + " ORDER BY CalcParams.CalcParamId;"))
                            SaveCompile(rec, true);
                    }

                    Procent = 80;
                    AddEvent("Сохранение списка используемых сигналов и графиков");
                    SaveInUse();
                    
                    Procent = 90;
                    AddEvent("Сохранение архивных параметров");
                    SaveArchive();

                    Procent = 96;
                    AddEvent("Сохранение параметров функций Пред");
                    SavePrevs();

                    Procent = 99;
                    SaveUsedProviders();
                    using (var sys = new SysTabl(_projectFile))
                    {
                        sys.PutSubValue("CompileStatus", "LastTimeCompile", DateTime.Now.ToString());
                        sys.PutSubValue("CompileStatus", "ErrorsCount", ErrorsCount.ToString());    
                    }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка компилятора", ex);
                }
            }
            return FinishAtom(State.Compiled, State.Signals, "Проект: " + _code + ";  Ошибок: " + ErrorsCount + ";" + Different.NewLine 
                + "Параметров: " + CalcParamsId.Count + "; Архивных параметров:" + ArchiveParams.Count + ";  Сигналов: " + SignalsInUseCount);
        }

        //Проверка на циклические ссылки
        public void FindCycleLinks()
        {
            foreach (var cp in _calcParamsId.Values)
            {
                cp.Stage = CompileStage.NotStarted;
                foreach (var m in cp.Methods.Values)
                    m.Stage = CompileStage.NotStarted;
            }
            foreach (var cp in _calcParamsId.Values)
            {
                if (cp.Stage == CompileStage.NotStarted)
                    cp.CheckCycleLinks(null);
                foreach (var m in cp.Methods.Values)
                    if (m.Stage == CompileStage.NotStarted)
                        m.CheckCycleLinks(null);
            }
        }

        //Создание рабочего файла, возвращает ошибку или ""
        public string MakeWorkFile()
        {
            if (State != State.Compiled)
            {
                string s = CompileProject();
                if (s != "") return s;
            }
            StartAtom(Atom.MakeWorkFile);
            if (State == State.Closed)
                AddError("Копилятор уже был закрыт");
            else
            {
                try
                {
                    var f = new FileInfo(_projectFile);
                    string cd = _compiledDir;
                    if (cd.IsEmpty()) cd = f.DirectoryName;
                    cd = (cd ?? "").EndsWith("\\") ? cd : cd + "\\";
                    _workFile = cd + f.Name.Substring(0, f.Name.Length - f.Extension.Length) + "_Work" + f.Extension;
                    Procent = 10;
                    AddEvent("Копирование в новый файл", _workFile);
                    f.CopyTo(_workFile, true);

                    Procent = 30;
                    AddEvent("Очистка расчетных параметров");
                    using (var db = new DaoDb(_workFile))
                    {
                        db.Execute("DELETE CalcParams.* FROM CalcParams WHERE CalcOn=False");
                        db.Execute("DELETE CalcSubParams.* FROM CalcSubParams WHERE CalcOn=False");
                        Procent = 40;
                        db.Execute("UPDATE CalcParams SET UserExpr1=Null, UserExpr2=Null");
                        db.Execute("UPDATE CalcSubParams SET UserExpr1=Null, UserExpr2=Null");

                        Procent = 50;
                        AddEvent("Удаление таблиц");
                        db.Database.TableDefs.Delete("Signals");
                        db.Database.TableDefs.Delete("Objects");
                        db.Database.TableDefs.Delete("TracesTemplates");
                    }

                    Procent = 60;
                    AddEvent("Сжатие базы", _workFile);
                    DaoDb.Compress(_workFile, 20000000, _tmpDir, 2000);
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при сжатии скомпилированного проекта", ex);
                }   
            }
            return FinishAtom(State.Compiled, State.Signals, "Проект: " + _code + ";  Ошибок: " + ErrorsCount + ";" + Different.NewLine 
                 + "Параметров: " + CalcParamsId.Count + "; Архивных параметров:" + ArchiveParams.Count + ";  Сигналов: " + SignalsInUseCount);
        }

        //Закрытие проекта, возвращает ошибку или ""
        public string Close()
        {
            try
            {
                using (var c = Start())
                {
                    if (State == State.Closed)
                        AddError("Копилятор уже был закрыт");
                    else
                    {
                        StartAtom(Atom.Close, false);
                        FinishAtom(State.Closed, State.Closed);
                        try { Form.Close(); } catch { }
                        UpdateHistory(false);
                        CloseHistory();
                        State = State.Closed;
                    }
                    return c.ErrorMessage();
                }
            }
            finally
            {
                GC.Collect();
            }
        }
        
        //Запись скомпилированных выражений в таблицу
        private void SaveCompile(RecDao rec, bool isSubParam)
        {
            try
            {
                while (!rec.EOF)
                {
                    CalcParam calc = !isSubParam ?
                        CalcParamsId[rec.GetInt("CalcParamId")] :
                        CalcParamsId[rec.GetInt("OwnerId")].MethodsId[rec.GetInt("CalcParamId")];
                    if (calc != null) calc.SaveCompile(rec);
                    rec.MoveNext();
                }    
            }
            catch(Exception ex)
            {
                AddError("Ошибка сохранения параметров", ex);
            }
        }

        //Формирование списка параметров для записи в архив и запись их в CalcParamsArchive
        private void SaveArchive()
        {
            try
            {
                using (var db = new DaoDb(_projectFile))
                {
                    foreach (var cp in CalcParamsId.Values)
                        if (cp.Inputs.Count == 0 && cp.CalcOn && cp.ErrMess == "")
                            new ParamArchive(cp);    

                    var old = new SetS();
                    var add = new SetS();
                    using (var rec = new RecDao(db, "CalcParamsArchive", RecordsetTypeEnum.dbOpenTable))
                        while (rec.Read())
                        {
                            var code = rec.GetString("FullCode");
                            if (ArchiveParams.ContainsKey(code))
                                old.Add(code);
                            else rec.Put("Del", true);
                        }
                    db.Execute("DELETE * FROM CalcParamsArchive WHERE Del=True");

                    foreach (var ap in ArchiveParams.Keys)
                        if (!old.Contains(ap)) add.Add(ap);
                    using (var rec = new RecDao(db, "CalcParamsArchive", RecordsetTypeEnum.dbOpenTable))
                    {
                        while (rec.Read())
                            ArchiveParams[rec.GetString("FullCode")].ToRecordset(rec, false);
                        foreach (var p in add.Keys)
                            ArchiveParams[p].ToRecordset(rec, true);
                    }
                }
            }
            catch(Exception ex)
            {
                AddError("Ошибка сохранения архивных параметров", ex);
            }
        }

        //Запись в таблицу PrevParams
        private void SavePrevs()
        {
            try
            {
                using (var db = new DaoDb(_projectFile))
                {
                    var old = new SetS();
                    var add = new SetS();
                    using (var rec = new RecDao(db, "PrevParams"))
                        while (rec.Read())
                        {
                            var code = rec.GetString("FullCode");
                            if (Prevs.ContainsKey(code) && Prevs[code].Id != 0)
                            {
                                old.Add(code);
                                rec.Put("Del", false);
                            }
                            else rec.Put("Del", true);
                        }
                    db.Execute("DELETE * FROM PrevParams WHERE Del=True");

                    foreach (var prev in Prevs.Keys)
                        if (!old.Contains(prev)) add.Add(prev);
                    if (Prevs.Count > 0)
                        using (var rec = new RecDao(db, "PrevParams"))
                        {
                            while (rec.Read())
                                Prevs[rec.GetString("FullCode")].ToRecordset(rec, false);
                            foreach (var p in add.Keys)
                                Prevs[p].ToRecordset(rec, true);
                        }

                    foreach (var p in Prevs.Values)
                    {
                        IsPrevAbs |= p.PrevAbs;
                        IsLastBase |= p.LastBase;
                        IsLastHour |= p.LastHour;
                        IsLastDay |= p.LastDay;
                        IsManyBase |= p.ManyBase;
                        IsManyHour |= p.ManyHour;
                        IsManyDay |= p.ManyDay;
                        IsManyMoments |= p.ManyMoments;
                    }
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка сохранения параметров фунций Пред", ex);
            }
        }

        //Запись используемых сигналов в SignalsInUse
        private void SaveInUse()
        {
            try
            {
                using (var db = new DaoDb(_projectFile))
                {
                    var old = new SetS();
                    using (var rec = new RecDao(db, "SignalsInUse"))
                    {
                        while (rec.Read())
                        {
                            var code = rec.GetString("FullCode");
                            if ((Signals.ContainsKey(code) && Signals[code].InUse) || HandSignals.ContainsKey(code))
                            {
                                old.Add(code);
                                rec.Put("Del", false);
                            }
                            else rec.Put("Del", true);
                        }
                    }
                    db.Execute("DELETE * FROM SignalsInUse WHERE Del=True");
                    var add = new SetS();
                    var use = from s in SignalsList where s.InUse select s;
                    SignalsInUseCount = use.Count();
                    foreach (var s in use)
                    {
                        if (!old.Contains(s.FullCode)) 
                            add.Add(s.FullCode);
                        if (s.InUseSource) UsedProviders.Add(s.SourceName);
                        if (s.InUseReceiver) UsedProviders.Add(s.ReceiverName);
                    }
                    if (HandSignals.Count > 0 && HandInputSource != null)
                        UsedProviders.Add(HandInputSource);
                    foreach (var hand in HandSignals.Values)
                        if (!old.Contains(hand.FullCode))
                            add.Add(hand.FullCode);

                    using (var rec = new RecDao(db, "SignalsInUse"))
                    {
                        while (rec.Read())
                        {
                            var s = rec.GetString("FullCode");
                            Signal sig = Signals.ContainsKey(s) ? Signals[s] : HandSignals[s];
                            sig.ToRecordset(rec, false);
                        }
                        foreach (var s in add.Keys)
                        {
                            Signal sig = Signals.ContainsKey(s) ? Signals[s] : HandSignals[s];
                            sig.ToRecordset(rec, true);
                        }
                    }
                    using (var rec = new RecDao(db, "GraficsList"))
                        while (rec.Read())
                            rec.Put("UsedUnits", UsingParamsString(Grafics[rec.GetString("Code")].UsingParams));
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка сохранения сигналов или графиков", ex);
            }
        }

        //Формирует строку параметров, использующих данный параметр или сигнал
        internal static string UsingParamsString(DicS<CalcParam> pars)
        {
            string s = "";
            foreach (var par in pars.Values)
            {
                s += par.FullCode + "=Pr_";
                if (par.Owner == null) s += "Параметр";
                else s += "Подпараметр";
                if (par.Inputs.Count != 0) s += "-функция";
                s += "|";
            }
            return s;
        }

        //Отметка используемых провайдеров и ручного ввода
        private void SaveUsedProviders()
        {
            try
            {
                using (var db = new DaoDb(_projectFile))
                {
                    using (var rec = new RecDao(db, "Providers"))
                        while (rec.Read())
                            switch (rec.GetString("ProviderType").ToProviderType())
                            {
                                case ProviderType.Communicator:
                                    rec.Put("IsUsed", true);
                                    break;
                                case ProviderType.Source:
                                case ProviderType.Receiver:
                                    rec.Put("IsUsed", UsedProviders.Contains(rec.GetString("ProviderName")));
                                    break;
                                case ProviderType.Archive:
                                    rec.Put("IsUsed", UsedProviders.Contains("Archive"));
                                    break;
                            }

                    using (var sys = new SysTabl(db))
                    {
                        sys.PutSubValue("ArchiveOptions", "IsAbsolute", IsAbsolute ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsPeriodic", IsPeriodic ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsMoments", IsMoments ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsPrevAbs", IsPrevAbs ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsLastBase", IsLastBase ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsLastHour", IsLastHour ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsLastDay", IsLastDay ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsManyBase", IsManyBase ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsManyHour", IsManyHour ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsManyDay", IsManyDay ? "True" : "False");
                        sys.PutSubValue("ArchiveOptions", "IsManyMoments", IsManyMoments ? "True" : "False");
                    }    
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка сохранения признаков использования провайдеров и ручного ввода", ex);
            }            
        }
        
        //-----------------------------------------------------------------------------------------------------------------------
        //Работа с историей и статус баром

        //Окно приложения
        internal TablikForm Form;
        //Соятояние данных проекта
        internal State State;

        //Создает новую атомарную комманду
        private CommandLog StartAtom(Atom atom, bool isView = true, string pars = "")
        {
            if (isView && ShowIndicator)
            {
                if (!Form.Visible) Form.Show();
                Form.SetOperaton(atom.RunMessage());
            }
            return StartLog(atom.RunMessage(), pars, "", "", true);
        }
        //Завершение атомарной комманды
        private string FinishAtom(State stateSuccess, State stateError, string results = "")
        {
            State = !Command.IsError ? stateSuccess : stateError;
            string s = Finish(results).ErrorMessage();
            if (Form.Visible) Form.Hide();
            return s;
        }

        //Реализация абстрактных методов

        protected override void FinishLogCommand() { }

        protected override void FinishSubLogCommand() { }

        protected override void FinishProgressCommand() {}

        protected override void MessageError(ErrorCommand er) { }

        protected override void ViewProcent(double procent)
        {
            if (Form.Visible) Form.SetProcent(procent);
        }
    }
}