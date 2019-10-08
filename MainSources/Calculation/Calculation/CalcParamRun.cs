using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один экземпляр расчетного параметра
    public class CalcParamRun 
    {
        //Коструктор для создания виртуального корневого параметра
        public CalcParamRun() {}

        //calc - CalcParam с формулой для расчета, inputs - список входных значений
        //owner - владелец, caller - параметр, из которого вызывается
        public CalcParamRun(CalcParam calc, CalcValue[] inputs, CalcParamRun owner, CalcParamRun caller)
        {
            CalcParam = calc;
            Owner = owner;
            Caller = caller;
            if (inputs == null || inputs.Length == 0)
            {
                Owner.Methods.Add(calc.Code, this);
                if (calc.IsNotObject) calc.RunParam = this;
            }
                
            StackDepth = caller == null ? 1 : caller.StackDepth + 1;
            if (StackDepth > 500)
            {
                CalcValue = new CalcValue(new SingleValue(new Moment(DataType.Value, new ErrorCalc("Переполнение стека", CalcParam.FullCode))));
                return;
            } 

            foreach (var k in calc.Vars.Keys)
                Vars.Add(k, new VarRun(calc.Vars[k]));
            
            if (inputs != null)
                for (int i = 0; i < inputs.Length; i++)
                {
                    VarRun v = Vars[calc.Inputs[i]];
                    v.CalcValue = inputs[i];
                    Inputs.Add(v);
                }   

            ThreadCalc = CalcParam.Project.ThreadCalc;
            //Сразу производится расчет
            Calculate();

            //Добавление ошибок расчета в проект
            if (CalcParam.Inputs.Count == 0)
            {
                var errors = CalcParam.Project.CalcErrors;
                if (CalcValue.Type != CalcValueType.Single)
                {
                    if (CalcValue.Error != null) errors.Add(CalcValue.Error);
                }
                else
                {
                    var sv = CalcValue.SingleValue;
                    if (sv.Moment != null && sv.Moment.Error != null)
                        errors.Add(sv.Moment.Error);
                    if (sv.Moments != null)
                        foreach (var m in sv.Moments)
                            if (m.Error != null)
                            {
                                errors.Add(m.Error);
                                break;
                            }
                    if (sv.Segments != null)
                        foreach (var seg in sv.Segments)
                            if (seg.Error != null)
                            {
                                errors.Add(seg.Error);
                                break;
                            }
                }    
            }
        }

        //Связать расчетный параметр с архивным параметром, если нужно
        //fullCode - полный код архивного параметра
        public void JoinArchiveParam(string fullCode, bool alreadyConcidered = false)
        {
            var apars = CalcParam.Project.ArchiveParams;
            var fc = fullCode.ToLower();
            bool ac = alreadyConcidered;
            if (!alreadyConcidered && apars.ContainsKey(fc) && !CalcParam.SuperProcess.IsNone())
            {
                apars[fc].RunParam = this;
                ac = true;
            }
            if (CalcValue.ParentParam != null)
                CalcValue.ParentParam.JoinArchiveParam(fullCode, ac);
            if (CalcParam.Inputs.Count > 0)
                foreach (var met in CalcParam.Methods.Values)
                    if (met.Inputs.Count == 0)
                    {
                        if (Methods.ContainsKey(met.Code))
                            Methods[met.Code].JoinArchiveParam(fullCode + "." + met.Code);
                        else
                            new CalcParamRun(met, null, this, null).JoinArchiveParam(fullCode + "." + met.Code);
                    }
        }

        //Ссылка на CalcParam
        public CalcParam CalcParam { get; private set; }
        //Результат вычисления
        public CalcValue CalcValue { get; set; }
        //Переменные
        private readonly DicS<VarRun> _vars = new DicS<VarRun>();
        public DicS<VarRun> Vars { get { return _vars; } }
        //Входные параметры
        private readonly List<VarRun> _inputs = new List<VarRun>();
        public List<VarRun> Inputs { get { return _inputs; } }
        //Владелец
        public CalcParamRun Owner { get; private set; }
        //Параметр, из которого вызван данный
        public CalcParamRun Caller { get; private set; }
        //Методы
        private readonly DicS<CalcParamRun> _methods = new DicS<CalcParamRun>();
        public DicS<CalcParamRun> Methods { get { return _methods; } }
        //Ссылка на поток
        public ThreadCalc ThreadCalc { get; private set; }
        //Глубина стека вызова данного параметра
        public int StackDepth { get; private set; }

        //Вычисляет значение выражения
        public void Calculate()
        {
            var stack = new Stack<CalcValue>(); //Стек для расчетов
            var stackif = new Stack<FilterIf>(); //Стек для фильтров по if
            stackif.Push(new FilterIf());
            int i = 0,n = CalcParam.Exprs.Length;
            while (i < n)
            {
                var expr = CalcParam.Exprs[i];
                if (!(expr is ExprOp) || !((ExprOp)expr).IsFilterIf) 
                    expr.ProcessCalcValue(stack, this);
                else //Обработка операторов If, While и присваиваний
                {
                    var par = new CalcValue[expr.ParamsCount];
                    for (int j = expr.ParamsCount - 1; j >= 0; j--)
                        par[j] = stack.Pop();
                    var f = ((ExprOp) expr).Code;
                    int adr = ((ExprOp) expr).LinkAddress;
                    switch (f)
                    {
                        case "assign":
                        case "assignr":
                        case "assignv":
                        case "assigni":
                        case "assigns":
                            CalcValue z = null;
                            VarRun vr = null;
                            switch (f)
                            {
                                case "assign":
                                    vr = par[0].VarRun;
                                    z = par[1];
                                    break;
                                case "assignr":
                                    vr = par[1].VarRun;
                                    z = par[0];
                                    break;
                                case "assignv":
                                    vr = par[0].VarRun;
                                    z = new CalcValue();
                                    break;
                                case "assigni":
                                    z = par[2];
                                    int ii = par[1].SingleValue.LastMoment.Integer;
                                    vr = par[0].VarRun;
                                    if (vr.CalcValue == null) 
                                        vr.CalcValue = new CalcValue(new SortedDictionary<int, VarRun>());
                                    var ia = vr.CalcValue.IntArray;
                                    if (ia.ContainsKey(ii)) vr = ia[ii];
                                    else
                                    {
                                        vr = new VarRun();
                                        ia.Add(ii, vr);
                                    }
                                    break;
                                case "assigns":
                                    z = par[2];
                                    string ss = par[1].SingleValue.LastMoment.String;
                                    vr = par[0].VarRun;
                                    if (vr.CalcValue == null)
                                        vr.CalcValue = new CalcValue(new SortedDictionary<string, VarRun>());
                                    var sa = vr.CalcValue.StringArray;
                                    if (sa.ContainsKey(ss)) vr = sa[ss];
                                    else
                                    {
                                        vr = new VarRun();
                                        sa.Add(ss, vr);
                                    }
                                    break;
                            }
                            if (z.Type == CalcValueType.Void || vr.CalcValue == null)
                                vr.CalcValue = z;
                            else
                            {
                                var ppar = new CalcValue[3];
                                ppar[0] = vr.CalcValue;
                                ppar[1] = z;
                                ppar[2] = stackif.Peek().True;
                                vr.CalcValue = stackif.Peek().IsPoints ? ThreadCalc.Funs.GeneralFunction(ppar, vr.DataType, this, null, ThreadCalc.Funs.assignp)
                                                                                                                : ThreadCalc.Funs.GeneralFunction(ppar, vr.DataType, this, ThreadCalc.Funs.assign, null);
                            }
                            vr.CalcValue.ParentParam = z.ParentParam;
                            break;

                        case "begin":
                            stackif.Push(stackif.Peek());
                            break;

                        case "if":
                        case "ifp":
                        case "ifv":
                        case "ifpv":
                            stackif.Push(new FilterIf(stackif.Peek(), par[0], this, f == "ifp" || f == "ifpv"));
                            if (f == "ifv" || f == "ifpv") stack.Push(stackif.Peek().True);
                            if (IsFalse(stackif.Peek().True))
                            {
                                if (f == "ifv" || f == "ifpv") stack.Push(new CalcValue(new SingleValue(new Moment(false))));
                                i = adr-1;
                            }
                            break;

                        case "while":
                        case "whilep":
                            stackif.Push(new FilterIf(stackif.Pop(), par[0], this, f == "whilep"));
                            if (IsFalse(stackif.Peek().True)) i = adr-1;
                            break;

                        case "else":
                        case "elsev":
                            stackif.Peek().ChangeToElse();
                            if (f == "elsev") stack.Push(stackif.Peek().True);
                            if (IsFalse(stackif.Peek().True))
                            {
                                if (f == "elsev") stack.Push(new CalcValue(new SingleValue(new Moment(false))));
                                i = adr-1;
                            }
                            break;

                        case "ret":
                            i = adr-1;
                            break;

                        case "end":
                            bool ep = stackif.Pop().IsPoints;
                            if (par.Length == 2) stack.Push(par[1]);
                            if (par.Length == 4)
                            {
                                var dt = ((ExprOp) expr).DataType;
                                if (IsFalse(par[0])) stack.Push(par[3].ChangeDataType(dt));
                                else if (IsFalse(par[2])) stack.Push(par[1].ChangeDataType(dt));
                                else
                                {
                                    stack.Push(ep ? ThreadCalc.Funs.GeneralFunction(par, dt, this, null, ThreadCalc.Funs.endifp) 
                                                         : ThreadCalc.Funs.GeneralFunction(par, dt, this, ThreadCalc.Funs.endif, null));
                                }
                            }
                            break;

                        case "for":
                            var ef = ((ExprOp) expr);
                            if (par[1].Type == CalcValueType.IntArray)
                            {
                                if (ef.Num == 0)
                                {
                                    ef.IntInd = new List<int>();
                                    foreach (var k in par[1].IntArray.Keys)
                                        ef.IntInd.Add(k);
                                }    
                                if (ef.Num == ef.IntInd.Count)
                                {
                                    ef.Num = 0;
                                    i = adr-1;
                                }
                                else par[0].VarRun.CalcValue = new CalcValue(new SingleValue(new Moment(ef.IntInd[ef.Num]))){Error = par[1].Error};
                            }
                            else
                            {
                                if (ef.Num == 0)
                                {
                                    ef.StringInd = new List<string>();
                                    foreach (var k in par[1].StringArray.Keys)
                                        ef.StringInd.Add(k);
                                }
                                if (ef.Num == ef.StringInd.Count)
                                {
                                    ef.Num = 0;
                                    i = adr-1;
                                }
                                else par[0].VarRun.CalcValue = new CalcValue(new SingleValue(new Moment(ef.StringInd[ef.Num]))) { Error = par[1].Error };
                            }
                            break;
                    }   
                }
                i++;
            }
            CalcValue = stack.Count > 0 ? stack.Pop() : new CalcValue();
        }

        //True, если cv - всегда false
        public bool IsFalse(CalcValue cv)
        {
            return cv.Type == CalcValueType.Single && cv.SingleValue.Type == SingleType.Moment && !cv.SingleValue.Moment.Boolean;
        }
   }
}
