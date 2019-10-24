using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using GraphicLibrary.Params;

namespace GraphicLibrary
{
    //Группа аналоговых параметров с общей шкалой по Y
    internal class GroupY
    {
    #region Prop
        internal const int AxCapHeight = 17;

        internal Panel Ax = new Panel();
        internal Label[] Labels = new Label[4];
        internal Label AxCap;
        internal PictureBox ResizeAreaButtonBottom = new PictureBox();
        internal PictureBox ResizeAreaButtonTop = new PictureBox();
        internal PictureBox ResizeAxButtonBottom = new PictureBox();
        internal PictureBox ResizeAxButtonTop = new PictureBox();
        internal Panel RulePanel = new Panel();
        internal Button[] RulePBoxes = new Button[2];
        internal List<PictureBox> OverlayPicList = new List<PictureBox>(4);
        internal TextBox TxtBoxMax = new TextBox();
        internal TextBox TxtBoxMin = new TextBox();
        
        //Список графиков
        private readonly List<AnalogGraphic> _graphics = new List<AnalogGraphic>();
        internal ReadOnlyCollection<AnalogGraphic> Graphics; //= new ReadOnlyCollection<AnalogGraphic>(_graphics);
        
        //Верхний график
        private AnalogGraphic _upperGraphic;
        internal AnalogGraphic UpperGraphic
        {
            get { return _upperGraphic; }
            set
            {
                //!сделать проверку на существование (м.б. добавлять его в список графиков оси, если нет)
                try
                {
                    _upperGraphic = value;

                    AxCap.Text = (IsInPercent ? "%" : "") + _upperGraphic.Num + (IsOverlayed ? "..." : "");
                    AxCap.ForeColor = _upperGraphic.Series.Color;
                    //Ax.Name = _upperGraphic.Num.ToString(); //не используется name
                    
                    if (_upperGraphic.Series.Color != Color.Transparent)
                    {
                        TxtBoxMax.BackColor = _upperGraphic.Series.Color;
                        TxtBoxMin.BackColor = _upperGraphic.Series.Color;
                        foreach (var label in Labels)
                            label.ForeColor = _upperGraphic.Series.Color;
                    }
                    //IsPercent = _upperGraphic.IsPercent;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace + "\n" + e.Message);
                }
            }
        }

        //Минимальный номер графика
        public int MinGraphicNum { get; private set; }
        
        private double _viewMin;
        internal double ViewMin
        {
            get { return _viewMin; }
            set
            {
                _viewMin = value;
                
                TxtBoxMin.Text = _viewMin.ToString(DecPlacesMask);
                TxtBoxMin.Select(0, 0);
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

                TxtBoxMax.Text = _viewMax.ToString(DecPlacesMask);
                TxtBoxMax.Select(0, 0);
                FillLabels(_viewMin, _viewMax);
            }
        }
        
        //Прикреплено ли к оси больше одного графика
        private bool _isOverlayed;
        internal bool IsOverlayed
        {
            get { return _isOverlayed; }
            private set
            {
                _isOverlayed = value;
                
                if (_isOverlayed)
                {
                    foreach (var bord in OverlayPicList)
                    {
                        bord.BackColor = Color.LightSlateGray;
                    }
                    AxCap.Text += "...";
                }
                else
                {
                    foreach (var bord in OverlayPicList)
                    {
                        bord.BackColor = Ax.BackColor;
                    }
                    if (AxCap.Text.Substring(AxCap.Text.Length - 1) == ".")
                    {
                        AxCap.Text = AxCap.Text.Substring(0, AxCap.Text.Length - 3);
                    }
                }
            }
        }

        //Ось в процентах
        private bool _isInPercent;
        internal bool IsInPercent
        {
            get { return _isInPercent; }
            set
            {
                _isInPercent = value;
                if (AxCap != null)
                {
                    if (_isInPercent)
                    {
                        if (AxCap.Text.Substring(0, 1) != "%") AxCap.Text = "%" + AxCap.Text;
                    }
                    else
                    {
                        if (AxCap.Text.Substring(0, 1) == "%") AxCap.Text = AxCap.Text.Remove(0, 1);
                    }
                }
            }
        }

