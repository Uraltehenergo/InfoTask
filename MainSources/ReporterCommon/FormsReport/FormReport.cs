using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;
using ControllerClient;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Interop.Excel;
using Workbook = Microsoft.Office.Interop.Excel.Workbook;
using Worksheet = Microsoft.Office.Tools.Excel.Worksheet;

namespace ReporterCommon
{
    public partial class FormReport : Form
    {
        public FormReport()
        {
            InitializeComponent();
        }

        //Ссылка на ReportBook
        private ReportBook _book;
        
        private void FormReportWin_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            var sys = _book.SysPage;
            using (_book.StartAtom("Открытие формы построения отчета"))
            {
                try
                {
                    _book.AddEvent("Настройка внешнего вида");
                    panelPeriod.Visible = !_book.OnlyAbsolute;
                    butSaveHandInput.Visible = _book.HandInputProjects.Count > 0;

                    //Загрузка данных с SysPage
                    IntervalName.Text = sys.GetValue("DefaultCalcName");
                    if (IntervalName.Text.IsEmpty())
                        IntervalName.Text = sys.GetValue("CalcName");
                    FillPages.Text = sys.GetValue("FillPages");
                    SaveToArchive.Checked = sys.GetBoolValue("SaveToArchive");
                    PeriodEnd.Enabled = PeriodEndPicker.Enabled = _book.DifferentLength != DifferentLength.Equals;
                    string s = "";
                    int d = sys.GetIntValue("DayLength");
                    if (d != 0) s += d + " сут ";
                    d = sys.GetIntValue("HourLength");
                    if (d != 0) s += d + " час ";
                    d = sys.GetIntValue("MinuteLength");
                    if (d != 0) s += d + " мин ";
                    PeriodLength.Text = s;
                    butPreviousInterval.Enabled = butNextInterval.Enabled = s != "";

                    switch (_book.DefaultPeriod)
                    {
                        case "Previous":
                            _book.SysPage.GetControl(PeriodBegin);
                            _book.SysPage.GetControl(PeriodEnd);
                            PeriodBegin.ChangePickerValue(PeriodBeginPicker);
                            PeriodEnd.ChangePickerValue(PeriodEndPicker);
                            break;
                        case "Now":
                            SetNowPeriod();
                            break;
                    }
                    UpdateSourcesTime();
                }
                catch (Exception ex)
                {
                    GeneralRep.ShowError("Ошибка при открытии формы построения отчетов", ex);
                }
            }
        }

        //Устанавливает период отчета, исходя из текущего времени
        private void SetNowPeriod()
        {
            var now = DateTime.Now;
            if (_book.PeriodLength.TotalSeconds == 0 && _book.MonthLengh == 0)
            {
                PeriodBegin.Text = now.ToString();
                PeriodEnd.Text = now.ToString();
            }
            else
            {
                if (_book.PeriodNull)
                {
                    PeriodEnd.Text = now.ToString();
                    PeriodBegin.Text = now.Subtract(_book.PeriodLength).AddMonths(-_book.MonthLengh).ToString();
                }
                else
                {
                    var time = (now.Subtract(_book.PeriodLength).AddMonths(-_book.MonthLengh));
                    if (time.Day < _book.DayStart) time = time.AddMonths(-1);
                    if (_book.MonthLengh > 0) time = time.AddDays(-time.Day).AddDays(_book.DayStart);

                    time = time.TimeOfDay > _book.PeriodStart ? time : time.Subtract(new TimeSpan(1, 0, 0, 0));
                    time = time.Date.Add(_book.PeriodStart);
                    while (time.Add(_book.PeriodLength).AddMonths(_book.MonthLengh) <= now)
                        time = time.Add(_book.PeriodLength).AddMonths(_book.MonthLengh);
                    PeriodEnd.Text = time.ToString();
                    PeriodBegin.Text = time.Subtract(_book.PeriodLength).AddMonths(-_book.MonthLengh).ToString();
                }
            }
        }

