using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один элемент выражения - системная функция
    public class ExprOp : Expr
    {
        public ExprOp(LexExpr lex, CalcParam calc) : base(lex, calc)
        {
            var ss = Lex.Code.Split('_');
            CodeFun = ss[0];
            if (ss.Length > 1) LinkAddress = int.Parse(ss[1]);
            IsFilterIf = !new HashSet<string> { "void", "owner", "caller", "getelement" }.Contains(CodeFun) && !CodeFun.StartsWith("prev") && !CodeFun.StartsWith("signal") && !CodeFun.StartsWith("take");
        }

        //Имя функции
        public string CodeFun { get; private set; }
        //True, если использует FilterIf
        public bool IsFilterIf { get; private set; }
        //Адрес ссылки по if 
        public int LinkAddress { get; private set; }
        
        //Списки индексов массива (для For)
        public List<int> IntInd { get; set; }
        public List<string> StringInd { get; set; }
        //Номер текущего индекса (для For)
        public int Num { get; set; }

        protected override CalcValue GetValue()
        {
            switch (CodeFun)
            {
                case "void":
                    return new CalcValue();
                    
                case "owner":
                    var cv = CalcRun.Owner.CalcValue;
                    if (cv.ParentParam == null) cv = cv.LinkClone(CalcRun.Owner);
                    return cv;

                case "caller":
                    return new CalcValue{ParentParam = CalcRun.Caller};

                case "getelement":
                    if (Inputs[1].Type != CalcValueType.Single || Inputs[1].SingleValue.Type != SingleType.Moment)
                        return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Индекс массива должен быть отдельным значением", CalcParam.Code))));
                    Moment m = Inputs[1].SingleValue.Moment;
                    if (m.DataType.LessOrEquals(DataType.Integer))
                    {
                        if (Inputs[0].IntArray == null || !Inputs[0].IntArray.ContainsKey(m.Integer))
                            return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Несуществующий индекс массива (" + m.Integer + ")", CalcParam.Code))));
                        return Inputs[0].IntArray[m.Integer].CalcValue;
                    }
                    if (Inputs[0].StringArray == null || !Inputs[0].StringArray.ContainsKey(m.String))
                        return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Несуществующий индекс массива (" + m.String + ")", CalcParam.Code))));
                    return Inputs[0].StringArray[m.String].CalcValue;

                case "signal":
                    return Inputs[0].Signal.Object.Signals[Inputs[1].SingleValue.LastMoment.String].CalcValue;

                case "signalbool":
                case "signalint":
                case "signalreal":
                case "signalstring":
                    string c = Inputs[0].SingleValue.LastMoment.String ?? "";
                    CalcSignal sig = null;
                    if (CalcParam.Project.SignalsSources.ContainsKey(c)) sig = CalcParam.Project.SignalsSources[c];
                    if (CalcParam.Project.Objects.ContainsKey(c)) sig = CalcParam.Project.Objects[c].DefaultSignal;
                    if (sig == null || !sig.DataType.LessOrEquals(CodeFun.Substring(6).ToDataType()))
                        return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Строка задает несуществующий сигнал или сигнал недопустимого типа (" + c + ")", CalcParam.Code))));
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
                        cp = CalcParam;
                    else if (Inputs[0].ParentParam != null) 
                        { cp = Inputs[0].ParentParam.CalcParam; }
                    else if (Inputs[0].Signal != null)
                    {
                        var si = Inputs[0].Signal;
                        return new CalcValue(new SingleValue(new Moment(CalcRun.ThreadCalc.PeriodBegin,
                            CodeFun == "takecode" ? si.CodeObject :
                            CodeFun == "takecodesignal" ? si.CodeSignal :
                            CodeFun == "takename" ? si.NameObject :
                            CodeFun == "takenamesignal" ? si.NameSignal : si.Units)));
                    }
                    if (cp != null)
                        return new CalcValue(new SingleValue(new Moment(CalcRun.ThreadCalc.PeriodBegin,
                            CodeFun == "takecode" ? cp.Code :
                            CodeFun == "takename" ? cp.Name :
                            CodeFun == "taketask" ? cp.Task :
                            CodeFun == "takecomment" ? cp.Comment : cp.Units)));
                    return new CalcValue(new SingleValue(new Moment(false, new ErrorCalc("Параметр функции получения характеристики не задает расчетный параметр или сигнал", CalcParam.Code))));

                case "prevabs":
                case "prevmom":
                case "prev":
                case "prevperiod":
                case "prevhour":
                case "prevhourperiod":
                case "prevday":
                case "prevdayperiod":
                    return new CalcValue(CalcPrev(CodeFun, Inputs));
            }
            return null;
        }

        //Вычисляет значение функции Пред f от параметров par
        private SingleValue CalcPrev(string f, CalcValue[] par)
        {
            string acode = par[0].SingleValue.LastMoment.String;
            string scode = acode.Split('.')[0];
            var pr = CalcParam.Project;
            var cpr = CalcRun;
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
                var err = new ErrorCalc("Не найден архивный параметр " + acode, CalcParam.FullCode);
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