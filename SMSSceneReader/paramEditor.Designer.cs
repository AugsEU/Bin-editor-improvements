namespace SMSSceneReader
{
    partial class paramEditor
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
            this.paramType = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.paramName = new System.Windows.Forms.TextBox();
            this.paramList = new System.Windows.Forms.ListBox();
            this.paramAdd = new System.Windows.Forms.Button();
            this.paramRemove = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.commentBox = new System.Windows.Forms.TextBox();
            this.paramTitle = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // paramType
            // 
            this.paramType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paramType.FormattingEnabled = true;
            this.paramType.Location = new System.Drawing.Point(8, 55);
            this.paramType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.paramType.Name = "paramType";
            this.paramType.Size = new System.Drawing.Size(228, 24);
            this.paramType.TabIndex = 4;
            this.paramType.SelectedIndexChanged += new System.EventHandler(this.paramType_SelectedIndexChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(263, 184);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(371, 184);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 28);
            this.saveButton.TabIndex = 8;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // paramName
            // 
            this.paramName.Location = new System.Drawing.Point(8, 23);
            this.paramName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.paramName.Name = "paramName";
            this.paramName.Size = new System.Drawing.Size(228, 22);
            this.paramName.TabIndex = 3;
            this.paramName.TextChanged += new System.EventHandler(this.paramName_TextChanged);
            // 
            // paramList
            // 
            this.paramList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paramList.FormattingEnabled = true;
            this.paramList.ItemHeight = 16;
            this.paramList.Location = new System.Drawing.Point(16, 15);
            this.paramList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.paramList.Name = "paramList";
            this.paramList.Size = new System.Drawing.Size(202, 164);
            this.paramList.TabIndex = 0;
            this.paramList.SelectedIndexChanged += new System.EventHandler(this.paramList_SelectedIndexChanged);
            // 
            // paramAdd
            // 
            this.paramAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.paramAdd.Location = new System.Drawing.Point(16, 186);
            this.paramAdd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.paramAdd.Name = "paramAdd";
            this.paramAdd.Size = new System.Drawing.Size(100, 28);
            this.paramAdd.TabIndex = 1;
            this.paramAdd.Text = "Add";
            this.paramAdd.UseVisualStyleBackColor = true;
            this.paramAdd.Click += new System.EventHandler(this.paramAdd_Click);
            // 
            // paramRemove
            // 
            this.paramRemove.Location = new System.Drawing.Point(8, 89);
            this.paramRemove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.paramRemove.Name = "paramRemove";
            this.paramRemove.Size = new System.Drawing.Size(100, 28);
            this.paramRemove.TabIndex = 5;
            this.paramRemove.Text = "Remove";
            this.paramRemove.UseVisualStyleBackColor = true;
            this.paramRemove.Click += new System.EventHandler(this.paramRemove_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.commentBox);
            this.groupBox1.Controls.Add(this.paramName);
            this.groupBox1.Controls.Add(this.paramRemove);
            this.groupBox1.Controls.Add(this.paramType);
            this.groupBox1.Location = new System.Drawing.Point(226, 43);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(245, 133);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // commentBox
            // 
            this.commentBox.Enabled = false;
            this.commentBox.Location = new System.Drawing.Point(112, 92);
            this.commentBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.commentBox.Name = "commentBox";
            this.commentBox.Size = new System.Drawing.Size(124, 22);
            this.commentBox.TabIndex = 6;
            this.commentBox.TextChanged += new System.EventHandler(this.commentBox_TextChanged);
            // 
            // paramTitle
            // 
            this.paramTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paramTitle.Location = new System.Drawing.Point(227, 15);
            this.paramTitle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.paramTitle.Name = "paramTitle";
            this.paramTitle.Size = new System.Drawing.Size(244, 22);
            this.paramTitle.TabIndex = 2;
            this.paramTitle.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // paramEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 229);
            this.Controls.Add(this.paramTitle);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.paramAdd);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.paramList);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(500, 266);
            this.Name = "paramEditor";
            this.ShowIcon = false;
            this.Text = "Object Parameter Editor";
            this.Load += new System.EventHandler(this.paramEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox paramType;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox paramName;
        private System.Windows.Forms.ListBox paramList;
        private System.Windows.Forms.Button paramAdd;
        private System.Windows.Forms.Button paramRemove;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox paramTitle;
        private System.Windows.Forms.TextBox commentBox;
    }
}