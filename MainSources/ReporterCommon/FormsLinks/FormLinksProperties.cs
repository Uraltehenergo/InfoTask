using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    public partial class FormLinkProperties : Form
    {
        public FormLinkProperties()
        {
            InitializeComponent();
        }

        //Форма установки ссылок
        private FormPutLinks _linksForm;
        //Текущие книга, проект и параметр
        private ReportBook Book { get { return GeneralRep.ActiveBook; } }

        //Получение строки свойств 
        public string PropsString { get { return PropsPanel.PropsString(); } }
        //Ошибка заполнения свойств
        public string PropsError { get { return PropsPanel.LinkPropsError; } }

        //Примечание к ячейке
        public string CellComment
        {
            get { return LinkCellComment.Text; }
            set { LinkCellComment.Text = value; }
        }

        private void FormLinkProperties_Load(object sender, EventArgs e)
        {
            _linksForm = (FormPutLinks)GeneralRep.ActiveBook.Forms[ReporterCommand.PutLinks];
        }

        private void FormLinkProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            Book.CurLinkProps = PropsString;
            _linksForm.PropForm = null;
            GeneralRep.CloseForm(ReporterCommand.LinkProperties);
        }

        private void LinkPropsPanel_OnLinkTypeChange(object sender, EventArgs e)
        {
            try
            {
                Book.GetLinkType(PropsPanel.CellLinkType.Text);
                var lt = Book.CurLinkType.ToLinkType();
                ButNd.Visible = lt == LinkType.Result || lt == LinkType.Absolute || lt == LinkType.AbsoluteEdit || lt == LinkType.Combined || lt == LinkType.CombinedList || lt == LinkType.MomentsList || lt == LinkType.AbsoluteList || lt == LinkType.AbsoluteCombined;
                ButTime.Visible = ButNd.Visible;
                ButNumber.Visible = lt == LinkType.MomentsList || lt == LinkType.CombinedList || lt == LinkType.AbsoluteList;
            }
            catch { }
        }
        
        private void ButValue_Click(object sender, EventArgs e)
        {
            _linksForm.AddLink(LinkField.Value);
        }

        private void ButTime_Click(object sender, EventArgs e)
        {
            _linksForm.AddLink(LinkField.Time);
        }

        private void ButNd_Click(object sender, EventArgs e)
        {
            _linksForm.AddLink(LinkField.Nd);
        }

        private void ButNumber_Click(object sender, EventArgs e)
        {
            _linksForm.AddLink(LinkField.Number);
        }

        private void LinkCellComment_TextChanged(object sender, EventArgs e)
        {
            if (_linksForm.CellComment.Text != CellComment)
                _linksForm.CellComment.Text = CellComment;
        }
    }
}
