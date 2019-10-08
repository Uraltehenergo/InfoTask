using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace ReporterCommon
{
    public partial class FormSetup : Form
    {
        public FormSetup()
        {
            InitializeComponent();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Текущая книга
        private ReportBook _book;
        //Словари кодов и настроек провайдеров
        private readonly DicS<string> _codes = new DicS<string>();
        private readonly DicS<string> _infs = new DicS<string>();
        //Идет загрузка настроек
        private bool _setupIsLoaded;

        private void butOK_Click(object sender, EventArgs e)
        {
            using (_book.StartAtom("Сохранение настроек"))
            {
                _book.AddEvent("Сохранение настроек отчета");
                if (CodeReport.Text.IsEmpty())
                {
                    Different.MessageError("Код отчета должен быть заполнен");
                    return;
                }
                int res;
                bool er = (!MonthLength.Text.IsEmpty() && (!int.TryParse(MonthLength.Text, out res) || res < 0));
                er |= (!DayLength.Text.IsEmpty() && (!int.TryParse(DayLength.Text, out res) || res < 0));
                er |= (!HourLength.Text.IsEmpty() && (!int.TryParse(HourLength.Text, out res) || res < 0));
                er |= (!MinuteLength.Text.IsEmpty() && (!int.TryParse(MinuteLength.Text, out res) || res < 0));
                er |= (!DayStart.Text.IsEmpty() && (!int.TryParse(DayStart.Text, out res) || res < 0));
                er |= (!HourStart.Text.IsEmpty() && (!int.TryParse(HourStart.Text, out res) || res < 0 ));
                er |= (!MinuteLength.Text.IsEmpty() && (!int.TryParse(MinuteLength.Text, out res) || res < 0));
                if (er)
                {
                    Different.MessageError("Указана недопустимая длительность или начало интеравала");
                    return;
                }

                var sys = _book.SysPage;
                bool isInternal = false;
                try 
                {
                    sys.PutValue("Report", CodeReport.Text);
                    sys.PutValue("ReportName", NameReport.Text);
                    sys.PutValue("ReportDescription", DescriptionReport.Text);
                    sys.PutControl(MonthLength);
                    sys.PutControl(DayLength);
                    sys.PutControl(HourLength);
                    sys.PutControl(MinuteLength);
                    sys.PutControl(DayStart);
                    sys.PutControl(HourStart);
                    sys.PutControl(MinuteStart);
                    sys.PutValue("DifferentLength", radioDifferent.Checked ? "True" : (radioLess.Checked ? "Less" : "Equals"));
                    if (radioNow.Checked) sys.PutValue("DefaultPeriod", "Now");
                    if (radioPrevious.Checked) sys.PutValue("DefaultPeriod", "Previous");
                    sys.PutValue("DifferentBegin", DifferentBegin.Checked);
                    sys.PutValue("DefaultCalcName", CalcName.Text);
                    sys.PutValue("AllowProjectsRepetitions", AllowProjectsRepetitions.Checked);

                    var list = new List<ReportProjectSetup>();
                    foreach (DataGridViewRow r in Projects.Rows)
                        if (!r.IsNewRow)
                        {
                            try
                            {
                                var cmode = r.Get("ProjectCalcMode").ToCalcModeType();
                                isInternal |= cmode == CalcModeType.Internal;
                                list.Add(new ReportProjectSetup(r.Get("Project"), AllowProjectsRepetitions.Checked ? r.Get("ProjectCode2") : null, cmode));
                            }
                            catch { }
                        }
                    sys.PutProjects(list);
                }
                catch(Exception ex)
                {
                    GeneralRep.ShowError("Текущий файл не является файлом отчета InfoTask или был поврежден", ex);
                }

                try {tabMain.Select();} catch {}
                _codes.Clear();
                _infs.Clear();
                foreach (DataGridViewRow row in Providers.Rows)
                {
                    string name = row.Get("ProviderName");
                    _codes.Add(name, row.Get("ProviderCode"));
                    _infs.Add(name, row.Get("ProviderInf"));
                }

                int tid;
                if (!int.TryParse(ThreadId.Text, out tid)) tid = 0;
                if (isInternal)
                {
                    try
                    {
                        _book.AddEvent("Сохранение настроек в ControllerData");
                        using (var db = new DaoDb(General.ControllerFile))
                        {
                            using (var rec = new RecDao(db, "SELECT * FROM Threads WHERE ThreadId=" + tid))
                            {
                                if (!rec.HasRows()) rec.AddNew();
                                rec.Put("ApplicationType", "Excel");
                                rec.Put("IsImit", IsImit.Checked);
                                rec.Put("ImitMode", ImitMode.Text);
                                tid = rec.GetInt("ThreadId");
                            }
                            db.Execute("DELETE * FROM Providers WHERE ThreadId=" + tid);
                            db.Execute("DELETE * FROM Projects WHERE ThreadId=" + tid);
                            using (var rec = new RecDao(db, "Projects"))
                                using (var recc = new RecDao(db, "Providers"))
                                {
                                    foreach (DataGridViewRow row in Projects.Rows)
                                        if (!row.IsNewRow && row.Get("ProjectCalcMode").ToCalcModeType() == CalcModeType.Internal)
                                        {
                                            rec.AddNew();
                                            rec.Put("ThreadId", tid);
                                            rec.PutFromDataGrid("Project", row);
                                            rec.PutFromDataGrid("ProjectFile", row);
                                        }
                                    foreach (DataGridViewRow row in Providers.Rows)
                                        if (row.Get("TypeProvider") != "Архив" || row.Get("ProviderName") == "CalcArchive" || (row.Get("ProviderName") == "Report_Archive" && UseOneArchive.Checked))
                                        {
                                            recc.AddNew();
                                            recc.Put("ThreadId", tid);
                                            recc.PutFromDataGrid("ProviderType", row, "TypeProvider");
                                            recc.PutFromDataGrid("ProviderCode", row);
                                            if (row.Get("ProviderName") != "Report_Archive")
                                                recc.PutFromDataGrid("ProviderName", row);    
                                            else recc.Put("ProviderName", "CalcArchive");
                                            recc.PutFromDataGrid("ProviderInf", row);   
                                            recc.Update();
                                        }
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        GeneralRep.ShowError("Неправильный файл ControllerData.accdb", ex);
                        return;
                    }
                }
                    
                _book.AddEvent("Сохранение настроек в ReporterData");
                try //Настройки отчета в ReporterData
                {
                    using (var db = new DaoDb(General.ReporterFile))
                    {
                        using (var rec = new RecDao(db, "SELECT * FROM Reports WHERE Report = '" + CodeReport.Text + "'"))
                        {
                            if (!rec.HasRows()) rec.AddNew();
                            rec.Put("Report", CodeReport.Text);
                            rec.Put("ReportName", NameReport.Text);
                            rec.Put("ReportDescription", DescriptionReport.Text);
                            rec.Put("ReportFile", GeneralRep.Application.ActiveWorkbook.FullName);
                            rec.Put("ThreadId", isInternal ? tid : 0);
                            rec.Put("UseOneArchive", UseOneArchive.Checked);
                            rec.Put("CodeArchive", _codes["Report_Archive"]);
                            rec.Put("InfArchive", _infs["Report_Archive"]);
                            string s = "FormTo=" + (FormToTemplate.Checked ? "Template" : (FormToDir.Checked ? "Dir" : "File")) + ";";
                            s += "ResultDir=" + ResultDir.Text + ";";
                            s += "ResultFileName=" + ResultFileName.Text + ";";
                            s += "ResultFile=" + ResultFile.Text + ";";

                            s += "AddDateToName=" + (AddDateToName.Checked ? "True" : "False") + ";";
                            var df = DateNameFormat.Text;
                            if (AddDateToName.Checked && (df.Contains(":") || df.Contains(@"\") || df.Contains(@"/") || df.Contains("[") || df.Contains("]") || df.Contains("*")))
                            {
                                Different.MessageError(@"Указан недопустимый формат даты для имени файла или листа. Имя не должно содержать символов / \ : * [ ]");
                                return;    
                            }
                            s += "DateNameFormat=" + df + ";";

                            s += "AddBeginToName=" + (AddBeginToName.Checked ? "True" : "False") + ";";
                            df = BeginNameFormat.Text;
                            if (AddBeginToName.Checked && (df.Contains(":") || df.Contains(@"\") || df.Contains(@"/") || df.Contains("[") || df.Contains("]") || df.Contains("*")))
                            {
                                Different.MessageError(@"Указан недопустимый формат даты для имени файла или листа. Имя не должно содержать символов / \ : * [ ]");
                                return;
                            }
                            s += "BeginNameFormat=" + df + ";";

                            s += "AddEndToName=" + (AddEndToName.Checked ? "True" : "False") + ";";
                            df = EndNameFormat.Text;
                            if (AddEndToName.Checked && (df.Contains(":") || df.Contains(@"\") || df.Contains(@"/") || df.Contains("[") || df.Contains("]") || df.Contains("*")))
                            {
                                Different.MessageError(@"Указан недопустимый формат даты для имени файла или листа. Имя не должно содержать символов / \ : * [ ]");
                                return;
                            }
                            s += "EndNameFormat=" + df + ";";

                            s += "AddCalcNameToName=" + (AddCalcNameToName.Checked ? "True" : "False") + ";";
                            s += "AddSheetToName=" + (AddSheetToName.Checked ? "True" : "False") + ";";
                            s += "SetFocusToFormed=" + (SetFocusToFormed.Checked ? "True" : "False") + ";";
                            rec.Put("FormInf", s);
                            rec.Put("ServerReport", ServerReport.Text);
                            if (!ServerReport.Text.IsEmpty())
                            {
                                var file = new FileInfo(ServerReport.Text);
                                if (!file.Exists || file.Extension != ".xlsx")
                                    Different.MessageError("Указанный файл контрольного экземпляра не существует или не является файлом Excel, " + ServerReport.Text);
                                else if (ServerReport.Text == _book.Workbook.FullName)
                                    Different.MessageError("Файл контрольного бланка расчета не должен совпадать с файлом бланка отчета, " + ServerReport.Text);
                            }

                            int rid = rec.GetInt("ReportId");
                            rec.Update();
                            db.Execute("DELETE * FROM Projects WHERE ReportId=" + rid);
                            using (var recp = new RecDao(db, "Projects"))
                                foreach (DataGridViewRow r  in Projects.Rows)
                                    if (!r.IsNewRow)
                                    {
                                        recp.AddNew();
                                        recp.Put("ReportId", rid);
                                        recp.PutFromDataGrid("Project", r);
                                        if (AllowProjectsRepetitions.Checked) recp.PutFromDataGrid("ProjectCode2", r);
                                        recp.PutFromDataGrid("ProjectName", r);
                                        recp.PutFromDataGrid("ProjectFile", r);
                                        if (UseOneArchive.Checked)
                                        {
                                            recp.Put("CodeArchive", _codes["Report_Archive"]);
                                            recp.Put("InfArchive", _infs["Report_Archive"]);
                                        }
                                        else if (r.Get("ProjectCalcMode").ToCalcModeType() == CalcModeType.Internal)
                                        {
                                            recp.Put("CodeArchive", _codes["CalcArchive"]);
                                            recp.Put("InfArchive", _infs["CalcArchive"]);
                                        }
                                        else
                                        {
                                            var proj = r.Get("Project");
                                            if (AllowProjectsRepetitions.Checked && !r.Get("ProjectCode2").IsEmpty())
                                                proj = r.Get("ProjectCode2");
                                            proj += "_Archive";
                                            recp.Put("CodeArchive", _codes[proj]);
                                            recp.Put("InfArchive", _infs[proj]);
                                        }
                                    }
                        }
                    }
                }
                catch (Exception ex)
                {
                    GeneralRep.ShowError("Неправильный файл ReporterData.accdb", ex);
                }

                _book.ClearReportDataFromMemory();
                CheckProviders();
                Close();    
            }
        }

        //Проверка связи с провайдерами
        private void CheckProviders()
        {
            using (_book.Start())
            {
                foreach (DataGridViewRow r in Providers.Rows)
                {
                    try
                    {
                        _book.AddEvent("Проверка соединения с провайдером", r.Get("TypeProvider") + ", " + r.Get("ProviderCode") + ", " + r.Get("ProviderName"));
                        General.RunProvider(r.Get("ProviderCode"), r.Get("ProviderName"), r.Get("ProviderInf"), _book, ProviderSetupType.Reporter).CheckConnection();
                    }
                    catch (Exception ex)
                    {
                        _book.AddError("Ошибка соединения с провайдером", ex, r.Get("TypeProvider") + ", " + r.Get("ProviderCode") + ", " + r.Get("ProviderName"));
                    }
                }
                if (_book.Command.IsError)
                    Different.MessageError(_book.Command.ErrorMessage());
            }
        }

        private void FormSetupWin_Load(object sender, EventArgs e)
        {
            _setupIsLoaded = false;
            _book = GeneralRep.ActiveBook;
            _book.AddEvent("Загрузка свойств отчета");
            var sys = _book.SysPage;
            try
            {   //Свойства отчета
                sys.GetControl(CodeReport, "Report");
                sys.GetControl(NameReport, "ReportName");
                sys.GetControl(DescriptionReport, "ReportDescription");
                FileReport.Text = GeneralRep.Application.ActiveWorkbook.FullName;
                sys.GetControl(MonthLength);
                sys.GetControl(DayLength);
                sys.GetControl(HourLength);
                sys.GetControl(MinuteLength);
                sys.GetControl(DayStart);
                sys.GetControl(HourStart);
                sys.GetControl(MinuteStart);
                radioDifferent.Checked = sys.GetBoolValue("DifferentLength");
                radioLess.Checked = sys.GetValue("DifferentLength") == "Less";
                radioEquals.Checked = sys.GetValue("DifferentLength") == "Equals";
                radioNow.Checked = sys.GetValue("DefaultPeriod") == "Now";
                radioPrevious.Checked = sys.GetValue("DefaultPeriod") != "Now";
                DifferentBegin.Checked = sys.GetBoolValue("DifferentBegin");
                CalcName.Text = sys.GetValue("DefaultCalcName");
                AllowProjectsRepetitions.Checked = sys.GetBoolValue("AllowProjectsRepetitions");
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Текущий файл не является файлом отчета InfoTask или был поврежден", ex);
                Close();
                return;
            }

            _book.AddEvent("Загрузка настроек из ReporterData");
            Providers.Rows.Clear();
            try
            {  
                if (CodeReport.Text != null)
                {
                    using (var db = new DaoDb(General.ReporterFile))
                    {
                        int rid = 0;
                        using (var rec = new RecDao(db, "SELECT * FROM Reports WHERE Report = '" + CodeReport.Text + "'"))
                            if (rec.HasRows())
                            {
                                ThreadId.Text = rec.GetInt("ThreadId").ToString();
                                UseOneArchive.Checked = rec.GetBool("UseOneArchive");
                                ServerReport.Text = rec.GetString("ServerReport");
                                var dic = rec.GetString("FormInf").ToPropertyDicS();
                                FormToTemplate.Checked = dic["FormTo"] == "Template";
                                FormToDir.Checked = dic["FormTo"] == "Dir";
                                FormToFile.Checked = dic["FormTo"] == "File";
                                ResultDir.Text = dic["ResultDir"];
                                ResultFileName.Text = dic["ResultFileName"];
                                ResultFile.Text = dic["ResultFile"];
                                AddDateToName.Checked = dic.GetBool("AddDateToName");
                                DateNameFormat.Text = dic["DateNameFormat"];
                                AddBeginToName.Checked = dic.GetBool("AddBeginToName");
                                BeginNameFormat.Text = dic["BeginNameFormat"];
                                AddEndToName.Checked = dic.GetBool("AddEndToName");
                                EndNameFormat.Text = dic["EndNameFormat"];
                                AddCalcNameToName.Checked = dic.GetBool("AddCalcNameToName");
                                AddSheetToName.Checked = dic.GetBool("AddSheetToName");
                                SetFocusToFormed.Checked = dic.GetBool("SetFocusToFormed");
                                rid = rec.GetInt("ReportId");
                                _codes.Add("Report_Archive", rec.GetString("CodeArchive"));
                                _infs.Add("Report_Archive", rec.GetString("InfArchive"));
                            }
                        SetFormingEnabled();
                        labelThreadId.Visible = ThreadId.Visible;

                        //Список проектов
                        using (var recr = new RecDao(db, "SELECT * FROM Projects WHERE ReportId =" + rid))
                            foreach (var pr in sys.GetProjects().Values)
                            {
                                int rn = Projects.Rows.Add();
                                var cells = Projects.Rows[rn].Cells;
                                bool b = recr.FindFirst("Project", pr.Code);
                                if (AllowProjectsRepetitions.Checked)
                                    b = !pr.Code2.IsEmpty() 
                                        ? recr.FindFirst("ProjectCode2", pr.Code2) 
                                        : recr.FindFirst("(Project='" + pr.Code + "') AND ((ProjectCode2 = '') OR (ProjectCode2 Is Null))");
                                if (b)
                                {
                                    recr.GetToDataGrid("ProjectFile", cells);
                                    recr.GetToDataGrid("ProjectName", cells);
                                    if (AllowProjectsRepetitions.Checked) recr.GetToDataGrid("ProjectCode2", cells);
                                    cells["Project"] = new DataGridViewTextBoxCell();
                                    if (!UseOneArchive.Checked && pr.CalcMode != CalcModeType.Internal)
                                    {
                                        var acode = recr.GetString("CodeArchive");
                                        var ainf = recr.GetString("InfArchive");
                                        _codes.Add(pr.CodeFinal + "_Archive", acode);
                                        _infs.Add(pr.CodeFinal + "_Archive", ainf);    
                                    }
                                }
                                cells["Project"].Value = pr.Code;
                                cells["ProjectCalcMode"].Value = pr.CalcMode.ToRussian();
                                if (AllowProjectsRepetitions.Checked) cells["ProjectCode2"].Value = pr.Code2;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Неправильный файл ReporterData.accdb", ex);
                Close();
                return;
            }

            _book.AddEvent("Загрузка настроек из ControllerData");
            try
            {
                int tid;
                if (!int.TryParse(ThreadId.Text, out tid)) tid = 0;
                if (tid != 0)
                    using (var rect = new RecDao(General.ControllerFile, "SELECT * FROM Threads WHERE ThreadId = " + tid))
                    {
                        IsImit.Checked = rect.GetBool("IsImit");
                        ImitMode.Text = rect.GetString("ImitMode");
                        using (var rec = new RecDao(rect.DaoDb, "SELECT * FROM Providers WHERE ThreadId=" + tid))
                            while (rec.Read())
                            {
                                string type = rec.GetString("ProviderType");
                                if (type == "Источник" || type == "Приемник" || (type == "Архив" && !UseOneArchive.Checked) || (type == "Имитатор" && IsImit.Checked))
                                {
                                    string name = rec.GetString("ProviderName");
                                    _codes.Add(name, rec.GetString("ProviderCode"));
                                    _infs.Add(name, rec.GetString("ProviderInf"));        
                                }
                            }
                    }
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Неправильный файл ControllerData.accdb", ex);
            }
            _setupIsLoaded = true;
            UpdateProvidersList();
        }

        //Загружает список провайдеров в соответствии со списком проектов, ControllerData и уже загруженным списком провайдеров
        private void UpdateProvidersList()
        {
            if (!_setupIsLoaded) return;
            foreach (DataGridViewRow row in Providers.Rows )
            {
                string name = row.Get("ProviderName");
                _codes.Add(name, row.Get("ProviderCode"), true);
                _infs.Add(name, row.Get("ProviderInf"), true);
            }
            Providers.Rows.Clear();

            ProjectCode2.Visible = AllowProjectsRepetitions.Checked;
            if (AllowProjectsRepetitions.Checked)
            {
                UseOneArchive.Checked = false;
                UseOneArchive.Visible = false;
            }
            else UseOneArchive.Visible = true;

            AddProviderToGrid("Архив", "Report_Archive", "Архив журнала отчетов", _codes.Get("Report_Archive", "AccessArchive"), _infs["Report_Archive"]);
            bool hasSingle = false;
            foreach (DataGridViewRow row in Projects.Rows)
                hasSingle |= !row.IsNewRow && row.Get("ProjectCalcMode").ToCalcModeType() == CalcModeType.Internal;
            labelThreadId.Visible = hasSingle;
            ThreadId.Visible = hasSingle;
            IsImit.Visible = hasSingle;
            ImitMode.Visible = hasSingle;
            ImitMode.Enabled = IsImit.Checked;

            var set = new HashSet<string>();
            foreach (DataGridViewRow row in Projects.Rows)
                if (!row.IsNewRow)
                {
                    string proj = row.Get("Project");
                    if (AllowProjectsRepetitions.Checked && !row.Get("ProjectCode2").IsEmpty())
                        proj = row.Get("ProjectCode2");
                    string name = proj + "_Archive";
                    if (row.Get("ProjectCalcMode").ToCalcModeType() != CalcModeType.Internal)
                    {
                        if (!UseOneArchive.Checked)
                            AddProviderToGrid("Архив", name, "Архив проекта " + proj, _codes.Get(name, "AccessArchive"), _infs[name]);
                    }
                    else
                    {
                        try
                        {
                            var projFile = row.Get("ProjectFile");
                            if (new FileInfo(projFile).Exists)
                                using (var rec = new ReaderAdo(projFile, "SELECT * FROM Providers"))
                                    while (rec.Read())
                                    {
                                        string type = rec.GetString("ProviderType");
                                        var sname = type == "Архив" ? "CalcArchive" : rec.GetString("ProviderName");
                                        if (!set.Contains(sname) && (type == "Источник" || type == "Приемник" || (type == "Архив" && !UseOneArchive.Checked) || (type == "Имитатор" && IsImit.Checked)))
                                        {
                                            if (_codes.ContainsKey(sname))
                                                AddProviderToGrid(type, sname, type + " потока отчета", _codes[sname], _infs[sname]);
                                            else AddProviderToGrid(type, sname, type + " потока отчета", rec.GetString("ProviderCode"), "");
                                            set.Add(sname);
                                        }
                                    }
                        }
                        catch { }
                    }
                }
        }

        //Добавляет строчку в грид Providers
        private void AddProviderToGrid(string type, string name, string desc, string code, string inf)
        {
            int row = Providers.Rows.Add();
            var cells = Providers.Rows[row].Cells;
            cells["TypeProvider"].Value = type;
            cells["ProviderName"].Value = name;
            cells["Description"].Value = desc;
            cells["ProviderCode"].Value = code;
            cells["ProviderInf"].Value = inf;
            try
            {
                var cell = new DataGridViewComboBoxCell();
                cells[1] = cell;
                foreach (var c in General.ProviderConfigs[code].JointProviders)
                    cell.Items.Add(c);
                cell.Value = code;
            }
            catch { }
        }
       
        private void Projects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try //Нажатие кнопки обзор
            {
                if (e.ColumnIndex == 5 && e.RowIndex >= 0)
                {
                    try
                    {
                        string t = "Файл проекта ";
                        var isNewRow = Projects.Rows[e.RowIndex].IsNewRow;
                        try { if (!isNewRow) t += Projects.Rows[e.RowIndex].Get("Project");}
                        catch { }
                        var d = new DialogCommand(DialogType.OpenFile) {DialogTitle = t, ErrorMessage = "Указан недопустимый файл проекта"};
                        d.FileTables = new[] {"CalcParams", "CalcSubParams", "GraficsList", "SignalsInUse", "Providers"};
                        string file = d.Run(Projects.Rows[e.RowIndex].Get("ProjectFile"));
                        if (file.IsEmpty()) return;
                        if (isNewRow) Projects.Rows.Add();
                        var cells = Projects.Rows[e.RowIndex].Cells;
                        cells["ProjectFile"].Value = file;
                        cells["Project"] = new DataGridViewTextBoxCell();
                        using (var syst = new SysTabl(file))
                        {
                            string s = syst.SubValue("ProjectInfo", "Project");
                            if (s != null) cells["Project"].Value = s;
                            else cells["Project"].Value = DBNull.Value;
                            s = syst.SubValue("ProjectInfo", "ProjectName");
                            if (s != null) cells["ProjectName"].Value = s;
                            else cells["ProjectName"].Value = DBNull.Value;
                        }
                        UpdateProvidersList();
                    }
                    catch (Exception ex)
                    {
                        GeneralRep.ShowError("Указан недопустимый файл проекта", ex);
                    }
                }    
            }
            catch {}
        }
        
        private void Projects_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateProvidersList();
        }

        private void Projects_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 3) 
                UpdateProvidersList();
        }

        private void IsImit_CheckedChanged(object sender, EventArgs e)
        {
            UpdateProvidersList();
        }
        
        private void UseOneArchive_Click(object sender, EventArgs e)
        {
            UpdateProvidersList();
        }

        private void AllowProjectsRepetitions_CheckedChanged(object sender, EventArgs e)
        {
            UpdateProvidersList();
        }

        private void FormSetupWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.Setup);
        }

        private void Providers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var row = Providers.Rows[e.RowIndex];
                var name = row.Get("ProviderName");
                var setupType = row.Get("TypeProvider") != "Архив" || name == "CalcArchive" || name == "Report_Archive" ? ProviderSetupType.Reporter : ProviderSetupType.ReporterArchive;
                row.Cells["ProviderInf"].Value = General.RunProvider(row.Get("ProviderCode"), name, row.Get("ProviderInf"), _book, setupType).Setup();
            }
            catch { }
        }

        //Устанавливает доступность элементов формы
        private void SetFormingEnabled()
        {
            ResultDir.ReadOnly = !FormToDir.Checked;
            ButResultDir.Enabled = FormToDir.Checked;
            ResultFileName.ReadOnly = !FormToDir.Checked;
            ResultFile.ReadOnly = !FormToFile.Checked;
            ButResultFile.Enabled = FormToFile.Checked;
            ButCreateResultFile.Enabled = FormToFile.Checked;

            AddDateToName.Enabled = !FormToTemplate.Checked;
            DateNameFormat.Enabled = !FormToTemplate.Checked;
            AddBeginToName.Enabled = !FormToTemplate.Checked;
            BeginNameFormat.Enabled = !FormToTemplate.Checked;
            AddEndToName.Enabled = !FormToTemplate.Checked;
            EndNameFormat.Enabled = !FormToTemplate.Checked;
            AddCalcNameToName.Enabled = !FormToTemplate.Checked;
            AddSheetToName.Enabled = FormToFile.Checked;
        }

        private void FormToDir_CheckedChanged(object sender, EventArgs e)
        {
            SetFormingEnabled();
        }

        private void FormToTemplate_CheckedChanged(object sender, EventArgs e)
        {
            SetFormingEnabled();
        }

        private void FormToFile_CheckedChanged(object sender, EventArgs e)
        {
            SetFormingEnabled();
        }

        private void AddDateToName_CheckedChanged(object sender, EventArgs e)
        {
            SetFormingEnabled();
        }
        
        private void ButResultDir_Click(object sender, EventArgs e)
        {
            ResultDir.Text = new DialogCommand(DialogType.OpenDir) { DialogTitle = "Каталог сформированных отчетов" }.Run(ResultDir.Text);
        }

        private void ButResultFile_Click(object sender, EventArgs e)
        {
            ResultFile.Text = new DialogCommand(DialogType.OpenExcel) {DialogTitle = "Выбрать файл сформированных отчетов"}.Run(ResultFile.Text);
        }

        private void ButCreateResultFile_Click(object sender, EventArgs e)
        {
            ResultFile.Text = new DialogCommand(DialogType.CreateExcel)
                {
                    DialogTitle = "Создать файл сформированных отчетов",
                    TemplateFile = General.ReporterDir + "ClearTemplate.xlsx"
                }.Run(ResultFile.Text);
        }

        private void ButServerReport_Click(object sender, EventArgs e)
        {
            ServerReport.Text = new DialogCommand(DialogType.OpenExcel) { DialogTitle = "Выбрать контрольный файл бланка шаблона отчета" }.Run(ServerReport.Text);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabReport_Click(object sender, EventArgs e)
        {

        }
    }
}
