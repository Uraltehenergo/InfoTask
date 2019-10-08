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
    public partial class FormScanNet : Form
    {
        private const string TextScan = "Сканировать сеть";
        private const string TextStopScan = "Завершить сканирование";
        
        private EFlagState _flagBlockForm = EFlagState.No;

        public FormScanNet()
        {
            InitializeComponent();
        
            cbPort.Items.Clear();
            cbPort.Items.Add("");
            foreach (var port in ClassHyperTerminal.GetAvailablePorts()) cbPort.Items.Add(port);

            cbPort.Text = Program.Net.Port.ToString();
            
            tbFirstModuleAddress.Text = Program.Params.FirstScanAddress.ToString();
            tbLastModuleAddress.Text = Program.Params.LastScanAddress.ToString();
        }
        
        private void butScanNet_Click(object sender, EventArgs e)
        {
            if (_flagBlockForm == EFlagState.No)
            {
                if (MessageBox.Show(@"Начать сканирование сети?", @"Сканирование сети", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ClassLog.AddLog(ELogType.Event, null, "Сканирование сети");

                    byte firstAddress;
                    byte lastAddress;
                    if (!byte.TryParse(tbFirstModuleAddress.Text, NumberStyles.Integer,
                                       CultureInfo.InvariantCulture.NumberFormat, out firstAddress))
                        firstAddress = 0;
                    tbFirstModuleAddress.Text = firstAddress.ToString();
                    if (!byte.TryParse(tbLastModuleAddress.Text, NumberStyles.Integer,
                                       CultureInfo.InvariantCulture.NumberFormat, out lastAddress))
                        lastAddress = 255;
                    tbLastModuleAddress.Text = lastAddress.ToString();

                    if (firstAddress > lastAddress)
                    {
                        tbLastModuleAddress.Focus();
                        MessageBox.Show(@"Кон. адрес должен быть меньше чем нач. адрес");
                        return;
                    }

                    _flagBlockForm = EFlagState.Yes;
                    UpdateFormView();
                    Program.FmMonitor.ClearModulesTreeView();
                    Program.FmMonitor.ClearSignalListView();

                    //ClassAdamNet.DelegateScanNet dlScanNet = Program.Net.ScanNet;
                    //IAsyncResult asyncResult = dlScanNet.BeginInvoke(firstAddress, lastAddress, UpdateView, null, null);
                    NewNet.DelegateScanNet dlScanNet = Program.Net.ScanStart;
                    IAsyncResult asyncResult = dlScanNet.BeginInvoke(firstAddress, lastAddress, UpdateView, null, null);
                    while (!asyncResult.IsCompleted)
                    {
                        Application.DoEvents();
                    }

                    tbCurModuleAddress.Text = null;

                    Program.FmMonitor.NetChange();
                    //ADAMScaner.TagListChange
                    //ADAMScaner.UpdateFormView

                    _flagBlockForm = EFlagState.No;
                    UpdateFormView();
                    ClassLog.AddLog(ELogType.Event, null, "Сканирование сети закончено");
                    MessageBox.Show(@"Сканирование сети закончено");
                }
            }
            else
            {
                Program.Net.ScanStop();
            }
        }

        private void butLoadConfigFromObjectsPTK_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Загрузить конфигурацию каналов из TagList?", @"Загрузка конфигурации каналов", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ClassLog.AddLog(ELogType.Event, null, "Загрузка конфигурации каналов");
                Application.DoEvents();
                _flagBlockForm = EFlagState.Yes;
                UpdateFormView();

                //ClassTagListTable.LoadChannelConfiguration(Program.Net);
                NewTagList.LoadChannelConfiguration(Program.Net);
                Program.Net.ProgramChannels();
                Program.FmMonitor.ModulesToListView();

                ClassLog.AddLog(ELogType.Event, null, "Загрузка конфигурации каналов завершена");
                _flagBlockForm = EFlagState.No;
                UpdateFormView();
            }
        }

        private void tbFirstModuleAddress_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbFirstModuleAddress.Text))
            {
                byte firstAddress;
                if (byte.TryParse(tbFirstModuleAddress.Text, NumberStyles.Integer,
                                    CultureInfo.InvariantCulture.NumberFormat, out firstAddress))
                    Program.Params.FirstScanAddress = firstAddress;
                else
                {
                    MessageBox.Show(@"Нач. адрес должен быть числом от 0 до 255");
                    tbFirstModuleAddress.Text = null;
                    e.Cancel = true;
                }
            }
            else
                Program.Params.FirstScanAddress = null;

        }

        private void tbLastModuleAddress_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbLastModuleAddress.Text))
            {
                byte lastAddress;
                if (byte.TryParse(tbLastModuleAddress.Text, NumberStyles.Integer,
                                   CultureInfo.InvariantCulture.NumberFormat, out lastAddress))
                    Program.Params.LastScanAddress = lastAddress;
                else
                {
                    MessageBox.Show(@"Нач. адрес должен быть числом от 0 до 255");
                    tbLastModuleAddress.Text = null;
                    e.Cancel = true;
                }
            }
            else
                Program.Params.LastScanAddress = null;
        }

        //private void ScanNet(byte beginAddress, byte endAddress)
        //{
        //    _fmMonitor.Net.ScanNet(beginAddress, endAddress, UpdateView);
        //}

        //private delegate void DelegateScanNet(byte beginAddress, byte endAddress);

        private delegate void DelegateUpdateView(string text);

        private void UpdateView(string text)
        {
            if (tbCurModuleAddress.InvokeRequired == false)
            {
                tbCurModuleAddress.Text = text;
            }
            else
            {
                var dlgUpdateView = new DelegateUpdateView(UpdateView);
                BeginInvoke(dlgUpdateView, new object[] { text });
            }
        }

        private void UpdateFormView()
        {
            if ((_flagBlockForm == EFlagState.Yes) || (_flagBlockForm == EFlagState.Stop))
            {
                butLoadConfigFromObjectsPTK.Enabled = false;
                cbPort.Enabled = false;
                tbFirstModuleAddress.Enabled = false;
                tbLastModuleAddress.Enabled = false;
                butScanNet.Text = TextStopScan;

                ControlBox = false;
            }
            else if ((_flagBlockForm == EFlagState.No) || (_flagBlockForm == EFlagState.Inhibit))
            {
                butLoadConfigFromObjectsPTK.Enabled = true;
                cbPort.Enabled = true;
                butScanNet.Enabled = true;
                tbFirstModuleAddress.Enabled = true;
                tbLastModuleAddress.Enabled = true;
                butScanNet.Text = TextScan;

                ControlBox = true;
            }
        }

        private void cbPort_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbPort.Text != (Program.Net.Port != 0 ? Program.Net.Port.ToString() : ""))
            {
                var port = cbPort.Text != "" ? byte.Parse(cbPort.Text) : (byte) 0;
                Program.Net.OpenPort(port);
                Program.FmMonitor.CurrentPortChange(port);
            }
        }
    }
}