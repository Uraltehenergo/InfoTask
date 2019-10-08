using System;
using BaseLibrary;

namespace Calculation
{
    //Класс для переопределения операций с коммандами логирования
    public abstract class ExternalThreadLogger : ExternalLogger
    {
        protected ExternalThreadLogger(ThreadCalc thread) : base(thread)
        {
            ThreadCalc = thread;
        }

        //Поток
        private ThreadCalc _thread; 
        public ThreadCalc ThreadCalc
        {
            get { return _thread; }
            set { Logger = _thread = value; }
        }

        //Имя объекта для вывода на табло
        protected abstract string LoggerObject { get; }

        //Запуск атомарной комманды
        protected Command StartAtom(Atom atom, double start = 0, double finish = 100, string context = "", string objectName = "")
        {
            return ThreadCalc.StartAtom(atom, start, finish, context.IsEmpty() ? Context : context, objectName.IsEmpty() ? LoggerObject : objectName);
        }
        //Запуск атомарной комманды и выполнение действия action, возвращает false, если ошибка
        protected bool StartAtom(Atom atom, Action action, double start = 0, double finish = 100, string context = "", string objectName = null)
        {
            return ThreadCalc.StartAtom(atom, action, start, finish, context.IsEmpty() ? Context : context, objectName.IsEmpty() ? LoggerObject : objectName);
        }
        //Запуск комманды отображения
        protected Command StartView(ViewAtom viewAtom, bool useSubLog)
        {
            return ThreadCalc.StartView(viewAtom, useSubLog);
        }
        //Запуск комманды отображения и выполнение действия action, возвращает false, если ошибка
        protected bool StartView(ViewAtom viewAtom, Action action, bool useSubLog)
        {
            return ThreadCalc.StartView(viewAtom, action, useSubLog);
        }
    }
}