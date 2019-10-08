using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using ReporterCommon;
using Microsoft.Office.Tools.Ribbon;

namespace ReporterExcel2007
{
    public partial class ReporterRibbon
    {
        private void ReporterRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void butFormReport_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.Report);
        }

        private void buttonNewReport_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.Create);
        }

        private void buttonSetup_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.Setup);
        }

        private void butAddLink_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.PutLinks);
        }

        private void butDeleteLinks_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.DeleteLinks);
        }

        private void butUpdate_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.Update);
        }
        
        private void Help_Click(object sender, RibbonControlEventArgs e)
        {
            if (GeneralRep.IsActivated)
                ((FormAppInfo)GeneralRep.RunReporterCommand(ReporterCommand.AppInfo)).PutInfo("Excel2007");
        }

        private void butAbsoluteEdit_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.AbsoluteEdit);
        }

        private void ButCopyServerReport_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.CopyServerReport);
        }

        private void butSaveReport_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.SaveReport);
        }

        private void butLoadReport_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.Intervals);
        }
        
        private void butFormLiveReport_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.LiveReport);
        }

        private void ButVideo_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.Video);
        }

        private void ButShapeLibray_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.ShapeLibrary);
        }

        private void ButClearCells_Click(object sender, RibbonControlEventArgs e)
        {
            GeneralRep.RunReporterCommand(ReporterCommand.ClearCells);
        }
    }
}
