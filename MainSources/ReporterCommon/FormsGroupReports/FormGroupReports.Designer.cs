namespace ReporterCommon
{
    partial class FormGroupReports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGroupReports));
            this.GroupReports = new System.Windows.Forms.ListView();
            this.GroupCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GroupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BeginPeriod = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.EndPeriod = new System.Windows.Forms.TextBox();
            this.ButFormReports = new System.Windows.Forms.Button();
            this.ButEdit = new System.Windows.Forms.Button();
            this.ButAdd = new System.Windows.Forms.Button();
            this.ButDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GroupReports
            // 
            this.GroupReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupReports.CheckBoxes = true;
            this.GroupReports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GroupCode,
            this.GroupName});
            this.GroupReports.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GroupReports.FullRowSelect = true;
            this.GroupReports.GridLines = true;
            this.GroupReports.Location = new System.Drawing.Point(-1, 50);
            this.GroupReports.MultiSelect = false;
            this.GroupReports.Name = "GroupReports";
            this.GroupReports.Size = new System.Drawing.Size(712, 338);
            this.GroupReports.TabIndex = 35;
            this.GroupReports.UseCompatibleStateImageBehavior = false;
            this.GroupReports.View = System.Windows.Forms.View.Details;
            this.GroupReports.DoubleClick += new System.EventHandler(this.GroupReports_DoubleClick);
            // 
            // GroupCode
            // 
            this.GroupCode.Text = "Группа отчетов";
            this.GroupCode.Width = 251;
            // 
            // GroupName
            // 
            this.GroupName.Text = "Описание";
            this.GroupName.Width = 457;
            // 
            // BeginPeriod
            // 
            this.BeginPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BeginPeriod.Location = new System.Drawing.Point(83, 4);
            this.BeginPeriod.Name = "BeginPeriod";
            this.BeginPeriod.Size = new System.Drawing.Size(152, 22);
            this.BeginPeriod.TabIndex = 5;
            this.BeginPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Период от";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(55, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "до";
            // 
            // EndPeriod
            // 
            this.EndPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EndPeriod.Location = new System.Drawing.Point(83, 25);
            this.EndPeriod.Name = "EndPeriod";
            this.EndPeriod.Size = new System.Drawing.Size(152, 22);
            this.EndPeriod.TabIndex = 10;
            this.EndPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ButFormReports
            // 
            this.ButFormReports.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButFormReports.Location = new System.Drawing.Point(246, 4);
            this.ButFormReports.Name = "ButFormReports";
            this.ButFormReports.Size = new System.Drawing.Size(228, 44);
            this.ButFormReports.TabIndex = 15;
            this.ButFormReports.Text = "Сформировать отмеченные группы отчетов";
            this.ButFormReports.UseVisualStyleBackColor = true;
            this.ButFormReports.Click += new System.EventHandler(this.ButFormReports_Click);
            // 
            // ButEdit
            // 
            this.ButEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButEdit.Location = new System.Drawing.Point(474, 4);
            this.ButEdit.Name = "ButEdit";
            this.ButEdit.Size = new System.Drawing.Size(86, 44);
            this.ButEdit.TabIndex = 20;
            this.ButEdit.Text = "Изменить";
            this.ButEdit.UseVisualStyleBackColor = true;
            this.ButEdit.Click += new System.EventHandler(this.ButEdit_Click);
            // 
            // ButAdd
            // 
            this.ButAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButAdd.Location = new System.Drawing.Point(560, 4);
            this.ButAdd.Name = "ButAdd";
            this.ButAdd.Size = new System.Drawing.Size(79, 44);
            this.ButAdd.TabIndex = 25;
            this.ButAdd.Text = "Добавить";
            this.ButAdd.UseVisualStyleBackColor = true;
            this.ButAdd.Click += new System.EventHandler(this.ButAdd_Click);
            // 
            // ButDelete
            // 
            this.ButDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButDelete.Location = new System.Drawing.Point(639, 4);
            this.ButDelete.Name = "ButDelete";
            this.ButDelete.Size = new System.Drawing.Size(72, 44);
            this.ButDelete.TabIndex = 30;
            this.ButDelete.Text = "Удалить";
            this.ButDelete.UseVisualStyleBackColor = true;
            this.ButDelete.Click += new System.EventHandler(this.ButDelete_Click);
            // 
            // FormGroupReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 388);
            this.Controls.Add(this.ButDelete);
            this.Controls.Add(this.ButAdd);
            this.Controls.Add(this.ButEdit);
            this.Controls.Add(this.ButFormReports);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EndPeriod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BeginPeriod);
            this.Controls.Add(this.GroupReports);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormGroupReports";
            this.Text = "Групповое формирование отчетов";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormGroupReportWin_FormClosed);
            this.Load += new System.EventHandler(this.FormGroupReports_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BeginPeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox EndPeriod;
        private System.Windows.Forms.Button ButFormReports;
        private System.Windows.Forms.Button ButEdit;
        private System.Windows.Forms.Button ButAdd;
        private System.Windows.Forms.Button ButDelete;
        private System.Windows.Forms.ColumnHeader GroupCode;
        private System.Windows.Forms.ColumnHeader GroupName;
        public System.Windows.Forms.ListView GroupReports;

    }
}