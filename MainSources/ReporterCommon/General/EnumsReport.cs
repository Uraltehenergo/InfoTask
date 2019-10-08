using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    //Комманда в панели меню Excel, или другая комманда открытия формы
    public enum ReporterCommand
    {
        Create,
        Setup,
        AppInfo,

        GroupReports,
        GroupReportEdit,

        Report,
        LiveReport,
        Video,
        ArchivesRanges,
        Intervals,
        FilterIntervals,
        
        AbsoluteEdit,
        HandInput,
        HandInputValues,

        PutLinks,
        LinkProperties,
        FilterParams,
        FindLinks,
        LinksTemplate,
        ClearCells,
        ShapeLibrary,

        LinksList,
        SaveReport,
        DeleteLinks,
        Update,
        CopyServerReport
    }

    //--------------------------------------------------------------------------
    //Тип исходных данных для проекта отчета
    public enum CalcModeType
    {
        Internal, //Расчет при построении отчета
        ExternalPeriodic, //Сторонний периодический
        ExternalSingle //Сторонний разовый
    }

    //--------------------------------------------------------------------------
    //Тип ячейки в ведомости
    public enum LinkType
    {
        MomentsList,
        Result,
        Combined,
        Absolute,
        AbsoluteEdit,
        AbsoluteCombined,
        AbsoluteList,
        CombinedList,
        HandInput,
        Save,
        System,
        Empty
    }

    //--------------------------------------------------------------------------
    //Свойство, записанное в ссылку в ведомости
    public enum LinkField
    {
        Project,
        Code,
        CodeParam,
        CodeSubParam,
        Task,
        Name,
        SubName,
        Units,
        DataType,
        CalcParamType,
        SuperProcessType,
        Comment,
        SubComment,
        Min,
        Max,
        DecPlaces,
        Tag,

        Value,
        Nd,
        Time,
        Number,
        Empty
    }

    //--------------------------------------------------------------------------
    //Действие выполняемое при установке ссылки по шаблону
    public enum CellActionType
    {
        Link,
        Value,
        Save,
        Text
    }

    //--------------------------------------------------------------------------
    //Способ ограничения длительности периода заполнения отчета
    public enum DifferentLength
    {
        True,
        Less,
        Equals
    }

    //--------------------------------------------------------------------------
    //Внешние методы для перечислений
    public static class ConvReport
    {
        //Возвращает список всех несистемных листов книги
        public static List<Worksheet> GetSheets(this Workbook book, bool withoutSystem = true)
        {
            return book.Worksheets.OfType<Worksheet>().Where(page => !withoutSystem || (page.Name != "SysPage" && page.Name != "Templates")).ToList();
        }

        //Возвращает значение ячейки x,y с листа page
        public static string CellValue(this Worksheet page, int y, int x)
        {
            try { return ((Range)page.Cells[y, x]).Value2.ToString();}
            catch { return null; }
        }

        //True, если значение ячейки x,y - пустое
        public static bool CellIsEmpty(this Worksheet page, int y, int x)
        {
            try { return ((Range)page.Cells[y, x]).Value2 == null || page.CellValue(y, x).IsEmpty(); }
            catch { }
            return true;
        }

        //Записать строку в ячейку
        public static void PutCellValue(this Worksheet page, int y, int x, string s)
        {
            ((Range)page.Cells[y, x]).Value2 = s;
        }

        //Переводит ReporterCommand в название формы, если форма есть
        public static string ToFormName(this ReporterCommand f)
        {
            switch (f)
            {
                case ReporterCommand.AbsoluteEdit:
                    return "редактирования абсолютных значений";
                case ReporterCommand.AppInfo:
                    return "информации о программе";
                case ReporterCommand.ArchivesRanges:
                    return "диапазонов провайдеров";
                case ReporterCommand.Create:
                    return "создания отчета";
                case ReporterCommand.FilterParams:
                    return "фильтрации списка параметров";
                case ReporterCommand.FilterIntervals:
                    return "фильтрации журнала отчетов";
                case ReporterCommand.FindLinks:
                    return "поиска ссылок на выбранный параметр";
                case ReporterCommand.GroupReports:
                    return "списка групповых отчетов";
                case ReporterCommand.GroupReportEdit:
                    return "редактирования группового отчета";
                case ReporterCommand.HandInput:
                    return "списка парамтров ручного ввода";
                case ReporterCommand.HandInputValues:
                    return "значений парамтра ручного ввода";
                case ReporterCommand.Intervals:
                    return "списка интервалов журнала отчетов";
                case ReporterCommand.LinkProperties:
                    return "свойств ссылки";
                case ReporterCommand.LinksTemplate:
                    return "редактирования шаблонов установки ссылок";
                case ReporterCommand.PutLinks:
                    return "установки ссылок";
                case ReporterCommand.Report:
                    return "формирования отчетов";
                case ReporterCommand.LiveReport:
                    return "динамических отчетов";
                case ReporterCommand.Video:
                    return "видеомагнитофона";
                case ReporterCommand.SaveReport:
                    return "сохранения в журнал";
                case ReporterCommand.Setup:
                    return "настройки отчета";
                case ReporterCommand.LinksList:
                    return "списка ссылок";
                case ReporterCommand.ShapeLibrary:
                    return "свойств библиотечной фигуры";
            }
            return "";
        }

        //True, если форма открывается одна на все книги
        public static bool OneForAllBooks(this ReporterCommand f)
        {
            return f == ReporterCommand.Create || f == ReporterCommand.GroupReports || f == ReporterCommand.GroupReportEdit || f == ReporterCommand.AppInfo;
        }

        //True, если при выполнении комманды нужно проверять, что активная книга является отчетом
        public static bool NeedCheckReport(this ReporterCommand f)
        {
            return f == ReporterCommand.Report || f == ReporterCommand.LiveReport || f == ReporterCommand.Video || f == ReporterCommand.Setup || f == ReporterCommand.PutLinks || f == ReporterCommand.DeleteLinks || f == ReporterCommand.Update ||
                   f== ReporterCommand.ClearCells || f == ReporterCommand.Update || f == ReporterCommand.AbsoluteEdit || f == ReporterCommand.LinksList || f == ReporterCommand.ShapeLibrary ||
                   f == ReporterCommand.HandInputValues || f == ReporterCommand.SaveReport || f == ReporterCommand.Intervals || f == ReporterCommand.CopyServerReport; 
        }

        //True, если комманда c1 строго менее важна чем форма f2, с1 нельзя выполнить, когда открыта f2
        public static bool Less(this ReporterCommand c1, ReporterCommand f2)
        {
            switch (c1)
            {
                case ReporterCommand.Create:
                    return false;
                case ReporterCommand.Setup:
                    return f2 == ReporterCommand.Create || f2 == ReporterCommand.ClearCells;
                case ReporterCommand.PutLinks:
                case ReporterCommand.DeleteLinks:
                case ReporterCommand.Update:
                case ReporterCommand.LinksList:
                case ReporterCommand.ShapeLibrary:
                    return f2 == ReporterCommand.Create || f2 == ReporterCommand.Setup || f2 == ReporterCommand.ClearCells;
                case ReporterCommand.Report:
                case ReporterCommand.GroupReports:
                    return f2 == ReporterCommand.LiveReport || f2 == ReporterCommand.Video || f2 == ReporterCommand.Create || f2 == ReporterCommand.Setup || f2 == ReporterCommand.PutLinks || f2 == ReporterCommand.LinksList || f2 == ReporterCommand.ClearCells || f2 == ReporterCommand.ShapeLibrary;
                case ReporterCommand.LiveReport:
                case ReporterCommand.Video:
                case ReporterCommand.Intervals:
                case ReporterCommand.SaveReport:
                case ReporterCommand.HandInput:
                case ReporterCommand.AbsoluteEdit:
                    return f2 == ReporterCommand.Create || f2 == ReporterCommand.Setup || f2 == ReporterCommand.PutLinks || f2 == ReporterCommand.LinksList || f2 == ReporterCommand.ClearCells || f2 == ReporterCommand.ShapeLibrary;
                case ReporterCommand.CopyServerReport:
                    return f2 == ReporterCommand.Create || f2 == ReporterCommand.Setup || f2 == ReporterCommand.PutLinks || f2 == ReporterCommand.LinksList || f2 == ReporterCommand.ShapeLibrary
                        || f2 == ReporterCommand.Report || f2 == ReporterCommand.LiveReport || f2 == ReporterCommand.Video || f2 == ReporterCommand.Intervals || f2 == ReporterCommand.AbsoluteEdit || f2 == ReporterCommand.HandInput;
                case ReporterCommand.GroupReportEdit:
                    return f2 != ReporterCommand.GroupReports;
            }
            return false;
        }

        //True, если комманда c1 строго более важна чем f2, форма f2 закрывается при выполнени c1
        public static bool Greater(this ReporterCommand c1, ReporterCommand f2)
        {
            switch (c1)
            {
                case ReporterCommand.PutLinks:
                case ReporterCommand.DeleteLinks:
                case ReporterCommand.Update:
                case ReporterCommand.LinksList:
                case ReporterCommand.ClearCells:
                case ReporterCommand.ShapeLibrary:
                    return f2 == ReporterCommand.Report || f2 == ReporterCommand.LiveReport || f2 == ReporterCommand.Video || f2 == ReporterCommand.ArchivesRanges || f2 == ReporterCommand.Intervals || f2 == ReporterCommand.AbsoluteEdit || f2 == ReporterCommand.HandInput;
                case ReporterCommand.Setup:
                    return f2 == ReporterCommand.Report || f2 == ReporterCommand.LiveReport || f2 == ReporterCommand.Video || f2 == ReporterCommand.ArchivesRanges || f2 == ReporterCommand.Intervals || f2 == ReporterCommand.AbsoluteEdit || f2 == ReporterCommand.HandInput
                         || f2 == ReporterCommand.PutLinks || f2 == ReporterCommand.LinksList || f2 == ReporterCommand.FindLinks || f2 == ReporterCommand.LinksTemplate || f2 == ReporterCommand.FilterParams || f2 == ReporterCommand.LinkProperties || f2 == ReporterCommand.ShapeLibrary;
                case ReporterCommand.LiveReport:
                case ReporterCommand.Video:
                    return f2 == ReporterCommand.Report || f2 == ReporterCommand.Intervals || f2 == ReporterCommand.AbsoluteEdit;
                case ReporterCommand.Report:
                    return f2 == ReporterCommand.Intervals || f2 == ReporterCommand.AbsoluteEdit;
                case ReporterCommand.Intervals:
                    return f2 == ReporterCommand.AbsoluteEdit || f2 == ReporterCommand.Report || f2 == ReporterCommand.LiveReport || f2 == ReporterCommand.Video;
                case ReporterCommand.AbsoluteEdit:
                    return f2 == ReporterCommand.Intervals || f2 == ReporterCommand.Report || f2 == ReporterCommand.LiveReport || f2 == ReporterCommand.Video;
            }
            return false;
        }

        //True, если форма f1 вызывется только из f2 и если f1 должна закрываться при закрытии f2
        public static bool IsChildOf(this ReporterCommand f1, ReporterCommand f2)
        {
            switch (f2)
            {
                case ReporterCommand.PutLinks:
                    return f1 == ReporterCommand.FindLinks || f1 == ReporterCommand.LinksTemplate || f1 == ReporterCommand.FilterParams || f1 == ReporterCommand.LinkProperties;
                case ReporterCommand.GroupReports:
                    return f1 == ReporterCommand.GroupReportEdit;
                case ReporterCommand.Report:
                case ReporterCommand.Video:
                    return f1 == ReporterCommand.ArchivesRanges;
                case ReporterCommand.HandInput:
                    return f1 == ReporterCommand.HandInputValues;
                case ReporterCommand.Intervals:
                    return f1 == ReporterCommand.FilterIntervals;
            }
            return false;
        }

        //Какую запись об открытии формы нужно добавлять в History, если null, то никакую
        public static string ToStringHistory(this ReporterCommand c)
        {
            if (c == ReporterCommand.DeleteLinks || c == ReporterCommand.AppInfo || c == ReporterCommand.FilterParams || c == ReporterCommand.LinkProperties ||c == ReporterCommand.HandInputValues) 
                return null;
            if (c == ReporterCommand.Update || c == ReporterCommand.CopyServerReport || c == ReporterCommand.ClearCells) 
                return "Запуск комманды " + c;
            return "Открытие формы " + c;
        }

        //Перевод из строки в CalcModeType
        public static CalcModeType ToCalcModeType(this string s)
        {
            if (s.IsEmpty()) return CalcModeType.ExternalPeriodic;
            switch (s.ToLower())
            {
                case "при построении отчета":
                    return CalcModeType.Internal;
                case "сторонний периодический":
                    return CalcModeType.ExternalPeriodic;
                case "сторонний разовый":
                    return CalcModeType.ExternalSingle;
            }
            return CalcModeType.ExternalPeriodic;
        }

        //Перевод из CalcModeType в русскую строку
        public static string ToRussian(this CalcModeType t)
        {
            switch (t)
            {
                case CalcModeType.Internal:
                    return "При построении отчета";
                case CalcModeType.ExternalPeriodic:
                    return "Сторонний периодический";
                case CalcModeType.ExternalSingle:
                    return "Сторонний разовый";
            }
            return "";
        }

        //Перевод строки в тип ячейки
        public static LinkType ToLinkType(this string s)
        {
            if (s == null) return LinkType.Empty;
            switch (s.ToLower())
            {
                case "result":
                case "итоговое значение":
                    return LinkType.Result;
                case "absolute":
                case "абсолютное значение":
                    return LinkType.Absolute;
                case "absoluteedit":
                case "абсолютное с редактированием":
                    return LinkType.AbsoluteEdit;
                case "absolutecombined":
                case "абсолютное комбинированное":
                    return LinkType.AbsoluteCombined;
                case "absolutelist":
                case "список абсолютных значений":
                    return LinkType.AbsoluteList;
                case "combined":
                case "комбинированное значение":
                    return LinkType.Combined;
                case "combinedlist":
                case "равномерный список значений":
                    return LinkType.CombinedList;
                case "momentslist":
                case "список мгновенных значений":
                    return LinkType.MomentsList;
                case "handinput":
                case "ручной ввод":
                    return LinkType.HandInput;
                case "save":
                case "сохранение в архив":
                    return LinkType.Save;
                case "system":
                case "системная ссылка":
                case "системные ссылки":
                    return LinkType.System;
            }
            return LinkType.Empty;
        }

        //Перевод типа ячейки в английское имя
        public static string ToEnglish(this LinkType t)
        {
            switch (t)
            {
                case LinkType.Result:
                    return "Result";
                case LinkType.Absolute:
                    return "Absolute";
                case LinkType.AbsoluteEdit:
                    return "AbsoluteEdit";
                case LinkType.Combined:
                    return "Combined";
                case LinkType.CombinedList:
                    return "CombinedList";
                case LinkType.AbsoluteCombined:
                    return "AbsoluteCombined";
                case LinkType.AbsoluteList:
                    return "AbsoluteList";
                case LinkType.MomentsList:
                    return "MomentsList";
                case LinkType.HandInput:
                    return "HandInput";
                case LinkType.Save:
                    return "Save";
                case LinkType.System:
                    return "System";
            }
            return null;
        }

        //Перевод типа ячейки в руссое имя
        public static string ToRussian(this LinkType t)
        {
            switch (t)
            {
                case LinkType.Result:
                    return "Итоговое значение";
                case LinkType.Absolute:
                    return "Абсолютное значение";
                case LinkType.AbsoluteEdit:
                    return "Абсолютное с редактированием";
                case LinkType.Combined:
                    return "Комбинированное значение";
                case LinkType.CombinedList:
                    return "Равномерный список значений";
                case LinkType.AbsoluteCombined:
                    return "Абсолютное комбинированное";
                case LinkType.AbsoluteList:
                    return "Список абсолютных значений";
                case LinkType.MomentsList:
                    return "Список мгновенных значений";
                case LinkType.HandInput:
                    return "Ручной ввод";
                case LinkType.Save:
                    return "Сохранение в архив";
                case LinkType.System:
                    return "Системная ссылка";
            }
            return null;
        }

        //True, ели ссылка заполняет список значений
        public static bool IsList(this LinkType t)
        {
            return t == LinkType.MomentsList || t == LinkType.CombinedList;
        }

        //Перевод строки в тип свойства
        public static LinkField ToLinkField(this string s)
        {
            if (s == null) return LinkField.Empty;
            switch (s.ToLower())
            {
                case "project":
                case "проект":
                    return LinkField.Project;
                case "code":
                case "полный код":
                    return LinkField.Code;
                case "codeparam":
                case "код параметра":
                    return LinkField.CodeParam;
                case "codesubparam":
                case "код подпараметра":
                    return LinkField.CodeSubParam;
                case "task":
                case "задача":
                    return LinkField.Task;
                case "name":
                case "имя параметра":
                    return LinkField.Name;
                case "subname":
                case "имя подпараметра":
                    return LinkField.SubName;
                case "units":
                case "единицы измерения":
                    return LinkField.Units;
                case "datatype":
                case "тип данных":
                    return LinkField.DataType;
                case "calcparamtype":
                case "ввод данных":
                    return LinkField.CalcParamType;
                case "superprocesstype":
                case "тип накопления":
                    return LinkField.SuperProcessType;
                case "comment":
                case "комментарий":
                    return LinkField.Comment;
                case "subcomment":
                case "комментарий подпараметра":
                    return LinkField.SubComment;
                case "min":
                case "минимум":
                    return LinkField.Min;
                case "max":
                case "максимум":
                    return LinkField.Max;
                case "decplaces":
                case "знаки после запятой":
                    return LinkField.DecPlaces;
                case "tag":
                case "свойства":
                    return LinkField.Tag;
                case "value":
                case "значение":
                    return LinkField.Value;
                case "time":
                case "время":
                    return LinkField.Time;
                case "nd":
                case "недостоверность":
                    return LinkField.Nd;
                case "number":
                case "номера значений":
                    return LinkField.Number;
            }
            return LinkField.Empty;
        }

        //Перевод типа свойства в английское имя
        public static string ToEnglish(this LinkField t)
        {
            switch (t)
            {
                case LinkField.Project:
                    return "Project";
                case LinkField.Code:
                    return "Code";
                case LinkField.CodeParam:
                    return "CodeParam";
                case LinkField.CodeSubParam:
                    return "CodeSubParam";
                case LinkField.Task:
                    return "Task";
                case LinkField.Name:
                    return "Name";
                case LinkField.SubName:
                    return "SubName";
                case LinkField.Units:
                    return "Units";
                case LinkField.DataType:
                    return "DataType";
                case LinkField.CalcParamType:
                    return "CalcParamType";
                case LinkField.SuperProcessType:
                    return "SuperProcessType";
                case LinkField.Comment:
                    return "Comment";
                case LinkField.SubComment:
                    return "SubComment";
                case LinkField.Min:
                    return "Min";
                case LinkField.Max:
                    return "Max";
                case LinkField.DecPlaces:
                    return "DecPlaces";
                case LinkField.Tag:
                    return "Tag";
                case LinkField.Value:
                    return "Value";
                case LinkField.Time:
                    return "Time";
                case LinkField.Nd:
                    return "Nd";
                case LinkField.Number:
                    return "Number";
            }
            return "";
        }

        //Перевод типа свойства в русское имя
        public static string ToRussian(this LinkField t)
        {
            switch (t)
            {
                case LinkField.Project:
                    return "Проект";
                case LinkField.Code:
                    return "Полный код";
                case LinkField.CodeParam:
                    return "Код параметра";
                case LinkField.CodeSubParam:
                    return "Код подпараметра";
                case LinkField.Task:
                    return "Задача";
                case LinkField.Name:
                    return "Имя параметра";
                case LinkField.SubName:
                    return "Имя подпараметра";
                case LinkField.Units:
                    return "Единицы измерения";
                case LinkField.DataType:
                    return "Тип данных";
                case LinkField.CalcParamType:
                    return "Ввод данных";
                case LinkField.SuperProcessType:
                    return "Тип накопления";
                case LinkField.Comment:
                    return "Комментарий";
                case LinkField.SubComment:
                    return "Комментарий подпараметра";
                case LinkField.Min:
                    return "Минимум";
                case LinkField.Max:
                    return "Максимум";
                case LinkField.DecPlaces:
                    return "Знаки после запятой";
                case LinkField.Tag:
                    return "Свойства";
                case LinkField.Value:
                    return "Значение";
                case LinkField.Time:
                    return "Время";
                case LinkField.Nd:
                    return "Недостоверность";
                case LinkField.Number:
                    return "Номера значений";
            }
            return "";
        }

        //Перевод типа свойства в имя для формы
        public static string ToFormText(this LinkField t)
        {
            switch (t)
            {
                case LinkField.Project:
                    return "Проект";
                case LinkField.Code:
                    return "Код";
                case LinkField.CodeParam:
                    return "Параметр";
                case LinkField.CodeSubParam:
                    return "Подпараметр";
                case LinkField.Task:
                    return "Задача";
                case LinkField.Name:
                    return "Имя";
                case LinkField.Units:
                    return "Единицы";
                case LinkField.DataType:
                    return "Тип данных";
                case LinkField.CalcParamType:
                    return "Ввод";
                case LinkField.SuperProcessType:
                    return "Накопление";
                case LinkField.Comment:
                    return "Комментарий";
                case LinkField.Min:
                    return "Мин";
                case LinkField.Max:
                    return "Макс";
                case LinkField.DecPlaces:
                    return "К-во знаков";
            }
            return "";
        }

        //True, если LinkField является частью значения, а не свойством параметра
        public static bool IsValueField(this LinkField t)
        {
            return t == LinkField.Value || t == LinkField.Time || t == LinkField.Number || t == LinkField.Nd;
        }

        //True, если для LinkField можно установить ссылку, а не просто записать значение
        public static bool IsLinkField(this LinkField t)
        {
            return t != LinkField.Code && t != LinkField.CodeParam && t != LinkField.CodeSubParam;
        }

        //Возвращает имя поля в таблице списка параметров и null, если такого поля там нет
        public static string ParamsTableName(this LinkField t)
        {
            if (t == LinkField.Code) return "FullCode";
            if (t == LinkField.Name) return "ParName";
            if (t == LinkField.Task || t == LinkField.Units || t == LinkField.SuperProcessType || t == LinkField.DataType || t == LinkField.CalcParamType)
                return t.ToEnglish();
            return null;
        }

        //Перевод строки в действие при установке ссылки по шаблону
        public static CellActionType ToCellAction(this string s)
        {
            if (s == null) return CellActionType.Link;
            switch (s.ToLower())
            {
                case "установить ссылку":
                    return CellActionType.Link;
                case "записать значение":
                    return CellActionType.Value;
                case "ссылка на сохранение":
                    return CellActionType.Save;
                case "записать текст":
                    return CellActionType.Text;
            }
            return CellActionType.Link;
        }

        //Перевод действия при установке ссылки по шаблону в строку
        public static string ToRussian(this CellActionType t)
        {
            switch (t)
            {
                case CellActionType.Link:
                    return "Установить ссылку";
                case CellActionType.Value:
                    return "Записать значение";
                case CellActionType.Save:
                    return "Cсылка на сохранение";
                case CellActionType.Text:
                    return "Записать текст";
            }
            return "Установить ссылку";
        }

        //Перевод строки в DifferentLength
        public static DifferentLength ToDifferentLength(this string s)
        {
            if (s == null) return DifferentLength.True;
            switch (s.ToLower())
            {
                case "true":
                    return DifferentLength.True;
                case "less":
                    return DifferentLength.Less;
                case "equals":
                    return DifferentLength.Equals;
            }
            return DifferentLength.True;
        }

        //Перевод типа фигуры в строку
        public static string ToEnglish(this MsoShapeType type)
        {
            switch (type)
            {
                case MsoShapeType.msoTextBox:
                    return "TextBox";
                case MsoShapeType.msoGroup:
                    return "Group";
            }
            return "";
        }

        //Перевод строки в тип фигуры
        public static MsoShapeType ToShapeType(this string s)
        {
            if (s == null) return MsoShapeType.msoComment;
            switch (s.ToLower())
            {
                case "textbox":
                    return MsoShapeType.msoTextBox;
                case "group":
                    return MsoShapeType.msoGroup;
            }
            return MsoShapeType.msoComment;
        }
    }
}