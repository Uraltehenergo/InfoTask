using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "OvationOpcReceiver")]
    public class OvationOpcReceiver : OpcServer
    {
        //public OvationOpcReceiver(string name, string inf, Logger logger) : base(name, inf, logger) { }
        
        //Код
        public override string Code { get { return "OvationOPCReceiver"; }}
        protected override void GetAdditionalInf(DicS<string> inf) { }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return inf["CodeObject"];
        }
    }
}
