using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "KosmotronikaRetroSource")]
    public class KosmotronikaRetroSource : KosmotronikaBaseSource
    {
        //Код провайдера
        public override string Code { get { return "KosmotronikaRetroSource"; } }
        //Настройки провайдера
        public override string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                DataSource = dic["RetroServerName"] ?? "";
                Hash = "RetroServer=" + DataSource;
            }
        }

        //Строка соединения с провайдером
        protected override string ConnectionString
        {
            get { return "Provider=RetroDB.RetroSQL;Data Source=" + DataSource; }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public override bool Check()
        {
            return Logger.Danger(Connect, 2, 500, "Не удалось определить временной диапазон Ретро-сервера");
        }

        //Проверка настроек
        public override string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return !inf["RetroServerName"].IsEmpty() ? "" : "Не задано имя Ретро-сервера";
        }

        //Проверка соединения
        public override bool CheckConnection()
        {
            if (Check() && GetTime() != null && TimeIntervals.Count > 0)
            {
                CheckConnectionMessage = "Успешное соединение. Диапазон источника: " + TimeIntervals[0].Begin + " - " + TimeIntervals[0].End;
                return true;
            }
            Logger.AddError(CheckConnectionMessage = "Ошибка соединения с Ретро-сервером");
            return false;
        }
    }
}
