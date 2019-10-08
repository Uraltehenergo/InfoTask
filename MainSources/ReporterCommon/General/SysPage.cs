using System.Collections.Generic;
using System.Windows.Forms;
using BaseLibrary;
using Microsoft.Office.Interop.Excel;
using Worksheet = Microsoft.Office.Interop.Excel.Worksheet;

namespace ReporterCommon
{
    //Работа с листом SysPage текущего документа
    public class SysPage
    {
        public SysPage(Workbook workbook = null)
        {
            Workbook book = workbook ?? GeneralRep.Application.ActiveWorkbook;
            SysSheet = (Worksheet)book.Sheets["SysPage"];
            try { TemplatesSheet = (Worksheet)book.Sheets["Templates"]; } catch { }
        }

        //Ссылка на лист SysPage
        public Worksheet SysSheet { get; private set; }
        //Ссылка на лист Templates
        public Worksheet TemplatesSheet { get; private set; }

        //Проверяет, является ли текущий документ документом InfoTask
        public static bool IsInfoTask(Workbook workbook = null)
        {
            try
            {
                var sp = (Worksheet)(workbook ?? GeneralRep.Application.ActiveWorkbook).Sheets["SysPage"];
                if (sp.CellValue(2, 1) != "ReportName") return false;
                if (sp.CellValue(3, 1) != "ReportDescription") return false;
                if (sp.CellValue(4, 1) != "Report") return false;
                if (sp.CellValue(5, 1) != "CalcMode") return false;
                if (sp.CellIsEmpty(4,3)) return false;
            }
            catch {return false;}
            return true;
        }

        //Возвращает значение ячейки y,x в SysPage
        private string SysValue(int y, int x)
        {
            return SysSheet.CellValue(y, x);
        }

        //True, если значение ячейки y, x в SysPage - пустое
        private bool SysIsEmpty(int y, int x)
        {
            return SysSheet.CellIsEmpty(y, x);
        }

        //Записать строку в ячейку y, x в SysPage
        private void PutSysValue(int y, int x, string s)
        {
            SysSheet.PutCellValue(y, x, s);
        }

        //Возвращает значение ячейки y, x с Temlates
        private string TempValue(int y, int x)
        {
            return TemplatesSheet.CellValue(y, x);
        }

        //True, если значение ячейки y, x в Templates - пустое
        private bool TempIsEmpty(int y, int x)
        {
            return TemplatesSheet.CellIsEmpty(y, x);
        }

        //Записать строку в ячейку y, x в Templates
        private void PutTempValue(int y, int x, string s)
        {
            TemplatesSheet.PutCellValue(y, x, s);
        }

        //Получение значения параметра
        public string GetValue(string paramName)
        {
            int i = 2;
            try
            {
                while (!SysIsEmpty(i, 1))
                {
                    if (SysValue(i, 1) == paramName)
                    {
                        var s = SysValue(i, 3);
                        if (s == "") s = null;
                        return s;
                    }
                    i++;
                }
            }
            catch {}
            return null;
        }

        //Получение значения логического параметра
        public bool GetBoolValue(string paramName)
        {
            return GetValue(paramName) == "True";
        }

        //Получение значения целочисленного параметра
        public int GetIntValue(string paramName)
        {
            int n;
            int.TryParse(GetValue(paramName), out n);
            return n;
        }

        //Запись значения параметра, возвращает true, если записалось
        public bool PutValue(string paramName, string paramValue)
        {
            int i = 2;
            try
            {
                while (!SysIsEmpty(i, 1))
                {
                    if (SysValue(i, 1) == paramName)
                    {
                        PutSysValue(i, 3, paramValue);
                        return true;
                    }
                    i++;
                }
            }
            catch {}
            return false;
        }

        //Запись значения логического параметра
        public bool PutValue(string paramName, bool paramValue)
        {
            return PutValue(paramName, paramValue ? "True" : "False");
        }

