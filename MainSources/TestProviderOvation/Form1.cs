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
using ControllerClient;
using Provider;

namespace TestProviderOvation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var timeBegin = dtTimeBegin.Value;
            //var timeEnd = dtTimeEnd.Value;
            //var cloneFile = tbCloneFile.Text;

            ////MessageBox.Show(timeBegin.ToString() + "\n" + timeEnd.ToString());

            //var cloner = new Cloner();

            //var rez = cloner.OpenLocal("OvationComm");
            //if (!string.IsNullOrEmpty(rez)) MessageBox.Show(rez);

            //rez = cloner.RunSource("OvationSource", "Src", "DataSource=DROP200");
            //if (!string.IsNullOrEmpty(rez)) MessageBox.Show(rez);

            //rez = cloner.MakeClone(cloneFile, timeBegin, timeEnd);
            //if (string.IsNullOrEmpty(rez)) rez = "Сработало!";
            //MessageBox.Show(rez);

            //cloner.Close();

            OvationHistorianSource ohs = new OvationHistorianSource();
            ohs.Name = "OHS";
            ohs.Logger = new ThreadCloner("CommCode", false);
            ohs.Inf = @"DatabaseFile=D:\Project\Clone_GazPlotn_18-12-18.accdb";
            DateTime tBegin = new DateTime(2018, 12, 18, 2, 30, 0);
            DateTime tEnd = new DateTime(2018, 12, 18, 11, 30, 0);
            //ohs.AddSignal("", "3ST40P001", DataType.Real, 4782);
            ohs.AddSignal("CodeObject=3SP00T597.UNIT3@NET0;Id=4718", "3SP00T597", DataType.Real, 4718);
            using (ohs.Logger.Start(0, 100))
                ohs.GetValues(tBegin, tEnd);
            
            MessageBox.Show("C'est tout!");
        }
    }
}
