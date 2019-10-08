using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один элемент выражения - график
    public class ExprGrafic : Expr
    {
        public ExprGrafic(LexExpr lex, CalcParam calc) : base(lex, calc)
        {
            Grafic = calc.Project.Grafics[lex.Code];
        }

        //Ссылка на график
        public Grafic Grafic { get; private set; }
        
        protected override CalcValue GetValue()
        {
            if (Grafic == null)
                return new CalcValue(new SingleValue(new Moment(0.0, new ErrorCalc("Не заполнены значения графика ", CalcParam.Code))));
            return CalcRun.ThreadCalc.Funs.GeneralFunction(Inputs, DataType.Real, CalcRun, Grafic.CalculateMean, null);
        }
    }
}