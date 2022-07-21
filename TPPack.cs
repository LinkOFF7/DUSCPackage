using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DUSCPackage
{
    internal class TPPack
    {
        public string filename;
        internal List<FileInfo> Files;
        internal struct Header
        {
            internal string Magic;
            internal byte Version;
            internal int DataPosition;
            internal int Count;
        }

        internal struct FileInfo
        {
            internal string Name;
            internal int Position;
            internal int Size;
            internal Hash128 Hash;
        }

        public void Extract(bool saveH128)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(filename));
            Read(reader);
            List<string> hashlist = new List<string>();
            int c = 1;
            foreach(var file in Files)
            {
                reader.BaseStream.Position = file.Position;
                byte[] data = reader.ReadBytes(file.Size);
                string path = Path.GetDirectoryName(file.Name);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                Console.WriteLine("Extracting: {0}", file.Name);
                Console.Title = $"[{c}/{Files.Count}] Processing: {Path.GetFileName(filename)}";
                File.WriteAllBytes(file.Name, data);
                hashlist.Add(file.Hash.ToString());
                c++;
            }
            if (saveH128)
                File.WriteAllLines(Path.GetFileNameWithoutExtension(filename) + "_hashlist.txt", hashlist);
        }

        private void Read(BinaryReader reader)
        {
            if (filename == null)
                throw new ArgumentException("Filename is equal null!");
            Header hdr = new Header();
            Files = new List<FileInfo>();
            hdr.Magic = Encoding.UTF8.GetString(reader.ReadBytes(2));
            if(hdr.Magic != "TP")
                throw new FormatException("Invalid TP header!");
            hdr.Version = reader.ReadByte();
            if (hdr.Version != 1)
                throw new FormatException($"Invalid version: {hdr.Version}");
            hdr.DataPosition = reader.ReadInt32();
            hdr.Count = reader.ReadInt32();
            for(int i = 0; i < hdr.Count; i++)
            {
                FileInfo fi = new FileInfo();
                fi.Name = reader.ReadString();
                fi.Position = reader.ReadInt32();
                fi.Size = reader.ReadInt32();
                fi.Hash = new Hash128(reader.ReadBytes(16));
                Files.Add(fi);
            }
        }
    }
}
