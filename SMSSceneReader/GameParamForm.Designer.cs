namespace SMSSceneReader
{
    partial class GameParamForm
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stringBox = new System.Windows.Forms.TextBox();
            this.sizeUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.byteUpDown = new System.Windows.Forms.NumericUpDown();
            this.wordUpDown = new System.Windows.Forms.NumericUpDown();
            this.intUpDown = new System.Windows.Forms.NumericUpDown();
            this.singleUpDown = new System.Windows.Forms.NumericUpDown();
            this.doubleUpDown = new System.Windows.Forms.NumericUpDown();
            this.vectorBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.byteUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.singleUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.doubleUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(150, 316);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(168, 12);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(162, 316);
            this.listBox2.TabIndex = 1;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(336, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Byte";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(337, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Word";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(337, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Int";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(337, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Single";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(337, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Double";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(337, 172);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Vector";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stringBox);
            this.groupBox1.Location = new System.Drawing.Point(336, 195);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(152, 133);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "String";
            // 
            // stringBox
            // 
            this.stringBox.Location = new System.Drawing.Point(6, 19);
            this.stringBox.Multiline = true;
            this.stringBox.Name = "stringBox";
            this.stringBox.Size = new System.Drawing.Size(139, 108);
            this.stringBox.TabIndex = 9;
            this.stringBox.TextChanged += new System.EventHandler(this.stringBox_TextChanged);
            // 
            // sizeUpDown
            // 
            this.sizeUpDown.Location = new System.Drawing.Point(370, 12);
            this.sizeUpDown.Name = "sizeUpDown";
            this.sizeUpDown.Size = new System.Drawing.Size(118, 20);
            this.sizeUpDown.TabIndex = 2;
            this.sizeUpDown.ValueChanged += new System.EventHandler(this.sizeUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(337, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Size";
            // 
            // byteUpDown
            // 
            this.byteUpDown.Location = new System.Drawing.Point(395, 40);
            this.byteUpDown.Name = "byteUpDown";
            this.byteUpDown.Size = new System.Drawing.Size(93, 20);
            this.byteUpDown.TabIndex = 3;
            this.byteUpDown.ValueChanged += new System.EventHandler(this.byteUpDown_ValueChanged);
            // 
            // wordUpDown
            // 
            this.wordUpDown.Location = new System.Drawing.Point(395, 66);
            this.wordUpDown.Name = "wordUpDown";
            this.wordUpDown.Size = new System.Drawing.Size(93, 20);
            this.wordUpDown.TabIndex = 4;
            this.wordUpDown.ValueChanged += new System.EventHandler(this.wordUpDown_ValueChanged);
            // 
            // intUpDown
            // 
            this.intUpDown.Location = new System.Drawing.Point(395, 92);
            this.intUpDown.Name = "intUpDown";
            this.intUpDown.Size = new System.Drawing.Size(93, 20);
            this.intUpDown.TabIndex = 5;
            this.intUpDown.ValueChanged += new System.EventHandler(this.intUpDown_ValueChanged);
            // 
            // singleUpDown
            // 
            this.singleUpDown.DecimalPlaces = 7;
            this.singleUpDown.Location = new System.Drawing.Point(395, 118);
            this.singleUpDown.Name = "singleUpDown";
            this.singleUpDown.Size = new System.Drawing.Size(93, 20);
            this.singleUpDown.TabIndex = 6;
            this.singleUpDown.ValueChanged += new System.EventHandler(this.singleUpDown_ValueChanged);
            // 
            // doubleUpDown
            // 
            this.doubleUpDown.DecimalPlaces = 14;
            this.doubleUpDown.Location = new System.Drawing.Point(395, 144);
            this.doubleUpDown.Name = "doubleUpDown";
            this.doubleUpDown.Size = new System.Drawing.Size(93, 20);
            this.doubleUpDown.TabIndex = 7;
            this.doubleUpDown.ValueChanged += new System.EventHandler(this.doubleUpDown_ValueChanged);
            // 
            // vectorBox
            // 
            this.vectorBox.Location = new System.Drawing.Point(395, 169);
            this.vectorBox.Name = "vectorBox";
            this.vectorBox.Size = new System.Drawing.Size(93, 20);
            this.vectorBox.TabIndex = 8;
            this.vectorBox.TextChanged += new System.EventHandler(this.vectorBox_TextChanged);
            // 
            // GameParamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 338);
            this.Controls.Add(this.vectorBox);
            this.Controls.Add(this.doubleUpDown);
            this.Controls.Add(this.singleUpDown);
            this.Controls.Add(this.intUpDown);
            this.Controls.Add(this.wordUpDown);
            this.Controls.Add(this.byteUpDown);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.sizeUpDown);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.MaximumSize = new System.Drawing.Size(509, 377);
            this.MinimumSize = new System.Drawing.Size(509, 377);
            this.Name = "GameParamForm";
            this.Text = "SMS Parameter Editor";
            this.Load += new System.EventHandler(this.GameParamForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.byteUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.singleUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.doubleUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox stringBox;
        private System.Windows.Forms.NumericUpDown sizeUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown byteUpDown;
        private System.Windows.Forms.NumericUpDown wordUpDown;
        private System.Windows.Forms.NumericUpDown intUpDown;
        private System.Windows.Forms.NumericUpDown singleUpDown;
        private System.Windows.Forms.NumericUpDown doubleUpDown;
        private System.Windows.Forms.TextBox vectorBox;
    }
}