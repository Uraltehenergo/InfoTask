using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace ReporterCommon
{
    internal partial class GrammarsForm : Form
    {
        public GrammarsForm()
        {
            InitializeComponent();
        }

        private void ButCondition_Click(object sender, EventArgs e)
        {
            var parsing = new ShapeViewParsing();
            Result.Text = parsing.Parse(null, Formula.Text).ToTestString();
            if (!parsing.Keeper.ErrMess.IsEmpty())
                Result.Text += parsing.Keeper.ErrMess;
        }
    }
}
