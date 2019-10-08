using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectOvation : ProviderObject
    {
        internal ObjectOvation(int id, string code)
        {
            Id = id;
            Inf = code + "; Id=" + Id;
        }

        //Дискретный или аналоговый сигнал
        public SignalOvation ValueSignal { get; set; }
        //Биты упакованного сигнала
        private Dictionary<int, SignalOvation> _bitSignals;
        public Dictionary<int, SignalOvation> BitSignals { get { return _bitSignals ?? (_bitSignals = new Dictionary<int, SignalOvation>()); } }
        //Сигнал со словом состояния
        public SignalOvation StateSignal { get; set; }
        //Id в Historian
        public int Id { get; set; }

        //Для OvationHistorianSource

        //Нужно считывать значение (а не только состояние)
        public bool IsValue { get; set; }
        //Тип данных значения объекта
        public DataType DataType
        {
            get
            {
                if (ValueSignal != null) return ValueSignal.DataType;
                return DataType.Integer;
            }
        }
        //Значения среза на начало и конец последнего периода
        public Moment ValueBegin { get; set; }
        public Moment ValueEnd { get; set; }
        //Значения среза слова состояния на начало и конец последнего периода
        public Moment StatBegin { get; set; }
        public Moment StatEnd { get; set; }

        //Для OvationSource

        private static bool BeginEquals(SignalOvation sig, DateTime t)
        {
            if (sig.BeginMoment == null) return false;
            return Math.Abs(sig.BeginMoment.Time.Subtract(t).TotalSeconds) < 0.5;
        }

        //Возвращает, есть ли у объекта неопределенные срезы на время time 
        public bool HasBegin(DateTime time)
        {
            bool e = true;
            if (ValueSignal != null) e &= BeginEquals(ValueSignal, time);
            if (StateSignal != null) e &= BeginEquals(StateSignal, time);
            if (BitSignals != null && BitSignals.Count != 0)
                foreach (var b in BitSignals.Values)
                    e &= BeginEquals(b, time);
            return e;
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public int AddBegin()
        {
            int n = 0;
            if (ValueSignal != null) n += ValueSignal.AddBegin();
            if (StateSignal != null) n += StateSignal.AddBegin();
            if (BitSignals != null && BitSignals.Count != 0)
                foreach (var b in BitSignals.Values)
                    n += b.AddBegin();
            return n;
        }
    }

    //--------------------------------------------------------------------------------------------

    //Один сигнал (с учетом бита)
    internal class SignalOvation : ProviderSignal
    {
        internal SignalOvation(string signalInf, string code, DataType dataType, IProvider provider, int idInClone)
            : base(signalInf, code, dataType, provider, idInClone)
        {
            Id = Inf.GetInt("Id");
            Bit =  Inf.GetInt("Bit", -1);
            IsState = Inf["Prop"] == "STAT";
            Value = new SingleValue(SingleType.List);
        }

        //Если надо, то бит
        public int Bit { get; private set; }
        //Является сигналом состояния
        public bool IsState { get; private set; }
        //Id в Historian
        public int Id { get; set; }
    }
}