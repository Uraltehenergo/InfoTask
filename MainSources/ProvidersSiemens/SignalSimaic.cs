using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Объект
    internal class SimaticObject : ProviderObject
    {
        public SimaticObject(SimaticSignal signal)
        {
            Id = signal.Id;
            Tag = signal.Inf["Tag"];
            Archive = signal.Inf["Archive"];
            AddSignal(signal);
        }

        //Добавление сигнала
        public SimaticSignal AddSignal(SimaticSignal signal)
        {
            switch (signal.Inf.Get("Prop", "").ToLower())
            {
                case "quality": 
                    return SignalQuality = SignalQuality ?? signal;
                case "flags": 
                    return SignalFlags = SignalFlags ?? signal; 
                default:
                    return SignalValue = SignalValue ?? signal; 
            }
        }

        //Сигналы: значение, качество, флаги
        public SimaticSignal SignalValue { get; private set; }
        public SimaticSignal SignalQuality { get; private set; }
        public SimaticSignal SignalFlags { get; private set; }

        //Имя архивного тэга
        public string Tag { get; private set; }
        //Имя архива
        public string Archive { get; private set; }
        //Id в таблице архива
        public int Id { get; private set; }
    }

    //---------------------------------------------------------------------------------------------------------------------
    //Сигнал
    internal class SimaticSignal : ProviderSignal
    {
        public SimaticSignal(string signalInf, string code, DataType dataType, IProvider provider, int idInClone = 0) 
            : base(signalInf, code, dataType, provider, idInClone)
        {
            Id = Inf.GetInt("Id");
        }

        //Id в таблице архива
        public int Id { get; private set; }
    }
}