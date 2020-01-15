using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataReader;

namespace SMSReader
{
    public class BmgFile
    {
        /* Flags for each string */
        public struct BMGFlags
        {
            private byte[] data;

            public BMGFlags(byte[] newflags)
            {
                if (newflags == null)
                    data = new byte[8];
                else if (newflags.Length != 8)
                    data = new byte[8];
                else
                    data = newflags;
            }
            public BMGFlags(Stream stream)
            {
                data = Data.Read(stream, 8);
            }
            public byte GetFlag(int id)
            {
                return data[id];
            }
            public BMGFlags SetFlag(int id, byte val)
            {
                data[id] = val;
                return this;
            }
            public void Write(Stream stream)
            {
                if (data.Length != 8)
                    throw new IndexOutOfRangeException("This should never happen.");
                Data.Write(stream, data);
            }
        }

        private const string MAGIC = "MESGbmg1";

        private List<string> StringTable;
        private List<BMGFlags> StringFlags;

        private UInt32 headUnknown;
        private UInt16 infUnk1;
        private UInt16 infUnk2;

        private UInt16 nSize;

        FileStream file;

        /* Contructor, loads file */
        public BmgFile(string filepath)
        {
            StringTable = new List<string>();
            StringFlags = new List<BMGFlags>();

            file = new FileStream(filepath, FileMode.Open);
            Read();
        }

        /* Returns amount of strings */
        public int Count
        {
            get { return StringTable.Count; }
        }

        /* Returns size of INF1 section */
        private long GetINF1Size()
        {
            long size = 16 + nSize * StringTable.Count;
            if (size % 32 != 0)
                size += 32 - (size % 32);
            return size;
        }
        /* Returns size of alignment buffer at end of INF1 section */
        private long GetINF1BufferSize()
        {
            long infsize = 16 + nSize * StringTable.Count;
            if (infsize % 32 != 0)
                return 32 - (infsize % 32);
            return 0;
        }
        /* Returns size of DAT1 section */
        private long GetDAT1Size()
        {
            long size = 9;
            for (int i = 0; i < StringTable.Count; i++)
                size += StringTable[i].Length + 1;
            if (size % 32 != 0)
                size += 32 - (size % 32);
            return size;
        }
        /* Returns size of alignment buffer at end of DAT1 section */
        private long GetDAT1BufferSize()
        {
            long datsize = 8;
            for (int i = 0; i < StringTable.Count; i++)
                datsize += StringTable[i].Length;
            if (datsize % 32 != 0)
                return 32 - (datsize % 32);
            return 0;
        }
        /* Returns the offset of the string in the file */
        private long GetStringOffset(int id)
        {
            int offset = 1 + id;
            for (int i = 0; i < id; i++)
                offset += StringTable[i].Length;
            return offset;
        }
        /* Returns total file size */
        private long GetFileSize()
        {
            return 32 + GetINF1Size() + GetDAT1Size();
        }

