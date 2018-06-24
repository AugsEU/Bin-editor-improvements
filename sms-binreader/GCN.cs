using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SMSReader;

namespace DataReader
{
    public class GCN
    {
        static readonly byte[] YAZ0 = new byte[] { 89, 97, 122, 48 };
        static readonly byte[] RARC = new byte[] { 82, 65, 82, 67 };

        #region yaz0dec
        /* Port of Yaz0Dec by thakis */
        private static void Yaz0Dec(byte[] src, byte[] dst)
        {
            int rsrc = 0;
            int rdst = 0;
            UInt32 validBitCount = 0; //number of valid bits left in "code" byte
            byte currCodeByte = 0;
            while (rdst < dst.Length)
            {
                //read new "code" byte if the current one is used up
                if (validBitCount == 0)
                {
                    currCodeByte = src[rsrc];
                    ++rsrc;
                    validBitCount = 8;
                }

                if ((currCodeByte & 0x80) != 0)
                {
                    //straight copy
                    dst[rdst] = src[rsrc];
                    rdst++;
                    rsrc++;
                    //if(r.srcPos >= srcSize)
                    //  return r;
                }
                else
                {
                    //RLE part
                    byte byte1 = src[rsrc];
                    byte byte2 = src[rsrc + 1];
                    rsrc += 2;
                    //if(r.srcPos >= srcSize)
                    //  return r;

                    UInt32 dist = (uint)(((byte1 & 0xF) << 8) | byte2);
                    UInt32 copySource = (uint)(rdst - (dist + 1));

                    UInt32 numBytes = (uint)(byte1 >> 4);
                    if (numBytes == 0)
                    {
                        numBytes = (uint)(src[rsrc] + 0x12);
                        rsrc++;
                        //if(r.srcPos >= srcSize)
                        //  return r;
                    }
                    else
                        numBytes += 2;

                    //copy run
                    for (int i = 0; i < numBytes; ++i)
                    {
                        dst[rdst] = dst[copySource];
                        copySource++;
                        rdst++;
                    }
                }

                //use next bit from "code" byte
                currCodeByte <<= 1;
                validBitCount -= 1;
            }
        }
        public static void Yaz0Dec(string src, string dest)
        {
            FileStream srcstr = new FileStream(src, FileMode.Open, FileAccess.Read);

            byte[] magic = Data.Read(srcstr, 4);
            int endsize = Data.ReadInt32(srcstr);
            Data.Read(srcstr, 8);

            if (Data.CompareBytes(magic, YAZ0) != 0)
                return;

            byte[] srcb = Data.Read(srcstr, (int)(srcstr.Length - srcstr.Position));
            srcstr.Close();

            byte[] dstb = new byte[endsize];
            Yaz0Dec(srcb, dstb);

            FileStream deststr = new FileStream(dest, FileMode.Create);
            deststr.Write(dstb, 0, dstb.Length);
            deststr.Close();
        }
        #endregion

        #region yaz0enc
        //Almost a direct port of yaz0enc by shevious
        public static void Yaz0Enc(string src, string dest, Action<int> progressAction)
        {
            FileStream srcstr = new FileStream(src, FileMode.Open, FileAccess.Read);
            byte[] dat = new byte[srcstr.Length];
            srcstr.Read(dat, 0, dat.Length);
            srcstr.Close();
            EncodeYaz0File(dat, dest, progressAction);
        }
        public static void Yaz0Enc(string src, string dest)
        {
            FileStream srcstr = new FileStream(src, FileMode.Open, FileAccess.Read);
            byte[] dat = new byte[srcstr.Length];
            srcstr.Read(dat, 0, dat.Length);
            srcstr.Close();
            EncodeYaz0File(dat, dest);
        }
        // simple and straight encoding scheme for Yaz0
        private static UInt32 SimpleEnc(byte[] src, int pos, out UInt32 pMatchPos)
        {
            const int COMP_LIMIT = 4096;
            int startPos = pos - 0x1000, j, i;
            UInt32 numBytes = 1;
            UInt32 matchPos = 0;

            if (startPos < 0)
                startPos = 0;
            for (i = startPos; i < pos; i++)
            {
                for (j = 0; j < src.Length - pos && j < COMP_LIMIT; j++)
                {
                    if (src[i + j] != src[j + pos])
                        break;
                }
                if (j > numBytes)
                {
                    numBytes = (uint)j;
                    matchPos = (uint)i;
                }
            }
            pMatchPos = matchPos;
            if (numBytes == 2)
                numBytes = 1;
            return numBytes;
        }

