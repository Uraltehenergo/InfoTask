using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Значение - переменная, реализована как декоратор над значением
    public class VarVal : IVal
    {
        public ValueType ValueType { get { return ValueType.Var; } }

        //Значение переменной
        public IVal Value { get; set; }

        //Свойства, взятые из значения
        public IVal CalcValue { get { return Value.CalcValue; } }
        public DataType DataType { get { return Value.DataType; } }

        public ErrMom Error { get { return Value.Error; } }
    }
}