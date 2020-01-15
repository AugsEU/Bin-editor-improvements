namespace SMSRallyEditor
{
    partial class VectorInput
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
            this.XLbl = new System.Windows.Forms.Label();
            this.YLbl = new System.Windows.Forms.Label();
            this.ZLbl = new System.Windows.Forms.Label();
            this.XUpDown = new System.Windows.Forms.NumericUpDown();
            this.YUpDown = new System.Windows.Forms.NumericUpDown();
            this.ZUpDown = new System.Windows.Forms.NumericUpDown();
            this.OKBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.XUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // XLbl
            // 
            this.XLbl.AutoSize = true;
            this.XLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XLbl.Location = new System.Drawing.Point(16, 11);
            this.XLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.XLbl.Name = "XLbl";
            this.XLbl.Size = new System.Drawing.Size(37, 29);
            this.XLbl.TabIndex = 0;
            this.XLbl.Text = "X:";
            // 
            // YLbl
            // 
            this.YLbl.AutoSize = true;
            this.YLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YLbl.Location = new System.Drawing.Point(16, 43);
            this.YLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.YLbl.Name = "YLbl";
            this.YLbl.Size = new System.Drawing.Size(36, 29);
            this.YLbl.TabIndex = 1;
            this.YLbl.Text = "Y:";
            // 
            // ZLbl
            // 
            this.ZLbl.AutoSize = true;
            this.ZLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZLbl.Location = new System.Drawing.Point(16, 76);
            this.ZLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ZLbl.Name = "ZLbl";
            this.ZLbl.Size = new System.Drawing.Size(35, 29);
            this.ZLbl.TabIndex = 2;
            this.ZLbl.Text = "Z:";
            // 
            // XUpDown
            // 
            this.XUpDown.DecimalPlaces = 4;
            this.XUpDown.Location = new System.Drawing.Point(67, 15);
            this.XUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.XUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.XUpDown.Minimum = new decimal(new int[] {
            32767,
            0,
            0,
            -2147483648});
            this.XUpDown.Name = "XUpDown";
            this.XUpDown.Size = new System.Drawing.Size(160, 22);
            this.XUpDown.TabIndex = 3;
            // 
            // YUpDown
            // 
            this.YUpDown.DecimalPlaces = 4;
            this.YUpDown.Location = new System.Drawing.Point(67, 47);
            this.YUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.YUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.YUpDown.Minimum = new decimal(new int[] {
            32767,
            0,
            0,
            -2147483648});
            this.YUpDown.Name = "YUpDown";
            this.YUpDown.Size = new System.Drawing.Size(160, 22);
            this.YUpDown.TabIndex = 4;
            // 
            // ZUpDown
            // 
            this.ZUpDown.DecimalPlaces = 4;
            this.ZUpDown.Location = new System.Drawing.Point(67, 79);
            this.ZUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ZUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.ZUpDown.Minimum = new decimal(new int[] {
            32767,
            0,
            0,
            -2147483648});
            this.ZUpDown.Name = "ZUpDown";
            this.ZUpDown.Size = new System.Drawing.Size(160, 22);
            this.ZUpDown.TabIndex = 5;
            // 
            // OKBtn
            // 
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(136, 111);
            this.OKBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(100, 28);
            this.OKBtn.TabIndex = 6;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // VectorInput
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 153);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.ZUpDown);
            this.Controls.Add(this.YUpDown);
            this.Controls.Add(this.XUpDown);
            this.Controls.Add(this.ZLbl);
            this.Controls.Add(this.YLbl);
            this.Controls.Add(this.XLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.Name = "VectorInput";
            this.Text = "Input vector";
            ((System.ComponentModel.ISupportInitialize)(this.XUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label XLbl;
        private System.Windows.Forms.Label YLbl;
        private System.Windows.Forms.Label ZLbl;
        private System.Windows.Forms.NumericUpDown XUpDown;
        private System.Windows.Forms.NumericUpDown YUpDown;
        private System.Windows.Forms.NumericUpDown ZUpDown;
        private System.Windows.Forms.Button OKBtn;
    }
}