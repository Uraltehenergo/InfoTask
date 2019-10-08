using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BaseLibrary;

namespace GraphicLibrary.Params
{
    public partial class FormGraphic : Form 
    {
        //ИДЕАЛОГИЯ:
        //Несмотря на то, что в аналоговых графиках есть ссылка на ось,
        //а в осях на все их графики
        //Компонент реализован так, что график не работает со своей осью, а ось с её графиками
        //Вся работа реализована в формы
        //Возможно, это не правильно, но переделывать на данном этапе было бы себе дороже

    #region Const
        internal const int AxWidth = 41;        //Ширина вертикальной оси
        internal const int AxYHeight = 20;      //высота дискр графика
        internal const int AxSummand = 24;      //Разница между верхом чарта и верхом фоновой арии
        internal const int AxYLabel = 17;       //Высота наклейки к дискр графику (при базовой высоте)
        
        internal int AxisXLabelFontHeight = 13; //Высота шрифта наклейки аналогового графика

        private const double MinApprox = 0.00001;      //Минимальный часть шкалы на которую возможно приближение выделением на Area или Оси (Димино наследство) 
        private const double MinViewTimeInterval = 1;  //Минимум приближения по оси X (в секундах) //! заменить 10 мс
    #endregion Const

    #region Static
        private static readonly System.Globalization.CultureInfo CurrentCulture =
            System.Globalization.CultureInfo.CurrentCulture;

        //разделитель действительного числа
        private static string _separator;

        //Проверка корректности ввода
        internal static bool CheckInputNumber(KeyPressEventArgs e, string text, string selectedText, int selectionStart, bool positiveOnly = false, bool integerOnly = false)
        {
            if (char.IsDigit(e.KeyChar)) return false;

            if (e.KeyChar == (char)Keys.Back) return false;

            if (e.KeyChar.ToString() == "-")
            {
                if (positiveOnly) return true;

                if (text != "")
                    if (text.IndexOf("-") != -1)
                    {
                        if (selectedText.IndexOf("-") == -1) return true;
                    }
                    else
                    {
                        if (selectionStart != 0) return true;
                    }

                return false;
            }

            if (e.KeyChar.ToString() == _separator)
            {
                if (integerOnly) return true;

                if ((text.IndexOf(_separator) != -1) && (selectedText.IndexOf(_separator) == -1)) return true;
                return false;
            }

            return true;
        }

        internal static void CheckInputPositive(object sender, KeyPressEventArgs e)
        {
            if ((sender is ComboBox) || (sender is TextBox))
            {
                string text, selectedText;
                int selectionStart;

                if (sender is ComboBox)
                {
                    var comboBox = (ComboBox)sender;
                    text = comboBox.Text;
                    selectedText = comboBox.SelectedText;
                    selectionStart = comboBox.SelectionStart;
                }
                else
                {
                    var textBox = (TextBox)sender;
                    text = textBox.Text;
                    selectedText = textBox.SelectedText;
                    selectionStart = textBox.SelectionStart;
                }

                e.Handled = CheckInputNumber(e, text, selectedText, selectionStart, true);
            }
        }

        internal static void CheckInputReal(object sender, KeyPressEventArgs e)
        {
            if ((sender is ComboBox) || (sender is TextBox))
            {
                string text, selectedText;
                int selectionStart;

                if (sender is ComboBox)
                {
                    var comboBox = (ComboBox)sender;
                    text = comboBox.Text;
                    selectedText = comboBox.SelectedText;
                    selectionStart = comboBox.SelectionStart;
                }
                else
                {
                    var textBox = (TextBox)sender;
                    text = textBox.Text;
                    selectedText = textBox.SelectedText;
                    selectionStart = textBox.SelectionStart;
                }

                e.Handled = CheckInputNumber(e, text, selectedText, selectionStart);
            }
        }

        internal static void CheckInputInt(object sender, KeyPressEventArgs e)
        {
            if ((sender is ComboBox) || (sender is TextBox))
            {
                string text, selectedText;
                int selectionStart;

                if (sender is ComboBox)
                {
                    var comboBox = (ComboBox)sender;
                    text = comboBox.Text;
                    selectedText = comboBox.SelectedText;
                    selectionStart = comboBox.SelectionStart;
                }
                else
                {
                    var textBox = (TextBox)sender;
                    text = textBox.Text;
                    selectedText = textBox.SelectedText;
                    selectionStart = textBox.SelectionStart;
                }

                e.Handled = CheckInputNumber(e, text, selectedText, selectionStart, true, true);
            }
        }
    #endregion Static

    #region Private Fields
        //Время начала и конца отображаемого периода
        private DateTime _viewTimeBegin;
        private DateTime _viewTimeEnd;

        //Визир
        private DateTime _vizirTime = DateTime.MinValue;

        //Текущие графики
        private int _curGraphicNum;
        private int _curAnalogGraphicNum;

        //Список команд
        internal readonly UserCommandList CmdList;

        //Ошибки
        public string LastOperationError { get; private set; } //убрать в LastError
        private ErrorCommand _lastError;
    #endregion Private Fields

    #region PublicProp
        //Заголовка формы
        public string Caption
        {
            get { return Text; }
            set { Text = value; }
        }

        //ВременнЫе границы
        public DateTime TimeBegin { get; private set; } //!? разрешить менять
        public DateTime TimeEnd { get; private set; }   //!? разрешить менять

        //Время начала отображаемого периода
        public DateTime ViewTimeBegin
        {
            get { return _viewTimeBegin; }
            set
            {
                DateTime timeE = _viewTimeEnd;
                if (value > timeE) timeE = timeE.AddSeconds(MinViewTimeInterval);
                CmdList.SetTimeView(value, timeE, DateTimeIntervalType.NotSet, ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType);
            }
        }

