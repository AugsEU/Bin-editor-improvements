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
    public partial class Subdivide : Form
    {
        public int Count;
        public string SelectedScheme;
        public Subdivide()
        {
            InitializeComponent();
            Count = 0;
            SelectedScheme = "Chaikin";
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            Count = (int)CountUpDown.Value;
            RadioButton SelectedRadio = GetCheckedRadio(RadioGroup);
            if (SelectedRadio != null)
                SelectedScheme = SelectedRadio.Text;
            Close();
        }
        RadioButton GetCheckedRadio(Control container)
        {
            foreach (var control in container.Controls)
            {
                RadioButton radio = control as RadioButton;

                if (radio != null && radio.Checked)
                {
                    return radio;
                }
            }

            return null;
        }
    }
}
