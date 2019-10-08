using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReporterCommon.FormsLinks
{
    public partial class FormLinksList : Form
    {
        public FormLinksList()
        {
            InitializeComponent();
        }

        //Ссылка на книгу
        private ReportBook _book;

        private void FormLinksList_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
        }

        private void FormLinksList_FormClosing(object sender, FormClosingEventArgs e)
        {
            _book.CloseForm(ReporterCommand.LinksList);
        }
    }
}
