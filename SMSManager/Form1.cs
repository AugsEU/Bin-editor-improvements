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
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using SMSReader;
using DataReader;

namespace SMSManager
{
    public partial class Form1 : Form
    {
        const string HASHFILE = "hashes.dat";
        string CurrentAction = null;

        ProcessStartInfo BinEditor = null;

        TOCFile directory;

        bool LoadedProject = false;
        string ProjectPath = null;
        string ProjectName = null;

        SMSScene StageArc;
        GameObject[][] Strings;
        GameObject[] Tables;
        public string[][] Levels;
        public string[] Names;

        Translate_Buddy translator = new Translate_Buddy();

        Dictionary<string, bool> FilesChanged = new Dictionary<string, bool>();
        Dictionary<string, long> dateDict = new Dictionary<string, long>();


        public Form1()
        {
            if (Properties.Settings.Default.needsUpgrade)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.needsUpgrade = false;
                Properties.Settings.Default.Save();
            }

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Projects"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Projects");
            Console.SetOut(new TextBoxWriter(consoleBox));

            toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;

            LoadPrograms();

            Console.WriteLine("Ready");

            if (Properties.Settings.Default.LastProject != "" && Properties.Settings.Default.AutoLoad)
                StartMultiTasking("Opening Startup Project", Open, Properties.Settings.Default.LastProject);

