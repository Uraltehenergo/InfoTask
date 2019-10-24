using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using GraphicLibrary;
using ConsGraphLibrary;
using GraphicLibrary.Params;
using Microsoft.Office.Interop.Access.Dao;

namespace _4._0
{
    public partial class Form1 : Form
    {
        private GraphicForm form;
        private int counter;
        private DateTime startPoint = DateTime.Now.AddSeconds(-100);
        private int dotzCount = 1000;
        public CGraphForm cform;

        public Form1()
        {
            InitializeComponent();

            button1.Text = "Show Form2";
        }

        //Большая кнопка сверху, хз  зачем
        private void button1_Click(object sender, EventArgs e)
        {
            ////form = new GraphicForm();
            ////form.SetTimerMode(0);
            ////form.TimeBegin = startPoint.AddHours(-12.1);
            ////form.TimeEnd = DateTime.Now.AddHours(1500.1);
            //button8.Visible = false;
            //button9.Visible = false;
            ////form.Show();

            //button1.Enabled = false;
            //button1.Text = "You have made your choice! Ha-ha-ha-ha!";
            //button1.BackColor = Color.Red;
            //button2.Visible = true;
            //button3.Visible = true;
            //button5.Visible = true;
            //button7.Visible = true;
            //button6.Visible = true;

            (new Form2()).Show(); 
        }

        //Add new analog graph (Верхняя)
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                bool fg = false;
                if (form == null) fg = true; else if (form.IsDisposed) fg = true;

                if (fg)
                {
                    form = new GraphicForm();
                    form.SetTimerMode(0);
                    //form.TimeBegin = startPoint.AddHours(-12.1);
                    //form.TimeEnd = DateTime.Now.AddHours(400.1);
                    form.TimeBegin = startPoint.AddSeconds(-600);
                    //form.TimeEnd = DateTime.Now.AddSeconds(dotzCount*40);
                    form.TimeEnd = startPoint.AddSeconds(Math.Max(dotzCount * 40, 12 * 60 * 60 - 600));
                    form.SetDecPlacesDefault(1);
                    //form.TimeBegin = startPoint;
                    //form.TimeEnd = startPoint;
                    form.Show();
                }

                Random random = new Random();
                string code = "code" + counter;
                form.AddParam(code, "name" + counter, "subname" + counter, "real",
                              random.NextDouble() * 10, random.NextDouble() * 10 + 60.0, "Калоши");
                GraphicParam temp = form.GetParamByCode(code);
                var m = new MomentReal(startPoint.AddHours(-1), 20.0 / 3, 0);
                //var m = new MomentReal(startPoint, 20.0/3, 0);
                temp.AddValues(m);

                for (int i = 0; i <= 300; /*i++*/ i = i + 25)
                {
                    //var w = new MomentReal(startPoint.AddSeconds(i),
                    //                       13.0 * Math.Sin(Convert.ToDouble(DateTime.Now.AddSeconds(-i).Second / 20.0)), 0);
                    var w = new MomentReal(startPoint.AddSeconds(i),
                                           startPoint.AddSeconds(i / 20).Second, 0);
                    //var w = new MomentReal(startPoint.AddSeconds(i), 60, 0);

                    temp.AddValues(w);
                }
                for (int i = 301; i <= dotzCount; i++)
                {
                    //var w = new MomentReal(startPoint.AddSeconds(i * 30),
                    //                       30 + 30.0 * Math.Sin(Convert.ToDouble(DateTime.Now.AddSeconds(-i).Second / 20.0)), 0);
                    var w = new MomentReal(startPoint.AddSeconds(i * 30),
                                           35 + 25.0 * Math.Sin(Convert.ToDouble(Math.PI * startPoint.AddSeconds(i * 30).Minute / 30.0)), 0);

                    //var w = new MomentReal(startPoint.AddSeconds(20 * i),
                    //                       startPoint.AddSeconds((double)i / 20).Second, 0);
                    //var w = new MomentReal(startPoint.AddSeconds(i), 40, 1);
                    temp.AddValues(w);
                }
                form.DefaultPeriod = 15.ToString();
                form.DefaultScale = "partial";
                form.Repaint(temp);
                counter++;

                //bool fg = false;
                //if (form == null) fg = true; else if (form.IsDisposed) fg = true;

                //if (fg)
                //{
                //    form = new GraphicForm();
                //    form.SetTimerMode(0);
                //    form.TimeBegin = startPoint;
                //    form.TimeEnd = startPoint.AddHours(1);
                //    form.Show();
                //}

                //string code = "code" + counter;
                //form.AddParam(code, "name" + counter, "subname" + counter, "real", 0, 10, "Калоши");
                //var m = new MomentReal(startPoint, 1, 0);
                //form.GetParamByCode(code).AddValues(m);
                //m = new MomentReal(startPoint.AddMinutes(20), 9, 0);
                //form.GetParamByCode(code).AddValues(m);
                //m = new MomentReal(startPoint.AddMinutes(40), 5, 0);
                //form.GetParamByCode(code).AddValues(m);
                //m = new MomentReal(startPoint.AddMinutes(60), 1, 0);
                //form.GetParamByCode(code).AddValues(m);

