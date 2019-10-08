using System;
using System.Collections.Generic;
using BaseLibrary;

namespace Calculation
{
    //Класс для сохранения результатов в ведомость анализатора
    public class VedSaver
    {
        public VedSaver(Project project, string vedFile, string task = "")
        {
            _project = project;
            _vedFile = vedFile;
            _task = task;
            _allTasks = _task.IsEmpty();
            AddEvent("Загрузка списка колонок и параметров");
            ReadColumnsList();
            ReadParams();
        }

        //Проект 
        private readonly Project _project;
        //Имя, описание и тэг задачи набора
        private readonly string _task;
        private string _taskDescription;
        private string _taskTag;
        //Формировать ведомость по всем задачам
        private readonly bool _allTasks;
        //Файл ведомости
        private readonly string _vedFile;

        //Словарь колонок таблицы параметров
        private readonly DicS<VedColumn> _columnsParams = new DicS<VedColumn>();
        internal DicS<VedColumn> ColumnsParams { get { return _columnsParams; }}
        //Словарь колонок таблицы значений
        private readonly DicS<VedColumn> _columnsVed = new DicS<VedColumn>();
        internal DicS<VedColumn> ColumnsVed { get { return _columnsVed; }}
        //Список всех колонок
        private readonly List<VedColumn> _columns = new List<VedColumn>();
        //Словарь параметров
        private readonly DicS<VedParam> _params = new DicS<VedParam>(); 
        
