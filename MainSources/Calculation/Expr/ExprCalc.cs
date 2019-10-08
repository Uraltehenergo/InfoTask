namespace Calculation
{
    //Один элемент выражения - расчетный параметр
    public class ExprCalc : Expr
    {
        public ExprCalc(LexExpr lex, CalcParam calc) : base(lex, calc)
        {
            IsMet = Lex.Type == "met";
        }

        //True, если метод
        public bool IsMet { get; private set; }
        //Для метода - значение, от которого он вызван
        public CalcValue MetOwner { get; set; }

        protected override CalcValue GetValue()
        {
            CalcParamRun c = CalcRun;
            var pr = CalcRun.CalcParam.Project;
            if (IsMet)
            {
                c = MetOwner.ParentParam;
                while (c != null && !c.CalcParam.Methods.ContainsKey(Lex.Code))
                    c = c.CalcValue.ParentParam;
            }
            else
            {
                while (c.CalcParam != null && !c.CalcParam.Methods.ContainsKey(Lex.Code))
                    c = c.Owner;
                if (c.CalcParam == null) c = pr.RootParam;
            }

            CalcParamRun pp;
            if (c.Methods.ContainsKey(Lex.Code))
                pp = c.Methods[Lex.Code];
            else
            {
                var cp = c.CalcParam == null ? pr.CalcParamsCode[Lex.Code] : c.CalcParam.Methods[Lex.Code];
                var cpr = new CalcParamRun(cp, Inputs, c, CalcRun);
                pp = cpr;
            }
            //if (pp.CalcValue == null) return new CalcValue();
            var cv = pp.CalcValue;
            if (cv.Error != null)
                cv = cv.Clone(CalcRun.CalcParam.FullCode);
            return cv.LinkClone(pp);
        }
    }
}