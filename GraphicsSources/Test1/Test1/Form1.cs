using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Series1.ChartType = SeriesChartType.FastLine;
            Series1.EmptyPointStyle.Color = Color.Blue;
            Series2.ChartType = SeriesChartType.Line;
            Series2.EmptyPointStyle.Color = Color.Blue;
            //Series2.EmptyPointStyle.BorderWidth = 10;
            for (int i = 0; i < 200; i++)
            {
                Series1.Points.Add(i - i * .9 % 10);
                Series1.Points.Last().IsEmpty = true;
            }
            for (int i = 0; i < 200; i++)
            {
                Series2.Points.Add(i * 1.1 + i * .9 % 10);
                Series2.Points.Last().IsEmpty = true;
            }
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[1].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            chart1.Series.Add(Series1);
            chart1.Series.Add(Series2);
            chart1.Series[1].ChartArea = chart1.ChartAreas[1].Name;

            splitContainer1.Panel1.AutoScroll = true;
            splitContainer1.Panel1.AutoScrollMinSize = new Size(500,500);
            splitContainer1.Panel2.AutoScroll = false;
            Refresh();
            splitContainer1.Panel2.AutoScrollMinSize = new Size(0, 500);
            splitContainer1.Panel1.Scroll += autoscrollingYep;
            splitContainer1.Panel2.AutoScrollPosition = new Point(10, 10);
        }

        private void autoscrollingYep(object sender, EventArgs e)
        {
            splitContainer1.Panel2.VerticalScroll.Value = splitContainer1.Panel1.VerticalScroll.Value;
        }

        internal Series Series1 = new Series();
        internal Series Series2 = new Series();

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            int Count = 10000;
            Random Rnd = new Random();

            int[] Mas_1 = new int[Count];
            int[] Mas_2 = new int[Count];

            for (int i = 0; i < Count; ++i)
            {
                Mas_1[i] = Rnd.Next();
                Mas_2[i] = Rnd.Next();
            }

            progressBar1.Maximum = Count;
            progressBar1.Value = 0;

            Thread t = new Thread(new ThreadStart(delegate
            {
                for (int i = 0; i < Count; ++i)
                {
                    this.Invoke(new ThreadStart(delegate
                    {
                        Text = Mas_1[i].ToString() + " * " +
                                      Mas_2[i].ToString() + " = " +
                                      (Mas_1[i] * Mas_2[i]).ToString();
                        progressBar1.Value++;
                    }));
                }
            }));
            t.Start();
        }

        private void menuStrip1_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("f");
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("g");
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                contextMenuStrip1.Show(pictureBox1, new System.Drawing.Point(0, pictureBox1.Height));
            }
            else
            {
                MessageBox.Show(GetHDDSn());
            }
        }

        private string GetHDDSn()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
            foreach (ManagementObject hdd in searcher.Get())
                if (hdd["SerialNumber"] != null && hdd["SerialNumber"].ToString().Trim() != "")
                {
                    return hdd["SerialNumber"].ToString().Trim();
                }

            return "Null";
        }
    }
}
