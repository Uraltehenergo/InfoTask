using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{   
    //Архив
    public class AccessArchive : ArchiveBase, IArchive
    {
        public AccessArchive(string name, string inf, ProviderSetupType setupType, Logger logger) 
            : base(DatabaseType.Access, name, inf,  setupType, logger)
        {
        }

        //Код провайдера
        public override string Code { get { return "AccessArchive"; } }
 
        //Задание комманд, вызываемых из меню
        protected override void AddMenuCommands()
        {
            var m = new Dictionary<string, IMenuCommand>();
            m.Add("Выбрать файл", new DialogCommand(DialogType.OpenFile)
                {
                    DialogTitle = "Файл архива результатов расчета",
                    ErrorMessage = "Указан недопустимый файл архива",
                    FileTables = new[] { "Projects", "Reports", "Params", "ReportParams", "Ranges", "SingleValues", "BaseValues" }
                });
            m.Add("Создать архив", new DialogCommand(DialogType.CreateFile)
                {
                    DialogTitle = "Создание нового файла архива результатов",
                    TemplateFile = Different.GetInfoTaskDir() + @"Providers\Archives\CalcArchiveTemplate.accdb"
                });
            MenuCommands.Add("DatabaseFile", m);

            m = new Dictionary<string, IMenuCommand>();
            m.Add("Выбрать каталог", new DialogCommand(DialogType.OpenDir)
                { DialogTitle = "Каталог для резервных копий архива" });
            MenuCommands.Add("ReserveDir", m);
        }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }
    }
}
