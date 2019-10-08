using System;
using BaseLibrary;
using Calculation;

namespace ControllerClient
{
    //Класс, вызываемый через Com
    //Все функции возвращают ошибку или ""
    public class Cloner
    {
        //Открыть поток клонирования, appliction - тип коммуникатора и т.п.
        public string OpenLocal(string application, bool showIndicator = true)
        {
            _thread = new ThreadCloner(application, showIndicator);
            return _thread.ErrMessage;
        }

        //Поток клонирования
        private ThreadCloner _thread;

        //Получение времени источников
        public string RunSource(string code, string name, string inf)
        {
            _thread.RunSource(code, name, inf);
            return _thread.ErrMessage;
        }

        //Начало и конец периода источников
        public DateTime SourceBegin { get { return _thread.SourceBegin; } }
        public DateTime SourceEnd { get { return _thread.SourceEnd; } }
        
        //Закрытие потока клонирования
        public string Close()
        {
            try
            {
                _thread.Close();
                return _thread.ErrMessage;    
            }
            finally
            {
                GC.Collect();
            }
        }

        //Создание клона от timeBegin до timeEnd, cloneFile - файл клона
        public string MakeClone(string cloneFile, DateTime timeBegin, DateTime timeEnd)
        {
            return _thread.MakeClone(cloneFile, timeBegin, timeEnd);
        }
    }
}
