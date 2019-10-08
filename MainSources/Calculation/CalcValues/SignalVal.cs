using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class SignalVal : IVal
    {
        public SignalVal(CalcSignal signal, ErrMom err = null) 
        {
            Signal = signal;
            _calcValue = Signal.SourceSignal.MomentsVal;
            _error = err;
        }

        //Порождающий сигнал
        public CalcSignal Signal { get; set; }

        //Типы
        public ValueType ValueType { get {return ValueType.Signal; } }
        public DataType DataType { get { return Signal.DataType; } }

        //Значение
        public IVal Value { get { return this; } }
        //Расчетное значение
        private readonly IVal _calcValue;
        public IVal CalcValue { get { return _calcValue; } }
        
        //Ошибка
        private readonly ErrMom _error;
        public ErrMom Error { get { return _error.Add(CalcValue.Error); } }
    }
}