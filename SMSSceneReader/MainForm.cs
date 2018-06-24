using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using SMSRallyEditor;
using DataReader;
using SMSReader;
using BMDReader;

namespace SMSSceneReader
{
    public partial class MainForm : Form
    {
        const string ARGINFO = "SMS Bin Editor by miluaces v. 1.2.3.2\r\n\r\nCommand Line arguments:\r\nSMSSceneReader.exe [file] [arguments]\r\n--updateparams\t\tUpdate parameter files.\r\n-file [file]\t\t\tLoad file\r\n-common [directory]\tLink to SMS common directory\r\n-stagearc [stagearc.bin]\tLoad Warp Info\r\n\r\nThat's it, the rest are debug related...";
        const string CLEARBLACKLIST = ",";
        const string BMGPATH = "map\\message.bmg";
        const string RAILPATH = "map\\scene.ral";
        const string PRMPATH = "map\\params\\";
        const string DEMOPATH = "map\\camera\\startcamera.bck";
        const string HASHFILE = "./hash.dat";

        public bool Changed = false;   //Whether or not something has changed (for save alert)

        Translate_Buddy Translator = new Translate_Buddy();

        string SceneRoot;
        string SavePath;    //Path to save

        Find Find = new Find(); //Find form
        Preview ScenePreview = null;    //Preview form
        RallyForm RailForm = null;

        ObjectParameters CurrentObjectParameters;   //Parameters of selected object

        public SMSScene LoadedScene;   //Current loaded scene
        RalFile LoadedRails;    //Rails
        BmgFile LoadedMessages; //Messages
        BckFile LoadedDemo;     //Demo

        string StageArc = "";
        string CommonPath = "";
        string ParamPath = "";

        bool EnableParameters = false;  //Whether or not parameters can be edited
        List<PrmFile> LoadedSceneParameters;    //Parameters
        List<PrmFile> LoadedGlobalParameters;    //Parameters

        private bool UpdateInfo = false;    //Whether or not object parameters are queued to update
        private bool UpdateNames = false; //Whether or not names are queued to update
        private bool UpdateDisplay = false; //Whether or not information is being changed so as not to unnecessarily change data.
        private bool UpdateDisplay2 = false;

        SMSScene[] undoDatabase = new SMSScene[256];
        int undoPointer;

        Bmd CameraModel = null;

        /* Form1 */
        public MainForm(string[] args)
        {
            //if (Properties.Settings.Default.needsUpgrade)
            //{
            //    Properties.Settings.Default.Upgrade();
            //    Properties.Settings.Default.needsUpgrade = false;
            //    Properties.Settings.Default.Save();
            //}

            InitializeComponent();

            CommonPath = Properties.Settings.Default.commonPath;
            StageArc = Properties.Settings.Default.stageArc;
            ParamPath = Properties.Settings.Default.paramPath;

            if (CommonPath != "" && CommonPath != null)
                scenariosToolStripMenuItem.Enabled = true;

            LoadedScene = new SMSScene();
            LoadedScene.Objects = new List<GameObject>();

            //Parse Arguments
            int ptr = 0;
            bool makemp = false;
            while (ptr < args.Length)
            {
                switch (args[ptr])
                {
                    case "/?":
                    case "-?":
                    case "?":
                        MessageBox.Show(ARGINFO);
                        break;
                    case "--updateparams":
                        MessageBox.Show("Please select your sms root directory to scan for .bin files.");
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        fbd.Description = "/root/data/scenes directory for Super Mario Sunshine. This is to read object names from existing scene files.";
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string file in Directory.GetFiles(fbd.SelectedPath, "*.bin", SearchOption.AllDirectories))
                            {
                                SMSScene sms = new SMSScene(file);
                                foreach (GameObject go in sms.AllObjects)
                                {
                                    if (File.Exists(SMSScene.BIN_PARAMPATH + go.Hash + ".txt"))
                                    {
                                        if (File.Exists(SMSScene.BIN_PARAMPATH + go.Name + ".txt"))
                                            continue;
                                        File.Move(SMSScene.BIN_PARAMPATH + go.Hash + ".txt", SMSScene.BIN_PARAMPATH + go.Name + ".txt");
                                    }
                                }
                            }
                            MessageBox.Show("Parameters updated!");
                        }
                        else
                            MessageBox.Show("Update cancelled.");
                        break;
                    case "-file":
                        if (File.Exists(args[++ptr])){
                            try { LoadFile(args[ptr]); SavePath = args[ptr]; }
                            catch { newToolStripMenuItem_Click(this, new EventArgs()); }
                        }
                        break;
                    case "-common":
                        if (Directory.Exists(args[++ptr]))
                            CommonPath = args[ptr];
                        break;
                    case "-stagearc":
                        if (File.Exists(args[++ptr]))
                            StageArc = args[ptr];
                        break;
                    case "-params":
                        if (Directory.Exists(args[++ptr]))
                            ParamPath = args[ptr];
                        break;
                    case "-makemp":
                        makemp = true;
                        break;
                    default:
                        if (File.Exists(args[ptr])){
                            try{LoadFile(args[ptr]);}
                            catch{newToolStripMenuItem_Click(this, new EventArgs());}
                        }
                        break;
                }
                ptr++;
            }

            if (StageArc != "")
                warpFinderToolStripMenuItem.Enabled = true;
            else
                warpFinderToolStripMenuItem.Enabled = false;


