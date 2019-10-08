using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using Microsoft.Office.Interop.Excel;
using ReporterCommon.FormsLinks;

namespace ReporterCommon
{
    //Один отчет вместе со всеми открытыми формами, потоком и архивом
    //Чтение общих настроек и работа с формами
    public partial class ReportBook : Logger
    {
        //Если workbook=null, значит создается CommonBook, иначе ActiveBook
        public ReportBook(string code, Workbook workbook)
        {
            Code = code;
            ThreadName = Code;
            Workbook = workbook;
            if (workbook != null)
            {
                SysPage = new SysPage(Workbook);
                UpdateReportVersion();
            }
        }

        //Ссылка на книгу
        public Workbook Workbook { get; private set; }
        //SysPage файла отчета
        public SysPage SysPage { get; private set; }
        //Словарь всех открытых форм
        private readonly Dictionary<ReporterCommand, Form> _forms = new Dictionary<ReporterCommand, Form>();
        public Dictionary<ReporterCommand, Form> Forms { get { return _forms; } }

        //Путь к файлу данных отчета
        public string DataFile { get; private set; }
        
        //Код отчета
        public string Code { get; private set; }
        //Код проекта отчета
        public string CodeAsProject { get { return Code + "_Report"; } }

        //Параметры, взятые c SysPage
        public string Name { get; private set; }
        public TimeSpan PeriodLength { get; private set; }
        public int MonthLengh { get; private set; }
        public TimeSpan PeriodStart { get; private set; }
        public int DayStart { get; private set; }
        public bool PeriodNull { get; private set; } //Если период не заполнен
        public string DefaultPeriod { get; private set; }
        public DifferentLength DifferentLength { get; private set; }
        public bool DifferentBegin { get; private set; }

        //Время обновления ссылок (передается строкой)
        public DateTime LastChangeLinks
        {
            get { return SysPage.GetValue("LastChangeLinks").ToDateTime(); }
            set { SysPage.PutValue("LastChangeLinks", value.ToString()); }
        }

        //Переопределение комманд логгера
        public Command StartAtom(string name, bool isCycle = false, string pars = null)
        {
            if (CommandLog != null && !CommandLog.IsFinished) return CommandLog;
            return StartLog(name, pars, Code, Code, isCycle);
        }
        public Command StartAtom(ReporterCommand form)
        {
            if (form.ToStringHistory() == null) return Start();
            return StartLog(form.ToStringHistory(), "", Code, Code, true);
        }

        //Выводит сообщение c ошибкой текущей комманды
        public void ShowError()
        {
            if (Command != null && Command.IsError)
                Different.MessageError(Command.ErrorMessage());
        }

        //Обновлении версии файла отчета
        private void UpdateReportVersion()
        {
            var curver = new VersionSynch.Version(SysPage.GetValue("Version"), SysPage.GetValue("VersionDate"));
            UpdateVersion(curver, "1.2.0", "07.02.2014", UpdateReport_1_2_0);
            UpdateVersion(curver, "1.2.1", "01.04.2014", UpdateReport_1_2_1);
            UpdateVersion(curver, "1.2.2", "10.04.2014", UpdateReport_1_2_2);
            UpdateVersion(curver, "1.3.0", "04.06.2014", UpdateReport_1_3_0);
            UpdateVersion(curver, "1.3.1", "01.02.2016", UpdateReport_1_3_1);
            UpdateVersion(curver, "1.3.2", "22.03.2016", UpdateReport_1_3_2);
            UpdateVersion(curver, "1.3.3", "02.11.2016", UpdateReport_1_3_3);
            UpdateVersion(curver, "1.3.4", "22.09.2017", UpdateReport_1_3_4);
            UpdateVersion(curver, "1.3.5", "20.10.2017", UpdateReport_1_3_5);
        }

        //Обновление на одну версию
        private void UpdateVersion(VersionSynch.Version curver, string version, string versionDate, System.Action action)
        {
            var newver = new VersionSynch.Version(version, versionDate);
            if (curver < newver)
            {
                action();
                SysPage.PutValue("Version", newver.ToString());
                SysPage.PutValue("VersionDate", newver.Date);
            }
        }

