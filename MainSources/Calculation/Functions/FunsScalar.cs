using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public partial class Funs
    {
        public  void plusii(Moment[] par, Moment res)
        {
            res.Integer = par[0].Integer + par[1].Integer;
        }

        public  void plusdr(Moment[] par, Moment res)
        {
            try { res.Date = par[0].Date.AddSeconds(par[1].Real);}
            catch { res.Date = par[0].Date;}
        }

        public  void plusrd(Moment[] par, Moment res)
        {
            try { res.Date = par[1].Date.AddSeconds(par[0].Real);}
            catch { res.Date = par[0].Date;}
        }

        public  void plusrr(Moment[] par, Moment res)
        {
            res.Real = par[0].Real + par[1].Real;
        }

        public  void plusss(Moment[] par, Moment res)
        {
            res.String = par[0].String + par[1].String;
        }

        public  void minusii(Moment[] par, Moment res)
        {
            res.Integer = par[0].Integer - par[1].Integer;
        }

        public  void minusdd(Moment[] par, Moment res)
        {
            res.Real = par[0].Date.Subtract(par[1].Date).TotalSeconds;
        }

        public  void minusdr(Moment[] par, Moment res)
        {
            try { res.Date = par[0].Date.AddSeconds(-par[1].Real);}
            catch { res.Date = par[0].Date;}
        }

        public  void minusrr(Moment[] par, Moment res)
        {
            res.Real = par[0].Real - par[1].Real;
        }

        public  void minusi(Moment[] par, Moment res)
        {
            res.Integer = -par[0].Integer;
        }

        public  void minusr(Moment[] par, Moment res)
        {
            res.Real = -par[0].Real;
        }

        public  void multiplyii(Moment[] par, Moment res)
        {
            if (par[0].Integer == 0)
            {
                res.Nd = par[0].Nd;
                res.Error = par[0].Error;
            }
            if (par[1].Integer == 0)
            {
                res.Nd = par[1].Nd;
                res.Error = par[1].Error;
            }
            if (par[0].Integer == 0 && par[1].Integer == 0)
            {
                res.Nd = MinNd(par);
                res.Error = MinErr(par);
            }
            res.Integer = par[0].Integer * par[1].Integer;
        }

        public void multiplyrr(Moment[] par, Moment res)
        {
            if (par[0].Real == 0)
            {
                res.Nd = par[0].Nd;
                res.Error = par[0].Error;
            }
            if (par[1].Real == 0)
            {
                res.Nd = par[1].Nd;
                res.Error = par[1].Error;
            }
            if (par[0].Real == 0 && par[1].Real == 0)
            {
                res.Nd = MinNd(par);
                res.Error = MinErr(par);
            }
            res.Real = par[0].Real * par[1].Real;
        }

        public  void dividerr(Moment[] par, Moment res)
        {
            if (par[0].Real == 0)
            {
                res.Nd = par[0].Nd;
                res.Error = par[0].Error;
            }
            if (par[1].Real == 0) PutErr("Деление на ноль", res);
            else res.Real = par[0].Real / par[1].Real;
        }

        public  void divii(Moment[] par, Moment res)
        {
            if (par[0].Integer == 0)
            {
                res.Nd = par[0].Nd;
                res.Error = par[0].Error;
            }
            if (par[1].Integer == 0) PutErr("Деление на ноль", res);
            else res.Integer = par[0].Integer / par[1].Integer;
        }

        public  void modii(Moment[] par, Moment res)
        {
            if (par[0].Integer == 0)
            {
                res.Nd = par[0].Nd;
                res.Error = par[0].Error;
            }
            if (par[1].Integer == 0) PutErr("Деление с остатком на ноль", res);
            else res.Integer = par[0].Integer % par[1].Integer;
        }

        public  void powerrr(Moment[] par, Moment res)
        {
            double m0 = par[0].Real, m1 = par[1].Real;
            if (m0 == 0)
            {
                res.Nd = par[0].Nd;
                res.Error = par[0].Error;
            }
            double m = 1;
            if (m0 < 0)
            {
                if (Convert.ToInt32(m1) == m1)
                {
                    if (m1 > 0)
                        for (int i = 1; i <= m1; ++i) m = m * m0;
                    else
                        for (int i = 1; i <= -m1; ++i) m = m / m0;
                }
                else PutErr("Неправильное возведение отрицательного числа в степень", res);
            }
            else
            {
                if (m0 == 0)
                {
                    if (m1 == 0) PutErr("Возведение ноля в нолевую степень", res);
                    else m = 0;
                }
                else
                {
                    if (m1 == 0) m = 1;
                    else
                    {
                        if (Convert.ToInt32(m1) == m1)
                        {
                            if (m1 > 0)
                                for (int i = 1; i <= Convert.ToInt32(m1); ++i)
                                    m = m * m0;
                            else
                                for (int i = 1; i <= -Convert.ToInt32(m1); ++i)
                                    m = m / m0;
                        }
                        else m = Math.Pow(m0, m1);
                    }
                }
            }
            res.Real = m;
        }

        public  void equaluu(Moment[] par, Moment res)
        {
            res.Boolean = par[0] == par[1];
        }

        public  void notequaluu(Moment[] par, Moment res)
        {
            res.Boolean = par[0] != par[1];
        }

        public  void lessuu(Moment[] par, Moment res)
        {
            res.Boolean = par[0] < par[1];
        }

        public  void lessequaluu(Moment[] par, Moment res)
        {
            res.Boolean = par[0] <= par[1];
        }

        public  void greateruu(Moment[] par, Moment res)
        {
            res.Boolean = par[0] > par[1];
        }

        public  void greaterequaluu(Moment[] par, Moment res)
        {
            res.Boolean = par[0] >= par[1];
        }

        public  void notb(Moment[] par, Moment res)
        {
            res.Boolean = !par[0].Boolean;
        }

        public  void xorbb(Moment[] par, Moment res)
        {
            res.Boolean = par[0].Boolean ^ par[1].Boolean;
        }

        public  void xorii(Moment[] par, Moment res)
        {
            res.Integer = par[0].Integer ^ par[1].Integer;
        }

        public  void orbb(Moment[] par, Moment res)
        {
            bool m0 = par[0].Boolean;
            bool m1 = par[1].Boolean;
            if (!m0 && !m1) res.Boolean = false;
            else
            {
                res.Boolean = true;
                if (m0 && !m1)
                {
                    res.Nd = par[0].Nd;
                    res.Error = par[0].Error;
                }
                if (!m0 && m1)
                {
                    res.Nd = par[1].Nd;
                    res.Error = par[1].Error;
                }
                if (m0 && m1)
                {
                    res.Nd = MinNd(par);
                    res.Error = MinErr(par);
                }
            }
        }

        public  void orii(Moment[] par, Moment res)
        {
            res.Integer = par[0].Integer | par[1].Integer;
        }

        public  void andbb(Moment[] par, Moment res)
        {
            bool m0 = par[0].Boolean;
            bool m1 = par[1].Boolean;
            if (m0 && m1) res.Boolean = true;
            else
            {
                res.Boolean = false;
                if (!m0 && m1)
                {
                    res.Nd = par[0].Nd;
                    res.Error = par[0].Error;
                }
                if (m0 && !m1)
                {
                    res.Nd = par[1].Nd;
                    res.Error = par[1].Error;
                }
                if (!m0 && !m1)
                {
                    res.Nd = MinNd(par);
                    res.Error = MinErr(par);
                }
            }
        }

        public  void andii(Moment[] par, Moment res)
        {
            res.Integer = par[0].Integer & par[1].Integer;
        }

        public  void likess(Moment[] par, Moment res)
        {
            string m1 = par[1].String.Replace("*", @"[\S|\s]*").Replace("?", @"[\S|\s]");
            string m0 = par[0].String;
            bool mean = false;
            var r = new Regex(m1);
            if (r.IsMatch(m0))
            {
                Match m = r.Match(m0);
                if (m0.Length == m.Value.Length) mean = true;
            }
            res.Boolean = mean;
        }

        public  void truefun(Moment[] par, Moment res)
        {
            res.Boolean = true;
        }

        public  void falsefun(Moment[] par, Moment res)
        {
            res.Boolean = false;
        }

        public void bitii(Moment[] par, Moment res)
        {
            res.Boolean = par[0].Integer.GetBit(par[1].Integer);
        }

        public  void bitandii(Moment[] par, Moment res)
        {
            var b = true;
            for (int i = 1; i < par.Length; i++ )
                b &= par[0].Integer.GetBit(par[i].Integer);
            res.Boolean = b;
        }

        public  void bitorii(Moment[] par, Moment res)
        {
            var b = false;
            for (int i = 1; i < par.Length; i++)
                b |= par[0].Integer.GetBit(par[i].Integer);
            res.Boolean = b;
        }

        public  void srii(Moment[] par, Moment res)
        {
            res.Integer = par[0].Integer >> par[1].Integer;
        }

        public  void slii(Moment[] par, Moment res)
        {
            res.Integer = par[0].Integer << par[1].Integer;
        }

        public  void random(Moment[] par, Moment res)
        {
            res.Real = new Random().NextDouble();
        }

        public  void randomii(Moment[] par, Moment res)
        {
            res.Integer = new Random().Next(par[0].Integer, par[1].Integer);
        }

        public  void absi(Moment[] par, Moment res)
        {
            res.Integer = Math.Abs(par[0].Integer);
        }

        public  void absr(Moment[] par, Moment res)
        {
            res.Real = Math.Abs(par[0].Real);
        }

        public  void signi(Moment[] par, Moment res)
        {
            res.Integer = Math.Sign(par[0].Integer);
        }

        public  void signr(Moment[] par, Moment res)
        {
            res.Integer = Math.Sign(par[0].Real);
        }

        public  void roundr(Moment[] par, Moment res)
        {
            res.Integer = Convert.ToInt32(par[0].Real);
        }

        public  void roundri(Moment[] par, Moment res)
        {
            res.Real = Math.Round(par[0].Real, par[1].Integer);
        }

        public  void minu(Moment[] par, Moment res)
        {
            Moment mv = par[0];
            for (int i = 1; i < par.Length; ++i)
                if (par[i] < mv) mv = par[i];
            res.CopyValueFrom(mv);
        }

        public  void maxu(Moment[] par, Moment res)
        {
            Moment mv = par[0];
            for (int i = 1; i < par.Length; ++i)
                if (par[i] > mv) mv = par[i];
            res.CopyValueFrom(mv);
        }

        public  void pi(Moment[] par, Moment res)
        {
            res.Real = Math.PI;
        }

        public  void sqrr(Moment[] par, Moment res)
        {
            if (par[0].Real < 0) PutErr("Извлечение корня из отрицательного числа", res);
            else res.Real = Math.Sqrt(par[0].Real);
        }

        public  void cosr(Moment[] par, Moment res)
        {
            res.Real = Math.Cos(par[0].Real);
        }

        public  void sinr(Moment[] par, Moment res)
        {
            res.Real = Math.Sin(par[0].Real);
        }

        public  void tanr(Moment[] par, Moment res)
        {
            double d = Math.Tan(par[0].Real);
            if (Double.IsNaN(d))
                PutErr("Тангенс от недопустимого аргумента", res);
            else res.Real = d;
        }

        public  void ctanr(Moment[] par, Moment res)
        {
            double d = Math.Sin(par[0].Real);
            if (Double.IsNaN(d))
                PutErr("Котангенс от недопустимого аргумента", res);
            else res.Real = Math.Cos(d) / Math.Sin(d);
        }

        public  void arccosr(Moment[] par, Moment res)
        {
            double d = par[0].Real;
            if (d < -1 || d > 1)
                PutErr("Арккосинус от недопустимого аргумента", res);
            else res.Real = Math.Acos(d);
        }

        public  void arcsinr(Moment[] par, Moment res)
        {
            double d = par[0].Real;
            if (d < -1 || d > 1)
                PutErr("Арксинус от недопустимого аргумента", res);
            else res.Real = Math.Asin(d);
        }

        public  void arctanr(Moment[] par, Moment res)
        {
            res.Real = Math.Atan(par[0].Real);
        }

        public  void shr(Moment[] par, Moment res)
        {
            res.Real = Math.Sinh(par[0].Real);
        }

        public  void chr(Moment[] par, Moment res)
        {
            res.Real = Math.Cosh(par[0].Real);
        }

        public  void thr(Moment[] par, Moment res)
        {
            res.Real = Math.Tanh(par[0].Real);
        }

        public  void arcshr(Moment[] par, Moment res)
        {
            double d = par[0].Real;
            try
            {
                d = Math.Log(d + Math.Sqrt(d * d + 1));
                if (Double.IsNaN(d)) PutErr("Гиперболический арксинус от недопустимого аргумента", res);
                else res.Real = d;
            }
            catch { PutErr("Гиперболический арксинус от недопустимого аргумента", res); }
        }

        public  void arcchr(Moment[] par, Moment res)
        {
            double d = par[0].Real;
            try
            {
                d = Math.Log(d + Math.Sqrt(d * d - 1));
                if (Double.IsNaN(d)) PutErr("Гиперболический арккосинус от недопустимого аргумента", res);
                else res.Real = d;
            }
            catch { PutErr("Гиперболический арккосинус от недопустимого аргумента", res); }
        }

        public  void arcthr(Moment[] par, Moment res)
        {
            double d = par[0].Real;
            try
            {
                if (d == 1 || (1 + d) / (1 - d) <= 0)
                    PutErr("Гиперболический арктангенс от недопустимого аргумента", res);
                else res.Real = Math.Log((1 + d) / (1 - d)) / 2;
            }
            catch { PutErr("Гиперболический арктангенс от недопустимого аргумента", res); }
        }

        public  void expr(Moment[] par, Moment res)
        {
            res.Real = Math.Exp(par[0].Real);
        }

        public  void lnr(Moment[] par, Moment res)
        {
            if (par[0].Real <= 0) PutErr("Взятие натурального логарифма от неположительного числа", res);
            else res.Real = Math.Log(par[0].Real);
        }

        public  void log10r(Moment[] par, Moment res)
        {
            if (par[0].Real <= 0) PutErr("Взятие десятичного логарифма от неположительного числа", res);
            else res.Real = Math.Log10(par[0].Real);
        }

        public  void logrr(Moment[] par, Moment res)
        {
            double m0 = par[0].Real, m1 = par[1].Real;
            if (m0 == 1)
            {
                res.Nd = par[0].Nd;
                res.Error = par[0].Error;
            }
            else
            {
                if (m0 <= 0 || m1 <= 0)
                    PutErr("Взятие логарифма от неположительного числа или по неположительному основанию", res);
                else
                {
                    if (m1 == 1) PutErr("Взятие логарифма по основанию 1", res);
                    else res.Real = Math.Log(m0, m1);
                }
            }
        }

        public void frombasesi(Moment[] par, Moment res)
        {
            try
            {
                res.Integer = Convert.ToInt32(par[0].String, par[1].Integer);
            }
            catch
            {
                PutErr("Недопустимые параметры функции FromBase", res);
            }
        }

        public void tobaseii(Moment[] par, Moment res)
        {
            try
            {
                res.String = Convert.ToString(par[0].Integer, par[1].Integer);
            }
            catch
            {
                PutErr("Недопустимые параметры функции ToBase", res);
            }
        }

        public  void uncertainu(Moment[] par, Moment res)
        {
            res.Nd = 0;
            res.Integer = par[0].Nd;
        }

        public  void makecertainuib(Moment[] par, Moment res)
        {
            res.CopyValueFrom(par[0]);
            if (par[2].Boolean) res.Nd = par[1].Integer;
            else res.Nd |= par[1].Integer;
        }

        public  void erroru(Moment[] par, Moment res)
        {
            res.Error = null;
            res.String = par[0].Error != null ? par[0].Error.ToString() : "";
        }

        public  void makeerrorusb(Moment[] par, Moment res)
        {
            res.CopyValueFrom(par[0]);
            if (par[2].Boolean) res.Error = null;
            PutErr(par[1].String, res);
        }

        private  Moment Certain(Moment[] par, Moment pn, double dn, double dp)
        {
            //Аппаратная недостоверность и сравнение с нормативным
            var dpar = (from p in par where p.Error == null && p.Nd == 0 && Math.Abs(p.Real - pn.Real) <= dn select p.Real).ToList();
            if (dpar.Count == 0) return pn;
            //Сравнение друг с другом
            while (dpar.Count > 2)
            {
                double m = dpar.Average();
                int k = 0;
                for (int i = 0; i < dpar.Count; i++)
                    if (Math.Abs(m - dpar[i]) > Math.Abs(m - dpar[k]))
                        k = i;
                if (Math.Abs(m - dpar[k]) > dp)
                    dpar.RemoveAt(k);
                else break;
            }
            if (dpar.Count == 2 && Math.Abs(dpar[1] - dpar[0]) > 2 * dp) return pn;
            return new Moment(dpar.Average());
        }

        public  void certainnprrrr(Moment[] par, Moment res)
        {
            var parv = new Moment[par.Length - 3];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 3];
            res.Error = par[0].Error | par[1].Error | par[2].Error;
            res.Nd = par[0].Nd | par[1].Nd | par[2].Nd;
            res.Real = Certain(parv, par[0], par[1].Real, par[2].Real).Real;
        }

        public  void certainnrrr(Moment[] par, Moment res)
        {
            var parv = new Moment[par.Length - 2];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 2];
            res.Error = par[0].Error | par[1].Error;
            res.Nd = par[0].Nd | par[1].Nd;
            res.Real = Certain(parv, par[0], par[1].Real, double.MaxValue).Real;
        }

        public  void certainprrr(Moment[] par, Moment res)
        {
            var parv = new Moment[par.Length - 2];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 2];
            res.Error = par[0].Error | par[1].Error;
            var m = Certain(parv, par[0], double.MaxValue, par[1].Real);
            res.Real = m.Real;
            res.Nd = par[1].Nd | m.Nd;
        }

        public  void certainrr(Moment[] par, Moment res)
        {
            var parv = new Moment[par.Length - 1];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 1];
            res.Error = par[0].Error;
            var m = Certain(parv, par[0], double.MaxValue, double.MaxValue);
            res.Real = m.Real;
            res.Nd = m.Nd;
        }

        public  void pointtimeu(Moment[] par, Moment res)
        {
            res.Date = par[0].Time;
        }

        public  void valueu(Moment[] par, Moment res)
        {
            //Ничего не делать
        }

        public  void boolu(Moment[] par, Moment res)
        {
            res.Boolean = par[0].String != "0";
            if (par[0].String != "0" && par[0].String != "1")
                PutErr("Ошибка при преобразовании в логическое значение", res);
        }

        public  void intu(Moment[] par, Moment res)
        {
            if (par[0].DataType.LessOrEquals(DataType.Integer))
                res.Integer = par[0].Integer;
            else
            {
                int i;
                if (par[0].DataType == DataType.Real) res.Integer = Convert.ToInt32(Math.Floor(par[0].Real));
                else if (int.TryParse(par[0].String, out i)) res.Integer = i;
                else PutErr("Ошибка при преобразовании в целочисленное значение", res);
            }
        }

        public  void realu(Moment[] par, Moment res)
        {
            if (par[0].DataType.LessOrEquals(DataType.Real))
                res.Real = par[0].Real;
            else
            {
                res.Real = (par[0].String ?? "0").ToDouble();
                if (double.IsNaN(res.Real))
                {
                    res.Real = 0;
                    PutErr("Ошибка при преобразовании в числовое значение", res);
                }
            }
        }

        public  void dateu(Moment[] par, Moment res)
        {
            DateTime d;
            if (DateTime.TryParse(par[0].String, out d)) res.Date = d;
            else PutErr("Ошибка при преобразовании в значение типа время", res);
        }

        public  void isintu(Moment[] par, Moment res)
        {
            int i;
            res.Boolean = int.TryParse(par[0].String, out i);
        }

        public  void isrealu(Moment[] par, Moment res)
        {
            double d;
            res.Boolean = double.TryParse(par[0].String, out d)
                                   || double.TryParse(par[0].String, NumberStyles.Any, new NumberFormatInfo { NumberDecimalSeparator = "." }, out d);
        }

        public  void istimeu(Moment[] par, Moment res)
        {
            DateTime d;
            res.Boolean = DateTime.TryParse(par[0].String, out d);
        }

        public  void stringu(Moment[] par, Moment res)
        {
            res.String = par[0].String;
        }

        public  void cyear(Moment[] par, Moment res)
        {
            res.String = "year";
        }

        public  void cmonth(Moment[] par, Moment res)
        {
            res.String = "month";
        }

        public  void cday(Moment[] par, Moment res)
        {
            res.String = "day";
        }

        public  void chour(Moment[] par, Moment res)
        {
            res.String = "hour";
        }

        public  void cminute(Moment[] par, Moment res)
        {
            res.String = "minute";
        }

        public  void csecond(Moment[] par, Moment res)
        {
            res.String = "second";
        }

        public  void cms(Moment[] par, Moment res)
        {
            res.String = "ms";
        }

        public  void now(Moment[] par, Moment res)
        {
            res.Date = DateTime.Now;
        }

        public  void timeaddsdr(Moment[] par, Moment res)
        {
            DateTime t = par[1].Date;
            try
            {
                switch (par[0].String.ToLower())
                {
                    case "мс":
                    case "ms":
                        t = t.AddMilliseconds(par[2].Real);
                        break;
                    case "секунда":
                    case "сек":
                    case "second":
                        t = t.AddSeconds(par[2].Real);
                        break;
                    case "минута":
                    case "мин":
                    case "minute":
                        t = t.AddMinutes(par[2].Real);
                        break;
                    case "час":
                    case "hour":
                        t = t.AddHours(par[2].Real);
                        break;
                    case "сут":
                    case "сутки":
                    case "day":
                        t = t.AddDays(par[2].Real);
                        break;
                    case "месяц":
                    case "мес":
                    case "month":
                        t = t.AddMonths(Convert.ToInt32(par[2].Real));
                        break;
                    case "год":
                    case "year":
                        t = t.AddYears(Convert.ToInt32(par[2].Real));
                        break;
                    default:
                        PutErr("Недопустимые параметры функции ДобавитьКоВремени (TimeAdd)", res);
                        break;
                }
            }
            catch { PutErr("Недопустимые параметры функции ДобавитьКоВремени (TimeAdd)", res); }
            res.Date = t;
        }

        public  void timediffsdd(Moment[] par, Moment res)
        {
            DateTime m1 = par[1].Date;
            DateTime m2 = par[2].Date;
            try
            {
                switch (par[0].String.ToLower())
                {
                    case "мс":
                    case "ms":
                        res.Real = m1.Subtract(m2).TotalMilliseconds;
                        break;
                    case "секунда":
                    case "сек":
                    case "second":
                        res.Real = m1.Subtract(m2).TotalSeconds;
                        break;
                    case "минута":
                    case "мин":
                    case "minute":
                        res.Real = m1.Subtract(m2).TotalMinutes;
                        break;
                    case "час":
                    case "hour":
                        res.Real = m1.Subtract(m2).TotalHours;
                        break;
                    case "сут":
                    case "сутки":
                    case "day":
                        res.Real = m1.Subtract(m2).TotalDays;
                        break;
                    default:
                        PutErr("Недопустимые параметры функции РазностьВремен (TimeDiff)", res);
                        break;
                }
            }
            catch { PutErr("Недопустимые параметры функции РазностьВремен (TimeDiff)", res); }
        }

        public  void timepartsd(Moment[] par, Moment res)
        {
            DateTime m1 = par[1].Date;
            switch (par[0].String.ToLower())
            {
                case "мс":
                case "ms":
                    res.Integer = m1.Millisecond;
                    break;
                case "секунда":
                case "сек":
                case "second":
                    res.Integer = m1.Second;
                    break;
                case "минута":
                case "мин":
                case "minute":
                    res.Integer = m1.Minute;
                    break;
                case "час":
                case "hour":
                    res.Integer = m1.Hour;
                    break;
                case "сут":
                case "сутки":
                case "day":
                    res.Integer = m1.Day;
                    break;
                case "месяц":
                case "мес":
                case "month":
                    res.Integer = m1.Month;
                    break;
                case "год":
                case "year":
                    res.Integer = m1.Year;
                    break;
                default:
                    PutErr("Недопустимые параметры функции ЧастьВремени (TimePart)", res);
                    break;
            }
        }

        public void timeserialiiiiiir(Moment[] par, Moment res)
        {
            try
            {
                string d = par[0].Integer + "." + par[1].Integer + "." + par[2].Integer + " " + par[3].Integer + ":" + par[4].Integer + ":" + par[5].Integer;
                res.Date = Convert.ToDateTime(d);
                res.Date = res.Date.AddMilliseconds(par[6].Real);
            }
            catch { PutErr("Недопустимые параметры функции СобратьВремя (TimeSerial)", res); }
        }

        public void newline(Moment[] par, Moment res)
        {
            res.String = Environment.NewLine;
        }

        public void tab(Moment[] par, Moment res)
        {
            res.String = "" + ((char)9);
        }

        public  void strmidsi(Moment[] par, Moment res)
        {
            int m1 = par[1].Integer - 1;
            string m0 = par[0].String;
            if (m1 < 0 || m1 >= m0.Length) PutErr("Недопустимые параметры функции strmid", res);
            else res.String = m0.Substring(m1);
        }

        public  void strmidsii(Moment[] par, Moment res)
        {
            int m1 = par[1].Integer - 1, m2 = par[2].Integer;
            string m0 = par[0].String;
            if (m1 < 0 || m1 + m2 > m0.Length || m2 < 0) PutErr("Недопустимые параметры функции strmid", res);
            else res.String = m0.Substring(m1, m2);
        }

        public  void strleftsi(Moment[] par, Moment res)
        {
            int m1 = par[1].Integer;
            string m0 = par[0].String;
            if (m1 < 0 || m1 > m0.Length) PutErr("Недопустимые параметры функции strleft", res);
            else res.String = m0.Substring(0, m1);
        }

        public void strrightsi(Moment[] par, Moment res)
        {
            int m1 = par[1].Integer;
            string m0 = par[0].String;
            if (m1 < 0 || m1 > m0.Length) PutErr("Недопустимые параметры функции strright", res);
            else res.String = m0.Substring(m0.Length - m1, m1);
        }

        public void strinsertssi(Moment[] par, Moment res)
        {
            string m0 = par[0].String;
            string m1 = par[1].String;
            int m2 = par[2].Integer - 1;
            if (m2 < 0 || m2 >= m0.Length) PutErr("Недопустимые параметры функции strinsert", res);
            else res.String = m0.Insert(m2, m1);
        }

        public void strremovesi(Moment[] par, Moment res)
        {
            string m0 = par[0].String;
            int m1 = par[1].Integer - 1;
            if (m1 < 0 || m1 >= m0.Length) PutErr("Недопустимые параметры функции strremove", res);
            else res.String = m0.Remove(m1);
        }

        public void strremovesii(Moment[] par, Moment res)
        {
            string m0 = par[0].String;
            int m1 = par[1].Integer - 1;
            int m2 = par[2].Integer;
            if (m1 < 0 || m1 + m2 > m0.Length || m2 < 0) PutErr("Недопустимые параметры функции strremove", res);
            else res.String = m0.Remove(m1, m2);
        }

        public void strreplacesss(Moment[] par, Moment res)
        {
            string m0 = par[0].String;
            string m1 = par[1].String;
            string m2 = par[2].String;
            res.String = m0.Replace(m1, m2);
        }

        public void strreplaceregsss(Moment[] par, Moment res)
        {
            string m0 = par[0].String;
            var r1 = new Regex(par[1].String);
            string m2 = par[2].String;
            res.String = r1.Replace(m0, m2);
        }

        public void strlens(Moment[] par, Moment res)
        {
            res.Integer = par[0].String.Length;
        }

        public void strfindssi(Moment[] par, Moment res)
        {
            int m2 = par[2].Integer - 1;
            string m0 = par[0].String;
            string m1 = par[1].String;
            if (m2 < 0 || m2 >= m1.Length) PutErr("Недопустимые параметры функции strfind", res);
            else res.Integer = m1.IndexOf(m0, m2) + 1;
        }

        public  void strfindlastssi(Moment[] par, Moment res)
        {
            int m2 = par[2].Integer - 1;
            string m0 = par[0].String;
            string m1 = par[1].String;
            if (m2 < -1 || m2 >= m1.Length) PutErr("Недопустимые параметры функции strfindlast", res);
            else res.Integer = (m2 == -1) ? m1.LastIndexOf(m0) + 1 : m1.LastIndexOf(m0, m2) + 1;
        }

        public  void strtrims(Moment[] par, Moment res)
        {
            res.String = par[0].String.Trim();
        }

        public  void strltrims(Moment[] par, Moment res)
        {
            res.String = par[0].String.TrimStart();
        }

        public  void strrtrims(Moment[] par, Moment res)
        {
            res.String = par[0].String.TrimEnd();
        }

        public  void strlcases(Moment[] par, Moment res)
        {
            res.String = par[0].String.ToLower();
        }

        public  void strucases(Moment[] par, Moment res)
        {
            res.String = par[0].String.ToUpper();
        }

        //Работа со справочными таблицами
        private  bool TablContains(int numTabl, Moment mcode, Moment msub, int fieldNum)
        {
            bool e = true;
            if (!_calc.Project.Tabls.ContainsKey(numTabl)) e = false;
            else
            {
                var tabl = _calc.Project.Tabls[numTabl];
                TablParam p = null;
                if (mcode.DataType.LessOrEquals(DataType.Integer))
                {
                    e &= tabl.ParamsNum.ContainsKey(mcode.Integer);
                    if (e) p = tabl.ParamsNum[mcode.Integer];
                }
                else
                {
                    e &= tabl.ParamsCode.ContainsKey(mcode.String);
                    if (e) p = tabl.ParamsCode[mcode.String];
                }
                if (e && msub != null)
                {
                    if (msub.DataType.LessOrEquals(DataType.Integer))
                        e &= p.ParamsNum.ContainsKey(msub.Integer);
                    else e &= p.ParamsCode.ContainsKey(msub.String);
                    e &= tabl.SubFieldsCount > fieldNum;
                }
                else e &= tabl.FieldsCount > fieldNum;
            }
            return e;
        }
        
        public  void tablcontainsiii(Moment[] par, Moment res)
        {
            res.Boolean = TablContains(par[0].Integer, par[1], null, par[2].Integer);
        }

        public  void tablcontainsisi(Moment[] par, Moment res)
        {
            res.Boolean = TablContains(par[0].Integer, par[1], null, par[2].Integer);
        }

        public  void subtablcontainsiiii(Moment[] par, Moment res)
        {
            res.Boolean = TablContains(par[0].Integer, par[1], par[2], par[3].Integer);
        }

        public  void subtablcontainsiisi(Moment[] par, Moment res)
        {
            res.Boolean = TablContains(par[0].Integer, par[1], par[2], par[3].Integer);
        }

        public  void subtablcontainsisii(Moment[] par, Moment res)
        {
            res.Boolean = TablContains(par[0].Integer, par[1], par[2], par[3].Integer);
        }

        public  void subtablcontainsissi(Moment[] par, Moment res)
        {
            res.Boolean = TablContains(par[0].Integer, par[1], par[2], par[3].Integer);
        }

        //Обрабатывает все фунции GetTabl, GetTablName, GetSubTabl, GetSubTablName
        private  void GetTabl(int numTabl, Moment mcode, Moment msub, int fieldNum, Moment res)
        {
            if (!_calc.Project.Tabls.ContainsKey(numTabl))
                PutErr("Таблица " + numTabl + " не найдена", res);
            else
            {
                var tabl = _calc.Project.Tabls[numTabl];
                bool e = true;
                TablParam p = null;
                if (mcode.DataType.LessOrEquals(DataType.Integer))
                {
                    if (!tabl.ParamsNum.ContainsKey(mcode.Integer)) e = false;
                    else p = tabl.ParamsNum[mcode.Integer];
                }
                else
                {
                    if (!tabl.ParamsCode.ContainsKey(mcode.String)) e = false;
                    else p = tabl.ParamsCode[mcode.String];
                }
                if (e && msub != null)
                {
                    if (msub.DataType.LessOrEquals(DataType.Integer))
                    {
                        if (!p.ParamsNum.ContainsKey(msub.Integer)) e = false;
                        else p = p.ParamsNum[msub.Integer];
                    }
                    else
                    {
                        if (!p.ParamsCode.ContainsKey(msub.String)) e = false;
                        else p = p.ParamsCode[msub.String];
                    }
                }
                e &= fieldNum >= -1;
                if (msub == null) e &= tabl.FieldsCount > fieldNum;
                else e &= tabl.SubFieldsCount > fieldNum;
                if (!e) PutErr("Не найдено значение в таблице " + numTabl 
                    + ", параметр " + mcode.String + (msub == null ? "" : ", подпараметр " + (msub.String ?? "")) + (fieldNum >=0 ? ", колонка " + fieldNum : ""), res);
                else
                    res.String = fieldNum == -1 ? p.Name : p.Values[fieldNum];
            }
        }

        public  void gettabliii(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], null, par[2].Integer, res);
        }

        public  void gettablisi(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], null, par[2].Integer, res);
        }

        public  void gettablnameii(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], null, -1, res);
        }
        
        public  void gettablnameis(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], null, -1, res);
        }

        public  void getsubtabliiii(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], par[3].Integer, res);
        }

        public  void getsubtabliisi(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], par[3].Integer, res);
        }

        public  void getsubtablisii(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], par[3].Integer, res);
        }

        public  void getsubtablissi(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], par[3].Integer, res);
        }

        public  void getsubtablnameiii(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], -1, res);
        }

        public  void getsubtablnameisi(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], -1, res);
        }

        public  void getsubtablnameiis(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], -1, res);
        }

        public  void getsubtablnameiss(Moment[] par, Moment res)
        {
            GetTabl(par[0].Integer, par[1], par[2], -1, res);
        }

        //Оператор end со значениями, параметры - фильтр и значение по true, фильтр и значение по false
        public  void endif(Moment[] par, Moment res)
        {
            if (par[0].Boolean)
            {
                res.CopyValueFrom(par[1]);
                res.Nd = par[0].Nd | par[1].Nd;
                res.Error = par[0].Error | par[1].Error;
            }
            if (par[2].Boolean)
            {
                res.CopyValueFrom(par[3]);
                res.Nd = par[2].Nd | par[3].Nd;
                res.Error = par[2].Error | par[3].Error;
            }
            if (!par[0].Boolean && !par[2].Boolean) res.Nd |= 1;
        }

        //Присвоение: параметры - переменная, выражение и фильтр по if
        public  void assign(Moment[] par, Moment res)
        {
            res.CopyValueFrom(par[2].Boolean ? par[1] : par[0]);
            res.Nd = par[1].Nd | par[2].Nd;
            res.Error = par[1].Error | par[2].Error;
        }

        //Функции типа ComplicateDelegate

        private  bool Compassignp(Moment[] par, Moment res, ISet<int> parnum)
        {
            bool e = parnum.Contains(0) | parnum.Contains(1);
            if (e)
            {
                res.CopyValueFrom(par[2].Boolean ? par[1] : par[0]);
                res.Nd = par[1].Nd | par[2].Nd;
                res.Error = par[1].Error | par[2].Error;   
            }
            return e;
        }

        private  bool Compendifp(Moment[] par, Moment res, ISet<int> parnum)
        {
            if (par[0].Boolean)
            {
                if (parnum.Contains(1))
                {
                    res.CopyValueFrom(par[1]);
                    res.Nd = par[0].Nd | par[1].Nd;
                }
                else return false;
            }
            if (par[2].Boolean)
            {
                if (parnum.Contains(3))
                {
                    res.CopyValueFrom(par[3]);
                    res.Nd = par[2].Nd | par[3].Nd;
                }
                else return false;
            }
            if (!par[0].Boolean && !par[2].Boolean) res.Nd |= 1;   
            return true;
        }

        private bool Compunion(Moment[] par, Moment res, ISet<int> parnum)
        {
            int n = parnum.Min();
            res.CopyValueFrom(par[n]);
            res.Nd = par[n].Nd;
            res.Error = par[n].Error;
            return true;
        }

        private bool Compvalueatpoints(Moment[] par, Moment res, ISet<int> parnum)
        {
            if (parnum.Contains(1)) res.CopyValueFrom(par[0]);
            return parnum.Contains(1);
        }

        private  bool Compselectpoints(Moment[] par, Moment res, ISet<int> parnum)
        {
            bool e = parnum.Contains(0) && par[1].Boolean;
            if (e) res.CopyValueFrom(par[0]);
            return e;
        }

        private  bool Compsettime(Moment[] par, Moment res, ISet<int> parnum)
        {
            if (parnum.Contains(0))
            {
                res.CopyValueFrom(par[0]);
                res.Time = par[1].Date;
            }
            return parnum.Contains(0);
        }
    }
}