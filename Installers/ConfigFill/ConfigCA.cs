using System;
using System.Diagnostics;
using Microsoft.Win32;
using BaseLibrary;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;
using System.IO;
using VersionSynch;

namespace ConfigFill
{
    public class CustomActions
    {
        //Копирует одну строчку из SysTabl ConfigTemplate вместе с подтаблицей
        private static void CopyConfigProp(string propName, RecDao cfgTemplate, RecDao cfg)
        {
            cfgTemplate.FindFirst("ParamName", propName);
            var id = CopyProp(cfgTemplate, cfg);
            CopySubProp(cfgTemplate.GetInt("ParamId"), cfgTemplate.DaoDb, id, cfg.DaoDb);    
        }

        private static int CopyProp(RecDao cfgTemplate, RecDao cfg)
        {
            bool flag = !cfg.FindFirst("ParamName", cfgTemplate.GetString("ParamName"));
            //if (!cfg.FindFirst("ParamName", cfgTemplate.GetString("ParamName")))
            if (flag)
            //    MessageBox.Show("Я не нашел запись в конфиге, о чем сейчас и сообщаю");
            //else MessageBox.Show("Я нашел запись в тимплейте. А вы чем занимаетесь?");
                cfg.AddNew();
            cfg.Put("ParamName", cfgTemplate.GetString("ParamName"));    
            cfg.Put("ParamType", cfgTemplate.GetString("ParamType"));
            cfg.Put("ParamValue", cfgTemplate.GetString("ParamValue"));
            cfg.Put("ParamDescription", cfgTemplate.GetString("ParamDescription"));
            cfg.Put("ParamTag", cfgTemplate.GetString("ParamTag"));
            int id = cfg.GetInt("ParamId");
            //MessageBox.Show(flag + " " + cfgTemplate.GetString("ParamName"));
            try
            {
                cfg.Update();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            return id;
        }

        private static void CopySubProp(int tid, DaoDb tdb, int id, DaoDb db)
        {
            using (var rec = new RecDao(tdb, "SELECT * FROM SysSubTabl WHERE ParamId=" + tid))
            {
                db.Execute("DELETE * FROM SysSubTabl WHERE ParamId=" + id);
                using (var res = new RecDao(db, "SELECT * FROM SysSubTabl WHERE ParamId=" + id))
                    while (rec.Read())
                    {
                        res.AddNew();
                        res.Put("ParamId", id);
                        res.Put("SubParamNum", rec.GetInt("SubParamNum"));
                        res.Put("SubParamType", rec.GetString("SubParamType"));
                        res.Put("SubParamName", rec.GetString("SubParamName"));
                        res.Put("SubParamValue", rec.GetString("SubParamValue"));
                        res.Put("SubParamDescription", rec.GetString("SubParamDescription"));
                        res.Put("SubParamTag", rec.GetString("SubParamTag"));
                        res.Put("SubParamRowSource", rec.GetString("SubParamRowSource"));
                    }    
            }
        }

        //Заполнение файла конфигурации
        //Установка ядра InfoTask
        [CustomAction]
        public static ActionResult ConfigFillIT(Session session)
        {
            session.Log("Begin ConfigFillIT");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("General") + "ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("General") + "Config.accdb", "SELECT * FROM SysTabl"))
                    {
                        if (session.Features["KernelMainFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("InfoTask", cfgTemplate, cfg);
                            CopyConfigProp("AccessArchive", cfgTemplate, cfg);
                            CopyConfigProp("SQLServerArchive", cfgTemplate, cfg);
                            CopyConfigProp("ArchiveProjectComm", cfgTemplate, cfg);
                            CopyConfigProp("ArchiveAccessSource", cfgTemplate, cfg);
                            CopyConfigProp("ArchiveSQLServerSource", cfgTemplate, cfg);
                            CopyConfigProp("Imitator", cfgTemplate, cfg);
                            CopyConfigProp("CloneComm", cfgTemplate, cfg);
                            CopyConfigProp("CloneSource", cfgTemplate, cfg);
                            CopyConfigProp("HandInputSource", cfgTemplate, cfg);
                            CopyConfigProp("HandInputSqlSource", cfgTemplate, cfg);
                            CopyConfigProp("MonitorHistory", cfgTemplate, cfg);
                        }
                        if (session.Features["ControllerMonitorFeature"].RequestState == InstallState.Local)
                            CopyConfigProp("Controller", cfgTemplate, cfg);
                        if (session.Features["CalcArchiveViewerFeature"].RequestState == InstallState.Local)
                            CopyConfigProp("CalcArchiveViewer", cfgTemplate, cfg);
                    }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillIT");

            return ActionResult.Success;
        }

        //Удаление ядра InfoTask
        [CustomAction]
        public static ActionResult ConfigFillITU(Session session)
        {
            session.Log("Begin ConfigFillITU");
            try
            {
                using (var db = new DaoDb(session.GetTargetPath("General") + "Config.accdb"))
                {
                    bool cFlag = false;
                    string warnM = "Обнаружены установленные компоненты InfoTask:";
                    using (var cfg = new RecDao(db, "SELECT * FROM SysTabl"))
                    {
                        //Проверка, есть ли установленные иные компоненты ИТ
                        cfg.FindFirst("ParamName='Analyzer'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - Analyzer";
                        }
                        cfg.FindFirst("ParamName='AnalyzerInfoTask'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - AnalyzerInfoTask";
                        }
                        cfg.FindFirst("ParamName='Constructor'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - Constructor";
                        }
                        cfg.FindFirst("ParamName='Reporter'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - Reporter";
                        }
                        cfg.FindFirst("ParamName='KosmotronikaRetroSource'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - KosmotronikaProviders";
                        }
                        cfg.FindFirst("ParamName='OvationSource'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - OvationProviders";
                        }
                        cfg.FindFirst("ParamName='WonderwareHistorianSource'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - WonderwareProviders";
                        }
                        cfg.FindFirst("ParamName='SimaticSource'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - SiemensProviders";
                        }
                        cfg.FindFirst("ParamName='MirSource'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - MirProviders";
                        }
                        cfg.FindFirst("ParamName='PrologSource'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - LogikaProviders";
                        }
                        cfg.FindFirst("ParamName='RasOvation'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - RAS";
                        }
                        cfg.FindFirst("ParamName='RasKosmotronika'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - RasKosmotronika";
                        }
                        cfg.FindFirst("ParamName='RasInfoTask'");
                        if (!cfg.NoMatch())
                        {
                            cFlag = true;
                            warnM += "\n  - RasInfoTask";
                        }
                                                
                    }
                    if (cFlag)
                    {
                        warnM += "\n Необходимо удалить/переустановить и их";
                        MessageBox.Show(warnM);
                        //return ActionResult.UserExit;
                    }

                    if (session.Features["KernelMainFeature"].RequestState == InstallState.Absent)
                        db.Execute("DELETE * FROM SysTabl WHERE ParamName='InfoTask' OR ParamName='AccessArchive' OR ParamName='SQLServerArchive' " +
                                   "OR ParamName='ArchiveProjectComm' OR ParamName='ArchiveAccessComm' OR ParamName='ArchiveSQLServerComm' OR ParamName='ArchiveAccessSource' " +
                                   "OR ParamName='ArchiveSQLServerSource' OR ParamName='Imitator' OR ParamName='CloneComm' OR ParamName='CloneSource' " +
                                   "OR ParamName='HandInputSource' OR ParamName='HandInputSqlSource' OR ParamName='MonitorHistory'");
                    
                    if (session.Features["ControllerMonitorFeature"].RequestState == InstallState.Absent)
                        db.Execute("DELETE * FROM SysTabl WHERE ParamName='Controller'");
                    if (session.Features["CalcArchiveViewerFeature"].RequestState == InstallState.Absent)
                        db.Execute("DELETE * FROM SysTabl WHERE ParamName='CalcArchiveViewer'");
                }
            }
            catch (Exception exception)
            {
                //return ActionResult.UserExit;
                exception.MessageError();
            }
            session.Log("End ConfigFillITU");
            return ActionResult.Success;
        }

        //Установка анализатора
        [CustomAction]
        public static ActionResult ConfigFillAnalyzer(Session session)
        {
            session.Log("Begin ConfigFillAnalyzer");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["AnalyzerFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("Analyzer", cfgTemplate, cfg);
                        }    
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillAnalyzer");

            return ActionResult.Success;
        }

        //Удаление анализатора
        [CustomAction]
        public static ActionResult ConfigFillAnalyzerU(Session session)
        {
            session.Log("Begin ConfigFillAnalyzerU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                                  "DELETE * FROM SysTabl WHERE ParamName='Analyzer'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillAnalyzerU");

            return ActionResult.Success;
        }

        //Установка анализатора ИнфоТаск
        [CustomAction]
        public static ActionResult ConfigFillAnalyzerInfoTask(Session session)
        {
            session.Log("Begin ConfigFillAnalyzerInfoTask");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                    if (session.Features["AnalyzerInfoTaskFeature"].RequestState == InstallState.Local)
                    {
                        CopyConfigProp("AnalyzerInfoTask", cfgTemplate, cfg);
                    }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillAnalyzerInfoTask");

            return ActionResult.Success;
        }

        //Удаление анализатора ИнфоТаск
        [CustomAction]
        public static ActionResult ConfigFillAnalyzerInfoTaskU(Session session)
        {
            session.Log("Begin ConfigFillAnalyzerInfoTaskU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                                  "DELETE * FROM SysTabl WHERE ParamName='AnalyzerInfoTask'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillAnalyzerInfoTaskU");

            return ActionResult.Success;
        }

        //Установка конструктора
        [CustomAction]
        public static ActionResult ConfigFillConstructor(Session session)
        {
            session.Log("Begin ConfigFillConstructor");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["ConstructorFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("Constructor", cfgTemplate, cfg);
                        }    
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillConstructor");

            return ActionResult.Success;
        }

        //Удаление конструктора
        [CustomAction]
        public static ActionResult ConfigFillConstructorU(Session session)
        {
            session.Log("Begin ConfigFillConstructorU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                                  "DELETE * FROM SysTabl WHERE ParamName='TestCommunicator' OR ParamName='TestSource' OR ParamName='Constructor'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillConstructorU");

            return ActionResult.Success;
        }

        //Установка репортера
        [CustomAction]
        public static ActionResult ConfigFillReporter(Session session)
        {
            session.Log("Begin ConfigFillReporter");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["ReporterFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("Reporter", cfgTemplate, cfg);
                        }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillReporter");

            return ActionResult.Success;
        }

        //Удаление репортера
        [CustomAction]
        public static ActionResult ConfigFillReporterU(Session session)
        {
            session.Log("Begin ConfigFillReporterU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                                  "DELETE * FROM SysTabl WHERE ParamName='Reporter'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillReporterU");

            return ActionResult.Success;
        }

        //Установка провайдеров космотроники
        [CustomAction]
        public static ActionResult ConfigFillK(Session session)
        {
            session.Log("Begin ConfigFillK");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["KosmotronikaFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("KosmotronikaSQLComm", cfgTemplate, cfg);
                            CopyConfigProp("KosmotronikaDbfComm", cfgTemplate, cfg);
                            CopyConfigProp("KosmotronikaRetroSource", cfgTemplate, cfg);
                            CopyConfigProp("KosmotronikaArchDbSource", cfgTemplate, cfg);
                            CopyConfigProp("KosmotronikaOPCReceiver", cfgTemplate, cfg);
                        }
                        //else if (session.Features["KosmotronikaFeature"].RequestState == InstallState.Removed)
                        //{
                        //    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                        //    "DELETE * FROM SysTabl WHERE ParamName='KosmotronikaSQLComm' OR ParamName='KosmotronikaDbfComm' OR ParamName='KosmotronikaRetroSource' OR ParamName='KosmotronikaOPCReceiver'");
                        //}
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillK");

            return ActionResult.Success;
        }

        //Удаление провайдеров космотроники
        [CustomAction]
        public static ActionResult ConfigFillKU(Session session)
        {
            session.Log("Begin ConfigFillKU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                        "DELETE * FROM SysTabl WHERE ParamName='KosmotronikaSQLComm' OR ParamName='KosmotronikaDbfComm' OR ParamName='KosmotronikaRetroSource' OR ParamName='KosmotronikaArchDbSource' OR ParamName='KosmotronikaOPCReceiver'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillKU");

            return ActionResult.Success;
        }

        //Установка провайдеров овации
        [CustomAction]
        public static ActionResult ConfigFillO(Session session)
        {
            session.Log("Begin ConfigFillO");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["OvationFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("OvationHistorianComm", cfgTemplate, cfg);
                            CopyConfigProp("OvationHistorianSource", cfgTemplate, cfg);
                            CopyConfigProp("OvationComm", cfgTemplate, cfg);
                            CopyConfigProp("OvationSource", cfgTemplate, cfg);
                            CopyConfigProp("OvationOPCReceiver", cfgTemplate, cfg);
                        }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillO");

            return ActionResult.Success;
        }

        //Удаление провайдеров овации
        [CustomAction]
        public static ActionResult ConfigFillOU(Session session)
        {
            session.Log("Begin ConfigFillOU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                    if (instlocPath.Exists) DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                        "DELETE * FROM SysTabl WHERE ParamName='OvationHistorianComm' OR ParamName='OvationHistorianSource' " +
                        "OR ParamName='OvationSource' OR ParamName='OvationComm' OR ParamName='OvationOPCReceiver'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillOU");

            return ActionResult.Success;
        }

        //Установка провайдеров Wonderware
        [CustomAction]
        public static ActionResult ConfigFillW(Session session)
        {
            session.Log("Begin ConfigFillWonderware");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["WonderwareFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("WonderwareHistorianComm", cfgTemplate, cfg);
                            CopyConfigProp("WonderwareHistorianSource", cfgTemplate, cfg);
                            CopyConfigProp("WonderwareOPCReceiver", cfgTemplate, cfg);
                        }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillWonderware");

            return ActionResult.Success;
        }

        //Удаление провайдеров Wonderware
        [CustomAction]
        public static ActionResult ConfigFillWU(Session session)
        {
            session.Log("Begin ConfigFillWonderwareU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists) DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                    "DELETE * FROM SysTabl WHERE ParamName='WonderwareHistorianComm' OR ParamName='WonderwareHistorianSource' OR ParamName='WonderwareOPCReceiver'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillWonderwareU");

            return ActionResult.Success;
        }

        //Установка провайдеров Siemens
        [CustomAction]
        public static ActionResult ConfigFillS(Session session)
        {
            session.Log("Begin ConfigFillSiemens");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["SiemensFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("SimaticComm", cfgTemplate, cfg);
                            CopyConfigProp("SimaticSource", cfgTemplate, cfg);
                        }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillSiemens");

            return ActionResult.Success;
        }

        //Удаление провайдеров Siemens
        [CustomAction]
        public static ActionResult ConfigFillSU(Session session)
        {
            session.Log("Begin ConfigFillSiemensU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists) DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                    "DELETE * FROM SysTabl WHERE ParamName='SimaticComm' OR ParamName='SimaticSource'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillSiemensU");

            return ActionResult.Success;
        }

        //Установка провайдеров Mir
        [CustomAction]
        public static ActionResult ConfigFillMir(Session session)
        {
            session.Log("Begin ConfigFillMir");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                    if (session.Features["MirFeature"].RequestState == InstallState.Local)
                    {
                        CopyConfigProp("MirComm", cfgTemplate, cfg);
                        CopyConfigProp("MirSource", cfgTemplate, cfg);
                    }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillMir");

            return ActionResult.Success;
        }

        //Удаление провайдеров Mir
        [CustomAction]
        public static ActionResult ConfigFillMirU(Session session)
        {
            session.Log("Begin ConfigFillMirU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists) DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                    "DELETE * FROM SysTabl WHERE ParamName='MirComm' OR ParamName='MirSource'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillMirU");

            return ActionResult.Success;
        }

        //Установка провайдеров Logika
        [CustomAction]
        public static ActionResult ConfigFillLogika(Session session)
        {
            session.Log("Begin ConfigFillLogika");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                    if (session.Features["LogikaFeature"].RequestState == InstallState.Local)
                    {
                        CopyConfigProp("PrologComm", cfgTemplate, cfg);
                        CopyConfigProp("PrologSource", cfgTemplate, cfg);
                    }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillLogika");

            return ActionResult.Success;
        }

        //Удаление провайдеров Logika
        [CustomAction]
        public static ActionResult ConfigFillLogikaU(Session session)
        {
            session.Log("Begin ConfigFillLogikaU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists) DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                    "DELETE * FROM SysTabl WHERE ParamName='PrologComm' OR ParamName='PrologSource'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillLogikaU");

            return ActionResult.Success;
        }

        //Установка провайдеров Kvint
        [CustomAction]
        public static ActionResult ConfigFillKvint(Session session)
        {
            session.Log("Begin ConfigFillKvint");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                    if (session.Features["KvintFeature"].RequestState == InstallState.Local)
                    {
                        CopyConfigProp("KvintComm", cfgTemplate, cfg);
                        CopyConfigProp("KvintSource", cfgTemplate, cfg);
                    }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillKvint");

            return ActionResult.Success;
        }

        //Удаление провайдеров Mir
        [CustomAction]
        public static ActionResult ConfigFillKvintU(Session session)
        {
            session.Log("Begin ConfigFillKvintU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists) DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                    "DELETE * FROM SysTabl WHERE ParamName='KvintComm' OR ParamName='KvintSource'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillKvintU");

            return ActionResult.Success;
        }

        //Установка РАС
        [CustomAction]
        public static ActionResult ConfigFillRAS(Session session)
        {
            session.Log("Begin ConfigFillRAS");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["RASFeature"].RequestState == InstallState.Local)
                        {
                            CopyConfigProp("RasOvation", cfgTemplate, cfg);
                        }
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillRAS");
            return ActionResult.Success;
        }
        
        //Удаление РАС
        [CustomAction]
        public static ActionResult ConfigFillRASU(Session session)
        {
            session.Log("Begin ConfigFillRASU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                        "DELETE * FROM SysTabl WHERE ParamName='RasOvation'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillRASU");
            return ActionResult.Success;
        }

        //Установка РАС Космотроника
        [CustomAction]
        public static ActionResult ConfigFillRASKosmotronika(Session session)
        {
            session.Log("Begin ConfigFillRASKosmotronika");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                    using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                        if (session.Features["RASKosmotronikaFeature"].RequestState == InstallState.Local)
                            CopyConfigProp("RASKosmotronika", cfgTemplate, cfg);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillRASKosmotronika");
            return ActionResult.Success;
        }

        //Удаление РАС Космотроника
        [CustomAction]
        public static ActionResult ConfigFillRASKosmotronikaU(Session session)
        {
            session.Log("Begin ConfigFillRASKosmotronikaU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                        "DELETE * FROM SysTabl WHERE ParamName='RASKosmotronika'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillRASKosmotronikaU");
            return ActionResult.Success;
        }



        //Установка РАС ИнфоТаск
        [CustomAction]
        public static ActionResult ConfigFillRASInfoTask(Session session)
        {
            session.Log("Begin ConfigFillRasInfoTask");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                    if (session.Features["RasInfoTaskFeature"].RequestState == InstallState.Local)
                        CopyConfigProp("RasInfoTask", cfgTemplate, cfg);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillRasInfoTask");
            return ActionResult.Success;
        }

        //Удаление РАС ИнфоТаск
        [CustomAction]
        public static ActionResult ConfigFillRASInfoTaskU(Session session)
        {
            session.Log("Begin ConfigFillRasInfoTaskU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                        "DELETE * FROM SysTabl WHERE ParamName='RasInfoTask'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillRasInfoTaskU");
            return ActionResult.Success;
        }

        //Установка ProjectManager ИнфоТаск
        [CustomAction]
        public static ActionResult ConfigFillProjectManager(Session session)
        {
            session.Log("Begin ConfigFillProjectManager");
            try
            {
                using (var cfgTemplate = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\ConfigTemplate.accdb", "SELECT * FROM SysTabl"))
                using (var cfg = new RecDao(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb", "SELECT * FROM SysTabl"))
                    if (session.Features["ProjectManagerFeature"].RequestState == InstallState.Local)
                        CopyConfigProp("ProjectManager", cfgTemplate, cfg);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillProjectManager");
            return ActionResult.Success;
        }

        //Удаление Проджект Менеджер ИнфоТаск
        [CustomAction]
        public static ActionResult ConfigFillProjectManagerU(Session session)
        {
            session.Log("Begin ConfigFillProjectManagerU");
            try
            {
                var instlocPath = new FileInfo(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb");
                if (instlocPath.Exists)
                    DaoDb.Execute(session.GetTargetPath("INSTALLLOCATION") + "General\\Config.accdb",
                        "DELETE * FROM SysTabl WHERE ParamName='ProjectManager'");
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConfigFillProjectManagerU");
            return ActionResult.Success;
        }


        //Предполагается возможность удалять патч 2007 вместе с ядром. Для этого в реестре снимается запрет с ручного удаления
        //На данный момент запрета нет, и патч удаляется аналогично компонентам.
        //Если делать все по-нормальному, то надо следить за GUID, иначе этот патч просто не удалить с компа без ручной правки реестра
        [CustomAction]
        public static ActionResult IT2007PackRegClean (Session session)
        {
            session.Log("Begin IT2007PackRegClean");
            try
            {
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{A2AA4EA5-0C66-444C-AC2D-5EB023DEC1E5}", true);
                myKey.SetValue("NoRemove", 0, RegistryValueKind.DWord);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End IT2007PackRegClean");
            return ActionResult.Success;
        }

        //Синхронизация БД
        [CustomAction]
        public static ActionResult AnalyzerVersionSynch (Session session)
        {
            session.Log("Begin AnalyzerVersionSynch");
            try
            {
                var vW = new DbVersion();
                var analyzerDir = session.GetTargetPath("INSTALLLOCATION") + @"Analyzer\";
                string er = vW.UpdateProjectVersion(analyzerDir + "ArhAnalyzerProject.accdb", true);
                er += "\n" + vW.UpdateProjectVersion(analyzerDir + "ArhAnalyzerTemplates.accdb", true);
                er += "\n" + vW.UpdateProjectVersion(analyzerDir + "VedData.accdb", true);
                er += "\n" +vW.UpdateAnalyzerAppDataVersion(analyzerDir + "AppData.accdb", true);
                er += vW.UpdateArchiveVersion(analyzerDir + "ArhAnalyzerArchive.accdb", true);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End AnalyzerVersionSynch");
            return ActionResult.Success;
        }

        //[CustomAction]
        //public static ActionResult AnalyzerInfoTaskVersionSynch(Session session)
        //{
        //    session.Log("Begin AnalyzerInfoTaskVersionSynch");
        //    try
        //    {
        //        var vW = new DbVersion();
        //        var analyzerITDir = session.GetTargetPath("INSTALLLOCATION") + @"Analyzer\";
        //        string er = vW.UpdateProjectVersion(analyzerITDir + "AnalyzerInfoTaskData.accdb", true);
        //        er += vW.UpdateArchiveVersion(analyzerITDir + "ObjSetTemplate.accdb", true);
        //    }
        //    catch (Exception exception)
        //    {
        //        exception.MessageError();
        //    }
        //    session.Log("End AnalyzerInfoTaskVersionSynch");
        //    return ActionResult.Success;
        //}

        [CustomAction]
        public static ActionResult RASVersionSynch(Session session)
        {
            session.Log("Begin RASVersionSynch");
            try
            {
                var vW = new DbVersion();
                var rasDir = session.GetTargetPath("INSTALLLOCATION") + @"RAS\";
                string er = vW.UpdateProjectVersion(rasDir + "RasAnalyzerProject.accdb", true);
                er += "\n" + vW.UpdateProjectVersion(rasDir + "RasTemplates.accdb", true);
                er += "\n" + vW.UpdateProjectVersion(rasDir + "RasProject.accdb", true);
                er += vW.UpdateArchiveVersion(rasDir + "RASArchive.accdb", true);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End RASVersionSynch");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ConstructorVersionSynch(Session session)
        {
            session.Log("Begin ConstructorVersionSynch");
            try
            {
                var vW = new DbVersion();
                var constructorDir = session.GetTargetPath("INSTALLLOCATION") + @"Constructor\";
                string er = vW.UpdateConstructorAppDataVersion(constructorDir + "AppData.accdb", true);
                er += vW.UpdateArchiveVersion(constructorDir + "CalcArchive.accdb", true);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ConstructorVersionSynch");
            return ActionResult.Success;
        }
        
        [CustomAction]
        public static ActionResult KernelVersionSynch(Session session)
        {
            session.Log("Begin KernelVersionSynch");
            try
            {
                var vW = new DbVersion();
                string er = vW.UpdateControllerDataVersion(session.GetTargetPath("INSTALLLOCATION") + "Controller\\ControllerData.accdb", true);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End KernelVersionSynch");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ReporterVersionSynch(Session session)
        {
            session.Log("Begin ReporterVersionSynch");
            try
            {
                var vW = new DbVersion();
                string er = vW.UpdateReporterDataVersion(session.GetTargetPath("INSTALLLOCATION") + "Reporter\\ReporterData.accdb", true);
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End ReporterVersionSynch");
            return ActionResult.Success;
        }

        //Для переустановки/удаления во время открытых приложений ИнфоТаск 
        [CustomAction]
        public static ActionResult KillProcess(Session session)
        {
            session.Log("Begin KillProcess");
            try
            {
                Process[] processAcc = Process.GetProcessesByName("MSACCESS");
                Process[] processExc = Process.GetProcessesByName("EXCEL");
                Process[] processCon = Process.GetProcessesByName("Controller");
                //foreach (Process pr in processAcc)
                //{
                //    pr.Kill();
                //}
                if (processAcc.Length > 0 || processExc.Length > 0 || processCon.Length > 0)
                {
                    MessageBox.Show("Прежде, чем удалить/переустановить данный компонент InfoTask, необходимо закрыть все приложения MS Access, MS Excel, а также Reporter", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ActionResult.UserExit;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + '\n' + exception.StackTrace);
            }
            session.Log("End KillProcess");
            return ActionResult.Success;
        }

        //Запись в реестр даты установки
        [CustomAction]
        public static ActionResult SetDate(Session session)
        {
            session.Log("Begin SetDate");
            try
            {
                DbVersion.SetDate();
            }
            catch (Exception exception)
            {
                exception.MessageError();
            }
            session.Log("End SetDate");
            return ActionResult.Success;
        }
    }
}
