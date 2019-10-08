using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class ArchiveSQLServerSource : ArchiveSourceBase
    {
        public ArchiveSQLServerSource(string name, string inf, Logger logger) : base(name, inf, logger) { }

        //Код провайдера
        public override string Code { get { return "ArchiveSQLServerSource"; } }

        //Настройки провайдера
        public override string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = Inf.ToPropertyDicS();
                dic.DefVal = "";
                Hash = "SQLServer=" + dic["SQLServer"] + ";Database=" + dic["Database"];
                Archive = new SQLServerArchive(Name, Inf, ProviderSetupType.ReporterArchive, Logger);    
            }
        }

        protected override void AddMenuCommands() { }

        //Проверка настроек
        public override string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера\n";
            if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации\n";
            if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин\n";
            if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных\n";
            return err;
        }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public override List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            try
            {
                if (propname == "Database" && props.ContainsKey("SQLServer") && !props["SQLServer"].IsEmpty() && (props["IndentType"].ToLower() == "windows" || (props.ContainsKey("Login") && !props["Login"].IsEmpty())))
                    return SqlDb.SqlDatabasesList(props["SQLServer"], props["IndentType"].ToLower() != "windows", props["Login"], props["Password"]);
            }
            catch { }
            return new List<string>();
        }
    }
}
