namespace ReporterCommon
{
    partial class FormPutLinks
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPutLinks));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CurrentLink = new System.Windows.Forms.TextBox();
            this.IsLoadFromCell = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Project = new System.Windows.Forms.ComboBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.Template = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ButTemplates = new System.Windows.Forms.Button();
            this.Params = new System.Windows.Forms.DataGridView();
            this.Otm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PutValue = new System.Windows.Forms.DataGridViewButtonColumn();
            this.PutTime = new System.Windows.Forms.DataGridViewButtonColumn();
            this.PutTemplate = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Task = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Units = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SuperProcessType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CalcParamType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Max = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataSet1 = new System.Data.DataSet();
            this.ParamsTable = new System.Data.DataTable();
            this.ParamOtm = new System.Data.DataColumn();
            this.ParamCode = new System.Data.DataColumn();
            this.ParamName = new System.Data.DataColumn();
            this.ParamTask = new System.Data.DataColumn();
            this.ParamDataType = new System.Data.DataColumn();
            this.ParamUnits = new System.Data.DataColumn();
            this.ParamSuperProcess = new System.Data.DataColumn();
            this.ParamCalcParamType = new System.Data.DataColumn();
            this.ParamMin = new System.Data.DataColumn();
            this.ParamMax = new System.Data.DataColumn();
            this.ParamComment = new System.Data.DataColumn();
            this.ButRedo = new System.Windows.Forms.Button();
            this.ButUndo = new System.Windows.Forms.Button();
            this.ButUpdate = new System.Windows.Forms.Button();
            this.ButFilter = new System.Windows.Forms.Button();
            this.ButDeleteLinks = new System.Windows.Forms.Button();
            this.ButSave = new System.Windows.Forms.Button();
            this.ButFindLinks = new System.Windows.Forms.Button();
            this.panelSingleLink = new System.Windows.Forms.Panel();
            this.NextCellShift = new System.Windows.Forms.ComboBox();
            this.ButLinkSave = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.CellComment = new System.Windows.Forms.TextBox();
            this.ButLinkProps = new System.Windows.Forms.Button();
            this.CellLinkType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NextCellStep = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ButOtmLinks = new System.Windows.Forms.Button();
            this.ButOtmTrue = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.ButOtmFalse = new System.Windows.Forms.Button();
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
            this.ButLoadLink = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Params)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ParamsTable)).BeginInit();
            this.panelSingleLink.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BindingNavigator1)).BeginInit();
            this.BindingNavigator1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CurrentLink
            // 
            this.CurrentLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentLink.BackColor = System.Drawing.SystemColors.Control;
            this.CurrentLink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CurrentLink.Font = new System.Drawing.Font("Arial", 9.75F);
            this.CurrentLink.Location = new System.Drawing.Point(1, 0);
            this.CurrentLink.Multiline = true;
            this.CurrentLink.Name = "CurrentLink";
            this.CurrentLink.ReadOnly = true;
            this.CurrentLink.Size = new System.Drawing.Size(733, 37);
            this.CurrentLink.TabIndex = 5;
            // 
            // IsLoadFromCell
            // 
            this.IsLoadFromCell.Checked = true;
            this.IsLoadFromCell.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IsLoadFromCell.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IsLoadFromCell.Location = new System.Drawing.Point(204, 38);
            this.IsLoadFromCell.Margin = new System.Windows.Forms.Padding(2);
            this.IsLoadFromCell.Name = "IsLoadFromCell";
            this.IsLoadFromCell.Size = new System.Drawing.Size(221, 38);
            this.IsLoadFromCell.TabIndex = 20;
            this.IsLoadFromCell.Text = "Переходить к параметру при выборе ячейки";
            this.IsLoadFromCell.UseVisualStyleBackColor = true;
            this.IsLoadFromCell.CheckedChanged += new System.EventHandler(this.IsLoadFromCell_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(3, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(195, 16);
            this.label4.TabIndex = 82;
            this.label4.Text = "Список параметров (проект)";
            // 
            // Project
            // 
            this.Project.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Project.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Project.FormattingEnabled = true;
            this.Project.Location = new System.Drawing.Point(204, 79);
            this.Project.Name = "Project";
            this.Project.Size = new System.Drawing.Size(530, 24);
            this.Project.TabIndex = 30;
            this.Project.SelectedIndexChanged += new System.EventHandler(this.Project_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Filter");
            this.imageList1.Images.SetKeyName(1, "02732.ico");
            this.imageList1.Images.SetKeyName(2, "02731.ico");
            this.imageList1.Images.SetKeyName(3, "arrow_refresh_small.png");
            this.imageList1.Images.SetKeyName(4, "Delete.ico");
            this.imageList1.Images.SetKeyName(5, "52.ico");
            this.imageList1.Images.SetKeyName(6, "arrow-curve-180-left.png");
            this.imageList1.Images.SetKeyName(7, "arrow-curve.png");
            this.imageList1.Images.SetKeyName(8, "filter_delete.png");
            this.imageList1.Images.SetKeyName(9, "AcceptTask.ico");
            this.imageList1.Images.SetKeyName(10, "AdpManageIndexes.ico");
            this.imageList1.Images.SetKeyName(11, "Filter.ico");
            this.imageList1.Images.SetKeyName(12, "FPAggregateDataSource.ico");
            this.imageList1.Images.SetKeyName(13, "Redo.ico");
            this.imageList1.Images.SetKeyName(14, "SharePointListRefreshSchema.ico");
            this.imageList1.Images.SetKeyName(15, "TableSharePointListsRefreshList.ico");
            this.imageList1.Images.SetKeyName(16, "Undo.ico");
            this.imageList1.Images.SetKeyName(17, "disk.png");
            this.imageList1.Images.SetKeyName(18, "table_save.png");
            this.imageList1.Images.SetKeyName(19, "update.png");
            this.imageList1.Images.SetKeyName(20, "find.png");
            this.imageList1.Images.SetKeyName(21, "17.ico");
            this.imageList1.Images.SetKeyName(22, "arrow-turn-180-left.png");
            this.imageList1.Images.SetKeyName(23, "arrow-turn.png");
            this.imageList1.Images.SetKeyName(24, "red_line.png");
            this.imageList1.Images.SetKeyName(25, "Redo-icon.png");
            this.imageList1.Images.SetKeyName(26, "Undo-icon.png");
            this.imageList1.Images.SetKeyName(27, "data_table.png");
            // 
            // Template
            // 
            this.Template.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Template.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Template.FormattingEnabled = true;
            this.Template.Location = new System.Drawing.Point(132, 163);
            this.Template.Name = "Template";
            this.Template.Size = new System.Drawing.Size(303, 24);
            this.Template.TabIndex = 60;
            this.Template.SelectedIndexChanged += new System.EventHandler(this.Template_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(68, 166);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 88;
            this.label5.Text = "Шаблон:";
            // 
            // ButTemplates
            // 
            this.ButTemplates.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButTemplates.ImageIndex = 27;
            this.ButTemplates.ImageList = this.imageList1;
            this.ButTemplates.Location = new System.Drawing.Point(436, 158);
            this.ButTemplates.Name = "ButTemplates";
            this.ButTemplates.Size = new System.Drawing.Size(182, 32);
            this.ButTemplates.TabIndex = 65;
            this.ButTemplates.Text = " Редактор шаблонов";
            this.ButTemplates.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButTemplates.UseVisualStyleBackColor = true;
            this.ButTemplates.Click += new System.EventHandler(this.ButTemplates_Click);
            // 
            // Params
            // 
            this.Params.AllowUserToAddRows = false;
            this.Params.AllowUserToDeleteRows = false;
            this.Params.AllowUserToOrderColumns = true;
            this.Params.AllowUserToResizeRows = false;
            this.Params.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Params.AutoGenerateColumns = false;
            this.Params.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Params.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Params.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Otm,
            this.PutValue,
            this.PutTime,
            this.PutTemplate,
            this.Code,
            this.ParName,
            this.Task,
            this.DataType,
            this.Units,
            this.SuperProcessType,
            this.CalcParamType,
            this.Min,
            this.Max,
            this.Comment});
            this.Params.DataSource = this.bindingSource1;
            this.Params.Location = new System.Drawing.Point(0, 191);
            this.Params.MultiSelect = false;
            this.Params.Name = "Params";
            this.Params.RowHeadersVisible = false;
            this.Params.RowTemplate.Height = 20;
            this.Params.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.Params.Size = new System.Drawing.Size(862, 348);
            this.Params.TabIndex = 75;
            this.Params.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Params_CellContentClick);
            this.Params.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Params_CellMouseDown);
            this.Params.SelectionChanged += new System.EventHandler(this.Params_SelectionChanged);
            // 
            // Otm
            // 
            this.Otm.DataPropertyName = "ParamOtm";
            this.Otm.HeaderText = "Отм";
            this.Otm.Name = "Otm";
            this.Otm.Width = 40;
            // 
            // PutValue
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Green;
            this.PutValue.DefaultCellStyle = dataGridViewCellStyle2;
            this.PutValue.HeaderText = "";
            this.PutValue.Name = "PutValue";
            this.PutValue.ReadOnly = true;
            this.PutValue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PutValue.Text = "Зн";
            this.PutValue.UseColumnTextForButtonValue = true;
            this.PutValue.Width = 30;
            // 
            // PutTime
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.PutTime.DefaultCellStyle = dataGridViewCellStyle3;
            this.PutTime.HeaderText = "";
            this.PutTime.Name = "PutTime";
            this.PutTime.ReadOnly = true;
            this.PutTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PutTime.Text = "Вр";
            this.PutTime.UseColumnTextForButtonValue = true;
            this.PutTime.Width = 30;
            // 
            // PutTemplate
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Olive;
            this.PutTemplate.DefaultCellStyle = dataGridViewCellStyle4;
            this.PutTemplate.HeaderText = "";
            this.PutTemplate.Name = "PutTemplate";
            this.PutTemplate.ReadOnly = true;
            this.PutTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PutTemplate.Text = "Ш";
            this.PutTemplate.UseColumnTextForButtonValue = true;
            this.PutTemplate.Width = 30;
            // 
            // Code
            // 
            this.Code.DataPropertyName = "ParamCode";
            this.Code.HeaderText = "Код";
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            this.Code.Width = 150;
            // 
            // ParName
            // 
            this.ParName.DataPropertyName = "ParamName";
            this.ParName.HeaderText = "Имя";
            this.ParName.Name = "ParName";
            this.ParName.ReadOnly = true;
            this.ParName.Width = 230;
            // 
            // Task
            // 
            this.Task.DataPropertyName = "ParamTask";
            this.Task.HeaderText = "Задача";
            this.Task.Name = "Task";
            this.Task.ReadOnly = true;
            this.Task.Width = 150;
            // 
            // DataType
            // 
            this.DataType.DataPropertyName = "ParamDataType";
            this.DataType.HeaderText = "Тип";
            this.DataType.Name = "DataType";
            this.DataType.ReadOnly = true;
            this.DataType.Width = 60;
            // 
            // Units
            // 
            this.Units.DataPropertyName = "ParamUnits";
            this.Units.HeaderText = "Ед.изм.";
            this.Units.Name = "Units";
            this.Units.ReadOnly = true;
            this.Units.Width = 60;
            // 
            // SuperProcessType
            // 
            this.SuperProcessType.DataPropertyName = "ParamSuperProcess";
            this.SuperProcessType.HeaderText = "Накопление";
            this.SuperProcessType.Name = "SuperProcessType";
            this.SuperProcessType.ReadOnly = true;
            this.SuperProcessType.Width = 80;
            // 
            // CalcParamType
            // 
            this.CalcParamType.DataPropertyName = "ParamCalcParamType";
            this.CalcParamType.HeaderText = "Ввод";
            this.CalcParamType.Name = "CalcParamType";
            this.CalcParamType.ReadOnly = true;
            this.CalcParamType.Width = 50;
            // 
            // Min
            // 
            this.Min.DataPropertyName = "ParamMin";
            this.Min.HeaderText = "Мин";
            this.Min.Name = "Min";
            this.Min.ReadOnly = true;
            this.Min.Width = 40;
            // 
            // Max
            // 
            this.Max.DataPropertyName = "ParamMax";
            this.Max.HeaderText = "Макс";
            this.Max.Name = "Max";
            this.Max.ReadOnly = true;
            this.Max.Width = 40;
            // 
            // Comment
            // 
            this.Comment.DataPropertyName = "ParamComment";
            this.Comment.HeaderText = "Комментарий";
            this.Comment.Name = "Comment";
            this.Comment.ReadOnly = true;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataMember = "ParamsTable";
            this.bindingSource1.DataSource = this.dataSet1;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.ParamsTable});
            // 
            // ParamsTable
            // 
            this.ParamsTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.ParamOtm,
            this.ParamCode,
            this.ParamName,
            this.ParamTask,
            this.ParamDataType,
            this.ParamUnits,
            this.ParamSuperProcess,
            this.ParamCalcParamType,
            this.ParamMin,
            this.ParamMax,
            this.ParamComment});
            this.ParamsTable.TableName = "ParamsTable";
            // 
            // ParamOtm
            // 
            this.ParamOtm.ColumnName = "ParamOtm";
            this.ParamOtm.DataType = typeof(bool);
            // 
            // ParamCode
            // 
            this.ParamCode.ColumnName = "ParamCode";
            // 
            // ParamName
            // 
            this.ParamName.ColumnName = "ParamName";
            // 
            // ParamTask
            // 
            this.ParamTask.ColumnName = "ParamTask";
            // 
            // ParamDataType
            // 
            this.ParamDataType.ColumnName = "ParamDataType";
            // 
            // ParamUnits
            // 
            this.ParamUnits.ColumnName = "ParamUnits";
            // 
            // ParamSuperProcess
            // 
            this.ParamSuperProcess.ColumnName = "ParamSuperProcess";
            // 
            // ParamCalcParamType
            // 
            this.ParamCalcParamType.ColumnName = "ParamCalcParamType";
            // 
            // ParamMin
            // 
            this.ParamMin.ColumnName = "ParamMin";
            // 
            // ParamMax
            // 
            this.ParamMax.ColumnName = "ParamMax";
            // 
            // ParamComment
            // 
            this.ParamComment.ColumnName = "ParamComment";
            // 
            // ButRedo
            // 
            this.ButRedo.Enabled = false;
            this.ButRedo.ImageKey = "Redo-icon.png";
            this.ButRedo.ImageList = this.imageList1;
            this.ButRedo.Location = new System.Drawing.Point(45, 38);
            this.ButRedo.Name = "ButRedo";
            this.ButRedo.Size = new System.Drawing.Size(40, 38);
            this.ButRedo.TabIndex = 12;
            this.ButRedo.UseVisualStyleBackColor = true;
            this.ButRedo.Click += new System.EventHandler(this.ButRedo_Click);
            // 
            // ButUndo
            // 
            this.ButUndo.Enabled = false;
            this.ButUndo.ImageIndex = 26;
            this.ButUndo.ImageList = this.imageList1;
            this.ButUndo.Location = new System.Drawing.Point(2, 38);
            this.ButUndo.Name = "ButUndo";
            this.ButUndo.Size = new System.Drawing.Size(40, 38);
            this.ButUndo.TabIndex = 10;
            this.ButUndo.UseVisualStyleBackColor = true;
            this.ButUndo.Click += new System.EventHandler(this.ButUndo_Click);
            // 
            // ButUpdate
            // 
            this.ButUpdate.ImageKey = "52.ico";
            this.ButUpdate.ImageList = this.imageList1;
            this.ButUpdate.Location = new System.Drawing.Point(88, 38);
            this.ButUpdate.Name = "ButUpdate";
            this.ButUpdate.Size = new System.Drawing.Size(40, 38);
            this.ButUpdate.TabIndex = 15;
            this.ButUpdate.UseVisualStyleBackColor = true;
            this.ButUpdate.Click += new System.EventHandler(this.ButUpdate_Click);
            // 
            // ButFilter
            // 
            this.ButFilter.ImageKey = "find.png";
            this.ButFilter.ImageList = this.imageList1;
            this.ButFilter.Location = new System.Drawing.Point(130, 38);
            this.ButFilter.Name = "ButFilter";
            this.ButFilter.Size = new System.Drawing.Size(40, 38);
            this.ButFilter.TabIndex = 17;
            this.ButFilter.UseVisualStyleBackColor = true;
            this.ButFilter.Click += new System.EventHandler(this.ButFilter_Click);
            // 
            // ButDeleteLinks
            // 
            this.ButDeleteLinks.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButDeleteLinks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButDeleteLinks.ImageKey = "Delete.ico";
            this.ButDeleteLinks.ImageList = this.imageList1;
            this.ButDeleteLinks.Location = new System.Drawing.Point(503, 37);
            this.ButDeleteLinks.Name = "ButDeleteLinks";
            this.ButDeleteLinks.Size = new System.Drawing.Size(107, 42);
            this.ButDeleteLinks.TabIndex = 22;
            this.ButDeleteLinks.Text = "Удалить ссылки";
            this.ButDeleteLinks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButDeleteLinks.UseVisualStyleBackColor = true;
            this.ButDeleteLinks.Click += new System.EventHandler(this.ButDeleteLinks_Click);
            // 
            // ButSave
            // 
            this.ButSave.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButSave.ImageKey = "disk.png";
            this.ButSave.ImageList = this.imageList1;
            this.ButSave.Location = new System.Drawing.Point(735, 37);
            this.ButSave.Name = "ButSave";
            this.ButSave.Size = new System.Drawing.Size(127, 42);
            this.ButSave.TabIndex = 27;
            this.ButSave.Text = "Сохранить отчет";
            this.ButSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButSave.UseVisualStyleBackColor = true;
            this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
            // 
            // ButFindLinks
            // 
            this.ButFindLinks.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButFindLinks.ImageIndex = 21;
            this.ButFindLinks.ImageList = this.imageList1;
            this.ButFindLinks.Location = new System.Drawing.Point(611, 37);
            this.ButFindLinks.Name = "ButFindLinks";
            this.ButFindLinks.Size = new System.Drawing.Size(123, 42);
            this.ButFindLinks.TabIndex = 25;
            this.ButFindLinks.Text = "Найти ссылки на параметр";
            this.ButFindLinks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButFindLinks.UseVisualStyleBackColor = true;
            this.ButFindLinks.Click += new System.EventHandler(this.ButFindLinks_Click);
            // 
            // panelSingleLink
            // 
            this.panelSingleLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSingleLink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSingleLink.Controls.Add(this.NextCellShift);
            this.panelSingleLink.Controls.Add(this.ButLinkSave);
            this.panelSingleLink.Controls.Add(this.label6);
            this.panelSingleLink.Controls.Add(this.CellComment);
            this.panelSingleLink.Controls.Add(this.ButLinkProps);
            this.panelSingleLink.Controls.Add(this.CellLinkType);
            this.panelSingleLink.Controls.Add(this.label1);
            this.panelSingleLink.Controls.Add(this.NextCellStep);
            this.panelSingleLink.Controls.Add(this.label3);
            this.panelSingleLink.Controls.Add(this.label2);
            this.panelSingleLink.Location = new System.Drawing.Point(-1, 105);
            this.panelSingleLink.Name = "panelSingleLink";
            this.panelSingleLink.Size = new System.Drawing.Size(871, 52);
            this.panelSingleLink.TabIndex = 31;
            // 
            // NextCellShift
            // 
            this.NextCellShift.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NextCellShift.Font = new System.Drawing.Font("Arial", 9.75F);
            this.NextCellShift.FormattingEnabled = true;
            this.NextCellShift.Items.AddRange(new object[] {
            "Нет",
            "Вправо",
            "Вниз"});
            this.NextCellShift.Location = new System.Drawing.Point(531, 14);
            this.NextCellShift.Name = "NextCellShift";
            this.NextCellShift.Size = new System.Drawing.Size(85, 24);
            this.NextCellShift.TabIndex = 45;
            this.NextCellShift.SelectedIndexChanged += new System.EventHandler(this.NextCellShift_SelectedIndexChanged);
            // 
            // ButLinkSave
            // 
            this.ButLinkSave.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButLinkSave.ImageKey = "table_save.png";
            this.ButLinkSave.ImageList = this.imageList1;
            this.ButLinkSave.Location = new System.Drawing.Point(735, 3);
            this.ButLinkSave.Name = "ButLinkSave";
            this.ButLinkSave.Size = new System.Drawing.Size(127, 45);
            this.ButLinkSave.TabIndex = 50;
            this.ButLinkSave.Text = "  Ссылка на сохранение";
            this.ButLinkSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButLinkSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButLinkSave.UseVisualStyleBackColor = true;
            this.ButLinkSave.Click += new System.EventHandler(this.ButLinkSave_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label6.Location = new System.Drawing.Point(3, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 16);
            this.label6.TabIndex = 106;
            this.label6.Text = "Примечание";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CellComment
            // 
            this.CellComment.Font = new System.Drawing.Font("Arial", 9.75F);
            this.CellComment.Location = new System.Drawing.Point(89, 26);
            this.CellComment.Name = "CellComment";
            this.CellComment.Size = new System.Drawing.Size(219, 22);
            this.CellComment.TabIndex = 35;
            this.CellComment.TextChanged += new System.EventHandler(this.CellComment_TextChanged);
            // 
            // ButLinkProps
            // 
            this.ButLinkProps.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButLinkProps.Location = new System.Drawing.Point(308, 2);
            this.ButLinkProps.Name = "ButLinkProps";
            this.ButLinkProps.Size = new System.Drawing.Size(84, 46);
            this.ButLinkProps.TabIndex = 40;
            this.ButLinkProps.Text = "Свойства ссылки";
            this.ButLinkProps.UseVisualStyleBackColor = true;
            this.ButLinkProps.Click += new System.EventHandler(this.ButLinkProps_Click);
            // 
            // CellLinkType
            // 
            this.CellLinkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CellLinkType.Font = new System.Drawing.Font("Arial", 9.75F);
            this.CellLinkType.FormattingEnabled = true;
            this.CellLinkType.Items.AddRange(new object[] {
            "Итоговое значение",
            "Комбинированное значение",
            "Абсолютное значение",
            "Абсолютное с редактированием",
            "Равномерный список значений",
            "Список мгновенных значений",
            "Абсолютное комбинированное",
            "Список абсолютных значений",
            "Ручной ввод",
            "Сохранение в архив",
            "Системная ссылка"});
            this.CellLinkType.Location = new System.Drawing.Point(89, 3);
            this.CellLinkType.Name = "CellLinkType";
            this.CellLinkType.Size = new System.Drawing.Size(219, 24);
            this.CellLinkType.TabIndex = 32;
            this.CellLinkType.SelectedIndexChanged += new System.EventHandler(this.CellLinkType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 16);
            this.label1.TabIndex = 102;
            this.label1.Text = "Тип ссылки";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NextCellStep
            // 
            this.NextCellStep.Font = new System.Drawing.Font("Arial", 9.75F);
            this.NextCellStep.FormattingEnabled = true;
            this.NextCellStep.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.NextCellStep.Location = new System.Drawing.Point(684, 14);
            this.NextCellStep.Name = "NextCellStep";
            this.NextCellStep.Size = new System.Drawing.Size(42, 24);
            this.NextCellStep.TabIndex = 47;
            this.NextCellStep.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(622, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 16);
            this.label3.TabIndex = 99;
            this.label3.Text = "с шагом";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(398, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 34);
            this.label2.TabIndex = 98;
            this.label2.Text = "После установки ссылки переходить ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ButOtmLinks
            // 
            this.ButOtmLinks.Font = new System.Drawing.Font("Arial", 9.75F);
            this.ButOtmLinks.ImageIndex = 24;
            this.ButOtmLinks.ImageList = this.imageList1;
            this.ButOtmLinks.Location = new System.Drawing.Point(684, 158);
            this.ButOtmLinks.Name = "ButOtmLinks";
            this.ButOtmLinks.Size = new System.Drawing.Size(178, 32);
            this.ButOtmLinks.TabIndex = 70;
            this.ButOtmLinks.Text = " Ссылки по отметкам";
            this.ButOtmLinks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButOtmLinks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButOtmLinks.UseVisualStyleBackColor = true;
            this.ButOtmLinks.Click += new System.EventHandler(this.butOtmLinks_Click);
            // 
            // ButOtmTrue
            // 
            this.ButOtmTrue.ImageIndex = 0;
            this.ButOtmTrue.ImageList = this.imageList2;
            this.ButOtmTrue.Location = new System.Drawing.Point(1, 161);
            this.ButOtmTrue.Name = "ButOtmTrue";
            this.ButOtmTrue.Size = new System.Drawing.Size(30, 28);
            this.ButOtmTrue.TabIndex = 55;
            this.ButOtmTrue.UseVisualStyleBackColor = true;
            this.ButOtmTrue.Click += new System.EventHandler(this.butOtmTrue_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "OtmAll.ico");
            this.imageList2.Images.SetKeyName(1, "OtmAll_No.ICO");
            // 
            // ButOtmFalse
            // 
            this.ButOtmFalse.ImageIndex = 1;
            this.ButOtmFalse.ImageList = this.imageList2;
            this.ButOtmFalse.Location = new System.Drawing.Point(32, 161);
            this.ButOtmFalse.Name = "ButOtmFalse";
            this.ButOtmFalse.Size = new System.Drawing.Size(30, 28);
            this.ButOtmFalse.TabIndex = 57;
            this.ButOtmFalse.UseVisualStyleBackColor = true;
            this.ButOtmFalse.Click += new System.EventHandler(this.butOtmFalse_Click);
            // 
            // BindingNavigator1
            // 
            this.BindingNavigator1.AddNewItem = null;
            this.BindingNavigator1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BindingNavigator1.BindingSource = this.bindingSource1;
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
            this.BindingNavigator1.Location = new System.Drawing.Point(0, 539);
            this.BindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.BindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.BindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.BindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.BindingNavigator1.Name = "BindingNavigator1";
            this.BindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.BindingNavigator1.Size = new System.Drawing.Size(278, 25);
            this.BindingNavigator1.TabIndex = 80;
            this.BindingNavigator1.Text = "bindingNavigator1";
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
            // ButLoadLink
            // 
            this.ButLoadLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButLoadLink.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButLoadLink.Location = new System.Drawing.Point(735, 0);
            this.ButLoadLink.Name = "ButLoadLink";
            this.ButLoadLink.Size = new System.Drawing.Size(127, 37);
            this.ButLoadLink.TabIndex = 89;
            this.ButLoadLink.Text = "Загрузить ссылку";
            this.ButLoadLink.UseVisualStyleBackColor = true;
            this.ButLoadLink.Click += new System.EventHandler(this.ButLoadLink_Click);
            // 
            // FormPutLinks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 563);
            this.Controls.Add(this.ButLoadLink);
            this.Controls.Add(this.BindingNavigator1);
            this.Controls.Add(this.ButOtmFalse);
            this.Controls.Add(this.ButOtmTrue);
            this.Controls.Add(this.ButOtmLinks);
            this.Controls.Add(this.panelSingleLink);
            this.Controls.Add(this.ButFindLinks);
            this.Controls.Add(this.ButSave);
            this.Controls.Add(this.Params);
            this.Controls.Add(this.Template);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ButTemplates);
            this.Controls.Add(this.ButRedo);
            this.Controls.Add(this.Project);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ButUndo);
            this.Controls.Add(this.ButUpdate);
            this.Controls.Add(this.ButFilter);
            this.Controls.Add(this.CurrentLink);
            this.Controls.Add(this.ButDeleteLinks);
            this.Controls.Add(this.IsLoadFromCell);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormPutLinks";
            this.Text = "Редактирование ссылок";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPutLinks_FormClosing);
            this.Load += new System.EventHandler(this.FormPutLinks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Params)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ParamsTable)).EndInit();
            this.panelSingleLink.ResumeLayout(false);
            this.panelSingleLink.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BindingNavigator1)).EndInit();
            this.BindingNavigator1.ResumeLayout(false);
            this.BindingNavigator1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButDeleteLinks;
        private System.Windows.Forms.TextBox CurrentLink;
        private System.Windows.Forms.CheckBox IsLoadFromCell;
        private System.Windows.Forms.Button ButFilter;
        private System.Windows.Forms.Button ButUpdate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox Project;
        private System.Windows.Forms.ImageList imageList1;
        public System.Windows.Forms.ComboBox Template;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ButTemplates;
        public System.Windows.Forms.DataGridView Params;
        private System.Windows.Forms.Button ButSave;
        private System.Windows.Forms.Button ButFindLinks;
        private System.Windows.Forms.Panel panelSingleLink;
        private System.Windows.Forms.ComboBox NextCellShift;
        private System.Windows.Forms.Button ButLinkSave;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox CellComment;
        private System.Windows.Forms.Button ButLinkProps;
        public System.Windows.Forms.ComboBox CellLinkType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox NextCellStep;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ButOtmLinks;
        private System.Windows.Forms.Button ButOtmTrue;
        private System.Windows.Forms.Button ButOtmFalse;
        private System.Windows.Forms.ImageList imageList2;
        public System.Windows.Forms.Button ButUndo;
        public System.Windows.Forms.Button ButRedo;
        private System.Windows.Forms.BindingSource bindingSource1;
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
        private System.Data.DataTable ParamsTable;
        private System.Data.DataColumn ParamOtm;
        private System.Data.DataColumn ParamCode;
        private System.Data.DataColumn ParamName;
        private System.Data.DataColumn ParamTask;
        private System.Data.DataColumn ParamDataType;
        private System.Data.DataColumn ParamUnits;
        private System.Data.DataColumn ParamSuperProcess;
        private System.Data.DataColumn ParamCalcParamType;
        private System.Data.DataColumn ParamMin;
        private System.Data.DataColumn ParamMax;
        private System.Data.DataColumn ParamComment;
        private System.Windows.Forms.ToolStripButton ButClearFilter;
        public System.Windows.Forms.BindingNavigator BindingNavigator1;
        private System.Windows.Forms.ToolStripButton ButSetFilter;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Otm;
        private System.Windows.Forms.DataGridViewButtonColumn PutValue;
        private System.Windows.Forms.DataGridViewButtonColumn PutTime;
        private System.Windows.Forms.DataGridViewButtonColumn PutTemplate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Task;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Units;
        private System.Windows.Forms.DataGridViewTextBoxColumn SuperProcessType;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalcParamType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Min;
        private System.Windows.Forms.DataGridViewTextBoxColumn Max;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.Button ButLoadLink;
    }
}