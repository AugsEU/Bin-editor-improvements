using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DataReader;

namespace SMSReader
{
    /* Holds Parameter data */
    public struct PrmData
    {
        private uint size;
        private byte[] value;

        public byte ByteValue
        {
            get
            {
                if (Size == 0)
                    return 0;
                return value[0];
            }
            set
            {
                if (Size != 0)
                    this.value[0] = value;
            }
        }
        public short ShortValue
        {
            get
            {
                if (Size < 2)
                    return 0;
                return BitConverter.ToInt16(value, 0);
            }
            set
            {
                if (Size >= 2)
                {
                    byte[] nb = BitConverter.GetBytes((short)value);
                    for (int i = 0; i < nb.Length; i++)
                        this.value[i] = nb[i];
                }
            }
        }
        public int IntValue
        {
            get
            {
                if (Size < 4)
                    return 0;
                return BitConverter.ToInt32(value, 0);
            }
            set
            {
                if (Size >= 4)
                {
                    byte[] nb = BitConverter.GetBytes((int)value);
                    for (int i = 0; i < nb.Length; i++)
                        this.value[i] = nb[i];
                }
            }
        }
        public float SingleValue
        {
            get
            {
                if (Size < 4)
                    return 0f;
                return BitConverter.ToSingle(value, 0);
            }
            set
            {
                if (Size >= 4)
                {
                    byte[] nb = BitConverter.GetBytes((float)value);
                    for (int i = 0; i < nb.Length; i++)
                        this.value[i] = nb[i];
                }
            }
        }
        public double DoubleValue
        {
            get
            {
                if (Size < 8)
                    return 0.0;
                return BitConverter.ToDouble(value, 0);
            }
            set
            {
                if (Size >= 4)
                {
                    byte[] nb = BitConverter.GetBytes((float)value);
                    for (int i = 0; i < nb.Length; i++)
                        this.value[i] = nb[i];
                }
            }
        }
        public Vector VectorValue
        {
            get
            {
                if (Size < 12)
                    return new Vector(0f, 0f, 0f);
                return new Vector(BitConverter.ToSingle(value, 0), BitConverter.ToSingle(value, 4), BitConverter.ToSingle(value, 8));
            }
            set
            {
                if (Size >= 12)
                {
                    MemoryStream ms = new MemoryStream(this.value);
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Write(BitConverter.GetBytes(value.X), 0, 4);
                    ms.Write(BitConverter.GetBytes(value.Y), 0, 4);
                    ms.Write(BitConverter.GetBytes(value.Z), 0, 4);
                    ms.Close();
                }
            }
        }
        public string StringValue
        {
            get
            {
                string ret = "";
                for (int i = 0; i < Size; i++)
                    ret += (char)value[i];
                return ret;
            }
            set
            {
                size = (uint)value.Length;
                this.value = new byte[size];
                for (int i = 0; i < value.Length; i++)
                    this.value[i] = (byte)value[i];
            }
        }

        public int Size
        {
            get { return (int)size; }
        }
        public PrmData(byte[] values)
        {
            size = (uint)values.Length;
            value = null;

            SetData(values);
        }
        public PrmData(uint size)
        {
            this.size = size;
            value = null;
        }
        public PrmData(Stream s)
        {
            size = 0;
            value = null;

            Read(s);
        }

        public Type GuessType()
        {
            switch (size)
            {
                case 0: return null;
                case 1: return typeof(byte);
                case 2: return typeof(short);
                case 4:
                    if (SingleValue > 10000000 || SingleValue < -10000000)
                        return typeof(int);
                    else
                        return typeof(float);
                case 8: return typeof(double);
                default: return typeof(string);
            }
        }

        public void SetSize(uint newsize)
        {
            size = newsize;
            byte[] newarray = new byte[newsize];
            for (int i = 0; i < newarray.Length && i < value.Length; i++)
                value[i] = newarray[i];
        }

