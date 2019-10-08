using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomInteger : Mom
    {
        internal MomInteger(DateTime time, ErrMom error) : base(time, error)
        {
        }

        public MomInteger(int integer, ErrMom error = null) : base(Different.MinDate, error)
        {
            _integer = integer;
        }

        public MomInteger(bool boolean, ErrMom error = null) : base(Different.MinDate, error)
        {
            Boolean = boolean;
        }

        public MomInteger(double real, ErrMom error = null) : base(Different.MinDate, error)
        {
            Real = real;
        }

        public MomInteger(string s, ErrMom error = null) : base(Different.MinDate, error)
        {
            String = s;
        }

        public override DataType DataType
        {
            get { return DataType.Integer; }
        }

        //Значение
        private int _integer;

        public override bool Boolean
        {
            get { return _integer != 0; }
            internal set { _integer = value ? 1 : 0; }
        }
        
        public override int Integer
        {
            get { return _integer; }
            internal set { _integer = value; }
        }

        public override double Real
        {
            get { return _integer; }
            internal set { _integer = Convert.ToInt32(value); }
        }

        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { _integer = 0; }
        }

        public override string String
        {
            get { return _integer.ToString(); }
            internal set { _integer = value.ToInt(); }
        }

        public override object Object
        {
            get { return _integer; }
        }

        internal override void CopyValue(IMom mom)
        {
            Integer = mom.Integer;
        }

        internal override void MakeDefaultValue()
        {
            Integer = 0;
        }

        public override bool ValueEquals(Mom mom)
        {
            return Integer == mom.Integer;
        }

        public override bool ValueLess(Mom mom)
        {
            return Integer < mom.Integer;
        }
    }
}