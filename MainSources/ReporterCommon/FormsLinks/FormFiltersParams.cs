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

namespace ReporterCommon
{
    public partial class FormFiltersParams : Form
    {
        public FormFiltersParams()
        {
            InitializeComponent();
        }

        //Текущие книга и проект
        private ReportBook _book;
        private ReportProjectForLinks CurProject
        {
            get { return ((FormPutLinks)_book.Forms[ReporterCommand.PutLinks]).CurProject; }
        }
        //Форма установки ссылок
        private FormPutLinks _linksForm;
        //Фильтр включен
        public bool IsFiltered { get; set; }
        
        private void FormFiltersParams_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            _linksForm = (FormPutLinks)_book.Forms[ReporterCommand.PutLinks];
            MakeFiltersLists();
            var sys = _book.SysPage;
            sys.GetControl(FilterOtm);
            sys.GetControl(RelationOtm);
            sys.GetControl(FilterFullCode);
            sys.GetControl(RelationFullCode);
            sys.GetControl(FilterCode);
            sys.GetControl(RelationCode);
            sys.GetControl(FilterSubCode);
            sys.GetControl(RelationSubCode);
            sys.GetControl(FilterName);
            sys.GetControl(RelationName);
            sys.GetControl(FilterComment);
            sys.GetControl(RelationComment);
            sys.GetControl(FilterTask);
            sys.GetControl(RelationTask);
            sys.GetControl(FilterUnits);
            sys.GetControl(RelationUnits);
            sys.GetControl(FilterDataType);
            sys.GetControl(RelationDataType);
            sys.GetControl(FilterSuperProcess);
            sys.GetControl(RelationSuperProcess);
            sys.GetControl(FilterCalcParamType);
            sys.GetControl(RelationCalcParamType);
            new ToolTip().SetToolTip(ButFilter, "Применить фильтр к списку параметров");
            new ToolTip().SetToolTip(ButClearFilter, "Снять фильтр со списка параметров");
            new ToolTip().SetToolTip(ButFind, "Поиск в списке параметров");
        }

