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
    public partial class FormModule4019P : Form
    {
        private FormModule4019P()
        {
            InitializeComponent();
            InitControls();
        }

        private void FormModule4019P_FormClosing(object sender, FormClosingEventArgs e)
        {
            cbModuleAddress.Focus();
        }

        private void InitControls()
        {
            cbModuleAddress.Items.Clear();
            //--foreach (ClassAbstractAdamModule module in Program.Net.Modules) cbModuleAddress.Items.Add(module.Address);
            foreach (NewModuleAdamAbstract module in Program.Net.Modules) cbModuleAddress.Items.Add(module.Address);

            string[] dataFormats = { "00", "01", "02", "80", "81", "82" };
            string[] inputRanges = {
                                       "02", "03", "04", "05", "07", "08", "09", "0D", "0E", "0F",
                                       "10", "11", "12", "13", "14"
                                   };
            string[] inLevels = { "", "±100 mV", "4-20 mA", "ТХК L" };
            string[] conversions = { "", "Расход()", "Давление(;)" };

            for (int i = 0; i <= 7; i++)
            {
                ((ComboBox)gbChannels.Controls["cbDataFormat" + i]).Items.Clear();
                ((ComboBox)gbChannels.Controls["cbDataFormat" + i]).Items.AddRange(dataFormats);
                ((ComboBox)gbChannels.Controls["cbChannelRange" + i]).Items.Clear();
                ((ComboBox)gbChannels.Controls["cbChannelRange" + i]).Items.AddRange(inputRanges);
                ((ComboBox)gbChannels.Controls["cbInlevel" + i]).Items.Clear();
                ((ComboBox)gbChannels.Controls["cbInlevel" + i]).Items.AddRange(inLevels);
                ((ComboBox)gbChannels.Controls["cbConversion" + i]).Items.Clear();
                ((ComboBox)gbChannels.Controls["cbConversion" + i]).Items.AddRange(conversions);
            }

            ((ComboBox)gbChannels.Controls["cbDataFormatCJC"]).Items.Clear();
            ((ComboBox)gbChannels.Controls["cbDataFormatCJC"]).Items.AddRange(dataFormats);
            ((ComboBox)gbChannels.Controls["cbChannelRangeCJC"]).Items.Clear();
            ((ComboBox)gbChannels.Controls["cbChannelRangeCJC"]).Items.AddRange(new[] { "CJC" });
            ((ComboBox)gbChannels.Controls["cbInlevelCJC"]).Items.Clear();
            ((ComboBox)gbChannels.Controls["cbConversionCJC"]).Items.Clear();
        }

        public FormModule4019P(string moduleAddress)
            : this()
        {
            var module = Program.Net.Modules[moduleAddress];
            if (module != null) SelectModule(module.Address); else Close();
        }

        public void SelectModule(string moduleAddress)
        {
            var module = (NewModuleAdam4019Plus)Program.Net.Modules[moduleAddress];
            Text = @"Модуль " + moduleAddress + @": " + module.ModuleType;
            cbModuleAddress.Text = moduleAddress;
            tbModuleNum.Text = module.Address10.ToString();
            tbModuleName.Text = module.Name;
            tbModuleFirmwareVersion.Text = module.FirmwareVersion;

            for (byte i = 0; i <= 7; i++)
            {
                gbChannels.Controls["cbDataFormat" + i].Text = module.DataFormat;
                gbChannels.Controls["cbChannelRange" + i].Text = ((NewChannelAdam4019Plus)module.Channels[i]).ChannelRange;
                gbChannels.Controls["tbCode" + i].Text = module.Channels[i].Code;
                gbChannels.Controls["tbName" + i].Text = module.Channels[i].Name;
                gbChannels.Controls["cbInlevel" + i].Text = module.Channels[i].InLevel;
                gbChannels.Controls["tbMin" + i].Text = module.Channels[i].Min.ToString();
                gbChannels.Controls["tbMax" + i].Text = module.Channels[i].Max.ToString();
                gbChannels.Controls["tbAperture" + i].Text = module.Channels[i].Aperture.ToString();
                gbChannels.Controls["cbConversion" + i].Text = module.Channels[i].Conversion;
                gbChannels.Controls["tbUnits" + i].Text = module.Channels[i].Units;
            }

            gbChannels.Controls["cbDataFormatCJC"].Text = module.Cjc.DataFormat;
            gbChannels.Controls["cbChannelRangeCJC"].Text = module.Cjc.ChannelRange;
            gbChannels.Controls["tbCodeCJC"].Text = module.Cjc.Code;
            gbChannels.Controls["tbNameCJC"].Text = module.Cjc.Name;
            gbChannels.Controls["cbInlevelCJC"].Text = module.Cjc.InLevel;
            gbChannels.Controls["tbMinCJC"].Text = module.Cjc.Min.ToString();
            gbChannels.Controls["tbMaxCJC"].Text = module.Cjc.Max.ToString();
            gbChannels.Controls["tbApertureCJC"].Text = module.Cjc.Aperture.ToString();
            gbChannels.Controls["cbConversionCJC"].Text = module.Cjc.Conversion;
            gbChannels.Controls["tbUnitsCJC"].Text = module.Cjc.Units;
        }

        private void ValueChanged(object sender, CancelEventArgs e)
        {
            var module = (NewModuleAdam4019Plus)Program.Net.Modules[cbModuleAddress.Text];
            string name = ((Control)sender).Name;
            //--ClassAbstractAdamChannel channel;
            NewChannelAdamAbstract channel;
            string paramName;
            if (!name.EndsWith("CJC"))
            {
                byte chn = byte.Parse(name.Substring(name.Length - 1));
                channel = (NewChannelAdamAbstract)module.Channels[chn];
                paramName = name.Substring(2, name.Length - 3);
            }
            else
            {
                channel = module.Cjc;
                paramName = name.Substring(2, name.Length - 5);
            }

            //bool flag = true;
            double val;
            switch (paramName.ToLower())
            {
                case "code":
                    channel.Code = ((TextBox)sender).Text;
                    if (((TextBox)sender).Text != (channel.Code ?? ""))
                    {
                        ((TextBox)sender).Text = channel.Code;
                        //flag = false;
                    }
                    break;
                case "name":
                    channel.Name = ((TextBox)sender).Text;
                    if (((TextBox)sender).Text != (channel.Name ?? ""))
                    {
                        ((TextBox)sender).Text = channel.Name;
                        //flag = false;
                    }
                    break;
                case "aperture":
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(".", ",");
                    if (double.TryParse(((TextBox)sender).Text, out val))
                    {
                        channel.Aperture = val;
                        if (((TextBox)sender).Text != channel.Aperture.ToString())
                        {
                            ((TextBox)sender).Text = channel.Aperture.ToString();
                            //flag = false;
                        }
                    }
                    else
                    {
                        ((TextBox)sender).Text = channel.Aperture.ToString();
                        //flag = false;
                    }
                    break;
                case "min":
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(".", ",");
                    if (double.TryParse(((TextBox)sender).Text, out val))
                    {
                        channel.Min = val;
                        if (((TextBox)sender).Text != channel.Min.ToString())
                        {
                            ((TextBox)sender).Text = channel.Min.ToString();
                            //flag = false;
                        }
                    }
                    else
                    {
                        ((TextBox)sender).Text = channel.Min.ToString();
                        //flag = false;
                    }
                    break;
                case "max":
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(".", ",");
                    if (double.TryParse(((TextBox)sender).Text, out val))
                    {
                        channel.Max = val;
                        if (((TextBox)sender).Text != channel.Max.ToString())
                        {
                            ((TextBox)sender).Text = channel.Max.ToString();
                            //flag = false;
                        }
                    }
                    else
                    {
                        ((TextBox)sender).Text = channel.Max.ToString();
                        //flag = false;
                    }
                    break;
                case "dataformat":
                    ((ComboBox)sender).Text = channel.DataFormat;
                    //flag = false;
                    break;
                //case "channelrange":
                //    ((ComboBox) sender).Text = channel.ChannelRange;
                //    flag = false;
                //    break;
                case "inlevel":
                    channel.InLevel = ((ComboBox)sender).Text;
                    if (((ComboBox)sender).Text != (channel.InLevel ?? ""))
                    {
                        ((ComboBox)sender).Text = channel.InLevel;
                        //flag = false;
                    }
                    break;
                case "conversion":
                    channel.Conversion = ((ComboBox)sender).Text;
                    if (((ComboBox)sender).Text != (channel.Conversion ?? ""))
                    {
                        ((ComboBox)sender).Text = channel.Conversion;
                        //flag = false;
                    }
                    break;
                case "units":
                    channel.Units = ((TextBox)sender).Text;
                    if (((TextBox)sender).Text != (channel.Units ?? ""))
                    {
                        ((TextBox)sender).Text = channel.Units;
                        //flag = false;
                    }
                    break;
                //default:
                //flag = false;
                //break;
            }

            /*if (flag)*/
            Program.FmMonitor.UpdateModulesParamInListView(cbModuleAddress.Text, channel.Channel, paramName);
        }

        private void ChannelRangeChanged(object sender, CancelEventArgs e)
        {
            string name = ((Control)sender).Name;
            byte channel = byte.Parse(name.Substring(name.Length - 1));

            /* Если не идет опрос */

            // "$AA7CiRrr"
            string stOut = "$" + cbModuleAddress.Text + "7C" + channel + "R" + ((ComboBox)sender).Text;
            string stIn = Program.Net.SendCommand(stOut);

            var module = (NewModuleAdam4019Plus)Program.Net.Modules[cbModuleAddress.Text];

            if (stIn == "!" + cbModuleAddress.Text)
            {
                ((NewChannelAdamAbstract)module.Channels[channel]).ChannelRange = ((ComboBox)sender).Text;
                Program.FmMonitor.UpdateModulesParamInListView(cbModuleAddress.Text, channel, "ChannelRange");
            }
            else
            {
                ((ComboBox)sender).Text = ((NewChannelAdamAbstract)module.Channels[channel]).ChannelRange;
            }
        }

        private void cbModuleAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectModule(cbModuleAddress.Text);
        }

        //private void butNext_Click(object sender, EventArgs e)
        //{
        //    byte addr = byte.Parse(tbModuleNum.Text);
        //    addr++;
        //    for (; addr <= 255; addr++)
        //        if ((Program.Net.Modules[ClassAbstractAdamModule.ByteToHex(addr)] != null) || (addr == byte.Parse(tbModuleNum.Text)))
        //            break;

        //    if (addr != byte.Parse(tbModuleNum.Text)) SelectModule(ClassAbstractAdamModule.ByteToHex(addr));
        //}

        //private void butPrevious_Click(object sender, EventArgs e)
        //{
        //    byte addr = byte.Parse(tbModuleNum.Text);
        //    addr--;
        //    for (; addr >= 0; addr--)
        //        if ((Program.Net.Modules[ClassAbstractAdamModule.ByteToHex(addr)] != null) || (addr == byte.Parse(tbModuleNum.Text)))
        //            break;

        //    if (addr != byte.Parse(tbModuleNum.Text)) SelectModule(ClassAbstractAdamModule.ByteToHex(addr));
        //}
    }
}