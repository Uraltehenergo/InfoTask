using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConsGraphLibrary
{
    public partial class EditPlaneForm : Form
    {
        internal List<TextBox> TBs = new List<TextBox>();
        internal CGraphForm ParentF;
        internal Act Action;
        internal enum Act
        {
            Add,
            Vary
        };

        public EditPlaneForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Button2Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button1Click(object sender, EventArgs e)
        {
            string newSeriesName = "";
            foreach (var textBox in TBs)
            {
                if (textBox.Text == "") Close();
                newSeriesName += textBox.Text + "; ";
            }
            newSeriesName = newSeriesName.Substring(0, newSeriesName.Length - 2);
            if (Action == Act.Add)
                if (ParentF.AddSeries(newSeriesName)) Close();
                else MessageBox.Show("Такая проекция уже существует");
            else
            {
                if (ParentF.VarySeries(newSeriesName)) Close();
                else MessageBox.Show("Такая проекция уже существует");
            }
        }
    }
}
