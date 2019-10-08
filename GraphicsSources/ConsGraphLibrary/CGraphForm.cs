using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BaseLibrary;

namespace ConsGraphLibrary
{
    public partial class CGraphForm : Form
    {
        public CGraphForm()
        {
            InitializeComponent();
            _separator = _currentCulture.NumberFormat.NumberDecimalSeparator;
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;

            //dataGridViewValues - таблица значений текущей проекции
            _dataTableOfMine.Columns.Add("X");
            _dataTableOfMine.Columns[0].DataType = typeof(Double);
            _dataTableOfMine.Columns.Add("Y");
            _dataTableOfMine.Columns[1].DataType = typeof(Double);
            dataGridViewValues.DataSource = _dataTableOfMine;
            
            _axisNames.Add(labelNameY);
            _axisNames.Add(labelNameX);
            _axisUnits.Add(labelUnitsY);
            _axisUnits.Add(labelUnitsX);
            for (int i = 2; i < 8; i++)
            {
                _axisLabels.Add(new Label
                                    {
                                        AutoSize = true,
                                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
                                        Left = 16,
                                        Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold,
                                                        GraphicsUnit.Point, ((204))),
                                        Text = "Z" + (i - 1) + ":"
                                    });
                tabPage2.Controls.Add(_axisLabels.Last());
                _axisNames.Add(new Label {AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Left, Left = 53});
                tabPage2.Controls.Add(_axisNames.Last());
                _axisUnits.Add(new Label {AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Left, Left = 42});
                tabPage2.Controls.Add(_axisUnits.Last());
                _axisValues.Add(new Label {AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Left, Left = 42});
                tabPage2.Controls.Add(_axisValues.Last());
            }

            toolTip1.SetToolTip(checkBox1, "Отображение всех/одного графика");
        }
        
        //изменение заголовка формы
        public string Caption
        {
            get { return Text; }
            set { Text = value; }
        }

        private string _units;
        public string Units
        {
            get { return _units; }
            set
            {
                _units = value;
                labelUnitsY.Text = "Ед. изм.: " + value;
                if (string.IsNullOrEmpty(value)) chart1.ChartAreas[0].AxisY.Title = labelNameY.Text;
                //else chart1.ChartAreas[0].AxisY.Title = labelNameY.Text + " (" + value + ")";
                else chart1.ChartAreas[0].AxisY.Title = labelNameY.Text + ", " + value;
            }
        }

