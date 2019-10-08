using System.Collections.Generic;
using CommonTypes;

namespace ProvidersLogika
{
    //Один объект в программе Пролог
    public class ObjectProlog : ProviderObject
    {
        public ObjectProlog(string tableName, int nodeId, string prop)
        {
            TableName = tableName;
            NodeId = nodeId;
            Prop = prop;
            Inf = "TableName=" + tableName + "; NodeId=" + nodeId + "; Prop=" + prop;
        }

        //Имя таблицы значений
        internal string TableName { get; private set; }
        //Id узла (объекта)
        internal int NodeId { get; private set; }
        //Имя свойства (Tatal или null)
        internal string Prop { get; private set; }
        
        //Сигналы
        private readonly Dictionary<string, ProviderSignal> _signals = new Dictionary<string, ProviderSignal>();
        internal Dictionary<string, ProviderSignal> Signals { get { return _signals; } }

        //Добавить к объекту сигнал, если такого еще не было
        public ProviderSignal AddSignal(ProviderSignal sig)
        {
            var code = sig.Inf["SignalCode"];
            if (!Signals.ContainsKey(code))
                Signals.Add(code, sig);
            return Signals[code];
        }
    }
}