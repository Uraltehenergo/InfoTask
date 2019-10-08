using System;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один интервал архива
    public class ArchiveInterval : TimeInterval
    {
        public ArchiveInterval(IntervalType type, DateTime begin, DateTime end, string name = "")
            : base(begin, end)
        {
            Type = type;
            Name = name ?? "";
            TimeChange = Different.MinDate;
        }

        //Если провайдер позволяет, чтение из таблицы архива
        public ArchiveInterval(IRecordRead rec, IntervalType type) : base(rec)
        {
            Type = type;
            Name = rec.GetString("IntervalName");
            Id = rec.GetInt("IntervalId");
            TimeChange = rec.GetTime("TimeChange");
        }

        //Запись в Intervals, projectId - Id проекта
        public void ToRecordset(IRecordSet rec, int projectId)
        {
            rec.Put("ProjectId", projectId);
            if (rec.GetTimeNull("TimeBegin") == null || (Type != IntervalType.NamedAdd && Type != IntervalType.NamedAddParams))
            {
                rec.Put("TimeBegin", Begin);
                rec.Put("TimeEnd", End);
            }
            else
            {
                if (rec.GetTime("TimeBegin") > Begin) rec.Put("TimeBegin", Begin);
                if (rec.GetTime("TimeEnd") < End) rec.Put("TimeEnd", End);
            }
            rec.Put("IntervalName", Name ?? "");
            TimeChange = DateTime.Now;
            rec.Put("TimeChange", TimeChange);
            if (rec.GetTimeNull("TimeAdd") == null) 
                rec.Put("TimeAdd", TimeChange);
        }

        //Имя интервала
        public string Name { get; set; }
        //Тип интервала
        public IntervalType Type { get; private set; }
        //Id из Intervals
        public int Id { get; set; }
        //Время последнего изменения
        public DateTime TimeChange { get; set; }
    }
}