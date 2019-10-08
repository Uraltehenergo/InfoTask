using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;
using ControllerClient;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Shape = Microsoft.Office.Interop.Excel.Shape;

namespace ReporterCommon
{
    //Один отчет вместе со всеми открытыми формами, потоком и архивом
    public partial class ReportBook : Logger
    {
        //Словарь проектов, используемых при получении данных, ключи - коды
        public DicS<ReportProject> UsedProjects { get; set; }
        //Словарь проектов, используемых при ручном вводе, ключи - коды
        public DicS<ReportProject> HandInputProjects { get; set; }
        //Словарь листов книги с заполняемыми ячейками, ключи - имена, значения списки ссылок
        public DicS<List<ReportCell>> Pages { get; set; }
        //Словарь листов книги с заполняемыми фигурами, ключи - имена листов и Id фигур
        internal DicS<DicI<ReportShape>> PagesShape { get; set; }
        //В отчете только ссылки на абсолютное накопление
        public bool OnlyAbsolute { get; private set; }

        //Id потока расчета или 0
        public int ThreadId { get; private set; }
        //Настройки архива - журнала расчетов
        private string _archiveCode;
        private string _archiveInf;
        
        //Настройки расположения итогового расчета (Бланк;Каталог;Файл и т.д.)
        public DicS<string> FormInf { get; private set; }
        //Производится формирование в каталог, файл
        public bool FormToDir { get; private set; }
        public bool FormToFile { get; private set; }
        //Книга, в котрой формируется отчет
        public Workbook FormingBook { get; set; }
        //Книга, в котрую формируется отчет
        public Workbook ResultBook { get; set; }

        //Поток контроллера
        public Controller Controller { get; set; }
        //Провайдер архива для журнала отчетов
        public IArchive Archive { get; set; }
        //Список диапазонов интервалов архивов и источников
        private readonly List<IntervalRange> _ranges = new List<IntervalRange>();
        public List<IntervalRange> Ranges { get { return _ranges; }}
        //Текущий проект отчета с параметрами для архива
        private ArchiveProject _archiveProject;
        //Текущий интервал заполнения и имя отчета
        public ArchiveInterval Interval { get; set; }
        //Выполняется заполнение отчета
        public bool IsReportForming { get; set; }

        //Очищает все данные формы заполнения отчетов
        public void ClearReportDataFromMemory()
        {
            HandInputProjects = null;
            UsedProjects = null;
            Pages = null;
            _archiveProject = null;
            Interval = null;
            if (Archive != null)
            {
                Archive.Dispose();
                Archive = null;
            }
            if (Controller != null)
            {
                try
                {
                    Controller.Close();
                    Controller = null;
                }
                catch { }
            }
        }

        //Обновляет Ranges 
        public TimeInterval UpdateRanges()
        {
            try
            {
                Ranges.Clear();
                if (Controller != null)
                {
                    AddEvent("Получение времени источников");
                    Controller.GetSourcesTime();
                    var times = Controller.SourcesTimeString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var st in times)
                    {
                        var ie = st.IndexOf("=");
                        var im = st.IndexOf("-");
                        var name = st.Substring(0, ie);
                        var beg = st.Substring(ie + 1, im - ie - 1).ToDateTime();
                        var en = st.Substring(im + 1).ToDateTime();
                        Ranges.Add(new IntervalRange(beg, en, name, IntervalType.Empty));
                    }
                }
                foreach (var pr in UsedProjects.Values)
                    if (pr.Archive != null && pr.CalcMode != CalcModeType.Internal)
                    {
                        AddEvent("Получение диапазонов интервалов архива", "Проект: " + pr.CodeFinal);
                        var dic = pr.Archive.ReadRanges(pr.CodeFinal, ReportType.Calc);
                        foreach (var t in dic)
                            Ranges.Add(new IntervalRange(t.Value, pr.Archive.Name, t.Key));
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при обновлении диапазона исходных данных", ex);
            }
            return SourcesRange();
        }

        //Текущий интервал исходных данных для отчета
        public TimeInterval SourcesRange()
        {
            var beg = Different.MaxDate;
            var en = Different.MinDate;
            foreach (var range in Ranges)
            {
                if (range.Begin < beg) beg = range.Begin;
                if (range.End > en) en = range.End;
            }
            if (beg == Different.MaxDate) beg = Different.MinDate;
            if (en == Different.MinDate) en = Different.MaxDate;
            return new TimeInterval(beg, en);
        }

