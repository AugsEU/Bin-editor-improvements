namespace SMSManager
{
    partial class NewProjectWizard
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.wizardTab1 = new SMSManager.WizardTab();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.projNameInvalidLabel = new System.Windows.Forms.Label();
            this.projNameExistsLabel = new System.Windows.Forms.Label();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rootDetLabel2 = new System.Windows.Forms.Label();
            this.rootDetLabel1 = new System.Windows.Forms.Label();
            this.dirBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.wizardTab1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(746, 578);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(624, 578);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 35);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.IncludeSubdirectories = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            this.fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Changed);
            // 
            // wizardTab1
            // 
            this.wizardTab1.Controls.Add(this.tabPage1);
            this.wizardTab1.Controls.Add(this.tabPage2);
            this.wizardTab1.Controls.Add(this.tabPage3);
            this.wizardTab1.Location = new System.Drawing.Point(20, 20);
            this.wizardTab1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.wizardTab1.Name = "wizardTab1";
            this.wizardTab1.SelectedIndex = 0;
            this.wizardTab1.Size = new System.Drawing.Size(838, 549);
            this.wizardTab1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.projNameInvalidLabel);
            this.tabPage1.Controls.Add(this.projNameExistsLabel);
            this.tabPage1.Controls.Add(this.pathBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(830, 516);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 263);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(534, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "When a proper name has been chosen you may continue to the next page.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 112);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(743, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Please enter a name for the new project to be created. This name is only used for" +
    " organizing project files.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 46);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(457, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "This New Project Wizard will help you create a new SMS Project.";
            // 
            // projNameInvalidLabel
            // 
            this.projNameInvalidLabel.AutoSize = true;
            this.projNameInvalidLabel.ForeColor = System.Drawing.Color.Red;
            this.projNameInvalidLabel.Location = new System.Drawing.Point(202, 212);
            this.projNameInvalidLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.projNameInvalidLabel.Name = "projNameInvalidLabel";
            this.projNameInvalidLabel.Size = new System.Drawing.Size(164, 20);
            this.projNameInvalidLabel.TabIndex = 2;
            this.projNameInvalidLabel.Text = "Project name is invalid";
            this.projNameInvalidLabel.Visible = false;
            // 
            // projNameExistsLabel
            // 
            this.projNameExistsLabel.AutoSize = true;
            this.projNameExistsLabel.ForeColor = System.Drawing.Color.Red;
            this.projNameExistsLabel.Location = new System.Drawing.Point(202, 192);
            this.projNameExistsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.projNameExistsLabel.Name = "projNameExistsLabel";
            this.projNameExistsLabel.Size = new System.Drawing.Size(283, 20);
            this.projNameExistsLabel.TabIndex = 1;
            this.projNameExistsLabel.Text = "A project with that name already exists.";
            this.projNameExistsLabel.Visible = false;
            // 
            // pathBox
            // 
            this.pathBox.Location = new System.Drawing.Point(204, 157);
            this.pathBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pathBox.Name = "pathBox";
            this.pathBox.Size = new System.Drawing.Size(404, 26);
            this.pathBox.TabIndex = 0;
            this.pathBox.TextChanged += new System.EventHandler(this.pathBox_TextChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.rootDetLabel2);
            this.tabPage2.Controls.Add(this.rootDetLabel1);
            this.tabPage2.Controls.Add(this.dirBox);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Size = new System.Drawing.Size(830, 516);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(153, 162);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 20);
            this.label8.TabIndex = 5;
            this.label8.Text = "Click to copy to clipboard.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 106);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(569, 20);
            this.label7.TabIndex = 4;
            this.label7.Text = "If you export it correctly, your \"root\" folder should be located in the project f" +
    "older.";
            // 
            // rootDetLabel2
            // 
            this.rootDetLabel2.AutoSize = true;
            this.rootDetLabel2.ForeColor = System.Drawing.Color.Red;
            this.rootDetLabel2.Location = new System.Drawing.Point(148, 246);
            this.rootDetLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rootDetLabel2.Name = "rootDetLabel2";
            this.rootDetLabel2.Size = new System.Drawing.Size(179, 20);
            this.rootDetLabel2.TabIndex = 3;
            this.rootDetLabel2.Text = "root folder not detected!";
            this.rootDetLabel2.Visible = false;
            // 
            // rootDetLabel1
            // 
            this.rootDetLabel1.AutoSize = true;
            this.rootDetLabel1.Location = new System.Drawing.Point(148, 226);
            this.rootDetLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rootDetLabel1.Name = "rootDetLabel1";
            this.rootDetLabel1.Size = new System.Drawing.Size(152, 20);
            this.rootDetLabel1.TabIndex = 2;
            this.rootDetLabel1.Text = "root folder detected!";
            this.rootDetLabel1.Visible = false;
            // 
            // dirBox
            // 
            this.dirBox.Location = new System.Drawing.Point(153, 191);
            this.dirBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dirBox.Name = "dirBox";
            this.dirBox.ReadOnly = true;
            this.dirBox.Size = new System.Drawing.Size(496, 26);
            this.dirBox.TabIndex = 1;
            this.dirBox.Click += new System.EventHandler(this.dirBox_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 86);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(727, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "A project folder has been created. Please export your gcn or iso into the followi" +
    "ng directory to continue.";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(830, 516);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 222);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(744, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Your project is now ready to be created. Press finish and your editing files will" +
    " be generated automatically.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 222);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 20);
            this.label2.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(152, 269);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(227, 31);
            this.button3.TabIndex = 6;
            this.button3.Text = "Extract From ISO";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "Super Mario Sunshine.iso";
            this.openFileDialog1.Filter = "Image Files|*.iso; *.gcm|All Files|*";
            // 
            // NewProjectWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 606);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.wizardTab1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(889, 662);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(889, 662);
            this.Name = "NewProjectWizard";
            this.Text = "New Project Wizard";
            this.Load += new System.EventHandler(this.NewProjectWizard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.wizardTab1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private WizardTab wizardTab1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox pathBox;
        private System.Windows.Forms.Label projNameExistsLabel;
        private System.Windows.Forms.Label projNameInvalidLabel;
        private System.Windows.Forms.TextBox dirBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label rootDetLabel2;
        private System.Windows.Forms.Label rootDetLabel1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label2;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}