using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace GrafeoLibrary
{
    public abstract class Graphic
    {
    #region Enum
        protected enum EDrawState //Состояние отрисовки графика
        {
            No,             //график не отрисован
            AllDots,        //отрисованы все точки графика
            DotsInInterval, //отрисованы все точки графика в заданном интервале
            SelectedDots    //график отрисован с процедурой проряжения
        }
    #endregion Enum

    #region Constructor
        protected Graphic(/*FormGraphic formGraphic,*/ Param param, int num)
        {
            //a FormGraphic = formGraphic;
            Param = param;
            Num = num;
            //a ChangeViewValues();
        }
    #endregion Constructor

    #region Abctract       
        public abstract bool IsAnalog { get; }
        public abstract bool IsDigital { get; }
        public abstract string DataTypeString { get; }

        public abstract int DotCount { get; }

        public abstract void ClearDots();

        //public abstract void AddDot(MomentValue dot);
        //public void AddDots(List<MomentValue> dots)
        //{
        //    foreach (var dot in dots) AddDot(dot);
        //}

        public abstract void AddValue(DateTime time, double val, int nd = 0);
        public abstract void AddValue(DateTime time, int val, int nd = 0);
        public abstract void AddValue(DateTime time, bool val, int nd = 0);
    #endregion Abctract

    #region ProtectedProp
        protected GraphicVisual GraphicVisual;
        //protected Series Series { get { return GraphicVisual.Series; } }
        //protected ChartArea Area { get { return GraphicVisual.Area; } }
        
        //Состояние отрисовки
        protected EDrawState DrawState = EDrawState.No;
        protected DateTime DrawTimeBegin;
        protected DateTime DrawTimeEnd;
    #endregion ProtectedProp
    
    #region Prop
        public Param Param { get; private set; }
        
        public int Num { get; private set; }
        
        public Color Color
        {
            get { return GraphicVisual.Color; }
            internal set { GraphicVisual.Color = value; } 
        }
        
        public virtual bool Visible
        {
            get { return GraphicVisual.Visible; }
            internal set
            {
                GraphicVisual.Visible = value;
                //a if (IsDiscret) ((DiscretGraphic)this).AxCap.Visible = value;
            }
        }

        public int LineWidth
        {
            get { return GraphicVisual.LineWidth; }
            internal set { GraphicVisual.LineWidth = value; }
        }
    #endregion Prop

    #region Internal
        internal virtual void NumDecrease()
        {
            Num--;
            GraphicVisual.Num = Num;
        }
    #endregion Internal

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
    }
}
