namespace ReporterCommon
{
    partial class FormLinksTemplate
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLinksTemplate));
            this.TemplateGrid = new System.Windows.Forms.DataGridView();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodeForming = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CellAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CellLinkType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Props = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.butSave = new System.Windows.Forms.Button();
            this.Template = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.butDelete = new System.Windows.Forms.Button();
            this.NextCellStep = new System.Windows.Forms.ComboBox();
            this.NextCellShift = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ParamPropsPanel = new System.Windows.Forms.Panel();
            this.LinkCellAction = new System.Windows.Forms.ComboBox();
            this.LabelCellAction = new System.Windows.Forms.Label();
            this.LinkCodeForming = new System.Windows.Forms.ComboBox();
            this.LabelCodeForming = new System.Windows.Forms.Label();
            this.LinkY = new System.Windows.Forms.TextBox();
            this.LabelY = new System.Windows.Forms.Label();
            this.LinkX = new System.Windows.Forms.TextBox();
            this.LabelX = new System.Windows.Forms.Label();
            this.CellField = new System.Windows.Forms.ComboBox();
            this.LabelField = new System.Windows.Forms.Label();
            this.WriteText = new System.Windows.Forms.TextBox();
            this.ButAddLink = new System.Windows.Forms.Button();
            this.ButCreate = new System.Windows.Forms.Button();
            this.ButExport = new System.Windows.Forms.Button();
            this.LinkPropsPanel = new ReporterCommon.ControlLinkProps();
            ((System.ComponentModel.ISupportInitialize)(this.TemplateGrid)).BeginInit();
            this.ParamPropsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TemplateGrid
            // 
            this.TemplateGrid.AllowUserToAddRows = false;
            this.TemplateGrid.AllowUserToOrderColumns = true;
            this.TemplateGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TemplateGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.TemplateGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TemplateGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.X,
            this.Y,
            this.CodeForming,
            this.CellAction,
            this.Field,
            this.CellLinkType,
            this.Props});
            this.TemplateGrid.Location = new System.Drawing.Point(2, 64);
            this.TemplateGrid.MultiSelect = false;
            this.TemplateGrid.Name = "TemplateGrid";
            this.TemplateGrid.RowHeadersWidth = 12;
            this.TemplateGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.TemplateGrid.Size = new System.Drawing.Size(633, 361);
            this.TemplateGrid.TabIndex = 25;
            this.TemplateGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.TemplateGrid_CellValueChanged);
            this.TemplateGrid.SelectionChanged += new System.EventHandler(this.TemplateGrid_SelectionChanged);
            // 
            // X
            // 
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = "0";
            this.X.DefaultCellStyle = dataGridViewCellStyle1;
            this.X.HeaderText = "X";
            this.X.Name = "X";
            this.X.Width = 20;
            // 
            // Y
            // 
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = "0";
            this.Y.DefaultCellStyle = dataGridViewCellStyle2;
            this.Y.HeaderText = "Y";
            this.Y.Name = "Y";
            this.Y.Width = 20;
            // 
            // CodeForming
            // 
            this.CodeForming.HeaderText = "Формирование кода";
            this.CodeForming.Name = "CodeForming";
            this.CodeForming.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CodeForming.Width = 150;
            // 
            // CellAction
            // 
            this.CellAction.HeaderText = "Действие";
            this.CellAction.Name = "CellAction";
            this.CellAction.ReadOnly = true;
            this.CellAction.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CellAction.Width = 130;
            // 
            // Field
            // 
            this.Field.HeaderText = "Тип информации";
            this.Field.Name = "Field";
            this.Field.ReadOnly = true;
            this.Field.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Field.Width = 120;
            // 
            // CellLinkType
            // 
            this.CellLinkType.HeaderText = "Тип ссылки";
            this.CellLinkType.Name = "CellLinkType";
            this.CellLinkType.ReadOnly = true;
            this.CellLinkType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CellLinkType.Width = 175;
            // 
            // Props
            // 
            this.Props.HeaderText = "Свойства";
            this.Props.Name = "Props";
            this.Props.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Props.Visible = false;
            // 
            // butSave
            // 
            this.butSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butSave.Location = new System.Drawing.Point(476, 1);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(106, 34);
            this.butSave.TabIndex = 12;
            this.butSave.Text = "Сохранить";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // Template
            // 
            this.Template.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Template.FormattingEnabled = true;
            this.Template.Location = new System.Drawing.Point(70, 7);
            this.Template.Name = "Template";
            this.Template.Size = new System.Drawing.Size(293, 24);
            this.Template.TabIndex = 5;
            this.Template.Text = "Новый шаблон";
            this.Template.SelectedIndexChanged += new System.EventHandler(this.Template_SelectedIndexChanged);
            this.Template.TextUpdate += new System.EventHandler(this.Template_TextUpdate);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(2, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Шаблон";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butDelete
            // 
            this.butDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butDelete.Location = new System.Drawing.Point(583, 1);
            this.butDelete.Name = "butDelete";
            this.butDelete.Size = new System.Drawing.Size(106, 34);
            this.butDelete.TabIndex = 15;
            this.butDelete.Text = "Удалить";
            this.butDelete.UseVisualStyleBackColor = true;
            this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
            // 
            // NextCellStep
            // 
            this.NextCellStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NextCellStep.FormattingEnabled = true;
            this.NextCellStep.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.NextCellStep.Location = new System.Drawing.Point(414, 37);
            this.NextCellStep.Name = "NextCellStep";
            this.NextCellStep.Size = new System.Drawing.Size(42, 24);
            this.NextCellStep.TabIndex = 22;
            this.NextCellStep.Text = "1";
            // 
            // NextCellShift
            // 
            this.NextCellShift.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NextCellShift.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NextCellShift.FormattingEnabled = true;
            this.NextCellShift.Items.AddRange(new object[] {
            "Нет",
            "Вправо",
            "Вниз"});
            this.NextCellShift.Location = new System.Drawing.Point(260, 37);
            this.NextCellShift.Name = "NextCellShift";
            this.NextCellShift.Size = new System.Drawing.Size(85, 24);
            this.NextCellShift.TabIndex = 20;
            this.NextCellShift.SelectedIndexChanged += new System.EventHandler(this.NextCellShift_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(354, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 16);
            this.label3.TabIndex = 80;
            this.label3.Text = "с шагом";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(4, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 24);
            this.label2.TabIndex = 79;
            this.label2.Text = "После установки ссылок переходить ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ParamPropsPanel
            // 
            this.ParamPropsPanel.Controls.Add(this.LinkCellAction);
            this.ParamPropsPanel.Controls.Add(this.LabelCellAction);
            this.ParamPropsPanel.Controls.Add(this.LinkCodeForming);
            this.ParamPropsPanel.Controls.Add(this.LabelCodeForming);
            this.ParamPropsPanel.Controls.Add(this.LinkY);
            this.ParamPropsPanel.Controls.Add(this.LabelY);
            this.ParamPropsPanel.Controls.Add(this.LinkX);
            this.ParamPropsPanel.Controls.Add(this.LabelX);
            this.ParamPropsPanel.Controls.Add(this.CellField);
            this.ParamPropsPanel.Controls.Add(this.LabelField);
            this.ParamPropsPanel.Controls.Add(this.WriteText);
            this.ParamPropsPanel.Location = new System.Drawing.Point(638, 98);
            this.ParamPropsPanel.Name = "ParamPropsPanel";
            this.ParamPropsPanel.Size = new System.Drawing.Size(415, 136);
            this.ParamPropsPanel.TabIndex = 35;
            // 
            // LinkCellAction
            // 
            this.LinkCellAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LinkCellAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LinkCellAction.FormattingEnabled = true;
            this.LinkCellAction.Items.AddRange(new object[] {
            "Установить ссылку",
            "Записать значение",
            "Записать текст",
            "Ссылка на сохранение"});
            this.LinkCellAction.Location = new System.Drawing.Point(128, 6);
            this.LinkCellAction.Name = "LinkCellAction";
            this.LinkCellAction.Size = new System.Drawing.Size(180, 24);
            this.LinkCellAction.TabIndex = 97;
            this.LinkCellAction.SelectedIndexChanged += new System.EventHandler(this.LinkCellAction_SelectedIndexChanged_1);
            // 
            // LabelCellAction
            // 
            this.LabelCellAction.AutoSize = true;
            this.LabelCellAction.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelCellAction.Location = new System.Drawing.Point(55, 9);
            this.LabelCellAction.Name = "LabelCellAction";
            this.LabelCellAction.Size = new System.Drawing.Size(70, 16);
            this.LabelCellAction.TabIndex = 98;
            this.LabelCellAction.Text = "Действие";
            // 
            // LinkCodeForming
            // 
            this.LinkCodeForming.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LinkCodeForming.FormattingEnabled = true;
            this.LinkCodeForming.Items.AddRange(new object[] {
            "<Полный код>",
            "<Полный код>Suffix",
            "<Код параметра>",
            "<Код параметра>Suffix",
            "<Код 2 параметра>",
            "<Код 2 параметра>Suffix"});
            this.LinkCodeForming.Location = new System.Drawing.Point(7, 109);
            this.LinkCodeForming.Name = "LinkCodeForming";
            this.LinkCodeForming.Size = new System.Drawing.Size(361, 24);
            this.LinkCodeForming.TabIndex = 55;
            this.LinkCodeForming.Text = "<Полный код>";
            this.LinkCodeForming.TextUpdate += new System.EventHandler(this.LinkCodeForming_TextUpdate);
            // 
            // LabelCodeForming
            // 
            this.LabelCodeForming.AutoSize = true;
            this.LabelCodeForming.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelCodeForming.Location = new System.Drawing.Point(7, 89);
            this.LabelCodeForming.Name = "LabelCodeForming";
            this.LabelCodeForming.Size = new System.Drawing.Size(323, 16);
            this.LabelCodeForming.TabIndex = 104;
            this.LabelCodeForming.Text = "Способ формирования кода параметра ссылки";
            // 
            // LinkY
            // 
            this.LinkY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LinkY.Location = new System.Drawing.Point(260, 62);
            this.LinkY.Name = "LinkY";
            this.LinkY.Size = new System.Drawing.Size(46, 22);
            this.LinkY.TabIndex = 50;
            this.LinkY.TextChanged += new System.EventHandler(this.LinkY_TextChanged);
            // 
            // LabelY
            // 
            this.LabelY.AutoSize = true;
            this.LabelY.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelY.Location = new System.Drawing.Point(181, 65);
            this.LabelY.Name = "LabelY";
            this.LabelY.Size = new System.Drawing.Size(78, 16);
            this.LabelY.TabIndex = 102;
            this.LabelY.Text = "Сдвиг по Y";
            // 
            // LinkX
            // 
            this.LinkX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LinkX.Location = new System.Drawing.Point(128, 62);
            this.LinkX.Name = "LinkX";
            this.LinkX.Size = new System.Drawing.Size(42, 22);
            this.LinkX.TabIndex = 48;
            this.LinkX.TextChanged += new System.EventHandler(this.LinkX_TextChanged);
            // 
            // LabelX
            // 
            this.LabelX.AutoSize = true;
            this.LabelX.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelX.Location = new System.Drawing.Point(43, 65);
            this.LabelX.Name = "LabelX";
            this.LabelX.Size = new System.Drawing.Size(79, 16);
            this.LabelX.TabIndex = 100;
            this.LabelX.Text = "Сдвиг по X";
            // 
            // CellField
            // 
            this.CellField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CellField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CellField.FormattingEnabled = true;
            this.CellField.Items.AddRange(new object[] {
            "Значение",
            "Время",
            "Недостоверность",
            "Номер",
            "Код",
            "Параметр",
            "Подпараметр",
            "Имя",
            "Имя подпараметра",
            "Единицы",
            "Тип данных",
            "Накопление",
            "Проект",
            "Задача",
            "Комментарий",
            "Комментарий подпараметра",
            "Мин",
            "Макс"});
            this.CellField.Location = new System.Drawing.Point(128, 34);
            this.CellField.Name = "CellField";
            this.CellField.Size = new System.Drawing.Size(212, 24);
            this.CellField.TabIndex = 45;
            this.CellField.SelectedIndexChanged += new System.EventHandler(this.LinkField_SelectedIndexChanged);
            // 
            // LabelField
            // 
            this.LabelField.AutoSize = true;
            this.LabelField.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelField.Location = new System.Drawing.Point(4, 37);
            this.LabelField.Name = "LabelField";
            this.LabelField.Size = new System.Drawing.Size(121, 16);
            this.LabelField.TabIndex = 98;
            this.LabelField.Text = "Тип информации";
            // 
            // WriteText
            // 
            this.WriteText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WriteText.Location = new System.Drawing.Point(3, 35);
            this.WriteText.Name = "WriteText";
            this.WriteText.Size = new System.Drawing.Size(409, 22);
            this.WriteText.TabIndex = 108;
            this.WriteText.Visible = false;
            this.WriteText.TextChanged += new System.EventHandler(this.WriteText_TextChanged_1);
            // 
            // ButAddLink
            // 
            this.ButAddLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButAddLink.Location = new System.Drawing.Point(638, 64);
            this.ButAddLink.Name = "ButAddLink";
            this.ButAddLink.Size = new System.Drawing.Size(167, 34);
            this.ButAddLink.TabIndex = 30;
            this.ButAddLink.Text = "Добавить действие";
            this.ButAddLink.UseVisualStyleBackColor = true;
            this.ButAddLink.Click += new System.EventHandler(this.ButAddLink_Click);
            // 
            // ButCreate
            // 
            this.ButCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButCreate.Location = new System.Drawing.Point(369, 1);
            this.ButCreate.Name = "ButCreate";
            this.ButCreate.Size = new System.Drawing.Size(106, 34);
            this.ButCreate.TabIndex = 10;
            this.ButCreate.Text = "Создать";
            this.ButCreate.UseVisualStyleBackColor = true;
            this.ButCreate.Click += new System.EventHandler(this.ButCreate_Click);
            // 
            // ButExport
            // 
            this.ButExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButExport.Location = new System.Drawing.Point(690, 1);
            this.ButExport.Name = "ButExport";
            this.ButExport.Size = new System.Drawing.Size(106, 34);
            this.ButExport.TabIndex = 18;
            this.ButExport.Text = "Экспорт";
            this.ButExport.UseVisualStyleBackColor = true;
            this.ButExport.Click += new System.EventHandler(this.ButExport_Click);
            // 
            // LinkPropsPanel
            // 
            this.LinkPropsPanel.Location = new System.Drawing.Point(638, 236);
            this.LinkPropsPanel.Name = "LinkPropsPanel";
            this.LinkPropsPanel.Size = new System.Drawing.Size(453, 189);
            this.LinkPropsPanel.TabIndex = 60;
            this.LinkPropsPanel.OnLinkTypeChange += new System.EventHandler(this.LinkPropsPanel_OnLinkTypeChange);
            // 
            // FormLinksTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 426);
            this.Controls.Add(this.ButExport);
            this.Controls.Add(this.ButCreate);
            this.Controls.Add(this.ButAddLink);
            this.Controls.Add(this.ParamPropsPanel);
            this.Controls.Add(this.LinkPropsPanel);
            this.Controls.Add(this.NextCellStep);
            this.Controls.Add(this.NextCellShift);
            this.Controls.Add(this.butDelete);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Template);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.TemplateGrid);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormLinksTemplate";
            this.Text = "Изменение шаблона установки ссылок";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLinksTemplate_FormClosing);
            this.Load += new System.EventHandler(this.FormLinksTemplate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TemplateGrid)).EndInit();
            this.ParamPropsPanel.ResumeLayout(false);
            this.ParamPropsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView TemplateGrid;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.ComboBox Template;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button butDelete;
        private System.Windows.Forms.ComboBox NextCellStep;
        private System.Windows.Forms.ComboBox NextCellShift;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public ControlLinkProps LinkPropsPanel;
        private System.Windows.Forms.Panel ParamPropsPanel;
        private System.Windows.Forms.TextBox LinkY;
        private System.Windows.Forms.Label LabelY;
        private System.Windows.Forms.TextBox LinkX;
        private System.Windows.Forms.Label LabelX;
        private System.Windows.Forms.ComboBox CellField;
        private System.Windows.Forms.Label LabelField;
        private System.Windows.Forms.Button ButAddLink;
        private System.Windows.Forms.ComboBox LinkCodeForming;
        private System.Windows.Forms.Label LabelCodeForming;
        private System.Windows.Forms.Button ButCreate;
        private System.Windows.Forms.Button ButExport;
        private System.Windows.Forms.ComboBox LinkCellAction;
        private System.Windows.Forms.Label LabelCellAction;
        private System.Windows.Forms.TextBox WriteText;
        private System.Windows.Forms.DataGridViewTextBoxColumn X;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeForming;
        private System.Windows.Forms.DataGridViewTextBoxColumn CellAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field;
        private System.Windows.Forms.DataGridViewTextBoxColumn CellLinkType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Props;

    }
}