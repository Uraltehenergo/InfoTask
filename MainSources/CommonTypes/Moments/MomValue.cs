using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomValue : Mom
    {
        public MomValue(DateTime time, ErrMom error = null) : base(time, error)
        {
        }

        public override DataType DataType
        {
            get { return DataType.Value;}
        }

        public override bool Boolean
        {
            get { return false; }
            internal set {}
        }

        public override int Integer
        {
            get { return 0; }
            internal set {}
        }

        public override double Real
        {
            get { return 0; }
            internal set {}
        }

        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { }
        }

        public override string String
        {
            get { return ""; }
            internal set { }
        }

        public override object Object
        {
            get { return 0; }
        }

        internal override void CopyValue(IMom mom)
        {
        }

        internal override void MakeDefaultValue()
        {
        }

        public override bool ValueEquals(Mom mom)
        {
            return true;
        }

        public override bool ValueLess(Mom mom)
        {
            return false;
        }
    }
}