namespace SMSRallyEditor
{
    partial class RallyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RallyForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.remButton = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.remButton2 = new System.Windows.Forms.Button();
            this.addButton2 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.xUpDown = new System.Windows.Forms.NumericUpDown();
            this.yUpDown = new System.Windows.Forms.NumericUpDown();
            this.zUpDown = new System.Windows.Forms.NumericUpDown();
            this.u1UpDown = new System.Windows.Forms.NumericUpDown();
            this.yawUpDown = new System.Windows.Forms.NumericUpDown();
            this.pitchUpDown = new System.Windows.Forms.NumericUpDown();
            this.u2UpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rollUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.speedUpDown = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.endUpDown = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.startUpDown = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.u7UpDown = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.u6UpDown = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.u5UpDown = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.u4UpDown = new System.Windows.Forms.NumericUpDown();
            this.u3UpDown = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.u8UpDown = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.DupBtn = new System.Windows.Forms.Button();
            this.TranslateBtn = new System.Windows.Forms.Button();
            this.MovementLbl = new System.Windows.Forms.Label();
            this.PositionLbl = new System.Windows.Forms.Label();
            this.ConnectionsLbl = new System.Windows.Forms.Label();
            this.MovementPanel = new System.Windows.Forms.Panel();
            this.PositionPanel = new System.Windows.Forms.Panel();
            this.ConnectionsPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.SubSurf = new System.Windows.Forms.Button();
            this.InsAfter = new System.Windows.Forms.Button();
            this.SetPositionToCamera = new System.Windows.Forms.Button();
            this.connectloop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.xUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u1UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yawUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rollUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u7UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u6UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u5UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u4UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u3UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.u8UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "scene.ral";
            this.openFileDialog1.Filter = "Rally File (*.ral)|*.ral|All Files|*.*";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(17, 16);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(236, 340);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton.Location = new System.Drawing.Point(137, 371);
            this.addButton.Margin = new System.Windows.Forms.Padding(4);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(117, 28);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // remButton
            // 
            this.remButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.remButton.Location = new System.Drawing.Point(16, 371);
            this.remButton.Margin = new System.Windows.Forms.Padding(4);
            this.remButton.Name = "remButton";
            this.remButton.Size = new System.Drawing.Size(113, 28);
            this.remButton.TabIndex = 1;
            this.remButton.Text = "Remove";
            this.remButton.UseVisualStyleBackColor = true;
            this.remButton.Click += new System.EventHandler(this.remButton_Click);
            // 
            // listBox2
            // 
            this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 16;
            this.listBox2.Location = new System.Drawing.Point(264, 16);
            this.listBox2.Margin = new System.Windows.Forms.Padding(4);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(236, 340);
            this.listBox2.TabIndex = 3;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // remButton2
            // 
            this.remButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.remButton2.Location = new System.Drawing.Point(264, 371);
            this.remButton2.Margin = new System.Windows.Forms.Padding(4);
            this.remButton2.Name = "remButton2";
            this.remButton2.Size = new System.Drawing.Size(111, 28);
            this.remButton2.TabIndex = 4;
            this.remButton2.Text = "Remove";
            this.remButton2.UseVisualStyleBackColor = true;
            this.remButton2.Click += new System.EventHandler(this.remButton2_Click);
            // 
            // addButton2
            // 
            this.addButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton2.Location = new System.Drawing.Point(381, 371);
            this.addButton2.Margin = new System.Windows.Forms.Padding(4);
            this.addButton2.Name = "addButton2";
            this.addButton2.Size = new System.Drawing.Size(119, 28);
            this.addButton2.TabIndex = 5;
            this.addButton2.Text = "Insert Before";
            this.addButton2.UseVisualStyleBackColor = true;
            this.addButton2.Click += new System.EventHandler(this.addButton2_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "scene.ral";
            this.saveFileDialog1.Filter = "Rally File (*.ral)|*.ral|All Files|*.*";
            // 
            // xUpDown
            // 
            this.xUpDown.Location = new System.Drawing.Point(581, 46);
            this.xUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.xUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.xUpDown.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.xUpDown.Name = "xUpDown";
            this.xUpDown.Size = new System.Drawing.Size(153, 22);
            this.xUpDown.TabIndex = 8;
            this.xUpDown.ValueChanged += new System.EventHandler(this.xUpDown_ValueChanged);
            // 
            // yUpDown
            // 
            this.yUpDown.Location = new System.Drawing.Point(581, 78);
            this.yUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.yUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.yUpDown.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.yUpDown.Name = "yUpDown";
            this.yUpDown.Size = new System.Drawing.Size(153, 22);
            this.yUpDown.TabIndex = 9;
            this.yUpDown.ValueChanged += new System.EventHandler(this.yUpDown_ValueChanged);
            // 
            // zUpDown
            // 
            this.zUpDown.Location = new System.Drawing.Point(581, 110);
            this.zUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.zUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.zUpDown.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.zUpDown.Name = "zUpDown";
            this.zUpDown.Size = new System.Drawing.Size(153, 22);
            this.zUpDown.TabIndex = 10;
            this.zUpDown.ValueChanged += new System.EventHandler(this.zUpDown_ValueChanged);
            // 
            // u1UpDown
            // 
            this.u1UpDown.Location = new System.Drawing.Point(813, 46);
            this.u1UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u1UpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.u1UpDown.Name = "u1UpDown";
            this.u1UpDown.Size = new System.Drawing.Size(153, 22);
            this.u1UpDown.TabIndex = 11;
            this.u1UpDown.ValueChanged += new System.EventHandler(this.u1UpDown_ValueChanged);
            // 
            // yawUpDown
            // 
            this.yawUpDown.Location = new System.Drawing.Point(581, 233);
            this.yawUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.yawUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.yawUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.yawUpDown.Name = "yawUpDown";
            this.yawUpDown.Size = new System.Drawing.Size(153, 22);
            this.yawUpDown.TabIndex = 15;
            this.yawUpDown.ValueChanged += new System.EventHandler(this.yawUpDown_ValueChanged);
            // 
            // pitchUpDown
            // 
            this.pitchUpDown.Location = new System.Drawing.Point(581, 201);
            this.pitchUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.pitchUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.pitchUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.pitchUpDown.Name = "pitchUpDown";
            this.pitchUpDown.Size = new System.Drawing.Size(153, 22);
            this.pitchUpDown.TabIndex = 14;
            this.pitchUpDown.ValueChanged += new System.EventHandler(this.pitchUpDown_ValueChanged);
            // 
            // u2UpDown
            // 
            this.u2UpDown.Hexadecimal = true;
            this.u2UpDown.Location = new System.Drawing.Point(581, 169);
            this.u2UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u2UpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.u2UpDown.Name = "u2UpDown";
            this.u2UpDown.Size = new System.Drawing.Size(67, 22);
            this.u2UpDown.TabIndex = 12;
            this.u2UpDown.ValueChanged += new System.EventHandler(this.u2UpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(523, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(523, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(523, 112);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 17);
            this.label3.TabIndex = 16;
            this.label3.Text = "Z";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(523, 203);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 19;
            this.label4.Text = "Value 1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(523, 171);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "Flags";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(755, 48);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 17);
            this.label6.TabIndex = 17;
            this.label6.Text = "Count";
            // 
            // rollUpDown
            // 
            this.rollUpDown.Location = new System.Drawing.Point(581, 263);
            this.rollUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.rollUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.rollUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.rollUpDown.Name = "rollUpDown";
            this.rollUpDown.Size = new System.Drawing.Size(153, 22);
            this.rollUpDown.TabIndex = 16;
            this.rollUpDown.ValueChanged += new System.EventHandler(this.rollUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(523, 235);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 21;
            this.label7.Text = "Value 2";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(523, 266);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 17);
            this.label8.TabIndex = 22;
            this.label8.Text = "Value 3";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(523, 298);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 17);
            this.label9.TabIndex = 24;
            this.label9.Text = "Value 4";
            // 
            // speedUpDown
            // 
            this.speedUpDown.Location = new System.Drawing.Point(581, 295);
            this.speedUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.speedUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.speedUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.speedUpDown.Name = "speedUpDown";
            this.speedUpDown.Size = new System.Drawing.Size(153, 22);
            this.speedUpDown.TabIndex = 17;
            this.speedUpDown.ValueChanged += new System.EventHandler(this.speedUpDown_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(755, 108);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 17);
            this.label10.TabIndex = 28;
            this.label10.Text = "C2";
            // 
            // endUpDown
            // 
            this.endUpDown.Enabled = false;
            this.endUpDown.Location = new System.Drawing.Point(813, 106);
            this.endUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.endUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.endUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.endUpDown.Name = "endUpDown";
            this.endUpDown.Size = new System.Drawing.Size(153, 22);
            this.endUpDown.TabIndex = 19;
            this.endUpDown.ValueChanged += new System.EventHandler(this.endUpDown_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(755, 76);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(25, 17);
            this.label11.TabIndex = 26;
            this.label11.Text = "C1";
            // 
            // startUpDown
            // 
            this.startUpDown.Enabled = false;
            this.startUpDown.Location = new System.Drawing.Point(813, 74);
            this.startUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.startUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.startUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.startUpDown.Name = "startUpDown";
            this.startUpDown.Size = new System.Drawing.Size(153, 22);
            this.startUpDown.TabIndex = 18;
            this.startUpDown.ValueChanged += new System.EventHandler(this.startUpDown_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(755, 270);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(25, 17);
            this.label12.TabIndex = 39;
            this.label12.Text = "C7";
            // 
            // u7UpDown
            // 
            this.u7UpDown.Enabled = false;
            this.u7UpDown.Location = new System.Drawing.Point(813, 267);
            this.u7UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u7UpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.u7UpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.u7UpDown.Name = "u7UpDown";
            this.u7UpDown.Size = new System.Drawing.Size(153, 22);
            this.u7UpDown.TabIndex = 24;
            this.u7UpDown.ValueChanged += new System.EventHandler(this.u7UpDown_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(755, 238);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(25, 17);
            this.label13.TabIndex = 37;
            this.label13.Text = "C6";
            // 
            // u6UpDown
            // 
            this.u6UpDown.Enabled = false;
            this.u6UpDown.Location = new System.Drawing.Point(813, 235);
            this.u6UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u6UpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.u6UpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.u6UpDown.Name = "u6UpDown";
            this.u6UpDown.Size = new System.Drawing.Size(153, 22);
            this.u6UpDown.TabIndex = 23;
            this.u6UpDown.ValueChanged += new System.EventHandler(this.u6UpDown_ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(755, 206);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(25, 17);
            this.label14.TabIndex = 35;
            this.label14.Text = "C5";
            // 
            // u5UpDown
            // 
            this.u5UpDown.Enabled = false;
            this.u5UpDown.Location = new System.Drawing.Point(813, 203);
            this.u5UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u5UpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.u5UpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.u5UpDown.Name = "u5UpDown";
            this.u5UpDown.Size = new System.Drawing.Size(153, 22);
            this.u5UpDown.TabIndex = 22;
            this.u5UpDown.ValueChanged += new System.EventHandler(this.u5UpDown_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(755, 172);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(25, 17);
            this.label15.TabIndex = 33;
            this.label15.Text = "C4";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(755, 140);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(25, 17);
            this.label16.TabIndex = 32;
            this.label16.Text = "C3";
            // 
            // u4UpDown
            // 
            this.u4UpDown.Enabled = false;
            this.u4UpDown.Location = new System.Drawing.Point(813, 170);
            this.u4UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u4UpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.u4UpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.u4UpDown.Name = "u4UpDown";
            this.u4UpDown.Size = new System.Drawing.Size(153, 22);
            this.u4UpDown.TabIndex = 21;
            this.u4UpDown.ValueChanged += new System.EventHandler(this.u4UpDown_ValueChanged);
            // 
            // u3UpDown
            // 
            this.u3UpDown.Enabled = false;
            this.u3UpDown.Location = new System.Drawing.Point(813, 138);
            this.u3UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u3UpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.u3UpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.u3UpDown.Name = "u3UpDown";
            this.u3UpDown.Size = new System.Drawing.Size(153, 22);
            this.u3UpDown.TabIndex = 20;
            this.u3UpDown.ValueChanged += new System.EventHandler(this.u3UpDown_ValueChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(755, 302);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(25, 17);
            this.label21.TabIndex = 44;
            this.label21.Text = "C8";
            // 
            // u8UpDown
            // 
            this.u8UpDown.Enabled = false;
            this.u8UpDown.Location = new System.Drawing.Point(813, 299);
            this.u8UpDown.Margin = new System.Windows.Forms.Padding(4);
            this.u8UpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.u8UpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.u8UpDown.Name = "u8UpDown";
            this.u8UpDown.Size = new System.Drawing.Size(153, 22);
            this.u8UpDown.TabIndex = 25;
            this.u8UpDown.ValueChanged += new System.EventHandler(this.u8UpDown_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Hexadecimal = true;
            this.numericUpDown1.Location = new System.Drawing.Point(668, 169);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(67, 22);
            this.numericUpDown1.TabIndex = 13;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // DupBtn
            // 
            this.DupBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DupBtn.Location = new System.Drawing.Point(17, 407);
            this.DupBtn.Margin = new System.Windows.Forms.Padding(4);
            this.DupBtn.Name = "DupBtn";
            this.DupBtn.Size = new System.Drawing.Size(112, 28);
            this.DupBtn.TabIndex = 60;
            this.DupBtn.Text = "Duplicate";
            this.DupBtn.UseVisualStyleBackColor = true;
            this.DupBtn.Click += new System.EventHandler(this.DupBtn_Click);
            // 
            // TranslateBtn
            // 
            this.TranslateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TranslateBtn.Location = new System.Drawing.Point(137, 407);
            this.TranslateBtn.Margin = new System.Windows.Forms.Padding(4);
            this.TranslateBtn.Name = "TranslateBtn";
            this.TranslateBtn.Size = new System.Drawing.Size(117, 28);
            this.TranslateBtn.TabIndex = 61;
            this.TranslateBtn.Text = "Translate";
            this.TranslateBtn.UseVisualStyleBackColor = true;
            this.TranslateBtn.Click += new System.EventHandler(this.TranslateBtn_Click);
            // 
            // MovementLbl
            // 
            this.MovementLbl.AutoSize = true;
            this.MovementLbl.Location = new System.Drawing.Point(524, 146);
            this.MovementLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MovementLbl.Name = "MovementLbl";
            this.MovementLbl.Size = new System.Drawing.Size(77, 17);
            this.MovementLbl.TabIndex = 62;
            this.MovementLbl.Text = "Movement:";
            // 
            // PositionLbl
            // 
            this.PositionLbl.AutoSize = true;
            this.PositionLbl.Location = new System.Drawing.Point(524, 16);
            this.PositionLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PositionLbl.Name = "PositionLbl";
            this.PositionLbl.Size = new System.Drawing.Size(62, 17);
            this.PositionLbl.TabIndex = 63;
            this.PositionLbl.Text = "Position:";
            // 
            // ConnectionsLbl
            // 
            this.ConnectionsLbl.AutoSize = true;
            this.ConnectionsLbl.Location = new System.Drawing.Point(755, 16);
            this.ConnectionsLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ConnectionsLbl.Name = "ConnectionsLbl";
            this.ConnectionsLbl.Size = new System.Drawing.Size(90, 17);
            this.ConnectionsLbl.TabIndex = 64;
            this.ConnectionsLbl.Text = "Connections:";
            // 
            // MovementPanel
            // 
            this.MovementPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MovementPanel.Location = new System.Drawing.Point(512, 143);
            this.MovementPanel.Margin = new System.Windows.Forms.Padding(4);
            this.MovementPanel.Name = "MovementPanel";
            this.MovementPanel.Size = new System.Drawing.Size(233, 181);
            this.MovementPanel.TabIndex = 65;
            // 
            // PositionPanel
            // 
            this.PositionPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PositionPanel.Location = new System.Drawing.Point(512, 6);
            this.PositionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.PositionPanel.Name = "PositionPanel";
            this.PositionPanel.Size = new System.Drawing.Size(233, 136);
            this.PositionPanel.TabIndex = 66;
            // 
            // ConnectionsPanel
            // 
            this.ConnectionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ConnectionsPanel.Location = new System.Drawing.Point(747, 6);
            this.ConnectionsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.ConnectionsPanel.Name = "ConnectionsPanel";
            this.ConnectionsPanel.Size = new System.Drawing.Size(225, 318);
            this.ConnectionsPanel.TabIndex = 67;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(137, 443);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 28);
            this.button1.TabIndex = 68;
            this.button1.Text = "Scale";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // SubSurf
            // 
            this.SubSurf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SubSurf.Location = new System.Drawing.Point(17, 443);
            this.SubSurf.Margin = new System.Windows.Forms.Padding(4);
            this.SubSurf.Name = "SubSurf";
            this.SubSurf.Size = new System.Drawing.Size(112, 28);
            this.SubSurf.TabIndex = 69;
            this.SubSurf.Text = "Subdivide";
            this.SubSurf.UseVisualStyleBackColor = true;
            this.SubSurf.Click += new System.EventHandler(this.SubSurf_Click);
            // 
            // InsAfter
            // 
            this.InsAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.InsAfter.Location = new System.Drawing.Point(381, 407);
            this.InsAfter.Margin = new System.Windows.Forms.Padding(4);
            this.InsAfter.Name = "InsAfter";
            this.InsAfter.Size = new System.Drawing.Size(119, 28);
            this.InsAfter.TabIndex = 70;
            this.InsAfter.Text = "Insert After";
            this.InsAfter.UseVisualStyleBackColor = true;
            this.InsAfter.Click += new System.EventHandler(this.InsAfter_Click);
            // 
            // SetPositionToCamera
            // 
            this.SetPositionToCamera.Location = new System.Drawing.Point(512, 347);
            this.SetPositionToCamera.Name = "SetPositionToCamera";
            this.SetPositionToCamera.Size = new System.Drawing.Size(241, 28);
            this.SetPositionToCamera.TabIndex = 71;
            this.SetPositionToCamera.Text = "Set Position to Camera";
            this.SetPositionToCamera.UseVisualStyleBackColor = true;
            this.SetPositionToCamera.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // connectloop
            // 
            this.connectloop.Location = new System.Drawing.Point(264, 446);
            this.connectloop.Name = "connectloop";
            this.connectloop.Size = new System.Drawing.Size(236, 23);
            this.connectloop.TabIndex = 72;
            this.connectloop.Text = "Connect Points as Loop";
            this.connectloop.UseVisualStyleBackColor = true;
            this.connectloop.Click += new System.EventHandler(this.connectloop_Click);
            // 
            // RallyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 477);
            this.Controls.Add(this.connectloop);
            this.Controls.Add(this.SetPositionToCamera);
            this.Controls.Add(this.InsAfter);
            this.Controls.Add(this.SubSurf);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ConnectionsLbl);
            this.Controls.Add(this.PositionLbl);
            this.Controls.Add(this.MovementLbl);
            this.Controls.Add(this.TranslateBtn);
            this.Controls.Add(this.DupBtn);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.u8UpDown);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.u7UpDown);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.u6UpDown);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.u5UpDown);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.u4UpDown);
            this.Controls.Add(this.u3UpDown);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.endUpDown);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.startUpDown);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.speedUpDown);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.rollUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.yawUpDown);
            this.Controls.Add(this.pitchUpDown);
            this.Controls.Add(this.u2UpDown);
            this.Controls.Add(this.u1UpDown);
            this.Controls.Add(this.zUpDown);
            this.Controls.Add(this.yUpDown);
            this.Controls.Add(this.xUpDown);
            this.Controls.Add(this.remButton2);
            this.Controls.Add(this.addButton2);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.remButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.MovementPanel);
            this.Controls.Add(this.PositionPanel);
            this.Controls.Add(this.ConnectionsPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(974, 427);
            this.Name = "RallyForm";
            this.Text = "SMS Rail Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RallyForm_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u1UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yawUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rollUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u7UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u6UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u5UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u4UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u3UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.u8UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button remButton;
        public System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button remButton2;
        private System.Windows.Forms.Button addButton2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.NumericUpDown xUpDown;
        private System.Windows.Forms.NumericUpDown yUpDown;
        private System.Windows.Forms.NumericUpDown zUpDown;
        private System.Windows.Forms.NumericUpDown u1UpDown;
        private System.Windows.Forms.NumericUpDown yawUpDown;
        private System.Windows.Forms.NumericUpDown pitchUpDown;
        private System.Windows.Forms.NumericUpDown u2UpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown rollUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown speedUpDown;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown endUpDown;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown startUpDown;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown u7UpDown;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown u6UpDown;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown u5UpDown;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown u4UpDown;
        private System.Windows.Forms.NumericUpDown u3UpDown;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.NumericUpDown u8UpDown;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button DupBtn;
        private System.Windows.Forms.Button TranslateBtn;
        private System.Windows.Forms.Label MovementLbl;
        private System.Windows.Forms.Label PositionLbl;
        private System.Windows.Forms.Label ConnectionsLbl;
        private System.Windows.Forms.Panel MovementPanel;
        private System.Windows.Forms.Panel PositionPanel;
        private System.Windows.Forms.Panel ConnectionsPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button SubSurf;
        private System.Windows.Forms.Button InsAfter;
        private System.Windows.Forms.Button SetPositionToCamera;
        private System.Windows.Forms.Button connectloop;
    }
}

