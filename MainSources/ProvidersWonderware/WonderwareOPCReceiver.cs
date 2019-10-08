using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "WonderwareOPCReceiver")]
    public class WonderwareOPCReceiver : OpcServer
    {
        //public WonderwareOPCReceiver(string name, string inf, Logger logger) : base(name, inf, logger) { }
        //Код
        public override string Code { get { return "WonderwareOPCReceiver"; } }
        //Серверный узел
        internal string ServerNode { get; set; }
        //Серверная группа
        internal string ServerGroup { get; set; }
        protected override void GetAdditionalInf(DicS<string> inf)
        {
            ServerNode = inf.Get("ServerNode", "");
            ServerGroup = inf.Get("ServerGroup", "");
        }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return ServerNode + "." + ServerGroup + "." + inf["TagName"];
        }
    }
}