        private void FormFiltersParams_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_book.Forms.ContainsKey(ReporterCommand.FilterParams))
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                var sys = _book.SysPage;
                sys.PutControl(FilterOtm);
                sys.PutControl(RelationOtm);
                sys.PutControl(FilterFullCode);
                sys.PutControl(RelationFullCode);
                sys.PutControl(FilterCode);
                sys.PutControl(RelationCode);
                sys.PutControl(FilterSubCode);
                sys.PutControl(RelationSubCode);
                sys.PutControl(FilterName);
                sys.PutControl(RelationName);
                sys.PutControl(FilterComment);
                sys.PutControl(RelationComment);
                sys.PutControl(FilterTask);
                sys.PutControl(RelationTask);
                sys.PutControl(FilterUnits);
                sys.PutControl(RelationUnits);
                sys.PutControl(FilterDataType);
                sys.PutControl(RelationDataType);
                sys.PutControl(FilterSuperProcess);
                sys.PutControl(RelationSuperProcess);
                sys.PutControl(FilterCalcParamType);
                sys.PutControl(RelationCalcParamType);
                GeneralRep.CloseForm(ReporterCommand.FilterParams);
            }
        }

        //Заполняет выпадающие списки для фильтров
        public void MakeFiltersLists()
        {
            MakeFilterItems(FilterTask, CurProject.Tasks);
            MakeFilterItems(FilterUnits, CurProject.Units);
            MakeFilterItems(FilterDataType, CurProject.DataTypes);
            MakeFilterItems(FilterCalcParamType, CurProject.CalcParamTypes);
            MakeFilterItems(FilterSuperProcess, CurProject.SuperProcesses);
        }

        //Заполняет источник для выпадающего списка по HashSet 
        private void MakeFilterItems(ComboBox field, SetS set)
        {
            field.Items.Clear();
            var arr = set.Values.ToArray();
            Array.Sort(arr);
            foreach (var it in arr)
                field.Items.Add(it);
        }

        //Проверяет сответствие параметра papam фильтрам, forFind - используется для поиска, otm - параметр отмечен в списке
        public bool CheckParam(ReportParam param, bool forFind = false)
        {
            if (!IsFiltered && !forFind) return true;
            bool b = CheckParamProperty(param, LinkField.Code, RelationFullCode, FilterFullCode);
            b &= CheckParamProperty(param, LinkField.CodeParam, RelationCode, FilterCode);
            b &= CheckParamProperty(param, LinkField.CodeSubParam, RelationSubCode, FilterSubCode);
            b &= CheckParamProperty(param, LinkField.Name, RelationName, FilterName);
            b &= CheckParamProperty(param, LinkField.Comment, RelationComment, FilterComment);
            b &= CheckParamProperty(param, LinkField.Task, RelationTask, FilterTask);
            b &= CheckParamProperty(param, LinkField.DataType, RelationDataType, FilterDataType);
            b &= CheckParamProperty(param, LinkField.Units, RelationUnits, FilterUnits);
            b &= CheckParamProperty(param, LinkField.SuperProcessType, RelationSuperProcess, FilterSuperProcess);
            b &= CheckParamProperty(param, LinkField.CalcParamType, RelationCalcParamType, FilterCalcParamType);
            if (RelationOtm.Text == "Равно" && (FilterOtm.Text == "Вкл" || FilterOtm.Text == "Откл"))
                b &= (FilterOtm.Text == "Вкл" && param.Otm) || (FilterOtm.Text == "Откл" && !param.Otm);
            return b;
        }

        //Проверка соответствия строки prop, фильтру filter, с отношением relation
        private bool CheckParamProperty(ReportParam param, LinkField field, Control relation, Control filter)
        {
            string prop = param.GetField(field);
            if (relation.Text == "Не пустое") return !prop.IsEmpty();
            if (relation.Text == "Пустое") return prop.IsEmpty();
            if (filter.Text.IsEmpty()) return true;
            string p = (prop ?? "").ToLower();
            string f = filter.Text.ToLower();
            switch (relation.Text)
            {
                case "Равно":
                    return p == f;
                case "Не равно":
                    return p != f;
                case "Содержит":
                    return p.IndexOf(f) != -1;
                case "Не содержит":
                    return p.IndexOf(f) == -1;
                case "Начинается с":
                    return p.IndexOf(f) == 0;
                case "Кончается на":
                    return f.Length <= p.Length && p.LastIndexOf(f) == p.Length - f.Length;
                case "По шаблону":
                    return p.CheckFilter(f);
            }
            return true;
        }

        private void ButFind_Click(object sender, EventArgs e)
        {
            _linksForm.FindParam();
        }
        
        //Установить фильтр
        private void ButFilter_Click(object sender, EventArgs e)
        {
            SetFilter();
        }

        //Установить фильтр
        public void SetFilter()
        {
            IsFiltered = true;
            ButClearFilter.Enabled = true;
            _linksForm.Params.EndEdit(); 
            _linksForm.BindingNavigator1.Items["ButClearFilter"].Visible = true;
            _linksForm.BindingNavigator1.Items["ButSetFilter"].Visible = false;
            if (!_linksForm.IsApplyProject)
                _linksForm.ApplyProject(false);
        }

        //Снять фильтр
        public void DeleteFilter()
        {
            IsFiltered = false;
            ButClearFilter.Enabled = false;
            _linksForm.BindingNavigator1.Items["ButClearFilter"].Visible = false;
            _linksForm.BindingNavigator1.Items["ButSetFilter"].Visible = true;
            if (!_linksForm.IsApplyProject)
                _linksForm.ApplyProject(false);
        }

        private void ButClearFilter_Click(object sender, EventArgs e)
        {
            DeleteFilter();
        }

        private void ButClearFilters_Click(object sender, EventArgs e)
        {
            FilterClear(FilterOtm, RelationOtm);
            FilterClear(FilterFullCode, RelationFullCode);
            FilterClear(FilterCode, RelationCode);
            FilterClear(FilterSubCode, RelationSubCode);
            FilterClear(FilterName, RelationName);
            FilterClear(FilterComment, RelationComment);
            FilterClear(FilterTask, RelationTask);
            FilterClear(FilterDataType, RelationDataType);
            FilterClear(FilterUnits, RelationUnits);
            FilterClear(FilterSuperProcess, RelationSuperProcess);
            FilterClear(FilterCalcParamType, RelationCalcParamType);
        }

        private void FilterClear(Control filter, ComboBox relation)
        {
            relation.Text = "Равно";
            filter.Enabled = true;
            filter.Text = null;
        }

        private void FilterEnabled(Control filter, ComboBox relation)
        {
            filter.Enabled = relation.Text != "Пустое" && relation.Text != "Не пустое";
        }

        private void RelationFullCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterFullCode, RelationFullCode);
        }
        
        private void RelationCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterCode, RelationCode);
        }

        private void RelationSubCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterSubCode, RelationSubCode);
        }

        private void RelationName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterName, RelationName);
        }

        private void RelationComment_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterComment, RelationComment);
        }

        private void RelationTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterTask, RelationTask);
        }

        private void RelationUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterUnits, RelationUnits);
        }

        private void RelationDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterDataType, RelationDataType);
        }

        private void RelationSuperProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterSuperProcess, RelationSuperProcess);
        }

        private void RelationCalcParamType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEnabled(FilterCalcParamType, RelationCalcParamType);
        }
    }
}
