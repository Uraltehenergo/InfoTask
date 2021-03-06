﻿using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Тип провайдера или другого подобного объекта
    public enum ProviderType
    {
        Communicator,
        Source,
        Archive,
        Receiver,
        Imitator,
        Thread,
        Project,
        MonitorHistory,
        Error
    }

    //---------------------------------------------------------------------

    //Тип окна для настройки провайдера
    public enum ProviderSetupType
    {
        Reporter,//Настройка разового расчета из репортера
        Controller,//Настройка собственных потоков контроллера
        ReporterArchive,//Настройка отчетов и менеджера на архив
        Analyzer,//Анализатор
        Constructor,//Конструктор
        Default, //Не задан
        Error
    }

    //--------------------------------------------------------------------
    //Тип расчетного параметра
    public enum CalcParamType
    {
        Class,//Обычный параметр или функция
        HandBool, //Ручной ввод
        HandInt,
        HandTime,
        HandReal,
        HandString,
        Error //не правильно заполненная
    }

    //--------------------------------------------------------------------
    
    //Тип графика
    public enum GraficType
    {
        Grafic,//График
        Grafic0,//График - ступенчатая интерполяция
        Diagramm,//Диаграмма
        Error
    }

    //--------------------------------------------------------------------
    //Тип интерполяции
    public enum InterpolationType
    {
        Constant,//Ступенчатая
        Linear,//Линейная
        None,
        Error //не правильно заполненная
    }

    //----------------------------------------------------------------------
    //Типы накопления
    public enum SuperProcess
    {
        Moment, //Мгновенные
        FirstP, //Периодическое - первое
        FirstA, //Абсолютное - первое
        FirstPA, //Периодическое и абсолютное - первое
        LastP, //Периодическое - последнее
        LastA,//Абсолютное - последнее
        LastPA, //Периодическое и абсолютное - последнее
        MinP, //Периодическое - минимум
        MinA, //Абсолютное - минимум
        MinPA, //Периодическое и абсолютное - минимум
        MaxP, //Итоговое - максимум
        MaxA, //Абсолютное - максимум
        MaxPA, //Периодическое и абсолютное - максимум
        AverageP, //Периодическое - среднее
        AverageA, //Абсолютное - среднее
        AveragePA, //Периодическое и абсолютное - среднее
        AvNonZeroP, //Периодическое - среднее не ноль
        AvNonZeroA, //Абсолютное - среднее не ноль
        AvNonZeroPA, //Периодическое и абсолютное - среднее не ноль
        SummP, //Периодическое - сумма
        SummA,//Абсолютное - сумма
        SummPA,  //Периодическое и абсолютное - сумма
        None, //Нет накопления
        Error //Не правильно заполненное
    }

   //-----------------------------------------------------------------

    //Режим запуска расчета потока контроллером
    public enum ThreadMode
    {
        Single,//Пошаговый режим, запуск из Контроллера
        Periodic,//Динамический режим, запуск из Контроллера
        Hand,//Расчет на основе ручного ввода
        RealTime,//Расчет в реальном времени
        Error
    }

    //-----------------------------------------------------------------

    //Приложения
    public enum ApplicationType
    {
        Constructor,//Конструктор расчетов
        Excel,//Менеджер отчетов Excel
        PowerPoint,//Менеджер отчетов PowerPoint
        Controller, //Контроллер расчетов
        Analyzer,//Анализатор 
        Ras,//РАС
        Error//Ошибка
    }
   
    //--------------------------------------------------------------------------
   
    //Тип отчета в архиве
    public enum ReportType
    {
        Calc,
        Template,
        Source,
        Excel,
        PowerPoint,
        Error
    }

    //--------------------------------------------------------------------------
    //Тип интервала для записи в архив
    public enum IntervalType
    {
        Single,
        Named,
        NamedAdd, //Именованный интервал с добавлением по времени и параметрам
        NamedAddParams, //Именованный интервал с добавлением по параметрам
        Periodic,
        Moments,
        Base,
        Hour,
        Day,
        Absolute,
        AbsoluteDay,
        Combined, //Для запроса данных для отчета Базовые, Часовые, Суточные, Часовые, Базовые 
        AbsoluteCombined, //Для запроса данных для отчета АбсолютныеСутки, Часовые, Базовые 
        AbsoluteListBase,
        AbsoluteListHour,
        AbsoluteListDay,
        Empty
    }

   //--------------------------------------------------------------------------
    //Режим формирования имитационного значения
    public enum ImitMode
    {
        NoImit,
        FromBegin,
        FromHour,
        FromDay,
        Default
    }

    //--------------------------------------------------------------------------
    
    //Преобразование различных типов данных
    public static class Conv
    {
        //Определяет, является ли провайдер источником данных
        public static bool IsProviderSource(this ProviderType t)
        {
            return t == ProviderType.Source || t == ProviderType.Imitator;
        }

        //Перевод из строки в ProviderType
        public static ProviderType ToProviderType(this string t)
        {
            if (t == null) return ProviderType.Error;
            switch (t.ToLower())
            {
                case "коммуникатор":
                case "communicator":
                    return ProviderType.Communicator;
                case "источник":
                case "source":
                    return ProviderType.Source;
                case "архив":
                case "archive":
                    return ProviderType.Archive;
                case "приемник":
                case "receiver":
                    return ProviderType.Receiver;
                case "имитатор":
                case "imitator":
                    return ProviderType.Imitator;
                case "поток":
                case "thread":
                    return ProviderType.Thread;
                case "проект":
                case "project":
                    return ProviderType.Project;
                case "историямонитора":
                case "monitorhistory":
                    return ProviderType.MonitorHistory;
            }
            return ProviderType.Error;
        }

        //Перевод из ProviderType в русское имя
        public static string ToRussian(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Communicator:
                    return "Коммуникатор";
                case ProviderType.Source:
                    return "Источник";
                case ProviderType.Archive:
                    return "Архив";
                case ProviderType.Receiver:
                    return "Приемник";
                case ProviderType.Imitator:
                    return "Имитатор";
                case ProviderType.Thread:
                    return "Поток";
                case ProviderType.Project:
                    return "Проект";
                case ProviderType.MonitorHistory:
                    return "ИсторияМонитора";
            }
            return "Ошибка";
        }

        //Перевод из ProviderType в английское имя
        public static string ToEnglish(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Communicator:
                    return "Communicator";
                case ProviderType.Source:
                    return "Source";
                case ProviderType.Archive:
                    return "Archive";
                case ProviderType.Receiver:
                    return "Receiver";
                case ProviderType.Imitator:
                    return "Imitator";
                case ProviderType.Thread:
                    return "Thread";
                case ProviderType.Project:
                    return "Project";
                case ProviderType.MonitorHistory:
                    return "MonitorHistory";
            }
            return "Error";
        }

        //Список совместимых кодов провайдеров для выбранного кода провайдера
        public static List<string> JointProviderCodes(this string code)
        {
            var list = new List<string>{code};
            using (var rec = new RecDao(Different.GetInfoTaskDir() + @"General\Config.accdb", "SysTabl"))
            {
                if (code == "CloneSource")
                {
                    while (rec.Read())
                        if (rec.GetString("ParamValue") == "Источник" && rec.GetString("ParamName") != "CloneSource")
                            list.Add(rec.GetString("ParamName"));
                }
                else
                {
                    rec.FindFirst("ParamName", code);
                    if (!rec.NoMatch())
                    {
                        var set = rec.GetString("ParamTag").ToPropertyDicS()["JointProviders"].ToPropertyHashSet();
                        foreach (var c in set.Values)
                        {
                            rec.FindFirst("ParamName", c);
                            if (!rec.NoMatch()) list.Add(c);
                        }
                    }    
                }
            }
            return list;
        } 

        //Перевод из строки в ProviderSetupType
        public static ProviderSetupType ToProviderSetupType(this string s)
        {
            if (s.IsEmpty()) return ProviderSetupType.Error;
            switch (s.ToLower())
            {
                case "reporter":
                    return ProviderSetupType.Reporter;
                case "controller":
                    return ProviderSetupType.Controller;
                case "reporterarchive":
                    return ProviderSetupType.ReporterArchive;
                case "analyzer":
                    return ProviderSetupType.Analyzer;
                case "constructor":
                    return ProviderSetupType.Constructor;
                case "default":
                    return ProviderSetupType.Default;
            }
            return ProviderSetupType.Error;
        }

        //Перевод CalcParamType в строку
        public static string ToRussian(this CalcParamType t)
        {
            switch (t)
            {
                case CalcParamType.HandBool:
                    return "ВводЛогич";
                case CalcParamType.HandInt:
                    return "ВводЦелое";
                case CalcParamType.HandTime:
                    return "ВводВремя";
                case CalcParamType.HandReal:
                    return "ВводДейств";
                case CalcParamType.HandString:
                    return "ВводСтрока";
            }
            return null;
        }

        //Перевод из строки в CalcParamType
        public static CalcParamType ToCalcParamType(this string t)
        {
            if (t == null) return CalcParamType.Class;
            switch (t.ToLower())
            {
                case "":
                    return CalcParamType.Class;
                case "вводлогич":
                    return CalcParamType.HandBool;
                case "вводцелое":
                    return CalcParamType.HandInt;
                case "вводвремя":
                    return CalcParamType.HandTime;
                case "вводдейств":
                    return CalcParamType.HandReal;
                case "вводстрока":
                    return CalcParamType.HandString;
            }
            return CalcParamType.Error;
        }

        //Тип данных параметра ручного ввода по CalcParamType
        public static DataType HandDataType(this CalcParamType t)
        {
            switch (t)
            {
                case CalcParamType.HandBool:
                    return DataType.Boolean;
                case CalcParamType.HandInt:
                    return DataType.Integer;
                case CalcParamType.HandReal:
                    return DataType.Real;
                case CalcParamType.HandTime:
                    return DataType.Time;
                case CalcParamType.HandString:
                    return DataType.String;
            }
            return DataType.Error;
        }

        //Перевод строки в тип графика
        public static GraficType ToGraficType(this string t)
        {
            if (t == null) return GraficType.Error;
            switch (t.ToLower())
            {
                case "график":
                    return GraficType.Grafic;
                case "график0":
                    return GraficType.Grafic0;
                case "диаграмма":
                    return GraficType.Diagramm;
            }
            return GraficType.Error;
        }

        //Перевод типа графика в строку
        public static string ToRussian(this GraficType t)
        {
            switch (t)
            {
                case GraficType.Grafic:
                    return "График";
                case GraficType.Grafic0:
                    return "График0";
                case GraficType.Diagramm:
                    return "Диаграмма";
            }
            return "Ошибка";
        }

        //Перевод из строки в InterpolationType
        public static InterpolationType ToInterpolation(this string t)
        {
            if (t == null) return InterpolationType.None;
            switch (t.ToLower())
            {
                case "ступенчатая":
                case "constant":
                    return InterpolationType.Constant;
                case "линейная":
                case "linear":
                    return InterpolationType.Linear;
                case "":
                    return InterpolationType.None;
            }
            return InterpolationType.Error;
        }

        //Перевод из InterpolationType в строку
        public static string ToRussian(this InterpolationType t)
        {
            switch (t)
            {
                case InterpolationType.Constant:
                    return "Ступенчатая";
                case InterpolationType.Linear:
                    return "Линейная";
            }
            return null;
        }

        //Перевод из строки в SuperProcessType
        public static SuperProcess ToSuperProcess(this string t)
        {
            if (t == null) return SuperProcess.None;
            switch (t.ToLower())
            {
                case "мгновенные":
                case "moment":
                    return SuperProcess.Moment;
                case "среднее(а)":
                case "average(a)":
                    return SuperProcess.AverageA;
                case "среднее(п)":
                case "average(p)":
                    return SuperProcess.AverageP;
                case "среднее(па)":
                case "average(pa)":
                    return SuperProcess.AveragePA;
                case "среднеененоль(а)":
                case "averagenonzero(a)":
                    return SuperProcess.AvNonZeroA;
                case "среднеененоль(п)":
                case "averagenonzero(п)":
                    return SuperProcess.AvNonZeroP;
                case "среднеененоль(па)":
                case "averagenonzero(пa)":
                    return SuperProcess.AvNonZeroPA;
                case "минимум(а)":
                case "minimum(a)":
                    return SuperProcess.MinA;
                case "минимум(п)":
                case "minimum(p)":
                    return SuperProcess.MinP;
                case "минимум(па)":
                case "minimum(pa)":
                    return SuperProcess.MinPA;
                case "максимум(а)":
                case "maximum(a)":
                    return SuperProcess.MaxA;
                case "максимум(п)":
                case "maximum(p)":
                    return SuperProcess.MaxP;
                case "максимум(па)":
                case "maximum(pa)":
                    return SuperProcess.MaxPA;
                case "первое(а)":
                case "first(a)":
                    return SuperProcess.FirstA;
                case "первое(п)":
                case "first(p)":
                    return SuperProcess.FirstP;
                case "первое(па)":
                case "first(pa)":
                    return SuperProcess.FirstPA;
                case "последнее(а)":
                case "last(a)":
                    return SuperProcess.LastA;
                case "последнее(п)":
                case "last(p)":
                    return SuperProcess.LastP;
                case "последнее(па)":
                case "last(pa)":
                    return SuperProcess.LastPA;
                case "сумма(а)":
                case "summ(a)":
                    return SuperProcess.SummA;
                case "сумма(п)":
                case "summ(p)":
                    return SuperProcess.SummP;
                case "сумма(па)":
                case "summ(pa)":
                    return SuperProcess.SummPA;
                case "":
                    return SuperProcess.None;
            }
            return SuperProcess.Error;
        }

        //Перевод из SuperProcessType в строку
        public static string ToRussian(this SuperProcess t)
        {
            switch (t)
            {
                case SuperProcess.Moment:
                    return "Мгновенные";
                case SuperProcess.AverageA:
                    return "Среднее(А)";
                case SuperProcess.AverageP:
                    return "Среднее(П)";
                case SuperProcess.AveragePA:
                    return "Среднее(ПА)";
                case SuperProcess.AvNonZeroA:
                    return "СреднееНеНоль(А)";
                case SuperProcess.AvNonZeroP:
                    return "СреднееНеНоль(П)";
                case SuperProcess.AvNonZeroPA:
                    return "СреднееНеНоль(ПА)";
                case SuperProcess.MinA:
                    return "Минимум(А)";
                case SuperProcess.MinP:
                    return "Минимум(П)";
                case SuperProcess.MinPA:
                    return "Минимум(ПА)";
                case SuperProcess.MaxA:
                    return "Максимум(А)";
                case SuperProcess.MaxP:
                    return "Максимум(П)";
                case SuperProcess.MaxPA:
                    return "Максимум(ПА)";
                case SuperProcess.FirstA:
                    return "Первое(А)";
                case SuperProcess.FirstP:
                    return "Первое(П)";
                case SuperProcess.FirstPA:
                    return "Первое(ПА)";
                case SuperProcess.LastA:
                    return "Последнее(А)";
                case SuperProcess.LastP:
                    return "Последнее(П)";
                case SuperProcess.LastPA:
                    return "Последнее(ПА)";
                case SuperProcess.SummA:
                    return "Сумма(А)";
                case SuperProcess.SummP:
                    return "Сумма(П)";
                case SuperProcess.SummPA:
                    return "Сумма(ПА)";
            }
            return null;
        }

        //True, если тип накопления не заполнен, или заполнен с ошибкой
        public static bool IsNone(this SuperProcess t)
        {
            return t == SuperProcess.None || t == SuperProcess.Error;
        }

        //True, если тип накопления предполагает абсолютное накопление
        public static bool IsAbsolute(this SuperProcess t)
        {
            return (new HashSet<SuperProcess> 
                {SuperProcess.MinA, SuperProcess.MinPA, SuperProcess.MaxA, SuperProcess.MaxPA, SuperProcess.FirstA, SuperProcess.FirstPA, SuperProcess.LastA, SuperProcess.LastPA, 
                 SuperProcess.SummA, SuperProcess.SummPA, SuperProcess.AverageA, SuperProcess.AveragePA, SuperProcess.AvNonZeroA, SuperProcess.AvNonZeroPA}) 
                .Contains(t);   
        }

        //True, если тип накопления предполагает периодическое накопление
        public static bool IsPeriodic(this SuperProcess t)
        {
            return (new HashSet<SuperProcess> 
                {SuperProcess.MinP, SuperProcess.MinPA, SuperProcess.MaxP, SuperProcess.MaxPA, SuperProcess.FirstP, SuperProcess.FirstPA, SuperProcess.LastP, SuperProcess.LastPA, 
                 SuperProcess.SummP, SuperProcess.SummPA, SuperProcess.AverageP, SuperProcess.AveragePA, SuperProcess.AvNonZeroP, SuperProcess.AvNonZeroPA})
                .Contains(t);
        }

        //Убирает A из типа накопления (оставляет P)
        public static SuperProcess ToProcess(this SuperProcess t)
        {
            switch (t)
            {
                case SuperProcess.None:
                    return SuperProcess.None;
                case SuperProcess.Moment:
                    return SuperProcess.Moment;
                case SuperProcess.AverageA:
                case SuperProcess.AverageP:
                case SuperProcess.AveragePA:
                    return SuperProcess.AverageP;
                case SuperProcess.AvNonZeroA:
                case SuperProcess.AvNonZeroP:
                case SuperProcess.AvNonZeroPA:
                    return SuperProcess.AvNonZeroP;
                case SuperProcess.MinA:
                case SuperProcess.MinP:
                case SuperProcess.MinPA:
                    return SuperProcess.MinP;
                case SuperProcess.MaxA:
                case SuperProcess.MaxP:
                case SuperProcess.MaxPA:
                    return SuperProcess.MaxP;
                case SuperProcess.FirstA:
                case SuperProcess.FirstP:
                case SuperProcess.FirstPA:
                    return SuperProcess.FirstP;
                case SuperProcess.LastA:
                case SuperProcess.LastP:
                case SuperProcess.LastPA:
                    return SuperProcess.LastP;
                case SuperProcess.SummA:
                case SuperProcess.SummP:
                case SuperProcess.SummPA:
                    return SuperProcess.SummP;
            }
            return SuperProcess.Error;
        }

        //Как преобразуется тип расчетного параметра при преобразовании в архивный параметр с учетом типа накопления
        public static DataType AplySuperProcess(this DataType dt, SuperProcess sp)
        {
            var s = sp.ToProcess();
            if (s == SuperProcess.AverageP || s == SuperProcess.AvNonZeroP) return DataType.Real;
            if (s == SuperProcess.SummP && dt == DataType.Boolean) return DataType.Integer;
            return dt;
        }

        //Перевод из строки в режим расчета
        public static ThreadMode ToThreadMode(this string m)
        {
            if (m == null) return ThreadMode.Error;
            switch (m.ToLower())
            {
                case "периодический":
                case "periodic":
                    return ThreadMode.Periodic;
                case "разовый":
                case "single":
                    return ThreadMode.Single;
                case "моментальный":
                case "realtime":
                    return ThreadMode.RealTime;
                case "ручной":
                case "hand":
                    return ThreadMode.Hand;
            }
            return ThreadMode.Error;
        }

        //Перевод из режима расчета в строку
        public static string ToCode(this ThreadMode m)
        {
            switch (m)
            {
                case ThreadMode.Periodic:
                    return "Periodic";
                case ThreadMode.Single:
                    return "Single";
                case ThreadMode.RealTime:
                    return "RealTime";
                case ThreadMode.Hand:
                    return "Hand";
                case ThreadMode.Error:
                    return "";
            }
            return "";
        }

        //Перевод из режима расчета в строку
        public static string ToRussian(this ThreadMode m)
        {
            switch (m)
            {
                case ThreadMode.Periodic:
                    return "Периодический";
                case ThreadMode.Single:
                    return "Разовый";
                case ThreadMode.RealTime:
                    return "Моментальный";
                case ThreadMode.Hand:
                    return "Ручной";
                case ThreadMode.Error:
                    return "";
            }
            return "";
        }

        //Перевод из строки в тип приложения
        public static ApplicationType ToApplicationType(this string m)
        {
            if (m == null) return ApplicationType.Error;
            switch (m.ToLower())
            {
                case "конструктор":
                case "constructor":
                    return ApplicationType.Constructor;
                case "рас":
                case "ras":
                    return ApplicationType.Ras;
                case "контроллер":
                case "controller":
                    return ApplicationType.Controller;
                case "анализатор":
                case "analyzer":
                    return ApplicationType.Analyzer;
                case "excel":
                    return ApplicationType.Excel;
                case "powerppoint":
                    return ApplicationType.PowerPoint;
                case "ошибка":
                case "error":
                    return ApplicationType.Error;
            }
            return ApplicationType.Error;
        }

        //Перевод из режима расчета в строку
        public static string ToRussian(this ApplicationType m)
        {
            switch (m)
            {
                case ApplicationType.Constructor:
                    return "Конструктор";
                case ApplicationType.Ras:
                    return "РАС";
                case ApplicationType.Controller:
                    return "Контроллер";
                case ApplicationType.Analyzer:
                    return "Анализатор";
                case ApplicationType.Excel:
                    return "Excel";
                case ApplicationType.PowerPoint:
                    return "PowerPoint";
            }
            return "Ошибка";
        }

        //Перевод из режима расчета в строку
        public static string ToEnglish(this ApplicationType m)
        {
            switch (m)
            {
                case ApplicationType.Constructor:
                    return "Constructor";
                case ApplicationType.Ras:
                    return "Ras";
                case ApplicationType.Controller:
                    return "Controller";
                case ApplicationType.Analyzer:
                    return "Analyzer";
                case ApplicationType.Excel:
                    return "Excel";
                case ApplicationType.PowerPoint:
                    return "PowerPoint";
            }
            return "Error";
        }

        //Перевод из режима расчета в режим натсроек
        public static ProviderSetupType ToProviderSetupType(this ApplicationType m)
        {
            switch (m)
            {
                case ApplicationType.Constructor:
                    return ProviderSetupType.Constructor;
                case ApplicationType.Controller:
                    return ProviderSetupType.Controller;
                case ApplicationType.Analyzer:
                    return ProviderSetupType.Analyzer;                
            }
            return ProviderSetupType.Reporter;
        }

        //Приложение является посторителем отчетов
        public static bool IsReport(this ApplicationType t)
        {
            return t == ApplicationType.Excel || t == ApplicationType.PowerPoint;
        }

        //Перевод строки в тип отчета 
        public static ReportType ToReportType(this string s)
        {
            if (s == null) return ReportType.Error;
            switch (s.ToLower())
            {
                case "calc":
                case "расчет":
                    return ReportType.Calc;
                case "template":
                case "шаблон":
                    return ReportType.Template;
                case "источник":
                case "source":
                    return ReportType.Source;
                case "excel":
                    return ReportType.Excel;
                case "powerpoint":
                    return ReportType.PowerPoint;
            }
            return ReportType.Error;
        }

        //Перевод типа шаблона в русское имя
        public static string ToEnglish(this ReportType t)
        {
            switch (t)
            {
                case ReportType.Calc:
                    return "Calc";
                case ReportType.Template:
                    return "Template";
                case ReportType.Source:
                    return "Source";
                case ReportType.Excel:
                    return "Excel";
                case ReportType.PowerPoint:
                    return "PowerPoint";
            }
            return "Ошибка";
        }

        //Перевод типа отчета в английское имя
        public static string ToRussian(this ReportType t)
        {
            switch (t)
            {
                case ReportType.Calc:
                    return "Расчет";
                case ReportType.Template:
                    return "Шаблон";
                case ReportType.Source:
                    return "Источник";
                case ReportType.Excel:
                    return "Excel";
                case ReportType.PowerPoint:
                    return "PowerPoint";
            }
            return "Error";
        }

        //Перевод строки в тип интервала 
        public static IntervalType ToIntervalType(this string s)
        {
            if (s.IsEmpty()) return IntervalType.Empty;
            switch (s.ToLower())
            {
                case "single":
                case "разовый":
                    return IntervalType.Single;
                case "named":
                case "именованный":
                    return IntervalType.Named;
                case "namedadd":
                    return IntervalType.NamedAdd;
                case "namedaddparams":
                    return IntervalType.NamedAddParams;
                case "periodic":
                    return IntervalType.Periodic;
                case "moments":
                case "мгновенные":
                    return IntervalType.Moments;
                case "base":
                case "базовый":
                    return IntervalType.Base;
                case "hour":
                case "часовой":
                    return IntervalType.Hour;
                case "day":
                case "суточный":
                    return IntervalType.Day;
                case "absolute":
                case "абсолютный":
                    return IntervalType.Absolute;
                case "absoluteday":
                case "абсолютныйсутки":
                    return IntervalType.AbsoluteDay;
                case "combined":
                case "комбинированный":
                    return IntervalType.Combined;
                case "absolutecombined":
                case "абсолютныйкомбинированный":
                    return IntervalType.AbsoluteCombined;
                case "absolutelistbase":
                case "абсолютныйбазовый":
                    return IntervalType.AbsoluteListBase;
                case "absolutelisthour":
                case "абсолютныйчасовой":
                    return IntervalType.AbsoluteListHour;
                case "absolutelistday":
                case "абсолютныйсуточный":
                    return IntervalType.AbsoluteListDay;
            }
            return IntervalType.Empty;
        }

        //Перевод типа интервала в английское имя
        public static string ToEnglish(this IntervalType t)
        {
            switch (t)
            {
                case IntervalType.Single:
                    return "Single";
                case IntervalType.Named:
                case IntervalType.NamedAdd:
                case IntervalType.NamedAddParams:
                    return "Named";
                case IntervalType.Periodic:
                    return "Periodic";
                case IntervalType.Moments:
                    return "Moments";
                case IntervalType.Base:
                    return "Base";
                case IntervalType.Hour:
                    return "Hour";
                case IntervalType.Day:
                    return "Day";
                case IntervalType.Absolute:
                    return "Absolute";
                case IntervalType.AbsoluteDay:
                    return "AbsoluteDay";
                case IntervalType.Combined:
                    return "Combined";
                case IntervalType.AbsoluteCombined:
                    return "AbsoluteCombined";
                case IntervalType.AbsoluteListBase:
                    return "AbsoluteListBase";
                case IntervalType.AbsoluteListHour:
                    return "AbsoluteListHour";
                case IntervalType.AbsoluteListDay:
                    return "AbsoluteListDay";
                case IntervalType.Empty:
                    return null;
            }
            return null;
        }

        //Перевод типа интервала в русское имя
        public static string ToRussian(this IntervalType t)
        {
            switch (t)
            {
                case IntervalType.Single:
                    return "Разовый";
                case IntervalType.Named:
                case IntervalType.NamedAdd:
                case IntervalType.NamedAddParams:
                    return "Именованный";
                case IntervalType.Periodic:
                    return "Периодический";
                case IntervalType.Moments:
                    return "Мгновенные";
                case IntervalType.Base:
                    return "Базовый";
                case IntervalType.Hour:
                    return "Часовой";
                case IntervalType.Day:
                    return "Суточный";
                case IntervalType.Absolute:
                    return "Абсолютный";
                case IntervalType.AbsoluteDay:
                    return "АбсолютныйСутки";
                case IntervalType.Combined:
                    return "Комбинированный";
                case IntervalType.AbsoluteCombined:
                    return "АбсолютныйКомбинированный";
                case IntervalType.AbsoluteListBase:
                    return "АбсолютныйБазовый";
                case IntervalType.AbsoluteListHour:
                    return "АбсолютныйЧасовой";
                case IntervalType.AbsoluteListDay:
                    return "АбсолютныйСуточный";
                case IntervalType.Empty:
                    return null;
            }
            return null;
        }

        //True, если тип интервал является именованным
        public static bool IsNamed(this IntervalType t)
        {
            return t == IntervalType.Named || t == IntervalType.NamedAdd || t == IntervalType.NamedAddParams;
        }

        //True, если тип интервала используется при разовом расчете
        public static bool IsSingle(this IntervalType t)
        {
            return t == IntervalType.Single || t.IsNamed();
        }

        //True, если тип интервала используется при периодическом накоплении
        public static bool IsPeriodic(this IntervalType t)
        {
            return t == IntervalType.Base || t == IntervalType.Hour || t == IntervalType.Day || t == IntervalType.Combined; 
        }

        //True, если тип интервала используется при абсолютном накоплении
        public static bool IsAbsolute(this IntervalType t)
        {
            return t == IntervalType.Absolute || t == IntervalType.AbsoluteDay || t == IntervalType.AbsoluteCombined;
        }

        //Название таблицы числовых значений архива по типу интервала
        public static string ToValuesTable(this IntervalType t)
        {
            if (t.IsNamed()) return "NamedValues";
            return t.ToEnglish() + "Values";
        }

        //Название таблицы строковых значений архива по типу интервала
        public static string ToStrValuesTable(this IntervalType t)
        {
            if (t.IsNamed()) return "NamedStrValues";
            return t.ToEnglish() + "StrValues";
        }

        //Название таблицы интервалов архива по типу интервала
        public static string ToIntervalsTable(this IntervalType t)
        {
            if (t.IsNamed()) return "NamedIntervals";
            return t.ToEnglish() + "Intervals";
        }

        //Номер часа, в начале которого удаляются старые интервалы указанного типа
        public static int HourNumber(this IntervalType t)
        {
            switch (t)
            {
                case IntervalType.Base: 
                    return 2;
                case IntervalType.Hour:
                    return 4;
                case IntervalType.Day:
                    return 6;
                case IntervalType.AbsoluteDay:
                    return 8;
                case IntervalType.Moments:
                    return 22;
            }
            return 0;
        }

        //Перевод строки в ImitMode
        public static ImitMode ToImitMode(this string t)
        {
            if (t == null) return ImitMode.Default;
            switch (t.ToLower())
            {
                case "отсчитывать от начала периода":
                case "frombegin":
                    return ImitMode.FromBegin;
                case "отсчитывать от начала часа":
                case "fromhour":
                    return ImitMode.FromHour;
                case "отсчитывать от начала суток":
                case "fromday":
                    return ImitMode.FromDay;
            }
            return ImitMode.Default;
        }

        //Перевод ImitMode в русскую строку
        public static string ToRussian(this ImitMode t)
        {
            switch (t)
            {
                case ImitMode.FromBegin:
                    return "Отсчитывать от начала периода";
                case ImitMode.FromHour:
                    return "Отсчитывать от начала часа";
                case ImitMode.FromDay:
                    return "Отсчитывать от начала суток";
            }
            return "Default";
        }
    }
}