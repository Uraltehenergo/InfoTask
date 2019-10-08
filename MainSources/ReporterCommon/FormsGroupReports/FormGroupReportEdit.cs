using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;
using Microsoft.Office.Interop.Excel;
using TextBox = System.Windows.Forms.TextBox;

namespace ReporterCommon
{
    public partial class FormGroupReportEdit : Form
    {
        public FormGroupReportEdit()
        {
            InitializeComponent();
        }

        //Id текущей настраиваемой группы отчетов
        private int _groupId;
        //Строка таблицы текущего редактируемого отчета
        private DataGridViewRow _curRow;
        //True, если форма уже загружена
        private bool _isLoaded;

        //Загружает в форму информацию о группе отчетов
        public void LoadGroup(int groupId)
        {
            _groupId = groupId;
            using (var db = new DaoDb(General.ReporterFile))
            {
                using (var rec = new ReaderAdo(db, "SELECT * FROM GroupReports WHERE GroupId=" + groupId))
                    if (rec.HasRows())
                    {
                        GroupCode.Text = rec.GetString("GroupCode");
                        GroupName.Text = rec.GetString("GroupName");
                    }

                Reports.Rows.Clear();
                using (var rec = new ReaderAdo(db, "SELECT * FROM ReportsForGroup WHERE GroupId=" + groupId))
                    while (rec.Read())
                    {
                        var row = Reports.Rows[Reports.Rows.Add()];
                        rec.GetToDataGrid("Report", row);
                        rec.GetToDataGrid("ReportName", row);
                        rec.GetToDataGrid("ReportFile", row);
                        rec.GetToDataGrid("ReportTag", row);
                    }
            }
            if (Reports.Rows.Count > 0)
                Reports.Rows[0].Selected = true;
            _isLoaded = true;
            ChangeReportsRow();
        }

