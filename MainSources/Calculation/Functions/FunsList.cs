using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public partial class Funs
    {
        public void endifp(SingleValue[] par, SingleValue res)
        {
            var dt = par[1].DataType;
            if (par.Length > 2) dt = dt.Add(par[3].DataType);
            var sv = ScalarFunction(par, dt, null, Compendifp);
            if (sv.Type == SingleType.Moment) res.Moment = sv.Moment;
            else res.Moments = sv.Moments;
        }

        public void assignp(SingleValue[] par, SingleValue res)
        {
            var dt = par[0].DataType.Add(par[1].DataType);
            var sv = ScalarFunction(par, dt, null, Compassignp);
            if (sv.Type == SingleType.Moment) res.Moment = sv.Moment;
            else res.Moments = sv.Moments;
        }

        public  void beginperiod(SingleValue[] par, SingleValue res)
        {
            res.Moment = new Moment(Begin, Begin);
        }

        public  void endperiod(SingleValue[] par, SingleValue res)
        {
            res.Moment = new Moment(Begin, End);
        }

        public void withoutpoints(SingleValue[] par, SingleValue res)
        {
            res.Moments = new List<Moment>();
        }

        public void pointbdi(SingleValue[] par, SingleValue res)
        {
            if (par[0].HasLastMoment && par[1].HasLastMoment && par[2].HasLastMoment)
                res.Moment = new Moment(par[1].LastMoment.Date, par[0].LastMoment.Boolean, null, par[2].LastMoment.Integer);
            else res.Moments = new List<Moment>();
        }

        public void pointidi(SingleValue[] par, SingleValue res)
        {
            if (par[0].HasLastMoment && par[1].HasLastMoment && par[2].HasLastMoment)
                res.Moment = new Moment(par[1].LastMoment.Date, par[0].LastMoment.Integer, null, par[2].LastMoment.Integer);
            else res.Moments = new List<Moment>();
        }

        public void pointrdi(SingleValue[] par, SingleValue res)
        {
            if (par[0].HasLastMoment && par[1].HasLastMoment && par[2].HasLastMoment)
                res.Moment = new Moment(par[1].LastMoment.Date, par[0].LastMoment.Real, null, par[2].LastMoment.Integer);
            else res.Moments = new List<Moment>();
        }

        public void pointddi(SingleValue[] par, SingleValue res)
        {
            if (par[0].HasLastMoment && par[1].HasLastMoment && par[2].HasLastMoment)
                res.Moment = new Moment(par[1].LastMoment.Date, par[0].LastMoment.Date, null, par[2].LastMoment.Integer);
            else res.Moments = new List<Moment>();
        }

        public void pointsdi(SingleValue[] par, SingleValue res)
        {
            if (par[0].HasLastMoment && par[1].HasLastMoment && par[2].HasLastMoment)
                res.Moment = new Moment(par[1].LastMoment.Date, par[0].LastMoment.String, null, par[2].LastMoment.Integer);
            else res.Moments = new List<Moment>();
        }

        public  void unionu(SingleValue[] par, SingleValue res)
        {
            var dt = par[0].DataType;
            var ppar = new SingleValue[par.Length];
            for (int i = 0; i < par.Length; i++)
            {
                dt = dt.Add(par[i].DataType);
                ppar[i] = par[i].Type == SingleType.List ? par[i] : new SingleValue(new List<Moment> {par[i].Moment});
            }
            var sv = ScalarFunction(ppar, dt, null, Compunion);
            if (sv.Type == SingleType.Moment) res.Moment = sv.Moment;
            else res.Moments = sv.Moments;
        }

        public  void valueatpointsuu(SingleValue[] par, SingleValue res)
        {
            var sv = ScalarFunction(par, par[0].DataType, null, Compvalueatpoints);
            if (sv.Type == SingleType.Moment) res.Moment = sv.Moment;
            else res.Moments = sv.Moments;
        }

        //Определение номера точки в списке, предшествующей указанному времени (двоичный поиск)
        private  int NumberAtTime(List<Moment> list, DateTime t)
        {
            if (list.Count == 0 || t < list[0].Time) return -1;
            if (t >= list[list.Count - 1].Time) return list.Count-1;
            int l = 0, r = list.Count-1;
            while (r - l > 1)
            {
                int c = (l + r) / 2;
                if (list[c].Time <= t) l = c;
                else r = c;
            }
            return l;
        }

        private  Moment ValueAtTime(List<Moment> list, Moment t)
        {
            Moment m = Interpolation(_interpol, list, NumberAtTime(list, t.Date), t.Date);
            if (m == null) return null;
            m.Time = t.Date;
            m.Nd |= t.Nd;
            m.Error |= t.Error;
            return m;
        }

        public  void valueattimeud(SingleValue[] par, SingleValue res)
        {
            List<Moment> list = par[0].Type == SingleType.Moment ? new List<Moment> {par[0].Moment} : par[0].Moments; 
            if (par[1].Type == SingleType.Moment)
                res.Moment = ValueAtTime(list, par[1].Moment);
            else
            {
                res.Moments = new List<Moment>();
                foreach (var m in par[1].Moments)
                    res.AddMoment(ValueAtTime(list, m));
            }
        }

        public  void selectpointsub(SingleValue[] par, SingleValue res)
        {
            var sv = ScalarFunction(par, par[0].DataType, null, Compselectpoints);
            if (sv.Type == SingleType.Moment) res.Moment = sv.Moment;
            else res.Moments = sv.Moments;
        }

        public  void settimeud(SingleValue[] par, SingleValue res)
        {
            var sv = ScalarFunction(par, par[0].DataType, null, Compsettime);
            if (sv.Type == SingleType.Moment) res.Moment = sv.Moment;
            else
            {
                sv.Moments.Sort((x, y) => x.Time < y.Time ? -1 : (x.Time > y.Time ? 1 : 0));
                res.Moments = sv.Moments;
            }
        }

        public void movebytimeur(SingleValue[] par, SingleValue res)
        {
            if (par[0].Type != SingleType.List || par[1].Type != SingleType.Moment)
                PutSingleErr("Недопустимые параметры функции СдвигПоВремени", res);
            else
            {
                double s = par[1].Moment.Real;
                var ps = new SingleValue(new List<Moment>());
                foreach (var m in par[0].Moments)
                    ps.Moments.Add(new Moment(m.Time.AddSeconds(s), 0, m.Error | par[1].Moment.Error, m.Nd | par[1].Moment.Nd));
                valueatpointsuu(new[] { par[0], ps }, res);    
            }
        }

        public  void pointnumberu(SingleValue[] par, SingleValue res)
        {
            if (par[0].Type == SingleType.Moment)
                res.Moment = new Moment(1);
            else
                res.Moments = par[0].Moments.Select((t, i) => new Moment(t.Time, i, t.Error, t.Nd)).ToList();
        }

        public  void movebynumberui(SingleValue[] par, SingleValue res)
        {
            res.Moments = new List<Moment>();
            if (par[0].Type != SingleType.List || par[1].Type != SingleType.Moment)
                PutSingleErr("Недопустимые параметры функции СдвигПоНомеру", res);
            else
            {
                int m = par[1].Moment.Integer;
                for (int i = 0; i < par[0].Moments.Count; i++)
                    if (i + m >= 0 && i + m < par[0].Moments.Count)
                        res.Moments.Add(par[0].Moments[i + m].Clone(par[0].Moments[i].Time));
            }
        }

        public  void uniformpointsurr(SingleValue[] par, SingleValue res)
        {
            if (par[1].Type != SingleType.Moment || par[2].Type != SingleType.Moment)
                PutSingleErr("Длина интервала и сдвиг для функции РавномерныеТочки должна быть постоянными величинами", res);
            else
            {
                double d = par[1].Moment.Real, s = par[2].Moment.Real;
                var sv = new SingleValue(new List<Moment>());
                DateTime t = Begin.AddSeconds(s * d);
                while (t < End)
                {
                    sv.Moments.Add(new Moment(t, 0));
                    t = t.AddSeconds(d);
                }
                valueatpointsuu(new[] { par[0], sv }, res);
            }
        }

        public void timebetweenpointsu(SingleValue[] par, SingleValue res)
        {
            if (par[0].Type == SingleType.Moment)
            {
                res.Moment = par[0].Moment.Clone(DataType.Real);
                res.Moment.Real = 0;
            }
            else
            {
                var moms = par[0].Moments;
                res.Moments = new List<Moment>();
                if (moms != null && moms.Count > 0)
                {
                    var m = moms[0].Clone(DataType.Real);
                    m.Real = Math.Max(0, m.Time.Subtract(Begin).TotalSeconds);
                    for (int i = 1; i < moms.Count; i++)
                    {
                        m = moms[i].Clone(DataType.Real);
                        m.Real = moms[i].Time.Subtract(moms[i - 1].Time).TotalSeconds;
                        res.Moments.Add(m);
                    }
                }
            }
        }

        public  void zonebb(SingleValue[] par, SingleValue res)
        {
            zonebbr(new[] { par[0], par[1], new SingleValue(new Moment(60 * 60 * 24 * 1000)) }, res);
        }

        public  void zonebr(SingleValue[] par, SingleValue res)
        {
            zonebbr(new[] { par[0], new SingleValue(new Moment(false)), par[1] }, res);
        }

        public  void zonebbr(SingleValue[] par, SingleValue res)
        {
            res.Moments = new List<Moment>();
            if (par[2].Type != SingleType.Moment || par[2].Moment.Real <= 0)
                PutSingleErr("Длина интервала для функции Зона должна быть постоянной величиной больше 0", res);
            else
            {
                var ms0 = par[0].Moments;
                var ms1 = par[1].Moments;
                DateTime t = Begin;
                if (ms0 != null && ms0.Count > 0 && t > ms0[0].Time) t = ms0[0].Time;
                res.Moments.Add(new Moment(t, 0));
                if (par[0].Type == SingleType.List && ms0.Count > 0)
                {
                    double d = par[2].Moment.Real;
                    bool v = false;
                    int i0 = 0, i1 = 0;
                    DateTime t2 = Different.MaxDate;
                    while (i0 < ms0.Count - 1 || (ms1 != null && i1 < ms1.Count - 1) || t2 != Different.MaxDate)
                    {
                        DateTime t0 = i0 + 1 >= ms0.Count ? Different.MaxDate : ms0[i0 + 1].Time;
                        DateTime t1 = ms1 == null || i1 + 1 >= ms1.Count ? Different.MaxDate : ms1[i1 + 1].Time;
                        if (t0 <= t1 && t0 <= t2)
                        {
                            var mv = ms0[++i0];
                            if (!ms0[i0 - 1].Boolean && mv.Boolean && !v)//Начало события
                            {
                                res.Moments.Add(new Moment(t0, true, mv.Error, mv.Nd));
                                v = true;
                                t2 = mv.Time.AddSeconds(d);
                            }
                        }
                        else if (t1 <= t2)
                        {
                            var mv = ms1[++i1];
                            if (!ms1[i1 - 1].Boolean && mv.Boolean && v)//Конец события по условию
                            {
                                if (t1 == res.Moments[res.Moments.Count - 1].Time)
                                    t1 = t1.AddMilliseconds(0.6);
                                res.Moments.Add(new Moment(t1, false, mv.Error, mv.Nd));
                                v = false;
                                t2 = Different.MaxDate;
                            }
                        }
                        else //Конец события по времени
                        {
                            if (t2 == res.Moments[res.Moments.Count - 1].Time)
                                t2 = t2.AddMilliseconds(0.6);
                            res.Moments.Add(new Moment(t2, false));
                            v = false;
                            t2 = Different.MaxDate;
                        }
                    }
                }
            }
        }

        public  void zonefromendbb(SingleValue[] par, SingleValue res)
        {
            zonefromendbbr(new[] { par[0], par[1], new SingleValue(new Moment(60 * 60 * 24 * 2000)) }, res);
        }

        public  void zonefromendbr(SingleValue[] par, SingleValue res)
        {
            zonefromendbbr(new[] { new SingleValue(new Moment(false)), par[0], par[1] }, res);
        }

        public void zonefromendbbr(SingleValue[] par, SingleValue res)
        {
            res.Moments = new List<Moment>();
            if (par[2].Type != SingleType.Moment || par[2].Moment.Real <= 0)
                PutSingleErr("Длина интервала для функции ЗонаСКонца должна быть постоянной величиной больше 0", res);
            else
            {
                var ms0 = par[0].Moments;
                var ms1 = par[1].Moments;
                if (par[1].Type == SingleType.List && ms1.Count > 0)
                {
                    double d = par[2].Moment.Real;
                    bool v = false;
                    int i1 = ms1.Count - 1, i0 = ms0 == null || ms0.Count == 0 ? 0 : ms0.Count - 1;
                    DateTime t2 = Different.MinDate;
                    while (i1 > 0 || i0 > 0 || t2 != Different.MinDate)
                    {
                        DateTime t1 = i1 < 1 ? Different.MinDate : ms1[i1 - 1].Time;
                        DateTime t0 = i0 < 1 ? Different.MinDate : ms0[i0 - 1].Time;
                        if (t1 >= t0 && t1 >= t2)
                        {
                            var mv = ms1[i1--];
                            if (mv.Boolean && !ms1[i1].Boolean && !v)//Конец события
                            {
                                res.Moments.Insert(0, new Moment(mv.Time, false, mv.Error, mv.Nd));
                                v = true;
                                t2 = mv.Time.AddSeconds(-d);
                            }
                        }
                        else if (t0 >= t2)
                        {
                            var mv = ms0[i0--];
                            if (mv.Boolean && !ms0[i0].Boolean && v)//Начало события по условию
                            {
                                DateTime tt0 = mv.Time;
                                if (tt0 == res.Moments[0].Time)
                                    tt0 = tt0.AddMilliseconds(-0.6);
                                res.Moments.Insert(0, new Moment(tt0, true, mv.Error, mv.Nd));
                                v = false;
                                t2 = Different.MinDate;
                            }
                        }
                        else //Начало события по времени
                        {
                            if (t2 == res.Moments[0].Time)
                                t2 = t2.AddMilliseconds(-0.6);
                            res.Moments.Insert(0, new Moment(t2, true));
                            v = false;
                            t2 = Different.MinDate;
                        }
                    }
                }
                if (res.Moments.Count == 0)
                    res.Moments.Add(new Moment(Begin, false));
                else if (res.Moments[0].Time > Begin)
                    res.Moments.Insert(0, new Moment(Begin, !res.Moments[0].Boolean));
            }
        }

        private  SingleValue EventFilter(SingleValue sv, bool v)
        {
            var res = new SingleValue(new List<Moment>());
            if (sv.Moments != null)
            {
                for (int i = 1; i < sv.Moments.Count; i++)
                    if (sv.Moments[i - 1].Boolean != v && sv.Moments[i].Boolean == v)
                        res.Moments.Add(sv.Moments[i]);
            }
            return res;
        }

        public  void eventub(SingleValue[] par, SingleValue res)
        {
            valueatpointsuu(new[] { par[0], EventFilter(par[1], true) }, res);
        }

        public  void eventuub(SingleValue[] par, SingleValue res)
        {
            var sv0 = new SingleValue();
            valueatpointsuu(new[] { par[0], EventFilter(par[2], true) }, sv0);
            var sv1 = new SingleValue();
            valueatpointsuu(new[] { par[1], EventFilter(par[2], false) }, sv1);
            unionu(new[] { sv0, sv1 }, res);
        }

        public  void aperturerr(SingleValue[] par, SingleValue res)
        {
            if (par[1].Type != SingleType.Moment || par[1].Moment.Real <= 0)
                PutSingleErr("Значение апертуры должно быть постоянной положительной величиной", res);
            else
            {
                if (par[0].Type == SingleType.Moment)
                    res.Moment = par[0].Moment;
                else
                {
                    double a = par[1].Moment.Real;
                    res.Moments = new List<Moment>();
                    double d = double.MinValue;
                    foreach (var m in par[0].Moments)
                        if (Math.Abs(d - m.Real) >= a)
                        {
                            d = m.Real;
                            res.Moments.Add(m);
                        }
                }
            }
        }

        public void apertureir(SingleValue[] par, SingleValue res)
        {
            aperturerr(par, res);
        }

        public void aperturebytimeur(SingleValue[] par, SingleValue res)
        {
            if (par[1].Type != SingleType.Moment || par[1].Moment.Real <= 0)
                PutSingleErr("Значение апертуры по времени должно быть постоянной положительной величиной", res);
            else if (par[0].Type == SingleType.Moment)
                    res.Moment = par[0].Moment;
            else
            {
                double a = par[1].Moment.Real;
                res.Moments = new List<Moment>();
                var moms = par[0].Moments;
                if (moms == null || moms.Count == 0) return;
                int k = 0, i = 0;
                while (i < moms.Count)
                {
                    while (i < moms.Count && moms[i] == moms[k] && moms[i].Nd == moms[k].Nd)
                        i++;
                    if (i == moms.Count || moms[i].Time.Subtract(moms[k].Time).TotalSeconds >= a)
                        res.Moments.Add(moms[k]);
                    k = i;
                }
            }
        }

        public void thinningur(SingleValue[] par, SingleValue res)
        {
            if (par[1].Type != SingleType.Moment || par[1].Moment.Real <= 0)
                PutSingleErr("Значение интервала прореживанияч должно быть постоянной положительной величиной", res);
            else
            {
                if (par[0].Type == SingleType.Moment)
                    res.Moment = par[0].Moment;
                else
                {
                    double a = par[1].Moment.Real;
                    res.Moments = new List<Moment>();
                    DateTime d = Different.MinDate;
                    foreach (var m in par[0].Moments)
                        if (m.Time.Subtract(d).TotalSeconds >= a)
                        {
                            d = m.Time;
                            res.Moments.Add(m);
                        }
                }
            }
        }

        public void deleterepetitionsub(SingleValue[] par, SingleValue res)
        {
            if (par[0].Type == SingleType.Moment) res.Moment = par[0].Moment;
            else
            {
                if (par[1].Type != SingleType.Moment)
                    PutSingleErr("Недопустимые параметры функции УдалитьПовторы", res);
                else
                {
                    bool b = par[1].Moment.Boolean;
                    var list = par[0].Moments;
                    if (list.Count == 0) res.Moments = new List<Moment>();
                    else
                    {
                        var rlist = new List<Moment> { list[0] };
                        for (int i = 1; i < list.Count; i++)
                            if (list[i] != list[i - 1] || list[i].Nd != list[i - 1].Nd || list[i - 1].Error != list[i].Error)
                                rlist.Add(list[i]);
                        if (b && rlist.Count == 1) res.Moment = rlist[0];
                        else res.Moments = rlist;   
                    }
                }
            }
        }

        public  void maxzoneb(SingleValue[] par, SingleValue res)
        {
            if (par[0].Type == SingleType.Moment || par[0].Moments == null || par[0].Moments.Count == 0)
                res.Moment = par[0].Moment;
            else
            {
                //list:                         0  1 ... n-1  
                //rlist   false Begin  0  1 ... n-1 End false
                //причем, Begin и End не добавляются, если 0 <= Begin или n-1 >= End
                res.Moments = new List<Moment>();
                var list = par[0].Moments;
                var rlist = new List<Moment> { new Moment(false) };
                if (list[0].Time > Begin)
                    rlist.Add(list[0].Clone(Begin));
                rlist.AddRange(list);
                if (list[list.Count - 1].Time < End)
                    rlist.Add(list[list.Count - 1].Clone(End));
                rlist.Add(new Moment(false) { Time = rlist[rlist.Count - 1].Time });
                rlist[0].Time = rlist[1].Time;

                int indbeg = 0, maxbeg = 0, maxen = 0;
                double len = 0, maxlen = 0;
                bool b = false;
                for (int i = 0; i < rlist.Count; i++)
                {
                    if (!b && rlist[i].Boolean)
                    {
                        indbeg = i;
                        len = 0;
                    }
                    if (b) len += rlist[i].Time.Subtract(rlist[i - 1].Time).TotalSeconds;
                    if (!rlist[i].Boolean && b && len > maxlen)
                    {
                        maxlen = len;
                        maxbeg = indbeg;
                        maxen = i;
                    }
                    b = rlist[i].Boolean;
                }
                if (maxlen > 0)
                {
                    if (maxbeg != 1) res.Moments.Add(new Moment(rlist[1].Time, false));
                    res.Moments.Add(rlist[maxbeg]);
                    if (maxbeg != rlist.Count - 2) res.Moments.Add(rlist[maxen]);
                }
            }
        }

        public  void accumulationr(SingleValue[] par, SingleValue res)
        {
            res.Moments = new List<Moment>();
            if (par[0].Type == SingleType.Moment)
            {
                var m = par[0].Moment.Clone(End);
                m.Real *= End.Subtract(Begin).TotalSeconds;
                res.Moments.Add(new Moment(Begin, 0.0));
                res.Moments.Add(m);
            }
            else
            {
                var ip = _interpol;
                var ms = par[0].Moments;
                if (ms.Count != 0)
                {
                    double d = 0;
                    int nd = 0;
                    ErrorCalc err = null;
                    res.Moments.Add(new Moment(ms[0].Time, d));
                    for (int i = 1; i < ms.Count; i++)
                    {
                        d += SimpleIntegral(ip, ms[i - 1], ms[i]);
                        nd |= ms[i - 1].Nd | (ip == InterpolationType.Linear ? ms[i].Nd : 0);
                        err |= ms[i - 1].Error | (ip == InterpolationType.Linear ? ms[i].Error : null);
                        res.Moments.Add(new Moment(ms[i].Time, d, err, nd));
                    }
                    if (ms.Last().Time < Begin)
                    {
                        nd |= ms.Last().Nd;
                        err |= ms.Last().Error;
                        res.Moments.Add(new Moment(End, d + SimpleIntegral(ip, ms.Last(), ms.Last().Clone(End)), err, nd));
                    }
                }
            }
        }

        //Вычисление скорости по заданному набору точек
        private  Moment Speed(LinkedList<Moment> par, double pos)
        {
            //Метод наименьших квадратов
            //v=M((X-MX)(Y-MY))/M((X-MX)(X-MX))
            int n = par.Count;
            double my = 0, mx = 0;
            int nd = 0;
            ErrorCalc err = null;
            LinkedListNode<Moment> cur = par.First;
            while (cur != null)
            {
                my += cur.Value.Real;
                mx += cur.Value.Time.Subtract(par.First.Value.Time).TotalSeconds;
                nd |= cur.Value.Nd;
                err |= cur.Value.Error;
                cur = cur.Next;
            }
            mx = mx / n;
            my = my / n;
            double mmx = 0, mmy = 0;
            cur = par.First;
            while (cur != null)
            {
                double dx = cur.Value.Time.Subtract(par.First.Value.Time).TotalSeconds - mx;
                mmx += dx * dx;
                mmy += dx * (cur.Value.Real - my);
                cur = cur.Next;
            }
            return new Moment(par.First.Value.Time.AddSeconds(par.Last.Value.Time.Subtract(par.First.Value.Time).TotalSeconds * pos),
                                                mmx == 0 ? 0 : mmy / mmx, err, nd);
        }

        //Вычисление медианы (4/5) из расстояний по времени между значениями, умноженная на 4
        private double FindMedianForSpeed(List<Moment> moments)
        {
            var mas = new double[moments.Count - 1];
            for (int i = 1; i < moments.Count; i++)
                mas[i-1] = moments[i].Time.Subtract(moments[i - 1].Time).TotalSeconds;
            Array.Sort(mas);
            return 4 * Math.Max(0.1, mas[4*mas.Length/5]);
        }

        //Вычисляет скорость и добавляет в результат
        private void AddSpeedPoint(LinkedList<Moment> lin, List<Moment> res, double pos)
        {
            Moment s = Speed(lin, pos);
            if (res.Count == 0 || res[res.Count - 1].Real != s.Real || res[res.Count - 1].Nd != s.Nd || res[res.Count - 1].Error != s.Error)
                res.Add(s);
        }

        //Добавляет точку в текущий список, а также вычисляет скорость и добавляет в результат если нужно
        private void AddSpeed(Moment mom, LinkedList<Moment> lin, List<Moment> res, int num, double pos)
        {
            if (lin.Count == num) lin.RemoveFirst();
            lin.AddLast(mom);
            if (lin.Count == num)
                AddSpeedPoint(lin, res, pos);
        }
        
        public void speedrir(SingleValue[] par, SingleValue res)
        {
            if (par[1].Type != SingleType.Moment || par[1].Moment.Integer < 2 || par[2].Type != SingleType.Moment || par[2].Moment.Real < 0 || par[2].Moment.Real > 1)
                PutSingleErr("Недопустимые параметры функции Скорость", res);
            else
            {
                var moments = par[0].Moments;
                if (par[0].Type == SingleType.Moment || moments == null || moments.Count < 2)
                    res.Moment = new Moment(Begin, 0);
                else
                {
                    res.Moments = new List<Moment>();
                    int num = Math.Min(par[1].Moment.Integer, moments.Count);
                    double pos = par[2].Moment.Real;
                    double med = FindMedianForSpeed(moments);
                    var lin = new LinkedList<Moment>();
                    DateTime t = Begin;
                    while (moments[0].Time.Subtract(t).TotalSeconds >= med)
                    {
                        AddSpeed(moments[0].Clone(t), lin, res.Moments, num, pos);
                        t = t.AddSeconds(med);
                    }
                    for (int i = 0; i < moments.Count - 1; i++)
                    {
                        AddSpeed(moments[i], lin, res.Moments, num, pos);
                        t = moments[i].Time.AddSeconds(med);
                        while (moments[i+1].Time.Subtract(t).TotalSeconds >= med)
                        {
                            AddSpeed(Interpolation(InterpolationType.Linear, moments, i, t), lin, res.Moments, num, pos);
                            t = t.AddSeconds(med);
                        }
                    }
                    var last = moments[moments.Count - 1];
                    AddSpeed(last, lin, res.Moments, num, pos);
                    t = last.Time.AddSeconds(med);
                    while (End.Subtract(t).TotalSeconds > 0)
                    {
                        AddSpeed(last.Clone(t), lin, res.Moments, num, pos);
                        t = t.AddSeconds(med);
                    }
                    if (lin.Count < num)
                        AddSpeedPoint(lin, res.Moments, pos);
                }
            }
        }

        //Статистические
        private  double SimpleIntegral(InterpolationType type, Moment m1, Moment m2)
        {
            switch (type)
            {
                case InterpolationType.Constant:
                    return m1.Real * (m2.Time.Subtract(m1.Time).TotalSeconds);

                case InterpolationType.Linear:
                    return (m1.Real + m2.Real) / 2 * (m2.Time.Subtract(m1.Time).TotalSeconds);
            }
            return 0;
        }

        private  void AddP(List<Moment> rlist, Moment mv, Moment res)
        {
            rlist.Add(mv);
            res.Nd |= mv.Nd;
            res.Error |= mv.Error;
        }

        //На входе исходное значение или список sv, интервал времени tbeg-ten
        //тип интерполяции - itype, нужно ли добавлять точки в начало и конец - addend
        //Формирует список значений за интервал (выходное значение) и момент с посчитаной недостоверностью res 
        private  List<Moment> AgregatePoint(Moment res, SingleValue sv, DateTime tbeg, DateTime ten, bool addends, InterpolationType itype = InterpolationType.Constant)
        {
            var rlist = new List<Moment>();
            if (sv.Type == SingleType.Moment)
            {
                if (sv.Moment.Time >= tbeg && sv.Moment.Time <= ten)
                    AddP(rlist, sv.Moment.Clone(), res);
                if (addends)
                {
                    AddP(rlist, sv.Moment.Clone(tbeg), res);
                    AddP(rlist, sv.Moment.Clone(ten), res);
                }
                return rlist;
            }

            var list = sv.Moments;
            if (list.Count == 0) return list;
            int beg = NumberAtTime(list, tbeg);
            int en = NumberAtTime(list, ten);

            if (addends)
                AddP(rlist, Interpolation(itype, list, beg, tbeg), res);
            else if (beg >= 0 && tbeg == list[beg].Time)
                AddP(rlist, list[beg], res);

            for (int i = beg+1; i <= en; i++)
                if (list[i].Time < ten)
                    AddP(rlist, list[i], res);
            
            if (addends)
                AddP(rlist, Interpolation(itype, list, en, ten), res);
            return rlist;
        }

        //Вычисление указанной аггрегирующей функции по сегментам par[0] - выражение, par[1] - сегменты
        private  void AggrSegments(SingleValue[] par, SingleValue res, AgregateDelegate aggr)
        {
            var ss = par[1].Segments;
            res.Moments = new List<Moment>();
            foreach (var seg in ss)
            {
                var m = aggr(par[0], seg.Begin, seg.End);
                if (seg.IsResultTime) m.Time = seg.ResultTime;
                m.Nd |= seg.Nd;
                m.Error |= seg.Error;
                res.AddMoment(m);
            }
        }

        public  void valuebeginu(SingleValue[] par, SingleValue res)
        {
            valueattimeud(new[] { par[0], new SingleValue(new Moment(Begin)) }, res);
        }

        public  void valuebeginug(SingleValue[] par, SingleValue res)
        {
            var rres = new SingleValue();
            var segp = new SingleValue(new Moment(DataType.Time));
            var ppar = new[] { par[0], segp };
            foreach (var seg in par[1].Segments)
            {
                segp.Moment.Date = seg.Begin;
                valueattimeud(ppar, rres);
                rres.Moment.Time = seg.IsResultTime ? seg.ResultTime : seg.Begin;
                res.AddMoment(rres.Moment);
            }
        }

        public  void valueendu(SingleValue[] par, SingleValue res)
        {
            valueattimeud(new[] { par[0], new SingleValue(new Moment(End)) }, res);
        }

        public  void valueendug(SingleValue[] par, SingleValue res)
        {
            var rres = new SingleValue();
            var segp = new SingleValue(new Moment(DataType.Time));
            var ppar = new[] { par[0], segp };
            foreach (var seg in par[1].Segments)
            {
                segp.Moment.Date = seg.End;
                valueattimeud(ppar, rres);
                rres.Moment.Time = seg.IsResultTime ? seg.ResultTime : seg.End;
                res.AddMoment(rres.Moment);
            }
        }

        private  Moment TimeM(SingleValue sv, DateTime beg, DateTime en)
        {
            var m = new Moment(DataType.Real, beg);
            var list = AgregatePoint(m, sv, beg, en, true);
            m.Real = 0;
            for (int i = 0; i < list.Count - 1; i++)
                m.Real += SimpleIntegral(InterpolationType.Constant, list[i], list[i + 1]);
            return m;
        }

        public  void timeb(SingleValue[] par, SingleValue res)
        {
            res.Moment = TimeM(par[0], Begin, End);
        }

        public  void timebg(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par,res, TimeM);
        }

        private  Moment IntegralM(SingleValue sv, DateTime beg, DateTime en)
        {
            var m = new Moment(DataType.Real, beg);
            var list = AgregatePoint(m, sv, beg, en, true, _interpol);
            m.Real = 0;
            for (int i = 0; i < list.Count - 1; i++)
                m.Real += SimpleIntegral(_interpol, list[i], list[i + 1]);
            return m;
        }

        public  void integralr(SingleValue[] par, SingleValue res)
        {
            res.Moment = IntegralM(par[0], Begin, End);
        }

        public  void integralrg(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par, res, IntegralM);
        }

        public  void averager(SingleValue[] par, SingleValue res)
        {
            integralr(par, res);
            double d = End.Subtract(Begin).TotalSeconds;
            if (d != 0) res.Moment.Real /= d;
            else res.Moment.Real = par[0].LastMoment.Real;
        }

        public  void averagerg(SingleValue[] par, SingleValue res)
        {
            var tim = new SingleValue();
            integralrg(par, res);
            timebg(new [] {new SingleValue(new Moment(true)), par[1] }, tim);
            for (int i = 0; i < res.Moments.Count; i++)
                if (tim.Moments[i].Real != 0) 
                    res.Moments[i].Real /= tim.Moments[i].Real;
        }

        public  void averageinzonerbr(SingleValue[] par, SingleValue res)
        {
            if (par[2].Type != SingleType.Moment)
                PutSingleErr("Значение по умолчению в функции СреднееВЗоне должно быть постоянным значением", res);
            else
            {
                integralr(new[] { ScalarFunction(new[] { par[0], par[1] }, DataType.Real, multiplyrr) }, res);
                var tim = new SingleValue();
                timeb(new[] { par[1] }, tim);
                if (tim.Moment.Real == 0) res.Moment = par[2].Moment;
                else res.Moment.Real /= tim.Moment.Real;
            }
        }

        public  void averageinzonergbr(SingleValue[] par, SingleValue res)
        {
            if (par[3].Type != SingleType.Moment)
                PutSingleErr("Значение по умолчению в функции СреднееВЗоне должно быть постоянным значением", res);
            else
            {
                var sv = ScalarFunction(new[] {par[0], par[2]}, DataType.Real, multiplyrr);
                integralrg(new[] { sv, par[1] }, res);
                var tim = new SingleValue();
                timebg(new[] { par[2], par[1] }, tim);
                for (int i = 0; i < res.Moments.Count; i++)
                {
                    if (tim.Moments[i].Real == 0) 
                        res.Moments[i] = par[3].Moment.Clone(res.Moments[i].Time);
                    else res.Moments[i].Real /= tim.Moments[i].Real;
                }
            }
        }

        public  void dispersionr(SingleValue[] par, SingleValue res)
        {
            var ress = new SingleValue();
            averager(par, ress);
            var d = ress.Moment.Real;
            var h = new SingleValue(par[0].Moments.Select(m => new Moment(DataType.Real, (m.Real - d) * (m.Real - d), m.Time, m.Nd, m.Error)).ToList());
            averager(new[] { h }, res);
        }

        public  void dispersionrg(SingleValue[] par, SingleValue res)
        {
            var sq = new SingleValue();
            averagerg(new[] {ScalarFunction(new[] {par[0], par[0]}, DataType.Real, multiplyrr), par[1]} , sq);
            var av = new SingleValue();
            averagerg(par, av);
            av = ScalarFunction(new[] {av, av}, DataType.Real, multiplyrr);
            res.Moments = ScalarFunction(new[] {sq, av}, DataType.Real, minusrr).Moments;
        }

        private  Moment SummiM(SingleValue sv, DateTime beg, DateTime en)
        {
            var m = new Moment(DataType.Integer, beg);
            var list = AgregatePoint(m, sv, beg, en, false);
            m.Integer = 0;
            foreach (var mv in list)
                m.Integer += mv.Integer;
            return m;
        }

        private  Moment SummrM(SingleValue sv, DateTime beg, DateTime en)
        {
            var m = new Moment(DataType.Real, beg);
            var list = AgregatePoint(m, sv, beg, en, false);
            m.Real = 0;
            foreach (var mv in list)
                m.Real += mv.Real;
            return m;
        }

        public  void summi(SingleValue[] par, SingleValue res)
        {
            //res.Moment = SummiM(par[0], Begin, End);
            int n = 0;
            if (par[0].Type == SingleType.Moment) n = par[0].Moment.Integer;
            else if (par[0].Moments != null)
                n = par[0].Moments.Sum(mom => mom.Integer);
            res.Moment = new Moment(Begin, n);
        }

        public  void summr(SingleValue[] par, SingleValue res)
        {
            //res.Moment = SummrM(par[0], Begin, End);
            double r = 0;
            if (par[0].Type == SingleType.Moment) r = par[0].Moment.Real;
            else if (par[0].Moments != null)
                r = par[0].Moments.Sum(mom => mom.Real);
            res.Moment = new Moment(Begin, r);
        }

        public  void summig(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par, res, SummiM);
        }

        public  void summrg(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par, res, SummrM);
        }

        private  Moment CountPointsM(SingleValue sv, DateTime beg, DateTime en)
        {
            var m = new Moment(DataType.Integer, beg);
            var list = AgregatePoint(m, sv, beg, en, false);
            m.Integer = list.Count;
            return m;
        }

        public  void countpointsu(SingleValue[] par, SingleValue res)
        {
            //res.Moment = CountPointsM(par[0], Begin, End);
            int n = 0;
            if (par[0].Type == SingleType.Moment) n = 1;
            else if (par[0].Moments != null) n = par[0].Moments.Count;
            res.Moment = new Moment(Begin, n);
        }

        public  void countpointsug(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par, res, CountPointsM);
        }

        private  Moment CountM(SingleValue sv, DateTime beg, DateTime en)
        {
            var m = new Moment(DataType.Integer, beg);
            var list = AgregatePoint(m, sv, beg, en, true);
            m.Integer = 0;
            for (int i = 1; i < list.Count; i++)
                if (!list[i-1].Boolean && list[i].Boolean)
                    m.Integer++;
            return m;
        }

        public  void countb(SingleValue[] par, SingleValue res)
        {
            res.Moment = CountM(par[0], Begin, End);
        }

        public  void countbg(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par, res, CountM);
        }

        private Moment MinimumME(SingleValue sv, DateTime beg, DateTime en, bool addends)
        {
            var m = new Moment(sv.DataType);
            var list = AgregatePoint(m, sv, beg, en, addends, _interpol);
            if (list.Count == 0) return m;
            var ress = list[0];
            for (int i = 1; i < list.Count; i++)
                if (list[i] < ress) ress = list[i];
            m.CopyValueFrom(ress);
            m.Time = ress.Time;
            return m;
        }
        private  Moment MinimumM(SingleValue sv, DateTime beg, DateTime en)
        {
            return MinimumME(sv, beg, en, true);
        }
        private Moment MinimumMG(SingleValue sv, DateTime beg, DateTime en)
        {
            return MinimumME(sv, beg, en, true);
        }
        
        public  void minimumu(SingleValue[] par, SingleValue res)
        {
            res.Moment = MinimumM(par[0], Begin, End);
        }

        public  void minimumug(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par, res, MinimumMG);
        }

        private Moment MaximumME(SingleValue sv, DateTime beg, DateTime en, bool addends)
        {
            var m = new Moment(sv.DataType);
            var list = AgregatePoint(m, sv, beg, en, addends, _interpol);
            if (list.Count == 0) return m;
            var ress = list[0];
            for (int i = 1; i < list.Count; i++)
                if (list[i] > ress) ress = list[i];
            m.CopyValueFrom(ress);
            m.Time = ress.Time;
            return m;
        }
        private Moment MaximumM(SingleValue sv, DateTime beg, DateTime en)
        {
            return MaximumME(sv, beg, en, true);
        }
        private Moment MaximumMG(SingleValue sv, DateTime beg, DateTime en)
        {
            return MaximumME(sv, beg, en, true);
        }

        public  void maximumu(SingleValue[] par, SingleValue res)
        {
            res.Moment = MaximumM(par[0], Begin, End);
        }

        public  void maximumug(SingleValue[] par, SingleValue res)
        {
            AggrSegments(par, res, MaximumMG);
        }

        public  void valueatnumberiu(SingleValue[] par, SingleValue res)
        {
            var p = par[0].Moment;
            if (par[0].Type != SingleType.Moment || par[1].Type != SingleType.List || p.Integer <= 0 || p.Integer > par[1].Moments.Count)
                PutSingleErr("Недопустимые параметры функции ЗначениеПоНомеру", res);
            else
            {
                var m = par[1].Moments[p.Integer - 1].Clone();
                m.Nd |= p.Nd;
                m.Error |= p.Error;
                res.Moment = m;
            }
        }

        public  void valueatnumberiug(SingleValue[] par, SingleValue res)
        {
            var p = par[0].Moment;
            if (par[0].Type != SingleType.Moment || p.Integer <= 0)
                PutSingleErr("Номер в функции ЗначениеПоНомеру должен быть постоянным положительным значением", res);
            else
            {
                var list = par[1].Moments;
                foreach (var seg in par[2].Segments)
                {
                    var l = NumberAtTime(list, seg.Begin);
                    var r = NumberAtTime(list, seg.End);
                    if (l == -1 || seg.Begin != list[l].Time) l++;
                    if (r >= 0 && seg.End == list[r].Time) r--;
                    Moment m = r-l+1 >= p.Integer ? list[l + p.Integer-1].Clone() :
                        new Moment(par[1].DataType, seg.Begin, new ErrorCalc("Вычисление функции ЗначениеПоНомеру от недопустимого номера", _calc.Code));
                    m.Error |= p.Error;
                    m.Nd |= p.Nd;
                    if (seg.IsResultTime) m.Time = seg.ResultTime;
                    res.AddMoment(m);
                }
            }
        }

        public  void valueatnumberendiu(SingleValue[] par, SingleValue res)
        {
            var p = par[0].Moment;
            if (par[0].Type != SingleType.Moment || par[1].Type != SingleType.List || p.Integer <= 0 || p.Integer > par[1].Moments.Count)
                PutSingleErr("Недопустимые параметры функции ЗначениеПоНомеруСКонца", res);
            else
            {
                var m = par[1].Moments[par[1].Moments.Count-p.Integer].Clone();
                m.Nd |= p.Nd;
                m.Error |= p.Error;
                res.Moment = m;
            }
        }

        public  void valueatnumberendiug(SingleValue[] par, SingleValue res)
        {
            var p = par[0].Moment;
            if (par[0].Type != SingleType.Moment || p.Integer <= 0)
                PutSingleErr("Номер в функции ЗначениеПоНомеруСКонца должен быть постоянным положительным значением", res);
            else
            {
                var list = par[1].Moments;
                foreach (var seg in par[2].Segments)
                {
                    var l = NumberAtTime(list, seg.Begin);
                    var r = NumberAtTime(list, seg.End);
                    if (l == -1 || seg.Begin != list[l].Time) l++;
                    if (r >= 0 && seg.End == list[r].Time) r--;
                    Moment m = r - l + 1 >= p.Integer ? list[r - p.Integer + 1].Clone() :
                        new Moment(par[1].DataType, seg.Begin, new ErrorCalc("Вычисление функции ЗначениеПоНомеру от недопустимого номера", _calc.Code));
                    m.Error |= p.Error;
                    m.Nd |= p.Nd;
                    if (seg.IsResultTime) m.Time = seg.ResultTime;
                    res.AddMoment(m);
                }
            }
        }

        //Сегменты
        private const string _segerr = "Положение результата в сегменте должно быть постоянным значением от 0 до 1";

        public  void segmentsbr(SingleValue[] par, SingleValue res)
        {
            res.Segments = new List<Segment>();
            var list = par[0].Moments;
            if (par[0].Type == SingleType.List && list.Count == 0) return;
            if (par[1].Type != SingleType.Moment)
                PutSingleErr(_segerr, res);
            else
            {
                var d = par[1].Moment.Real;
                if ((d != -1 && d < 0) || d > 1)
                    PutSingleErr(_segerr, res);
                else
                {
                    if (par[0].Type == SingleType.Moment)
                        res.Segments.Add(new Segment(Begin, End, d));
                    else
                    {
                        var b = new Moment(false);
                        var sbegin = Different.MinDate;
                        int i = 0;
                        while (i < list.Count)
                        {
                            if (!b.Boolean && list[i].Boolean) 
                                sbegin = (i==0 && list[i].Time > Begin) ? Begin : list[i].Time;
                            if (b.Boolean && !list[i].Boolean)
                                res.Segments.Add(new Segment(sbegin, list[i].Time, d, b.Nd | list[i].Nd, b.Error | list[i].Error));
                            b = list[i];
                            i++;
                        }
                        if (b.Boolean && sbegin < End) res.Segments.Add(new Segment(sbegin, End, d, b.Nd, b.Error));   
                    }
                }
            }
        }

        public  void uniformsegmentsrr(SingleValue[] par, SingleValue res)
        {
            res.Segments = new List<Segment>();
            if (par[1].Type != SingleType.Moment || par[0].Type != SingleType.Moment)
                PutSingleErr("Недопустимые параметры функции РавномерныеСегменты", res);
            else
            {
                var mv1 = par[1].Moment;
                var mv0 = par[0].Moment;
                var d = mv1.Real;
                if ((d != -1 && d < 0) || d > 1)
                    PutSingleErr(_segerr, res);
                else
                {
                    DateTime t = Begin;
                    while (t < End)
                    {
                        DateTime te = t.AddSeconds(mv0.Real);
                        res.Segments.Add(new Segment(t, te, d, mv0.Nd | mv1.Nd, mv0.Error | mv1.Error));
                        t = te;
                    }
                }
            }
        }

        public  void segmentsfrompointsurr(SingleValue[] par, SingleValue res)
        {
            res.Segments = new List<Segment>();
            var list = par[0].Type == SingleType.List ? par[0].Moments : new List<Moment> { par[0].Moment.Clone(Begin) };
            var rlist = par[1].Moments;
            bool e = par[1].Type == SingleType.List;
            if (e && (rlist == null || rlist.Count == 0))
            {
                PutSingleErr("Недопустимая длина сегмента в функции СегментыОтТочек", res);
                return;
            }
            var r = new Moment(0.0);
            if (!e) r = par[1].Moment;
            if (list == null || list.Count == 0) return;
            if (par[2].Type != SingleType.Moment) PutSingleErr(_segerr, res);
            else
            {
                var md = par[2].Moment;
                var d = md.Real;
                if ((d != -1 && d < 0) || d > 1) PutSingleErr(_segerr, res);
                else
                {
                    int i = 0;
                    foreach (var u in list)
                    {
                        while (e && i < rlist.Count - 1 && rlist[i + 1].Time < u.Time) i++;
                        if (e) r = rlist[i];
                        res.Segments.Add(r.Real < 0
                            ? new Segment(u.Time.AddSeconds(r.Real), u.Time, d, md.Nd | r.Nd | u.Nd, md.Error | r.Error | u.Error)
                            : new Segment(u.Time, u.Time.AddSeconds(r.Real), d, md.Nd | r.Nd | u.Nd, md.Error | r.Error | u.Error));
                    }
                    SortSegments(res.Segments);
                }
            }
        }

        public  void countsegmentsg(SingleValue[] par, SingleValue res)
        {
            res.Moment = new Moment(par[0].Segments.Count);
        }

        public  void segmentbynumbergi(SingleValue[] par, SingleValue res)
        {
            res.Segments = new List<Segment>();
            if (par[1].Type != SingleType.Moment) PutSingleErr("Несуществующий номер сегмента", res);
            else
            {
                var i = par[1].Moment.Integer-1;
                if (i >= par[0].Segments.Count || i < 0) PutSingleErr("Несуществующий номер сегмента", res);
                else res.Segments.Add(par[0].Segments[i]);
            }
        }

        public  void unionsegmentsg(SingleValue[] par, SingleValue res)
        {
            res.Segments = par.SelectMany(cv => cv.Segments).ToList();
            SortSegments(res.Segments);
        }

        private  void SortSegments(List<Segment> seg)
        {
            seg.Sort(Segment.Compare);
            for (int i = seg.Count - 1; i > 0; i--)
                if (seg[i].EqualsTo(seg[i - 1]))
                    seg.RemoveAt(i);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //Массивы
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public  CalcValue sizeu(CalcValue[] par, CalcParamRun calc)
        {
            int n = 1;
            if (par[0].Type == CalcValueType.IntArray) n = par[0].IntArray.Count;
            if (par[0].Type == CalcValueType.StringArray) n = par[0].StringArray.Count;
            return new CalcValue(new SingleValue(new Moment(n)) { Error = par[0].Error });
        }

        public  CalcValue arrayu(CalcValue[] par, CalcParamRun calc)
        {
            var ar = new SortedDictionary<int, VarRun>();
            ErrorCalc error = null;
            for (int i = 0; i < par.Length; i++)
            {
                ar.Add(i + 1, new VarRun(par[i]));
                error |= par[i].Error;
            }
            return new CalcValue(ar) { Error = error };
        }

        public  CalcValue arraybynumbersiu(CalcValue[] par, CalcParamRun calc)
        {
            var ar = new SortedDictionary<int, VarRun>();
            ErrorCalc error = null;
            for (int i = 0; i < par.Length - 1; i += 2)
            {
                error |= par[i].Error | par[i + 1].Error;
                if (par[i].SingleValue.Type != SingleType.Moment)
                    error |= new ErrorCalc("Индекс массива должен быть постоянным значением (функция МассивПоНомерам)", calc.CalcParam.Code);
                else ar.Add(par[i].SingleValue.Moment.Integer, new VarRun(par[i+1]));
            }
            return new CalcValue(ar) { Error = error };
        }

        public  CalcValue arraybystringssu(CalcValue[] par, CalcParamRun calc)
        {
            var ar = new SortedDictionary<string, VarRun>();
            ErrorCalc error = null;
            for (int i = 0; i < par.Length - 1; i += 2)
            {
                error |= par[i].Error | par[i + 1].Error;
                if (par[i].SingleValue.Type != SingleType.Moment)
                    error |= new ErrorCalc("Индекс массива должен быть постоянным значением (функция МассивПоСтрокам)", calc.CalcParam.Code);
                else ar.Add(par[i].SingleValue.Moment.String, new VarRun(par[i+1]));
            }
            return new CalcValue(ar) { Error = error };
        }

        public  CalcValue containsindexui(CalcValue[] par, CalcParamRun calc)
        {
            ErrorCalc error = par[0].Error | par[1].Error;
            bool b = false;
            if (par[1].SingleValue.Type != SingleType.Moment)
                error |= new ErrorCalc("Индекс массива должен быть постоянным значением (функция СодержитИндекс)", calc.CalcParam.Code);
            else b = par[0].IntArray.ContainsKey(par[1].SingleValue.Moment.Integer);
            return new CalcValue(new SingleValue(new Moment(b))) { Error = error };
        }

        public  CalcValue containsindexus(CalcValue[] par, CalcParamRun calc)
        {
            ErrorCalc error = par[0].Error | par[1].Error;
            bool b = false;
            if (par[1].SingleValue.Type != SingleType.Moment)
                error |= new ErrorCalc("Индекс массива должен быть постоянным значением (функция СодержитИндекс)", calc.CalcParam.Code);
            else b = par[0].StringArray.ContainsKey(par[1].SingleValue.Moment.String);
            return new CalcValue(new SingleValue(new Moment(b))) { Error = error };
        }

        public  CalcValue deleteelementui(CalcValue[] par, CalcParamRun calc)
        {
            ErrorCalc error = par[0].Error | par[1].Error;
            if (par[1].SingleValue.Type != SingleType.Moment)
                error |= new ErrorCalc("Индекс массива должен быть постоянным значением (функция УдалитьЭлемент)", calc.CalcParam.Code);
            else if (par[0].IntArray.ContainsKey(par[1].SingleValue.Moment.Integer))
                par[0].IntArray.Remove(par[1].SingleValue.Moment.Integer);
            return new CalcValue(par[0].IntArray) { Error = error };
        }

        public  CalcValue deleteelementus(CalcValue[] par, CalcParamRun calc)
        {
            ErrorCalc error = par[0].Error | par[1].Error;
            if (par[1].SingleValue.Type != SingleType.Moment)
                error |= new ErrorCalc("Индекс массива должен быть постоянным значением (функция УдалитьЭлемент)", calc.CalcParam.Code);
            else if (par[0].StringArray.ContainsKey(par[1].SingleValue.Moment.String))
                par[0].StringArray.Remove(par[1].SingleValue.Moment.String);
            return new CalcValue(par[0].StringArray) { Error = error };
        }

        public  CalcValue strsplitss(CalcValue[] par, CalcParamRun calc)
        {
            var res = new SortedDictionary<int, VarRun>();
            ErrorCalc error = par[0].Error | par[1].Error;
            if (par[0].SingleValue.Type != SingleType.Moment || par[1].SingleValue.Type != SingleType.Moment)
                error |= new ErrorCalc("Параметры функции StrSplit должны быть постоянными значениями", calc.CalcParam.Code);
            else
            {
                var m0 = par[0].SingleValue.Moment;
                var m1 = par[1].SingleValue.Moment;
                var ss = m0.String.Split(new[] { m1.String }, StringSplitOptions.RemoveEmptyEntries);
                int i = 1;
                foreach (var s in ss)
                {
                    res.Add(i, new VarRun(new CalcValue(new SingleValue(new Moment(s, m0.Error | m1.Error, m0.Nd | m1.Nd)))) { DataType = DataType.String });
                    i++;
                }
            }
            return new CalcValue(res) { Error = error };
        }
    }
}