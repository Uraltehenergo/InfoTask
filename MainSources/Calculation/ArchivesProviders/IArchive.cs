using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Стандартный интерфейс архива
    public interface IArchive : IProvider
    {
        //Загрузка проекта и списка параметров по проекту, добавление недостающих в архив, зыаполняются id
        void PrepareProject(ArchiveProject project);
        //Запись данных по проекту project, записываются интервалы и значения, begin, end - период обработки
        void WriteProject(ArchiveProject project, DateTime begin, DateTime end);
        //Получение значений по проекту project, в параметры записываются значения по интервалам в соответствии с заказом
        void ReadProject(ArchiveProject project);

        //Загрузка списка параметров по отчету, добавление недостающих в архив
        void PrepareReport(ArchiveReport report);
        //Получение значений по отчету report, в параметры записываются значения по интервалам в соответствии с заказом
        void ReadReport(ArchiveReport report);

        //Запись абсолютных значений по списку pars можно из разных проектов
        void WriteAbsoluteEdit(List<HandInputParam> pars);
        //Чтение абсолютных значений по проекту project, возвращает словарь параметров со значениями, ключи - коды в нижнем регистре
        //Если onlyEdit, то читаем только введенные значений, иначе еще и посчитанные
        DicS<HandInputParam> ReadAbsoluteEdit(string project, bool onlyEdit);
        //True, если был произведен ручной ввод абсолютных значений
        bool IsAbsoluteEdited(string project);
        
        //Получение списка всех проектов из архива, проекты возвращаются без списка параметров
        DicS<ArchiveProject> ReadProjects(ReportType type = ReportType.Error);
        //Получение списка всех отчетов из архива, отчеты возвращаются без списка параметров
        DicS<ArchiveReport> ReadReports(ReportType type = ReportType.Error);
        //Получение списка расчетных параметров по проекту, project - код проекта, projectType - тип проекта
        List<ArchiveParam> ReadParams(string project, ReportType projectType);
        //Получение списка разовых интервалов без значений по проекту, project - код проекта, projectType - тип проекта
        List<ArchiveInterval> ReadIntervals(string project, ReportType projectType);
        //Удаление списка указанных интервалов по проекту, project - код проекта
        void DeleteIntervals(string project, ReportType projectType, List<ArchiveInterval> intervals);
        //Получение времени архива по всем типам интервалов
        Dictionary<IntervalType, TimeInterval> ReadRanges(string project, ReportType projectType);
        //Полная очистка архива
        void ClearArchive();
    }
}