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
using OpenTK;

namespace SMSSceneReader
{
    public partial class CameraDemoEditor : Form
    {
        const int MAXKFWIDTH = 500;
        const int MINKFWIDTH = 20;

        public BckFile demo = null;

        int KeyFrameWidth = 50;
        Dictionary<int, LookAtFromPair> CameraAnimation;//The int key is the time(frame) and the LookAtFromPair 
        Vector2 SelectedFrame = Vector2.Zero;



        public CameraDemoEditor()
        {
            InitializeComponent();
            Init();
        }

        private void Init()//Starts new timeline
        {
            CameraAnimation = new Dictionary<int, LookAtFromPair>();
            CameraAnimation.Add(0, new LookAtFromPair(Vector3.UnitX, Vector3.Zero));//Add initial key frame. This key frame must not be moved or deleted
            DurationUpDn.Value = 1;
            SelectedFrame = Vector2.Zero;
            RefreshTimeLine();
            SetControls();
        }

        private void RefreshTimeLine()//Forces a re-draw of the timeline. 
        {
            while (TimeLine.ColumnCount < (int)DurationUpDn.Value)
            {
                TimeLine.ColumnCount++;
                TimeLine.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, KeyFrameWidth));//Add any additional columns
            }

            while (TimeLine.ColumnCount > (int)DurationUpDn.Value)
            {
                TimeLine.ColumnCount--;
                TimeLine.ColumnStyles.RemoveAt(TimeLine.ColumnStyles.Count - 1);//remove last
            }