            if (makemp)
            {
                if (LoadedScene.AllObjects.Count == 0)
                {
                    Environment.Exit(1);
                    return;
                }

                bool marmp2 = false;
                bool marmp3 = false;
                foreach (GameObject o in LoadedScene.AllObjects)
                    if (o.Description == "MarMP2")
                        marmp2 = true;
                    else if (o.Description == "MarMP3")
                        marmp3 = true;

                GameObject mp2 = null;
                GameObject mp3 = null;
                foreach (GameObject o in LoadedScene.AllObjects)
                {
                    if (o.Name == "IdxGroup" && o.DescHash == 15673)
                    {
                        if (!marmp2)
                        {
                            mp2 = new GameObject("EMario", "MarMP2", 124);
                            mp2.Values = new byte[] { 69, 175, 0, 0, 66, 72, 0, 0, 197, 84, 128, 0, 0, 0, 0, 0, 66, 220, 0, 0, 0, 0, 0, 0, 63, 128, 0, 0, 63, 128, 0, 0, 63, 128, 0, 0, 0, 19, 131, 125, 131, 138, 131, 73, 131, 130, 131, 104, 131, 76, 32, 131, 76, 131, 131, 131, 137, 0, 0, 0, 0, 0, 24, 131, 125, 131, 138, 131, 73, 131, 130, 131, 104, 131, 76, 131, 125, 131, 108, 129, 91, 131, 87, 131, 131, 129, 91, 0, 11, 109, 97, 114, 105, 111, 109, 111, 100, 111, 107, 105, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0, 0, 255, 0, 0, 0, 255, 0, 0, 0, 255 };
                            mp2.Parent = o;
                            o.Grouped.Add(mp2);
                        }
                        if (!marmp3)
                        {
                            //mp3 = new GameObject("EMario", "MarMP3", 122);
                            //mp3.Values = new byte[] { 69, 159, 96, 0, 66, 72, 0, 0, 197, 134, 96, 0, 0, 0, 0, 0, 66, 160, 0, 0, 0, 0, 0, 0, 63, 128, 0, 0, 63, 128, 0, 0, 63, 128, 0, 0, 0, 17, 131, 130, 131, 147, 131, 101, 131, 125, 131, 147, 32, 131, 76, 131, 131, 131, 137, 0, 0, 0, 0, 0, 24, 131, 125, 131, 138, 131, 73, 131, 130, 131, 104, 131, 76, 131, 125, 131, 108, 129, 91, 131, 87, 131, 131, 129, 91, 0, 11, 109, 97, 114, 105, 111, 109, 111, 100, 111, 107, 105, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0, 0, 255, 0, 0, 0, 255, 0, 0, 0, 255 };
                            ////mp3 = new GameObject("EMario", "MarMP3", 124);
                            ////mp3.Values = new byte[] { 69, 175, 0, 0, 66, 72, 0, 0, 197, 84, 128, 0, 0, 0, 0, 0, 66, 220, 0, 0, 0, 0, 0, 0, 63, 128, 0, 0, 63, 128, 0, 0, 63, 128, 0, 0, 0, 19, 131, 125, 131, 138, 131, 73, 131, 130, 131, 104, 131, 76, 32, 131, 76, 131, 131, 131, 137, 0, 0, 0, 0, 0, 24, 131, 125, 131, 138, 131, 73, 131, 130, 131, 104, 131, 76, 131, 125, 131, 108, 129, 91, 131, 87, 131, 131, 129, 91, 0, 11, 109, 97, 114, 105, 111, 109, 111, 100, 111, 107, 105, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 0, 0, 0, 255, 0, 0, 0, 255, 0, 0, 0, 255 };
                            //mp3.Parent = o;
                            
                            //o.Grouped.Add(mp3);
                        }
                    }
                }

                if (mp2 != null)
                    LoadedScene.AllObjects.Add(mp2);
                if (mp3 != null)
                    LoadedScene.AllObjects.Add(mp3);

                LoadedScene.Save(SavePath);

                Environment.Exit(0);
                return;
            }
        }

        /* Form Load - Initialize stuff */
        private void Form1_Load(object sender, EventArgs e)
        {
            paramTextBox.Enabled = false;
            createParameterToolStripMenuItem.Enabled = true;
            listBox2.ContextMenuStrip.Items[0].Enabled = false;

            treeView1.ContextMenuStrip = treeNodeContextMenu;
            treeView1.ContextMenuStrip.Enabled = false;
            treeView1.HideSelection = false;

            AddObjectNodes(LoadedScene.Objects.ToArray());

            Application.Idle += Application_Idle;

            if (ParamPath != "")
            {
                LoadedGlobalParameters = new List<PrmFile>();    //Load parameters
                if (Directory.Exists(ParamPath))
                {
                    EnableParameters = true;
                    string[] prmdirs = Directory.GetDirectories(ParamPath);
                    foreach (string dir in prmdirs)
                    {
                        string[] files = Directory.GetFiles(dir);
                        foreach (string fl in files)
                        {
                            FileInfo cf = new FileInfo(fl);
                            if (cf.Extension.ToLower() == ".prm")
                                try { LoadedGlobalParameters.Add(new PrmFile(fl)); }
                                catch { LoadedGlobalParameters = null; return; }
                        }
                    }
                    globalToolStripMenuItem1.Enabled = true;
                }
                else
                    LoadedGlobalParameters = null;
            }
            else
                LoadedGlobalParameters = null;
        }

        /* Form is idle */
        private void Application_Idle(object sender, EventArgs e)
        {
            if (UpdateNames && !ObjName.Focused)
            {
                UpdateNames = false;
                UpdateObjectNames(treeView1.Nodes);
            }
            if (UpdateInfo && !paramTextBox.Focused)
            {
                UpdateInfo = false;
                UpdateObjectInfo();
            }
            if (Changed)
            {
                saveToolStripMenuItem.Enabled = true;
            }
        }

        /* Loads file and updates trees */
        public void LoadFile(string path)
        {
            bool railOpen = RailForm != null;
            if (railOpen)
                RailForm.Close();

            //Clear old files
            if (LoadedRails != null)
                LoadedRails.UnLoad();
            if (LoadedMessages != null)
                LoadedMessages.Close();
            //if (LoadedDemo != null)
            //    LoadedDemo.Close();
            if (LoadedSceneParameters != null)
                foreach (PrmFile pf in LoadedSceneParameters)
                    pf.UnLoad();
            if (LoadedGlobalParameters != null)
                foreach (PrmFile pf in LoadedGlobalParameters)
                    pf.UnLoad();
            if (CameraModel != null)
                CameraModel.Close();
            CameraModel = null;

            if (File.Exists(CommonPath + "/camera/camera_model.bmd"))
            {
                FileBase cambase = new FileBase() { Stream = new FileStream(CommonPath + "/camera/camera_model.bmd", FileMode.Open, FileAccess.Read) };
                CameraModel = new Bmd(cambase);
            }

            treeView1.Nodes.Clear();

            SceneRoot = Path.GetDirectoryName(Path.GetDirectoryName(path)) + "\\";
            LoadedScene = new SMSScene(path);                   //Load scene
            if (Properties.Settings.Default.mainLoadSeconds)
            {
                if (File.Exists(SceneRoot + RAILPATH))
                    LoadedRails = new RalFile(SceneRoot + RAILPATH);    //Load rails
                else
                    LoadedRails = null;
                if (File.Exists(SceneRoot + BMGPATH))
                    LoadedMessages = new BmgFile(SceneRoot + BMGPATH);    //Load messages
                else
                    LoadedMessages = null;
                if (File.Exists(SceneRoot + DEMOPATH))
                    LoadedDemo = new BckFile(SceneRoot + DEMOPATH);    //Load demo
                else
                    LoadedDemo = null;

                LoadedSceneParameters = new List<PrmFile>();    //Load parameters
                if (Directory.Exists(SceneRoot + PRMPATH))
                {
                    EnableParameters = true;
                    string[] prmdirs = Directory.GetDirectories(SceneRoot + PRMPATH);
                    foreach (string dir in prmdirs)
                    {
                        string[] files = Directory.GetFiles(dir);
                        foreach (string fl in files)
                        {
                            FileInfo cf = new FileInfo(fl);
                            if (cf.Extension.ToLower() == ".prm")
                                LoadedSceneParameters.Add(new PrmFile(fl));
                        }
                    }
                }

                if (LoadedRails != null)
                    rallyEditorToolStripMenuItem.Enabled = true;
                else
                    rallyEditorToolStripMenuItem.Enabled = false;

                if (LoadedDemo != null)
                    cameraEditorToolStripMenuItem.Enabled = true;
                else
                    cameraEditorToolStripMenuItem.Enabled = false;

                pRMEditorToolStripMenuItem.Enabled = true;

                if (LoadedMessages != null)
                    messagesToolStripMenuItem.Enabled = true;
                else
                    messagesToolStripMenuItem.Enabled = false;
            }
            else
            {
                LoadedRails = null;
                LoadedDemo = null;
                LoadedMessages = null;
                LoadedSceneParameters = new List<PrmFile>();
                rallyEditorToolStripMenuItem.Enabled = false;
                pRMEditorToolStripMenuItem.Enabled = false;
                messagesToolStripMenuItem.Enabled = false;
            }

            if (LoadedScene.error == 0)
                MessageBox.Show("The file specified could not be found.", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (LoadedScene.error == 2)
                MessageBox.Show("The file specified is either corrupted or cannot be loaded into this program.", "Invalid file type", MessageBoxButtons.OK, MessageBoxIcon.Error);

            AddObjectNodes(LoadedScene.Objects.ToArray());
            Find.treeview1 = treeView1;

            //Update scene preview on scene change
            if (ScenePreview != null)
            {
                ScenePreview.ChangeScene(SceneRoot);

                foreach (GameObject go in LoadedScene.AllObjects)
                    ScenePreview.UpdateObject(go);

                ScenePreview.InitRails(LoadedRails);
                ScenePreview.InitDemo(CameraModel, LoadedDemo);
            }
            else if (Properties.Settings.Default.mainPreviewLoad)
                LoadPreview();

            //Open rail editor if it was open
            if (railOpen && LoadedRails != null)
                rallyEditorToolStripMenuItem_Click(this, new EventArgs());

            ResetUndo();
            Changed = false;
            saveToolStripMenuItem.Enabled = false;
        }

        /* Fills treeview with objects */
        public void AddObjectNodes(GameObject[] array, TreeNode previousNode = null)
        {
            if (previousNode == null)
            {
                //Progress bar
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = LoadedScene.AllObjects.Count;
                toolStripStatusLabel1.Text = "Adding Labels...";

                //Clear treeview
                treeView1.Nodes.Clear();
            }
            foreach (GameObject g in array)
            {
                //Create node and add, then add children
                TreeNode node;
                if (previousNode == null)
                    node = treeView1.Nodes.Add(g.Name);
                else
                    node = previousNode.Nodes.Add(g.Name);

                node.Tag = g;
                AddObjectNodes(g.Grouped.ToArray(), node);  //Add children

                //Update progress bar
                if (toolStripProgressBar1.Value < toolStripProgressBar1.Maximum)
                    toolStripProgressBar1.Value++;
            }
            if (previousNode == null)
            {
                //Expand all nodes
                treeView1.ExpandAll();

                //Progress bar complete
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = 100;
                toolStripStatusLabel1.Text = "Ready";
            }
        }

        /* After an object is selected */
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            UpdateDisplay = true;

            //Clear param text box
            paramTextBox.Enabled = false;
            paramTextBox.Text = "";

            //Clear value text box
            Values.Text = "";

            //Read object information
            GameObject gObject = (GameObject)treeView1.SelectedNode.Tag;
            ID.Value = gObject.Hash;
            ObjName.Text = gObject.Name;
            Flags.Text = gObject.DescHash.ToString();
            if (gObject.Values.Length > 256)
                Values.Text = "Too Long";   //Prevents lag
            else
            {
                if (Properties.Settings.Default.paramHex)
                {
                    foreach (byte b in gObject.Values)
                        Values.Text += b.ToString("X2") + " ";
                }
                else
                {
                    foreach (byte b in gObject.Values)
                        Values.Text += b.ToString() + " ";
                }
            }
            stringValue.Text = gObject.Description;

            //Fill up parameter list
            listBox2.Items.Clear();
            CurrentObjectParameters.ReadObjectParameters(gObject);
            for (int i = 0; i < CurrentObjectParameters.DataNames.Length; i++)
            {
                if (CurrentObjectParameters.DataTypes[i] != ParameterDataTypes.COMMENT)
                    listBox2.Items.Add(CurrentObjectParameters.DataNames[i] + ": " + CurrentObjectParameters.GetParamValue(i, gObject));
                else if (Properties.Settings.Default.showParamInfo)
                    listBox2.Items.Add("*" + CurrentObjectParameters.DataNames[i] + "* = " + CurrentObjectParameters.GetParamValue(i, null));
            }

            ParamBox.Text = CurrentObjectParameters.DisplayName;
            UpdateNames = false;

            //If preview is open, update it
            if (ScenePreview != null)
                ScenePreview.SelectObject((GameObject)treeView1.SelectedNode.Tag);

            createParameterToolStripMenuItem.Enabled = true;
            listBox2.ContextMenuStrip.Items[0].Enabled = true;
            treeView1.ContextMenuStrip.Enabled = true;

            UpdateDisplay = false;
        }

        /* Select the node mouse is clicking */
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
        }

        private int GetRealParamID(ObjectParameters currentObjParams, int index, bool ignorecomments)
        {
            int c = 0;
            for (int i = 0; i < currentObjParams.DataNames.Length && c < index; i++)
            {
                if (!ignorecomments || currentObjParams.DataTypes[i] != ParameterDataTypes.COMMENT)
                    c++;
            }
            return c;
        }

        /* On parameter select */
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
                return;

            UpdateDisplay2 = true;
            
            //Prepare textbox for update
            paramTextBox.Enabled = false;

            //Update parameter information, and set to value
            GameObject gObject = (GameObject)treeView1.SelectedNode.Tag;
            int id = GetRealParamID(CurrentObjectParameters, listBox2.SelectedIndex, Properties.Settings.Default.showParamInfo);
            paramLabel.Text = CurrentObjectParameters.DataNames[id];

            //Add combo items (if there are any)
            paramTextBox.Items.Clear();
            foreach (KeyValuePair<string, string> kvp in CurrentObjectParameters.ComboInfo[id])
                paramTextBox.Items.Add(kvp.Key);

            if (CurrentObjectParameters.DataTypes[id] != ParameterDataTypes.COMMENT)
            {
                paramTextBox.Text = CurrentObjectParameters.GetParamValue(id, gObject);
                paramTextBox.Enabled = true;
            }
            else
            {
                paramTextBox.Text = "";
                paramTextBox.Enabled = false;
            }

            UpdateDisplay2 = false;
        }

        /* New scene */
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Changed)
            {
                DialogResult dr = MessageBox.Show("Changed have been made which have not been saved. Would you like to save before closing?", "Warning", MessageBoxButtons.YesNo);
                switch (dr)
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(sender, new EventArgs());
                        break;
                    case DialogResult.No:
                        break;
                }
            }

            LoadedScene = new SMSScene();
            LoadedScene.Objects = new List<GameObject>();
            AddObjectNodes(LoadedScene.Objects.ToArray());
            ResetUndo();
            Changed = false;
        }

        /* Open file */
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Initial directory
            if (Properties.Settings.Default.PreviousOpen == "")
                openFileDialog1.InitialDirectory = "C:\\";
            else
                openFileDialog1.InitialDirectory = Properties.Settings.Default.PreviousOpen;

            //File dialog
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(openFileDialog1.FileName);

                Properties.Settings.Default.PreviousOpen = fi.DirectoryName;
                Properties.Settings.Default.Save();

                LoadFile(fi.FullName);
                SavePath = fi.FullName;
            }
        }

        /* Save As */
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.PreviousOpen != "")
                saveFileDialog1.InitialDirectory = "C:\\";
            else
                saveFileDialog1.InitialDirectory = Properties.Settings.Default.PreviousOpen;

            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SavePath = saveFileDialog1.FileName;
                LoadedScene.Save(SavePath);
                Changed = false;
            }
        }

        /* Save */
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SavePath == null || SavePath == "")
            {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }

            LoadedScene.Save(SavePath);
            Changed = false;

            saveToolStripMenuItem.Enabled = false;
        }

        /* Create parameter */
        private void createParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActorSelect actSel = new ActorSelect(SMSScene.BIN_PARAMPATH);
            actSel.ButtonText = "Edit";
            if (treeView1.SelectedNode != null)
                actSel.SelectedActor = ((GameObject)treeView1.SelectedNode.Tag).Name;
            if (actSel.ShowDialog() != DialogResult.OK)
                return;

            paramEditor pe = new paramEditor();
            pe.ObjectName = actSel.SelectedActor;
            pe.LoadParameter();
            pe.ShowDialog();

            //Update param stuff
            treeView1_AfterSelect(sender, new TreeViewEventArgs(treeView1.SelectedNode));

            if (ScenePreview != null)
            {
                //Update objects
                ScenePreview.UpdateObjectModel((GameObject)treeView1.SelectedNode.Tag);
                foreach (GameObject go in LoadedScene.AllObjects)
                    if (go.Name == ((GameObject)treeView1.SelectedNode.Tag).Name) ScenePreview.UpdateObject(go);
            }
        }

        /* On DescHash change - no longer used */
        private void Flags_ValueChanged(object sender, EventArgs e)
        {
            ((GameObject)treeView1.SelectedNode.Tag).DescHash = (ushort)Flags.Value;

            int c = 0;
            for (int i = 0; i < LoadedScene.AllObjects.Count; i++)
            {
                if (LoadedScene.AllObjects[i].DescHash == (ushort)Flags.Value)
                    c++;
            }
        }

        /* Update parameter to object */
        private void paramTextBox_TextChanged(object sender, EventArgs e)
        {
            if (UpdateDisplay2 || UpdateDisplay)
                return;
            if (paramTextBox.Enabled == false)
                return;

            Changed = true;

            if (!UpdateInfo)
                CreateUndoSnapshot();

            GameObject gObject = (GameObject)treeView1.SelectedNode.Tag;
            int id = GetRealParamID(CurrentObjectParameters, listBox2.SelectedIndex, Properties.Settings.Default.showParamInfo);
            CurrentObjectParameters.SetParamValue(id, gObject, paramTextBox.Text);
            CurrentObjectParameters.ReadObjectParameters(gObject);

            //Update values string
            Values.Text = "";

            if (Properties.Settings.Default.paramHex)
            {
                foreach (byte b in gObject.Values)
                    Values.Text += b.ToString("X2") + " ";
            }
            else
            {
                foreach (byte b in gObject.Values)
                    Values.Text += b.ToString() + " ";
            }

            UpdateInfo = true;

            //Update scene
            if (ScenePreview != null)
                ScenePreview.UpdateObject((GameObject)treeView1.SelectedNode.Tag);
        }

        /* Duplicate object */
        private void treeView1ContextMenuDuplicate_Clicked(object sender, EventArgs e)
        {
            GameObject parent = (GameObject)treeView1.SelectedNode.Tag;
            GameObject clone = parent.DeepCopy();

            Changed = true;
            CreateUndoSnapshot();

            TreeNode cloneNode = new TreeNode(clone.Name);
            cloneNode.Tag = clone;
            if (parent.Parent == null)
            {
                LoadedScene.Objects.Add(clone);
                treeView1.Nodes.Add(cloneNode);
            }
            else
            {
                parent.Parent.Grouped.Add(clone);
                treeView1.SelectedNode.Parent.Nodes.Add(cloneNode);
            }

            LoadedScene.AllObjects.Add(clone);

            treeView1.SelectedNode = cloneNode;

            //Update preview
            if (ScenePreview != null)
                ScenePreview.UpdateObject(clone);
        }

        /* Delete object */
        private void treeView1ContextMenuDelete_Clicked(object sender, EventArgs e)
        {
            DeleteObject(treeView1.SelectedNode);
        }

        /* Edit Parameter (same as Create Parameter) */
        private void editParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createParameterToolStripMenuItem_Click(sender, e);
        }

        /* Description changed */
        private void stringValue_TextChanged(object sender, EventArgs e)
        {
            if (UpdateDisplay || UpdateDisplay2)
                return;
            if (treeView1.SelectedNode == null)
            {
                stringValue.Text = "";
                return;
            }
            Changed = true;

            CreateUndoSnapshot();

            ((GameObject)treeView1.SelectedNode.Tag).Description = stringValue.Text;
            ((GameObject)treeView1.SelectedNode.Tag).DescHash = GCN.CreateHash(stringValue.Text);   //Update hash too
            Flags.Value = ((GameObject)treeView1.SelectedNode.Tag).DescHash;
        }

        /* Copy object to clipboard */
        private void treeView1ContextMenuCopy_Clicked(object sender, EventArgs e)
        {
            Clipboard.Clear();

            //Save object to byte array
            GameObject sObject = (GameObject)treeView1.SelectedNode.Tag;
            byte[] oData = new byte[sObject.CalculateLength()];
            MemoryStream ms = new MemoryStream(oData);
            sObject.SaveObject(ms);
            ms.Close();

            //Convert byte array to CSV
            string str = "";
            for (int i = 0; i < oData.Length; i++)
                str += oData[i].ToString() + ",";

            //Put string into clipboard
            Clipboard.SetText(str, TextDataFormat.CommaSeparatedValue); 
        }

        /* Paste object from clipboard */
        private void treeView1ContextMenuPaste_Clicked(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText(TextDataFormat.CommaSeparatedValue))
                return;

            //Create object
            GameObject gObject = new GameObject();
            try
            {
                //Get string, and split
                string str = Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
                string[] sdat = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                byte[] oData = new byte[sdat.Length];

                //Read bytes
                for (int i = 0; i < sdat.Length; i++)
                    oData[i] = byte.Parse(sdat[i]);

                //Load object from bytes
                MemoryStream ms = new MemoryStream(oData);
                gObject.LoadObject(ms);
                ms.Close();
            }
            catch
            {
                return; //Not a valid object
            }

            Changed = true;
            CreateUndoSnapshot();

            TreeNode pasteNode = new TreeNode(gObject.Name);
            pasteNode.Tag = gObject;
            //Update lists
            if (treeView1.SelectedNode != null)
            {
                bool isgroup = GameObject.IsGroup(((GameObject)treeView1.SelectedNode.Tag).Name);
                if (isgroup)
                {
                    ((GameObject)treeView1.SelectedNode.Tag).Grouped.Add(gObject);
                    gObject.Parent = (GameObject)treeView1.SelectedNode.Tag;

                    treeView1.SelectedNode.Nodes.Add(pasteNode);
                }
                else if (((GameObject)treeView1.SelectedNode.Tag).Parent == null)
                {
                    LoadedScene.Objects.Add(gObject);
                    treeView1.Nodes.Add(pasteNode);
                }
                else
                {
                    ((GameObject)treeView1.SelectedNode.Tag).Parent.Grouped.Add(gObject);
                    gObject.Parent = ((GameObject)treeView1.SelectedNode.Tag).Parent;
                    treeView1.SelectedNode.Parent.Nodes.Add(pasteNode);
                }
            }
            else
            {
                LoadedScene.Objects.Add(gObject);
                treeView1.Nodes.Add(pasteNode);
            }

            LoadedScene.AllObjects.Add(gObject);

            treeView1.SelectedNode = pasteNode;

            //Update preview
            if (ScenePreview != null)
                ScenePreview.UpdateObject(gObject);
        }

        /* Create default parameters */
        private void treeView1ContextMenuObjectType_Clicked(object sender, EventArgs e)
        {
            GameObject selobject = ((GameObject)treeView1.SelectedNode.Tag);
            string path = "Parameters\\" + selobject.Name + ".txt";
            if (File.Exists(path))  //If there are already parameters, ask to be sure
            {
                if (MessageBox.Show("This will replace the current parameters for this object type. Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                File.Delete(path);
            }

            //Read file from assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader stream = new StreamReader(assembly.GetManifestResourceStream("SMSSceneReader.Object.txt"));
            string data = stream.ReadToEnd();
            stream.Close();

            //Write to file, replacing name
            StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Create));
            sw.Write(data.Replace("<Name>", selobject.Name));
            sw.Close();

            if (ScenePreview != null)
            {
                //Update objects
                ScenePreview.UpdateObjectModel(selobject);
                foreach (GameObject go in LoadedScene.AllObjects)
                    if (go.Name == selobject.Name) ScenePreview.UpdateObject(go);
            }

            treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
        }
        private void treeView1ContextMenuNPCType_Clicked(object sender, EventArgs e)
        {
            //Same as treeView1ContextMenuObjectType, but for NPC.txt
            GameObject selobject = ((GameObject)treeView1.SelectedNode.Tag);
            string path = "Parameters\\" + selobject.Name + ".txt";
            if (File.Exists(path))
            {
                if (MessageBox.Show("This will replace the current parameters for this object type. Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                File.Delete(path);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader stream = new StreamReader(assembly.GetManifestResourceStream("SMSSceneReader.NPC.txt"));
            string data = stream.ReadToEnd();
            stream.Close();

            StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Create));
            sw.Write(data.Replace("<Name>", selobject.Name));
            sw.Close();

            if (ScenePreview != null)
            {
                //Update objects
                ScenePreview.UpdateObjectModel(selobject);
                foreach (GameObject go in LoadedScene.AllObjects)
                    if (go.Name == selobject.Name) ScenePreview.UpdateObject(go);
            }

            treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
        }
        private void treeView1ContextMenuManagerType_Clicked(object sender, EventArgs e)
        {
            //Same as treeView1ContextMenuObjectType, but for Manager.txt
            GameObject selobject = ((GameObject)treeView1.SelectedNode.Tag);
            string path = "Parameters\\" + selobject.Name + ".txt";
            if (File.Exists(path))
            {
                if (MessageBox.Show("This will replace the current parameters for this object type. Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                File.Delete(path);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader stream = new StreamReader(assembly.GetManifestResourceStream("SMSSceneReader.Manager.txt"));
            string data = stream.ReadToEnd();
            stream.Close();

            StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Create));
            sw.Write(data.Replace("<Name>", selobject.Name));
            sw.Close();

            if (ScenePreview != null)
            {
                //Update objects
                ScenePreview.UpdateObjectModel(selobject);
                foreach (GameObject go in LoadedScene.AllObjects)
                    if (go.Name == selobject.Name) ScenePreview.UpdateObject(go);
            }

            treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
        }

        /* Clear parameters */
        private void treeView1ContextMenuClearType_Clicked(object sender, EventArgs e)
        {
            string path = "Parameters\\" + ((GameObject)treeView1.SelectedNode.Tag).Name + ".txt";
            if (File.Exists(path))
            {
                if (MessageBox.Show("This will replace the current parameters for this object type. Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                File.Delete(path);
            }

            if (ScenePreview != null)
            {
                GameObject selobject = ((GameObject)treeView1.SelectedNode.Tag);
                //Update objects
                ScenePreview.UpdateObjectModel(selobject);
                foreach (GameObject go in LoadedScene.AllObjects)
                    if (go.Name == selobject.Name) ScenePreview.UpdateObject(go);
            }

            treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
        }

        /* Update object info */
        public void UpdateObjectInfo()
        {
            int oldid = listBox2.SelectedIndex;
            treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
            listBox2.SelectedIndex = oldid;
        }

        /* Update object names */
        public void UpdateObjectNames(TreeNodeCollection startNodes)
        {
            foreach (TreeNode n in startNodes)
            {
                n.Text = ((GameObject)n.Tag).Name;
                UpdateObjectNames(n.Nodes);
            }
        }

        #region QuickKeys
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!Properties.Settings.Default.mainShortcuts)
                return base.ProcessCmdKey(ref msg, keyData);
            if (keyData == (Keys.Control | Keys.S)) //Save
            {
                saveToolStripMenuItem_Click(this, new EventArgs());
                return true;
            }
            else if (keyData == (Keys.Control | Keys.C)) //Copy
            {
                if (treeView1.Focused)
                {
                    if (treeView1.Nodes.Count > 0 && treeView1.SelectedNode != null)
                        treeView1ContextMenuCopy_Clicked(this, new EventArgs());
                }
                if (listBox2.Focused)
                {
                    if (listBox2.SelectedIndex != -1 && paramTextBox.Enabled)
                    {
                        Clipboard.Clear();
                        int id = GetRealParamID(CurrentObjectParameters, listBox2.SelectedIndex, Properties.Settings.Default.showParamInfo);
                        Clipboard.SetText(CurrentObjectParameters.GetParamValue(id, (GameObject)treeView1.SelectedNode.Tag));
                    }
                }
                else if (stringValue.Focused)
                {
                    if (stringValue.SelectedText != "")
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(stringValue.SelectedText);
                    }
                }
                else if (Values.Focused)
                {
                    if (Values.SelectedText != "")
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(Values.SelectedText);
                    }
                }
                else if (paramTextBox.Focused)
                {
                    if (paramTextBox.SelectedText != "")
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(paramTextBox.SelectedText);
                    }
                }
                else if (ID.Focused)
                {
                    Clipboard.Clear();
                    Clipboard.SetText(ID.Value.ToString());
                }
                else if (Flags.Focused)
                {
                    Clipboard.Clear();
                    Clipboard.SetText(Flags.Value.ToString());
                }
                else if (ObjName.Focused)
                {
                    if (ObjName.SelectedText != "")
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(ObjName.SelectedText);
                    }
                }
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D)) //Duplicate
            {
                if (treeView1.Nodes.Count > 0 && treeView1.SelectedNode != null)
                    treeView1ContextMenuDuplicate_Clicked(this, new EventArgs());
                return true;
            }
            else if (keyData == Keys.Delete) //Delete
            {
                if (treeView1.Nodes.Count > 0 && treeView1.SelectedNode != null)
                    treeView1ContextMenuDelete_Clicked(this, new EventArgs());
                return true;
            }
            else if (keyData == (Keys.Control | Keys.V)) //Paste
            {
                if (treeView1.Focused)
                {
                    treeView1ContextMenuPaste_Clicked(this, new EventArgs());
                }
                else if (stringValue.Focused)
                {
                    if (Clipboard.ContainsText())
                        stringValue.SelectedText = Clipboard.GetText();
                }
                else if (paramTextBox.Focused)
                {
                    if (Clipboard.ContainsText())
                        paramTextBox.SelectedText = Clipboard.GetText();
                }
                else if (Flags.Focused)
                {
                    if (Clipboard.ContainsText())
                    {
                        try { Flags.Value = int.Parse(Clipboard.GetText()); }
                        catch { }
                    }
                }
                return true;
            }
            else if (keyData == (Keys.Control | Keys.W)) //Delete
            {
                Close();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Alt | Keys.P)) //Preview
            {
                if (previewToolStripMenuItem.Enabled)
                    previewToolStripMenuItem_Click(this, new EventArgs());
                return true;
            }
            else if (keyData == (Keys.Control | Keys.P)) //Parameters
            {
                if (createParameterToolStripMenuItem.Enabled)
                    createParameterToolStripMenuItem_Click(this, new EventArgs());
                return true;
            }
            else if (keyData == (Keys.Control | Keys.R))
            {
                if (rallyEditorToolStripMenuItem.Enabled)
                    rallyEditorToolStripMenuItem_Click(this, new EventArgs());
                return true;
            }
            else if (keyData == (Keys.Control | Keys.M))
            {
                if (bMGEditorToolStripMenuItem.Enabled)
                    bMGEditorToolStripMenuItem_Click(this, new EventArgs());
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        /* Show help */
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "https://egaddsworkshop.com/forums/showthread.php?tid=28";
            System.Diagnostics.Process.Start(url);
        }

        /* Show about */
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (AboutBox1 ab1 = new AboutBox1())
                ab1.ShowDialog();
        }

        /* Show find dialog */
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find.parent = this;
            Find.treeview1 = treeView1;
            Find.Show();
            Find.Focus();
        }

        /* On object name text change */
        private void ObjName_TextChanged(object sender, EventArgs e)
        {
            if (UpdateDisplay2 || UpdateDisplay)
                return;
            GameObject obj = (GameObject)treeView1.SelectedNode.Tag;
            if (obj == null || ObjName.ReadOnly)
                return;

            Changed = true;

            if (!UpdateNames)
                CreateUndoSnapshot();

            obj.Name = ObjName.Text;
            obj.Hash = GCN.CreateHash(obj.Name);    //Update hash
            ID.Value = obj.Hash;

            UpdateNames = true;
        }

        /* On hash value changed -- No longer used */
        private void ID_ValueChanged(object sender, EventArgs e)
        {
            GameObject obj = (GameObject)treeView1.SelectedNode.Tag;
            if (obj == null || !ID.ReadOnly)
                return;
            obj.Hash = (ushort)ID.Value;
        }

        /* Find a disrepancy in the file between hash and strings */
        private void findDisrepancyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode[] allnodes = ListNodes(treeView1.Nodes);
            int dr = LoadedScene.VerifyIntegrity(0);

            if (dr >= LoadedScene.AllObjects.Count)
            {
                MessageBox.Show("No disrepancy found!");
                return;
            }

            for (int i = 0; i < allnodes.Length; i++)
            {
                if (allnodes[i].Tag == LoadedScene.AllObjects[dr])
                {
                    treeView1.SelectedNode = allnodes[i];
                    return;
                }
            }
        }

        /* List all nodes */
        private TreeNode[] ListNodes(TreeNodeCollection location)
        {
            List<TreeNode> allnodes = new List<TreeNode>();
            foreach (TreeNode tn in location)
            {
                allnodes.Add(tn);
                if (tn.Nodes.Count > 0)
                    allnodes.AddRange(ListNodes(tn.Nodes));
            }
            return allnodes.ToArray();
        }

        /* Repair broken hashes */
        private void repairFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateUndoSnapshot();
            LoadedScene.RepairHashes();
        }

        /* Show preview */
        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SavePath == null)
            {
                MessageBox.Show("No scene loaded.");
                return;
            }

            LoadPreview();
        }

        private void LoadPreview()
        {
            if (ScenePreview != null)
            {
                ScenePreview.Focus();
                return;
            }
            if (SceneRoot == null)
                return;

            ScenePreview = new Preview(SceneRoot);
            ScenePreview.Show();

            //Feed all objects to preview
            foreach (GameObject go in LoadedScene.AllObjects)
                ScenePreview.UpdateObject(go);

            ScenePreview.InitRails(LoadedRails);
            ScenePreview.InitDemo(CameraModel, LoadedDemo);

            goToToolStripMenuItem.Enabled = true;

            ScenePreview.mainForm = this;
            ScenePreview.FormClosed += new FormClosedEventHandler(ScenePreview_FormClosed); //Closed event
        }

        /* Update the rail form */
        public void UpdateRailForm(int rail, int frame)
        {
            RailForm.UpdateRails(rail, frame);
        }

        /* On preview closed */
        private void ScenePreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Properties.Settings.Default.previewSave)
            {
                Properties.Settings.Default.previewPosition = ScenePreview.DesktopLocation;
                if (ScenePreview.WindowState == FormWindowState.Normal)
                    Properties.Settings.Default.previewSize = ScenePreview.Size;
                Properties.Settings.Default.previewState = ScenePreview.WindowState;
                Properties.Settings.Default.Save();
            }
            ScenePreview = null;
            goToToolStripMenuItem.Enabled = false;
        }

        /* Close form */
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /* On form closing */
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            //If something has been changed, ask to save settings
            if (Changed)
            {
                DialogResult dr = MessageBox.Show("Changed have been made which have not been saved. Would you like to save before closing?", "Warning", MessageBoxButtons.YesNoCancel);
                switch (dr)
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(sender, new EventArgs());
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            if (e.Cancel != true)
            {
                if (RailForm != null)
                    RailForm.Close();

                //Clear old files
                if (LoadedRails != null)
                    LoadedRails.UnLoad();
                if (LoadedMessages != null)
                    LoadedMessages.Close();
                if (LoadedSceneParameters != null)
                    foreach (PrmFile pf in LoadedSceneParameters)
                        pf.UnLoad();
            }
        }

        /* Zoom into object in preview */
        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CurrentObjectParameters.ContainsParameter("X") || !CurrentObjectParameters.ContainsParameter("Y") || !CurrentObjectParameters.ContainsParameter("Z"))
            {
                MessageBox.Show("Object needs X, Y, and Z parameters to zoom in to.");
                return;
            }

            float x = 0f;
            float y = 0f;
            float z = 0f;

            float.TryParse(CurrentObjectParameters.GetParamValue("X", (GameObject)treeView1.SelectedNode.Tag), out x);
            float.TryParse(CurrentObjectParameters.GetParamValue("Y", (GameObject)treeView1.SelectedNode.Tag), out y);
            float.TryParse(CurrentObjectParameters.GetParamValue("Z", (GameObject)treeView1.SelectedNode.Tag), out z);

            ScenePreview.ZoomTo(x, y, z, 300.0f);
        }

        /* Open PRM editor */
        private void pRMEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnableParameters)
            {
                MessageBox.Show("Parameters not loaded.");
                return;
            }

            GameParamForm gpf = new GameParamForm(LoadedSceneParameters);
            gpf.ShowDialog();

            foreach (PrmFile pf in LoadedSceneParameters)
                pf.Save();
        }

        /* Open rail editor */
        private void rallyEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadedRails == null)
            {
                MessageBox.Show("Rails not loaded.");
                return;
            }
            if (RailForm == null)
            {
                RailForm = new RallyForm(LoadedRails);
                RailForm.Show();

                RailForm.FormClosed += new FormClosedEventHandler(RallyForm_FormClosed);
                RailForm.applyButton.Click += new EventHandler(RallyForm_Apply);
                RailForm.listBox1.SelectedIndexChanged += new EventHandler(RallyForm_SelectedRail);
                RailForm.listBox2.SelectedIndexChanged += new EventHandler(RallyForm_SelectedFrame);

                if (ScenePreview != null)
                {
                    ScenePreview.RenderRails = true;
                    ScenePreview.ForceDraw();
                }
            }
            else
                RailForm.Focus();
        }

        //Keeps track of what happens in rail editor
        private void RallyForm_SelectedRail(object sender, EventArgs e)
        {
            if (ScenePreview != null)
            {
                ScenePreview.SelectedRail = RailForm.listBox1.SelectedIndex;
                ScenePreview.ForceDraw();
            }
        }
        private void RallyForm_SelectedFrame(object sender, EventArgs e)
        {
            if (ScenePreview != null)
            {
                ScenePreview.SelectedFrame = RailForm.listBox2.SelectedIndex;
                ScenePreview.ForceDraw();
            }
        }
        private void RallyForm_Apply(object sender, EventArgs e)
        {
            if (ScenePreview != null)
                ScenePreview.ForceDraw();
        }
        private void RallyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ScenePreview != null)
            {
                ScenePreview.RenderRails = false;
                ScenePreview.SelectedRail = -1;
                ScenePreview.SelectedFrame = -1;
                ScenePreview.ForceDraw();
            }
            RailForm = null;
        }

        /* Selects the given object in tree view */
        public void GoToObject(GameObject o, TreeNode parent = null)
        {
            foreach (TreeNode n in ((parent == null) ? treeView1.Nodes : parent.Nodes))
            {
                if (n.Tag == o)
                {
                    treeView1.SelectedNode = n;
                    return;
                }
                if (n.Nodes.Count > 0)
                    GoToObject(o, n);
            }
        }

        /* Open settings */
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings set = new Settings();
            set.ShowDialog();

            CommonPath = Properties.Settings.Default.commonPath;
            StageArc = Properties.Settings.Default.stageArc;

            if (StageArc != "")
                warpFinderToolStripMenuItem.Enabled = true;
            else
                warpFinderToolStripMenuItem.Enabled = false;

            if (CommonPath != "" && CommonPath != null)
                scenariosToolStripMenuItem.Enabled = true;
            else
                scenariosToolStripMenuItem.Enabled = false;
        }

        /* Open BMG Editor */
        private void bMGEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadedMessages != null)
                messagesToolStripMenuItem_Click(sender, e);
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string latest = null;
            try
            {
                WebClient update = new WebClient();
                StreamReader stream = new StreamReader(update.OpenRead("http://blastsoftstudios.com/downloads/smsedit.txt"));
                latest = stream.ReadLine();
                stream.Close();
            }
            catch { MessageBox.Show("Unable to connect to update server.", "Update"); return; }

            if (Application.ProductVersion != latest)
            {
                if (MessageBox.Show("A new update is available. Would you like to download it now?", "Update", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    System.Diagnostics.Process.Start("http://www.blastsoftstudios.com/downloads/smsedit.html");
            }
            else
                MessageBox.Show("You have the latest version.", "Update");
        }

        private bool HasManager(string name)
        {
            GameObject manGroup = null;
            foreach (GameObject go in LoadedScene.AllObjects)
                if (go.Description == "コンダクター初期化用")
                    manGroup = go;
            foreach (GameObject go in manGroup.Grouped)
                if (go.Name == name)
                    return true;
            return false;
        }
        private void AddManager(string name)
        {
            GameObject manGroup = null;
            foreach (GameObject go in LoadedScene.AllObjects)
            {
                if (go.Description == "コンダクター初期化用")
                    manGroup = go;
            }
            ObjectParameters op = new ObjectParameters();
            op.ReadObjectParameters(name);

            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);

            if (op.ContainsParameter("RequiredManager"))
                if (!HasManager(op.GetParamValue("RequiredManager", gObject)))
                    AddManager(op.GetParamValue("RequiredManager", gObject));

            LoadedScene.AllObjects.Add(gObject);
            manGroup.Grouped.Add(gObject);

            //Update lists
            AddObjectNodes(LoadedScene.Objects.ToArray());

            //Update preview
            if (ScenePreview != null)
                ScenePreview.UpdateObject(gObject);
        }

        /* Create blank object */
        private void insertObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create object
            ActorSelect aselect = new ActorSelect(SMSScene.BIN_PARAMPATH);
            if (aselect.ShowDialog() != DialogResult.OK)
                return;

            ObjectParameters op = new ObjectParameters();
            op.ReadObjectParameters(aselect.SelectedActor);

            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);

            AddObject(treeView1.SelectedNode, gObject, op);
        }

        /* Resize object */
        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject gObject = (GameObject)treeView1.SelectedNode.Tag;

            if (gObject == null)
                return;

            SizeSelect ss = new SizeSelect(gObject.Values.Length);

            if (ss.ShowDialog() != DialogResult.OK)
                return;

            Changed = true;
            CreateUndoSnapshot();

            if (ss.SelectedSize > gObject.Values.Length)
            {
                byte[] nval = new byte[ss.SelectedSize];
                for (int i = 0; i < gObject.Values.Length; i++)
                    nval[i] = gObject.Values[i];
                gObject.Values = nval;
            }
            else if (ss.SelectedSize < gObject.Values.Length)
            {
                if (MessageBox.Show("Data will be lost? Continue?", "Data Loss Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    byte[] nval = new byte[ss.SelectedSize];
                    for (int i = 0; i < nval.Length; i++)
                        nval[i] = gObject.Values[i];
                    gObject.Values = nval;
                }
                else
                    return;
            }
            else
                return;

            treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));
        }

        /* Open messages, same as bMGEditorToolStripMenuItem_Click */
        private void messagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadedMessages == null)
            {
                MessageBox.Show("Messages not loaded.");
                return;
            }

            BMGEditor bmgEditor = new BMGEditor(LoadedMessages);
            bmgEditor.ShowDialog();
            LoadedMessages.Save();
        }

        /* Open scenario bmg */
        private void scenariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CommonPath == "")
            {
                MessageBox.Show("Messages not loaded.");
                return;
            }

            BmgFile scenarioNames = new BmgFile(CommonPath + "\\2d\\scenarioname.bmg");
            BMGEditor bmgEditor = new BMGEditor(scenarioNames);
            bmgEditor.ShowDialog();
            scenarioNames.Save();
            scenarioNames.Close();
        }

        /* Open other bmg file */
        private void otherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmgOpenFile.ShowDialog();
            BmgFile bmg = new BmgFile(bmgOpenFile.FileName);
            BMGEditor bmgEditor = new BMGEditor(bmg);
            bmgEditor.ShowDialog();
            bmg.Save();
            bmg.Close();
        }

        /* List warp locations */
        private void warpFinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (StageArc == "")
                return;
            WarpFinder wf = new WarpFinder(StageArc, CommonPath);
            wf.Show();
        }

        /* Corrupts a bin file, while avoiding errors */
        public void CorruptBin(SMSScene scene, string seed)
        {
            Random r = new Random(GCN.CreateHash(seed));
            foreach (GameObject go in scene.AllObjects)
            {
                ObjectParameters op = new ObjectParameters();
                op.ReadObjectParameters(go);

                if (go.Values.Length < 4)
                    continue;

                float x = 0, y = 0, z = 0, rotx = 0, roty = 0, rotz = 0, scx = 0, scy = 0, scz = 0;

                if (op.ContainsParameter("X"))
                    x = float.Parse(op.GetParamValue("X", go));
                if (op.ContainsParameter("Y"))
                    y = float.Parse(op.GetParamValue("Y", go));
                if (op.ContainsParameter("Z"))
                    z = float.Parse(op.GetParamValue("Z", go));

                if (op.ContainsParameter("Pitch"))
                    rotx = float.Parse(op.GetParamValue("Pitch", go));
                else if (op.ContainsParameter("RotationX"))
                    rotx = float.Parse(op.GetParamValue("RotationX", go));
                if (op.ContainsParameter("Yaw"))
                    roty = float.Parse(op.GetParamValue("Yaw", go));
                else if (op.ContainsParameter("RotationY"))
                    roty = float.Parse(op.GetParamValue("RotationY", go));
                if (op.ContainsParameter("Roll"))
                    rotz = float.Parse(op.GetParamValue("Roll", go));
                else if (op.ContainsParameter("RotationZ"))
                    rotz = float.Parse(op.GetParamValue("RotationZ", go));

                if (op.ContainsParameter("ScaleX"))
                    scx = float.Parse(op.GetParamValue("ScaleX", go));
                if (op.ContainsParameter("ScaleY"))
                    scy = float.Parse(op.GetParamValue("ScaleY", go));
                if (op.ContainsParameter("ScaleZ"))
                    scz = float.Parse(op.GetParamValue("ScaleZ", go));

                if (r.Next(0, 50) < 10)
                {
                    x += r.Next(-200, 200);
                    y += r.Next(-200, 200);
                    z += r.Next(-200, 200);
                }
                if (r.Next(0, 50) < 20)
                {
                    rotx += r.Next(-180, 180);
                    roty += r.Next(-180, 180);
                    rotz += r.Next(-180, 180);
                }
                if (r.Next(0, 50) < 10)
                {
                    scx *= r.Next(10, 250) / 125f;
                    scy *= r.Next(10, 250) / 125f;
                    scz *= r.Next(10, 250) / 125f;
                }

                if (r.Next(120) == 1)
                {
                    go.Description += seed;
                    go.DescHash = GCN.CreateHash(go.Description);
                }

                if (op.ContainsParameter("X"))
                    op.SetParamValue("X", go, x.ToString());
                if (op.ContainsParameter("Y"))
                    op.SetParamValue("Y", go, y.ToString());
                if (op.ContainsParameter("Z"))
                    op.SetParamValue("Z", go, z.ToString());

                if (op.ContainsParameter("Pitch"))
                    op.SetParamValue("Pitch", go, rotx.ToString());
                else if (op.ContainsParameter("RotationX"))
                    op.SetParamValue("RotationX", go, rotx.ToString());
                if (op.ContainsParameter("Yaw"))
                    op.SetParamValue("Yaw", go, roty.ToString());
                else if (op.ContainsParameter("RotationY"))
                    op.SetParamValue("RotationY", go, roty.ToString());
                if (op.ContainsParameter("Roll"))
                    op.SetParamValue("Roll", go, rotz.ToString());
                else if (op.ContainsParameter("RotationZ"))
                    op.SetParamValue("RotationZ", go, rotz.ToString());

                if (op.ContainsParameter("ScaleX"))
                    op.SetParamValue("ScaleX", go, scx.ToString());
                if (op.ContainsParameter("ScaleY"))
                    op.SetParamValue("ScaleY", go, scy.ToString());
                if (op.ContainsParameter("ScaleZ"))
                    op.SetParamValue("ScaleZ", go, scz.ToString());

                if (r.Next(5) == 1)
                    if (op.ContainsParameter("WarpTo"))
                        op.SetParamValue("WarpTo", go, r.Next(0, 60).ToString());
            }
        }

        /* Corrupts a bmg file, while avoiding errors */
        public void CorruptBmg(BmgFile msg, string seed)
        {
            Random r = new Random(GCN.CreateHash(seed));
            for (int i = 0; i < msg.Count; i++)
            {
                string cur = msg.GetString(i);
                if (cur == "")
                    continue;
                string ostr = "";
                if (cur.Contains((char)0x1A))
                    continue;
                switch (r.Next(9))
                {
                    case 0:
                    case 1:
                        for (int j = 0; j < cur.Length; j++)
                        {
                            char c = (char)((r.Next(2) == 1) ? r.Next(0x41, 0x5A) : r.Next(0x30, 0x39));
                            if (r.Next(2) == 1)
                                c = char.ToLower(c);
                            ostr += c;
                        }
                        break;
                    case 2:
                        int ptr = r.Next(0, cur.Length);
                        ostr = cur.Substring(0, ptr) + seed + cur.Substring(ptr, cur.Length - ptr);
                        break;
                    case 3:
                        ostr = seed;
                        while (true)
                        {
                            if (r.Next(3) == 1)
                                break;
                            else
                                ostr += seed;
                        }
                        break;
                    case 4:
                        ostr = seed;
                        break;
                    default:
                        ostr = cur;
                        break;
                }
                msg.SetString(i, ostr);
            }
        }

        /* Corrupts a prm file, while avoiding errors */
        public void CorruptPrm(PrmFile prm, string seed)
        {
            Random r = new Random(GCN.CreateHash(seed));
            string[] keys = prm.GetAllKeys();
            foreach (string key in keys)
            {
                PrmData dat = prm.GetData(key);
                switch (dat.Size)
                {
                    case 1:
                        dat.ByteValue = (byte)r.Next(0, 256);
                        prm.SetData(key, dat);
                        continue;
                    case 2:
                        dat.ShortValue = (short)((float)dat.ShortValue * (float)r.Next(0, 200) / 100f);
                        prm.SetData(key, dat);
                        continue;
                    case 4:
                        if (dat.IntValue == 0)
                            continue;
                        if (dat.SingleValue < 10000f && dat.SingleValue > -10000f)
                            dat.SingleValue = dat.SingleValue * (float)r.Next(0, 200) / 100f;
                        else
                            dat.IntValue += r.Next(-dat.IntValue + 1, 5);
                        prm.SetData(key, dat);
                        continue;
                    case 12:
                        Vector vec = dat.VectorValue;
                        vec.X += r.Next(-200, 200);
                        vec.Y += r.Next(-200, 200);
                        vec.Z += r.Next(-200, 200);
                        dat.VectorValue = vec;
                        prm.SetData(key, dat);
                        continue;
                    default:
                        continue;
                }
            }
        }

        private void corruptBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to corrupt the current scene?", "Corruption", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            Changed = true;
            CreateUndoSnapshot();
            StringInput si = new StringInput("Seed:");
            si.ShowDialog();
            CorruptBin(LoadedScene, si.Input);
        }

        private void corruptMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadedMessages == null)
                return;
            if (MessageBox.Show("Are you sure you want to corrupt the current scene's messages?\nThis cannot be undone.", "Corruption", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            StringInput si = new StringInput("Seed:");
            si.ShowDialog();
            CorruptBmg(LoadedMessages, si.Input);
            LoadedMessages.Save();
        }

        private void corruptParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to corrupt the current scene's game parameters?\nThis cannot be undone.", "Corruption", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            StringInput si = new StringInput("Seed:");
            si.ShowDialog();
            for (int i = 0; i < LoadedSceneParameters.Count; i++)
            {
                CorruptPrm(LoadedSceneParameters[i], si.Input);
                LoadedSceneParameters[i].Save();
            }
        }

        private void corruptAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringInput si = new StringInput("Seed:");
            si.ShowDialog();

            ProgressForm pf = new ProgressForm("Corrupting...");

            folderBrowserDialog1.Description = "/root/data/scenes directory for Super Mario Sunshine. This is to corrupt every file in the directory and subdirectories.";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("Are you sure you want to corrupt \"" + folderBrowserDialog1.SelectedPath + "\"? This will corrupt every .bin .bmg and .prm file in this directory and subdirectories. This cannot be undone.", "Corruption", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                string[] alldirs = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.*", SearchOption.AllDirectories);
                int finished = alldirs.Length;
                int progress = 0;

                pf.Show();
                pf.Focus();
                foreach (string file in alldirs)
                {
                    FileInfo fi = new FileInfo(file);
                    switch (fi.Extension)
                    {
                        case ".bin":
                            SMSScene scene = new SMSScene(file);
                            CorruptBin(scene, si.Input);
                            scene.Save(file);
                            break;
                        case ".bmg":
                            BmgFile bmg = new BmgFile(file);
                            CorruptBmg(bmg, si.Input);
                            bmg.Save();
                            bmg.Close();
                            break;
                        case ".prm":
                            PrmFile prm = new PrmFile(file);
                            CorruptPrm(prm, si.Input);
                            prm.Save();
                            break;
                    }
                    progress++;

                    pf.UpdateProgress((int)((float)progress / (float)finished * 100f));
                }
                MessageBox.Show("Corrupted everything!");
            }

            if (SavePath != "")
                LoadFile(SavePath);
        }

        /* Edit selected parameter */
        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            paramEditor pe = new paramEditor();
            pe.ObjectName = ((GameObject)treeView1.SelectedNode.Tag).Name;
            pe.LoadParameter();
            pe.ShowDialog();

            //Update param stuff
            treeView1_AfterSelect(sender, new TreeViewEventArgs(treeView1.SelectedNode));

            if (ScenePreview != null)
            {
                //Update objects
                ScenePreview.UpdateObjectModel((GameObject)treeView1.SelectedNode.Tag);
                foreach (GameObject go in LoadedScene.AllObjects)
                    if (go.Name == ((GameObject)treeView1.SelectedNode.Tag).Name) ScenePreview.UpdateObject(go);
            }
        }

        /* Update combo box selection to actual value */
        private void paramTextBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (paramTextBox.SelectedIndex == -1 || listBox2.SelectedIndex == -1)
                return;

            int id = GetRealParamID(CurrentObjectParameters, listBox2.SelectedIndex, Properties.Settings.Default.showParamInfo);
            string value = CurrentObjectParameters.ComboInfo[id][paramTextBox.Text];
            BeginInvoke(new Action(() => paramTextBox.Text = value));
        }

        private void globalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pRMEditorToolStripMenuItem_Click(sender, e);
        }

        private void globalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!EnableParameters)
            {
                MessageBox.Show("Parameters not loaded.");
                return;
            }

            GameParamForm gpf = new GameParamForm(LoadedGlobalParameters);
            gpf.ShowDialog();

            foreach (PrmFile pf in LoadedGlobalParameters)
                pf.Save();
        }

        private void clearMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Changed = true;
            CreateUndoSnapshot();
            for (int i = LoadedScene.AllObjects.Count - 1; i >= 0; i--)
            {
                ObjectParameters op = new ObjectParameters();
                op.ReadObjectParameters(LoadedScene.AllObjects[i].Name);

                if (op.ContainsParameter("Default"))
                {
                    if (op.GetParamValue("Default", LoadedScene.AllObjects[i]).ToLower() != "true")
                    {
                        DeleteObject(GetObjectNode(LoadedScene.AllObjects[i]), false);
                    }
                }
                else
                {
                    DeleteObject(GetObjectNode(LoadedScene.AllObjects[i]), false);
                }
            }
        }

        public void ResetUndo()
        {
            undoPointer = 0;
            for (int i = 0; i < undoDatabase.Length; i++)
                undoDatabase[i] = null;
        }
        public void CreateUndoSnapshot()
        {
           if (undoPointer + 1 < undoDatabase.Length)
            {
                undoDatabase[undoPointer++] = LoadedScene.Clone();
                for (int i = undoPointer; i < undoDatabase.Length; i++)
                    undoDatabase[i] = null;
            }
            else
            {
                ScrollUndoDatabase();
                undoDatabase[undoPointer++] = LoadedScene.Clone();
            }

            TestUndo();
        }
        private void TestUndo()
        {
            if (CanUndo())
                undoToolStripMenuItem.Enabled = true;
            else
                undoToolStripMenuItem.Enabled = false;
            if (CanRedo())
                redoToolStripMenuItem.Enabled = true;
            else
                redoToolStripMenuItem.Enabled = false;
        }
        private void ScrollUndoDatabase()
        {
            if (undoPointer > 0)
                undoPointer--;
            for (int i = 0; i < undoDatabase.Length - 1; i++)
                undoDatabase[i] = undoDatabase[i + 1];
            undoDatabase[undoDatabase.Length - 1] = null;
        }
        public bool CanUndo()
        {
            if (undoPointer > 0)
                return true;
            return false;
        }
        public bool CanRedo()
        {
            if (undoDatabase[undoPointer] != null)
                return true;
            return false;
        }
        public void Undo()
        {
            if (CanUndo())
            {
                listBox2.SelectedIndex = -1;
                LoadedScene = undoDatabase[--undoPointer];
                AddObjectNodes(LoadedScene.Objects.ToArray());
            }
            TestUndo();
        }
        public void Redo()
        {
            if (CanRedo())
            {
                listBox2.SelectedIndex = -1;
                LoadedScene = undoDatabase[undoPointer++];
                AddObjectNodes(LoadedScene.Objects.ToArray());
            }
            TestUndo();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }
        public static GameObject GetRootObject(SMSScene scene)
        {
            if (scene.Objects.Count > 1)
                throw new NotImplementedException();
            if (scene.Objects.Count == 0)
                return null;
            return scene.Objects[0];
        }
        public static List<GameObject> GetChildObjects(GameObject o, string name = null, string desc = null, byte[] objparams = null, int objpoffset = 0)
        {
            if (o == null)
                return null;
            List<GameObject> objects = new List<GameObject>();
            foreach (GameObject obj in o.Grouped){
                //Check values
                bool objval = true;
                if (objparams != null)
                {
                    for (int i = 0; i < objparams.Length; i++)
                    {
                        if (i + objpoffset >= obj.Values.Length || objparams[i] != obj.Values[i + objpoffset])
                        {
                            objval = false;
                            break;
                        }
                    }
                }

                bool objname = (name != null) ? (obj.Name == name) : true;
                bool objdesc = (desc != null) ? (obj.Description == desc) : true;
                if (objval && objname && objdesc)
                    objects.Add(obj);
            }
            return objects;
        }
        public TreeNode GetObjectNode(GameObject node, TreeNodeCollection next = null)
        {
            if (next == null)
                next = treeView1.Nodes;
            foreach (TreeNode tn in next)
            {
                if (tn.Tag == node)
                    return tn;
                TreeNode o = GetObjectNode(node, tn.Nodes);
                if (o != null)
                    return o;
            }
            return null;
        }
        public void DeleteObject(TreeNode node, bool snapshot = true)
        {
            if (snapshot)
            {
                Changed = true;
                CreateUndoSnapshot();
            }

            if (((GameObject)node.Tag).Parent == null)
            {
                LoadedScene.Objects.Remove((GameObject)node.Tag);
                treeView1.Nodes.Remove(node);
            }
            else
            {
                ((GameObject)node.Tag).Parent.Grouped.Remove((GameObject)node.Tag);
                node.Parent.Nodes.Remove(node);
            }

            LoadedScene.AllObjects.Remove((GameObject)node.Tag);

            //Remove from preview
            if (ScenePreview != null)
                ScenePreview.RemoveObject((GameObject)node.Tag);
        }
        public void AddObject(TreeNode node, GameObject gObject, ObjectParameters op)
        {
            Changed = true;
            CreateUndoSnapshot();

            if (ScenePreview != null && op.ContainsParameter("X") && op.ContainsParameter("Y") && op.ContainsParameter("Z"))
            {
                op.SetParamValue("X", gObject, ScenePreview.CameraPos.X);
                op.SetParamValue("Y", gObject, ScenePreview.CameraPos.Y);
                op.SetParamValue("Z", gObject, ScenePreview.CameraPos.Z);
            }
            if (op.ContainsParameter("ScaleX") && op.ContainsParameter("ScaleY") && op.ContainsParameter("ScaleZ"))
            {
                op.SetParamValue("ScaleX", gObject, 1.0f);
                op.SetParamValue("ScaleY", gObject, 1.0f);
                op.SetParamValue("ScaleZ", gObject, 1.0f);
            }

            if (op.ContainsParameter("RequiredManager"))
                if (!HasManager(op.GetParamValue("RequiredManager", gObject)))
                    AddManager(op.GetParamValue("RequiredManager", gObject));

            //Update lists
            if (node != null)
            {
                bool isgroup = GameObject.IsGroup(((GameObject)node.Tag).Name);
                if (isgroup)
                {
                    ((GameObject)node.Tag).Grouped.Add(gObject);
                    gObject.Parent = (GameObject)node.Tag;
                }
                else if (((GameObject)node.Tag).Parent == null)
                    LoadedScene.Objects.Add(gObject);
                else
                {
                    ((GameObject)node.Tag).Parent.Grouped.Add(gObject);
                    gObject.Parent = ((GameObject)node.Tag).Parent;
                }
            }
            else
                LoadedScene.Objects.Add(gObject);

            LoadedScene.AllObjects.Add(gObject);

            AddObjectNodes(LoadedScene.Objects.ToArray());

            //Update preview
            if (ScenePreview != null)
                ScenePreview.UpdateObject(gObject);
        }
        public GameObject GetScene()
        {
            List<GameObject> scene = GetChildObjects(GetRootObject(LoadedScene), "MarScene");
            if (scene == null || scene.Count == 0)
                return null;
            return scene[0];
        }
        public GameObject GetStrategy()
        {
            List<GameObject> strat = GetChildObjects(GetScene(), "Strategy");
            if (strat == null || strat.Count == 0)
                return null;
            return strat[0];
        }
        public GameObject GetInitializationGroup()
        {
            List<GameObject> group = GetChildObjects(GetRootObject(LoadedScene), "GroupObj", "コンダクター初期化用", new byte[] { }, 0);
            if (group == null || group.Count == 0)
                return null;
            return group[0];
        }
        public GameObject GetGameGroup(int id)
        {
            return null;
        }
        public GameObject GetObjectGroup()
        {
            List<GameObject> group = GetChildObjects(GetStrategy(), "IdxGroup", null, new byte[]{ 0, 0, 0, 3 }, 0);
            if (group == null || group.Count == 0)
                return null;
            return group[0];
        }
        public GameObject GetEnemyGroup()
        {
            List<GameObject> group = GetChildObjects(GetStrategy(), "IdxGroup", null, new byte[] { 0, 0, 0, 7 }, 0);
            if (group.Count == 0)
                return null;
            return group[0];
        }
        public GameObject GetNPCGroup()
        {
            List<GameObject> group = GetChildObjects(GetStrategy(), "IdxGroup", null, new byte[] { 0, 0, 0, 9 }, 0);
            if (group.Count == 0)
                return null;
            return group[0];
        }
        public GameObject GetShineGroup()
        {
            List<GameObject> group = GetChildObjects(GetStrategy(), "IdxGroup", null, new byte[] { 0, 0, 0, 5 }, 0);
            if (group.Count == 0)
                return null;
            return group[0];
        }

        private void decorationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void breakableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("BrickBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "BrickBlock");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void watermelonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("WatermelonBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "WatermelonBlock");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void iceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("IceBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "IceBlock");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void genericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("MapObjBase");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("NormalBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "NormalBlock");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void sandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("SandBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "SandBlock");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void railBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("RailBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "railvariant1");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void woodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("WoodBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "WoodBlockLarge");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void startingPlatformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("MapObjBase");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Model", gObject, "SkyIsland");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void emptyMapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            clearMapToolStripMenuItem_Click(sender, e);
        }

        private void changeStageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("MapObjChangeStage");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Type", gObject, "ChangeStage");
            op.SetParamValue("Desc", gObject, "ステージ切替オブジェ キャラ");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void pineappleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ResetFruit");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ボステレサ用フルーツ キャラ");
            op.SetParamValue("Model", gObject, "FruitPine");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void treeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("Palm");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "palm キャラ");
            op.SetParamValue("Model", gObject, "palmNormal");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void coconutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ResetFruit");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ボステレサ用フルーツ キャラ");
            op.SetParamValue("Model", gObject, "FruitCoconut");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void durianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ResetFruit");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ボステレサ用フルーツ キャラ");
            op.SetParamValue("Model", gObject, "FruitDurian");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void redPepperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ResetFruit");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ボステレサ用フルーツ キャラ");
            op.SetParamValue("Model", gObject, "FruitRedPepper");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void bananaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ResetFruit");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ボステレサ用フルーツ キャラ");
            op.SetParamValue("Model", gObject, "FruitBanana");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void papayaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ResetFruit");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ボステレサ用フルーツ キャラ");
            op.SetParamValue("Model", gObject, "FruitPapaya");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void coinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("Coin");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "コイン キャラ");
            op.SetParamValue("Model", gObject, "coin");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void redCoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("CoinRed");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "赤コイン キャラ");
            op.SetParamValue("Model", gObject, "coin_red");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void blueCoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("CoinBlue");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "青コイン キャラ");
            op.SetParamValue("Model", gObject, "coin_blue");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void shineSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("Shine");
            GameObject objGroup = GetShineGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "スター キャラ");
            op.SetParamValue("Model", gObject, "shine");
            gObject.Description = "New Shine " + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void hoverNozzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("NozzleBox");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ノズルボックス（通常） キャラ");
            op.SetParamValue("Model", gObject, "NozzleBox");
            gObject.Description = "権利ノズルボックス（ホバー） " + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void rocketNozzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("NozzleBox");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ノズルボックス（通常） キャラ");
            op.SetParamValue("Model", gObject, "NozzleBox");
            gObject.Description = "ノズルボックス（ロケット） " + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void turboNozzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("NozzleBox");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ノズルボックス（通常） キャラ");
            op.SetParamValue("Model", gObject, "NozzleBox");
            gObject.Description = "ノズルボックス（背面）" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void yoshiEggToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("EggYoshi");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "ヨッシーの卵 キャラ");
            op.SetParamValue("Model", gObject, "eggYoshi");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void upMushroomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("Mushroom1upX");
            GameObject objGroup = GetShineGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "１ＵＰキノコ キャラ");
            op.SetParamValue("Model", gObject, "mushroom1upX");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void sHDBSlamBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("SuperHipDropBlock");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "スーパーヒップドロップブロック キャラ");
            op.SetParamValue("Model", gObject, "SuperHipDropBlock");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void woodBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("WoodBox");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Desc", gObject, "木箱（偽物） キャラ");
            op.SetParamValue("Model", gObject, "WoodBox");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void makeMovingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("No object selected");
                return;
            }
            GameObject go = (GameObject)treeView1.SelectedNode.Tag;
            ObjectParameters op = new ObjectParameters(go.Name);
            op.Adjust(go);
            if (!op.ContainsParameter("Rail"))
            {
                MessageBox.Show("Selected object does not support rails.");
                return;
            }

            if (op.GetParamValue("Rail", go) != "(null)")
            {
                rallyEditorToolStripMenuItem_Click(sender, e);
                return;
            }

            string railname = "New Rail " + new Random().Next();
            op.SetParamValue("Rail", go, railname);

            LoadedRails.AddRail(railname);
            rallyEditorToolStripMenuItem_Click(sender, e);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Translator.Translate(Translate_Buddy.LANGUAGE_JAPANESE, Translate_Buddy.LANGUAGE_ENGLISH, stringValue.Text);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Translator.Translate(Translate_Buddy.LANGUAGE_JAPANESE, Translate_Buddy.LANGUAGE_ENGLISH, paramTextBox.Text);
        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://smsrealm.net/board/thread.php?id=4");
        }

        private void hexadecimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.paramHex = hexadecimalToolStripMenuItem.Checked;

            if (treeView1.SelectedNode?.Tag == null)
                return;

            GameObject gObject = (GameObject)treeView1.SelectedNode.Tag;

            if (gObject.Values.Length == 0)
                return;

            Values.Text = "";

            if (Properties.Settings.Default.paramHex)
            {
                foreach (byte b in gObject.Values)
                    Values.Text += b.ToString("X2") + " ";
            }
            else
            {
                foreach (byte b in gObject.Values)
                    Values.Text += b.ToString() + " ";
            }

            Properties.Settings.Default.Save();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Values.Text);
        }

        private void asArrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string array = "{ ";

            if (treeView1.SelectedNode?.Tag == null)
                return;

            GameObject gObject = (GameObject)treeView1.SelectedNode.Tag;

            if (gObject.Values.Length == 0)
                return;
            for (int i = 0; i < gObject.Values.Length; i++)
            {
                byte b = gObject.Values[i];
                array += "0x" + b.ToString("X2");
                if (i < gObject.Values.Length - 1)
                    array += ", ";
            }
            array += " }";

            Clipboard.SetText(array);
        }

        private void cameraEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadedDemo == null)
                return;

            MessageBox.Show("Warning: This tool is still WIP and is neither accurate nor able to edit the camera demos.");

            if (ScenePreview != null)
            {

                ScenePreview.RenderDemo = true;
                ScenePreview.ForceDraw();
            }
        }

        private void runDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ScenePreview != null && LoadedDemo != null)
                ScenePreview.StartDemoAnimation();
        }

        private void iceCartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("IceCar");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "屋台（ドルピック） キャラ");
            op.SetParamValue("Data", gObject, "ice_car");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void footballToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("Football");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "サッカーボール キャラ");
            op.SetParamValue("Data", gObject, "football");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void balloonBallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("Football");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "サッカーボール キャラ");
            op.SetParamValue("Data", gObject, "baloonball");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void coconutBallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("Football");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "サッカーボール キャラ");
            op.SetParamValue("Data", gObject, "coconut_ball");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void strollinStuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "HamuKuriManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("HamuKuriManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ハムクリマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 20);
                mop.SetParamValue("Unknown2", mObject, 3);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ハムクリマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("HamuKuri");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "ハムクリマネージャー キャラ");
            op.SetParamValue("Data", gObject, "ハムクリマネージャー");
            op.SetParamValue("Rail", gObject, "(null)");
            op.SetParamValue("Padding", gObject, -1);
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void kaneStrollingStuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "HaneHamuKuriManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("HaneHamuKuriManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "はねハムクリマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 15);
                mop.SetParamValue("Unknown2", mObject, 0);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "はねハムクリマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("HaneHamuKuri");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "はねハムクリマネージャー キャラ");
            op.SetParamValue("Data", gObject, "はねハムクリマネージャー");
            op.SetParamValue("Rail", gObject, "(null)");
            op.SetParamValue("Padding", gObject, -1);
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void typicalEnemyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "TypicalManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("TypicalManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "典型敵マネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 3);
                mop.SetParamValue("Unknown2", mObject, 0);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "典型敵マネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("TypicalEnemy");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "典型敵マネージャー キャラ");
            op.SetParamValue("Data", gObject, "典型敵マネージャー");
            op.SetParamValue("Rail", gObject, "(null)");
            op.SetParamValue("Padding", gObject, -1);
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void tramplinStuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "HinoKuri2Manager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("HinoKuri2Manager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ヒノクリ２マネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 3);
                mop.SetParamValue("Unknown2", mObject, 0);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ヒノクリ２マネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }
            managers = GetChildObjects(manGroup, "HamuKuriManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("HamuKuriManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ハムクリマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 20);
                mop.SetParamValue("Unknown2", mObject, 3);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ハムクリマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }
            managers = GetChildObjects(manGroup, "NameKuriManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("NameKuriManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ナメクリマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 20);
                mop.SetParamValue("Unknown2", mObject, 3);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ナメクリマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }
            managers = GetChildObjects(manGroup, "PopoManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("PopoManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ポポマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 20);
                mop.SetParamValue("Unknown2", mObject, 3);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ポポマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("HinoKuri2");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "ヒノクリ２マネージャー キャラ");
            op.SetParamValue("Data", gObject, "ヒノクリ２マネージャー");
            op.SetParamValue("Rail", gObject, "(null)");
            op.SetParamValue("Padding", gObject, -1);
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void theifStrollinStuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "DoroHamuKuriManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("DoroHamuKuriManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ドロクリマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 3);
                mop.SetParamValue("Unknown2", mObject, 0);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ドロクリマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("DoroHamuKuri");
            GameObject objGroup = GetEnemyGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "ドロクリマネージャー キャラ");
            op.SetParamValue("Data", gObject, "ドロクリマネージャー");
            op.SetParamValue("Rail", gObject, "(null)");
            op.SetParamValue("Padding", gObject, -1);
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void signToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "BoardNpcManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("BoardNpcManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "看板ＮＰＣマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 3);
                mop.SetParamValue("Unknown2", mObject, 0);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "看板ＮＰＣマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("NPCBoard");
            GameObject objGroup = GetNPCGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "看板ＮＰＣマネージャー キャラ");
            op.SetParamValue("Data", gObject, "看板ＮＰＣマネージャー");
            op.SetParamValue("Rail", gObject, "(null)");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void eggGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "EggGenManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("EggGenManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ヨッシーの卵ジェネレーターマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 3);
                mop.SetParamValue("Unknown2", mObject, 0);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ヨッシーの卵ジェネレーターマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("EggGenerator");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "ヨッシーの卵ジェネレーターマネージャー キャラ");
            op.SetParamValue("Data", gObject, "ヨッシーの卵ジェネレーターマネージャー");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void wickedEggGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameObject manGroup = GetInitializationGroup();
            if (manGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            List<GameObject> managers = GetChildObjects(manGroup, "EggGenManager", null, null, 0);
            if (managers.Count == 0)
            {
                ObjectParameters mop = new ObjectParameters("EggGenManager");
                GameObject mObject = mop.CreateEmpty();
                mop.Adjust(mObject);
                mop.SetParamValue("Character", mObject, "ヨッシーの卵ジェネレーターマネージャー キャラ");
                mop.SetParamValue("Unknown1", mObject, 3);
                mop.SetParamValue("Unknown2", mObject, 0);
                mop.SetParamValue("Unknown3", mObject, 0);
                mObject.Description = "ヨッシーの卵ジェネレーターマネージャー";
                mObject.DescHash = GCN.CreateHash(mObject.Description);
                AddObject(GetObjectNode(manGroup), mObject, mop);
            }

            ObjectParameters op = new ObjectParameters("WickedEggGenerator");
            GameObject objGroup = GetObjectGroup();
            GameObject gObject = op.CreateEmpty();
            op.Adjust(gObject);
            op.SetParamValue("Manager", gObject, "ヨッシーの卵ジェネレーターマネージャー キャラ");
            op.SetParamValue("Data", gObject, "ヨッシーの卵ジェネレーターマネージャー");
            gObject.Description = "Unique" + new Random().Next();
            gObject.DescHash = GCN.CreateHash(gObject.Description);

            if (objGroup == null)
            {
                MessageBox.Show("This map does not have the required groups.");
                return;
            }
            AddObject(GetObjectNode(objGroup), gObject, op);
        }

        private void createPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
