using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class SQLServerArchive : ArchiveBase, IArchive
    {
        public SQLServerArchive(string name, string inf, ProviderSetupType setupType, Logger logger) 
            : base(DatabaseType.SqlServer, name, inf, setupType, logger)
        {
            Name = name;
        }

        //Код провайдера
        public override string Code { get { return "SQLServerArchive"; } }
       //Задание комманд, вызываемых из меню
        protected override void AddMenuCommands(){}

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
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