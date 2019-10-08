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
using Calculation;
using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    public partial class FormAbsoluteEdit : Form
    {
        public FormAbsoluteEdit()
        {
            InitializeComponent();
        }

        //Поле ссылки для выбранной ячейки
        private LinkField _linkField;
        //Свойства выбранного параметра
        private string _project;
        private string _code;
        private DataType _type;

        private void butChangeValue_Click(object sender, EventArgs e)
        {
            //if (!Time.Text.IsEmpty() && !Time.Text.IsOfType(DataType.Time))
            //{
            //    Different.MessageError("Введено недопустимое время");
            //    return;
            //}
            if (!Value.Text.IsOfType(_type))
            {
                Different.MessageError("Введено недопустимое значение");
                return;
            }
            var proj = GeneralRep.ActiveBook.UsedProjects[_project];

            //DateTime time = Time.Text.IsEmpty() ? OldTime.Text.ToDateTime() : Time.Text.ToDateTime();
            DateTime time = OldTime.Text.ToDateTime();
            string val = Value.Text;
            if (proj.Params.ContainsKey(_code) && proj.Params[_code].ArchiveParam.DataType.LessOrEquals(DataType.Real))
                val = val.Replace(",", ".");
            if (!proj.AbsoluteEditValues.ContainsKey(_code))
                proj.AbsoluteEditValues.Add(_code, new HandInputParam(_project, _code, val, time));
            else
            {
                var hip = proj.AbsoluteEditValues[_code];
                hip.Time = time;
                hip.Value = val;   
            }
            if (_linkField == LinkField.Value) 
                GeneralRep.Application.ActiveCell.Value2 = val;
            //if (_linkField == LinkField.Time && !Time.Text.IsEmpty()) 
            //    GeneralRep.Application.ActiveCell.Value2 = Time.Text.ToDateTime();
        }

        private void FormAbsoluteEdit_Load(object sender, EventArgs e)
        {
            GeneralRep.Application.SheetSelectionChange += OnSelectionChange;
            LoadFromCell();
        }

        private void OnSelectionChange(object sh, Range range)
        {
            LoadFromCell();
        }

        private void LoadFromCell()
        {
            Props.Rows.Clear();
            OldTime.Text = null;
            OldValue.Text = null;
            var cell = GeneralRep.Application.ActiveCell;
            if (cell == null || cell.Comment == null) return;
            var dic = cell.Comment.Text().ToPropertyDictionary();
            if (!dic.ContainsKey("LinkType") || (dic["LinkType"].ToLinkType() != LinkType.AbsoluteEdit && dic["LinkType"].ToLinkType() != LinkType.AbsoluteList && dic["LinkType"].ToLinkType() != LinkType.AbsoluteCombined) 
                || !dic.ContainsKey("Project") || !dic.ContainsKey("Code") || !dic.ContainsKey("Field") || (dic["Field"].ToLinkField() != LinkField.Time && dic["Field"].ToLinkField() != LinkField.Value))
                return;
            var projs = GeneralRep.ActiveBook.UsedProjects;
            _project = dic["Project"]; 
            _code = dic["Code"];
            if (projs == null || !projs.ContainsKey(_project) || !projs[_project].Params.ContainsKey(_code)) 
                return;

            _linkField = dic["Field"].ToLinkField();
            AddToGrid("Проект", _project);
            AddToGrid("Код", dic["Code"]);
            var pr = projs[_project];
            var par = projs[_project].Params[_code].ArchiveParam;
            AddToGrid("Имя", par.FirstParam.Name);
            if (par.FirstParam != par.LastParam && par.LastParam != null) AddToGrid("", par.LastParam.Name);
            AddToGrid("Ед.изм.", par.Units);
            _type = par.DataType;
            AddToGrid("Тип", _type.ToRussian());

            if (pr.AbsoluteEditValues.ContainsKey(_code))
            {
                var apar = pr.AbsoluteEditValues[_code];
                OldValue.Text = apar.OldValue;
                OldTime.Text = apar.OldTime.ToString();
                Value.Enabled = true;
                Time.Enabled = true;
                Value.Text = apar.Value;
                //Time.Text = apar.Time == Different.MinDate ? null : apar.Time.ToString();
            }
            //Time.ChangePickerValue(TimePicker);
        }

        private void AddToGrid(string prop, string val)
        {
            int rn = Props.Rows.Add();
            var cells = Props.Rows[rn].Cells;
            cells["PropName"].Value = prop;
            cells["PropValue"].Value = val;
        }

        private void FormAbsoluteEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            GeneralRep.CloseForm(ReporterCommand.AbsoluteEdit); 
            GeneralRep.Application.SheetSelectionChange -= OnSelectionChange;
        }

        private void TimePicker_ValueChanged(object sender, EventArgs e)
        {
            try { Time.Text = TimePicker.Value.ToString();}
            catch {}
        }

        private void Time_Validated(object sender, EventArgs e)
        {
            Time.ChangePickerValue(TimePicker);
        }
    }
}
