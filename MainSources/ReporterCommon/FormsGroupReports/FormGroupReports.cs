using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;

namespace ReporterCommon
{
    public partial class FormGroupReports : Form
    {
        public FormGroupReports()
        {
            InitializeComponent();
        }

        private void FormGroupReportWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            using (var sys = new SysTabl(General.ReporterFile))
            {
                sys.PutSubValue("FormGroupReports", "BeginPeriod", BeginPeriod.Text);
                sys.PutSubValue("FormGroupReports", "EndPeriod", EndPeriod.Text);
            }
            GeneralRep.CloseForm(ReporterCommand.GroupReports);
        }

        private void FormGroupReports_Load(object sender, EventArgs e)
        {
            using (var db = new DaoDb(General.ReporterFile))
            {
                using (var sys = new SysTabl(db))
                {
                    BeginPeriod.Text = sys.SubValue("FormGroupReports", "BeginPeriod");
                    EndPeriod.Text = sys.SubValue("FormGroupReports", "EndPeriod");
                }
                using (var rec = new RecDao(db, "SELECT * FROM GroupReports ORDER BY GroupCode"))
                    while (rec.Read())
                        EditItem(rec);
            }
        }

        //Изменяет элемент в GroupReports из рекордсета, id - ключ элемента, если элемента нет, то элемент добавляется
        public ListViewItem EditItem(IRecordRead rec, int id = 0)
        {
            int key = id == 0 ? rec.GetInt("GroupId") : id;
            var code = rec.GetString("GroupCode");
            var sid = key.ToString();
            ListViewItem item = null;
            if (id == 0) item = GroupReports.Items.Add(sid, code, null);
            else foreach (ListViewItem it in GroupReports.Items) 
                if ((int)it.Tag == key) item = it;
            item.SubItems.Clear();
            item.Tag = key;
            item.Text = code;
            item.SubItems.Add(rec.GetString("GroupName"));
            return item;    
        }

        private void OpenReportGroup()
        {
            if (GroupReports.SelectedItems.Count > 0 && !GeneralRep.CommonBook.Forms.ContainsKey(ReporterCommand.GroupReportEdit))
            {
                var item = GroupReports.SelectedItems[0];
                var f = (FormGroupReportEdit)GeneralRep.RunReporterCommand(ReporterCommand.GroupReportEdit);
                f.LoadGroup((int)item.Tag);
            }
        }

        private void ButEdit_Click(object sender, EventArgs e)
        {
            OpenReportGroup();
        }

        private void GroupReports_DoubleClick(object sender, EventArgs e)
        {
            OpenReportGroup();
        }

        private void ButAdd_Click(object sender, EventArgs e)
        {
            using (var rec = new RecDao(General.ReporterFile, "GroupReports"))
            {
                rec.AddNew();
                rec.Put("GroupCode", "NewGroupReport");
                GroupReports.SelectedItems.Clear();
                var item = EditItem(rec);
                item.Selected = true;
                rec.Update();
                var f = (FormGroupReportEdit)GeneralRep.RunReporterCommand(ReporterCommand.GroupReportEdit);
                f.LoadGroup((int)item.Tag);
            }
        }

        private void ButDelete_Click(object sender, EventArgs e)
        {
            if (GroupReports.SelectedItems.Count > 0)
            {
                var item = GroupReports.SelectedItems[0];
                if (Different.MessageQuestion("Удалить группу отчетов " + item.SubItems[0].Text + "?", "Удаление"))
                {
                    GroupReports.Items.Remove(item);
                    var id = (int)item.Tag;
                    using (var db = new DaoDb(General.ReporterFile))
                    {
                        db.Execute("DELETE * FROM ReportsForGroup WHERE GroupId=" + id);
                        db.Execute("DELETE * FROM GroupReports WHERE GroupId=" + id);
                    }
                }
            }
        }

        private void ButFormReports_Click(object sender, EventArgs e)
        {
            foreach (var item in GroupReports.CheckedItems)
            {
                
            }
        }
    }
}
