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
    public partial class SizeSelect : Form
    {
        public int SelectedSize;

        public SizeSelect(int defSize)
        {
            InitializeComponent();

            SelectedSize = defSize;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedSize = (int)numericUpDown1.Value;
            this.Close();
        }

        private void SizeSelect_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = SelectedSize;
        }
    }
}
