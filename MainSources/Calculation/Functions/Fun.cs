using System.Reflection;
using BaseLibrary;

namespace Calculation
{
    //Типы функций
    public enum FunCodeType
    {
        Scalar,
        List,
        Array,
        Operator
    }

    //------------------------------------------------------------------

    //Одна функция из сиска описаний функций SprFunctionsTypes, 
    public class Fun
    {
        public Fun(IRecordRead rec, ThreadCalc thread)
        {
            string name = rec.GetString("Name");
            string synonym = rec.GetString("Synonym");
            Code = rec.GetString("Code") ?? (synonym ?? name).ToLower();
            string s = Code;
            for (int i = 1; i < 10 && rec.GetString("Operand" + i) != null; i++)
                s = s + rec.GetString("Operand" + i).ToDataType().ToLetter();
                
            MethodInfo met = typeof(Funs).GetMethod(s);
            switch (rec.GetString("CodeType"))
            {
                case "scalar":
                    CodeType = FunCodeType.Scalar;
                    if (met != null) ScalarDelegate = (Funs.ScalarDelegate)System.Delegate.CreateDelegate(typeof(Funs.ScalarDelegate), thread.Funs, met);
                    break;
                case "list":
                    CodeType = FunCodeType.List;
                    if (met != null) ListDelegate = (Funs.ListDelegate)System.Delegate.CreateDelegate(typeof(Funs.ListDelegate), thread.Funs, met);
                    break;
                case "array":
                    CodeType = FunCodeType.Array;
                    if (met != null) ArrayDelegate = (Funs.ArrayDelegate)System.Delegate.CreateDelegate(typeof(Funs.ArrayDelegate), thread.Funs, met);
                    break;
                case "operator":
                    CodeType = FunCodeType.Operator;
                    break;
            }
            thread.FunsDic.Add(s, this);
        }

        //Делегат функции scalar
        public Funs.ScalarDelegate ScalarDelegate { get; private set; }
        //Делегат функции list
        public Funs.ListDelegate ListDelegate { get; private set; }
        //Делегат функции array
        public Funs.ArrayDelegate ArrayDelegate { get; private set; }

        //Используемое имя
        public string Code { get; private set; }
        //Тип функции
        public FunCodeType CodeType { get; private set; }
    }
}