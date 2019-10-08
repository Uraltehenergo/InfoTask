using System;
using System.Collections.Generic;
using CommonTypes;

namespace Calculation
{
    //Данные архивного проекта или отчета для внутреннего пользования провайдера
    public class InnerArchiveProject
    {
        public InnerArchiveProject(ArchiveProject project)
        {
            Project = project;
        }

        public InnerArchiveProject(ArchiveReport report)
        {
            Report = report;
        }

        //Ссылка на архивный проект
        public ArchiveProject Project { get; private set; }
        //Ссылка на архивный отчет
        public ArchiveReport Report { get; private set; }

        //True, если текущий расчет - разовый
        public bool IsSingle { get; set; }
        //Начало и конец текущего обрабатываемого периода
        public DateTime PeriodBegin { get; set; }
        public DateTime PeriodEnd { get; set; }
        //Признаки наличия в проекте или в отчете числовых и строковых параметров по типам интервалов
        private readonly HashSet<IntervalType> _hasValues = new HashSet<IntervalType>();
        public HashSet<IntervalType> HasValues { get { return _hasValues; } }
        private readonly HashSet<IntervalType> _hasStrValues = new HashSet<IntervalType>();
        public HashSet<IntervalType> HasStrValues { get { return _hasStrValues; } }
        
        //Словарь архивных параметров проекта, ключи - Id
        private readonly Dictionary<int, ArchiveParam> _paramsId = new Dictionary<int, ArchiveParam>();
        public Dictionary<int, ArchiveParam> ParamsId { get { return _paramsId; }}
        //Словарь архивных параметров отчета, ключи - ParamId
        private readonly Dictionary<int, ArchiveReportParam> _reportParamsId = new Dictionary<int, ArchiveReportParam>();
        public Dictionary<int, ArchiveReportParam> ReportParamsId { get { return _reportParamsId; } }
    }
}