            TimeLine.Width = KeyFrameWidth * TimeLine.ColumnCount;
            for (int i = 0; i < TimeLine.ColumnStyles.Count; i++)
            {
                TimeLine.ColumnStyles[i].SizeType = SizeType.Percent;//Jank to get the things to paint. It won't paint unless something has changed.
                TimeLine.ColumnStyles[i].SizeType = SizeType.Absolute;
                TimeLine.ColumnStyles[i].Width = KeyFrameWidth;
            }
        }

        private void DurationUpDn_ValueChanged(object sender, EventArgs e)
        {
            RefreshTimeLine();
        }

        private void TimeLine_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            bool IsLookAt = e.Row == 0 ? true : false;
            bool IsSelected = e.Column == SelectedFrame.X && e.Row == SelectedFrame.Y;
            Brush MyBrush;
            if (CameraAnimation.ContainsKey(e.Column))
            {
                if (IsSelected)
                    MyBrush = IsLookAt ? Brushes.Salmon : Brushes.SlateBlue;
                else
                    MyBrush = IsLookAt ? Brushes.Crimson : Brushes.Navy;
            }
            else
            {
                if ((e.Column + e.Row) % 2 == 1)
                    MyBrush = IsSelected ? Brushes.Gray : Brushes.LightGray;
                else
                    MyBrush = IsSelected ? Brushes.Wheat : Brushes.White;
            }
            e.Graphics.FillRectangle(MyBrush, e.CellBounds);
            if (e.Row == 0)
            {
                // Create string to draw.
                string drawString = (e.Column).ToString();

                // Create font and brush.
                Font drawFont = new Font("Arial", 8);
                SolidBrush drawBrush = new SolidBrush(Color.Black);

                float x = 3f + (float)e.Column * KeyFrameWidth;//This position is relative to the timeline object not the individual cell
                float y = 3f;
                // Draw string to screen.
                e.Graphics.DrawString(drawString, drawFont, drawBrush, x, y);
            }
        }

        private void AddLookBtn_Click(object sender, EventArgs e)
        {
            KeyFramePrompt KFInput = new KeyFramePrompt();//This prompt is another form. Check that for reference.
            DialogResult MyDialog = KFInput.ShowDialog();//Show dialog means we must get a response before we can change anything on this form.
            if (MyDialog != DialogResult.OK || CameraAnimation.ContainsKey(KFInput.time) || KFInput.time > DurationUpDn.Value - 1)//Cancel if no KF is put in or KF time is invalid
                return;

            CameraAnimation.Add(KFInput.time, new LookAtFromPair(KFInput.AtPosition, KFInput.FromPosition));//Add the new KF
            RefreshTimeLine();//Force draw
        }

        private void TimeLine_MouseDown(object sender, MouseEventArgs e)
        {
            SelectedFrame.X = (int)Math.Floor((decimal)e.X / KeyFrameWidth);//e.X and e.Y are relative to the timeline. This just gets the time of the cell we clicked
            SelectedFrame.Y = (int)Math.Floor((decimal)e.Y * 2 / TimeLine.Height);//See if we clicked look at(0) or look from(1)
            RefreshTimeLine();
            SetControls();
        }

        private void SetControls()//This sets the controls to the value of the selected key frame
        {
            if (!CameraAnimation.ContainsKey((int)SelectedFrame.X))
            {
                KeyFrameTypeLbl.Text = "Select a key frame";
                SetControlsEnabled(false);//Disable the controls when no key frame is selected
                return;
            }
            else
                SetControlsEnabled(true);

            LookAtFromPair SelectedKF = CameraAnimation[(int)SelectedFrame.X];
            Vector3 SelectedPos;
            if (SelectedFrame.Y == 0)
            {
                SelectedPos = CameraAnimation[(int)SelectedFrame.X].LookAt;
                KeyFrameTypeLbl.Text = "'Look at' key frame";
            }
            else
            {
                SelectedPos = CameraAnimation[(int)SelectedFrame.X].LookFrom;
                KeyFrameTypeLbl.Text = "'Look from' key frame";
            }

            XUpDn.Value = (decimal)SelectedPos.X;
            YUpDn.Value = (decimal)SelectedPos.Y;
            ZUpDn.Value = (decimal)SelectedPos.Z;
            TimeUpDn.Value = (int)SelectedFrame.X;
        }

        private void SetControlsEnabled(bool Enabled)
        {
            TimeUpDn.Enabled = XUpDn.Enabled = YUpDn.Enabled = ZUpDn.Enabled = Enabled;
        }


        private void XUpDn_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedFrame.Y == 0)//Hopefully we don't need to check if the key exists because the control will be disabled
                CameraAnimation[(int)SelectedFrame.X].LookAt.X = (float)XUpDn.Value;
            else
                CameraAnimation[(int)SelectedFrame.X].LookFrom.X = (float)XUpDn.Value;
        }

        private void YUpDn_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedFrame.Y == 0)
                CameraAnimation[(int)SelectedFrame.X].LookAt.Y = (float)YUpDn.Value;
            else
                CameraAnimation[(int)SelectedFrame.X].LookFrom.Y = (float)YUpDn.Value;
        }

        private void ZUpDn_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedFrame.Y == 0)
                CameraAnimation[(int)SelectedFrame.X].LookAt.Z = (float)ZUpDn.Value;
            else
                CameraAnimation[(int)SelectedFrame.X].LookFrom.Z = (float)ZUpDn.Value;
        }

        private void TimeUpDn_ValueChanged(object sender, EventArgs e)
        {
            if (CameraAnimation.ContainsKey((int)TimeUpDn.Value) || SelectedFrame.X == 0 || (int)TimeUpDn.Value + 1 > DurationUpDn.Value)
            {
                TimeUpDn.Value = (int)SelectedFrame.X;
                return;
            }
            LookAtFromPair KFCopy = CameraAnimation[(int)SelectedFrame.X];//We can't change the key of an existing entry so we have to cut and paste to the new time
            CameraAnimation.Add((int)TimeUpDn.Value, KFCopy);
            CameraAnimation.Remove((int)SelectedFrame.X);
            SelectedFrame.X = (float)TimeUpDn.Value;
            RefreshTimeLine();
        }

        private void ZoomInBtn_Click(object sender, EventArgs e)
        {
            KeyFrameWidth = (int)Math.Min(MAXKFWIDTH, KeyFrameWidth * 1.25);
            RefreshTimeLine();
        }

        private void ZoomOutBtn_Click(object sender, EventArgs e)
        {
            KeyFrameWidth = (int)Math.Max(MINKFWIDTH, KeyFrameWidth * 0.75);
            RefreshTimeLine();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void RemoveLookBtn_Click(object sender, EventArgs e)
        {
            if (SelectedFrame.X == 0)
                return;
            if(CameraAnimation.ContainsKey((int)SelectedFrame.X))
                CameraAnimation.Remove((int)SelectedFrame.X);
            RefreshTimeLine();
            SetControls();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (demo == null)
                return;

            Init();
            CameraAnimation = new Dictionary<int, LookAtFromPair>();

            BckSection.BckANK1 sec = ((BckSection.BckANK1)demo.sections[0]);

            BckSection.BckANK1.Animation LookAtAnim = sec.GetJointAnimation(0);
            BckSection.BckANK1.Animation LookFromAnim = sec.GetJointAnimation(1);
            DurationUpDn.Value = (decimal)LookFromAnim.Duration + 1;
            List<float> PositionAtKeyFramesTimes = new List<float>();
            for (int KFidx = 0; KFidx < (int)Math.Max(LookAtAnim.x.Count, Math.Max(LookAtAnim.y.Count, LookAtAnim.z.Count)); KFidx++)
            {
                int XIdx = (int)Math.Min(KFidx, LookAtAnim.x.Count - 1);
                if (!PositionAtKeyFramesTimes.Contains(LookAtAnim.x[XIdx].time))
                    PositionAtKeyFramesTimes.Add(LookAtAnim.x[XIdx].time);

                int YIdx = (int)Math.Min(KFidx, LookAtAnim.y.Count - 1);
                if (!PositionAtKeyFramesTimes.Contains(LookAtAnim.y[YIdx].time))
                    PositionAtKeyFramesTimes.Add(LookAtAnim.y[YIdx].time);

                int ZIdx = (int)Math.Min(KFidx, LookAtAnim.z.Count - 1);
                if (!PositionAtKeyFramesTimes.Contains(LookAtAnim.z[ZIdx].time))
                    PositionAtKeyFramesTimes.Add(LookAtAnim.z[ZIdx].time);
            }

            PositionAtKeyFramesTimes.Sort();

            for (int i = 0; i < PositionAtKeyFramesTimes.Count; i++)//draw lines
            {
                Vector3 AtPosition = DataVectorToVector3(LookAtAnim.InterpolatePosition(PositionAtKeyFramesTimes[i], BckSection.BckANK1.Animation.InterpolationType.Linear));
                Vector3 FromPosition = (DataVectorToVector3(LookFromAnim.InterpolatePosition(PositionAtKeyFramesTimes[i], BckSection.BckANK1.Animation.InterpolationType.Linear)));

                Vector3 AtRot = DataVectorToVector3(LookAtAnim.InterpolateRotation(PositionAtKeyFramesTimes[i], BckSection.BckANK1.Animation.InterpolationType.Linear)) * (float)(Math.PI / 180f);

                Matrix3 Rx = new Matrix3(
                             new Vector3(1, 0, 0),
                             new Vector3(0, (float)Math.Cos(AtRot.X), (float)Math.Sin(AtRot.X)),
                             new Vector3(0, (float)-Math.Sin(AtRot.X), (float)Math.Cos(AtRot.X)));

                Matrix3 Ry = new Matrix3(
                             new Vector3((float)Math.Cos(AtRot.Y), 0, (float)Math.Sin(AtRot.Y)),
                             new Vector3(0, 1, 0),
                             new Vector3(-(float)Math.Sin(AtRot.Y), 0, (float)Math.Cos(AtRot.Y)));

                Matrix3 Rz = new Matrix3(
                             new Vector3((float)Math.Cos(AtRot.Z), (float)-Math.Sin(AtRot.Z), 0),
                             new Vector3((float)Math.Sin(AtRot.Z), (float)Math.Cos(AtRot.Z), 0),
                             new Vector3(0, 0, 1));

                FromPosition = AtPosition + Rx * Ry * Rz * FromPosition;

                CameraAnimation.Add((int)PositionAtKeyFramesTimes[i], new LookAtFromPair(AtPosition, FromPosition));
            }

        }

        private Vector3 DataVectorToVector3(DataReader.Vector v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }

    class LookAtFromPair
    {
        public Vector3 LookAt;
        public Vector3 LookFrom;

        public LookAtFromPair(Vector3 InitAt,Vector3 InitFrom)
        {
            LookFrom = InitFrom;
            LookAt = InitAt;
        }
    }
}
