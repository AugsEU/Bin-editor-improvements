using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using DataReader;

namespace SMSReader
{
    /// <summary>
    /// Represents a scene in Super Mario Sunshine
    /// Use this class to save and load bin files
    /// Error 1 is normal
    /// </summary>
    public class SMSScene
    {
        public const string BIN_PARAMPATH = ".\\Parameters\\";

        public int error;

        string FilePath;    //bin file path

        public List<GameObject> Objects;    //Objects in tree format
        public List<GameObject> AllObjects; //All objects in list format

        public SMSScene()
        {
            FilePath = "";

            //Clear object lists
            Objects = new List<GameObject>();
            AllObjects = new List<GameObject>();

            error = 1;
        }
        /// <param name="path">Path of the bin </param>
        public SMSScene(string path)
        {
            FilePath = path;

            //Clear object lists
            Objects = new List<GameObject>();
            AllObjects = new List<GameObject>();

            //Check if file exists
            if (!File.Exists(path))
                error = 0;

            //Load file
            FileStream sceneFile = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                int i = 0;
                while (sceneFile.Position < sceneFile.Length)
                {
                    int e = ReadObject(sceneFile, null);
                    if (e != 1)
                    {
                        sceneFile.Close();
                        error = 2;
                        return;
                    }
                    i++;
                }
            }
            catch { sceneFile.Close(); error = 2; return; }
            sceneFile.Close();
            error = 1;
        }

        public SMSScene Clone()
        {
            SMSScene dupe = new SMSScene();
            dupe.error = error;
            dupe.SetPath(FilePath);

            Dictionary<GameObject, GameObject> dupedictionary = new Dictionary<GameObject,GameObject>();
            for (int i = 0; i < AllObjects.Count; i++)
            {
                GameObject cur = AllObjects[i].Clone();
                dupe.AllObjects.Add(cur);
                dupedictionary.Add(AllObjects[i], cur);
            }
            for (int i = 0; i < dupe.AllObjects.Count; i++)
            {
                if (dupe.AllObjects[i].Parent != null)
                    dupe.AllObjects[i].Parent = dupedictionary[dupe.AllObjects[i].Parent];
                for (int j = 0; j < dupe.AllObjects[i].Grouped.Count; j++)
                    if (dupe.AllObjects[i].Grouped[j] != null)
                        dupe.AllObjects[i].Grouped[j] = dupedictionary[dupe.AllObjects[i].Grouped[j]];
            }
            for (int i = 0; i < Objects.Count; i++)
                dupe.Objects.Add(dupedictionary[Objects[i]]);
            return dupe;
        }

        public void SetPath(string path)
        {
            FilePath = path;
        }