        //Список обновлений
        private void UpdateReport_1_2_0()
        {
            SysPage.AddProperty("AllowProjectsRepetitions", "Разрешить повтор кодов проектов", "False");
            SysPage.SysSheet.PutCellValue(1, 5, "");
        }

        private void UpdateReport_1_2_1()
        {
            bool e = false;
            foreach (var sheet in Workbook.GetSheets(false))
                e |= sheet.Name == "Templates";
            if (!e)
            {
                var w = (Worksheet)Workbook.Sheets.Add();
                w.Name = "Templates";
                w.Visible = XlSheetVisibility.xlSheetHidden;    
            }
            SysPage.AddProperty("LastSaveParamId", "Номер последней добавленной ячейки для сохранения", "0");
            SysPage.AddProperty("CurCellComment", "Текущее примечание к ячейке");
            SysPage.AddProperty("CurNextCellShift", "Текущее направление сдвига после установки ссылки", "Вправо");
            SysPage.AddProperty("CurNextCellStep", "Текущая величина сдвига после установки ссылки", "1");
        }

        private void UpdateReport_1_2_2()
        {
            //Добавлены параметры для свойство типов ссылок
            SysPage.AddProperty("Итоговое значение", "Настройки для разных типов ссылок", "LinkType=Result;CellComment=;AllowEdit=False;");
            SysPage.AddProperty("Абсолютное значение", null, "LinkType=Absolute;CellComment=;AllowEdit=False;");
            SysPage.AddProperty("Абсолютное c редактированием", null, "LinkType=AbsoluteEdit;CellComment=;AllowEdit=False;");
            SysPage.AddProperty("Комбинированное значение", null, "LinkType=Combined;CellComment=;AllowEdit=False;");
            SysPage.AddProperty("Равномерный список значений", null, "LinkType=CombinedList;CellComment=;ValueDistanceY=1;ValuesOrder=IncTime;LengthHour=1;");
            SysPage.AddProperty("Список мгновенных значений", null, "LinkType=MomentsList;CellComment=;ValueDistanceY=1;ValuesOrder=IncTime;");
            SysPage.AddProperty("Ручной ввод", null, "LinkType=HandInput;CellComment=;AllowEdit=True;");
            SysPage.AddProperty("Сохранение в архив", null, "LinkType=Save;CellComment=;AllowEdit=True;");
            SysPage.AddProperty("Системная ссылка", null, "LinkType=System;CellComment=;");
            //Добавлены параметры для условий фильтров списка параметров и отношений по ним
            SysPage.AddProperty("FilterFullCode", "Условия фильтров списка параметров и отношения для них");
            SysPage.AddProperty("RelationFullCode", null, "Равно");
            SysPage.AddProperty("FilterCode");
            SysPage.AddProperty("RelationCode", null, "Равно");
            SysPage.AddProperty("FilterSubCode");
            SysPage.AddProperty("RelationSubCode", null, "Равно");
            SysPage.AddProperty("FilterName");
            SysPage.AddProperty("RelationName", null, "Равно");
            SysPage.AddProperty("FilterComment");
            SysPage.AddProperty("RelationComment", null, "Равно");
            SysPage.AddProperty("FilterTask");
            SysPage.AddProperty("RelationTask", null, "Равно");
            SysPage.AddProperty("FilterUnits");
            SysPage.AddProperty("RelationUnits", null, "Равно");
            SysPage.AddProperty("FilterDataType");
            SysPage.AddProperty("RelationDataType", null, "Равно");
            SysPage.AddProperty("FilterSuperProcess");
            SysPage.AddProperty("RelationSuperProcess", null, "Равно");
            SysPage.AddProperty("FilterCalcParamType");
            SysPage.AddProperty("RelationCalcParamType", null, "Равно");
        }

