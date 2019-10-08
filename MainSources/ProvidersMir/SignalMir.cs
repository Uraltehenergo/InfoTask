using BaseLibrary;
using CommonTypes;

namespace Provider
{
    internal class SignalMir : ProviderSignal
    {
        internal SignalMir(string signalInf, string code, DataType dataType, IProvider provider, int idInClone)
            : base(signalInf, code, dataType, provider, idInClone)
        {
            Value = new SingleValue(SingleType.List);
        }

        //Id для получения значений
        public int IdChannel { get; set; }
    }
}