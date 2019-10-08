using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один элемент выражения - константа
    public class ExprConst : Expr
    {
        public ExprConst(LexExpr lex, CalcParam calc)
            : base(lex, calc)
        {
            switch (lex.DataType)
            {
                case DataType.Boolean:
                    _moment = new Moment(DataType.Boolean, lex.Code);
                    break;
                case DataType.Integer:
                    _moment = new Moment(DataType.Integer, lex.Code);
                    break;
                case DataType.Real:
                    _moment = new Moment(DataType.Real, lex.Code);
                    break;
                case DataType.Time:
                    _moment = new Moment(DataType.Time, Lex.Code);
                    break;
                case DataType.String:
                    _moment = new Moment(DataType.String, calc.Strings[int.Parse(Lex.Code)]);
                    break;
            }
            if (_moment == null) _moment = new Moment(DataType.Boolean);
        }

        //Значение константы
        private readonly Moment _moment;

        protected override CalcValue GetValue()
        {
            return new CalcValue(new SingleValue(_moment.Clone(Thread.PeriodBegin)));
        }
    }
}