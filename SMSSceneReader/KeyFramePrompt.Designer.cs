namespace SMSSceneReader
{
    partial class KeyFramePrompt
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
            this.TimeUpDn = new System.Windows.Forms.NumericUpDown();
            this.AtPositionLbl = new System.Windows.Forms.Label();
            this.AtZLbl = new System.Windows.Forms.Label();
            this.AtYLbl = new System.Windows.Forms.Label();
            this.AtXLbl = new System.Windows.Forms.Label();
            this.AtZUpDn = new System.Windows.Forms.NumericUpDown();
            this.AtYUpDn = new System.Windows.Forms.NumericUpDown();
            this.AtXUpDn = new System.Windows.Forms.NumericUpDown();
            this.OKBtn = new System.Windows.Forms.Button();
            this.FromPositionLbl = new System.Windows.Forms.Label();
            this.FromZLbl = new System.Windows.Forms.Label();
            this.FromYLbl = new System.Windows.Forms.Label();
            this.FromXLbl = new System.Windows.Forms.Label();
            this.FromZUpDn = new System.Windows.Forms.NumericUpDown();
            this.FromYUpDn = new System.Windows.Forms.NumericUpDown();
            this.FromXUpDn = new System.Windows.Forms.NumericUpDown();
            this.LookAtToCamera = new System.Windows.Forms.Button();
            this.LookFromToCamera = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.TimeUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtZUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtYUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtXUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FromZUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FromYUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FromXUpDn)).BeginInit();
            this.SuspendLayout();
            // 
            // TimeLbl
            // 
            this.TimeLbl.AutoSize = true;
            this.TimeLbl.Location = new System.Drawing.Point(17, 190);
            this.TimeLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TimeLbl.Name = "TimeLbl";
            this.TimeLbl.Size = new System.Drawing.Size(43, 17);
            this.TimeLbl.TabIndex = 26;
            this.TimeLbl.Text = "Time:";
            // 
            // TimeUpDn
            // 
            this.TimeUpDn.Location = new System.Drawing.Point(68, 190);
            this.TimeUpDn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TimeUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TimeUpDn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TimeUpDn.Name = "TimeUpDn";
            this.TimeUpDn.Size = new System.Drawing.Size(139, 22);
            this.TimeUpDn.TabIndex = 25;
            this.TimeUpDn.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // AtPositionLbl
            // 
            this.AtPositionLbl.AutoSize = true;
            this.AtPositionLbl.Location = new System.Drawing.Point(16, 11);
            this.AtPositionLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AtPositionLbl.Name = "AtPositionLbl";
            this.AtPositionLbl.Size = new System.Drawing.Size(120, 17);
            this.AtPositionLbl.TabIndex = 24;
            this.AtPositionLbl.Text = "\'Look At\' Position:";
            // 
            // AtZLbl
            // 
            this.AtZLbl.AutoSize = true;
            this.AtZLbl.Location = new System.Drawing.Point(16, 97);
            this.AtZLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AtZLbl.Name = "AtZLbl";
            this.AtZLbl.Size = new System.Drawing.Size(21, 17);
            this.AtZLbl.TabIndex = 23;
            this.AtZLbl.Text = "Z:";
            // 
            // AtYLbl
            // 
            this.AtYLbl.AutoSize = true;
            this.AtYLbl.Location = new System.Drawing.Point(16, 65);
            this.AtYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AtYLbl.Name = "AtYLbl";
            this.AtYLbl.Size = new System.Drawing.Size(21, 17);
            this.AtYLbl.TabIndex = 22;
            this.AtYLbl.Text = "Y:";
            // 
            // AtXLbl
            // 
            this.AtXLbl.AutoSize = true;
            this.AtXLbl.Location = new System.Drawing.Point(16, 33);
            this.AtXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AtXLbl.Name = "AtXLbl";
            this.AtXLbl.Size = new System.Drawing.Size(21, 17);
            this.AtXLbl.TabIndex = 21;
            this.AtXLbl.Text = "X:";
            // 
            // AtZUpDn
            // 
            this.AtZUpDn.DecimalPlaces = 3;
            this.AtZUpDn.Location = new System.Drawing.Point(47, 95);
            this.AtZUpDn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.AtZUpDn.Size = new System.Drawing.Size(160, 22);
            this.AtZUpDn.TabIndex = 20;
            // 
            // AtYUpDn
            // 
            this.AtYUpDn.DecimalPlaces = 3;
            this.AtYUpDn.Location = new System.Drawing.Point(47, 63);
            this.AtYUpDn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.AtYUpDn.Size = new System.Drawing.Size(160, 22);
            this.AtYUpDn.TabIndex = 19;
            // 
            // AtXUpDn
            // 
            this.AtXUpDn.DecimalPlaces = 3;
            this.AtXUpDn.Location = new System.Drawing.Point(47, 31);
            this.AtXUpDn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.AtXUpDn.Size = new System.Drawing.Size(160, 22);
            this.AtXUpDn.TabIndex = 18;
            // 
            // OKBtn
            // 
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(319, 190);
            this.OKBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(100, 28);
            this.OKBtn.TabIndex = 27;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // FromPositionLbl
            // 
            this.FromPositionLbl.AutoSize = true;
            this.FromPositionLbl.Location = new System.Drawing.Point(224, 11);
            this.FromPositionLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FromPositionLbl.Name = "FromPositionLbl";
            this.FromPositionLbl.Size = new System.Drawing.Size(139, 17);
            this.FromPositionLbl.TabIndex = 34;
            this.FromPositionLbl.Text = "\'Look From\' Position:";
            // 
            // FromZLbl
            // 
            this.FromZLbl.AutoSize = true;
            this.FromZLbl.Location = new System.Drawing.Point(224, 97);
            this.FromZLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FromZLbl.Name = "FromZLbl";
            this.FromZLbl.Size = new System.Drawing.Size(21, 17);
            this.FromZLbl.TabIndex = 33;
            this.FromZLbl.Text = "Z:";
            // 
            // FromYLbl
            // 
            this.FromYLbl.AutoSize = true;
            this.FromYLbl.Location = new System.Drawing.Point(224, 65);
            this.FromYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FromYLbl.Name = "FromYLbl";
            this.FromYLbl.Size = new System.Drawing.Size(21, 17);
            this.FromYLbl.TabIndex = 32;
            this.FromYLbl.Text = "Y:";
            // 
            // FromXLbl
            // 
            this.FromXLbl.AutoSize = true;
            this.FromXLbl.Location = new System.Drawing.Point(224, 33);
            this.FromXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FromXLbl.Name = "FromXLbl";
            this.FromXLbl.Size = new System.Drawing.Size(21, 17);
            this.FromXLbl.TabIndex = 31;
            this.FromXLbl.Text = "X:";
            // 
            // FromZUpDn
            // 
            this.FromZUpDn.DecimalPlaces = 3;
            this.FromZUpDn.Location = new System.Drawing.Point(255, 95);
            this.FromZUpDn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FromZUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.FromZUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.FromZUpDn.Name = "FromZUpDn";
            this.FromZUpDn.Size = new System.Drawing.Size(160, 22);
            this.FromZUpDn.TabIndex = 30;
            // 
            // FromYUpDn
            // 
            this.FromYUpDn.DecimalPlaces = 3;
            this.FromYUpDn.Location = new System.Drawing.Point(255, 63);
            this.FromYUpDn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FromYUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.FromYUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.FromYUpDn.Name = "FromYUpDn";
            this.FromYUpDn.Size = new System.Drawing.Size(160, 22);
            this.FromYUpDn.TabIndex = 29;
            // 
            // FromXUpDn
            // 
            this.FromXUpDn.DecimalPlaces = 3;
            this.FromXUpDn.Location = new System.Drawing.Point(255, 31);
            this.FromXUpDn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FromXUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.FromXUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.FromXUpDn.Name = "FromXUpDn";
            this.FromXUpDn.Size = new System.Drawing.Size(160, 22);
            this.FromXUpDn.TabIndex = 28;
            // 
            // LookAtToCamera
            // 
            this.LookAtToCamera.Location = new System.Drawing.Point(19, 139);
            this.LookAtToCamera.Name = "LookAtToCamera";
            this.LookAtToCamera.Size = new System.Drawing.Size(188, 23);
            this.LookAtToCamera.TabIndex = 35;
            this.LookAtToCamera.Text = "Set LookAt to Camera";
            this.LookAtToCamera.UseVisualStyleBackColor = true;
            this.LookAtToCamera.Click += new System.EventHandler(this.button1_Click);
            // 
            // LookFromToCamera
            // 
            this.LookFromToCamera.Location = new System.Drawing.Point(227, 139);
            this.LookFromToCamera.Name = "LookFromToCamera";
            this.LookFromToCamera.Size = new System.Drawing.Size(188, 23);
            this.LookFromToCamera.TabIndex = 36;
            this.LookFromToCamera.Text = "Set LookFrom to Camera";
            this.LookFromToCamera.UseVisualStyleBackColor = true;
            this.LookFromToCamera.Click += new System.EventHandler(this.LookFromToCamera_Click);
            // 
            // KeyFramePrompt
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 231);
            this.Controls.Add(this.LookFromToCamera);
            this.Controls.Add(this.LookAtToCamera);
            this.Controls.Add(this.FromPositionLbl);
            this.Controls.Add(this.FromZLbl);
            this.Controls.Add(this.FromYLbl);
            this.Controls.Add(this.FromXLbl);
            this.Controls.Add(this.FromZUpDn);
            this.Controls.Add(this.FromYUpDn);
            this.Controls.Add(this.FromXUpDn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.TimeLbl);
            this.Controls.Add(this.TimeUpDn);
            this.Controls.Add(this.AtPositionLbl);
            this.Controls.Add(this.AtZLbl);
            this.Controls.Add(this.AtYLbl);
            this.Controls.Add(this.AtXLbl);
            this.Controls.Add(this.AtZUpDn);
            this.Controls.Add(this.AtYUpDn);
            this.Controls.Add(this.AtXUpDn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "KeyFramePrompt";
            this.Text = "Enter initial data...";
            ((System.ComponentModel.ISupportInitialize)(this.TimeUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtZUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtYUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AtXUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FromZUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FromYUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FromXUpDn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TimeLbl;
        private System.Windows.Forms.NumericUpDown TimeUpDn;
        private System.Windows.Forms.Label AtPositionLbl;
        private System.Windows.Forms.Label AtZLbl;
        private System.Windows.Forms.Label AtYLbl;
        private System.Windows.Forms.Label AtXLbl;
        private System.Windows.Forms.NumericUpDown AtZUpDn;
        private System.Windows.Forms.NumericUpDown AtYUpDn;
        private System.Windows.Forms.NumericUpDown AtXUpDn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Label FromPositionLbl;
        private System.Windows.Forms.Label FromZLbl;
        private System.Windows.Forms.Label FromYLbl;
        private System.Windows.Forms.Label FromXLbl;
        private System.Windows.Forms.NumericUpDown FromZUpDn;
        private System.Windows.Forms.NumericUpDown FromYUpDn;
        private System.Windows.Forms.NumericUpDown FromXUpDn;
        private System.Windows.Forms.Button LookAtToCamera;
        private System.Windows.Forms.Button LookFromToCamera;
    }
}