using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace GrafeoLibrary
{
    internal abstract class Gv
    {
        protected Series Series = new Series();
        protected ChartArea Area = new ChartArea();

        internal Gv(int lineWidth, int num = 0)
        {
            var sName = num.ToString();
            
            Series.Name = "Series" + sName;
            Area.Name = "ChartArea" + sName;
            Series.ChartArea = Area.Name;

            Init();
        }

        private void Init()
        {
            Series.Legend = "Legend1";
            Series.BorderWidth = 2;

            Series.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
            Series.ChartType = SeriesChartType.FastLine;

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

            //Area.AxisX.IsLabelAutoFit = true;
            Area.AxisX.IsMarginVisible = false;

            Area.AxisX.MajorGrid.LineColor = Color.Black;
        }

    #region Props
        internal string Name
        {
            get { return Series.Name.Substring(6); }
            set
            {
                Series.Name = "Series" + value;
                Area.Name = "ChartArea" + value;
                Series.ChartArea = Area.Name;
            }
        }
        
        internal int LineWidth
        {
            get { return Series.BorderWidth; }
            set
            {
                if (value >= 1)
                {
                    Series.BorderWidth = value;
                    Series.EmptyPointStyle.BorderWidth = Series.BorderWidth;
                }
            }
        }
    #endregion Props

    #region PublicFunction
        internal double? ValueAt(DateTime time)
        {
            return ValueAt(time.ToOADate());
        }

        internal double? ValueAt(double time)
        {
            double? res = null;

            foreach (var pt in Series.Points)
                if (pt.XValue <= time) res = pt.YValues[0];
                else break;

            return res;
        }

        internal double? FirstValue
        {
            get
            {
                if (Series.Points.Count > 0) return Series.Points.First().YValues[0];
                return null;
            }
        }

        internal double? LastValue
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
            Area.AxisY.MajorGrid.Interval = (Area.AxisY.ScaleView.ViewMaximum - Area.AxisY.ScaleView.ViewMinimum) / 5;
        }
    #endregion ScaleView
    }
}
