using System;

namespace BaseLibrary
{
    //Комманда для записи в History или SubHistory
    public class CommandLog : Command
    {
        public CommandLog(Logger logger, Command parent, CommandBehaviour behaviour, double start, double finish, bool isProgress)
            : base(logger, parent, start, finish, behaviour, isProgress)
        {
            StartTime = DateTime.Now;
            LogEventTime = DateTime.Now;
        }

        public CommandLog(Logger logger, Command parent, CommandBehaviour behaviour, bool isProgress)
            : base(logger, parent, behaviour, isProgress)
        {
            StartTime = DateTime.Now;
            LogEventTime = DateTime.Now;
        }

        //Время запуска комманды
        public DateTime StartTime { get; private set; }
        //Время логирования последнего события
        public DateTime LogEventTime { get; internal set; }

        //Поля из таблиц History или SubHistory
        public string Name { get; internal set; }
        public string Params { get; internal set; }
        //Контекст выполнения и отдельно его имя
        public string Context { get; internal set; }
        public string ContextName { get; internal set; }

        public DateTime PeriodBegin { get; internal set; }
        public DateTime PeriodEnd { get; internal set; }
        public string CalcName { get; internal set; }
        public string Mode { get; internal set; }

        //Разность времени между сейчас и стартом комманды
        public double FromStart
        {
            get { return Math.Round(DateTime.Now.Subtract(StartTime).TotalSeconds, 2); }
        }
        //Разность времени между сейчас и временем предыдущего логирования
        public double FromEvent
        {
            get { return Math.Round(DateTime.Now.Subtract(LogEventTime).TotalSeconds, 2); }
        }

        //Строка для записи в лог состояния комманды
        public string Status
        {
            get
            {
                if (IsBreaked) return "Прервано";
                if (!IsFinished) return "Запущено";
                if (IsError) return "Ошибка";
                if (IsRepeated) return "Повтор";
                if (Quality == CommandQuality.Warning) return "Предупреждение";
                return "Успешно";
            }
        }
    }
}
