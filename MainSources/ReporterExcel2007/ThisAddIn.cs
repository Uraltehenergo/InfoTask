using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ReporterCommon;
using Office = Microsoft.Office.Core;
using Workbook = Microsoft.Office.Interop.Excel.Workbook;

namespace ReporterExcel2007
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            GeneralRep.Initialize(Globals.ThisAddIn.Application, "Excel2007");
            if (GeneralRep.IsActivated)
            {
                GeneralRep.Application.WorkbookOpen += OnWorkbookOpen;
                GeneralRep.Application.WorkbookActivate += OnWorkbookChange;
                GeneralRep.Application.WorkbookBeforeClose += OnWorkbookClose;    
            }
        }

        //Обработка события по изменению текущей книги
        private void OnWorkbookOpen(Workbook workbook)
        {
            GeneralRep.IsOpening = true;
        }

        //Обработка события по изменению текущей книги
        private void OnWorkbookChange(Workbook workbook)
        {
            GeneralRep.ChangeActiveBook();
        }

        //Обработка события по закрытию текущей книги
        private void OnWorkbookClose(Workbook workbook, ref bool flag)
        {
            GeneralRep.CloseBook();
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            GeneralRep.Close();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
