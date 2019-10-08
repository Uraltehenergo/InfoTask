using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Общие функции
    public static class General
    {
        //True, если уже был инициализирован
        private static bool _isInitialized;

        //Путь к каталогу InfoTask
        public static string InfoTaskDir { get; private set; }
        //Путь к каталогу General
        public static string GeneralDir { get; private set; }
        //База данных Config.accdb
        public static string ConfigFile { get; private set; }
        //База данных HistryTemplate.accdb
        public static string HistryTemplateFile { get; private set; }
        //Путь к каталогу Docs
        public static string DocsDir { get; private set; }
        //Путь к каталогу Controller
        public static string ControllerDir { get; private set; }
        //База данных ControllerData.accdb
        public static string ControllerFile { get; private set; }
        //Путь к каталогу Reporter
        public static string ReporterDir { get; private set; }
        //База данных ReporterData.accdb
        public static string ReporterFile { get; private set; }
        //База данных ReportTemplate.accdb
        public static string ReportTemplateFile { get; private set; }
        //Каталог Tmp
        public static string TmpDir { get; private set; }

        //Режим отладки, отображаются скрытые кнопки
        public static bool DebugMode { get; private set; }
        //Приостановка всех потоков, чтобы сделать срез памяти
        public static bool Paused { get; set; }

        public static void Initialize()
        {
            if (_isInitialized) return;
            try
            {
                InfoTaskDir = Different.GetInfoTaskDir();
                GeneralDir = InfoTaskDir + @"General\";
                TmpDir = InfoTaskDir + @"Tmp\";
                DocsDir = InfoTaskDir + @"Docs\";
                ControllerDir = InfoTaskDir + @"Controller\";
                ReporterDir = InfoTaskDir + @"Reporter\";
                ConfigFile = GeneralDir + "Config.accdb";
                HistryTemplateFile = GeneralDir + "HistoryTemplate.accdb";
                if (!DaoDb.Check(GeneralDir + "General.accdb", new[] {"Functions", "FunctionsOverloads"}))
                    Different.MessageError("Не допустимый General.accdb");
                if (!DaoDb.Check(ConfigFile, new[] { "SysTabl", "SysSubTabl" }))
                    Different.MessageError("Не допустимый Config.accdb");
                ControllerFile = ControllerDir + "ControllerData.accdb";
                if (!DaoDb.Check(ControllerFile, new[] { "Threads", "Projects", "Providers" }))
                    Different.MessageError("Не допустимый ControllerData.accdb");
                ReporterFile = ReporterDir + "ReporterData.accdb";
                ReportTemplateFile = ReporterDir + "ReportTemplate.accdb";
                DebugMode = SysTabl.ValueS(ControllerFile, "DebugMode") == "True";
                Oka.Register();
                ReadProvidersLists();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                ex.MessageError("Настроечный файл имеет недопустимый формат");
            }
        }

        //Словарь описаний всех используемых провайдеров
        private static readonly  DicS<ProviderConfig> _providerConfigs = new DicS<ProviderConfig>();
        public static DicS<ProviderConfig> ProviderConfigs { get { return _providerConfigs; }}
        
        //Получение списков кодов провайдеров по типам для выпадающих списков
        private static void ReadProvidersLists()
        {
            try
            {
                var prIds = new DicI<ProviderConfig>();
                using (var rec = new ReaderAdo(ConfigFile, "SELECT * FROM SysTabl WHERE (ParamType='Provider') And (ParamValue <>'Коммуникатор')"))
                {
                    while (rec.Read())
                    {
                        var code = rec.GetString("ParamName");
                        var pr = new ProviderConfig(rec.GetString("ParamValue").ToProviderType(), code);
                        pr.JointProviders.Add(code);
                        prIds.Add(rec.GetInt("ParamId"), pr);
                        ProviderConfigs.Add(pr.Code, pr);
                    }
                    foreach (var pr in prIds.Values)
                        if (pr.Type == ProviderType.Source)
                            ProviderConfigs["CloneSource"].JointProviders.Add(pr.Code);
                    using (var recp = new ReaderAdo(ConfigFile, "SELECT * FROM SysSubTabl WHERE (SubParamName='ProviderFile') Or (SubParamName='JointProviders')"))
                        while (recp.Read())
                        {
                            int id = recp.GetInt("ParamId");
                            if (prIds.ContainsKey(id))
                            {
                                string prop = recp.GetString("SubParamName");
                                var pval = recp.GetString("SubParamValue");
                                if (prop == "ProviderFile") 
                                    prIds[id].File = InfoTaskDir + (pval.StartsWith(@"\") ? pval.Substring(1) : pval);
                                if (prop == "JointProviders")
                                    prIds[id].JointProviders.AddRange(pval.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка загрузки установленных провайдеров. Не правильный файл Config.accdb");
            }
        }

        //Возвращает ссылку на объект IProvider по заданному типу, имени и сваойствам провайдера
        public static IProvider RunProvider(string code, string name, string inf, Logger logger, ProviderSetupType setupType = ProviderSetupType.Controller)
        {
            switch (code)
            {
                case "Imitator":
                    return new Imitator(name, inf, logger);
                case "CloneSource":
                    return new CloneSource(name, inf, logger, false);
                case "HandInputSource":
                    return new CloneSource(name, inf, logger, true);
                case "HandInputSqlSource":
                    return new HandInputSqlSource(name, inf, logger);
                case "ArchiveAccessSource":
                    return new ArchiveAccessSource(name, inf, logger);
                case "ArchiveSQLServerSource":
                    return new ArchiveSQLServerSource(name, inf, logger);
                case "AccessArchive":
                    return new AccessArchive(name, inf, setupType, logger);
                case "SQLServerArchive":
                    return new SQLServerArchive(name, inf, setupType, logger);
                default:
                    return ProviderConfigs[code].RunProvider(code, name, inf, logger);
            }
        }

        //Очереди для запросов к провайдерам, ключи - хэши экземпляров провайдеров, элементы очередей - номера потоков
        private static readonly Dictionary<string, Queue<int>> _providersQueues = new Dictionary<string, Queue<int>>();
        public static Dictionary<string, Queue<int>> ProvidersQueues { get { return _providersQueues; } }
        private static readonly object _providersQueriesLock = new object();
        public static object ProvidersQueriesLock { get { return _providersQueriesLock; } }
    }
}