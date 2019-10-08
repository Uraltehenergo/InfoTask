using CommonTypes;

namespace Calculation
{
    public class CalcParamBase
    {
        public CalcParamBase() {}

        public CalcParamBase(string code, string name = null, string comment = null, string task = null, CalcParamType calcParamType = CalcParamType.Class, string tag = null)
        {
            Code = code;
            Name = name;
            Comment = comment;
            Task = task;
            CalcParamType = calcParamType;
            Tag = tag;
        }

        //Код
        public string Code { get; protected set; }
        //Поле Name из CalcParams
        public string Name { get; protected set; }
        //Поле Task из Tasks
        public string Task { get; protected set; }
        //Поле Comment из CalcParams
        public string Comment { get; protected set; }
        //Тип параметра
        public CalcParamType CalcParamType { get; protected set; }
        //Дополнительные сведения
        public string Tag { get; protected set; }
    }
}