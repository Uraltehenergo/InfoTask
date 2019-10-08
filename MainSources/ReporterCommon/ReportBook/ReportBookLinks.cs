using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using ColorFormat = Microsoft.Office.Core.ColorFormat;
using Shape = Microsoft.Office.Interop.Excel.Shape;

namespace ReporterCommon
{
    //Один отчет вместе со всеми открытыми формами, потоком и архивом
    //Загрузка и сохранение данных
    public partial class ReportBook : Logger
    {
        //Словарь всех проектов, на которые настроен отчет вместе со всеми параметрами, ключи - коды, 
        //Используется для обновления файла дяанных отчета и для формы установки ссылок
        private readonly DicS<ReportProjectForLinks> _projects = new DicS<ReportProjectForLinks>();
        public DicS<ReportProjectForLinks> Projects { get { return _projects; } }

        //Текущие тип ссылки и строка со свойствами устанавливаемой ссылки
        public string CurLinkType { get; set; }
        public string CurLinkProps { get; set; }
        
        //Если на листе выбран Shape, то его возвращает
        public Shape ActiveShape()
        {
            try
            {
                return (Shape)GeneralRep.Application.ActiveWindow.Selection.ShapeRange.Item(1);
            }
            catch { return null; }
        }
        
        //Изменяет текущий тип ссылки
        public void GetLinkType(string linkType)
        {
            if (linkType.IsEmpty() || CurLinkType == linkType) return;
            var f = (FormPutLinks)Forms[ReporterCommand.PutLinks];
            var p = Forms.ContainsKey(ReporterCommand.LinkProperties) ? ((FormLinkProperties) Forms[ReporterCommand.LinkProperties]).PropsPanel : null;
            if (p != null)
            {
                string s = p.PropsString();
                if (p.LinkPropsError.IsEmpty())
                    SysPage.PutValue(CurLinkType, s);
            }
            CurLinkType = linkType;
            CurLinkProps = SysPage.GetValue(linkType);
            if (f.CellLinkType.Text != CurLinkType)
                f.CellLinkType.Text = CurLinkType;
            if (p != null && p.CellLinkType.Text != CurLinkType)
            {
                //p.PropsFromDic(CurLinkProps.ToPropertyDictionary());
                p.CellLinkType.Text = CurLinkType;
            }
        }

        //Загружает свойства из выделенной ячейки
        public void GetLinkProps(Dictionary<string, string> props)
        {
            var f = (FormPutLinks)Forms[ReporterCommand.PutLinks];
            var p = Forms.ContainsKey(ReporterCommand.LinkProperties) ? ((FormLinkProperties)Forms[ReporterCommand.LinkProperties]).PropsPanel : null;
            var dic = props.Where(t => t.Key != "Project" && t.Key != "Code" && t.Key != "Field" && t.Key != "CellComment").ToDictionary(t => t.Key, t => t.Value);
            string lt = dic["LinkType"].ToLinkType().ToRussian();
            if (CurLinkType != lt)
            {
                string s = CurLinkProps;
                if (p != null)
                {
                    string ss = p.PropsString();
                    if (p.LinkPropsError.IsEmpty()) s = ss;
                }
                SysPage.PutValue(CurLinkType, s);
                CurLinkType = lt;
            }
            CurLinkProps = dic.ToPropertyString();
            if (f.CellLinkType.Text != CurLinkType)
                f.CellLinkType.Text = CurLinkType;
            if (f.CellComment.Text != dic.Get("CellComment"))
                f.CellComment.Text = dic.Get("CellComment");
            if (p != null)
            {
                if (p.CellLinkType.Text != CurLinkType) p.CellLinkType.Text = CurLinkType;
                p.PropsFromDic(dic);
            } 
        }

        //Текущий шаблон установки ссылок
        private readonly List<Dictionary<string, string>> _curLinkTemplate = new List<Dictionary<string, string>>();
        public List<Dictionary<string, string>> CurLinkTemplate { get { return _curLinkTemplate; } }
        //На сколько сдвигать текущую ячейку после установки ссылок
        public int CurTemplateShiftX { get; private set; }
        public int CurTemplateShiftY { get; private set; }

