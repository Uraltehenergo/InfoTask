using BaseLibrary;

namespace Calculation
{
    public class ProviderApp : Provider
    {
        internal ProviderApp(IRecordRead rec, ThreadCalc thread) : base(rec, thread) { }

        //Код провайдера
        public override string Code { get; set; }
        public override string Inf { get; set; }
        public override bool Otm { get; set; }
    }
}