namespace AuditMonitor
{
    partial class FormYesNoDialog
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
            this.labMessage = new System.Windows.Forms.Label();
            this.butYes = new System.Windows.Forms.Button();
            this.butNo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labMessage
            // 
            this.labMessage.BackColor = System.Drawing.SystemColors.Control;
            this.labMessage.Location = new System.Drawing.Point(12, 9);
            this.labMessage.Name = "labMessage";
            this.labMessage.Size = new System.Drawing.Size(260, 65);
            this.labMessage.TabIndex = 0;
            this.labMessage.Text = "Message";
            this.labMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // butYes
            // 
            this.butYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butYes.Location = new System.Drawing.Point(12, 77);
            this.butYes.Name = "butYes";
            this.butYes.Size = new System.Drawing.Size(108, 28);
            this.butYes.TabIndex = 1;
            this.butYes.Text = "Да";
            this.butYes.UseVisualStyleBackColor = true;
            this.butYes.Click += new System.EventHandler(this.butYes_Click);
            // 
            // butNo
            // 
            this.butNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butNo.Location = new System.Drawing.Point(164, 77);
            this.butNo.Name = "butNo";
            this.butNo.Size = new System.Drawing.Size(108, 28);
            this.butNo.TabIndex = 2;
            this.butNo.Text = "Нет";
            this.butNo.UseVisualStyleBackColor = true;
            this.butNo.Click += new System.EventHandler(this.butNo_Click);
            // 
            // FormYesNoDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 114);
            this.ControlBox = false;
            this.Controls.Add(this.butNo);
            this.Controls.Add(this.butYes);
            this.Controls.Add(this.labMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormYesNoDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Caption";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labMessage;
        private System.Windows.Forms.Button butYes;
        private System.Windows.Forms.Button butNo;
    }
}