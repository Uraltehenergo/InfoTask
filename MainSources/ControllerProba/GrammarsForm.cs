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
using Tablik.Generator;

namespace ControllerProba
{
    internal partial class GrammarsForm : Form
    {
        public GrammarsForm()
        {
            InitializeComponent();
        }

        private void ButCondition_Click(object sender, EventArgs e)
        {
            var parsing = new ParsingCondition();
            Result.Text = parsing.Parse(Formula.Text).ToTestString();
        }

        private void ButGenerator_Click(object sender, EventArgs e)
        {
            var parsing = new ParsingGenerator();
            Result.Text = parsing.Parse(Formula.Text).ToTestString();
        }
    }
}
