using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMSReader;

namespace SMSSceneReader
{
    public partial class BMGEditor : Form
    {
        private const int STRSHOW = 8;

        BmgFile bmgFile;

        public BMGEditor(BmgFile bmg)
        {
            bmgFile = bmg;

            InitializeComponent();
        }

        /* Initialize lists with bmg file */
        private void BMGEditor_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < bmgFile.Count; i++)
            {
                int show = STRSHOW;
                string str = bmgFile.GetString(i);
                if (str.Length < show)
                    show = str.Length;
                listBox1.Items.Add(i.ToString() + " - " + bmgFile.GetString(i).Substring(0, show) + "... (" + bmgFile.GetSoundEffectID(i).ToString() + ")");
            }
        }

        /* Show selected message */
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                textBox1.Text = bmgFile.GetStringReadable(listBox1.SelectedIndex);
                numericUpDown1.Value = bmgFile.GetSoundEffectID(listBox1.SelectedIndex);
                numericUpDown1.Enabled = true;
                textBox1.Enabled = true;
            }
            else
            {
                numericUpDown1.Enabled = false;
                textBox1.Enabled = false;
            }
        }

        /* Save message */
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bmgFile.SetStringReadable(listBox1.SelectedIndex, textBox1.Text);
        }

        /* Change message flags/sfx/etc */
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            bmgFile.SetSoundEffectID(listBox1.SelectedIndex, (byte)numericUpDown1.Value);
        }

        /* Add new message to the file */
        private void addButton_Click(object sender, EventArgs e)
        {
            bmgFile.Add("");
            BMGEditor_Load(sender, e);
        }

        /* Inserts new message into the selected index */
        private void insertButton_Click(object sender, EventArgs e)
        {
            bmgFile.Insert(listBox1.SelectedIndex, "");
            BMGEditor_Load(sender, e);
        }

        /* Removes selected message */
        private void removeButton_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            bmgFile.RemoveAt(listBox1.SelectedIndex);
            BMGEditor_Load(sender, e);
            listBox1.SelectedIndex = index;
        }
    }
}
