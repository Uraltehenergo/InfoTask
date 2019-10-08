using System.Windows.Forms;

namespace ConsGraphLibrary
{
    public partial class CGraphForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CGraphForm));
            this.panelLegend = new System.Windows.Forms.Panel();
            this.labelDim = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonRemovePlane = new System.Windows.Forms.Button();
            this.dataGridViewLegend = new System.Windows.Forms.DataGridView();
            this.Column4 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonAddPlane = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.checkBoxAutoScaleX = new System.Windows.Forms.CheckBox();
            this.tBoxGridY = new System.Windows.Forms.TextBox();
            this.tBoxGridX = new System.Windows.Forms.TextBox();
            this.tBoxMaxY = new System.Windows.Forms.TextBox();
            this.tBoxMaxX = new System.Windows.Forms.TextBox();
            this.tBoxMinY = new System.Windows.Forms.TextBox();
            this.tBoxMinX = new System.Windows.Forms.TextBox();
            this.labelAxY = new System.Windows.Forms.Label();
            this.labelAxX = new System.Windows.Forms.Label();
            this.labelStepY = new System.Windows.Forms.Label();
            this.labelStepX = new System.Windows.Forms.Label();
            this.labelMaxY = new System.Windows.Forms.Label();
            this.labelMaxX = new System.Windows.Forms.Label();
            this.labelMinY = new System.Windows.Forms.Label();
            this.labelMinX = new System.Windows.Forms.Label();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridViewValues = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonSaveScale = new System.Windows.Forms.Button();
            this.buttonDefaultY = new System.Windows.Forms.Button();
            this.checkBoxAutoScaleY = new System.Windows.Forms.CheckBox();
            this.labelUnitsY = new System.Windows.Forms.Label();
            this.labelNameY = new System.Windows.Forms.Label();
            this.buttonDefaultX = new System.Windows.Forms.Button();
            this.labelUnitsX = new System.Windows.Forms.Label();
            this.labelNameX = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panelLegend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLegend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewValues)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLegend
            // 
            this.panelLegend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLegend.BackColor = System.Drawing.SystemColors.Control;
            this.panelLegend.Controls.Add(this.labelDim);
            this.panelLegend.Controls.Add(this.checkBox1);
            this.panelLegend.Controls.Add(this.buttonRemovePlane);
            this.panelLegend.Controls.Add(this.dataGridViewLegend);
            this.panelLegend.Controls.Add(this.buttonAddPlane);
            this.panelLegend.Location = new System.Drawing.Point(501, 25);
            this.panelLegend.MinimumSize = new System.Drawing.Size(66, 0);
            this.panelLegend.Name = "panelLegend";
            this.panelLegend.Size = new System.Drawing.Size(66, 374);
            this.panelLegend.TabIndex = 6;
            // 
            // labelDim
            // 
            this.labelDim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDim.AutoSize = true;
            this.labelDim.Location = new System.Drawing.Point(2, 358);
            this.labelDim.Name = "labelDim";
            this.labelDim.Size = new System.Drawing.Size(40, 13);
            this.labelDim.TabIndex = 7;
            this.labelDim.Text = "Разм.:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(6, 6);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
            // 
            // buttonRemovePlane
            // 
            this.buttonRemovePlane.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemovePlane.BackColor = System.Drawing.Color.White;
            this.buttonRemovePlane.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemovePlane.Image = global::ConsGraphLibrary.Properties.Resources.ЗаписьУдалить;
            this.buttonRemovePlane.Location = new System.Drawing.Point(44, 3);
            this.buttonRemovePlane.Name = "buttonRemovePlane";
            this.buttonRemovePlane.Size = new System.Drawing.Size(19, 19);
            this.buttonRemovePlane.TabIndex = 1;
            this.buttonRemovePlane.UseVisualStyleBackColor = false;
            this.buttonRemovePlane.Click += new System.EventHandler(this.ButtonRemovePlaneClick);
            // 
            // dataGridViewLegend
            // 
            this.dataGridViewLegend.AllowUserToAddRows = false;
            this.dataGridViewLegend.AllowUserToDeleteRows = false;
            this.dataGridViewLegend.AllowUserToResizeColumns = false;
            this.dataGridViewLegend.AllowUserToResizeRows = false;
            this.dataGridViewLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewLegend.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewLegend.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewLegend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewLegend.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewLegend.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLegend.ColumnHeadersVisible = false;
            this.dataGridViewLegend.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column4,
            this.Column5});
            this.dataGridViewLegend.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewLegend.GridColor = System.Drawing.Color.White;
            this.dataGridViewLegend.Location = new System.Drawing.Point(3, 25);
            this.dataGridViewLegend.MultiSelect = false;
            this.dataGridViewLegend.Name = "dataGridViewLegend";
            this.dataGridViewLegend.RowHeadersVisible = false;
            this.dataGridViewLegend.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewLegend.Size = new System.Drawing.Size(60, 330);
            this.dataGridViewLegend.TabIndex = 5;
            this.dataGridViewLegend.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewLegendCellDoubleClick);
            this.dataGridViewLegend.SelectionChanged += new System.EventHandler(this.DataGridViewLegendSelectionChanged);
            // 
            // Column4
            // 
            this.Column4.FillWeight = 20F;
            this.Column4.HeaderText = "Column4";
            this.Column4.MinimumWidth = 20;
            this.Column4.Name = "Column4";
            this.Column4.Width = 20;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column5.FillWeight = 159.3909F;
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            // 
            // buttonAddPlane
            // 
            this.buttonAddPlane.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddPlane.BackColor = System.Drawing.Color.White;
            this.buttonAddPlane.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddPlane.Image = global::ConsGraphLibrary.Properties.Resources.ЗаписьСоздать;
            this.buttonAddPlane.Location = new System.Drawing.Point(23, 3);
            this.buttonAddPlane.Name = "buttonAddPlane";
            this.buttonAddPlane.Size = new System.Drawing.Size(19, 19);
            this.buttonAddPlane.TabIndex = 0;
            this.buttonAddPlane.UseVisualStyleBackColor = false;
            this.buttonAddPlane.Click += new System.EventHandler(this.ButtonAddPlaneClick);
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AxisX.ScrollBar.Enabled = false;
            chartArea1.AxisY.ScrollBar.Enabled = false;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.MinimumSize = new System.Drawing.Size(200, 150);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            this.chart1.Size = new System.Drawing.Size(501, 457);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            this.chart1.SelectionRangeChanging += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CursorEventArgs>(this.Chart1SelectionRangeChanging);
            this.chart1.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.Chart1AxisViewChanged);
            this.chart1.PostPaint += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs>(this.Chart1PostPaint);
            this.chart1.Click += new System.EventHandler(this.Chart1Click);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseDown);
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseMove);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseUp);
            // 
            // checkBoxAutoScaleX
            // 
            this.checkBoxAutoScaleX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoScaleX.AutoSize = true;
            this.checkBoxAutoScaleX.Location = new System.Drawing.Point(19, 20);
            this.checkBoxAutoScaleX.Name = "checkBoxAutoScaleX";
            this.checkBoxAutoScaleX.Size = new System.Drawing.Size(50, 17);
            this.checkBoxAutoScaleX.TabIndex = 29;
            this.checkBoxAutoScaleX.Text = "Авто";
            this.checkBoxAutoScaleX.UseVisualStyleBackColor = true;
            this.checkBoxAutoScaleX.CheckedChanged += new System.EventHandler(this.CheckBoxAutoScaleXCheckedChanged);
            // 
            // tBoxGridY
            // 
            this.tBoxGridY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxGridY.Location = new System.Drawing.Point(76, 235);
            this.tBoxGridY.Name = "tBoxGridY";
            this.tBoxGridY.Size = new System.Drawing.Size(80, 20);
            this.tBoxGridY.TabIndex = 28;
            this.tBoxGridY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxGridY.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxGridX
            // 
            this.tBoxGridX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxGridX.Location = new System.Drawing.Point(76, 89);
            this.tBoxGridX.Name = "tBoxGridX";
            this.tBoxGridX.Size = new System.Drawing.Size(80, 20);
            this.tBoxGridX.TabIndex = 27;
            this.tBoxGridX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxGridX.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMaxY
            // 
            this.tBoxMaxY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMaxY.Location = new System.Drawing.Point(76, 209);
            this.tBoxMaxY.Name = "tBoxMaxY";
            this.tBoxMaxY.Size = new System.Drawing.Size(80, 20);
            this.tBoxMaxY.TabIndex = 26;
            this.tBoxMaxY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMaxY.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMaxX
            // 
            this.tBoxMaxX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMaxX.Location = new System.Drawing.Point(76, 62);
            this.tBoxMaxX.Name = "tBoxMaxX";
            this.tBoxMaxX.Size = new System.Drawing.Size(80, 20);
            this.tBoxMaxX.TabIndex = 25;
            this.tBoxMaxX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMaxX.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMinY
            // 
            this.tBoxMinY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMinY.Location = new System.Drawing.Point(76, 183);
            this.tBoxMinY.Name = "tBoxMinY";
            this.tBoxMinY.Size = new System.Drawing.Size(80, 20);
            this.tBoxMinY.TabIndex = 24;
            this.tBoxMinY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMinY.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // tBoxMinX
            // 
            this.tBoxMinX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxMinX.Location = new System.Drawing.Point(76, 36);
            this.tBoxMinX.Name = "tBoxMinX";
            this.tBoxMinX.Size = new System.Drawing.Size(80, 20);
            this.tBoxMinX.TabIndex = 23;
            this.tBoxMinX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxInputAnyReal);
            this.tBoxMinX.Leave += new System.EventHandler(this.TextBoxLeave);
            // 
            // labelAxY
            // 
            this.labelAxY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAxY.AutoSize = true;
            this.labelAxY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelAxY.Location = new System.Drawing.Point(16, 149);
            this.labelAxY.Name = "labelAxY";
            this.labelAxY.Size = new System.Drawing.Size(19, 13);
            this.labelAxY.TabIndex = 22;
            this.labelAxY.Text = "Y:";
            // 
            // labelAxX
            // 
            this.labelAxX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAxX.AutoSize = true;
            this.labelAxX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelAxX.Location = new System.Drawing.Point(16, 3);
            this.labelAxX.Name = "labelAxX";
            this.labelAxX.Size = new System.Drawing.Size(19, 13);
            this.labelAxX.TabIndex = 21;
            this.labelAxX.Text = "X:";
            // 
            // labelStepY
            // 
            this.labelStepY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStepY.AutoSize = true;
            this.labelStepY.Location = new System.Drawing.Point(42, 242);
            this.labelStepY.Name = "labelStepY";
            this.labelStepY.Size = new System.Drawing.Size(30, 13);
            this.labelStepY.TabIndex = 20;
            this.labelStepY.Text = "Шаг:";
            // 
            // labelStepX
            // 
            this.labelStepX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStepX.AutoSize = true;
            this.labelStepX.Location = new System.Drawing.Point(42, 96);
            this.labelStepX.Name = "labelStepX";
            this.labelStepX.Size = new System.Drawing.Size(30, 13);
            this.labelStepX.TabIndex = 19;
            this.labelStepX.Text = "Шаг:";
            // 
            // labelMaxY
            // 
            this.labelMaxY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxY.AutoSize = true;
            this.labelMaxY.Location = new System.Drawing.Point(42, 212);
            this.labelMaxY.Name = "labelMaxY";
            this.labelMaxY.Size = new System.Drawing.Size(30, 13);
            this.labelMaxY.TabIndex = 18;
            this.labelMaxY.Text = "Max:";
            // 
            // labelMaxX
            // 
            this.labelMaxX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxX.AutoSize = true;
            this.labelMaxX.Location = new System.Drawing.Point(42, 69);
            this.labelMaxX.Name = "labelMaxX";
            this.labelMaxX.Size = new System.Drawing.Size(30, 13);
            this.labelMaxX.TabIndex = 17;
            this.labelMaxX.Text = "Max:";
            // 
            // labelMinY
            // 
            this.labelMinY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinY.AutoSize = true;
            this.labelMinY.Location = new System.Drawing.Point(42, 187);
            this.labelMinY.Name = "labelMinY";
            this.labelMinY.Size = new System.Drawing.Size(27, 13);
            this.labelMinY.TabIndex = 16;
            this.labelMinY.Text = "Min:";
            // 
            // labelMinX
            // 
            this.labelMinX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinX.AutoSize = true;
            this.labelMinX.Location = new System.Drawing.Point(42, 43);
            this.labelMinX.Name = "labelMinX";
            this.labelMinX.Size = new System.Drawing.Size(27, 13);
            this.labelMinX.TabIndex = 15;
            this.labelMinX.Text = "Min:";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainerMain.Panel1.Controls.Add(this.panelLegend);
            this.splitContainerMain.Panel1.Controls.Add(this.chart1);
            this.splitContainerMain.Panel1MinSize = 350;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.buttonSave);
            this.splitContainerMain.Panel2.Controls.Add(this.tabControl1);
            this.splitContainerMain.Panel2MinSize = 50;
            this.splitContainerMain.Size = new System.Drawing.Size(749, 457);
            this.splitContainerMain.SplitterDistance = 570;
            this.splitContainerMain.TabIndex = 1;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.Location = new System.Drawing.Point(6, 405);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(161, 40);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Редактировать";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(2, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(173, 396);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridViewValues);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(165, 370);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Значения";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridViewValues
            // 
            this.dataGridViewValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewValues.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewValues.Name = "dataGridViewValues";
            this.dataGridViewValues.RowHeadersWidth = 20;
            this.dataGridViewValues.Size = new System.Drawing.Size(159, 364);
            this.dataGridViewValues.TabIndex = 0;
            this.dataGridViewValues.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewValuesCellClick);
            this.dataGridViewValues.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridViewValuesDataError);
            this.dataGridViewValues.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewValuesRowEnter);
            this.dataGridViewValues.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewValuesRowLeave);
            this.dataGridViewValues.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewValuesUserAddedRow);
            this.dataGridViewValues.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DataGridViewValuesUserDeletingRow);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonSaveScale);
            this.tabPage2.Controls.Add(this.buttonDefaultY);
            this.tabPage2.Controls.Add(this.checkBoxAutoScaleY);
            this.tabPage2.Controls.Add(this.labelUnitsY);
            this.tabPage2.Controls.Add(this.labelNameY);
            this.tabPage2.Controls.Add(this.buttonDefaultX);
            this.tabPage2.Controls.Add(this.labelUnitsX);
            this.tabPage2.Controls.Add(this.labelNameX);
            this.tabPage2.Controls.Add(this.checkBoxAutoScaleX);
            this.tabPage2.Controls.Add(this.labelAxX);
            this.tabPage2.Controls.Add(this.labelStepY);
            this.tabPage2.Controls.Add(this.tBoxGridY);
            this.tabPage2.Controls.Add(this.labelMaxY);
            this.tabPage2.Controls.Add(this.tBoxMinX);
            this.tabPage2.Controls.Add(this.labelMinY);
            this.tabPage2.Controls.Add(this.tBoxMaxY);
            this.tabPage2.Controls.Add(this.tBoxGridX);
            this.tabPage2.Controls.Add(this.tBoxMinY);
            this.tabPage2.Controls.Add(this.tBoxMaxX);
            this.tabPage2.Controls.Add(this.labelAxY);
            this.tabPage2.Controls.Add(this.labelMinX);
            this.tabPage2.Controls.Add(this.labelMaxX);
            this.tabPage2.Controls.Add(this.labelStepX);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(165, 370);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Отображение";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonSaveScale
            // 
            this.buttonSaveScale.Location = new System.Drawing.Point(6, 290);
            this.buttonSaveScale.Name = "buttonSaveScale";
            this.buttonSaveScale.Size = new System.Drawing.Size(150, 23);
            this.buttonSaveScale.TabIndex = 37;
            this.buttonSaveScale.Text = "Сохранить шкалы";
            this.buttonSaveScale.UseVisualStyleBackColor = true;
            this.buttonSaveScale.Click += new System.EventHandler(this.ButtonSaveScaleClick);
            // 
            // buttonDefaultY
            // 
            this.buttonDefaultY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDefaultY.Location = new System.Drawing.Point(48, 261);
            this.buttonDefaultY.Name = "buttonDefaultY";
            this.buttonDefaultY.Size = new System.Drawing.Size(108, 23);
            this.buttonDefaultY.TabIndex = 36;
            this.buttonDefaultY.Text = "По шкале";
            this.buttonDefaultY.UseVisualStyleBackColor = true;
            this.buttonDefaultY.Click += new System.EventHandler(this.ButtonDefaultYClick);
            // 
            // checkBoxAutoScaleY
            // 
            this.checkBoxAutoScaleY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoScaleY.AutoSize = true;
            this.checkBoxAutoScaleY.Location = new System.Drawing.Point(19, 167);
            this.checkBoxAutoScaleY.Name = "checkBoxAutoScaleY";
            this.checkBoxAutoScaleY.Size = new System.Drawing.Size(50, 17);
            this.checkBoxAutoScaleY.TabIndex = 35;
            this.checkBoxAutoScaleY.Text = "Авто";
            this.checkBoxAutoScaleY.UseVisualStyleBackColor = true;
            this.checkBoxAutoScaleY.CheckedChanged += new System.EventHandler(this.CheckBoxAutoScaleYCheckedChanged);
            // 
            // labelUnitsY
            // 
            this.labelUnitsY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUnitsY.AutoSize = true;
            this.labelUnitsY.Location = new System.Drawing.Point(76, 167);
            this.labelUnitsY.Name = "labelUnitsY";
            this.labelUnitsY.Size = new System.Drawing.Size(35, 13);
            this.labelUnitsY.TabIndex = 34;
            this.labelUnitsY.Text = "label2";
            // 
            // labelNameY
            // 
            this.labelNameY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNameY.AutoSize = true;
            this.labelNameY.Location = new System.Drawing.Point(45, 149);
            this.labelNameY.MaximumSize = new System.Drawing.Size(120, 0);
            this.labelNameY.Name = "labelNameY";
            this.labelNameY.Size = new System.Drawing.Size(35, 13);
            this.labelNameY.TabIndex = 33;
            this.labelNameY.Text = "label1";
            // 
            // buttonDefaultX
            // 
            this.buttonDefaultX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDefaultX.Location = new System.Drawing.Point(48, 115);
            this.buttonDefaultX.Name = "buttonDefaultX";
            this.buttonDefaultX.Size = new System.Drawing.Size(108, 23);
            this.buttonDefaultX.TabIndex = 32;
            this.buttonDefaultX.Text = "По шкале";
            this.buttonDefaultX.UseVisualStyleBackColor = true;
            this.buttonDefaultX.Click += new System.EventHandler(this.ButtonDefaultXClick);
            // 
            // labelUnitsX
            // 
            this.labelUnitsX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUnitsX.AutoSize = true;
            this.labelUnitsX.Location = new System.Drawing.Point(76, 20);
            this.labelUnitsX.Name = "labelUnitsX";
            this.labelUnitsX.Size = new System.Drawing.Size(35, 13);
            this.labelUnitsX.TabIndex = 31;
            this.labelUnitsX.Text = "label2";
            // 
            // labelNameX
            // 
            this.labelNameX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNameX.AutoSize = true;
            this.labelNameX.Location = new System.Drawing.Point(45, 4);
            this.labelNameX.MaximumSize = new System.Drawing.Size(120, 0);
            this.labelNameX.Name = "labelNameX";
            this.labelNameX.Size = new System.Drawing.Size(35, 13);
            this.labelNameX.TabIndex = 30;
            this.labelNameX.Text = "label1";
            // 
            // CGraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 457);
            this.Controls.Add(this.splitContainerMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 300);
            this.Name = "CGraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CGraphForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CGraphFormFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CGraphFormFormClosed);
            this.panelLegend.ResumeLayout(false);
            this.panelLegend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLegend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewValues)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxAutoScaleX;
        private System.Windows.Forms.TextBox tBoxGridY;
        private System.Windows.Forms.TextBox tBoxGridX;
        private System.Windows.Forms.TextBox tBoxMaxY;
        private System.Windows.Forms.TextBox tBoxMaxX;
        private System.Windows.Forms.TextBox tBoxMinY;
        private System.Windows.Forms.TextBox tBoxMinX;
        private System.Windows.Forms.Label labelAxY;
        private System.Windows.Forms.Label labelAxX;
        private System.Windows.Forms.Label labelStepY;
        private System.Windows.Forms.Label labelStepX;
        private System.Windows.Forms.Label labelMaxY;
        private System.Windows.Forms.Label labelMaxX;
        private System.Windows.Forms.Label labelMinY;
        private System.Windows.Forms.Label labelMinX;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridViewValues;
        private DataGridView dataGridViewLegend;
        private Panel panelLegend;
        private Button buttonRemovePlane;
        private Button buttonAddPlane;
        private CheckBox checkBox1;
        private Label labelUnitsX;
        private Label labelNameX;
        private CheckBox checkBoxAutoScaleY;
        private Label labelUnitsY;
        private Label labelNameY;
        private Button buttonDefaultX;
        private Button buttonDefaultY;
        private Button buttonSaveScale;
        private DataGridViewImageColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private Label labelDim;
        private ToolTip toolTip1;
    }
}