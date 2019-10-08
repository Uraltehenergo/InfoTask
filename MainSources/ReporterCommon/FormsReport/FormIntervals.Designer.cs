namespace ReporterCommon
{
    partial class FormIntervals
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormIntervals));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.BindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.BindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ButClearFilter = new System.Windows.Forms.ToolStripButton();
            this.ButSetFilter = new System.Windows.Forms.ToolStripButton();
            this.butUncheckAll = new System.Windows.Forms.Button();
            this.butCheckAll = new System.Windows.Forms.Button();
            this.butDeleteIntervals = new System.Windows.Forms.Button();
            this.butLoadInterval = new System.Windows.Forms.Button();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.butFilter = new System.Windows.Forms.Button();
            this.Intervals = new System.Windows.Forms.DataGridView();
            this.Otm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PeriodBegin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PeriodEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IntervalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeChange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressReport = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingNavigator1)).BeginInit();
            this.BindingNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Intervals)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Delete.ico");
            this.imageList1.Images.SetKeyName(1, "OtmAll.ico");
            this.imageList1.Images.SetKeyName(2, "OtmAll_No.ICO");
            this.imageList1.Images.SetKeyName(3, "filter.png");
            this.imageList1.Images.SetKeyName(4, "filter_delete.png");
            this.imageList1.Images.SetKeyName(5, "find.png");
            this.imageList1.Images.SetKeyName(6, "Down.ICO");
            // 
            // BindingSource1
            // 
            this.BindingSource1.DataMember = "IntervalsTable";
            this.BindingSource1.DataSource = this.dataSet1;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5});
            this.dataTable1.TableName = "IntervalsTable";
            // 
            // dataColumn1
            // 
            this.dataColumn1.AllowDBNull = false;
            this.dataColumn1.Caption = "Отм";
            this.dataColumn1.ColumnName = "Otm";
            this.dataColumn1.DataType = typeof(bool);
            this.dataColumn1.DefaultValue = false;
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Начало";
            this.dataColumn2.ColumnName = "PeriodBegin";
            this.dataColumn2.DataType = typeof(System.DateTime);
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Конец";
            this.dataColumn3.ColumnName = "PeriodEnd";
            this.dataColumn3.DataType = typeof(System.DateTime);
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Имя";
            this.dataColumn4.ColumnName = "IntervalName";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Время сохранения";
            this.dataColumn5.ColumnName = "TimeChange";
            this.dataColumn5.DataType = typeof(System.DateTime);
            // 
            // BindingNavigator1
            // 
            this.BindingNavigator1.AddNewItem = null;
            this.BindingNavigator1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BindingNavigator1.BindingSource = this.BindingSource1;
            this.BindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.BindingNavigator1.CountItemFormat = "из {0}";
            this.BindingNavigator1.DeleteItem = null;
            this.BindingNavigator1.Dock = System.Windows.Forms.DockStyle.None;
            this.BindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.ButClearFilter,
            this.ButSetFilter});
            this.BindingNavigator1.Location = new System.Drawing.Point(1, 463);
            this.BindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.BindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.BindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.BindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.BindingNavigator1.Name = "BindingNavigator1";
            this.BindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.BindingNavigator1.Size = new System.Drawing.Size(278, 25);
            this.BindingNavigator1.TabIndex = 35;
            this.BindingNavigator1.Text = "BindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(36, 22);
            this.bindingNavigatorCountItem.Text = "из {0}";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ButClearFilter
            // 
            this.ButClearFilter.Image = ((System.Drawing.Image)(resources.GetObject("ButClearFilter.Image")));
            this.ButClearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButClearFilter.Name = "ButClearFilter";
            this.ButClearFilter.Size = new System.Drawing.Size(103, 22);
            this.ButClearFilter.Text = "Снять фильтр";
            this.ButClearFilter.ToolTipText = "Снять фильтр";
            this.ButClearFilter.Visible = false;
            this.ButClearFilter.Click += new System.EventHandler(this.ButClearFilter_Click);
            // 
            // ButSetFilter
            // 
            this.ButSetFilter.Enabled = false;
            this.ButSetFilter.Image = ((System.Drawing.Image)(resources.GetObject("ButSetFilter.Image")));
            this.ButSetFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButSetFilter.Name = "ButSetFilter";
            this.ButSetFilter.Size = new System.Drawing.Size(68, 22);
            this.ButSetFilter.Text = "Фильтр";
            this.ButSetFilter.Click += new System.EventHandler(this.ButSetFilter_Click);
            // 
            // butUncheckAll
            // 
            this.butUncheckAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butUncheckAll.ImageKey = "OtmAll_No.ICO";
            this.butUncheckAll.ImageList = this.imageList1;
            this.butUncheckAll.Location = new System.Drawing.Point(552, 1);
            this.butUncheckAll.Name = "butUncheckAll";
            this.butUncheckAll.Size = new System.Drawing.Size(35, 32);
            this.butUncheckAll.TabIndex = 25;
            this.butUncheckAll.UseVisualStyleBackColor = true;
            this.butUncheckAll.Click += new System.EventHandler(this.butUncheckAll_Click);
            // 
            // butCheckAll
            // 
            this.butCheckAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCheckAll.ImageKey = "OtmAll.ico";
            this.butCheckAll.ImageList = this.imageList1;
            this.butCheckAll.Location = new System.Drawing.Point(517, 1);
            this.butCheckAll.Name = "butCheckAll";
            this.butCheckAll.Size = new System.Drawing.Size(35, 32);
            this.butCheckAll.TabIndex = 20;
            this.butCheckAll.UseVisualStyleBackColor = true;
            this.butCheckAll.Click += new System.EventHandler(this.butCheckAll_Click);
            // 
            // butDeleteIntervals
            // 
            this.butDeleteIntervals.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butDeleteIntervals.ImageKey = "Delete.ico";
            this.butDeleteIntervals.ImageList = this.imageList1;
            this.butDeleteIntervals.Location = new System.Drawing.Point(587, 1);
            this.butDeleteIntervals.Name = "butDeleteIntervals";
            this.butDeleteIntervals.Size = new System.Drawing.Size(175, 32);
            this.butDeleteIntervals.TabIndex = 30;
            this.butDeleteIntervals.Text = "Удалить отмеченные";
            this.butDeleteIntervals.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butDeleteIntervals.UseVisualStyleBackColor = true;
            this.butDeleteIntervals.Click += new System.EventHandler(this.butDeleteIntervals_Click);
            // 
            // butLoadInterval
            // 
            this.butLoadInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butLoadInterval.ImageKey = "Down.ICO";
            this.butLoadInterval.ImageList = this.imageList1;
            this.butLoadInterval.Location = new System.Drawing.Point(1, 1);
            this.butLoadInterval.Name = "butLoadInterval";
            this.butLoadInterval.Size = new System.Drawing.Size(192, 32);
            this.butLoadInterval.TabIndex = 10;
            this.butLoadInterval.Text = " Загрузить выделенный";
            this.butLoadInterval.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butLoadInterval.UseVisualStyleBackColor = true;
            this.butLoadInterval.Click += new System.EventHandler(this.butLoadInterval_Click);
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            // 
            // butFilter
            // 
            this.butFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butFilter.ImageKey = "find.png";
            this.butFilter.ImageList = this.imageList1;
            this.butFilter.Location = new System.Drawing.Point(476, 1);
            this.butFilter.Name = "butFilter";
            this.butFilter.Size = new System.Drawing.Size(35, 32);
            this.butFilter.TabIndex = 15;
            this.butFilter.UseVisualStyleBackColor = true;
            this.butFilter.Click += new System.EventHandler(this.butFilter_Click);
            // 
            // Intervals
            // 
            this.Intervals.AllowUserToAddRows = false;
            this.Intervals.AllowUserToDeleteRows = false;
            this.Intervals.AllowUserToOrderColumns = true;
            this.Intervals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Intervals.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Intervals.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Intervals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Intervals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Otm,
            this.PeriodBegin,
            this.PeriodEnd,
            this.IntervalName,
            this.TimeChange});
            this.Intervals.DataSource = this.BindingSource1;
            this.Intervals.Location = new System.Drawing.Point(1, 35);
            this.Intervals.Name = "Intervals";
            this.Intervals.RowHeadersVisible = false;
            this.Intervals.RowHeadersWidth = 22;
            this.Intervals.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Intervals.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Intervals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.Intervals.Size = new System.Drawing.Size(762, 426);
            this.Intervals.TabIndex = 5;
            // 
            // Otm
            // 
            this.Otm.DataPropertyName = "Otm";
            this.Otm.HeaderText = "Отм";
            this.Otm.Name = "Otm";
            this.Otm.Width = 40;
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.DataPropertyName = "PeriodBegin";
            dataGridViewCellStyle2.Format = "dd.MM.yyyy HH:mm:ss";
            this.PeriodBegin.DefaultCellStyle = dataGridViewCellStyle2;
            this.PeriodBegin.HeaderText = "Начало";
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.ReadOnly = true;
            this.PeriodBegin.Width = 150;
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.DataPropertyName = "PeriodEnd";
            dataGridViewCellStyle3.Format = "dd.MM.yyyy HH:mm:ss";
            this.PeriodEnd.DefaultCellStyle = dataGridViewCellStyle3;
            this.PeriodEnd.HeaderText = "Конец";
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.ReadOnly = true;
            this.PeriodEnd.Width = 150;
            // 
            // IntervalName
            // 
            this.IntervalName.DataPropertyName = "IntervalName";
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.IntervalName.DefaultCellStyle = dataGridViewCellStyle4;
            this.IntervalName.HeaderText = "Имя";
            this.IntervalName.Name = "IntervalName";
            this.IntervalName.ReadOnly = true;
            this.IntervalName.Width = 250;
            // 
            // TimeChange
            // 
            this.TimeChange.DataPropertyName = "TimeChange";
            dataGridViewCellStyle5.Format = "dd.MM.yyyy HH:mm:ss";
            this.TimeChange.DefaultCellStyle = dataGridViewCellStyle5;
            this.TimeChange.HeaderText = "Время сохранения";
            this.TimeChange.Name = "TimeChange";
            this.TimeChange.ReadOnly = true;
            this.TimeChange.Width = 160;
            // 
            // progressReport
            // 
            this.progressReport.Location = new System.Drawing.Point(199, 6);
            this.progressReport.MarqueeAnimationSpeed = 50;
            this.progressReport.Name = "progressReport";
            this.progressReport.Size = new System.Drawing.Size(271, 23);
            this.progressReport.Step = 1;
            this.progressReport.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressReport.TabIndex = 36;
            // 
            // FormIntervals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 489);
            this.Controls.Add(this.progressReport);
            this.Controls.Add(this.butFilter);
            this.Controls.Add(this.butUncheckAll);
            this.Controls.Add(this.Intervals);
            this.Controls.Add(this.BindingNavigator1);
            this.Controls.Add(this.butCheckAll);
            this.Controls.Add(this.butDeleteIntervals);
            this.Controls.Add(this.butLoadInterval);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormIntervals";
            this.Text = "Журнал отчетов";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormIntervalsWin_FormClosed);
            this.Load += new System.EventHandler(this.FormIntervalsWin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingNavigator1)).EndInit();
            this.BindingNavigator1.ResumeLayout(false);
            this.BindingNavigator1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Intervals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butLoadInterval;
        private System.Windows.Forms.Button butDeleteIntervals;
        private System.Windows.Forms.Button butCheckAll;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button butUncheckAll;
        public System.Windows.Forms.BindingSource BindingSource1;
        private System.Windows.Forms.Button butFilter;
        private System.Windows.Forms.ToolStripButton ButSetFilter;
        private System.Windows.Forms.ToolStripButton ButClearFilter;
        public System.Windows.Forms.BindingNavigator BindingNavigator1;
        public System.Windows.Forms.DataGridView Intervals;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Otm;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeriodBegin;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeriodEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn IntervalName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeChange;
        public System.Windows.Forms.ProgressBar progressReport;
    }
}