using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BaseLibrary;

namespace GraphicLibrary.Params
{
    public partial class FormGraphic : Form
    {
    #region Const
        internal const int AxWidth = 41;        //Ширина вертикальной оси
        internal const int AxYHeight = 20;      //высота дискр графика
        private const int AxSummand = 24;       //Разница между верхом чарта и верхом фоновой арии
        internal int AxYLabel = 17;             //высота наклейки к дискр графику
        internal int AxisXLabelFontHeight = 13; //Высота шрифта наклейки дискретного графика
    #endregion Const

    #region PublicProp
        //Заголовка формы
        public string Caption
        {
            get { return Text; }
            set { Text = value; }
        }

        //ВременнЫе границы
        public DateTime TimeBegin { get; private set; }
        public DateTime TimeEnd { get; private set; }

        public TimeSpan TotalTime
        {
            get { return TimeEnd.Subtract(TimeBegin); }
        }

        //Время начала и конца отображаемого периода
        private DateTime _timeViewBegin;
        private DateTime _timeViewEnd;

        public DateTime TimeViewBegin
        {
            get { return _timeViewBegin; }
            set
            {
                DateTime timeE = _timeViewEnd;

                if (value > timeE) timeE = timeE.AddSeconds(_scaleXMin);
                ChangeScaleView(value, timeE);
            }
        }

        public DateTime TimeViewEnd
        {
            get { return _timeViewEnd; }
            set
            {
                DateTime timeB = _timeViewBegin;

                if (value < timeB) timeB = timeB.AddSeconds(-_scaleXMin);
                ChangeScaleView(timeB, value);
            }
        }

        //Толщина линии графиков
        public int LineWidth
        {
            get { return int.Parse(cbLineWidth.Text); }
            set { cbLineWidth.Text = value.ToString(); }
        }

        //Кол-во графиков
        public int GraphicCount
        {
            get { return Graphics.Count; }
        }

        public int AnalogGraphicCount
        {
            get { return AnalogGraphics.Count; }
        }

        public int DiscretGraphicCount
        {
            get { return DiscretGraphics.Count; }
        }

        //Ошибка последней операции {сделать стек ошибок}
        public string LastOperationError { get; private set; }
    #endregion PublicProp

    #region PrivateProp
        private readonly System.Globalization.CultureInfo _currentCulture =
            System.Globalization.CultureInfo.CurrentCulture;

        //разделитель действительного числа
        private readonly string _separator;

        //Фоновые графики
        private GraphicVisual _analogBackGround;
        private GraphicVisual _discretBackGround;

        private GraphicVisual BackGround
        {
            get { return _analogBackGround ?? _discretBackGround; }
        }

        //Список всех графиков
        private readonly List<Graphic> _graphics = new List<Graphic>();

        internal List<Graphic> Graphics
        {
            get { return _graphics; }
        }

        //Список аналоговых графиков
        private readonly List<AnalogGraphic> _analogGraphics = new List<AnalogGraphic>();

        internal List<AnalogGraphic> AnalogGraphics
        {
            get { return _analogGraphics; }
        }

        //Список дискретных графиков
        private readonly List<DiscretGraphic> _discretGraphics = new List<DiscretGraphic>();

        internal List<DiscretGraphic> DiscretGraphics
        {
            get { return _discretGraphics; }
        }

        //Список вертикальных осей аналоговых графиков
        private readonly List<GroupY> _groupsY = new List<GroupY>();

        internal List<GroupY> GroupsY
        {
            get { return _groupsY; }
        }

        //Все Serieas/Arias графиков
        internal IEnumerable<GraphicVisual> GraphicVisuals
        {
            get
            {
                if (_analogBackGround != null) yield return _analogBackGround;
                if (_discretBackGround != null) yield return _discretBackGround;
                foreach (var gr in Graphics) yield return gr.GraphicVisual;
            }
        }

        private int _noas; //Кол-во видимых вертикальных (аналоговых) осей
        private int _nods; //Кол-во видимых дискретных графиков

        private int Noas
        {
            get { return _noas; }
            set
            {
                _noas = value;

                //При изменении кол-ва осей пересчитываем panelChart.AutoScrollMinSize
                if (AnalogGraphics.Count > 0 && DiscretGraphics.Count > 0)
                {
                    panelChart.AutoScrollMinSize = new Size(_noas*AxWidth + 500,
                                                            ((_nods + 1)*AxYHeight + 2*(Nods - 1) + AxYHeight) + 10 +
                                                            130);
                }
                else
                {
                    panelChart.AutoScrollMinSize = new Size(_noas*AxWidth + 500, panelChart.AutoScrollMinSize.Height);
                }
            }
        }

        private int Nods
        {
            get { return _nods; }
            set
            {
                _nods = value;

                //При изменении кол-ва графиков пересчитываем panelChart.AutoScrollMinSize
                //Выходит, что минимальный размер аналоговой арии по y - 126
                if (AnalogGraphics.Count == 0 && DiscretGraphics.Count > 0)
                {
                    panelChart.AutoScrollMinSize = new Size(panelChart.AutoScrollMinSize.Width,
                                                            ((_nods + 1)*AxYHeight + 2*(Nods - 1) + AxYHeight
                                                            /*новый график если*/) + 10); /*ну, и отступ*/
                }
                else
                {
                    panelChart.AutoScrollMinSize = new Size(panelChart.AutoScrollMinSize.Width,
                                                            ((_nods + 1)*AxYHeight + 2*(Nods - 1) + AxYLabel
                                                             /*новый график если*/+ 126 + 1));
                }
            }
        }

        //Номер текущего графика
        internal int CurGraphicNum { get; set; }

        //Скролл по Y
        private readonly ScrollBar _hscrollb = new HScrollBar();
        //Кнопка сбросить масштабирование по горизонтали
        private readonly Button _buttonScaleDrop = new Button();

        //дробная часть пустой части приближенного отображения от текущего локального отображения графика
        private double _fillScaleViewPerCentage = .8; // т.е. график отображен на столько процентов от экрана

        //~ private double _timerFactor1;     //в произведении дают интервал таймера
        //~ private int _timerFactor2 = 1000; //в произведении дают интервал таймера

        private double _scaleXMin = 1; //Минимум приближения по оси X

        private bool _panelBottomHide; //Спрятана ли нижняя панель
        private bool _panelRightHide; //Спрятана ли правая панель

        //Для обработки событий
        private bool _isMouseDown4Move;
        private bool _isMouseDown4Divide;
        private bool _isMouseDown4Expand;
        private int _dividedParam;
        private int _mouseH;
        private int _mouseY;
    #endregion PrivateProp

    #region Dynamic
        private enum EDynState
        {
            NotDyn,
            Dyn,
            DynStop
        }

        private EDynState _dynState = EDynState.NotDyn;

        //Динамический режим
        public bool IsDynamic
        {
            get { return _dynState != EDynState.NotDyn; }
        }

        //Период обновления динамического графика
        public double DynPeriod { get; private set; }
        //Режим обновления динамического графика: со здвигом (true), со сжатием (false)
        public bool IsDynShift { get; private set; }
    #endregion Dynamic

    #region Init
        public FormGraphic()
        {
            InitializeComponent();

            try
            {
                _separator = _currentCulture.NumberFormat.NumberDecimalSeparator;

                GraphicVisual.FreeColors();

                InitControls();
                InitControlEvents();
                DataGridInit();

                LastOperationError = "";
            }
            catch (Exception ex)
            {
                LastOperationError = "FormGraphic: " + ex.Message;
            }
        }

