using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;
using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    public partial class FormLiveReport : Form
    {
        public FormLiveReport()
        {
            InitializeComponent();
        }

        //Ссылка на ReportBook
        private ReportBook _book;

        private void FormLiveReport_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            _book.FormingBook = _book.Workbook;
            var sys = _book.SysPage;
            using (_book.StartAtom("Открытие формы динамических отчетов"))
            {
                try
                {
                    _book.AddEvent("Настройка внешнего вида");
                    PeriodLength.Text = sys.GetValue("LiveReportPeriodLength");
                    SourcesLate.Text = sys.GetValue("LiveReportSourcesLate");
                    FillPages.Text = sys.GetValue("LiveReportFillPages");
                }
                catch (Exception ex)
                {
                    GeneralRep.ShowError("Ошибка при открытии формы динамических отчетов", ex);
                }
            }
        }

        //Длина периода и задержка источника
        private int _periodLength;
        private int _sourcesLate;

        //Текущий период отчета
        private DateTime _periodBegin;
        private DateTime _periodEnd;
        //Поток для заполнения отчета
        private Thread _task;

        private void ButStart_Click(object sender, EventArgs e)
        {
            if (!SetInitialPeriod()) return;
            try { ((Range)((Worksheet)_book.Workbook.ActiveSheet).Cells[1, 1]).Select(); } catch { }
            ButStart.Enabled = false;
            ButStop.Enabled = true;
            PeriodLength.Enabled = false;
            SourcesLate.Enabled = false;
            FillPages.Enabled = false;
            _book.AddEvent("Запуск режима динамического формирования отчета");
            timer1.Start();
        }

        private bool SetInitialPeriod()
        {
            _periodLength = PeriodLength.Text.ToInt();
            _sourcesLate = SourcesLate.Text.ToInt();
            if (_periodLength == 0)
            {
                Different.MessageInfo("Не заполнена длина интервала отчета");
                return false;
            }
            SaveSettings();
            var d = DateTime.Now.AddMinutes(-_sourcesLate);
            d = d.Date.AddHours(d.Hour).AddMinutes(d.Minute);
            _periodEnd = d.AddMinutes(-(d.Minute % _periodLength));
            _periodBegin = _periodEnd.AddMinutes(-_periodLength);
            PeriodBegin.Text = _periodBegin.ToString();
            PeriodEnd.Text = _periodEnd.ToString();
            return _book.CheckPeriodTime(PeriodBegin.Text, PeriodEnd.Text) != null;
        }

        private void SaveSettings()
        {
            var sys = _book.SysPage;
            sys.PutValue("LiveReportPeriodLength", PeriodLength.Text);
            sys.PutValue("LiveReportSourcesLate", SourcesLate.Text);
            sys.PutValue("LiveReportFillPages", FillPages.Text);
        }

        private void ButStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            ButStop.Enabled = false;
            ButStart.Enabled = true;
            PeriodLength.Enabled = true;
            SourcesLate.Enabled = true;
            FillPages.Enabled = true;
            _book.AddEvent("Остановка режима динамического формирования отчета");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now >= _periodEnd.AddMinutes(_sourcesLate))
            {
                timer1.Stop();
                PeriodBegin.Text = _periodBegin.ToString();
                PeriodEnd.Text = _periodEnd.ToString();
                FillReport();
                _periodBegin = _periodEnd;
                _periodEnd = _periodBegin.AddMinutes(_periodLength);
                timer1.Start();
            }
        }

        //Запполнение отчета
        private void FillReport()
        {
            _book.IsReportForming = true;
            _book.Interval = new ArchiveInterval(IntervalType.Single, _periodBegin, _periodEnd);
            if (_book.ThreadId == 0)
                FormingReport();
            else
            {
                if (!_book.PrepareController())
                    Different.MessageError("Ошибка запуска расчета");
                _task = new Thread(FormingReport) { Priority = ThreadPriority.AboveNormal };
                _task.Start();
            }
        }

        //Формирование отчета, для вызова в другом потоке
        private void FormingReport()
        {
            _book.FormDynamicReport(_periodBegin, _periodEnd, FillPages.Text);
        }

        private void FormLiveReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            _book.CloseForm(ReporterCommand.LiveReport);
        }
    }
}
