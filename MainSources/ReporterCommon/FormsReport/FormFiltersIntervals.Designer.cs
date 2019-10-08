namespace ReporterCommon
{
    partial class FormFiltersIntervals
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFiltersIntervals));
            this.FilterOtm = new System.Windows.Forms.ComboBox();
            this.RelationOtm = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ButClearFilters = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.FilterName = new System.Windows.Forms.TextBox();
            this.RelationName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FilterBegin1 = new System.Windows.Forms.TextBox();
            this.RelationBegin1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterBegin2 = new System.Windows.Forms.TextBox();
            this.RelationBegin2 = new System.Windows.Forms.ComboBox();
            this.FilterEnd2 = new System.Windows.Forms.TextBox();
            this.RelationEnd2 = new System.Windows.Forms.ComboBox();
            this.FilterEnd1 = new System.Windows.Forms.TextBox();
            this.RelationEnd1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.FilterTimeChange2 = new System.Windows.Forms.TextBox();
            this.RelationTimeChange2 = new System.Windows.Forms.ComboBox();
            this.FilterTimeChange1 = new System.Windows.Forms.TextBox();
            this.RelationTimeChange1 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButClearFilter = new System.Windows.Forms.Button();
            this.ButFilter = new System.Windows.Forms.Button();
            this.ButFind = new System.Windows.Forms.Button();
            this.PickerBegin1 = new System.Windows.Forms.DateTimePicker();
            this.PickerBegin2 = new System.Windows.Forms.DateTimePicker();
            this.PickerEnd1 = new System.Windows.Forms.DateTimePicker();
            this.PickerEnd2 = new System.Windows.Forms.DateTimePicker();
            this.PickerTimeChange1 = new System.Windows.Forms.DateTimePicker();
            this.PickerTimeChange2 = new System.Windows.Forms.DateTimePicker();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilterOtm
            // 
            this.FilterOtm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterOtm.FormattingEnabled = true;
            this.FilterOtm.Items.AddRange(new object[] {
            "Вкл",
            "Откл"});
            this.FilterOtm.Location = new System.Drawing.Point(189, 35);
            this.FilterOtm.Name = "FilterOtm";
            this.FilterOtm.Size = new System.Drawing.Size(142, 21);
            this.FilterOtm.TabIndex = 10;
            // 
            // RelationOtm
            // 
            this.RelationOtm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationOtm.FormattingEnabled = true;
            this.RelationOtm.Items.AddRange(new object[] {
            "Равно"});
            this.RelationOtm.Location = new System.Drawing.Point(70, 35);
            this.RelationOtm.Name = "RelationOtm";
            this.RelationOtm.Size = new System.Drawing.Size(120, 21);
            this.RelationOtm.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Отметка";
            // 
            // ButClearFilters
            // 
            this.ButClearFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButClearFilters.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButClearFilters.ImageKey = "cross.png";
            this.ButClearFilters.ImageList = this.imageList2;
            this.ButClearFilters.Location = new System.Drawing.Point(189, 1);
            this.ButClearFilters.Name = "ButClearFilters";
            this.ButClearFilters.Size = new System.Drawing.Size(143, 33);
            this.ButClearFilters.TabIndex = 110;
            this.ButClearFilters.Text = " Очистить бланк";
            this.ButClearFilters.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButClearFilters.UseVisualStyleBackColor = true;
            this.ButClearFilters.Click += new System.EventHandler(this.ButClearFilters_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "cross.png");
            this.imageList2.Images.SetKeyName(1, "filter.png");
            this.imageList2.Images.SetKeyName(2, "filter_delete.png");
            this.imageList2.Images.SetKeyName(3, "find.png");
            // 
            // FilterName
            // 
            this.FilterName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterName.Location = new System.Drawing.Point(189, 56);
            this.FilterName.Name = "FilterName";
            this.FilterName.Size = new System.Drawing.Size(142, 20);
            this.FilterName.TabIndex = 20;
            // 
            // RelationName
            // 
            this.RelationName.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationName.FormattingEnabled = true;
            this.RelationName.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Начинается с",
            "Кончается на",
            "Пустое",
            "Не пустое"});
            this.RelationName.Location = new System.Drawing.Point(70, 56);
            this.RelationName.Name = "RelationName";
            this.RelationName.Size = new System.Drawing.Size(120, 21);
            this.RelationName.TabIndex = 15;
            this.RelationName.SelectedIndexChanged += new System.EventHandler(this.RelationName_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Имя";
            // 
            // FilterBegin1
            // 
            this.FilterBegin1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterBegin1.Location = new System.Drawing.Point(189, 76);
            this.FilterBegin1.Name = "FilterBegin1";
            this.FilterBegin1.Size = new System.Drawing.Size(142, 20);
            this.FilterBegin1.TabIndex = 30;
            this.FilterBegin1.Validated += new System.EventHandler(this.FilterBegin1_Validated);
            // 
            // RelationBegin1
            // 
            this.RelationBegin1.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationBegin1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationBegin1.FormattingEnabled = true;
            this.RelationBegin1.Items.AddRange(new object[] {
            "Равно",
            "Больше",
            "Меньше",
            "Больше или равно",
            "Меньше или равно"});
            this.RelationBegin1.Location = new System.Drawing.Point(70, 76);
            this.RelationBegin1.Name = "RelationBegin1";
            this.RelationBegin1.Size = new System.Drawing.Size(120, 21);
            this.RelationBegin1.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 48;
            this.label2.Text = "Начало";
            // 
            // FilterBegin2
            // 
            this.FilterBegin2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterBegin2.Location = new System.Drawing.Point(189, 96);
            this.FilterBegin2.Name = "FilterBegin2";
            this.FilterBegin2.Size = new System.Drawing.Size(142, 20);
            this.FilterBegin2.TabIndex = 40;
            this.FilterBegin2.Validated += new System.EventHandler(this.FilterBegin2_Validated);
            // 
            // RelationBegin2
            // 
            this.RelationBegin2.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationBegin2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationBegin2.FormattingEnabled = true;
            this.RelationBegin2.Items.AddRange(new object[] {
            "Равно",
            "Больше",
            "Меньше",
            "Больше или равно",
            "Меньше или равно"});
            this.RelationBegin2.Location = new System.Drawing.Point(70, 96);
            this.RelationBegin2.Name = "RelationBegin2";
            this.RelationBegin2.Size = new System.Drawing.Size(120, 21);
            this.RelationBegin2.TabIndex = 35;
            // 
            // FilterEnd2
            // 
            this.FilterEnd2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterEnd2.Location = new System.Drawing.Point(189, 136);
            this.FilterEnd2.Name = "FilterEnd2";
            this.FilterEnd2.Size = new System.Drawing.Size(142, 20);
            this.FilterEnd2.TabIndex = 60;
            this.FilterEnd2.TextChanged += new System.EventHandler(this.FilterEnd2_TextChanged);
            // 
            // RelationEnd2
            // 
            this.RelationEnd2.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationEnd2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationEnd2.FormattingEnabled = true;
            this.RelationEnd2.Items.AddRange(new object[] {
            "Равно",
            "Больше",
            "Меньше",
            "Больше или равно",
            "Меньше или равно"});
            this.RelationEnd2.Location = new System.Drawing.Point(70, 136);
            this.RelationEnd2.Name = "RelationEnd2";
            this.RelationEnd2.Size = new System.Drawing.Size(120, 21);
            this.RelationEnd2.TabIndex = 55;
            // 
            // FilterEnd1
            // 
            this.FilterEnd1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterEnd1.Location = new System.Drawing.Point(189, 116);
            this.FilterEnd1.Name = "FilterEnd1";
            this.FilterEnd1.Size = new System.Drawing.Size(142, 20);
            this.FilterEnd1.TabIndex = 50;
            this.FilterEnd1.TextChanged += new System.EventHandler(this.FilterEnd1_TextChanged);
            // 
            // RelationEnd1
            // 
            this.RelationEnd1.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationEnd1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationEnd1.FormattingEnabled = true;
            this.RelationEnd1.Items.AddRange(new object[] {
            "Равно",
            "Больше",
            "Меньше",
            "Больше или равно",
            "Меньше или равно"});
            this.RelationEnd1.Location = new System.Drawing.Point(70, 116);
            this.RelationEnd1.Name = "RelationEnd1";
            this.RelationEnd1.Size = new System.Drawing.Size(120, 21);
            this.RelationEnd1.TabIndex = 45;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 53;
            this.label3.Text = "Конец";
            // 
            // FilterTimeChange2
            // 
            this.FilterTimeChange2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterTimeChange2.Location = new System.Drawing.Point(189, 176);
            this.FilterTimeChange2.Name = "FilterTimeChange2";
            this.FilterTimeChange2.Size = new System.Drawing.Size(142, 20);
            this.FilterTimeChange2.TabIndex = 80;
            this.FilterTimeChange2.TextChanged += new System.EventHandler(this.FilterTimeChange2_TextChanged);
            // 
            // RelationTimeChange2
            // 
            this.RelationTimeChange2.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationTimeChange2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationTimeChange2.FormattingEnabled = true;
            this.RelationTimeChange2.Items.AddRange(new object[] {
            "Больше или равно",
            "Меньше"});
            this.RelationTimeChange2.Location = new System.Drawing.Point(70, 176);
            this.RelationTimeChange2.Name = "RelationTimeChange2";
            this.RelationTimeChange2.Size = new System.Drawing.Size(120, 21);
            this.RelationTimeChange2.TabIndex = 75;
            // 
            // FilterTimeChange1
            // 
            this.FilterTimeChange1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterTimeChange1.Location = new System.Drawing.Point(189, 156);
            this.FilterTimeChange1.Name = "FilterTimeChange1";
            this.FilterTimeChange1.Size = new System.Drawing.Size(142, 20);
            this.FilterTimeChange1.TabIndex = 70;
            this.FilterTimeChange1.TextChanged += new System.EventHandler(this.FilterTimeChange1_TextChanged);
            // 
            // RelationTimeChange1
            // 
            this.RelationTimeChange1.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationTimeChange1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationTimeChange1.FormattingEnabled = true;
            this.RelationTimeChange1.Items.AddRange(new object[] {
            "Больше или равно",
            "Меньше"});
            this.RelationTimeChange1.Location = new System.Drawing.Point(70, 156);
            this.RelationTimeChange1.Name = "RelationTimeChange1";
            this.RelationTimeChange1.Size = new System.Drawing.Size(120, 21);
            this.RelationTimeChange1.TabIndex = 65;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(-4, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 37);
            this.label5.TabIndex = 58;
            this.label5.Text = "Время сохранения";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ButClearFilter);
            this.panel1.Controls.Add(this.ButFilter);
            this.panel1.Controls.Add(this.ButFind);
            this.panel1.Location = new System.Drawing.Point(-2, 205);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 49);
            this.panel1.TabIndex = 88;
            // 
            // ButClearFilter
            // 
            this.ButClearFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButClearFilter.ImageKey = "filter_delete.png";
            this.ButClearFilter.ImageList = this.imageList2;
            this.ButClearFilter.Location = new System.Drawing.Point(234, 2);
            this.ButClearFilter.Name = "ButClearFilter";
            this.ButClearFilter.Size = new System.Drawing.Size(110, 41);
            this.ButClearFilter.TabIndex = 100;
            this.ButClearFilter.Text = " Снять фильтр";
            this.ButClearFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButClearFilter.UseVisualStyleBackColor = true;
            this.ButClearFilter.Click += new System.EventHandler(this.ButClearFilter_Click);
            // 
            // ButFilter
            // 
            this.ButFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButFilter.ImageKey = "filter.png";
            this.ButFilter.ImageList = this.imageList2;
            this.ButFilter.Location = new System.Drawing.Point(130, 2);
            this.ButFilter.Name = "ButFilter";
            this.ButFilter.Size = new System.Drawing.Size(101, 41);
            this.ButFilter.TabIndex = 95;
            this.ButFilter.Text = " Фильтр";
            this.ButFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButFilter.UseVisualStyleBackColor = true;
            this.ButFilter.Click += new System.EventHandler(this.ButFilter_Click);
            // 
            // ButFind
            // 
            this.ButFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButFind.ImageKey = "find.png";
            this.ButFind.ImageList = this.imageList2;
            this.ButFind.Location = new System.Drawing.Point(6, 2);
            this.ButFind.Name = "ButFind";
            this.ButFind.Size = new System.Drawing.Size(89, 41);
            this.ButFind.TabIndex = 90;
            this.ButFind.Text = " Найти";
            this.ButFind.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButFind.UseVisualStyleBackColor = true;
            this.ButFind.Click += new System.EventHandler(this.ButFind_Click);
            // 
            // PickerBegin1
            // 
            this.PickerBegin1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PickerBegin1.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PickerBegin1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PickerBegin1.Location = new System.Drawing.Point(329, 76);
            this.PickerBegin1.Name = "PickerBegin1";
            this.PickerBegin1.Size = new System.Drawing.Size(16, 20);
            this.PickerBegin1.TabIndex = 32;
            this.PickerBegin1.ValueChanged += new System.EventHandler(this.PickerBegin1_ValueChanged);
            // 
            // PickerBegin2
            // 
            this.PickerBegin2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PickerBegin2.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PickerBegin2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PickerBegin2.Location = new System.Drawing.Point(329, 96);
            this.PickerBegin2.Name = "PickerBegin2";
            this.PickerBegin2.Size = new System.Drawing.Size(16, 20);
            this.PickerBegin2.TabIndex = 42;
            this.PickerBegin2.ValueChanged += new System.EventHandler(this.PickerBegin2_ValueChanged);
            // 
            // PickerEnd1
            // 
            this.PickerEnd1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PickerEnd1.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PickerEnd1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PickerEnd1.Location = new System.Drawing.Point(329, 116);
            this.PickerEnd1.Name = "PickerEnd1";
            this.PickerEnd1.Size = new System.Drawing.Size(16, 20);
            this.PickerEnd1.TabIndex = 52;
            this.PickerEnd1.ValueChanged += new System.EventHandler(this.PickerEnd1_ValueChanged);
            // 
            // PickerEnd2
            // 
            this.PickerEnd2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PickerEnd2.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PickerEnd2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PickerEnd2.Location = new System.Drawing.Point(329, 136);
            this.PickerEnd2.Name = "PickerEnd2";
            this.PickerEnd2.Size = new System.Drawing.Size(16, 20);
            this.PickerEnd2.TabIndex = 62;
            this.PickerEnd2.ValueChanged += new System.EventHandler(this.PickerEnd2_ValueChanged);
            // 
            // PickerTimeChange1
            // 
            this.PickerTimeChange1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PickerTimeChange1.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PickerTimeChange1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PickerTimeChange1.Location = new System.Drawing.Point(329, 156);
            this.PickerTimeChange1.Name = "PickerTimeChange1";
            this.PickerTimeChange1.Size = new System.Drawing.Size(16, 20);
            this.PickerTimeChange1.TabIndex = 72;
            this.PickerTimeChange1.ValueChanged += new System.EventHandler(this.PickerTimeChange1_ValueChanged);
            // 
            // PickerTimeChange2
            // 
            this.PickerTimeChange2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PickerTimeChange2.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PickerTimeChange2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PickerTimeChange2.Location = new System.Drawing.Point(329, 176);
            this.PickerTimeChange2.Name = "PickerTimeChange2";
            this.PickerTimeChange2.Size = new System.Drawing.Size(16, 20);
            this.PickerTimeChange2.TabIndex = 85;
            this.PickerTimeChange2.ValueChanged += new System.EventHandler(this.PickerTimeChange2_ValueChanged);
            // 
            // FormFiltersIntervals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 251);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.FilterTimeChange2);
            this.Controls.Add(this.RelationTimeChange2);
            this.Controls.Add(this.FilterTimeChange1);
            this.Controls.Add(this.RelationTimeChange1);
            this.Controls.Add(this.FilterEnd2);
            this.Controls.Add(this.RelationEnd2);
            this.Controls.Add(this.FilterEnd1);
            this.Controls.Add(this.RelationEnd1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.FilterBegin2);
            this.Controls.Add(this.RelationBegin2);
            this.Controls.Add(this.FilterBegin1);
            this.Controls.Add(this.RelationBegin1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FilterName);
            this.Controls.Add(this.RelationName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FilterOtm);
            this.Controls.Add(this.RelationOtm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ButClearFilters);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.PickerBegin1);
            this.Controls.Add(this.PickerBegin2);
            this.Controls.Add(this.PickerEnd1);
            this.Controls.Add(this.PickerEnd2);
            this.Controls.Add(this.PickerTimeChange1);
            this.Controls.Add(this.PickerTimeChange2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFiltersIntervals";
            this.Text = "Фильтрация журнала отчетов";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFiltersIntervals_FormClosing);
            this.Load += new System.EventHandler(this.FormFiltersIntervals_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox FilterOtm;
        private System.Windows.Forms.ComboBox RelationOtm;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ButClearFilters;
        private System.Windows.Forms.TextBox FilterName;
        private System.Windows.Forms.ComboBox RelationName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FilterBegin1;
        private System.Windows.Forms.ComboBox RelationBegin1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterBegin2;
        private System.Windows.Forms.ComboBox RelationBegin2;
        private System.Windows.Forms.TextBox FilterEnd2;
        private System.Windows.Forms.ComboBox RelationEnd2;
        private System.Windows.Forms.TextBox FilterEnd1;
        private System.Windows.Forms.ComboBox RelationEnd1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox FilterTimeChange2;
        private System.Windows.Forms.ComboBox RelationTimeChange2;
        private System.Windows.Forms.TextBox FilterTimeChange1;
        private System.Windows.Forms.ComboBox RelationTimeChange1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ButClearFilter;
        private System.Windows.Forms.Button ButFilter;
        private System.Windows.Forms.Button ButFind;
        private System.Windows.Forms.DateTimePicker PickerBegin1;
        private System.Windows.Forms.DateTimePicker PickerBegin2;
        private System.Windows.Forms.DateTimePicker PickerEnd1;
        private System.Windows.Forms.DateTimePicker PickerEnd2;
        private System.Windows.Forms.DateTimePicker PickerTimeChange1;
        private System.Windows.Forms.DateTimePicker PickerTimeChange2;
    }
}