        //Последовательный список транзакций
        private readonly List<Transaction> _transactions = new List<Transaction>();
        //Ячейки, в которых был фокус до и после транзакции
        private readonly List<Range> _beforeTransCells = new List<Range>();
        private readonly List<Range> _afterTransCells = new List<Range>();
        //Текущий номер последней транзакции + 1
        private int _curTransactionNum;
        public int CurTransactionNum
        {
            get { return _curTransactionNum; }
            set
            {
                _curTransactionNum = value;
                if (Forms.ContainsKey(ReporterCommand.PutLinks))
                {
                    var f = (FormPutLinks)Forms[ReporterCommand.PutLinks];
                    f.ButUndo.Enabled = value > 0;
                    f.ButRedo.Enabled = value < _transactions.Count;    
                }
            }
        }
        

        //Загрузка текущего шаблона установки ссылок
        public void LoadCurTemplate()
        {
            var tname = SysPage.GetValue("CurTemplate");
            var list = SysPage.GetTemplate(tname);
            CurLinkTemplate.Clear();
            foreach (var s in list)
                CurLinkTemplate.Add(s.ToPropertyDictionary());
            var dic = SysPage.GerTemplateGeneralProps(tname);
            string shift = dic.Get("NextCellShift", "Нет");
            int step = dic.GetInt("NextCellStep");
            CurTemplateShiftX = shift == "Вправо" ? step : 0;
            CurTemplateShiftY = shift == "Вниз" ? step : 0;
        }

        //Обновление файла данных из проектов и отчета, загрузка параметров Projects 
        //updateAll - обновлять, даже если дата обновления ссылок не совпадет, delete - удалять файл перед обновлением
        public void UpdateDataFile(bool delete, bool updateAll = false)
        {
            AddEvent("Обновление файла данных", Code);
            if (delete) DaoDb.FromTemplate(General.ReportTemplateFile, DataFile, ReplaceByTemplate.Always);
            if (updateAll) LastChangeLinks = DateTime.Now;
            bool needUpdate = ParamsProjectsToFile();
            if (updateAll || needUpdate || Projects.Count == 0)
                ParamsFileToMemory();
            if (needUpdate || LastChangeLinks != SysTabl.ValueS(DataFile, "LastChangeLinks").ToDateTime())
                LinksReportToFile();
        }

        //Обновление списка параметров в файле данных отчета из архивов и проектов
        private bool ParamsProjectsToFile()
        {
            try
            {
                bool b = false;
                var list = MakeProjectsList();
                foreach (var p in list)
                    b |= p.UpdateParams();
                if (b)
                {
                    DaoDb.Execute(DataFile, "DELETE Cells.* FROM CalcParams INNER JOIN Cells ON CalcParams.ParamId = Cells.ParamId WHERE CalcParams.SysField='Del';");
                    DaoDb.Execute(DataFile, "DELETE * FROM CalcParams WHERE SysField='Del'");
                }
                return b;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при обновлении списка параметров", ex);
            }
            return false;
        }