        public void Read(Stream s)
        {
            size = Data.ReadUInt32(s);
            value = Data.ReadEndian(s, Size);
        }
        public void Write(Stream s)
        {
            Data.WriteUInt32(s, size);
            Data.WriteEndian(s, value);
        }

        public byte[] GetData()
        {
            return value;
        }
        public bool SetData(byte[] dat)
        {
            if (dat.Length != value.Length)
                return false;
            dat.CopyTo(value, 0);
            return true;
        }
    }

    //SMS Parameter file
    public class PrmFile
    {
        private FileStream file;    //File Stream
        private Dictionary<string, PrmData> paramList = new Dictionary<string, PrmData>();  //Parameters

        private string path;

        public string Name
        {
            get
            {
                FileInfo fi = new FileInfo(path);
                return fi.Name;
            }
        }

        /*
         * Creates empty, nameless file
         */
        public PrmFile()
        {
            paramList = new Dictionary<string, PrmData>();
            file = null;
        }

        /*
         * Creates object and loads from file
         */
        public PrmFile(string filename)
        {
            paramList = new Dictionary<string, PrmData>();
            file = null;
            Load(filename);
        }

        /*
         * Loads prm file type
         */
        public void Load(string filename)
        {
            path = filename;

            if (!File.Exists(filename))
                return;
            file = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);

            int count = Data.ReadInt32(file);
            for (int i = 0; i < count; i++)
            {
                UInt16 hash = Data.ReadUInt16(file);
                UInt16 strSize = Data.ReadUInt16(file);
                string key = Data.ReadString(file, strSize);

                PrmData dati = new PrmData(file);

                paramList.Add(key, dati);
            }
        }

        /*
         * Gets target file size
         */
        public int GetSize()
        {
            int size = 4;
            foreach (KeyValuePair<string, PrmData> kvp in paramList)
                size += 8 + Encoding.GetEncoding("shift-jis").GetByteCount(kvp.Key) + kvp.Value.Size;
            return size;
        }

        /*
         * Saves to new file
         */
        public void Save(string filename)
        {
            if (file != null)
                file.Close();
            file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
            path = filename;
            Save();
        }

        /*
         * Saves prm file type
         */
        public void Save()
        {
            if (file == null)
                return;
            file.SetLength(GetSize());
            file.Seek(0, SeekOrigin.Begin);
            Data.WriteInt32(file, paramList.Count);
            foreach (KeyValuePair<string, PrmData> kvp in paramList)
            {
                Data.WriteUInt16(file, GCN.CreateHash(kvp.Key));
                Data.WriteUInt16(file, (UInt16)Encoding.GetEncoding("shift-jis").GetByteCount(kvp.Key));
                Data.WriteString(file, kvp.Key, Encoding.GetEncoding("shift-jis"), false);
                kvp.Value.Write(file);
            }
            file.Flush();
        }

        public void SetSize(string key, uint size)
        {
            PrmData prm = paramList[key];
            prm.SetSize(size);
            paramList[key] = prm;
        }

        public string[] GetAllKeys()
        {
            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, PrmData> kvp in paramList)
                keys.Add(kvp.Key);
            return keys.ToArray();
        }


        public bool AddData(string key, byte[] newval)
        {
            if (paramList.ContainsKey(key))
                return false;
            PrmData prm = new PrmData(newval);
            paramList.Add(key, prm);
            return true; ;
        }
        public bool AddData(string key, PrmData newval)
        {
            if (paramList.ContainsKey(key))
                return false;
            paramList.Add(key, newval);
            return true;
        }
        public PrmData GetData(string key)
        {
            if (paramList.ContainsKey(key))
                return paramList[key];
            else
                return new PrmData(0);
        }
        public void SetData(string key, PrmData newval)
        {
            if (paramList.ContainsKey(key))
                paramList[key] = newval;
            else
                AddData(key, newval);
        }

        public void UnLoad()
        {
            if (file != null)
                file.Close();
            paramList.Clear();
        }
    }
}
