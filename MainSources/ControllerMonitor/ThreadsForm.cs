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
    public partial class ThreadsForm : Form
    {
        public ThreadsForm()
        {
            InitializeComponent();
            try
            {
                Threads.Columns["LastTime"].DefaultCellStyle.Format = "dd.MM.yyyy hh:mm:ss";
            }
            catch {}
            timer1.Start();
        }

        //Обновление списка потоков
        internal void ReloadThreads(ThreadHistory errThread) //Поток с последней ошибкой, или null - если нет ошибок
        {
            ErrorTablo.Visible = errThread != null;
            if (errThread != null)
                ErrorTablo.Text = "Поток " + errThread.Id + ", Ошибка в " + errThread.LastTime;
            
            var tab = DataSet1.Tables["ThreadsTable"];
            tab.Rows.Clear();
            foreach (var thread in Program.Threads.Values)
            {
                var row = tab.NewRow();
                row["ThreadId"] = thread.Id;
                row["LastTime"] = thread.LastTime;
                row["LastText"] = thread.LastText;
                tab.Rows.Add(row);
            }
            Threads.Update();
        }

        private void ButSetup_Click(object sender, EventArgs e)
        {
            Program.MonitorHistory.Inf = Program.MonitorHistory.Setup();
            SysTabl.PutValueS(Program.ControllerDataFile, "MonitorHistoryProps", Program.MonitorHistory.Inf);
            Program.LoadThreads();
        }

        private void ButKvit_Click(object sender, EventArgs e)
        {
            Program.CheckTime = DateTime.Now;
            SysTabl.PutValueS(Program.ControllerDataFile, "MonitorHistoryCheckTime", Program.CheckTime.ToString());
            Program.LoadThreads();
        }

        private void ButUpdate_Click(object sender, EventArgs e)
        {
            Program.LoadThreads();
        }

        private void ThreadsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Program.LoadThreads();
        }

        private void ButExit_Click(object sender, EventArgs e)
        {
            if (Different.MessageQuestion("Выйти из приложения?"))
            {
                Close();
                Environment.Exit(0);
            }
        }

        private void ErrorTablo_Click(object sender, EventArgs e)
        {

        }

        private void ThreadsForm_Load(object sender, EventArgs e)
        {
            Program.LoadThreads();
        }

        //Загрузка списка ошибок по одному потоку
        private void LoadErrorsForm()
        {
            if (Threads.CurrentRow != null)
            {
                var form = new ErrorsForm();
                form.Show();
                form.LoadErrors(Program.Threads[Threads.CurrentRow.Get("ThreadId").ToInt()]);
            }
        }

        private void ButThread_Click(object sender, EventArgs e)
        {
            LoadErrorsForm();
        }
        
        private void Threads_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadErrorsForm();
        }
    }
}
