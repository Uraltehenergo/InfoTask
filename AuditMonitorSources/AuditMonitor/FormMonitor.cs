using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;

namespace AuditMonitor
{
    public partial class FormMonitor : Form
    {
        private const string TextStart = "Циклический";
        private const string TextStop = "СТОП";

        private FormModule4019P _formModule;
        private EFlagState _flagRead = EFlagState.No;
        private ListViewItem _activeItem;
        
        public FormMonitor()
        {
            InitializeComponent();

            ClassLog.EventNewLog += AddNewLog;
            ClassLog.AddLog(ELogType.Event, null, "AuditMonitor запущен");
            NewNet.EventNetRead += UpdateModulesValuesInListView;
            NewNet.EventNetRead += UpdateLastReadTime;
            NewNet.EventNetReadStop += NetReadStop;
            NewNet.EventNewModuleFind += AddModuleToTreeView;
            NewNet.EventNewModuleFind += AddModuleToListView;
            NewNet.EventArchiveUpdated += UpdateArchiveSize;
            NewNet.EventConnectArchiveError += ConnectArchiveError;
            NewNet.EventWriteArchiveState += WriteArchiveError;
            
            var ttErrStatus = new ToolTip {Active = true, AutomaticDelay = 0};
            ttErrStatus.SetToolTip(lvSignalList, null);
        }

        private void FormMonitor_Load(object sender, EventArgs e)
        {
            Program.Params.Read();
            
            FileArchiveChange(ArchiveFileName, false);
            CurrentPortChange(Program.Net.Port);

            if (MessageBox.Show(@"Загрузить структуру сети из таблицы TagList?" + "\n" +
                @"(При загрузке структуры сети будет выполнено" + "\n" + @"перепрограммирование модулей)", @"Audit Monitor", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                NewTagList.LoadNetFromTagList(Program.Net);
                ModulesToTreeView();
                ModulesToListView();
            }
            
            NetChange();
            //ViewArchive(chbWriteArchive.Checked);
        }
        
        private void fmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult;
            
            if ((_flagRead != EFlagState.No) && (_flagRead != EFlagState.Inhibit))
            {
                var fmYesNoDialog = new FormYesNoDialog("Закрытие приложения", "Остановить опрос сети и закрыть приложение?");
                dialogResult = fmYesNoDialog.ShowDialog(this);
                if (dialogResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                Program.Net.ReadStopWait();
                _flagRead = EFlagState.Stop;
                ViewReadStop();
            }
            else
            {
                var fmYesNoDialog = new FormYesNoDialog("Закрытие приложения", "Закрыть приложение?");
                dialogResult = fmYesNoDialog.ShowDialog(this);
                if (dialogResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            Program.Params.Write();

            if ((Program.Params.SaveNetOnClose == null) && (Program.Net.Modules.Count > 0))
            {
                var fmYesNoDialog = new FormYesNoDialog("Сохранение конфигурации каналов", "Сохранить конфигурацию каналов?");
                dialogResult = fmYesNoDialog.ShowDialog(this);
            }
            else if ((Program.Params.SaveNetOnClose != false) && (Program.Net.Modules.Count == 0))
            {
                var fmYesNoDialog = new FormYesNoDialog("Сохранение конфигурации каналов",
                                                        "Сохранить пустую конфигурацию каналов?");
                dialogResult = fmYesNoDialog.ShowDialog(this);
            }
            else if (Program.Params.SaveNetOnClose == true) dialogResult = DialogResult.Yes;
            else dialogResult = DialogResult.No;
            
            if (dialogResult == DialogResult.Yes) NewTagList.SaveNetToTagList(Program.Net);
        }
        
    #region События элементов управления
        private void butSingleRead_Click(object sender, EventArgs e)
        {
            if (_flagRead != EFlagState.Inhibit)
            {
                _flagRead = EFlagState.Yes;
                string archiveFileName = (chbWriteArchive.Checked) ? tbArchiveFile.Text : null;
                Program.Net.ReadSingle(archiveFileName, true);
                _flagRead = EFlagState.No;
            }
        }

        private void butCyclicRead_Click(object sender, EventArgs e)
        {
            if (_flagRead != EFlagState.Inhibit)
            {
                if (_flagRead == EFlagState.No)
                {
                    _flagRead = EFlagState.Yes;
                    butCyclicRead.Text = TextStop;
                    //UpdateFormView();
                    ViewReadYes();
                    ViewArchive(chbWriteArchive.Checked);

                    //архив
                    string archiveFileName = (chbWriteArchive.Checked) ? tbArchiveFile.Text : null;
                    Program.Net.ReadThreadStart(archiveFileName, true);
                }
                else
                {
                    var fmYesNoDialog = new FormYesNoDialog("Опрос сети", "Остановить опрос сети?");
                    DialogResult dialogResult = fmYesNoDialog.ShowDialog(this);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Program.Net.ReadStop();
                        _flagRead = EFlagState.Stop;
                        ViewReadStop();
                    }
                }
            }
        }
        
        private void butQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void butScanNet_Click(object sender, EventArgs e)
        {
            if ((_flagRead == EFlagState.No) || (_flagRead == EFlagState.Inhibit))
            {
                var fmScanNet = new FormScanNet();
                fmScanNet.ShowDialog();
            }
        }

        private void butSelectArchiveFile_Click(object sender, EventArgs e)
        {
            var fileDlg = new OpenFileDialog
                              {
                                  Title = @"Файл архива",
                                  InitialDirectory = (tbArchiveFile.Text != ""  ? tbArchiveFile.Text : Application.StartupPath /*typeof(fmMonitor).Assembly.Location*/),
                                  Filter = @"Базы данных Access (mdb, mde, accdb, accde)|*.mdb; *.mde; *.accdb; *.accde",
                                  ValidateNames = true,
                                  CheckFileExists = false,
                                  //ShowReadOnly = true
                              };
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = fileDlg.FileName;
                FileArchiveChange(fileName);
            }
        }

        private void butSetup_Click(object sender, EventArgs e)
        {
            if ((_flagRead == EFlagState.No) || (_flagRead == EFlagState.Inhibit))
            {
                var fmSetup = new FormSetup();
                fmSetup.ShowDialog();
            }
        }
        
        private void chbWriteArchive_Click(object sender, EventArgs e)
        {
            if (chbWriteArchive.Checked)
            {
                if (_flagRead == EFlagState.Yes)
                {
                    Program.Net.ArchiveStart(tbArchiveFile.Text);
                }
                ViewArchive(true);
            }
            else
            {
                DialogResult stopWrite = DialogResult.Yes;
                if (_flagRead == EFlagState.Yes)
                {
                    var fmYesNoDialog = new FormYesNoDialog("Запиь в архив", "Остановить запись в архив?");
                    stopWrite = fmYesNoDialog.ShowDialog(this);
                }

                if (stopWrite == DialogResult.Yes)
                {
                    Program.Net.ArchiveStop();
                    ViewArchive(false);
                }
                else
                    chbWriteArchive.Checked = true;
            }
        }
        
        private void lvSignalList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string moduleAddress = lvSignalList.Items[e.Index].Name.Substring(1, 2);
            byte channel = byte.Parse(lvSignalList.Items[e.Index].Name.Substring(4, 1));
            Program.Net.Modules[moduleAddress].Channels[channel].Selected = (e.NewValue == CheckState.Checked);
        }

