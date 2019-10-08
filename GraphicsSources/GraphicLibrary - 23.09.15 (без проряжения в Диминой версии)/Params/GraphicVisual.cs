using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Linq;
using System.Text;


namespace GraphicLibrary
{
    internal class GraphicVisual
    {
        private static readonly OuterColorUseList Colors = new OuterColorUseList();

        internal static void FreeColors()
        {
            Colors.FreeAllColors();
        }
        
        internal bool IsAnalog { get; private set; }            //аналоговый/дискретный
        internal bool IsDiscret { get { return !IsAnalog; } }   //дискретный/аналоговый
        internal bool IsBackground { get; private set; }        //фоновый/нет

        internal Series Series = new Series();
        internal ChartArea Area = new ChartArea();

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
            } 
            else
            {
                Area.AxisX.LabelStyle.ForeColor = Color.Transparent;
            }
        }

        private void InitEvent()
        {
            
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

        public double? LastValue()
        {
            if (Series.Points.Count > 0) return Series.Points.Last().YValues[0];
            return null;
        }
    }
}
