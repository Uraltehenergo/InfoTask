using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomReal : Mom
    {
        internal MomReal(DateTime time, ErrMom error) : base(time, error)
        {
        }

        public MomReal(double real, ErrMom error = null) : base(Different.MinDate, error)
        {
            _real = real;
        }

        public MomReal(bool boolean, ErrMom error = null) : base(Different.MinDate, error)
        {
            Boolean = boolean;
        }

        public MomReal(int integer, ErrMom error = null) : base(Different.MinDate, error)
        {
            Integer = integer;
        }

        public MomReal(string s, ErrMom error = null) : base(Different.MinDate, error)
        {
            String = s;
        }

        public override DataType DataType
        {
            get { return DataType.Real; }
        }

        //Значение
        private double _real;

        public override bool Boolean
        {
            get { return _real != 0; }
            internal set { _real = value ? 1 : 0; }
        }

        public override int Integer
        {
            get { return Convert.ToInt32(_real); }
            internal set { _real = value; }
        }

        public override double Real
        {
            get { return _real; }
            internal set { _real = value; }
        }

        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { _real = 0; }
        }

        public override string String
        {
            get { return _real.ToString(); }
            internal set { _real = value.ToDouble(0);}
        }

        public override object Object
        {
            get { return _real; }
        }

        internal override void CopyValue(IMom mom)
        {
            Real = mom.Real;
        }

        internal override void MakeDefaultValue()
        {
            Real = 0;
        }

        public override bool ValueEquals(Mom mom)
        {
            return Real == mom.Real;
        }

        public override bool ValueLess(Mom mom)
        {
            return Real < mom.Real;
        }
    }
}