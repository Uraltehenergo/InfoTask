using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один параметр из архива для отчета по одному типу интервала
    public class ArchiveReportParam
    {
        public ArchiveReportParam(string code, string project, DataType dataType, SuperProcess superProcess, CalcParamType ctype, IEnumerable<IntervalType> intervals)
        {
            Code = code;
            Project = project;
            DataType = dataType;
            SuperProcess = superProcess;
            CalcParamType = ctype;
            foreach (var t in intervals)
                Queries.Add(t, new ArchiveQueryValues());
        }

        //Записывает себя в ReportParams, reportId - id родительской записи
        public void ToRecordset(IRecordSet rec, int reportId, bool addnew = false)
        {
            if (addnew) rec.AddNew();
            rec.Put("ReportId", reportId);
            rec.Put("ParamId", ParamId);
            rec.Put("Project", Project);
            rec.Put("Code", Code);
            rec.Put("DataType", DataType.ToRussian());
            rec.Put("SuperProcessType", SuperProcess.ToRussian());
            rec.Put("CalcParamType", CalcParamType.ToRussian());
            rec.Put("IsSingle", Queries.ContainsKey(IntervalType.Single));
            rec.Put("IsBase", Queries.ContainsKey(IntervalType.Base));
            rec.Put("IsHour", Queries.ContainsKey(IntervalType.Hour));
            rec.Put("IsDay", Queries.ContainsKey(IntervalType.Day));
            rec.Put("IsCombined", Queries.ContainsKey(IntervalType.Combined));
            rec.Put("IsAbsolute", Queries.ContainsKey(IntervalType.Absolute));
            rec.Put("IsAbsoluteDay", Queries.ContainsKey(IntervalType.AbsoluteDay));
            rec.Put("IsAbsoluteCombined", Queries.ContainsKey(IntervalType.AbsoluteCombined));
            rec.Put("IsAbsoluteListBase", Queries.ContainsKey(IntervalType.AbsoluteListBase));
            rec.Put("IsAbsoluteListHour", Queries.ContainsKey(IntervalType.AbsoluteListHour));
            rec.Put("IsAbsoluteListDay", Queries.ContainsKey(IntervalType.AbsoluteListDay));
            rec.Put("IsMoments", Queries.ContainsKey(IntervalType.Moments));
            if (addnew) rec.Update();
        }

        //Id параметра из архива
        public string Code { get; private set; }
        //Проект
        public string Project { get; private set; }
        //Id из Params архива
        public int ParamId { get; set; }
        //Тип данных
        public DataType DataType { get; private set; }
        //Тип накопления
        public SuperProcess SuperProcess { get; private set; }
        //Тип ввода
        public CalcParamType CalcParamType { get;  private set; }

        //Мгновенные значения, ключи - типы интервалов в списке заказа
        private readonly Dictionary<IntervalType, ArchiveQueryValues> _queries = new Dictionary<IntervalType, ArchiveQueryValues>();
        public Dictionary<IntervalType, ArchiveQueryValues> Queries { get { return _queries; } }
    }
}