        public string Code
        {
            get { return labelNameY.Text; }
            set
            {
                labelNameY.Text = value;
                if (string.IsNullOrEmpty(Units)) chart1.ChartAreas[0].AxisY.Title = labelNameY.Text;
                //else chart1.ChartAreas[0].AxisY.Title = labelNameY.Text + " (" + Units + ")";
                else chart1.ChartAreas[0].AxisY.Title = labelNameY.Text + ", " + Units;
            }
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
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
                return ErrorMessage;
            }
        }

        private GraphicTypes _graphicType;
        //Набор цветов для графиков
        internal OuterColorUseList Colorz = new OuterColorUseList();
        internal int Dim;
        private readonly System.Globalization.CultureInfo _currentCulture =
            System.Globalization.CultureInfo.CurrentCulture;
        private readonly string _separator;
        private TextBox _currentTextBox;
        private string _currentStSQL = "";
        private string _currentGraphic = "NoGraphicsLoaded";
        private string CurrentGraphic
        {
            get
            {
                return _currentGraphic;
            }
            set
            {
                if (checkBox1.CheckState == CheckState.Unchecked)
                {
                    chart1.Series[_currentGraphic].Enabled = false;
                    if (chart1.Series[_currentGraphic].Color == Color.LightGray)
                        chart1.Series[_currentGraphic].Color = Color.Transparent;

                    if (chart1.Series[value].Color == Color.Transparent)
                        chart1.Series[value].Color = Color.LightGray;
                    chart1.Series[value].Enabled = true;
                }

                //Записываем имя (серии) текущего графика
                _currentGraphic = value;
                Series s1 = chart1.Series[_currentGraphic];
                //Заполняем таблицу значений данными серии текущего графика
                _dataTableOfMine.Rows.Clear();
                foreach (DataPoint t in s1.Points)
                {
                    _dataTableOfMine.Rows.Add(t.XValue, t.YValues[0]);
                }
                //Заполняем таблицу осей данными
                string string4split = value + "; ";
                for (int i = Dim; i > 2; i--)
                {
                    int dx = string4split.IndexOf("; ");
                    if (dx != -1)
                    {
                        _axisValues[i - 3].Text = "Значение: " + string4split.Substring(0, dx);
                        string4split = string4split.Substring(dx + 2, string4split.Length - dx - 2);
                    }
                    else
                    {
                        _axisValues[i - 3].Text = "Значение: " + string4split;
                    }
                }

                chart1.Series.Remove(s1);
                chart1.Series.Add(s1);
                //chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = s1.Color;
                //chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = s1.Color;
                dataGridViewValues.GridColor = s1.Color == Color.Transparent ? Color.Black : s1.Color;
            }
        }
        private readonly DataTable _dataTableOfMine = new DataTable();
        private readonly DataPoint _currentPoint = new DataPoint();
        private bool _isRowInAddProcess;
        private string _graphicId;
        //Списки названий осей, единиц измерения и значений по текущим осям
        private List<Label> _axisNames = new List<Label>();
        private List<Label> _axisUnits = new List<Label>();
        private List<Label> _axisValues = new List<Label>();
        private List<Label> _axisLabels = new List<Label>();
        private bool _doWeNeedToSaveChanges;
        //В каком режиме мы работаем: правки или просмотра
        private enum ViewModes{View, Edit}
        private ViewModes _viewMode;
        public string ViewMode
        {
            get { return _viewMode.ToString(); }
            set
            {
                switch (value)
                {
                    case "View":
                        _viewMode = ViewModes.View;
                        buttonAddPlane.Visible = false;
                        buttonRemovePlane.Visible = false;
                        dataGridViewValues.ReadOnly = true;
                        buttonSave.Text = "Редактировать";
                        //Visible = false;
                        //Show();
                        break;
                    case "Edit":
                        _viewMode = ViewModes.Edit;
                        buttonAddPlane.Visible = true;
                        buttonRemovePlane.Visible = true;
                        dataGridViewValues.ReadOnly = false;
                        buttonSave.Text = "Сохранить и закрыть";
                        //Visible = false;
                        //ShowDialog();
                        break;
                }
            }
        }

        public string GraphicType
        {
            get { return _graphicType.ToString(); }
            set
            {
                switch (value)
                {
                    case "График":
                        _graphicType = GraphicTypes.Graphic;
                        foreach (var s in chart1.Series)
                        {
                            s.ChartType = SeriesChartType.Line;
                        }
                        break;
                    case "График0":
                        _graphicType = GraphicTypes.Graphic0;
                        foreach (var s in chart1.Series)
                        {
                            s.ChartType = SeriesChartType.StepLine;
                        }
                        break;
                    case "Диаграмма":
                        _graphicType = GraphicTypes.Diagram;
                        foreach (var s in chart1.Series)
                        {
                            s.ChartType = SeriesChartType.Point;
                        }
                        break;
                }
            }
        }
        private bool _doWeNeedToFillXTextBoxesByChartValues;
        private bool _doWeNeedToFillYTextBoxesByChartValues;

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
                    //while (reader.Read())
                    if (reader.Read())
                    {
                        _graphicId = reader.GetString("GraficId");
                        //Определяем тип графика
                        switch (reader.GetString("GraficType"))
                        {
                            case "График":
                                _graphicType = GraphicTypes.Graphic;
                                break;
                            case "График0":
                                _graphicType = GraphicTypes.Graphic0;
                                break;
                            case "Диаграмма":
                                _graphicType = GraphicTypes.Diagram;
                                break;
                        }
                        //Подписываем оси и сам график
                        string uS = "(" + reader.GetString("UnitsX2") + ")";
                        if (uS == "()") uS = "";
                        chart1.ChartAreas[0].AxisX.Title = reader.GetString("NameX2") + uS;
                        Units = reader.GetString("Units");
                        Code = reader.GetString("Code");
                        Text = reader.GetString("Code") + " (" + reader.GetString("Name") + ")";

                        Dim = reader.GetInt("Dimension");
                        for (int i = 8; i > Dim; i--)
                        {
                            _axisLabels[i - 3].Visible = false;
                            _axisNames[i - 1].Visible = false;
                            _axisUnits[i - 1].Visible = false;
                            _axisValues[i - 3].Visible = false;
                        }
                        for (int i = 2; i <= Dim; i++)
                        {
                            _axisNames[i - 1].Visible = true;
                            _axisNames[i - 1].Text = reader.GetString("NameX" + i);
                            _axisUnits[i - 1].Visible = true;
                            _axisUnits[i - 1].Text = "Ед. изм.: " + reader.GetString("UnitsX" + i);
                        }
                        _axisUnits[0].Text = "Ед. изм.: " + reader.GetString("Units");

                        dataGridViewLegend.Rows.Clear();
                        checkBox1.Checked = true;

                        //Настраиваем правую панель
                        tBoxMaxX.Text = reader.GetString("MaxX");
                        tBoxMinX.Text = reader.GetString("MinX");
                        tBoxMaxY.Text = reader.GetString("MaxY");
                        tBoxMinY.Text = reader.GetString("MinY");
                        tBoxGridX.Text = reader.GetString("MajorX");
                        tBoxGridY.Text = reader.GetString("MajorY");
                        checkBoxAutoScaleX.Checked = reader.GetBool("AutoScale");
                        checkBoxAutoScaleY.Checked = checkBoxAutoScaleX.Checked;

                        checkBoxAutoScaleX.Top = _axisNames[1].Bottom + 2;
                        _axisUnits[1].Top = checkBoxAutoScaleX.Top + 1;
                        tBoxMinX.Top = _axisUnits[1].Top + 17;
                        labelMinX.Top = tBoxMinX.Top + 6;
                        tBoxMaxX.Top = tBoxMinX.Top + 22;
                        labelMaxX.Top = tBoxMaxX.Top + 6;
                        tBoxGridX.Top = tBoxMaxX.Top + 22;
                        labelStepX.Top = tBoxGridX.Top + 6;
                        buttonDefaultX.Top = tBoxGridX.Top + 21;

                        labelUnitsY.Top = buttonDefaultX.Bottom + 4;
                        _axisNames[0].Top = labelUnitsY.Top + 1;
                        labelAxY.Top = labelUnitsY.Top + 1;
                        checkBoxAutoScaleY.Top = _axisNames[0].Bottom + 2;
                        _axisUnits[0].Top = checkBoxAutoScaleY.Top + 1;
                        tBoxMinY.Top = _axisUnits[0].Top + 17;
                        labelMinY.Top = tBoxMinY.Top + 6;
                        tBoxMaxY.Top = tBoxMinY.Top + 22;
                        labelMaxY.Top = tBoxMaxY.Top + 6;
                        tBoxGridY.Top = tBoxMaxY.Top + 22;
                        labelStepY.Top = tBoxGridY.Top + 6;
                        buttonDefaultY.Top = tBoxGridY.Top + 21;
                        buttonSaveScale.Top = buttonDefaultY.Top + 23;

                        if (Dim > 2)
                        {
                            _axisLabels[0].Top = buttonSaveScale.Bottom + 4;
                            _axisLabels[0].Visible = true;
                            for (int i = 2; i < Dim - 1; i++)
                            {
                                _axisNames[i].Top = _axisLabels[i - 2].Top + 1;
                                _axisUnits[i].Top = _axisNames[i].Bottom + 1;
                                _axisValues[i - 2].Top = _axisUnits[i].Bottom + 1;
                                _axisValues[i - 2].Visible = true;
                                _axisLabels[i - 1].Top = _axisValues[i - 2].Bottom + 5;
                                _axisLabels[i - 1].Visible = true;
                            }
                            _axisNames[Dim - 1].Top = _axisLabels[Dim - 3].Top + 1;
                            _axisUnits[Dim - 1].Top = _axisNames[Dim - 1].Bottom + 1;
                            _axisValues[Dim - 3].Top = _axisUnits[Dim - 1].Bottom + 1;
                            _axisValues[Dim - 3].Visible = true;

                            labelDim.Text = "Разм.: " + Dim;
                        }
                    }
                }
                return "";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
                Error = new ErrorCommand("Траблы с LoadParams", exception);
                return ErrorMessage;
            }
        }

        public string LoadValues(string stSql)
        {
            try
            {
                var x = new double[]{0};
                //обнуляем наш график
                foreach (var series in chart1.Series)
                {
                    Colorz.FreeColor(series.Color);
                }
                chart1.Series.Clear();
                //chart1.Refresh();
                
                if (stSql == "") stSql = _currentStSQL;
                else _currentStSQL = stSql;

                using (var recDao = new RecDao(ConnectionString, stSql))
                {
                    Cursor = Cursors.WaitCursor;
                    bool graphicNumberOverflaw = false;
                    while (recDao.Read())
                    {
                        string substLegend = "";//формируется из таблицы и данного массива
                        string realLegend = "";//формируется из таблицы
                        //считаем, что в x есть элемент хотя бы один (не имеющий смысла), т.к. пустой массив из аксеса плохо передается
                        for (int i = 1; i <= x.Count() - 1; i++)
                        {
                            realLegend += recDao.GetDouble("X" + (Dim - i + 1)) + "; ";
                            substLegend += x[i] + "; ";
                        }
                        for (int i = 3; i <= Dim - x.Count() + 1; i++)
                        {
                            realLegend += recDao.GetDouble("X" + i) + "; ";
                            substLegend += recDao.GetDouble("X" + i) + "; ";
                        }
                        if (realLegend.Equals(substLegend))//фильтр строк //отжил свое, надо убирать
                        {
                            if (substLegend.Length > 0) substLegend = substLegend.Substring(0, substLegend.Length - 2);
                            //Сформировали легенду для конкретной точки. Дальше фасуем ее по сериям

                            bool chart1SeriesContainsLegend = false;
                            foreach (var se in chart1.Series)
                            {
                                if (se.Name == substLegend) chart1SeriesContainsLegend = true;
                            }
                            if (!chart1SeriesContainsLegend && (Dim > 2 || chart1.Series.Count == 0))
                            {
                                var s = new Series
                                {
                                    Name = substLegend,
                                    BorderWidth = 2,
                                    MarkerStyle = MarkerStyle.Circle,
                                    MarkerSize = 10,
                                    //MarkerBorderColor = !graphicNumberOverflaw ? Color.WhiteSmoke : Color.Transparent
                                    MarkerBorderColor = Color.WhiteSmoke
                                };
                                try
                                {
                                    s.Color = !graphicNumberOverflaw ? Colorz.GetColor() : Color.Transparent;
                                }
                                catch (Exception ex)
                                {
                                    Error =
                                        new ErrorCommand(
                                            "Не удалось загрузить параметр. Превышено максимальное число параметров (80)", ex);
                                    graphicNumberOverflaw = true;
                                    s.Color = Color.Transparent;
                                    //Cursor = Cursors.Default;
                                    //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                                    //return ErrorMessage;
                                }
                                switch (_graphicType)
                                {
                                    case GraphicTypes.Graphic:
                                        s.ChartType = SeriesChartType.Line;
                                        break;
                                    case GraphicTypes.Graphic0:
                                        s.ChartType = SeriesChartType.StepLine;
                                        break;
                                    case GraphicTypes.Diagram:
                                        s.ChartType = SeriesChartType.Point;
                                        break;
                                }
                                chart1.Series.Add(s);
                                
                                if (Dim > 2)
                                {
                                    int u = chart1.Series.Count;
                                    Image im = new Bitmap(20, 20);
                                    
                                    //~изменено Батьков
                                    //dataGridViewLegend.Rows.Add(im, substLegend);

                                    bool fg = true;
                                    foreach (DataGridViewRow dgvr in dataGridViewLegend.Rows)
                                        if (substLegend == dgvr.Cells[1].Value.ToString()) { fg = false; break; }
                                    if (fg) dataGridViewLegend.Rows.Add(im, substLegend);
                                    //~конец изменено Батьков

                                    Graphics paintin = Graphics.FromImage(im);
                                    var pen = new Pen(s.Color, 2);
                                    Brush brush = new SolidBrush(Color.White);
                                    paintin.FillRectangle(brush, 2, 0, 17, 20);
                                    paintin.DrawLine(pen, 3, 10, 17, 10);
                                    brush = new SolidBrush(s.Color);
                                    paintin.FillEllipse(brush, 6, 6, 7, 7);

                                    //~изменено Батьков
                                    //dataGridViewLegend.Rows[u - 1].Cells[0].Value = im;

                                    ////dataGridViewLegend.Rows[u - 1].Cells[0].Style.SelectionBackColor = Color.White;
                                    ////dataGridViewLegend.Rows[u - 1].Cells[0].Style.BackColor = s.Color;
                                    //dataGridViewLegend.Rows[u - 1].Height = 18;

                                    if (fg)
                                    {
                                        dataGridViewLegend.Rows[u - 1].Cells[0].Value = im;
                                        dataGridViewLegend.Rows[u - 1].Height = 18;
                                    }
                                    //~конец изменено Батьков
                                }
                                //else
                                //{
                                //    panelLegend.Visible = false;
                                //    chart1.Width = splitContainer1.Panel1.Width;
                                //    //chart1.Dock = DockStyle.Fill;
                                //}
                            }
                            if (substLegend == "") substLegend = chart1.Series[0].Name;
                            chart1.Series[substLegend].Points.AddXY(recDao.GetDouble("X2"), recDao.GetDouble("X1"));
                        }
                    }

                    SetLegendWidth();
                    Cursor = Cursors.Default;
                }
                if (chart1.Series.Count > 0)
                {
                    if (dataGridViewLegend.CurrentRow != null)
                        CurrentGraphic = dataGridViewLegend.CurrentRow.Cells[1].Value.ToString();
                    else if (Dim == 2) CurrentGraphic = "Series1";//в данном случае график лишь один, стало быть всегда текущий
                }
                else
                {
                    dataGridViewValues.GridColor = SystemColors.ControlDark;
                    for (; dataGridViewValues.Rows.Count != 1; )
                    {
                        dataGridViewValues.Rows.RemoveAt(0);
                    }
                }
                _isRowInAddProcess = false;
                if (!checkBoxAutoScaleX.Checked)
                {
                    if (tBoxMaxX.Text == tBoxMinX.Text)
                    {
                        checkBoxAutoScaleX.Checked = true;
                        _doWeNeedToFillXTextBoxesByChartValues = true;
                    }
                    else
                    {
                        ApplyXScale();
                    }
                }
                if (!checkBoxAutoScaleY.Checked)
                {
                    if (tBoxMaxY.Text == tBoxMinY.Text)
                    {
                        checkBoxAutoScaleY.Checked = true;
                        _doWeNeedToFillYTextBoxesByChartValues = true;
                    }
                    else
                    {
                        ApplyYScale();
                    }
                }
                return ErrorMessage;
            }
            catch (Exception e)
            {
                Cursor = Cursors.Default;
                Error = new ErrorCommand("Проблема с загрузкой значений в параметр (LoadValues)", e);
                //MessageBox.Show(e.Message + "\n===\n" + e.StackTrace);
                return ErrorMessage;
            }
        }

        public void CloseConnection()
        {
            GC.Collect();
        }

        private enum GraphicTypes
        {
            Graphic, Graphic0, Diagram
        }
        
        //Изменение масштаба
        private void Chart1AxisViewChanged(object sender, ViewEventArgs e)
        {
            tBoxMaxX.Text = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum.ToString();
            tBoxMinX.Text = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum.ToString();
            if (chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum < chart1.ChartAreas[0].AxisX.Maximum ||
                chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum > chart1.ChartAreas[0].AxisX.Minimum)
                if (checkBoxAutoScaleX.Checked) checkBoxAutoScaleX.Checked = false;
        }

        //Выделение графика по Y
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
        private void Chart1MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int eY = e.Y >= 0 ? e.Y : 0;
                if (eY > chart1.ChartAreas[0].Position.Bottom/100*chart1.Height) eY = (int)(chart1.ChartAreas[0].Position.Bottom/100*chart1.Height);
                double selectionEnd = chart1.ChartAreas[0].AxisY.PixelPositionToValue(eY);
                chart1.ChartAreas[0].CursorY.SelectionEnd =
                    Math.Abs(selectionEnd - chart1.ChartAreas[0].CursorY.SelectionStart) <
                    .05 * (chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum -
                         chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum)
                        ? chart1.ChartAreas[0].CursorY.SelectionStart
                        : selectionEnd;
            }
        }
        private void Chart1MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    int eY = e.Y >= 0 ? e.Y : 0;
                    if (eY > chart1.ChartAreas[0].Position.Bottom / 100 * chart1.Height) eY = (int)(chart1.ChartAreas[0].Position.Bottom / 100 * chart1.Height);
                    chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    double selectionEnd = chart1.ChartAreas[0].AxisY.PixelPositionToValue(eY);
                    chart1.ChartAreas[0].CursorY.SelectionEnd =
                        Math.Abs(selectionEnd - chart1.ChartAreas[0].CursorY.SelectionStart) <
                        .05 * (chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum -
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
                        if (checkBoxAutoScaleY.Checked) checkBoxAutoScaleY.Checked = false;
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
        
        private void CheckBoxAutoScaleXCheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoScaleX.Checked)
            {
                chart1.ChartAreas[0].AxisX.Interval = double.NaN;
                chart1.ChartAreas[0].AxisX.ScaleView.Size = double.NaN;
                chart1.ChartAreas[0].AxisX.ScaleView.Position = double.NaN;
                tBoxMaxX.Enabled = false;
                tBoxMinX.Enabled = false;
                tBoxGridX.Enabled = false;
                buttonDefaultX.Enabled = false;
                buttonSaveScale.Enabled = false;
            }
            else
            {
                chart1.ChartAreas[0].AxisX.Interval = double.NaN;
                //TextBoxLeave(tBoxMaxX, new KeyPressEventArgs((char)Keys.Enter));
                //TextBoxLeave(tBoxMinX, new KeyPressEventArgs((char)Keys.Enter));
                //TextBoxLeave(tBoxGridX, new KeyPressEventArgs((char)Keys.Enter));
                ApplyXScale();
                tBoxMaxX.Enabled = true;
                tBoxMinX.Enabled = true;
                tBoxGridX.Enabled = true;
                buttonDefaultX.Enabled = true;
                if (!checkBoxAutoScaleY.Checked)buttonSaveScale.Enabled = true;
            }
        }

        private void ApplyXScale()
        {
            if (tBoxMaxX.Text!="" && tBoxMinX.Text!="")
            {
                double minX = double.Parse(tBoxMinX.Text);
                double maxX = double.Parse(tBoxMaxX.Text);
                if (maxX >= minX)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.Position = minX;
                    chart1.ChartAreas[0].AxisX.ScaleView.Size = maxX - minX;
                }
            }
            if (tBoxGridX.Text != "") chart1.ChartAreas[0].AxisX.Interval = double.Parse(tBoxGridX.Text);
        }

        private void CheckBoxAutoScaleYCheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoScaleY.Checked)
            {
                chart1.ChartAreas[0].AxisY.Interval = double.NaN;
                //chart1.ChartAreas[0].AxisY.ScaleView.Size = double.NaN;
                //chart1.ChartAreas[0].AxisY.ScaleView.Position = double.NaN;

                chart1.ResetAutoValues();

                double mcY = chart1.ChartAreas[0].AxisY.Minimum;
                double McY = chart1.ChartAreas[0].AxisY.Maximum;
                //double msY = mcY;
                //double MsY = McY;
                //var findMinByValue = chart1.Series[0].Points.FindMinByValue();
                //if (findMinByValue != null) msY = findMinByValue.YValues[0];
                //var findMaxByValue = chart1.Series[0].Points.FindMaxByValue();
                //if (findMaxByValue != null) MsY = findMaxByValue.YValues[0];
                chart1.ChartAreas[0].AxisY.ScaleView.Position = mcY;
                chart1.ChartAreas[0].AxisY.ScaleView.Size = McY - mcY;
                tBoxMaxY.Enabled = false;
                tBoxMinY.Enabled = false;
                tBoxGridY.Enabled = false;
                buttonDefaultY.Enabled = false;
                buttonSaveScale.Enabled = false;
            }
            else
            {
                //TextBoxLeave(tBoxGridY, new KeyPressEventArgs((char)Keys.Enter));
                ////chart1.Refresh();
                //TextBoxLeave(tBoxMaxY, new KeyPressEventArgs((char)Keys.Enter));
                //TextBoxLeave(tBoxMinY, new KeyPressEventArgs((char)Keys.Enter));
                ApplyYScale();
                tBoxMaxY.Enabled = true;
                tBoxMinY.Enabled = true;
                tBoxGridY.Enabled = true;
                buttonDefaultY.Enabled = true;
                if (!checkBoxAutoScaleX.Checked) buttonSaveScale.Enabled = true;
            }
        }

        private void ApplyYScale()
        {
            if (tBoxMaxY.Text != "" && tBoxMinY.Text != "")
            {
                double minY = double.Parse(tBoxMinY.Text);
                double maxY = double.Parse(tBoxMaxY.Text);
                if (maxY >= minY)
                {
                    chart1.ChartAreas[0].AxisY.ScaleView.Position = minY;
                    chart1.ChartAreas[0].AxisY.ScaleView.Size = maxY - minY;
                }
            }
            if (tBoxGridY.Text != "") chart1.ChartAreas[0].AxisY.Interval = double.Parse(tBoxGridY.Text);
        }

        //Защита от дурака на ввод текста и изменение масштаба по нажатию Enter
        private void TextBoxInputAnyReal(object sender, KeyPressEventArgs e)
        {
            var textBox = (TextBox)sender;
            _currentTextBox = (TextBox)sender;
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
                if (tt.Text != "")
                {
                    double val = double.Parse(tt.Text);
                    switch (tt.Name)
                    {
                        case "tBoxMinX":
                            if (!checkBoxAutoScaleX.Checked && val < chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum)
                            {
                                double size = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum - val;
                                chart1.ChartAreas[0].AxisX.ScaleView.Position = val;
                                chart1.ChartAreas[0].AxisX.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxMaxX":
                            if (!checkBoxAutoScaleX.Checked && val > chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum)
                            {
                                double size = val - chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisX.ScaleView.Position = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisX.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxMinY":
                            if (!checkBoxAutoScaleY.Checked && val < chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum)
                            {
                                double size = chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum - val;
                                chart1.ChartAreas[0].AxisY.ScaleView.Position = val;
                                chart1.ChartAreas[0].AxisY.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxMaxY":
                            if (!checkBoxAutoScaleY.Checked && val > chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum)
                            {
                                double size = val - chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisY.ScaleView.Position = chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                                chart1.ChartAreas[0].AxisY.ScaleView.Size = size;
                            }
                            break;
                        case "tBoxGridX":
                            if (!checkBoxAutoScaleX.Checked)
                                chart1.ChartAreas[0].AxisX.Interval = val;
                            break;
                        case "tBoxGridY":
                            if (!checkBoxAutoScaleY.Checked)
                                chart1.ChartAreas[0].AxisY.Interval = val;
                            break;
                    }
                }
            }
            e.Handled = true;
        }

        //Минимум приближения по Х
        private void Chart1SelectionRangeChanging(object sender, CursorEventArgs e)
        {
            if (e.Axis.AxisName == AxisName.X)
            {
                if (Math.Abs(e.NewSelectionEnd - e.NewSelectionStart) < .03 * (e.ChartArea.AxisX.ScaleView.ViewMaximum -
                                                                             e.ChartArea.AxisX.ScaleView.ViewMinimum)
                    )
                {
                    e.NewSelectionEnd = e.NewSelectionStart;
                }
            }
        }

        //изменение масштаба
        private void TextBoxLeave(object sender, EventArgs e)
        {
            TextBoxInputAnyReal(sender, new KeyPressEventArgs((char)Keys.Enter));
        }

        private void CGraphFormFormClosed(object sender, FormClosedEventArgs e)
        {
            CloseConnection();
        }

        //запасной метод
        private void SpareMethod()
        {
            //chart1.BackColor = Color.SteelBlue;
        }

        private void Chart1Click(object sender, EventArgs e)
        {
            SpareMethod();
        }

        private void DataGridViewValuesRowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_tempBool)
            try
            {
                _currentPoint.XValue = (double)dataGridViewValues.Rows[e.RowIndex].Cells[0].Value;
                _currentPoint.YValues[0] = (double)dataGridViewValues.Rows[e.RowIndex].Cells[1].Value;
            }
            catch (Exception)
            {
                //при удалении последней записи в таблице возникает ошибка
            }
        }

        private bool _tempBool = true;

        //Если произошла ошибка при переходе из строки таблицы, изменения ее (таблицы) сбрасываются
        private void DataGridViewValuesRowLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //Временное решение (не исключено, что оптимальное): если фокус теряется не на dataGridViewValues,
                //dataGridViewValues.EndEdit() уводит нас в метод DataGridViewValuesRowEnter, и значение фактически не обновляется
                _tempBool = false;
                dataGridViewValues.EndEdit();
                _tempBool = true;

                var x = (double)dataGridViewValues.Rows[e.RowIndex].Cells[0].Value;//уже исправленное значение x
                var y = (double)dataGridViewValues.Rows[e.RowIndex].Cells[1].Value;//уже исправленное значение y
                if (_isRowInAddProcess)
                {
                    int ind;
                    for (ind = 0; ind < chart1.Series[CurrentGraphic].Points.Count; ind++)
                        if (chart1.Series[CurrentGraphic].Points[ind].XValue > x) break;
                    chart1.Series[CurrentGraphic].Points.InsertXY(ind, x, y);
                    _isRowInAddProcess = false;
                    _doWeNeedToSaveChanges = true;
                }
                else
                {
                    if (x != _currentPoint.XValue)
                    {
                        int ind;
                        for (ind = 0; ind < chart1.Series[CurrentGraphic].Points.Count; ind++)
                            if (chart1.Series[CurrentGraphic].Points[ind].XValue > x) break;
                        chart1.Series[CurrentGraphic].Points.InsertXY(ind, x, y);
                        DataPoint tdp = chart1.Series[CurrentGraphic].Points.FindByValue(_currentPoint.XValue, "X");
                        ind = chart1.Series[CurrentGraphic].Points.IndexOf(tdp);
                        chart1.Series[CurrentGraphic].Points.RemoveAt(ind);
                        _doWeNeedToSaveChanges = true;
                    }
                    else if (y != _currentPoint.YValues[0])
                    {
                        chart1.Series[CurrentGraphic].Points.FindByValue(_currentPoint.XValue, "X").YValues[0] = y;
                        chart1.Refresh();
                        _doWeNeedToSaveChanges = true;
                    }
                }
            }
            catch (Exception)
            {
                dataGridViewValues.CancelEdit();
            }
        }

        //Смена текущего графика
        private void DataGridViewLegendSelectionChanged(object sender, EventArgs e)
        {
            //if (CurrentGraphic != "NoGraphicsLoaded")
            if (dataGridViewLegend.CurrentRow != null)
                    CurrentGraphic = dataGridViewLegend.CurrentRow.Cells[1].Value.ToString();
        }

        private void SaveChanges()
        {
            try
            {
                if (dataGridViewValues.Focused)
                {
                    //DataGridViewValuesRowLeave(dataGridViewValues,
                    //                           new DataGridViewCellEventArgs(dataGridViewValues.CurrentCell.ColumnIndex,
                    //                                                         dataGridViewValues.CurrentCell.RowIndex));
                    //if (dataGridViewValues.CurrentRow != null)
                    //{
                    //    var findByValue = chart1.Series[CurrentGraphic].Points.FindByValue(_currentPoint.XValue, "X");
                    //    if (findByValue == null)
                    //findByValue.YValues[0] = (double)dataGridViewValues.CurrentRow.Cells[1].Value;
                    //dataGridViewValues.UpdateCellValue(dataGridViewValues.CurrentCell.ColumnIndex,
                    //                                                 dataGridViewValues.CurrentCell.RowIndex);
                    //}
                    //chart1.Refresh();
                    _doWeNeedToSaveChanges = true;
                }
                //if (_doWeNeedToSaveChanges)
                using (var db = new DaoDb(ConnectionString))
                {
                    db.Execute("DELETE * FROM GraficsValues WHERE GraficId=" + _graphicId);
                    using (var rec = new RecDao(ConnectionString, "GraficsValues"))
                    {
                        foreach (var s in chart1.Series)
                        {
                            var means = new List<string>();
                            string string4split = s.Name + "; ";
                            for (int i = 0; i < Dim - 2; i++)
                            {
                                int dx = string4split.IndexOf("; ");
                                means.Add(string4split.Substring(0, dx));
                                string4split = string4split.Substring(dx + 2, string4split.Length - dx - 2);
                            }
                            foreach (var p in s.Points)
                            {
                                rec.AddNew();
                                rec.Put("GraficId", _graphicId);
                                for (int i = 0; i < Dim - 2; i++)
                                {
                                    rec.Put("X" + (i + 3), means[i]);
                                }
                                rec.Put("X2", p.XValue);
                                rec.Put("X1", p.YValues[0]);
                            }
                        }
                    }
                }
                _doWeNeedToSaveChanges = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка записи в базу данных");
            }
            finally {Close();}
        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            if (ViewMode == "View")
            {
                ViewMode = "Edit";
                Visible = false;
                //((Button) sender).Text = "Сохранить и закрыть";
                ShowDialog();
            }
            else
            {
                SaveChanges();
            }
        }

        //private void CheckBoxEditModeCheckedChanged(object sender, EventArgs e)
        //{
        //    dataGridViewValues.EndEdit();
        //    if (checkBoxEditMode.Checked)
        //    {
        //        ViewMode = "Edit";
        //        Visible = false;
        //        ShowDialog();
        //    }
        //    else
        //    {
        //        if (_doWeNeedToSaveChanges)
        //        {
        //            DialogResult yResult = MessageBox.Show("Сохранить изменения?", "Закрытие формы",
        //                                                   MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        //            if (yResult == DialogResult.Yes)
        //            {
        //                SaveChanges();
        //                ViewMode = "View";
        //                _doWeNeedToSaveChanges = false;
        //                Visible = false;
        //                DialogResult = DialogResult.None;
        //                Show();
        //            }
        //            else if (yResult == DialogResult.No)
        //            {
        //                ViewMode = "View";
        //                _doWeNeedToSaveChanges = false;
        //                Visible = false;
        //                DialogResult = DialogResult.None;
        //                Show();
        //            }
        //        }
        //        else
        //        {
        //            ViewMode = "View";
        //            Visible = false;
        //            DialogResult = DialogResult.None;
        //            Show();
        //        }
        //    }
        //}

        //Удаление проекции
        private void ButtonRemovePlaneClick(object sender, EventArgs e)
        {
            if (chart1.Series.Count > 0)
            {
                DialogResult yResult = MessageBox.Show("Удалить проекцию " + CurrentGraphic + "?", "Запрос на удаление",
                                                       MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (yResult == DialogResult.OK)
                {
                    chart1.Series.Remove(chart1.Series[CurrentGraphic]);
                    if (dataGridViewLegend.CurrentRow != null)
                        dataGridViewLegend.Rows.Remove(dataGridViewLegend.CurrentRow);
                    _doWeNeedToSaveChanges = true;
                }
            }
        }

        //Добавление проекции
        private void ButtonAddPlaneClick(object sender, EventArgs e)
        {
            var addForm = new EditPlaneForm {Action = EditPlaneForm.Act.Add};
            //addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            //addForm.MaximizeBox = false;
            //addForm.MinimizeBox = false;
            //addForm.StartPosition = FormStartPosition.CenterScreen;
            int fullWidth = addForm.button2.Right - addForm.button1.Left;
            addForm.ParentF = this;
            for (int i = 0; i < Dim - 2; i++)
            {
                var ti = new TextBox();
                var li = new Label {AutoSize = true};
                ti.Width = (fullWidth + 5)/(Dim - 2) - 5;
                ti.Left = addForm.button1.Left + (fullWidth + 5)/(Dim - 2)*i;
                ti.Top = (addForm.button1.Top - 20)*3/4;
                ti.Name = "TB" + i;
                ti.KeyPress += TextBoxInputAnyReal;
                li.Text = "Z" + (Dim - i-2) + ":";
                li.Left = ti.Left;
                li.Top = ti.Top - 17;
                addForm.TBs.Add(ti);
                addForm.Controls.Add(ti);
                addForm.Controls.Add(li);
            }
            addForm.ShowDialog();
        }
        internal bool AddSeries(string newSeriesName)
        {
            bool chart1SeriesContainsLegend = false;
            foreach (var se in chart1.Series)
            {
                if (se.Name == newSeriesName) chart1SeriesContainsLegend = true;
            }
            if (!chart1SeriesContainsLegend)
            {
                var s = new Series
                            {
                                Name = newSeriesName,
                                BorderWidth = 2,
                                MarkerStyle = MarkerStyle.Circle,
                                MarkerSize = 10
                            };
                try
                {
                    s.MarkerBorderColor = Color.WhiteSmoke;
                    s.Color = Colorz.GetColor();
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось загрузить параметр. Превышено максимальное число параметров (80)");
                    s.Color = Color.Transparent;
                }
                switch (_graphicType)
                {
                    case GraphicTypes.Graphic:
                        s.ChartType = SeriesChartType.Line;
                        break;
                    case GraphicTypes.Graphic0:
                        s.ChartType = SeriesChartType.StepLine;
                        break;
                    case GraphicTypes.Diagram:
                        s.ChartType = SeriesChartType.Point;
                        break;
                }
                chart1.Series.Add(s);

                int u = chart1.Series.Count;
                Image im = new Bitmap(20, 20);
                dataGridViewLegend.Rows.Add(im, newSeriesName);

                Graphics paintin = Graphics.FromImage(im);
                var pen = new Pen(s.Color, 2);
                Brush brush = new SolidBrush(s.Color);
                paintin.DrawLine(pen, 3, 10, 17, 10);
                paintin.FillEllipse(brush, 6, 6, 7, 7);
                dataGridViewLegend.Rows[u - 1].Cells[0].Value = im;
                dataGridViewLegend.Rows[u - 1].Cells[0].Style.SelectionBackColor = Color.White;
                dataGridViewLegend.Rows[u - 1].Height = 18;

                SetLegendWidth();
                return true;
            }
            return false;
        }

        //Правка старших значений проекции
        private void DataGridViewLegendCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && _viewMode == ViewModes.Edit)
            {
                var VaryForm = new EditPlaneForm();
                VaryForm.Action = EditPlaneForm.Act.Vary;
                int fullWidth = VaryForm.button2.Right - VaryForm.button1.Left;
                VaryForm.ParentF = this;
                string string4split = dataGridViewLegend.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + "; ";
                for (int i = 0; i < Dim - 2; i++)
                {
                    var ti = new TextBox();
                    var li = new Label {AutoSize = true};
                    ti.Width = (fullWidth + 5)/(Dim - 2) - 5;
                    ti.Left = VaryForm.button1.Left + (fullWidth + 5)/(Dim - 2)*i;
                    ti.Top = (VaryForm.button1.Top - 20)*3/4;
                    ti.Name = "TB" + i;
                    ti.KeyPress += TextBoxInputAnyReal;
                    li.Text = "Z" + (Dim - i-2) + ":";
                    li.Left = ti.Left;
                    li.Top = ti.Top - 17;
                    VaryForm.TBs.Add(ti);
                    VaryForm.Controls.Add(ti);
                    VaryForm.Controls.Add(li);

                    int dx = string4split.IndexOf("; ");
                    if (dx != -1)
                    {
                        ti.Text = string4split.Substring(0, dx);
                        string4split = string4split.Substring(dx + 2, string4split.Length - dx - 2);
                    }
                    else ti.Text = string4split;
                }
                VaryForm.Text = "Изменение координат проекции";
                VaryForm.button1.Text = "Применить";
                VaryForm.ShowDialog();
            }
        }
        internal bool VarySeries(string newSeriesName)
        {
            bool chart1SeriesContainsLegend = false;
            foreach (var se in chart1.Series)
            {
                if (se.Name == newSeriesName) chart1SeriesContainsLegend = true;
            }
            if (!chart1SeriesContainsLegend)
            {
                chart1.Series[dataGridViewLegend.CurrentCell.Value.ToString()].Name = newSeriesName;
                dataGridViewLegend.CurrentCell.Value = newSeriesName;

                SetLegendWidth();
                _doWeNeedToSaveChanges = true;
                return true;
            }
            return false;
        }

        private void DataGridViewValuesDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            dataGridViewValues.CancelEdit();
            //e.Cancel = true;
            MessageBox.Show(e.Exception.Message);
        }

        //Режим отображения: текущий, все
        //private bool _allGraphicsViewed;
        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                foreach (var s in chart1.Series)
                {
                    s.Enabled = true;
                }
                //_allGraphicsViewed = true;
                if (chart1.Series[CurrentGraphic].Color == Color.LightGray)
                    chart1.Series[CurrentGraphic].Color = Color.Transparent;
            }
            else
            {
                if (checkBox1.CheckState == CheckState.Unchecked)
                {
                    //_allGraphicsViewed = false;
                    if (chart1.Series[CurrentGraphic].Color == Color.Transparent)
                        chart1.Series[CurrentGraphic].Color = Color.LightGray;
                    foreach (var s in chart1.Series)
                    {
                        if (s.Name != CurrentGraphic) s.Enabled = false;
                    }
                }
            }
        }

        //Расположение окна
        public void StartPos(int x, int y)
        {
            Location = new Point(x, y);
        }

        //Ширина легенды
        private void SetLegendWidth()
        {
            int maxlen = chart1.Series.Select(series => series.Name.Length).Concat(new[] {0}).Max();

            panelLegend.Width = 38 + maxlen*7;
            panelLegend.Left = splitContainerMain.Panel1.Width - panelLegend.Width - 3;

            if (Dim > 2)
            {
                panelLegend.Visible = true;
                chart1.Width = splitContainerMain.Panel1.Width - panelLegend.Width + 5;
            }
            else
            {
                panelLegend.Visible = false;
                chart1.Width = splitContainerMain.Panel1.Width;
            }
        }

        //Сохранять ли изменения по закрытию
        private void CGraphFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_viewMode == ViewModes.Edit && _doWeNeedToSaveChanges)
            {
                DialogResult yResult = MessageBox.Show("Сохранить изменения?", "Закрытие формы",
                                                       MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (yResult == DialogResult.Yes) SaveChanges();
                else if (yResult == DialogResult.Cancel) e.Cancel = true;
            }
        }

        private void ButtonSaveScaleClick(object sender, EventArgs e)
        {
            using (var rec = new RecDao(ConnectionString, "GraficsList"))
            {
                rec.FindFirst("GraficId=" + _graphicId);
                rec.Put("MaxX", double.Parse(tBoxMaxX.Text));
                rec.Put("MinX", double.Parse(tBoxMinX.Text));
                rec.Put("MaxY", double.Parse(tBoxMaxY.Text));
                rec.Put("MinY", double.Parse(tBoxMinY.Text));
                rec.Put("MajorX", double.Parse(tBoxGridX.Text));
                rec.Put("MajorY", double.Parse(tBoxGridY.Text));
            }
        }

        //Сброс масштаба
        private void ButtonDefaultXClick(object sender, EventArgs e)
        {
            using (var rec = new RecDao(ConnectionString, "GraficsList"))
            {
                rec.FindFirst("GraficId=" + _graphicId);
                tBoxGridX.Text = rec.GetString("MajorX");
                //checkBoxAutoScaleX.Checked = rec.GetBool("AutoScale");
                if (rec.GetString("MaxX") == rec.GetString("MinX"))
                {
                    tBoxMaxX.Text = chart1.ChartAreas[0].AxisX.Maximum.ToString();
                    tBoxMinX.Text = chart1.ChartAreas[0].AxisX.Minimum.ToString();
                }
                else
                {
                    tBoxMaxX.Text = rec.GetString("MaxX");
                    tBoxMinX.Text = rec.GetString("MinX");
                }
                ApplyXScale();
            }
        }

        private void ButtonDefaultYClick(object sender, EventArgs e)
        {
            using (var rec = new RecDao(ConnectionString, "GraficsList"))
            {
                rec.FindFirst("GraficId=" + _graphicId);
                tBoxGridY.Text = rec.GetString("MajorY");
                if (rec.GetString("MaxY") == rec.GetString("MinY"))
                {
                    tBoxMaxY.Text = chart1.ChartAreas[0].AxisY.Maximum.ToString();
                    tBoxMinY.Text = chart1.ChartAreas[0].AxisY.Minimum.ToString();
                }
                else
                {
                    tBoxMaxY.Text = rec.GetString("MaxY");
                    tBoxMinY.Text = rec.GetString("MinY");
                }
                ApplyYScale();
            }
        }

        //Удаление строки в датагриде
        private void DataGridViewValuesUserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (_viewMode == ViewModes.View)
            {
                e.Cancel = true;
                return;
            }
            if (CurrentGraphic != "NoGraphicsLoaded")
            {
                DeleteDataFromChart(e.Row.Index);
                _doWeNeedToSaveChanges = true;
            }
        }

        //Редактирование ячейки по одиночному клику
        private void DataGridViewValuesCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewValues.SelectedRows.Count == 0) dataGridViewValues.BeginEdit(true);
        }

        private void DataGridViewValuesUserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            _isRowInAddProcess = true;
        }

        //Подписываем ось Х извне
        public void MarkAxes(string label, string val)
        {
            try
            {
                int nmbr = int.Parse(label.Substring(label.Length - 1, 1)) - 1;
                switch (label.Substring(0, 4))
                {
                    case "Name":
                        _axisNames[nmbr].Text = val;
                        if (nmbr == 1)
                        {
                            string lS =
                                chart1.ChartAreas[0].AxisX.Title.Substring(
                                    chart1.ChartAreas[0].AxisX.Title.LastIndexOf("("));
                            chart1.ChartAreas[0].AxisX.Title = val + lS;
                        }
                        break;
                    case "Unit":
                        _axisUnits[nmbr].Text = "Ед. изм.: " + val;
                        if (nmbr == 1)
                        {
                            string lS = chart1.ChartAreas[0].AxisX.Title.Substring(0,
                                     chart1.ChartAreas[0].AxisX.Title.LastIndexOf("("));
                            chart1.ChartAreas[0].AxisX.Title = lS + "(" + val + ")";
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " " + e.StackTrace);
            }
        }

        //Если начальные значения отображения (шкалы) были нулевыми, автосайз делаем
        private void Chart1PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (_doWeNeedToFillXTextBoxesByChartValues)
            {
                tBoxMaxX.Text = chart1.ChartAreas[0].AxisX.Maximum.ToString();
                tBoxMinX.Text = chart1.ChartAreas[0].AxisX.Minimum.ToString();
                _doWeNeedToFillXTextBoxesByChartValues = false;
            }
            if (_doWeNeedToFillYTextBoxesByChartValues)
            {
                tBoxMaxY.Text = chart1.ChartAreas[0].AxisY.Maximum.ToString();
                tBoxMinY.Text = chart1.ChartAreas[0].AxisY.Minimum.ToString();
                _doWeNeedToFillYTextBoxesByChartValues = false;
            }
        }

        private void DeleteDataFromChart(int rowIndex)
        {
            chart1.Series[_currentGraphic].Points.RemoveAt(rowIndex);
        }
    }
}
