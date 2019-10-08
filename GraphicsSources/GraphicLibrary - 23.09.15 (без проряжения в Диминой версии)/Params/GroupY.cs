using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using GraphicLibrary.Params;

namespace GraphicLibrary
{
    //Группа аналоговых парамтров с общей шкалой по Y
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
        //internal List<int> AxesOverlay = new List<int>();//список привязанных к оси графиков
        internal List<PictureBox> OverlayPicList = new List<PictureBox>(4);
        internal TextBox TxtBoxMax = new TextBox();
        internal TextBox TxtBoxMin = new TextBox();
        
        //Список графиков
        private readonly List<AnalogGraphic> _graphics = new List<AnalogGraphic>();
        //internal List<AnalogGraphic> Graphics { get { return _graphics; } }
        internal ReadOnlyCollection<AnalogGraphic> Graphics; //= new ReadOnlyCollection<AnalogGraphic>(_graphics);
        
        //Верхний график
        private AnalogGraphic _upperGraphic;
        internal AnalogGraphic UpperGraphic
        {
            get { return _upperGraphic; }
            set
            {
                //сделать проверку на существование

                try
                {
                    _upperGraphic = value;

                    AxCap.Text = IsOverlayed
                                     ? _upperGraphic.Num + "..."
                                     : _upperGraphic.Num.ToString();

                    AxCap.ForeColor = _upperGraphic.Series.Color;
                    Ax.Name = _upperGraphic.Num.ToString();
                    if (_upperGraphic.Series.Color != Color.Transparent)
                    {
                        TxtBoxMax.BackColor = _upperGraphic.Series.Color;
                        TxtBoxMin.BackColor = _upperGraphic.Series.Color;
                        foreach (var label in Labels)
                        {
                            label.ForeColor = _upperGraphic.Series.Color;
                        }
                    }

                    //IsPercent = _upperGraphic.IsPercent;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace + "\n" + e.Message);
                }
            }
        }

        //Свойства оси Y
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
        private bool IsOverlayed
        {
            get { return _isOverlayed; }
            set
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

        //Спрятана ли ось
        internal bool IsHidden;

        //автошкала
        internal bool IsAutoScale;
        
        //Маска для заполнения необходимого кол-ва символов после запятой
        internal string DecPlacesMask { get; private set; }
        internal void DecPlacesMaskFill(int decPlaces)
        {
            if (decPlaces == -1) DecPlacesMask = "";
            else
            {
                DecPlacesMask = "0.";
                for (int i = 0; i < decPlaces; i++) DecPlacesMask += "#"; 
            }
        }

        internal double CurAxSize = 1; //текущий размер оси в процентах от эталона
        internal double CurAxPos = 0;//текущее положение оси в процентах
    #endregion Prop

    #region Constructor
        internal GroupY(AnalogGraphic graphic)
        {
            Graphics = new ReadOnlyCollection<AnalogGraphic>(_graphics);
            _graphics.Add(graphic);
            graphic.GroupY = this;

            Init();
            
            ViewMin = _graphics[0].Param.Min;
            ViewMax = _graphics[0].Param.Max;
            
            InitEvent();
        }
    #endregion Constructor

    #region Public
        internal void AddGraphic(AnalogGraphic graphic)
        {
            _graphics.Add(graphic);
            graphic.GroupY = this;
            if (!IsOverlayed) IsOverlayed = true;
        }

        internal void RemoteGraphic(AnalogGraphic graphic)
        {
            if (_graphics.Count > 1)
            {
                _graphics.Remove(graphic);
                graphic.GroupY = null;
                if (IsOverlayed && _graphics.Count == 1) IsOverlayed = false;
            }
        }
    #endregion Public

    #region Private
        private void Init()
        {
            IsInPercent = false;
            IsHidden = false;

            Ax.BorderStyle = BorderStyle.Fixed3D;
            Ax.MinimumSize = new Size(0, 125);
            Ax.Name = _graphics[0].Num.ToString();
            Ax.Width = FormGraphic.AxWidth;

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
            //~ DecPlacesMaskFill(_graphics[0].DecPlaces);
        }

        private void InitEvent()
        {
            //Ax.DoubleClick += AxMouseDouble;
            Ax.Resize += _graphics[0].FormGraphic.AxResize;
            //Ax.MouseUp += MovBMouseUp;
            //Ax.MouseDown += ResBMouseDown;
            //Ax.MouseMove += AxMBMove;
            ////Ax.MouseDown += MovBMouseDown;

            //ResizeAxButtonTop.MouseDown += ResBMouseDown;
            //ResizeAxButtonTop.MouseUp += ResBMouseUp;
            //ResizeAxButtonTop.MouseMove += AxResizeAreaTop;
            //AxCap.MouseClick += OverlayTopChoose;

            //ResizeAreaButtonTop.MouseDown += ResScndBMouseDown;
            //ResizeAreaButtonTop.MouseUp += ResScndBMouseUp;
            //ResizeAreaButtonTop.MouseMove += AxResizeTop;

            //RulePBoxes[1].MouseDown += AxControlDetect;
            ////RulePBoxes[1].MouseUp += MovBMouseUp;
            ////RulePBoxes[1].MouseDown += ResBMouseDown;
            ////RulePBoxes[1].MouseMove += AxMBMove;

            //RulePBoxes[0].MouseDown += AxControlDetect;
            //RulePBoxes[1].MouseClick += HideGrClick;
            //RulePBoxes[0].MouseClick += HideAx;
            //TxtBoxMax.KeyPress += TextBoxInputAnyReal;
            //TxtBoxMax.Enter += AxControlDetect;

            //ResizeAxButtonBottom.MouseDown += ResBMouseDown;
            //ResizeAxButtonBottom.MouseUp += ResBMouseUp;
            //ResizeAxButtonBottom.MouseMove += AxResizeAreaBottom;

            //ResizeAreaButtonBottom.MouseDown += ResScndBMouseDown;
            //ResizeAreaButtonBottom.MouseUp += ResScndBMouseUp;
            //ResizeAreaButtonBottom.MouseMove += AxResizeBottom;

            //TxtBoxMin.KeyPress += TextBoxInputAnyReal;
            //TxtBoxMin.Enter += AxControlDetect;

            //for (int j = 0; j < 4; j++)
            //{
            //    Labels[j].MouseDown += AxControlDetect;
            //    Labels[j].DoubleClick += AxMouseDouble;
            //    Labels[j].MouseDown += ResBMouseDown;
            //    Labels[j].MouseMove += AxMBMove;
            //    Labels[j].MouseUp += MovBMouseUp;
            //}
            

            //TxtBoxMax.LostFocus += TextBoxMaxLeave;
            //TxtBoxMin.LostFocus += TextBoxMinLeave;
            //TxtBoxMin.KeyPress += TextBoxMinKeyPress;
            //TxtBoxMax.KeyPress += TextBoxMaxKeyPress;
        }

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
   }
}