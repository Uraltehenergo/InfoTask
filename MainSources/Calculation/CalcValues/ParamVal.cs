using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Значение - расчетный параметр
    public class ParamVal : IVal
    {
        public ParamVal(CalcParamRun calcRun, ErrMom err = null)
        {
            _val = calcRun.Value;
            CalcParam = calcRun.CalcParam;
            _error = err;
        }

        //Расчетный параметр и его значение
        public CalcParam CalcParam { get; private set; }
        private readonly IVal _val;

        //Типы
        public ValueType ValueType { get { return ValueType.Param; } }
        public DataType DataType { get { return _val.DataType; } }
        
        //Ошибка
        private readonly ErrMom _error;
        public ErrMom Error { get { return _error.Add(_val.Error); } }
        
        //Значения
        public IVal Value { get { return this; } }
        public IVal CalcValue { get { return _val.CalcValue; } }
    }
}