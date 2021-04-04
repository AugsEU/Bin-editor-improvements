using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataReader;

//Rail file
namespace SMSReader
{
    /*
     * Key frame structure
     */
    public struct KeyFrame
    {
        public short x;     //X Position
        public short y;     //Y Position
        public short z;     //Z Position
        public short u1;    //Unknown 1
        public short u2;     //Unknown 2
        public short u3;     //Unknown 3
        public short pitch; //Pitch
        public short yaw;   //Yaw
        public short roll;  //Roll
        public short speed; //Speed
        public short[] connections;
        public float[] periods;

        /*
         * Sets all data to default
         */
        public void NullData()
        {
            x = 0;
            y = 0;
            z = 0;
            u1 = 0;
            u2 = 0;
            pitch = -1;
            yaw = -1;
            roll = -1;
            speed = -1;
            connections = new short[8];
            periods = new float[8];
        }

        /*
         * Reads data from stream
         */
        public void ReadData(Stream file)
        {
            Data.IsBigEndian = true;
            x = Data.ReadInt16(file);
            y = Data.ReadInt16(file);
            z = Data.ReadInt16(file);
            u1 = Data.ReadInt16(file);
            u2 = Data.ReadInt16(file);
            u3 = Data.ReadInt16(file);
            pitch = Data.ReadInt16(file);
            yaw = Data.ReadInt16(file);
            roll = Data.ReadInt16(file);
            speed = Data.ReadInt16(file);

            connections = new short[8];
            for (int i = 0; i < 8; i++)
                connections[i] = Data.ReadInt16(file);

            periods = new float[8];
            for (int i = 0; i < 8; i++)
                periods[i] = Data.ReadSingle(file);
        }

        /*
         * Writes data to stream
         */
        public void WriteData(Stream file)
        {
            Data.IsBigEndian = true;
            Data.WriteInt16(file, x);
            Data.WriteInt16(file, y);
            Data.WriteInt16(file, z);
            Data.WriteInt16(file, u1);
            Data.WriteInt16(file, u2);
            Data.WriteInt16(file, u3);
            Data.WriteInt16(file, pitch);
            Data.WriteInt16(file, yaw);
            Data.WriteInt16(file, roll);
            Data.WriteInt16(file, speed);
            for (int i = 0; i < 8; i++)
                Data.WriteInt16(file, connections[i]);

            for (int i = 0; i < 8; i++)
                Data.WriteSingle(file, periods[i]);
        }

        public KeyFrame DeepCopy()
        {
            KeyFrame ReturnFrame = new KeyFrame();
            ReturnFrame.connections = new short[8];
            ReturnFrame.periods = new float[8];
            for (int j = 0; j < u1; j++)
            {
                ReturnFrame.connections[j] = connections[j];
                ReturnFrame.periods[j] = periods[j];
            }

            ReturnFrame.u1 = u1;
            ReturnFrame.u2 = u2;
            ReturnFrame.u3 = u3;
            ReturnFrame.pitch = pitch;
            ReturnFrame.yaw = yaw;
            ReturnFrame.roll = roll;
            ReturnFrame.x = x;
            ReturnFrame.y = y;
            ReturnFrame.z = z;
            ReturnFrame.speed = speed;
            return ReturnFrame;
        }
        
    }

    /*
     * Rail structure
     */
    public class Rail
    {
        public string name;         //Name of rail
        public KeyFrame[] frames;   //Array of frames

        /*
         * Creates new rail with default name
         */
        public Rail()
        {
            name = "New Rail";
            frames = new KeyFrame[0];
        }

        /*
         * Creates new rail with given name
         */
        public Rail(string nm)
        {
            name = nm;
            frames = new KeyFrame[0];
        }

        /*
         * Swaps position of 2 frames
         */
        public void SwapFrames(int p1, int p2)
        {
            if (p1 < 0 || p2 < 0)
                return;
            if (p1 >= frames.Length || p2 >= frames.Length)
                return;

            KeyFrame rp1 = frames[p1];
            KeyFrame rp2 = frames[p2];

            frames[p1] = rp2;
            frames[p2] = rp1;
        }

