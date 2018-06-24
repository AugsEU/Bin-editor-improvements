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
    public partial class Find : Form
    {
        public MainForm parent;
        public TreeView treeview1;
        public int index = 0;

        public Find()
        {
            InitializeComponent();
        }

        /* Search button */
        private void button1_Click(object sender, EventArgs e)
        {
            if (treeview1 == null)
                return;
            if (treeview1.Nodes == null)
                return;

            int count = 0;
            for (int i = 0; i < treeview1.Nodes.Count; i++ )
            {
                GameObject gobject = (GameObject)treeview1.Nodes[i].Tag;

                bool comp = false;
                if (radioButton1.Checked)
                    comp = gobject.Name.ToLower().Contains(textBox1.Text.ToLower());
                else if (radioButton2.Checked)
                    comp = gobject.Description.ToLower().Contains(textBox1.Text.ToLower());
                else if (radioButton3.Checked)
                {
                    int pos = 0;
                    for (int j = 0; j < gobject.Values.Length; j++)
                    {
                        if (textBox1.Text[pos] == (char)gobject.Values[j])
                            pos++;
                        else
                            pos = 0;
                        if (pos == textBox1.Text.Length - 1)
                        {
                            comp = true;
                            break;
                        }
                    }
                }
                if (comp)
                {
                    if (count >= index)
                    {
                        index = count + 1;
                        treeview1.SelectedNode = treeview1.Nodes[i];
                        parent.Focus();
                        treeview1.Focus();
                        return;
                    }
                }
                else
                {
                    count++;
                    TreeNode match = SearchNodes(treeview1.Nodes[i], textBox1.Text, ref count);
                    if (match != null)
                    {
                        treeview1.SelectedNode = match;
                        parent.Focus();
                        treeview1.Focus();
                        return;
                    }
                }
            }
            index = 0;
            MessageBox.Show("Reached end of file.");
        }

        /* Searches through nodes */
        private TreeNode SearchNodes(TreeNode node, string name, ref int count)
        {
            for (int i = 0; i < node.Nodes.Count; i++ )
            {
                GameObject gobject = (GameObject)node.Nodes[i].Tag;

                bool comp = false;
                if (radioButton1.Checked)
                    comp = gobject.Name.ToLower().Contains(textBox1.Text.ToLower());
                else if (radioButton2.Checked)
                    comp = gobject.Description.ToLower().Contains(textBox1.Text.ToLower());
                else if (radioButton3.Checked)
                {
                    byte[] cmpbyte = Encoding.GetEncoding("shift-jis").GetBytes(textBox1.Text);
                    int pos = 0;
                    for (int j = 0; j < gobject.Values.Length; j++)
                    {
                        if (cmpbyte[pos] == gobject.Values[j])
                            pos++;
                        else if (pos > 0)
                        {
                            pos = 0;
                            j--;
                        }
                        if (pos == cmpbyte.Length - 1)
                        {
                            comp = true;
                            break;
                        }
                    }
                }

                if (comp)
                {
                    if (count >= index)
                    {
                        index = count + 1;
                        return node.Nodes[i];
                    }
                }
                else
                {
                    count++;
                    TreeNode match = SearchNodes(node.Nodes[i], name, ref count);
                    if (match != null)
                        return match;
                }
                count++;
            }
            return null;
        }

        /* Close -- Find is always open */
        private void Find_Closing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