        // a lookahead encoding scheme for ngc Yaz0
        private static UInt32 NintendoEnc_numBytes1;
        private static UInt32 NintendoEnc_matchPos;
        private static int NintendoEnc_prevFlag = 0;
        private static UInt32 NintendoEnc(byte[] src, int pos, out UInt32 pMatchPos)
        {
            int startPos = pos - 0x1000;
            UInt32 numBytes = 1;

            // if prevFlag is set, it means that the previous position was determined by look-ahead try.
            // so just use it. this is not the best optimization, but nintendo's choice for speed.
            if (NintendoEnc_prevFlag == 1)
            {
                pMatchPos = NintendoEnc_matchPos;
                NintendoEnc_prevFlag = 0;
                return NintendoEnc_numBytes1;
            }
            NintendoEnc_prevFlag = 0;
            numBytes = SimpleEnc(src, pos, out NintendoEnc_matchPos);
            pMatchPos = NintendoEnc_matchPos;

            // if this position is RLE encoded, then compare to copying 1 byte and next position(pos+1) encoding
            if (numBytes >= 3) {
                NintendoEnc_numBytes1 = SimpleEnc(src, pos + 1, out NintendoEnc_matchPos);
                // if the next position encoding is +2 longer than current position, choose it.
                // this does not guarantee the best optimization, but fairly good optimization with speed.
                if (NintendoEnc_numBytes1 >= numBytes + 2)
                {
                    numBytes = 1;
                    NintendoEnc_prevFlag = 1;
                }
            }
            return numBytes;
        }
        private static int EncodeYaz0(byte[] src, Stream dstFile, Action<int> progressAction)
        {
            int rsrc = 0, rdest = 0;
            byte[] dst = new byte[24];    // 8 codes * 3 bytes maximum
            int dstSize = 0;
            int percent = -1;

            UInt32 validBitCount = 0; //number of valid bits left in "code" byte
            byte currCodeByte = 0;
            while (rsrc < src.Length)
            {
                UInt32 numBytes;
                UInt32 matchPos;
                UInt32 srcPosBak;

                numBytes = NintendoEnc(src, rsrc, out matchPos);
                if (numBytes < 3)
                {
                    //straight copy
                    dst[rdest] = src[rsrc];
                    rdest++;
                    rsrc++;
                    //set flag for straight copy
                    currCodeByte |= (byte)(0x80 >> (int)validBitCount);
                }
                else
                {
                    //RLE part
                    UInt32 dist = (uint)(rsrc - matchPos - 1);
                    byte byte1, byte2, byte3;

                    if (numBytes >= 0x12)  // 3 byte encoding
                    {
                        byte1 = (byte)(0 | (dist >> 8));
                        byte2 = (byte)(dist & 0xff);
                        dst[rdest++] = byte1;
                        dst[rdest++] = byte2;
                        // maximum runlength for 3 byte encoding
                        if (numBytes > 0xff + 0x12)
                            numBytes = 0xff + 0x12;
                        byte3 = (byte)(numBytes - 0x12);
                        dst[rdest++] = byte3;
                    }
                    else  // 2 byte encoding
                    {
                        byte1 = (byte)(((numBytes - 2) << 4) | (dist >> 8));
                        byte2 = (byte)(dist & 0xff);
                        dst[rdest++] = byte1;
                        dst[rdest++] = byte2;
                    }
                    rsrc += (int)numBytes;
                }
                validBitCount++;
                //write eight codes
                if (validBitCount == 8)
                {
                    dstFile.WriteByte(currCodeByte);
                    dstFile.Write(dst, 0, rdest);
                    dstSize += rdest + 1;

                    srcPosBak = (uint)rsrc;
                    currCodeByte = 0;
                    validBitCount = 0;
                    rdest = 0;
                }
                if ((rsrc + 1) * 100 / src.Length != percent)
                {
                    percent = (rsrc + 1) * 100 / src.Length;
                    progressAction(percent);
                }
            }
            if (validBitCount > 0)
            {
                dstFile.WriteByte(currCodeByte);
                dstFile.Write(dst, 0, rdest);
                dstSize += rdest + 1;

                currCodeByte = 0;
                validBitCount = 0;
                rdest = 0;
            }

            return dstSize;
        }
        private static void EncodeYaz0File(byte[] src, string dstName, Action<int> percentProgress)
        {
            int dstSize;

            FileStream DataFile = new FileStream(dstName, FileMode.Create);

            // write 4 bytes yaz0 header
            Data.Write(DataFile, YAZ0);

            // write 4 bytes uncompressed size
            Data.WriteUInt32(DataFile, (uint)src.Length);

            Data.WriteNull(DataFile, 8);

            dstSize = EncodeYaz0(src, DataFile, percentProgress);

            DataFile.Close();
        }
        private static void EncodeYaz0File(byte[] src, string dstName)
        {
            int dstSize;

            FileStream DataFile = new FileStream(dstName, FileMode.Create);

            // write 4 bytes yaz0 header
            Data.Write(DataFile, YAZ0);

            // write 4 bytes uncompressed size
            Data.WriteUInt32(DataFile, (uint)src.Length);

            Data.WriteNull(DataFile, 8);

            dstSize = EncodeYaz0(src, DataFile, new Action<int>(delegate(int i) { }));

            DataFile.Close();
        }
        #endregion

