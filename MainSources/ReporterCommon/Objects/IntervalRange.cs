using System;
using BaseLibrary;
using CommonTypes;

namespace ReporterCommon
{
    //Диапазан источника или типа интервалов по архиву
    public class IntervalRange : TimeInterval 
    {
        public IntervalRange(DateTime begin, DateTime end, string provider, IntervalType intervalType) : base(begin, end)
        {
            IntervalType = intervalType;
            ProviderName = provider;
        }

        public IntervalRange(TimeInterval interval, string provider, IntervalType intervalType) 
            : this(interval.Begin, interval.End, provider, intervalType)
        { }

        //Тип интервала, если задан, для диапазонов источников - Empty
        public IntervalType IntervalType { get; private set; }
        public string Interval { get { return IntervalType == IntervalType.Empty ? "" : IntervalType.ToRussian(); } }
        //Провайдер архива или источника
        public string ProviderName { get; private set; }
    }
}
