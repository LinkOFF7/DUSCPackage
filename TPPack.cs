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
        internal Header hdr;
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

        public void Repack(string inputDirectory, string outputTP)
        {
            if (outputTP == filename)
                throw new ArgumentException($"You trying to overwrite {filename}. The new archive must have a different name!");
            string[] files = Directory.GetFiles(inputDirectory, "*", SearchOption.AllDirectories);
            Array.Sort(files, StringComparer.Ordinal); //should be enough

            BinaryReader reader = new BinaryReader(File.OpenRead(filename));
            Read(reader);
            reader.Close();

            if (files.Length != Files.Count)
                throw new IndexOutOfRangeException($"Input files count not equal files count in {filename}.\n{inputDirectory}/{filename}: {files.Length}/{Files.Count}");

            BinaryWriter writer = new BinaryWriter(File.Create(outputTP));

            //header
            writer.Write(Encoding.UTF8.GetBytes(hdr.Magic));
            writer.Write(hdr.Version);
            writer.Write(hdr.DataPosition);
            writer.Write(hdr.Count);
            int curOffset = hdr.DataPosition + 7;

            //entities
            for(int i = 0; i < files.Length; i++)
            {
                FileInfo file = Files[i];
                Console.WriteLine("Repacking: {0}", file.Name);
                Console.Title = $"[{i + 1}/{Files.Count}] Processing: importing {inputDirectory} to {outputTP}";
                byte[] data = File.ReadAllBytes(file.Name);
                writer.Write(file.Name);
                writer.Write(curOffset);
                writer.Write(data.Length);
                writer.Write(file.Hash.storedHash);
                WriteByteArrayToOffset(writer, curOffset, data, out curOffset);
            }
            writer.Close();
        }

        private void WriteByteArrayToOffset(BinaryWriter writer, int offset, byte[] array, out int endPos)
        {
            var savepos = writer.BaseStream.Position;
            writer.BaseStream.Position = offset;
            writer.Write(array);
            endPos = (int)writer.BaseStream.Position;
            writer.BaseStream.Position = savepos;
        }
        public void Extract()
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(filename));
            Read(reader);
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
                c++;
            }
        }

        private void Read(BinaryReader reader)
        {
            if (filename == null)
                throw new ArgumentException("Filename is equal null!");;
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
