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
    public partial class WarpEdit : Form
    {
        public string tableName;
        public string[] tableList;

        public WarpEdit()
        {
            InitializeComponent();
        }
        public WarpEdit(string name, string[] list)
        {
            tableName = name;
            tableList = list;

            InitializeComponent();
        }

        private void WarpEdit_Load(object sender, EventArgs e)
        {
            descBox.Text = tableName;
            listBox.Text = "";
            foreach (string str in tableList)
                listBox.Text += str + Environment.NewLine;

            if (descBox.Text == "" || listBox.Text == "")
                button1.Enabled = false;
            else
                button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tableName = descBox.Text;
            tableList = listBox.Text.Split(new string[1]{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void descBox_TextChanged(object sender, EventArgs e)
        {
            if (descBox.Text == "" || listBox.Text == "")
                button1.Enabled = false;
            else
                button1.Enabled = true;
        }

        private void listBox_TextChanged(object sender, EventArgs e)
        {
            if (descBox.Text == "" || listBox.Text == "")
                button1.Enabled = false;
            else
                button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
