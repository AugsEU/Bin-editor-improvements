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
    public partial class VectorInput : Form
    {

        public float X;
        public float Y;
        public float Z;

        public VectorInput()
        {
            InitializeComponent();
            X = Y = Z  = 0f;
            XUpDown.Value = YUpDown.Value = ZUpDown.Value = 0;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            X = (float)XUpDown.Value;
            Y = (float)YUpDown.Value;
            Z = (float)ZUpDown.Value;
            Close();
        }
    }
}
