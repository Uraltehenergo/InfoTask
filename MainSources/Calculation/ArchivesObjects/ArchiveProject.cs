using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один проект из архива
    public class ArchiveProject
    {
        public ArchiveProject(string code, string name, ReportType type, DateTime lastCahnge)
        {
            Code = code;
            Name = name;
            Type = type;
            SourceChange = lastCahnge;
        }

        //Из таблицы архива, если провайдер позволяет
        public ArchiveProject(IRecordRead rec)
        {
            Code = rec.GetString("Project");
            Name = rec.GetString("ProjectName");
            Type = rec.GetString("ProjectType").ToReportType();
            SourceChange = rec.GetTime("SourceChange");
            Id = rec.GetInt("ProjectId");
        }

        //Запись в Projects архива, если провайдер позволяет
        public void ToRecordset(IRecordSet rec)
        {
            rec.Put("Project", Code);
            rec.Put("ProjectName", Name);
            rec.Put("ProjectType", Type.ToRussian());
            rec.Put("TimeChange", DateTime.Now.ToString());
            rec.Put("SourceChange", SourceChange);
            if (rec.GetTimeNull("TimeAdd") == null)
                rec.Put("TimeAdd", DateTime.Now);
        }

        //Код проекта
        public string Code { get; private set; }
        //Имя проекта
        public string Name { get; private set; }
        //Id в таблице Projects архива
        public int Id { get; set; }
        //Тип проект расчета или проект отчета
        public ReportType Type { get; private set; }
        //Дата последней компиляции
        public DateTime SourceChange { get; private set; }

        //Словарь расчетных параметров, ключи - код параметра в нижнем регистре
        private readonly DicS<ArchiveParam> _params = new DicS<ArchiveParam>();
        public DicS<ArchiveParam> Params { get { return _params; }}
        //Добавить параметр
        public void AddParam(ArchiveParam par)
        {
            Params.Add(par.FullCode, par);
        }

        //Интервалы для записи
        private readonly List<ArchiveInterval> _intervalsForWrite = new List<ArchiveInterval>();
        public List<ArchiveInterval> IntervalsForWrite { get { return _intervalsForWrite; } }
        //Заказ интервалов для чтения
        private readonly List<ArchiveInterval> _intervalsForRead = new List<ArchiveInterval>();
        public List<ArchiveInterval> IntervalsForRead { get { return _intervalsForRead; } }
    }
}