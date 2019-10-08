using System;
using BaseLibrary;

namespace GraphicLibrary
{
    //Одно мгновенное значение
    public class MomentValue
    {
        public MomentValue()
        {
            DataType = DataType.Value;
            Nd = 0;
        }

        public MomentValue(int vNd)
        {
            DataType = DataType.Value;
            Nd = vNd;
        }

        public MomentValue(DateTime vTime, int vNd = 0)
        {
            DataType = DataType.Value;
            Time = vTime;
            Nd = vNd;
        }

        public MomentValue(MomentValue val)
        {
            DataType = DataType.Value;
            Time = val.Time;
            Nd = val.Nd;
        }

        //Тип данных
        public DataType DataType { get; set; }
        //Время, включая милисекунды
        public DateTime Time { get; set; }
        //Недостоверность
        public int Nd { get; set; }

        //Преобразовние величины в MomentBoolean
        public MomentBoolean ToMomentBoolean()
        {
            if (DataType == DataType.Boolean) return ((MomentBoolean) this);
            var res = new MomentBoolean(Time, false, Nd = 0);
            switch (DataType)
            {
                case DataType.Integer:
                    res.Mean = ((MomentInteger) this).Mean != 0;
                    break;
                case DataType.Real:
                    res.Mean = ((MomentReal) this).Mean != 0;
                    break;
            }
            return res;
        }

        //Преобразовние величины в MomentInteger
        public MomentInteger ToMomentInteger()
        {
            if (DataType == DataType.Integer) return ((MomentInteger) this);
            var res = new MomentInteger(Time, 0, Nd = 0);
            switch (DataType)
            {
                case DataType.Boolean:
                    res.Mean = ((MomentBoolean) this).Mean ? 1 : 0;
                    break;
                case DataType.Real:
                    try
                    {
                        res.Mean = Convert.ToInt32(((MomentReal) this).Mean);
                    }
                    catch
                    {
                    }
                    break;
            }
            return res;
        }

        //Преобразовние величины в MomentReal
        public MomentReal ToMomentReal()
        {
            if (DataType == DataType.Real) return ((MomentReal) this);
            var res = new MomentReal(Time, 0, Nd = 0);
            switch (DataType)
            {
                case DataType.Boolean:
                    res.Mean = ((MomentBoolean) this).Mean ? 1 : 0;
                    break;
                case DataType.Integer:
                    res.Mean = ((MomentInteger) this).Mean;
                    break;
            }
            return res;
        }

        //Преобразует значение Mean величины в строку 
        public string ToValueString()
        {
            switch (DataType)
            {
                case DataType.Boolean:
                    return ((MomentBoolean) this).Mean ? "1" : "0";
                case DataType.Integer:
                    return Convert.ToString(((MomentInteger) this).Mean);
                case DataType.Real:
                    return Convert.ToString(((MomentReal) this).Mean);
            }
            return "";
        }

        public static bool operator ==(MomentValue x, MomentValue y)
        {
            if (Equals(null, x) && Equals(null, y)) return true;
            if (Equals(null, x)) return false;
            if (Equals(null, y)) return false;

            switch (x.DataType)
            {
                case DataType.Boolean:
                    bool b = ((MomentBoolean) x).Mean;
                    switch (y.DataType)
                    {
                        case DataType.Boolean:
                            return b == ((MomentBoolean) y).Mean;
                        case DataType.Integer:
                            return (b ? 1 : 0) == ((MomentInteger) y).Mean;
                        case DataType.Real:
                            return (b ? 1 : 0) == ((MomentReal) y).Mean;
                    }
                    break;

                case DataType.Integer:
                    int i = ((MomentInteger) x).Mean;
                    switch (y.DataType)
                    {
                        case DataType.Boolean:
                            return i == (((MomentBoolean) y).Mean ? 1 : 0);
                        case DataType.Integer:
                            return i == ((MomentInteger) y).Mean;
                        case DataType.Real:
                            return i == ((MomentReal) y).Mean;
                    }
                    break;

                case DataType.Real:
                    double r = ((MomentReal) x).Mean;
                    switch (y.DataType)
                    {
                        case DataType.Boolean:
                            return r == (((MomentBoolean) y).Mean ? 1 : 0);
                        case DataType.Integer:
                            return r == ((MomentInteger) y).Mean;
                        case DataType.Real:
                            return r == ((MomentReal) y).Mean;
                    }
                    break;
            }
            return false;
        }

