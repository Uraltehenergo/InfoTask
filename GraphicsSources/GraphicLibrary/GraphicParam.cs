using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BaseLibrary;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace GraphicLibrary
{
    public class GraphicParam
    {
        public GraphicParam()
        {
            V();
        }

        //internal GraphicParam(IRecordRead rec)
        //{
        //    Code = rec.GetString("Code");
        //    Name = rec.GetString("Name");
        //    Comment = rec.GetString("Comment");
        //    Units = rec.GetString("Units");
        //    DataType = Conv.StringToDataType(rec.GetString("DataType"));
        //    V();
        //}

        public GraphicParam(string code, string name, string comment, string units, DataType dataType, double min = 0, double max = 1.0)
        {
            Code = code;
            Name = name;
            Comment = comment;
            Units = units;
            DataTypeD = dataType;
            Min = min;
            Max = max;
            V();
        }

        private void V() //внутренняя отрисовка каких-то вещей
        {
            Area.AxisX.IsStartedFromZero = false;
            Area.AxisY.IsStartedFromZero = false;
            //if (DataTypeD == BaseLibrary.DataType.Boolean) Series.ChartType = SeriesChartType.StackedArea;
            //else Series.ChartType = SeriesChartType.FastLine;
            Series.Legend = "Legend1";
            Series.BorderWidth = 2;
            //Series.EmptyPointStyle.Color = Series.Color;
            Series.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;

            AxCap = new Label
                        {
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204)))
                        };
            Area.AxisX.LabelStyle.IsEndLabelVisible = false;
            Area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            Area.AxisY.MajorTickMark.Enabled = false;
            Area.AxisY.LabelStyle.Enabled = false;
            Area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            Area.AxisX.IsLabelAutoFit = false;
            Area.AxisX.ScrollBar.Enabled = false;
            Area.AxisY.ScrollBar.Enabled = false;

            Area.CursorX.IsUserEnabled = false; //ParamsAnalog.Count == 1;
            Area.CursorX.IsUserSelectionEnabled = false; //ParamsAnalog.Count == 1;
            Area.CursorX.LineColor = Color.Transparent;
            Area.CursorX.IntervalType = DateTimeIntervalType.Milliseconds;
            Area.CursorX.AutoScroll = false;

            Area.CursorY.LineColor = Color.Transparent;
            Area.CursorY.IntervalType = DateTimeIntervalType.Milliseconds;
            Area.CursorY.AutoScroll = false;
        }

        //добавление элемента из архива
        //public void AddBefore(MomentValue dot)
        //{
        //    if (dot.Undef % 2 != 0) dot.ToMomentReal().Mean = Dots.ParamsCount != 0 ? Dots.Last().ToMomentReal().Mean : 0;
        //    Dots.Insert(0, dot);
        //}
        //public void AddBefore(List<MomentValue> dots)
        //{
        //    if (dots[0].Undef % 2 != 0) dots.Insert(0, new MomentReal(dots[0].Time, 0, -1, 0, 0));
        //    for (int index = dots.ParamsCount-1; index >= 0; index--)
        //    {
        //        var momentValue = dots[index];
        //        if (momentValue.Undef % 2 != 0) momentValue.ToMomentReal().Mean = dots[index + 1].ToMomentReal().Mean;
        //        Dots.Insert(0, momentValue);
        //    }
        //}

        //добавление элемента
        public void AddValues(MomentValue dot)
        {
            if (!IsUpdated)
            {
                IsUpdated = true;
                NumberOfFirstWaiting = Dots.Count;
            }
            //if (dot.Nd % 2 != 0) dot.ToMomentReal().Mean = Dots.Count != 0 ? Dots.Last().ToMomentReal().Mean : 0;
            Dots.Add(dot);
        }

        public void AddValues(List<MomentValue> dots)
        {
            //if (dots[0].Nd % 2 != 0) dots.Insert(0, new MomentReal(dots[0].Time.AddMilliseconds(-1), 0, 0));

            if (!IsUpdated)
            {
                IsUpdated = true;
                NumberOfFirstWaiting = Dots.Count;
            }

            foreach (var momentValue in dots)
            {
                //if (momentValue.Nd % 2 != 0) momentValue.ToMomentReal().Mean = Dots.Last().ToMomentReal().Mean;
                Dots.Add(momentValue);
            }
            //Dots.Sort(CompareMomentVal);
        }

        //тут не реализована проверка на отрисовываемость (т.к. только тормозить будет для наших задач)
        public void AddValues(DateTime time, bool val, int nd = 0)
        {
            //if (nd % 2 != 0) dot.ToMomentReal().Mean = Dots.ParamsCount != 0 ? Dots.Last().ToMomentReal().Mean : 0;
            Dots.Add(new MomentBoolean(time, val, nd));
        }
        public void AddValues(DateTime time, int val, int nd = 0)
        {
            //if (nd % 2 != 0) dot.ToMomentReal().Mean = Dots.ParamsCount != 0 ? Dots.Last().ToMomentReal().Mean : 0;
            Dots.Add(new MomentInteger(time, val, nd));
        }
        public void AddValues(DateTime time, double val, int nd = 0)
        {
            //if (nd % 2 != 0) dot.ToMomentReal().Mean = Dots.ParamsCount != 0 ? Dots.Last().ToMomentReal().Mean : 0;
            Dots.Add(new MomentReal(time, val, nd));
        }

        internal double MaxVal ()
        {
            //if (Series.Points.Count == 0) return 1;
            double m = Series.Points.First().YValues.First();
            m = Series.Points.Select(point => point.YValues.First()).Concat(new[] {m}).Max();
            return m;
        }
        internal double MinVal()
        {
            //if (Series.Points.Count == 0) return 0;
            double m = Series.Points.First().YValues.First();
            m = Series.Points.Select(point => point.YValues.First()).Concat(new[] { m }).Min();
            return m;
        }

        //internal void ToRecordset(RecDao rec)
        //{
        //    rec.Put("Code", Code);
        //    rec.Put("Name", Name);
        //    rec.Put("Comment", Comment);
        //    rec.Put("Units", Units);
        //    rec.Put("DataType", Conv.DataTypeToEnglish(DataType));
        //}

        public void ClearValues()
        {
            _dots.Clear();
        }

        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                Area.Name = "ChartArea" + _code;
                Series.ChartArea = "ChartArea" + _code;
                Series.Name = "Series" + _code;
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubName { get; set; }

        private string _localDataType;
        public string DataType
        {
            get { return _localDataType; }
            set
            {
                _localDataType = value;
                _dataTypeD = value.ToDataType();
            }
        }
        private DataType _dataTypeD;
        public DataType DataTypeD
        {
            get { return _dataTypeD; }
            set
            {
                _dataTypeD = value;
                _localDataType = value.ToEnglish();
            }
        }

        public double PtkMin { get; set; }
        public double PtkMax { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public string Units { get; set; }
        public string Comment { get; set; }
        public string Tag { get; set; }
        internal string DataTypeString;

        private readonly List<MomentValue> _dots = new List<MomentValue>();
        internal List<MomentValue> Dots { get { return _dots; } }

        internal ChartArea Area = new ChartArea();
        internal Series Series = new Series();
        internal Label AxCap;

        internal double CurAxSize = 1; //текущий размер оси в процентах от эталона
        internal double CurAxPos = 0;//текущее положение оси в процентах

        internal bool IsUpdated = false;//лежит ли что-нибудь в очереди на отрисовку
        internal int NumberOfFirstWaiting;//номер первой точки, ожидающей отрисовку
        internal int NumberOfLastViewed;//номер последней точки последнего отрисованного блока

        private PercentModeClass _percentMode;
        internal PercentModeClass PercentMode
        {
            get { return _percentMode; }
            set
            {
                _percentMode = value;
                if (AxY != null) AxY.PercentMode = value;
            }
        }
        internal double ValueToPercent(double xD)
        {
            double rD = (xD - Min)/(Max - Min)*100;
            return rD;
        }
        internal double PercentToValue (double xD)
        {
            return xD*(Max - Min)/100 + Min;
        }

        private int _index;
        public int Index
        {
            get { return _index; }
            internal set
            {
                _index = value;
                if (AxY != null) AxY.Ax.Name = value.ToString();
            }
        }

        private bool _isVisible = true;
        internal bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                //Area.Visible = value;
            }
        }

        //Авто ли шкала по Y
        internal bool IsYScaledAuto;

        //Кол-во цифр после запятой
        internal int DecPlaces;

        internal AxY AxY;

        public double LastVal()
        {
            if (Series.Points.Count > 0)
            return Series.Points.Last().YValues[0];
            return 0;
        }
    }

    public enum PercentModeClass { Absolute, Percentage, NotDefined }
}
