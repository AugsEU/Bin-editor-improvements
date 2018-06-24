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
    public partial class StringInput : Form
    {
        public string Input;

        public StringInput(string label, string def = "")
        {
            InitializeComponent();

            label1.Text = label;
            textBox1.Text = def;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Input = textBox1.Text;
            this.Close();
        }
    }
}
