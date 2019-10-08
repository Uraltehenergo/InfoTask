using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один элемент выражения
    public abstract class Expr
    {
        protected Expr(){}

        protected Expr(string[] s)
        {
            ParamsCount = s.Length - 3;
        }

        //Количество входных параметров
        public int ParamsCount { get; protected set; }
        
        //Фабрика для создания Expr разного типа, на входе строка типа type!name(parmas)
        public static Expr New(string s, CalcParam calc)
        {
            var ss = s.Split(new[]{'!', '(', ',' ,')'}, StringSplitOptions.RemoveEmptyEntries);
            switch (ss[0])
            {
                case "const":
                    return new ExprConst(ss, calc);
                case "signal":
                    return new ExprSignal(ss, calc, false);
                case "handsignal":
                    return new ExprSignal(ss, calc, true);
                case "var":
                case "varuse":
                    return new ExprVar(ss, calc);
                case "fun":
                    return new ExprFun(ss, calc.Project.ThreadCalc);
                case "calc":
                    return new ExprCalc(ss, false);
                case "met":
                    return new ExprCalc(ss, true);
                case "prev":
                    return new ExprPrev(ss);
                case "grafic":
                    return new ExprGrafic(ss, calc);
                case "op":
                    return new ExprOp(ss, calc);
            }
            return new ExprOp(ss, calc);
        }

        //Входные параметры
        protected CalcValue[] Inputs { get; private set; }

        //Вычисление значения, свое для каждого типа выражения
        //На входе стек расчета и расчетный параметр, содержащий этот Expr
        public void ProcessCalcValue(Stack<CalcValue> stack, CalcParamRun calcRun)
        {
            Inputs = new CalcValue[ParamsCount];
            for (int i = ParamsCount - 1; i >= 0; i--)
                Inputs[i] = stack.Pop();
            var em = this as ExprCalc;
            if (em != null && em.IsMet) 
                em.MetOwner = stack.Pop();
            var cv = GetValue(calcRun);
            //if (cv.Type != CalcValueType.Void)
            stack.Push(cv);
            Inputs = null;
        }

        //Вычисление значения, свое для каждого типа выражения
        protected virtual CalcValue GetValue(CalcParamRun calcRun)
        {
            return null;
        }
    }

    //-------------------------------------------------------------------------

    //Один элемент выражения - константа
    public class ExprConst : Expr
    {
        public ExprConst(string[] s, CalcParam calc)
        {
            _thread = calc.Project.ThreadCalc;
            switch (s[2].ToDataType())
            {
                case DataType.Boolean:
                    _moment = new Moment(DataType.Boolean, s[1]);
                    break;
                case DataType.Integer:
                    _moment = new Moment(DataType.Integer, s[1]);
                    break;
                case DataType.Real:
                    _moment = new Moment(DataType.Real, s[1]);
                    break;
                case DataType.Time:
                    _moment = new Moment(DataType.Time, s[1]);
                    break;
                case DataType.String:
                    _moment = new Moment(DataType.String, calc.Strings[int.Parse(s[1])]);
                    break;
            }
            if (_moment == null) _moment = new Moment(DataType.Boolean);
        }

        //Значение константы
        private readonly Moment _moment;
        //Поток 
        private readonly ThreadCalc _thread;

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            return new CalcValue(new SingleValue(_moment.Clone(_thread.PeriodBegin)));
        }
    }

    //-------------------------------------------------------------------------
    //Один элемент выражения - сигнал
    public class ExprSignal : Expr
    {
        public ExprSignal(string[] s, CalcParam calc, bool isHand)
        {
            var sou = calc.Signals[int.Parse(s[1])];
            if (calc.Project.SignalsSources.ContainsKey(sou))
            {
                Signal = calc.Project.SignalsSources[sou];
                if (Signal.SourceSignal == null) ConstValue = Signal.ConstValue;
            }
            _isHand = isHand;
            if (isHand) ConstValue = calc.HandInputValue;
        }

        //Сигнал
        public CalcUnit Signal { get; private set; }
        //Значение-константа
        public Moment ConstValue { get; private set; }
        //Является сигналом ручного ввода
        private readonly bool _isHand;

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            SingleValue sv = Signal == null || Signal.SourceSignal == null ? null : Signal.SourceSignal.Value;
            if (!_isHand && ConstValue != null)
                sv = new SingleValue(ConstValue.Clone(calcRun.ThreadCalc.PeriodBegin));
            else if (sv == null || (sv.Type == SingleType.Moment && sv.Moment == null) || (sv.Type == SingleType.List && (sv.Moments == null || sv.Moments.Count == 0)))
                sv = _isHand ? new SingleValue(ConstValue.Clone(calcRun.ThreadCalc.PeriodBegin)) : new SingleValue(new List<Moment>());
            return new CalcValue(sv, Signal);
        }
    }

    //----------------------------------------------------------------------------------------------------------------
    //Один элемент выражения - переменная
    public class ExprVar : Expr
    {
        public ExprVar(string[] s, CalcParam calc)
        {
            IsUse = s[0] == "varuse";
            CalcParam cp = calc;
            while (!cp.Vars.ContainsKey(s[1]))
                cp = cp.Owner;
            Code = s[1];
        }
        
        //Имя переменной
        public string Code { get; private set; }
        //IsUse - использование, иначе левая часть присвоения
        public bool IsUse { get; private set; }

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            CalcParamRun cp = calcRun;
            while (!cp.Vars.ContainsKey(Code))
                cp = cp.Owner;
            if (IsUse) return cp.Vars[Code].CalcValue;
            return new CalcValue(cp.Vars[Code]);
        }
    }

    //-------------------------------------------------------------------------

    //Один элемент выражения - функция
    public class ExprFun : Expr
    {
        public ExprFun(string[] s, ThreadCalc thread) : base(s)
        {
            _thread = thread;
            string fname = s[1];//Имя делегата
            for (int k = 3; k < s.Length; k++)
                if (s[k][0] != '$') fname += s[k].Substring(1);
            Fun = thread.FunsDic[fname];
            DataType = s[2].ToDataType();
        }
        
        //Функция
        public Fun Fun { get; private set; }
        //Возвращаемый тип 
        public DataType DataType { get; private set; }
        //Поток
        private readonly ThreadCalc _thread;

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            switch (Fun.CodeType)
            {
                case FunCodeType.Scalar:
                    return _thread.Funs.GeneralFunction(Inputs, DataType, calcRun, Fun.ScalarDelegate, null);
                case FunCodeType.List:
                    return _thread.Funs.GeneralFunction(Inputs, DataType, calcRun, null, Fun.ListDelegate);
                case FunCodeType.Array:
                    return Fun.ArrayDelegate(Inputs, calcRun);
            }
            return null;
        }
    }

    //-------------------------------------------------------------------------

    //Один элемент выражения - расчетный параметр
    public class ExprCalc : Expr
    {
        public ExprCalc(string[] s, bool isMet) : base(s)
        {
            IsMet = isMet;
            Code = s[1];
        }
        //Код расчетного параметра
        public string Code { get; private set; }
        //True, если метод
        public bool IsMet { get; private set; }
        //Для метода - значение, от которого он вызван
        public CalcValue MetOwner { get; set; }

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            CalcParamRun c = calcRun;
            var pr = calcRun.CalcParam.Project;
            if (IsMet)
            {
                c =  MetOwner.ParentParam;
                while (c != null && !c.CalcParam.Methods.ContainsKey(Code))
                    c = c.CalcValue.ParentParam;
            }
            else
            {
                while (c.CalcParam != null && !c.CalcParam.Methods.ContainsKey(Code))
                    c = c.Owner;
                if (c.CalcParam == null) c = pr.RootParam;
            }

            CalcParamRun pp;
            if (c.Methods.ContainsKey(Code))
                pp = c.Methods[Code];
            else
            {
                var cp = c.CalcParam == null ? pr.CalcParamsCode[Code] : c.CalcParam.Methods[Code];
                var cpr = new CalcParamRun(cp, Inputs, c, calcRun);
                pp = cpr;
            }
            //if (pp.CalcValue == null) return new CalcValue();
            var cv = pp.CalcValue;
            if (cv.Error != null)
                cv = cv.Clone(calcRun.CalcParam.FullCode);
            MetOwner = null;
            return cv.LinkClone(pp);
        }
    }

    //-------------------------------------------------------------------------

    //Один элемент выражения - параметр из цепочки функции Пред
    public class ExprPrev : Expr
    {
        public ExprPrev(string[] s) : base(s)
        {
            Code = s[1];
        }
        //Имя параметра
        public string Code { get; private set; }

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            string s = Inputs.Length == 0 ? Code : Inputs[0].SingleValue.LastMoment.String + "." + Code;
            return new CalcValue(new SingleValue(new Moment(calcRun.ThreadCalc.PeriodBegin, s)));
        }
    }

    //-------------------------------------------------------------------------

    //Один элемент выражения - график
    public class ExprGrafic : Expr
    {
        public ExprGrafic(string[] s, CalcParam calc) : base(s)
        {
            _calc = calc;
            Grafic = calc.Project.Grafics[s[1]];
        }

        //Ссылка на график
        public Grafic Grafic { get; private set; }
        //Расчетный параметр
        private readonly CalcParam _calc;

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            if (Grafic == null) 
                return new CalcValue(new SingleValue(new Moment(0.0, new ErrorCalc("Не заполнены значения графика ", _calc.Code))));
            return calcRun.ThreadCalc.Funs.GeneralFunction(Inputs, DataType.Real, calcRun, Grafic.CalculateMean, null);
        }
    }

    //-------------------------------------------------------------------------
    //Один элемент выражения - системная функция
    public class ExprOp : Expr
    {
        public ExprOp(string[] s, CalcParam calc) : base(s)
        {
            _calc = calc;
            var ss = s[1].Split('_');
            Code = ss[0];
            if (ss.Length > 1) LinkAddress = int.Parse(ss[1]);
            if (s.Length > 2) DataType = s[2].ToDataType();
            IsFilterIf = !new HashSet<string> { "void", "owner", "caller", "getelement" }.Contains(Code) && !Code.StartsWith("prev") && !Code.StartsWith("signal") && !Code.StartsWith("take");
        }

        //Имя функции
        public string Code { get; private set; }
        //True, если использует FilterIf
        public bool IsFilterIf { get; private set; }
        //Адрес ссылки по if 
        public int LinkAddress { get; private set; }
        //Возвращаемый тип
        public DataType DataType { get; private set; }
        //Расчетный параметр
        private readonly CalcParam _calc;

        //Списки индексов массива (для For)
        public List<int> IntInd { get; set; }
        public List<string> StringInd { get; set; }
        //Номер текущего индекса (для For)
        public int Num { get; set; }

        protected override CalcValue GetValue(CalcParamRun calcRun)
        {
            switch (Code)
            {
                case "void":
                    return new CalcValue();
                    
                case "owner":
                    var cv = calcRun.Owner.CalcValue;
                    if (cv.ParentParam == null) cv = cv.LinkClone(calcRun.Owner);
                    return cv;

                case "caller":
                    return new CalcValue { ParentParam = calcRun.Caller };

                case "getelement":
                    if (Inputs[1].Type != CalcValueType.Single || Inputs[1].SingleValue.Type != SingleType.Moment)
                        return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Индекс массива должен быть отдельным значением", _calc.Code))));
                    Moment m = Inputs[1].SingleValue.Moment;
                    if (m.DataType.LessOrEquals(DataType.Integer))
                    {
                        if (Inputs[0].IntArray == null || !Inputs[0].IntArray.ContainsKey(m.Integer))
                            return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Несуществующий индекс массива (" + m.Integer + ")", _calc.Code))));
                        return Inputs[0].IntArray[m.Integer].CalcValue;
                    }
                    if (Inputs[0].StringArray == null || !Inputs[0].StringArray.ContainsKey(m.String))
                        return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Несуществующий индекс массива (" + m.String + ")", _calc.Code))));
                    return Inputs[0].StringArray[m.String].CalcValue;

                case "signal":
                    try { return Inputs[0].Signal.Object.Signals[Inputs[1].SingleValue.LastMoment.String].CalcValue; }
                    catch { return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Недопустимые параметры функции Сигнал", _calc.Code)))); }

                case "signalbool":
                case "signalint":
                case "signalreal":
                case "signalstring":
                    string c = Inputs[0].SingleValue.LastMoment.String ?? "";
                    CalcUnit sig = null;
                    if (_calc.Project.SignalsSources.ContainsKey(c)) sig = _calc.Project.SignalsSources[c];
                    if (_calc.Project.Objects.ContainsKey(c)) sig = _calc.Project.Objects[c].DefaultSignal;
                    if (sig == null || !sig.DataType.LessOrEquals(Code.Substring(6).ToDataType()))
                        return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Строка задает несуществующий сигнал или сигнал недопустимого типа (" + c + ")", _calc.Code))));
                    return sig.CalcValue;

                case "takecode":
                case "takename":
                case "takeunits":
                case "taketask":
                case "takecomment":
                case "takecodesignal":
                case "takenamesignal":
                    CalcParam cp = null;
                    if (Inputs.Length == 0) 
                        cp = _calc;
                    else if (Inputs[0].ParentParam != null) 
                        { cp = Inputs[0].ParentParam.CalcParam; }
                    else if (Inputs[0].Signal != null)
                    {
                        var si = Inputs[0].Signal;
                        return new CalcValue(new SingleValue(new Moment(calcRun.ThreadCalc.PeriodBegin,
                            Code == "takecode" ? si.CodeObject :
                            Code == "takecodesignal" ? si.CodeSignal :
                            Code == "takename" ? si.NameObject :
                            Code == "takenamesignal" ? si.NameSignal : si.Units)));
                    }
                    if (cp != null)
                        return new CalcValue(new SingleValue(new Moment(calcRun.ThreadCalc.PeriodBegin,
                            Code == "takecode" ? cp.Code :
                            Code == "takename" ? cp.Name :
                            Code == "taketask" ? cp.Task :
                            Code == "takecomment" ? cp.Comment : cp.Units)));
                    return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Параметр функции получения характеристики не задает расчетный параметр или сигнал", _calc.Code))));

                case "prevabs":
                case "prevmom":
                case "prev":
                case "prevperiod":
                case "prevhour":
                case "prevhourperiod":
                case "prevday":
                case "prevdayperiod":
                    return new CalcValue(CalcPrev(Code, Inputs, calcRun));
            }
            return null;
        }

        //Вычисляет значение функции Пред f от параметров par
        private SingleValue CalcPrev(string f, CalcValue[] par, CalcParamRun calcRun)
        {
            string acode = par[0].SingleValue.LastMoment.String;
            string scode = acode.Split('.')[0];
            var pr = _calc.Project;
            var cpr = calcRun;
            while (cpr != pr.RootParam && !cpr.CalcParam.Methods.ContainsKey(scode))
                cpr = cpr.Owner;
            while (cpr != pr.RootParam)
            {
                while (cpr.Inputs.Count != 0)
                    cpr = cpr.Caller;
                acode = cpr.CalcParam.Code + "." + acode;
                cpr = cpr.Owner;
            }
            if (!pr.ArchiveParams.ContainsKey(acode))
            {
                var err = new ErrorCalc("Не найден архивный параметр " + acode, _calc.FullCode);
                return new SingleValue(new Moment(par[1].SingleValue.DataType, err));
            }
            var beg = pr.ThreadCalc.PeriodBegin;
            var p1 = par[1].SingleValue.LastMoment.Clone(beg);
            if (f == "prevabs")
            {
                var ap = pr.ArchiveParams[acode];
                var m = ((ap.AbsoluteEnd == beg && ap.AbsoluteValue != null) ? ap.AbsoluteValue : p1).Clone();
                if (par.Length == 2 || !par[2].SingleValue.LastMoment.Boolean)
                    m.Time = beg;
                return new SingleValue(m);
            }
            var pp = pr.PrevParams[acode];
            Moment mom = null;
            if (f == "prev") 
                mom = (pp.LastBase == null || pp.LastBase.DataType == DataType.Value) ? p1 : pp.LastBase;
            if (f == "prevhour") 
                mom = (pp.LastHour == null || pp.LastHour.DataType == DataType.Value) ? p1 : pp.LastHour;
            if (f == "prevday") 
                mom = (pp.LastDay == null || pp.LastDay.DataType == DataType.Value) ? p1 : pp.LastDay;
            if (mom != null) return new SingleValue(mom.Clone(beg));
            if (f == "prevmom")
                return new SingleValue(pp.ManyMoments.Where(m => beg.Subtract(m.Time).TotalMinutes <= p1.Real).ToList());
            if (f == "prevperiod")
                return new SingleValue(pp.ManyBase.Where(m => beg.Subtract(m.Time).TotalMinutes <= p1.Real).ToList());
            if (f == "prevhourperiod")
                return new SingleValue(pp.ManyHour.Where(m => beg.Subtract(m.Time).TotalHours <= p1.Real).ToList());
            if (f == "prevdayperiod")
                return new SingleValue(pp.ManyDay.Where(m => beg.Subtract(m.Time).TotalDays <= p1.Real).ToList());
            return null;
        }

    }
}