        /* Reads BMG file */
        private void Read()
        {
            Data.IsBigEndian = true;

            //Need these for later
            UInt16 numMes = 0;
            nSize = 0;
            infUnk1 = 0;
            infUnk2 = 0;

            UInt32[] strOffsets = null;
            BMGFlags[] strFlags = null;

            //Load disposable header information
            string magic = Data.ReadString(file, 8);

            if (magic != MAGIC)
                throw new FileLoadException("Invalid file type.");

            UInt32 size = Data.ReadUInt32(file);
            UInt32 sectioncount = Data.ReadUInt32(file);
            headUnknown = Data.ReadUInt32(file);
            file.Seek(12, SeekOrigin.Current);    //Skip over null bytes

            //Clear tables
            StringTable.Clear();
            StringFlags.Clear();

            //Loop through sections
            bool inf1 = false;
            for (int i = 0; i < sectioncount; i++)
            {
                if (file.Position % 32 != 0)
                    file.Seek(((file.Position / 32) + 1) * 32, SeekOrigin.Begin);
                string secmagic = Data.ReadString(file, 4);
                UInt32 secsize = Data.ReadUInt32(file);
                switch (secmagic)
                {
                    case "INF1":    //Read INF1 section
                        if (inf1)
                            throw new FileLoadException("More than one index table in file.");
                        numMes = Data.ReadUInt16(file);
                        nSize = Data.ReadUInt16(file);
                        infUnk1 = Data.ReadUInt16(file);
                        infUnk2 = Data.ReadUInt16(file);

                        switch (nSize)
                        {
                            case 12:
                                strOffsets = new UInt32[numMes];
                                strFlags = new BMGFlags[numMes];
                                for (int j = 0; j < numMes; j++)
                                {
                                    strOffsets[j] = Data.ReadUInt32(file);
                                    strFlags[j] = new BMGFlags(file);
                                }
                                break;
                            case 8:
                                strOffsets = new UInt32[numMes];
                                strFlags = new BMGFlags[numMes];
                                for (int j = 0; j < numMes; j++)
                                {
                                    strOffsets[j] = Data.ReadUInt32(file);
                                    List<byte> list = new List<byte>();
                                    list.AddRange(BitConverter.GetBytes(Data.ReadUInt32(file)));
                                    list.AddRange(new byte[4]);
                                    strFlags[j] = new BMGFlags(list.ToArray());
                                }
                                break;
                            case 4:
                                strOffsets = new UInt32[numMes];
                                strFlags = new BMGFlags[numMes];
                                for (int j = 0; j < numMes; j++)
                                {
                                    strOffsets[j] = Data.ReadUInt32(file);
                                    strFlags[j] = new BMGFlags(new byte[8]);
                                }
                                break;
                            default: throw new NotImplementedException("Unknown index table type.");
                        }

                        inf1 = true;
                        break;
                    case "DAT1":    //Read DAT1 section
                        if (!inf1)
                            throw new FileLoadException("String pool located too early in file.");

                        long datOffset = file.Position;

                        for (int j = 0; j < numMes; j++)
                        {   //Read all strings and save string data
                            string str = "";

                            file.Seek(datOffset + strOffsets[j], SeekOrigin.Begin);

                            long end = 0;
                            if (j == numMes - 1)
                                end = file.Length - datOffset;
                            else
                                end = strOffsets[j + 1];

                            //Read string like this, because I still don't know SMS escape codes
                            while (file.Position < datOffset + end - 1)
                                str += Data.ReadChar(file);
                            Data.ReadByte(file);

                            //Clean up string
                            if (j == numMes - 1)
                                while (str.Length > 0 && str[str.Length - 1] == '\0')
                                    str = str.Substring(0, str.Length - 1);

                                    StringTable.Add(str);
                            StringFlags.Add(strFlags[j]);
                        }
                        break;
                    default: throw new NotImplementedException("Unknown BMG section.");
                }
            }
        }
        /* Save to different file */
        public void Save(string filepath)
        {
            if (file != null)
                file.Close();
            file = new FileStream(filepath, FileMode.Create);
            Save();
        }
        /* Saves BMG File */
        public void Save()
        {
            long size = GetFileSize();
            file.SetLength(size);
            file.Seek(0, SeekOrigin.Begin);

            //Write header
            Data.WriteString(file, MAGIC, false);
            Data.WriteUInt32(file, (UInt32)(size / 32L));
            Data.WriteUInt32(file, 2);  //Sections
            Data.WriteUInt32(file, headUnknown);
            for (int i = 0; i < 12; i++)
                Data.WriteByte(file, 0);

            /* INF1 */
            Data.WriteString(file, "INF1", false);
            Data.WriteUInt32(file, (UInt32)GetINF1Size());
            Data.WriteUInt16(file, (UInt16)StringTable.Count);
            Data.WriteUInt16(file, nSize);
            Data.WriteUInt16(file, infUnk1);
            Data.WriteUInt16(file, infUnk2);

            switch (nSize)
            {
                case 12:
                    for (int i = 0; i < StringTable.Count; i++)
                    {
                        Data.WriteUInt32(file, (UInt32)GetStringOffset(i));
                        StringFlags[i].Write(file);
                    }
                    break;
                case 8:
                    for (int i = 0; i < StringTable.Count; i++)
                    {
                        Data.WriteUInt32(file, (UInt32)GetStringOffset(i));
                        Data.WriteByte(file, StringFlags[i].GetFlag(0));
                        Data.WriteByte(file, StringFlags[i].GetFlag(1));
                        Data.WriteByte(file, StringFlags[i].GetFlag(2));
                        Data.WriteByte(file, StringFlags[i].GetFlag(3));
                    }
                    break;
                case 4:
                    for (int i = 0; i < StringTable.Count; i++)
                        Data.WriteUInt32(file, (UInt32)GetStringOffset(i));
                    break;
                default: throw new NotImplementedException("Unknown index table type.");
            }

            //Align to 32-byte block
            while (file.Position % 32 != 0)
                file.WriteByte(0);

            /* DAT1 */
            Data.WriteString(file, "DAT1", false);
            Data.WriteUInt32(file, (UInt32)GetDAT1Size());
            Data.WriteByte(file, 0);
            for (int i = 0; i < StringTable.Count; i++){
                for (int j = 0; j < StringTable[i].Length; j++)
                    Data.WriteChar(file, StringTable[i][j]);
                Data.WriteByte(file, 0);
            }

            //Align to 32-byte block
            while (file.Position % 32 != 0)
                file.WriteByte(0);
            file.Flush();
        }

