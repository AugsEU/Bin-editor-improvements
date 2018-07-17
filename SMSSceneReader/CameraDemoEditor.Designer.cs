namespace SMSSceneReader
{
    partial class CameraDemoEditor
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
            this.TimeLinePanel = new System.Windows.Forms.Panel();
            this.TimeLine = new System.Windows.Forms.TableLayoutPanel();
            this.TimeLineLbl = new System.Windows.Forms.Label();
            this.XUpDn = new System.Windows.Forms.NumericUpDown();
            this.YUpDn = new System.Windows.Forms.NumericUpDown();
            this.ZUpDn = new System.Windows.Forms.NumericUpDown();
            this.XLbl = new System.Windows.Forms.Label();
            this.YLbl = new System.Windows.Forms.Label();
            this.ZLbl = new System.Windows.Forms.Label();
            this.PositionLbl = new System.Windows.Forms.Label();
            this.KeyFrameTypeLbl = new System.Windows.Forms.Label();
            this.AddLookBtn = new System.Windows.Forms.Button();
            this.RemoveLookBtn = new System.Windows.Forms.Button();
            this.DurationLbl = new System.Windows.Forms.Label();
            this.DurationUpDn = new System.Windows.Forms.NumericUpDown();
            this.TimeLbl = new System.Windows.Forms.Label();
            this.TimeUpDn = new System.Windows.Forms.NumericUpDown();
            this.ZoomLbl = new System.Windows.Forms.Label();
            this.ZoomInBtn = new System.Windows.Forms.Button();
            this.ZoomOutBtn = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TimeLinePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DurationUpDn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimeUpDn)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TimeLinePanel
            // 
            this.TimeLinePanel.AutoScroll = true;
            this.TimeLinePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TimeLinePanel.Controls.Add(this.TimeLine);
            this.TimeLinePanel.Location = new System.Drawing.Point(161, 73);
            this.TimeLinePanel.Name = "TimeLinePanel";
            this.TimeLinePanel.Size = new System.Drawing.Size(590, 158);
            this.TimeLinePanel.TabIndex = 0;
            // 
            // TimeLine
            // 
            this.TimeLine.ColumnCount = 1;
            this.TimeLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TimeLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TimeLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TimeLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TimeLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TimeLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TimeLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TimeLine.Location = new System.Drawing.Point(0, 0);
            this.TimeLine.Name = "TimeLine";
            this.TimeLine.RowCount = 2;
            this.TimeLine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TimeLine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TimeLine.Size = new System.Drawing.Size(587, 137);
            this.TimeLine.TabIndex = 0;
            this.TimeLine.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.TimeLine_CellPaint);
            this.TimeLine.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimeLine_MouseDown);
            // 
            // TimeLineLbl
            // 
            this.TimeLineLbl.AutoSize = true;
            this.TimeLineLbl.Location = new System.Drawing.Point(158, 57);
            this.TimeLineLbl.Name = "TimeLineLbl";
            this.TimeLineLbl.Size = new System.Drawing.Size(49, 13);
            this.TimeLineLbl.TabIndex = 1;
            this.TimeLineLbl.Text = "Timeline:";
            // 
            // XUpDn
            // 
            this.XUpDn.Location = new System.Drawing.Point(35, 73);
            this.XUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.XUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.XUpDn.Name = "XUpDn";
            this.XUpDn.Size = new System.Drawing.Size(120, 20);
            this.XUpDn.TabIndex = 2;
            this.XUpDn.ValueChanged += new System.EventHandler(this.XUpDn_ValueChanged);
            // 
            // YUpDn
            // 
            this.YUpDn.Location = new System.Drawing.Point(35, 99);
            this.YUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.YUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.YUpDn.Name = "YUpDn";
            this.YUpDn.Size = new System.Drawing.Size(120, 20);
            this.YUpDn.TabIndex = 3;
            this.YUpDn.ValueChanged += new System.EventHandler(this.YUpDn_ValueChanged);
            // 
            // ZUpDn
            // 
            this.ZUpDn.Location = new System.Drawing.Point(35, 125);
            this.ZUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ZUpDn.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.ZUpDn.Name = "ZUpDn";
            this.ZUpDn.Size = new System.Drawing.Size(120, 20);
            this.ZUpDn.TabIndex = 4;
            this.ZUpDn.ValueChanged += new System.EventHandler(this.ZUpDn_ValueChanged);
            // 
            // XLbl
            // 
            this.XLbl.AutoSize = true;
            this.XLbl.Location = new System.Drawing.Point(12, 75);
            this.XLbl.Name = "XLbl";
            this.XLbl.Size = new System.Drawing.Size(17, 13);
            this.XLbl.TabIndex = 5;
            this.XLbl.Text = "X:";
            // 
            // YLbl
            // 
            this.YLbl.AutoSize = true;
            this.YLbl.Location = new System.Drawing.Point(12, 101);
            this.YLbl.Name = "YLbl";
            this.YLbl.Size = new System.Drawing.Size(17, 13);
            this.YLbl.TabIndex = 6;
            this.YLbl.Text = "Y:";
            // 
            // ZLbl
            // 
            this.ZLbl.AutoSize = true;
            this.ZLbl.Location = new System.Drawing.Point(12, 127);
            this.ZLbl.Name = "ZLbl";
            this.ZLbl.Size = new System.Drawing.Size(17, 13);
            this.ZLbl.TabIndex = 7;
            this.ZLbl.Text = "Z:";
            // 
            // PositionLbl
            // 
            this.PositionLbl.AutoSize = true;
            this.PositionLbl.Location = new System.Drawing.Point(12, 57);
            this.PositionLbl.Name = "PositionLbl";
            this.PositionLbl.Size = new System.Drawing.Size(100, 13);
            this.PositionLbl.TabIndex = 8;
            this.PositionLbl.Text = "Key Frame Position:";
            // 
            // KeyFrameTypeLbl
            // 
            this.KeyFrameTypeLbl.AutoSize = true;
            this.KeyFrameTypeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyFrameTypeLbl.Location = new System.Drawing.Point(159, 34);
            this.KeyFrameTypeLbl.Name = "KeyFrameTypeLbl";
            this.KeyFrameTypeLbl.Size = new System.Drawing.Size(134, 20);
            this.KeyFrameTypeLbl.TabIndex = 9;
            this.KeyFrameTypeLbl.Text = "Select Key Frame";
            // 
            // AddLookBtn
            // 
            this.AddLookBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddLookBtn.Location = new System.Drawing.Point(756, 73);
            this.AddLookBtn.Name = "AddLookBtn";
            this.AddLookBtn.Size = new System.Drawing.Size(75, 75);
            this.AddLookBtn.TabIndex = 10;
            this.AddLookBtn.Text = "+";
            this.AddLookBtn.UseVisualStyleBackColor = true;
            this.AddLookBtn.Click += new System.EventHandler(this.AddLookBtn_Click);
            // 
            // RemoveLookBtn
            // 
            this.RemoveLookBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveLookBtn.Location = new System.Drawing.Point(756, 154);
            this.RemoveLookBtn.Name = "RemoveLookBtn";
            this.RemoveLookBtn.Size = new System.Drawing.Size(75, 75);
            this.RemoveLookBtn.TabIndex = 11;
            this.RemoveLookBtn.Text = "-";
            this.RemoveLookBtn.UseVisualStyleBackColor = true;
            this.RemoveLookBtn.Click += new System.EventHandler(this.RemoveLookBtn_Click);
            // 
            // DurationLbl
            // 
            this.DurationLbl.AutoSize = true;
            this.DurationLbl.Location = new System.Drawing.Point(658, 41);
            this.DurationLbl.Name = "DurationLbl";
            this.DurationLbl.Size = new System.Drawing.Size(50, 13);
            this.DurationLbl.TabIndex = 15;
            this.DurationLbl.Text = "Duration:";
            // 
            // DurationUpDn
            // 
            this.DurationUpDn.Location = new System.Drawing.Point(714, 39);
            this.DurationUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.DurationUpDn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DurationUpDn.Name = "DurationUpDn";
            this.DurationUpDn.Size = new System.Drawing.Size(120, 20);
            this.DurationUpDn.TabIndex = 14;
            this.DurationUpDn.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DurationUpDn.ValueChanged += new System.EventHandler(this.DurationUpDn_ValueChanged);
            // 
            // TimeLbl
            // 
            this.TimeLbl.AutoSize = true;
            this.TimeLbl.Location = new System.Drawing.Point(12, 166);
            this.TimeLbl.Name = "TimeLbl";
            this.TimeLbl.Size = new System.Drawing.Size(33, 13);
            this.TimeLbl.TabIndex = 17;
            this.TimeLbl.Text = "Time:";
            // 
            // TimeUpDn
            // 
            this.TimeUpDn.Location = new System.Drawing.Point(51, 164);
            this.TimeUpDn.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TimeUpDn.Name = "TimeUpDn";
            this.TimeUpDn.Size = new System.Drawing.Size(104, 20);
            this.TimeUpDn.TabIndex = 16;
            this.TimeUpDn.ValueChanged += new System.EventHandler(this.TimeUpDn_ValueChanged);
            // 
            // ZoomLbl
            // 
            this.ZoomLbl.AutoSize = true;
            this.ZoomLbl.Location = new System.Drawing.Point(434, 41);
            this.ZoomLbl.Name = "ZoomLbl";
            this.ZoomLbl.Size = new System.Drawing.Size(37, 13);
            this.ZoomLbl.TabIndex = 18;
            this.ZoomLbl.Text = "Zoom:";
            // 
            // ZoomInBtn
            // 
            this.ZoomInBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZoomInBtn.Location = new System.Drawing.Point(477, 34);
            this.ZoomInBtn.Name = "ZoomInBtn";
            this.ZoomInBtn.Size = new System.Drawing.Size(33, 33);
            this.ZoomInBtn.TabIndex = 19;
            this.ZoomInBtn.Text = "+";
            this.ZoomInBtn.UseVisualStyleBackColor = true;
            this.ZoomInBtn.Click += new System.EventHandler(this.ZoomInBtn_Click);
            // 
            // ZoomOutBtn
            // 
            this.ZoomOutBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZoomOutBtn.Location = new System.Drawing.Point(516, 34);
            this.ZoomOutBtn.Name = "ZoomOutBtn";
            this.ZoomOutBtn.Size = new System.Drawing.Size(33, 33);
            this.ZoomOutBtn.TabIndex = 20;
            this.ZoomOutBtn.Text = "-";
            this.ZoomOutBtn.UseVisualStyleBackColor = true;
            this.ZoomOutBtn.Click += new System.EventHandler(this.ZoomOutBtn_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(846, 24);
            this.menuStrip1.TabIndex = 21;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "New";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // CameraDemoEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 250);
            this.Controls.Add(this.ZoomOutBtn);
            this.Controls.Add(this.ZoomInBtn);
            this.Controls.Add(this.ZoomLbl);
            this.Controls.Add(this.TimeLbl);
            this.Controls.Add(this.TimeUpDn);
            this.Controls.Add(this.DurationLbl);
            this.Controls.Add(this.DurationUpDn);
            this.Controls.Add(this.RemoveLookBtn);
            this.Controls.Add(this.AddLookBtn);
            this.Controls.Add(this.KeyFrameTypeLbl);
            this.Controls.Add(this.PositionLbl);
            this.Controls.Add(this.ZLbl);
            this.Controls.Add(this.YLbl);
            this.Controls.Add(this.XLbl);
            this.Controls.Add(this.ZUpDn);
            this.Controls.Add(this.YUpDn);
            this.Controls.Add(this.XUpDn);
            this.Controls.Add(this.TimeLineLbl);
            this.Controls.Add(this.TimeLinePanel);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CameraDemoEditor";
            this.Text = "Intro Editor";
            this.TimeLinePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.XUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DurationUpDn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimeUpDn)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel TimeLinePanel;
        private System.Windows.Forms.TableLayoutPanel TimeLine;
        private System.Windows.Forms.Label TimeLineLbl;
        private System.Windows.Forms.NumericUpDown XUpDn;
        private System.Windows.Forms.NumericUpDown YUpDn;
        private System.Windows.Forms.NumericUpDown ZUpDn;
        private System.Windows.Forms.Label XLbl;
        private System.Windows.Forms.Label YLbl;
        private System.Windows.Forms.Label ZLbl;
        private System.Windows.Forms.Label PositionLbl;
        private System.Windows.Forms.Label KeyFrameTypeLbl;
        private System.Windows.Forms.Button AddLookBtn;
        private System.Windows.Forms.Button RemoveLookBtn;
        private System.Windows.Forms.Label DurationLbl;
        private System.Windows.Forms.NumericUpDown DurationUpDn;
        private System.Windows.Forms.Label TimeLbl;
        private System.Windows.Forms.NumericUpDown TimeUpDn;
        private System.Windows.Forms.Label ZoomLbl;
        private System.Windows.Forms.Button ZoomInBtn;
        private System.Windows.Forms.Button ZoomOutBtn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    }
}