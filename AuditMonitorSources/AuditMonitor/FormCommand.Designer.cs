namespace AuditMonitor
{
    partial class FormCommand
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
            this.tbIn = new System.Windows.Forms.TextBox();
            this.cbOut = new System.Windows.Forms.ComboBox();
            this.butSendCommand = new System.Windows.Forms.Button();
            this.labOut = new System.Windows.Forms.Label();
            this.labIn = new System.Windows.Forms.Label();
            this.pnCommand = new System.Windows.Forms.Panel();
            this.tbAA = new System.Windows.Forms.TextBox();
            this.labAA = new System.Windows.Forms.Label();
            this.labCommandComment = new System.Windows.Forms.Label();
            this.lbCommandLog = new System.Windows.Forms.ListBox();
            this.pnCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbIn
            // 
            this.tbIn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbIn.Location = new System.Drawing.Point(61, 43);
            this.tbIn.Name = "tbIn";
            this.tbIn.Size = new System.Drawing.Size(609, 20);
            this.tbIn.TabIndex = 12;
            // 
            // cbOut
            // 
            this.cbOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOut.FormattingEnabled = true;
            this.cbOut.Location = new System.Drawing.Point(61, 16);
            this.cbOut.Name = "cbOut";
            this.cbOut.Size = new System.Drawing.Size(546, 21);
            this.cbOut.TabIndex = 13;
            this.cbOut.SelectedIndexChanged += new System.EventHandler(this.cbOut_SelectedIndexChanged);
            // 
            // butSendCommand
            // 
            this.butSendCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.butSendCommand.Location = new System.Drawing.Point(61, 69);
            this.butSendCommand.Name = "butSendCommand";
            this.butSendCommand.Size = new System.Drawing.Size(611, 34);
            this.butSendCommand.TabIndex = 4;
            this.butSendCommand.Text = "Выполнить";
            this.butSendCommand.UseVisualStyleBackColor = true;
            this.butSendCommand.Click += new System.EventHandler(this.butSendCommand_Click);
            // 
            // labOut
            // 
            this.labOut.AutoSize = true;
            this.labOut.Location = new System.Drawing.Point(3, 19);
            this.labOut.Name = "labOut";
            this.labOut.Size = new System.Drawing.Size(52, 13);
            this.labOut.TabIndex = 14;
            this.labOut.Text = "Команда";
            // 
            // labIn
            // 
            this.labIn.AutoSize = true;
            this.labIn.Location = new System.Drawing.Point(18, 46);
            this.labIn.Name = "labIn";
            this.labIn.Size = new System.Drawing.Size(37, 13);
            this.labIn.TabIndex = 15;
            this.labIn.Text = "Ответ";
            // 
            // pnCommand
            // 
            this.pnCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnCommand.Controls.Add(this.tbAA);
            this.pnCommand.Controls.Add(this.labAA);
            this.pnCommand.Controls.Add(this.labCommandComment);
            this.pnCommand.Controls.Add(this.labIn);
            this.pnCommand.Controls.Add(this.labOut);
            this.pnCommand.Controls.Add(this.butSendCommand);
            this.pnCommand.Controls.Add(this.cbOut);
            this.pnCommand.Controls.Add(this.tbIn);
            this.pnCommand.Location = new System.Drawing.Point(12, 12);
            this.pnCommand.Name = "pnCommand";
            this.pnCommand.Size = new System.Drawing.Size(675, 107);
            this.pnCommand.TabIndex = 12;
            // 
            // tbAA
            // 
            this.tbAA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAA.Location = new System.Drawing.Point(640, 17);
            this.tbAA.Name = "tbAA";
            this.tbAA.Size = new System.Drawing.Size(30, 20);
            this.tbAA.TabIndex = 18;
            // 
            // labAA
            // 
            this.labAA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labAA.AutoSize = true;
            this.labAA.Location = new System.Drawing.Point(613, 19);
            this.labAA.Name = "labAA";
            this.labAA.Size = new System.Drawing.Size(21, 13);
            this.labAA.TabIndex = 17;
            this.labAA.Text = "AA";
            // 
            // labCommandComment
            // 
            this.labCommandComment.AutoSize = true;
            this.labCommandComment.Location = new System.Drawing.Point(58, 0);
            this.labCommandComment.Name = "labCommandComment";
            this.labCommandComment.Size = new System.Drawing.Size(106, 13);
            this.labCommandComment.TabIndex = 16;
            this.labCommandComment.Text = "Команда не задана";
            // 
            // lbCommandLog
            // 
            this.lbCommandLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCommandLog.BackColor = System.Drawing.SystemColors.Window;
            this.lbCommandLog.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbCommandLog.FormattingEnabled = true;
            this.lbCommandLog.ItemHeight = 12;
            this.lbCommandLog.Location = new System.Drawing.Point(12, 125);
            this.lbCommandLog.Name = "lbCommandLog";
            this.lbCommandLog.Size = new System.Drawing.Size(675, 100);
            this.lbCommandLog.TabIndex = 13;
            // 
            // FormCommand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 238);
            this.Controls.Add(this.lbCommandLog);
            this.Controls.Add(this.pnCommand);
            this.Name = "FormCommand";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Выполнить команду";
            this.pnCommand.ResumeLayout(false);
            this.pnCommand.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbIn;
        private System.Windows.Forms.ComboBox cbOut;
        private System.Windows.Forms.Button butSendCommand;
        private System.Windows.Forms.Label labOut;
        private System.Windows.Forms.Label labIn;
        private System.Windows.Forms.Panel pnCommand;
        private System.Windows.Forms.Label labCommandComment;
        private System.Windows.Forms.ListBox lbCommandLog;
        private System.Windows.Forms.TextBox tbAA;
        private System.Windows.Forms.Label labAA;
    }
}