using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReporterCommon
{
    public partial class FormSaveReport : Form
    {
        public FormSaveReport()
        {
            InitializeComponent();
        }

        private ReportBook _book;

        private void ButCancel_Click(object sender, EventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.SaveReport, true);
        }

        private void ButOk_Click(object sender, EventArgs e)
        {
            _book.Interval.Name = IntervalName.Text;
            _book.SaveReport();
            GeneralRep.CloseForm(ReporterCommand.SaveReport, true);
        }

        private void FormSaveReport_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            IntervalName.Text = _book.Interval.Name;
            PeriodBegin.Text = _book.Interval.Begin.ToString();
            PeriodEnd.Text = _book.Interval.End.ToString();
        }
    }
}