        //Видна ли ось
        private bool _isVisible;
        internal bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                Ax.Visible = value;
                _isVisible = value;

                if(value) IsHidden = false;
            }
        }

        //Скрыты ли графики оси
        //Не является отрицанием IsVisible (М.б. IsVisible = false и isHidden = false)
        private bool _isHidden;
        internal bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                if (value) IsVisible = false;
            }
        } 
        
        //Маска для заполнения необходимого кол-ва символов после запятой
        internal int DecPlaces { get; private set; }
        internal string DecPlacesMask { get; private set; }
        internal void DecPlacesMaskFill(int decPlaces)
        {
            DecPlaces = decPlaces;

            if (decPlaces == -1) DecPlacesMask = "";
            else
            {
                DecPlacesMask = "0.";
                for (int i = 0; i < decPlaces; i++) DecPlacesMask += "#"; 
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
        internal GroupY(AnalogGraphic graphic)
        {
            Graphics = new ReadOnlyCollection<AnalogGraphic>(_graphics);
            _graphics.Add(graphic);
            graphic.GroupY = this;

            Init();
            InitEvent();

            //mm ViewMin = _graphics[0].Param.Min;
            ViewMin = _graphics[0].MinViewValue;
            //mm ViewMax = _graphics[0].Param.Max;
            ViewMax = _graphics[0].MaxViewValue;
        }
    #endregion Constructor

    #region Init
        private void Init()
        {
            IsInPercent = false;
            IsVisible = true;
            IsHidden = false;

            Ax.BorderStyle = BorderStyle.Fixed3D;
            Ax.MinimumSize = new Size(0, 125);
            Ax.Name = _graphics[0].Num.ToString();
            Ax.Width = FormGraphic.AxWidth;

            MinGraphicNum = _graphics[0].Num;

            AxCap = new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204))),
                Size = new Size(FormGraphic.AxWidth, AxCapHeight),
                //Text = index.ToString(),
                //ForeColor = color
            };

            var npbR = new PictureBox();
            var npbB = new PictureBox();
            var npbL = new PictureBox();
            var npbT = new PictureBox();
            OverlayPicList.AddRange(new[] { npbL, npbB, npbR, npbT });

            ResizeAreaButtonTop.Size = new Size(FormGraphic.AxWidth - 10, 3);
            ResizeAreaButtonTop.Cursor = Cursors.PanSouth;
            ResizeAreaButtonTop.BackColor = SystemColors.GradientInactiveCaption;

            ResizeAxButtonTop.Size = new Size(20, 3);
            ResizeAxButtonTop.Cursor = Cursors.SizeNS;
            ResizeAxButtonTop.BackColor = SystemColors.GradientInactiveCaption;

            RulePanel.Size = new Size(20, 19);
            for (int j = 0; j < 2; j++)
            {
                RulePBoxes[j] = new Button { Size = new Size(19, 19) };
                RulePanel.Controls.Add(RulePBoxes[j]);
                RulePBoxes[j].Location = new Point(j % 2 * 19, j / 2 * 19);
                RulePBoxes[j].Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, ((204)));
            }

            for (int i = 0; i < 4; i++)
                Ax.Controls.Add(OverlayPicList[i]);
            Ax.Controls.Add(ResizeAreaButtonTop);
            Ax.Controls.Add(ResizeAreaButtonBottom);
            Ax.Controls.Add(TxtBoxMin);
            Ax.Controls.Add(TxtBoxMax);
            Ax.Controls.Add(RulePanel);
            Ax.Controls.Add(AxCap);
            Ax.Controls.Add(ResizeAxButtonTop);
            Ax.Controls.Add(ResizeAxButtonBottom);

            AxCap.Dock = DockStyle.Bottom;
            //AxCap.SendToBack();

            //RulePBoxes[1].Cursor = Cursors.SizeAll;
            //RulePBoxes[1].BackColor = Color.SteelBlue;
            RulePBoxes[1].Image = Properties.Resources.cross16;
            RulePBoxes[0].Image = Properties.Resources.earth16;

            TxtBoxMax.Dock = DockStyle.Top;
            TxtBoxMax.ForeColor = Color.White;

            ResizeAreaButtonBottom.Size = new Size(FormGraphic.AxWidth - 10, 3);
            ResizeAreaButtonBottom.Cursor = Cursors.PanNorth;
            ResizeAreaButtonBottom.BackColor = SystemColors.GradientInactiveCaption;
            //ResizeAreaButtonBottom.Dock = DockStyle.Bottom;

            ResizeAxButtonBottom.Size = new Size(20, 3);
            ResizeAxButtonBottom.Cursor = Cursors.SizeNS;
            ResizeAxButtonBottom.BackColor = SystemColors.GradientInactiveCaption;
            ResizeAxButtonBottom.Dock = DockStyle.Bottom;

            TxtBoxMin.Dock = DockStyle.Bottom;
            TxtBoxMin.ForeColor = Color.White;

            RulePanel.Dock = DockStyle.Top;
            //ResizeAreaButtonTop.Dock = DockStyle.Top;
            ResizeAxButtonTop.Dock = DockStyle.Top;

            for (int j = 0; j < 4; j++)
            {
                Labels[j] = new Label
                {
                    //ForeColor = color,
                    Size = new Size(FormGraphic.AxWidth, 20),
                    MinimumSize = new Size(FormGraphic.AxWidth, 20),
                    AutoSize = true
                };
                Ax.Controls.Add(Labels[j]);
                Labels[j].Left = 0;
            }
            ResizeAreaButtonTop.Left = 3;
            ResizeAreaButtonBottom.Left = 3;
            ResizeAreaButtonTop.BringToFront();
            ResizeAreaButtonBottom.BringToFront();

            //AxesOverlay.Add(upperP.Index);
            OverlayPicList[3].Height = 3;
            OverlayPicList[1].Height = 3;
            OverlayPicList[0].Width = 3;
            OverlayPicList[2].Width = 3;
            OverlayPicList[0].Dock = DockStyle.Left;
            OverlayPicList[1].Dock = DockStyle.Bottom;
            OverlayPicList[2].Dock = DockStyle.Right;
            OverlayPicList[3].Dock = DockStyle.Top;

            UpperGraphic = _graphics[0];

            //из PrepareParam
            //настраиваем кол-во цифр после запятой
            DecPlacesMaskFill(_graphics[0].Param.DecPlaces);
        }

        private void InitEvent()
        {
            //~Ax.DoubleClick += Ax_MouseDouble;
            Ax.MouseDoubleClick += Ax_MouseDouble;
            Ax.Resize += Ax_Resize;
            Ax.MouseDown += Ax_MouseDown;
            Ax.MouseUp += Ax_MouseUp;
            Ax.MouseMove += Ax_MouseMove;

            ResizeAxButtonTop.MouseDown += Ax_MouseDown;
            ResizeAxButtonTop.MouseUp += ResizeAxButton_MouseUp;
            ResizeAxButtonTop.MouseMove += ResizeAxButtonTop_MouseMove;

            ResizeAreaButtonTop.MouseDown += ResizeAreaButton_MouseDown;
            ResizeAreaButtonTop.MouseUp += ResizeAreaButton_MouseUp;
            ResizeAreaButtonTop.MouseMove += ResizeAreaButtonTop_MouseMove;

            ResizeAxButtonBottom.MouseDown += Ax_MouseDown;
            ResizeAxButtonBottom.MouseUp += ResizeAxButton_MouseUp;
            ResizeAxButtonBottom.MouseMove += ResizeAxButtonBottom_MouseMove;

            ResizeAreaButtonBottom.MouseDown += ResizeAreaButton_MouseDown;
            ResizeAreaButtonBottom.MouseUp += ResizeAreaButton_MouseUp;
            ResizeAreaButtonBottom.MouseMove += ResizeAreaButtonBottom_MouseMove;

            AxCap.MouseClick += AxCap_MouseClick;

            RulePBoxes[1].MouseDown += AxSelect;
            RulePBoxes[0].MouseDown += AxSelect;
            RulePBoxes[1].MouseClick += RulePBoxesHideGr_MouseClick;
            RulePBoxes[0].MouseClick += RulePBoxesHideAx_MouseClick;

            for (int j = 0; j < 4; j++)
            {
                Labels[j].MouseDown += AxSelect;
                //~Labels[j].DoubleClick += Ax_MouseDouble;
                Labels[j].MouseDoubleClick += Ax_MouseDouble;
                Labels[j].MouseDown += Ax_MouseDown;
                Labels[j].MouseUp += Ax_MouseUp;
                Labels[j].MouseMove += Ax_MouseMove;
            }

            TxtBoxMin.KeyPress += FormGraphic.CheckInputReal;
            TxtBoxMin.KeyPress += TxtBoxMin_KeyPress;
            TxtBoxMin.Enter += AxSelect;
            TxtBoxMin.LostFocus += TxtBoxMin_Leave;

            TxtBoxMax.KeyPress += FormGraphic.CheckInputReal;
            TxtBoxMax.KeyPress += TxtBoxMax_KeyPress;
            TxtBoxMax.Enter += AxSelect;
            TxtBoxMax.LostFocus += TxtBoxMax_Leave;
        }
    #endregion Init

    #region Private
        //Заполнение промежуточных подписей оси
        private void FillLabels(double min, double max)
        {
            string mask = (DecPlacesMask == null || DecPlacesMask.Length > 5 || DecPlacesMask == "") ? mask = "0.###" : DecPlacesMask;
            for (int i = 0; i <= 3; i++)
            {
                Labels[i].Text = (min + (4 - i) * (-min + max) / 5).ToString(mask);
            }
        }
    #endregion Private

    #region Public
        internal void AddGraphic(AnalogGraphic graphic)
        {
            _graphics.Add(graphic);
            graphic.GroupY = this;
            
            if (!IsOverlayed) IsOverlayed = true;
            if (graphic.Num < MinGraphicNum) MinGraphicNum = graphic.Num;
            if ((graphic.Param.DecPlaces > DecPlaces) && (DecPlaces != -1)) DecPlacesMaskFill(graphic.Param.DecPlaces);
        }
        
        internal void RemoveGraphic(AnalogGraphic graphic)
        {
            if (_graphics.Count > 1)
            {
                _graphics.Remove(graphic);
                graphic.GroupY = null;
                if (IsOverlayed && _graphics.Count == 1) IsOverlayed = false;
                if(UpperGraphic.Num == graphic.Num)
                {
                    AnalogGraphic newUpGr = null;
                    foreach(var gr in Graphics)
                    {
                        if (gr.IsVisible)
                        {
                            newUpGr = gr;
                            break;
                        }
                    }

                    UpperGraphic = newUpGr ?? _graphics[0];
                    IsVisible = (newUpGr != null);
                }
                
                if(MinGraphicNum == graphic.Num)
                {
                    MinGraphicNum = UpperGraphic.Num;
                    foreach(var gr in Graphics)
                    {
                        if (gr.Num < MinGraphicNum) MinGraphicNum = gr.Num;
                    }
                }

                if(DecPlaces == graphic.Param.DecPlaces)
                {
                    int dP = UpperGraphic.Param.DecPlaces;
                    foreach(var gr in Graphics)
                    {
                        if ((gr.Param.DecPlaces > dP) && (dP != -1)) dP = gr.Param.DecPlaces;
                    }
                    DecPlacesMaskFill(dP);
                }
            }
        }

        internal int GraphicAreaBottom
        {
            //get { return TxtBoxMin.Top + TxtBoxMin.Height; }
            get { return TxtBoxMin.Bottom; }
        }
    #endregion Public

    #region ControlFunction
        private double _curSize;
        private double _curPos;
        private bool _md;

        private void Ax_Resize(object sender, EventArgs e)
        {
            for (int j = 0; j < 4; j++)
            {
                Labels[j].Top = (j + 1)*(Ax.Height - AxCap.Height - RulePanel.Height - 2)/5 - 10 +
                                RulePanel.Height + 2;
            }

            ResizeAreaButtonTop.Top = TxtBoxMax.Bottom;
            ResizeAreaButtonBottom.Top = TxtBoxMin.Top - 3;

            if (!_isMouseDown4Expand) UpperGraphic.FormGraphic.AxYResize(this);
        }
        
        //заменяет AxControlDetect, AxMouseDown из Диминого варианта
        private void AxSelect(object sender, EventArgs e)
        {
            UpperGraphic.FormGraphic.CurGraphicNum = UpperGraphic.Num;
        }

        private void SetMin()
        {
            _txtBoxBlock = true;
            UpperGraphic.FormGraphic.SetViewYMinMax(UpperGraphic, TxtBoxMin.Text, ViewMax.ToString(), IsInPercent);
            TxtBoxMin.Text = UpperGraphic.GroupY.ViewMin.ToString();
            _txtBoxBlock = false;
        }

        private void SetMax()
        {
            _txtBoxBlock = true;
            UpperGraphic.FormGraphic.SetViewYMinMax(UpperGraphic, ViewMin.ToString(), TxtBoxMax.Text, IsInPercent);
            TxtBoxMax.Text = UpperGraphic.GroupY.ViewMax.ToString();
            _txtBoxBlock = false;
        }
        
        private void TxtBoxMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SetMin();
            }
        }

        private void TxtBoxMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SetMax();
            }
        }

        //чтобы _Leave не срабатывала несколько раз при ошибке
        private bool _txtBoxBlock = false;

        private void TxtBoxMin_Leave(object sender, EventArgs e)
        {
            if (!_txtBoxBlock) SetMin();
        }

        private void TxtBoxMax_Leave(object sender, EventArgs e)
        {
            if (!_txtBoxBlock) SetMax();
        }

        private void Ax_MouseDouble(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                UpperGraphic.FormGraphic.AxToAllHeight(this);
            else
            {
                if (!IsInPercent)
                    //mm UpperGraphic.FormGraphic.SetAxYScaleView(this, UpperGraphic.Param.Min, UpperGraphic.Param.Max);
                    UpperGraphic.FormGraphic.CmdList.AxYSetViewY(this, UpperGraphic.MinViewValue, UpperGraphic.MaxViewValue, false);
                else
                    UpperGraphic.FormGraphic.CmdList.AxYSetViewY(this, 0, 100, true);
            }
        }

        //ResBMouseDown
        private void Ax_MouseDown(object sender, MouseEventArgs e)
        {
            AxSelect(sender, null);

            if (e.Button == MouseButtons.Left) _isMouseDown4Move = true;
            _mouseY = e.Y;
            double eY = e.Y;

            _mouseH = Ax.Bottom;
            if (sender is Label) eY += ((Label)sender).Top;

            if (e.Button == MouseButtons.Right)
            {
                //mm _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.Param.PercentToValue(ViewMin);
                _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.PercentToValue(ViewMin);
                //mm _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.Param.PercentToValue(ViewMax);
                _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.PercentToValue(ViewMax);

                double val = -(-TxtBoxMax.Top + eY)
                        / (-TxtBoxMax.Top + (double)TxtBoxMin.Bottom)
                        * (_expandBufMax - _expandBufMin) + _expandBufMax;

                UpperGraphic.FormGraphic.SelectCursorYSelectionStart(UpperGraphic, val);
            }
            else if (e.Button == MouseButtons.Left)
            {
                _curPos = CurAxPos;
                _curSize = CurAxSize;
                _md = true;
            }
        }

        //AxMBMove
        private void Ax_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown4Move)
            {
                int ad = 0;
                if (sender is Label) ad = ((Label)sender).Top;

                UpperGraphic.FormGraphic.AxYMoveV(this, e.Y, _mouseY);
                UpperGraphic.FormGraphic.AxYTryMoveH(this, e.X, e.Y, ad);
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    double eY = e.Y;
                    if (sender is Label) eY += ((Label)sender).Top;

                    //mm _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.Param.PercentToValue(ViewMin);
                    _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.PercentToValue(ViewMin);
                    //mm _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.Param.PercentToValue(ViewMax);
                    _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.PercentToValue(ViewMax);

                    double val = _expandBufMax -
                                 (-TxtBoxMax.Top + eY) / (-TxtBoxMax.Top + (double)TxtBoxMin.Bottom) *
                                 (_expandBufMax - _expandBufMin);

                    UpperGraphic.FormGraphic.SelectCursorYSelectionEnd(UpperGraphic, val);
                }
            }
        }

        //MovBMouseUp
        private void Ax_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown4Move = false;

            int ad = 0;
            double eY = e.Y;
            if (sender is Label)
            {
                ad = ((Label)sender).Top;
                eY += ((Label)sender).Top;
            }

            UpperGraphic.FormGraphic.AxYMoveH(this, e.X, e.Y, ad, e.Button == MouseButtons.Left);
            
            if (e.Button == MouseButtons.Right)
            {
                //mm _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.Param.PercentToValue(ViewMin);
                _expandBufMin = !IsInPercent ? ViewMin : UpperGraphic.PercentToValue(ViewMin);
                //mm _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.Param.PercentToValue(ViewMax);
                _expandBufMax = !IsInPercent ? ViewMax : UpperGraphic.PercentToValue(ViewMax);

                double val = _expandBufMax -
                            (-TxtBoxMax.Top + eY) / (-TxtBoxMax.Top + (double)TxtBoxMin.Bottom) *
                            (_expandBufMax - _expandBufMin);

                UpperGraphic.FormGraphic.SelectCursorYSelectionEnter(UpperGraphic, val);
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (_md) UpperGraphic.FormGraphic.CmdList.AxYSizePosChanged(this, _curSize, _curPos);
                _md = false;
            }
        }
        
        //ResBMouseUp
        private void ResizeAxButton_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown4Move = false;
            UpperGraphic.FormGraphic.CmdList.AxYSizePosChanged(this, _curSize, _curPos);
        }

        //AxResizeAreaTop
        private void ResizeAxButtonTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown4Move)
                UpperGraphic.FormGraphic.AxYChangeTop(this, e.Y, _mouseH);
        }

        //AxResizeAreaBottom
        private void ResizeAxButtonBottom_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown4Move)
                UpperGraphic.FormGraphic.AxYChangeBottom(this, e.Y, _mouseY);
        }

        //ResScndBMouseDown
        private void ResizeAreaButton_MouseDown(object sender, MouseEventArgs e)
        {
            AxSelect(sender, null);

            if (e.Button == MouseButtons.Left) _isMouseDown4Expand = true;
            _mouseY = e.Y;
            _mouseH = Ax.Bottom;

            if (sender is PictureBox)
            {
                _expandBufMin = double.Parse(TxtBoxMin.Text);
                _expandBufMax = double.Parse(TxtBoxMax.Text);
            }
        }

        //AxResizeTop
        private void ResizeAreaButtonTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown4Expand)
            {
                var holder = (PictureBox) sender;
                if (holder.Top + e.Y >= TxtBoxMax.Bottom)
                {
                    if (holder.Top + e.Y < ResizeAreaButtonBottom.Top - 5)
                        holder.Top = holder.Top + e.Y;
                    else
                        holder.Top = ResizeAreaButtonBottom.Top - 5;
                }
                else
                    holder.Top = TxtBoxMax.Bottom;

                UpperGraphic.FormGraphic.AnalogResizeRankingY(this, holder.Top - TxtBoxMax.Bottom, 0);

                int h = GraphicAreaBottom - FormGraphic.AxSummand;
                int h1 = h - (holder.Top - TxtBoxMax.Bottom);
                double dh = _expandBufMin + (double) h/h1*(_expandBufMax - _expandBufMin);

                //mm ViewMax = FormGraphic.GetReduceValue(dh, UpperGraphic.Param.Min, UpperGraphic.Param.Max, UpperGraphic.Param.DecPlaces);
                ViewMax = FormGraphic.GetReduceValue(dh, UpperGraphic.MinViewValue, UpperGraphic.MaxViewValue, UpperGraphic.Param.DecPlaces);
            }
        }

        //AxResizeBottom
        private void ResizeAreaButtonBottom_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown4Expand)
            {
                var holder = (PictureBox) sender;
                if (holder.Top + e.Y > TxtBoxMax.Bottom + 5)
                {
                    if (holder.Top + e.Y <= TxtBoxMin.Top - 3)
                        holder.Top = holder.Top + e.Y;
                    else
                        holder.Top = TxtBoxMin.Top - 3;
                }
                else
                    holder.Top = TxtBoxMax.Bottom + 5;

                UpperGraphic.FormGraphic.AnalogResizeRankingY(this, 0, TxtBoxMin.Top - 3 - holder.Top);

                int h = GraphicAreaBottom - FormGraphic.AxSummand;
                int h1 = h - (TxtBoxMin.Top - 3 - holder.Top);
                double dh = _expandBufMax - (double)h / h1 * (_expandBufMax - _expandBufMin);

                //mm ViewMin = FormGraphic.GetReduceValue(dh, UpperGraphic.Param.Min, UpperGraphic.Param.Max, UpperGraphic.Param.DecPlaces);
                ViewMin = FormGraphic.GetReduceValue(dh, UpperGraphic.MinViewValue, UpperGraphic.MaxViewValue, UpperGraphic.Param.DecPlaces);
            }
        }

        //ResScndBMouseUp
        private void ResizeAreaButton_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown4Expand = false;

            ResizeAreaButtonTop.Top = TxtBoxMax.Bottom;
            ResizeAreaButtonBottom.Top = TxtBoxMin.Top - 3;

            var min = TxtBoxMin.Text;
            var max = TxtBoxMax.Text;

            Ax_Resize(null, null);
            UpperGraphic.FormGraphic.SetViewYMinMax(UpperGraphic, min, max, IsInPercent);
        }

        //OverlayTopChoose
        private void AxCap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AxSelect(null, null);
                UpperGraphic.FormGraphic.ShowMenuOverlayedGraphics(this);
            }
        }

        private void RulePBoxesHideAx_MouseClick(object sender, EventArgs e)
        {
            UpperGraphic.FormGraphic.CmdList.AxYHide(this);
        }

        private void RulePBoxesHideGr_MouseClick(object sender, EventArgs e)
        {
            //var grs = new List<Graphic>();
            //foreach (var gr in Graphics) grs.Add(gr);
            //UpperGraphic.FormGraphic.CmdList.GraphicHide(grs);

            bool fgGroup = false;
            foreach (var gr in Graphics)
            {
                if (fgGroup) UpperGraphic.FormGraphic.CmdList.ContinueGroup();
                fgGroup = UpperGraphic.FormGraphic.CmdList.GraphicHide(gr) || fgGroup;
            }
        }
    #endregion ControlFunction
    }
}