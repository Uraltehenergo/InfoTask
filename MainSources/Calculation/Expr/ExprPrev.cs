using CommonTypes;

namespace Calculation
{
    //Один элемент выражения - параметр из цепочки функции Пред
    public class ExprPrev : Expr
    {
        public ExprPrev(LexExpr lex, CalcParam calc) : base(lex, calc)
        {
        }

        protected override CalcValue GetValue()
        {
            string s = Inputs.Length == 0 ? Lex.Code : Inputs[0].SingleValue.LastMoment.String + "." + Lex.Code;
            return new CalcValue(new SingleValue(new Moment(Thread.PeriodBegin, s)));
        }
    }
}