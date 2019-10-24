using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ControllerClient;
using Tablik;

namespace TestTmp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            tbProjectFile.Text = @"D:\Project\InfoTask\Tumen2Bl1Ras\Tumen2Bl1RasProject.accdb";
            
            cbComm.SelectedIndex = 1;
            tbCloneFile.Text = @"C:\InfoTask\Debug\Providers\clone1.accdb";
            dtpTimeBegin.Value = new DateTime(2016, 10, 6, 20, 0, 0);
            dtpTimeEnd.Value = dtpTimeBegin.Value.AddHours(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string appDir = @"C:\InfoTask\Debug\Constructor";
            const string infoTaskDir = @"C:\InfoTask\Debug";
            string pFile = this.tbProjectFile.Text;

            //TablikInit
            TablikCompiler compiller = new TablikCompiler();
            string stParam = appDir + @"\TablikHistory\TablikHistory.accdb";
            string errMess = compiller.SetHistoryFile(stParam);
            stParam = infoTaskDir + @"\Compiled";
            errMess = compiller.SetCompiledDir(stParam);
            
            compiller.LoadProject(pFile);
            compiller.LoadSignals();
            compiller.CompileProject();
            compiller.Close();
        }

        private void butTestCloner_Click(object sender, EventArgs e)
        {
            var commName = cbComm.Text;

            var cloner = new Cloner();
            var errMess = cloner.OpenLocal(commName); //OvationComm //KosmotronikaDbfComm
            if (!string.IsNullOrEmpty(errMess)) MessageBox.Show(errMess);

            cloner.MakeClone(tbCloneFile.Text, dtpTimeBegin.Value, dtpTimeEnd.Value);
            if (!string.IsNullOrEmpty(errMess)) 
                MessageBox.Show(errMess);

            errMess = cloner.Close();
            if (!string.IsNullOrEmpty(errMess)) MessageBox.Show(errMess);
        }

        private void butTestCompile_Click(object sender, EventArgs e)
        {
            TablikCompiler tc = new TablikCompiler();
            var s = tc.LoadProject(this.tbProjectFile.Text);
            if (string.IsNullOrEmpty(s)) s = "OK";
            MessageBox.Show(s);
            
            s  = tc.LoadSignals();
            if (string.IsNullOrEmpty(s)) s = "OK";
            MessageBox.Show(s);
            
            tc.Close();
        }

        private void butChangeProject_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            if (!string.IsNullOrEmpty(this.tbProjectFile.Text))
            {
                int i = this.tbProjectFile.Text.LastIndexOf(@"\");
                if( i>-1)
                {
                    string initDir = this.tbProjectFile.Text.Substring(0, i);
                    fd.InitialDirectory = initDir;
                }
            }
            if (fd.ShowDialog() != DialogResult.Cancel)
                this.tbProjectFile.Text = fd.FileName;
        }

        private void butTestAnalizer_Click(object sender, EventArgs e)
        {
            DateTime timeBegin = new DateTime(2018, 10, 21, 1, 0, 0);
            DateTime timeEnd = new DateTime(2018, 10, 22, 0, 0, 0);
            string curVedFile = @"C:\InfoTask\Debug\AnalyzerInfoTask\Наборы\Гр2\Ved\Н1_Линейная.accdb";
            string curTask = "Линейная";
            int curTrend = 8; //6
            
            Controller controllerObject = new Controller();
            controllerObject.OpenLocal(curTrend, true);

            controllerObject.LoadSetup();

            controllerObject.SetCalcOperations(true, false, false);
            controllerObject.SetDebugOperations(false, false, false, false, false, false);
            String errMess = controllerObject.AnalyzerCalc(timeBegin, timeEnd, curVedFile, curTask);
            if (!string.IsNullOrEmpty(errMess)) MessageBox.Show(errMess);
        }
    }
}
