namespace SMSSceneReader
{
    partial class ArrayPrompt
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
            this.TimeLbl = new System.Windows.Forms.Label();
            this.CountUpDn = new System.Windows.Forms.NumericUpDown();
            this.AtPositionLbl = new System.Windows.Forms.Label();
            this.AtZLbl = new System.Windows.Forms.Label();
            this.AtYLbl = new System.Windows.Forms.Label();
            this.AtXLbl = new System.Windows.Forms.Label();
            this.AtZUpDn = new System.Windows.Forms.NumericUpDown();
            this.AtYUpDn = new System.Windows.Forms.NumericUpDown();
            this.AtXUpDn = new System.Windows.Forms.NumericUpDown();
            this.OKBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CountUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtZUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtYUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtXUpDn)).BeginInit();
            this.SuspendLayout();
            // 
            // TimeLbl
            // 
            this.TimeLbl.AutoSize = true;
            this.TimeLbl.Location = new System.Drawing.Point(168, 27);
            this.TimeLbl.Name = "TimeLbl";
            this.TimeLbl.Size = new System.Drawing.Size(38, 13);
            this.TimeLbl.TabIndex = 26;
            this.TimeLbl.Text = "Count:";
            // 
            // CountUpDn
            // 
            this.CountUpDn.Location = new System.Drawing.Point(207, 25);
            this.CountUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.CountUpDn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CountUpDn.Name = "CountUpDn";
            this.CountUpDn.Size = new System.Drawing.Size(104, 20);
            this.CountUpDn.TabIndex = 25;
            this.CountUpDn.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // AtPositionLbl
            // 
            this.AtPositionLbl.AutoSize = true;
            this.AtPositionLbl.Location = new System.Drawing.Point(12, 9);
            this.AtPositionLbl.Name = "AtPositionLbl";
            this.AtPositionLbl.Size = new System.Drawing.Size(105, 13);
            this.AtPositionLbl.TabIndex = 24;
            this.AtPositionLbl.Text = "DisplacementVector:";
            // 
            // AtZLbl
            // 
            this.AtZLbl.AutoSize = true;
            this.AtZLbl.Location = new System.Drawing.Point(12, 79);
            this.AtZLbl.Name = "AtZLbl";
            this.AtZLbl.Size = new System.Drawing.Size(17, 13);
            this.AtZLbl.TabIndex = 23;
            this.AtZLbl.Text = "Z:";
            // 
            // AtYLbl
            // 
            this.AtYLbl.AutoSize = true;
            this.AtYLbl.Location = new System.Drawing.Point(12, 53);
            this.AtYLbl.Name = "AtYLbl";
            this.AtYLbl.Size = new System.Drawing.Size(17, 13);
            this.AtYLbl.TabIndex = 22;
            this.AtYLbl.Text = "Y:";
            // 
            // AtXLbl
            // 
            this.AtXLbl.AutoSize = true;
            this.AtXLbl.Location = new System.Drawing.Point(12, 27);
            this.AtXLbl.Name = "AtXLbl";
            this.AtXLbl.Size = new System.Drawing.Size(17, 13);
            this.AtXLbl.TabIndex = 21;
            this.AtXLbl.Text = "X:";
            // 
            // AtZUpDn
            // 
            this.AtZUpDn.DecimalPlaces = 3;
            this.AtZUpDn.Location = new System.Drawing.Point(35, 77);
            this.AtZUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.AtZUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.AtZUpDn.Name = "AtZUpDn";
            this.AtZUpDn.Size = new System.Drawing.Size(120, 20);
            this.AtZUpDn.TabIndex = 20;
            // 
            // AtYUpDn
            // 
            this.AtYUpDn.DecimalPlaces = 3;
            this.AtYUpDn.Location = new System.Drawing.Point(35, 51);
            this.AtYUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.AtYUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.AtYUpDn.Name = "AtYUpDn";
            this.AtYUpDn.Size = new System.Drawing.Size(120, 20);
            this.AtYUpDn.TabIndex = 19;
            // 
            // AtXUpDn
            // 
            this.AtXUpDn.DecimalPlaces = 3;
            this.AtXUpDn.Location = new System.Drawing.Point(35, 25);
            this.AtXUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.AtXUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.AtXUpDn.Name = "AtXUpDn";
            this.AtXUpDn.Size = new System.Drawing.Size(120, 20);
            this.AtXUpDn.TabIndex = 18;
            // 
            // OKBtn
            // 
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(234, 77);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 27;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // ArrayPrompt
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 106);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.TimeLbl);
            this.Controls.Add(this.CountUpDn);
            this.Controls.Add(this.AtPositionLbl);
            this.Controls.Add(this.AtZLbl);
            this.Controls.Add(this.AtYLbl);
            this.Controls.Add(this.AtXLbl);
            this.Controls.Add(this.AtZUpDn);
            this.Controls.Add(this.AtYUpDn);
            this.Controls.Add(this.AtXUpDn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ArrayPrompt";
            this.Text = "Enter initial data...";
            ((System.ComponentModel.ISupportInitialize)(this.CountUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtZUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtYUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtXUpDn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TimeLbl;
        private System.Windows.Forms.NumericUpDown CountUpDn;
        private System.Windows.Forms.Label AtPositionLbl;
        private System.Windows.Forms.Label AtZLbl;
        private System.Windows.Forms.Label AtYLbl;
        private System.Windows.Forms.Label AtXLbl;
        private System.Windows.Forms.NumericUpDown AtZUpDn;
        private System.Windows.Forms.NumericUpDown AtYUpDn;
        private System.Windows.Forms.NumericUpDown AtXUpDn;
        private System.Windows.Forms.Button OKBtn;
    }
}