namespace GrafeoFormTest
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
            this.butShow = new System.Windows.Forms.Button();
            this.butShowTmp = new System.Windows.Forms.Button();
            this.butShowGrafeoForm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // butShow
            // 
            this.butShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.butShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butShow.Location = new System.Drawing.Point(12, 12);
            this.butShow.Name = "butShow";
            this.butShow.Size = new System.Drawing.Size(260, 32);
            this.butShow.TabIndex = 0;
            this.butShow.Text = "Show Grafeo";
            this.butShow.UseVisualStyleBackColor = true;
            this.butShow.Click += new System.EventHandler(this.butShow_Click);
            // 
            // butShowTmp
            // 
            this.butShowTmp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.butShowTmp.Enabled = false;
            this.butShowTmp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butShowTmp.Location = new System.Drawing.Point(12, 218);
            this.butShowTmp.Name = "butShowTmp";
            this.butShowTmp.Size = new System.Drawing.Size(260, 32);
            this.butShowTmp.TabIndex = 1;
            this.butShowTmp.Text = "Show Tmp";
            this.butShowTmp.UseVisualStyleBackColor = true;
            this.butShowTmp.Click += new System.EventHandler(this.butShowTmp_Click);
            // 
            // butShowGrafeoForm
            // 
            this.butShowGrafeoForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.butShowGrafeoForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butShowGrafeoForm.Location = new System.Drawing.Point(12, 180);
            this.butShowGrafeoForm.Name = "butShowGrafeoForm";
            this.butShowGrafeoForm.Size = new System.Drawing.Size(260, 32);
            this.butShowGrafeoForm.TabIndex = 2;
            this.butShowGrafeoForm.Text = "Show Grafeo Form";
            this.butShowGrafeoForm.UseVisualStyleBackColor = true;
            this.butShowGrafeoForm.Click += new System.EventHandler(this.butShowGrafeoForm_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.butShowGrafeoForm);
            this.Controls.Add(this.butShowTmp);
            this.Controls.Add(this.butShow);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "Form1";
            this.Text = "Grafeo Test";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butShow;
        private System.Windows.Forms.Button butShowTmp;
        private System.Windows.Forms.Button butShowGrafeoForm;
    }
}

