using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuditMonitor
{
    public partial class FormSetup : Form
    {
        public static StringBuilder[] GetFonts()
        {
            var ifc = new InstalledFontCollection();
            var fontFamily = ifc.Families;
            var fonts = new StringBuilder[ifc.Families.Count()];
            Array fsa = Enum.GetValues(typeof (FontStyle));

            long j = fontFamily.GetLowerBound(0) - 1;
            foreach (FontFamily ff in fontFamily)
            {
                j++;
                fonts[j] = new StringBuilder();
                fonts[j].Clear();

                for (int i = 0; i < fsa.Length; i++)
                {
                    if (ff.IsStyleAvailable((FontStyle) fsa.GetValue(i)))
                    {
                        if (fonts[j].Length != 0) fonts[j].Append(", ");
                        fonts[j].Append(fsa.GetValue(i));
                    }
                    fonts[j].Insert(0, ff.Name + " (");
                    fonts[j].Append(")");
                }
            }
            return fonts;
        }

        private bool _changeFontSize = true;

        public FormSetup()
        {
            InitializeComponent();
        
            tbReadPeriod.Text = Program.Net.ReadPeriodSec.ToString();
            tbModuleTimeOut.Text = Program.Net.ModuleTimeOut.ToString();
            
            //chbArchiveWriteMode.Checked = Program.Net.WriteArchiveByRead;
            rbWriteArchive_ByAperture.Checked = ! Program.Net.WriteArchiveByRead;
            rbWriteArchive_ByRead.Checked = Program.Net.WriteArchiveByRead;
            
            //chbSaveNetOnClose.Checked = Program.Params.SaveNetOnClose;
            rbSaveNetOnClose_No.Checked = (Program.Params.SaveNetOnClose == false);
            rbSaveNetOnClose_Yes.Checked = (Program.Params.SaveNetOnClose == true);
            rbSaveNetOnClose_OnRequest.Checked = (Program.Params.SaveNetOnClose == null);
            
            cbFontName.Items.Clear();
            foreach (FontFamily font in (new InstalledFontCollection()).Families)
                if ((font.IsStyleAvailable(FontStyle.Regular)) || (font.IsStyleAvailable(FontStyle.Bold)) ||
                    (font.IsStyleAvailable(FontStyle.Italic)))
                    cbFontName.Items.Add(font.Name);

            cbFontSize.Items.Clear();
            for (int i = 8; i <= 12; i++) cbFontSize.Items.Add(i);
            for (int i = 14; i <= 28; i = i + 2) cbFontSize.Items.Add(i);
            cbFontSize.Items.Add(36);
            cbFontSize.Items.Add(48);
            cbFontSize.Items.Add(72);

            SetFontParams();
        }

        private void SetFontParams()
        {
            _changeFontSize = false;
            cbFontName.Text = Program.FmMonitor.LvSignalList.Font.Name;
            cbFontSize.Text = Program.FmMonitor.LvSignalList.Font.Size.ToString();
            chbBold.Checked = Program.FmMonitor.LvSignalList.Font.Bold;
            chbItalic.Checked = Program.FmMonitor.LvSignalList.Font.Italic;
            _changeFontSize = true;
        }

        private void butSendCommand_Click(object sender, EventArgs e)
        {
            var fmCommand = new FormCommand();
            fmCommand.ShowDialog();
        }

        private void tbReadPeriod_Validating(object sender, CancelEventArgs e)
        {
            double val;
            if (double.TryParse(tbReadPeriod.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                Program.Net.ReadPeriodSec = val;
            tbReadPeriod.Text = Program.Net.ReadPeriodSec.ToString();
        }

        private void tbModuleTimeOut_Validating(object sender, CancelEventArgs e)
        {
            int val;
            if (int.TryParse(tbModuleTimeOut.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                Program.Net.ModuleTimeOut = val;
            else
                tbReadPeriod.Text = Program.Net.ModuleTimeOut.ToString();
        }

        private void butModulesConfig_Click(object sender, EventArgs e)
        {
            var fmModuleInit = new FormModuleInit();
            fmModuleInit.ShowDialog();
        }

        private void FormSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            butSendCommand.Select();
        }

        private void cbFontName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FontStyle fontStyle;
            if (chbBold.Checked && chbItalic.Checked) fontStyle = FontStyle.Bold | FontStyle.Italic;
            else if (chbBold.Checked) fontStyle = FontStyle.Bold;
            else if (chbItalic.Checked) fontStyle = FontStyle.Italic;
            else fontStyle = FontStyle.Regular;

            var fontFamily = new FontFamily(cbFontName.Text);
            if (!fontFamily.IsStyleAvailable(fontStyle))
            {
                if (fontFamily.IsStyleAvailable(FontStyle.Regular))
                {
                    if ((chbBold.Checked) && (!fontFamily.IsStyleAvailable(FontStyle.Bold)))
                        chbBold.Checked = false;
                    if ((chbItalic.Checked) && (!fontFamily.IsStyleAvailable(FontStyle.Italic)))
                        chbItalic.Checked = false;
                }
                else
                {
                    bool flag = false;

                    if ((chbBold.Checked) && (!fontFamily.IsStyleAvailable(FontStyle.Bold)))
                        chbBold.Checked = false;
                    else if ((!chbBold.Checked) && (fontFamily.IsStyleAvailable(FontStyle.Bold)))
                    {
                        chbBold.Checked = true;
                        flag = true;
                    }


                    if ((chbItalic.Checked) && (!fontFamily.IsStyleAvailable(FontStyle.Italic)))
                        chbItalic.Checked = false;
                    else if ((!chbItalic.Checked) && (fontFamily.IsStyleAvailable(FontStyle.Italic)) && (!flag))
                        chbItalic.Checked = true;
                }
            }

            float fontSize;
            if (float.TryParse(cbFontSize.Text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture,
                               out fontSize))
                if (!Program.FmMonitor.SetlvSignalListFontParams(cbFontName.Text, fontSize, chbBold.Checked,
                                                                 chbItalic.Checked))
                    SetFontParams();
        }

        private void chbBold_Click(object sender, EventArgs e)
        {
            var fontFamily = new FontFamily(cbFontName.Text);
            FontStyle fontStyle;
            if (chbBold.Checked && chbItalic.Checked) fontStyle = FontStyle.Bold | FontStyle.Italic;
            else if (chbBold.Checked) fontStyle = FontStyle.Bold;
            else if (chbItalic.Checked) fontStyle = FontStyle.Italic;
            else fontStyle = FontStyle.Regular;

            if (!fontFamily.IsStyleAvailable(fontStyle)) chbBold.Checked = !chbBold.Checked;
            else
            {
                float fontSize;
                if (float.TryParse(cbFontSize.Text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture,
                                   out fontSize))
                    if (
                        !Program.FmMonitor.SetlvSignalListFontParams(cbFontName.Text, fontSize, chbBold.Checked,
                                                                     chbItalic.Checked))
                        SetFontParams();
            }
        }

        private void chbItalic_Click(object sender, EventArgs e)
        {
            var fontFamily = new FontFamily(cbFontName.Text);
            FontStyle fontStyle;
            if (chbBold.Checked && chbItalic.Checked) fontStyle = FontStyle.Bold | FontStyle.Italic;
            else if (chbBold.Checked) fontStyle = FontStyle.Bold;
            else if (chbItalic.Checked) fontStyle = FontStyle.Italic;
            else fontStyle = FontStyle.Regular;

            if (!fontFamily.IsStyleAvailable(fontStyle)) chbItalic.Checked = !chbItalic.Checked;
            else
            {
                float fontSize;
                if (float.TryParse(cbFontSize.Text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture,
                                   out fontSize))
                    if (
                        !Program.FmMonitor.SetlvSignalListFontParams(cbFontName.Text, fontSize, chbBold.Checked,
                                                                     chbItalic.Checked))
                        SetFontParams();
            }
        }

        private void cbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_changeFontSize)
            {
                float fontSize;
                if (float.TryParse(cbFontSize.Text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture,
                                   out fontSize))
                    if (
                        !Program.FmMonitor.SetlvSignalListFontParams(cbFontName.Text, fontSize, chbBold.Checked,
                                                                     chbItalic.Checked))
                        SetFontParams();
            }
        }

        private void butDefault_Click(object sender, EventArgs e)
        {
            Program.FmMonitor.SetlvSignalListFontParams("Microsoft Sans Sherif", 8);
            SetFontParams();
        }

        private void cbFontSize_Leave(object sender, EventArgs e)
        {
            cbFontSize_SelectedIndexChanged(sender, e);
        }

        private void butInitVirtualNet_Click(object sender, EventArgs e)
        {
            var fmVertualNet = new FormVirtualNet();
            fmVertualNet.ShowDialog();
        }

        private void butProgramModules_Click(object sender, EventArgs e)
        {
            for (byte i = 0; i<= 255; i++)
            {
                //--string addr = ClassAbstractAdamModule.ByteToHex(i);
                string addr = NewModuleAbstract.ByteToHex(i);
                if (Program.Net.Modules[addr] != null)
                {
                    var fmModule = new FormModule4019P(addr);
                    fmModule.ShowDialog();
                    break;
                }
                if (i == 255)
                {
                    MessageBox.Show(@"Модули не найдены" + "\n" +
                                    @"Выполните сканирование сети и повторите попытку");
                    break;
                }
            }
        }

        private void ArchiveWriteMode_Changed(object sender, EventArgs e)
        {
            Program.Net.WriteArchiveByRead = rbWriteArchive_ByRead.Checked;
        }

        private void SaveNetOnClose_Changed(object sender, EventArgs e)
        {
            Program.Params.SaveNetOnClose = rbSaveNetOnClose_OnRequest.Checked
                                                ? (bool?) null
                                                : rbSaveNetOnClose_Yes.Checked;
        }

        private void butSaveNet_Click(object sender, EventArgs e)
        {
            //ClassTagListTable.SaveNetToTagList(Program.Net);
            NewTagList.SaveNetToTagList(Program.Net);
            MessageBox.Show(@"Конфигурация сети сохранена");
        }
    }
}
