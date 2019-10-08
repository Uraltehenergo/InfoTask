using System;
using BaseLibrary;

namespace CommonTypes
{
    public class Moment
    {
        //Тип данных
        public DataType DataType { get; set; }
        //Время, включая милисекунды
        public DateTime Time { get; set; }

        //Значения разных типов
        public bool Boolean { get; set; }

        private int _integer;
        public int Integer
        {
            get 
            { 
                if (DataType == DataType.Integer) return _integer;
                if (DataType == DataType.Boolean) return Boolean ? 1 : 0;
                return _integer;
            }
            set
            {
                _integer = value;
                if (DataType == DataType.Boolean) DataType = DataType.Integer;
            }
        }

        private double _real;
        public double Real
        {
            get
            {
                if (DataType == DataType.Real) return _real;
                if (DataType == DataType.Boolean) return Boolean ? 1 : 0;
                if (DataType == DataType.Integer) return _integer;
                return _real;
            }
            set
            {
                _real = value;
                if (DataType.LessOrEquals(DataType.Real)) DataType=DataType.Real;
            }
        }

        public DateTime Date { get; set; }

        private string _string;
        public string String
        {
            get
            {
                if (DataType == DataType.String) return _string ?? "";
                if (DataType == DataType.Real) return _real.ToString();
                if (DataType == DataType.Integer) return _integer.ToString();
                if (DataType == DataType.Time) return Date.ToString();
                if (DataType == DataType.Boolean) return Boolean ? "1" : "0";
                return _string;
            } 
            set
            {
                _string = value;
                if (DataType != DataType.Error) DataType = DataType.String;
            }
        }

        //Ошибка
        public ErrorCalc Error { get; set; }
        //Недостоверность
        public int Nd { get; set; }

        //Возвращает значение в виде object
        public object Object
        {
            get
            {
                switch (DataType)
                {
                    case DataType.Boolean:
                        return Boolean;
                    case DataType.Integer:
                        return Integer;
                    case DataType.Real:
                        return Real;
                    case DataType.Time:
                        return Time;
                    case DataType.String:
                        return String;
                }
                return 0;
            }
        }

        public Moment(DateTime time, bool boolean, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Boolean;
            Time = time;
            Nd = nd;
            Error = error;
            Boolean = boolean;
        }

        public Moment(bool boolean, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Boolean;
            Boolean = boolean;
            Error = error;
            Nd = nd;
        }

        public Moment(DateTime time, int integer, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Integer;
            Time = time;
            Integer = integer;
            Error = error;
            Nd = nd;
        }

        public Moment(int integer, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Integer;
            Integer = integer;
            Error = error;
            Nd = nd;
        }

        public Moment(DateTime time, double real, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Real;
            Time = time;
            Real = real;
            Error = error;
            Nd = nd;
        }

        public Moment(double real, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Real;
            Real = real;
            Error = error;
            Nd = nd;
        }

        public Moment(DateTime time, DateTime date, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Time;
            Time = time;
            Date = date;
            Error = error;
            Nd = nd;
        }

        public Moment(DateTime date, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.Time;
            Date = date;
            Error = error;
            Nd = nd;
        }

        public Moment(DateTime time, string s, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.String;
            Time = time;
            String = s;
            Error = error;
            Nd = nd;
        }

        public Moment(string s, ErrorCalc error = null, int nd = 0)
        {
            DataType = DataType.String;
            String = s;
            Error = error;
            Nd = nd;
        }

        public Moment(DataType dataType, DateTime time, ErrorCalc error = null, int nd = 0)
        {
            DataType = dataType;
            Time = time;
            Error = error;
            Nd = nd;
        }

        public Moment(DataType dataType = DataType.Value, ErrorCalc error = null, int nd = 0)
        {
            DataType = dataType;
            Error = error;
            Nd = nd;
        }

        //Из строкового значения
        public Moment(DataType dataType, string s,  int nd = 0, ErrorCalc error = null)
        {
            DataType = dataType;
            Nd = nd;
            Error = error;
            switch (dataType)
            {
                case DataType.String:
                    String = s;
                    break;
                case DataType.Real:
                    Real = s.ToDouble();
                    if (Double.IsNaN(Real)) Real = 0;
                    break;
                case DataType.Time:
                    DateTime t;
                    Date = DateTime.TryParse(s, out t) ? t : Different.MinDate;
                    break;
                case DataType.Integer:
                    int i;
                    Integer = Int32.TryParse(s, out i) ? i : 0;
                    break;
                case DataType.Boolean:
                    Boolean = s== "1";
                    break;
            }
        }

        public Moment(DataType dataType, string s, DateTime time, int nd = 0, ErrorCalc error = null)
            : this(dataType, s, nd, error)
        {
            Time = time;
        }

        //Из действительного значения
        public Moment(DataType dataType, double d, int nd = 0, ErrorCalc error = null)
        {
            DataType = dataType;
            Nd = nd;
            Error = error;
            switch (dataType)
            {
                case DataType.String:
                    String = d.ToString();
                    break;
                case DataType.Real:
                    Real = d;
                    break;
                case DataType.Integer:
                    Integer = Convert.ToInt32(d);
                    break;
                case DataType.Boolean:
                    Boolean = d > 0;
                    break;
            }
        }

