using System.Windows.Forms;

namespace ReporterCommon
{
    partial class FormSetup
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetup));
            this.butOK = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.tabReport = new System.Windows.Forms.TabPage();
            this.NameReport = new System.Windows.Forms.TextBox();
            this.CalcName = new System.Windows.Forms.TextBox();
            this.MinuteStart = new System.Windows.Forms.TextBox();
            this.HourStart = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.HourLength = new System.Windows.Forms.TextBox();
            this.DayLength = new System.Windows.Forms.TextBox();
            this.MinuteLength = new System.Windows.Forms.TextBox();
            this.radioEquals = new System.Windows.Forms.RadioButton();
            this.radioLess = new System.Windows.Forms.RadioButton();
            this.radioDifferent = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.DifferentBegin = new System.Windows.Forms.CheckBox();
            this.FileReport = new System.Windows.Forms.TextBox();
            this.DescriptionReport = new System.Windows.Forms.TextBox();
            this.CodeReport = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.DefaultPeriod = new System.Windows.Forms.GroupBox();
            this.radioNow = new System.Windows.Forms.RadioButton();
            this.radioPrevious = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.tabArchive = new System.Windows.Forms.TabPage();
            this.Projects = new System.Windows.Forms.DataGridView();
            this.Project = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectCode2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectCalcMode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ProjectFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Brouse = new System.Windows.Forms.DataGridViewButtonColumn();
            this.UseOneArchive = new System.Windows.Forms.CheckBox();
            this.Providers = new System.Windows.Forms.DataGridView();
            this.TypeProvider = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProviderCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProviderName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProviderInf = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImitMode = new System.Windows.Forms.ComboBox();
            this.IsImit = new System.Windows.Forms.CheckBox();
            this.ThreadId = new System.Windows.Forms.TextBox();
            this.labelThreadId = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.AllowProjectsRepetitions = new System.Windows.Forms.CheckBox();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ButServerReportFile = new System.Windows.Forms.Button();
            this.ServerReport = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.FormTo = new System.Windows.Forms.GroupBox();
            this.SetFocusToFormed = new System.Windows.Forms.CheckBox();
            this.AddSheetToName = new System.Windows.Forms.CheckBox();
            this.EndNameFormat = new System.Windows.Forms.ComboBox();
            this.AddEndToName = new System.Windows.Forms.CheckBox();
            this.BeginNameFormat = new System.Windows.Forms.ComboBox();
            this.AddBeginToName = new System.Windows.Forms.CheckBox();
            this.DateNameFormat = new System.Windows.Forms.ComboBox();
            this.ButCreateResultFile = new System.Windows.Forms.Button();
            this.AddCalcNameToName = new System.Windows.Forms.CheckBox();
            this.ResultFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AddDateToName = new System.Windows.Forms.CheckBox();
            this.ButResultFile = new System.Windows.Forms.Button();
            this.ResultFile = new System.Windows.Forms.TextBox();
            this.ButResultDir = new System.Windows.Forms.Button();
            this.ResultDir = new System.Windows.Forms.TextBox();
            this.FormToFile = new System.Windows.Forms.RadioButton();
            this.FormToDir = new System.Windows.Forms.RadioButton();
            this.FormToTemplate = new System.Windows.Forms.RadioButton();
            this.MonthLength = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.DayStart = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tabReport.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.DefaultPeriod.SuspendLayout();
            this.tabArchive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Projects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Providers)).BeginInit();
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.FormTo.SuspendLayout();
            this.SuspendLayout();
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butOK.Location = new System.Drawing.Point(259, 510);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(181, 37);
            this.butOK.TabIndex = 300;
            this.butOK.Text = "Сохранить настройки";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCancel.Location = new System.Drawing.Point(478, 510);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(181, 37);
            this.butCancel.TabIndex = 305;
            this.butCancel.Text = "Отмена";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // tabReport
            // 
            this.tabReport.Controls.Add(this.DayStart);
            this.tabReport.Controls.Add(this.label16);
            this.tabReport.Controls.Add(this.NameReport);
            this.tabReport.Controls.Add(this.CalcName);
            this.tabReport.Controls.Add(this.MinuteStart);
            this.tabReport.Controls.Add(this.HourStart);
            this.tabReport.Controls.Add(this.groupBox1);
            this.tabReport.Controls.Add(this.DifferentBegin);
            this.tabReport.Controls.Add(this.FileReport);
            this.tabReport.Controls.Add(this.DescriptionReport);
            this.tabReport.Controls.Add(this.CodeReport);
            this.tabReport.Controls.Add(this.label21);
            this.tabReport.Controls.Add(this.label17);
            this.tabReport.Controls.Add(this.label24);
            this.tabReport.Controls.Add(this.label18);
            this.tabReport.Controls.Add(this.label7);
            this.tabReport.Controls.Add(this.label8);
            this.tabReport.Controls.Add(this.label9);
            this.tabReport.Controls.Add(this.DefaultPeriod);
            this.tabReport.Controls.Add(this.label6);
            this.tabReport.Location = new System.Drawing.Point(4, 25);
            this.tabReport.Name = "tabReport";
            this.tabReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabReport.Size = new System.Drawing.Size(942, 476);
            this.tabReport.TabIndex = 1;
            this.tabReport.Text = "Свойства бланка отчета";
            this.tabReport.UseVisualStyleBackColor = true;
            this.tabReport.Click += new System.EventHandler(this.tabReport_Click);
            // 
            // NameReport
            // 
            this.NameReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NameReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NameReport.Location = new System.Drawing.Point(314, 7);
            this.NameReport.Name = "NameReport";
            this.NameReport.Size = new System.Drawing.Size(540, 22);
            this.NameReport.TabIndex = 207;
            // 
            // CalcName
            // 
            this.CalcName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CalcName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CalcName.Location = new System.Drawing.Point(233, 377);
            this.CalcName.Name = "CalcName";
            this.CalcName.Size = new System.Drawing.Size(304, 22);
            this.CalcName.TabIndex = 255;
            // 
            // MinuteStart
            // 
            this.MinuteStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MinuteStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinuteStart.Location = new System.Drawing.Point(502, 245);
            this.MinuteStart.Name = "MinuteStart";
            this.MinuteStart.Size = new System.Drawing.Size(34, 21);
            this.MinuteStart.TabIndex = 238;
            // 
            // HourStart
            // 
            this.HourStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HourStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HourStart.Location = new System.Drawing.Point(411, 245);
            this.HourStart.Name = "HourStart";
            this.HourStart.Size = new System.Drawing.Size(34, 21);
            this.HourStart.TabIndex = 235;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.MonthLength);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.HourLength);
            this.groupBox1.Controls.Add(this.DayLength);
            this.groupBox1.Controls.Add(this.MinuteLength);
            this.groupBox1.Controls.Add(this.radioEquals);
            this.groupBox1.Controls.Add(this.radioLess);
            this.groupBox1.Controls.Add(this.radioDifferent);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(42, 116);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(545, 118);
            this.groupBox1.TabIndex = 215;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Длительность периода отчета по умолчанию";
            // 
            // HourLength
            // 
            this.HourLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HourLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HourLength.Location = new System.Drawing.Point(211, 19);
            this.HourLength.Name = "HourLength";
            this.HourLength.Size = new System.Drawing.Size(35, 22);
            this.HourLength.TabIndex = 222;
            // 
            // DayLength
            // 
            this.DayLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DayLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DayLength.Location = new System.Drawing.Point(120, 19);
            this.DayLength.Name = "DayLength";
            this.DayLength.Size = new System.Drawing.Size(35, 22);
            this.DayLength.TabIndex = 220;
            // 
            // MinuteLength
            // 
            this.MinuteLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MinuteLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinuteLength.Location = new System.Drawing.Point(305, 19);
            this.MinuteLength.Name = "MinuteLength";
            this.MinuteLength.Size = new System.Drawing.Size(35, 22);
            this.MinuteLength.TabIndex = 225;
            // 
            // radioEquals
            // 
            this.radioEquals.AutoSize = true;
            this.radioEquals.Location = new System.Drawing.Point(19, 88);
            this.radioEquals.Name = "radioEquals";
            this.radioEquals.Size = new System.Drawing.Size(411, 20);
            this.radioEquals.TabIndex = 230;
            this.radioEquals.TabStop = true;
            this.radioEquals.Text = "Длительность должна быть равна заданной по умолчанию";
            this.radioEquals.UseVisualStyleBackColor = true;
            // 
            // radioLess
            // 
            this.radioLess.AutoSize = true;
            this.radioLess.Location = new System.Drawing.Point(19, 67);
            this.radioLess.Name = "radioLess";
            this.radioLess.Size = new System.Drawing.Size(449, 20);
            this.radioLess.TabIndex = 228;
            this.radioLess.TabStop = true;
            this.radioLess.Text = "Длительность не должна превосходить заданную по умолчанию";
            this.radioLess.UseVisualStyleBackColor = true;
            // 
            // radioDifferent
            // 
            this.radioDifferent.AutoSize = true;
            this.radioDifferent.Location = new System.Drawing.Point(19, 46);
            this.radioDifferent.Name = "radioDifferent";
            this.radioDifferent.Size = new System.Drawing.Size(304, 20);
            this.radioDifferent.TabIndex = 227;
            this.radioDifferent.TabStop = true;
            this.radioDifferent.Text = "Разрешается произвольная длительность";
            this.radioDifferent.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.Location = new System.Drawing.Point(340, 21);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 16);
            this.label15.TabIndex = 31;
            this.label15.Text = "минут";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(155, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 16);
            this.label13.TabIndex = 27;
            this.label13.Text = "суток";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.Location = new System.Drawing.Point(246, 21);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 16);
            this.label14.TabIndex = 29;
            this.label14.Text = "часов";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DifferentBegin
            // 
            this.DifferentBegin.AutoSize = true;
            this.DifferentBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DifferentBegin.Location = new System.Drawing.Point(44, 268);
            this.DifferentBegin.Name = "DifferentBegin";
            this.DifferentBegin.Size = new System.Drawing.Size(371, 20);
            this.DifferentBegin.TabIndex = 240;
            this.DifferentBegin.Text = "Разрешается произвольное начало периода отчета";
            this.DifferentBegin.UseVisualStyleBackColor = true;
            // 
            // FileReport
            // 
            this.FileReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FileReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FileReport.Location = new System.Drawing.Point(82, 79);
            this.FileReport.Name = "FileReport";
            this.FileReport.ReadOnly = true;
            this.FileReport.Size = new System.Drawing.Size(772, 22);
            this.FileReport.TabIndex = 212;
            // 
            // DescriptionReport
            // 
            this.DescriptionReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DescriptionReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DescriptionReport.Location = new System.Drawing.Point(82, 34);
            this.DescriptionReport.Multiline = true;
            this.DescriptionReport.Name = "DescriptionReport";
            this.DescriptionReport.Size = new System.Drawing.Size(772, 39);
            this.DescriptionReport.TabIndex = 210;
            // 
            // CodeReport
            // 
            this.CodeReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CodeReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CodeReport.Location = new System.Drawing.Point(42, 7);
            this.CodeReport.Name = "CodeReport";
            this.CodeReport.Size = new System.Drawing.Size(230, 22);
            this.CodeReport.TabIndex = 205;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label21.Location = new System.Drawing.Point(44, 247);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(271, 16);
            this.label21.TabIndex = 40;
            this.label21.Text = "Отсчитывать начало периода отчета от";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label17.Location = new System.Drawing.Point(539, 247);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(48, 16);
            this.label17.TabIndex = 35;
            this.label17.Text = "минут";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label24.Location = new System.Drawing.Point(44, 379);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(188, 16);
            this.label24.TabIndex = 38;
            this.label24.Text = "Имя расчета по умолчанию";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label18.Location = new System.Drawing.Point(448, 247);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 16);
            this.label18.TabIndex = 33;
            this.label18.Text = "часов";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(6, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 16);
            this.label7.TabIndex = 15;
            this.label7.Text = "Описание";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(275, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Имя";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.label9.Location = new System.Drawing.Point(5, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 16);
            this.label9.TabIndex = 11;
            this.label9.Text = "Код";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DefaultPeriod
            // 
            this.DefaultPeriod.Controls.Add(this.radioNow);
            this.DefaultPeriod.Controls.Add(this.radioPrevious);
            this.DefaultPeriod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DefaultPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DefaultPeriod.Location = new System.Drawing.Point(42, 294);
            this.DefaultPeriod.Name = "DefaultPeriod";
            this.DefaultPeriod.Size = new System.Drawing.Size(545, 77);
            this.DefaultPeriod.TabIndex = 245;
            this.DefaultPeriod.TabStop = false;
            this.DefaultPeriod.Text = "Устанавливать начало и конец периода отчета";
            // 
            // radioNow
            // 
            this.radioNow.AutoSize = true;
            this.radioNow.Location = new System.Drawing.Point(16, 45);
            this.radioNow.Name = "radioNow";
            this.radioNow.Size = new System.Drawing.Size(307, 20);
            this.radioNow.TabIndex = 250;
            this.radioNow.TabStop = true;
            this.radioNow.Text = "расчитанным, исходя из текущего времени";
            this.radioNow.UseVisualStyleBackColor = true;
            // 
            // radioPrevious
            // 
            this.radioPrevious.AutoSize = true;
            this.radioPrevious.Location = new System.Drawing.Point(16, 21);
            this.radioPrevious.Name = "radioPrevious";
            this.radioPrevious.Size = new System.Drawing.Size(442, 20);
            this.radioPrevious.TabIndex = 248;
            this.radioPrevious.TabStop = true;
            this.radioPrevious.Text = "равным концу периода при предыдущем формировании отчета";
            this.radioPrevious.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(36, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 16);
            this.label6.TabIndex = 17;
            this.label6.Text = "Файл";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabArchive
            // 
            this.tabArchive.Controls.Add(this.Projects);
            this.tabArchive.Controls.Add(this.UseOneArchive);
            this.tabArchive.Controls.Add(this.Providers);
            this.tabArchive.Controls.Add(this.ImitMode);
            this.tabArchive.Controls.Add(this.IsImit);
            this.tabArchive.Controls.Add(this.ThreadId);
            this.tabArchive.Controls.Add(this.labelThreadId);
            this.tabArchive.Controls.Add(this.label11);
            this.tabArchive.Controls.Add(this.label10);
            this.tabArchive.Controls.Add(this.label1);
            this.tabArchive.Controls.Add(this.AllowProjectsRepetitions);
            this.tabArchive.Location = new System.Drawing.Point(4, 25);
            this.tabArchive.Name = "tabArchive";
            this.tabArchive.Padding = new System.Windows.Forms.Padding(3);
            this.tabArchive.Size = new System.Drawing.Size(942, 476);
            this.tabArchive.TabIndex = 0;
            this.tabArchive.Text = "Архив и проекты";
            this.tabArchive.UseVisualStyleBackColor = true;
            // 
            // Projects
            // 
            this.Projects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Projects.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Projects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Projects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Project,
            this.ProjectCode2,
            this.ProjectName,
            this.ProjectCalcMode,
            this.ProjectFile,
            this.Brouse});
            this.Projects.Location = new System.Drawing.Point(3, 24);
            this.Projects.Name = "Projects";
            this.Projects.RowHeadersWidth = 24;
            this.Projects.Size = new System.Drawing.Size(936, 168);
            this.Projects.TabIndex = 5;
            this.Projects.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Projects_CellContentClick);
            this.Projects.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Projects_CellValueChanged);
            this.Projects.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.Projects_RowsRemoved);
            // 
            // Project
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Project.DefaultCellStyle = dataGridViewCellStyle1;
            this.Project.HeaderText = "Код";
            this.Project.Name = "Project";
            this.Project.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Project.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Project.Width = 150;
            // 
            // ProjectCode2
            // 
            this.ProjectCode2.HeaderText = "Псевдоним";
            this.ProjectCode2.Name = "ProjectCode2";
            this.ProjectCode2.Visible = false;
            this.ProjectCode2.Width = 110;
            // 
            // ProjectName
            // 
            this.ProjectName.HeaderText = "Имя";
            this.ProjectName.Name = "ProjectName";
            this.ProjectName.ReadOnly = true;
            this.ProjectName.Width = 220;
            // 
            // ProjectCalcMode
            // 
            dataGridViewCellStyle2.NullValue = "Сторонний периодический";
            this.ProjectCalcMode.DefaultCellStyle = dataGridViewCellStyle2;
            this.ProjectCalcMode.HeaderText = "Выполнение расчета";
            this.ProjectCalcMode.Items.AddRange(new object[] {
            "При построении отчета",
            "Сторонний периодический"});
            this.ProjectCalcMode.Name = "ProjectCalcMode";
            this.ProjectCalcMode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ProjectCalcMode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ProjectCalcMode.Width = 210;
            // 
            // ProjectFile
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProjectFile.DefaultCellStyle = dataGridViewCellStyle3;
            this.ProjectFile.HeaderText = "Файл";
            this.ProjectFile.Name = "ProjectFile";
            this.ProjectFile.ReadOnly = true;
            this.ProjectFile.Width = 280;
            // 
            // Brouse
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle4.NullValue = "Обзор";
            this.Brouse.DefaultCellStyle = dataGridViewCellStyle4;
            this.Brouse.HeaderText = "...";
            this.Brouse.Name = "Brouse";
            this.Brouse.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Brouse.Text = "...";
            this.Brouse.ToolTipText = "Выбор файла";
            this.Brouse.Width = 50;
            // 
            // UseOneArchive
            // 
            this.UseOneArchive.AutoSize = true;
            this.UseOneArchive.Location = new System.Drawing.Point(130, 202);
            this.UseOneArchive.Name = "UseOneArchive";
            this.UseOneArchive.Size = new System.Drawing.Size(197, 20);
            this.UseOneArchive.TabIndex = 15;
            this.UseOneArchive.Text = "Использовать один архив";
            this.UseOneArchive.UseVisualStyleBackColor = true;
            this.UseOneArchive.Click += new System.EventHandler(this.UseOneArchive_Click);
            // 
            // Providers
            // 
            this.Providers.AllowUserToAddRows = false;
            this.Providers.AllowUserToDeleteRows = false;
            this.Providers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Providers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Providers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Providers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TypeProvider,
            this.ProviderCode,
            this.ProviderName,
            this.Description,
            this.ProviderInf});
            this.Providers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Providers.Location = new System.Drawing.Point(4, 225);
            this.Providers.Name = "Providers";
            this.Providers.RowHeadersVisible = false;
            this.Providers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Providers.Size = new System.Drawing.Size(935, 216);
            this.Providers.TabIndex = 20;
            this.Providers.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Providers_CellDoubleClick);
            // 
            // TypeProvider
            // 
            this.TypeProvider.HeaderText = "Тип";
            this.TypeProvider.Name = "TypeProvider";
            this.TypeProvider.ReadOnly = true;
            this.TypeProvider.Width = 90;
            // 
            // ProviderCode
            // 
            this.ProviderCode.HeaderText = "Код";
            this.ProviderCode.Name = "ProviderCode";
            this.ProviderCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ProviderCode.Width = 210;
            // 
            // ProviderName
            // 
            this.ProviderName.HeaderText = "Имя";
            this.ProviderName.Name = "ProviderName";
            this.ProviderName.ReadOnly = true;
            this.ProviderName.Width = 310;
            // 
            // Description
            // 
            this.Description.HeaderText = "Описание";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 320;
            // 
            // ProviderInf
            // 
            this.ProviderInf.HeaderText = "Свойства";
            this.ProviderInf.Name = "ProviderInf";
            this.ProviderInf.ReadOnly = true;
            this.ProviderInf.Visible = false;
            this.ProviderInf.Width = 250;
            // 
            // ImitMode
            // 
            this.ImitMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImitMode.FormattingEnabled = true;
            this.ImitMode.Items.AddRange(new object[] {
            "Отсчитывать от начала периода",
            "Отсчитывать от начала часа",
            "Отсчитывать от начала суток"});
            this.ImitMode.Location = new System.Drawing.Point(444, 447);
            this.ImitMode.Name = "ImitMode";
            this.ImitMode.Size = new System.Drawing.Size(316, 24);
            this.ImitMode.TabIndex = 35;
            // 
            // IsImit
            // 
            this.IsImit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IsImit.AutoSize = true;
            this.IsImit.Location = new System.Drawing.Point(280, 449);
            this.IsImit.Name = "IsImit";
            this.IsImit.Size = new System.Drawing.Size(160, 20);
            this.IsImit.TabIndex = 30;
            this.IsImit.Text = "Включить имитацию";
            this.IsImit.UseVisualStyleBackColor = true;
            this.IsImit.CheckedChanged += new System.EventHandler(this.IsImit_CheckedChanged);
            // 
            // ThreadId
            // 
            this.ThreadId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ThreadId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ThreadId.Enabled = false;
            this.ThreadId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ThreadId.Location = new System.Drawing.Point(199, 448);
            this.ThreadId.Name = "ThreadId";
            this.ThreadId.ReadOnly = true;
            this.ThreadId.Size = new System.Drawing.Size(57, 22);
            this.ThreadId.TabIndex = 25;
            // 
            // labelThreadId
            // 
            this.labelThreadId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelThreadId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelThreadId.Location = new System.Drawing.Point(6, 450);
            this.labelThreadId.Name = "labelThreadId";
            this.labelThreadId.Size = new System.Drawing.Size(189, 16);
            this.labelThreadId.TabIndex = 50;
            this.labelThreadId.Text = "Номер потока контроллера";
            this.labelThreadId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label11.Location = new System.Drawing.Point(622, 204);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(303, 15);
            this.label11.TabIndex = 45;
            this.label11.Text = "Настройка свойств провайдера двойным щелчком";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(3, 203);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 16);
            this.label10.TabIndex = 44;
            this.label10.Text = "Провайдеры";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 38;
            this.label1.Text = "Проекты";
            // 
            // AllowProjectsRepetitions
            // 
            this.AllowProjectsRepetitions.AutoSize = true;
            this.AllowProjectsRepetitions.Location = new System.Drawing.Point(678, 2);
            this.AllowProjectsRepetitions.Name = "AllowProjectsRepetitions";
            this.AllowProjectsRepetitions.Size = new System.Drawing.Size(256, 20);
            this.AllowProjectsRepetitions.TabIndex = 10;
            this.AllowProjectsRepetitions.Text = "Разрешить повтор кодов проектов";
            this.AllowProjectsRepetitions.UseVisualStyleBackColor = true;
            this.AllowProjectsRepetitions.CheckedChanged += new System.EventHandler(this.AllowProjectsRepetitions_CheckedChanged);
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabArchive);
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabReport);
            this.tabMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabMain.Location = new System.Drawing.Point(1, 1);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(950, 505);
            this.tabMain.TabIndex = 200;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ButServerReportFile);
            this.tabPage1.Controls.Add(this.ServerReport);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.FormTo);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(942, 476);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Файлы и каталоги";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // ButServerReportFile
            // 
            this.ButServerReportFile.Location = new System.Drawing.Point(581, 389);
            this.ButServerReportFile.Name = "ButServerReportFile";
            this.ButServerReportFile.Size = new System.Drawing.Size(75, 32);
            this.ButServerReportFile.TabIndex = 170;
            this.ButServerReportFile.Text = "Выбрать";
            this.ButServerReportFile.UseVisualStyleBackColor = true;
            this.ButServerReportFile.Click += new System.EventHandler(this.ButServerReport_Click);
            // 
            // ServerReport
            // 
            this.ServerReport.Location = new System.Drawing.Point(11, 394);
            this.ServerReport.Name = "ServerReport";
            this.ServerReport.Size = new System.Drawing.Size(570, 22);
            this.ServerReport.TabIndex = 165;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 375);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(318, 16);
            this.label5.TabIndex = 46;
            this.label5.Text = "Файл контрольного экземпляра бланка отчета";
            // 
            // FormTo
            // 
            this.FormTo.Controls.Add(this.SetFocusToFormed);
            this.FormTo.Controls.Add(this.AddSheetToName);
            this.FormTo.Controls.Add(this.EndNameFormat);
            this.FormTo.Controls.Add(this.AddEndToName);
            this.FormTo.Controls.Add(this.BeginNameFormat);
            this.FormTo.Controls.Add(this.AddBeginToName);
            this.FormTo.Controls.Add(this.DateNameFormat);
            this.FormTo.Controls.Add(this.ButCreateResultFile);
            this.FormTo.Controls.Add(this.AddCalcNameToName);
            this.FormTo.Controls.Add(this.ResultFileName);
            this.FormTo.Controls.Add(this.label4);
            this.FormTo.Controls.Add(this.label3);
            this.FormTo.Controls.Add(this.label2);
            this.FormTo.Controls.Add(this.AddDateToName);
            this.FormTo.Controls.Add(this.ButResultFile);
            this.FormTo.Controls.Add(this.ResultFile);
            this.FormTo.Controls.Add(this.ButResultDir);
            this.FormTo.Controls.Add(this.ResultDir);
            this.FormTo.Controls.Add(this.FormToFile);
            this.FormTo.Controls.Add(this.FormToDir);
            this.FormTo.Controls.Add(this.FormToTemplate);
            this.FormTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormTo.Location = new System.Drawing.Point(4, 6);
            this.FormTo.Name = "FormTo";
            this.FormTo.Size = new System.Drawing.Size(737, 356);
            this.FormTo.TabIndex = 45;
            this.FormTo.TabStop = false;
            this.FormTo.Text = "Формировать отчеты";
            // 
            // SetFocusToFormed
            // 
            this.SetFocusToFormed.AutoSize = true;
            this.SetFocusToFormed.Checked = true;
            this.SetFocusToFormed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SetFocusToFormed.Location = new System.Drawing.Point(12, 328);
            this.SetFocusToFormed.Name = "SetFocusToFormed";
            this.SetFocusToFormed.Size = new System.Drawing.Size(376, 20);
            this.SetFocusToFormed.TabIndex = 166;
            this.SetFocusToFormed.Text = "Переводить фокус на сформированный файл отчета";
            this.SetFocusToFormed.UseVisualStyleBackColor = true;
            // 
            // AddSheetToName
            // 
            this.AddSheetToName.AutoSize = true;
            this.AddSheetToName.Location = new System.Drawing.Point(253, 190);
            this.AddSheetToName.Name = "AddSheetToName";
            this.AddSheetToName.Size = new System.Drawing.Size(359, 20);
            this.AddSheetToName.TabIndex = 165;
            this.AddSheetToName.Text = "имя исходного листа (для файлов из одного листа)";
            this.AddSheetToName.UseVisualStyleBackColor = true;
            // 
            // EndNameFormat
            // 
            this.EndNameFormat.Items.AddRange(new object[] {
            "yyyy.MM.dd",
            "yyyy.MM.dd_HH",
            "yyyy.MM.dd_HH_mm",
            "yyyy.MM",
            "MM.dd",
            "dd.MM",
            "MM",
            "dd",
            "HH",
            "MM.dd_HH",
            "MM.dd_HH_mm",
            "dd.MM_HH",
            "dd.MM_HH_mm",
            "dd_HH",
            "dd_HH_mm",
            "HH_mm"});
            this.EndNameFormat.Location = new System.Drawing.Point(512, 294);
            this.EndNameFormat.Name = "EndNameFormat";
            this.EndNameFormat.Size = new System.Drawing.Size(160, 24);
            this.EndNameFormat.TabIndex = 164;
            this.EndNameFormat.Text = "yyyy.MM.dd";
            // 
            // AddEndToName
            // 
            this.AddEndToName.AutoSize = true;
            this.AddEndToName.Location = new System.Drawing.Point(253, 296);
            this.AddEndToName.Name = "AddEndToName";
            this.AddEndToName.Size = new System.Drawing.Size(247, 20);
            this.AddEndToName.TabIndex = 163;
            this.AddEndToName.Text = "конец периода отчета в формате";
            this.AddEndToName.UseVisualStyleBackColor = true;
            // 
            // BeginNameFormat
            // 
            this.BeginNameFormat.Items.AddRange(new object[] {
            "yyyy.MM.dd",
            "yyyy.MM.dd_HH",
            "yyyy.MM.dd_HH_mm",
            "yyyy.MM",
            "MM.dd",
            "dd.MM",
            "MM",
            "dd",
            "HH",
            "MM.dd_HH",
            "MM.dd_HH_mm",
            "dd.MM_HH",
            "dd.MM_HH_mm",
            "dd_HH",
            "dd_HH_mm",
            "HH_mm"});
            this.BeginNameFormat.Location = new System.Drawing.Point(512, 267);
            this.BeginNameFormat.Name = "BeginNameFormat";
            this.BeginNameFormat.Size = new System.Drawing.Size(160, 24);
            this.BeginNameFormat.TabIndex = 162;
            this.BeginNameFormat.Text = "yyyy.MM.dd";
            // 
            // AddBeginToName
            // 
            this.AddBeginToName.AutoSize = true;
            this.AddBeginToName.Location = new System.Drawing.Point(253, 269);
            this.AddBeginToName.Name = "AddBeginToName";
            this.AddBeginToName.Size = new System.Drawing.Size(256, 20);
            this.AddBeginToName.TabIndex = 161;
            this.AddBeginToName.Text = "начало периода отчета в формате";
            this.AddBeginToName.UseVisualStyleBackColor = true;
            // 
            // DateNameFormat
            // 
            this.DateNameFormat.Items.AddRange(new object[] {
            "yyyy.MM.dd",
            "yyyy.MM.dd_HH",
            "yyyy.MM.dd_HH_mm",
            "yyyy.MM",
            "MM.dd",
            "dd.MM",
            "MM",
            "dd",
            "HH",
            "MM.dd_HH",
            "MM.dd_HH_mm",
            "dd.MM_HH",
            "dd.MM_HH_mm",
            "dd_HH",
            "dd_HH_mm",
            "HH_mm"});
            this.DateNameFormat.Location = new System.Drawing.Point(512, 240);
            this.DateNameFormat.MaxDropDownItems = 30;
            this.DateNameFormat.Name = "DateNameFormat";
            this.DateNameFormat.Size = new System.Drawing.Size(160, 24);
            this.DateNameFormat.TabIndex = 155;
            this.DateNameFormat.Text = "yyyy.MM.dd";
            // 
            // ButCreateResultFile
            // 
            this.ButCreateResultFile.Location = new System.Drawing.Point(656, 147);
            this.ButCreateResultFile.Name = "ButCreateResultFile";
            this.ButCreateResultFile.Size = new System.Drawing.Size(75, 31);
            this.ButCreateResultFile.TabIndex = 145;
            this.ButCreateResultFile.Text = "Создать";
            this.ButCreateResultFile.UseVisualStyleBackColor = true;
            this.ButCreateResultFile.Click += new System.EventHandler(this.ButCreateResultFile_Click);
            // 
            // AddCalcNameToName
            // 
            this.AddCalcNameToName.AutoSize = true;
            this.AddCalcNameToName.Location = new System.Drawing.Point(253, 216);
            this.AddCalcNameToName.Name = "AddCalcNameToName";
            this.AddCalcNameToName.Size = new System.Drawing.Size(108, 20);
            this.AddCalcNameToName.TabIndex = 160;
            this.AddCalcNameToName.Text = "имя расчета";
            this.AddCalcNameToName.UseVisualStyleBackColor = true;
            // 
            // ResultFileName
            // 
            this.ResultFileName.Location = new System.Drawing.Point(206, 101);
            this.ResultFileName.Name = "ResultFileName";
            this.ResultFileName.Size = new System.Drawing.Size(192, 22);
            this.ResultFileName.TabIndex = 125;
            this.ResultFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(397, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 16);
            this.label4.TabIndex = 16;
            this.label4.Text = ".xlsx";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "Имя формируемого файла";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(230, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "Добавлять в имя файла или листа";
            // 
            // AddDateToName
            // 
            this.AddDateToName.AutoSize = true;
            this.AddDateToName.Location = new System.Drawing.Point(253, 242);
            this.AddDateToName.Name = "AddDateToName";
            this.AddDateToName.Size = new System.Drawing.Size(191, 20);
            this.AddDateToName.TabIndex = 150;
            this.AddDateToName.Text = "текущую дату в формате";
            this.AddDateToName.UseVisualStyleBackColor = true;
            this.AddDateToName.CheckedChanged += new System.EventHandler(this.AddDateToName_CheckedChanged);
            // 
            // ButResultFile
            // 
            this.ButResultFile.Location = new System.Drawing.Point(582, 147);
            this.ButResultFile.Name = "ButResultFile";
            this.ButResultFile.Size = new System.Drawing.Size(75, 31);
            this.ButResultFile.TabIndex = 140;
            this.ButResultFile.Text = "Выбрать";
            this.ButResultFile.UseVisualStyleBackColor = true;
            this.ButResultFile.Click += new System.EventHandler(this.ButResultFile_Click);
            // 
            // ResultFile
            // 
            this.ResultFile.Location = new System.Drawing.Point(12, 152);
            this.ResultFile.Name = "ResultFile";
            this.ResultFile.Size = new System.Drawing.Size(570, 22);
            this.ResultFile.TabIndex = 135;
            // 
            // ButResultDir
            // 
            this.ButResultDir.Location = new System.Drawing.Point(656, 70);
            this.ButResultDir.Name = "ButResultDir";
            this.ButResultDir.Size = new System.Drawing.Size(75, 31);
            this.ButResultDir.TabIndex = 120;
            this.ButResultDir.Text = "Выбрать";
            this.ButResultDir.UseVisualStyleBackColor = true;
            this.ButResultDir.Click += new System.EventHandler(this.ButResultDir_Click);
            // 
            // ResultDir
            // 
            this.ResultDir.Location = new System.Drawing.Point(12, 75);
            this.ResultDir.Name = "ResultDir";
            this.ResultDir.Size = new System.Drawing.Size(643, 22);
            this.ResultDir.TabIndex = 115;
            // 
            // FormToFile
            // 
            this.FormToFile.AutoSize = true;
            this.FormToFile.Location = new System.Drawing.Point(7, 128);
            this.FormToFile.Name = "FormToFile";
            this.FormToFile.Size = new System.Drawing.Size(258, 20);
            this.FormToFile.TabIndex = 130;
            this.FormToFile.Text = "новыми листами в указанный файл";
            this.FormToFile.UseVisualStyleBackColor = true;
            this.FormToFile.CheckedChanged += new System.EventHandler(this.FormToFile_CheckedChanged);
            // 
            // FormToDir
            // 
            this.FormToDir.AutoSize = true;
            this.FormToDir.Location = new System.Drawing.Point(7, 49);
            this.FormToDir.Name = "FormToDir";
            this.FormToDir.Size = new System.Drawing.Size(227, 20);
            this.FormToDir.TabIndex = 110;
            this.FormToDir.Text = "файлами в указанный каталог";
            this.FormToDir.UseVisualStyleBackColor = true;
            this.FormToDir.CheckedChanged += new System.EventHandler(this.FormToDir_CheckedChanged);
            // 
            // FormToTemplate
            // 
            this.FormToTemplate.AutoSize = true;
            this.FormToTemplate.Checked = true;
            this.FormToTemplate.Location = new System.Drawing.Point(7, 24);
            this.FormToTemplate.Name = "FormToTemplate";
            this.FormToTemplate.Size = new System.Drawing.Size(242, 20);
            this.FormToTemplate.TabIndex = 105;
            this.FormToTemplate.TabStop = true;
            this.FormToTemplate.Text = "непосредственно в файл бланка";
            this.FormToTemplate.UseVisualStyleBackColor = true;
            this.FormToTemplate.CheckedChanged += new System.EventHandler(this.FormToTemplate_CheckedChanged);
            // 
            // MonthLength
            // 
            this.MonthLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MonthLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MonthLength.Location = new System.Drawing.Point(12, 19);
            this.MonthLength.Name = "MonthLength";
            this.MonthLength.Size = new System.Drawing.Size(35, 22);
            this.MonthLength.TabIndex = 232;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(47, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 16);
            this.label12.TabIndex = 231;
            this.label12.Text = "месяцев";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DayStart
            // 
            this.DayStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DayStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DayStart.Location = new System.Drawing.Point(321, 245);
            this.DayStart.Name = "DayStart";
            this.DayStart.Size = new System.Drawing.Size(34, 21);
            this.DayStart.TabIndex = 257;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(358, 247);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(45, 16);
            this.label16.TabIndex = 256;
            this.label16.Text = "суток";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 552);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.tabMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSetupWin_FormClosed);
            this.Load += new System.EventHandler(this.FormSetupWin_Load);
            this.tabReport.ResumeLayout(false);
            this.tabReport.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.DefaultPeriod.ResumeLayout(false);
            this.DefaultPeriod.PerformLayout();
            this.tabArchive.ResumeLayout(false);
            this.tabArchive.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Projects)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Providers)).EndInit();
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.FormTo.ResumeLayout(false);
            this.FormTo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Button butCancel;
        private TabPage tabReport;
        private CheckBox DifferentBegin;
        private TextBox MinuteStart;
        private TextBox CalcName;
        private TextBox HourStart;
        private TextBox FileReport;
        private TextBox DescriptionReport;
        private TextBox NameReport;
        private TextBox CodeReport;
        private TextBox MinuteLength;
        private TextBox DayLength;
        private TextBox HourLength;
        private Label label21;
        private Label label17;
        private Label label24;
        private Label label18;
        private Label label14;
        private Label label7;
        private Label label8;
        private Label label9;
        private GroupBox DefaultPeriod;
        private RadioButton radioNow;
        private RadioButton radioPrevious;
        private Label label6;
        private Label label15;
        private Label label13;
        private TabPage tabArchive;
        private DataGridView Projects;
        private Label label1;
        private TabControl tabMain;
        private GroupBox groupBox1;
        private RadioButton radioEquals;
        private RadioButton radioLess;
        private RadioButton radioDifferent;
        private TabPage tabPage1;
        private GroupBox FormTo;
        private Button ButResultDir;
        private TextBox ResultDir;
        private RadioButton FormToFile;
        private RadioButton FormToDir;
        private RadioButton FormToTemplate;
        private Button ButResultFile;
        private TextBox ResultFile;
        private Label label2;
        private CheckBox AddDateToName;
        private TextBox ResultFileName;
        private Label label4;
        private Label label3;
        private CheckBox UseOneArchive;
        private DataGridView Providers;
        private ComboBox ImitMode;
        private CheckBox IsImit;
        private TextBox ThreadId;
        private Label labelThreadId;
        private Label label11;
        private Label label10;
        private CheckBox AddCalcNameToName;
        private Button ButCreateResultFile;
        private Button ButServerReportFile;
        private TextBox ServerReport;
        private Label label5;
        private CheckBox AllowProjectsRepetitions;
        private DataGridViewTextBoxColumn Project;
        private DataGridViewTextBoxColumn ProjectCode2;
        private DataGridViewTextBoxColumn ProjectName;
        private DataGridViewComboBoxColumn ProjectCalcMode;
        private DataGridViewTextBoxColumn ProjectFile;
        private DataGridViewButtonColumn Brouse;
        private ComboBox DateNameFormat;
        private DataGridViewTextBoxColumn TypeProvider;
        private DataGridViewTextBoxColumn ProviderCode;
        private DataGridViewTextBoxColumn ProviderName;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn ProviderInf;
        private ComboBox EndNameFormat;
        private CheckBox AddEndToName;
        private ComboBox BeginNameFormat;
        private CheckBox AddBeginToName;
        private CheckBox AddSheetToName;
        private CheckBox SetFocusToFormed;
        private TextBox MonthLength;
        private Label label12;
        private TextBox DayStart;
        private Label label16;
    }
}