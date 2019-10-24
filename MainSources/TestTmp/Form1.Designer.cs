namespace TestTmp
{
    partial class Form1
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
            this.butTestCloner = new System.Windows.Forms.Button();
            this.butTestAnalizer = new System.Windows.Forms.Button();
            this.cbComm = new System.Windows.Forms.ComboBox();
            this.gbCompile = new System.Windows.Forms.GroupBox();
            this.butChangeProject = new System.Windows.Forms.Button();
            this.butTestCompile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbProjectFile = new System.Windows.Forms.TextBox();
            this.butTest = new System.Windows.Forms.Button();
            this.gbAnalizer = new System.Windows.Forms.GroupBox();
            this.gbCloner = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpTimeBegin = new System.Windows.Forms.DateTimePicker();
            this.butSelectCloneFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbCloneFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbCompile.SuspendLayout();
            this.gbAnalizer.SuspendLayout();
            this.gbCloner.SuspendLayout();
            this.SuspendLayout();
            // 
            // butTestCloner
            // 
            this.butTestCloner.Location = new System.Drawing.Point(81, 98);
            this.butTestCloner.Name = "butTestCloner";
            this.butTestCloner.Size = new System.Drawing.Size(150, 30);
            this.butTestCloner.TabIndex = 3;
            this.butTestCloner.Text = "TestCloner";
            this.butTestCloner.UseVisualStyleBackColor = true;
            this.butTestCloner.Click += new System.EventHandler(this.butTestCloner_Click);
            // 
            // butTestAnalizer
            // 
            this.butTestAnalizer.Location = new System.Drawing.Point(68, 19);
            this.butTestAnalizer.Name = "butTestAnalizer";
            this.butTestAnalizer.Size = new System.Drawing.Size(89, 30);
            this.butTestAnalizer.TabIndex = 6;
            this.butTestAnalizer.Text = "Test Analizer";
            this.butTestAnalizer.UseVisualStyleBackColor = true;
            this.butTestAnalizer.Click += new System.EventHandler(this.butTestAnalizer_Click);
            // 
            // cbComm
            // 
            this.cbComm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComm.FormattingEnabled = true;
            this.cbComm.Items.AddRange(new object[] {
            "OvationComm",
            "KosmotronikaDbfComm"});
            this.cbComm.Location = new System.Drawing.Point(81, 19);
            this.cbComm.Name = "cbComm";
            this.cbComm.Size = new System.Drawing.Size(150, 21);
            this.cbComm.TabIndex = 7;
            // 
            // gbCompile
            // 
            this.gbCompile.Controls.Add(this.butChangeProject);
            this.gbCompile.Controls.Add(this.butTestCompile);
            this.gbCompile.Controls.Add(this.label1);
            this.gbCompile.Controls.Add(this.tbProjectFile);
            this.gbCompile.Controls.Add(this.butTest);
            this.gbCompile.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbCompile.Location = new System.Drawing.Point(0, 0);
            this.gbCompile.Name = "gbCompile";
            this.gbCompile.Size = new System.Drawing.Size(584, 86);
            this.gbCompile.TabIndex = 8;
            this.gbCompile.TabStop = false;
            this.gbCompile.Text = "Test Compile";
            // 
            // butChangeProject
            // 
            this.butChangeProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butChangeProject.Location = new System.Drawing.Point(524, 19);
            this.butChangeProject.Name = "butChangeProject";
            this.butChangeProject.Size = new System.Drawing.Size(48, 28);
            this.butChangeProject.TabIndex = 10;
            this.butChangeProject.Text = "...";
            this.butChangeProject.UseVisualStyleBackColor = true;
            // 
            // butTestCompile
            // 
            this.butTestCompile.Location = new System.Drawing.Point(163, 50);
            this.butTestCompile.Name = "butTestCompile";
            this.butTestCompile.Size = new System.Drawing.Size(90, 30);
            this.butTestCompile.TabIndex = 9;
            this.butTestCompile.Text = "Test Compile";
            this.butTestCompile.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "ProjectFile";
            // 
            // tbProjectFile
            // 
            this.tbProjectFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProjectFile.Location = new System.Drawing.Point(68, 24);
            this.tbProjectFile.Name = "tbProjectFile";
            this.tbProjectFile.Size = new System.Drawing.Size(450, 20);
            this.tbProjectFile.TabIndex = 7;
            // 
            // butTest
            // 
            this.butTest.Location = new System.Drawing.Point(68, 50);
            this.butTest.Name = "butTest";
            this.butTest.Size = new System.Drawing.Size(89, 30);
            this.butTest.TabIndex = 6;
            this.butTest.Text = "Test";
            this.butTest.UseVisualStyleBackColor = true;
            // 
            // gbAnalizer
            // 
            this.gbAnalizer.Controls.Add(this.butTestAnalizer);
            this.gbAnalizer.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbAnalizer.Location = new System.Drawing.Point(0, 86);
            this.gbAnalizer.Name = "gbAnalizer";
            this.gbAnalizer.Size = new System.Drawing.Size(584, 55);
            this.gbAnalizer.TabIndex = 9;
            this.gbAnalizer.TabStop = false;
            this.gbAnalizer.Text = "Test Analizer";
            // 
            // gbCloner
            // 
            this.gbCloner.Controls.Add(this.label5);
            this.gbCloner.Controls.Add(this.dtpTimeEnd);
            this.gbCloner.Controls.Add(this.label4);
            this.gbCloner.Controls.Add(this.dtpTimeBegin);
            this.gbCloner.Controls.Add(this.butSelectCloneFile);
            this.gbCloner.Controls.Add(this.label3);
            this.gbCloner.Controls.Add(this.tbCloneFile);
            this.gbCloner.Controls.Add(this.label2);
            this.gbCloner.Controls.Add(this.cbComm);
            this.gbCloner.Controls.Add(this.butTestCloner);
            this.gbCloner.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbCloner.Location = new System.Drawing.Point(0, 141);
            this.gbCloner.Name = "gbCloner";
            this.gbCloner.Size = new System.Drawing.Size(584, 134);
            this.gbCloner.TabIndex = 10;
            this.gbCloner.TabStop = false;
            this.gbCloner.Text = "Test Cloner";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(307, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Time End:";
            // 
            // dtpTimeEnd
            // 
            this.dtpTimeEnd.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.dtpTimeEnd.CustomFormat = "yyyy.MM.dd HH:mm:ss";
            this.dtpTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTimeEnd.Location = new System.Drawing.Point(368, 72);
            this.dtpTimeEnd.Name = "dtpTimeEnd";
            this.dtpTimeEnd.ShowUpDown = true;
            this.dtpTimeEnd.Size = new System.Drawing.Size(150, 20);
            this.dtpTimeEnd.TabIndex = 16;
            this.dtpTimeEnd.UseWaitCursor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Time Begin:";
            // 
            // dtpTimeBegin
            // 
            this.dtpTimeBegin.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.dtpTimeBegin.CustomFormat = "yyyy.MM.dd HH:mm:ss";
            this.dtpTimeBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTimeBegin.Location = new System.Drawing.Point(81, 72);
            this.dtpTimeBegin.Name = "dtpTimeBegin";
            this.dtpTimeBegin.ShowUpDown = true;
            this.dtpTimeBegin.Size = new System.Drawing.Size(150, 20);
            this.dtpTimeBegin.TabIndex = 14;
            this.dtpTimeBegin.UseWaitCursor = true;
            // 
            // butSelectCloneFile
            // 
            this.butSelectCloneFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectCloneFile.Location = new System.Drawing.Point(524, 41);
            this.butSelectCloneFile.Name = "butSelectCloneFile";
            this.butSelectCloneFile.Size = new System.Drawing.Size(48, 28);
            this.butSelectCloneFile.TabIndex = 13;
            this.butSelectCloneFile.Text = "...";
            this.butSelectCloneFile.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Clone File:";
            // 
            // tbCloneFile
            // 
            this.tbCloneFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCloneFile.Location = new System.Drawing.Point(81, 46);
            this.tbCloneFile.Name = "tbCloneFile";
            this.tbCloneFile.Size = new System.Drawing.Size(437, 20);
            this.tbCloneFile.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Comm:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 280);
            this.Controls.Add(this.gbCloner);
            this.Controls.Add(this.gbAnalizer);
            this.Controls.Add(this.gbCompile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.gbCompile.ResumeLayout(false);
            this.gbCompile.PerformLayout();
            this.gbAnalizer.ResumeLayout(false);
            this.gbCloner.ResumeLayout(false);
            this.gbCloner.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butTestCloner;
        private System.Windows.Forms.Button butTestAnalizer;
        private System.Windows.Forms.ComboBox cbComm;
        private System.Windows.Forms.GroupBox gbCompile;
        private System.Windows.Forms.Button butChangeProject;
        private System.Windows.Forms.Button butTestCompile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbProjectFile;
        private System.Windows.Forms.Button butTest;
        private System.Windows.Forms.GroupBox gbAnalizer;
        private System.Windows.Forms.GroupBox gbCloner;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpTimeBegin;
        private System.Windows.Forms.Button butSelectCloneFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbCloneFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpTimeEnd;
    }
}

