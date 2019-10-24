using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrafeoLibrary
{
    public partial class GrafeoForm : Form
    {
        private DataGridViewGrafeo _dataGridViewGrafeo;

        public GrafeoForm()
        {
            InitializeComponent();

            _dataGridViewGrafeo = new DataGridViewGrafeo(dataGridView);
        }

        private void labBottomPanel_Click(object sender, EventArgs e)
        {
            splitContainerV.Panel2Collapsed = !splitContainerV.Panel2Collapsed;
            //labBottomPanel_Move(sender, e);
            labBottomPanel.Image = (splitContainerV.Panel2Collapsed ? Properties.Resources.upl : Properties.Resources.downl).ToBitmap();
        }

        private void labRightPanel_Click(object sender, EventArgs e)
        {
            splitContainerH.Panel2Collapsed = !splitContainerH.Panel2Collapsed;
            //labRightPanel_Move(sender, e);
            labRightPanel.Image = (splitContainerH.Panel2Collapsed ? Properties.Resources.leftl : Properties.Resources.rightl).ToBitmap();
        }

        private void labPanel_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.White;
        }

        private void labPanel_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = SystemColors.GradientInactiveCaption; //Color.LightSeaGreen;
        }
    }
}
