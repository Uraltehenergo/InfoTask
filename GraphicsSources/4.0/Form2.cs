using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using GraphicLibrary;

namespace _4._0
{
    public partial class Form2 : Form
    {
        //public Form2()
        //{
        //    InitializeComponent();

        //    chart1.Series.Add("Series 2");
        //    chart1.Series[1].ChartType = SeriesChartType.Line;

        //    DateTime time0 = DateTime.Parse("12:03:33.234"); //DateTime.Now;

        //    for (int i = -30; i <= 30; i++)
        //    {
        //        double x = ((double)i / 10);
        //        DateTime time = time0.AddMinutes(x);

        //        chart1.Series[0].Points.AddXY(time, Math.Pow(x, 3));
        //        chart1.Series[1].Points.AddXY(time, -Math.Pow(x, 3));
        //    }

        //    chart1.Series[0].Color = Color.Blue;
        //    chart1.Series[1].Color = Color.Red;

        //    var axX = chart1.ChartAreas[0].AxisX;
        //    axX.IntervalType = DateTimeIntervalType.Minutes;
        //    axX.LabelStyle.Format = "HH':'mm':'ss";
        //    //axX.LabelStyle.Format = "HH':'mm";
        //    axX.Interval = 6.0 / 12;
        //    //double os = 1 - (time0.AddMinutes(-3).Second / 60.0);
        //    //axX.IntervalOffsetType = DateTimeIntervalType.Seconds;
        //    //axX.IntervalOffset = 0; // os;
        //    //axX.Minimum = -3;
        //    //axX.Maximum = 3;

        //    chart1.DataBind();

        //    //t.Area.AxisX.Interval = temp / IntervalCount;
        //    //t.Area.AxisX.ScaleView.Position = standartParam.Area.AxisX.ScaleView.Position;
        //}


        private int _n = 5;
        private int _p = 10;
        private OuterColorUseList _colorz;

