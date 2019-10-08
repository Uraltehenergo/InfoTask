using System;
using BaseLibrary;

namespace Calculation
{
    //Параметр ручного ввода или редактирования абсолютных значений со значением
    public class HandInputParam
    {
        public HandInputParam(string project, string code, string val, DateTime time)
        {
            Project = project;
            Code = code;
            Value = val;
            Time = time;
        }

        //Если isAbsolute, то в Moment добавляется время, если isString, то значения берутся из StrValue и TimeValue
        public HandInputParam(IRecordRead rec)
        {
            Project = rec.GetString("Project");
            Code = rec.GetString("Code");
        }

        //Записываеn в рекордсет, если isAbsolute, то записывает и время
        public void ToRecordset(IRecordAdd rec, bool isAbsolute)
        {
            rec.AddNew();
            rec.Put("Project", Project);
            rec.Put("Code", Code);
            rec.Put("Value", Value);
            if (isAbsolute) rec.Put("Time", Time);
        }

        //Код проекта
        public string Project { get; private set; }
        //Код параметра
        public string Code { get; private set; }
        //Значение-строка
        public string Value { get; set; }
        //Старое значение для абсолютных
        public string OldValue { get; set; }
        //Время
        private DateTime _time = Different.MinDate;
        public DateTime Time { get { return _time; } set { _time = value; } }
        //Старое время для абсолютных
        public DateTime OldTime { get; set; }
    }
}