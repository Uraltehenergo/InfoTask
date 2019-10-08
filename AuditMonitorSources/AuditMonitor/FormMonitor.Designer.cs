namespace AuditMonitor
{
    partial class FormMonitor
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labMode = new System.Windows.Forms.Label();
            this.labRead = new System.Windows.Forms.Label();
            this.labWriteArchive = new System.Windows.Forms.Label();
            this.panStatus = new System.Windows.Forms.Panel();
            this.labWriteArchiveError = new System.Windows.Forms.Label();
            this.panButtons = new System.Windows.Forms.Panel();
            this.butQuit = new System.Windows.Forms.Button();
            this.butSetup = new System.Windows.Forms.Button();
            this.butScanNet = new System.Windows.Forms.Button();
            this.gbRead = new System.Windows.Forms.GroupBox();
            this.labLastReadTime = new System.Windows.Forms.Label();
            this.labLastRead = new System.Windows.Forms.Label();
            this.butSingleRead = new System.Windows.Forms.Button();
            this.butCyclicRead = new System.Windows.Forms.Button();
            this.gbArchive = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labArchiveSize1 = new System.Windows.Forms.Label();
            this.labArchiveSize2 = new System.Windows.Forms.Label();
            this.labArchiveSize = new System.Windows.Forms.Label();
            this.chbWriteArchive = new System.Windows.Forms.CheckBox();
            this.butSelectArchiveFile = new System.Windows.Forms.Button();
            this.tbArchiveFile = new System.Windows.Forms.TextBox();
            this.labArchiveFile = new System.Windows.Forms.Label();
            this.splitContainerListsLog = new System.Windows.Forms.SplitContainer();
            this.splitContainerLists = new System.Windows.Forms.SplitContainer();
            this.tvModuleTree = new System.Windows.Forms.TreeView();
            this.lvSignalList = new System.Windows.Forms.ListView();
            this.chNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSignal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chModule = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chChannel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAperture = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDataFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chChannelRange = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chInLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chConversion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chUnits = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.сMenuAddGraphic = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemAddGraphic = new System.Windows.Forms.ToolStripMenuItem();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.butAddGraphic = new System.Windows.Forms.Button();
            this.panStatus.SuspendLayout();
            this.panButtons.SuspendLayout();
            this.gbRead.SuspendLayout();
            this.gbArchive.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListsLog)).BeginInit();
            this.splitContainerListsLog.Panel1.SuspendLayout();
            this.splitContainerListsLog.Panel2.SuspendLayout();
            this.splitContainerListsLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLists)).BeginInit();
            this.splitContainerLists.Panel1.SuspendLayout();
            this.splitContainerLists.Panel2.SuspendLayout();
            this.splitContainerLists.SuspendLayout();
            this.сMenuAddGraphic.SuspendLayout();
            this.gbLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // labMode
            // 
            this.labMode.BackColor = System.Drawing.Color.Yellow;
            this.labMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labMode.ForeColor = System.Drawing.Color.Blue;
            this.labMode.Location = new System.Drawing.Point(0, 0);
            this.labMode.Name = "labMode";
            this.labMode.Size = new System.Drawing.Size(105, 35);
            this.labMode.TabIndex = 0;
            this.labMode.Text = "Режим Имитации";
            this.labMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labRead
            // 
            this.labRead.BackColor = System.Drawing.Color.Lime;
            this.labRead.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labRead.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labRead.ForeColor = System.Drawing.Color.Blue;
            this.labRead.Location = new System.Drawing.Point(111, 0);
            this.labRead.Name = "labRead";
            this.labRead.Size = new System.Drawing.Size(105, 35);
            this.labRead.TabIndex = 1;
            this.labRead.Text = "Цикл. опрос вкл. ";
            this.labRead.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labRead.Visible = false;
            // 
            // labWriteArchive
            // 
            this.labWriteArchive.BackColor = System.Drawing.Color.Lime;
            this.labWriteArchive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labWriteArchive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labWriteArchive.ForeColor = System.Drawing.Color.Blue;
            this.labWriteArchive.Location = new System.Drawing.Point(222, 0);
            this.labWriteArchive.Name = "labWriteArchive";
            this.labWriteArchive.Size = new System.Drawing.Size(105, 35);
            this.labWriteArchive.TabIndex = 2;
            this.labWriteArchive.Text = "Запись в архив вкл. ";
            this.labWriteArchive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labWriteArchive.Visible = false;
            // 
            // panStatus
            // 
            this.panStatus.Controls.Add(this.labWriteArchiveError);
            this.panStatus.Controls.Add(this.labWriteArchive);
            this.panStatus.Controls.Add(this.labRead);
            this.panStatus.Controls.Add(this.labMode);
            this.panStatus.Location = new System.Drawing.Point(12, 12);
            this.panStatus.Name = "panStatus";
            this.panStatus.Size = new System.Drawing.Size(438, 35);
            this.panStatus.TabIndex = 1;
            // 
            // labWriteArchiveError
            // 
            this.labWriteArchiveError.BackColor = System.Drawing.Color.Red;
            this.labWriteArchiveError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labWriteArchiveError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labWriteArchiveError.ForeColor = System.Drawing.Color.White;
            this.labWriteArchiveError.Location = new System.Drawing.Point(333, 0);
            this.labWriteArchiveError.Name = "labWriteArchiveError";
            this.labWriteArchiveError.Size = new System.Drawing.Size(105, 35);
            this.labWriteArchiveError.TabIndex = 3;
            this.labWriteArchiveError.Text = "Ошибка записи в Архив";
            this.labWriteArchiveError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labWriteArchiveError.Visible = false;
            // 
            // panButtons
            // 
            this.panButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panButtons.Controls.Add(this.butQuit);
            this.panButtons.Controls.Add(this.butSetup);
            this.panButtons.Controls.Add(this.butScanNet);
            this.panButtons.Location = new System.Drawing.Point(590, 12);
            this.panButtons.Name = "panButtons";
            this.panButtons.Size = new System.Drawing.Size(327, 35);
            this.panButtons.TabIndex = 2;
            // 
            // butQuit
            // 
            this.butQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butQuit.Location = new System.Drawing.Point(222, 0);
            this.butQuit.Name = "butQuit";
            this.butQuit.Size = new System.Drawing.Size(105, 35);
            this.butQuit.TabIndex = 2;
            this.butQuit.Text = "Выход";
            this.butQuit.UseVisualStyleBackColor = true;
            this.butQuit.Click += new System.EventHandler(this.butQuit_Click);
            // 
            // butSetup
            // 
            this.butSetup.Location = new System.Drawing.Point(111, 0);
            this.butSetup.Name = "butSetup";
            this.butSetup.Size = new System.Drawing.Size(105, 35);
            this.butSetup.TabIndex = 1;
            this.butSetup.Text = "Настройки";
            this.butSetup.UseVisualStyleBackColor = true;
            this.butSetup.Click += new System.EventHandler(this.butSetup_Click);
            // 
            // butScanNet
            // 
            this.butScanNet.Location = new System.Drawing.Point(0, 0);
            this.butScanNet.Name = "butScanNet";
            this.butScanNet.Size = new System.Drawing.Size(105, 35);
            this.butScanNet.TabIndex = 0;
            this.butScanNet.Text = "Скан. сети";
            this.butScanNet.UseVisualStyleBackColor = true;
            this.butScanNet.Click += new System.EventHandler(this.butScanNet_Click);
            // 
            // gbRead
            // 
            this.gbRead.Controls.Add(this.labLastReadTime);
            this.gbRead.Controls.Add(this.labLastRead);
            this.gbRead.Controls.Add(this.butSingleRead);
            this.gbRead.Controls.Add(this.butCyclicRead);
            this.gbRead.Location = new System.Drawing.Point(12, 52);
            this.gbRead.Name = "gbRead";
            this.gbRead.Size = new System.Drawing.Size(216, 86);
            this.gbRead.TabIndex = 3;
            this.gbRead.TabStop = false;
            this.gbRead.Text = "Опрос";
            // 
            // labLastReadTime
            // 
            this.labLastReadTime.Location = new System.Drawing.Point(103, 63);
            this.labLastReadTime.Name = "labLastReadTime";
            this.labLastReadTime.Size = new System.Drawing.Size(107, 13);
            this.labLastReadTime.TabIndex = 4;
            // 
            // labLastRead
            // 
            this.labLastRead.AutoSize = true;
            this.labLastRead.Location = new System.Drawing.Point(6, 63);
            this.labLastRead.Name = "labLastRead";
            this.labLastRead.Size = new System.Drawing.Size(99, 13);
            this.labLastRead.TabIndex = 3;
            this.labLastRead.Text = "Последний опрос:";
            // 
            // butSingleRead
            // 
            this.butSingleRead.Location = new System.Drawing.Point(111, 19);
            this.butSingleRead.Name = "butSingleRead";
            this.butSingleRead.Size = new System.Drawing.Size(99, 34);
            this.butSingleRead.TabIndex = 2;
            this.butSingleRead.Text = "Однократный";
            this.butSingleRead.UseVisualStyleBackColor = true;
            this.butSingleRead.Click += new System.EventHandler(this.butSingleRead_Click);
            // 
            // butCyclicRead
            // 
            this.butCyclicRead.Location = new System.Drawing.Point(6, 19);
            this.butCyclicRead.Name = "butCyclicRead";
            this.butCyclicRead.Size = new System.Drawing.Size(99, 34);
            this.butCyclicRead.TabIndex = 1;
            this.butCyclicRead.Text = "Циклический";
            this.butCyclicRead.UseVisualStyleBackColor = true;
            this.butCyclicRead.Click += new System.EventHandler(this.butCyclicRead_Click);
            // 
            // gbArchive
            // 
            this.gbArchive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbArchive.Controls.Add(this.panel1);
            this.gbArchive.Controls.Add(this.chbWriteArchive);
            this.gbArchive.Controls.Add(this.butSelectArchiveFile);
            this.gbArchive.Controls.Add(this.tbArchiveFile);
            this.gbArchive.Controls.Add(this.labArchiveFile);
            this.gbArchive.Location = new System.Drawing.Point(234, 53);
            this.gbArchive.Name = "gbArchive";
            this.gbArchive.Size = new System.Drawing.Size(683, 85);
            this.gbArchive.TabIndex = 4;
            this.gbArchive.TabStop = false;
            this.gbArchive.Text = "Архив";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.labArchiveSize1);
            this.panel1.Controls.Add(this.labArchiveSize2);
            this.panel1.Controls.Add(this.labArchiveSize);
            this.panel1.Location = new System.Drawing.Point(57, 59);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(475, 20);
            this.panel1.TabIndex = 9;
            // 
            // labArchiveSize1
            // 
            this.labArchiveSize1.AutoSize = true;
            this.labArchiveSize1.Location = new System.Drawing.Point(3, 4);
            this.labArchiveSize1.Name = "labArchiveSize1";
            this.labArchiveSize1.Size = new System.Drawing.Size(88, 13);
            this.labArchiveSize1.TabIndex = 6;
            this.labArchiveSize1.Text = "Размер Архива:";
            // 
            // labArchiveSize2
            // 
            this.labArchiveSize2.AutoSize = true;
            this.labArchiveSize2.Location = new System.Drawing.Point(166, 4);
            this.labArchiveSize2.Name = "labArchiveSize2";
            this.labArchiveSize2.Size = new System.Drawing.Size(49, 13);
            this.labArchiveSize2.TabIndex = 8;
            this.labArchiveSize2.Text = "записей";
            // 
            // labArchiveSize
            // 
            this.labArchiveSize.Location = new System.Drawing.Point(97, 4);
            this.labArchiveSize.Name = "labArchiveSize";
            this.labArchiveSize.Size = new System.Drawing.Size(63, 13);
            this.labArchiveSize.TabIndex = 7;
            this.labArchiveSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chbWriteArchive
            // 
            this.chbWriteArchive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbWriteArchive.Appearance = System.Windows.Forms.Appearance.Button;
            this.chbWriteArchive.Location = new System.Drawing.Point(578, 18);
            this.chbWriteArchive.Name = "chbWriteArchive";
            this.chbWriteArchive.Size = new System.Drawing.Size(99, 34);
            this.chbWriteArchive.TabIndex = 5;
            this.chbWriteArchive.TabStop = false;
            this.chbWriteArchive.Text = "Вкл/Откл запись в архив";
            this.chbWriteArchive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chbWriteArchive.UseVisualStyleBackColor = true;
            this.chbWriteArchive.Click += new System.EventHandler(this.chbWriteArchive_Click);
            // 
            // butSelectArchiveFile
            // 
            this.butSelectArchiveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectArchiveFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butSelectArchiveFile.Location = new System.Drawing.Point(538, 18);
            this.butSelectArchiveFile.Name = "butSelectArchiveFile";
            this.butSelectArchiveFile.Size = new System.Drawing.Size(34, 34);
            this.butSelectArchiveFile.TabIndex = 2;
            this.butSelectArchiveFile.Text = "...";
            this.butSelectArchiveFile.UseVisualStyleBackColor = true;
            this.butSelectArchiveFile.Click += new System.EventHandler(this.butSelectArchiveFile_Click);
            // 
            // tbArchiveFile
            // 
            this.tbArchiveFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbArchiveFile.BackColor = System.Drawing.SystemColors.Window;
            this.tbArchiveFile.Location = new System.Drawing.Point(57, 18);
            this.tbArchiveFile.Multiline = true;
            this.tbArchiveFile.Name = "tbArchiveFile";
            this.tbArchiveFile.ReadOnly = true;
            this.tbArchiveFile.Size = new System.Drawing.Size(475, 34);
            this.tbArchiveFile.TabIndex = 1;
            // 
            // labArchiveFile
            // 
            this.labArchiveFile.Location = new System.Drawing.Point(6, 19);
            this.labArchiveFile.Name = "labArchiveFile";
            this.labArchiveFile.Size = new System.Drawing.Size(45, 34);
            this.labArchiveFile.TabIndex = 0;
            this.labArchiveFile.Text = "Файл архива";
            this.labArchiveFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainerListsLog
            // 
            this.splitContainerListsLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerListsLog.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerListsLog.Location = new System.Drawing.Point(12, 144);
            this.splitContainerListsLog.Name = "splitContainerListsLog";
            this.splitContainerListsLog.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerListsLog.Panel1
            // 
            this.splitContainerListsLog.Panel1.Controls.Add(this.splitContainerLists);
            this.splitContainerListsLog.Panel1MinSize = 232;
            // 
            // splitContainerListsLog.Panel2
            // 
            this.splitContainerListsLog.Panel2.Controls.Add(this.gbLog);
            this.splitContainerListsLog.Panel2MinSize = 64;
            this.splitContainerListsLog.Size = new System.Drawing.Size(905, 340);
            this.splitContainerListsLog.SplitterDistance = 232;
            this.splitContainerListsLog.TabIndex = 5;
            // 
            // splitContainerLists
            // 
            this.splitContainerLists.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLists.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerLists.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLists.Name = "splitContainerLists";
            // 
            // splitContainerLists.Panel1
            // 
            this.splitContainerLists.Panel1.Controls.Add(this.tvModuleTree);
            this.splitContainerLists.Panel1MinSize = 108;
            // 
            // splitContainerLists.Panel2
            // 
            this.splitContainerLists.Panel2.Controls.Add(this.lvSignalList);
            this.splitContainerLists.Panel2MinSize = 108;
            this.splitContainerLists.Size = new System.Drawing.Size(905, 232);
            this.splitContainerLists.SplitterDistance = 108;
            this.splitContainerLists.TabIndex = 6;
            // 
            // tvModuleTree
            // 
            this.tvModuleTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvModuleTree.Location = new System.Drawing.Point(0, 0);
            this.tvModuleTree.Name = "tvModuleTree";
            this.tvModuleTree.Size = new System.Drawing.Size(108, 232);
            this.tvModuleTree.TabIndex = 0;
            this.tvModuleTree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvModuleTree_KeyPress);
            // 
            // lvSignalList
            // 
            this.lvSignalList.AllowColumnReorder = true;
            this.lvSignalList.BackColor = System.Drawing.SystemColors.Window;
            this.lvSignalList.CheckBoxes = true;
            this.lvSignalList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chNum,
            this.chCode,
            this.chName,
            this.chSignal,
            this.chValue,
            this.chTime,
            this.chStatus,
            this.chModule,
            this.chChannel,
            this.chAperture,
            this.chMin,
            this.chMax,
            this.chDataFormat,
            this.chChannelRange,
            this.chInLevel,
            this.chConversion,
            this.chUnits});
            this.lvSignalList.ContextMenuStrip = this.сMenuAddGraphic;
            this.lvSignalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSignalList.FullRowSelect = true;
            this.lvSignalList.GridLines = true;
            this.lvSignalList.Location = new System.Drawing.Point(0, 0);
            this.lvSignalList.Name = "lvSignalList";
            this.lvSignalList.ShowItemToolTips = true;
            this.lvSignalList.Size = new System.Drawing.Size(793, 232);
            this.lvSignalList.TabIndex = 0;
            this.lvSignalList.UseCompatibleStateImageBehavior = false;
            this.lvSignalList.View = System.Windows.Forms.View.Details;
            this.lvSignalList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lvSignalList_ItemCheck);
            // 
            // chNum
            // 
            this.chNum.Text = "№ пп";
            // 
            // chCode
            // 
            this.chCode.Text = "Код";
            // 
            // chName
            // 
            this.chName.Text = "Имя";
            // 
            // chSignal
            // 
            this.chSignal.Text = "Сигнал";
            this.chSignal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chValue
            // 
            this.chValue.Text = "Значение";
            this.chValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chTime
            // 
            this.chTime.Text = "Время";
            this.chTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chStatus
            // 
            this.chStatus.Text = "Статус";
            this.chStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chModule
            // 
            this.chModule.Text = "№ модуля";
            this.chModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chChannel
            // 
            this.chChannel.Text = "№ канала";
            this.chChannel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chAperture
            // 
            this.chAperture.Text = "Апертура";
            // 
            // chMin
            // 
            this.chMin.Text = "Min";
            // 
            // chMax
            // 
            this.chMax.Text = "Max";
            // 
            // chDataFormat
            // 
            this.chDataFormat.Text = "Формат данных";
            this.chDataFormat.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chChannelRange
            // 
            this.chChannelRange.Text = "Тип канала";
            this.chChannelRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chInLevel
            // 
            this.chInLevel.Text = "Тип сигнала";
            // 
            // chConversion
            // 
            this.chConversion.Text = "Преобраз. сигнала";
            // 
            // chUnits
            // 
            this.chUnits.Text = "Ед. изм.";
            // 
            // сMenuAddGraphic
            // 
            this.сMenuAddGraphic.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemAddGraphic});
            this.сMenuAddGraphic.Name = "сMenuAddGraphic";
            this.сMenuAddGraphic.Size = new System.Drawing.Size(170, 48);
            this.сMenuAddGraphic.Opening += new System.ComponentModel.CancelEventHandler(this.сMenuAddGraphic_Opening);
            // 
            // ToolStripMenuItemAddGraphic
            // 
            this.ToolStripMenuItemAddGraphic.Name = "ToolStripMenuItemAddGraphic";
            this.ToolStripMenuItemAddGraphic.Size = new System.Drawing.Size(169, 22);
            this.ToolStripMenuItemAddGraphic.Text = "Добавить график";
            this.ToolStripMenuItemAddGraphic.Click += new System.EventHandler(this.ToolStripMenuItem_AddGraphic_Click);
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.lbLog);
            this.gbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbLog.Location = new System.Drawing.Point(0, 0);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(905, 104);
            this.gbLog.TabIndex = 0;
            this.gbLog.TabStop = false;
            // 
            // lbLog
            // 
            this.lbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLog.FormattingEnabled = true;
            this.lbLog.Location = new System.Drawing.Point(3, 16);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(899, 85);
            this.lbLog.TabIndex = 0;
            // 
            // butAddGraphic
            // 
            this.butAddGraphic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butAddGraphic.Location = new System.Drawing.Point(479, 12);
            this.butAddGraphic.Name = "butAddGraphic";
            this.butAddGraphic.Size = new System.Drawing.Size(105, 35);
            this.butAddGraphic.TabIndex = 6;
            this.butAddGraphic.Text = "Добавить график";
            this.butAddGraphic.UseVisualStyleBackColor = true;
            this.butAddGraphic.Click += new System.EventHandler(this.butAddGraphic_Click);
            // 
            // FormMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 496);
            this.Controls.Add(this.butAddGraphic);
            this.Controls.Add(this.splitContainerListsLog);
            this.Controls.Add(this.gbArchive);
            this.Controls.Add(this.gbRead);
            this.Controls.Add(this.panButtons);
            this.Controls.Add(this.panStatus);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "FormMonitor";
            this.Text = "AuditMonitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fmMonitor_FormClosing);
            this.Load += new System.EventHandler(this.FormMonitor_Load);
            this.panStatus.ResumeLayout(false);
            this.panButtons.ResumeLayout(false);
            this.gbRead.ResumeLayout(false);
            this.gbRead.PerformLayout();
            this.gbArchive.ResumeLayout(false);
            this.gbArchive.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainerListsLog.Panel1.ResumeLayout(false);
            this.splitContainerListsLog.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListsLog)).EndInit();
            this.splitContainerListsLog.ResumeLayout(false);
            this.splitContainerLists.Panel1.ResumeLayout(false);
            this.splitContainerLists.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLists)).EndInit();
            this.splitContainerLists.ResumeLayout(false);
            this.сMenuAddGraphic.ResumeLayout(false);
            this.gbLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labMode;
        private System.Windows.Forms.Label labRead;
        private System.Windows.Forms.Label labWriteArchive;
        private System.Windows.Forms.Panel panStatus;
        private System.Windows.Forms.Label labWriteArchiveError;
        private System.Windows.Forms.Panel panButtons;
        private System.Windows.Forms.Button butQuit;
        private System.Windows.Forms.Button butSetup;
        private System.Windows.Forms.Button butScanNet;
        private System.Windows.Forms.GroupBox gbRead;
        private System.Windows.Forms.Label labLastReadTime;
        private System.Windows.Forms.Label labLastRead;
        private System.Windows.Forms.Button butSingleRead;
        private System.Windows.Forms.Button butCyclicRead;
        private System.Windows.Forms.GroupBox gbArchive;
        private System.Windows.Forms.Label labArchiveFile;
        private System.Windows.Forms.Button butSelectArchiveFile;
        private System.Windows.Forms.TextBox tbArchiveFile;
        private System.Windows.Forms.Label labArchiveSize2;
        private System.Windows.Forms.Label labArchiveSize;
        private System.Windows.Forms.Label labArchiveSize1;
        private System.Windows.Forms.CheckBox chbWriteArchive;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainerListsLog;
        private System.Windows.Forms.SplitContainer splitContainerLists;
        private System.Windows.Forms.TreeView tvModuleTree;
        private System.Windows.Forms.ListView lvSignalList;
        private System.Windows.Forms.ColumnHeader chCode;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSignal;
        private System.Windows.Forms.ColumnHeader chValue;
        private System.Windows.Forms.ColumnHeader chTime;
        private System.Windows.Forms.ColumnHeader chStatus;
        private System.Windows.Forms.ColumnHeader chModule;
        private System.Windows.Forms.ColumnHeader chChannel;
        private System.Windows.Forms.ColumnHeader chAperture;
        private System.Windows.Forms.ColumnHeader chMin;
        private System.Windows.Forms.ColumnHeader chMax;
        private System.Windows.Forms.ColumnHeader chDataFormat;
        private System.Windows.Forms.ColumnHeader chChannelRange;
        private System.Windows.Forms.ColumnHeader chInLevel;
        private System.Windows.Forms.ColumnHeader chConversion;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Button butAddGraphic;
        private System.Windows.Forms.ColumnHeader chUnits;
        public System.Windows.Forms.ColumnHeader chNum;
        private System.Windows.Forms.ContextMenuStrip сMenuAddGraphic;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAddGraphic;
    }
}

