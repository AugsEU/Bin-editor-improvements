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
            this.ZLbl.Location = new System.Drawing.Point(6, 80);
            this.ZLbl.Name = "ZLbl";
            this.ZLbl.Size = new System.Drawing.Size(38, 13);
            this.ZLbl.TabIndex = 2;
            this.ZLbl.Text = "Count:";
            this.ZLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ZLbl.UseWaitCursor = true;
            // 
            // CountUpDown
            // 
            this.CountUpDown.Location = new System.Drawing.Point(42, 78);
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
            this.CountUpDown.Size = new System.Drawing.Size(47, 20);
            this.CountUpDown.TabIndex = 5;
            this.CountUpDown.UseWaitCursor = true;
            this.CountUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // OKBtn
            // 
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(95, 75);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 6;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.UseWaitCursor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // SchemeType
            // 
            this.SchemeType.AutoSize = true;
            this.SchemeType.Location = new System.Drawing.Point(3, 3);
            this.SchemeType.Name = "SchemeType";
            this.SchemeType.Size = new System.Drawing.Size(60, 17);
            this.SchemeType.TabIndex = 7;
            this.SchemeType.TabStop = true;
            this.SchemeType.Text = "Chaikin";
            this.SchemeType.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.SchemeType.UseVisualStyleBackColor = true;
            this.SchemeType.UseWaitCursor = true;
            // 
            // RadioGroup
            // 
            this.RadioGroup.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RadioGroup.Controls.Add(this.SchemeType);
            this.RadioGroup.Location = new System.Drawing.Point(9, 28);
            this.RadioGroup.Name = "RadioGroup";
            this.RadioGroup.Size = new System.Drawing.Size(161, 41);
            this.RadioGroup.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Subdivision Scheme:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.UseWaitCursor = true;
            // 
            // Subdivide
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(176, 105);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RadioGroup);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.CountUpDown);
            this.Controls.Add(this.ZLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "Subdivide";
            this.Text = "Sub surf\'s up";
            this.UseWaitCursor = true;
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