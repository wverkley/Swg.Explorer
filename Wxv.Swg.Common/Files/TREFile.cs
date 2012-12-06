using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace Wxv.Swg.Common
{
    /// <summary>
    /// Tre package file
    /// </summary>
    public class TREFile 
    {
        public sealed class TreHeader
        {
            public int ResourceCount { get; internal set; }
            public int InfoOffset { get; internal set; }
            public int InfoCompression { get; internal set; }
            public int InfoCompressedSize { get; internal set; }
            public int InfoSize { get { return ResourceCount * 24; } }
            public int NameCompression { get; internal set; }
            public int NameCompressedSize { get; internal set; }
            public int NameSize { get; internal set; }
        }

        public sealed class TreInfo
        {
            public string Name { get; internal set; }
            public int Checksum { get; internal set; }
            public int DataSize { get; internal set; }
            public int DataOffset { get; internal set; }
            public int DataCompression { get; internal set; }
            public int DataCompressedSize { get; internal set; }
            public int NameOffset { get; internal set; }

            public Stream Open(Stream stream)
            {
                stream.Seek(DataOffset, SeekOrigin.Begin);
                byte[] data;
                if (DataCompression == 0)
                {
                    data = stream.ReadBytes(DataSize);
                }
                else
                {
                    var dataBuffer = stream.ReadBytes(DataCompressedSize);
                    var inflater = new Inflater(false);
                    inflater.SetInput(dataBuffer);
                    data = new Byte[DataSize];
                    inflater.Inflate(data);
                }
                return new MemoryStream(data);
            }
        }

        private const string HeaderTagPrefix = "EERT";
        private static readonly string[] HeaderTagValidVersions = new[]{"5000"};

        public TreHeader Header { get; internal set; }
        public IEnumerable<TreInfo> InfoFiles { get; internal set; }

        public bool ContainsInfoFile(string name)
        {
            return InfoFiles.Any(iff => string.Equals(iff.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public TreInfo this [string name]
        {
            get { return InfoFiles.FirstOrDefault(iff => string.Equals(iff.Name, name, StringComparison.InvariantCultureIgnoreCase)); }

        }

        public TreInfo this[int index]
        {
            get { return InfoFiles.ElementAt(index); }
        }

        internal TREFile() { }

        public static TREFile Load(Stream stream)
        {
            var start = stream.Position;
            var tag = stream.ReadString(8);
            if (!tag.StartsWith(HeaderTagPrefix))
                throw new IOException("TRE File does not contain valid TRE data");
            if (!HeaderTagValidVersions.Any(htv => tag.EndsWith(htv)))
                throw new IOException("TRE File does not contain valid TRE data");

            var header = new TreHeader
            {
                ResourceCount = stream.ReadInt32(),
                InfoOffset = stream.ReadInt32(),
                InfoCompression = stream.ReadInt32(),
                InfoCompressedSize = stream.ReadInt32(),
                NameCompression = stream.ReadInt32(),
                NameCompressedSize = stream.ReadInt32(),
                NameSize = stream.ReadInt32(),
            };

            stream.Seek(start + header.InfoOffset, SeekOrigin.Begin);
            byte[] infoData;
            if (header.InfoCompression == 0)
            {
                infoData = stream.ReadBytes(header.InfoSize);
            }
            else
            {
                var infoBuffer = stream.ReadBytes(header.InfoCompressedSize);
                var inflater = new Inflater(false);
                inflater.SetInput(infoBuffer);
                infoData = new Byte[header.InfoSize];
                inflater.Inflate(infoData);
            }
            
            byte[] nameData;
            if (header.NameCompression == 0)
            {
                nameData = stream.ReadBytes(header.NameSize);
            }
            else
            {
                var nameBuffer = stream.ReadBytes(header.NameCompressedSize);
                var inflater = new Inflater(false);
                inflater.SetInput(nameBuffer);
                nameData = new Byte[header.NameSize];
                inflater.Inflate(nameData);
            }

            var names = new List<String>();
            using (var nameStream = new MemoryStream(nameData))
            while (nameStream.Position < nameStream.Length)
            {
                names.Add(nameStream.ReadNullTerminatedString());
            }

            var infoList = new List<TreInfo>();
            using (var infoStream = new MemoryStream(infoData))
            using (var infoReader = new BinaryReader(infoStream))
            foreach (var name in names)
            {
                var info = new TreInfo
                {
                    Name = name,
                    Checksum = infoReader.ReadInt32(),
                    DataSize = infoReader.ReadInt32(),
                    DataOffset = infoReader.ReadInt32(),
                    DataCompression = infoReader.ReadInt32(),
                    DataCompressedSize = infoReader.ReadInt32(),
                    NameOffset = infoReader.ReadInt32()
                };
                infoList.Add(info);
            }

            return new TREFile
            {
                Header = header,
                InfoFiles = infoList
            };
        }

        public static TREFile Load(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
                return Load(stream);
        }
    }
}
