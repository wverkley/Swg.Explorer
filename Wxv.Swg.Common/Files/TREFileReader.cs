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
    public class TREFileReader : SWGFileReader<TREFile>
    {
        private const string HeaderTagPrefix = "EERT";
        private static readonly string[] HeaderTagValidVersions = new[]{"5000"};

        public override TREFile Load(Stream stream)
        {
            var start = stream.Position;
            var tag = stream.ReadString(8);
            if (!tag.StartsWith(HeaderTagPrefix))
                throw new IOException("TRE File does not contain valid TRE data");
            if (!HeaderTagValidVersions.Any(htv => tag.EndsWith(htv)))
                throw new IOException("TRE File does not contain valid TRE data");

            var header = new TREFile.TreHeader
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
                names.Add(nameStream.ReadString());
            }

            var infoList = new List<TREFile.TreInfo>();
            using (var infoStream = new MemoryStream(infoData))
            using (var infoReader = new BinaryReader(infoStream))
            foreach (var name in names)
            {
                var info = new TREFile.TreInfo
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
    }
}
