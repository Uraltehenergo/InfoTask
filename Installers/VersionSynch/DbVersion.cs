using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BaseLibrary;
using System.Windows.Forms;
using Microsoft.Win32;

namespace VersionSynch
{
    public class DbVersion
    {
        private string _templatePath;
        private string _fileTypeStr;
        private Version _dbVersion;
        private DaoDb _db;

        //Сравниваем версии базы данных и текущего ПО
        //Возвращает: -1 - ошибка, 0 - версии одинаковы, 1 - нужно обновлять, страые версии ПО совместимы, 2 - старые версии ПО не совместимы
        private int VersionStatus(string dbPath, DbMode dbMode)
        {
            try
            {
                PrepareDelegates(dbMode);
                if (_delegates.Count == 0) return 0;
                using (var sys = new SysTabl(dbPath))
                    _dbVersion = new Version(sys.SubValue("AppOptions", "AppVersion"), sys.SubValue("AppOptions", "AppVersionDate"));

                if (_delegates.Last().Version <= _dbVersion) return 0;
                return _delegates.Exists(x => x.Version > _dbVersion && !x.IsBack) ? 2 : 1;
            }
            catch { return -1;}
        }

        //Определяет _fileTypeStr и _templatePath
        private void DefineConstants(DbMode dbMode)
        {
            string itd = Different.GetInfoTaskDir();
            switch (dbMode)
            {
                case DbMode.Project:
                    _fileTypeStr = "Проект";
                    _templatePath = itd + @"General\ProjectTemplate.accdb";
                    break;
                case DbMode.Imit:
                    _fileTypeStr = "Файл имитационных данных";
                    _templatePath = itd + @"General\ImitDataTemplate.accdb";
                    break;
                case DbMode.Archive:
                case DbMode.ArchiveSQL:
                    _fileTypeStr = "Архив";
                    _templatePath = itd + @"Providers\Archives\CalcArchiveTemplate.accdb";
                    break;
                case DbMode.ControllerData:
                    _fileTypeStr = "Файл данных контроллера";
                    _templatePath = itd + @"Tmp\ControllerDataTemplate.accdb";
                    break;
                case DbMode.ReporterData:
                    _fileTypeStr = "Файл данных построителя отчетов";
                    _templatePath = itd + @"Tmp\ReporterDataTemplate.accdb";
                    break;
                case DbMode.AnalyzerAppData:
                    _fileTypeStr = "Файл данных анализатора";
                    _templatePath = itd + @"Tmp\AppDataTemplate.accdb";
                    break;
                case DbMode.ConstructorAppData:
                    _fileTypeStr = "Файл данных конструктора расчетов";
                    _templatePath = itd + @"Tmp\AppDataTemplate.accdb";
                        break;
            }
        }

        //Обновляет указаный файл указанного типа то текущей версии, silentMode - не выводить сообщение о несовместимости
        private string UpdateVersion(DbMode dbmode, string dbPath, bool silentMode)
        {
            try
            {
                if (dbPath.IsEmpty()) return "";
                var dbFile = new FileInfo(dbPath);
                if (!dbFile.Exists) return "";
                int status = VersionStatus(dbPath, dbmode);
                DefineConstants(dbmode);
                string mess = _fileTypeStr + " " + dbPath + " имеет устаревшую версию. ";
                if (status <= 0) return "";
                if (status == 2 && !silentMode && !Different.MessageQuestion("В случае обновления данный файл будет некорректно работать на более старых версиях системы InfoTask\nОбновить версию файла автоматически?"))
                    return "";
                var tmpPath = dbFile.Directory + @"\tmp__" + dbFile.Name;
                try { dbFile.CopyTo(tmpPath, true); }
                catch
                {
                    Different.MessageError(mess += "\nНе удается скопировать файл в " + tmpPath);
                    return mess;
                }
                using (_db = new DaoDb(tmpPath))
                    foreach (var d in _delegates)
                        if (d.Version > _dbVersion)
                        {
                            try
                            {
                                d.Action();
                                _dbVersion = d.Version;
                            }
                            catch (Exception ex)
                            {
                                return ex.MessageError(mess + "\nОшибка обновления файла"); 
                            }
                        }
                using (var sys = new SysTabl(tmpPath))
                {
                    sys.PutSubValue("AppOptions", "AppVersionDate", _dbVersion.Date);
                    sys.PutSubValue("AppOptions", "AppVersion", _dbVersion.ToString());
                }

                var tmpFile = new FileInfo(tmpPath);
                try
                {
                    dbFile.Delete();
                    Thread.Sleep(300);
                    tmpFile.CopyTo(dbPath, true);
                }
                catch
                {
                    Different.MessageError(mess += "При обновлении произошла ошибка, файл занят. Файл не был обновлен. Обновленная версия содержится в файле " + @"\tmp__" + dbFile.Name);
                    return mess;
                }
                try
                {
                    Thread.Sleep(400);
                    tmpFile.Delete();
                }
                catch (Exception ex)
                {
                    ex.MessageError("");
                }
                return "";
            }
            finally { GC.Collect(); }
        }

