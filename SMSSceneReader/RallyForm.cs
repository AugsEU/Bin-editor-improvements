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
using DataReader;
using SMSReader;

namespace SMSRallyEditor
{
    public partial class RallyForm : Form
    {
        RalFile file;

        public RallyForm(RalFile loadFile)
        {
            InitializeComponent();
            file = loadFile;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetAll(false);
            UpdateItems();
        }

        public void UpdateRails(int rail, int frame)
        {
            listBox1.SelectedIndex = rail;
            listBox2.SelectedIndex = -1;
            listBox2.SelectedIndex = frame;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;
            listBox2.Items.Clear();
            for (int i = 0; i < file.GetRail(listBox1.SelectedIndex).frames.Length; i++)
                listBox2.Items.Add(i.ToString());
            if (listBox2.Items.Count > 0)
                listBox2.SelectedIndex = 0;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            file.UnLoad();
            SetAll(false);
            this.Close();
        }

        void SetAll(bool value)
        {
            remButton.Enabled = value;
            remButton2.Enabled = value;
            addButton.Enabled = value;
            addButton2.Enabled = value;
            listBox1.Enabled = value;
            listBox2.Enabled = value;
            applyButton.Enabled = value;
        }

        private void UpdateItems()
        {
            SetAll(false);

            listBox1.Items.Clear();
            for (int i = 0; i < file.Count; i++)
                listBox1.Items.Add(file.GetRail(i).name);
            
            SetAll(true);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            NameCheck nc = new NameCheck("New Rail");
            if (nc.ShowDialog() != DialogResult.OK)
                return;

            if (file.ContainsRail(nc.name))
            {
                MessageBox.Show("There is already a path with this key name", "Key taken", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            file.AddRail(nc.name);
            UpdateItems();
        }

        private void remButton_Click(object sender, EventArgs e)
        {
            file.RemoveRail((string)listBox1.SelectedItem);
            UpdateItems();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1 || listBox2.Items.Count == 0)
                return;

            KeyFrame rp = file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex];

            xUpDown.Value = rp.x;
            yUpDown.Value = rp.y;
            zUpDown.Value = rp.z;
            u1UpDown.Value = rp.u1;
            u2UpDown.Value = rp.u2;
            numericUpDown1.Value = rp.u3;
            yawUpDown.Value = rp.yaw;
            pitchUpDown.Value = rp.pitch;
            rollUpDown.Value = rp.roll;
            speedUpDown.Value = rp.speed;
            startUpDown.Value = rp.connections[0];
            endUpDown.Value = rp.connections[1];
            u3UpDown.Value = rp.connections[2];
            u4UpDown.Value = rp.connections[3];
            u5UpDown.Value = rp.connections[4];
            u6UpDown.Value = rp.connections[5];
            u7UpDown.Value = rp.connections[6];
            u8UpDown.Value = rp.connections[7];
            u9UpDown.Value = (decimal)rp.periods[0];
            u10UpDown.Value = (decimal)rp.periods[1];
            u11UpDown.Value = (decimal)rp.periods[2];
            u12UpDown.Value = (decimal)rp.periods[3];
            u13UpDown.Value = (decimal)rp.periods[4];
            u14UpDown.Value = (decimal)rp.periods[5];
            u15UpDown.Value = (decimal)rp.periods[6];
            u16UpDown.Value = (decimal)rp.periods[7];
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            KeyFrame rp = file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex];
            rp.x = (short)xUpDown.Value;
            rp.y = (short)yUpDown.Value;
            rp.z = (short)zUpDown.Value;
            rp.u1 = (short)u1UpDown.Value;
            rp.u2 = (short)u2UpDown.Value;
            rp.u3 = (short)numericUpDown1.Value;
            rp.yaw = (short)yawUpDown.Value;
            rp.pitch = (short)pitchUpDown.Value;
            rp.roll = (short)rollUpDown.Value;
            rp.speed = (short)speedUpDown.Value;
            rp.connections[0] = (short)startUpDown.Value;
            rp.connections[1] = (short)endUpDown.Value;
            rp.connections[2] = (short)u3UpDown.Value;
            rp.connections[3] = (short)u4UpDown.Value;
            rp.connections[4] = (short)u5UpDown.Value;
            rp.connections[5] = (short)u6UpDown.Value;
            rp.connections[6] = (short)u7UpDown.Value;
            rp.connections[7] = (short)u8UpDown.Value;
            rp.periods[0] = (float)u9UpDown.Value;
            rp.periods[1] = (float)u10UpDown.Value;
            rp.periods[2] = (float)u11UpDown.Value;
            rp.periods[3] = (float)u12UpDown.Value;
            rp.periods[4] = (float)u13UpDown.Value;
            rp.periods[5] = (float)u14UpDown.Value;
            rp.periods[6] = (float)u15UpDown.Value;
            rp.periods[7] = (float)u16UpDown.Value;
            file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex] = rp;
        }

        private void addButton2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;

            KeyFrame rp = new KeyFrame();
            rp.NullData();

            file.GetAllRails()[listBox1.SelectedIndex].InsertFrame(rp, listBox2.SelectedIndex);

            listBox2.Items.Clear();
            for (int i = 0; i < file.GetAllRails()[listBox1.SelectedIndex].frames.Length; i++)
                listBox2.Items.Add(i.ToString());
        }

        private void remButton2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;
            file.GetAllRails()[listBox1.SelectedIndex].RemoveFrame(listBox2.SelectedIndex);

            listBox2.Items.Clear();
            for (int i = 0; i < file.GetAllRails()[listBox1.SelectedIndex].frames.Length; i++)
                listBox2.Items.Add(i.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;
            if (listBox2.SelectedIndex >= listBox2.Items.Count || listBox2.SelectedIndex < 1)
                return;

            int nid = listBox2.SelectedIndex - 1;
            file.GetAllRails()[listBox1.SelectedIndex].SwapFrames(listBox2.SelectedIndex, listBox2.SelectedIndex - 1);

            listBox2.Items.Clear();
            for (int i = 0; i < file.GetAllRails()[listBox1.SelectedIndex].frames.Length; i++)
                listBox2.Items.Add(i.ToString());

            listBox2.SelectedIndex = nid;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;
            if (listBox2.SelectedIndex >= listBox2.Items.Count - 1 || listBox2.SelectedIndex < 0)
                return;

            int nid = listBox2.SelectedIndex + 1;
            file.GetAllRails()[listBox1.SelectedIndex].SwapFrames(listBox2.SelectedIndex, listBox2.SelectedIndex + 1);

            listBox2.Items.Clear();
            for (int i = 0; i < file.GetAllRails()[listBox1.SelectedIndex].frames.Length; i++)
                listBox2.Items.Add(i.ToString());

            listBox2.SelectedIndex = nid;
        }

        private void RallyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            file.Save();
        }

        private void u1UpDown_ValueChanged(object sender, EventArgs e)
        {
            startUpDown.Enabled = false;
            endUpDown.Enabled = false;
            u3UpDown.Enabled = false;
            u4UpDown.Enabled = false;
            u5UpDown.Enabled = false;
            u6UpDown.Enabled = false;
            u7UpDown.Enabled = false;
            u8UpDown.Enabled = false;

            u9UpDown.Enabled = false;
            u10UpDown.Enabled = false;
            u11UpDown.Enabled = false;
            u12UpDown.Enabled = false;
            u13UpDown.Enabled = false;
            u14UpDown.Enabled = false;
            u15UpDown.Enabled = false;
            u16UpDown.Enabled = false;

            switch ((int)u1UpDown.Value)
            {
                case 8:
                    u8UpDown.Enabled = true;
                    u16UpDown.Enabled = true;
                    goto case 7;
                case 7:
                    u7UpDown.Enabled = true;
                    u15UpDown.Enabled = true;
                    goto case 6;
                case 6:
                    u6UpDown.Enabled = true;
                    u14UpDown.Enabled = true;
                    goto case 5;
                case 5:
                    u5UpDown.Enabled = true;
                    u13UpDown.Enabled = true;
                    goto case 4;
                case 4:
                    u4UpDown.Enabled = true;
                    u12UpDown.Enabled = true;
                    goto case 3;
                case 3:
                    u3UpDown.Enabled = true;
                    u11UpDown.Enabled = true;
                    goto case 2;
                case 2:
                    endUpDown.Enabled = true;
                    u10UpDown.Enabled = true;
                    goto case 1;
                case 1:
                    startUpDown.Enabled = true;
                    u9UpDown.Enabled = true;
                    break;
            }
        }

        private void DupBtn_Click(object sender, EventArgs e)
        {
            int DuplicationIndex = listBox1.SelectedIndex;//Index of rail to duplicate;
            int OutputIndex = listBox1.Items.Count; //Index of duplicated rail.
            if (DuplicationIndex == -1 || listBox1.Items.Count == 0)//Make sure indicies make sense
                return;

            NameCheck nc = new NameCheck("New Rail");
            if (nc.ShowDialog() != DialogResult.OK)//Cancel if no name is put in.
                return;

            if (file.ContainsRail(nc.name))//Check rail name
            {
                MessageBox.Show("There is already a path with this key name", "Key taken", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if(nc.Name == "")
            {
                MessageBox.Show("Invalid name", "Too short", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            file.AddRail(nc.name); //Add rail
            UpdateItems();//Update

            
            for(int i = 0; i < file.GetAllRails()[DuplicationIndex].frames.Length;i++)//For each frame
            {
                KeyFrame rp = file.GetAllRails()[DuplicationIndex].frames[i];//Keyframe = node
                file.GetAllRails()[OutputIndex].InsertFrame(rp, i);//add node to file
            }

            UpdateItems();//Update
            listBox2.SelectedIndex = 0;
            listBox1.SelectedIndex = OutputIndex;
        }

        private void TranslateBtn_Click(object sender, EventArgs e)
        {
            int RailIndex = listBox1.SelectedIndex;//Index of rail we are translating
            int OldFrameIndex = listBox2.SelectedIndex;//Store this so we can set the selected index back at the end
            if (RailIndex == -1 || listBox1.Items.Count == 0)//Make sure indicies make sense
                return;

            VectorInput VectorInput = new VectorInput();
            if (VectorInput.ShowDialog() != DialogResult.OK)//Cancel if no vector is put in.
                return;


            for (int i = 0; i < file.GetAllRails()[RailIndex].frames.Length; i++)
            {
                KeyFrame FrameToEdit = file.GetAllRails()[RailIndex].frames[i];
                FrameToEdit.x += (short)VectorInput.X;
                FrameToEdit.y += (short)VectorInput.Y;
                FrameToEdit.z += (short)VectorInput.Z;//Translate
                file.GetAllRails()[RailIndex].frames[i] = FrameToEdit;
                UpdateRails(RailIndex, i);
            }
            listBox2.SelectedIndex = OldFrameIndex;
        }
    }
}