        //Обновление списка проектов в файле данных отчета
        private IEnumerable<ReportProjectForData> MakeProjectsList()
        {
            AddEvent("Обновление списка проектов");
            var plist = new DicS<ReportProjectForData>();
            try
            {
                using (var rec = new RecDao(General.ReporterFile, "SELECT Reports.Report, Projects.* FROM Reports INNER JOIN Projects ON Reports.ReportId = Projects.ReportId WHERE Report='" + Code + "'"))
                    while (rec.Read())
                    {
                        var p = new ReportProjectForData(this, rec);
                        plist.Add(p.CodeFinal, p);    
                    }
                
                foreach (var pr in SysPage.GetProjects().Values)
                    if (plist.ContainsKey(pr.CodeFinal))
                        plist[pr.CodeFinal].CalcMode = pr.CalcMode;
                foreach (var pr in plist.Values)
                    pr.GetChangeTime();
                using (var db = new DaoDb(DataFile))
                {
                    var dic = new SetS();
                    using (var rec = new RecDao(db, "SELECT * FROM Projects"))
                    {
                        while (rec.Read())
                        {
                            var code = rec.GetString("Project");
                            if (code != "Сохранение" && code != "Системные")
                            {
                                if (!plist.ContainsKey(code)) rec.Put("SysField", "Del");
                                else
                                {
                                    dic.Add(code);
                                    rec.Put("SysField", "");
                                    plist[code].DataChangeTime = rec.GetTime("LastChange");
                                    plist[code].ToRecordset(rec, false);
                                }    
                            }
                        }
                        foreach (var p in plist.Values)
                            if (!dic.Contains(p.Code)) 
                                p.ToRecordset(rec, true);
                    }
                    db.Execute("DELETE Cells.* FROM Projects INNER JOIN (CalcParams INNER JOIN Cells ON CalcParams.ParamId = Cells.ParamId) ON Projects.Project = CalcParams.Project WHERE Projects.SysField='Del';");
                    db.Execute("DELETE CalcParams.* FROM Projects INNER JOIN CalcParams ON Projects.Project = CalcParams.Project WHERE Projects.SysField='Del';");
                    db.Execute("DELETE * FROM Projects WHERE Projects.SysField='Del'");
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при работе с ReporterData, архивом или файлом данных отчета", ex);
            }
            return plist.Values;
        }

        //Загрузка параметров из файла данных в память (Projects)
        private void ParamsFileToMemory()
        {
            AddEvent("Загрузка параметров из файла данных в память");
            try
            {
                Projects.Clear();
                using (var db = new DaoDb(DataFile))
                {
                    using (var rec = new ReaderAdo(db, "SELECT * FROM Projects"))
                        while (rec.Read())
                        {
                            var proj = new ReportProjectForLinks(this, rec);
                            Projects.Add(proj.CodeFinal, proj);
                        }
                    foreach (var pr in SysPage.GetProjects().Values)
                        if (Projects.ContainsKey(pr.CodeFinal))
                            Projects[pr.CodeFinal].CalcMode = pr.CalcMode;
                    using (var rec = new ReaderAdo(db, "SELECT * FROM CalcParams"))
                        while (rec.Read())
                        {
                            var par = new ReportParam(rec);
                            if (Projects.ContainsKey(par.Project))
                                Projects[par.Project].Params.Add(par.FullCode, par);
                        }
                }
                foreach (var proj in Projects.Values)
                    proj.MakeFilters();
            }
            catch (Exception ex)
            {
                AddError("Ошибка при загрузке параметров", ex);
            }
        }

        //Перенос ссылок из отчета в файл данных
        private void LinksReportToFile()
        {
            try
            {
                AddEvent("Обновление таблицы Cells");
                ClearReportDataFromMemory();
                bool onlyAbsolute = true;
                using (var db = new DaoDb(DataFile))
                {
                    db.Execute("DELETE * FROM Cells");
                    db.Execute("DELETE * FROM Shapes");
                    db.Execute("DELETE * FROM CalcParams WHERE Project='Сохранение'");
                    Projects["Сохранение"].Params.Clear();
                    using (var rp = new RecDao(db, "CalcParams"))
                    using (var rc = new RecDao(db, "Cells"))
                    using (var rs = new RecDao(db, "Shapes"))
                    {
                        foreach (var sheet in GeneralRep.Application.ActiveWorkbook.GetSheets())
                        {
                            foreach (Comment c in sheet.Comments)
                            {
                                onlyAbsolute &= CheckAbsolute(c.Text());
                                SaveLink(c, null, rp, rc, rs, sheet);
                            }
                            foreach (Shape sh in sheet.Shapes)
                            {
                                if (sh.Type == MsoShapeType.msoTextBox || sh.Type == MsoShapeType.msoGroup)
                                {
                                    onlyAbsolute &= CheckAbsolute(sh.Title);
                                    SaveLink(null, sh, rp, rc, rs, sheet);
                                    if (sh.Type == MsoShapeType.msoGroup)
                                        foreach (Shape gsh in sh.GroupItems)
                                            if (gsh.Type == MsoShapeType.msoTextBox)
                                            {
                                                onlyAbsolute &= CheckAbsolute(gsh.Title);
                                                SaveLink(null, gsh, rp, rc, rs, sheet);    
                                            }
                                }
                            }
                        }
                    }

                    UpdateParams(db);
                    using (var sys = new SysTabl(db))
                    {
                        sys.PutValue("LastChangeLinks", LastChangeLinks.ToString());
                        sys.PutValue("OnlyAbsolute", onlyAbsolute ? "True" : "False");
                    }
                }

                //Сжатие базы
                try { DaoDb.Compress(DataFile, 30000000, General.TmpDir, 2000); }
                catch { }
            }
            catch (Exception ex)
            { AddError("Ошибка при загрузке отчета в файл данных", ex); }
        }

        private static bool CheckAbsolute(string comment)
        {
            LinkType lt = comment.ToPropertyDicS()["LinkType"].ToLinkType();
            return lt == LinkType.Absolute || lt == LinkType.AbsoluteEdit;
        }

        //Сохранение одной ссылки
        private void SaveLink(Comment c, Shape sh, RecDao rp, RecDao rc, RecDao rs, Worksheet sheet)
        {
            var dic = (sh == null ? c.Text() : sh.Title).ToPropertyDicS();
            if (dic != null && dic.ContainsKey("Field") && dic.ContainsKey("Project") && dic.ContainsKey("Code"))
            {
                LinkType linkType = dic["LinkType"].ToLinkType();
                var projcode = dic["Project"];
                string parcode = dic["Code"];
                if (Projects.ContainsKey(projcode))
                {
                    ReportProjectForLinks proj = Projects[projcode];
                    ReportParam param = null;
                    if (proj.IsSave)
                    {
                        //Сохранение
                        if (proj.Params.ContainsKey(parcode)) param = proj.Params[parcode];
                        else
                        {
                            rp.AddNew();
                            param = new ReportParam(projcode, new ArchiveParam(parcode, DataType.String, null,
                                new CalcParamBase(parcode, dic["CellComment"]), new CalcParamBase(dic["Field"])))
                            {
                                Id = rp.GetInt("ParamId")
                            };
                            rp.Put("Project", projcode);
                            rp.Put("Code", parcode);
                            rp.Put("CodeParam", parcode);
                            rp.Put("DataType", DataType.String.ToRussian());
                            rp.Put("IsUsed", true);
                            rp.Put("IsHandInput", false);
                            proj.Params.Add(parcode, param);
                            rp.Update();
                        }
                    }
                    else if (proj.Params.ContainsKey(parcode))
                    {
                        param = proj.Params[parcode];
                        if (linkType == LinkType.HandInput)
                        {
                            //Ручной ввод
                            param.IsHandInput = true;
                            proj.IsHandInput = true;
                        }
                        else
                        {
                            //Обычное получние данных
                            param.IsUsed = true;
                            proj.IsUsed = true;
                        }
                    }

                    if (param != null)
                    {
                        var itype = IntervalType.Empty;
                        if (proj.CalcMode == CalcModeType.Internal)
                        {
                            if (linkType == LinkType.Result || linkType == LinkType.MomentsList)
                                itype = IntervalType.Single;
                        }
                        else
                        {
                            switch (linkType)
                            {
                                case LinkType.Combined:
                                case LinkType.CombinedList:
                                    if (dic.ContainsKey("LengthMinute") || dic.ContainsKey("PartBeginMinute") ||
                                        dic.ContainsKey("PartEndMinute"))
                                        itype = IntervalType.Base;
                                    else if (dic.ContainsKey("LengthHour") || dic.ContainsKey("PartBeginHour") ||
                                             dic.ContainsKey("PartEndHour"))
                                        itype = IntervalType.Hour;
                                    else if (dic.ContainsKey("LengthDay") || dic.ContainsKey("PartByTime"))
                                        itype = IntervalType.Day;
                                    else itype = IntervalType.Combined;
                                    break;
                                case LinkType.Absolute:
                                case LinkType.AbsoluteEdit:
                                    itype = IntervalType.Absolute;
                                    break;
                                case LinkType.AbsoluteCombined:
                                    itype = IntervalType.AbsoluteCombined;
                                    break;
                                case LinkType.AbsoluteList:
                                    if (dic.ContainsKey("LengthMinute") && dic["LengthMinute"].ToInt() != 0)
                                        itype = IntervalType.AbsoluteListBase;
                                    else if (dic.ContainsKey("LengthHour") && dic["LengthHour"].ToInt() != 0)
                                        itype = IntervalType.AbsoluteListHour;
                                    else itype = IntervalType.AbsoluteListDay;
                                    break;
                                case LinkType.MomentsList:
                                case LinkType.Result:
                                    itype = IntervalType.Moments;
                                    break;
                            }
                        }
                        if (!param.IntervalTypes.Contains(itype))
                            param.IntervalTypes.Add(itype);
                        
                        if (sh == null)
                        {
                            //Добавление в таблицу Cells
                            rc.AddNew();
                            rc.Put("Page", sheet.Name);
                            rc.Put("Y", ((Range) c.Parent).Row);
                            rc.Put("X", ((Range) c.Parent).Column);
                            rc.Put("LinkType", linkType.ToEnglish());
                            rc.Put("LinkField", dic["Field"]);
                            if (dic.ContainsKey("AllowEdit"))
                                rc.Put("AllowEdit", dic["AllowEdit"] == "True");
                            string prop = "";
                            foreach (var k in dic.Keys)
                                if (k != "LinkType" && k != "Field" && k != "Project" && k != "Code")
                                    prop += k + "=" + dic[k] + ";";
                            rc.Put("Properties", prop);
                            rc.Put("ParamId", param.Id);
                            rc.Put("IntervalType", itype.ToRussian());
                            rc.Put("SaveCode", dic["SaveCode"]);
                            rc.Update();
                        }
                        else
                        {
                            //Добавление в таблицу Shapes
                            rs.AddNew();
                            rs.Put("Page", sheet.Name);
                            rs.Put("ShapeId", sh.ID);
                            rs.Put("ShapeType", sh.Type.ToEnglish());
                            if (sh.Type == MsoShapeType.msoGroup)
                                rs.Put("Formula", sh.AlternativeText);
                            rs.Put("LinkType", linkType.ToEnglish());
                            rs.Put("LinkField", dic["Field"]);
                            string prop = "";
                            foreach (var k in dic.Keys)
                                if (k != "LinkType" && k != "Field" && k != "Project" && k != "Code")
                                    prop += k + "=" + dic[k] + ";";
                            rs.Put("Properties", prop);
                            rs.Put("ParamId", param.Id);
                            rs.Put("IntervalType", itype.ToRussian());
                            rs.Update();
                        }
                    }
                }
            }
        }

        private void UpdateParams(DaoDb db)
        {
            AddEvent("Обновление таблиц CalcParams и Projects по ячейкам отчета");
            using (var rpr = new RecDao(db, "Projects"))
                while (rpr.Read())
                {
                    string pr = rpr.GetString("ProjectCode2");
                    if (pr.IsEmpty()) pr = rpr.GetString("Project");
                    Projects[pr].PropsToRecordset(rpr);
                }

            using (var rp = new RecDao(db, "CalcParams"))
                if (rp.HasRows())
                {
                    rp.MoveFirst();
                    while (!rp.EOF)
                    {
                        var proj = Projects[rp.GetString("Project")];
                        var code = rp.GetString("Code");
                        var param = proj.Params[code];
                        param.PropsToRecordset(rp);
                        rp.MoveNext();
                    }
                }
        }

        //Удаляет ячейки в выделенном диапазоне
        public void DeleteLinks()
        {
            try
            {
                if (!GeneralRep.CheckOneSheet(true)) return;
                var sh = ActiveShape();
                if (sh != null)
                {
                    BeforeTransaction();
                    var list = new Transaction();
                    list.AddShape(new TransactionShape(sh) { NewLink = null });
                    sh.Title = "";
                    LastChangeLinks = DateTime.Now;
                    AddTransaction(list);
                }
                else
                {
                    Range range = GeneralRep.Application.ActiveWindow.RangeSelection;
                    bool many = true;
                    try { many = range.Cells.Count > 10000; }
                    catch { }
                    if (MessageReportQuestion((!many ? "" : "Выделенный фрагмент очень большой, отмена удаления будет невозможна. ") + "Удалить все ссылки в выделенном фрагменте?"))
                    {
                        var list = new Transaction();
                        BeforeTransaction();
                        if (!many)
                            foreach (Range cell in range.Cells)
                                if (cell.Comment != null)
                                    list.AddCell(new TransactionCell(cell) { NewLink = null });
                        range.ClearComments();
                        LastChangeLinks = DateTime.Now;
                        ClearReportDataFromMemory();
                        if (!many) AddTransaction(list);
                    }    
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при удалении ссылок", ex);
            }
        }

        public void BeforeTransaction()
        {
            if (CurTransactionNum != _transactions.Count)
                _beforeTransCells.RemoveRange(CurTransactionNum, _transactions.Count - CurTransactionNum);
            if (_beforeTransCells.Count >= 200)
                _beforeTransCells.RemoveRange(0, _transactions.Count - 100);
            _beforeTransCells.Add(GeneralRep.Application.ActiveCell);
        }

        //Выполнение транцзакции
        public void AddTransaction(Transaction t)
        {
            if (!t.ErrMess().IsEmpty())
                Different.MessageError(t.ErrMess());   
            if (t.Cells.Count == 0 && t.TrShape == null) return;
            if (CurTransactionNum != _transactions.Count)
            {
                _transactions.RemoveRange(CurTransactionNum, _transactions.Count - CurTransactionNum);
                _afterTransCells.RemoveRange(CurTransactionNum, _transactions.Count - CurTransactionNum);
            }
            if (_transactions.Count >= 200)
            {
                _transactions.RemoveRange(0, _transactions.Count - 100);
                _afterTransCells.RemoveRange(0, _transactions.Count - 100);
                CurTransactionNum = _transactions.Count;
            }
            _transactions.Add(t);
            if (t.TrShape == null) 
                _afterTransCells.Add(GeneralRep.Application.ActiveCell);
            CurTransactionNum++;
        }

        //Откат транзкции
        public void UndoTransaction()
        {
            if (CurTransactionNum > 0)
            {
                CurTransactionNum--;
                try
                {
                    var trans = _transactions[CurTransactionNum];
                    if (trans.TrShape != null)
                        trans.TrShape.Undo();
                    var cells = trans.Cells;
                    foreach (var t in cells)
                        t.Undo();
                    if (cells.Count > 0)
                    {
                        var c = _beforeTransCells[CurTransactionNum]; 
                        if (GeneralRep.Application.ActiveSheet != c.Worksheet)
                            ((_Worksheet)c.Worksheet).Activate();
                        c.Activate();
                    }
                }
                catch { }
            }
        }
        //Обратный накат транзакции
        public void RedoTransaction()
        {
            if (CurTransactionNum < _transactions.Count)
            {
                try
                {
                    var t = _transactions[CurTransactionNum];
                    if (t.TrShape != null)
                        t.TrShape.Undo();
                    foreach (var cell in t.Cells)
                        cell.Redo();
                    var c = _afterTransCells[CurTransactionNum];
                    if (GeneralRep.Application.ActiveSheet != c.Worksheet)
                        ((_Worksheet)c.Worksheet).Activate();
                    c.Activate();
                } 
                catch {}
                CurTransactionNum++;
            }
        }
    }
}