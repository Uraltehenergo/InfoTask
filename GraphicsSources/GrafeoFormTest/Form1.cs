using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GrafeoLibrary;

namespace GrafeoFormTest
{
    public partial class Form1 : Form
    {
        private Grafeo _grafeo;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void butShow_Click(object sender, EventArgs e)
        {
            if (_grafeo == null) _grafeo = new Grafeo();
            _grafeo.Show();
        }

        private void butShowTmp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sorry!");
            //var formTmp = new FormTmp();
            //formTmp.Show();
        }

        private void butShowGrafeoForm_Click(object sender, EventArgs e)
        {
            var formGrafeo = new GrafeoForm();
            formGrafeo.Show();
        }
    }
}
