using System;
using CommonTypes;

namespace Calculation
{
    public partial class Funs
    {
        //Свойства газов

        private  string GazSpecification(Moment[] par, int n)
        {
            string res = "";
            for (int i = n; i < par.Length; i += 2)
                res += par[i].String + ":" + par[i + 1].String + ";";
            return res;
        }

        private const string _wsperr = "Ошибка вычисления значения функции ";

        public  void wspgcptri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgCPIDT(par[1].Integer, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgCPT", res);
            }
            catch { PutErr(_wsperr + "wspgCPT", res); }
        }

        public  void wspgcptrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgCPGST(par[1].String, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgCPT", res);
            }
            catch { PutErr(_wsperr + "wspgCPT", res); }
        }

        public  void wspgcptrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgCPGST(GazSpecification(par, 1), par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgCPT", res);
            }
            catch { PutErr(_wsperr + "wspgCPT", res); }
        }

        public  void wspgcvtri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgCVIDT(par[1].Integer, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgCVT", res);
            }
            catch { PutErr(_wsperr + "wspgCVT", res); }
        }

        public  void wspgcvtrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgCVGST(par[1].String, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgCVT", res);
            }
            catch { PutErr(_wsperr + "wspgCVT", res); }
        }

        public  void wspgcvtrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgCVGST(GazSpecification(par, 1), par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgCVT", res);
            }
            catch { PutErr(_wsperr + "wspgCVT", res); }
        }

        public  void wspghtri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgHIDT(par[1].Integer, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgHT", res);
            }
            catch { PutErr(_wsperr + "wspgHT", res); }
        }

        public  void wspghtrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgHGST(par[1].String, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgHT", res);
            }
            catch { PutErr(_wsperr + "wspgHT", res); }
        }

        public  void wspghtrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgHGST(GazSpecification(par, 1), par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgHT", res);
            }
            catch { PutErr(_wsperr + "wspgHT", res); }
        }

        public  void wspgutri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgUIDT(par[1].Integer, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgUT", res);
            }
            catch { PutErr(_wsperr + "wspgUT", res); }
        }

        public  void wspgutrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgUGST(par[1].String, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgUT", res);
            }
            catch { PutErr(_wsperr + "wspgUT", res); }
        }

        public  void wspgutrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgUGST(GazSpecification(par, 1), par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgUT", res);
            }
            catch { PutErr(_wsperr + "wspgUT", res); }
        }

        public  void wspgthri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgTIDH(par[1].Integer, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgTH", res);
            }
            catch { PutErr(_wsperr + "wspgTH", res); }
        }

        public  void wspgthrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgTGSH(par[1].String, par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgTH", res);
            }
            catch { PutErr(_wsperr + "wspgTH", res); }
        }

        public  void wspgthrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgTGSH(GazSpecification(par, 1), par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgTH", res);
            }
            catch { PutErr(_wsperr + "wspgTH", res); }
        }

        public  void wspggci(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgGCID(par[0].Integer);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgGC", res);
            }
            catch { PutErr(_wsperr + "wspgGC", res); }
        }

        public  void wspggcs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgGCGS(par[0].String);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgGC", res);
            }
            catch { PutErr(_wsperr + "wspgGC", res); }
        }

        public  void wspggcsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgGCGS(GazSpecification(par, 0));
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgGC", res);
            }
            catch { PutErr(_wsperr + "wspgGC", res); }
        }

        public  void wspgmmi(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgMMID(par[0].Integer);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgMM", res);
            }
            catch { PutErr(_wsperr + "wspgMM", res); }
        }

        public  void wspgmms(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgMMGS(par[0].String);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgMM", res);
            }
            catch { PutErr(_wsperr + "wspgMM", res); }
        }

        public  void wspgmmsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgMMGS(GazSpecification(par, 0));
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgMM", res);
            }
            catch { PutErr(_wsperr + "wspgMM", res); }
        }

        public  void wspgmfii(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgMFIDID(par[0].Integer, par[1].Integer);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgMF", res);
            }
            catch { PutErr(_wsperr + "wspgMF", res); }
        }

        public  void wspgmfss(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgMFGSGS(par[0].String, par[1].String);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgMF", res);
            }
            catch { PutErr(_wsperr + "wspgMF", res); }
        }

        public  void wspgvfii(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgVFIDID(par[0].Integer, par[1].Integer);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgVF", res);
            }
            catch { PutErr(_wsperr + "wspgVF", res); }
        }

        public  void wspgvfss(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgVFGSGS(par[0].String, par[1].String);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgVF", res);
            }
            catch { PutErr(_wsperr + "wspgVF", res); }
        }

        public  void wspgptsrri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgPIDTS(par[2].Integer, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgPTS", res);
            }
            catch { PutErr(_wsperr + "wspgPTS", res); }
        }

        public  void wspgptsrrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgPGSTS(par[2].String, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgPTS", res);
            }
            catch { PutErr(_wsperr + "wspgPTS", res); }
        }

        public  void wspgptsrrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgPGSTS(GazSpecification(par, 2), par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgPTS", res);
            }
            catch { PutErr(_wsperr + "wspgPTS", res); }
        }

        public  void wspgsptrri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgSIDPT(par[2].Integer, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgSPT", res);
            }
            catch { PutErr(_wsperr + "wspgSPT", res); }
        }

        public  void wspgsptrrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgSGSPT(par[2].String, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgSPT", res);
            }
            catch { PutErr(_wsperr + "wspgSPT", res); }
        }

        public  void wspgsptrrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgSGSPT(GazSpecification(par, 2), par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgSPT", res);
            }
            catch { PutErr(_wsperr + "wspgSPT", res); }
        }

        public  void wspgtpsrri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgTIDPS(par[2].Integer, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgTPS", res);
            }
            catch { PutErr(_wsperr + "wspgTPS", res); }
        }

        public  void wspgtpsrrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgTGSPS(par[2].String, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgTPS", res);
            }
            catch { PutErr(_wsperr + "wspgTPS", res); }
        }

        public  void wspgtpsrrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgTGSPS(GazSpecification(par, 2), par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgTPS", res);
            }
            catch { PutErr(_wsperr + "wspgTPS", res); }
        }

        public  void wspgvptrri(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgVIDPT(par[2].Integer, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgVPT", res);
            }
            catch { PutErr(_wsperr + "wspgVPT", res); }
        }

        public  void wspgvptrrs(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgVGSPT(par[2].String, par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgVPT", res);
            }
            catch { PutErr(_wsperr + "wspgVPT", res); }
        }

        public  void wspgvptrrsr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspgVGSPT(GazSpecification(par, 2), par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspgVPT", res);
            }
            catch { PutErr(_wsperr + "wspgVPT", res); }
        }

        //WSP - параметры

        public void wspgN2(Moment[] par, Moment res)
        {
            res.Integer = 0;
        }
        public void wspgO2(Moment[] par, Moment res)
        {
            res.Integer = 1;
        }
        public void wspgCO(Moment[] par, Moment res)
        {
            res.Integer = 2;
        }
        public void wspgCO2(Moment[] par, Moment res)
        {
            res.Integer = 3;
        }
        public void wspgH2O(Moment[] par, Moment res)
        {
            res.Integer = 4;
        }
        public void wspgSO2(Moment[] par, Moment res)
        {
            res.Integer = 5;
        }
        public void wspgAir(Moment[] par, Moment res)
        {
            res.Integer = 6;
        }
        public void wspgN2Air(Moment[] par, Moment res)
        {
            res.Integer = 7;
        }
        public void wspgNO(Moment[] par, Moment res)
        {
            res.Integer = 8;
        }
        public void wspgNO2(Moment[] par, Moment res)
        {
            res.Integer = 9;
        }
        public void wspgAr(Moment[] par, Moment res)
        {
            res.Integer = 10;
        }
        public void wspgNe(Moment[] par, Moment res)
        {
            res.Integer = 11;
        }
        public void wspgH2(Moment[] par, Moment res)
        {
            res.Integer = 12;
        }
        public void wspgAirMix(Moment[] par, Moment res)
        {
            res.Integer = 13;
        }
        public void wspgN2AirMix(Moment[] par, Moment res)
        {
            res.Integer = 14;
        }


        //Количество параметров в списке параметров, заданном строкой
        private int WpsParamsNum(string pars)
        {
            switch (pars)
            {
                case "sst":
                case "swt":
                case "melitt":
                case "st":
                case "subt":
                case "sp":
                    return 1;
                case "hs":
                case "pt":
                case "ph":
                case "ps":
                case "stx":
                case "mspt":
                case "shs":
                    return 2;
                case "ptx":
                    return 3;
                case "ptpeff":
                    return 4;
                case "ptxpeff":
                    return 5;
            }
            return 0;
        }

        public void wspphs(Moment[] par, Moment res)
        {
            res.String = "hs";
        }
        public void wsppmeltit(Moment[] par, Moment res)
        {
            res.String = "meltit";
        }
        public void wsppmspt(Moment[] par, Moment res)
        {
            res.String = "mspt";
        }
        public  void wsppph(Moment[] par, Moment res)
        {
            res.String = "ph";
        }
        public void wsppps(Moment[] par, Moment res)
        {
            res.String = "ps";
        }
        public void wspppt(Moment[] par, Moment res)
        {
            res.String = "pt";
        }
        public void wsppptpeff(Moment[] par, Moment res)
        {
            res.String = "ptpeff";
        }
        public void wsppptx(Moment[] par, Moment res)
        {
            res.String = "ptx";
        }

        public void wsppptxpeff(Moment[] par, Moment res)
        {
            res.String = "ptxpeff";
        }
        public void wsppshs(Moment[] par, Moment res)
        {
            res.String = "shs";
        }
        public void wsppsp(Moment[] par, Moment res)
        {
            res.String = "sp";
        }
        public void wsppsst(Moment[] par, Moment res)
        {
            res.String = "sst";
        }
        public void wsppst(Moment[] par, Moment res)
        {
            res.String = "st";
        }
        public void wsppstx(Moment[] par, Moment res)
        {
            res.String = "stx";
        }
        public void wsppsubt(Moment[] par, Moment res)
        {
            res.String = "subt";
        }
        public void wsppswt(Moment[] par, Moment res)
        {
            res.String = "swt";
        }

        // ВодаПар
        //
        public  void wspsurftenr(Moment[] par, Moment res)
        {
            try
            {
                res.Real = Oka.wspSURFTENT(par[0].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspSurftent", res);
            }
            catch { PutErr(_wsperr + "wspSurftent", res); }
        }

        public  void wspphasestaterr(Moment[] par, Moment res)
        {
            try
            {
                res.Integer = Oka.wspPHASESTATEPT(par[0].Real, par[1].Real);
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + "wspPhaseState", res);
            }
            catch { PutErr(_wsperr + "wspPhaseState", res); }
        }

        //Общая функция для вызова функций по пару и воде, funName - имя функции WSP
        private void WspFun(Moment[] par, Moment res, string funName, Func<string, Moment[], double> deleg)
        {
            string f = "wsp" + funName;
            try
            {
                string s = par[0].String.ToLower();
                if (WpsParamsNum(s) != par.Length-1) PutErr("Недопустимое количество параметров функции " + f, res);
                else
                {
                    double d = deleg(s, par);
                    if (double.IsNaN(d)) PutErr(_wsperr + f, res);
                    else res.Real = d;
                }
                if (Oka.wspGETLASTERROR() != 0) PutErr(_wsperr + f, res);
            }
            catch { PutErr(_wsperr + f, res); }
        }

        private double WspV(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspVPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspVPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspVPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspVPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspVEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspVEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspVHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspVSST(par[1].Real);
                case "swt":
                    return Oka.wspVSWT(par[1].Real);
                case "stx":
                    return Oka.wspVSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspVMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspvsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "V", WspV);
        }
        public void wspvsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "V", WspV);
        }
        public void wspvsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "V", WspV);
        }
        public void wspvsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "V", WspV);
        }
        public void wspvsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "V", WspV);
        }

        private double WspU(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspUPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspUPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspUPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspUPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspUEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspUEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspUHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspUSST(par[1].Real);
                case "swt":
                    return Oka.wspUSWT(par[1].Real);
                case "stx":
                    return Oka.wspUSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspUMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspusr(Moment[] par, Moment res)
        {
            WspFun(par, res, "U", WspU);
        }
        public void wspusrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "U", WspU);
        }
        public void wspusrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "U", WspU);
        }
        public void wspusrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "U", WspU);
        }
        public  void wspusrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "U", WspU);
        }

        private double WspS(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspSPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspSPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspSPH(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspSEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspSEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "sst":
                    return Oka.wspSSST(par[1].Real);
                case "swt":
                    return Oka.wspSSWT(par[1].Real);
                case "stx":
                    return Oka.wspSSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspSMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspssr(Moment[] par, Moment res)
        {
            WspFun(par, res, "S", WspS);
        }
        public void wspssrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "S", WspS);
        }
        public void wspssrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "S", WspS);
        }
        public void wspssrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "S", WspS);
        }
        public void wspssrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "S", WspS);
        }

        private double WspH(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspHPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspHPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ps":
                    return Oka.wspHPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspHEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspHEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "sst":
                    return Oka.wspHSST(par[1].Real);
                case "swt":
                    return Oka.wspHSWT(par[1].Real);
                case "stx":
                    return Oka.wspHSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspHMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public  void wsphsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "H", WspH);
        }
        public void wsphsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "H", WspH);
        }
        public void wsphsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "H", WspH);
        }
        public void wsphsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "H", WspH);
        }
        public void wsphsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "H", WspH);
        }

        private double WspCV(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspCVPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspCVPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspCVPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspCVPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspCVEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspCVEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspCVHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspCVSST(par[1].Real);
                case "swt":
                    return Oka.wspCVSWT(par[1].Real);
                case "stx":
                    return Oka.wspCVSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspCVMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }


        public void wspcvsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CV", WspCV);
        }
        public void wspcvsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CV", WspCV);
        }
        public void wspcvsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CV", WspCV);
        }
        public void wspcvsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CV", WspCV);
        }
        public  void wspcvsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CV", WspCV);
        }

        private double WspCP(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspCPPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspCPPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspCPPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspCPPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspCPEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspCPEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspCPHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspCPSST(par[1].Real);
                case "swt":
                    return Oka.wspCPSWT(par[1].Real);
                case "stx":
                    return Oka.wspCPSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspCPMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspcpsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CP", WspCP);
        }
        public void wspcpsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CP", WspCP);
        }
        public void wspcpsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CP", WspCP);
        }
        public void wspcpsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CP", WspCP);
        }
        public void wspcpsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "CP", WspCP);
        }

        private double WspW(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspWPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspWPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspWPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspWPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspWEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspWEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspWHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspWSST(par[1].Real);
                case "swt":
                    return Oka.wspWSWT(par[1].Real);
                case "stx":
                    return Oka.wspWSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspWMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspwsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "W", WspW);
        }
        public void wspwsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "W", WspW);
        }
        public void wspwsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "W", WspW);
        }
        public void wspwsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "W", WspW);
        }
        public void wspwsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "W", WspW);
        }

        private double WspJOULETHOMPSON(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspJOULETHOMPSONPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspJOULETHOMPSONPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspJOULETHOMPSONPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspJOULETHOMPSONPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspJOULETHOMPSONEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspJOULETHOMPSONEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspJOULETHOMPSONHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspJOULETHOMPSONSST(par[1].Real);
                case "swt":
                    return Oka.wspJOULETHOMPSONSWT(par[1].Real);
                case "stx":
                    return Oka.wspJOULETHOMPSONSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspJOULETHOMPSONMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspjoulethompsonsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "JOULETHOMPSON", WspJOULETHOMPSON);
        }
        public void wspjoulethompsonsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "JOULETHOMPSON", WspJOULETHOMPSON);
        }
        public void wspjoulethompsonsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "JOULETHOMPSON", WspJOULETHOMPSON);
        }
        public void wspjoulethompsonsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "JOULETHOMPSON", WspJOULETHOMPSON);
        }
        public void wspjoulethompsonsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "JOULETHOMPSON", WspJOULETHOMPSON);
        }

        private double WspTHERMCOND(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspTHERMCONDPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspTHERMCONDPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspTHERMCONDPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspTHERMCONDPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspTHERMCONDEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspTHERMCONDEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspTHERMCONDHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspTHERMCONDSST(par[1].Real);
                case "swt":
                    return Oka.wspTHERMCONDSWT(par[1].Real);
                case "stx":
                    return Oka.wspTHERMCONDSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspTHERMCONDMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspthermcondsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "THERMCOND", WspTHERMCOND);
        }
        public void wspthermcondsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "THERMCOND", WspTHERMCOND);
        }
        public void wspthermcondsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "THERMCOND", WspTHERMCOND);
        }
        public void wspthermcondsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "THERMCOND", WspTHERMCOND);
        }
        public void wspthermcondsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "THERMCOND", WspTHERMCOND);
        }

        private double WspDYNVIS(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspDYNVISPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspDYNVISPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspDYNVISPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspDYNVISPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspDYNVISEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspDYNVISEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspDYNVISHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspDYNVISSST(par[1].Real);
                case "swt":
                    return Oka.wspDYNVISSWT(par[1].Real);
                case "stx":
                    return Oka.wspDYNVISSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspDYNVISMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspdynvissr(Moment[] par, Moment res)
        {
            WspFun(par, res, "DYNVIS", WspDYNVIS);
        }
        public void wspdynvissrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "DYNVIS", WspDYNVIS);
        }
        public void wspdynvissrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "DYNVIS", WspDYNVIS);
        }
        public void wspdynvissrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "DYNVIS", WspDYNVIS);
        }
        public void wspdynvissrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "DYNVIS", WspDYNVIS);
        }

        private double WspPRANDTLE(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspPRANDTLEPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspPRANDTLEPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspPRANDTLEPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspPRANDTLEPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspPRANDTLEEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspPRANDTLEEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspPRANDTLEHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspPRANDTLESST(par[1].Real);
                case "swt":
                    return Oka.wspPRANDTLESWT(par[1].Real);
                case "stx":
                    return Oka.wspPRANDTLESTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspPRANDTLEMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspprandtlesr(Moment[] par, Moment res)
        {
            WspFun(par, res, "PRANDTLE", WspPRANDTLE);
        }
        public void wspprandtlesrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "PRANDTLE", WspPRANDTLE);
        }
        public void wspprandtlesrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "PRANDTLE", WspPRANDTLE);
        }
        public void wspprandtlesrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "PRANDTLE", WspPRANDTLE);
        }
        public void wspprandtlesrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "PRANDTLE", WspPRANDTLE);
        }

        private double WspKINVIS(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspKINVISPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspKINVISPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspKINVISPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspKINVISPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspKINVISEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspKINVISEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspKINVISHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspKINVISSST(par[1].Real);
                case "swt":
                    return Oka.wspKINVISSWT(par[1].Real);
                case "stx":
                    return Oka.wspKINVISSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspKINVISMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspkinvissr(Moment[] par, Moment res)
        {
            WspFun(par, res, "KINVIS", WspKINVIS);
        }
        public void wspkinvissrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "KINVIS", WspKINVIS);
        }
        public void wspkinvissrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "KINVIS", WspKINVIS);
        }
        public void wspkinvissrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "KINVIS", WspKINVIS);
        }
        public void wspkinvissrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "KINVIS", WspKINVIS);
        }

        private double WspK(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "pt":
                    return Oka.wspKPT(par[1].Real, par[2].Real);
                case "ptx":
                    return Oka.wspKPTX(par[1].Real, par[2].Real, par[3].Real);
                case "ph":
                    return Oka.wspKPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspKPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspKEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspKEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspKHS(par[1].Real, par[2].Real);
                case "sst":
                    return Oka.wspKSST(par[1].Real);
                case "swt":
                    return Oka.wspKSWT(par[1].Real);
                case "stx":
                    return Oka.wspKSTX(par[1].Real, par[2].Real);
                case "mspt":
                    return Oka.wspKMSPT(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wspksr(Moment[] par, Moment res)
        {
            WspFun(par, res, "K", WspK);
        }
        public void wspksrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "K", WspK);
        }
        public void wspksrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "K", WspK);
        }
        public void wspksrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "K", WspK);
        }
        public void wspksrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "K", WspK);
        }

        private double WspT(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "ph":
                    return Oka.wspTPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspTPS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspTEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspTEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
                case "hs":
                    return Oka.wspTHS(par[1].Real, par[2].Real);
                case "sp":
                    return Oka.wspTSP(par[1].Real);
                case "shs":
                    return Oka.wspTSHS(par[1].Real, par[2].Real);
            }
            return double.NaN;
        }

        public void wsptsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "T", WspT);
        }
        public void wsptsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "T", WspT);
        }
        public void wsptsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "T", WspT);
        }
        public void wsptsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "T", WspT);
        }
        public void wsptsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "T", WspT);
        }

        private double WspP(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "hs":
                    return Oka.wspPHS(par[1].Real, par[2].Real);
                case "st":
                    return Oka.wspPST(par[1].Real);
                case "subt":
                    return Oka.wspPSUBT(par[1].Real);
                case "meltit":
                    return Oka.wspPMELTIT(par[1].Real);
            }
            return double.NaN;
        }

        public void wsppsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "P", WspP);
        }
        public void wsppsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "P", WspP);
        }

        private double WspX(string pstring, Moment[] par)
        {
            switch (pstring)
            {
                case "ph":
                    return Oka.wspXPH(par[1].Real, par[2].Real);
                case "ps":
                    return Oka.wspXPS(par[1].Real, par[2].Real);
                case "hs":
                    return Oka.wspXHS(par[1].Real, par[2].Real);
                case "ptpeff":
                    return Oka.wspXEXPANSIONPTPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real);
                case "ptxpeff":
                    return Oka.wspXEXPANSIONPTXPEFF(par[1].Real, par[2].Real, par[3].Real, par[4].Real, par[5].Real);
            }
            return double.NaN;
        }

        public void wspxsr(Moment[] par, Moment res)
        {
            WspFun(par, res, "X", WspX);
        }
        public void wspxsrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "X", WspX);
        }
        public void wspxsrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "X", WspX);
        }
        public void wspxsrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "X", WspX);
        }
        public void wspxsrrrrr(Moment[] par, Moment res)
        {
            WspFun(par, res, "X", WspX);
        }
    }
}