        /*
         * Inserts a frame at given point
         */
        public void InsertFrame(KeyFrame point, int p)
        {
            if (frames.Length == int.MaxValue)
                return;
            if (p < 0)
                p = 0;
            KeyFrame[] npoints = new KeyFrame[frames.Length + 1];
            for (int i = 0; i < frames.Length + 1; i++)
            {
                if (i < p)
                    npoints[i] = frames[i];
                else if (i == p)
                    npoints[i] = point;
                else
                    npoints[i] = frames[i - 1];
            }
            frames = npoints;
        }

        /*
         * Removes the frame at given point
         */
        public void RemoveFrame(int p)
        {
            if (frames.Length <= 0)
                return;
            KeyFrame[] npoints = new KeyFrame[frames.Length - 1];
            for (int i = 0; i < frames.Length - 1; i++)
            {
                if (i < p)
                    npoints[i] = frames[i];
                else
                    npoints[i] = frames[i + 1];
            }
            frames = npoints;
        }

        /*
         * Reads data from stream
         */
        public bool ReadData(Stream file)
        {
            Data.IsBigEndian = true;
            uint count = Data.ReadUInt32(file);
            uint d1 = Data.ReadUInt32(file);
            uint d2 = Data.ReadUInt32(file);

            if (count == 0)
                return true;

            long spos = file.Position;
            file.Position = d1;
            name = Data.ReadString(file);
            file.Position = d2;
            frames = new KeyFrame[count];
            for (int i = 0; i < count; i++)
            {
                frames[i] = new KeyFrame();
                frames[i].ReadData(file);
            }
            file.Position = spos;
            return false;
        }

        /*
         * Saves data to stream, and outputs buffer locations
         */
        public void SaveData(Stream file, ref long hbuffer, ref long nbuffer, ref long dbuffer)
        {
            Data.IsBigEndian = true;
            file.Seek(hbuffer, SeekOrigin.Begin);
            Data.WriteUInt32(file, (UInt32)frames.Length);
            Data.WriteUInt32(file, (UInt32)nbuffer);
            Data.WriteUInt32(file, (UInt32)dbuffer);
            hbuffer = file.Position;

            file.Seek(nbuffer, SeekOrigin.Begin);
            Data.WriteString(file, name);
            nbuffer = file.Position;

            file.Seek(dbuffer, SeekOrigin.Begin);
            for (int i = 0; i < frames.Length; i++)
                frames[i].WriteData(file);
            dbuffer = file.Position;
        }

        /*
         * Size
         */
        public long GetSize()
        {
            return HeaderSize() + NameSize() + DataSize();
        }

        /*
         * Size of header
         */
        public long HeaderSize()
        {
            return 12;
        }

        /*
         * Size of name
         */
        public long NameSize()
        {
            return name.Length + 1;
        }

        /*
         * Size of data
         */
        public long DataSize()
        {
            return 68 * frames.Length;
        }

        public Rail DeepCopy()
        {
            Rail ReturnRail = new Rail();
            ReturnRail.name = name;
            ReturnRail.frames = new KeyFrame[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                ReturnRail.frames[i] = frames[i].DeepCopy();
            }
            return ReturnRail;
        }
    }

    /*
    * Ral File Reader
    */
    public class RalFile
    {
        private readonly byte[] NULLDAT = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };     //Null data constant

        private FileStream file;    //File Stream

        private List<Rail> rails;   //Rail array

        /*
         * Count of rails
         */
        public int Count
        {
            get { return rails.Count; }
        }

        /*
         * Creates empty, nameless file
         */
        public RalFile()
        {
            rails = new List<Rail>();
            file = null;
        }

        /*
         * Creates object and loads from file
         */
        public RalFile(string filename)
        {
            rails = new List<Rail>();
            file = null;
            Load(filename);
        }

        /*
         * Clears all data, and creates new ral file
         */
        public void NewFile(string filename)
        {
            if (file != null)
                file.Close();

            file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);

