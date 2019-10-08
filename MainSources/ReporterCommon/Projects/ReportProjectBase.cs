using System;
using BaseLibrary;
using Calculation;
using CommonTypes;
using VersionSynch;

namespace ReporterCommon
{
    //Базовый класс для ReportProjectForData, ReportProjectForLinks и ReportProject
    public class ReportProjectBase : ReportProjectSetup
    {
        //Загрузка из рекордсета ReporterData или файла данных отчета
        public ReportProjectBase(ReportBook book, IRecordRead rec)
        {
            Book = book;
            Code = rec.GetString("Project");
            Code2 = rec.GetString("ProjectCode2");
            Name = rec.GetString("ProjectName");
            var prs = book.SysPage.GetProjects();
            if (prs.ContainsKey(CodeFinal))
                CalcMode = prs[CodeFinal].CalcMode;
            File = rec.GetString("ProjectFile");
            if (!DbVersion.IsProject(File)) File = null;
        }

        //Создание провайдера архива
        protected void LoadArchive(IRecordRead rec, Logger logger)
        {
            try
            {
                if (!rec.GetString("CodeArchive").IsEmpty() && !rec.GetString("InfArchive").IsEmpty())
                    Archive = (IArchive)General.RunProvider(rec.GetString("CodeArchive"), CodeFinal + "_Archive", rec.GetString("InfArchive"), logger, ProviderSetupType.ReporterArchive);
                if (Archive == null || !Archive.Check())
                {
                    if (this is ReportProject)
                        Book.AddError("Не удалось соединиться с архивом. Следует проверить настройки", null, CodeFinal + "_Archive");
                    Archive.Dispose();
                    Archive = null;
                }
            }
            catch (Exception ex)
            {
                if (this is ReportProject)
                    Book.AddError("Не удалось соединиться с архивом. Следует проверить настройки", ex, CodeFinal + "_Archive");
                Archive = null;
            }
        }

        //Имя
        public string Name { get; protected set; }
        //Ссылка на книгу
        public ReportBook Book { get; private set; }
        //Провайдер архива
        public IArchive Archive { get; private set; }
        //Файл проекта
        public string File { get; private set; }

        //Проект используется в отчете
        public bool IsUsed { get; set; }
        //В отчете есть ручной ввод для данного проекта
        public bool IsHandInput { get; set; }
        //Является проектом Сохранение
        public bool IsSave { get { return Code == "Сохранение"; } }
        //Является проектом Системные
        public bool IsSystem { get { return Code == "Системные"; } }
    }
}
