using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using BMDReader;
using SMSReader;

namespace SMSSceneReader
{
    public enum DrawModes : byte
    {
        None = 0x00,
        Normal = 0x01,
        Effects = 0x02,
        Both = 0x03
    }

    public partial class Preview : Form
    {
        public MainForm mainForm;

        private const string MAP_PATH = "map\\map\\";    //Map model location
        private const string MAP_FILE = "map\\map\\map.bmd";    //Map model location
        private const string SKY_FILE = "map\\map\\sky.bmd";    //Map model location
        private const string SKYTEX_FILE = "map\\map\\sky.bmt";    //Map model location

        /* Camera variables */
        /* Camera and Rendering based off of camera in LevelEditorForm.cs in Whitehole by StapleButter */
        /* Changed to free camera 1/24/15 */
        private const float k_FOV = (float)((70f * Math.PI) / 180f);
        private const float k_zNear = 0.01f;
        private const float k_zFar = 1000f;
        private const float k_zNearOrtho = -1f;
        private const float k_zFarOrtho = 100f;
        private Matrix4 m_ProjMatrix;
        private float m_AspectRatio;
        private Vector3 CameraRotation;
        private Vector3 CameraPosition;
        private Vector3 CameraVelocity;
        private Matrix4 m_CamMatrix, m_SkyboxMatrix;
        private RenderInfo m_RenderInfo;
        private bool MouseLook = false;
        //private bool WireFrame;
        private bool Orthographic;
        private float OrthoZoom = 1;

        private System.Timers.Timer DisplayTimer = new System.Timers.Timer();

        /* Free look controls */
        private bool key_w = false;
        private bool key_a = false;
        private bool key_s = false;
        private bool key_d = false;
        private bool key_space = false;
        private bool key_shift = false;

        /* Axis stuff */
        bool LockKeyHeld;
        bool LockedAxis;
        Ray LastAxis;

        /* Mouse stuff */
        private bool DidMove;
        private Vector3 ClickRelMouse;
        private Vector3 ClickNormal;
        private Vector3 ClickPosition;
        private Vector3 ClickOrigin;

        private bool startedMoving = false;



        private Point FormCenter
        {
            get
            {
                return new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);
            }
        }

        private bool loaded;    //Whether or not the glControl is loaded

        private string SceneRoot;   //Root of scene

        private Dictionary<GameObject, SceneObject> SceneObjects = new Dictionary<GameObject, SceneObject>();   //All objects in the scene
        private RalFile rails = null;
        private Bmd camera = null;
        private BckFile demo = null;

        public bool ClickRail = false;
        public bool RenderRails = false;
        public bool RenderDemo = false;
        public int SelectedRail = -1;
        public int SelectedFrame = -1;

        public Vector3 CameraPos
        {
            get { return CameraPosition * 10000f; }
        }

        public Vector3 CameraOrigin
        {
            get { return CameraPosition * 10000f; }
        }

        int MapGList = 0;   //Map model list
        int SkyGList = 0;   //Map model list
        Bmd scene;  //Map model
        Bmd sky;    //Sky
        List<Bmd> otherModels;  //Others

        /* Debug */
        private Ray LastClick;
        bool Debug_ShowLastMouse = true;

        /* Preview */
        public Preview(string sceneRoot)
        {
            InitializeComponent();

            if (Properties.Settings.Default.previewSave)
            {
                if (Properties.Settings.Default.previewSize != Size.Empty)
                {
                    this.DesktopLocation = Properties.Settings.Default.previewPosition;
                    this.Size = Properties.Settings.Default.previewSize;
                }
                this.WindowState = Properties.Settings.Default.previewState;
            }

            //60 fps
            DisplayTimer.Interval = 16;
            DisplayTimer.Elapsed += DisplayTimer_Elapsed;

            SceneRoot = sceneRoot + "\\";

            this.Text = "Loading OpenGL...";
        }

        private void Preview_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Unload objects
            foreach (KeyValuePair<GameObject, SceneObject> so in SceneObjects)
                so.Value.UnLoadModel();
            SceneObjects.Clear();

            //Clear cache
            SceneObject.ClearCache();

            //Unload map model
            GL.DeleteLists(MapGList, 1);
            GL.DeleteLists(SkyGList, 1);

