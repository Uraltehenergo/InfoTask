using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BaseLibrary;

namespace CommonTypes
{
    //Список мгновенных значений или сегментов
    public class MomList : CalcVal, IMomentsVal
    {
        public MomList(DataType dataType, ErrMom err = null) : base(err)
        {
            _dataType = dataType;
            _moments = new List<Mom>();
            Moments = new ReadOnlyCollection<Mom>(_moments);
        }

        //Тип значения
        public override ValueType ValueType { get {return ValueType.Moments;} }
        //Тип данных
        private readonly DataType _dataType;
        public override DataType DataType { get { return _dataType; } }

        //Список мгновенных значений
        private readonly List<Mom> _moments;
        public ReadOnlyCollection<Mom> Moments { get; private set; }

        //Возвращает последнее мгновенное значение, или null, если список пустой
        public Mom LastMoment
        {
            get
            {
                if (_moments.Count == 0) return null;
                return _moments[_moments.Count - 1];
            }
        }

        //Создание нового Mom и добавление его в MomList
        public Mom AddMomValue(DateTime time, ErrMom error = null)
        {
            return AddMoment(CreateMom(time, error));
        }

        public Mom AddMomFromBoolean(DateTime time, bool boolean, ErrMom error = null)
        {
            return AddMoment(CreateFromBoolean(time, boolean, error));
        }

        public Mom AddMomFromInteger(DateTime time, int integer, ErrMom error = null)
        {
            return AddMoment(CreateFromInteger(time, integer, error));
        }

        public Mom AddMomFromReal(DateTime time, double real, ErrMom error = null)
        {
            return AddMoment(CreateFromReal(time, real, error));
        }

        public Mom AddMomFromTime(DateTime time, DateTime date, ErrMom error = null)
        {
            return AddMoment(CreateFromTime(time, date, error));
        }

        public Mom AddMomFromString(DateTime time, string s, ErrMom error = null)
        {
            return AddMoment(CreateFromString(time, s, error));
        }

        //Создает клон значения и добавляет в список
        public Mom AddMomClone(IMom mom)
        {
            var m = CreateMom(mom.Time, mom.Error);
            m.CopyValue(mom);
            return AddMoment(m);
        }
        //Создает клон значения с указанием времени и добавляет в список
        public Mom AddMomClone(IMom mom, DateTime time)
        {
            var m = CreateMom(time, mom.Error);
            m.CopyValue(mom);
            return AddMoment(m);
        }

        //Создает MomEdit, в список не добавляет
        public MomEdit CreateMomEdit(ErrMom err = null)
        {
            return new MomEdit(DataType, Different.MinDate, err);
        }
        public MomEdit CreateMomEdit(DateTime time, ErrMom err = null)
        {
            return new MomEdit(DataType, time, err);
        }
        
        private Mom CreateMom(DateTime time, ErrMom error)
        {
            return Mom.Create(DataType, time, error);
        }

        //Создание мгновенных значений разных типов
        private Mom CreateFromBoolean(DateTime time, bool boolean, ErrMom error)
        {
            var mom = CreateMom(time, error);
            if (mom != null) mom.Boolean = boolean;
            return mom;
        }

        private Mom CreateFromInteger(DateTime time, int integer, ErrMom error)
        {
            var mom = CreateMom(time, error);
            if (mom != null) mom.Integer = integer;
            return mom;
        }

        private Mom CreateFromReal(DateTime time, double real, ErrMom error)
        {
            var mom = CreateMom(time, error);
            if (mom != null) mom.Real = real;
            return mom;
        }

        private Mom CreateFromTime(DateTime time, DateTime date, ErrMom error)
        {
            var mom = CreateMom(time, error);
            if (mom != null) mom.Date = date;
            return mom;
        }

        private Mom CreateFromString(DateTime time, string s, ErrMom error)
        {
            var mom = CreateMom(time, error);
            if (mom != null) mom.String = s;
            return mom;
        }
        
        //Добавляет мгновенное значение в список, сохраняет упорядоченность по времени, два момента списка могут иметь одинаковое время
        private Mom AddMoment(Mom mom)
        {
            if (mom == null) return null;
            if (_moments.Count == 0 || mom.Time >= _moments[Moments.Count - 1].Time)
                _moments.Add(mom);
            else
            {
                int i = _moments.Count - 1;
                while (i >= 0 && _moments[i].Time > mom.Time) i--;
                _moments.Insert(i + 1, mom);
            }
            return mom;
        }

        //Итоговая ошибка для записи в Result
        public ErrMom TotalError 
        { 
            get
            {
                ErrMom err = Error;
                foreach (var mom in Moments)
                    err = err.Add(mom.Error);
                return err;
            } 
        }

        //Интерполяция типа type значений list на время time по точке с номером n и следующим за ней, если n = -1, то значение в начале
        public IMom Interpolation(InterpolationType type, int n, DateTime time)
        {
            if (Moments.Count == 0) return new MomValue(time);
            if (n >= 0 && time == Moments[n].Time) 
                return Moments[n];
            if (type == InterpolationType.Constant || DataType == DataType.String || n < 0 || n >= Moments.Count - 1)
                return Moments[n < 0 ? 0 : n].Clone(time);

            var err = Moments[n].Error.Add(Moments[n + 1].Error);
            double t = time.Subtract(Moments[n].Time).TotalSeconds;
            double t0 = Moments[n + 1].Time.Subtract(Moments[n].Time).TotalSeconds;
            if (DataType.LessOrEquals(DataType.Real))
            {
                double x1 = Moments[n + 1].Real;
                double x0 = Moments[n].Real;
                double r = t0 == 0 || t == 0 ? x0 : x0 + t * (x1 - x0) / t0;
                return new MomEdit(DataType.Real, time, err) { Real = r } ;
            }
            DateTime x1D = Moments[n + 1].Date;
            DateTime x0D = Moments[n].Date;
            DateTime d = t0 == 0 || t == 0 ? x0D : x0D.AddSeconds(t * x1D.Subtract(x0D).TotalSeconds / t0);
            return new MomEdit(DataType.Time, time, err) { Date = d };
        }
    }
}