        public Moment(DataType dataType, double d, DateTime time, int nd = 0, ErrorCalc error = null)
            : this(dataType, d, nd, error)
        {
            Time = time;
        }

        //Из объекта
        public Moment(DataType dataType, object ob, int nd = 0, ErrorCalc error = null)
        {
            DataType = dataType;
            Nd = nd;
            Error = error;
            switch (dataType)
            {
                case DataType.Real:
                    Real = Convert.ToDouble(ob);
                    break;
                case DataType.Boolean:
                    Boolean = Convert.ToBoolean(ob);
                    break;
                case DataType.Integer:
                    Integer = Convert.ToInt32(ob);
                    break;
                case DataType.String:
                    String = Convert.ToString(ob);
                    break;
                case DataType.Time:
                    Date = Convert.ToDateTime(ob);
                    break;
            }
        }

        public Moment(DataType dataType, object ob, DateTime time, int nd = 0, ErrorCalc error = null)
            : this(dataType, ob, nd, error )
        {
            Time = time;
        }

        //Копирование с указанием нового времени
        public Moment Clone(DateTime t)
        {
            var m = new Moment(DataType, t, Error, Nd);
            switch (DataType)
            {
                case DataType.Boolean:
                    m.Boolean = Boolean;
                    break;
                case DataType.Integer:
                    m.Integer = Integer;
                    break;
                case DataType.Real:
                    m.Real = Real;
                    break;
                case DataType.Time:
                    m.Date = Date;
                    break;
                case DataType.String:
                    m.String = String;
                    break;
            }
            return m;
        }
        //Копирование, если erradr != null с добавлением нового параметра в цепочку ошибки
        public Moment Clone(string erradr = null)
        {
            var m = Clone(Time);
            if (m.Error != null && erradr != null) m.Error=new ErrorCalc(erradr, m.Error);
            return m;
        }
        //Копирование с измененением типа данных
        public Moment Clone(DataType type)
        {
            var m = new Moment(type, Time, Error, Nd);
            switch (type)
            {
                case DataType.Boolean:
                    m.Boolean = Boolean;
                    break;
                case DataType.Integer:
                    m.Integer = Integer;
                    break;
                case DataType.Real:
                    m.Real = Real;
                    break;
                case DataType.Time:
                    m.Date = Date;
                    break;
                case DataType.String:
                    m.String = String;
                    break;
            }
            return m;
        }

        //Принимает значение от другого Moment
        public void CopyValueFrom(Moment mv)
        {
            switch (DataType)
            {
                case DataType.Boolean:
                    Boolean = mv.Boolean;
                    break;
                case DataType.Integer:
                    Integer = mv.Integer;
                    break;
                case DataType.Time:
                    Date = mv.Date;
                    break;
                case DataType.Real:
                    Real = mv.Real;
                    break;
                case DataType.String:
                    String = mv.String;
                    break;
            }   
        }

        public static bool operator ==(Moment x, Moment y)
        {
            if (Equals(null, x) && Equals(null, y)) return true;
            if (Equals(null, x)) return false;
            if (Equals(null, y)) return false;

            switch (x.DataType.Add(y.DataType))
            {
                case DataType.Boolean:
                    return x.Boolean == y.Boolean;
                case DataType.Integer:
                    return x.Integer == y.Integer;
                case DataType.Time:
                    return x.Date == y.Date;
                case DataType.Real:
                    return x.Real == y.Real;
                case DataType.String:
                    return x.String == y.String;
            }
            return false;
        }

        public static bool operator <(Moment x, Moment y)
        {
            if (Equals(x, null) || Equals(y, null)) return false;

            switch (x.DataType.Add(y.DataType))
            {
                case DataType.Boolean:
                    return !x.Boolean && y.Boolean;
                case DataType.Integer:
                    return x.Integer < y.Integer;
                case DataType.Time:
                    return x.Date < y.Date;
                case DataType.Real:
                    return x.Real < y.Real;
                case DataType.String:
                    if (x.String.IsEmpty() || y.String.IsEmpty()) return false;
                    return x.String.CompareTo(y.String) < 0;
            }
            return false;
        }

        public static bool operator !=(Moment x, Moment y)
        {
            return !(x == y);
        }

        public static bool operator >=(Moment x, Moment y)
        {
            return !(x < y);
        }

        public static bool operator <=(Moment x, Moment y)
        {
            return (y >= x);
        }

        public static bool operator >(Moment x, Moment y)
        {
            return (y < x);
        }

        public bool Equals(Moment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Moment)) return false;
            return Equals((Moment)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = DataType.GetHashCode();
                result = (result * 397) ^ Time.GetHashCode();
                result = (result * 397) ^ Boolean.GetHashCode();
                result = (result * 397) ^ Integer;
                result = (result * 397) ^ Date.GetHashCode();
                result = (result * 397) ^ Real.GetHashCode();
                result = (result * 397) ^ String.GetHashCode();
                result = (result * 397) ^ Nd;
                result = (result * 397) ^ Error.GetHashCode();
                return result;
            }
        }
    }
}