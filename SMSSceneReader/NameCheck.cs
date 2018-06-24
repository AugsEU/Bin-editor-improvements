using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSRallyEditor
{
    /* Pretty boring form */
    public partial class NameCheck : Form
    {
        public string name;

        public NameCheck(string pathname)
        {
            InitializeComponent();
            name = pathname;
        }

        private void NameCheck_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.Text = name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            Close();
        }
    }
}
