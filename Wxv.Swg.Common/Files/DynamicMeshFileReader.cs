using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class DynamicMeshFileReader : SWGFileReader<DynamicMeshFile>
    {
        public static DynamicMeshFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            if (!string.Equals(iffFile.Root.Type, "SKMG", StringComparison.InvariantCultureIgnoreCase))
                throw new IOException();

            int skeletonCount;
            int bonesCount;
            int pointsCount;
            int twdtCount;
            int normalsCount;
            int psdtCount;
            int blendsCount;

            var infoNode = iffFile.Root.Children("0004").Children("INFO").First();
            using (var stream = new MemoryStream(infoNode.Data))
            {
                stream.ReadInt32(); // u1?
                stream.ReadInt32(); // u2?
                skeletonCount = stream.ReadInt32();
                bonesCount = stream.ReadInt32();
                pointsCount = stream.ReadInt32();
                twdtCount = stream.ReadInt32();
                normalsCount = stream.ReadInt32();
                psdtCount = stream.ReadInt32();
                blendsCount = stream.ReadInt32();

                stream.ReadInt16(); // u10?
                stream.ReadInt16(); // u11?
                stream.ReadInt16(); // u12? // Something to do with OITL
                stream.ReadInt16(); // u13?
            }

            string skeletonFileName = iffFile.Root.Children("0004").Children("SKTM").First().Data.ReadString();

            return new DynamicMeshFile
            {
                SkeletonFileName = skeletonFileName
            };
        }

        public override DynamicMeshFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
