using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "KosmotronikaOpcReceiver")]
    public class KosmotronikaOpcReceiver : OpcServer 
    {
        //public KosmotronikaOpcReceiver(string name, string inf, Logger logger) : base(name, inf, logger) { }

        //Код
        public override string Code { get { return "KosmotronikaOPCReceiver"; } }
        //Серверная группа
        internal string ServerGroup { get; set; }
        //Загрузка дополнительных настроек провайдера из Inf
        protected override void GetAdditionalInf(DicS<string> inf)
        {
            ServerGroup = inf.Get("ServerGroup", "");
        }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return ServerGroup + ".point." + inf["SysNum"];
        }
    }
}
