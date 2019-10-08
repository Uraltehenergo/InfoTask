
namespace Tablik
{
    //Тип переменной
    internal enum VarType
    {
        Var, //Переменная
        Input //Входной параметр
    }

    //--------------------------------------------------------------    

    //Переменная или параметр функции
    internal class Var
    {
        public Var(VarType varType, string code, string defaultValue = null)
        {
            CalcType = new CalcType(ClassType.Undef);
            VarType = varType;
            Code = code;
            DefaultValue = defaultValue;
        }

        //Тип переменной
        public VarType VarType { get; private set; }
        //Код
        public string Code { get; private set; }
        //Значение по умолчанию
        public string DefaultValue { get; set; }
        //Тип 
        public CalcType CalcType { get; set; }
    }
}

