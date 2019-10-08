using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Speed
{
    public partial class Speed : Form
    {
        private Form form;
        private DateTime startPoint = DateTime.Now.AddSeconds(-50000);
        private int dotzCount = 100000;
        private Chart ch1;

        public Speed()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            viewForm();
            Random random = new Random();
            
            ChartArea a1 = new ChartArea();
            a1.Name = "1";
            a1.CursorX.IsUserEnabled = true;
            a1.CursorX.IsUserSelectionEnabled = true;
            a1.CursorX.Interval = .001;
            a1.AxisX.ScaleView.SmallScrollMinSize = .001;
            ch1.ChartAreas.Add(a1);

            Series[] s = new Series[10];
            for (int index = 0; index < s.Length; index++)
            {
                s[index] = new Series
                               {
                                   ChartType = SeriesChartType.FastLine
                               };
                for (int i = 0; i < 50000; i++)
                {
                    s[index].Points.AddXY(startPoint.AddSeconds(i), index*50 + 10* startPoint.AddSeconds(i / 2000).Second + random.Next(30));
                }
                ch1.Series.Add(s[index]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            viewForm();
            Random random = new Random();

            ChartArea[] a = new ChartArea[10];
            for (int index = 0; index < a.Length; index++)
            {
                a[index] = new ChartArea
                               {
                                   Name = index.ToString(),
                                   CursorX = {IsUserEnabled = true, IsUserSelectionEnabled = true, Interval = .001}
                               };
                a[index].AxisX.ScaleView.SmallScrollMinSize = .001;
                ch1.ChartAreas.Add(a[index]);
                a[index].Position.X = 0;
                a[index].Position.Y = 0;
                a[index].Position.Height = 100;
                a[index].Position.Width = 100;
                a[index].BackColor = Color.Transparent;
                a[index].AxisX.ScrollBar.ButtonColor = Color.Violet;
            }

            Series[] s = new Series[10];
            for (int index = 0; index < s.Length; index++)
            {
                s[index] = new Series
                {
                    ChartType = SeriesChartType.FastLine
                };
                for (int i = 0; i < 50000; i++)
                {
                    s[index].Points.AddXY(startPoint.AddSeconds(i), index + 10 * startPoint.AddSeconds(i / 2000).Second + random.Next(30));
                }
                s[index].ChartArea = index.ToString();
                ch1.Series.Add(s[index]);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            viewForm();
            Random random = new Random();

            ChartArea[] a = new ChartArea[10];
            for (int index = 0; index < a.Length; index++)
            {
                a[index] = new ChartArea
                               {
                                   Name = index.ToString(),
                                   CursorX =
                                       {
                                           IsUserEnabled = true, 
                                           IsUserSelectionEnabled = true, 
                                           Interval = .001
                                       }
                               };
                a[index].AxisX.ScaleView.SmallScrollMinSize = .001;
                ch1.ChartAreas.Add(a[index]);
                a[index].Position.X = 0;
                a[index].Position.Y = 0;
                a[index].Position.Height = 100;
                a[index].Position.Width = 100;
            }

            Series[] s = new Series[10];
            for (int index = 0; index < s.Length; index++)
            {
                s[index] = new Series
                               {
                                   ChartType = SeriesChartType.FastLine
                               };
                for (int i = 0; i < 50000; i++)
                {
                    s[index].Points.AddXY(startPoint.AddSeconds(i), index + 10* startPoint.AddSeconds(i / 2000).Second + random.Next(30));
                }
                s[index].ChartArea = index.ToString();
                ch1.Series.Add(s[index]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            viewForm();
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            viewForm();
            
        }

        private void viewForm()
        {

            form = new Form();
            ch1 = new Chart();
            ch1.Name = "ChartArea1";
            ch1.Location = new Point(13, 13);
            ch1.Name = "chart1";
            ch1.Size = new Size(800, 600);
            ch1.TabIndex = 2;
            ch1.Text = "chart1";

            form.Size = new Size(800, 600);
            form.Controls.Add(ch1);
            ch1.Dock = DockStyle.Fill;
            form.Show();

        }
    }
}
