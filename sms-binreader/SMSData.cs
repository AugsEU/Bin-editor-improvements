using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReader;

//For crawling through and reading object data from Start.dol
namespace SMSReader
{
    public static class SMSData
    {
        const uint DOL_START = 0x80003000;

        const uint DATA_TABLE_POINTER = 0x003C5580;
        const uint DATA_TABLE_SIZE = 0x5a0/4;

        public class Object_Data
        {
            public string key = null;
            public ushort flags;
            public ushort id;
            public string manager = null;
            public string guru = null;
            public Anim_Info anim = null;
            public Hit_Info hit = null;
            public Col_Info col = null;
            public Sound_Info sound = null;
            public Physical_Info physical = null;
            private uint u1;
            private uint u2;
            private uint u3;
            private ushort u4;
            private ushort u5;
            public uint flags2;
            public uint zero;
        }
        public class Anim_Info
        {
            public ushort flags;
            public ushort id;
            public string[] animstrings;
        }
        public class Col_Info
        {
            public ushort flags;
            public ushort id;
            public string[] colstrings;
        }
        public class Hit_Info
        {
            public float u1;
            public float u2;
            public float u3;
            public float u4;
        }
        public class Sound_Info
        {

        }
        public class Physical_Info
        {

        }

        public static Dictionary<string, Object_Data> ScrapeData(string dolfile)
        {
            Dictionary<string, Object_Data> ObjectData = new Dictionary<string, Object_Data>();
            uint[] addresses = new uint[DATA_TABLE_SIZE];
            FileStream fs = new FileStream(dolfile, FileMode.Open, FileAccess.Read);
            Data.IsBigEndian = true;
            fs.Seek(DATA_TABLE_POINTER, SeekOrigin.Begin);
            for (int i = 0; i < DATA_TABLE_SIZE; i++)
                addresses[i] = Data.ReadUInt32(fs);

            ObjectData = new Dictionary<string, Object_Data>();
            int c = 1;
            foreach (uint address in addresses)
            {
                fs.Seek(MemoryAddressToDolAddress(address), SeekOrigin.Begin);
                Object_Data cur = ReadData(fs);
                ObjectData.Add(cur.key != null ? cur.key : ("no_data" + (c++)), cur);
            }
            return ObjectData;
        }

        private static Object_Data ReadData(Stream stream)
        {
            Object_Data od = new Object_Data();
            uint keyaddr = Data.ReadUInt32(stream);
            od.flags = Data.ReadUInt16(stream);
            od.id = Data.ReadUInt16(stream);
            uint manageraddr = Data.ReadUInt32(stream);
            uint guruaddr = Data.ReadUInt32(stream);
            uint animaddr = Data.ReadUInt32(stream);
            uint hitaddr = Data.ReadUInt32(stream);
            uint coladdr = Data.ReadUInt32(stream);
            uint soundaddr = Data.ReadUInt32(stream);
            uint physicaladdr = Data.ReadUInt32(stream);
            stream.Seek(20, SeekOrigin.Current);
            od.flags2 = Data.ReadUInt32(stream);
            od.zero = Data.ReadUInt32(stream);

            if (MemoryAddressToDolAddress(keyaddr) >= stream.Length)
            {
                if (keyaddr != 0)
                    od.key = GetStringAtAddress(stream, keyaddr - 0x80412d90 + 0x003EB630);
                if (manageraddr != 0)
                    od.manager = GetStringAtAddress(stream, manageraddr - 0x80412d90 + 0x003EB630);
                if (guruaddr != 0)
                    od.guru = GetStringAtAddress(stream, guruaddr - 0x80412d90 + 0x003EB630);
            }
            else
            {
                if (keyaddr != 0)
                    od.key = GetStringAtAddress(stream, MemoryAddressToDolAddress(keyaddr));
                if (manageraddr != 0)
                    od.manager = GetStringAtAddress(stream, MemoryAddressToDolAddress(manageraddr));
                if (guruaddr != 0)
                    od.guru = GetStringAtAddress(stream, MemoryAddressToDolAddress(guruaddr));
            }

            if (animaddr != 0)
            {
                stream.Seek(animaddr - 0x80412d90 + 0x003EB630, SeekOrigin.Begin);
                od.anim = ReadAnimInfo(stream);
            }
            if (hitaddr != 0)
            {
                stream.Seek(coladdr - 0x80412d90 + 0x003EB630, SeekOrigin.Begin);
                od.hit = new Hit_Info();
                od.hit.u1 = Data.ReadSingle(stream);
                od.hit.u2 = Data.ReadSingle(stream);
                od.hit.u3 = Data.ReadSingle(stream);
                od.hit.u4 = Data.ReadSingle(stream);
            }
            return od;
        }
        private static Anim_Info ReadAnimInfo(Stream stream)
        {
            Anim_Info ai = new Anim_Info();
            ai.flags = Data.ReadUInt16(stream);
            ai.id = Data.ReadUInt16(stream);
            uint tableaddr = Data.ReadUInt32(stream);
            if (tableaddr == 0)
                return ai;
            stream.Seek(MemoryAddressToDolAddress(tableaddr), SeekOrigin.Begin);
            ai.animstrings = new string[0x50 / 4];
            for (int i = 0; i < 0x50 / 4; i++)
            {
                uint address = Data.ReadUInt32(stream);
                if (address <= 0x80000000 || address > DOL_START + 0x003F0100)  //This shouldnt have worked
                    continue;
                long last = stream.Position;
                ai.animstrings[i] = GetStringAtAddress(stream, MemoryAddressToDolAddress(address));
                stream.Seek(last, SeekOrigin.Begin);
            }
            return ai;
        }

        private static string GetStringAtAddress(Stream stream, uint address)
        {
            stream.Seek(address, SeekOrigin.Begin);
            return Data.ReadString(stream);
        }

        private static uint MemoryAddressToDolAddress(uint memaddress)
        {
            return memaddress - DOL_START;
        }
        private static uint DolAddressToMemoryAddress(uint doladdress)
        {
            return DOL_START + doladdress;
        }
    }
}
