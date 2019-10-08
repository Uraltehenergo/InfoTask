using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "KosmotronikaArchDbSource")]
    public class KosmotronikaArchDbSource : KosmotronikaBaseSource
    {
        //Код провайдера
        public override string Code { get { return "KosmotronikaArchDbSource"; } }
        //Настройки провайдера
        public override string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                DataSource = dic["ArchiveDir"] ?? "";
                _location = dic.GetInt("Location");
                Hash = "ArchDbArchive=" + DataSource;
            }
        }

        //Временной сдвиг
        private int _location;

        //Строка соединения с провайдером
        protected override string ConnectionString
        {
            get { return "Provider=ArchDB.OpenSQL;Data Source=" + DataSource + ";Location=" + _location; }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public override bool Check()
        {
            return Logger.Danger(Connect, 2, 500, "Не удалось определить временной диапазон архива ПТК");
        }

        //Проверка настроек
        public override string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return !inf["ArchiveDir"].IsEmpty() ? "" : "Не задан путь к каталогу архива";
        }

        //Проверка соединения
        public override bool CheckConnection()
        {
            if (Check() && GetTime() != null && TimeIntervals.Count > 0)
            {
                CheckConnectionMessage = "Успешное соединение. Диапазон источника: " + TimeIntervals[0].Begin + " - " + TimeIntervals[0].End;
                return true;
            }
            Logger.AddError(CheckConnectionMessage = "Ошибка соединения с архивом ПТК Космотроника");
            return false;
        }
    }
}
