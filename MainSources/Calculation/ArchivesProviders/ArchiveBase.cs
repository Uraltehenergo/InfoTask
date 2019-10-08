using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;
using VersionSynch;

namespace Calculation
{
    //Класс для работы с архивами, не зависимо от их типа
    public abstract class ArchiveBase : ProviderBase
    {
        protected ArchiveBase(DatabaseType databaseType, string name, string inf, ProviderSetupType setupType, Logger logger) : base (name, logger)
        {
            DatabaseType = databaseType;
            SetupType = setupType;
            Inf = inf;
        }

        //Тип - архив
        public override ProviderType Type { get { return ProviderType.Archive; } }
        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = Inf.ToPropertyDicS();
                dic.DefVal = "";
                if (DatabaseType == DatabaseType.Access)
                {
                    _databaseFile = dic["DatabaseFile"];
                    Hash = "AccessDb=" + _databaseFile;
                    _reserveDir = dic["ReserveDir"];
                    _reserveFrequency = dic.GetInt("ReserveFrequency");
                    _addDate = dic.GetBool("AddDate");
                    if (DbVersion.IsArchive(_databaseFile)) 
                        new DbVersion().UpdateArchiveVersion(_databaseFile, false);
                }
                else
                {
                    bool e = dic.Get("IndentType","").ToLower() != "windows";
                    string server = dic["SQLServer"], db = dic["Database"];
                    _sqlProps = new SqlProps(server, db, e, dic["Login"], dic["Password"]);
                    Hash = "SQLServer=" + server + ";Database=" + db;
                }
                _singleTime = dic.GetInt("SingleTime", -1);
                _baseTime = dic.GetInt("BaseTime", -1);
                _hourTime = dic.GetInt("HourTime", -1);
                _dayTime = dic.GetInt("DayTime", -1);
                _absoluteDayTime = dic.GetInt("AbsoluteDayTime", -1);
                _momentsTime = dic.GetInt("MomentsTime", -1);
            }
        }

        //Тип базы данных
        public DatabaseType DatabaseType { get; private set; }
        
        //Путь к файлу accdb
        private string _databaseFile;
        //Частота резервного копирования
        private int _reserveFrequency;
        //Каталог резерного копирования
        private string _reserveDir;
        //Довавлять дату в название резервного файла
        private bool _addDate;

        //Настройки на архив SqlServer
        private SqlProps _sqlProps;

        //Ограничения накопления в архив по типам интервалов, измеряются в сутках
        private int _singleTime;
        private int _baseTime;
        private int _hourTime;
        private int _dayTime;
        private int _absoluteDayTime;
        private int _momentsTime;

        //Соединение проверено
        internal bool IsConnected { get; private set; }

        public bool Check()
        {
            //Access
            if (DatabaseType == DatabaseType.Access)
            {
                if (DbVersion.IsArchive(_databaseFile)) 
                    return IsConnected = true;    
                Logger.AddError("Не найден или недопустимый файл архива", null, "", Context);
                return IsConnected = false;
            }
            //SQL Server
            try
            {
                using (SqlDb.Connect(_sqlProps))
                    return IsConnected = true;
            }
            catch (Exception ex)
            {
                Logger.AddError("Не удалось соединиться с SQL-сервером", ex, "", Context);
                return IsConnected = false;
            }
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (DatabaseType == DatabaseType.Access)
            {
                if (inf["DatabaseFile"].IsEmpty())
                    err += "Не указан файл архива\n";
                err += PropIsInt("ReserveFrequency", inf, names, 0, 400);
            }
            else
            {
                if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера\n";
                if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации\n";
                if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин\n";
                if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных\n";
            }
            err += PropIsInt("SingleTime", inf, names, 1, 100000);
            err += PropIsInt("BaseTime", inf, names, 1, 400);
            err += PropIsInt("HourTime", inf, names, 1, 4000);
            err += PropIsInt("DayTime", inf, names, 1, 100000);
            err += PropIsInt("AbsoluteDayTime", inf, names, 1, 100000);
            err += PropIsInt("MomentsTime", inf, names, 1, 4000);
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (!Check()) 
            {
                Logger.AddError(CheckConnectionMessage = "Не удалось соединиться с " + (DatabaseType == DatabaseType.Access ? "архивом" : "SQL-сервером"));
                return false;
            }
            try
            {
                ExecuteDelete("TestConnection");
                using (var reb = Adder("TestConnection"))
                {
                    reb.AddNew();
                    reb.Put("Num", 1);
                    reb.Put("Strin", "1s");
                    reb.AddNew();
                    reb.Put("Num", 2);
                    reb.Put("Strin", "2s");
                    reb.Update();
                }
                using (var rec = Writer("SELECT * FROM TestConnection"))
                {
                    rec.MoveFirst();
                    rec.Put("Strin", "1n");
                    rec.AddNew();
                    rec.Put("Num", 3);
                    rec.Put("Strin", "3s");
                    rec.Update();
                }
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            catch
            {
                Logger.AddError(CheckConnectionMessage = "Неправильный формат базы данных архива или база недоступна");
                return false;
            }
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        public void Dispose()
        {
            CloseConnections();
        }

        //Словарь внутренних проектов, ключи архивные проекты
        private readonly DicS<InnerArchiveProject> _innerProjects = new DicS<InnerArchiveProject>();
        //Словарь внутренних проектов-отчетов, ключи архивные отчеты
        private readonly DicS<InnerArchiveProject> _innerReports = new DicS<InnerArchiveProject>();

        //Текущeе соединение
        private DaoDb _daoDb;

        //Открывает рекордсет для чтения
        private IRecordRead Reader(string stSql)
        {
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    if (_daoDb == null) _daoDb = new DaoDb(_databaseFile);
                    return new ReaderAdo(_daoDb, stSql);
                case DatabaseType.SqlServer:
                    return new ReaderAdo(_sqlProps, stSql);
            }
            return null;
        }

        //Открывает рекордсет для добавления записей
        private IRecordAdd Adder(string stSql)
        {
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    if (_daoDb == null) _daoDb = new DaoDb(_databaseFile);
                    return new RecDao(_daoDb, stSql);
                case DatabaseType.SqlServer:
                    return new BulkSql(_sqlProps, stSql);
            }
            return null;
        }

        //Открывает рекордсет со всеми возможностями
        private IRecordSet Writer(string stSql)
        {
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    if (_daoDb == null) _daoDb = new DaoDb(_databaseFile);
                    return new RecDao(_daoDb, stSql);
                case DatabaseType.SqlServer:
                    return new DataSetSql(_sqlProps, stSql);
            }
            return null;
        }

        //Выполняет комманду 
        private void ExecuteCommand(string stSql)
        {
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    if (_daoDb == null) _daoDb = new DaoDb(_databaseFile);
                    _daoDb.Execute(stSql);
                    break;
                case DatabaseType.SqlServer:
                    SqlDb.Execute(_sqlProps, stSql);
                    break;
            }
        }
        //Выполняет комманду DELETE tabl.* stSql
        private void ExecuteDelete(string tabl, string stSql = null)
        {
            string from = stSql ?? ("FROM " + tabl);
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    if (_daoDb == null) _daoDb = new DaoDb(_databaseFile);
                    _daoDb.Execute("DELETE " + tabl + ".* " + from);
                    break;
                case DatabaseType.SqlServer:
                    SqlDb.Execute(_sqlProps, "DELETE " + tabl + " " + from);
                    break;
            }
        }
        //Закрывает все соединения с базами
        private void CloseConnections()
        {
            if (_daoDb != null)
                try { _daoDb.Dispose(); }
                catch { }
            _daoDb = null;
        }

        //Перезагружает данные а DataSetSql
        private void Reload(IRecordSet rec)
        {
            rec.Update();
            if (DatabaseType == DatabaseType.SqlServer)
                ((DataSetSql)rec).Reload();
        }

        //Преобразует дату в строку для запроса
        private string DateStr(DateTime d)
        {
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    return d.ToAccessString();
                case DatabaseType.SqlServer:
                    return d.ToSqlString();
            }
            return d.ToString();
        }

        //Возвращает строку True или False для запросов
        private string BoolStr(bool b)
        {
            if (DatabaseType == DatabaseType.Access)
                return b ? "True" : "False";
            return b ? "1" : "0";
        }

        //Оболочка для запуска опасной операции
        private void ADanger(Action action, int repetiotion, int errorWaiting, string operationForError)
        {
            _action = action;
            _opertionForError = operationForError;
            Logger.Danger(ATry, repetiotion, errorWaiting, "Ошибка " + _opertionForError);
        }
        //Вспомогательные переменные
        private Action _action;
        private string _opertionForError;
        //Оболочка для опасной операции
        private bool ATry()
        {
            if (!IsConnected && !Logger.Danger(Check, 2, 200, "Ошибка соединения с архивом")) return false;
            try
            {
                _action();
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка " + _opertionForError, ex);
                IsConnected = false;
            }
            CloseConnections();
            return !Logger.Command.IsError;
        }

        //Параметры для использования в опасных операциях
        private ArchiveProject _project;
        private ArchiveReport _report;
        private string _projectName;

        //Добавление проекта и параметров, если нужно. Загрузка Id параметров
        public void PrepareProject(ArchiveProject project)
        {
            _project = project;
            ADanger(TryPrepareProject, 2, 2000, "подготовки архивного проекта");
        }

        private void TryPrepareProject()
        {
            _innerProjects.Add(_project.Code, new InnerArchiveProject(_project));
            var iproject = _innerProjects[_project.Code];
            Logger.AddEvent("Получение Id проекта");
            bool needUpdate = true;
            string stp = "SELECT * FROM Projects WHERE (Project='" + _project.Code + "') AND (ProjectType='" + _project.Type.ToRussian() + "')";
            using (var rec = Writer(stp))
            {
                if (rec.HasRows()) needUpdate &= (rec.GetTime("SourceChange") != _project.SourceChange);
                else rec.AddNew();
                _project.ToRecordset(rec);
                Reload(rec);
                rec.MoveFirst();
                _project.Id = rec.GetInt("ProjectId");
            }

            Logger.AddEvent("Обновление и получение Id параметров");
            using (var rec = Writer("SELECT * FROM Params WHERE ProjectId=" + _project.Id))
            {
                if (needUpdate)
                {
                    var set = new SetS();
                    while (rec.Read())
                    {
                        var k = rec.GetString("Code");
                        if (_project.Params.ContainsKey(k))
                        {
                            _project.Params[k].ToRecordset(rec, _project.Id);
                            set.Add(k);
                        }
                        else rec.Put("Active", false);
                    }
                    foreach (var k in _project.Params.Dic)
                        if (!set.Contains(k.Key))
                            k.Value.ToRecordset(rec, _project.Id, true);
                }
                Reload(rec);
                rec.MoveFirst();
                while (!rec.EOF)
                {
                    var code = rec.GetString("Code");
                    if (_project.Params.ContainsKey(code))
                        _project.Params[code].Id = rec.GetInt("ParamId");
                    rec.MoveNext();
                }
            }

            //Подготовка InnerProject
            iproject.ParamsId.Clear();
            iproject.HasValues.Clear();
            iproject.HasStrValues.Clear();
            foreach (var par in _project.Params.Values)
                if (!iproject.ParamsId.ContainsKey(par.Id))
                {
                    iproject.ParamsId.Add(par.Id, par);
                    AddHasValue(iproject, par, IntervalType.Single);
                    AddHasValue(iproject, par, IntervalType.Named);
                    AddHasValue(iproject, par, IntervalType.NamedAdd);
                    AddHasValue(iproject, par, IntervalType.NamedAddParams);
                    if (par.SuperProcess == SuperProcess.Moment)
                        AddHasValue(iproject, par, IntervalType.Moments);
                    if (par.SuperProcess.IsPeriodic())
                    {
                        AddHasValue(iproject, par, IntervalType.Base);
                        AddHasValue(iproject, par, IntervalType.Hour);
                        AddHasValue(iproject, par, IntervalType.Day);
                    }
                    if (par.SuperProcess.IsAbsolute())
                    {
                        AddHasValue(iproject, par, IntervalType.Absolute);
                        AddHasValue(iproject, par, IntervalType.AbsoluteDay);
                    }    
                }
        }

        private void TryPrepareReport()
        {
            _innerReports.Add(_report.Code, new InnerArchiveProject(_report));
            var iproject = _innerReports[_report.Code];
            bool e = false;
            DateTime change = Different.MinDate;
            var str = "SELECT * FROM Reports WHERE (Report='" + _report.Code + "') AND (ReportType='" + _report.Type.ToRussian() + "')";
            using (var rec = Writer(str))
            {
                if (rec.HasRows())
                {
                    //Проверка, обновился ли отчет
                    e |= (rec.GetTime("SourceChange") != _report.SourceChange);
                    change = rec.GetTime("TimeChange");
                }
                else
                {
                    rec.AddNew();
                    e = true;
                }
                _report.ToRecordset(rec);

                //Проверка, обновились ли проекты
                using (var recp = Reader("SELECT * FROM Projects"))
                    while (recp.Read())
                        if (_report.Projects.ContainsKey(recp.GetString("Project")))
                            e |= (recp.GetTime("TimeChange") > change);

                if (e) rec.Put("TimeChange", DateTime.Now.ToString());
                Reload(rec);
                rec.MoveFirst();
                _report.Id = rec.GetInt("ReportId");
            }

            if (e) ExecuteDelete("ReportParams", "FROM ReportParams WHERE ReportId=" + _report.Id);
            using (var rec = Writer("SELECT * FROM ReportParams WHERE ReportId=" + _report.Id))
            {
                if (e)
                {
                    //Обновление таблицы параметров отчетов
                    var rea = Reader("SELECT Projects.Project, Params.Code, Params.ParamId FROM Projects INNER JOIN Params ON Projects.ProjectId = Params.ProjectId");
                    while (rea.Read())
                    {
                        var proj = rea.GetString("Project");
                        var pcode = rea.GetString("Code");
                        if (_report.Projects.ContainsKey(proj) && _report.Projects[proj].ContainsKey(pcode))
                        {
                            var par = _report.Projects[proj][pcode];
                            par.ParamId = rea.GetInt("ParamId");
                            par.ToRecordset(rec, _report.Id, true);
                        }
                    }
                }
                Reload(rec);
                //Загрузка ParamId из таблицы
                rec.MoveFirst();
                while (!rec.EOF)
                {
                    var proj = rec.GetString("Project");
                    string pcode = rec.GetString("Code").ToLower();
                    if (_report.Projects.ContainsKey(proj) && _report.Projects[proj].ContainsKey(pcode))
                    {
                        var par = _report.Projects[proj][pcode];
                        par.ParamId = rec.GetInt("ParamId");
                    }
                    rec.MoveNext();
                }
            }

            //Подготовка InnerProject
            iproject.ReportParamsId.Clear();
            iproject.HasValues.Clear();
            iproject.HasStrValues.Clear();
            foreach (var proj in _report.Projects.Values)
            {
                foreach (var par in proj.Values)
                {
                    if (par.ParamId != 0)
                    {
                        iproject.ReportParamsId.Add(par.ParamId, par);
                        AddHasValue(iproject, par, IntervalType.Single);
                        AddHasValue(iproject, par, IntervalType.Named);
                        if (par.SuperProcess == SuperProcess.Moment)
                            AddHasValue(iproject, par, IntervalType.Moments);
                        if (par.SuperProcess.IsPeriodic())
                        {
                            AddHasValue(iproject, par, IntervalType.Base);
                            AddHasValue(iproject, par, IntervalType.Hour);
                            AddHasValue(iproject, par, IntervalType.Day);
                        }
                        if (par.SuperProcess.IsAbsolute())
                        {
                            AddHasValue(iproject, par, IntervalType.Absolute);
                            AddHasValue(iproject, par, IntervalType.AbsoluteDay);
                        }
                    }
                }
            }
        }

        //Добавление отчета и параметров, если нужно. Загрузка Id параметров
        public void PrepareReport(ArchiveReport report)
        {
            _report = report;
            ADanger(TryPrepareReport, 2, 2000, "подготовки отчета");
        }

        //Добавляет тип интервала в HasValues или HasStrValues
        private void AddHasValue(InnerArchiveProject iproject, ArchiveParam par, IntervalType it)
        {
            if (par.DataType.LessOrEquals(DataType.Real))
            {
                if (!iproject.HasValues.Contains(it))
                    iproject.HasValues.Add(it);
            }
            else
            {
                if (!iproject.HasStrValues.Contains(it))
                    iproject.HasStrValues.Add(it);
            }
        }

        //Добавляет тип интервала в HasValues или HasStrValues для отчета
        private void AddHasValue(InnerArchiveProject iproject, ArchiveReportParam par, IntervalType it)
        {
            if (par.DataType.LessOrEquals(DataType.Real))
            {
                if (!iproject.HasValues.Contains(it))
                    iproject.HasValues.Add(it);
            }
            else
            {
                if (!iproject.HasStrValues.Contains(it))
                    iproject.HasStrValues.Add(it);
            }
        }

        private void TryReadProject()
        {
            InnerArchiveProject iproject = _innerProjects[_project.Code];
            if (_project.IntervalsForRead.Count == 0) return;
            foreach (var p in _project.Params.Values)
                p.Intervals.Clear();
            foreach (var t in _project.IntervalsForRead)
            {
                Logger.AddEvent("Чтение значений интервалов " + t.Type.ToRussian(), t.Begin + " - " + t.End);
                ReadOrderInterval(t, iproject);
            }
        }

        //Чтение из архива заданных интервалов вместе со значениями
        public void ReadProject(ArchiveProject project)
        {
            _project = project;
            ADanger(TryReadProject, 2, 3000, "чтения накопленных значений");
        }

        private void TryReadReport()
        {
            InnerArchiveProject iproject = _innerReports[_report.Code];
            if (_report.IntervalsForRead.Count == 0) return;
            foreach (var proj in _report.Projects.Values)
                foreach (var par in proj.Values)
                    foreach (var t in par.Queries.Values)
                    {
                        if (t.SingleValue.Moments != null) t.SingleValue.Moments.Clear();
                        t.Intervals.Clear();
                    }

            foreach (var t in _report.IntervalsForRead)
            {
                switch (t.Type)
                {
                    case IntervalType.Moments:
                    case IntervalType.Single:
                    case IntervalType.Named:
                    case IntervalType.Absolute:
                    case IntervalType.AbsoluteDay:
                    case IntervalType.Base:
                        ReadOrderInterval(t, iproject, t.Type);
                        break;
                    case IntervalType.Combined:
                        DateTime bh = t.Begin.Minute == 0 && t.Begin.Second == 0 ? t.Begin : t.Begin.AddSeconds(-t.Begin.Second).AddMinutes(-t.Begin.Minute).AddHours(1);
                        DateTime bd = bh.Hour == 0 ? bh : bh.AddHours(-bh.Hour).AddDays(1);
                        DateTime eh = t.End.AddSeconds(-t.End.Second).AddMinutes(-t.End.Minute);
                        DateTime ed = eh.AddHours(-eh.Hour);
                        if (bh.Subtract(t.End).TotalSeconds > -1)
                            ReadForCombined(iproject, IntervalType.Base, t.Begin, t.End);
                        else
                        {
                            if (bh.Subtract(t.Begin).TotalSeconds >= 1)
                                ReadForCombined(iproject, IntervalType.Base, t.Begin, bh);
                            if (bd.Subtract(eh).TotalSeconds > -1)
                                ReadForCombined(iproject, IntervalType.Hour, bh, eh);
                            else
                            {
                                if (bd.Subtract(bh).TotalSeconds >= 1)
                                    ReadForCombined(iproject, IntervalType.Hour, bh, bd);
                                if (ed.Subtract(bd).TotalSeconds >= 1)
                                    ReadForCombined(iproject, IntervalType.Day, bd, ed);
                                if (eh.Subtract(ed).TotalSeconds >= 1)
                                    ReadForCombined(iproject, IntervalType.Hour, ed, eh);
                            }
                            if (t.End.Subtract(eh).TotalSeconds >= 1)
                                ReadForCombined(iproject, IntervalType.Base, eh, t.End);
                        }
                        break;
                    case IntervalType.Hour:
                        if (t.Begin.Minute > 0 || t.End.Minute > 0)
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Base, t.Begin, t.End), iproject, t.Type);
                        else ReadOrderInterval(t, iproject, t.Type);
                        break;
                    case IntervalType.Day:
                        if (t.Begin.Minute > 0 || t.End.Minute > 0)
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Base, t.Begin, t.End), iproject, t.Type);
                        else if (t.Begin.Hour > 0 || t.End.Hour > 0)
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Hour, t.Begin, t.End), iproject, t.Type);
                        else ReadOrderInterval(t, iproject, t.Type);
                        break;
                    case IntervalType.AbsoluteCombined:
                        eh = t.End.AddSeconds(-t.End.Second).AddMinutes(-t.End.Minute);
                        ed = eh.AddHours(-eh.Hour);
                        ReadOrderInterval(new ArchiveInterval(IntervalType.AbsoluteDay, ed, ed), iproject, t.Type);
                        if (eh.Subtract(ed).TotalSeconds >= 1)
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Hour, ed, eh), iproject, t.Type);
                        if (t.End.Subtract(eh).TotalSeconds >= 1)
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Base, eh, t.End), iproject, t.Type);
                        break;
                    case IntervalType.AbsoluteListBase:
                        bd = t.Begin.AddMinutes(-t.Begin.Minute).AddHours(-t.Begin.Hour);
                        eh = t.End.AddMinutes(-t.End.Minute).AddHours(-t.End.Hour);
                        ReadOrderInterval(new ArchiveInterval(IntervalType.AbsoluteDay, bd, bd), iproject, t.Type);
                        ReadOrderInterval(new ArchiveInterval(IntervalType.Base, bd, t.End), iproject, t.Type);
                        break;
                    case IntervalType.AbsoluteListHour:
                        bd = t.Begin.AddMinutes(-t.Begin.Minute).AddHours(-t.Begin.Hour);
                        eh = t.End.AddMinutes(-t.End.Minute).AddHours(-t.End.Hour);
                        ReadOrderInterval(new ArchiveInterval(IntervalType.AbsoluteDay, bd, bd), iproject, t.Type);
                        if (t.Begin.Minute > 0 || t.End.Minute > 0)
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Base, bd, t.End), iproject, t.Type);
                        else ReadOrderInterval(new ArchiveInterval(IntervalType.Hour, bd, t.End), iproject, t.Type);;
                        break;
                    case IntervalType.AbsoluteListDay:
                        bd = t.Begin.AddMinutes(-t.Begin.Minute).AddHours(-t.Begin.Hour);
                        ed = t.End.AddMinutes(-t.End.Minute).AddHours(-t.End.Hour);
                        if (t.Begin.Minute > 0 || t.End.Minute > 0)
                        {
                            ReadOrderInterval(new ArchiveInterval(IntervalType.AbsoluteDay, bd, bd), iproject, t.Type);
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Base, bd, t.End), iproject, t.Type);
                        }
                        else if (t.Begin.Hour > 0 || t.End.Hour > 0)
                        {
                            ReadOrderInterval(new ArchiveInterval(IntervalType.AbsoluteDay, bd, bd), iproject, t.Type);
                            ReadOrderInterval(new ArchiveInterval(IntervalType.Hour, bd, t.End), iproject, t.Type);
                        }
                        else ReadOrderInterval(new ArchiveInterval(IntervalType.AbsoluteDay, bd, ed), iproject, t.Type);
                        break;
                }
            }
        }

        //Чтение из архива значений по отчету
        public void ReadReport(ArchiveReport report)
        {
            _report = report;
            ADanger(TryReadReport, 2, 3000, "чтения значений для отчета");
        }

        private void ReadForCombined(InnerArchiveProject iproject, IntervalType type, DateTime beg, DateTime en)
        {
            ReadOrderInterval(new ArchiveInterval(type, beg, en), iproject, IntervalType.Combined);
        }

        //Чтение данных по одному интервалу заказа для проекта или отчета
        //t - интервал заказа, iproject - внутренний проект проекта или отчета, isProject - true, если проект, false - если отчет
        private void ReadOrderInterval(ArchiveInterval t, InnerArchiveProject iproject, IntervalType orderType = IntervalType.Empty)
        {
            Logger.AddEvent("Чтение значений интервалов " + t.Type.ToRussian(), t.Begin + " - " + t.End);
            bool isReport = orderType != IntervalType.Empty;
            string tabl = t.Type.ToValuesTable();
            string stabl = t.Type.ToStrValuesTable();
            string itabl = t.Type.ToIntervalsTable();
            SingleType singleType = t.Type.IsSingle() || t.Type == IntervalType.Moments ? SingleType.List : SingleType.Moment;
            string s = "SELECT " + itabl + ".TimeEnd, " + itabl + ".TimeBegin, " + itabl + ".IntervalName, ";
            string std = s + tabl + ".ParamId, " + tabl + ".Value, " + tabl + ".Time, " + tabl + ".Nd FROM ";
            string sts = s + stabl + ".ParamId, " + stabl + ".StrValue" + ", " + stabl + ".TimeValue" + ", " + stabl + ".Time, " + stabl + ".Nd FROM ";
            string sti = "SELECT TimeEnd, TimeBegin, IntervalName FROM " + itabl + " ";
            var join = itabl + " INNER JOIN " + tabl + " ON " + itabl + ".IntervalId = " + tabl + ".IntervalId ";
            var joins = itabl + " INNER JOIN " + stabl + " ON " + itabl + ".IntervalId = " + stabl + ".IntervalId ";
            if (!isReport)
            {
                std += join;
                sts += joins;
            }
            else
            {
                if (DatabaseType == DatabaseType.Access)
                {
                    std += "ReportParams INNER JOIN (" + join + ") ON ReportParams.ParamId = " + tabl + ".ParamId ";
                    sts += "ReportParams INNER JOIN (" + joins + ") ON ReportParams.ParamId = " + stabl + ".ParamId ";    
                }
                else
                {
                    std += " " + join + " INNER JOIN ReportParams ON ReportParams.ParamId = " + tabl + ".ParamId ";
                    sts += " " + joins + " INNER JOIN ReportParams ON ReportParams.ParamId = " + stabl + ".ParamId ";
                }
            }

            s = "";
            if (t.Type.IsPeriodic() || t.Type == IntervalType.Moments)
                s = "(TimeBegin>=" + DateStr(t.Begin) + ") AND (TimeEnd<=" + DateStr(t.End) + ")";
            if (t.Type == IntervalType.AbsoluteDay)
                s = "(TimeEnd>=" + DateStr(t.Begin) + ") AND (TimeEnd<=" + DateStr(t.End) + ")";
            if (t.Type == IntervalType.Single)
                s = "(TimeBegin=" + DateStr(t.Begin) + ") AND (TimeEnd=" + DateStr(t.End) + ")";
            if (t.Type.IsNamed())
                s = "(IntervalName='" + t.Name + "')";

            if (!isReport)
            {
                s = "WHERE (ProjectId=" + iproject.Project.Id + ")" + (s == "" ? "" : (" AND " + s));
                sti += " ORDER BY TimeEnd, TimeBegin";
            }
            else
            {
                sti += (s == "" ? "" : ("WHERE " + s)) + " ORDER BY TimeEnd, TimeBegin";
                s = "WHERE (ReportParams.Is" + orderType.ToEnglish() + "=" + BoolStr(true) + ") AND (ReportParams.ReportId = " + iproject.Report.Id + ") " + (s == "" ? "" : ("AND " + s));
            }
            std += s + " ORDER BY " + itabl + ".TimeEnd, " + itabl + ".TimeBegin, " + tabl + ".ParamId, " + tabl + ".Time";
            sts += s + " ORDER BY " + itabl + ".TimeEnd, " + itabl + ".TimeBegin, " + stabl + ".ParamId, " + stabl + ".Time";

            using (var reci = Reader(sti)) //Интервалы
            {
                var recs = new IRecordRead[2];
                try
                {
                    int u = 1, v = 0;
                    if (iproject.HasValues.Contains(t.Type))
                    {
                        recs[0] = Reader(std); //Числовые значения
                        recs[0].Read();
                        u = 0;
                    }
                    if (iproject.HasStrValues.Contains(t.Type))
                    {
                        recs[1] = Reader(sts); //Строковые значения
                        recs[1].Read();
                        v = 1;
                    }
                    while (reci.Read())
                    {
                        DateTime beg = reci.GetTime("TimeBegin");
                        DateTime en = reci.GetTime("TimeEnd");
                        var interval = new ArchiveInterval(t.Type, beg, en, reci.GetString("IntervalName"));
                        for (int i = u; i <= v; i++) //u=0 - есть числовые значения, v=1 - есть строковые
                        {
                            var rec = recs[i];
                            while (!rec.EOF && rec.GetTime("TimeBegin") == beg && rec.GetTime("TimeEnd") == en)
                            {
                                int id = rec.GetInt("ParamId");
                                ArchiveParam par = null;
                                ArchiveReportParam reppar = null;
                                var dt = DataType.Value;
                                if (!isReport && iproject.ParamsId.ContainsKey(id))
                                {
                                    par = iproject.ParamsId[id];
                                    dt = par.DataType.AplySuperProcess(par.SuperProcess);
                                }
                                if (isReport && iproject.ReportParamsId.ContainsKey(id))
                                {
                                    reppar = iproject.ReportParamsId[id];
                                    dt = reppar.DataType.AplySuperProcess(reppar.SuperProcess);
                                }

                                if (par != null || reppar != null)
                                {
                                    var sv = new SingleValue(singleType);
                                    while (!rec.EOF && rec.GetInt("ParamId") == id && rec.GetTime("TimeBegin") == beg && rec.GetTime("TimeEnd") == en)
                                    {
                                        Moment mv;
                                        if (i == 0) mv = new Moment(dt, rec.GetDouble("Value"), rec.GetTime("Time"), rec.GetInt("Nd"));
                                        else mv = dt == DataType.Time
                                                      ? new Moment(rec.GetTime("Time"), rec.GetTime("TimeValue"), null, rec.GetInt("Nd"))
                                                      : new Moment(rec.GetTime("Time"), rec.GetString("StrValue"), null, rec.GetInt("Nd"));
                                        if (sv.Type == SingleType.Moment) sv.Moment = mv;
                                        else sv.Moments.Add(mv);
                                        rec.Read(); 
                                    }
                                    if (!isReport)
                                        par.Intervals.Add(interval, sv);
                                    else
                                    {
                                        var aqv = reppar.Queries[orderType];
                                        if (orderType.IsPeriodic() || orderType == IntervalType.AbsoluteDay || orderType == IntervalType.AbsoluteListDay || orderType == IntervalType.AbsoluteListHour || orderType == IntervalType.AbsoluteListBase || orderType == IntervalType.AbsoluteCombined)
                                        {
                                            aqv.Intervals.Add(interval);
                                            aqv.SingleValue.Moments.Add(sv.Moment);
                                        }
                                        else if (orderType == IntervalType.Moments)
                                        {
                                            aqv.Intervals.Add(interval);
                                            foreach (var m in sv.Moments)
                                                aqv.SingleValue.Moments.Add(m);
                                        }
                                        else
                                        {
                                            aqv.Interval = interval;
                                            aqv.SingleValue = sv;
                                        }
                                    }
                                }
                                else rec.Read();
                            }
                        }
                    }
                }
                finally
                {
                    if (recs[0] != null) recs[0].Dispose();
                    if (recs[1] != null) recs[1].Dispose();
                }
            }
        }

        private DateTime _begin;
        private DateTime _end;
        
        private void TryWriteProject()
        {
            if (_project.IntervalsForWrite.Count == 0) return;
            InnerArchiveProject iproject = _innerProjects[_project.Code];
            iproject.PeriodBegin = _begin;
            iproject.PeriodEnd = _end;
            iproject.IsSingle = false;
            foreach (var t in _project.IntervalsForWrite)
                if (t.Type.IsSingle()) iproject.IsSingle = true;

            using (Logger.Start(0, 25))
                WriteIntervals(_project);
            if (!Logger.Command.IsError)
                using (Logger.Start(25, 65))
                    WriteValues(_project);
            if (!Logger.Command.IsError)
                using (Logger.Start(65, 70))
                    UpdateRanges(_project);
            if (!Logger.Command.IsError)
                using (Logger.Start(70, 80))
                    DeleteOldIntervals(_project);
            CloseConnections();

            if (DatabaseType == DatabaseType.Access)
            {
                Logger.Start(ReserveCopy, 80, 90);
                Logger.Start(Compress, 90);
            }
        }

        //Запись данных по проекту project, записываются интервалы и значения, begin, end - период обработки
        public void WriteProject(ArchiveProject project, DateTime begin, DateTime end)
        {
            _project = project;
            _begin = begin;
            _end = end;
            if (!Logger.IsRepeat) ADanger(TryWriteProject, 2, 3000, "при записи данных в архив");
            else ADanger(TryWriteProject, 3, 20000, "при записи данных в архив");
        }

        //Добавление или обновление интервалов, удаление старых значений
        private void WriteIntervals(ArchiveProject project)
        {
            var n = project.IntervalsForWrite.Count;
            if (n == 0) return;
            try
            {
                foreach (var t in project.IntervalsForWrite)
                    if (t.Type == IntervalType.Absolute)
                        PrepareAbsolute(project, t);
                Logger.Procent = 30;
                
                foreach (var t in project.IntervalsForWrite)
                    if (t.Type != IntervalType.Empty)
                    {
                        Logger.AddEvent("Запись интервала. " + t.Type.ToRussian());
                        string s = "";
                        string sbeg = " (TimeBegin=" + DateStr(t.Begin) + ") ";
                        string sen = " (TimeEnd=" + DateStr(t.End) + ") ";
                        string sname = " (IntervalName='" + t.Name + "') ";

                        if (t.Type.IsPeriodic() || t.Type == IntervalType.Moments || t.Type == IntervalType.Single) s = sbeg + "AND" + sen;
                        if (t.Type.IsAbsolute()) s = sen;
                        if (t.Type.IsNamed()) s = sname;
                        s = "SELECT " + t.Type.ToIntervalsTable() + ".* FROM " + t.Type.ToIntervalsTable() + " WHERE (ProjectId=" + project.Id + ") AND (" + s + ")";
                        bool isold;
                        using (var rec = Writer(s)) //Список уже существующих интервалов
                        {
                            isold = rec.HasRows();
                            if (isold) rec.MoveFirst();
                            else rec.AddNew();
                            if (!isold || t.Type == IntervalType.NamedAdd || t.Type == IntervalType.NamedAddParams || t.Type == IntervalType.Single)
                            {
                                t.ToRecordset(rec, project.Id);
                                Reload(rec);
                                rec.MoveFirst();
                            }
                            t.Id = rec.GetInt("IntervalId");
                        }
                        Logger.Procent += 20.0/n;

                        if (isold || t.Type == IntervalType.Base)
                        {
                            Logger.AddEvent("Удаление прежних значений интервала. " + t.Type.ToRussian());
                            switch (t.Type)
                            {
                                case IntervalType.Single:
                                    ExecuteDelete("SingleValues", "FROM Params INNER JOIN SingleValues ON Params.ParamId = SingleValues.ParamId WHERE SingleValues.IntervalId=" + t.Id);
                                    ExecuteDelete("SingleStrValues", "FROM Params INNER JOIN SingleStrValues ON Params.ParamId = SingleStrValues.ParamId WHERE SingleStrValues.IntervalId=" + t.Id);
                                    break;
                                case IntervalType.Named:
                                    ExecuteDelete("NamedValues", "FROM NamedValues WHERE NamedValues.IntervalId=" + t.Id);
                                    ExecuteDelete("NamedStrValues", "FROM NamedStrValues WHERE NamedStrValues.IntervalId=" + t.Id);
                                    break;
                                case IntervalType.NamedAdd:
                                    ExecuteDelete("NamedValues", "FROM Params INNER JOIN NamedValues ON Params.ParamId = NamedValues.ParamId " +
                                        "WHERE (Params.Active=" + BoolStr(true) + ") AND (IntervalId=" + t.Id + ") AND (Time >= " + DateStr(t.Begin) + ") AND (Time < " + DateStr(t.End) + ")");
                                    ExecuteDelete("NamedStrValues", "FROM Params INNER JOIN NamedStrValues ON Params.ParamId = NamedStrValues.ParamId " +
                                        "WHERE (Params.Active=" + BoolStr(true) + ") AND (IntervalId=" + t.Id + ") AND (Time >= " + DateStr(t.Begin) + ") AND (Time < " + DateStr(t.End) + ")");
                                    break;
                                case IntervalType.NamedAddParams:
                                    ExecuteDelete("NamedValues", "FROM Params INNER JOIN NamedValues ON Params.ParamId = NamedValues.ParamId " +
                                        "WHERE (Params.Active=" + BoolStr(true) + ") AND (IntervalId=" + t.Id + ")");
                                    ExecuteDelete("NamedStrValues", "FROM Params INNER JOIN NamedStrValues ON Params.ParamId = NamedStrValues.ParamId " +
                                        "WHERE (Params.Active=" + BoolStr(true) + ") AND (IntervalId=" + t.Id + ")");
                                    break;
                                case IntervalType.Hour:
                                case IntervalType.Day:
                                case IntervalType.Absolute:
                                case IntervalType.AbsoluteDay:
                                case IntervalType.Moments:
                                    string vtab = t.Type.ToValuesTable();
                                    ExecuteDelete(vtab, "FROM Params INNER JOIN " + vtab + " ON Params.ParamId = " + vtab + ".ParamId " +
                                        " WHERE (Params.Active=" + BoolStr(true) + ") AND (" + vtab + ".IntervalId=" + t.Id + ")");
                                    string stab = t.Type.ToStrValuesTable();
                                    ExecuteDelete(stab, "FROM Params INNER JOIN " + stab + " ON Params.ParamId = " + stab + ".ParamId " +
                                        " WHERE (Params.Active=" + BoolStr(true) + ") AND (" + stab + ".IntervalId=" + t.Id + ")");
                                    break;
                                case IntervalType.Base:
                                    ExecuteDelete("BaseValues", "FROM ((Projects INNER JOIN Params ON Projects.ProjectId = Params.ProjectId) INNER JOIN BaseIntervals ON Projects.ProjectId = BaseIntervals.ProjectId) INNER JOIN BaseValues ON (Params.ParamId = BaseValues.ParamId) AND (BaseIntervals.IntervalId = BaseValues.IntervalId) " +
                                    "WHERE (Projects.Project='" + project.Code + "') AND (Params.Active=" + BoolStr(true) + ") AND " +
                                    "(BaseIntervals.TimeBegin < " + DateStr(t.End) + ") AND (BaseIntervals.TimeEnd > " + DateStr(t.Begin) + ")");
                                    ExecuteDelete("BaseStrValues", "FROM ((Projects INNER JOIN Params ON Projects.ProjectId = Params.ProjectId) INNER JOIN BaseIntervals ON Projects.ProjectId = BaseIntervals.ProjectId) INNER JOIN BaseStrValues ON (Params.ParamId = BaseStrValues.ParamId) AND (BaseIntervals.IntervalId = BaseStrValues.IntervalId) " +
                                    "WHERE (Projects.Project='" + project.Code + "') AND (Params.Active=" + BoolStr(true) + ") AND " +
                                    "(BaseIntervals.TimeBegin < " + DateStr(t.End) + ") AND (BaseIntervals.TimeEnd > " + DateStr(t.Begin) + ")");
                                    ExecuteDelete("BaseIntervals", "FROM ((Projects INNER JOIN Params ON Projects.ProjectId = Params.ProjectId) INNER JOIN BaseIntervals ON Projects.ProjectId = BaseIntervals.ProjectId) " +
                                    "WHERE (Projects.Project='" + project.Code + "') AND (Params.Active=" + BoolStr(true) + ") AND " +
                                    "(BaseIntervals.TimeBegin < " + DateStr(t.End) + ") AND (BaseIntervals.TimeEnd > " + DateStr(t.Begin) + ") AND ((BaseIntervals.TimeBegin <> " + DateStr(t.Begin) + ") OR (BaseIntervals.TimeEnd <> " + DateStr(t.End) + "))");
                                    break;
                            }
                        }
                        Logger.Procent += 50.0 / n;
                    }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка добавления интервала или удаления старых значений", ex);
            }
        }


        //Подготовка абсолютных значений
        private void PrepareAbsolute(ArchiveProject project, ArchiveInterval t)
        {
            Logger.AddEvent("Подготовка абсолютных таблиц. Поиск не активных параметров");
            ExecuteCommand("UPDATE AbsoluteIntervals SET SysField=null WHERE (ProjectId=" + project.Id + ")");
            if (DatabaseType == DatabaseType.Access)
            {
                ExecuteCommand("UPDATE Projects INNER JOIN (Params INNER JOIN (AbsoluteIntervals INNER JOIN AbsoluteValues ON AbsoluteIntervals.IntervalId = AbsoluteValues.IntervalId) ON Params.ParamId = AbsoluteValues.ParamId) ON (Projects.ProjectId = AbsoluteIntervals.ProjectId) AND (Projects.ProjectId = Params.ProjectId) " +
                               "SET AbsoluteIntervals.SysField = 'NotActive' WHERE (Params.Active=False) AND (Projects.ProjectId=" + project.Id + ");");
                ExecuteCommand("UPDATE Projects INNER JOIN (Params INNER JOIN (AbsoluteIntervals INNER JOIN AbsoluteStrValues ON AbsoluteIntervals.IntervalId = AbsoluteStrValues.IntervalId) ON Params.ParamId = AbsoluteStrValues.ParamId) ON (Projects.ProjectId = AbsoluteIntervals.ProjectId) AND (Projects.ProjectId = Params.ProjectId) " +
                               "SET AbsoluteIntervals.SysField = 'NotActive' WHERE (Params.Active=False) AND (Projects.ProjectId=" + project.Id + ");");
            }
            else
            {
                ExecuteCommand("UPDATE AbsoluteIntervals SET AbsoluteIntervals.SysField = 'NotActive' " +
                                "FROM AbsoluteValues INNER JOIN AbsoluteIntervals ON AbsoluteValues.IntervalId = AbsoluteIntervals.IntervalId  " +
                                "INNER JOIN Params ON AbsoluteValues.ParamId = Params.ParamId INNER JOIN Projects ON AbsoluteIntervals.ProjectId = Projects.ProjectId AND Params.ProjectId = Projects.ProjectId  " +
                                "WHERE (Params.Active=0) AND (Projects.ProjectId=" + project.Id + ")");
                ExecuteCommand("UPDATE AbsoluteIntervals SET AbsoluteIntervals.SysField = 'NotActive' " +
                                "FROM AbsoluteStrValues INNER JOIN AbsoluteIntervals ON AbsoluteStrValues.IntervalId = AbsoluteIntervals.IntervalId  " +
                                "INNER JOIN Params ON AbsoluteStrValues.ParamId = Params.ParamId INNER JOIN Projects ON AbsoluteIntervals.ProjectId = Projects.ProjectId AND Params.ProjectId = Projects.ProjectId  " +
                                "WHERE (Params.Active=0) AND (Projects.ProjectId=" + project.Id + ")");
            }

            Logger.AddEvent("Подготовка абсолютных таблиц. Удаление старых значений");
            ExecuteDelete("AbsoluteValues", "FROM AbsoluteIntervals INNER JOIN AbsoluteValues ON AbsoluteIntervals.IntervalId = AbsoluteValues.IntervalId " +
                            "WHERE (AbsoluteIntervals.ProjectId=" + project.Id + ") And (AbsoluteIntervals.SysField Is Not Null) And (TimeEnd < " + DateStr(t.End) + ")");
            ExecuteDelete("AbsoluteStrValues", "FROM AbsoluteIntervals INNER JOIN AbsoluteStrValues ON AbsoluteIntervals.IntervalId = AbsoluteStrValues.IntervalId " +
                            "WHERE (AbsoluteIntervals.ProjectId=" + project.Id + ") And (AbsoluteIntervals.SysField Is Not Null) And (TimeEnd < " + DateStr(t.End) + ")");
            ExecuteDelete("AbsoluteIntervals", "FROM AbsoluteIntervals WHERE (AbsoluteIntervals.ProjectId=" + project.Id + ") And (SysField Is Null) And (TimeEnd < " + DateStr(t.End) + ")");
            
            Logger.AddEvent("Подготовка абсолютных таблиц. Удаление старых значений, если новые введены вручную");
            if (DatabaseType == DatabaseType.Access)
            {
                ExecuteDelete("AbsoluteValues", "FROM (Projects INNER JOIN (Params INNER JOIN AbsoluteValues ON Params.ParamId = AbsoluteValues.ParamId) ON Projects.ProjectId = Params.ProjectId) " +
                                                "INNER JOIN AbsoluteEditValues ON (Params.Code = AbsoluteEditValues.Code) AND (Projects.Project = AbsoluteEditValues.Project) " +
                                                "WHERE Projects.ProjectId=" + project.Id + ";");
                ExecuteDelete("AbsoluteStrValues", "FROM (Projects INNER JOIN (Params INNER JOIN AbsoluteStrValues ON Params.ParamId = AbsoluteStrValues.ParamId) ON Projects.ProjectId = Params.ProjectId) " +
                                                "INNER JOIN AbsoluteEditValues ON (Params.Code = AbsoluteEditValues.Code) AND (Projects.Project = AbsoluteEditValues.Project) " +
                                                "WHERE Projects.ProjectId=" + project.Id + ";");
            }
            else
            {
                ExecuteDelete("AbsoluteValues", "FROM Params INNER JOIN Projects ON Params.ProjectId = Projects.ProjectId INNER JOIN AbsoluteValues ON Params.ParamId = AbsoluteValues.ParamId " +
                                                "INNER JOIN AbsoluteEditValues ON Projects.Project = AbsoluteEditValues.Project AND Params.Code = AbsoluteEditValues.Code " +
                                                "WHERE Projects.ProjectId=" + project.Id + ";");
                ExecuteDelete("AbsoluteStrValues", "FROM Params INNER JOIN Projects ON Params.ProjectId = Projects.ProjectId INNER JOIN AbsoluteStrValues ON Params.ParamId = AbsoluteStrValues.ParamId " +
                                                "INNER JOIN AbsoluteEditValues ON Projects.Project = AbsoluteEditValues.Project AND Params.Code = AbsoluteEditValues.Code " +
                                                "WHERE Projects.ProjectId=" + project.Id + ";");
            }
            ExecuteDelete("AbsoluteEditValues", "FROM AbsoluteEditValues WHERE Project='" + project.Code + "'");

            using (var recp = Writer("SELECT * FROM Projects WHERE ProjectId=" + project.Id))
                using (var rece = Writer("SELECT * FROM HandInputProjects WHERE Project='" + project.Code + "'"))
                    if (recp.HasRows() && rece.HasRows())
                        recp.Put("AbsoluteEditTime", rece.GetTimeNull("AbsoluteEditTime"));
        }

        //Добавление значений в архив
        private void WriteValues(ArchiveProject project)
        {
            if (project.IntervalsForWrite.Count == 0 || project.Params.Count == 0) return;
            try
            {
                double d = 100.0 / project.IntervalsForWrite.Count;
                foreach (var t in project.IntervalsForWrite)
                {
                    Logger.AddEvent("Открытие рекордсета значений. " + t.Type.ToRussian() ,  "Начало: " + t.Begin + ", Конец: " + t.End + ", Имя:" + (t.Name ?? ""));
                    using (var rec = Adder(t.Type.ToValuesTable()))
                        using (var recs = Adder(t.Type.ToStrValuesTable()))
                        {
                            Logger.Procent += d * 0.2;
                            Logger.AddEvent("Запись значений интервала. " + t.Type.ToRussian(),  "Начало: " + t.Begin + ", Конец: " + t.End + ", Имя:" + (t.Name ?? ""));
                            int i = 0, n = project.Params.Count, nval = 0;
                            foreach (var p in project.Params.Values)
                            {
                                if (p.Intervals.ContainsKey(t) && p.Intervals[t] != null)
                                {
                                    SingleValue sv = p.Intervals[t];
                                    if (sv.Type == SingleType.Moment)
                                    {
                                        nval++;
                                        MomToRecordset(sv.Moment, rec, recs, p.Id, t.Id);
                                    }
                                    else
                                    {
                                        nval += sv.Moments.Count;
                                        foreach (var m in sv.Moments)
                                            MomToRecordset(m, rec, recs, p.Id, t.Id);    
                                    }
                                }
                                if ((++i) % 50 == 0) Logger.Procent += d * 0.8 * 50.0 / n;
                            }  
                            rec.Update();
                            recs.Update();
                            Logger.AddEvent("Значения записаны. " + t.Type.ToRussian(), project.Params.Count + " параметров, " + nval + " значений");
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при записи значений", ex);
            }
        }

        //Запись в таблицу архива rec или recs(если строки) момента mv, c id параметра и интервала paramId и intervalId
        private static void MomToRecordset(Moment mv, IRecordAdd rec, IRecordAdd recs, int paramId, int intervalId)
        {
            if (mv == null) return;
            try
            {
                if (mv.DataType == DataType.String || mv.DataType == DataType.Time)
                {
                    recs.AddNew();
                    recs.Put("ParamId", paramId);
                    recs.Put("IntervalId", intervalId);
                    recs.Put("Time", mv.Time);
                    recs.Put("Nd", mv.Nd);
                    if (mv.DataType == DataType.Time) recs.Put("TimeValue", mv.Date);
                    else recs.Put("StrValue", mv.String);
                }
                else
                {
                    rec.AddNew();
                    rec.Put("ParamId", paramId);
                    rec.Put("IntervalId", intervalId);
                    rec.Put("Time", mv.Time);
                    rec.Put("Nd", mv.Nd);
                    rec.Put("Value", mv.Real);
                }
            }
            catch {}
        }

        //Обновление диапазонов
        private void UpdateRanges(ArchiveProject project)
        {
            Logger.AddEvent("Обновление диапазонов");
            try
            {
                using (var rec = Writer("SELECT * FROM Ranges WHERE ProjectId=" + project.Id))
                    foreach (var interval in project.IntervalsForWrite)
                    {
                        IntervalType itype = interval.Type;
                        if (!rec.FindFirst("IntervalType", itype.ToRussian()))
                        {
                            rec.AddNew();
                            rec.Put("ProjectId", project.Id);
                            rec.Put("IntervalType", itype.ToRussian());
                            rec.Put("TimeBegin", interval.Begin);
                            rec.Put("TimeEnd", interval.End);
                        }
                        else
                        {
                            if (interval.Begin < rec.GetTime("TimeBegin") || interval.Type == IntervalType.Named)
                                rec.Put("TimeBegin", interval.Begin);
                            if (interval.End > rec.GetTime("TimeEnd") || interval.Type == IntervalType.Named)
                                rec.Put("TimeEnd", interval.End);
                        }
                    }
            }
            catch(Exception ex)
            {
                Logger.AddError("Ошибка удаления диапазонов", ex);
            }
        }

        //Обновление диапазонов интервалов разных типов и удаление интервалов по ограничениям
        private void DeleteOldIntervals(ArchiveProject project)
        {
            try
            {
                InnerArchiveProject iproject = _innerProjects[project.Code];
                if (!iproject.IsSingle && (iproject.PeriodEnd.Minute != 0 || iproject.PeriodEnd.Hour % 2 != 0)) return;
                Logger.AddEvent("Получение диапазонов для удаления старых интервалов");
                DateTime time = (iproject.PeriodEnd < DateTime.Now ? iproject.PeriodEnd : DateTime.Now).Date;
                using (var rec = Writer("SELECT * FROM Ranges WHERE ProjectId=" + project.Id))
                    while (rec.Read())
                    {
                        DateTime? beg = rec.GetTimeNull("TimeBegin");
                        if (beg != null)
                        {
                            var d = Different.MinDate;
                            var t = rec.GetString("IntervalType").ToIntervalType();
                            if (iproject.IsSingle)
                            {
                                if (t == IntervalType.Single)
                                    d = GetDelTime(time, _singleTime);
                            }
                            else
                            {
                                switch (t)
                                {
                                    case IntervalType.Base:
                                        d = GetDelTime(time, _baseTime);
                                        break;
                                    case IntervalType.Hour:
                                        d = GetDelTime(time, _hourTime);
                                        break;
                                    case IntervalType.Day:
                                        d = GetDelTime(time, _dayTime);
                                        break;
                                    case IntervalType.AbsoluteDay:
                                        d = GetDelTime(time, _absoluteDayTime);
                                        break;
                                    case IntervalType.Moments:
                                        d = GetDelTime(time, _momentsTime);
                                        break;
                                }
                            }
                            if (beg < d && (iproject.IsSingle || iproject.PeriodEnd.Hour == t.HourNumber()))
                            {
                                rec.Put("TimeBegin", d);
                                string itab = t.ToIntervalsTable();
                                if (DatabaseType == DatabaseType.Access)
                                {
                                    Logger.AddEvent("Удаление старых числовых значений. " + t.ToRussian(), "До " + d);
                                    string vtab = t.ToValuesTable(), stab = t.ToStrValuesTable();
                                    ExecuteDelete(t.ToValuesTable(), "FROM " + itab + " INNER JOIN " + vtab + " ON " + itab + ".IntervalId = " + vtab + ".IntervalId " +
                                                                          "WHERE (" + itab + ".ProjectId = " + project.Id + ") AND (" + itab + ".TimeEnd <= " + DateStr(d) + ")");
                                    Logger.AddEvent("Удаление старых строковых значений. " + t.ToRussian(), "До " + d);
                                    ExecuteDelete(t.ToStrValuesTable(), "FROM " + itab + " INNER JOIN " + stab + " ON " + itab + ".IntervalId = " + stab + ".IntervalId " +
                                                                          "WHERE (" + itab + ".ProjectId = " + project.Id + ") AND (" + itab + ".TimeEnd <= " + DateStr(d) + ")");    
                                }
                                Logger.AddEvent("Удаление старых интервалов. " + t.ToRussian(), "До " + d);
                                ExecuteDelete(itab, "FROM " + itab + " WHERE (ProjectId = " + project.Id + ") AND (TimeEnd <= " + DateStr(d) + ")");
                                Logger.AddEvent("Старые интервалы удалены. " + t.ToRussian(), "До " + d);
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при удалении интервалов", ex);
            }
        }

        private DateTime GetDelTime(DateTime time, int num)
        {
            if (num < 0) return Different.MinDate;
            return time.AddDays(-num).Date;
        }

        //Резервное копирование базы архива
        private void ReserveCopy()
        {
            try
            {
                if (!_reserveDir.IsEmpty())
                {
                    using (var sys = new SysTabl(_databaseFile, false))
                    {
                        DateTime date;
                        if (!DateTime.TryParse(sys.Value("ReserveTime"), out date) || (DateTime.Now.Subtract(date).TotalDays > _reserveFrequency))
                        {
                            Logger.AddEvent("Резервное копирование архива");
                            var d = new DirectoryInfo(_reserveDir);
                            if (!d.Exists) d.Create();
                            var f = new FileInfo(_databaseFile);
                            string s = _addDate ? DateTime.Now.ToString("yyyyMMddHH") : "";
                            f.CopyTo(d.FullName + @"\" + f.Name.Substring(0, f.Name.Length - 6) + s + ".accdb", true);
                            sys.PutValue("ReserveTime", DateTime.Now.ToString());
                        }    
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при резервном копировании архива", ex);
            }
        }

        //Сжатие базы данных
        private void Compress()
        {
            try
            {
                using (var sys = new SysTabl(_databaseFile, false))
                {
                    int freq = int.Parse(sys.Value("CompressFrequency"));
                    int fileSize = int.Parse(sys.Value("CompressSize"));
                    DateTime date;
                    if ((!DateTime.TryParse(sys.Value("CompressTime"), out date) || DateTime.Now.Subtract(date).TotalHours > freq) && new FileInfo(_databaseFile).Length > fileSize)
                    {
                        Logger.AddEvent("Сжатие базы данных");
                        sys.PutValue("CompressTime", DateTime.Now.ToString());
                        var tmpDir = Different.GetInfoTaskDir() + @"Tmp";
                        DaoDb.Compress(_databaseFile, fileSize, tmpDir, 1000);
                    }    
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при сжатии базы", ex);
            }
        }
        
        private DicS<HandInputParam> _dicpars;
        private bool _flag;

        private void TryReadAbsoluteEdit()
        {
            _dicpars = new DicS<HandInputParam>();
            using (var rec = Reader("SELECT * FROM AbsoluteEditValues WHERE Project='" + _projectName + "'"))
                while (rec.Read())
                {
                    var hip = new HandInputParam(rec) { Value = rec.GetString("Value"), Time = rec.GetTime("Time") };
                    _dicpars.Add(hip.Code, hip);
                }
            if (!_flag)
            {
                string stFrom = DatabaseType == DatabaseType.Access
                                    ? "((Projects INNER JOIN Params ON Projects.ProjectId = Params.ProjectId) INNER JOIN AbsoluteIntervals ON Projects.ProjectId = AbsoluteIntervals.ProjectId) INNER JOIN AbsoluteValues ON (Params.ParamId = AbsoluteValues.ParamId) AND (AbsoluteIntervals.IntervalId = AbsoluteValues.IntervalId) "
                                    : "AbsoluteValues INNER JOIN AbsoluteIntervals ON AbsoluteValues.IntervalId = AbsoluteIntervals.IntervalId INNER JOIN " +
                                        "Params ON AbsoluteValues.ParamId = Params.ParamId INNER JOIN Projects ON AbsoluteIntervals.ProjectId = Projects.ProjectId AND Params.ProjectId = Projects.ProjectId ";
                using (var rec = Reader("SELECT Projects.Project, Params.Code, AbsoluteValues.Time, AbsoluteValues.Value FROM " + stFrom +
                            "WHERE (Projects.Project='" + _projectName + "') ORDER BY AbsoluteIntervals.TimeEnd DESC "))
                    while (rec.Read())
                    {
                        var code = rec.GetString("Code");
                        string ov = rec.GetString("Value");
                        DateTime ot = rec.GetTime("Time");
                        if (!_dicpars.ContainsKey(code))
                            _dicpars.Add(code, new HandInputParam(rec) {OldValue = ov, OldTime = ot});
                        else
                        {
                            _dicpars[code].OldValue = ov;
                            _dicpars[code].OldTime = ot;
                        }
                    }
                stFrom = DatabaseType == DatabaseType.Access
                                ? "((Projects INNER JOIN Params ON Projects.ProjectId = Params.ProjectId) INNER JOIN AbsoluteIntervals ON Projects.ProjectId = AbsoluteIntervals.ProjectId) INNER JOIN AbsoluteStrValues ON (Params.ParamId = AbsoluteStrValues.ParamId) AND (AbsoluteIntervals.IntervalId = AbsoluteStrValues.IntervalId) "
                                : "AbsoluteStrValues INNER JOIN AbsoluteIntervals ON AbsoluteStrValues.IntervalId = AbsoluteIntervals.IntervalId INNER JOIN " +
                                "Params ON AbsoluteStrValues.ParamId = Params.ParamId INNER JOIN Projects ON AbsoluteIntervals.ProjectId = Projects.ProjectId AND Params.ProjectId = Projects.ProjectId ";
                using (var rec = Reader("SELECT Projects.Project, Params.Code, AbsoluteStrValues.Time, AbsoluteStrValues.StrValue, AbsoluteStrValues.TimeValue FROM " + stFrom +
                        "WHERE (Projects.Project='" + _projectName + "') ORDER BY AbsoluteIntervals.TimeEnd DESC "))
                    while (rec.Read())
                    {
                        var code = rec.GetString("Code");
                        string ov = rec.GetString("StrValue") ?? rec.GetString("TimeValue");
                        DateTime ot = rec.GetTime("Time");
                        if (!_dicpars.ContainsKey(code))
                            _dicpars.Add(code, new HandInputParam(rec) {OldValue = ov, OldTime = ot});
                        else
                        {
                            _dicpars[code].OldValue = ov;
                            _dicpars[code].OldTime = ot;
                        }
                    }
            }
        }

        //Чтение ручного ввода по проекту project, возвращает словарь параметров со значениями, ключи - коды в нижнем регистре
        //Если onlyEdit, то читаем только введенные значений, иначе еще и посчитанные
        public DicS<HandInputParam> ReadAbsoluteEdit(string project, bool onlyEdit)
        {
            _projectName = project;
            _flag = onlyEdit;
            ADanger(TryReadAbsoluteEdit, 3, 500, "чтения отредактированных абсолютных значений");
            return _dicpars;
        }

        private List<HandInputParam> _listpars;
        private string _hitype;

        //Запись абсолютных значений по списку pars можно из разных проектов
        public void WriteAbsoluteEdit(List<HandInputParam> pars)
        {
            _listpars = pars;
            _hitype = "AbsoluteEdit";
            ADanger(WriteInput, 2, 1000, "записи значений введенных вручную");
        }

        //Запись ручного ввода или абсолютных значений
        private void WriteInput()
        {
            var time = DateTime.Now;
            using (var rec = Writer("SELECT * FROM HandInputProjects"))
            {
                var projects = new SetS();
                foreach (var par in _listpars)
                    projects.Add(par.Project);
                foreach (var pr in projects.Values)
                {
                    if (!rec.FindFirst("Project", pr)) rec.AddNew();
                    rec.Put("Project", pr);
                    rec.Put(_hitype + "Time", time);
                    ExecuteDelete(_hitype + "Values", "FROM " + _hitype + "Values WHERE Project='" + pr + "'");
                }    
            }
            using (var rea = Adder(_hitype + "Values"))
            {
                foreach (var par in _listpars)
                    par.ToRecordset(rea, _hitype == "AbsoluteEdit");
                rea.Update();
            }
        }

        private void TryIsAbsoluteEdited()
        {
            _flag = false;
            using (var rec = Reader("SELECT * FROM HandInputProjects WHERE Project='" + _projectName + "'"))
                using (var recp = Reader("SELECT * FROM Projects WHERE Project='" + _projectName + "'"))
                    if (rec.HasRows() && rec.GetTimeNull("AbsoluteEditTime") != null)
                        _flag = !recp.HasRows() || recp.GetTimeNull("AbsoluteEditTime") == null || recp.GetTime("AbsoluteEditTime") != rec.GetTime("AbsoluteEditTime");
        }

        //True, если был произведен ручной ввод абсолютных значений
        public bool IsAbsoluteEdited(string project)
        {
            _projectName = project;
            ADanger(TryIsAbsoluteEdited, 2, 1000, "при проверке абсолютного ввода");
            return _flag;
        }

        //Получение списка проектов
        public DicS<ArchiveProject> ReadProjects(ReportType type = ReportType.Error)
        {
            var res = new DicS<ArchiveProject>();
            using (var rec = Reader("SELECT * FROM Projects" + (type == ReportType.Error ? "" : " WHERE ProjectType='" + type.ToRussian() + "'")))
                while (rec.Read())
                {
                    var pr = new ArchiveProject(rec);
                    res.Add(pr.Code, pr);
                }    
            CloseConnections();
            return res;
        }

        //Получение списка отчетов
        public DicS<ArchiveReport> ReadReports(ReportType type = ReportType.Error)
        {
            var res = new DicS<ArchiveReport>();
            using (var rec = Reader("SELECT * FROM Reports" + (type == ReportType.Error ? "" : " WHERE ReportType='" + type.ToRussian() + "'")))
                while (rec.Read())
                {
                    var rep = new ArchiveReport(rec);
                    res.Add(rep.Code, rep);
                }
            CloseConnections();
            return res;
        }

        //Получение списка архивных параметров заданного проекта
        public List<ArchiveParam> ReadParams(string project, ReportType projectType)
        {
            var res = new List<ArchiveParam>();
            using (var recp = Reader("SELECT * FROM Projects WHERE (Project='" + project + "') AND (ProjectType ='" + projectType.ToRussian() + "')")) 
                if (recp.Read())
                {
                    int pid = recp.GetInt("ProjectId");
                    using (var rec = Reader("SELECT * FROM Params WHERE ProjectId=" + pid))
                        while (rec.Read()) res.Add(new ArchiveParam(rec));
                }
            CloseConnections();
            return res;
        }

        private ReportType _reportType;
        private List<ArchiveInterval> _listIntervals;

        private void TryReadIntervals()
        {
            _listIntervals = new List<ArchiveInterval>();
            using (var rec = Reader("SELECT SingleIntervals.TimeBegin, SingleIntervals.TimeEnd, SingleIntervals.IntervalName, SingleIntervals.IntervalId, SingleIntervals.TimeChange FROM Projects INNER JOIN SingleIntervals ON Projects.ProjectId = SingleIntervals.ProjectId " +
                                    " WHERE (Project='" + _projectName + "') AND (ProjectType ='" + _reportType.ToRussian() + "')"))
                while (rec.Read()) _listIntervals.Add(new ArchiveInterval(rec, IntervalType.Single));
        }

        //Получение списка разовых интервалов по указанному проекту
        public List<ArchiveInterval> ReadIntervals(string project, ReportType projectType)
        {
            _projectName = project;
            _reportType = projectType;
            ADanger(TryReadIntervals, 2, 2000, "при чтении списка интервалов");
            return _listIntervals;
        }

        private void TryDeleteIntervals()
        {
            int pid = 0;
            using (var rec = Writer("SELECT * FROM Projects WHERE (Project='" + _projectName + "') AND (ProjectType ='" + _reportType.ToRussian() + "')"))
                if (rec.HasRows())
                    pid = rec.GetInt("ProjectId");
            if (pid != 0)
                foreach (var interval in _listIntervals)
                {
                    ExecuteDelete("SingleValues", "FROM SingleIntervals INNER JOIN SingleValues ON SingleIntervals.IntervalId = SingleValues.IntervalId WHERE " +
                                  "(SingleIntervals.ProjectId=" + pid + ") AND " + "(SingleIntervals.TimeBegin = " + DateStr(interval.Begin) + ") AND (SingleIntervals.TimeEnd = " + DateStr(interval.End) + ") AND (SingleIntervals.IntervalName = '" + interval.Name + "')");
                    ExecuteDelete("SingleStrValues", "FROM SingleIntervals INNER JOIN SingleStrValues ON SingleIntervals.IntervalId = SingleStrValues.IntervalId WHERE " +
                                  "(SingleIntervals.ProjectId=" + pid + ") AND " + "(SingleIntervals.TimeBegin = " + DateStr(interval.Begin) + ") AND (SingleIntervals.TimeEnd = " + DateStr(interval.End) + ") AND (SingleIntervals.IntervalName = '" + interval.Name + "')");
                    ExecuteDelete("SingleIntervals", "FROM SingleIntervals WHERE (ProjectId=" + pid + ") AND " +
                                                "(TimeBegin = " + DateStr(interval.Begin) + ") AND (TimeEnd = " + DateStr(interval.End) + ") AND (IntervalName = '" + interval.Name + "')");}
        }

        //Удаление списка указанных интервалов по проекту, project - имя проекта, projectType - тип проекта
        public void DeleteIntervals(string project, ReportType projectType, List<ArchiveInterval> intervals)
        {
            _projectName = project;
            _reportType = projectType;
            _listIntervals = intervals;
            ADanger(TryDeleteIntervals, 2, 3000, "при удалении интервалов");
        }

        private Dictionary<IntervalType, TimeInterval> _dicIntervlas;

        private void TryReadRanges()
        {
            _dicIntervlas = new Dictionary<IntervalType, TimeInterval>();
            using (var rec = Reader("SELECT Ranges.* FROM Projects INNER JOIN Ranges ON Projects.ProjectId=Ranges.ProjectId " +
                                    "WHERE (Projects.Project='" + _projectName + "') AND (Projects.ProjectType ='" + _reportType.ToRussian() + "')"))
                while (rec.Read())
                    _dicIntervlas.Add(rec.GetString("IntervalType").ToIntervalType(), new TimeInterval(rec));
        }

        //Получение списка диапазонов для разных типов интервалов по проекту
        public Dictionary<IntervalType, TimeInterval> ReadRanges(string project, ReportType projectType)
        {
            _projectName = project;
            _reportType = projectType;
            ADanger(TryReadRanges, 2, 1000, "получения списка диапазонов");
            return _dicIntervlas;
        }

        //Полная очистка архива
        public void ClearArchive()
        {
            if (DatabaseType == DatabaseType.SqlServer)
            {
                ExecuteCommand("TRUNCATE TABLE SingleValues");
                ExecuteCommand("TRUNCATE TABLE SingleStrValues");
                ExecuteCommand("TRUNCATE TABLE NamedValues");
                ExecuteCommand("TRUNCATE TABLE NamedStrValues");
                ExecuteCommand("TRUNCATE TABLE BaseValues");
                ExecuteCommand("TRUNCATE TABLE BaseStrValues");
                ExecuteCommand("TRUNCATE TABLE HourValues");
                ExecuteCommand("TRUNCATE TABLE HourStrValues");
                ExecuteCommand("TRUNCATE TABLE DayValues");
                ExecuteCommand("TRUNCATE TABLE DayStrValues");
                ExecuteCommand("TRUNCATE TABLE AbsoluteValues");
                ExecuteCommand("TRUNCATE TABLE AbsoluteStrValues");
                ExecuteCommand("TRUNCATE TABLE AbsoluteDayValues");
                ExecuteCommand("TRUNCATE TABLE AbsoluteDayStrValues");
                ExecuteCommand("TRUNCATE TABLE MomentsValues");
                ExecuteCommand("TRUNCATE TABLE MomentsStrValues");
            }
            else
            {
                ExecuteDelete("SingleValues");
                ExecuteDelete("SingleStrValues");
                ExecuteDelete("NamedValues");
                ExecuteDelete("NamedStrValues");
                ExecuteDelete("BaseValues");
                ExecuteDelete("BaseStrValues");
                ExecuteDelete("HourValues");
                ExecuteDelete("HourStrValues");
                ExecuteDelete("DayValues");
                ExecuteDelete("DayStrValues");
                ExecuteDelete("AbsoluteValues");
                ExecuteDelete("AbsoluteStrValues");
                ExecuteDelete("AbsoluteDayValues");
                ExecuteDelete("AbsoluteDayStrValues");
                ExecuteDelete("MomentsValues");
                ExecuteDelete("MomentsStrValues");
            }
            ExecuteDelete("SingleIntervals");
            ExecuteDelete("NamedIntervals");
            ExecuteDelete("BaseIntervals");
            ExecuteDelete("HourIntervals");
            ExecuteDelete("DayIntervals");
            ExecuteDelete("AbsoluteIntervals");
            ExecuteDelete("AbsoluteDayIntervals");
            ExecuteDelete("Ranges");
            ExecuteDelete("Params");
            ExecuteDelete("Projects");
            ExecuteDelete("ReportParams");
            ExecuteDelete("Reports");
            ExecuteDelete("HandInputValues");
            ExecuteDelete("AbsoluteEditValues");
            ExecuteDelete("HandInputProjects");
            CloseConnections();
        }
    }
}