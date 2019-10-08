using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace BaseLibrary
{
    //Интерфейс, логирующий и отображающий комманды и ошибки
    public abstract class Logger
    {
        //Задание файла истории historyFile и файла его шаблона historyTemplate, открытие истории, useSubHistory - использовать SubHistory
        public void OpenHistory(string historyFile = null, string historyTemplate = null, bool useSubHistory = false, bool useErrorsList = true)
        {
            try
            {
                _historyFile = historyFile;
                _historyTemplate = historyTemplate;
                _useSubHistory = useSubHistory;
                _useErrorsList = useErrorsList;
                if (_historyFile != null)
                {
                    if (_historyTemplate != null && DaoDb.FromTemplate(_historyTemplate, _historyFile, ReplaceByTemplate.IfNewVersion, true))
                            _reasonUpdate = "Новая версия файла истории";
                    OpenHistoryRecs();
                }
            }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Сохранение старого файла истории и добавление нового
        public void UpdateHistory(bool openAfterUpdate)
        {
            try
            {
                if (LastHistoryId > 300000)
                    _reasonUpdate = "Старый файл истории содержит более 300000 записей";
                    
                if (_reasonUpdate != null && HistoryIsStable)
                {
                    CloseHistory();
                    Thread.Sleep(1500);
                    DaoDb.FromTemplate(_historyTemplate, _historyFile, ReplaceByTemplate.Always, true);
                    Thread.Sleep(1500);
                    if (openAfterUpdate) OpenHistoryRecs();
                }
            }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Закрывает историю, если надо то копирует новый файл
        public void CloseHistory()
        {
            try
            {
                if (SubHistory != null) SubHistory.Dispose();
                if (History != null) History.Dispose();
                if (SuperHistory != null) SuperHistory.Dispose();
                if (ErrorsRec != null) ErrorsRec.Dispose();
                if (HistoryDb != null) HistoryDb.Dispose();
                SubHistory = null;
                History = null;
                SuperHistory = null;
                ErrorsRec = null;
                HistoryDb = null;
            }
            catch { }
        }

        //Открытие рекордсетов истории и добавление в историю первой записи после создания
        private void OpenHistoryRecs()
        {
            HistoryIsStable = false;
            HistoryDb = new DaoDb(_historyFile);
            History = new RecDao(HistoryDb, "History");
            SuperHistory = new RecDao(HistoryDb, "SuperHistory");
            if (_useSubHistory) SubHistory = new RecDao(HistoryDb, "SubHistory");
            if (_useErrorsList) ErrorsRec = new RecDao(HistoryDb, "ErrorsList");
            if (_reasonUpdate != null)
            {
                SubHistoryId = 0;
                try
                {
                    StartLog("Создание нового файла истории", _reasonUpdate).Dispose();
                    _reasonUpdate = null;
                }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
                LastHistoryId = 0;
            }
        }

        //Вызывается, если при работе с историей произошла ошибка
        void AddErrorAboutHistory(Exception ex)
        {
            _reasonUpdate = "Ошибка при работе с файлом истории.\n" + ex.Message + "\n" + ex.StackTrace;
            UpdateHistory(true);
        }

        //Описание потока выполнения
        private string _threadName;
        public string ThreadName
        {
            get { return _threadName; }
            protected set
            {
                _threadName = value;
                FullThreadName = value + "_" + Dns.GetHostName();
            }
        }
        //Полное описание потока, с указанием имени компьютера
        public string FullThreadName { get; private set; }

        //Файл истории и шаблон для него
        private string _historyFile;
        private string _historyTemplate;
        //База данных History
        protected DaoDb HistoryDb;
        //Строка с причиной создания нового файла истории
        private string _reasonUpdate;
        //Рекордсет с таблицей SubHistory
        public RecDao SubHistory { get; private set; }
        //Использовать SubHistory
        private bool _useSubHistory;
        //Использовать ErrorsList
        private bool _useErrorsList;
        //Текущий HistoryId
        public int SubHistoryId { get; set; }
        //Рекордсет с таблицей History
        public RecDao History { get; private set; }
        //Текущий HistoryId и предыдущий
        public int HistoryId { get; set; }
        public int LastHistoryId { get; set; }
        //История стабильно работает (HistoryId > 1)
        public bool HistoryIsStable { get; set; }
        //Рекордсет с таблицей SuperHistory
        public RecDao SuperHistory { get; set; }
        //Рекордсет с таблицей ErrorsList
        public RecDao ErrorsRec { get; set; }
        //True, если операции выполняются без присутствия пользователя (периодический режим)
        public bool IsRepeat{ get; protected set; }

        //Объект для синхронизации потоков
        public Synchro Synchro { get; set; }

        //Переопределение комманд Synchro
        public void StartProcess(string process, int maxCount = 1) 
        {
            if (Synchro != null)
                Synchro.StartProcess(process, maxCount);
        }

        public void StartProcess(string process, int maxCount, string process2, int maxCount2 = 1)
        {
            if (Synchro != null)
                Synchro.StartProcess(process, maxCount, process2, maxCount2);
        }
        
        public void StartProcess(string process, params Tuple<string, int>[] bounds)
        {
            if (Synchro != null)
                Synchro.StartProcess(process, bounds);
        }

        public void FinishProcess(string process)
        {
            if (Synchro != null)
                Synchro.FinishProcess(process);
        }

        //Текущая выполняемая комманда
        private Command _command;
        public Command Command 
        {
            get { return _command; }
            private set
            {
                _command = value;
                if (_command != null)
                {
                    if (_command.IsProgress) CommandProgress = _command;
                    if (_command.Behaviour == CommandBehaviour.Log) CommandLog = (CommandLog)Command;
                    if (_command.Behaviour == CommandBehaviour.SubLog) CommandSubLog = (CommandLog)Command;    
                }
            } 
        }
        //Текущая комманда, записывающая в History
        public CommandLog CommandLog { get; private set; }
        //Текущая комманда, записывающая в SubHistory
        public CommandLog CommandSubLog { get; private set; }
        //Комманда, отображающая индикатор
        public Command CommandProgress { get; private set; }

        //Запуск простой комманды с процентами и без
        public Command Start(double start, double finish = 100, bool isProgress = false, bool isDanger = false)
        {
            return Command = new Command(this, Command, start, finish, isDanger ? CommandBehaviour.Danger : CommandBehaviour.Simple, isProgress);
        }
        public Command Start(bool isProgress = false, bool isDanger = false)
        {
            return Command = new Command(this, Command, isDanger ? CommandBehaviour.Danger : CommandBehaviour.Simple, isProgress);
        }
        //Запуск простой комманды для записи в History с процентами и без
        public CommandLog StartLog(double start, double finish, string name, string pars = "", string context = "", string contextName = "", bool isProgress = false)
        {
            Command = new CommandLog(this, Command, CommandBehaviour.Log, start, finish, isProgress) 
                { Name = name, Params = pars, Context = context, ContextName = contextName};
            BeginLogCommand();
            return CommandLog;
        }
        public CommandLog StartLog(string name, string pars = "", string context = "", string contextName = "", bool isProgress = false)
        {
            Command = new CommandLog(this, Command, CommandBehaviour.Log, isProgress) 
                { Name = name, Params = pars, Context = context, ContextName = contextName};
            BeginLogCommand();
            return CommandLog;
        }
        //Запуск простой комманды для записи в SubHistory с процентами и без
        public CommandLog StartSubLog(double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, string calcName = "", bool isProgress = false)
        {
            Command = new CommandLog(this, Command, CommandBehaviour.SubLog, start, finish, isProgress)
                { Name = name, PeriodBegin = periodBegin, PeriodEnd = periodEnd, Mode = mode, CalcName = calcName };
            BeginSubLogCommand();
            return CommandSubLog;
        }
        public CommandLog StartSubLog(string name, DateTime periodBegin, DateTime periodEnd, string mode, string calcName = "", bool isProgress = false)
        {
            Command = new CommandLog(this, Command, CommandBehaviour.SubLog, isProgress) 
                { Name = name, PeriodBegin = periodBegin, PeriodEnd = periodEnd, Mode = mode, CalcName = calcName };
            BeginSubLogCommand();
            return CommandSubLog;
        }
        //Такие же запуски комманд только с выполнением действия и завершением комманды
        //Возвращают true, если все выполнилось успешно
        public bool Start(Action action, double start, double finish = 100, bool isProgress = false, bool isDanger = false)
        {
            using (var c = Start(start, finish, isProgress, isDanger))
            {
                action();
                return !c.IsError;
            }
        }
        public bool Start(Action action, bool isProgress = false, bool isDanger = false)
        {
            using (var c = Start(isProgress, isDanger))
            {
                action();
                return !c.IsError;
            }
        }
        public bool StartLog(Action action, double start, double finish, string name, string pars = "", string context = "", string contextName = "", bool isProgress = false)
        {
            using (var c = StartLog(start, finish, name, pars, context, contextName, isProgress))
            {
                action();
                return !c.IsError;
            }
        }
        public bool StartLog(Action action, string name, string pars = "", string context = "", string contextName = "", bool isProgress = false)
        {
            using (var c = StartLog(name, pars, context, contextName, isProgress))
            {
                action();
                return !c.IsError;
            }
        }
        
        //Завершает комманду, results - результаты для записи в лог или отображения, возвращает команду
        public Command Finish(string results = null, bool isBreaked = false)
        {
            var c = Command;
            c.Procent = 100;
            c.IsFinished = true;
            c.IsBreaked = isBreaked;
            if (c.IsProgress)
            {
                FinishProgressCommand();
                CommandProgress = null;
            }
            if (c.Behaviour == CommandBehaviour.Log)
            {
                EndLogCommand(results);
                FinishLogCommand();
                CommandLog = null;
            }
            if (c.Behaviour == CommandBehaviour.SubLog)
            {
                EndSubLogCommand();
                FinishSubLogCommand();
                CommandSubLog = null;
            }
            Command = c.Parent;
            return c;
        }

        //Запускает опасную операцию, Однопоточный вариант
        //Возвращает true, если операция прошла успешно (может не с первого раза)
        //operation - операция, которую нужно выполнить, возвращает True если все успешно
        //repetitions - сколько раз повторять, если не удалась (вместе с первым)
        //errorOperation - операция, выполняемя между повторами, errorWaiting - сколько мс ждать при ошибке
        public bool Danger(Func<bool> operation, int repetitions, int errorWaiting = 0, string errMess = "Не удалось выполнить опасную операцию", Func<bool> errorOperation = null)
        {
            //Выполняем первый раз
            var dan = RunDanger(operation, null);
            if (!dan.IsError) return true;

            bool b = true;
            ErrorCommand err = dan.Errors.First();
            for (int i = 2; i <= repetitions && b; i++)
            {   //Выполняем последующие разы
                Command.IsRepeated = true;
                if (errorWaiting < 3000) Thread.Sleep(errorWaiting);
                else
                {
                    double p = Procent;
                    for (int j = 0; j < errorWaiting / 500; j++)
                    {
                        Procent = j * 100 * (500.0 / errorWaiting);
                        Thread.Sleep(500);    
                    }
                    Procent = p;
                }
                AddEvent("Повтор опасной операции", i + "-й раз");
                b &= RunDanger(operation, errorOperation).IsError;
            }
            if (b && CommandLog != null)
            {
                var lerr = new ErrorCommand(errMess);
                CommandLog.AddError(lerr);
                LogError(lerr);
                if (_useErrorsList)
                {
                    LogToErrorsList(lerr);
                    MessageError(lerr);
                }
                var c = CommandLog.Parent;
                while (c != null)
                {
                    c.AddError(err);
                    c = c.Parent;
                }
            }
            return !b;
        }

        //Запуск опасной операции, возвращает комманду
        private Command RunDanger(Func<bool> operation, Func<bool> errorOperation)
        {
            using (var c = Start(0, 100, false, true))
            {
                try
                {
                    if (errorOperation == null || errorOperation())
                        operation();
                }
                catch (Exception ex)
                {
                    AddEvent("Объем используемой памяти", GC.GetTotalMemory(false).ToString()); 
                    AddError("Ошибка при опасной операции", ex);
                }
                return c;
            }
        }


        //Добавляет ошибку в комманду, лог и отображение, er - ошибка, 
        public void AddError(ErrorCommand er)
        {
            bool danger = false;
            var c = Command;
            while (c != null)
            {
                danger |= c.Behaviour == CommandBehaviour.Danger;
                if (c.Behaviour == CommandBehaviour.Log)
                {
                    LogError(er);
                    if (danger) break;
                    if (_useErrorsList)
                    {
                        LogToErrorsList(er);
                        MessageError(er);
                    }
                }
                c.AddError(er);
                c = c.Parent;
            }
        }
        //text - текст ошибки, ex - исключение, par - праметры ошибки, isFatal - ошибка или предупреждение
        public void AddError(string text, Exception ex = null, string par = "", string context = "", bool isFatal = true)
        {
            AddError(new ErrorCommand(text, ex, par, context, isFatal));
        }

        //Процент текущей комманды
        public double Procent
        {
            get { return Command.Procent; }
            set
            {
                Command.Procent = value;
                if (CommandProgress != null)
                    ViewProcent(CommandProgress.Procent);
            }
        }

        //Запись комманды в History
        private void BeginLogCommand()
        {
            if (History != null)
            {
                try
                {
                    History.AddNew();
                    if (SubHistoryId > 0) History.Put("SubHistoryId", SubHistoryId);
                    History.Put("Command", CommandLog.Name);
                    History.Put("Context", CommandLog.Context, true);
                    History.Put("Params", CommandLog.Params);
                    History.Put("Time", CommandLog.StartTime);
                    History.Put("Status", CommandLog.Status);
                    LastHistoryId = HistoryId = History.GetInt("HistoryId");
                    History.Update();
                    HistoryIsStable = true;
                }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
            }
        }

        //Запись комманды в SubHistory
        private void BeginSubLogCommand()
        {
            if (SubHistory != null)
            {
                try
                {
                    SubHistory.AddNew();
                    SubHistory.Put("Command", CommandSubLog.Name);
                    SubHistory.Put("Time", CommandSubLog.StartTime);
                    SubHistory.Put("Status", CommandSubLog.Status);
                    SubHistory.Put("PeriodBegin", CommandSubLog.PeriodBegin);
                    SubHistory.Put("PeriodEnd", CommandSubLog.PeriodEnd);
                    SubHistory.Put("Mode", CommandSubLog.Mode);
                    SubHistory.Put("CalcName", CommandSubLog.CalcName);
                    SubHistoryId = SubHistory.GetInt("SubHistoryId");
                    SubHistory.Update();    
                }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
            }
        }

        //Записать результат комманды в History
        private void EndLogCommand(string results = "")
        {
            if (History != null)
            {
                try
                {
                    History.MoveLast();
                    History.Put("Status", CommandLog.Status);
                    string s = "";
                    if (results.IsEmpty())
                    {
                        if (!CommandLog.Params.IsEmpty())
                            s = CommandLog.Params;
                    }
                    else
                    {
                        if (CommandLog.Params.IsEmpty()) s = results;
                        else s = CommandLog.Params + "; " + results;
                    }
                    History.Put("Params", s);
                    History.Put("ProcessLength", CommandLog.FromStart);
                    History.Update();
                    HistoryId = 0;    
                }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
            }
        }

        //Записать результат комманды в SubHistory
        private void EndSubLogCommand()
        {
            if (SubHistory != null)
            {
                try
                {
                    if (SubHistory.HasRows())
                    {
                        SubHistory.MoveLast();
                        SubHistory.Put("Status", CommandSubLog.Status);
                        SubHistory.Put("ProcessLength", CommandSubLog.FromStart);
                        SubHistory.Update();         
                    }
                }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
            }
        }

        //Добавляет Ошибку в лог
        private void LogError(ErrorCommand er)
        {
            try
            {
                if (SuperHistory != null)
                {
                    SuperHistory.AddNew();
                    if (HistoryId > 0) SuperHistory.Put("HistoryId", HistoryId);
                    SuperHistory.Put("Description", er.Text);
                    SuperHistory.Put("Params", er.ToLog());
                    SuperHistory.Put("Time", DateTime.Now);
                    if (CommandLog != null)
                        SuperHistory.Put("FromStart", CommandLog.FromEvent);
                    SuperHistory.Put("Status", "Ошибка");
                    SuperHistory.Update();
                }
            }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Добавляет событие в лог
        public void AddEvent(string description, string pars = "")
        {
            try
            {
                if (SuperHistory != null && HistoryId > 0)
                {
                    SuperHistory.AddNew();
                    //if (HistoryId > 0) 
                    SuperHistory.Put("HistoryId", HistoryId);
                    SuperHistory.Put("Description", description);
                    SuperHistory.Put("Params", pars);
                    SuperHistory.Put("Time", DateTime.Now);
                    if (CommandLog != null)
                    {
                        SuperHistory.Put("FromStart", CommandLog.FromEvent);
                        CommandLog.LogEventTime = DateTime.Now;
                    }
                    SuperHistory.Update();
                }
            }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);    
            }
        }

        //Добавляет ошибку в ErrorsList
        private void LogToErrorsList(ErrorCommand er)
        {
            try
            {
                DateTime t = DateTime.Now;
                if (ErrorsRec != null)
                {
                    ErrorsRec.AddNew();
                    ErrorsRec.Put("Description", er.Text);
                    ErrorsRec.Put("Params", er.ToLog());
                    ErrorsRec.Put("Time", t);
                    if (CommandLog != null)
                    {
                        ErrorsRec.Put("Command", CommandLog.Name);    
                        ErrorsRec.Put("Context", CommandLog.Context);
                    }
                    if (CommandSubLog != null)
                    {
                        ErrorsRec.Put("PeriodBegin", CommandSubLog.PeriodBegin);
                        ErrorsRec.Put("PeriodEnd", CommandSubLog.PeriodEnd);    
                    }
                    ErrorsRec.Update();
                }
                LogErrorsSpecial(er, t, CommandLog);
            }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Запись в ошибки SQL-базу
        protected virtual void LogErrorsSpecial(ErrorCommand er, DateTime time, CommandLog command) { }

        //Вызываются при завершении комманд
        protected abstract void FinishLogCommand();
        protected abstract void FinishSubLogCommand();
        protected abstract void FinishProgressCommand();

        //Добавляет Ошибку в сообщения
        protected abstract void MessageError(ErrorCommand er);
        //Отображает процент на индикаторе процесса
        protected abstract void ViewProcent(double procent);
    }

    //---------------------------------------------------------------------------------
    //Простой логгер, который ничего не делает
    public class EmptyLogger : Logger
    {
        protected override void FinishLogCommand() { }
        protected override void FinishSubLogCommand() { }
        protected override void FinishProgressCommand() { }
        protected override void MessageError(ErrorCommand er) { }
        protected override void ViewProcent(double procent) { }
    }
}