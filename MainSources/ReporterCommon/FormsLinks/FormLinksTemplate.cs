using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    public partial class FormLinksTemplate : Form
    {
        public FormLinksTemplate()
        {
            InitializeComponent();
        }

        //Ссылка на книгу
        private ReportBook _book;
        //Ссылка на ячейки текущей строки
        private DataGridViewCellCollection _cells;
        //Текущее имя шаблона
        private string _template;

        //Загрузка списка шаблонов в таблицу
        private void FormLinksTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                _book = GeneralRep.ActiveBook;
                _book.SysPage.GetTemplatesList(Template);
                var items = LinkPropsPanel.CellLinkType.Items;
                items.Clear();
                items.Add(LinkType.Combined.ToRussian());
                items.Add(LinkType.CombinedList.ToRussian());
                items.Add(LinkType.MomentsList.ToRussian());
                items.Add(LinkType.Absolute.ToRussian());
                items.Add(LinkType.AbsoluteEdit.ToRussian());
                items.Add(LinkType.AbsoluteCombined.ToRussian());
                items.Add(LinkType.AbsoluteList.ToRussian());
                items.Add(LinkType.HandInput.ToRussian());
                items.Add(LinkType.Result.ToRussian());
                
                _template = Template.Text = _book.SysPage.GetValue("CurTemplate");
                LoadTemplate();
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при загрузке списка шаблонов", ex);
            }
        }
        
        private void Template_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveTemplate();
            _template = Template.Text;
            LoadTemplate();
        }

        private void Template_TextUpdate(object sender, EventArgs e)
        {
            _template = Template.Text;
        }

        //Заполняет GridTemplate по выбранному шаблону
        private void LoadTemplate()
        {
            var props = _book.SysPage.GerTemplateGeneralProps(Template.Text);
            NextCellShift.Text = props["NextCellShift"];
            NextCellStep.Text = props["NextCellStep"];
            NextCellStep.Enabled = NextCellShift.Text != "Нет";
            var list = _book.SysPage.GetTemplate(Template.Text);
            TemplateGrid.Rows.Clear();
            LinkPropsPanel.Visible = false;
            foreach (var s in list)
            {
                int rn = TemplateGrid.Rows.Add();
                var cells = TemplateGrid.Rows[rn].Cells;
                cells["Props"].Value = s;
                var dic = s.ToPropertyDicS();
                cells["CodeForming"].Value = dic.Get("CodeForming", "<Полный код>");
                cells["CellAction"].Value = dic.Get("CellAction", CellActionType.Link.ToRussian());
                cells["Field"].Value = dic.Get("Field", "Значение");
                cells["CellLinkType"].Value = dic["LinkType"].ToLinkType().ToRussian();
                cells["X"].Value = dic.Get("X", "0");
                cells["Y"].Value = dic.Get("Y", "0");
            }
            if (TemplateGrid.Rows.Count > 0)
            {
                TemplateGrid.Rows[0].Selected = true;
                LoadParam();
            }
            else _cells = null;
            UpdateTemplateInBook(Template.Text);
        }

        //Обновляет текущий шаблон в книге и форме установки ссылок
        private void UpdateTemplateInBook(string templateName)
        {
            if (_book.Forms.ContainsKey(ReporterCommand.PutLinks))
            {
                var comboBox = ((FormPutLinks)_book.Forms[ReporterCommand.PutLinks]).Template;
                _book.SysPage.GetTemplatesList(comboBox);
                comboBox.Text = templateName;
            }
            _book.SysPage.PutValue("CurTemplate", templateName);
            _book.LoadCurTemplate();
        }

        //Сохранение шаблона, templateName - имя шаблона
        private void SaveTemplate()
        {
            if (_template.IsEmpty()) return;
            if (Template.Text.IsEmpty())
            {
                Different.MessageError("Не указано имя шаблона");
                return;
            }
            int tmp;
            if (NextCellShift.Text != "Нет" && (!int.TryParse(NextCellStep.Text, out tmp) || tmp < -1000 || tmp > 1000))
                Different.MessageError("Не корректно заполнена величина сдвига после установки ссылки");
            SaveParam();
            var list = new List<string>();
            foreach (DataGridViewRow row in TemplateGrid.Rows)
            {
                var p = row.Get("Props");
                if (p != null) list.Add(p);
            }
            _book.SysPage.PutTemplate(_template, "NextCellShift=" + NextCellShift.Text + ";NextCellStep=" + NextCellStep.Text, list);
            if (!Template.Items.Contains(_template))
                Template.Items.Add(_template);
            UpdateTemplateInBook(_template);
        }

        //Сохранение предыдущего выбранного параметра в таблицу
        private void SaveParam()
        {
            if (_cells != null)
            {
                try
                {
                    var ac = LinkCellAction.Text.ToCellAction();
                    string field = ac != CellActionType.Text ? CellField.Text : "";
                    int ix = LinkX.Text.ToInt(); 
                    int iy = LinkY.Text.ToInt();
                    _cells["CellAction"].Value = LinkCellAction.Text;
                    _cells["Field"].Value = ac != CellActionType.Text ? CellField.Text : "";
                    _cells["X"].Value = ix.ToString();
                    _cells["Y"].Value = iy.ToString();
                    string codeForming = (WriteText.Visible ? WriteText.Text : LinkCodeForming.Visible ? LinkCodeForming.Text : "") ?? "<Полный код>";
                    _cells["CodeForming"].Value = codeForming;
                    string s = "CodeForming=" + codeForming + ";CellAction=" + LinkCellAction.Text + ";Field=" + field + ";X=" + ix + ";Y=" + iy + ";";
                    if (ac == CellActionType.Save) s += "LinkType=Сохранение";
                    else if (ac != CellActionType.Text && field.ToLinkField().IsValueField())
                    {
                        s += LinkPropsPanel.PropsString();
                        if (LinkPropsPanel.LinkPropsError != "") 
                            Different.MessageError(LinkPropsPanel.LinkPropsError, "Не правильно заполнены поля");
                    }
                    _cells["Props"].Value = s;
                    _cells["LinkType"].Value = !LinkPropsPanel.Visible ? "" : LinkPropsPanel.CellLinkType.Text;
                }
                catch { }
            }
        }

        //Загрузка нового выбранного параметра из таблицы
        private void LoadParam()
        {
            _cells = TemplateGrid.CurrentRow == null ? null : TemplateGrid.CurrentRow.Cells;
            try
            {
                var dic =  (_cells == null ? "" : _cells.Get("Props")).ToPropertyDictionary();
                LinkCellAction.Text = dic.Get("CellAction", CellActionType.Link.ToRussian());
                LinkCodeForming.Text = WriteText.Text = dic.Get("CodeForming", "<Полный код>");
                ApplyCellAction();
                CellField.Text = dic.Get("Field", "Значение");
                ApplyLinkField();
                LinkX.Text = dic.Get("X", "0");
                LinkY.Text = dic.Get("Y", "0");
                if (LinkPropsPanel.Visible)
                    LinkPropsPanel.PropsFromDic(dic);
            }
            catch { }
        }
        
        private void TemplateGrid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                SaveParam();
                LoadParam();
            }
            catch (Exception ex)
            {
                GeneralRep.ShowError("Ошибка при выборе параметра для шаблона", ex);
            }
        }
        
        private void FormLinksTemplate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Template.Text.IsEmpty())
            {
                if (!Different.MessageQuestion("Не указано имя шаблона. Выйти без сохранения?"))
                    e.Cancel = true;
            }
            else SaveTemplate();
            if (!e.Cancel) _book.CloseForm(ReporterCommand.LinksTemplate);    
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            SaveTemplate();
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            if (Different.MessageQuestion("Вы действительно хотите удалить шаблон " + Template.Text + "?","Удаление"))
            {
                try
                {
                    _book.SysPage.DeleteTemplate(Template.Text);
                    _book.SysPage.GetTemplatesList(Template);
                    _book.CurLinkTemplate.Clear();
                    _template = Template.Text = null;
                    _cells = null;
                    UpdateTemplateInBook(null);
                }
                catch (Exception ex)
                {
                    GeneralRep.ShowError("Ошибка при удалении шаблона", ex);
                }
            }
        }

        private void LinkCellAction_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (_cells != null)
            {
                var ac = LinkCellAction.Text.ToCellAction();
                _cells["CellAction"].Value = LinkCellAction.Text;
                if (ac != CellActionType.Link || !CellField.Text.ToLinkField().IsValueField())
                    _cells["CellLinkType"].Value = "";
                if (ac == CellActionType.Text && !WriteText.Visible) WriteText.Text = "";
                if (ac != CellActionType.Text && WriteText.Visible)
                    LinkCodeForming.Text = "<Полный код>";
            }
            ApplyCellAction();
        }

        //Вызывается при изменении CellAction
        private void ApplyCellAction()
        {
            var ac = LinkCellAction.Text.ToCellAction();
            if (ac == CellActionType.Text)
            {
                WriteText.Visible = true;
                CellField.Visible = LabelField.Visible = LinkCodeForming.Visible = LabelCodeForming.Visible = false;
            }
            else
            {
                WriteText.Visible = false;
                CellField.Visible = LabelField.Visible = true;
                LinkCodeForming.Visible = LabelCodeForming.Visible = ac != CellActionType.Save;    
                var s = CellField.Text;
                CellField.Items.Clear();
                AddFieldToList(LinkField.Value, s);
                AddFieldToList(LinkField.Time, s);
                AddFieldToList(LinkField.Nd, s);
                AddFieldToList(LinkField.Number, s);
                AddFieldToList(LinkField.Code, s);
                AddFieldToList(LinkField.CodeParam, s);
                AddFieldToList(LinkField.CodeSubParam, s);
                AddFieldToList(LinkField.Name, s);
                AddFieldToList(LinkField.SubName, s);
                AddFieldToList(LinkField.Task, s);
                AddFieldToList(LinkField.Units, s);
                AddFieldToList(LinkField.DataType, s);
                AddFieldToList(LinkField.SuperProcessType, s);
                AddFieldToList(LinkField.CalcParamType, s);
                AddFieldToList(LinkField.Comment, s);
                AddFieldToList(LinkField.SubComment, s);
                AddFieldToList(LinkField.Min, s);
                AddFieldToList(LinkField.Max, s);
                try { CellField.Text = s; }
                catch { CellField.Text = (string)CellField.Items[0]; }
            }
            ApplyLinkField();
        }

        private void AddFieldToList(LinkField field, string oldValue)
        {
            var ca = LinkCellAction.Text.ToCellAction();
            if (ca == CellActionType.Save)
            {
                if (field == LinkField.Value)
                {
                    CellField.Items.Add(field.ToRussian());
                    CellField.Text = LinkField.Value.ToRussian();
                }
            }
            else
            {
                if ((ca == CellActionType.Link && field.IsLinkField()) || (ca == CellActionType.Value && !field.IsValueField()))
                    CellField.Items.Add(field.ToRussian());
                if (field.ToRussian() == oldValue)
                    CellField.Text = oldValue;    
            }
        }

        //Вызывается при изменении типа информации ссылки
        private void ApplyLinkField()
        {
            if (_cells != null)
            {
                var dic = _cells.Get("Props").ToPropertyDictionary();
                var ac = LinkCellAction.Text.ToCellAction();
                if (CellField.Text.ToLinkField().IsValueField() && ac != CellActionType.Save && ac != CellActionType.Text)
                {
                    LinkPropsPanel.Visible = true;
                    LinkPropsPanel.PropsFromDic(dic);
                }
                else LinkPropsPanel.Visible = false;    
            }
        }

        private void LinkField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cells != null)
            {
                _cells["Field"].Value = CellField.Text;
                ApplyLinkField();
                _cells["CellLinkType"].Value = !LinkPropsPanel.Visible ? "" : LinkPropsPanel.CellLinkType.Text;
            }
        }

        private void ButAddLink_Click(object sender, EventArgs e)
        {
            SaveParam();
            var n = TemplateGrid.Rows.Add();
            TemplateGrid.Rows[n].Selected = true;
            _cells = TemplateGrid.Rows[n].Cells;
            _cells["X"].Value = LinkX.Text;
            _cells["Y"].Value = LinkY.Text;
            _cells["CellAction"].Value = LinkCellAction.Text;
            _cells["CodeForming"].Value = WriteText.Visible ? WriteText.Text : LinkCodeForming.Text;
            _cells["Field"].Value = WriteText.Visible ? "" : CellField.Text;
            _cells["CellLinkType"].Value = !LinkPropsPanel.Visible ? "" : LinkPropsPanel.CellLinkType.Text;
        }
        
        private void LinkX_TextChanged(object sender, EventArgs e)
        {
            if (_cells != null)
                _cells["X"].Value = LinkX.Text;
        }

        private void LinkY_TextChanged(object sender, EventArgs e)
        {
            if (_cells != null)
                _cells["Y"].Value = LinkY.Text;
        }

        private void LinkPropsPanel_OnLinkTypeChange(object sender, EventArgs e)
        {
            if (_cells != null)
                _cells["CellLinkType"].Value = !LinkPropsPanel.Visible ? "" : LinkPropsPanel.CellLinkType.Text;
        }

        private void LinkCodeForming_TextUpdate(object sender, EventArgs e)
        {
            if (_cells != null)
                _cells["CodeForming"].Value = LinkCodeForming.Text;
        }

        private void WriteText_TextChanged_1(object sender, EventArgs e)
        {
            if (_cells != null)
                _cells["CodeForming"].Value = WriteText.Text;
        }
        
        private void ButCreate_Click(object sender, EventArgs e)
        {
            SaveTemplate();
            int i = 1;
            while (Template.Items.Contains("Новый шаблон " + i))
                i++;
            _template = Template.Text = "Новый шаблон " + i;
            LoadTemplate();
            _cells = null;
        }

        private void ButExport_Click(object sender, EventArgs e)
        {
            if (Template.Text.IsEmpty())
            {
                Different.MessageError("Не указано имя шаблона");
                return;
            }
            SaveTemplate();
            var f = new DialogCommand(DialogType.OpenExcel)
                {
                    DialogTitle = "Файл отчета для экспорта шаблона",
                    ErrorMessage = "Указан недопустимый файл отчета"
                }.Run();
            try
            {
                if (!f.IsEmpty() )
                {
                    var t = _book.SysPage.GetTemplate(Template.Text);
                    var p = _book.SysPage.GerTemplateGeneralProps(Template.Text);
                    Workbook rbook = null;
                    ReportBook book = null;
                    bool notb = true;
                    foreach (var b in GeneralRep.Books.Values)
                    {
                        try
                        {
                            if (b.Workbook.FullName == f)
                            {
                                notb = false;
                                book = b;
                                rbook = b.Workbook;
                            }
                        }
                        catch { }
                    }
                    if (notb)
                    {
                        rbook = GeneralRep.Application.Workbooks.Open(f);
                        book = GeneralRep.ActiveBook;
                    }
                    if (book.SysPage.TemplateX(Template.Text) == -1 || Different.MessageQuestion("В отчете " + book.Code + " уже есть шаблон " + Template.Text + ". Заменить?"))
                    {
                        book.SysPage.PutTemplate(Template.Text, p.ToPropertyString(), t);
                        rbook.Save();
                    }
                    if (notb) rbook.Close();
                }
            }
            catch { Different.MessageError("Ошибка при экспорте шаблона в файл " + f);}
        }

        private void NextCellShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            NextCellStep.Enabled = NextCellShift.Text != "Нет";
        }

        private void TemplateGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                    LinkX.Text = TemplateGrid.Rows[e.RowIndex].Get("X");
                if (e.ColumnIndex == 1)
                    LinkY.Text = TemplateGrid.Rows[e.RowIndex].Get("Y");
                if (e.ColumnIndex == 2)
                    LinkCodeForming.Text = TemplateGrid.Rows[e.RowIndex].Get("CodeForming");    
            }   
        }
    }
}