        private void UpdateReport_1_3_0()
        {
            SysPage.AddProperty("FilterOtm");
            SysPage.AddProperty("RelationOtm", null, "Равно");
            //Добавлены параметры для условий фильтров списка интервалов и отношений по ним
            SysPage.AddProperty("FilterOtmIntervals", "Условия фильтров списка интервалов и отношения для них");
            SysPage.AddProperty("RelationOtmIntervals", null, "Равно");
            SysPage.AddProperty("FilterNameIntervals");
            SysPage.AddProperty("RelationNameIntervals", null, "Равно");
            SysPage.AddProperty("FilterBegin1Intervals");
            SysPage.AddProperty("RelationBegin1Intervals", null, "Равно");
            SysPage.AddProperty("FilterBegin2Intervals");
            SysPage.AddProperty("RelationBegin2Intervals", null, "Равно");
            SysPage.AddProperty("FilterEnd1Intervals");
            SysPage.AddProperty("RelationEnd1Intervals", null, "Равно");
            SysPage.AddProperty("FilterEnd2Intervals");
            SysPage.AddProperty("RelationEnd2Intervals", null, "Равно");
            SysPage.AddProperty("FilterTimeChange1Intervals");
            SysPage.AddProperty("RelationTimeChange1Intervals", null, "Равно");
            SysPage.AddProperty("FilterTimeChange2Intervals");
            SysPage.AddProperty("RelationTimeChange2Intervals", null, "Равно");
        }

        private void UpdateReport_1_3_1()
        {
            SysPage.AddProperty("Абсолютное комбинированное", null, "LinkType=AbsoluteCombined;CellComment=;AllowEdit=False;");
            SysPage.AddProperty("Список абсолютных значений", null, "LinkType=AbsoluteList;CellComment=;ValueDistanceY=1;ValuesOrder=IncTime;LengthHour=1;");
        }

        private void UpdateReport_1_3_2()
        {
            SysPage.AddProperty("FillPages", "Какие листы заполнять (Все листы, Текущий лист)", "Все листы");
        }

        private void UpdateReport_1_3_3()
        {
            SysPage.AddProperty("MonthLength", "Длительность интервала по умолчанию, количество месяцев");
            SysPage.AddProperty("DayStart", "Начало интервала по умолчанию, сутки");
        }

        private void UpdateReport_1_3_4()
        {
            SysPage.AddProperty("LiveReportPeriodLength", "Длина периода динамического отчета (мин)", "15");
            SysPage.AddProperty("LiveReportSourcesLate", "Задержка источников для динамического отчета (мин)", "0");
            SysPage.AddProperty("LiveReportFillPages", "Какие листы заполнять в динамическом отчете (Все листы, Текущий лист)", "Все листы");
        }

        private void UpdateReport_1_3_5()
        {
            SysPage.AddProperty("VideoBegin", "Начало просмотра в видеомагнитофоне");
            SysPage.AddProperty("VideoEnd", "Конец просмотра в видеомагнитофоне");
            SysPage.AddProperty("VideoPeriodLength", "Длина периода в видеомагнитофоне (мин)", "15");
            SysPage.AddProperty("VideoBetweenPeriods", "Ожидание следущего периода в видеомагнитофоне (сек)", "5");
            SysPage.AddProperty("VideoFillPages", "Какие листы заполнять в видеомагнитофоне (Все листы, Текущий лист)", "Все листы");
        }
        
        //Загрузка настроек из SysPage и создание файла данных, если нужно
        public void LoadSetup()
        {
            AddEvent("Загрузка свойств отчета и открытие файла данных");
            string s = Workbook.Name;
            DataFile = General.ReporterDir + @"Tmp\" + s.Substring(0, s.Length - 5) + "Data.accdb";
            DaoDb.FromTemplate(General.ReportTemplateFile, DataFile);
            Name = SysPage.GetValue("ReportName");
            PeriodLength = new TimeSpan(SysPage.GetIntValue("DayLength"), SysPage.GetIntValue("HourLength"), SysPage.GetIntValue("MinuteLength"), 0);
            MonthLengh = SysPage.GetIntValue("MonthLength");
            DefaultPeriod = SysPage.GetValue("DefaultPeriod");
            PeriodNull = SysPage.GetValue("HourStart").IsEmpty() && SysPage.GetValue("MinuteStart").IsEmpty();
            PeriodStart = new TimeSpan(0, SysPage.GetIntValue("HourStart"), SysPage.GetIntValue("MinuteStart"), 0);
            DayStart = SysPage.GetIntValue("DayStart");
            DifferentLength = SysPage.GetValue("DifferentLength").ToDifferentLength();
            DifferentBegin = SysPage.GetBoolValue("DifferentBegin");
        }

