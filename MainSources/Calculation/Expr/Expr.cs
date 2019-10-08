using System.Collections.Generic;

namespace Calculation
{
    //Один элемент выражения для расчета
    public abstract class Expr
    {
        protected Expr(LexExpr lex, CalcParam calc)
        {
            Lex = lex;
            CalcParam = calc;
        }

        //Лексема для расчета
        public LexExpr Lex { get; private set; }
        //Расчетный параметр
        protected CalcParam CalcParam { get; private set; }
        //Поток
        protected ThreadCalc Thread { get { return CalcParam.Project.ThreadCalc; } }
        //Количество входных параметров
        public int ParamsCount { get { return Lex.Pars.Count; } }

        //Фабрика для создания Expr разного типа, на входе строка типа Type!Name(DataType,Par,Par,.....)
        public static Expr New(string s, CalcParam calc)
        {

            var lex = new LexExpr(s);
            switch (lex.Type)
            {
                case "const":
                    return new ExprConst(lex, calc);
                case "signal":
                case "handsignal":
                    return new ExprSignal(lex, calc);
                case "var":
                case "varuse":
                    return new ExprVar(lex, calc);
                case "fun":
                    return new ExprFun(lex, calc);
                case "calc":
                case "met":
                    return new ExprCalc(lex, calc);
                case "prev":
                    return new ExprPrev(lex, calc);
                case "grafic":
                    return new ExprGrafic(lex, calc);
                case "op":
                    return new ExprOp(lex, calc);
            }
            return new ExprOp(lex, calc);
        }

        //Входные параметры
        protected CalcValue[] Inputs { get; private set; }
        //Текущий расчетный параметр выполнения
        protected CalcParamRun CalcRun { get; private set; }

        //Вычисление значения, свое для каждого типа выражения
        //На входе стек расчета и расчетный параметр, содержащий этот Expr
        public void ProcessCalcValue(Stack<CalcValue> stack, CalcParamRun calc)
        {
            CalcRun = calc;
            Inputs = new CalcValue[ParamsCount];
            for (int i = ParamsCount - 1; i >= 0; i--)
                Inputs[i] = stack.Pop();
            var em = this as ExprCalc;
            if (em != null && em.IsMet)
                em.MetOwner = stack.Pop();
            var cv = GetValue();
            //if (cv.Type != CalcValueType.Void)
            stack.Push(cv);
            Inputs = null;
        }

        //Вычисление значения, свое для каждого типа выражения
        protected virtual CalcValue GetValue()
        {
            return null;
        }
    }
}