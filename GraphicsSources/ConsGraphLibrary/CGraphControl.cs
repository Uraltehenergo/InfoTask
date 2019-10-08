using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;
using Microsoft.Win32;

namespace ConsGraphLibrary
{
    [ProgId("ActiveXTestLibrary.UserControl")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)] 
    public partial class CGraphControl : UserControl
    {
        public CGraphControl()
        {
            InitializeComponent();
            _separator = _currentCulture.NumberFormat.NumberDecimalSeparator;
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart1.Legends[0].BorderDashStyle = ChartDashStyle.Solid;
            chart1.Legends[0].BorderColor = SystemColors.GradientInactiveCaption;
            chart1.Legends[0].BorderWidth = 3;
            chart1.Legends[0].Alignment = StringAlignment.Center;
        }

        //Строки, выдаваемые наружу при возникновении ошибки
        public string ErrorMessage
        {
            get { return Error == null ? "" : Error.Text; }
        }
        public string ErrorParams
        {
            get { return Error == null ? "" : Error.Exeption.Message + Error.Exeption.StackTrace; }
        }
        internal ErrorCommand Error;

        internal string ConnectionString { get; set; }

        public string SetDatabase(string databaseFile)
        {
            try
            {
                ConnectionString = databaseFile;
                return "";
            }
            catch (Exception exception)
            {
                Error = new ErrorCommand("SetDatabase error", exception);
                return ErrorMessage;
            }
        }

        private GraphicType _graphicType;
        //Набор цветов для графиков
        internal OuterColorUseList Colorz = new OuterColorUseList();
        private int _dim;
        private readonly System.Globalization.CultureInfo _currentCulture =
            System.Globalization.CultureInfo.CurrentCulture;
        private readonly string _separator;
        private TextBox _currentTextBox;
        private string _currentStSQL = "";

        public string LoadList(string stSql)
        {
            try
            {
                tBoxGridX.Text = "";
                tBoxGridY.Text = "";
                tBoxMaxX.Text = "";
                tBoxMaxY.Text = "";
                tBoxMinX.Text = "";
                tBoxMinY.Text = "";

                using (var reader = new ReaderAdo(ConnectionString, stSql))
                {
                    while (reader.Read())
                    {
                        switch (reader.GetString("GraficType"))
                        {
                            case "График":
                                _graphicType = GraphicType.Graphic;
                                break;
                            case "График0":
                                _graphicType = GraphicType.Graphic0;
                                break;
                            case "Диаграмма":
                                _graphicType = GraphicType.Diagram;
                                break;
                        }
                        string uS = "(" +reader.GetString("UnitsX2") + ")";
                        if (uS == "()") uS = "";
                        chart1.ChartAreas[0].AxisX.Title = reader.GetString("NameX2") + uS;
                        uS = "(" + reader.GetString("UnitsX1") + ")";
                        if (uS == "()") uS = "";
                        chart1.ChartAreas[0].AxisY.Title = reader.GetString("NameX1") + uS;
                        chart1.Titles[0].Text = reader.GetString("Code");
                        //_graphicId = int.Parse(reader.GetString("GraficId"));
                        _dim = reader.GetInt("Dimension");
                        chart1.Legends[0].Enabled = _dim >= 3;
                    }
                }
                return "";
            }
            catch (Exception exception)
            {
                Error = new ErrorCommand("Траблы с LoadParams", exception);
                return ErrorMessage;
            }
        }

        public string LoadValues(string stSql, double[] x)
        {
            try
            {
                //обнуляем наш компонент
                foreach (var series in chart1.Series)
                {
                    Colorz.FreeColor(series.Color);
                }
                chart1.Series.Clear();
                //chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                //chart1.ChartAreas[0].AxisX.Interval = double.NaN;
                //chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                //chart1.ChartAreas[0].AxisY.Interval = double.NaN;
                if (x.Count() == 1)
                    if (!checkBoxAutoScale.Checked) checkBoxAutoScale.Checked = true;
                    else ScaleToAuto();

                if (stSql == "") stSql = _currentStSQL;
                else _currentStSQL = stSql;

                using (var reader = new RecDao(ConnectionString, stSql))
                {
                    Cursor = Cursors.WaitCursor;
                        while (reader.Read())
                        {
                            string substLegend = "";//вормируется из таблицы и данного массива
                            string realLegend = "";//вормируется из таблицы
                            //считаем, что в x есть элемент хотя бы один (не имеющий смысла), т.к. пустой массив из аксеса плохо передается
                            for (int i = 1; i <= x.Count() - 1; i++)
                            {
                                realLegend += reader.GetDouble("X" + (_dim - i + 1)) + "-";
                                substLegend += x[i] + "-";
                            }
                            for (int i = 3; i <= _dim - x.Count() + 1; i++)
                            {
                                realLegend += reader.GetDouble("X" + i) + "-";
                                substLegend += reader.GetDouble("X" + i) + "-";
                            }
                            if (realLegend.Equals(substLegend))//фильтр строк
                            {
                                if (substLegend.Length > 0) substLegend = substLegend.Substring(0, substLegend.Length - 1);
                                //Сформировали легенду для конкретной точки. Дальше фасуем ее по сериям

                                bool chart1SeriesContainsLegend = false;
                                foreach (var se in chart1.Series)
                                {
                                    if (se.Name == substLegend) chart1SeriesContainsLegend = true;
                                }
                                if (!chart1SeriesContainsLegend)
                                {
                                    var s = new Series
                                                {
                                                    Name = substLegend,
                                                    BorderWidth = 2,
                                                    MarkerStyle = MarkerStyle.Circle,
                                                    MarkerSize = 10,
                                                    MarkerBorderColor = Color.WhiteSmoke
                                                };
                                    try
                                    {
                                        s.Color = Colorz.GetColor();
                                    }
                                    catch (Exception ex)
                                    {
                                        Error =
                                            new ErrorCommand(
                                                "Не удалось загрузить параметр. Превышено максимальное число параметров (80)",
                                                ex);
                                        return ErrorMessage;
                                    }
                                    switch (_graphicType)
                                    {
                                        case GraphicType.Graphic:
                                            s.ChartType = SeriesChartType.Line;
                                            break;
                                        case GraphicType.Graphic0:
                                            s.ChartType = SeriesChartType.StepLine;
                                            break;
                                        case GraphicType.Diagram:
                                            s.ChartType = SeriesChartType.Point;
                                            break;
                                    }
                                    chart1.Series.Add(s);
                                }
                                if (substLegend == "") substLegend = chart1.Series[0].Name;
                                chart1.Series[substLegend].Points.AddXY(reader.GetDouble("X2"), reader.GetDouble("X1"));
                            }
                        }
                    Cursor = Cursors.Default;
                }
                //chart1.Refresh();
                //if (!checkBoxAutoScale.Checked) checkBoxAutoScale.Checked = true;
                //else ScaleToAuto();
                return "";
            }
            catch (Exception e)
            {
                Cursor = Cursors.Default;
                Error = new ErrorCommand("Проблема с загрузкой значений в параметр (LoadValues)", e);
                //MessageBox.Show(e.Message + "\n===\n" + e.StackTrace);
                return ErrorMessage;
            }
            //throw new Exception();
            //return "";
        }

        public void CloseConnection()
        {
            GC.Collect();
        }

        [ComRegisterFunction()]
        public static void RegisterClass(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open the CLSID\{guid} key for write access  

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            RegistryKey ctrl = k.CreateSubKey("Control");
            ctrl.Close();

            // Next create the CodeBase entry - needed if not string named and GACced.  

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
            inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
            inprocServer32.Close();

            k.Close();
        }

        [ComUnregisterFunction()]
        public static void UnregisterClass(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open HKCR\CLSID\{guid} for write access  

            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // Delete the 'Control' key, but don't throw an exception if it does not exist  
            if (k == null)
            {
                return;
            }
            k.DeleteSubKey("Control", false);

            // Next open up InprocServer32  

            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);

            // And delete the CodeBase key, again not throwing if missing   

            inprocServer32.DeleteSubKey("CodeBase", false);

            // Finally close the main key   

            inprocServer32.Close(); k.Close();
        }

        private enum GraphicType
        {
            Graphic, Graphic0, Diagram
        }

        //включение CursorY
        private void Chart1MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    chart1.ChartAreas[0].CursorY.SelectionStart = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                }
            }
            catch (Exception ex)
            {
                Error = new ErrorCommand("(Chart1MouseDown)", ex);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void Chart1MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    double selectionEnd = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                    chart1.ChartAreas[0].CursorY.SelectionEnd =
                        Math.Abs(selectionEnd - chart1.ChartAreas[0].CursorY.SelectionStart) <
                        .05*(chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum -
                             chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum)
                            ? chart1.ChartAreas[0].CursorY.SelectionStart
                            : selectionEnd;
                    double tmax = Math.Max(chart1.ChartAreas[0].CursorY.SelectionStart, chart1.ChartAreas[0].CursorY.SelectionEnd);
                    double tmin = Math.Min(chart1.ChartAreas[0].CursorY.SelectionStart, chart1.ChartAreas[0].CursorY.SelectionEnd);
                    int tpos = (int)Math.Log10(tmax - tmin) - 2;
                    double tempMax = Math.Pow(10, tpos) * (int)(tmax / Math.Pow(10, tpos));
                    double tempMin = Math.Pow(10, tpos) * (int)(tmin / Math.Pow(10, tpos));
                    if (chart1.ChartAreas[0].CursorY.SelectionEnd != chart1.ChartAreas[0].CursorY.SelectionStart)
                    {
                        chart1.ChartAreas[0].AxisY.ScaleView.Zoom(tempMin, tempMax);
                        tBoxMinY.Text = tempMin.ToString();
                        tBoxMaxY.Text = tempMax.ToString();
                        if (checkBoxAutoScale.Checked) checkBoxAutoScale.Checked = false;
                    }
                    //chart1.ChartAreas[0].CursorY.SelectionStart = double.NaN;
                    chart1.ChartAreas[0].CursorY.SelectionEnd = double.NaN;
                }
            }
            catch (Exception ex)
            {
                Error = new ErrorCommand("(Chart1MouseDown)", ex);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void Chart1MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                double selectionEnd = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                chart1.ChartAreas[0].CursorY.SelectionEnd =
                    Math.Abs(selectionEnd - chart1.ChartAreas[0].CursorY.SelectionStart) <
                    .05 * (chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum -
                         chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum)
                        ? chart1.ChartAreas[0].CursorY.SelectionStart
                        : selectionEnd;
            }
        }

        private void TextBoxInputAnyReal(object sender, KeyPressEventArgs e)
        {
            var textBox = (TextBox)sender;
            _currentTextBox = (TextBox) sender;
            if (char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == (char)Keys.Back) return;
            if (e.KeyChar.ToString() == "-")
            {
                if (textBox.Text != "")
                    if (textBox.Text.IndexOf("-") != -1)
                    {
                        if (textBox.SelectedText.IndexOf("-") == -1) e.Handled = true;
                    }
                    else
                    {
                        if (textBox.SelectionStart != 0) e.Handled = true;
                    }
                return;
            }
            if ((textBox.Text.IndexOf(_separator) == -1) || textBox.SelectedText.IndexOf(_separator) != -1)
                if (e.KeyChar.ToString() == _separator) return;
            if (e.KeyChar == (char)Keys.Enter)
            {
                var tt = (TextBox)sender;
                if (!checkBoxAutoScale.Checked && tt.Text != "")
                {
                    double val = double.Parse(tt.Text);
                    switch (tt.Name)
                    {
                        case "tBoxMinX":
                            //if (val >= chart1.ChartAreas[0].AxisX.Minimum)
                            //    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(val,
                            //                                              chart1.ChartAreas[0].AxisX.ScaleView.
                            //                                                  ViewMaximum);
                            //else tt.Text = chart1.ChartAreas[0].AxisX.Minimum.ToString();
                            if (val < chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum)
                            {
                                double size = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum - val;
                                chart1.ChartAreas[0].AxisX.ScaleView.Position = val;
                                chart1.ChartAreas[0].AxisX.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxMaxX":
                            //if (val <= chart1.ChartAreas[0].AxisX.Maximum)
                            //    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(
                            //        chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum,
                            //        val);
                            //else tt.Text = chart1.ChartAreas[0].AxisX.Maximum.ToString();
                            if (val > chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum)
                            {
                                double size = val - chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisX.ScaleView.Position = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisX.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxMinY":
                            //if (val >= chart1.ChartAreas[0].AxisY.Minimum)
                            //    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(val,
                            //                                              chart1.ChartAreas[0].AxisY.ScaleView.
                            //                                                  ViewMaximum);
                            //else tt.Text = chart1.ChartAreas[0].AxisY.Minimum.ToString();
                            if (val < chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum)
                            {
                                double size = chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum - val;
                                chart1.ChartAreas[0].AxisY.ScaleView.Position = val;
                                chart1.ChartAreas[0].AxisY.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxMaxY":
                            //if (val <= chart1.ChartAreas[0].AxisY.Maximum)
                            //    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(
                            //        chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum,
                            //        val);
                            //else tt.Text = chart1.ChartAreas[0].AxisY.Maximum.ToString();
                            if (val > chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum)
                            {
                                double size = val - chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisY.ScaleView.Position = chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisY.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxGridX":
                            //chart1.ChartAreas[0].AxisX.Interval = (chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum -
                            //                                       chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum)/val;
                            chart1.ChartAreas[0].AxisX.Interval = val;
                            break;
                        case "tBoxGridY":
                            //chart1.ChartAreas[0].AxisY.Interval = (chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum -
                            //                                       chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum)/val;
                            chart1.ChartAreas[0].AxisY.Interval = val;
                            break;
                    }
                }
            }
            e.Handled = true;
        }

        private void Chart1SelectionRangeChanging(object sender, CursorEventArgs e)
        {
            if (e.Axis.AxisName == AxisName.X)
            {
                if (Math.Abs(e.NewSelectionEnd - e.NewSelectionStart) < .03 * (e.ChartArea.AxisX.ScaleView.ViewMaximum -
                                                                             e.ChartArea.AxisX.ScaleView.ViewMinimum)
                    //||(Math.Abs(e.NewSelectionEnd - e.NewSelectionStart) < ScaleXMin)
                    )
                {
                    e.NewSelectionEnd = e.NewSelectionStart;
                }
            }
        }
        
        public void TextBoxLeave()
        {
            if (_currentTextBox != null)
                TextBoxInputAnyReal(_currentTextBox, new KeyPressEventArgs((char)Keys.Enter));
        }
        private void TextBoxLeave(object sender, EventArgs e)
        {
            TextBoxInputAnyReal(sender,new KeyPressEventArgs((char)Keys.Enter));
        }

        public void CheckBoxAutoScaleCheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoScale.Checked)
            {
                ScaleToAuto();
            }
            else
            {
                TextBoxLeave(tBoxGridX, new KeyPressEventArgs((char)Keys.Enter));
                TextBoxLeave(tBoxGridY, new KeyPressEventArgs((char)Keys.Enter));
                TextBoxLeave(tBoxMaxX, new KeyPressEventArgs((char)Keys.Enter));
                TextBoxLeave(tBoxMaxY, new KeyPressEventArgs((char)Keys.Enter));
                TextBoxLeave(tBoxMinX, new KeyPressEventArgs((char)Keys.Enter));
                TextBoxLeave(tBoxMinY, new KeyPressEventArgs((char)Keys.Enter));
            }
        }

        private void ScaleToAuto()
        {
            chart1.ChartAreas[0].AxisX.Interval = double.NaN;
            chart1.ChartAreas[0].AxisY.Interval = double.NaN;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = double.NaN;
            chart1.ChartAreas[0].AxisY.ScaleView.Size = double.NaN;
            chart1.ChartAreas[0].AxisX.ScaleView.Position = double.NaN;
            chart1.ChartAreas[0].AxisY.ScaleView.Position = double.NaN;
            //int tpos = (int)Math.Log10(chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum) - 2;
            //double tmin = (int)((chart1.ChartAreas[0].AxisX.Minimum / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
            //double tsize =
            //    ((int)((chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum) / Math.Pow(10, tpos) + 1)) *
            //    Math.Pow(10, tpos);
            //chart1.ChartAreas[0].AxisX.ScaleView.Position = tmin;
            //chart1.ChartAreas[0].AxisX.ScaleView.Size = tsize;
        }

        private void Chart1AxisViewChanged(object sender, ViewEventArgs e)
        {
            tBoxMaxX.Text = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum.ToString();
            tBoxMinX.Text = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum.ToString();
            if (chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum < chart1.ChartAreas[0].AxisX.Maximum ||
                chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum > chart1.ChartAreas[0].AxisX.Minimum)
                if (checkBoxAutoScale.Checked) checkBoxAutoScale.Checked = false;
        }
    }
}
