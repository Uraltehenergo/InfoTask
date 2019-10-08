using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Параметр функции Пред вместе со значениями
    public class Prev
    {
        public Prev(IRecordRead rec, Project project)
        {
            _project = project;
            Code = rec.GetString("FullCode");
            _calcParamArchive = _project.ArchiveParams[Code];
            var ap = _calcParamArchive.ArchiveParam;
            var listLast = new List<IntervalType>();
            IsPrevAbs = rec.GetBool("PrevAbs");
            IsLastBase = rec.GetBool("LastBase");
            IsLastHour = rec.GetBool("LastHour");
            IsLastDay = rec.GetBool("LastDay");
            IsManyBase = rec.GetBool("ManyBase");
            IsManyHour = rec.GetBool("ManyHour");
            IsManyDay = rec.GetBool("ManyDay");
            IsManyMoments = rec.GetBool("ManyMoments");

            if (IsLastBase) listLast.Add(IntervalType.Base);
            if (IsLastHour) listLast.Add(IntervalType.Hour);
            if (IsLastDay) listLast.Add(IntervalType.Day);
            if (listLast.Count > 0)
                LastReportParam = new ArchiveReportParam(ap.FullCode, project.Code, ap.DataType, ap.SuperProcess, ap.FirstParam.CalcParamType, listLast);
            var listMany = new List<IntervalType>();
            if (IsManyBase)
            {
                listMany.Add(IntervalType.Base);
                ManyBase = new List<Moment>();
            }
            if (IsManyHour)
            {
                listMany.Add(IntervalType.Hour);
                ManyHour = new List<Moment>();
            }
            if (IsManyDay)
            {
                listMany.Add(IntervalType.Day);
                ManyDay = new List<Moment>();
            }
            if (IsManyMoments)
            {
                listMany.Add(IntervalType.Moments);
                ManyMoments = new List<Moment>();
            }
            if (listMany.Count > 0)
                ManyReportParam = new ArchiveReportParam(ap.FullCode, project.Code, ap.DataType, ap.SuperProcess, ap.FirstParam.CalcParamType, listMany);
        }
        
        //Код 
        public string Code { get; private set; }
        //Ссылка на CalcParamArchive
        private readonly CalcParamArchive _calcParamArchive;
        //Ссылка на ArchiveReportParam для последних данных и данных за период
        public ArchiveReportParam LastReportParam { get; private set; }
        public ArchiveReportParam ManyReportParam { get; private set; }
        //Ссылка на Project
        private readonly Project _project;

        //Флаги использования значений
        public bool IsPrevAbs { get; private set; }
        public bool IsLastBase { get; private set; }
        public bool IsLastHour { get; private set; }
        public bool IsLastDay { get; private set; }
        public bool IsManyBase { get; private set; }
        public bool IsManyHour { get; private set; }
        public bool IsManyDay { get; private set; }
        public bool IsManyMoments { get; private set; }

        //Одиночные предыдущие значения
        public Moment LastBase { get; set; }
        public Moment LastHour { get; set; }
        public Moment LastDay { get; set; }
        //Предыдущие значения за период
        public List<Moment> ManyBase { get; private set; }
        public List<Moment> ManyHour { get; private set; }
        public List<Moment> ManyDay { get; private set; }
        public List<Moment> ManyMoments { get; private set; }

        //Чтение данных из архивного параметра
        public void FromArchiveParam()
        {
            if (LastReportParam != null)
            {
                foreach (var q in LastReportParam.Queries)
                {
                    switch (q.Key)
                    {
                        case IntervalType.Base:
                            LastBase = q.Value.SingleValue.LastMoment;
                            break;
                        case IntervalType.Hour:
                            LastHour = q.Value.SingleValue.LastMoment;
                            break;
                        case IntervalType.Day:
                            LastDay = q.Value.SingleValue.LastMoment;
                            break;
                    }
                }   
            }
            if (ManyReportParam != null)
            {
                foreach (var q in ManyReportParam.Queries)
                {
                    switch (q.Key)
                    {
                        case IntervalType.Base:
                            ManyBase = q.Value.SingleValue.Moments;
                            break;
                        case IntervalType.Hour:
                            ManyHour = q.Value.SingleValue.Moments;
                            break;
                        case IntervalType.Day:
                            ManyDay = q.Value.SingleValue.Moments;
                            break;
                        case IntervalType.Moments:
                            ManyMoments = q.Value.SingleValue.Moments;
                            break;
                    }
                }
            }
        }

        //Обновление предыдущих значений после расчета
        public void Accumulate()
        {
            var en = _project.ThreadCalc.PeriodEnd;
            var intervals = _calcParamArchive.ArchiveParam.Intervals;
            if (IsLastBase)
                LastBase = _project.BaseInterval == null ? null : intervals[_project.BaseInterval].Moment;
            if (IsLastHour && en.Minute == 0)
                LastHour = (_project.HourInterval == null || !intervals.ContainsKey(_project.HourInterval)) ? null : intervals[_project.HourInterval].Moment;
            if (IsLastDay && en.Hour == 0 && en.Minute == 0)
                LastDay = (_project.DayInterval == null || !intervals.ContainsKey(_project.DayInterval)) ? null : intervals[_project.DayInterval].Moment;
            if (IsManyBase && _project.BaseInterval != null)
                ManyBase = AddMoment(ManyBase, intervals[_project.BaseInterval].Moment, 1, 2);
            if (IsManyHour && en.Minute == 0 && _project.HourInterval != null && intervals.ContainsKey(_project.HourInterval))
                ManyHour = AddMoment(ManyHour, intervals[_project.HourInterval].Moment, 3, 6);
            if (IsManyDay && en.Minute == 0 && en.Hour == 0 && _project.DayInterval != null && intervals.ContainsKey(_project.DayInterval))
                ManyDay = AddMoment(ManyDay, intervals[_project.DayInterval].Moment, 33, 66);
            if (IsManyMoments && _project.MomentsInterval != null)
            {
                var irl = intervals[_project.MomentsInterval];
                if (irl.Moments != null)
                    foreach (var m in irl.Moments)
                        ManyMoments = AddMoment(ManyMoments, m, 1, 2);
                else if (irl.Moment != null)
                    ManyMoments = AddMoment(ManyMoments, irl.Moment, 1, 2);
            }
        }

        //Удаление старых значений из списка
        private List<Moment> AddMoment(List<Moment> list, Moment m, int daysMin, int daysMax)
        {
            if (list.Count == 0 || list[list.Count - 1].Time < m.Time) 
                list.Add(m);
            if (list.Count == 0 || list[list.Count - 1].Time.Subtract(list[0].Time).TotalDays < daysMax) 
                return list;
            return list.Where(t => m.Time.Subtract(t.Time).TotalDays <= daysMin + 1).ToList();
        }
    }
}