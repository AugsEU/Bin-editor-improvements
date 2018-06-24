using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataReader;

namespace SMSReader
{
    public class TOCFile
    {
        public class Entry
        {
            public uint index;

            public byte type;
            public string name;
            public uint offset;
            public uint length;

            public Entry parent;
            public List<Entry> children;

            public Entry(byte type, string name, uint offset, uint length, Entry parent = null)
            {
                this.parent = parent;
                this.type = type;
                this.name = name;
                this.offset = offset;
                this.length = length;
                children = new List<Entry>();
            }
            public Entry(Stream stream, Dictionary<int, string> stringdat, long start = 0, Entry parent = null)
            {
                this.parent = parent;

                index = (uint)((stream.Position - start) / 12);
                uint curindex = index;

                type = (byte)stream.ReadByte();
                int stringoffset = (stream.ReadByte() << 16) | (stream.ReadByte() << 8) | stream.ReadByte();
                offset = Data.ReadUInt32(stream);

                length = Data.ReadUInt32(stream);
                children = new List<Entry>();

                name = "";
                if (stringdat.ContainsKey(stringoffset) && index > 0)
                    name = stringdat[stringoffset];

                if (type == 1)
                {
                    while (curindex < length)
                    {
                        children.Add(new Entry(stream, stringdat, start, this));

                        curindex = (uint)((stream.Position - start) / 12);
                    }
                }
            }
            public void Save(Stream stream, Dictionary<Entry, int> stringdat)
            {
                int stringoffset = stringdat[this];

                stream.WriteByte(type);
                stream.WriteByte((byte)((stringoffset & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((stringoffset & 0x0000FF00) >> 8));
                stream.WriteByte((byte)(stringoffset & 0x000000FF));

                Data.WriteUInt32(stream, offset);
                Data.WriteUInt32(stream, length);

                if (type == 1)
                {
                    foreach (Entry e in children)
                        e.Save(stream, stringdat);
                }
            }

            public string FullPath
            {
                get
                {
                    string path = name;
                    Entry current = this;
                    while (current.parent != null)
                    {
                        current = current.parent;
                        path = current.name + "/" + path;
                    }
                    return path;
                }
            }
        }

        Entry RootEntry = null;

        long FileStart;
        long DiscSize;
        Dictionary<uint, Entry> Layout;

        List<Entry> entries;

        public void ConfigureOffset(long offset)
        {
            FileStart = offset;
        }

        public TOCFile(string file)
        {
            Dictionary<int, string> stringdat = new Dictionary<int, string>();

            FileStream fs = new FileStream(file, FileMode.Open);
            fs.Seek(8, SeekOrigin.Begin);
            int size = Data.ReadInt32(fs);
            fs.Seek(size * 12, SeekOrigin.Begin);

            int pos = 0;
            while (fs.Position < fs.Length)
            {
                pos = (int)(fs.Position - (size * 12));
                stringdat.Add(pos, Data.ReadString(fs));
            }
            fs.Seek(0, SeekOrigin.Begin);
            RootEntry = new Entry(fs, stringdat);
            fs.Close();

            ReadLayout(RootEntry);
        }
        public TOCFile(Stream stream, int length)
        {
            Dictionary<int, string> stringdat = new Dictionary<int, string>();

            long start = stream.Position;
            stream.Seek(start + 8, SeekOrigin.Begin);
            int size = Data.ReadInt32(stream);
            stream.Seek(start + size * 12, SeekOrigin.Begin);

            int pos = 0;
            while (stream.Position - start < length)
            {
                pos = (int)((stream.Position - start) - (size * 12));
                stringdat.Add(pos, Data.ReadString(stream));
            }
            stream.Seek(start, SeekOrigin.Begin);
            RootEntry = new Entry(stream, stringdat, start);

            ReadLayout(RootEntry);
        }

        public uint FirstOffset
        {
            get
            {
                foreach (Entry e in entries)
                    if (e.type == 0)
                        return e.offset;
                return 0;
            }
        }

        public Entry FirstFile
        {
            get
            {
                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].type == 0)
                        return entries[i];
                return null;
            }
        }
        public Entry LastFile
        {
            get
            {
                for (int i = entries.Count - 1; i >= 0; i--)
                    if (entries[i].type == 0)
                        return entries[i];
                return null;
            }
        }

        public List<Entry> EnumerateEntries()
        {
            return new List<Entry>(entries);
        }
        public List<Entry> EnumerateFiles(bool sort = false)
        {
            if (sort)
            {
                SortedList<uint, Entry> files = new SortedList<uint, Entry>();
                foreach (Entry e in entries)
                    if (e.type == 0) files.Add(e.offset, e);
                return files.Values.ToList();
            }
            else {
                List<Entry> files = new List<Entry>();
                foreach (Entry e in entries)
                    if (e.type == 0) files.Add(e);
                return files;
            }
        }

        public void Save(string file)
        {
            Reformat();

            Dictionary<Entry, int> stringdat = CreateStringTable(RootEntry);

            FileStream fs = new FileStream(file, FileMode.Create);
            RootEntry.Save(fs, stringdat);
            foreach (KeyValuePair<Entry, int> kvp in stringdat)
                if (kvp.Key.name != "") Data.WriteString(fs, kvp.Key.name);
            fs.Close();
        }

