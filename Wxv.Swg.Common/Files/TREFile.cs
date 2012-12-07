using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace Wxv.Swg.Common.Files
{
    /// <summary>
    /// Tre package file
    /// </summary>
    public class TREFile : ISWGFile
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
    }
}
