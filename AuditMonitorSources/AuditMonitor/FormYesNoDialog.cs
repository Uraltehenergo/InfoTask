using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuditMonitor
{
    public partial class FormYesNoDialog : Form
    {
        private FormYesNoDialog()
        {
            InitializeComponent();
        }

        public FormYesNoDialog(string caption, string message) : this()
        {
            Text = caption;
            labMessage.Text = message;
        }

        private void butYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void butNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