        /// <summary>
        /// Reads an object and all subobjects in a bin file
        /// Reads all subobjects and adds them also
        /// </summary>
        /// <param name="sceneFile">Bin file stream</param>
        /// <param name="Parent">Parent object. Null if no parent.</param>
        int ReadObject(Stream sceneFile, GameObject Parent)
        {
            GameObject thisObject = new GameObject();

            //Object Length
            byte[] len = new byte[4];
            sceneFile.Read(len, 0, 4);
            int length = BitConverter.ToInt32(DataManipulation.SwapEndian(len), 0);

            if (length > sceneFile.Length - sceneFile.Position + 4)
                return 2;

            long end = sceneFile.Position - 4 + (long)length;

            //NameHash
            byte[] id = new byte[2];
            sceneFile.Read(id, 0, 2);
            ushort NameHash = BitConverter.ToUInt16(DataManipulation.SwapEndian(id), 0);

            //Name Length
            byte[] nlen = new byte[2];
            sceneFile.Read(nlen, 0, 2);
            short nameLength = BitConverter.ToInt16(DataManipulation.SwapEndian(nlen), 0);

            //Name
            string name = DataManipulation.ReadString(sceneFile, (int)nameLength);

            //Description Hash
            byte[] flags = new byte[2];
            sceneFile.Read(flags, 0, 2);
            ushort DescHash = BitConverter.ToUInt16(DataManipulation.SwapEndian(flags), 0);

            //Data Length
            byte[] dlen = new byte[2];
            sceneFile.Read(dlen, 0, 2);
            short dataLength = BitConverter.ToInt16(DataManipulation.SwapEndian(dlen), 0);

            //Read Data
            byte[] gdata = new byte[dataLength];
            for (int i = 0; i < gdata.Length; i++)
                gdata[i] = (byte)sceneFile.ReadByte();

            //Read object specific data
            int grouped = 0;
            List<byte> values = new List<byte>();

            //Check if the object is a group object.
            //I still do not know what makes an object a group object or not, so I just allow which objects are usually group objects
            //There is probably a better way to do this...
            if (NameHash == 16824 || NameHash == 15406 || NameHash == 28318 || NameHash == 18246 || NameHash == 43971 || NameHash == 9858 || NameHash == 25289 ||   //Levels
                NameHash == 33769 || NameHash == 49941 || NameHash == 13756 || NameHash == 65459 || NameHash == 38017 || NameHash == 47488 ||   //Tables
                NameHash == 8719 || NameHash == 22637 )
            {

                //4 extra bytes of data for these objects for some reason
                if (NameHash == 15406 || NameHash == 9858)
                {
                    values.Add((byte)sceneFile.ReadByte());
                    values.Add((byte)sceneFile.ReadByte());
                    values.Add((byte)sceneFile.ReadByte());
                    values.Add((byte)sceneFile.ReadByte());
                }

                byte[] datv = new byte[4];
                sceneFile.Read(datv, 0, 4);
                grouped = BitConverter.ToInt32(DataManipulation.SwapEndian(datv), 0);
            }
            else
            {
                //Read the rest of the object
                while (sceneFile.Position < end)
                    values.Add((byte)sceneFile.ReadByte());
            }

            thisObject.Name = name;
            thisObject.Hash = NameHash;
            thisObject.DescHash = DescHash;
            thisObject.Values = values.ToArray();
            thisObject.Description = Encoding.GetEncoding("shift-jis").GetString(gdata);
            thisObject.Parent = Parent;
            thisObject.Grouped = new List<GameObject>();

            //Add object to object list
            AllObjects.Add(thisObject);

            //Add object to parent
            if (Parent != null)
                Parent.Grouped.Add(thisObject);
            else
                Objects.Add(thisObject);

            //Read all grouped objects
            for (int i = 0; i < grouped; i++)
                ReadObject(sceneFile, thisObject);

            return 1;
        }

        /// <summary>
        /// Saves the bin file to the path specified.
        /// </summary>
        /// <param name="path">Path to save the bin file to</param>
        public void Save(string path)
        {
            FileStream sceneFile = new FileStream(path, FileMode.Create, FileAccess.Write);

            int i = 0;
            foreach (GameObject go in Objects)
            {
                go.SaveObject(sceneFile);
                i++;
            }

            sceneFile.Close();
        }

        /// <summary>
        /// Repairs disrepencies between names, descriptions, and their hashes
        /// </summary>
        public int RepairHashes()
        {
            int c = 0;
            foreach (GameObject g in AllObjects)
            {
                ushort nhash = GCN.CreateHash(g.Name);
                ushort dhash = GCN.CreateHash(g.Description);
                if (g.Hash != nhash || g.DescHash != dhash)
                {
                    g.Hash = nhash;
                    g.DescHash = dhash;
                    c++;
                }
            }
            return c;
        }

