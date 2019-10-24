using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrafeoLibrary
{
    public class AnalogGraphic : Graphic
    {
    #region Const
        //Количество точек добавляемых в серию
        private const int AnalogDotsMaxCount = 2000;
    #endregion Const

    #region Constructor
        public AnalogGraphic(/*FormGraphic formGraphic,*/ Param param, int num)
            : base(param, num)
        {
            SetInnerMinMax();
        }
    #endregion Constructor

        private readonly List<MomentValue<double>> _dots = new List<MomentValue<double>>();

        //минимум, максимум значений
        private bool _minMaxValueChanged = false; //используется для изменения _innerMin, innerMax
        private double? _minValue;
        private double? _maxValue;
        
        public double? MinValue
        {
            get { return _minValue; }
            private set 
            {
                if (value == _minValue) return;
                _minMaxValueChanged = true;
                _minValue = value;
            }
        }
        public double? MaxValue
        {
            get { return _maxValue; }
            private set
            {
                if (value == _maxValue) return;
                _minMaxValueChanged = true;
                _maxValue = value;
            }
        }

        //значения, используемое как min и max шкалы при отрисовке
        //см. функцию SetInnerMinMax
        private double _innerMin;
        private double _innerMax;

        public double InnerMin 
        {
            get
            {
                if (_minMaxValueChanged)
                {
                    SetInnerMinMax();
                    _minMaxValueChanged = false;
                }
                return _innerMin;
            }
            private set { _innerMin = value; }
        }
        public double InnerMax
        {
            get
            {
                if (_minMaxValueChanged)
                {
                    SetInnerMinMax();
                    _minMaxValueChanged = false;
                }
                return _innerMax;
            }
            private set { _innerMax = value; }
        }

    #region Overrride
        public override bool IsAnalog { get { return true; } }
        public override bool IsDigital { get { return false; } }
        public override string DataTypeString { get { return "Аналоговый"; } }


    #endregion Overrride

    #region ReDraw
        //Добавление точек для отрисовки
        internal override void ReDrawLin(DateTime timeBegin, DateTime timeEnd)
        {
            //if (!CheckNeedReDraw(timeBegin, timeEnd)) return;

            GraphicVisual.ClearPoints();
            //if (_dots.Count == 0) return;

            if (_dots.Count <= AnalogDotsMaxCount)
            {
                foreach (var dot in _dots)
                    GraphicVisual.AddPoint(dot.Time, dot.Mean);

                DrawState = EDrawState.AllDots;
            }
            else
            {
                TimeSpan ts = timeEnd.Subtract(timeBegin);
                double step = Math.Truncate(ts.TotalSeconds / AnalogDotsMaxCount);

                MomentValue<double> curDot = null;
                DateTime lastTime = timeBegin.AddSeconds(-2 * step);

                int i = 0;
                while ((i < _dots.Count) && (_dots[i].Time < timeBegin))
                {
                    curDot = _dots[i];
                    i++;
                }
                if (curDot != null)
                {
                    GraphicVisual.AddPoint(curDot.Time, curDot.Mean);
                    lastTime = curDot.Time;
                    curDot = null;
                }

                while ((i < _dots.Count) && (_dots[i].Time < timeEnd))
                {
                    bool fgFirst = false;

                    if (_dots[i].Time.Subtract(lastTime).TotalSeconds > step)
                    {
                        if (curDot != null) GraphicVisual.AddPoint(curDot.Time, curDot.Mean);
                        GraphicVisual.AddPoint(_dots[i].Time, _dots[i].Mean);
                        fgFirst = true;
                    }

                    MomentValue<double> dotFirst = _dots[i];
                    MomentValue<double> dotMin = _dots[i];
                    MomentValue<double> dotMax = _dots[i];
                    bool fgEx = false;

                    i++;
                    while ((i < _dots.Count) && (_dots[i].Time < dotFirst.Time.AddSeconds(step)))
                    {
                        curDot = _dots[i];

                        if (_dots[i].Mean < dotMin.Mean)
                        {
                            dotMin = _dots[i];
                            fgEx = true;
                        }
                        if (_dots[i].Mean > dotMax.Mean)
                        {
                            dotMax = _dots[i];
                            fgEx = false;
                        }

                        i++;
                    }

                    MomentValue<double> dot1 = fgEx ? dotMax : dotMin;
                    MomentValue<double> dot2 = fgEx ? dotMin : dotMax;

                    if ((!fgFirst) || (dotFirst != dot1))
                        GraphicVisual.AddPoint(dot1.Time, dot1.Mean);
                    if (dot2 != dot1)
                        GraphicVisual.AddPoint(dot2.Time, dot2.Mean);

                    if ((curDot != null) && (curDot.Mean == dot2.Mean)) curDot = null;
                    lastTime = (curDot == null) ? dot2.Time : curDot.Time;
                }

                if (curDot != null)
                    GraphicVisual.AddPoint(curDot.Time, curDot.Mean);
                if (i < _dots.Count) GraphicVisual.AddPoint(_dots[i].Time, _dots[i].Mean);
            }
        }

        internal override void ReDrawStep(DateTime timeBegin, DateTime timeEnd)
        {
            //if (!CheckNeedReDraw(timeBegin, timeEnd)) return;

            GraphicVisual.ClearPoints();

            if (_dots.Count == 0) return;

            //int j = -1;

            if (_dots.Count <= AnalogDotsMaxCount)
            {
                if (_dots[0].Time > timeBegin)
                    GraphicVisual.AddPoint(timeBegin, _dots[0].Mean, true);

                foreach (var dot in _dots)
                    GraphicVisual.AddPointStep(dot.Time, dot.Mean, dot.Nd != 0);

                var lastDot = _dots.Last();
                if (lastDot.Time < timeEnd)
                    GraphicVisual.AddPoint(timeEnd, lastDot.Mean, lastDot.Nd != 0);
            }
            else
            {
                TimeSpan ts = timeEnd.Subtract(timeBegin);
                double step = Math.Truncate(ts.TotalSeconds / AnalogDotsMaxCount);

                MomentValue<double> curDot = null;
                DateTime curTime;

                int i = 0;
                while ((i < _dots.Count) && (_dots[i].Time < timeBegin))
                {
                    curDot = _dots[i];
                    i++;
                }

                if (curDot != null)
                {
                    GraphicVisual.AddPoint(curDot.Time, curDot.Mean, curDot.Nd != 0);
                    curTime = curDot.Time;
                    curDot = null;
                }
                else
                {
                    if (_dots[0].Time > timeBegin)
                    {
                        GraphicVisual.AddPoint(timeBegin, _dots[0].Mean, true);
                        curTime = timeBegin;
                    }
                    else
                    {
                        curTime = timeBegin.AddSeconds(-2 * step);
                        curDot = _dots[0];
                        i++;
                    }
                }

                int n = i;
                while ((n < _dots.Count) && (_dots[i].Time <= timeEnd)) n++;

                if ((n - i) < AnalogDotsMaxCount)
                {
                    while ((i < _dots.Count) && (_dots[i].Time < timeEnd))
                    {
                        GraphicVisual.AddPointStep(_dots[i].Time, _dots[i].Mean, _dots[i].Nd != 0);
                        i++;
                    }
                }
                else //если точек много
                {
                    while ((i < _dots.Count) && (_dots[i].Time < timeEnd))
                    {
                        if (_dots[i].Time.Subtract(curTime).TotalSeconds > step)
                        {
                            if (curDot != null)
                                GraphicVisual.AddPoint(curDot.Time, curDot.Mean, curDot.Nd != 0);

                            GraphicVisual.DuplicatePoint(_dots[i].Time.AddMilliseconds(-1));
                        }

                        MomentValue<double> dotFirst = _dots[i];
                        MomentValue<double> dotMin = _dots[i];
                        MomentValue<double> dotMax = _dots[i];

                        DateTime lastTime = dotFirst.Time.AddSeconds(step);

                        curDot = null;

                        i++;
                        while ((i < _dots.Count) && (_dots[i].Time <= lastTime))
                        {
                            curDot = _dots[i];

                            if (_dots[i].Mean < dotMin.Mean)
                                dotMin = _dots[i];

                            if (_dots[i].Mean > dotMax.Mean)
                                dotMax = _dots[i];

                            i++;
                        }

                        MomentValue<double> dot1;
                        MomentValue<double> dot2;

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

                        bool isEmpty = (dot1.Nd != 0);
                        if ((dot2 == dot1) && (curDot != null)) isEmpty = (curDot.Nd != 0);
                        GraphicVisual.AddPoint(dot1.Time, dot1.Mean, isEmpty);

                        isEmpty = (dot2.Nd != 0);
                        if (curDot != null) isEmpty = (curDot.Nd != 0);

                        if (dot2 != dot1)
                            GraphicVisual.AddPoint(dot2.Time, dot2.Mean, isEmpty);

                        if (curDot != null)
                            if (curDot.Mean == dot2.Mean) curDot = null;

                        curTime = (curDot == null) ? dot2.Time : curDot.Time;
                    }

                    if (curDot != null)
                        GraphicVisual.AddPoint(curDot.Time, curDot.Mean, curDot.Nd != 0);
                }

                GraphicVisual.DuplicatePoint((i < _dots.Count) ? _dots[i].Time.AddMilliseconds(-1) : timeEnd);
            }
        }
    #endregion ReDraw

    #region Private
        private void SetInnerMinMax()
        {
            if ((Param.Min != null) && (Param.Max != null))
            {
                if (Param.Min < Param.Max)
                {
                    InnerMin = (double)Param.Min;
                    InnerMax = (double)Param.Max;
                }
                else if (Param.Min > Param.Max)
                {
                    InnerMin = (double)Param.Max;
                    InnerMax = (double)Param.Min;
                }
                else
                {
                    InnerMin = (double)Param.Min - 1;
                    InnerMax = (double)Param.Max + 1;
                }
            }
            else if (Param.Min != null)
            {
                double min = MinValue ?? 0;
                double max = MaxValue ?? 0;

                if (max >= Param.Min)
                {
                    InnerMin = (double)Param.Min;
                    if (max > 0)
                        InnerMax = max + (max - InnerMin) * 0.1;
                    else if (max < 0)
                        InnerMax = 0;
                    else InnerMax = 1;
                }
                else if (max < Param.Min)
                {
                    InnerMax = (double)Param.Min; 
                    if (min > 0)
                        InnerMin = 0;
                    else
                        InnerMin = min - (InnerMax - min) * 0.1;
                }
            }
            else if (Param.Max != null)
            {
                double min = MinValue ?? 0;
                double max = MaxValue ?? 0;

                if (min <= Param.Max)
                {
                    InnerMax = (double)Param.Max;
                    if (min > 0)
                        InnerMin = 0;
                    else if (min < 0)
                        InnerMin = min - (InnerMax - min) * 0.1;
                    else InnerMin = -1;
                }
                else
                {
                    InnerMin = (double)Param.Max;
                    if (max > 0)
                        InnerMax = max + (max - InnerMin) * 0.1;
                    else
                        InnerMax = 0;
                }
            }
            else
            {
                double min = MinValue ?? 0;
                double max = MaxValue ?? 0;

                if (min > 0) min = 0;
                if (max < 0) max = 0;

                if (max > 0) max = max + (max - min) * 0.1;
                if (min < 0) min = min - (max - min) * 0.1;
                if (min == max)
                {
                    min -= 1;
                    max += 1;
                }

                InnerMin = min;
                InnerMax = max;
            }
        }
    #endregion Private
    }
}
