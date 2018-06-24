namespace SMSManager
{
    partial class Settings
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
            this.beBox = new System.Windows.Forms.TextBox();
            this.dolphinBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.beBrowse = new System.Windows.Forms.Button();
            this.dolphinBrowse = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.defaultButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // beBox
            // 
            this.beBox.Location = new System.Drawing.Point(70, 24);
            this.beBox.Name = "beBox";
            this.beBox.Size = new System.Drawing.Size(197, 20);
            this.beBox.TabIndex = 0;
            // 
            // dolphinBox
            // 
            this.dolphinBox.Location = new System.Drawing.Point(70, 50);
            this.dolphinBox.Name = "dolphinBox";
            this.dolphinBox.Size = new System.Drawing.Size(197, 20);
            this.dolphinBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Bin Editor";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Dolphin";
            // 
            // beBrowse
            // 
            this.beBrowse.Location = new System.Drawing.Point(273, 21);
            this.beBrowse.Name = "beBrowse";
            this.beBrowse.Size = new System.Drawing.Size(75, 23);
            this.beBrowse.TabIndex = 12;
            this.beBrowse.Text = "Browse";
            this.beBrowse.UseVisualStyleBackColor = true;
            this.beBrowse.Click += new System.EventHandler(this.beBrowse_Click);
            // 
            // dolphinBrowse
            // 
            this.dolphinBrowse.Location = new System.Drawing.Point(273, 48);
            this.dolphinBrowse.Name = "dolphinBrowse";
            this.dolphinBrowse.Size = new System.Drawing.Size(75, 23);
            this.dolphinBrowse.TabIndex = 17;
            this.dolphinBrowse.Text = "Browse";
            this.dolphinBrowse.UseVisualStyleBackColor = true;
            this.dolphinBrowse.Click += new System.EventHandler(this.dolphinBrowse_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(273, 344);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // defaultButton
            // 
            this.defaultButton.Location = new System.Drawing.Point(15, 344);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Size = new System.Drawing.Size(75, 23);
            this.defaultButton.TabIndex = 19;
            this.defaultButton.Text = "Default";
            this.defaultButton.UseVisualStyleBackColor = true;
            this.defaultButton.Click += new System.EventHandler(this.defaultButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(192, 344);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 20;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 76);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(134, 17);
            this.checkBox1.TabIndex = 21;
            this.checkBox1.Text = "Auto Load Last Project";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(15, 130);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(169, 17);
            this.checkBox2.TabIndex = 22;
            this.checkBox2.Text = "Open Dolphin in Debug mode.";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 379);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.defaultButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.dolphinBrowse);
            this.Controls.Add(this.beBrowse);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dolphinBox);
            this.Controls.Add(this.beBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(376, 418);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(376, 418);
            this.Name = "Settings";
            this.RightToLeftLayout = true;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox beBox;
        private System.Windows.Forms.TextBox dolphinBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button beBrowse;
        private System.Windows.Forms.Button dolphinBrowse;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button defaultButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}