using BaseLibrary;

namespace Calculation
{
    //Один элемент выражения - функция
    public class ExprFun : Expr
    {
        public ExprFun(LexExpr lex, CalcParam calc) : base(lex, calc)
        {
            string fname = lex.Code;//Имя делегата
            for (int k = 0; k < lex.Pars.Count; k++)
                if (lex.Pars[k][0] != '$') fname += lex.Pars[k].Substring(1);
            Fun = Thread.FunsDic[fname];
        }

        //Функция
        public Fun Fun { get; private set; }

        protected override CalcValue GetValue()
        {
            switch (Fun.CodeType)
            {
                case FunCodeType.Scalar:
                    return Thread.Funs.GeneralFunction(Inputs, Lex.DataType, CalcRun, Fun.ScalarDelegate, null);
                case FunCodeType.List:
                    return Thread.Funs.GeneralFunction(Inputs, Lex.DataType, CalcRun, null, Fun.ListDelegate);
                case FunCodeType.Array:
                    return Fun.ArrayDelegate(Inputs, CalcRun);
            }
            return null;
        }
    }
}