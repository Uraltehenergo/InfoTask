using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;

namespace AuditMonitor
{
    public enum ELogType
    {
        Event = 0,           //Событие
        ParamsError = 1,     //Неверный параметр функций
        ErrorModuleSettings, //Ошибка чтения/установки конфигурации модуля
        ErrorModuleRead,     //Ошибка опроса модуля
        ErrorWriteArchive    //Ошибка при записи в архив 
    }
    
    public class ClassLog
    {
        public delegate void DelegateNewLogHandler(string message, DateTime time);

        public static event DelegateNewLogHandler EventNewLog;
        
        public static void AddLog(ELogType type, string function, string message, DateTime time)
        {
            if (EventNewLog != null) EventNewLog(message, time);
        }

        public static void AddLog(ELogType type, string function, string message)
        {
            AddLog(type, function, message, DateTime.Now);
        }

        //protected static void OnEventNewLog(string message, DateTime time)
        //{
        //    if (EventNewLog != null) EventNewLog(message, time);
        //}
    }
}
