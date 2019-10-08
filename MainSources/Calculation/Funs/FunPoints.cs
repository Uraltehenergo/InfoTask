using System;
using CommonTypes;
using BaseLibrary;

namespace Calculation
{
    public class FunPoints : CalcFunction
    {
        private FunComplexScalar _valueAtPointsFun;
        public FunComplexScalar ValueAtPointsFun
        {
            get { return _valueAtPointsFun; }
        }

        public ISingleVal UniformPoints(ISingleVal[] par)
        {
            double len = ((Mom)par[1]).Real;
            double shift = ((Mom)par[2]).Real;
            var points = new MomList(DataType.Value);
            DateTime t = Thread.PeriodBegin.AddSeconds(len * shift);
            while (t < Thread.PeriodEnd)
            {
                points.AddMomValue(t);
                t = t.AddSeconds(len);
            }
            //_valueAtPointsFun = new FunComplexScalar();
            _valueAtPointsFun.MomCalculate = _valueAtPointsFun.ValueAtPoints;
            return _valueAtPointsFun.CalculateSingle(new[] {par[0], points});
        }
    }
}