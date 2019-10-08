using System;
using System.Collections.Generic;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace ReporterCommon
{
    //-----------------------------------------------------------------------------------------
    //Один проект для формирования отчетов
    public class ReportProject : ReportProjectBase
    {
        //Загрузка из рекордсета ReporterData (recd) и файла данных отчета (rec)
        public ReportProject(ReportBook book, IRecordRead recd, IRecordRead rec) 
            : base(book, recd)
        {
            IsUsed = rec.GetBool("IsUsed");
            IsHandInput = rec.GetBool("IsHandInput");
            if (IsUsed) LoadArchive(recd, book);
            if (IsHandInput) GetHandInputFile();
        }

        //Список используемых параметров отчета
        private readonly DicS<ReportParam> _params = new DicS<ReportParam>();
        public DicS<ReportParam> Params { get { return _params; } }

        //Путь к файлу ручного ввода
        private string _handInputFile;

        //Архивный отчет для получения значений параметров проекта
        private ArchiveReport _archiveReport;
        //Типы интервалов для получения отчета
        private readonly HashSet<IntervalType> _intervalTypes = new HashSet<IntervalType>();

        //Абсолютные значения прочитаные и для записи, ключи - коды проектов и параметров
        public DicS<HandInputParam> AbsoluteEditValues { get; set; }

        //Определяет файл ручного ввода для проекта
        private void GetHandInputFile()
        {
            string name = "";
            using (var rec = new RecDao(File, "SELECT ProviderName FROM Providers WHERE (ProviderCode='HandInputSource') OR (ProviderCode='HandInputSqlSource')"))
                if (!rec.NoMatch())
                    name = rec.GetString("ProviderName");
            using (var rec = new RecDao(General.ControllerFile, "SELECT ProviderInf FROM Providers WHERE (ThreadId=" + Book.ThreadId + ") AND (ProviderName='" + name + "')"))
                if (!rec.NoMatch())
                    _handInputFile = rec.GetString("ProviderInf").ToPropertyDicS()["CloneFile"];
        }

        //Подготовка архивного отчета для чтения значений
        public void PrepareArchiveReport()
        {
            if (_archiveReport == null)
            {
                Book.AddEvent("Подготовка архивного отчета", CodeFinal);
                try
                {
                    _archiveReport = new ArchiveReport(CodeFinal + "_" + Book.FullThreadName, Name, ReportType.Excel, Book.LastChangeLinks);
                    if (!IsSave && !IsSystem)
                        foreach (var param in Params.Values)
                            if (param.IntervalTypes.Count > 0)
                            {
                                var ap = param.ArchiveParam;
                                var arp = new ArchiveReportParam(param.FullCode, Code, ap.DataType, ap.SuperProcess, ap.FirstParam.CalcParamType, param.IntervalTypes);
                                _archiveReport.AddParam(arp);
                                foreach (var cell in param.Cells)
                                    if (arp.Queries.ContainsKey(cell.IntervalType))
                                        cell.ArchiveQueryValues = arp.Queries[cell.IntervalType];
                                foreach (var shape in param.Shapes)
                                    if (arp.Queries.ContainsKey(shape.IntervalType))
                                        shape.ArchiveQueryValues = arp.Queries[shape.IntervalType];
                                foreach (var t in param.IntervalTypes)
                                    if (t != IntervalType.Empty && !_intervalTypes.Contains(t))
                                        _intervalTypes.Add(t);
                            }
                    using (Book.Start(30))
                        Archive.PrepareReport(_archiveReport);
                }
                catch (Exception ex)
                {
                    Book.AddError("Ошибка подготовки архивного расчета", ex);
                }
            }
        }

        //Чтение значений из архива
        public void ReadArchiveReport(DateTime begin, DateTime end, string calcName)
        {
            try
            {
                _archiveReport.IntervalsForRead.Clear();
                foreach (var t in _intervalTypes)
                    _archiveReport.IntervalsForRead.Add(new ArchiveInterval(t, begin, end, calcName));
                Archive.ReadReport(_archiveReport);
            }
            catch (Exception ex)
            {
                Book.AddError("Ошибка при чтении из архива", ex, CodeFinal);
            }
        }

        //Запись значений ручного ввода, beg - начало периода расчета
        public void WriteHandInput(DateTime beg, DateTime en)
        {
            if (IsHandInput)
            {
                try
                {
                    Book.AddEvent("Сохрание значений ручного ввода из ячеек", CodeFinal);
                    var pars = new DicS<ReportHandParam>();
                    foreach (var param in Params.Values)
                        if (param.HandInputCell != null)
                            pars.Add(param.FullCode,
                                     new ReportHandParam(param, GeneralRep.ActiveBook.ValueFromCell(param.HandInputCell)));
                    using (var db = new DaoDb(_handInputFile))
                    {
                        using (
                            var rec = new RecDao(db,
                                                 "SELECT Objects.CodeObject, MomentsValues.Time FROM (Objects INNER JOIN Signals ON Objects.ObjectId = Signals.ObjectId) INNER JOIN MomentsValues ON Signals.SignalId = MomentsValues.SignalId " +
                                                 "WHERE (Time >= " + beg.ToAccessString() + ") AND (Time < " + en.ToAccessString() + ")"))
                            while (rec.Read())
                                if (pars.ContainsKey(rec.GetString("CodeObject")))
                                    rec.Put("Time", Different.MaxDate);
                        using (
                            var rec = new RecDao(db,
                                                 "SELECT Objects.CodeObject, MomentsStrValues.Time FROM (Objects INNER JOIN Signals ON Objects.ObjectId = Signals.ObjectId) INNER JOIN MomentsStrValues ON Signals.SignalId = MomentsStrValues.SignalId " +
                                                 "WHERE (Time >= " + beg.ToAccessString() + ") AND (Time < " + en.ToAccessString() + ")"))
                            while (rec.Read())
                                if (pars.ContainsKey(rec.GetString("CodeObject")))
                                    rec.Put("Time", Different.MaxDate);
                        db.Execute("DELETE * FROM MomentsValues WHERE Time=" + Different.MaxDate.ToAccessString());
                        db.Execute("DELETE * FROM MomentsStrValues WHERE Time=" + Different.MaxDate.ToAccessString());
                        using (
                            var rec = new RecDao(db,
                                                 "SELECT Objects.CodeObject, Objects.ObjectId, Signals.CodeSignal, Signals.SignalId FROM Objects INNER JOIN Signals ON Objects.ObjectId = Signals.SignalId")
                            )
                            while (rec.Read())
                            {
                                var code = rec.GetString("CodeObject");
                                if (pars.ContainsKey(code))
                                {
                                    pars[code].ObjectId = rec.GetInt("ObjectId");
                                    pars[code].SignalId = rec.GetInt("SignalId");
                                }
                            }
                        using (var reco = new RecDao(db, "Objects"))
                        using (var recs = new RecDao(db, "Signals"))
                            foreach (var param in pars.Values)
                                if (param.ObjectId == 0)
                                {
                                    var ap = param.ReportParam.ArchiveParam;
                                    var par = ap.FirstParam;
                                    reco.AddNew();
                                    reco.Put("CodeObject", par.Code);
                                    reco.Put("NameObject", par.Name);
                                    param.ObjectId = reco.GetInt("ObjectId");
                                    reco.Update();
                                    recs.AddNew();
                                    recs.Put("ObjectId", param.ObjectId);
                                    recs.Put("CodeSignal", "Руч");
                                    recs.Put("NameSignal", "Ручной ввод");
                                    recs.Put("FullCode", par.Code + ".Руч");
                                    recs.Put("Default", true);
                                    recs.Put("DataType", ap.DataType.ToRussian());
                                    recs.Put("Units", ap.Units);
                                    recs.Put("Min", ap.Min);
                                    recs.Put("Max", ap.Max);
                                    recs.Put("Active", true);
                                    param.SignalId = recs.GetInt("SignalId");
                                    recs.Update();
                                }
                        using (var rec = new RecDao(db, "MomentsValues"))
                        using (var recs = new RecDao(db, "MomentsStrValues"))
                            foreach (var param in pars.Values)
                                if (!param.StringValue.IsEmpty())
                                {
                                    var dt = param.ReportParam.ArchiveParam.DataType;
                                    var r = dt.LessOrEquals(DataType.Real) ? rec : recs;
                                    r.AddNew();
                                    r.Put("SignalId", param.SignalId);
                                    r.Put("Time", beg);
                                    if (dt.LessOrEquals(DataType.Real))
                                    {
                                        var d = param.StringValue.ToDouble();
                                        if (!double.IsNaN(d))
                                            r.Put("Value", d);
                                        else
                                        {
                                            r.Put("Value", 0);
                                            r.Put("Nd", 1);
                                        }
                                    }
                                    else if (dt == DataType.String)
                                        r.Put("StrValue", param.StringValue);
                                    else if (dt == DataType.Time)
                                    {
                                        var t = param.StringValue.ToDateTime();
                                        r.Put("TimeValue", t);
                                        if (t == Different.MinDate) r.Put("Nd", 1);
                                    }
                                }
                        using (var sys = new SysTabl(db, false))
                        {
                            var d = sys.Value("BeginInterval").ToDateTime();
                            if (d == Different.MinDate || d > beg) sys.PutValue("BeginInterval", beg.ToString());
                            d = sys.Value("EndInterval").ToDateTime();
                            if (d < beg) sys.PutValue("EndInterval", beg.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Book.AddError("Ошибка при записи ручного ввода", ex);
                }    
            }
        }
    }
}