        //Чтение списка колонок
        private void ReadColumnsList()
        {
            try
            {
                _columnsParams.Clear();
                _columnsVed.Clear();
                using (var rec = new RecDao(_project.File, "SELECT * FROM VedColumns " + (_allTasks ? "" : ("WHERE Task='" + _task + "'")) +" ORDER BY ColumnNum"))
                {
                    while (rec.Read())
                    {
                        var col = new VedColumn(rec);
                        _columns.Add(col);
                        if (col.SourceType == ColumnSourceType.ParamChar ||
                            col.SourceType == ColumnSourceType.SignalChar ||
                            col.SourceType == ColumnSourceType.ResultValue)
                            _columnsParams.Add(col.Code, col);
                        if ((col.SourceType == ColumnSourceType.ParamValue ||
                             col.SourceType == ColumnSourceType.SubValue) &&
                            (col.LinVedView != VedView.None || col.GroupVedView != VedView.None))
                            _columnsVed.Add(col.Code, col);
                    }
                    if (!_task.IsEmpty())
                        using (var rect = new RecDao(rec.DaoDb, "SELECT Tasks.TaskDescription, Tasks.TaskTag FROM Tasks WHERE Task='" +_task + "'"))
                            if (rect.HasRows())
                            {
                                _taskDescription = rect.GetString("TaskDescription");
                                _taskTag = rect.GetString("TaskTag");
                            }
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка загрузки списка колонок", ex, "", _project.Code);
            }
        }

        //Чтение списка параметров
        private void ReadParams()
        {
            try
            {
                foreach (var par in _project.ArchiveParams.Values)
                {
                    if (_allTasks || par.FirstParam.Task == _task)
                    {
                        var fcode = par.ArchiveParam.FullCode;
                        if (!fcode.Contains("."))
                        {
                            if (!_params.ContainsKey(fcode))
                                _params.Add(fcode, new VedParam(par, this));
                        }
                        else
                        {
                            string code = fcode.Substring(0, fcode.LastIndexOf('.'));
                            if (!_params.ContainsKey(code) && _project.ArchiveParams.ContainsKey(code))
                                _params.Add(code, new VedParam(_project.ArchiveParams[code], this));
                            var cv = par.RunParam.CalcValue;
                            if (cv != null && cv.SingleValue != null && _params.ContainsKey(code))
                                _params[code].SubParams.Add(par.LastParam.Code, cv.SingleValue);
                        }
                    }
                }    
            }
            catch (Exception ex)
            {
                AddError("Ошибка загрузки списка параметров", ex, "", _project.Code);
            }
        }

        //Сохранение в ведомость
        public void SaveVed()
        {
            try
            {
                DaoDb.FromTemplate(General.GeneralDir + @"\Templates\AnalyzerVedTemplate.accdb", _vedFile, ReplaceByTemplate.Always);
                using (_vedDb = new DaoDb(_vedFile))
                {
                    WriteSysTabl();
                    MakeColumnsAndFilters();
                    _project.ThreadCalc.Procent = 10;
                    WriteParams();
                    _project.ThreadCalc.Start(WriteLinVed, 20);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при записи в ведомость", ex, "", _project.Code);
            }
        }

        //База данных ведомости
        private DaoDb _vedDb;

        //Запись в SysTabl ведомости
        private void WriteSysTabl()
        {
            using (var sys = new SysTabl(_vedDb))
            {
                sys.PutSubValue("ProjectOptions", "Project", _project.Code);
                sys.PutSubValue("ProjectOptions", "ProjectName", _project.Name);
                sys.PutSubValue("ProjectOptions", "ProjectDescription", _project.Description);
                sys.PutSubValue("ProjectOptions", "ProjectFile", _project.File);
                sys.PutSubValue("TaskOptions", "Task", _task);
                sys.PutSubValue("TaskOptions", "TaskDescription", _taskDescription);

                sys.PutSubValue("VedOptions", "VedName", _task.IsEmpty() ? _project.Name : _task);
                sys.PutSubValue("VedOptions", "VedDescription", _task.IsEmpty() ? _project.Description : _taskDescription);
                sys.PutSubValue("VedOptions", "PeriodBegin", _project.ThreadCalc.PeriodBegin.ToString());
                sys.PutSubValue("VedOptions", "PeriodEnd", _project.ThreadCalc.PeriodEnd.ToString());
                sys.PutSubValue("VedOptions", "CreationTime", DateTime.Now.ToString());

                string tag = _project.VedTag;
                if (!tag.IsEmpty() && !tag.EndsWith(";")) tag += ";";
                sys.PutTag("VedTag", tag + _taskTag);
            }
        }

        //Запись списка колонок и формирование колонок таблиц Params и VedLin
        private void MakeColumnsAndFilters()
        {
            AddEvent("Формирование колонок ведомости");
            using (var rec = new RecDao(_vedDb, "VedColumns"))
                foreach (var column in _columns)
                    column.ToRecordset(rec);
            _vedDb.Execute("INSERT INTO VedColumnsFormatConditions SELECT VedColumnsFormatConditions.* FROM [" + _project.File + "].VedColumnsFormatConditions " +
                           "INNER JOIN [" + _project.File + "].VedColumns ON VedColumnsFormatConditions.ParentId = VedColumns.ColumnId " +
                           "WHERE VedColumns.Task='" + _task + "' ORDER BY ColumnNum");
            _vedDb.Execute("INSERT INTO VedFilters ( Otm, FilterId, FilterNum, FilterType, FilterName, FilterValue, FilterDescription, FilterDefault ) " +
                                     "SELECT VedFilters.Otm, VedFilters.FilterId, VedFilters.FilterNum, VedFilters.FilterType, VedFilters.FilterName, VedFilters.FilterValue, VedFilters.FilterDescription, VedFilters.FilterDefault " +
                                     "FROM [" + _project.File + "].VedFilters" + (_allTasks ? "" : " WHERE VedFilters.Task='" + _task + "';"));
            _vedDb.Execute("INSERT INTO VedFiltersObjects SELECT VedFiltersObjects.* " +
                                     "FROM [" + _project.File + "].VedFilters INNER JOIN [" + _project.File + "].VedFiltersObjects ON VedFilters.FilterId = VedFiltersObjects.ParentId" +
                                     (_allTasks ? "" : " WHERE VedFilters.Task='" + _task + "';"));
            foreach (var col in _columnsVed.Values)
                _vedDb.SetColumn("VedLin", col.Code, col.DataType);
            foreach (var col in _columnsParams.Values)
            {
                _vedDb.SetColumn("Params", col.Code, col.DataType);
                //if (col.LinVedView != VedView.None)
                //    _vedDb.SetColumn("VedLin", col.Code, col.DataType);
            }
        }

        //Запись в таблицу Params
        private void WriteParams()
        {
            AddEvent("Запись списка параметров");
            using (var recp = new RecDao(_vedDb, "Params"))
                using (var recs = new RecDao(_vedDb, "SubParams"))
                    foreach (var par in _params.Values)
                        par.ToParamsTables(recp, recs, _columnsParams.Values);
        }

        //Запись в таблицу LinVed
        private void WriteLinVed()
        {
            AddEvent("Запись мгновенных значений");
            using (var rec = new RecDao(_vedDb, "VedLin"))
                foreach (var par in _params.Values)
                {
                    par.ToLinVed(rec, _columnsVed);
                    _project.ThreadCalc.Procent += 100.0 / _params.Count;
                }
        }

        //Переопределение методов из Logger
        private void AddEvent(string description, string pars = "")
        {
            _project.ThreadCalc.AddEvent(description, pars);
        }
        private void AddError(string text, Exception ex = null, string par = "", string context = "", bool isFatal = true)
        {
            _project.ThreadCalc.AddError(text, ex, par, context, isFatal);
        }
    }
}