        public Form2()
        {
            InitializeComponent();

            comboBox1.Items.Clear();
            comboBox1.Items.Add("1. Одна Area, Одна Serie, Без точек");
            comboBox1.Items.Add("2. Несколько Area и Serie, Без точек");
            comboBox1.Items.Add("3. Несколько Area и Serie, С точками и без");
            comboBox1.Items.Add("4. Границы вне точек");
            comboBox1.Items.Add("5. Точке вне границ");
            comboBox1.Items.Add("6. Если что...");
            comboBox1.Items.Add("7. Несколько Area и Serie, сетка в одном");
            comboBox1.Items.Add("8. 20 Areas и Serie, 50.000 pts");
            comboBox1.Items.Add("9. Закрашенный дискретный график");
            comboBox1.Items.Add("10. 50 Дискретных: 50.000; 1000 pts");
            comboBox1.Items.Add("11. Тест");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _colorz = new OuterColorUseList();
            
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea area;
            Series seria;

            DateTime tm0 = DateTime.Parse("12:00:00");
            DateTime tm1;
            TimeSpan ts;

            double y;
            int l;

            Random rnd;
            
            switch(comboBox1.SelectedIndex)
            {
                case 0:
                    //chart1.Series[0].Name = "Series:";
                    chart1.Series.Add("Serie0");
                    chart1.Series[0].ChartType = SeriesChartType.Line;

                    chart1.ChartAreas.Add("ChartArea0");

                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Minutes;
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH':'mm':'ss";

                    chart1.ChartAreas[0].AxisX.Minimum = 0;
                    chart1.ChartAreas[0].AxisX.Minimum  = 10;
                    
                    break;
                
                case 1:
                    _n = 5;

                    for (int i = 0; i < _n; i++)
                    {
                        chart1.Series.Add("Serie" + i);
                        chart1.Series[i].ChartType = SeriesChartType.Line;

                        chart1.ChartAreas.Add("Area" + i);
                        chart1.Series[i].ChartArea = "Area" + i;
                    }
                    
                    break;

                case 2:
                    _n = 5;

                    for (int i = 0; i < _n; i++)
                    {
                        chart1.ChartAreas.Add("Area" + i);
                        chart1.ChartAreas[i].AxisX.IntervalType = DateTimeIntervalType.Minutes;
                        chart1.ChartAreas[i].AxisX.LabelStyle.Format = "HH':'mm':'ss";
                        chart1.ChartAreas[i].BackColor = Color.Transparent;
                        chart1.ChartAreas[i].AxisY.Minimum = 0;
                        chart1.ChartAreas[i].AxisY.Maximum = 100;

                        chart1.Series.Add("Serie" + i);
                        chart1.Series[i].ChartType = SeriesChartType.Line;
                        chart1.Series[i].Color = _colorz.GetColor();
                        chart1.Series[i].ChartArea = "Area" + i;
                        
                        if (i < _n - 1)
                        {
                            chart1.ChartAreas[i].Position.X = 3;
                            chart1.ChartAreas[i].Position.Y = 3;
                            chart1.ChartAreas[i].Position.Width = 80;
                            chart1.ChartAreas[i].Position.Height = 80;
                        }

                        if(i<_n-1)
                        {
                            for (int j = 0; j < _p; j++)
                            {
                                DateTime x = tm0.AddMinutes(j);
                                y = (i + 1)*j;

                                chart1.Series[i].Points.AddXY(x, y);
                            }
                        }

                        MessageBox.Show(i.ToString());
                    }
                    
                    break;

                case 3:
                    _n = 5;
                    _p = 10;

                    area = chart1.ChartAreas.Add("Area0");
                    AriaPrepare(area, false, tm0.AddMinutes(-60), tm0.AddMinutes(60));

                    seria = chart1.Series.Add("Serie0");
                    seria.ChartType = SeriesChartType.Line;
                    seria.Color = Color.Blue;
                    seria.ChartArea = area.Name;

                    AddPoints(seria, _p, tm0, ELineType.Line, 1);

                    _p = 10;
                    for (int j = 0; j < _p; j++)
                    {
                        DateTime x = tm0.AddMinutes(j);
                        y = j;
                        chart1.Series[0].Points.AddXY(x, y);
                    }

                    break;

                case 4:
                    _n = 5;
                    _p = 10;

                    area = chart1.ChartAreas.Add("Area0");
                    AriaPrepare(area, false, tm0.AddMinutes(5), tm0.AddMinutes(7));

                    seria = chart1.Series.Add("Serie0");
                    seria.ChartType = SeriesChartType.Line;
                    seria.Color = Color.Blue;
                    seria.ChartArea = area.Name;

                    AddPoints(seria, _p, tm0, ELineType.Line, 1);

                    break;

                case 5:
                    MessageBox.Show(":(");
                    break;

                case 6:
                    _n = 5;
                    _p = 10;

                    for (int i = 0; i < _n; i++)
                    {
                        area = chart1.ChartAreas.Add("Area" + i);
                        AriaPrepare(area, i > 0, tm0.AddMinutes(-2), tm0.AddMinutes(_p + 2));

                        seria = chart1.Series.Add("Serie" + i);
                        seria.ChartType = SeriesChartType.Line;
                        seria.Color = _colorz.GetColor();
                        seria.ChartArea = "Area" + i;
                        
                        area.AxisY.Minimum = -5;
                        area.AxisY.Maximum = (i + 1) * _p + 5;

                        AddPoints(seria, _p, tm0, ELineType.Line, i + 1);
                    }

                    break;

                case 7:
                    _n = 20;
                    
                    switch(comboBox2.SelectedIndex)
                    {
                        case 0: _p = 10; break;
                        case 1: _p = 100; break;
                        case 2: _p = 1000; break;
                        case 3: _p = 5000; break;
                        case 4: _p = 10000; break;
                        case 5: _p = 20000; break;
                        case 6: _p = 50000; break;
                        default: _p = 10; break;
                    }

                    tm0 = DateTime.Now;

                    for (int i = 0; i < _n; i++)
                    {
                        area = chart1.ChartAreas.Add("Area" + i);
                        AriaPrepare(area, i > 0, tm0.AddMinutes(-60), tm0.AddMinutes(_p + 60));

                        seria = chart1.Series.Add("Serie" + i);
                        chart1.Series[i].Color = _colorz.GetColor();
                        chart1.Series[i].ChartType = SeriesChartType.FastLine;
                        chart1.Series[i].ChartArea = "Area" + i;

                        AddPoints(seria, _p, tm0, ELineType.Line, i + 1);
                    }

                    tm1 = DateTime.Now;
                    ts = tm1.Subtract(tm0);

                    MessageBox.Show(ts.TotalMilliseconds.ToString());

                    break;

                case 8:
                    _p = 10;

                    chart1.Series.Add("Serie" + 0);
                    chart1.Series[0].Color = _colorz.GetColor();

                    //
                    chart1.Series.Add("Serie" + 1);
                    chart1.Series[1].Color = _colorz.GetColor();
                    //

                    if (comboBox2.SelectedIndex > -1)
                        chart1.Series[0].ChartType = (SeriesChartType) comboBox2.SelectedIndex;
                    else
                        chart1.Series[0].ChartType = SeriesChartType.Point;

                    //
                    chart1.Series[1].ChartType = SeriesChartType.Point;
                    //

                    area = chart1.ChartAreas.Add("Area0");
                    AriaPrepare(area, false, tm0.AddMinutes(-1), tm0.AddMinutes(_p + 1));
                    area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                    area.AxisX.Interval = 1;

                    rnd = new Random();
                    l = rnd.NextDouble() < 0.5 ? 0 : 1;

                    for (int j = 0; j <= _p; j++)
                    {
                        DateTime x = tm0.AddMinutes(j);

                        if (chart1.Series[0].ChartType == SeriesChartType.Area)
                            chart1.Series[0].Points.AddXY(x, l);

                        l = rnd.NextDouble() < 0.5 ? 0 : 1;
                        chart1.Series[0].Points.AddXY(x, l);

                        //
                        chart1.Series[1].Points.AddXY(x, l);
                        //
                    }

                    break;

                case 9:
                    _n = 18;

                    rnd = new Random();
                    
                    switch(comboBox2.SelectedIndex)
                    {
                        case 0: _p = 10; break;
                        case 1: _p = 100; break;
                        case 2: _p = 1000; break;
                        case 3: _p = 5000; break;
                        case 4: _p = 10000; break;
                        case 5: _p = 20000; break;
                        case 6: _p = 50000; break;
                        default: _p = 10; break;
                    }
                    
                    tm0 = DateTime.Now;

                    for (int i = 0; i < _n; i++)
                    {
                        seria = chart1.Series.Add("Serie" + i);
                        seria.Color = _colorz.GetColor();
                        seria.ChartType = SeriesChartType.Area;

                        area = chart1.ChartAreas.Add("Area" + i);
                        AriaPrepare(area, true, tm0, tm0.AddMinutes(_p));
                        chart1.ChartAreas[i].AxisY.Minimum = 0;
                        chart1.ChartAreas[i].AxisY.Maximum = 1;
                        
                        chart1.ChartAreas[i].Position.X = 1;
                        chart1.ChartAreas[i].Position.Y = 90 - 5 * i;
                        chart1.ChartAreas[i].Position.Width = 80;
                        chart1.ChartAreas[i].Position.Height = 10;
                        
                        seria.ChartArea = area.Name;
                        
                        if (i > 0)
                        {
                            chart1.ChartAreas[i].AxisX.LabelStyle.ForeColor = Color.Transparent;
                            //chart1.ChartAreas[i].AxisY.Enabled = AxisEnabled.False;
                        }
                        
                        l = rnd.NextDouble() < 0.5 ? 0 : 1;

                        for (int j = 0; j <= _p; j++)
                        {
                            DateTime x = tm0.AddMinutes(j);

                            chart1.Series[i].Points.AddXY(x, l);
                            l = rnd.NextDouble() < 0.5 ? 0 : 1;
                            chart1.Series[i].Points.AddXY(x, l);
                        }
                    }

                    ts = DateTime.Now.Subtract(tm0);
                    MessageBox.Show(ts.TotalMilliseconds.ToString());

                    break;

                case 10:
                    _n = 20;

                    int k = comboBox2.SelectedIndex;
                    
                    switch(k)
                    {
                        case 0:
                        case 1:
                            _p = 2000;
                            break;
                        case 2: 
                        case 3:
                            _p = 5000;
                            break;
                        case 4:
                        case 5:
                            _p = 20000; 
                            break;
                        default: 
                            _p = 2000; 
                            break;
                    }

                    tm0 = DateTime.Now;
                    TimeSpan ts1 = tm0.Subtract(tm0);

                    var srs = new Series[_n];
                    var ars = new ChartArea[_n];

                    for (int i = 0; i < _n; i++)
                    {
                        switch(k)
                        {
                            case 0:
                            case 2:
                            case 4:
                                srs[i] = chart1.Series.Add("Serie" + i);
                                ars[i] = chart1.ChartAreas.Add("Area" + i);
                                srs[i].ChartArea = "Area" + i;
                                break;

                            case 1:
                            case 3:
                            case 5:
                                srs[i] = new Series("Serie" + i);
                                ars[i] = new ChartArea("Area" + i);
                                break;

                            default:
                                srs[i] = chart1.Series.Add("Serie" + i);
                                ars[i] = chart1.ChartAreas.Add("Area" + i);
                                srs[i].ChartArea = "Area" + i;
                                break;
                        }

                        srs[i].Color = _colorz.GetColor();
                        srs[i].ChartType = SeriesChartType.FastLine;

                        AriaPrepare(ars[i], i > 0, tm0.AddMinutes(-60), tm0.AddMinutes(_p + 60));
                        
                        //srs[i].ChartArea = "Area" + i;

                        ts1 = DateTime.Now.Subtract(tm0);

                        AddPoints(srs[i], _p, tm0, ELineType.Line, i);
                        //AddPoints(srs[i], _p, tm0, ELineType.Sin, i + 1);
                        //AddPoints(srs[i], _p, tm0, ELineType.Random, 1);
                    }
                    
                    switch (k)
                    {
                        case 1:
                        case 3:
                        case 5:
                            for (int i = 0; i < _n; i++)
                            {
                                chart1.Series.Add(srs[i]);
                                chart1.ChartAreas.Add(ars[i]);
                                srs[i].ChartArea = "Area" + i;
                            }

                            break;
                    }

                    tm1 = DateTime.Now;
                    ts = tm1.Subtract(tm0);

                    MessageBox.Show(ts1.TotalMilliseconds + "\n" + ts.TotalMilliseconds);

                    break;
                
                default:
                    MessageBox.Show(":(");
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Visible = false;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    break;
                
                case 1:
                    break;
                
                case 2:
                    break;
                
                case 3:
                    break;
                
                case 4:
                    break;
                
                case 5:
                    break;
                
                case 6:
                    break;
                
                case 7:
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("10 pts;");
                    comboBox2.Items.Add("100 pts;");
                    comboBox2.Items.Add("1000 pts;");
                    comboBox2.Items.Add("2000 pts;");
                    comboBox2.Items.Add("5000 pts;");
                    comboBox2.Items.Add("10000 pts;");
                    comboBox2.Items.Add("50000 pts;");
                    comboBox2.Visible = true;
                    break;

                case 8:
                    comboBox2.Items.Clear();
                    foreach (var value in Enum.GetValues(typeof(SeriesChartType)))
                    {
                        comboBox2.Items.Add(value);
                    }
                    comboBox2.Visible = true;
                    break;
                
                case 9:
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("10 pts;");
                    comboBox2.Items.Add("100 pts;");
                    comboBox2.Items.Add("1000 pts;");
                    comboBox2.Items.Add("2000 pts;");
                    comboBox2.Items.Add("5000 pts;");
                    comboBox2.Items.Add("10000 pts;");
                    comboBox2.Items.Add("50000 pts;");
                    comboBox2.Visible = true;
                    break;
                
                case 10:
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("2000 pts, before");
                    comboBox2.Items.Add("2000 pts, after");
                    comboBox2.Items.Add("5000 pts, before");
                    comboBox2.Items.Add("5000 pts, after");
                    comboBox2.Items.Add("10000 pts, before");
                    comboBox2.Items.Add("10000 pts, after");
                    comboBox2.Visible = true;
                    break;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedIndex)
            {
                case 8:
                    chart1.Series[0].ChartType = (SeriesChartType)comboBox2.SelectedIndex;
                    //MessageBox.Show(chart1.Series[0].ChartType.ToString());
                    break;
            }
        }

