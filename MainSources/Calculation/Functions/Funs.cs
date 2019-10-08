using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public partial class Funs
    {
        //Делегат для ссылок на функции scalar, par - парметры, res - результат, calc - расчетный параметр выполнения
        public delegate void ScalarDelegate(Moment[] par, Moment res);
        //numpar - номера параметров по времени значения которых идет расчет, возвращает false, если значения нет
        public delegate bool ComplicateDelegate(Moment[] par, Moment res, ISet<int> numpar);
        //Делегат для ссылок на функции list
        public delegate void ListDelegate(SingleValue[] par, SingleValue res);
        //Делегат для ссылок на статистические функции
        public delegate Moment AgregateDelegate(SingleValue par, DateTime beg, DateTime end);
        //Делегат для ссылок на функции array
        public delegate CalcValue ArrayDelegate(CalcValue[] par, CalcParamRun calc);

        //Текущий расчетный параметр 
        private CalcParam _calc;
        //Текущая ошибка для скалярных функций
        private ErrorCalc _errorScalar;
        //Текущие исследуемый период и интерполяция
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        private InterpolationType _interpol;

        //Вычисление значения функций типа scalar и list
        public CalcValue GeneralFunction(CalcValue[] par, DataType dt, CalcParamRun calc, ScalarDelegate scalar, ListDelegate fun)
        {
            bool isScalar = scalar != null;
            int n = par.Count();
            _calc = calc.CalcParam;
            Begin = calc.ThreadCalc.PeriodBegin;
            End = calc.ThreadCalc.PeriodEnd;
            _interpol = _calc.Interpolation;
            var sv = new SingleValue[n];
            int ki = 0, ks = 0;
            for (int i = 0; i < n; i++)
            {
                var cvt = par[i].Type;
                if (ki == 0 && cvt == CalcValueType.IntArray) ki = i;
                if (ks == 0 && cvt == CalcValueType.StringArray) ks = i;
                if (cvt != CalcValueType.IntArray && cvt != CalcValueType.StringArray)
                    sv[i] = par[i].SingleValue;
            }
            if (ki == 0 && ks == 0)//Нет массивов
            {
                if (isScalar) 
                    return new CalcValue(ScalarFunction(sv, dt, scalar));
                var ss = new SingleValue();
                foreach (var s in sv) ss.Error |= s.Error;
                fun(sv, ss);
                if (ss.Type != SingleType.Segments && ss.Moment == null && ss.Moments == null )
                    ss.Moments = new List<Moment>();
                if (ss.Type == SingleType.Segments && ss.Segments == null) 
                    ss.Segments = new List<Segment>();
                return new CalcValue(ss);
            }
            var cv = new CalcValue[n];
            if (ki > 0)//Массивы с целыми индексами
            {
                var resi = new CalcValue(new SortedDictionary<int, VarRun>());
                foreach (var key in par[ki].IntArray.Keys)
                {
                    bool e = true;
                    for (int i = 0; i < n; i++)
                    {
                        var p = par[i];
                        if (p.Type == CalcValueType.StringArray || (p.Type == CalcValueType.IntArray && !p.IntArray.ContainsKey(key)))
                            e = false;
                        else if (p.Type == CalcValueType.IntArray)
                            cv[i] = p.IntArray[key].CalcValue;
                        else cv[i] = p;
                    }
                    if (e)
                    {
                        var c = GeneralFunction(cv, dt, calc, scalar, fun);
                        resi.Error |= c.Error;
                        resi.IntArray.Add(key, new VarRun(c));
                    }
                }
                return resi;
            }
            //Массивы со строковыми индексами
            var ress = new CalcValue(new SortedDictionary<int, VarRun>());
            foreach (var key in par[ki].StringArray.Keys)
            {
                bool e = true;
                for (int i = 0; i < n; i++)
                {
                    var p = par[i];
                    if (p.Type == CalcValueType.IntArray || (p.Type == CalcValueType.StringArray && !p.StringArray.ContainsKey(key)))
                        e = false;
                    else if (p.Type == CalcValueType.StringArray)
                        cv[i] = p.StringArray[key].CalcValue;
                    else cv[i] = p;
                }
                if (e)
                {
                    var c = GeneralFunction(cv, dt, calc, scalar, fun);
                    ress.Error |= c.Error;
                    ress.StringArray.Add(key, new VarRun(c));
                }
            }
            return ress;
        }

        //Интерполяция типа type значений list на время time по точке с номером n и следующим за ней
        private Moment Interpolation(InterpolationType type, List<Moment> list, int n, DateTime time)
        {
            if (list == null || list.Count == 0) return null;
            if (n >= 0 && time == list[n].Time) return list[n].Clone();
            DataType dt = list[0].DataType;
            if (type == InterpolationType.Constant || dt == DataType.String || n < 0 || n >= list.Count - 1)
                return list[n == -1 ? 0 : n].Clone(time);

            var seg = new[] {list[n], list[n+1]};
            var res = new Moment(dt, time, MaxErr(seg), MaxNd(seg));
            if (dt.LessOrEquals(DataType.Real))
            {
                double x1 = list[n + 1].Real;
                double x0 = list[n].Real;
                double t = time.Subtract(list[n].Time).TotalSeconds;
                double t0 = list[n + 1].Time.Subtract(list[n].Time).TotalSeconds;
                if (t0 == 0 || t == 0) res.Real = x0;
                else res.Real = x0 + t * (x1 - x0) / t0;
            }
            else
            {
                DateTime x1D = list[n + 1].Date;
                DateTime x0D = list[n].Date;
                double t = time.Subtract(list[n].Time).TotalSeconds;
                double t0 = list[n + 1].Time.Subtract(list[n].Time).TotalSeconds;
                if (t0 == 0 || t == 0) res.Date = x0D;
                else res.Date = x0D.AddSeconds(t * x1D.Subtract(x0D).TotalSeconds / t0);
            }
            return res;
        }

        //Вычисления значения скалярной функции, параметры: par - списки Moment, dt - тип данных результата, 
        public SingleValue ScalarFunction(SingleValue[] par, DataType dt, ScalarDelegate scalar, ComplicateDelegate comp = null)
        {
            ErrorCalc error = null;
            _errorScalar = null;
            int n = par.Length;
            var pos = new int[n]; //Текущий обрабатываемый индекс для каждого списка 
            var islist = new bool[n]; //Значение является списком
            var spar = new Moment[n]; //Параметры для вызова скалярного делегата
            var list = new List<Moment>[n]; //Параметры, обрабатываемые как списки (а не как константы)
            bool hasList = false; //Среди параметров есть хоть один список
            for (int i = 0; i < n; ++i)
            {
                pos[i] = -1;
                if ((par[i].Moments == null || par[i].Moments.Count == 0) && par[i].Moment == null && !(comp != null && (comp.Method.Name == "Compunion" || comp.Method.Name == "Compendifp"))) //Пустой список
                    return new SingleValue(new List<Moment>());
                if (comp == null)
                {
                    hasList |= islist[i] = (par[i].Type == SingleType.List);
                    if (!islist[i]) spar[i] = par[i].Moment;
                    else list[i] = par[i].Moments;
                }
                else
                {
                    hasList = islist[i] = true;
                    if (par[i].Moments != null)
                        list[i] = par[i].Moments;
                    else
                    {
                        list[i] = new List<Moment>();
                        if (par[i].Moment != null) list[i].Add(par[i].Moment);
                    }
                }
                //hasList |= par[i].Type == SingleType.List;
                //if (comp != null && par[i].Moment != null)
                //    list[i] = new List<Moment> {par[i].Moment};
                //else list[i] = par[i].Moments;
                //pos[i] = -1;
                //if (par[i].Type == SingleType.Moment && comp == null)                
                //    spar[i] = par[i].Moment;                               
            }
            if (!hasList) //Одно значение
            {
                DateTime t = Different.MaxDate;
                foreach (var p in par)
                    if (p.Moment.Time != Different.MinDate && p.Moment.Time < t)
                        t = p.Moment.Time;
                if (t == Different.MaxDate) t = Begin;
                var mv = new Moment(dt, t, MaxErr(spar), MaxNd(spar));
                scalar(spar, mv);
                return new SingleValue(mv){Error = mv.Error};
            }

            //Список значений
            var rlist = new List<Moment>();//Результаты
            var d = Different.MaxDate;
            bool e = true;
            var parnum = new HashSet<int>(); //Номера параметров с точками в рассматриваемое время
            while (e)
            {
                e = false;
                DateTime ctime = d;
                for (int i = 0; i < n; ++i)
                {
                    if (islist[i] && pos[i] + 1 < list[i].Count)
                    {
                        e = true;
                        if (ctime > list[i][pos[i] + 1].Time)
                            ctime = list[i][pos[i] + 1].Time;
                    }
                }
                if (e)
                {
                    parnum.Clear();
                    for (int i = n-1; i >= 0; --i)
                        if (islist[i])
                        {
                            if (pos[i] + 1 < list[i].Count && ctime == list[i][pos[i] + 1].Time)
                            {
                                pos[i]++;
                                parnum.Add(i);
                            }
                            spar[i] = Interpolation(_interpol, list[i], pos[i], ctime);    
                        }
                    var m = new Moment(dt, ctime, MaxErr(spar), MaxNd(spar));
                    if (comp == null)
                    {
                        scalar(spar, m);
                        if (scalar.Method.Name != "endif")
                            error |= MaxErr(spar);
                        error |= _errorScalar;
                        rlist.Add(m);
                    }
                    else if (comp(spar, m, parnum)) rlist.Add(m);
                }
            }
            return new SingleValue(rlist) {Error = error};
        }
        
        //Вычисляет сумму недостоверности параметров
        private int MaxNd(IEnumerable<Moment> par)
        {
            int nd = 0;
            foreach (var m in par)
                if (m != null) nd |= m.Nd;
            return nd;
        }
        //Вычисляет общую недостоверность параметров
        private int MinNd(IEnumerable<Moment> par)
        {
            int nd = 0;
            foreach (var m in par)
                if (m != null) nd &= m.Nd;
            return nd;
        }
        //Вычисляет суммарную ошибку параметров
        private ErrorCalc MaxErr(IEnumerable<Moment> par)
        {
            ErrorCalc err = null;
            foreach (var m in par)
                if (err == null && m != null && m.Error != null) err = m.Error;
            return err;
        }
        //Вычисляет минимальную ошибку параметров
        private ErrorCalc MinErr(Moment[] par)
        {
            ErrorCalc err = par[0].Error;
            foreach (var m in par)
                if (err != null && m != null && m.Error == null) err = null;
            return err;
        }

        //Записывает ошибку text в res, если она не совпадает с предыдущей
        public void PutErr(string text, Moment res)
        {
            if (res.Error != null) _errorScalar = res.Error;
            else
            {
                if (_errorScalar != null && _errorScalar.Parent == null && text == _errorScalar.Text)
                    res.Error = _errorScalar;
                else
                {
                    res.Error = new ErrorCalc(text, _calc.FullCode);
                    _errorScalar = res.Error;
                }
            }
        }

        //Записывает ошибку text в res
        private void PutSingleErr(string text, SingleValue res)
        {
            res.Error = res.Error ?? new ErrorCalc(text, _calc.Code);
        }
    }
}