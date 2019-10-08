using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary
{
    //Одна комманда
    public class Command : IDisposable
    {
        //Указатель на родителя
        public Command Parent { get; private set; }
        //Указатель на Logger
        internal Logger Logger { get; set; }
        //Ошибки, возвращаемые коммандой
        private readonly List<ErrorCommand> _errors = new List<ErrorCommand>();
        public List<ErrorCommand> Errors { get { lock(_errorLock) return _errors; } }
        //True, если ошибка
        public bool IsError { get { return Quality == CommandQuality.Error; } }
        //Ошибочность комманды
        private CommandQuality _quality;
        public CommandQuality Quality
        {
            get { lock (_errorLock) return _quality;}
            private set { lock(_errorLock ) _quality = value; }
        }
        //Совокупное сообщение об ошибках
        //Добавляются в описание: addContext - контекст ошибки, addParams - параметры, addErrType - Ошибка или Предупреждение
        public string ErrorMessage(bool addContext = true, bool addParams = true, bool addErrType = true)
        {
            string s = "";
            foreach (var e in Errors.Where(e => e.IsFatal))
            {
                s += (s != "" ? Different.NewLine : "");
                if (addErrType) s += "Ошибка: ";
                s += e.Text;
                if (addContext && !e.Context.IsEmpty()) s += "; " + e.Context;
                if (addParams && !e.Params.IsEmpty()) s += "; " + e.Params;    
            }
            foreach (var e in Errors.Where(e => !e.IsFatal))
            {
                s += (s != "" ? Different.NewLine : "");
                if (addErrType) s += "Предупреждение: ";
                s += e.Text;
                if (addContext && !e.Context.IsEmpty()) s += "; " + e.Context;
                if (addParams && !e.Params.IsEmpty()) s += "; " + e.Params;
            }
            return s;
        }

        //Объекты для блокировки ошибок и процентов потоком
        private readonly object _errorLock = new object();
        private readonly object _procentLock = new object();

        //Комманда коллекционирует сообщения для прользователя
        public bool IsProgress { get; private set; }
        //Тип комманды
        public CommandBehaviour Behaviour { get; private set; }
        
        //Величина
        private double _procent;
        public double Procent
        {
            get { lock (_procentLock) return _procent;}
            set
            {
                lock (_procentLock)
                {
                    _procent = value;
                    if (Parent != null)
                        Parent.Procent = StartProcent + value * (FinishProcent - StartProcent) / 100;
                }
            }
        }
        //Каким значениям родителя соответствует 0% и 100% счетчика
        internal double StartProcent { get; private set; }
        internal double FinishProcent { get; private set; }

        //Команда завершена
        public bool IsFinished { get; internal set; }
        //Внутри комманды был выполнен повтор опасной операции
        private bool _isRepeated;
        public bool IsRepeated
        {
            get { return _isRepeated; }
            set 
            { 
                _isRepeated = false;
                if (Parent != null) Parent.IsRepeated = true;
            }
        }
        //Комманда была прервана
        public bool IsBreaked { get; internal set; }
        
        //Создание новой комманды, 
        //parent - родитель, start, finish - проценты от родителя, 
        //isDanger - задает опасную операцию, isProgress - процент отображается на индикаторе
        //context - контекст в котором выполняется комманда (проект, провайдер и т.п.)
        public Command(Logger logger, Command parent, double start, double finish, CommandBehaviour behaviour, bool isProgress)
        {
            Logger = logger;
            Parent = parent;
            StartProcent = start;
            FinishProcent = finish;
            Behaviour = behaviour;
            IsProgress = isProgress;
            Quality = CommandQuality.Success;
            Procent = 0;
        }

        //Создание новой комманды без указания процентов
        //parent - родитель, isDanger - задает опасную операцию, isProgress - процент отображается на индикаторе
        //context - контекст в котором выполняется комманда (проект, провайдер и т.п.)
        public Command(Logger logger, Command parent, CommandBehaviour behaviour, bool isProgress)
        {
            Logger = logger;
            Parent = parent;
            if (parent == null)
            {
                StartProcent = 0;
                FinishProcent = 100;    
            }
            else
            {
                StartProcent = Parent.Procent;
                FinishProcent = Parent.Procent;
            }
            Behaviour = behaviour;
            IsProgress = isProgress;
            Quality = CommandQuality.Success;
            Procent = 0;
        }

        //Добавляет ошибку er в комманду
        public ErrorCommand AddError(ErrorCommand er)
        {
            foreach (var e in Errors)
                if (e.Text == er.Text && e.Params == er.Params && e.IsFatal == er.IsFatal)
                    return e;
            Errors.Add(er);
            if (er.Context.IsEmpty())
            {
                var c = this;
                while (c != null && !(c is CommandLog))
                    c = c.Parent;
                if (c != null) er.Context = ((CommandLog)c).Context;    
            }
            if (er.IsFatal) Quality = CommandQuality.Error;
            else if (Quality == CommandQuality.Success) Quality = CommandQuality.Warning;
            return er;
        }

        public void Dispose()
        {
            try { Logger.Finish(); }
            catch { }
        }
    }
}