        //Открывает новую заданную форму
        public Form RunCommandReporter(ReporterCommand c)
        {
            if (Forms.ContainsKey(c) && Forms[c] != null)
            {
                var fm = Forms[c];
                if (!fm.Visible) fm.Show();
                if (fm.WindowState == FormWindowState.Minimized)
                    fm.WindowState = FormWindowState.Normal;
                fm.Focus();
                return fm;
            }
            if (!c.OneForAllBooks() && IsReportForming)
                return null;
            if (c.NeedCheckReport() && !SysPage.IsInfoTask())
            {
                MessageReportError("Да запуска команды необходимо открыть отчет InfoTask");
                return null;
            }
            
            Form f = null;
            using (StartAtom(c))
            {
                try
                {
                    AddEvent("Закрытие лишних форм");
                    var less = new Dictionary<ReporterCommand, Form>();
                    var great = new Dictionary<ReporterCommand, Form>();
                    foreach (var k in Forms)
                    {
                        if (c.Greater(k.Key)) less.Add(k.Key, k.Value);
                        if (c.Less(k.Key)) great.Add(k.Key, k.Value);
                    }
                    var cForms = GeneralRep.CommonBook.Forms;
                    foreach (var k in cForms)
                        if (c.Less(k.Key)) great.Add(k.Key, k.Value);

                    if (great.Count > 0)
                    {
                        string smess = "";
                        foreach (var g in great)
                            smess += (smess != "" ? "," : "") + " Форма " + g.Key.ToFormName();
                        smess = "Команда не может быть выполнена, пока открыты:" + smess + ". Закрыть формы?";
                        if (!MessageReportQuestion(smess)) return null;    
                    }
                    
                    foreach (var k in less)
                    {
                        k.Value.Close();
                        if (Forms.ContainsKey(k.Key)) Forms.Remove(k.Key);
                        if (cForms.ContainsKey(k.Key)) cForms.Remove(k.Key);
                    }
                    foreach (var k in great)
                        GeneralRep.CloseForm(k.Key, true);

                    if (c == ReporterCommand.PutLinks || c == ReporterCommand.ShapeLibrary || c == ReporterCommand.Report || c == ReporterCommand.LiveReport || c == ReporterCommand.Intervals || c == ReporterCommand.AbsoluteEdit || c == ReporterCommand.LinksList)
                        UpdateDataFile(false);

                    System.Windows.Forms.Application.EnableVisualStyles();
                    switch (c)
                    {
                        case ReporterCommand.GroupReports:
                            f = new FormGroupReports();
                            break;
                        case ReporterCommand.GroupReportEdit:
                            f = new FormGroupReportEdit();
                            break;
                        case ReporterCommand.Create:
                            f = new FormCreate();
                            break;

                        case ReporterCommand.Setup:
                            f = new FormSetup();
                            break;
                        case ReporterCommand.CopyServerReport:
                            CopyServerReport();
                            break;

                        case ReporterCommand.PutLinks:
                            using (Start()) LoadCurTemplate();
                            LastChangeLinks = DateTime.Now;
                            f = new FormPutLinks();
                            break;
                        case ReporterCommand.FilterParams:
                            f = new FormFiltersParams();
                            break;
                        case ReporterCommand.LinkProperties:
                            f = new FormLinkProperties();
                            break;
                        case ReporterCommand.FindLinks:
                            f = new FormFindLinks();
                            break;
                        case ReporterCommand.LinksTemplate:
                            f = new FormLinksTemplate();
                            break;
                        case ReporterCommand.DeleteLinks:
                            DeleteLinks();
                            LastChangeLinks = DateTime.Now;
                            break;
                        case ReporterCommand.ClearCells:
                            Forms.Add(ReporterCommand.ClearCells, null);
                            UsedFileToMemory();
                            ClearAllCells();
                            Forms.Remove(ReporterCommand.ClearCells);
                            break;
                        case ReporterCommand.ShapeLibrary:
                            f = new FormShapeLibrary();
                            break;

                        case ReporterCommand.Report:
                            UsedFileToMemory();
                            PrepareController();
                            f = new FormReport();
                            break;
                        case ReporterCommand.LiveReport:
                            UsedFileToMemory();
                            PrepareController();
                            f = new FormLiveReport();
                            break;
                        case ReporterCommand.Video:
                            UsedFileToMemory();
                            PrepareController();
                            f = new FormVideo();
                            break;
                        case ReporterCommand.ArchivesRanges:
                            f = new FormArchivesRanges();
                            break;
                        case ReporterCommand.Intervals:
                            UsedFileToMemory();
                            if (FormInf == null || FormToDir || FormToFile)
                                MessageReportError("Сохранение в журнал возможно только при формировании отчета непосредственно в бланк отчета (задается в настройках)");
                            else
                            {
                                PrepareArchive();
                                f = new FormIntervals();
                            }
                            break;
                        case ReporterCommand.FilterIntervals:
                            f = new FormFiltersIntervals();
                            break;
                        case ReporterCommand.SaveReport:
                            if (CanSaveReport())
                                f = new FormSaveReport();
                            break;
                        case ReporterCommand.AbsoluteEdit:
                            UsedFileToMemory();
                            AddEvent("Чтение абсолютных значений из архива");
                            foreach (var project in UsedProjects.Values)
                                project.AbsoluteEditValues = project.Archive.ReadAbsoluteEdit(project.Code, false);
                            f = new FormAbsoluteEdit();
                            break;
                        
                        case ReporterCommand.Update:
                            UpdateDataFile(true);
                            if (!Command.IsError) MessageReport("Ссылки были обновлены");
                            break;
                        case ReporterCommand.LinksList:
                            f = new FormLinksList();
                            break;
                        case ReporterCommand.AppInfo:
                            f = new FormAppInfo();
                            break;
                    }
                    if (f != null)
                    {
                        if (!c.OneForAllBooks()) f.Text += " (" + Code + ")";
                        Forms.Add(c, f);
                        f.Show();
                    }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при открытии формы", ex, c.ToString());
                }
                if (Command != null && Command.IsError)
                {
                    foreach (var ff in Forms)
                        if (ff.Key != ReporterCommand.LinkProperties && ff.Value != null && ff.Value.Visible)
                        {
                            ff.Value.Activate();
                            break;
                        }
                    ShowError();
                }
            }
            return f;
        }

