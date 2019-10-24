namespace TestProviderOvation
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
            this.button1 = new System.Windows.Forms.Button();
            this.dtTimeBegin = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.tbCloneFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 227);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Fight";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dtTimeBegin
            // 
            this.dtTimeBegin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtTimeBegin.CustomFormat = "MMMMdd, yyyy  |  HH:mm:ss";
            this.dtTimeBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTimeBegin.Location = new System.Drawing.Point(75, 12);
            this.dtTimeBegin.Name = "dtTimeBegin";
            this.dtTimeBegin.Size = new System.Drawing.Size(197, 20);
            this.dtTimeBegin.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TimeBegin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "TimeEnd";
            // 
            // dtTimeEnd
            // 
            this.dtTimeEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtTimeEnd.CustomFormat = "MMMMdd, yyyy  |  HH:mm:ss";
            this.dtTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTimeEnd.Location = new System.Drawing.Point(75, 38);
            this.dtTimeEnd.Name = "dtTimeEnd";
            this.dtTimeEnd.Size = new System.Drawing.Size(197, 20);
            this.dtTimeEnd.TabIndex = 3;
            // 
            // tbCloneFile
            // 
            this.tbCloneFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCloneFile.Location = new System.Drawing.Point(75, 64);
            this.tbCloneFile.Name = "tbCloneFile";
            this.tbCloneFile.Size = new System.Drawing.Size(197, 20);
            this.tbCloneFile.TabIndex = 5;
            this.tbCloneFile.Text = "C:\\InfoTask\\Debug\\Providers\\clone.accdb";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "CloneFile";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbCloneFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtTimeEnd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtTimeBegin);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dtTimeBegin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtTimeEnd;
        private System.Windows.Forms.TextBox tbCloneFile;
        private System.Windows.Forms.Label label3;
    }
}