        private void InitControls()
        {
            try
            {
                //почему-то было в PrepareParam
                butViewPeriodToMin.Visible = false;
                butViewPeriodToMax.Left = 42;
                //~ AxYLabel = chartMain.Height * 9 / 303 + 8;

                //Наклейки на скрывающие кнопки
                labRightPanel.Image = Properties.Resources.rightl;
                labBottomPanel.Image = Properties.Resources.downl;

                //Для красивого отображение форм
                Application.EnableVisualStyles();
                
                //заполняем комбобоксы
                cbViewPeriodType.Items.AddRange(new[] {"Сут.", "Час.", "Мин.", "Сек."});
                cbDynPeriodType.Items.AddRange(new[] {"Час.", "Мин.", "Сек."});
                for (int i = 1; i < 11; i++) cbViewPeriod.Items.Add(i.ToString());
                cbViewPeriod.Items.AddRange(new[] {"12", "15", "20", "24", "30", "45", "60"});
                for (int i = 1; i < 11; i++) cbDynPeriod.Items.Add(i.ToString());
                for (int i = 1; i < 6; i++) cbLineWidth.Items.Add(i.ToString());
                cbViewPeriodStep.Items.AddRange(new[] {"3%", "5%", "10%", "25%", "50%"});

                //Легенда отключена
                chartMain.Legends.Last().Enabled = false;

                //Кнопки скрывания приделываем
                Controls.Add(labBottomPanel);
                labBottomPanel.BringToFront();
                Controls.Add(labRightPanel);
                labRightPanel.BringToFront();

                //Приделываем полосы прокрутки
                splitContainerV.Panel1.Controls.Add(_hscrollb);
                splitContainerV.Panel1.Controls.Add(_buttonScaleDrop);
                splitContainerV.Panel1.Resize += panelChart_Resize;

                _hscrollb.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                _hscrollb.Location = new Point(_buttonScaleDrop.Width, splitContainerV.Panel1.Size.Height - 20);
                _hscrollb.Width = chartMain.Size.Width - _buttonScaleDrop.Width;
                //_hscrollb.Visible = false;
                _hscrollb.Value = 0;
                _hscrollb.Maximum = 1000;
                _hscrollb.Minimum = 0;
                _hscrollb.LargeChange = 1001;
                _hscrollb.BringToFront();
                _hscrollb.Scroll += HScrollerScroll;
                //_hscrollb.MouseDoubleClick += ScrollDoubleHit;

                _buttonScaleDrop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                _buttonScaleDrop.Location = new Point(0, splitContainerV.Panel1.Size.Height - 20);
                //_buttonScaleDrop.Visible = false;
                _buttonScaleDrop.FlatStyle = FlatStyle.Popup;
                _buttonScaleDrop.Height = 17;
                //_buttonScaleDrop.Text = "max";
                _buttonScaleDrop.TextAlign = ContentAlignment.MiddleCenter;
                _buttonScaleDrop.Image = Properties.Resources.X_Full;
                _buttonScaleDrop.ImageAlign = ContentAlignment.MiddleCenter;
                _buttonScaleDrop.Font = new Font("Microsoft Sans Serif", 6F, FontStyle.Bold, GraphicsUnit.Point, ((204)));
                _buttonScaleDrop.BringToFront();
                _buttonScaleDrop.Click += butViewPeriodToMax_Click;

                //MouseWheel += WheelZooming;

                //Поле с номером текущего графика
                labCur.ForeColor = Color.White;
                labCur.TextAlign = ContentAlignment.MiddleCenter;
                labTimeCur.ForeColor = Color.White;
                labTimeCur.TextAlign = ContentAlignment.MiddleCenter;
                labCtrlCur.ForeColor = Color.White;
                labCtrlCur.TextAlign = ContentAlignment.MiddleCenter;
                labVizirL.ForeColor = Color.White;
                labVizirL.TextAlign = ContentAlignment.MiddleCenter;
                labVizirR.ForeColor = Color.White;
                labVizirR.TextAlign = ContentAlignment.MiddleCenter;

                //~ _chartTypeMode = GraphicTypeMode.Empty;

                butCurMinMaxApply.Enabled = false;
                tbCurMin.Enabled = false;
                tbCurMax.Enabled = false;
                //button8.Enabled = false;
                //button9.Enabled = false;
                rbDynStorageFrom.Enabled = false;
                rbDynShift.Enabled = false;

                //~ butViewPeriodDec.Enabled = false;
                //~ butViewPeriodInc.Enabled = false;
                //~ butViewPeriodToMin.Enabled = false;
                //~ butViewPeriodToMax.Enabled = false;

                panelChart.AutoScroll = true;

                contextMenuStrip.ShowImageMargin = false;
                contextMenuStrip.MaximumSize = new Size(40, 0);
                
                //Устанавливаем некоторые значения по умолчанию
                //~ cbViewPeriod.Text = "7";
                //~ cbViewPeriodType.Text = "Мин.";

                //~ _timerFactor1 = 1;
                //~ _timerFactor2 = 1000;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void InitControlEvents()
        {
            Shown += Form_BlockAppear; //Блокировка отображения формы, если не задан интервал времени
            
            //Назначение точности прокрутки по x
            //chart1.AxisScrollBarClicked += Chart1ScrollXClick;

            //запрещаем ввод не чисел в текстовые поля
            //~ cbViewPeriod.KeyPress += TextBoxInputReal;
            //~ cbDynPeriod.KeyPress += TextBoxInputReal;
            //~ tbCurMin.KeyPress += TextBoxInputAnyReal;
            //~ tbCurMax.KeyPress += TextBoxInputAnyReal;
            //~ cbLineWidth.KeyPress += TextBoxInputReal;
            //~ cbViewPeriodStep.KeyPress += TextBoxInputReal;
            //~ cbViewPeriodStep.KeyPress += ComboBox1KeyPress;
            //~ cbViewPeriodStep.Leave += ComboBox1Leave;

            //Режим отображения "Сдвиг"
            //~ rbDynShift.Click += SetScalePosition1;

            //~ butViewPeriodDec.Click += WheelZooming;
            //~ butViewPeriodInc.Click += WheelZooming;
            //~ butVizirNextCur.Click += ButtonTimeWalkAhead;
            //~ butVizirPrevCur.Click += ButtonTimeWalkBack;
            //~ butVizirPrevAll.Click += ButtonTimeStepBack;
            //~ butVizirNextAll.Click += ButtonTimeStepAhead;
            //~ butViewPeriodDec.MouseDown += ButtonHold;
            //~ butViewPeriodDec.MouseUp += ButtonRelease;
            //~ butViewPeriodInc.MouseDown += ButtonHold;
            //~ butViewPeriodInc.MouseUp += ButtonRelease;
            //~ butVizirPrevAll.MouseDown += ButtonHold;
            //~ butVizirPrevAll.MouseUp += ButtonRelease;
            //~ butVizirNextAll.MouseDown += ButtonHold;
            //~ butVizirNextAll.MouseUp += ButtonRelease;
            //~ butVizirNextCur.MouseDown += ButtonHold;
            //~ butVizirNextCur.MouseUp += ButtonRelease;
            //~ butVizirPrevCur.MouseDown += ButtonHold;
            //~ butVizirPrevCur.MouseUp += ButtonRelease;

            butCurMinMaxApply.MouseHover += ButtonsHover;
            butViewPeriodApply.MouseHover += ButtonsHover;
            butViewPeriodDec.MouseHover += ButtonsHover;
            butViewPeriodInc.MouseHover += ButtonsHover;
            butViewPeriodToMin.MouseHover += ButtonsHover;
            butViewPeriodToMax.MouseHover += ButtonsHover;
            butLineWidthCur.MouseHover += ButtonsHover;
            butLineWidthAll.MouseHover += ButtonsHover;
            butVizirNextCur.MouseHover += ButtonsHover;
            butVizirPrevCur.MouseHover += ButtonsHover;
            butVizirPrevAll.MouseHover += ButtonsHover;
            butVizirNextAll.MouseHover += ButtonsHover;

            //checkBox25.MouseHover += CheckboxHover;

            //Запасной метод для отладки
            //~ chartMain.MouseDown += SpareMethod;

            //"Обнуляет" кнопки путешествия по времени
            //~ butVizirNextCur.LostFocus += WalkColorReset;
            //~ chartMain.MouseDown += WalkColorReset;

            //Метод разлепляет графики, если мы перетащили один из скрепленных на область построения,
            //а также в случае выделения правой кнопкой мыши осуществляет масштабирование
            //~ chartMain.MouseUp += Chart1MouseUp;


            //при отображении визира появляется данные с его местоположением
            //chartMain.CursorPositionChanged += CursorPositionView;
            //chartMain.CursorPositionChanged += CursorPositionGridView;
            //Синхронизация курсоров аналоговых и дискретных
            //chartMain.CursorPositionChanging += CursorSynch;
            //chartMain.CursorPositionChanged += CursorSynchFinal;

            Closing += GarbageCollection; //Чистка мусора при закрытии формы
        }

        private void SetDynPeriod(double dynPeriod, bool isDynShift = true)
        {
            if (dynPeriod > 0)
            {
                DynPeriod = dynPeriod;
                IsDynShift = isDynShift;

                if (DynPeriod < 60)
                {
                    //~ _timerFactor1 = m;
                    //~ _timerFactor2 = 1000;
                    cbDynPeriod.Text = DynPeriod.ToString();
                    cbDynPeriodType.Text = "Сек.";
                }
                else
                {
                    //~ _timerFactor1 = m/60;
                    //~ _timerFactor2 = 1000*60;
                    cbDynPeriod.Text = (DynPeriod/60).ToString();
                    cbDynPeriodType.Text = "Мин.";
                }

                //timer1.Enabled = true;
            }
            else
            {
                cbDynPeriodType.Visible = false;
                cbDynPeriod.Visible = false;
                labDynPeriod.Visible = false;
                butDynShiftOnOff.Visible = false;
                butDynClear.Visible = false;
                rbDynStorageFrom.Visible = false;
                rbDynShift.Visible = false;
                dateTimePicker.Visible = false;
                butDynStorageFromApply.Visible = false;
                labViewPeriod.Location = new Point(labViewPeriod.Location.X, labViewPeriod.Location.Y - 89);
                cbViewPeriodType.Location = new Point(cbViewPeriodType.Location.X, cbViewPeriodType.Location.Y - 89);
                cbViewPeriod.Location = new Point(cbViewPeriod.Location.X, cbViewPeriod.Location.Y - 89);
                rbDynShift.Location = new Point(rbDynShift.Location.X, rbDynShift.Location.Y - 89);
                butViewPeriodApply.Top -= 89;
                butViewPeriodDec.Top = butViewPeriodApply.Top + 25;
                butViewPeriodInc.Top = butViewPeriodApply.Top + 25;
                butViewPeriodToMin.Top = butViewPeriodDec.Top + 25;
                butViewPeriodToMax.Top = butViewPeriodDec.Top + 25;
                cbViewPeriodStep.Top = butViewPeriodApply.Top + 25;
                rbDynStorageFrom.Text = "Полное";
                rbDynShift.Text = "Частичное";
                tabControl.Size = new Size(tabControl.Size.Width, Math.Max(
                    gbVizir.Bottom,
                    gbCtrl.Bottom) + 29);
                gbPeriod.Height = butViewPeriodToMin.Location.Y + 30;
                gbVizir.Location = new Point(2, gbPeriod.Height + 2);
                labTimeCurTxt.Location = new Point(labTimeCurTxt.Location.X,
                                                   gbVizir.Location.Y + gbVizir.Height + 5);
                labTimeCur.Location = new Point(labTimeCur.Location.X, labTimeCurTxt.Location.Y - 3);

                _fillScaleViewPerCentage = 1;

                dataGridView.Columns.Remove("Последнее");
            }
        }

        //Инициализация фоновых графиков
        private void InitBackGround(GraphicVisual visual)
        {
            visual.Series.Points.Clear();
            visual.Series.Points.AddXY(TimeBegin, 0);
            visual.Series.Points.AddXY(TimeEnd, 0);
        }

        private void InitBackGrounds()
        {
            if (_analogBackGround != null)
                InitBackGround(_analogBackGround);

            if (_discretBackGround != null)
                InitBackGround(_discretBackGround);
        }
    #endregion Init

    #region PublicFunctions
        //инициализация параметров формы
        //public void Init(DateTime beginTime, DateTime endTime, double dynPeriod = 0, bool isDynShift = true)
        //{
        //    if (beginTime < endTime)
        //    {
        //        TimeBegin = beginTime;
        //        TimeEnd = endTime;
        //    }
        //    else if (beginTime > endTime)
        //    {
        //        TimeBegin = endTime;
        //        TimeEnd = beginTime;
        //    }
        //    else
        //    {
        //        TimeBegin = beginTime.AddSeconds(-1);
        //        TimeEnd = beginTime;
        //    }

        //    DateTimeIntervalType intervalType;
        //    double intv;
        //    GetInitScaleView(TimeBegin, TimeEnd, out intervalType, out intv);
        //    SetCbViewPeriodType(intervalType, intv);

        //    SetDynPeriod(dynPeriod, isDynShift);
        //}

        //сделать для динамического режима
        //public void InitDyn(double viewInterval, double dynPeriod, bool isDynShift = true)
        //{
        //    if ((viewInterval > 0) && (dynPeriod > 0))
        //    {
        //        TimeBegin = DateTime.Now.AddSeconds(-viewInterval);
        //        TimeEnd = DateTime.Now;

        //        SetDynPeriod(dynPeriod, isDynShift);
        //    }
        //}

        public void Init(DateTime beginTime, DateTime endTime)
        {
            if (beginTime < endTime)
            {
                TimeBegin = beginTime;
                TimeEnd = endTime;
            }
            else if (beginTime > endTime)
            {
                TimeBegin = endTime;
                TimeEnd = beginTime;
            }
            else
            {
                TimeBegin = beginTime.AddSeconds(-1);
                TimeEnd = beginTime;
            }

            SetDynPeriod(0);

            GetAxisXScaleView(TimeBegin, TimeEnd);
            SetCbViewPeriodType(_axisXScaleViewSizeType, _axisXScaleViewSize);
        }

        //Добавление графиков
        //если существует график с таким code или id (если id != 0), то не добавляет график и ничего не меняет
        public AnalogGraphic AddAnalogGraphic(string code, int id = 0, string name = "", string subname = "",
                                              double min = 0, double max = 1, string units = "", int decPlaces = -1)
        {
            try
            {
                int fg = GraphicExists(code) ? 1 : 0;
                if ((fg == 0) && GraphicExists(id)) fg = 2;

                if (fg == 0)
                {
                    if (AnalogGraphics.Count == 0)
                    {
                        int lineWidth = int.Parse(cbLineWidth.Text);
                        _analogBackGround = new GraphicVisual(lineWidth, 0, true, true);

                        InitBackGround(_analogBackGround);
                        SetAxisXScaleView(_analogBackGround);

                        chartMain.ChartAreas.Add(_analogBackGround.Area);
                        chartMain.Series.Add(_analogBackGround.Series);

                        AxisXLabelFontHeight = _analogBackGround.Area.AxisX.LabelStyle.Font.Height;
                    }

                    var par = new Param(id, code, name, subname, DataType.Real, units, min, max);
                    int num = Graphics.Count == 0 ? 1 : Graphics.Last().Num + 1;
                    var gr = new AnalogGraphic(this, par, num);
                    AnalogGraphics.Add(gr);
                    Graphics.Add(gr);
                    GroupsY.Add(new GroupY(gr));
                    Noas++;

                    ReRanking();
                    chartMain.Controls.Add(gr.GroupY.Ax);
                    chartMain.ChartAreas.Add(gr.Area);
                    chartMain.Series.Add(gr.Series);

                    SetAxisXScaleView(gr.GraphicVisual);

                    DataGridAddRow(gr);

                    int curNum = (num != 1) ? CurGraphicNum : num;
                    CurGraphicChange(curNum);

                    LastOperationError = "";
                    return gr;
                }

                LastOperationError = "AddAnalogGraphic: Не удалось добавить график: Параметр с данным " +
                                     (fg == 1 ? "code" : "id") + " уже существует";
                return null;
            }
            catch (Exception exception)
            {
                LastOperationError = "AddAnalogGraphic: Не удалось добавить график: " + exception.Message;
                return null;
            }
        }

        public DiscretGraphic AddDiscretGraphic(string code, int id = 0, string name = "", string subname = "")
        {
            try
            {
                int fg = (GraphicExists(code)) ? 1 : 0;
                if ((fg == 0) && GraphicExists(id)) fg = 2;

                if (fg == 0)
                {
                    if (DiscretGraphics.Count == 0)
                    {
                        int lineWidth = int.Parse(cbLineWidth.Text);
                        _discretBackGround = new GraphicVisual(lineWidth, 0, false, true);

                        InitBackGround(_discretBackGround);
                        SetAxisXScaleView(_discretBackGround);

                        chartMain.ChartAreas.Add(_discretBackGround.Area);
                        chartMain.Series.Add(_discretBackGround.Series);
                    }

                    var par = new Param(id, code, name, subname, DataType.Boolean, "", 0, 1);
                    int num = Graphics.Count == 0 ? 1 : Graphics.Last().Num + 1;
                    var gr = new DiscretGraphic(this, par, num);
                    DiscretGraphics.Add(gr);
                    Graphics.Add(gr);
                    Nods++;

                    ReRanking();
                    chartMain.Controls.Add(gr.AxCap);
                    chartMain.ChartAreas.Add(gr.Area);
                    chartMain.Series.Add(gr.Series);

                    SetAxisXScaleView(gr.GraphicVisual);

                    DataGridAddRow(gr);

                    //int curNum = (num != 1) ? CurGraphicNum : num;
                    //CurGraphicChange(curNum);
                    if (CurGraphicNum == 0) CurGraphicChange(num);

                    LastOperationError = "";
                    return gr;
                }

                LastOperationError = "AddDiscretGraphic: Не удалось добавить график: Параметр с данным " +
                                     (fg == 1 ? "code" : "id") + " уже существует";
                return null;
            }
            catch (Exception exception)
            {
                LastOperationError = "AddDiscretGraphic: Не удалось добавить график: " + exception.Message;
                return null;
            }
        }

        //SQL/Access
        //public string SetDatabase(string databaseType, string databaseFile){}
        //public string LoadParams(string stSql){}

        //public string LoadValues(string stSql){}
        //public string LoadValues(string stSql, DateTime timeBegin, DateTime timeEnd){}

        public bool AddAnalogValue(string code, DateTime time, double val, int nd = 0)
        {
            AnalogGraphic gr = null;
            foreach (AnalogGraphic grS in AnalogGraphics)
            {
                if (grS.Param.Code == code)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddAnalogValue(int id, DateTime time, double val, int nd = 0)
        {
            AnalogGraphic gr = null;
            foreach (AnalogGraphic grS in AnalogGraphics)
            {
                if (grS.Param.Id == id)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddDiscretValue(string code, DateTime time, bool val, int nd = 0)
        {
            DiscretGraphic gr = null;
            foreach (DiscretGraphic grS in DiscretGraphics)
            {
                if (grS.Param.Code == code)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddDiscretValue(int id, DateTime time, bool val, int nd = 0)
        {
            DiscretGraphic gr = null;
            foreach (DiscretGraphic grS in DiscretGraphics)
            {
                if (grS.Param.Id == id)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddValue(string code, DateTime time, double val, int nd = 0)
        {
            Graphic gr = null;
            foreach (Graphic grS in Graphics)
            {
                if (grS.Param.Code == code)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddValue(string code, DateTime time, int val, int nd = 0)
        {
            Graphic gr = null;
            foreach (Graphic grS in Graphics)
            {
                if (grS.Param.Code == code)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddValue(string code, DateTime time, bool val, int nd = 0)
        {
            Graphic gr = null;
            foreach (Graphic grS in Graphics)
            {
                if (grS.Param.Code == code)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddValue(int id, DateTime time, double val, int nd = 0)
        {
            Graphic gr = null;
            foreach (Graphic grS in Graphics)
            {
                if (grS.Param.Id == id)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddValue(int id, DateTime time, int val, int nd = 0)
        {
            Graphic gr = null;
            foreach (Graphic grS in Graphics)
            {
                if (grS.Param.Id == id)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        public bool AddValue(int id, DateTime time, bool val, int nd = 0)
        {
            Graphic gr = null;
            foreach (Graphic grS in Graphics)
            {
                if (grS.Param.Id == id)
                {
                    gr = grS;
                    break;
                }
            }

            if (gr != null)
            {
                gr.AddValue(time, val, nd);
                return true;
            }

            return false;
        }

        //public void DeleteGraphic(string code);
        //public void DeleteGraphic(int id);
        //public void DeleteGraphicByNum(int num);
        //public void ClearParams();

        //Поиск графиков
        public Graphic GetGraphic(string code)
        {
            foreach (Graphic gr in Graphics)
                if (gr.Param.Code == code) return gr;
            return null;
        }

        public Graphic GetGraphic(int id)
        {
            foreach (Graphic gr in Graphics)
                if (gr.Param.Id == id) return gr;
            return null;
        }

        public Graphic GetGraphicByNum(int num)
        {
            foreach (Graphic gr in Graphics)
                if (gr.Num == num) return gr;
            return null;
        }

        public bool GraphicExists(string code)
        {
            return (GetGraphic(code) != null);
        }

        public bool GraphicExists(int id)
        {
            return (GetGraphic(id) != null);
        }

        public bool GraphicNumExists(int num)
        {
            return (GetGraphicByNum(num) != null);
        }

        //Отрисовка графиков
        public void RepaintGraphic(Graphic graphic)
        {
            try
            {
                SetScaleXPosition(graphic);
            }
            catch (OverflowException err)
            {
                MessageBox.Show(err.Message + "\n" + err.StackTrace);
            }
        }

        public void RepaintGraphic(string code)
        {
            var gr = GetGraphic(code);
            if (gr != null) RepaintGraphic(gr);
        }

        public void RepaintGraphic(int id)
        {
            var gr = GetGraphic(id);
            if (gr != null) RepaintGraphic(gr);
        }

        public void RepaintGraphicByNum(int num)
        {
            var gr = GetGraphicByNum(num);
            if (gr != null) RepaintGraphic(gr);
        }

        public void RepaintAllGraphics()
        {
            foreach (var gr in Graphics) RepaintGraphic(gr);
        }

        public void DeleteGraphic(Graphic graphic)
        {
            //удаление Series и Area
            //удаление из спимков
            //удаление фона, если последний
            //для аналоговых: удаление оси (если один в оси) или удаление из оси
            //для дискретных: удаление AxCap
            //удаление из датагрида
            //перенумерация графиков (нужно ли?)
            //перерисовка

            try
            {
                int victimNumber = graphic.Num;

                chartMain.Series.Remove(graphic.Series);
                chartMain.ChartAreas.Remove(graphic.Area);

                if (graphic.IsAnalog)
                {
                    AnalogGraphics.Remove((AnalogGraphic)graphic);

                    if (AnalogGraphics.Count == 0)
                    {
                        chartMain.ChartAreas.Remove(_analogBackGround.Area);
                        chartMain.Series.Remove(_analogBackGround.Series);
                        _analogBackGround = null;
                    }

                    if (Graphics.Count == 0)
                    {
                        butCurMinMaxApply.Enabled = false;
                        tbCurMin.Enabled = false;
                        tbCurMax.Enabled = false;
                    }

                    //if (_chartTypeMode == GraphicTypeMode.Analog)
                    //{
                    //    if (ParamsAnalog.Count == 1)
                    //    {
                    //        _chartTypeMode = GraphicTypeMode.Empty;
                    //        ParamsAnalog.First().Series.Points.Clear();
                    //        ParamsDiscrete.First().Area.CursorX.LineColor = Color.Transparent;
                    //        ParamsDiscrete.First().Area.CursorX.SelectionColor = Color.Transparent;
                    //        ParamsAnalog.First().Area.CursorX.LineColor = Color.Transparent;
                    //        ParamsAnalog.First().Area.CursorX.SelectionColor = Color.Transparent;
                    //        ParamsAnalog.First().Area.CursorX.IsUserEnabled = false;
                    //        ParamsAnalog.First().Area.CursorX.IsUserSelectionEnabled = false;
                    //        butCurMinMaxApply.Enabled = false;
                    //        tbCurMin.Enabled = false;
                    //        tbCurMax.Enabled = false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (ParamsAnalog.Count == 1)
                    //    {
                    //        _chartTypeMode = GraphicTypeMode.Discrete;
                    //        ParamsAnalog.First().Series.Points.Clear();
                    //        ParamsAnalog.First().Area.CursorX.LineColor = Color.Transparent;
                    //        ParamsAnalog.First().Area.CursorX.SelectionColor = Color.Transparent;
                    //        ParamsAnalog.First().Area.CursorX.IsUserEnabled = false;
                    //        ParamsAnalog.First().Area.CursorX.IsUserSelectionEnabled = false;
                    //    }
                    //}

                    //int overlayNext = VisibleOverlaySearch(victimGraph);
                    //GraphicParam newTopGraph = GetParam(overlayNext);
                    
                    //if (victimGraph.IsVisible)
                    //{
                    //    if (overlayNext < 1)
                    //    {
                    //        if (!victimGraph.AxY.IsHidden) Noas--;
                    //        if (victimGraph.AxY.AxesOverlay.Count == 1)
                    //        {
                    //            chart1.Controls.Remove(victimGraph.AxY.Ax);
                    //            AxesY.Remove(victimGraph.AxY);
                    //        }
                    //        else
                    //        {
                    //            victimGraph.AxY.Ax.Visible = false;
                    //            int df = victimGraph.AxY.AxesOverlay.Find(x => x != victimGraph.Index);
                    //            newTopGraph = GetParam(df);
                    //            victimGraph.AxY.UpperParam = newTopGraph;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        victimGraph.AxY.UpperParam = newTopGraph;
                    //    }
                    //}

                    //if (victimGraph.AxY.AxesOverlay.Count == 2)
                    //{
                    //    victimGraph.AxY.IsOverlayed = false;
                    //}

                    //victimGraph.AxY.AxesOverlay.Remove(victimGraph.Index);
                    ////for (int index = 0; index < victimGraph.AxY.AxesOverlay.Count; index++)
                    ////{
                    ////    if (victimGraph.AxY.AxesOverlay[index] >= victimNumber) victimGraph.AxY.AxesOverlay[index]--;
                    ////}

                    //if (newTopGraph != null)
                    //    foreach (var t in newTopGraph.AxY.AxesOverlay)
                    //    {
                    //        //dataGridView1[0, t - 1].Style.BackColor = newTopGraph.Series.Color;
                    //        //dataGridView1[0, t - 1].Style.SelectionBackColor = newTopGraph.Series.Color;
                    //        WantedRow(t).Cells[0].Style.BackColor = newTopGraph.Series.Color;
                    //        WantedRow(t).Cells[0].Style.SelectionBackColor = newTopGraph.Series.Color;
                    //        WantedRow(t).Cells["Group"].Value = newTopGraph.AxY.UpperParam.Index;
                    //    }
                    //else
                    //    foreach (var t in victimGraph.AxY.AxesOverlay)
                    //    {
                    //        WantedRow(t).Cells[0].Style.BackColor = Color.White;
                    //        WantedRow(t).Cells[0].Style.SelectionBackColor = Color.White;
                    //        WantedRow(t).Cells["Group"].Value = newTopGraph.AxY.UpperParam.Index;
                    //    }
                    //dataGridView1.Sort(new DatagridColorSort());
                }
                else //if(graphic.IsAnalog)
                {
                    chartMain.Controls.Remove(((DiscretGraphic) graphic).AxCap);
                    DiscretGraphics.Remove((DiscretGraphic) graphic);

                    if(DiscretGraphics.Count == 0)
                    {
                        chartMain.ChartAreas.Remove(_discretBackGround.Area);
                        chartMain.Series.Remove(_discretBackGround.Series);
                        _discretBackGround = null;
                    }

                    if (Graphics.Count == 0)
                    {
                        butCurMinMaxApply.Enabled = false;
                        tbCurMin.Enabled = false;
                        tbCurMax.Enabled = false;
                    }
                    
                    //if (_chartTypeMode == GraphicTypeMode.Discrete)
                    //{
                    //    if (ParamsDiscrete.Count == 1)
                    //    {
                    //        _chartTypeMode = GraphicTypeMode.Empty;
                    //        ParamsDiscrete.First().Area.CursorX.LineColor = Color.Transparent;
                    //        ParamsDiscrete.First().Area.CursorX.SelectionColor = Color.Transparent;
                    //        ParamsAnalog.First().Area.CursorX.LineColor = Color.Transparent;
                    //        ParamsAnalog.First().Area.CursorX.SelectionColor = Color.Transparent;
                    //        ParamsDiscrete.First().Area.CursorX.IsUserEnabled = false;
                    //        ParamsDiscrete.First().Area.CursorX.IsUserSelectionEnabled = false;
                    //        butCurMinMaxApply.Enabled = false;
                    //        tbCurMin.Enabled = false;
                    //        tbCurMax.Enabled = false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (ParamsAnalog.Count == 1)
                    //    {
                    //        _chartTypeMode = GraphicTypeMode.Analog;
                    //        ParamsDiscrete.First().Area.CursorX.LineColor = Color.Transparent;
                    //        ParamsDiscrete.First().Area.CursorX.SelectionColor = Color.Transparent;
                    //        ParamsDiscrete.First().Area.CursorX.IsUserEnabled = false;
                    //        ParamsDiscrete.First().Area.CursorX.IsUserSelectionEnabled = false;
                    //    }
                    //}

                    if (!graphic.IsHidden) Nods--;
                }

                //удаление строки из датагрида
                dataGridView.Rows.Remove(GetRowByGraphicNum(victimNumber));

                //перенкмерация параметров
                foreach (var gr in Graphics)
                    if(gr.Num > victimNumber)
                    {
                        gr.NumDecrease();
                        //dataGridView["№ в таблице", gr.Num - 1].Value = gr.Num;
                        GetRowByGraphicNum(gr.Num + 1).Cells["№ в таблице"].Value = gr.Num;

                        if(gr.IsDiscret) ((DiscretGraphic) gr).AxCap.Text = gr.Num.ToString();
                    }
                
                foreach (var axY in GroupsY)
                {
                    if (int.Parse(axY.Ax.Name) >= victimNumber)
                    {
                        axY.AxCap.Text = (axY.UpperGraphic.Num).ToString();
                        axY.Ax.Name = (axY.UpperGraphic.Num).ToString();
                        //axY.IsInPercent = axY.UpperGraphic.GroupY.PercentMode;
                    }
                }
                
                //перерисовка оставшихся
                ReRanking();

                if (dataGridView.CurrentRow != null) 
                    labCur.Text = dataGridView.CurrentRow.Cells[1].Value.ToString();
                else
                {
                    labCur.Text = "";
                    labCur.BackColor = Color.White;
                    labTimeCur.Text = "";
                    labTimeCur.BackColor = Color.White;
                    labCtrlCur.Text = "";
                    labCtrlCur.BackColor = Color.White;
                    labVizirL.Text = "";
                    labVizirL.BackColor = Color.White;
                    labVizirR.Text = "";
                    labVizirR.BackColor = Color.White;
                    butVizirNextCur.ForeColor = Color.Black;
                    butVizirPrevCur.ForeColor = Color.Black;
                    CurGraphicNum = 0;
                }
                
                DataGridSelectionChanged(null, null);
                //RadioButton4CheckedChanged(null, null);
                if (dataGridView.CurrentRow != null) dataGridView.CurrentRow.Selected = true;
                //if (GetParam(CurrentParamNumber).DataTypeD != DataType.Boolean) ToTopOverlay(GetParam(CurrentParamNumber));
            }
            catch (Exception exception)
            {
                //Error = new ErrorCommand("Не удалось удалить параметр (DeleteParam)", exception);
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
            }
        }

        public void DeleteGraphic(string code)
        {
            var gr = GetGraphic(code);
            if (gr != null) DeleteGraphic(gr);
        }

        public void DeleteGraphic(int id)
        {
            var gr = GetGraphic(id);
            if (gr != null) DeleteGraphic(gr);
        }

        public void DeleteGraphicByNum(int num)
        {
            var gr = GetGraphicByNum(num);
            if (gr != null) DeleteGraphic(gr);
        }

        //чистка мусора
        public void Gethering()
        {
            GC.Collect();
        }
    #endregion PublicFunctions

    #region ScaleViewX
        //сделать функцию независмой от параметра scaleType (т.е. вид подписи должен определяться полным интервалом)
        private string _axisXLabelFormat;
        private double _axisXInterval;
        private DateTimeIntervalType _axisXIntervalType;
        private double _axisXScaleViewSize;
        private DateTimeIntervalType _axisXScaleViewSizeType;

        private void GetAxisXScaleView(DateTimeIntervalType scaleType, DateTime timeViewBegin, DateTime timeViewEnd)
        {
            DateTime timeB = timeViewBegin;
            DateTime timeE = timeViewEnd;

            if (timeViewBegin > timeViewEnd)
            {
                timeB = timeViewEnd;
                timeE = timeViewBegin;
            }

            if (timeB < TimeBegin) timeB = TimeBegin;
            if (timeE.Subtract(timeB).TotalSeconds < _scaleXMin) timeE = timeViewBegin.AddSeconds(_scaleXMin);

            if (timeE > TimeEnd) timeE = TimeEnd;
            if (timeE.Subtract(timeB).TotalSeconds < _scaleXMin) timeB = timeViewEnd.AddSeconds(-_scaleXMin);

            _timeViewBegin = timeB;
            _timeViewEnd = timeE;

            var ts = timeE.Subtract(timeB);

            _axisXIntervalType = scaleType;
            _axisXScaleViewSizeType = scaleType;
            _axisXInterval = GetAxisXInterval(scaleType, TimeViewEnd.Subtract(TimeViewBegin));

            //string format = "HH':'mm':'ss";
            //if (ts.TotalDays > 1)
            //    format = "dd'.'MM HH':'mm";
            //else
            //    if (ts.TotalHours > 1)
            //        format = "HH':'mm':'ss";
            //    else
            //        if (ts.TotalMinutes > 1)
            //            format = "HH':'mm':'ss";
            //        else
            //            format = "HH':'mm':'ss','fff";

            string format = (TimeBegin.Date != TimeEnd.Date) ? "dd'.'MM'.'yy HH':'mm" : "HH':'mm";

            if (ts.TotalHours < 1) format = format + "':'ss";
            if (ts.TotalMinutes < 1) format = format + "','fff";

            double size = _scaleXMin;

            switch (scaleType)
            {
                case DateTimeIntervalType.Seconds:
                    //format = "HH':'mm':'ss','fff";
                    //format = format + ":'ss','fff";
                    size = ts.TotalSeconds;
                    break;
                case DateTimeIntervalType.Minutes:
                    //format = "HH':'mm':'ss";
                    //format = format + ":'ss";
                    size = ts.TotalMinutes;
                    break;
                case DateTimeIntervalType.Hours:
                    //format = "HH':'mm':'ss";
                    //format = format + ":'ss";
                    size = ts.TotalHours;
                    break;
                case DateTimeIntervalType.Days:
                    //format = "dd'.'MM'.'yy HH':'mm";
                    size = ts.TotalDays;
                    break;
            }

            _axisXScaleViewSize = size;
            _axisXLabelFormat = format;
        }

        private void GetAxisXScaleView(DateTime timeViewBegin, DateTime timeViewEnd)
        {
            DateTimeIntervalType scaleType;

            TimeSpan ts = timeViewEnd.Subtract(timeViewBegin);
            if (ts.TotalDays > 2)
            {
                scaleType = DateTimeIntervalType.Days;
            }
            else if (ts.TotalHours > 2)
            {
                scaleType = DateTimeIntervalType.Hours;
            }
            else if (ts.TotalMinutes > 2)
            {
                scaleType = DateTimeIntervalType.Minutes;
            }
            else
            {
                scaleType = DateTimeIntervalType.Seconds;
            }

            GetAxisXScaleView(scaleType, timeViewBegin, timeViewEnd);
        }

        private void SetAxisXScaleView(GraphicVisual grVisual)
        {
            try
            {
                grVisual.Area.AxisX.LabelStyle.Format = _axisXLabelFormat;
                grVisual.Area.AxisX.IntervalType = _axisXIntervalType;
                grVisual.Area.AxisX.Interval = _axisXInterval;
                grVisual.Area.AxisX.ScaleView.SizeType = _axisXScaleViewSizeType;
                grVisual.Area.AxisX.ScaleView.Position = TimeViewBegin.ToOADate();
                grVisual.Area.AxisX.ScaleView.Size = _axisXScaleViewSize;
            }
            catch (OverflowException)
            {
                MessageBox.Show("OverflowException");
            }
        }

        private void SetAxisXScaleView()
        {
            foreach (var grV in GraphicVisuals)
            {
                try
                {
                    grV.Area.AxisX.LabelStyle.Format = _axisXLabelFormat;
                    grV.Area.AxisX.IntervalType = _axisXIntervalType;
                    grV.Area.AxisX.Interval = _axisXInterval;
                    grV.Area.AxisX.ScaleView.SizeType = _axisXScaleViewSizeType;
                    grV.Area.AxisX.ScaleView.Position = TimeViewBegin.ToOADate();
                    grV.Area.AxisX.ScaleView.Size = _axisXScaleViewSize;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("OverflowException");
                }
            }
        }

        private void SetScaleXPosition(Graphic graphic)
        {
            try
            {
                graphic.ReDrawStep(_timeViewBegin, _timeViewEnd);
                SetAxisXScaleView(graphic.GraphicVisual);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void SetScaleXPosition()
        {
            if (_analogBackGround != null) SetAxisXScaleView(_analogBackGround);
            if (_discretBackGround != null) SetAxisXScaleView(_discretBackGround);

            foreach (var gr in Graphics)
                SetScaleXPosition(gr);
        }

        private void SetScrollPosition()
        {
            double totalIntv = TimeEnd.Subtract(TimeBegin).TotalSeconds;
            double intv = TimeViewEnd.Subtract(TimeViewBegin).TotalSeconds;

            _hscrollb.LargeChange = (int) (intv/totalIntv*1000) + 1;
            _hscrollb.Value = (int) (TimeViewBegin.Subtract(TimeBegin).TotalSeconds/totalIntv*1000);
        }

        private void ChangeScaleView(DateTimeIntervalType scaleType, DateTime timeViewBegin, DateTime timeViewEnd)
        {
            GetAxisXScaleView(scaleType, timeViewBegin, timeViewEnd);
            SetCbViewPeriodType(_axisXScaleViewSizeType, _axisXScaleViewSize);
            SetScaleXPosition();
            SetScrollPosition();
        }

        private void ChangeScaleView(DateTime timeViewBegin, DateTime timeViewEnd)
        {
            ChangeScaleView(_axisXScaleViewSizeType, timeViewBegin, timeViewEnd);
        }

        private void ChangeScaleViewAuto(DateTime timeViewBegin, DateTime timeViewEnd)
        {
            GetAxisXScaleView(timeViewBegin, timeViewEnd);
            SetCbViewPeriodType(_axisXScaleViewSizeType, _axisXScaleViewSize);
            SetScaleXPosition();
            SetScrollPosition();
        }

        private void HScrollerScroll(object sender, ScrollEventArgs e)
        {
            try
            {
                double totalIntv = TimeEnd.Subtract(TimeBegin).TotalSeconds;
                double intv = TimeViewEnd.Subtract(TimeViewBegin).TotalSeconds;

                DateTime timeB = TimeBegin.AddMilliseconds(totalIntv*_hscrollb.Value);
                DateTime timeE = timeB.AddSeconds(intv);

                GetAxisXScaleView(_axisXScaleViewSizeType, timeB, timeE);
                SetScaleXPosition();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Scroll/n" + ex.Message);
            }
        }
    #endregion ScaleViewX

    #region ScaleViewY
        //private void SetAxisYScaleView(GraphicVisual grVisual, double minViewY, double maxViewY)
        //{
        //    grVisual.Area.AxisY.ScaleView.Position = minViewY;
        //    grVisual.Area.AxisY.ScaleView.Size = maxViewY - minViewY;

        //    grVisual.Area.AxisY.MajorGrid.Interval = (grVisual.Area.AxisY.ScaleView.ViewMaximum -
        //                                              grVisual.Area.AxisY.ScaleView.ViewMinimum)/5;
        //}

        //private void SetAxisYScaleView(GroupY axY, double minViewY, double maxViewY)
        //{
        //    foreach (var gr in Graphics)
        //        SetAxisYScaleView(gr.GraphicVisual, minViewY, maxViewY);

        //    axY.ViewMin = minViewY;
        //    axY.ViewMax = maxViewY;
        //}

        //private void SetScaleYPosition(GroupY axY, double minViewY, double maxViewY, bool inPercent = false)
        //{
            
        //}


        //private void AxYToAutoScale(GroupY axY)
        //{
        //    try
        //    {
        //        //перенести в конец
        //        axY.IsAutoScale = true;

        //        double? scaleYMin = null;
        //        double? scaleYMax = null;

        //        foreach (var gr in axY.Graphics)
        //        {
        //            double? locMin = gr.MinValue();
        //            double? locMax = gr.MaxValue();

        //            if (locMin != null)
        //                if ((scaleYMin == null) || (locMin < scaleYMin)) scaleYMin = locMin;

        //            if (locMax != null)
        //                if ((scaleYMax == null) || (locMax > scaleYMax)) scaleYMax = locMax;
        //        }

        //        if (scaleYMin == null) scaleYMin = 0;
        //        if (scaleYMax == null) scaleYMax = 0;

        //        if (scaleYMax == scaleYMin)
        //        {
        //            scaleYMax += 1;
        //            scaleYMin -= 1;
        //        }

        //        foreach (var gr in axY.Graphics)
        //        {
        //            //gr.Area.AxisY.ScaleView.Position = (double) scaleYMin*1.03 - (double) scaleYMax*0.03;
        //            //gr.Area.AxisY.ScaleView.Size = (double) scaleYMax*1.06 - (double) scaleYMin*1.06;

        //            gr.Area.AxisY.ScaleView.Position = (double) scaleYMin - 1;
        //            gr.Area.AxisY.ScaleView.Size = (double) scaleYMax - (double) scaleYMin + 2;

        //            gr.Area.AxisY.MajorGrid.Interval = (gr.Area.AxisY.ScaleView.ViewMaximum -
        //                                                gr.Area.AxisY.ScaleView.ViewMinimum)/5;
        //        }

        //        axY.ViewMin = (double) scaleYMin - 1;
        //        axY.ViewMax = (double) scaleYMax + 1;

        //        //curParam.Area.AxisY.ScaleView.Position = scaleYMin * 1.03 - scaleYMax * 0.03;
        //        //curParam.Area.AxisY.ScaleView.Size = scaleYMax * 1.06 - scaleYMin * 1.06;

        //        //double tmin, tmax; //временные переменные
        //        //int tpos = (int)Math.Log10(scaleYMax - scaleYMin) - 2;

        //        //tmin = (int)((curParam.Area.AxisY.ScaleView.Position / Math.Pow(10, tpos))) * Math.Pow(10, tpos);
        //        //tmax =
        //        //    (int)
        //        //    ((curParam.Area.AxisY.ScaleView.Position + curParam.Area.AxisY.ScaleView.Size) /
        //        //     Math.Pow(10, tpos)) *
        //        //    Math.Pow(10, tpos);

        //        //curParam.AxY.ViewMaxMsr = tmax;
        //        //curParam.AxY.ViewMinMsr = tmin;
        //        //int tposPrc =
        //        //    (int)
        //        //    Math.Log10(curParam.ValueToPercent(curParam.AxY.ViewMaxMsr) -
        //        //               curParam.ValueToPercent(curParam.AxY.ViewMinMsr)) - 2;
        //        //curParam.AxY.ViewMaxPrc = ((int)(curParam.ValueToPercent(tmax) / Math.Pow(10, tposPrc))) *
        //        //                          Math.Pow(10, tposPrc);
        //        //curParam.AxY.ViewMinPrc = ((int)(curParam.ValueToPercent(tmin) / Math.Pow(10, tposPrc))) *
        //        //                          Math.Pow(10, tposPrc);

        //        //curParam.Area.AxisY.MajorGrid.Interval = (curParam.Area.AxisY.ScaleView.ViewMaximum -
        //        //                                          curParam.Area.AxisY.ScaleView.ViewMinimum) / 5;

        //        //button4.Enabled = true;
        //        //textBox1.Enabled = true;
        //        //textBox2.Enabled = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
        //    }
        //}

        //private void AxYToAutoScale(AnalogGraphic graphic)
        //{
        //    AxYToAutoScale(graphic.GroupY);
        //}

        //private void AxYToAutoScale(int graphicNum)
        //{
        //    var gr = GetGraphicByNum(graphicNum);
        //    if (gr != null)
        //    {
        //        if (gr.IsAnalog) AxYToAutoScale(((AnalogGraphic) gr).GroupY);
        //    }
        //}
    #endregion ScaleViewY

    #region PrivateFunction
        private static double GetAxisXInterval(DateTimeIntervalType intervalType, TimeSpan viewPeriod, byte gridFreq = 1)
        {
            //возвращает длину интервала (AxisX.IntervalCount)
            //в зависимости от типа интервала (intervalType), и отображаемого промежутка (viewPeriod), 
            //а также частоты сетки (gridFreq): 0 - редкая (~ 6 делений), 1 - частая (~ 12 делений), (>1 = 1)

            double a; //= 1;
            bool isSec = false;


            if (viewPeriod.TotalDays > 2)
                a = viewPeriod.TotalDays;
            else if (viewPeriod.TotalHours > 2)
                a = viewPeriod.TotalHours;
            else if (viewPeriod.TotalMinutes > 2)
                a = viewPeriod.TotalMinutes;
            else
            {
                a = viewPeriod.TotalSeconds;
                isSec = true;
            }

            double b = a/((gridFreq == 0) ? 6 : 12);

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
                else if (b >= (1.0/3))
                    b = 1.0/3;
                else if (b >= .25)
                    b = .25;
                else if (b >= (1.0/6))
                    b = 1.0/6;
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
                    double c = b*10;
                    while (c > 1)
                    {
                        i++;
                        c *= 10;
                    }
                    b = Math.Round(b, i);
                }
            }

            a = a/b;

            if (a >= ((gridFreq == 0) ? 7 : 13))
                if (!isSec)
                {
                    if (b >= 2)
                        b = b + 1;
                    else if (b >= .5)
                        b = b + .5;
                    else if (b >= (1.0/3))
                        b = .5;
                    else if (b >= .25)
                        b = 1.0/3;
                    else if (b >= (1.0/6))
                        b = .25;
                    else
                        b = 1.0/6;
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
                        b = b*2;
                }

            switch (intervalType)
            {
                case DateTimeIntervalType.Days:
                    if (viewPeriod.TotalDays <= 2)
                    {
                        b /= 24;
                        if (viewPeriod.TotalHours <= 2)
                        {
                            b /= 60;
                            if (viewPeriod.TotalMinutes <= 2) b /= 60;
                        }
                    }
                    break;
                case DateTimeIntervalType.Hours:
                    if (viewPeriod.TotalDays > 2)
                        b *= 24;
                    else
                    {
                        if (viewPeriod.TotalHours <= 2)
                        {
                            b /= 60;
                            if (viewPeriod.TotalMinutes <= 2) b /= 60;
                        }
                    }
                    break;
                case DateTimeIntervalType.Minutes:
                    if (viewPeriod.TotalHours > 2)
                    {
                        b *= 60;
                        if (viewPeriod.TotalDays > 2) b *= 24;
                    }
                    else if (viewPeriod.TotalMinutes <= 2) b /= 60;
                    break;
                case DateTimeIntervalType.Seconds:
                    if (viewPeriod.TotalMinutes > 2)
                    {
                        b *= 60;
                        if (viewPeriod.TotalHours > 2)
                        {
                            b *= 60;
                            if (viewPeriod.TotalDays > 2) b *= 24;
                        }

                    }

                    break;
            }

            return b;
        }

        private static double GetAxisXInterval(DateTimeIntervalType intervalType, double viewPeriod, byte gridFreq = 1)
        {
            //возвращает длину интервала (AxisX.IntervalCount)
            //в зависимости от типа интервала (intervalType) и отображаемого промежутка (viewPeriod)
            //а также частоты сетки (gridFreq)
            //см. выше

            double temp = viewPeriod;
            temp *= Math.Pow(10, 7);

            switch (intervalType)
            {
                case DateTimeIntervalType.Seconds:
                    break;
                case DateTimeIntervalType.Minutes:
                    temp *= 60;
                    break;
                case DateTimeIntervalType.Hours:
                    temp *= 60*60;
                    break;
                case DateTimeIntervalType.Days:
                    temp *= 60*60*24;
                    break;
            }

            long ns = Convert.ToInt64(temp);
            var ts = new TimeSpan(ns);
            return GetAxisXInterval(intervalType, ts, gridFreq);
        }
        
        internal void CurGraphicChange(int curNum)
        {
            try
            {
                var gr = GetGraphicByNum(curNum);
                if (gr != null)
                {
                    CurGraphicNum = curNum;

                    labCur.BackColor = gr.Series.Color;
                    labCur.Text = CurGraphicNum.ToString();
                    labTimeCur.BackColor = gr.Series.Color;
                    labTimeCur.Text = labCur.Text;
                    labCtrlCur.BackColor = gr.Series.Color;
                    labCtrlCur.Text = labCur.Text;

                    butVizirNextCur.ForeColor = gr.Series.Color;
                    butVizirPrevCur.ForeColor = gr.Series.Color;
                    
                    if (gr.IsAnalog)
                    {
                        tbCurMin.Text = ((AnalogGraphic)gr).GroupY.TxtBoxMin.Text;
                        tbCurMin.Enabled = true;
                        tbCurMax.Text = ((AnalogGraphic)gr).GroupY.TxtBoxMax.Text;
                        tbCurMax.Enabled = true;
                        butCurMinMaxApply.Enabled = true;
                    }
                    else
                    {
                        tbCurMin.Text = "0";
                        tbCurMin.Enabled = false;
                        tbCurMax.Text = "1";
                        tbCurMax.Enabled = false;
                        butCurMinMaxApply.Enabled = false;
                    }

                    butCtrlHide.Text = gr.IsHidden ? "Отобразить график" : "Скрыть график";

                    if ((_dynState == EDynState.NotDyn) && (gr.IsAnalog) && (((AnalogGraphic)gr).GroupY.IsAutoScale))
                    {
                        butCurScaleAuto.UseVisualStyleBackColor = false;
                        butCurScaleAuto.BackColor = SystemColors.Highlight;
                    }
                    else
                    {
                        //butCurScaleAuto.BackColor = Color.White;
                        butCurScaleAuto.UseVisualStyleBackColor = true;
                    }

                    if ((gr.IsAnalog) && (((AnalogGraphic)gr).GroupY.IsInPercent))
                    {
                        butCurInPrc.Text = "В единицы изм.";
                        labCurUnits.Text = "%";
                    }
                    else
                    {
                        butCurInPrc.Text = "В проценты";
                        labCurUnits.Text = gr.Param.Units;
                    }

                    //При выборе графика отрисовка и ось выползают наверх
                    if (gr.IsAnalog)
                    {
                        bool seriesFlag = chartMain.Series.Remove(gr.Series);
                        bool areaFlag = chartMain.ChartAreas.Remove(gr.Area);
                        if (areaFlag) chartMain.ChartAreas.Add(gr.Area);
                        if (seriesFlag) chartMain.Series.Add(gr.Series);
                        //ToTopOverlay(tempGr);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private int _analogAxisMaxHeight;
        private int AnalogAxisMaxHeight
        {
            get
            {
                int h = ((chartMain.Height - AxisXLabelFontHeight - 2*(Nods - 1) - 11 - AxYHeight*(Nods + 1)));
                if (splitContainerV.Panel1.HorizontalScroll.Visible) h -= 20;

                return h;
            }
        }
    #endregion PrivateFunction

    #region DataGrid
        //установка DoubleBuffered для DataGridView
        private static void SetDoubleBuffered(Control control)
        {
            //set instance non-public property with name "DoubleBuffered" to true
            typeof (Control).InvokeMember("DoubleBuffered",
                                          BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                                          null, control, new object[] {true});
        }

        private void DataGridInit()
        {
            //Для того, чтобы датагрид отображался быстрее
            SetDoubleBuffered(dataGridView);

            dataGridView.Columns.Add("№ в таблице", "№ в таблице");
            dataGridView.Columns.Add("Код", "Код");
            dataGridView.Columns.Add("Наименование", "Наименование");
            dataGridView.Columns.Add("Визир", "Визир");
            dataGridView.Columns.Add("Недост.", "Недост.");
            dataGridView.Columns.Add("Последнее", "Последнее");
            dataGridView.Columns.Add("Ед. измер.", "Ед. измер.");
            dataGridView.Columns.Add("Мин.", "Мин.");
            dataGridView.Columns.Add("Макс.", "Макс.");
            dataGridView.Columns.Add("Тип данных", "Тип данных");
            //dataGridView.Columns.Add("Доп. инфо", "Доп. инфо");
            //dataGridView.Columns.Add("А мин", "А мин");
            //dataGridView.Columns.Add("П мин", "П мин");
            //dataGridView.Columns.Add("П макс", "П макс");
            //dataGridView.Columns.Add("А макс", "А макс");
            dataGridView.Columns.Add("Group", "Group");
            var yColumn = new DataGridViewCheckBoxColumn
                              {
                                  HeaderText = @"Y",
                                  Width = 22,
                                  Frozen = true,
                                  AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                                  TrueValue = true,
                                  FalseValue = false
                              };
            dataGridView.Columns.Insert(0, yColumn);
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView.ScrollBars = ScrollBars.Both;
            dataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.GradientActiveCaption;
            dataGridView.Columns["№ в таблице"].Width = 22;
            dataGridView.Columns["№ в таблице"].Frozen = true;
            dataGridView.Columns["№ в таблице"].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F,
                                                                                 FontStyle.Bold,
                                                                                 GraphicsUnit.Point, ((204)));
            dataGridView.Columns["№ в таблице"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["№ в таблице"].ReadOnly = true;
            dataGridView.Columns["Код"].Width = 130;
            dataGridView.Columns["Код"].ReadOnly = true;
            dataGridView.Columns["Наименование"].Width = 310;
            dataGridView.Columns["Наименование"].ReadOnly = true;
            dataGridView.Columns["Ед. измер."].Width = 80;
            dataGridView.Columns["Ед. измер."].ReadOnly = true;
            dataGridView.Columns["Визир"].Width = 70;
            dataGridView.Columns["Визир"].ReadOnly = true;
            dataGridView.Columns["Недост."].Width = 50;
            dataGridView.Columns["Недост."].ReadOnly = true;
            dataGridView.Columns["Последнее"].Width = 70;
            dataGridView.Columns["Последнее"].ReadOnly = true;
            dataGridView.Columns["Мин."].Width = 50;
            dataGridView.Columns["Мин."].ReadOnly = true;
            dataGridView.Columns["Макс."].Width = 50;
            dataGridView.Columns["Макс."].ReadOnly = true;
            dataGridView.Columns["Тип данных"].Width = 80;
            dataGridView.Columns["Тип данных"].ReadOnly = true;
            //dataGridView.Columns["Доп. инфо"].Width = 95;
            //dataGridView.Columns["Доп. инфо"].ReadOnly = true;
            //dataGridView.Columns["А мин"].Width = 49;
            //dataGridView.Columns["П мин"].Width = 49;
            //dataGridView.Columns["П макс"].Width = 50;
            //dataGridView.Columns["А макс"].Width = 49;
            //dataGridView.Columns["А мин"].ReadOnly = true;
            //dataGridView.Columns["П мин"].ReadOnly = true;
            //dataGridView.Columns["П макс"].ReadOnly = true;
            //dataGridView.Columns["А макс"].ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.Columns["№ в таблице"].DefaultCellStyle.ForeColor = Color.White;
            dataGridView.Columns["№ в таблице"].DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView.Columns["Group"].Visible = false;
            dataGridView.MultiSelect = false;
            dataGridView.AllowUserToAddRows = false;

            //~ dataGridView.CellClick += HideGraph;
            //~ dataGridView.CellValueChanged += CheckChangeDatagrid;
            //~ dataGridView.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            //~ dataGridView.SelectionChanged += DatagridSelectionChanged;
            //~ dataGridView.UserDeletingRow += DatagridRowAnnihilate;
            dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;
        }

        //Заполнение таблицы инфой
        private void DataGridAddRow(Graphic graphic)
        {
            int row;

            if (IsDynamic)
                row = dataGridView.Rows.Add(new IComparable[]
                                                {
                                                    true, graphic.Num.ToString(), graphic.Param.Code,
                                                    graphic.Param.SubName == ""
                                                        ? graphic.Param.Name
                                                        : graphic.Param.Name + ". " + graphic.Param.SubName,
                                                    "", "", graphic.Param.Units, "",
                                                    graphic.Param.Min.ToString(), graphic.Param.Max.ToString(),
                                                    graphic.DataTypeString
                                                });
            else
                row = dataGridView.Rows.Add(new IComparable[]
                                                {
                                                    true, graphic.Num.ToString(), graphic.Param.Code,
                                                    graphic.Param.SubName == ""
                                                        ? graphic.Param.Name
                                                        : graphic.Param.Name + ". " + graphic.Param.SubName,
                                                    "", "", graphic.Param.Units,
                                                    graphic.Param.Min.ToString(), graphic.Param.Max.ToString(),
                                                    graphic.DataTypeString
                                                });

            //dataGridView["№ в таблице", t.Index - 1].Style.BackColor = t.Series.Color;
            //dataGridView["№ в таблице", t.Index - 1].Style.SelectionBackColor = t.Series.Color;
            //dataGridView[0, t.Index - 1].Style.BackColor = t.Series.Color;
            //dataGridView[0, t.Index - 1].Style.SelectionBackColor = t.Series.Color;
            dataGridView.Rows[row].Cells["№ в таблице"].Style.BackColor = graphic.Series.Color;
            dataGridView.Rows[row].Cells["№ в таблице"].Style.SelectionBackColor = graphic.Series.Color;
            dataGridView.Rows[row].Cells[0].Style.BackColor = graphic.Series.Color;
            dataGridView.Rows[row].Cells[0].Style.SelectionBackColor = graphic.Series.Color;
            //dataGridView["Group", t.Index - 1].Value = t.Index;
            dataGridView.Rows[row].Cells["Group"].Value = graphic.Num;

            if (graphic.IsDiscret)
            {
                ((DataGridViewCheckBoxCell) dataGridView.Rows[row].Cells[0]).ThreeState = true;
                //((DataGridViewCheckBoxCell) dataGridView.Rows[t.Index - 1].Cells[0]).IndeterminateValue = 3;
                dataGridView.Rows[row].Cells[0].Value = 2;
                dataGridView.Rows[row].Cells[0].ReadOnly = true;
            }
        }

        //Вычисляет выделенный в таблице параметр и записывает в CurrentParamNumber
        internal void DataGridSelectionChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    //проверку на нулл сделать, ну нулевой ячейки
            //    if (dataGridView1.Rows.Count >= 1)
            //    {
            //        CurrentParamNumber = int.Parse(dataGridView1.CurrentRow.Cells["№ в таблице"].Value.ToString());
            //        GraphicParam tempGr = GetParam(CurrentParamNumber);
            //        label9.BackColor = tempGr.Series.Color;
            //        label9.Text = CurrentParamNumber.ToString();
            //        label13.BackColor = tempGr.Series.Color;
            //        button1.ForeColor = tempGr.Series.Color;
            //        button5.ForeColor = tempGr.Series.Color;
            //        label19.BackColor = tempGr.Series.Color;
            //        label13.Text = label9.Text;
            //        label19.Text = label9.Text;
            //        //if (dataGridView1.CurrentCell != dataGridView1.CurrentRow.Cells[0])
            //        //{
            //        if (tempGr.DataTypeD == DataType.Real)
            //        {
            //            textBox1.Text = tempGr.AxY.TBoxMin.Text;
            //            textBox2.Text = tempGr.AxY.TBoxMax.Text;
            //            button4.Enabled = true;
            //            //radioButton6.Enabled = true;
            //            //radioButton7.Enabled = true;
            //            textBox1.Enabled = true;
            //            textBox2.Enabled = true;
            //            //button8.Enabled = true;
            //            //button9.Enabled = true;
            //        }
            //        else
            //        {
            //            textBox1.Text = "0";
            //            textBox2.Text = "2";
            //            button4.Enabled = false;
            //            //radioButton6.Enabled = false;
            //            //radioButton7.Enabled = false;
            //            textBox1.Enabled = false;
            //            textBox2.Enabled = false;
            //            //button8.Enabled = false;
            //            //button9.Enabled = false;
            //        }
            //        //}
            //        //dataGridView1.CurrentRow.Selected = true;

            //        button10.Text = tempGr.IsVisible ? "Скрыть график" : "Отобразить график";

            //        //IsYScaledAuto = tempGr.IsYScaledAuto;
            //        if (tempGr.IsYScaledAuto && TimerMode == TimerModes.Analyzer)
            //        {
            //            button23.UseVisualStyleBackColor = false;
            //            button23.BackColor = SystemColors.Highlight;
            //        }
            //        else
            //        {
            //            //button23.BackColor = Color.White;
            //            button23.UseVisualStyleBackColor = true;
            //        }

            //        if (tempGr.PercentMode == PercentModeClass.Percentage)
            //        {
            //            button24.Text = "В единицы изм.";
            //            label15.Text = "%";
            //        }
            //        else
            //        {
            //            button24.Text = "В проценты";
            //            label15.Text = dataGridView1.CurrentRow.Cells["Ед. измер."].Value.ToString();
            //        }

            //        //При выборе графика отрисовка и ось выползают наверх
            //        if (tempGr.DataTypeD != DataType.Boolean)
            //        {
            //            bool seriesFlag = chart1.Series.Remove(tempGr.Series);
            //            bool areaFlag = chart1.ChartAreas.Remove(tempGr.Area);
            //            if (areaFlag) chart1.ChartAreas.Add(tempGr.Area);
            //            if (seriesFlag) chart1.Series.Add(tempGr.Series);
            //            //ToTopOverlay(tempGr);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    bool flag = dataGridView1.Rows.Count == 0;
            //    if (!flag)
            //    {
            //        Error = new ErrorCommand("Проблема со вводом в таблицу (datagridSelectionChanged)", ex);
            //        //MessageBox.Show(ex.StackTrace);
            //        throw;
            //    }
            //}
        }

        //Нахождение строки датагрида по номеру графика
        private DataGridViewRow GetRowByGraphicNum(int num)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
                if (row.Cells[1].Value.ToString() == num.ToString())
                    return row;
            return null;
        }
    #endregion DataGrid

    #region Отрисовка
        //Установка на место осей Y
        private void AxesRanking(GroupY groupY, int analogCounter)
        {
            groupY.Ax.Location = new Point((analogCounter - 1)*AxWidth, 0);

            int h = AnalogAxisMaxHeight;
            groupY.Ax.Height = (int)(h * groupY.CurAxSize);
            groupY.Ax.Top = (int) (h*groupY.CurAxPos);
            if(groupY.Ax.Bottom > h)
            {
                int d = groupY.Ax.Top - groupY.Ax.Bottom + h;
                groupY.Ax.Top = d > 0 ? d : 0;
            }
        }

        private void AxCapRanking(DiscretGraphic dGr)
        {
            dGr.AxCap.Left = (int) (dGr.Area.Position.X/100*chartMain.Width) - AxWidth;
            //dGr.AxCap.Location = new Point(dGr.AxCap.Location.X, (int)(dGr.Area.Position.Y * chartMain.Height / 100));
            dGr.AxCap.Top = (int) (dGr.Area.Position.Y*chartMain.Height/100);
        }

        //Установка на место графиков: аналоговых ширину и позицию по Х, дискретных ширину и позицию по Х
        private void AreaRankingX(ChartArea area)
        {
            if (Noas > 0)
            {
                area.Position.Width = 100 - 100*((Noas)*(float) AxWidth)/chartMain.Width;
                area.Position.X = 100*(Noas*(float) AxWidth)/chartMain.Width;
            }
            else
            {
                area.Position.Width = 100 - 100*(float) AxWidth/chartMain.Width;
                area.Position.X = 100*(float) AxWidth/chartMain.Width;
            }
        }

        private void AnalogAreaRankingHeight(AnalogGraphic aGr)
        {
            //aGr.Area.Position.Height = (float) ((aGr.GroupY.Ax.Size.Height - AxSummand - 5)*100/.976/chartMain.Height);
            float h2 = (aGr.GroupY.Ax.Size.Height - AxSummand - GroupY.AxCapHeight + 20) * 100f / chartMain.Height;
            aGr.Area.Position.Height = h2;
        }

        private void AnalogAreaRankingY(AnalogGraphic aGr)
        {
            aGr.Area.Position.Y = 100f*(aGr.GroupY.Ax.Top + AxSummand)/chartMain.Height;
        }

        private void DiscretAreaRankingHeight(ChartArea area)
        {
            area.Position.Height = ((float) 100*(AxYLabel + AxYHeight))/chartMain.Height;
            //MessageBox.Show(AxYLabel + "   " + AxYHeight + "\nChartH: " + chartMain.Height + "\nH: " + area.Position.Height +
            //    "\nY: " + area.Position.Y);
        }

        private void DiscretAreaRankingY(ChartArea area, int discretCounter)
        {
            area.Position.Y = 100*(1 - ((discretCounter + 1)*AxYHeight + 2*(discretCounter - 1) + 10f)/chartMain.Height);
        }

        private void DiscretAreaBgrHeight() //AreaDisFirstHeight
        {
            _discretBackGround.Area.Position.Height = ((float) 100*(AxYLabel + AxYHeight*Nods + 2*(Nods - 1)))/
                                                      chartMain.Height;
            _discretBackGround.Area.Position.Y = 100*(1 - (AxYHeight*(Nods + 1) + 2*(Nods - 1) + 10f)/chartMain.Height);
        }

        private void AnalogAreaBgrHeight() //AreaDisFirstHeight
        {
            const int y = 0;
            _analogBackGround.Area.Position.Y = 100f*(y + AxSummand)/chartMain.Height;

            //float h1 = (float) ((h - AxSummand - 5)*100/.976/chartMain.Height);
            //_analogBackGround.Area.Position.Height = h1;
            int h = AnalogAxisMaxHeight;
            if (h < GroupsY.First().Ax.MinimumSize.Height) h = GroupsY.First().Ax.MinimumSize.Height;
            float h2 = (h - AxSummand - GroupY.AxCapHeight + 20)*100f/chartMain.Height;
            _analogBackGround.Area.Position.Height = h2;

            //MessageBox.Show(h1 + "\n" + h2);
        }

        private void DownerScrollRankingSize()
        {
            var w = (BackGround != null) ? BackGround.Area.Position.Width : 100;
            _hscrollb.Width = Convert.ToInt32(w*chartMain.Size.Width/100);
            _buttonScaleDrop.Top = splitContainerV.Panel1.Size.Height - 20;
            _buttonScaleDrop.Width = splitContainerV.Panel1.Size.Width - _hscrollb.Width;
            _hscrollb.Location = new Point(_buttonScaleDrop.Width, splitContainerV.Panel1.Size.Height - 20);
        }

        //Установка на место графиков: аналоговых высоту и позицию по Y, дискретных позицию по Y
        private void ReRanking()
        {
            if (AnalogGraphics.Count > 0)
            {
                AreaRankingX(_analogBackGround.Area);
                AnalogAreaBgrHeight();
            }

            int i = 0;
            foreach (var grY in GroupsY)
            {
                i++;
                if (!grY.IsHidden) AxesRanking(grY, i);

                foreach (var aGr in grY.Graphics)
                {
                    AreaRankingX(aGr.Area);
                    AnalogAreaRankingHeight(aGr);
                    AnalogAreaRankingY(aGr);
                }
            }

            if (DiscretGraphics.Count > 0)
            {
                AreaRankingX(_discretBackGround.Area);
                DiscretAreaBgrHeight();
            }

            i = 0;
            foreach (var dGr in DiscretGraphics)
            {
                if (!dGr.IsHidden)
                {
                    i++;
                    AreaRankingX(dGr.Area);
                    DiscretAreaRankingHeight(dGr.Area);
                    DiscretAreaRankingY(dGr.Area, i);
                    AxCapRanking(dGr);
                }
            }

            DownerScrollRankingSize();
        }
    #endregion Отрисовка

    #region ControlFunction
        private DateTimeIntervalType IntervalTypeFromCbViewPeriodType
        {
            get
            {
                switch (cbViewPeriodType.Text)
                {
                    case "Сек.":
                        return DateTimeIntervalType.Seconds;
                    case "Мин.":
                        return DateTimeIntervalType.Minutes;
                    case "Час.":
                        return DateTimeIntervalType.Hours;
                    case "Сут.":
                        return DateTimeIntervalType.Days;
                }
                return DateTimeIntervalType.NotSet;
            }
        }

        private void SetCbViewPeriodType(DateTimeIntervalType intervalType, double interval)
        {
            switch (intervalType)
            {
                case DateTimeIntervalType.Seconds:
                    cbViewPeriodType.Text = "Сек.";
                    break;
                case DateTimeIntervalType.Minutes:
                    cbViewPeriodType.Text = "Мин.";
                    break;
                case DateTimeIntervalType.Hours:
                    cbViewPeriodType.Text = "Час.";
                    break;
                case DateTimeIntervalType.Days:
                    cbViewPeriodType.Text = "Сут.";
                    break;
            }

            cbViewPeriod.Text = interval.ToString();
        }

        private void SetCbViewPeriodType(DateTimeIntervalType intervalType, TimeSpan interval)
        {
            switch (intervalType)
            {
                case DateTimeIntervalType.Seconds:
                    cbViewPeriodType.Text = "Сек.";
                    cbViewPeriod.Text = interval.TotalSeconds.ToString();
                    break;
                case DateTimeIntervalType.Minutes:
                    cbViewPeriodType.Text = "Мин.";
                    cbViewPeriod.Text = interval.TotalMinutes.ToString();
                    break;
                case DateTimeIntervalType.Hours:
                    cbViewPeriodType.Text = "Час.";
                    cbViewPeriod.Text = interval.TotalHours.ToString();
                    break;
                case DateTimeIntervalType.Days:
                    cbViewPeriodType.Text = "Сут.";
                    cbViewPeriod.Text = interval.TotalDays.ToString();
                    break;
            }
        }

        //Проверка корректности ввода
        //private void TextBoxInputReal(object sender, KeyPressEventArgs e)
        //{
        //    bool separatorFlag = false;
        //    var textBox = (ComboBox)sender;
        //    //var textBox = (TextBox)sender;
        //    if (char.IsDigit(e.KeyChar)) return;
        //    if (e.KeyChar == (char)Keys.Back) return;
        //    if (textBox.Text.IndexOf(_separator) != -1) separatorFlag = true;
        //    if (separatorFlag)
        //    {
        //        e.Handled = true;
        //        return;
        //    }
        //    if (e.KeyChar.ToString() == _separator) return;
        //    e.Handled = true;
        //}

        //private void TextBoxInputAnyReal(object sender, KeyPressEventArgs e)
        //{
        //    bool separatorFlag = false;
        //    var textBox = (TextBox)sender;
        //    if (char.IsDigit(e.KeyChar)) return;
        //    if (e.KeyChar == (char)Keys.Back) return;
        //    if (e.KeyChar.ToString() == "-")
        //    {
        //        if (textBox.Text != "")
        //            if (textBox.Text.IndexOf("-") != -1)
        //            {
        //                if (textBox.SelectedText.IndexOf("-") == -1) e.Handled = true;
        //            }
        //            else
        //            {
        //                if (textBox.SelectionStart != 0) e.Handled = true;
        //            }
        //        return;
        //    }
        //    if ((textBox.Text.IndexOf(_separator) != -1) && textBox.SelectedText.IndexOf(_separator) == -1)
        //        separatorFlag = true;
        //    if (separatorFlag)
        //    {
        //        e.Handled = true;
        //        return;
        //    }
        //    if (e.KeyChar.ToString() == _separator) return;
        //    e.Handled = true;
        //}
    #endregion ControlFunction

    #region ControlFunction AxisY
        //делаем текущим дискретный сигнал по выделению AxCap
        internal void AxCapClick(object sender, EventArgs e)
        {
            try
            {
                CurGraphicNum = int.Parse(((Label)sender).Text);
                dataGridView.CurrentCell = GetRowByGraphicNum(CurGraphicNum).Cells[1];
                DataGridSelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Resize оси Y
        internal void AxResize(object sender, EventArgs e)
        {
            try
            {
                var ax = ((Panel) sender);
                var gr = (AnalogGraphic) GetGraphicByNum(int.Parse(ax.Name));
                for (int j = 0; j < 4; j++)
                {
                    gr.GroupY.Labels[j].Top = (j + 1)*
                                              (ax.Height - gr.GroupY.AxCap.Height - gr.GroupY.RulePanel.Height - 2)/5 -
                                              10 + gr.GroupY.RulePanel.Height + 2;
                }

                gr.GroupY.ResizeAreaButtonTop.Top = gr.GroupY.TxtBoxMax.Bottom;
                gr.GroupY.ResizeAreaButtonBottom.Top = gr.GroupY.TxtBoxMin.Top - 3;
                //curP.TBoxMax.Top = curP.ResizeButtonTop.Bottom;
                //AreaAnRankingHeight(curP);
                if (!_isMouseDown4Expand)
                    foreach (var grOv in gr.GroupY.Graphics) AnalogAreaRankingHeight(grOv);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
    #endregion ControlFunction AxisY

    #region ControlFunction Form
        //Блокировка отображения формы, если не задан интервал времени
        private void Form_BlockAppear(object sender, EventArgs e)
        {
            if (TimeBegin.Year < 1900 || TimeEnd.Year < 1900)
            {
                Dispose();
                MessageBox.Show("Не задан интервал отображения", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void From_Resize(object sender, EventArgs e)
        {
            if (splitContainerH.Width - splitContainerH.SplitterWidth -
                tabControl.Width > 0)
                splitContainerH.SplitterDistance = splitContainerH.Width - splitContainerH.SplitterWidth -
                                                   tabControl.Width;
            labRightPanel_Move(sender, e);
        }

        private void chartMain_Resize(object sender, EventArgs e)
        {
            try
            {
                //~ AxYLabel = chartMain.Height * 9 / 303 + 8;
                ReRanking();
            }
            catch (Exception exception)
            {
                //~ Error = new ErrorCommand("При увеличении масштаба в графике возникает конфликт с визиром", exception);
                //MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }
        
        private void labBottomPanel_Click(object sender, EventArgs e)
        {
            _panelBottomHide = !_panelBottomHide;
            splitContainerV.Panel2Collapsed = _panelBottomHide;
            labBottomPanel_Move(sender, e);
            //label6.Text = label6.Text == @"          V         " ? "          ^         " : "          V         ";
            labBottomPanel.Image = _panelBottomHide ? Properties.Resources.upl : Properties.Resources.downl;
        }

        //Приклеиваем кнопку скрывания датагрида
        private void labBottomPanel_Move(object sender, EventArgs e)
        {
            labBottomPanel.Location = new Point(labBottomPanel.Location.X, splitContainerV.Panel1.Height + 5);
        }
        
        private void labRightPanel_Click(object sender, EventArgs e)
        {
            _panelRightHide = !_panelRightHide;
            splitContainerH.Panel2Collapsed = _panelRightHide;
            labRightPanel_Move(sender, e);
            //label1.Text = label1.Text == "\n\n<\n\n\n" ? "\n\n>\n\n\n" : "\n\n<\n\n\n";
            labRightPanel.Image = _panelRightHide ? Properties.Resources.leftl : Properties.Resources.rightl;
        }

        //Приклеиваем кнопку скрывания меню
        private void labRightPanel_Move(object sender, EventArgs e)
        {
            labRightPanel.Location = new Point(splitContainerH.Panel1.Width + 5, labRightPanel.Location.Y);
            //labRightPanel.Top = splitContainerH.Panel1.Width + 5;
        }

        //изменение цвета кнопок, скрывающих панели
        private void labPanel_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.White;
        }

        private void labPanel_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = SystemColors.GradientInactiveCaption; //Color.LightSeaGreen;
        }
        
        private void panelChart_Resize(object sender, EventArgs e)
        {
            panelChart.Height = splitContainerV.Panel1.Height - 20;
        }

        private void splitContainerV_SplitterMoved(object sender, SplitterEventArgs e)
        {
            labBottomPanel_Move(sender, e);
        }

        //Сборка мусора по команде
        private void GarbageCollection(object sender, EventArgs e)
        {
            Gethering();
        }

        //Всплывающая подсказка над кнопками
        private void ButtonsHover(object sender, EventArgs e)
        {
            string tempS = "";
            switch (((Button)sender).Name)
            {
                case "butLineWidthAll":
                case "butLineWidthCur":
                case "butViewPeriodApply":
                case "butCurMinMaxApply":
                    tempS = "Применить";
                    break;
                case "butViewPeriodToMax":
                    tempS = "Максимальный период отображения";
                    break;
                case "butViewPeriodToMin":
                    tempS = "Минимальный период отображения";
                    break;
                case "butViewPeriodDec":
                    tempS = "Уменьшить период отображения";
                    break;
                case "butViewPeriodInc":
                    tempS = "Увеличить период отображения";
                    break;
                case "butVizirPrevAll":
                    tempS = "Установить визир в предыдущую точку по всем графикам";
                    break;
                case "butVizirNextAll":
                    tempS = "Установить визир в следующую точку по всем графикам";
                    break;
                case "butVizirPrevCur":
                    tempS = "Установить визир в предыдущую точку по текущему графику";
                    break;
                case "butVizirNextCur":
                    tempS = "Установить визир в следующую точку по текущему графику";
                    break;
            }
            toolTip.SetToolTip((Button)sender, tempS);
        }
    #endregion ControlFunction Form

    #region ControlFunction Time
        private void butViewPeriodApply_Click(object sender, EventArgs e)
        {
            try
            {
                double intv = double.Parse(cbViewPeriod.Text);
                DateTimeIntervalType intvType = IntervalTypeFromCbViewPeriodType;

                DateTime timeViewEnd = _timeViewEnd;
                switch (intvType)
                {
                    case DateTimeIntervalType.Seconds:
                        timeViewEnd = _timeViewBegin.AddSeconds(intv);
                        break;
                    case DateTimeIntervalType.Minutes:
                        timeViewEnd = _timeViewBegin.AddMinutes(intv);
                        break;
                    case DateTimeIntervalType.Hours:
                        timeViewEnd = _timeViewBegin.AddHours(intv);
                        break;
                    case DateTimeIntervalType.Days:
                        timeViewEnd = _timeViewBegin.AddDays(intv);
                        break;
                }

                DateTime timeViewBegin = _timeViewBegin;
                if (timeViewEnd > TimeEnd)
                {
                    timeViewBegin = timeViewBegin.Add(TimeEnd.Subtract(timeViewEnd));
                    timeViewEnd = TimeEnd;
                }

                ChangeScaleView(intvType, timeViewBegin, timeViewEnd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void butViewPeriodToMin_Click(object sender, EventArgs e)
        {
            ChangeScaleViewAuto(_timeViewBegin, _timeViewBegin.AddSeconds(_scaleXMin));
        }

        private void butViewPeriodToMax_Click(object sender, EventArgs e)
        {
            ChangeScaleViewAuto(TimeBegin, TimeEnd);
        }

        private void butViewPeriodInc_Click(object sender, EventArgs e)
        {
            double intv = double.Parse(cbViewPeriod.Text);
            double pcnt = double.Parse(cbViewPeriodStep.Text.Substring(0, cbViewPeriodStep.Text.Length - 1))/100;
            double newIntv = intv*(1 + pcnt);

            //DateTime timeE = TimeEnd;

            //switch (IntervalTypeFromCbViewPeriodType)
            //{
            //    case DateTimeIntervalType.Seconds:
            //        timeE = TimeViewBegin.AddSeconds(newIntv);
            //        break;
            //    case DateTimeIntervalType.Minutes:
            //        timeE = TimeViewBegin.AddMinutes(newIntv);
            //        break;
            //    case DateTimeIntervalType.Hours:
            //        timeE = TimeViewBegin.AddHours(newIntv);
            //        break;
            //    case DateTimeIntervalType.Days:
            //        timeE = TimeViewBegin.AddDays(newIntv);
            //        break;
            //}

            //ChangeScaleView(IntervalTypeFromCbViewPeriodType, _timeViewBegin, timeE);

            cbViewPeriod.Text = newIntv.ToString();
            butViewPeriodApply_Click(null, null);
        }

        private void butViewPeriodDec_Click(object sender, EventArgs e)
        {
            double intv = double.Parse(cbViewPeriod.Text);
            double pcnt = double.Parse(cbViewPeriodStep.Text.Substring(0, cbViewPeriodStep.Text.Length - 1))/100;
            double newIntv = intv*(1 - pcnt);

            cbViewPeriod.Text = newIntv.ToString();
            butViewPeriodApply_Click(null, null);
        }

        //Сделать возвращение к исходным параметрам при ненажатии кнопки применить
    #endregion ControlFunction Time

    #region ControlFunction Value
        private void butCurScaleByScale_Click(object sender, EventArgs e)
        {
            //
        }

        private void butCurScaleAuto_Click(object sender, EventArgs e)
        {
            //CurGraphicNum = 1;

            //var gr = GetGraphicByNum(CurGraphicNum);
            //if ((gr != null) && (gr.IsAnalog))
            //{
            //    AxYToAutoScale((AnalogGraphic) gr);

            //    butCurScaleAuto.UseVisualStyleBackColor = false;
            //    butCurScaleAuto.BackColor = SystemColors.Highlight;

            //    butCurMinMaxApply.Enabled = true;
            //    tbCurMin.Enabled = true;
            //    tbCurMax.Enabled = true;
            //}
        }

        private void butPositionCascade_Click(object sender, EventArgs e)
        {
            if (Noas > 1)
            {
                AnalogGraphic firstAGr = AnalogGraphics.First();

                int h = AnalogAxisMaxHeight;
                int h1 = (h - 2*AxSummand)/Noas + 2*AxSummand;

                if (h1 < firstAGr.GroupY.Ax.MinimumSize.Height)
                    h1 = firstAGr.GroupY.Ax.MinimumSize.Height;

                var d = (h - h1)/(Noas - 1);
                if (d < 0) d = 0;

                if (h1 > h) h1 = h;

                int current = 0;
                foreach (var axY in GroupsY)
                {
                    if (!axY.IsHidden) //|| (!axY.UpperGraphic.IsHidden)
                    {
                        axY.CurAxPos = (double) (d*current)/h;
                        axY.CurAxSize = (current < (Noas - 1)) ? (double) h1/h : (double) (h - d*current)/h;

                        current++;
                        AxesRanking(axY, current);

                        foreach (var gr in axY.Graphics)
                        {
                            AnalogAreaRankingY(gr);
                            //AnalogAreaRankingHeight(gr);
                        }
                    }
                }
            }
        }

        private void butPositionOverlap_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (var axY in GroupsY)
            {
                axY.CurAxSize = 1;
                axY.CurAxPos = 0;
                AxesRanking(axY, ++i);
            }

            foreach (var aGr in AnalogGraphics)
            {
                AnalogAreaRankingY(aGr);
                //AnalogAreaRankingHeight(aGr);
            }
        }
    #endregion ControlFunction Value

    #region ControlFunction Control
        private void butLineWidthCur_Click(object sender, EventArgs e)
        {
            var gr = GetGraphicByNum(CurGraphicNum);
            if (gr != null)
            {
                gr.Series.BorderWidth = LineWidth;
                gr.Series.EmptyPointStyle.BorderWidth = gr.Series.BorderWidth;
            }
        }

        private void butLineWidthAll_Click(object sender, EventArgs e)
        {
            foreach (var gr in Graphics)
            {
                gr.Series.BorderWidth = LineWidth;
                gr.Series.EmptyPointStyle.BorderWidth = gr.Series.BorderWidth;
            }
        }

        private void cbLineWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) butLineWidthCur_Click(sender, e);
        }
    #endregion ControlFunction Control
        
    #region ControlFunction Cursor
        //При установке визира всплывает подсказка относительно текущей точки текущего графика
        //private void CursorPositionView(object sender, CursorEventArgs e)
        //{
        //    try
        //    {
        //        Graphic gr = GetGraphicByNum(CurGraphicNum);

        //        var axX = BackGround;

        //        tbVizirTime.Text =
        //            DateTime.FromOADate(axX.Area.CursorX.Position).TimeOfDay.ToString("hh':'mm':'ss','fff");
        //        tbVizirDate.Text = (DateTime.FromOADate(axX.Area.CursorX.Position)).Date.ToString("dd'.'MM'.'yyyy");

        //        string decPlacesTemplate = gr.IsDiscret ? "0" : ((AnalogGraphic)gr).GroupY.DecPlacesMask;

        //        string st;
        //        if (gr.IsAnalog)
        //        {
        //            MomentReal val = ((AnalogGraphic)gr).DotAt(e.ChartArea.CursorX.Position);
        //            st = val.Mean.ToString(decPlacesTemplate) + "\n" +
        //                 gr.Param.ValueToPercent(val.Mean).ToString(decPlacesTemplate);
        //        }
        //        else
        //        {
        //            MomentBoolean val = ((DiscretGraphic)gr).DotAt(e.ChartArea.CursorX.Position);
        //            //st = val.Mean.ToString() + "\n" +
        //            //     gr.Param.ValueToPercent(Convert.ToDouble(val.Mean)).ToString(decPlacesTemplate);
        //            st = val.Mean.ToString();
        //        }

        //        string tempStr = string.Format("График №" + CurGraphicNum + "\n" + st +
        //                                       "%\n{0:dd.MM.yyyy}\n{1:hh':'mm':'ss','fff}",
        //                                       (DateTime.FromOADate(e.ChartArea.CursorX.Position)).Date,
        //                                       (DateTime.FromOADate(e.ChartArea.CursorX.Position)).TimeOfDay);
        //        toolTip.SetToolTip(chartMain, tempStr);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Error = new ErrorCommand("При увеличении масштаба в графике возникает конфликт с визиром", ex);
        //        //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
        //    }
        //}

        //При установке визира записывает инфу в датагрид и в строку Время визира
        //private void CursorPositionGridView(object sender, CursorEventArgs e)
        //{
        //    try
        //    {
        //        GraphicVisual backParam;
        //        switch (e.ChartArea.Name)
        //        {
        //            case "ChartAreaAnalogBG":
        //                backParam = _analogBackGround;
        //                break;
        //            case "ChartAreaDiscretBG":
        //                backParam = _discretBackGround;
        //                break;
        //        }

        //        foreach (DataGridViewRow row in dataGridView.Rows)
        //        {
        //            int num = int.Parse(row.Cells[1].Value.ToString());
        //            Graphic gr = GetGraphicByNum(num);

        //            if (gr.IsAnalog)
        //            {
        //                MomentReal val = ((AnalogGraphic) gr).DotAt(e.ChartArea.CursorX.Position);
        //                if (val != null)
        //                {
        //                    string decPlacesMask = ((AnalogGraphic) gr).GroupY.DecPlacesMask;

        //                    row.Cells["Визир"].Value = !((AnalogGraphic) gr).GroupY.IsInPercent
        //                                                   ? val.Mean.ToString(decPlacesMask)
        //                                                   : gr.Param.PercentToValue(val.Mean).ToString(decPlacesMask);
        //                    row.Cells["Недост."].Value = val.Nd.ToString();
        //                }
        //                else
        //                {
        //                    row.Cells["Визир"].Value = null;
        //                    row.Cells["Недост."].Value = null;
        //                }
        //            }
        //            else
        //            {
        //                MomentBoolean val = ((DiscretGraphic) gr).DotAt(e.ChartArea.CursorX.Position);
        //                if (val != null)
        //                {
        //                    row.Cells["Визир"].Value = val.Mean.ToString();
        //                    row.Cells["Недост."].Value = val.Nd.ToString();
        //                }
        //                else
        //                {
        //                    row.Cells["Визир"].Value = null;
        //                    row.Cells["Недост."].Value = null;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //Error = new ErrorCommand("При попадании курсора вне области построения всплывающее окно не появляется", ex);
        //        for (int index = 0; index < dataGridView.Rows.Count; index++)
        //        {
        //            //var row = dataGridView1.Rows[index];
        //            var row = WantedRow(index + 1);
        //            row.Cells["Визир"].Value = "";
        //            //row.Cells["Время визира"].Value = "";
        //        }
        //    }
        //}

        //Синхронизация визиров
        //private void CursorSynch(object sender, CursorEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Axis.AxisName == AxisName.X)
        //        {
        //            switch (e.ChartArea.Name)
        //            {
        //                case "ChartAreaAnalogBG":
        //                    //чтобы не моргал визир
        //                    if (Math.Abs(_discretBackGround.Area.CursorX.SelectionEnd - _discretBackGround.Area.CursorX.SelectionStart)
        //                        < .03 * (e.ChartArea.AxisX.ScaleView.ViewMaximum - e.ChartArea.AxisX.ScaleView.ViewMinimum))
        //                    {
        //                        _discretBackGround.Area.CursorX.SelectionStart = double.NaN;
        //                        //ParamsDiscrete.First().Area.CursorX.SelectionEnd = double.NaN;
        //                    }

        //                    _discretBackGround.Area.CursorX.SelectionStart =
        //                        _analogBackGround.Area.CursorX.SelectionStart;
        //                    _discretBackGround.Area.CursorX.SelectionEnd =
        //                        _analogBackGround.Area.CursorX.SelectionEnd;
        //                    break;
        //                case "ChartAreaDiscretBG":
        //                    if (Math.Abs(_analogBackGround.Area.CursorX.SelectionEnd - _analogBackGround.Area.CursorX.SelectionStart)
        //                        < .03 * (e.ChartArea.AxisX.ScaleView.ViewMaximum - e.ChartArea.AxisX.ScaleView.ViewMinimum))
        //                    {
        //                        _analogBackGround.Area.CursorX.SelectionStart = double.NaN;
        //                        //ParamsAnalog[0].Area.CursorX.SelectionEnd = double.NaN;
        //                    }

        //                    _analogBackGround.Area.CursorX.SelectionStart =
        //                        _discretBackGround.Area.CursorX.SelectionStart;
        //                    _analogBackGround.Area.CursorX.SelectionEnd = _discretBackGround.Area.CursorX.SelectionEnd;
        //                    break;
        //            }

        //            _discretBackGround.Area.CursorX.Position = e.NewPosition;
        //            _analogBackGround.Area.CursorX.Position = e.NewPosition;
        //            //ParamsAnalog[0].Area.CursorX.SelectionStart = e.ChartArea.CursorX.SelectionStart;
        //            //ParamsAnalog[0].Area.CursorX.SelectionEnd = e.ChartArea.CursorX.SelectionEnd;
        //            //ParamsDiscrete[0].Area.CursorX.SelectionStart = e.ChartArea.CursorX.SelectionStart;
        //            //ParamsDiscrete[0].Area.CursorX.SelectionEnd = e.ChartArea.CursorX.SelectionEnd;
        //        }
        //        //else
        //        //{
        //        //    _curPos = Math.Min(ParamsAnalog[0].Area.CursorY.SelectionEnd, ParamsAnalog[0].Area.CursorY.SelectionStart)
        //        //                / ParamsAnalog[0].Area.AxisY.ScaleView.Size;
        //        //    _curSize = ParamsAnalog[0].Area.CursorY.Interval / ParamsAnalog[0].Area.AxisY.ScaleView.Size;
        //        //    Text = ParamsAnalog[0].Area.AxisY.Interval.ToString();
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
        //    }
        //}

        //    private void CursorSynchFinal(object sender, CursorEventArgs e)
        //    {
        //        try
        //        {
        //            if (e.Axis.AxisName == AxisName.X)
        //            {
        //                _discretBackGround.Area.CursorX.Position = e.NewPosition;
        //                _analogBackGround.Area.CursorX.Position = e.NewPosition;
        //                _analogBackGround.Area.CursorX.SelectionStart = double.NaN;
        //                //ParamsAnalog[0].Area.CursorX.SelectionEnd = double.NaN;
        //                _discretBackGround.Area.CursorX.SelectionStart = double.NaN;
        //                //ParamsDiscrete[0].Area.CursorX.SelectionEnd = double.NaN;
        //            }
        //            chartMain.Refresh();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
        //        }
        //    }
    #endregion ControlFunction Cursor

        public void Q1()
        {
            MessageBox.Show(chartMain.Height + "\n" + AnalogAxisMaxHeight);
        }
    }
}



//button1	butVizirNextCur
//button2	butDynShiftOnOff
//button3	butPrint
//button4	butCurMinMaxApply
//button5	butVizirPrevCur
//button6	butDynClear
//button7	butAllScaleAuto
//button8	butViewPeriodApply
//button9	butCurScaleByScale
//button10	butCtrlHide
//button11	butCtrlDel
//button12	butVizirPrevAll
//button13	butVizirNextAll
//button14	butViewPeriodDec
//button15	butViewPeriodInc
//button16	butViewPeriodToMin
//button17	butViewPeriodToMax
//button18	butPositionOverlap
//button19	butPositionCascade
//button20	butAllScaleByScale
//button21	butAllInUnits
//button22	butAllInPrc
//button23	butCurScaleAuto
//button24	butCurInPrc
//button25	butLineWidthCur
//button26	butLineWidthAll
//button27	butDynPeriodApply
//button28	butDynStorageFromApply
//chart1	chartMain
//checkBox1	chbPrintLabels
//comboBox1	cbViewPeriodStep
//comboBox2	cbViewPeriodType
//comboBox25	cbLineWidth
//comboBox3	cbDynPeriodType
//comboBox5	cbViewPeriod
//comboBox6	cbDynPeriod
//dataGridView1	dataGridView
//dataTimePicker1	dateTimePicker
//groupBox1	gbPeriod
//groupBox2	gbCur
//groupBox3	gbAll
//groupBox4	gbCtrl
//groupBox5	gbVizir
//groupBox6	gbPosition
//groupBox7	gbPrint
//groupBox8	gbLineWidth
//label1	labRightPanel
//label4	labViewPeriod
//label5	labDynPeriod
//label6	labBottomPanel
//label7	labCurMin
//label8	labCurMax
//label9	labCur
//label10	labTimeCurTxt
//label12	labCurTxt
//label13	labTimeCur
//label15	labCurUnits
//label16	labVizirL
//label17	labVizirR
//label18	labCtrlCurTxt
//label19	labCtrlCur
//panel1	panelChart
//radioButton4	rbDynStorageFrom
//radioButton5	rbDynShift
//splitContainer1	splitContainerV
//splitContainer2	splitContainerH
//tabControl1	tabControl
//tabPage1	tabPageValue
//tabPage2	tabPageTime
//tabPage4	tabPageCtrl
//textBox1	tbCurMin
//textBox2	tbCurMax
//textBoxVizirDate	tbVizirDate
//textBoxVizirTime	tbVizirTime