        private void AriaPrepare(ChartArea area, bool noGrid, DateTime beginTime, DateTime endTime)
        {
            area.BackColor = Color.Transparent;

            var ts = endTime.Subtract(beginTime);

            if (ts.TotalDays > 1)
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Days;
                area.AxisX.LabelStyle.Format = "dd'.'MM'.'yy HH':'mm";
            }
            else
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                area.AxisX.LabelStyle.Format = "HH':'mm':'ss";
            }

            area.AxisY.LabelStyle.Enabled = false;
            area.AxisY.MajorTickMark.Enabled = false;

            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            //area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            if (noGrid)
            {
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisY.MajorGrid.Enabled = false;
                area.AxisX.LabelStyle.ForeColor = Color.Transparent;
            }

            var min = beginTime.ToOADate();
            var max = endTime.ToOADate();
            area.AxisX.Minimum = min;
            area.AxisX.Maximum = max;

            area.Position.X = 3;
            area.Position.Y = 3;
            area.Position.Width = 80;
            area.Position.Height = 80;
        }

        private void AddPoints(Series seria, int pointCount, DateTime beginTime, ELineType lineType, double a = 1, double b = 0, double c = 1)
        {
            Random rnd = new Random();

            for (int j = 0; j <= pointCount; j++)
            {
                DateTime x = beginTime.AddMinutes(j);
                double y = 0;
                
                switch(lineType)
                {
                    case ELineType.Line :
                        y = a*j + b;
                        break;
                    case ELineType.Sin :
                        y = c*Math.Sin(a*j/30*Math.PI) + b;
                        break;
                    case ELineType.Random:
                        y = rnd.NextDouble();
                        if (a == 0) { y = y < 0.5 ? 0 : 1; }
                        else
                            if (c != 0) y = c*y + b;
                        break;

                }

                seria.Points.AddXY(x, y);
            }
        }
        
        private enum ELineType { Line, Sin, Random }
    }
}
