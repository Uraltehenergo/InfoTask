using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class ArchiveAccessSource : ArchiveSourceBase
    {
        public ArchiveAccessSource(string name, string inf, Logger logger) : base(name, inf, logger) { }

        //Код провайдера
        public override string Code { get { return "ArchiveAccessSource"; } }

        //Настройки провайдера
        public override string Inf
        {
            get { return ProviderInf; }
            set
            {
                if (ProviderInf != value)
                {
                    ProviderInf = value;
                    var dic = ProviderInf.ToPropertyDicS();
                    Hash = "AccessDb=" + dic["DatabaseFile"];
                    Archive = new AccessArchive(Name, Inf, ProviderSetupType.ReporterArchive, Logger);    
                }
            }
        }
        
         //Задание комманд, вызываемых из меню
        protected override void AddMenuCommands()
        {
            var m = new Dictionary<string, IMenuCommand>();
            m.Add("Выбрать файл", new DialogCommand(DialogType.OpenFile)
                {
                    DialogTitle = "Файл архива результатов расчета",
                    ErrorMessage = "Указан недопустимый файл архива",
                    FileTables = new[] {"Projects", "Reports", "Params", "ReportParams", "Ranges", "SingleValues", "BaseValues", "HourValues", "DayValues"}
                });
            MenuCommands.Add("DatabaseFile", m);
        }

        //Проверка настроек
        public override string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (inf["DatabaseFile"].IsEmpty()) err += "Не задан файл архива\n";
            return err;
        }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public override List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }
    }
}
