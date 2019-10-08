using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace ReporterCommon
{
    public partial class FormFiltersIntervals : Form
    {
        public FormFiltersIntervals()
        {
            InitializeComponent();
        }

        //Текщая книга
        private ReportBook _book;
        //Форма интервалов
        private FormIntervals _intervalsForm;
        //Фильтр включен
        public bool IsFiltered { get; set; }

        private void FormFiltersIntervals_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            _intervalsForm = (FormIntervals)_book.Forms[ReporterCommand.Intervals];
            var sys = _book.SysPage;
            sys.GetControl(FilterOtm, "FilterOtmIntervals");
            sys.GetControl(RelationOtm, "RelationOtmIntervals");
            sys.GetControl(FilterName, "FilterNameIntervals");
            sys.GetControl(RelationName, "RelationNameIntervals");
            sys.GetControl(FilterBegin1, "FilterBegin1Intervals");
            sys.GetControl(RelationBegin1, "RelationBegin1Intervals");
            sys.GetControl(FilterBegin2, "FilterBegin2Intervals");
            sys.GetControl(RelationBegin2, "RelationBegin2Intervals");
            sys.GetControl(FilterEnd1, "FilterEnd1Intervals");
            sys.GetControl(RelationEnd1, "RelationEnd1Intervals");
            sys.GetControl(FilterEnd2, "FilterEnd2Intervals");
            sys.GetControl(RelationEnd2, "RelationEnd2Intervals");
            sys.GetControl(FilterTimeChange1, "FilterTimeChange1Intervals");
            sys.GetControl(RelationTimeChange1, "RelationTimeChange1Intervals");
            sys.GetControl(FilterTimeChange2, "FilterTimeChange2Intervals");
            sys.GetControl(RelationTimeChange2, "RelationTimeChange2Intervals");
            FilterBegin1.ChangePickerValue(PickerBegin1);
            FilterBegin2.ChangePickerValue(PickerBegin2);
            FilterEnd1.ChangePickerValue(PickerEnd1);
            FilterEnd2.ChangePickerValue(PickerEnd2);
            FilterTimeChange1.ChangePickerValue(PickerTimeChange1);
            FilterTimeChange2.ChangePickerValue(PickerTimeChange2);
            new ToolTip().SetToolTip(ButFilter, "Применить фильтр к журналу отчетов");
            new ToolTip().SetToolTip(ButClearFilter, "Снять фильтр с журнала отчетов");
            new ToolTip().SetToolTip(ButFind, "Поиск в журнале отчетов");
        }

        private void FormFiltersIntervals_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_book.Forms.ContainsKey(ReporterCommand.FilterIntervals))
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                var sys = _book.SysPage;
                sys.PutControl(FilterOtm, "FilterOtmIntrevals");
                sys.PutControl(RelationOtm, "RelationOtmIntervals");
                sys.PutControl(FilterName, "FilterNameIntervals");
                sys.PutControl(RelationName, "RelationNameIntervals");
                sys.PutControl(FilterBegin1, "FilterBegin1Intervals");
                sys.PutControl(RelationBegin1, "RelationBegin1Intervals");
                sys.PutControl(FilterBegin2, "FilterBegin2Intervals");
                sys.PutControl(RelationBegin2, "RelationBegin2Intervals");
                sys.PutControl(FilterEnd1, "FilterEnd1Intervals");
                sys.PutControl(RelationEnd1, "RelationEnd1Intervals");
                sys.PutControl(FilterEnd2, "FilterEnd2Intervals");
                sys.PutControl(RelationEnd2, "RelationEnd2Intervals");
                sys.PutControl(FilterTimeChange1, "FilterTimeChange1Intervals");
                sys.PutControl(RelationTimeChange1, "RelationTimeChange1Intervals");
                sys.PutControl(FilterTimeChange2, "FilterTimeChange2Intervals");
                sys.PutControl(RelationTimeChange2, "RelationTimeChange2Intervals");
                GeneralRep.CloseForm(ReporterCommand.FilterIntervals);
            }
        }

        private void ButFilter_Click(object sender, EventArgs e)
        {
            SetFilter();    
        }

        private void ButClearFilter_Click(object sender, EventArgs e)
        {
            DeleteFilter();
        }

        //Установить фильтр
        public void SetFilter()
        {
            IsFiltered = true;
            ButClearFilter.Enabled = true;
            _intervalsForm.Intervals.EndEdit();
             _intervalsForm.BindingSource1.EndEdit();
            _intervalsForm.BindingNavigator1.Items["ButClearFilter"].Visible = true;
            _intervalsForm.BindingNavigator1.Items["ButSetFilter"].Visible = false;
            _filter = "";
            _err = "";
            if (RelationOtm.Text == "Равно" && !FilterOtm.Text.IsEmpty())
            {
                if (FilterOtm.Text != "Вкл" && FilterOtm.Text != "Откл")
                    _err += "Недопостимое значение для фильтра по отметке; ";
                else _filter += (_filter == "" ? "" : " AND ") + "Otm=" + (FilterOtm.Text == "Вкл" ? "true" : "false");
            }
            if (RelationName.Text == "Пустое") _filter += (_filter == "" ? "" : " AND ") + "IntervalName=''";
            else if (RelationName.Text == "Не пустое") _filter += (_filter == "" ? "" : " AND ") + "IntervalName<>''";
            else if (!FilterName.Text.IsEmpty())
                switch (RelationName.Text)
                {
                    case "Равно":
                        _filter += (_filter == "" ? "" : " AND ") + "IntervalName='" + FilterName.Text + "'";
                        break;
                    case "Содержит":
                        _filter += (_filter == "" ? "" : " AND ") + "IntervalName like '*" + FilterName.Text + "*'";
                        break;
                    case "Начинается с":
                        _filter += (_filter == "" ? "" : " AND ") + "IntervalName like '" + FilterName.Text + "*'";
                        break;
                    case "Кончается на":
                        _filter += (_filter == "" ? "" : " AND ") + "IntervalName like '*" + FilterName.Text + "'";
                        break;
                }
            FilterDate("PeriodBegin", RelationBegin1, FilterBegin1);
            FilterDate("PeriodBegin", RelationBegin2, FilterBegin2);
            FilterDate("PeriodEnd", RelationEnd1, FilterEnd1);
            FilterDate("PeriodEnd", RelationEnd2, FilterEnd2);
            FilterDate("TimeChange", RelationTimeChange1, FilterTimeChange1);
            FilterDate("TimeChange", RelationTimeChange2, FilterTimeChange2);

            if (_err.IsEmpty())
            {
                _intervalsForm.BindingSource1.Filter = _filter;
                _intervalsForm.Intervals.Update();
                foreach (DataGridViewRow r in _intervalsForm.Intervals.Rows)
                    r.MinimumHeight = 20;
            }
            else Different.MessageError(_err);
        }

        //Строка фильтра  и ошибка ввода условий фильтра
        private string _filter;
        private string _err;

        //Проверяет дату на выполнение условия
        private bool CheckFindDate(string sdate, ComboBox relation, TextBox text)
        {
            if (text.Text.IsEmpty()) return true;
            var date = sdate.ToDateTime();
            var d = text.Text.ToDateTime();
            if (d == Different.MinDate || date == Different.MinDate) return true;
            switch (relation.Text)
            {
                case "Равно":
                    return d == date;
                case "Больше":
                    return date > d;
                case "Больше или равно":
                    return date >= d;
                case "Меньше":
                    return date < d;
                case "Меньше или равно":
                    return date <= d;
            }
            return true;
        }

        //Добавляется в строку фильтра условие по дате
        private void FilterDate(string field, ComboBox relation, TextBox text)
        {
            if (!text.Text.IsEmpty() && !relation.Text.IsEmpty())
            {
                DateTime d = text.Text.ToDateTime();
                if (d == Different.MinDate)
                {
                    _err += "Недопустимое значение даты для фильтра; ";
                    return;
                }
                _filter += (_filter == "" ? "" : " AND ");
                switch (relation.Text)
                {
                    case "Равно":
                        _filter += field + "=" + d.ToAccessString();
                        break;
                    case "Больше":
                        _filter += field + ">" + d.ToAccessString();
                        break;
                    case "Больше или равно":
                        _filter += field + ">=" + d.ToAccessString();
                        break;
                    case "Меньше":
                        _filter += field + "<" + d.ToAccessString();
                        break;
                    case "Меньше или равно":
                        _filter += field + "<=" + d.ToAccessString();
                        break;
                }
            }
        }

        //Снять фильтр
        public void DeleteFilter()
        {
            IsFiltered = false;
            ButClearFilter.Enabled = false;
            _intervalsForm.BindingNavigator1.Items["ButClearFilter"].Visible = false;
            _intervalsForm.BindingNavigator1.Items["ButSetFilter"].Visible = true;
            _intervalsForm.BindingSource1.Filter = null;
            _intervalsForm.Intervals.Update();
            foreach (DataGridViewRow r in _intervalsForm.Intervals.Rows)
                r.MinimumHeight = 20;
        }

        private void ButClearFilters_Click(object sender, EventArgs e)
        {
            FilterClear(FilterOtm, RelationOtm);
            FilterClear(FilterName, RelationName);
            FilterClear(FilterBegin1, RelationBegin1);
            FilterClear(FilterBegin2, RelationBegin2);
            FilterClear(FilterEnd1, RelationEnd1);
            FilterClear(FilterEnd2, RelationEnd2);
            FilterClear(FilterTimeChange1, RelationTimeChange1);
            RelationTimeChange1.Text = "Больше или равно";
            FilterClear(FilterTimeChange2, RelationTimeChange2);
            RelationTimeChange2.Text = "Меньше";
        }

        private void FilterClear(Control filter, ComboBox relation)
        {
            relation.Text = "Равно";
            filter.Enabled = true;
            filter.Text = null;
        }

        private void RelationName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterName.Enabled = RelationName.Text != "Пустое" && RelationName.Text != "Не пустое";
        }

        private void PickerBegin1_ValueChanged(object sender, EventArgs e)
        {
            try { FilterBegin1.Text = PickerBegin1.Value.ToString(); } 
            catch { }
        }

        private void PickerBegin2_ValueChanged(object sender, EventArgs e)
        {
            try { FilterBegin2.Text = PickerBegin2.Value.ToString(); }
            catch { }
        }

        private void PickerEnd1_ValueChanged(object sender, EventArgs e)
        {
            try { FilterEnd1.Text = PickerEnd1.Value.ToString(); }
            catch { }
        }

        private void PickerEnd2_ValueChanged(object sender, EventArgs e)
        {
            try { FilterEnd2.Text = PickerEnd2.Value.ToString(); }
            catch { }
        }

        private void PickerTimeChange1_ValueChanged(object sender, EventArgs e)
        {
            try { FilterTimeChange1.Text = PickerTimeChange1.Value.ToString(); }
            catch { }
        }

        private void PickerTimeChange2_ValueChanged(object sender, EventArgs e)
        {
            try { FilterTimeChange2.Text = PickerTimeChange2.Value.ToString(); }
            catch { }
        }

        private void FilterBegin1_Validated(object sender, EventArgs e)
        {
            FilterBegin1.ChangePickerValue(PickerBegin1);
        }

        private void FilterBegin2_Validated(object sender, EventArgs e)
        {
            FilterBegin2.ChangePickerValue(PickerBegin2);
        }

        private void FilterEnd1_TextChanged(object sender, EventArgs e)
        {
            FilterEnd1.ChangePickerValue(PickerEnd1);
        }

        private void FilterEnd2_TextChanged(object sender, EventArgs e)
        {
            FilterEnd2.ChangePickerValue(PickerEnd2);
        }

        private void FilterTimeChange1_TextChanged(object sender, EventArgs e)
        {
            FilterTimeChange1.ChangePickerValue(PickerTimeChange1);
        }

        private void FilterTimeChange2_TextChanged(object sender, EventArgs e)
        {
            FilterTimeChange2.ChangePickerValue(PickerTimeChange2);
        }

        private void ButFind_Click(object sender, EventArgs e)
        {
            var grid = _intervalsForm.Intervals;
            int s = 0;
            try { s = grid.SelectedCells[0].RowIndex + 1;}
            catch { }
            for (int i = 0; i <= grid.Rows.Count; i++)
            {
                var row = grid.Rows[(s + i) % grid.Rows.Count];
                bool b = true;
                if (RelationOtm.Text == "Равно" && !FilterOtm.Text.IsEmpty())
                    b &= (FilterOtm.Text == "Вкл" && row.GetBool("Otm")) || (FilterOtm.Text == "Откл" && !row.GetBool("Otm"));
                var cname = row.Get("IntervalName").ToLower();
                if (RelationName.Text == "Пустое") b &= cname.IsEmpty();
                else if (RelationName.Text == "Не пустое") b &= !cname.IsEmpty();
                else if (!FilterName.Text.IsEmpty())
                {
                    var ftext = FilterName.Text.ToLower();
                    switch (RelationName.Text)
                    {
                        case "Равно":
                            b &= cname == ftext;
                            break;
                        case "Содержит":
                            b &= cname.Contains(ftext);
                            break;
                        case "Начинается с":
                            b &= cname.StartsWith(ftext);
                            break;
                        case "Кончается на":
                            b &= cname.EndsWith(ftext);
                            break;
                    }
                }
                b &= CheckFindDate(row.Get("PeriodBegin"), RelationBegin1, FilterBegin1);
                b &= CheckFindDate(row.Get("PeriodBegin"), RelationBegin2, FilterBegin2);
                b &= CheckFindDate(row.Get("PeriodEnd"), RelationEnd1, FilterEnd1);
                b &= CheckFindDate(row.Get("PeriodEnd"), RelationEnd2, FilterEnd2);
                b &= CheckFindDate(row.Get("TimeChange"), RelationTimeChange1, FilterTimeChange1);
                b &= CheckFindDate(row.Get("TimeChange"), RelationTimeChange2, FilterTimeChange2);
                if (b)
                {
                    var cell = row.Cells[1];
                    cell.Selected = true;
                    grid.CurrentCell = cell;
                    break;
                }
            }
        }
    }
}