using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;
using BaseLibrary;

namespace GraphicLibrary
{
	public partial class GraphicForm : Form
	{
		private int _noas = -1; //Кол-во видимых аналоговых вертикальных осей
		private int _nods = -1; //Кол-во видимых дискретных графиков
		private int Noas
		{
			get { return _noas; }
			set
			{
				_noas = value;
                //При изменении кол-ва осей пересчитываем panel1.AutoScrollMinSize
				if (_chartTypeMode == GraphicTypeMode.Combined)
				{
					panel1.AutoScrollMinSize = new Size(_noas * AxWidth + 500,
						(int)((_nods + 1) * AxYHeight + 2 * (Nods - 1) + AxYHeight) + 10 + 130);
				}
				else
				{
					panel1.AutoScrollMinSize = new Size(
						_noas * AxWidth + 500, panel1.AutoScrollMinSize.Height);
				}
			}
		}
		private int Nods
		{
			get { return _nods; }
			set
			{
                _nods = value;
                //При изменении кол-ва графиков пересчитываем panel1.AutoScrollMinSize
				//Выходит, что минимальный размер аналоговой арии по y - 126
				if (_chartTypeMode == GraphicTypeMode.Discrete)
				{
					panel1.AutoScrollMinSize = new Size(
						panel1.AutoScrollMinSize.Width,
						(int)((_nods + 1) * AxYHeight 
						+ 2*(Nods - 1)
						+ AxYHeight //новый график если
						) + 10);//ну, и отступ
				}
				else 
				{
					panel1.AutoScrollMinSize = new Size(
						panel1.AutoScrollMinSize.Width,
						(int)((_nods + 1) * AxYHeight + 
						2*(Nods - 1) +
						AxYLabel //новый график если
						+ 126 + 1));
				}
			}
		}

		//Списки графиков
		protected List<GraphicParam> ParamsAnalog = new List<GraphicParam>();
		protected List<GraphicParam> ParamsDiscrete = new List<GraphicParam>();

        internal List<AxY> AxesY = new List<AxY>(); //Список вертикальных осей
        internal int AxWidth = 41; //Ширина вертикальной оси
        internal float AxYHeight = 20;//высота дискр графика
        internal float AxYLabel = 17;//высота наклейки к дискр графику
        internal int AxisXLabelFontHeight;

		//separator - разделитель действительного числа
		private readonly System.Globalization.CultureInfo _currentCulture =
			System.Globalization.CultureInfo.CurrentCulture;
		private readonly string _separator;

		private double _timerFactor1; //в произведении дают интервал таймера
		private int _timerFactor2 = 1000; //в произведении дают интервал таймера
        
		private bool _dataGridHide;//Спрятана ли нижняя панель
		private bool _mainMenuHide;//Спрятана ли правая панель

		//Для печати документа
		[DllImport("gdi32.dll")]
		public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
										 int nXSrc, int nYSrc, int dwRop);
        private Bitmap _memoryImage;
        
		//Скролл по Y
		private readonly ScrollBar _hscrollb = new HScrollBar();

        //Кнопка сбросить масштабирование по горизонтали
		private readonly Button _buttonScaleDrop = new Button();

        private bool _isScaleShifting = true; //двигается ли график в режиме монитора
        private bool _isInBreak; //оборван (в недост. режиме) ли график во время прорисовки точки в данный момент в режиме монитора
        public event Action<DateTime, DateTime> NeedAPieceOfArchiveSoMuch;//ивент на подкачку точек из архива в режиме монитора

		//дробная часть пустой части приближенного отображения от текущего локального отображения графика
		private double _fillScaleViewPerCentage = .8;// т.е. график отображен на столько процентов от экрана
        
		internal DatabaseType DatabaseType { get; set; } //Access/SQL
		internal string ConnectionString { get; set; } //Полный путь к базе данных

        //ВременнЫе границы отображения
		private DateTime _timeBegin = DateTime.Now;
		private DateTime _timeEnd = DateTime.Now;
		public DateTime TimeBegin
		{
			get { return _timeBegin; }
			set
			{
				if (value != _timeEnd)
				{
				    _timeBegin = value;
				}
				else
				{
					_timeBegin = value.AddSeconds(-1);
                    _timeEnd = value.AddSeconds(1);
				}
			}
		}
		public DateTime TimeEnd
		{
			get { return _timeEnd; }
			set
			{
				if (_timeBegin != value)
					_timeEnd = value;
				else
				{
					_timeBegin = value.AddSeconds(-1);
					_timeEnd = value.AddSeconds(1);
				}
			}
		}

        //Позиция визира
	    public DateTime VizirTime
	    {
            get { return DateTime.FromOADate(chart1.ChartAreas[0].CursorX.Position); }
            set
            {
                chart1.ChartAreas[0].CursorX.Position = value.ToOADate();
                textBoxVizirTime.Text = value.TimeOfDay.ToString("hh':'mm':'ss','fff");
                textBoxVizirDate.Text = value.Date.ToString("dd'.'MM'.'yyyy");
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

        //Границы по времени
        public DateTime BeginScaleViewTime
	    {
            get { return DateTime.FromOADate(chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum); }
            set
            {
                //chart1.ChartAreas[0].AxisX.ScaleView.Position = (value >= DateTime.FromOADate(chart1.ChartAreas[0].AxisX.Minimum)) ? value.ToOADate() : chart1.ChartAreas[0].AxisX.Minimum;
                chart1.ChartAreas[0].AxisX.ScaleView.Position = value.ToOADate();
                Button8Click(null, null);
                HScrollerPos();
            }
	    }

        public DateTime EndScaleViewTime
        {
            get { return DateTime.FromOADate(chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum); }
            set
            {
                //DateTime tmp = (value <= DateTime.FromOADate(chart1.ChartAreas[0].AxisX.Maximum)) ? value : DateTime.FromOADate(chart1.ChartAreas[0].AxisX.Maximum);
                DateTime tmp = value;
                TimeSpan ts = tmp.Subtract(DateTime.FromOADate(chart1.ChartAreas[0].AxisX.ScaleView.Position));

                switch(comboBox2.Text)
                {
                    case "Сек.":
                        comboBox5.Text = ts.TotalSeconds.ToString();
                        break;
                    case "Мин.":
                        comboBox5.Text = ts.TotalMinutes.ToString();
                        break;
                    case "Час.":
                        comboBox5.Text = ts.TotalHours.ToString();
                        break;
                    case "Сут.":
                        comboBox5.Text = ts.TotalDays.ToString();
                        break;
                }

                Button8Click(null, null);
            }
        }

	    private int _setScaleViewTimesType = 0;
        public void SetScaleViewTimes(DateTime beginTime, DateTime endTime)
        {
            TimeSpan ts = endTime.Subtract(beginTime);

            if (_setScaleViewTimesType == 0)
            {
                switch (comboBox2.Text)
                {
                    case "Сек.":
                        comboBox5.Text = ts.TotalSeconds.ToString();
                        break;
                    case "Мин.":
                        comboBox5.Text = ts.TotalMinutes.ToString();
                        break;
                    case "Час.":
                        comboBox5.Text = ts.TotalHours.ToString();
                        break;
                    case "Сут.":
                        comboBox5.Text = ts.TotalDays.ToString();
                        break;
                }

                _setScaleViewTimesType = 1;
            }
            else
            {
                if (ts.TotalDays > 1)
                {
                    comboBox5.Text = ts.TotalDays.ToString();
                    comboBox2.Text = "Сут.";
                }
                else if (ts.TotalHours > 1)
                {
                    comboBox5.Text = ts.TotalHours.ToString();
                    comboBox2.Text = "Час.";
                }
                else if (ts.TotalMinutes > 1)
                {
                    comboBox5.Text = ts.TotalMinutes.ToString();
                    comboBox2.Text = "Мин.";
                }
                else
                {
                    comboBox5.Text = ts.TotalSeconds.ToString();
                    comboBox2.Text = "Сек.";
                }

                _setScaleViewTimesType = 0;
            }
            //Button8Click(null, null);
            BeginScaleViewTime = beginTime;
        }

	    //На сколько кусков делим сетку по оси X
        /*//---private int _intervalCount = 10;
        
        internal int IntervalCount
		{
			get { return _intervalCount; }
			set
			{
				_intervalCount = value;
				IntervalFroMtextbox();
			}
		}
        //---*/

	    private double ScaleXMin;//Минимум приближения по оси X

		//изменение заголовка формы
		public string Caption
		{
			get { return Text; }
			set { Text = value; }
		}

		//Начальный отображаемый период свойством
		public string DefaultPeriod
		{
			get { return comboBox5.Text; }
			set
			{
				double d;
				if (double.TryParse(value, out d))
				{
					comboBox5.Text = value;
				}
			}
		}
        public string DefaultPeriodType
		{
			get { return comboBox2.Text; }
			set
			{
				var t = new HashSet<string> {"Сут.", "Час.", "Мин.", "Сек."};
				if (t.Contains(value))
				{
					comboBox2.Text = value;
				}
			}
		}
        //Установка режима начального отображения: полное/частичное
		private string _defaultScale = "full";
		public string DefaultScale
		{
			get { return _defaultScale; }
			set { _defaultScale = value == "full" ? "full" : "partial"; }
		}
        
        //Набор цветов для графиков
		internal OuterColorUseList Colorz = new OuterColorUseList();

        //установка режима анализатор/монитор
		internal TimerModes TimerMode { get; private set; }
        internal double TimerModeD { get; private set; }
        public string SetTimerMode(double m)
        {
            try
            {
                TimerModeD = m;
                TimerMode = TimerModeD == 0.0 ? TimerModes.Analyzer : TimerModes.Monitor;

                if (TimerMode == TimerModes.Monitor)
                {
                    if (m < 60)
                    {
                        _timerFactor1 = m;
                        _timerFactor2 = 1000;
                        comboBox6.Text = m.ToString();
                        comboBox3.Text = "Сек.";
                    }
                    else
                    {
                        _timerFactor1 = m / 60;
                        _timerFactor2 = 1000 * 60;
                        comboBox6.Text = _timerFactor1.ToString();
                        comboBox3.Text = "Мин.";
                    }
                    timer1.Enabled = true;
                }
                else
                {
                    comboBox3.Visible = false;
                    comboBox6.Visible = false;
                    label5.Visible = false;
                    button2.Visible = false;
                    button6.Visible = false;
                    radioButton4.Visible = false;
                    radioButton5.Visible = false;
                    dateTimePicker1.Visible = false;
                    button28.Visible = false;
                    label4.Location = new Point(label4.Location.X, label4.Location.Y - 89);
                    comboBox2.Location = new Point(comboBox2.Location.X, comboBox2.Location.Y - 89);
                    comboBox5.Location = new Point(comboBox5.Location.X, comboBox5.Location.Y - 89);
                    radioButton5.Location = new Point(radioButton5.Location.X, radioButton5.Location.Y - 89);
                    button8.Top -= 89;
                    button14.Top = button8.Top + 25;
                    button15.Top = button8.Top + 25;
                    button16.Top = button14.Top + 25;
                    button17.Top = button14.Top + 25;
                    comboBox1.Top = button8.Top + 25;
                    radioButton4.Text = "Полное";
                    radioButton5.Text = "Частичное";
                    tabControl1.Size = new Size(tabControl1.Size.Width, Math.Max(
                        groupBox5.Bottom,
                        groupBox4.Bottom) + 29);
                    groupBox1.Height = button16.Location.Y + 30;
                    groupBox5.Location = new Point(2, groupBox1.Height + 2);
                    label10.Location = new Point(label10.Location.X,
                                                 groupBox5.Location.Y + groupBox5.Height + 5);
                    label13.Location = new Point(label13.Location.X, label10.Location.Y - 3);

                    _fillScaleViewPerCentage = 1;

                    dataGridView1.Columns.Remove("Последнее");
                    //dataGridView1.Columns["Последнее"].Visible = false;
                }

                timer1.Interval = (int)(_timerFactor1 * _timerFactor2);
                return "";
            }
            catch (Exception e)
            {
                Error = new ErrorCommand("Невозможно установить режим таймера (SetTimerMode)", e);
                return ErrorMessage;
            }
        }

		//чтобы каждый раз не пересчитывать длину отображаемого периода
		private TimeSpan _paintedTimeInterval;

        //Отражает номер текущего параметра
		internal int CurrentParamNumber;

		//Режим графика: аналоговый, дискретный, комбинированный
		private enum GraphicTypeMode
		{
			Analog,
			Discrete,
			Combined,
			Empty
		}
        private GraphicTypeMode _chartTypeMode;

        //Для обработки событий
		private bool _isMouseDown4Move;
		private bool _isMouseDown4Divide;
		private bool _isMouseDown4Expand;
		private int _dividedParam;
		private int _mouseH;
		private int _mouseY;
        
        private const int AxSummand = 24; //Разница между верхом чарта и верхом фоновой арии

		//private DataGridViewCheckBoxColumn yColumn;

        //ограничения по количеству знаков после запятой отображения чисел в датагриде, на оси и т.п.
        internal int DecPlacesDefault = -1;

		public GraphicForm()
		{
			InitializeComponent();
            
            try
            {
                //Наклейки на скрывающие кнопки
                label1.Image = Properties.Resources.rightl;
                label6.Image = Properties.Resources.downl;

                //Для красивого отображение форм
                Application.EnableVisualStyles();

                //при отображении визира появляется данные с его местоположением
                chart1.CursorPositionChanged += CursorPositionView;
                chart1.CursorPositionChanged += CursorPositionGridView;
                //Синхронизация курсоров аналоговых и дискретных
                chart1.CursorPositionChanging += CursorSynch;
                chart1.CursorPositionChanged += CursorSynchFinal;

                //Чистка мусора при закрытии формы
                Closing += GarbCol;

                //Блокировка отображения формы, если не задан интервал времени
                Shown += FormAppear;

                //Назначение точности прокрутки по x
                //chart1.AxisScrollBarClicked += Chart1ScrollXClick;

                _separator = _currentCulture.NumberFormat.NumberDecimalSeparator;

                //запрещаем ввод не чисел в текстовые поля
                comboBox5.KeyPress += TextBoxInputReal;
                comboBox6.KeyPress += TextBoxInputReal;
                textBox1.KeyPress += TextBoxInputAnyReal;
                textBox2.KeyPress += TextBoxInputAnyReal;
                comboBox25.KeyPress += TextBoxInputReal;
                comboBox1.KeyPress += TextBoxInputReal;
                comboBox1.KeyPress += ComboBox1KeyPress;
                comboBox1.Leave += ComboBox1Leave;

                //Режим отображения "Сдвиг"
                radioButton5.Click += SetScalePosition1;

                //заполняем комбобоксы
                comboBox2.Items.AddRange(new[] {"Сут.", "Час.", "Мин.", "Сек."});
                comboBox3.Items.AddRange(new[] {"Час.", "Мин.", "Сек."});
                for (int i = 1; i < 11; i++) comboBox5.Items.Add(i.ToString());
                comboBox5.Items.AddRange(new[] {"12", "15", "20", "24", "30", "45", "60"});
                for (int i = 1; i < 11; i++) comboBox6.Items.Add(i.ToString());
                for (int i = 1; i < 6; i++) comboBox25.Items.Add(i.ToString());
                comboBox1.Items.AddRange(new[] {"3%", "5%", "10%", "25%", "50%"});

                //Для того, чтобы датагрид отображался быстрее
                SetDoubleBuffered(dataGridView1);

                dataGridView1.Columns.Add("№ в таблице", "№ в таблице");
                dataGridView1.Columns.Add("Код", "Код");
                dataGridView1.Columns.Add("Наименование", "Наименование");
                dataGridView1.Columns.Add("Визир", "Визир");
                dataGridView1.Columns.Add("Недост.", "Недост.");
                dataGridView1.Columns.Add("Последнее", "Последнее");
                dataGridView1.Columns.Add("Ед. измер.", "Ед. измер.");
                dataGridView1.Columns.Add("Мин.", "Мин.");
                dataGridView1.Columns.Add("Макс.", "Макс.");
                dataGridView1.Columns.Add("Тип данных", "Тип данных");
                //dataGridView1.Columns.Add("Доп. инфо", "Доп. инфо");
                //dataGridView1.Columns.Add("А мин", "А мин");
                //dataGridView1.Columns.Add("П мин", "П мин");
                //dataGridView1.Columns.Add("П макс", "П макс");
                //dataGridView1.Columns.Add("А макс", "А макс");
                dataGridView1.Columns.Add("Group", "Group");
                var yColumn = new DataGridViewCheckBoxColumn
                              {
                                  HeaderText = "Y",
                                  Width = 22,
                                  Frozen = true,
                                  AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                                  TrueValue = true,
                                  FalseValue = false
                              };
                dataGridView1.Columns.Insert(0, yColumn);
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGridView1.ScrollBars = ScrollBars.Both;
                dataGridView1.DefaultCellStyle.SelectionBackColor = SystemColors.GradientActiveCaption;
                dataGridView1.Columns["№ в таблице"].Width = 22;
                dataGridView1.Columns["№ в таблице"].Frozen = true;
                dataGridView1.Columns["№ в таблице"].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F,
                                                                                      FontStyle.Bold,
                                                                                      GraphicsUnit.Point, ((204)));
                dataGridView1.Columns["№ в таблице"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["№ в таблице"].ReadOnly = true;
                dataGridView1.Columns["Код"].Width = 130;
                dataGridView1.Columns["Код"].ReadOnly = true;
                dataGridView1.Columns["Наименование"].Width = 310;
                dataGridView1.Columns["Наименование"].ReadOnly = true;
                dataGridView1.Columns["Ед. измер."].Width = 80;
                dataGridView1.Columns["Ед. измер."].ReadOnly = true;
                dataGridView1.Columns["Визир"].Width = 70;
                dataGridView1.Columns["Визир"].ReadOnly = true;
                dataGridView1.Columns["Недост."].Width = 50;
                dataGridView1.Columns["Недост."].ReadOnly = true;
                dataGridView1.Columns["Последнее"].Width = 70;
                dataGridView1.Columns["Последнее"].ReadOnly = true;
                dataGridView1.Columns["Мин."].Width = 50;
                dataGridView1.Columns["Мин."].ReadOnly = true;
                dataGridView1.Columns["Макс."].Width = 50;
                dataGridView1.Columns["Макс."].ReadOnly = true;
                dataGridView1.Columns["Тип данных"].Width = 80;
                dataGridView1.Columns["Тип данных"].ReadOnly = true;
                //dataGridView1.Columns["Доп. инфо"].Width = 95;
                //dataGridView1.Columns["Доп. инфо"].ReadOnly = true;
                //dataGridView1.Columns["А мин"].Width = 49;
                //dataGridView1.Columns["П мин"].Width = 49;
                //dataGridView1.Columns["П макс"].Width = 50;
                //dataGridView1.Columns["А макс"].Width = 49;
                //dataGridView1.Columns["А мин"].ReadOnly = true;
                //dataGridView1.Columns["П мин"].ReadOnly = true;
                //dataGridView1.Columns["П макс"].ReadOnly = true;
                //dataGridView1.Columns["А макс"].ReadOnly = true;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns["№ в таблице"].DefaultCellStyle.ForeColor = Color.White;
                dataGridView1.Columns["№ в таблице"].DefaultCellStyle.SelectionForeColor = Color.White;
                dataGridView1.Columns["Group"].Visible = false;
                dataGridView1.MultiSelect = false;
                dataGridView1.AllowUserToAddRows = false;

                dataGridView1.CellClick += HideGraph;
                dataGridView1.CellValueChanged += CheckChangeDatagrid;
                dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
                dataGridView1.SelectionChanged += DatagridSelectionChanged;
                dataGridView1.UserDeletingRow += DatagridRowAnnihilate;
                dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;

                //Легенда отключена
                chart1.Legends.Last().Enabled = false;

                //Добавляем нулевые параметры (их арии служат фоном для остальных)
                AddParam("BckgrndAnalog", "nil", "nil", "real", 0, 1);
                AddParam("BckgrndDiscrete", "nil", "nil", "bool", 0, 1);

                //Кнопки скрывания приделываем
                Controls.Add(label6);
                label6.BringToFront();
                Controls.Add(label1);
                label1.BringToFront();

                //Приделываем полосы прокрутки
                splitContainer1.Panel1.Controls.Add(_hscrollb);
                splitContainer1.Panel1.Controls.Add(_buttonScaleDrop);
                splitContainer1.Panel1.Resize += Panel1Resize;

                _hscrollb.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                _buttonScaleDrop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                _hscrollb.Scroll += HScrollerScroll;
                _buttonScaleDrop.Click += ButtonFreeClick;
                _hscrollb.Location = new Point(0, splitContainer1.Panel1.Size.Height - 20);
                _buttonScaleDrop.Location = new Point(0, splitContainer1.Panel1.Size.Height - 20);
                _hscrollb.Width = Convert.ToInt32(chart1.Size.Width);
                //_hscrollb.Visible = false;
                _hscrollb.Value = 0;
                _hscrollb.Maximum = 1000;
                _hscrollb.Minimum = 0;
                _hscrollb.LargeChange = 1001;
                //_hscrollb.MouseDoubleClick += ScrollDoubleHit;
                //_buttonScaleDrop.Visible = false;
                _buttonScaleDrop.FlatStyle = FlatStyle.Popup;
                _buttonScaleDrop.Height = 17;
                //_buttonScaleDrop.Text = "max";
                _buttonScaleDrop.TextAlign = ContentAlignment.MiddleCenter;
                _buttonScaleDrop.Image = Properties.Resources.X_Full;
                _buttonScaleDrop.ImageAlign = ContentAlignment.MiddleCenter;
                _buttonScaleDrop.Font = new Font("Microsoft Sans Serif", 6F, FontStyle.Bold, GraphicsUnit.Point, ((204)));
                //MouseWheel += WheelZooming;
                _hscrollb.BringToFront();
                _buttonScaleDrop.BringToFront();

                //Поле с номером текущего графика
                label9.ForeColor = Color.White;
                label9.TextAlign = ContentAlignment.MiddleCenter;
                label13.ForeColor = Color.White;
                label13.TextAlign = ContentAlignment.MiddleCenter;
                label19.ForeColor = Color.White;
                label19.TextAlign = ContentAlignment.MiddleCenter;
                label16.ForeColor = Color.White;
                label16.TextAlign = ContentAlignment.MiddleCenter;
                label17.ForeColor = Color.White;
                label17.TextAlign = ContentAlignment.MiddleCenter;

                _chartTypeMode = GraphicTypeMode.Empty;
                button4.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                //button8.Enabled = false;
                //button9.Enabled = false;
                radioButton4.Enabled = false;
                radioButton5.Enabled = false;

                button14.Click += WheelZooming;
                button15.Click += WheelZooming;
                button1.Click += ButtonTimeWalkAhead;
                button5.Click += ButtonTimeWalkBack;
                button12.Click += ButtonTimeStepBack;
                button13.Click += ButtonTimeStepAhead;
                button14.MouseDown += ButtonHold;
                button14.MouseUp += ButtonRelease;
                button15.MouseDown += ButtonHold;
                button15.MouseUp += ButtonRelease;
                button12.MouseDown += ButtonHold;
                button12.MouseUp += ButtonRelease;
                button13.MouseDown += ButtonHold;
                button13.MouseUp += ButtonRelease;
                button1.MouseDown += ButtonHold;
                button1.MouseUp += ButtonRelease;
                button5.MouseDown += ButtonHold;
                button5.MouseUp += ButtonRelease;

                button4.MouseHover += ButtonsHover;
                button8.MouseHover += ButtonsHover;
                button14.MouseHover += ButtonsHover;
                button15.MouseHover += ButtonsHover;
                button16.MouseHover += ButtonsHover;
                button17.MouseHover += ButtonsHover;
                button25.MouseHover += ButtonsHover;
                button26.MouseHover += ButtonsHover;
                button1.MouseHover += ButtonsHover;
                button5.MouseHover += ButtonsHover;
                button12.MouseHover += ButtonsHover;
                button13.MouseHover += ButtonsHover;

                //checkBox25.MouseHover += CheckboxHover;

                button14.Enabled = false;
                button15.Enabled = false;
                button16.Enabled = false;
                button17.Enabled = false;

                //Запасной метод для отладки
                chart1.MouseDown += SpareMethod;

                //"Обнуляет" кнопки путешествия по времени
                button1.LostFocus += WalkColorReset;
                chart1.MouseDown += WalkColorReset;

                panel1.AutoScroll = true;

                contextMenuStrip1.ShowImageMargin = false;
                contextMenuStrip1.MaximumSize = new Size(40, 0);

                //Метод разлепляет графики, если мы перетащили один из скрепленных на область построения,
                //а также в случае выделения правой кнопкой мыши осуществляет масштабирование
                chart1.MouseUp += Chart1MouseUp;

                //Устанавливаем некоторые значения по умолчанию
                comboBox5.Text = "7";
                comboBox2.Text = "Мин.";

                _timerFactor1 = 1;
                _timerFactor2 = 1000;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}
		
        //Подготовка параметра к отрисовке
		internal void PrepareParam(GraphicParam t)
		{
			try
			{
			    button16.Visible = false;
			    button17.Left = 42;
				switch (t.DataTypeD)
				{
					case DataType.Real:
						t.PercentMode = PercentModeClass.Absolute;

						AxesY.Add(new AxY(t, AxWidth));
						t.AxY = AxesY.Last();

						t.Area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
						t.Area.AxisY.MajorGrid.LineColor = t.Series.Color;

						t.AxY.Ax.DoubleClick += AxMouseDouble;
						t.AxY.Ax.Resize += AxResize;
						t.AxY.Ax.MouseUp += MovBMouseUp;
						t.AxY.Ax.MouseDown += ResBMouseDown;
						t.AxY.Ax.MouseMove += AxMBMove;
				        //t.AxY.Ax.MouseDown += MovBMouseDown;

						t.AxY.ResizeAxButtonTop.MouseDown += ResBMouseDown;
						t.AxY.ResizeAxButtonTop.MouseUp += ResBMouseUp;
						t.AxY.ResizeAxButtonTop.MouseMove += AxResizeAreaTop;
						t.AxY.AxCap.MouseClick += OverlayTopChoose;

						t.AxY.ResizeAreaButtonTop.MouseDown += ResScndBMouseDown;
						t.AxY.ResizeAreaButtonTop.MouseUp += ResScndBMouseUp;
						t.AxY.ResizeAreaButtonTop.MouseMove += AxResizeTop;

						t.AxY.RulePBoxes[1].MouseDown += AxControlDetect;
						//t.AxY.RulePBoxes[1].MouseUp += MovBMouseUp;
						//t.AxY.RulePBoxes[1].MouseDown += ResBMouseDown;
						//t.AxY.RulePBoxes[1].MouseMove += AxMBMove;

						t.AxY.RulePBoxes[0].MouseDown += AxControlDetect;
						t.AxY.RulePBoxes[1].MouseClick += HideGrClick;
						t.AxY.RulePBoxes[0].MouseClick += HideAx;
						t.AxY.TBoxMax.KeyPress += TextBoxInputAnyReal;
						t.AxY.TBoxMax.Enter += AxControlDetect;
						
						t.AxY.ResizeAxButtonBottom.MouseDown += ResBMouseDown;
						t.AxY.ResizeAxButtonBottom.MouseUp += ResBMouseUp;
						t.AxY.ResizeAxButtonBottom.MouseMove += AxResizeAreaBottom;

						t.AxY.ResizeAreaButtonBottom.MouseDown += ResScndBMouseDown;
						t.AxY.ResizeAreaButtonBottom.MouseUp += ResScndBMouseUp;
						t.AxY.ResizeAreaButtonBottom.MouseMove += AxResizeBottom;

						t.AxY.TBoxMin.KeyPress += TextBoxInputAnyReal;
						t.AxY.TBoxMin.Enter += AxControlDetect;
                        
                        //настраиваем кол-во цифр после запятой
                        t.AxY.DecPlacesMaskFill(t.DecPlaces);

						for (int j = 0; j < 4; j++)
						{
							t.AxY.La[j].MouseDown += AxControlDetect;
							t.AxY.La[j].DoubleClick += AxMouseDouble;
							t.AxY.La[j].MouseDown += ResBMouseDown;
							t.AxY.La[j].MouseMove += AxMBMove;
							t.AxY.La[j].MouseUp += MovBMouseUp;
						}
						
                        //Если загружен лишь один аналог график
						if (ParamsAnalog.Count == 2)
						{
							if (ParamsDiscrete.Count == 1)
							{
								textBox1.Text = t.AxY.TBoxMin.Text;
								textBox2.Text = t.AxY.TBoxMax.Text;
								_hscrollb.LargeChange = 100;

								_chartTypeMode = GraphicTypeMode.Analog;

								ParamsDiscrete.First().Area.CursorX.SelectionColor = Color.Transparent;
								radioButton4.Enabled = true;
								radioButton5.Enabled = true;
								textBox1.Enabled = true;
								textBox2.Enabled = true;
								//button8.Enabled = true;
								//button9.Enabled = true;

								button14.Enabled = true;
								button15.Enabled = true;
								button16.Enabled = true;
								button17.Enabled = true;
							}
							else
							{
								_chartTypeMode = GraphicTypeMode.Combined;
								panel1.AutoScrollMinSize = new Size(
									panel1.AutoScrollMinSize.Width,
									(int) (_nods*AxYHeight +
										   //_hscrollb.Height +
										   AxYLabel //новый график если
										   + 126 + 1));
							}
							if (t.DataTypeD == DataType.Real)
							{
								textBox1.Enabled = true;
								textBox2.Enabled = true;
								button4.Enabled = true;
								//button8.Enabled = true;
								//button9.Enabled = true;
							}
						}

						Noas++; //Считаем количество видимых аналоговых графиков

						t.Series.BorderWidth = (int) Math.Abs(double.Parse(comboBox25.Text));
						t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
                        t.Area.AxisX.MajorGrid.LineColor = Color.Transparent;
						t.Area.BackColor = Color.Transparent;
						break;
					case DataType.Boolean:
						t.AxCap.Size = new Size(AxWidth, 17);
						t.AxCap.Text = t.Index.ToString();
						t.AxCap.ForeColor = t.Series.Color;
				        t.AxCap.Click += AxCapClick;

						t.Area.AxisY.MajorGrid.Enabled = false;

                        //Если загружен лишь один дискр график
						if (ParamsDiscrete.Count == 2)
						{
							if (ParamsAnalog.Count == 1)
							{
								textBox1.Text = "0";
								textBox2.Text = "2";
								_hscrollb.LargeChange = 100;

								_chartTypeMode = GraphicTypeMode.Discrete;
								//ParamsAnalog.First().Area.CursorX.LineColor = Color.Transparent;
								//ParamsAnalog.First().Area.CursorY.LineColor = Color.Transparent;
								ParamsAnalog.First().Area.CursorX.SelectionColor = Color.Transparent;
								//ParamsAnalog.First().Area.CursorY.SelectionColor = Color.Transparent;
								radioButton4.Enabled = true;
								radioButton5.Enabled = true;
								button14.Enabled = true;
								button15.Enabled = true;
								button16.Enabled = true;
								button17.Enabled = true;
							}
							else
							{
								_chartTypeMode = GraphicTypeMode.Combined;
							}
						}

						Nods++; //Добавляется видимый график

						t.PercentMode = PercentModeClass.Absolute;
						//t.DecPlaces = 0;
						chart1.Controls.Add(t.AxCap);
						break;
				}
                
                //t.Area.AxisX.IsLabelAutoFit = true;
				AxisXLabelFontHeight = ParamsAnalog[0].Area.AxisX.LabelStyle.Font.Height;
                t.Area.AxisX.IsMarginVisible = false;
			    if (ParamsCount() > 0) t.Area.AxisX.LabelStyle.Format = ParamsAnalog[0].Area.AxisX.LabelStyle.Format;

                //Если добавляемый график не является фоновым
				if (!t.Series.Color.Equals(Color.Transparent))
				{
					//Заполнение таблицы инфой
					if (TimerMode == TimerModes.Monitor)
						dataGridView1.Rows.Add(new IComparable[]
														  {
															  true, t.Index.ToString(), t.Code,
															  t.SubName == "" ? t.Name : t.Name + ". " + t.SubName
															  , "", "", t.Units, "", //"",
															  t.PtkMin.ToString(), t.PtkMax.ToString(), t.Comment
														  });
					else
					{
					    if (t.Units == null) t.Units = "";
                        //if (t.Index == null) throw new Exception("Параметру не передается индекс");
                        if (t.Code == null) t.Code = "";
                        if (t.SubName == null) t.SubName = "";
                        if (t.Name == null) t.Name = "";
                        //if (t.Min == null) t.Min = 0;
                        //if (t.Max == null) t.Max = 1;
						dataGridView1.Rows.Add(new IComparable[]
														  {
															  true, t.Index.ToString(), t.Code,
															  t.SubName == "" ? t.Name : t.Name + ". " + t.SubName
															  , "", "", t.Units, //"",
															  t.PtkMin.ToString(), t.PtkMax.ToString(), t.Comment
														  });
					}
					//dataGridView1["№ в таблице", t.Index - 1].Style.BackColor = t.Series.Color;
					//dataGridView1["№ в таблице", t.Index - 1].Style.SelectionBackColor = t.Series.Color;
					//dataGridView1[0, t.Index - 1].Style.BackColor = t.Series.Color;
					//dataGridView1[0, t.Index - 1].Style.SelectionBackColor = t.Series.Color;
					WantedRow(t.Index).Cells["№ в таблице"].Style.BackColor = t.Series.Color;
					WantedRow(t.Index).Cells["№ в таблице"].Style.SelectionBackColor = t.Series.Color;
					WantedRow(t.Index).Cells[0].Style.BackColor = t.Series.Color;
					WantedRow(t.Index).Cells[0].Style.SelectionBackColor = t.Series.Color;
					//dataGridView1["Group", t.Index - 1].Value = t.Index;
					WantedRow(t.Index).Cells["Group"].Value = t.Index;

					if (t.DataTypeD == DataType.Real)
					{
					    t.AxY.Ax.Width = AxWidth;
					    AxesRankingSize(t);
					    t.AxY.Ax.Location = new Point((Noas - 1)*AxWidth, 0);
					    t.AxY.TBoxMax.LostFocus += TextBoxMaxLeave;
					    t.AxY.TBoxMin.LostFocus += TextBoxMinLeave;
					    t.AxY.TBoxMin.KeyPress += TextBoxMinKeyPress;
					    t.AxY.TBoxMax.KeyPress += TextBoxMaxKeyPress;
					    chart1.Controls.Add(t.AxY.Ax);
                        //---
					    //---if (ParamsAnalog.Count > 6) IntervalCount = ParamsAnalog.Count == 7 ? 9 : 8;
					    //---
					}
					else
					{
					    ((DataGridViewCheckBoxCell) dataGridView1.Rows[t.Index - 1].Cells[0]).ThreeState = true;
					    //((DataGridViewCheckBoxCell) dataGridView1.Rows[t.Index - 1].Cells[0]).IndeterminateValue = 3;
					    ((DataGridViewCheckBoxCell) dataGridView1.Rows[t.Index - 1].Cells[0]).Value = 2;
					    ((DataGridViewCheckBoxCell) dataGridView1.Rows[t.Index - 1].Cells[0]).ReadOnly = true;
					}
					t.Area.AxisX.LabelStyle.ForeColor = Color.Transparent;
				}
				else
				{
					if (t.DataTypeD != DataType.Boolean)
					{
						t.AxY.Ax.Visible = false;
						t.AxY.IsHidden = true;
					}
					else
						t.AxCap.Visible = false;
					t.Area.BackColor = Color.White;
				}
				t.Area.AxisX.MajorGrid.LineColor = Color.Black;

                //Опять смотрим, действительный (целый) ли график
				if (t.DataTypeD == DataType.Real)
				{
					AreaAnRankingHeight(t);
					t.Area.Position.Y = 100f * (t.AxY.Ax.Top + AxSummand) / chart1.Height;
					foreach (var param in ParamsAnalog)
					{
						AreaRankingX(param);
					}
					foreach (var param in ParamsDiscrete)
					{
						AreaRankingX(param);
					}
				}
				else
				{
					AxYLabel = chart1.Height*9/303 + 8;
					AreaDisRankingHeight(t);
					AreaRankingX(t);
					int discrParamCounter = 1;
					foreach (var param in ParamsAnalog)
					{
						if (param.Index == param.AxY.UpperParam.Index)
						{
							AxesRankingSize(param); // расстановка осей Y в зависимости от кол-ва дискр. графиков

							if ((param.AxY.Ax.Bottom) > ParamsAnalog.First().AxY.Ax.Bottom)
							{
								param.AxY.Ax.Top = ParamsAnalog.First().AxY.Ax.Bottom - param.AxY.Ax.Height;
							}

                            //размещение арии в зависимости от оси Y
                            AreaAnRankingHeight(param);
						}
					} 
					
                    //Существуют ли видимые дискр графики
					bool visibleAxExists = AxesY.Any(axY => !axY.IsHidden);

					if (!visibleAxExists)
					{
						int nmbrOfVsblAnAxes = 0;
						foreach (var u in ParamsAnalog)
						{
							if (u.AxY.Ax.Visible && !u.AxY.IsHidden && u.AxY.UpperParam != null &&
										u.AxY.UpperParam.Index == u.Index)
							{
								nmbrOfVsblAnAxes++;
								//u.AxY.Ax.Left = (nmbrOfVsblAnAxes - 1) * AxWidth;
							}
							AreaRankingX(u);
							AreaAnRankingY(u);
						}
					}
					else
                        foreach (var param in ParamsAnalog)
                        {
                            AreaAnRankingY(param);
                        }
				    foreach (var param in ParamsDiscrete)
					{
						if (param.IsVisible) discrParamCounter++;
						AreaDisRankingY(param, discrParamCounter);
					}
					AreaDisFirstHeight();
				}

				DownerScrollRankingSize();

				chart1.ChartAreas.Add(t.Area);
				chart1.Series.Add(t.Series);

				//При выборе графика отрисовка выползаeт наверх
				GraphicParam tempGr = GetParam(CurrentParamNumber);
				if (CurrentParamNumber > 0 && tempGr.DataTypeD != DataType.Boolean)
				{
					bool seriesFlag = chart1.Series.Remove(tempGr.Series);
					bool areaFlag = chart1.ChartAreas.Remove(tempGr.Area);
					if (areaFlag) chart1.ChartAreas.Add(tempGr.Area);
					if (seriesFlag) chart1.Series.Add(tempGr.Series);
				}
			}
			catch (Exception exception)
			{
				//MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
                Error =
                        new ErrorCommand(
                            "Ошибка при добавлении нового графика (Prepare param)", exception);
			}
		}

        public void T1T()
        {
            Timer1Tick(null, null);
        }

		private void Timer1Tick(object sender, EventArgs e)
		{
            //Отрисовка накопившихся точек
            if (ParamsAnalog.Count > 1)
                foreach (GraphicParam t in ParamsAnalog)
                {
                    DrawNewStuff(t);
                }
            if (ParamsDiscrete.Count > 1)
                foreach (GraphicParam t in ParamsDiscrete)
                {
                    DrawNewStuff(t);
                }

			//сдвигаемся ли вместе с приближением
            if (chart1.ChartAreas.First().AxisX.ScaleView.IsZoomed && _isScaleShifting)
			{
			    double v =
                    (DateTime.FromOADate(chart1.ChartAreas[0].AxisX.ScaleView.Position).AddSeconds((double)timer1.Interval / 1000))
                                                                                                        .ToOADate();
                foreach (var c in chart1.ChartAreas)
			    {
                    c.AxisX.ScaleView.Position = v;
			    }
			}

            //Запись в датагрид последней точки
            for (int index = 0; index <= dataGridView1.Rows.Count - 1; index++)
            {
                if (GetParam(index + 1).Series.Points.Count > 0)
                {
                    var row = dataGridView1.Rows[index];
                    row.Cells["Последнее"].Value = GetParam(index + 1).Series.Points.Last().YValues.First();
                }
            }

            //Изменение временных рамок
            if (ParamsAnalog.Count > 1)
            {
                _paintedTimeInterval = DateTime.FromOADate(ParamsAnalog[1].Series.Points.Last().XValue) -
                                       DateTime.FromOADate(ParamsAnalog[1].Series.Points[1].XValue);
                TimeEnd = DateTime.FromOADate(ParamsAnalog[1].Series.Points.Last().XValue);
            }
            else
            {
                _paintedTimeInterval = DateTime.FromOADate(ParamsDiscrete[1].Series.Points.Last().XValue) -
                                           DateTime.FromOADate(ParamsDiscrete[1].Series.Points[1].XValue);
                TimeEnd = DateTime.FromOADate(ParamsDiscrete[1].Series.Points.Last().XValue);
            }

            //Изменение масштаба по Х
            //if (chart1.ChartAreas.First().AxisX.ScaleView.IsZoomed)
            //    chart1.ChartAreas.First().AxisX.Maximum =
            //        DateTime.FromOADate(ParamsAnalog.First().Series.Points.Last().XValue).AddSeconds(
            //            _scaleDrawFactor1*_scaleDrawFactor2).ToOADate();
            //chart1.ChartAreas.Last().AxisX.Maximum = ParamsAnalog.First().Series.Points.Last().XValue + chart1.ChartAreas.Last().AxisX.ScaleView.Size * (1 - _fillScaleViewPerCentage);
            //else
            //{
            //    foreach (var ca in ParamsAnalog)
            //    {
            //        ca.Area.AxisX.Maximum = ca.Series.Points.Last().XValue + ca.Area.AxisX.ScaleView.Size * (1 - _fillScaleViewPerCentage);
            //    }
            //    foreach (var ca in ParamsDiscrete)
            //    {
            //        ca.Area.AxisX.Maximum = ca.Series.Points.Last().XValue + ca.Area.AxisX.ScaleView.Size * (1 - _fillScaleViewPerCentage);
            //    }
            //}
		}

		//Проверка корректности ввода
		private void TextBoxInputReal(object sender, KeyPressEventArgs e)
		{
			bool separatorFlag = false;
			var textBox = (ComboBox) sender;
			if (char.IsDigit(e.KeyChar)) return;
			if (e.KeyChar == (char) Keys.Back) return;
			if (textBox.Text.IndexOf(_separator) != -1) separatorFlag = true;
			if (separatorFlag)
			{
				e.Handled = true;
				return;
			}
			if (e.KeyChar.ToString() == _separator) return;
			e.Handled = true;
		}

		private void TextBoxInputAnyReal(object sender, KeyPressEventArgs e)
		{
			bool separatorFlag = false;
			var textBox = (TextBox) sender;
			if (char.IsDigit(e.KeyChar)) return;
			if (e.KeyChar == (char) Keys.Back) return;
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
			if ((textBox.Text.IndexOf(_separator) != -1) && textBox.SelectedText.IndexOf(_separator) == -1)
				separatorFlag = true;
			if (separatorFlag)
			{
				e.Handled = true;
				return;
			}
			if (e.KeyChar.ToString() == _separator) return;
			e.Handled = true;
		}

		//Режим отображения полного графика
		private void RadioButton4CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (radioButton4.Checked)
				{
					button2.Enabled = false;
					//comboBox5.Enabled = false;
					//comboBox2.Enabled = false;
					_hscrollb.LargeChange = 1001;
					//_buttonScaleDrop.Visible = false;

                    IntervalReset();

                    //new
                    var t1 = DateTime.FromOADate(ParamsAnalog[0].Area.AxisX.ScaleView.ViewMinimum);
                    var t2 = DateTime.FromOADate(ParamsAnalog[0].Area.AxisX.ScaleView.ViewMaximum);
                    RedrawAllParams(t1, t2);
				}
			}
			catch (Exception ex)
			{
				//Смотрим, не меньше ли одной точке в нашем графике
				bool flag = ParamsAnalog.Aggregate(true,
												   (current, graphicParam) =>
												   current & graphicParam.Series.Points.Count <= 1);
				flag = ParamsDiscrete.Aggregate(flag,
												(current, graphicParam) =>
												current & graphicParam.Series.Points.Count <= 1);
				if (!flag)
				{
					Error =
						new ErrorCommand(
							"Ошибка при включении режима полного отображения (RadioButton4CheckedChanged)", ex);
					MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		//Режим отображения приближения
		private void RadioButton5CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (radioButton5.Checked)
				{
                    if ((comboBox5.Text == "") || (comboBox2.Text == "Сек." && double.Parse(comboBox5.Text) <= ScaleXMin * 60 * 60 * 24))
                        comboBox5.Text = (ScaleXMin * 60 * 60 * 24).ToString();
					//берем значения отображения приближения и интервал сетки из комбобоксов меню
					SetScalePosition1(null, null);

					button2.Enabled = true;
					//comboBox5.Enabled = true;
					//comboBox2.Enabled = true;
					_hscrollb.LargeChange = 100;
					//_buttonScaleDrop.Visible = true;
				}
			}
			catch (Exception ex)
			{
				//Смотрим, не меньше ли одной точки в нашем графике
				bool flag = ParamsAnalog.Aggregate(true,
												   (current, graphicParam) =>
												   current & graphicParam.Series.Points.Count <= 1);
				flag = ParamsDiscrete.Aggregate(flag,
												(current, graphicParam) =>
												current & graphicParam.Series.Points.Count <= 1);
				if (!flag)
				{
					Error =
						new ErrorCommand(
							"Ошибка при включении режима полного отображения (RadioButton4CheckedChanged)", ex);
					MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
				}
				throw;
			}
		}

        //Установка интервала отображения
		private void IntervalFroMtextbox()
		{
			//---
            //---double temp = double.Parse(comboBox5.Text)/IntervalCount;
            double temp = FáBil(Convert.ToDouble(comboBox5.Text), comboBox2.Text);
            //---

			foreach (var t in ParamsAnalog)
			{
                t.Area.AxisX.Interval = temp;
			}
			foreach (var t in ParamsDiscrete)
			{
				t.Area.AxisX.Interval = temp;
			}
		}

		private void ScaleviewFroMtextbox()
		{
			double temp = double.Parse(comboBox5.Text);
            if ((comboBox5.Text == "") || (comboBox2.Text == "Сек." && temp <= ScaleXMin * 60 * 60 * 24))
                comboBox5.Text = (ScaleXMin * 60 * 60 * 24).ToString();
            var standartParam = new GraphicParam();
			if (!double.IsNaN(ParamsDiscrete.First().Area.AxisX.ScaleView.Size)
				&& !double.IsNaN(ParamsAnalog.First().Area.AxisX.ScaleView.Size))
				switch (_chartTypeMode)
				{
					case GraphicTypeMode.Discrete:
						standartParam = ParamsDiscrete.First();
						break;
					case GraphicTypeMode.Analog:
						standartParam = ParamsAnalog.First();
						break;
					case GraphicTypeMode.Combined:
						standartParam = ParamsDiscrete.First().Area.AxisX.ScaleView.Size <=
										ParamsAnalog.First().Area.AxisX.ScaleView.Size
											? ParamsAnalog.First()
											: ParamsDiscrete.First();
						break;
				}
			else
			{
				if (double.IsNaN(ParamsDiscrete.First().Area.AxisX.ScaleView.Size))
					if (double.IsNaN(ParamsAnalog.First().Area.AxisX.ScaleView.Size))
					{
						standartParam = _chartTypeMode == GraphicTypeMode.Discrete
											? ParamsDiscrete.First()
											: ParamsAnalog.First();
					}
					else standartParam = ParamsAnalog.First();
				else standartParam = ParamsDiscrete.First();
            }

            //учитываем положение визира
                if (standartParam.Area.CursorX.Position >= standartParam.Area.AxisX.ScaleView.ViewMinimum &&
                    standartParam.Area.CursorX.Position <= standartParam.Area.AxisX.ScaleView.ViewMaximum)
                {
                    //if ((standartParam.Area.CursorX.SelectionEnd == standartParam.Area.CursorX.SelectionStart))
                    if (double.IsNaN(standartParam.Area.CursorX.SelectionEnd) || double.IsNaN(standartParam.Area.CursorX.SelectionStart)
                        || standartParam.Area.CursorX.SelectionEnd == standartParam.Area.CursorX.SelectionStart)
                    {
                        if (double.IsNaN(standartParam.Area.AxisX.ScaleView.Position))
                            standartParam.Area.AxisX.ScaleView.Position = //standartParam.Dots.First().Time.ToOADate();
                                TimeBegin.ToOADate();

                        double xD;
                        double pc;
                        pc = (DateTime.FromOADate(standartParam.Area.CursorX.Position) !=
                              DateTime.FromOADate(standartParam.Area.AxisX.ScaleView.Position))
                                 ? (DateTime.FromOADate(standartParam.Area.CursorX.Position)
                                    - DateTime.FromOADate(standartParam.Area.AxisX.ScaleView.Position)).TotalSeconds /
                                   (DateTime.FromOADate(standartParam.Area.AxisX.ScaleView.ViewMaximum) -
                                    DateTime.FromOADate(standartParam.Area.AxisX.ScaleView.Position)).TotalSeconds
                                 : 0;
                        ScaleviewTypeFroMcombo();
                        switch (comboBox2.Text)
                        {
                            case "Сек.":
                                try
                                {
                                    standartParam.Area.AxisX.ScaleView.Position = Math.Max(
                                        DateTime.FromOADate(standartParam.Area.CursorX.Position).AddSeconds(
                                            -temp * pc).ToOADate(),
                                        TimeBegin.ToOADate());
                                    xD = (DateTime.FromOADate(standartParam.Area.CursorX.Position)
                                          - TimeBegin.AddSeconds(temp)).TotalSeconds / _paintedTimeInterval.TotalSeconds;
                                    _hscrollb.Value = xD >= 0 ? (int)(xD * 1000) : 0;
                                }
                                catch (OverflowException)
                                {
                                    MessageBox.Show("15");
                                }
                                break;
                            case "Мин.":
                                standartParam.Area.AxisX.ScaleView.Position = Math.Max(
                                    DateTime.FromOADate(standartParam.Area.CursorX.Position).AddMinutes(
                                        -temp * pc).ToOADate(),
                                    TimeBegin.ToOADate());
                                xD = (DateTime.FromOADate(standartParam.Area.CursorX.Position)
                                      - TimeBegin.AddMinutes(temp)).TotalSeconds / _paintedTimeInterval.TotalSeconds;
                                _hscrollb.Value = xD >= 0 ? (int)(xD * 1000) : 0;
                                break;
                            case "Час.":
                                standartParam.Area.AxisX.ScaleView.Position = Math.Max(
                                    DateTime.FromOADate(standartParam.Area.CursorX.Position).AddHours(
                                        -temp * pc).ToOADate(),
                                    TimeBegin.ToOADate());
                                xD = (DateTime.FromOADate(standartParam.Area.CursorX.Position)
                                      - TimeBegin.AddHours(temp)).TotalSeconds / _paintedTimeInterval.TotalSeconds;
                                _hscrollb.Value = xD >= 0 ? (int)(xD * 1000) : 0;
                                break;
                            case "Сут.":
                                standartParam.Area.AxisX.ScaleView.Position = Math.Max(
                                    DateTime.FromOADate(standartParam.Area.CursorX.Position).AddDays(
                                        -temp * pc).ToOADate(),
                                    TimeBegin.ToOADate());
                                xD = (DateTime.FromOADate(standartParam.Area.CursorX.Position)
                                      - TimeBegin.AddDays(temp)).TotalSeconds / _paintedTimeInterval.TotalSeconds;
                                _hscrollb.Value = xD >= 0 ? (int)(xD * 1000) : 0;
                                break;
                        }
                        //---
                        double dt = FáBil(temp, comboBox2.Text);
                        //---
                        foreach (var t in ParamsAnalog)
                        {
                            t.Area.AxisX.ScaleView.Size = temp;
                            //---
                            //---t.Area.AxisX.Interval = temp / IntervalCount;
                            t.Area.AxisX.Interval = dt;
                            //---
                            t.Area.AxisX.ScaleView.Position = standartParam.Area.AxisX.ScaleView.Position;
                        }
                        foreach (var t in ParamsDiscrete)
                        {
                            try
                            {
                                t.Area.AxisX.ScaleView.Size = temp;
                                //---
                                //---t.Area.AxisX.Interval = temp / IntervalCount;
                                t.Area.AxisX.Interval = dt;
                                //---
                                t.Area.AxisX.ScaleView.Position = standartParam.Area.AxisX.ScaleView.Position;

                            }
                            catch (OverflowException)
                            {
                                MessageBox.Show("15");
                            }
                        }
                    }
                    else
                    {
                        ScaleviewTypeFroMcombo();
                        //---
                        double dt = FáBil(temp, comboBox2.Text);
                        //---
                        foreach (var t in ParamsAnalog)
                        {
                            t.Area.AxisX.ScaleView.Size = temp;
                            //---
                            //---t.Area.AxisX.Interval = temp / IntervalCount;
                            t.Area.AxisX.Interval = dt;
                            //---
                            t.Area.AxisX.ScaleView.Position = chart1.ChartAreas.First().CursorX.SelectionStart <
                                         chart1.ChartAreas.First().CursorX.SelectionEnd
                                             ? (chart1.ChartAreas.First().CursorX.SelectionStart)
                                             : (chart1.ChartAreas.First().CursorX.SelectionEnd);//standartParam.Area.CursorX.SelectionStart;
                        }
                        foreach (var t in ParamsDiscrete)
                        {
                            try
                            {
                                t.Area.AxisX.ScaleView.Size = temp;
                                //---
                                //---t.Area.AxisX.Interval = temp / IntervalCount;
                                t.Area.AxisX.Interval = dt;
                                //---
                                t.Area.AxisX.ScaleView.Position = chart1.ChartAreas.First().CursorX.SelectionStart <
                                             chart1.ChartAreas.First().CursorX.SelectionEnd
                                                 ? (chart1.ChartAreas.First().CursorX.SelectionStart)
                                                 : (chart1.ChartAreas.First().CursorX.SelectionEnd);//standartParam.Area.CursorX.SelectionStart;
                            }
                            catch (OverflowException)
                            {
                                MessageBox.Show("15");
                            }
                        }
                    }
                }
                else
                {
                    ScaleviewTypeFroMcombo();
                    //---
                    double dt = FáBil(temp, comboBox2.Text);
                    //---

                    if (TimerMode == TimerModes.Analyzer)
                    {
                        foreach (var t in ParamsAnalog)
                        {
                            try
                            {
                                t.Area.AxisX.ScaleView.Size = temp;
                                //---
                                //---t.Area.AxisX.Interval = temp / IntervalCount;
                                t.Area.AxisX.Interval = dt;
                                //---
                                t.Area.AxisX.ScaleView.Position =
                                    t.Area.AxisX.ScaleView.Position > 0
                                        ? ParamsAnalog.First().Area.AxisX.ScaleView.Position
                                        : standartParam.Area.AxisX.ScaleView.Position;
                                //t.Area.AxisX.ScaleView.Zoom(position, temp, DateTimeIntervalType.Seconds);
                                chart1.Refresh();
                            }
                            catch (OverflowException)
                            {
                                MessageBox.Show("14");
                            }
                        }
                        foreach (var t in ParamsDiscrete)
                        {
                            try
                            {
                                t.Area.AxisX.ScaleView.Size = temp;
                                //---
                                //---t.Area.AxisX.Interval = temp / IntervalCount;
                                t.Area.AxisX.Interval = dt;
                                //---
                                t.Area.AxisX.ScaleView.Position =
                                    t.Area.AxisX.ScaleView.Position > 0
                                        ? ParamsDiscrete.First().Area.AxisX.ScaleView.Position
                                        : standartParam.Area.AxisX.ScaleView.Position;
                            }
                            catch (OverflowException)
                            {
                                MessageBox.Show("15");
                            }
                        }
                    }
                    else
                    {
                        switch (comboBox2.Text)
                        {
                            case "Сек.":
                                foreach (var t in ParamsAnalog)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 + t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                            .AddSeconds(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                foreach (var t in ParamsDiscrete)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 + t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                            .AddSeconds(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                break;
                            case "Мин.":
                                foreach (var t in ParamsAnalog)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 +
                                                                t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                                .AddMinutes(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                foreach (var t in ParamsDiscrete)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 + t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                            .AddMinutes(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                break;
                            case "Час.":
                                foreach (var t in ParamsAnalog)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 + t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                            .AddHours(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                foreach (var t in ParamsDiscrete)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 + t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                            .AddHours(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                break;
                            case "Сут.":
                                foreach (var t in ParamsAnalog)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 + t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                            .AddDays(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                foreach (var t in ParamsDiscrete)
                                {
                                    t.Area.AxisX.ScaleView.Size = temp;
                                    //---
                                    //---t.Area.AxisX.Interval = temp / IntervalCount;
                                    t.Area.AxisX.Interval = dt;
                                    //---
                                    if (!double.IsNaN(t.Area.AxisX.ScaleView.ViewMaximum))
                                        t.Area.AxisX.ScaleView.Position =
                                            DateTime.FromOADate(t.Area.AxisX.ScaleView.ViewMaximum * .6 + t.Area.AxisX.ScaleView.ViewMinimum * .4)
                                            .AddDays(-.6 * t.Area.AxisX.ScaleView.Size).ToOADate();
                                }
                                break;
                        }
                    }
                }
            
            //new
            var t1 = DateTime.FromOADate(ParamsAnalog[0].Area.AxisX.ScaleView.ViewMinimum);
            var t2 = DateTime.FromOADate(ParamsAnalog[0].Area.AxisX.ScaleView.ViewMaximum);
		    RedrawAllParams(t1, t2);
		}

		private void TimeFromTextbox()
		{
			_timerFactor1 = double.Parse(comboBox6.Text);
			//timer1.Interval = (int)(_timerFactor1*_timerFactor2);
		}

		private void IntervalTypeFroMcombo()
		{
			switch (comboBox2.Text)
			{
				case "Сек.":
					foreach (var t in ParamsAnalog)
					{
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss','fff";
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Seconds;
					}
					foreach (var t in ParamsDiscrete)
					{
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss','fff";
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Seconds;
					}
					break;
				case "Мин.":
					foreach (var t in ParamsAnalog)
					{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss";
					}
					foreach (var t in ParamsDiscrete)
					{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss";
					}
					break;
				case "Час.":
					foreach (var t in ParamsAnalog)
					{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Hours;
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss";
					}
					foreach (var t in ParamsDiscrete)
					{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Hours;
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss";
					}
					break;
				case "Сут.":
					foreach (var t in ParamsAnalog)
					{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Days;
						t.Area.AxisX.LabelStyle.Format = "dd'.'MM'.'yy HH':'mm";
					}
					foreach (var t in ParamsDiscrete)
					{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Days;
						t.Area.AxisX.LabelStyle.Format = "dd'.'MM'.'yy HH':'mm";
					}
					break;
			}
		}

		private void ScaleviewTypeFroMcombo()
		{
			switch (comboBox2.Text)
			{
				case "Сек.":
					foreach (var t in ParamsAnalog)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
					foreach (var t in ParamsDiscrete)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
					break;
				case "Мин.":
					foreach (var t in ParamsAnalog)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Minutes;
					foreach (var t in ParamsDiscrete)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Minutes;
					break;
				case "Час.":
					foreach (var t in ParamsAnalog)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Hours;
					foreach (var t in ParamsDiscrete)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Hours;
					break;
				case "Сут.":
					foreach (var t in ParamsAnalog)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Days;
					foreach (var t in ParamsDiscrete)
						t.Area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Days;
					break;
			}
			//foreach (var t in ParamsAnalog)
			//    t.Area.AxisX.IntervalType = t.Area.AxisX.ScaleView.SizeType;
			//foreach (var t in ParamsDiscrete)
			//    t.Area.AxisX.IntervalType = t.Area.AxisX.ScaleView.SizeType;
		}

		private void TimeFromCombo()
		{
			switch (comboBox3.Text)
			{
				case "Сек.":
					_timerFactor2 = 1000;
					break;
				case "Мин.":
					_timerFactor2 = 60000;
					break;
				case "Час.":
					_timerFactor2 = 3600000;
					break;
			}
			//timer1.Interval = (int)(_timerFactor1*_timerFactor2);
		}

		private void IntervalReset()
		{
            try
            {
                bool isMonitorAtWork = TimerMode == TimerModes.Monitor;

                if (isMonitorAtWork)
                {
                    foreach (var t in ParamsAnalog)
                    {
                        while (t.Area.AxisX.ScaleView.IsZoomed) t.Area.AxisX.ScaleView.ZoomReset(0);
                        //t.Area.AxisX.Interval = _paintedTimeInterval.TotalDays / IntervalCount;
                        //t.Area.AxisX.IntervalType = DateTimeIntervalType.Days;
                        if (t.Series.Points.Count > 1)
                        {
                            _paintedTimeInterval = DateTime.FromOADate(t.Series.Points.Last().XValue) -
                                                   DateTime.FromOADate(t.Series.Points.First().XValue);
                            ScaleXMin = Math.Max(.000024, _paintedTimeInterval.TotalDays/1400000);
                        }
                        t.Area.AxisX.LabelStyle.Format = _paintedTimeInterval.TotalDays > 1.5
                                                             ? "dd'.'MM HH':'mm"
                                                             : "HH':'mm':'ss";
                        //IntervalSet(t);
                        t.Area.AxisX.Interval = double.NaN;
                    }
                    foreach (var t in ParamsDiscrete)
                    {
                        while (t.Area.AxisX.ScaleView.IsZoomed) t.Area.AxisX.ScaleView.ZoomReset(0);
                        //t.Area.AxisX.IntervalType = DateTimeIntervalType.Days;
                        //t.Area.AxisX.Interval = _paintedTimeInterval.TotalDays / IntervalCount;
                        if (t.Series.Points.Count > 1)
                        {
                            _paintedTimeInterval = DateTime.FromOADate(t.Series.Points.Last().XValue) -
                                                   DateTime.FromOADate(t.Series.Points.First().XValue);
                            ScaleXMin = Math.Max(.000024, _paintedTimeInterval.TotalDays/1400000);
                        }
                        //t.Area.AxisX.LabelStyle.Format = "dd'.'MM HH':'mm";
                        t.Area.AxisX.LabelStyle.Format = _paintedTimeInterval.TotalDays > 1.5
                                                             ? "dd'.'MM HH':'mm"
                                                             : "HH':'mm':'ss";
                        t.Area.AxisX.Interval = double.NaN;
                    }
                }
                else
                {
                    foreach (var t in ParamsAnalog)
                    {
                        t.Area.AxisX.ScaleView.Zoom(TimeBegin.ToOADate(), TimeEnd.ToOADate());
                        IntervalSet(t);
                    }

                    foreach (var t in ParamsDiscrete)
                    {
                        t.Area.AxisX.ScaleView.Zoom(TimeBegin.ToOADate(), TimeEnd.ToOADate());
                        IntervalSet(t);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private void IntervalSet(GraphicParam t)
		{
			t.Area.AxisX.ScaleView.Position = TimeBegin.ToOADate();
			t.Area.AxisX.ScaleView.Size = TimeEnd.ToOADate() - TimeBegin.ToOADate();
			TimeSpan size = TimeEnd - TimeBegin;
			if (_paintedTimeInterval.TotalDays > 1)
			{
				t.Area.AxisX.IntervalType = DateTimeIntervalType.Days;
                //---
				//---t.Area.AxisX.Interval = _paintedTimeInterval.TotalDays / IntervalCount;
                t.Area.AxisX.Interval = FáBil(_paintedTimeInterval.TotalDays, "Сут.");
                //---
				t.Area.AxisX.LabelStyle.Format = "dd'.'MM HH':'mm";
				comboBox5.Text = size.TotalDays.ToString();
				comboBox2.Text = "Сут.";
			}
			else
			{
				if (_paintedTimeInterval.TotalHours > 1)
				{
					t.Area.AxisX.IntervalType = DateTimeIntervalType.Hours;
                    //---
					//---t.Area.AxisX.Interval = _paintedTimeInterval.TotalHours / IntervalCount;
                    t.Area.AxisX.Interval = FáBil(_paintedTimeInterval.TotalHours, "Час.");
                    //---
					t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss";
					comboBox5.Text = size.TotalHours.ToString();
					comboBox2.Text = "Час.";
				}
				else
				{
					if (_paintedTimeInterval.TotalMinutes > 1)
					{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                        //---
						//---t.Area.AxisX.Interval = _paintedTimeInterval.TotalMinutes / IntervalCount;
                        t.Area.AxisX.Interval = FáBil(_paintedTimeInterval.TotalMinutes, "Мин.");
                        //---
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss";
						comboBox5.Text = size.TotalMinutes.ToString();
						comboBox2.Text = "Мин.";
					}
					else
					{
						try
						{
						t.Area.AxisX.IntervalType = DateTimeIntervalType.Seconds;
                        //---
						//---t.Area.AxisX.Interval = _paintedTimeInterval.TotalSeconds / IntervalCount;
                        t.Area.AxisX.Interval = FáBil(_paintedTimeInterval.TotalSeconds, "Сек.");
                        //---
						t.Area.AxisX.LabelStyle.Format = "HH':'mm':'ss','fff";
						comboBox5.Text =size.TotalSeconds.ToString();
						comboBox2.Text = "Сек.";
						}
						catch (OverflowException)
						{
							MessageBox.Show("15");
						}
					}
				}
			}
		}

		private void ComboBox5Leave(object sender, EventArgs e)
		{
			comboBox5.Select(0,0); //Отображения числа со старших разрядов
		}

		private void ComboBox6Leave(object sender, EventArgs e)
		{
			if (comboBox6.Text == "") comboBox6.Text = @"1";
			TimeFromTextbox();
		}

		private void ComboBox5KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) Keys.Enter) Button8Click(null, null);//ComboBox5Leave(comboBox5, null);
		}

		private void ComboBox6KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) Keys.Enter)
			{
				ComboBox6Leave(comboBox6, null);
			}
		}

        //private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
		//{
			//ScaleviewTypeFroMcombo();
			////Synch();
			//if (chart1.ChartAreas.Count != 0)
			//{
			//    SetScalePosition2();
			//    switch (comboBox2.Text)
			//    {
			//        case "Сек.":
			//            _scaleDrawFactor2 = 1;
			//            break;
			//        case "Мин.":
			//            _scaleDrawFactor2 = 60;
			//            break;
			//        case "Час.":
			//            _scaleDrawFactor2 = 3600;
			//            break;
			//        case "Сут.":
			//            _scaleDrawFactor2 = 3600*24;
			//            break;
			//    }
			//}
		//}

        private void ComboBox3SelectedIndexChanged(object sender, EventArgs e)
		{
			TimeFromCombo();
			//Synch();
		}

		private void ComboBox5SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox5Leave(sender, e);
		}

		private void ComboBox6SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox6Leave(sender, e);
		}

		private void ComboBox1Leave(object sender, EventArgs e)
		{
			if (comboBox1.Text.Last() != '%') comboBox1.Text += "%";
		}

		private void ComboBox1KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter) ComboBox1Leave(null, null);
		}

        private void Button27Click(object sender, EventArgs e)
        {
            timer1.Interval = (int)(_timerFactor1 * _timerFactor2);
        }

		//сдвигается ли периодически график
		private void Button2Click(object sender, EventArgs e)
		{
			_isScaleShifting = !_isScaleShifting;
			button2.Text = _isScaleShifting ? "Сдвиг стоп" : "Сдвиг пуск";
		}

		//переключение в режим Прокрутка/Наполнение автоматически с выделения мышью
		private void Chart1AxisViewChanged(object sender, ViewEventArgs e)
		{
		    try
			{
				if (e.ChartArea.CursorX.SelectionStart > 0)
				{
					TimeSpan cursorXsize =
						(DateTime.FromOADate(e.ChartArea.CursorX.SelectionEnd) -
						 DateTime.FromOADate(e.ChartArea.CursorX.SelectionStart)).Duration();

					if (cursorXsize.Days > 1) comboBox2.Text = @"Сут.";
					else
					{
						if (cursorXsize.Hours > 1) comboBox2.Text = @"Час.";
						else
						{
							comboBox2.Text = cursorXsize.TotalMinutes > 1 ? @"Мин." : @"Сек.";
						}
					}

					switch (comboBox2.Text)
					{
						case "Сек.":
							try
							{
							comboBox5.Text = string.Format("{0:F3}", cursorXsize.TotalSeconds);
                            //_scaleDrawFactor2 = 1;
							}
							catch (OverflowException)
							{
								MessageBox.Show("15");
							}
							break;
						case "Мин.":
							comboBox5.Text = string.Format("{0:F3}", cursorXsize.TotalMinutes);
							//_scaleDrawFactor2 = 60;
							break;
						case "Час.":
							comboBox5.Text = string.Format("{0:F3}", cursorXsize.TotalHours);
							//_scaleDrawFactor2 = 3600;
							break;
						case "Сут.":
							comboBox5.Text = string.Format("{0:F3}", cursorXsize.TotalDays);
							//_scaleDrawFactor2 = 3600*24;
							break;
					}

                    //double numberFromCombo5 = double.Parse(comboBox5.Text);
					//_scaleDrawFactor1 = numberFromCombo5*(1 - _fillScaleViewPerCentage);
					if (e.Axis.AxisName == AxisName.X)
					{
						if (!radioButton5.Checked)
						{
							radioButton5.Checked = true;
						}
						else
						{
                            if ((comboBox5.Text == "") || (comboBox2.Text == "Сек." && double.Parse(comboBox5.Text) <= ScaleXMin * 60 * 60 * 24))
                                comboBox5.Text = (ScaleXMin * 60 * 60 * 24).ToString();
                            SetScalePosition1(null, null);
						}

						DateTime mTime = e.ChartArea.CursorX.SelectionStart <
										 e.ChartArea.CursorX.SelectionEnd
											 ? DateTime.FromOADate(e.ChartArea.CursorX.SelectionStart)
											 : DateTime.FromOADate(e.ChartArea.CursorX.SelectionEnd);
                        //---TimeBegin = (ParamsAnalog[0].Series.Points.Count > 2)
                        //---                ? DateTime.FromOADate(ParamsAnalog[0].Series.Points[0].XValue)
                        //---                : DateTime.FromOADate(ParamsDiscrete[0].Series.Points[0].XValue);
                        TimeSpan lSpan = mTime - TimeBegin;
						_hscrollb.Value = (int) Math.Round(lSpan.TotalSeconds/_paintedTimeInterval.TotalSeconds*1000.0);
						//SetScalePosition2();
					}
					//else
				//    {
				//        //foreach (var t in ParamsAnalog)
				//        //{
				//        //    t.Area.AxisY.ScaleView.Size = ParamsAnalog[0].Area.AxisY.ScaleView.Size;
				//        //    t.Area.AxisY.Interval = t.Area.AxisY.ScaleView.Size / IntervalCount;
				//        //    t.Area.AxisY.ScaleView.Position = ParamsAnalog[0].Area.AxisY.ScaleView.Position;

				//        //    t.Area.AxisY.MajorGrid.Interval = (t.Area.AxisY.ScaleView.ViewMaximum -
				//        //                                       t.Area.AxisY.ScaleView.ViewMinimum) / 4;
				//        //    if (t == t.AxY.UpperParam)
				//        //    {
				//        //        t.AxY.TBoxMax.Text = t.Area.AxisY.ScaleView.ViewMaximum.ToString();
				//        //        t.AxY.TBoxMin.Text = t.Area.AxisY.ScaleView.ViewMinimum.ToString();
				//        //    }
				//        //}

				//        ParamsAnalog[0].Area.AxisY.ScaleView.ZoomReset();
				//        double curPos = Math.Max(ParamsAnalog[0].Area.CursorY.SelectionEnd, ParamsAnalog[0].Area.CursorY.SelectionStart);
				//        double curSize =
				//            Math.Abs(ParamsAnalog[0].Area.CursorY.SelectionEnd -
				//                     ParamsAnalog[0].Area.CursorY.SelectionStart);
				//        //сначала определили область выделения пользователем, а потом сбросили приближение и вычислили
				//        //относительные значения
				//        curPos = ParamsAnalog[0].Area.AxisY.Maximum - curPos;
				//        curPos /= (ParamsAnalog[0].Area.AxisY.Maximum - ParamsAnalog[0].Area.AxisY.Minimum);
				//        curSize /= (ParamsAnalog[0].Area.AxisY.Maximum - ParamsAnalog[0].Area.AxisY.Minimum);
				//        //MessageBox.Show(curPos + " " + curSize);
				//        //MessageBox.Show(ParamsAnalog[0].Area.CursorY.SelectionEnd + " " + ParamsAnalog[0].Area.CursorY.SelectionStart);
				//        for (int index = 1; index < AxesY.Count; index++)
				//        {
				//            var axY = AxesY[index];
				//            GraphicParam param = axY.UpperParam;
				//            double tbMin = double.Parse(axY.TBoxMin.Text);
				//            double tbMax = double.Parse(axY.TBoxMax.Text);
				//            if (curPos >= param.CurAxPos)
				//            {
				//                axY.TBoxMax.Text = ((tbMax - tbMin)
				//                                    *(param.CurAxPos + param.CurAxSize - curPos)
				//                                    /param.CurAxSize + tbMin).ToString();
				//                if (curSize + curPos < param.CurAxPos + param.CurAxSize)
				//                    axY.TBoxMin.Text = ((tbMax - tbMin)
				//                                        *(param.CurAxPos + param.CurAxSize - curPos - curSize)
				//                                        /param.CurAxSize + tbMin).ToString();
				//            }
				//            else
				//            {
				//                if (curSize + curPos < param.CurAxPos + param.CurAxSize)
				//                {
				//                    axY.TBoxMin.Text = ((tbMax - tbMin)
				//                                        *(-param.CurAxPos + curPos + curSize)
				//                                        /param.CurAxSize + tbMin).ToString();
				//                }
				//            }
				//            //TextBoxMLeave(null, null);
				//            foreach (var x in param.AxY.AxesOverlay)
				//            {
				//                YScaleDraw(GetParam(x));
				//            }
				//        }
					//}
				}

				//Надо бы это дело в порядок привести. Вот этих действий по идее делать, ну, не нужно.
				//Там еще при установке визира Selection мерцает иногда
				//Refresh();
				//ParamsDiscrete[0].Area.CursorX.SelectionStart = e.ChartArea.CursorX.Position;
				//ParamsDiscrete[0].Area.CursorX.SelectionEnd = e.ChartArea.CursorX.Position;
				//ParamsAnalog[0].Area.CursorX.SelectionStart = e.ChartArea.CursorX.Position;
				//ParamsAnalog[0].Area.CursorX.SelectionEnd = e.ChartArea.CursorX.Position;
			}
			catch (Exception ex)
			{
				bool flag = true;
				//foreach (var graphicParam in ParamsAnalog)
				//{
				//    flag = flag & graphicParam.Series.Points.Count == 0;
				//}
				//foreach (var graphicParam in ParamsDiscrete)
				//{
				//    flag = flag & graphicParam.Series.Points.Count == 0;
				//}
				flag = ParamsAnalog.Count == 1 || ParamsDiscrete.Count == 1;
				if (!flag)
				{
					Error = new ErrorCommand("Ошибка при ручном увелицении масштаба (Chart1AxisViewChanged)", ex);
					MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		//Установка позиции приближения
		private void SetScalePosition1(object sender, EventArgs e)
		{
            try
            {
                IntervalTypeFroMcombo();
                IntervalFroMtextbox();
                //ScaleviewTypeFroMcombo();
                ScaleviewFroMtextbox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Установка позиции приближения в данном месте с проверкой, в конце ли мы
		private void SetScalePosition2()
		{
			try
			{
				GraphicParam curParam = GetParam(CurrentParamNumber);
				GraphicParam standartGraph = curParam.DataTypeD == DataType.Real
												 ? ParamsAnalog.First()
												 : ParamsDiscrete.First();
				switch (comboBox2.Text)
				{
					case "Сек.":
						if (standartGraph.Area.AxisX.ScaleView.Position >
							DateTime.FromOADate(standartGraph.Series.Points.Last().XValue).AddSeconds(
								-standartGraph.Area.AxisX.ScaleView.Size).ToOADate())
						{
							foreach (var t in ParamsAnalog)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddSeconds(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).
										ToOADate();
							}
							foreach (var t in ParamsDiscrete)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddSeconds(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).
										ToOADate();
							}
						}
						else
						{
							foreach (var t in ParamsAnalog)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
							foreach (var t in ParamsDiscrete)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
						}
						break;
					case "Мин.":
						//foreach (var t in ParamsAnalog)
						//    t.Area.AxisX.ScaleView.Position =
						//        DateTime.FromOADate(t.Area.AxisX.ScaleView.Position).AddMinutes(
						//            oldScaleSize).AddMinutes(-chart1.ChartAreas.First().AxisX.ScaleView.Size).ToOADate();
						if (standartGraph.Area.AxisX.ScaleView.Position >
							DateTime.FromOADate(standartGraph.Series.Points.Last().XValue).AddMinutes(
								-standartGraph.Area.AxisX.ScaleView.Size).ToOADate())
						{
							foreach (var t in ParamsAnalog)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddMinutes(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).ToOADate();
							}
							foreach (var t in ParamsDiscrete)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddMinutes(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).ToOADate();
							}
						}
						else
						{
							foreach (var t in ParamsAnalog)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
							foreach (var t in ParamsDiscrete)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
						}
						break;
					case "Час.":
						if (standartGraph.Area.AxisX.ScaleView.Position >
							DateTime.FromOADate(standartGraph.Series.Points.Last().XValue).AddHours(
								-standartGraph.Area.AxisX.ScaleView.Size).ToOADate())
						{
							foreach (var t in ParamsAnalog)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddHours(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).
										ToOADate();
							}
							foreach (var t in ParamsDiscrete)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddHours(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).
										ToOADate();
							}
						}
						else
						{
							foreach (var t in ParamsAnalog)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
							foreach (var t in ParamsDiscrete)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
						}
						break;
					case "Сут.":
						if (standartGraph.Area.AxisX.ScaleView.Position >
							DateTime.FromOADate(standartGraph.Series.Points.Last().XValue).AddDays(
								-standartGraph.Area.AxisX.ScaleView.Size).ToOADate())
						{
							foreach (var t in ParamsAnalog)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddDays(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).
										ToOADate();
							}
							foreach (var t in ParamsDiscrete)
							{
								if (t.Series.Points.Count != 0)
								t.Area.AxisX.ScaleView.Position =
									DateTime.FromOADate(t.Series.Points.Last().XValue).AddDays(
										-t.Area.AxisX.ScaleView.Size*_fillScaleViewPerCentage).
										ToOADate();
							}
						}
						else
						{
							foreach (var t in ParamsAnalog)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
							foreach (var t in ParamsDiscrete)
							{
								t.Area.AxisX.ScaleView.Position = standartGraph.Area.AxisX.ScaleView.Position;
								t.Area.AxisX.ScaleView.Size = standartGraph.Area.AxisX.ScaleView.Size;
							}
						}
						break;
				}
			}
			catch (Exception ex)
			{
				bool flag = true;
				foreach (var graphicParam in ParamsAnalog)
				{
					flag = graphicParam.Series.Points.Count == 0;
				}
				foreach (var graphicParam in ParamsDiscrete)
				{
					flag = flag | graphicParam.Series.Points.Count == 0;
				}
				if (!flag)
				{
					Error = new ErrorCommand("Ошибка с проверкой, в конце ли графика приближение (SetScalePosition2)",
											 ex);
					MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

        //Чтобы по 10 раз не отрисовывать
	    private bool _allowChangeTBoxM = false;

        //Работа с текстбоксами осей Y
		private void TextBoxMaxKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) Keys.Enter)
			{
                GraphicParam curP = GetParam(CurrentParamNumber);
                foreach (var x in curP.AxY.AxesOverlay)
                {
                    YScaleCommonDraw(GetParam(x));
                }
                textBox2.Text = curP.AxY.TBoxMax.Text;
			}
		}

		private void TextBoxMaxLeave(object sender, EventArgs e)
		{
            if (!_allowChangeTBoxM)
            {
                //curP.AxY.TBoxMax.Select(0, 0);
                //curP.AxY.TBoxMin.Select(0, 0);
                //TextBox textBox = ((TextBox) sender);
                //textBox.Text = textBox2.Text;
                //textBox.Select(0, 0);
                GraphicParam curP = GetParam(CurrentParamNumber);
                foreach (var x in curP.AxY.AxesOverlay)
                {
                    YScaleCommonDraw(GetParam(x));
                }
                textBox1.Text = curP.AxY.TBoxMin.Text;
            }
		}

        private void TextBoxMinLeave(object sender, EventArgs e)
        {
            //GraphicParam curP = GetParam(CurrentParamNumber);
            //foreach (var x in curP.AxY.AxesOverlay)
            //{
            //    YScaleCommonDraw(GetParam(x));
            //}
            //textBox1.Text = curP.AxY.TBoxMin.Text;
            //textBox2.Text = curP.AxY.TBoxMax.Text;
            //IsYScaledAuto = false;
            //curP.AxY.TBoxMax.Select(0, 0);
            //curP.AxY.TBoxMin.Select(0, 0);
            if (!_allowChangeTBoxM)
            {
                //TextBox textBox = ((TextBox) sender);
                //textBox.Text = textBox1.Text;
                //textBox.Select(0, 0);
                GraphicParam curP = GetParam(CurrentParamNumber);
                foreach (var x in curP.AxY.AxesOverlay)
                {
                    YScaleCommonDraw(GetParam(x));
                }
                textBox1.Text = curP.AxY.TBoxMin.Text;
            }
        }

        private void TextBoxMChange(GraphicParam tParam)
        {
            //GraphicParam curP = GetParam(CurrentParamNumber);
            if (tParam.DataTypeD != DataType.Boolean)
                foreach (var x in tParam.AxY.AxesOverlay)
                {
                    YScaleDraw(GetParam(x));
                }
            //textBox1.Text = tParam.AxY.TBoxMin.Text;
            //textBox2.Text = tParam.AxY.TBoxMax.Text;
            //IsYScaledAuto = false;
        }

		private void TextBoxMinKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) Keys.Enter)
			{
                GraphicParam curP = GetParam(CurrentParamNumber);
                foreach (var x in curP.AxY.AxesOverlay)
                {
                    YScaleCommonDraw(GetParam(x));
                }
                textBox1.Text = curP.AxY.TBoxMin.Text;
			}
		}

	    internal bool YScaleFlag = true;

        //Масштаб по Y
		private void YScaleDraw(GraphicParam curP)
		{
		    try
		    {
			double fromTextBoxMax = 0;// = Convert.ToDouble(curP.AxY.TBoxMax.Text);
			//double fromTextBoxMin = double.Parse(curP.AxY.TBoxMin.Text);
			double fromTextBoxMin = 0;// = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                switch (curP.PercentMode)
                {
                    case PercentModeClass.Absolute:
                        //if (curP.AxY.TBoxMax.Text == "")
                        //{
                        //    //curP.AxY.TBoxMax.Text = curP.Area.AxisY.Maximum.ToString();
                        //    curP.AxY.ViewMaxMsr = curP.Area.AxisY.Maximum;
                        //    curP.AxY.ViewMaxPrc = curP.ValueToPercent(curP.AxY.ViewMaxMsr);
                        //}
                        //else
                        //{
                        fromTextBoxMax = Convert.ToDouble(curP.AxY.TBoxMax.Text);
                        //    curP.AxY.ViewMaxMsr = fromTextBoxMax;
                        //    //curP.AxY.ViewMaxPrc = curP.ValueToPercent(fromTextBoxMax);
                        //}
                        //if (curP.AxY.TBoxMin.Text == "")
                        //{
                        //    //curP.AxY.TBoxMin.Text = curP.Area.AxisY.Minimum.ToString();
                        //    curP.AxY.ViewMinMsr = curP.Area.AxisY.Minimum;
                        //    curP.AxY.ViewMinPrc = curP.ValueToPercent(curP.AxY.ViewMinMsr);
                        //}
                        //else
                        //{
                        fromTextBoxMin = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                        //    curP.AxY.ViewMinMsr = fromTextBoxMin;
                        //    //curP.AxY.ViewMinPrc = curP.ValueToPercent(fromTextBoxMin);
                        //}
                        if (fromTextBoxMin < fromTextBoxMax)
                        {
                            curP.Area.AxisY.ScaleView.Position = fromTextBoxMin;
                            curP.Area.AxisY.ScaleView.Size = fromTextBoxMax - fromTextBoxMin;
                        }
                        else
                        {
                            YScaleDrawDefault(curP);
                        }
                        break;
                    case PercentModeClass.Percentage:
                        //if (curP.AxY.TBoxMax.Text == "")
                        //{
                        //    //curP.AxY.TBoxMax.Text = curP.Area.AxisY.Maximum.ToString();
                        //    //curP.AxY.ViewMaxMsr = curP.Area.AxisY.Maximum;
                        //    curP.AxY.ViewMaxPrc = curP.ValueToPercent(curP.AxY.ViewMaxMsr);
                        //}
                        //else
                        //{
                        fromTextBoxMax = Convert.ToDouble(curP.AxY.TBoxMax.Text);
                        //    //curP.AxY.ViewMaxMsr = curP.PercentToValue(fromTextBoxMax); 
                        //    curP.AxY.ViewMaxPrc = fromTextBoxMax;
                        //}
                        //if (curP.AxY.TBoxMin.Text == "")
                        //{
                        //    //curP.AxY.TBoxMin.Text = curP.Area.AxisY.Minimum.ToString();
                        //    //curP.AxY.ViewMinMsr = curP.Area.AxisY.Minimum;
                        //    curP.AxY.ViewMinPrc = curP.ValueToPercent(curP.AxY.ViewMinMsr);
                        //}
                        //else
                        //{
                        fromTextBoxMin = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                        //    //curP.AxY.ViewMinMsr = curP.PercentToValue(fromTextBoxMin);
                        //    curP.AxY.ViewMinPrc = fromTextBoxMin;
                        //}
                        if (fromTextBoxMin < fromTextBoxMax)
                        {
                            //curP.Area.AxisY.ScaleView.Position = curP.AxY.ViewMinMsr;
                            //curP.Area.AxisY.ScaleView.Size = curP.AxY.ViewMaxPrc - curP.AxY.ViewMinMsr;
                            curP.Area.AxisY.ScaleView.Position = curP.PercentToValue(curP.AxY.ViewMinPrc);
                            curP.Area.AxisY.ScaleView.Size = curP.PercentToValue(curP.AxY.ViewMaxPrc) - curP.PercentToValue(curP.AxY.ViewMinPrc);
                        }
                        else
                        {
                            YScaleDrawDefault(curP);
                        }
                        break;
                }
		    curP.Area.AxisY.MajorGrid.Interval = (curP.Area.AxisY.ScaleView.ViewMaximum -
		                                          curP.Area.AxisY.ScaleView.ViewMinimum)/5;
            //for (int y = 0; y <= 3; y++)
            //{
            //    curP.AxY.La[y].Text =
            //        (fromTextBoxMin + (4 - y) * (-fromTextBoxMin + fromTextBoxMax) / 5).ToString("0.###");
            //}

                //Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private void YScaleDrawDefault(GraphicParam t)
		{
			t.Area.AxisY.ScaleView.Position = t.Min;
            t.Area.AxisY.ScaleView.Size = t.Max - t.Min;

            t.AxY.ViewMaxPrc = 100;//t.ValueToPercent(t.Max);
            t.AxY.ViewMinPrc = 0;//t.ValueToPercent(t.Min);
            t.AxY.ViewMaxMsr = t.Max;
            t.AxY.ViewMinMsr = t.Min;
		}

        private void YScaleShow(GraphicParam curP)
        {
            try
            {
                double fromTextBoxMax = 0;
                double fromTextBoxMin = 0;
                switch (curP.PercentMode)
                {
                    case PercentModeClass.Absolute:
                        //fromTextBoxMax = Convert.ToDouble(curP.AxY.TBoxMax.Text);
                        //curP.AxY.ViewMaxMsr = fromTextBoxMax;
                        //curP.AxY.ViewMaxPrc = curP.ValueToPercent(fromTextBoxMax);
                        //fromTextBoxMin = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                        //curP.AxY.ViewMinMsr = fromTextBoxMin;
                        //curP.AxY.ViewMinPrc = curP.ValueToPercent(fromTextBoxMin);
                        fromTextBoxMax = curP.AxY.ViewMaxMsr;
                        fromTextBoxMin = curP.AxY.ViewMinMsr;
                        curP.AxY.TBoxMax.Text = fromTextBoxMax.ToString();
                        curP.AxY.TBoxMin.Text = fromTextBoxMin.ToString();
                        curP.Area.AxisY.ScaleView.Position = curP.AxY.ViewMinMsr;
                        curP.Area.AxisY.ScaleView.Size = curP.AxY.ViewMaxMsr - curP.AxY.ViewMinMsr;
                        break;
                    case PercentModeClass.Percentage:
                        //fromTextBoxMax = Convert.ToDouble(curP.AxY.TBoxMax.Text);
                        //curP.AxY.ViewMaxMsr = curP.PercentToValue(fromTextBoxMax);
                        //curP.AxY.ViewMaxPrc = fromTextBoxMax;
                        //fromTextBoxMin = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                        //curP.AxY.ViewMinMsr = curP.PercentToValue(fromTextBoxMin);
                        //curP.AxY.ViewMinPrc = fromTextBoxMin;
                        fromTextBoxMax = curP.AxY.ViewMaxPrc;
                        fromTextBoxMin = curP.AxY.ViewMinPrc;
                        curP.AxY.TBoxMax.Text = curP.AxY.ViewMaxPrc.ToString();
                        curP.AxY.TBoxMin.Text = curP.AxY.ViewMinPrc.ToString();
                        curP.Area.AxisY.ScaleView.Position = curP.PercentToValue(curP.AxY.ViewMinPrc);
                        curP.Area.AxisY.ScaleView.Size = curP.PercentToValue(curP.AxY.ViewMaxPrc) -
                                                         curP.PercentToValue(curP.AxY.ViewMinPrc);
                        break;
                }
                curP.Area.AxisY.MajorGrid.Interval = (curP.Area.AxisY.ScaleView.ViewMaximum -
                                                      curP.Area.AxisY.ScaleView.ViewMinimum)/5;

                //Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void YScaleCommonDraw(GraphicParam curP)
        {
            try
            {
                double fromTextBoxMax = 0; // = Convert.ToDouble(curP.AxY.TBoxMax.Text);
                //double fromTextBoxMin = double.Parse(curP.AxY.TBoxMin.Text);
                double fromTextBoxMin = 0; // = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                switch (curP.PercentMode)
                {
                    case PercentModeClass.Absolute:
                        if (curP.AxY.TBoxMax.Text == "")
                        {
                            //curP.AxY.TBoxMax.Text = curP.Area.AxisY.Maximum.ToString();
                            curP.AxY.ViewMaxMsr = curP.Area.AxisY.Maximum;
                            curP.AxY.ViewMaxPrc = curP.ValueToPercent(curP.AxY.ViewMaxMsr);
                        }
                        else
                        {
                            fromTextBoxMax = Convert.ToDouble(curP.AxY.TBoxMax.Text);
                            curP.AxY.ViewMaxMsr = fromTextBoxMax;
                            curP.AxY.ViewMaxPrc = curP.ValueToPercent(fromTextBoxMax);
                        }
                        if (curP.AxY.TBoxMin.Text == "")
                        {
                            //curP.AxY.TBoxMin.Text = curP.Area.AxisY.Minimum.ToString();
                            curP.AxY.ViewMinMsr = curP.Area.AxisY.Minimum;
                            curP.AxY.ViewMinPrc = curP.ValueToPercent(curP.AxY.ViewMinMsr);
                        }
                        else
                        {
                            fromTextBoxMin = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                            curP.AxY.ViewMinMsr = fromTextBoxMin;
                            curP.AxY.ViewMinPrc = curP.ValueToPercent(fromTextBoxMin);
                        }
                        if (fromTextBoxMin < fromTextBoxMax)
                        {
                            curP.Area.AxisY.ScaleView.Position = fromTextBoxMin;
                            curP.Area.AxisY.ScaleView.Size = fromTextBoxMax - fromTextBoxMin;
                        }
                        else
                        {
                            YScaleDrawDefault(curP);
                        }
                        break;
                    case PercentModeClass.Percentage:
                        if (curP.AxY.TBoxMax.Text == "")
                        {
                            //curP.AxY.TBoxMax.Text = curP.Area.AxisY.Maximum.ToString();
                            curP.AxY.ViewMaxMsr = curP.Area.AxisY.Maximum;
                            curP.AxY.ViewMaxPrc = curP.ValueToPercent(curP.AxY.ViewMaxMsr);
                        }
                        else
                        {
                            fromTextBoxMax = Convert.ToDouble(curP.AxY.TBoxMax.Text);
                            curP.AxY.ViewMaxMsr = curP.PercentToValue(fromTextBoxMax);
                            curP.AxY.ViewMaxPrc = fromTextBoxMax;
                        }
                        if (curP.AxY.TBoxMin.Text == "")
                        {
                            //curP.AxY.TBoxMin.Text = curP.Area.AxisY.Minimum.ToString();
                            curP.AxY.ViewMinMsr = curP.Area.AxisY.Minimum;
                            curP.AxY.ViewMinPrc = curP.ValueToPercent(curP.AxY.ViewMinMsr);
                        }
                        else
                        {
                            fromTextBoxMin = Convert.ToDouble(curP.AxY.TBoxMin.Text);
                            curP.AxY.ViewMinMsr = curP.PercentToValue(fromTextBoxMin);
                            curP.AxY.ViewMinPrc = fromTextBoxMin;
                        }
                        if (fromTextBoxMin < fromTextBoxMax)
                        {
                            //curP.Area.AxisY.ScaleView.Position = curP.AxY.ViewMinMsr;
                            //curP.Area.AxisY.ScaleView.Size = curP.AxY.ViewMaxPrc - curP.AxY.ViewMinMsr;
                            curP.Area.AxisY.ScaleView.Position = curP.PercentToValue(curP.AxY.ViewMinPrc);
                            curP.Area.AxisY.ScaleView.Size = curP.PercentToValue(curP.AxY.ViewMaxPrc) -
                                                             curP.PercentToValue(curP.AxY.ViewMinPrc);
                        }
                        else
                        {
                            YScaleDrawDefault(curP);
                        }
                        break;
                }
                curP.Area.AxisY.MajorGrid.Interval = (curP.Area.AxisY.ScaleView.ViewMaximum -
                                                      curP.Area.AxisY.ScaleView.ViewMinimum)/5;
                //for (int y = 0; y <= 3; y++)
                //{
                //    curP.AxY.La[y].Text =
                //        (fromTextBoxMin + (4 - y) * (-fromTextBoxMin + fromTextBoxMax) / 5).ToString("0.###");
                //}
                _allowChangeTBoxM = true;
                curP.AxY.La[0].Focus();
                _allowChangeTBoxM = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

		//Отрисовка графика
		private void DrawNewStuff(GraphicParam t)
		{
		    try
            {
                if (t.IsUpdated)
                {
                    for (var j = t.NumberOfFirstWaiting; j < t.Dots.Count; j++)
                    {
                        if (_isInBreak)
                        {
                            if (t.Dots[j].Nd == 0)
                            {
                                _isInBreak = false;
                                if ((t.Dots.Count > 1) && (j > 0)) //le mien
                                    t.Series.Points.AddXY(t.Dots[j].Time.AddMilliseconds(-1),
                                                          t.Dots[j - 1].ToMomentReal().Mean);
                                t.Series.Points.Last().IsEmpty = true;
                                t.Series.Points.AddXY(t.Dots[j].Time, t.Dots[j].ToMomentReal().Mean);
                            }
                            else
                            {
                                if ((t.Dots.Count > 1) && (j > 0)) //le mien
                                {
                                    t.Series.Points.AddXY(t.Dots[j].Time.AddMilliseconds(-1),
                                                          t.Dots[j - 1].ToMomentReal().Mean);
                                    t.Series.Points.Last().IsEmpty = true;
                                }
                                t.Series.Points.AddXY(t.Dots[j].Time, t.Dots[j].ToMomentReal().Mean);
                                t.Series.Points.Last().IsEmpty = true;
                            }
                        }
                        else
                        {
                            if (t.Dots[j].Nd != 0)
                            {
                                if ((t.Dots.Count > 1) && (j > 0)) //le mien
                                    t.Series.Points.AddXY(t.Dots[j].Time.AddMilliseconds(-1),
                                                          t.Dots[j - 1].ToMomentReal().Mean);
                                t.Series.Points.AddXY(t.Dots[j].Time, t.Dots[j].ToMomentReal().Mean);
                                t.Series.Points.Last().IsEmpty = true;
                                _isInBreak = true;
                            }
                            else
                            {
                                if ((t.Dots.Count > 1) && (j > 0)) //le mien
                                    t.Series.Points.AddXY(t.Dots[j].Time.AddMilliseconds(-1),
                                                          t.Dots[j - 1].ToMomentReal().Mean);
                                t.Series.Points.AddXY(t.Dots[j].Time, t.Dots[j].ToMomentReal().Mean);
                            }
                        }
                    }
                    t.IsUpdated = false;
                    t.NumberOfLastViewed = t.Dots.Count - 1;
                }
                
                if (TimerMode == TimerModes.Monitor)
                {
                    if (t.Dots.Count > 0) //le mien
                    {
                        //double tD = t.Dots.Last().ToMomentReal().Mean;
                        t.Series.Points.AddXY(DateTime.Now, t.Dots.Last().ToMomentReal().Mean);
                        //t.Series.Points.RemoveAt(t.Series.Points.Count - 2);
                        if (_isInBreak) t.Series.Points.Last().IsEmpty = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//При установке визира всплывает подсказка относительно текущей точки текущего графика
		private void CursorPositionView(object sender, CursorEventArgs e)
		{
			try
			{
				GraphicParam currentParam = GetParam(CurrentParamNumber);

				textBoxVizirTime.Text =
					DateTime.FromOADate(ParamsDiscrete[0].Area.CursorX.Position).TimeOfDay.ToString(
						"hh':'mm':'ss','fff");
				textBoxVizirDate.Text =
					(DateTime.FromOADate(ParamsAnalog[0].Area.CursorX.Position)).Date.ToString("dd'.'MM'.'yyyy");

			    string decPlacesTemplate = currentParam.DataTypeD == DataType.Boolean ? "0" : currentParam.AxY.DecPlacesMask;
			    string tempStr = string.Format("График №" + CurrentParamNumber + "\n" + currentParam.Dots.FindLast(x =>
			                                                                                                       (x.Time.
			                                                                                                            ToOADate() <=
			                                                                                                        e.ChartArea.
			                                                                                                            CursorX.
			                                                                                                            Position))
			                                                                                .ToMomentReal().Mean.ToString(
                                                                                                decPlacesTemplate) +
			                                   "\n" + currentParam.ValueToPercent(
			                                       currentParam.Dots.FindLast(x => (x.Time.ToOADate() <=
			                                                                        e.ChartArea.CursorX.Position)).ToMomentReal()
                                                       .Mean).ToString(decPlacesTemplate) +
			                                   "%\n{0:dd.MM.yyyy}\n{1:hh':'mm':'ss','fff}",
			                                   (DateTime.FromOADate(e.ChartArea.CursorX.Position)).Date,
			                                   (DateTime.FromOADate(e.ChartArea.CursorX.Position)).TimeOfDay);
			    toolTip1.SetToolTip(chart1, tempStr);
			}
			catch (Exception ex)
			{
				//Error = new ErrorCommand("При увеличении масштаба в графике возникает конфликт с визиром", ex);
				//MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}

		//При установке визира записывает инфу в датагрид и в строку Время визира
        //private void CursorPositionGridView(object sender, CursorEventArgs e)
        //{
        //    try
        //    {
        //        GraphicParam backParam = new GraphicParam();
        //        switch (e.ChartArea.Name)
        //        {
        //            case "ChartAreaBckgrndAnalog":
        //                backParam = ParamsAnalog.First();
        //                break;
        //            case "ChartAreaBckgrndDiscrete":
        //                backParam = ParamsDiscrete.First();
        //                break;
        //        }
        //        //if (button9.BackColor == SystemColors.ButtonHighlight)
        //        for (int index = 0; index < dataGridView1.Rows.Count; index++)
        //        {
        //            //var row = dataGridView1.Rows[index];
        //            var row = WantedRow(index + 1);
        //            GraphicParam curP = GetParam(index + 1);
        //            int dotIndex;
        //            switch (curP.PercentMode)
        //            {
        //                case PercentModeClass.Absolute:
        //                    //row.Cells["Визир"].Value =
        //                    //    GetParam(index + 1).Dots.FindLast(
        //                    //        x => (x.Time.ToOADate() <= backParam.Area.CursorX.Position))
        //                    //        .ToValueString();
        //                    dotIndex = curP.Dots.FindIndex(
        //                        x => (x.Time.ToOADate() > backParam.Area.CursorX.Position));
        //                    if (dotIndex != -1)
        //                    {
        //                        //row.Cells["Визир"].Value = GetParam(index + 1).Dots[dotIndex - 1].ToValueString();
        //                        double ts = (curP.Dots[dotIndex - 1].ToMomentReal().Mean);
        //                        row.Cells["Визир"].Value = curP.DataTypeD == DataType.Boolean
        //                                                       ? ts.ToString()
        //                                                       : ts.ToString(curP.AxY.DecPlacesMask);
        //                        //row.Cells["Визир"].Value = ts.ToString();
        //                        row.Cells["Недост."].Value = curP.Dots[dotIndex - 1].Nd.ToString();
        //                    }
        //                    break;
        //                case PercentModeClass.Percentage:
        //                    dotIndex = curP.Dots.FindIndex(
        //                        x => (x.Time.ToOADate() > backParam.Area.CursorX.Position));
        //                    if (dotIndex != -1)
        //                    {
        //                        //row.Cells["Визир"].Value =
        //                        //    curP.ValueToPercent(GetParam(index + 1).Dots[dotIndex - 1].ToMomentReal().Mean) + "%";
        //                        double ts = curP.ValueToPercent(curP.Dots[dotIndex - 1].ToMomentReal().Mean);
        //                        row.Cells["Визир"].Value = curP.DataTypeD == DataType.Boolean
        //                                                       ? ts + "%"
        //                                                       : ts.ToString(curP.AxY.DecPlacesMask) + "%";
        //                        //row.Cells["Визир"].Value = ts.ToString() + "%";
        //                        row.Cells["Недост."].Value = curP.Dots[dotIndex - 1].Nd.ToString();
        //                    }
        //                    break;
        //                case PercentModeClass.NotDefined:
        //                    row.Cells["Визир"].Value = "";
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //Error = new ErrorCommand("При попадании курсора вне области построения всплывающее окно не появляется", ex);
        //        for (int index = 0; index < dataGridView1.Rows.Count; index++)
        //        {
        //            //var row = dataGridView1.Rows[index];
        //            var row = WantedRow(index + 1);
        //            row.Cells["Визир"].Value = "";
        //            //row.Cells["Время визира"].Value = "";
        //        }
        //    }
        //}

        //При установке визира записывает инфу в датагрид и в строку Время визира
        private void CursorPositionGridView(object sender, CursorEventArgs e)
        {
            try
            {
                GraphicParam backParam = new GraphicParam();
                switch (e.ChartArea.Name)
                {
                    case "ChartAreaBckgrndAnalog":
                        backParam = ParamsAnalog.First();
                        break;
                    case "ChartAreaBckgrndDiscrete":
                        backParam = ParamsDiscrete.First();
                        break;
                }
                
                for (int index = 0; index < dataGridView1.Rows.Count; index++)
                {
                    var row = WantedRow(index + 1);
                    GraphicParam curP = GetParam(index + 1);
                    
                    var dot = curP.Dots.FindLast(x => (x.Time.ToOADate() <= backParam.Area.CursorX.Position));
                    if ((dot != null) && (curP.PercentMode!=PercentModeClass.NotDefined))
                    {
                        double ts;
                        switch (curP.PercentMode)
                        {
                            case PercentModeClass.Absolute:
                                ts = dot.ToMomentReal().Mean;
                                row.Cells["Визир"].Value = (curP.DataTypeD == DataType.Boolean)
                                                               ? ts.ToString() : ts.ToString(curP.AxY.DecPlacesMask);
                                
                                break;
                            case PercentModeClass.Percentage:
                                ts = curP.ValueToPercent(dot.ToMomentReal().Mean);
                                row.Cells["Визир"].Value = (curP.DataTypeD == DataType.Boolean)
                                                               ? ts + "%" : ts.ToString(curP.AxY.DecPlacesMask) + "%";
                                
                                break;
                        }

                        row.Cells["Недост."].Value = dot.Nd.ToString();
                    }
                    else
                    {
                        row.Cells["Визир"].Value = "";
                        row.Cells["Недост."].Value = "";
                    }
                }
            }
            catch (Exception)
            {
                for (int index = 0; index < dataGridView1.Rows.Count; index++)
                {
                    var row = WantedRow(index + 1);
                    row.Cells["Визир"].Value = "";
                    row.Cells["Недост."].Value = "";
                }
            }
        }
        
		//Синхронизация визиров
		private void CursorSynch(object sender, CursorEventArgs e)
		{
            try
            {
                if (e.Axis.AxisName == AxisName.X)
                {
                    switch (e.ChartArea.Name)
                    {
                        case "ChartAreaBckgrndAnalog":
                            //чтобы не моргал визир
                            if (Math.Abs(ParamsDiscrete.First().Area.CursorX.SelectionEnd -
                                         ParamsDiscrete.First().Area.CursorX.SelectionStart)
                                <
                                .03*(e.ChartArea.AxisX.ScaleView.ViewMaximum - e.ChartArea.AxisX.ScaleView.ViewMinimum))
                            {
                                ParamsDiscrete.First().Area.CursorX.SelectionStart = double.NaN;
                                //ParamsDiscrete.First().Area.CursorX.SelectionEnd = double.NaN;
                            }

                            ParamsDiscrete.First().Area.CursorX.SelectionStart =
                                ParamsAnalog.First().Area.CursorX.SelectionStart;
                            ParamsDiscrete.First().Area.CursorX.SelectionEnd =
                                ParamsAnalog.First().Area.CursorX.SelectionEnd;
                            break;
                        case "ChartAreaBckgrndDiscrete":
                            if (Math.Abs(ParamsAnalog[0].Area.CursorX.SelectionEnd -
                                         ParamsAnalog[0].Area.CursorX.SelectionStart)
                                <
                                .03*(e.ChartArea.AxisX.ScaleView.ViewMaximum - e.ChartArea.AxisX.ScaleView.ViewMinimum))
                            {
                                ParamsAnalog[0].Area.CursorX.SelectionStart = double.NaN;
                                //ParamsAnalog[0].Area.CursorX.SelectionEnd = double.NaN;
                            }

                            ParamsAnalog[0].Area.CursorX.SelectionStart =
                                ParamsDiscrete[0].Area.CursorX.SelectionStart;
                            ParamsAnalog[0].Area.CursorX.SelectionEnd = ParamsDiscrete[0].Area.CursorX.SelectionEnd;
                            break;
                    }
                    ParamsDiscrete.First().Area.CursorX.Position = e.NewPosition;
                    ParamsAnalog.First().Area.CursorX.Position = e.NewPosition;
                    //ParamsAnalog[0].Area.CursorX.SelectionStart = e.ChartArea.CursorX.SelectionStart;
                    //ParamsAnalog[0].Area.CursorX.SelectionEnd = e.ChartArea.CursorX.SelectionEnd;
                    //ParamsDiscrete[0].Area.CursorX.SelectionStart = e.ChartArea.CursorX.SelectionStart;
                    //ParamsDiscrete[0].Area.CursorX.SelectionEnd = e.ChartArea.CursorX.SelectionEnd;
                }
                //else
                //{
                //    _curPos = Math.Min(ParamsAnalog[0].Area.CursorY.SelectionEnd, ParamsAnalog[0].Area.CursorY.SelectionStart)
                //                / ParamsAnalog[0].Area.AxisY.ScaleView.Size;
                //    _curSize = ParamsAnalog[0].Area.CursorY.Interval / ParamsAnalog[0].Area.AxisY.ScaleView.Size;
                //    Text = ParamsAnalog[0].Area.AxisY.Interval.ToString();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private void CursorSynchFinal(object sender, CursorEventArgs e)
		{
            try
            {
                if (e.Axis.AxisName == AxisName.X)
                {
                    ParamsDiscrete.First().Area.CursorX.Position = e.NewPosition;
                    ParamsAnalog.First().Area.CursorX.Position = e.NewPosition;
                    ParamsAnalog[0].Area.CursorX.SelectionStart = double.NaN;
                    //ParamsAnalog[0].Area.CursorX.SelectionEnd = double.NaN;
                    ParamsDiscrete[0].Area.CursorX.SelectionStart = double.NaN;
                    //ParamsDiscrete[0].Area.CursorX.SelectionEnd = double.NaN;
                }
                chart1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		/*Печать чарта*/
		private void Button3Click(object sender, EventArgs e)
		{
			try
			{
				//var pd1 = new PrintDialog {Document = chart1.Printing.PrintDocument};
				//var ppd1 = new PrintPreviewDialog {Document = chart1.Printing.PrintDocument};
				//var psd1 = new PageSetupDialog {Document = chart1.Printing.PrintDocument};
				//chart1.Printing.PrintDocument.DefaultPageSettings.Landscape = true;
				//chart1.Printing.PrintDocument.DefaultPageSettings.Margins = new Margins(100,100,100,100);
				//if (ppd1.ShowDialog() == DialogResult.OK) chart1.Printing.Print(false);
				var s = new Size(Size.Width, Size.Height);
				//DialogResult d = MessageBox.Show("Включить метки графиков?", "Печать", MessageBoxButtons.YesNo,
				//                                 MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				//Visible = false;
				Size = new Size(2048,1736);

				Refresh();
				Label[,] indexLabels = new Label[ParamsAnalog.Count - 1, 4];
				Label capL = new Label{Text = Caption, AutoSize = true, BackColor = Color.Transparent};
				Label dateL = new Label { Text = DateTime.Now.ToString(), AutoSize = true, BackColor = Color.Transparent };
				if (checkBox1.Checked)
				{
					for (int indexP = 0; indexP < ParamsAnalog.Count - 1; indexP++)
					{
						GraphicParam curP = ParamsAnalog[indexP + 1];
						if (curP.IsVisible)
						for (int indexN = 0; indexN < 4; indexN++)
						{
							indexLabels[indexP, indexN] = new Label
															  {
																  Text = curP.Index.ToString(),
																  BackColor = Color.Transparent,
																  AutoSize = true,
																  Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204)))
															  };
							chart1.Controls.Add(indexLabels[indexP, indexN]);
							double labelXPosition;
							if (indexN < 3)
								labelXPosition = ((curP.Area.Position.X*chart1.Width/100) +
												  (indexN + .2)*(curP.Area.Position.Width*chart1.Width/330)
												  + (-3 + indexP)*15);
							else
                                labelXPosition = ((curP.Area.Position.X * chart1.Width / 100) +
                                                  (indexN) * (curP.Area.Position.Width * chart1.Width / 320)
                                                  + (-2 - ParamsAnalog.Count + indexP) * 15);
						    try
						    {
						        double labelXValue = curP.Area.AxisX.PixelPositionToValue(labelXPosition);
						        MomentValue xValue = curP.Dots.FindLast(x => (x.Time.ToOADate() <= labelXValue));
						        var m = xValue == null ? 0 : xValue.ToMomentReal().Mean;
						        double labelYPosition = curP.Area.AxisY.ValueToPixelPosition(m);
						        indexLabels[indexP, indexN].Top = (int) (Math.Round(labelYPosition)) - 5;
						        indexLabels[indexP, indexN].Left = (int) Math.Round(labelXPosition) - 8;
						        if (labelYPosition + 35 > curP.Area.Position.Bottom*chart1.Height/100 ||
						            labelYPosition < curP.Area.Position.Y*chart1.Height/100)
						            indexLabels[indexP, indexN].Text = "";
						        //indexLabels[indexP, indexN].Text = (DateTime.FromOADate(
						        //    curP.Area.AxisX.PixelPositionToValue(0))).ToString();        
						    }
                            catch {}
						}
					}
				}
				chart1.Controls.Add(dateL);
				dateL.Location = new Point((int)((ParamsAnalog[0].Area.Position.X*chart1.Width/100) +
												  (ParamsAnalog[0].Area.Position.Width*chart1.Width/100) - 150), 0);
				chart1.Controls.Add(capL);
				capL.Location = new Point((int)((ParamsAnalog[0].Area.Position.X * chart1.Width / 100)+20), 0);

				CaptureP();
				if (checkBox1.Checked)
				{
					foreach (var l in indexLabels)
					{
						if (l != null) l.Dispose();
					}
				}
				dateL.Dispose();
				capL.Dispose();
				Size = s;
				//Visible = true;
				//var ppd = new PrintPreviewDialog { Document = new PrintDocument() };
				//var ppd = new PageSetupDialog { Document = chart1.Printing.PrintDocument };
			    var ppd = new PrintDocument();
                ppd.PrintPage += PrintDocument1PrintPage;
				//ppd.Document.PrintPage += PrintDocument1PrintPage;
				//ppd.Document.DefaultPageSettings.Landscape = true;
			    ppd.DefaultPageSettings.Landscape = true;
				//ppd.ShowDialog();
				TopMost = false;
				//ppd.Show();
				//ppd.TopMost = true;
				//if (ppd.DialogResult == DialogResult.OK)
				//{
				//    ppd.Document.Print();
				//}
				//ppd.Closing += PpdClosing;
				//if (ppd.ShowDialog() == DialogResult.OK) chart1.Printing.Print(false);
			    var predlg = new PrintPreviewDialog {Document = ppd};
			    predlg.ShowDialog();
                //ppd.Print();
			}
			catch (Exception ex)
			{
				Error = new ErrorCommand("Ошибка печати (Button3Click)", ex);
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}

		private void PpdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			TopMost = true;
		}

		private void CaptureP()
		{
			//Graphics mygraphics = CreateGraphics();
			//_memoryImage = new Bitmap(splitContainer1.Width, splitContainer1.Height, mygraphics);
			//Graphics memoryGraphics = Graphics.FromImage(_memoryImage);
			//IntPtr dc1 = mygraphics.GetHdc();
			//IntPtr dc2 = memoryGraphics.GetHdc();
			//BitBlt(dc2, 0, 0, ClientRectangle.Width, ClientRectangle.Height, dc1, 7,
			//       7, 13369376);
			//mygraphics.ReleaseHdc(dc1);
			//memoryGraphics.ReleaseHdc(dc2);
			Graphics mGraphics = splitContainer1.CreateGraphics();
			_memoryImage = new Bitmap(splitContainer1.Width, splitContainer1.Height, mGraphics);
			splitContainer1.DrawToBitmap(_memoryImage, new Rectangle(0, 0, splitContainer1.Width, splitContainer1.Height));
		}

		private void PrintDocument1PrintPage(object sender, PrintPageEventArgs e)
		{
			e.Graphics.DrawImage(_memoryImage, 40, 40, e.PageSettings.PrintableArea.Height - 70,
								 e.PageSettings.PrintableArea.Width - 70);
		}

		private void Chart1Resize(object sender, EventArgs e)
		{
			try
			{
				foreach (var axY in AxesY)
				{
					AxesRankingSize(axY.UpperParam);
				}

				foreach (var t in ParamsAnalog)
				{
					AreaRankingX(t);
					//if (t.Index == t.AxY.UpperParam.Index) AxesRankingSize(t);
					//AreaAnRankingHeight(t);
					AreaAnRankingY(t);
				}
				int discrParamCounter = 2;
				AxYLabel = chart1.Height * 9 / 303 + 8;
				for (int index = 1; index < ParamsDiscrete.Count; index++)
				{
					var t = ParamsDiscrete[index];
					AreaDisRankingY(t, discrParamCounter);
					if (t.IsVisible) discrParamCounter++;
					AreaRankingX(t);
					AreaDisRankingHeight(t);
				}
				AreaRankingX(ParamsDiscrete[0]);
				AreaDisFirstHeight();
			}
			catch (Exception exception)
			{
                Error = new ErrorCommand("При увеличении масштаба в графике возникает конфликт с визиром", exception);
				//MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
			}
		}
		
		////Масштабирование
		//private void RadioButton6CheckedChanged(object sender, EventArgs e)
		//{
		//    GraphicParam temp = GetParam(CurrentParamNumber);
		//    if (radioButton6.Checked)
		//    {
		//        temp.Area.AxisY.ScaleView.Position = temp.Area.AxisY.Minimum;
		//        temp.Area.AxisY.ScaleView.Size = temp.Area.AxisY.Maximum - temp.Area.AxisY.Minimum;
		//        switch (temp.PercentMode)
		//        {
		//            case PercentModeClass.Absolute:
		//                temp.TBoxMin.Text = temp.Area.AxisY.Minimum.ToString();
		//                temp.TBoxMax.Text = temp.Area.AxisY.Maximum.ToString();
		//                break;
		//            case PercentModeClass.Percentage:
		//                temp.TBoxMin.Text = temp.ValueToPercent(temp.Area.AxisY.Minimum).ToString();
		//                temp.TBoxMax.Text = temp.ValueToPercent(temp.Area.AxisY.Maximum).ToString();
		//                break;
		//        }

		//        for (int y = 0; y <= 2; y++)
		//        {
		//            temp.la[y].Text =
		//                (double.Parse(temp.TBoxMin.Text) +
		//                 (3 - y)*(-double.Parse(temp.TBoxMin.Text) + double.Parse(temp.TBoxMax.Text))/4).ToString(
		//                     "0.###");
		//        }

		//        button4.Enabled = false;
		//        textBox1.Enabled = false;
		//        textBox2.Enabled = false;
		//    }
		//    temp.Area.AxisY.MajorGrid.Interval = (temp.Area.AxisY.ScaleView.ViewMaximum -
		//                                          temp.Area.AxisY.ScaleView.ViewMinimum)/4;
		//}

		//private void RadioButton7CheckedChanged(object sender, EventArgs e)
		//{
		//    GraphicParam temp = GetParam(CurrentParamNumber);
		//    if (radioButton7.Checked)
		//    {
		//        if (temp.DataTypeD == DataType.Real)
		//        {
		//            string a1 = textBox1.Text == "" ? temp.Area.AxisY.Minimum.ToString() : textBox1.Text;
		//            string a2 = textBox2.Text == "" ? temp.Area.AxisY.Maximum.ToString() : textBox2.Text;
		//            switch (temp.PercentMode)
		//            {
		//                case PercentModeClass.Absolute:
		//                    temp.Area.AxisY.ScaleView.Position = double.Parse(a1);
		//                    temp.Area.AxisY.ScaleView.Size = double.Parse(a2) - double.Parse(a1);
		//                    break;
		//                case PercentModeClass.Percentage:
		//                    //temp.TBoxMin.Text = temp.ValueToPercent(double.Parse(a1)).ToString();
		//                    //temp.TBoxMax.Text = temp.ValueToPercent(double.Parse(a2)).ToString();
		//                    temp.Area.AxisY.ScaleView.Position = temp.PercentToValue(double.Parse(a1));
		//                    temp.Area.AxisY.ScaleView.Size = temp.PercentToValue(double.Parse(a2)) -
		//                                                     temp.PercentToValue(double.Parse(a1));
		//                    break;
		//            }
		//            temp.TBoxMin.Text = a1;
		//            temp.TBoxMax.Text = a2;
		//            for (int y = 0; y <= 2; y++)
		//            {
		//                temp.la[y].Text = (double.Parse(temp.TBoxMin.Text) + (3 - y)*
		//                                   (-double.Parse(temp.TBoxMin.Text) +
		//                                    double.Parse(temp.TBoxMax.Text))/4).ToString("0.###");
		//            }
		//        }
		//        button4.Enabled = true;
		//        textBox1.Enabled = true;
		//        textBox2.Enabled = true;
		//    }
		//    temp.Area.AxisY.MajorGrid.Interval = (temp.Area.AxisY.ScaleView.ViewMaximum -
		//                                          temp.Area.AxisY.ScaleView.ViewMinimum)/4;
		//}

		//Масштабирование по y текущего графика по указанным значениям
		private void Button4Click(object sender, EventArgs e)
		{
            try
            {
                GraphicParam t = GetParam(CurrentParamNumber);
                double a1 = -9999, a2 = 9999;

                YScaleFlag = false;
                switch (t.PercentMode)
                {
                    case PercentModeClass.Absolute:
                        a1 = textBox1.Text == "" ? t.Area.AxisY.Minimum : double.Parse(textBox1.Text);
                        a2 = textBox2.Text == "" ? t.Area.AxisY.Maximum : double.Parse(textBox2.Text);

                        if (t.DecPlaces >= 0)
                        {
                            a1 = Math.Round(a1, t.DecPlaces);
                            textBox1.Text = a1.ToString();
                        }
                        if (t.DecPlaces >= 0)
                        {
                            a2 = Math.Round(a2, t.DecPlaces);
                            textBox2.Text = a2.ToString();
                        }

                        if (a1 > a2)
                        {
                            double b = a1;
                            a1 = a2;
                            a2 = b;
                        }
                        t.Area.AxisY.ScaleView.Position = a1;
                        t.Area.AxisY.ScaleView.Size = a2 - a1;

                        t.AxY.ViewMaxMsr = a2;
                        t.AxY.ViewMinMsr = a1;
                        t.AxY.ViewMaxPrc = t.ValueToPercent(a2);
                        t.AxY.ViewMinPrc = t.ValueToPercent(a1);
                        break;
                    case PercentModeClass.Percentage:
                        a1 = textBox1.Text == "" ? t.ValueToPercent(t.Area.AxisY.Minimum) : double.Parse(textBox1.Text);
                        a2 = textBox2.Text == "" ? t.ValueToPercent(t.Area.AxisY.Maximum) : double.Parse(textBox2.Text);

                        if (t.DecPlaces >= 0)
                        {
                            a1 = Math.Round(a1, t.DecPlaces);
                            textBox1.Text = a1.ToString();
                        }
                        if (t.DecPlaces >= 0)
                        {
                            a2 = Math.Round(a2, t.DecPlaces);
                            textBox2.Text = a2.ToString();
                        }

                        if (a1 > a2)
                        {
                            double b = a1;
                            a1 = a2;
                            a2 = b;
                        }

                        t.Area.AxisY.ScaleView.Position = t.PercentToValue(a1);
                        t.Area.AxisY.ScaleView.Size = t.PercentToValue(a2) - t.PercentToValue(a1);

                        t.AxY.ViewMaxMsr = t.PercentToValue(a2);
                        t.AxY.ViewMinMsr = t.PercentToValue(a1);
                        t.AxY.ViewMaxPrc = a2;
                        t.AxY.ViewMinPrc = a1;
                        break;
                }
                t.AxY.TBoxMin.Text = a1.ToString();
                t.AxY.TBoxMax.Text = a2.ToString();

                t.Area.AxisY.MajorGrid.Interval = (t.Area.AxisY.ScaleView.ViewMaximum -
                                                   t.Area.AxisY.ScaleView.ViewMinimum)/5;
                //IsYScaledAuto = false;
                YScaleFlag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//манипулирование экстремумами отображения
		private void TextBox1KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter) //TextBox1Leave(sender, e);
				Button4Click(sender, e);
		}

		private void TextBox2KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter) //TextBox2Leave(sender, e);
				Button4Click(sender, e);
		}

		private void TextBox1Leave(object sender, EventArgs e)
		{
			try
			{
				if (textBox2.Text == "") textBox2.Text = GetParam(CurrentParamNumber).AxY.TBoxMax.Text;
				if (textBox1.Text == "" || double.Parse(textBox1.Text) >= double.Parse(textBox2.Text))
				{
					textBox1.Text = GetParam(CurrentParamNumber).AxY.TBoxMin.Text;
				}
				//RadioButton7CheckedChanged(sender, e);
				textBox1.Select(0, 0);
			}
			catch (Exception ex)
			{
				MessageBox.Show(@"Ой " + ex.Message);
				Error = new ErrorCommand("Ошибка вода текста (TextBox1Leave)", ex);
			}
		}

		private void TextBox2Leave(object sender, EventArgs e)
		{
			try
			{
				if (textBox1.Text == "") textBox1.Text = GetParam(CurrentParamNumber).AxY.TBoxMin.Text;
				if (textBox2.Text == "" || double.Parse(textBox1.Text) >= double.Parse(textBox2.Text))
				{
					textBox2.Text = GetParam(CurrentParamNumber).AxY.TBoxMax.Text;
				}
				//RadioButton7CheckedChanged(sender, e);
				textBox2.Select(0, 0);
			}
			catch (Exception ex)
			{
				MessageBox.Show(@"Ой " + ex.Message);
				Error = new ErrorCommand("Ошибка вода текста (TextBox2Leave)", ex);
			}
		}

		//скрываем таблицу данных
		private void Label6Click(object sender, EventArgs e)
		{
			_dataGridHide = !_dataGridHide;
			splitContainer1.Panel2Collapsed = _dataGridHide;
			Label6Move(sender, e);
			//label6.Text = label6.Text == @"          V         " ? "          ^         " : "          V         ";
			label6.Image = _dataGridHide ? Properties.Resources.upl : Properties.Resources.downl;
		}

        //делаем текущим дискретный сигнал по выделению AxCap
        private void AxCapClick(object sender, EventArgs e)
        {
            try
            {
                CurrentParamNumber = int.Parse(((Label) sender).Text);
                dataGridView1.CurrentCell = WantedRow(CurrentParamNumber).Cells[1];
                DatagridSelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

		//Приклеиваем кнопку скрывания датагрида
		private void Label6Move(object sender, EventArgs e)
		{
			label6.Location = new Point(label6.Location.X, splitContainer1.Panel1.Height + 5);
		}

		private void SplitContainer1SplitterMoved(object sender, SplitterEventArgs e)
		{
			Label6Move(sender, e);
		}

		//ресайзим наш контрол
        private void UserControl1Resize(object sender, EventArgs e)
		{
			if (splitContainer2.Width - splitContainer2.SplitterWidth -
				tabControl1.Width > 0)
				splitContainer2.SplitterDistance = splitContainer2.Width - splitContainer2.SplitterWidth -
												   tabControl1.Width;
			Label1Move(sender, e);
		}

		private void Panel1Resize (object sender, EventArgs e)
		{
			panel1.Height = splitContainer1.Panel1.Height - 20;
		}

		//Приклеиваем кнопку скрывания меню
		private void Label1Move(object sender, EventArgs e)
		{
			label1.Location = new Point(splitContainer2.Panel1.Width + 5, label1.Location.Y);
			//label1.Top = splitContainer2.Panel1.Width + 5;
		}

		private void Label1Click(object sender, EventArgs e)
		{
			_mainMenuHide = !_mainMenuHide;
			splitContainer2.Panel2Collapsed = _mainMenuHide;
			Label1Move(sender, e);
			//label1.Text = label1.Text == "\n\n<\n\n\n" ? "\n\n>\n\n\n" : "\n\n<\n\n\n";
			label1.Image = _mainMenuHide ? Properties.Resources.leftl : Properties.Resources.rightl;
		}

		//Минимум выделения приближения вручную по x
		private void Chart1SelectionRangeChanging(object sender, CursorEventArgs e)
		{
			if (e.Axis.AxisName == AxisName.X)
			{
				if (Math.Abs(e.NewSelectionEnd - e.NewSelectionStart) < .03*(e.ChartArea.AxisX.ScaleView.ViewMaximum -
																			 e.ChartArea.AxisX.ScaleView.ViewMinimum) ||
                    (Math.Abs(e.NewSelectionEnd - e.NewSelectionStart) < ScaleXMin))
				{
					e.NewSelectionEnd = e.NewSelectionStart;
				}
			}
		}

		//Обнуление чарта (Режим Монитор)
		private void Button6Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                TimeBegin = (ParamsAnalog[0].Series.Points.Count > 2)
                                ? DateTime.FromOADate(ParamsAnalog[0].Series.Points.Last().XValue)
                                : DateTime.FromOADate(ParamsDiscrete[0].Series.Points.Last().XValue);
                //DialogResult res = 
                //    MessageBox.Show(@"Значения показателей до текущего момента сотрутся. Все-равно продолжить?", @"Установка начальной позиции", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                //if (res == DialogResult.Yes)
                //{
                dateTimePicker1.Value = DateTime.Now;
                foreach (var graphicParam in ParamsAnalog)
                {
                    //graphicParam.Series.Points.Clear();
                    if (graphicParam.Series.Points.Count != 0)
                    {
                        //DataPoint lastOne = graphicParam.Series.Points.Last();
                        double lastVal = graphicParam.Series.Points.Last().YValues[0];
                        double lastTime = graphicParam.Series.Points.Last().XValue;
                        graphicParam.Series.Points.Clear();
                        //graphicParam.Series.Points.AddXY(lastOne.XValue, lastOne.YValues[0]);
                        graphicParam.Series.Points.AddXY(lastTime, lastVal);
                        graphicParam.Area.AxisX.RoundAxisValues();
                    }
                }
                foreach (var graphicParam in ParamsDiscrete)
                {
                    //graphicParam.Series.Points.Clear();
                    if (graphicParam.Series.Points.Count != 0)
                    {
                        //DataPoint lastOne = graphicParam.Series.Points.Last();
                        double lastVal = graphicParam.Series.Points.Last().YValues[0];
                        double lastTime = graphicParam.Series.Points.Last().XValue;
                        graphicParam.Series.Points.Clear();
                        //graphicParam.Series.Points.AddXY(lastOne.XValue, lastOne.YValues[0]);
                        graphicParam.Series.Points.AddXY(lastTime, lastVal);
                        graphicParam.Area.AxisX.RoundAxisValues();
                    }
                }
                //}
                //chart1.Refresh();
                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

        //Обнуление чарта до конкретного момента / подкачка из архива (Режим Монитор)
		private void DateTimePicker1KeyPress(object sender, KeyPressEventArgs e)
		{
            if (e.KeyChar == (char)Keys.Enter) Button28Click(sender, null);
			e.Handled = true;
		}

        private void Button28Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (ParamsAnalog.Last().Series.Points.Count != 0)
            {
                if (dateTimePicker1.Value > DateTime.FromOADate(ParamsAnalog.First().Series.Points.First().XValue))
                {
                    if (dateTimePicker1.Value < DateTime.FromOADate(ParamsAnalog.First().Series.Points.Last().XValue))
                    {
                        //DialogResult res =
                        //    MessageBox.Show(
                        //        @"Значения показателей до указанной даты (" + dateTimePicker1.Value +
                        //        @") сотрутся. Все-равно продолжить?", @"Установка начальной позиции",
                        //        MessageBoxButtons.YesNo,
                        //        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        //if (res == DialogResult.Yes)
                        //{
                        foreach (var p in ParamsAnalog)
                        {
                            if (p.Series.Points.Count > 0)
                            {
                                double tempVal = 0;
                                while (p.Series.Points[0].XValue < dateTimePicker1.Value.ToOADate())
                                {
                                    tempVal = p.Series.Points[0].YValues[0];
                                    p.Series.Points.RemoveAt(0);
                                }
                                p.Series.Points.InsertXY(0, dateTimePicker1.Value.ToOADate(), tempVal);
                            }
                            p.Area.AxisX.RoundAxisValues();
                        }
                        foreach (var p in ParamsDiscrete)
                        {
                            if (p.Series.Points.Count > 0)
                            {
                                double tempVal = 0;
                                while (p.Series.Points.First().XValue < dateTimePicker1.Value.ToOADate())
                                {
                                    tempVal = p.Series.Points[0].YValues[0];
                                    p.Series.Points.RemoveAt(0);
                                }
                                p.Series.Points.InsertXY(0, dateTimePicker1.Value.ToOADate(), tempVal);
                            }
                            p.Area.AxisX.RoundAxisValues();
                        }

                        //DateTime remAt = DateTime.FromOADate(removeAt);
                        //while (ParamsAnalog.First().Dots.First().Time < remAt)
                        //    ParamsAnalog.First().Dots.RemoveAt(0);
                        //}
                        //else
                        //{
                        //    dateTimePicker1.Value = DateTime.FromOADate(ParamsAnalog.First().Series.Points.First().XValue);
                        //}
                    }
                    else
                    {
                        dateTimePicker1.Value = DateTime.FromOADate(ParamsAnalog.First().Series.Points.First().XValue);
                    }
                }
                else
                {
                    DateTime beginning = dateTimePicker1.Value;
                    DateTime ending = DateTime.FromOADate(ParamsAnalog.First().Series.Points.First().XValue);
                    try
                    {
                        if (NeedAPieceOfArchiveSoMuch != null)
                        {
                            NeedAPieceOfArchiveSoMuch(beginning, ending);
                            TimeBegin = beginning;
                            TimeEnd = DateTime.Now;
                            foreach (var t in ParamsAnalog)
                            {
                                t.Dots.Sort(CompareMomentVal);
                                Repaint(t);
                            }
                        }
                        else
                        {
                            dateTimePicker1.Value =
                                DateTime.FromOADate(ParamsAnalog.First().Series.Points.First().XValue);
                            MessageBox.Show(@"Архив не прочитан", @"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Error = new ErrorCommand("Ошибка вода текста (DateTimePicker1Leave)", ex);
                    }
                }
            }
            else
            {
                int beginAt = ParamsAnalog.First().Dots.FindIndex(x => (x.Time > dateTimePicker1.Value));
                for (int i = beginAt; i < ParamsAnalog.First().Dots.Count; i++)
                    ParamsAnalog.First().Series.Points.AddXY(ParamsAnalog.First().Dots[i].ToMomentReal().Time,
                                                             ParamsAnalog.First().Dots[i].ToMomentReal().Mean);
            }
            TimeBegin = (ParamsAnalog[0].Series.Points.Count > 2)
                                        ? DateTime.FromOADate(ParamsAnalog[0].Series.Points[0].XValue)
                                        : DateTime.FromOADate(ParamsDiscrete[0].Series.Points[0].XValue);

            timer1.Start();
        }

        private static int CompareMomentVal(MomentValue x, MomentValue y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    if (x.Time.ToOADate() > y.Time.ToOADate())
                    {
                        return 1;
                    }
                    else
                    {
                        if (Math.Abs(x.Time.ToOADate() - y.Time.ToOADate()) < .00000001)
                            return 0;
                        return -1;
                    }
                }
            }
        }

		//блокировка ввода в комбобоксы
        private void TextHandled(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

		//изменение цвета кнопок, скрывающих панели
		private void Label1MouseEnter(object sender, EventArgs e)
		{
			((Label) sender).BackColor = Color.White;
		}
		private void Label1MouseLeave(object sender, EventArgs e)
		{
			((Label) sender).BackColor = SystemColors.GradientInactiveCaption; //Color.LightSeaGreen;
		}

		//Установка шага прокрутки по оси x
		//private void Chart1ScrollXClick (object sender, EventArgs e)
		//{
		//    foreach (var t in ParamsAnalog)
		//    {
		//        t.Area.AxisX.ScaleView.SmallScrollMinSizeType = t.Area.AxisX.ScaleView.SizeType;//DateTimeIntervalType.Seconds;
		//        t.Area.AxisX.ScaleView.SmallScrollMinSize = t.Area.AxisX.ScaleView.Size / 20.0;//1.0;
		//    }
		//}

		//всплывающая подсказка с отображением даты и включение CursorY
		private void Chart1MouseDown(object sender, MouseEventArgs e)
		{
		    try
			{
				//int h2 = Convert.ToInt32((ParamsAnalog.First().Area.Position.Height * chart1.Height) / 100);
				//int h1 = h2 - AxisXLabelFontHeight;
				int h2 = ParamsAnalog[0].AxY.Ax.Height;
				int h1 = h2 - AxSummand;
				int h4 = (int)(((ParamsDiscrete[0].Area.Position.Bottom - 1) * chart1.Height) / 100);
				int h3 = Math.Min((int) (((ParamsDiscrete[0].Area.Position.Bottom - 4)*chart1.Height)/100), h4 -AxSummand);
				if ((e.Y > h1 && e.Y < h2) /*||
					((e.Y < (int)((splitContainer1.Panel1.Height - _hscrollb.Height - 6) * .99)) &&
					 e.Y > (splitContainer1.Panel1.Height - AxisXLabelFontHeight - _hscrollb.Height - 3))*/
					|| (e.Y > h3 && e.Y < h4))
				{
					double scaleViewXPart = e.X;
					double scaleViewPerCent = 100 * scaleViewXPart / chart1.Width;
					if (scaleViewPerCent <= 100 && scaleViewPerCent >= 0)
					{
						string temp = string.Format("{0:d}", DateTime.FromOADate(
														chart1.ChartAreas.First().AxisX.PositionToValue(scaleViewPerCent)));
						toolTip1.SetToolTip(chart1, temp);
					}
				}

                if (e.Button == MouseButtons.Right && ParamsAnalog.Count > 1)
                {
                   //ParamsAnalog[0].Area.CursorY.SelectionEnd = double.NaN;
                    ParamsAnalog[0].Area.CursorY.SelectionStart = (ParamsAnalog[0].Area.AxisY.ScaleView.ViewMaximum -
                                                                 ParamsAnalog[0].Area.AxisY.ScaleView.ViewMinimum) *
                                                                (ParamsAnalog[0].AxY.Ax.Height - AxSummand - e.Y) /
                                                                (ParamsAnalog[0].AxY.Ax.Height - 2 * AxSummand) +
                                                                ParamsAnalog[0].Area.AxisY.ScaleView.ViewMinimum;
                }
			}
			catch (Exception ex)
			{
				Error = new ErrorCommand("(Chart1MouseDown)", ex);
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}

		private void Chart1MouseMove (object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				double selectionEnd = (ParamsAnalog[0].Area.AxisY.ScaleView.ViewMaximum -
																 ParamsAnalog[0].Area.AxisY.ScaleView.ViewMinimum)*
																(ParamsAnalog[0].AxY.Ax.Height - AxSummand - e.Y)/
																(ParamsAnalog[0].AxY.Ax.Height - 2*AxSummand) +
																ParamsAnalog[0].Area.AxisY.ScaleView.ViewMinimum;
				if (Math.Abs(selectionEnd - ParamsAnalog[0].Area.CursorY.SelectionStart) <
					.05*(ParamsAnalog[0].Area.AxisY.ScaleView.ViewMaximum -
						 ParamsAnalog[0].Area.AxisY.ScaleView.ViewMinimum) /*||
					(Math.Abs(selectionEnd - ParamsAnalog[0].Area.CursorY.SelectionStart) < 1.0/10)*/)
				{
					ParamsAnalog[0].Area.CursorY.SelectionEnd = ParamsAnalog[0].Area.CursorY.SelectionStart;
				}
				else
				{
					ParamsAnalog[0].Area.CursorY.SelectionEnd = selectionEnd;
				}
			}
		}

		//При нажатии на шкалу Y соответствующий график становится текущим
		private void AxMouseDown(object sender, EventArgs e)
		{
			int currentNum = -42;

			switch (sender.GetType().ToString())
			{
				case "System.Windows.Forms.Label":
					currentNum = int.Parse(((Label)sender).Parent.Name);
					break;
				case "System.Windows.Forms.PictureBox":
					currentNum = int.Parse(((PictureBox)sender).Parent.Name);
					break;
				case "System.Windows.Forms.Panel":
					currentNum = int.Parse(((Panel) sender).Name);
					break;
			}

			//dataGridView1.CurrentCell = dataGridView1.Rows[currentNum - 1].Cells[1];
			dataGridView1.CurrentCell = WantedRow(currentNum).Cells[1];

			DatagridSelectionChanged(null, null);
		}

		//При двойном щелчке по шкале Y соответствующий график разворачивается по Y
		private void AxMouseDouble(object sender, EventArgs e)
		{
			GraphicParam curP = GetParam(CurrentParamNumber);

			foreach (var t in curP.AxY.AxesOverlay)
			{
				GraphicParam ixrP = GetParam(t);
				ixrP.AxY.Ax.Top = 0;
				ixrP.AxY.Ax.Height = ParamsAnalog.First().AxY.Ax.Height;
				ixrP.Area.Position.Y = 100f * (ixrP.AxY.Ax.Top + AxSummand) / chart1.Height;
			}
		}
		
		//делает активным ту ось, по элементу которой был клик
		private void AxControlDetect(object sender, EventArgs e)
		{
			switch (sender.GetType().ToString())
			{
                case "System.Windows.Forms.Label":
					AxMouseDown(((Label) sender).Parent, null);
					break;
				case "System.Windows.Forms.PictureBox":
					AxMouseDown(((PictureBox)sender).Parent.Parent, null);
					break;
				case "System.Windows.Forms.TextBox":
					AxMouseDown(((TextBox)sender).Parent, null);
					break;
			}
		}

		public string AddParam(string code, string name = "", string subname = "",
							   string dataType = "real", double min = 0, double max = 0, string units = "", int decPlaces = -1)
		{
			try
			{
				try
				{
					bool flag = true;
					var newParam = new GraphicParam();
					foreach (var graphicParam in (ParamsAnalog))
					{
						if (graphicParam.Code == code)
						{
							flag = false;
							newParam = graphicParam;
							break;
						}
                    }
                    if (dataType.ToDataType() == DataType.Integer)
                    {
                        dataType = "real";
                        newParam.DataTypeString = "Целочисленный";
                    }
                    else if (dataType.ToDataType() == DataType.Real)
                    {
                        newParam.DataTypeString = "Аналоговый";
                    }
                    else newParam.DataTypeString = "Дискретный";
					if (flag)
						foreach (var graphicParam in (ParamsDiscrete))
						{
							if (graphicParam.Code == code)
							{
								flag = false;
								newParam = graphicParam;
								break;
							}
						}

					//Если у нас еще не было такого параметра в списке, добавляем. Иначе чистим его.
					if (flag)
					{
						var p = new GraphicParam
									{
										Code = code,
										Name = name,
										SubName = subname,
                                        PtkMin = min,
                                        PtkMax = max,
										Min = Math.Min(min, max),
                                        Max = Math.Max(min, max),
										//Precision = precision,
										Units = units,
										DataType = dataType,
										Index = ParamsCount() + 1,
                                        DataTypeString = newParam.DataTypeString
									};
						if (p.Code != "BckgrndAnalog" && p.Code != "BckgrndDiscrete")
						{
                            try
                            {
                                p.Series.Color = Colorz.GetColor();
                            }
                            catch (Exception ex)
                            {
                                Error = new ErrorCommand("Не удалось загрузить параметр. Превышено максимальное число параметров (80)", ex);
                                return ErrorMessage;
                            }
						}
						else
						{
							p.Series.Color = Color.Transparent;
							p.IsVisible = false;
						}
						switch (p.DataTypeD)
						{
							case DataType.Real:
								ParamsAnalog.Add(p);
								p.Series.ChartType = SeriesChartType.FastLine;
								p.Series.EmptyPointStyle.Color = p.Series.Color;
						        p.DecPlaces = decPlaces == -1 ? DecPlacesDefault : decPlaces;
								break;
							case DataType.Boolean:
								ParamsDiscrete.Add(p);
								p.Series.ChartType = SeriesChartType.FastLine;
								p.Series.EmptyPointStyle.Color = p.Series.Color;
								break;
						}
						PrepareParam(p);

					    dataGridView1.Rows[p.Index-1].Cells["Тип данных"].Value = p.DataTypeString;
					}
					else
					{
						newParam.Name = name;
						newParam.SubName = subname;
						newParam.DataType = dataType;
					    newParam.PtkMin = min;
                        newParam.PtkMax = max;
                        if (min < max)
                        {
                            newParam.Min = min;
                            newParam.Max = max;
                        }
                        else
                        {
                            newParam.Min = max;
                            newParam.Max = min;
                        }
					    //newParam.Precision = precision;
						newParam.Units = units;

						int currentNumber = newParam.Index;
						chart1.Series[currentNumber].Points.Clear();
						newParam.Dots.Clear();
						Gethering();
					}
				}
				catch (OverflowException)
				{
					MessageBox.Show("15");
				}
				return "";
			}
			catch (Exception exception)
			{
				Error = new ErrorCommand("Не удалось загрузить параметр (AddParam)", exception);
				//MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
				return ErrorMessage;
			}
		}

		public string DeleteParam(string code)
		{
			try
			{
				GraphicParam victimGraph = ParamsAnalog.Find(x => (x.Code == code)) ??
									  ParamsDiscrete.Find(x => (x.Code == code));
				int victimNumber = victimGraph.Index;

				//chart1.Series.RemoveAt(victimNumber + 1);
				//chart1.ChartAreas.RemoveAt(victimNumber + 1);
				//dataGridView1.Rows.RemoveAt(victimNumber - 1);
				chart1.Series.Remove(victimGraph.Series);
				chart1.ChartAreas.Remove(victimGraph.Area);
				if (victimGraph.DataTypeD == DataType.Real)
				{
					ParamsAnalog.Remove(victimGraph);
					if (_chartTypeMode == GraphicTypeMode.Analog)
					{
						if (ParamsAnalog.Count == 1)
						{
							_chartTypeMode = GraphicTypeMode.Empty;
							ParamsAnalog.First().Series.Points.Clear();
							ParamsDiscrete.First().Area.CursorX.LineColor = Color.Transparent;
							ParamsDiscrete.First().Area.CursorX.SelectionColor = Color.Transparent;
							ParamsAnalog.First().Area.CursorX.LineColor = Color.Transparent;
							//ParamsAnalog.First().Area.CursorY.LineColor = Color.Transparent;
							ParamsAnalog.First().Area.CursorX.SelectionColor = Color.Transparent;
							ParamsAnalog.First().Area.CursorX.IsUserEnabled = false;
							ParamsAnalog.First().Area.CursorX.IsUserSelectionEnabled = false;
							//ParamsAnalog.First().Area.CursorY.IsUserEnabled = false;
							//ParamsAnalog.First().Area.CursorY.IsUserSelectionEnabled = false;
							button4.Enabled = false;
							//radioButton6.Enabled = false;
							//radioButton7.Enabled = false;
							textBox1.Enabled = false;
							textBox2.Enabled = false;
							//button8.Enabled = false;
							//button9.Enabled = false;
						}
					}
					else
					{
						if (ParamsAnalog.Count == 1)
						{
							_chartTypeMode = GraphicTypeMode.Discrete;
							ParamsAnalog.First().Series.Points.Clear();
							ParamsAnalog.First().Area.CursorX.LineColor = Color.Transparent;
							//ParamsAnalog.First().Area.CursorY.LineColor = Color.Transparent;
							ParamsAnalog.First().Area.CursorX.SelectionColor = Color.Transparent;
							ParamsAnalog.First().Area.CursorX.IsUserEnabled = false;
							ParamsAnalog.First().Area.CursorX.IsUserSelectionEnabled = false;
							//ParamsAnalog.First().Area.CursorY.IsUserEnabled = false;
							//ParamsAnalog.First().Area.CursorY.IsUserSelectionEnabled = false;
							//button4.Enabled = false;
							//radioButton6.Enabled = false;
							//radioButton7.Enabled = false;
							//textBox1.Enabled = false;
							//textBox2.Enabled = false;
						}
					}

					//int overlayNmr = tempGr.AxesOverlay.Count;
					int overlayNext = VisibleOverlaySearch(victimGraph);
					GraphicParam newTopGraph = GetParam(overlayNext);
					//if (tempGr.IsVisible && overlayNext < 1)
					if (victimGraph.IsVisible)
					{
						if (overlayNext < 1)
						{
							if (!victimGraph.AxY.IsHidden) Noas--;
							if (victimGraph.AxY.AxesOverlay.Count == 1)
							{
								chart1.Controls.Remove(victimGraph.AxY.Ax);
								AxesY.Remove(victimGraph.AxY);
							}
							else
							{
								victimGraph.AxY.Ax.Visible = false;
								int df = victimGraph.AxY.AxesOverlay.Find(x => x != victimGraph.Index);
								newTopGraph = GetParam(df);
								victimGraph.AxY.UpperParam = newTopGraph;
							}
						}
						else
						{
							victimGraph.AxY.UpperParam = newTopGraph;
						}
					}

					if (victimGraph.AxY.AxesOverlay.Count == 2)
					{
						victimGraph.AxY.IsOverlayed = false;
					}

					victimGraph.AxY.AxesOverlay.Remove(victimGraph.Index);
					//for (int index = 0; index < victimGraph.AxY.AxesOverlay.Count; index++)
					//{
					//    if (victimGraph.AxY.AxesOverlay[index] >= victimNumber) victimGraph.AxY.AxesOverlay[index]--;
					//}

					if (newTopGraph != null)
						foreach (var t in newTopGraph.AxY.AxesOverlay)
						{
							//dataGridView1[0, t - 1].Style.BackColor = newTopGraph.Series.Color;
							//dataGridView1[0, t - 1].Style.SelectionBackColor = newTopGraph.Series.Color;
							WantedRow(t).Cells[0].Style.BackColor = newTopGraph.Series.Color;
							WantedRow(t).Cells[0].Style.SelectionBackColor = newTopGraph.Series.Color;
							WantedRow(t).Cells["Group"].Value = newTopGraph.AxY.UpperParam.Index;
						}
					else
						foreach (var t in victimGraph.AxY.AxesOverlay)
						{
							WantedRow(t).Cells[0].Style.BackColor = Color.White;
							WantedRow(t).Cells[0].Style.SelectionBackColor = Color.White;
							WantedRow(t).Cells["Group"].Value = newTopGraph.AxY.UpperParam.Index;
						}
					dataGridView1.Sort(new DatagridColorSort());
				}
				else
				{
					chart1.Controls.Remove(victimGraph.AxCap);
					ParamsDiscrete.Remove(victimGraph);
					//foreach (var param in ParamsAnalog)
					//{
					//    AxesRankingSize(param);
					//}

					if (_chartTypeMode == GraphicTypeMode.Discrete)
					{
						if (ParamsDiscrete.Count == 1)
						{
							_chartTypeMode = GraphicTypeMode.Empty;
							ParamsDiscrete.First().Area.CursorX.LineColor = Color.Transparent;
							ParamsDiscrete.First().Area.CursorX.SelectionColor = Color.Transparent;
							//ParamsDiscrete.First().Area.AxisX.LabelStyle.ForeColor = Color.Transparent;
							//ParamsDiscrete.First().Area.AxisX.MajorGrid.LineColor = Color.Transparent;
							ParamsAnalog.First().Area.CursorX.LineColor = Color.Transparent;
							//ParamsAnalog.First().Area.CursorY.LineColor = Color.Transparent;
							ParamsAnalog.First().Area.CursorX.SelectionColor = Color.Transparent;
							ParamsDiscrete.First().Area.CursorX.IsUserEnabled = false;
							ParamsDiscrete.First().Area.CursorX.IsUserSelectionEnabled = false;
							button4.Enabled = false;
							//radioButton6.Enabled = false;
							//radioButton7.Enabled = false;
							textBox1.Enabled = false;
							textBox2.Enabled = false;
							//button8.Enabled = false;
							//button9.Enabled = false;
						}
					}
					else
					{
						if (ParamsAnalog.Count == 1)
						{
							_chartTypeMode = GraphicTypeMode.Analog;
							ParamsDiscrete.First().Area.CursorX.LineColor = Color.Transparent;
							ParamsDiscrete.First().Area.CursorX.SelectionColor = Color.Transparent;
							ParamsDiscrete.First().Area.CursorX.IsUserEnabled = false;
							ParamsDiscrete.First().Area.CursorX.IsUserSelectionEnabled = false;
						}
					}
					if (victimGraph.IsVisible) Nods--;
				}

				dataGridView1.Rows.Remove(WantedRow(victimNumber));
                
				int anParamCounter = 0;
				foreach (var t in ParamsAnalog)
				{
					if (t.Index > victimNumber)
					{
						t.Index--;
						dataGridView1["№ в таблице", t.Index - 1].Value = t.Index;
						//WantedRow(t.Index+1).Cells["№ в таблице"].Value = t.Index;
					}

					AreaRankingX(t);
					if (t.IsVisible && !t.AxY.IsHidden && t.Index == t.AxY.UpperParam.Index)
					{
						t.AxY.Ax.Left = anParamCounter * AxWidth;
						anParamCounter++;
					}
				}

				int discrParamCounter = 2;
				foreach (var t in ParamsDiscrete)
				{
					if (t.Index > victimNumber)
					{
						t.Index--;
						dataGridView1["№ в таблице", t.Index - 1].Value = t.Index;
						//WantedRow(t.Index).Cells["№ в таблице"].Value = t.Index;
						t.AxCap.Text = t.Index.ToString();
					}
					AreaDisRankingY(t, discrParamCounter);
					if (t.IsVisible) discrParamCounter++;
					AreaRankingX(t);
					//AreaDisRankingHeight(t);
				}
				foreach (var a in AxesY)
				{
					if (int.Parse(a.Ax.Name) >= victimNumber)
					{
						a.AxCap.Text = (a.UpperParam.Index).ToString();
						a.Ax.Name = (a.UpperParam.Index).ToString();
						a.PercentMode = a.UpperParam.AxY.PercentMode;
					}
					for (int index = 0; index < a.AxesOverlay.Count; index++)
					{
						if (a.AxesOverlay[index] > victimNumber) a.AxesOverlay[index]--;
					}
				}
				AreaDisFirstHeight();
				if (victimGraph.DataTypeD == DataType.Boolean)
				{
					foreach (var a in AxesY)
					{
						AxesRankingSize(a.UpperParam);
					}
						foreach (var param in ParamsAnalog)
						{
							AreaAnRankingY(param);
							AreaAnRankingHeight(param);
						}
				}

				//_hscrollb.Width = Convert.ToInt32(ParamsAnalog.First().Area.Position.Width*chart1.Size.Width/100);
				//_buttonScaleDrop.Width = splitContainer1.Panel1.Size.Width - _hscrollb.Width;//chart1.Size.Width - _hscrollb.Width;
				//_hscrollb.Location =
				//    new Point(_buttonScaleDrop.Width,
				//              splitContainer1.Panel1//chart1
				//              .Size.Height - 20);
				DownerScrollRankingSize();

                /*//---
                if (ParamsAnalog.Count > 6)
				//---{
					IntervalCount = ParamsAnalog.Count == 7 ? 9 : 8;
				}
				else IntervalCount = 10;
                ///---*/

				//SetScalePosition1(null, null);

				if (dataGridView1.CurrentRow != null) label9.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
				else
				{
					label9.Text = "";
					label9.BackColor = Color.White;
					label13.Text = "";
					label13.BackColor = Color.White;
					label19.Text = "";
					label19.BackColor = Color.White;
					label16.Text = "";
					label16.BackColor = Color.White;
					label17.Text = "";
					label17.BackColor = Color.White;
					button1.ForeColor = Color.Black;
					button5.ForeColor = Color.Black;
					CurrentParamNumber = 0;
				}

				//CurrentParamNumber = int.Parse(dataGridView1.CurrentRow.Cells[1].Value.ToString());
				DatagridSelectionChanged(null, null);
				RadioButton4CheckedChanged(null, null);
				if (dataGridView1.CurrentRow != null) dataGridView1.CurrentRow.Selected = true;
				//if (GetParam(CurrentParamNumber).DataTypeD != DataType.Boolean) ToTopOverlay(GetParam(CurrentParamNumber));
				return "";
			}
			catch (Exception exception)
			{
				//throw exception;
				Error = new ErrorCommand("Не удалось удалить параметр (DeleteParam)", exception);
				MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
				return ErrorMessage;
			}
		}

		//Поиск хотя бы одного видимого графика, слепленного с данным
		private int VisibleOverlaySearch (GraphicParam graphic)
		{
			int i = -42;
			foreach (var v in graphic.AxY.AxesOverlay)
			{
				if (v!= graphic.Index && GetParam(v).IsVisible) i = v;
			}
			return i;
		}

		public string ClearParams()
		{
			try
			{
				for (int index = 0; index < ParamsAnalog.Count; index++)
				{
					ParamsAnalog.RemoveAt(0);
				}
				for (int index = 0; index < ParamsDiscrete.Count; index++)
				{
					ParamsDiscrete.RemoveAt(0);
				}
				return "";
			}
			catch (Exception exception)
			{
				Error = new ErrorCommand("Не удалось очистить параметры (ClearParams)", exception);
				MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
				return ErrorMessage;
			}
		}

		public int ParamsCount()
		{
			return ParamsAnalog.Count + ParamsDiscrete.Count - 2;
		}

		public GraphicParam GetParam(int num)
		{
			GraphicParam temp = ParamsAnalog.Find(x => (x.Index == num)) ??
								ParamsDiscrete.Find(x => (x.Index == num));
			if (num <= ParamsCount() + 1) return temp;
			return null;
		}

		public GraphicParam GetParamByCode(string code)
		{
			try
			{
				GraphicParam temp = ParamsAnalog.Find(x => (x.Code == code)) ??
									ParamsDiscrete.Find(x => (x.Code == code));
				return temp;
			}
			catch (Exception exception)
			{
				Error = new ErrorCommand("Не удалось извлечь параметр (GetParamByCode)", exception);
				MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
				return null;
			}
		}

        //SQL/Access
		public string SetDatabase(string databaseType, string databaseFile)
		{
			try
			{
				DatabaseType = databaseType == "Access" ? DatabaseType.Access : DatabaseType.SqlServer;
				ConnectionString = databaseFile;
				return "";
			}
			catch (Exception exception)
			{
				Error = new ErrorCommand("SetDatabase error", exception);
				return ErrorMessage;
			}
		}
        
		public string LoadParams(string stSql)
		{
			try
			{
                using (var reader = new ReaderAdo(ConnectionString, stSql))
				//       SELECT  Null As Num, ‘Project1’ As Project, CodeParam As Code, “ &  _
				// “‘real’ as DataType, ED as Units,Min,Max,Comment As Tag  “ & _
				//“FROM MyTable WHERE SetNum=1 ORDER BY Num, Code
				{while (reader.Read())
				{
					var lso = new GraphicParam
								  {
									  //Project = reader.GetString("Project"),
									  Code = reader.GetString("Code"),
									  DataTypeD = reader.GetString("DataType").ToDataType(),
									  Units = reader.GetString("Units"),
									  Min = reader.GetDouble("Min"),
									  Max = reader.GetDouble("Max"),
									  Tag = reader.GetString("Tag"),
									  Comment = reader.GetString("Comment"),
									  Id = reader.GetInt("Id"),
									  Name = reader.GetString("Name"),
									  SubName = reader.GetString("SubName")
								  };
					switch (lso.DataTypeD)
					{
						case DataType.Real:
							ParamsAnalog.Add(lso);
							break;
						case DataType.Boolean:
							ParamsDiscrete.Add(lso);
							break;
					}
				}}
				//reader.Close();
				return "";
			}
			catch (Exception exception)
			{
				Error = new ErrorCommand("Траблы с LoadParams", exception);
				return ErrorMessage;
			}
		}

		public string LoadValues(string stSql)
		{
			try
			{
                using (var reader = new ReaderAdo(ConnectionString, stSql))
                {
                    Cursor = Cursors.WaitCursor;
                    while (reader.Read())
                    {
                        int nd = reader.GetIntNull("Nd").HasValue ? reader.GetIntNull("Nd").Value : 0;
                        var dot = new MomentReal(reader.GetTime("Time"), reader.GetDouble("Val"), nd);
                        int id = reader.GetIntNull("Id").HasValue ? reader.GetIntNull("Id").Value : 0;
                        if (reader.GetString("code") == null || reader.GetString("code") == "")
                        {
                            if (id != 0)
                                //if (reader.GetString("Id") != null)
                                //ParamsAnalog.Find(x => (x.Id == int.Parse(reader.GetString("Id")))).Dots.Add(dot);
                                //GetParam(int.Parse(reader.GetString("Id")));
                            {
                                GraphicParam curP = ParamsAnalog.Find(x => (x.Id == id)) ??
                                                    ParamsDiscrete.Find(x => (x.Id == id));
                                if (curP != null) curP.Dots.Add(dot);
                            }
                        }
                        else
                        {
                            //ParamsAnalog.Find(x => (x.Code == reader.GetString("code"))).Dots.Add(dot);
                            GetParamByCode(reader.GetString("code")).Dots.Add(dot);
                        }
                    }
                }
				return "";
			}
			catch (Exception e)
			{
				Cursor = Cursors.Default;
				Error = new ErrorCommand("Проблема с загрузкой значений в параметр (LoadValues)", e);
				MessageBox.Show(e.Message + "\n===\n" + e.StackTrace);
				return ErrorMessage;
			}
		}

        public string LoadValues(string stSql, DateTime timeBegin, DateTime timeEnd)
        {
            try
            {
                using (var reader = new ReaderAdo(ConnectionString, stSql))
                {
                    Cursor = Cursors.WaitCursor;
                    while (reader.Read())
                    {
                        GraphicParam curP = null;

                        int id = reader.GetIntNull("Id").HasValue ? reader.GetIntNull("Id").Value : 0;
                        if (reader.GetString("code") == null || reader.GetString("code") == "")
                        {
                            if (id != 0)
                                curP = ParamsAnalog.Find(x => (x.Id == id)) ??
                                       ParamsDiscrete.Find(x => (x.Id == id));
                        }
                        else
                            curP = GetParamByCode(reader.GetString("code"));

                        if (curP != null)
                        {
                            if (reader.GetTime("Time") <= timeEnd)
                            {
                                int nd = reader.GetIntNull("Nd").HasValue ? reader.GetIntNull("Nd").Value : 0;
                                var time = (reader.GetTime("Time") >= timeBegin) ? reader.GetTime("Time") : timeBegin;
                                var dot = new MomentReal(time, reader.GetDouble("Val"), nd);

                                if ((reader.GetTime("Time") > timeBegin) || (curP.Dots.Count == 0))
                                    curP.Dots.Add(dot);
                                else
                                    curP.Dots[curP.Dots.Count - 1] = dot;
                            }
                            else
                            {
                                if (curP.Dots.Count > 0)
                                {
                                    if (curP.Dots[curP.Dots.Count - 1].Time < timeEnd)
                                    {
                                        int nd = reader.GetIntNull("Nd").HasValue ? reader.GetIntNull("Nd").Value : 0;
                                        double val;
                                        if (curP.DataTypeD == DataType.Real)
                                        {
                                            double a = ((MomentReal) curP.Dots[curP.Dots.Count - 1]).Mean;

                                            val = (reader.GetDouble("Val") - a);
                                            val *=
                                                timeEnd.Subtract(curP.Dots[curP.Dots.Count - 1].Time).TotalMilliseconds/
                                                reader.GetTime("Time").Subtract(curP.Dots[curP.Dots.Count - 1].Time).
                                                    TotalMilliseconds;
                                            val += a;
                                        }
                                        else
                                            val = Convert.ToDouble(((MomentBoolean) curP.Dots[curP.Dots.Count - 1]).Mean);

                                        var dot = new MomentReal(timeEnd, val, nd);
                                        curP.Dots.Add(dot);
                                    }
                                }
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                Cursor = Cursors.Default;
                Error = new ErrorCommand("Проблема с загрузкой значений в параметр (LoadValues)", e);
                MessageBox.Show(e.Message + "\n===\n" + e.StackTrace);
                return ErrorMessage;
            }
        }

        //public void LoadKrestValues(string stSql)
        //{
        //    try
        //    {
        //    }
        //    catch (Exception e)
        //    {
        //        Error = new ErrorCommand("Траблы с LoadKrestValues", e);
        //    }
        //}

        //Графическая отрисовка параметров
		public string Repaint(GraphicParam t)
		{
			try
			{
				if (t.Dots.Count == 0)
				{
					Cursor = Cursors.Default;
					GraphicParam tempA = GetParamByCode("BckgrndAnalog");
					GraphicParam tempD = GetParamByCode("BckgrndDiscrete");

					if (t.DataTypeD == DataType.Boolean)
					{
						tempD.AddValues(new MomentReal(TimeBegin, 1, 1));
						tempD.AddValues(new MomentReal(TimeEnd, 1, 1));
						PaintBckgrnd(tempD);
						t.Area.BackColor = Color.Transparent;
						if (tempA.Series.Points.Count < 1)
						if (DefaultScale == "full") radioButton4.Checked = true;
						else radioButton5.Checked = true;

						tempD.Area.CursorX.IsUserEnabled = true;
						tempD.Area.CursorX.IsUserSelectionEnabled = true;
						tempD.Area.CursorX.LineColor = Color.Red;
						tempD.Area.CursorX.LineWidth = 2;
						tempD.Area.CursorX.SelectionColor = SystemColors.Highlight;
					}
					else
					{
						if ((t.DataTypeD == DataType.Real || t.DataTypeD == DataType.Integer))
						{
							tempA.AddValues(new MomentReal(TimeBegin, 1, 1));
							tempA.AddValues(new MomentReal(TimeEnd, 1, 1));
							PaintBckgrnd(tempA);

							if (tempD.Series.Points.Count < 1)
							if (DefaultScale == "full") radioButton4.Checked = true;
							else radioButton5.Checked = true;

							tempA.Area.CursorX.IsUserEnabled = true;
							tempA.Area.CursorX.IsUserSelectionEnabled = true;
							//tempA.Area.CursorY.IsUserEnabled = true;
							tempA.Area.CursorX.LineColor = Color.Red;
							tempA.Area.CursorX.LineWidth = 2;
							tempA.Area.CursorX.SelectionColor = SystemColors.Highlight;
							//tempA.Area.CursorY.Interval = .01;
							//tempD.Area.CursorY.IsUserEnabled = true;
							//tempD.Area.CursorY.IsUserSelectionEnabled = true;
						}
					}
					return "";
				}

                //t.Series.Points.Clear();
                //t.IsUpdated = true;
                ////if (t.Dots.Count > 1) t.IsUpdated = true;
                ////else t.IsUpdated = false;
                ////добавляем точку в конец отображаемого периода (режим аналайзер)
                //if (t.Dots.Last().Time < TimeEnd)
                //    t.Dots.Add(new MomentReal(TimeEnd, t.Dots.Last().ToMomentReal().Mean, 0));
                //if (t.Dots.First().Time > TimeBegin)
                //    t.Dots.Insert(0, new MomentReal(TimeBegin, t.Dots.First().ToMomentReal().Mean, 1));
                //_isInBreak = t.Dots.First().Nd != 0;
                //t.Series.Points.AddXY(t.Dots[0].Time, t.Dots[0].ToMomentReal().Mean);
                //if (_isInBreak) t.Series.Points.First().IsEmpty = true;
                t.NumberOfFirstWaiting = 1;
                t.NumberOfLastViewed = 0;
                ////t.Series.Enabled = false;
                //chart1.Series.Remove(t.Series);
                //DrawNewStuff(t);
                ////t.Series.Enabled = true;
                //chart1.Series.Add(t.Series);

                chart1.Series.Remove(t.Series);
                RedrawParam(t, TimeBegin, TimeEnd);
                chart1.Series.Add(t.Series);
                
				if (t.Max == t.Min)
				{
					if (t.DataTypeD != DataType.Boolean)
					{
						t.Max = t.MaxVal() * 1.03 - t.MinVal() * 0.03;
						t.Min = t.MinVal() * 1.03 - t.MaxVal() * 0.03;
						if (t.Max == t.Min)
						{
							t.Max += 0.5;
							t.Min -= 0.5;
						}
						//dataGridView1.Rows[t.Index - 1].Cells["Макс."].Value = t.Max.ToString();
						//dataGridView1.Rows[t.Index - 1].Cells["Мин."].Value = t.Min.ToString();
						//WantedRow(t.Index).Cells["Макс."].Value = t.Max.ToString();
						//WantedRow(t.Index).Cells["Мин."].Value = t.Min.ToString();
                        WantedRow(t.Index).Cells["Макс."].Value = t.PtkMax.ToString();
                        WantedRow(t.Index).Cells["Мин."].Value = t.PtkMin.ToString();
                        //switch (t.PercentMode)
                        //{
                        //    case PercentModeClass.Absolute:
                        //        t.AxY.TBoxMax.Text = t.Max.ToString();
                        //        t.AxY.TBoxMin.Text = t.Min.ToString();
                        //        break;
                        //    case PercentModeClass.Percentage:
                        //        t.AxY.TBoxMin.Text = t.ValueToPercent(t.Min).ToString();
                        //        t.AxY.TBoxMax.Text = t.ValueToPercent(t.Max).ToString();
                        //        break;
                        //}

                        //t.AxY.ViewMaxMsr = t.Max;
                        //t.AxY.ViewMinMsr = t.Min;
                        //t.AxY.ViewMaxPrc = 100;//t.ValueToPercent(t.Max);
                        //t.AxY.ViewMinPrc = 0;//t.ValueToPercent(t.Min);

                        //textBox1.Text = GetParam(CurrentParamNumber).AxY.TBoxMin.Text;
						//textBox2.Text = GetParam(CurrentParamNumber).AxY.TBoxMax.Text;
						textBox1.Text = t.AxY.TBoxMin.Text;
						textBox2.Text = t.AxY.TBoxMax.Text;
					}
					else
					{
						t.Max = 1;
						t.Min = 0;
						//WantedRow(t.Index).Cells["Макс."].Value = t.Max.ToString();
						//WantedRow(t.Index).Cells["Мин."].Value = t.Min.ToString();
                        WantedRow(t.Index).Cells["Макс."].Value = t.PtkMax.ToString();
                        WantedRow(t.Index).Cells["Мин."].Value = t.PtkMin.ToString();
					}
				}
                
				if (t.DataTypeD == DataType.Real)
				{
					GraphicParam tempA = GetParamByCode("BckgrndAnalog");
					GraphicParam tempD = GetParamByCode("BckgrndDiscrete");
					//if (tempA.Dots.Count == 0)
					//if (Noas == 1)
					
                    if (tempA.Series.Points.Count == 0)
                    {
                        if (tempA.Dots.Find(x => x.Time == t.Dots[0].Time) == null) tempA.AddValues(t.Dots[0]);
                        if (tempA.Dots.Find(x => x.Time == t.Dots.Last().Time) == null) tempA.AddValues(t.Dots.Last());
                        //if (tempD.Dots.Count < 2)
                        //    foreach (var dot in t.Dots)
                        //    {
                        //        tempD.AddValues(dot.Time, 1);
                        //    }
                        if (tempD.Dots.Count == 0)
                        {
                            tempD.AddValues(t.Dots[0].Time, 1);
                            tempD.AddValues(t.Dots.Last().Time, 1);
                        }
                        PaintBckgrnd(tempA);
                        if (tempD.Series.Points.Count < 1)
                            if (DefaultScale == "full") radioButton4.Checked = true;
                            else radioButton5.Checked = true;

                        tempA.Area.CursorX.IsUserEnabled = true;
                        tempA.Area.CursorX.IsUserSelectionEnabled = true;
                        tempA.Area.CursorX.LineColor = Color.Red;
                        tempA.Area.CursorX.LineWidth = 2;
                        tempA.Area.CursorX.SelectionColor = SystemColors.Highlight;
                        tempA.Area.CursorY.Interval = .001;
                        tempA.Area.CursorY.IntervalType = DateTimeIntervalType.Number;
                        //tempA.Area.CursorY.IsUserEnabled = true;
                        //tempA.Area.CursorY.IsUserSelectionEnabled = true;
                    }

				    t.AxY.ViewMaxMsr = t.Max;
                    t.AxY.ViewMinMsr = t.Min;
                    t.AxY.ViewMaxPrc = 100;
                    t.AxY.ViewMinPrc = 0;
				}
				else
				{
					GraphicParam tempA = GetParamByCode("BckgrndAnalog");
					GraphicParam tempD = GetParamByCode("BckgrndDiscrete");
					//if (ParamsDiscrete.Count == 2)
					//if (tempD.Dots.Count == 0)
					//if (Nods == 1)
					if (tempD.Series.Points.Count == 0)
					{
						if (tempD.Dots.Find(x => x.Time == t.Dots[0].Time) == null) tempD.AddValues(t.Dots[0]);
						if (tempD.Dots.Find(x => x.Time == t.Dots.Last().Time) == null) tempD.AddValues(t.Dots.Last());
						//if (tempA.Dots.Count < 2)
						//    foreach (var dot in t.Dots)
						//    {
						//        tempA.AddValues(dot.Time, 1);
						//    }
						if (tempA.Dots.Count == 0)
						{
							tempA.AddValues(t.Dots[0].Time, 1);
							tempA.AddValues(t.Dots.Last().Time, 1);
						}
						PaintBckgrnd(tempD);
						if (tempA.Series.Points.Count < 1)
							if (DefaultScale == "full") radioButton4.Checked = true;
							else radioButton5.Checked = true;

						tempD.Area.CursorX.IsUserEnabled = true;
						tempD.Area.CursorX.IsUserSelectionEnabled = true;
						tempD.Area.CursorX.LineColor = Color.Red;
						tempD.Area.CursorX.LineWidth = 2;
						tempD.Area.CursorX.SelectionColor = SystemColors.Highlight;

                        //*добавлено, чтобы обойти косяк в ситуации, когда сначала добавляется дискретный график
                        tempA.Area.CursorX.IsUserEnabled = true;
                        tempA.Area.CursorX.IsUserSelectionEnabled = true;
                        tempA.Area.CursorX.LineColor = Color.Red;
                        tempA.Area.CursorX.LineWidth = 2;
                        tempA.Area.CursorX.SelectionColor = SystemColors.Highlight;
                        tempA.Area.CursorY.Interval = .001;
                        tempA.Area.CursorY.IntervalType = DateTimeIntervalType.Number;
                        //*конец добавления
					}
				}
                
				//radioButton7.Checked = true;
				double a1 = t.Min;
				double a2 = t.Max;
				t.Area.AxisY.ScaleView.Position = a1;
				t.Area.AxisY.ScaleView.Size = a2 - a1;
			    t.Area.AxisY.MajorGrid.Interval = Math.Abs((t.Area.AxisY.ScaleView.ViewMaximum -
			                                                t.Area.AxisY.ScaleView.ViewMinimum))/5;

				//if (Noas == 1)
				//{
				//    //chart1.Series.First().Points.AddXY(t.Dots.First().Time, t.Dots.First().ToMomentReal().Mean);
				//    foreach (var d in t.Dots)
				//    {
				//        chart1.Series.First().Points.AddXY(d.Time, d.ToMomentReal().Mean);
				//    }
				//    //chart1.ChartAreas.First().AxisX.LabelStyle.Format = t.Area.AxisX.LabelStyle.Format;
				//}
                
				if (radioButton5.Checked)
				{
                    if ((comboBox5.Text == "") || (comboBox2.Text == "Сек." && double.Parse(comboBox5.Text) <= ScaleXMin * 60 * 60 * 24))
                        comboBox5.Text = (ScaleXMin * 60 * 60 * 24).ToString();
                    //SetScalePosition1(null, null);
                    //SetScalePosition2();

				    t.Area.AxisX.ScaleView.SizeType = chart1.ChartAreas[0].AxisX.ScaleView.SizeType;
				    t.Area.AxisX.ScaleView.Position = chart1.ChartAreas[0].AxisX.ScaleView.Position;
                    t.Area.AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size;
                    t.Area.AxisX.IntervalType = chart1.ChartAreas[0].AxisX.IntervalType;
                    t.Area.AxisX.Interval = chart1.ChartAreas[0].AxisX.Interval;

                    //chart1.ChartAreas[1].AxisX.ScaleView.SizeType = chart1.ChartAreas[0].AxisX.ScaleView.SizeType;
                    chart1.ChartAreas[1].AxisX.ScaleView.Position = chart1.ChartAreas[0].AxisX.ScaleView.Position;
                    //chart1.ChartAreas[1].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size;
                    //chart1.ChartAreas[1].AxisX.IntervalType = chart1.ChartAreas[0].AxisX.IntervalType;
                    //chart1.ChartAreas[1].AxisX.Interval = chart1.ChartAreas[0].AxisX.Interval;
				}
				else
				{
					IntervalReset();
				}

				_isScaleShifting = true;
				button2.Text = _isScaleShifting ? "Сдвиг стоп" : "Сдвиг пуск";

				//radioButton5.Checked = true;

				//if (chart1.ChartAreas.Count == 1)
				//{
				//    SetScalePosition1(null, null);
				//}
				//else
				//{
				//    t.Area.AxisX.ScaleView.Position = chart1.ChartAreas.First().AxisX.ScaleView.Position;
				//}

				//chart1.Refresh();

				if (chart1.ChartAreas.First().CursorX.Position > 0)
				{
					//CursorPositionGridView(button1, null);
                    GraphicParam tempP = t.DataTypeD == DataType.Boolean ? ParamsDiscrete[0] : ParamsAnalog[0];
                    CursorPositionGridView(button1, new CursorEventArgs(tempP.Area, tempP.Area.AxisX, double.NaN));
				}

				if (GetParam(ParamsCount()).Series.Points.Count != 0) Cursor = Cursors.Default;
				return "";
			}
			catch (OverflowException e)
			{
				//if (t.Code != "BckgrndAnalog")
				//{
				Error = new ErrorCommand("Невозможно отрисовать параметр (Repaint)\n" + e.StackTrace +
										 "\n" + e.Message, e);
				MessageBox.Show(e.StackTrace);
				//}
				return ErrorMessage;
			}
		}

        //Отрисовка фона
		private void PaintBckgrnd(GraphicParam t)
		{
			//_isInBreak = t.Dots.First().Nd != 0;
			t.Series.Points.Clear();
			t.IsUpdated = true;
			t.Series.Points.AddXY(t.Dots[0].Time, t.Dots[0].ToMomentReal().Mean);
			t.NumberOfFirstWaiting = 1;
			t.NumberOfLastViewed = 0;

			DrawNewStuff(t);
			//TimeBegin = t.Dots.First().Time;
			//paintedTimeInterval = DateTime.FromOADate(t.Series.Points.Last().XValue) -
			//                      DateTime.FromOADate(t.Series.Points.First().XValue);
			_paintedTimeInterval = TimeEnd - TimeBegin;
            ScaleXMin = Math.Max(.000024, _paintedTimeInterval.TotalDays / 1400000);
			//t.Area.AxisX.Interval = radioButton5.Checked ? double.Parse(comboBox4.Text) : double.NaN;
		}

		//чистка мусора
		public void Gethering()
		{
            GC.Collect();
		}

		//Сборка мусора по команде
		private void GarbCol(object sender, EventArgs e)
		{
			Gethering();
		}
        
        //Перед отрисовкой устанавливаться должен. Кол-во знаков после запятой
		public void SetDecPlacesDefault(int decPlaces)
        {
            DecPlacesDefault = decPlaces;
        }

		//Вычисляет выделенный в таблице параметр и записывает в CurrentParamNumber
		internal void DatagridSelectionChanged(object sender, EventArgs e)
		{
			try
			{
				//проверку на нулл сделать, ну нулевой ячейки
				if (dataGridView1.Rows.Count >= 1)
				{
					CurrentParamNumber = int.Parse(dataGridView1.CurrentRow.Cells["№ в таблице"].Value.ToString());
					GraphicParam tempGr = GetParam(CurrentParamNumber);
					label9.BackColor = tempGr.Series.Color;
					label9.Text = CurrentParamNumber.ToString();
					label13.BackColor = tempGr.Series.Color;
					button1.ForeColor = tempGr.Series.Color;
					button5.ForeColor = tempGr.Series.Color;
					label19.BackColor = tempGr.Series.Color;
					label13.Text = label9.Text;
					label19.Text = label9.Text;
					//if (dataGridView1.CurrentCell != dataGridView1.CurrentRow.Cells[0])
					//{
					if (tempGr.DataTypeD == DataType.Real)
					{
						textBox1.Text = tempGr.AxY.TBoxMin.Text;
						textBox2.Text = tempGr.AxY.TBoxMax.Text;
						button4.Enabled = true;
						//radioButton6.Enabled = true;
						//radioButton7.Enabled = true;
						textBox1.Enabled = true;
						textBox2.Enabled = true;
						//button8.Enabled = true;
						//button9.Enabled = true;
					}
					else
					{
						textBox1.Text = "0";
						textBox2.Text = "2";
						button4.Enabled = false;
						//radioButton6.Enabled = false;
						//radioButton7.Enabled = false;
						textBox1.Enabled = false;
						textBox2.Enabled = false;
						//button8.Enabled = false;
						//button9.Enabled = false;
					}
					//}
					//dataGridView1.CurrentRow.Selected = true;

					button10.Text = tempGr.IsVisible ? "Скрыть график" : "Отобразить график";

					//IsYScaledAuto = tempGr.IsYScaledAuto;
                    if (tempGr.IsYScaledAuto && TimerMode == TimerModes.Analyzer)
					{
						button23.UseVisualStyleBackColor = false;
						button23.BackColor = SystemColors.Highlight;
					}
					else
					{
						//button23.BackColor = Color.White;
						button23.UseVisualStyleBackColor = true;
					}

					if (tempGr.PercentMode == PercentModeClass.Percentage)
					{
						button24.Text = "В единицы изм.";
						label15.Text = "%";
					}
					else
					{
						button24.Text = "В проценты";
						label15.Text = dataGridView1.CurrentRow.Cells["Ед. измер."].Value.ToString();
					}
					
					//При выборе графика отрисовка и ось выползают наверх
					if (tempGr.DataTypeD != DataType.Boolean)
					{
						bool seriesFlag = chart1.Series.Remove(tempGr.Series);
						bool areaFlag = chart1.ChartAreas.Remove(tempGr.Area);
						if (areaFlag) chart1.ChartAreas.Add(tempGr.Area);
						if (seriesFlag) chart1.Series.Add(tempGr.Series);
						//ToTopOverlay(tempGr);
					}
				}
			}
			catch (Exception ex)
			{
				bool flag = dataGridView1.Rows.Count == 0;
				if (!flag)
				{
					Error = new ErrorCommand("Проблема со вводом в таблицу (datagridSelectionChanged)", ex);
					//MessageBox.Show(ex.StackTrace);
					throw;
				}
			}
		}

		//Удаление графика удалением строки датагрида
		internal void DatagridRowAnnihilate(object sender, DataGridViewRowCancelEventArgs e)
		{
			e.Cancel = true;
			DialogResult yResult = MessageBox.Show("Удалить график №" + CurrentParamNumber + " ?", "Запрос на удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (CurrentParamNumber > 0 && yResult == DialogResult.OK)
				DeleteParam(GetParam(CurrentParamNumber).Code);
		}

		//cкрывает/отображает конкретные графики из датагрида
		internal void HideGraph(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				if (e.ColumnIndex == 1)
				{
					DataGridViewCell curCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
					//DataGridViewCell curCell = WantedRow(e.RowIndex + 1).Cells[e.ColumnIndex];
					GraphicParam curParam = GetParam(int.Parse(curCell.Value.ToString()));
					if (curCell.Style.BackColor != Color.Empty)
					{
						curCell.Style.BackColor = Color.Empty;
						curCell.Style.SelectionBackColor = Color.White; //Color.Empty;
						curCell.Style.ForeColor = curParam.Series.Color;
						curCell.Style.SelectionForeColor = curCell.Style.ForeColor;
						curParam.Area.Visible = false;
						curParam.IsVisible = false;
						if (curParam.DataTypeD != DataType.Boolean)
						{
							if (!curParam.AxY.IsOverlayed)
							//if (curParam.DataTypeD == DataType.Real)
							{
								curParam.AxY.Ax.Visible = false;
								if (!curParam.AxY.IsHidden) Noas--;
							}
							//else
							//{
							//    Nods--;
							//    curParam.AxCap.Visible = false;
							//}
							else
							{
								//if (curParam.Index == curParam.AxY.UpperParam.Index)
								//{
								//DigginOverlayedOut(curParam);
								int thereIsOneMoreVisibleGraphInPack = -42;
								foreach (int t in curParam.AxY.AxesOverlay)
								{
									if (GetParam(t).IsVisible)
									{
										//curParam.AxY.UpperParam = GetParam(t);
										thereIsOneMoreVisibleGraphInPack = t;
										break;
									}
								}
								if (thereIsOneMoreVisibleGraphInPack <= 0)
								{
									if (!curParam.AxY.IsHidden) Noas--;
									curParam.AxY.Ax.Visible = false;
								}
								else
								{
									curParam.AxY.UpperParam = GetParam(thereIsOneMoreVisibleGraphInPack);
									foreach (var t in curParam.AxY.AxesOverlay)
									{
										//dataGridView1[0, t - 1].Style.BackColor = curParam.AxY.UpperParam.Series.Color;
										//dataGridView1[0, t - 1].Style.SelectionBackColor = curParam.AxY.UpperParam.Series.Color;
										WantedRow(t).Cells[0].Style.BackColor = curParam.AxY.UpperParam.Series.Color;
										WantedRow(t).Cells[0].Style.SelectionBackColor = curParam.AxY.UpperParam.Series.Color;
									}
								}
								//}
							}
						}
						else
						{
							Nods--;
							curParam.AxCap.Visible = false;
						}
						button10.Text = "Отобразить график";

						dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
						dataGridView1.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Gray;
						//WantedRow(e.RowIndex + 1).DefaultCellStyle.ForeColor = Color.Gray;
						//WantedRow(e.RowIndex + 1).DefaultCellStyle.SelectionForeColor = Color.Gray;
					}
					else
					{
						curCell.Style.BackColor = curParam.Series.Color;
						curCell.Style.SelectionBackColor = curParam.Series.Color;
						curCell.Style.ForeColor = Color.White;
						curCell.Style.SelectionForeColor = Color.White;

						if (curParam.DataTypeD != DataType.Boolean)
						{
                            if (!curParam.AxY.IsOverlayed && !curParam.AxY.IsHidden)
                            //if (curParam.DataTypeD == DataType.Real)
                            {
                                curParam.AxY.Ax.Visible = true;
                                Noas++;
                            }
                            //else
                            //{
                            //    Nods++;
                            //    curParam.AxCap.Visible = true;
                            //}
                            else
                            {
                                //ищем хотя бы один видимый график среди склеенных с данным
                                bool flagVisible = false;
                                //int nmbrOfVsblPrm = -42;
                                for (int i = 0; i <= curParam.AxY.AxesOverlay.Count - 1; i++)
                                {
                                    if (GetParam(curParam.AxY.AxesOverlay[i]).IsVisible)
                                    {
                                        flagVisible = true;
                                        //nmbrOfVsblPrm = i;
                                        break;
                                    }
                                }
                                if (!flagVisible && !curParam.AxY.IsHidden)
                                {
                                    curParam.AxY.Ax.Visible = true;
                                    Noas++;

                                    //curParam.AxY.UpperParam = GetParam(nmbrOfVsblPrm);
                                }
                                curParam.AxY.UpperParam = curParam;
                                foreach (var t in curParam.AxY.AxesOverlay)
                                {
                                    //dataGridView1[0, t - 1].Style.BackColor = curParam.Series.Color;
                                    //dataGridView1[0, t - 1].Style.SelectionBackColor = curParam.Series.Color;
                                    WantedRow(t).Cells[0].Style.BackColor = curParam.Series.Color;
                                    WantedRow(t).Cells[0].Style.SelectionBackColor = curParam.Series.Color;
                                }
                            }
						}
						else
						{
							Nods++;
							curParam.AxCap.Visible = true;
						}
						curParam.Area.Visible = true;
						curParam.IsVisible = true;
						button10.Text = "Скрыть график";

						dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Empty;
						dataGridView1.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Empty;
						//WantedRow(e.RowIndex + 1).DefaultCellStyle.ForeColor = Color.Empty;
						//WantedRow(e.RowIndex + 1).DefaultCellStyle.SelectionForeColor = Color.Empty;

						AreaDisRankingHeight(curParam);
					}
					int nmbrOfVsblAnAxes = 0;
					if (curParam.DataTypeD != DataType.Boolean)
						foreach (GraphicParam t in ParamsAnalog)
						{
							if (t.AxY.Ax.Visible && !t.AxY.IsHidden && t.AxY.UpperParam != null &&
								t.AxY.UpperParam.Index == t.Index)
							{
								nmbrOfVsblAnAxes++;
								t.AxY.Ax.Left = (nmbrOfVsblAnAxes - 1)*AxWidth;
							}
							//AxesRankingSize(t);
							AreaRankingX(t);
							AreaAnRankingHeight(t);
						}
					else foreach (GraphicParam t in ParamsAnalog)
					{
						if (t.Index == t.AxY.UpperParam.Index) AxesRankingSize(t);
					}

					int nmbrOfVsblDisGrs = 1;
					foreach (GraphicParam t in ParamsDiscrete)
					{
						AreaRankingX(t);
						if (t.IsVisible) nmbrOfVsblDisGrs++;
						AreaDisRankingY(t, nmbrOfVsblDisGrs);
					}
					AreaDisFirstHeight();

					DownerScrollRankingSize();
				}
				//else
				//{
				//    if (e.RowIndex >= 0 && e.ColumnIndex == 0)
				//    {
				//        DataGridViewCheckBoxCell curCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
				//        GraphicParam curParam = GetParam(int.Parse(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
				//        if ((bool)curCell.Value == false)
				//        {
				//            //curCell.Value = true;
				//            MessageBox.Show(curParam.Series.Color.ToString());
				//        }
				//        else
				//        {
				//            //curCell.Value = false;
				//        }
				//    }
				//}
				////При выборе графика отрисовка и ось выползают наверх
				GraphicParam df = GetParam(CurrentParamNumber);
				if (df != null && df.DataTypeD != DataType.Boolean && df.AxY.UpperParam != null && df.AxY.UpperParam.Index != df.Index) ToTopOverlay(df);
				//dataGridView1.RefreshEdit();
			}
		}
		internal void HideAx(object sender, EventArgs e)
		{
			int cGNumber = int.Parse(((Button) sender).Parent.Parent.Name);
			//GraphicParam curP = GetParam(cGNumber);
			//foreach (var u in curP.AxY.AxesOverlay)
			//{
				//dataGridView1.Rows[u - 1].Cells[0].Value = false;
				WantedRow(cGNumber).Cells[0].Value = false;
			//}
		}

        //Скрытие оси Y
		private bool _needToCheckChangeDatagrid = true;
		internal void CheckChangeDatagrid(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex == 0)// && _needToCheckChangeDatagrid)
			{
				GraphicParam curParam = GetParam(int.Parse(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
				
				if (curParam.DataTypeD != DataType.Boolean && _needToCheckChangeDatagrid)// && curParam.AxY.UpperParam.Index == curParam.Index)
				{
					HideAxClick(curParam.AxY.RulePBoxes[0], e);
				}
				_needToCheckChangeDatagrid = true;
			}
		}
		void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (dataGridView1.IsCurrentCellDirty)
				dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}

		//Работа скролла
		private double _pos;
        private void HScrollerScroll(object sender, ScrollEventArgs e)
		{
		    _pos = ((double) (_hscrollb.Value) - _hscrollb.Minimum)/(_hscrollb.Maximum - _hscrollb.Minimum);
			double interval = double.Parse(comboBox5.Text);

            //TimeBegin = (ParamsAnalog[0].Series.Points.Count > 2)
            //                            ? DateTime.FromOADate(ParamsAnalog[0].Series.Points[0].XValue)
            //                            : DateTime.FromOADate(ParamsDiscrete[0].Series.Points[0].XValue);

            if (TimerMode == TimerModes.Monitor) interval *= .7;
			try
			{
				switch (comboBox2.Text)
				{
					case "Сек.":
						foreach (var t in ParamsAnalog)
						{
						    double ts = _paintedTimeInterval.TotalSeconds - interval;
							ts = ts/.901;
                            DateTime dt = TimeBegin.AddSeconds(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						foreach (var t in ParamsDiscrete)
						{
							try
							{
							double ts = _paintedTimeInterval.TotalSeconds - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddSeconds(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
							}
							catch (OverflowException)
							{
								MessageBox.Show("15");
							}
						}
						break;
					case "Мин.":
						foreach (var t in ParamsAnalog)
						{
							double ts = _paintedTimeInterval.TotalMinutes - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddMinutes(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						foreach (var t in ParamsDiscrete)
						{
                            if (t.Series.Points.Count > 1)
                            {
                                double ts = _paintedTimeInterval.TotalMinutes - interval;
                                ts = ts/.901;
                                DateTime dt = TimeBegin.AddMinutes(ts * _pos);
                                t.Area.AxisX.ScaleView.Position = dt.ToOADate();
                            }
						}
						break;
					case "Час.":
						foreach (var t in ParamsAnalog)
						{
							double ts = _paintedTimeInterval.TotalHours - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddHours(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						foreach (var t in ParamsDiscrete)
						{
							double ts = _paintedTimeInterval.TotalHours - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddHours(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						break;
					case "Сут.":
						foreach (var t in ParamsAnalog)
						{
							double ts = _paintedTimeInterval.TotalDays - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddDays(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						foreach (var t in ParamsDiscrete)
						{
							double ts = _paintedTimeInterval.TotalDays - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddDays(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						break;
					default:
						foreach (var t in ParamsAnalog)
						{
							double ts = _paintedTimeInterval.TotalMinutes - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddMinutes(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						foreach (var t in ParamsDiscrete)
						{
							double ts = _paintedTimeInterval.TotalMinutes - interval;
							ts = ts / .901;
                            DateTime dt = TimeBegin.AddMinutes(ts * _pos);
							t.Area.AxisX.ScaleView.Position = dt.ToOADate();
						}
						break;
				}

                //new
                var t1 = DateTime.FromOADate(ParamsAnalog[0].Area.AxisX.ScaleView.ViewMinimum);
                var t2 = DateTime.FromOADate(ParamsAnalog[0].Area.AxisX.ScaleView.ViewMaximum);
                RedrawAllParams(t1, t2);
			}
			catch (Exception ex)
			{
				bool flag = true;
				foreach (var graphicParam in ParamsAnalog)
				{
					flag = flag & graphicParam.Series.Points.Count == 0;
				}
				if (!flag)
				{
					Error = new ErrorCommand("Проблема со скроллом (hScroller_Scroll)", ex);
					throw;
				}
			}
		}

        //Перемещает скроллер (по времени (ось х)) в соответствующее положение.
        private void HScrollerPos()
        {
            double interval = double.Parse(comboBox5.Text);
            DateTime dt = DateTime.FromOADate(chart1.ChartAreas[0].AxisX.ScaleView.Position);
            if (TimerMode == TimerModes.Monitor) interval *= .7;
            try
            {
                switch (comboBox2.Text)
                {
                    case "Сек.":
                        double ts = _paintedTimeInterval.TotalSeconds - interval;
                        ts = ts / .901;
                        _pos = (dt - TimeBegin).TotalSeconds/ts;
                        break;
                    case "Мин.":
                        ts = _paintedTimeInterval.TotalMinutes - interval;
                        ts = ts / .901;
                        _pos = (dt - TimeBegin).TotalMinutes / ts;
                        break;
                    case "Час.":
                        ts = _paintedTimeInterval.TotalHours - interval;
                        ts = ts / .901;
                        _pos = (dt - TimeBegin).TotalHours / ts;
                        break;
                    case "Сут.":
                        ts = _paintedTimeInterval.TotalDays - interval;
                        ts = ts / .901;
                        _pos = (dt - TimeBegin).TotalDays / ts;
                        break;
                    default:
                        ts = _paintedTimeInterval.TotalMinutes - interval;
                        ts = ts / .901;
                        _pos = (dt - TimeBegin).TotalMinutes / ts;
                        break;
                }

                int x = _hscrollb.Minimum + (int) (_pos*(_hscrollb.Maximum - _hscrollb.Minimum));
                _hscrollb.Value = x > 0 ? x : 0;
            }
            catch (Exception ex)
            {
                bool flag = true;
                foreach (var graphicParam in ParamsAnalog)
                {
                    flag = flag & graphicParam.Series.Points.Count == 0;
                }
                if (!flag)
                {
                    Error = new ErrorCommand("Проблема со скроллом (hScroller_Scroll)", ex);
                    throw;
                }
            }
        }

		//Путешествие визира по времени с кнопочек
		private void ButtonTimeWalkAhead(object sender, EventArgs e)
		{
			try
			{
				GraphicParam curParam = GetParam(CurrentParamNumber);
				if (curParam.IsVisible && curParam.Dots.Count > 0)
				{
					GraphicParam firstDParam = ParamsDiscrete.First();
					GraphicParam firstAParam = ParamsAnalog.First();
					MomentReal fakeEnd = new MomentReal(TimeEnd, 1, 1);
					MomentReal fakeBegin = new MomentReal(TimeBegin, 1, 1);
					int dotIndex;
					if (curParam.DataTypeD == DataType.Boolean)
					{
						if (firstDParam.Area.CursorX.Position > 0)
						{
							double p1 = firstDParam.Area.CursorX.Position;
							dotIndex = curParam.Dots.FindIndex(x => (x.Time.ToOADate() > p1));
							if (dotIndex == -1) dotIndex = curParam.Dots.Count;
							MomentValue ff = curParam.Dots[dotIndex - 1];
							MomentValue gg;
							if (curParam.Dots.Find(x =>
												   (x.Time > DateTime.FromOADate(p1) &&
													x.ToMomentBoolean().Mean != ff.ToMomentBoolean().Mean)) != null)
							{gg = curParam.Dots.Find(x =>
														(x.Time > DateTime.FromOADate(p1) &&
														 (x.ToMomentBoolean().Mean != ff.ToMomentBoolean().Mean ||
														 x.Nd != ff.Nd)));
								firstDParam.Area.CursorX.Position = gg.Time.ToOADate();
								firstAParam.Area.CursorX.Position = firstDParam.Area.CursorX.Position;
							}
							else
							{
								firstDParam.Area.CursorX.Position = fakeEnd.Time.ToOADate();
								firstAParam.Area.CursorX.Position = fakeEnd.Time.ToOADate();
							}

							Refresh();
							double p2 = firstDParam.Area.CursorX.Position;
							if (p2 > firstDParam.Area.AxisX.ScaleView.ViewMaximum)
							{
                                //firstDParam.Area.AxisX.ScaleView.Position += (p2 - p1) +
                                //                                             (firstDParam.Area.AxisX.ScaleView.
                                //                  ViewMaximum - firstDParam.Area.AxisX.ScaleView.Position) / 4;
                                firstDParam.Area.AxisX.ScaleView.Position = p2 -
                                                                             3*(firstDParam.Area.AxisX.ScaleView.
                                                  ViewMaximum - firstDParam.Area.AxisX.ScaleView.Position) / 4;
								if (radioButton5.Checked) SetScalePosition2();

								try
								{

								TimeSpan lSpan = DateTime.FromOADate(firstDParam.Area.AxisX.ScaleView.Position) - TimeBegin;
                                var v = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
                                _hscrollb.Value = v < 100 ? v : 901;
								}
                                catch (Exception)
								{
									//MessageBox.Show("15");
								}
							}
							
							var ar = new CursorEventArgs(firstDParam.Area, firstDParam.Area.AxisX,
														 firstDParam.Area.CursorX.Position);
							CursorPositionView(button1, ar);
							CursorPositionGridView(button1, ar);
						}
					}
					else
					{
						if (ParamsAnalog.First().Area.CursorX.Position > 0)
						{
							double p1 = firstAParam.Area.CursorX.Position;
                            //индекс ближайшей точки в списке точек, следующей за текущим положением
							dotIndex = curParam.Dots.FindIndex(x => (x.Time.ToOADate() > p1));
							if (dotIndex == -1) dotIndex = curParam.Dots.Count;
							//MomentValue ff = curParam.Dots.FindLast(x => (x.Time.ToOADate() <= p1)) ?? fakeBegin;
							//MomentValue gg = curParam.Dots.Find(
							//    x => (x.Time > DateTime.FromOADate(p1) && x.ToMomentReal() != ff.ToMomentReal())) ??
							//                 fakeEnd;
                            //значение в этой точке
							MomentValue ff = curParam.Dots[dotIndex - 1];
							MomentValue gg;
							if (curParam.Dots.Find(x =>
												   (x.Time > DateTime.FromOADate(p1) &&
													x.ToMomentReal().Mean != ff.ToMomentReal().Mean)) != null)
							{
								gg = curParam.Dots.Find(x =>
														   (x.Time > DateTime.FromOADate(p1) &&
															(x.ToMomentReal().Mean != ff.ToMomentReal().Mean ||
															x.Nd != ff.Nd)));
								firstDParam.Area.CursorX.Position = gg.Time.ToOADate();
								firstAParam.Area.CursorX.Position = firstDParam.Area.CursorX.Position;
							}
							else
							{
								firstDParam.Area.CursorX.Position = fakeEnd.Time.ToOADate();
								firstAParam.Area.CursorX.Position = fakeEnd.Time.ToOADate();
							}
							Refresh();
                            //сдвигаем график, если визир вышел за пределы экрана
							double p2 = firstAParam.Area.CursorX.Position;
							if (p2 > firstAParam.Area.AxisX.ScaleView.ViewMaximum)
							{
                                //firstAParam.Area.AxisX.ScaleView.Position += (p2 - p1) +
                                //                                             (firstAParam.Area.AxisX.ScaleView.
                                //                  ViewMaximum - firstAParam.Area.AxisX.ScaleView.Position) / 4;
                                firstAParam.Area.AxisX.ScaleView.Position = p2 -
                                                                             3*(firstAParam.Area.AxisX.ScaleView.
                                                  ViewMaximum - firstAParam.Area.AxisX.ScaleView.Position) / 4;
								if (radioButton5.Checked) SetScalePosition2();

                                try
                                {
                                    TimeSpan lSpan = DateTime.FromOADate(firstAParam.Area.AxisX.ScaleView.Position) - TimeBegin;
                                    var v = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
                                    _hscrollb.Value = v < 100 ? v : 901;
                                }
                                catch (Exception)
                                {
                                    //MessageBox.Show("15");
                                }
							}

							var ar = new CursorEventArgs(firstAParam.Area, firstAParam.Area.AxisX,
														 firstAParam.Area.CursorX.Position);
							CursorPositionView(button1, ar);
							CursorPositionGridView(button1, ar);
							//MessageBox.Show("Значится так. Мы находимся внутри метода, значит CursorPosition в порядке:" +
							//                ParamsAnalog.First().Area.CursorX.Position +
							//                "\nПоследняя точка перед текущей: " + ff.Time + " " + ff.ToMomentReal().Mean +
							//                "\nНу, а необходимая: " + gg.Time + " " + gg.ToMomentReal().Mean + 
							//                "\nКак-то, блин, не помогло. Посмотрим на разность " + 
							//                (p1 - ff.Time.ToOADate()) + "\nДолжен быть ноль так-то");
						}
					}
					label17.BackColor = curParam.Series.Color;
					label17.Text = curParam.Index.ToString();
					label16.BackColor = Color.White;
				}
			}
			catch (Exception ex)
			{
				timer2.Stop();
				//MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
                Error = new ErrorCommand("", ex);
			}
		}

		private void ButtonTimeWalkBack(object sender, EventArgs e)
		{
			try
			{
				GraphicParam curParam = GetParam(CurrentParamNumber);
				if (curParam.IsVisible && curParam.Dots.Count > 0)
				{
					GraphicParam firstDParam = ParamsDiscrete.First();
					GraphicParam firstAParam = ParamsAnalog.First();
					var fakeBegin = new MomentReal(TimeBegin, 1, 1);
					int ind = -1;
					if (curParam.DataTypeD == DataType.Boolean)
					{
						if (ParamsDiscrete.First().Area.CursorX.Position > 0)
						{
							double p1 = firstDParam.Area.CursorX.Position;
							MomentValue ff = curParam.Dots.FindLast(x => (x.Time < DateTime.FromOADate(p1)));
							ind = curParam.Dots.FindLastIndex(x =>
									(x.Time < DateTime.FromOADate(p1) && (x.ToMomentBoolean() != ff.ToMomentBoolean() || x.Nd != ff.Nd)));
							firstDParam.Area.CursorX.Position = curParam.Dots[ind + 1].Time.ToOADate();
							//CursorPositionView(button1, null);
							//CursorPositionGridView(button1, null);
							double p2 = firstDParam.Area.CursorX.Position;
							firstAParam.Area.CursorX.Position = p2;
							if (p2 < firstDParam.Area.AxisX.ScaleView.ViewMinimum)
							{
								if (ind != -1)
                                //firstDParam.Area.AxisX.ScaleView.Position += (p2 - p1) -
                                //             (firstDParam.Area.AxisX.ScaleView.ViewMaximum -
                                //             firstDParam.Area.AxisX.ScaleView.Position)/4;
                                    firstDParam.Area.AxisX.ScaleView.Position = p2 -
                                             3*(firstDParam.Area.AxisX.ScaleView.ViewMaximum -
                                             firstDParam.Area.AxisX.ScaleView.Position) / 4;
								else
								{
									firstDParam.Area.AxisX.ScaleView.Position = curParam.Dots[ind + 1].Time.ToOADate();
								}
								if (radioButton5.Checked) SetScalePosition2();

								try
								{
                                    chart1.Refresh();
                                    //DateTime time = DateTime.FromOADate(firstDParam.Area.AxisX.ScaleView.Position);
                                    DateTime time = DateTime.FromOADate(firstDParam.Area.CursorX.Position);
								    TimeSpan lSpan = time - TimeBegin;
								    var v = (int) Math.Round(lSpan.TotalSeconds/_paintedTimeInterval.TotalSeconds*1000.0);
								    _hscrollb.Value = v >= 0 ? v : 0;
								}
                                catch (Exception)
								{
									//MessageBox.Show("15");
								}
							}
							var ar = new CursorEventArgs(firstDParam.Area, firstDParam.Area.AxisX,
														 firstDParam.Area.CursorX.Position);
							CursorPositionView(button1, ar);
							CursorPositionGridView(button1, ar);
						}
					}
					else
					{
						if (ParamsAnalog.First().Area.CursorX.Position > 0)
						{
							double p1 = firstAParam.Area.CursorX.Position;
							MomentValue ff = curParam.Dots.FindLast(x => (x.Time < DateTime.FromOADate(p1)));
							//firstAParam.Area.CursorX.Position = curParam.Dots.Find(
							//    x => (x.Time < ff.Time && x.ToMomentReal() != ff.ToMomentReal())).Time.ToOADate();
							ind =
								curParam.Dots.FindLastIndex(
									x =>
									(x.Time < DateTime.FromOADate(p1) && (x.ToMomentReal().Mean != ff.ToMomentReal().Mean || x.Nd != ff.Nd)));
							firstAParam.Area.CursorX.Position = curParam.Dots[ind + 1].Time.ToOADate();
							//CursorPositionView(button1, null);
							//CursorPositionGridView(button1, null);
							double p2 = firstAParam.Area.CursorX.Position;
							firstDParam.Area.CursorX.Position = p2;
							if (p2 < firstAParam.Area.AxisX.ScaleView.ViewMinimum)
							{
								if (ind != -1)
                                //firstAParam.Area.AxisX.ScaleView.Position += (p2 - p1) -
                                //                 (firstAParam.Area.AxisX.ScaleView.ViewMaximum -
                                //                 firstAParam.Area.AxisX.ScaleView.Position)/4;
                                    firstAParam.Area.AxisX.ScaleView.Position = p2 -
                                                 3*(firstAParam.Area.AxisX.ScaleView.ViewMaximum -
                                                 firstAParam.Area.AxisX.ScaleView.Position) / 4;
								else
								{
									firstAParam.Area.AxisX.ScaleView.Position = curParam.Dots[ind + 1].Time.ToOADate();
								}
								if (radioButton5.Checked) SetScalePosition2();

								try
								{
								TimeSpan lSpan = DateTime.FromOADate(firstAParam.Area.AxisX.ScaleView.Position) - TimeBegin;
								var v = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
								_hscrollb.Value = v >= 0 ? v : 0;
								}
                                catch (Exception)
								{
									//MessageBox.Show("15");
								}
							}
							var ar = new CursorEventArgs(firstAParam.Area, firstAParam.Area.AxisX,
														 firstAParam.Area.CursorX.Position);
							CursorPositionView(button1, ar);
							CursorPositionGridView(button1, ar);
						}
					}
					//label16.BackColor = ind != -1 ? curParam.Series.Color : Color.White;
					label16.BackColor = curParam.Series.Color;
					label17.BackColor = Color.White;
					label16.Text = curParam.Index.ToString();
				}
			}
			catch (Exception ex)
			{
				timer2.Stop();
                //MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
                Error = new ErrorCommand("", ex);
			}
		}

		private void WalkColorReset(object sender, EventArgs e)
		{
			label17.BackColor = Color.White;
			label16.BackColor = Color.White;
			label17.Text = "";
			label16.Text = "";
		}

		private void ButtonTimeStepAhead(object sender, EventArgs e)
		{
			try
			{
				GraphicParam firstDParam = ParamsDiscrete.First();
				GraphicParam firstAParam = ParamsAnalog.First();
				GraphicParam chosenP = firstAParam;
				if (GetParam(CurrentParamNumber).DataTypeD == DataType.Boolean)
				{
					if (firstDParam.Area.CursorX.Position > 0)
					{
						double p1 = firstDParam.Area.CursorX.Position;
						MomentValue fakeEnd = new MomentReal(TimeEnd, 1, 1);
						MomentValue fakeBegin = new MomentReal(TimeBegin, 1, 1);
						MomentValue ff;
						MomentValue gg = fakeEnd;
						foreach (var graphicParam in ParamsAnalog)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								//ff = graphicParam.Dots.FindLast(x => (x.Time.ToOADate() <= p1)) ?? fakeBegin;
								int dotIndex = graphicParam.Dots.FindIndex(x => (x.Time.ToOADate() > p1));
								if (dotIndex == -1) dotIndex = graphicParam.Dots.Count;
								ff = graphicParam.Dots[dotIndex - 1];
								MomentValue temp =
									graphicParam.Dots.Find(
										x => (x.Time > DateTime.FromOADate(p1) && (x.ToMomentReal() != ff.ToMomentReal() || x.Nd != ff.Nd))) ??
									fakeEnd;
								if (gg.Time > temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
								}
							}
						}
						foreach (var graphicParam in ParamsDiscrete)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								//ff = graphicParam.Dots.FindLast(x => (x.Time.ToOADate() <= p1)) ?? fakeBegin;
								int dotIndex = graphicParam.Dots.FindIndex(x => (x.Time.ToOADate() > p1));
								if (dotIndex == -1) dotIndex = graphicParam.Dots.Count;
								ff = graphicParam.Dots[dotIndex - 1];
								MomentValue temp =
									graphicParam.Dots.Find(
										x => (x.Time > DateTime.FromOADate(p1) && (x.ToMomentBoolean() != ff.ToMomentBoolean() || x.Nd != ff.Nd))) ??
									fakeEnd;
								if (gg.Time > temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
								}
							}
						}
						firstDParam.Area.CursorX.Position = gg.Time.ToOADate();
						//CursorPositionView(button1, null);
						//CursorPositionGridView(button1, null);
						double p2 = firstDParam.Area.CursorX.Position;
						firstAParam.Area.CursorX.Position = firstDParam.Area.CursorX.Position;
						if (p2 > firstDParam.Area.AxisX.ScaleView.ViewMaximum)
						{
							firstDParam.Area.AxisX.ScaleView.Position += (p2 - p1) +
																		 (firstDParam.Area.AxisX.ScaleView.
																			  ViewMaximum
																		  -
																		  firstDParam.Area.AxisX.ScaleView.Position)/
																		 4;
							if (radioButton5.Checked) SetScalePosition2();

							try
							{
							TimeSpan lSpan = DateTime.FromOADate(firstDParam.Area.AxisX.ScaleView.Position) - TimeBegin;
                            //_hscrollb.Value = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
                            var v = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
                            _hscrollb.Value = v <= 100 ? v : 901;
							}
                            catch (Exception)
							{
								//MessageBox.Show("15");
							}
						}
						var ar = new CursorEventArgs(firstDParam.Area, firstDParam.Area.AxisX, firstDParam.Area.CursorX.Position);
						CursorPositionView(button1, ar);
						CursorPositionGridView(button1, ar);
					}
				}
				else
				{
					if (ParamsAnalog.First().Area.CursorX.Position > 0)
					{
						double p1 = firstAParam.Area.CursorX.Position;
						MomentValue fakeEnd = new MomentReal(TimeEnd, 1, 1);
						MomentValue fakeBegin = new MomentReal(TimeBegin, 1, 1);
						MomentValue ff;
						MomentValue gg = fakeEnd;
						foreach (var graphicParam in ParamsAnalog)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								//ff = graphicParam.Dots.FindLast(x => (x.Time.ToOADate() <= p1)) ?? fakeBegin;
								int dotIndex = graphicParam.Dots.FindIndex(x => (x.Time.ToOADate() > p1));
								if (dotIndex == -1) dotIndex = graphicParam.Dots.Count;
								ff = graphicParam.Dots[dotIndex - 1];
								MomentValue temp =
									graphicParam.Dots.Find(
										x => (x.Time > DateTime.FromOADate(p1) && (x.ToMomentReal() != ff.ToMomentReal() || x.Nd != ff.Nd))) ??
									fakeEnd;
								if (gg.Time > temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
								}
							}
						}
						foreach (var graphicParam in ParamsDiscrete)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								//ff = graphicParam.Dots.FindLast(x => (x.Time.ToOADate() <= p1)) ?? fakeBegin;
								int dotIndex = graphicParam.Dots.FindIndex(x => (x.Time.ToOADate() > p1));
								if (dotIndex == -1) dotIndex = graphicParam.Dots.Count;
								ff = graphicParam.Dots[dotIndex - 1];
								MomentValue temp =
									graphicParam.Dots.Find(
										x => (x.Time > DateTime.FromOADate(p1) && (x.ToMomentBoolean() != ff.ToMomentBoolean() || x.Nd != ff.Nd))) ??
									fakeEnd;
								if (gg.Time > temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
								}
							}
						}
						firstAParam.Area.CursorX.Position = gg.Time.ToOADate();
						//CursorPositionView(button1, null);
						//CursorPositionGridView(button1, null);
						double p2 = firstAParam.Area.CursorX.Position;
						firstDParam.Area.CursorX.Position = p2;
						if (p2 > firstAParam.Area.AxisX.ScaleView.ViewMaximum)
						{
							firstAParam.Area.AxisX.ScaleView.Position += (p2 - p1) +
																		 (firstAParam.Area.AxisX.ScaleView.
																			  ViewMaximum -
																		  firstAParam.Area.AxisX.ScaleView.Position)/
																		 4;
							if (radioButton5.Checked) SetScalePosition2();
							try
							{
							TimeSpan lSpan = DateTime.FromOADate(firstAParam.Area.AxisX.ScaleView.Position) - TimeBegin;
                            //_hscrollb.Value = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
                            var v = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
							    _hscrollb.Value = v <= 100 ? v : 901;
							}
                            catch (Exception)
							{
								//MessageBox.Show("15");
							}
						}
						var ar = new CursorEventArgs(firstAParam.Area, firstAParam.Area.AxisX, firstAParam.Area.CursorX.Position);
						CursorPositionView(button1, ar);
						CursorPositionGridView(button1, ar);
					}
				}
				label17.BackColor = chosenP.Series.Color;
				label16.BackColor = Color.White;
				label17.Text = chosenP.Index.ToString();
			}
			catch (Exception ex)
			{
				timer2.Stop();
				//MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
                Error = new ErrorCommand("", ex);
			}
		}

		private void ButtonTimeStepBack(object sender, EventArgs e)
		{
			try
			{
				GraphicParam curParam = GetParam(CurrentParamNumber);
				GraphicParam firstDParam = ParamsDiscrete.First();
				GraphicParam firstAParam = ParamsAnalog.First();
				GraphicParam chosenP = firstAParam;
				if (curParam.DataTypeD == DataType.Boolean)
				{
					if (ParamsDiscrete.First().Area.CursorX.Position > 0)
					{
						bool toBeginFlag = true;
						MomentValue fakeEnd = new MomentReal(TimeEnd, 1, 1);
						MomentValue fakeBegin = new MomentReal(TimeBegin, 1, 1);
						MomentValue ff;
						MomentValue gg = fakeBegin;
						double p1 = firstDParam.Area.CursorX.Position;
						foreach (var graphicParam in ParamsAnalog)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								ff = graphicParam.Dots.FindLast(x => (x.Time < DateTime.FromOADate(p1)));
								int ind = 0;
								if (ff == null) ind = -1;
								else
									if (ParamsAnalog[0].Area.CursorX.Position != TimeBegin.ToOADate())
										ind = graphicParam.Dots.FindLastIndex(
											x => (x.Time < ff.Time && (x.ToMomentReal().Mean != ff.ToMomentReal().Mean || x.Nd != ff.Nd)));
								MomentValue temp = graphicParam.Dots[ind + 1];
								if (gg.Time < temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
									toBeginFlag = false;
								}
							}
						}
						foreach (var graphicParam in ParamsDiscrete)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								ff = graphicParam.Dots.FindLast(x => (x.Time < DateTime.FromOADate(p1)));
								int ind = 0;
								//if (ParamsAnalog[0].Area.CursorX.Position != TimeBegin.ToOADate())
								//    ind = graphicParam.Dots.FindLastIndex(
								//        x => (x.Time < ff.Time && x.ToMomentBoolean().Mean != ff.ToMomentBoolean().Mean));
								//else ind = -1;
								if (ff == null) ind = -1;
								else
									if (ParamsAnalog[0].Area.CursorX.Position != TimeBegin.ToOADate())
										ind = graphicParam.Dots.FindLastIndex(
											x => (x.Time < ff.Time && (x.ToMomentBoolean().Mean != ff.ToMomentBoolean().Mean || x.Nd != ff.Nd)));
								MomentValue temp = graphicParam.Dots[ind + 1];
								if (gg.Time < temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
									toBeginFlag = false;
								}
							}
						}
						firstDParam.Area.CursorX.Position = gg.Time.ToOADate();
						//CursorPositionView(button1, null);
						//CursorPositionGridView(button1, null);
						double p2 = firstDParam.Area.CursorX.Position;
						firstAParam.Area.CursorX.Position = p2;
						if (p2 < firstDParam.Area.AxisX.ScaleView.ViewMinimum)
						{
							if (!toBeginFlag)
							firstDParam.Area.AxisX.ScaleView.Position += (p2 - p1) -
																		 (firstDParam.Area.AxisX.ScaleView.ViewMaximum
																		  - firstDParam.Area.AxisX.ScaleView.Position)/4;

							else firstAParam.Area.AxisX.ScaleView.Position = TimeBegin.ToOADate();
							if (radioButton5.Checked) SetScalePosition2();

							try
							{
							TimeSpan lSpan = DateTime.FromOADate(firstDParam.Area.AxisX.ScaleView.Position) - TimeBegin;
							var v = (int)Math.Round(lSpan.TotalSeconds / _paintedTimeInterval.TotalSeconds * 1000.0);
							_hscrollb.Value = v >= 0 ? v : 0;
							}
                            catch (Exception)
							{
								//MessageBox.Show("15");
							}
						}
						var ar = new CursorEventArgs(firstDParam.Area, firstDParam.Area.AxisX, firstDParam.Area.CursorX.Position);
						CursorPositionView(button1, ar);
						CursorPositionGridView(button1, ar);
					}
				}
				else
				{
					if (ParamsAnalog.First().Area.CursorX.Position > 0)
					{
						bool toBeginFlag = true;
						MomentValue fakeEnd = new MomentReal(TimeEnd, 1, 1);
						MomentValue fakeBegin = new MomentReal(TimeBegin, 1, 1);
						MomentValue ff;
						MomentValue gg = fakeBegin;
						double p1 = firstAParam.Area.CursorX.Position;
						foreach (var graphicParam in ParamsAnalog)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								ff = graphicParam.Dots.FindLast(x => (x.Time < DateTime.FromOADate(p1)));
								int ind = 0;
								//if (DateTime.FromOADate(ParamsAnalog[0].Area.CursorX.Position) - TimeBegin >
								//    new TimeSpan(0,0,0,0,1))
								//    ind = graphicParam.Dots.FindLastIndex(
								//            x => (x.Time < ff.Time && x.ToMomentReal().Mean != ff.ToMomentReal().Mean));
								//else ind = -1;
								if (ff == null) ind = -1;
								else
									if (ParamsAnalog[0].Area.CursorX.Position != TimeBegin.ToOADate())
										ind = graphicParam.Dots.FindLastIndex(
											x => (x.Time < ff.Time && (x.ToMomentReal().Mean != ff.ToMomentReal().Mean || x.Nd != ff.Nd)));
								MomentValue temp = graphicParam.Dots[ind + 1];
								if (gg.Time < temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
									toBeginFlag = false;
								}
							}
						}
						foreach (var graphicParam in ParamsDiscrete)
						{
							if (graphicParam.IsVisible && graphicParam.Dots.Count > 0)
							{
								ff = graphicParam.Dots.FindLast(x => (x.Time < DateTime.FromOADate(p1)));
								int ind = 0;
								//if (ParamsAnalog[0].Area.CursorX.Position != TimeBegin.ToOADate())

								//if (DateTime.FromOADate(ParamsAnalog[0].Area.CursorX.Position) - TimeBegin >
								//    new TimeSpan(0, 0, 0, 0, 1))
								//    ind = graphicParam.Dots.FindLastIndex(
								//        x => (x.Time < ff.Time && x.ToMomentBoolean().Mean != ff.ToMomentBoolean().Mean));
								//else ind = -1;
								if (ff == null) ind = -1;
								else
									if (ParamsAnalog[0].Area.CursorX.Position != TimeBegin.ToOADate())
										ind = graphicParam.Dots.FindLastIndex(
											x => (x.Time < ff.Time && (x.ToMomentReal().Mean != ff.ToMomentReal().Mean || x.Nd != ff.Nd)));
								MomentValue temp = graphicParam.Dots[ind + 1];
								if (gg.Time < temp.Time)
								{
									gg = temp;
									chosenP = graphicParam;
									toBeginFlag = false;
								}
							}
						}
						firstAParam.Area.CursorX.Position = gg.Time.ToOADate();
						//CursorPositionView(button1, null);
						//CursorPositionGridView(button1, null);
						double p2 = firstAParam.Area.CursorX.Position;
						firstDParam.Area.CursorX.Position = p2;
						if (p2 < firstAParam.Area.AxisX.ScaleView.ViewMinimum)
						{
							if (!toBeginFlag)
								firstAParam.Area.AxisX.ScaleView.Position += (p2 - p1) -
										   (firstAParam.Area.AxisX.ScaleView.ViewMaximum -
										   firstAParam.Area.AxisX.ScaleView.Position)/4;
							else firstAParam.Area.AxisX.ScaleView.Position = TimeBegin.ToOADate();
							if (radioButton5.Checked) SetScalePosition2();

                            try
                            {
                                TimeSpan lSpan = DateTime.FromOADate(firstAParam.Area.AxisX.ScaleView.Position) - TimeBegin;
                                var v = (int) Math.Round(lSpan.TotalSeconds/_paintedTimeInterval.TotalSeconds*1000.0);
                                _hscrollb.Value = v >= 0 ? v : 0;
                            }
                            catch (Exception)
                            {
                                //MessageBox.Show("15");
                            }
						}
						var ar = new CursorEventArgs(firstAParam.Area, firstAParam.Area.AxisX, firstAParam.Area.CursorX.Position);
						CursorPositionView(button1, ar);
						CursorPositionGridView(button1, ar);
					}
				}
				label16.BackColor = chosenP.Series.Color;
				label17.BackColor = Color.White;
				label16.Text = chosenP.Index.ToString();
			}
			catch (Exception ex)
			{
				timer2.Stop();
                //MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
                Error = new ErrorCommand("", ex);
			}
		}
		
		//Установка на место графиков: аналоговых высоту и позицию по Y, дискретных позицию по Y
		private void AreaDisRankingY(GraphicParam grParam, int discrParamCounter)
		{
			grParam.Area.Position.Y = 100 * (1 - (discrParamCounter * (AxYHeight) + 2 * (discrParamCounter - 2) + 10f) / chart1.Height);
			grParam.AxCap.Location = new Point(grParam.AxCap.Location.X, (int)(grParam.Area.Position.Y * chart1.Height / 100));
		}
		private void AreaDisRankingHeight(GraphicParam grParam)
		{
			grParam.Area.Position.Height = 100*(AxYLabel + AxYHeight)/chart1.Height;
		}
		private void AreaDisFirstHeight()
		{
			ParamsDiscrete[0].Area.Position.Height = 100*(AxYLabel + (AxYHeight)*(Nods) + 2*(Nods - 1))/chart1.Height;
			ParamsDiscrete[0].Area.Position.Y = 100 * (1 - ((AxYHeight) * (Nods + 1) + 10f + 2 * (Nods - 1)) / chart1.Height);
		}

		private void AreaAnRankingHeight(GraphicParam grParam)
		{
			grParam.Area.Position.Height = (float)((grParam.AxY.Ax.Size.Height - AxSummand - 5) * 100 / .976 / chart1.Height);
		}

		private void AreaAnRankingY(GraphicParam grParam)
		{
			grParam.Area.Position.Y = 100f * (grParam.AxY.Ax.Top + AxSummand) / chart1.Height;
		}

		//Установка на место графиков: аналоговых ширину и позицию по Х, дискретных ширину и позицию по Х
		private void AreaRankingX(GraphicParam grParam)
		{
			//if (ParamsAnalog.Count > 1)
			if (_noas > 1)
			{
				grParam.Area.Position.Width = 100 - 100*((Noas)*(float) AxWidth)/chart1.Width;
				grParam.Area.Position.X = 100*(Noas*(float) AxWidth)/chart1.Width;
			}
			else
			{
				//if (_noas == 1 && ParamsDiscrete.Count == 1 && grParam.AxY.IsHidden)
				//{
				//    grParam.Area.Position.Width = 100;
				//    grParam.Area.Position.X = 0;
				//}
				//else
				//{
					grParam.Area.Position.Width = 100 - 100*(float) AxWidth/chart1.Width;
					grParam.Area.Position.X = 100*(float) AxWidth/chart1.Width;
				//}
			}
			if (grParam.DataTypeD == DataType.Boolean)
				grParam.AxCap.Left = (int) (grParam.Area.Position.X/100*chart1.Width) - AxWidth;
		}

		//Установка на место осей Y
		private void AxesRankingSize(GraphicParam grParam)
		{
			if (!splitContainer1.Panel1.HorizontalScroll.Visible)
                grParam.AxY.Ax.Height = (int)((chart1.Height - AxisXLabelFontHeight - 2 * (Nods - 1) -
                               11 - AxYHeight * (Nods + 1)) * grParam.CurAxSize);
                //grParam.AxY.Ax.Height = (int)((splitContainer1.Panel1.Height - AxisXLabelFontHeight - 2 * (Nods - 1) -
                //               11 - AxYHeight * (Nods + 1)) * grParam.CurAxSize);
			else grParam.AxY.Ax.Height = (int)((chart1.Height - AxisXLabelFontHeight - 2 * (Nods - 1) -
												   31 - AxYHeight * (Nods + 1)) * grParam.CurAxSize);

			grParam.AxY.Ax.Top = (grParam.AxY.Ax.Bottom) > ParamsAnalog.First().AxY.Ax.Bottom
									 ? ParamsAnalog.First().AxY.Ax.Bottom - grParam.AxY.Ax.Height
									 : (int)(ParamsAnalog[0].AxY.Ax.Height * grParam.CurAxPos);

			//grParam.AxY.Ax.Top = ParamsAnalog.First().AxY.Ax.Bottom - grParam.AxY.Ax.Height;
		}

		private void DownerScrollRankingSize()
		{
			_buttonScaleDrop.Top = splitContainer1.Panel1.Size.Height - 20;
			_hscrollb.Width = Convert.ToInt32(ParamsAnalog[0].Area.Position.Width * chart1.Size.Width / 100);
			_buttonScaleDrop.Width = splitContainer1.Panel1.Size.Width -_hscrollb.Width;
			_hscrollb.Location = new Point(_buttonScaleDrop.Width, splitContainer1.Panel1.Size.Height - 20);
		}

		//Сбросить масштабирование по x
		private void ButtonFreeClick(object sender, EventArgs e)
		{
            Button17Click(sender, e);
		}

		private void WheelZooming(object sender, EventArgs e)
		{
			try
			{
			//отображаемый период
			double inter = double.Parse(comboBox5.Text);
			double q = 1.0;
			switch (comboBox2.Text)
			{
				case "Сут.":
					q = _paintedTimeInterval.TotalDays;
					break;
				case "Час.":
					q = _paintedTimeInterval.TotalHours;
					break;
				case "Мин.":
					q = _paintedTimeInterval.TotalMinutes;
					break;
				case "Сек.":
					q = _paintedTimeInterval.TotalSeconds;
					break;
			}
			//comboBox5.Text = sender == button14 ? (double.Parse(comboBox5.Text) - 2).ToString() : (double.Parse(comboBox5.Text) + 2).ToString();
			//процентный дискрет, на который изменяется масштаб по x
			double fD = double.Parse(comboBox1.Text.Remove(comboBox1.Text.Length - 1)) / 100;
			if (sender == button14)
			{
				if (comboBox2.Text != "Сек." || inter > .5/(1-fD))
				{
					comboBox5.Text = (inter*(1-fD)).ToString();
					switch (comboBox2.Text)
					{
						case "Сек.":
							if (inter <= ScaleXMin*60*60*24)
                                comboBox5.Text = (ScaleXMin * 60 * 60 * 24).ToString();
							break;
						case "Мин.":
							if (inter <= .3)
							{
								comboBox2.Text = "Сек.";
								comboBox5.Text = (inter * 60).ToString();
							}
							break;
						case "Час.":
							if (inter <= .3)
							{
								comboBox2.Text = "Мин.";
								comboBox5.Text = (inter * 60).ToString();
							}
							break;
						case "Сут.":
							if (inter <= .3)
							{
								comboBox2.Text = "Час.";
								comboBox5.Text = (inter * 24).ToString();
							}
							break;
					}
				}
				else comboBox5.Text = "0,5";
				Button8Click(null, null);
			}
			else
			{
				if (inter < q/(1+fD))
				{
					comboBox5.Text = (inter*(1+fD)).ToString();
					switch (comboBox2.Text)
					{
						case "Сек.":
							if (inter >= 60)
							{
								comboBox2.Text = "Мин.";
								comboBox5.Text = (inter / 60).ToString();
							}
							break;
						case "Мин.":
							if (inter >= 60)
							{
								comboBox2.Text = "Час.";
								comboBox5.Text = (inter / 60).ToString();
							}
							break;
						case "Час.":
							if (inter >= 24)
							{
								comboBox2.Text = "Сут.";
								comboBox5.Text = (inter / 24).ToString();
							}
							break;
						//case "Сут.":
						//    if (inter <= .3)
						//    {
						//        comboBox2.Text = "Час.";
						//        comboBox5.Text = (inter * 24).ToString();
						//    }
						//    break;
					}
					Button8Click(null, null);
				}
			}
			}
			catch (OverflowException)
			{
				MessageBox.Show("15");
			}
		}
        
		private void AxYToPercent(GraphicParam curParam)
		{
            try
            {
                double locMin = curParam.AxY.ViewMinPrc;
                //double locMax = curParam.ValueToPercent(double.Parse(curParam.AxY.TBoxMax.Text));
                double locMax = curParam.AxY.ViewMaxPrc;
                //curParam.AxY.TBoxMax.Text = locMax.ToString();
                //curParam.AxY.TBoxMin.Text = locMin.ToString();
                for (int y = 0; y <= 3; y++)
                {
                    curParam.AxY.La[y].Text =
                        (locMin + (4 - y) * (-locMin + locMax) / 5).ToString("0.###");
                }
                //textBox1.Text = curParam.ValueToPercent(double.Parse(textBox1.Text)).ToString();
                //textBox2.Text = curParam.ValueToPercent(double.Parse(textBox2.Text)).ToString();
                if (curParam.Index == CurrentParamNumber)
                {
                    //textBox1.Text = curParam.PercentToValue(double.Parse(textBox1.Text)).ToString();
                    //textBox2.Text = curParam.PercentToValue(double.Parse(textBox2.Text)).ToString();
                    textBox2.Text = locMax.ToString();
                    textBox1.Text = locMin.ToString();
                }
                YScaleFlag = false;
                foreach (var x in curParam.AxY.AxesOverlay)
                {
                    YScaleShow(GetParam(x));
                }
                //MessageBox.Show(curParam.AxY.ViewMinPrc + " " + curParam.AxY.ViewMaxPrc +
                //                "\n" + curParam.AxY.ViewMinMsr + " " + curParam.AxY.ViewMaxMsr);
                YScaleFlag = true;
                GraphicParam tempP = curParam.DataTypeD == DataType.Boolean ? ParamsDiscrete[0] : ParamsAnalog[0];
                CursorPositionGridView(null, new CursorEventArgs(tempP.Area, tempP.Area.AxisX, double.NaN));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.StackTrace);
            }
		}

		private void AxYFromPercent(GraphicParam curParam)
		{
            try
            {
                //double tmin = curParam.PercentToValue(double.Parse(curParam.AxY.TBoxMin.Text));
                //double tmax = curParam.PercentToValue(double.Parse(curParam.AxY.TBoxMax.Text));
                double tmin = curParam.AxY.ViewMinMsr;
                double tmax = curParam.AxY.ViewMaxMsr;
                //int tpos = (int)Math.Log10(tmax - tmin) - 1;
                //tmin = ((int)(tmin / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
                //tmax = ((int)(tmax) / Math.Pow(10, tpos)) * Math.Pow(10, tpos);
                //curParam.AxY.TBoxMin.Text = tmin.ToString();
                //curParam.AxY.TBoxMax.Text = tmax.ToString();

                //curParam.AxY.TBoxMax.Text = curParam.PercentToValue(double.Parse(curParam.AxY.TBoxMax.Text)).ToString();
                //curParam.AxY.TBoxMin.Text = curParam.PercentToValue(double.Parse(curParam.AxY.TBoxMin.Text)).ToString();

                if (curParam.Index == CurrentParamNumber)
                {
                    //textBox1.Text = curParam.PercentToValue(double.Parse(textBox1.Text)).ToString();
                    //textBox2.Text = curParam.PercentToValue(double.Parse(textBox2.Text)).ToString();
                    textBox2.Text = tmax.ToString();
                    textBox1.Text = tmin.ToString();
                }
                YScaleFlag = false;
                foreach (var x in curParam.AxY.AxesOverlay)
                {
                    YScaleShow(GetParam(x));
                }
                for (int y = 0; y <= 3; y++)
                {
                    curParam.AxY.La[y].Text =
                        (double.Parse(curParam.AxY.TBoxMin.Text) +
                         (4 - y) * (-double.Parse(curParam.AxY.TBoxMin.Text) + double.Parse(curParam.AxY.TBoxMax.Text)) / 5).ToString(
                             "0.###");
                }
                YScaleFlag = true;
                GraphicParam tempP = curParam.DataTypeD == DataType.Boolean ? ParamsDiscrete[0] : ParamsAnalog[0];
                CursorPositionGridView(null, new CursorEventArgs(tempP.Area, tempP.Area.AxisX, double.NaN));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.StackTrace);
            }
		}

		//private Control _dragger;
		private void ResBMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) _isMouseDown4Move = true;
			_mouseY = e.Y;
			var _dragger = (Control)sender;
			double eY = e.Y;
			switch (sender.GetType().ToString())
			{
				case "System.Windows.Forms.Label":
					_mouseH = ((Label)sender).Parent.Bottom;
					_dragger = ((Label)sender).Parent;
					eY += ((Label)sender).Top;
					break;
				case "System.Windows.Forms.PictureBox":
					_mouseH = ((PictureBox)sender).Parent.Bottom;
					_dragger = ((PictureBox)sender).Parent;
					break;
				case "System.Windows.Forms.Panel":
					_mouseH = ((Panel)sender).Bottom;
					_dragger = (Panel)sender;
					break;
			}
			AxMouseDown(sender, null);

			if (e.Button == MouseButtons.Right)
			{
				GraphicParam curP = GetParam(int.Parse(_dragger.Name));
				foreach (var t in curP.AxY.AxesOverlay)
				{
					GraphicParam tG = GetParam(t);
                    //_expandBufMax = double.Parse(curP.AxY.TBoxMax.Text);
                    //_expandBufMin = double.Parse(curP.AxY.TBoxMin.Text);
                    _expandBufMax = curP.AxY.ViewMaxMsr;
                    _expandBufMin = curP.AxY.ViewMinMsr;
					tG.Area.CursorY.SelectionStart =
						-(-curP.AxY.TBoxMax.Top + eY)
						/ (-curP.AxY.TBoxMax.Top + (double)curP.AxY.TBoxMin.Bottom)
						*(_expandBufMax - _expandBufMin) + _expandBufMax;
				}
			}
		}

		private void ResScndBMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) _isMouseDown4Expand = true;
			_mouseY = e.Y;
			switch (sender.GetType().ToString())
			{
				case "System.Windows.Forms.Label":
					_mouseH = ((Label)sender).Parent.Bottom;
					break;
				case "System.Windows.Forms.PictureBox":
					_mouseH = ((PictureBox)sender).Parent.Bottom;

					var holder = (PictureBox) sender;
					var ax = holder.Parent;
					GraphicParam curP = GetParam(int.Parse(ax.Name));
					_expandBufMax = double.Parse(curP.AxY.TBoxMax.Text);
					_expandBufMin = double.Parse(curP.AxY.TBoxMin.Text);
					break;
				case "System.Windows.Forms.Panel":
					_mouseH = ((Panel)sender).Bottom;
					break;
			}
			AxMouseDown(sender, null);
		}

		private void contextMenuStrip1MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) _isMouseDown4Divide = true;
			var l1 = (ToolStripMenuItem)sender;
			_dividedParam = int.Parse(l1.Text);
		}

		private void ResBMouseUp(object sender, MouseEventArgs e)
		{
			_isMouseDown4Move = false;
			var ax = ((PictureBox) sender).Parent;
			GraphicParam curP = GetParam(int.Parse(ax.Name));
			curP.CurAxPos = (double) (ax.Top)/(ParamsAnalog.First().AxY.Ax.Height - 2*AxSummand);
			curP.CurAxSize = (double)(ax.Height - 2 * AxSummand) / (ParamsAnalog.First().AxY.Ax.Height - 2 * AxSummand);
		}

		private void ResScndBMouseUp(object sender, MouseEventArgs e)
		{
			_isMouseDown4Expand = false;
			var ax = ((PictureBox)sender).Parent;
			GraphicParam curP = GetParam(int.Parse(ax.Name));
			//curP.CurAxPos = (double)ax.Top / ParamsAnalog.First().AxY.Ax.Height;
			//curP.CurAxSize = (double)ax.Height / ParamsAnalog.First().AxY.Ax.Height;

			curP.AxY.ResizeAreaButtonTop.Top = curP.AxY.TBoxMax.Bottom;
			curP.AxY.ResizeAreaButtonBottom.Top = curP.AxY.TBoxMin.Top - 3;

			foreach (var t in curP.AxY.AxesOverlay)
			{
				GraphicParam tparam = GetParam(t);
				AreaAnRankingY(tparam);
				AreaAnRankingHeight(tparam);
                TextBoxMChange(tparam);
			}
		}

		private void Chart1MouseUp(object sender, MouseEventArgs e)
		{
			try
			{
				if (_isMouseDown4Divide) //расслаивание слепленных осей
				{
					GraphicParam dividedParam = GetParam(_dividedParam);
					if (dividedParam.AxY.IsOverlayed)
					{
						int overlayNext = VisibleOverlaySearch(dividedParam);
						GraphicParam xParam = GetParam(overlayNext) ??
											  GetParam(dividedParam.AxY.AxesOverlay.Find(x => x != _dividedParam));
						//if (dividedParam.IsVisible)
						//{
						AxesY.Add(new AxY(dividedParam, AxWidth));
						dividedParam.AxY = AxesY.Last();
						//AxesY.Sort((y, axY) =>
						//               {
						//                   if (y.UpperParam.Index > axY.UpperParam.Index) return 1;
						//                   if (y.UpperParam.Index < axY.UpperParam.Index) return -1;
						//                   return 0;
						//               });
						dividedParam.AxY.UpperParam = dividedParam;
						//dataGridView1[0, dividedParam.Index - 1].Style.BackColor = dividedParam.Series.Color;
						//dataGridView1[0, dividedParam.Index - 1].Style.SelectionBackColor = dividedParam.Series.Color;
						WantedRow(dividedParam.Index).Cells[0].Style.BackColor = dividedParam.Series.Color;
						WantedRow(dividedParam.Index).Cells[0].Style.SelectionBackColor = dividedParam.Series.Color;

						xParam.AxY.UpperParam = xParam;

                        //возвращаем осям Y их точности
                        xParam.AxY.DecPlacesMaskFill(xParam.DecPlaces);
                        dividedParam.AxY.DecPlacesMaskFill(dividedParam.DecPlaces);
                        xParam.AxY.ViewMaxMsr = xParam.AxY.ViewMaxMsr;
                        xParam.AxY.ViewMaxPrc = xParam.AxY.ViewMaxPrc;
                        xParam.AxY.ViewMinMsr = xParam.AxY.ViewMinMsr;
                        xParam.AxY.ViewMinPrc = xParam.AxY.ViewMinPrc;

						dividedParam.AxY.Ax.MouseUp += MovBMouseUp;
						dividedParam.AxY.Ax.MouseDown += ResBMouseDown;
						dividedParam.AxY.Ax.MouseMove += AxMBMove;

						AxesRankingSize(dividedParam);
						dividedParam.AxY.Ax.Top = xParam.AxY.Ax.Top;
						dividedParam.AxY.Ax.Height = xParam.AxY.Ax.Height;
						//dividedParam.AxY.Ax.Click += AxMouseDown;
						dividedParam.AxY.Ax.DoubleClick += AxMouseDouble;
						dividedParam.AxY.Ax.Resize += AxResize;
						dividedParam.AxY.ResizeAxButtonTop.MouseDown += ResBMouseDown;
						dividedParam.AxY.ResizeAxButtonTop.MouseUp += ResBMouseUp;
						dividedParam.AxY.ResizeAxButtonTop.MouseMove += AxResizeAreaTop;
						dividedParam.AxY.TBoxMax.LostFocus += TextBoxMaxLeave;
						dividedParam.AxY.TBoxMin.LostFocus += TextBoxMinLeave;
						dividedParam.AxY.TBoxMin.KeyPress += TextBoxMinKeyPress;
						dividedParam.AxY.TBoxMax.KeyPress += TextBoxMaxKeyPress;

						dividedParam.AxY.AxCap.MouseClick += OverlayTopChoose;
						dividedParam.AxY.RulePBoxes[1].MouseDown += AxControlDetect;
						//dividedParam.AxY.RulePBoxes[1].MouseUp += MovBMouseUp;
						//dividedParam.AxY.RulePBoxes[1].MouseDown += ResBMouseDown;
						//dividedParam.AxY.RulePBoxes[1].MouseMove += AxMBMove;
						dividedParam.AxY.RulePBoxes[0].MouseDown += AxControlDetect;
						dividedParam.AxY.RulePBoxes[1].MouseClick += HideGrClick;
                        //dividedParam.AxY.RulePBoxes[0].MouseClick += HideAxClick;
                        dividedParam.AxY.RulePBoxes[0].MouseClick += HideAx;
						dividedParam.AxY.TBoxMax.KeyPress += TextBoxInputAnyReal;
						dividedParam.AxY.TBoxMax.Enter += AxControlDetect;
						dividedParam.AxY.ResizeAxButtonBottom.MouseDown += ResBMouseDown;
						dividedParam.AxY.ResizeAxButtonBottom.MouseUp += ResBMouseUp;
						dividedParam.AxY.ResizeAxButtonBottom.MouseMove += AxResizeAreaBottom;
						dividedParam.AxY.TBoxMin.KeyPress += TextBoxInputAnyReal;
						dividedParam.AxY.TBoxMin.Enter += AxControlDetect;

						dividedParam.AxY.ResizeAreaButtonBottom.MouseDown += ResScndBMouseDown;
						dividedParam.AxY.ResizeAreaButtonBottom.MouseUp += ResScndBMouseUp;
						dividedParam.AxY.ResizeAreaButtonBottom.MouseMove += AxResizeBottom;

						dividedParam.AxY.ResizeAreaButtonTop.MouseDown += ResScndBMouseDown;
						dividedParam.AxY.ResizeAreaButtonTop.MouseUp += ResScndBMouseUp;
						dividedParam.AxY.ResizeAreaButtonTop.MouseMove += AxResizeTop;

						for (int j = 0; j < 4; j++)
						{
							dividedParam.AxY.La[j].MouseDown += AxControlDetect;
							dividedParam.AxY.La[j].DoubleClick += AxMouseDouble;
							dividedParam.AxY.La[j].MouseDown += ResBMouseDown;
							dividedParam.AxY.La[j].MouseMove += AxMBMove;
							dividedParam.AxY.La[j].MouseUp += MovBMouseUp;
							dividedParam.AxY.La[j].Top = (j + 1)*
														 (dividedParam.AxY.Ax.Height - dividedParam.AxY.AxCap.Height
														  - dividedParam.AxY.RulePanel.Height - 2)/5 - 10
														 + dividedParam.AxY.RulePanel.Height + 2;
						}
						dividedParam.AxY.ResizeAreaButtonTop.Top = dividedParam.AxY.TBoxMax.Bottom;
						dividedParam.AxY.ResizeAreaButtonBottom.Top = dividedParam.AxY.TBoxMin.Top - 3;

						bool needToIncreaseNoas = true;
						if (overlayNext < 1)
						{
							xParam.AxY.Ax.Visible = false;
							needToIncreaseNoas = false;
						}
						else
						{
							if (!dividedParam.IsVisible)
							{
								dividedParam.AxY.Ax.Visible = false;
								needToIncreaseNoas = false;
							}
						}
						if (needToIncreaseNoas) Noas++;

						chart1.Controls.Add(dividedParam.AxY.Ax);
						dividedParam.AxY.TBoxMax.Text = xParam.AxY.TBoxMax.Text;
                        dividedParam.AxY.TBoxMin.Text = xParam.AxY.TBoxMin.Text;
                        dividedParam.AxY.ViewMaxMsr = xParam.AxY.ViewMaxMsr;
                        dividedParam.AxY.ViewMaxPrc = xParam.AxY.ViewMaxPrc;
                        dividedParam.AxY.ViewMinMsr = xParam.AxY.ViewMinMsr;
                        dividedParam.AxY.ViewMinPrc = xParam.AxY.ViewMinPrc;
						for (int index = 0; index < dividedParam.AxY.La.Length; index++)
						{
							dividedParam.AxY.La[index].Text = xParam.AxY.La[index].Text;
						}

						xParam.AxY.AxesOverlay.Remove(dividedParam.Index);
						//dividedParam.AxY.AxesOverlay.Add(dividedParam.Index);
						dividedParam.AxY.IsOverlayed = false;
						//dividedParam.AxCap.Text = dividedParam.AxCap.Text.Substring(0, dividedParam.AxCap.Text.Length - 3);

						WantedRow(_dividedParam).Cells["Group"].Value = dividedParam.Index;
						foreach (var t in xParam.AxY.AxesOverlay)
						{
							//dataGridView1[0, t - 1].Style.BackColor = xParam.Series.Color;
							//dataGridView1[0, t - 1].Style.SelectionBackColor = xParam.Series.Color;
							WantedRow(t).Cells[0].Style.BackColor = xParam.Series.Color;
							WantedRow(t).Cells[0].Style.SelectionBackColor = xParam.Series.Color;
							WantedRow(t).Cells["Group"].Value = xParam.Index;
						}
						dataGridView1.Sort(new DatagridColorSort());

						if (xParam.AxY.AxesOverlay.Count == 1)
						{
							//xParam.AxCap.Text = xParam.AxCap.Text.Substring(0, xParam.AxCap.Text.Length - 3);
							xParam.AxY.IsOverlayed = false;
						}
						int anParamCounter = 0;
						foreach (var t in ParamsAnalog)
						{
							AreaRankingX(t);
							if (t.IsVisible && t.Index == t.AxY.UpperParam.Index)
							{
								t.AxY.Ax.Left = anParamCounter*AxWidth;
								anParamCounter++;
							}
						}
						foreach (var t in ParamsDiscrete)
						{
							AreaRankingX(t);
						}
						//foreach (var a in AxesY)
						//{
						//    foreach (var cntr in a.AxesOverlay)
						//    {
						//        AreaRankingX(GetParam(cntr));
						//    }
						//    a.Ax.Left = anParamCounter*AxWidth;
						//    if (a.UpperParam.IsVisible) anParamCounter++;
						//}
					}
					_isMouseDown4Divide = false;
					contextMenuStrip1.Hide();
				}
				if (e.Button == MouseButtons.Right)
				{
					//foreach (var t in ParamsAnalog)
					//{
					//    t.Area.AxisY.ScaleView.Size = ParamsAnalog[0].Area.AxisY.ScaleView.Size;
					//    t.Area.AxisY.Interval = t.Area.AxisY.ScaleView.Size / IntervalCount;
					//    t.Area.AxisY.ScaleView.Position = ParamsAnalog[0].Area.AxisY.ScaleView.Position;

					//    t.Area.AxisY.MajorGrid.Interval = (t.Area.AxisY.ScaleView.ViewMaximum -
					//                                       t.Area.AxisY.ScaleView.ViewMinimum) / 4;
					//    if (t == t.AxY.UpperParam)
					//    {
					//        t.AxY.TBoxMax.Text = t.Area.AxisY.ScaleView.ViewMaximum.ToString();
					//        t.AxY.TBoxMin.Text = t.Area.AxisY.ScaleView.ViewMinimum.ToString();
					//    }
					//}

					//ParamsAnalog[0].Area.AxisY.ScaleView.ZoomReset();

                    //Как считается, какой период по Y отображать: (выбранный период)/(весь отображенный период)
                    //С позиции: (максимум по Y - текущая позиция)/(весь отображенный период)
					double cursPos = Math.Max(ParamsAnalog[0].Area.CursorY.SelectionEnd,
											  ParamsAnalog[0].Area.CursorY.SelectionStart);
					double cursSize =
						Math.Abs(ParamsAnalog[0].Area.CursorY.SelectionEnd -
								 ParamsAnalog[0].Area.CursorY.SelectionStart);
					if (cursSize != 0)
					{
                        button23.UseVisualStyleBackColor = true;
						//сначала определили область выделения пользователем, а потом сбросили приближение и вычислили
						//относительные значения
						cursPos = ParamsAnalog[0].Area.AxisY.Maximum - cursPos;
						cursPos /= (ParamsAnalog[0].Area.AxisY.Maximum - ParamsAnalog[0].Area.AxisY.Minimum);
						cursSize /= (ParamsAnalog[0].Area.AxisY.Maximum - ParamsAnalog[0].Area.AxisY.Minimum);
						for (int index = 1; index < AxesY.Count; index++)
						{
							var axY = AxesY[index];
							GraphicParam param = axY.UpperParam;
							double tbMin;// = double.Parse(axY.TBoxMin.Text);
                            double tbMax;// = double.Parse(axY.TBoxMax.Text);
                            //GraphicParam tG = axY.UpperParam;
                            //switch (tG.PercentMode)
                            //{
                            //    case PercentModeClass.Absolute:
                            //        tbMax = axY.ViewMaxMsr;
                            //        tbMin = axY.ViewMinMsr;
                            //        break;
                            //    case PercentModeClass.Percentage:
                            //        tbMax = axY.ViewMaxPrc;
                            //        tbMin = axY.ViewMinPrc;
                            //        break;
                            //}

                            tbMax = axY.ViewMaxMsr;
                            tbMin = axY.ViewMinMsr;
                            int tpos = (int)Math.Log10(tbMax - tbMin) - 2;
						    int tposPrc =
                                (int)Math.Log10(axY.ViewMaxPrc - axY.ViewMinPrc) - 2;
							if (tbMax - tbMin > 0.00001*(param.Area.AxisY.Maximum - param.Area.AxisY.Minimum))
							{
								if (cursPos >= param.CurAxPos)
								{
									if (cursSize + cursPos < param.CurAxPos + param.CurAxSize)
									{
                                        double tmin = (tbMax - tbMin)
                                                            * (param.CurAxPos + param.CurAxSize - cursPos - cursSize)
                                                            / param.CurAxSize + tbMin;
									    double tmax = (tbMax - tbMin)*(param.CurAxPos + param.CurAxSize - cursPos)
									                  /param.CurAxSize + tbMin;

                                        axY.ViewMaxMsr = ((int)(tmax / Math.Pow(10, tpos))) * Math.Pow(10, tpos);//tmax;
                                        axY.ViewMinMsr = ((int)(tmin / Math.Pow(10, tpos))) * Math.Pow(10, tpos);//tmin;
                                        axY.ViewMaxPrc = ((int)(param.ValueToPercent(tmax) / Math.Pow(10, tposPrc))) * Math.Pow(10, tposPrc);//tG.ValueToPercent(tmax);
                                        axY.ViewMinPrc = ((int)(param.ValueToPercent(tmin) / Math.Pow(10, tposPrc))) * Math.Pow(10, tposPrc);//tG.ValueToPercent(tmin);
									}
									else
									{
										if (cursPos < param.CurAxPos + param.CurAxSize)
                                        {
                                            //axY.TBoxMax.Text = ((tbMax - tbMin)
                                            //                    *(param.CurAxPos + param.CurAxSize - cursPos)
                                            //                    /param.CurAxSize + tbMin).ToString();
                                            double tmax = ((int)(((tbMax - tbMin)
                                                            * (param.CurAxPos + param.CurAxSize - cursPos)
                                                            / param.CurAxSize + tbMin) / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
                                            //axY.TBoxMax.Text = tmax.ToString();
                                            //GraphicParam t = axY.UpperParam;
                                            //switch (t.PercentMode)
                                            //{
                                            //    case PercentModeClass.Absolute:

                                                    //axY.ViewMaxMsr = tmax;
                                                    //axY.ViewMaxPrc = tG.ValueToPercent(tmax);
                                            axY.ViewMaxMsr = ((int)(tmax / Math.Pow(10, tpos))) * Math.Pow(10, tpos);//tmax;
                                            axY.ViewMaxPrc = ((int)(param.ValueToPercent(tmax) / Math.Pow(10, tposPrc))) * Math.Pow(10, tposPrc);

                                            //        break;
                                            //    case PercentModeClass.Percentage:
                                            //        axY.ViewMaxMsr = t.PercentToValue(tmax);
                                            //        axY.ViewMaxPrc = tmax;
                                            //        break;
                                            //}
                                        }
									}
								}
								else
								{
									if (cursSize + cursPos < param.CurAxPos + param.CurAxSize)
									{
										if (cursSize + cursPos > param.CurAxPos)
                                        {
                                            //axY.TBoxMin.Text = (-(tbMax - tbMin)
                                            //                    *(-param.CurAxPos + cursPos + cursSize)
                                            //                    /param.CurAxSize + tbMax).ToString();
                                            double tmin = ((int)(((tbMax - tbMin)
                                                            * (param.CurAxPos + param.CurAxSize - cursPos - cursSize)
                                                            / param.CurAxSize + tbMin) / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
                                            //axY.TBoxMin.Text = tmin.ToString();
                                            //GraphicParam t = axY.UpperParam;
                                            //switch (t.PercentMode)
                                            //{
                                            //    case PercentModeClass.Absolute:

                                                    //axY.ViewMinMsr = tmin;
                                                    //axY.ViewMinPrc = tG.ValueToPercent(tmin);

                                                    axY.ViewMinMsr = ((int)(tmin / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
                                                    axY.ViewMinPrc = ((int)(param.ValueToPercent(tmin) / Math.Pow(10, tposPrc))) * Math.Pow(10, tposPrc);
                                            //        break;
                                            //    case PercentModeClass.Percentage:
                                            //        axY.ViewMinMsr = t.PercentToValue(tmin);
                                            //        axY.ViewMinPrc = tmin;
                                            //        break;
                                            //}
                                        }
									}
                                }
                                foreach (var x in param.AxY.AxesOverlay)
                                {
                                    YScaleDraw(GetParam(x));
                                }
							}
						}
						//ParamsAnalog[0].Area.CursorY.SelectionStart = double.NaN;
						ParamsAnalog[0].Area.CursorY.SelectionEnd = double.NaN;
                        textBox1.Text = GetParam(CurrentParamNumber).AxY.TBoxMin.Text;
                        textBox2.Text = GetParam(CurrentParamNumber).AxY.TBoxMax.Text;
					    //chart1.Refresh();
					}
				}
			}
			catch (Exception exception)
			{
				Error = new ErrorCommand("", exception);
			}
		}

        //private void MovBMouseDown(object sender, MouseEventArgs e)
        //{
        //    //var ax = sender.GetType().ToString() == "System.Windows.Forms.Panel"
        //    //             ? ((Panel)sender)
        //    //             : ((PictureBox)sender).Parent.Parent;
        //    //ax.DoDragDrop(ax.Name, DragDropEffects.Copy | DragDropEffects.Move);
        //    if (e.Button == MouseButtons.Right && ParamsAnalog.Count > 1)
        //    {
        //        ParamsAnalog[0].Area.CursorY.SelectionStart = (ParamsAnalog[0].Area.AxisY.ScaleView.ViewMaximum -
        //                                                     ParamsAnalog[0].Area.AxisY.ScaleView.ViewMinimum) *
        //                                                    (ParamsAnalog[0].AxY.Ax.Height - _axSummand - e.Y) /
        //                                                    (ParamsAnalog[0].AxY.Ax.Height - 2 * _axSummand) +
        //                                                    ParamsAnalog[0].Area.AxisY.ScaleView.ViewMinimum;
        //    }
        //}

		//private void Ax_DragEnter(object sender, DragEventArgs e)
		//{
		//    e.Effect = DragDropEffects.Copy;
		//}

		//private void AxDragDrop(object sender, DragEventArgs e)
		//{
		//    Text = e.Data.GetData(DataFormats.Text).ToString();
		//    //Text = _isMouseDown.ToString();
		//}

		private void MovBMouseUp(object sender, MouseEventArgs e)
		{
			_isMouseDown4Move = false;
			Cursor = Cursors.Default;
			//var ax = ((PictureBox)sender).Parent.Parent;
			//var ax = ((Panel)sender);
			//var ax = (Control)sender;
			//var ax = sender.GetType().ToString() == "System.Windows.Forms.Label"
			//                 ? ((Label)sender).Parent
			//                 : ((Panel)sender);
			Control ax;
			int ad;
			double eY = e.Y;
			if (sender.GetType().ToString() == "System.Windows.Forms.Label")
			{
				ax = ((Label)sender).Parent;
				ad = ((Label)sender).Top;
				eY += ((Label) sender).Top;
			}
			else
			{
				ax = (Panel)sender;
				ad = 0;
			}
			GraphicParam curP = GetParam(CurrentParamNumber);
			//curP.CurAxPos = (double)ax.Top / ParamsAnalog.First().AxY.Ax.Height;
			//curP.CurAxSize = (double)ax.Height / ParamsAnalog.First().AxY.Ax.Height;
			curP.CurAxPos = (double)(ax.Top) / (ParamsAnalog.First().AxY.Ax.Height - 2 * AxSummand);
			//curP.CurAxSize = (double)(ax.Height - 2 * _axSummand) / (ParamsAnalog.First().AxY.Ax.Height - 2 * _axSummand);

			double xD = ((double)(e.X + ax.Left) / AxWidth + 1);
			if (xD <= Noas + 1 && e.Button == MouseButtons.Left)
			{
				int current = 0;
				//for (int i = 1; i < xD; current++)
				//{
				//    if (ParamsAnalog[current].AxY.UpperParam.Index == ParamsAnalog[current].Index
				//        && ParamsAnalog[current].AxY.Ax.Visible && !ParamsAnalog[current].AxY.IsHidden)
				//    {
				//        i++;
				//    }
				//}
				//current--;
				//current = ParamsAnalog[current].AxY.UpperParam.Index;
				foreach (var axY in AxesY)
				{
					if (axY.Ax.Left / AxWidth + 1 < xD && axY.Ax.Right / AxWidth + 1 >= xD && axY.Ax.Visible)
					{
						current = axY.UpperParam.Index;
						break;
					}
				}

				if (current > 0 
                    && CurrentParamNumber != current 
					&& e.Y + curP.AxY.Ax.Top + ad <= GetParam(current).AxY.Ax.Bottom
					&& e.Y + curP.AxY.Ax.Top + ad >= GetParam(current).AxY.Ax.Top
					)
					Amalgamation(curP, GetParam(current));

				//dataGridView1.CurrentCell = dataGridView1.Rows[CurrentParamNumber - 1].Cells[1];
				dataGridView1.CurrentCell = WantedRow(CurrentParamNumber).Cells[1];
				//DatagridSelectionChanged(null, null);
			}

			if (e.Button == MouseButtons.Right)
			{
				//GraphicParam curP = GetParam(int.Parse(_dragger.Name));
				foreach (var t in curP.AxY.AxesOverlay)
				{
					GraphicParam tG = GetParam(t);
                    //_expandBufMax = double.Parse(curP.AxY.TBoxMax.Text);
                    //_expandBufMin = double.Parse(curP.AxY.TBoxMin.Text);
                    _expandBufMax = curP.AxY.ViewMaxMsr;
				    _expandBufMin = curP.AxY.ViewMinMsr;
					double yD = _expandBufMax -
									(-curP.AxY.TBoxMax.Top + eY) /
									(-curP.AxY.TBoxMax.Top + (double)curP.AxY.TBoxMin.Bottom)
									* (_expandBufMax - _expandBufMin);
                    //if (Math.Abs(yD - tG.Area.CursorY.SelectionStart) > 0.05 * tG.Area.AxisY.ScaleView.Size)
                    if (Math.Abs(yD - tG.Area.CursorY.SelectionStart) > 0.00001*(tG.Area.AxisY.Maximum - tG.Area.AxisY.Minimum)&&
                        Math.Abs(yD - tG.Area.CursorY.SelectionStart) > 0.05 * tG.Area.AxisY.ScaleView.Size)
                    {
                        double tmax = Math.Max(tG.Area.CursorY.SelectionStart, tG.Area.CursorY.SelectionEnd);
                        double tmin = Math.Min(tG.Area.CursorY.SelectionStart, tG.Area.CursorY.SelectionEnd);
                        int tpos = (int) Math.Log10(tmax - tmin) - 2;
                        double tempMax = Math.Pow(10, tpos)*(int) (tmax/Math.Pow(10, tpos));
                        double tempMin = Math.Pow(10, tpos)*(int) (tmin/Math.Pow(10, tpos));
                        //tG.AxY.TBoxMax.Text = (tempMax).ToString();
                        //tG.AxY.TBoxMin.Text = (tempMin).ToString();
                        //switch (tG.PercentMode)
                        //{
                        //    case PercentModeClass.Absolute:
                                tG.AxY.ViewMaxMsr = tempMax;
                                tG.AxY.ViewMinMsr = tempMin;
                                int tposPrc = (int)Math.Log10(tG.ValueToPercent(tmax) - tG.ValueToPercent(tmin)) - 2;
                                //tG.AxY.ViewMaxPrc = tG.ValueToPercent(tempMax);
                                //tG.AxY.ViewMinPrc = tG.ValueToPercent(tempMin);
                                tG.AxY.ViewMaxPrc = Math.Pow(10, tposPrc) * (int)(tG.ValueToPercent(tmax) / Math.Pow(10, tposPrc));
                                tG.AxY.ViewMinPrc = Math.Pow(10, tposPrc) * (int)(tG.ValueToPercent(tmin) / Math.Pow(10, tposPrc));
                        //        break;
                        //    case PercentModeClass.Percentage:
                        //        tG.AxY.ViewMaxMsr = tG.PercentToValue(tempMax);
                        //        tG.AxY.ViewMinMsr = tG.PercentToValue(tempMin);
                        //        tG.AxY.ViewMaxPrc = tempMax;
                        //        tG.AxY.ViewMinPrc = tempMin;
                        //        break;
                        //}
                                TextBoxMChange(tG);
                    }

                    tG.Area.CursorY.SelectionEnd = double.NaN;
				    //tG.Area.CursorY.SelectionStart = double.NaN;
				    tG.IsYScaledAuto = false;
				}

                button23.UseVisualStyleBackColor = true;
                textBox1.Text = curP.AxY.TBoxMin.Text;
                textBox2.Text = curP.AxY.TBoxMax.Text;
			}
		}

		//Слитие осей
		private void Amalgamation(GraphicParam aParam, GraphicParam bParam)
		{
            try
            {
                //AxControlDetect(aParam.AxY.RulePBoxes[0], null); //сделали ось активной

                if (bParam.PercentMode == PercentModeClass.Absolute ^ aParam.PercentMode == PercentModeClass.Absolute)
                    Button24Click(null, null);

                bParam.AxY.IsOverlayed = true;
                bParam.AxY.AxesOverlay.AddRange(aParam.AxY.AxesOverlay);

                aParam.CurAxPos = bParam.CurAxPos;
                aParam.CurAxSize = bParam.CurAxSize;
                bParam.AxY.UpperParam = aParam;

                chart1.Controls.Remove(aParam.AxY.Ax);
                AxesY.Remove(aParam.AxY);
                int pr = -1;
                foreach (var a in aParam.AxY.AxesOverlay)
                {
                    GetParam(a).AxY = bParam.AxY;
                    WantedRow(a).Cells[0].Style.BackColor = aParam.Series.Color;
                    WantedRow(a).Cells[0].Style.SelectionBackColor = aParam.Series.Color;
                    WantedRow(a).Cells["Group"].Value = aParam.Index;
                    pr = GetParam(a).DecPlaces == -1 ? 999 : Math.Max(GetParam(a).DecPlaces, pr);
                }
                foreach (var b in bParam.AxY.AxesOverlay)
                {
                    WantedRow(b).Cells[0].Style.BackColor = aParam.Series.Color;
                    WantedRow(b).Cells[0].Style.SelectionBackColor = aParam.Series.Color;
                    WantedRow(b).Cells["Group"].Value = aParam.Index;
                    pr = GetParam(b).DecPlaces == -1 ? 999 : Math.Max(GetParam(b).DecPlaces, pr);
                }

                //устанавливаем точность максимальную (небесконечную) среди сливаемых графиков
                if (pr == 999) pr = -1;
                //int pr = aParam.DecPlaces < 0 || bParam.DecPlaces < 0 ? -1 : Math.Max(bParam.DecPlaces, aParam.DecPlaces);
                bParam.AxY.DecPlacesMaskFill(pr);
                //string pr;
                //if (aParam.AxY.DecPlacesMask == "" || aParam.AxY.DecPlacesMask == "") pr = "";
                //else if (aParam.AxY.DecPlacesMask.Length > bParam.AxY.DecPlacesMask.Length) bParam.AxY.DecPlacesMask = aParam.AxY.DecPlacesMask;
                bParam.AxY.ViewMaxMsr = bParam.AxY.ViewMaxMsr;
                bParam.AxY.ViewMaxPrc = bParam.AxY.ViewMaxPrc;
                bParam.AxY.ViewMinMsr = bParam.AxY.ViewMinMsr;
                bParam.AxY.ViewMinPrc = bParam.AxY.ViewMinPrc;

                dataGridView1.Sort(new DatagridColorSort());
                if (!aParam.AxY.IsHidden) Noas--;
                aParam.AxY = bParam.AxY;
                int nmbrOfVsblAnGrs = 0;
                foreach (GraphicParam t in ParamsAnalog)
                {
                    if (t.AxY.Ax.Visible && !t.AxY.IsHidden && t.Index == t.AxY.UpperParam.Index)
                    {
                        nmbrOfVsblAnGrs++;
                        t.AxY.Ax.Left = (nmbrOfVsblAnGrs - 1)*AxWidth;
                    }
                    AreaRankingX(t);
                    AreaAnRankingHeight(t);
                    AreaAnRankingY(t);
                }
                foreach (var t in ParamsDiscrete)
                {
                    AreaRankingX(t);
                }
                DownerScrollRankingSize();
                //TextBoxMLeave(aParam, null);
                foreach (var x in aParam.AxY.AxesOverlay)
                {
                    YScaleDraw(GetParam(x));
                }

                aParam.IsYScaledAuto = false;
                //button23.BackColor = Color.White;
                button23.UseVisualStyleBackColor = true;

                //dataGridView1.CurrentCell = dataGridView1[1, aParam.Index - 1];
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
            }
		}

		private void AxResizeAreaBottom(object sender, MouseEventArgs e)
		{
            try
            {
                if (_isMouseDown4Move)
                {
                    var ax = ((PictureBox) sender).Parent;
                    if ((ax.Bottom + e.Y) - _mouseY <= ParamsAnalog.First().AxY.Ax.Bottom)
                    {
                        ax.Height = ax.Bottom + e.Y - ax.Top;
                    }
                    else ax.Height = ParamsAnalog.First().AxY.Ax.Bottom - ax.Top;
                    //GraphicParam curP = GetParam(int.Parse(ax.Name));
                    //AreaAnRankingHeight(curP);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

        private void AxResizeAreaTop(object sender, MouseEventArgs e)
		{
            try
            {
                if (_isMouseDown4Move)
                {
                    var ax = ((PictureBox) sender).Parent;
                    if ((ax.Top + e.Y) >= 0)
                    {
                        ax.Height = ax.Bottom - e.Y - ax.Top;
                        if ((ax.Bottom + e.Y) <= ParamsAnalog.First().AxY.Ax.Bottom)
                        {
                            ax.Top = (ax.Top + e.Y);
                        }
                        else
                        {
                            ax.Top = ParamsAnalog.First().AxY.Ax.Bottom - ax.Height;
                        }
                    }
                    else
                    {
                        ax.Top = 0;
                        ax.Height = _mouseH;
                    }
                    //GraphicParam curP = GetParam(int.Parse(ax.Name));
                    //AreaAnRankingY(curP);
                    foreach (var t in GetParam(int.Parse(ax.Name)).AxY.AxesOverlay)
                    {
                        AreaAnRankingY(GetParam(t));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private double _expandBufMax;
		private double _expandBufMin;

		private void AxResizeTop(object sender, MouseEventArgs e)
		{
            try
            {
                if (_isMouseDown4Expand)
                {
                    var holder = (PictureBox) sender;
                    var ax = holder.Parent;
                    GraphicParam curP = GetParam(int.Parse(ax.Name));
                    if (holder.Top + e.Y >= curP.AxY.TBoxMax.Bottom)
                    {
                        if (holder.Top + e.Y < curP.AxY.ResizeAreaButtonBottom.Top - 5)
                        {
                            holder.Top = holder.Top + e.Y;
                            foreach (var t in curP.AxY.AxesOverlay)
                            {
                                GraphicParam tparam = GetParam(t);
                                tparam.Area.Position.Height = (float) ((tparam.AxY.Ax.Size.Height - AxSummand - 5
                                                                        + tparam.AxY.TBoxMax.Bottom - holder.Top)*100/
                                                                       .976/chart1.Height);
                                tparam.Area.Position.Y = 100f*
                                                         (tparam.AxY.Ax.Top + holder.Top - tparam.AxY.TBoxMax.Height + 2)/
                                                         chart1.Height;
                            }
                        }
                        else
                        {
                            holder.Top = curP.AxY.ResizeAreaButtonBottom.Top - 5;
                            foreach (var t in curP.AxY.AxesOverlay)
                            {
                                GraphicParam tparam = GetParam(t);
                                tparam.Area.Position.Height = (float) ((tparam.AxY.Ax.Size.Height - AxSummand - 5
                                                                        + tparam.AxY.TBoxMax.Bottom - holder.Top)*100/
                                                                       .976/chart1.Height);
                                tparam.Area.Position.Y = 100f*
                                                         (tparam.AxY.Ax.Top + holder.Top - tparam.AxY.TBoxMax.Height + 2)/
                                                         chart1.Height;
                            }
                        }
                    }
                    else
                    {
                        holder.Top = curP.AxY.TBoxMax.Bottom;
                        foreach (var t in curP.AxY.AxesOverlay)
                        {
                            GraphicParam tparam = GetParam(t);
                            tparam.Area.Position.Height = (float) ((tparam.AxY.Ax.Size.Height - AxSummand - 5
                                                                    + tparam.AxY.TBoxMax.Bottom - holder.Top)*100/.976/
                                                                   chart1.Height);
                            tparam.Area.Position.Y = 100f*
                                                     (tparam.AxY.Ax.Top + holder.Top - tparam.AxY.TBoxMax.Height + 2)/
                                                     chart1.Height;
                        }
                    }
                    //curP.AxY.TBoxMax.Text = ((curP.AxY.Ax.Height - holder.Top + (double)curP.AxY.TBoxMax.Bottom) / curP.AxY.Ax.Height
                    //    * (_expandBufMax - _expandBufMin) + _expandBufMin).ToString();
                    double xD = (curP.AxY.TBoxMin.Top - (double) curP.AxY.TBoxMax.Bottom - holder.Height + 40)
                                /(curP.AxY.TBoxMin.Top - holder.Top - holder.Height + 40)
                                *(_expandBufMax - _expandBufMin) + _expandBufMin;
                    //curP.AxY.TBoxMax.Text = (xD).ToString();

                    //switch (curP.PercentMode)
                    //{
                    //    case PercentModeClass.Absolute:

                    //curP.AxY.ViewMaxMsr = xD;
                    //curP.AxY.ViewMaxPrc = curP.ValueToPercent(xD);
                    int tpos = (int) Math.Log10(xD - curP.AxY.ViewMinMsr) - 2;
                    curP.AxY.ViewMaxMsr = ((int) (xD/Math.Pow(10, tpos)))*Math.Pow(10, tpos);
                    int tposPrc =
                        (int)
                        Math.Log10(curP.ValueToPercent(curP.AxY.ViewMaxMsr) - curP.ValueToPercent(curP.AxY.ViewMinMsr)) -
                        2;
                    curP.AxY.ViewMaxPrc = ((int) (curP.ValueToPercent(xD)/Math.Pow(10, tposPrc)))*Math.Pow(10, tposPrc);

                    //        break;
                    //    case PercentModeClass.Percentage:
                    //        curP.AxY.ViewMaxMsr = curP.PercentToValue(xD);
                    //        curP.AxY.ViewMaxPrc = xD;
                    //        break;
                    //}
                    //TextBoxMLeave(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private void AxResizeBottom(object sender, MouseEventArgs e)
		{
		    try
		    {
                if (_isMouseDown4Expand)
                {
                    var holder = (PictureBox) sender;
                    var ax = holder.Parent;
                    GraphicParam curP = GetParam(int.Parse(ax.Name));
                    if (holder.Top + e.Y > curP.AxY.TBoxMax.Bottom + 5)
                    {
                        if (holder.Top + e.Y <= curP.AxY.TBoxMin.Top - 3)
                        {
                            holder.Top = holder.Top + e.Y;
                            foreach (var t in curP.AxY.AxesOverlay)
                            {
                                GraphicParam tparam = GetParam(t);
                                //tparam.Area.Position.Height = (float)((tparam.AxY.Ax.Size.Height - _axSummand - 5
                                //                                        + tparam.AxY.TBoxMax.Bottom - holder.Top) * 100 / .976 / chart1.Height);
                                tparam.Area.Position.Height = (float) ((holder.Top + tparam.AxY.TBoxMin.Height - 2
                                                                       )*100/.976/chart1.Height);
                                //tparam.Area.Position.Y = 100f * (tparam.AxY.Ax.Top + holder.Top - tparam.AxY.TBoxMax.Height + 2) / chart1.Height;
                            }
                        }
                        else
                        {
                            holder.Top = curP.AxY.TBoxMin.Top - 3;
                            foreach (var t in curP.AxY.AxesOverlay)
                            {
                                GraphicParam tparam = GetParam(t);
                                tparam.Area.Position.Height = (float) ((holder.Top + tparam.AxY.TBoxMin.Height - 2
                                                                       )*100/.976/chart1.Height);
                                //tparam.Area.Position.Y = 100f * (tparam.AxY.Ax.Top + holder.Top - tparam.AxY.TBoxMax.Height + 2) / chart1.Height;
                            }
                        }
                    }
                    else
                    {
                        holder.Top = curP.AxY.TBoxMax.Bottom + 5;
                        foreach (var t in curP.AxY.AxesOverlay)
                        {
                            GraphicParam tparam = GetParam(t);
                            tparam.Area.Position.Height = (float) ((holder.Top + tparam.AxY.TBoxMin.Height - 2
                                                                   )*100/.976/chart1.Height);
                            //tparam.Area.Position.Y = 100f * (tparam.AxY.Ax.Top + holder.Top - tparam.AxY.TBoxMax.Height + 2) /
                            //                         chart1.Height;
                        }
                    }

                    double xD = -(curP.AxY.TBoxMin.Top - 3.0)/(holder.Top)*(_expandBufMax - _expandBufMin) +
                                _expandBufMax;
                    //curP.AxY.TBoxMin.Text = xD.ToString();
                    //switch (curP.PercentMode)
                    //{
                    //    case PercentModeClass.Absolute:

                    int tpos = (int) Math.Log10(curP.AxY.ViewMaxMsr - xD) - 2;
                    curP.AxY.ViewMinMsr = ((int) (xD/Math.Pow(10, tpos)))*Math.Pow(10, tpos); //xD;
                    //curP.AxY.ViewMinPrc = curP.ValueToPercent(xD);
                    int tposPrc =
                        (int)
                        Math.Log10(curP.ValueToPercent(curP.AxY.ViewMaxMsr) - curP.ValueToPercent(curP.AxY.ViewMinMsr)) -
                        2;
                    curP.AxY.ViewMinPrc = ((int) (curP.ValueToPercent(xD)/Math.Pow(10, tposPrc)))*Math.Pow(10, tposPrc);
                    //        break;
                    //    case PercentModeClass.Percentage:
                    //        curP.AxY.ViewMinMsr = curP.PercentToValue(xD);
                    //        curP.AxY.ViewMinPrc = xD;
                    //        break;
                    //}
                    //TextBoxMLeave(null, null);
                }
		    }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private void AxResize(object sender, EventArgs e)
		{
            try
            {
                var ax = ((Panel) sender);
                GraphicParam curP = GetParam(int.Parse(ax.Name));
                for (int j = 0; j < 4; j++)
                {
                    curP.AxY.La[j].Top = (j + 1)*(ax.Height - curP.AxY.AxCap.Height - curP.AxY.RulePanel.Height - 2)/5 -
                                         10 + curP.AxY.RulePanel.Height + 2;
                }
                curP.AxY.ResizeAreaButtonTop.Top = curP.AxY.TBoxMax.Bottom;
                curP.AxY.ResizeAreaButtonBottom.Top = curP.AxY.TBoxMin.Top - 3;
                //curP.TBoxMax.Top = curP.ResizeButtonTop.Bottom;
                //AreaAnRankingHeight(curP);
                if (!_isMouseDown4Expand)
                    foreach (var t in GetParam(int.Parse(ax.Name)).AxY.AxesOverlay)
                    {
                        AreaAnRankingHeight(GetParam(t));
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}
        
        //Перемещение вертикальной оси левой кнопкой мыши и масштабирование графика правой
		private void AxMBMove(object sender, MouseEventArgs e)
		{
		    try
		    {
                if (_isMouseDown4Move)
                {
                    Control ax;
                    int ad;
                    if (sender.GetType().ToString() == "System.Windows.Forms.Label")
                    {
                        ax = ((Label)sender).Parent;
                        ad = ((Label)sender).Top;
                    }
                    else
                    {
                        ax = (Panel)sender;
                        ad = 0;
                    }

                    if ((ax.Top + e.Y) - _mouseY > 0)
                    {
                        if ((ax.Bottom + e.Y) - _mouseY <= ParamsAnalog.First().AxY.Ax.Bottom)
                        {
                            ax.Top = (ax.Top + e.Y) - _mouseY;
                        }
                        else ax.Top = ParamsAnalog.First().AxY.Ax.Bottom - ax.Height;
                    }
                    else ax.Top = 0;
                    GraphicParam curP = GetParam(int.Parse(ax.Name));
                    foreach (var t in curP.AxY.AxesOverlay)
                    {
                        AreaAnRankingY(GetParam(t));
                    }

                    double xD = ((double)(e.X + ax.Left) / AxWidth + 1);
                    if (xD <= Noas + 1)
                    {
                        int current = 1;
                        //for (int i = 1; i < xD; current++)
                        //{
                        //    if (ParamsAnalog[current].AxY.UpperParam.Index == ParamsAnalog[current].Index
                        //    && ParamsAnalog[current].AxY.Ax.Visible && !ParamsAnalog[current].AxY.IsHidden)
                        //    {
                        //        i++;
                        //    }
                        //}
                        //current--;
                        //current = ParamsAnalog[current].AxY.UpperParam.Index;
                        foreach (var axY in AxesY)
                        {
                            if (axY.Ax.Left / AxWidth + 1 < xD && axY.Ax.Right / AxWidth + 1 >= xD && axY.Ax.Visible)
                            {
                                current = axY.UpperParam.Index;
                                break;
                            }
                        }
                        if (CurrentParamNumber != current &&
                            e.Y + curP.AxY.Ax.Top + ad <= GetParam(current).AxY.Ax.Bottom &&
                            e.Y + curP.AxY.Ax.Top + ad >= GetParam(current).AxY.Ax.Top)
                            Cursor = curP.Index == current ? Cursors.SizeAll : Cursors.Cross;
                        else Cursor = Cursors.SizeAll;
                    }
                    else Cursor = Cursors.SizeAll;
                }
                else
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        Control ax;
                        double eY = e.Y;
                        if (sender.GetType().ToString() == "System.Windows.Forms.Label")
                        {
                            ax = ((Label) sender).Parent;
                            eY += ((Label) sender).Top;
                        }
                        else
                        {
                            ax = (Panel) sender;
                        }
                        GraphicParam curP = GetParam(int.Parse(ax.Name));
                        foreach (var t in curP.AxY.AxesOverlay)
                        {
                            GraphicParam tG = GetParam(t);
                            _expandBufMax = curP.AxY.ViewMaxMsr; //double.Parse(curP.AxY.TBoxMax.Text);
                            _expandBufMin = curP.AxY.ViewMinMsr; //double.Parse(curP.AxY.TBoxMin.Text);
                            double xD = _expandBufMax -
                                        (-curP.AxY.TBoxMax.Top + eY)/
                                        (-curP.AxY.TBoxMax.Top + (double) curP.AxY.TBoxMin.Bottom)
                                        *(_expandBufMax - _expandBufMin);
                            tG.Area.CursorY.SelectionEnd =
                                Math.Abs(xD - tG.Area.CursorY.SelectionStart) > 0.05*tG.Area.AxisY.ScaleView.Size
                                    ? xD
                                    : tG.Area.CursorY.SelectionStart;
                        }
                    }
                }
		    }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//запасное событие
		private void SpareMethod(object sender, MouseEventArgs e)
		{
		    //Text = chart1.ChartAreas.First().AxisY.ScaleView.ViewMaximum + " " + chart1.ChartAreas.First().AxisY.Maximum;
		}

		//Кнопка скрытия текущего графика, а также оси Y
        ////Значение. Управление. Скрыть график
		private void Button10Click(object sender, EventArgs e)
		{
		    try
		    {
                if (CurrentParamNumber > 0) HideGraph(dataGridView1, new DataGridViewCellEventArgs(1, CurrentParamNumber - 1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private void HideGrClick(object sender, EventArgs e)
		{
            try
            {
                Control ax = ((Button) sender).Parent.Parent;
                GraphicParam curP = GetParam(int.Parse(ax.Name));
                foreach (var u in curP.AxY.AxesOverlay)
                {
                    int index = 0;
                    for (;
                        index < dataGridView1.Rows.Count &&
                        dataGridView1.Rows[index].Cells[1].Value.ToString() != (u).ToString();
                        index++)
                    {}
                    HideGraph(dataGridView1, new DataGridViewCellEventArgs(1, index));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		private void HideAxClick(object sender, EventArgs e)
		{
            try
            {
                Control ax = ((Button) sender).Parent.Parent;
                GraphicParam curP = GetParam(int.Parse(ax.Name));

                if (curP.AxY.IsHidden)
                {
                    if (curP.IsVisible)
                    {
                        ax.Visible = true;
                        Noas++;
                    }
                    curP.AxY.IsHidden = false;
                    foreach (var u in curP.AxY.AxesOverlay)
                    {
                        if (u != GetParam(CurrentParamNumber).Index)
                        {
                            _needToCheckChangeDatagrid = false;
                            WantedRow(u).Cells[0].Value = true;
                        }

                        GetParam(u).Area.AxisY.MajorGrid.Enabled = true;
                    }
                }
                else
                {
                    if (curP.IsVisible)
                    {
                        ax.Visible = false;
                        Noas--;
                    }
                    curP.AxY.IsHidden = true;
                    foreach (var u in curP.AxY.AxesOverlay)
                    {
                        if (u != GetParam(CurrentParamNumber).Index)
                        {
                            _needToCheckChangeDatagrid = false;
                            WantedRow(u).Cells[0].Value = false;
                        }

                        GetParam(u).Area.AxisY.MajorGrid.Enabled = false;
                    }
                }

                bool visibleDiscrExists = false;
                foreach (var u in ParamsDiscrete)
                {
                    AreaRankingX(u);
                    if (u.Series.Color != Color.Transparent) visibleDiscrExists = true;
                }

                bool visibleAxExists = AxesY.Any(axY => !axY.IsHidden);

                if (visibleAxExists)
                {
                    int nmbrOfVsblAnAxes = 0;
                    foreach (var u in ParamsAnalog)
                    {
                        if (u.AxY.Ax.Visible && !u.AxY.IsHidden && u.AxY.UpperParam != null &&
                            u.AxY.UpperParam.Index == u.Index)
                        {
                            nmbrOfVsblAnAxes++;
                            u.AxY.Ax.Left = (nmbrOfVsblAnAxes - 1)*AxWidth;
                        }
                        AreaRankingX(u);
                    }
                }
                else if (!visibleDiscrExists) //ParamsDiscrete.Count == 1)
                    foreach (var u in ParamsAnalog)
                    {
                        u.Area.Position.Width = 100;
                        u.Area.Position.X = 0;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Кнопка удаления текущего графика
        //Значение. Управление. Удалить график
		private void Button11Click(object sender, EventArgs e)
		{
            try
            {
                DialogResult yResult = MessageBox.Show("Удалить график №" + CurrentParamNumber + " ?",
                                                       "Запрос на удаление", MessageBoxButtons.OKCancel,
                                                       MessageBoxIcon.Question);
                if (CurrentParamNumber > 0 && yResult == DialogResult.OK)
                    DeleteParam(GetParam(CurrentParamNumber).Code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Блокировка отображения формы, если не задан интервал времени
		private void FormAppear(object sender, EventArgs e)
		{
			if (TimeBegin.Year < 1900 || TimeEnd.Year < 1900)
			{
				Dispose();
				MessageBox.Show("Не задан интервал отображения", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		//Полное/частичное отображение
        //Время. Отображаемый период. |<-->|
		private void Button17Click(object sender, EventArgs e)
		{
			radioButton4.Checked = true;
		}

        //Время. Отображаемый период. ->|<-
		private void Button16Click(object sender, EventArgs e)
		{
			//radioButton5.Checked = true;
			comboBox5.Text = "2";
			comboBox2.Text = "Сек.";
			Button8Click(null, null);
		}

		//Расположение осей Y наложением/каскадом
        //Значение. Взаимное положение. Наложение
		private void Button18Click(object sender, EventArgs e)
		{
			foreach (var t in ParamsAnalog)
			{
				t.AxY.Ax.Top = ParamsAnalog.First().AxY.Ax.Top;
				t.AxY.Ax.Height = ParamsAnalog.First().AxY.Ax.Height;
				t.CurAxPos = (double)(t.AxY.Ax.Top) / (ParamsAnalog.First().AxY.Ax.Height - 2 * AxSummand);
				t.CurAxSize = (double)(t.AxY.Ax.Height - 2 * AxSummand) / (ParamsAnalog.First().AxY.Ax.Height - 2 * AxSummand);
				AreaAnRankingY(t);
			}
		}

        //Значение. Взаимное положение. Каскадом
		private void Button19Click(object sender, EventArgs e)
		{
			GraphicParam firstAParam = ParamsAnalog.First();
			int visAreas =
				ParamsAnalog.Count(
					graphicParam => graphicParam.IsVisible && graphicParam.Index == graphicParam.AxY.UpperParam.Index);
			if (firstAParam.AxY.Ax.MinimumSize.Height * visAreas < firstAParam.AxY.Ax.Height)
			{
				int current = 0;
				int invisibleImportantAxes = 0;
				for (int i = 1; current < visAreas; i++)
				{
					//if (ParamsAnalog[i].AxY.UpperParam.Index == ParamsAnalog[i].Index 
					//    //&& !ParamsAnalog[i].AxY.IsHidden 
					//    && ParamsAnalog[i].IsVisible)
					//{
					//    ParamsAnalog[i].AxY.Ax.Top = current * (firstAParam.AxY.Ax.Height - 2*_axSummand) / visAreas;
					//    ParamsAnalog[i].AxY.Ax.Height = (firstAParam.AxY.Ax.Height - 2 * _axSummand) / visAreas + 2 * _axSummand;

					//    ParamsAnalog[i].CurAxPos = (double)(ParamsAnalog[i].AxY.Ax.Top) / (ParamsAnalog.First().AxY.Ax.Height - 2 * _axSummand);
					//    ParamsAnalog[i].CurAxSize = (double)(ParamsAnalog[i].AxY.Ax.Height - 2 * _axSummand) / (ParamsAnalog.First().AxY.Ax.Height - 2 * _axSummand);
					//    AreaAnRankingY(ParamsAnalog[i]);
					//    current++;
					//}
					if (/*!AxesY[i].IsHidden &&*/ AxesY[i].Ax.Visible || AxesY[i].UpperParam.IsVisible)
					{
						AxesY[i].Ax.Top = current*(firstAParam.AxY.Ax.Height - 2*AxSummand)/visAreas;
						AxesY[i].Ax.Height = (firstAParam.AxY.Ax.Height - 2 * AxSummand) / visAreas + 2 * AxSummand;
						AxesY[i].Ax.Left = (current - invisibleImportantAxes) * AxWidth;
						invisibleImportantAxes += !AxesY[i].Ax.Visible ? 1 : 0;
						foreach (var t in AxesY[i].AxesOverlay)
						{
							GetParam(t).CurAxPos = (double) (AxesY[i].Ax.Top)/
												   (ParamsAnalog.First().AxY.Ax.Height - 2*AxSummand);
							GetParam(t).CurAxSize = (double) (AxesY[i].Ax.Height - 2*AxSummand)/
													(ParamsAnalog.First().AxY.Ax.Height - 2*AxSummand);
							AreaAnRankingY(GetParam(t));
						}
						current++;
					}
				}
			}
			else
			{
				int current = 0;
				int invisibleImportantAxes = 0;
				for (int i = 1; current < visAreas; i++)
				{
					//if (ParamsAnalog[i].AxY.UpperParam.Index == ParamsAnalog[i].Index 
					//    //&& !ParamsAnalog[i].AxY.IsHidden
					//    )
					//{
					//    ParamsAnalog[i].AxY.Ax.Top = current*(firstAParam.AxY.Ax.Height -
					//                                      firstAParam.AxY.Ax.MinimumSize.Height) / (visAreas - 1);
					//    ParamsAnalog[i].AxY.Ax.Height = firstAParam.AxY.Ax.Height / visAreas;
					//    //for (int j = 0; j < 3; j++)
					//    //{
					//    //    ParamsAnalog[i].la[j].Top = (j + 1)*ParamsAnalog[i].Ax.Height/4 - 10;
					//    //}

					//    ParamsAnalog[i].CurAxPos = (double)(ParamsAnalog[i].AxY.Ax.Top) / (ParamsAnalog.First().AxY.Ax.Height - 2 * _axSummand);
					//    ParamsAnalog[i].CurAxSize = (double)(ParamsAnalog[i].AxY.Ax.Height - 2 * _axSummand) / (ParamsAnalog.First().AxY.Ax.Height - 2 * _axSummand);
					//    AreaAnRankingY(ParamsAnalog[i]);
					//    current++;
					//}
					if (/*!AxesY[i].IsHidden &&*/ AxesY[i].Ax.Visible || AxesY[i].UpperParam.IsVisible)
					{
						AxesY[i].Ax.Height = (firstAParam.AxY.Ax.Height - 2 * AxSummand) / visAreas + 2 * AxSummand;
						//AxesY[i].Ax.Top = current * (firstAParam.AxY.Ax.Height - firstAParam.AxY.Ax.MinimumSize.Height + _axSummand) / (visAreas - 1);
						//AxesY[i].Ax.Top = current * (AxesY[i].Ax.Height - 2 * _axSummand);
						AxesY[i].Ax.Top = current * (firstAParam.AxY.Ax.Height - AxesY[i].Ax.Height) / (visAreas - 1);
						AxesY[i].Ax.Left = (current - invisibleImportantAxes) * AxWidth;
						invisibleImportantAxes += !AxesY[i].Ax.Visible ? 1 : 0;
						foreach (var t in AxesY[i].AxesOverlay)
						{
							GetParam(t).CurAxPos = (double)(AxesY[i].Ax.Top) /
												   (ParamsAnalog.First().AxY.Ax.Height - 2 * AxSummand);
							GetParam(t).CurAxSize = (double)(AxesY[i].Ax.Height - 2 * AxSummand) /
													(ParamsAnalog.First().AxY.Ax.Height - 2 * AxSummand);
							AreaAnRankingY(GetParam(t));
						}
						current++;
					}
				}
			}
		}

		//Эффект залипания кнопок
		private int _starter;
		private Button _holdedButton;

		//?
        private void ButtonHold(object sender, EventArgs e)
		{
			_holdedButton = (Button) sender;
			timer2.Enabled = true;
            timer2.Interval = 130;
			timer2.Start();
			_starter = 0;
		}

		//?
        private void ButtonRelease(object sender, EventArgs e)
		{
			chart1.Refresh();
			timer2.Stop();
			_holdedButton = null;
		}

        //?
		private void Timer2Tick(object sender, EventArgs e)
		{
			if (_starter >= 3)
				switch (_holdedButton.Name)
				{
					case "button14":
						WheelZooming(_holdedButton, e);
						break;
					case "button15":
						WheelZooming(_holdedButton, e);
						break;
					case "button1":
						ButtonTimeWalkAhead(_holdedButton, e);
						break;
					case "button12":
						ButtonTimeStepBack(_holdedButton, e);
						break;
					case "button13":
						ButtonTimeStepAhead(_holdedButton, e);
						break;
					case "button5":
						ButtonTimeWalkBack(_holdedButton, e);
						break;
				}
			_starter++;
		}

		//Кнопки "масштаб по шкале" и "автомасштаб" всех графиков (интервала отображения по Y)
        ////Значение. Все графики. Авто масштаб
		private void Button7Click(object sender, EventArgs e)
		{
            try
            {
                if (_chartTypeMode != GraphicTypeMode.Discrete && _chartTypeMode != GraphicTypeMode.Empty)
                {
                    //Было foreach, включая нулевую невидимую арию
                    for (int index = 1; index < ParamsAnalog.Count; index++)
                    {
                        var t = ParamsAnalog[index];
                        if (t.Dots.Count != 0 && !t.AxY.IsOverlayed)
                        {
                            double scaleYMin = t.MinVal();
                            double scaleYMax = t.MaxVal();
                            if (scaleYMax == scaleYMin)
                            {
                                scaleYMax += 1;
                                scaleYMin -= 1;
                            }
                            t.Area.AxisY.ScaleView.Position = scaleYMin*1.03 - scaleYMax*0.03;
                            t.Area.AxisY.ScaleView.Size = scaleYMax*1.06 - scaleYMin*1.06;

                            if (t.AxY.UpperParam.Index == t.Index)
                                //switch (t.PercentMode)
                            {
                                //    case PercentModeClass.Absolute:
                                //        //t.AxY.TBoxMin.Text = t.Area.AxisY.ScaleView.Position.ToString();
                                //        //t.AxY.TBoxMax.Text =
                                //        //    (t.Area.AxisY.ScaleView.Position + t.Area.AxisY.ScaleView.Size).ToString();
                                int tpos = (int) Math.Log10(scaleYMax - scaleYMin) - 2;
                                double tmin = (int) ((t.Area.AxisY.ScaleView.Position/Math.Pow(10, tpos)))*
                                              Math.Pow(10, tpos);
                                double tmax =
                                    (int)
                                    ((t.Area.AxisY.ScaleView.Position + t.Area.AxisY.ScaleView.Size)/Math.Pow(10, tpos))*
                                    Math.Pow(10, tpos);
                                //        t.AxY.TBoxMin.Text = tmin.ToString();
                                //        t.AxY.TBoxMax.Text = tmax.ToString();
                                t.AxY.ViewMaxMsr = tmax;
                                t.AxY.ViewMinMsr = tmin;

                                int tposPrc =
                                    (int)
                                    Math.Log10(t.ValueToPercent(t.AxY.ViewMaxMsr) - t.ValueToPercent(t.AxY.ViewMinMsr)) -
                                    2;
                                t.AxY.ViewMaxPrc = ((int) (t.ValueToPercent(tmax)/Math.Pow(10, tposPrc)))*
                                                   Math.Pow(10, tposPrc);
                                t.AxY.ViewMinPrc = ((int) (t.ValueToPercent(tmin)/Math.Pow(10, tposPrc)))*
                                                   Math.Pow(10, tposPrc);
                                //t.AxY.ViewMaxPrc = t.ValueToPercent(tmax);
                                //t.AxY.ViewMinPrc = t.ValueToPercent(tmin);
                            }
                            //        break;
                            //    case PercentModeClass.Percentage:
                            //        t.AxY.TBoxMin.Text = t.ValueToPercent(t.Area.AxisY.ScaleView.Position).ToString();
                            //        t.AxY.TBoxMax.Text =
                            //            t.ValueToPercent(t.Area.AxisY.ScaleView.Position + t.Area.AxisY.ScaleView.Size).ToString();
                            //        //tmin =  ((int)(t.ValueToPercent(t.Area.AxisY.ScaleView.Position) / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
                            //        //tmax =
                            //        //    ((int)
                            //        //     (t.ValueToPercent((t.Area.AxisY.ScaleView.Position + t.Area.AxisY.ScaleView.Size))/Math.Pow(10, tpos)))*
                            //        //    Math.Pow(10, tpos);
                            //        //t.AxY.TBoxMin.Text = tmin.ToString();
                            //        //t.AxY.TBoxMax.Text = tmax.ToString();
                            //        t.AxY.ViewMaxMsr = t.Area.AxisY.ScaleView.Position + t.Area.AxisY.ScaleView.Size;
                            //        t.AxY.ViewMinMsr = t.Area.AxisY.ScaleView.Position;
                            //        t.AxY.ViewMaxPrc = t.ValueToPercent(t.Area.AxisY.ScaleView.Position + t.Area.AxisY.ScaleView.Size);
                            //        t.AxY.ViewMinPrc = t.ValueToPercent(t.Area.AxisY.ScaleView.Position);
                            //        break;
                            //}
                            t.Area.AxisY.MajorGrid.Interval = (t.Area.AxisY.ScaleView.ViewMaximum -
                                                               t.Area.AxisY.ScaleView.ViewMinimum)/5;

                            t.IsYScaledAuto = true;
                            //TextBoxMChange(t);
                        }
                    }
                    //button4.Enabled = false;
                    //textBox1.Enabled = false;
                    //textBox2.Enabled = false;
                    //IsYScaledAuto = true;

                    if (GetParam(CurrentParamNumber).DataTypeD != DataType.Boolean &&
                        !GetParam(CurrentParamNumber).AxY.IsOverlayed && TimerMode == TimerModes.Analyzer)
                    {
                        button23.UseVisualStyleBackColor = false;
                        button23.BackColor = SystemColors.Highlight;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

        //Значение. Все графики. Масштаб по шкале
		private void Button20Click(object sender, EventArgs e)
		{
            try
            {
                for (int index = 1; index < ParamsAnalog.Count; index++)
                {
                    var t = ParamsAnalog[index];
                    if (!t.AxY.IsOverlayed)
                    {
                        t.Area.AxisY.ScaleView.Position = t.Min;
                        t.Area.AxisY.ScaleView.Size = t.Max - t.Min;

                        if (t.AxY.UpperParam.Index == t.Index)
                            //switch (t.PercentMode)
                        {
                            //    case PercentModeClass.Absolute:
                            //        t.AxY.TBoxMin.Text = t.Min.ToString();
                            //        t.AxY.TBoxMax.Text = t.Max.ToString();
                            t.AxY.ViewMaxMsr = t.Max;
                            t.AxY.ViewMinMsr = t.Min;
                            t.AxY.ViewMaxPrc = 100;
                            t.AxY.ViewMinPrc = 0;
                        }
                        //            break;
                        //        case PercentModeClass.Percentage:
                        //            t.AxY.TBoxMin.Text = t.ValueToPercent(t.Min).ToString();
                        //            t.AxY.TBoxMax.Text = t.ValueToPercent(t.Max).ToString();
                        //            t.AxY.ViewMaxMsr = t.Max;
                        //            t.AxY.ViewMinMsr = t.Min;
                        //            t.AxY.ViewMaxPrc = t.ValueToPercent(t.Max);
                        //            t.AxY.ViewMinPrc = t.ValueToPercent(t.Min);
                        //            break;
                        //    }
                        t.Area.AxisY.MajorGrid.Interval = (t.Area.AxisY.ScaleView.ViewMaximum -
                                                           t.Area.AxisY.ScaleView.ViewMinimum)/5;
                        t.IsYScaledAuto = false;
                        TextBoxMChange(t);
                    }
                }

                GraphicParam curP = GetParam(CurrentParamNumber);

                if (curP.DataTypeD != DataType.Boolean)
                {
                    button4.Enabled = true;
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;

                    //button23.BackColor = Color.White;
                    button23.UseVisualStyleBackColor = true;
                }
                //IsYScaledAuto = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Кнопки сброса всех графиков в проценты/ед.изм.
        //Значение. Все графики. В проценты
		private void Button22Click(object sender, EventArgs e)
		{
            try
            {
                if (ParamsAnalog.First().Series.Points.Count != 0)
                {
                    foreach (var pa in ParamsAnalog)
                    {
                        if (pa.PercentMode == PercentModeClass.Absolute && !pa.AxY.IsOverlayed &&
                            pa.Series.Color != Color.Transparent)
                        {
                            pa.PercentMode = PercentModeClass.Percentage;
                            AxYToPercent(pa);
                        }
                    }
                    GraphicParam cP = GetParam(CurrentParamNumber);
                    if (cP.DataTypeD != DataType.Boolean && !cP.AxY.IsOverlayed)
                    {
                        textBox1.Text = cP.AxY.TBoxMin.Text;
                        textBox2.Text = cP.AxY.TBoxMax.Text;
                        button24.Text = "В единицы изм.";
                        label15.Text = "%";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

        //Значение. Все графики. В единицы изм.
        private void Button21Click(object sender, EventArgs e)
		{
            try
            {
                if (ParamsAnalog.First().Series.Points.Count != 0)
                {
                    foreach (var pa in ParamsAnalog)
                    {
                        if (pa.PercentMode == PercentModeClass.Percentage && !pa.AxY.IsOverlayed)
                        {
                            pa.PercentMode = PercentModeClass.Absolute;
                            AxYFromPercent(pa);
                        }
                    }
                    GraphicParam cP = GetParam(CurrentParamNumber);
                    if (cP.DataTypeD != DataType.Boolean && !cP.AxY.IsOverlayed)
                    {
                        textBox1.Text = cP.AxY.TBoxMin.Text;
                        textBox2.Text = cP.AxY.TBoxMax.Text;
                        button24.Text = "В проценты";
                        label15.Text = dataGridView1.CurrentRow.Cells["Ед. измер."].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Кнопки "масштаб по шкале" и "автомасштаб" текущего графика (интервала отображения по Y)
        //Значение. Текущий график. Масштаб. Авто масштаб
		private void Button23Click(object sender, EventArgs e)
		{
            try
            {
                if (CurrentParamNumber > 0)
                {
                    GraphicParam curParam = GetParam(CurrentParamNumber);
                    if (curParam.Dots.Count == 0) return;
                    if (curParam.DataTypeD != DataType.Boolean)
                    {
                        //IsYScaledAuto = true;
                        foreach (var n in curParam.AxY.AxesOverlay)
                        {
                            GetParam(n).IsYScaledAuto = false;
                        }
                        curParam.IsYScaledAuto = true;
                        if (TimerMode == TimerModes.Analyzer)
                        {
                            button23.UseVisualStyleBackColor = false;
                            button23.BackColor = SystemColors.Highlight;
                        }

                        double scaleYMin = curParam.MinVal();
                        double scaleYMax = curParam.MaxVal();
                        if (scaleYMax == scaleYMin)
                        {
                            scaleYMax += 1;
                            scaleYMin -= 1;
                        }
                        curParam.Area.AxisY.ScaleView.Position = scaleYMin*1.03 - scaleYMax*0.03;
                            //t.Area.AxisY.Minimum;
                        curParam.Area.AxisY.ScaleView.Size = scaleYMax*1.06 - scaleYMin*1.06;
                        //t.Area.AxisY.Maximum - t.Area.AxisY.Minimum;
                        double tmin, tmax; //временные переменные
                        int tpos = (int) Math.Log10(scaleYMax - scaleYMin) - 2;
                        //switch (curParam.PercentMode)
                        //{
                        //case PercentModeClass.Absolute:
                        //    //curParam.AxY.TBoxMin.Text = curParam.Area.AxisY.ScaleView.Position.ToString();
                        //    //curParam.AxY.TBoxMax.Text = (curParam.Area.AxisY.ScaleView.Position + curParam.Area.AxisY.ScaleView.Size).ToString();
                        tmin = (int) ((curParam.Area.AxisY.ScaleView.Position/Math.Pow(10, tpos)))*Math.Pow(10, tpos);
                        tmax =
                            (int)
                            ((curParam.Area.AxisY.ScaleView.Position + curParam.Area.AxisY.ScaleView.Size)/
                             Math.Pow(10, tpos))*
                            Math.Pow(10, tpos);
                        //curParam.AxY.TBoxMin.Text = tmin.ToString();
                        //curParam.AxY.TBoxMax.Text = tmax.ToString();

                        curParam.AxY.ViewMaxMsr = tmax;
                        curParam.AxY.ViewMinMsr = tmin;
                        int tposPrc =
                            (int)
                            Math.Log10(curParam.ValueToPercent(curParam.AxY.ViewMaxMsr) -
                                       curParam.ValueToPercent(curParam.AxY.ViewMinMsr)) - 2;
                        curParam.AxY.ViewMaxPrc = ((int) (curParam.ValueToPercent(tmax)/Math.Pow(10, tposPrc)))*
                                                  Math.Pow(10, tposPrc);
                        curParam.AxY.ViewMinPrc = ((int) (curParam.ValueToPercent(tmin)/Math.Pow(10, tposPrc)))*
                                                  Math.Pow(10, tposPrc);
                        //curParam.AxY.ViewMaxPrc = curParam.ValueToPercent(tmax);
                        //curParam.AxY.ViewMinPrc = curParam.ValueToPercent(tmin);
                        //        break;
                        //    case PercentModeClass.Percentage:
                        //        curParam.AxY.TBoxMin.Text = curParam.ValueToPercent(curParam.Area.AxisY.ScaleView.Position).ToString();
                        //        curParam.AxY.TBoxMax.Text =
                        //            curParam.ValueToPercent((curParam.Area.AxisY.ScaleView.Position + curParam.Area.AxisY.ScaleView.Size)).
                        //                ToString();
                        //        //tmin = ((int)(curParam.ValueToPercent(curParam.Area.AxisY.ScaleView.Position) / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
                        //        //    tmax =
                        //        //        ((int)
                        //        //         (curParam.ValueToPercent((curParam.Area.AxisY.ScaleView.Position + curParam.Area.AxisY.ScaleView.Size)) / Math.Pow(10, tpos))) *
                        //        //        Math.Pow(10, tpos);
                        //        //    curParam.AxY.TBoxMin.Text = tmin.ToString();
                        //        //    curParam.AxY.TBoxMax.Text = tmax.ToString();

                        //        curParam.AxY.ViewMaxMsr = curParam.Area.AxisY.ScaleView.Position + curParam.Area.AxisY.ScaleView.Size;
                        //        curParam.AxY.ViewMinMsr = curParam.Area.AxisY.ScaleView.Position;
                        //        curParam.AxY.ViewMaxPrc = curParam.ValueToPercent(curParam.Area.AxisY.ScaleView.Position + curParam.Area.AxisY.ScaleView.Size);
                        //        curParam.AxY.ViewMinPrc = curParam.ValueToPercent(curParam.Area.AxisY.ScaleView.Position);
                        //        break;
                        //}
                        curParam.Area.AxisY.MajorGrid.Interval = (curParam.Area.AxisY.ScaleView.ViewMaximum -
                                                                  curParam.Area.AxisY.ScaleView.ViewMinimum)/5;

                        button4.Enabled = true;
                        textBox1.Enabled = true;
                        textBox2.Enabled = true;
                        //TextBoxMChange(curParam);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

        //Значение. Текущий график. Масштаб. Масштаб по шкале
        private void Button9Click(object sender, EventArgs e)
		{
            try
            {
                if (CurrentParamNumber > 0)
                {
                    GraphicParam curParam = GetParam(CurrentParamNumber);
                    if (curParam.DataTypeD != DataType.Boolean)
                    {
                        curParam.Area.AxisY.ScaleView.Position = curParam.Min;
                        curParam.Area.AxisY.ScaleView.Size = curParam.Max - curParam.Min;
                        //switch (curParam.PercentMode)
                        //{
                        //    case PercentModeClass.Absolute:
                        //        curParam.AxY.TBoxMin.Text = curParam.Min.ToString();
                        //        curParam.AxY.TBoxMax.Text = curParam.Max.ToString();

                        curParam.AxY.ViewMaxMsr = curParam.Max;
                        curParam.AxY.ViewMinMsr = curParam.Min;
                        curParam.AxY.ViewMaxPrc = curParam.ValueToPercent(curParam.Max);
                        curParam.AxY.ViewMinPrc = curParam.ValueToPercent(curParam.Min);
                        //        break;
                        //    case PercentModeClass.Percentage:
                        //        curParam.AxY.TBoxMin.Text = curParam.ValueToPercent(curParam.Min).ToString();
                        //        curParam.AxY.TBoxMax.Text = curParam.ValueToPercent(curParam.Max).ToString();

                        //        curParam.AxY.ViewMaxMsr = curParam.Max;
                        //        curParam.AxY.ViewMinMsr = curParam.Min;
                        //        curParam.AxY.ViewMaxPrc = curParam.ValueToPercent(curParam.Max);
                        //        curParam.AxY.ViewMinPrc = curParam.ValueToPercent(curParam.Min);
                        //        break;
                        //}
                        curParam.Area.AxisY.MajorGrid.Interval = (curParam.Area.AxisY.ScaleView.ViewMaximum -
                                                                  curParam.Area.AxisY.ScaleView.ViewMinimum)/5;
                        button4.Enabled = true;
                        textBox1.Enabled = true;
                        textBox2.Enabled = true;

                        TextBoxMChange(curParam);
                        //IsYScaledAuto = false;
                        foreach (var n in curParam.AxY.AxesOverlay)
                        {
                            GetParam(n).IsYScaledAuto = false;
                        }
                    }
                    button23.UseVisualStyleBackColor = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Кнопка переключения текущего режима в проценты и обратно
        //Значение. Текущий график. Масштаб. В проценты
		private void Button24Click(object sender, EventArgs e)
		{
            try
            {
                if (CurrentParamNumber > 0)
                {
                    GraphicParam curParam = GetParam(CurrentParamNumber);
                    if (curParam.DataTypeD != DataType.Boolean)
                    {
                        if (button24.Text != "В проценты" && curParam.PercentMode != PercentModeClass.Absolute)
                        {
                            button24.Text = "В проценты";
                            foreach (var n in curParam.AxY.AxesOverlay)
                            {
                                GetParam(n).PercentMode = PercentModeClass.Absolute;
                            }
                            AxYFromPercent(curParam);
                            label15.Text = dataGridView1.CurrentRow.Cells["Ед. измер."].Value.ToString();
                        }
                        else
                        {
                            button24.Text = "В единицы изм.";
                            foreach (var n in curParam.AxY.AxesOverlay)
                            {
                                GetParam(n).PercentMode = PercentModeClass.Percentage;
                            }
                            AxYToPercent(curParam);
                            label15.Text = "%";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Кнопка "Применить интервал отображения"
        //Время. Отображаемый интервал. Применить
		private void Button8Click(object sender, EventArgs e)
		{
            try
            {
                if (radioButton5.Checked)
                {
                    //double xD = double.Parse(comboBox5.Text);
                    if ((comboBox5.Text == "") ||
                        (comboBox2.Text == "Сек." && double.Parse(comboBox5.Text) <= ScaleXMin * 60 * 60 * 24))
                        comboBox5.Text = (ScaleXMin*60*60*24).ToString();
                    //if (comboBox6.Text == "") comboBox6.Text = @"1";
                    //TimeFromTextbox();
                    //if (xD > .3)
                    //{
                    SetScalePosition1(comboBox5, null);
                    //}

                    //if (chart1.ChartAreas.Count != 0)
                    //{
                    //    switch (comboBox2.Text)
                    //    {
                    //        case "Сек.":
                    //            //_scaleDrawFactor2 = 1;
                    //            //if (xD > .5) SetScalePosition1(comboBox5, null);
                    //            //else comboBox5.Text = "0,5";
                    //            break;
                    //        case "Мин.":
                    //            //_scaleDrawFactor2 = 60;
                    //            //if (xD > .3) SetScalePosition1(comboBox5, null);
                    //            //else
                    //            //{
                    //            //    comboBox2.Text = "Сек.";
                    //            //    comboBox5.Text = (.3*60).ToString();
                    //            //}
                    //            break;
                    //        case "Час.":
                    //            //_scaleDrawFactor2 = 3600;
                    //            //if (xD > .3) SetScalePosition1(comboBox5, null);
                    //            //else
                    //            //{
                    //            //    comboBox2.Text = "Мин.";
                    //            //    comboBox5.Text = (.3 * 60).ToString();
                    //            //}
                    //            break;
                    //        case "Сут.":
                    //            //_scaleDrawFactor2 = 3600*24;
                    //            //if (xD > .3) SetScalePosition1(comboBox5, null);
                    //            //else
                    //            //{
                    //            //    comboBox2.Text = "Час.";
                    //            //    comboBox5.Text = (.3 * 24).ToString();
                    //            //}
                    //            break;
                    //    }
                    //}
                }
                else
                {
                    radioButton5.Checked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Выбор толщины линии отрисовки графиков
		//private void ComboBoxLineWidthChange(object sender, EventArgs e)
		//{
		//    if (((ComboBox)sender).Name == "comboBox25")
		//    {
		//        foreach (var t in ParamsAnalog)
		//        {
		//            t.Series.BorderWidth = (int)(Math.Abs(double.Parse(((ComboBox)sender).Text)));
		//            t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
		//        }
		//        ((ComboBox)sender).Text = ParamsAnalog[0].Series.BorderWidth.ToString();
		//    }
		//    else
		//    {
		//        foreach (var t in ParamsDiscrete)
		//        {
		//            t.Series.BorderWidth = (int)(Math.Abs(double.Parse(((ComboBox)sender).Text)));
		//            t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
		//        }
		//        ((ComboBox)sender).Text = ParamsDiscrete[0].Series.BorderWidth.ToString();
		//    }
		//}

        //Печать.Толщина линии
        private void ComboBox25KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter) Button25Click(sender, e);
		}

        //Печать.Толщина линии. Текущий график
        private void Button25Click(object sender, EventArgs e)
		{
			//if (checkBox25.Checked)
			//foreach (var t in ParamsAnalog)
			//{
			//    t.Series.BorderWidth = (int)(Math.Abs(double.Parse(comboBox25.Text)));
			//    t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
			//    checkBox25.Checked = false;
			//}
			//else
			//{
				GraphicParam t = GetParam(CurrentParamNumber);
                //if (t.DataTypeD != DataType.Boolean)
                //{
					t.Series.BorderWidth = (int) (Math.Abs(double.Parse(comboBox25.Text)));
					t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
                //}
			//}
		}

		//Печать.Толщина линии. Все графики
        private void Button26Click(object sender, EventArgs e)
		{
			//if (checkBox26.Checked)
				foreach (var t in ParamsAnalog)
				{
					t.Series.BorderWidth = (int)(Math.Abs(double.Parse(comboBox25.Text)));
					t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
				}
                foreach (var t in ParamsDiscrete)
                {
                    t.Series.BorderWidth = (int)(Math.Abs(double.Parse(comboBox25.Text)));
                    t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
                }
			//else
			//{
			//    GraphicParam t = GetParam(CurrentParamNumber);
			//    if (t.DataTypeD == DataType.Boolean)
			//    {
			//        t.Series.BorderWidth = (int) (Math.Abs(double.Parse(comboBox26.Text)));
			//        t.Series.EmptyPointStyle.BorderWidth = t.Series.BorderWidth;
			//    }
			//}
		}

		//Всплывающая подсказка над кнопками
		private void ButtonsHover(object sender, EventArgs e)
		{
			string tempS = "";
			switch (((Button)sender).Name)
			{
				case "button26":
				case "button25":
				case "button8":
				case "button4":
					tempS = "Применить";
					break;
				case "button17":
					tempS = "Максимальный период отображения";
					break;
				case "button16":
					tempS = "Минимальный период отображения";
					break;
				case "button14":
					tempS = "Уменьшить период отображения";
					break;
				case "button15":
					tempS = "Увеличить период отображения";
					break;
				case "button12":
					tempS = "Установить визир в предыдущую точку по всем графикам";
					break;
				case "button5":
					tempS = "Установить визир в предыдущую точку по текущему графику";
					break;
				case "button1":
					tempS = "Установить визир в следующую точку по текущему графику";
					break;
				case "button13":
					tempS = "Установить визир в следующую точку по всем графикам";
					break;
			}
			toolTip1.SetToolTip((Button)sender, tempS);
		}
        
		//Делает данный график верхним
		private void ToTopOverlay(GraphicParam curParam)
		{
			try
			{
				if (curParam.IsVisible)
				{
					curParam.AxY.UpperParam = curParam;
					foreach (var t in curParam.AxY.AxesOverlay)
					{
						//dataGridView1[0, t - 1].Style.BackColor = curParam.Series.Color;
						//dataGridView1[0, t - 1].Style.SelectionBackColor = curParam.Series.Color;
						WantedRow(t).Cells[0].Style.BackColor = curParam.Series.Color;
						WantedRow(t).Cells[0].Style.SelectionBackColor = curParam.Series.Color;
					}
				}
			}
			catch (Exception rew)
			{
				MessageBox.Show(rew.StackTrace);
			}
		}

		//Выбор верхнего графика в слитых вместе осях Y
		private void OverlayTopChoose(object sender, MouseEventArgs e)
		{
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    var l1 = (Label) sender;
                    int curNmbr = int.Parse(l1.Parent.Name);
                    contextMenuStrip1.Items.Clear();
                    int j = 0;
                    GetParam(curNmbr).AxY.AxesOverlay.Sort();
                    foreach (var cntr in GetParam(curNmbr).AxY.AxesOverlay)
                    {
                        contextMenuStrip1.Items.Add(cntr.ToString());
                        contextMenuStrip1.Items[j].MouseDown += contextMenuStrip1MouseDown;
                        contextMenuStrip1.Items[j].ForeColor = GetParam(cntr).Series.Color;
                        contextMenuStrip1.Items[j].Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold,
                                                                   GraphicsUnit.Point, ((204)));
                        int cntr1 = cntr;
                        contextMenuStrip1.Items[j].Click += (s, ev) =>
                                                                {
                                                                    ToTopOverlay(GetParam(cntr1));
                                                                    //dataGridView1.CurrentCell = dataGridView1[2, cntr1-1];
                                                                    dataGridView1.CurrentCell =
                                                                        WantedRow(cntr1).Cells[2];
                                                                    DatagridSelectionChanged(null, null);
                                                                    _isMouseDown4Divide = false;
                                                                };
                        j++;
                    }
                    contextMenuStrip1.Show(l1, new Point(0, l1.Height));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
		}

		//Нахождение строки датагрида по номеру графика
		private DataGridViewRow WantedRow(int currentNum)
		{
			int index = 0;
			for (; index < dataGridView1.Rows.Count && dataGridView1.Rows[index].Cells[1].Value.ToString() != (currentNum).ToString(); index++)
			{ }
			return dataGridView1.Rows[index];
		}

        //установка DoubleBuffered для DataGridView
        private static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, control, new object[] { true });
        }

        //private void GraphicForm_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    timer1.Stop();
        //}

        //---
        private static double FáBil(DateTimeIntervalType bilGerðe, TimeSpan lengdTímabil, byte stærðBil = 1)
        {
            //возвращает количество интервалов (AxisX.IntervalCount)
            //в зависимости от типа интервала (bilGerðe), и отображаемого промежутка (lengdTímabil), 
            //а также частоты сетки (stærðBil): 0 - редкая (~ 6 делений), 1 - частая (~ 12 делений), (>1 = 1)

            //double[] dayInt = { 1/12, 1/6, 1/4, 1/3, 1/2, 1, 1.5 };
            //double[] dayInt = { 1/10, 1/5, 1/4, 1/2, 1, 1.5 };
            double a; //= 1;
            bool isSec = false;

            //switch (bilGerðe)
            //{
            //    case DateTimeIntervalType.Days:
            //        a = lengdTímabil.TotalDays;
            //        break;
            //    case DateTimeIntervalType.Hours:
            //        a = lengdTímabil.TotalHours;
            //        break;
            //    case DateTimeIntervalType.Minutes:
            //        a = lengdTímabil.TotalMinutes;
            //        break;
            //    case DateTimeIntervalType.Seconds:
            //        a = lengdTímabil.TotalSeconds;
            //        break;
            //}

            if (lengdTímabil.TotalDays > 2)
                a = lengdTímabil.TotalDays;
            else if (lengdTímabil.TotalHours > 2)
                a = lengdTímabil.TotalHours;
            else if (lengdTímabil.TotalMinutes > 2)
                a = lengdTímabil.TotalMinutes;
            else
            {
                a = lengdTímabil.TotalSeconds;
                isSec = true;
            }

            double b = a/((stærðBil == 0) ? 6 : 12);

            if (!isSec)
            {
                if (b >= 2)
                    b = Math.Truncate(b);
                else if (b >= 1.5)
                    b = 1.5;
                else if (b >= 1)
                    b = 1.0;
                else if (b >= .5)
                    b = .5;
                else if (b >= (1.0 / 3))
                    b = 1.0 / 3;
                else if (b >= .25)
                    b = .25;
                else if (b >= (1.0 / 6))
                    b = 1.0 / 6;
                else
                    b = 1.0/12;
            }
            else
            {
                if (b >= 2)
                    b = Math.Truncate(b);
                else if (b >= 1.5)
                    b = 1.5;
                else if (b >= 1)
                    b = 1.0;
                else if (b >= .5)
                    b = .5;
                else if (b >= .25)
                    b = .25;
                else if (b >= .2)
                    b = .2;
                else if (b >= .1)
                    b = 0.1;
                else
                {
                    int i = 1;
                    double c = b * 10;
                    while (c > 1) { i++; c *= 10; }
                    b = Math.Round(b, i);
                }
            }

            a = a/b;

            if (a >= ((stærðBil == 0) ? 7 : 13))
                if (!isSec)
                {
                    if (b >= 2)
                        b = b + 1;
                    else if (b >= .5)
                        b = b + .5;
                    else if (b >= (1.0 / 3))
                        b = .5;
                    else if (b >= .25)
                        b = 1.0 / 3;
                    else if (b >= (1.0 / 6))
                        b = .25;
                    else
                        b = 1.0 / 6;
                }
                else
                {
                    if (b >= 2)
                        b = b + 1;
                    else if (b >= .5)
                        b = b + .5;
                    else if (b >= .25)
                        b = .5;
                    else if (b >= .2)
                        b = .25;
                    else if (b >= .1)
                        b = .2;
                    else
                        b = b * 2;
                }

            switch (bilGerðe)
            {
                case DateTimeIntervalType.Days:
                    if (lengdTímabil.TotalDays <= 2)
                    {
                        b /= 24;
                        if (lengdTímabil.TotalHours <= 2)
                        {
                            b /= 60;
                            if (lengdTímabil.TotalMinutes <= 2) b /= 60;
                        }
                    }
                    break;
                case DateTimeIntervalType.Hours:
                    if (lengdTímabil.TotalDays > 2)
                        b *= 24;
                    else
                    {
                        if (lengdTímabil.TotalHours <= 2)
                        {
                            b /= 60;
                            if (lengdTímabil.TotalMinutes <= 2) b /= 60;
                        }
                    }
                    break;
                case DateTimeIntervalType.Minutes:
                    if (lengdTímabil.TotalHours > 2)
                    {
                        b *= 60;
                        if (lengdTímabil.TotalDays > 2) b *= 24;
                    }
                    else
                        if (lengdTímabil.TotalMinutes <= 2) b /= 60;
                    break;
                case DateTimeIntervalType.Seconds:
                    if (lengdTímabil.TotalMinutes > 2)
                    {
                        b *= 60;
                        if (lengdTímabil.TotalHours > 2)
                        {
                            b *= 60;
                            if (lengdTímabil.TotalDays > 2) b *= 24;
                        }

                    }

                    break;
            }

            return b;
        }
        
        private static double FáBil(double tímabil, string tímabilGerð, byte stærðBil = 1)
        {
            //возвращает количество интервалов (AxisX.IntervalCount)
            //в зависимости от типа интервала (bilGerðe), и отображаемого промежутка (tímabil), его типа (tímabilGerð)
            //а также частоты сетки (stærðBil)
            //см. выше
            //допустимые значения параметра tímabilGerð: Сек., Мин., Час., Сут.
            
            double temp = Convert.ToDouble(tímabil);
            temp *= Math.Pow(10, 7);
            DateTimeIntervalType it = DateTimeIntervalType.Seconds;

            switch (tímabilGerð)
            {
                case "Сек.":
                    break;
                case "Мин.":
                    temp *= 60;
                    it = DateTimeIntervalType.Minutes;
                    break;
                case "Час.":
                    temp *= 60 * 60;
                    it = DateTimeIntervalType.Hours;
                    break;
                case "Сут.":
                    temp *= 60 * 60 * 24;
                    it = DateTimeIntervalType.Days;
                    break;
            }

            long ns = Convert.ToInt64(temp);
            var ts = new TimeSpan(ns);
            return FáBil(it, ts, stærðBil);
        }
        //---

	    private void RedrawParam(GraphicParam param, DateTime timeBegin, DateTime timeEnd)
        {
            //const int maxDotCount = 20;
            const int maxDotCount = 2000;
            
            param.Series.Points.Clear();
            
            int j = -1;

            if (param.Dots.Count == 0) return;

            if (param.Dots.Count <= maxDotCount)
            {
                if (param.Dots[0].Time > timeBegin)
                {
                    param.Series.Points.AddXY(timeBegin, param.Dots[0].ToMomentReal().Mean);
                    j++;
                    param.Series.Points[j].IsEmpty = true;
                }

                foreach (var dot in param.Dots)
                {
                    if (j >= 0)
                    {
                        param.Series.Points.AddXY(dot.Time.AddMilliseconds(-1), param.Series.Points[j].YValues[0]);
                        j++;
                        if (param.Series.Points[j - 1].IsEmpty) param.Series.Points[j].IsEmpty = true;
                    }

                    param.Series.Points.AddXY(dot.Time, dot.ToMomentReal().Mean);
                    j++;
                    if (dot.Nd != 0) param.Series.Points[j].IsEmpty = true;
                }

                var lastDot = param.Dots.Last();
                if (lastDot.Time < timeEnd)
                {
                    param.Series.Points.AddXY(timeEnd, lastDot.ToMomentReal().Mean);
                    j++;
                    if (lastDot.Nd != 0) param.Series.Points[j].IsEmpty = true;
                }
            }
            else
            {
                TimeSpan ts = timeEnd.Subtract(timeBegin);
                double step = Math.Truncate(ts.TotalSeconds/maxDotCount);

                MomentReal curDot = null;
                //DateTime lastTime = FormGraphic.TimeViewBegin.AddSeconds(-2 * step);
                DateTime curTime;

                int i = 0;
                while ((i < param.Dots.Count) && (param.Dots[i].Time < timeBegin))
                {
                    curDot = param.Dots[i].ToMomentReal();
                    i++;
                }

                if (curDot != null)
                {
                    param.Series.Points.AddXY(curDot.Time, curDot.Mean);
                    j++;
                    if (curDot.Nd != 0) param.Series.Points[j].IsEmpty = true;
                    curTime = curDot.Time;
                    curDot = null;
                }
                else
                {
                    if (param.Dots[0].Time > timeBegin)
                    {
                        param.Series.Points.AddXY(timeBegin, param.Dots[0].ToMomentReal().Mean);
                        j++;
                        param.Series.Points[j].IsEmpty = true;
                        curTime = timeBegin;
                    }
                    else
                    {
                        curTime = timeBegin.AddSeconds(-2*step);
                        curDot = param.Dots[0].ToMomentReal();
                        i++;
                    }
                }

                int n = i;
                while ((n < param.Dots.Count) && (param.Dots[i].Time <= timeEnd)) n++;

                if ((n - i) < maxDotCount)
                {
                    while ((i < param.Dots.Count) && (param.Dots[i].Time < timeEnd))
                    {
                        param.Series.Points.AddXY(param.Dots[i].Time.AddMilliseconds(-1),
                                                  param.Series.Points[j].YValues[0]);
                        j++;
                        if (param.Series.Points[j - 1].IsEmpty) param.Series.Points[j].IsEmpty = true;

                        param.Series.Points.AddXY(param.Dots[i].Time, param.Dots[i].ToMomentReal().Mean);
                        j++;
                        if (param.Dots[i].Nd != 0) param.Series.Points[j].IsEmpty = true;

                        i++;
                    }
                }
                else //если точек много
                {
                    while ((i < param.Dots.Count) && (param.Dots[i].Time < timeEnd))
                    {
                        if (param.Dots[i].Time.Subtract(curTime).TotalSeconds > step)
                        {
                            if (curDot != null)
                            {
                                param.Series.Points.AddXY(curDot.Time, curDot.Mean);
                                j++;
                                if (curDot.Nd != 0) param.Series.Points[j].IsEmpty = true;
                            }

                            param.Series.Points.AddXY(param.Dots[i].Time.AddMilliseconds(-1),
                                                      param.Series.Points[j].YValues[0]);
                            j++;
                            if (param.Series.Points[j - 1].IsEmpty) param.Series.Points[j].IsEmpty = true;
                        }

                        MomentReal dotFirst = param.Dots[i].ToMomentReal();
                        MomentReal dotMin = param.Dots[i].ToMomentReal();
                        MomentReal dotMax = param.Dots[i].ToMomentReal();

                        DateTime lastTime = dotFirst.Time.AddSeconds(step);

                        curDot = null;

                        i++;
                        while ((i < param.Dots.Count) && (param.Dots[i].Time <= lastTime))
                        {
                            curDot = param.Dots[i].ToMomentReal();

                            if (param.Dots[i].ToMomentReal().Mean < dotMin.Mean)
                                dotMin = param.Dots[i].ToMomentReal();

                            if (param.Dots[i].ToMomentReal().Mean > dotMax.Mean)
                                dotMax = param.Dots[i].ToMomentReal();

                            i++;
                        }

                        MomentReal dot1;
                        MomentReal dot2;

                        if (dotMin.Time <= dotMax.Time)
                        {
                            dot1 = dotMin;
                            dot2 = dotMax;
                        }
                        else
                        {
                            dot1 = dotMax;
                            dot2 = dotMin;
                        }

                        param.Series.Points.AddXY(dot1.Time, dot1.Mean);
                        j++;
                        if (dot1.Nd != 0) param.Series.Points[j].IsEmpty = true;

                        if (dot2 != dot1) 
                        {
                            param.Series.Points.AddXY(dot2.Time, dot2.Mean);
                            j++;
                            if (dot2.Nd != 0) param.Series.Points[j].IsEmpty = true;
                        }

                        if (curDot != null)
                        {
                            param.Series.Points[j].IsEmpty = (curDot.Nd != 0);
                            if (curDot.Mean == dot2.Mean) curDot = null;
                        }
                        
                        curTime = (curDot == null) ? dot2.Time : curDot.Time;
                    }

                    if (curDot != null)
                    {
                        param.Series.Points.AddXY(curDot.Time, curDot.Mean);
                        j++;
                        if (curDot.Nd != 0) param.Series.Points[j].IsEmpty = true;
                    }
                }

                if (i < param.Dots.Count)
                {
                    param.Series.Points.AddXY(param.Dots[i].Time.AddMilliseconds(-1), param.Series.Points[j].YValues[0]);
                    j++;
                    if (param.Dots[i].Nd != 0) param.Series.Points[j].IsEmpty = true;
                }
                else
                {
                    param.Series.Points.AddXY(timeEnd, param.Series.Points[j].YValues[0]);
                    j++;
                    if (param.Series.Points[j - 1].IsEmpty) param.Series.Points[j].IsEmpty = true;
                }
            }

            param.IsUpdated = false;
            param.NumberOfLastViewed = param.Dots.Count - 1;
        }

        private void RedrawAllParams(DateTime timeBegin, DateTime timeEnd)
        {
            foreach (GraphicParam gp in ParamsAnalog) RedrawParam(gp, timeBegin, timeEnd);
        }

	    public void Otladka()
        {
            Timer1Tick(null, null);
        }
	}

	public enum TimerModes { Monitor, Analyzer }
    
	//comparer сортировки датагрида по цвету
	public class DatagridColorSort : IComparer
	{
		public int Compare(object xO, object yO)
		{
			var x = (DataGridViewRow) xO;
			var y = (DataGridViewRow)yO;
			if ((int)x.Cells["Group"].Value > (int)y.Cells["Group"].Value) return 1;
			if ((int)x.Cells["Group"].Value < (int)y.Cells["Group"].Value) return -1;
			if (int.Parse(x.Cells[1].Value.ToString()) > int.Parse(y.Cells[1].Value.ToString())) return 1;
			if (int.Parse(x.Cells[1].Value.ToString()) < int.Parse(y.Cells[1].Value.ToString())) return -1;

			return 0;
		}
	}
}