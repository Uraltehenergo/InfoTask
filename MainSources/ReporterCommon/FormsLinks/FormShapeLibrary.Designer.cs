namespace ReporterCommon.FormsLinks
{
    partial class FormShapeLibrary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShapeLibrary));
            this.label1 = new System.Windows.Forms.Label();
            this.ShapeType = new System.Windows.Forms.TextBox();
            this.ShapeName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ShapeFormula = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ShapeId = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ButSaveShape = new System.Windows.Forms.Button();
            this.ColorDialog = new System.Windows.Forms.ColorDialog();
            this.ButLoadShape = new System.Windows.Forms.Button();
            this.ShapeLink = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ButCheck = new System.Windows.Forms.Button();
            this.TestValue = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.ButSyntax = new System.Windows.Forms.Button();
            this.CursorPosition = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(2, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Тип";
            // 
            // ShapeType
            // 
            this.ShapeType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ShapeType.Location = new System.Drawing.Point(42, 8);
            this.ShapeType.Name = "ShapeType";
            this.ShapeType.ReadOnly = true;
            this.ShapeType.Size = new System.Drawing.Size(269, 22);
            this.ShapeType.TabIndex = 1;
            // 
            // ShapeName
            // 
            this.ShapeName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ShapeName.Location = new System.Drawing.Point(42, 34);
            this.ShapeName.Name = "ShapeName";
            this.ShapeName.Size = new System.Drawing.Size(349, 22);
            this.ShapeName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(2, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Имя ";
            // 
            // ShapeFormula
            // 
            this.ShapeFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ShapeFormula.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ShapeFormula.Location = new System.Drawing.Point(-1, 86);
            this.ShapeFormula.Multiline = true;
            this.ShapeFormula.Name = "ShapeFormula";
            this.ShapeFormula.Size = new System.Drawing.Size(554, 252);
            this.ShapeFormula.TabIndex = 5;
            this.ShapeFormula.Enter += new System.EventHandler(this.ShapeFormula_Enter);
            this.ShapeFormula.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShapeFormula_KeyDown);
            this.ShapeFormula.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ShapeFormula_KeyUp);
            this.ShapeFormula.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ShapeFormula_MouseDown);
            this.ShapeFormula.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ShapeFormula_MouseUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(1, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(165, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Описание поведения";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(315, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Id";
            // 
            // ShapeId
            // 
            this.ShapeId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ShapeId.Location = new System.Drawing.Point(337, 8);
            this.ShapeId.Name = "ShapeId";
            this.ShapeId.ReadOnly = true;
            this.ShapeId.Size = new System.Drawing.Size(54, 22);
            this.ShapeId.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(233, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 26);
            this.button1.TabIndex = 11;
            this.button1.Text = "Выбрать цвет";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ButSaveShape
            // 
            this.ButSaveShape.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButSaveShape.Location = new System.Drawing.Point(393, 48);
            this.ButSaveShape.Name = "ButSaveShape";
            this.ButSaveShape.Size = new System.Drawing.Size(159, 37);
            this.ButSaveShape.TabIndex = 12;
            this.ButSaveShape.Text = "Сохранить фигуру";
            this.ButSaveShape.UseVisualStyleBackColor = true;
            this.ButSaveShape.Click += new System.EventHandler(this.ButSaveShape_Click_1);
            // 
            // ButLoadShape
            // 
            this.ButLoadShape.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButLoadShape.Location = new System.Drawing.Point(393, 7);
            this.ButLoadShape.Name = "ButLoadShape";
            this.ButLoadShape.Size = new System.Drawing.Size(159, 37);
            this.ButLoadShape.TabIndex = 13;
            this.ButLoadShape.Text = "Загрузить фигуру";
            this.ButLoadShape.UseVisualStyleBackColor = true;
            this.ButLoadShape.Click += new System.EventHandler(this.ButLoadShape_Click);
            // 
            // ShapeLink
            // 
            this.ShapeLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ShapeLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ShapeLink.Location = new System.Drawing.Point(-1, 374);
            this.ShapeLink.Multiline = true;
            this.ShapeLink.Name = "ShapeLink";
            this.ShapeLink.ReadOnly = true;
            this.ShapeLink.Size = new System.Drawing.Size(554, 54);
            this.ShapeLink.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(0, 357);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 16);
            this.label5.TabIndex = 15;
            this.label5.Text = "Ссылка";
            // 
            // ButCheck
            // 
            this.ButCheck.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ButCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButCheck.Location = new System.Drawing.Point(416, 338);
            this.ButCheck.Name = "ButCheck";
            this.ButCheck.Size = new System.Drawing.Size(101, 26);
            this.ButCheck.TabIndex = 17;
            this.ButCheck.Text = "Применить";
            this.ButCheck.UseVisualStyleBackColor = true;
            this.ButCheck.Click += new System.EventHandler(this.ButCheck_Click);
            // 
            // TestValue
            // 
            this.TestValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.TestValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TestValue.Location = new System.Drawing.Point(311, 340);
            this.TestValue.Name = "TestValue";
            this.TestValue.Size = new System.Drawing.Size(104, 22);
            this.TestValue.TabIndex = 19;
            this.TestValue.TextChanged += new System.EventHandler(this.TestValue_TextChanged);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(229, 343);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 16);
            this.label6.TabIndex = 18;
            this.label6.Text = "Значение";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(517, 338);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(36, 26);
            this.button2.TabIndex = 20;
            this.button2.Text = "*2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ButSyntax
            // 
            this.ButSyntax.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ButSyntax.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButSyntax.Location = new System.Drawing.Point(122, 338);
            this.ButSyntax.Name = "ButSyntax";
            this.ButSyntax.Size = new System.Drawing.Size(101, 26);
            this.ButSyntax.TabIndex = 21;
            this.ButSyntax.Text = "Проверить";
            this.ButSyntax.UseVisualStyleBackColor = true;
            this.ButSyntax.Click += new System.EventHandler(this.ButSyntax_Click);
            // 
            // CursorPosition
            // 
            this.CursorPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CursorPosition.AutoSize = true;
            this.CursorPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CursorPosition.Location = new System.Drawing.Point(0, 338);
            this.CursorPosition.Name = "CursorPosition";
            this.CursorPosition.Size = new System.Drawing.Size(83, 16);
            this.CursorPosition.TabIndex = 22;
            this.CursorPosition.Text = "стр. 1 поз. 1";
            // 
            // FormShapeLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 428);
            this.Controls.Add(this.CursorPosition);
            this.Controls.Add(this.ButSyntax);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.TestValue);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ButCheck);
            this.Controls.Add(this.ShapeLink);
            this.Controls.Add(this.ButLoadShape);
            this.Controls.Add(this.ButSaveShape);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ShapeId);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ShapeFormula);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ShapeName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ShapeType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormShapeLibrary";
            this.Text = "Свойства фигуры";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormShapeLibrary_FormClosing);
            this.Load += new System.EventHandler(this.FormShapeLibrary_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ShapeType;
        private System.Windows.Forms.TextBox ShapeName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ShapeFormula;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ShapeId;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button ButSaveShape;
        private System.Windows.Forms.ColorDialog ColorDialog;
        private System.Windows.Forms.Button ButLoadShape;
        private System.Windows.Forms.TextBox ShapeLink;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ButCheck;
        private System.Windows.Forms.TextBox TestValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button ButSyntax;
        private System.Windows.Forms.Label CursorPosition;
    }
}