        //Вызовы VersionStatus и UpdateVersion
        #region
        //Сравниваем версии базы данных и ПО для файлов разных типов
        public int ProjectVersionStatus(string dbPath)
        {
            if (!IsProject(dbPath)) return -1;
            return VersionStatus(dbPath, DbMode.Project);
        }
        public int ArchiveVersionStatus(string dbPath)
        {
            if (!IsArchive(dbPath)) return -1;
            return VersionStatus(dbPath, DbMode.Archive);
        }
        public int TestProviderVersionStatus(string dbPath)
        {
            if (!IsImitFile(dbPath)) return -1;
            return VersionStatus(dbPath, DbMode.Imit);
        }
        private int AnalizerAppDataVersionStatus(string dbPath)
        {
            return VersionStatus(dbPath, DbMode.AnalyzerAppData);
        }
        private int ConstructorAppDataVersionStatus(string dbPath)
        {
            return VersionStatus(dbPath, DbMode.ConstructorAppData);
        }
        private int ControllerDataVersionStatus(string dbPath)
        {
            return VersionStatus(dbPath, DbMode.ControllerData);
        }
        private int ReporterDataVersionStatus(string dbPath)
        {
            return VersionStatus(dbPath, DbMode.ReporterData);
        }

        //Обновляет архив
        public string UpdateArchiveVersion(string dbPath, bool silentMode)
        {
            if (!IsArchive(dbPath)) return "Файл " + dbPath + " не является файлом архива";
            return UpdateVersion(DbMode.Archive, dbPath, silentMode);
        }
        //Обновляет архив SQL
        public string UpdateArchiveSQLVersion(string serverName, string dbName, bool windowsIdentification, string login, string password, bool silentMode)
        {
            throw new NotImplementedException();
        }
        //Обновляет файл имитатора
        public string UpdateImitVersion(string dbPath, bool silentMode)
        {
            if (!IsImitFile(dbPath)) return "Файл " + dbPath + " не является файлом имитационных значений";
            return UpdateVersion(DbMode.Imit, dbPath, silentMode);
        }
        //Обновляет файл AnalyzerAppData
        public string UpdateAnalyzerAppDataVersion(string dbPath, bool silentMode)
        {
            return UpdateVersion(DbMode.AnalyzerAppData, dbPath, silentMode);
        }
        //Обновляет файл ConstructorAppData
        public string UpdateConstructorAppDataVersion(string dbPath, bool silentMode)
        {
            return UpdateVersion(DbMode.ConstructorAppData, dbPath, silentMode);
        }
        //Обновляет файл ControllerData
        public string UpdateControllerDataVersion(string dbPath, bool silentMode)
        {
            return UpdateVersion(DbMode.ControllerData, dbPath, silentMode);
        }
        //Обновляет файл ReporterData до текущей версии
        public string UpdateReporterDataVersion(string dbPath, bool silentMode)
        {
            return UpdateVersion(DbMode.ReporterData, dbPath, silentMode);
        }
        //Обновляет проект до версии ProjectTemplate
        public string UpdateProjectVersion(string dbPath, bool silentMode)
        {
            if (!IsProject(dbPath)) return "Файл " + dbPath + " не является файлом проекта";
            return UpdateVersion(DbMode.Project, dbPath, silentMode);
        }

        //Проверки на то, что файл является файлом одного из типов
        public static bool IsProject(string dbPath)
        {
            return DaoDb.Check(dbPath, new[] {"Objects", "Signals", "CalcParams", "CalcSubParams", "CalcParamsArchive", "Tasks", "SignalsInUse", "GraficsList"});
        }
        public static bool IsClone(string dbPath)
        {
            return DaoDb.Check(dbPath, new[] { "Objects", "Signals", "MomentsValues" });
        }
        public static bool IsImitFile(string dbPath)
        {
            return DaoDb.Check(dbPath, new[] { "SignalsBehavior", "SignalsValues" });
        }
        public static bool IsArchive(string dbPath)
        {
            return DaoDb.Check(dbPath, new[] {"Projects", "Reports", "Params", "ReportParams", "Ranges", "SingleIntervals", "SingleValues", "NamedIntervals", "NamedValues", "MomentsValues", "BaseIntervals", "BaseValues", "HourIntervals", "HourValues", "DayIntervals", "DayValues", "AbsoluteValues"});
        }
        #endregion

        //Список обновлений
        #region
        private List<UpdateDelegate> _delegates;

        //Добавление одного действия в список
        private void AddAction(Action action, string version, string date, bool isBack = true)
        {
            _delegates.Add(new UpdateDelegate(action, version, date, isBack));
        }