            rails.Clear();
            file.Write(NULLDAT, 0, 12);
            file.Flush();
        }

        /*
         * Loads data from file
         */
        public void Load(string filename)
        {
            if (file != null)
                file.Close();

            file = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);

            bool done = false;
            while (!done)
            {
                Rail rl = new Rail();
                if (rl.ReadData(file))
                    break;
                rails.Add(rl);
            }
        }

        /*
         * Saves data to current file
         */
        public void Save()
        {
            if (file == null)
                return;

            long hbuffer = GetHeaderStart();
            long nbuffer = GetNameStart();
            long dbuffer = GetDataStart();

            file.SetLength(GetFileSize());
            file.Seek(hbuffer, SeekOrigin.Begin);
            for (int i = 0; i < rails.Count; i++)
                rails[i].SaveData(file, ref hbuffer, ref nbuffer, ref dbuffer);

            file.Seek(hbuffer, SeekOrigin.Begin);
            for (int i = 0; i < 12; i++)
                file.WriteByte(0);

            file.Flush();
        }

        /*
         * Switches filename and saves data
         */
        public void SaveAs(string filename)
        {
            if (file != null)
                file.Close();

            file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);

            long hbuffer = GetHeaderStart();
            long nbuffer = GetNameStart();
            long dbuffer = GetDataStart();

            file.SetLength(GetFileSize());
            file.Seek(hbuffer, SeekOrigin.Begin);
            for (int i = 0; i < rails.Count; i++)
                rails[i].SaveData(file, ref hbuffer, ref nbuffer, ref dbuffer);

            file.Seek(hbuffer, SeekOrigin.Begin);
            for (int i = 0; i < 12; i++)
                file.WriteByte(0);

            file.Flush();
        }

        /*
         * Outputs all rails to an array
         */
        public Rail[] GetAllRails()
        {
            return rails.ToArray();
        }

        /*
         * Gets rail from name
         */
        public Rail GetRail(string name)
        {
            int id = GetRailID(name);
            if (id == -1)
                return null;
            return rails[id];
        }

        /*
         * Gets rail from id
         */
        public Rail GetRail(int id)
        {
            if (id < 0 || id > rails.Count)
                return null;
            return rails[id];
        }

        /*
         * Renames rail
         */
        public bool RenameRail(string railname, string newname)
        {
            int id = GetRailID(railname);
            if (id == -1)
                return false;
            if (ContainsRail(newname))
                return false;
            rails[id].name = newname;
            return true;
        }

        /*
         * Adds rail
         */
        public bool AddRail(string railname = "New Rail")
        {
            Rail rally = new Rail(railname);

            if (ContainsRail(rally.name))
                return false;

            rails.Add(rally);
            return true;
        }

        /*
         * Removes rail
         */
        public bool RemoveRail(string railname)
        {
            int id = GetRailID(railname);
            if (id == -1)
                return false;
            rails.RemoveAt(id);
            return true;
        }

        /*
         * Unloads object
         */
        public void UnLoad()
        {
            if (file != null)
                file.Close();
            rails.Clear();
        }

        /*
         * Gets id from rail name
         */
        public int GetRailID(string key)
        {
            for (int i = 0; i < rails.Count; i++)
            {
                if (rails[i].name == key)
                    return i;
            }
            return -1;
        }

        /*
         * Returns whether or not name is taken
         */
        public bool ContainsRail(string key)
        {
            for (int i = 0; i < rails.Count; i++)
            {
                if (rails[i].name == key)
                    return true;
            }
            return false;
        }

        /*
         * Returns file size
         */
        public long GetFileSize()
        {
            long size = 0;
            for (int i = 0; i < rails.Count; i++)
                size += rails[i].GetSize();
            size += GetBufferSize();
            return size;
        }
        private long GetHeaderStart()
        {
            return 0;
        }
        private long GetNameStart()
        {
            long size = 0;
            for (int i = 0; i < rails.Count; i++)
                size += rails[i].HeaderSize();
            size += 12;
            return size;
        }
        private long GetDataStart()
        {
            long size = 0;
            for (int i = 0; i < rails.Count; i++)
                size += rails[i].HeaderSize() + rails[i].NameSize();
            size += 12;
            size += GetBufferSize();
            return size;
        }

        /*
         * Nintendo buffer text
         */
        private string GetBufferText()
        {
            long size = 0;
            for (int i = 0; i < rails.Count; i++)
                size += rails[i].HeaderSize() + rails[i].NameSize();
            switch (size % 4)
            {
                case 1:
                    return "Thi";
                case 2:
                    return "Th";
                case 3:
                    return "T";
                default:
                    return "";
            }
        }

        /*
         * Returns amount of empty space
         */
        private long GetBufferSize()
        {
            long size = 0;
            for (int i = 0; i < rails.Count; i++)
                size += rails[i].HeaderSize() + rails[i].NameSize();
            size += 12;
            return 4 - (size % 4);
        }
    }
}
