namespace Calculation
{
    //Один элемент выражения - переменная
    public class ExprVar : Expr
    {
        public ExprVar(LexExpr lex, CalcParam calc) : base (lex, calc)
        {
            IsUse = lex.Type == "varuse";
            CalcParam cp = calc;
            while (!cp.Vars.ContainsKey(lex.Code))
                cp = cp.Owner;
            Code = lex.Code;
        }

        //Имя переменной
        public string Code { get; private set; }
        //IsUse - использование, иначе левая часть присвоения
        public bool IsUse { get; private set; }

        protected override CalcValue GetValue()
        {
            CalcParamRun cp = CalcRun;
            while (!cp.Vars.ContainsKey(Code))
                cp = cp.Owner;
            if (IsUse) return cp.Vars[Code].CalcValue;
            return new CalcValue(cp.Vars[Code]);
        }
    }
}