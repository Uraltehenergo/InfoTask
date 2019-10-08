using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomBoolean : Mom
    {
        internal MomBoolean(DateTime time, ErrMom error) : base(time, error)
        {
        }

        public MomBoolean(bool boolean, ErrMom error = null) : base(Different.MinDate, error)
        {
            _boolean = boolean;
        }

        public MomBoolean(int integer, ErrMom error = null) : base(Different.MinDate, error)
        {
            Integer = integer;
        }

        public MomBoolean(double real, ErrMom error = null) : base(Different.MinDate, error)
        {
            Real = real;
        }

        public MomBoolean(string s, ErrMom error = null) : base(Different.MinDate, error)
        {
            String = s;
        }

        public override DataType DataType
        {
            get { return DataType.Boolean; }
        }

        //Значение
        private bool _boolean;

        public override bool Boolean
        {
            get { return _boolean; }
            internal set { _boolean = value; }
        }
        
        public override int Integer
        {
            get { return _boolean ? 1 : 0; }
            internal set { _boolean = value != 0; }
        }

        public override double Real
        {
            get { return _boolean ? 1 : 0; }
            internal set { _boolean = value != 0; }
        }

        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { _boolean = false; }
        }

        public override string String
        {
            get { return _boolean ? "1" : "0"; }
            internal set { _boolean = value != "0"; }
        }

        public override object Object
        {
            get { return _boolean; }
        }

        internal override void CopyValue(IMom mom)
        {
            Boolean = mom.Boolean;
        }

        internal override void MakeDefaultValue()
        {
            Boolean = false;
        }

        public override bool ValueEquals(Mom mom)
        {
            return mom.Boolean = Boolean;
        }

        public override bool ValueLess(Mom mom)
        {
            return !Boolean && mom.Boolean;
        }
    }
}