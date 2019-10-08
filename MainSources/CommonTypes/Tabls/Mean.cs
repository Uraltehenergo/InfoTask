using System;
using BaseLibrary;

namespace CommonTypes
{
    //Одно значение
    public abstract class Mean
    {
        public abstract DataType DataType { get; }
        public virtual bool Bool { get { return false; } }
        public virtual int Integer { get { return 0; } }
        public virtual double Real { get { return 0; } }
        public virtual DateTime Date { get { return Different.MinDate; } }
        public virtual string String { get { return ""; } }

        public static Mean Create(DataType dtype, IRecordRead rec, string field)
        {
            switch (dtype)
            {
                case DataType.Real:
                    return new MeanReal(rec.GetDouble(field));
                case DataType.String:
                    return new MeanString(rec.GetString(field));
                case DataType.Integer:
                    return new MeanInteger(rec.GetInt(field));
                case DataType.Boolean:
                    return new MeanBool(rec.GetBool(field));
                case DataType.Time:
                    return new MeanTime(rec.GetTime(field));
            }
            return null;
        }

        //Операции сравнения
        public static bool operator ==(Mean x, Mean y)
        {
            var dt = x.DataType.Add(y.DataType);
            switch (dt)
            {
                case DataType.String:
                    return x.String == y.String;
                case DataType.Real:
                    return x.Real == y.Real;
                case DataType.Integer:
                    return x.Integer == y.Integer;
                case DataType.Boolean:
                    return x.Bool == y.Bool;
                case DataType.Time:
                    return x.Date == y.Date;
            }
            return false;
        }

        public static bool operator !=(Mean x, Mean y)
        {
            return !(x == y);
        }

        public static bool operator <(Mean x, Mean y)
        {
            var dt = x.DataType.Add(y.DataType);
            switch (dt)
            {
                case DataType.String:
                    return x.String.CompareTo(y.String) < 0;
                case DataType.Real:
                    return x.Real < y.Real;
                case DataType.Integer:
                    return x.Integer < y.Integer;
                case DataType.Boolean:
                    return !x.Bool && y.Bool;
                case DataType.Time:
                    return x.Date < y.Date;
            }
            return false;
        }

        public static bool operator <=(Mean x, Mean y)
        {
            return x < y || x == y;
        }

        public static bool operator >(Mean x, Mean y)
        {
            return !(x <= y);
        }

        public static bool operator >=(Mean x, Mean y)
        {
            return !(x < y);
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanBool : Mean
    {
        public MeanBool(bool b)
        {
            _bool = b;
        }

        private readonly bool _bool;

        public override DataType DataType { get { return DataType.Boolean; }}
        public override bool Bool { get { return _bool; } }
        public override int Integer { get { return _bool ? 1 : 0; } }
        public override double Real { get { return _bool ? 1 : 0; } }
        public override string String { get { return _bool ? "1" : "0"; } }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanInteger : Mean
    {
        public MeanInteger(int i)
        {
            _integer = i;
        }

        private readonly int _integer;

        public override DataType DataType { get { return DataType.Integer; } }
        public override bool Bool { get { return _integer == 0; } }
        public override int Integer { get { return _integer; } }
        public override double Real { get { return _integer; } }
        public override string String { get { return _integer.ToString(); } }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanReal : Mean
    {
        public MeanReal(double r)
        {
            _real = r;
        }

        private readonly double _real;

        public override DataType DataType { get { return DataType.Real; } }
        public override bool Bool { get { return _real == 0; } }
        public override int Integer { get { return Convert.ToInt32(_real); } }
        public override double Real { get { return _real; } }
        public override string String { get { return _real.ToString(); } }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanString : Mean
    {
        public MeanString(string s)
        {
            _string = s ?? "";
        }

        private readonly string _string;

        public override DataType DataType { get { return DataType.String; } }
        public override bool Bool { get { return _string == "0"; } }
        public override int Integer { get { return _string.ToInt(); } }
        public override double Real { get { return _string.ToDouble(); } }
        public override DateTime Date { get { return _string.ToDateTime(); } }
        public override string String { get { return _string; } }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanTime : Mean
    {
        public MeanTime(DateTime d)
        {
            _date = d;
        }

        private readonly DateTime _date;

        public override DataType DataType { get { return DataType.Time; } }
        public override DateTime Date { get { return _date; } }
        public override string String { get { return _date.ToString(); } }
    }
}