        //Загрузка только используемых параметров и ссылок из файла данных в память (UsedProjects, HandInputProjects, Pages)
        private void UsedFileToMemory()
        {
            if (UsedProjects != null) return;
            
            AddEvent("Загрузка используемых параметров и ссылок в память");
            try
            {
                UsedProjects = new DicS<ReportProject>();
                HandInputProjects = new DicS<ReportProject>();
                Pages = new DicS<List<ReportCell>>();
                PagesShape = new DicS<DicI<ReportShape>>();
                var paramsId = new Dictionary<int, ReportParam>();
                using (var rec = new ReaderAdo(General.ReporterFile, "SELECT * FROM Reports WHERE Report='" + Code + "'"))
                {
                    ThreadId = rec.GetInt("ThreadId");
                    _archiveCode = rec.GetString("CodeArchive");
                    _archiveInf = rec.GetString("InfArchive");
                    FormInf = rec.GetString("FormInf").ToPropertyDicS();
                    FormToDir = FormInf["FormTo"] == "Dir";
                    FormToFile = FormInf["FormTo"] == "File";
                }
                using (var db = new DaoDb(DataFile))
                {
                    using (var recd = new RecDao(General.ReporterFile, "SELECT Projects.* FROM Reports INNER JOIN Projects ON Reports.ReportId = Projects.ReportId WHERE Reports.Report='" + Code + "'"))
                        using (var rec = new RecDao(db, "SELECT * FROM Projects"))
                            while (rec.Read())
                            {
                                var code = rec.GetString("Project");
                                var code2 = rec.GetString("ProjectCode2");
                                if (code2.IsEmpty()) recd.FindFirst("Project", code);
                                else recd.FindFirst("(ProjectCode2='" + code2 + "') AND (Project='" + code + "')");
                                if (!recd.NoMatch())
                                {
                                    if (rec.GetBool("IsHandInput"))
                                    {
                                        var proj = new ReportProject(this, recd, rec);
                                        HandInputProjects.Add(proj.CodeFinal, proj);
                                    }
                                    if (rec.GetBool("IsUsed"))
                                    {
                                        var proj = new ReportProject(this, recd, rec);
                                        UsedProjects.Add(proj.CodeFinal, proj);
                                    }    
                                }
                            }

                    using (var rec = new ReaderAdo(db, "SELECT * FROM CalcParams"))
                        while (rec.Read())
                        {
                            var par = new ReportParam(rec);
                            par.PropsFromRecordset(rec);
                            paramsId.Add(par.Id, par);
                            if (rec.GetBool("IsHandInput"))
                            {
                                var proj = HandInputProjects[par.Project];
                                proj.Params.Add(par.FullCode, par);
                            }
                            if (rec.GetBool("IsUsed") && par.Project != "Системные")
                                UsedProjects[par.Project].Params.Add(par.FullCode, par);
                        }
                    
                    using (var rec = new ReaderAdo(db, "SELECT * FROM Cells ORDER BY Page, Y, X"))
                        while (rec.Read())
                        {
                            var cell = new ReportCell(rec);
                            if (paramsId.ContainsKey(cell.ParamId))
                            {
                                var par = paramsId[cell.ParamId];
                                cell.Param = par;
                                par.Cells.Add(cell);
                                if (!Pages.ContainsKey(cell.Page))
                                    Pages.Add(cell.Page, new List<ReportCell>());
                                Pages[cell.Page].Add(cell);
                                if (cell.LinkType == LinkType.HandInput)
                                    par.HandInputCell = cell;
                            }
                        }

                    using (var rec = new ReaderAdo(db, "SELECT * FROM Shapes ORDER BY Page, ShapeType DESC, ShapeId"))
                        while (rec.Read())
                        {
                            var shape = new ReportShape(rec);
                            if (paramsId.ContainsKey(shape.ParamId))
                            {
                                var par = paramsId[shape.ParamId];
                                shape.Param = par;
                                par.Shapes.Add(shape);
                                if (!PagesShape.ContainsKey(shape.Page))
                                    PagesShape.Add(shape.Page, new DicI<ReportShape>());
                                PagesShape[shape.Page].Add(shape.Id, shape);
                            }
                        }

                    foreach (var sh in Workbook.Sheets)
                        if (sh is Worksheet)
                        {
                            var sheet = (Worksheet)sh;
                            if (PagesShape.ContainsKey(sheet.Name))
                            {
                                foreach (Shape shape in sheet.Shapes)
                                {
                                    var page = PagesShape[sheet.Name];
                                    if (page.ContainsKey(shape.ID))
                                        page[shape.ID].SetShape(shape);
                                    if (shape.Type == MsoShapeType.msoGroup)
                                        foreach (Shape gshape in shape.GroupItems)
                                            if (page.ContainsKey(gshape.ID))
                                                page[gshape.ID].SetShape(gshape);
                                }
                            }
                        }

                    using (var sys = new SysTabl(db))
                        OnlyAbsolute = sys.Value("OnlyAbsolute") == "True";
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при загрузке ссылок", ex);
            }
        }

        //Подготовка контроллера к работе, false - если случилась ошибка
        public bool PrepareController()
        {
            if (ThreadId != 0 && Controller == null)
            {
                AddEvent("Подготовка контроллера");
                Controller = new Controller();
                if (!Controller.OpenLocal(ThreadId).IsEmpty())
                {
                    AddError("Ошибка загрузки контроллера расчетов");
                    Controller.Close();
                    Controller = null;
                    return false;
                }    
            }
            return true;
        }

        //Подготовка журнала отчетов, false - если случилась ошибка
        public bool PrepareArchive()
        {
            if (Archive == null)
            {
                AddEvent("Подготовка архива журнала отчетов");
                Archive = (IArchive)General.RunProvider(_archiveCode, "ReportArchive", _archiveInf, this, ProviderSetupType.ReporterArchive);
                if (Archive == null || !Archive.Check())
                {
                    AddError("Не удалось соединиться с архивом журнала отчетов");
                    if (Archive != null) Archive.Dispose();
                    Archive = null;
                    return false;
                }
            }
            return true;
        }
        
        //Задает книгу, в которую формируется отчет, beg - начало расчета, name - имя расчета
        public void OpenFormingBook(DateTime beg, DateTime en, string name)
        {
            if (!FormToDir && !FormToFile)
            {
                FormingBook = Workbook;
                return;
            }
            string f = "";
            try
            {
                if (FormToDir)
                {
                    f = FormInf["ResultDir"];
                    if (!f.EndsWith(@"\")) f += @"\";
                    if (!FormInf["ResultFileName"].IsEmpty()) f += FormInf["ResultFileName"];
                    else f += Workbook.Name.Substring(0, Workbook.Name.Length - 5);
                    if (FormInf.GetBool("AddDateToName"))
                        f += " " + string.Format("{0:" + FormInf["DateNameFormat"] + "}", DateTime.Now);
                    if (FormInf.GetBool("AddBeginToName"))
                        f += " " + string.Format("{0:" + FormInf["BeginNameFormat"] + "}", beg);
                    if (FormInf.GetBool("AddEndToName"))
                        f += " " + string.Format("{0:" + FormInf["EndNameFormat"] + "}", en);
                    if (FormInf.GetBool("AddCalcNameToName") && !name.IsEmpty()) f += " " + name;
                    f += ".xlsx";
                }
                else if (FormToFile)
                {
                    var file = new FileInfo(FormInf["ResultFile"]);
                    f = file.Directory + @"\Tmp_" + file.Name;
                }
                CloseBookByFullName(f);
                GeneralRep.ProgrammOpening = true;
                new FileInfo(Workbook.FullName).CopyTo(f, true);
                FormingBook = GeneralRep.Application.Workbooks.Open(f);
            }
            catch (Exception ex)
            {
                AddError("Ошибка при открытии книги для формирования отчета", ex, f);
                GeneralRep.Application.DisplayAlerts = true;
                FormingBook = null;
            }
        }

        //Копирует сформированные листы в конечную книгу, возвращает ссылку на сформированную книгу
        public void CopySheetsToBook(DateTime beg, string name, string fillPages)
        {
            if (FormingBook == null) return;
            ResultBook = FormingBook;
            if (FormToFile)
            {
                try
                {
                    var rfile = FormInf["ResultFile"];
                    CloseBookByFullName(rfile);
                    DaoDb.FromTemplate(General.ReporterDir + "ReportTemplate.xlsx", rfile, ReplaceByTemplate.IfNotExists);
                    GeneralRep.ProgrammOpening = true;
                    ResultBook = GeneralRep.Application.Workbooks.Open(rfile);
                    foreach (var ws in FormingBook.GetSheets())
                        if (IsFillPage(ws, fillPages))
                        {
                            string s = "";
                            if (FormInf.GetBool("AddSheetToName"))
                                s += ws.Name;
                            if (FormInf.GetBool("AddDateToName"))
                                s += (s == "" ? "" : " ") + string.Format("{0:" + FormInf["DateNameFormat"] + "}", DateTime.Now);
                            if (FormInf.GetBool("AddBeginToName"))
                                s += (s == "" ? "" : " ") + string.Format("{0:" + FormInf["BeginNameFormat"] + "}", beg);
                            if (FormInf.GetBool("AddEndToName"))
                                s += (s == "" ? "" : " ") + string.Format("{0:" + FormInf["EndNameFormat"] + "}", beg);
                            if (FormInf.GetBool("AddCalcNameToName") && !name.IsEmpty())
                                s += (s == "" ? "" : " ") + name;
                            s = s.Length <= 30 ? s : s.Substring(0, 29);
                            Worksheet prevws = null, nextws = null;
                            for (int i = 1; i <= ResultBook.Sheets.Count; i++)
                                if (ResultBook.Sheets[i] is Worksheet && ((Worksheet)ResultBook.Sheets[i]).Name == s)
                                {
                                    ResultBook.Application.DisplayAlerts = false;
                                    var curws = (Worksheet) ResultBook.Sheets[i];
                                    prevws = curws.Previous;
                                    nextws = curws.Next;
                                    curws.Delete();
                                    ResultBook.Application.DisplayAlerts = true;
                                }

                            int last = ResultBook.Worksheets.Count;
                            if (nextws != null)
                                ws.Copy(nextws, Type.Missing);
                            else if (prevws != null)
                                ws.Copy(Type.Missing, prevws);
                            else 
                                ws.Copy(Type.Missing, ResultBook.Worksheets[last]);
                            try
                            {
                                for (int i = 1; i <= ResultBook.Sheets.Count; i++)
                                    if (ResultBook.Sheets[i] is Worksheet && ((Worksheet)ResultBook.Sheets[i]).Name == ws.Name)
                                        ((Worksheet)ResultBook.Sheets[i]).Name = s;
                            }
                            catch (Exception ex)
                            {
                                AddError("Не удалось создать лист " + s + ". Возможно название листа содержит недопустимые символы или лист с таким именем уже есть в книге", ex);
                            }
                        }
                    CloseBookByFullName(FormingBook.FullName, true);
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при копировании листов отчета", ex);
                }
            }
            if (FormToFile || FormToDir)
                foreach (var sheet in ResultBook.GetSheets(false))
                {
                    if (sheet.Name == "SysPage" || sheet.Name == "Templates")
                    {
                        try
                        {
                            GeneralRep.Application.DisplayAlerts = false;
                            sheet.Delete();
                        }
                        catch { }
                        GeneralRep.Application.DisplayAlerts = true;
                    }
                    else sheet.UsedRange.ClearComments();
                }
            ResultBook.Save();
        }

        //Закрыть книгу с указанным полным путем, если она открыта, delete - после закрытия книгу удалить
        private void CloseBookByFullName(string file, bool delete = false)
        {
            try
            {
                foreach (Workbook wb in GeneralRep.Application.Workbooks)
                {
                    if (wb.FullName.ToLower() == file.ToLower())
                    {
                        GeneralRep.Application.DisplayAlerts = false;
                        wb.Close();
                    }
                }
                if (delete) new FileInfo(file).Delete();
            }
            finally { GeneralRep.Application.DisplayAlerts = true;}
        }

        //Получение строкового значения из ячейки, если ячейка не заполнена, то для строковых параметров возвращается "", иначе null
        public string ValueFromCell(ReportCell cell)
        {
            var sheet = Workbook.Sheets[cell.Page];
            if (!(sheet is Worksheet)) return "";
            var ws = (Worksheet)sheet;
            var v = ((Range)ws.Cells[cell.Y, cell.X]).Value2;
            if (v == null) return cell.DataType == DataType.String ? "" : null;
            if (cell.DataType != DataType.Time) return v.ToString();
            try { return DateTime.FromOADate((double)v).ToString();}
            catch {return null;}
        }
        
        //Считывает значения по отчету из архива за указанный период и заполняет значения в памяти (Cells)
        public void ReadArchiveReport()
        {
            try
            {
                double proc = 0;
                if (UsedProjects.Count > 0)
                    foreach (var proj in UsedProjects.Values)
                    {
                        Start(proj.PrepareArchiveReport, proc, proc += 20.0/UsedProjects.Count);
                        using (Start(proc, proc += 60.0/UsedProjects.Count))
                            proj.ReadArchiveReport(Interval.Begin, Interval.End, Interval.Name);
                    }
                Start(ReadFromArchiveToCells, 80);
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении из архива", ex);
            }
        }

        //Чтение данных из архивного расчета и формирование значений для ячеек
        private void ReadFromArchiveToCells()
        {
            AddEvent("Формирование значений для ячеек");
            try
            {
                int nn = 0;
                if (Pages != null)
                    foreach (var page in Pages.Values)
                    {
                        foreach (var cell in page)
                            ReadToCell(cell);
                        Procent = 50.0 * ++nn / Pages.Count;
                    }
                if (PagesShape != null)
                    foreach (var page in PagesShape.Values)
                    {
                        foreach (var shape in page.Values)
                            ReadToCell(shape);
                        Procent = 50 + 50.0 * ++nn / PagesShape.Count;
                    }
            }
            catch (Exception ex) { AddError("Ошибка при формировнии значений для ячеек", ex); }
        }

        private void ReadToCell(ReportObject cell)
        {
            if (!cell.LinkField.IsValueField())
                cell.SingleValue = new SingleValue(PropResult(cell));
            else if (cell.LinkType == LinkType.System)
                cell.SingleValue = new SingleValue(PropResult(cell, SystemResult(cell.Param.FullCode)));
            else
            {
                var qv = cell.ArchiveQueryValues;
                var sv = qv == null ? null : qv.SingleValue;
                if (sv == null || (sv.Moment == null && (sv.Moments == null || sv.Moments.Count == 0)))
                    cell.SingleValue = null;
                else
                {
                    if (UsedProjects[cell.Param.Project].CalcMode == CalcModeType.Internal)
                    {
                        if (cell.LinkType == LinkType.Result)
                            cell.SingleValue = sv.HasLastMoment
                                ? new SingleValue(PropResult(cell, sv.LastMoment))
                                : new SingleValue(SingleType.List);
                        else //Мгновенные
                        {
                            cell.SingleValue = new SingleValue(SingleType.List);
                            if (sv.Type == SingleType.Moment)
                                cell.SingleValue.AddMoment(PropResult(cell, sv.Moment, 1));
                            else
                                for (int i = 0; i < sv.Moments.Count; i++)
                                    if (InPart(cell, sv.Moments[i].Time, i))
                                        cell.SingleValue.AddMoment(PropResult(cell, sv.Moments[i], i + 1));
                        }
                    }
                    else //Периодческий расчет
                    {
                        switch (cell.LinkType)
                        {
                            case LinkType.Absolute:
                            case LinkType.AbsoluteEdit:
                                cell.SingleValue = new SingleValue(PropResult(cell, sv.Moment));
                                break;
                            case LinkType.MomentsList:
                                cell.SingleValue = new SingleValue(SingleType.List);
                                for (int i = 0; i < sv.Moments.Count; i++)
                                    if (InPart(cell, sv.Moments[i].Time, i))
                                        cell.SingleValue.AddMoment(PropResult(cell, sv.Moments[i], i + 1));
                                break;
                            case LinkType.Result:
                                cell.SingleValue = sv.HasLastMoment
                                    ? new SingleValue(PropResult(cell, sv.LastMoment))
                                    : new SingleValue(SingleType.List);
                                break;
                            case LinkType.Combined:
                                List<Moment> moms;
                                List<ArchiveInterval> ints;
                                if (InPart(cell, qv.Intervals[0].Begin, 0) &&
                                    InPart(cell, qv.Intervals[qv.Intervals.Count - 1].Begin, qv.Intervals.Count - 1))
                                {
                                    moms = sv.Moments;
                                    ints = qv.Intervals;
                                }
                                else
                                {
                                    moms = new List<Moment>();
                                    ints = new List<ArchiveInterval>();
                                    for (int i = 0; i < sv.Moments.Count; i++)
                                        if (InPart(cell, qv.Intervals[i].Begin, i))
                                        {
                                            ints.Add(qv.Intervals[i]);
                                            moms.Add(sv.Moments[i]);
                                        }
                                }
                                cell.SingleValue = new SingleValue(PropResult(cell, ApplySuperProcess(moms, ints, cell)));
                                break;
                            case LinkType.AbsoluteCombined:
                                cell.SingleValue =
                                    new SingleValue(PropResult(cell, ApplySuperProcess(sv.Moments, qv.Intervals, cell)));
                                break;
                            case LinkType.CombinedList:
                                cell.SingleValue = new SingleValue(SingleType.List);
                                var ts = new TimeSpan(cell.Properties.GetInt("LengthDay"), cell.Properties.GetInt("LengthHour"),
                                    cell.Properties.GetInt("LengthMinute"), 0);
                                var mos = new List<Moment>();
                                var ins = new List<ArchiveInterval>();
                                bool b = false;
                                DateTime ibeg = Different.MinDate;
                                int n = 1;
                                for (int i = 0; i < sv.Moments.Count; i++)
                                {
                                    if (InPart(cell, qv.Intervals[i].Begin, i))
                                    {
                                        if (b && qv.Intervals[i].Begin.AddSeconds(1).Subtract(ibeg) > ts)
                                        {
                                            cell.SingleValue.AddMoment(PropResult(cell, ApplySuperProcess(mos, ins, cell), n++));
                                            ibeg = qv.Intervals[i].Begin;
                                            mos.Clear();
                                            ins.Clear();
                                        }
                                        if (!b)
                                        {
                                            b = true;
                                            ibeg = qv.Intervals[i].Begin;
                                        }
                                        mos.Add(sv.Moments[i]);
                                        ins.Add(qv.Intervals[i]);
                                    }
                                }
                                if (mos.Count > 0)
                                    cell.SingleValue.AddMoment(PropResult(cell, ApplySuperProcess(mos, ins, cell), n));
                                break;
                            case LinkType.AbsoluteList:
                                cell.SingleValue = new SingleValue(SingleType.List);
                                if (sv.Moments != null && sv.Moments.Count > 0)
                                {
                                    ts = new TimeSpan(cell.Properties.GetInt("LengthDay"), cell.Properties.GetInt("LengthHour"),
                                        cell.Properties.GetInt("LengthMinute"), 0);
                                    if (ts.Minutes == 0 && ts.Hours == 0 && Interval.Begin.Hour == 0 &&
                                        Interval.Begin.Minute == 0 && Interval.End.Hour == 0 && Interval.End.Minute == 0)
                                        for (int i = 1; i < sv.Moments.Count; i++)
                                            cell.SingleValue.Moments.Add(PropResult(cell, sv.Moments[i], i));
                                    else
                                    {
                                        mos = new List<Moment>();
                                        ins = new List<ArchiveInterval>();
                                        int k = 0;
                                        while (k < sv.Moments.Count && qv.Intervals[k].End <= Interval.Begin)
                                        {
                                            mos.Add(sv.Moments[k]);
                                            ins.Add(qv.Intervals[k++]);
                                        }
                                        if (ins.Count > 0)
                                        {
                                            Moment ma = ApplySuperProcess(mos, ins, cell);
                                            ma.Time = ins.Last().End;
                                        }

                                        b = false;
                                        ibeg = Different.MinDate;
                                        n = 1;
                                        for (int i = k; i < sv.Moments.Count; i++)
                                        {
                                            if (b && qv.Intervals[i].Begin.AddSeconds(1).Subtract(ibeg) > ts)
                                            {
                                                var ma = ApplySuperProcess(mos, ins, cell);
                                                ma.Time = ins.Last().End;
                                                cell.SingleValue.AddMoment(PropResult(cell, ma, n++));
                                                ibeg = qv.Intervals[i].Begin;
                                            }
                                            if (!b)
                                            {
                                                b = true;
                                                ibeg = qv.Intervals[i].Begin;
                                            }
                                            mos.Add(sv.Moments[i]);
                                            ins.Add(qv.Intervals[i]);
                                        }
                                        if (mos.Count > 0)
                                        {
                                            var ma = ApplySuperProcess(mos, ins, cell);
                                            ma.Time = ins.Last().End;
                                            cell.SingleValue.AddMoment(PropResult(cell, ma, n));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        //Производит накопление по списку интервалов и списку значений на них
        private Moment ApplySuperProcess(List<Moment> moms, List<ArchiveInterval> ints, ReportObject cell)
        {
            if (moms.Count == 0) return null;
            var spt = cell.Param.ArchiveParam.SuperProcess.ToProcess();
            var m = moms[0].Clone();
            double d = ints[0].Length();
            for (int i = 1; i < moms.Count; i++)
            {
                switch (spt)
                {
                    case SuperProcess.LastP:
                        m = moms[i];
                        break;
                    case SuperProcess.MinP:
                        if (m > moms[i]) m = moms[i];
                        m.Nd |= moms[i].Nd;
                        break;
                    case SuperProcess.MaxP:
                        if (m < moms[i]) m = moms[i];
                        m.Nd |= moms[i].Nd;
                        break;
                    case SuperProcess.SummP:
                        if (m.DataType == DataType.Integer)
                            m.Integer += moms[i].Integer;
                        else if (m.DataType == DataType.Real)
                            m.Real += moms[i].Real;
                        else if (m.DataType == DataType.String)
                            m.String = StringSummConnect(m.String, moms[i].String);
                        m.Nd |= moms[i].Nd;
                        break;
                    case SuperProcess.AverageP:
                        m.Real = (m.Real * d + moms[i].Real * ints[i].Length()) / (d + ints[i].Length());
                        d += ints[i].Length();
                        m.Nd |= moms[i].Nd;
                        break;
                }
            }
            return m;
        }

        //Накопление Сумма для двух строк
        private string StringSummConnect(string s1, string s2)
        {
            var set1 = s1.ToPropertyHashSet();
            var set2 = s2.ToPropertyHashSet();
            foreach (var s in set2.Values)
                set1.Add(s);
            return set1.Values.ToPropertyString();
        }

        //Проверяет, входит ли m в часть интервала, заданную в ячейке cell
        private bool InPart(ReportObject cell, DateTime time, int number)
        {
            try
            {
                var props = cell.Properties;
                if (props.GetBool("GetPartTime"))
                {
                    DateTime en = Interval.Begin;
                    DateTime beg = en;
                    if (props.ContainsKey("PartBeginDay"))
                        beg = beg.AddDays(int.Parse(props["PartBeginDay"]));
                    if (props.ContainsKey("PartBeginHour"))
                        beg = beg.AddHours(int.Parse(props["PartBeginHour"]));
                    if (props.ContainsKey("PartBeginMinute"))
                        beg = beg.AddMinutes(int.Parse(props["PartBeginMinute"]));
                    if (props.ContainsKey("PartEndDay"))
                        en = en.AddDays(int.Parse(props["PartEndDay"]));
                    if (props.ContainsKey("PartEndHour"))
                        en = en.AddHours(int.Parse(props["PartEndHour"]));
                    if (props.ContainsKey("PartEndMinute"))
                        en = en.AddMinutes(int.Parse(props["PartEndMinute"]));
                    if (time < beg || time >= en)
                        return false;
                }
                if (props.GetBool("GetPartNumber"))
                {
                    if (props.ContainsKey("PartBeginNumber") && number + 1 < int.Parse(props["PartBeginNumber"]))
                        return false;
                    if (props.ContainsKey("PartEndNumber") && number >= int.Parse(props["PartEndNumber"]))
                        return false;
                }
            }
            catch { }
            return true;
        }

        //Возвращает значение системной ссылки
        //code - код системного параметра
        private Moment SystemResult(string code)
        {
            switch (code)
            {
                case "System_IntervalName":
                    return new Moment(Interval.Name);
                case "System_Now":
                    return new Moment(DateTime.Now);
                case "System_TemplateCode":
                    return new Moment(Code);
                case "System_TemplateName":
                    return new Moment(Name);
                case "System_TimeBegin":
                    return new Moment(Interval.Begin);
                case "System_TimeEnd":
                    return new Moment(Interval.End);
            }
            return new Moment("");
        }

        //Значение свойства для ячейки cell по моменту m
        private Moment PropResult(ReportObject cell, Moment m = null, int number = 0)
        {
            if (cell.LinkField.IsValueField())
                return ValueResult(m, cell, number);
            return FieldResult(cell, m);
        }

        //Возвращает значение свойства значения для записи в ячейку cell
        private Moment ValueResult(Moment m, ReportObject cell, int number = 0)
        {
            switch (cell.LinkField)
            {
                case LinkField.Value:
                    return m ?? new Moment();
                case LinkField.Time:
                    return m == null ? new Moment() : new Moment(m.Time, m.Time);
                case LinkField.Nd:
                    return m == null ? new Moment() : new Moment(m.Time, m.Nd);
                case LinkField.Number:
                    return m == null ? new Moment() : new Moment(m.Time, number);
            }
            return m;
        }

        //Возвращает значение свойства параметра для записи в ячейку cell
        //Если m == null, то просто записывает свойство с временем = начало интервала, иначе записывает с временем m
        private Moment FieldResult(ReportObject cell, Moment m = null)
        {
            var t = m == null ? Interval.Begin : m.Time;
            var ap = cell.Param.ArchiveParam;
            var fp = ap.FirstParam;
            var lp = ap.LastParam ?? fp;
            switch (cell.LinkField)
            {
                case LinkField.Code:
                    return new Moment(t, cell.Param.FullCode);
                case LinkField.CodeParam:
                    return new Moment(t, fp.Code);
                case LinkField.CodeSubParam:
                    return new Moment(t, lp.Code);
                case LinkField.Name:
                    return new Moment(t, fp.Name);
                case LinkField.SubName:
                    return new Moment(t, lp.Name);
                case LinkField.Units:
                    return new Moment(t, ap.Units);
                case LinkField.DataType:
                    return new Moment(t, ap.DataType.ToRussian());
                case LinkField.SuperProcessType:
                    return new Moment(t, ap.SuperProcess.ToRussian());
                case LinkField.Comment:
                    return new Moment(t, fp.Comment);
                case LinkField.SubComment:
                    return new Moment(t, lp.Comment);
                case LinkField.Task:
                    return new Moment(t, fp.Task);
                case LinkField.Project:
                    return new Moment(t, cell.Param.Project);
                case LinkField.CalcParamType:
                    return new Moment(t, fp.CalcParamType.ToRussian());
                case LinkField.Min:
                    return new Moment(t, ap.Min.ToString());
                case LinkField.Max:
                    return new Moment(t, ap.Max.ToString());
                case LinkField.DecPlaces:
                    return new Moment(t, ap.DecPlaces.ToString());
                case LinkField.Tag:
                    return new Moment(t, fp.Tag);
            }
            return null;
        }

        //Нужно ли заполнять указанный лист shеet, fillPages = Все листы или Текущий лист
        public bool IsFillPage(object sheet, string fillPages)
        {
            if (!(sheet is Worksheet)) return false;
            var page = (Worksheet)sheet;
            if (page.Name == "SysPage" || page.Name == "Templates") return false;
            if (fillPages == "Все листы") return true;
            if (!(Workbook.ActiveSheet is Worksheet)) return false;
            return page.Name == ((Worksheet)Workbook.ActiveSheet).Name;
        }

        //Заполняет отчет значениями из Cells, fillPages = Все листы или Текущий лист
        public void FillReport(string fillPages)
        {
            if (FormingBook == null) return;
            try
            {
                //Запись в ячейки
                int k = 0; 
                var sheets = FormingBook.GetSheets();
                foreach (var page in sheets)
                {
                    if (IsFillPage(page, fillPages))
                    {
                        if (Pages.ContainsKey(page.Name) || PagesShape.ContainsKey(page.Name))
                        {
                            var ws = (Worksheet) FormingBook.Worksheets[page.Name];
                            bool ispr = ws.Protection != null && ws.ProtectContents;
                            if (ispr) ws.Unprotect();
                            if (Pages.ContainsKey(page.Name))
                            {
                                ClearCells(page);
                                FillCells(page);    
                            }
                            if (PagesShape.ContainsKey(page.Name))
                                FillShapes(page);
                            if (ispr) ws.Protect(UserInterfaceOnly: true);
                        }
                    }
                    Procent = 100.0*(++k) / sheets.Count;
                }
                SysPage.PutValue("LastFillReport", DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                AddError("Ошибка при заполнении отчета", ex);
            }
        }

        //Очистка всех ячеек
        public void ClearAllCells()
        {
            if (!GeneralRep.CheckOneSheet(false)) return;
            if (MessageReportQuestion("Очистить все ячейки отчета?"))
                foreach (var sheet in Workbook.GetSheets())
                    ClearCells(sheet);
        }

        private void ClearCells(Worksheet page)
        {
            if (!Pages.ContainsKey(page.Name)) return;
            AddEvent("Очистка ячеек листа " + page.Name);
            var list = Pages[page.Name];
            foreach (var cell in list)
            {
                try
                {
                    if (cell.LinkType != LinkType.Save && cell.LinkType != LinkType.HandInput)
                    {
                        var range = (Range)page.Cells[cell.Y, cell.X];
                        if (range.Comment != null)
                        {
                            string com = range.Comment.Text();
                            var dic = com.ToPropertyDictionary();
                            if (com.LastIndexOf(";NumPoints=") == -1 && com.LastIndexOf(";SkipFirstCell=") == -1)
                                range.Value2 = null;
                            else
                            {
                                try
                                {
                                    int mx = dic.GetInt("ValueDistanceX");
                                    int my = dic.GetInt("ValueDistanceY");
                                    int mnum = dic.GetInt("NumPoints");
                                    int skip = dic.GetBool("SkipFirstCell") ? 1 : 0;
                                    for (int j = 0; j < mnum; j++)
                                        ((Range)page.Cells[cell.Y + (j + skip) * my, cell.X + (j + skip) * mx]).Value2 = null;
                                    PutNumPointsToComment(range, null);
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void FillCells(Worksheet page)
        {
            AddEvent("Заполнение ячеек листа " + page.Name);
            var list = Pages[page.Name];
            foreach (var cell in list)
            {
                try
                {
                    if (cell.SingleValue != null)
                    {
                        if (cell.SingleValue.Type == SingleType.Moment)
                        {
                            var range = (Range) page.Cells[cell.Y, cell.X];
                            try { PutNumPointsToComment(range, cell.SingleValue); } catch {}
                            MomToCell(range, cell.SingleValue.Moment);
                        }
                        else
                        {
                            int dx = cell.Properties.GetInt("ValueDistanceX");
                            int dy = cell.Properties.GetInt("ValueDistanceY");
                            int skip = cell.Properties.GetBool("SkipFirstCell") ? 1 : 0;
                            int x = cell.X, y = cell.Y;
                            var moms = cell.SingleValue.Moments;

                            var r = (Range) page.Cells[y, x];
                            PutNumPointsToComment(r, cell.SingleValue);

                            x += dx * skip;
                            y += dy * skip;
                            if (cell.Properties["ValuesOrder"] != "DecTime")
                                for (int i = 0; i < moms.Count; i++)
                                {
                                    var range = (Range) page.Cells[y, x];
                                    if ((i > 0 || skip == 1) && (range.Comment != null && !range.Comment.Text().IsEmpty()))
                                        AddError("Списки значений, сформированных по некоторым ссылкам, перекрывают другие ссылки");
                                    MomToCell(range, cell.SingleValue.Moments[i]);
                                    x += dx;
                                    y += dy;
                                }
                            else
                                for (int i = moms.Count - 1; i >= 0; i--)
                                {
                                    var range = (Range) page.Cells[y, x];
                                    MomToCell(range, cell.SingleValue.Moments[i]);
                                    x += dx;
                                    y += dy;
                                }
                        }
                    }
                }
                catch { }
            }
        }

        private void FillShapes(Worksheet page)
        {
            AddEvent("Заполнение фигур листа " + page.Name);
            var list = PagesShape[page.Name];
            foreach (Shape shape in page.Shapes)
            {
                try
                {
                    if (list.ContainsKey(shape.ID))
                    {
                        var rshape = list[shape.ID];
                        if (shape.Type == MsoShapeType.msoTextBox)
                            shape.TextFrame.Characters().Text = rshape.SingleValue != null && rshape.SingleValue.LastMoment != null
                                ? rshape.SingleValue.LastMoment.String : null;
                        else if (shape.Type == MsoShapeType.msoGroup)
                        {
                            foreach (Shape gshape in shape.GroupItems)
                            {
                                try
                                {
                                    if (gshape.Type == MsoShapeType.msoTextBox)
                                    {
                                        var rgshape = list[gshape.ID];
                                        gshape.TextFrame.Characters().Text = rgshape.SingleValue != null && rgshape.SingleValue.LastMoment != null
                                            ? rgshape.SingleValue.LastMoment.String : null;
                                    }
                                }
                                catch { }
                            }
                            rshape.Node.Apply(rshape, rshape.SingleValue.LastMoment.Integer);
                        }    
                    }
                } 
                catch { }
            }
        }

        //Добавление в комментарий количества значений
        private static void PutNumPointsToComment(Range r, SingleValue sv)
        {
            int n = 0;
            if (sv != null)
            {
                if (sv.Moments != null) n = sv.Moments.Count;
                else if (sv.Moment != null) n = 1;    
            }
            string com = r.Comment.Text();
            int pos = com.LastIndexOf("NumPoints=");
            if (pos != -1) com = com.Substring(0, pos);
            r.ClearComments();
            r.AddComment(com + (n <= 1 ? "" : ("NumPoints=" + n)));
        }

        //Записывает значение 
        private void MomToCell(Range cell, Moment m)
        {
            switch (m.DataType)
            {
                case DataType.Boolean:
                    cell.Value2 = m.Boolean ? 1 : 0;
                    return;
                case DataType.Integer:
                    cell.Value2 = m.Integer;
                    return;
                case DataType.Real:
                    cell.Value2 = m.Real;
                    return;
                case DataType.Time:
                    cell.Value2 = m.Date;
                    return;
                default:
                    cell.Value2 = m.String;
                    return;
            }
        }

        //Подготовка архивного проекта ведомости и запись его параметров в архив
        private void PrepareArchiveProject()
        {
            if (_archiveProject == null)
            {
                AddEvent("Подготовка архивного проекта ведомости");
                try
                {
                    _archiveProject = new ArchiveProject(CodeAsProject, Name, ReportType.Excel, LastChangeLinks);
                    foreach (var page in Pages.Values)
                        foreach (var cell in page)
                        {
                            var par = cell.Param.ArchiveParam;
                            var ap = new ArchiveParam(cell.SaveCode, cell.DataType, cell.Units, par.FirstParam, par.LastParam);
                            _archiveProject.AddParam(ap);
                        }
                    using (Start(30)) 
                        Archive.PrepareProject(_archiveProject);
                }
                catch (Exception ex)
                {
                    AddError("Ошибка подготовки архивного проекта", ex);
                }
            }
        }
       
        //Возвращает, можно ли сохранять отчет в журнал в данный момент и возвращает сообщение, если нельзя
        private bool CanSaveReport()
        {
            if (FormInf == null || FormToDir || FormToFile)
            {
                MessageReportError("Сохранение в журнал возможно только при формировании отчета непосредственно в бланк отчета (задается в настройках)");
                return false;
            }
            if (Interval == null)
            {
                MessageReportError("Для сохранения в журнал отчет нужно сначала сформировать");
                return false;
            }
            return true;
        }

         //Сохраняет отчет в журнал, name - имя интервала
        public void SaveReport()
        {
            using (Start(0, 5))
                if (!PrepareArchive() || Archive == null)
                {
                    MessageReportError("Не удается соединиться с архивом журнала отчетов. Проверьте настройки.");
                    return;
                }
            AddEvent("Подготовка проекта для записи в журнал");
            using (Start(5, 25))
                PrepareArchiveProject();
            try
            {
                _archiveProject.IntervalsForWrite.Clear();
                _archiveProject.IntervalsForWrite.Add(Interval);
                foreach (var sheet in Workbook.Sheets)
                    if (sheet is Worksheet && Pages.ContainsKey(((Worksheet)sheet).Name))
                    {
                        var page = Pages[((Worksheet)sheet).Name];
                        foreach (var cell in page)
                            if (_archiveProject.Params.ContainsKey(cell.SaveCode))
                            {
                                var par = _archiveProject.Params[cell.SaveCode];
                                par.Intervals.Clear();
                                if (cell.AllowEdit)
                                {
                                    string s = ValueFromCell(cell);
                                    if (s != null)
                                    {
                                        var m = new Moment(par.DataType, s, Interval.Begin);
                                        var sv = new SingleValue(m);
                                        par.Intervals.Add(Interval, sv);
                                    }
                                }
                                else par.Intervals.Add(Interval, cell.SingleValue);
                            }
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке архивного проекта", ex);
            }
            AddEvent("Запись проекта в журнал");
            using(Start(40))
                Archive.WriteProject(_archiveProject, Interval.Begin, Interval.End);
        }

        //Читает отчет из журнала
        public void ReadReportProject()
        {
            try
            {
                if (Archive == null)
                {
                    Different.MessageError("Не удается соединиться с архивом журнала отчетов. Проверьте настройки.");
                    return;
                }
                using (Start(0, 25))
                    PrepareArchiveProject();
                _archiveProject.IntervalsForRead.Clear();
                _archiveProject.IntervalsForRead.Add(Interval);
                AddEvent("Чтение значений из журнала");
                using (Start(25, 80))
                    Archive.ReadProject(_archiveProject);
                AddEvent("Подготовка значений ячеек");
                foreach (var page in Pages.Values)
                    foreach (var cell in page)
                        if (_archiveProject.Params.ContainsKey(cell.SaveCode))
                        {
                            var par = _archiveProject.Params[cell.SaveCode];
                            cell.SingleValue = par.Intervals.Count > 0 ? par.Intervals.Values.First() : null;
                        }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении отчета из журнала", ex);
            }
        }

        //Проверка допустимости периода, возвращает интервал, если все прошло успешно, иначе null
        public TimeInterval CheckPeriodTime(string beg, string en)
        {
            //Проверка времени заполнения отчета
            DateTime begin, end;
            if (!DateTime.TryParse(beg, out begin) || !DateTime.TryParse(en, out end))
            {
                Different.MessageError("Недопустимое время начала или конца периода");
                return null;
            }
            
            string s = null;
            var sec = end.AddMonths(-MonthLengh).Subtract(PeriodLength).Subtract(begin).TotalSeconds;
            if (DifferentLength == DifferentLength.Equals && sec != 0)
                s = @"Недопустимая длительность периода для данного отчета, длительность должна быть равна " + (MonthLengh == 0 ? "" : (MonthLengh + " месяцев ")) + PeriodLength;
            if (DifferentLength == DifferentLength.Less && sec > 0)
                s = @"Недопустимая длительность периода для данного отчета, длительность не должна превосходить " + (MonthLengh == 0 ? "" : (MonthLengh + " месяцев ")) + PeriodLength;

            if (end.Subtract(begin).TotalSeconds < 0)
                s = @"Введенный конец периода раньше, чем его начало";

            if (!DifferentBegin && !PeriodNull)
            {
                DateTime d = begin;
                if (MonthLengh > 0 && d.AddMonths(-MonthLengh).Day != DayStart)
                    s = @"Недопустимое время начала периода для данного отчета";
                else if (PeriodLength >= new TimeSpan(1, 0, 0, 0) || MonthLengh > 0)
                {
                    if (d.Subtract(d.Date) != PeriodStart)
                        s = @"Недопустимое время начала периода для данного отчета";
                }
                else
                {
                    TimeSpan t = PeriodLength;
                    while (t < new TimeSpan(2, 0, 0, 0) && d.Subtract(d.Date).Subtract(PeriodStart).TotalSeconds != 0)
                    {
                        d = d.Subtract(PeriodLength);
                        t = t.Add(PeriodLength);
                    }
                    if (t >= new TimeSpan(2, 0, 0, 0))
                        s = @"Недопустимое время начала периода для данного отчета";
                }
            }
            if (s != null)
            {
                Different.MessageError(s);
                return null;
            }
            var dlen = Convert.ToInt32(end.Subtract(begin).TotalDays);
            if ((dlen > 5 && Controller != null) || (dlen > 50 && Controller == null)) 
                if (!Different.MessageQuestion("Задан очень длинный интервал формирования отчета (" + dlen + " суток). Вы уверены, что хотите сформировать отчет за этот интервал?"))
                    return null;
            return new TimeInterval(begin, end);
        }

        //Формирование отчета, для форм динамического отчета и видеомагнитофона
        public void FormDynamicReport(DateTime periodBegin, DateTime periodEnd, string fillPages)
        {
            using (StartAtom("Формирование отчета", true, periodBegin + " - " + periodEnd))
            {
                try
                {
                    bool hasCalc = Controller != null;
                    if (hasCalc)
                    {
                        AddEvent("Расчет", periodBegin + " - " + periodEnd);
                        var con = Controller;
                        con.SetCalcOperations(true, true, true);
                        con.Calc(periodBegin, periodEnd);
                        if (!con.ErrorMessage.IsEmpty())
                            AddError("Ошибки при расчете:\n", null, con.ErrorMessage);
                        Procent = 50;
                    }
                    //Получение данных из архива
                    using (Start(hasCalc ? 50 : 0, hasCalc ? 60 : 40))
                        ReadArchiveReport();

                    //Заполнение отчета
                    using (Start(hasCalc ? 60 : 40, 80))
                        FillReport(fillPages);

                    try { Workbook.Save(); }
                    catch { }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка заполнения отчета", ex);
                }
                IsReportForming = false;
                ShowError();
            }
        }

    }
}
