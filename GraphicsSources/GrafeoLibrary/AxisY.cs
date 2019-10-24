using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GrafeoLibrary
{
    //Ось аналоговых графиов
    internal class AxisY
    {
    #region Const
        /*internal*/
        internal const int AxCapHeight = 17;
        internal const int AxPanelWidth = 41;
    #endregion Const

    #region Prop
        private readonly Panel _axPanel = new Panel();
        private readonly Label[] _labels = new Label[4];
        private Label _axCap;
        private readonly PictureBox _resizeAreaButtonBottom = new PictureBox();
        private readonly PictureBox _resizeAreaButtonTop = new PictureBox();
        private readonly PictureBox _resizeAxButtonBottom = new PictureBox();
        private readonly PictureBox _resizeAxButtonTop = new PictureBox();
        private readonly Panel _rulePanel = new Panel();
        private readonly Button[] _rulePanelBoxes = new Button[2];
        private readonly List<PictureBox> _overlayPicturesList = new List<PictureBox>(4);
        private readonly TextBox _textBoxMax = new TextBox();
        private readonly TextBox _textBoxMin = new TextBox();

        //Список графиков
        private readonly List<AnalogGraphic> _graphics = new List<AnalogGraphic>();
        internal readonly ReadOnlyCollection<AnalogGraphic> Graphics; //= new ReadOnlyCollection<AnalogGraphic>(_graphics);

        //Верхний график
        private AnalogGraphic _upperGraphic;
        internal AnalogGraphic UpperGraphic
        {
            get { return _upperGraphic; }
            set
            {
                if (_graphics.Contains(value))
                {
                    _upperGraphic = value;

                    _axCap.Text = (IsInPercent ? "%" : "") + _upperGraphic.Num + (IsOverlayed ? "..." : "");
                    _axCap.ForeColor = _upperGraphic.Color;

                    //if (_upperGraphic.Color != Color.Transparent)
                    //{
                    //    _textBoxMax.BackColor = _upperGraphic.Color;
                    //    _textBoxMin.BackColor = _upperGraphic.Color;
                    //    foreach (var label in _labels)
                    //        label.ForeColor = _upperGraphic.Color;
                    //}
                    //IsPercent = _upperGraphic.IsPercent;
                }
            }
        }

        //Минимальный номер графика
        internal int MinGraphicNum { get; private set; }

        private double _viewMin;
        internal double ViewMin
        {
            get { return _viewMin; }
            set
            {
                _viewMin = value;
                _textBoxMin.Text = _viewMin.ToString(DecimalPlacesMask);
                _textBoxMin.Select(0, 0);
                FillLabels(_viewMin, _viewMax);
            }
        }

        private double _viewMax;
        internal double ViewMax
        {
            get { return _viewMax; }
            set
            {
                _viewMax = value;
                _textBoxMax.Text = _viewMax.ToString(DecimalPlacesMask);
                _textBoxMax.Select(0, 0);
                FillLabels(_viewMin, _viewMax);
            }
        }

        //Прикреплено ли к оси больше одного графика
        private bool _isOverlayed;
        internal bool IsOverlayed
        {
            get { return _isOverlayed; }
            set
            {
                _isOverlayed = value;

                if (_isOverlayed)
                {
                    foreach (var bord in _overlayPicturesList)
                        bord.BackColor = Color.LightSlateGray;

                    if (!Regex.IsMatch(_axCap.Text, @"\w*...$")) _axCap.Text += "...";
                }
                else
                {
                    foreach (var bord in _overlayPicturesList)
                        bord.BackColor = _axPanel.BackColor;

                    if (Regex.IsMatch(_axCap.Text, @"\w*...$"))
                        _axCap.Text = _axCap.Text.Substring(0, _axCap.Text.Length - 3);
                }
            }
        }

        //Ось в процентах
        private bool _isInPercent;
        internal bool IsInPercent
        {
            get { return _isInPercent; }
            private set
            {
                _isInPercent = value;
                if (_axCap != null)
                {
                    if (_isInPercent)
                    {
                        if (!Regex.IsMatch(_axCap.Text, @"^%\w*")) _axCap.Text = "%" + _axCap.Text;
                    }
                    else
                    {
                        if (Regex.IsMatch(_axCap.Text, @"^%\w*")) _axCap.Text = _axCap.Text.Remove(0, 1);
                    }
                }
            }
        }

        //Видна ли ось
        //private bool _isVisible;
        internal bool IsVisible
        {
            get { return /*_isVisible;*/ _axPanel.Visible; }
            set
            {
                _axPanel.Visible = value;
                //_isVisible = value;

                //a if(value) IsHidden = false;
            }
        }

        ////Скрыты ли графики оси
        ////Не является отрицанием IsVisible (М.б. IsVisible = false и isHidden = false)
        //private bool _isHidden;
        //internal bool IsHidden
        //{
        //    get { return _isHidden; }
        //    set
        //    {
        //        _isHidden = value;
        //        if (value) IsVisible = false;
        //    }
        //} 

        //Маска для заполнения необходимого кол-ва символов после запятой
        private int DecimalPlaces { get; set; }
        private string DecimalPlacesMask { get; set; }
        private void DecimalPlacesMaskFill(int decimalPlaces)
        {
            DecimalPlaces = decimalPlaces;

            if (decimalPlaces == -1) DecimalPlacesMask = "";
            else
            {
                DecimalPlacesMask = "0.";
                //for (int i = 0; i < decPlaces; i++) DecimalPlacesMask += "#"; 
                DecimalPlacesMask += new String('#', decimalPlaces);
            }
        }

        internal double CurAxSize = 1; //текущий размер оси в процентах от эталона
        internal double CurAxPos = 0;//текущее положение оси в процентах

        //Для обработки событий
        private bool _isMouseDown4Move;
        private bool _isMouseDown4Expand;
        private int _mouseH;
        private int _mouseY;
        private double _expandBufMax;
        private double _expandBufMin;
    #endregion Prop

    #region Constructor
        internal AxisY(AnalogGraphic graphic)
        {
            Graphics = new ReadOnlyCollection<AnalogGraphic>(_graphics);
            _graphics.Add(graphic);
            //a graphic.GroupY = this;

            Init();
            InitEvent();
            
            //a ViewMin = _graphics[0].MinViewValue;
            //a ViewMax = _graphics[0].MaxViewValue;
        }
    #endregion Constructor

    #region Init
        private void Init()
        {
            IsInPercent = false;
            IsVisible = true;
            //IsHidden = false;

            _axPanel.BorderStyle = BorderStyle.Fixed3D;
            _axPanel.MinimumSize = new Size(0, 125);
            _axPanel.Name = _graphics[0].Num.ToString();
            _axPanel.Width = AxPanelWidth;

            MinGraphicNum = _graphics[0].Num;

            _axCap = new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204))),
                Size = new Size(AxPanelWidth, AxCapHeight),
                //Text = index.ToString(),
                //ForeColor = color
            };

            var npbR = new PictureBox();
            var npbB = new PictureBox();
            var npbL = new PictureBox();
            var npbT = new PictureBox();
            _overlayPicturesList.AddRange(new[] { npbL, npbB, npbR, npbT });

            _resizeAreaButtonTop.Size = new Size(AxPanelWidth - 10, 3);
            _resizeAreaButtonTop.Cursor = Cursors.PanSouth;
            _resizeAreaButtonTop.BackColor = SystemColors.GradientInactiveCaption;

            _resizeAxButtonTop.Size = new Size(20, 3);
            _resizeAxButtonTop.Cursor = Cursors.SizeNS;
            _resizeAxButtonTop.BackColor = SystemColors.GradientInactiveCaption;

            _rulePanel.Size = new Size(20, 19);
            for (int j = 0; j < 2; j++)
            {
                _rulePanelBoxes[j] = new Button { Size = new Size(19, 19) };
                _rulePanel.Controls.Add(_rulePanelBoxes[j]);
                _rulePanelBoxes[j].Location = new Point(j % 2 * 19, j / 2 * 19);
                _rulePanelBoxes[j].Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, ((204)));
            }

            for (int i = 0; i < 4; i++)
                _axPanel.Controls.Add(_overlayPicturesList[i]);
            _axPanel.Controls.Add(_resizeAreaButtonTop);
            _axPanel.Controls.Add(_resizeAreaButtonBottom);
            _axPanel.Controls.Add(_textBoxMin);
            _axPanel.Controls.Add(_textBoxMax);
            _axPanel.Controls.Add(_rulePanel);
            _axPanel.Controls.Add(_axCap);
            _axPanel.Controls.Add(_resizeAxButtonTop);
            _axPanel.Controls.Add(_resizeAxButtonBottom);

            _axCap.Dock = DockStyle.Bottom;
            //AxCap.SendToBack();

            //RulePBoxes[1].Cursor = Cursors.SizeAll;
            //RulePBoxes[1].BackColor = Color.SteelBlue;
            
            //a _rulePanelBoxes[1].Image = Properties.Resources.cross16;
            //a _rulePanelBoxes[0].Image = Properties.Resources.earth16;
            _rulePanelBoxes[1].Image = Properties.Resources.cross12.ToBitmap();
            _rulePanelBoxes[0].Image = Properties.Resources.earth12.ToBitmap();

            _textBoxMax.Dock = DockStyle.Top;
            _textBoxMax.ForeColor = Color.White;

            _resizeAreaButtonBottom.Size = new Size(AxPanelWidth - 10, 3);
            _resizeAreaButtonBottom.Cursor = Cursors.PanNorth;
            _resizeAreaButtonBottom.BackColor = SystemColors.GradientInactiveCaption;
            //ResizeAreaButtonBottom.Dock = DockStyle.Bottom;

            _resizeAxButtonBottom.Size = new Size(20, 3);
            _resizeAxButtonBottom.Cursor = Cursors.SizeNS;
            _resizeAxButtonBottom.BackColor = SystemColors.GradientInactiveCaption;
            _resizeAxButtonBottom.Dock = DockStyle.Bottom;

            _textBoxMin.Dock = DockStyle.Bottom;
            _textBoxMin.ForeColor = Color.White;

            _rulePanel.Dock = DockStyle.Top;
            //ResizeAreaButtonTop.Dock = DockStyle.Top;
            _resizeAxButtonTop.Dock = DockStyle.Top;

            for (int j = 0; j < 4; j++)
            {
                _labels[j] = new Label
                {
                    //ForeColor = color,
                    Size = new Size(AxPanelWidth, 20),
                    MinimumSize = new Size(AxPanelWidth, 20),
                    AutoSize = true
                };
                _axPanel.Controls.Add(_labels[j]);
                _labels[j].Left = 0;
            }
            _resizeAreaButtonTop.Left = 3;
            _resizeAreaButtonBottom.Left = 3;
            _resizeAreaButtonTop.BringToFront();
            _resizeAreaButtonBottom.BringToFront();

            //AxesOverlay.Add(upperP.Index);
            _overlayPicturesList[3].Height = 3;
            _overlayPicturesList[1].Height = 3;
            _overlayPicturesList[0].Width = 3;
            _overlayPicturesList[2].Width = 3;
            _overlayPicturesList[0].Dock = DockStyle.Left;
            _overlayPicturesList[1].Dock = DockStyle.Bottom;
            _overlayPicturesList[2].Dock = DockStyle.Right;
            _overlayPicturesList[3].Dock = DockStyle.Top;

            UpperGraphic = _graphics[0];

            //из PrepareParam
            //настраиваем кол-во цифр после запятой
            DecimalPlacesMaskFill(_graphics[0].Param.DecimalPlaces);
        }

        private void InitEvent()
        {
            ////~Ax.DoubleClick += Ax_MouseDouble;
            //_axPanel.MouseDoubleClick += Ax_MouseDouble;
            //_axPanel.Resize += Ax_Resize;
            //_axPanel.MouseDown += Ax_MouseDown;
            //_axPanel.MouseUp += Ax_MouseUp;
            //_axPanel.MouseMove += Ax_MouseMove;

            //_resizeAxButtonTop.MouseDown += Ax_MouseDown;
            //_resizeAxButtonTop.MouseUp += ResizeAxButton_MouseUp;
            //_resizeAxButtonTop.MouseMove += ResizeAxButtonTop_MouseMove;

            //_resizeAreaButtonTop.MouseDown += ResizeAreaButton_MouseDown;
            //_resizeAreaButtonTop.MouseUp += ResizeAreaButton_MouseUp;
            //_resizeAreaButtonTop.MouseMove += ResizeAreaButtonTop_MouseMove;

            //_resizeAxButtonBottom.MouseDown += Ax_MouseDown;
            //_resizeAxButtonBottom.MouseUp += ResizeAxButton_MouseUp;
            //_resizeAxButtonBottom.MouseMove += ResizeAxButtonBottom_MouseMove;

            //_resizeAreaButtonBottom.MouseDown += ResizeAreaButton_MouseDown;
            //_resizeAreaButtonBottom.MouseUp += ResizeAreaButton_MouseUp;
            //_resizeAreaButtonBottom.MouseMove += ResizeAreaButtonBottom_MouseMove;

            //_axCap.MouseClick += AxCap_MouseClick;

            //_rulePanelBoxes[1].MouseDown += AxSelect;
            //_rulePanelBoxes[0].MouseDown += AxSelect;
            //_rulePanelBoxes[1].MouseClick += RulePBoxesHideGr_MouseClick;
            //_rulePanelBoxes[0].MouseClick += RulePBoxesHideAx_MouseClick;

            //for (int j = 0; j < 4; j++)
            //{
            //    _labels[j].MouseDown += AxSelect;
            //    //~Labels[j].DoubleClick += Ax_MouseDouble;
            //    _labels[j].MouseDoubleClick += Ax_MouseDouble;
            //    _labels[j].MouseDown += Ax_MouseDown;
            //    _labels[j].MouseUp += Ax_MouseUp;
            //    _labels[j].MouseMove += Ax_MouseMove;
            //}

            //_textBoxMin.KeyPress += FormGraphic.CheckInputReal;
            //_textBoxMin.KeyPress += TxtBoxMin_KeyPress;
            //_textBoxMin.Enter += AxSelect;
            //_textBoxMin.LostFocus += TxtBoxMin_Leave;

            //_textBoxMax.KeyPress += FormGraphic.CheckInputReal;
            //_textBoxMax.KeyPress += TxtBoxMax_KeyPress;
            //_textBoxMax.Enter += AxSelect;
            //_textBoxMax.LostFocus += TxtBoxMax_Leave;
        }
    #endregion Init

    #region Private
        //Заполнение промежуточных подписей оси
        private void FillLabels(double min, double max)
        {
            string mask = (String.IsNullOrEmpty(DecimalPlacesMask)) ? mask = "0.###" : DecimalPlacesMask;
            if (DecimalPlacesMask.Length > 5) mask = "0.#####";
            for (int i = 0; i <= 3; i++)
                _labels[i].Text = (min + (4 - i) * (-min + max) / 5).ToString(mask);
            
        }
    #endregion Private

    #region Public
        internal void AddGraphic(AnalogGraphic graphic)
        {
            _graphics.Add(graphic);
            //a graphic.GroupY = this;

            if (!IsOverlayed) IsOverlayed = true;
            if (graphic.Num < MinGraphicNum) MinGraphicNum = graphic.Num;
            if ((graphic.Param.DecimalPlaces > DecimalPlaces) && (DecimalPlaces != -1)) DecimalPlacesMaskFill(graphic.Param.DecimalPlaces);
        }

        internal void RemoveGraphic(AnalogGraphic graphic)
        {
            if (_graphics.Count > 1)
            {
                _graphics.Remove(graphic);
                //a graphic.GroupY = null;
                if (IsOverlayed && _graphics.Count == 1) IsOverlayed = false;
                if (UpperGraphic.Num == graphic.Num)
                {
                    var newUpperGraphic = Graphics.FirstOrDefault(gr => gr.Visible);
                    UpperGraphic = newUpperGraphic ?? _graphics[0];
                    IsVisible = (newUpperGraphic != null);
                }

                if (MinGraphicNum == graphic.Num)
                {
                    MinGraphicNum = UpperGraphic.Num;
                    foreach (var gr in Graphics)
                    {
                        if (gr.Num < MinGraphicNum) MinGraphicNum = gr.Num;
                    }
                }

                if (DecimalPlaces == graphic.Param.DecimalPlaces)
                {
                    var dP = UpperGraphic.Param.DecimalPlaces;
                    foreach (var gr in Graphics)
                    {
                        if ((gr.Param.DecimalPlaces > dP) && (dP != -1)) dP = gr.Param.DecimalPlaces;
                    }
                    DecimalPlacesMaskFill(dP);
                }
            }
        }

        //internal int GraphicAreaBottom
        //{
        //    //get { return TxtBoxMin.Top + TxtBoxMin.Height; }
        //    get { return _textBoxMin.Bottom; }
        //}
    #endregion Public

    #region ControlFunction
        //private double _curSize;
        //private double _curPos;
        //private bool _md;

        //private void Ax_Resize(object sender, EventArgs e)
        //{
        //    for (int j = 0; j < 4; j++)
        //    {
        //        _labels[j].Top = (j + 1)*(_axPanel.Height - _axCap.Height - _rulePanel.Height - 2)/5 - 10 +
        //                        _rulePanel.Height + 2;
        //    }

        //    _resizeAreaButtonTop.Top = _textBoxMax.Bottom;
        //    _resizeAreaButtonBottom.Top = _textBoxMin.Top - 3;

        //    if (!_isMouseDown4Expand) UpperGraphic.FormGraphic.AxYResize(this);
        //}

        ////заменяет AxControlDetect, AxMouseDown из Диминого варианта
        //private void AxSelect(object sender, EventArgs e)
        //{
        //    UpperGraphic.FormGraphic.CurGraphicNum = UpperGraphic.Num;
        //}

        //private void SetMin()
        //{
        //    _txtBoxBlock = true;
        //    UpperGraphic.FormGraphic.SetViewYMinMax(UpperGraphic, _textBoxMin.Text, ViewMax.ToString(), IsInPercent);
        //    _textBoxMin.Text = UpperGraphic.GroupY.ViewMin.ToString();
        //    _txtBoxBlock = false;
        //}

        //private void SetMax()
        //{
        //    _txtBoxBlock = true;
        //    UpperGraphic.FormGraphic.SetViewYMinMax(UpperGraphic, ViewMin.ToString(), _textBoxMax.Text, IsInPercent);
        //    _textBoxMax.Text = UpperGraphic.GroupY.ViewMax.ToString();
        //    _txtBoxBlock = false;
        //}

        //private void TxtBoxMin_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == (char)Keys.Enter)
        //    {
        //        SetMin();
        //    }
        //}

        //private void TxtBoxMax_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == (char)Keys.Enter)
        //    {
        //        SetMax();
        //    }
        //}

        ////чтобы _Leave не срабатывала несколько раз при ошибке
        //private bool _txtBoxBlock = false;

        //private void TxtBoxMin_Leave(object sender, EventArgs e)
        //{
        //    if (!_txtBoxBlock) SetMin();
        //}

        //private void TxtBoxMax_Leave(object sender, EventArgs e)
        //{
        //    if (!_txtBoxBlock) SetMax();
        //}

        //private void Ax_MouseDouble(object sender, MouseEventArgs e)
        //{
        //    if(e.Button == MouseButtons.Left)
        //        UpperGraphic.FormGraphic.AxToAllHeight(this);
        //    else
        //    {
        //        if (!IsInPercent)
        //            //mm UpperGraphic.FormGraphic.SetAxYScaleView(this, UpperGraphic.Param.Min, UpperGraphic.Param.Max);
        //            UpperGraphic.FormGraphic.CmdList.AxYSetViewY(this, UpperGraphic.MinViewValue, UpperGraphic.MaxViewValue, false);
        //        else
        //            UpperGraphic.FormGraphic.CmdList.AxYSetViewY(this, 0, 100, true);
        //    }
        //}

        ////ResBMouseDown
        //private void Ax_MouseDown(object sender, MouseEventArgs e)
        //{
        //    AxSelect(sender, null);

        //    if (e.Button == MouseButtons.Left) _isMouseDown4Move = true;
        //    _mouseY = e.Y;
        //    double eY = e.Y;

        //    _mouseH = _axPanel.Bottom;
        //    if (sender is Label) eY += ((Label)sender).Top;

        //    if (e.Button == MouseButtons.Right)
        //    {
        //        //mm _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.Param.PercentToValue(ViewMin);
        //        _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.PercentToValue(ViewMin);
        //        //mm _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.Param.PercentToValue(ViewMax);
        //        _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.PercentToValue(ViewMax);

        //        double val = -(-_textBoxMax.Top + eY)
        //                / (-_textBoxMax.Top + (double)_textBoxMin.Bottom)
        //                * (_expandBufMax - _expandBufMin) + _expandBufMax;

        //        UpperGraphic.FormGraphic.SelectCursorYSelectionStart(UpperGraphic, val);
        //    }
        //    else if (e.Button == MouseButtons.Left)
        //    {
        //        _curPos = CurAxPos;
        //        _curSize = CurAxSize;
        //        _md = true;
        //    }
        //}

        ////AxMBMove
        //private void Ax_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_isMouseDown4Move)
        //    {
        //        int ad = 0;
        //        if (sender is Label) ad = ((Label)sender).Top;

        //        UpperGraphic.FormGraphic.AxYMoveV(this, e.Y, _mouseY);
        //        UpperGraphic.FormGraphic.AxYTryMoveH(this, e.X, e.Y, ad);
        //    }
        //    else
        //    {
        //        if (e.Button == MouseButtons.Right)
        //        {
        //            double eY = e.Y;
        //            if (sender is Label) eY += ((Label)sender).Top;

        //            //mm _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.Param.PercentToValue(ViewMin);
        //            _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.PercentToValue(ViewMin);
        //            //mm _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.Param.PercentToValue(ViewMax);
        //            _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.PercentToValue(ViewMax);

        //            double val = _expandBufMax -
        //                         (-_textBoxMax.Top + eY) / (-_textBoxMax.Top + (double)_textBoxMin.Bottom) *
        //                         (_expandBufMax - _expandBufMin);

        //            UpperGraphic.FormGraphic.SelectCursorYSelectionEnd(UpperGraphic, val);
        //        }
        //    }
        //}

        ////MovBMouseUp
        //private void Ax_MouseUp(object sender, MouseEventArgs e)
        //{
        //    _isMouseDown4Move = false;

        //    int ad = 0;
        //    double eY = e.Y;
        //    if (sender is Label)
        //    {
        //        ad = ((Label)sender).Top;
        //        eY += ((Label)sender).Top;
        //    }

        //    UpperGraphic.FormGraphic.AxYMoveH(this, e.X, e.Y, ad, e.Button == MouseButtons.Left);

        //    if (e.Button == MouseButtons.Right)
        //    {
        //        //mm _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.Param.PercentToValue(ViewMin);
        //        _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.PercentToValue(ViewMin);
        //        //mm _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.Param.PercentToValue(ViewMax);
        //        _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.PercentToValue(ViewMax);

        //        double val = _expandBufMax -
        //                    (-_textBoxMax.Top + eY) / (-_textBoxMax.Top + (double)_textBoxMin.Bottom) *
        //                    (_expandBufMax - _expandBufMin);

        //        UpperGraphic.FormGraphic.SelectCursorYSelectionEnter(UpperGraphic, val);
        //    }
        //    else if (e.Button == MouseButtons.Left)
        //    {
        //        if (_md) UpperGraphic.FormGraphic.CmdList.AxYSizePosChanged(this, _curSize, _curPos);
        //        _md = false;
        //    }
        //}

        ////ResBMouseUp
        //private void ResizeAxButton_MouseUp(object sender, MouseEventArgs e)
        //{
        //    _isMouseDown4Move = false;
        //    UpperGraphic.FormGraphic.CmdList.AxYSizePosChanged(this, _curSize, _curPos);
        //}

        ////AxResizeAreaTop
        //private void ResizeAxButtonTop_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_isMouseDown4Move)
        //        UpperGraphic.FormGraphic.AxYChangeTop(this, e.Y, _mouseH);
        //}

        ////AxResizeAreaBottom
        //private void ResizeAxButtonBottom_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_isMouseDown4Move)
        //        UpperGraphic.FormGraphic.AxYChangeBottom(this, e.Y, _mouseY);
        //}

        ////ResScndBMouseDown
        //private void ResizeAreaButton_MouseDown(object sender, MouseEventArgs e)
        //{
        //    AxSelect(sender, null);

        //    if (e.Button == MouseButtons.Left) _isMouseDown4Expand = true;
        //    _mouseY = e.Y;
        //    _mouseH = _axPanel.Bottom;

        //    if (sender is PictureBox)
        //    {
        //        _expandBufMin = double.Parse(_textBoxMin.Text);
        //        _expandBufMax = double.Parse(_textBoxMax.Text);
        //    }
        //}

        ////AxResizeTop
        //private void ResizeAreaButtonTop_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_isMouseDown4Expand)
        //    {
        //        var holder = (PictureBox) sender;
        //        if (holder.Top + e.Y >= _textBoxMax.Bottom)
        //        {
        //            if (holder.Top + e.Y < _resizeAreaButtonBottom.Top - 5)
        //                holder.Top = holder.Top + e.Y;
        //            else
        //                holder.Top = _resizeAreaButtonBottom.Top - 5;
        //        }
        //        else
        //            holder.Top = _textBoxMax.Bottom;

        //        UpperGraphic.FormGraphic.AnalogResizeRankingY(this, holder.Top - _textBoxMax.Bottom, 0);

        //        int h = GraphicAreaBottom - FormGraphic.AxSummand;
        //        int h1 = h - (holder.Top - _textBoxMax.Bottom);
        //        double dh = _expandBufMin + (double) h/h1*(_expandBufMax - _expandBufMin);

        //        //mm ViewMax = FormGraphic.GetReduceValue(dh, UpperGraphic.Param.Min, UpperGraphic.Param.Max, UpperGraphic.Param.DecPlaces);
        //        ViewMax = FormGraphic.GetReduceValue(dh, UpperGraphic.MinViewValue, UpperGraphic.MaxViewValue, UpperGraphic.Param.DecPlaces);
        //    }
        //}

        ////AxResizeBottom
        //private void ResizeAreaButtonBottom_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_isMouseDown4Expand)
        //    {
        //        var holder = (PictureBox) sender;
        //        if (holder.Top + e.Y > _textBoxMax.Bottom + 5)
        //        {
        //            if (holder.Top + e.Y <= _textBoxMin.Top - 3)
        //                holder.Top = holder.Top + e.Y;
        //            else
        //                holder.Top = _textBoxMin.Top - 3;
        //        }
        //        else
        //            holder.Top = _textBoxMax.Bottom + 5;

        //        UpperGraphic.FormGraphic.AnalogResizeRankingY(this, 0, _textBoxMin.Top - 3 - holder.Top);

        //        int h = GraphicAreaBottom - FormGraphic.AxSummand;
        //        int h1 = h - (_textBoxMin.Top - 3 - holder.Top);
        //        double dh = _expandBufMax - (double)h / h1 * (_expandBufMax - _expandBufMin);

        //        //mm ViewMin = FormGraphic.GetReduceValue(dh, UpperGraphic.Param.Min, UpperGraphic.Param.Max, UpperGraphic.Param.DecPlaces);
        //        ViewMin = FormGraphic.GetReduceValue(dh, UpperGraphic.MinViewValue, UpperGraphic.MaxViewValue, UpperGraphic.Param.DecPlaces);
        //    }
        //}

        ////ResScndBMouseUp
        //private void ResizeAreaButton_MouseUp(object sender, MouseEventArgs e)
        //{
        //    _isMouseDown4Expand = false;

        //    _resizeAreaButtonTop.Top = _textBoxMax.Bottom;
        //    _resizeAreaButtonBottom.Top = _textBoxMin.Top - 3;

        //    var min = _textBoxMin.Text;
        //    var max = _textBoxMax.Text;

        //    Ax_Resize(null, null);
        //    UpperGraphic.FormGraphic.SetViewYMinMax(UpperGraphic, min, max, IsInPercent);
        //}

        ////OverlayTopChoose
        //private void AxCap_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        AxSelect(null, null);
        //        UpperGraphic.FormGraphic.ShowMenuOverlayedGraphics(this);
        //    }
        //}

        //private void RulePBoxesHideAx_MouseClick(object sender, EventArgs e)
        //{
        //    UpperGraphic.FormGraphic.CmdList.AxYHide(this);
        //}

        //private void RulePBoxesHideGr_MouseClick(object sender, EventArgs e)
        //{
        //    //var grs = new List<Graphic>();
        //    //foreach (var gr in Graphics) grs.Add(gr);
        //    //UpperGraphic.FormGraphic.CmdList.GraphicHide(grs);

        //    bool fgGroup = false;
        //    foreach (var gr in Graphics)
        //    {
        //        if (fgGroup) UpperGraphic.FormGraphic.CmdList.ContinueGroup();
        //        fgGroup = UpperGraphic.FormGraphic.CmdList.GraphicHide(gr) || fgGroup;
        //    }
        //}
    #endregion ControlFunction
    }
}