            IEnumerable<string> projects = Directory.EnumerateDirectories(Directory.GetCurrentDirectory() + "\\Projects");
            foreach (string project in projects)
            {
                if (Directory.Exists(project + "\\root") && Directory.Exists(project + "\\edit"))
                {
                    DirectoryInfo di = new DirectoryInfo(project);
                    ToolStripMenuItem proj = new ToolStripMenuItem(di.Name);
                    proj.Click += new EventHandler(delegate
                    {
                        StartMultiTasking("Opening Project", Open, project);
                    });

                    openProjectToolStripMenuItem.DropDownItems.Add(proj);
                }
            }
        }

        private byte[] GetFolderHash(string dir)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
            byte[] dat = new byte[16];
            using (var md5 = MD5.Create())
            {
                foreach (string file in files)
                {
                    using (var stream = File.OpenRead(file))
                    {
                        dat = XORBytes(dat, md5.ComputeHash(stream));
                    }
                }
            }
            return dat;
        }
        private static byte[] XORBytes(byte[] dat1, byte[] dat2)
        {
            byte[] o = new byte[Math.Max(dat1.Length, dat2.Length)];
            for (int i = 0; i < dat1.Length && i < dat2.Length; i++)
                o[i] = (byte)(dat1[i] ^ dat2[i]);
            return o;
        }

        private void LoadPrograms()
        {
            Console.WriteLine("Checking programs...");

            bool missing = false;

            //Check if everything is there
            if (File.Exists(Properties.Settings.Default.BinEditorPath))
                BinEditor = new ProcessStartInfo(Properties.Settings.Default.BinEditorPath) { UseShellExecute = false };
            else
            {
                Console.WriteLine("Failed to find Bin Editor (" + Properties.Settings.Default.BinEditorPath + ")");
                missing = true;
            }

            if (missing)
                Console.WriteLine("One or more programs are missing. Select their location in the settings screen.");
            else
                Console.WriteLine("All programs found.");
        }

        private void statusStrip1_Resize(object sender, EventArgs e)
        {
            toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;
        }

        private byte[] StringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length / 2];
            for (int i = 0; i < str.Length; i += 2)
                bytes[i / 2] = byte.Parse("" + str[i] + str[i + 1], System.Globalization.NumberStyles.HexNumber);
            return bytes;
        }
        private string BytesToString(byte[] bytes)
        {
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
                str += bytes[i].ToString("X2");
            return str;
        }
        private bool CompareBytes(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1.Length != bytes2.Length)
                return false;
            for (int i = 0; i < bytes1.Length; i++)
                if (bytes1[i] != bytes2[i])
                    return false;
            return true;
        }

        void Decompile(string activedir, string targetdir, string arc, bool openProj = true)
        {
            Uri root = new Uri(new DirectoryInfo(activedir).FullName);
            Uri target = new Uri(new DirectoryInfo(targetdir).FullName);

            Dictionary<string, byte[]> cacheDict = new Dictionary<string, byte[]>();
            int prog = 0;

            Console.WriteLine("Importing files...");
            string curdir = activedir;

            DirectoryInfo td = new DirectoryInfo(targetdir);
            targetdir = td.FullName;

            if (arc == null)
            {
                string[] archives = EnumerateFiles(Uri.UnescapeDataString(root.AbsolutePath), SearchOption.AllDirectories, "*.szs", "*.arc").ToArray();
                foreach (string file in archives)
                {
                    FileInfo fi = new FileInfo(file);
                    Uri currentFile = new Uri(file);
                    Uri relative = root.MakeRelativeUri(currentFile);
                    curdir = "./" + Uri.UnescapeDataString(relative.OriginalString).Replace(fi.Name, "").Replace("data/", "/") + "/";

                    prog++;
                    UpdateTaskProgress(prog * 1000 / archives.Length);
                    if (fi.Extension == ".szs")
                    {
                        string name = fi.Name.Replace(".szs", "");
                        
                        if (!Directory.Exists(targetdir + curdir))
                            Directory.CreateDirectory(targetdir + curdir);

                        Console.WriteLine("Processing: " + curdir + name);

                        GCN.Yaz0Dec(file, targetdir + curdir + name + ".arc");

                        if (Directory.Exists(targetdir + curdir + name))
                            DeleteDirectoryRec(targetdir + curdir + name);

                        GCN.ExtractArc(targetdir + curdir + name + ".arc", targetdir + curdir + name);

                        cacheDict.Add(curdir + name, GetFolderHash(targetdir + curdir + name));

                        File.Delete(targetdir + curdir + name + ".arc");
                    }
                    else
                    {
                        string name = fi.Name.Replace(".arc", "");
                        Console.WriteLine("Processing: " + curdir + name);

                        if (!Directory.Exists(targetdir + curdir))
                            Directory.CreateDirectory(targetdir + curdir);
                        if (Directory.Exists(targetdir + curdir + name))
                            DeleteDirectoryRec(targetdir + curdir + name);

                        GCN.ExtractArc(file, targetdir + curdir + name);

                        cacheDict.Add(curdir + name, GetFolderHash(targetdir + curdir + name));
                    }
                }
                Console.WriteLine("All szs archives processed.");
            }
            else
            {
                FileInfo fi = new FileInfo(arc);

                Uri currentFile = new Uri(fi.FullName);
                Uri relative = currentFile.MakeRelativeUri(root);
                curdir = "./" + Uri.UnescapeDataString(relative.OriginalString).Replace(fi.Name, "").Replace("data/", "/") + "/";

                if (fi.Extension == ".szs")
                {
                    string name = fi.Name.Replace(".szs", "");

                    if (!Directory.Exists(targetdir + curdir))
                        Directory.CreateDirectory(targetdir + curdir);

                    Console.WriteLine("Processing: " + curdir + name);

                    GCN.Yaz0Dec(arc, targetdir + curdir + name + ".arc");

                    if (Directory.Exists(targetdir + curdir + name))
                        DeleteDirectoryRec(targetdir + curdir + name);

                    GCN.ExtractArc(targetdir + curdir + name + ".arc", targetdir + curdir + name);

                    cacheDict.Add(curdir + name, GetFolderHash(targetdir + curdir + name));

                    File.Delete(targetdir + curdir + name + ".arc");
                }
                else
                {
                    string name = fi.Name.Replace(".arc", "");
                    curdir = fi.Directory.FullName.Replace(activedir, "") + "/";

                    Console.WriteLine("Processing: " + curdir + name);

                    if (!Directory.Exists(targetdir + curdir))
                        Directory.CreateDirectory(targetdir + curdir);
                    if (Directory.Exists(targetdir + curdir + name))
                        DeleteDirectoryRec(targetdir + curdir + name);

                    GCN.ExtractArc(arc, targetdir + curdir + name);
                    cacheDict.Add(curdir + name, GetFolderHash(targetdir + curdir + name));
                }
            }

            //StreamWriter sw = new StreamWriter(new FileStream(ProjectPath + "\\readme.txt", FileMode.Create));
            //sw.Write("This is your SMS Project Folder.\r\n\r\nThe \"root\" directory contains the contents of the disc that you can edit and write back to an iso using an external tool. The \"edit\" directory contains the contents of archives that the SMS Manager can export back into the archives in the \"root\" directory.");
            //sw.Close();
            //Console.WriteLine("Generated Readme.");

            if (openProj)
            {
                UpdateTaskName("Opening Project");
                UpdateTaskProgress(0);
                Open(targetdir + "/..");
            }
        }
        void Compile(string activedir, string targetdir, string arc, bool szs = false)
        {
            Console.Write("Exporting to ");
            if (szs)
                Console.WriteLine(".szs files...");
            else
                Console.WriteLine(".arc files...");

            string curdir = activedir;

            DirectoryInfo td = new DirectoryInfo(targetdir);
            targetdir = td.FullName;

            UpdateTaskProgress(0);
            CheckAllDirectories();
            if (arc == null)
            {
                int prog = 0;

                List<string> dirs = new List<string>();
                dirs.AddRange(Directory.EnumerateDirectories(targetdir).ToArray());
                dirs.AddRange(Directory.EnumerateDirectories(targetdir + "\\scene\\").ToArray());
                foreach (string path in dirs)
                {
                    prog++;
                    UpdateTaskProgress(prog * 1000 / dirs.Count);

                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Name == "scene")
                        continue;
                    curdir = di.FullName.Replace(targetdir, "").Replace(di.Name, "");
                    string target = Directory.EnumerateDirectories(path).ToArray()[0];

                    DirectoryInfo tmp = new DirectoryInfo(target);
                    string pth = activedir + "\\" + curdir + "\\" + di.Name;

                    if (!CheckIfDirectoryChanged(path) || ((!szs && !File.Exists(pth + ".arc")) && (szs && !File.Exists(pth + ".szs"))))
                        continue;

                    Console.WriteLine("Processing: " + di.Name);

                    if (File.Exists(pth + ".arc"))
                        File.Delete(pth + ".arc");
                    if (File.Exists(pth + ".szs"))
                        File.Delete(pth + ".szs");

                    GCN.CreateArc(target, pth + ".arc");

                    if (szs)
                    {
                        if (!Directory.Exists(activedir + "\\" + curdir + "\\"))
                            Directory.CreateDirectory(activedir + "\\" + curdir + "\\");

                        GCN.Yaz0Enc(pth + ".arc", pth + ".szs");

                        File.Delete(pth + ".arc");

                        if (directory != null)
                        {
                            FileInfo fi = new FileInfo(pth + ".szs");
                            directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".szs", di.Name + ".szs", (uint)fi.Length);
                            if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".arc"))
                                directory.RemoveFile("data" + curdir + "\\" + di.Name + ".arc");
                        }
                    }
                    else if (directory != null)
                    {
                        FileInfo fi = new FileInfo(pth + ".arc");
                        directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".arc", di.Name + ".arc", (uint)fi.Length);
                        if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".szs"))
                            directory.RemoveFile("data" + curdir + "\\" + di.Name + ".szs");
                    }
                }
                Console.WriteLine("All archives processed.");
            }
            else
            {
                string path = arc;
                DirectoryInfo di = new DirectoryInfo(path);
                curdir = di.FullName.Replace(targetdir, "").Replace(di.Name, "");
                string target = Directory.EnumerateDirectories(path).ToArray()[0];
                DirectoryInfo tmp = new DirectoryInfo(target);

                string pth = activedir + "\\" + curdir + "\\" + di.Name;
                //cacheDict[arc] = GetFolderHash(path);

                Console.Write("Processing: " + di.Name);

                if (File.Exists(pth + ".arc"))
                    File.Delete(pth + ".arc");
                if (File.Exists(pth + ".szs"))
                    File.Delete(pth + ".szs");

                GCN.CreateArc(target, pth + ".arc");

                if (szs)
                {
                    if (!Directory.Exists(activedir + "\\" + curdir + "\\"))
                        Directory.CreateDirectory(activedir + "\\" + curdir + "\\");

                    GCN.Yaz0Enc(pth + ".arc", pth + ".szs", delegate (int i)
                    {
                    });

                    File.Delete(pth + ".arc");

                    if (directory != null)
                    {
                        FileInfo fi = new FileInfo(pth + ".szs");
                        directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".szs", di.Name + ".szs", (uint)fi.Length);
                        if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".arc"))
                            directory.RemoveFile("data" + curdir + "\\" + di.Name + ".szs");
                    }
                }
                else if (directory != null)
                {
                    FileInfo fi = new FileInfo(pth + ".arc");
                    directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".arc", di.Name + ".arc", (uint)fi.Length);
                    if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".szs"))
                        directory.RemoveFile("data" + curdir + "\\" + di.Name + ".arc");
                }
                Console.WriteLine(" ... Done!");
            }

            if (directory != null)
                directory.Save(ProjectPath + "\\root\\&&systemdata\\Game.toc");

            SaveDateCache();
            ClearFileChangedCache();
        }

        void ReadDateCache()
        {
            dateDict.Clear();
            try
            {
                using (StreamReader fs = new StreamReader(ProjectPath + "/projinfo.dat"))
                {
                    while (!fs.EndOfStream)
                    {
                        string path = fs.ReadLine();
                        long date = BitConverter.ToInt64(Convert.FromBase64String(fs.ReadLine()), 0);
                        dateDict.Add(path, date);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error reading file cache. Next export will run for all archives.");
            }
        }
        void SaveDateCache()
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(ProjectPath + "/projinfo.dat"))
                {
                    foreach (KeyValuePair<string, long> kvp in dateDict)
                    {
                        fs.WriteLine(kvp.Key);
                        fs.WriteLine(Convert.ToBase64String(BitConverter.GetBytes(kvp.Value)));
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error saving file cache. Next export will run for all archives.");
                if (File.Exists(ProjectPath + "/projinfo.dat"))
                    File.Delete(ProjectPath + "/projinfo.dat");
            }
        }
        void CheckAllDirectories()
        {
            var cachedfiles = new List<string>(dateDict.Keys);
            foreach (var file in cachedfiles)
            {
                FileInfo fi = new FileInfo(ProjectPath + "/" + file);
                if (!fi.Exists)
                {
                    dateDict.Remove(file);
                    SetFileChanged(file);
                }
                else
                {
                    long date = fi.LastWriteTime.Ticks;
                    if (dateDict[file] != date)
                    {
                        dateDict[file] = date;
                        SetFileChanged(ProjectPath + "/" + file);
                    }
                }
            }
        }
        bool CheckIfDirectoryChanged(string directory)
        {
            Uri path = new Uri(ProjectPath);
            Uri diruri = new Uri(directory);
            Uri duri = path.MakeRelativeUri(new Uri(directory));
            IEnumerable<string> files = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories);
            bool changed = false;
            foreach (string file in files)
            {
                Uri fileUri = path.MakeRelativeUri(new Uri(file));
                string filepath = Uri.UnescapeDataString(fileUri.OriginalString).Replace(ProjectName, "");
                if (!dateDict.ContainsKey(filepath))
                {
                    long date = File.GetLastWriteTime(file).Ticks;
                    dateDict.Add(filepath, date);
                    changed = true;
                }
            }
            if (changed)
                SetFileChanged(directory);
            else if (!FilesChanged.ContainsKey(path.MakeRelativeUri(diruri).OriginalString.Replace(ProjectName, "")))
                FilesChanged.Add(path.MakeRelativeUri(diruri).OriginalString.Replace(ProjectName, ""), false);
            return FilesChanged[path.MakeRelativeUri(diruri).OriginalString.Replace(ProjectName, "")];
        }
        void SetFileChanged(string path)
        {
            Uri proj = new Uri(ProjectPath);
            Uri uri = new Uri(path);

            List<string> dirs = new List<string>();
            dirs.AddRange(Directory.EnumerateDirectories(ProjectPath + "\\edit").ToArray());
            dirs.AddRange(Directory.EnumerateDirectories(ProjectPath + "\\edit\\scene").ToArray());

            foreach (string path2 in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(path2);
                if (di.Name == "scene")
                    continue;

                Uri uri2 = new Uri(path2);
                if (uri2.OriginalString.StartsWith(uri.OriginalString))
                {
                    Uri relative = proj.MakeRelativeUri(uri2);
                    string key = relative.OriginalString.Replace(ProjectName, "");
                    if (!FilesChanged.ContainsKey(key))
                        FilesChanged.Add(key, true);
                    else
                        FilesChanged[key] = true;
                    break;
                }
            }
        }
        void ClearFileChangedCache()
        {
            var keys = new List<string>(FilesChanged.Keys);
            foreach (var k in keys)
                FilesChanged[k] = false;
        }
        /*void Compile(string activedir, string targetdir, string arc, bool szs = false)
        {
            Console.Write("Exporting to ");
            if (szs)
                Console.WriteLine(".szs files...");
            else
                Console.WriteLine(".arc files...");

            string curdir = activedir;

            DirectoryInfo td = new DirectoryInfo(targetdir);
            targetdir = td.FullName;

            //Dictionary<string, byte[]> cacheDict = ReadHashes();

            if (arc == null)
            {
                int prog = 0;

                List<string> dirs = new List<string>();
                dirs.AddRange(Directory.EnumerateDirectories(targetdir).ToArray());
                dirs.AddRange(Directory.EnumerateDirectories(targetdir + "\\scene\\").ToArray());
                foreach (string path in dirs)
                {
                    prog++;
                    UpdateTaskProgress(prog * 1000 / dirs.Count);

                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Name == "scene")
                        continue;
                    curdir = di.FullName.Replace(targetdir, "").Replace(di.Name, "");
                    string target = Directory.EnumerateDirectories(path).ToArray()[0];
                    DirectoryInfo tmp = new DirectoryInfo(target);

                    string pth = activedir + "\\" + curdir + "\\" + di.Name;
                    //if (cacheDict.ContainsKey(curdir + "\\" + di.Name))
                    //    if (CompareBytes(cacheDict[curdir + "\\" + di.Name], GetFolderHash(target)) && !(szs && File.Exists(pth + ".arc")))
                    //        continue;

                    //cacheDict[curdir + "\\" + di.Name] = GetFolderHash(target);

                    Console.WriteLine("Processing: " + di.Name);

                    if (File.Exists(pth + ".arc"))
                        File.Delete(pth + ".arc");
                    if (File.Exists(pth + ".szs"))
                        File.Delete(pth + ".szs");

                    GCN.CreateArc(target, pth + ".arc");

                    if (szs)
                    {
                        if (!Directory.Exists(activedir + "\\" + curdir + "\\"))
                            Directory.CreateDirectory(activedir + "\\" + curdir + "\\");

                        GCN.Yaz0Enc(pth + ".arc", pth + ".szs", delegate(int i)
                        {
                            double cur = prog * 1000.0 / dirs.Count;
                            double next = (prog+1) * 1000.0 / dirs.Count;
                            UpdateTaskProgress((prog * 1000 / dirs.Count) + (int)(i / 1000.0 * (next - cur)));
                        });

                        File.Delete(pth + ".arc");

                        if (directory != null)
                        {
                            FileInfo fi = new FileInfo(pth + ".szs");
                            directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".szs", di.Name + ".szs", (uint)fi.Length);
                            if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".arc"))
                                directory.RemoveFile("data" + curdir + "\\" + di.Name + ".arc");
                        }
                    }
                    else if (directory != null)
                    {
                        FileInfo fi = new FileInfo(pth + ".arc");
                        directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".arc", di.Name + ".arc", (uint)fi.Length);
                        if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".szs"))
                            directory.RemoveFile("data" + curdir + "\\" + di.Name + ".szs");
                    }
                }
                Console.WriteLine("All archives processed.");
            }
            else
            {
                string path = arc;
                DirectoryInfo di = new DirectoryInfo(path);
                curdir = di.FullName.Replace(targetdir + "data\\", "").Replace(di.Name, "");
                string target = Directory.EnumerateDirectories(path).ToArray()[0];
                DirectoryInfo tmp = new DirectoryInfo(target);

                string pth = activedir + "\\" + curdir + "\\" + di.Name;
                //cacheDict[arc] = GetFolderHash(path);

                Console.Write("Processing: " + di.Name);

                if (File.Exists(pth + ".arc"))
                    File.Delete(pth + ".arc");
                if (File.Exists(pth + ".szs"))
                    File.Delete(pth + ".szs");

                GCN.CreateArc(target, pth + ".arc");

                if (szs)
                {
                    if (!Directory.Exists(activedir + "\\" + curdir + "\\"))
                        Directory.CreateDirectory(activedir + "\\" + curdir + "\\");

                    GCN.Yaz0Enc(pth + ".arc", pth + ".szs", delegate (int i)
                    {
                        UpdateTaskProgress(i * 10);
                    });

                    File.Delete(pth + ".arc");

                    if (directory != null)
                    {
                        FileInfo fi = new FileInfo(pth + ".szs");
                        directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".szs", di.Name + ".szs", (uint)fi.Length);
                        if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".arc"))
                            directory.RemoveFile("data" + curdir + "\\" + di.Name + ".szs");
                    }
                }
                else if (directory != null)
                {
                    FileInfo fi = new FileInfo(pth + ".arc");
                    directory.UpdateFileSize("data" + curdir + "\\" + di.Name + ".arc", di.Name + ".arc", (uint)fi.Length);
                    if (directory.ContainsFile("data" + curdir + "\\" + di.Name + ".szs"))
                        directory.RemoveFile("data" + curdir + "\\" + di.Name + ".arc");
                }
                Console.WriteLine(" ... Done!");
            }

            if (directory != null)
                directory.Save(ProjectPath + "\\root\\&&systemdata\\Game.toc");

            //SaveHashes(cacheDict);
        }*/
        void CompileAndRun(string activedir, string targetdir, string arc, bool szs = false)
        {
            Compile(activedir, targetdir, arc, szs);
            TestProject();
        }

        static void DeleteDirectoryRec(string path)
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
                DeleteDirectoryRec(dir);

            IEnumerable<string> files = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
                File.Delete(file);

            Directory.Delete(path);
        }

        static IEnumerable<string> EnumerateFiles(string dir, SearchOption searchOption, params string[] patterns)
        {
            List<string> files = new List<string>();
            foreach (string pattern in patterns)
                files.AddRange(Directory.EnumerateFiles(dir, pattern, searchOption));
            return files.AsEnumerable<string>();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectWizard npw = new NewProjectWizard();
            if (npw.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            StartMultiTasking("Building New Project", Decompile, npw.ProjectPath + "\\root\\data", npw.ProjectPath + "\\edit", (string)null, true);
        }

        #region Multitasking
        private void StartMultiTasking(string action, Action f)
        {
            if (CurrentAction != null)
                throw new Exception("Too many tasks");
            fileSystemWatcher1.EnableRaisingEvents = false;
            toolStripStatusLabel1.Text = CurrentAction = action;
            toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;

            menuStrip1.Enabled = false;
            tabControl1.Enabled = false;
            fileTree.Enabled = false;
            rootTree.Enabled = false;
            sceneTree.Enabled = false;

            new Thread(delegate()
            {
                f();
                EndMultiTasking();
            }).Start();
        }
        private void StartMultiTasking<T>(string action, Action<T> f, T p1)
        {
            if (CurrentAction != null)
                throw new Exception("Too many tasks");
            fileSystemWatcher1.EnableRaisingEvents = false;
            toolStripStatusLabel1.Text = CurrentAction = action;
            toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;

            menuStrip1.Enabled = false;
            tabControl1.Enabled = false;
            fileTree.Enabled = false;
            rootTree.Enabled = false;
            sceneTree.Enabled = false;

            new Thread(delegate()
            {
                f(p1);
                EndMultiTasking();
            }).Start();
        }
        private void StartMultiTasking<T, U>(string action, Action<T, U> f, T p1, U p2)
        {
            if (CurrentAction != null)
                throw new Exception("Too many tasks");
            fileSystemWatcher1.EnableRaisingEvents = false;
            toolStripStatusLabel1.Text = CurrentAction = action;
            toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;

            menuStrip1.Enabled = false;
            tabControl1.Enabled = false;
            fileTree.Enabled = false;
            rootTree.Enabled = false;
            sceneTree.Enabled = false;

            new Thread(delegate()
            {
                f(p1, p2);
                EndMultiTasking();
            }).Start();
        }
        private void StartMultiTasking<T,U,V>(string action, Action<T,U,V> f, T p1, U p2, V p3)
        {
            if (CurrentAction != null)
                throw new Exception("Too many tasks");
            fileSystemWatcher1.EnableRaisingEvents = false;
            toolStripStatusLabel1.Text = CurrentAction = action;
            toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;

            menuStrip1.Enabled = false;
            tabControl1.Enabled = false;
            fileTree.Enabled = false;
            rootTree.Enabled = false;
            sceneTree.Enabled = false;

            new Thread(delegate()
            {
                f(p1, p2, p3);
                EndMultiTasking();
            }).Start();
        }
        private void StartMultiTasking<T, U, V, W>(string action, Action<T, U, V, W> f, T p1, U p2, V p3, W p4)
        {
            if (CurrentAction != null)
                throw new Exception("Too many tasks");
            fileSystemWatcher1.EnableRaisingEvents = false;
            toolStripStatusLabel1.Text = CurrentAction = action;
            toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;

            menuStrip1.Enabled = false;
            tabControl1.Enabled = false;
            fileTree.Enabled = false;
            rootTree.Enabled = false;
            sceneTree.Enabled = false;

            new Thread(delegate()
            {
                f(p1, p2, p3, p4);
                EndMultiTasking();
            }).Start();
        }
        private void UpdateTaskProgress(int prog)
        {
            MethodInvoker invoker = delegate { toolStripProgressBar1.Value = prog; };
            statusStrip1.BeginInvoke(invoker);
        }
        private void UpdateTaskName(string action)
        {
            CurrentAction = action;
            MethodInvoker invoker = delegate {
                toolStripStatusLabel1.Text = CurrentAction;
                toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;
            };
            statusStrip1.BeginInvoke(invoker);

        }
        private void EndMultiTasking()
        {
            CurrentAction = null;
            MethodInvoker action = delegate { 
                toolStripStatusLabel1.Text = "Ready";
                toolStripProgressBar1.Width = statusStrip1.Width - toolStripStatusLabel1.Width - 32;
            }; statusStrip1.BeginInvoke(action);
            
            MethodInvoker action2 = delegate
            {
                menuStrip1.Enabled = true;
                tabControl1.Enabled = true;
                fileTree.Enabled = true;
                rootTree.Enabled = true;
                sceneTree.Enabled = true;
                fileSystemWatcher1.EnableRaisingEvents = true;
            }; this.BeginInvoke(action2);
        }
        #endregion

        private void Open(string path)
        {
            if (!Directory.Exists(path + "\\edit"))
            {
                MethodInvoker fsactioncr = delegate
                {
                    fileSystemWatcher1.EnableRaisingEvents = false;
                }; this.BeginInvoke(fsactioncr);

                MessageBox.Show("Invalid Project directory.");
                Console.WriteLine("Failed to load project.");
                return;
            }

            LoadedProject = true;
            ProjectPath = path;
            ProjectName = new DirectoryInfo(path).Name;

            MethodInvoker fsaction = delegate
            {
                fileSystemWatcher1.Path = ProjectPath;
                fileSystemWatcher1.EnableRaisingEvents = true;
            }; this.BeginInvoke(fsaction);

            DirectoryInfo di = new DirectoryInfo(ProjectPath);
            Console.WriteLine("Loading Project \"" + di.Name + "\" (" + ProjectPath + ")");

            UpdateTaskProgress(100);
            Console.Write("Reading StageArc.bin...");
            try { StageArc = new SMSScene(path + "\\root\\data\\StageArc.bin"); Console.WriteLine("Loaded!"); }
            catch { Console.WriteLine("StageArc.bin is either missing or corrupted."); }

            UpdateTaskProgress(300);
            Console.WriteLine("Building Edit Tree...");
            TreeNode[] fs = BuildDirTree(ProjectPath + "\\edit");
            UpdateTaskProgress(600);
            Console.WriteLine("Building Root Tree...");
            TreeNode[] rt = BuildDirTree(ProjectPath + "\\root");
            UpdateTaskProgress(700);
            Console.WriteLine("Building Scene Tree...");
            TreeNode[] sn = LoadStageInfo();
            Console.WriteLine("Trees built.");
            UpdateTaskProgress(800);
            Console.WriteLine("Loading Game.toc...");
            directory = new TOCFile(ProjectPath + "\\root\\&&systemdata\\Game.toc");
            Console.WriteLine("Loaded.");

            ReadDateCache();
            FilesChanged.Clear();

            MethodInvoker action = delegate
            {
                UpdateTaskProgress(800);
                fileTree.Nodes.Clear();
                fileTree.Nodes.AddRange(fs);
                UpdateTaskProgress(900);
                rootTree.Nodes.Clear();
                rootTree.Nodes.AddRange(rt);
                UpdateTaskProgress(1000);
                sceneTree.Nodes.Clear();
                sceneTree.Nodes.AddRange(sn);
                UpdateTaskProgress(0);

                Console.WriteLine("Project loaded successfully.");

                projectToolStripMenuItem.Enabled = true;
                editStageArcbinToolStripMenuItem.Enabled = true;
                editSceneCommonToolStripMenuItem.Enabled = true;
                editPerformListsToolStripMenuItem.Enabled = true;
                editStageArcbinToolStripMenuItem.Enabled = true;
                openProjectFolderToolStripMenuItem.Enabled = true;
                testToolStripMenuItem.Enabled = true;

            }; this.BeginInvoke(action);
        }

        private TreeNode[] BuildDirTree(string path)
        {
            List<TreeNode> outtree = new List<TreeNode>();
            IEnumerable<string> editdirs = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
            IEnumerable<string> editfiles = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in editdirs)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                TreeNode tn = new TreeNode(di.Name);
                tn.Tag = dir;
                tn.ImageIndex = 1;
                tn.Nodes.AddRange(BuildDirTree(dir));
                outtree.Add(tn);
            }
            foreach (string file in editfiles)
            {
                FileInfo fi = new FileInfo(file);
                TreeNode tn = new TreeNode(fi.Name);
                tn.Tag = file;
                switch (fi.Extension)
                {
                    case ".bin":
                        tn.ImageIndex = 3;
                        break;
                    default:
                        tn.ImageIndex = 2;
                        break;
                }
                outtree.Add(tn);
            }
            return outtree.ToArray();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory() + "\\Projects";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                StartMultiTasking("Opening Project", Open, folderBrowserDialog1.SelectedPath);
        }

        private TreeNode[] LoadStageInfo()
        {
            List<TreeNode> lvls = new List<TreeNode>();
            try
            {
                Encoding shiftjis = Encoding.GetEncoding("shift-jis");
                ObjectParameters op = new ObjectParameters("ScenarioArchiveName");
                Levels = new string[StageArc.Objects[0].Grouped[0].Grouped.Count][];
                Strings = new GameObject[StageArc.Objects[0].Grouped[0].Grouped.Count][];
                Tables = new GameObject[StageArc.Objects[0].Grouped[0].Grouped.Count];
                for (int i = 0; i < StageArc.Objects[0].Grouped[0].Grouped.Count; i++)
                {
                    Tables[i] = StageArc.Objects[0].Grouped[0].Grouped[i];
                    Levels[i] = new string[Tables[i].Grouped.Count];
                    Strings[i] = new GameObject[Tables[i].Grouped.Count];

                    TreeNode tn = new TreeNode(i + ": " + Tables[i].Description);
                    tn.Tag = Tables[i];
                    tn.ImageIndex = 0;

                    for (int j = 0; j < Tables[i].Grouped.Count; j++)
                    {
                        op.Adjust(Tables[i].Grouped[j]);
                        Levels[i][j] = op.GetParamValue(0, Tables[i].Grouped[j]);
                        Strings[i][j] = Tables[i].Grouped[j];

                        TreeNode tni = new TreeNode(Levels[i][j]);
                        tni.Tag = Strings[i][j];
                        tni.ImageIndex = 1;
                        tn.Nodes.Add(tni);
                    }

                    lvls.Add(tn);
                }
            }
            catch
            {
                Console.WriteLine("Failed to analyze scene information. Is your StageArc corrupted?");
                return new TreeNode[0];
            }

            return lvls.ToArray();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Importing will undo any unexported changes you made to the project. Are you sure you want to import?", "Import", MessageBoxButtons.YesNo) == DialogResult.Yes)
                StartMultiTasking("Building Project", Decompile, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)null, true);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartMultiTasking("Exporting Project", Compile, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)null, false);
        }

        private void compressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Compressing will convert all your archives into .szs files, which will decrease size. This will take anywhere from 30 minutes to a couple hours. Are you sure you want to compress?", "Import", MessageBoxButtons.YesNo) == DialogResult.Yes)
                StartMultiTasking("Compressing Project", Compile, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)null, true);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Settings().ShowDialog();
            LoadPrograms();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

        private void editStrip_Opening(object sender, CancelEventArgs e)
        {
            if (!LoadedProject)
            {
                e.Cancel = true;
                return;
            }
            if (File.Exists((string)fileTree.SelectedNode.Tag) || (fileTree.SelectedNode.Parent != null && fileTree.SelectedNode.Parent.Text == "scene"))
                editToolStripMenuItem1.Enabled = true;
            else
                editToolStripMenuItem1.Enabled = false;
            if (fileTree.SelectedNode.Parent == null || fileTree.SelectedNode.Parent.Text == "scene")
            {
                importToolStripMenuItem1.Enabled = true;
                exportToolStripMenuItem1.Enabled = true;
                compressToolStripMenuItem1.Enabled = true;
            }
            else
            {
                importToolStripMenuItem1.Enabled = false;
                exportToolStripMenuItem1.Enabled = false;
                compressToolStripMenuItem1.Enabled = false;
            }
        }

        private void sceneStrip_Opening(object sender, CancelEventArgs e)
        {
            if (!LoadedProject)
            {
                e.Cancel = true;
                return;
            }
            GameObject go = (GameObject)sceneTree.SelectedNode.Tag;
            if (go.Name == "ScenarioArchiveName")
            {
                editSceneBinToolStripMenuItem.Enabled = true;
                editTableBinToolStripMenuItem.Enabled = true;
            }
            else
            {
                editSceneBinToolStripMenuItem.Enabled = false;
                editTableBinToolStripMenuItem.Enabled = false;
            }
        }

        private void rootStrip_Opening(object sender, CancelEventArgs e)
        {
            if (!LoadedProject)
            {
                e.Cancel = true;
                return;
            }
        }

        private void showInFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/select," + (string)fileTree.SelectedNode.Tag);
        }

        private void gotoFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ScenarioArchiveName");
            GameObject go = (GameObject)sceneTree.SelectedNode.Tag;

            if (go.Name != "ScenarioArchiveName")
                return;

            op.Adjust(go);
            string name = op.GetParamValue(0, go);

            string folder = ProjectPath + "\\edit\\scene\\" + name.Substring(0, name.Length - 4) + "\\scene";
            Process.Start("explorer.exe", folder);
        }

        private void showInFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/select," + (string)rootTree.SelectedNode.Tag);
        }

        private void fileTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) fileTree.SelectedNode = e.Node;
        }

        private void sceneTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) sceneTree.SelectedNode = e.Node;
        }

        private void rootTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) rootTree.SelectedNode = e.Node;
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode.Parent.Text == "scene")
            {
                string file = ProjectPath + "\\edit\\scene\\" + fileTree.SelectedNode.Text + "\\scene\\map\\scene.bin";
                BinEditor.Arguments = GetBinEditorArguments(file);
                Process.Start(BinEditor);
                return;
            }
            if (File.Exists((string)fileTree.SelectedNode.Tag))
            {
                string file = ((string)fileTree.SelectedNode.Tag);
                switch (file.Substring(file.Length - 4, 4))
                {
                    case ".bin":
                        if (BinEditor == null)
                            Console.WriteLine("Bin Editor cannot be found.");
                        BinEditor.Arguments = GetBinEditorArguments(file);
                        Process.Start(BinEditor);

                        Console.WriteLine(BinEditor.FileName + " " + BinEditor.Arguments);
                        break;
                    default:
                        Process.Start("explorer.exe", (string)fileTree.SelectedNode.Tag);
                        break;
                }
            }
        }

        private string GetBinEditorArguments(string file, string extra = "")
        {
            string arg = "";
            if (file != null)
            {
                if (File.Exists(file))
                {
                    arg += "-file \"" + file + "\" ";
                }
            }
            return arg + "-stagearc \"" + ProjectPath + "\\root\\data\\stageArc.bin\" -params \"" + ProjectPath + "\\edit\\params\\params\" -common \"" + ProjectPath + "\\edit\\common\\common\" " + extra;
        }

        private void editSceneBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ScenarioArchiveName");
            GameObject go = (GameObject)sceneTree.SelectedNode.Tag;

            if (go.Name != "ScenarioArchiveName")
                return;

            op.Adjust(go);
            string name = op.GetParamValue(0, go);

            string file = ProjectPath + "\\edit\\scene\\" + name.Substring(0, name.Length - 4) + "\\scene\\map\\scene.bin";
            BinEditor.Arguments = GetBinEditorArguments(file);
            Process.Start(BinEditor);
        }

        private void editTableBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectParameters op = new ObjectParameters("ScenarioArchiveName");
            GameObject go = (GameObject)sceneTree.SelectedNode.Tag;

            if (go.Name != "ScenarioArchiveName")
                return;

            op.Adjust(go);
            string name = op.GetParamValue(0, go);

            string file = ProjectPath + "\\edit\\scene\\" + name.Substring(0, name.Length - 4) + "\\scene\\map\\tables.bin";
            BinEditor.Arguments = GetBinEditorArguments(file);
            Process.Start(BinEditor);
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            Console.Write("Filesystem change detected...");
            fileSystemWatcher1.EnableRaisingEvents = false;
            if (e.FullPath == ProjectPath + "\\root\\data\\stageArc.bin")
            {
                sceneTree.Nodes.Clear();
                sceneTree.Nodes.AddRange(LoadStageInfo());
                Console.WriteLine("Updated scene list.");
            }
            else
            {
                Console.WriteLine("Updating trees...");

                if (e.FullPath.Contains(ProjectPath + "\\edit")){
                    Console.WriteLine("Building Edit Tree...");
                    TreeNode[] fs = BuildDirTree(ProjectPath + "\\edit");

                    fileTree.Nodes.Clear();
                    fileTree.Nodes.AddRange(fs);
                }
                else if (e.FullPath.Contains(ProjectPath + "\\root")){
                    Console.WriteLine("Building Root Tree...");
                    TreeNode[] rt = BuildDirTree(ProjectPath + "\\root");

                    rootTree.Nodes.Clear();
                    rootTree.Nodes.AddRange(rt);
                }
                Console.WriteLine("Trees updated.");

                SetFileChanged(e.FullPath);
            }
            Console.WriteLine("Done.");
            fileSystemWatcher1.EnableRaisingEvents = true;
        }

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StartMultiTasking("Building Project", Decompile, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)fileTree.SelectedNode.Tag, true);
        }

        private void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StartMultiTasking("Building Project", Compile, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)fileTree.SelectedNode.Tag, false);
        }

        private void compressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StartMultiTasking("Building Project", Compile, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)fileTree.SelectedNode.Tag, true);
        }

        static Thread editStageArcbinToolStripMenuItem_Click_ProcessThread;
        private void editStageArcbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editStageArcbinToolStripMenuItem_Click_ProcessThread != null && editStageArcbinToolStripMenuItem_Click_ProcessThread.IsAlive)
                editStageArcbinToolStripMenuItem_Click_ProcessThread.Abort();
            BinEditor.Arguments = GetBinEditorArguments(ProjectPath + "\\root\\data\\stageArc.bin");
            editStageArcbinToolStripMenuItem_Click_ProcessThread = new Thread(new ThreadStart(delegate()
            {
                Process.Start(BinEditor).WaitForExit();
                treeChanged(this, new EventArgs());

                try { StageArc = new SMSScene(ProjectPath + "\\root\\data\\StageArc.bin"); Console.WriteLine("Loaded!"); }
                catch { Console.WriteLine("StageArc.bin is either missing or corrupted."); }
            }));
            editStageArcbinToolStripMenuItem_Click_ProcessThread.IsBackground = true;
            editStageArcbinToolStripMenuItem_Click_ProcessThread.Start();
        }

        private void treeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Building scene tree...");
            TreeNode[] sn = LoadStageInfo();
            Console.WriteLine("Trees built.");

            MethodInvoker action = delegate
            {
                sceneTree.Nodes.Clear();
                sceneTree.Nodes.AddRange(sn);
            };
            sceneTree.Invoke(action);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (exportTestToolStripMenuItem1.Checked)
                exportTestToolStripMenuItem1_Click(sender, e);
            else if (comToolStripMenuItem.Checked)
                comToolStripMenuItem_Click(sender, e);
            else if (compressTestToolStripMenuItem.Checked)
                compressTestToolStripMenuItem_Click(sender, e);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openProjectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", ProjectPath);
        }

        public void ConfigureDolphin(string path, string root)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!Directory.Exists(path + "/Config"))
                Directory.CreateDirectory(path + "/Config");

            List<string> vars;
            if (File.Exists(path + "/Config/Dolphin.ini"))
            {
                string conf;
                using (StreamReader sr = new StreamReader(new FileStream(path + "/Config/Dolphin.ini", FileMode.Open))) { conf = sr.ReadToEnd(); }

                bool droot = false;
                bool apploader = false;
                string section = "";
                vars = new List<string>(conf.Split('\n', '\r'));
                for (int i = 0; i < vars.Count; i++)
                {
                    if (vars[i].Length == 0)
                        continue;
                    if (vars[i][0] == '[')
                    {
                        if (section == "[Core]")
                        {
                            if (!droot)
                            {
                                vars.Insert(i++, "DVDRoot = " + root);
                                droot = true;
                            }
                            if (!apploader)
                            {
                                vars.Insert(i++, "Apploader = " + root + "\\&&systemdata\\AppLoader.ldr");
                                apploader = true;
                            }
                        }
                        section = vars[i];
                        continue;
                    }
                    if (section == "[Core]")
                    {
                        if (vars[i].Contains("DVDRoot"))
                        {
                            vars[i] = "DVDRoot = " + root;
                            droot = true;
                        }
                        else if (vars[i].Contains("Apploader"))
                        {
                            vars[i] = "Apploader = " + root + "\\&&systemdata\\AppLoader.ldr";
                            apploader = true;
                        }
                    }
                }
                if (!droot || !apploader)
                {
                    vars.Add("[Core]");
                    vars.Add("DVDRoot = " + root);
                    vars.Add("Apploader = " + root + "\\&&systemdata\\AppLoader.ldr");
                }
            }
            else
                vars = new List<string>(new string[2] { "DVDRoot = " + root, "DVDRoot = " + root + "\\&&systemdata\\AppLoader.ldr" });

            StreamWriter sw = new StreamWriter(new FileStream(path + "/Config/Dolphin.ini", FileMode.Create));
            foreach (string var in vars)
                sw.WriteLine(var);
            sw.Close();
        }
        public void TestProject()
        {
            if (ProjectPath == null)
                return;

            ConfigureDolphin("./dolphinconf", ProjectPath + "/root");

            Process.Start(Properties.Settings.Default.DolphinPath, "--user \"" + Directory.GetCurrentDirectory() + "/dolphinconf\" bob" + ProjectPath + "\\root\\&&systemdata\\Start.dol\"" + (Properties.Settings.Default.DolphinDebug ? " -d" : ""));
        }

        private void exportTestToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (comToolStripMenuItem.Checked)
                comToolStripMenuItem.Checked = false;
            else if (compressTestToolStripMenuItem.Checked)
                compressTestToolStripMenuItem.Checked = false;

            TestProject();
        }

        private void comToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (exportTestToolStripMenuItem1.Checked)
                exportTestToolStripMenuItem1.Checked = false;
            else if (compressTestToolStripMenuItem.Checked)
                compressTestToolStripMenuItem.Checked = false;

            StartMultiTasking("Exporting Project", CompileAndRun, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)null, false);
        }

        private void compressTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comToolStripMenuItem.Checked)
                comToolStripMenuItem.Checked = false;
            else if (exportTestToolStripMenuItem1.Checked)
                exportTestToolStripMenuItem1.Checked = false;

            if (MessageBox.Show("Compressing will convert all your archives into .szs files, which will decrease size. This will take anywhere from 30 minutes to a couple hours. Are you sure you want to compress?", "Import", MessageBoxButtons.YesNo) == DialogResult.Yes)
                StartMultiTasking("Compressing Project", CompileAndRun, ProjectPath + "\\root\\data", ProjectPath + "\\edit", (string)null, true);
        }

        private void forceKillDolphinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Taskkill.exe", "-f -IM Dolphin.exe");
        }

        public static void CopyDir(string source, string target)
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(source);
            IEnumerable<string> files = Directory.EnumerateFiles(source);

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            foreach (string dir in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                CopyDir(source + "\\" + di.Name, target + "\\" + di.Name);
            }
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                File.Copy(source + "\\" + fi.Name, target + "\\" + fi.Name);
            }
        }

        private void translateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sceneTree.SelectedNode != null)
                translator.Translate(Translate_Buddy.LANGUAGE_JAPANESE, Translate_Buddy.LANGUAGE_ENGLISH, sceneTree.SelectedNode.Text);
        }

        private void sceneTree_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void editSceneCommonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinEditor.Arguments = GetBinEditorArguments(ProjectPath + "\\root\\data\\scenecmn.bin");
            Process.Start(BinEditor).WaitForExit();
        }

        private void editPerformListsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinEditor.Arguments = GetBinEditorArguments(ProjectPath + "\\root\\data\\PerformLists.bin");
            Process.Start(BinEditor).WaitForExit();
        }

        private void makeMultiplayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileSystemWatcher1.EnableRaisingEvents = false;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = ProjectPath + "\\edit\\scene";
            fbd.Description = "Select shadow mario folder to use in every stage.";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = ProjectPath + "\\edit\\scene";
                MessageBox.Show("Select piantissimo model to use in every stage.");
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                IEnumerable<string> folders = Directory.EnumerateDirectories(ProjectPath + "\\edit\\scene", "*", SearchOption.TopDirectoryOnly);
                foreach (string folder in folders)
                {
                    string name = new DirectoryInfo(folder).Name;
                    if (name == "option")
                        continue;

                    if (fbd.SelectedPath != folder + "\\scene\\kagemario")
                        DirectoryCopy(fbd.SelectedPath, folder + "\\scene\\kagemario", true);
                    if (!Directory.Exists(folder + "\\scene\\map\\map\\pad"))
                        Directory.CreateDirectory(folder + "\\scene\\map\\map\\pad");
                    if (!File.Exists(folder + "\\scene\\map\\map\\pad\\monteman_model.bmd"))
                        File.Copy(ofd.FileName, folder + "\\scene\\map\\map\\pad\\monteman_model.bmd");

                    BinEditor.Arguments = GetBinEditorArguments(folder + "\\scene\\map\\scene.bin", "-makemp");
                    Process.Start(BinEditor);

                    Console.WriteLine("Made " + name + " multiplayer");
                }
            }

            fileSystemWatcher1.EnableRaisingEvents = true;
        }


        //Taken from Microsoft MSDN
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (dir.FullName == new DirectoryInfo(destDirName).FullName)
                return;

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (File.Exists(temppath))
                    File.Delete(temppath);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ProjectPath + "\\root\\&&systemdata\\Start.dol"))
            {
                MessageBox.Show("Unable to find game executable.");
                return;
            }
            FileStream fs = new FileStream(ProjectPath + "\\root\\&&systemdata\\Start.dol", FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(2766539, SeekOrigin.Begin);
            fs.WriteByte(0x09);
            fs.Close();
            MessageBox.Show("Patch complete.");
        }

        private void createPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ProjectPath + "\\root\\&&systemdata\\Start.dol"))
            {
                MessageBox.Show("Unable to find game executable.");
                return;
            }
            FileStream fs = new FileStream(ProjectPath + "\\root\\&&systemdata\\Start.dol", FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(2766539, SeekOrigin.Begin);
            fs.WriteByte(0x04);
            fs.Close();
            MessageBox.Show("Patch complete.");
        }

        private void enableStackTraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(ProjectPath + "\\root\\&&systemdata\\Start.dol", FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(2727932, SeekOrigin.Begin);
            fs.WriteByte(0x60);
            fs.WriteByte(0x00);
            fs.WriteByte(0x00);
            fs.WriteByte(0x00);
            fs.Close();

            if (!directory.ContainsFile("marioUS.MAP") && File.Exists(ProjectPath + "\\root\\marioUS.MAP"))
                directory.AddFile("marioUS.MAP", 7997789);

            MessageBox.Show("Patch complete.");
        }

        private bool CompareFiles(string file1, string file2)
        {
            using (FileStream fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read))
            using (FileStream fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read))
            {
                if (fs1.Length != fs2.Length)
                    return false;

                byte[] buffer1 = new byte[512];
                byte[] buffer2 = new byte[512];
                while (fs1.Position < fs1.Length)
                {
                    int len = (int)Math.Min(fs1.Length - fs1.Position, buffer1.Length);
                    fs1.Read(buffer1, 0, len);
                    fs2.Read(buffer2, 0, len);
                    if (!buffer1.SequenceEqual(buffer2))
                        return false;
                }
                return true;
            }
        }

        private Dictionary<string, string> FileCacheRoot = new Dictionary<string, string>();
        private void CacheFile(string root, string relpath)
        {
            using (FileStream fs = new FileStream(root + relpath, FileMode.Open, FileAccess.Read))
            using (MD5 hasher = MD5.Create())
            {
                string hash = Convert.ToBase64String(hasher.ComputeHash(fs));
                if (!FileCacheRoot.ContainsKey(hash))
                    FileCacheRoot.Add(hash, relpath);
            }
        }
        private string GetFileHash(string root, string relpath)
        {
            using (FileStream fs = new FileStream(root + relpath, FileMode.Open, FileAccess.Read))
            using (MD5 hasher = MD5.Create())
                return Convert.ToBase64String(hasher.ComputeHash(fs));
        }
        private void CreateFilePatch(string file1, string file2, Stream o)
        {
        }

        private void GeneratePatch()
        {
            FileStream fs = new FileStream(ProjectPath + "/patch.smspatch", FileMode.Create, FileAccess.Write);
            fs.Write(new byte[] { (byte)'G', (byte)'P', (byte)'C', 0 }, 0, 4);
            Data.WriteString(fs, "GMSE01");

            if (Directory.Exists("./patchtemp"))
            {
                Console.WriteLine("Deleting old temp directory...");
                DeleteDirectoryRec("./patchtemp");
            }

            Directory.CreateDirectory("./patchtemp");
            Directory.CreateDirectory("./patchtemp/data");

            Console.WriteLine("Generating temp compare directory...");
            Decompile("./root/data", "./patchtemp/data", null);

            Uri patchRoot = new Uri(new Uri(Application.ExecutablePath), "./patchtemp");
            Uri projRoot = new Uri(ProjectPath + "/edit");

            int prog = 0;
            int count = 0;

            Console.WriteLine("Patch pass 1");
            IEnumerable<string> mainFiles = Directory.EnumerateFiles(Uri.UnescapeDataString(patchRoot.AbsolutePath), "*", SearchOption.AllDirectories);
            count = mainFiles.Count();

            foreach (string path in mainFiles)
            {
                UpdateTaskProgress(prog * 9000 / count / 20);

                Uri fullPath = new Uri(new Uri(Application.ExecutablePath), path);
                Uri relPath = patchRoot.MakeRelativeUri(fullPath);
                string rel = Uri.UnescapeDataString(relPath.OriginalString).Replace("patchtemp", "").Replace("/data", "");

                CacheFile(Uri.UnescapeDataString(patchRoot.AbsolutePath), "/data" + rel);

                if (!File.Exists(Uri.UnescapeDataString(projRoot.AbsolutePath) + rel))
                {
                    Console.WriteLine("Delete: " + rel);

                    //Delete file command
                    fs.WriteByte(0);
                    Data.WriteString(fs, Uri.UnescapeDataString(relPath.OriginalString));
                }
                else if (!CompareFiles(Uri.UnescapeDataString(patchRoot.AbsolutePath) + "/data" + rel, Uri.UnescapeDataString(projRoot.AbsolutePath) + rel))
                {
                    //Patch file command
                    fs.WriteByte(1);
                    Data.WriteString(fs, "/data" + rel);

                    //Console.WriteLine("Patch: " + rel);
                    Process p = Process.Start(new ProcessStartInfo("./xdelta.exe")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        Arguments = "-c -s \"" + path + "\" \"" + Uri.UnescapeDataString(projRoot.AbsolutePath).Replace("/data", "") + rel + "\"",
                    });

                    Task.Delay(250);

                    int last;
                    int len = 0;
                    byte[] buf = new byte[1];
                    MemoryStream ms = new MemoryStream();
                    do
                    {
                        last = p.StandardOutput.BaseStream.Read(buf, 0, 1);
                        if (last > 0) ms.WriteByte(buf[0]);
                        len++;
                    } while (last > 0);

                    Data.WriteInt64(fs, ms.Length);
                    fs.Write(ms.ToArray(), 0, (int)ms.Length);
                }
                prog++;
            }

            prog = 0;
            Console.WriteLine("Patch pass 2");
            IEnumerable<string> newFiles = Directory.EnumerateFiles(Uri.UnescapeDataString(projRoot.AbsolutePath), "*", SearchOption.AllDirectories);
            count = newFiles.Count();
            foreach (string path in newFiles)
            {
                UpdateTaskProgress(prog * 9000 / count / 20 + 45);

                Uri fullPath = new Uri(new Uri(Application.ExecutablePath), path);
                Uri relPath = projRoot.MakeRelativeUri(fullPath);
                string rel = Uri.UnescapeDataString(relPath.OriginalString).Replace("edit", "");

                if (!File.Exists(Uri.UnescapeDataString(patchRoot.AbsolutePath) + "/data" + rel))
                {
                    string filecache = GetFileHash(Uri.UnescapeDataString(projRoot.AbsolutePath), rel);
                    if (!FileCacheRoot.ContainsKey(filecache))
                    {
                        Console.WriteLine("Add File: " + rel);
                        using (FileStream newFile = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            //Add file from patch command
                            fs.WriteByte(2);
                            Data.WriteString(fs, "data/" + rel);
                            Data.WriteInt64(fs, newFile.Length);

                            byte[] buffer = new byte[512];
                            while (newFile.Position < newFile.Length)
                            {
                                int d = (int)Math.Min(newFile.Length - newFile.Position, 512);
                                newFile.Read(buffer, 0, d);
                                fs.Write(buffer, 0, d);
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Copy File: " + rel);

                        //Add file from origin command
                        fs.WriteByte(3);
                        Data.WriteString(fs, "/data" + rel);
                        Data.WriteString(fs, FileCacheRoot[filecache]);
                    }
                }
            }

            Console.WriteLine("Cleaning up...");
            if (Directory.Exists("./patchtemp"))
                DeleteDirectoryRec("./patchtemp");
            if (Directory.Exists("./patchtemp2"))
                DeleteDirectoryRec("./patchtemp2");

            Console.WriteLine("Creating system file temp directory...");
            Directory.CreateDirectory("./patchtemp2");
            File.Copy("./root/opening.bnr", "./patchtemp2/opening.bnr");
            DirectoryCopy("./root/&&systemdata", "./patchtemp2/&&systemdata", true);
            DirectoryCopy("./root/AudioRes", "./patchtemp2/AudioRes", true);

            Uri patch2Root = new Uri(new Uri(Application.ExecutablePath), "./patchtemp2");
            Uri proj2Root = new Uri(ProjectPath + "/root");

            prog = 0;
            Console.WriteLine("Patching image files...");
            IEnumerable<string> imageFiles = Directory.EnumerateFiles(Uri.UnescapeDataString(patch2Root.AbsolutePath), "*", SearchOption.AllDirectories);
            count = imageFiles.Count();
            foreach (string path in imageFiles)
            {
                UpdateTaskProgress(prog * 1000 / count / 10 + 90);

                Uri fullPath = new Uri(new Uri(Application.ExecutablePath), path);
                Uri relPath = patch2Root.MakeRelativeUri(fullPath);
                string rel = Uri.UnescapeDataString(relPath.OriginalString).Replace("patchtemp2", "").Replace("/data", "");

                if (path.Contains("/edit/data"))
                    continue;

                //if (!File.Exists(Uri.UnescapeDataString(proj2Root.AbsolutePath) + rel))
                //{
                //    fs.WriteByte(5);
                //    Data.WriteString(fs, rel);

                //    using (FileStream nfile = new FileStream(Uri.UnescapeDataString(patch2Root.AbsolutePath) + rel, FileMode.Open, FileAccess.Read))
                //    {
                //        int last;
                //        int len = 0;
                //        byte[] buf = new byte[1];
                //        MemoryStream ms = new MemoryStream();
                //        do
                //        {
                //            last = nfile.Read(buf, 0, 1);
                //            if (last > 0) ms.WriteByte(buf[0]);
                //            len++;
                //        } while (last > 0);

                //        Data.WriteInt64(fs, ms.Length);
                //        fs.Write(ms.ToArray(), 0, (int)ms.Length);
                //    }
                //}
                //else
                if (!CompareFiles(Uri.UnescapeDataString(patch2Root.AbsolutePath) + rel, Uri.UnescapeDataString(proj2Root.AbsolutePath) + rel))
                {
                    Console.WriteLine("Patch Image: " + rel);

                    //Patch file command
                    fs.WriteByte(4);
                    Data.WriteString(fs, rel);

                    Process p = Process.Start(new ProcessStartInfo("./xdelta.exe")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        Arguments = "-c -s \"" + path + "\" \"" + Uri.UnescapeDataString(proj2Root.AbsolutePath).Replace("/data", "") + rel + "\"",
                    });

                    Task.Delay(250);

                    int last;
                    int len = 0;
                    byte[] buf = new byte[1];
                    MemoryStream ms = new MemoryStream();
                    do
                    {
                        last = p.StandardOutput.BaseStream.Read(buf, 0, 1);
                        if (last > 0) ms.WriteByte(buf[0]);
                        len++;
                    } while (last > 0);

                    Data.WriteInt64(fs, ms.Length);
                    fs.Write(ms.ToArray(), 0, (int)ms.Length);
                }
            }

            Console.WriteLine("Cleaning up...");
            DeleteDirectoryRec(".\\patchtemp2");
            fs.Close();

            Console.WriteLine("Done patching!");
            UpdateTaskProgress(0);
        }

        private void generateISOPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("./xdelta.exe"))
            {
                MessageBox.Show("xdelta.exe is required to create patches.");
                return;
            }
            if (!Directory.Exists("./root") || File.Exists("./root/readme.txt"))
                MessageBox.Show("Cannot create a patch because there is no root extracted to ./root or readme.txt still exists.");
            else
                StartMultiTasking("Generating Patch", GeneratePatch);
        }

        private void insertToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ProjectPath + "\\root\\&&systemdata\\Start.dol"))
            {
                MessageBox.Show("Unable to find game executable.");
                return;
            }
            FileStream fs = new FileStream(ProjectPath + "\\root\\&&systemdata\\Start.dol", FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(2829780, SeekOrigin.Begin);
            fs.WriteByte(0x60);
            fs.WriteByte(0x00);
            fs.WriteByte(0x00);
            fs.WriteByte(0x00);
            fs.Close();
            MessageBox.Show("Patch complete.");
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ProjectPath + "\\root\\&&systemdata\\Start.dol"))
            {
                MessageBox.Show("Unable to find game executable.");
                return;
            }
            FileStream fs = new FileStream(ProjectPath + "\\root\\&&systemdata\\Start.dol", FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(2829780, SeekOrigin.Begin);
            fs.WriteByte(0x41);
            fs.WriteByte(0x82);
            fs.WriteByte(0x00);
            fs.WriteByte(0x20);
            fs.Close();
            MessageBox.Show("Patch complete.");
        }

        private void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilesChanged.Clear();
            dateDict.Clear();
            SaveDateCache();
        }
    }
}
