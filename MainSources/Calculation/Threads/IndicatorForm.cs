using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace Calculation
{
    public partial class IndicatorForm : Form
    {
        public IndicatorForm()
        {
            InitializeComponent();
        }

        public ThreadApp ThreadApp { get; set; }

        private void IndicatorForm_Load(object sender, EventArgs e)
        {
            Application.EnableVisualStyles();
        }

        private void ButBreak_Click(object sender, EventArgs e)
        {
            if (Different.MessageQuestion("Вы действительно хотите прервать расчет?"))
                ThreadApp.BreakCalc();
        }
    }
}
