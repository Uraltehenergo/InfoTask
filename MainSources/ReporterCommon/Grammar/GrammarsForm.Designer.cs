﻿namespace ReporterCommon
{
    partial class GrammarsForm
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
            this.Result = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ButCondition = new System.Windows.Forms.Button();
            this.Formula = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Result
            // 
            this.Result.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Result.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Result.Location = new System.Drawing.Point(2, 316);
            this.Result.Multiline = true;
            this.Result.Name = "Result";
            this.Result.Size = new System.Drawing.Size(672, 232);
            this.Result.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Формула";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 297);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Результат";
            // 
            // ButCondition
            // 
            this.ButCondition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButCondition.Location = new System.Drawing.Point(2, 239);
            this.ButCondition.Name = "ButCondition";
            this.ButCondition.Size = new System.Drawing.Size(131, 52);
            this.ButCondition.TabIndex = 4;
            this.ButCondition.Text = "Shape";
            this.ButCondition.UseVisualStyleBackColor = true;
            this.ButCondition.Click += new System.EventHandler(this.ButCondition_Click);
            // 
            // Formula
            // 
            this.Formula.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Formula.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Formula.Location = new System.Drawing.Point(2, 26);
            this.Formula.Multiline = true;
            this.Formula.Name = "Formula";
            this.Formula.Size = new System.Drawing.Size(672, 209);
            this.Formula.TabIndex = 0;
            // 
            // GrammarsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 550);
            this.Controls.Add(this.ButCondition);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.Formula);
            this.Name = "GrammarsForm";
            this.Text = "GrammarsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Result;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ButCondition;
        private System.Windows.Forms.TextBox Formula;
    }
}