        #region ARC
        //Based off rarcdump by thakis
        public struct ArcInfo
        {
            public long dataoffset;
            public int cid;

            public ArcHeader head;
            public List<ArcNode> nodes;
            public List<ArcFileEntry> files;
            public List<string> strings;

            List<long> fileSizes;

            public ArcInfo(ArcHeader h)
            {
                dataoffset = 0;
                cid = 0;
                head = h;
                nodes = new List<ArcNode>();
                files = new List<ArcFileEntry>();
                strings = new List<string>();
                fileSizes = new List<long>();
            }
            public long Size()
            {
                long ptr = DataStart();
                foreach (long len in fileSizes)
                {
                    ptr += len;
                    if (ptr % 32 != 0)
                        ptr += 32 - ptr % 32;
                }
                return ptr;
            }
            public long CountData(string file, out uint size)
            {
                long oldoff = dataoffset;
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                dataoffset += fs.Length;
                if (dataoffset % 32 != 0)
                    dataoffset += 32 - dataoffset % 32;
                fileSizes.Add(fs.Length);
                size = (uint)fs.Length;
                fs.Close();
                return oldoff;
            }
            public long AddNode(ArcNode node)
            {
                long ptr = nodes.Count;
                nodes.Add(node);
                return ptr;
            }
            public long AddFile(ArcFileEntry file)
            {
                long ptr = files.Count;
                if (file.id != 0xFFFF)
                    file.id = (ushort)cid;
                cid++;
                files.Add(file);
                return ptr;
            }
            public long AddString(string name)
            {
                long ptr = 0;
                foreach (string str in strings)
                    ptr += str.Length + 1;
                strings.Add(name);
                return ptr;
            }
            public long NodeStart()
            {
                return 64;
            }
            public long FileStart()
            {
                long ptr =  NodeStart() + nodes.Count * ArcNode.ARCNODE_SIZE;
                if (ptr % 32 != 0)
                    ptr += 32 - ptr % 32;
                return ptr;
            }
            public long StringStart()
            {
                long ptr = FileStart() + files.Count * ArcFileEntry.ARCENTRY_SIZE;
                if (ptr % 32 != 0)
                    ptr += 32 - ptr % 32;
                return ptr;
            }
            public long StringSize()
            {
                long size = 0;
                foreach (string str in strings)
                    size += str.Length + 1;
                if (size % 32 != 0)
                    size += 32 - size % 32;
                return size;
            }
            public long DataStart()
            {
                long ptr = StringStart();
                foreach (string str in strings)
                    ptr += str.Length + 1;
                if (ptr % 32 != 0)
                    ptr += 32 - ptr % 32;
                return ptr;
            }
            public void AlignHead()
            {
                head.dataStart = (uint)DataStart() - 32;
                head.entryOffset = (uint)FileStart() - 32;
                head.stringOffset = (uint)StringStart() - 32;
                head.size = (uint)Size();
                head.nodeCount = (uint)nodes.Count;
                head.unknown = 32;
                head.unknown2 = new UInt32[4];
                head.unknown2[0] = (uint)dataoffset;
                head.unknown2[1] = (uint)dataoffset;
                head.unknown2[2] = 0;
                head.unknown2[3] = 0;
                head.unknown3 = new UInt32[2];
                head.unknown3[0] = 32;
                head.unknown3[1] = (uint)files.Count;
                head.unknown4 = (uint)StringSize();
                head.unknown5 = new UInt32[2];
                head.unknown5[0] = (uint)((ushort)files.Count << 16 + 0x0100);
                head.unknown5[1] = 0;
            }
        }
        public struct ArcHeader
        {
            public const int ARCHEADER_SIZE = 60;