            //Unload THE CUBE and the flag
            SceneObject.DeleteCube();
            SceneObject.DeleteFlag();
        }

        /* Unload scene, then load new one */
        public void ChangeScene(string newScene) {
            SceneRoot = newScene + "\\";

            if (loaded)
            {
                this.Text = "Building Scene... (Environment)";

                GL.DeleteLists(MapGList, 1);
                GL.DeleteLists(SkyGList, 1);
                BuildScene();
            }

            //Unload old objects, new ones will be added
            foreach (KeyValuePair<GameObject, SceneObject> so in SceneObjects)
                so.Value.UnLoadModel();
            SceneObjects.Clear();
            SceneObject.ClearCache();
        }

        private void BuildScene()
        {
            FileBase fb = new FileBase();
            if (File.Exists(SceneRoot + MAP_FILE))
            {
                fb.Stream = new FileStream(SceneRoot + MAP_FILE, FileMode.Open);
                scene = new Bmd(fb);
                fb.Close();
            }
            else
                scene = null;
            if (File.Exists(SceneRoot + SKY_FILE))
            {
                fb.Stream = new FileStream(SceneRoot + SKY_FILE, FileMode.Open);
                sky = new Bmd(fb);
                fb.Close();
                if (File.Exists(SceneRoot + SKYTEX_FILE))
                {
                    fb.Stream = new FileStream(SceneRoot + SKYTEX_FILE, FileMode.Open);
                    sky.ReadBMT(fb);
                    fb.Close();
                }
            }
            else
                sky = null;

            otherModels = new List<Bmd>();
            //Pointless
            /*foreach (string file in Directory.GetFiles(SceneRoot + MAP_PATH))
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Name == "map.bmd" || fi.Name == "sky.bmd")
                    continue;
                if (fi.Extension == ".bmd")
                {
                    fb.Stream = new FileStream(SceneRoot + SKY_FILE, FileMode.Open);
                    otherModels.Add(new Bmd(fb));
                    fb.Close();
                }
            }*/

            MapGList = GL.GenLists(1);
            SkyGList = GL.GenLists(1);

            GL.NewList(SkyGList, ListMode.Compile);
            if (sky != null)
            {
                if ((Properties.Settings.Default.skyDrawMode | 0x01) == Properties.Settings.Default.skyDrawMode && !Orthographic) 
                    DrawBMD(sky);
                if ((Properties.Settings.Default.skyDrawMode | 0x02) == Properties.Settings.Default.skyDrawMode && !Orthographic) 
                    DrawBMD(sky, RenderMode.Translucent);
            }
            GL.EndList();

            GL.NewList(MapGList, ListMode.Compile);
            if (scene != null)
            {
                if ((Properties.Settings.Default.worldDrawMode | 0x01) == Properties.Settings.Default.worldDrawMode)
                    DrawBMD(scene);
                if ((Properties.Settings.Default.worldDrawMode | 0x02) == Properties.Settings.Default.worldDrawMode)
                    DrawBMD(scene, RenderMode.Translucent);
            }
            GL.EndList();
        }

        /* Update object information */
        public void UpdateObject(GameObject go)
        {
            this.Text = "Building Scene... (" + go.Name + ")";

            SceneObject.UpdateParameter(go);
            if (SceneObjects.ContainsKey(go))
            {
                //Update object
                SceneObject cur = SceneObjects[go];
                cur.Update();
                if (!cur.CanBeDrawn)
                {
                    //Object has no drawing information, forget about it
                    SceneObjects[go].UnLoadModel();
                    SceneObjects.Remove(go);
                }
                else
                    SceneObjects[go] = cur;
            }
            else
            {
                //Scene does not have object, add it
                SceneObject so = new SceneObject(go, SceneRoot);
                if (so.CanBeDrawn)
                    SceneObjects.Add(go, so);
            }
            glControl1.Invalidate();

            this.Text = "Scene Preview";
        }
        public void UpdateAllObjects()
        {
            SceneObjects.Clear();
            foreach (GameObject go in mainForm.LoadedScene.AllObjects)
                UpdateObject(go);
        }

        /* Update object model */
        public void UpdateObjectModel(GameObject go)
        {
            this.Text = "Building Scene... (" + go.Name + ")";
            SceneObject.UpdateParameter(go);
            if (SceneObjects.ContainsKey(go))
            {
                //Update all models
                foreach (KeyValuePair<GameObject, SceneObject> kvp in SceneObjects)
                {
                    if (kvp.Key.Name == go.Name)
                    {
                        //Update object
                        SceneObject cur = kvp.Value;
                        cur.UnLoadModel();
                        cur.GenerateModel(SceneRoot);
                    }
                }
            }
            glControl1.Invalidate();

            this.Text = "Scene Preview";
        }

        /* Remove object */
        public void RemoveObject(GameObject go)
        {
            this.Text = "Building Scene... (" + go.Name + ")";
            if (SceneObjects.ContainsKey(go))
            {
                SceneObjects[go].UnLoadModel();
                SceneObjects.Remove(go);
            }

            glControl1.Refresh();
            this.Text = "Scene Preview";
        }

        /* Select an object */
        public void SelectObject(GameObject go)
        {
            //Find targeted objects
            foreach (KeyValuePair<GameObject, SceneObject> so in SceneObjects)
            {
                if (so.Key == go)
                {
                    so.Value.Select();
                }
                else if (so.Value.Selected == true)
                    so.Value.Deselect();
            }

            UpdateCamera();
            glControl1.Refresh();
        }

        /* Returns all selected objects */
        public GameObject[] GetSelectedObjects()
        {
            List<GameObject> selected = new List<GameObject>();
            //Find targeted objects
            foreach (KeyValuePair<GameObject, SceneObject> so in SceneObjects)
                if (so.Value.Selected)
                    selected.Add(so.Key);

            return selected.ToArray();
        }

        /* Camera and Rendering based off of camera in LevelEditorForm.cs in Whitehole by StapleButter */
        /* 1/21/15 */
        private void UpdateViewport()
        {
            GL.Viewport(glControl1.ClientRectangle);

            m_AspectRatio = (float)glControl1.Width / (float)glControl1.Height;
            GL.MatrixMode(MatrixMode.Projection);
            if (!Orthographic)
                m_ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(k_FOV, m_AspectRatio, k_zNear, k_zFar);
            else
                m_ProjMatrix = Matrix4.CreateOrthographic(0.1f * (float)OrthoZoom * m_AspectRatio, 0.1f * (float)OrthoZoom, k_zNearOrtho, k_zFarOrtho);
            GL.LoadMatrix(ref m_ProjMatrix);
        }
        private void UpdateCamera()
        {
            Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

            Vector3 CameraUnitVector = new Vector3((float)(Math.Cos(CameraRotation.X) * Math.Cos(CameraRotation.Y)) + (CameraPosition.X),
                                                   (float)(Math.Sin(CameraRotation.Y) + (CameraPosition.Y)),
                                                   (float)(Math.Sin(CameraRotation.X) * Math.Cos(CameraRotation.Y)) + (CameraPosition.Z));

            Vector3 skybox_target;
            skybox_target.X = -CameraUnitVector.X;
            skybox_target.Y = -CameraUnitVector.Y;
            skybox_target.Z = -CameraUnitVector.Z;

            m_CamMatrix = Matrix4.LookAt(CameraPosition, CameraUnitVector, up);
            m_SkyboxMatrix = Matrix4.LookAt(Vector3.Zero, skybox_target, up);

            m_CamMatrix = Matrix4.Mult(Matrix4.CreateScale(0.0001f), m_CamMatrix);
        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;

            glControl1.MakeCurrent();

            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1f);

            GL.FrontFace(FrontFaceDirection.Cw);

            CameraPosition = new Vector3(0f, 0f, 0f);
            CameraRotation = new Vector3(0.0f, 0.0f, 0.0f);

            m_RenderInfo = new RenderInfo();

            UpdateViewport();
            UpdateCamera();

            GL.ClearColor(Properties.Settings.Default.voidColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            this.Text = "Building Scene... (Environment)";

            BuildScene();

            SceneObject.BuildCube();
            SceneObject.BuildFlag();

            loaded = true;
        }
        private void glControl1_Paint(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            glControl1.MakeCurrent();

            GL.DepthMask(true); // ensures that GL.Clear() will successfully clear the buffers

            GL.ClearColor(0f, 0f, 0.125f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m_CamMatrix);

            GL.Disable(EnableCap.Texture2D);

            GL.CallList(SkyGList);

            GL.CallList(MapGList);

            if (Properties.Settings.Default.previewOrigin)
            {
                Vector3 originPosition = Vector3.Zero;
                GL.LineWidth(1);
                GL.Begin(PrimitiveType.Lines);
                GL.Color4(1f, 0f, 0f, 1f);
                GL.Vertex3(originPosition);
                GL.Vertex3(originPosition + new Vector3(100000f, 0f, 0f));
                GL.Color4(0f, 1f, 0f, 1f);
                GL.Vertex3(originPosition);
                GL.Vertex3(originPosition + new Vector3(0f, 100000f, 0f));
                GL.Color4(0f, 0f, 1f, 1f);
                GL.Vertex3(originPosition);
                GL.Vertex3(originPosition + new Vector3(0f, 0f, 100000f));

                if (Debug_ShowLastMouse && LastClick != null)
                {
                    GL.Color4(1f, 0.5f, 0f, 1f);
                    GL.Vertex3(LastClick.Origin);
                    GL.Vertex3(LastClick.Origin + 100000f * LastClick.Direction);
                }

                GL.End();
            }

            if (RenderRails)
                DrawRails();
            if (RenderDemo)
                DrawDemo();

            GL.LineWidth(1);

            foreach (KeyValuePair<GameObject, SceneObject> so in SceneObjects)
                if ((so.Value.ObjPosition - (CameraPosition * 10000f)).LengthFast < Properties.Settings.Default.previewDrawDistance)
                    so.Value.Draw();

            GL.Color4(1f, 1f, 1f, 1f);

            glControl1.SwapBuffers();

            label1.Text = "pos: (" + CameraPos.X + ", " + CameraPos.Y + ", " + CameraPos.Z + ") rot: (" + (CameraRotation.X / 2.0f) + ", " + (CameraRotation.Y / 2.0f) + ")";
            label1.Refresh();
        }
        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();

            UpdateViewport();
        }

        private static Vector3 RotateAroundX(Vector3 vector, float angle)
        {
            float s = (float)Math.Sin(angle * Math.PI / 180f);
            float c = (float)Math.Cos(angle * Math.PI / 180f);
            Vector3 n = Vector3.Zero;
            n.Y = vector.Y * c - vector.Z * s;
            n.Z = vector.Y * s + vector.Z * c;
            n.X = vector.X;
            return n;
        }
        private static Vector3 RotateAroundY(Vector3 vector, float angle)
        {
            float s = (float)Math.Sin(-angle * Math.PI / 180f);
            float c = (float)Math.Cos(-angle * Math.PI / 180f);
            Vector3 n = Vector3.Zero;
            n.X = vector.X * c - vector.Z * s;
            n.Z = vector.X * s + vector.Z * c;
            n.Y = vector.Y;
            return n;
        }
        private static Vector3 RotateAroundZ(Vector3 vector, float angle)
        {
            float s = (float)Math.Sin(angle * Math.PI / 180f);
            float c = (float)Math.Cos(angle * Math.PI / 180f);
            Vector3 n = Vector3.Zero;
            n.X = vector.X * c - vector.Y * s;
            n.Y = vector.X * s + vector.Y * c;
            n.Z = vector.Z;
            return n;
        }

        private Ray ScreenToRay(Point mousePos)
        {
            //Create camera
            Matrix4 projmtx = m_ProjMatrix;
            Matrix4 viewmtx = m_CamMatrix;

            //Get Normalized mouse position
            Vector3 normalizedmouse = new Vector3((2.0f * mousePos.X) / glControl1.Width - 1.0f, -((2.0f * mousePos.Y) / glControl1.Height - 1.0f), -1.0f);
            Vector3 origin;
            Vector3 dir;
            if (!Orthographic)
            {
                Vector4 clip = new Vector4(normalizedmouse.X, normalizedmouse.Y, -1.0f, 1.0f);

                //Unproject mouse position
                Vector4 eye = (clip * projmtx.Inverted());
                eye.Z = -1.0f;
                eye.W = 0.0f;

                //Convert to direction
                dir = (eye * viewmtx.Inverted()).Xyz;
                dir.Normalize();

                origin = CameraPos;
                
            }
            else
            {
                Vector3 CameraUnitVector = new Vector3((float)(Math.Cos(CameraRotation.X) * Math.Cos(CameraRotation.Y)),
                                                       (float)Math.Sin(CameraRotation.Y),
                                                       (float)(Math.Sin(CameraRotation.X) * Math.Cos(CameraRotation.Y)));//Unit vector in camera direction

                Vector3 scaledmouse = normalizedmouse * OrthoZoom * new Vector3(glControl1.Width, glControl1.Height, 0f) / 2f;
                Vector3 ScreenXBasis = new Vector3((float)(-Math.Sin(CameraRotation.X))
                                                       ,0f
                                                       ,(float)(Math.Cos(CameraRotation.X)));//Basis vector on the viewport plane. This one is flat on the y axis.
                Vector3 ScreenYBasis = Vector3.Cross(ScreenXBasis, CameraUnitVector);
                Vector3 ApparentCameraPos = CameraPos - 10000f * CameraUnitVector;
                origin = scaledmouse.X* ScreenXBasis*1.415f + scaledmouse.Y*ScreenYBasis*1.415f + ApparentCameraPos;
                dir = CameraUnitVector;

            }
            //LastClick = new Ray(origin, dir);//Debug
            return new Ray(origin, dir);
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            DidMove = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                MouseLook = true;
                Cursor.Position = FormCenter;
                DisplayTimer.Start();

                Cursor.Hide();
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ClickRail = false;

                Ray r = ScreenToRay(glControl1.PointToClient(Cursor.Position));

                Vector3 center = new Vector3(glControl1.Width / 2f, glControl1.Height / 2f, 0);
                GameObject currentObject = null;
                float currentDepth = float.MaxValue;
                foreach (KeyValuePair<GameObject, SceneObject> kvp in SceneObjects)
                {
                    //Check bounds
                    for (int i = 0; i < 6; i++)
                    {
                        Vector3 normal = new Vector3(i / 2 == 0 ? 1 : 0,    //Loops through normals of an unrotated cube
                                                     i / 2 == 1 ? 1 : 0,
                                                     i / 2 == 2 ? 1 : 0)
                                         * (i % 2 == 0 ? 1 : -1);           //Both sides

                        //Gets the center of the bounds with that normal
                        Vector3 pcenter = new Vector3(normal.X == 0 ? (kvp.Value.RealObjBBoxMin.X + kvp.Value.RealObjBBoxMax.X) / 2f : (normal.X < 0 ? kvp.Value.RealObjBBoxMin.X : kvp.Value.RealObjBBoxMax.X),
                                                      normal.Y == 0 ? (kvp.Value.RealObjBBoxMin.Y + kvp.Value.RealObjBBoxMax.Y) / 2f : (normal.Y < 0 ? kvp.Value.RealObjBBoxMin.Y : kvp.Value.RealObjBBoxMax.Y),
                                                      normal.Z == 0 ? (kvp.Value.RealObjBBoxMin.Z + kvp.Value.RealObjBBoxMax.Z) / 2f : (normal.Z < 0 ? kvp.Value.RealObjBBoxMin.Z : kvp.Value.RealObjBBoxMax.Z));

                        //Rotate bounds around object center
                        pcenter -= kvp.Value.ObjPosition;
                        pcenter = RotateAroundX(pcenter, kvp.Value.ObjRotation.X);
                        pcenter = RotateAroundY(pcenter, kvp.Value.ObjRotation.Y);
                        pcenter = RotateAroundZ(pcenter, kvp.Value.ObjRotation.Z);
                        pcenter += kvp.Value.ObjPosition;

                        //Rotate plane
                        normal = RotateAroundX(normal, kvp.Value.ObjRotation.X);
                        normal = RotateAroundY(normal, kvp.Value.ObjRotation.Y);
                        normal = RotateAroundZ(normal, kvp.Value.ObjRotation.Z);

                        float num = Vector3.Dot(pcenter - r.Origin, normal);
                        float den = Vector3.Dot(r.Direction, normal);
                        float t = num / den;    //Distance from camera

                        if (t > 0 && t < currentDepth)
                        {
                            Vector3 point = r.Origin + (r.Direction * t);

                            //Rotate collision point to angle where bounds are parallel to axes
                            point -= kvp.Value.ObjPosition;
                            point = RotateAroundZ(point, -kvp.Value.ObjRotation.Z);
                            point = RotateAroundY(point, -kvp.Value.ObjRotation.Y);
                            point = RotateAroundX(point, -kvp.Value.ObjRotation.X);
                            point += kvp.Value.ObjPosition;

                            //Check if point is within the bounds
                            if (point.X <= kvp.Value.RealObjBBoxMax.X && point.X >= kvp.Value.RealObjBBoxMin.X &&
                                point.Y <= kvp.Value.RealObjBBoxMax.Y && point.Y >= kvp.Value.RealObjBBoxMin.Y &&
                                point.Z <= kvp.Value.RealObjBBoxMax.Z && point.Z >= kvp.Value.RealObjBBoxMin.Z)
                            {
                                currentDepth = t;
                                currentObject = kvp.Key;
                                ClickNormal = normal;
                                ClickPosition = pcenter;
                                ClickRelMouse = kvp.Value.ObjPosition - point;
                                ClickOrigin = kvp.Value.ObjPosition;
                            }
                        }
                    }
                }
                if (RenderRails)
                {
                    for (int i = 0; i < rails.Count; i++)
                    {
                        for (int j = 0; j < rails.GetRail(i).frames.Length; j++)
                        {

                            KeyFrame kf = rails.GetRail(i).frames[j];

                            //Check bounds
                            for (int f = 0; f < 6; f++)
                            {
                                Vector3 normal = new Vector3(f / 2 == 0 ? f : 0,    //Loops through normals of an unrotated cube
                                                             f / 2 == 1 ? 1 : 0,
                                                             f / 2 == 2 ? 1 : 0)
                                                 * (f % 2 == 0 ? 1 : -1);           //Both sides

                                //Gets the center of the bounds with that normal
                                Vector3 pcenter = new Vector3(kf.x + (normal.X == 0 ? 0 : (normal.X < 0 ? -64 : 64)),
                                                              kf.y + (normal.Y == 0 ? 0 : (normal.Y < 0 ? -64 : 64)),
                                                              kf.z + (normal.Z == 0 ? 0 : (normal.Z < 0 ? -64 : 64)));

                                pcenter -= new Vector3(kf.x, kf.y, kf.z);
                                pcenter = RotateAroundX(pcenter, kf.pitch);
                                pcenter = RotateAroundY(pcenter, kf.yaw);
                                pcenter = RotateAroundZ(pcenter, kf.roll);
                                pcenter += new Vector3(kf.x, kf.y, kf.z);

                                normal = RotateAroundX(normal, kf.pitch);
                                normal = RotateAroundY(normal, kf.yaw);
                                normal = RotateAroundZ(normal, kf.roll);

                                float num = Vector3.Dot(pcenter - r.Origin, normal);
                                float den = Vector3.Dot(r.Direction, normal);
                                float t = num / den;    //Distance from camera

                                if (t > 0 && t < currentDepth)
                                {
                                    Vector3 point = r.Origin + (r.Direction * t);
                                    Vector3 opoint = point;

                                    point -= new Vector3(kf.x, kf.y, kf.z);
                                    point = RotateAroundZ(point, -kf.pitch);
                                    point = RotateAroundY(point, -kf.yaw);
                                    point = RotateAroundX(point, -kf.roll);
                                    point += new Vector3(kf.x, kf.y, kf.z);

                                    if (point.X <= kf.x + 64 && point.X >= kf.x - 64 &&
                                        point.Y <= kf.y + 64 && point.Y >= kf.y - 64 &&
                                        point.Z <= kf.z + 64 && point.Z >= kf.z - 64)
                                    {
                                        currentDepth = t;
                                        currentObject = null;
                                        ClickRail = true;
                                        SelectedRail = i;
                                        SelectedFrame = j;
                                        ClickNormal = normal;
                                        ClickPosition = pcenter;
                                        ClickRelMouse = new Vector3(kf.x, kf.y, kf.z) - opoint;
                                        ClickOrigin = new Vector3(kf.x, kf.y, kf.z);
                                    }
                                }
                            }
                        }
                    }
                }
                if (ClickRail)
                    mainForm.UpdateRailForm(SelectedRail, SelectedFrame);
                if (currentObject != null)
                {
                    SelectObject(currentObject);
                    mainForm.GoToObject(currentObject);
                }
                else
                    SelectObject(null);

                UpdateCamera();
                UpdateViewport();
                glControl1.Refresh();
            }
        }
        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && MouseLook)
            {
                DisplayTimer.Stop();
                MouseLook = false;
                CameraVelocity = new Vector3(0f, 0f, 0f);

                key_w = false;
                key_s = false;
                key_a = false;
                key_d = false;
                key_space = false;
                key_shift = false;

                Cursor.Show();
            }

            LockedAxis = false;

            if (DidMove)
            { 
               mainForm.UpdateObjectInfo();
               startedMoving = false;
            }
        }
        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseLook)
            {
                //Get change in direction using change in mouse position.
                Point center = FormCenter;
                int dx = Cursor.Position.X - center.X;
                int dy = Cursor.Position.Y - center.Y;
                Cursor.Position = center;   //Lock mouse to center of screen

                //Rotate camera
                CameraRotation.X += dx * 0.01f;
                CameraRotation.Y -= dy * 0.01f;

                //Keep direction from overflowing
                float pi2 = 2f * (float)Math.PI - 0.001f;
                float pii2 = (float)Math.PI / 2f - 0.001f;
                while (CameraRotation.X > pi2)
                    CameraRotation.X -= pi2;
                while (CameraRotation.X < -pi2)
                    CameraRotation.X += pi2;

                //Keep camera from going upside down
                if (CameraRotation.Y > pii2)
                    CameraRotation.Y = pii2;
                if (CameraRotation.Y < -pii2)
                    CameraRotation.Y = -pii2;

                UpdateCamera();
                glControl1.Refresh();
            }
            else if (e.Button == MouseButtons.Left)//Drag
            {
                if (!Orthographic)
                {
                    Ray r = ScreenToRay(glControl1.PointToClient(Cursor.Position));

                    float num = Vector3.Dot(ClickPosition - CameraPos, ClickNormal);
                    float den = Vector3.Dot(r.Direction, ClickNormal);
                    float t = num / den;    //Distance from camera

                    Vector3 newpoint = CameraPos + (r.Direction * t);

                    if (LockKeyHeld || LockedAxis)
                    {
                        Vector3 nY;
                        Vector3 nX;
                        float angle;
                        Vector3 axis;

                        if (ClickNormal == new Vector3(0, 0, 1))
                        {
                            angle = 0;
                            axis = new Vector3(1, 0, 0);
                        }
                        else
                        {
                            angle = Vector3.Dot(new Vector3(0, 0, 1), ClickNormal);
                            axis = Vector3.Cross(new Vector3(0, 0, 1), ClickNormal).Normalized();
                        }

                        if (angle != 0)
                        {
                            float c = angle;
                            float s = (float)Math.Sqrt(1 - Math.Pow(angle, 2));
                            float C = 1 - angle;
                            float x = axis.X;
                            float y = axis.Y;
                            float z = axis.Z;

                            Matrix3 rotmtx = new Matrix3(
                                new Vector3(x * x * C + c, x * y * C - z * s, x * z * C + y * s),
                                new Vector3(y * x * C + z * s, y * y * C + c, y * z * C - x * s),
                                new Vector3(z * x * C - y * s, z * y * C + x * s, z * z * C + c));

                            nX = rotmtx * new Vector3(1, 0, 0);
                            nY = rotmtx * new Vector3(0, 1, 0);
                        }
                        else
                        {
                            nX = new Vector3(1, 0, 0);
                            nY = new Vector3(0, 1, 0);
                        }

                        Vector3 pdif = (newpoint + ClickRelMouse) - ClickOrigin;
                        float xdif = Vector3.Dot(pdif, nX);
                        float ydif = Vector3.Dot(pdif, nY);

                        if (LockedAxis)
                            newpoint = (pdif * LastAxis.Direction) + LastAxis.Origin;
                        else
                        {
                            if (xdif > ydif)
                                LastAxis = new Ray(ClickOrigin, nX);
                            else
                                LastAxis = new Ray(ClickOrigin, nY);

                            LockedAxis = true;
                        }
                    }
                    if (!ClickRail)
                    {
                        if (!startedMoving)
                        {
                            startedMoving = true;
                            mainForm.CreateUndoSnapshot();
                            mainForm.Changed = true;
                        }

                        GameObject[] selected = GetSelectedObjects();
                        //Update all selected objects
                        foreach (GameObject go in selected)
                        {
                            ObjectParameters op = new ObjectParameters();
                            op.ReadObjectParameters(go);
                            Vector3 n = newpoint + ClickRelMouse;
                            op.SetParamValue("X", go, n.X.ToString());
                            op.SetParamValue("Y", go, n.Y.ToString());
                            op.SetParamValue("Z", go, n.Z.ToString());

                            UpdateObject(go);

                            DidMove = true;
                        }
                    }
                    else
                    {
                        rails.GetRail(SelectedRail).frames[SelectedFrame].x = (short)(ClickOrigin.X + (short)ClickRelMouse.X);
                        rails.GetRail(SelectedRail).frames[SelectedFrame].y = (short)(ClickOrigin.Y + (short)ClickRelMouse.Y);
                        rails.GetRail(SelectedRail).frames[SelectedFrame].z = (short)(ClickOrigin.Z + (short)ClickRelMouse.Z);
                        mainForm.UpdateRailForm(SelectedRail, SelectedFrame);
                    }
                }
                else
                {
                    Ray ClickLine = ScreenToRay(glControl1.PointToClient(Cursor.Position));
                    Vector3 CameraUnitVector = new Vector3((float)(Math.Cos(CameraRotation.X) * Math.Cos(CameraRotation.Y)),
                                                           (float)Math.Sin(CameraRotation.Y),
                                                           (float)(Math.Sin(CameraRotation.X) * Math.Cos(CameraRotation.Y)));//Unit vector in camera direction. Also normal vector to plane of movement.
                    GameObject[] selected = GetSelectedObjects();
                    if(selected.Length != 0)
                    {
                        ObjectParameters op = new ObjectParameters();
                        op.ReadObjectParameters(selected[0]);

                        float x = 0f;
                        float y = 0f;
                        float z = 0f;

                        float.TryParse(op.GetParamValue("X", selected[0]), out x);
                        float.TryParse(op.GetParamValue("Y", selected[0]), out y);
                        float.TryParse(op.GetParamValue("Z", selected[0]), out z);

                        Vector3 ObjectPosition = new Vector3(x,y,z);
                        float t = Vector3.Dot(CameraUnitVector, ObjectPosition - ClickLine.Origin) / Vector3.Dot(CameraUnitVector, ClickLine.Direction);//Parameter for our line. This is in the form r=u+t*v where r,u, and v are vectors. and t is the scalar. This formula is from solving the vector eqn.
                        Vector3 OutputPosition = ClickLine.Origin + t * ClickLine.Direction;
                        if (!startedMoving)
                        {
                            startedMoving = true;
                            mainForm.CreateUndoSnapshot();
                            mainForm.Changed = true;
                        }
                        op.SetParamValue("X", selected[0], OutputPosition.X.ToString());
                        op.SetParamValue("Y", selected[0], OutputPosition.Y.ToString());
                        op.SetParamValue("Z", selected[0], OutputPosition.Z.ToString());
                        UpdateObject(selected[0]);
                        DidMove = true;//dodgy
                    }
                    
                }

                glControl1.Invalidate();
            }
        }

        /* Mousewheel zooms in and out */
        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Orthographic)
            {
                //Get change in mousewheel
                float delta = e.Delta / 4800f;

                //Get forward facing direction
                Vector3 f = new Vector3((float)(Math.Cos(CameraRotation.X) * (float)Math.Cos(CameraRotation.Y)),
                                        (float)Math.Sin(CameraRotation.Y),
                                        (float)(Math.Sin(CameraRotation.X) * (float)Math.Cos(CameraRotation.Y)));

                //Move camera
                CameraPosition += f * delta;
            }
            else
            {
                
                OrthoZoom *= (float)Math.Pow((double)1.1,Convert.ToDouble(-e.Delta/100));//To do: scale zoom according to distance.
                if (OrthoZoom < 0.025)
                    OrthoZoom = 0.025f;
                UpdateViewport();
            }
            

            //Update screen
            UpdateCamera();
            glControl1.Refresh();
        }

        /* Free look controls */
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (MouseLook)
            {
                if (e.KeyCode == Keys.W)
                    key_w = true;
                if (e.KeyCode == Keys.S)
                    key_s = true;
                if (e.KeyCode == Keys.A)
                    key_a = true;
                if (e.KeyCode == Keys.D)
                    key_d = true;
                if (e.KeyCode == Keys.Space)
                    key_space = true;
                if (e.KeyCode == Keys.ShiftKey)
                    key_shift = true;
                LockKeyHeld = false;
            }
            else
            {
                if (e.KeyCode == Keys.ShiftKey)
                    LockKeyHeld = true;
            }
            //View angle shortcuts
            if (e.KeyCode == Keys.NumPad1)//Front view
            {
                CameraRotation.X = (float)Math.PI / 2;
                CameraRotation.Y = 0f;
                CameraPosition = new Vector3(0f, 0f, -1f);
                UpdateCamera();
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.NumPad3)//Right view
            {
                CameraRotation.X = (float)Math.PI;
                CameraRotation.Y = 0f;
                CameraPosition = new Vector3(1f, 0f, 0f);
                UpdateCamera();
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.NumPad5)
            {
                Orthographic = !Orthographic;
                UpdateViewport();
                UpdateCamera();
                glControl1.Refresh();
            }
            if (e.KeyCode == Keys.NumPad7)//Top view
            {
                CameraRotation.X = 0f;
                CameraRotation.Y = (float)-Math.PI / 2;
                CameraPosition = new Vector3(0f, 1f, 0f);
                UpdateCamera();
                glControl1.Invalidate();
            }
        }
        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (MouseLook)
            {
                if (e.KeyCode == Keys.W)
                    key_w = false;
                if (e.KeyCode == Keys.S)
                    key_s = false;
                if (e.KeyCode == Keys.A)
                    key_a = false;
                if (e.KeyCode == Keys.D)
                    key_d = false;
                if (e.KeyCode == Keys.Space)
                    key_space = false;
                if (e.KeyCode == Keys.ShiftKey)
                    key_shift = false;
                LockKeyHeld = false;
            }
            else
            {
                if (e.KeyCode == Keys.ShiftKey)
                    LockKeyHeld = false;
            }
        }

        /* Free look update timer */
        void DisplayTimer_Elapsed(object sender, EventArgs e)
        {
            Vector3 trot = CameraRotation;
            if (Orthographic)
            {
                if (CameraRotation.X < 0)
                    CameraRotation.X += (float)Math.PI * 2;
                if (CameraRotation.X >= Math.PI / 4 && CameraRotation.X < 3 * Math.PI / 4)
                    trot.X = (float)Math.PI / 2;
                else if (CameraRotation.X >= 3 * Math.PI / 4 && CameraRotation.X < 5 * Math.PI / 4)
                    trot.X = (float)Math.PI;
                else if (CameraRotation.X >= 5 * Math.PI / 4 && CameraRotation.X < 7 * Math.PI / 4)
                    trot.X = 3 * (float)Math.PI / 2;
                else if (CameraRotation.X >= 7 * Math.PI / 4 || CameraRotation.X < Math.PI / 4)
                    trot.X = 0;
                trot.Y = 0;
            }
            //Get forward and right vectors
            Vector3 f = new Vector3((float)(Math.Cos(trot.X) * (Properties.Settings.Default.cameraMoveY ? (float)Math.Cos(trot.Y) : 1.0f)),
                                    Properties.Settings.Default.cameraMoveY ? (float)Math.Sin(trot.Y) : 0.0f,
                                    (float)(Math.Sin(trot.X) * (Properties.Settings.Default.cameraMoveY ? (float)Math.Cos(trot.Y) : 1.0f)));

            Vector3 r = new Vector3((float)(Math.Sin(trot.X)),
                                    0.0f,
                                    -(float)(Math.Cos(trot.X)));

            Vector3 u = new Vector3(0.0f, 1.0f, 0.0f);

            //Camera should not go upside down
            //if (m_UpsideDown)
            //    u.Y = -u.Y;

            //Set camera velocity according to key input
            CameraVelocity = new Vector3(0f, 0f, 0f);
            CameraVelocity += ((key_w ? 1.0f : 0.0f) + (key_s ? -1.0f : 0.0f)) * f;
            CameraVelocity += ((key_space ? 1.0f : 0.0f) + (key_shift ? -1.0f : 0.0f)) * u;
            CameraVelocity += ((key_a ? 1.0f : 0.0f) + (key_d ? -1.0f : 0.0f)) * r;

            CameraPosition += CameraVelocity * Properties.Settings.Default.cameraSpeed / 100f;

            //Update view
            UpdateCamera();
            glControl1.Invalidate();
        }

        /* Zooms into a spot */
        public void ZoomTo(float tX, float tY, float tZ, float distance)
        {
            CameraPosition = new Vector3(tX / 10000f, tY / 10000f, tZ / 10000f);
            CameraPosition.X -= distance / 10000f;
            CameraRotation = new Vector3(0f, 0f, 0f);
            UpdateCamera();
            glControl1.Refresh();
        }

        /* Draws a bmd model */
        public static void DrawBMD(Bmd model, RenderMode rnd = RenderMode.Opaque)
        {
            RenderInfo ri = new RenderInfo();
            ri.Mode = rnd;

            BmdRenderer br = new BmdRenderer(model);
            br.Render(ri);
        }

        /* Creates a framed cube */
        public static void GLLineCube(Vector3 v1, Vector3 v2, float padding = 0.0f)
        {
            v1.X -= padding;
            v1.Y -= padding;
            v1.Z -= padding;

            v2.X += padding;
            v2.Y += padding;
            v2.Z += padding;

            GL.Vertex3(v1.X, v1.Y, v1.Z);
            GL.Vertex3(v2.X, v1.Y, v1.Z);

            GL.Vertex3(v1.X, v1.Y, v1.Z);
            GL.Vertex3(v1.X, v2.Y, v1.Z);

            GL.Vertex3(v1.X, v1.Y, v1.Z);
            GL.Vertex3(v1.X, v1.Y, v2.Z);

            GL.Vertex3(v2.X, v1.Y, v1.Z);
            GL.Vertex3(v2.X, v2.Y, v1.Z);

            GL.Vertex3(v2.X, v1.Y, v1.Z);
            GL.Vertex3(v2.X, v1.Y, v2.Z);

            GL.Vertex3(v1.X, v2.Y, v1.Z);
            GL.Vertex3(v2.X, v2.Y, v1.Z);

            GL.Vertex3(v1.X, v2.Y, v1.Z);
            GL.Vertex3(v1.X, v2.Y, v2.Z);

            GL.Vertex3(v1.X, v1.Y, v2.Z);
            GL.Vertex3(v2.X, v1.Y, v2.Z);

            GL.Vertex3(v1.X, v1.Y, v2.Z);
            GL.Vertex3(v1.X, v2.Y, v2.Z);

            GL.Vertex3(v2.X, v2.Y, v1.Z);
            GL.Vertex3(v2.X, v2.Y, v2.Z);

            GL.Vertex3(v2.X, v1.Y, v2.Z);
            GL.Vertex3(v2.X, v2.Y, v2.Z);

            GL.Vertex3(v1.X, v2.Y, v2.Z);
            GL.Vertex3(v2.X, v2.Y, v2.Z);
        }
        /* Creates a framed cube */
        public static void GLLineCube(Vector3 center, float size = 0.0f)
        {
            Vector3 v1 = new Vector3(center.X - size, center.Y - size, center.Z - size);
            Vector3 v2 = new Vector3(center.X + size, center.Y + size, center.Z + size);

            GLLineCube(v1, v2);
        }

        /* Creates a framed cube */
        public static void GLLineCamera(Vector3 v1, Vector3 v2, float length, float angle, float padding = 0.0f)
        {
            v1.X -= padding;
            v1.Y -= padding;
            v1.Z -= padding;

            v2.X += padding;
            v2.Y += padding;
            v2.Z += padding;

            GLLineCube(v1, v2, 0.0f);

            //Camera end
            GL.Vertex3(v2.X, v1.Y, v1.Z);
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v1.Y - (length * Math.Cos(angle)), v1.Z - (length * Math.Cos(angle)));

            GL.Vertex3(v2.X, v2.Y, v1.Z);
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v2.Y + (length * Math.Cos(angle)), v1.Z - (length * Math.Cos(angle)));

            GL.Vertex3(v2.X, v1.Y, v2.Z);
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v1.Y - (length * Math.Cos(angle)), v2.Z + (length * Math.Cos(angle)));

            GL.Vertex3(v2.X, v2.Y, v2.Z);
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v2.Y + (length * Math.Cos(angle)), v2.Z + (length * Math.Cos(angle)));

            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v1.Y - (length * Math.Cos(angle)), v1.Z - (length * Math.Cos(angle)));
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v2.Y + (length * Math.Cos(angle)), v1.Z - (length * Math.Cos(angle)));

            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v1.Y - (length * Math.Cos(angle)), v1.Z - (length * Math.Cos(angle)));
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v1.Y - (length * Math.Cos(angle)), v2.Z + (length * Math.Cos(angle)));

            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v2.Y + (length * Math.Cos(angle)), v1.Z - (length * Math.Cos(angle)));
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v2.Y + (length * Math.Cos(angle)), v2.Z + (length * Math.Cos(angle)));

            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v1.Y - (length * Math.Cos(angle)), v2.Z + (length * Math.Cos(angle)));
            GL.Vertex3(v2.X + (length * Math.Sin(angle)), v2.Y + (length * Math.Cos(angle)), v2.Z + (length * Math.Cos(angle)));
        }
        /* Creates a framed camera shape */
        public static void GLLineCamera(Vector3 center, float length, float angle, float size = 0.0f)
        {
            Vector3 v1 = new Vector3(center.X - size, center.Y - size, center.Z - size);
            Vector3 v2 = new Vector3(center.X + size, center.Y + size, center.Z + size);

            GLLineCamera(v1, v2, length, angle);
        }

        public void InitRails(RalFile ral)
        {   //Initializes rail file
            rails = ral;
        }
        public void InitDemo(Bmd cam, BckFile bck)
        {   //Initializes rail file
            camera = cam;
            demo = bck;
        }
        public void DrawRails()
        {   //Draws all rails and their paths
            if (rails == null)
                return;

            Rail[] allRails = rails.GetAllRails();
            for (int i = 0; i < allRails.Length; i++)
            {
                Rail rail = allRails[i];
                List<Vector3> railPath = new List<Vector3>();

                GL.LineWidth(2);
                for (int j = 0; j < rail.frames.Length; j++)
                {
                    KeyFrame frame = rail.frames[j];
                    railPath.Add(new Vector3(frame.x, frame.y, frame.z));

                    GL.PushMatrix();

                    GL.Translate(frame.x, frame.y, frame.z);

                    if (SelectedRail == i && SelectedFrame == j)
                        GL.Color3(Properties.Settings.Default.railSelColor);
                    else
                        GL.Color3(Properties.Settings.Default.railColor);

                    GL.Begin(PrimitiveType.Lines);
                    GLLineCube(Vector3.Zero, 64f);
                    GL.End();

                    GL.PopMatrix();
                }
                if (SelectedRail == i)
                    GL.Color3(Properties.Settings.Default.railNodeSelColor);
                else
                    GL.Color3(Properties.Settings.Default.railNodeColor);

                GL.LineWidth(4);
                GL.Begin(PrimitiveType.Lines);
                for (int j = 0; j < railPath.Count - 1; j++)
                {
                    for (int k = 0; k < rail.frames[j].u1; k++){
                        if (rail.frames[j].connections[k] >= railPath.Count)
                            continue;
                        GL.Vertex3(railPath[j]);
                        GL.Vertex3(railPath[rail.frames[j].connections[k]]);
                    }
                }
                GL.End();
            }
        }

        static float StartDemo_DemoDuration = 0.0f;
        static Timer StartDemo_DemoTimer;
        public void StartDemoAnimation()
        {
            if (StartDemo_DemoTimer != null && StartDemo_DemoTimer.Enabled)
                StartDemo_DemoTimer.Stop();
            StartDemo_DemoTimer = new Timer();
            StartDemo_DemoTimer.Interval = 16;
            StartDemo_DemoTimer.Tick += DemoAnim_Tick;
            StartDemo_DemoTimer.Start();

            StartDemo_DemoDuration = 0.0f;
        }

        private void DemoAnim_Tick(object sender, EventArgs e)
        {

            float rdur = StartDemo_DemoDuration;
            BckSection.BckANK1 sec = ((BckSection.BckANK1)demo.sections[0]);

            BckSection.BckANK1.Animation anim1 = sec.GetJointAnimation(0);
            BckSection.BckANK1.Animation anim2 = sec.GetJointAnimation(1);
            BckSection.BckANK1.Animation canim = null;
            if (StartDemo_DemoDuration > anim1.Duration)
            {
                rdur -= anim1.Duration;
                canim = anim2;
            }
            else
                canim = anim1;

            CameraPosition = (DataVectorToVector3(canim.InterpolatePosition(rdur, BckSection.BckANK1.Animation.InterpolationType.Linear))) / 10000f;
            Vector3 camrot = (DataVectorToVector3(canim.InterpolateRotation(rdur, BckSection.BckANK1.Animation.InterpolationType.Linear)) * (float)Math.PI / 180f);
            CameraRotation = new Vector3(camrot.Y, camrot.X, camrot.Z);

            StartDemo_DemoDuration += 1;
            if (StartDemo_DemoDuration > anim1.Duration + anim2.Duration)
                StartDemo_DemoTimer.Stop();

            UpdateCamera();
            UpdateViewport();
            glControl1.Refresh();
        }

        public void DrawDemo()
        {   //Draws all rails and their paths
            if (demo == null)
                return;
            if (camera == null)
                return;

            Vector3 p0 = Vector3.Zero;
            for (int i = 0; i < ((BckSection.BckANK1)demo.sections[0]).jointanims.Count; i++)
            {
                BckSection.BckANK1.Animation anim = ((BckSection.BckANK1)demo.sections[0]).GetJointAnimation(i);

                List<float> allKeys = new List<float>();
                SortedDictionary<float, Vector3> pos = new SortedDictionary<float, Vector3>();
                SortedDictionary<float, Vector3> rot = new SortedDictionary<float, Vector3>();
                SortedDictionary<float, Vector3> scale = new SortedDictionary<float, Vector3>();

                Action<SortedDictionary<float, Vector3>, List<AnimKeyFrame>, List<AnimKeyFrame>, List<AnimKeyFrame>> FillNodeDict =
                    (SortedDictionary<float, Vector3> dict, List<AnimKeyFrame> framex, List<AnimKeyFrame> framey, List<AnimKeyFrame> framez) =>
                {
                    for (int j = 0; j < framex.Count; j++)
                        if (!dict.ContainsKey(framex[j].time)) dict.Add(framex[j].time, new Vector3(framex[j].value, float.NaN, float.NaN)); else dict[framex[j].time] = new Vector3(framex[j].value, dict[framex[j].time].Y, dict[framex[j].time].Z);
                    for (int j = 0; j < framey.Count; j++)
                        if (!dict.ContainsKey(framey[j].time)) dict.Add(framey[j].time, new Vector3(float.NaN, framey[j].value, float.NaN)); else dict[framey[j].time] = new Vector3(dict[framey[j].time].X, framey[j].value, dict[framey[j].time].Z);
                    for (int j = 0; j < framez.Count; j++)
                        if (!dict.ContainsKey(framez[j].time)) dict.Add(framez[j].time, new Vector3(float.NaN, float.NaN, framez[j].value)); else dict[framez[j].time] = new Vector3(dict[framez[j].time].X, dict[framez[j].time].Y, framez[j].value);

                    for (int j = 0; j < framex.Count; j++)
                        if (!allKeys.Contains(framex[j].time)) allKeys.Add(framex[j].time);
                    for (int j = 0; j < framey.Count; j++)
                        if (!allKeys.Contains(framey[j].time)) allKeys.Add(framey[j].time);
                    for (int j = 0; j < framez.Count; j++)
                        if (!allKeys.Contains(framez[j].time)) allKeys.Add(framez[j].time);
                };

                FillNodeDict(pos, anim.x, anim.y, anim.z);
                FillNodeDict(rot, anim.rotationx, anim.rotationy, anim.rotationz);
                FillNodeDict(scale, anim.scalex, anim.scaley, anim.scalez);

                allKeys.Sort();

                Vector3 lp = Vector3.Zero;
                Vector3 p = Vector3.Zero;
                Vector3 r = Vector3.Zero;
                Vector3 s = Vector3.Zero;

                //Draw Path
                const int DEMOPATH_RESOLUTION = 8;
                GL.LineWidth(4);
                GL.Color3(Properties.Settings.Default.railNodeColor);

                GL.PushMatrix();
                GL.Begin(PrimitiveType.Lines);
                for (int j = 0; j < allKeys.Count - 1; j++)
                {
                    float dtime = (allKeys[j + 1] - allKeys[j]) / DEMOPATH_RESOLUTION;
                    for (int k = 0; k < DEMOPATH_RESOLUTION; k++)
                    {
                        Vector3 cur = DataVectorToVector3(anim.InterpolatePosition(allKeys[j] + (dtime * k), BckSection.BckANK1.Animation.InterpolationType.Linear));// + camera.Joints[i].Translation;
                        GL.Vertex3(cur);
                        if (j != 0 || k != 0)
                            GL.Vertex3(cur);
                    }
                }
                //GL.Vertex3(DataVectorToVector3(anim.InterpolatePosition(allKeys[allKeys.Count - 1])));
                GL.End();

                GL.PopMatrix();

                //Draw Keyframes
                GL.LineWidth(2);
                for (int j = 0; j < allKeys.Count; j++)
                {
                    p = DataVectorToVector3(anim.InterpolatePosition(allKeys[j], BckSection.BckANK1.Animation.InterpolationType.Cubic)) + camera.Joints[i].Translation;
                    r = DataVectorToVector3(anim.InterpolateRotation(allKeys[j], BckSection.BckANK1.Animation.InterpolationType.Cubic)) + camera.Joints[i].Rotation;
                    s = DataVectorToVector3(anim.InterpolateScale(allKeys[j], BckSection.BckANK1.Animation.InterpolationType.Cubic));
                    
                    GL.PushMatrix();
                    GL.Translate(p);
                    GL.Rotate(r.Z, 0f, 0f, 1f);
                    GL.Rotate(r.Y, 0f, 1f, 0f);
                    GL.Rotate(r.X, 1f, 0f, 0f);

                    GL.Color3(Properties.Settings.Default.railColor);

                    if (j == 0 || j == allKeys.Count - 1 || rot.ContainsKey(allKeys[j]) || scale.ContainsKey(allKeys[j]))
                    {
                        GL.Begin(PrimitiveType.Lines);
                        GLLineCamera(Vector3.Zero, 25f, 0.785398f, 64f);
                        GL.End();
                    }

                    GL.PopMatrix();
                    lp = p;
                }
            }

            GL.PushMatrix();

            GL.Translate(camera.Joints[0].Translation);
            GL.Rotate(camera.Joints[0].Rotation.Z, 0f, 0f, 1f);
            GL.Rotate(camera.Joints[0].Rotation.Y, 0f, 1f, 0f);
            GL.Rotate(camera.Joints[0].Rotation.X, 1f, 0f, 0f);

            GL.Color3(Color.DarkRed);
            GL.Begin(PrimitiveType.Lines);
            GLLineCamera(Vector3.Zero, 25f, 0.785398f, 64f);
            GL.End();

            GL.PopMatrix();
            GL.PushMatrix();

            GL.Translate(camera.Joints[1].Translation);
            GL.Rotate(camera.Joints[1].Rotation.Z, 0f, 0f, 1f);
            GL.Rotate(camera.Joints[1].Rotation.Y, 0f, 1f, 0f);
            GL.Rotate(camera.Joints[1].Rotation.X, 1f, 0f, 0f);

            GL.Color3(Color.Red);
            GL.Begin(PrimitiveType.Lines);
            GLLineCamera(Vector3.Zero, 25f, 0.785398f, 64f);
            GL.End();

            GL.PopMatrix();
        }

        public void ForceDraw()
        {
            glControl1.Refresh();
        }

        /* Returns the center of a cube */
        private static Vector3[] GetCubeVertices(Vector3 p1, Vector3 p2)
        {
            Vector3[] pnts = new Vector3[8];
            pnts[0] = new Vector3(p1.X, p1.Y, p1.Z);
            pnts[1] = new Vector3(p2.X, p1.Y, p1.Z);
            pnts[2] = new Vector3(p1.X, p2.Y, p1.Z);
            pnts[3] = new Vector3(p1.X, p1.Y, p2.Z);
            pnts[4] = new Vector3(p2.X, p2.Y, p1.Z);
            pnts[5] = new Vector3(p1.X, p2.Y, p2.Z);
            pnts[6] = new Vector3(p2.X, p1.Y, p2.Z);
            pnts[7] = new Vector3(p2.X, p2.Y, p2.Z);
            return pnts;
        }
        /* Returns the center of a cube */
        private static Vector3 GetCubeCenter(Vector3 p1, Vector3 p2)
        {
            return (p1 + p2) / 2f;
        }
        /* Creates smallest possible rectangle around points */
        private static Rectangle BoxIn(Vector2[] pnts)
        {
            if (pnts.Length == 0)
                return new Rectangle(0, 0, 0, 0);
            Vector2 min = new Vector2(pnts[0].X, pnts[0].Y);
            Vector2 max = new Vector2(pnts[0].X, pnts[0].Y); ;
            for (int i = 0; i < pnts.Length; i++)
            {
                if (pnts[i].X < min.X)
                    min.X = pnts[i].X;
                if (pnts[i].Y < min.Y)
                    min.Y = pnts[i].Y;

                if (pnts[i].X > max.X)
                    max.X = pnts[i].X;
                if (pnts[i].Y > max.Y)
                    max.Y = pnts[i].Y;
            }
            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
        /* Removes axis from vector */
        private static Vector2 RemoveAxis(Vector3 point, int axis = 3)
        {
            switch (axis){
                case 1: return new Vector2(point.Y, point.Z);
                case 2: return new Vector2(point.X, point.Z);
                default:
                case 3: return new Vector2(point.X, point.Y);
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.Undo();
            UpdateAllObjects();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.Redo();
            UpdateAllObjects();
        }

        private static Vector3 DataVectorToVector3(DataReader.Vector v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        #region glControlX
        private void glControlX_Load(object sender, EventArgs e)
        {
        }
        private void glControlX_Paint(object sender, EventArgs e)
        {
        }
        private void glControlX_Resize(object sender, EventArgs e)
        {
        }

        private void glControlX_MouseDown(object sender, MouseEventArgs e)
        {
        }
        private void glControlX_MouseUp(object sender, MouseEventArgs e)
        {

        }

        /* Mousewheel zooms in and out */
        private void glControlX_MouseWheel(object sender, MouseEventArgs e)
        {
        }
        private void glControlX_KeyDown(object sender, KeyEventArgs e)
        {
           
        }
        private void glControlX_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion
        #region glControlX
        private void glControlY_Load(object sender, EventArgs e)
        {
        }
        private void glControlY_Paint(object sender, EventArgs e)
        {
        }
        private void glControlY_Resize(object sender, EventArgs e)
        {
        }

        private void glControlY_MouseDown(object sender, MouseEventArgs e)
        {
        }
        private void glControlY_MouseUp(object sender, MouseEventArgs e)
        {

        }


        /* Mousewheel zooms in and out */
        private void glControlY_MouseWheel(object sender, MouseEventArgs e)
        {
        }
        private void glControlY_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void glControlY_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion
        #region glControlX
        private void glControlZ_Load(object sender, EventArgs e)
        {
        }
        private void glControlZ_Paint(object sender, EventArgs e)
        {
        }
        private void glControlZ_Resize(object sender, EventArgs e)
        {
        }

        private void glControlZ_MouseDown(object sender, MouseEventArgs e)
        {
        }
        private void glControlZ_MouseUp(object sender, MouseEventArgs e)
        {

        }


        /* Mousewheel zooms in and out */
        private void glControlZ_MouseWheel(object sender, MouseEventArgs e)
        {
        }
        private void glControlZ_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void glControlZ_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion
    }

    /* Stores model information for cache purposes */
    public class BmdInfo
    {
        public string ModelName;
        public string TexName;
        public Bmd Model;   //Model 
        public int List;    //GL List
        public int BBList;  //Bounding box List

        public uint ForceMode;

        /* Bmd Info */
        public BmdInfo(FileBase fb, string name, FileBase bmt = null, string texname = "", uint drawmodes = 0)
        {
            Model = new Bmd(fb);

            if (bmt != null)
                Model.ReadBMT(bmt);
            TexName = texname;

            ForceMode = drawmodes;

            ModelName = name;
            BuildLists();
        }

        public void BuildLists(uint forceddrawmodes = 0)
        {
            List = GL.GenLists(1);
            BBList = GL.GenLists(1);

            uint cmode = Properties.Settings.Default.objectDrawMode | ForceMode;
            //Model
            GL.NewList(List, ListMode.Compile);
            if ((cmode | 0x01) == cmode)
                Preview.DrawBMD(Model);
            if ((cmode | 0x02) == cmode)
                Preview.DrawBMD(Model, RenderMode.Translucent);
            GL.EndList();

            //Calculate bounds of model
            Vector3 minBox = Vector3.Zero;
            Vector3 maxBox = Vector3.Zero;
            GetRealBounds(out minBox, out maxBox);

            //Create cube around bounds
            GL.NewList(BBList, ListMode.Compile);
            GL.Begin(PrimitiveType.Lines);
            Preview.GLLineCube(minBox, maxBox, 0.2f);
            GL.End();
            GL.EndList();
        }

        //Makes a quick bounding box
        public void GetRealBounds(out Vector3 minBox, out Vector3 maxBox)
        {
            minBox.X = float.MaxValue;
            minBox.Y = float.MaxValue;
            minBox.Z = float.MaxValue;

            maxBox.X = float.MinValue;
            maxBox.Y = float.MinValue;
            maxBox.Z = float.MinValue;

            //Yeah its pretty bad
            foreach (Bmd.SceneGraphNode node in Model.SceneGraph)
            {
                if (node.NodeType != 0)
                    continue;
                Bmd.Batch batch = Model.Batches[node.NodeID];
                foreach (Bmd.Batch.Packet packet in batch.Packets)
                {
                    foreach (Bmd.Batch.Packet.Primitive primitive in packet.Primitives)
                    {
                        PrimitiveType[] primtypes = { PrimitiveType.Quads, PrimitiveType.Points, PrimitiveType.Triangles, PrimitiveType.TriangleStrip,
                                                    PrimitiveType.TriangleFan, PrimitiveType.Lines, PrimitiveType.LineStrip, PrimitiveType.Points };
                        PrimitiveType type = primtypes[(primitive.PrimitiveType - 0x80) / 8];
                        if (type != PrimitiveType.Triangles && type != PrimitiveType.TriangleStrip && type != PrimitiveType.Quads)
                            continue;
                        foreach (int nd in primitive.PositionIndices)
                        {
                            Vector3 pos = Model.PositionArray[nd];
                            if (pos.X < minBox.X)
                                minBox.X = pos.X;
                            if (pos.Y < minBox.Y)
                                minBox.Y = pos.Y;
                            if (pos.Z < minBox.Z)
                                minBox.Z = pos.Z;

                            if (pos.X > maxBox.X)
                                maxBox.X = pos.X;
                            if (pos.Y > maxBox.Y)
                                maxBox.Y = pos.Y;
                            if (pos.Z > maxBox.Z)
                                maxBox.Z = pos.Z;
                        }
                    }
                }
            }
        }

        public void UnLoad()
        {
            GL.DeleteLists(List, 1);
            GL.DeleteLists(BBList, 1);
            ModelName = null;
            Model = null;
        }
    }

    /* Object to be drawn in scene */
    public class SceneObject
    {
        private const float CUBESIZE = 64f; //Size of model-less cube
        private const float FLAGOFFSET = 0f; //Width of flags
        private const float FLAGWIDTH = 64f; //Width of flags
        private const float FLAGHEIGHT = 64f; //Height of flags

        public bool Selected;   //Whether or not this is selected


        private GameObject Parent;  //Object this is based off of

        private bool Drawable;  //Whether or not this can be drawn

        public int FlagTex = 0;
        public bool IsFlag = false;
        private int FlagWidth;
        private int FlagHeight;
        
        /* Spacial information */
        private Vector3 Position;
        private Vector3 Rotation;
        private Vector3 Scale;

        public Vector3 ObjPosition { get { return Position; } }
        public Vector3 ObjRotation { get { return Rotation; } }
        public Vector3 ObjScale { get { return Scale; } }

        private Vector3 BBoxMin;
        private Vector3 BBoxMax;

        public Vector3 ObjBBoxMin { get { return BBoxMin + Position; } }
        public Vector3 ObjBBoxMax { get { return BBoxMax + Position; } }
        public Vector3 RealObjBBoxMin { get { return (ObjScale * BBoxMin) + Position; } }
        public Vector3 RealObjBBoxMax { get { return (ObjScale * BBoxMax) + Position; } }

        /* Model information */
        private string ModelName;
        public BmdInfo Model;

        private Color DrawColor;

        /* GL Lists */
        public int GLList;
        public int GLBB;    //Bounding box

        private static Dictionary<string, ObjectParameters> ParamCache = new Dictionary<string, ObjectParameters>();

        private static Dictionary<string, int> FlagTextures = new Dictionary<string, int>();

        private static Dictionary<string, BmdInfo> ModelCache = new Dictionary<string, BmdInfo>();  //Cache of all models
        private static int Cube = 0;    //Model-less cube GL List
        private static int Flag = 0;
        private static int FlagBB = 0;

        /* Returns whether or not this can be drawn */
        public bool CanBeDrawn
        {
            get { return Drawable; }
        }

        /* SceneObject */
        public SceneObject(GameObject parent, string sceneRoot)
        {
            Parent = parent;
            Drawable = false;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.Zero;
            ModelName = null;
            Model = null;
            GLList = 0;
            GLBB = 0;
            IsFlag = false;
            Selected = false;

            GenerateModel(sceneRoot);
        }

        /* Selects and deselects */
        public void Select()
        {
            Selected = true;
        }
        public void Deselect()
        {
            Selected = false;
        }

        /* Generates model to list (or gets one from cache) */
        public void GenerateModel(string ScenePath)
        {
            Update();
            if (!Drawable)
                return;

            ModelName = null;

            //Read parameters
            ObjectParameters op;
            if (ParamCache.ContainsKey(Parent.Name))
            {
                op = ParamCache[Parent.Name];
                op.Adjust(Parent);
            }
            else
            {
                op = new ObjectParameters();
                op.ReadObjectParameters(Parent);
                ParamCache.Add(Parent.Name, op);
            }

            string model = op.GetParamValue("Model", Parent);

            if (op.ContainsParameter("FlagTexture") || op.ContainsParameter(model + "_flag") || op.GetParamValue("Flag", Parent).ToLower() == "true")
            {
                string texname;
                if (op.ContainsParameter(model + "_flag"))
                    texname = op.GetParamValue(model + "_flag", Parent);
                else if (op.ContainsParameter("FlagTexture"))
                    texname = op.GetParamValue("FlagTexture", Parent);
                else
                    texname = "mapobj\\" + model.ToLower() + ".bti";

                if (!FlagTextures.ContainsKey(texname))
                {
                    if (File.Exists(ScenePath + texname))
                    {
                        FileBase fb = new FileBase();
                        fb.Stream = new FileStream(ScenePath + texname, FileMode.Open);
                        FlagTex = BTIFile.ReadBTI(fb, out FlagWidth, out FlagHeight);
                        fb.Close();
                        FlagTextures.Add(texname, FlagTex);
                    }
                }
                else
                    FlagTex = FlagTextures[texname];

                IsFlag = true;
                GLList = Flag;  //Flag
                GLBB = FlagBB;
                BBoxMax = new Vector3(FLAGOFFSET + 1f, FLAGHEIGHT, FLAGWIDTH);
                BBoxMin = new Vector3(FLAGOFFSET - 1f, 0f, 0f);
                return;
            }
            else
                IsFlag = false;

            if (op.ContainsParameter("DisplayModel"))   //If the object has a DisplayModel comment, use it as the model
                ModelName = op.GetParamValue("DisplayModel", Parent).ToLower();

            //If the object only appears correctly in certain draw modes, force that mode.
            uint ForceMode = 0;
            if (op.ContainsParameter("ForceDrawMode") || op.ContainsParameter(ModelName + "_mode"))
            {
                string[] modes = Enum.GetNames(typeof(DrawModes));
                string selmode;
                if (op.ContainsParameter("ForceDrawMode"))
                    selmode = op.GetParamValue("ForceDrawMode", Parent);
                else
                    selmode = op.GetParamValue(ModelName + "_mode", Parent);
                for (int i = 0; i < modes.Length; i++)
                    if (selmode == modes[i])
                        ForceMode = (uint)i;
            }
            if (ModelName == "" || ModelName == null)
            {
                //If object has a model value, use it to get the model
                if (op.ContainsParameter(model))    //If there is a comment with the same name as the Model parameter, use it as the model
                    ModelName = op.GetParamValue(model, Parent).ToLower();
                else                                //Otherwise, just estimate the model path
                    ModelName = "mapobj/" + model.ToLower() + ".bmd";
            }
            if (!File.Exists(ScenePath + ModelName))
                ModelName = null;   //No such model

            if (ModelName != null)
            {
                //Model exists

                string texname = "";
                FileBase bmt = null;
                if (op.ContainsParameter("DisplayTexture"))
                {   //If the object has a DisplayModel comment, use it as the model
                    texname = op.GetParamValue("DisplayTexture", Parent).ToLower();
                    if (File.Exists(ScenePath + texname))
                    {
                        bmt = new FileBase();
                        bmt.Stream = new FileStream(ScenePath + texname, FileMode.Open);
                    }
                }
                else if (op.ContainsParameter(model + "_tex"))
                {
                    texname = op.GetParamValue(model + "_tex", Parent).ToLower();
                    if (File.Exists(ScenePath + texname))
                    {
                        bmt = new FileBase();
                        bmt.Stream = new FileStream(ScenePath + texname, FileMode.Open);
                    }
                }

                string key = ModelName + texname + ForceMode;
                if (ModelCache.ContainsKey(key))
                {
                    Model = ModelCache[key];    //Get from cache
                    if (Model.ModelName != ModelName || Model.TexName != texname || Model.ForceMode != ForceMode)
                    {
                        //Model changed, update cache
                        FileBase fb = new FileBase();
                        fb.Stream = new FileStream(ScenePath + ModelName, FileMode.Open);
                        try { Model = new BmdInfo(fb, ModelName, bmt, texname, ForceMode); }
                        catch { ModelName = null; }
                        fb.Close();

                        if (ModelName != null)
                            UpdateModel(key, Model);
                    }
                }
                else
                {
                    //Load model to cache
                    FileBase fb = new FileBase();
                    fb.Stream = new FileStream(ScenePath + ModelName, FileMode.Open);
                    try { Model = new BmdInfo(fb, ModelName, bmt, texname, ForceMode); }
                    catch { ModelName = null; }
                    fb.Close();

                    if (ModelName != null)
                        UpdateModel(key, Model);
                }

                if (Model != null)
                    Model.GetRealBounds(out BBoxMin, out BBoxMax);

                if (bmt != null)
                    bmt.Close();
            }

            if (Model == null)
            {
                GLList = Cube;  //Cube
                BBoxMax = new Vector3(CUBESIZE, CUBESIZE, CUBESIZE);
                BBoxMin = -BBoxMax;
            }
            else
            {
                //Set up GL Lists from cache
                GLList = Model.List;
                GLBB = Model.BBList;
            }
        }

        /* Update object from parent */
        public void Update()
        {
            bool prev = Drawable;

            //Read parameters
            ObjectParameters op;
            if (ParamCache.ContainsKey(Parent.Name))
            {
                op = ParamCache[Parent.Name];
                op.Adjust(Parent);
            }
            else
            {
                op = new ObjectParameters();
                op.ReadObjectParameters(Parent);
                ParamCache.Add(Parent.Name, op);
            }

            if (op.ContainsParameter("X") && op.ContainsParameter("Y") && op.ContainsParameter("Z"))
            {
                float.TryParse(op.GetParamValue("X", Parent), out Position.X);
                float.TryParse(op.GetParamValue("Y", Parent), out Position.Y);
                float.TryParse(op.GetParamValue("Z", Parent), out Position.Z);
                Drawable = true;
            }
            else
            {
                Drawable = false;
                return;
            }
            if (op.ContainsParameter("Pitch") && op.ContainsParameter("Yaw") && op.ContainsParameter("Roll"))
            {
                float.TryParse(op.GetParamValue("Pitch", Parent), out Rotation.X);
                float.TryParse(op.GetParamValue("Yaw", Parent), out Rotation.Y);
                float.TryParse(op.GetParamValue("Roll", Parent), out Rotation.Z);
            }
            else if (op.ContainsParameter("RotationX") && op.ContainsParameter("RotationY") && op.ContainsParameter("RotationZ"))
            {
                float.TryParse(op.GetParamValue("RotationX", Parent), out Rotation.X);
                float.TryParse(op.GetParamValue("RotationY", Parent), out Rotation.Y);
                float.TryParse(op.GetParamValue("RotationZ", Parent), out Rotation.Z);
            } 
            else
                Rotation = Vector3.Zero;
            if (op.ContainsParameter("ScaleX") && op.ContainsParameter("ScaleY") && op.ContainsParameter("ScaleZ"))
            {
                float.TryParse(op.GetParamValue("ScaleX", Parent), out Scale.X);
                float.TryParse(op.GetParamValue("ScaleY", Parent), out Scale.Y);
                float.TryParse(op.GetParamValue("ScaleZ", Parent), out Scale.Z);
            }
            else
                Scale = new Vector3(1f, 1f, 1f);
            if (op.ContainsParameter("DisplayColor"))
            {
                string colorStr = op.GetParamValue("DisplayColor", null);
                string[] validColors = Enum.GetNames(typeof(KnownColor));

                bool valid = false;
                for (int i = 0; i < validColors.Length; i++)
                {
                    if (colorStr == validColors[i])
                    {
                        valid = true;
                        break;
                    }
                }

                if (valid)  //If color exists, use it
                    DrawColor = Color.FromName(colorStr);
                else
                {   //If color doesn't exist try reading integer
                    int color;
                    try { color = Convert.ToInt32(colorStr, 16); }
                    catch { color = 0xFFFFFF; }
                    DrawColor = Color.FromArgb(color);
                }
            }
            else
                DrawColor = Color.White;

        }

        /* Draw to scene */
        public void Draw()
        {
            if (!Drawable)
                return;

            //Set up spacial information
            GL.LineWidth(2);
            GL.PushMatrix();
            GL.Translate(Position.X, Position.Y, Position.Z);
            GL.Rotate(Rotation.Z, 0f, 0f, 1f);
            GL.Rotate(Rotation.Y, 0f, 1f, 0f);
            GL.Rotate(Rotation.X, 1f, 0f, 0f);
            GL.Scale(Scale.X, Scale.Y, Scale.Z);


            if (Model == null && !IsFlag)
            {
                //No model colors
                if (Selected)
                    GL.Color4(Properties.Settings.Default.objSelColor);
                else
                    GL.Color4(Properties.Settings.Default.objColor);
            }
            else if (Selected)
            {
                //Draw bounding box
                GL.Color4(Properties.Settings.Default.objSelColor);
                GL.CallList(GLBB);
            }

            if (Model != null || IsFlag)
                GL.Color4(DrawColor); //Set draw color

            if (IsFlag)
            {   //Draw flag
                // shader: handles multitexturing, color combination, alpha test
                GL.UseProgram(1);

                GL.ActiveTexture(TextureUnit.Texture0);

                int loc = GL.GetUniformLocation(1, "texture0");
                GL.Uniform1(loc, 0);
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, FlagTex);

                GL.CullFace(CullFaceMode.Front);
                GL.CallList(GLList);
                GL.CullFace(CullFaceMode.Back);
                GL.CallList(GLList);

                GL.Uniform1(loc, 0);
                GL.Disable(EnableCap.Texture2D);
            }
            else//Draw model
                GL.CallList(GLList);

            //Restore camera
            GL.PopMatrix();
        }

        /* Unload model */
        public void UnLoadModel()
        {
            Model = null;
            ModelName = null;
            Drawable = false;
        }

        /* Clear cache */
        public static void ClearCache()
        {
            foreach (KeyValuePair<string, BmdInfo> kvp in ModelCache)
                GL.DeleteLists(kvp.Value.List, 1);
            foreach (KeyValuePair<string, int> kvp in FlagTextures)
                GL.DeleteTexture(kvp.Value);
            ParamCache.Clear();
            ModelCache.Clear();
        }

        /* Updates model in cache */
        public static void UpdateModel(string key, BmdInfo newModel)
        {
            if (ModelCache.ContainsKey(key))
            {
                ModelCache[key].UnLoad();
                ModelCache[key] = newModel;
            }
            else
                ModelCache.Add(key, newModel);
        }

        public static void UpdateParameter(GameObject go)
        {
            ObjectParameters op = new ObjectParameters();
            op.ReadObjectParameters(go);
            if (!ParamCache.ContainsKey(go.Name))
                ParamCache.Add(go.Name, op);
            else
                ParamCache[go.Name] = op;
        }

        /* Build default cube */
        public static void BuildCube()
        {
            Cube = GL.GenLists(1);
            GL.NewList(Cube, ListMode.Compile);
            GL.Begin(PrimitiveType.Lines);
            Preview.GLLineCube(new Vector3(-CUBESIZE, -CUBESIZE, -CUBESIZE), new Vector3(CUBESIZE, CUBESIZE, CUBESIZE));
            GL.End();
            GL.EndList();
        }
        /* Build default flag */
        public static void BuildFlag()
        {
            Flag = GL.GenLists(1);
            GL.NewList(Flag, ListMode.Compile);
            GL.Begin(PrimitiveType.Triangles);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(FLAGOFFSET, 0f, 0f);
            GL.TexCoord2(-1f, 0f);
            GL.Vertex3(FLAGOFFSET, 0f, FLAGWIDTH);
            GL.TexCoord2(0f, -1f);
            GL.Vertex3(FLAGOFFSET, FLAGHEIGHT, 0f);

            GL.TexCoord2(-1f, 0f);
            GL.Vertex3(FLAGOFFSET, 0f, FLAGWIDTH);
            GL.TexCoord2(-1f, -1f);
            GL.Vertex3(FLAGOFFSET, FLAGHEIGHT, FLAGWIDTH);
            GL.TexCoord2(0f, -1f);
            GL.Vertex3(FLAGOFFSET, FLAGHEIGHT, 0f);
            GL.End();
            GL.EndList();

            FlagBB = GL.GenLists(1);
            GL.NewList(FlagBB, ListMode.Compile);
            GL.Begin(PrimitiveType.Lines);
            Preview.GLLineCube(new Vector3(FLAGOFFSET + 1f, FLAGHEIGHT, FLAGWIDTH), new Vector3(FLAGOFFSET - 1f, 0f, 0f));
            GL.End();
            GL.EndList();
        }
        /* Deletes cube */
        public static void DeleteCube()
        {
            GL.DeleteLists(Cube, 1);
        }
        /* Deletes flag */
        public static void DeleteFlag()
        {
            GL.DeleteLists(Flag, 1);
        }
    }

    class Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public Ray(Vector3 origin, Vector3 dir)
        {
            Origin = origin;
            Direction = dir;
        }
    }
}
