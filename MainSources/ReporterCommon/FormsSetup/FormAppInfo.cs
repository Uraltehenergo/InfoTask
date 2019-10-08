using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using VersionSynch;

namespace ReporterCommon
{
    public partial class FormAppInfo : Form
    {
        public FormAppInfo()
        {
            InitializeComponent();
        }

        //Отображает данные для указанного типа приложения app = Excel2007 или Excel2010
        public void PutInfo(string app)
        {
            ProgrammInfo.Text = app == "Excel2010" 
                ? "Построитель отчетов для Excel 2010 (ReporterExcel2010)" 
                : "Построитель отчетов для Excel 2007 (ReporterExcel2007)";
            using (var sys = new SysTabl(General.ConfigFile))
            {
                AppVersion.Text = sys.SubValue("InfoTask", "AppVersion");
            }
            var dbv = new DbVersion();
            AppUserOrg.Text = dbv.AUser("Excel");
            LicenseNumber.Text = dbv.ANumber("Excel");
            Refresh();
        }

        private void FormAppInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.AppInfo);
        }

        private void ButBase_Click(object sender, EventArgs e)
        {
            Different.OpenDocument(General.DocsDir + "InfoTask-UG.01-Base.pdf");
        }

        private void ButReporter_Click(object sender, EventArgs e)
        {
            Different.OpenDocument(General.DocsDir + "InfoTask-UG.05-Reporter.pdf");
        }

        private void ButController_Click(object sender, EventArgs e)
        {
            Different.OpenDocument(General.DocsDir + "InfoTask-UG.04-Controller.pdf");
        }
    }
}
