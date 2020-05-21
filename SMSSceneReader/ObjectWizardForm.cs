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
using SMSReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SMSSceneReader {



    public partial class ObjectWizardForm : Form {
        private List<
                (string name, 
                 Dictionary<string, List<ObjectTemplate>> actors)
                 > categories;

        public Dictionary<string, ObjectTemplate> actorMgrs;
        public List<ObjectTemplate> objects;

        ObjectTemplate selectedObject = null;
        string selectedCategory;
        string selectedActor;
        public bool addObjectClicked = false;
        public bool includeManager;
        public bool includeAssets;

        public ObjectWizardForm() {
            InitializeComponent();
            includeManager = checkBox1.Checked;
            includeAssets = checkBox2.Checked;

            string basePath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string assetPath = Path.Combine(basePath, "GameAssets");
            string templatePath = Path.Combine(basePath, "Templates");
            
            
            Console.WriteLine(assetPath);
            Console.WriteLine(templatePath);

            categories = new List<
                (string name, 
                 Dictionary<string, List<ObjectTemplate>> actors)
                 >();

            actorMgrs = new Dictionary<string, ObjectTemplate>();
            objects = new List<ObjectTemplate>();
            
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(
                    (new TupleConverter())
                );

            var dirList = Directory.GetDirectories(templatePath).ToList<string>();
            dirList.Insert(0, templatePath);
            dirList.Sort();
            foreach (string dir in dirList) {
                Console.WriteLine(dir);
                string categoryName = new DirectoryInfo(dir).Name;
                if (dir == templatePath) {
                    categoryName = "No Category";
                }

                categoryListBox.Items.Add(categoryName);
                Console.WriteLine(categoryName);

                var actorDict = new Dictionary<string, List<ObjectTemplate>>();

                foreach (string actorFile in Directory.GetFiles(dir)) {
                    
                    Console.WriteLine(actorFile);
                    if (Path.GetExtension(actorFile).ToLower() == ".json") {
                        using (TextReader file = File.OpenText(actorFile)) {
                            using (JsonTextReader reader = new JsonTextReader(file)) {
                                Console.WriteLine(actorFile);
                                ObjectTemplate[] tempList = serializer.Deserialize<ObjectTemplate[]>(reader);

                                List<ObjectTemplate> objList;
                                foreach (ObjectTemplate temp in tempList) {
                                    if (actorDict.TryGetValue(temp.type, out objList)) {
                                        objList.Add(temp);
                                    }
                                    else {
                                        objList = new List<ObjectTemplate>();
                                        objList.Add(temp);
                                        actorDict.Add(temp.type, objList);
                                    }

                                    if (!actorMgrs.ContainsKey(temp.type)) {
                                        actorMgrs.Add(temp.type, temp);
                                    }
                                    objects.Add(temp);
                                }
                            }
                        }
                    }
                }
                categories.Add((categoryName, actorDict));

            }
        }

        private void ObjectWizardForm_Load(object sender, EventArgs e) {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            includeManager = checkBox1.Checked;
        }



        private void label1_Click(object sender, EventArgs e) {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            includeAssets = checkBox2.Checked;
        }

        private void categoryListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int index = categoryListBox.SelectedIndex;
            if (index < 0) {
                return;
            }
            var actorDict = categories[index].actors;
            actorListBox.Items.Clear();
            objectListBox.Items.Clear();
            selectedCategory = (string)categoryListBox.SelectedItem;
            selectedObject = null;

            foreach (string actor in actorDict.Keys) {
                actorListBox.Items.Add(actor);
            }
        }

        private void actorListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int index = categoryListBox.SelectedIndex;
            if (index < 0) {
                return;
            }
            var actorDict = categories[index].actors;

            string actorName = (string)actorListBox.SelectedItem;
            if (actorName == null) {
                return;
            }

            selectedActor = actorName;
            objectListBox.Items.Clear();
            selectedObject = null;

            List<ObjectTemplate> objList;
            if (actorDict.TryGetValue(actorName, out objList)) {
                foreach (ObjectTemplate temp in objList) {
                    objectListBox.Items.Add(temp.name);
                }
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e) {
            int index = categoryListBox.SelectedIndex;
            if (index < 0 | objectListBox.SelectedIndex < 0) {
                return;
            }
            var actorDict = categories[index].actors;

            string actorName = (string)actorListBox.SelectedItem;
            if (actorName == null) {
                return;
            }

            List<ObjectTemplate> objList;
            if (actorDict.TryGetValue(actorName, out objList)) {
                selectedObject = objList[objectListBox.SelectedIndex];
            }
        }

        public (string category, string actor, ObjectTemplate template, bool insertMgr, bool insertAsset) backupState() {
            return (selectedCategory, selectedActor, selectedObject, includeManager, includeAssets);
        }

        public void restoreState((string, string, ObjectTemplate, bool, bool) state) {
            checkBox1.Checked = state.Item4;
            checkBox2.Checked = state.Item5;

            if (state.Item3 == null) {
                return;
            }

            categoryListBox.SelectedItem = state.Item1;
            actorListBox.SelectedItem = state.Item2;
            objectListBox.SelectedItem = state.Item3.name;
            
        }

        private void button1_Click(object sender, EventArgs e) {
            if (selectedObject != null) {
                addObjectClicked = true;
                Close();
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }
    }

    public class ObjectTemplate {
        public string name;
        public string type;
        public string objkey;
        public string manager;
        public (string type, object value)[] data;
        public string[] assets;

        public ObjectTemplate() {
        
        }

        /*public ObjectTemplate(string path) {
            JsonSerializer serializer = new JsonSerializer();
            using (TextReader file = File.OpenText(path)) {
                using (JsonTextReader reader = new JsonTextReader(file)) {
                    ObjectTemplate temp = serializer.Deserialize<ObjectTemplate>(reader);
                    
                    name = temp.name;
                    type = temp.type;
                    objkey = temp.objkey;
                    manager = temp.manager;
                    data = temp.data;
                    assets = temp.assets;
                }
            }

            

        }*/
        public List<string> getManagerList() {
            var mgrList = new List<string>();
            if (!manager.Contains(",")) {
                mgrList.Add(manager.Trim(' '));
            }
            else {
                var mgrs = manager.Split(',');
                foreach (string mgr in mgrs) {
                    mgrList.Add(mgr.Trim(' '));
                }
            }

            return mgrList;
        }

        public void CopyAssetsToStage(string sourceStash, string destinationStage) {
            if (assets == null) {
                return;
            }

            foreach (string filepath in assets) {
                if (filepath == "") {
                    continue;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(destinationStage, filepath)));
                File.Copy(
                    Path.Combine(sourceStash, filepath),
                    Path.Combine(destinationStage, filepath),
                    true
                );
            }
        }

        public void BackupAssetsToStash(string sourceStage, string destinationStash) {
            if (assets == null) {
                return;
            }

            foreach (string filepath in assets) {
                if (filepath == "") {
                    continue;
                }
                string src = Path.Combine(sourceStage, filepath);
                string dest = Path.Combine(destinationStash, filepath);
                if (File.Exists(src) & !Directory.Exists(dest)) {
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                }
                if (File.Exists(src)) {
                    try {
                        File.Copy(
                            src,
                            dest,
                            true
                        );
                    }
                    catch (DirectoryNotFoundException) {
                        continue;
                    }
                    catch (FileNotFoundException) {
                        continue;
                    }
                }
            }
        }

        public List<string> getMissingAssets(string sourceStash) {
            var missing = new List<string>();
            if (assets != null) {
                foreach (string filepath in assets) {
                    if (filepath == "") {
                        continue;
                    }
                     
                    if (!File.Exists(Path.Combine(sourceStash, filepath))) {
                        missing.Add(filepath);
                    }
                }
            }


            return missing;
        }

        public (GameObject obj, ObjectParameters objparams) CreateObject() {
            ObjectParameters op = new ObjectParameters(type);
            GameObject newObject = op.CreateEmpty();
            op.Adjust(newObject);

            newObject.SetObjectKey(objkey);
            if (data != null) {
                for (int i = 0; i < data.Length; i++) {
                    op.SetParamValue(i, newObject, (string)(JValue)data[i].value);
                }
            }

            return (newObject, op);
        }

    }
    public class TupleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JToken.Load(reader);
            if (obj.Type == JTokenType.Array)
            {
                var arr = (JArray)obj;
                if (arr.Count == 2)
                {
                    return new ValueTuple<string, object>((string)arr[0], arr[1]);
                }
            }
            return null;
        }

        public override bool CanConvert(Type type)
        {
            return type.Name == "ValueTuple`2";
        }
    }
}
