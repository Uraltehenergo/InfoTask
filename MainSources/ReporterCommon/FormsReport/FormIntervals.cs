using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace ReporterCommon
{
    public partial class FormIntervals : Form
    {
        public FormIntervals()
        {
            InitializeComponent();
        }

        private void FormIntervalsWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.Intervals);
        }

        private ReportBook _book;
        //Форма фильтров
        private FormFiltersIntervals _filterForm;

        private void UpdateIntervals()
        {
            if (_book.Archive == null) return;
            using (_book.Start(true))
            {
                try
                {
                    var t = dataSet1.Tables["IntervalsTable"];
                    t.Rows.Clear();
                    var list = _book.Archive.ReadIntervals(_book.CodeAsProject, ReportType.Excel);
                    foreach (var interval in list)
                    {
                        var r = t.NewRow();
                        r["Otm"] = false;
                        r["PeriodBegin"] = interval.Begin;
                        r["PeriodEnd"] = interval.End;
                        r["IntervalName"] = interval.Name ?? "";
                        r["TimeChange"] = interval.TimeChange;
                        t.Rows.Add(r);
                    }
                    Intervals.Update();
                    foreach (DataGridViewRow r in Intervals.Rows)
                        r.MinimumHeight = 20;
                }
                catch (Exception ex)
                {
                    _book.AddError("Ошибка при получении списка интервалов", ex);   
                }
                _book.ShowError();
            }
        }

        private void FormIntervalsWin_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            UpdateIntervals();
            new ToolTip().SetToolTip(butFilter, "Фильтрация и поиск в журнале отчетов");
            new ToolTip().SetToolTip(butCheckAll, "Отметить все");
            new ToolTip().SetToolTip(butUncheckAll, "Снять все отметки");
            new ToolTip().SetToolTip(butLoadInterval, "Загрузить выбранный отчет из журнала");
            new ToolTip().SetToolTip(butDeleteIntervals, "Удалить все отмеченные отчеты из журнала");
        }

        private void butLoadInterval_Click(object sender, EventArgs e)
        {
            try
            {
                if (Intervals.Rows.Count == 0) return;
                if (!GeneralRep.CheckOneSheet(false)) return;
                GeneralRep.Application.Visible = false;
                _book.IsReportForming = true;
                var row = Intervals.Rows[Intervals.SelectedCells[0].RowIndex];
                var beg = row.Get("PeriodBegin").ToDateTime();
                var en = row.Get("PeriodEnd").ToDateTime();
                var cname = row.Get("IntervalName");
                _book.Interval = new ArchiveInterval(IntervalType.Single, beg, en, cname);
                using (_book.StartAtom("Заполнение отчета из журнала", true, beg + " - " + en + " " + cname))
                {
                    try
                    {
                        _book.AddEvent("Загрузка интервала", beg + " - " + en + " " + cname);
                        using (_book.Start(10, 40))
                            _book.ReadReportProject();
                        using (_book.Start(40))
                        {
                            _book.FormingBook = _book.Workbook;
                            _book.FillReport("Все листы");
                        }
                        if (_book.Forms.ContainsKey(ReporterCommand.Report))
                        {
                            var f = (FormReport)_book.Forms[ReporterCommand.Report];
                            f.PeriodBegin.Text = beg.ToString();
                            f.PeriodEnd.Text = en.ToString();
                            f.IntervalName.Text = cname;
                        }
                    }
                    catch (Exception ex)
                    {
                        _book.AddError("Ошибка заполнения отчета", ex);
                    }
                    _book.ShowError();
                }
                Different.MessageInfo("Отчет загружен из журнала.\nОтчет: " + _book.Code + ", " + _book.Name + "\nПериод: " + beg.ToString() + " - " + en.ToString());
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка заполнения отчета", ex);
            }
            GeneralRep.Application.Visible = true;
            _book.Workbook.Activate();
            _book.IsReportForming = false;
        }

        private void butDeleteIntervals_Click(object sender, EventArgs e)
        {
            if (Different.MessageQuestion("Удалить отмеченные интервалы?"))
            {
                using (_book.StartAtom("Удаление интервалов из журнала"))
                {
                    var intervals = new List<ArchiveInterval>();
                    int n = 0;
                    var t = dataSet1.Tables["IntervalsTable"];
                    foreach (DataRow row in t.Rows)
                    {
                        try
                        {
                            if ((bool)row["Otm"])
                            {
                                var interval = new ArchiveInterval(IntervalType.Single, (DateTime)row["PeriodBegin"], (DateTime)row["PeriodEnd"], (string)row["IntervalName"]);
                                intervals.Add(interval);
                                n++;
                            }
                        }
                        catch { }
                    }
                    _book.AddEvent("Удаление интервалов", n + " интервалов");
                    using (_book.Start(30, 65))
                        _book.Archive.DeleteIntervals(_book.CodeAsProject, ReportType.Excel, intervals);
                    using (_book.Start(65))
                        UpdateIntervals();
                    _book.ShowError();
                }
            }
        }

        private void butCheckAll_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dataSet1.Tables[0].Rows)
            {
                try { row["Otm"] = true; }
                catch { }
            }
            Intervals.Update();
        }

        private void butUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dataSet1.Tables[0].Rows)
            {
                try { row["Otm"] = false; }
                catch { }
            }
            Intervals.Update();
        }

        private void butFilter_Click(object sender, EventArgs e)
        {
            _filterForm = (FormFiltersIntervals)_book.RunCommandReporter(ReporterCommand.FilterIntervals);
            BindingNavigator1.Items["ButSetFilter"].Enabled = true;
        }

        private void ButClearFilter_Click(object sender, EventArgs e)
        {
            _filterForm.DeleteFilter();
        }

        private void ButSetFilter_Click(object sender, EventArgs e)
        {
            _filterForm.SetFilter();
        }
    }
}
