using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    internal class ObjectWonderware : ProviderObject
    {
        public ObjectWonderware(string tagName)
        {
            Inf = TagName = tagName;
        }

        //Добавляет сигнал в объект, если еще такого нет
        public SignalWonderware AddSignal(SignalWonderware signal)
        {
            if (signal.Bit == -1)
            {
                if (ValueSignal != null) return ValueSignal;
                return ValueSignal = signal;
            }
            if (BitSignals.ContainsKey(signal.Bit)) return BitSignals[signal.Bit];
            BitSignals.Add(signal.Bit, signal);
            return signal;
        }

        //Имя тэга 
        public string TagName { get; private set; }
        //Дискретный или аналоговый сигнал
        public SignalWonderware ValueSignal { get; set; }
        //Биты упакованного сигнала
        private Dictionary<int, SignalWonderware> _bitSignals;
        public Dictionary<int, SignalWonderware> BitSignals { get { return _bitSignals ?? (_bitSignals = new Dictionary<int, SignalWonderware>()); } }

        //Тип данных значения объекта
        public DataType DataType
        {
            get
            {
                if (ValueSignal != null) return ValueSignal.DataType;
                return DataType.Integer;
            }
        }
    }

    //------------------------------------------------------------------------------------------------------

    internal class SignalWonderware : ProviderSignal
    {
        internal SignalWonderware(string signalInf, string code, DataType dataType, IProvider provider, int idInClone)
            : base(signalInf, code, dataType, provider, idInClone)
        {
            TagName = Inf["TagName"];
            Bit = Inf.GetInt("Bit", -1);
            Value = new SingleValue(SingleType.List);
        }

        //Если надо, то бит
        public int Bit { get; private set; }
        //Имя тэга 
        public string TagName { get; private set; }
    }
}