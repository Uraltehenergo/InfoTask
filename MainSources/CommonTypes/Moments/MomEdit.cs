using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения с возможностью изменения, декоратор над Mom
    public class MomEdit: IMom
    {
        public MomEdit(DataType type = DataType.Value, ErrMom error = null)
        {
            Mom = Mom.Create(type, DateTime.MinValue, error);
        }

        public MomEdit(DataType type, DateTime time, ErrMom error = null)
        {
            Mom = Mom.Create(type, time, error);
        }

        //Создается как клон значения с указанным временем
        public MomEdit(Mom mom, DateTime time)
        {
            Mom = mom.Clone(time);
        }

        //Обертываемое мгновенное значение
        internal Mom Mom { get; private set; }

        //Время значения
        public DateTime Time 
        {
            get { return Mom.Time; }
            set { Mom.Time = value; } 
        }

        //Ошибка
        public ErrMom Error
        {
            get { return Mom.Error; }
            set { Mom.Error = value; }
        }
        //Добавить новую ошибку
        public void AddError(ErrMom err)
        {
            Error = Error.Add(err);
        }

        //Значения разных типов 
        public bool Boolean
        {
            get { return Mom.Boolean; }
            set { Mom.Boolean = value; }
        }

        public int Integer
        {
            get { return Mom.Integer; }
            set { Mom.Integer = value; }
        }

        public double Real
        {
            get { return Mom.Real; }
            set { Mom.Real = value; }
        }

        public DateTime Date
        {
            get { return Mom.Date; }
            set { Mom.Date = value; }
        }

        public string String
        {
            get { return Mom.String; }
            set { Mom.String = value;}
        }

        public bool ValueEquals(Mom mom)
        {
            return Mom.ValueEquals(mom);
        }

        public bool ValueLess(Mom mom)
        {
            return Mom.ValueLess(mom);
        }

        public bool ValueAndErrorEquals(Mom mom)
        {
            return Mom.ValueAndErrorEquals(mom);
        }

        //Сброс значения
        public void MakeDefaultValue()
        {
            Mom.MakeDefaultValue();
        }
        //Копирует значение из другого мгновенного значения
        public void CopyValue(IMom mom)
        {
            Mom.CopyValue(mom);
        }

        //Типы данных и значения
        public ValueType ValueType { get { return Mom.ValueType; } }
        public DataType DataType { get { return Mom.DataType; } }
        public IVal Value { get { return Mom.Value; } }
        public IVal CalcValue { get { return Mom.CalcValue; } }

        //Создает значение из временного значения
        public Mom ToMom()
        {
            var res = Mom.Create(DataType, Time, Error);
            res.CopyValue(this);
            return res;
        }

        public ErrMom TotalError { get; private set; }
        public Mom LastMoment { get; private set; }
    }
}