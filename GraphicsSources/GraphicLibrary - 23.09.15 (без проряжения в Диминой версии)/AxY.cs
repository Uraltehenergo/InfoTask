using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BaseLibrary;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace GraphicLibrary
{
    class AxY
    {
        internal Panel Ax = new Panel();
        internal Label[] La = new Label[4];
        internal Label AxCap;
        internal PictureBox ResizeAreaButtonBottom = new PictureBox();
        internal PictureBox ResizeAreaButtonTop = new PictureBox();
        internal PictureBox ResizeAxButtonBottom = new PictureBox();
        internal PictureBox ResizeAxButtonTop = new PictureBox();
        internal Panel RulePanel = new Panel();
        internal Button[] RulePBoxes = new Button[2];
        internal List<int> AxesOverlay = new List<int>();//список привязанных к оси графиков
        internal List<PictureBox> OverlayPicList = new List<PictureBox>(4);
        internal TextBox TBoxMax = new TextBox();
        internal TextBox TBoxMin = new TextBox();

        //Верхняя и нижняя границы по Y хранятся сразу в двух вариантах: абсолютном и процентном
        private double _viewMinMsr;
        internal double ViewMinMsr
        {
            get { return _viewMinMsr; }
            set 
            { 
                _viewMinMsr = value;
                if (UpperParam.PercentMode == PercentModeClass.Absolute)
                {
                    TBoxMin.Text = _viewMinMsr.ToString(DecPlacesMask);
                    TBoxMin.Select(0, 0);
                    LaFill(_viewMinMsr, _viewMaxMsr);
                }
            }
        }
        private double _viewMaxMsr;
        internal double ViewMaxMsr
        {
            get { return _viewMaxMsr; }
            set
            {
                _viewMaxMsr = value;
                if (UpperParam.PercentMode == PercentModeClass.Absolute)
                {
                    TBoxMax.Text = _viewMaxMsr.ToString(DecPlacesMask);
                    TBoxMax.Select(0, 0);
                    LaFill(_viewMinMsr, _viewMaxMsr);
                }
            }
        }
        private double _viewMinPrc;
        internal double ViewMinPrc
        {
            get { return _viewMinPrc; }
            set
            {
                _viewMinPrc = value;
                if (UpperParam.PercentMode == PercentModeClass.Percentage)
                {
                    TBoxMin.Text = _viewMinPrc.ToString(DecPlacesMask);
                    TBoxMin.Select(0, 0);
                    LaFill(_viewMinPrc, _viewMaxPrc);
                }
            }
        }
        private double _viewMaxPrc;
        internal double ViewMaxPrc
        {
            get { return _viewMaxPrc; }
            set
            {
                _viewMaxPrc = value;
                if (UpperParam.PercentMode == PercentModeClass.Percentage)
                {
                    TBoxMax.Text = _viewMaxPrc.ToString(DecPlacesMask);
                    TBoxMax.Select(0, 0);
                    LaFill(_viewMinPrc, _viewMaxPrc);
                }
            }
        }

        //Заполнение промежуточных подписей оси
        private void LaFill(double min, double max)
        {
            string mask = DecPlacesMask == null || DecPlacesMask.Length > 5 || DecPlacesMask == ""? mask = "0.###" : DecPlacesMask;
            for (int y = 0; y <= 3; y++)
            {
                La[y].Text = (min + (4 - y) * (-min + max) / 5).ToString(mask);
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

        //private Color _axColor;

        private PercentModeClass _percentMode;
        internal PercentModeClass PercentMode
        {
            get { return _percentMode; }
            set
            {
                _percentMode = value;
                if (AxCap != null)
                {
                    if (value == PercentModeClass.Percentage)
                    {
                        //if (AxCap.Text.Last() != ']') AxCap.Text = "[%]" + AxCap.Text;
                        if (AxCap.Text.First() != '%') AxCap.Text = "%" + AxCap.Text;
                    }
                    else
                    {
                        //if (AxCap.Text.Last() == ']') AxCap.Text = AxCap.Text.Remove(AxCap.Text.Length - 3, 3);
                        if (AxCap.Text.First() == '%') AxCap.Text = AxCap.Text.Remove(0, 1);
                    }
                }
            }
        }

        //Верхний параметр
        private GraphicParam _upperParam;
        internal GraphicParam UpperParam
        {
            get { return _upperParam; }
            set
            {
                try
                {
                    _upperParam = value;
                    AxCap.Text = IsOverlayed
                                     ? _upperParam.Index + "..."
                                     : _upperParam.Index.ToString();
                    AxCap.ForeColor = UpperParam.Series.Color;
                    Ax.Name = UpperParam.Index.ToString();
                    if (UpperParam.Series.Color != Color.Transparent)
                    {
                        TBoxMax.BackColor = UpperParam.Series.Color;
                        TBoxMin.BackColor = UpperParam.Series.Color;
                        foreach (var label in La)
                        {
                            label.ForeColor = UpperParam.Series.Color;
                        }
                    }
                    
                    PercentMode = UpperParam.PercentMode;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace + "\n" + e.Message);
                }
            }
        }

        //Спрятана ли ось
        internal bool IsHidden;

        internal AxY(GraphicParam upperP, int AxWidth)
        {
            Ax.BorderStyle = BorderStyle.Fixed3D;
            Ax.MinimumSize = new Size(0, 125);
            Ax.Name = upperP.Index.ToString();
            Ax.Width = AxWidth;
            
            AxCap = new Label
                        {
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204))),
                            Size = new Size(AxWidth, 17),
                            //Text = index.ToString(),
                            //ForeColor = color
                        };

            var npbR = new PictureBox();
            var npbB = new PictureBox();
            var npbL = new PictureBox();
            var npbT = new PictureBox();
            OverlayPicList.AddRange(new[] { npbL, npbB, npbR, npbT });

            ResizeAreaButtonTop.Size = new Size(AxWidth - 10, 3);
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
            Ax.Controls.Add(TBoxMin);
            Ax.Controls.Add(TBoxMax);
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

            TBoxMax.Dock = DockStyle.Top;
            TBoxMax.ForeColor = Color.White;

            ResizeAreaButtonBottom.Size = new Size(AxWidth - 10, 3);
            ResizeAreaButtonBottom.Cursor = Cursors.PanNorth;
            ResizeAreaButtonBottom.BackColor = SystemColors.GradientInactiveCaption;
            //ResizeAreaButtonBottom.Dock = DockStyle.Bottom;

            ResizeAxButtonBottom.Size = new Size(20, 3);
            ResizeAxButtonBottom.Cursor = Cursors.SizeNS;
            ResizeAxButtonBottom.BackColor = SystemColors.GradientInactiveCaption;
            ResizeAxButtonBottom.Dock = DockStyle.Bottom;

            TBoxMin.Dock = DockStyle.Bottom;
            TBoxMin.ForeColor = Color.White;

            RulePanel.Dock = DockStyle.Top;
            //ResizeAreaButtonTop.Dock = DockStyle.Top;
            ResizeAxButtonTop.Dock = DockStyle.Top;

            for (int j = 0; j < 4; j++)
            {
                La[j] = new Label
                {
                    //ForeColor = color,
                    Size = new Size(AxWidth, 20),
                    MinimumSize = new Size(AxWidth, 20),
                    AutoSize = true
                };
                Ax.Controls.Add(La[j]);
                La[j].Left = 0;
            }
            ResizeAreaButtonTop.Left = 3;
            ResizeAreaButtonBottom.Left = 3;
            ResizeAreaButtonTop.BringToFront();
            ResizeAreaButtonBottom.BringToFront();

            AxesOverlay.Add(upperP.Index);
            OverlayPicList[3].Height = 3;
            OverlayPicList[1].Height = 3;
            OverlayPicList[0].Width = 3;
            OverlayPicList[2].Width = 3;
            OverlayPicList[0].Dock = DockStyle.Left;
            OverlayPicList[1].Dock = DockStyle.Bottom;
            OverlayPicList[2].Dock = DockStyle.Right;
            OverlayPicList[3].Dock = DockStyle.Top;

            UpperParam = upperP;
        }

        //Маска для заполнения необходимого кол-ва символов после запятой
        internal string DecPlacesMask {get; private set;}
        internal void DecPlacesMaskFill(int dp)
        {
            if (dp == -1) DecPlacesMask = "";
            else
            {
                DecPlacesMask = "0.";
                for (int i = 0; i < dp; i++) DecPlacesMask += "#";
            }
        }
    }
}
