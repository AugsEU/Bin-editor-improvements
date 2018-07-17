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
    public partial class KeyFramePrompt : Form
    {
        public Vector3 AtPosition;
        public Vector3 FromPosition;
        public int time;
        public KeyFramePrompt()
        {
            InitializeComponent();
            AtPosition = Vector3.Zero;
            FromPosition = Vector3.Zero;
            AtXUpDn.Value = AtYUpDn.Value = AtZUpDn.Value = FromXUpDn.Value = FromYUpDn.Value = FromZUpDn.Value = 0;
            time = 0;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            AtPosition = new Vector3((float)AtXUpDn.Value, (float)AtYUpDn.Value, (float)AtZUpDn.Value);
            FromPosition = new Vector3((float)FromXUpDn.Value, (float)FromYUpDn.Value, (float)FromZUpDn.Value);
            time = (int)TimeUpDn.Value;
            Close();
        }
    }
}
