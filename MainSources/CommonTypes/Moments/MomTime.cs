using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomTime : Mom
    {
        internal MomTime(ErrMom error, DateTime time) : base(time, error)
        {
        }

        public MomTime(DateTime date, ErrMom error = null) : base(Different.MinDate, error)
        {
            _date = date;
        }

        public MomTime(string s, ErrMom error = null) : base(Different.MinDate, error)
        {
            String = s;
        }

        public override DataType DataType
        {
            get { return DataType.Time;}
        }

        //Значение
        private DateTime _date;

        public override bool Boolean
        {
            get { return false; }
            internal set { _date = Different.MinDate; }
        }

        public override int Integer
        {
            get { return 0; }
            internal set { _date = Different.MinDate; }
        }

        public override double Real
        {
            get { return 0; }
            internal set { _date = Different.MinDate; }
        }

        public override DateTime Date
        {
            get { return _date; }
            internal set { _date = value; }
        }

        public override string String
        {
            get { return _date.ToString(); }
            internal set { _date = value.ToDateTime(); }
        }

        public override object Object
        {
            get { return _date; }
        }

        internal override void CopyValue(IMom mom)
        {
            Date = mom.Date;
        }

        internal override void MakeDefaultValue()
        {
            Date = Different.MinDate;
        }

        public override bool ValueEquals(Mom mom)
        {
            return Date == mom.Date;
        }

        public override bool ValueLess(Mom mom)
        {
            return Date < mom.Date;
        }
    }
}