        private void FormGroupReportEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.GroupReportEdit);
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (GroupCode.Text.IsEmpty())
            {
                Different.MessageError("Не заполнено имя группы");
                return;
            }
            ChangeReportsRow();
            using (var db = new DaoDb(General.ReporterFile))
            {
                //Сохранение в ReporterData
                using (var rec = new RecDao(db, "SELECT * FROM GroupReports WHERE GroupId=" + _groupId))
                {
                    rec.Put("GroupCode", GroupCode.Text);
                    rec.Put("GroupName", GroupName.Text);
                    ((FormGroupReports) GeneralRep.CommonBook.Forms[ReporterCommand.GroupReports]).EditItem(rec, _groupId);
                }

                db.Execute("DELETE * FROM ReportsForGroup WHERE GroupId=" + _groupId);
                using (var rec = new RecDao(db, "ReportsForGroup"))
                    foreach (DataGridViewRow row in Reports.Rows)
                        if (!row.IsNewRow)
                        {
                            rec.AddNew();
                            rec.Put("GroupId", _groupId);
                            rec.PutFromDataGrid("Report", row);
                            rec.PutFromDataGrid("ReportName", row);
                            rec.PutFromDataGrid("ReportFile", row);
                            rec.PutFromDataGrid("ReportTag", row);
                        }
            }
            Close();
        }
        
        private void Reports_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //Файл бланка отчета
                int ind = e.RowIndex;
                var isNewRow = Reports.Rows[ind].IsNewRow;
                if (e.ColumnIndex == 3)
                {
                    var d = new DialogCommand(DialogType.OpenExcel);
                    d.DialogTitle = "Файл шаблона отчета";
                    d.ErrorMessage = "Указан недопустимый файл бланка отчета";
                    string file = d.Run(Reports.Rows[ind].Get("ReportFile"));
                    try
                    {
                        Workbook wbook = GeneralRep.Application.Workbooks.Open(file);
                        var sys = new SysPage(wbook);
                        string code = sys.GetValue("Report");
                        string name = sys.GetValue("ReportName");
                        if (isNewRow)
                        {
                            if (file.IsEmpty()) return;
                            ind = Reports.Rows.Add();
                        }
                        var cells = Reports.Rows[ind].Cells;
                        cells["ReportFile"].Value = file;
                        cells["Report"].Value = code;
                        cells["ReportName"].Value = name;

                        var tag = cells.Get("ReportTag").IsEmpty() ? new Dictionary<string, string>() : cells.Get("ReportTag").ToPropertyDictionary();
                        AddPropToTag(tag, sys, "SaveToArchive");
                        AddPropToTag(tag, sys, "DayLength");
                        AddPropToTag(tag, sys, "HourLength");
                        AddPropToTag(tag, sys, "MinuteLength");
                        cells["ReportTag"].Value = tag.ToPropertyString();
                        wbook.Save();
                        wbook.Close();
                        Reports.Rows[ind].Selected = true;
                    }
                    catch { Different.MessageError("Указан недопустимый файл бланка отчета"); }
                }
            }
            catch { }
            ChangeReportsRow();
        }

        private void AddPropToTag(Dictionary<string, string> tag, SysPage sys, string prop)
        {
            if (!tag.ContainsKey(prop)) tag.Add(prop, sys.GetValue(prop));
            else tag[prop] = sys.GetValue(prop);
        }

        //Вызывается при изменениии выделенной строки в списке отчетов
        private void ChangeReportsRow()
        {
            if (!_isLoaded) return;
            if (_curRow != null && !_curRow.IsNewRow)
            {
                string tag = "SaveToArchive=" + (SaveToArchive.Checked ? "True" : "False") + ";";
                int res;
                if (!int.TryParse(DayLength.Text, out res)) res = 0;
                tag += "DayLength=" + res + ";";
                if (!int.TryParse(HourLength.Text, out res)) res = 0;
                tag += "HourLength=" + res + ";";
                if (!int.TryParse(MinuteLength.Text, out res)) res = 0;
                tag += "MinuteLength=" + res + ";";
                tag += "ResultDir=" + ResultDir.Text + ";";
                tag += "ResultFileName=" + ResultFileName.Text + ";";
                tag += "ResultFile=" + ResultFile.Text + ";";
                if (FormToDir.Checked) tag += "FormToDir=True;";
                if (FormToFile.Checked) tag += "FormToFile=True;";
                if (AddToFile.Checked) tag += "AddToFile=True;";
                try { _curRow.Cells["ReportTag"].Value = tag; } catch { }
            }
            if (Reports.SelectedRows.Count == 0 || Reports.SelectedRows[0].IsNewRow)
            {
                ReportPanel.Enabled = false;
                _curRow = null;
                SelectedReport.Text = null;
                SelectedReportName.Text = null;
                SelectedReportFile.Text = null;
                SaveToArchive.Checked = true;
                DayLength.Text = null;
                HourLength.Text = null;
                MinuteLength.Text = null;
                ResultDir.Text = null;
                ResultFileName.Text = null;
                ResultFile.Text = null;
                FormToDir.Checked = true;
            }
            else
            {
                ReportPanel.Enabled = true;
                _curRow = Reports.SelectedRows[0];
                SelectedReport.Text = _curRow.Get("Report");
                SelectedReportName.Text = _curRow.Get("ReportName");
                SelectedReportFile.Text = _curRow.Get("ReportFile");
                var dic = _curRow.Get("ReportTag").ToPropertyDicS();
                if (dic.Count > 0)
                {
                    SaveToArchive.Checked = dic.GetBool("SaveToArchive");
                    if ("0" == (DayLength.Text = dic["DayLength"])) DayLength.Text = null;
                    if ("0" == (HourLength.Text = dic["HourLength"])) HourLength.Text = null;
                    if ("0" == (MinuteLength.Text = dic["MinuteLength"])) MinuteLength.Text = null;
                    ResultDir.Text = dic["ResultDir"];
                    ResultFileName.Text = dic["ResultFileName"];
                    ResultFile.Text = dic["ResultFile"];
                    FormToDir.Checked = true;
                    FormToFile.Checked = dic.GetBool("FormToFile");
                    AddToFile.Checked = dic.GetBool("AddToFile");
                }
            }
         }

        private void ButSetupReport_Click(object sender, EventArgs e)
        {
            try
            {
                Workbook wbook = GeneralRep.Application.Workbooks.Open(SelectedReportFile.Text);
                var code = new SysPage(wbook).GetValue("Report");
                var book = new ReportBook(code, wbook);
                GeneralRep.Books.Add(code, book);
                book.RunCommandReporter(ReporterCommand.Setup);
            }
            catch
            {
                Different.MessageError("Указан недопустимый файл бланка отчета");
            }
        }

        private void Reports_SelectionChanged(object sender, EventArgs e)
        {
            ChangeReportsRow();
        }

        private void ButResultFile_Click(object sender, EventArgs e)
        {
            var d = new DialogCommand(DialogType.OpenExcel) {DialogTitle = "Файл для добавления сформированных отчетов"};
            ResultFile.Text = d.Run(ResultFile.Text);
        }

        private void ButResultDir_Click(object sender, EventArgs e)
        {
            var d = new DialogCommand(DialogType.OpenDir) { DialogTitle = "Каталог сформированных отчетов" };
            ResultDir.Text = d.Run(ResultDir.Text);
        }
    }
}
