namespace ConsGraphLibrary
{
    partial class CGraphControl
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBoxAutoScale = new System.Windows.Forms.CheckBox();
            this.tBoxGridY = new System.Windows.Forms.TextBox();
            this.tBoxGridX = new System.Windows.Forms.TextBox();
            this.tBoxMaxY = new System.Windows.Forms.TextBox();
            this.tBoxMaxX = new System.Windows.Forms.TextBox();
            this.tBoxMinY = new System.Windows.Forms.TextBox();
            this.tBoxMinX = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.AxisX.ScrollBar.Enabled = false;
            chartArea1.AxisY.ScrollBar.Enabled = false;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            this.chart1.Size = new System.Drawing.Size(658, 327);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            title1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Left;
            title1.Name = "Title1";
            this.chart1.Titles.Add(title1);
            this.chart1.SelectionRangeChanging += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CursorEventArgs>(this.Chart1SelectionRangeChanging);
            this.chart1.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.Chart1AxisViewChanged);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseDown);
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseMove);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseUp);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chart1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkBoxAutoScale);
            this.splitContainer1.Panel2.Controls.Add(this.tBoxGridY);
            this.splitContainer1.Panel2.Controls.Add(this.tBoxGridX);
            this.splitContainer1.Panel2.Controls.Add(this.tBoxMaxY);
            this.splitContainer1.Panel2.Controls.Add(this.tBoxMaxX);
            this.splitContainer1.Panel2.Controls.Add(this.tBoxMinY);
            this.splitContainer1.Panel2.Controls.Add(this.tBoxMinX);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(658, 399);
            this.splitContainer1.SplitterDistance = 327;
            this.splitContainer1.TabIndex = 1;
            // 
            // checkBoxAutoScale
            // 
            this.checkBoxAutoScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoScale.AutoSize = true;
            this.checkBoxAutoScale.Location = new System.Drawing.Point(154, 38);
            this.checkBoxAutoScale.Name = "checkBoxAutoScale";
            this.checkBoxAutoScale.Size = new System.Drawing.Size(82, 17);
            this.checkBoxAutoScale.TabIndex = 14;
            this.checkBoxAutoScale.Text = "Автошкала";
            this.checkBoxAutoScale.UseVisualStyleBackColor = true;
            this.checkBoxAutoScale.CheckedChanged += new System.EventHandler(this.CheckBoxAutoScaleCheckedChanged);
            // 
            // tBoxGridY
            // 
            this.tBoxGridY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxGridY.Location = new System.Drawing.Point(566, 38);
            this.tBoxGridY.Name = "tBoxGridY";
            this.tBoxGridY.Size = new System.Drawing.Size(80, 20);
            this.tBoxGridY.TabIndex = 13;
            this.tBoxGridY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxGridY.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxGridX
            // 
            this.tBoxGridX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxGridX.Location = new System.Drawing.Point(566, 12);
            this.tBoxGridX.Name = "tBoxGridX";
            this.tBoxGridX.Size = new System.Drawing.Size(80, 20);
            this.tBoxGridX.TabIndex = 12;
            this.tBoxGridX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxGridX.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMaxY
            // 
            this.tBoxMaxY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMaxY.Location = new System.Drawing.Point(447, 38);
            this.tBoxMaxY.Name = "tBoxMaxY";
            this.tBoxMaxY.Size = new System.Drawing.Size(80, 20);
            this.tBoxMaxY.TabIndex = 11;
            this.tBoxMaxY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMaxY.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMaxX
            // 
            this.tBoxMaxX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMaxX.Location = new System.Drawing.Point(447, 12);
            this.tBoxMaxX.Name = "tBoxMaxX";
            this.tBoxMaxX.Size = new System.Drawing.Size(80, 20);
            this.tBoxMaxX.TabIndex = 10;
            this.tBoxMaxX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMaxX.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMinY
            // 
            this.tBoxMinY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMinY.Location = new System.Drawing.Point(328, 38);
            this.tBoxMinY.Name = "tBoxMinY";
            this.tBoxMinY.Size = new System.Drawing.Size(80, 20);
            this.tBoxMinY.TabIndex = 9;
            this.tBoxMinY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMinY.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMinX
            // 
            this.tBoxMinX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMinX.Location = new System.Drawing.Point(328, 12);
            this.tBoxMinX.Name = "tBoxMinX";
            this.tBoxMinX.Size = new System.Drawing.Size(80, 20);
            this.tBoxMinX.TabIndex = 8;
            this.tBoxMinX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMinX.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(240, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Ось Y";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(240, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Ось X";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(533, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Шаг";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(533, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Шаг";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(414, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Max";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(414, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Max";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(298, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Min";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(298, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min";
            // 
            // CGraphControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CGraphControl";
            this.Size = new System.Drawing.Size(658, 399);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox tBoxGridY;
        private System.Windows.Forms.TextBox tBoxGridX;
        private System.Windows.Forms.TextBox tBoxMaxY;
        private System.Windows.Forms.TextBox tBoxMaxX;
        private System.Windows.Forms.TextBox tBoxMinY;
        private System.Windows.Forms.TextBox tBoxMinX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxAutoScale;
    }
}
