using BaseLibrary;

namespace GrafeoLibrary
{
    //Один параметр из базы данных
    public class Param
    {
    #region Props
        //Свойства параметра
        public string Code { get; private set; }
        public int Id { get; private set; }  //Id из внешней таблицы значений
        public string Name { get; private set; }
        public string SubName { get; private set; }
        public DataType DataType { get; private set; }
        public string Units { get; private set; }
        public double? Min { get; private set; }
        public double? Max { get; private set; }
        public int DecimalPlaces { get; private set; }
    #endregion Props

    #region Constructor
        public Param(int id, string code, string name, string subName, DataType dataType, string units = "", double? min = 0, double? max = 1, int decimalPlaces = -1)
        {
            Id = id;
            Code = code;
            Name = name;
            SubName = subName;
            DataType = dataType;
            Units = units;
            //OutMin = min;
            //OutMax = max;
            Min = min;
            Max = max;
            DecimalPlaces = decimalPlaces;
        }
    #endregion Constructor
    }
}