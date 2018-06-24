using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSSceneReader
{
    public partial class ProgressForm : Form
    {
        public ProgressForm(string title)
        {
            InitializeComponent();

            this.Text = title;
        }

        public void UpdateProgress(int progress)
        {
            progressBar1.Value = progress;
            if (progressBar1.Value == progressBar1.Maximum)
                this.Close();
        }

        private void ProgressForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (progressBar1.Value != progressBar1.Maximum)
                e.Cancel = true;
        }

        public void ForceClosed()
        {
            UpdateProgress(progressBar1.Maximum);
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 0;
        }
    }
}