        //Время конца отображаемого периода
        public DateTime ViewTimeEnd
        {
            get { return _viewTimeEnd; }
            set
            {
                DateTime timeB = _viewTimeBegin;

                if (value < timeB) timeB = timeB.AddSeconds(-MinViewTimeInterval);
                CmdList.SetTimeView(timeB, value, DateTimeIntervalType.NotSet, ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType);
            }
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
        
        public int CurGraphicNum
        {
            get { return _curGraphicNum; }
            set { ChangeCurGraphic(value); }
        }

        //Позиция визира
        public DateTime VizirTime
        {
            get
            {
                var bgr = BackGround;
                if (bgr != null) return DateTime.FromOADate(bgr.Area.CursorX.Position);
                return DateTime.MinValue;
            }
            set
            {
                CmdList.SetVizir(value, _vizirTime, ViewTimeBegin, ViewTimeEnd, _axisXIntervalType);
            }
        }
        
        //Строки, выдаваемые наружу при возникновении ошибки
        public string ErrorMessage
        {
            get { return _lastError == null ? "" : _lastError.Text; }
        }
        public string ErrorParams
        {
            get { return _lastError == null ? "" : _lastError.Exeption.Message + _lastError.Exeption.StackTrace; }
        }
    #endregion PublicProp
    
    #region PrivateProp
        //
        internal int CbLineWidth
        {
            get
            {
                try
                {
                    return int.Parse(cbLineWidth.Text);
                }
                catch
                {
                    cbLineWidth.Text = @"2";
                    return 2;
                }
            }
        }

        //Текущий график
        internal Graphic CurrentGraphic
        {
            get { return GetGraphicByNum(_curGraphicNum); }
        }

        internal Graphic CurrentAnalogGraphic
        {
            get { return GetGraphicByNum(_curAnalogGraphicNum); }
        }

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
        
        //Скролл по Y
        private readonly ScrollBar _hscrollb = new HScrollBar();
        //Кнопка сбросить масштабирование по горизонтали
        private readonly Button _buttonScaleDrop = new Button();

        //дробная часть пустой части приближенного отображения от текущего локального отображения графика
        private double _fillScaleViewPerCentage = .8; // т.е. график отображен на столько процентов от экрана

        //~ private double _timerFactor1;     //в произведении дают интервал таймера
        //~ private int _timerFactor2 = 1000; //в произведении дают интервал таймера
        
        private bool _panelBottomHide; //Спрятана ли нижняя панель
        private bool _panelRightHide; //Спрятана ли правая панель

        private DatabaseType DbType { get; set; } //Access/SQL
        private string ConnString { get; set; }   //Полный путь к базе данных

        private bool _isMouseDown4Divide;
        private int _dividedGraphicNum;
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
                _separator = CurrentCulture.NumberFormat.NumberDecimalSeparator;

                GraphicVisual.FreeColors();

                InitControls();
                InitControlEvents();
                DataGridInit();

                CmdList = new UserCommandList(this, 30);

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

                //принтеры
                InitPrinters();

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
                _hscrollb.MouseEnter += HScroller1;
                _hscrollb.MouseLeave += HScroller2;
                
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
                //labCur1.ForeColor = Color.White;
                //labCur1.TextAlign = ContentAlignment.MiddleCenter;
                //labTimeCur.ForeColor = Color.White;
                //labTimeCur.TextAlign = ContentAlignment.MiddleCenter;
                //labCtrlCur.ForeColor = Color.White;
                //labCtrlCur.TextAlign = ContentAlignment.MiddleCenter;
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

                PrintDocInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void InitControlEvents()
        {
            Shown += Form_BlockAppear; //Блокировка отображения формы, если не задан интервал времени
            
            //запрещаем ввод не чисел в текстовые поля
            cbViewPeriod.KeyPress += CheckInputPositive;
            cbDynPeriod.KeyPress += CheckInputPositive;
            tbCurMin.KeyPress += CheckInputReal;
            tbCurMax.KeyPress += CheckInputReal;
            cbLineWidth.KeyPress += CheckInputInt;
            cbViewPeriodStep.KeyPress += CheckInputPositive;
            //~ cbViewPeriodStep.KeyPress += ComboBox1KeyPress; //Сделано в конструктаре
            //~ cbViewPeriodStep.Leave += ComboBox1Leave;       //Сделано в конструктаре

            //Режим отображения "Сдвиг"
            //~ rbDynShift.Click += SetScalePosition1;

            //~ butViewPeriodDec.Click += WheelZooming;
            //~ butViewPeriodInc.Click += WheelZooming;
            //~ butViewPeriodDec.MouseDown += ButtonHold;
            //~ butViewPeriodDec.MouseUp += ButtonRelease;
            //~ butViewPeriodInc.MouseDown += ButtonHold;
            //~ butViewPeriodInc.MouseUp += ButtonRelease;
            
            butVizirNextCur.Click += VizirNextCur;
            butVizirPrevCur.Click += VizirPrevCur;
            butVizirPrevAll.Click += VizirPrevAll;
            butVizirNextAll.Click += VizirNextAll;
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

            butUndo.MouseHover += ButtonsHover;
            butRedo.MouseHover += ButtonsHover;
            
            //"Обнуляет" кнопки путешествия по времени
            butVizirNextCur.LostFocus += VizirLabelsColorReset;
            butVizirPrevCur.LostFocus += VizirLabelsColorReset;
            butVizirNextAll.LostFocus += VizirLabelsColorReset;
            butVizirPrevAll.LostFocus += VizirLabelsColorReset;
            chartMain.MouseDown += VizirLabelsColorReset;

            //Метод разлепляет графики, если мы перетащили один из скрепленных на область построения,
            //а также в случае выделения правой кнопкой мыши осуществляет масштабирование
            //~ chartMain.MouseUp += Chart1MouseUp; //Перенесено в конструктор

            //при отображении визира появляется данные с его местоположением
            //chartMain.CursorPositionChanged += CursorPositionView;
            //chartMain.CursorPositionChanged += CursorPositionGridView;
            chartMain.CursorPositionChanged += CursorPositionChanged; 
            //Синхронизация курсоров аналоговых и дискретных
            chartMain.CursorPositionChanging += CursorSynch;
            chartMain.CursorPositionChanged += CursorSynchFinal;

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
                //labTimeCurTxt.Location = new Point(labTimeCurTxt.Location.X,
                //                                   gbVizir.Location.Y + gbVizir.Height + 5);
                //labTimeCur.Location = new Point(labTimeCur.Location.X, labTimeCurTxt.Location.Y - 3);

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

        public void Init(DateTime beginTime, DateTime endTime, bool enableGraphicDelete = true)
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

            butCtrlDel.Enabled = enableGraphicDelete;
        }

        //Добавление графиков 
        //если существует график с таким code или id (если id != 0), то не добавляет график и ничего не меняет
        public AnalogGraphic AddAnalogGraphic(string code, int id = 0, string name = "", string subname = "", string dataType = "real",
                                              double? min = null, double? max = null, string units = "", int decPlaces = -1)
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

                    var dt = dataType.ToDataType();
                    var par = new Param(id, code, name, subname, dt, units, min, max);
                    int num = Graphics.Count == 0 ? 1 : Graphics.Last().Num + 1;
                    var gr = new AnalogGraphic(this, par, num);
                    AnalogGraphics.Add(gr);
                    Graphics.Add(gr);
                    GroupsY.Add(new GroupY(gr));
                    Noas++;
                    
                    chartMain.Controls.Add(gr.GroupY.Ax);
                    chartMain.ChartAreas.Add(gr.Area);
                    chartMain.Series.Add(gr.Series);

                    ReRanking();

                    SetAxisXScaleView(gr.GraphicVisual);
                    //mm gr.SetAxisYScaleView(gr.Param.Min, gr.Param.Max);
                    gr.SetAxisYScaleView(gr.MinViewValue, gr.MaxViewValue);

                    DataGridAddRow(gr);

                    //if (num == 1) CurGraphicNum = num;
                    /*else*/ if ((num != 1) && (_curAnalogGraphicNum != 0))
                    {
                        var aGr = (AnalogGraphic) GetGraphicByNum(_curAnalogGraphicNum);
                        AnalogGraphicToTop(aGr);
                    }
                    else _curAnalogGraphicNum = num;

                    CmdList.Clear();

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

        public DiscretGraphic AddDiscretGraphic(string code, int id = 0, string name = "", string subname = "", string dataType = "bool")
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

                    var dt = dataType.ToDataType();
                    var par = new Param(id, code, name, subname, dt, "", 0, 1);
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
                    gr.GraphicVisual.SetAxisYScaleView(0, 1);

                    DataGridAddRow(gr);

                    //if (num == 1) CurGraphicNum = num;

                    CmdList.Clear();

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
                if (graphic.IsAnalog)
                {
                    var agr = (AnalogGraphic) graphic;
                    if (((graphic.Param.OutMin == null) || (graphic.Param.OutMax == null)) && (!agr.GroupY.IsOverlayed))
                    {
                        double min, max;
                        
                        if (!agr.GroupY.IsInPercent)
                        {
                            min = agr.MinViewValue;
                            max = agr.MaxViewValue;
                        }
                        else
                        {
                            min = agr.ValueToPercent(agr.MinViewValue);
                            max = agr.ValueToPercent(agr.MaxViewValue);
                        }
                        SetAxYScaleView(agr, min, max, agr.GroupY.IsInPercent);
                    }
                }

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
            try
            {
                var delGrNum = graphic.Num;

                //удаление из спиcков
                Graphics.Remove(graphic);

                //удаление Series и Area из Chart'а
                chartMain.Series.Remove(graphic.Series);
                chartMain.ChartAreas.Remove(graphic.Area);
                
                if (graphic.IsAnalog)
                {
                    var aGr = (AnalogGraphic) graphic;

                    //удаление оси
                    if (aGr.GroupY.IsOverlayed)
                        AxSeparation(aGr);

                    chartMain.Controls.Remove(aGr.GroupY.Ax);
                    GroupsY.Remove(aGr.GroupY);
                    if(aGr.GroupY.IsVisible) Noas--;

                    AnalogGraphics.Remove((AnalogGraphic)graphic);

                    //удаление фона, если последний
                    if (AnalogGraphics.Count == 0)
                    {
                        chartMain.ChartAreas.Remove(_analogBackGround.Area);
                        chartMain.Series.Remove(_analogBackGround.Series);
                        _analogBackGround = null;
                    }
                }
                else //if(graphic.IsAnalog)
                {
                    var dGr = (DiscretGraphic) graphic;

                    //удаление AxCap
                    chartMain.Controls.Remove(dGr.AxCap);

                    DiscretGraphics.Remove(dGr);

                    //удаление фона, если последний
                    if(DiscretGraphics.Count == 0)
                    {
                        chartMain.ChartAreas.Remove(_discretBackGround.Area);
                        chartMain.Series.Remove(_discretBackGround.Series);
                        _discretBackGround = null;
                    }

                    if (graphic.IsVisible) Nods--;
                }

                //удаление строки из датагрида
                dataGridView.Rows.Remove(GetRowByGraphicNum(graphic.Num));

                //перенкмерация графиков
                foreach (var gr in Graphics)
                    if (gr.Num > delGrNum)
                    {
                        var row = GetRowByGraphicNum(gr.Num);
                        gr.NumDecrease();
                        row.Cells["№ в таблице"].Value = gr.Num;

                        //if(gr.IsDiscret) ((DiscretGraphic) gr).AxCap.Text = gr.Num.ToString(); //перенесено в NumDecrease
                    }

                foreach (var axY in GroupsY)
                {
                    axY.UpperGraphic = axY.UpperGraphic;
                }
                
                //перерисовка
                ReRanking();

                //смена текущего графика
                if(Graphics.Count > 0)
                {
                    CurGraphicNum = (delGrNum > 1) ? delGrNum - 1 : 1;
                    if (_curAnalogGraphicNum == graphic.Num)
                        _curAnalogGraphicNum = AnalogGraphics.Count > 0 ? AnalogGraphics[0].Num : 0;
                }
                else
                {
                    butCurMinMaxApply.Enabled = false;
                    tbCurMin.Enabled = false;
                    tbCurMax.Enabled = false;
                }

                CmdList.Clear();

                //освобождение цвета
                GraphicVisual.FreeColor(graphic.Series.Color);
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

        public void ClearGraphics()
        {
            foreach (var gr in Graphics)
                DeleteGraphic(gr);
        }

        //SQL/Access
        public string SetDatabase(string databaseType, string connectionString)
        {
            try
            {
                DbType = databaseType == "Access" ? DatabaseType.Access : DatabaseType.SqlServer;
                ConnString = connectionString;
                return "";
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return e.Message;
            }
        }

        public string LoadGraphics(string stSql)
        {
            try
            {
                using (var reader = new ReaderAdo(ConnString, stSql))
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt("Id");
                        string code = reader.GetString("Code");
                        string name = reader.GetString("Name");
                        string subName = reader.GetString("SubName");
                        string dataType = reader.GetString("DataType");
                        string units = reader.GetString("Units");
                        double? min = reader.GetDoubleNull("Min");
                        double? max = reader.GetDoubleNull("Max");
                        const int decPlaces = -1;

                        if (!string.IsNullOrEmpty(code))
                        {
                            if (dataType.ToDataType() == DataType.Boolean)
                                AddDiscretGraphic(code, id, name, subName, dataType);
                            else
                                AddAnalogGraphic(code, id, name, subName, dataType, min, max, units, decPlaces);
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return e.Message;
            }
        }

        public string LoadValues(string stSql)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                using (var reader = new ReaderAdo(ConnString, stSql))
                {
                    while (reader.Read())
                    {
                        int nd = reader.GetIntNull("Nd").HasValue ? reader.GetIntNull("Nd").Value : 0;
                        var dot = new MomentReal(reader.GetTime("Time"), reader.GetDouble("Val"), nd);
                        int id = reader.GetIntNull("Id").HasValue ? reader.GetIntNull("Id").Value : 0;
                        string code = reader.GetString("code");

                        Graphic gr = null;
                        if (!string.IsNullOrEmpty(code))
                            gr = GetGraphic(code);
                        else
                            if (id != 0) gr = GetGraphic(id);

                        if (gr != null) gr.AddDot(dot);
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\n===\n" + e.StackTrace);
                return e.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        public string LoadValues(string stSql, DateTime timeBegin, DateTime timeEnd)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                using (var reader = new ReaderAdo(ConnString, stSql))
                {
                    while (reader.Read())
                    {
                        string code = reader.GetString("code");
                        int id = reader.GetIntNull("Id").HasValue ? reader.GetIntNull("Id").Value : 0;
                        
                        Graphic gr = null;
                        if (!string.IsNullOrEmpty(code))
                            gr = GetGraphic(code);
                        else
                            if (id != 0) gr = GetGraphic(id);

                        if (gr != null)
                        {
                            if (reader.GetTime("Time") <= timeEnd)
                            {
                                int nd = reader.GetIntNull("Nd").HasValue ? reader.GetIntNull("Nd").Value : 0;
                                //var time = (reader.GetTime("Time") >= timeBegin) ? reader.GetTime("Time") : timeBegin;
                                var time = reader.GetTime("Time");
                                var dot = new MomentReal(time, reader.GetDouble("Val"), nd);

                                if ((reader.GetTime("Time") > timeBegin) || (gr.DotCount == 0))
                                    gr.AddDot(dot);
                                else
                                {
                                    if (gr.IsAnalog)
                                        ((AnalogGraphic) gr).Dots[gr.DotCount - 1] = dot;
                                    else
                                        ((DiscretGraphic) gr).Dots[gr.DotCount - 1] = dot.ToMomentBoolean();
                                }
                            }
                            else
                            {
                                if (gr.DotCount > 0)
                                {
                                    if (gr.Dot(gr.DotCount - 1).Time < timeEnd)
                                    {
                                        int nd = reader.GetIntNull("Nd").HasValue ? reader.GetIntNull("Nd").Value : 0;
                                        
                                        //double val;
                                        //if (gr.IsAnalog)
                                        //{
                                        //    double a = ((MomentReal)gr.Dot(gr.DotCount - 1)).Mean;

                                        //    val = (reader.GetDouble("Val") - a);
                                        //    val *= timeEnd.Subtract(gr.Dot(gr.DotCount - 1).Time).TotalMilliseconds/
                                        //           reader.GetTime("Time").Subtract(gr.Dot(gr.DotCount - 1).Time).TotalMilliseconds;
                                        //    val += a;
                                        //}
                                        //else
                                        //    val = Convert.ToDouble(((MomentBoolean)gr.Dot(gr.DotCount - 1)).Mean);

                                        //var dot = new MomentReal(timeEnd, val, nd);

                                        var time = reader.GetTime("Time");
                                        var dot = new MomentReal(time, reader.GetDouble("Val"), nd);
                                        
                                        gr.AddDot(dot);
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
                //MessageBox.Show(e.Message + "\n===\n" + e.StackTrace);
                return e.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        //чистка мусора
        public void Gethering()
        {
            GC.Collect();
        }
    #endregion PublicFunctions

    #region ScaleViewX
        //?сделать функцию независмой от параметра scaleType (т.е. вид подписи должен определяться полным интервалом)
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
            if (timeE.Subtract(timeB).TotalSeconds < MinViewTimeInterval) timeE = timeViewBegin.AddSeconds(MinViewTimeInterval);

            if (timeE > TimeEnd) timeE = TimeEnd;
            if (timeE.Subtract(timeB).TotalSeconds < MinViewTimeInterval) timeB = timeViewEnd.AddSeconds(-MinViewTimeInterval);

            _viewTimeBegin = timeB;
            _viewTimeEnd = timeE;

            var ts = timeE.Subtract(timeB);

            _axisXIntervalType = scaleType;
            _axisXScaleViewSizeType = scaleType;
            _axisXInterval = GetAxisXInterval(scaleType, ViewTimeEnd.Subtract(ViewTimeBegin));

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

            double size = MinViewTimeInterval;

            switch (scaleType)
            {
                case DateTimeIntervalType.Milliseconds:
                    size = ts.TotalMilliseconds;
                    break;
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
            else if (ts.TotalSeconds > 2)
            {
                scaleType = DateTimeIntervalType.Seconds;
            }
            else
            {
                scaleType = DateTimeIntervalType.Milliseconds;
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
                grVisual.Area.AxisX.ScaleView.Position = ViewTimeBegin.ToOADate();
                grVisual.Area.AxisX.ScaleView.Size = _axisXScaleViewSize;
            }
            catch (OverflowException)
            {
                MessageBox.Show(@"OverflowException");
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
                    grV.Area.AxisX.ScaleView.Position = ViewTimeBegin.ToOADate();
                    grV.Area.AxisX.ScaleView.Size = _axisXScaleViewSize;
                }
                catch (OverflowException)
                {
                    MessageBox.Show(@"OverflowException");
                }
            }
        }

        private void SetScaleXPosition(Graphic graphic)
        {
            try
            {
                graphic.ReDrawStep(_viewTimeBegin, _viewTimeEnd);
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
            double intv = ViewTimeEnd.Subtract(ViewTimeBegin).TotalSeconds;

            _hscrollb.LargeChange = (int) (intv/totalIntv*1000) + 1;
            _hscrollb.Value = (int) (ViewTimeBegin.Subtract(TimeBegin).TotalSeconds/totalIntv*1000);
        }

        private void ChangeScaleView(DateTime timeViewBegin, DateTime timeViewEnd, DateTimeIntervalType scaleType)
        {
            GetAxisXScaleView(scaleType, timeViewBegin, timeViewEnd);
            SetCbViewPeriodType(_axisXScaleViewSizeType, _axisXScaleViewSize);
            SetScaleXPosition();
            SetScrollPosition();
        }

        //private void ChangeScaleView(DateTime timeViewBegin, DateTime timeViewEnd)
        //{
        //    ChangeScaleView(timeViewBegin, timeViewEnd, _axisXScaleViewSizeType);
        //}

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
                double intv = ViewTimeEnd.Subtract(ViewTimeBegin).TotalSeconds;

                DateTime timeB = TimeBegin.AddMilliseconds(totalIntv*_hscrollb.Value);
                DateTime timeE = timeB.AddSeconds(intv);

                //_cmdList.HScrollerScroll(timeB, timeE, _axisXScaleViewSizeType, 
                //                         ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType);

                GetAxisXScaleView(_axisXScaleViewSizeType, timeB, timeE);
                SetScaleXPosition();

                _scrollValueChanged = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Scroll\n" + ex.Message);
            }
        }

        private DateTime _scrollViewTimeBegin;
        private DateTime _scrollViewTimeEnd;
        private bool _scrollValueChanged = false;

        private void HScroller1(object sender, EventArgs e)
        {
            _scrollViewTimeBegin = ViewTimeBegin;
            _scrollViewTimeEnd = ViewTimeEnd;
        }

        private void HScroller2(object sender, EventArgs e)
        {
            if (_scrollValueChanged)
                CmdList.HScrollerScroll(ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType, _scrollViewTimeBegin, _scrollViewTimeEnd);
            _scrollValueChanged = false;
        }
    #endregion ScaleViewX

    #region ScaleViewY
        //Задаёт начальное и конечное значение для оси axY
        internal void SetAxYScaleView(GroupY axY, double minViewY, double maxViewY, bool isInPercent = false)
        {
            axY.ViewMin = minViewY;
            axY.ViewMax = maxViewY;
            axY.IsInPercent = isInPercent;

            foreach (var gr in axY.Graphics)
                gr.SetAxisYScaleView(minViewY, maxViewY, isInPercent);
        }

        internal void SetAxYScaleView(AnalogGraphic gr, double minViewY, double maxViewY, bool isInPercent = false)
        {
            SetAxYScaleView(gr.GroupY, minViewY, maxViewY, isInPercent);
        }

        //Задаёт начальное и конечное значение для оси axY графика равным его Min и Max (заданных значений параметра)
        private bool SetGraphicScaleByMinMax(AnalogGraphic gr, bool inPercent)
        {
            bool res = false;

            if (gr != null)
            {
                if (!inPercent)
                    //mm SetAxYScaleView(gr, gr.Param.Min, gr.Param.Max);
                    //SetAxYScaleView(gr, gr.MinViewValue, gr.MaxViewValue);
                    res = CmdList.AxYSetViewY(gr.GroupY, gr.MinViewValue, gr.MaxViewValue, false);
                else
                    //SetAxYScaleView(gr, 0, 100, true);
                    res = CmdList.AxYSetViewY(gr.GroupY, 0, 100, true);

                //tbCurMin.Text = ((AnalogGraphic) gr).GroupY.TxtBoxMin.Text;
                //tbCurMax.Text = ((AnalogGraphic) gr).GroupY.TxtBoxMax.Text;
                //butCurScaleAuto.UseVisualStyleBackColor = true;
                foreach (var grA in gr.GroupY.Graphics) 
                    grA.IsAutoScaleY = false;
                //gr.IsAutoScaleY = false;
            }

            return res;
        }

        //Задаёт начальное и конечное значение для оси axY графика на основе его значений
        //Сейчас реализовано на основе всех значений без учёта выделенной области
        //Для учёта выделенной области в ф-ции gr.MinValue() и gr.MaxValue() добавить соответсвующие с параметрами
        private bool SetGraphicScaleAuto(AnalogGraphic gr, bool inPercent)
        {
            bool res = false;

            if (gr != null)
            {
                double minVal = gr.MinValue ?? 0;
                double maxVal = gr.MaxValue ?? 0;
                int dP = gr.Param.DecPlaces;
                
                if (inPercent)
                {
                    //mm minVal = gr.Param.ValueToPercent(minVal);
                    minVal = gr.ValueToPercent(minVal);
                    //mm maxVal = gr.Param.ValueToPercent(maxVal);
                    maxVal = gr.ValueToPercent(maxVal);
                    dP = 0;
                }

                double tmin, tmax;

                GetAutoMinMax(minVal, maxVal, out tmin, out tmax, dP);
                res = CmdList.AxYSetViewY(gr.GroupY, tmin, tmax, inPercent);
                
                //foreach (var grA in gr.GroupY.Graphics)
                //    grA.IsAutoScaleY = false;
                //gr.IsAutoScaleY = true;
            }

            return res;
        }

        //Переводит ось графика в % или абсолютные величины
        //Не сохраняет Автомасштаб
        //Для сохранения автомаштаба необходимо перерасчитывать заменить ф-цию SetAxYScaleView
        //ф-цией SetGraphicScaleAuto при наличии признака автомасштаба графика
        private bool SetGraphicInPercent(AnalogGraphic gr, bool inPercent)
        {
            bool res = false;

            if (gr != null)
            {
                if (inPercent != gr.GroupY.IsInPercent)
                {
                    if (!inPercent)
                        //mm SetAxYScaleView(gr.GroupY, gr.Param.PercentToValue(gr.GroupY.ViewMin), gr.Param.PercentToValue(gr.GroupY.ViewMax));
                        res = CmdList.AxYSetViewY(gr.GroupY, gr.PercentToValue(gr.GroupY.ViewMin), gr.PercentToValue(gr.GroupY.ViewMax), false);
                    else
                        //mm SetAxYScaleView(gr.GroupY, gr.Param.ValueToPercent(gr.GroupY.ViewMin), gr.Param.ValueToPercent(gr.GroupY.ViewMax), true);
                        res = CmdList.AxYSetViewY(gr.GroupY, gr.ValueToPercent(gr.GroupY.ViewMin), gr.ValueToPercent(gr.GroupY.ViewMax), true);

                    //tbCurMin.Text = ((AnalogGraphic) gr).GroupY.TxtBoxMin.Text;
                    //tbCurMax.Text = ((AnalogGraphic) gr).GroupY.TxtBoxMax.Text;
                    //butCurScaleAuto.UseVisualStyleBackColor = true;
                    foreach (var grA in gr.GroupY.Graphics)
                        grA.IsAutoScaleY = false;
                    //gr.IsAutoScaleY = false;
                }
            }

            return res;
        }

        //Вычисляет Min и Max оси по всем её графикам как минимум Min и максимум Max
        private void GetAxYMinMax(GroupY axY, out double min, out double max)
        {
            if (axY.Graphics.Count > 0)
            {
                //mm min = axY.Graphics[0].Param.Min;
                min = axY.Graphics[0].MinViewValue;
                //mm max = axY.Graphics[0].Param.Max;
                max = axY.Graphics[0].MaxViewValue;

                foreach(var gr in axY.Graphics)
                {
                    //mm if (gr.Param.Min < min) min = gr.Param.Min;
                    if (gr.MinViewValue < min) min = gr.MinViewValue;
                    //mm if (gr.Param.Max > max) max = gr.Param.Max;
                    if (gr.MaxViewValue > max) max = gr.MaxViewValue;
                }
            }
            else
            {
                min = 0;
                max = 0;
            }
        }

        //Вычисляет автозначения для начального и конечного значений оси Y как, если бы все графики оси были бы одним
        //При условии, что Min - минимум Min'ов, а Max - максимум Max'ов (для значений в процентах)
        private void GetAxAutoYMinMax(GroupY axY, out double min, out double max, bool inPercent = false)
        {
            double? cMin = null;
            double? cMax = null;

            foreach (var gr in axY.Graphics)
            {
                //mm if ((cMin == null) || (gr.Param.Min < cMin)) cMin = gr.Param.Min;
                if ((cMin == null) || (gr.MinViewValue < cMin)) cMin = gr.MinViewValue;
                //mm if ((cMax == null) || (gr.Param.Max > cMax)) cMax = gr.Param.Max;
                if ((cMax == null) || (gr.MaxViewValue > cMax)) cMax = gr.MaxViewValue;
            }

            double minVal = cMin ?? 0;
            double maxVal = cMax ?? 0;
            
            GetAutoMinMax(minVal, maxVal, out min, out max);

            if (inPercent)
            {
                double axMin, axMax;
                GetAxYMinMax(axY, out axMin, out axMax);

                double minPrc = (min - axMin) / (axMax - axMin) * 100;
                double maxPrc = (max - axMin) / (axMax - axMin) * 100;

                GetAutoMinMaxValue(minPrc, maxPrc, out min, out max);
            }
        }

        //Вычисляет автозначения Min и Max (для автомасштаба)
        //Корректирует значения начальные Min и Max
        private void GetAutoMinMax(double minVal, double maxVal, out double autoMin, out double autoMax, int decPlaces = -1)
        {
            if (minVal == maxVal)
            {
                minVal -= 1;
                maxVal += 1;
            }

            double minY = minVal * 1.03 - maxVal * 0.03;
            double maxY = maxVal * 1.03 - minVal * 0.03;
            
            if(decPlaces < 0)
                GetAutoMinMaxValue(minY, maxY, out autoMin, out autoMax);
            else
            {
                minY = Math.Round(minY, decPlaces);
                maxY = Math.Round(maxY, decPlaces);

                if (minY >= minVal) minY = minY - Math.Pow(10, -decPlaces);
                if (maxY <= maxVal) maxY = maxY + Math.Pow(10, -decPlaces);

                autoMin = minY;
                autoMax = maxY;
            }
        }

        //Вычисляет автозначения Min и Max (для автомасштаба)
        private void GetAutoMinMaxValue(double minVal, double maxVal, out double autoMin, out double autoMax)
        {
            int tpos = 2 - (int) Math.Log10(maxVal - minVal);
            if (tpos < 0) tpos = 0;
            autoMin = Math.Round(minVal, tpos);
            autoMax = Math.Round(maxVal, tpos);
        }
        
        internal static double GetReduceValue(double value, double minVal, double maxVal, int decPlaces = -1)
        {
            if (decPlaces == -1)
            {
                int tpos = 2 - (int) Math.Log10(maxVal - minVal);
                if (tpos < 0) tpos = 0;
                return Math.Round(value, tpos);
            }

            return Math.Round(value, decPlaces);
        }

        private bool _cmdRes;
        internal void SetViewYMinMax(AnalogGraphic gr, double? min, double? max, bool inPercent = false)
        {
            _cmdRes = false;

            try
            {
                if (gr != null)
                {
                    double minV, maxV;

                    if (!gr.GroupY.IsInPercent)
                    {
                        minV = min ?? gr.Area.AxisY.Minimum;
                        maxV = max ?? gr.Area.AxisY.Maximum;
                    }
                    else
                    {
                        //mm minV = min ?? gr.Param.ValueToPercent(gr.Area.AxisY.Minimum);
                        minV = min ?? gr.ValueToPercent(gr.Area.AxisY.Minimum);
                        //mm maxV = max ?? gr.Param.ValueToPercent(gr.Area.AxisY.Maximum);
                        maxV = max ?? gr.ValueToPercent(gr.Area.AxisY.Maximum);
                    }

                    //if (aGr.Param.DecPlaces >= 0) minV = Math.Round(minV, aGr.Param.DecPlaces);
                    //if (aGr.Param.DecPlaces >= 0) maxV = Math.Round(maxV, aGr.Param.DecPlaces);

                    if (minV > maxV)
                    {
                        double b = minV;
                        minV = maxV;
                        maxV = b;
                    }

                    if (minV != maxV)
                    {
                        _cmdRes = CmdList.AxYSetViewY(gr.GroupY, minV, maxV, gr.GroupY.IsInPercent);
                        //ChangeCurGraphic(CurGraphicNum); //по хорошему, надо только если график принадлежит текущей оси (оси с текущим графиком)
                        CurGraphicNum = _curGraphicNum;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                //ChangeCurGraphic(CurGraphicNum); //по хорошему, надо только если график принадлежит текущей оси (оси с текущим графиком)
                CurGraphicNum = _curGraphicNum;
            }
        }

        internal void SetViewYMinMax(AnalogGraphic gr, string min, string max, bool inPercent = false)
        {
            try
            {
                double? minV = null;
                if (!string.IsNullOrEmpty(min)) minV = double.Parse(min);
                double? maxV = null;
                if (!string.IsNullOrEmpty(max)) maxV = double.Parse(max);

                SetViewYMinMax(gr, minV, maxV, inPercent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                //ChangeCurGraphic(CurGraphicNum);
                CurGraphicNum = _curGraphicNum;
            }
        }
    #endregion ScaleViewY

    #region ScaleViewY (старые варианты)
        private void SetGraphicScaleAuto_Old(AnalogGraphic gr, bool inPercent)
        {
            if (gr != null)
            {
                double minVal = gr.MinValue ?? 0;
                double maxVal = gr.MaxValue ?? 0;

                /*if (minVal == maxVal)
                {
                    minVal -= 1;
                    maxVal += 1;
                }

                double minY = minVal * 1.03 - maxVal * 0.03;
                double maxY = maxVal * 1.03 - minVal * 0.03;

                int tpos = (int)Math.Log10(maxVal - minVal) - 2;
                double tmin = (int)(minY / Math.Pow(10, tpos)) * Math.Pow(10, tpos);
                double tmax = (int)(maxY / Math.Pow(10, tpos)) * Math.Pow(10, tpos);
                */
                //заменено вынесенными ф-циями (ниже)
                //при этом немного изменилось определение tpos
                //но на результат это не влияет
                double tmin, tmax;

                GetAutoMinMax(minVal, maxVal, out tmin, out tmax);

                if (!inPercent)
                    CmdList.AxYSetViewY(gr.GroupY, tmin, tmax, false);
                else
                {
                    //mm double minPrc = gr.Param.ValueToPercent(tmin);
                    double minPrc = gr.ValueToPercent(tmin);
                    //mm double maxPrc = gr.Param.ValueToPercent(tmax);
                    double maxPrc = gr.ValueToPercent(tmax);

                    /*int tposPrc = (int)Math.Log10(maxPrc - minPrc) - 2;
                    double tminPrc = ((int)(minPrc / Math.Pow(10, tposPrc))) * Math.Pow(10, tposPrc);
                    double tmaxPrc = ((int)(maxPrc / Math.Pow(10, tposPrc))) * Math.Pow(10, tposPrc);
                    */
                    //заменено вынесенными ф-циями (ниже)
                    double tminPrc, tmaxPrc;
                    GetAutoMinMaxValue(minPrc, maxPrc, out tminPrc, out tmaxPrc);

                    CmdList.AxYSetViewY(gr.GroupY, tminPrc, tmaxPrc, true);
                }

                //tbCurMin.Text = ((AnalogGraphic) gr).GroupY.TxtBoxMin.Text;
                //tbCurMax.Text = ((AnalogGraphic) gr).GroupY.TxtBoxMax.Text;
                //butCurScaleAuto.UseVisualStyleBackColor = false;
                //butCurScaleAuto.BackColor = SystemColors.Highlight;
                
                //foreach (var grA in gr.GroupY.Graphics)
                //    grA.IsAutoScaleY = false;
                //gr.IsAutoScaleY = true;
            }
        }

        private void GetAutoMinMax_Old(double minVal, double maxVal, out double autoMin, out double autoMax)
        {
            if (minVal == maxVal)
            {
                minVal -= 1;
                maxVal += 1;
            }

            double minY = minVal * 1.03 - maxVal * 0.03;
            double maxY = maxVal * 1.03 - minVal * 0.03;

            GetAutoMinMaxValue(minY, maxY, out autoMin, out autoMax);
        }
        
        private void GetAutoMinMaxValue_Old(double minVal, double maxVal, out double autoMin, out double autoMax)
        {
            int tpos = (int)Math.Log10(maxVal - minVal) - 2;
            autoMin = (int)(minVal / Math.Pow(10, tpos)) * Math.Pow(10, tpos);
            autoMax = (int)(maxVal / Math.Pow(10, tpos)) * Math.Pow(10, tpos);
        }
    #endregion ScaleViewY (старые варианты)

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
        
        private void ChangeCurGraphic(int curNum)
        {
            try
            {
                var gr = GetGraphicByNum(curNum);
                if (gr != null)
                {
                    //_flagDgNoChangeOld = true;
                    if(_flagDgNoChange) dataGridView.CurrentCell = GetRowByGraphicNum(curNum).Cells[1];
                    _flagDgNoChange = true;
                    //_flagDgNoChangeOld = false;

                    _curGraphicNum = curNum;

                    labCur.BackColor = gr.Series.Color;
                    labCur.Text = CurGraphicNum.ToString();
                    //labCur1.BackColor = gr.Series.Color;
                    //labCur1.Text = CurGraphicNum.ToString();
                    //labTimeCur.BackColor = gr.Series.Color;
                    //labTimeCur.Text = labCur.Text;
                    //labCtrlCur.BackColor = gr.Series.Color;
                    //labCtrlCur.Text = labCur.Text;

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

                    butCtrlHide.Text = gr.IsVisible ? "Скрыть график" : "Отобразить график";

                    if ((_dynState == EDynState.NotDyn) && (gr.IsAnalog) && (((AnalogGraphic)gr).IsAutoScaleY))
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
                        _curAnalogGraphicNum = curNum;
                        AnalogGraphicToTop((AnalogGraphic)gr);

                        //Меняется раскраска таблицы для графиков склееных очей
                        DataGridGraphicToTop((AnalogGraphic) gr);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void AnalogGraphicToTop(AnalogGraphic gr)
        {
            if (gr!=null)
            {
                bool seriesFlag = chartMain.Series.Remove(gr.Series);
                bool areaFlag = chartMain.ChartAreas.Remove(gr.Area);
                if (areaFlag) chartMain.ChartAreas.Add(gr.Area);
                if (seriesFlag) chartMain.Series.Add(gr.Series);
                //ToTopOverlay(tempGr);
            }
        }

        private int _analogAxisMaxHeight;
        private int AnalogAxisMaxHeight
        {
            get
            {
                int h = ((chartMain.Height - AxisXLabelFontHeight - 2*(Nods - 1) - 11 - AxYHeight*(Nods + 1)));
                if (splitContainerV.Panel1.HorizontalScroll.Visible) h -= 20;

                if (GroupsY.Count > 0)
                    if (h < GroupsY.First().Ax.MinimumSize.Height) 
                        h = GroupsY.First().Ax.MinimumSize.Height;

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

            dataGridView.CellClick += DataGrid_CellClick;
            dataGridView.CellValueChanged += DataGridCheckChange;
            dataGridView.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;
            dataGridView.SelectionChanged += DataGridSelectionChanged;
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
                                                    graphic.Param.OutMin.ToString(), graphic.Param.OutMax.ToString(),
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
                                                    graphic.Param.OutMin.ToString(), graphic.Param.OutMax.ToString(),
                                                    graphic.DataTypeString
                                                });

            dataGridView.Rows[row].Cells["№ в таблице"].Style.BackColor = graphic.Series.Color;
            dataGridView.Rows[row].Cells["№ в таблице"].Style.SelectionBackColor = graphic.Series.Color;
            dataGridView.Rows[row].Cells[0].Style.BackColor = graphic.Series.Color;
            dataGridView.Rows[row].Cells[0].Style.SelectionBackColor = graphic.Series.Color;
            dataGridView.Rows[row].Cells["Group"].Value = graphic.Num;

            if (graphic.IsDiscret)
            {
                ((DataGridViewCheckBoxCell) dataGridView.Rows[row].Cells[0]).ThreeState = true;
                //((DataGridViewCheckBoxCell) dataGridView.Rows[t.Index - 1].Cells[0]).IndeterminateValue = 3;
                dataGridView.Rows[row].Cells[0].Value = 2;
                dataGridView.Rows[row].Cells[0].ReadOnly = true;
            }
        }

        //Вычисляет выделенный в таблице параметр и записывает в CurGraphicNum
        //bool _flagDgNoChangeOld = false;
        bool _flagDgNoChange = true;
        internal void DataGridSelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count >= 1)
            {
                int num = int.Parse(dataGridView.CurrentRow.Cells["№ в таблице"].Value.ToString());
                _flagDgNoChange = false;
                CurGraphicNum = num;
            }
        }

        //cкрывает/отображает графика
        private void DataGridHideGraphic(Graphic gr)
        {
            var row = GetRowByGraphicNum(gr.Num);

            var curCell = row.Cells["№ в таблице"];
            curCell.Style.BackColor = Color.Empty;
            curCell.Style.SelectionBackColor = Color.White; //Color.Empty;
            curCell.Style.ForeColor = gr.Series.Color;
            curCell.Style.SelectionForeColor = curCell.Style.ForeColor;

            row.DefaultCellStyle.ForeColor = Color.Gray;
            row.DefaultCellStyle.SelectionForeColor = Color.Gray;
        }

        private void DataGridShowGraphic(Graphic gr)
        {
            var row = GetRowByGraphicNum(gr.Num);

            var curCell = row.Cells["№ в таблице"];
            curCell.Style.BackColor = gr.Series.Color;
            curCell.Style.SelectionBackColor = gr.Series.Color;
            curCell.Style.ForeColor = Color.White;
            curCell.Style.SelectionForeColor = Color.White;

            row.DefaultCellStyle.ForeColor = Color.Empty;
            row.DefaultCellStyle.SelectionForeColor = Color.Empty;
        }

        internal void DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 1)
                {
                    DataGridViewCell curCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    var num = int.Parse(curCell.Value.ToString());
                    var gr = GetGraphicByNum(num);

                    if (curCell.Style.BackColor != Color.Empty)
                        CmdList.GraphicHide(gr);
                    else
                        CmdList.GraphicShow(gr);
                }
            }
        }

        private bool _needToCheckChangeDatagrid = true;
        internal void DataGridCheckChange(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex >= 0) && (e.ColumnIndex == 0))
            {
                var num = int.Parse(dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString());
                var gr = GetGraphicByNum(num);

                if ((gr.IsAnalog) && (_needToCheckChangeDatagrid))
                {
                    if ((bool)dataGridView.Rows[e.RowIndex].Cells[0].Value)
                        CmdList.AxYShow(((AnalogGraphic) gr).GroupY);
                    else
                        CmdList.AxYHide(((AnalogGraphic)gr).GroupY);
                }
                
                _needToCheckChangeDatagrid = true;
            }
        }

        private void DataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        //Делает данный график верхним
        private void DataGridGraphicToTop(AnalogGraphic gr)
        {
            if (gr.IsVisible)
            {
                gr.GroupY.UpperGraphic = gr;
                foreach (var grOv in gr.GroupY.Graphics)
                {
                    var row = GetRowByGraphicNum(grOv.Num);

                    row.Cells[0].Style.BackColor = gr.Series.Color;
                    row.Cells[0].Style.SelectionBackColor = gr.Series.Color;
                }
            }
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

    #region Ranking
        //Установка на место оси Y (координата Y и высота) по CurAxSize и CurAxPos
        private void AxisRankingY(GroupY groupY)
        {
            int h = AnalogAxisMaxHeight;
            double curPos = groupY.CurAxPos;
            groupY.Ax.Height = (int)(h * groupY.CurAxSize);
            groupY.CurAxPos = curPos;
            groupY.Ax.Top = (int)(h * groupY.CurAxPos);
            if (groupY.Ax.Bottom > h)
            {
                int d = groupY.Ax.Top - groupY.Ax.Bottom + h;
                groupY.Ax.Top = d > 0 ? d : 0;
            }
        }

        //Установка на место оси Y (координата X)
        private void AxisRankingX(GroupY groupY, int analogNum)
        {
            groupY.Ax.Location = new Point((analogNum - 1) * AxWidth, 0);
        }

        //Установка на место оси Y
        private void AxisRanking(GroupY groupY, int analogCounter)
        {
            AxisRankingX(groupY, analogCounter);
            AxisRankingY(groupY);
        }

        //Установка на место метки дискретного графика Y
        private void AxCapRanking(DiscretGraphic dGr)
        {
            dGr.AxCap.Left = (int) (dGr.Area.Position.X/100*chartMain.Width) - AxWidth;
            //dGr.AxCap.Location = new Point(dGr.AxCap.Location.X, (int)(dGr.Area.Position.Y * chartMain.Height / 100));
            dGr.AxCap.Top = (int) (dGr.Area.Position.Y*chartMain.Height/100);
        }

        //Установка ширины и позицию по Х Area графиков
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

        //Установка высоты Area аналогового графика
        private void AnalogAreaRankingHeight(AnalogGraphic aGr)
        {
            int h = aGr.GroupY.Ax.Size.Height - AxSummand;
            aGr.Area.Position.Height = h*100f/chartMain.Height;
            
            float h1 = aGr.GroupY.GraphicAreaBottom - AxSummand;
            aGr.Area.InnerPlotPosition.Y = 0;
            aGr.Area.InnerPlotPosition.Height = h1 / h * 100f;

            aGr.Area.InnerPlotPosition.X = 0;
            aGr.Area.InnerPlotPosition.Width = 100;
        }

        //Установка позиции по Y Area аналогового графика
        private void AnalogAreaRankingY(AnalogGraphic aGr)
        {
            aGr.Area.Position.Y = 100f*(aGr.GroupY.Ax.Top + AxSummand)/chartMain.Height;
        }

        //Установка высоты Area дискретного графика
        private void DiscretAreaRankingHeight(ChartArea area)
        {
            int h = (AxYLabel + AxYHeight);
            area.Position.Height = h*100f/chartMain.Height;
            
            area.InnerPlotPosition.Y = 0;
            area.InnerPlotPosition.Height = AxYLabel*100f/h;

            area.InnerPlotPosition.X = 0;
            area.InnerPlotPosition.Width = 100;
        }

        //Установка позиции по Y Area дискретного графика
        private void DiscretAreaRankingY(ChartArea area, int discretCounter)
        {
            //дискретные графики снизу вверх
            //area.Position.Y = 100*(1 - ((discretCounter + 1)*AxYHeight + 2*(discretCounter - 1) + 10f)/chartMain.Height);
            
            //дискретные графики сверху вниз
            var i = Nods - discretCounter + 1;
            area.Position.Y = 100 * (1 - ((i + 1) * AxYHeight + 2 * (i - 1) + 10f) / chartMain.Height);
        }

        //Установка высоты Area фонового дискретного графика
        private void DiscretAreaBgrHeight()
        {
            float y = AxYHeight*(Nods + 1) + 2*(Nods - 1) + 10;
            _discretBackGround.Area.Position.Y = 100*(1 - y/chartMain.Height);

            int h = (AxYLabel + AxYHeight*Nods + 2*(Nods - 1));
            _discretBackGround.Area.Position.Height = h*100f/chartMain.Height;
            
            _discretBackGround.Area.InnerPlotPosition.Y = 0;
            _discretBackGround.Area.InnerPlotPosition.Height = (h - AxYHeight) * 100f / h;

            _discretBackGround.Area.InnerPlotPosition.X = 0;
            _discretBackGround.Area.InnerPlotPosition.Width = 100;
        }

        //Установка высоты Area фонового аналогового графика
        private void AnalogAreaBgrHeight()
        {
            const int y = 0;
            _analogBackGround.Area.Position.Y = (y + AxSummand)*100f/chartMain.Height;

            int h = AnalogAxisMaxHeight;
            //if (h < GroupsY.First().Ax.MinimumSize.Height) h = GroupsY.First().Ax.MinimumSize.Height;

            int h1 = (h - AxSummand);
            _analogBackGround.Area.Position.Height = h1*100f/chartMain.Height;

            int h2 = h - 2 * AxSummand;
            _analogBackGround.Area.InnerPlotPosition.Y = 0;
            _analogBackGround.Area.InnerPlotPosition.Height = h2 * 100f / h1;

            _analogBackGround.Area.InnerPlotPosition.X = 0;
            _analogBackGround.Area.InnerPlotPosition.Width = 100;
        }

        private void DownerScrollRankingSize()
        {
            var w = (BackGround != null) ? BackGround.Area.Position.Width : 100;
            _hscrollb.Width = Convert.ToInt32(w*chartMain.Size.Width/100);
            _buttonScaleDrop.Top = splitContainerV.Panel1.Size.Height - 20;
            _buttonScaleDrop.Width = splitContainerV.Panel1.Size.Width - _hscrollb.Width;
            _hscrollb.Location = new Point(_buttonScaleDrop.Width, splitContainerV.Panel1.Size.Height - 20);
        }

        //Установка на место аналоговых графиков оси (координата Y и высота)
        //В отличае от AxYResize ставит на место ось по заданным CurAxSize и CurAxPos,
        //а не рассчитывает их
        private void AnalogRankingY(GroupY groupY)
        {
            AxisRankingY(groupY);
            foreach (var gr in groupY.Graphics)
            {
                AnalogAreaRankingHeight(gr);
                AnalogAreaRankingY(gr);
            }
        }

        //Установка на место всех графиков (оси, Area)
        private void ReRanking()
        {
            if (AnalogGraphics.Count > 0)
            {
                if (Noas > 0)
                {
                    _analogBackGround.Area.Visible = true;
                    AreaRankingX(_analogBackGround.Area);
                    AnalogAreaBgrHeight();
                }
                else
                    _analogBackGround.Area.Visible = false;
            }

            int i = 0;
            foreach (var grY in GroupsY)
            {
                if (grY.IsVisible) i++; 
                
                AxisRanking(grY, i);

                foreach (var aGr in grY.Graphics)
                {
                    AreaRankingX(aGr.Area);
                    AnalogAreaRankingHeight(aGr);
                    AnalogAreaRankingY(aGr);
                }
            }

            if (DiscretGraphics.Count > 0)
            {
                if (Nods > 0)
                {
                    _discretBackGround.Area.Visible = true;
                    AreaRankingX(_discretBackGround.Area);
                    DiscretAreaBgrHeight();
                }
                else
                    _discretBackGround.Area.Visible = false;
            }

            i = 0;
            foreach (var dGr in DiscretGraphics)
            {
                if (dGr.IsVisible) i++;
                
                AreaRankingX(dGr.Area);
                DiscretAreaRankingHeight(dGr.Area);
                DiscretAreaRankingY(dGr.Area, i);
                AxCapRanking(dGr);
            }

            DownerScrollRankingSize();
        }
    #endregion Ranking

    #region ControlFunction
        private DateTimeIntervalType IntervalTypeFromCbViewPeriodType
        {
            get
            {
                switch (cbViewPeriodType.Text)
                {
                    case "мс.":
                        return DateTimeIntervalType.Milliseconds;
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
                case DateTimeIntervalType.Milliseconds:
                    cbViewPeriodType.Text = "мс.";
                    break;
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
                case DateTimeIntervalType.Milliseconds:
                    cbViewPeriodType.Text = "Сек.";
                    cbViewPeriod.Text = interval.TotalMilliseconds.ToString();
                    break;
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

        internal void ComboBoxAutoDropDownWidth(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            int newWidth = cb.DropDownWidth;
            int width = cb.DropDownWidth;
            Font font = cb.Font;
            Graphics g = cb.CreateGraphics();
            int vertScrollBarWidth =
                (cb.Items.Count > cb.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;
            foreach (var item in cb.Items)
            {
                string s = cb.GetItemText(item);
                newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            cb.DropDownWidth = width;
        }
    #endregion ControlFunction
    
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

        private void chartMain_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool fA = false;
                if(AnalogGraphics.Count > 0)
                {
                    int h2 = AnalogAxisMaxHeight;
                    int h1 = h2 - AxSummand;
                    
                    if((e.Y > h1) && (e.Y < h2)) fA = true;
                }

                bool fD = false;
                if(DiscretGraphics.Count > 0)
                {
                    //int h4 = (int)(((_discretBackGround.Area.Position.Bottom - 1) * chartMain.Height) / 100);
                    //int h3 = Math.Min((int)(((_discretBackGround.Area.Position.Bottom - 4) * chartMain.Height) / 100), h4 - AxSummand);
                    int h4 = (int)(_discretBackGround.Area.Position.Bottom * chartMain.Height / 100);
                    int h3 = h4 - AxSummand;

                    if ((e.Y > h3) && (e.Y < h4)) fD = true;
                }

                if (fA || fD)
                {
                    double scaleViewXPart = e.X;
                    double scaleViewPerCent = 100 * scaleViewXPart / chartMain.Width;
                    if (scaleViewPerCent <= 100 && scaleViewPerCent >= 0)
                    {
                        string temp = string.Format("{0:d}", DateTime.FromOADate(
                                                        chartMain.ChartAreas.First().AxisX.PositionToValue(scaleViewPerCent)));
                        toolTip.SetToolTip(chartMain, temp);
                    }
                }

                if ((e.Button == MouseButtons.Right) && (AnalogGraphics.Count > 0))
                {
                    _analogBackGround.Area.CursorY.SelectionStart = (_analogBackGround.Area.AxisY.ScaleView.ViewMaximum -
                                                                 _analogBackGround.Area.AxisY.ScaleView.ViewMinimum) *
                                                                (AnalogAxisMaxHeight - AxSummand - e.Y) /
                                                                (AnalogAxisMaxHeight - 2 * AxSummand) +
                                                                _analogBackGround.Area.AxisY.ScaleView.ViewMinimum;
                }
            }
            catch (Exception ex)
            {
                //Error = new ErrorCommand("(Chart1MouseDown)", ex);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void chartMain_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (_isMouseDown4Divide) //разделение слепленных осей
                {
                    var gr = GetGraphicByNum(_dividedGraphicNum);
                    //cmd AxSeparation((AnalogGraphic) gr);
                    CmdList.GraphicChangeAxY((AnalogGraphic) gr, null);
                    _isMouseDown4Divide = false;
                    contextMenuStrip.Hide();
                }

                if ((e.Button == MouseButtons.Right) && (AnalogGraphics.Count > 0))
                {
                    double selMin = Math.Min(_analogBackGround.Area.CursorY.SelectionEnd,
                                             _analogBackGround.Area.CursorY.SelectionStart);
                    double selMax = Math.Max(_analogBackGround.Area.CursorY.SelectionEnd,
                                             _analogBackGround.Area.CursorY.SelectionStart);
                    
                    //if (selMin != selMax)
                    //{
                    //    var selMinY = (_analogBackGround.Area.AxisY.Maximum - selMin)/
                    //                  (_analogBackGround.Area.AxisY.Maximum - _analogBackGround.Area.AxisY.Minimum);

                    //    var selMaxY = (_analogBackGround.Area.AxisY.Maximum - selMax)/
                    //                  (_analogBackGround.Area.AxisY.Maximum - _analogBackGround.Area.AxisY.Minimum);

                    //    int minY = AxSummand + (int) (selMinY*(AnalogAxisMaxHeight - 2*AxSummand));
                    //    int maxY = AxSummand + (int) (selMaxY*(AnalogAxisMaxHeight - 2*AxSummand));
                        
                    //    foreach(var axY in GroupsY)
                    //    {
                    //        var gr = axY.UpperGraphic;

                    //        bool fg = false;
                    //        double min = axY.ViewMin;
                    //        double max = axY.ViewMax;

                    //        if ((maxY >= (axY.Ax.Top + AxSummand)) && (maxY <= (axY.Ax.Bottom - AxSummand)))
                    //        {
                    //            fg = true;
                    //            max = 
                    //        }

                    //        if ((minY >= (axY.Ax.Top + AxSummand)) && (minY <= (axY.Ax.Bottom - AxSummand)))
                    //        {
                    //            fg = true;
                    //            min = 
                    //        }

                    //        //if (fg) SetViewYMinMaxByNum(gr.Num, min, max, axY.IsInPercent);
                    //    }
                    //}

                    if (selMin != selMax)
                    {
                        selMin = (-_analogBackGround.Area.AxisY.Minimum + selMin)/
                                 (_analogBackGround.Area.AxisY.Maximum - _analogBackGround.Area.AxisY.Minimum);

                        selMax = (-_analogBackGround.Area.AxisY.Minimum + selMax) /
                                 (_analogBackGround.Area.AxisY.Maximum - _analogBackGround.Area.AxisY.Minimum);
                        
                        int h = AnalogAxisMaxHeight - 2*AxSummand;

                        bool fgGroup = false;

                        foreach(var axY in GroupsY)
                        {
                            var gr = axY.UpperGraphic;

                            double top = axY.Ax.Top;
                            //double bot = axY.Ax.Bottom - 2 * AxSummand;
                            double bot = axY.Ax.Top + axY.GraphicAreaBottom - AxSummand;

                            double axMax = 1 - top/h;
                            double axMin = 1 - bot/h;

                            bool fg = false;
                            double min = axY.ViewMin;
                            double max = axY.ViewMax;

                            if ((selMin >= axMin) && (selMin <= axMax))
                            {
                                fg = true;
                                min = (selMin - axMin)/(axMax - axMin);
                                min = axY.ViewMin + min*(axY.ViewMax - axY.ViewMin);

                                //mm min = !axY.IsInPercent ? GetReduceValue(min, gr.Param.Min, gr.Param.Max, gr.Param.DecPlaces) : GetReduceValue(min, 0, 100, 0);
                                min = !axY.IsInPercent ? GetReduceValue(min, gr.MinViewValue, gr.MaxViewValue, gr.Param.DecPlaces) : GetReduceValue(min, 0, 100, 0);
                            }

                            if ((selMax >= axMin) && (selMax <= axMax))
                            {
                                fg = true;
                                max = (selMax - axMin)/(axMax - axMin);
                                max = axY.ViewMin + max * (axY.ViewMax - axY.ViewMin);

                                //mm max = !axY.IsInPercent ? GetReduceValue(max, gr.Param.Min, gr.Param.Max, gr.Param.DecPlaces) : GetReduceValue(max, 0, 100, 0);
                                max = !axY.IsInPercent ? GetReduceValue(max, gr.MinViewValue, gr.MaxViewValue, gr.Param.DecPlaces) : GetReduceValue(max, 0, 100, 0);
                            }

                            if (fg)
                            {
                                if (fgGroup) CmdList.ContinueGroup();
                                SetViewYMinMax(axY.UpperGraphic, min, max, axY.IsInPercent);
                                fgGroup = fgGroup || _cmdRes;
                            }
                        }
                    }

                    _analogBackGround.Area.CursorY.SelectionEnd = double.NaN;
                }
            }
            catch (Exception exception)
            {
                //Error = new ErrorCommand("", exception);
                if (_analogBackGround != null)
                    _analogBackGround.Area.CursorY.SelectionEnd = double.NaN;
            }
        }

        private void chartMain_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if ((e.Button == MouseButtons.Right) && (AnalogGraphics.Count > 0))
                {
                    double selEnd = (_analogBackGround.Area.AxisY.ScaleView.ViewMaximum -
                                     _analogBackGround.Area.AxisY.ScaleView.ViewMinimum)*
                                    (AnalogAxisMaxHeight - AxSummand - e.Y)/
                                    (AnalogAxisMaxHeight - 2*AxSummand) +
                                    _analogBackGround.Area.AxisY.ScaleView.ViewMinimum;

                    double d = .05*
                               (_analogBackGround.Area.AxisY.ScaleView.ViewMaximum -
                                _analogBackGround.Area.AxisY.ScaleView.ViewMinimum);

                    if (Math.Abs(selEnd - _analogBackGround.Area.CursorY.SelectionStart) < d)
                        selEnd = _analogBackGround.Area.CursorY.SelectionStart;

                    _analogBackGround.Area.CursorY.SelectionEnd = selEnd;
                }
            }
            catch (Exception ex)
            {
                //Error = new ErrorCommand("(Chart1MouseDown)", ex);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void chartMain_AxisViewChanged(object sender, ViewEventArgs e)
        {
            try
			{
                if ((e.Axis.AxisName == AxisName.X) && (e.ChartArea.CursorX.SelectionStart > 0))
                {
                    DateTime timeB;
                    DateTime timeE;

					if(e.ChartArea.CursorX.SelectionStart < e.ChartArea.CursorX.SelectionEnd)
					{
					    timeB = DateTime.FromOADate(e.ChartArea.CursorX.SelectionStart);
                        timeE = DateTime.FromOADate(e.ChartArea.CursorX.SelectionEnd);
					}
                    else
					{
                        timeB = DateTime.FromOADate(e.ChartArea.CursorX.SelectionEnd);
                        timeE = DateTime.FromOADate(e.ChartArea.CursorX.SelectionStart);
					}

                    CmdList.SetTimeView(timeB, timeE, DateTimeIntervalType.NotSet, ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType);
				}
			}
			catch (Exception ex)
			{
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
        }

        //Блокировка слишком маленького приближения по времени
        private void chartMain_SelectionRangeChanging(object sender, CursorEventArgs e)
        {
            if (e.Axis.AxisName == AxisName.X)
            {
                var dt = Math.Abs(e.NewSelectionEnd - e.NewSelectionStart);
                var ts = DateTime.FromOADate(Math.Max(e.NewSelectionEnd, e.NewSelectionStart)).Subtract(
                        DateTime.FromOADate(Math.Min(e.NewSelectionEnd, e.NewSelectionStart))).TotalSeconds;
                
                if (dt < .03 * (e.ChartArea.AxisX.ScaleView.ViewMaximum - e.ChartArea.AxisX.ScaleView.ViewMinimum) ||
                   (ts < MinViewTimeInterval))
                {
                    e.NewSelectionEnd = e.NewSelectionStart;
                }
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

                case "butUndo":
                    tempS = "Отменить:" + CmdList.UndoList(1);
                    break;
                case "butRedo":
                    tempS = "Вернуть:" + CmdList.RedoList(1);
                    break;

            }
            toolTip.SetToolTip((Button)sender, tempS);
        }
        
        //Эффект залипания кнопок
        //~ private int _starter;
        //~ private Button _holdedButton;

        /*private void ButtonHold(object sender, EventArgs e)
        {
            _holdedButton = (Button)sender;
            timerHold.Enabled = true;
            timerHold.Interval = 130;
            timerHold.Start();
            _starter = 0;
        }*/
       
        /*private void ButtonRelease(object sender, EventArgs e)
        {
            chartMain.Refresh();
            timerHold.Stop();
            _holdedButton = null;
        }*/

        /*private void TimerHold_Tick(object sender, EventArgs e)
        {
            if (_starter >= 3)
                switch (_holdedButton.Name)
                {
                    case "butVizirNextCur":
                        VizirNextCur(_holdedButton, e);
                        break;
                    case "butVizirPrevCur":
                        VizirPrevCur(_holdedButton, e);
                        break;
                    case "butVizirPrevAll":
                        VizirPrevAll(_holdedButton, e);
                        break;
                    case "butVizirNextAll":
                        VizirNextAll(_holdedButton, e);
                        break;

                    //case "butViewPeriodDec":
                    //    WheelZooming(_holdedButton, e);
                    //    break;
                    //case "butViewPeriodInc":
                    //    WheelZooming(_holdedButton, e);
                    //    break;
                }
            _starter++;
        }*/
    #endregion ControlFunction Form

    #region ControlFunction Time
        //+
        private void butViewPeriodApply_Click(object sender, EventArgs e)
        {
            try
            {
                double intv = double.Parse(cbViewPeriod.Text);
                DateTimeIntervalType intvType = IntervalTypeFromCbViewPeriodType;

                DateTime timeViewEnd = _viewTimeEnd;
                switch (intvType)
                {
                    case DateTimeIntervalType.Seconds:
                        timeViewEnd = _viewTimeBegin.AddSeconds(intv);
                        break;
                    case DateTimeIntervalType.Minutes:
                        timeViewEnd = _viewTimeBegin.AddMinutes(intv);
                        break;
                    case DateTimeIntervalType.Hours:
                        timeViewEnd = _viewTimeBegin.AddHours(intv);
                        break;
                    case DateTimeIntervalType.Days:
                        timeViewEnd = _viewTimeBegin.AddDays(intv);
                        break;
                }

                DateTime timeViewBegin = _viewTimeBegin;
                if (timeViewEnd > TimeEnd)
                {
                    timeViewBegin = timeViewBegin.Add(TimeEnd.Subtract(timeViewEnd));
                    timeViewEnd = TimeEnd;
                }

                CmdList.SetTimeView(timeViewBegin, timeViewEnd, intvType, ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        //+
        private void butViewPeriodToMin_Click(object sender, EventArgs e)
        {
            CmdList.SetTimeView(_viewTimeBegin, _viewTimeBegin.AddSeconds(MinViewTimeInterval), DateTimeIntervalType.NotSet, 
                                 ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType);
        }

        //+
        private void butViewPeriodToMax_Click(object sender, EventArgs e)
        {
            CmdList.SetTimeView(TimeBegin, TimeEnd, DateTimeIntervalType.NotSet, 
                                 ViewTimeBegin, ViewTimeEnd, _axisXScaleViewSizeType);
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

        private void cbViewPeriodStep_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) cbViewPeriodStep_Leave(null, null);
        }

        private void cbViewPeriodStep_Leave(object sender, EventArgs e)
        {
            if (cbViewPeriodStep.Text.Last() != '%') cbViewPeriodStep.Text += @"%";
        }

        //ХЗ для чего слелующие три ф-ции
        private void cbViewPeriod_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                butViewPeriodApply_Click(null, null);
        }

        private void cbViewPeriod_Leave(object sender, EventArgs e)
        {
            cbViewPeriod.Select(0, 0); //Отображения числа со старших разрядов
        }

        private void cbViewPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbViewPeriod_Leave(sender, e);
        }
    #endregion ControlFunction Time

    #region ControlFunction Value
        private void butCurScaleAuto_Click(object sender, EventArgs e)
        {
            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog))
            {
                var aGr = (AnalogGraphic)gr;
                SetGraphicScaleAuto(aGr, aGr.GroupY.IsInPercent);
                CurGraphicNum = gr.Num;
            }
        }
        
        private void butCurScaleByMinMax_Click(object sender, EventArgs e)
        {
            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog))
            {
                var aGr = (AnalogGraphic) gr;
                SetGraphicScaleByMinMax(aGr, aGr.GroupY.IsInPercent);
                CurGraphicNum = gr.Num;
            }
        }

        private void butCurInPrc_Click(object sender, EventArgs e)
        {
            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog))
            {
                if (butCurInPrc.Text != "В проценты")
                {
                    SetGraphicInPercent((AnalogGraphic) gr, false);
                    //butCurInPrc.Text = "В проценты";
                    //labCurUnits.Text = gr.Param.Units;
                }
                else
                {
                    SetGraphicInPercent((AnalogGraphic) gr, true);
                    //butCurInPrc.Text = "В единицы изм.";
                    //labCurUnits.Text = "%";
                }

                CurGraphicNum = gr.Num;
            }
        }

        private void butAllScaleAuto_Click(object sender, EventArgs e)
        {
            bool fgGroup = false;
            foreach(var axY in GroupsY)
            {
                var aGr = axY.UpperGraphic;
                if (fgGroup) CmdList.ContinueGroup();
                fgGroup = SetGraphicScaleAuto(aGr, aGr.GroupY.IsInPercent) || fgGroup;
            }

            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog)) CurGraphicNum = gr.Num;
        }

        private void butAllScaleByMinMax_Click(object sender, EventArgs e)
        {
            bool fgGroup = false;
            foreach (var axY in GroupsY)
            {
                var aGr = axY.UpperGraphic;
                if (fgGroup) CmdList.ContinueGroup();
                fgGroup = SetGraphicScaleByMinMax(aGr, aGr.GroupY.IsInPercent) || fgGroup;
            }

            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog)) CurGraphicNum = gr.Num;
        }

        private void butAllInPrc_Click(object sender, EventArgs e)
        {
            bool fgGroup = false;
            foreach (var axY in GroupsY)
            {
                var aGr = axY.UpperGraphic;
                if (fgGroup) CmdList.ContinueGroup();
                fgGroup = SetGraphicInPercent(aGr, true) || fgGroup;
            }

            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog)) CurGraphicNum = gr.Num;
        }

