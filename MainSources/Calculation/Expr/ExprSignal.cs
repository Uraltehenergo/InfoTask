using System.Collections.Generic;
using CommonTypes;

namespace Calculation
{
    //Один элемент выражения - сигнал
    public class ExprSignal : Expr
    {
        public ExprSignal(LexExpr lex, CalcParam calc) : base(lex, calc)
        {
            var sou = calc.Signals[int.Parse(lex.Code)];
            if (calc.Project.SignalsSources.ContainsKey(sou))
                _signal = calc.Project.SignalsSources[sou];
            if (lex.Type == "handsignal")
                _handDefaultValue = calc.HandInputValue;
        }

        //Сигнал
        private readonly CalcSignal _signal;
        //Значение-константа для значения по умолчанию для ручного ввода
        private readonly Moment _handDefaultValue;

        protected override CalcValue GetValue()
        {
            if (_signal.SourceSignal == null)
            {
                if (_signal.HasFormula)
                {
                    _signal.CalcSignalValue = Inputs[0].SingleValue;
                    return Inputs[0];
                }
                return new CalcValue(new SingleValue(new List<Moment>()));
            }
            SingleValue sv =_signal.SourceSignal.Value;
            if (sv == null || !sv.HasMoment)
                sv = _handDefaultValue != null ? new SingleValue(_handDefaultValue.Clone(Thread.PeriodBegin)) : new SingleValue(new List<Moment>());
            return new CalcValue(sv, _signal);
        }
    }
}