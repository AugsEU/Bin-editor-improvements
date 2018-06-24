using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/* Simple Data Reader */
namespace DataReader
{
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        public float _Yaw
        {
            get { return X; }
            set { X = value; }
        }
        public float _Pitch
        {
            get { return Y; }
            set { Y = value; }
        }
        public float _Roll
        {
            get { return Z; }
            set { Z = value; }
        }
        public float Length
        {
            get { return (float)Math.Pow(X, 2) + (float)Math.Pow(Y, 2) + (float)Math.Pow(Z, 2); }
        }

        public static Vector Zero { get { return new Vector(0f, 0f, 0f); } }
        public static Vector Unit { get { return new Vector(0.5773503f, 0.5773503f, 0.5773503f); } }
        public static Vector UnitX { get { return new Vector(1f, 0f, 0f); } }
        public static Vector UnitY { get { return new Vector(0f, 1f, 0f); } }
        public static Vector UnitZ { get { return new Vector(0f, 0f, 1f); } }

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return "{ " + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + " }";
        }

        public static Vector Parse(string val)
        {
            Vector v = new Vector();
            string[] vals = val.Split(' ', '}', '{', ',');
            v.X = float.Parse(vals[0]);
            v.Y = float.Parse(vals[1]);
            v.Z = float.Parse(vals[2]);
            return v;
        }
        public static bool TryParse(string val, out Vector v)
        {
            v = Vector.Zero;

            string[] vals = val.Split(new char[] { ' ', '}', '{', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length != 3)
                return false;
            if (!float.TryParse(vals[0], out v.X)) return false;
            if (!float.TryParse(vals[1], out v.Y)) return false;
            if (!float.TryParse(vals[2], out v.Z)) return false;
            return true;
        }
    }
    class DataManipulation
    {
        //Gamecube is big endian and modern computers are not so this should fix things.
        /// <summary>
        /// Converts big endian to little endian and back if the computer is little endian
        /// </summary>
        /// <param name="data">Bytes</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="length">Length</param>
        /// <returns></returns>
        public static byte[] SwapEndian(byte[] data, int startIndex = 0, int length = 0)
        {
            if (BitConverter.IsLittleEndian == false)
                return data;

            if (length == 0)
                length = data.Length;

            byte[] newdata = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int position = startIndex + length - 1 - i;
                if (position < 0 || position > data.Length - 1)
                {
                    newdata[i] = 0;
                    continue;
                }

                newdata[i] = data[position];
            }

            return newdata;
        }

        /// <summary>
        /// Converts a string name to an enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="name">String</param>
        /// <returns></returns>
        public static T GetEnumFromString<T>(string name)
        {
            string[] enumNames = Enum.GetNames(typeof(T));

            for (int i = 0; i < enumNames.Length; i++)
            {
                if (name == enumNames[i])
                    return (T)Enum.GetValues(typeof(T)).GetValue(i);
            }

            return (T)((object)0);
        }

        /// <summary>
        /// Reads a shift-jis string from the bin file.
        /// </summary>
        /// <param name="stream">Stream to read the string from</param>
        /// <param name="length">Length of the string</param>
        /// <returns></returns>
        public static string ReadString(Stream stream, int length)
        {
            byte[] data = new byte[length];

            for (int i = 0; i < length; i++)
                data[i] = (byte)stream.ReadByte();

            return Encoding.GetEncoding("shift-jis").GetString(data);
        }
    }
    public static class Data
    {
        private static bool Endian;

        public static bool IsLittleEndian
        {
            get { return Endian; }
            set { Endian = value; }
        }
        public static bool IsBigEndian
        {
            get { return !Endian; }
            set { Endian = !value; }
        }

        public static void TestEndian(byte[] data)
        {
            if (!Endian)
            {
                byte[] databuf = new byte[data.Length];
                data.CopyTo(databuf, 0);
                for (int i = 0; i < databuf.Length; i++)
                    data[i] = databuf[databuf.Length - i - 1];
            }
        }
        public static string ReadString(Stream fs)
        {
            string ostr = "";
            char c;
            while ((c = (char)fs.ReadByte()) != '\0')
                ostr += c;
            return ostr;
        }
        public static string ReadString(Stream fs, Encoding enc)
        {
            List<byte> strdat = new List<byte>();
            while (true)
            {
                byte cbyte = ReadByte(fs);
                if (cbyte == 0)
                    break;
                strdat.Add(cbyte);
            }
            return enc.GetString(strdat.ToArray());
        }
        public static string ReadString(Stream fs, int size)
        {
            string ostr = "";
            for (int i = 0; i < size; i++)
                ostr += (char)(byte)fs.ReadByte();
            return ostr;
        }
        public static string ReadString(Stream fs, int size, Encoding enc)
        {
            byte[] sb = new byte[size];
            fs.Read(sb, 0, size);
            return enc.GetString(sb);
        }
        public static char ReadChar(Stream fs)
        {
            return (char)(byte)fs.ReadByte();
        }
        public static Vector ReadVector(Stream fs)
        {
            Vector ovec = new Vector();
            ovec.X = ReadSingle(fs);
            ovec.Y = ReadSingle(fs);
            ovec.Z = ReadSingle(fs);
            return ovec;
        }
        public static double ReadDouble(Stream fs)
        {
            byte[] data = new byte[8];
            fs.Read(data, 0, 8);
            TestEndian(data);
            return BitConverter.ToDouble(data, 0);
        }
        public static float ReadSingle(Stream fs)
        {
            byte[] data = new byte[4];
            fs.Read(data, 0, 4);
            TestEndian(data);
            return BitConverter.ToSingle(data, 0);
        }
        public static UInt64 ReadUInt64(Stream fs)
        {
            byte[] data = new byte[8];
            fs.Read(data, 0, 8);
            TestEndian(data);
            return BitConverter.ToUInt64(data, 0);
        }
        public static Int64 ReadInt64(Stream fs)
        {
            byte[] data = new byte[8];
            fs.Read(data, 0, 8);
            TestEndian(data);
            return BitConverter.ToInt64(data, 0);
        }
        public static UInt32 ReadUInt32(Stream fs)
        {
            byte[] data = new byte[4];
            fs.Read(data, 0, 4);
            TestEndian(data);
            return BitConverter.ToUInt32(data, 0);
        }
        public static Int32 ReadInt32(Stream fs)
        {
            byte[] data = new byte[4];
            fs.Read(data, 0, 4);
            TestEndian(data);
            return BitConverter.ToInt32(data, 0);
        }
        public static UInt16 ReadUInt16(Stream fs)
        {
            byte[] data = new byte[2];
            fs.Read(data, 0, 2);
            TestEndian(data);
            return BitConverter.ToUInt16(data, 0);
        }
        public static Int16 ReadInt16(Stream fs)
        {
            byte[] data = new byte[2];
            fs.Read(data, 0, 2);
            TestEndian(data);
            return BitConverter.ToInt16(data, 0);
        }
        public static byte ReadByte(Stream fs)
        {
            return (byte)fs.ReadByte();
        }
        public static bool ReadBool(Stream fs)
        {
            return fs.ReadByte() != 0;
        }

        public static byte[] Read(Stream fs, int count)
        {
            byte[] outp = new byte[count];
            fs.Read(outp, 0, count);
            return outp;
        }
        public static byte[] ReadEndian(Stream fs, int count)
        {
            if (Endian)
                return Read(fs, count);
            byte[] outp = new byte[count];
            for (int i = count - 1; i >= 0; i--)
                outp[i] = (byte)fs.ReadByte();
            return outp;
        }

        public static void WriteString(Stream fs, string str, bool nullterminate = true)
        {
            for (int i = 0; i < str.Length; i++)
                fs.WriteByte((byte)str[i]);
            if (nullterminate)
                fs.WriteByte(0);
        }
        public static void WriteString(Stream fs, string str, Encoding enc, bool nullterminate = true)
        {
            byte[] strb = enc.GetBytes(str);
            fs.Write(strb, 0, strb.Length);
            if (nullterminate)
                fs.WriteByte(0);
        }
        public static void WriteChar(Stream fs, char c)
        {
            fs.WriteByte((byte)c);
        }
        public static void WriteVector(Stream fs, Vector vec)
        {
            WriteSingle(fs, vec.X);
            WriteSingle(fs, vec.Y);
            WriteSingle(fs, vec.Z);
        }
        public static void WriteDouble(Stream fs, double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteSingle(Stream fs, float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteUInt64(Stream fs, UInt64 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteInt64(Stream fs, Int64 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteUInt32(Stream fs, UInt32 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteInt32(Stream fs, Int32 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteUInt16(Stream fs, UInt16 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteInt16(Stream fs, Int16 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            TestEndian(data);
            fs.Write(data, 0, data.Length);
        }
        public static void WriteByte(Stream fs, byte value)
        {
            fs.WriteByte(value);
        }
        public static void WriteBool(Stream fs, bool value)
        {
            if (value)
                fs.WriteByte(255);
            else
                fs.WriteByte(0);
        }
        public static void Write(Stream fs, byte[] value)
        {
            fs.Write(value, 0, value.Length);
        }
        public static void WriteEndian(Stream fs, byte[] value)
        {
            if (Endian)
                Write(fs, value);
            else
            {
                for (int i = value.Length - 1; i >= 0; i--)
                    fs.WriteByte(value[i]);
            }
        }
        public static void WriteBuffer(Stream fs, int size, byte value)
        {
            for (int i = 0; i < size; i++)
                fs.WriteByte(value);
        }
        public static void WritePadding(Stream fs, int size)
        {
            const string pad = "This is padding data to align";
            for (int i = 0; i < size; i++)
                fs.WriteByte((byte)pad[i % pad.Length]);
        }
        public static void WritePaddingFast(Stream fs, int size)
        {
            fs.Write(new byte[size], 0, size);
        }
        public static void WriteNull(Stream fs, int size)
        {
            for (int i = 0; i < size; i++)
                fs.WriteByte(0);
        }

        public static int CompareBytes(byte[] b1, byte[] b2, int c = int.MaxValue)
        {
            int i = 0;
            while (true)
            {
                if (i >= b1.Length)
                {
                    if (i >= b2.Length)
                        return 0;
                    else
                        return b2[i];
                }
                if (i >= b2.Length)
                {
                    if (i >= b1.Length)
                        return 0;
                    else
                        return b1[i];
                }

                if (b2[i] != b1[i])
                    return b2[i] - b1[i];

                if (++i == c)
                    return 0;
            }
        }
        public static int CompareBytes(byte[] b1, string str, int c = int.MaxValue)
        {
            byte[] b2 = new byte[str.Length];
            for (int i = 0; i < b2.Length; i++)
                b2[i] = (byte)str[i];
            return CompareBytes(b1, b2, c);
        }
    }
}
