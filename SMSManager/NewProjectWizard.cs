using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DataReader;

namespace SMSManager
{
    public partial class NewProjectWizard : Form
    {
        public string ProjectName = "New Project";
        public string ProjectPath = null;

        public NewProjectWizard()
        {
            InitializeComponent();
        }

        private void NewProjectWizard_Load(object sender, EventArgs e)
        {
            string project;
            int c = 0;
            do
            {
                project = ProjectName + " " + (c++);
            } while (File.Exists(Directory.GetCurrentDirectory() + "\\" + project));
            pathBox.Text = project;
        }

        private void OnChange(int currentTab)
        {
            fileSystemWatcher1.EnableRaisingEvents = false;
            switch (currentTab){
                case 1:
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName);
                    dirBox.Text = Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName;
                    fileSystemWatcher1.Path = dirBox.Text;
                    fileSystemWatcher1.EnableRaisingEvents = true;

                    if (Directory.Exists("./root") && !File.Exists("./root/readme.txt"))
                        if (MessageBox.Show("A default root folder was detected. Would you like to copy this folder? (This may take a couple minutes)", "Root Folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            Form1.CopyDir("./root", Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName + "\\root");
                    if (Directory.Exists("./root") && File.Exists("./root/readme.txt"))
                        MessageBox.Show("Ignoring user root folder because \"readme.txt\" exists.");

                    CheckRoot();
                    break;
            }
        }
        private void OnBack(int currentTab)
        {
            switch (currentTab)
            {
                case 0:
                    try { Directory.Delete(Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName); }
                    catch { break; }
                    break;
            }
        }

        private void OnFinish()
        {
            ProjectPath = Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (wizardTab1.SelectedIndex == wizardTab1.TabCount - 1)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.OnFinish();
                this.Close();
            }
            if (wizardTab1.SelectedIndex++ == 1)
                button2.Text = "Back";
            else if (wizardTab1.SelectedIndex == wizardTab1.TabCount - 1)
                button1.Text = "Finish";
            this.OnChange(wizardTab1.SelectedIndex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (wizardTab1.SelectedIndex == 0)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
            if (wizardTab1.SelectedIndex-- == 0)
                button2.Text = "Cancel";
            else if (wizardTab1.SelectedIndex != wizardTab1.TabCount - 1)
                button1.Text = "Next";
            this.OnChange(wizardTab1.SelectedIndex);
            this.OnBack(wizardTab1.SelectedIndex);
        }

        private void CheckProject(string name)
        {
            button1.Enabled = true;
            projNameExistsLabel.Visible = false;
            projNameInvalidLabel.Visible = false;
            if (name == "" || name.Replace(" ", "").Replace(".", "") == "" || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                projNameInvalidLabel.Visible = true;
                button1.Enabled = false;
                return;
            }
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Projects\\" + name))
            {
                projNameExistsLabel.Visible = true;
                button1.Enabled = false;
            }
        }

        private void pathBox_TextChanged(object sender, EventArgs e)
        {
            ProjectName = pathBox.Text;
            CheckProject(ProjectName);
        }

        private void CheckRoot()
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName + "\\root") && Directory.Exists(Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName + "\\root\\AudioRes") && Directory.Exists(Directory.GetCurrentDirectory() + "\\Projects\\" + ProjectName + "\\root\\data"))
            {
                rootDetLabel1.Visible = true;
                rootDetLabel2.Visible = false;
                button1.Enabled = true;
                button3.Visible = true;
            }
            else
            {
                rootDetLabel1.Visible = false;
                rootDetLabel2.Visible = true;
                button1.Enabled = false;
                button3.Visible = false;
            }
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            CheckRoot();
        }

        private void dirBox_Click(object sender, EventArgs e)
        {
            dirBox.SelectAll();
            Clipboard.SetText(dirBox.Text, TextDataFormat.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!GCN.ExtractISO(openFileDialog1.FileName, "./root"))
                    MessageBox.Show("Not a valid image.");
            }
        }
    }
}
