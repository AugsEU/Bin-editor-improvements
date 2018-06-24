using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSManager
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void LoadSettings()
        {
            beBox.Text = Properties.Settings.Default.BinEditorPath;
            dolphinBox.Text = Properties.Settings.Default.DolphinPath;

            checkBox1.Checked = Properties.Settings.Default.AutoLoad;
            checkBox2.Checked = Properties.Settings.Default.DolphinDebug;
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.BinEditorPath = beBox.Text;
            Properties.Settings.Default.DolphinPath = dolphinBox.Text;

            Properties.Settings.Default.AutoLoad = checkBox1.Checked;
            Properties.Settings.Default.DolphinDebug = checkBox2.Checked;

            Properties.Settings.Default.Save();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void defaultButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            SaveSettings();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void beBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = beBox.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                beBox.Text = Properties.Settings.Default.BinEditorPath = openFileDialog1.FileName;
        }

        private void dolphinBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = dolphinBox.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                dolphinBox.Text = Properties.Settings.Default.DolphinPath = openFileDialog1.FileName;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
