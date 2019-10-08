using System;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый клас для мгновенных значений
    public abstract class Mom : CalcVal, IMom
    {
        protected Mom(DateTime time, ErrMom error) : base(error)
        {
            Time = time;
            Error = error;
        }
        
        //Тип значения
        public override ValueType ValueType { get { return ValueType.Moments; } }
        //Последнее значение
        public Mom LastMoment { get { return this; } }
        //Итоговая ошибка для записи в Result
        public ErrMom TotalError { get { return Error; } }

        //Время значения
        public DateTime Time { get; internal set; }
        
        //Значения разных типов
        public abstract bool Boolean { get; internal set; }
        public abstract int Integer { get; internal set; }
        public abstract double Real { get; internal set; }
        public abstract DateTime Date { get; internal set; }
        public abstract string String { get; internal set; }
        
        //Возвращает значение в виде object
        public abstract object Object { get; }

        //Сравнение значений
        public abstract bool ValueEquals(Mom mom);
        public abstract bool ValueLess(Mom mom);
        //Сравнение значений и ошибок
        public bool ValueAndErrorEquals(Mom mom)
        {
            return ValueEquals(mom) && Error == mom.Error;
        }

        //Клонирует значение
        internal Mom Clone(DateTime time)
        {
            var mom = Create(DataType, time, Error);
            mom.CopyValue(this);
            return mom;
        }

        //Копирует значение из другого мгновенного значения
        internal abstract void CopyValue(IMom mom);
        //Присваивает значение по умолчанию
        internal abstract void MakeDefaultValue();
        
        //Создание мгновенного значения по указанному типу данных
        internal static Mom Create(DataType dataType, DateTime time, ErrMom error)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return new MomBoolean(time, error);
                case DataType.Integer:
                    return new MomInteger(time, error);
                case DataType.Real:
                    return new MomReal(time, error);
                case DataType.Time:
                    return new MomTime(error, time);
                case DataType.String:
                    return new MomString(error, time);
            }
            return null;
        }

        //Создание мгновенного значения из строки с указанным 
        public static Mom CreateFromString(DataType dataType, string s, ErrMom error = null)
        {
            var mom = Create(dataType, Different.MinDate, error);
            mom.String = s;
            return mom;
        }
    }
}
