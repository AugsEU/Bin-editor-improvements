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
    public partial class Settings : Form
    {
        bool restart = false;
        bool restart2 = false;
        bool Loading = false;

        /* Boring settings stuff */
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            Loading = true;
            FrontViewComboBox.DataSource = Enum.GetValues(typeof(Keys));
            RightViewComboBox.DataSource = Enum.GetValues(typeof(Keys));
            OrthographicViewComboBox.DataSource = Enum.GetValues(typeof(Keys));
            TopViewComboBox.DataSource = Enum.GetValues(typeof(Keys));

            MoveForwardComboBox.DataSource = Enum.GetValues(typeof(Keys));
            MoveBackwardComboBox.DataSource = Enum.GetValues(typeof(Keys));
            MoveLeftComboBox.DataSource = Enum.GetValues(typeof(Keys));
            MoveRightComboBox.DataSource = Enum.GetValues(typeof(Keys));
            MoveUpComboBox.DataSource = Enum.GetValues(typeof(Keys));
            MoveDownComboBox.DataSource = Enum.GetValues(typeof(Keys));
            StartDragComboBox.DataSource = Enum.GetValues(typeof(Keys));
            /* Main */
            startPreviewBox.Checked = Properties.Settings.Default.mainPreviewLoad;
            loadSecondBox.Checked = Properties.Settings.Default.mainLoadSeconds;
            shortcutBox.Checked = Properties.Settings.Default.mainShortcuts;
            paramComBox.Checked = !Properties.Settings.Default.showParamInfo;
            stageArcBox.Text = Properties.Settings.Default.stageArc;
            commonBox.Text = Properties.Settings.Default.commonPath;
            paramBox.Text = Properties.Settings.Default.paramPath;

            /* Preview */
            previewLocBox.Checked = Properties.Settings.Default.previewSave;
            if (Properties.Settings.Default.previewSize != Size.Empty && Properties.Settings.Default.previewState == FormWindowState.Normal)
                previewResetButton.Enabled = true;
            else
                previewResetButton.Enabled = false;
            
            foreach (string nm in Enum.GetNames(typeof(DrawModes)))
            {
                skyCombo.Items.Add(nm);
                worldCombo.Items.Add(nm);
                objectCombo.Items.Add(nm);
            }
            skyCombo.SelectedIndex = Properties.Settings.Default.skyDrawMode;
            worldCombo.SelectedIndex = Properties.Settings.Default.worldDrawMode;
            objectCombo.SelectedIndex = Properties.Settings.Default.objectDrawMode;

            TestGraphicLevel();

            objPicture.BackColor = Properties.Settings.Default.objColor;
            selObjPicture.BackColor = Properties.Settings.Default.objSelColor;
            railPicture.BackColor = Properties.Settings.Default.railColor;
            selRailPicture.BackColor = Properties.Settings.Default.railSelColor;
            nodePicture.BackColor = Properties.Settings.Default.railNodeColor;
            selNodePicture.BackColor = Properties.Settings.Default.railNodeSelColor;
            voidPicture.BackColor = Properties.Settings.Default.voidColor;

            numericUpDown1.Value = (decimal)Properties.Settings.Default.previewDrawDistance;
            numericUpDown2.Value = (decimal)Properties.Settings.Default.cameraSpeed;

            moveYBox.Checked = Properties.Settings.Default.cameraMoveY;
            originBox.Checked = Properties.Settings.Default.previewOrigin;

            /* Preview Keybinds*/
            FrontViewComboBox.SelectedItem = Properties.Settings.Default.KeyBindFrontView;
            RightViewComboBox.SelectedItem = Properties.Settings.Default.KeyBindRightView;
            OrthographicViewComboBox.SelectedItem = Properties.Settings.Default.KeyBindOrthoView;
            TopViewComboBox.SelectedItem = Properties.Settings.Default.KeyBindTopView;

            MoveForwardComboBox.SelectedItem = Properties.Settings.Default.KeyBindMoveForward;
            MoveBackwardComboBox.SelectedItem = Properties.Settings.Default.KeyBindMoveBackward;
            MoveLeftComboBox.SelectedItem = Properties.Settings.Default.KeyBindMoveLeft;
            MoveRightComboBox.SelectedItem = Properties.Settings.Default.KeyBindMoveRight;
            MoveUpComboBox.SelectedItem = Properties.Settings.Default.KeyBindMoveUp;
            MoveDownComboBox.SelectedItem = Properties.Settings.Default.KeyBindMoveDown;
            StartDragComboBox.SelectedItem = Properties.Settings.Default.KeyBindStartDrag;

            applyButton.Enabled = false;

            restart = false;
            restart2 = false;
            Loading = false;
        }

        private void previewResetButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.previewPosition = Point.Empty;
            Properties.Settings.Default.previewSize = Size.Empty;
            Properties.Settings.Default.previewState = FormWindowState.Normal;
            previewResetButton.Enabled = false;

            applyButton.Enabled = true;
        }

        private void TestGraphicLevel()
        {
            if (skyCombo.SelectedIndex == 0 && worldCombo.SelectedIndex == 1 && objectCombo.SelectedIndex == 1)
                renderBox.SelectedIndex = 0;
            else if (skyCombo.SelectedIndex == 3 && worldCombo.SelectedIndex == 1 && objectCombo.SelectedIndex == 1)
                renderBox.SelectedIndex = 1;
            else if (skyCombo.SelectedIndex == 3 && worldCombo.SelectedIndex == 3 && objectCombo.SelectedIndex == 1)
                renderBox.SelectedIndex = 2;
            else
                renderBox.SelectedIndex = 3;
        }

        private void renderBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (renderBox.SelectedIndex)
            {
                case 0:     //Low
                    skyCombo.SelectedIndex = 0;
                    worldCombo.SelectedIndex = 1;
                    objectCombo.SelectedIndex = 1;
                    break;
                case 1:     //Med
                    skyCombo.SelectedIndex = 3;
                    worldCombo.SelectedIndex = 1;
                    objectCombo.SelectedIndex = 1;
                    break;
                case 2:     //High
                    skyCombo.SelectedIndex = 3;
                    worldCombo.SelectedIndex = 3;
                    objectCombo.SelectedIndex = 1;
                    break;
                case 3:     //Custom
                    break;
            }
            applyButton.Enabled = true;
        }

        private void skyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.skyDrawMode = (byte)skyCombo.SelectedIndex;
            TestGraphicLevel();
            applyButton.Enabled = true;

            restart = true;
        }

        private void worldCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.worldDrawMode = (byte)worldCombo.SelectedIndex;
            TestGraphicLevel();
            applyButton.Enabled = true;

            restart = true;
        }

        private void objectCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.objectDrawMode = (byte)objectCombo.SelectedIndex;
            TestGraphicLevel();
            applyButton.Enabled = true;

            restart = true;
        }

        private void moveYBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.cameraMoveY = moveYBox.Checked;
            applyButton.Enabled = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.needsUpgrade = false;
            Properties.Settings.Default.Save();

            if (restart)
            {
                MessageBox.Show("One or more of the settings you changed require you to restart the preview for changes to take effect.");
                restart = false;
            }

            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.needsUpgrade = false;
            Properties.Settings.Default.Save();
            applyButton.Enabled = false;

            if (restart)
            {
                if (!MainForm.IsPreviewerOpen)
                {
                    restart = false;
                    return;
                }
                MessageBox.Show("One or more of the settings you changed require you to restart the preview for changes to take effect.","Notice",MessageBoxButtons.OK,MessageBoxIcon.Information);
                restart = false;
            }
            if (restart2)
            {
                MessageBox.Show("One or more of the settings you changed require you to restart the editor for changes to take effect.");
                restart2 = false;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            this.Close();
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Reload();
            this.Close();
        }

        private void previewDefaultButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will reset all preview settings to their default values. Would you like to continue?", "Default Settings", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            previewLocBox.Checked = Properties.Settings.Default.previewSave = true;
            Properties.Settings.Default.previewSize = Size.Empty;
            previewResetButton.Enabled = false;

            renderBox.SelectedIndex = 1;

            Properties.Settings.Default.objColor = objPicture.BackColor = Color.Blue;
            Properties.Settings.Default.objSelColor = selObjPicture.BackColor = Color.Red;
            Properties.Settings.Default.railColor = railPicture.BackColor = Color.Green;
            Properties.Settings.Default.railSelColor = selRailPicture.BackColor = Color.DarkRed;
            Properties.Settings.Default.railNodeColor = nodePicture.BackColor = Color.YellowGreen;
            Properties.Settings.Default.railNodeSelColor = selNodePicture.BackColor = Color.Red;
            Properties.Settings.Default.voidColor = voidPicture.BackColor = Color.FromArgb(0, 0, 32);

            numericUpDown1.Value = (decimal)(Properties.Settings.Default.previewDrawDistance = 500000f);
            numericUpDown2.Value = (decimal)(Properties.Settings.Default.cameraSpeed = 1f);

            moveYBox.Checked = Properties.Settings.Default.cameraMoveY = false;
            originBox.Checked = Properties.Settings.Default.previewOrigin = true;
            applyButton.Enabled = true;
        }

        private void objPicture_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = objPicture.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                objPicture.BackColor = Properties.Settings.Default.objColor = colorDialog1.Color;
                applyButton.Enabled = true;
            }
        }

        private void selObjPicture_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = selObjPicture.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                selObjPicture.BackColor = Properties.Settings.Default.objSelColor = colorDialog1.Color;
                applyButton.Enabled = true;
            }
        }

        private void railPicture_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = railPicture.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                railPicture.BackColor = Properties.Settings.Default.railColor = colorDialog1.Color;
                applyButton.Enabled = true;
            }
        }

        private void selRailPicture_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = selRailPicture.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                selRailPicture.BackColor = Properties.Settings.Default.railSelColor = colorDialog1.Color;
                applyButton.Enabled = true;
            }
        }

        private void nodePicture_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = nodePicture.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                nodePicture.BackColor = Properties.Settings.Default.railNodeColor = colorDialog1.Color;
                applyButton.Enabled = true;
            }
        }

        private void selNodePicture_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = selNodePicture.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                selNodePicture.BackColor = Properties.Settings.Default.railNodeSelColor = colorDialog1.Color;
                applyButton.Enabled = true;
            }
        }

        private void voidPicture_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = voidPicture.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                voidPicture.BackColor = Properties.Settings.Default.voidColor = colorDialog1.Color;
                applyButton.Enabled = true;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.previewDrawDistance = (float)numericUpDown1.Value;
            applyButton.Enabled = true;
        }

        private void mainDefaultButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will reset all main settings to their default values. Would you like to continue?", "Default Settings", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            Properties.Settings.Default.mainPreviewLoad = startPreviewBox.Checked = false;
            Properties.Settings.Default.mainLoadSeconds = loadSecondBox.Checked = true;
            Properties.Settings.Default.mainShortcuts = shortcutBox.Checked = true;
            paramComBox.Checked = !(Properties.Settings.Default.showParamInfo = false);
            Properties.Settings.Default.commonPath = commonBox.Text = "";
            Properties.Settings.Default.stageArc = stageArcBox.Text = "";
            Properties.Settings.Default.paramPath = paramBox.Text = "";
            restart2 = true;
        }

        private void startPreviewBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mainPreviewLoad = startPreviewBox.Checked;
            applyButton.Enabled = true;
        }

        private void loadSecondBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mainLoadSeconds = loadSecondBox.Checked;
            applyButton.Enabled = true;
        }

        private void shortcutBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mainShortcuts = shortcutBox.Checked;
            applyButton.Enabled = true;
        }

        private void originBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.previewOrigin = originBox.Checked;
            applyButton.Enabled = true;
        }

        private void stageArcBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            Properties.Settings.Default.stageArc = stageArcBox.Text = openFileDialog1.FileName;
            restart2 = true;
        }

        private void commonBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            Properties.Settings.Default.commonPath = commonBox.Text = folderBrowserDialog1.SelectedPath;
            restart2 = true;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.cameraSpeed = (float)numericUpDown2.Value;
            applyButton.Enabled = true;
        }

        private void paramBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            Properties.Settings.Default.paramPath = paramBox.Text = folderBrowserDialog1.SelectedPath;
            restart2 = true;
        }

        private void paramComBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.showParamInfo = !paramComBox.Checked;
        }

        private void FrontViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindFrontView = (Keys)FrontViewComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void RightViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindRightView = (Keys)RightViewComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void TopViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindTopView = (Keys)TopViewComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void OrthographicViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindOrthoView = (Keys)OrthographicViewComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void MoveForwardComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindMoveForward = (Keys)MoveForwardComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void MoveBackwardComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindMoveBackward = (Keys)MoveBackwardComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void MoveLeftComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindMoveLeft = (Keys)MoveLeftComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void MoveRightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindMoveRight = (Keys)MoveRightComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void MoveUpComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindMoveUp = (Keys)MoveUpComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void MoveDownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindMoveDown = (Keys)MoveDownComboBox.SelectedItem;
            applyButton.Enabled = true;
        }

        private void StartDragComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.KeyBindStartDrag = (Keys)StartDragComboBox.SelectedItem;
            applyButton.Enabled = true;
        }
    }
}
