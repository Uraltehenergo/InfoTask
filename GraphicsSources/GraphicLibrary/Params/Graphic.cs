using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GraphicLibrary.Params;
using System.Drawing;

namespace GraphicLibrary
{
    //Один график для отображения
    public abstract class Graphic
    {
        protected enum EDrawState { No, AllDots, DotsInInterval, SelectedDots }
        /* No - график не отрисован
         * AllPts - отрисованы все точки графика
         * PtsInInterval - отрисованы все точки графика в заданном интервале
         * SelectedPts - график отрисован с процедурой проряжения
         */

    #region Prop
        internal FormGraphic FormGraphic;
        public Param Param { get; private set; }

        //тип графика
        public abstract bool IsAnalog { get; }
        public abstract bool IsDiscret { get; }
        public abstract string DataTypeString { get; } 
        
        //Внутренний номер графика
        internal int Num { get; private set; }

        internal void NumDecrease()
        {
            Num--;

            //переименование Aria и Series
            Series.Name = "Series" + Num;
            Area.Name = "ChartArea" + Num;
            Series.ChartArea = Area.Name;

            if(IsDiscret) ((DiscretGraphic)this).AxCap.Text = Num.ToString();
        }

        //Спрятан ли график
        private bool _isVisible = true;
        internal bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                Area.Visible = value;
                if(IsDiscret) ((DiscretGraphic) this).AxCap.Visible = value;
                _isVisible = value;
            }
        }

        //динамический график
        internal int FirstWaitingDotNum; //номер первой точки, ожидающей отрисовку
        
        //Состояние отрисовки
        protected EDrawState DrawState = EDrawState.No;
        protected DateTime DrawTimeBegin;
        protected DateTime DrawTimeEnd;
        
        //Серия и Арея
        internal GraphicVisual GraphicVisual;
        internal Series Series { get { return GraphicVisual.Series; } }
        internal ChartArea Area { get { return GraphicVisual.Area; } }

        //минимум, максимум
        public double? MinValue { get; protected set; }
        public double? MaxValue { get; protected set; }

        //ширина линии
        internal int LineWidth
        {
            get { return Series.BorderWidth; }
            set 
            {  
                Series.BorderWidth = value;
                Series.EmptyPointStyle.BorderWidth = Series.BorderWidth;
            }
        }

        //Min, Max
        protected bool MinMaxChanged = false;
        private double _minViewValue;
        private double _maxViewValue;
        
        public double MinViewValue
        {
            get
            {
                if (MinMaxChanged)
                {
                    ChangeViewValues();
                    MinMaxChanged = false;
                }
                return _minViewValue;
            }
        }
        
        public double MaxViewValue
        {
            get
            {
                if (MinMaxChanged)
                {
                    ChangeViewValues();
                    MinMaxChanged = false;
                }
                return _maxViewValue;
            }
        }
    #endregion Prop
        
    #region Constructor
        internal Graphic(FormGraphic formGraphic, Param param, int num)
        {
            FormGraphic = formGraphic;
            Param = param;
            Num = num;
            ChangeViewValues();
        }
    #endregion Constructor

    #region Abstract
        public abstract void ClearDots();

        public abstract void AddDot(MomentValue dot);
        public void AddDots(List<MomentValue> dots)
        {
            foreach (var dot in dots) AddDot(dot);
        }

        public abstract void AddValue(DateTime time, double val, int nd = 0);
        public abstract void AddValue(DateTime time, int val, int nd = 0);
        public abstract void AddValue(DateTime time, bool val, int nd = 0);
        
        public abstract double? MaxValueIn(DateTime timeBegin, DateTime timeEnd);
        public abstract double? MinValueIn(DateTime timeBegin, DateTime timeEnd);
        
        public abstract double? ValueAt(DateTime time);
        public abstract double? ValueAt(double time);
        
        //public abstract MomentValue DotAt(DateTime time);
        //public abstract MomentValue DotAt(double time);
    #endregion Abstract

    #region ReDraw
        internal abstract void ReDrawLin(DateTime timeBegin, DateTime timeEnd);
        internal abstract void ReDrawStep(DateTime timeBegin, DateTime timeEnd);

        protected bool CheckNeedReDraw(DateTime timeBegin, DateTime timeEnd)
        {
            if ((DrawState == EDrawState.AllDots) ||
                ((DrawState == EDrawState.DotsInInterval) && (timeBegin >= DrawTimeBegin) && (timeEnd <= DrawTimeEnd)))
                return false;

            return true;
        }
    #endregion ReDraw

    #region Public
        public double ValueToPercent(double value)
        {
            //return Param.ValueToPercent(value);
            if (MinViewValue == MaxViewValue) return 100;
            return (value - MinViewValue) / (MaxViewValue - MinViewValue) * 100;
        }

        public double PercentToValue(double percentValue)
        {
            //return Param.PercentToValue(percentValue);
            return percentValue * (MaxViewValue - MinViewValue) / 100 + MinViewValue;
        }

        public int DotCount
        {
            get
            {
                if (IsAnalog) return ((AnalogGraphic)this).Dots.Count;
                return ((DiscretGraphic)this).Dots.Count;
            }
        }

        public MomentValue Dot(int i)
        {
            if ((i >= 0) && (i < DotCount))
                if (IsAnalog)
                    return ((AnalogGraphic)this).Dots[i];
                else
                    return ((DiscretGraphic) this).Dots[i];
            
            return null;
        }
    #endregion Public

    #region Private
        private void ChangeViewValues()
        {
            if ((Param.OutMin != null) && (Param.OutMax != null))
            {
                if (Param.OutMin < Param.OutMax)
                {
                    _minViewValue = (double)Param.OutMin;
                    _maxViewValue = (double)Param.OutMax;
                }

                if (Param.OutMin > Param.OutMax)
                {
                    _minViewValue = (double)Param.OutMax;
                    _maxViewValue = (double)Param.OutMin;
                }

                _minViewValue = (double)Param.OutMin - 1;
                _maxViewValue = (double)Param.OutMax + 1;
            }
            else if (Param.OutMin != null)
            {
                double min = MinValue ?? 0;
                double max = MaxValue ?? 0;

                if (max > Param.OutMin)
                {
                    _minViewValue = (double)Param.OutMin;
                    if (max > 0)
                        _maxViewValue = max + (max - MinViewValue) * 0.1;
                    else
                        _maxViewValue = 0;
                }
                else
                {
                    _maxViewValue = (double)Param.OutMin;
                    if (min > 0)
                        _minViewValue = 0;
                    else
                        _minViewValue = min - (MaxViewValue - min) * 0.1;
                }
            }
            else if (Param.OutMax != null)
            {
                double min = MinValue ?? 0;
                double max = MaxValue ?? 0;

                if (min < Param.OutMax)
                {
                    _maxViewValue = (double)Param.OutMax;
                    if (min > 0)
                        _minViewValue = 0;
                    else
                        _minViewValue = min - (MaxViewValue - min) * 0.1;
                }
                else
                {
                    _minViewValue = (double)Param.OutMax;
                    if (max > 0)
                        _maxViewValue = max + (max - MinViewValue) * 0.1;
                    else
                        _maxViewValue = 0;
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
                    min = -1;
                    max = 1;
                }

                _minViewValue = min;
                _maxViewValue = max;
            }
        }
    #endregion Protected
    }
 }