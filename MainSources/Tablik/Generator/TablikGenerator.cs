using System;
using BaseLibrary;
using CommonTypes;

namespace Tablik.Generator
{
    //Генерация проекта из таблиц
    public class TablikGenerator : Logger
    {
        public TablikGenerator()
        {
            _infoTaskDir = Different.GetInfoTaskDir();
            OpenHistory(_infoTaskDir + @"Constructor\TablikHistory\GeneratorHistory.accdb",
                        _infoTaskDir + @"General\HistoryTemplate.accdb");
            using (StartLog("Открытие"))
                _isClosed = false;
        }

        //Путь к каталогу InfoTask 
        private readonly string _infoTaskDir;

        //Генерация файла проекта
        public string Generate(string templateFile, //Файл шаблона генерации
                               string tablsFile, //Файл с таблицами данных
                               string projectFile) //Генерируемый файл проекта
        {
            using (var c = Start())
            {
                try
                {
                    if (_isClosed) AddError("Генератор уже был закрыт");
                    else
                        using (StartLog("Генерация проекта", projectFile))
                        {
                            _templateFile = templateFile;
                            _tablsFile = tablsFile;
                            _projectFile = projectFile;
                            Start(ClearProject, 0, 10);
                            Start(LoadTables, 10, 25);
                            Start(LoadParams, 25, 30);
                            Start(Generation, 30);
                        }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при генерации проекта", ex);
                }
                return c.ErrorMessage();
            }
        }

        //Файлы шаблонов, таблиц и проекта
        private string _templateFile;
        private string _tablsFile;
        private string _projectFile;

        //Очистка таблиц CalcParams и т.д.
        private void ClearProject()
        {
            AddEvent("Очистка таблиц параметров проекта");
            using (var pdb = new DaoDb(_projectFile))
            {
                pdb.Execute("DELETE * FROM CalcSubParams");
                pdb.Execute("DELETE * FROM CalcParams");
            }
            DaoDb.Compress(_templateFile, 300000000, _infoTaskDir + @"Tmp\");
        }

        //Список таблиц
        internal TablsList TablsList { get; private set; }
        
        //Загрузка таблиц
        private void LoadTables()
        {
            AddEvent("Загрузка данных исходных таблиц");
            using (var db = new DaoDb(_tablsFile))
            {
                db.ConnectDao();
                TablsList = new TablsList(db);
                TablsList.LoadValues();
            }
        }

        //Словарь генерирующих параметров, ключи - Id
        private readonly DicI<GenParam> _params = new DicI<GenParam>();
        internal DicI<GenParam> Params { get { return _params; } }

        //Загрузка параметров из шаблона
        private void LoadParams()
        {
            AddEvent("Загрузка генерирующих параметров");
            var parsingCondition = new ParsingCondition();
            var parsingGenerator = new ParsingGenerator();
            using (var rec = new RecDao(_templateFile, "SELECT * FROM CalcParams WHERE GenOn=True"))
            using (var recs = new RecDao(rec.DaoDb, "SELECT CalcSubParams.* FROM CalcSubParams INNER JOIN CalcParams ON CalcParams.CalcParamId = CalcSubParams.OwnerId WHERE (CalcParams.GenOn = True) AND (CalcSubParams.GenOn = True)"))
                {
                    while (rec.Read())
                    {
                        var par = new GenParam(rec, parsingCondition, parsingGenerator);
                        Params.Add(par.CalcParamId, par);
                    }
                    while (recs.Read())
                    {
                        var par = new GenSubParam(recs, parsingCondition, parsingGenerator);
                        var owner = Params[par.OwnerId];
                        owner.SubParams.Add(par);
                        par.Owner = owner;
                    }
                }
        }

        //Генерация парамтров
        private void Generation()
        {
            AddEvent("Генерация параметров");
            using (var rec = new RecDao(_projectFile, "CalcParams"))
                using (var recs = new RecDao(rec.DaoDb, "CalcSubParams"))
                    foreach (var par in Params.Values)
                        par.GenerateParams(TablsList, rec, recs);
        }

        //Закрытие
        public string Close()
        {
            try
            {
                using (var c = Start())
                {
                    if (_isClosed) AddError("Генератор уже был закрыт");
                    else
                    {
                        using (StartLog("Закрытие")) { }
                        UpdateHistory(false);
                        CloseHistory();
                        _isClosed = true;
                    }
                    return c.ErrorMessage();
                }
            }
            finally { GC.Collect(); }
        }
        //Был ранее закрыт
        private bool _isClosed;

        //Реализация абстрактных методов
        protected override void FinishLogCommand() {}
        protected override void FinishSubLogCommand() { }
        protected override void FinishProgressCommand() { }
        protected override void MessageError(ErrorCommand er) { }
        protected override void ViewProcent(double procent) { }
    }
}