        /// <summary>
        /// Verifies the integrity of the file
        /// using hashes
        /// </summary>
        /// <returns>Return of true means the file passed the test</returns>
        public bool VerifyIntegrity()
        {
            foreach (GameObject g in AllObjects)
            {
                if (g.Hash != GCN.CreateHash(g.Name))
                    return false;
                if (g.DescHash != GCN.CreateHash(g.Description))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Verifies the integrity of the file
        /// using hashes
        /// </summary>
        /// <returns>id of the next broken object.</returns>
        public int VerifyIntegrity(int start)
        {
            for (int i = start; i < AllObjects.Count; i++)
            {
                GameObject g = AllObjects[i];
                if (g.Hash != GCN.CreateHash(g.Name))
                    return i;
                if (g.DescHash != GCN.CreateHash(g.Description))
                    return i;
            }
            return AllObjects.Count;
        }
    }

    /// <summary>
    /// Represents a game object
    /// </summary>
    public class GameObject
    {
        public ushort Hash;
        public string Name;
        public ushort DescHash;
        public string Description;
        public byte[] Values;

        public GameObject Parent;
        public List<GameObject> Grouped;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns></returns>
        public GameObject()
        {
            Name = "(null)";
            Hash = GCN.CreateHash("(null)");
            Description = "(null)1";
            DescHash = GCN.CreateHash("(null)1");

            Values = new byte[0];
            Parent = null;
            Grouped = new List<GameObject>();
        }
        public GameObject(string name, string refname, int length)
        {
            Name = name;
            Hash = GCN.CreateHash(Name);
            Description = refname;
            DescHash = GCN.CreateHash(Description);

            Values = new byte[length];
            Parent = null;
            Grouped = new List<GameObject>();
        }

        public GameObject Clone()
        {
            GameObject dupe = new GameObject();
            dupe.Hash = Hash;
            dupe.Name = Name;
            dupe.DescHash = DescHash;
            dupe.Description = Description;
            dupe.Values = new byte[Values.Length];
            Values.CopyTo(dupe.Values, 0);
            dupe.Grouped = new List<GameObject>(Grouped);
            dupe.Parent = Parent;
            return dupe;
        }

        public void SetObjectKey(string key) {
            Description = key;
            DescHash = GCN.CreateHash(key);
        }

        public void SetActorType(string type) {
            Name = type;
            Hash = GCN.CreateHash(type);
        }

        /// <summary>
        /// Calculates the length of the game object.
        /// Used for saving.
        /// </summary>
        /// <returns></returns>
        public uint CalculateLength()
        {
            uint length = (uint)12 + (uint)Name.Length + (uint)Encoding.GetEncoding("shift-jis").GetByteCount(Description) + (uint)Values.Length;
            if (Hash == 16824 || Hash == 15406 || Hash == 28318 || Hash == 18246 || Hash == 43971 || Hash == 9858 || Hash == 25289 ||   //Levels
                Hash == 33769 || Hash == 49941 || Hash == 13756 || Hash == 65459 || Hash == 38017 || Hash == 47488 ||   //Tables
                Hash == 8719 || Hash == 22637 )
            {
                length += 4;
                foreach (GameObject go in Grouped)
                    length += go.CalculateLength();
            }
            return length;
        }

        /// <summary>
        /// Saves the object to the stream
        /// </summary>
        /// <param name="stream"></param>
        public void SaveObject(Stream stream)
        {
            byte[] len = BitConverter.GetBytes(CalculateLength());
            byte[] bid = BitConverter.GetBytes(Hash);
            byte[] nln = BitConverter.GetBytes((ushort)Name.Length);

            byte[] nam = new byte[Name.Length];
            for (int i = 0; i < Name.Length; i++)
                nam[i] = (byte)Name[i];

            byte[] flg = BitConverter.GetBytes(DescHash);

            byte[] data = Encoding.GetEncoding("shift-jis").GetBytes(Description);
            byte[] dln = BitConverter.GetBytes((ushort)data.Length);

            stream.Write(DataManipulation.SwapEndian(len), 0, 4);
            stream.Write(DataManipulation.SwapEndian(bid), 0, 2);
            stream.Write(DataManipulation.SwapEndian(nln), 0, 2);
            stream.Write(nam, 0, nam.Length);
            stream.Write(DataManipulation.SwapEndian(flg), 0, 2);
            stream.Write(DataManipulation.SwapEndian(dln), 0, 2);
            stream.Write(data, 0, data.Length);
            stream.Write(Values, 0, Values.Length);

            if (Hash == 16824 || Hash == 15406 || Hash == 28318 || Hash == 18246 || Hash == 43971 || Hash == 9858 || Hash == 25289 ||   //Levels
                Hash == 33769 || Hash == 49941 || Hash == 13756 || Hash == 65459 || Hash == 38017 || Hash == 47488 ||   //Tables
                Hash == 8719 || Hash == 22637 )
            {
                byte[] grouped = BitConverter.GetBytes((uint)Grouped.Count);
                stream.Write(DataManipulation.SwapEndian(grouped), 0, 4);
                foreach (GameObject go in Grouped)
                    go.SaveObject(stream);
            }
        }

        /// <summary>
        /// Loads the object from a stream
        /// Useful for clipboard loading
        /// </summary>
        /// <param name="stream"></param>
        public void LoadObject(Stream stream)
        {
            //Object Length
            byte[] len = new byte[4];
            stream.Read(len, 0, 4);
            int length = BitConverter.ToInt32(DataManipulation.SwapEndian(len), 0);

            long end = stream.Position - 4 + (long)length;

            //ObjectID?
            byte[] id = new byte[2];
            stream.Read(id, 0, 2);
            Hash = BitConverter.ToUInt16(DataManipulation.SwapEndian(id), 0);

            //Name Length
            byte[] nlen = new byte[2];
            stream.Read(nlen, 0, 2);
            short nameLength = BitConverter.ToInt16(DataManipulation.SwapEndian(nlen), 0);

            //Name
            Name = DataManipulation.ReadString(stream, (int)nameLength);

            //Flags?
            byte[] flags = new byte[2];
            stream.Read(flags, 0, 2);
            DescHash = BitConverter.ToUInt16(DataManipulation.SwapEndian(flags), 0);

            //Data Length
            byte[] dlen = new byte[2];
            stream.Read(dlen, 0, 2);
            short dataLength = BitConverter.ToInt16(DataManipulation.SwapEndian(dlen), 0);

            //Read Data
            byte[] dat = new byte[dataLength];
            for (int i = 0; i < dat.Length; i++)
                dat[i] = (byte)stream.ReadByte();

            Description = Encoding.GetEncoding("shift-jis").GetString(dat);

            //Read object specific data
            int grouped = 0;
            List<byte> values = new List<byte>();

            //Check if the object is a group object.
            //I still do not know what makes an object a group object or not, so I just allow which objects are usually group objects
            //There is probably a better way to do this...
            if (Hash == 16824 || Hash == 15406 || Hash == 28318 || Hash == 18246 || Hash == 43971 || Hash == 9858 || Hash == 25289 ||   //Levels
                Hash == 33769 || Hash == 49941 || Hash == 13756 || Hash == 65459 || Hash == 38017 || Hash == 47488 ||   //Tables
                Hash == 8719 || Hash == 22637 )
            {
                if (Hash == 15406 || Hash == 9858)
                {
                    values.Add((byte)stream.ReadByte());
                    values.Add((byte)stream.ReadByte());
                    values.Add((byte)stream.ReadByte());
                    values.Add((byte)stream.ReadByte());
                }

                byte[] datv = new byte[4];
                stream.Read(datv, 0, 4);
                grouped = BitConverter.ToInt32(DataManipulation.SwapEndian(datv), 0);
            }
            else
            {
                while (stream.Position < end)
                    values.Add((byte)stream.ReadByte());
            }

            Values = values.ToArray();

            Parent = null;
            Grouped = new List<GameObject>();
        }

        /// <summary>
        /// Creates a copy of the object
        /// </summary>
        /// <returns></returns>
        public GameObject DeepCopy()
        {
            GameObject clone = new GameObject();
            clone.Hash = Hash;
            clone.Name = Name;
            clone.Values = new byte[Values.Length];
            for (int i = 0; i < Values.Length; i++)
                clone.Values[i] = Values[i];
            clone.Description = Description;

            clone.Grouped = new List<GameObject>(); //lets not duplicate children
            clone.Parent = Parent;
            clone.DescHash = DescHash;

            return clone;
        }

        public static bool IsGroup(string name)
        {
            UInt16 hash = GCN.CreateHash(name);
            if (hash == 16824 || hash == 15406 || hash == 28318 || hash == 18246 || hash == 43971 || hash == 9858 || hash == 25289 ||   //Levels
                hash == 33769 || hash == 49941 || hash == 13756 || hash == 65459 || hash == 38017 || hash == 47488 ||   //Tables
                hash == 8719 || hash == 22637)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Data types of object parameters
    /// </summary>
    public enum ParameterDataTypes
    {
        BYTE = 0,
        SHORT,
        INT,
        FLOAT,
        DOUBLE,
        STRING,
        BUFFER,
        COMMENT
    }

    /// <summary>
    /// Parameters in the object data
    /// Data types seem to be hardcoded into the game based off of
    /// the object type. This is an easy way to get around it. The
    /// text files store what the data is and how to read and write
    /// to it.
    /// </summary>
    public struct ObjectParameters
    {
        private string ObjName;

        public string DisplayName;

        public string[] DataNames;
        public ParameterDataTypes[] DataTypes;
        public int[] DataPositions;

        public Dictionary<int, int> CustomInfo;
        public Dictionary<int, string> CommentInfo; //Holds comments and default values
        public Dictionary<int, Dictionary<string, string>> ComboInfo; //Combo box info

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        public ObjectParameters(string Name)
        {
            DisplayName = Name;
            ObjName = "";
            DataNames = new string[0];
            DataTypes = new ParameterDataTypes[0];
            DataPositions = new int[0];

            CustomInfo = new Dictionary<int, int>();
            CommentInfo = new Dictionary<int, string>();
            ComboInfo = new Dictionary<int, Dictionary<string, string>>();

            ReadObjectParameters(Name);
        }

        /// <summary>
        /// Reads object parameters from the object from txt files
        /// in the "Parameters" folder that stores parameter types
        /// </summary>
        /// <param name="gObject">Object</param>
        public void ReadObjectParameters(GameObject gObject)
        {
            ReadObjectParameters(gObject.Name);
            ObjName = gObject.Name;

            Adjust(gObject);
        }

        /// <summary>
        /// Ajusts data positions for given object
        /// </summary>
        /// <param name="gObject">Object</param>
        public void Adjust(GameObject gObject)
        {
            DataPositions = new int[DataNames.Length];
            for (int i = 1; i < DataNames.Length; i++)
                DataPositions[i] = DataPositions[i - 1] + GetParamLength(i - 1, gObject);
        }

        /// <summary>
        /// Creates blank object
        /// </summary>
        /// <param name="gObject">Object</param>
        public GameObject CreateEmpty()
        {
            int length = 0;
            for (int i = 0; i < DataTypes.Length; i++)
            {
                switch (DataTypes[i])
                {
                    case ParameterDataTypes.BYTE:
                        length += 1;
                        break;
                    case ParameterDataTypes.SHORT:
                        length += 2;
                        break;
                    case ParameterDataTypes.INT:
                        length += 4;
                        break;
                    case ParameterDataTypes.FLOAT:
                        length += 4;
                        break;
                    case ParameterDataTypes.DOUBLE:
                        length += 8;
                        break;
                    case ParameterDataTypes.STRING:
                        length += 2;
                        break;
                }
            }

            GameObject ret = new GameObject(ObjName, DisplayName + " 1", length);

            Adjust(ret);
            for (int i = 0; i < DataTypes.Length; i++)
            {
                if (CommentInfo.ContainsKey(i) && DataTypes[i] != ParameterDataTypes.BUFFER && DataTypes[i] != ParameterDataTypes.COMMENT)
                {
                    if (CommentInfo[i] == "(null)" && DataTypes[i] != ParameterDataTypes.STRING)
                        SetParamValue(i, ret, "0"); //use null as a real null value
                    else
                        SetParamValue(i, ret, CommentInfo[i]);
                }
            }

            return ret;
        }

        /// <summary>
        /// Reads object parameters from the object from txt files
        /// in the "Parameters" folder that stores parameter types
        /// </summary>
        /// <param name="gObjectID">Object ID</param>
        public void ReadObjectParameters(string objName)
        {
            CommentInfo = new Dictionary<int, string>();
            CustomInfo = new Dictionary<int, int>();
            ComboInfo = new Dictionary<int, Dictionary<string,string>>();

            ObjName = objName;
            try
            {
                StreamReader sr = new StreamReader(SMSScene.BIN_PARAMPATH + objName + ".txt");
                DisplayName = sr.ReadLine();

                List<string> datanames = new List<string>();
                List<ParameterDataTypes> datatypes = new List<ParameterDataTypes>();

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] segs = line.Split(' ');

                    datanames.Add(segs[0]);

                    ParameterDataTypes t = DataManipulation.GetEnumFromString<ParameterDataTypes>(segs[1]);

                    ComboInfo.Add(datatypes.Count, new Dictionary<string, string>());

                    if (t == ParameterDataTypes.COMMENT)
                    {
                        string cust = "";
                        for (int i = 2; i < segs.Length; i++)
                            cust += segs[i] + " ";
                        CommentInfo.Add(datatypes.Count, cust.Substring(0, cust.Length - 1));
                    }
                    else if (t == ParameterDataTypes.BUFFER)
                    {
                        int size = 0;
                        if (segs.Length > 2)
                            int.TryParse(segs[2], out size);
                        CustomInfo.Add(datatypes.Count, size);
                    }
                    else if (segs.Length > 2)
                    {
                        //Default value
                        int ptr = 2;
                        string def = "";
                        if (segs[ptr][0] == '\"')
                        {
                            //quote seperated
                            def += segs[ptr].Substring(1, segs[ptr].Length - 1);
                            if (segs[ptr][segs[ptr].Length - 1] != '\"')
                            {
                                do
                                {
                                    ptr++;
                                    if (ptr == segs.Length)
                                        break;
                                    def += " " + segs[ptr];
                                }
                                while (segs[ptr][segs[ptr].Length - 1] != '\"');
                            }
                            ptr++;
                            def = def.Substring(0, def.Length - 1);
                        }
                        else //Normal seperated
                            def = segs[ptr++];

                        CommentInfo.Add(datatypes.Count, def);

                        while (ptr < segs.Length)
                        {
                            while (ptr < segs.Length && segs[ptr] == "")
                                ptr++;
                            if (ptr >= segs.Length)
                                break;

                            //ComboBox
                            string key = "";
                            if (segs[ptr][0] == '\"')
                            {
                                //quote seperated
                                key += segs[ptr].Substring(1, segs[ptr].Length - 1);
                                if (segs[ptr][segs[ptr].Length - 1] != '\"')
                                {
                                    do
                                    {
                                        ptr++;
                                        if (ptr == segs.Length)
                                            break;
                                        key += " " + segs[ptr];
                                    }
                                    while (segs[ptr][segs[ptr].Length - 1] != '\"');
                                }
                                ptr++;
                                key = key.Substring(0, key.Length - 1);
                            }
                            else //Normal seperated
                                key = segs[ptr++];
                            string entry = "";
                            if (segs[ptr][0] == '\"')
                            {
                                //quote seperated
                                entry += segs[ptr].Substring(1, segs[ptr].Length - 1);
                                if (segs[ptr][segs[ptr].Length - 1] != '\"')
                                {
                                    do
                                    {
                                        ptr++;
                                        if (ptr == segs.Length)
                                            break;
                                        entry += " " + segs[ptr];
                                    }
                                    while (segs[ptr][segs[ptr].Length - 1] != '\"');
                                }
                                ptr++;
                                entry = entry.Substring(0, entry.Length - 1);
                            }
                            else //Normal seperated
                                entry = segs[ptr++];
                            ComboInfo[datatypes.Count].Add(key, entry);
                        }
                    }

                    //Add datatype
                    datatypes.Add(t);
                }

                DataNames = datanames.ToArray();
                DataTypes = datatypes.ToArray();

                DataPositions = new int[0];

                sr.Close();
            }
            catch
            {
                DisplayName = "Unknown";
                DataNames = new string[0];
                DataTypes = new ParameterDataTypes[0];
                DataPositions = new int[0];
            }
        }

        /// <summary>
        /// Returns the parameter length
        /// </summary>
        /// <param name="index">Parameter index</param>
        /// <param name="gObject">Game object</param>
        /// <returns></returns>
        public int GetParamLength(int index, GameObject gObject)
        {
            try
            {
                int dpos = DataPositions[index];

                switch (DataTypes[index])
                {
                    case ParameterDataTypes.BYTE:
                        return 1;
                    case ParameterDataTypes.SHORT:
                        return 2;
                    case ParameterDataTypes.INT:
                        return 4;
                    case ParameterDataTypes.FLOAT:
                        return 4;
                    case ParameterDataTypes.DOUBLE:
                        return 8;
                    case ParameterDataTypes.STRING:
                        return BitConverter.ToInt16(DataManipulation.SwapEndian(gObject.Values, dpos, 2), 0) + 2;
                    case ParameterDataTypes.BUFFER:
                        return CustomInfo[index];
                    case ParameterDataTypes.COMMENT:
                        return 0;
                }
            }
            catch { return 0; }
            return 0;
        }

        /// <summary>
        /// Returns whether the object contains this parameter
        /// </summary>
        /// <param name="index">Parameter name</param>
        /// <returns></returns>
        public bool ContainsParameter(string key)
        {
            if (DataNames == null) {
                return false;
            }

            for (int i = 0; i < DataNames.Length; i++)
            {
                if (DataNames[i] == key)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the parameter value
        /// </summary>
        /// <param name="index">Parameter name</param>
        /// <param name="gObject">Game object</param>
        /// <returns></returns>
        public string GetParamValue(string key, GameObject gObject)
        {
            int index = -1;
            for (int i = 0; i < DataNames.Length; i++)
            {
                if (DataNames[i] != key)
                    continue;
                index = i;
                break;
            }
            return GetParamValue(index, gObject);
        }

        /// <summary>
        /// Returns the parameter value
        /// </summary>
        /// <param name="index">Parameter index</param>
        /// <param name="gObject">Game object</param>
        /// <returns></returns>
        public string GetParamValue(int index, GameObject gObject)
        {
            if (index < 0 || index >= DataNames.Length)
                return "";
            try
            {
                int dpos = 0;
                if (DataTypes[index] != ParameterDataTypes.COMMENT)
                    dpos = DataPositions[index];

                switch (DataTypes[index])
                {
                    case ParameterDataTypes.BYTE:
                        return gObject.Values[dpos].ToString();
                    case ParameterDataTypes.SHORT:
                        return BitConverter.ToInt16(DataManipulation.SwapEndian(gObject.Values, dpos, 2), 0).ToString();
                    case ParameterDataTypes.INT:
                        return BitConverter.ToInt32(DataManipulation.SwapEndian(gObject.Values, dpos, 4), 0).ToString();
                    case ParameterDataTypes.FLOAT:
                        return BitConverter.ToSingle(DataManipulation.SwapEndian(gObject.Values, dpos, 4), 0).ToString();
                    case ParameterDataTypes.DOUBLE:
                        return BitConverter.ToDouble(DataManipulation.SwapEndian(gObject.Values, dpos, 8), 0).ToString();
                    case ParameterDataTypes.STRING:
                        int length = BitConverter.ToInt16(DataManipulation.SwapEndian(gObject.Values, dpos, 2), 0);

                        MemoryStream ms = new MemoryStream(gObject.Values);
                        ms.Position = dpos + 2;
                        return DataManipulation.ReadString(ms, length);
                    case ParameterDataTypes.BUFFER:
                        string array = "";
                        for (int i = dpos; i < dpos + CustomInfo[index]; i++)
                            array += gObject.Values[i].ToString("X2") + " ";
                        return array.Substring(0, array.Length - 1);
                    case ParameterDataTypes.COMMENT:
                        return CommentInfo[index];
                }
            }
            catch { return ""; }
            return "";
        }

        /// <summary>
        /// Sets value to object
        /// </summary>
        /// <param name="index">Parameter key</param>
        /// <param name="gObject">Game object</param>
        /// <returns></returns>
        public void SetParamValue(string key, GameObject gObject, string value)
        {
            int index = -1;
            for (int i = 0; i < DataNames.Length; i++)
            {
                if (DataNames[i] != key)
                    continue;
                index = i;
                break;
            }
            SetParamValue(index, gObject, value);
        }

        /// <summary>
        /// Sets value to object
        /// </summary>
        /// <param name="index">Parameter key</param>
        /// <param name="gObject">Game object</param>
        /// <returns></returns>
        public void SetParamValue(string key, GameObject gObject, ValueType value)
        {
            int index = -1;
            for (int i = 0; i < DataNames.Length; i++)
            {
                if (DataNames[i] != key)
                    continue;
                index = i;
                break;
            }
            SetParamValue(index, gObject, value.ToString());
        }

        /// <summary>
        /// Sets value to object
        /// </summary>
        /// <param name="index">Parameter index</param>
        /// <param name="gObject">Game object</param>
        /// <returns></returns>
        public void SetParamValue(int index, GameObject gObject, string value)
        {
            if (index < 0 || index >= DataPositions.Length)
                return;

            if (gObject == null)
                return;

            int dpos = DataPositions[index];

            switch (DataTypes[index])
            {
                case ParameterDataTypes.BYTE:
                    byte b;
                    if (byte.TryParse(value, out b))
                        gObject.Values[dpos] = b;
                    break;
                case ParameterDataTypes.SHORT:
                    short sh;
                    if (short.TryParse(value, out sh))
                    {
                        byte[] shdat = BitConverter.GetBytes(sh);
                        for (int i = 0; i < 2; i++)
                            gObject.Values[dpos + i] = shdat[1 - i];
                    }
                    break;
                case ParameterDataTypes.INT:
                    int ind;
                    if (int.TryParse(value, out ind))
                    {
                        byte[] indat = BitConverter.GetBytes(ind);
                        for (int i = 0; i < 4; i++)
                            gObject.Values[dpos + i] = indat[3 - i];
                    }
                    break;
                case ParameterDataTypes.FLOAT:
                    float fl;
                    if (float.TryParse(value, out fl))
                    {
                        byte[] fldat = BitConverter.GetBytes(fl);
                        for (int i = 0; i < 4; i++)
                            gObject.Values[dpos + i] = fldat[3 - i];
                    }
                    break;
                case ParameterDataTypes.DOUBLE:
                    float db;
                    if (float.TryParse(value, out db))
                    {
                        byte[] dbdat = BitConverter.GetBytes(db);
                        for (int i = 0; i < 8; i++)
                            gObject.Values[dpos + i] = dbdat[7 - i];
                    }
                    break;
                case ParameterDataTypes.STRING:
                    int len = BitConverter.ToInt16(DataManipulation.SwapEndian(gObject.Values, dpos, 2), 0);

                    byte[] tdata = Encoding.GetEncoding("shift-jis").GetBytes(value);

                    int length = tdata.Length;

                    List<byte> values = new List<byte>(gObject.Values);

                    int change = length - len;

                    try
                    {
                        if (change > 0)
                            values.InsertRange(dpos + 2 + len - 1, new byte[change]);
                        if (change < 0)
                            values.RemoveRange(dpos + 2 + len - 1 + change, -change);
                    }
                    catch { return; }

                    gObject.Values = values.ToArray();

                    byte[] sshdat = BitConverter.GetBytes((ushort)length);
                    for (int i = 0; i < 2; i++)
                        gObject.Values[dpos + i] = sshdat[1 - i];

                    for (int i = 0; i < length; i++)
                        gObject.Values[dpos + 2 + i] = tdata[i];
                    Adjust(gObject);
                    break;
                case ParameterDataTypes.BUFFER:
                case ParameterDataTypes.COMMENT:
                    return;
            }
        }

        /// <summary>
        /// Saves object parameters to parameter txt file
        /// </summary>
        public void SaveObjectParameters()
        {
            StreamWriter sr = new StreamWriter(SMSScene.BIN_PARAMPATH + ObjName + ".txt");
            sr.WriteLine(DisplayName);

            for (int i = 0; i < DataNames.Length; i++)
            {
                sr.Write(DataNames[i] + " " + DataTypes[i].ToString());
                if (DataTypes[i] == ParameterDataTypes.BUFFER)
                    sr.Write(" " + CustomInfo[i].ToString());
                else if (DataTypes[i] == ParameterDataTypes.COMMENT)
                    sr.Write(" " + CommentInfo[i]);
                else
                {
                    if (CommentInfo.ContainsKey(i))
                        sr.Write(" " + "\"" + CommentInfo[i] + "\"");
                    else
                        sr.Write(" (null)");
                    foreach (KeyValuePair<string, string> kvp in ComboInfo[i])
                        sr.Write(" \"" + kvp.Key.Replace("\"", "\\\"") + "\" \"" + kvp.Value.Replace("\"", "\\\"") + "\"");
                }
                sr.WriteLine();
            }
            sr.Close();
        }
    }
}
