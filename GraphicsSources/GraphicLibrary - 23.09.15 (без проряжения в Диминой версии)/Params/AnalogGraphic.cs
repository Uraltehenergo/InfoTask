using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using GraphicLibrary.Params;

namespace GraphicLibrary
{
    //Аналоговый график
    public class AnalogGraphic : Graphic
    {
        #region Const
        //Количество точек добавляемых в серию
        private const int AnalogDotsMaxCount = 2000;
        #endregion Const

        #region Props
        public override bool IsAnalog { get { return true; } }
        public override bool IsDiscret { get { return false; } }
        public override string DataTypeString { get { return "Аналоговый"; } }
        
        //Список значений
        private readonly List<MomentReal> _dots = new List<MomentReal>();
        internal List<MomentReal> Dots { get { return _dots; } }

        //Группа графиков
        internal GroupY GroupY { get; set; }
        #endregion Props

        #region Constructor
        internal AnalogGraphic(FormGraphic formGraphic, Param param, int num) : base(formGraphic, param, num)
        {
            int lineWidth = FormGraphic.LineWidth;
            GraphicVisual = new GraphicVisual(lineWidth, num);
            Area.AxisY.Minimum = Param.Min;
            Area.AxisY.Maximum = Param.Max;
            //Area.AxisX.Minimum = formGraphic.TimeBegin.ToOADate();
            //Area.AxisX.Maximum = formGraphic.TimeEnd.ToOADate();
            GroupY = null;
        }
        #endregion Constructor

        public override void ClearValues()
        {
            Dots.Clear();
        }
        
        //Добавить значение на график
        public void AddDot(MomentReal dot)
        {
            Dots.Add(dot);

            if ((_minValue == null) || (dot.Mean < _minValue)) _minValue = dot.Mean;
            if ((_maxValue == null) || (dot.Mean > _maxValue)) _maxValue = dot.Mean;
        }

        public override void AddValue(DateTime time, double val, int nd = 0)
        {
            var m = new MomentReal(time, val, nd);
            AddDot(m);
        }

        public override void AddValue(DateTime time, int val, int nd = 0)
        {
            var m = new MomentReal(time, val, nd);
            AddDot(m);
        }

        public override void AddValue(DateTime time, bool val, int nd = 0)
        {
            var m = new MomentReal(time, val ? 1 : 0, nd);
            AddDot(m);
        }

        public override void AddDot(MomentValue dot)
        {
            AddDot(dot.ToMomentReal());
        }

        public override double? MaxValue()
        {
            if (Dots.Count > 0)
            {
                double res = Dots[0].Mean;
                foreach (var dot in Dots)
                    if (dot.Mean > res) res = dot.Mean;
                return res;
            }
            return null;
        }

        public override double? MinValue()
        {
            if (Dots.Count > 0)
            {
                double res = Dots[0].Mean;
                foreach (var dot in Dots)
                    if (dot.Mean < res) res = dot.Mean;
                return res;
            }
            return null;
        }

