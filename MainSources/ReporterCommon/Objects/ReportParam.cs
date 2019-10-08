using System.Collections.Generic;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace ReporterCommon
{
    //Параметр отчета
    public class ReportParam
    {
        public ReportParam(string project, ArchiveParam ap = null)
        {
            ArchiveParam = ap;
            Project = project;
            IsUsed = false;
            IsHandInput = false;
        }
        
        //Загрузка из файла данных
        public ReportParam(IRecordRead rec)
        {
            ArchiveParam = new ArchiveParam(rec);
            Project = rec.GetString("Project");
            Id = rec.GetInt("ParamId");
            FromArchive = rec.GetBool("FromArchive");
            FromProject = rec.GetBool("FromProject");
        }

        //Загрузка свойств из файла данных
        public void PropsFromRecordset(IRecordRead rec)
        {
            IsUsed = rec.GetBool("IsUsed");
            IsHandInput = rec.GetBool("IsHandInput");
            IntervalTypes.Clear();
            if (rec.GetBool("IsSingle")) IntervalTypes.Add(IntervalType.Single);
            if (rec.GetBool("IsBase")) IntervalTypes.Add(IntervalType.Base);
            if (rec.GetBool("IsHour")) IntervalTypes.Add(IntervalType.Hour);
            if (rec.GetBool("IsDay")) IntervalTypes.Add(IntervalType.Day);
            if (rec.GetBool("IsCombined")) IntervalTypes.Add(IntervalType.Combined);
            if (rec.GetBool("IsAbsolute")) IntervalTypes.Add(IntervalType.Absolute);
            if (rec.GetBool("IsMoments")) IntervalTypes.Add(IntervalType.Moments);
            if (rec.GetBool("IsAbsoluteListBase")) IntervalTypes.Add(IntervalType.AbsoluteListBase);
            if (rec.GetBool("IsAbsoluteListHour")) IntervalTypes.Add(IntervalType.AbsoluteListHour);
            if (rec.GetBool("IsAbsoluteListDay")) IntervalTypes.Add(IntervalType.AbsoluteListDay);
            if (rec.GetBool("IsAbsoluteCombined")) IntervalTypes.Add(IntervalType.AbsoluteCombined);
        }

        //Сохранение свойств в файл данных
        public void PropsToRecordset(IRecordSet rec)
        {
            rec.Put("Project", Project);
            rec.Put("IsUsed", IsUsed);
            rec.Put("IsHandInput", IsHandInput);
            rec.Put("IsSingle", IntervalTypes.Contains(IntervalType.Single));
            rec.Put("IsBase", IntervalTypes.Contains(IntervalType.Base));
            rec.Put("IsHour", IntervalTypes.Contains(IntervalType.Hour));
            rec.Put("IsDay", IntervalTypes.Contains(IntervalType.Day));
            rec.Put("IsCombined", IntervalTypes.Contains(IntervalType.Combined));
            rec.Put("IsAbsolute", IntervalTypes.Contains(IntervalType.Absolute));
            rec.Put("IsMoments", IntervalTypes.Contains(IntervalType.Moments));
            rec.Put("IsAbsoluteListBase", IntervalTypes.Contains(IntervalType.AbsoluteListBase));
            rec.Put("IsAbsoluteListHour", IntervalTypes.Contains(IntervalType.AbsoluteListHour));
            rec.Put("IsAbsoluteListDay", IntervalTypes.Contains(IntervalType.AbsoluteListDay));
            rec.Put("IsAbsoluteCombined", IntervalTypes.Contains(IntervalType.AbsoluteCombined));
        }

        //Ссылка на архивный параметр
        public ArchiveParam ArchiveParam { get; set; }
        //FullCode из ArchiveParam
        public string FullCode { get { return ArchiveParam.FullCode; } }
        //Проект
        public string Project { get; private set; }
        //Id в таблице CalcParams файла данных отчета
        public int Id { get; set; }
        //True, если параметр есть в проекте
        public bool FromProject { get; set; }
        //True, если параметр есть в проекте архива
        public bool FromArchive { get; set; }
        //Параметр отмечен в форме установки ссылок
        public bool Otm { get; set; }

        //True, если параметр есть в проекте и используется в отчете
        public bool IsUsed { get; set; }
        //True, если используется ручной ввод
        public bool IsHandInput { get; set; }
        //Ячейка ручного ввода, если есть
        public ReportCell HandInputCell { get; set; }
        //Список ячеек, содержащих данный параметр
        private List<ReportCell> _cells;
        public List<ReportCell> Cells { get { return _cells ?? (_cells = new List<ReportCell>()); } }
        private List<ReportShape> _shapes;
        internal List<ReportShape> Shapes { get { return _shapes ?? (_shapes = new List<ReportShape>()); } }
        
        //Список типов интервалов, которые нужно снимать из архива (или Комбинированный)
        private readonly HashSet<IntervalType> _intervalTypes = new HashSet<IntervalType>();
        public HashSet<IntervalType> IntervalTypes { get { return _intervalTypes; } }

        //Получение значения поля field из параметра, null - если значение не определено
        public string GetField(LinkField field)
        {
             var fp = ArchiveParam.FirstParam;
            var lp = ArchiveParam.LastParam;
            switch (field)
            {
                case LinkField.Code:
                    return FullCode;
                case LinkField.CodeParam:
                    return fp.Code;
                case LinkField.CodeSubParam:
                    return ArchiveParam.CodeSubParam;
                case LinkField.Name:
                    return fp.Name;
                case LinkField.SubName:
                    if (lp != null) return lp.Name;
                    break;
                case LinkField.Comment:
                    return fp.Comment;
                case LinkField.SubComment:
                    if (lp != null) return lp.Comment;
                    return null;
                case LinkField.Units:
                    return ArchiveParam.Units;
                case LinkField.DataType:
                    return ArchiveParam.DataType.ToRussian();
                case LinkField.Task:
                    return fp.Task;
                case LinkField.CalcParamType:
                    return fp.CalcParamType.ToRussian();
                case LinkField.SuperProcessType:
                    return ArchiveParam.SuperProcess.ToRussian();
                case LinkField.Project:
                    return Project;
                case LinkField.Min:
                    return ArchiveParam.Min.ToString();
                case LinkField.Max:
                    return ArchiveParam.Max.ToString();
                case LinkField.Tag:
                    return ArchiveParam.FirstParam.Tag;
                case LinkField.DecPlaces:
                    return ArchiveParam.DecPlaces.ToString();
            }
            return null;
        }
    }
}
