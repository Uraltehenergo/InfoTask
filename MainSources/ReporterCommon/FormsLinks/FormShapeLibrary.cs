using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Shape = Microsoft.Office.Interop.Excel.Shape;

namespace ReporterCommon.FormsLinks
{
    public partial class FormShapeLibrary : Form
    {
        public FormShapeLibrary()
        {
            InitializeComponent();
        }

        //Текущая книга
        private ReportBook _book;
        
        private void FormShapeLibrary_Load(object sender, EventArgs e)
        {
            _book = GeneralRep.ActiveBook;
            GeneralRep.Application.CommandBars.OnUpdate += OnShapeChange;
            OnShapeChange();
            LoadShape();

            //var menu = new ContextMenuStrip();
            //var item = menu.Items.Add("Выбрать цвет"); 
            //item.Click += SelectColor;
            //ShapeFormula.ContextMenuStrip = menu;
        }
        
        //Текущая фигура
        private Shape _curShape;

        //Обработка события по изменению текущей фигуры
        private void OnShapeChange()
        {
            SaveShape();
            LoadShape();
        }

        //Загрузка информации из текущей выдененной фигуры
        private void LoadShape()
        {
            var sh = _book.ActiveShape();
            if (sh == null)
            {
                ShapeType.Text = null;
                ShapeId.Text = null;
                ShapeName.Text = null;
                ShapeFormula.Text = null;
                ShapeLink.Text = null;
                ShapeFormula.Enabled = false;
                _curShape = null;
            }
            else if (sh != _curShape)
            {
                ShapeType.Text = sh.Type.ToString();
                ShapeId.Text = sh.ID.ToString();
                ShapeName.Text = sh.Name;
                ShapeFormula.Text = MakeEnters(sh.AlternativeText);
                ShapeLink.Text = sh.Title;
                ShapeFormula.Enabled = sh.Type == MsoShapeType.msoGroup;
                _curShape = sh;
            }
        }

        private string MakeEnters(string s)
        {
            string st = s.Replace("\n", "#").Replace(Environment.NewLine, "#");
            var sb = new StringBuilder();
            bool en = false;
            foreach (var c in st)
            {
                if (c == '#') en = true;
                else
                {
                    if (en) sb.Append(Environment.NewLine);
                    sb.Append(c);
                    en = false;
                }
            }
            return sb.ToString();
        }

        //Сохранение информации в текущую выдененную фигуру
        private void SaveShape()
        {
            if (_curShape != null)
            {
                try {_curShape.Name = ShapeName.Text;} catch {}
                try 
                {
                    if (_curShape.Type == MsoShapeType.msoGroup)
                        _curShape.AlternativeText = ShapeFormula.Text;
                    _book.Workbook.Save();
                    _book.UpdateDataFile(true);
                }
                catch {}
            }
        }

        private void FormShapeLibrary_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveShape();
            GeneralRep.Application.CommandBars.OnUpdate -= OnShapeChange;
            _book.CloseForm(ReporterCommand.ShapeLibrary);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var b = ShapeFormula.SelectedText.ToInt();
            int r = b % 256;
            b = b / 256;
            int g = b % 256;
            b = b / 256;
            ColorDialog.Color = Color.FromArgb(r, g, b); 
            if (ColorDialog.ShowDialog() == DialogResult.Cancel) return;
            var c = ColorDialog.Color;
            ShapeFormula.Paste((c.R + 256*c.G + 256*256*c.B).ToString());
        }

        private void ButSaveShape_Click_1(object sender, EventArgs e)
        {
            SaveShape();
        }

        private void ButLoadShape_Click(object sender, EventArgs e)
        {
            LoadShape();
        }

        private void ButCheck_Click(object sender, EventArgs e)
        {
            var sh = _book.ActiveShape();
            if (sh != null)
            {
                try
                {
                    var rs = new ReportShapeDebug(sh, ShapeFormula.Text);
                    rs.Node.Apply(rs, TestValue.Text.ToInt());
                    if (!rs.ErrMess.IsEmpty())
                        Different.MessageError(rs.ErrMess);
                }
                catch (Exception ex)
                {
                    ex.MessageError("Ошибка синтаксиса или применения формулы");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestValue.Text = (TestValue.Text.ToInt() << 1).ToString();
        }

        private void TestValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void ButSyntax_Click(object sender, EventArgs e)
        {
            bool er = false;
            var sh = _book.ActiveShape();
            if (sh != null)
            {
                try
                {
                    var rs = new ReportShapeDebug(sh, ShapeFormula.Text);
                    if (!rs.ErrMess.IsEmpty())
                    {
                        Different.MessageError(rs.ErrMess);
                        er = true;
                    }
                }
                catch (Exception ex)
                {
                    ex.MessageError("Ошибка синтаксиса или применения формулы");
                    er = true;
                }
            }
            if (!er) Different.MessageInfo("Формула корректна");
        }

        private void GetCursorPosition()
        {
            string t = ShapeFormula.Text;
            var n = ShapeFormula.SelectionStart;
            int p = 0, s = 0;
            for (int i = 0; i < n; i++)
            {
                if (t[i] == '\n')
                {
                    p = 0;
                    s++;
                }
                else p++;
            }
            CursorPosition.Text = "стр." + (s + 1) + " поз." + (p + 1);
        }

        private void ShapeFormula_KeyDown(object sender, KeyEventArgs e)
        {
            GetCursorPosition();
        }

        private void ShapeFormula_KeyUp(object sender, KeyEventArgs e)
        {
            GetCursorPosition();
        }

        private void ShapeFormula_MouseDown(object sender, MouseEventArgs e)
        {
            GetCursorPosition();
        }

        private void ShapeFormula_MouseUp(object sender, MouseEventArgs e)
        {
            GetCursorPosition();
        }

        private void ShapeFormula_Enter(object sender, EventArgs e)
        {
            GetCursorPosition();
        }
    }
}
