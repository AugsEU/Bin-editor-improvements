namespace SMSRallyEditor
{
    partial class Subdivide
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
            this.ZLbl = new System.Windows.Forms.Label();
            this.CountUpDown = new System.Windows.Forms.NumericUpDown();
            this.OKBtn = new System.Windows.Forms.Button();
            this.SchemeType = new System.Windows.Forms.RadioButton();
            this.RadioGroup = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CountUpDown)).BeginInit();
            this.RadioGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // ZLbl
            // 
            this.ZLbl.AutoSize = true;
            this.ZLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZLbl.Location = new System.Drawing.Point(8, 98);
            this.ZLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ZLbl.Name = "ZLbl";
            this.ZLbl.Size = new System.Drawing.Size(49, 17);
            this.ZLbl.TabIndex = 2;
            this.ZLbl.Text = "Count:";
            this.ZLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // CountUpDown
            // 
            this.CountUpDown.Location = new System.Drawing.Point(56, 96);
            this.CountUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CountUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.CountUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CountUpDown.Name = "CountUpDown";
            this.CountUpDown.Size = new System.Drawing.Size(63, 22);
            this.CountUpDown.TabIndex = 5;
            this.CountUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // OKBtn
            // 
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(127, 92);
            this.OKBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(100, 28);
            this.OKBtn.TabIndex = 6;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // SchemeType
            // 
            this.SchemeType.AutoSize = true;
            this.SchemeType.Checked = true;
            this.SchemeType.Location = new System.Drawing.Point(4, 4);
            this.SchemeType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SchemeType.Name = "SchemeType";
            this.SchemeType.Size = new System.Drawing.Size(75, 21);
            this.SchemeType.TabIndex = 7;
            this.SchemeType.TabStop = true;
            this.SchemeType.Text = "Chaikin";
            this.SchemeType.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.SchemeType.UseVisualStyleBackColor = true;
            // 
            // RadioGroup
            // 
            this.RadioGroup.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RadioGroup.Controls.Add(this.SchemeType);
            this.RadioGroup.Location = new System.Drawing.Point(12, 34);
            this.RadioGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RadioGroup.Name = "RadioGroup";
            this.RadioGroup.Size = new System.Drawing.Size(213, 50);
            this.RadioGroup.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Subdivision Scheme:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Subdivide
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 129);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RadioGroup);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.CountUpDown);
            this.Controls.Add(this.ZLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.Name = "Subdivide";
            this.Text = "Sub surf\'s up";
            ((System.ComponentModel.ISupportInitialize)(this.CountUpDown)).EndInit();
            this.RadioGroup.ResumeLayout(false);
            this.RadioGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label ZLbl;
        private System.Windows.Forms.NumericUpDown CountUpDown;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.RadioButton SchemeType;
        private System.Windows.Forms.Panel RadioGroup;
        private System.Windows.Forms.Label label1;
    }
}