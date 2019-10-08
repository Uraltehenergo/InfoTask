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
    public partial class FormCommand : Form
    {
        public FormCommand()
        {
            InitializeComponent();

            cbOut.Items.Add("%AANNTTCCFF");   // !AA *
            cbOut.Items.Add("$AA2");          // !AATTCCFF
            cbOut.Items.Add("$AAF");          // !AA(Version)
            cbOut.Items.Add("$AAM");          // !AA(Module Name)
            cbOut.Items.Add("#AA");           // >(data)
            cbOut.Items.Add("#AAN");          // >(data)
            cbOut.Items.Add("$AA5VV");        // !AA *
            cbOut.Items.Add("$AA6");          // !AAVV
            cbOut.Items.Add("$AAB");          // !AANN
            cbOut.Items.Add("$AA3");          // >data
            cbOut.Items.Add("$AA9SNNNN");     // !AA
            cbOut.Items.Add("$AA0Ci");        // !AA
            cbOut.Items.Add("$AA1Ci");        // !AA
            cbOut.Items.Add("$AA7CiRrr");     // !AA *
            cbOut.Items.Add("$AA8Ci");        // !AACiRrr
            cbOut.Items.Add("$AAXnnnn");      // !AA * 
            cbOut.Items.Add("$AAY");          // !AAnnnn
        }

        private void cbOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbOut.SelectedIndex)
            {
                case -1:
                    labCommandComment.Text = @"Команда не выбрана";
                    break;
                case 0:
                    labCommandComment.Text = @"Задание конфигурации модуля ";
                    break;
                case 1:
                    labCommandComment.Text = @"Чтение конфигурации модуля";
                    break;
                case 2:
                    labCommandComment.Text = @"Чтение версии программного обеспечения";
                    break;
                case 3:
                    labCommandComment.Text = @"Чтение имени модуля";
                    break;
                case 4:
                    labCommandComment.Text = @"Чтение входных данных всех каналов";
                    break;
                case 5:
                    labCommandComment.Text = @"Чтение входных данных канала N";
                    break;
                case 6:
                    labCommandComment.Text = @"Включение/выключение блокировок каналов";
                    break;
                case 7:
                    labCommandComment.Text = @"Чтение статуса канала";
                    break;
                case 8:
                    labCommandComment.Text = @"Тестирование обрыва цепи термопары";
                    break;
                case 9:
                    labCommandComment.Text = @"Чтение температуры холодного спая";
                    break;
                case 10:
                    labCommandComment.Text = @"Калибровка температуры холодного спая";
                    break;
                case 11:
                    labCommandComment.Text = @"Калибровка диапазона канала";
                    break;
                case 12:
                    labCommandComment.Text = @"Калибровка нуля канала";
                    break;
                case 13:
                    labCommandComment.Text = @"Установка входного диапазона канала";
                    break;
                case 14:
                    labCommandComment.Text = @"Чтение входного диапазона канала";
                    break;
                case 15:
                    labCommandComment.Text = @"Установка периода цикла связи";
                    break;
                case 16:
                    labCommandComment.Text = @"Чтение периода цикла связи";
                    break;
            }

        }

        private void butSendCommand_Click(object sender, EventArgs e)
        {
            string stOut = cbOut.Text;
            if ((stOut.Substring(1, 2) == "AA") && (tbAA.Text != null))
                stOut = stOut.Substring(0, 1) + tbAA.Text + stOut.Substring(3);
            
            tbIn.Text = Program.Net.SendCommand(stOut);
            if (tbIn.Text != "")
            {
                string aa = tbIn.Text.Substring(1, 2);

                if (tbIn.Text.StartsWith("!" + aa))
                {
                    // "%AANNTTCCFF"
                    if ((cbOut.Text.StartsWith("%")) && (cbOut.Text.Length >= 11))
                    {
                        string nn = cbOut.Text.Substring(3, 2);
                        string tt = cbOut.Text.Substring(5, 2);
                        string cc = cbOut.Text.Substring(7, 2);
                        string ff = cbOut.Text.Substring(9, 2);

                        if (nn != aa) Program.Net.Modules[aa].Address = nn;

                        ((NewModuleAdam4019Plus)Program.Net.Modules[aa]).InputRange = tt;
                        ((NewModuleAdam4019Plus)Program.Net.Modules[aa]).BaudRate = cc;

                        ((NewModuleAdam4019Plus)Program.Net.Modules[aa]).DataFormat = ff;
                        //--foreach (ClassAbstractAdamChannel channel in Program.Net.Modules[aa].Channel)
                        foreach (NewChannelAdamAbstract channel in Program.Net.Modules[aa].Channels)
                        {
                            Program.FmMonitor.UpdateModulesParamInListView(aa, channel.Channel, "DataFormat");
                        }
                    }

                        // $AA5VV

                        // $AA7CiRrr
                    else if ((cbOut.Text.StartsWith("$" + aa + "7C")) && (cbOut.Text.Length >= 9))
                    {
                        byte i = byte.Parse(cbOut.Text.Substring(5, 1));
                        string rr = cbOut.Text.Substring(7, 2);

                        ((NewChannelAdam4019Plus)Program.Net.Modules[aa].Channels[i]).ChannelRange = rr;
                        Program.FmMonitor.UpdateModulesParamInListView(aa, i, "ChannelRange");
                    }

                    // $AAXnnnn
                }

                /*il faut
                Отлавливать команды, меняющие конфигурацию модулей
                */

                //if (lbLog.Items.Count == maxItemsCount)
                //    lbLog.Items.RemoveAt(maxItemsCount - 1);

                //lbCommandLog.Items.Insert(0, DateTime.Now + "   " + cbOut.Text.PadRight(11) + "   " + tbIn.Text);
                lbCommandLog.Items.Insert(0, DateTime.Now + "   " + stOut.PadRight(11) + "   " + tbIn.Text);
            }
            else
                //lbCommandLog.Items.Insert(0, DateTime.Now + "   " + cbOut.Text.PadRight(11) + "   " + "Недопустимая команда или Модуль не отвечает.");
                lbCommandLog.Items.Insert(0, DateTime.Now + "   " + stOut.PadRight(11) + "   " + "Недопустимая команда или Модуль не отвечает.");
            
            lbCommandLog.SetSelected(0, true);
            lbCommandLog.SetSelected(0, false);
        }
    }
}