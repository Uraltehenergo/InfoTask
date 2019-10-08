using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Shape = Microsoft.Office.Interop.Excel.Shape;

namespace ReporterCommon
{
    public partial class FormPutLinks : Form
    {
        public FormPutLinks()
        {
            InitializeComponent();
        }

        //Текущая книга
        private ReportBook _book;
        //Текущие проект и параметр
        public ReportProjectForLinks CurProject { get; private set; }
        public ReportParam CurParam { get; private set; }
        //Таблица с параметрами в dataSet1
        private System.Data.DataTable _table;

        //Форма фильтров
        private FormFiltersParams _filterForm;
        //Форма свойств ссылки и параметра
        public FormLinkProperties PropForm { get; set; }
        //Производится обновление текущего проекта
        public bool IsApplyProject { get; private set; }

        private void FormPutLinks_Load(object sender, EventArgs e)
        {
            try
            {
                _book = GeneralRep.ActiveBook;
                _table = dataSet1.Tables[0];
                new ToolTip().SetToolTip(ButDeleteLinks, "Удалить все ссылки из выделенной области листа");
                new ToolTip().SetToolTip(ButUndo, "Отменить последнюю установку ссылок");
                new ToolTip().SetToolTip(ButRedo, "Вернуть отмененную установку ссылок");
                new ToolTip().SetToolTip(ButUpdate, "Обновить список параметров");
                new ToolTip().SetToolTip(ButFilter, "Поиск и фильтрация списка параметров");
                new ToolTip().SetToolTip(Template, "Текущий шаблон для установки ссылок");
                new ToolTip().SetToolTip(ButSave, "Сохранить установленные ссылки и отчет");
                new ToolTip().SetToolTip(ButLinkSave, "Добавить ссылку на ячейку для сохранения в журнал отчетов");
                new ToolTip().SetToolTip(ButFindLinks, "Список всех ячеек со ссылками на выбранный параметр");
                new ToolTip().SetToolTip(ButOtmLinks, "Установить ссылки на отмеченные параметры в соответствии с текущим шаблоном");
                new ToolTip().SetToolTip(ButOtmTrue, "Отметить все");
                new ToolTip().SetToolTip(ButOtmFalse, "Снять все отметки");
                
                _book.SysPage.GetTemplatesList(Template);
                _book.CurTransactionNum = _book.CurTransactionNum;//Обновить доступность кнопок

                //Выпадающий список проектов 
                Project.Items.Clear();
                foreach (var p in _book.Projects.Values)
                {
                    if (p.IsSystem) Project.Items.Add(p.Code);
                    else if (!p.IsSave) Project.Items.Add(p.CodeFinal + ":  " + p.Name);
                }
                if (Project.Items.Count > 1 && (string)Project.Items[0] == "Системные")
                    Project.Text = (string)Project.Items[1];
                else Project.Text = (string)Project.Items[0];

                var menu = new ContextMenuStrip();
                menu.Items.Add(LinkField.Code.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddCode;
                menu.Items.Add(LinkField.CodeParam.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddCodeParam;
                menu.Items.Add(LinkField.CodeSubParam.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddCodeSubParam;
                menu.Items.Add(LinkField.Name.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddName;
                menu.Items.Add(LinkField.SubName.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddSubName;
                menu.Items.Add(LinkField.Units.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddUnits;
                menu.Items.Add(LinkField.Task.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddTask;
                menu.Items.Add(LinkField.Comment.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddComment;
                menu.Items.Add(LinkField.SuperProcessType.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddSuperProcess;
                menu.Items.Add(LinkField.DataType.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddDataType;
                menu.Items.Add(LinkField.Min.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddMin;
                menu.Items.Add(LinkField.Max.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddMax;
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Ссылки по шаблону");
                menu.Items[menu.Items.Count - 1].Click += AddTemplateLinks;
                menu.Items.Add(LinkField.Value.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddValueLink;
                menu.Items.Add(LinkField.Time.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddTimeLink;
                menu.Items.Add(LinkField.Nd.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddNdLink;
                menu.Items.Add(LinkField.Number.ToRussian());
                menu.Items[menu.Items.Count - 1].Click += AddNumberLink;
                Params.ContextMenuStrip = menu;
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка загрузки формы установки ссылок", ex);
            }
            try
            {
                Template.Text = _book.SysPage.GetValue("CurTemplate");
                CellComment.Text = _book.SysPage.GetValue("CurCellComment");
                NextCellShift.Text = _book.SysPage.GetValue("CurNextCellShift");
                NextCellStep.Text = _book.SysPage.GetValue("CurNextCellStep");
                NextCellStep.Enabled = NextCellShift.Text != "Нет";
            }
            catch { }
            GeneralRep.Application.CommandBars.OnUpdate += OnShapeChange;
            GeneralRep.Application.SheetSelectionChange += OnSelectionChange;
        }

        //Действия для комманд контекстного меню
        private void AddCode(object sender, EventArgs e)
        {
            AddLink(LinkField.Code);
        }
        private void AddCodeParam(object sender, EventArgs e)
        {
            AddLink(LinkField.CodeParam);
        }
        private void AddCodeSubParam(object sender, EventArgs e)
        {
            AddLink(LinkField.CodeSubParam);
        }
        private void AddName(object sender, EventArgs e)
        {
            AddLink(LinkField.Name);
        }
        private void AddSubName(object sender, EventArgs e)
        {
            AddLink(LinkField.SubName);
        }
        private void AddUnits(object sender, EventArgs e)
        {
            AddLink(LinkField.Units);
        }
        private void AddTask(object sender, EventArgs e)
        {
            AddLink(LinkField.Task);
        }
        private void AddComment(object sender, EventArgs e)
        {
            AddLink(LinkField.Comment);
        }
        private void AddSuperProcess(object sender, EventArgs e)
        {
            AddLink(LinkField.SuperProcessType);
        }
        private void AddDataType(object sender, EventArgs e)
        {
            AddLink(LinkField.DataType);
        }
        private void AddMax(object sender, EventArgs e)
        {
            AddLink(LinkField.Max);
        }
        private void AddMin(object sender, EventArgs e)
        {
            AddLink(LinkField.Min);
        }

        private void AddValueLink(object sender, EventArgs e)
        {
            AddLink(LinkField.Value);
        }
        private void AddTimeLink(object sender, EventArgs e)
        {
            AddLink(LinkField.Time);
        }
        private void AddNdLink(object sender, EventArgs e)
        {
            AddLink(LinkField.Nd);
        }
        private void AddNumberLink(object sender, EventArgs e)
        {
            AddLink(LinkField.Number);
        }
        private void AddTemplateLinks(object sender, EventArgs e)
        {
            if (_book.ActiveShape() == null)
                AddLinksTemplate();
        }

        //Загружает значения отметок в параметры
        public void LoadOtmToParams()
        {
            if (CurProject != null)
                foreach (DataGridViewRow r in Params.Rows)
                    CurProject.Params[r.Get("Code")].Otm = r.GetBool("Otm");
        }

        //Обновляет грид параметров по выбранному проекту учитывется состояние кнопки установки фильтра
        //updateItems - обновлять истночники списков для фильтров 
        public void ApplyProject(bool updateItems)
        {
            IsApplyProject = true;
            try
            {
                LoadOtmToParams();
                _table.Rows.Clear();
                string pr = Project.Text;
                if (pr.IndexOf(':') >= 0) pr = pr.Substring(0, pr.IndexOf(':'));
                if (!_book.Projects.ContainsKey(pr))
                    CurProject = null;
                else
                {
                    CurProject = _book.Projects[pr];
                    if (_filterForm != null && updateItems) _filterForm.MakeFiltersLists();
                    foreach (var par in CurProject.Params.Values)
                        if (_filterForm == null || _filterForm.CheckParam(par))
                        {
                            var r = _table.NewRow();
                            r["ParamOtm"] = par.Otm;
                            r["ParamTask"] = par.GetField(LinkField.Task);
                            r["ParamCode"] = par.GetField(LinkField.Code);
                            r["ParamName"] = par.GetField(LinkField.Name);
                            r["ParamDataType"] = par.GetField(LinkField.DataType);
                            r["ParamUnits"] = par.GetField(LinkField.Units);
                            r["ParamSuperProcess"] = par.GetField(LinkField.SuperProcessType);
                            r["ParamCalcParamType"] = par.GetField(LinkField.CalcParamType);
                            r["ParamMin"] = par.GetField(LinkField.Min);
                            r["ParamMax"] = par.GetField(LinkField.Max);
                            r["ParamComment"] = par.GetField(LinkField.Comment);
                            _table.Rows.Add(r);
                        }
                    Params.Update();
                    if (Params.Rows.Count > 0)
                    {
                        Params.Rows[0].Cells[4].Selected = true;
                        ApplyParam();
                    }
                    Params.Columns[2].Visible = Params.Columns[3].Visible = !CurProject.IsSystem;
                }
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при обновлении списка параметров", ex);
            }
            IsApplyProject = false;
        }

        //Обновляет свойства по выбранному параметру
        private void ApplyParam()
        {
            try
            {
                var row = Params.SelectedCells[0].OwningRow;
                string s = row.Get("Code");
                if (CurParam == null || CurParam.FullCode != s)
                {
                    CurParam = CurProject.Params[row.Get("Code")];
                    MakeLinkTypeList(CellLinkType);    
                    if (PropForm != null)
                        MakeLinkTypeList(PropForm.PropsPanel.CellLinkType);    
                }
            }
            catch { CurParam = null; }
        }

        //Поиск параметра по условию
        public void FindParam()
        {
            LoadOtmToParams();
            int s = 0;
            try { s = Params.SelectedCells[0].RowIndex + 1; }
            catch { }
            for (int i = 0; i <= Params.Rows.Count; i++)
            {
                var row = Params.Rows[(s + i) % Params.Rows.Count];
                if (_filterForm.CheckParam(CurProject.Params[row.Get("Code")], true))
                {
                    var cell = row.Cells[4];
                    cell.Selected = true;
                    Params.CurrentCell = cell;
                    ApplyParam();
                    break;
                }
            }
        }

        private void ButDeleteLinks_Click(object sender, EventArgs e)
        {
            _book.DeleteLinks();
        }

        private void ButFilter_Click(object sender, EventArgs e)
        {
            _filterForm = (FormFiltersParams)_book.RunCommandReporter(ReporterCommand.FilterParams);
            BindingNavigator1.Items["ButSetFilter"].Enabled = true;
        }

        //Формирует выпадающий список типов ссылок для заданного параметра
        public void MakeLinkTypeList(ComboBox field)
        {
            string lts = field.Text;
            var items = field.Items;
            var ap = CurParam.ArchiveParam;
            items.Clear();
            if (CurProject.Code == "Системные") items.Add(LinkType.System.ToRussian());
            else
            {
                if (ap.FirstParam != null && ap.FirstParam.CalcParamType.HandDataType() != BaseLibrary.DataType.Error)
                    items.Add(LinkType.HandInput.ToRussian());
                if (CurProject.CalcMode == CalcModeType.Internal)
                {
                    if (!ap.SuperProcess.IsNone())
                    {
                        items.Add(LinkType.Result.ToRussian());
                        items.Add(LinkType.MomentsList.ToRussian());
                    }
                }
                else
                {
                    if (ap.SuperProcess.IsPeriodic())
                    {
                        items.Add(LinkType.Combined.ToRussian());
                        items.Add(LinkType.CombinedList.ToRussian());
                    }
                    if (ap.SuperProcess.IsAbsolute())
                    {
                        items.Add(LinkType.Absolute.ToRussian());
                        items.Add(LinkType.AbsoluteEdit.ToRussian());
                    }
                    if (ap.SuperProcess.IsPeriodic() && ap.SuperProcess.IsAbsolute())
                    {
                        items.Add(LinkType.AbsoluteCombined.ToRussian());
                        items.Add(LinkType.AbsoluteList.ToRussian());
                    }
                    if (ap.SuperProcess == SuperProcess.Moment)
                    {
                        items.Add(LinkType.MomentsList.ToRussian());
                        items.Add(LinkType.Result.ToRussian());
                    }
                }
            }
            if (items.Count > 0)
            {
                bool b = false;
                foreach (var item in items)
                    b |= (string)item == lts;
                if (!b) field.Text = (string)items[0];
                else field.Text = lts;
            }
        }

        private void ButTemplates_Click(object sender, EventArgs e)
        {
            _book.RunCommandReporter(ReporterCommand.LinksTemplate);
        }

        private void FormPutLinks_FormClosing(object sender, FormClosingEventArgs e)
        {
            _book.SysPage.PutValue("CurTemplate", Template.Text);
            _book.SysPage.PutValue("CurCellComment", CellComment.Text);
            _book.SysPage.PutValue("CurNextCellShift", NextCellShift.Text);
            _book.SysPage.PutValue("CurNextCellStep", NextCellStep.Text);
            _book.CloseForm(ReporterCommand.PutLinks);
            GeneralRep.Application.SheetSelectionChange -= OnSelectionChange;
            GeneralRep.Application.CommandBars.OnUpdate -= OnShapeChange;
        }

        private void Project_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsApplyProject) ApplyProject(true);
        }
        
        //Обработка события по изменению текущей ячейки
        private void OnSelectionChange(object sh, Range range)
        {
            LoadFromCell();
        }

        //Id текущей выбранной фигуры
        private int _curShapeId;

        //Обработка события по изменению текущей фигуры
        private void OnShapeChange()
        {
            var sh = _book.ActiveShape();
            if (sh != null && sh.ID != _curShapeId)
            {
                _curShapeId = sh.ID;
                LoadFromCell();
            }
        }

        //Загружает данные из примечания к ячейке
        private void LoadFromCell()
        {
            var sh = _book.ActiveShape();
            string comment = null, address = null;
            if (sh != null)
            {
                if (sh.Type == MsoShapeType.msoTextBox || sh.Type == MsoShapeType.msoGroup)
                {
                    comment = sh.Title;
                    address = sh.Name;    
                }
            }
            else
            {
                var activeCell = GeneralRep.Application.ActiveCell;
                comment = activeCell.Comment == null ? null : activeCell.Comment.Text();
                address = activeCell.Address.Replace("$", "");
            }
            
            if (comment == null)
            {
                CurrentLink.Text = "";
                return;
            }
            GetCurrentLink(comment, address);
            var dic = comment.ToPropertyDictionary();

            if (IsLoadFromCell.Checked)
            {
                if (dic.ContainsKey("Project") && dic.ContainsKey("Code") && dic.ContainsKey("Field"))
                {
                    var proj = dic["Project"];
                    var lpar = dic["Code"];
                    var field = dic["Field"].ToLinkField();
                    if (_book.Projects.ContainsKey(proj) && _book.Projects[proj].Params.ContainsKey(lpar))
                    {
                        if (CurProject.CodeFinal.ToLower() != dic["Project"].ToLower()) return;
                        foreach (DataGridViewRow r in Params.Rows)
                            try
                            {
                                if (r.Get("Code").ToLower() == lpar.ToLower())
                                {
                                    var cell = field.ParamsTableName() == null ? r.Cells[4] : r.Cells[field.ParamsTableName()];
                                    cell.Selected = true;
                                    Params.CurrentCell = cell;
                                    ApplyParam();
                                    _book.GetLinkProps(dic);
                                    break;
                                }
                            }
                            catch { }
                        if (dic.ContainsKey("CellComment") && dic["CellComment"] != CellComment.Text)
                            CellComment.Text = dic["CellComment"];
                    }
                }
            }
        }

        //Заполняет строку для поля CurrentLink из словаря свойств ячейки
        private void GetCurrentLink(string comment, string address)
        {
            var dic = comment.ToPropertyDictionary();
            string s = "Ячейка " + address + ". ";
            if (dic.ContainsKey("Code")) s += "Параметр: " + dic.Get("Code") + "; ";
            if (dic.ContainsKey("Project")) s += "Проект: " + dic.Get("Project") + "; ";
            if (dic.ContainsKey("LinkType")) s += "Тип ссылки: " + dic.Get("LinkType").ToLinkType().ToRussian() + "; ";
            if (dic.ContainsKey("Field")) s += "Тип информации: " + dic.Get("Field").ToLinkField().ToRussian() + "; ";
            if (dic.ContainsKey("CellComment")) s += "Примечание: " + dic.Get("CellComment") + "; ";
            if (dic.ContainsKey("SaveCode")) s += "Сохраняется в журнал как: " + dic.Get("SaveCode") + "; ";
            CurrentLink.Text = s;
        }

        //Записывает в текущую ячейку значение свойства выделенного параметра, field - имя поля
        //Транзакция создается из одной ячейки и добавляется в Transactions
        public void WriteValue(LinkField field, Transaction tlist)
        {
            try
            {
                if (!GeneralRep.CheckOneSheet(true)) return;
                var v = CurParam.GetField(field);
                var shape = _book.ActiveShape();
                if (shape == null)
                {
                    Range cell = GeneralRep.Application.ActiveCell;
                    var t = new TransactionCell(cell)
                    {
                        NewValue = v,
                        Value = v
                    };
                    tlist.AddCell(t);
                }
                else if (shape.Type == MsoShapeType.msoTextBox)
                {
                    var t = new TransactionShape(shape)
                    {
                        NewValue = v,
                        Value = v
                    };
                    tlist.AddShape(t);
                }
            }
            catch {}
        }

        //Добавляет в текущую ячейку ссылку на выделенный параметр, field - имя поля
        //Транзакция создается из одной ячейки и добавляется в Transactions
        public void AddLink(LinkField field)
        {
            try
            {
                if (!GeneralRep.CheckOneSheet(true)) return;
                Shape sh = _book.ActiveShape();
                var lt = _book.CurLinkType.ToLinkType();
                if (sh != null && (lt != LinkType.Absolute && lt != LinkType.AbsoluteCombined  && lt != LinkType.Combined && lt != LinkType.Result && lt != LinkType.System || (sh.Type != MsoShapeType.msoTextBox && sh.Type != MsoShapeType.msoGroup)))
                    return;

                var tlist = new Transaction();
                string res = "Project=" + CurProject.Code + ";Field=" + field.ToEnglish() + ";Code=" + CurParam.FullCode + ";" + "CellComment=" + CellComment.Text + ";SaveCode=" + GetSaveCode() + ";";
                if (!field.IsValueField())
                    WriteValue(field, tlist);
                else
                {
                    if (PropForm != null)
                    {
                        res += PropForm.PropsString;
                        if (!PropForm.PropsError.IsEmpty())
                        {
                            Different.MessageError(PropForm.PropsError, "Не правильно заполнены свойства ссылки");
                            return;
                        }
                    }
                    else
                    {
                        res += _book.CurLinkProps;
                        if (_book.CurLinkProps.IsEmpty())
                        {
                            Different.MessageError("Нужно заполнить свойства устанавливаемой ссылки");
                            return;
                        }
                    }
                }

                if (field != LinkField.Code && field != LinkField.CodeParam && field != LinkField.CodeSubParam)
                {
                    if (sh != null) AddShapeLink(res, tlist, sh);
                    else AddCellLink(res, tlist, GeneralRep.Application.ActiveCell);
                }
                if (sh == null)
                {
                    _book.BeforeTransaction();
                    MoveAfterLink();    
                }
                _book.AddTransaction(tlist);
            }
            catch { }
        }

        //Возвращает следующий номер парамтра для сохранения
        private string GetSaveCode()
        {
            int n = int.Parse(_book.SysPage.GetValue("LastSaveParamId")) + 1;
            _book.SysPage.PutValue("LastSaveParamId", n.ToString());
            return "SaveCell_" + n;
        }

        //Добавляет ссылку типа Сохранение
        //tlist - список операций текущей транзакции, если не задан то транзакция создается из одной ячейки и добавляется в Transactions
        public void AddLinkSave(Transaction tlist = null, Range cell = null)
        {
            try
            {
                if (!GeneralRep.CheckOneSheet(true)) return;
                string res = "Project=Сохранение;Field=" + LinkField.Value.ToEnglish() + ";AllowEdit=True;";
                res += "LinkType=" + LinkType.Save.ToEnglish() + ";";
                res += "CellComment=" + CellComment.Text + ";";
                string scode = GetSaveCode();
                res += "Code=" + scode + ";SaveCode=" + scode;
                AddCellLink(res, tlist, cell);
            }
            catch { }
        }

        //Добавляет примечание comment в ячейку cell
        //tlist - список операций текущей транзакции, если не задан то транзакция создается из одной ячейки и добавляется в Transactions
        private void AddCellLink(string comment, Transaction tlist, Range cell)
        {
            string c = comment;
            if (cell.Comment != null)
            {
                var dicOld = cell.Comment.Text().ToPropertyDictionary();
                var dic = comment.ToPropertyDictionary();
                if (dic.ContainsKey("NumPoints"))
                    dic.Remove("NumPoints");
                if (dicOld.ContainsKey("NumPoints"))
                    dic.Add("NumPoints", dicOld["NumPoints"]);
                c = dic.ToPropertyString();
            }
            var t = new TransactionCell(cell) { NewLink = c };
            cell.ClearComments();
            cell.AddComment(c);
            tlist.AddCell(t);
            GetCurrentLink(c, cell.Address.Replace("$", ""));
        }

        //Добавляет примечание comment в фигуру
        private void AddShapeLink(string comment, Transaction tlist, Shape shape)
        {
            var t = new TransactionShape(shape) { NewLink = comment };
            shape.Title = comment;
            tlist.AddShape(t);
            GetCurrentLink(comment, shape.Name);
        }

        //Сдигает выделение после установки ссылки, isValue - производится запись свойства в ячейку, иначе установка ссылки
        public void MoveAfterLink()
        {
            try
            {
                var acell = GeneralRep.Application.ActiveCell;
                Range rcell = null;
                int d;
                if (int.TryParse(NextCellStep.Text, out d))
                {
                    if (NextCellShift.Text == "Вправо")
                        rcell = (Range) acell.Worksheet.Cells[acell.Row, acell.Column + d];
                    if (NextCellShift.Text == "Вниз")
                        rcell = (Range) acell.Worksheet.Cells[acell.Row + d, acell.Column];
                    if (rcell != null) rcell.Activate();
                }
                else Different.MessageError("Недопустимое значение шага перехода после установки ссылки: " + NextCellStep.Text);
            }
            catch { }
        }

        //Добавляет ссылки по шаблону, rp - параметр по которому устанавливать ссылки, tlist - текущая транзакция
        public void AddLinksTemplate(ReportParam rp = null, Transaction tlist = null)
        {
            var trans = tlist ?? new Transaction();
            try
            {
                if (!GeneralRep.CheckOneSheet(true)) return;
                var param = rp ?? CurParam;
                if (CurProject == null || param == null) return;
                if (tlist == null) _book.BeforeTransaction();
                var page = (Worksheet)_book.Workbook.ActiveSheet;
                var acell = GeneralRep.Application.ActiveCell;
                foreach (var dic in _book.CurLinkTemplate)
                {
                    var action = dic["CellAction"].ToCellAction();
                    string code = dic["CodeForming"];//Способ формирования кода или текст для записи, если action=Записать текст
                    if (action != CellActionType.Text)
                    {
                        code = code.Replace("<Код параметра>", param.ArchiveParam.FirstParam.Code).Replace("<Полный код>", param.FullCode);
                        int n1 = param.FullCode.IndexOf(".");
                        if (n1 > 0)
                        {
                            int n2 = param.FullCode.IndexOf(".", n1 + 1);
                            if (n2 > 0) code = code.Replace("<Код 2 параметра>", param.FullCode.Substring(0, n2));
                        } 
                    }
                    
                    int x = acell.Column, y = acell.Row;
                    x += dic.GetInt("X");
                    y += dic.GetInt("Y");
                    var field = dic["Field"].ToLinkField();
                    var cell = (Range)page.Cells[y, x];
                    var dicl = dic.Keys.Where(prop => prop != "X" && prop != "Y" && prop != "CellAction" && prop != "CodeForming").ToDictionary(prop => prop, prop => dic[prop]);
                    if (action == CellActionType.Save)
                        AddLinkSave(trans, cell);
                    if (action == CellActionType.Text)
                    {
                        trans.AddCell(new TransactionCell(page, x, y) { NewValue = code });
                        page.PutCellValue(y, x, code);
                    }
                    else if (!CurProject.Params.ContainsKey(code))
                        trans.NotFound.Add(code);
                    else
                    {
                        var par = CurProject.Params[code];
                        if (action == CellActionType.Value)
                        {
                            string s = par.GetField(field);
                            if (s != null)
                            {
                                trans.AddCell(new TransactionCell(page, x, y) {NewValue = s});
                                page.PutCellValue(y, x, s);
                            }
                        }
                        else
                        {
                            dicl.Add("Project", CurProject.CodeFinal);
                            dicl.Add("Code", par.FullCode);
                            dicl.Add("CellComment", CellComment.Text ?? "");
                            dicl.Add("SaveCode", GetSaveCode());
                            if (field == LinkField.CodeSubParam || field == LinkField.SubName ||
                                field == LinkField.SubComment)
                            {
                                if (par.ArchiveParam.LastParam != null)
                                    AddCellLink(dicl.ToPropertyString(), trans, cell);
                            }
                            else if (field == LinkField.Min || field == LinkField.Max)
                            {
                                if (par.ArchiveParam.DataType == BaseLibrary.DataType.Real ||
                                    par.ArchiveParam.DataType == BaseLibrary.DataType.Integer)
                                    AddCellLink(dicl.ToPropertyString(), trans, cell);
                            }
                            else if (!field.IsValueField() || CanAddLinkType(par, dicl["LinkType"].ToLinkType()))
                                AddCellLink(dicl.ToPropertyString(), trans, cell);
                            else trans.NotFound.Add(code);
                        }
                    }
                }
                ((Range)page.Cells[acell.Row + _book.CurTemplateShiftY, acell.Column + _book.CurTemplateShiftX]).Activate();
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при установке ссылки по шаблону", ex);
            }
            if (tlist == null) _book.AddTransaction(trans);
        }
     
        //Проверяет, можно ли устанавливать ссылку типа ltype на параметр par
        private bool CanAddLinkType(ReportParam par, LinkType ltype)
        {
            if (CurProject.CalcMode == CalcModeType.Internal)
                return ltype == LinkType.MomentsList || ltype == LinkType.Result;
            var sp = par.ArchiveParam.SuperProcess;
            if (sp.IsAbsolute())
            {
                if (!sp.IsPeriodic())
                    return ltype == LinkType.Absolute || ltype == LinkType.AbsoluteEdit;
                return ltype == LinkType.Absolute || ltype == LinkType.AbsoluteEdit || ltype == LinkType.AbsoluteList || ltype == LinkType.AbsoluteCombined || ltype == LinkType.Combined || ltype == LinkType.CombinedList;
            }
            if (!sp.IsPeriodic()) return ltype == LinkType.MomentsList;
            return ltype == LinkType.Combined || ltype == LinkType.CombinedList;
        }

        private void Params_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 1: //Ссылка на значение
                    AddLink(LinkField.Value);
                    break;
                case 2: //Ссылка на время
                    AddLink(LinkField.Time);
                    break;
                case 3: //Ссылка по шаблону
                    if (_book.ActiveShape() == null) 
                        AddLinksTemplate();
                    break;
            }
        }

        private void Template_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _book.SysPage.PutValue("CurTemplate", Template.Text);
                _book.LoadCurTemplate();
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при выборе шаблона", ex);
            }
        }

        private void ButUpdate_Click(object sender, EventArgs e)
        {
            _book.UpdateDataFile(false);
            ApplyProject(true);
        }

        private void ButUndo_Click(object sender, EventArgs e)
        {
            _book.UndoTransaction();
        }

        private void ButRedo_Click(object sender, EventArgs e)
        {
            _book.RedoTransaction();
        }

        private void Params_SelectionChanged(object sender, EventArgs e)
        {
            ApplyParam();
        }

        //Выделение по нажатию правой кнопки
        private void Params_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1 && e.Button == MouseButtons.Right)
                Params.CurrentCell = Params[e.ColumnIndex, e.RowIndex];
        }

        private void ButSave_Click(object sender, EventArgs e)
        {
            try
            {
                _book.UpdateDataFile(false, true);
                _book.Workbook.Save();
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка при сохранении отчета");
            }
        }

        private void ButFindLinks_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurParam != null)
                {
                    var f = (FormFindLinks)GeneralRep.RunReporterCommand(ReporterCommand.FindLinks);
                    f.Project.Text = CurProject.Code;
                    f.Code.Text = CurParam.FullCode;
                    f.FirstName.Text = CurParam.ArchiveParam.FirstParam.Name;
                    if (CurParam.ArchiveParam.LastParam != null)
                        f.LastName.Text = CurParam.ArchiveParam.LastParam.Name;
                    f.Find();
                }
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при поиске ссылок на выбранный параметр", ex);
            }
        }

        private void ButLinkProps_Click(object sender, EventArgs e)
        {
            if (PropForm == null)
            {
                PropForm = (FormLinkProperties)_book.RunCommandReporter(ReporterCommand.LinkProperties);
                PropForm.CellComment = CellComment.Text;
                PropForm.PropsPanel.PropsFromDic(_book.CurLinkProps.ToPropertyDictionary());
                ApplyParam();
                MakeLinkTypeList(PropForm.PropsPanel.CellLinkType);
            }
        }

        private void CellLinkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _book.GetLinkType(CellLinkType.Text);
        }

        private void ButLinkSave_Click(object sender, EventArgs e)
        {
            if (_book.ActiveShape() != null) return;
            var tlist = new Transaction();
            _book.BeforeTransaction();
            AddLinkSave(tlist);
            MoveAfterLink();
            _book.AddTransaction(tlist);
        }

        private void CellComment_TextChanged(object sender, EventArgs e)
        {
            if (PropForm != null && PropForm.CellComment != CellComment.Text)
                PropForm.CellComment = CellComment.Text;
        }

        private void butOtmLinks_Click(object sender, EventArgs e)
        {
            if (!GeneralRep.CheckOneSheet(true)) return;
            if (_book.ActiveShape() != null) return;
            if (CurProject != null)
            {
                var tlist = new Transaction();
                _book.BeforeTransaction();
                foreach (DataGridViewRow row in Params.Rows)
                    if (row.GetBool("Otm"))
                        AddLinksTemplate(CurProject.Params[row.Get("Code")], tlist);
                _book.AddTransaction(tlist);
            }
        }

        private void butOtmTrue_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Params.Rows)
                row.Cells["Otm"].Value = true;
        }

        private void butOtmFalse_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Params.Rows)
                row.Cells["Otm"].Value = false;
        }

        private void IsLoadFromCell_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void NextCellShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            NextCellStep.Enabled = NextCellShift.Text != "Нет";
        }

        private void ButClearFilter_Click(object sender, EventArgs e)
        {
            if (_filterForm != null) _filterForm.DeleteFilter();
        }

        private void ButSetFilter_Click(object sender, EventArgs e)
        {
            if (_filterForm != null) _filterForm.SetFilter();
        }

        private void ButLoadLink_Click(object sender, EventArgs e)
        {
            LoadFromCell();
        }
    }
}