        //Подготовка списка обновлений для указанного типа файлов
        private void PrepareDelegates(DbMode dbMode)
        {
            _delegates = new List<UpdateDelegate>();
            switch (dbMode)
            {
                case DbMode.Project:
                    AddAction(Update_1_0_1, "1.0.1", "10.10.2012");
                    AddAction(Update_1_0_2, "1.0.2", "26.10.2012", false);
                    AddAction(Update_1_0_3, "1.0.3", "08.11.2012");
                    AddAction(Update_1_0_4, "1.0.4", "15.11.2012", false);
                    AddAction(Update_1_0_5, "1.0.5", "26.11.2012");
                    AddAction(Update_1_0_6, "1.0.6", "27.11.2012");
                    AddAction(Update_1_0_7, "1.0.7", "05.12.2012");
                    AddAction(Update_1_0_8, "1.0.8", "23.01.2012");
                    AddAction(Update_1_0_9, "1.0.9", "29.05.2013");
                    AddAction(Update_1_0_10, "1.0.10", "20.06.2013");
                    AddAction(Update_1_1_0, "1.1.0", "27.06.2013");
                    AddAction(Update_1_1_1, "1.1.1", "03.07.2013");
                    AddAction(Update_1_1_2, "1.1.2", "23.07.2013");
                    AddAction(Update_1_1_3, "1.1.3", "30.07.2013");
                    AddAction(Update_1_1_4, "1.1.4", "26.08.2013");
                    AddAction(Update_1_1_5, "1.1.5", "19.09.2013");
                    AddAction(Update_1_1_6, "1.1.6", "30.09.2013");
                    AddAction(Update_1_2_1, "1.2.1", "09.12.2013");
                    AddAction(Update_1_2_2, "1.2.2", "18.02.2014");
                    AddAction(Update_1_2_3, "1.2.3", "08.04.2014");
                    AddAction(Update_1_2_4, "1.2.4", "07.05.2014");
                    AddAction(Update_1_3_0, "1.3.0", "16.06.2014");
                    AddAction(Update_1_3_1, "1.3.1", "29.07.2014");
                    AddAction(Update_1_3_2, "1.3.2", "25.08.2014");
                    AddAction(Update_1_3_3, "1.3.3", "30.10.2014");
                    AddAction(Update_1_3_4, "1.3.4", "31.10.2014");
                    AddAction(Update_1_3_5, "1.3.5", "06.11.2014");
                    AddAction(Update_1_3_6, "1.3.6", "06.11.2014");
                    AddAction(Update_1_3_7, "1.3.7", "07.07.2015");
                    AddAction(Update_1_3_8, "1.3.8", "01.09.2015");
                    AddAction(Update_1_3_9, "1.3.9", "11.09.2015");
                    AddAction(Update_1_3_10, "1.3.10", "16.09.2015");
                    AddAction(Update_1_3_11, "1.3.11", "29.09.2015");
                    AddAction(Update_1_3_12, "1.3.12", "30.09.2015");
                    AddAction(Update_1_3_13, "1.3.13", "04.10.2015");
                    AddAction(Update_1_3_14, "1.3.14", "05.10.2015");
                    AddAction(Update_1_3_15, "1.3.15", "23.11.2015");
                    AddAction(Update_1_3_16, "1.3.16", "13.01.2016");
                    AddAction(Update_1_3_17, "1.3.17", "17.02.2016");
                    break;
                case DbMode.Imit:
                    AddAction(UpdateImit_1_2_0, "1.2.0", "04.12.2013");
                    AddAction(UpdateImit_1_3_0, "1.3.0", "11.09.2015");
                    break;
                case DbMode.Clone:
                    AddAction(UpdateClone_1_3_0, "1.3.0", "11.09.2015");
                    AddAction(UpdateClone_1_3_1, "1.3.1", "30.09.2015");
                    break;
                case DbMode.AnalyzerAppData:
                    AddAction(UpdateAnalyzer_1_3_0, "1.3.0", "11.09.2015");
                    break;
                case DbMode.ConstructorAppData:
                    break;
                case DbMode.ControllerData:
                    AddAction(UpdateController_1_2_0, "1.2.0", "02.12.2013");
                    AddAction(UpdateController_1_2_1, "1.2.1", "20.02.2014");
                    AddAction(UpdateController_1_3_0, "1.3.0", "11.09.2015");
                    AddAction(UpdateController_1_3_1, "1.3.1", "12.02.2016");
                    AddAction(UpdateController_1_3_3, "1.3.3", "27.02.2017");
                    AddAction(UpdateController_1_3_4, "1.3.4", "07.09.2017");
                    break;
                case DbMode.ReporterData:
                    AddAction(UpdateReporter_1_2_0, "1.2.0", "30.01.2014");
                    AddAction(UpdateReporter_1_3_0, "1.3.0", "11.09.2015");
                    break;
                case DbMode.Archive:
                    AddAction(UpdateArchive_1_1_0, "1.1.0", "01.08.2013");
                    AddAction(UpdateArchive_1_3_0, "1.3.0", "09.06.2014");
                    AddAction(UpdateArchive_1_3_1, "1.3.1", "11.09.2015");
                    AddAction(UpdateArchive_1_3_2, "1.3.2", "01.02.2016");
                    AddAction(UpdateArchive_1_3_3, "1.3.3", "03.02.2016");
                    break;
                case DbMode.ArchiveSQL:
                    break;
            }
        }
        #endregion
        