        //Вызываетя при закрытии формы
        private void CloseSaving()
        {
            try
            {
                _book.SysPage.PutControl(PeriodBegin);
                _book.SysPage.PutControl(PeriodEnd);
                _book.SysPage.PutValue("CalcName", IntervalName.Text);
                _book.SysPage.PutValue("FillPages", FillPages.Text);
                _book.SysPage.PutValue("SaveToArchive", SaveToArchive.Checked);
                _book.CloseForm(ReporterCommand.Report);
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при закрытия формы построения отчетов", ex);
            }
        }

        private void FormReportWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseSaving();
        }

        private void butNextInterval_Click(object sender, EventArgs e)
        {
            DateTime d;
            if (DateTime.TryParse(PeriodEnd.Text, out d))
            {
                PeriodBegin.Text = Convert.ToString(d);
                PeriodEnd.Text = Convert.ToString(d.Add(_book.PeriodLength).AddMonths(_book.MonthLengh));
            }
        }

        private void butPreviousInterval_Click(object sender, EventArgs e)
        {
            DateTime d;
            if (DateTime.TryParse(PeriodBegin.Text, out d))
            {
                PeriodEnd.Text = Convert.ToString(d);
                PeriodBegin.Text = Convert.ToString(d.Subtract(_book.PeriodLength).AddMonths(-_book.MonthLengh));
            }
        }

        //Формирование отчета, для вызова в другом потоке
        private void FormingReport(string fill, //fill - заполнять все листы или выбранный
                                                  bool saveReport) //saveReport - сохранять отчет в журнал
        {
            var beg = _book.Interval.Begin;
            var en = _book.Interval.End;
            var cname = _book.Interval.Name;
            using (_book.StartAtom("Формирование отчета", true, beg + " - " + en + " " + cname))
            {
                try
                {
                    bool hasCalc = _book.Controller != null;
                    if (hasCalc)
                    {
                        _book.AddEvent("Расчет", beg + " - " + en + " " + cname);
                        var con = _book.Controller;
                        con.SetCalcOperations(true, true, true);
                        con.Calc(beg, en, "Default", cname);
                        if (!con.ErrorMessage.IsEmpty())
                            _book.AddError("Ошибки при расчете:\n", null, con.ErrorMessage);
                        _book.Procent = 50;
                    }
                    //Получение данных из архива
                    using (_book.Start(hasCalc ? 50 : 0, hasCalc ? 60 : 40))
                        _book.ReadArchiveReport();

                    //Заполнение отчета
                    using (_book.Start(hasCalc ? 60 : 40, 80))
                    {
                        _book.OpenFormingBook(beg, en, cname);
                        _book.FillReport(fill);
                        _book.CopySheetsToBook(beg, cname, fill);
                    }
                    
                    //Сохранение результатов в архив
                    if (saveReport)
                        using (_book.Start(80))
                            _book.SaveReport();
                    try { _book.Workbook.Save(); } catch { }
                }
                catch (Exception ex)
                {
                    _book.AddError("Ошибка заполнения отчета", ex);
                }
                _book.ShowError();
            }
            SetCalcMode(false);
            if (!_book.FormToDir && !_book.FormToFile)
                _book.MessageReport("Отчет сформирован");
        }

        //Поток для заполнения отчета
        private Thread _task;

        private void butFormReport_Click(object sender, EventArgs e)
        {
            if (!GeneralRep.CheckOneSheet(false)) return;
            var d = DateTime.Now;
            var t = new TimeInterval(Different.MinDate.AddYears(1), d.AddMilliseconds(-d.Millisecond));
            if (!_book.OnlyAbsolute)
            {
                t = _book.CheckPeriodTime(PeriodBegin.Text, PeriodEnd.Text);
                if (t == null) return;    
            }
            var fill = FillPages.Text;
            var save = SaveToArchive.Checked;

            _book.IsReportForming = true;
            _book.Interval = new ArchiveInterval(IntervalType.Single, t.Begin, t.End, IntervalName.Text);
            GeneralRep.Application.Visible = false;
            SetCalcMode(true);
            if (_book.ThreadId == 0)
                FormingReport(fill, save);
            else
            {
                if (!_book.PrepareController())
                    Different.MessageError("Ошибка запуска расчета");
                _task = new Thread(() => FormingReport(fill, save)) { Priority = ThreadPriority.AboveNormal };
                _task.Start();    
            }
        }

        private void butBreak_Click(object sender, EventArgs e)
        {
            if (Different.MessageQuestion("Прервать заполнение отчета?"))
            { 
                try
                {
                    _task.Abort();
                    _task = null;
                }
                catch { }
                Thread.Sleep(1000);
                try
                {
                    _book.Controller = null;
                    if (_book.CommandLog != null)
                        _book.AddEvent("Выполнение прервано");
                    while (_book.Command != null)
                        _book.Finish("Выполнение прервано", true);
                }
                catch { }
                SetCalcMode(false);
            }    
        }

        private delegate void IsCalcDelegate();

        //Установить, запущен расчет или нет
        public void SetCalcMode(bool isCalc)
        {
            IsCalc = isCalc;
            if (IsCalc)
                Invoke(new IsCalcDelegate(() =>
                {
                    butSourcesTime.Enabled = false;
                    butFormReport.Enabled = false;
                    butSaveHandInput.Enabled = false;
                    butBreak.Visible = true;
                    PeriodBegin.ReadOnly = true;
                    PeriodBeginPicker.Enabled = false;
                    PeriodEnd.ReadOnly = true;
                    butNextInterval.Enabled = false;
                    butPreviousInterval.Enabled = false;
                    butNowInterval.Enabled = false;
                    SaveToArchive.Enabled = false;
                    IntervalName.ReadOnly = true;
                    FillPages.Enabled = false;
                }));
            else
                Invoke(new IsCalcDelegate(() =>
                {
                    CloseSaving();
                    _book.IsReportForming = false;
                    GeneralRep.Application.Visible = true;
                    if (_book.ResultBook != null && _book.FormInf.GetBool("SetFocusToFormed"))
                        _book.ResultBook.ActivateBook();
                    else _book.Workbook.ActivateBook();
                    Close();
                }));
        }

        public bool IsCalc { get; set; }

        private void butSourcesTime_Click(object sender, EventArgs e)
        {
            UpdateSourcesTime();
        }

        //Обновление диапазона источников
        private void UpdateSourcesTime()
        {
            var t = _book.UpdateRanges();
            SourcesBegin.Text = t.Begin.ToString();
            SourcesEnd.Text = t.End.ToString();
        }

        private void butNowInterval_Click(object sender, EventArgs e)
        {
            SetNowPeriod();
        }

        private void ButArchivesRanges_Click(object sender, EventArgs e)
        {
            _book.RunCommandReporter(ReporterCommand.ArchivesRanges);
        }

        private void ButSaveHandInput_Click(object sender, EventArgs e)
        {
            TimeInterval t = _book.CheckPeriodTime(PeriodBegin.Text, PeriodEnd.Text);
            if (t == null) return;
            using (_book.StartAtom("Сохранение значений ручного ввода"))
            {
                foreach (var pr in _book.HandInputProjects.Values)
                    pr.WriteHandInput(PeriodBegin.Text.ToDateTime(), PeriodEnd.Text.ToDateTime());
                if (_book.Command.IsError)
                    Different.MessageError(_book.Command.ErrorMessage());
            }
        }

        private void PeriodBeginPicker_ValueChanged(object sender, EventArgs e)
        {
            try 
            { 
                PeriodBegin.Text = PeriodBeginPicker.Value.ToString();
                if (PeriodEnd.Enabled == false)
                    PeriodEnd.Text = PeriodBegin.Text.ToDateTime().Add(_book.PeriodLength).AddMonths(_book.MonthLengh).ToString();
            }
            catch { }
        }

        private void PeriodEndPicker_ValueChanged(object sender, EventArgs e)
        {
            try { PeriodEnd.Text = PeriodEndPicker.Value.ToString(); }
            catch { }
        }

        private void PeriodBegin_Validated(object sender, EventArgs e)
        {
            PeriodBegin.ChangePickerValue(PeriodBeginPicker);
            if (PeriodEnd.Enabled == false)
                PeriodEnd.Text = PeriodBegin.Text.ToDateTime().Add(_book.PeriodLength).AddMonths(_book.MonthLengh).ToString();
        }

        private void PeriodEnd_Validated(object sender, EventArgs e)
        {
            PeriodEnd.ChangePickerValue(PeriodEndPicker);
        }
    }
}