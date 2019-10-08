using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //1 - Операции
    public class FunOperations : ScalarFunction
    {
        public void PlusInt(IMom[] par, MomEdit res)
        {
            res.Integer = par[0].Integer + par[1].Integer;
        }

        public void PlusReal(IMom[] par, MomEdit res)
        {
            res.Real = par[0].Real + par[1].Real;
        }

        public void PlusTime(IMom[] par, MomEdit res)
        {
            try { res.Date = par[1].Date.AddSeconds(par[0].Real);}
            catch { res.Date = Different.MaxDate; }
        }

        public void PlusToTime(IMom[] par, MomEdit res)
        {
            try { res.Date = par[0].Date.AddSeconds(par[1].Real); }
            catch { res.Date = Different.MaxDate; }
        }

        public void PlusString(IMom[] par, MomEdit res)
        {
            res.String = par[0].String + par[1].String;
        }

        public void MinusInt(IMom[] par, MomEdit res)
        {
            res.Integer = par[0].Integer - par[1].Integer;
        }

        public void MinusReal(IMom[] par, MomEdit res)
        {
            res.Real = par[0].Real - par[1].Real;
        }

        public void MinusTime(IMom[] par, MomEdit res)
        {
            res.Real = par[0].Date.Subtract(par[1].Date).TotalSeconds;
        }

        public void MinusFromTime(IMom[] par, MomEdit res)
        {
            try { res.Date = par[0].Date.AddSeconds(-par[1].Real); }
            catch { res.Date = Different.MinDate; }
        }

        public void UnaryMinusInt(IMom[] par, MomEdit res)
        {
            res.Integer = - par[0].Integer;
        }

        public void UnaryMinusReal(IMom[] par, MomEdit res)
        {
            res.Real = -par[0].Real;
        }

        public void MultiplyInt(IMom[] par, MomEdit res)
        {
            int m0 = par[0].Integer, m1 = par[1].Integer;
            if (m0 == 0) res.Error = par[0].Error;
            if (m1 == 0) res.Error = par[1].Error;
            if (m0 == 0 && m1 == 0) res.Error = MinError(par);
            res.Integer = m0 * m1;
        }

        public void MultiplyReal(IMom[] par, MomEdit res)
        {
            double m0 = par[0].Real, m1 = par[1].Real;
            if (m0 == 0) res.Error = par[0].Error;
            if (m1 == 0) res.Error = par[1].Error;
            if (m0 == 0 && m1 == 0) res.Error = MinError(par);
            res.Real = m0 * m1;
        }

        public void Divide(IMom[] par, MomEdit res)
        {
            if (par[0].Real == 0)
                res.Error = par[0].Error;
            if (par[1].Real != 0)
                res.Real = par[0].Real/par[1].Real;
            else AddError(res, "Деление на ноль");
        }
    }
}