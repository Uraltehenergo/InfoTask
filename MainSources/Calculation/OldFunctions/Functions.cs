using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class Functions
    {
        //Делегат для ссылок на функции
        public delegate List<MomentValue> GeneralDelegate(List<MomentValue>[] par, CalcParamRun calc);
        //Делегат для ссылок на все функции массивов
        public delegate CommonValue ArrayDelegate(CommonValue[] par, CalcParamRun calc);

        private Oka Oka;

        public Functions()
        {
            Oka=new Oka();
        }
        
        //Создание нового GeneralValue с одним значением
        public static CommonValue CreateNewCommon(int errnum, int nd, DataType type, DateTime time)
        {
            var res = new CommonValue(type) { Err = new ErrNum(errnum)};
            MomentValue mv = MomentValue.Create(type);
            mv.Err = new ErrNum(errnum);
            mv.Nd = nd;
            mv.Time = time;
            res.Values.Add(mv);
            return res;
        }

        //Вычисление большинства функций 
        public static CommonValue GeneralFunction(CommonValue[] gen, GeneralDelegate func, DataType type, CalcParamRun calc)
        {
            var res = new CommonValue(type);
            var genn = new CommonValue[gen.Length];
            
            //Добавление несуществующих индексов в массивы
            if (func.Method.Name == "updatevalueuuu" || func.Method.Name == "addundefuuu" || func.Method.Name == "addundefuuub")
            {
                for (int i = 0; i < gen.Length; i++) genn[i] = gen[i].Clone();
                if (genn[1].IntArray != null)
                {
                    foreach (int k in genn[1].IntArray.Keys)
                    {
                        if (genn[0].IntArray == null) genn[0].IntArray = new SortedDictionary<int, CommonValue>();
                        if (!genn[0].IntArray.ContainsKey(k))
                        {
                            genn[0].IntArray.Add(k, new CommonValue(genn[0].DataType));
                        }
                    }    
                }
                if (genn[1].StringArray != null)
                {
                    foreach (string k in genn[1].StringArray.Keys)
                    {
                        if (genn[0].StringArray == null) genn[0].StringArray = new SortedDictionary<string, CommonValue>();
                        if (!genn[0].StringArray.ContainsKey(k))
                        {
                            genn[0].StringArray.Add(k, new CommonValue(genn[0].DataType));
                        }
                    }   
                }
            }
            else for (int i = 0; i < gen.Length; i++) genn[i] = gen[i];

            //Определение размерностей
            int ni = genn.Length, ns = genn.Length;
            for (int j = 0; j < genn.Length; ++j)
            {
                if (genn[j].IntArray != null && genn[j].IntArray.Count > 0) ni = j;
                if (genn[j].StringArray != null && genn[j].StringArray.Count > 0) ns = j;
            }

            //Вычисление Values
            var par = new List<MomentValue>[genn.Length];
            for (int j = 0; j < genn.Length; ++j)
            {
                par[j] = genn[j].Values;
            }
            res.Values = func(par, calc);

            //Расчет IntArray
            var pgen = new CommonValue[genn.Length];
            for (int i = 0; i < genn.Length; ++i) {pgen[i] = gen[i];}
            if (ni < genn.Length)
            {
                if (genn[ni].IntArray != null)
                {
                    if (res.IntArray == null) res.IntArray = new SortedDictionary<int, CommonValue>();
                    foreach (int k in genn[ni].IntArray.Keys)
                    {
                        bool e = false;
                        for (int j = 0; j < genn.Length; ++j)
                        {
                            if (genn[j].IntArray != null && genn[j].IntArray.Count > 0)
                            {
                                if (genn[j].IntArray.ContainsKey(k)) pgen[j] = genn[j].IntArray[k];
                                else e = true;
                            }
                        }
                        if (!e) res.IntArray.Add(k, GeneralFunction(pgen, func, type, calc));
                    }   
                }
            }

            //Расчет StringArray
            pgen = new CommonValue[genn.Length];
            for (int i = 0; i < genn.Length; ++i) { pgen[i] = gen[i]; }
            if (ns < genn.Length)
            {
                if (genn[ns].StringArray != null)
                {
                    if (res.StringArray == null) res.StringArray=new SortedDictionary<string, CommonValue>();
                    foreach (string k in genn[ns].StringArray.Keys)
                    {
                        bool e = false;
                        for (int j = 0; j < genn.Length; ++j)
                        {
                            if (genn[j].StringArray != null && genn[j].StringArray.Count > 0)
                            {
                                if (genn[j].StringArray.ContainsKey(k)) pgen[j] = genn[j].StringArray[k];
                                else e = true;
                            }
                        }
                        if (!e) res.StringArray.Add(k, GeneralFunction(pgen, func, type, calc));
                    }   
                }
            }
            res.Nd |= MaxNd(genn);
            res.Err |= MaxErrNum(genn);
            return res;
        }

        //Присвоение
        public static void assign(CommonValue[] gen, CalcParamRun calc)
        {
            gen[0].Err |= gen[1].Err;
            gen[0].Undef |= gen[1].Undef;
            gen[0].Nd |= gen[1].Nd;
            if (gen[1].ClassType == ClassType.Calc)
            {
                gen[0].ClassType = ClassType.Calc;
                gen[0].CalcRun = gen[1].CalcRun;
            }
            CommonValue res = GeneralFunction(gen, updatevalueuuu, gen[0].DataType, calc);
            gen[0].Values = res.Values;
            gen[0].IntArray = res.IntArray;
            gen[0].StringArray = res.StringArray;
        }

        //Проверка фильтра на наличие значений
        public static bool EmptyFilter(CommonValue gen)
        {
            if (gen.Values.Any(mv => (mv.Undef & ZoneFlag.Filter) == 0)) return false;
            if (gen.IntArray != null && gen.IntArray.Keys.Any(key => !EmptyFilter(gen.IntArray[key]))) return false;
           return gen.StringArray == null || gen.StringArray.Keys.All(key => EmptyFilter(gen.StringArray[key]));
        }

        //Если, возвращает фильтр по true
        public static CommonValue ifgoto(CommonValue[] gen, CalcParamRun calc)
        {
            return GeneralFunction(gen, ifgotovbi, DataType.Value, calc);
        }

        //Если, возвращает фильтр по false
        public static CommonValue ifnotgoto(CommonValue[] gen, CalcParamRun calc)
        {
            return GeneralFunction(gen, ifnotgotovbi, DataType.Value, calc);
        }

        // Массивы
        public static CommonValue getelement(CommonValue[] par, CalcParamRun calc)
        {
            CommonValue cv = null;
            switch (par[0].ClassType)
            {
                case ClassType.Void:
                    cv = new CommonValue(ClassType.Void);
                    break;
                case ClassType.Single:
                    cv = new CommonValue(par[0].DataType);
                    break;
                case ClassType.Calc:
                    cv = new CommonValue(par[0].CalcRun);
                    break;
            }
            if (par[1].DataType == DataType.Integer)
            {
                int r = ((MomentInteger)par[1].Values[0]).Mean;
                if (par[0].IntArray == null) par[0].IntArray = new SortedDictionary<int, CommonValue>();
                if (!par[0].IntArray.ContainsKey(r)) par[0].IntArray.Add(r, cv);
                return par[0].IntArray[r];
            }
            string s = ((MomentString)par[1].Values[0]).Mean;
            if (par[0].StringArray == null) par[0].StringArray = new SortedDictionary<string, CommonValue>();
            if (!par[0].StringArray.ContainsKey(s)) par[0].StringArray.Add(s, cv);
            return par[0].StringArray[s];
        }

        public static CommonValue GeneralArrayFunction(CommonValue[] gen, ArrayDelegate func, DataType type, CalcParamRun calc)
        {
            CommonValue res = func(gen, calc);
            res.Nd |= MaxNd(gen);
            res.Err |= MaxErrNum(gen);
            return res;
        }

        private static int dimension(CommonValue gen)
        {
            int mean = 0;
            if (gen.IntArray != null)
                foreach (int i in gen.IntArray.Keys)
                    mean = Math.Max(mean, dimension(gen.IntArray[i]) + 1);
            if (gen.StringArray != null)
                foreach (string s in gen.StringArray.Keys)
                    mean = Math.Max(mean, dimension(gen.StringArray[s])+1);
            return mean;
        }

        public static CommonValue dimensioniu(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, DataType.Integer, calc.ThreadCalc.PeriodBegin);
            ((MomentInteger) res.Values[0]).Mean = dimension(gen[0]);
            return res;
        }

        private static int size(CommonValue gen)
        {
            int mean = 0;
            if (gen.IntArray != null) mean += gen.IntArray.Keys.Sum(i => size(gen.IntArray[i]));
            if (gen.StringArray != null) mean += gen.StringArray.Keys.Sum(s => size(gen.StringArray[s]));
            if (gen.Values.Count > 0) mean++;
            return mean;
        }

        public static CommonValue sizeiui(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, DataType.Integer, calc.ThreadCalc.PeriodBegin);
            if (gen.Length > 1 && (gen[1].Values.Count == 0 || ((MomentInteger)gen[1].Values[0]).Mean < 0 || ((MomentInteger)gen[1].Values[0]).Mean > 2))
            {
                //Неправильный параметр функции Размер(Size)
                res.Err |= 2402;
            }
            else
            {
                int m = 0;
                if (gen.Length > 1 && ((MomentInteger)gen[1].Values[0]).Mean == 1)
                {
                    if (gen[0].IntArray != null) m += gen[0].IntArray.Count;
                    if (gen[0].StringArray != null) m += gen[0].StringArray.Count;
                }
                else m = size(gen[0]);       
                ((MomentInteger) res.Values[0]).Mean = m;
            }
            return res;
        }

        private static int arrsummii(CommonValue gen)
        {
            int mean = 0;
            if (gen.IntArray != null)
                mean += gen.IntArray.Keys.Sum(i => arrsummii(gen.IntArray[i]));
            if (gen.StringArray != null)
                mean += gen.StringArray.Keys.Sum(s => arrsummii(gen.StringArray[s]));
            return mean + gen.Values.Sum(t => ((MomentInteger) gen.Values[0]).Mean);
        }

        public static CommonValue arraysummii(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, DataType.Integer, calc.ThreadCalc.PeriodBegin);
            ((MomentInteger)res.Values[0]).Mean = arrsummii(gen[0]);
            return res;
        }

        private static double arrsummrr(CommonValue gen)
        {
            double mean = 0;
            if (gen.IntArray != null)
                mean += gen.IntArray.Keys.Sum(i => arrsummrr(gen.IntArray[i]));
            if (gen.StringArray != null)
                mean += gen.StringArray.Keys.Sum(s => arrsummrr(gen.StringArray[s]));
            return mean + gen.Values.Sum(t => ((MomentReal)gen.Values[0]).Mean);
        }

        public static CommonValue arraysummrr(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, DataType.Real, calc.ThreadCalc.PeriodBegin);
            ((MomentReal)res.Values[0]).Mean = arrsummrr(gen[0]);
            return res;
        }

        private static string arrsummss(CommonValue gen)
        {
            string mean = "";
            if (gen.IntArray != null)
                foreach (int i in gen.IntArray.Keys)
                    mean += arrsummss(gen.IntArray[i]);
            if (gen.StringArray != null)
                foreach (string s in gen.StringArray.Keys)
                    mean += arrsummss(gen.StringArray[s]);
            for (int i = 0; i < gen.Values.Count; ++i)
                mean += ((MomentString)gen.Values[0]).Mean;
            return mean;
        }

        public static CommonValue arraysummss(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, DataType.Integer, calc.ThreadCalc.PeriodBegin);
            ((MomentString)res.Values[0]).Mean = arrsummss(gen[0]);
            return res;
        }

        public static CommonValue arrayaveragerr(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = arraysummrr(gen, calc);
            CommonValue res2 = sizeiui(gen, calc);
            ((MomentReal)res.Values[0]).Mean /= ((MomentInteger)res2.Values[0]).Mean;
            return res;
        }

        private static MomentValue arrminimum(CommonValue gen)
        {
            MomentValue res = null;
            if (gen.Values.Count > 0) res = gen.Values[0].Clone(); 
            for (int i = 1; i < gen.Values.Count; ++i)
            {
                if (res > gen.Values[i]) res = gen.Values[i].Clone();
            }
            if (gen.IntArray != null)
            {
                foreach (int i in gen.IntArray.Keys)
                {
                    MomentValue mv = arrminimum(gen.IntArray[i]);
                    if (res == null || (mv != null && res > mv)) res = mv.Clone();
                }   
            }
            if (gen.StringArray != null)
            {
                foreach (string s in gen.StringArray.Keys)
                {
                    MomentValue mv = arrminimum(gen.StringArray[s]);
                    if (res == null || (mv != null && res > mv)) res = mv.Clone();
                }   
            }
            return res;
        }

        public static CommonValue arrayminimumbb(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrminimum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arrayminimumii(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrminimum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arrayminimumdd(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrminimum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arrayminimumrr(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrminimum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arrayminimumss(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrminimum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        private static MomentValue arrmaximum(CommonValue gen)
        {
            MomentValue res = null;
            if (gen.Values.Count > 0) res = gen.Values[0].Clone();
            for (int i = 1; i < gen.Values.Count; ++i)
            {
                if (res < gen.Values[i]) res = gen.Values[i].Clone();
            }
            if (gen.IntArray != null)
            {
                foreach (int i in gen.IntArray.Keys)
                {
                    MomentValue mv = arrminimum(gen.IntArray[i]);
                    if (res == null || (mv != null && res < mv)) res = mv.Clone();
                }    
            }
            if (gen.StringArray != null)
            {
                foreach (string s in gen.StringArray.Keys)
                {
                    MomentValue mv = arrminimum(gen.StringArray[s]);
                    if (res == null || (mv != null && res < mv)) res = mv.Clone();
                }    
            }
            return res;
        }

        public static CommonValue arraymaximumbb(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrmaximum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arraymaximumii(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrmaximum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arraymaximumdd(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrmaximum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arraymaximumrr(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrmaximum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue arraymaximumss(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = CreateNewCommon(0, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
            var mv = arrmaximum(gen[0]);
            if (mv != null) res.Values[0] = mv;
            return res;
        }

        public static CommonValue minindexiu(CommonValue[] gen, CalcParamRun calc)
        {
            var res = CreateNewCommon(0, 0, DataType.Integer, calc.ThreadCalc.PeriodBegin);
            if (gen[0].IntArray == null || gen[0].IntArray.Count == 0)
            {
                //Параметр функции МинИндекс (MinIndex) должен быть массивом
                res.Err |= 2403;
            }
            else
            {
                ((MomentInteger)res.Values[0]).Mean = gen[0].IntArray.Keys.First();
            }
            return res;
        }

        public static CommonValue maxindexiu(CommonValue[] gen, CalcParamRun calc)
        {
            var res = CreateNewCommon(0, 0, DataType.Integer, calc.ThreadCalc.PeriodBegin);
            if (gen[0].IntArray == null || gen[0].IntArray.Count == 0)
            {
                //Параметр функции МаксИндекс (MaxIndex) должен быть массивом
                res.Err |= 2404;
            }
            else
            {
                ((MomentInteger)res.Values[0]).Mean = gen[0].IntArray.Keys.Last();
            }
            return res;
        }

        public static CommonValue minstrindexsu(CommonValue[] gen, CalcParamRun calc)
        {
            var res = CreateNewCommon(0, 0, DataType.String, calc.ThreadCalc.PeriodBegin);
            if (gen[0].StringArray == null || gen[0].StringArray.Count == 0)
            {
                //Параметр функции МинСтрИндекс (MinStrIndex) должен быть массивом
                res.Err |= 2405;
            }
            else
            {
                ((MomentString)res.Values[0]).Mean = gen[0].StringArray.Keys.First();
            }
            return res;
        }

        public static CommonValue maxstrindexsu(CommonValue[] gen, CalcParamRun calc)
        {
            var res = CreateNewCommon(0, 0, DataType.String, calc.ThreadCalc.PeriodBegin);
            if (gen[0].StringArray == null || gen[0].StringArray.Count == 0)
            {
                //Параметр функции МаксСтрИндекс (MaxStrIndex) должен быть массивом
                res.Err |= 2406;
            }
            else
            {
                ((MomentString)res.Values[0]).Mean = gen[0].StringArray.Keys.Last();
            }
            return res;
        }

        public static CommonValue arrayvv(CommonValue[] gen, CalcParamRun calc)
        {
            var res = new CommonValue(gen[0].DataType) { IntArray = new SortedDictionary<int, CommonValue>() };
            for (int i = 0; i < gen.Length; ++i)
            {
                res.IntArray.Add(i + 1, gen[i].Clone());
            }
            return res;
        }

        public static CommonValue arraybb(CommonValue[] gen, CalcParamRun calc)
        {
            return arrayvv(gen, calc);
        }

        public static CommonValue arrayii(CommonValue[] gen, CalcParamRun calc)
        {
            return arrayvv(gen, calc);
        }

        public static CommonValue arraydd(CommonValue[] gen, CalcParamRun calc)
        {
            return arrayvv(gen, calc);
        }

        public static CommonValue arrayrr(CommonValue[] gen, CalcParamRun calc)
        {
            return arrayvv(gen, calc);
        }

        public static CommonValue arrayss(CommonValue[] gen, CalcParamRun calc)
        {
            return arrayvv(gen, calc);
        }

        public static CommonValue arraybynumbersviv(CommonValue[] gen, CalcParamRun calc)
        {
            var res = new CommonValue(gen[1].DataType) {IntArray = new SortedDictionary<int, CommonValue>()} ; 
            for (int i = 0; i < gen.Length; i+=2)
            {
                if (gen[i].Values.Count == 0)
                {
                    //Недопустимые параметры функции МассивПоНомерам (ArrayByNumbers)
                    return CreateNewCommon(2411, MaxNd(gen), gen[1].DataType, calc.ThreadCalc.PeriodBegin);
                }
                res.IntArray.Add(((MomentInteger)gen[i].Values[0]).Mean, gen[i+1].Clone());
            }
            return res;
        }

        public static CommonValue arraybynumbersbib(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybynumbersviv(gen, calc);
        }

        public static CommonValue arraybynumbersiii(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybynumbersviv(gen, calc);
        }

        public static CommonValue arraybynumbersdid(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybynumbersviv(gen, calc);
        }

        public static CommonValue arraybynumbersrir(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybynumbersviv(gen, calc);
        }

        public static CommonValue arraybynumberssis(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybynumbersviv(gen, calc);
        }

        public static CommonValue arraybystringsvsv(CommonValue[] gen, CalcParamRun calc)
        {
            var res = new CommonValue (gen[1].DataType) {StringArray = new SortedDictionary<string, CommonValue>()};
            for (int i = 0; i < gen.Length; i += 2)
            {
                if (gen[i].Values.Count == 0)
                {
                    //Недопустимые параметры функции МассивПоСтрокам (ArrayByStrings)
                    return CreateNewCommon(2412, MaxNd(gen), gen[1].DataType, calc.ThreadCalc.PeriodBegin);
                }
                res.StringArray.Add(((MomentString)gen[i].Values[0]).Mean, gen[i+1].Clone());
            }
            return res;
        }

        public static CommonValue arraybystringsbsb(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybystringsvsv(gen, calc);
        }

        public static CommonValue arraybystringsisi(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybystringsvsv(gen, calc);
        }

        public static CommonValue arraybystringsdsd(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybystringsvsv(gen, calc);
        }

        public static CommonValue arraybystringsrsr(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybystringsvsv(gen, calc);
        }

        public static CommonValue arraybystringssss(CommonValue[] gen, CalcParamRun calc)
        {
            return arraybystringsvsv(gen, calc);
        }

        public static CommonValue directproductvvv(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = gen[0].Clone();
            if ((gen[0].IntArray == null || gen[0].IntArray.Count == 0) && (gen[0].StringArray == null || gen[0].StringArray.Count == 0))
            {
                CommonValue res1 = gen[1].Clone();
                res.Values = res1.Values;
                res.IntArray = res1.IntArray;
                res.StringArray = res1.StringArray;
            }
            if (gen[0].IntArray != null)
                foreach(var k in gen[0].IntArray.Keys)
                    res.IntArray[k] = directproductvvv(new[] {gen[0].IntArray[k], gen[1]}, calc);
            if (gen[0].StringArray !=  null)
                foreach (var k in gen[0].StringArray.Keys)
                    res.StringArray[k] = directproductvvv(new[] { gen[0].StringArray[k], gen[1] }, calc);
            return res;
        }

        public static CommonValue directproductbbb(CommonValue[] gen, CalcParamRun calc)
        {
            return directproductvvv(gen, calc);
        }

        public static CommonValue directproductiii(CommonValue[] gen, CalcParamRun calc)
        {
            return directproductvvv(gen, calc);
        }

        public static CommonValue directproductddd(CommonValue[] gen, CalcParamRun calc)
        {
            return directproductvvv(gen, calc);
        }

        public static CommonValue directproductrrr(CommonValue[] gen, CalcParamRun calc)
        {
            return directproductvvv(gen, calc);
        }

        public static CommonValue directproductsss(CommonValue[] gen, CalcParamRun calc)
        {
            return directproductvvv(gen, calc);
        }

        public static CommonValue selectarray(CommonValue[] gen, CalcParamRun calc)
        {
            if (gen[0].IntArray != null)
            {
                var kk = gen[0].IntArray.Keys.ToArray();
                foreach (var k in kk)
                {
                    if (gen[1].IntArray == null || !gen[1].IntArray.ContainsKey(k))
                    {
                        gen[0].IntArray.Remove(k);
                    }
                    else
                    {
                        gen[0].IntArray[k] = selectarray(gen.Length == 2 ? new[] { gen[0].IntArray[k], gen[1].IntArray[k] } : new[] { gen[0].IntArray[k], gen[1].IntArray[k], gen[2] }, calc);
                    }
                }   
            }
            if (gen[0].StringArray != null)
            {
                var ks = gen[0].StringArray.Keys.ToArray();
                foreach (var k in ks)
                {
                    if (gen[1].StringArray == null || !gen[1].StringArray.ContainsKey(k))
                    {
                        gen[0].StringArray.Remove(k);
                    }
                    else
                    {
                        gen[0].StringArray[k] = selectarray(gen.Length == 2 ? new[] { gen[0].StringArray[k], gen[1].StringArray[k] } : new[] { gen[0].StringArray[k], gen[1].StringArray[k], gen[2] }, calc);
                    }
                }   
            }
            if (gen.Length == 3)
            {
                if (gen[1].IntArray != null)
                {
                    if (gen[0].IntArray == null) gen[0].IntArray = new SortedDictionary<int, CommonValue>();
                    var kk = gen[1].IntArray.Keys.ToArray();
                    foreach (var k in kk)
                    {
                        if (!gen[0].IntArray.ContainsKey(k))
                        {
                            gen[0].IntArray.Add(k, new CommonValue(gen[0].DataType));
                            gen[0].IntArray[k] = selectarray(new[] { gen[0].IntArray[k], gen[1].IntArray[k], gen[2] }, calc);
                        }
                    }   
                }

                if (gen[1].StringArray != null)
                {
                    if (gen[0].StringArray == null) gen[0].StringArray = new SortedDictionary<string, CommonValue>();
                    var ks = gen[1].StringArray.Keys.ToArray();
                    foreach (var k in ks)
                    {
                        if (!gen[0].StringArray.ContainsKey(k))
                        {
                            gen[0].StringArray.Add(k, new CommonValue(gen[0].DataType));
                            gen[0].StringArray[k] =
                                selectarray(new[] {gen[0].StringArray[k], gen[1].StringArray[k], gen[2]}, calc);
                        }
                    }
                }
                if ((gen[0].IntArray == null || gen[0].IntArray.Count == 0) && (gen[0].StringArray == null || gen[0].StringArray.Count == 0) && gen[0].Values.Count == 0)
                {
                    //Недопустимые параметры функции ВыделитьМассив (SelectArray)
                    if (gen[2].Values.Count == 0) return CreateNewCommon(2413, 0, gen[0].DataType, calc.ThreadCalc.PeriodBegin);
                    gen[0].Values.Add(gen[2].Values[0].Clone());
                }
            }
            return gen[0];
        }

        public static CommonValue selectarrayvvu(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] {gen[0].Clone(), gen[1]}, calc);
        }

        public static CommonValue selectarraybbu(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1] }, calc);
        }

        public static CommonValue selectarrayiiu(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1] }, calc);
        }

        public static CommonValue selectarrayddu(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1] }, calc);
        }

        public static CommonValue selectarrayrru(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1] }, calc);
        }

        public static CommonValue selectarrayssu(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1] }, calc);
        }

        public static CommonValue selectarrayvvuv(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1], gen[2] }, calc);
        }

        public static CommonValue selectarraybbub(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1], gen[2] }, calc);
        }

        public static CommonValue selectarrayiiui(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1], gen[2] }, calc);
        }

        public static CommonValue selectarrayddud(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1], gen[2] }, calc);
        }

        public static CommonValue selectarrayrrur(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1], gen[2] }, calc);
        }

        public static CommonValue selectarrayssus(CommonValue[] gen, CalcParamRun calc)
        {
            return selectarray(new[] { gen[0].Clone(), gen[1], gen[2] }, calc);
        }

        public static CommonValue deleteelementuui(CommonValue[] gen, CalcParamRun calc)
        {
            if (gen[1].Values.Count == 0) return gen[0];
            int k = ((MomentInteger) gen[1].Values[0]).Mean;
            if (gen[0].IntArray != null && gen[0].IntArray.ContainsKey(k))
                gen[0].IntArray.Remove(k);
            return gen[0];
        }

        public static CommonValue deleteelementvvi(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuui(gen, calc);
        }

        public static CommonValue deleteelementbbi(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuui(gen, calc);
        }

        public static CommonValue deleteelementiii(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuui(gen, calc);
        }

        public static CommonValue deleteelementddi(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuui(gen, calc);
        }

        public static CommonValue deleteelementrri(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuui(gen, calc);
        }

        public static CommonValue deleteelementssi(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuui(gen, calc);
        }

        public static CommonValue deleteelementuus(CommonValue[] gen, CalcParamRun calc)
        {
            if (gen[1].Values.Count == 0) return gen[0];
            string s = ((MomentString)gen[1].Values[0]).Mean;
            if (gen[0].StringArray != null && gen[0].StringArray.ContainsKey(s))
                gen[0].StringArray.Remove(s);
            return gen[0];
        }

        public static CommonValue deleteelementvvs(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuus(gen, calc);
        }

        public static CommonValue deleteelementbbs(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuus(gen, calc);
        }

        public static CommonValue deleteelementiis(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuus(gen, calc);
        }

        public static CommonValue deleteelementdds(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuus(gen, calc);
        }

        public static CommonValue deleteelementsss(CommonValue[] gen, CalcParamRun calc)
        {
            return deleteelementuus(gen, calc);
        }
        
        public static CommonValue strsplitsss(CommonValue[] gen, CalcParamRun calc)
        {
            if (gen[0].Values.Count == 0 || gen[1].Values.Count == 0)
            {
                //Недопустимые параметры функции StrSplit
                return CreateNewCommon(2208, MaxNd(gen), DataType.String, calc.ThreadCalc.PeriodBegin);
            }
            var res = new CommonValue(DataType.String) {IntArray = new SortedDictionary<int, CommonValue>()};
            var sep = new [] {((MomentString) gen[1].Values[0]).Mean};
            string[] ss = ((MomentString) gen[0].Values[0]).Mean.Split(sep, StringSplitOptions.None);
            int i = 1;
            foreach (var s in ss)
            {
                var cv = CreateNewCommon(0, gen[0].Nd, DataType.String, gen[0].Values[0].Time);
                ((MomentString) cv.Values[0]).Mean = s;
                res.IntArray.Add(i, cv);
                i++;
            }
            return res;
        }

        //Функции без массивов

        //Интерполяция типа type значений par на время time по точке с номером n и следующим за ней
        private static MomentValue Interpolation(InterpolationType type, List<MomentValue> par, int n, DateTime time)
        {
            if (par == null || par.Count == 0) return null;
            if (n >= 0 && time == par[n].Time) return par[n].Clone();
            MomentValue res = (n == -1) ? par[0].Clone() : par[n].Clone();
            res.Time = time;
            switch (type)
            {
                case InterpolationType.Constant:
                    return res;

                case InterpolationType.Linear:
                    if (n >= 0 && n < par.Count - 1)
                    {
                        res.Undef = par[n].Undef | par[n+1].Undef;
                        if (res.Undef == 0)
                        {
                            res.Nd = par[n].Nd;
                            res.Err = par[n].Err;
                            switch (res.DataType)
                            {
                                case DataType.Real:
                                    double x1 = ((MomentReal)par[n+1]).Mean;
                                    double x0 = ((MomentReal)par[n]).Mean;
                                    double t = time.Subtract(par[n].Time).TotalSeconds;
                                    double t0 = par[n+1].Time.Subtract(par[n].Time).TotalSeconds;
                                    if (t0 == 0 || t == 0)
                                    {
                                        ((MomentReal)res).Mean = x0;
                                    }
                                    else
                                    {
                                        ((MomentReal)res).Mean = x0 + t * (x1 - x0) / t0;
                                    }
                                    res.Nd = res.Nd | par[n + 1].Nd;
                                    break;

                                case DataType.Time:
                                    DateTime x1d = ((MomentTime)par[n+1]).Mean;
                                    DateTime x0d = ((MomentTime)par[n]).Mean;
                                    t = time.Subtract(par[n].Time).TotalSeconds;
                                    t0 = par[n+1].Time.Subtract(par[n].Time).TotalSeconds;
                                    if (t0 == 0 || t == 0)
                                    {
                                        ((MomentTime)res).Mean = x0d;
                                    }
                                    else
                                    {
                                        ((MomentTime)res).Mean = x0d.AddSeconds(t * x1d.Subtract(x0d).TotalSeconds / t0);
                                    }
                                    res.Nd = res.Nd | par[n + 1].Nd;
                                    break;
                            }      
                        }
                    }
                    return res;
            }
            return res;
        }
        
        private static double SimpleIntegral(InterpolationType type, MomentValue m1, MomentValue m2)
        {
            switch (type)
            {
                case InterpolationType.Constant:
                     return ((MomentReal)m1).Mean * (m2.Time.Subtract(m1.Time).TotalSeconds);

                case InterpolationType.Linear:
                    return (((MomentReal)m1).Mean + ((MomentReal)m2).Mean) / 2 * (m2.Time.Subtract(m1.Time).TotalSeconds);
            }
            return 0;
        }

        private static List<MomentValue> GeneralSimple(List<MomentValue>[] par, Func<MomentValue[], MomentValue> funcsimple, CalcParamRun calc)
        {
            return GeneralCommon(par, null, funcsimple, calc, 0);
        }

        private static List<MomentValue> GeneralComplicate(List<MomentValue>[] par, Func<FunParams, List<MomentValue>, int> funccomplicate, CalcParamRun calc, int flags)
        {
            return GeneralCommon(par, funccomplicate, null, calc, flags);
        }

        //Расчет простых функций, параметры- список параметров и скалярная функция расчета
        private static List<MomentValue> GeneralCommon(List<MomentValue>[] par, Func<FunParams, List<MomentValue>, int> funccomplicate, Func<MomentValue[], MomentValue> funcsimple, CalcParamRun calc, int flags)
        {
            int n = par.Length;
            var fp = new FunParams(calc, par, flags);
            var pos = new int[n];
            for (int i = 0; i < n; ++i)
            {
                if (par[i].Count == 0)
                {
                    if ((fp.Flags & FunFlags.AllowEmptyList) == 0)
                    {
                        return new List<MomentValue>();
                    }
                    fp.Par[i] = null;
                }
                else
                {
                    fp.Par[i] = par[i][0].Clone();
                }
                pos[i] = -1;
                fp.Number[i] = -1;
            }

            var res = new List<MomentValue>();
            DateTime d = calc.ThreadCalc.PeriodEnd.AddDays(1000);
            bool e = true, ebegin = (fp.Flags & FunFlags.AddBegin) != 0;
            while (e)
            {
                e = false;
                DateTime ctime = d;
                for (int i = 0; i < n; ++i)
                {
                    if (pos[i] + 1 < par[i].Count)
                    {
                        e = true;
                        if (ctime >= par[i][pos[i] + 1].Time)
                        {
                            ctime = par[i][pos[i] + 1].Time;
                        }
                        if (ebegin && pos[i] == -1 && par[i][0].Time > calc.ThreadCalc.PeriodBegin)
                        {
                            ctime = calc.ThreadCalc.PeriodBegin;
                            ebegin = false;
                        }   
                    }
                }
                if (!e && (fp.Flags & FunFlags.AddEnd) != 0 && fp.Time != calc.ThreadCalc.PeriodEnd)
                {
                    e = true;
                    ctime = calc.ThreadCalc.PeriodEnd;
                }
                if (e)
                {
                    fp.Time = ctime;
                    for (int i = 0; i < n; ++i)
                    {
                        if ((pos[i] + 1 < par[i].Count && fp.Time == par[i][pos[i] + 1].Time))
                        {
                            pos[i]++;
                            fp.Current[i] = true;
                            fp.Number[i] = pos[i];
                        }
                        else
                        {
                            fp.Current[i] = false;
                        }
                        fp.Par[i] = Interpolation(calc.CalcParam.Interpolation, par[i], pos[i], fp.Time);
                    }

                    if  (fp.Flags == FunFlags.Empty)
                    {
                        MomentValue mv = funcsimple(fp.Par);
                        mv.Time = fp.Time;
                        mv.Undef = MaxUndef(fp.Par);
                        res.Add(mv);    
                    }
                    else funccomplicate(fp, res);
                }
            }
            return res;
        }
        
        public static CommonValue unionvvv(CommonValue[] gen, CalcParamRun calc)
        {
            CommonValue res = gen[0].Clone();
            for(int i = 1; i < gen.Length; ++i)
            {
                if (gen[i].IntArray != null)
                {
                    foreach (int k in gen[i].IntArray.Keys)
                    {
                        if (!res.IntArray.ContainsKey(k))
                        {
                            res.IntArray.Add(k, gen[i].IntArray[k].Clone());
                        }
                        else
                        {
                            res.IntArray[k] = unionvvv(new[] { res.IntArray[k], gen[i].IntArray[k] }, calc);
                        }
                    }   
                }
                if (gen[i].StringArray != null)
                {
                    foreach (string k in gen[i].StringArray.Keys)
                    {
                        if (!res.StringArray.ContainsKey(k))
                        {
                            res.StringArray.Add(k, gen[i].StringArray[k].Clone());
                        }
                        else
                        {
                            res.StringArray[k] = unionvvv(new[] { res.StringArray[k], gen[i].StringArray[k] }, calc);
                        }
                    }  
                }
            }
            return res;
        }

        public static CommonValue unionbbb(CommonValue[] gen, CalcParamRun calc)
        {
            return unionvvv(gen, calc);
        }

        public static CommonValue unioniii(CommonValue[] gen, CalcParamRun calc)
        {
            var genn = new CommonValue[gen.Length];
            for(int i = 0;i < gen.Length; ++i)
            {
                if ((gen[i].IntArray != null && gen[i].IntArray.Count != 0) || (gen[i].StringArray != null && gen[i].StringArray.Count != 0))
                {
                    genn[i] = gen[i];
                }
                else
                {
                    genn[i] = CreateNewCommon(0, 0, DataType.Integer, calc.ThreadCalc.PeriodBegin);
                    if (gen[i].Values.Count == 0)
                    {
                        //Недопустимые параметры функции ОбъединитьМассивы (ArrayUnion)
                        genn[i].Err |= 2415;
                    }
                    else
                    {
                        int mean = ((MomentInteger)gen[i].Values[0]).Mean;
                        CommonValue mv = CreateNewCommon(0, gen[i].Values[0].Nd, DataType.Integer, calc.ThreadCalc.PeriodBegin);
                        ((MomentInteger) mv.Values[0]).Mean = mean;
                        if (genn[i].IntArray == null) genn[i].IntArray = new SortedDictionary<int, CommonValue>();
                        genn[i].IntArray.Add(mean, mv);
                    }
                }
            }
            return unionvvv(genn, calc);
        }

        public static CommonValue unionddd(CommonValue[] gen, CalcParamRun calc)
        {
            return unionvvv(gen, calc);
        }

        public static CommonValue unionrrr(CommonValue[] gen, CalcParamRun calc)
        {
            return unionvvv(gen, calc);
        }

        public static CommonValue unionss(CommonValue[] gen, CalcParamRun calc)
        {
            var genn = new CommonValue[gen.Length];
            for (int i = 0; i < gen.Length; ++i)
            {
                if (gen[i].IntArray.Count != 0 || gen[i].StringArray.Count != 0)
                {
                    genn[i] = gen[i];
                }
                else
                {
                    genn[i] = CreateNewCommon(0, 0, DataType.String, calc.ThreadCalc.PeriodBegin);
                    if (gen[i].Values.Count == 0)
                    {
                        //Недопустимые параметры функции ОбъединитьМассивы (ArrayUnion)
                        genn[i].Err |= 2415;
                    }
                    else
                    {
                        string mean = ((MomentString)gen[i].Values[0]).Mean;
                        CommonValue mv = CreateNewCommon(0, gen[i].Values[0].Nd, DataType.String, calc.ThreadCalc.PeriodBegin);
                        ((MomentString)mv.Values[0]).Mean = mean;
                        genn[i].StringArray.Add(mean, mv);
                    }
                }
            }
            return unionvvv(genn, calc);
        }

        private static bool getinbuu(CommonValue  gen, MomentValue mv)
        {
            switch (mv.DataType)
            {
                case DataType.Boolean:
                    bool bb = ((MomentBoolean)mv).Mean;
                    bool b = gen.Values.Count > 0 && ((MomentBoolean)gen.Values[0]).Mean == bb;
                    if (gen.IntArray != null)
                        b |= gen.IntArray.Values.Any(x => (x.Values.Count > 0 && ((MomentBoolean)x.Values[0]).Mean == bb));
                    if (gen.StringArray != null)
                        b |= gen.StringArray.Values.Any(x => (x.Values.Count > 0 && ((MomentBoolean)x.Values[0]).Mean == bb));
                    return b;
                case DataType.Integer:
                    int v = ((MomentInteger)mv).Mean;
                    b = gen.Values.Count > 0 && ((MomentInteger) gen.Values[0]).Mean == v;
                    if (gen.IntArray != null)
                        b |= gen.IntArray.Values.Any(x => (x.Values.Count > 0 && ((MomentInteger) x.Values[0]).Mean == v));
                    if (gen.StringArray != null)
                        b |= gen.StringArray.Values.Any(x => (x.Values.Count > 0 && ((MomentInteger)x.Values[0]).Mean == v));
                    return b;
                case DataType.Time:
                    DateTime t = ((MomentTime)mv).Mean;
                    b = gen.Values.Count > 0 && ((MomentTime)gen.Values[0]).Mean == t;
                    if (gen.IntArray != null)
                        b |= gen.IntArray.Values.Any(x => (x.Values.Count > 0 && ((MomentTime)x.Values[0]).Mean == t));
                    if (gen.StringArray != null)
                        b |= gen.StringArray.Values.Any(x => (x.Values.Count > 0 && ((MomentTime)x.Values[0]).Mean == t));
                    return b;
                case DataType.Real:
                    double r = ((MomentReal)mv).Mean;
                    b = gen.Values.Count > 0 && ((MomentReal)gen.Values[0]).Mean == r;
                    if (gen.IntArray != null)
                        b |= gen.IntArray.Values.Any(x => (x.Values.Count > 0 && ((MomentReal)x.Values[0]).Mean == r));
                    if (gen.StringArray != null)
                        b |= gen.StringArray.Values.Any(x => (x.Values.Count > 0 && ((MomentReal)x.Values[0]).Mean == r));
                    return b;
                case DataType.String:
                    string s = ((MomentString)mv).Mean;
                    b = gen.Values.Count > 0 && ((MomentString)gen.Values[0]).Mean == s;
                    if (gen.IntArray != null)
                        b |= gen.IntArray.Values.Any(x => (x.Values.Count > 0 && ((MomentString)x.Values[0]).Mean == s));
                    if (gen.StringArray != null)
                        b |= gen.StringArray.Values.Any(x => (x.Values.Count > 0 && ((MomentString)x.Values[0]).Mean == s));
                    return b;
            }
            return false;
        }

        public static CommonValue inbuu(CommonValue[] gen, CalcParamRun calc)
        {
            var res = new CommonValue(DataType.Boolean);
            foreach (var mv in gen[0].Values)
            {
                res.Values.Add(new MomentBoolean(mv.Time, getinbuu(gen[1], mv), mv.Nd | gen[1].Nd, mv.Err | gen[1].Err, mv.Undef | gen[1].Undef));
            }
            if (gen[0].IntArray != null)
            {
                res.IntArray = new SortedDictionary<int, CommonValue>();
                foreach (var k in gen[0].IntArray)
                {
                    var cv = new CommonValue[2];
                    cv[0] = k.Value;
                    cv[1] = gen[1];
                    res.IntArray.Add(k.Key, inbuu(cv, calc));
                }
            }
            if (gen[0].StringArray != null)
            {
                res.StringArray = new SortedDictionary<string, CommonValue>();
                foreach (var k in gen[0].StringArray)
                {
                    var cv = new CommonValue[2];
                    cv[0] = k.Value;
                    cv[1] = gen[1];
                    res.StringArray.Add(k.Key, inbuu(cv, calc));
                }
            }
            return res;
        }

        public static CommonValue inbbb(CommonValue[] gen, CalcParamRun calc)
        {
            return inbuu(gen, calc);
        }

        public static CommonValue inbii(CommonValue[] gen, CalcParamRun calc)
        {
            return inbuu(gen, calc);
        }

        public static CommonValue inbtt(CommonValue[] gen, CalcParamRun calc)
        {
            return inbuu(gen, calc);
        }

        public static CommonValue inbrr(CommonValue[] gen, CalcParamRun calc)
        {
            return inbuu(gen, calc);
        }

        public static CommonValue inbss(CommonValue[] gen, CalcParamRun calc)
        {
            return inbuu(gen, calc);
        }
        
        //10 - Операция

        public static List<MomentValue> plusiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarplusiii, calc);
        }

        public static List<MomentValue> plusddr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarplusddr, calc);
        }

        public static List<MomentValue> plusdrd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarplusdrd, calc);
        }

        public static List<MomentValue> plusrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarplusrrr, calc);
        }

        public static List<MomentValue> plussss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarplussss, calc);
        }

        public static List<MomentValue> minusii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminusii, calc);
        }

        public static List<MomentValue> minusrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminusrr, calc);
        }

        public static List<MomentValue> minusiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminusiii, calc);
        }

        public static List<MomentValue> minusddr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminusddr, calc);
        }

        public static List<MomentValue> minusrdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminusrdd, calc);
        }

        public static List<MomentValue> minusrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminusrrr, calc);
        }

        public static List<MomentValue> multiplyiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmultiplyiii, calc);
        }

        public static List<MomentValue> multiplyrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmultiplyrrr, calc);
        }

        public static List<MomentValue> dividerrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalardividerrr, calc);
        }

        public static List<MomentValue> diviii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalardiviii, calc);
        }

        public static List<MomentValue> modiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmodiii, calc);
        }

        public static List<MomentValue> powerrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarpowerrrr, calc);
        }

        public static List<MomentValue> equalbbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarequalbuu, calc);
        }

        public static List<MomentValue> equalbii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarequalbuu, calc);
        }

        public static List<MomentValue> equalbdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarequalbuu, calc);
        }

        public static List<MomentValue> equalbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarequalbuu, calc);
        }

        public static List<MomentValue> equalbss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarequalbuu, calc);
        }

        public static List<MomentValue> notequalbbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarnotequalbuu, calc);
        }

        public static List<MomentValue> notequalbii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarnotequalbuu, calc);
        }

        public static List<MomentValue> notequalbdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarnotequalbuu, calc);
        }

        public static List<MomentValue> notequalbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarnotequalbuu, calc);
        }

        public static List<MomentValue> notequalbss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarnotequalbuu, calc);
        }

        public static List<MomentValue> lessequalbii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessequalbuu, calc);
        }

        public static List<MomentValue> lessequalbdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessequalbuu, calc);
        }

        public static List<MomentValue> lessequalbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessequalbuu, calc);
        }

        public static List<MomentValue> lessequalbss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessequalbuu, calc);
        }

        public static List<MomentValue> lessbii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessbuu, calc);
        }

        public static List<MomentValue> lessbdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessbuu, calc);
        }

        public static List<MomentValue> lessbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessbuu, calc);
        }

        public static List<MomentValue> lessbss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlessbuu, calc);
        }

        public static List<MomentValue> greaterequalbii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterequalbuu, calc);
        }

        public static List<MomentValue> greaterequalbdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterequalbuu, calc);
        }

        public static List<MomentValue> greaterequalbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterequalbuu, calc);
        }

        public static List<MomentValue> greaterequalbss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterequalbuu, calc);
        }

        public static List<MomentValue> greaterbii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterbuu, calc);
        }

        public static List<MomentValue> greaterbdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterbuu, calc);
        }

        public static List<MomentValue> greaterbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterbuu, calc);
        }

        public static List<MomentValue> greaterbss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalargreaterbuu, calc);
        }

        public static List<MomentValue> notbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarnotbb, calc);
        }

        public static List<MomentValue> orbbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarorbbb, calc);
        }

        public static List<MomentValue> oriii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalaroriii, calc);
        }

        public static List<MomentValue> andbbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarandbbb, calc);
        }

        public static List<MomentValue> andiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarandiii, calc);
        }

        public static List<MomentValue> xorbbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarxorbbb, calc);
        }

        public static List<MomentValue> xoriii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarxoriii, calc);
        }

        public static List<MomentValue> likebss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlikebss, calc);
        }

        //2 - Логическая

        public static List<MomentValue> trueb(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            var v = new MomentBoolean(calc.ThreadCalc.PeriodBegin, true, 0, 0, 0);
            res.Add(v);
            return res;
        }

        public static List<MomentValue> falseb(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            var v = new MomentBoolean(calc.ThreadCalc.PeriodBegin, false, 0, 0, 0);
            res.Add(v);
            return res;
        }

        public static List<MomentValue> bitbii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarbitbii, calc);
        }

        public static List<MomentValue> sliii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarsliii, calc);
        }

        public static List<MomentValue> sriii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarsriii, calc);
        }

        // 3 - Математическая
        public static List<MomentValue> pir(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            var v = new MomentReal(calc.ThreadCalc.PeriodBegin, Math.PI, 0, 0, 0);
            res.Add(v);
            return res;
        }

        public static List<MomentValue> randomiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarrandomiii, calc);
        }

        public static List<MomentValue> randomr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            var r = new Random();
            var v = new MomentReal(calc.ThreadCalc.PeriodBegin, r.NextDouble(), 0, 0, 0);
            res.Add(v);
            return res;
        }

        public static List<MomentValue> absii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarabsii, calc);
        }

        public static List<MomentValue> absrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarabsrr, calc);
        }

        public static List<MomentValue> signii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarsignii, calc);
        }

        public static List<MomentValue> signir(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarsignir, calc);
        }

        public static List<MomentValue> minii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminuu, calc);
        }

        public static List<MomentValue> mindd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminuu, calc);
        }

        public static List<MomentValue> minrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminuu, calc);
        }

        public static List<MomentValue> minss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarminuu, calc);
        }

        public static List<MomentValue> maxii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmaxuu, calc);
        }

        public static List<MomentValue> maxdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmaxuu, calc);
        }

        public static List<MomentValue> maxrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmaxuu, calc);
        }

        public static List<MomentValue> maxss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmaxuu, calc);
        }

        public static List<MomentValue> roundir(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarroundir, calc);
        }

        public static List<MomentValue> roundrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarroundrri, calc);
        }

        public static List<MomentValue> sqrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarsqrrr, calc);
        }

        public static List<MomentValue> cosrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcosrr, calc);
        }

        public static List<MomentValue> sinrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarsinrr, calc);
        }

        public static List<MomentValue> tanrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalartanrr, calc);
        }

        public static List<MomentValue> ctanrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarctanrr, calc);
        }

        public static List<MomentValue> arccosrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalararccosrr, calc);
        }

        public static List<MomentValue> arcsinrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalararcsinrr, calc);
        }

        public static List<MomentValue> arctanrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalararctanrr, calc);
        }

        public static List<MomentValue> shrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarshrr, calc);
        }

        public static List<MomentValue> chrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarchrr, calc);
        }

        public static List<MomentValue> thrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarthrr, calc);
        }

        public static List<MomentValue> arcshrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalararcshrr, calc);
        }

        public static List<MomentValue> arcchrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalararcchrr, calc);
        }

        public static List<MomentValue> arcthrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalararcthrr, calc);
        }

        public static List<MomentValue> exprr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarexprr, calc);
        }

        public static List<MomentValue> lnrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlnrr, calc);
        }

        public static List<MomentValue> log10rr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlog10rr, calc);
        }

        public static List<MomentValue> logrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarlogrrr, calc);
        }

        // 4 - Недостоверность

        public static List<MomentValue> uncertainiu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalaruncertainiu, calc);
        }

        public static List<MomentValue> undefiu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarundefiu, calc);
        }

        public static List<MomentValue> erroriu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarerroriu, calc);
        }

        public static List<MomentValue> makecertainvvib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakecertainuuib, calc);
        }

        public static List<MomentValue> makeacertainbbib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakecertainuuib, calc);
        }

        public static List<MomentValue> makecertainiiib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakecertainuuib, calc);
        }

        public static List<MomentValue> makecertainddib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakecertainuuib, calc);
        }

        public static List<MomentValue> makecertainrrib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakecertainuuib, calc);
        }

        public static List<MomentValue> makecertainssib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakecertainuuib, calc);
        }

        public static List<MomentValue> makeerrorvvib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakeerroruuib, calc);
        }

        public static List<MomentValue> makeerrorbbib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakeerroruuib, calc);
        }

        public static List<MomentValue> makeerroriiib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakeerroruuib, calc);
        }

        public static List<MomentValue> makeerrorddib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakeerroruuib, calc);
        }

        public static List<MomentValue> makeerrorrrib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakeerroruuib, calc);
        }

        public static List<MomentValue> makeerrorssib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarmakeerroruuib, calc);
        }

        public static List<MomentValue> makeundefvvib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarmakeundefuuib, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> makeundefbbib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarmakeundefuuib, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> makeundefiiib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarmakeundefuuib, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> makeundefddib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarmakeundefuuib, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> makeundefrrib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarmakeundefuuib, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> makeundefssib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarmakeundefuuib, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefuuu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefuuub(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefvvub(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefbbub(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefiiub(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefddub(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefrrub(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addundefssub(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddundefuuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> checkcertain2rirrrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcheckcertain2rirrrrrrr, calc);
        }

        public static List<MomentValue> checkcertainnrirrrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcheckcertainnrirrrrrrr, calc);
        }

        public static List<MomentValue> certainnprrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainnprrrrr, calc);
        }

        public static List<MomentValue> certainnrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainnrrrr, calc);
        }

        public static List<MomentValue> certainprrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainprrr, calc);
        }

        public static List<MomentValue> certainrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainrr, calc);
        }

        public static List<MomentValue> certainparamnprirrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainparamnprirrrr, calc);
        }

        public static List<MomentValue> certainparamnrirrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainparamnrirrr, calc);
        }

        public static List<MomentValue> certainparamprirr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainparamprirr, calc);
        }

        public static List<MomentValue> certainparamrir(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcertainparamrir, calc);
        }

        //5 - Фундаментальная
        
        public static List<MomentValue> apertureiir(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarapertureiir, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> aperturerrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraperturerrr, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> pointsvvu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> pointsbbu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> pointsiiu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> pointsddu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> pointsrru(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> pointsssu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> conditionindicatorbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarconditionindicatorbb, calc, FunFlags.SaveResult);
        }
        
        public static List<MomentValue> conditiontimevvb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarconditiontimeuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> conditiontimebbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarconditiontimeuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> conditiontimeiib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarconditiontimeuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> conditiontimeddb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarconditiontimeuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> conditiontimerrb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarconditiontimeuub, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> conditiontimessb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarconditiontimeuub, calc, FunFlags.SaveResult);
        }

        private static List<MomentValue> zone(List<MomentValue> par, string s, CalcParamRun calc)
        {
            if (s=="all") return par;
            var res = new List<MomentValue>();
            switch (s)
            {
                case "begin":
                    int n = 0;
                    while (n < par.Count && (par[n].Undef & ZoneFlag.Zone) == 0)
                    {
                        res.Add(par[n]);
                        n++;
                    }
                    if (n < par.Count) res.Add(par[n]);
                    break;

                case "first":
                    n = 0;
                    res.Add(par[0]);
                    while (n < par.Count && (par[n].Undef & ZoneFlag.Zone) != 0) ++n;
                    while (n < par.Count && (par[n].Undef & ZoneFlag.Zone) == 0)
                    {
                        if (n>0) res.Add(par[n]);
                        ++n;
                    }
                    if (n < par.Count) res.Add(par[n]);
                    break;

                case "end":
                    n = par.Count - 1;
                    while (n >= 1 && (par[n].Undef & ZoneFlag.Zone)== 0) --n;
                    if (n > 0)
                    {
                        var mv = par[0].Clone();
                        mv.Undef = 2;
                        mv.Time = calc.ThreadCalc.PeriodBegin;
                        res.Add(mv);
                    }
                    for (int i = n+1; i < par.Count; ++i)
                    {
                        res.Add(par[i]);
                    }
                    break;

                case "last":
                    n = par.Count - 1;
                    while (n >= 1 && (par[n].Undef & ZoneFlag.Zone) != 0) --n;
                    int nl = n+1;
                    while (n >= 1 && (par[n].Undef & ZoneFlag.Zone) == 0) --n;
                    if (n > 0)
                    {
                        var mv = par[0].Clone();
                        mv.Undef = 2;
                        mv.Time = calc.ThreadCalc.PeriodBegin;
                        res.Add(mv);
                    }
                    for (int i = n + 1; i < par.Count && (par[i].Undef & ZoneFlag.Zone) == 0; ++i)
                    {
                        res.Add(par[i]);
                    }
                    if (nl < par.Count - 1) res.Add(par[nl]);
                    break;

                case "max":
                    n = 0;
                    int undef = ZoneFlag.Zone;
                    double maxt = 0;
                    int begn = 0, mbegn = 0, mendn = 0;
                    while (n < par.Count)
                    {
                        if (undef != 0 && (par[n].Undef & ZoneFlag.Zone) == 0) begn = n;
                        if (undef == 0 && (par[n].Undef & ZoneFlag.Zone) != 0)
                        {
                            if (par[n].Time.Subtract(par[begn].Time).TotalSeconds > maxt)
                            {
                                mbegn = begn;
                                mendn = n;
                                maxt = par[n].Time.Subtract(par[begn].Time).TotalSeconds;
                            }
                        }
                        undef = (par[n].Undef & ZoneFlag.Zone);
                        n++;
                    }
                    if ((par[n - 1].Undef & ZoneFlag.Zone) == 0 && calc.ThreadCalc.PeriodEnd.Subtract(par[begn].Time).TotalSeconds > maxt)
                    {
                        mbegn = begn;
                        mendn = n-1;
                    }
                    if (mbegn > 0)
                    {
                        var mv = par[0].Clone();
                        mv.Undef = mv.Undef | ZoneFlag.Zone;
                        mv.Time = calc.ThreadCalc.PeriodBegin;
                        res.Add(mv);
                    }
                    for (int i = mbegn; i <= mendn;++i )
                    {
                        res.Add(par[i]);
                    }
                    break;
            }
            return res;
        }

        public static List<MomentValue> zonevvbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            List<MomentValue> res=GeneralComplicate(par, Scalarzoneuubs, calc, FunFlags.SaveResult | FunFlags.AddBegin);
            return zone(res, ((MomentString) par[2][0]).Mean, calc);
        }

        public static List<MomentValue> zonebbbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            List<MomentValue> res = GeneralComplicate(par, Scalarzoneuubs, calc, FunFlags.SaveResult | FunFlags.AddBegin);
            return zone(res, ((MomentString)par[2][0]).Mean, calc);
        }

        public static List<MomentValue> zoneiibs(List<MomentValue>[] par, CalcParamRun calc)
        {
            List<MomentValue> res = GeneralComplicate(par, Scalarzoneuubs, calc, FunFlags.SaveResult | FunFlags.AddBegin);
            return zone(res, ((MomentString)par[2][0]).Mean, calc);
        }

        public static List<MomentValue> zoneddbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            List<MomentValue> res = GeneralComplicate(par, Scalarzoneuubs, calc, FunFlags.SaveResult | FunFlags.AddBegin);
            return zone(res, ((MomentString)par[2][0]).Mean, calc);
        }

        public static List<MomentValue> zonerrbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            List<MomentValue> res = GeneralComplicate(par, Scalarzoneuubs, calc, FunFlags.SaveResult | FunFlags.AddBegin);
            return zone(res, ((MomentString)par[2][0]).Mean, calc);
        }

        public static List<MomentValue> zonessbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            List<MomentValue> res = GeneralComplicate(par, Scalarzoneuubs, calc, FunFlags.SaveResult | FunFlags.AddBegin);
            return zone(res, ((MomentString)par[2][0]).Mean, calc);
        }

        public static List<MomentValue> zonepointsvvbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[1] = par[0];
            parp[0] = zonevvbs(par, calc);
            List<MomentValue> res = pointsvvu(parp, calc);
            var rres = new List<MomentValue>();
            for (int i = 0; i < res.Count; ++i)
            {
                if ((res[i].Undef & ZoneFlag.Zone) == 0) rres.Add(res[i]);
            }
            return rres;
        }

        public static List<MomentValue> zonepointsbbbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[1] = par[0];
            parp[0] = zonebbbs(par, calc);
            List<MomentValue> res = pointsbbu(parp, calc);
            return res.Where(t => (t.Undef & ZoneFlag.Zone) == 0).ToList();
        }

        public static List<MomentValue> zonepointsiibs(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[1] = par[0];
            parp[0] = zoneiibs(par, calc);
            List<MomentValue> res = pointsiiu(parp, calc);
            return res.Where(t => (t.Undef & ZoneFlag.Zone) == 0).ToList();
        }

        public static List<MomentValue> zonepointsddbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[1] = par[0];
            parp[0] = zoneddbs(par, calc);
            List<MomentValue> res = pointsddu(parp, calc);
            return res.Where(t => (t.Undef & ZoneFlag.Zone) == 0).ToList();
        }

        public static List<MomentValue> zonepointsrrbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[1] = par[0];
            parp[0] = zonerrbs(par, calc);
            List<MomentValue> res = pointsrru(parp, calc);
            return res.Where(t => (t.Undef & ZoneFlag.Zone) == 0).ToList();
        }

        public static List<MomentValue> zonepointsssbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[1] = par[0];
            parp[0] = zonessbs(par, calc);
            List<MomentValue> res = pointsssu(parp, calc);
            return res.Where(t => (t.Undef & ZoneFlag.Zone) == 0).ToList();
        }

        public static List<MomentValue> thinout(List<MomentValue> par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            DateTime d = calc.ThreadCalc.PeriodBegin;
            int n = 0;
            while (d < calc.ThreadCalc.PeriodEnd)
            {
                res.Add(new MomentValue(d, 0, 0, 0));
                d = d.AddSeconds(((MomentReal)par[n]).Mean);
                while (n < par.Count-1 && d > par[n+1].Time) ++n;
            }
            return res;
        }

        public static List<MomentValue> thinoutvvr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[0] = par[0];
            parp[1] = thinout(par[1], calc);
            List<MomentValue> res = pointsvvu(parp, calc);
            return res;
        }

        public static List<MomentValue> thinoutbbr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[0] = par[0];
            parp[1] = thinout(par[1], calc);
            List<MomentValue> res = pointsbbu(parp, calc);
            return res;
        }

        public static List<MomentValue> thinoutiir(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[0] = par[0];
            parp[1] = thinout(par[1], calc);
            List<MomentValue> res = pointsiiu(parp, calc);
            return res;
        }

        public static List<MomentValue> thinoutddr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[0] = par[0];
            parp[1] = thinout(par[1], calc);
            List<MomentValue> res = pointsddu(parp, calc);
            return res;
        }

        public static List<MomentValue> thinoutrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[0] = par[0];
            parp[1] = thinout(par[1], calc);
            List<MomentValue> res = pointsrru(parp, calc);
            return res;
        }

        public static List<MomentValue> thinoutssr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var parp = new List<MomentValue>[2];
            parp[0] = par[0];
            parp[1] = thinout(par[1], calc);
            List<MomentValue> res = pointsssu(parp, calc);
            return res;
        }

        public static List<MomentValue> quantitypointsiu(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new MomentInteger(calc.ThreadCalc.PeriodEnd, par[0].Count, 0, 0, 0);
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> pointvvi(List<MomentValue>[] par, CalcParamRun calc)
        {
            int n = ((MomentInteger)par[1][0]).Mean - 1;
            //Отсутствует точка с заданным номером
            if (n < 0 || n >= par[0].Count)
                return new List<MomentValue> { new MomentValue(calc.ThreadCalc.PeriodBegin, par[1][0].Nd, par[1][0].Err | 1601 , 0) };
            var res = new MomentValue(par[0][n]);
            res.Nd |= par[1][0].Nd;
            res.Err |= par[1][0].Err;
            res.Undef |= par[1][0].Undef;
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> pointbbi(List<MomentValue>[] par, CalcParamRun calc)
        {
            int n = ((MomentInteger)par[1][0]).Mean - 1;
            //Отсутствует точка с заданным номером
            if (n < 0 || n >= par[0].Count)
                return new List<MomentValue> { new MomentBoolean(calc.ThreadCalc.PeriodBegin, false, par[1][0].Nd, par[1][0].Err | 1601, 0) };
            var res = new MomentBoolean(par[0][n]);
            res.Nd |= par[1][0].Nd;
            res.Err |= par[1][0].Err;
            res.Undef |= par[1][0].Undef;
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> pointiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            int n = ((MomentInteger)par[1][0]).Mean - 1;
            //Отсутствует точка с заданным номером
            if (n < 0 || n >= par[0].Count)
                return new List<MomentValue> { new MomentInteger(calc.ThreadCalc.PeriodBegin, 0, par[1][0].Nd, par[1][0].Err | 1601, 0) };
            var res = new MomentInteger(par[0][n]);
            res.Nd |= par[1][0].Nd;
            res.Err |= par[1][0].Err;
            res.Undef |= par[1][0].Undef;
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> pointddi(List<MomentValue>[] par, CalcParamRun calc)
        {
            int n = ((MomentInteger)par[1][0]).Mean - 1;
            //Отсутствует точка с заданным номером
            if (n < 0 || n >= par[0].Count)
                return new List<MomentValue> { new MomentTime(calc.ThreadCalc.PeriodBegin, calc.ThreadCalc.PeriodBegin, par[1][0].Nd, par[1][0].Err | 1601, 0) };
            var res = new MomentTime(par[0][n]);
            res.Nd |= par[1][0].Nd;
            res.Err |= par[1][0].Err;
            res.Undef |= par[1][0].Undef;
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> pointrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            int n = ((MomentInteger)par[1][0]).Mean - 1;
            //Отсутствует точка с заданным номером
            if (n < 0 || n >= par[0].Count)
                return new List<MomentValue> { new MomentReal(calc.ThreadCalc.PeriodBegin, 0, par[1][0].Nd, par[1][0].Err | 1601, 0) };
            var res = new MomentReal(par[0][n]);
            res.Nd |= par[1][0].Nd;
            res.Err |= par[1][0].Err;
            res.Undef |= par[1][0].Undef;
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> pointssi(List<MomentValue>[] par, CalcParamRun calc)
        {
            int n = ((MomentInteger)par[1][0]).Mean - 1;
            //Отсутствует точка с заданным номером
            if (n < 0 || n >= par[0].Count)
                return new List<MomentValue> { new MomentString(calc.ThreadCalc.PeriodBegin, "", par[1][0].Nd, par[1][0].Err | 1601, 0) };
            var res = new MomentString(par[0][n]);
            res.Nd |= par[1][0].Nd;
            res.Err |= par[1][0].Err;
            res.Undef |= par[1][0].Undef;
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> pointtimedui(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarpointtimedui, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> topointtimerui(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalartopointtimerui, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> pointnumberiu(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            for (int i=0; i<par[0].Count; ++i)
            {
                res.Add(new MomentInteger(par[0][i].Time, i, 0, 0, 0));
            }
            return res;
        }

        //Ищет номер точки в списке по заданному времени
        private static int SearchTime(List<MomentValue> par, DateTime d)
        {
            if (par.Count == 0 || d < par[0].Time) return -1;
            if (d > par[par.Count-1].Time) return par.Count-1;
            int beg = 0, en = par.Count - 1;
            while (en - beg > 1)
            {
                int c = (beg + en)/2;
                if (par[c].Time > d)
                {
                    en = c;
                }
                else
                {
                    beg = c;
                }
            }
            return beg;
        }

        public static List<MomentValue> valueattimevvd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            foreach (var v in par[1])
            {
                if (((MomentTime)v).Mean < calc.ThreadCalc.PeriodBegin || ((MomentTime)v).Mean > calc.ThreadCalc.PeriodEnd)
                {
                    //Попытка взять значение на время лежащее вне периода обработки
                    AddMomentValue(res, new MomentValue(v.Time, v.Nd, v.Err.Num | 1501, 0));
                }
                else
                {
                    int n = SearchTime(par[0], ((MomentTime)v).Mean);
                    if (n == -1) n = 0;
                    AddMomentValue(res, new MomentValue(v.Time, v.Nd | par[0][n].Nd, par[0][n].Err | v.Err, v.Undef | par[0][n].Undef));   
                }
            }
            return res;
        }

        public static List<MomentValue> valueattimebbd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            foreach (var v in par[1])
            {
                if (((MomentTime)v).Mean < calc.ThreadCalc.PeriodBegin || ((MomentTime)v).Mean > calc.ThreadCalc.PeriodEnd)
                {
                    //Попытка взять значение на время лежащее вне периода обработки
                    AddMomentValue(res, new MomentBoolean(v.Time, false, v.Nd, v.Err | 1501, 0));
                }
                else
                {
                    int n = SearchTime(par[0], ((MomentTime)v).Mean);
                    if (n == -1) n = 0;
                    AddMomentValue(res, new MomentBoolean(v.Time, ((MomentBoolean)par[0][n]).Mean, v.Nd | par[0][n].Nd, par[0][n].Err | v.Err, v.Undef | par[0][n].Undef));
                }
            }
            return res;
        }

        public static List<MomentValue> valueattimeiid(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            foreach (var v in par[1])
            {
                if (((MomentTime)v).Mean < calc.ThreadCalc.PeriodBegin || ((MomentTime)v).Mean > calc.ThreadCalc.PeriodEnd)
                {
                    //Попытка взять значение на время лежащее вне периода обработки
                    AddMomentValue(res, new MomentInteger(v.Time, 0, v.Nd, v.Err | 1501, 0));
                }
                else
                {
                    int n = SearchTime(par[0], ((MomentTime) v).Mean);
                    MomentValue mv = Interpolation(calc.CalcParam.Interpolation, par[0], n, ((MomentTime)v).Mean);
                    mv.Undef |= v.Undef;
                    mv.Nd |= v.Nd;
                    mv.Err |= v.Err;
                    AddMomentValue(res, mv);
                }
            }
            return res;
        }

        public static List<MomentValue> valueattimeddd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            foreach (var v in par[1])
            {
                if (((MomentTime)v).Mean < calc.ThreadCalc.PeriodBegin || ((MomentTime)v).Mean > calc.ThreadCalc.PeriodEnd)
                {
                    //Попытка взять значение на время лежащее вне периода обработки
                    res.Add(new MomentTime(v.Time, calc.ThreadCalc.PeriodBegin, v.Nd, v.Err | 1501, 0));
                }
                else
                {
                    int n = SearchTime(par[0], ((MomentTime) v).Mean);
                    MomentValue mv = Interpolation(calc.CalcParam.Interpolation, par[0], n, ((MomentTime)v).Mean);
                    mv.Undef |= v.Undef;
                    mv.Nd |= v.Nd;
                    mv.Err |= v.Err;
                    AddMomentValue(res, mv);
                }
            }
            return res;
        }

        public static List<MomentValue> valueattimerrd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            foreach (var v in par[1])
            {
                if (((MomentTime)v).Mean < calc.ThreadCalc.PeriodBegin || ((MomentTime)v).Mean > calc.ThreadCalc.PeriodEnd)
                {
                    //Попытка взять значение на время лежащее вне периода обработки
                    res.Add(new MomentReal(v.Time, 0, v.Nd, v.Err | 1501, 0));
                }
                else
                {
                    int n = SearchTime(par[0], ((MomentTime) v).Mean);
                    MomentValue mv = Interpolation(calc.CalcParam.Interpolation, par[0], n, ((MomentTime)v).Mean);
                    mv.Undef |= v.Undef;
                    mv.Nd |= v.Nd;
                    mv.Err |= v.Err;
                    AddMomentValue(res, mv);
                }
            }
            return res;
        }
        
        public static List<MomentValue> valueattimessd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            foreach (var v in par[1])
            {
                if (((MomentTime)v).Mean < calc.ThreadCalc.PeriodBegin || ((MomentTime)v).Mean > calc.ThreadCalc.PeriodEnd)
                {
                    //Попытка взять значение на время лежащее вне периода обработки
                    res.Add(new MomentString(v.Time, "", v.Nd, v.Err | 1501, 0));
                }
                else
                {
                    int n = SearchTime(par[0], ((MomentTime) v).Mean);
                    MomentValue mv = Interpolation(calc.CalcParam.Interpolation, par[0], n, ((MomentTime)v).Mean);
                    mv.Undef |= v.Undef;
                    mv.Nd |= v.Nd;
                    mv.Err |= v.Err;
                    AddMomentValue(res, mv);
                }
            }
            return res;
        }

        public static List<MomentValue> addpointsvvv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addpointsbbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addpointsiii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addpointsddd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addpointsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> addpointssss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalaraddpointsuuu, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> settimevvd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsettimeuud, calc, FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> settimebbd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsettimeuud, calc, FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> settimeiid(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsettimeuud, calc, FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> settimeddd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsettimeuud, calc, FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> settimerrd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsettimeuud, calc, FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> settimessd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsettimeuud, calc, FunFlags.SaveResult | FunFlags.Unsorted);
        }

        //6 - Статистическая

        public static List<MomentValue> firstvv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfirstuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> firstbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfirstuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> firstii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfirstuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> firstdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfirstuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> firstrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfirstuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> firstss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfirstuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> lastvv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarlastuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> lastbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarlastuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> lastii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarlastuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> lastdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarlastuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> lastrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarlastuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> lastss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarlastuu, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd);
        }

        public static List<MomentValue> minimumii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd);
        }

        public static List<MomentValue> minimumdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd);
        }

        public static List<MomentValue> minimumrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd);
        }

        public static List<MomentValue> minimumss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd);
        }

        public static List<MomentValue> maximumii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd | FunFlags.Max);
        }

        public static List<MomentValue> maximumdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd | FunFlags.Max);
        }

        public static List<MomentValue> maximumrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd | FunFlags.Max);
        }

        public static List<MomentValue> maximumss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarminimumuu, calc, FunFlags.SaveResult | FunFlags.AddEnd | FunFlags.Max);
        }

        public static List<MomentValue> integralrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarintegralrr, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.AllowEmptyList);
        }

        public static List<MomentValue> timerb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalartimerb, calc, FunFlags.SaveResult | FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.AllowEmptyList);
        }
        
        public static List<MomentValue> summii(List<MomentValue>[] par, CalcParamRun calc )
        {
            return GeneralComplicate(par, Scalarsummuu, calc, FunFlags.SaveResult | FunFlags.AllowEmptyList | FunFlags.Int);
        }

        public static List<MomentValue> summrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsummuu, calc, FunFlags.SaveResult | FunFlags.AllowEmptyList | FunFlags.Real);
        }

        public static List<MomentValue> summss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarsummuu, calc, FunFlags.SaveResult | FunFlags.AllowEmptyList | FunFlags.String);
        }

        public static List<MomentValue> quantityib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarquantityib, calc, FunFlags.SaveResult | FunFlags.AddEnd | FunFlags.AllowEmptyList);
        }
        
        public static List<MomentValue> averagerr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var integral = (MomentReal)integralrr(par, calc)[0];
            var part = new List<MomentValue>[1];
            part[0] = new List<MomentValue>();
            foreach (var mv in par[0])
            {
                part[0].Add(new MomentReal(mv.Time, 1, mv.Nd, mv.Err, mv.Undef));
            }
            var time = (MomentReal)integralrr(part, calc)[0];
            int undef = 0;
            double mean = 0;
            if (time.Mean == 0)
            {
                undef = 2;
            }
            else
            {
                mean = integral.Mean / time.Mean;
            }
            var res = new MomentReal(integral.Time, mean, integral.Nd | time.Nd, integral.Err | time.Err, undef);
            return new List<MomentValue> {res};
        }

        public static List<MomentValue> dispersionrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var p=new List<MomentValue>[2];
            p[0] = par[0];
            p[1] = averagerr(par, calc);
            var p2 = new List<MomentValue>[2];
            p2[0] = minusrrr(p, calc);
            p2[1] = p2[0];
            var pa = new List<MomentValue>[1];
            pa[0] = multiplyrrr(p2, calc);
            return averagerr(pa, calc);
        }

        public static List<MomentValue> deviationrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var p=new List<MomentValue>[1];
            p[0] = dispersionrr(par, calc);
            return sqrrr(p, calc);
        }

        public static List<MomentValue> covariationrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var px = new List<MomentValue>[2];
            px[0] = par[0];
            px[1] = averagerr(new[] {par[0]}, calc);
            var p = new List<MomentValue>[2];
            p[0] = minusrrr(px, calc);
            var py = new List<MomentValue>[2];
            py[0] = par[1];
            py[1] = averagerr(new[] { par[1] }, calc);
            p[1] = minusrrr(py, calc);
            p[0] = multiplyrrr(p, calc);
            return averagerr(new[] {p[0]}, calc);
        }

        public static List<MomentValue> correlationrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var pd = new List<MomentValue>[2];
            pd[0] = covariationrrr(par, calc);
            var px = new List<MomentValue>[1];
            px[0] = dispersionrr(new[] { par[0] }, calc);
            var p = new List<MomentValue>[2];
            p[0] = sqrrr(new [] { px[0] }, calc);
            var py = new List<MomentValue>[1];
            py[0] = dispersionrr(new [] { par[1] }, calc);
            p[1] = sqrrr(new [] { py[0] }, calc);
            pd[1] = multiplyrrr(p, calc);
            if (((MomentReal)pd[1][0]).Mean == 0 && pd[1][0].Undef == 0)
            {
                var r = new MomentReal(pd[0][0].Time, 1, pd[0][0].Nd, pd[0][0].Err, 0);
                return new List<MomentValue> {r};
            }
            return dividerrr(pd, calc);
        }

        //7 - Фильтр

        public static List<MomentValue> speedrrir(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarspeedrrir, calc, FunFlags.AddEnd | FunFlags.Filter | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterfirstbbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterfirstuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterfirstiirr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterfirstuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterfirstddrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterfirstuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterfirstrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterfirstuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterfirstssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterfirstuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.Unsorted);
        }
        
        public static List<MomentValue> filterminimumiirr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterminimumddrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterminimumrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterminimumssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> filtermaximumbbrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Max | FunFlags.Unsorted);
        }

        public static List<MomentValue> filtermaximumiirr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Max | FunFlags.Unsorted);
        }

        public static List<MomentValue> filtermaximumddrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Max | FunFlags.Unsorted);
        }

        public static List<MomentValue> filtermaximumrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Max | FunFlags.Unsorted);
        }

        public static List<MomentValue> filtermaximumssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterminimumuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Max | FunFlags.Unsorted);
        }

        public static List<MomentValue> filtertimebrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfiltertimebrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterintegralrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterintegralrrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> filteraveragerrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterintegralrrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Average | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterdispersionrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var par1 = new [] {par[0], par[0]};
            var par2 = new[] {multiplyrrr(par1, calc), par[1], par[2]};
            List<MomentValue> mv = GeneralComplicate(par, Scalarfilterintegralrrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Average | FunFlags.Dispersion | FunFlags.Unsorted);
            List<MomentValue> mv1 = GeneralComplicate(par2, Scalarfilterintegralrrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Average | FunFlags.Dispersion | FunFlags.Unsorted);
            var par3 = new[] {mv1, multiplyrrr(new[] {mv, mv}, calc)};
            return minusrrr(par3, calc);
        }

        public static List<MomentValue> filtercovariationrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            var par1 = new[] { par[0], par[1] };
            var par2 = new[] { multiplyrrr(par1, calc), par[2], par[3] };
            List<MomentValue> mv1 = GeneralComplicate(new[] { par[0], par[2], par[3] }, Scalarfilterintegralrrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Average | FunFlags.Covariation | FunFlags.Unsorted);
            List<MomentValue> mv2 = GeneralComplicate(new[] { par[1], par[2], par[3] }, Scalarfilterintegralrrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Average | FunFlags.Covariation | FunFlags.Unsorted);
            List<MomentValue> mv3 = GeneralComplicate(par2, Scalarfilterintegralrrrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Average | FunFlags.Covariation | FunFlags.Unsorted);
            var par3 = new[] { mv3, multiplyrrr(new[] { mv1, mv2 }, calc) };
            return minusrrr(par3, calc);
        }

        public static List<MomentValue> filtersummiirr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfiltersummuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Int | FunFlags.Unsorted);
        }

        public static List<MomentValue> filtersummrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfiltersummuurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Real | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterquantityibrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterquantityibrr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.SaveResult | FunFlags.Unsorted);
        }

        public static List<MomentValue> filterquantitypointsiurr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarfilterquantitypointsiurr, calc, FunFlags.AddBegin | FunFlags.AddEnd | FunFlags.Filter | FunFlags.Unsorted);
        }

        //8 - График

        public static List<MomentValue> diagramr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalardiagramr, calc, FunFlags.SaveResult);
        }

        //10 - Типы данных
        public static List<MomentValue> valuevv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarvaluevv, calc);
        }

        public static List<MomentValue> valuevb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarvaluevb, calc);
        }

        public static List<MomentValue> valuevi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarvaluevi, calc);
        }

        public static List<MomentValue> valuevd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarvaluevd, calc);
        }

        public static List<MomentValue> valuevr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarvaluevr, calc);
        }

        public static List<MomentValue> valuevs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarvaluevs, calc);
        }

        public static List<MomentValue> boolbv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarboolbv, calc);
        }

        public static List<MomentValue> boolbb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarboolbb, calc);
        }

        public static List<MomentValue> boolbi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarboolbi, calc);
        }

        public static List<MomentValue> boolbr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarboolbr, calc);
        }

        public static List<MomentValue> boolbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarboolbs, calc);
        }

        public static List<MomentValue> intiv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarintiv, calc);
        }

        public static List<MomentValue> intib(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarintib, calc);
        }

        public static List<MomentValue> intii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarintii, calc);
        }

        public static List<MomentValue> intir(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarintir, calc);
        }

        public static List<MomentValue> intis(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarintis, calc);
        }

        public static List<MomentValue> datedv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalardatedv, calc);
        }

        public static List<MomentValue> datedd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalardatedd, calc);
        }

        public static List<MomentValue> datedr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalardatedr, calc);
        }
        
        public static List<MomentValue> dateds(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalardateds, calc);
        }

        public static List<MomentValue> realrv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarrealrv, calc);
        }

        public static List<MomentValue> realrb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarrealrb, calc);
        }

        public static List<MomentValue> realri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarrealri, calc);
        }

        public static List<MomentValue> realrd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarrealrd, calc);
        }
        
        public static List<MomentValue> realrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarrealrr, calc);
        }

        public static List<MomentValue> realrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarrealrs, calc);
        }

        public static List<MomentValue> stringsv(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstringsv, calc);
        }

        public static List<MomentValue> stringsb(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstringsb, calc);
        }

        public static List<MomentValue> stringsi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstringsi, calc);
        }

        public static List<MomentValue> stringsd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstringsd, calc);
        }

        public static List<MomentValue> stringsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstringsr, calc);
        }

        public static List<MomentValue> stringss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstringss, calc);
        }

        public static List<MomentValue> isintbr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarisintbr, calc);
        }

        public static List<MomentValue> isintbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarisintbs, calc);
        }

        public static List<MomentValue> isrealbs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarisrealbs, calc);
        }

        public static List<MomentValue> isdatebs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarisdatebs, calc);
        }
        
        //12 - Строковая 

        //Табуляция
        public static List<MomentValue> tabs(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            var v = new MomentString(calc.ThreadCalc.PeriodBegin, "\t", 0, 0, 0);
            res.Add(v);
            return res;
        }

        //Перенос строки
        public static List<MomentValue> newlines(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new List<MomentValue>();
            var v = new MomentString(calc.ThreadCalc.PeriodBegin, "\n", 0, 0, 0);
            res.Add(v);
            return res;
        }

        public static List<MomentValue> strmidssi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrmidssi, calc);
        }

        public static List<MomentValue> strmidssii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrmidssii, calc);
        }

        public static List<MomentValue> strleftssi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrleftssi, calc);
        }

        public static List<MomentValue> strrightssi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrrightssi, calc);
        }

        public static List<MomentValue> strlenis(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrlenis, calc);
        }

        public static List<MomentValue> strfindissi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrfindissi, calc);
        }

        public static List<MomentValue> strfindlastissi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrfindlastissi, calc);
        }

        public static List<MomentValue> strinsertsssi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrinsertsssi, calc);
        }

        public static List<MomentValue> strremovessi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrremovessi, calc);
        }

        public static List<MomentValue> strremovessii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrremovessii, calc);
        }

        public static List<MomentValue> strtrimss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrtrimss, calc);
        }

        public static List<MomentValue> strltrimss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrltrimss, calc);
        }

        public static List<MomentValue> strrtrimss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrrtrimss, calc);
        }

        public static List<MomentValue> strlcasess(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrlcasess, calc);
        }

        public static List<MomentValue> strucasess(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrucasess, calc);
        }

        public static List<MomentValue> strreplacessss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrreplacessss, calc);
        }

        public static List<MomentValue> strreplaceregssss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarstrreplaceregssss, calc);
        }

        //13 - Дата

        public static List<MomentValue> nowd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new MomentTime(calc.ThreadCalc.PeriodBegin, DateTime.Now, 0, 0, 0);
            return new List<MomentValue> { res }; 
        }

        public static List<MomentValue> beginperiodd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new MomentTime(calc.ThreadCalc.PeriodBegin, calc.ThreadCalc.PeriodBegin, 0, 0, 0);
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> endperiodd(List<MomentValue>[] par, CalcParamRun calc)
        {
            var res = new MomentTime(calc.ThreadCalc.PeriodBegin, calc.ThreadCalc.PeriodEnd, 0, 0, 0);
            return new List<MomentValue> { res };
        }

        public static List<MomentValue> currenttimedu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarcurrenttimedu, calc);
        }

        public static List<MomentValue> timeadddsdr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalartimeadddsdr, calc);
        }

        public static List<MomentValue> timediffrsdd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalartimediffrsdd, calc);
        }

        public static List<MomentValue> timeserialdiiiiiir(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalartimeserialdiiiiiir, calc);
        }

        public static List<MomentValue> timepartisd(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalartimepartisd, calc);
        }
        
        // 30 - Системная

        public static List<MomentValue> updatevalueuuu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarupdatevalueuuu, calc, FunFlags.AllowEmptyList);
        }

        public static List<MomentValue> updatefilteruuu(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarupdatefilteruuu, calc, FunFlags.AllowEmptyList);
        }

        public static List<MomentValue> ifgotovbi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarifgotovbi, calc, FunFlags.SaveResult);
        }

        public static List<MomentValue> ifnotgotovbi(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralComplicate(par, Scalarifnotgotovbi, calc, FunFlags.SaveResult);
        }
        
        //20 - Свойства газов

        public static List<MomentValue> wspgcpidtrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgcpidtrri, calc);
        }

        public static List<MomentValue> wspgcpgstrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgcpgstrrs, calc);
        }

        public static List<MomentValue> wspgcpgstrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgcpgstrrsr, calc);
        }

        public static List<MomentValue> wspgcvidtrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgcvidtrri, calc);
        }

        public static List<MomentValue> wspgcvgstrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgcvgstrrs, calc);
        }

        public static List<MomentValue> wspgcvgstrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgcvgstrrsr, calc);
        }

        public static List<MomentValue> wspghidtrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspghidtrri, calc);
        }

        public static List<MomentValue> wspghgstrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspghgstrrs, calc);
        }

        public static List<MomentValue> wspghgstrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspghgstrrsr, calc);
        }

        public static List<MomentValue> wspguidtrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspguidtrri, calc);
        }

        public static List<MomentValue> wspgugstrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgugstrrs, calc);
        }

        public static List<MomentValue> wspgugstrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgugstrrsr, calc);
        }

        public static List<MomentValue> wspgvidptrrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgvidptrrri, calc);
        }

        public static List<MomentValue> wspgvgsptrrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgvgsptrrrs, calc);
        }

        public static List<MomentValue> wspgvgsptrrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgvgsptrrrsr, calc);
        }

        public static List<MomentValue> wspggcidri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspggcidri, calc);
        }

        public static List<MomentValue> wspggcgsrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspggcgsrs, calc);
        }

        public static List<MomentValue> wspggcgsrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspggcgsrsr, calc);
        }

        public static List<MomentValue> wspgmmidri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgmmidri, calc);
        }

        public static List<MomentValue> wspgmmgsrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgmmgsrs, calc);
        }

        public static List<MomentValue> wspgmmgsrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgmmgsrsr, calc);
        }

        public static List<MomentValue> wspgmfididrii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgmfididrii, calc);
        }

        public static List<MomentValue> wspgmfgsgsrss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgmfgsgsrss, calc);
        }

        public static List<MomentValue> wspgvfididrii(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgvfididrii, calc);
        }

        public static List<MomentValue> wspgvfgsgsrss(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgvfgsgsrss, calc);
        }

        public static List<MomentValue> wspgtidhrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgtidhrri, calc);
        }

        public static List<MomentValue> wspgtgshrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgtgshrrs, calc);
        }

        public static List<MomentValue> wspgtgshrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgtgshrrsr, calc);
        }

        public static List<MomentValue> wspgpidtsrrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgpidtsrrri, calc);
        }

        public static List<MomentValue> wspgpgstsrrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgpgstsrrrs, calc);
        }

        public static List<MomentValue> wspgpgstsrrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgpgstsrrrsr, calc);
        }

        public static List<MomentValue> wspgsidptrrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgsidptrrri, calc);
        }

        public static List<MomentValue> wspgsgsptrrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgsgsptrrrs, calc);
        }

        public static List<MomentValue> wspgsgsptrrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgsgsptrrrsr, calc);
        }

        public static List<MomentValue> wspgtidpsrrri(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgtidpsrrri, calc);
        }

        public static List<MomentValue> wspgtgspsrrrs(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgtgspsrrrs, calc);
        }

        public static List<MomentValue> wspgtgspsrrrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspgtgspsrrrsr, calc);
        }

        // 21 - ВодаПар

        public static List<MomentValue> wspsurftenrsr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspsurften, calc);
        }

        public static List<MomentValue> wspvrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspv, calc);
        }

        public static List<MomentValue> wspvrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspv, calc);
        }

        public static List<MomentValue> wspvrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspv, calc);
        }

        public static List<MomentValue> wspvrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspv, calc);
        }

        public static List<MomentValue> wspvrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspv, calc);
        }

        public static List<MomentValue> wspvrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspv, calc);
        }

        public static List<MomentValue> wspursrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspu, calc);
        }

        public static List<MomentValue> wspursrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspu, calc);
        }

        public static List<MomentValue> wspursrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspu, calc);
        }

        public static List<MomentValue> wspursrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspu, calc);
        }

        public static List<MomentValue> wspurssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspu, calc);
        }

        public static List<MomentValue> wspurssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspu, calc);
        }

        public static List<MomentValue> wsphrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsph, calc);
        }

        public static List<MomentValue> wsphrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsph, calc);
        }

        public static List<MomentValue> wsphrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsph, calc);
        }

        public static List<MomentValue> wsphrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsph, calc);
        }

        public static List<MomentValue> wsphrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsph, calc);
        }

        public static List<MomentValue> wsphrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsph, calc);
        }

        public static List<MomentValue> wspsrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsps, calc);
        }

        public static List<MomentValue> wspsrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsps, calc);
        }

        public static List<MomentValue> wspsrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsps, calc);
        }

        public static List<MomentValue> wspsrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsps, calc);
        }

        public static List<MomentValue> wspsrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsps, calc);
        }

        public static List<MomentValue> wspsrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwsps, calc);
        }

        public static List<MomentValue> wspcvrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcv, calc);
        }

        public static List<MomentValue> wspcvrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcv, calc);
        }

        public static List<MomentValue> wspcvrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcv, calc);
        }

        public static List<MomentValue> wspcvrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcv, calc);
        }

        public static List<MomentValue> wspcvrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcv, calc);
        }

        public static List<MomentValue> wspcvrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcv, calc);
        }

        public static List<MomentValue> wspcprsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcp, calc);
        }

        public static List<MomentValue> wspcprsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcp, calc);
        }

        public static List<MomentValue> wspcprsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcp, calc);
        }

        public static List<MomentValue> wspcprsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcp, calc);
        }

        public static List<MomentValue> wspcprssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcp, calc);
        }

        public static List<MomentValue> wspcprssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspcp, calc);
        }

        public static List<MomentValue> wspwrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspw, calc);
        }

        public static List<MomentValue> wspwrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspw, calc);
        }

        public static List<MomentValue> wspwrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspw, calc);
        }

        public static List<MomentValue> wspwrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspw, calc);
        }

        public static List<MomentValue> wspwrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspw, calc);
        }

        public static List<MomentValue> wspwrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspw, calc);
        }

        public static List<MomentValue> wspjoulethompsonrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspjoulethompson, calc);
        }

        public static List<MomentValue> wspjoulethompsonrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspjoulethompson, calc);
        }

        public static List<MomentValue> wspjoulethompsonrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspjoulethompson, calc);
        }

        public static List<MomentValue> wspjoulethompsonrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspjoulethompson, calc);
        }

        public static List<MomentValue> wspjoulethompsonrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspjoulethompson, calc);
        }

        public static List<MomentValue> wspjoulethompsonrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspjoulethompson, calc);
        }

        public static List<MomentValue> wspthermcondrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspthermcond, calc);
        }

        public static List<MomentValue> wspthermcondrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspthermcond, calc);
        }

        public static List<MomentValue> wspthermcondrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspthermcond, calc);
        }

        public static List<MomentValue> wspthermcondrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspthermcond, calc);
        }

        public static List<MomentValue> wspthermcondrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspthermcond, calc);
        }

        public static List<MomentValue> wspthermcondrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspthermcond, calc);
        }

        public static List<MomentValue> wspdynvisrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspdynvis, calc);
        }

        public static List<MomentValue> wspdynvisrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspdynvis, calc);
        }

        public static List<MomentValue> wspdynvisrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspdynvis, calc);
        }

        public static List<MomentValue> wspdynvisrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspdynvis, calc);
        }

        public static List<MomentValue> wspdynvisrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspdynvis, calc);
        }

        public static List<MomentValue> wspdynvisrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspdynvis, calc);
        }

        public static List<MomentValue> wspprandtlersrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspprandtle, calc);
        }

        public static List<MomentValue> wspprandtlersrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspprandtle, calc);
        }

        public static List<MomentValue> wspprandtlersrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspprandtle, calc);
        }

        public static List<MomentValue> wspprandtlersrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspprandtle, calc);
        }

        public static List<MomentValue> wspprandtlerssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspprandtle, calc);
        }

        public static List<MomentValue> wspprandtlerssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspprandtle, calc);
        }

        public static List<MomentValue> wspkinvisrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspkinvis, calc);
        }

        public static List<MomentValue> wspkinvisrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspkinvis, calc);
        }

        public static List<MomentValue> wspkinvisrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspkinvis, calc);
        }

        public static List<MomentValue> wspkinvisrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspkinvis, calc);
        }

        public static List<MomentValue> wspkinvisrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspkinvis, calc);
        }

        public static List<MomentValue> wspkinvisrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspkinvis, calc);
        }

        public static List<MomentValue> wspkrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspk, calc);
        }

        public static List<MomentValue> wspkrsrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspk, calc);
        }

        public static List<MomentValue> wspkrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspk, calc);
        }

        public static List<MomentValue> wspkrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspk, calc);
        }

        public static List<MomentValue> wspkrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspk, calc);
        }

        public static List<MomentValue> wspkrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspk, calc);
        }

        public static List<MomentValue> wsptrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspt, calc);
        }

        public static List<MomentValue> wsptrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspt, calc);
        }

        public static List<MomentValue> wsptrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspt, calc);
        }

        public static List<MomentValue> wsptrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspt, calc);
        }

        public static List<MomentValue> wsptrssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspt, calc);
        }

        public static List<MomentValue> wspprsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspp, calc);
        }

        public static List<MomentValue> wspprssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspp, calc);
        }

        public static List<MomentValue> wspprssrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspp, calc);
        }

        public static List<MomentValue> wspxrsrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspx, calc);
        }

        public static List<MomentValue> wspxrsrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspx, calc);
        }

        public static List<MomentValue> wspxrsrrrrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspx, calc);
        }

        public static List<MomentValue> wspxrssrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspx, calc);
        }

        public static List<MomentValue> wspwaterstateareaisrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspwaterstatearea, calc);
        }

        public static List<MomentValue> wspphasestateisrr(List<MomentValue>[] par, CalcParamRun calc)
        {
            return GeneralSimple(par, Scalarwspphasestate, calc);
        }

        //****************************************
        //Скалярные функции

        //Время расчета всегда лежит в par[0]

        //Вычисление максимальной недостоверности
        private static int MaxNd(MomentValue[] par)
        {
            int m = 0;
            for (int i = par.Length - 1; i >= 0; --i)
            {
                 m |= par[i].Nd;
            }
            return m;
        }

        //Вычисление максимальной неопределенности
        private static int MaxUndef(MomentValue[] par)
        {
            int m = 0;
            for (int i = par.Length - 1; i >= 0; --i)
            {
                m |= par[i].Undef;
            }
            return m;
        }

        //Вычисление максимальной неопределенности
        private static ErrNum MaxErrNum(MomentValue[] par)
        {
            var m = new ErrNum(0);
            return par.Aggregate(m, (current, t) => current | t.Err);
        }

        //1 - Операция

        private static MomentInteger Scalarplusiii(MomentValue[] par)
        {
            int Mean = ((MomentInteger)par[0]).Mean + ((MomentInteger)par[1]).Mean;
            return new MomentInteger(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentTime Scalarplusddr(MomentValue[] par)
        {
            DateTime Mean = ((MomentTime)par[0]).Mean.AddSeconds(((MomentReal)par[1]).Mean);
            return new MomentTime(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentTime Scalarplusdrd(MomentValue[] par)
        {
            DateTime Mean = ((MomentTime)par[1]).Mean.AddSeconds(((MomentReal)par[0]).Mean);
            return new MomentTime(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentReal Scalarplusrrr(MomentValue[] par)
        {
            double Mean = ((MomentReal)par[0]).Mean + ((MomentReal)par[1]).Mean;
            return new MomentReal(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentString Scalarplussss(MomentValue[] par)
        {
            string Mean = ((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean;
            return new MomentString(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentReal Scalardividerrr(MomentValue[] par)
        {
            double a = ((MomentReal)par[0]).Mean, b = ((MomentReal)par[1]).Mean;
            double Mean = 0;
            ErrNum err = MaxErrNum(par);
            int nd = MaxNd(par);
            if (a == 0)
            {
                Mean = 0;
                err = par[0].Err;
                nd = par[0].Nd;
            }
            else
            {
                if (b == 0) err |= 1101;//Деление на 0
                else Mean = a / b; 
            }
            return new MomentReal(Mean, nd, err);
        }

        private static MomentInteger Scalardiviii(MomentValue[] par)
        {
            int a = ((MomentInteger)par[0]).Mean, b = ((MomentInteger)par[1]).Mean;
            int Mean = 0;
            ErrNum err = MaxErrNum(par);
            if (b == 0)
            {
                err |= 1102;//Деление на 0 (div)
            }
            else
            {
                Mean = a / b;
                err = Mean == 0 ? par[0].Err : err;
            }
            int nd = Mean == 0 ? par[0].Nd : MaxNd(par);
            return new MomentInteger(Mean, nd, err);
        }

        private static MomentInteger Scalarmodiii(MomentValue[] par)
        {
            int a = ((MomentInteger)par[0]).Mean, b = ((MomentInteger)par[1]).Mean;
            int Mean = 0;
            ErrNum err = MaxErrNum(par);
            if (b == 0)
            {
                err |= 1103;//Деление на 0 (mod)
            }
            else
            {
                Mean = a % b;
                err = Mean == 0 ? par[0].Err : err;
            }
            int nd = Mean == 0 ? par[0].Nd : MaxNd(par);
            return new MomentInteger(Mean, nd, err);
        }

        private static MomentInteger Scalarminusii(MomentValue[] par)
        {
            int Mean = -((MomentInteger)par[0]).Mean;
            return new MomentInteger(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentReal Scalarminusrr(MomentValue[] par)
        {
            double Mean = - ((MomentReal)par[0]).Mean;
            return new MomentReal(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarminusiii(MomentValue[] par)
        {
            int Mean = ((MomentInteger)par[0]).Mean - ((MomentInteger)par[1]).Mean;
            return new MomentInteger(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentTime Scalarminusddr(MomentValue[] par)
        {
            DateTime Mean = ((MomentTime)par[0]).Mean.AddSeconds(-((MomentReal)par[1]).Mean);
            return new MomentTime(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentReal Scalarminusrdd(MomentValue[] par)
        {
            double Mean = ((MomentTime)par[0]).Mean.Subtract(((MomentTime)par[1]).Mean).TotalSeconds;
            return new MomentReal(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentReal Scalarminusrrr(MomentValue[] par)
        {
            double Mean = ((MomentReal)par[0]).Mean - ((MomentReal)par[1]).Mean;
            return new MomentReal(Mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarunminusii(MomentValue[] par)
        {
            int Mean = -((MomentInteger)par[0]).Mean;
            return new MomentInteger(Mean, par[0].Nd, MaxErrNum(par));
        }

        private static MomentReal Scalarunminusrr(MomentValue[] par)
        {
            double Mean = -((MomentReal)par[0]).Mean;
            return new MomentReal(Mean, par[0].Nd, MaxErrNum(par));
        }

        private static MomentInteger Scalarmultiplyiii(MomentValue[] par)
        {
            int Mean = ((MomentInteger)par[0]).Mean * ((MomentInteger)par[1]).Mean;
            int nd = MaxNd(par); 
            ErrNum err = MaxErrNum(par);
            if (Mean == 0)
            {
                if (((MomentInteger)par[1]).Mean == 0)
                {
                    nd = par[1].Nd;
                    err = par[1].Err;
                }
                if (((MomentInteger)par[0]).Mean == 0)
                {
                    nd = par[0].Nd;
                    err.Num = (err.Num == 0) ? 0 : par[0].Err.Num;
                }
            }
            var res = new MomentInteger(Mean, nd, err);
            return res;
        }

        private static MomentReal Scalarmultiplyrrr(MomentValue[] par)
        {
            double Mean = ((MomentReal)par[0]).Mean * ((MomentReal)par[1]).Mean;
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            if (Mean == 0)
            {
                if (((MomentReal)par[1]).Mean == 0)
                {
                    nd = par[1].Nd;
                    err = par[1].Err;
                }
                if (((MomentReal)par[0]).Mean == 0)
                {
                    nd = par[0].Nd;
                    err.Num = (err.Num == 0) ? 0 : par[0].Err.Num;
                }
            }
            var res = new MomentReal(Mean, nd, err);
            return res;
        }

        private static MomentReal Scalarpowerrrr(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err=MaxErrNum(par);
            double m0 = ((MomentReal)par[0]).Mean;
            double m1 = ((MomentReal)par[1]).Mean;
            if (m0 == 0)
            {
                err = par[0].Err;
                nd = par[0].Nd;
            }
            double m = 1;
            if (m0 < 0)
            {
                if (Convert.ToInt32(m1) == m1)
                {
                    if (m1 > 0)
                    {
                        for (int i = 1; i <= m1; ++i)
                        {
                            m = m * m0;
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= -m1; ++i)
                        {
                            m = m / m0;
                        }
                    }
                }
                else
                {
                    err |= 1104;//Неправильное возведение отрицательного числа в степень
                }
            }
            else
            {
                if (m0 == 0)
                {
                    if (m1 == 0)
                    {
                        err |= 1105;//Возведение ноля в нолевую степень
                    }
                    else
                    {
                        m = 0;
                    }
                }
                else
                {
                    if (m1 == 0)
                    {
                        m = 1;
                    }
                    else
                    {
                        if (Convert.ToInt32(m1) == m1)
                        {
                            if (m1 > 0)
                            {
                                for (int i = 1; i <= Convert.ToInt32(m1); ++i)
                                {
                                    m = m * m0;
                                }
                            }
                            else
                            {
                                for (int i = 1; i <= -Convert.ToInt32(m1); ++i)
                                {
                                    m = m / m0;
                                }
                            }
                        }
                        else
                        {
                            m = Math.Pow(m0, m1);
                        }
                    }
                }
            }
            var res = new MomentReal(m, nd, err);
            return res;
        }

        //Равно
        private static MomentBoolean Scalarequalbuu(MomentValue[] par)
        {
            return new MomentBoolean(par[0] == par[1], MaxNd(par), MaxErrNum(par));
        }
        
        private static MomentBoolean Scalarnotequalbuu(MomentValue[] par)
        {
            return new MomentBoolean(par[0] != par[1], MaxNd(par), MaxErrNum(par));
        }

        private static MomentBoolean Scalarlessequalbuu(MomentValue[] par)
        {
            return new MomentBoolean(par[0] <= par[1], MaxNd(par), MaxErrNum(par));
        }

        private static MomentBoolean Scalarlessbuu(MomentValue[] par)
        {
            return new MomentBoolean(par[0] < par[1], MaxNd(par), MaxErrNum(par));
        }

        private static MomentBoolean Scalargreaterequalbuu(MomentValue[] par)
        {
            return new MomentBoolean(par[0] >= par[1], MaxNd(par), MaxErrNum(par));
        }

        private static MomentBoolean Scalargreaterbuu(MomentValue[] par)
        {
            return new MomentBoolean(par[0] > par[1], MaxNd(par), MaxErrNum(par));
        }

        private static MomentBoolean Scalarnotbb(MomentValue[] par)
        {
            bool Mean = !((MomentBoolean)par[0]).Mean;
            return new MomentBoolean(Mean, par[0].Nd, par[0].Err);
        }

        private static MomentBoolean Scalarorbbb(MomentValue[] par)
        {
            bool m0 = ((MomentBoolean)par[0]).Mean;
            bool m1 = ((MomentBoolean)par[1]).Mean;
            if (!m0 && !m1) return new MomentBoolean(false, MaxNd(par), MaxErrNum(par));
            if (m0 && !m1) return new MomentBoolean(true, (par[0].Nd == 0) ? 0 : MaxNd(par), par[0].Err);
            if (!m0 && m1) return new MomentBoolean(true, (par[1].Nd == 0) ? 0 : MaxNd(par), par[1].Err);
            return new MomentBoolean(true, par[0].Nd & par[1].Nd, (par[1].Err.Num == 0) ? 0 : par[0].Err.Num);
        }

        private static MomentInteger Scalaroriii(MomentValue[] par)
        {
            int m0 = ((MomentInteger)par[0]).Mean;
            int m1 = ((MomentInteger)par[1]).Mean;
            return new MomentInteger(m0 | m1, par[0].Nd & par[1].Nd, par[1].Err | par[0].Err);
        }

        private static MomentBoolean Scalarandbbb(MomentValue[] par)
        {
            bool m0 = ((MomentBoolean)par[0]).Mean;
            bool m1 = ((MomentBoolean)par[1]).Mean;
            if (m0 && m1) return new MomentBoolean(true, MaxNd(par), MaxErrNum(par));
            if (!m0 && m1) return new MomentBoolean(false, (par[0].Nd == 0) ? 0 : MaxNd(par), par[0].Err.Num);
            if (m0 && !m1) return new MomentBoolean(false, (par[1].Nd == 0) ? 0 : MaxNd(par), par[1].Err.Num);
            return new MomentBoolean(false, par[0].Nd & par[1].Nd, (par[1].Err.Num == 0) ? 0 : par[0].Err.Num);
        }

        private static MomentInteger Scalarandiii(MomentValue[] par)
        {
            int m0 = ((MomentInteger)par[0]).Mean;
            int m1 = ((MomentInteger)par[1]).Mean;
            return new MomentInteger(m0 & m1, par[0].Nd & par[1].Nd, par[1].Err | par[0].Err);
        }

        private static MomentBoolean Scalarxorbbb(MomentValue[] par)
        {
            bool mean = ((MomentBoolean)par[0]).Mean ^ ((MomentBoolean)par[1]).Mean;
            return new MomentBoolean(mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarxoriii(MomentValue[] par)
        {
            int m0 = ((MomentInteger)par[0]).Mean;
            int m1 = ((MomentInteger)par[1]).Mean;
            return new MomentInteger(m0 ^ m1, par[0].Nd & par[1].Nd, par[1].Err | par[0].Err);
        }

        private static MomentBoolean Scalarlikebss(MomentValue[] par)
        {
            string m1 = ((MomentString) par[1]).Mean.Replace("*", @"[\S|\s]*").Replace("?", @"[\S|\s]");
            string m0 = ((MomentString) par[0]).Mean;
            bool mean = false;
            var r = new Regex(m1);
            if (r.IsMatch(m0))
            {
                Match m = r.Match(m0);
                if (m0.Length==m.Value.Length) mean = true;
            }
            return new MomentBoolean(mean, MaxNd(par), MaxErrNum(par));
        }

        //02 - Логическая

        private static MomentBoolean Scalarbitbii(MomentValue[] par)
        {
            int mean = (((MomentInteger)par[0]).Mean >> ((MomentInteger)par[1]).Mean) & 1;
            return new MomentBoolean(mean == 1, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarsriii(MomentValue[] par)
        {
            int mean = ((MomentInteger)par[0]).Mean >> ((MomentInteger)par[1]).Mean;
            return new MomentInteger(mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarsliii(MomentValue[] par)
        {
            int mean = ((MomentInteger)par[0]).Mean << ((MomentInteger)par[1]).Mean;
            return new MomentInteger(mean, MaxNd(par), MaxErrNum(par));
        }

        // 03 - Математическая

        private static MomentInteger Scalarrandomiii(MomentValue[] par)
        {
            var r=new Random();
            int mean = r.Next(((MomentInteger)par[0]).Mean, ((MomentInteger)par[1]).Mean);
            return new MomentInteger(mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarabsii(MomentValue[] par)
        {
            int mean = Math.Abs(((MomentInteger)par[0]).Mean);
            return new MomentInteger(mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarabsrr(MomentValue[] par)
        {
            double mean = Math.Abs(((MomentReal)par[0]).Mean);
            return new MomentReal(mean, par[0].Nd, par[0].Err);
        }

        private static MomentInteger Scalarsignii(MomentValue[] par)
        {
            int mean = Math.Sign(((MomentInteger)par[0]).Mean);
            return new MomentInteger(mean, par[0].Nd, par[0].Err);
        }

        private static MomentInteger Scalarsignir(MomentValue[] par)
        {
            int mean = Math.Sign(((MomentReal)par[0]).Mean);
            return new MomentInteger(mean, par[0].Nd, par[0].Err);
        }

        private static MomentInteger Scalarroundir(MomentValue[] par)
        {
            int mean = Convert.ToInt32(Math.Round(((MomentReal)par[0]).Mean));
            return new MomentInteger(mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarroundrri(MomentValue[] par)
        {
            double mean = Math.Round(((MomentReal)par[0]).Mean, ((MomentInteger)par[1]).Mean);
            return new MomentReal( mean, MaxNd(par), MaxErrNum(par));
        }

        private static MomentValue Scalarminuu(MomentValue[] par)
        {
            MomentValue mv = par[0];
            int n = par.Length;
            for (int i = 1; i < n; ++i)
            {
                if (par[i] < mv) mv = par[i];
            }
            return mv.Clone();
        }

        private static MomentValue Scalarmaxuu(MomentValue[] par)
        {
            MomentValue mv = par[0];
            int n = par.Length;
            for (int i = 1; i < n; ++i)
            {
                if (par[i] > mv) mv = par[i];
            }
            return mv.Clone();
        }

        private static MomentReal Scalarsqrrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            if (mean < 0)
            {
                err |= 1306;//Извлечение корня из отрицательного числа
            }
            else
            {
                mean = Math.Sqrt(mean);
            }
            return new MomentReal( mean, par[0].Nd, err);
        }

        private static MomentReal Scalarcosrr(MomentValue[] par)
        {
            double mean = Math.Cos(((MomentReal)par[0]).Mean);
            return new MomentReal( mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarsinrr(MomentValue[] par)
        {
            double mean = Math.Sin(((MomentReal)par[0]).Mean);
            return new MomentReal(mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalartanrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            try
            {
                mean = Math.Tan(mean);
                if (Double.IsNaN(mean))
                {
                    err |= 1307;
                    mean = 0;
                }
            }
            catch
            {
                err |= 1307;//Тангенс от недопустимого аргумента
            }
            return new MomentReal( mean, par[0].Nd, err);
        }

        private static MomentReal Scalarctanrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            if (Math.Sin(mean) == 0)
            {
                err |= 1308;//Котангенс от недопустимого аргумента
                mean = 0;
            }
            else mean = Math.Cos(mean) / Math.Sin(mean);
            
            return new MomentReal(mean, par[0].Nd, err);
        }

        private static MomentReal Scalararccosrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            if (mean < -1 || mean > 1)
            {
                err |= 1309;//Арккосинус от недопустимого аргумента
            }
            else
            {
                mean = Math.Acos(mean);    
            }
            return new MomentReal(mean, par[0].Nd, err);
        }

        private static MomentReal Scalararcsinrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            if (mean < -1 || mean > 1)
            {
                err |= 1310;//Арксинус от недопустимого аргумента
                mean = 0;
            }
            else
            {
                mean = Math.Asin(mean);
            }
            return new MomentReal( mean, par[0].Nd, err);
        }

        private static MomentReal Scalararctanrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            return new MomentReal( Math.Atan(mean), par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarshrr(MomentValue[] par)
        {
            double mean = Math.Sinh(((MomentReal)par[0]).Mean);
            return new MomentReal( mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarchrr(MomentValue[] par)
        {
            double mean = Math.Cosh(((MomentReal)par[0]).Mean);
            return new MomentReal( mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarthrr(MomentValue[] par)
        {
            double mean = Math.Tanh(((MomentReal)par[0]).Mean);
            return new MomentReal( mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalararcshrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            try
            {
                mean = Math.Log(mean + Math.Sqrt(mean*mean + 1));
                if (Double.IsNaN(mean))
                {
                    err |= 1314;
                    mean = 0;
                }
            }
            catch
            {
                err |= 1314;//Гиперболический арксинус от недопустимого аргумента
            }
            return new MomentReal( mean, par[0].Nd, err);
        }

        private static MomentReal Scalararcchrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            try
            {
                mean = Math.Log(mean + Math.Sqrt(mean * mean - 1));
                if (Double.IsNaN(mean))
                {
                    err |= 1315;
                    mean = 0;
                }
            }
            catch
            {
                err |= 1315;//Гиперболический арккосинус от недопустимого аргумента
                mean = 0;
            }
            return new MomentReal(mean, par[0].Nd, err);
        }

        private static MomentReal Scalararcthrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            try
            {
                if (mean == 1 || (1 + mean) / (1 - mean) <= 0)
                {
                    err |= 1316;
                    mean = 0;
                }
                else mean = Math.Log((1+mean) / (1-mean))/2;
            }
            catch
            {
                err |= 1316;//Гиперболический арктангенс от недопустимого аргумента
                mean = 0;
            }
            return new MomentReal( mean, par[0].Nd, err);
        }

        private static MomentReal Scalarexprr(MomentValue[] par)
        {
            double mean = Math.Exp(((MomentReal)par[0]).Mean);
            return new MomentReal( mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarlnrr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            try
            {
                if (mean <= 0) err |= 1317;
                else  mean = Math.Log(mean);
            }
            catch
            {
                err |= 1317;//Взятие натурального логарифма от неположительного числа
            }
            return new MomentReal(mean, par[0].Nd, err);
        }

        private static MomentReal Scalarlog10rr(MomentValue[] par)
        {
            double mean = ((MomentReal)par[0]).Mean;
            ErrNum err = par[0].Err;
            try
            {
                if (mean <= 0) err |= 1318;
                else mean = Math.Log10(mean);
            }
            catch
            {
                err |= 1318;//Взятие десятичного логарифма от неположительного числа
            }
            return new MomentReal( mean, par[0].Nd, err);
        }

        private static MomentReal Scalarlogrrr(MomentValue[] par)
        {
            double m0 = ((MomentReal)par[0]).Mean;
            double m1 = ((MomentReal)par[1]).Mean;
            double mean = 0;
            int nd = MaxNd(par); 
            ErrNum err = MaxErrNum(par);
            if (m0 == 1)
            {
                err = par[0].Err;
                nd = par[0].Nd;
            }
            else
            {
                if (m0 <= 0 || m1 <= 0)
                {
                    err |= 1319;//Взятие логарифма от неположительного числа или по неположительному основанию
                }
                else
                {
                    if (m1 == 1)
                    {
                        err |= 1320;//Взятие логарифма по основанию 1
                    }
                    else
                    {
                        mean = Math.Log(m0, m1);
                    }
                }
            }
            return new MomentReal( mean, nd, err);
        }

        //04 - Неопредленность

        private static MomentInteger Scalaruncertainiu(MomentValue[] par)
        {
            return new MomentInteger(par[0].Nd, 0, par[0].Err);
        }

        private static MomentInteger Scalarundefiu(MomentValue[] par)
        {
            return new MomentInteger(par[0].Undef, 0, 0);
        }

        private static MomentInteger Scalarerroriu(MomentValue[] par)
        {
            return new MomentInteger(par[0].Err.Num, par[0].Nd, 0);
        }

        private static MomentValue Scalarmakecertainuuib(MomentValue[] par)
        {
            MomentValue res = par[0].Clone();
            res.Err = MaxErrNum(par);
            int nd = ((MomentInteger) par[1]).Mean;
            res.Nd = ((MomentBoolean)par[2]).Mean ? nd : (res.Nd | nd);
            return res;
        }

        private static MomentValue Scalarmakeerroruuib(MomentValue[] par)
        {
            MomentValue res = par[0].Clone();
            res.Nd = MaxNd(par);
            int err  = ((MomentInteger)par[1]).Mean;
            res.Err = new ErrNum(((MomentBoolean)par[2]).Mean ? err : (res.Err | err).Num);
            return res;
        }

        private static int Scalarmakeundefuuib(FunParams fp, List<MomentValue> res)
        {
            MomentValue mv = fp.Par[0].Clone();
            mv.Nd = MaxNd(fp.Par);
            mv.Err = MaxErrNum(fp.Par);
            int undef = ((MomentInteger)fp.Par[1]).Mean;
            if (undef != 0 && undef != 2 && undef != 4 && undef != 6)
            {
                //Недопустимое значение параметра функции Неопределенность ЗаписьНеопр (MakeUndef)
                mv.Err |= 1407;
            }
            else
            {
                mv.Undef = ((MomentBoolean)fp.Par[2]).Mean ? undef : (mv.Undef | undef);    
            }
            res.Add(mv);
            return 0;
        }

        private static int Scalaraddundefuuub(FunParams fp, List<MomentValue> res)
        {
            MomentValue mv = fp.Par[0].Clone();
            if (fp.Par.Length == 2 || !((MomentBoolean)fp.Par[2]).Mean)
            {
                mv.Nd = MaxNd(fp.Par);
                mv.Err = MaxErrNum(fp.Par);
                mv.Undef = MaxUndef(fp.Par);
            }
            else
            {
                mv.Nd = fp.Par[1].Nd | fp.Par[2].Nd;
                mv.Err = fp.Par[1].Err | fp.Par[2].Nd;
                mv.Undef = fp.Par[1].Undef | fp.Par[2].Nd;
            }
            res.Add(mv);
            return 0;
        }

        private static MomentReal Scalarcheckcertain2rirrrrrrr(MomentValue[] par)
        {
          var v = new double[9];
          for (int i = 1; i <8; i++)
          {
              v[i+1] = ((MomentReal) par[i]).Mean;
          }
          if (Math.Abs(v[5] + v[6] - v[2]) > v[3]) v[5] = v[2] - v[6];
          if (Math.Abs(v[7] + v[8] -v[2]) > v[3]) v[7] = v[2] - v[8];
          if (Math.Abs(v[5] + v[6] - v[7] - v[8]) >= 2 * v[4])
          {
              if (Math.Abs(v[5] + v[6] - v[2]) > Math.Abs(v[7] + v[8] - v[2]))
                  v[5] = (v[5] + v[6] + v[7] + v[8])/2 - v[6];
              else
                  v[7] = (v[5] + v[6] + v[7] + v[8])/2 - v[8];
          }
          if (((MomentInteger)par[0]).Mean == 1) return new MomentReal(v[5], 0, 0);
          return new MomentReal(v[7], 0, 0);
        }

        private static MomentReal Scalarcheckcertainnrirrrrrrr(MomentValue[] par)
        {
            var v = new double[par.Length + 1];
            for (int i = 1; i < par.Length; i++)
            {
                v[i+1] = ((MomentReal) par[i]).Mean;
            }

            int k = (par.Length - 2) / 2;
            double mm = 9E+30;
            while (mm >= v[2])
            {
                double mx = 0;
                for (int j = 1; j <= k; j++)
                {
                    mx = mx + v[2*j + 1] + v[2*j + 2];
                }
                mx /= k;
                mm = 0;
                for (int j = 1; j <= k; j++)
                {
                    if (mm < Math.Abs(v[2*j + 1] + v[2*j + 2] - mx)) 
                        mm = Math.Abs(v[2*j + 1] + v[2*j + 2] - mx);
                }
                if (mm >= v[2])
                {
                    for (int j = 1; j <= k; j++)
                    {
                        if (mm == Math.Abs(v[2 * j + 1] + v[2 * j + 2] - mx))
                            v[2 * j + 1] = mx - v[2 * j + 2];
                    }   
                }
            }
            if (((MomentInteger)par[0]).Mean > k) return new MomentReal(0, 0, 1401);
            return new MomentReal(v[2*((MomentInteger)par[0]).Mean + 1], 0, 0);
        }

        private static MomentReal Scalarcertainnp(MomentReal[] par, double pn, double dn, double dp)
        {
            var res = new MomentReal(pn, NdFlag.Dp | NdFlag.Dn | NdFlag.Nd, 0);
            double s = 0;
            foreach (var p in par)
            {
                //Аппаратная недостоверность
                if (p.Nd >= NdFlag.Nd) p.Mean = pn;
                else
                {
                    res.Nd &= (NdFlag.Dp | NdFlag.Dn);
                    //Сравнение с нормативным
                    if (Math.Abs(p.Mean - pn) > dn)
                    {
                        p.Mean = pn;
                        p.Nd = p.Nd | NdFlag.Dn;
                    }
                    else res.Nd &= NdFlag.Dp;
                }
                s += p.Mean;
            }
            s /= par.Length;
            
            bool e = res.Nd <= NdFlag.Dp;
            if (!e) 
                res.Mean = s;
            //Сравнение друг с другом
            while (e)
            {
                int n = 0;  
                s = 0;
                foreach (var p in par)
                {
                    if (p.Nd == 0)
                    {
                        n++;
                        s += p.Mean;
                    }
                }
                s /= n; //Среднее

                int k = 0; double d = 0;//Максимальное отклонение
                for (int i = 0; i < par.Length; i++)
                {
                    if (Math.Abs(par[i].Mean - s) >= d && par[i].Nd == 0)
                    {
                        k = i;
                        d = Math.Abs(par[i].Mean - s);
                    }
                    if (n == 2 && Math.Abs(par[i].Mean - s) > dp)
                    {
                        par[i].Mean = s;
                        par[i].Nd = NdFlag.Dp;
                        res.Mean = s;
                        e = false;
                    }
                }
                if (d < dp)
                {
                    res.Mean = s;
                    res.Nd = 0;
                    e = false;
                }
                else
                {
                    par[k].Mean = s;
                    par[k].Nd = NdFlag.Dp;
                }
            }
            return res;
        }

        private static MomentReal Scalarcertainnprrrrr(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length - 3];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i + 3].Clone();
            MomentReal res = Scalarcertainnp(parv, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
            res.Nd |= par[0].Nd | par[1].Nd | par[2].Nd;
            res.Err |= par[0].Err | par[1].Err | par[2].Err;
            return res;
        }

        private static MomentReal Scalarcertainnrrrr(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length - 2];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i + 2].Clone();
            MomentReal res = Scalarcertainnp(parv, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean, 9E+40);
            res.Nd |= par[0].Nd | par[1].Nd;
            res.Err |= par[0].Err | par[1].Err;
            return res;
        }

        private static MomentReal Scalarcertainprrr(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length - 1];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i + 1].Clone();
            MomentReal res = Scalarcertainnp(parv, 0, 9E+40, ((MomentReal)par[0]).Mean);
            res.Nd |= par[0].Nd;
            res.Err |= par[0].Err;
            return res;
        }

        private static MomentReal Scalarcertainrr(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i].Clone();
            return Scalarcertainnp(parv, 0, 9E+40, 9E+40);
        }

        private static MomentReal Scalarcertainparamnprirrrr(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length - 4];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i + 4].Clone();
            Scalarcertainnp(parv, ((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
            int num = ((MomentInteger) par[0]).Mean-1;
            parv[num].Nd |= par[0].Nd | par[1].Nd | par[2].Nd | par[3].Nd;
            parv[num].Err |= par[0].Err | par[1].Err | par[2].Err | par[3].Err;
            return parv[num];
        }

        private static MomentReal Scalarcertainparamnrirrr(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length - 3];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i + 3].Clone();
            Scalarcertainnp(parv, ((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, 9E+40);
            int num = ((MomentInteger)par[0]).Mean-1;
            parv[num].Nd |= par[0].Nd | par[1].Nd | par[2].Nd;
            parv[num].Err |= par[0].Err | par[1].Err | par[2].Err;
            return parv[num];
        }

        private static MomentReal Scalarcertainparamprirr(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length - 2];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i + 2].Clone();
            Scalarcertainnp(parv, 0, 9E+40, ((MomentReal)par[1]).Mean);
            int num = ((MomentInteger)par[0]).Mean-1;
            parv[num].Nd |= par[0].Nd | par[1].Nd;
            parv[num].Err |= par[0].Err | par[1].Err;
            return parv[num];
        }

        private static MomentReal Scalarcertainparamrir(MomentValue[] par)
        {
            var parv = new MomentReal[par.Length - 1];
            for (int i = 0; i < parv.Length; i++) parv[i] = (MomentReal)par[i + 1].Clone();
            Scalarcertainnp(parv, 0, 9E+40, 9E+40);
            int num = ((MomentInteger)par[0]).Mean-1;
            parv[num].Nd |= par[0].Nd;
            parv[num].Err |= par[0].Err;
            return parv[num];
        }

        //5 - Фундаментальная
        
        private static int Scalarapertureiir(FunParams fp, List<MomentValue> res)
        {
            if (fp.Previous == null)
            {
                res.Add(new MomentInteger(fp.Par[0]));
            }
            else
            {
                if (fp.Par[0].Undef == 0 && fp.Par[1].Undef == 0)
                {
                    if (fp.Previous.Undef > 0)
                    {
                        res.Add(new MomentInteger(fp.Par[0]));
                    }        
                    else
                    {
                        int m0 = ((MomentInteger)fp.Par[0]).Mean;
                        double m1 = ((MomentReal)fp.Par[1]).Mean;
                        int m2 = ((MomentInteger)fp.Previous).Mean;
                        if (Math.Abs(m0 - m2) >= m1)
                        {
                            res.Add(new MomentInteger(fp.Time, m0, MaxNd(fp.Par), MaxErrNum(fp.Par), 0));
                        }
                    }
                }    
            }
            fp.Previous = res[res.Count-1];
            return 0;
        }

        private static int Scalaraperturerrr(FunParams fp, List<MomentValue> res)
        {
            if (fp.Previous == null)
            {
                res.Add(new MomentReal(fp.Par[0]));
            }
            else
            {
                if (fp.Par[0].Undef == 0 && fp.Par[1].Undef == 0)
                {
                    if (fp.Previous.Undef > 0)
                    {
                        res.Add(new MomentReal(fp.Par[0]));
                    }
                    else
                    {
                        double m0 = ((MomentReal)fp.Par[0]).Mean;
                        double m1 = ((MomentReal)fp.Par[1]).Mean;
                        double m2 = ((MomentReal)fp.Previous).Mean;
                        if (Math.Abs(m0 - m2) >= m1)
                        {
                            res.Add(new MomentReal(fp.Time, m0, MaxNd(fp.Par), MaxErrNum(fp.Par), 0));
                        }
                    }
                }
            }
            fp.Previous = res[res.Count - 1];
            return 0;
        }   

        private static int Scalarpointsuuu(FunParams fp, List<MomentValue> res)
        {
            if (fp.Current[1]) res.Add(fp.Par[0].Clone());
            return 0;
        }
        
        private static int Scalarconditionindicatorbb(FunParams fp, List<MomentValue> res)
        {
            if (fp.Previous == null || fp.Previous.Undef != fp.Par[0].Undef  || ((MomentBoolean)fp.Previous).Mean !=((MomentBoolean)fp.Par[0]).Mean)
            {
                res.Add(new MomentBoolean(fp.Par[0]));
             }
            fp.Previous = fp.Par[0];
            return 0;
        }

        private static int Scalarconditiontimeuub(FunParams fp, List<MomentValue> res)
        {
            if (fp.Previous != null && fp.Previous.Undef == 0 && fp.Par[1].Undef == 0 && !((MomentBoolean)fp.Previous).Mean && ((MomentBoolean)fp.Par[1]).Mean)
            {
                var r=fp.Par[0].Clone();
                r.Nd |= (fp.Par[1].Nd | fp.Previous.Nd);
                r.Err = r.Err | fp.Par[1].Err | fp.Previous.Err;
                res.Add(r);
            }
            fp.Previous = fp.Par[1];
            return 0;
        }   

        private static int Scalarzoneuubs(FunParams fp, List<MomentValue> res)
        {
            int undef = MaxUndef(fp.Par);
            if (fp.Previous == null)
            {
                if (fp.Par[0].Undef == 0 && fp.Par[1].Undef == 0 && !((MomentBoolean)fp.Par[1]).Mean) undef = ZoneFlag.Zone;
            }
            else
            {
                if (fp.Par[0].Undef > 0 || fp.Par[1].Undef > 0 || !((MomentBoolean)fp.Par[1]).Mean)
                {
                    undef |= ZoneFlag.Zone;
                }
                else
                {
                    undef = 0;
                }
            }
            if (undef == 0 || fp.Previous == null || fp.Previous.Undef != undef)
            {
                var mv = fp.Par[0].Clone();
                mv.Time = fp.Time;
                mv.Nd = MaxNd(fp.Par);
                mv.Err = MaxErrNum(fp.Par);
                mv.Undef = undef;
                res.Add(mv);
                fp.Previous = mv;
            }
            return 0;
        }

        private static int Scalarpointtimedui(FunParams fp, List<MomentValue> res)
        {
            if (fp.Current[0])
            {
                int n = fp.Number[0] + ((MomentInteger) fp.Par[1]).Mean;
                if (n >= 0 && n < fp.ListPar[0].Count)
                {
                    res.Add(new MomentTime(fp.Time, fp.ListPar[0][n].Time, MaxNd(fp.Par), MaxErrNum(fp.Par), MaxUndef(fp.Par)));
                }
            }
            return 0;
        }

        private static int Scalartopointtimerui(FunParams fp, List<MomentValue> res)
        {
            if (fp.Current[0])
            {
                int n = fp.Number[0] + ((MomentInteger)fp.Par[1]).Mean;
                if (n >= 0 && n < fp.ListPar[0].Count)
                {
                    res.Add(new MomentReal(fp.Time, fp.Time.Subtract(fp.ListPar[0][n].Time).TotalSeconds, MaxNd(fp.Par), MaxErrNum(fp.Par), MaxUndef(fp.Par)));
                }
            }
            return 0;
        }

        private static int Scalaraddpointsuuu(FunParams fp, List<MomentValue> res)
        {
            res.Add(!fp.Current[1] ? fp.Par[0].Clone() : fp.Par[1].Clone());
            return 0;
        }

        //Добавляет MomentValue в список так, чтобы он получился отсортированным по времени
        private static void AddMomentValue(IList<MomentValue> res, MomentValue mv)
        {
            if (res.Count == 0 || mv.Time > res[res.Count-1].Time)
            {
                res.Add(mv);    
            }
            else
            {
                if (mv.Time < res[0].Time)
                {
                    res.Insert(0, mv);
                }
                else
                {
                    int beg = 0;
                    int en = res.Count - 1;
                    while (beg < en - 1)
                    {
                        int c = (beg + en) / 2;
                        if (res[c].Time > mv.Time) 
                            en = c;
                        else
                            beg = c;
                    }
                    if (res[beg].Time == mv.Time)
                    {
                        res[beg] = mv;
                    }
                    else
                    {
                        if (res[en].Time == mv.Time)
                        {
                            res[en] = mv;
                        }
                        else
                        {
                            res.Insert(beg + 1, mv);
                        }
                    }   
                }
            }
        }

        private static int Scalarsettimeuud(FunParams fp, List<MomentValue> res)
        {
            if (fp.Current[0])
            {
                MomentValue mv = fp.Par[0].Clone();
                mv.Time = ((MomentTime)fp.Par[1]).Mean;
                mv.Undef = MaxUndef(fp.Par);
                mv.Nd = MaxNd(fp.Par);
                mv.Err = MaxErrNum(fp.Par);
                if (mv.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || mv.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    mv.Err |= 1502;
                    mv.Time = fp.Par[0].Time;
                }
                AddMomentValue(res, mv);   
            }
            return 0;
        }
        
        //6 - Статистическая

        private static int Scalarfirstuu(FunParams fp, List<MomentValue> res)
        {
            if (fp.Result == null) fp.Result=fp.Par[0].Clone();
            if (fp.Par[0].Undef < fp.Result.Undef) fp.Result = fp.Par[0].Clone();
            if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(fp.Result);
            return 0;
        }

        private static int Scalarlastuu(FunParams fp, List<MomentValue> res)
        {
            if (fp.Result == null) fp.Result = fp.Par[0].Clone();
            if (fp.Par[0].Undef == 0) fp.Result = fp.Par[0].Clone();
            if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(fp.Result);
            return 0;
        }

        private static int Scalarminimumuu(FunParams fp, List<MomentValue> res)
        {
            bool fmax = (fp.Flags & FunFlags.Max) > 0;
            if (fp.Result == null || fp.Result.Undef > 0)
            {
                fp.Result = fp.Par[0].Clone();
            }
            else
            {
                if (fp.Result.Undef == 0 && fp.Par[0].Undef == 0)
                {
                    int nd = fp.Result.Nd;
                    ErrNum err = fp.Result.Err;
                    if ((!fmax && fp.Par[0] < fp.Result) || (fmax && fp.Par[0] > fp.Result))
                    {
                        fp.Result = fp.Par[0].Clone();
                    }
                    fp.Result.Nd = nd | fp.Par[0].Nd;
                    fp.Result.Err = err | fp.Par[0].Err;
                }
            }
            if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(fp.Result);
            return 0;
        }
        
        private static int Scalarintegralrr(FunParams fp, List<MomentValue> res)
        {
            if (fp.Par[0] == null)
            {
                if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(new MomentReal(fp.CalcParamRun.ThreadCalc.PeriodBegin, 0, 0, 0, 0));
                return 0;
            }
            if (fp.Result == null)
            {
                fp.Result = new MomentReal(fp.Time, 0, fp.Par[0].Nd, fp.Par[0].Err, 0);
            }
            else
            {
                if (fp.Previous.Undef == 0)
                {
                    switch (fp.CalcParamRun.CalcParam.Interpolation)
                    {
                        case InterpolationType.Constant:
                            fp.Result.Nd |= fp.Previous.Nd;
                            fp.Result.Err |= fp.Previous.Err;
                            break;
                        case InterpolationType.Linear:
                            fp.Result.Nd |= (fp.Previous.Nd | fp.Par[0].Nd);
                            fp.Result.Err = fp.Result.Err | fp.Par[0].Err | fp.Previous.Err;
                            break;
                    }
                    ((MomentReal) fp.Result).Mean += SimpleIntegral(fp.CalcParamRun.CalcParam.Interpolation, fp.Previous, fp.Par[0]);
                }   
            }
            fp.Previous = new MomentReal(fp.Par[0]);
            if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(fp.Result);
            return 0;
        }

        private static int Scalartimerb(FunParams fp, List<MomentValue> res)
        {
            if (fp.Par[0] == null)
            {
                if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(new MomentReal(fp.CalcParamRun.ThreadCalc.PeriodBegin, 0, 0, 0, 0));
                return 0;
            }
            if (fp.Result == null)
            {
                fp.Result = new MomentReal(fp.Time, 0, fp.Par[0].Nd, fp.Par[0].Err, 0);
            }
            else
            {
                if (fp.Previous.Undef == 0 && ((MomentBoolean)fp.Previous).Mean)
                {
                    fp.Result.Nd |= (fp.Par[0].Nd | fp.Previous.Nd);
                    fp.Result.Err = fp.Result.Err | fp.Par[0].Err | fp.Previous.Err;
                    ((MomentReal)fp.Result).Mean += fp.Par[0].Time.Subtract(fp.Previous.Time).TotalSeconds;
                }
            }
            fp.Previous = new MomentBoolean(fp.Par[0]);
            if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(fp.Result);
            return 0;
        }

        private static int Scalarsummuu(FunParams fp, List<MomentValue> res)
        {
            int type = (fp.Flags & FunFlags.Int) != 0 ? FunFlags.Int : ((fp.Flags & FunFlags.Real) != 0 ? FunFlags.Real : FunFlags.String);
            if (fp.Par[0] == null)
            {
                if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    if (type == FunFlags.Int) res.Add(new MomentInteger(fp.CalcParamRun.ThreadCalc.PeriodBegin, 0, 0, 0, 0));
                    if (type == FunFlags.Real) res.Add(new MomentReal(fp.CalcParamRun.ThreadCalc.PeriodBegin, 0, 0, 0, 0));
                    if (type == FunFlags.String) res.Add(new MomentString(fp.CalcParamRun.ThreadCalc.PeriodBegin, "", 0, 0, 0));
                }
                return 0;
            }
            if (fp.Result == null)
            {
                if (type == FunFlags.Int) fp.Result = new MomentInteger(fp.Time, 0, 0, 0, 0);
                if (type == FunFlags.Real) fp.Result = new MomentReal(fp.Time, 0, 0, 0, 0);
                if (type == FunFlags.String) fp.Result = new MomentString(fp.Time, "", 0, 0, 0);
                res.Add(fp.Result);
            }
            if (fp.Par[0].Undef == 0)
            {
                fp.Result.Nd |= fp.Par[0].Nd;
                fp.Result.Err |= fp.Par[0].Err;
                if (type == FunFlags.Int) ((MomentInteger)fp.Result).Mean += ((MomentInteger)fp.Par[0]).Mean;
                if (type == FunFlags.Real) ((MomentReal)fp.Result).Mean += ((MomentReal)fp.Par[0]).Mean;
                if (type == FunFlags.String) ((MomentString)fp.Result).Mean += ((MomentString)fp.Par[0]).Mean;
            }
            return 0;
        }
        
        private static int Scalarquantityib(FunParams fp, List<MomentValue> res)
        {
            if (fp.Par[0] == null)
            {
                if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(new MomentInteger(fp.CalcParamRun.ThreadCalc.PeriodBegin, 0, 0, 0, 0));
                return 0;
            }
            if (fp.Result == null)
            {
                fp.Result = new MomentInteger(fp.Time, 0, fp.Par[0].Nd, fp.Par[0].Err, 0);
            }
            else
            {
                if (fp.Previous.Undef == 0 && fp.Par[0].Undef == 0)
                {
                    fp.Result.Nd |= (fp.Par[0].Nd | fp.Previous.Nd);
                    fp.Result.Err = fp.Result.Err | fp.Par[0].Err | fp.Previous.Err;
                    if (!((MomentBoolean)fp.Previous).Mean && ((MomentBoolean)fp.Par[0]).Mean)
                    {
                        ((MomentInteger)fp.Result).Mean ++;
                    }
                }
            }
            fp.Previous = new MomentBoolean(fp.Par[0]);
            if (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd) res.Add(fp.Result);
            return 0;
        }

        //7 - Фильтр
        
        public static double speed(LinkedList<MomentValue> par)
        {
            //Метод наименьших квадратов
            //v=M((X-MX)(Y-MY))/M((X-MX)(X-MX))
            int n = par.Count;
            if (n <= 1) return 0;
            double my = 0, mx = 0;
            LinkedListNode<MomentValue> cur = par.First;
            while (cur != null)
            {
                my += ((MomentReal)cur.Value).Mean;
                mx += cur.Value.Time.Subtract(par.First.Value.Time).TotalSeconds;
                cur = cur.Next;
            }
            mx = mx / n;
            my = my / n;
            double mmx = 0, mmy = 0;
            cur = par.First;
            while (cur != null)
            {
                double dx = cur.Value.Time.Subtract(par.First.Value.Time).TotalSeconds - mx;
                mmx += dx*dx;
                mmy += dx * (((MomentReal)cur.Value).Mean - my);
                cur = cur.Next;
            }
            if (mmx == 0) return 0;
            mmx = mmx / n;
            mmy = mmy / n;
            return mmy / mmx;
        }

        private static int Scalarspeedrrir(FunParams fp, List<MomentValue> res)
        {
            int undef = MaxUndef(fp.Par), nd =MaxNd(fp.Par);
            ErrNum err = MaxErrNum(fp.Par);
            int n = ((MomentInteger) fp.Par[1]).Mean;
            double t = ((MomentReal) fp.Par[2]).Mean;
            if (n < 2)
            {
                //Недопустимое количество точек или итоговое время у функции Скорость (Speed)
                err |= 1701;
            }
            fp.Filter.AddLast(fp.Par[0].Clone());
            if (fp.FilterNd.ContainsKey(fp.Par[0].Nd))
            { fp.FilterNd[fp.Par[0].Nd]++; }
            else
            { fp.FilterNd.Add(fp.Par[0].Nd, 1); }
            if (fp.FilterErr.ContainsKey(fp.Par[0].Err.Num))
            { fp.FilterErr[fp.Par[0].Err.Num]++; }
            else
            { fp.FilterErr.Add(fp.Par[0].Err.Num, 1); }

            if (fp.Filter.Count > n)
            {
                if (fp.FilterNd[fp.Filter.First.Value.Nd] == 1)
                { fp.FilterNd.Remove(fp.Filter.First.Value.Nd); }
                else
                { fp.FilterNd[fp.Filter.First.Value.Nd]--; }

                if (fp.FilterErr[fp.Filter.First.Value.Err.Num] == 1)
                { fp.FilterErr.Remove(fp.Filter.First.Value.Err.Num); }
                else
                { fp.FilterErr[fp.Filter.First.Value.Err.Num]--; }

                fp.Filter.RemoveFirst();
            }
            if ((undef == 0 && fp.Filter.Count == n) || (undef > 0 && fp.Filter.Count > 1 && fp.Filter.Count < n) || (fp.Time == fp.CalcParamRun.ThreadCalc.PeriodEnd && fp.Filter.Count < n))
            {
                DateTime time =fp.Filter.First.Value.Time.AddSeconds(t*fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                if (time < fp.CalcParamRun.ThreadCalc.PeriodBegin || time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    var mv = new MomentReal(fp.Time, speed(fp.Filter), fp.FilterNd.Keys[fp.FilterNd.Count - 1] | nd, fp.FilterErr.Keys[fp.FilterErr.Count - 1] | err | 1701, undef);
                    AddMomentValue(res, mv);
                }
                else
                {
                    var mv = new MomentReal(time, speed(fp.Filter), fp.FilterNd.Keys[fp.FilterNd.Count - 1] | nd, fp.FilterErr.Keys[fp.FilterErr.Count - 1] | err, undef);
                    AddMomentValue(res, mv);
                }
            }
            if (undef > 0 && fp.Filter.Count > 0)
            {
                var mv = new MomentReal(fp.Time, 0, nd, err, undef);
                AddMomentValue(res, mv);
                fp.Filter.Clear();
                fp.FilterNd.Clear();
            }
            return 0;
        }

        //Пересчет недостоверности для фильтра
        private static void ChangeFilterNd(FunParams fp)
        {
            int nd = fp.Par[0].Nd, errn=fp.Par[0].Err.Num;
            if (fp.FilterNd.ContainsKey(nd)) fp.FilterNd[nd]++;
            else fp.FilterNd.Add(nd, 1);
            if (fp.FilterErr.ContainsKey(errn)) fp.FilterErr[errn]++;
            else fp.FilterErr.Add(errn, 1);
            if (fp.Filter.Count == 0) return;

            LinkedListNode<MomentValue> cur = fp.Filter.First;
            DateTime btime = fp.Time.AddSeconds(-((MomentReal)fp.Par[1]).Mean);
            while (cur.Next != null && cur.Next.Value.Time < btime)
            {
                if (fp.FilterNd[cur.Value.Nd] == 1)
                    fp.FilterNd.Remove(cur.Value.Nd);
                else
                    fp.FilterNd[cur.Value.Nd]--;
                
                if (fp.FilterErr[cur.Value.Err.Num] == 1)
                    fp.FilterErr.Remove(cur.Value.Err.Num);
                else
                    fp.FilterErr[cur.Value.Err.Num]--;
                cur = cur.Next;
            }
        }

        private static int Scalarfilterfirstuurr(FunParams fp, List<MomentValue> res)
        {
            int nd =fp.Par[1].Nd | fp.Par[2].Nd;
            ErrNum err = fp.Par[1].Err | fp.Par[2].Err;
            double r = ((MomentReal)fp.Par[1]).Mean;
            double t = ((MomentReal)fp.Par[2]).Mean;
            //if (r < 0 && r > fp.CalcParamRun.ThreadCalc.PeriodLength)
            if (r < 0)
            {
                //Недопустимое количество точек или итоговое время у функции ФильтрПервое
                err |= 1702;
            }
            ChangeFilterNd(fp);
            fp.Filter.AddLast(fp.Par[0].Clone());
            DateTime tt = fp.Filter.Last.Value.Time.AddSeconds(-r);
            while (fp.Filter.Count > 1 && fp.Filter.First.Value.Time < tt)
            {
                if (fp.Filter.First.Next.Value.Time <= tt)
                {
                    fp.Filter.RemoveFirst();    
                }
                else
                {
                    if (fp.Filter.First.Value.Time < tt)
                    {
                        var vpar = new List<MomentValue> { fp.Filter.First.Value, fp.Filter.First.Next.Value };
                        int tnd = fp.Filter.First.Value.Nd;
                        ErrNum terr = fp.Filter.First.Value.Err;
                        fp.Filter.First.Value = Interpolation(fp.CalcParamRun.CalcParam.Interpolation, vpar, 0, tt);
                        fp.Filter.First.Value.Nd = tnd;
                        fp.Filter.First.Value.Err = terr;   
                    }
                }
            }
            if (fp.Filter.First.Value.Time == tt)
            {
                var mv = fp.Filter.First.Value.Clone();
                mv.Time = fp.Filter.First.Value.Time.AddSeconds(t * fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                mv.Nd |= nd;
                mv.Err |= err;
                if (fp.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || fp.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    mv.Err |= 1702;
                    mv.Time = fp.Time;
                }
                AddMomentValue(res, mv);   
            }
            return 0;
        }

        private static int Scalarfilterminimumuurr(FunParams fp, List<MomentValue> res)
        {
            bool fmax = (fp.Flags & FunFlags.Max) > 0; 
            int undef = MaxUndef(fp.Par), nd=MaxNd(fp.Par);
            ErrNum err = MaxErrNum(fp.Par);
            double r = ((MomentReal)fp.Par[1]).Mean;
            double t = ((MomentReal)fp.Par[2]).Mean;
            //if (r < 0 && r > fp.CalcParamRun.ThreadCalc.PeriodLength)
            if (r < 0)
            {
                //Недопустимое количество точек или итоговое время у функции ФильтрМинимум
                err |= 1703;
            }
            ChangeFilterNd(fp);
            if (fp.Result == null || (!fmax && fp.Par[0] < fp.Result) || (fmax && fp.Par[0] > fp.Result))
            {
                fp.Result = fp.Par[0].Clone();
            }
            fp.Filter.AddLast(fp.Par[0].Clone());

            DateTime tt = fp.Filter.Last.Value.Time.AddSeconds(-r);
            bool e = false;
            while (fp.Filter.Count > 1 && fp.Filter.First.Value.Time < tt)
            {
                if (fp.Filter.First.Next.Value.Time <= tt)
                {
                    if (fp.Filter.First.Value == fp.Result) e = true;
                    fp.Filter.RemoveFirst();
                }
                else
                {
                    if (fp.Filter.First.Value.Time < tt)
                    {
                        if (fp.Filter.First.Value == fp.Result) e = true;
                        var vpar = new List<MomentValue> { fp.Filter.First.Value, fp.Filter.First.Next.Value };
                        int tnd = fp.Filter.First.Value.Nd;
                        ErrNum terr = fp.Filter.First.Value.Err;
                        fp.Filter.First.Value = Interpolation(fp.CalcParamRun.CalcParam.Interpolation, vpar, 0, tt);
                        fp.Filter.First.Value.Nd = tnd;
                        fp.Filter.First.Value.Err = terr;
                    }
                }
            }
            if (fp.Filter.First.Value.Time == tt)
            {
                if (e)
                {
                    fp.Result = fp.Filter.First.Value;
                    foreach (var mm in fp.Filter)
                    {
                        if ((!fmax && mm < fp.Result) || (fmax && mm > fp.Result)) fp.Result = mm;
                    }
                    fp.Result = fp.Result.Clone();
                }
                var mv = fp.Result.Clone();
                mv.Nd = fp.FilterNd.Keys[fp.FilterNd.Keys.Count - 1] | nd;
                mv.Err = fp.FilterErr.Keys[fp.FilterErr.Keys.Count - 1] | err;
                mv.Time = fp.Filter.First.Value.Time.AddSeconds(t * fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                if (fp.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || fp.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    mv.Err |= 1703;
                    mv.Time = fp.Time;
                }
                mv.Undef = undef;
                AddMomentValue(res, mv);
            }
            if (undef > 0)
            {
                fp.Filter.Clear();
                fp.FilterNd.Clear();
                fp.FilterErr.Clear();
                fp.Result = null;
            }
            return 0;
        }

        private static int Scalarfiltertimebrrr(FunParams fp, List<MomentValue> res)
        {
            int errint = (fp.Flags & FunFlags.Covariation) != 0 ? 1708 : ((fp.Flags & FunFlags.Dispersion) != 0 ? 1707 : ((fp.Flags & FunFlags.Average) != 0 ? 1706 : 1705));
            int undef = MaxUndef(fp.Par), nd = MaxNd(fp.Par);
            ErrNum err = MaxErrNum(fp.Par);
            double r = ((MomentReal)fp.Par[1]).Mean;
            double t = ((MomentReal)fp.Par[2]).Mean;
            //if (r <= 0 && r > fp.CalcParamRun.ThreadCalc.PeriodLength)
            if (r <= 0)
            {
                //Недопустимая длина промежутка или итоговое время у функции ФильтрИнтеграл
                err |= errint;
            }
            ChangeFilterNd(fp);

            if (fp.Result == null)
                fp.Result = new MomentReal(fp.Time, 0, nd, err, 0);
            else
            {
                fp.Result.Nd |= fp.Previous.Nd;
                fp.Result.Err |= fp.Previous.Err;
                if (((MomentBoolean)fp.Previous).Mean)
                    ((MomentReal)fp.Result).Mean += (fp.Par[0].Time.Subtract(fp.Previous.Time).TotalSeconds);
            }

            fp.Filter.AddLast(fp.Par[0].Clone());
            fp.Previous = new MomentBoolean(fp.Filter.Last.Value);
            DateTime tt = fp.Filter.Last.Value.Time.AddSeconds(-r);
            while (fp.Filter.Count > 1 && fp.Filter.First.Value.Time < tt)
            {
                if (fp.Filter.First.Next.Value.Time <= tt)
                {
                    if (((MomentBoolean)fp.Filter.First.Value).Mean)
                        ((MomentReal)fp.Result).Mean -= (fp.Filter.First.Next.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                    fp.Filter.RemoveFirst();
                }
                else
                {
                    if (fp.Filter.First.Value.Time < tt)
                    {
                        if (((MomentBoolean)fp.Filter.First.Value).Mean)
                            ((MomentReal)fp.Result).Mean -= (tt.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                    }
                }
            }
            if (fp.Filter.First.Value.Time == tt)
            {
                var mv = fp.Result.Clone();
                mv.Nd = fp.FilterNd.Keys[fp.FilterNd.Keys.Count - 1] | nd;
                mv.Err = fp.FilterErr.Keys[fp.FilterErr.Keys.Count - 1] | err;
                mv.Time = fp.Filter.First.Value.Time.AddSeconds(t * fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                if (fp.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || fp.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    mv.Err |= errint;
                    mv.Time = fp.Time;
                }
                mv.Undef = undef;
                AddMomentValue(res, mv);
            }
            if (undef > 0)
            {
                fp.Filter.Clear();
                fp.FilterNd.Clear();
                fp.FilterErr.Clear();
                fp.Result = null;
            }
            return 0;
        }
        
        private static int Scalarfilterintegralrrrr(FunParams fp, List<MomentValue> res)
        {
            int errint = (fp.Flags & FunFlags.Covariation) != 0 ? 1708 : ((fp.Flags & FunFlags.Dispersion) != 0 ? 1707 : ((fp.Flags & FunFlags.Average) != 0 ? 1706 : 1705));
            int undef = MaxUndef(fp.Par), nd = MaxNd(fp.Par);
            ErrNum err = MaxErrNum(fp.Par);
            double r = ((MomentReal)fp.Par[1]).Mean;
            double t = ((MomentReal)fp.Par[2]).Mean;
            //if (r <= 0 && r > fp.CalcParamRun.ThreadCalc.PeriodLength)
            if (r <= 0)
            {
                //Недопустимая длина промежутка или итоговое время у функции ФильтрИнтеграл
                err |= errint;
            }
            ChangeFilterNd(fp);

            if (fp.Result == null)
            {
                fp.Result = new MomentReal(fp.Time, 0, nd, err, 0);
            }
            else
            {
                switch (fp.CalcParamRun.CalcParam.Interpolation)
                {
                    case InterpolationType.Constant:
                        fp.Result.Nd |= fp.Previous.Nd;
                        fp.Result.Err |= fp.Previous.Err;
                        break;
                    case InterpolationType.Linear:
                        fp.Result.Nd |= (fp.Previous.Nd | nd);
                        fp.Result.Err = fp.Result.Err | fp.Previous.Err | err;
                        break;
                }
                ((MomentReal)fp.Result).Mean += SimpleIntegral(fp.CalcParamRun.CalcParam.Interpolation, fp.Previous, fp.Par[0]);
            }
            
            fp.Filter.AddLast(fp.Par[0].Clone());
            fp.Previous = new MomentReal(fp.Filter.Last.Value);
            DateTime tt = fp.Filter.Last.Value.Time.AddSeconds(-r);
            while (fp.Filter.Count > 1 && fp.Filter.First.Value.Time < tt)
            {
                if (fp.Filter.First.Next.Value.Time <= tt)
                {
                    ((MomentReal)fp.Result).Mean -= SimpleIntegral(fp.CalcParamRun.CalcParam.Interpolation, fp.Filter.First.Value, fp.Filter.First.Next.Value);
                    fp.Filter.RemoveFirst();
                }
                else
                {
                    if (fp.Filter.First.Value.Time < tt)
                    {
                        var mv = fp.Filter.First.Value;                        
                        var vpar = new List<MomentValue> { mv, fp.Filter.First.Next.Value };
                        int tnd = mv.Nd;
                        ErrNum terr = mv.Err;
                        fp.Filter.First.Value = Interpolation(fp.CalcParamRun.CalcParam.Interpolation, vpar, 0, tt);
                        fp.Filter.First.Value.Nd = tnd;
                        fp.Filter.First.Value.Err = terr;
                        ((MomentReal)fp.Result).Mean -= SimpleIntegral(fp.CalcParamRun.CalcParam.Interpolation, mv, fp.Filter.First.Value);
                    }
                }
            }
            if (fp.Filter.First.Value.Time == tt)
            {
                var mv = fp.Result.Clone();
                mv.Nd = fp.FilterNd.Keys[fp.FilterNd.Keys.Count - 1] | nd;
                mv.Err = fp.FilterErr.Keys[fp.FilterErr.Keys.Count - 1] | err;
                mv.Time = fp.Filter.First.Value.Time.AddSeconds(t * fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                if (fp.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || fp.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    mv.Err |= errint;
                    mv.Time = fp.Time;
                }
                mv.Undef = undef;
                if ((fp.Flags & FunFlags.Average) != 0 && r > 0) ((MomentReal)mv).Mean /= ((MomentReal)fp.Par[1]).Mean;
                AddMomentValue(res, mv);
            }
            if (undef > 0)
            {
                fp.Filter.Clear();
                fp.FilterNd.Clear();
                fp.FilterErr.Clear();
                fp.Result = null;
            }
            return 0;
        }

        private static int Scalarfiltersummuurr(FunParams fp, List<MomentValue> res)
        {
            int type = (fp.Flags & FunFlags.Int) != 0 ? FunFlags.Int : ((fp.Flags & FunFlags.Real) != 0 ? FunFlags.Real : FunFlags.String);
            int undef = MaxUndef(fp.Par), nd = MaxNd(fp.Par);
            ErrNum err = MaxErrNum(fp.Par);
            double r = ((MomentReal)fp.Par[1]).Mean;
            double t = ((MomentReal)fp.Par[2]).Mean;
            //if (r < 0 && r > fp.CalcParamRun.ThreadCalc.PeriodLength)
            if (r < 0)
            {
                //Недопустимое количество точек или итоговое время у функции ФильтрСумма
                err |= 1710;
            }
            ChangeFilterNd(fp);

            if (fp.Result == null)
            {
                if (type == FunFlags.Int)
                {
                    fp.Result = new MomentInteger(fp.Time, ((MomentInteger)fp.Par[0]).Mean, nd, err, 0);
                }
                else
                {
                    fp.Result = new MomentReal(fp.Time, ((MomentReal)fp.Par[0]).Mean, nd, err, 0);                    
                }
            }
            else
            {
                fp.Result.Nd |= nd;
                fp.Result.Err |= err;
                if (type == FunFlags.Int)
                {
                    ((MomentInteger)fp.Result).Mean += ((MomentInteger)fp.Par[0]).Mean;    
                }
                else
                {
                    ((MomentReal)fp.Result).Mean += ((MomentReal)fp.Par[0]).Mean;    
                }
            }
            fp.Filter.AddLast(fp.Par[0].Clone());

            DateTime tt = fp.Filter.Last.Value.Time.AddSeconds(-r);
            bool e = (fp.Filter.First.Value.Time <= tt);
            while (fp.Filter.Count > 1 && fp.Filter.First.Value.Time < tt)
            {
                if (type == FunFlags.Int)
                {
                    ((MomentInteger) fp.Result).Mean -= ((MomentInteger) fp.Filter.First.Value).Mean;
                }
                else
                {
                    ((MomentReal)fp.Result).Mean -= ((MomentReal)fp.Filter.First.Value).Mean;                    
                }
                fp.Filter.RemoveFirst();
            }
            if (e)
            {
                var mv = fp.Result.Clone();
                mv.Nd = fp.FilterNd.Keys[fp.FilterNd.Keys.Count - 1] | nd;
                mv.Err = fp.FilterErr.Keys[fp.FilterErr.Keys.Count - 1] | err;
                mv.Time = fp.Filter.First.Value.Time.AddSeconds(t * fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                if (fp.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || fp.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    mv.Err |= 1710;
                    mv.Time = fp.Time;
                }
                mv.Undef = undef;
                AddMomentValue(res, mv);
            }
            if (undef > 0)
            {
                fp.Filter.Clear();
                fp.FilterNd.Clear();
                fp.FilterErr.Clear();
                fp.Result = null;
            }
            return 0;
        }

        private static int Scalarfilterquantitypointsiurr(FunParams fp, List<MomentValue> res)
        {
            int undef = fp.Par[1].Undef | fp.Par[2].Undef, nd = fp.Par[1].Nd | fp.Par[2].Nd;
            ErrNum err = fp.Par[1].Err | fp.Par[2].Err;
            double r = ((MomentReal)fp.Par[1]).Mean;
            double t = ((MomentReal)fp.Par[2]).Mean;
            //if (r < 0 && r > fp.CalcParamRun.ThreadCalc.PeriodLength)
            if (r < 0)
            {
                res.Add(new MomentInteger(fp.Time, 0, nd,  err | 1711, undef));
                return 0;
            }
            fp.Filter.AddLast(fp.Par[0].Clone());
            DateTime tt = fp.Filter.Last.Value.Time.AddSeconds(-r);
            bool e = (fp.Filter.First.Value.Time <= tt);
            while (fp.Filter.Count > 1 && fp.Filter.First.Value.Time < tt)
            {
                fp.Filter.RemoveFirst();
            }
            if (e)
            {
                var mv = new MomentInteger(fp.Filter.Count, nd, err)
                             {
                                 Undef = undef,
                                 Time = fp.Filter.First.Value.Time.AddSeconds(t * fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds)
                             };
                if (fp.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || fp.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    res.Add(new MomentInteger(fp.Time, 0, nd, err | 1711, undef));
                }
                else
                {
                    AddMomentValue(res, mv);       
                }
            }
            return 0;
        }

        private static int Scalarfilterquantityibrr(FunParams fp, List<MomentValue> res)
        {
            int undef = MaxUndef(fp.Par), nd = MaxNd(fp.Par);
            ErrNum err = MaxErrNum(fp.Par);
            double r = ((MomentReal)fp.Par[1]).Mean;
            double t = ((MomentReal)fp.Par[2]).Mean;
            //if (r < 0 && r > fp.CalcParamRun.ThreadCalc.PeriodLength)
            if (r < 0)
            {
                //Недопустимое количество точек или итоговое время у функции ФильтрКоличество
                err |= 1709;
            }
            ChangeFilterNd(fp);

            if (fp.Result == null)
            {
                fp.Result = new MomentInteger(fp.Time, 0, nd, err, 0);
            }
            else
            {
                fp.Result.Nd |= nd;
                fp.Result.Err |= err;
                if (((MomentBoolean)fp.Par[0]).Mean && !((MomentBoolean)fp.Filter.Last.Value).Mean)
                {
                    ((MomentInteger)fp.Result).Mean++;
                }
            }
            fp.Filter.AddLast(fp.Par[0].Clone());

            DateTime tt = fp.Filter.Last.Value.Time.AddSeconds(-r);
            bool e = (fp.Filter.First.Value.Time <= tt);
            while (fp.Filter.Count > 1 && fp.Filter.First.Value.Time < tt)
            {
                if (((MomentBoolean)fp.Filter.First.Next.Value).Mean && !((MomentBoolean)fp.Filter.First.Value).Mean)
                {
                    ((MomentInteger) fp.Result).Mean--;    
                }
                fp.Filter.RemoveFirst();
            }
            if (e)
            {
                var mv = fp.Result.Clone();
                mv.Nd = fp.FilterNd.Keys[fp.FilterNd.Keys.Count - 1] | nd;
                mv.Err = fp.FilterErr.Keys[fp.FilterErr.Keys.Count - 1] | err;
                mv.Time = fp.Filter.First.Value.Time.AddSeconds(t * fp.Filter.Last.Value.Time.Subtract(fp.Filter.First.Value.Time).TotalSeconds);
                if (fp.Time < fp.CalcParamRun.ThreadCalc.PeriodBegin || fp.Time > fp.CalcParamRun.ThreadCalc.PeriodEnd)
                {
                    mv.Err |= 1709;
                    mv.Time = fp.Time;
                }
                mv.Undef = undef;
                AddMomentValue(res, mv);
            }
            if (undef > 0)
            {
                fp.Filter.Clear();
                fp.FilterNd.Clear();
                fp.FilterErr.Clear();
                fp.Result = null;
            }
            return 0;
        }

        //8 - График

        private static int Scalardiagramr(FunParams fp, List<MomentValue> res)
        {
            ErrNum err = MaxErrNum(fp.Par);
            var vpar = new double[fp.Par.Length+1];
            for (int i = 0; i < fp.Par.Length;++i )
            {
                vpar[fp.Par.Length - i] = ((MomentReal)fp.Par[i]).Mean;
            }
            Grafic gr = fp.CalcParamRun.CalcParam.Grafic;
            double mean = gr.Calculate(vpar, gr.GraficType);
            if (double.IsNaN(mean))
            {
                err |= 1801; //Ошибка при вычислении значения по графику
                mean = 0;
            }
            res.Add(new MomentReal(fp.Time, mean, MaxNd(fp.Par), err, MaxUndef(fp.Par)));
            return 0;
        }

        //10 - Типы данных

        private static MomentValue Scalarvaluevv(MomentValue[] par)
        {
            return new MomentValue(par[0]);
        }

        private static MomentValue Scalarvaluevb(MomentValue[] par)
        {
            return new MomentValue(par[0]);
        }

        private static MomentValue Scalarvaluevi(MomentValue[] par)
        {
            return new MomentValue(par[0]);
        }

        private static MomentValue Scalarvaluevd(MomentValue[] par)
        {
            return new MomentValue(par[0]);
        }

        private static MomentValue Scalarvaluevr(MomentValue[] par)
        {
            return new MomentValue(par[0]);
        }

        private static MomentValue Scalarvaluevs(MomentValue[] par)
        {
            return new MomentValue(par[0]);
        }

        private static MomentBoolean Scalarboolbv(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Boolean) return new MomentBoolean(par[0]);
            return new MomentBoolean(false, par[0].Nd, par[0].Err);   
        }

        private static MomentBoolean Scalarboolbb(MomentValue[] par)
        {
            return new MomentBoolean(par[0]);
        }

        private static MomentBoolean Scalarboolbi(MomentValue[] par)
        {
            bool m = ((MomentInteger)par[0]).Mean == 0 ? false : true;
            return new MomentBoolean(m, par[0].Nd, par[0].Err);
        }

        private static MomentBoolean Scalarboolbr(MomentValue[] par)
        {
            bool m = ((MomentReal)par[0]).Mean == 0 ? false : true;
            return new MomentBoolean(m, par[0].Nd, par[0].Err);
        }

        private static MomentBoolean Scalarboolbs(MomentValue[] par)
        {
            bool m = (((MomentString)par[0]).Mean == "0" || ((MomentString)par[0]).Mean.ToLower() == "false") ? false : true;
            return new MomentBoolean(m, par[0].Nd, par[0].Err);
        }

        private static MomentInteger Scalarintiv(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Integer) return new MomentInteger(par[0]);
            return new MomentInteger(0, par[0].Nd, par[0].Err);
        }

        private static MomentInteger Scalarintib(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Integer) return new MomentInteger(par[0]);
            int m = ((MomentBoolean)par[0]).Mean ? 1 : 0;
            return new MomentInteger(m, par[0].Nd, par[0].Err); 
        }

        private static MomentInteger Scalarintii(MomentValue[] par)
        {
            return new MomentInteger(par[0]);
        }

        private static MomentInteger Scalarintir(MomentValue[] par)
        {
            int m = 0; 
            ErrNum err = par[0].Err;
            try
            {
                m = Convert.ToInt32(((MomentReal)par[0]).Mean);
            }
            catch
            {
                err |= 1008;//Ошибка при преобразовании вещественного числа в целое
            }
            return new MomentInteger(m, par[0].Nd, err); 
        }

        private static MomentInteger Scalarintis(MomentValue[] par)
        {
            int m = 0; 
            ErrNum err = par[0].Err;
            try
            {
                m = Convert.ToInt32(((MomentString)par[0]).Mean);
            }
            catch
            {
                err |= 2009;//Ошибка при преобразовании строки в целое число
            }
            return new MomentInteger(m, par[0].Nd, err); 
        }

        private static MomentTime Scalardatedv(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Time) return new MomentTime(par[0]);
            return new MomentTime(par[0].Time, par[0].Nd, par[0].Err);
        }

        private static MomentTime Scalardatedd(MomentValue[] par)
        {
            return new MomentTime(par[0]);
        }

        private static MomentTime Scalardatedr(MomentValue[] par)
        {
            DateTime mean = ((MomentReal)par[0]).Mean.ToDateForCalc();
            return new MomentTime(mean, par[0].Nd, par[0].Err);
        }

        private static MomentTime Scalardateds(MomentValue[] par)
        {
            DateTime m = DateTime.Now;
            ErrNum err = par[0].Err;
            try
            {
                m = Convert.ToDateTime(((MomentString)par[0]).Mean);
            }
            catch
            {
                err |= 2010;//Некорректное преобразование строки в дату
            }
            return new MomentTime( m, par[0].Nd, err);
        }

        private static MomentReal Scalarrealrv(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Real) return new MomentReal(par[0]);
            return new MomentReal( 0, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarrealrb(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Real) return new MomentReal(par[0]);
            double m = ((MomentBoolean)par[0]).Mean ? 1 : 0;
            return new MomentReal( m, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarrealri(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Real) return new MomentReal(par[0]);
            double m = ((MomentInteger)par[0]).Mean;
            return new MomentReal( m, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarrealrd(MomentValue[] par)
        {
            if (par[0].DataType == DataType.Real) return new MomentReal(par[0]);
            double mean = ((MomentTime)par[0]).Mean.ToRealForCalc();
            return new MomentReal(mean, par[0].Nd, par[0].Err);
        }

        private static MomentReal Scalarrealrr(MomentValue[] par)
        {
            return new MomentReal(par[0]);
        }

        private static MomentReal Scalarrealrs(MomentValue[] par)
        {
            double m = 0;
            ErrNum err = par[0].Err;
            try
            {
                m = Convert.ToDouble(((MomentString)par[0]).Mean);
            }
            catch
            {
                err |= 2011;//Ошибка при преобразовании строки в вещественное число
            }
            return new MomentReal(m, par[0].Nd, err);
        }

        private static MomentString Scalarstringsv(MomentValue[] par)
        {
            if (par[0].DataType == DataType.String) return new MomentString(par[0]);
            return new MomentString("", par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstringsb(MomentValue[] par)
        {
            if (par[0].DataType == DataType.String) return new MomentString(par[0]);
            string m = ((MomentBoolean)par[0]).Mean ? "1" : "0";
            return new MomentString( m, par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstringsi(MomentValue[] par)
        {
            if (par[0].DataType == DataType.String) return new MomentString(par[0]);
            string m = Convert.ToString(((MomentInteger)par[0]).Mean);
            return new MomentString( m, par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstringsd(MomentValue[] par)
        {
            if (par[0].DataType == DataType.String) return new MomentString(par[0]);
            string m = Convert.ToString(((MomentTime)par[0]).Mean);
            return new MomentString( m, par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstringsr(MomentValue[] par)
        {
            if (par[0].DataType == DataType.String) return new MomentString(par[0]);
            string m = Convert.ToString(((MomentReal)par[0]).Mean);
            return new MomentString( m, par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstringss(MomentValue[] par)
        {
            return new MomentString(par[0]);
        }

        private static MomentBoolean Scalarisintbr(MomentValue[] par)
        {
            double r = ((MomentReal) par[0]).Mean;
            return new MomentBoolean(Convert.ToInt32(r) == r, par[0].Nd, par[0].Err); 
        }

        private static MomentBoolean Scalarisintbs(MomentValue[] par)
        {
            string s = ((MomentString)par[0]).Mean; int i;
            return new MomentBoolean(Int32.TryParse(s, out i), par[0].Nd, par[0].Err);
        }

        private static MomentBoolean Scalarisrealbs(MomentValue[] par)
        {
            string s = ((MomentString)par[0]).Mean; double d;
            return new MomentBoolean(Double.TryParse(s, out d), par[0].Nd, par[0].Err);
        }

        private static MomentBoolean Scalarisdatebs(MomentValue[] par)
        {
            string s = ((MomentString)par[0]).Mean; DateTime t;
            return new MomentBoolean(DateTime.TryParse(s, out t), par[0].Nd, par[0].Err);
        }

        //12 - Строковая

        private static MomentString Scalarstrmidssi(MomentValue[] par)
        {
            int m1 = ((MomentInteger)par[1]).Mean - 1;
            string m0 = ((MomentString)par[0]).Mean;
            ErrNum err = MaxErrNum(par);
            if (m1 < 0 || m1 >= m0.Length)
            {
                //Недопустимые параметры функции strmid
                err |= 2201;
            }
            else
            {
                m0 = m0.Substring(m1);
            }
            return new MomentString(m0, MaxNd(par), err);
        }

        private static MomentString Scalarstrmidssii(MomentValue[] par)
        {
            int m1 = ((MomentInteger)par[1]).Mean - 1;
            int m2 = ((MomentInteger)par[2]).Mean;
            string m0 = ((MomentString) par[0]).Mean;
            ErrNum err = MaxErrNum(par);
            if (m1 < 0 || m1 + m2 > m0.Length || m2 < 0)
            {
                //Недопустимые параметры функции strmid
                err |= 2201;
            }
            else
            {
                m0 = m0.Substring(m1, m2);    
            }
            return new MomentString(m0, MaxNd(par), err);
        }

        private static MomentString Scalarstrleftssi(MomentValue[] par)
        {
            int m1 = ((MomentInteger)par[1]).Mean;
            string m0 = ((MomentString)par[0]).Mean;
            ErrNum err = MaxErrNum(par);
            if (m1 < 0 || m1 > m0.Length)
            {
                //Недопустимые параметры функции strleft
                err |= 2202;
            }
            else
            {
                m0 = m0.Substring(0, m1);   
            }
            return new MomentString(m0, MaxNd(par), err);
        }

        private static MomentString Scalarstrrightssi(MomentValue[] par)
        {
            int m1 = ((MomentInteger)par[1]).Mean;
            string m0 = ((MomentString)par[0]).Mean;
            ErrNum err = MaxErrNum(par);
            if (m1 < 0 || m1 > m0.Length)
            {
                //Недопустимые параметры функции strright
                err |= 2203;
            }
            else
            {
                m0 = m0.Substring(m0.Length-m1, m1);
            }
            return new MomentString(m0, MaxNd(par), err);
        }

        private static MomentString Scalarstrinsertsssi(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            string m1 = ((MomentString)par[1]).Mean;
            int m2 = ((MomentInteger)par[2]).Mean-1;
            ErrNum err = MaxErrNum(par);
            if (m2 < 0 || m2 >= m0.Length)
            {
                //Недопустимые параметры функции strinsert
                err |= 2206;
            }
            else
            {
                m0 = m0.Insert(m2, m1);
            }
            return new MomentString(m0, MaxNd(par), err);
        }

        private static MomentString Scalarstrremovessi(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            int m1 = ((MomentInteger)par[1]).Mean - 1;
            ErrNum err = MaxErrNum(par);
            if (m1 < 0 || m1 >= m0.Length)
            {
                //Недопустимые параметры функции strremove
                err |= 2207;
            }
            else
            {
                m0 = m0.Remove(m1);
            }
            return new MomentString(m0, MaxNd(par), err);
        }

        private static MomentString Scalarstrremovessii(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            int m1 = ((MomentInteger)par[1]).Mean - 1;
            int m2 = ((MomentInteger)par[2]).Mean;
            ErrNum err = MaxErrNum(par);
            if (m1 < 0 || m1 + m2 > m0.Length || m2 < 0)
            {
                //Недопустимые параметры функции strremove
                err |= 2207;
            }
            else
            {
                m0 = m0.Remove(m1, m2);
            }
            return new MomentString(m0, MaxNd(par), err);
        }

        private static MomentString Scalarstrreplacessss(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            string m1 = ((MomentString)par[1]).Mean;
            string m2 = ((MomentString)par[2]).Mean;
            m0 = m0.Replace(m1, m2);
            return new MomentString(m0, MaxNd(par), MaxErrNum(par));
        }

        private static MomentString Scalarstrreplaceregssss(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            var r1 = new Regex(((MomentString)par[1]).Mean);
            string m2 = ((MomentString)par[2]).Mean;
            m0 = r1.Replace(m0, m2);
            return new MomentString(m0, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarstrlenis(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            return new MomentInteger(m0.Length, MaxNd(par), MaxErrNum(par));
        }

        private static MomentInteger Scalarstrfindissi(MomentValue[] par)
        {
            int m2 = ((MomentInteger)par[2]).Mean-1;
            string m0 = ((MomentString)par[0]).Mean;
            string m1 = ((MomentString)par[1]).Mean;
            ErrNum err=MaxErrNum(par);
            if (m2 < 0 || m2 >= m1.Length)
            {
                //Недопустимые параметры функции strfind
                err |= 2204;
            }
            else
            {
                m2 = m1.IndexOf(m0, m2) + 1;
            }
            return new MomentInteger(m2, MaxNd(par), err);
        }

        private static MomentInteger Scalarstrfindlastissi(MomentValue[] par)
        {
            int m2 = ((MomentInteger)par[2]).Mean - 1;
            string m0 = ((MomentString)par[0]).Mean;
            string m1 = ((MomentString)par[1]).Mean;
            ErrNum err = MaxErrNum(par);
            if (m2 < -1 || m2 >= m1.Length)
            {
                //Недопустимые параметры функции strfindlast
                err |= 2205;
            }
            else
            {
                m2 = (m2 == -1) ? m1.LastIndexOf(m0)+1 : m1.LastIndexOf(m0, m2)+1;
            }
            return new MomentInteger(m2, MaxNd(par), err);
        }

        private static MomentString Scalarstrtrimss(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            return new MomentString(m0.Trim(), par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstrltrimss(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            return new MomentString(m0.TrimStart(), par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstrrtrimss(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            return new MomentString(m0.TrimEnd(), par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstrlcasess(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            return new MomentString(m0.ToLower(), par[0].Nd, par[0].Err);
        }

        private static MomentString Scalarstrucasess(MomentValue[] par)
        {
            string m0 = ((MomentString)par[0]).Mean;
            return new MomentString(m0.ToUpper(), par[0].Nd, par[0].Err);
        }


        //13 - Дата

        private static MomentTime Scalarcurrenttimedu(MomentValue[] par)
        {
            return new MomentTime(par[0].Time, 0, 0);
        }

        private static MomentTime Scalartimeadddsdr(MomentValue[] par)
        {
            DateTime mean = ((MomentTime)par[1]).Mean;
            ErrNum err = MaxErrNum(par);
            try
            {
                switch (((MomentString) par[0]).Mean.ToLower())
                {
                    case "мс":
                    case "ms":
                        mean = mean.AddMilliseconds(((MomentReal) par[2]).Mean);
                        break;
                    case "секунда":
                    case "сек":
                    case "second":
                        mean = mean.AddSeconds(((MomentReal) par[2]).Mean);
                        break;
                    case "минута":
                    case "мин":
                    case "minute":
                        mean = mean.AddMinutes(((MomentReal) par[2]).Mean);
                        break;
                    case "час":
                    case "hour":
                        mean = mean.AddHours(((MomentReal) par[2]).Mean);
                        break;
                    case "день":
                    case "day":
                        mean = mean.AddDays(((MomentReal) par[2]).Mean);
                        break;
                    case "месяц":
                    case "мес":
                    case "month":
                        mean = mean.AddMonths(Convert.ToInt32(((MomentReal) par[2]).Mean));
                        break;
                    case "год":
                    case "year":
                        mean = mean.AddYears(Convert.ToInt32(((MomentReal) par[2]).Mean));
                        break;
                    default:
                        err |= 2301;
                        break;
                }
            }
            catch
            {
                //Недопустимые параметры функции ДобавитьКоВремени (TimeAdd)
                err |= 2301;
            }
            return new MomentTime(mean, MaxNd(par), err);
        }
        
        private static MomentReal Scalartimediffrsdd(MomentValue[] par)
        {
            DateTime m1 = ((MomentTime)par[1]).Mean;
            DateTime m2 = ((MomentTime)par[2]).Mean;
            double mean = 0;
            ErrNum err = MaxErrNum(par);
            try
            {
                switch (((MomentString) par[0]).Mean.ToLower())
                {
                    case "мс":
                    case "ms":
                        mean = m1.Subtract(m2).TotalMilliseconds;
                        break;
                    case "секунда":
                    case "сек":
                    case "second":
                        mean = m1.Subtract(m2).TotalSeconds;
                        break;
                    case "минута":
                    case "мин":
                    case "minute":
                        mean = m1.Subtract(m2).TotalMinutes;
                        break;
                    case "час":
                    case "hour":
                        mean = m1.Subtract(m2).TotalHours;
                        break;
                    case "день":
                    case "day":
                        mean = m1.Subtract(m2).TotalDays;
                        break;
                    default:
                        err |= 2302;
                        break;
                }
            }
            catch
            {
                //Недопустимые параметры функции РазностьВремен (TimeDiff)
                err |= 2302;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentInteger Scalartimepartisd(MomentValue[] par)
        {
            DateTime m1 = ((MomentTime)par[1]).Mean;
            int mean = 0;
            ErrNum err = MaxErrNum(par);
            switch (((MomentString)par[0]).Mean.ToLower())
            {
                case "мс":
                case "ms":
                    mean = m1.Millisecond;
                    break;
                case "секунда":
                case "сек":
                case "second":
                    mean = m1.Second;
                    break;
                case "минута":
                case "мин":
                case "minute":
                    mean = m1.Minute;
                    break;
                case "час":
                case "hour":
                    mean = m1.Hour;
                    break;
                case "день":
                case "day":
                    mean = m1.Day;
                    break;
                case "месяц":
                case "мес":
                case "month":
                    mean = m1.Month;
                    break;
                case "год":
                case "year":
                    mean = m1.Year;
                    break;
                default:
                    //Недопустимые параметры функции ЧастьВремени (TimePart)
                    err |= 2303;
                    break;
            }
            return new MomentInteger(mean, MaxNd(par), err);
        }

        private static MomentTime Scalartimeserialdiiiiiir(MomentValue[] par)
        {
            DateTime mean = DateTime.Now;
            ErrNum err = MaxErrNum(par);
            try
            {
                string d = ((MomentInteger)par[0]).Mean + "." + ((MomentInteger)par[1]).Mean + "." + ((MomentInteger)par[2]).Mean + " " + ((MomentInteger)par[3]).Mean + ":" + ((MomentInteger)par[4]).Mean + ":" + ((MomentInteger)par[5]).Mean;
                mean = Convert.ToDateTime(d);
                mean = mean.AddMilliseconds(((MomentReal)par[6]).Mean);    
            }
            catch
            {
                //Недопустимые параметры функции СобратьВремя (TimeSerial)
                err |= 2304;
            }
            return new MomentTime(mean, MaxNd(par), err);
        }

        // 30 - Системная

        private static int Scalarupdatevalueuuu(FunParams fp, List<MomentValue> res)
        {
            if (fp.Par[0] == null && fp.Par[1] == null) return 0;
            if (fp.Par[0] != null && fp.Par[1] == null)
            {
                res.Add(fp.Par[0].Clone());
                return 0;
            }
            if (fp.Par[0] == null && fp.Par[1] != null)
            {
                res.Add(fp.Par[1].Clone());
                return 0;
            }
            res.Add((fp.Par[1].Undef & ZoneFlag.Filter) > 0 ? fp.Par[0].Clone() : fp.Par[1].Clone());
            return 0;
        }

        private static int Scalarupdatefilteruuu(FunParams fp, List<MomentValue> res)
        {
            if (fp.Par[0] == null && fp.Par[1] == null) return 0;
            if (fp.Par[0] != null && fp.Par[1] == null)
            {
                res.Add(fp.Par[0].Clone());
                return 0;
            }
            if (fp.Par[0] == null && fp.Par[1] != null)
            {
                res.Add(fp.Par[1].Clone());
                return 0;
            }
            MomentValue mv = (fp.Par[1].Undef & ZoneFlag.Filter) > 0 ? fp.Par[0] : fp.Par[1];
            if (res.Count == 0 || res.Last().Undef != mv.Undef)
                res.Add(mv.Clone());
            return 0;
        }

        private static int Scalarifgotovbi(FunParams fp, List<MomentValue> res)
        {
            int undef = MaxUndef(fp.Par);
            if (!((MomentBoolean)fp.Par[fp.Par.Length-2]).Mean) undef |= ZoneFlag.Filter;
            res.Add(new MomentValue(fp.Time, MaxNd(fp.Par), MaxErrNum(fp.Par), undef));    
            return 0;
        }

        private static int Scalarifnotgotovbi(FunParams fp, List<MomentValue> res)
        {
            int undef = MaxUndef(fp.Par);
            if (((MomentBoolean)fp.Par[fp.Par.Length - 2]).Mean) undef |= ZoneFlag.Filter;
            res.Add(new MomentValue(fp.Time, MaxNd(fp.Par), MaxErrNum(fp.Par), undef));
            return 0;
        }

        //20 - Свойства газов

        private static string GazSpecification(MomentValue[] par, int n)
        {
            string res = "";
            for (int i=n;i<par.Length;i+=2)
            {
                res += ((MomentString) par[i]).Mean + ":" + ((MomentReal) par[i+1]).Mean + ";";
            }
            return res;
        }

        private static MomentReal Scalarwspgcpidtrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgCPIDT(((MomentInteger)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3001;
            }
            catch
            {
                err |= 3001;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgcpgstrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgCPGST(((MomentString)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3002;
            }
            catch
            {
                err |= 3002;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgcpgstrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgCPGST(GazSpecification(par, 1), ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3002;
            }
            catch
            {
                err |= 3002;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspgcvidtrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgCVIDT(((MomentInteger)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3003;
            }
            catch
            {
                err |= 3003;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgcvgstrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgCVGST(((MomentString)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3004;
            }
            catch
            {
                err |= 3004;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgcvgstrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgCVGST(GazSpecification(par, 1), ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3004;
            }
            catch
            {
                err |= 3004;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }
        
        private static MomentReal Scalarwspghidtrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgHIDT(((MomentInteger)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3007;
            }
            catch
            {
                err |= 3007;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspghgstrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgHGST(((MomentString)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3008;
            }
            catch
            {
                err |= 3008;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspghgstrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgHGST(GazSpecification(par, 1), ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3008;
            }
            catch
            {
                err |= 3008;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgtidhrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgTIDH(((MomentInteger)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3017;
            }
            catch
            {
                err |= 3017;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgtgshrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgTGSH(((MomentString)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3018;
            }
            catch
            {
                err |= 3018;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgtgshrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgTGSH(GazSpecification(par, 1), ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3018;
            }
            catch
            {
                err |= 3018;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        
        private static MomentReal Scalarwspguidtrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgUIDT(((MomentInteger)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3021;
            }
            catch
            {
                err |= 3021;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgugstrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgUGST(((MomentString)par[1]).Mean, ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3022;
            }
            catch
            {
                err |= 3022;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgugstrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgUGST(GazSpecification(par, 1), ((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3022;
            }
            catch
            {
                err |= 3022;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspggcidri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgGCID(((MomentInteger)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3005;
            }
            catch
            {
                err |= 3005;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspggcgsrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgGCGS(((MomentString)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3006;
            }
            catch
            {
                err |= 3006;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspggcgsrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgGCGS(GazSpecification(par, 0));
                if (Oka.wspGETLASTERROR() != 0) err |= 3006;
            }
            catch
            {
                err |= 3006;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspgmmidri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgMMID(((MomentInteger)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3011;
            }
            catch
            {
                err |= 3011;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgmmgsrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgMMGS(((MomentString)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3012;
            }
            catch
            {
                err |= 3012;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgmmgsrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgMMGS(GazSpecification(par, 0));
                if (Oka.wspGETLASTERROR() != 0) err |= 3012;
            }
            catch
            {
                err |= 3012;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspgmfididrii(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgMFIDID(((MomentInteger)par[0]).Mean, ((MomentInteger)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3009;
            }
            catch
            {
                err |= 3009;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgmfgsgsrss(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgMFGSGS(((MomentString)par[0]).Mean, ((MomentString)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3010;
            }
            catch
            {
                err |= 3010;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspgvfididrii(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgVFIDID(((MomentInteger)par[0]).Mean, ((MomentInteger)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3023;
            }
            catch
            {
                err |= 3023;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgvfgsgsrss(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgVFGSGS(((MomentString)par[0]).Mean, ((MomentString)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3024;
            }
            catch
            {
                err |= 3024;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspgpidtsrrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgPIDTS(((MomentInteger)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3013;
            }
            catch
            {
                err |= 3013;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgpgstsrrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgPGSTS(((MomentString)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3014;
            }
            catch
            {
                err |= 3014;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgpgstsrrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgPGSTS(GazSpecification(par, 2), ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3014;
            }
            catch
            {
                err |= 3014;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspgsidptrrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgSIDPT(((MomentInteger)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3015;
            }
            catch
            {
                err |= 3015;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgsgsptrrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgSGSPT(((MomentString)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3016;
            }
            catch
            {
                err |= 3016;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgsgsptrrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgSGSPT(GazSpecification(par, 2), ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3016;
            }
            catch
            {
                err |= 3016;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }


        private static MomentReal Scalarwspgtidpsrrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgTIDPS(((MomentInteger)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3019;
            }
            catch
            {
                err |= 3019;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgtgspsrrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgTGSPS(((MomentString)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3020;
            }
            catch
            {
                err |= 3020;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgtgspsrrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgTGSPS(GazSpecification(par, 2), ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3020;
            }
            catch
            {
                err |= 3020;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgvidptrrri(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgVIDPT(((MomentInteger)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3025;
            }
            catch
            {
                err |= 3025;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgvgsptrrrs(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgVGSPT(((MomentString)par[2]).Mean, ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3026;
            }
            catch
            {
                err |= 3026;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspgvgsptrrrsr(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspgVGSPT(GazSpecification(par, 2), ((MomentReal)par[0]).Mean, ((MomentReal)par[1]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3026;
            }
            catch
            {
                err |= 3026;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        // 21 - ВодаПар
        private static MomentReal Scalarwspsurften(MomentValue[] par)
        {
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                mean = Oka.wspSURFTENT(((MomentReal)par[0]).Mean);
                if (Oka.wspGETLASTERROR() != 0) err |= 3101;
            }
            catch
            {
                err |= 3101;
            }
            return new MomentReal(mean, MaxNd(par), err);
        }

        private static MomentReal Scalarwspv(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspVPT(((MomentReal) par[1]).Mean, ((MomentReal) par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspVPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspVPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspVPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspVEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspVEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspVHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3102;
                            break;
                    }
                } 
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspVSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspVSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspVSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspVMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspV1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspV2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspV3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspV3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3ps":
                            mean = Oka.wspV3PS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3ph":
                            mean = Oka.wspV3PH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3hs":
                            mean = Oka.wspV3HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspV5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3102;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3102;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3102);
            }
        }

        private static MomentReal Scalarwspu(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspUPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspUPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspUPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspUPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspUEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspUEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspUHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3103;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspUSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspUSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspUSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspUMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspU1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspU2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspU3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspU3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspU5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3103;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3103;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3103);
            }
        }

        private static MomentReal Scalarwsps(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspSPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspSPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspSPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspSEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspSEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        default:
                            err |= 3104;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspSSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspSSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspSSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspSMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspS1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspS2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspS3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspS3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspS5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3104;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3104;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3104);
            }
        }

        private static MomentReal Scalarwsph(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspHPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspHPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspHPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspHEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspHEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        default:
                            err |= 3105;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspHSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspHSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspHSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspHMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspH1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspH2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspH3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspH3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspH5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2b2cp":
                            mean = Oka.wspH2B2CP(((MomentReal)par[2]).Mean);
                            break;
                        case "b13":
                            mean = Oka.wspHB13S(((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3105;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3105;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3105);
            }
        }

        private static MomentReal Scalarwspcv(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspCVPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspCVPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspCVPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspCVPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspCVEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspCVEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspCVHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3106;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspCVSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspCVSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspCVSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspCVMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspCV1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspCV2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspCV3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspCV3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspCV5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3106;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3106;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3106);
            }
        }

        private static MomentReal Scalarwspcp(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspCPPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspCPPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspCPPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspCPPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspCPEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspCPEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspCPHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3107;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspCPSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspCPSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspCPSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspCPMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspCP1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspCP2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspCP3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspCP3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspCP5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3107;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3107;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3107);
            }
        }
        
        private static MomentReal Scalarwspw(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspWPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspWPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspWPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspWPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspWEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspWEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspWHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3122;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspWSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspWSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspWSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspWMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspW1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspW2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspW3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspW3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspW5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3122;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3122;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3122);
            }
        }

        private static MomentReal Scalarwspjoulethompson(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspJOULETHOMPSONPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspJOULETHOMPSONPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspJOULETHOMPSONPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspJOULETHOMPSONPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspJOULETHOMPSONEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspJOULETHOMPSONEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspJOULETHOMPSONHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3108;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspJOULETHOMPSONSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspJOULETHOMPSONSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspJOULETHOMPSONSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspJOULETHOMPSONMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1pt":
                            mean = Oka.wspJOULETHOMPSON1PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2pt":
                            mean = Oka.wspJOULETHOMPSON2PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3pt":
                            mean = Oka.wspJOULETHOMPSON3PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rt":
                            mean = Oka.wspJOULETHOMPSON3RT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5pt":
                            mean = Oka.wspJOULETHOMPSON5PT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3108;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3108;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3108);
            }
        }

        private static MomentReal Scalarwspthermcond(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspTHERMCONDPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspTHERMCONDPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspTHERMCONDPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspTHERMCONDPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspTHERMCONDEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspTHERMCONDEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspTHERMCONDHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3109;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspTHERMCONDSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspTHERMCONDSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspTHERMCONDSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspTHERMCONDMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3109;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3109;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3109);
            }
        }

        private static MomentReal Scalarwspdynvis(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspDYNVISPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspDYNVISPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspDYNVISPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspDYNVISPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspDYNVISEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspDYNVISEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspDYNVISHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3110;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspDYNVISSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspDYNVISSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspDYNVISSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspDYNVISMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3110;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3110;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3110);
            }
        }

        private static MomentReal Scalarwspprandtle(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspPRANDTLEPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspPRANDTLEPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspPRANDTLEPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspPRANDTLEPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspPRANDTLEEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspPRANDTLEEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspPRANDTLEHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3111;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspPRANDTLESST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspPRANDTLESWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspPRANDTLESTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspPRANDTLEMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3111;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3111;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3111);
            }
        }

        private static MomentReal Scalarwspkinvis(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspKINVISPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspKINVISPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspKINVISPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspKINVISPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspKINVISEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspKINVISEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspKINVISHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3112;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspKINVISSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspKINVISSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspKINVISSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspKINVISMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3112;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3112;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3112);
            }
        }

        private static MomentReal Scalarwspk(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "pt":
                            mean = Oka.wspKPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptx":
                            mean = Oka.wspKPTX(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspKPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspKPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspKEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspKEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspKHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3113;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sst":
                            mean = Oka.wspKSST(((MomentReal)par[2]).Mean);
                            break;
                        case "swt":
                            mean = Oka.wspKSWT(((MomentReal)par[2]).Mean);
                            break;
                        case "stx":
                            mean = Oka.wspKSTX(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "mspt":
                            mean = Oka.wspKMSPT(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3113;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3113;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3113);
            }
        }

        private static MomentReal Scalarwspt(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "ph":
                            mean = Oka.wspTPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspTPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspTEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspTEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "hs":
                            mean = Oka.wspTHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3114;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "sp":
                            mean = Oka.wspTSP(((MomentReal)par[2]).Mean);
                            break;
                        case "shs":
                            mean = Oka.wspTSHS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1hs":
                            mean = Oka.wspT1HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1ph":
                            mean = Oka.wspT1PH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "1ps":
                            mean = Oka.wspT1PS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2hs":
                            mean = Oka.wspT2HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2ph":
                            mean = Oka.wspT2PH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2ps":
                            mean = Oka.wspT2PS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2aph":
                            mean = Oka.wspT2APH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2aps":
                            mean = Oka.wspT2APS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2bph":
                            mean = Oka.wspT2BPH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2bps":
                            mean = Oka.wspT2BPS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2cph":
                            mean = Oka.wspT2CPH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2cps":
                            mean = Oka.wspT2CPS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "23p":
                            mean = Oka.wspT23P(((MomentReal)par[2]).Mean);
                            break;
                        case "3hs":
                            mean = Oka.wspT3HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3ph":
                            mean = Oka.wspT3PH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3ps":
                            mean = Oka.wspT3PS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "3rh":
                            mean = Oka.wspT3RH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5hs":
                            mean = Oka.wspT5HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5ph":
                            mean = Oka.wspT5PH(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5ps":
                            mean = Oka.wspT5PS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "b23hs":
                            mean = Oka.wspTB23HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3114;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3114;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3114);
            }
        }

        private static MomentReal Scalarwspp(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "hs":
                            mean = Oka.wspTHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3115;
                            break;
                    }
                }
                else
                {
                    switch ((((MomentString)par[0]).Mean + ((MomentString)par[1]).Mean).ToLower())
                    {
                        case "st":
                            mean = Oka.wspPST(((MomentReal)par[2]).Mean);
                            break;
                        case "subt":
                            mean = Oka.wspPSUBT(((MomentReal)par[2]).Mean);
                            break;
                        case "melitt":
                            mean = Oka.wspPMELTIT(((MomentReal)par[2]).Mean);
                            break;
                        case "1hs":
                            mean = Oka.wspP1HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "2hs":
                            mean = Oka.wspP2HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "23t":
                            mean = Oka.wspP23T(((MomentReal)par[2]).Mean);
                            break;
                        case "2b2ch":
                            mean = Oka.wspP2B2CH(((MomentReal)par[2]).Mean);
                            break;
                        case "3hs":
                            mean = Oka.wspP3HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "5hs":
                            mean = Oka.wspP5HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        case "b23hs":
                            mean = Oka.wspPB23HS(((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean);
                            break;
                        default:
                            err |= 3115;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3115;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3115);
            }
        }

        private static MomentReal Scalarwspx(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "hs":
                            mean = Oka.wspXHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ph":
                            mean = Oka.wspXPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ps":
                            mean = Oka.wspXPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "ptpeff":
                            mean = Oka.wspXEXPANSIONPTPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        case "ptxpeff":
                            mean = Oka.wspXEXPANSIONPTXPEFF(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean, ((MomentReal)par[3]).Mean, ((MomentReal)par[4]).Mean, ((MomentReal)par[4]).Mean);
                            break;
                        default:
                            err |= 3116;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3116;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3116);
            }
        }

        private static MomentReal Scalarwspwaterstatearea(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "PT":
                            mean = Oka.wspWATERSTATEAREA(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "2PT":
                            mean = Oka.wspWATERSTATEAREA2(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "HS":
                            mean = Oka.wspWATERSTATEAREAHS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "PH":
                            mean = Oka.wspWATERSTATEAREAPH(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        case "PS":
                            mean = Oka.wspWATERSTATEAREAPS(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3120;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3120;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3120);
            }
        }

        private static MomentReal Scalarwspphasestate(MomentValue[] par)
        {
            int nd = MaxNd(par);
            ErrNum err = MaxErrNum(par);
            double mean = 0;
            try
            {
                if (par[1].DataType != DataType.String)
                {
                    switch (((MomentString)par[0]).Mean.ToLower())
                    {
                        case "PT":
                            mean = Oka.wspPHASESTATEPT(((MomentReal)par[1]).Mean, ((MomentReal)par[2]).Mean);
                            break;
                        default:
                            err |= 3121;
                            break;
                    }
                }
                if (Oka.wspGETLASTERROR() != 0) err |= 3121;
                return new MomentReal(mean, nd, err);
            }
            catch
            {
                return new MomentReal(0, nd, err | 3121);
            }
        }
    }
    
}