        /* Close file and clean up */
        public void Close()
        {
            file.Close();
            file = null;
            StringTable = null;
            StringFlags = null;
        }

        /* Returns all strings in string table */
        public string[] GetAllStrings()
        {
            return StringTable.ToArray();
        }
        /* Gets string at position */
        public string GetString(int id)
        {
            if (id >= StringTable.Count)
                return "";
            return StringTable[id];
        }
        /* Returns a readable string with text replacing escape codes */
        public string GetStringReadable(int id)
        {
            if (id >= StringTable.Count)
                return "";
            string english = "";
            for (int i = 0; i < StringTable[id].Length; i++)
            {
                if (StringTable[id][i] == 0x1a)
                    english += "[esc]";
                else if (StringTable[id][i] == '\n')
                    english += "\r\n";
                else if (StringTable[id][i] < 0x20 || StringTable[id][i] > 0x80)        //Until we know all escape codes
                    english += "[" + ((byte)StringTable[id][i]).ToString("X2") + "]";
                else if (IsEscape(StringTable[id][i]))
                    english += ReadEscapeSeq(StringTable[id], ref i);
                else
                    english += StringTable[id][i];
            }
            return english;
        }
        /* Sets the string at given index to given string */
        public void SetString(int id, string str)
        {
            if (id >= StringTable.Count)
                return;
            StringTable[id] = str;
        }
        /* Sets string at given index to given string with replaced escape characters */
        public void SetStringReadable(int id, string str)
        {
            if (id >= StringTable.Count)
                return;
            string machineCode = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '%' || str[i] == '\\' || str[i] == '[' || str[i] == '\r')
                    machineCode += ParseEscapeSeqStr(str, ref i);
                else
                    machineCode += str[i];
            }
            StringTable[id] = machineCode;
        }

        /* Returns sound effect id for string at index */
        public byte GetSoundEffectID(int id)
        {
            if (id >= StringFlags.Count)
                return 0x00000000;
            return StringFlags[id].GetFlag(4);
        }
        /* Sets sound effect id for string at index */
        public void SetSoundEffectID(int id, byte flag)
        {
            if (id < 0 || id >= StringFlags.Count)
                return;
            BMGFlags flg = StringFlags[id];
            flg.SetFlag(4, flag);
            StringFlags[id] = flg;
        }

        /* Add / Remove */
        public void RemoveAt(int id)
        {
            if (id < 0 || id >= StringTable.Count)
                return;
            StringTable.RemoveAt(id);
            StringFlags.RemoveAt(id);
        }
        public void Add(string str)
        {
            StringTable.Add(str);
            StringFlags.Add(new BMGFlags(new byte[8]));
        }
        public void Insert(int pos, string str, UInt16 val1 = 0, UInt16 val2 = 0, UInt16 val3 = 0, UInt16 val4 = 0)
        {
            if (pos < 0 || pos >= StringTable.Count)
                return;
            StringTable.Insert(pos, str);
            StringFlags.Insert(pos, new BMGFlags(new byte[8]));
        }


        /* Escape sequence functions */
        /* Incomplete because not all Sunshine Escape sequences are known */

        /* Returns whether or not the given character is an escape code, or other unreadable character */
        public static bool IsEscape(char c)
        {
            switch (c)
            {
                case (char)0x1a: return true;
                case '\\': return true;
                case '\n': return true;
                case '\b': return true;
                case '\t': return true;
                default: return false;
            }
        }
        public static string ReadEscapeSeq(string str, ref int ind)
        {
            return "";
            /*byte esc = (byte)str[ind];
            if (esc != 0x1a)
            {
                switch (esc)
                {
                    case (byte)'\\': return "\\\\";
                    case (byte)'\n': return "\\n";
                    case (byte)'\b': return "\\b";
                    case (byte)'\t': return "\\t";
                    default: throw new NotImplementedException("Unknown escape sequence.");
                }
            }
            byte size = (byte)str[++ind];

            switch (size)
            {
                case 8:
                    byte id8 = (byte)str[++ind];
                    switch (id8)
                    {
                        case 1:
                            ind++;

                            string arg1 = "";
                            string arg2 = "";
                            string arg3 = "";

                            while (str[ind] != 0)
                                arg1 += str[++ind];
                            ind++;
                            while (str[ind] != 0)
                                arg2 += str[++ind];
                            ind++;
                            while (str[ind] != 0)
                                arg3 += str[++ind];

                            return "%Question(" + arg1 + "," + arg2 + "," + arg3 + ")";
                        default: throw new NotImplementedException("Unknown escape code: " + id8.ToString("X2"));
                    }
                default: throw new NotImplementedException("Unknown escape code size: " + size.ToString("X2"));
            }*/
        }

        /* Used only to get byte sequences until escape codes are known */
        public static string ParseEscapeSeqStr(string str, ref int ind)
        {
            byte[] dat = ParseEscapeSeq(str, ref ind);

            string outp = "";
            for (int i = 0; i < dat.Length; i++)
                outp += (char)dat[i];
            return outp;
        }
        public static byte[] ParseEscapeSeq(string str, ref int ind)
        {
            char esc = str[ind];
            if (esc != '%')
            {
                if (esc == '\r')
                    return new byte[0];
                if (esc == '\\')
                {
                    switch (str[++ind])
                    {
                        case '\\': return new byte[1] { (byte)'\\' };
                        case 'n': return new byte[1] { (byte)'\n' };
                        case 'b': return new byte[1] { (byte)'\b' };
                        case 't': return new byte[1] { (byte)'\b' };
                        default: return new byte[1] { (byte)str[ind] };
                    }
                }
                else if (esc == '[')
                {
                    if (str[ind + 1] == '[')
                    {
                        ind++;
                        return new byte[1] { (byte)'[' };
                    }

                    string escstr = "";
                    while (str[ind + 1] != ']')
                        escstr += str[++ind];
                    ind++;
                    byte o = 0;
                    if (byte.TryParse(escstr,System.Globalization.NumberStyles.HexNumber, null, out o))
                        return new byte[1] { o };
                    else if (escstr == "esc")
                        return new byte[1] { 0x1a };
                }
                return new byte[0];
            }

            return new byte[0];
            //Should not get this far...
            /* string key = "";
            while (ind < str.Length-1 && str[ind+1] != ' ' && str[ind+1] != '(')
                key += str[++ind];

            string[] args = new string[0];
            if (str[ind] == '(')
                args = str.Substring(ind + 1, str.Length - ind - 1).Split(',');

            switch (key)
            {
                case "%": return new byte[] { (byte)'%' };
                case "Question":
                    byte[] bout = new byte[7 + args[0].Length + args[1].Length + args[2].Length];
                    bout[0] = 0x1a;
                    bout[1] = 0x08;
                    bout[2] = 0x01;
                    return new byte[3] { 0x1a, 0x08, 0x01 };
                default:
                    byte[] val = new byte[key.Length + 1];
                    val[0] = (byte)'%';
                    for (int i = 0; i < key.Length; i++)
                        val[i + 1] = (byte)key[i];
                    return val;
            }*/
        }
    }
}
