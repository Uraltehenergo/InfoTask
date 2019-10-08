namespace ReporterCommon
{
    partial class FormFiltersParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFiltersParams));
            this.label1 = new System.Windows.Forms.Label();
            this.RelationFullCode = new System.Windows.Forms.ComboBox();
            this.FilterFullCode = new System.Windows.Forms.TextBox();
            this.FilterCode = new System.Windows.Forms.TextBox();
            this.RelationCode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterSubCode = new System.Windows.Forms.TextBox();
            this.RelationSubCode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.FilterName = new System.Windows.Forms.TextBox();
            this.RelationName = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.RelationTask = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.FilterTask = new System.Windows.Forms.ComboBox();
            this.FilterUnits = new System.Windows.Forms.ComboBox();
            this.RelationUnits = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.FilterDataType = new System.Windows.Forms.ComboBox();
            this.RelationDataType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.FilterSuperProcess = new System.Windows.Forms.ComboBox();
            this.RelationSuperProcess = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.FilterCalcParamType = new System.Windows.Forms.ComboBox();
            this.RelationCalcParamType = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.FilterComment = new System.Windows.Forms.TextBox();
            this.RelationComment = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButClearFilter = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.ButFilter = new System.Windows.Forms.Button();
            this.ButFind = new System.Windows.Forms.Button();
            this.FilterOtm = new System.Windows.Forms.ComboBox();
            this.RelationOtm = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ButClearFilters = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Полный код";
            // 
            // RelationFullCode
            // 
            this.RelationFullCode.AutoCompleteCustomSource.AddRange(new string[] {
            "Равно"});
            this.RelationFullCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationFullCode.FormattingEnabled = true;
            this.RelationFullCode.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на",
            "По шаблону",
            "Пустое",
            "Не пустое"});
            this.RelationFullCode.Location = new System.Drawing.Point(108, 56);
            this.RelationFullCode.Name = "RelationFullCode";
            this.RelationFullCode.Size = new System.Drawing.Size(97, 21);
            this.RelationFullCode.TabIndex = 15;
            this.RelationFullCode.SelectedIndexChanged += new System.EventHandler(this.RelationFullCode_SelectedIndexChanged);
            // 
            // FilterFullCode
            // 
            this.FilterFullCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterFullCode.Location = new System.Drawing.Point(205, 56);
            this.FilterFullCode.Name = "FilterFullCode";
            this.FilterFullCode.Size = new System.Drawing.Size(175, 20);
            this.FilterFullCode.TabIndex = 20;
            // 
            // FilterCode
            // 
            this.FilterCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterCode.Location = new System.Drawing.Point(205, 76);
            this.FilterCode.Name = "FilterCode";
            this.FilterCode.Size = new System.Drawing.Size(175, 20);
            this.FilterCode.TabIndex = 30;
            // 
            // RelationCode
            // 
            this.RelationCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationCode.FormattingEnabled = true;
            this.RelationCode.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на",
            "По шаблону",
            "Пустое",
            "Не пустое"});
            this.RelationCode.Location = new System.Drawing.Point(108, 76);
            this.RelationCode.Name = "RelationCode";
            this.RelationCode.Size = new System.Drawing.Size(97, 21);
            this.RelationCode.TabIndex = 25;
            this.RelationCode.SelectedIndexChanged += new System.EventHandler(this.RelationCode_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Код параметра";
            // 
            // FilterSubCode
            // 
            this.FilterSubCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterSubCode.Location = new System.Drawing.Point(205, 96);
            this.FilterSubCode.Name = "FilterSubCode";
            this.FilterSubCode.Size = new System.Drawing.Size(175, 20);
            this.FilterSubCode.TabIndex = 40;
            // 
            // RelationSubCode
            // 
            this.RelationSubCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationSubCode.FormattingEnabled = true;
            this.RelationSubCode.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на",
            "По шаблону",
            "Пустое",
            "Не пустое"});
            this.RelationSubCode.Location = new System.Drawing.Point(108, 96);
            this.RelationSubCode.Name = "RelationSubCode";
            this.RelationSubCode.Size = new System.Drawing.Size(97, 21);
            this.RelationSubCode.TabIndex = 35;
            this.RelationSubCode.SelectedIndexChanged += new System.EventHandler(this.RelationSubCode_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Код подпараметра";
            // 
            // FilterName
            // 
            this.FilterName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterName.Location = new System.Drawing.Point(205, 116);
            this.FilterName.Name = "FilterName";
            this.FilterName.Size = new System.Drawing.Size(175, 20);
            this.FilterName.TabIndex = 50;
            // 
            // RelationName
            // 
            this.RelationName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationName.FormattingEnabled = true;
            this.RelationName.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на",
            "По шаблону",
            "Пустое",
            "Не пустое"});
            this.RelationName.Location = new System.Drawing.Point(108, 116);
            this.RelationName.Name = "RelationName";
            this.RelationName.Size = new System.Drawing.Size(97, 21);
            this.RelationName.TabIndex = 45;
            this.RelationName.SelectedIndexChanged += new System.EventHandler(this.RelationName_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(76, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Имя";
            // 
            // RelationTask
            // 
            this.RelationTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationTask.FormattingEnabled = true;
            this.RelationTask.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на",
            "По шаблону",
            "Пустое",
            "Не пустое"});
            this.RelationTask.Location = new System.Drawing.Point(108, 156);
            this.RelationTask.Name = "RelationTask";
            this.RelationTask.Size = new System.Drawing.Size(97, 21);
            this.RelationTask.TabIndex = 65;
            this.RelationTask.SelectedIndexChanged += new System.EventHandler(this.RelationTask_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(63, 159);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Задача";
            // 
            // FilterTask
            // 
            this.FilterTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterTask.FormattingEnabled = true;
            this.FilterTask.Location = new System.Drawing.Point(205, 156);
            this.FilterTask.Name = "FilterTask";
            this.FilterTask.Size = new System.Drawing.Size(175, 21);
            this.FilterTask.TabIndex = 70;
            // 
            // FilterUnits
            // 
            this.FilterUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterUnits.FormattingEnabled = true;
            this.FilterUnits.Location = new System.Drawing.Point(205, 176);
            this.FilterUnits.Name = "FilterUnits";
            this.FilterUnits.Size = new System.Drawing.Size(175, 21);
            this.FilterUnits.TabIndex = 80;
            // 
            // RelationUnits
            // 
            this.RelationUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationUnits.FormattingEnabled = true;
            this.RelationUnits.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на",
            "По шаблону",
            "Пустое",
            "Не пустое"});
            this.RelationUnits.Location = new System.Drawing.Point(108, 176);
            this.RelationUnits.Name = "RelationUnits";
            this.RelationUnits.Size = new System.Drawing.Size(97, 21);
            this.RelationUnits.TabIndex = 75;
            this.RelationUnits.SelectedIndexChanged += new System.EventHandler(this.RelationUnits_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(54, 179);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Единицы";
            // 
            // FilterDataType
            // 
            this.FilterDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterDataType.FormattingEnabled = true;
            this.FilterDataType.Location = new System.Drawing.Point(205, 196);
            this.FilterDataType.Name = "FilterDataType";
            this.FilterDataType.Size = new System.Drawing.Size(175, 21);
            this.FilterDataType.TabIndex = 90;
            // 
            // RelationDataType
            // 
            this.RelationDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationDataType.FormattingEnabled = true;
            this.RelationDataType.Items.AddRange(new object[] {
            "Равно",
            "Не равно"});
            this.RelationDataType.Location = new System.Drawing.Point(108, 196);
            this.RelationDataType.Name = "RelationDataType";
            this.RelationDataType.Size = new System.Drawing.Size(97, 21);
            this.RelationDataType.TabIndex = 85;
            this.RelationDataType.SelectedIndexChanged += new System.EventHandler(this.RelationDataType_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(40, 199);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Тип данных";
            // 
            // FilterSuperProcess
            // 
            this.FilterSuperProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterSuperProcess.FormattingEnabled = true;
            this.FilterSuperProcess.Location = new System.Drawing.Point(205, 216);
            this.FilterSuperProcess.Name = "FilterSuperProcess";
            this.FilterSuperProcess.Size = new System.Drawing.Size(175, 21);
            this.FilterSuperProcess.TabIndex = 105;
            // 
            // RelationSuperProcess
            // 
            this.RelationSuperProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationSuperProcess.FormattingEnabled = true;
            this.RelationSuperProcess.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на"});
            this.RelationSuperProcess.Location = new System.Drawing.Point(108, 216);
            this.RelationSuperProcess.Name = "RelationSuperProcess";
            this.RelationSuperProcess.Size = new System.Drawing.Size(97, 21);
            this.RelationSuperProcess.TabIndex = 100;
            this.RelationSuperProcess.SelectedIndexChanged += new System.EventHandler(this.RelationSuperProcess_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(37, 219);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 27;
            this.label10.Text = "Накопление";
            // 
            // FilterCalcParamType
            // 
            this.FilterCalcParamType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterCalcParamType.FormattingEnabled = true;
            this.FilterCalcParamType.Location = new System.Drawing.Point(205, 236);
            this.FilterCalcParamType.Name = "FilterCalcParamType";
            this.FilterCalcParamType.Size = new System.Drawing.Size(175, 21);
            this.FilterCalcParamType.TabIndex = 115;
            // 
            // RelationCalcParamType
            // 
            this.RelationCalcParamType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationCalcParamType.FormattingEnabled = true;
            this.RelationCalcParamType.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Пустое",
            "Не пустое"});
            this.RelationCalcParamType.Location = new System.Drawing.Point(108, 236);
            this.RelationCalcParamType.Name = "RelationCalcParamType";
            this.RelationCalcParamType.Size = new System.Drawing.Size(97, 21);
            this.RelationCalcParamType.TabIndex = 110;
            this.RelationCalcParamType.SelectedIndexChanged += new System.EventHandler(this.RelationCalcParamType_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(73, 239);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "Ввод";
            // 
            // FilterComment
            // 
            this.FilterComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterComment.Location = new System.Drawing.Point(205, 136);
            this.FilterComment.Name = "FilterComment";
            this.FilterComment.Size = new System.Drawing.Size(175, 20);
            this.FilterComment.TabIndex = 60;
            // 
            // RelationComment
            // 
            this.RelationComment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationComment.FormattingEnabled = true;
            this.RelationComment.Items.AddRange(new object[] {
            "Равно",
            "Содержит",
            "Не содержит",
            "Начинается с",
            "Кончается на",
            "По шаблону",
            "Пустое",
            "Не пустое"});
            this.RelationComment.Location = new System.Drawing.Point(108, 136);
            this.RelationComment.Name = "RelationComment";
            this.RelationComment.Size = new System.Drawing.Size(97, 21);
            this.RelationComment.TabIndex = 55;
            this.RelationComment.SelectedIndexChanged += new System.EventHandler(this.RelationComment_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(36, 139);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "Коментарий";
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
            this.panel1.Location = new System.Drawing.Point(-4, 260);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 49);
            this.panel1.TabIndex = 120;
            // 
            // ButClearFilter
            // 
            this.ButClearFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButClearFilter.ImageKey = "filter_delete.png";
            this.ButClearFilter.ImageList = this.imageList2;
            this.ButClearFilter.Location = new System.Drawing.Point(245, 3);
            this.ButClearFilter.Name = "ButClearFilter";
            this.ButClearFilter.Size = new System.Drawing.Size(110, 41);
            this.ButClearFilter.TabIndex = 135;
            this.ButClearFilter.Text = " Снять фильтр";
            this.ButClearFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButClearFilter.UseVisualStyleBackColor = true;
            this.ButClearFilter.Click += new System.EventHandler(this.ButClearFilter_Click);
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
            // ButFilter
            // 
            this.ButFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButFilter.ImageKey = "filter.png";
            this.ButFilter.ImageList = this.imageList2;
            this.ButFilter.Location = new System.Drawing.Point(138, 3);
            this.ButFilter.Name = "ButFilter";
            this.ButFilter.Size = new System.Drawing.Size(101, 41);
            this.ButFilter.TabIndex = 130;
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
            this.ButFind.Location = new System.Drawing.Point(9, 3);
            this.ButFind.Name = "ButFind";
            this.ButFind.Size = new System.Drawing.Size(89, 41);
            this.ButFind.TabIndex = 125;
            this.ButFind.Text = " Найти";
            this.ButFind.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButFind.UseVisualStyleBackColor = true;
            this.ButFind.Click += new System.EventHandler(this.ButFind_Click);
            // 
            // FilterOtm
            // 
            this.FilterOtm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterOtm.FormattingEnabled = true;
            this.FilterOtm.Items.AddRange(new object[] {
            "Вкл",
            "Откл"});
            this.FilterOtm.Location = new System.Drawing.Point(205, 36);
            this.FilterOtm.Name = "FilterOtm";
            this.FilterOtm.Size = new System.Drawing.Size(175, 21);
            this.FilterOtm.TabIndex = 10;
            // 
            // RelationOtm
            // 
            this.RelationOtm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationOtm.FormattingEnabled = true;
            this.RelationOtm.Items.AddRange(new object[] {
            "Равно"});
            this.RelationOtm.Location = new System.Drawing.Point(108, 36);
            this.RelationOtm.Name = "RelationOtm";
            this.RelationOtm.Size = new System.Drawing.Size(97, 21);
            this.RelationOtm.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(54, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "Отметка";
            // 
            // ButClearFilters
            // 
            this.ButClearFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButClearFilters.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButClearFilters.ImageKey = "cross.png";
            this.ButClearFilters.ImageList = this.imageList2;
            this.ButClearFilters.Location = new System.Drawing.Point(234, 1);
            this.ButClearFilters.Name = "ButClearFilters";
            this.ButClearFilters.Size = new System.Drawing.Size(147, 33);
            this.ButClearFilters.TabIndex = 140;
            this.ButClearFilters.Text = " Очистить бланк";
            this.ButClearFilters.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButClearFilters.UseVisualStyleBackColor = true;
            this.ButClearFilters.Click += new System.EventHandler(this.ButClearFilters_Click);
            // 
            // FormFiltersParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 311);
            this.Controls.Add(this.FilterOtm);
            this.Controls.Add(this.RelationOtm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ButClearFilters);
            this.Controls.Add(this.FilterComment);
            this.Controls.Add(this.RelationComment);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.FilterCalcParamType);
            this.Controls.Add(this.RelationCalcParamType);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.FilterSuperProcess);
            this.Controls.Add(this.RelationSuperProcess);
            this.Controls.Add(this.FilterDataType);
            this.Controls.Add(this.RelationDataType);
            this.Controls.Add(this.FilterUnits);
            this.Controls.Add(this.RelationUnits);
            this.Controls.Add(this.FilterTask);
            this.Controls.Add(this.RelationTask);
            this.Controls.Add(this.FilterName);
            this.Controls.Add(this.RelationName);
            this.Controls.Add(this.FilterSubCode);
            this.Controls.Add(this.RelationSubCode);
            this.Controls.Add(this.FilterCode);
            this.Controls.Add(this.RelationCode);
            this.Controls.Add(this.FilterFullCode);
            this.Controls.Add(this.RelationFullCode);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFiltersParams";
            this.ShowInTaskbar = false;
            this.Text = "Фильтры и поиск параметров";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFiltersParams_FormClosing);
            this.Load += new System.EventHandler(this.FormFiltersParams_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox RelationFullCode;
        private System.Windows.Forms.TextBox FilterFullCode;
        private System.Windows.Forms.TextBox FilterCode;
        private System.Windows.Forms.ComboBox RelationCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterSubCode;
        private System.Windows.Forms.ComboBox RelationSubCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox FilterName;
        private System.Windows.Forms.ComboBox RelationName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox RelationTask;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox FilterTask;
        private System.Windows.Forms.ComboBox FilterUnits;
        private System.Windows.Forms.ComboBox RelationUnits;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox FilterDataType;
        private System.Windows.Forms.ComboBox RelationDataType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox FilterSuperProcess;
        private System.Windows.Forms.ComboBox RelationSuperProcess;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox FilterCalcParamType;
        private System.Windows.Forms.ComboBox RelationCalcParamType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox FilterComment;
        private System.Windows.Forms.ComboBox RelationComment;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button ButClearFilters;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ButClearFilter;
        private System.Windows.Forms.Button ButFilter;
        private System.Windows.Forms.Button ButFind;
        private System.Windows.Forms.ComboBox FilterOtm;
        private System.Windows.Forms.ComboBox RelationOtm;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ImageList imageList2;
    }
}