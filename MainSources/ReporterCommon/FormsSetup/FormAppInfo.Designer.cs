namespace ReporterCommon
{
    partial class FormAppInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAppInfo));
            this.label1 = new System.Windows.Forms.Label();
            this.ProgrammInfo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AppVersion = new System.Windows.Forms.Label();
            this.AppUserOrg = new System.Windows.Forms.Label();
            this.ButBase = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ButReporter = new System.Windows.Forms.Button();
            this.ButController = new System.Windows.Forms.Button();
            this.LicenseNumber = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(47, 9);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(572, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Програмный комплекс реализации расчетно-аналитических задач InfoTask";
            // 
            // ProgrammInfo
            // 
            this.ProgrammInfo.AutoSize = true;
            this.ProgrammInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProgrammInfo.Location = new System.Drawing.Point(47, 29);
            this.ProgrammInfo.Name = "ProgrammInfo";
            this.ProgrammInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ProgrammInfo.Size = new System.Drawing.Size(284, 16);
            this.ProgrammInfo.TabIndex = 2;
            this.ProgrammInfo.Text = "Построитель отчетов Excel (ReporterExcel)";
            this.ProgrammInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ProgrammInfo.UseMnemonic = false;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(10, 58);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(68, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Версия:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Location = new System.Drawing.Point(10, 104);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(124, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Пользователь:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AppVersion
            // 
            this.AppVersion.AutoSize = true;
            this.AppVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AppVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AppVersion.Location = new System.Drawing.Point(70, 58);
            this.AppVersion.Name = "AppVersion";
            this.AppVersion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.AppVersion.Size = new System.Drawing.Size(26, 16);
            this.AppVersion.TabIndex = 5;
            this.AppVersion.Text = "***";
            // 
            // AppUserOrg
            // 
            this.AppUserOrg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AppUserOrg.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AppUserOrg.Location = new System.Drawing.Point(125, 104);
            this.AppUserOrg.Name = "AppUserOrg";
            this.AppUserOrg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.AppUserOrg.Size = new System.Drawing.Size(491, 16);
            this.AppUserOrg.TabIndex = 8;
            this.AppUserOrg.Text = "***";
            // 
            // ButBase
            // 
            this.ButBase.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.ButBase.FlatAppearance.BorderSize = 0;
            this.ButBase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButBase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButBase.ForeColor = System.Drawing.Color.Navy;
            this.ButBase.Image = global::ReporterCommon.Properties.Resources.help;
            this.ButBase.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButBase.Location = new System.Drawing.Point(4, 130);
            this.ButBase.Name = "ButBase";
            this.ButBase.Size = new System.Drawing.Size(612, 38);
            this.ButBase.TabIndex = 10;
            this.ButBase.Text = "  InfoTask-UG.01-Base - Общее описание программного комплекса InfoTask";
            this.ButBase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButBase.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButBase.UseVisualStyleBackColor = true;
            this.ButBase.Click += new System.EventHandler(this.ButBase_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ReporterCommon.Properties.Resources.ITicon32_R;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(8, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(33, 32);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ButReporter
            // 
            this.ButReporter.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.ButReporter.FlatAppearance.BorderSize = 0;
            this.ButReporter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButReporter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButReporter.ForeColor = System.Drawing.Color.Navy;
            this.ButReporter.Image = global::ReporterCommon.Properties.Resources.help;
            this.ButReporter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButReporter.Location = new System.Drawing.Point(4, 171);
            this.ButReporter.Name = "ButReporter";
            this.ButReporter.Size = new System.Drawing.Size(612, 38);
            this.ButReporter.TabIndex = 15;
            this.ButReporter.Text = "  InfoTask-UG.05-Reporter - Построитель отчетов";
            this.ButReporter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButReporter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButReporter.UseVisualStyleBackColor = true;
            this.ButReporter.Click += new System.EventHandler(this.ButReporter_Click);
            // 
            // ButController
            // 
            this.ButController.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.ButController.FlatAppearance.BorderSize = 0;
            this.ButController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButController.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButController.ForeColor = System.Drawing.Color.Navy;
            this.ButController.Image = global::ReporterCommon.Properties.Resources.help;
            this.ButController.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButController.Location = new System.Drawing.Point(4, 212);
            this.ButController.Name = "ButController";
            this.ButController.Size = new System.Drawing.Size(612, 38);
            this.ButController.TabIndex = 20;
            this.ButController.Text = "  InfoTask-UG.04-Controller - Контроллер расчетов. Монитор расчетов";
            this.ButController.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButController.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ButController.UseVisualStyleBackColor = true;
            this.ButController.Click += new System.EventHandler(this.ButController_Click);
            // 
            // LicenseNumber
            // 
            this.LicenseNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LicenseNumber.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LicenseNumber.Location = new System.Drawing.Point(142, 80);
            this.LicenseNumber.Name = "LicenseNumber";
            this.LicenseNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LicenseNumber.Size = new System.Drawing.Size(474, 16);
            this.LicenseNumber.TabIndex = 7;
            this.LicenseNumber.Text = "***";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Location = new System.Drawing.Point(10, 80);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(144, 16);
            this.label5.TabIndex = 11;
            this.label5.Text = "Номер лицензии:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormAppInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 257);
            this.Controls.Add(this.LicenseNumber);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ButController);
            this.Controls.Add(this.ButReporter);
            this.Controls.Add(this.ButBase);
            this.Controls.Add(this.AppUserOrg);
            this.Controls.Add(this.AppVersion);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ProgrammInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAppInfo";
            this.Text = "InfoTask";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAppInfo_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label ProgrammInfo;
        public System.Windows.Forms.Label AppVersion;
        public System.Windows.Forms.Label AppUserOrg;
        private System.Windows.Forms.Button ButBase;
        private System.Windows.Forms.Button ButReporter;
        private System.Windows.Forms.Button ButController;
        public System.Windows.Forms.Label LicenseNumber;
        private System.Windows.Forms.Label label5;
    }
}