        //Вызывается при закрытии заданной формы form, close - закрывать форму прямо здесь в процедуре
        public void CloseForm(ReporterCommand form, bool close = false)
        {
            if (Forms.ContainsKey(form))
            {
                var fform = Forms[form];
                Forms.Remove(form);
                var list = (from f in Forms where f.Key.IsChildOf(form) select f.Key).ToList();
                foreach (var ff in list)
                    CloseForm(ff, true);
                using (StartAtom("Закрытие формы " + form))
                {
                    try
                    {
                        if (close) fform.Close();
                        switch (form)
                        {
                            case ReporterCommand.PutLinks:
                                UpdateDataFile(false, true);
                                Workbook.Save();
                                break;
                            case ReporterCommand.Setup:
                                SysPage.PutValue("LastSetup", DateTime.Now.ToString());
                                Workbook.Save();
                                UpdateDataFile(true);
                                LoadSetup();
                                break;
                            case ReporterCommand.AbsoluteEdit:
                                AddEvent("Запись абсолютных значений в архив");
                                foreach (var project in UsedProjects.Values)
                                    project.Archive.WriteAbsoluteEdit(project.AbsoluteEditValues.Values.Where(hip => !hip.Value.IsEmpty()).ToList());
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        AddError("Ошибка при закрытии формы", ex, form.ToString());
                    }
                }    
            }
        }
        
        //Вызывается при закрытии книги
        public void CloseBook()
        {
            ClearReportDataFromMemory();
            UpdateHistory(false);
            CloseHistory();
            //Не получается закрыть все формы культурно, ну и ладно, данные файла отчета обновятся при следующем запуске любой из форм
        }

        //Копирует контрольный бланк отчета в текущий
        public void CopyServerReport()
        {
            if (GeneralRep.IsUpdateFromReportServer) return;
            using (StartAtom(ReporterCommand.CopyServerReport))
            {
                var code = SysPage.GetValue("Report");
                string serverReport;
                using (var rec = new RecDao(General.ReporterFile, "SELECT ServerReport FROM Reports WHERE Report='" + code + "'"))
                {
                    if (!rec.HasRows() || rec.GetString("ServerReport").IsEmpty())
                    {
                        AddError("Файл контрольного экземпляра не задан");
                        return;
                    }
                    serverReport = rec.GetString("ServerReport");
                }
                var file = new FileInfo(serverReport);
                if (!file.Exists || file.Extension != ".xlsx")
                {
                    AddError("Указанный файл контрольного экземпляра не существует или не является файлом Excel, " + serverReport);
                    return;
                }
                if (serverReport == Workbook.FullName)
                {
                    AddError("Файл контрольного бланка расчета не должен совпадать с файлом бланка отчета, " + serverReport);
                    return;
                }
                try { Workbook.Save(); } catch {}
                try
                {
                    GeneralRep.IsUpdateFromReportServer = true;
                    string tbook = General.TmpDir + "TmpServerReport.xlsx";
                    file.CopyTo(tbook, true);
                    var twbook = GeneralRep.Application.Workbooks.Open(tbook);
                    if (!SysPage.IsInfoTask(twbook))
                    {
                        AddError("Указанный файл контрольного экземпляра не является файлом отчета, " + serverReport);
                    }
                    else if (MessageReportQuestion("Обновить данный бланк отчета из контрольного бланка отчета?"))
                    {
                        _transactions.Clear();
                        _afterTransCells.Clear();
                        CurTransactionNum = 0;
                        string fbook = Workbook.FullName;
                        Workbook.Close();
                        var tfile = new FileInfo(tbook);
                        tfile.CopyTo(fbook, true);
                        Workbook = GeneralRep.Application.Workbooks.Open(fbook);
                        SysPage = new SysPage(Workbook);
                    }
                    twbook.Close();
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при копировании контрольного бланка отчета, " + serverReport, ex);
                }
                GeneralRep.IsUpdateFromReportServer = false;
            }
        }

        //Выводит сообщение, чтобы он было поверх форм
        public void MessageReport(string mess)
        {
            if (Forms.Count > 0) Forms.Values.First().Activate();
            MessageBox.Show(mess, "InfoTask");
        }
        public void MessageReportError(string mess, Exception ex = null)
        {
            if (Forms.Count > 0) Forms.Values.First().Activate();
            if (ex != null) ex.MessageError(mess);
            else Different.MessageError(mess);
        }
        public bool MessageReportQuestion(string mess)
        {
            foreach (var form in Forms.Values)
                if (form != null)
                {
                    form.Activate();
                    break;
                }
            return Different.MessageQuestion(mess);
        }

        //Реализация методов из Logger

        protected override void FinishLogCommand() { }
        protected override void FinishSubLogCommand() { }
        protected override void FinishProgressCommand()
        {
            ViewProcent(0);
        }

        protected override void MessageError(ErrorCommand er) {}

        private delegate void StatusDelegate();

        protected override void ViewProcent(double procent)
        {
            Form f = null;
            ProgressBar ind = null;
            if (Forms.ContainsKey(ReporterCommand.Report))
            {
                f = Forms[ReporterCommand.Report];
                ind = ((FormReport)f).progressReport;
            }
            if (Forms.ContainsKey(ReporterCommand.Intervals))
            {
                f = Forms[ReporterCommand.Intervals];
                ind = ((FormIntervals)f).progressReport;
            }
            if (f != null)
            {
                ind.Invoke(new StatusDelegate(() =>
                {
                    ind.Value = Convert.ToInt32(Convert.ToInt32(procent));
                    ind.Refresh();
                    f.Refresh();
                }));
            }
        }
    }
}