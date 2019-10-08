using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один отчет из архива
    public class ArchiveReport
    {
        public ArchiveReport(string code, string name, ReportType type, DateTime lastChange)
        {
            Code = code;
            Name = name;
            Type = type;
            SourceChange = lastChange;
        }

        public ArchiveReport(IRecordRead rec)
        {
            Code = rec.GetString("Report");
            Name = rec.GetString("ReportName");
            Type = rec.GetString("ReportType").ToReportType();
            SourceChange = rec.GetTime("SourceChahge");
            Id = rec.GetInt("ReportId");
        }

        //Записывает себя в Reports архива
        public void ToRecordset(IRecordSet rec)
        {
            rec.Put("Report", Code);
            rec.Put("ReportName", Name);
            rec.Put("ReportType", Type.ToRussian());
            rec.Put("SourceChange", SourceChange);
            if (rec.GetTimeNull("TimeAdd") == null)
                rec.Put("TimeAdd", DateTime.Now);
        }

        //Код
        public string Code { get; private set; }
        //Имя 
        public string Name { get; private set; }
        //Id из таблицы Reports архива
        public int Id { get; set; }
        //Тип шаблона (ведомость или шаблон)
        public ReportType Type { get; private set; }
        //Время последнего обновления ссылок
        public DateTime SourceChange { get; private set; }

        //Словарь словарей параметров отчета, ключи - код проекта и код параметра в нижнем регистре
        private readonly DicS<DicS<ArchiveReportParam>> _projects = new DicS<DicS<ArchiveReportParam>>();
        public DicS<DicS<ArchiveReportParam>> Projects { get { return _projects; } }

        //Заказ интервалов для чтения
        private readonly List<ArchiveInterval> _intervalsForRead = new List<ArchiveInterval>();
        public List<ArchiveInterval> IntervalsForRead { get { return _intervalsForRead; } }

        //Добавляет параметр в словари
        public void AddParam(ArchiveReportParam par)
        {
            Projects.Add(par.Project, new DicS<ArchiveReportParam>());
            Projects[par.Project].Add(par.Code, par);
        }
    }
}