                //form.Repaint(form.GetParamByCode(code));
                //counter++;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        //Add new analog graph (Нижняя)
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (form == null)
                {
                    form = new GraphicForm();
                    form.SetTimerMode(0);
                    form.TimeBegin = startPoint.AddHours(-12.1);
                    form.TimeEnd = DateTime.Now.AddHours(400.1);
                    form.SetDecPlacesDefault(2);
                    //form.TimeBegin = startPoint;
                    //form.TimeEnd = startPoint;
                    form.Show();
                }
                Random random = new Random();
                string code = "code" + counter;
                form.AddParam(code, "name" + counter, "subname" + counter, "int",
                              random.NextDouble() * 10, random.NextDouble() * 10 + 60.0, "Калоши");
                GraphicParam temp = form.GetParamByCode(code);
                var m = new MomentReal(startPoint.AddHours(-1), 20.0 / 3, 0);
                //var m = new MomentReal(startPoint, 20.0/3, 0);
                temp.AddValues(m);
                for (int i = 0; i <= 300; i++)
                {
                    //var w = new MomentReal(startPoint.AddSeconds(i),
                    //                       13.0 * Math.Sin(Convert.ToDouble(DateTime.Now.AddSeconds(-i).Second / 20.0)), 0);
                    var w = new MomentReal(startPoint.AddSeconds(i),
                                           startPoint.AddSeconds(i / 20).Second, 0);
                    //var w = new MomentReal(startPoint.AddSeconds(i), 60, 0);
                    temp.AddValues(w);
                }
                for (int i = 301; i <= dotzCount; i++)
                {
                    var w = new MomentReal(startPoint.AddSeconds(i * 30),
                                           30 + 30.0 * Math.Sin(Convert.ToDouble(DateTime.Now.AddSeconds(-i).Second / 20.0)), 0);
                    //var w = new MomentReal(startPoint.AddSeconds(20 * i),
                    //                       startPoint.AddSeconds((double)i / 20).Second, 0);
                    //var w = new MomentReal(startPoint.AddSeconds(i), 40, 1);
                    temp.AddValues(w);
                }
                //form.DefaultScale = "part";
                form.Repaint(temp);
                counter++;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        //Annihilate graph
        private void button3_Click(object sender, EventArgs e)
        {
            if (!form.Visible)
            {
                form = new GraphicForm();
                form.SetTimerMode(0);
                form.Show();
            }
            comboBox1.Visible = true;
            comboBox1.Items.Clear();
            for (int j = 1; j <= form.ParamsCount(); j++)
                comboBox1.Items.Add(form.GetParam(j).Code);
            button3.BackColor = Color.Red;
        }

        //Select Graph to annihilate
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button4.Visible = true;
            comboBox1.BackColor = Color.Red;
        }

