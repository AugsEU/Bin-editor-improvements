using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace SMSSceneReader
{
    public partial class ArrayPrompt : Form
    {
        public Vector3 Displacement;
        public int count;
        public ArrayPrompt()
        {
            InitializeComponent();
            Displacement = Vector3.Zero;
            AtXUpDn.Value = AtYUpDn.Value = AtZUpDn.Value = 0;
            count = 0;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            Displacement = new Vector3((float)AtXUpDn.Value, (float)AtYUpDn.Value, (float)AtZUpDn.Value);
            count = (int)CountUpDn.Value;
            Close();
        }
    }
}
