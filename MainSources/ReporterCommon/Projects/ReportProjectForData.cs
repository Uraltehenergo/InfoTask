using System;
using BaseLibrary;
using Calculation;
using CommonTypes;
using VersionSynch;

namespace ReporterCommon
{
    //Один проект для добавления в файл данных отчета из файла проекта и архива
    public class ReportProjectForData : ReportProjectBase
    {
        //Загрузка из рекордсета ReporterData
        public ReportProjectForData(ReportBook book, IRecordRead rec) : base(book, rec)
        {
            ProjectChangeTime = Different.MinDate;
            ArchiveChangeTime = Different.MinDate;
            DataChangeTime = Different.MinDate;
            LoadArchive(rec, book);
        }

        //Дата последней компиляции проекта
        public DateTime ProjectChangeTime { get; set; }
        //Дата последней компиляции проекта записанного в архив
        public DateTime ArchiveChangeTime { get; set; }
        //Дата последней компиляции проекта записанного в файл данных
        public DateTime DataChangeTime { get; set; }

        //Определяет ProjectChangeTime и ArchiveChangeTime
        public void GetChangeTime()
        {
            try
            {
                if (!File.IsEmpty())
                    ProjectChangeTime = SysTabl.SubValueS(File, "CompileStatus", "LastTimeCompile").ToDateTime();
            }
            catch { }
            try
            {
                if (Archive != null)
                {
                    var dic = Archive.ReadProjects(ReportType.Calc);
                    if (dic.ContainsKey(Code))
                    {
                        ArchiveChangeTime = dic[Code].SourceChange;
                        Name = dic[Code].Name;
                    }
                }
            }
            catch { }
        }

        //Сохранение в рекордсет файла данных отчета
        public void ToRecordset(RecDao rec, bool isAddNew)
        {
            if (isAddNew) rec.AddNew();
            rec.Put("Project", Code);
            rec.Put("ProjectCode2", Code2);
            rec.Put("ProjectName", Name);
            rec.Put("ProjectFile", File);
        }

        //Получает в память список актуальный список параметров из файла данных, проекта и архива и обновляет список параметров в файле данных
        //Возвращает true, если параметры были обновлены
        public bool UpdateParams()
        {
            if (DataChangeTime >= ProjectChangeTime && DataChangeTime >= ArchiveChangeTime) return false;
            var dataParams = new DicS<ArchiveParam>();
            var projectParams = new DicS<ArchiveParam>();
            var archiveParams = new DicS<ArchiveParam>();
            try
            {
                if (!File.IsEmpty())
                {
                    Book.AddEvent("Получение списка параметров из проекта", CodeFinal);
                    const string stSql = "SELECT CalcParamsArchive.FullCode As Code, CalcParams.Code AS CodeParam, CalcSubParams.Code AS CodeSubParam, CalcParams.Name, CalcSubParams.Name AS SubName, " +
                                         "CalcParamsArchive.DataType, CalcParams.CalcParamType, CalcParamsArchive.SuperProcessType, CalcParams.Comment, CalcSubParams.Comment AS SubComment, " +
                                         "CalcParamsArchive.Units, CalcParams.Task, CalcParamsArchive.Min, CalcParamsArchive.Max, CalcParamsArchive.DecPlaces, CalcParams.Tag " +
                                         "FROM CalcParams INNER JOIN (CalcSubParams RIGHT JOIN CalcParamsArchive ON CalcSubParams.CalcParamId = CalcParamsArchive.CalcSubParamId) ON CalcParams.CalcParamId = CalcParamsArchive.CalcParamId;";
                    using (var rec = new ReaderAdo(File, stSql))
                        while (rec.Read())
                        {
                            var ap = new ArchiveParam(rec);
                            projectParams.Add(ap.FullCode, ap);
                        }
                    
                    if (CalcMode == CalcModeType.Internal)
                    {
                        Book.AddEvent("Получение параметров ручного ввода из проекта", CodeFinal);
                        using (var rec = new ReaderAdo(File, "SELECT CalcParams.Code, CalcParams.Code AS CodeParam, CalcParams.Name, CalcParams.ResultType AS DataType, CalcParams.Units, CalcParams.Task, CalcParams.CalcParamType, CalcParams.Comment, CalcParams.SuperProcessType, CalcParams.Min, CalcParams.Max, CalcParams.DecPlaces, CalcParams.Tag " +
                                                             "FROM CalcParams WHERE CalcParamType Is Not Null"))
                            while (rec.Read())
                            {
                                var ap = new ArchiveParam(rec);
                                projectParams.Add(ap.FullCode, ap);
                            }    
                    }
                }
                if (Archive != null && CalcMode != CalcModeType.Internal)
                {
                    Book.AddEvent("Получение списка параметров из архива", CodeFinal);
                    foreach (var ap in Archive.ReadParams(Code, ReportType.Calc))
                        archiveParams.Add(ap.FullCode, ap);
                }

                var dic1 = ProjectChangeTime >= ArchiveChangeTime ? projectParams : archiveParams;
                var dic2 = ProjectChangeTime < ArchiveChangeTime ? projectParams : archiveParams;

                Book.AddEvent("Обновление параметров в файле данных", CodeFinal);
                using (var rec = new RecDao(Book.DataFile, "SELECT * FROM CalcParams WHERE Project='" + CodeFinal + "'"))
                {
                    while (rec.Read())
                    {
                        var ap = new ArchiveParam(rec);
                        dataParams.Add(ap.FullCode, ap);
                        if (!dic1.ContainsKey(ap.FullCode) && !dic2.ContainsKey(ap.FullCode))
                            rec.Put("SysField", "Del");
                        else
                        {
                            rec.Put("SysField", "");
                            if (dic1.ContainsKey(ap.FullCode))
                                dic1[ap.FullCode].ToRecordset(rec, CodeFinal);
                            else dic2[ap.FullCode].ToRecordset(rec, CodeFinal);
                            rec.Put("FromProject", projectParams.ContainsKey(ap.FullCode));
                            rec.Put("FromArchive", archiveParams.ContainsKey(ap.FullCode));
                        }
                    }

                    Book.AddEvent("Добавление недостающих параметров в файл данных", CodeFinal);
                    foreach (var ap in dic1.Values)
                        if (!dataParams.ContainsKey(ap.FullCode))
                        {
                            dataParams.Add(ap.FullCode, ap);
                            ap.ToRecordset(rec, CodeFinal, true);
                            rec.Put("FromProject", projectParams.ContainsKey(ap.FullCode));
                            rec.Put("FromArchive", archiveParams.ContainsKey(ap.FullCode));
                        }
                    foreach (var ap in dic2.Values)
                        if (!dataParams.ContainsKey(ap.FullCode))
                        {
                            ap.ToRecordset(rec, CodeFinal, true);
                            rec.Put("FromProject", projectParams.ContainsKey(ap.FullCode));
                            rec.Put("FromArchive", archiveParams.ContainsKey(ap.FullCode));
                        }

                    using (var rep = new RecDao(rec.DaoDb, "SELECT LastChange FROM Projects WHERE (ProjectCode2='" + CodeFinal + "') OR (ProjectCode2 Is Null AND Project = '" + CodeFinal + "')"))
                        rep.Put("LastChange", ProjectChangeTime >= ArchiveChangeTime ? ProjectChangeTime : ArchiveChangeTime);
                }
            }
            catch (Exception ex)
            {
                Book.AddError("Ошибка при обновлении спиcка параметров", ex, CodeFinal);
            }
            return true;
        }
    }
}
