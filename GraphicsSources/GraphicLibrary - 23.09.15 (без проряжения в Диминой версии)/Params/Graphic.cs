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
        protected enum EDrawState { No, AllPts, PtsInInterval, SelectedPts }

        //ссылка на форму
        internal FormGraphic FormGraphic;
        //Ссылка на параметр
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
        }

        //Спрятан ли график
        internal bool IsHidden;

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

        //минимумб максимум
        protected double? _minValue;
        protected double? _maxValue;
        public double? MinimumValue
        {
            get { return _minValue; }
        }
        public double? MaximumValue
        {
            get { return _maxValue; }
        }
        
        #region Constructor
        internal Graphic(FormGraphic formGraphic, Param param, int num)
        {
            FormGraphic = formGraphic;

            Param = param;
            Num = num;
        }
        #endregion Constructor
        
        public abstract void ClearValues();

        public abstract void AddValue(DateTime time, double val, int nd = 0);
        public abstract void AddValue(DateTime time, int val, int nd = 0);
        public abstract void AddValue(DateTime time, bool val, int nd = 0);
        
        public abstract void AddDot(MomentValue dot);
        public void AddDots(List<MomentValue> dots)
        {
            foreach (var dot in dots) AddDot(dot);
        }

        public abstract double? MaxValue();
        public abstract double? MinValue();

        public abstract double? MaxValue(DateTime timeBegin, DateTime timeEnd);
        public abstract double? MinValue(DateTime timeBegin, DateTime timeEnd);
        
        public abstract double? ValueAt(DateTime time);
        public abstract double? ValueAt(double time);
        
        //public abstract MomentValue DotAt(DateTime time);
        //public abstract MomentValue DotAt(double time);

        internal abstract void ReDrawLin(DateTime timeBegin, DateTime timeEnd);
        internal abstract void ReDrawStep(DateTime timeBegin, DateTime timeEnd);

        protected bool CheckNeedReDraw(DateTime timeBegin, DateTime timeEnd)
        {
            if ((DrawState == EDrawState.AllPts) ||
                ((DrawState == EDrawState.PtsInInterval) && (timeBegin >= DrawTimeBegin) && (timeEnd <= DrawTimeEnd)))
                return false;

            return true;
        }
        
        #region NotAbstrct 
        public double ValueToPercent(double value)
        {
            return Param.ValueToPercent(value);
        }

        public double PercentToValue(double percentValue)
        {
            return Param.PercentToValue(percentValue);
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
        #endregion NotAbstrct 
    }

 }