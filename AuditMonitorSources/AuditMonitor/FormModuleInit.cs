using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuditMonitor
{
    public partial class FormModuleInit : Form
    {
        private byte _step;
        
        public FormModuleInit()
        {
            InitializeComponent();
            _step = 1;
            cbBaudRate.SelectedIndex = 3;
            cbChecksum.SelectedIndex = 0;
            cbDataFormat.SelectedIndex = 0;
            cbInputRange.SelectedIndex = 16;
            cbIntegrationTime.SelectedIndex = 1;
        }

        private void butNext_Click(object sender, EventArgs e)
        {
            string stOut;
            
            switch(_step)
            {
                case 1:
                    labStepNum.Text = @"Шаг 2.";
                    labStep1.Visible = false;
                    labStep2.Visible = true;
                    frModuleInit.Visible = true;
                    tbAddress10.Select();
                    tbCommand.Visible = true;
                    _step = 2;
                    break;
                case 2:
                    if (CheckModuleParams())
                    {
                        stOut = (Program.Net.Port != 0)
                                    ? Program.Net.SendCommand(GetCommandString())
                                    : @"!" + tbAddress.Text;

                        if (stOut == @"!" + tbAddress.Text)
                        {
                            labStepNum.Text = @"Шаг 3.";
                            labStep2.Visible = false;
                            frModuleInit.Visible = false;
                            labStep3.Visible = true;
                            butNext.Enabled = false;
                            tbCommand.Visible = false;
                            Refresh();
                            _step = 3;

                            for (byte i = 7; i >= 1; i--)
                            {
                                if (i >= 5)
                                    labStep3.Text = @"Подождите " + i + @" секунд";
                                else if (i > 1)
                                    labStep3.Text = @"Подождите " + i + @" секунды";
                                else
                                    labStep3.Text = @"Подождите " + i + @" секунду";
                                DateTime tm = DateTime.Now;
                                while (DateTime.Now.Subtract(tm).TotalSeconds <= 1)
                                {
                                    Application.DoEvents();
                                }
                            }

                            labStepNum.Text = @"Шаг 4.";
                            labStep3.Visible = false;
                            labStep4.Visible = true;
                            butNext.Enabled = true;
                            _step = 4;
                        }
                        else
                        {
                            labStepNum.Visible = false;
                            labStep2.Visible = false;
                            labStepComplited.Text = @"Модуль не инициализирован";
                            labStepComplited.Visible = true;
                            frModuleInit.Visible = false;
                            tbCommand.Visible = false;
                            butNext.Text = @"Выход";
                            _step = 5;
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Неверный адрес модуля");
                        //MessageBox.Show(@"Неверная конфигурация модуля");
                    }
                    break;
                case 4:
                    string stRet = GetReturnString();

                    stOut = (Program.Net.Port != 0)
                                ? Program.Net.SendCommand("$" + tbAddress.Text + 2)
                                : stRet;
                    
                    if ((stOut.Length == 9) && (stOut.StartsWith("!")))
                    {
                        if (stOut != stRet) 
                            labStepComplited.Text = @"Конфигурация модуля не соответствует заданной:" + "\n"
                                                    + @"(" + stOut.Substring(1) + @"   -   " + stRet.Substring(1) + @")";
                    }
                    else labStepComplited.Text = @"Не удалось прочитать конфигурацию модуля";
                    
                    labStepNum.Visible = false;
                    labStep4.Visible = false;
                    labStepComplited.Visible = true;
                    butNext.Text = @"Выход";
                    _step = 5;
                    break;
                case 5:
                    Close();
                    break;
            }
        }

        private void tbAddress10_Validating(object sender, CancelEventArgs e)
        {
            byte address10;
            if(byte.TryParse(tbAddress10.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out address10))
            {
                //tbAddress.Text = ClassAbstractAdamModule.ByteToHex(address10);
                tbAddress.Text = NewModuleAbstract.ByteToHex(address10);
                tbCommand.Text = GetCommandString();
            }
            else
            {
                MessageBox.Show(@"Адрес должен быть целым числом от 0 до 255");
                e.Cancel = true;
                tbAddress10.SelectionStart = 0;
                tbAddress10.SelectionLength = tbAddress10.Text.Length;
            }
        }

        private void tbAddress_Validating(object sender, CancelEventArgs e)
        {
            //byte? address10 = ClassAbstractAdamModule.HexToByte(tbAddress.Text);
            byte? address10 = NewModuleAbstract.HexToByte(tbAddress.Text);
            if(address10 != null)
            {
                tbAddress.Text = (tbAddress.Text.Length == 1) ? '0' + tbAddress.Text.ToUpper() : tbAddress.Text.ToUpper();
                tbAddress10.Text = address10.ToString();
                tbCommand.Text = GetCommandString();
            }
            else
            {
                MessageBox.Show(@"Адрес должен быть 16-ричным числом от 00 до FF");
                tbAddress.SelectionStart = 0;
                tbAddress.SelectionLength = tbAddress.Text.Length;
                e.Cancel = true;
            }
        }

        private bool CheckModuleParams()
        {
            byte address;
            return (byte.TryParse(tbAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address));
        }

        private string GetCommandString()
        {
            var bFf = (byte) (cbChecksum.SelectedIndex*(byte) Math.Pow(2, 6));
            bFf += (byte) ((byte) cbIntegrationTime.SelectedIndex*(byte) Math.Pow(2, 7));
            bFf += (byte) cbDataFormat.SelectedIndex;
            return @"%00" + tbAddress.Text + cbInputRange.Text.Substring(0, 2) +
                   cbBaudRate.Text.Substring(0, 2) + Convert.ToString(bFf, 16);
        }

        private string GetReturnString()
        {
            var bFf = (byte)(cbChecksum.SelectedIndex * (byte)Math.Pow(2, 6));
            bFf += (byte)((byte)cbIntegrationTime.SelectedIndex * (byte)Math.Pow(2, 7));
            bFf += (byte)cbDataFormat.SelectedIndex;
            return @"!" + tbAddress.Text + cbInputRange.Text.Substring(0, 2) +
                   cbBaudRate.Text.Substring(0, 2) + Convert.ToString(bFf, 16);
        }
    }
}