        //Проект
        #region
        private void Update_1_0_1()
        {
            //CalcParamsArchive	Поле FullCode сделано уникальным
            _db.SetColumnIndex("CalcParamsArchive", "FullCode", IndexModes.UniqueIndex, "CodeCalc");
        }
        private void Update_1_0_2()
        {
            //SysTabl	В параметр CompileStatus добавлен подпараметр ErrorsCount, значение по умолчанию 0
            //CalcParams	Поле Number заменено на CalcNumber
            //CalcSubParams	Поле Number заменено на CalcNumber
            _db.AddSysSubParam(_templatePath, "CompileStatus", "ErrorsCount");
            _db.ConnectDao();
            _db.Database.TableDefs["CalcParams"].Fields["Number"].Name = "CalcNumber";
            _db.Database.TableDefs["CalcSubParams"].Fields["Number"].Name = "CalcNumber";
        }
        private void Update_1_0_3()
        {
            //Tasks	Добавлено поле TaskDescription (255 символов)
            //Signals	Добавлено поле ReceiverName (50 символов)
            //SignalsInUse	Добавлено поле ReceiverName (50 символов)
            _db.SetColumnString("Tasks", "TaskDescription");
            _db.SetColumnString("Signals", "ReceiverName", 50);
            _db.SetColumnString("SignalsInUse", "ReceiverName", 50);
        }
        private void Update_1_0_4()
        {
            //ObjectReceivers	Удалена
           _db.Execute("DROP TABLE ObjectReceivers");
        }
        private void Update_1_0_5()
        {
            //CalcParams	Добавлено поле DecPlaces (целое)
            //CalcSubParams	Добавлено поле DecPlaces (целое)
            _db.SetColumnLong("CalcParams", "DecPlaces");
            _db.SetColumnLong("CalcSubParams", "DecPlaces");
        }
        private void Update_1_0_6()
        {
            //CalcParamsArchive	Добавлено поле DataType
            //Добавлено поле SuperProcessType
            //Добавлено поле Min
            //Добавлено поле Max
            //Добавлено поле DecPlaces
            //Добавлено поле Units
            _db.SetColumnString("CalcParamsArchive", "DataType", 10);
            _db.SetColumnString("CalcParamsArchive", "SuperProcessType", 10);
            _db.SetColumnDouble("CalcParamsArchive", "Min");
            _db.SetColumnDouble("CalcParamsArchive", "Max");
            _db.SetColumnLong("CalcParamsArchive", "DecPlaces");
            _db.SetColumnString("CalcParamsArchive", "Units", 30);
        }
        private void Update_1_0_7()
        {
            //Signals	Поле NumSignal по умолчанию 0
            _db.SetColumnLong("Signals", "NumSignal", IndexModes.WithoutChange, 0);
        }
        private void Update_1_0_8()
        {
            //PrevParams	Добавлена таблица PrevParams	
            //Добавлена связь с таблицей CalcParamsArchive:	Включить каскадное удаление, каскдное обновление
            //CalcParamsArchive.Id=PrevParams.ArchiveParamId	
            //SysTabl	В параметр ArchiveOptions добавлены подпараметры 	
            //IsPrevAbs, IsLastBase, IsLastHour, IsLastDay, IsLastMoments,	
            //IsManyBase, IsManyHour, IsManyDay, IsManyMoments
            _db.AddTable("PrevParams", "[" + _templatePath + "].PrevParams");
            _db.Dispose();
            _db.SetColumnIndex("PrevParams", "FullCode", IndexModes.UniqueIndex);
            //db.ExecuteAdo("ALTER TABLE PrevParams ADD CONSTRAINT CalcParamsArchivePrevParams FOREIGN KEY (ArchiveParamId) REFERENCES CalcParamsArchive(Id) ON DELETE CASCADE ON UPDATE CASCADE");
            _db.AddForeignLink("PrevParams", "ArchiveParamId", "CalcParamsArchive", "Id");

            //отдельно настраиваем отображение в виде чекбоксов
            _db.SetColumnBool("PrevParams", "PrevAbs");
            _db.SetColumnBool("PrevParams", "LastBase");
            _db.SetColumnBool("PrevParams", "LastHour");
            _db.SetColumnBool("PrevParams", "LastDay");
            _db.SetColumnBool("PrevParams", "LastMoments");
            _db.SetColumnBool("PrevParams", "ManyBase");
            _db.SetColumnBool("PrevParams", "ManyHour");
            _db.SetColumnBool("PrevParams", "ManyDay");
            _db.SetColumnBool("PrevParams", "ManyMoments");

            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsPrevAbs");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsLastBase");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsLastHour");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsLastDay");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsLastMoments");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsManyBase");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsManyHour");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsManyDay");
            _db.AddSysSubParam(_templatePath, "ArchiveOptions", "IsManyMoments");
        }
        private void Update_1_0_9()
        {
            //Tasks	Добавлено поле TaskTag типа МЕМО и со сжатием Unicode
            _db.SetColumnMemo("Tasks", "TaskTag");
        }
        private void Update_1_0_10()
        {
            //SignalsInUse	Удален индекс с полей SourceName, ReceiverName
            //Signals	
            //PrevParams	Добавлен индекс в поле ArchiveParamId
            //Удален индекс с полей SourceName, ReceiverName
            _db.SetColumnIndex("SignalsInUse", "ReceiverName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SignalsInUse", "ProviderName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("Signals", "ProviderName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("Signals", "ReceiverName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("PrevParams", "ArchiveParamId", IndexModes.CommonIndex);
        }
        private void Update_1_1_0()
        {
            //Добавлены таблицы VedLinTemplate и VedParamTemplate из ProjectTemplate
            _db.AddTable("VedLinTemplate", "[" + _templatePath + "].VedLinTemplate");
            _db.AddTable("VedParamTemplate", "[" + _templatePath + "].VedParamTemplate");
            //Поправлено название VedLinTemplate
            _db.RenameTable("VedLinTemlpate", "VedLinTemplate");
            _db.Dispose();

            //При копировании таблиц индексы затираются, поэтому создаем их снова
            _db.SetColumnIndex("VedLinTemplate", "BookMark", IndexModes.CommonIndex);
            //_db.SetColumnIndex("VedLinTemplate", "ParamId", IndexModes.CommonIndex);
            _db.SetColumnIndex("VedLinTemplate", "Time", IndexModes.CommonIndex);
            _db.SetColumnIndex("VedParamTemplate", "CalcParamId", IndexModes.CommonIndex);

            //_db.SetColumnDefaultNumeric("VedLinTemplate", "ParamId");
            //_db.SetColumnDefaultNumeric("VedParamTemplate", "ParamId");
            _db.SetColumnLong("VedLinTemplate", "ParamId", IndexModes.CommonIndex, 0);
            _db.SetColumnLong("VedParamTemplate", "ParamId", IndexModes.WithoutChange, 0);
            //_db.ExecuteAdo("ALTER TABLE VedParamTemplate ALTER COLUMN [ParamId] SET DEFAULT " + 0);
            _db.SetColumnBool("VedLinTemplate", "Otm");
        }
        private void Update_1_1_1()
        {
            //Удалены поля UsedCalcParams и UsedSignals
            _db.DeleteColumn("CalcParams", "UsedCalcParams");
            _db.DeleteColumn("CalcParams", "UsedSignals");
            _db.DeleteColumn("CalcSubParams", "UsedCalcParams");
            _db.DeleteColumn("CalcSubParams", "UsedSignals");
            //Добавлено поле UsedUnits
            _db.SetColumnMemo("CalcParams", "UsedUnits");
            _db.SetColumnMemo("CalcSubParams", "UsedUnits");
        }
        private void Update_1_1_2()
        {
            //Добавлены Default, UsedUnits
            _db.SetColumnMemo("SignalsInUse", "UsedUnits");
            _db.SetColumnBool("SignalsInUse", "Default");
        }
        private void Update_1_1_3()
        {
            //Добавлено поле UsedUnits
            _db.SetColumnMemo("GraficsList", "UsedUnits");
        }
        private void Update_1_1_4()
        {
            //Добавлено поле Del в три таблицы
            _db.SetColumnBool("SignalsInUse", "Del");
            _db.SetColumnBool("CalcParamsArchive", "Del");
            _db.SetColumnBool("PrevParams", "Del");
        }
        private void Update_1_1_5()
        {
            //Поправлено название VedLinTemplate
            _db.RenameTable("VedLinTemlpate", "VedLinTemplate");

            //Переименованы индексы
            _db.SetColumnIndex("CalcParams", "Code", IndexModes.CommonIndex, "CodeCalc");
            _db.SetColumnIndex("CalcParams", "Task", IndexModes.CommonIndex, "IdTask");
            _db.SetColumnIndex("CalcParamsArchive", "FullCode", IndexModes.UniqueIndex, "CodeCalc");
            _db.SetColumnIndex("CalcSubParams", "Code", IndexModes.CommonIndex, "CodeCalc");
            _db.SetColumnIndex("CalcSubParams", "OwnerId", IndexModes.CommonIndex, "Task");
            _db.SetColumnIndex("GraficsList", "Code", IndexModes.CommonIndex, "CodeCalc");
            _db.SetColumnIndex("GraficsValues", "GraficId", IndexModes.CommonIndex, "NameGr");
            _db.SetColumnIndex("GraficsValues", "X1", IndexModes.CommonIndex, "Value");
            _db.SetColumnIndex("Objects", "CodeObject", IndexModes.CommonIndex, "KKS");
            _db.SetColumnIndex("Providers", "ProviderCode", IndexModes.CommonIndex, "ProvidersFromDbProviderType");
            _db.SetColumnIndex("Signals", "CodeSignal", IndexModes.CommonIndex, "SignalsCodeSignal");
            _db.SetColumnIndex("Tasks", "Task", IndexModes.UniqueIndex, "NodeText");
            _db.SetColumnIndex("VedLinTemplate", "Time", IndexModes.CommonIndex, "TimeColumn");
        }
        private void Update_1_1_6()
        {
            //Добавлено строковое поле SubParamsCount, по умолчанию 0
            _db.SetColumnString("CalcParams", "SubParamsCount", 30, IndexModes.EmptyIndex, "0");
        }
        private void Update_1_2_1()
        {
        }
        private void Update_1_2_2()
        {
            //Добавлено текстовое поле ConstValue
            _db.SetColumnString("SignalsInUse", "ConstValue");
            _db.AddSysSubParam(_templatePath, "ProjectInfo", "HandInputSource");
        }
        private void Update_1_2_3()
        {
            //Добавлено числовое поле ParamNum
            _db.SetColumnLong("VedParamTemplate", "ParamNum");
        }
        private void Update_1_2_4()
        {
            //Имя архива изменено на CalcArchive
            _db.Execute("UPDATE Providers SET Providers.ProviderName = 'CalcArchive' WHERE Providers.ProviderType='Архив'");
        }
        private void Update_1_3_0()
        {
            //В параметр ConstructorStatus добавлен подпараметр LastTagObjectUpdate
            _db.AddSysSubParam(_templatePath, "ConstructorStatus", "LastTagObjectUpdate");
        }
        private void Update_1_3_1()
        {
            //В таблицу SingnalsInUse добавлены поля CodeObject, CodeSignal
            _db.SetColumnString("SignalsInUse", "CodeObject", 100);
            _db.SetColumnString("SignalsInUse", "CodeSignal", 100);
        }
        private void Update_1_3_2()
        {
            //В таблицу SingnalsInUse добавлены поля NameObject, NameSignal, Units
            _db.SetColumnString("SignalsInUse", "NameObject");
            _db.SetColumnString("SignalsInUse", "NameSignal", 100);
            _db.SetColumnString("SignalsInUse", "Units", 50);
        }
        private void Update_1_3_3()
        {
            //Добавлена таблица MsgTable
            _db.AddTable("MsgTable", "[" + _templatePath + "].MsgTable");
            _db.Dispose();
        }
        private void Update_1_3_4()
        {
            //В таблицу VedParamTemplate добавлены поля VedParamTag и MsgKey
            _db.SetColumnMemo("VedParamTemplate", "VedParamTag");
            _db.SetColumnString("VedParamTemplate", "MsgKey");
            //В таблицу VedLinTemplate добавлены поля StrValue и Status
            _db.SetColumnMemo("VedLinTemplate", "StrValue");
            _db.SetColumnString("VedLinTemplate", "Status");
        }
        private void Update_1_3_5()
        {
            //В таблице MsgTable поле Label0 сделано текстовым
            _db.DeleteColumn("MsgTable", "Label0");
            _db.SetColumnString("MsgTable", "Label0", 40);
        }
        private void Update_1_3_6()
        {
            //В SysTabl добавлен параметр LinVedStatusFormatCondition
            _db.AddSysParam(_templatePath, "LinVedStatusFormatCondition");
        }
        private void Update_1_3_7()
        {
            //Добавлена таблица VedColumns
            _db.AddTable("VedColumns", "[" + _templatePath + "].VedColumns");
            //В таблицу SignalsInUse добавлено поле МEMO TagObject
            _db.SetColumnMemo("SignalsInUse", "TagObject");
            //В таблицу CalcParams добавлено поле CodeSignal
            _db.SetColumnString("CalcParams", "CodeSignal");
        }
        private void Update_1_3_8()
        {
            //В таблицу CalcSubParams добавлено поле CodeSignal
            _db.SetColumnString("CalcSubParams", "CodeSignal");
        }
        private void Update_1_3_9()
        {
            //Удален индекс с поля SubParamId
            //Добавлен составной уникальный индекс на поля ParamId и SubParamName
            _db.SetColumnIndex("SysSubTabl", "ParamId", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamId", "SubParamName", true, IndexModes.UniqueIndex, "PrimaryKey");
        }
        private void Update_1_3_10()
        {
            //В таблицу VedColumns добавлено поле ReportColumnWidth
            _db.SetColumnString("VedColumns", "ReportColumnWidth");
        }
        private void Update_1_3_11()
        {
            //В таблицу VedColumns добавлено поле ColumnOrder
            _db.SetColumnLong("VedColumns", "ColumnOrder");
        }
        private void Update_1_3_12()
        {
            //В таблицу Objects добавлены поля Prop10 - Prop15
            _db.SetColumnString("Objects", "Prop10");
            _db.SetColumnString("Objects", "Prop11");
            _db.SetColumnString("Objects", "Prop12");
            _db.SetColumnString("Objects", "Prop13");
            _db.SetColumnString("Objects", "Prop14");
            _db.SetColumnString("Objects", "Prop15");
        }
        private void Update_1_3_13()
        {
            //Добавлена таблица DecodeTable
            _db.AddTable("DecodeTable", "[" + _templatePath + "].DecodeTable");
            _db.Dispose();
        }
        private void Update_1_3_14()
        {
            //В таблицу VedParamTemplate Добавлено поле DecodeKey
            _db.SetColumnString("VedParamTemplate", "DecodeKey", 50);
        }
        private void Update_1_3_15()
        {
            //В таблице MsgTable размер полей Label_0, Label_1 увеличен до 100 символов
            _db.SetColumnString("MsgTable", "Label_0", 100);
            _db.SetColumnString("MsgTable", "Label_1", 100);
        }
        private void Update_1_3_16()
        {
            //В таблицах CalcParams и CalcSubParams размер поля Code увеличен до 255 символов
            //Не удается поменять размер, так как поле участвует в связях
            //_db.SetColumnString("CalcParams", "Code");
            _db.SetColumnString("CalcSubParams", "Code");
        }
        private void Update_1_3_17()
        {
            //В таблице Objects размер поля TypeObject увеличен до 255 символов
            _db.SetColumnString("Objects", "TypeObject");
        }
        #endregion

        //Архивы
        #region
        private void UpdateArchive_1_1_0()
        {
        }

        private void UpdateArchive_1_3_0()
        {
            //Добавлено поле IsAbsoluteDay
            _db.SetColumnBool("ReportParams", "IsAbsoluteDay");
        }

        private void UpdateArchive_1_3_1()
        {
            //Удален индекс с поля SubParamId
            //Добавлен составной уникальный индекс на поля ParamId и SubParamName
            _db.SetColumnIndex("SysSubTabl", "ParamId", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamId", "SubParamName", true, IndexModes.UniqueIndex, "PrimaryKey");
        }

        private void UpdateArchive_1_3_2()
        {
            //Добавлены поля в ReportParams
            _db.SetColumnBool("ReportParams", "IsAbsoluteListBase");
            _db.SetColumnBool("ReportParams", "IsAbsoluteListHour");
            _db.SetColumnBool("ReportParams", "IsAbsoluteListDay");
        }

        private void UpdateArchive_1_3_3()
        {
            //Добавлены поля в ReportParams
            _db.SetColumnBool("ReportParams", "IsAbsoluteCombined");
        }
        #endregion
        
        //Файлы имитационных данных и клона
        #region
        private void UpdateImit_1_2_0()
        {
        }

        private void UpdateImit_1_3_0()
        {
            //Удален индекс с поля SubParamId
            //Добавлен составной уникальный индекс на поля ParamId и SubParamName
            _db.SetColumnIndex("SysSubTabl", "ParamId", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamId", "SubParamName", true, IndexModes.UniqueIndex, "PrimaryKey");
        }

        private void UpdateClone_1_3_0()
        {
            //Удален индекс с поля SubParamId
            //Добавлен составной уникальный индекс на поля ParamId и SubParamName
            _db.SetColumnIndex("SysSubTabl", "ParamId", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamId", "SubParamName", true, IndexModes.UniqueIndex, "PrimaryKey");
        }

        private void UpdateClone_1_3_1()
        {
            //В таблицу Objects добавлены поля Prop10 - Prop15
            _db.SetColumnString("Objects", "Prop10");
            _db.SetColumnString("Objects", "Prop11");
            _db.SetColumnString("Objects", "Prop12");
            _db.SetColumnString("Objects", "Prop13");
            _db.SetColumnString("Objects", "Prop14");
            _db.SetColumnString("Objects", "Prop15");
        }
        #endregion

        //Файлы настроек
        #region
        private void UpdateController_1_2_0()
        {
            //Threads Добавлено логическое поле IsImit
            _db.SetColumnBool("Threads", "IsImit");
            //Threads Добавлено текстовое поле ImitMode
            _db.SetColumnString("Threads", "ImitMode", 100);
        }

        private void UpdateController_1_2_1()
        {
            //Добавлен параметр DebugWriteOPC и его подпараметры OpcServerName, Node, OpcTag, DataType, TagValue
            _db.AddSysParam(_templatePath, "DebugWriteOPC");
            _db.AddSysSubParam(_templatePath, "DebugWriteOPC", "OpcServerName");
            _db.AddSysSubParam(_templatePath, "DebugWriteOPC", "Node");
            _db.AddSysSubParam(_templatePath, "DebugWriteOPC", "OpcTag");
            _db.AddSysSubParam(_templatePath, "DebugWriteOPC", "DataType");
            _db.AddSysSubParam(_templatePath, "DebugWriteOPC", "TagValue");
        }

        private void UpdateReporter_1_2_0()
        {
            //Reports Добавлены поля FormInf и UseOneArchive
            _db.SetColumnMemo("Reports", "FormInf");
            _db.SetColumnBool("Reports", "UseOneArchive");
            //Projects Добавлены поля CodeArchive и InfAcrhive
            _db.SetColumnString("Projects", "CodeArchive", 50);
            _db.SetColumnMemo("Projects", "InfArchive");

            //Добавлены таблицы GroupReports и ReportsForGroup
            _db.AddTable("GroupReports", "[" + _templatePath + "].GroupReports");
            _db.AddTable("ReportsForGroup", "[" + _templatePath + "].ReportsForGroup"); 
            _db.Dispose();

            //Добавлена связь между таблицами GroupReports и ReportsForGroup
            _db.AddForeignLink("ReportsForGroup", "GroupId", "GroupReports", "GroupId");
            //При копировании таблиц индексы затираются, поэтому создаем их снова
            _db.SetColumnIndex("GroupReports", "GroupCode", IndexModes.CommonIndex);
            _db.SetColumnIndex("GroupReports", "ThreadId", IndexModes.CommonIndex);
            _db.SetColumnIndex("GroupReports", "ThreadId", IndexModes.CommonIndex);
            _db.SetColumnIndex("ReportsForGroup", "GroupId", IndexModes.CommonIndex);
            _db.SetColumnIndex("ReportsForGroup", "Report", IndexModes.CommonIndex);
        }

        private void UpdateController_1_3_0()
        {
            //Удален индекс с поля SubParamId
            //Добавлен составной уникальный индекс на поля ParamId и SubParamName
            _db.SetColumnIndex("SysSubTabl", "ParamId", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamId", "SubParamName", true, IndexModes.UniqueIndex, "PrimaryKey");
        }

        private void UpdateController_1_3_1()
        {
            //Threads Добавлено поле RushWaitingTime
            _db.SetColumnDouble("Threads", "RushWaitingTime");
        }

        private void UpdateController_1_3_3()
        {
            //Добавлен параметр MonitorHistoryCheckTime
            _db.AddSysParam(_templatePath, "MonitorHistoryCheckTime");
        }

        private void UpdateController_1_3_4()
        {
            //Добавлен параметр MonitorHistoryProps
            _db.AddSysParam(_templatePath, "MonitorHistoryProps");
        }

        private void UpdateReporter_1_3_0()
        {
            //Удален индекс с поля SubParamId
            //Добавлен составной уникальный индекс на поля ParamId и SubParamName
            _db.SetColumnIndex("SysSubTabl", "ParamId", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamId", "SubParamName", true, IndexModes.UniqueIndex, "PrimaryKey");
        }

        private void UpdateAnalyzer_1_3_0()
        {
            //Удален индекс с поля SubParamId
            //Добавлен составной уникальный индекс на поля ParamId и SubParamName
            _db.SetColumnIndex("SysSubTabl", "ParamId", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamName", IndexModes.EmptyIndex);
            _db.SetColumnIndex("SysSubTabl", "ParamId", "SubParamName", true, IndexModes.UniqueIndex, "PrimaryKey");
        }
        #endregion

        //Регистрация
        #region
        //Запись в реестр даты установки
        public static void SetDate()
        {
            try
            {
                RegistryKey dateKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\UAI", false);
                if (dateKey == null || dateKey.GetValue("id") == null || (string)dateKey.GetValue("id") == "")
                {
                    dateKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\UAI");
                    dateKey.SetValue("id", Crypting.Encrypt(DateTime.Now.ToOADate().ToString()), RegistryValueKind.String);
                }
                dateKey.Close();
            }
            catch (Exception e1)
            {
                e1.MessageError();
            }
        }

        //Возвращает, зарегистрировано ли приложение (код приложения изменен)
        private bool IsReg(string app, RegistryKey key)
        {
            return key.GetValue(app) != null && Crypting.Decrypt(key.GetValue(app).ToString()).ToLower() == "true";
        }

        //Читает из реестра, сколько дней прошло с активации
        private int DaysFromActivation(RegistryKey key)
        {
            int errResult = Convert.ToInt32(DateTime.Now.Subtract(Different.MinDate).TotalDays);
            try
            {
                DateTime insDate = DateTime.FromOADate(Crypting.Decrypt(key.GetValue("id").ToString()).ToDouble());
                return Convert.ToInt32(DateTime.Now.Subtract(insDate).TotalDays);
            }
            catch { return errResult;}
        }

        private int _leftDays;

        //Возвращает -1, если в реестре не были найдены необходимые записи; 0, если приложение не активиновано и пробный период иссяк
        //1, если прилож не активировано, но пробный период еще идет; 2, если приложение активировано
        public int ACheck(string app, bool silent = false)
        {
            try
            {
                RegistryKey UAIKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\UAI");
                if (UAIKey == null || UAIKey.GetValue("id") == null)
                {
                    if (!silent) Different.MessageError("Ошибка целостности программного обеспечения. Рекомендуется переустановить комплекс InfoTask");
                    return -1;
                }
                bool appRegistered = false;
                string application = "";
                switch (app)
                {
                    case "Constructor":
                        appRegistered = IsReg("rc", UAIKey);
                        application = "Constructor";
                        break;
                    case "Ras":
                        appRegistered = IsReg("sr", UAIKey);
                        application = "RAS";
                        break;
                    case "RasInfoTask":
                        appRegistered = IsReg("ri", UAIKey);
                        application = "RasInfoTask";
                        break;
                    case "Controller":
                        appRegistered = IsReg("rm", UAIKey);
                        application = "Controller";
                        break;
                    case "Analyzer":
                        appRegistered = IsReg("ra", UAIKey);
                        application = "Analyzer";
                        break;
                    case "AnalyzerInfoTask":
                        appRegistered = IsReg("ai", UAIKey);
                        application = "AnalyzerInfoTask";
                        break;
                    case "Excel":
                        appRegistered = IsReg("rr", UAIKey);
                        application = "Reporter";
                        break;
                    case "CalcArchiveViewer":
                        appRegistered = IsReg("rv", UAIKey);
                        application = "CalcArchiveViewer";
                        break;
                    case "ProjectManager":
                        appRegistered = IsReg("pm", UAIKey);
                        application = "ProjectManager";
                        break;
                }
                if (appRegistered)
                {
                    UAIKey.Close();
                    return 2;
                }

                _leftDays = 31 - DaysFromActivation(UAIKey); 
                UAIKey.Close();
                if (_leftDays >= 0)
                {
                    if (!silent)
                        MessageBox.Show("Продукт " + application + ", входящий в состав системы InfoTask не активирован. У вас осталось " + _leftDays + " дней пробной версии", "InfoTask");
                    return 1;
                }
                if (!silent) Different.MessageError("Срок пробной версии InfoTask." + application + " истек. Чтобы продолжить работу, необходимо активировать продукт");
                return 0;
            }
            catch (Exception e1)
            {
                if (!silent) e1.MessageError();
            }
            return -1;
        }

        //Возвращает пользователя указанного приложения или сообщает о пробном периоде
        public string AUser(string app)
        {
            int c = ACheck(app, true);
            if (c <= 0) return "Приложение не активировано. Пробный период истек.";
            if (c == 1) return "Приложение не активировано. Осталось " + _leftDays + " дней до окончания пробного периода.";
            RegistryKey UAIKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\UAI");
            try
            {
                return Crypting.Decrypt(UAIKey.GetValue("cp").ToString());
            }
            catch { return ""; }
            finally { UAIKey.Close(); } 
        }

        //Возвращает номер лицензии указанного приложения или сообщает, что нет лицензии
        public string ANumber(string app)
        {
            int c = ACheck(app, true);
            if (c <= 1) return "Нет лицензии";
            RegistryKey UAIKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\UAI");
            try
            {
                return Crypting.Decrypt(UAIKey.GetValue("un").ToString());
            }
            catch { return "Нет лицензии"; }
            finally { UAIKey.Close(); }
        }
        
        #endregion
    }
}
