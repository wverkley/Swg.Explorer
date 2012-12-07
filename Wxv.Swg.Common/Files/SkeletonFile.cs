using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class SkeletonFile : ISWGFile
    {
        public class SkeletonFileItem
        {
            public IEnumerable<string> BoneNames { get; internal set; }

            internal SkeletonFileItem() { }

            public override string ToString()
            {
                return string.Format("BoneNames={0}", string.Join(", ", BoneNames));
            }
        }

        public IEnumerable<SkeletonFile> Items { get; internal set; }
        internal SkeletonFile() { }

        public override string ToString()
        {
            return string.Format("Items.Count={0}", Items.Count());
        }

    }
}