        public static bool operator <(MomentValue x, MomentValue y)
        {
            if (Equals(x, null) || Equals(y, null)) return false;

            switch (x.DataType)
            {
                case DataType.Boolean:
                    bool b = ((MomentBoolean) x).Mean;
                    switch (y.DataType)
                    {
                        case DataType.Boolean:
                            return !b && ((MomentBoolean) y).Mean;
                        case DataType.Integer:
                            return (b ? 1 : 0) < ((MomentInteger) y).Mean;
                        case DataType.Real:
                            return (b ? 1 : 0) < ((MomentReal) y).Mean;
                    }
                    break;

                case DataType.Integer:
                    int i = ((MomentInteger) x).Mean;
                    switch (y.DataType)
                    {
                        case DataType.Boolean:
                            return i < (((MomentBoolean) y).Mean ? 1 : 0);
                        case DataType.Integer:
                            return i < ((MomentInteger) y).Mean;
                        case DataType.Real:
                            return i < ((MomentReal) y).Mean;
                    }
                    break;

                case DataType.Real:
                    double r = ((MomentReal) x).Mean;
                    switch (y.DataType)
                    {
                        case DataType.Boolean:
                            return r < (((MomentBoolean) y).Mean ? 1 : 0);
                        case DataType.Integer:
                            return r < ((MomentInteger) y).Mean;
                        case DataType.Real:
                            return r < ((MomentReal) y).Mean;
                    }
                    break;
            }
            return false;
        }

        public static bool operator !=(MomentValue x, MomentValue y)
        {
            return !(x == y);
        }

        public static bool operator >=(MomentValue x, MomentValue y)
        {
            return !(x < y);
        }

        public static bool operator <=(MomentValue x, MomentValue y)
        {
            return (y >= x);
        }

        public static bool operator >(MomentValue x, MomentValue y)
        {
            return (y < x);
        }

        public bool Equals(MomentValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MomentValue)) return false;
            return Equals((MomentValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = DataType.GetHashCode();
                result = (result * 397) ^ Time.GetHashCode();
                result = (result * 397) ^ Nd;
                return result;
            }
        }
    }

    public class MomentReal : MomentValue
    {
        //Действительное мгновенное значение
        public double Mean { get; set; }

        public MomentReal()
        {
            DataType = DataType.Real;
            Mean = 0;
        }

        public MomentReal(double vMean, int vNd)
            : base(vNd)
        {
            DataType = DataType.Real;
            Mean = vMean;
        }

        public MomentReal(DateTime vTime, double vMean, int vNd)
            : base(vTime, vNd)
        {
            DataType = DataType.Real;
            Mean = vMean;
        }
    }

    public class MomentInteger : MomentValue
    {
        //Целое мгновенное значение
        public int Mean { get; set; }

        public MomentInteger()
        {
            DataType = DataType.Integer;
            Mean = 0;
        }

        public MomentInteger(int vMean, int vNd)
            : base(vNd)
        {
            DataType = DataType.Integer;
            Mean = vMean;
        }

        public MomentInteger(DateTime vTime, int vMean, int vNd)
            : base(vTime, vNd)
        {
            DataType = DataType.Integer;
            Mean = vMean;
        }
    }

    public class MomentBoolean : MomentValue
    {
        //Булевое мгновенное значение
        public bool Mean { get; set; }

        public MomentBoolean()
        {
            DataType = DataType.Boolean;
            Mean = false;
        }

        public MomentBoolean(bool vMean, int vNd)
            : base(vNd)
        {
            DataType = DataType.Boolean;
            Mean = vMean;
        }

        public MomentBoolean(DateTime vTime, bool vMean, int vNd)
            : base(vTime, vNd)
        {
            DataType = DataType.Boolean;
            Mean = vMean;
        }
    }
}