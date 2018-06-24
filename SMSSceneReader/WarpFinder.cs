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
using DataReader;

namespace SMSSceneReader
{
    public partial class WarpFinder : Form
    {
        string arc;
        string common;

        SMSScene stageArc;

        GameObject[][] Strings;
        GameObject[] Tables;

        public string[][] levels;
        public string[] names;

        public WarpFinder(string StageArc, string CommonPath)
        {
            arc = StageArc;
            common = CommonPath;

            InitializeComponent();
        }

        private void LoadInfo()
        {
            listBox1.Items.Clear();
            if (arc != "")
            {
                try
                {
                    Encoding shiftjis = Encoding.GetEncoding("shift-jis");
                    ObjectParameters op = new ObjectParameters("ScenarioArchiveName");
                    stageArc = new SMSScene(arc);
                    levels = new string[stageArc.Objects[0].Grouped[0].Grouped.Count][];
                    Strings = new GameObject[stageArc.Objects[0].Grouped[0].Grouped.Count][];
                    Tables = new GameObject[stageArc.Objects[0].Grouped[0].Grouped.Count];
                    for (int i = 0; i < stageArc.Objects[0].Grouped[0].Grouped.Count; i++)
                    {
                        Tables[i] = stageArc.Objects[0].Grouped[0].Grouped[i];
                        levels[i] = new string[Tables[i].Grouped.Count];
                        Strings[i] = new GameObject[Tables[i].Grouped.Count];
                        for (int j = 0; j < Tables[i].Grouped.Count; j++)
                        {
                            op.Adjust(Tables[i].Grouped[j]);
                            levels[i][j] = op.GetParamValue(0, Tables[i].Grouped[j]);
                            Strings[i][j] = Tables[i].Grouped[j];
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Failed to open StageArc.bin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            else
                this.Close();

            if (common != "")
            {
                BmgFile bmg;
                try
                {
                    bmg = new BmgFile(common + "\\2d\\stagename.bmg");
                    names = new string[bmg.Count];
                    for (int i = 0; i < bmg.Count; i++)
                        names[i] = bmg.GetString(i);
                    bmg.Close();
                }
                catch
                {
                    MessageBox.Show("Failed to open StageArc.bin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }

            for (int i = 0; i < levels.Length; i++)
            {
                string name = "";
                if (names != null)
                    if (i < names.Length)
                        name = names[i];
                if (name == "")
                    listBox1.Items.Add(i + ": " + levels[i][0]);
                else
                    listBox1.Items.Add(i + " (" + name + ") : " + levels[i][0]);
            }
        }

        private void WarpFinder_Load(object sender, EventArgs e)
        {
            LoadInfo();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ScenarioArchiveName");

            WarpEdit we = new WarpEdit();
            we.tableName = Tables[listBox1.SelectedIndex].Description;
            we.tableList = levels[listBox1.SelectedIndex];
            if (we.ShowDialog() != DialogResult.OK)
                return;

            levels[listBox1.SelectedIndex] = we.tableList;

            Tables[listBox1.SelectedIndex].Description = we.tableName;
            Tables[listBox1.SelectedIndex].DescHash = GCN.CreateHash(we.tableName);
            Tables[listBox1.SelectedIndex].Grouped.Clear();
            for (int i = 0; i < we.tableList.Length; i++)
            {
                GameObject go = new GameObject("ScenarioArchiveName", we.tableName + " " + i, 2);
                op.Adjust(go);
                op.SetParamValue(0, go, we.tableList[i]);
                Tables[listBox1.SelectedIndex].Grouped.Add(go);
                go.Parent = Tables[listBox1.SelectedIndex];
            }

            stageArc.Save(arc);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ScenarioArchiveName");

            WarpEdit we = new WarpEdit();
            we.tableName = "BinEditorGenerated" + Tables.Length;
            we.tableList = new string[0];
            if (we.ShowDialog() != DialogResult.OK)
                return;

            GameObject table = new GameObject("ScenarioArchiveNameTable", we.tableName, 0);
            stageArc.Objects[0].Grouped[0].Grouped.Add(table);
            table.Parent = stageArc.Objects[0].Grouped[0];

            table.Grouped = new List<GameObject>();
            for (int i = 0; i < we.tableList.Length; i++)
            {
                GameObject go = new GameObject("ScenarioArchiveName", we.tableName + " " + i, 2);
                op.Adjust(go);
                op.SetParamValue(0, go, we.tableList[i]);
                table.Grouped.Add(go);
                go.Parent = table;
            }

            stageArc.Save(arc);

            LoadInfo();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
                e.Cancel = true;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tables[listBox1.SelectedIndex].Parent.Grouped.Remove(Tables[listBox1.SelectedIndex]);
            Tables[listBox1.SelectedIndex].Parent = null;

            stageArc.Save(arc);

            LoadInfo();
        }
    }
}
