using BaseLibrary;

namespace ReporterCommon
{
    //Один проект для использования в настройках
    public class ReportProjectSetup
    {
        public ReportProjectSetup() { }

        public ReportProjectSetup(string code, string code2, CalcModeType calcMode)
        {
            Code = code;
            Code2 = code2;
            CalcMode = calcMode;
        }

        public ReportProjectSetup(string code, string sysPageInf)
        {
            Code = code;
            var list = sysPageInf.Split(new[] { ';' });
            CalcMode = list.Length == 0 ? CalcModeType.ExternalPeriodic : list[0].ToCalcModeType();
            Code2 = list.Length == 1 ? null : list[1];
        }

        //Код
        public string Code { get; protected set; }
        //Псевдоним
        public string Code2 { get; protected set; }
        //Итоговый код
        public string CodeFinal { get { return Code2.IsEmpty() ? Code : Code2; } }
        //Тип исходных данных
        public CalcModeType CalcMode { get; set; }

        //Запись в ячейку SysPage
        public string ToSysPage()
        {
            string s = CalcMode.ToRussian();
            if (!Code2.IsEmpty()) s += ";" + Code2;
            return s;
        }
    }
}