        public override double? MaxValue(DateTime timeBegin, DateTime timeEnd)
        {
            if (Dots.Count > 0)
            {
                int i = 0;
                while ((i < Dots.Count) && (Dots[i].Time < timeBegin)) i++;

                if((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                {
                    double res = Dots[i].Mean;
                    i++;
                    while((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                    {
                        if (Dots[i].Mean > res) res = Dots[i].Mean;
                    }

                    return res;
                }
            }
            return null;
        }

        public override double? MinValue(DateTime timeBegin, DateTime timeEnd)
        {
            if (Dots.Count > 0)
            {
                int i = 0;
                while ((i < Dots.Count) && (Dots[i].Time < timeBegin)) i++;

                if ((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                {
                    double res = Dots[i].Mean;
                    i++;
                    while ((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                    {
                        if (Dots[i].Mean < res) res = Dots[i].Mean;
                    }

                    return res;
                }
            }
            return null;
        }

        public override double? ValueAt(DateTime time)
        {
            MomentReal dot = DotAt(time);
            return (dot != null) ? (double?) dot.Mean : null;
        }

        public override double? ValueAt(double time)
        {
            return ValueAt(DateTime.FromOADate(time));
        }

        public MomentReal DotAt(DateTime time)
        {
            MomentReal res = null;
            foreach (var dot in Dots)
                if (dot.Time <= time) res = dot;
                else break;
            return res;
        }

        public MomentReal DotAt(double time)
        {
            return DotAt(DateTime.FromOADate(time));
        }
        
        //Добавление точек для отрисовки
        internal override void ReDrawLin(DateTime timeBegin, DateTime timeEnd)
        {
            //if (!CheckNeedReDraw(timeBegin, timeEnd)) return;

            Series.Points.Clear();

            if (Dots.Count <= AnalogDotsMaxCount)
            {
                foreach (var dot in Dots)
                    Series.Points.AddXY(dot.Time, dot.Mean);

                DrawState = EDrawState.AllPts;
            }
            else
            {
                TimeSpan ts = timeEnd.Subtract(timeBegin);
                double step = Math.Truncate(ts.TotalSeconds / AnalogDotsMaxCount);

                MomentReal curDot = null;
                DateTime lastTime = timeBegin.AddSeconds(-2 * step);

                int i = 0;
                while ((i < Dots.Count) && (Dots[i].Time < timeBegin))
                {
                    curDot = Dots[i];
                    i++;
                }
                if (curDot != null)
                {
                    Series.Points.AddXY(curDot.Time, curDot.Mean);
                    lastTime = curDot.Time;
                    curDot = null;
                }

                while ((i < Dots.Count) && (Dots[i].Time < timeEnd))
                {
                    bool fgFirst = false;

                    if (Dots[i].Time.Subtract(lastTime).TotalSeconds > step)
                    {
                        if (curDot != null) Series.Points.AddXY(curDot.Time, curDot.Mean);
                        Series.Points.AddXY(Dots[i].Time, Dots[i].Mean);
                        fgFirst = true;
                    }

                    MomentReal dotFirst = Dots[i];
                    MomentReal dotMin = Dots[i];
                    MomentReal dotMax = Dots[i];
                    bool fgEx = false;

                    i++;
                    while ((i < Dots.Count) && (Dots[i].Time < dotFirst.Time.AddSeconds(step)))
                    {
                        curDot = Dots[i];

                        if (Dots[i].Mean < dotMin.Mean)
                        {
                            dotMin = Dots[i];
                            fgEx = true;
                        }
                        if (Dots[i].Mean > dotMax.Mean)
                        {
                            dotMax = Dots[i];
                            fgEx = false;
                        }

                        i++;
                    }

                    MomentReal dot1 = fgEx ? dotMax : dotMin;
                    MomentReal dot2 = fgEx ? dotMin : dotMax;

                    if ((!fgFirst) || (dotFirst != dot1))
                        Series.Points.AddXY(dot1.Time, dot1.Mean);
                    if (dot2 != dot1)
                        Series.Points.AddXY(dot2.Time, dot2.Mean);

                    if ((curDot != null) && (curDot.Mean == dot2.Mean)) curDot = null;
                    lastTime = (curDot == null) ? dot2.Time : curDot.Time;
                }

                if (curDot != null)
                    Series.Points.AddXY(curDot.Time, curDot.Mean);
                if (i < Dots.Count) Series.Points.AddXY(Dots[i].Time, Dots[i].Mean);
            }
        }

        internal override void ReDrawStep(DateTime timeBegin, DateTime timeEnd)
        {
            //if (!CheckNeedReDraw(timeBegin, timeEnd)) return;

            Series.Points.Clear();

            int j = -1;

            if (Dots.Count <= AnalogDotsMaxCount)
            {
                if (Dots[0].Time > timeBegin)
                {
                    Series.Points.AddXY(timeBegin, Dots[0].Mean);
                    j++;
                    Series.Points[j].IsEmpty = true;
                }

                foreach (var dot in Dots)
                {
                    if (j >= 0)
                    {
                        Series.Points.AddXY(dot.Time.AddMilliseconds(-1), Series.Points[j].YValues[0]);
                        j++;
                        if (Series.Points[j - 1].IsEmpty) Series.Points[j].IsEmpty = true;
                    }

                    Series.Points.AddXY(dot.Time, dot.Mean);
                    j++;
                    if (dot.Nd != 0) Series.Points[j].IsEmpty = true;
                }

                var lastDot = Dots.Last();
                if (lastDot.Time < timeEnd)
                {
                    Series.Points.AddXY(timeEnd, lastDot.Mean);
                    j++;
                    if (lastDot.Nd != 0) Series.Points[j].IsEmpty = true;
                }
            }
            else
            {
                TimeSpan ts = timeEnd.Subtract(timeBegin);
                double step = Math.Truncate(ts.TotalSeconds / AnalogDotsMaxCount);

                MomentReal curDot = null;
                DateTime curTime;

                int i = 0;
                while ((i < Dots.Count) && (Dots[i].Time < timeBegin))
                {
                    curDot = Dots[i];
                    i++;
                }

                if (curDot != null)
                {
                    Series.Points.AddXY(curDot.Time, curDot.Mean);
                    j++;
                    if (curDot.Nd != 0) Series.Points[j].IsEmpty = true;
                    curTime = curDot.Time;
                    curDot = null;
                }
                else
                {
                    if (Dots[0].Time > timeBegin)
                    {
                        Series.Points.AddXY(timeBegin, Dots[0].Mean);
                        j++;
                        Series.Points[j].IsEmpty = true;
                        curTime = timeBegin;
                    }
                    else
                    {
                        curTime = timeBegin.AddSeconds(-2 * step);
                        curDot = Dots[0];
                        i++;
                    }
                }

                int n = i;
                while ((n < Dots.Count) && (Dots[i].Time <= timeEnd)) n++;

                if ((n - i) < AnalogDotsMaxCount)
                {
                    while ((i < Dots.Count) && (Dots[i].Time < timeEnd))
                    {
                        Series.Points.AddXY(Dots[i].Time.AddMilliseconds(-1),
                                                  Series.Points[j].YValues[0]);
                        j++;
                        if (Series.Points[j - 1].IsEmpty) Series.Points[j].IsEmpty = true;

                        Series.Points.AddXY(Dots[i].Time, Dots[i].Mean);
                        j++;
                        if (Dots[i].Nd != 0) Series.Points[j].IsEmpty = true;
                    }
                }
                else //если точек много
                {
                    while ((i < Dots.Count) && (Dots[i].Time < timeEnd))
                    {
                        if (Dots[i].Time.Subtract(curTime).TotalSeconds > step)
                        {
                            if (curDot != null)
                            {
                                Series.Points.AddXY(curDot.Time, curDot.Mean);
                                j++;
                                if (curDot.Nd != 0) Series.Points[j].IsEmpty = true;
                            }

                            Series.Points.AddXY(Dots[i].Time.AddMilliseconds(-1),
                                                      Series.Points[j].YValues[0]);
                            j++;
                            if (Series.Points[j - 1].IsEmpty) Series.Points[j].IsEmpty = true;
                        }

                        MomentReal dotFirst = Dots[i];
                        MomentReal dotMin = Dots[i];
                        MomentReal dotMax = Dots[i];

                        DateTime lastTime = dotFirst.Time.AddSeconds(step);

                        curDot = null;

                        i++;
                        while ((i < Dots.Count) && (Dots[i].Time <= lastTime))
                        {
                            curDot = Dots[i];

                            if (Dots[i].Mean < dotMin.Mean)
                                dotMin = Dots[i];

                            if (Dots[i].Mean > dotMax.Mean)
                                dotMax = Dots[i];

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

                        Series.Points.AddXY(dot1.Time, dot1.Mean);
                        j++;
                        if (dot1.Nd != 0) Series.Points[j].IsEmpty = true;

                        if (dot2 != dot1)
                        {
                            Series.Points.AddXY(dot2.Time, dot2.Mean);
                            j++;
                            if (dot2.Nd != 0) Series.Points[j].IsEmpty = true;
                        }

                        if (curDot != null)
                        {
                            Series.Points[j].IsEmpty = (curDot.Nd != 0);
                            if (curDot.Mean == dot2.Mean) curDot = null;
                        }

                        curTime = (curDot == null) ? dot2.Time : curDot.Time;
                    }

                    if (curDot != null)
                    {
                        Series.Points.AddXY(curDot.Time, curDot.Mean);
                        j++;
                        if (curDot.Nd != 0) Series.Points[j].IsEmpty = true;
                    }
                }

                if (i < Dots.Count)
                {
                    Series.Points.AddXY(Dots[i].Time.AddMilliseconds(-1), Series.Points[j].YValues[0]);
                    j++;
                    if (Dots[i].Nd != 0) Series.Points[j].IsEmpty = true;
                }
                else
                {
                    Series.Points.AddXY(timeEnd, Series.Points[j].YValues[0]);
                    j++;
                    if (Series.Points[j - 1].IsEmpty) Series.Points[j].IsEmpty = true;
                }
            }
        }
    }
}