        private void tvModuleTree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((byte)e.KeyChar == 13) && (ModifierKeys == Keys.Control))
            {
                if (tvModuleTree.SelectedNode != null)
                {
                    string moduleAddress = (tvModuleTree.SelectedNode.Parent == null)
                                               ? tvModuleTree.SelectedNode.Name.Substring(1)
                                               : tvModuleTree.SelectedNode.Parent.Name.Substring(1);

                    if ((_formModule == null) || (_formModule.IsDisposed))
                    {
                        _formModule = new FormModule4019P(moduleAddress);
                        _formModule.Show();
                    }
                    else
                    {
                        _formModule.SelectModule(moduleAddress);
                        _formModule.Activate();
                    }
                }
            }
        }

        public GraphicList GraphicList = new GraphicList();

        private void butAddGraphic_Click(object sender, EventArgs e)
        {
            if (lvSignalList.SelectedItems.Count > 0)
            {
                //var fmGraphic = new FormGraphics();
                ////fmGraphic.AddGraphic(lvSignalList.SelectedItems[0].SubItems[7].Text,
                ////                     lvSignalList.SelectedItems[0].SubItems[8].Text);
                //fmGraphic.AddGraphic(lvSignalList.SelectedItems[0].SubItems[1].Text);
                //fmGraphic.Text = lvSignalList.SelectedItems[0].SubItems[1].Text ?? 
                //                 "Модуль " + lvSignalList.SelectedItems[0].SubItems[7].Text + " Канал " +
                //                 lvSignalList.SelectedItems[0].SubItems[8].Text;
                //fmGraphic.Show();

                var code = lvSignalList.SelectedItems[0].SubItems[1].Text ??
                           "Модуль " + lvSignalList.SelectedItems[0].SubItems[7].Text + " Канал " +
                           lvSignalList.SelectedItems[0].SubItems[8].Text;

                var chn = Program.Net.Modules.Channel(code);
                NewEnumSignalStatus status;
                double? min = (chn.Min == null) || (chn.Conversion == null) ? chn.Min : chn.SignalConversion((double)chn.Min, out status);
                double? max = (chn.Max == null) || (chn.Conversion == null) ? chn.Max : chn.SignalConversion((double)chn.Max, out status);

                string dataFormat = null;
                switch (chn.Module.ModuleType)
                {
                    case "Adam4019+":
                        dataFormat = ((NewModuleAdam4019Plus)chn.Module).DataFormat;
                        break;
                }

                GraphicList.AddGraphic(code, chn.Name, "", chn.Units, CommonTypes.DataType.Real, min, max);
            }
        }
    #endregion

        //private bool WriteArchiveChanged(bool writeArchive)
        //{
        //    if (chbWriteArchive.Checked)
        //    {
        //        if ((!Program.Archive.IsConnected()) && ((_flagRead == EFlagState.Yes) || (_flagRead == EFlagState.Stop)))
        //            if (Program.Archive.Connect(Program.Params.ArchiveFileName, false) == 0)
        //            {
        //                Program.Params.WriteArchive = true;
        //                Program.Net.WriteArchiveStart();
        //                UpdateFormView();
        //                return true;
        //            }
        //    }
        //    else
        //    {
        //        DialogResult stopWrite = DialogResult.Yes;
        //        if (_flagRead == EFlagState.Yes)
        //        {
        //            var fmYesNoDialog = new FormYesNoDialog("Запиь в архив", "Остановить запись в архив?");
        //            stopWrite = fmYesNoDialog.ShowDialog(this);
        //        }

        //        if (stopWrite == DialogResult.Yes)
        //        {
        //            //делагат остановки
        //            Program.Net.WriteArchiveStop();

        //            Action dlgStopRead = Program.Net.WriteArchiveStop;
        //            IAsyncResult asyncResult = dlgStopRead.BeginInvoke(null, null);
        //            while (!asyncResult.IsCompleted)
        //            {
        //                Application.DoEvents();
        //            }
        //            //ClassLog.AddLog(ELogType.Event, null, "Запись в архив остановлена");

        //            Program.Archive.Disconnect();
        //            UpdateFormView();
        //            Program.Params.WriteArchive = false;
        //        }
        //        else
        //            chbWriteArchive.Checked = true;
        //    }
        //}
        
    #region Изменения формы или параметров
        public void CurrentPortChange(byte? currentPort)
        {
            if ((currentPort != null) && (currentPort != 0))
            {
                labMode.BackColor = Color.Lime;
                labMode.Text = @"Порт " + currentPort;
            }
            else
            {
                labMode.BackColor = Color.Yellow;
                labMode.Text = @"Режим имитации";
            }
        }

        public void NetChange()
        {
            _flagRead = (Program.Net.Modules.Count != 0) ? EFlagState.No : EFlagState.Inhibit;
            //UpdateFormView();
            ViewReadInhibit();
        }
        
        //public void UpdateFormView()
        //{
        //    if (InvokeRequired == false)
        //    {
        //        if (_flagRead == EFlagState.Inhibit)
        //        {
        //            labRead.Text = @"Опрос невозможен";
        //            //var tt = new ToolTip();
        //            //tt.SetToolTip(labRead, "Требуется сканирование сети");
        //            labRead.BackColor = Color.Red;
        //            labRead.Visible = true;

        //            butCyclicRead.Enabled = false;
        //            butSingleRead.Enabled = false;
        //        }
        //        else
        //        {
        //            labRead.Text = @"Цикл. опрос вкл.";
        //            //var tt = new ToolTip();
        //            //tt.SetToolTip(labRead, "Идет опрос сети");
        //            labRead.BackColor = Color.Lime;
        //            butCyclicRead.Enabled = true;

        //            if (_flagRead == EFlagState.No)
        //            {
        //                labRead.Visible = false;
        //                butCyclicRead.Text = TextStart;
        //                butSingleRead.Enabled = true;
        //                butScanNet.Enabled = true;
        //                butSetup.Enabled = true;
        //            }
        //            else
        //            {
        //                labRead.Visible = true;
        //                butCyclicRead.Text = TextStop;
        //                butSingleRead.Enabled = false;
        //                butScanNet.Enabled = false;
        //                butSetup.Enabled = false;
        //            }
        //        }

        //        if (chbWriteArchive.Enabled == false)
        //        {
        //            labWriteArchive.Text = @"Запись в архив невозможна";
        //            //var tt = new ToolTip();
        //            //tt.SetToolTip(labWriteArchive, "Не выбран файл архива");
        //            labWriteArchive.BackColor = Color.Red;
        //            labWriteArchive.Visible = true;
        //            chbWriteArchive.Enabled = false;
        //        }
        //        else
        //        {
        //            labWriteArchive.Text = @"Запись в архив вкл.";
        //            //var tt = new ToolTip();
        //            //tt.SetToolTip(labWriteArchive, "");
        //            labWriteArchive.BackColor = Color.Lime;
        //            chbWriteArchive.Enabled = true;

        //            if (chbWriteArchive.Checked == false)
        //            {
        //                labWriteArchive.Visible = false;
        //                tbArchiveFile.Enabled = true;
        //                butSelectArchiveFile.Enabled = true;
        //            }
        //            else //if (chbWriteArchive.Checked == true)
        //            {
        //                labWriteArchive.Visible = true;

        //                if ((_flagRead == EFlagState.Inhibit) || (_flagRead == EFlagState.No))
        //                {
        //                    tbArchiveFile.Enabled = true;
        //                    butSelectArchiveFile.Enabled = true;
        //                }
        //                else
        //                {
        //                    tbArchiveFile.Enabled = false;
        //                    butSelectArchiveFile.Enabled = false;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var dlg = new Action(UpdateFormView);
        //        BeginInvoke(dlg);
        //    }
        //}

        private void FileArchiveChange(string fileName, bool showAlerts = true)
        {
            Exception ex;
            int recCnt;

            if (NewAuditArchive.TryConnect(fileName, out recCnt, out ex))
            {
                tbArchiveFile.Text = fileName;
                labArchiveSize.Text = recCnt.ToString();
                chbWriteArchive.Enabled = true;
            }
            else
            {
                if (showAlerts) MessageBox.Show(ex.Message);

                tbArchiveFile.Text = null;
                labArchiveSize.Text = null;
                chbWriteArchive.Checked = false;
                chbWriteArchive.Enabled = false;
            }

            if (chbWriteArchive.Enabled == false)
            {
                labWriteArchive.Text = @"Запись в архив невозможна";
                //var tt = new ToolTip();
                //tt.SetToolTip(labWriteArchive, "Не выбран файл архива");
                labWriteArchive.BackColor = Color.Red;
                labWriteArchive.Visible = true;
                chbWriteArchive.Enabled = false;
            }
            else
            {
                labWriteArchive.Text = @"Запись в архив вкл.";
                //var tt = new ToolTip();
                //tt.SetToolTip(labWriteArchive, "");
                labWriteArchive.BackColor = Color.Lime;
                chbWriteArchive.Enabled = true;

                if (chbWriteArchive.Checked == false)
                {
                    labWriteArchive.Visible = false;
                    tbArchiveFile.Enabled = true;
                    butSelectArchiveFile.Enabled = true;
                }
                else //if (chbWriteArchive.Checked == true)
                {
                    labWriteArchive.Visible = true;

                    if ((_flagRead == EFlagState.Inhibit) || (_flagRead == EFlagState.No))
                    {
                        tbArchiveFile.Enabled = true;
                        butSelectArchiveFile.Enabled = true;
                    }
                    else
                    {
                        tbArchiveFile.Enabled = false;
                        butSelectArchiveFile.Enabled = false;
                    }
                }
            }
        }
    #endregion Изменения формы или параметров
        
    #region TreeView, ListView
        public void ClearModulesTreeView()
        {
            if (tvModuleTree.InvokeRequired == false)
            {
                tvModuleTree.Nodes.Clear();
            }
            else
            {
                var dlg = new Action(ClearModulesTreeView);
                tvModuleTree.BeginInvoke(dlg);
            }
        }

        public void ClearSignalListView()
        {
            if (lvSignalList.InvokeRequired == false)
            {
                lvSignalList.Items.Clear();
            }
            else
            {
                var dlg = new Action(ClearSignalListView);
                lvSignalList.BeginInvoke(dlg);
            }
        }

        public void AddModuleToTreeView(string moduleAddress)
        {
            if (tvModuleTree.InvokeRequired == false)
            {
                NewModuleAbstract module = Program.Net.Modules[moduleAddress];

                if (module.Address != null)
                {
                    string nodeKey = "M" + module.Address;
                    string nodeText = "Module " + module.Address;

                    if (!tvModuleTree.Nodes.ContainsKey(nodeKey))
                    {
                        tvModuleTree.Nodes.Add(nodeKey, nodeText);

                        foreach (NewChannelAbstract channel in module.Channels)
                        {
                            string nodeChieldKey = nodeKey + "_C" + channel.Channel;
                            nodeText = (!channel.IsCjc) ? "Channel " + channel.Channel : "CJC";
                            if (!tvModuleTree.Nodes[nodeKey].Nodes.ContainsKey(nodeChieldKey))
                            {
                                tvModuleTree.Nodes[nodeKey].Nodes.Add(nodeChieldKey, nodeText);
                            }
                            else
                            {
                                ClassLog.AddLog(ELogType.Event, "ClassViews.AddModuleToTreeView",
                                                "Дерево модуля уже содержит подузел "
                                                + nodeChieldKey + " в узле " + nodeKey, DateTime.Now);
                            }
                        }
                    }
                    else
                    {
                        ClassLog.AddLog(ELogType.Event, "ClassViews.AddModuleToTreeView",
                                        "Дерево модулей уже содержит узел " + nodeKey, DateTime.Now);
                    }
                }
            }
            else
            {
                var dlg = new Action<string>(AddModuleToTreeView);
                tvModuleTree.BeginInvoke(dlg, new object[] {moduleAddress});
            }
        }

        public void ModulesToTreeView()
        {
            if (tvModuleTree.InvokeRequired == false)
            {
                tvModuleTree.Nodes.Clear();
                
                foreach (NewModuleAbstract module in Program.Net.Modules)
                {
                    if (module.Address != null)
                    {
                        string nodeKey = "M" + module.Address;
                        string nodeText = "Module " + module.Address;

                        if (!tvModuleTree.Nodes.ContainsKey(nodeKey))
                        {
                            tvModuleTree.Nodes.Add(nodeKey, nodeText);

                            foreach (NewChannelAbstract channel in module.Channels)
                            {
                                string nodeChieldKey = nodeKey + "_C" + channel.Channel;
                                nodeText = (!channel.IsCjc) ? "Channel " + channel.Channel : "CJC";
                                if (!tvModuleTree.Nodes[nodeKey].Nodes.ContainsKey(nodeChieldKey))
                                {
                                    tvModuleTree.Nodes[nodeKey].Nodes.Add(nodeChieldKey, nodeText);
                                }
                                else
                                {
                                    ClassLog.AddLog(ELogType.Event, "ClassViews.ModulesToTreeView",
                                                    "Дерево модуля уже содержит подузел "
                                                    + nodeChieldKey + " в узле " + nodeKey, DateTime.Now);
                                }
                            }
                        }
                        else
                        {
                            ClassLog.AddLog(ELogType.Event, "ClassViews.ModulesToTreeView",
                                            "Дерево модулей уже содержит узел " + nodeKey, DateTime.Now);
                        }
                    }
                }
            }
            else
            {
                var dlg = new Action(ModulesToTreeView);
                tvModuleTree.BeginInvoke(dlg);
            }
        }

        public void AddModuleToListView(string moduleAddress)
        {
            if (lvSignalList.InvokeRequired == false)
            {
                NewModuleAbstract module = Program.Net.Modules[moduleAddress];

                if (module != null)
                {
                    foreach (NewChannelAbstract channel in module.Channels)
                    {
                        string itemKey = "M" + channel.Module.Address + "C" + channel.Channel;
                        string itemText = (lvSignalList.Items.Count + 1).ToString();

                        if (!lvSignalList.Items.ContainsKey(itemKey))
                        {
                            ListViewItem curListItem = lvSignalList.Items.Add(itemKey, itemText, null);
                            curListItem.Checked = channel.Selected;
                            curListItem.SubItems.Add(channel.Code);
                            curListItem.SubItems.Add(channel.Name);
                            curListItem.SubItems.Add(channel.Signal);
                            curListItem.SubItems.Add(channel.Value.ToString());
                            curListItem.SubItems.Add(channel.Time.ToString());
                            //curListItem.SubItems.Add(((int)channel.Status).ToString());
                            curListItem.SubItems.Add((channel.Status != null) ? ((int)channel.Status).ToString() : null);
                            curListItem.SubItems.Add(channel.Module.Address10.ToString());
                            curListItem.SubItems.Add((!channel.IsCjc) ? channel.Channel.ToString() : "CJC");
                            curListItem.SubItems.Add(channel.Aperture.ToString());
                            curListItem.SubItems.Add(channel.Min.ToString());
                            curListItem.SubItems.Add(channel.Max.ToString());

                            switch (channel.ChannelType)
                            {
                                case "Adam4019+":
                                    curListItem.SubItems.Add(((NewChannelAdamAbstract)channel).DataFormat);
                                    curListItem.SubItems.Add(((NewChannelAdamAbstract)channel).ChannelRange);
                                    break;
                                default:
                                    curListItem.SubItems.Add("");
                                    curListItem.SubItems.Add("");
                                    break;
                            }

                            curListItem.SubItems.Add(channel.InLevel);
                            curListItem.SubItems.Add(channel.Conversion);
                            curListItem.SubItems.Add(channel.Units);
                            curListItem.UseItemStyleForSubItems = false;
                        }
                        else
                        {
                            ClassLog.AddLog(ELogType.Event, "ClassViews.AddModuleToListView",
                                            "Элемент " + itemKey + " уже существует в списке каналов", DateTime.Now);
                            break;
                        }
                    }
                }
            }
            else
            {
                var dlg = new Action<string>(AddModuleToListView);
                lvSignalList.BeginInvoke(dlg, new object[] {moduleAddress});
            }
        }

        public void ModulesToListView()
        {
            if (lvSignalList.InvokeRequired == false)
            {
                lvSignalList.Items.Clear();
                
                foreach (NewModuleAbstract module in Program.Net.Modules)
                    foreach (NewChannelAbstract channel in module.Channels)
                    {
                        string itemKey = "M" + channel.Module.Address + "C" + channel.Channel;
                        string itemText = (lvSignalList.Items.Count + 1).ToString();

                        if (!lvSignalList.Items.ContainsKey(itemKey))
                        {
                            ListViewItem curListItem = lvSignalList.Items.Add(itemKey, itemText, null);
                            curListItem.Checked = channel.Selected;
                            curListItem.SubItems.Add(channel.Code);
                            curListItem.SubItems.Add(channel.Name);
                            curListItem.SubItems.Add(channel.Signal);
                            curListItem.SubItems.Add(channel.Value.ToString());
                            curListItem.SubItems.Add(channel.Time.ToString());
                            //curListItem.SubItems.Add(((int)channel.Status).ToString());
                            curListItem.SubItems.Add ( (channel.Status != null) ? ((int)channel.Status).ToString() : null);
                            curListItem.SubItems.Add(channel.Module.Address10.ToString());
                            curListItem.SubItems.Add((!channel.IsCjc) ? channel.Channel.ToString() : "CJC");
                            curListItem.SubItems.Add(channel.Aperture.ToString());
                            curListItem.SubItems.Add(channel.Min.ToString());
                            curListItem.SubItems.Add(channel.Max.ToString());

                            switch (channel.ChannelType)
                            {
                                case "Adam4019+":
                                    curListItem.SubItems.Add(((NewChannelAdamAbstract)channel).DataFormat);
                                    curListItem.SubItems.Add(((NewChannelAdamAbstract)channel).ChannelRange);
                                    break;
                                default:
                                    curListItem.SubItems.Add("");
                                    curListItem.SubItems.Add("");
                                    break;
                            }

                            curListItem.SubItems.Add(channel.InLevel);
                            curListItem.SubItems.Add(channel.Conversion);
                            curListItem.SubItems.Add(channel.Units);
                            curListItem.UseItemStyleForSubItems = false;
                        }
                        else
                        {
                            ClassLog.AddLog(ELogType.Event, "ClassViews.AddModuleToListView",
                                            "Элемент " + itemKey + " уже существует в списке каналов", DateTime.Now);
                            break;
                        }
                    }
            }
            else
            {
                var dlg = new Action(ModulesToListView);
                lvSignalList.BeginInvoke(dlg);
            }
        }

        public void UpdateModulesValuesInListView(DateTime time)
        {
            const byte maxDigitsAfterDecimal = 3;

            if (lvSignalList.InvokeRequired == false)
            {
                foreach (NewModuleAbstract module in Program.Net.Modules)
                    foreach (NewChannelAbstract channel in module.Channels)
                    {
                        lock (channel)
                        {
                            string itemKey = "M" + channel.Module.Address + "C" + channel.Channel;
                            if (lvSignalList.Items.ContainsKey(itemKey))
                            {
                                ListViewItem curListItem = lvSignalList.Items[itemKey];

                                curListItem.SubItems[3].Text = channel.Signal;
                                curListItem.SubItems[4].Text = (channel.Value != null)
                                                                   ? Math.Round((double) channel.Value,
                                                                                maxDigitsAfterDecimal).ToString()
                                                                   : null;
                                curListItem.SubItems[5].Text = channel.Time.ToString();
                                //curListItem.SubItems[6].Text = ((int)channel.Status).ToString();
                                curListItem.SubItems[6].Text = (channel.Status != null)
                                                                   ? ((int) channel.Status).ToString()
                                                                   : null;
                                if (channel.ChannelValue.Status == NewEnumSignalStatus.NoError)
                                    curListItem.SubItems[6].BackColor = SystemColors.Window;
                                else if (channel.ChannelValue.Status == NewEnumSignalStatus.NoRead)
                                    curListItem.SubItems[6].BackColor = Color.Yellow;
                                else curListItem.SubItems[6].BackColor = Color.Tomato;

                                //curListItem.SubItems[6].BackColor = (channel.Status == EChannelStatus.NoError)
                                //                                  ? SystemColors.Window : Color.Tomato;
                            }
                        }
                    }
            }
            else
            {
                var dlg = new Action<DateTime>(UpdateModulesValuesInListView);
                lvSignalList.BeginInvoke(dlg, new object[] {time});
            }
        }

        public void UpdateModulesParamInListView(string moduleAddress, byte channel, string paramName)
        {
            if (lvSignalList.InvokeRequired == false)
            {
                string itemKey = "M" + moduleAddress + "C" + channel;
                if (lvSignalList.Items.ContainsKey(itemKey))
                {
                    int subItem;
                    string value;

                    switch (paramName.ToLower())
                    {
                        case "code":
                            subItem = 1;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].Code;
                            break;
                        case "name":
                            subItem = 2;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].Name;
                            break;
                        case "aperture":
                            subItem = 9;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].Aperture.ToString();
                            break;
                        case "min":
                            subItem = 10;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].Min.ToString();
                            break;
                        case "max":
                            subItem = 11;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].Max.ToString();
                            break;
                        case "units":
                            subItem = 16;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].Units;
                            break;
                        case "dataformat":
                            subItem = 12;
                            //value = Program.Net.Modules[moduleAddress].Channels[channel].DataFormat;
                            switch (Program.Net.Modules[moduleAddress].ModuleType)
                            {
                                case "Adam4019+":
                                    value = ((NewModuleAdamAbstract)Program.Net.Modules[moduleAddress]).DataFormat;
                                    break;
                                default:
                                    value = null;
                                    break;
                            }
                            break;
                        case "channelrange":
                            subItem = 13;
                            //value = Program.Net.Modules[moduleAddress].Channels[channel]).ChannelRange;
                            switch (Program.Net.Modules[moduleAddress].ModuleType)
                            {
                                case "Adam4019+":
                                    value = ((NewChannelAdamAbstract)Program.Net.Modules[moduleAddress].Channels[channel]).ChannelRange;
                                    break;
                                default:
                                    value = null;
                                    break;
                            }
                            break;
                        case "inlevel":
                            subItem = 14;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].InLevel;
                            break;
                        case "conversion":
                            subItem = 15;
                            value = Program.Net.Modules[moduleAddress].Channels[channel].Conversion;
                            break;
                        default:
                            value = "";
                            subItem = 0;
                            break;
                    }

                    if (subItem > 0) lvSignalList.Items[itemKey].SubItems[subItem].Text = value;
                }
            }
            else
            {
                var dlg = new Action<string, byte, string>(UpdateModulesParamInListView);
                lvSignalList.BeginInvoke(dlg);
            }
        }

        public bool SetlvSignalListFontParams(string fontName = "Microsoft Sans Serif", float fontSize = 8.25F,
                                              bool bold = false, bool italic = false)
        {
            FontStyle fontStyle;
            if (bold && italic) fontStyle = FontStyle.Bold | FontStyle.Italic;
            else if (bold) fontStyle = FontStyle.Bold;
            else if (italic) fontStyle = FontStyle.Italic;
            else fontStyle = FontStyle.Regular;

            try
            {
                lvSignalList.Font = new Font(fontName, fontSize, fontStyle, GraphicsUnit.Point, 204);
                return true;
            }
            catch
            {
                return false;
            }
        }
    #endregion
        
    #region Доступ к данным
        public ListView LvSignalList
        {
            get { return lvSignalList; }
        }

        public string ArchiveFileName
        {
            get { return tbArchiveFile.Text; }
            set { FileArchiveChange(value, false); }
        }

        public bool WriteArchive
        {
            get { return chbWriteArchive.Checked; }
            set { if (ArchiveFileName != null) chbWriteArchive.Checked = value; }
        }

        public int SplitterDistanceListsLog
        {
            get { return splitContainerListsLog.SplitterDistance; }
            set { splitContainerListsLog.SplitterDistance = value; }
        }

        public int SplitterDistanceLists
        {
            get { return splitContainerLists.SplitterDistance; }
            set { splitContainerLists.SplitterDistance = value; }
        }

        public string SignalListColumnDisplayIndex
        {
            get
            {
                string columnDisplayIndex = "";
                foreach (ColumnHeader column in lvSignalList.Columns) columnDisplayIndex += "/" + column.DisplayIndex;
                return columnDisplayIndex.Substring(1);
            }

            set
            {
                string[] columnDisplayIndex = (value ?? "").Split(new[] {"/"}, StringSplitOptions.None);
                int i = 0;
                foreach (ColumnHeader column in lvSignalList.Columns)
                {
                    int j = -1;
                    if (i <= columnDisplayIndex.GetUpperBound(0))
                        if (!int.TryParse(columnDisplayIndex[i], out j)) j = -1;
                    if ((j >= 0) && (j <= lvSignalList.Columns.Count))
                        column.DisplayIndex = int.Parse(columnDisplayIndex[i]);
                    i++;
                }
            }
        }

        public string SignalListColumnWidths
        {
            get
            {
                string columnWidths = "";
                foreach (ColumnHeader column in lvSignalList.Columns) columnWidths += "/" + column.Width;
                return columnWidths.Substring(1);
            }

            set
            {
                string[] columnWidth = (value ?? "").Split(new[] {"/"}, StringSplitOptions.None);
                int i = 0;
                foreach (ColumnHeader column in lvSignalList.Columns)
                {
                    int j = -1;
                    if (i <= columnWidth.GetUpperBound(0))
                        if (!int.TryParse(columnWidth[i], out j)) j = -1;
                    if (j >= 0) column.Width = int.Parse(columnWidth[i]);
                    i++;
                }
            }
        }
    #endregion

    #region События
        public void UpdateLastReadTime(DateTime time)
        {
            if (labLastReadTime.InvokeRequired == false)
            {
                labLastReadTime.Text = time.ToString();
                //labLastReadTime.Text = DateTime.Now.ToString();
            }
            else
            {
                var dlg = new Action<DateTime>(UpdateLastReadTime);
                labLastReadTime.BeginInvoke(dlg, new object[] { time });
            }
        }

        public void UpdateArchiveSize(int rowCount)
        {
            if (labArchiveSize.InvokeRequired == false)
            {
                labArchiveSize.Text = rowCount.ToString();
            }
            else
            {
                var dlg = new Action<int>(UpdateArchiveSize);
                labArchiveSize.BeginInvoke(dlg, new object[] { rowCount });
            }
        }

        private void AddNewLog(string message, DateTime time)
        {
            //const int maxItemsCount = 100;

            if (lbLog.InvokeRequired == false)
            {
                //if (lbLog.Items.Count == maxItemsCount)
                //    lbLog.Items.RemoveAt(maxItemsCount - 1);

                string ms = time.Millisecond.ToString();
                while (ms.Length < 3) ms = "0" + ms;

                lbLog.Items.Insert(0, time + " " + ms + "   " + message);
                lbLog.SetSelected(0, true);
                lbLog.SetSelected(0, false);
            }
            else
            {
                var dlgNewLog = new Action<string, DateTime>(AddNewLog);
                BeginInvoke(dlgNewLog, new object[] {message, time});
            }
        }

        private void WriteArchiveError(bool isError)
        {
            if (labWriteArchiveError.InvokeRequired == false)
                labWriteArchiveError.Visible = isError;
            else
            {
                var dlgUpd = new Action<bool>(WriteArchiveError);
                BeginInvoke(dlgUpd, new object[] { isError });
            }
        }

        private void ConnectArchiveError()
        {
            if (InvokeRequired == false)
            {
                chbWriteArchive.Checked = false;
                ViewArchive(false);
            }
            else
            {
                var dlgUpd = new Action(ConnectArchiveError);
                BeginInvoke(dlgUpd);
            }
        }

        private void NetReadStop()
        {
            _flagRead = EFlagState.No;
            //UpdateFormView();
            ViewReadNo();
            ViewArchive(false);
        }
    #endregion

    #region Graphics
        private void ToolStripMenuItem_AddGraphic_Click(object sender, EventArgs e)
        {
            if (_activeItem != null)
            {
                var fmGraphic = new FormGraphics();
                //fmGraphic.AddGraphic(_activeItem.SubItems[7].Text, _activeItem.SubItems[8].Text);
                fmGraphic.AddGraphic(_activeItem.SubItems[1].Text);
                fmGraphic.Text = _activeItem.SubItems[1].Text ??
                                 "Модуль " + _activeItem.SubItems[7].Text + 
                                 " Канал " + _activeItem.SubItems[8].Text;
                fmGraphic.Show();
                _activeItem = null;
            }
        }

        private void сMenuAddGraphic_Opening(object sender, CancelEventArgs e)
        {
            _activeItem = lvSignalList.GetItemAt(lvSignalList.PointToClient(Cursor.Position).X,
                                                 lvSignalList.PointToClient(Cursor.Position).Y);
        }
    #endregion Graphics

    #region FormView
        private void ViewReadInhibit()
        {
            if (InvokeRequired == false)
            {
                if (_flagRead == EFlagState.Inhibit)
                {

                    labRead.Text = @"Опрос невозможен";
                    //var tt = new ToolTip();
                    //tt.SetToolTip(labRead, "Требуется сканирование сети");
                    labRead.BackColor = Color.Red;
                    labRead.Visible = true;

                    butCyclicRead.Enabled = false;
                    butSingleRead.Enabled = false;

                    //butScanNet.Enabled = true;
                    //butSetup.Enabled = true;
                }
                else
                {
                    labRead.Text = @"Цикл. опрос вкл.";
                    //var tt = new ToolTip();
                    //tt.SetToolTip(labRead, "Идет опрос сети");
                    labRead.BackColor = Color.Lime;
                    labRead.Visible = false;

                    butCyclicRead.Enabled = true;
                    butSingleRead.Enabled = true;

                    //butScanNet.Enabled = true;
                    //butSetup.Enabled = true;
                }
            }
            else
            {
                var dlg = new Action(ViewReadInhibit);
                BeginInvoke(dlg);
            }
        }

        private void ViewReadYes()
        {
            if (InvokeRequired == false)
            {
                labRead.Visible = true;

                butCyclicRead.Text = TextStop;
                butSingleRead.Enabled = false;
                
                butScanNet.Enabled = false;
                butSetup.Enabled = false;
            }
            else
            {
                var dlg = new Action(ViewReadYes);
                BeginInvoke(dlg);
            }
        }

        private void ViewReadStop()
        {
            if (InvokeRequired == false)
            {
                //labRead.Visible = false;
                
                butCyclicRead.Text = TextStart;
                //butSingleRead.Enabled = false;
                
                //butScanNet.Enabled = true;
                //butSetup.Enabled = true;
            }
            else
            {
                var dlg = new Action(ViewReadStop);
                BeginInvoke(dlg);
            }
        }

        private void ViewReadNo()
        {
            if (InvokeRequired == false)
            {
                labRead.Visible = false;
                //butCyclicRead.Text = TextStart;
                butSingleRead.Enabled = true;
                butScanNet.Enabled = true;
                butSetup.Enabled = true;
            }
            else
            {
                var dlg = new Action(ViewReadNo);
                BeginInvoke(dlg);
            }
        }

        private void ViewArchive(bool writeArchive)
        {
            if (InvokeRequired == false)
            {
                if (writeArchive)
                {
                    labWriteArchive.Visible = true;
                    if (_flagRead == EFlagState.Yes) butSelectArchiveFile.Enabled = false;
                }
                else
                {
                    labWriteArchive.Visible = false;
                    butSelectArchiveFile.Enabled = true;
                }
            }
            else
            {
                var dlg = new Action<bool>(ViewArchive);
                BeginInvoke(dlg, writeArchive);
            }
        }
    #endregion
    }
}