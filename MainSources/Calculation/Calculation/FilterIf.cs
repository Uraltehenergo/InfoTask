using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Элементы для стека фильтров по If
    public class FilterIf
    {
        public FilterIf()
        {
            True = new CalcValue(new SingleValue(new Moment(true)));
            False = new CalcValue(new SingleValue(new Moment(false)));
        }

        public FilterIf(FilterIf parent, CalcValue condition, CalcParamRun calc, bool isPoints)
        {
            var funs = calc.ThreadCalc.Funs;
            IsPoints = isPoints;
            var par = new CalcValue[2];
            par[0] = parent.True;
            par[1] = condition;
            True = funs.GeneralFunction(par, DataType.Boolean, calc, funs.andbb, null);
            var parn = new[] { condition };
            par[1] = funs.GeneralFunction(parn, DataType.Boolean, calc, funs.notb, null);
            False = funs.GeneralFunction(par, DataType.Boolean, calc, funs.andbb, null);
            var b = new CalcValue(new SingleValue(new Moment(true)));
            parn = new[] { True, b };
            True = funs.GeneralFunction(parn, DataType.Boolean, calc, null, funs.deleterepetitionsub);
            parn = new[] { False, b };
            False = funs.GeneralFunction(parn, DataType.Boolean, calc, null, funs.deleterepetitionsub);    
        }

        //Фильтр по значению true
        public CalcValue True { get; private set; }
        //Фильтр по значению false
        public CalcValue False { get; private set; }
        //True, если фильтр только по точкам
        public bool IsPoints { get; private set; }

        public void ChangeToElse()
        {
            True = False;
            False = new CalcValue(new SingleValue(new Moment(false)));
        }
    }
}