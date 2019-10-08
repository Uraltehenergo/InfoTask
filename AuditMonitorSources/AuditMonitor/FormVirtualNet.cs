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
    public partial class FormVirtualNet : Form
    {
        public FormVirtualNet()
        {
            InitializeComponent();

            ScanVirtualNet();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            ScanVirtualNet();
        }

        private void ScanVirtualNet()
        {
            for (byte i = 0; i <= 255; i++)
            {
                //--string addr = ClassAbstractAdamModule.ByteToHex(i);
                string addr = NewModuleAbstract.ByteToHex(i);
                string stIn = Program.Net.VirtSendCommand("$" + addr + "M");

                GroupBox groupBox;
                if ((i >= 0) && (i <= 31)) groupBox = gb001F;
                else if ((i >= 32) && (i <= 63)) groupBox = gb203F;
                else if ((i >= 64) && (i <= 95)) groupBox = gb405F;
                else if ((i >= 96) && (i <= 127)) groupBox = gb607F;
                else if ((i >= 128) && (i <= 159)) groupBox = gb809F;
                else if ((i >= 160) && (i <= 191)) groupBox = gbA0BF;
                else if ((i >= 182) && (i <= 223)) groupBox = gbC0DF;
                else groupBox = gbE0FF;

                ((ComboBox)groupBox.Controls["cb" + addr]).SelectedIndex = (stIn == "!" + addr + "4019P") ? 1 : 0;

                if (i == 255) break;
            }
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            string curGBName = ((Control) sender).Name.Substring(2);
            string[] gbNames = {"001F", "203F", "405F", "607F",  "809F", "A0BF",  "C0DF", "E0FF"};
            foreach (string gbName in gbNames) Controls["gb" + gbName].Visible = (gbName == curGBName);
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            string virtModules = "";
            
            for (byte i = 0; i <= 255; i++)
            {
                //--string addr = ClassAbstractAdamModule.ByteToHex(i);
                string addr = NewModuleAbstract.ByteToHex(i);

                GroupBox groupBox;
                if ((i >= 0) && (i <= 31)) groupBox = gb001F;
                else if ((i >= 32) && (i <= 63)) groupBox = gb203F;
                else if ((i >= 64) && (i <= 95)) groupBox = gb405F;
                else if ((i >= 96) && (i <= 127)) groupBox = gb607F;
                else if ((i >= 128) && (i <= 159)) groupBox = gb809F;
                else if ((i >= 160) && (i <= 191)) groupBox = gbA0BF;
                else if ((i >= 182) && (i <= 223)) groupBox = gbC0DF;
                else groupBox = gbE0FF;

                if (((groupBox).Controls["cb" + addr]).Text == @"Adam 4019+") virtModules += "4019+/";
                else virtModules += "/";

                if (i == 255) break;
            }
            
            Program.Net.InitVirtNet(virtModules);
            
            MessageBox.Show(@"Виртуальная сеть обновлена");
            //Close();
        }
    }
}
