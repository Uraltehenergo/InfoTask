using CommonTypes;

namespace Calculation
{
    //Сложные скалярные функции
    public class FunComplexScalar : MomFunction
    {
        public FunComplexScalar(MomDelegate deleg) : base(deleg) {}

        public bool ValueAtPoints(IMom[] par, bool[] cpar, MomEdit res)
        {
            if (!cpar[1]) return false;
            res.CopyValue(par[0]);
            return true;
        }
    }
}