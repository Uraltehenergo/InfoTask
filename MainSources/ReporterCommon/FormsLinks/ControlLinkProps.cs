using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace ReporterCommon
{
    public partial class ControlLinkProps : UserControl
    {
        public ControlLinkProps()
        {
            InitializeComponent();
        }

        private void ControlLinkProps_Load(object sender, EventArgs e)
        {
            ValuesOrder.Text = "по возрастанию времени";
        }

        //Установка видимости и размеров по типу ссылки
        public void ApplyLinkType()
        {
            var lt = CellLinkType.Text.ToLinkType();
            int h = LinkTypePanel.Top + LinkTypePanel.Height;
            AllowEdit.Visible = lt == LinkType.Absolute || lt == LinkType.AbsoluteEdit || lt == LinkType.Result || lt == LinkType.Combined || lt == LinkType.Save || lt == LinkType.HandInput;
            if (lt == LinkType.Save || lt == LinkType.HandInput)
            {
                AllowEdit.Checked = true;
                AllowEdit.Enabled = false;
            }
            else AllowEdit.Enabled = true;
            DistancePanel.Visible = lt == LinkType.CombinedList || lt == LinkType.MomentsList || lt == LinkType.AbsoluteList;
            if (DistancePanel.Visible) h += DistancePanel.Height;
            if (lt != LinkType.CombinedList && lt != LinkType.AbsoluteList) LengthPanel.Visible = false;
            else
            {
                LengthPanel.Visible = true;
                LengthPanel.Top = h;
                h += LengthPanel.Height;
            }
            PartPanel.Visible = lt == LinkType.Combined || lt == LinkType.CombinedList || lt == LinkType.MomentsList;
            PartPanel.Top = h;
            PartNumberPanel.Visible = lt == LinkType.CombinedList || lt == LinkType.MomentsList || lt == LinkType.AbsoluteList;
            if (PartPanel.Visible) h += PartPanel.Height;
            Height = h;

            Invalidate();
            if (_onLinkTypeChange != null)
                _onLinkTypeChange(this, new EventArgs());
        }

        //Событие на обновление типа ссылки, для внешней формы
        private event EventHandler _onLinkTypeChange;
        [Category("Change")]
        public event EventHandler OnLinkTypeChange
        {
            add { _onLinkTypeChange += value; }
            remove { _onLinkTypeChange -= value; }
        }

        private void CellLinkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyLinkType();
        }

        //Свойства ссылки в виде строки
        public string PropsString()
        {
            _linkPropsError = "";
            string res = "LinkType=" + CellLinkType.Text.ToLinkType().ToEnglish() +";";
            if (AllowEdit.Visible) 
                res += "AllowEdit=" + (AllowEdit.Checked ? "True;" : "False;");
            if (DistancePanel.Visible)
            {
                int x = 0, y = 0;
                if (!ValueDistanceX.Text.IsEmpty() && int.TryParse(ValueDistanceX.Text, out x) && (x != 0))
                    res += "ValueDistanceX=" + x + ";";
                if (!ValueDistanceY.Text.IsEmpty() && int.TryParse(ValueDistanceY.Text, out y) && (y != 0))
                    res += "ValueDistanceY=" + y + ";";
                if (x == 0 && y == 0)
                    _linkPropsError = "Не заполнено расстояние между значениями";
                res += "ValuesOrder=" + (ValuesOrder.Text == "по убываению времени" ? "DecTime" : "IncTime") + ";";
                if (SkipFirstCell.Checked) res += "SkipFirstCell=True;";
            }
            if (LengthPanel.Visible)
            {
                int d = 0, h = 0, m = 0, s = 0;
                if (!LengthDay.Text.IsEmpty() && int.TryParse(LengthDay.Text, out d) && (d > 0))
                    res += "LengthDay=" + d + ";";
                if (!LengthHour.Text.IsEmpty() && int.TryParse(LengthHour.Text, out h) && (h > 0))
                    res += "LengthHour=" + h + ";";
                if (!LengthMinute.Text.IsEmpty() && int.TryParse(LengthMinute.Text, out m) && (m > 0))
                    res += "LengthMinute=" + m + ";";
                if (d == 0 && h == 0 && m == 0 & s == 0)
                    _linkPropsError = "Длина каждого интервала должна быть правильно заполнена и не равна 0";
            }
            if (PartNumberPanel.Visible && PartByNumber.Checked)
            {
                res += "GetPartNumber=True;";
                int b = 0, e = 0;
                if (!PartBeginNumber.Text.IsEmpty() && int.TryParse(PartBeginNumber.Text, out b) && (b > 0))
                    res += "PartBeginNumber=" + b + ";";
                if (!PartEndNumber.Text.IsEmpty() && int.TryParse(PartEndNumber.Text, out e) && (e > 0))
                    res += "PartEndNumber=" + e + ";";
                if (b == 0 && e == 0)
                    _linkPropsError = "Номер значения начала или конца части интервала должен быть заполнен и не равен 0";
                else if (b >= e)
                    _linkPropsError = "Номер конца части интервала должен быть больше номера начала части интервала";
            }

            if (PartPanel.Visible && PartByTime.Checked)
            {
                res += "GetPartTime=True;";
                int db = 0, hb = 0, mb = 0, de = 0, he = 0, me = 0;
                if (!PartBeginDay.Text.IsEmpty() && int.TryParse(PartBeginDay.Text, out db) && (db > 0))
                    res += "PartBeginDay=" + db + ";";
                if (!PartBeginHour.Text.IsEmpty() && int.TryParse(PartBeginHour.Text, out hb) && (hb > 0))
                    res += "PartBeginHour=" + hb + ";";
                if (!PartBeginMinute.Text.IsEmpty() && int.TryParse(PartBeginMinute.Text, out mb) && (mb > 0))
                    res += "PartBeginMinute=" + mb + ";";
                if (!PartEndDay.Text.IsEmpty() && int.TryParse(PartEndDay.Text, out de) && (de > 0))
                    res += "PartEndDay=" + de + ";";
                if (!PartEndHour.Text.IsEmpty() && int.TryParse(PartEndHour.Text, out he) && (he > 0))
                    res += "PartEndHour=" + he + ";";
                if (!PartEndMinute.Text.IsEmpty() && int.TryParse(PartEndMinute.Text, out me) && (me > 0))
                    res += "PartEndMinute=" + me + ";";
                if (db == 0 && hb == 0 && mb == 0 && de == 0 && he == 0 && me == 0)
                    _linkPropsError = "Начало или конец части интервала должен быть правильно заполнен и не равен 0";
                else if (new TimeSpan(db, hb, mb, 0) >= new TimeSpan(de, he, me, 0))
                    _linkPropsError = "Конец части интервала должен быть позже начала части интервала";
            }
            return res;
        }

        //Ошибка, если поля заполнены не правильно
        private string _linkPropsError;
        public string LinkPropsError { get { return _linkPropsError; } }
        
        //Загружает свойства в ячейки из словаря свойств
        public void PropsFromDic(Dictionary<string, string> dic)
        {
            var linkType = dic.Get("LinkType").ToLinkType();
            var lt = linkType.ToRussian();
            if (lt != CellLinkType.Text) CellLinkType.Text = lt;
            ApplyLinkType();

            if (AllowEdit.Visible)
                AllowEdit.Checked = dic.GetBool("AllowEdit");

            if (DistancePanel.Visible)
            {
                ValueDistanceX.Text = dic.Get("ValueDistanceX", "");
                ValueDistanceY.Text = dic.Get("ValueDistanceY", "");
                string ord = dic.Get("ValuesOrder", "");
                ValuesOrder.Text = ord == "DecTime" ? "по убыванию времени" : "по возрастанию времени";
                SkipFirstCell.Checked = dic.ContainsKey("SkipFirstCell") && dic["SkipFirstCell"].ToLower() == "true";
            }

            if (linkType == LinkType.CombinedList || linkType == LinkType.AbsoluteList)
            {
                LengthDay.Text = dic.Get("LengthDay", "");
                LengthHour.Text = dic.Get("LengthHour", "");
                LengthMinute.Text = dic.Get("LengthMinute", "");
            }
            if (dic.Get("GetPartNumber", "").ToLower() == "true")
            {
                PartByNumber.Checked = true;
                PartBeginNumber.Text = dic.Get("PartBeginNumber", "");
                PartEndNumber.Text = dic.Get("PartEndNumber", "");
            }
            else
            {
                if (PartNumberPanel.Visible)
                    PartByNumber.Checked = false;
            }
            if (dic.Get("GetPartTime", "").ToLower() == "true")
            {
                PartByTime.Checked = true;
                PartBeginDay.Text = dic.Get("PartBeginDay", "");
                PartBeginHour.Text = dic.Get("PartBeginHour", "");
                PartBeginMinute.Text = dic.Get("PartBeginMinute", "");
                PartEndDay.Text = dic.Get("PartEndDay", "");
                PartEndHour.Text = dic.Get("PartEndHour", "");
                PartEndMinute.Text = dic.Get("PartEndMinute", "");
            }
            else
            {
                if (PartPanel.Visible)
                    PartByTime.Checked = false;
            }
        }

        private void PartByNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (PartByNumber.Checked && PartByTime.Checked)
                PartByTime.Checked = false;
            if (PartByNumber.Checked)
            {
                PartBeginNumber.Enabled = true;
                PartBeginNumber.ReadOnly = false;
                PartEndNumber.Enabled = true;
                PartEndNumber.ReadOnly = false;
            }
            else
            {
                PartBeginNumber.Enabled = false;
                PartBeginNumber.ReadOnly = true;
                PartEndNumber.Enabled = false;
                PartEndNumber.ReadOnly = true;
            }
        }

        private void PartByTime_CheckedChanged(object sender, EventArgs e)
        {
            if (PartByTime.Checked && PartNumberPanel.Visible && PartByNumber.Checked)
                PartByNumber.Checked = false;
            if (PartByTime.Checked)
            {
                PartBeginDay.Enabled = true;
                PartBeginDay.ReadOnly = false;
                PartBeginHour.Enabled = true;
                PartBeginHour.ReadOnly = false;
                PartBeginMinute.Enabled = true;
                PartBeginMinute.ReadOnly = false;
                PartEndDay.Enabled = true;
                PartEndDay.ReadOnly = false;
                PartEndHour.Enabled = true;
                PartEndHour.ReadOnly = false;
                PartEndMinute.Enabled = true;
                PartEndMinute.ReadOnly = false;
            }
            else
            {
                PartBeginDay.Enabled = false;
                PartBeginDay.ReadOnly = true;
                PartBeginHour.Enabled = false;
                PartBeginHour.ReadOnly = true;
                PartBeginMinute.Enabled = false;
                PartBeginMinute.ReadOnly = true;
                PartEndDay.Enabled = false;
                PartEndDay.ReadOnly = true;
                PartEndHour.Enabled = false;
                PartEndHour.ReadOnly = true;
                PartEndMinute.Enabled = false;
                PartEndMinute.ReadOnly = true;
            }
        }
    }
}
