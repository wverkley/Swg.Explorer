using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Wxv.Swg.Common;
using Wxv.Swg.Common.Files;

namespace Wxv.Swg.Explorer
{
    public class TREInfoFile
    {
        public IRepository Repository { get; internal set; }
        public string TreFileName { get; internal set; }
        public string Path { get; internal set; }
        public int DataSize { get; internal set; }

        public TREInfoFile(IRepository repository, string treFileName, string path, int dataSize)
        {
            Repository = repository;
            TreFileName = treFileName;
            Path = path;
            DataSize = dataSize;
        }

        public TREInfoFile(IRepository repository, string treFileName, TREFile.TreInfo treInfo)
        {
            Repository = repository;
            TreFileName = treFileName;
            Path = treInfo.Name;
            DataSize = treInfo.DataSize;
        }

        private FileType fileType = null;
        public FileType FileType
        {
            get
            {
                if (fileType == null)
                    fileType = FileType.FromFileName(Path);
                return fileType;
            }
        }

        public byte[] Data
        {
            get
            {
                return Repository.Load<byte[]>(TreFileName, Path, stream => stream.ReadBytes());
            }
        }

        public IEnumerable<FileTypeExporter> Exporters
        {
            get { return FileType.Exporters; }
        }

        public override string ToString()
        {
            return string.Format("{0} > {1}", TreFileName, Path);
        }
    };
}
