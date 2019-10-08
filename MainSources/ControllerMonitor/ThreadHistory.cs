using System;
using BaseLibrary;

namespace ControllerMonitor
{
    internal class ThreadHistory
    {
        internal ThreadHistory(int id)
        {
            Id = id;
            _props = Program.MonitorHistory.SqlProps;
            LastTime = Different.MinDate;
        }

        //Id потока
        internal int Id { get; private set; }
        //Описание потока
        internal string Description { get; private set; }
        //Список проектов потока
        internal string Projects { get; private set; }
        
        //Время последней ошибки
        internal DateTime LastTime { get; private set; }
        //Сообщение последней ошибки
        internal string LastText { get; private set; }
        //Настрйки базы данных
        private readonly SqlProps _props;

        //Загрузка из таблицы истории
        internal void LoadFromDb()
        {
            string table = " ErrorsList" + Id;
            using (var rec = new ReaderAdo(_props, "SELECT Time, Description, ThreadDescription, ThreadProjects FROM" + table + " ORDER BY" + table + ".Time DESC"))
                if (rec.HasRows())
                {
                    LastTime = rec.GetTime("Time");
                    LastText = rec.GetString("Description");
                    Description = rec.GetString("ThreadDescription");
                    Projects = rec.GetString("ThreadProjects");
                }
        }
    }
}