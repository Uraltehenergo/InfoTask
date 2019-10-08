using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Одно значение одной координаты графика
    public class GraficPoint
    {
        public GraficPoint(int dim, double mean)
        {
            Dimension = dim;
            Mean = mean;
            if (Dimension > 0) Points = new List<GraficPoint>();
        }

        //Размерность
        public int Dimension { get; private set; }
        //Mean - значение старшей координаты
        public double Mean { get; private set; }
        //Список n-1-мерных графиков
        public List<GraficPoint> Points { get; private set; }

        //Рекурсивно вызываемая процедура чтения значения точек графика
        public void ReadPoint(IRecordRead recg)
        {
            double r = recg.GetDouble("X" + Dimension);
            GraficPoint g;
            if (Points.Count == 0 || r != Points[Points.Count - 1].Mean)
            {
                g = new GraficPoint(Dimension - 1, r);
                Points.Add(g);
            }
            else
            {
                g = Points.Last();
            }
            if (Dimension > 1) g.ReadPoint(recg);
        }

        //Рекурсивно вызываемая процедура расчета значения по координатам
        public double Calculate(double[] x, GraficType type)
        {
            if (Dimension == 1) return Points[0].Mean;
            if (type != GraficType.Diagramm)
            {
                if (x[Dimension - 1] < Points[0].Mean)
                    return Points[0].Calculate(x, type);
                if (x[Dimension - 1] > Points[Points.Count - 1].Mean)
                    return Points[Points.Count - 1].Calculate(x, type);
            }
            int beg = 0;
            int en = Points.Count - 1;
            while (en - beg > 1)
            {
                int c = (en + beg) / 2;
                if (x[Dimension - 1] < Points[c].Mean) en = c;
                else beg = c;
            }
            double y1;
            switch (type)
            {
                case GraficType.Grafic0:
                    y1 = Points[beg].Calculate(x, type);
                    return y1;
                case GraficType.Grafic:
                    y1 = Points[beg].Calculate(x, type);
                    double y2 = Points[en].Calculate(x, type);
                    double x1 = Points[beg].Mean;
                    double x2 = Points[en].Mean;
                    if (x2 == x1) return double.NaN;
                    return y1 + ((y2 - y1) / (x2 - x1)) * (x[Dimension - 1] - x1);
                case GraficType.Diagramm:
                    if (Points[beg].Mean == x[Dimension - 1])
                    {
                        y1 = Points[beg].Calculate(x, type);
                        return y1;
                    }
                    if (Points[en].Mean == x[Dimension - 1])
                    {
                        y1 = Points[en].Calculate(x, type);
                        return y1;
                    }
                    return double.NaN;
            }
            return double.NaN;
        }
    }

    //---------------------------------------------------------------------------

    //Один график (расчетный параметр-график)
    public class Grafic : GraficPoint
    {
        public Grafic(IRecordRead recg, ThreadCalc thread)
            : base(recg.GetInt("Dimension"), 0)
        {
            Code = recg.GetString("Code");
            GraficType = recg.GetString("GraficType").ToGraficType();
            ThreadCalc = thread;
            while (!recg.EOF && recg.GetString("Code").ToLower() == Code.ToLower())
            {
                try
                {
                    ReadPoint(recg);
                    recg.Read();
                }
                catch (Exception ex)
                {
                    thread.AddError("Ошибка загрузки графика", ex, "График=" + Code);
                }
            }
        }

        //Код графика
        public string Code { get; private set; }
        //Тип графика
        public GraficType GraficType { get; private set; }
        //Поток
        public ThreadCalc ThreadCalc { get; private set; }

        public void CalculateMean(Moment[] par, Moment res)
        {
            int n = par.Count();
            var vpar = new double[n + 1];
            for (int i = 0; i < n; ++i)
                vpar[n - i] = par[i].Real;
            
            double d = Calculate(vpar, GraficType);
            if (d.Equals(double.NaN))
            {
                ThreadCalc.Funs.PutErr("Ошибка при вычислении значения по графику", res);
                d = 0;
            }
            res.Real = d;
        }
    }
}