        //Kill!
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Visible = false;
            comboBox1.Visible = false;
            button3.BackColor = Color.DarkGoldenrod;
            comboBox1.BackColor = Color.DarkGoldenrod;
            form.DeleteParam(comboBox1.Text);
            form.Text = form.ErrorMessage;
            comboBox1.Text = "";
        }

        //Add new dicrete graph (Верхняя)
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                bool fg = false;
                if (form == null) fg = true; else if (form.IsDisposed) fg = true;

                if (fg)
                {
                    form = new GraphicForm();
                    form.SetTimerMode(0);
                    form.TimeBegin = startPoint.AddHours(-12.1);
                    form.TimeEnd = DateTime.Now.AddHours(400.1);
                    form.Show();
                }

                Random random = new Random();
                string code = "code" + counter;
                form.AddParam(code, "name" + counter, "subname" + counter, "bool", 0, 0);
                var m = new MomentReal(startPoint.AddHours(-0.5), 1, 0);
                form.GetParamByCode(code).AddValues(m);
                double r = -100500;
                for (int i = 0; i <= dotzCount * .3; i++)
                {
                    var w = new MomentReal(startPoint.AddSeconds(i * 4000), random.Next() % 2, 0);
                    if (w.Mean != r) form.GetParamByCode(code).AddValues(w);
                    r = w.Mean;
                    //form.GetParamByCode(code).AddValues(w);
                }
                form.Repaint(form.GetParamByCode(code));
                counter++;

                //bool fg = false;
                //if (form == null) fg = true; else if (form.IsDisposed) fg = true;

                //if (fg)
                //{
                //    form = new GraphicForm();
                //    form.SetTimerMode(0);
                //    form.TimeBegin = startPoint;
                //    form.TimeEnd = startPoint.AddHours(1);
                //    form.Show();
                //}

                //string code = "code" + counter;
                //form.AddParam(code, "name" + counter, "subname" + counter, "bool", 0, 1);
                //var m = new MomentReal(startPoint, 1, 0);
                ////form.GetParamByCode(code).AddValues(m);
                ////m = new MomentReal(startPoint.AddMinutes(20), 0, 0);
                ////form.GetParamByCode(code).AddValues(m);
                //m = new MomentReal(startPoint.AddMinutes(40), 1, 0);
                //form.GetParamByCode(code).AddValues(m);
                ////m = new MomentReal(startPoint.AddMinutes(60), 0, 0);
                ////form.GetParamByCode(code).AddValues(m);
                
                //form.Repaint(form.GetParamByCode(code));
                //counter++;
            }
            catch (OverflowException d)
            {
                MessageBox.Show(d.StackTrace);
            }
        }

        //Add new dicrete graph (Нижняя)
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                for (int u = 0; u < 2; u++)
                {
                    if (form == null)
                    {
                        form = new GraphicForm();
                        form.SetTimerMode(0);
                        form.TimeBegin = startPoint.AddHours(-12.1);
                        form.TimeEnd = DateTime.Now.AddHours(400.1);
                        //form.TimeBegin = startPoint;
                        //form.TimeEnd = startPoint;
                        form.Show();
                    }

                    Random random = new Random();
                    string code = "code" + counter;
                    form.AddParam(code, "name" + counter, "subname" + counter, "int",
                                  random.NextDouble()*10, random.NextDouble()*10 + 60.0, "Калоши", 1);
                    GraphicParam temp = form.GetParamByCode(code);
                    var m = new MomentReal(startPoint.AddHours(-1), 20.0/3, 0);
                    //var m = new MomentReal(startPoint, 20.0/3, 0);
                    temp.AddValues(m);
                    for (int i = 0; i <= 300; i++)
                    {
                        //var w = new MomentReal(startPoint.AddSeconds(i),
                        //                       13.0 * Math.Sin(Convert.ToDouble(DateTime.Now.AddSeconds(-i).Second / 20.0)), 0);
                        var w = new MomentReal(startPoint.AddSeconds(i),
                                               startPoint.AddSeconds(i/20).Second, 0);
                        //var w = new MomentReal(startPoint.AddSeconds(i), 60, 0);
                        temp.AddValues(w);
                    }
                    for (int j = 301; j <= dotzCount; j++)
                    {
                        var w = new MomentReal(startPoint.AddSeconds(j*30),
                                               30 +
                                               30.0*Math.Sin(Convert.ToDouble(DateTime.Now.AddSeconds(-j).Second/20.0)),
                                               0);
                        //var w = new MomentReal(startPoint.AddSeconds(20 * i),
                        //                       startPoint.AddSeconds((double)i / 20).Second, 0);
                        //var w = new MomentReal(startPoint.AddSeconds(i), 40, 1);
                        temp.AddValues(w);
                    }
                    //form.DefaultScale = "part";
                    form.Repaint(temp);
                    counter++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        //or maybe...
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                //CGraphic g1 = new CGraphic();
                //g1.ReadGraphic(4, "123", "график");
                //double[]tDoubles = new double[] {1,2,3,4,5};
                //g1.AddPoint(tDoubles);
                //tDoubles[0] = 3;
                //g1.AddPoint(tDoubles);
                //tDoubles[2] = 2;
                //g1.AddPoint(tDoubles);
                //tDoubles[3] = 3;
                //g1.AddPoint(tDoubles);
                //tDoubles[0] = 0;
                //g1.AddPoint(tDoubles);
                Form f1 = new Form();
                var c = new CGraphControl();
                f1.Controls.Add(c);
                f1.Show();
                //c.Dock = DockStyle.Fill;
                c.Height = 100;
                c.Width = 100;
                c.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                c.SetDatabase("D:\\Ktec2_Tep.accdb");
                string sSql =
                    "SELECT  GraficType,NameX1,NameX2,UnitsX1,UnitsX2,Code,GraficId,Dimension FROM GraficsList WHERE GraficId=24";
                c.LoadList(sSql);
                sSql =
                    "SELECT  GraficId,X1,X2,X3,X4,X5,X6,X7,X8 FROM GraficsValues WHERE GraficId=24 ORDER BY X2,X8,X7,X6,X5,X4,X3";
                //DaoDb DaoDb = new DaoDb("D:\\Ktec2_Tep.accdb").ConnectDao();
                //Recordset reader = DaoDb.Database.OpenRecordset(sSql, RecordsetTypeEnum.dbOpenDynaset);
                c.LoadValues(sSql, new double[] {0});

                //cform = new CGraphicForm();
                //cform.LoadGraph(g1);
                //cform.Repaint();
                //cform.Show();
            }
            catch (Exception eu)
            {
                MessageBox.Show(eu.Message);
            }
        }

        //button9
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (form == null)
                {
                    form = new GraphicForm();
                    form.SetTimerMode(3);
                    //form.TimeBegin = startPoint.AddHours(-0.1);
                    //form.TimeEnd = DateTime.Now.AddHours(0.1);
                    //form.TimeBegin = startPoint;
                    //form.TimeEnd = startPoint;
                    form.Show();
                    timer1.Interval = 200;
                    timer1.Start();
                }
                //dotzCount = 45;
                Random random = new Random();
                string code = "code0";
                form.AddParam(code, "name" + counter, "subname" + counter, "int");
                GraphicParam temp = form.GetParamByCode(code);
                //var m = new MomentReal(startPoint.AddHours(-1), 20.0 / 3, 0);
                var m = new MomentReal(DateTime.Now, 20.0 / 3, 0);
                temp.AddValues(m);
                //for (int i = 0; i <= 30; i++)
                //{
                //    var w = new MomentReal(startPoint.AddSeconds(i),
                //                           startPoint.AddSeconds(i / 20).Second, 0);
                //    temp.AddValues(w);
                //}
                
                form.Repaint(temp);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GraphicParam temp1 = form.GetParamByCode("code0");
            Random random = new Random();
            var w = new MomentReal(DateTime.Now, random.NextDouble() * 100, 0);
            if (temp1 != null) temp1.AddValues(w);

            GraphicParam temp2 = form.GetParamByCode("code1");
            w = new MomentReal(DateTime.Now, random.NextDouble() * 100, 0);
            if (temp2 != null) temp2.AddValues(w);

            GraphicParam temp3 = form.GetParamByCode("code2");
            w = new MomentReal(DateTime.Now, random.NextDouble() * 100, 0);
            if (temp3 != null) temp3.AddValues(w);
        }

        //button10
        private void button10_Click(object sender, EventArgs e)
        {
            if (form == null)
            {
                form = new GraphicForm();
                form.SetTimerMode(1.5);
                form.Show();
                timer1.Interval = 200;
                timer1.Start();
            }
            //dotzCount = 45;
            Random random = new Random();
            string code = "code1";
            form.AddParam(code, "name" + counter, "subname" + counter, "int");
            GraphicParam temp = form.GetParamByCode(code);
            //var m = new MomentReal(startPoint.AddHours(-1), 20.0 / 3, 0);
            var m = new MomentReal(DateTime.Now, 26.0 / 3, 0);
            temp.AddValues(m);
            //for (int i = 0; i <= 30; i++)
            //{
            //    var w = new MomentReal(startPoint.AddSeconds(i),
            //                           startPoint.AddSeconds(i / 20).Second, 0);
            //    temp.AddValues(w);
            //}

            form.Repaint(temp);
        }

        //button11
        private void button11_Click(object sender, EventArgs e)
        {
            var m = new MomentReal(DateTime.Now.AddSeconds(-1), 19.0 / 3, 0);
            if (form == null)
            {
                form = new GraphicForm();
                form.SetTimerMode(1.5);
                form.Show();
                timer1.Interval = 201;
                timer1.Start();
            }
            string code = "code2";
            form.AddParam(code, "name" + counter, "subname" + counter, "int");
            GraphicParam temp = form.GetParamByCode(code);

            temp.AddValues(m);

            form.Repaint(temp);
        }

        //some more...
        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                if (form == null)
                {
                    cform = new CGraphForm();
                    //cform.Show();
                }
                cform.SetDatabase("D:\\KosmoTest\\Electro3.accdb");
                string sSql =
                    //"SELECT  GraficType,NameX1,NameX2,NameX3,NameX4,NameX5,NameX6,NameX7,NameX8,Units,UnitsX1,UnitsX2,UnitsX3,UnitsX4,UnitsX5,UnitsX6,UnitsX7,UnitsX8,Code,GraficId,Dimension,Name,MinX,MinY,MaxX,MaxY,MajorX,MajorY,AutoScale FROM GraficsList WHERE GraficId=36";
                    "SELECT  GraficType,NameX1,NameX2,NameX3,NameX4,NameX5,NameX6,NameX7,NameX8,Units,UnitsX1,UnitsX2,UnitsX3,UnitsX4,UnitsX5,UnitsX6,UnitsX7,UnitsX8,Code,GraficId,Dimension,Name,MinX,MinY,MaxX,MaxY,MajorX,MajorY,AutoScale FROM GraficsList WHERE GraficId=3";
                cform.LoadList(sSql);
                sSql = "SELECT  GraficId,X1,X2,X3,X4,X5,X6,X7,X8 FROM GraficsValues WHERE GraficId=3 ORDER BY X2,X8,X7,X6,X5,X4,X3";
                //DaoDb DaoDb = new DaoDb("D:\\Ktec2_Tep.accdb").ConnectDao();
                //Recordset reader = DaoDb.Database.OpenRecordset(sSql, RecordsetTypeEnum.dbOpenDynaset);
                cform.LoadValues(sSql);
                cform.ViewMode = "View";
                cform.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        //button13
        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                //const string code = "3NA11P004";
                //const string dbFile = @"E:\InfoTask\Debug\Analyzer\ArhAnalyzer.accdb";
                //const string archiveFile = @"E:\InfoTask\Debug\Analyzer\ArhAnalyzerArchive.accdb";
                //const int intervalId = 1;
                //const int paramId = 1;

                //form = null;

                //if (form == null)
                //{
                //    form = new GraphicForm();

                //    form.SetDatabase("Access", dbFile);

                //    form.SetTimerMode(0);

                //    var stPoint = new DateTime(2013, 3, 3, 0, 0, 0);
                //    form.TimeBegin = stPoint;
                //    form.TimeEnd = stPoint.AddHours(4);
                //    form.SetDecPlacesDefault(1);
                //    form.Show();
                //}

                //form.AddParam(code, "Р ДО ВЗ А1", "", "real", 0, 400, "кг/см2");
                //GraphicParam temp = form.GetParamByCode(code);
                //temp.Id = paramId;

                //string stSql = "SELECT '" + code + "' AS Code, Null AS Id, Time, Value AS Val, Nd " + "FROM [" +
                //               archiveFile + "].NamedValues WHERE (IntervalId=" + intervalId + ") AND (ParamId=" +
                //               paramId + ") ORDER BY Time;";

                //string errMess = form.LoadValues(stSql);
                //if (errMess != "")
                //{
                //    //errMess = form.ErrorMessage + "\n" + form.ErrorParams;
                //    //MessageBox.Show(errMess);
                //    return;
                //}

                //form.Repaint(temp);
                
                const string dbFile = @"E:\Tmp\Clone.accdb";
                const string archiveFile = @"E:\Tmp\Clone.accdb";
                
                form = null;

                if (form == null)
                {
                    form = new GraphicForm();

                    form.SetDatabase("Access", dbFile);

                    form.SetTimerMode(0);

                    //03.03.2013

                    var stPoint = new DateTime(2013, 3, 3, 0, 0, 0);
                    form.TimeBegin = stPoint;
                    form.TimeEnd = stPoint.AddHours(24);
                    form.SetDecPlacesDefault(1);
                    form.Show();
                }

                int paramId = 41760;
                //string code = "Code" + paramId;
                string code = "3DEHBPWDEM";

                form.AddParam(code, "СИГНАЛ БЛОКА ОГРАНИЧ МОЩНОСТИ", "", "real", 0, 150, "%");
                GraphicParam temp = form.GetParamByCode(code);
                temp.Id = paramId;

                string stSql = "SELECT '" + code + "' AS Code, Null AS Id, [TIMESTAMP] AS [Time], [F_VALUE] AS [Val], 0 AS [Nd] " +
                               "FROM [" + archiveFile + "].[PT_HF_HIST] " +
                               "WHERE (Id=" + paramId + ") " +
                               "ORDER BY [TIMESTAMP];";

                string errMess = form.LoadValues(stSql);
                if (errMess != "") return;

                form.Repaint(temp);

                paramId = 40884;
                //code = "Code" + paramId;
                code = "3DEHYFREQ";
                
                form.AddParam(code, "ОТКЛОНЕНИЕ ЧАСТОТЫ СЕТИ", "", "real", 0, 100);
                temp = form.GetParamByCode(code);
                temp.Id = paramId;

                stSql = "SELECT '" + code + "' AS Code, Null AS Id, [TIMESTAMP] AS [Time], [F_VALUE] AS [Val], 0 AS [Nd] " +
                        "FROM [" + archiveFile + "].[PT_HF_HIST] " +
                        "WHERE (Id=" + paramId + ") " +
                        "ORDER BY [TIMESTAMP];";

                errMess = form.LoadValues(stSql);
                if (errMess != "") return;

                form.Repaint(temp);

                paramId = 40882;
                //code = "Code" + paramId;
                code = "3DEHWSACCEL";
                
                form.AddParam(code, "УСКОРЕНИЕ РОТОРА ТУРБИНЫ", "", "real", 0, 100);
                temp = form.GetParamByCode(code);
                temp.Id = paramId;

                stSql = "SELECT '" + code + "' AS Code, Null AS Id, [TIMESTAMP] AS [Time], [F_VALUE] AS [Val], 0 AS [Nd] " +
                        "FROM [" + archiveFile + "].[PT_HF_HIST] " +
                        "WHERE (Id=" + paramId + ") " +
                        "ORDER BY [TIMESTAMP];";

                errMess = form.LoadValues(stSql);
                if (errMess != "") return;

                form.Repaint(temp);

                paramId = 1304;
                code = "Code" + paramId;
                code = "3NR10A920";
                
                form.AddParam(code, "СОДЕРЖАНИЕ СО", "", "real", 0, 2000, "ppm");
                temp = form.GetParamByCode(code);
                temp.Id = paramId;

                stSql = "SELECT '" + code + "' AS Code, Null AS Id, [TIMESTAMP] AS [Time], [F_VALUE] AS [Val], 0 AS [Nd] " +
                        "FROM [" + archiveFile + "].[PT_HF_HIST] " +
                        "WHERE (Id=" + paramId + ") " +
                        "ORDER BY [TIMESTAMP];";

                errMess = form.LoadValues(stSql);
                if (errMess != "") return;

                form.Repaint(temp);

                paramId = 1302;
                //code = "Code" + paramId;
                code = "3NG40P920";
                
                form.AddParam(code, "P ВОЗДУХА ДО ГОРЕЛОК (ТЫЛ)", "", "real", 0, 250, "кгс/м2");
                temp = form.GetParamByCode(code);
                temp.Id = paramId;

                stSql = "SELECT '" + code + "' AS Code, Null AS Id, [TIMESTAMP] AS [Time], [F_VALUE] AS [Val], 0 AS [Nd] " +
                        "FROM [" + archiveFile + "].[PT_HF_HIST] " +
                        "WHERE (Id=" + paramId + ") " +
                        "ORDER BY [TIMESTAMP];";

                errMess = form.LoadValues(stSql);
                if (errMess != "") return;

                form.Repaint(temp);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        //Test ArchAnalizer
        private void button16_Click(object sender, EventArgs e)
        {
            var formArhAnTest = new ArhAnalyzerTest();
            formArhAnTest.Show();

            //try
            //{
            //    const string dbFile = @"C:\InfoTask\Debug\Analyzer\ArhAnalyzer.accdb";
            //    const string archiveFile = @"C:\InfoTask\Debug\Analyzer\ArhAnalyzerArchive.accdb";
            //    const string projectFile = @"C:\InfoTask\Debug\Analyzer\ArhAnalyzerProject.accdb";
            //    const string task = "ObjectsSet_1";
            //    const byte decPlacesDefault = 4;
            //    //const int intervalId = 1;

            //    DateTime beginTime; //= new DateTime(2010, 6, 1, 0, 0, 0);
            //    DateTime endTime; //= new DateTime(2010, 6, 1, 1, 0, 0);

            //    form = null;

            //    if (form == null)
            //    {
            //        int projectId = -1;
            //        int intervalId = -1;

            //        string stSql = "SELECT ProjectId FROM Projects WHERE (Project='ArhAnalyzer');";
            //        using (var reader = new ReaderAdo(archiveFile, stSql))
            //        {
            //            projectId = reader.GetInt("ProjectId");
            //        }

            //        stSql = "SELECT IntervalId, TimeBegin, TimeEnd FROM NamedIntervals WHERE (IntervalName='" + task + "');";
            //        using (var reader = new ReaderAdo(archiveFile, stSql))
            //        {
            //            intervalId = reader.GetInt("IntervalId");
            //            beginTime = reader.GetTime("TimeBegin");
            //            endTime = reader.GetTime("TimeEnd");
            //        }

            //        form = new GraphicForm();
            //        form.SetDatabase("Access", dbFile);
            //        form.SetTimerMode(0);
            //        form.TimeBegin = beginTime;
            //        form.TimeEnd = endTime;
            //        form.SetDecPlacesDefault(1);
            //        form.Show();

            //        if ((projectId > -1) && (intervalId > -1))
            //        {

            //            stSql = "SELECT * FROM CalcParams WHERE (Task='" + task + "') AND (CalcOtm=True) ORDER BY Code;";
            //            using (var readerParam = new ReaderAdo(projectFile, stSql))
            //            {
            //                while (readerParam.Read())
            //                {
            //                    var code = readerParam.GetString("Code");
            //                    var name = readerParam.GetString("Name");
            //                    var resultType = readerParam.GetString("ResultType");
            //                    var min1 = readerParam.GetDoubleNull("Min");
            //                    double min = min1 ?? 0;
            //                    var max1 = readerParam.GetDoubleNull("Max");
            //                    double max = max1 ?? 0;
            //                    var units = readerParam.GetString("Units");
            //                    var decPlaces1 = readerParam.GetIntNull("DecPlaces");
            //                    int decPlaces = decPlaces1 ?? decPlacesDefault;

            //                    var errMess = form.AddParam(code, name, "", resultType, min, max, units, decPlaces);

            //                    if (errMess != "")
            //                    {
            //                        errMess = form.ErrorMessage + "\n" + form.ErrorParams;
            //                        MessageBox.Show(errMess);
            //                        return;
            //                    }

            //                    GraphicParam temp = form.GetParamByCode(code);

            //                    stSql = "SELECT ParamId FROM Params WHERE (ProjectId=" + projectId + ") AND (Task='" +
            //                            task +
            //                            "') AND (Code='" + temp.Code + "')";

            //                    int paramId = -1;
            //                    using (var reader = new ReaderAdo(archiveFile, stSql))
            //                    {
            //                        paramId = reader.GetInt("ParamId");
            //                        temp.Id = paramId;
            //                    }

            //                    if (paramId > -1)
            //                    {
            //                        stSql = "SELECT '" + temp.Code + "' AS Code, Null AS Id, Time, Value AS Val, Nd " +
            //                                "FROM [" + archiveFile + "].NamedValues WHERE (IntervalId=" + intervalId +
            //                                ") AND (ParamId=" + paramId + ") ORDER BY Time;";

            //                        errMess = form.LoadValues(stSql, beginTime, endTime);
            //                        //errMess = form.LoadValues(stSql);
            //                        if (errMess != "")
            //                        {
            //                            //errMess = form.ErrorMessage + "\n" + form.ErrorParams;
            //                            //MessageBox.Show(errMess);
            //                            return;
            //                        }
            //                    }

            //                    form.Repaint(temp);
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception exception)
            //{
            //    MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            //}
        }

        #region TestConsGraph

        CGraphForm _gr;

        private void butTestConsGraph_Click(object sender, EventArgs e)
        {
            _gr = new CGraphForm();
            _gr.SetDatabase(@"E:\InfoTask\Debug\Projects\GraphicTest.accdb");
            //gr.StartPos(76, 565);
            //gr.Width = 1204;
            //gr.Height = 435;
            _gr.LoadList(@"SELECT GraficType,NameX1,NameX2,NameX3,NameX4,NameX5,NameX6,NameX7,NameX8,Units,UnitsX1,UnitsX2,UnitsX3,UnitsX4,UnitsX5,UnitsX6,UnitsX7,UnitsX8,Code,GraficId,Dimension,Name,MinX,MinY,MaxX,MaxY,MajorX,MajorY,AutoScale FROM GraficsList WHERE GraficId=2");
            _gr.LoadValues(@"SELECT GraficId,X1,X2,X3,X4,X5,X6,X7,X8 FROM GraficsValues WHERE GraficId=2 ORDER BY X2,X8,X7,X6,X5,X4,X3");
            _gr.ViewMode = "Edit";
            _gr.Show();
        }

        private void butLoadValues_Click(object sender, EventArgs e)
        {
            _gr.LoadValues(@"SELECT  GraficId,X1,X2,X3,X4,X5,X6,X7,X8 FROM GraficsValues WHERE GraficId=2 ORDER BY X2,X8,X7,X6,X5,X4,X3");
        }
        #endregion

        #region TestDinGraph

        //private DinGraphicForm _formGr;
        private GraphicForm _formGr;
        private int num;
        private DateTime _time0;

        private void butTestDinGraph_Click(object sender, EventArgs e)
        {
            if (num == 0)
            {
                //_formGr = new DinGraphicForm();
                _formGr = new GraphicForm();
                _time0 = DateTime.Now;
            }

            //график 1
            //fmGraphic.AddGraphic(lvSignalList.SelectedItems[0].SubItems[1].Text);
            //fmGraphic.Text = lvSignalList.SelectedItems[0].SubItems[1].Text ??
            //                 "Модуль " + lvSignalList.SelectedItems[0].SubItems[7].Text + " Канал " +
            //                 lvSignalList.SelectedItems[0].SubItems[8].Text;

            num++;

            _formGr.AddParam("code" + num, "график " + num, "", "real", 0, num + 1, "иксы");
            //var param = new GraphicParam(code, name, "", units, DataType.Real, min, max);
            var param = _formGr.GetParamByCode("code" + num);

            double value = num;
            const int stat = 1;

            var dot = new MomentReal(_time0, value, stat);
            //graphicSeries.IsUpdated = true;
            //graphicSeries.Add(dot);
            //graphicSeries.IsUpdated = false;
            param.AddValues(dot);

            //value = 1 + -1; //Math.Sin(DateTime.Now.Millisecond);
            //dot = new MomentReal(time.AddSeconds(1), value, stat);
            //param.AddValues(dot);

            if (num == 1)
            {
                _formGr.SetTimerMode(3);
                _formGr.SetDecPlacesDefault(1);
                _formGr.Caption = "Мдя...";
                _formGr.Show();
            }

            //_formGr.T1T();
            //formGr.Otladka();
        }

        private void butDinGraphAddValue_Click(object sender, EventArgs e)
        {
            //var param = _formGr.GetParamByCode("code1");
            //DateTime time = DateTime.Now;
            //double value = 1 + Math.Sin(DateTime.Now.Second);
            //int stat = 0;

            //var dot = new MomentReal(time, value, stat);
            //param.AddValues(dot);

            
            //param = _formGr.GetParamByCode("code2");
            //value = 1 + Math.Cos(DateTime.Now.Second);
            //stat = 0;

            //dot = new MomentReal(time, value, stat);
            //param.AddValues(dot);

            for (int i = 1; i <= _formGr.ParamsCount(); i++)
            {
                var param = _formGr.GetParam(i);
                
                DateTime time = DateTime.Now;
                //double p = (double)DateTime.Now.Second/60;
                //p = Math.Sin(p);
                //double value = 0.5*(param.Max + param.Min + (param.Max - param.Min)*p);

                double value;
                if ((param.LastVal() == param.Max) || (param.LastVal() == param.Min))
                    value = 0.5 * (param.Max + param.Min);
                else
                    value = (DateTime.Now.Second < 30) ? param.Min : param.Max;

                const int stat = 0;

                var dot = new MomentReal(time, value, stat);
                param.AddValues(dot);

                //_formGr.T1T();
            }
        }
        #endregion

        private FormGraphic _newForm;
        private void butNewVersion_Click(object sender, EventArgs e)
        {
            bool fg = false;
            if (_newForm == null) fg = true;
            else if (_newForm.IsDisposed) fg = true;

            if (fg)
            {
                DateTime sp = DateTime.Now.AddHours(-1);
                sp = new DateTime(sp.Year, sp.Month, sp.Day, sp.Hour, sp.Minute, 0);

                _newForm = new FormGraphic();
                _newForm.Init(sp, sp.AddHours(1));
                _newForm.Show();
            }
        }

        private void butNewVersionTest_Click(object sender, EventArgs e)
        {
            bool fg = false;
            if (_newForm == null) fg = true;
            else if (_newForm.IsDisposed) fg = true;

            if (fg)
            {
                DateTime sp = DateTime.Now.AddHours(-1);
                sp = new DateTime(sp.Year, sp.Month, sp.Day, sp.Hour, sp.Minute, 0);

                _newForm = new FormGraphic();
                _newForm.Init(sp, sp.AddHours(1));
                _newForm.Show();

                _newForm.AddAnalogGraphic("code1", 1, "Аналог 1", "an", "real", 4, 20.123, "мА");
                _newForm.AddDiscretGraphic("code2", 2, "Дискрет 1", "dig");
                _newForm.AddAnalogGraphic("code3", 3, "Аналог 2", "an", "real", 0, 10.987, "кг", 1);
                _newForm.AddDiscretGraphic("code4", 4, "Дискрет 2", "dig");
                _newForm.AddAnalogGraphic("code5", 5, "Аналог 3", "an", "real", -7, 22.835, "х/з", 2);
                _newForm.AddDiscretGraphic("code6", 6, "Дискрет 3", "dig");

                bool a = true;
                bool b = true;

                for (int i = 1; i <= 6; i++)
                {
                    Graphic gr = _newForm.GetGraphic("code" + i);
                    if (gr.IsAnalog)
                    {
                        for (int j = 0; j <= 60; j++)
                        {
                            double x;
                            //if (a) x = (grA.Param.Max - grA.Param.Min) * j / 60 + grA.Param.Min;
                            //else x = (grA.Param.Min - grA.Param.Max) * j / 60 + grA.Param.Max;
                            //grA.AddValue(sp.AddMinutes(j), x, 0);
                            if (a)
                            {
                                //mm x = (gr.Param.Max + gr.Param.Min) * .5 + (gr.Param.Max - gr.Param.Min) * .5 * Math.Sin(Math.PI * j * ((i + 1) / 2) / 30);
                                x = (gr.MaxViewValue + gr.MinViewValue) * .5 + (gr.MaxViewValue - gr.MinViewValue) * .5 * Math.Sin(Math.PI * j * ((i + 1) / 2) / 30);
                            }
                            else
                            {
                                //mm x = (gr.Param.Max + gr.Param.Min) * .5 + (gr.Param.Max - gr.Param.Min) * .5 * Math.Cos(Math.PI * j * (i / 2) / 30);
                                x = (gr.MaxViewValue + gr.MinViewValue) * .5 + (gr.MaxViewValue - gr.MinViewValue) * .5 * Math.Cos(Math.PI * j * (i / 2) / 30);
                            }

                            if ((i != 1) || ((j != 0) && (j != 60)))
                                gr.AddValue(sp.AddMinutes(j), x, 0);
                        }
                        //gr.ReDrawStep(sp, sp.AddHours(1));
                        _newForm.RepaintGraphic(gr);

                        a = !a;
                    }
                    else
                    {
                        var grD = (DiscretGraphic)gr;
                        for (int j = 0; j <= 60; j++)
                        {
                            bool x = b ? (j % i != 0) : (j % i == 0);
                            grD.AddValue(sp.AddMinutes(j), x, 0);
                        }
                        //grD.ReDrawStep(sp, sp.AddHours(1));
                        _newForm.RepaintGraphic(gr);

                        b = !b;
                    }
                }
            }
        }

        private void butAddAnalog_Click(object sender, EventArgs e)
        {
            bool fg = true;
            if (_newForm == null) fg = false;
            else if (_newForm.IsDisposed) fg = false;

            if (fg)
            {
                int i = _newForm.GraphicCount + 1;
                int k = _newForm.AnalogGraphicCount + 1;

                while(_newForm.GraphicExists("code" + i))
                {
                    i++;
                }
                
                var gr = _newForm.AddAnalogGraphic("code" + i, i, "Аналог " + k, "an", "real", 4, 20, "мА");
                
                bool a = ((_newForm.AnalogGraphicCount%2) != 0);

                //mm double max = gr.Param.Max - 1;
                double max = gr.MaxViewValue - 1;
                //mm double min = gr.Param.Min + 1;
                double min = gr.MinViewValue + 1;

                if (i == 1)
                {
                    //mm max = gr.Param.Max;
                    max = gr.MaxViewValue;
                    //mm min = gr.Param.Min;
                    min = gr.MinViewValue;
                }

                for (int j = -2; j <= 60; j++)
                {
                    double x;
                    if (a)
                    {
                        x = (max + min)*.5 +
                            (max - min)*.5*Math.Sin(Math.PI*j*((i + 1)/2)/30);
                    }
                    else
                    {
                        x = (max + min)*.5 +
                            (max - min)*.5*Math.Cos(Math.PI*j*(i/2)/30);
                    }
                    gr.AddValue(_newForm.TimeBegin.AddMinutes(j), x, 0);
                }
                //gr.ReDrawStep(newForm.TimeBegin, newForm.TimeBegin.AddHours(1));
                _newForm.RepaintGraphic(gr);
            }
        }

        private bool _xF = true;
        private void butAddAnalog1_Click(object sender, EventArgs e)
        {
            bool fg = true;
            if (_newForm == null) fg = false;
            else if (_newForm.IsDisposed) fg = false;

            if (fg)
            {
                int i = _newForm.GraphicCount + 1;
                int k = _newForm.AnalogGraphicCount + 1;

                while (_newForm.GraphicExists("code" + i))
                {
                    i++;
                }

                var gr = _newForm.AddAnalogGraphic("code" + i, i, "Аналог " + k, "an", "real", 0, null, "???");

                bool a = ((_newForm.AnalogGraphicCount % 2) != 0);

                const double max = 15;
                const double min = 3;

                if (!_xF)
                    for (int j = -2; j <= 60; j++)
                    {
                        double x;
                        if (a)
                        {
                            x = (max + min) * .5 +
                                (max - min) * .5 * Math.Sin(Math.PI * j * ((i + 1) / 2) / 30);
                        }
                        else
                        {
                            x = (max + min) * .5 +
                                (max - min) * .5 * Math.Cos(Math.PI * j * (i / 2) / 30);
                        }
                        gr.AddValue(_newForm.TimeBegin.AddMinutes(j), x, 0);
                    }
                else
                {
                    _xF = false;
                    gr.AddValue(_newForm.TimeBegin, 0, 0);
                }

                //gr.ReDrawStep(newForm.TimeBegin, newForm.TimeBegin.AddHours(1));
                _newForm.RepaintGraphic(gr);
            }
        }

        private void butAddDiscret_Click(object sender, EventArgs e)
        {
            bool fg = true;
            if (_newForm == null) fg = false;
            else if (_newForm.IsDisposed) fg = false;

            if (fg)
            {
                int i = _newForm.GraphicCount + 1;
                int k = _newForm.DiscretGraphicCount + 1;

                while (_newForm.GraphicExists("code" + i))
                {
                    i++;
                }

                var gr = _newForm.AddDiscretGraphic("code" + i, i, "Дискрет " + k, "dig");

                bool b = ((_newForm.DiscretGraphicCount % 2) != 0);

                for (int j = 0; j <= 60; j++)
                {
                    bool x = b ? (j % i != 0) : (j % i == 0);
                    gr.AddValue(_newForm.TimeBegin.AddMinutes(j), x, 0);
                }
                //gr.ReDrawStep(newForm.TimeBegin, newForm.TimeBegin.AddHours(1));
                _newForm.RepaintGraphic(gr);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (_newForm != null)
            {
            }
        }

        private void butLoadVedTest_Click(object sender, EventArgs e)
        {
            const string vedFile = @"C:\InfoTask\Debug\AnalyzerInfoTask\Наборы\Гр2\Ved\Н1_Линейная.accdb";

            var testForm = new FormGraphic();
            //testForm.Init(new DateTime(2018, 10, 25), new DateTime(2018, 10, 25, 8, 0, 0));
            string errMes = testForm.InitVed(vedFile);
            if (string.IsNullOrEmpty(errMes))
            {
                testForm.Show();

                //testForm.ViewTimeBegin = new DateTime(2018, 10, 20, 12, 0, 0);
                //testForm.ViewTimeBegin = new DateTime(2018, 10, 22, 12, 0, 0);
                //testForm.ViewTimeEnd = new DateTime(2018, 10, 22, 12, 0, 0);
                testForm.ViewTimeEnd = new DateTime(2018, 10, 20, 12, 0, 0);
            }
            else
                MessageBox.Show(errMes);
        }

        private void butSaveState_Click(object sender, EventArgs e)
        {
            if (_newForm != null)
            {
                var res = _newForm.SaveState(@"C:\InfoTask\Debug\AnalyzerInfoTask\Наборы\Гр2\Ved\Н1_Линейная.accdb");
                if (res != "") MessageBox.Show(res);
            }
            else MessageBox.Show(@"Нетути!");
        }

        private void butLoadState_Click(object sender, EventArgs e)
        {
            bool fg = false;
            if (_newForm == null) fg = true;
            else if (_newForm.IsDisposed) fg = true;

            if (fg) _newForm = new FormGraphic();
            var res = _newForm.LoadState(@"C:\InfoTask\Debug\AnalyzerInfoTask\Наборы\Гр2\Ved\Н1_Линейная.accdb");
            if (res == "")
                _newForm.Show();
            else
                MessageBox.Show(res);
        }

        private void butSetDB_Click(object sender, EventArgs e)
        {
            if (_newForm != null)
            {
                const string dbString = @"C:\InfoTask\Debug\AnalyzerInfoTask\Наборы\Гр2\Ved\Н1_Линейная.accdb";
                _newForm.SetDatabase("Access", dbString);
            }
        }

        private void butTestVizir_Click(object sender, EventArgs e)
        {
            if (_newForm != null)
            {
                MessageBox.Show(_newForm.VizirTime.ToString( ));
            }
            else MessageBox.Show("Нетути!");
        }

        private void butEnableDeleteGraphic_Click(object sender, EventArgs e)
        {
            if (_newForm != null) _newForm.EnableGraphicDelete = !_newForm.EnableGraphicDelete;
        }

        private void butVisibleSaveState_Click(object sender, EventArgs e)
        {
            if (_newForm != null) _newForm.VisibleSaveState = !_newForm.VisibleSaveState;
        }
        
        //private void buton14_Click(object sender, EventArgs e)
        //{
        //    DateTime sp = DateTime.Now.AddMinutes(-10);
        //    sp = new DateTime(sp.Year, sp.Month, sp.Day, sp.Hour, sp.Minute, 0);

        //    form = new GraphicForm();
        //    form.SetTimerMode(0);
        //    form.TimeBegin = sp;
        //    form.TimeEnd = sp.AddMinutes(10);
        //    form.Show();

        //    const string code = "code1";
        //    form.AddParam(code, "name", "subname", "real", 0, 100, "ы");
        //    var m = new MomentReal(startPoint.AddHours(-0.5), 1, 0);

        //    GraphicParam param = form.GetParamByCode(code);

        //    //var w = new MomentReal(sp.AddMinutes(1), 1, 0);
        //    //param.AddValues(w);
        //    //w = new MomentReal(sp.AddMinutes(2), 2, 0);
        //    //param.AddValues(w);
        //    //w = new MomentReal(sp.AddMinutes(4), 4, 1);
        //    //param.AddValues(w);
        //    //w = new MomentReal(sp.AddMinutes(6), 6, 1);
        //    //param.AddValues(w);
        //    //w = new MomentReal(sp.AddMinutes(8), 8, 0);
        //    //param.AddValues(w);
        //    //w = new MomentReal(sp.AddMinutes(9), 9, 0);
        //    //param.AddValues(w);

        //    for (int i = 0; i < 600; i += 10)
        //    {
        //        int j = i - (i / 100) * 100;
        //        if (j <= 50)
        //        {

        //            int nd = (i % 100 == 0) ? 1 : 0;
        //            double x = Math.Pow(i, 2) / 900.0 - (2.0 * i / 3) + 100;
        //            var w = new MomentReal(sp.AddSeconds(i), x, nd);
        //            param.AddValues(w);
        //        }
        //    }

        //    form.Repaint(param);
        //}
    }
}
