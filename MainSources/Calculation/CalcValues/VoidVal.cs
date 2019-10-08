using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Пустое значение, например, результат операции присвоения
    public class VoidVal : IVal
    {
        public ValueType ValueType { get { return ValueType.Void; } }

        public IVal Value { get { return this; } }

        public IVal CalcValue { get { return null; } }

        public DataType DataType { get {return DataType.Error;} }

        public ErrMom Error { get { return null; } }
    }
}