using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphicLibrary.Params;

namespace GraphicLibrary
{
    //Дискретный график
    public class DiscretGraphic : Graphic
    {
        #region Const
        public override bool IsAnalog { get { return false; } }
        public override bool IsDiscret { get { return true; } }
        public override string DataTypeString { get { return "Дискретный"; } }

        //Количество точек добавляемых в серию
        private const int DiscretDotsMaxCount = 2000;
        #endregion Const

        #region Props
        //Список значений
        private readonly List<MomentBoolean> _dots = new List<MomentBoolean>();
        internal List<MomentBoolean> Dots { get { return _dots; } }

        //Метка с номером графика (вместо верт. оси)
        internal Label AxCap;

        #endregion Props

        #region Constructor
        internal DiscretGraphic(FormGraphic formGraphic, Param param, int num) : base(formGraphic, param, num)
        {
            int lineWidth = FormGraphic.LineWidth;
            //Возможно не надо lineWidth
            GraphicVisual = new GraphicVisual(lineWidth, num, false);
            Area.AxisY.Minimum = 0;
            Area.AxisY.Maximum = 1;
            //Area.AxisX.Minimum = formGraphic.TimeBegin.ToOADate();
            //Area.AxisX.Maximum = formGraphic.TimeEnd.ToOADate();
            Init();
        }

