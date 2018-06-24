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

namespace SMSSceneReader
{
    public partial class ActorSelect : Form
    {
        string paramFolder;

        public string SelectedActor;

        public string ButtonText
        {
            get { return button1.Text; }
            set { button1.Text = value; }
        }

        public ActorSelect(string paramdir)
        {
            InitializeComponent();

            paramFolder = paramdir;
            SelectedActor = "";
        }

        private void ActorSelect_Load(object sender, EventArgs e)
        {
            if (SelectedActor == null)
                SelectedActor = "";
            comboBox1.Text = SelectedActor;

            List<string> files = Directory.EnumerateFiles(paramFolder).ToList();
            files.Sort();
            foreach (string str in files)
            {
                FileInfo fi = new FileInfo(str);
                if (fi.Extension != ".txt")
                    continue;
                comboBox1.Items.Add(fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length));
            }
            if (SelectedActor != null && SelectedActor != "")
                comboBox1.Text = SelectedActor;
            else
                comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedActor = comboBox1.Text;
            this.Close();
        }
    }
}
