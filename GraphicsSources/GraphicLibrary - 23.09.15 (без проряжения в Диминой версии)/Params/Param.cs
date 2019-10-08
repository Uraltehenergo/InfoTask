using BaseLibrary;

namespace GraphicLibrary
{
    //Один параметр из базы данных
    public class Param
    {
        public Param(int id, string code, string name, string subName, DataType dataType, string units = "", double min = 0, double max = 100, int decPlaces = -1)
        {
            Id = id;
            Code = code;
            Name = name;
            SubName = subName;
            DataType = dataType;
            Units = units;
            Min = min;
            Max = max;
            DecPlaces = decPlaces;
        }
        
        //Свойства параметра
        public string Code { get; internal set; }
        public int Id { get; internal set; }  //Id из внешней таблицы значений
        public string Name { get; internal set; }
        public string SubName { get; internal set; }
        public DataType DataType { get; internal set; }
        public string Units { get; internal set; }
        public double Min { get; internal set; }
        public double Max { get; internal set; }
        public int DecPlaces { get; internal set; }

        public double ValueToPercent(double value)
        {
            return (value - Min)/(Max - Min)*100;
        }

        public double PercentToValue(double percnetValue)
        {
            return percnetValue * (Max - Min) / 100 + Min;
        }
    }
}