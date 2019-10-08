using System;

namespace BaseLibrary
{
    //Базовый класс для комманд записи в SubHistory или History
    public abstract class CommandLogBase : Command
    {
        internal CommandLogBase(Logger logger, Command parent, double start, double finish, string name, string context)
            : base(logger, parent, start, finish, context)
        {
            StartTime = DateTime.Now;
            Name = name;
            BeginLog();
        }

        internal CommandLogBase(Logger logger, Command parent, string name, string context)
            : base(logger, parent, context)
        {
            StartTime = DateTime.Now;
            Name = name;
            BeginLog();
        }

        //Добавление в лог записи о команде
        protected abstract void BeginLog();

        //Текущий HistoryId или SubHistoryId
        internal int HistoryId { get; set; }

        //Имя комманды
        public string Name { get; private set; }

        //Время запуска комманды
        protected DateTime StartTime { get; private set; }
        //Разность времени между сейчас и стартом комманды
        protected double FromStart
        {
            get { return Math.Round(DateTime.Now.Subtract(StartTime).TotalSeconds, 2); }
        }

        //Строка для записи в лог состояния комманды
        protected string Status
        {
            get
            {
                if (IsBreaked) return "Прервано";
                if (!IsFinished) return "Запущено";
                return Quality.ToRussian();
            }
        }
    }
}