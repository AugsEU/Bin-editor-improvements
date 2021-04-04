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
            //applyButton.Enabled = value;
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
            if (listBox2.SelectedIndex == -1 || listBox2.Items.Count == 0 || listBox1.SelectedIndex == -1)
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
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox2.SelectedIndex == -1)
                return;
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
            for(int i = 0;i < rp.u1;i++)//Calculate distance values
            {
                try
                {
                    Vector rpPosition = new Vector(rp.x, rp.y, rp.z);
                    KeyFrame ConnectedFrame = file.GetAllRails()[listBox1.SelectedIndex].frames[rp.connections[i]];
                    Vector ConnectionDisplacement = new Vector(rp.x - ConnectedFrame.x, rp.y - ConnectedFrame.y, rp.z - ConnectedFrame.z);
                    rp.periods[i] = (float)Math.Sqrt(ConnectionDisplacement.Length);
                }
                catch
                {
                    rp.periods[i] = 0;
                }
            }
            file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex] = rp;
        }

        private void UpdatePeriods() {
            if (listBox1.SelectedIndex == -1 || listBox2.SelectedIndex == -1)
                return;

            KeyFrame rp = file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex];
            Vector rpPosition = new Vector(rp.x, rp.y, rp.z);

            for(int i = 0;i < rp.u1;i++)//Calculate distance values
            {
                try
                {
                    
                    KeyFrame ConnectedFrame = file.GetAllRails()[listBox1.SelectedIndex].frames[rp.connections[i]];
                    Vector ConnectionDisplacement = new Vector(rp.x - ConnectedFrame.x, rp.y - ConnectedFrame.y, rp.z - ConnectedFrame.z);
                    rp.periods[i] = (float)Math.Sqrt(ConnectionDisplacement.Length);
                }
                catch
                {
                    rp.periods[i] = 0;
                }
            }
            file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex] = rp;
        }

        private void addButton2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;

            KeyFrame rp = new KeyFrame();
            //rp.u1 = 2;
            rp.NullData();

            file.GetAllRails()[listBox1.SelectedIndex].InsertFrame(rp, listBox2.SelectedIndex);
            int index = listBox2.SelectedIndex;
            RefreshListBox2();
            listBox2.SelectedIndex = index;
        }

        private void RefreshListBox2()
        {
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

            switch ((int)u1UpDown.Value)
            {
                case 8:
                    u8UpDown.Enabled = true;
                    goto case 7;
                case 7:
                    u7UpDown.Enabled = true;
                    goto case 6;
                case 6:
                    u6UpDown.Enabled = true;
                    goto case 5;
                case 5:
                    u5UpDown.Enabled = true;
                    goto case 4;
                case 4:
                    u4UpDown.Enabled = true;
                    goto case 3;
                case 3:
                    u3UpDown.Enabled = true;
                    goto case 2;
                case 2:
                    endUpDown.Enabled = true;
                    goto case 1;
                case 1:
                    startUpDown.Enabled = true;
                    break;
            }
            CurrentKeyframeSetValue("u1", (short)u1UpDown.Value);
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
            if (listBox2.Items.Count > 0) {
                listBox2.SelectedIndex = 0;
            }
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
            }
            UpdateRails(RailIndex, 0);
            listBox2.SelectedIndex = OldFrameIndex;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int RailIndex = listBox1.SelectedIndex;//Index of rail we are translating
            int OldFrameIndex = listBox2.SelectedIndex;//Store this so we can set the selected index back at the end
            if (RailIndex == -1 || listBox1.Items.Count == 0)//Make sure indicies make sense
                return;

            VectorInput VectorInput = new VectorInput();
            if (VectorInput.ShowDialog() != DialogResult.OK)//Cancel if no vector is put in.
                return;

            Vector Centre = Vector.Zero;
            int NumOfFrames = file.GetAllRails()[RailIndex].frames.Length;
            for (int i = 0; i < NumOfFrames; i++)
            {
                Vector KFPos = new Vector(file.GetAllRails()[RailIndex].frames[i].x, file.GetAllRails()[RailIndex].frames[i].y, file.GetAllRails()[RailIndex].frames[i].z);
                Centre = new Vector(KFPos.X + Centre.X, KFPos.Y + Centre.Y, KFPos.Z + Centre.Z);
            }
            Centre = new Vector(Centre.X / NumOfFrames, Centre.Y / NumOfFrames, Centre.Z / NumOfFrames);//Average position of Key frames

            for (int i = 0; i < NumOfFrames; i++)
            {
                KeyFrame FrameToEdit = file.GetAllRails()[RailIndex].frames[i];
                Vector KFPosToCentre = new Vector(file.GetAllRails()[RailIndex].frames[i].x - Centre.X, file.GetAllRails()[RailIndex].frames[i].y - Centre.Y, file.GetAllRails()[RailIndex].frames[i].z - Centre.Z);
                FrameToEdit.x = (short)(Centre.X + VectorInput.X* KFPosToCentre.X);
                FrameToEdit.y = (short)(Centre.Y + VectorInput.Y * KFPosToCentre.Y);
                FrameToEdit.z = (short)(Centre.Z + VectorInput.Z * KFPosToCentre.Z);//Scale
                file.GetAllRails()[RailIndex].frames[i] = FrameToEdit;
                
            }
            UpdateRails(RailIndex, 0);
            listBox2.SelectedIndex = OldFrameIndex;
        }

        private void SubSurf_Click(object sender, EventArgs e)
        {
            int RailIndex = listBox1.SelectedIndex;//Index of rail we are translating
            if (RailIndex == -1 || listBox1.Items.Count == 0)//Make sure indicies make sense
                return;

            Subdivide SubiInput = new Subdivide();
            if (SubiInput.ShowDialog() != DialogResult.OK)//Cancel if no SubiInput is put in.
                return;

            if(SubiInput.SelectedScheme == "Chaikin")
            {
                for(int i = 0; i < SubiInput.Count; i++)
                {
                    ChaikinSub(RailIndex);
                }
            }

            UpdateRails(RailIndex, 0);
            RefreshListBox2();
        }

        private void ChaikinSub(int RailIndex)
        {
            Rail OldPath = file.GetAllRails()[RailIndex].DeepCopy();
            Rail NewPath = file.GetAllRails()[RailIndex];

            for (int i = 0; i < OldPath.frames.Length; i++)
            {
                if(OldPath.frames[i].u1 == 2)
                {
                    ChaikinSplit(OldPath, ref NewPath, i);
                }
            }

            file.GetAllRails()[RailIndex] = NewPath;
        }

        private void ChaikinSplit(Rail OldPath, ref Rail NewPath, int Index)//lol
        {
            KeyFrame NewNode = NewPath.frames[Index].DeepCopy();
            int NewNodeIndex = NewPath.frames.Length;
            NewPath.InsertFrame(NewNode, NewPath.frames.Length);//Add node at the end to not change the indicies
            KeyFrame MiddleKeyFrame = OldPath.frames[Index];
            KeyFrame FirstKeyframe = OldPath.frames[MiddleKeyFrame.connections[0]];
            KeyFrame LastKeyFrame = OldPath.frames[MiddleKeyFrame.connections[1]];
            NewPath.frames[Index].x = (short)(0.25 * FirstKeyframe.x + 0.75 * MiddleKeyFrame.x);
            NewPath.frames[Index].y = (short)(0.25 * FirstKeyframe.y + 0.75 * MiddleKeyFrame.y);
            NewPath.frames[Index].z = (short)(0.25 * FirstKeyframe.z + 0.75 * MiddleKeyFrame.z);

            NewPath.frames[NewNodeIndex].x = (short)(0.25 * LastKeyFrame.x + 0.75 * MiddleKeyFrame.x);
            NewPath.frames[NewNodeIndex].y = (short)(0.25 * LastKeyFrame.y + 0.75 * MiddleKeyFrame.y);
            NewPath.frames[NewNodeIndex].z = (short)(0.25 * LastKeyFrame.z + 0.75 * MiddleKeyFrame.z);

            NewPath.frames[Index].connections[1] = (short)NewNodeIndex;
            NewPath.frames[NewNodeIndex].connections[0] = (short)Index;
            int ConnectToMiddleIndexOfIndex = -1;
            for(int i = 0; i < LastKeyFrame.connections.Length; i++)
            {
                if (LastKeyFrame.connections[i] == (short)Index)
                    ConnectToMiddleIndexOfIndex = i;
            }
            if(ConnectToMiddleIndexOfIndex != -1)
            {
                NewPath.frames[MiddleKeyFrame.connections[1]].connections[ConnectToMiddleIndexOfIndex] = (short)NewNodeIndex;
            }
        }

        private void InsAfter_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;

            KeyFrame rp = new KeyFrame();
            //rp.u1 = 2;
            rp.NullData();

            file.GetAllRails()[listBox1.SelectedIndex].InsertFrame(rp, listBox2.SelectedIndex + 1);

            int index = listBox2.SelectedIndex+1;
            RefreshListBox2();
            listBox2.SelectedIndex = index;
        }

        private void button2_Click_1(object sender, EventArgs e) {
            SMSSceneReader.Preview preview = SMSSceneReader.MainForm.ScenePreview;
            if (preview != null) {
                xUpDown.Value = (decimal)preview.CameraPos.X;
                yUpDown.Value = (decimal)preview.CameraPos.Y;
                zUpDown.Value = (decimal)preview.CameraPos.Z;
                applyButton_Click(sender, e);
                preview.ForceDraw();
            }

        }
        
        private void startUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(0,  (short)startUpDown.Value);
            UpdatePeriods();
        }

        private void endUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(1, (short)endUpDown.Value);
            UpdatePeriods();
        }

        private void u3UpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(2,  (short)u3UpDown.Value);
            UpdatePeriods();
        }

        private void u4UpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(3,  (short)u4UpDown.Value);
            UpdatePeriods();
        }

        private void u5UpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(4,  (short)u5UpDown.Value);
            UpdatePeriods();
        }

        private void u6UpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(5,  (short)u6UpDown.Value);
            UpdatePeriods();
        }

        private void u7UpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(6,  (short)u7UpDown.Value);
            UpdatePeriods();
        }

        private void u8UpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetConnectionsValue(7,  (short)u8UpDown.Value);
            UpdatePeriods();
        }

        private void connectloop_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndex == -1 || listBox1.Items.Count == 0)
                return;

            DialogResult dialogResult = MessageBox.Show("This operation will modify all points on the current rail. Do you want to proceed?", 
                "Connect Points as Loop", MessageBoxButtons.YesNo);
            
            
            if(dialogResult == DialogResult.Yes)
            {
                Rail rail = file.GetAllRails()[listBox1.SelectedIndex];
                for (int i = 0; i < rail.frames.Count(); i++) {
                    int last, next;

                    if (i == 0) {
                        last = rail.frames.Count()-1;
                    }
                    else {
                        last = i - 1;
                    }

                    if (i == rail.frames.Count() - 1) {
                        next = 0;
                    }
                    else {
                        next = i + 1;
                    }

                    KeyFrame frame = rail.frames[i];
                    if (frame.u1 < 2) {
                        frame.u1 = 2;
                    }
                    frame.connections[0] = (short)last;
                    frame.connections[1] = (short)next;
                    rail.frames[i] = frame;
                }
                RefreshListBox2();
                updateRender();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        void updateRender() {
            SMSSceneReader.Preview preview = SMSSceneReader.MainForm.ScenePreview;
            if (preview != null) {
                preview.ForceDraw();
            }
        }

        private void u2UpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("u2", (short)u2UpDown.Value);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("u3", (short)numericUpDown1.Value);
        }

        private void CurrentKeyframeSetValue(string property, short value) {
            if (listBox1.SelectedIndex == -1 || listBox2.SelectedIndex == -1)
                return;
            KeyFrame rp = file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex];
            Object rpboxed = (Object) rp;
            Type kfType = rpboxed.GetType();
            
            var field = kfType.GetField(property);
            
            field.SetValue(rpboxed, value);
            rp = (KeyFrame)rpboxed;

            file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex] = rp;
            updateRender();
        }

        private void CurrentKeyframeSetConnectionsValue(int index, short value) {
            if (listBox1.SelectedIndex == -1 || listBox2.SelectedIndex == -1)
                return;
            KeyFrame rp = file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex];
            rp.connections[index] = value;
            file.GetAllRails()[listBox1.SelectedIndex].frames[listBox2.SelectedIndex] = rp;
            updateRender();
        }

        private void pitchUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("pitch", (short)pitchUpDown.Value);
        }

        private void yawUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("yaw", (short)yawUpDown.Value);
        }

        private void rollUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("roll", (short)rollUpDown.Value);
        }

        private void speedUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("speed", (short)speedUpDown.Value);
        }

        private void xUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("x", (short)xUpDown.Value);
            UpdatePeriods();
        }

        private void yUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("y", (short)yUpDown.Value);
            UpdatePeriods();
        }

        private void zUpDown_ValueChanged(object sender, EventArgs e) {
            CurrentKeyframeSetValue("z", (short)zUpDown.Value);
            UpdatePeriods();
        }
    }
}
