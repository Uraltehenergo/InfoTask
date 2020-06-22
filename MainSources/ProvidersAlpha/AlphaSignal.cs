using BaseLibrary;
using CommonTypes;

namespace Provider
{
    public class AlphaSignal : ProviderSignal
    {
        public AlphaSignal(string signalInf, string code, DataType dataType, IProvider provider, int idInClone = 0)
            : base(signalInf, code, dataType, provider, idInClone)
        {
            NodeId = Inf.GetInt("nodeid");
        }

        //id сигнала в Historian
        public int NodeId { get; set; }
    }
}