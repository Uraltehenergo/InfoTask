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
    public partial class FormVideo : Form
    {
        public FormVideo()
        {
            InitializeComponent();
        }

        //Ссылка на ReportBook
        private ReportBook _book;

        private void FormVideo_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            _book.FormingBook = _book.Workbook;
            var sys = _book.SysPage;
            using (_book.StartAtom("Открытие формы видеомагнитофона"))
            {
                try
                {
                    _book.AddEvent("Настройка внешнего вида");
                    VideoBegin.Text = sys.GetValue("VideoBegin");
                    VideoEnd.Text = sys.GetValue("VideoEnd");
                    PeriodLength.Text = sys.GetValue("VideoPeriodLength");
                    BetweenPeriods.Text = sys.GetValue("VideoBetweenPeriods");
                    FillPages.Text = sys.GetValue("VideoFillPages");
                    UpdateSourcesTime();
                }
                catch (Exception ex)
                {
                    GeneralRep.ShowError("Ошибка при открытии формы видеомагнитофона", ex);
                }
            }
        }

        private void SaveSettings()
        {
            var sys = _book.SysPage;
            sys.PutValue("VideoBegin", VideoBegin.Text);
            sys.PutValue("VideoEnd", VideoEnd.Text);
            sys.PutValue("VideoPeriodLength", PeriodLength.Text);
            sys.PutValue("VideoBetweenPeriods", BetweenPeriods.Text);
            sys.PutValue("VideoFillPages", FillPages.Text);
        }

        private void FormVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            _book.CloseForm(ReporterCommand.Video);
        }
        
        //Параметры из формы
        private DateTime _videoBegin;
        private DateTime _videoEnd;
        private int _periodLength;
        private int _betweenPeriods;

        //Текущий период отчета
        private DateTime _periodBegin;
        private DateTime _periodEnd;
        //Поток для заполнения отчета
        private Thread _task;

        private void ButStart_Click(object sender, EventArgs e)
        {
            if (!SetInitialPeriod()) return;
            try { ((Range)((Worksheet) _book.Workbook.ActiveSheet).Cells[1, 1]).Select();} catch {}
            ButStart.Enabled = false;
            ButStop.Enabled = true;
            ButPause.Enabled = true;
            ButPause.Text = "Пауза";
            VideoBegin.Enabled = false;
            VideoEnd.Enabled = false;
            PeriodBeginPicker.Enabled = false;
            PeriodEndPicker.Enabled = false;
            PeriodLength.Enabled = false;
            BetweenPeriods.Enabled = false;
            FillPages.Enabled = false;
            _book.AddEvent("Запуск режима динамического формирования отчета");
            _lastTime = Different.MinDate;
            timer1.Start();
        }

        private bool SetInitialPeriod()
        {
            _videoBegin = VideoBegin.Text.ToDateTime();
            _videoEnd = VideoEnd.Text.ToDateTime();
            _periodLength = PeriodLength.Text.ToInt();
            _betweenPeriods = BetweenPeriods.Text.ToInt();
            if (_periodLength == 0 || _betweenPeriods == 0)
            {
                Different.MessageInfo("Не заполнена длина интервала отчета или длительность ожидание");
                return false;
            }
            SaveSettings();
            _periodBegin = _videoBegin;
            _periodEnd = _periodBegin.AddMinutes(_periodLength);
            PeriodBegin.Text = _periodBegin.ToString();
            PeriodEnd.Text = _periodEnd.ToString();
            return _book.CheckPeriodTime(PeriodBegin.Text, PeriodEnd.Text) != null;
        }

        private void StopProcess()
        {
            timer1.Stop();
            ButStop.Enabled = false;
            ButPause.Enabled = false;
            ButPause.Text = "Пауза";
            ButStart.Enabled = true;
            VideoBegin.Enabled = true;
            VideoEnd.Enabled = true;
            PeriodBeginPicker.Enabled = true;
            PeriodEndPicker.Enabled = true;
            PeriodLength.Enabled = true;
            BetweenPeriods.Enabled = true;
            FillPages.Enabled = true;
            _book.AddEvent("Остановка режима динамического формирования отчета");
        }

        private void ButStop_Click(object sender, EventArgs e)
        {
            StopProcess();
        }

        private void ButPause_Click(object sender, EventArgs e)
        {
            if (ButPause.Text == "Пауза")
            {
                timer1.Stop();
                ButPause.Text = "Продолжить";
                _book.AddEvent("Пауза в динамическом формировании отчета");
            }
            else
            {
                ButPause.Text = "Пауза";
                _book.AddEvent("Возобновление динамического формирования отчета");
                timer1.Start();
            }
        }

        //Время последнего формирования отчета
        private DateTime _lastTime;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(_lastTime).TotalSeconds >= _betweenPeriods)
            {
                UpdateSourcesTime();
                _lastTime = DateTime.Now;
                timer1.Stop();
                PeriodBegin.Text = _periodBegin.ToString();
                PeriodEnd.Text = _periodEnd.ToString();
                FillReport();
                if (_videoEnd <= _periodEnd)
                    StopProcess();
                else
                {
                    _periodBegin = _periodEnd;
                    _periodEnd = _periodBegin.AddMinutes(_periodLength);
                    timer1.Start();    
                }
            }
        }

        //Заполнение отчета
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

        private void ButArchivesRanges_Click(object sender, EventArgs e)
        {
            _book.RunCommandReporter(ReporterCommand.ArchivesRanges);
        }

        private void PeriodBeginPicker_ValueChanged(object sender, EventArgs e)
        {
            try { VideoBegin.Text = PeriodBeginPicker.Value.ToString(); }
            catch { }
        }

        private void PeriodEndPicker_ValueChanged(object sender, EventArgs e)
        {
            try { VideoEnd.Text = PeriodEndPicker.Value.ToString(); }
            catch { }
        }
    }
}
