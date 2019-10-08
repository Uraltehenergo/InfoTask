using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomString : Mom
    {
        internal MomString(ErrMom error, DateTime time) : base(time, error)
        {
        }

        public MomString(string s, ErrMom error = null) : base(Different.MinDate, error)
        {
            _string = s;
        }

        public MomString(bool boolean, ErrMom error = null) : base(Different.MinDate, error)
        {
            Boolean = boolean;
        }

        public MomString(int integer, ErrMom error = null) : base(Different.MinDate, error)
        {
            Integer = integer;
        }

        public MomString(double real, ErrMom error = null) : base(Different.MinDate, error)
        {
            Real = real;
        }

        public MomString(DateTime date, ErrMom error = null) : base(Different.MinDate, error)
        {
            Date = date;
        }

        public override DataType DataType
        {
            get { return DataType.String;}
        }

        //Значение
        private string _string;

        public override bool Boolean
        {
            get { return _string != "0"; }
            internal set { _string = value ? "1" : "0"; }
        }

        public override int Integer
        {
            get { int i; return Int32.TryParse(_string, out i) ? i : 0; }
            internal set { _string = value.ToString(); }
        }

        public override double Real
        {
            get
            {
                double d = _string.ToDouble();
                if (Double.IsNaN(d)) d = 0;
                return d;
            }
            internal set { _string = value.ToString(); }
        }

        public override DateTime Date
        {
            get
            {
                DateTime t;
                return DateTime.TryParse(_string, out t) ? t : Different.MinDate;
            }
            internal set { _string = value.ToString(); }
        }

        public override string String
        {
            get { return _string; }
            internal set { _string = value; }
        }

        public override object Object
        {
            get { return _string; }
        }

        internal override void CopyValue(IMom mom)
        {
            String = mom.String;
        }

        internal override void MakeDefaultValue()
        {
            String = "";
        }

        public override bool ValueEquals(Mom mom)
        {
            return String == mom.String;
        }

        public override bool ValueLess(Mom mom)
        {
            if (String.IsEmpty() || mom.String.IsEmpty()) return false;
            return String.CompareTo(mom.String) < 0;
        }
    }
}