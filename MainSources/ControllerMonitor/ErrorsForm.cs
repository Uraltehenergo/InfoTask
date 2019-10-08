using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace ControllerMonitor
{
    public partial class ErrorsForm : Form
    {
        public ErrorsForm()
        {
            InitializeComponent();
            try
            {
                Errors.Columns["Time"].DefaultCellStyle.Format = "dd.MM.yyyy hh:mm:ss";
                Errors.Columns["PeriodBegin"].DefaultCellStyle.Format = "dd.MM.yyyy hh:mm:ss";
                Errors.Columns["PeriodEnd"].DefaultCellStyle.Format = "dd.MM.yyyy hh:mm:ss";
            }
            catch { }
        }

        //Текущий номер потока
        private ThreadHistory _thread;

        //Загрузка списка событий из базы SQL
        internal void LoadErrors(ThreadHistory thread)
        {
            _thread = thread;
            Text = "Поток" + _thread.Id + " : " + _thread.Description + " (" + thread.Projects + ")";
            var tab = DataSet1.Tables["ErrorsTable"];
            tab.Rows.Clear();
            using (var rec = new ReaderAdo(Program.MonitorHistory.SqlProps,
                "SELECT * FROM ErrorsList" + thread.Id + " ORDER BY ErrorsList" + thread.Id + ".Time DESC"))
                while (rec.Read())
                {
                    var row = tab.NewRow();
                    row["Time"] = rec.GetTime("Time");
                    row["Description"] = rec.GetString("Description");
                    row["PeriodBegin"] = rec.GetTime("PeriodBegin");
                    row["PeriodEnd"] = rec.GetTime("PeriodEnd");
                    row["Params"] = rec.GetString("Params");
                    row["Command"] = rec.GetString("Command");
                    row["Context"] = rec.GetString("Context");
                    row["ThreadId"] = rec.GetString("ThreadId");
                    tab.Rows.Add(row);
                }
            Errors.Update();
        }

        private void ButUpdate_Click(object sender, EventArgs e)
        {
            LoadErrors(_thread);
        }

        private void ButDelete_Click(object sender, EventArgs e)
        {
            if (Different.MessageQuestion("Удалить все ошибки потока " + _thread.Id + "?"))
            {
                SqlDb.Execute(Program.MonitorHistory.SqlProps, "TRUNCATE TABLE ErrorsList" + _thread.Id);
                LoadErrors(_thread);
            }
        }
    }
}
