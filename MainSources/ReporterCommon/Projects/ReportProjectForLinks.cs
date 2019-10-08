using BaseLibrary;
using CommonTypes;

namespace ReporterCommon
{
    //Один проект для добавления ссылок в файл данных и для формы установки ссылок
    public class ReportProjectForLinks : ReportProjectBase
    {
        //Загрузка из файла данных отчета (recd) 
        public ReportProjectForLinks(ReportBook book, IRecordRead rec)
            : base(book, rec)
        {
            IsUsed = false;
            IsHandInput = false;
        }

        //Запись свойств проекта 
        public void PropsToRecordset(RecDao rec)
        {
            rec.Put("IsUsed", IsUsed);
            rec.Put("IsHandInput", IsHandInput);
        }

        //Список параметров отчета, для обновления файла данных и формы установки ссылок
        private readonly DicS<ReportParam> _params = new DicS<ReportParam>();
        public DicS<ReportParam> Params { get { return _params; } }

        //Для фильтров 
        private SetS _tasks;
        public SetS Tasks { get { return _tasks ?? (_tasks = new SetS()); } }
        private SetS _dataTypes;
        public SetS DataTypes { get { return _dataTypes ?? (_dataTypes = new SetS()); } }
        private SetS _superProcesses;
        public SetS SuperProcesses { get { return _superProcesses ?? (_superProcesses = new SetS()); } }
        private SetS _units;
        public SetS Units { get { return _units ?? (_units = new SetS()); } }
        private SetS _calcParamTypes;
        public SetS CalcParamTypes { get { return _calcParamTypes ?? (_calcParamTypes = new SetS()); } }

        //Формирует выпадающие списки для фильтров по параметрам
        public void MakeFilters()
        {
            Tasks.Clear();
            Units.Clear();
            CalcParamTypes.Clear();
            DataTypes.Clear();
            SuperProcesses.Clear();
            foreach (var par in Params.Values)
            {
                var ap = par.ArchiveParam;
                var fp = ap.FirstParam;
                Tasks.Add(fp.Task ?? "");
                Units.Add(ap.Units ?? "");
                CalcParamTypes.Add(fp.CalcParamType.ToRussian() ?? "");
                SuperProcesses.Add(ap.SuperProcess.ToRussian() ?? "");
                DataTypes.Add(ap.DataType.ToRussian());
            }
            if (Book.Forms.ContainsKey(ReporterCommand.FilterParams))
                ((FormFiltersParams)Book.Forms[ReporterCommand.FilterParams]).MakeFiltersLists();
        }
    }
}
