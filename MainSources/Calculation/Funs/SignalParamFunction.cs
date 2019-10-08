using CommonTypes;

namespace Calculation
{
    //Сигналы и параметры
    public class SignalsParamFunction : ParamSignalFunction
    {
        public IVal TakeCodeSignal(IVal[] par)
        {
            var sig = (SignalVal)par[0];
            return new MomString(sig.Signal.FullCode);
        }

        public IVal TakeCodeName(IVal[] par)
        {
            var sig = (SignalVal)par[0];
            return new MomString(sig.Signal.NameObject);
        }

        public IVal TakeCode(IVal[] par)
        {
            var cp = (ParamVal)par[0];
            return new MomString(cp.CalcParam.Code);
        }

        public IVal TakeName(IVal[] par)
        {
            var cp = (ParamVal)par[0];
            return new MomString(cp.CalcParam.Name);
        }
    }
}