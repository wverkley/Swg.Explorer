using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class DynamicMeshFile : ISWGFile
    {
        public class DynamicMeshBlend
        {
            public string Name;
            internal DynamicMeshBlend() { }

            public override string ToString()
            {
                return string.Format("Name={0}", Name);
            }
        }

        public string SkeletonFileName { get; internal set; }

        //public IEnumerable<int> PointIndexes { get; internal set; }
        //public IEnumerable<int> NormalIndexes { get; internal set; }

        internal DynamicMeshFile() { }

        public override string ToString()
        {
            return string.Format("SkeletonFileName={0}", SkeletonFileName);
        }
    }
}
