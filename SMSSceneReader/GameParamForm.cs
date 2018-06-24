using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataReader;
using SMSReader;

namespace SMSSceneReader
{
    public partial class GameParamForm : Form
    {
        List<PrmFile> Params = new List<PrmFile>();     //All parameter files loaded

        bool noEdit = false;    //If true, textboxes do not push changes

        public GameParamForm(List<PrmFile> prms)
        {
            Params = prms;
            InitializeComponent();
        }

        private void GameParamForm_Load(object sender, EventArgs e)
        {
            //Set max values for updown boxes
            byteUpDown.Maximum = byte.MaxValue;
            byteUpDown.Minimum = byte.MinValue;
            wordUpDown.Maximum = short.MaxValue;
            wordUpDown.Minimum = short.MinValue;
            intUpDown.Maximum = int.MaxValue;
            intUpDown.Minimum = int.MinValue;
            singleUpDown.Maximum = decimal.MaxValue;
            singleUpDown.Minimum = decimal.MinValue;
            doubleUpDown.Maximum = decimal.MaxValue;
            doubleUpDown.Minimum = decimal.MinValue;

            //Load items
            foreach (PrmFile pf in Params)
                listBox1.Items.Add(pf.Name);

            if (listBox1.Items.Count != 0)
                listBox1.SelectedIndex = 0;
            else
                listBox1.SelectedIndex = -1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrmFile selected = Params[listBox1.SelectedIndex];
            listBox2.Items.Clear();
            string[] keys = selected.GetAllKeys();
            foreach (string key in keys)
                listBox2.Items.Add(key);

            if (listBox2.Items.Count != 0)
                listBox2.SelectedIndex = 0;
            else
                listBox2.SelectedIndex = -1;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrmFile selected = Params[listBox1.SelectedIndex];
            if (listBox2.SelectedIndex >= 0)
                UpdateTextBoxes();
            else
                DisableTextBoxes();
        }

        /* Updates textbox values and enables the ones needed */
        private void UpdateTextBoxes()
        {
            DisableTextBoxes();

            noEdit = true;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            sizeUpDown.Value = seldat.Size;
            byteUpDown.Value = seldat.ByteValue;
            wordUpDown.Value = seldat.ShortValue;
            intUpDown.Value = seldat.IntValue;
            singleUpDown.Value = (decimal)seldat.SingleValue;
            doubleUpDown.Value = (decimal)seldat.DoubleValue;
            vectorBox.Text = seldat.VectorValue.ToString();
            stringBox.Text = seldat.StringValue;

            int size = seldat.Size;
            if (size == 1)
                byteUpDown.Enabled = true;
            else if (size == 2)
                wordUpDown.Enabled = true;
            else if (size == 4)
            {
                singleUpDown.Enabled = true;
                intUpDown.Enabled = true;
            }
            else if (size == 8)
                doubleUpDown.Enabled = true;
            else if (size == 12)
                vectorBox.Enabled = true;

            stringBox.Enabled = true;
            sizeUpDown.Enabled = true;

            noEdit = false;
        }
        private void DisableTextBoxes()
        {
            sizeUpDown.Enabled = false;
            byteUpDown.Enabled = false;
            wordUpDown.Enabled = false;
            intUpDown.Enabled = false;
            singleUpDown.Enabled = false;
            doubleUpDown.Enabled = false;
            vectorBox.Enabled = false;
            stringBox.Enabled = false;
        }


        /* These functions update the values of parameter */
        private void sizeUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            if (MessageBox.Show("Changing this may cause loss of data, are you sure you want to continue?", "Change Size", MessageBoxButtons.YesNo) == DialogResult.Yes)
                selected.SetSize((string)listBox2.SelectedItem, (uint)sizeUpDown.Value);

            UpdateTextBoxes();
        }
        private void byteUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            seldat.ByteValue = (byte)byteUpDown.Value;
            selected.SetData((string)listBox2.SelectedItem, seldat);

            UpdateTextBoxes();
        }
        private void wordUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            seldat.ShortValue = (short)wordUpDown.Value;
            selected.SetData((string)listBox2.SelectedItem, seldat);

            UpdateTextBoxes();
        }
        private void intUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            seldat.IntValue = (int)intUpDown.Value;
            selected.SetData((string)listBox2.SelectedItem, seldat);

            UpdateTextBoxes();
        }
        private void singleUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            seldat.SingleValue = (float)singleUpDown.Value;
            selected.SetData((string)listBox2.SelectedItem, seldat);

            UpdateTextBoxes();
        }
        private void doubleUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            seldat.DoubleValue = (double)doubleUpDown.Value;
            selected.SetData((string)listBox2.SelectedItem, seldat);

            UpdateTextBoxes();
        }
        private void vectorBox_TextChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            

            Vector outvec;
            if (!Vector.TryParse(vectorBox.Text, out outvec))
                return;
            seldat.VectorValue = outvec;

            selected.SetData((string)listBox2.SelectedItem, seldat);

            UpdateTextBoxes();
        }
        private void stringBox_TextChanged(object sender, EventArgs e)
        {
            if (noEdit)
                return;
            PrmFile selected = Params[listBox1.SelectedIndex];
            PrmData seldat = selected.GetData((string)listBox2.SelectedItem);
            seldat.StringValue = stringBox.Text;
            selected.SetData((string)listBox2.SelectedItem, seldat);

            UpdateTextBoxes();
        }
    }
}