        static int CreateStringTable_offset;
        private Dictionary<Entry, int> CreateStringTable(Entry entry = null, Dictionary<Entry, int> table = null)
        {
            if (table == null)
            {
                table = new Dictionary<Entry, int>();
                table.Add(entry, 0);
                CreateStringTable_offset = 0;
            }

            foreach (Entry e in entry.children)
            {
                if (!table.ContainsKey(e))
                {
                    table.Add(e, CreateStringTable_offset);
                    CreateStringTable_offset += e.name.Length + 1;
                }
                if (e.type == 1)
                   CreateStringTable(e, table);
            }
            return table;
        }

        private void ReadLayout(Entry entry = null, bool rec = false)
        {
            if (!rec)
            {
                entries = new List<Entry>();
                entry = RootEntry;

                DiscSize = 0;
                Layout = new Dictionary<uint, Entry>();
                FileStart = long.MaxValue;
            }
            entries.Add(entry);
            foreach (Entry e in entry.children)
            {
                if (e.type == 0)
                {
                    Layout.Add(e.offset, e);
                    if (DiscSize < e.offset + e.length)
                        DiscSize = e.offset + e.length;
                    if (FileStart > e.offset)
                        FileStart = e.offset;
                    entries.Add(e);
                }
                else
                    ReadLayout(e, true);
            }
        }

        private string GetNameFromPath(string relpath)
        {
            List<string> segpath = new List<string>(relpath.Split('\\', '/'));
            for (int i = segpath.Count - 1; i >= 0; i--)
                if (segpath[i] == "." || segpath[i] == "")
                    segpath.RemoveAt(i);
            return segpath[segpath.Count - 1];
        }

        public bool ContainsFile(string relpath)
        {
            return GetEntry(relpath) != null;
        }

        public Entry GetEntry(string relpath)
        {
            List<string> segpath = new List<string>(relpath.Split('\\', '/'));
            for (int i = segpath.Count - 1; i >= 0; i--)
                if (segpath[i] == "." || segpath[i] == "")
                    segpath.RemoveAt(i);
            return GetEntry(RootEntry, segpath);
        }
        public Entry GetParentEntry(string relpath)
        {
            List<string> segpath = new List<string>(relpath.Split('\\', '/'));
            for (int i = segpath.Count - 1; i >= 0; i--)
                if (segpath[i] == "." || segpath[i] == "")
                    segpath.RemoveAt(i);
            segpath.RemoveAt(segpath.Count - 1);
            if (segpath.Count == 0)
                return RootEntry;
            return GetEntry(RootEntry, segpath);
        }

        private Entry GetEntry(Entry root, List<string> segpath)
        {
            foreach (Entry e in root.children)
            {
                if (e.name == segpath[0])
                {
                    segpath.RemoveAt(0);
                    if (segpath.Count == 0)
                        return e;
                    else
                        return GetEntry(e, segpath);
                }
            }
            return null;
        }

        public void UpdateFileSize(string relpath, string name, uint size)
        {
            Entry file = GetEntry(relpath);
            if (file == null)
            {
                AddFile(relpath, size);
                return;
            }
            if (file.type == 0)
                file.length = size;
            else
                throw new Exception("TOC: Error updating file size (Not a file)");
        }

        public void AddFile(string relpath, uint size)
        {
            Entry parent = GetParentEntry(relpath);
            Entry n = new Entry(0, GetNameFromPath(relpath), 0, size, parent);
            parent.children.Add(n);
            entries.Add(n);
        }

        public void RemoveFile(string relpath)
        {
            string name = GetNameFromPath(relpath);
            Entry parent = GetParentEntry(relpath);
            for (int i = parent.children.Count - 1; i >= 0; i--)
            {
                if (parent.children[i].name == name)
                {
                    entries.Remove(parent.children[i]);
                    parent.children.RemoveAt(i);
                    break;
                }
            }
        }

        public void AddDirectory(string relpath)
        {
            Entry parent = GetParentEntry(relpath);
            parent.children.Add(new Entry(1, GetNameFromPath(relpath), 0, 0));
        }

        public long Reformat(Entry entry = null, long offset = -1, uint index = 0)
        {
            if (offset == -1)
            {
                entry = RootEntry;

                Layout = new Dictionary<uint, Entry>();
                DiscSize = 0;
                offset = FileStart;
            }
            entry.index = index++;
            for (int i = 0; i < entry.children.Count; i++)
            {
                Entry e = entry.children[i];
                if (e.type == 0)
                {
                    e.index = index++;

                    if (e == FirstFile)
                        e = LastFile;
                    else if (e == LastFile)
                        e = FirstFile;

                    e.offset = (uint)offset;
                    offset += e.length;
                    if (offset % 32 != 0)
                        offset += 32 - (offset % 32);

                    Layout.Add(e.offset, e);
                    if (DiscSize < e.offset + e.length)
                        DiscSize = e.offset + e.length;
                }
                else
                {
                    e.offset = entry.index;
                    offset = Reformat(e, offset, index);
                    index = e.length;
                }
            }
            entry.length = index;

            return offset;
        }
    }
}
