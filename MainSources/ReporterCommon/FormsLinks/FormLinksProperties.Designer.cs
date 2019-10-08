namespace ReporterCommon
{
    partial class FormLinkProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLinkProperties));
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButTime = new System.Windows.Forms.Button();
            this.ButNumber = new System.Windows.Forms.Button();
            this.ButNd = new System.Windows.Forms.Button();
            this.ButValue = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.LinkCellComment = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PropsPanel = new ReporterCommon.ControlLinkProps();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ButTime);
            this.panel1.Controls.Add(this.ButNumber);
            this.panel1.Controls.Add(this.ButNd);
            this.panel1.Controls.Add(this.ButValue);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 218);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(458, 72);
            this.panel1.TabIndex = 15;
            // 
            // ButTime
            // 
            this.ButTime.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButTime.ForeColor = System.Drawing.Color.MidnightBlue;
            this.ButTime.Location = new System.Drawing.Point(84, 20);
            this.ButTime.Name = "ButTime";
            this.ButTime.Size = new System.Drawing.Size(80, 46);
            this.ButTime.TabIndex = 25;
            this.ButTime.Text = "Время";
            this.ButTime.UseVisualStyleBackColor = true;
            this.ButTime.Click += new System.EventHandler(this.ButTime_Click);
            // 
            // ButNumber
            // 
            this.ButNumber.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButNumber.ForeColor = System.Drawing.Color.MidnightBlue;
            this.ButNumber.Location = new System.Drawing.Point(259, 20);
            this.ButNumber.Name = "ButNumber";
            this.ButNumber.Size = new System.Drawing.Size(84, 46);
            this.ButNumber.TabIndex = 35;
            this.ButNumber.Text = "Номера значений";
            this.ButNumber.UseVisualStyleBackColor = true;
            this.ButNumber.Click += new System.EventHandler(this.ButNumber_Click);
            // 
            // ButNd
            // 
            this.ButNd.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButNd.ForeColor = System.Drawing.Color.MidnightBlue;
            this.ButNd.Location = new System.Drawing.Point(166, 20);
            this.ButNd.Name = "ButNd";
            this.ButNd.Size = new System.Drawing.Size(91, 46);
            this.ButNd.TabIndex = 30;
            this.ButNd.Text = "Недостовер ность";
            this.ButNd.UseVisualStyleBackColor = true;
            this.ButNd.Click += new System.EventHandler(this.ButNd_Click);
            // 
            // ButValue
            // 
            this.ButValue.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButValue.ForeColor = System.Drawing.Color.Blue;
            this.ButValue.Location = new System.Drawing.Point(3, 20);
            this.ButValue.Name = "ButValue";
            this.ButValue.Size = new System.Drawing.Size(79, 46);
            this.ButValue.TabIndex = 20;
            this.ButValue.Text = "Значение";
            this.ButValue.UseVisualStyleBackColor = true;
            this.ButValue.Click += new System.EventHandler(this.ButValue_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(6, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 16);
            this.label1.TabIndex = 107;
            this.label1.Text = "Установить по ссылку на:";
            // 
            // LinkCellComment
            // 
            this.LinkCellComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LinkCellComment.Location = new System.Drawing.Point(94, 2);
            this.LinkCellComment.Name = "LinkCellComment";
            this.LinkCellComment.Size = new System.Drawing.Size(360, 22);
            this.LinkCellComment.TabIndex = 5;
            this.LinkCellComment.TextChanged += new System.EventHandler(this.LinkCellComment_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(2, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 16);
            this.label3.TabIndex = 112;
            this.label3.Text = "Примечание";
            // 
            // PropsPanel
            // 
            this.PropsPanel.Location = new System.Drawing.Point(1, 26);
            this.PropsPanel.Name = "PropsPanel";
            this.PropsPanel.Size = new System.Drawing.Size(457, 192);
            this.PropsPanel.TabIndex = 10;
            this.PropsPanel.OnLinkTypeChange += new System.EventHandler(this.LinkPropsPanel_OnLinkTypeChange);
            // 
            // FormLinkProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 291);
            this.Controls.Add(this.LinkCellComment);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PropsPanel);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLinkProperties";
            this.ShowInTaskbar = false;
            this.Text = "Свойства ссылки";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLinkProperties_FormClosed);
            this.Load += new System.EventHandler(this.FormLinkProperties_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButTime;
        private System.Windows.Forms.Button ButNumber;
        private System.Windows.Forms.Button ButNd;
        private System.Windows.Forms.Button ButValue;
        public ControlLinkProps PropsPanel;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox LinkCellComment;
    }
}