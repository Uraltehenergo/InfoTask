using BaseLibrary;

namespace Calculation
{
    //Один экземпляр переменной или элемента массива
    public class VarRun
    {
        public VarRun(DataType dt = DataType.Value)
        {
            DataType = dt;
        }

        public VarRun(CalcValue calcValue)
        {
            CalcValue = calcValue;
        }

        //Значение
        public CalcValue CalcValue { get; set; }
        //Тип данных, если есть
        public DataType DataType { get; set; }
    }
}