        private void Init()
        {
            AxCap = new Label
                        {
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204))),
                            Size = new Size(FormGraphic.AxWidth, FormGraphic.AxYLabel),
                            Text = Num.ToString(),
                            ForeColor = GraphicVisual.Series.Color
                        };
            
            AxCap.Click += FormGraphic.AxCapClick;
        }
        #endregion Constructor

        public override void ClearValues()
        {
            Dots.Clear();
        }

        //Добавить значение на график
        public void AddDot(MomentBoolean dot)
        {
            Dots.Add(dot);

            double val = dot.Mean ? 1 : 0;
            if ((_minValue == null) || (val < _minValue)) _minValue = val;
            if ((_maxValue == null) || (val > _maxValue)) _maxValue = val;
        }

        public override void AddValue(DateTime time, double val, int nd = 0)
        {
            var m = new MomentBoolean(time, val != 0, nd);
            AddDot(m);
        }

        public override void AddValue(DateTime time, int val, int nd = 0)
        {
            var m = new MomentBoolean(time, val != 0, nd);
            AddDot(m);
        }

        public override void AddValue(DateTime time, bool val, int nd = 0)
        {
            var m = new MomentBoolean(time, val, nd);
            AddDot(m);
        }

        public override void AddDot(MomentValue dot)
        {
            AddDot(dot.ToMomentBoolean());
        }

        public override double? MaxValue()
        {
            if (Dots.Count > 0)
            {
                if (Dots[0].Mean) return 1;
                foreach (var dot in Dots)
                    if (dot.Mean) return 1;
                return 0; 
            }
            return null;
        }

        public override double? MinValue()
        {
            if (Dots.Count > 0)
            {
                if (!Dots[0].Mean) return 0;
                foreach (var dot in Dots)
                    if (!dot.Mean) return 0;
                return 1;
            }
            return null;
        }

        public override double? MaxValue(DateTime timeBegin, DateTime timeEnd)
        {
            if (Dots.Count > 0)
            {
                //double res = Dots[0].Mean ? 1 : 0;
                //foreach (var dot in Dots)
                //    if (dot.Mean)
                //    {
                //        res = 1;
                //        break;
                //    }
                //return res;

                int i = 0;
                while ((i < Dots.Count) && (Dots[i].Time < timeBegin)) i++;

                if ((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                {
                    if (Dots[i].Mean) return 1;
                    
                    i++;
                    while ((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                    {
                        if (Dots[i].Mean) return 1;
                    }

                    return 0;
                }
            }
            return null;
        }

        public override double? MinValue(DateTime timeBegin, DateTime timeEnd)
        {
            if (Dots.Count > 0)
            {
                //double res = Dots[0].Mean ? 1 : 0;
                //foreach (var dot in Dots)
                //    if (!dot.Mean)
                //    {
                //        res = 0;
                //        break;
                //    }
                //return res;

                int i = 0;
                if ((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                {
                    if (!Dots[i].Mean) return 0;
                    
                    i++;
                    while ((i < Dots.Count) && (Dots[i].Time <= timeEnd))
                    {
                        if (!Dots[i].Mean) return 0;
                        break;
                    }

                    return 1;
                }
            }
            return null;
        }
        
        public MomentBoolean DotAt(DateTime time)
        {
            MomentBoolean res = null;
            foreach (var dot in Dots)
                if (dot.Time <= time) res = dot;
                else break;
            return res;
        }

        public MomentBoolean DotAt(double time)
        {
            return DotAt(DateTime.FromOADate(time));
        }

        public override double? ValueAt(DateTime time)
        {
            MomentBoolean dot = DotAt(time);
            return (dot != null) ? (double?)(dot.Mean ? 1 : 0) : null;
        }

        public override double? ValueAt(double time)
        {
            return ValueAt(DateTime.FromOADate(time));
        }

        //Добавление точек для отрисовки
        internal override void ReDrawLin(DateTime timeBegin, DateTime timeEnd)
        {
            //if (!CheckNeedReDraw(timeBegin, timeEnd)) return;

            Series.Points.Clear();

            if (Dots.Count <= DiscretDotsMaxCount)
                foreach (var dot in Dots)
                    Series.Points.AddXY(dot.Time, dot.Mean);
            else
            {
                TimeSpan ts = timeEnd.Subtract(timeBegin);
                double step = Math.Truncate(ts.TotalSeconds / DiscretDotsMaxCount);

                MomentBoolean curDot = null;
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

                    MomentBoolean dotFirst = Dots[i];
                    MomentBoolean dotMin = Dots[i];
                    MomentBoolean dotMax = Dots[i];
                    bool fgEx = false;

                    i++;
                    while ((i < Dots.Count) && (Dots[i].Time < dotFirst.Time.AddSeconds(step)))
                    {
                        curDot = Dots[i];

                        if (Dots[i].ToMomentReal().Mean < dotMin.ToMomentReal().Mean)
                        {
                            dotMin = Dots[i];
                            fgEx = true;
                        }
                        if (Dots[i].ToMomentReal().Mean > dotMax.ToMomentReal().Mean)
                        {
                            dotMax = Dots[i];
                            fgEx = false;
                        }

                        i++;
                    }

                    MomentBoolean dot1 = fgEx ? dotMax : dotMin;
                    MomentBoolean dot2 = fgEx ? dotMin : dotMax;

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

            if (Dots.Count <= DiscretDotsMaxCount)
            {
                if (Dots[0].Time > timeBegin)
                {
                    Series.Points.AddXY(timeBegin, Dots[0].ToMomentInteger().Mean);
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

                    Series.Points.AddXY(dot.Time, dot.ToMomentInteger().Mean);
                    j++;
                    if (dot.Nd != 0) Series.Points[j].IsEmpty = true;
                }

                var lastDot = Dots.Last();
                if (lastDot.Time < timeEnd)
                {
                    Series.Points.AddXY(timeEnd, lastDot.ToMomentInteger().Mean);
                    j++;
                    if (lastDot.Nd != 0) Series.Points[j].IsEmpty = true;
                }
            }
            else
            {
                TimeSpan ts = timeEnd.Subtract(timeBegin);
                double step = Math.Truncate(ts.TotalSeconds/DiscretDotsMaxCount);

                MomentBoolean curDot = null;
                DateTime curTime;

                int i = 0;
                while ((i < Dots.Count) && (Dots[i].Time < timeBegin))
                {
                    curDot = Dots[i];
                    i++;
                }

                if (curDot != null)
                {
                    Series.Points.AddXY(curDot.Time, curDot.ToMomentInteger().Mean);
                    j++;
                    if (curDot.Nd != 0) Series.Points[j].IsEmpty = true;
                    curTime = curDot.Time;
                    curDot = null;
                }
                else
                {
                    if (Dots[0].Time > timeBegin)
                    {
                        Series.Points.AddXY(timeBegin, Dots[0].ToMomentInteger().Mean);
                        j++;
                        Series.Points[j].IsEmpty = true;
                        curTime = timeBegin;
                    }
                    else
                    {
                        curTime = timeBegin.AddSeconds(-2*step);
                        curDot = Dots[0];
                        i++;
                    }
                }

                int n = i;
                while ((n < Dots.Count) && (Dots[i].Time <= timeEnd)) n++;

                if ((n - i) < DiscretDotsMaxCount)
                {
                    while ((i < Dots.Count) && (Dots[i].Time < timeEnd))
                    {
                        Series.Points.AddXY(Dots[i].Time.AddMilliseconds(-1),
                                            Series.Points[j].YValues[0]);
                        j++;
                        if (Series.Points[j - 1].IsEmpty) Series.Points[j].IsEmpty = true;

                        Series.Points.AddXY(Dots[i].Time, Dots[i].ToMomentInteger().Mean);
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
                                Series.Points.AddXY(curDot.Time, curDot.ToMomentInteger().Mean);
                                j++;
                                if (curDot.Nd != 0) Series.Points[j].IsEmpty = true;
                            }

                            Series.Points.AddXY(Dots[i].Time.AddMilliseconds(-1),
                                                Series.Points[j].YValues[0]);
                            j++;
                            if (Series.Points[j - 1].IsEmpty) Series.Points[j].IsEmpty = true;
                        }

                        MomentBoolean dotFirst = Dots[i];
                        MomentBoolean dotMin = Dots[i];
                        MomentBoolean dotMax = Dots[i];

                        DateTime lastTime = dotFirst.Time.AddSeconds(step);

                        curDot = null;

                        i++;
                        while ((i < Dots.Count) && (Dots[i].Time <= lastTime))
                        {
                            curDot = Dots[i];

                            if (Dots[i].ToMomentInteger().Mean < dotMin.ToMomentInteger().Mean)
                                dotMin = Dots[i];

                            if (Dots[i].ToMomentInteger().Mean > dotMax.ToMomentInteger().Mean)
                                dotMax = Dots[i];

                            i++;
                        }

                        MomentBoolean dot1;
                        MomentBoolean dot2;

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

                        Series.Points.AddXY(dot1.Time, dot1.ToMomentInteger().Mean);
                        j++;
                        if (dot1.Nd != 0) Series.Points[j].IsEmpty = true;

                        if (dot2 != dot1)
                        {
                            Series.Points.AddXY(dot2.Time, dot2.ToMomentInteger().Mean);
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
                        Series.Points.AddXY(curDot.Time, curDot.ToMomentInteger().Mean);
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