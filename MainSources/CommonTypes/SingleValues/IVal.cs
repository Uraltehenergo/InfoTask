using BaseLibrary;

namespace CommonTypes
{
    //Базовый интерфейс для всех значений
    public interface IVal
    {
        //Тип значения
        ValueType ValueType { get; }
        //Тип данных
        DataType DataType { get; }

        //Общая ошибка для значения (ошибки мгновенных значений не считаются)
        ErrMom Error { get; }
        
        //Значение само, или значение переменной
        IVal Value { get; }
        //Расчетное значение 
        IVal CalcValue { get; }
    }

    //-----------------------------------------------------------------------------------
    //Базовый класс для расчетных значений и массивов
    public abstract class CalcVal : IVal
    {
        protected CalcVal(ErrMom error)
        {
            Error = error;
        }

        //Является словарем или списком
        public bool IsArray
        {
            get { return ValueType == ValueType.DicString || ValueType == ValueType.DicInt || ValueType == ValueType.List; }
        }

        public abstract ValueType ValueType { get; }
        public abstract DataType DataType { get; }

        //Общая ошибка на все значение
        public ErrMom Error { get; internal set; }

        public IVal Value { get { return this; } }
        public IVal CalcValue { get { return this; } }
    }

    //------------------------------------------------------------------------------------------------
    //Базовый интерфейс для списков мгновенных значений и сегментов
    public interface ISingleVal : IVal
    {
        //Итоговая ошибка для записи в Result
        ErrMom TotalError { get; }
    }

    //------------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений
    public interface IMomentsVal : ISingleVal
    {
        //Возвращает последнее или единственное мгновенное значение, или null, если список пустой
        Mom LastMoment { get; }
    }
}