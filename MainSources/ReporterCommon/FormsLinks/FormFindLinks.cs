using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Microsoft.Office.Interop.Excel;
using CommonTypes;

namespace ReporterCommon
{
    public partial class FormFindLinks : Form
    {
        public FormFindLinks()
        {
            InitializeComponent();
        }

        public void Find()
        {
            Cells.Rows.Clear();
            foreach (var ws in GeneralRep.ActiveBook.Workbook.GetSheets())
            {
                try
                {
                    foreach (Comment c in ws.Comments)
                    {
                        var dic = c.Text().ToPropertyDictionary();
                        if (dic != null && dic.Get("Project") == Project.Text && dic.Get("Code") == Code.Text && dic.ContainsKey("Field"))
                        {
                            int i = Cells.Rows.Add();
                            var cells = Cells.Rows[i].Cells;
                            cells["Page"].Value = ws.Name;
                            cells["CellAdr"].Value = ((Range)c.Parent).Address;
                            cells["Cell"].Value = cells.Get("CellAdr").Replace("$", "");
                            cells["Field"].Value = dic["Field"].ToLinkField().ToRussian();
                            if (dic.ContainsKey("LinkType"))
                                cells["LinkType"].Value = dic["LinkType"].ToLinkType().ToRussian();
                        }
                    }
                }
                catch { }
            }
        }

        private void FormFindLinksWin_Load(object sender, EventArgs e)
        {
        }

        private void FormFindLinksWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.FindLinks);    
        }

        private void Cells_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Cells_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var cells = Cells.Rows[e.RowIndex].Cells;
                var ws = GeneralRep.ActiveBook.Workbook.Sheets[cells.Get("Page")];
                if (ws is _Worksheet)
                {
                    ((_Worksheet)ws).Activate();
                    ws.Range[cells.Get("CellAdr")].Select();
                }
            }
            catch{}
        }
    }
}
