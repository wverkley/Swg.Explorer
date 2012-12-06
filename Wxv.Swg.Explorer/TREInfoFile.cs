using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Wxv.Swg.Common;

namespace Wxv.Swg.Explorer
{
    public class TREInfoFile
    {
        public IRepository Repository { get; set; }
        public string TreFileName { get; set; }
        public TREFile.TreInfo TreInfo { get; set; }

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

        public string Path { get { return TreInfo.Name; } }

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
