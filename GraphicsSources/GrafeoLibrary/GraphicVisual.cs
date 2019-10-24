using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GrafeoLibrary
{
    public class GraphicVisual
    {
    #region static
        private static readonly OuterColorUseList Colors = new OuterColorUseList();

        internal static void FreeColors()
        {
            Colors.FreeAllColors();
        }

        internal static void FreeColor(Color color)
        {
            Colors.FreeColor(color);
        }
    #endregion static

    #region Prop
        public bool IsAnalog { get; private set; }
        public bool IsDigital { get { return !IsAnalog; } }
        public bool IsBackground { get; private set; }

        protected Series Series = new Series();
        protected ChartArea Area = new ChartArea();
    #endregion Prop

    #region Constructor
        internal GraphicVisual(int lineWidth, int num = 0, bool isAnalog = true, bool isBackground = false)
        {
            IsAnalog = isAnalog;
            IsBackground = isBackground;

            string sName = isBackground ? (isAnalog ? "AnalogBG" : "DiscretBG") : num.ToString();
            
            Series.Name = "Series" + sName;
            Area.Name = "ChartArea" + sName;
            Series.ChartArea = Area.Name;

            Init(lineWidth);
        }
    #endregion Constructor

    #region Init
        private void Init(int lineWidth)
        {
            Series.Legend = "Legend1";
            Series.BorderWidth = 2;

            Series.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
            Series.Color = IsBackground ? Color.Transparent : Colors.GetColor();
            
            if (IsAnalog)
            {
                Series.ChartType = SeriesChartType.FastLine;
                Series.EmptyPointStyle.Color = Series.Color;
                //p.DecPlaces = decPlaces == -1 ? DecPlacesDefault : decPlaces;

                //Из PrepareParam
                Area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                Area.AxisY.MajorGrid.LineColor = Series.Color;

                Series.BorderWidth = Math.Abs(lineWidth);
                Series.EmptyPointStyle.BorderWidth = Series.BorderWidth;
                
                Area.AxisX.MajorGrid.LineColor = Color.Transparent;
                Area.BackColor = Color.Transparent;
            }
            else
            {
                Series.ChartType = SeriesChartType.FastLine;
                Series.EmptyPointStyle.Color = Series.Color;

                //Из PrepareParam
                Area.AxisY.MajorGrid.Enabled = false;
            }
            
            Area.AxisX.IsStartedFromZero = false;
            Area.AxisX.LabelStyle.IsEndLabelVisible = false;
            Area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            Area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            Area.AxisX.IsLabelAutoFit = false;
            Area.AxisX.ScrollBar.Enabled = false;

            Area.AxisY.IsStartedFromZero = false;
            Area.AxisY.MajorTickMark.Enabled = false;
            Area.AxisY.LabelStyle.Enabled = false;
            Area.AxisY.ScrollBar.Enabled = false;

            Area.CursorX.IsUserEnabled = false; //ParamsAnalog.Count == 1;
            Area.CursorX.IsUserSelectionEnabled = false; //ParamsAnalog.Count == 1;
            Area.CursorX.LineColor = Color.Transparent;
            Area.CursorX.IntervalType = DateTimeIntervalType.Milliseconds;
            Area.CursorX.AutoScroll = false;

            Area.CursorY.LineColor = Color.Transparent;
            Area.CursorY.IntervalType = DateTimeIntervalType.Milliseconds;
            Area.CursorY.AutoScroll = false;

            //PrepareParam
            //Area.AxisX.IsLabelAutoFit = true;
            Area.AxisX.IsMarginVisible = false;
            
            Area.AxisX.MajorGrid.LineColor = Color.Black;

            if (IsBackground)
            {
                Area.BackColor = Color.White;
                
                Area.CursorX.IsUserEnabled = true;
                Area.CursorX.IsUserSelectionEnabled = true;
                Area.CursorX.LineColor = Color.Red;
                Area.CursorX.LineWidth = 2;
                Area.CursorX.SelectionColor = SystemColors.Highlight;

                if(IsAnalog)
                {
                    Area.CursorY.Interval = .001;
                    Area.CursorY.IntervalType = DateTimeIntervalType.Number;
                }
            } 
            else
            {
                Area.AxisX.LabelStyle.ForeColor = Color.Transparent;
            }
        }

        public int LineWidth
        {
            get { return Series.BorderWidth; }
            internal set
            {
                if (value >= 1)
                {
                    Series.BorderWidth = value;
                    Series.EmptyPointStyle.BorderWidth = Series.BorderWidth;
                }
            }
        }

        public Color Color
        {
            get { return Series.Color; }
            internal set
            {
                if (IsBackground) return;
                if (Colors.colorUseDictionary[value]) return;
                Colors.FreeColor(Series.Color);

                Series.Color = value;
                Series.EmptyPointStyle.Color = Series.Color;
                if (IsAnalog) Area.AxisY.MajorGrid.LineColor = Series.Color;
            }
        }

        public int Num
        {
            get
            {
                if (IsBackground) return 0;
                return int.Parse(Series.Name.Substring(6));
            }
            internal set
            {
                if (IsBackground) return;

                Series.Name = "Series" + value;
                Area.Name = "ChartArea" + value;
                Series.ChartArea = Area.Name;
            }
        }

        public bool Visible
        {
            get { return Area.Visible; }
            internal set { Area.Visible = value; }
        }
    #endregion Init

    #region PublicFunction
        internal void ClearPoints()
        {
            Series.Points.Clear();
        }

        internal void AddPoint(DateTime time, double value, bool isEmpty = false)
        {
            var i = Series.Points.AddXY(time, value);
            if (isEmpty) Series.Points[i].IsEmpty = true;
        }

        internal void DuplicatePoint(DateTime time)
        {
            if (Series.Points.Count > 0)
                AddPoint(time, Series.Points[Series.Points.Count].YValues[0],
                         Series.Points[Series.Points.Count].IsEmpty);
        }

        internal void AddPointStep(DateTime time, double value, bool isEmpty = false)
        {
            DuplicatePoint(time.AddMilliseconds(-1));
            AddPoint(time, value, isEmpty);
        }

        internal void AddPoint(DateTime time, bool value, bool isEmpty = false)
        {
            var i = Series.Points.AddXY(time, value);
            if (isEmpty) Series.Points[i].IsEmpty = true;
        }

        internal void AddPointStep(DateTime time, bool value, bool isEmpty = false)
        {
            if (Series.Points.Count > 0)
                AddPoint(time.AddMilliseconds(-1), Series.Points[Series.Points.Count].YValues[0],
                         Series.Points[Series.Points.Count].IsEmpty);

            AddPoint(time, value, isEmpty);
        }

        public double? ValueAt(DateTime time)
        {
            return ValueAt(time.ToOADate());
        }

        public double? ValueAt(double time)
        {
            double? res = null;

            foreach (var pt in Series.Points)
                if (pt.XValue <= time) res = pt.YValues[0];
                else break;

            return res;
        }

        public double? FirstValue
        {
            get
            {
                if (Series.Points.Count > 0) return Series.Points.First().YValues[0];
                return null;
            }
        }

        public double? LastValue
        {
            get
            {
                if (Series.Points.Count > 0) return Series.Points.Last().YValues[0];
                return null;
            }
        }
    #endregion PublicFunction

    #region ScaleView
        //Задаёт начальное и конечное значение Area по Y 
        //Значения всегда должны быть указаны в абсолютных величинах
        internal void SetAxisYScaleView(double minViewY, double maxViewY)
        {
            Area.AxisY.ScaleView.Position = minViewY;
            Area.AxisY.ScaleView.Size = maxViewY - minViewY;
            Area.AxisY.MajorGrid.Interval = (Area.AxisY.ScaleView.ViewMaximum - Area.AxisY.ScaleView.ViewMinimum)/5;
        }
    #endregion ScaleView
    }
}