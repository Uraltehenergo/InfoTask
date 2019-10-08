using System;

namespace BaseLibrary
{
    //Класс для переопределения операция логгера
    public abstract class ExternalLogger
    {
        protected ExternalLogger(Logger logger)
        {
            Logger = logger;
        }

        //Ссылка на логгер
        public Logger Logger { get; set; }

        //Контекст заданный по умолчанию
        public abstract string Context { get; }

        protected double Procent
        {
            get { return Logger.Procent; }
            set { Logger.Procent = value; }
        }

        protected void AddEvent(string description, double procent)
        {
            Logger.AddEvent(description, procent);
        }
        protected void AddEvent(string description, string pars = "")
        {
            Logger.AddEvent(description, pars);
        }
        protected void AddEvent(string description, string pars, double procent)
        {
            Logger.AddEvent(description, pars, procent);
        }

        protected void AddError(string text, Exception ex = null, string pars = "", string context = "")
        {
            Logger.AddError(text, ex, pars, context.IsEmpty() ? Context : context);
        }
        protected void AddWarning(string text, Exception ex = null, string pars = "", string context = "")
        {
            Logger.AddWarning(text, ex, pars, context.IsEmpty() ? Context : context);
        }

        protected Command Command { get { return Logger.Command; } }

        protected Command Start(double start, double finish = 100, string context = "")
        {
            return Logger.Start(start, finish, context);
        }
        protected Command Start(string context = "")
        {
            return Logger.Start(context);
        }
        
        public CommandProgress StartProgress(string context = "")
        {
            return Logger.StartProgress(context);
        }
        
        public CommandLog StartLog(double start, double finish, string name, string pars = "", string context = "")
        {
            return Logger.StartLog(start, finish, name, pars, context);
        }
        public CommandLog StartLog(string name, string pars = "", string context = "")
        {
            return Logger.StartLog(name, pars, context);
        }

        public CommandSubLog StartSubLog(double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, string context = "")
        {
            return Logger.StartSubLog(start, finish, name, periodBegin, periodEnd, mode, context);
        }
        public CommandSubLog StartSubLog(string name, DateTime periodBegin, DateTime periodEnd, string mode, string context = "")
        {
            return Logger.StartSubLog(name, periodBegin, periodEnd, mode, context);
        }

        public bool Start(Action action, double start, double finish, string errMess = "Ошибка")
        {
            return Logger.Start(action, start, finish, errMess);
        }
        public bool Start(Action action, string errMess = "Ошибка")
        {
            return Logger.Start(action, errMess);
        }

        public bool StartProgress(Action action, string errMess = "Ошибка")
        {
            return Logger.StartProgress(action, errMess);
        }
        
        public bool StartLog(Action action, double start, double finish, string name, string pars = "", string context = "", string errMess = "Ошибка")
        {
            return Logger.StartLog(action, start, finish, name, pars, context, errMess);
        }
        public bool StartLog(Action action, string name, string pars = "", string context = "", string errMess = "Ошибка")
        {
            return Logger.StartLog(action, name, pars, context, errMess);
        }

        public bool StartSubLog(Action action, double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, string errMess = "Ошибка")
        {
            return Logger.StartSubLog(action, start, finish, name, periodBegin, periodEnd, mode, errMess);
        }
        public bool StartSubLog(Action action, string name, DateTime periodBegin, DateTime periodEnd, string mode, string errMess = "Ошибка")
        {
            return Logger.StartSubLog(action, name, periodBegin, periodEnd, mode, errMess);
        }

        public Command Finish(string results = null, bool isBreaked = false)
        {
            return Logger.Finish(results, isBreaked);
        }
        
        public bool Danger(Func<bool> operation, int repetitions, int errorWaiting = 0, string errMess = "Не удалось выполнить опасную операцию", Func<bool> errorOperation = null)
        {
            return Logger.Danger(operation, repetitions, errorWaiting, errMess, errorOperation);
        }
    }
}