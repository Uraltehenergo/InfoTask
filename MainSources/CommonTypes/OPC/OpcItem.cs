﻿using BaseLibrary;

namespace CommonTypes
{
    //Один параметр для записи в OPC
    public class OpcItem : ProviderSignal
    {
        internal OpcItem(string signalInf, string code, DataType dataType, IProvider provider) 
            : base(signalInf, code, dataType, provider)
        {}

        //Стандартные свойства
        public string Tag { get; set; }
        public int ClientHandler { get; set; }
        public int ServerHandler { get; internal set; }
    }
}