        //Записывает в контрол значение параметра с тем же или указанным именем
        public void GetControl(Control control, string name = null)
        {
            control.Text = GetValue(name ?? control.Name);
        }

        //Записывает значение контрола в параметр с тем же или указанным именем
        public bool PutControl(Control control, string name = null)
        {
            return PutValue(name ?? control.Name, control.Text);
        }

        //Добавить новое свойство на SysPage
        public void AddProperty(string propCode, string propName = null, string propValue = null)
        {
            int i = 2;
            while (!SysIsEmpty(i, 1))
            {
                if (SysValue(i, 1) == propCode) return;
                i++;
            }
            PutSysValue(i, 1, propCode);
            PutSysValue(i, 2, propName);
            PutSysValue(i, 3, propValue);
        }

        //Получение списка проектов и режимов расчета
        public DicS<ReportProjectSetup> GetProjects()
        {
            var res = new DicS<ReportProjectSetup>();
            int i = 2;
            try
            {
                while (!SysIsEmpty(i, 4))
                {
                    var pr = new ReportProjectSetup(SysValue(i, 4), SysValue(i, 5));
                    res.Add(pr.CodeFinal, pr);
                    i++;
                }
            }
            catch {}
            return res;
        }

        //Запись списка проектов и режимов расчета
        public void PutProjects(List<ReportProjectSetup> projects)
        {
            int i = 0;
            try
            {
                while (i < projects.Count || !SysIsEmpty(i + 2, 4) || !SysIsEmpty(i + 2, 5))
                {
                    PutSysValue(i + 2, 4, (i < projects.Count) ? projects[i].Code : "");
                    PutSysValue(i + 2, 5, (i < projects.Count) ? projects[i].ToSysPage() : "");
                    i++;
                }
            }
            catch {}
        }
        
        //Загружает список шаблонов
        public void GetTemplatesList(ComboBox cbox)
        {
            cbox.Items.Clear();
            int x = 1;
            while (!TempIsEmpty(1, x))
                cbox.Items.Add(TempValue(1, x++));
        }

        //Возвращает номер стобца, содржащего описание шаблона или -1, если шаблон не найден
        public int TemplateX(string name)
        {
            int x = 1;
            while (!TempIsEmpty(1, x) && TempValue(1, x) != name)
                x++;
            if (!TempIsEmpty(1, x)) return x;
            return -1;
        }

        //Загружает общие свойства шаблона
        public DicS<string> GerTemplateGeneralProps(string name)
        {
            int x = TemplateX(name);
            if (x != -1) return TempValue(2, x).ToPropertyDicS();
            return new DicS<string>();
        } 

        //Загружает ячейки шаблона c указанным именем как список строк свойств
        public List<string> GetTemplate(string name)
        {
            var list = new List<string>();
            int x = TemplateX(name);
            if (x != -1)
            {
                int y = 3;
                while (!TempIsEmpty(y, x))
                {
                    string s = TempValue(y++, x);
                    if (s.ToPropertyDictionary().Count > 0)
                        list.Add(s);
                }
            }
            return list;
        }

         //Сохраняет общие свойства шаблона
        public void PutTemplateGeneralProps(string name, string props)
        {
            int x = TemplateX(name);
            if (x != -1) PutTempValue(2, x, props);
        }

        //Сохраняет шаблон c указанным именем из списка строк свойств
        public void PutTemplate(string name, string props, List<string> list)
        {
            int x = 1;
            while (!TempIsEmpty(1, x) && TempValue(1, x) != name)
                x++;
            PutTempValue(1, x, name);
            PutTempValue(2, x, props);
            int y = 3;
            while (!TempIsEmpty(y, x))
                PutTempValue(y++, x, null);
            y = 3;
            foreach (var s in list)
                PutTempValue(y++, x, s);
        }

        //Удаляет шаблон с заданным именем
        public void DeleteTemplate(string name)
        {
            int x = TemplateX(name);
            if (x != -1) ((Range)TemplatesSheet.Columns[x]).Delete();
        }
    }
}