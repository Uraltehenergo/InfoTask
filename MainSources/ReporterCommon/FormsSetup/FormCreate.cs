using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace ReporterCommon
{
    public partial class FormCreate : Form
    {
        public FormCreate()
        {
            InitializeComponent();
        }

        //True, если файл был выбран при помощи кнопки Обзор
        private bool _fileChecked;

        private void butFileReport_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xlsx",
                Multiselect = false,
                Title = @"Файл отчета",
                Filter = @"Файлы MS Excel (.xlsx) | *.xlsx"
            };
            op.ShowDialog();
            if (!op.FileName.IsEmpty())
            {
                FileReport.Text = op.FileName;
                _fileChecked = true;
            }
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (CodeReport.Text.IsEmpty())
                {
                    Different.MessageError("Не указан код отчета");
                    return;
                }
                if (FileReport.Text.IsEmpty() || (new FileInfo(FileReport.Text).Extension != ".xlsx"))
                {
                    Different.MessageError("Указан недопустимый файл отчета");
                    return;
                }
                using (var rec = new ReaderAdo(General.ReporterFile, "SELECT Report FROM Reports WHERE Report='" + CodeReport.Text + "'"))
                    if (rec.HasRows() && !Different.MessageQuestion("Отчет с кодом " + CodeReport.Text + " уже существует. Заменить на новый?"))
                        return;
                var fnew = new FileInfo(FileReport.Text);
                if (fnew.Exists && !Different.MessageQuestion("Файл " + FileReport.Text + " уже существует. Заменить на новый?"))
                    return;
                try
                {
                    new FileInfo(General.ReporterDir + "ReportTemplate.xlsx").CopyTo(FileReport.Text, true);
                    GeneralRep.Application.Workbooks.Open(FileReport.Text);
                    var sys = new SysPage(GeneralRep.Application.ActiveWorkbook);
                    sys.PutValue("Report", CodeReport.Text);
                    sys.PutValue("ReportName", NameReport.Text);
                    sys.PutValue("ReportDescription", DescriptionReport.Text);
                    sys.PutValue("DifferentLength", "True");
                    sys.PutValue("SaveToArchive", "True");
                    GeneralRep.Application.ActiveWorkbook.Save();
                }
                catch (Exception ex)
                {
                    ex.MessageError("Ошибка создания файла, возможно файл уже существует");
                }
                Close();
                GeneralRep.ChangeActiveBook();
                var f = GeneralRep.RunReporterCommand(ReporterCommand.Setup);
                ((TabControl) f.Controls["tabMain"]).SelectTab("tabReport");
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при создании отчета", ex);
            }
        }

        private void CodeReport_TextChanged(object sender, EventArgs e)
        {
            if (!CodeReport.Text.IsEmpty() && !_fileChecked)
                FileReport.Text = General.InfoTaskDir + @"Reports\" + CodeReport.Text + @".xlsx";
        }

        private void FormCreateWin_Load(object sender, EventArgs e)
        {
        }

        private void FormCreateWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.Create);
        }
    }
}
