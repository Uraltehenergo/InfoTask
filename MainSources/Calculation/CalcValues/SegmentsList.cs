using System.Collections.Generic;
using System.Collections.ObjectModel;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class SegmentsList : CalcVal, ISingleVal
    {
        public SegmentsList(ErrMom err = null) : base(err)
        {
            _segments = new List<Segment>();
            Segments = new ReadOnlyCollection<Segment>(_segments);
        }

        //Список сегментов
        private readonly List<Segment> _segments;
        public ReadOnlyCollection<Segment> Segments { get; private set; }

        //Добавляет сегмент, сохраняя упорядоченность
        public void AddSegment(Segment segment)
        {
            if (_segments.Count == 0 || _segments[_segments.Count - 1].Less(segment))
                _segments.Add(segment);
            else
            {
                int i = _segments.Count - 1;
                while (i >= 0 && segment.Less(_segments[i])) i--;
                _segments.Insert(i + 1, segment);
            }
        }
        
        public override DataType DataType { get {return DataType.Segments;} }
        public override ValueType ValueType { get {return ValueType.Segments;} }
        
        public ErrMom TotalError 
        { 
            get
            {
                if (_segments.Count == 0) return Error;
                ErrMom err = Error;
                foreach (var segment in _segments)
                    err = err.Add(segment.ErrMom);
                return err;
            } 
        }
    }
}