            public UInt32 size;
            public UInt32 unknown;
            public UInt32 dataStart;
            public UInt32[] unknown2;
            public UInt32 nodeCount;
            public UInt32[] unknown3;
            public UInt32 entryOffset;
            public UInt32 unknown4;
            public UInt32 stringOffset;
            public UInt32[] unknown5;

            public ArcHeader(Stream arcstr)
            {
                size = Data.ReadUInt32(arcstr);
                unknown = Data.ReadUInt32(arcstr);
                dataStart = Data.ReadUInt32(arcstr);
                unknown2 = new UInt32[4];
                unknown2[0] = Data.ReadUInt32(arcstr);
                unknown2[1] = Data.ReadUInt32(arcstr);
                unknown2[2] = Data.ReadUInt32(arcstr);
                unknown2[3] = Data.ReadUInt32(arcstr);
                nodeCount = Data.ReadUInt32(arcstr);
                unknown3 = new UInt32[2];
                unknown3[0] = Data.ReadUInt32(arcstr);
                unknown3[1] = Data.ReadUInt32(arcstr);
                entryOffset = Data.ReadUInt32(arcstr);
                unknown4 = Data.ReadUInt32(arcstr);
                stringOffset = Data.ReadUInt32(arcstr);
                unknown5 = new UInt32[2];
                unknown5[0] = Data.ReadUInt32(arcstr);
                unknown5[1] = Data.ReadUInt32(arcstr);
            }
            public void Write(Stream arcstr){
                Data.WriteUInt32(arcstr, size);
                Data.WriteUInt32(arcstr, unknown);
                Data.WriteUInt32(arcstr, dataStart);
                Data.WriteUInt32(arcstr, unknown2[0]);
                Data.WriteUInt32(arcstr, unknown2[1]);
                Data.WriteUInt32(arcstr, unknown2[2]);
                Data.WriteUInt32(arcstr, unknown2[3]);
                Data.WriteUInt32(arcstr, nodeCount);
                Data.WriteUInt32(arcstr, unknown3[0]);
                Data.WriteUInt32(arcstr, unknown3[1]);
                Data.WriteUInt32(arcstr, entryOffset);
                Data.WriteUInt32(arcstr, unknown4);
                Data.WriteUInt32(arcstr, stringOffset);
                Data.WriteUInt32(arcstr, unknown5[0]);
                Data.WriteUInt32(arcstr, unknown5[1]);
            }
        }
        public struct ArcNode
        {
            public const int ARCNODE_SIZE = 16;

            public UInt32 type;
            public UInt32 filenameOffset;
            public UInt16 unknown;
            public UInt16 entryCount;
            public UInt32 entryOffset;

            public string WritePath;

            public ArcNode(Stream arcstr, int i)
            {
                arcstr.Seek(64 + i * 16, SeekOrigin.Begin);
                type = Data.ReadUInt32(arcstr);
                filenameOffset = Data.ReadUInt32(arcstr);
                unknown = Data.ReadUInt16(arcstr);
                entryCount = Data.ReadUInt16(arcstr);
                entryOffset = Data.ReadUInt32(arcstr);

                WritePath = null;
            }
            public void Write(Stream arcstr)
            {
                Data.WriteUInt32(arcstr, type);
                Data.WriteUInt32(arcstr, filenameOffset);
                Data.WriteUInt16(arcstr, unknown);
                Data.WriteUInt16(arcstr, entryCount);
                Data.WriteUInt32(arcstr, entryOffset);
            }
        }
        public struct ArcFileEntry
        {
            public const int ARCENTRY_SIZE = 20;

