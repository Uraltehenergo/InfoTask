using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Controller
{
    //Общая история для монитора расчетов
    public class MonitorHistory : ProviderBase, IProvider
    {
        //Тип
        public override ProviderType Type { get { return ProviderType.MonitorHistory;} }
        //Код
        public override string Code { get { return "MonitorHistory"; } }

        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                bool e = dic.Get("IndentType", "").ToLower() != "windows";
                string server = dic["SQLServer"], db = dic["Database"];
                SqlProps = new SqlProps(server, db, e, dic["Login"], dic["Password"]);
                Hash = "SQLServer=" + server + ";Database=" + db;
            }
        }

        //Проверка соединения
        public bool Check()
        {
            try { SqlDb.Connect(SqlProps); }
            catch { return false; }
            return true;
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера" + Different.NewLine;
            if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации" + Different.NewLine;
            if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин" + Different.NewLine;
            if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных" + Different.NewLine;
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            CheckConnectionMessage = "Не удалось соединиться с SQL-сервером";
            return false;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }
        
        //Настройки на базу SQL
        public SqlProps SqlProps { get; private set; }
        
        protected override void AddMenuCommands() { }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            try
            {
                if (propname == "Database" && props.ContainsKey("SQLServer") && !props["SQLServer"].IsEmpty() && (props["IndentType"].ToLower() == "windows" || props.ContainsKey("Login") && !props["Login"].IsEmpty()))
                    return SqlDb.SqlDatabasesList(props["SQLServer"], props["IndentType"].ToLower() != "windows", props["Login"], props["Password"]);
            }
            catch { }
            return new List<string>();
        }
        
        //Все соединения и так закрыты
        public void Dispose() { }

        //Добавить если надо в базу таблицу ошибок
        public void AddHistoryTables(int threadId) //Id потока
        {
            try
            {
                SqlDb.CreateTable(SqlProps, "ErrorsListTemplate", "ErrorsList" + threadId);
            }
            catch {}
        }

        //Добавить ошибку
        public void AddError(ThreadController thread, //Поток
                                         ErrorCommand er,  //Ошибка
                                         DateTime time, //Время возникновения
                                         CommandLog command) //Текущая команда
        {
            try
            {
                SqlDb.Execute(SqlProps,
                    "INSERT INTO ErrorsList" + thread.Id + "(ThreadId, ThreadDescription, ThreadProjects, PeriodBegin, PeriodEnd, Description, Params, Time, Command, Context) " +
                    "VALUES (" + thread.Id + ",'" + thread.Comment + "', '" + thread.ProjectsString + "', " + thread.PeriodBegin.ToSqlString() + ", " + thread.PeriodEnd.ToSqlString() + ",'"
                    + er.Text + "',  '" + er.Params + "', " + time.ToSqlString() + ", '" + command.Name + "' ,'" + er.Context + "')");
            }
            catch { }
        }
    }
}