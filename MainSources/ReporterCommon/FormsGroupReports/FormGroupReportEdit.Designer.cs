namespace ReporterCommon
{
    partial class FormGroupReportEdit
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGroupReportEdit));
            this.Reports = new System.Windows.Forms.DataGridView();
            this.Report = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReportName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReportFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ButReportFile = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ReportTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.butCancel = new System.Windows.Forms.Button();
            this.butOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.GroupCode = new System.Windows.Forms.TextBox();
            this.GroupName = new System.Windows.Forms.TextBox();
            this.ReportPanel = new System.Windows.Forms.Panel();
            this.ResultFileName = new System.Windows.Forms.TextBox();
            this.ResultDir = new System.Windows.Forms.TextBox();
            this.SelectedReportFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ButResultDir = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.SelectedReportName = new System.Windows.Forms.TextBox();
            this.SelectedReport = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.FormTo = new System.Windows.Forms.GroupBox();
            this.ResultFile = new System.Windows.Forms.TextBox();
            this.ButResultFile = new System.Windows.Forms.Button();
            this.AddToFile = new System.Windows.Forms.RadioButton();
            this.FormToFile = new System.Windows.Forms.RadioButton();
            this.FormToDir = new System.Windows.Forms.RadioButton();
            this.ButSetupReport = new System.Windows.Forms.Button();
            this.MinuteLength = new System.Windows.Forms.TextBox();
            this.HourLength = new System.Windows.Forms.TextBox();
            this.DayLength = new System.Windows.Forms.TextBox();
            this.SaveToArchive = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Reports)).BeginInit();
            this.ReportPanel.SuspendLayout();
            this.FormTo.SuspendLayout();
            this.SuspendLayout();
            // 
            // Reports
            // 
            this.Reports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Reports.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Reports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Reports.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Report,
            this.ReportName,
            this.ReportFile,
            this.ButReportFile,
            this.ReportTag});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Reports.DefaultCellStyle = dataGridViewCellStyle3;
            this.Reports.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Reports.Location = new System.Drawing.Point(2, 34);
            this.Reports.MultiSelect = false;
            this.Reports.Name = "Reports";
            this.Reports.RowHeadersWidth = 25;
            this.Reports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Reports.Size = new System.Drawing.Size(841, 237);
            this.Reports.TabIndex = 15;
            this.Reports.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Reports_CellContentClick);
            this.Reports.SelectionChanged += new System.EventHandler(this.Reports_SelectionChanged);
            // 
            // Report
            // 
            this.Report.HeaderText = "Код отчета";
            this.Report.Name = "Report";
            this.Report.ReadOnly = true;
            this.Report.Width = 150;
            // 
            // ReportName
            // 
            this.ReportName.HeaderText = "Имя отчета";
            this.ReportName.Name = "ReportName";
            this.ReportName.ReadOnly = true;
            this.ReportName.Width = 250;
            // 
            // ReportFile
            // 
            this.ReportFile.HeaderText = "Файл шаблона отчета";
            this.ReportFile.Name = "ReportFile";
            this.ReportFile.ReadOnly = true;
            this.ReportFile.Width = 390;
            // 
            // ButReportFile
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "...";
            this.ButReportFile.DefaultCellStyle = dataGridViewCellStyle2;
            this.ButReportFile.HeaderText = "";
            this.ButReportFile.MinimumWidth = 20;
            this.ButReportFile.Name = "ButReportFile";
            this.ButReportFile.Text = "";
            this.ButReportFile.ToolTipText = "Выбрать файл шаблона отчета";
            this.ButReportFile.Width = 20;
            // 
            // ReportTag
            // 
            this.ReportTag.HeaderText = "ReportTag";
            this.ReportTag.Name = "ReportTag";
            this.ReportTag.ReadOnly = true;
            this.ReportTag.Visible = false;
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCancel.Location = new System.Drawing.Point(421, 552);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(181, 37);
            this.butCancel.TabIndex = 100;
            this.butCancel.Text = "Отмена";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butOK.Location = new System.Drawing.Point(202, 552);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(181, 37);
            this.butOK.TabIndex = 90;
            this.butOK.Text = "Сохранить";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 16);
            this.label2.TabIndex = 55;
            this.label2.Text = "Имя";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(277, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 16);
            this.label3.TabIndex = 56;
            this.label3.Text = "Описание";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GroupCode
            // 
            this.GroupCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GroupCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GroupCode.Location = new System.Drawing.Point(44, 5);
            this.GroupCode.Name = "GroupCode";
            this.GroupCode.Size = new System.Drawing.Size(228, 22);
            this.GroupCode.TabIndex = 5;
            // 
            // GroupName
            // 
            this.GroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GroupName.Location = new System.Drawing.Point(360, 5);
            this.GroupName.Multiline = true;
            this.GroupName.Name = "GroupName";
            this.GroupName.Size = new System.Drawing.Size(483, 23);
            this.GroupName.TabIndex = 10;
            // 
            // ReportPanel
            // 
            this.ReportPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReportPanel.Controls.Add(this.ResultFileName);
            this.ReportPanel.Controls.Add(this.ResultDir);
            this.ReportPanel.Controls.Add(this.SelectedReportFile);
            this.ReportPanel.Controls.Add(this.label5);
            this.ReportPanel.Controls.Add(this.label6);
            this.ReportPanel.Controls.Add(this.ButResultDir);
            this.ReportPanel.Controls.Add(this.label9);
            this.ReportPanel.Controls.Add(this.SelectedReportName);
            this.ReportPanel.Controls.Add(this.SelectedReport);
            this.ReportPanel.Controls.Add(this.label4);
            this.ReportPanel.Controls.Add(this.FormTo);
            this.ReportPanel.Controls.Add(this.ButSetupReport);
            this.ReportPanel.Controls.Add(this.MinuteLength);
            this.ReportPanel.Controls.Add(this.HourLength);
            this.ReportPanel.Controls.Add(this.DayLength);
            this.ReportPanel.Controls.Add(this.SaveToArchive);
            this.ReportPanel.Controls.Add(this.label1);
            this.ReportPanel.Controls.Add(this.label15);
            this.ReportPanel.Controls.Add(this.label13);
            this.ReportPanel.Controls.Add(this.label14);
            this.ReportPanel.Controls.Add(this.label10);
            this.ReportPanel.Controls.Add(this.label7);
            this.ReportPanel.Controls.Add(this.label8);
            this.ReportPanel.Location = new System.Drawing.Point(1, 275);
            this.ReportPanel.Name = "ReportPanel";
            this.ReportPanel.Size = new System.Drawing.Size(842, 273);
            this.ReportPanel.TabIndex = 20;
            // 
            // ResultFileName
            // 
            this.ResultFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ResultFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultFileName.Location = new System.Drawing.Point(256, 141);
            this.ResultFileName.Name = "ResultFileName";
            this.ResultFileName.Size = new System.Drawing.Size(202, 22);
            this.ResultFileName.TabIndex = 65;
            this.ResultFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ResultDir
            // 
            this.ResultDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ResultDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultDir.Location = new System.Drawing.Point(256, 113);
            this.ResultDir.Name = "ResultDir";
            this.ResultDir.Size = new System.Drawing.Size(505, 22);
            this.ResultDir.TabIndex = 60;
            // 
            // SelectedReportFile
            // 
            this.SelectedReportFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectedReportFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SelectedReportFile.Location = new System.Drawing.Point(48, 24);
            this.SelectedReportFile.Name = "SelectedReportFile";
            this.SelectedReportFile.ReadOnly = true;
            this.SelectedReportFile.Size = new System.Drawing.Size(790, 22);
            this.SelectedReportFile.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(457, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(322, 16);
            this.label5.TabIndex = 76;
            this.label5.Text = ".xlsx   (к имени файла добавляется дата и время)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(7, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(249, 16);
            this.label6.TabIndex = 74;
            this.label6.Text = "Основа имени формируемого файла";
            // 
            // ButResultDir
            // 
            this.ButResultDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButResultDir.Location = new System.Drawing.Point(763, 108);
            this.ButResultDir.Name = "ButResultDir";
            this.ButResultDir.Size = new System.Drawing.Size(75, 31);
            this.ButResultDir.TabIndex = 62;
            this.ButResultDir.Text = "Выбрать";
            this.ButResultDir.UseVisualStyleBackColor = true;
            this.ButResultDir.Click += new System.EventHandler(this.ButResultDir_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(8, 115);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(235, 16);
            this.label9.TabIndex = 71;
            this.label9.Text = "Каталог сформированных отчетов";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SelectedReportName
            // 
            this.SelectedReportName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectedReportName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SelectedReportName.Location = new System.Drawing.Point(381, 1);
            this.SelectedReportName.Name = "SelectedReportName";
            this.SelectedReportName.ReadOnly = true;
            this.SelectedReportName.Size = new System.Drawing.Size(457, 22);
            this.SelectedReportName.TabIndex = 27;
            // 
            // SelectedReport
            // 
            this.SelectedReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectedReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SelectedReport.Location = new System.Drawing.Point(177, 1);
            this.SelectedReport.Name = "SelectedReport";
            this.SelectedReport.ReadOnly = true;
            this.SelectedReport.Size = new System.Drawing.Size(167, 22);
            this.SelectedReport.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(4, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 16);
            this.label4.TabIndex = 67;
            this.label4.Text = "Выбранный отчет";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormTo
            // 
            this.FormTo.Controls.Add(this.ResultFile);
            this.FormTo.Controls.Add(this.ButResultFile);
            this.FormTo.Controls.Add(this.AddToFile);
            this.FormTo.Controls.Add(this.FormToFile);
            this.FormTo.Controls.Add(this.FormToDir);
            this.FormTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormTo.Location = new System.Drawing.Point(5, 166);
            this.FormTo.Name = "FormTo";
            this.FormTo.Size = new System.Drawing.Size(833, 102);
            this.FormTo.TabIndex = 70;
            this.FormTo.TabStop = false;
            this.FormTo.Text = "Формировать";
            // 
            // ResultFile
            // 
            this.ResultFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ResultFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultFile.Location = new System.Drawing.Point(330, 73);
            this.ResultFile.Name = "ResultFile";
            this.ResultFile.Size = new System.Drawing.Size(426, 22);
            this.ResultFile.TabIndex = 82;
            // 
            // ButResultFile
            // 
            this.ButResultFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButResultFile.Location = new System.Drawing.Point(758, 68);
            this.ButResultFile.Name = "ButResultFile";
            this.ButResultFile.Size = new System.Drawing.Size(75, 31);
            this.ButResultFile.TabIndex = 85;
            this.ButResultFile.Text = "Выбрать";
            this.ButResultFile.UseVisualStyleBackColor = true;
            this.ButResultFile.Click += new System.EventHandler(this.ButResultFile_Click);
            // 
            // AddToFile
            // 
            this.AddToFile.AutoSize = true;
            this.AddToFile.Location = new System.Drawing.Point(7, 74);
            this.AddToFile.Name = "AddToFile";
            this.AddToFile.Size = new System.Drawing.Size(325, 20);
            this.AddToFile.TabIndex = 80;
            this.AddToFile.Text = "добавлять дополнительными листами в файл";
            this.AddToFile.UseVisualStyleBackColor = true;
            // 
            // FormToFile
            // 
            this.FormToFile.AutoSize = true;
            this.FormToFile.Location = new System.Drawing.Point(7, 48);
            this.FormToFile.Name = "FormToFile";
            this.FormToFile.Size = new System.Drawing.Size(661, 20);
            this.FormToFile.TabIndex = 78;
            this.FormToFile.Text = "отчеты сформированные по одному бланку за одно формирование группы отчетов в один" +
    " файл";
            this.FormToFile.UseVisualStyleBackColor = true;
            // 
            // FormToDir
            // 
            this.FormToDir.AutoSize = true;
            this.FormToDir.Checked = true;
            this.FormToDir.Location = new System.Drawing.Point(7, 22);
            this.FormToDir.Name = "FormToDir";
            this.FormToDir.Size = new System.Drawing.Size(358, 20);
            this.FormToDir.TabIndex = 75;
            this.FormToDir.TabStop = true;
            this.FormToDir.Text = "каждый сформированный отчет в отдельный файл";
            this.FormToDir.UseVisualStyleBackColor = true;
            // 
            // ButSetupReport
            // 
            this.ButSetupReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButSetupReport.Location = new System.Drawing.Point(663, 52);
            this.ButSetupReport.Name = "ButSetupReport";
            this.ButSetupReport.Size = new System.Drawing.Size(174, 52);
            this.ButSetupReport.TabIndex = 50;
            this.ButSetupReport.Text = "Настройка отчета";
            this.ButSetupReport.UseVisualStyleBackColor = true;
            this.ButSetupReport.Click += new System.EventHandler(this.ButSetupReport_Click);
            // 
            // MinuteLength
            // 
            this.MinuteLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MinuteLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinuteLength.Location = new System.Drawing.Point(381, 78);
            this.MinuteLength.Name = "MinuteLength";
            this.MinuteLength.Size = new System.Drawing.Size(35, 22);
            this.MinuteLength.TabIndex = 45;
            // 
            // HourLength
            // 
            this.HourLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HourLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HourLength.Location = new System.Drawing.Point(299, 78);
            this.HourLength.Name = "HourLength";
            this.HourLength.Size = new System.Drawing.Size(35, 22);
            this.HourLength.TabIndex = 42;
            // 
            // DayLength
            // 
            this.DayLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DayLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DayLength.Location = new System.Drawing.Point(220, 78);
            this.DayLength.Name = "DayLength";
            this.DayLength.Size = new System.Drawing.Size(35, 22);
            this.DayLength.TabIndex = 40;
            // 
            // SaveToArchive
            // 
            this.SaveToArchive.Checked = true;
            this.SaveToArchive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SaveToArchive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SaveToArchive.Location = new System.Drawing.Point(11, 52);
            this.SaveToArchive.Name = "SaveToArchive";
            this.SaveToArchive.Size = new System.Drawing.Size(462, 25);
            this.SaveToArchive.TabIndex = 35;
            this.SaveToArchive.Text = "Автоматически сохранять в журнал отчетов при формировании";
            this.SaveToArchive.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(8, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 16);
            this.label1.TabIndex = 56;
            this.label1.Text = "Длина периода одного отчета";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.Location = new System.Drawing.Point(416, 80);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 16);
            this.label15.TabIndex = 55;
            this.label15.Text = "минут";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(255, 80);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 16);
            this.label13.TabIndex = 51;
            this.label13.Text = "суток";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.Location = new System.Drawing.Point(334, 80);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 16);
            this.label14.TabIndex = 53;
            this.label14.Text = "часов";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(142, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 16);
            this.label10.TabIndex = 79;
            this.label10.Text = "Код";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(4, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 16);
            this.label7.TabIndex = 77;
            this.label7.Text = "Файл";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(347, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 16);
            this.label8.TabIndex = 69;
            this.label8.Text = "Имя";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormGroupReportEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 593);
            this.ControlBox = false;
            this.Controls.Add(this.GroupCode);
            this.Controls.Add(this.GroupName);
            this.Controls.Add(this.ReportPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.Reports);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGroupReportEdit";
            this.Text = "Редактирование группы отчетов";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGroupReportEdit_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.Reports)).EndInit();
            this.ReportPanel.ResumeLayout(false);
            this.ReportPanel.PerformLayout();
            this.FormTo.ResumeLayout(false);
            this.FormTo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Reports;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox GroupCode;
        private System.Windows.Forms.TextBox GroupName;
        private System.Windows.Forms.Panel ReportPanel;
        private System.Windows.Forms.CheckBox SaveToArchive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox HourLength;
        private System.Windows.Forms.TextBox DayLength;
        private System.Windows.Forms.TextBox MinuteLength;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button ButSetupReport;
        private System.Windows.Forms.TextBox SelectedReportName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox SelectedReport;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox FormTo;
        private System.Windows.Forms.RadioButton FormToFile;
        private System.Windows.Forms.RadioButton FormToDir;
        private System.Windows.Forms.TextBox ResultFileName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button ButResultDir;
        private System.Windows.Forms.TextBox ResultDir;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox SelectedReportFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button ButResultFile;
        private System.Windows.Forms.TextBox ResultFile;
        private System.Windows.Forms.RadioButton AddToFile;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Report;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReportName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReportFile;
        private System.Windows.Forms.DataGridViewButtonColumn ButReportFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReportTag;
    }
}