            public UInt16 id; //file id. If this is 0xFFFF, then this entry is a subdirectory link
            public UInt16 unknown;
            public UInt16 unknown2;
            public UInt16 filenameOffset; //file/subdir name, offset into string table
            public UInt32 dataOffset; //offset to file data (for subdirs: index of Node representing the subdir)
            public UInt32 dataSize; //size of data
            public UInt32 zero; //seems to be always '0'

            public string WritePath;
            public ArcFileEntry(Stream arcstr, ArcHeader head, int i)
            {
                arcstr.Seek(head.entryOffset + i * 20 + 0x20, SeekOrigin.Begin);
                id = Data.ReadUInt16(arcstr);
                unknown = Data.ReadUInt16(arcstr);
                unknown2 = Data.ReadUInt16(arcstr);
                filenameOffset = Data.ReadUInt16(arcstr);
                dataOffset = Data.ReadUInt32(arcstr);
                dataSize = Data.ReadUInt32(arcstr);
                zero = Data.ReadUInt32(arcstr);

                WritePath = null;
            }
            public void Write(Stream arcstr)
            {
                Data.WriteUInt16(arcstr, id);
                Data.WriteUInt16(arcstr, unknown);
                Data.WriteUInt16(arcstr, unknown2);
                Data.WriteUInt16(arcstr, filenameOffset);
                Data.WriteUInt32(arcstr, dataOffset);
                Data.WriteUInt32(arcstr, dataSize);
                Data.WriteUInt32(arcstr, 0);
            }
        };
        private static void DumpArcNode(Stream arcstr, ArcHeader head, ArcNode node, string destroot, string nameoverride = null)
        {
            string nodeName = nameoverride;
            arcstr.Seek(node.filenameOffset + head.stringOffset + 0x20, SeekOrigin.Begin);
            if (nodeName == null)
                nodeName = Data.ReadString(arcstr);

            destroot += "\\" + nodeName;
            Directory.CreateDirectory(destroot);

            for(int i = 0; i < node.entryCount; ++i)
            {
                ArcFileEntry curr = new ArcFileEntry(arcstr, head, (int)(node.entryOffset + i));

                if(curr.id == 0xFFFF) //subdirectory
                {
                    if(curr.filenameOffset != 0 && curr.filenameOffset != 2){ //don't go to "." and ".."
                        ArcNode dirNode = new ArcNode(arcstr, (int)curr.dataOffset);                        //Some arc packing programs have a glitch
                        arcstr.Seek(curr.filenameOffset + head.stringOffset + 0x20, SeekOrigin.Begin);      //People use them so just work around it
                        DumpArcNode(arcstr, head, dirNode, destroot, Data.ReadString(arcstr));
                    }
                }
                else //file
                {
                    arcstr.Seek(curr.filenameOffset + head.stringOffset + 0x20, SeekOrigin.Begin);
                    string currName = Data.ReadString(arcstr);

                    FileStream dest = new FileStream(destroot + "\\" + currName, FileMode.Create);

                    int read = 0;
                    byte[] buff = new byte[1024];
                    arcstr.Seek(curr.dataOffset + head.dataStart + 0x20, SeekOrigin.Begin);
                    while(read < curr.dataSize)
                    {
                        int r = arcstr.Read(buff, 0, (int)Math.Min(1024, curr.dataSize - read));
                        dest.Write(buff, 0, r);
                        read += r;
                    }
                    dest.Close();
                }
            }
        }
        public static void ExtractArc(string arc, string dest)
        {
            FileStream arcstr = new FileStream(arc, FileMode.Open, FileAccess.Read);

            byte[] magic = Data.Read(arcstr, 4);

            if (Data.CompareBytes(magic, RARC) != 0)
            {
                Console.WriteLine(arc + " is not a valid arc file.");
                return;
            }

            ArcHeader head = new ArcHeader(arcstr);
            ArcNode root = new ArcNode(arcstr, 0);
            DumpArcNode(arcstr, head, root, dest);
            arcstr.Close();
        }
        private static ArcInfo BuildArcInfo(string dir, string arcdir, long parent, Dictionary<string, ushort> dirStrings, ArcInfo ai)
        {
            DirectoryInfo di = new DirectoryInfo(dir);

            string curdir;
            if (arcdir != "")
                curdir = arcdir + "/" + di.Name;
            else
                curdir = di.Name;

            ArcNode dirNode = new ArcNode();
            if (!dirStrings.ContainsKey(dir))
                dirNode.filenameOffset = (uint)ai.AddString(di.Name);
            else
                dirNode.filenameOffset = dirStrings[dir];
            dirNode.unknown = GCN.CreateHash(di.Name);
            dirNode.entryOffset = (uint)(ai.files.Count);
            dirNode.entryCount = 0;
            dirNode.WritePath = curdir;
            if (ai.nodes.Count == 0)
                dirNode.type = BitConverter.ToUInt32(new byte[4] { (byte)'T', (byte)'O', (byte)'O', (byte)'R' }, 0);
            else
            {
                byte[] type = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    if (i < di.Name.Length)
                        type[3 - i] = (byte)(di.Name.ToUpper())[i];
                    else
                        type[3 - i] = 0x20;
                }
                dirNode.type = BitConverter.ToUInt32(type, 0);
            }

