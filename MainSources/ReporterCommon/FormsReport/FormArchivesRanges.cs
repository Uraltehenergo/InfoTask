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
    public partial class FormArchivesRanges : Form
    {
        public FormArchivesRanges()
        {
            InitializeComponent();
        }

        private void FormArchivesRanges_FormClosed(object sender, FormClosedEventArgs e)
        {
            GeneralRep.ActiveBook.CloseForm(ReporterCommand.ArchivesRanges);
        }

        private void FormArchivesRanges_Load(object sender, EventArgs e)
        {
            LoadRanges();
        }

        private void butArchiveTime_Click(object sender, EventArgs e)
        {
            LoadRanges();
        }

        //Запрос и заполнение списка диапазонов из архивов и источников
        private void LoadRanges()
        {
            RangesList.Rows.Clear();
            var book = GeneralRep.ActiveBook;
            book.UpdateRanges();
            foreach (var r in book.Ranges)
            {
                var cells = RangesList.Rows[RangesList.Rows.Add()].Cells;
                cells["Provider"].Value = r.ProviderName;
                cells["Interval"].Value = r.Interval;
                cells["TimeBegin"].Value = r.Begin.ToString();
                cells["TimeEnd"].Value = r.End.ToString();
            }
        }
    }
}
