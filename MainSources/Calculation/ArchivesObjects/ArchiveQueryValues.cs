using System.Collections.Generic;
using CommonTypes;

namespace Calculation
{
    //Значения для одного заказа на чтение из архива
    //Если интервал в результате получается один (Interval != null), то SingleValue - это его значение или значения
    //Если интервалов получается много, то i-е значение из SingleValue.Moments, соответствует интервалу Intervals[i]
    public class ArchiveQueryValues
    {
        //Значения
        private SingleValue _singleValue;
        public SingleValue SingleValue
        {
            get { return _singleValue ?? (_singleValue = new SingleValue(SingleType.List)); }
            set { _singleValue = value; }
        }
        //Один интервал
        public ArchiveInterval Interval { get; set; }
        //Много интервалов
        private List<ArchiveInterval> _intervals;
        public List<ArchiveInterval> Intervals { get { return _intervals ?? (_intervals = new List<ArchiveInterval>()); } }
    }
}