            long coffset = ai.AddNode(dirNode);

            string[] files = Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).ToArray();
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                ArcFileEntry fentry = new ArcFileEntry();
                fentry.id = 0;
                if (dirStrings.ContainsKey(file))
                    fentry.filenameOffset = dirStrings[file];
                else
                    fentry.filenameOffset = (ushort)ai.AddString(fi.Name);
                fentry.dataOffset = (uint)ai.CountData(file, out fentry.dataSize);
                fentry.WritePath = curdir + "/" + fi.Name;
                fentry.unknown = GCN.CreateHash(fi.Name);
                fentry.unknown2 = 0x1100;
                ai.AddFile(fentry);
                dirNode.entryCount++;
            }

            long fileoff = coffset;
            string[] dirs = Directory.EnumerateDirectories(dir, "*", SearchOption.TopDirectoryOnly).ToArray();

            foreach (string idir in dirs)
            {
                DirectoryInfo idi = new DirectoryInfo(idir);
                ArcFileEntry dentry = new ArcFileEntry();
                dentry.id = 0xFFFF;
                dentry.filenameOffset = (ushort)ai.AddString(idi.Name);
                dirStrings.Add(idir, dentry.filenameOffset);
                dentry.dataOffset = (uint)(++fileoff);
                fileoff += Directory.EnumerateDirectories(idir, "*", SearchOption.AllDirectories).Count();
                dentry.dataSize = 16;
                dentry.WritePath = curdir;
                dentry.unknown = GCN.CreateHash(idi.Name);
                dentry.unknown2 = 0x0200;
                ai.AddFile(dentry);
                dirNode.entryCount++;
            }

            ArcFileEntry cEntry = new ArcFileEntry();
            cEntry.id = 0xFFFF;
            cEntry.filenameOffset = 0;
            cEntry.WritePath = curdir;
            cEntry.dataOffset = (uint)coffset;
            cEntry.dataSize = 16;
            cEntry.unknown = 0x2E;
            cEntry.unknown2 = 0x0200;
            ai.AddFile(cEntry);
            ArcFileEntry pEntry = new ArcFileEntry();
            pEntry.filenameOffset = 2;
            pEntry.id = 0xFFFF;
            pEntry.WritePath = arcdir;
            pEntry.dataOffset = (uint)parent;
            pEntry.dataSize = 16;
            pEntry.unknown = 0xB8;
            pEntry.unknown2 = 0x0200;
            ai.AddFile(pEntry);
            dirNode.entryCount += 2;

            ai.nodes[ai.nodes.Count - 1] = dirNode;

            foreach (string idir in dirs)
                ai = BuildArcInfo(idir, curdir, coffset, dirStrings, ai);
            return ai;
        }
        private static void WriteArc(Stream arcstr, string dir, ArcInfo ai)
        {
            arcstr.Write(RARC, 0, 4);
            ai.head.Write(arcstr);
            foreach (ArcNode an in ai.nodes)
                an.Write(arcstr);

            Data.WritePadding(arcstr, (int)(ai.FileStart() - arcstr.Length));
            foreach (ArcFileEntry afe in ai.files)
                afe.Write(arcstr);

            Data.WritePadding(arcstr, (int)(ai.StringStart() - arcstr.Length));
            foreach (string str in ai.strings)
                Data.WriteString(arcstr, str);

            dir += "/../";

            foreach (ArcFileEntry afe in ai.files)
            {
                if (afe.id == 0xFFFF)
                    continue;

                if (arcstr.Position % 32 != 0)
                    Data.WritePadding(arcstr, (int)(32 - arcstr.Position % 32));

                FileStream fs = new FileStream(dir + "/" + afe.WritePath, FileMode.Open, FileAccess.Read);

                int read = 0;
                byte[] buff = new byte[1024];
                while (read < afe.dataSize)
                {
                    int r = fs.Read(buff, 0, (int)Math.Min(1024, afe.dataSize - read));
                    arcstr.Write(buff, 0, r);
                    read += r;
                }
                fs.Close();
            }
            arcstr.Close();
        }
        public static void CreateArc(string dir, string dest)
        {
            ArcHeader head = new ArcHeader();

            //Build dir info
            ArcInfo ai = new ArcInfo(head);
            ai.strings.Add(".");
            ai.strings.Add("..");
            ai = BuildArcInfo(dir, "", 0xFFFF, new Dictionary<string,ushort>(), ai);
            ai.AlignHead();

            FileStream arcstr = new FileStream(dest, FileMode.Create);
            WriteArc(arcstr, dir, ai);
        }
        #endregion

        #region ISO
        public static bool ExtractISO(string source, string target)
        {
            if (!File.Exists(source))
                return false;

            if (!Directory.Exists(target + "/&&systemdata"))
                Directory.CreateDirectory(target + "/&&systemdata");

            long dolptr;
            long tocptr;
            long toclen;

            byte[] hdr = new byte[9280];
            FileStream iso = new FileStream(source, FileMode.Open, FileAccess.Read);

            if (iso.Length < 9280)
                return false;

            iso.Read(hdr, 0, 9280);
            dolptr = (hdr[1056] << 24) | (hdr[1057] << 16) | (hdr[1058] << 8) | hdr[1059];
            tocptr = (hdr[1060] << 24) | (hdr[1061] << 16) | (hdr[1062] << 8) | hdr[1063];
            toclen = (hdr[1064] << 24) | (hdr[1065] << 16) | (hdr[1066] << 8) | hdr[1067];

            if (dolptr > iso.Length || tocptr > iso.Length || tocptr + toclen > iso.Length)
                return false;

            using (FileStream hdrfile = new FileStream(target + "/&&systemdata/ISO.hdr", FileMode.Create, FileAccess.Write))
                hdrfile.Write(hdr, 0, hdr.Length);
            using (FileStream apploader = new FileStream(target + "/&&systemdata/AppLoader.ldr", FileMode.Create, FileAccess.Write))
            {
                byte[] buf = new byte[dolptr - 9280 - 92];
                iso.Read(buf, 0, buf.Length);
                apploader.Write(buf, 0, buf.Length);
            }
            iso.Seek(dolptr, SeekOrigin.Begin);
            using (FileStream dol = new FileStream(target + "/&&systemdata/Start.dol", FileMode.Create, FileAccess.Write))
            {
                byte[] buf = new byte[tocptr - dolptr];
                iso.Read(buf, 0, buf.Length);
                dol.Write(buf, 0, buf.Length);
            }

            iso.Seek(tocptr, SeekOrigin.Begin);
            long filestart = iso.Position + toclen;
            TOCFile tocfile = new TOCFile(iso, (int)toclen);

            List<TOCFile.Entry> entries = tocfile.EnumerateEntries();

            
            if (filestart % 4096 != 0)
                filestart += 4096 - (filestart % 4096);
            uint firstoffset = tocfile.FirstOffset;
            foreach (TOCFile.Entry entry in entries)
            {
                if (entry.children.Count > 0 && !Directory.Exists(target + "/" + entry.FullPath))
                    Directory.CreateDirectory(target + "/" + entry.FullPath);
            }
            bool first = false;
            foreach (TOCFile.Entry entry in entries)
            {
                if (entry.children.Count > 0)
                    continue;
                using (FileStream fs = new FileStream(target + "/" + entry.FullPath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buf = new byte[entry.length];
                    iso.Seek(entry.offset, SeekOrigin.Begin);
                    iso.Read(buf, 0, buf.Length);
                    fs.Write(buf, 0, buf.Length);
                    first = true;
                }
            }
            iso.Close();

            tocfile.Save(target + "/&&systemdata/Game.toc");

            return true;
        }

        public static bool RebuildISO(string source, string target)
        {
            if (!Directory.Exists(source))
                return false;

            if (!Directory.Exists(source + "/&&systemdata"))
                return false;

            if (!File.Exists(source + "/&&systemdata/Game.toc") || !File.Exists(source + "/&&systemdata/AppLoader.ldr") || !File.Exists(source + "/&&systemdata/Start.dol") || !File.Exists(source + "/&&systemdata/ISO.hdr"))
                return false;

            long dolptr;
            long tocptr;
            long toclen;

            byte[] hdr = new byte[9280];
            FileStream iso = new FileStream(target, FileMode.Create, FileAccess.Write);

            {
                FileInfo tocinfo = new FileInfo(source + "/&&systemdata/Game.toc");
                FileInfo alinfo = new FileInfo(source + "/&&systemdata/AppLoader.ldr");
                FileInfo dolinfo = new FileInfo(source + "/&&systemdata/Start.dol");

                dolptr = 9280 + alinfo.Length;
                if (dolptr % 4096 != 0)
                    dolptr += 4096 - (dolptr % 4096);

                tocptr = dolinfo.Length + dolptr;
                if (tocptr % 4096 != 0)
                    tocptr += 4096 - (tocptr % 4096);

                toclen = tocinfo.Length;
            }

            using (FileStream hdrfile = new FileStream(source + "/&&systemdata/ISO.hdr", FileMode.Open, FileAccess.Read))
            {
                if (hdrfile.Length != 9280)
                    return false;
                hdrfile.Read(hdr, 0, 9280);

                iso.Write(hdr, 0, 1056);
                Data.WriteUInt32(iso, (uint)dolptr);
                Data.WriteUInt32(iso, (uint)tocptr);
                Data.WriteUInt32(iso, (uint)toclen);
                Data.WriteUInt32(iso, (uint)toclen);
                iso.Write(hdr, 1072, 8208);
            }

            using (FileStream apploader = new FileStream(source + "/&&systemdata/AppLoader.ldr", FileMode.Open, FileAccess.Read))
            {
                byte[] buf = new byte[dolptr - 9280 - 92];
                apploader.Read(buf, 0, buf.Length);
                iso.Write(buf, 0, buf.Length);

            }
            Data.WritePaddingFast(iso, 92);
            using (FileStream dol = new FileStream(source + "/&&systemdata/Start.dol", FileMode.Open, FileAccess.Read))
            {
                byte[] buf = new byte[tocptr - dolptr];
                dol.Read(buf, 0, buf.Length);
                iso.Write(buf, 0, buf.Length);
            }
            using (FileStream toc = new FileStream(source + "/&&systemdata/Game.toc", FileMode.Open, FileAccess.Read))
            {
                byte[] buf = new byte[toclen];
                toc.Read(buf, 0, buf.Length);
                iso.Write(buf, 0, buf.Length);
            }

            long filestart = iso.Position + toclen;

            TOCFile tocfile = new TOCFile(source + "/&&systemdata/Game.toc");

            List<TOCFile.Entry> entries = tocfile.EnumerateFiles(true);

            foreach (TOCFile.Entry entry in entries)
            {
                using (FileStream fs = new FileStream(source + "/" + entry.FullPath, FileMode.Open, FileAccess.Read))
                {
                    if (iso.Position < entry.offset)
                        Data.WritePaddingFast(iso, (int)(entry.offset - iso.Position));

                    byte[] buf = new byte[entry.length];
                    fs.Read(buf, 0, buf.Length);
                    iso.Write(buf, 0, buf.Length);
                }
            }
            iso.Close();

            return true;
        }
        #endregion

        /// <summary>
        /// Creates hash of string (Thanks to blank and NWPlayer123 for this function)
        /// </summary>
        public static ushort CreateHash(string str)
        {
            byte[] dat = Encoding.GetEncoding("shift-jis").GetBytes(str);
            ushort h = 0;
            for (int i = 0; i < dat.Length; i++)
                h = (ushort)(h * 3 + dat[i]);
            return h;
        }
        public static ushort CreateHash(byte[] dat)
        {
            ushort h = 0;
            for (int i = 0; i < dat.Length; i++)
                h = (ushort)(h * 3 + (byte)dat[i]);
            return h;
        }
    }
}
