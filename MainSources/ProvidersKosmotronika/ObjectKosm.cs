using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Структура для индекса словаря сигналов
    internal struct ObjectIndex
    {
        //Системный номер
        internal int Sn;
        //Номер типа
        internal int NumType;
        //Подразделение
        internal int Appartment;
        //Номер выхода
        internal int Out;
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    //Один объект для непосредственного считывания с архива космотроники
    //Для аналоговых - один ТМ, для выходов - один выход ТМ
    internal class ObjectKosm : ProviderObject
    {
        public ObjectKosm(ObjectIndex ind, string code)
        {
            Sn = ind.Sn; 
            NumType = ind.NumType;
            Appartment = ind.Appartment;
            Out = ind.Out;
            int p = code.LastIndexOf(".");
            string s = p == -1 ? code : code.Substring(0, p);
            Inf = "Code=" + s + "; SN=" + Sn + "; NumType=" + NumType + ";Out=" + Out + "; Appartment=" + Appartment + ";";
        }

        //Добавить к объекту сигнал, если такого еще не было
        public ProviderSignal AddSignal(ProviderSignal sig)
        {
            if (sig.Inf["Prop"] == "ND")
                return StateSignal ?? (StateSignal = sig);
            if (sig.Inf["Prop"] == "POK")
                return PokSignal ?? (PokSignal = sig);
            int bit = sig.Inf.GetInt("NumBit", -1);
            if (bit == -1)
                return ValueSignal ?? (ValueSignal = sig);
            BitSignals.Add(bit, sig);
            return BitSignals[bit];
        }
        
        //Системный номер
        internal int Sn { get; private set; }
        //Номер типа
        internal int NumType { get; private set; }
        //Подразделение
        internal int Appartment { get; private set; }
        //Номер выхода
        internal int Out { get; private set; }

        //Сигнал, содержащий значения самого выхода параметра
        internal ProviderSignal ValueSignal { get; private set; }
        //Сигнал недостоврности
        internal ProviderSignal StateSignal { get; private set; }
        //Сигнал ПОК
        internal ProviderSignal PokSignal { get; private set; }
        //Сигналы битов
        private DicI<ProviderSignal> _bitSignals;
        internal DicI<ProviderSignal> BitSignals { get { return _bitSignals ?? (_bitSignals = new DicI<ProviderSignal>()); } }
    }
}