        private void butAllInUnits_Click(object sender, EventArgs e)
        {
            bool fgGroup = false;
            foreach (var axY in GroupsY)
            {
                var aGr = axY.UpperGraphic;
                if (fgGroup) CmdList.ContinueGroup();
                fgGroup = SetGraphicInPercent(aGr, false) || fgGroup;
            }

            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog)) CurGraphicNum = gr.Num;
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
                bool fgGroup = false;
                foreach (var axY in GroupsY)
                {
                    if (axY.IsVisible) //|| (!axY.UpperGraphic.IsHidden)
                    {
                        //axY.CurAxPos = (double) (d*current)/h;
                        //axY.CurAxSize = (current < (Noas - 1)) ? (double) h1/h : (double) (h - d*current)/h;

                        //current++;
                        ////AxisRanking(axY, current);

                        // /*AxisRankingY(axY);
                        //foreach (var gr in axY.Graphics)
                        //{
                        //    AnalogAreaRankingY(gr);
                        //    //AnalogAreaRankingHeight(gr);
                        //}*/
                        //AnalogRankingY(axY);

                        var pos = (double)(d * current) / h;
                        var size = (current < (Noas - 1)) ? (double)h1 / h : (double)(h - d * current) / h;
                        current++;

                        if (fgGroup) CmdList.ContinueGroup();
                        fgGroup = CmdList.AxYChangeSizePos(axY, size, pos) || fgGroup;
                    }
                }
            }
        }

        private void butPositionOverlap_Click(object sender, EventArgs e)
        {
            //int i = 0;
            /*foreach (var axY in GroupsY)
            {
                axY.CurAxSize = 1;
                axY.CurAxPos = 0;
                //AxisRanking(axY, ++i);
                AxisRankingY(axY);
            }

            foreach (var aGr in AnalogGraphics)
            {
                AnalogAreaRankingY(aGr);
                //AnalogAreaRankingHeight(aGr);

            }*/

            //foreach (var axY in GroupsY)
            //{
            //    axY.CurAxSize = 1;
            //    axY.CurAxPos = 0;
            //    AnalogRankingY(axY);
            //}

            bool fgGroup = false;
            foreach (var axY in GroupsY)
            {
                if (fgGroup) CmdList.ContinueGroup();
                fgGroup = CmdList.AxYChangeSizePos(axY, 1, 0) || fgGroup;
            }
        }

        private void butCurMinMaxApply_Click(object sender, EventArgs e)
        {
            var gr = GetGraphicByNum(CurGraphicNum);
            if ((gr != null) && (gr.IsAnalog))
            {
                SetViewYMinMax((AnalogGraphic) gr, tbCurMin.Text, tbCurMax.Text);
            }
        }

        private void tbCurMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                butCurMinMaxApply_Click(sender, e);
        }

        private void tbCurMax_Leave(object sender, EventArgs e)
        {
            try
            {
                var gr = (AnalogGraphic)GetGraphicByNum(CurGraphicNum);

                if (tbCurMin.Text == "") tbCurMin.Text = gr.GroupY.TxtBoxMin.Text;
                if (tbCurMax.Text == "" || (double.Parse(tbCurMin.Text) >= double.Parse(tbCurMax.Text)))
                    tbCurMax.Text = gr.GroupY.TxtBoxMax.Text;

                tbCurMax.Select(0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ой " + ex.Message);
                //Error = new ErrorCommand("Ошибка вода текста (TextBox1Leave)", ex);
            }
        }

        private void tbCurMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) 
                butCurMinMaxApply_Click(sender, e);
        }

        private void tbCurMin_Leave(object sender, EventArgs e)
        {
            try
            {
                var gr = (AnalogGraphic) GetGraphicByNum(CurGraphicNum);

                if (tbCurMax.Text == "") tbCurMax.Text = gr.GroupY.TxtBoxMax.Text;
                if (tbCurMin.Text == "" || (double.Parse(tbCurMin.Text) >= double.Parse(tbCurMax.Text)))
                    tbCurMin.Text = gr.GroupY.TxtBoxMin.Text;

                tbCurMin.Select(0, 0); //ХЗ для чего это
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ой " + ex.Message);
                //Error = new ErrorCommand("Ошибка вода текста (TextBox1Leave)", ex);
            }
        }

        private void butCtrlHide_Click(object sender, EventArgs e)
        {
            var gr = GetGraphicByNum(CurGraphicNum);
            if (gr.IsVisible)
                CmdList.GraphicHide(gr);
            else
                CmdList.GraphicShow(gr);
        }

        private void butCtrlDel_Click(object sender, EventArgs e)
        {
            DialogResult yResult = MessageBox.Show(@"Удалить график №" + CurGraphicNum + @"?",
                                                    "Запрос на удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            
            if ((CurGraphicNum > 0) && (yResult == DialogResult.OK))
                DeleteGraphicByNum(CurGraphicNum);
        }
    #endregion ControlFunction Value

    #region ControlFunction Control
        //Толщина линии графиков
        private void butLineWidthCur_Click(object sender, EventArgs e)
        {
            var gr = CurrentGraphic;
            int lineWidth = CbLineWidth;
            CmdList.SetLineWidth(gr, lineWidth);
        }

        private void butLineWidthAll_Click(object sender, EventArgs e)
        {
            int lineWidth = CbLineWidth;
            //CmdList.SetLineWidth(Graphics, lineWidth);

            bool fgGroup = false;
            foreach (var gr in Graphics)
            {
                if (fgGroup) CmdList.ContinueGroup();
                fgGroup = CmdList.SetLineWidth(gr, lineWidth) || fgGroup;
            }
        }

        private void cbLineWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) butLineWidthCur_Click(sender, e);
        }

        private void butPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void butPrinterSettings_Click(object sender, EventArgs e)
        {
            ShowPrinterSettings();
        }

        private void butPageSetup_Click(object sender, EventArgs e)
        {
            ShowPrintPageSetup();
        }

        private void cbPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            _printDoc.DefaultPageSettings.PrinterSettings.PrinterName = cbPrinters.Text;
        }

        private void butUndo_Click(object sender, EventArgs e)
        {
            CmdList.Undo();
        }

        private void butRedo_Click(object sender, EventArgs e)
        {
            CmdList.Redo();
        }
    #endregion ControlFunction Control
        
    #region ControlFunction Cursor +
        //При установке визира всплывает подсказка относительно текущей точки текущего графика
        /*private void CursorPositionView(object sender, CursorEventArgs e)
        {
            try
            {
                Graphic gr = GetGraphicByNum(CurGraphicNum);

                var axX = BackGround;

                tbVizirTime.Text =
                    DateTime.FromOADate(axX.Area.CursorX.Position).TimeOfDay.ToString("hh':'mm':'ss','fff");
                tbVizirDate.Text = (DateTime.FromOADate(axX.Area.CursorX.Position)).Date.ToString("dd'.'MM'.'yyyy");

                string decPlacesTemplate = gr.IsDiscret ? "0" : ((AnalogGraphic)gr).GroupY.DecPlacesMask;

                string st;
                if (gr.IsAnalog)
                {
                    MomentReal val = ((AnalogGraphic)gr).DotAt(e.ChartArea.CursorX.Position);
                    st = val.Mean.ToString(decPlacesTemplate) + "\n" +
                         gr.Param.ValueToPercent(val.Mean).ToString(decPlacesTemplate);
                    st += "%";
                }
                else
                {
                    MomentBoolean val = ((DiscretGraphic)gr).DotAt(e.ChartArea.CursorX.Position);
                    //st = val.Mean.ToString() + "\n" +
                    //     gr.Param.ValueToPercent(Convert.ToDouble(val.Mean)).ToString(decPlacesTemplate);
                    st = val.Mean.ToString();
                }

                string tempStr = string.Format("График №" + CurGraphicNum + "\n" + st +
                                               "\n{0:dd.MM.yyyy}\n{1:hh':'mm':'ss','fff}",
                                               (DateTime.FromOADate(e.ChartArea.CursorX.Position)).Date,
                                               (DateTime.FromOADate(e.ChartArea.CursorX.Position)).TimeOfDay);
                toolTip.SetToolTip(chartMain, tempStr);
            }
            catch (Exception ex)
            {
                //Error = new ErrorCommand("При увеличении масштаба в графике возникает конфликт с визиром", ex);
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }*/
        
        //При установке визира записывает инфу в датагрид и в строку Время визира
        /*private void CursorPositionGridView(object sender, CursorEventArgs e)
        {
            try
            {
                GraphicVisual backParam;
                switch (e.ChartArea.Name)
                {
                    case "ChartAreaAnalogBG":
                        backParam = _analogBackGround;
                        break;
                    case "ChartAreaDiscretBG":
                        backParam = _discretBackGround;
                        break;
                }

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    int num = int.Parse(row.Cells[1].Value.ToString());
                    Graphic gr = GetGraphicByNum(num);

                    if (gr.IsAnalog)
                    {
                        MomentReal val = ((AnalogGraphic) gr).DotAt(e.ChartArea.CursorX.Position);
                        if (val != null)
                        {
                            string decPlacesMask = ((AnalogGraphic) gr).GroupY.DecPlacesMask;

                            row.Cells["Визир"].Value = !((AnalogGraphic) gr).GroupY.IsInPercent
                                                           ? val.Mean.ToString(decPlacesMask)
                                                           : gr.Param.PercentToValue(val.Mean).ToString(decPlacesMask);
                            row.Cells["Недост."].Value = val.Nd.ToString();
                        }
                        else
                        {
                            row.Cells["Визир"].Value = null;
                            row.Cells["Недост."].Value = null;
                        }
                    }
                    else
                    {
                        MomentBoolean val = ((DiscretGraphic) gr).DotAt(e.ChartArea.CursorX.Position);
                        if (val != null)
                        {
                            row.Cells["Визир"].Value = val.Mean.ToString();
                            row.Cells["Недост."].Value = val.Nd.ToString();
                        }
                        else
                        {
                            row.Cells["Визир"].Value = null;
                            row.Cells["Недост."].Value = null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Error = new ErrorCommand("При попадании курсора вне области построения всплывающее окно не появляется", ex);
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    row.Cells["Визир"].Value = "";
                    //row.Cells["Время визира"].Value = "";
                }
            }
        }*/
        
        //Синхронизация визиров
        private void CursorSynch(object sender, CursorEventArgs e)
        {
            const double csMinApprox = .03; //минимальное приближение

            try
            {
                if ((_analogBackGround != null) && (_discretBackGround != null))
                {
                    if (e.Axis.AxisName == AxisName.X)
                    {
                        //чтобы не моргал визир
                        if (e.ChartArea.Name == "ChartAreaAnalogBG")
                        {
                            if (Math.Abs(_discretBackGround.Area.CursorX.SelectionEnd - _discretBackGround.Area.CursorX.SelectionStart) <
                                csMinApprox * (e.ChartArea.AxisX.ScaleView.ViewMaximum - e.ChartArea.AxisX.ScaleView.ViewMinimum))
                            {
                                _discretBackGround.Area.CursorX.SelectionStart = double.NaN;
                                //_discretBackGround.Area.CursorX.SelectionEnd = double.NaN;
                            }

                            _discretBackGround.Area.CursorX.SelectionStart = _analogBackGround.Area.CursorX.SelectionStart;
                            _discretBackGround.Area.CursorX.SelectionEnd = _analogBackGround.Area.CursorX.SelectionEnd;
                        }
                        else //"ChartAreaDiscretBG"
                        {
                            if (Math.Abs(_analogBackGround.Area.CursorX.SelectionEnd - _analogBackGround.Area.CursorX.SelectionStart) <
                                csMinApprox * (e.ChartArea.AxisX.ScaleView.ViewMaximum - e.ChartArea.AxisX.ScaleView.ViewMinimum))
                            {
                                _analogBackGround.Area.CursorX.SelectionStart = double.NaN;
                                //_analogBackGround.Area.CursorX.SelectionEnd = double.NaN;
                            }

                            _analogBackGround.Area.CursorX.SelectionStart = _discretBackGround.Area.CursorX.SelectionStart;
                            _analogBackGround.Area.CursorX.SelectionEnd = _discretBackGround.Area.CursorX.SelectionEnd;
                        }

                        _discretBackGround.Area.CursorX.Position = e.NewPosition;
                        _analogBackGround.Area.CursorX.Position = e.NewPosition;
                    }
                }
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
                if ((_analogBackGround != null) && (_discretBackGround != null))
                {
                    if (e.Axis.AxisName == AxisName.X)
                    {
                        _discretBackGround.Area.CursorX.Position = e.NewPosition;
                        _analogBackGround.Area.CursorX.Position = e.NewPosition;
                        _analogBackGround.Area.CursorX.SelectionStart = double.NaN;
                        //_analogBackGround.Area.CursorX.SelectionEnd = double.NaN;
                        _discretBackGround.Area.CursorX.SelectionStart = double.NaN;
                        //_discretBackGround.Area.CursorX.SelectionEnd = double.NaN;
                    }
                    chartMain.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        //При установке курсора меняет совйства визира
        private void CursorPositionChanged(object sender, CursorEventArgs e)
        {
            if (!double.IsNaN(e.NewPosition))
            {
                var vizirTime = DateTime.FromOADate(e.NewPosition);
                CmdList.VizirChanged(vizirTime, _vizirTime, ViewTimeBegin, ViewTimeEnd, _axisXIntervalType);
            }
        }
    #endregion ControlFunction Cursor

    #region ControlFunction AxY
        //Resize оси Y
        internal void AxYResize(GroupY axY)
        {
            int h = AnalogAxisMaxHeight;
            //if (h < GroupsY.First().Ax.MinimumSize.Height) h = GroupsY.First().Ax.MinimumSize.Height;

            axY.CurAxSize = ((double) axY.Ax.Height)/h;
            axY.CurAxPos = ((double) axY.Ax.Top)/h;
            
            foreach (var gr in axY.Graphics)
            {
                AnalogAreaRankingY(gr); //бесполезно, если сначала менять высоту, а потом координату
                AnalogAreaRankingHeight(gr);
            }
        }

        //Разворачивание оси по миксимуму
        internal void AxToAllHeight(GroupY axY)
        {
            //axY.CurAxSize = 1;
            //axY.CurAxPos = 0;
            ///*AxisRankingY(axY);
            //foreach (var grOv in axY.Graphics)
            //{
            //    AnalogAreaRankingY(grOv);
            //}*/
            //AnalogRankingY(axY);

            CmdList.AxYChangeSizePos(axY, 1, 0);
        }

        internal void SelectCursorYSelectionStart(AnalogGraphic gr, double val)
        {
            try
            {
                gr.Area.CursorY.SelectionStart = val;
            }
            catch (Exception e)
            {
            }
        }

        internal void SelectCursorYSelectionEnd(AnalogGraphic gr, double val)
        {
            try
            {
                double valEnd = Math.Abs(val - gr.Area.CursorY.SelectionStart) > 0.05*gr.Area.AxisY.ScaleView.Size
                                    ? val : gr.Area.CursorY.SelectionStart;
                gr.Area.CursorY.SelectionEnd = valEnd;
            }
            catch (Exception)
            {
            }
        }

        internal void SelectCursorYSelectionEnter(AnalogGraphic gr, double val)
        {
            //0.00001
            if ((Math.Abs(val - gr.Area.CursorY.SelectionStart) > MinApprox * (gr.Area.AxisY.Maximum - gr.Area.AxisY.Minimum)) &&
                (Math.Abs(val - gr.Area.CursorY.SelectionStart) > 0.05 * gr.Area.AxisY.ScaleView.Size))
            {
                SelectCursorYSelectionEnd(gr, val);

                double tmin = Math.Min(gr.Area.CursorY.SelectionStart, gr.Area.CursorY.SelectionEnd);
                double tmax = Math.Max(gr.Area.CursorY.SelectionStart, gr.Area.CursorY.SelectionEnd);
                
                if(!gr.GroupY.IsInPercent)
                {
                    //mm tmin = GetReduceValue(tmin, gr.Param.Min, gr.Param.Max, gr.Param.DecPlaces);
                    tmin = GetReduceValue(tmin, gr.MinViewValue, gr.MaxViewValue, gr.Param.DecPlaces);
                    //mm tmax = GetReduceValue(tmax, gr.Param.Min, gr.Param.Max, gr.Param.DecPlaces);
                    tmax = GetReduceValue(tmax, gr.MinViewValue, gr.MaxViewValue, gr.Param.DecPlaces);
                }
                else
                {
                    //mm tmin = GetReduceValue(gr.Param.ValueToPercent(tmin), 0, 100, 0);
                    tmin = GetReduceValue(gr.ValueToPercent(tmin), 0, 100, 0);
                    //mm tmax = GetReduceValue(gr.Param.ValueToPercent(tmax), 0, 100, 0);
                    tmax = GetReduceValue(gr.ValueToPercent(tmax), 0, 100, 0);
                }

                if ((tmin > gr.GroupY.ViewMin) && (tmax < gr.GroupY.ViewMax))
                    SetViewYMinMax(gr, tmin, tmax, gr.GroupY.IsInPercent);
            }

            gr.Area.CursorY.SelectionEnd = double.NaN;
        }

        internal void AxYMoveV(GroupY axY, int mouseY, int mouseYStart)
        {
            if ((axY.Ax.Top + mouseY) - mouseYStart > 0)
            {
                if ((axY.Ax.Bottom + mouseY) - mouseYStart <= AnalogAxisMaxHeight)
                {
                    axY.Ax.Top = (axY.Ax.Top + mouseY) - mouseYStart;
                }
                else axY.Ax.Top = AnalogAxisMaxHeight - axY.Ax.Height;
            }
            else axY.Ax.Top = 0;
            AxYResize(axY);
        }

        internal void AxYTryMoveH(GroupY axY, int mouseX, int mouseY, int ad)
        {
            double xD = ((double)(mouseX + axY.Ax.Left) / AxWidth + 1);
            if (xD <= Noas + 1)
            {
                int current = 0;

                foreach (var ax in GroupsY)
                {
                    if ((ax.Ax.Left / AxWidth + 1 < xD) && (ax.Ax.Right / AxWidth + 1 >= xD) && (ax.Ax.Visible))
                    {
                        current = ax.UpperGraphic.Num;
                        break;
                    }
                }

                if ((current > 0) && (CurGraphicNum != current) && 
                    (mouseY + axY.Ax.Top + ad <= ((AnalogGraphic)GetGraphicByNum(current)).GroupY.Ax.Bottom) && 
                    (mouseY + axY.Ax.Top + ad >= ((AnalogGraphic)GetGraphicByNum(current)).GroupY.Ax.Top))
                    Cursor = (axY.UpperGraphic.Num == current) ? Cursors.SizeAll : Cursors.Cross;
                else Cursor = Cursors.SizeAll;
            }
            else Cursor = Cursors.SizeAll;
        }

        internal void AxYMoveH(GroupY axY, int mouseX, int mouseY, int ad, bool move = true)
        {
            Cursor = Cursors.Default;
            
            if(move)
            {
                double xD = ((double)(mouseX + axY.Ax.Left) / AxWidth + 1);
                if (xD <= Noas + 1)
                {
                    int current = 0;

                    foreach (var ax in GroupsY)
                    {
                        if ((ax.Ax.Left / AxWidth + 1 < xD) && (ax.Ax.Right / AxWidth + 1 >= xD) && (ax.Ax.Visible))
                        {
                            current = ax.UpperGraphic.Num;
                            break;
                        }
                    }

                    if ((current > 0) && (CurGraphicNum != current) &&
                        (mouseY + axY.Ax.Top + ad <= ((AnalogGraphic)GetGraphicByNum(current)).GroupY.Ax.Bottom) &&
                        (mouseY + axY.Ax.Top + ad >= ((AnalogGraphic)GetGraphicByNum(current)).GroupY.Ax.Top))
                    {
                        //MessageBox.Show(@"Amalgamation " + axY.UpperGraphic.Num + @" в " + current);
                        var catcher = ((AnalogGraphic) GetGraphicByNum(current)).GroupY;
                        
                        if (catcher == axY)
                        {
                            var gr = (AnalogGraphic) GetGraphicByNum(_dividedGraphicNum);
                            if (gr.GroupY.IsOverlayed)
                                //ChangeGraphicAxY(gr, catcher);
                                CmdList.GraphicChangeAxY(gr, catcher);
                            else
                                //AxJunction(catcher, gr.GroupY);
                                CmdList.AxYJunction(catcher, gr.GroupY);

                            contextMenuStrip.Hide();
                        }
                        else
                            //AxJunction(catcher, axY);
                            CmdList.AxYJunction(catcher, axY);
                    }
                    //~dataGridView1.CurrentCell = WantedRow(CurrentParamNumber).Cells[1];
                }
            }
        }

        internal void AxYChangeTop(GroupY axY, int mouseY, int mouseH)
        {
            if ((axY.Ax.Top + mouseY) >= 0)
            {
                axY.Ax.Height = axY.Ax.Bottom - mouseY - axY.Ax.Top;
                if ((axY.Ax.Bottom + mouseY) <= AnalogAxisMaxHeight)
                    axY.Ax.Top = (axY.Ax.Top + mouseY);
                else
                    axY.Ax.Top = AnalogAxisMaxHeight - axY.Ax.Height;
            }
            else
            {
                axY.Ax.Top = 0;
                axY.Ax.Height = mouseH;
            }

            AxYResize(axY);
        }

        internal void AxYChangeBottom(GroupY axY, int mouseY, int mouseYStart)
        {
            if ((axY.Ax.Bottom + mouseY) - mouseYStart <= AnalogAxisMaxHeight)
                axY.Ax.Height = axY.Ax.Bottom + mouseY - axY.Ax.Top;
            else
                axY.Ax.Height = AnalogAxisMaxHeight - axY.Ax.Top;

            AxYResize(axY);
        }

        //Установка высоты Area аналогового графика (при прокручивании на оси)
        private void AnalogAreaResizeRankingHeight(AnalogGraphic aGr, int dTop, int dBottom)
        {
            int h = aGr.GroupY.Ax.Size.Height - AxSummand - dTop - dBottom;
            aGr.Area.Position.Height = h * 100f / chartMain.Height;

            float h1 = aGr.GroupY.GraphicAreaBottom - AxSummand - dTop - dBottom;
            aGr.Area.InnerPlotPosition.Y = 0;
            aGr.Area.InnerPlotPosition.Height = h1 / h * 100f;

            aGr.Area.InnerPlotPosition.X = 0;
            aGr.Area.InnerPlotPosition.Width = 100;
        }

        //Установка высоты Area аналогового графика (при прокручивании на оси)
        private void AnalogAreaResizeRankingY(AnalogGraphic aGr, int dTop)
        {
            aGr.Area.Position.Y = 100f * (aGr.GroupY.Ax.Top + AxSummand + dTop) / chartMain.Height;
        }

        internal void AnalogResizeRankingY(GroupY axY, int dTop, int dBottom)
        {
            foreach (var gr in axY.Graphics)
            {
                AnalogAreaResizeRankingHeight(gr, dTop, dBottom);
                AnalogAreaResizeRankingY(gr, dTop);
            }
        }

        internal void ShowMenuOverlayedGraphics(GroupY axY)
        {
            contextMenuStrip.Items.Clear();
            
            var grOv = new SortedSet<int>();
            foreach (var gr in axY.Graphics)
            {
                grOv.Add(gr.Num);
            }

            foreach (var num in grOv){
                var item = contextMenuStrip.Items.Add(num.ToString());
                item.MouseDown += contextMenuStrip_MouseDown;
                item.ForeColor = GetGraphicByNum(num).Series.Color;
                item.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold,
                                                           GraphicsUnit.Point, ((204)));

                item.Click += contextMenuStripItem_Click;
            }
            contextMenuStrip.Show(axY.AxCap, new Point(0, axY.AxCap.Height));
        }

        internal void contextMenuStripItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem) sender;
            var num = int.Parse(item.Text);
            var gr = (AnalogGraphic) GetGraphicByNum(num);
            //DataGridGraphicToTop(gr);
            CurGraphicNum = gr.Num;
            //dataGridView1.CurrentCell = WantedRow(cntr1).Cells[2];
            _isMouseDown4Divide = false;
        }

        private void contextMenuStrip_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) _isMouseDown4Divide = true;
            var l1 = (ToolStripMenuItem) sender;
            _dividedGraphicNum = int.Parse(l1.Text);
        }
    #endregion ControlFunction AxY

    #region Hide/Show Axis/Graphic +
        private void HideGraphic(Graphic graphic)
        {
            if (graphic.IsVisible)
            {
                DataGridHideGraphic(graphic);
                graphic.IsVisible = false;

                if (graphic.IsAnalog)
                {
                    var axY = ((AnalogGraphic) graphic).GroupY;

                    AnalogGraphic visGr = null;
                    foreach (var axGr in axY.Graphics)
                    {
                        if ((axGr.IsVisible) && (axGr.Num != graphic.Num))
                        {
                            visGr = axGr;
                            break;
                        }
                    }

                    if (visGr == null)
                    {
                        Noas--;
                        axY.IsVisible = false;
                    }
                    else
                    {
                        //DataGridGraphicToTop(visGr);
                        CurGraphicNum = visGr.Num;
                    }
                }
                else
                {
                    Nods--;
                }

                ReRanking();
                CurGraphicNum = _curGraphicNum;
            }
        }

        private void ShowGraphic(Graphic graphic)
        {
            if (!graphic.IsVisible)
            {
                DataGridShowGraphic(graphic);
                graphic.IsVisible = true;

                if (graphic.IsAnalog)
                {
                    var axY = ((AnalogGraphic) graphic).GroupY;
                    
                    if ((!axY.IsHidden) && (!axY.IsVisible))
                    {
                        Noas++;
                        axY.IsVisible = true;
                    }

                    foreach (var axGr in axY.Graphics)
                    {
                        var row = GetRowByGraphicNum(axGr.Num);
                        row.Cells[0].Style.BackColor = graphic.Series.Color;
                        row.Cells[0].Style.SelectionBackColor = graphic.Series.Color;
                    }

                    DataGridGraphicToTop((AnalogGraphic) graphic);
                }
                else
                {
                    Nods++;
                }

                //CurGraphicNum = gr.Num;
                ReRanking();
            }
        }

        private void HideAxY(GroupY axY)
        {
            if (!axY.IsHidden)
            {
                axY.IsHidden = true;
                Noas--;
                ReRanking();

                foreach(var gr in axY.Graphics)
                {
                    _needToCheckChangeDatagrid = false;
                    GetRowByGraphicNum(gr.Num).Cells[0].Value = false;
                    gr.Area.AxisY.MajorGrid.Enabled = false;
                }
            }
        }

        private void ShowAxY(GroupY axY)
        {
            if (axY.IsHidden)
            {
                axY.IsVisible = true;
                Noas++;
                ReRanking();

                foreach (var gr in axY.Graphics)
                {
                    _needToCheckChangeDatagrid = false;
                    GetRowByGraphicNum(gr.Num).Cells[0].Value = true;
                    gr.Area.AxisY.MajorGrid.Enabled = true;
                }
            }
        }
    #endregion Hide/Show

    #region Axis Junction/Separation
        //Слияниие осей
        //catcher - ось, в которую добавляется другая ось
        //pitcher - добавляемая ось
        private void AxJunction(GroupY catcher, GroupY pitcher)
        {
            try
            {
                //if (catcher == pitcher)
                //{
                //    //? Сделать добавления графика _dividedGraphicNum в ось pitcher
                //    var gr = (AnalogGraphic) GetGraphicByNum(_dividedGraphicNum);
                //    if (gr.GroupY.IsOverlayed)
                //        ChangeGraphicAxY(gr, catcher);
                //    else
                //        AxJunction(catcher, gr.GroupY);

                //    contextMenuStrip.Hide();
                //    return;
                //}

                if (catcher == pitcher) return;

                //Приводим ось append к состоянию оси catcher (перенесено в перерисовку)
                
                //Добавляем графики оси append в ось catcher
                foreach(var gr in pitcher.Graphics)
                {
                    gr.IsAutoScaleY = false;
                    catcher.AddGraphic(gr);
                    //gr.GroupY = catcher;
                }
                
                //int num = catcher.UpperGraphic.Num;
                //foreach (var gr in catcher.Graphics)
                //    if (gr.Num < num) num = gr.Num;

                foreach (var gr in catcher.Graphics)
                    GetRowByGraphicNum(gr.Num).Cells["Group"].Value = catcher.MinGraphicNum; //=num;
                
                //Удаляем ось catcher
                if (pitcher.IsVisible) Noas--;
                chartMain.Controls.Remove(pitcher.Ax);
                GroupsY.Remove(pitcher);
                
                //Перерисовка
                dataGridView.Sort(new DatagridColorSort());
                _groupsY.Sort(new GroupsYSort());
                SetAxYScaleView(catcher, catcher.ViewMin, catcher.ViewMax, catcher.IsInPercent);
                DataGridGraphicToTop(pitcher.UpperGraphic);
                ReRanking();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
            }
        }
        
        //разделение осей
        private void AxSeparation(AnalogGraphic graphic)
        {
            if (graphic.GroupY.IsOverlayed)
            {
                var pitcher = graphic.GroupY;

                //удаляем график из старой оси
                pitcher.RemoveGraphic(graphic);
                
                //добавляем новую ось
                GroupsY.Add(new GroupY(graphic));
                chartMain.Controls.Add(graphic.GroupY.Ax);
                if ((pitcher.IsVisible) && (graphic.IsVisible)) Noas++;
                
                //меняем свойства старой оси
                //int num = pitcher.UpperGraphic.Num;
                //foreach (var gr in pitcher.Graphics)
                //    if (gr.Num < num) num = gr.Num;

                foreach (var gr in pitcher.Graphics)
                    GetRowByGraphicNum(gr.Num).Cells["Group"].Value = pitcher.MinGraphicNum; //=num;

                DataGridGraphicToTop(pitcher.UpperGraphic);

                //меняем свойства новой график
                GetRowByGraphicNum(graphic.Num).Cells["Group"].Value = graphic.Num;
                var row = GetRowByGraphicNum(graphic.Num);
                row.Cells[0].Style.BackColor = graphic.Series.Color;
                row.Cells[0].Style.SelectionBackColor = graphic.Series.Color;
                graphic.GroupY.CurAxPos = pitcher.CurAxPos;
                graphic.GroupY.CurAxSize = pitcher.CurAxSize;
                graphic.GroupY.IsVisible = graphic.IsVisible;
                SetAxYScaleView(graphic, pitcher.ViewMin, pitcher.ViewMax, pitcher.IsInPercent);

                //Перерисовка
                dataGridView.Sort(new DatagridColorSort());
                _groupsY.Sort(new GroupsYSort());
                ReRanking();
            }
        }

        private void AxRestore(GroupY axY, IEnumerable<AnalogGraphic> graphics)
        {
            try
            {
                //Восстанавливаем ось
                if (axY.IsVisible) Noas++;
                chartMain.Controls.Add(axY.Ax);
                GroupsY.Add(axY);
                
                //Переносим графики в ось
                foreach (var gr in graphics)
                {
                    AxYRemoveGraphic(gr);

                    gr.IsAutoScaleY = false;
                    if (!axY.Graphics.Contains(gr))
                        axY.AddGraphic(gr);
                    else
                        gr.GroupY = axY;
                    //gr.GroupY = catcher;
                }

                foreach (var gr in axY.Graphics)
                    GetRowByGraphicNum(gr.Num).Cells["Group"].Value = axY.MinGraphicNum; //=num;

                //Перерисовка
                dataGridView.Sort(new DatagridColorSort());
                _groupsY.Sort(new GroupsYSort());
                SetAxYScaleView(axY, axY.ViewMin, axY.ViewMax, axY.IsInPercent);
                DataGridGraphicToTop(axY.UpperGraphic);
                ReRanking();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void AxYRemoveGraphic(AnalogGraphic graphic)
        {
            var pitcher = graphic.GroupY;
            bool fg = (graphic.Num == pitcher.MinGraphicNum);

            //удаляем график из старой оси
            pitcher.RemoveGraphic(graphic);

            //меняем свойства старой оси
            if (fg)
                foreach (var gr in pitcher.Graphics)
                    GetRowByGraphicNum(gr.Num).Cells["Group"].Value = pitcher.MinGraphicNum;

            DataGridGraphicToTop(pitcher.UpperGraphic);

            //Перерисовка
            //dataGridView.Sort(new DatagridColorSort()); (в функции ChangeGraphicAxY)
        }
        
        private void AxYAddGraphic(GroupY axY, AnalogGraphic graphic)
        {
            try
            {
                graphic.IsAutoScaleY = false;
                
                if (graphic.Num < axY.MinGraphicNum)
                    foreach (var gr in axY.Graphics)
                        GetRowByGraphicNum(gr.Num).Cells["Group"].Value = graphic.Num;
                
                axY.AddGraphic(graphic);
                GetRowByGraphicNum(graphic.Num).Cells["Group"].Value = axY.MinGraphicNum;

                //Перерисовка
                //dataGridView.Sort(new DatagridColorSort()); (в функции ChangeGraphicAxY)
                SetAxYScaleView(graphic, axY.ViewMin, axY.ViewMax, axY.IsInPercent);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void ChangeGraphicAxY(AnalogGraphic graphic, GroupY catcher)
        {
            AxYRemoveGraphic(graphic);
            AxYAddGraphic(catcher, graphic);

            DataGridGraphicToTop(graphic);

            dataGridView.Sort(new DatagridColorSort());
            _groupsY.Sort(new GroupsYSort());
            ReRanking(); //заменить - пересортировывать только оси
        }
    #endregion Junction/Separation

    #region Print
        [DllImport("gdi32.dll")]
        public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
                                         int nXSrc, int nYSrc, int dwRop);
        private Bitmap _memoryImage;

        private readonly PrintDocument _printDoc = new PrintDocument();
        private void PrintDocInit()
        {
            _printDoc.DefaultPageSettings.Landscape = true;
        }

        private void Print()
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
                Size = new Size(2048, 1736);

                Refresh();
                Label[,] indexLabels = new Label[AnalogGraphics.Count, 4];
                Label capL = new Label { Text = Caption, AutoSize = true, BackColor = Color.Transparent };
                Label dateL = new Label { Text = DateTime.Now.ToString(), AutoSize = true, BackColor = Color.Transparent };
                if (chbPrintLabels.Checked)
                {
                    //for (int indexP = 0; indexP < AnalogGraphics.Count - 1; indexP++)
                    int indexP = -1;
                    foreach(var aGr in AnalogGraphics)
                    {
                        indexP++;
                        if (aGr.IsVisible)
                            for (int indexN = 0; indexN < 4; indexN++)
                            {
                                indexLabels[indexP, indexN] = new Label
                                {
                                    Text = aGr.Num.ToString(),
                                    BackColor = Color.Transparent,
                                    AutoSize = true,
                                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204)))
                                };
                                chartMain.Controls.Add(indexLabels[indexP, indexN]);
                                double labelXPosition;
                                if (indexN < 3)
                                    labelXPosition = ((aGr.Area.Position.X * chartMain.Width / 100) +
                                                      (indexN + .2) * (aGr.Area.Position.Width * chartMain.Width / 330)
                                                      + (-3 + indexP) * 15);
                                else
                                    labelXPosition = ((aGr.Area.Position.X * chartMain.Width / 100) +
                                                      (indexN) * (aGr.Area.Position.Width * chartMain.Width / 320)
                                                      + (-2 - AnalogGraphics.Count + indexP) * 15);
                                try
                                {
                                    double labelXValue = aGr.Area.AxisX.PixelPositionToValue(labelXPosition);
                                    MomentValue xValue = aGr.Dots.FindLast(x => (x.Time.ToOADate() <= labelXValue));
                                    var m = xValue == null ? 0 : xValue.ToMomentReal().Mean;
                                    double labelYPosition = aGr.Area.AxisY.ValueToPixelPosition(m);
                                    indexLabels[indexP, indexN].Top = (int)(Math.Round(labelYPosition)) - 5;
                                    indexLabels[indexP, indexN].Left = (int)Math.Round(labelXPosition) - 8;
                                    if (labelYPosition + 35 > aGr.Area.Position.Bottom * chartMain.Height / 100 ||
                                        labelYPosition < aGr.Area.Position.Y * chartMain.Height / 100)
                                        indexLabels[indexP, indexN].Text = "";
                                    //indexLabels[indexP, indexN].Text = (DateTime.FromOADate(
                                    //    curP.Area.AxisX.PixelPositionToValue(0))).ToString();        
                                }
                                catch { }
                            }
                    }
                }

                chartMain.Controls.Add(dateL);
                dateL.Location = new Point((int)((BackGround.Area.Position.X * chartMain.Width / 100) +
                                                  (BackGround.Area.Position.Width * chartMain.Width / 100) - 150), 0);
                chartMain.Controls.Add(capL);
                capL.Location = new Point((int)((BackGround.Area.Position.X * chartMain.Width / 100) + 20), 0);

                CaptureP();
                if (chbPrintLabels.Checked)
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
                //var ppd = new PrintDocument();
                _printDoc.PrintPage += PrintDocument_PrintPage;
                //ppd.Document.PrintPage += PrintDocument1PrintPage;
                //ppd.Document.DefaultPageSettings.Landscape = true;
                
                //_ppd.DefaultPageSettings.Landscape = true;
                //_ppd.DefaultPageSettings.PrinterSettings.PrinterName = cbPrinters.Text;
                
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

                //var prdlg = new PrintDialog { Document = ppd };
                //prdlg.ShowDialog();
                //var pgstdlg = new PageSetupDialog {Document = ppd};
                //pgstdlg.ShowDialog();

                var prewdlg = new PrintPreviewDialog {Document = _printDoc};
                prewdlg.ShowDialog();
                //ppd.Print();
            }
            catch (Exception ex)
            {
                //~ Error = new ErrorCommand("Ошибка печати (Button3Click)", ex);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
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
            Graphics mGraphics = splitContainerV.CreateGraphics();
            _memoryImage = new Bitmap(splitContainerV.Width, splitContainerV.Height, mGraphics);
            splitContainerV.DrawToBitmap(_memoryImage, new Rectangle(0, 0, splitContainerV.Width, splitContainerV.Height));
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(_memoryImage, 40, 40, e.PageSettings.PrintableArea.Height - 70,
                                 e.PageSettings.PrintableArea.Width - 70);
        }

        private void InitPrinters()
        {
            PrinterSettings.StringCollection sc = PrinterSettings.InstalledPrinters;
            foreach(var pr in sc)
            {
                var it = cbPrinters.Items.Add(pr.ToString());
                if (new PrinterSettings { PrinterName = pr.ToString() }.IsDefaultPrinter)
                    cbPrinters.SelectedIndex = it;
            }
        }

        private void ShowPrinterSettings()
        {
            var prdlg = new PrintDialog { Document = _printDoc };
            prdlg.ShowDialog();

            cbPrinters.Text = _printDoc.DefaultPageSettings.PrinterSettings.PrinterName;
        }

        private void ShowPrintPageSetup()
        {
            var pgstdlg = new PageSetupDialog {Document = _printDoc};
            pgstdlg.ShowDialog();
        }
    #endregion Print

    #region Vizir +
        private void SetVizirTextBoxes(DateTime vizirTime)
        {
            tbVizirTime.Text = vizirTime.TimeOfDay.ToString("hh':'mm':'ss','fff");
            tbVizirDate.Text = vizirTime.Date.ToString("dd'.'MM'.'yyyy");
        }

        private void SetVizirToolTip(DateTime vizirTime)
        {
            Graphic gr = GetGraphicByNum(CurGraphicNum);
            string decPlacesTemplate = gr.IsDiscret ? "0" : ((AnalogGraphic) gr).GroupY.DecPlacesMask;

            string st;
            if (gr.IsAnalog)
            {
                MomentReal val = ((AnalogGraphic) gr).DotAt(vizirTime);
                st = val.Mean.ToString(decPlacesTemplate) + "\n" +
                     //mm gr.Param.ValueToPercent(val.Mean).ToString(decPlacesTemplate);
                     gr.ValueToPercent(val.Mean).ToString(decPlacesTemplate);
                st += "%";
            }
            else
            {
                MomentBoolean val = ((DiscretGraphic) gr).DotAt(vizirTime);
                //st = val.Mean.ToString() + "\n" +
                //     gr.Param.ValueToPercent(Convert.ToDouble(val.Mean)).ToString(decPlacesTemplate);
                st = val.Mean.ToString();
            }

            string tempStr = string.Format("График №" + CurGraphicNum + "\n" + st +
                                           "\n{0:dd.MM.yyyy}\n{1:hh':'mm':'ss','fff}",
                                           vizirTime.Date, vizirTime.TimeOfDay);
            toolTip.SetToolTip(chartMain, tempStr);
        }

        private void SetVizirDataGridView(DateTime vizirTime)
        {
            //try
            //{
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                int num = int.Parse(row.Cells[1].Value.ToString());
                Graphic gr = GetGraphicByNum(num);

                if (gr.IsAnalog)
                {
                    MomentReal val = ((AnalogGraphic)gr).DotAt(vizirTime);
                    if (val != null)
                    {
                        string decPlacesMask = ((AnalogGraphic)gr).GroupY.DecPlacesMask;

                        row.Cells["Визир"].Value = !((AnalogGraphic)gr).GroupY.IsInPercent
                                                       ? val.Mean.ToString(decPlacesMask)
                                                       //mm : gr.Param.PercentToValue(val.Mean).ToString(decPlacesMask);
                                                       : gr.PercentToValue(val.Mean).ToString(decPlacesMask);
                        row.Cells["Недост."].Value = val.Nd.ToString();
                    }
                    else
                    {
                        row.Cells["Визир"].Value = null;
                        row.Cells["Недост."].Value = null;
                    }
                }
                else
                {
                    MomentBoolean val = ((DiscretGraphic)gr).DotAt(vizirTime);
                    if (val != null)
                    {
                        row.Cells["Визир"].Value = (Convert.ToInt32(val.Mean)).ToString();
                        row.Cells["Недост."].Value = val.Nd.ToString();
                    }
                    else
                    {
                        row.Cells["Визир"].Value = null;
                        row.Cells["Недост."].Value = null;
                    }
                }
            }
            //}
            //catch (Exception e)
            //{
            //    foreach (DataGridViewRow row in dataGridView.Rows)
            //    {
            //        row.Cells["Визир"].Value = null; //было = "";
            //        row.Cells["Недост."].Value = null;
            //        //row.Cells["Время визира"].Value = "";
            //    }

            //    SetError("SetVizirDataGridView", e);
            //}
        }

        private void UnsetVizirTextBoxes()
        {
            tbVizirTime.Text = "";
            tbVizirDate.Text = "";
        }

        private void UnsetVizirDataGridView()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                row.Cells["Визир"].Value = null;
                row.Cells["Недост."].Value = null;
            }
        }

        private void VizirChanged(DateTime vizirTime)
        {
            //Инфо
            SetVizirTextBoxes(vizirTime);
            //всплывающая подсказка
            SetVizirToolTip(vizirTime);
            //DataGrid
            SetVizirDataGridView(vizirTime);
            _vizirTime = vizirTime;

            SetError("VizirChanged", null);
        }
    #endregion Vizir

    #region Vizir Next/Prev +
        //Путешествие визира по времени с кнопочек
        //WalkColorReset
        private void VizirLabelsColorReset(object sender, EventArgs e)
        {
            labVizirL.BackColor = Color.White;
            labVizirR.BackColor = Color.White;
            labVizirL.Text = "";
            labVizirR.Text = "";
        }

        private MomentValue GetVizirNextDot(Graphic gr)
        {
            MomentValue nextDot = null;

            if ((gr.DotCount > 0) && (BackGround.Area.CursorX.Position > 0))
            {
                var curTime = DateTime.FromOADate(BackGround.Area.CursorX.Position);
                MomentValue curDot = null;

                int i;
                for (i = 0; i < gr.DotCount; i++)
                {
                    if (gr.Dot(i).Time > curTime) break;
                    curDot = gr.Dot(i);
                }

                if (curDot != null)
                {
                    for (; i < gr.DotCount; i++)
                    {
                        var dot = gr.Dot(i);
                        if (dot.Time > TimeEnd) break;

                        if ((dot.ToMomentReal().Mean != curDot.ToMomentReal().Mean) || (dot.Nd != curDot.Nd))
                        {
                            nextDot = dot;
                            break;
                        }
                    }
                }
                else
                {
                    nextDot = gr.Dot(0);
                }
            }

            return nextDot;
        }

        private MomentValue GetVizirPrevDot(Graphic gr)
        {
            MomentValue prevDot = null;

            if ((gr.DotCount > 0) && (BackGround.Area.CursorX.Position > 0))
            {
                var curTime = DateTime.FromOADate(BackGround.Area.CursorX.Position);
                MomentValue curDot = null;

                int i;
                for (i = 0; i < gr.DotCount; i++)
                {
                    if (gr.Dot(i).Time >= curTime) break;
                    curDot = gr.Dot(i);
                }

                if (curDot != null)
                {
                    i--;
                    i--;

                    if (i == -1) prevDot = gr.Dot(0);

                    for (; i >= 0; i--)
                    {
                        var dot = gr.Dot(i);
                        if ((dot.ToMomentReal().Mean != curDot.ToMomentReal().Mean) || (dot.Nd != curDot.Nd))
                        {
                            prevDot = gr.Dot(i + 1);
                            break;
                        }

                        if (i == 0) prevDot = gr.Dot(0);
                    }
                }

                if ((prevDot != null) && (prevDot.Time < TimeBegin)) prevDot = null;
            }

            return prevDot;
        }
        
        //ButtonTimeWalkAhead
        private void VizirNextCur(object sender, EventArgs e)
        {
            //try
            //{
            var gr = GetGraphicByNum(CurGraphicNum);

            if ((gr.DotCount > 0) && (BackGround.Area.CursorX.Position > 0))
            {
                var nextDot = GetVizirNextDot(gr);
                var nextTime = (nextDot != null) ? nextDot.Time : TimeEnd;

                CmdList.SetVizir(nextTime, _vizirTime, ViewTimeBegin, ViewTimeEnd, _axisXIntervalType);

                labVizirR.BackColor = gr.Series.Color;
                labVizirR.Text = gr.Num.ToString();
                labVizirL.BackColor = Color.White;
            }
            //}
            //catch (Exception ex)
            //{
            //    //timerHold.Stop();
            //    //MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
            //    //Error = new ErrorCommand("", ex);
            //}
        }

        //ButtonTimeStepAhead
        private void VizirNextAll(object sender, EventArgs e)
        {
            //try
            //{
            if (BackGround.Area.CursorX.Position > 0)
            {
                MomentValue nextDot = null;
                Graphic nextGr = null;

                foreach (var gr in Graphics)
                {
                    var dot = GetVizirNextDot(gr);
                    if (dot != null)
                    {
                        if ((nextDot == null) || (dot.Time < nextDot.Time))
                        {
                            nextDot = dot;
                            nextGr = gr;
                        }
                    }
                }

                var nextTime = (nextDot != null) ? nextDot.Time : TimeEnd;

                CmdList.SetVizir(nextTime, _vizirTime, ViewTimeBegin, ViewTimeEnd, _axisXIntervalType);

                string num = (nextGr != null) ? nextGr.Num.ToString() : "A";
                if (nextGr == null) nextGr = Graphics[0];

                labVizirR.BackColor = nextGr.Series.Color;
                labVizirR.Text = num;
                labVizirL.BackColor = Color.White;
            }
            //}
            //catch (Exception ex)
            //{
            //    //timerHold.Stop();
            //    //MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
            //    //Error = new ErrorCommand("", ex);
            //}
        }

        //ButtonTimeWalkBack
        private void VizirPrevCur(object sender, EventArgs e)
        {
            //try
            //{
            var gr = GetGraphicByNum(CurGraphicNum);

            if ((gr.DotCount > 0) && (BackGround.Area.CursorX.Position > 0))
            {
                var prevDot = GetVizirPrevDot(gr);
                var prevTime = (prevDot != null) ? prevDot.Time : TimeBegin;

                CmdList.SetVizir(prevTime, _vizirTime, ViewTimeBegin, ViewTimeEnd, _axisXIntervalType);

                labVizirL.BackColor = gr.Series.Color;
                labVizirL.Text = gr.Num.ToString();
                labVizirR.BackColor = Color.White;
            }
            //}
            //catch (Exception ex)
            //{
            //    //timerHold.Stop();
            //    //MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
            //    //Error = new ErrorCommand("", ex);
            //}
        }

        //ButtonTimeStepBack
        private void VizirPrevAll(object sender, EventArgs e)
        {
            //try
            //{
            if (BackGround.Area.CursorX.Position > 0)
            {
                MomentValue prevDot = null;
                Graphic prevGr = null;

                foreach (var gr in Graphics)
                {
                    var dot = GetVizirPrevDot(gr);
                    if (dot != null)
                    {
                        if ((prevDot == null) || (dot.Time > prevDot.Time))
                        {
                            prevDot = dot;
                            prevGr = gr;
                        }
                    }
                }

                var prevTime = (prevDot != null) ? prevDot.Time : TimeBegin;

                CmdList.SetVizir(prevTime, _vizirTime, ViewTimeBegin, ViewTimeEnd, _axisXIntervalType);

                string num = (prevGr != null) ? prevGr.Num.ToString() : "A";
                if (prevGr == null) prevGr = Graphics[0];

                labVizirL.BackColor = prevGr.Series.Color;
                labVizirL.Text = num;
                labVizirR.BackColor = Color.White;
            }
            //}
            //catch (Exception ex)
            //{
            //    //timerHold.Stop();
            //    //MessageBox.Show(ex.Message + "\n===\n" + ex.StackTrace);
            //    //Error = new ErrorCommand("", ex);
            //}
        }
    #endregion Vizir Next/Prev

    #region Debug +
        //вспомогательная функция для отладки
        private void labCur_DoubleClick(object sender, EventArgs e)
        {
            //DebugFunction();
        }
        
        private void DebugFunction()
        {
            MessageBox.Show("Form Width: " + Size.Width + "\n" +
                "ContainerH Width: " + splitContainerH.Width + "\n" +
                "ContainerH Panel1 X, Width: " + splitContainerH.Panel1.Location.X + " " + splitContainerH.Panel1.Size.Width + "\n" +
                "ContainerH Panel2 X, Width: " + splitContainerH.Panel2.Location.X + " " + splitContainerH.Panel2.Size.Width + "\n" +
                "SplitterDistance: " + splitContainerH.SplitterDistance + "\n");
        }
    #endregion Debug

    #region Undo/Redo +
        internal void SetUndoRedoStatus(bool canUndo, bool canRedo)
        {
            butUndo.Enabled = canUndo;
            butRedo.Enabled = canRedo;
        }
    #endregion Undo/Redo

    #region CmdListOperation
        //internal void CmdAddGraphic(Graphic graphic) { }
        //internal void CmdDeleteGraphic(Graphic graphic) { }
        
        internal void CmdGraphicShow(Graphic graphic)
        {
            try
            {
                ShowGraphic(graphic);
                SetError("GraphicShow", null);
            }
            catch (Exception e)
            {
                SetError("GraphicShow", e);
            }
        }

        internal void CmdGraphicShow(List<Graphic> graphics)
        {
            try
            {
                var errs = new List<ErrorCommand>();

                foreach (var gr in graphics)
                {
                    CmdGraphicShow(gr);
                    if (_lastError != null) errs.Add(_lastError);
                }

                SetError("GraphicShow", null);
            }
            catch (Exception e)
            {
                SetError("GraphicShow", e);
            }
        }

        internal void CmdGraphicHide(Graphic graphic)
        {
            try
            {
                HideGraphic(graphic);
                SetError("GraphicShow", null);
            }
            catch (Exception e)
            {
                SetError("GraphicShow", e);
            }
        }

        internal void CmdGraphicHide(List<Graphic> graphics)
        {
            try
            {
                var errs = new List<ErrorCommand>();

                foreach (var gr in graphics)
                {
                    CmdGraphicHide(gr);
                    if (_lastError != null) errs.Add(_lastError);
                }

                SetError("GraphicHide", null);
            }
            catch (Exception e)
            {
                SetError("GraphicHide", e);
            }
        }
         
        //internal void CmdSelectGraphic(Graphic graphic) { }

        internal void CmdGraphicChangeAxY(AnalogGraphic graphic, GroupY axY = null)
        {
            try
            {
                if(axY != null)
                {
                    if (graphic.GroupY.IsOverlayed)
                    {
                        //if (!GroupsY.Contains(axY)) AxRestore(axY);
                        ChangeGraphicAxY(graphic, axY);
                    }
                    else
                        AxJunction(axY, graphic.GroupY);
                }
                else
                {
                    if(graphic.GroupY.IsOverlayed) AxSeparation(graphic);
                }

                SetError("ChangeAnalogGraphicAxis", null);
            }
            catch (Exception e)
            {
                SetError("ChangeAnalogGraphicAxis", e);
            }
        }

        /*internal void Test()
        {
            string s = "Оси аналоговых графики\n";
            foreach (var aGr in AnalogGraphics) s += aGr.Num + " - " + aGr.GroupY.AxCap.Text + "\n";
            MessageBox.Show(s);

            s = "Оси\n";
            int i = 0;
            foreach (var axY in GroupsY)
            {
                s += ++i + ": ";
                foreach (var aGr in axY.Graphics) s += aGr.Num + "; ";
                s += "\n";
            }

            MessageBox.Show(s);
        }*/

        /*internal void Test1(GroupY axY)
        {
            string s = axY.AxCap.Text + "\n";
            foreach (var aGr in axY.Graphics) s += aGr.Num + "; ";
            s += "\n";

            MessageBox.Show(s);
        }*/

        internal void CmdAxYJunction(GroupY axCatcher, GroupY axY)
        {
            try
            {
                if ((axY != null) && (axCatcher!=null)) AxJunction(axCatcher, axY);
                SetError("AxYJunction", null);
            }
            catch (Exception e)
            {
                SetError("AxisJunction", e);
            }
        }

        //internal void CmdAxYJunctionRepeal(List<AnalogGraphic> graphics, double viewMin, double viewMax, bool isInPercent, double axSize, double axPos,
        //                                   AnalogGraphic upperGraphic, AnalogGraphic catcherUpperGraphic)
        //{
        //    try
        //    {
        //        AxSeparation(graphics[0]);
        //        var axY = graphics[0].GroupY;
        //        for (int i = 1; i < graphics.Count; i++) ChangeGraphicAxY(graphics[i], axY);
        //        SetAxYScaleView(axY, viewMin, viewMax, isInPercent);
        //        axY.CurAxSize = axSize;
        //        axY.CurAxPos = axPos;
        //        AnalogRankingY(axY);

        //        axY.UpperGraphic = upperGraphic;
        //        catcherUpperGraphic.GroupY.UpperGraphic = catcherUpperGraphic;

        //        SetError("AxYJunctionRepeal", null);
        //    }
        //    catch (Exception e)
        //    {
        //        SetError("AxYJunctionRepeal", e);
        //    }
        //}

        internal void CmdAxYJunctionRepeal(GroupY axY, List<AnalogGraphic> graphics, AnalogGraphic catcherUpperGraphic)
        {
            AxRestore(axY, graphics);
            catcherUpperGraphic.GroupY.UpperGraphic = catcherUpperGraphic;
        }
        
        internal void CmdAxYShow(GroupY axY)
        {
            try
            {
                ShowAxY(axY);
                SetError("GraphicShow", null);
            }
            catch (Exception e)
            {
                SetError("GraphicShow", e);
            }
        }

        internal void CmdAxYHide(GroupY axY)
        {
            try
            {
                HideAxY(axY);
                SetError("GraphicShow", null);
            }
            catch (Exception e)
            {
                SetError("GraphicShow", e);
            }
        }

        internal void CmdAxYChangeSizePos(GroupY axY, double newSize, double newPos)
        {
            try
            {
                axY.CurAxSize = newSize;
                axY.CurAxPos = newPos;
                AnalogRankingY(axY);
                SetError("AxYChangeSizePos", null);
            }
            catch (Exception e)
            {
                SetError("AxYChangeSizePos", e);
            }
        }

        //internal void CmdChangeAxisSizePos(List<GroupY> axYs, List<double> newSizes, List<double> newPoses) { }

        //internal void CmdAxisJunction(GroupY pincher, GroupY catcher) { }

        internal void CmdAxYSetViewY(GroupY axY, double viewYMin, double viewYMax, bool inPercent)
        {
            try
            {
                SetAxYScaleView(axY, viewYMin, viewYMax, inPercent);
                SetError("AxYSetViewY", null);
            }
            catch (Exception e)
            {
                SetError("AxYSetViewY", e);
            }
        }

        internal void CmdAxYSetViewY(List<GroupY> axYs, List<double> viewYMins, List<double> viewYMaxs, List<bool> inPercents)
        {
            try
            {
                var errs = new List<ErrorCommand>();

                for (int i = 0; i < axYs.Count; i++ )
                {
                    SetAxYScaleView(axYs[i], viewYMins[i], viewYMaxs[i], inPercents[i]);
                    if (_lastError != null) errs.Add(_lastError);
                }
                SetError("AxYSetViewY", null);
            }
            catch (Exception e)
            {
                SetError("AxYSetViewY", e);
            }
        }
        
        internal void CmdSetTimeView(DateTime viewTimeBegin, DateTime viewTimeEnd, DateTimeIntervalType scaleType) 
        {
            try
            {
                if (scaleType!= DateTimeIntervalType.NotSet)
                    ChangeScaleView(viewTimeBegin, viewTimeEnd, scaleType);
                else
                    ChangeScaleViewAuto(viewTimeBegin, viewTimeEnd);
                SetError("SetTimeView", null);
            }
            catch (Exception e)
            {
                SetError("SetTimeView", e);
            }
        }

        //internal void CmdSetTimeBegiEnd(DateTime timeBegin, DateTime timeEnd) { }
        
        internal void CmdSetVizir(DateTime vizirTime, bool changeCursor = true)
        {
            try
            {
                double vTime = vizirTime.ToOADate();

                if (changeCursor)
                {
                    if (_analogBackGround != null) _analogBackGround.Area.CursorX.Position = vTime;
                    if (_discretBackGround != null) _discretBackGround.Area.CursorX.Position = vTime;

                    //сдвигаем график, если визир вышел за пределы экрана
                    Refresh();
                    var bgr = BackGround;
                    var newTime = bgr.Area.CursorX.Position;

                    if (newTime > bgr.Area.AxisX.ScaleView.ViewMaximum)
                    {
                        double newTimeB = newTime -
                                          .75*(bgr.Area.AxisX.ScaleView.ViewMaximum - bgr.Area.AxisX.ScaleView.Position);
                        DateTime timeB = DateTime.FromOADate(newTimeB);
                        TimeSpan ts = ViewTimeEnd.Subtract(ViewTimeBegin);
                        DateTime timeE = timeB.Add(ts);

                        if (timeE > TimeEnd)
                        {
                            TimeSpan dt = timeE.Subtract(TimeEnd);
                            timeB = timeB.Subtract(dt);
                            timeE = TimeEnd;
                        }

                        ChangeScaleView(timeB, timeE, _axisXScaleViewSizeType);
                    }
                    else if (newTime < bgr.Area.AxisX.ScaleView.ViewMinimum)
                    {
                        double newTimeB = newTime -
                                          .75*(bgr.Area.AxisX.ScaleView.ViewMaximum - bgr.Area.AxisX.ScaleView.Position);
                        DateTime timeB = DateTime.FromOADate(newTimeB);
                        TimeSpan ts = ViewTimeEnd.Subtract(ViewTimeBegin);
                        DateTime timeE = timeB.Add(ts);

                        if (timeB < TimeBegin)
                        {
                            TimeSpan dt = TimeBegin.Subtract(timeB);
                            timeB = TimeBegin;
                            timeE = timeE.Add(dt);
                        }

                        ChangeScaleView(timeB, timeE, _axisXScaleViewSizeType);
                    }
                }

                VizirChanged(vizirTime);

                SetError("SetVizir", null);
            }
            catch (Exception e)
            {
                SetError("SetVizir", e);
            }
        }

        internal void CmdUnsetVizir()
        {
            try
            {
                if (_analogBackGround != null) _analogBackGround.Area.CursorX.Position = double.NaN;
                if (_discretBackGround != null) _discretBackGround.Area.CursorX.Position = double.NaN;

                //Инфо
                UnsetVizirTextBoxes();
                //DataGrid
                UnsetVizirDataGridView();
                _vizirTime = DateTime.MinValue;

                SetError("UnsetVizir", null);
            }
            catch(Exception e)
            {
                SetError("UnsetVizir", e);
            }
        }

        internal void CmdSetLineWidth(Graphic graphic, int lineWidth)
        {
            try
            {
                graphic.LineWidth = lineWidth;
                SetError("SetLineWidth(Graphic, int)", null);
            }
            catch (Exception e)
            {
                SetError("SetLineWidth(Graphic, int)", e);
            }
        }

        internal void CmdSetLineWidth(List<Graphic> graphics, int lineWidth)
        {
            try
            {
                var errs = new List<ErrorCommand>();

                foreach (var gr in graphics)
                {
                    CmdSetLineWidth(gr, lineWidth);
                    if (_lastError != null) errs.Add(_lastError);
                }

                SetError("SetLineWidth", null);
            }
            catch (Exception e)
            {
                SetError("SetLineWidth", e);
            }
        }

        private void SetError(string functionName, Exception e)
        {
            _lastError = e != null ? new ErrorCommand("", e) : null;
        }
    #endregion CmdListOperation
    }

    //comparer сортировки датагрида по цвету
    public class DatagridColorSort : IComparer
    {
        public int Compare(object obj1, object obj2)
        {
            var row1 = (DataGridViewRow) obj1;
            var row2 = (DataGridViewRow) obj2;
            if ((int)row1.Cells["Group"].Value > (int)row2.Cells["Group"].Value) return 1;
            if ((int)row1.Cells["Group"].Value < (int)row2.Cells["Group"].Value) return -1;
            if (int.Parse(row1.Cells[1].Value.ToString()) > int.Parse(row2.Cells[1].Value.ToString())) return 1;
            if (int.Parse(row1.Cells[1].Value.ToString()) < int.Parse(row2.Cells[1].Value.ToString())) return -1;

            return 0;
        }
    }

    //comparer сортировки датагрида по цвету
    internal class GroupsYSort : IComparer<GroupY>
    {
        public int Compare(GroupY axY1, GroupY axY2)
        {
            //int num1 = axY1.UpperGraphic.Num;
            //foreach(var gr in axY1.Graphics)
            //    if (gr.Num < num1) num1 = gr.Num;

            //int num2 = axY2.UpperGraphic.Num;
            //foreach(var gr in axY2.Graphics)
            //    if (gr.Num < num2) num2 = gr.Num;

            //if (num1 > num2) return 1;
            //if (num2 > num1) return -1;
            //return 0;

            if (axY1.MinGraphicNum > axY2.MinGraphicNum) return 1;
            if (axY2.MinGraphicNum > axY1.MinGraphicNum) return -1;
            return 0;
        }
    }
}

/* Замена имён
//button1	butVizirNextCur
//button2	butDynShiftOnOff
//button3	butPrint
//button4	butCurMinMaxApply
//button5	butVizirPrevCur
//button6	butDynClear
//button7	butAllScaleAuto
//button8	butViewPeriodApply
//button9	butCurScaleByMinMax
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
*/