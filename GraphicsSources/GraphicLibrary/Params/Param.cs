using BaseLibrary;

namespace GraphicLibrary
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
        public double? OutMin { get; private set; }
        public double? OutMax { get; private set; }
        //public double Min { get; private set; }
        //public double Max { get; private set; }
        public int DecPlaces { get; private set; }
    #endregion Props

    #region Constructor
        public Param(int id, string code, string name, string subName, DataType dataType, string units = "", double? min = 0, double? max = 1, int decPlaces = -1)
        {
            Id = id;
            Code = code;
            Name = name;
            SubName = subName;
            DataType = dataType;
            Units = units;
            OutMin = min;
            OutMax = max;

            /*if ((min != null) && (max != null))
                if (min < max)
                {
                    Min = (double)min;
                    Max = (double)max;
                }
                else if (min > max)
                {
                    Min = (double)max;
                    Max = (double)min;
                }
                else
                {
                    Min = (double)min;
                    Max = (double)min + 1;
                }
            else if (min != null)
            {
                Min = (double)min;
                Max = Min + 1;
            }
            else if (max != null)
            {
                Max = (double)max;
                Min = (double)max - 1;
            }
            else
            {
                Min = 0;
                Max = 1;
            }*/

            DecPlaces = decPlaces;
        }
    #endregion Constructor

    //#region PublicFunction 
        //public double ValueToPercent(double value)
        //{
        //    if (Min == Max) return 100;
        //    return (value - Min) / (Max - Min) * 100;
        //}

        //public double PercentToValue(double percentValue)
        //{
        //    return percentValue * (Max - Min) / 100 + Min;
        //}
    //#endregion PublicFunction
    }
}