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
    public partial class paramEditor : Form
    {
        private ObjectParameters parameter;
        private bool editing = false;

        public string ObjectName;

        /* paramEditor */
        public paramEditor()
        {
            InitializeComponent();
        }

        /* Load parameter */
        public void LoadParameter()
        {
            parameter.ReadObjectParameters(ObjectName);
        }

        /* Save parameter and close */
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (paramList.SelectedIndex != -1)
                parameter.ComboInfo[paramList.SelectedIndex].Clear();
            //string[] segs = { };
            //int ptr = 0;
            parameter.SaveObjectParameters();
        }

        /* On Load */
        private void paramEditor_Load(object sender, EventArgs e)
        {
            groupBox1.Text = parameter.DisplayName;
            paramTitle.Text = parameter.DisplayName;

            UpdateListBox();
            paramType.Items.AddRange(Enum.GetNames(typeof(ParameterDataTypes)));    //Update types

            paramRemove.Enabled = false;
            paramName.Enabled = false;
            paramType.Enabled = false;
        }

        /* Populate list */
        private void UpdateListBox()
        {
            paramList.Items.Clear();
            foreach (string s in parameter.DataNames)
                paramList.Items.Add(s);
        }

        /* On parameter selection */
        private void paramList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (paramList.SelectedIndex == -1 || paramList.SelectedIndex > parameter.DataNames.Length - 1)
            {
                //Not valid
                paramRemove.Enabled = false;
                paramName.Enabled = false;
                paramType.Enabled = false;
                return;
            }

            if (editing == true)    //hack
                return;

            //Update info
            paramName.Text = parameter.DataNames[paramList.SelectedIndex];
            paramType.SelectedIndex = (int)parameter.DataTypes[paramList.SelectedIndex];

            commentBox.Enabled = true;
            if (parameter.DataTypes[paramList.SelectedIndex] == ParameterDataTypes.COMMENT)
            { //Update comment info
                commentBox.Text = parameter.GetParamValue(paramList.SelectedIndex, null);
            }
            else if (parameter.DataTypes[paramList.SelectedIndex] == ParameterDataTypes.BUFFER)
            { //Update custom type info
                commentBox.Text = parameter.GetParamLength(paramList.SelectedIndex, null).ToString();
            }
            else
            {
                if (parameter.CommentInfo.ContainsKey(paramList.SelectedIndex))
                    commentBox.Text = parameter.CommentInfo[paramList.SelectedIndex];
                else
                    commentBox.Text = "(null)";

            }

            paramRemove.Enabled = true;
            paramName.Enabled = true;
            paramType.Enabled = true;
        }

        /* On param name change */
        private void paramName_TextChanged(object sender, EventArgs e)
        {
            parameter.DataNames[paramList.SelectedIndex] = paramName.Text;

            editing = true;

            int sIndex = paramList.SelectedIndex;
            UpdateListBox();
            paramList.SelectedIndex = sIndex;

            editing = false;
        }

        /* On param type change */
        private void paramType_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameter.DataTypes[paramList.SelectedIndex] = (ParameterDataTypes)paramType.SelectedIndex;

            //Open new input for these types
            if ((ParameterDataTypes)paramType.SelectedIndex == ParameterDataTypes.COMMENT || (ParameterDataTypes)paramType.SelectedIndex == ParameterDataTypes.BUFFER)
                commentBox.Enabled = true;
            else
                commentBox.Enabled = false;

            if ((ParameterDataTypes)paramType.SelectedIndex == ParameterDataTypes.COMMENT)
            {
                if (!parameter.CommentInfo.ContainsKey(paramList.SelectedIndex))
                    parameter.CommentInfo.Add(paramList.SelectedIndex, "");
            }
            if ((ParameterDataTypes)paramType.SelectedIndex == ParameterDataTypes.BUFFER)
            {
                if (!parameter.CustomInfo.ContainsKey(paramList.SelectedIndex))
                    parameter.CustomInfo.Add(paramList.SelectedIndex, 0);
            }
        }

        /* On param add */
        private void paramAdd_Click(object sender, EventArgs e)
        {
            //build list of parameters
            List<string> names = new List<string>(parameter.DataNames);
            List<ParameterDataTypes> types = new List<ParameterDataTypes>(parameter.DataTypes);

            //Add new parameter
            names.Add("Unknown");
            types.Add(ParameterDataTypes.FLOAT);

            //Create new array using lists
            parameter.DataNames = names.ToArray();
            parameter.DataTypes = types.ToArray();

            for (int i = 0; i < names.Count; i++)
                if (!parameter.ComboInfo.ContainsKey(i)) parameter.ComboInfo.Add(i, new Dictionary<string, string>());

            if (paramList.SelectedIndex < 0 || paramList.SelectedIndex > parameter.DataNames.Length - 1)
            {
                paramRemove.Enabled = false;
                paramName.Enabled = false;
                paramType.Enabled = false;
            }
            else
            {
                paramName.Text = parameter.DataNames[paramList.SelectedIndex];
                paramType.SelectedIndex = (int)parameter.DataTypes[paramList.SelectedIndex];
            }

            UpdateListBox();
        }

        /* Delete parameter */
        private void paramRemove_Click(object sender, EventArgs e)
        {
            if (paramList.SelectedIndex < 0 || paramList.SelectedIndex > parameter.DataNames.Length - 1)
                return;

            //Build list
            List<string> names = new List<string>(parameter.DataNames);
            List<ParameterDataTypes> types = new List<ParameterDataTypes>(parameter.DataTypes);

            //Remove parameter
            names.RemoveAt(paramList.SelectedIndex);
            types.RemoveAt(paramList.SelectedIndex);

            //New array
            parameter.DataNames = names.ToArray();
            parameter.DataTypes = types.ToArray();

            if (paramList.SelectedIndex < 0 || paramList.SelectedIndex > parameter.DataNames.Length - 1)
            {
                paramRemove.Enabled = false;
                paramName.Enabled = false;
                paramType.Enabled = false;
            }
            else
            {
                paramName.Text = parameter.DataNames[paramList.SelectedIndex];
                paramType.SelectedIndex = (int)parameter.DataTypes[paramList.SelectedIndex];
            }

            UpdateListBox();
        }

        /* Title changed */
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            groupBox1.Text = paramTitle.Text;
            parameter.DisplayName = paramTitle.Text;
        }

        /* Comment/Custom changed */
        private void commentBox_TextChanged(object sender, EventArgs e)
        {
            if (parameter.CommentInfo.ContainsKey(paramList.SelectedIndex))
                parameter.CommentInfo[paramList.SelectedIndex] = commentBox.Text;
            else
                parameter.CommentInfo.Add(paramList.SelectedIndex, commentBox.Text);

            if (parameter.DataTypes[paramList.SelectedIndex] != ParameterDataTypes.COMMENT && parameter.DataTypes[paramList.SelectedIndex] != ParameterDataTypes.BUFFER && commentBox.Text == "")
                parameter.CommentInfo[paramList.SelectedIndex] = "(null)";

            editing = true;

            int sIndex = paramList.SelectedIndex;
            UpdateListBox();
            paramList.SelectedIndex = sIndex;

            editing = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
