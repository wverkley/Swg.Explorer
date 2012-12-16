using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public class SkeletonFile : ISWGFile
    {
        public class SkeletonBone
        {
            public string Name { get; internal set; }
            public SkeletonBone Parent { get; internal set; }
            public int ParentIndex { get; internal set; }
            public Quaternion PreRotation { get; internal set; }
            public Quaternion PostRotation { get; internal set; }
            public Vector3 Offset { get; internal set; }

            public override string ToString()
            {
                return string.Format("Name={0}", Name);
            }
        }

        public class Skeleton
        {
            public IEnumerable<SkeletonBone> Bones { get; internal set; }

            internal Skeleton() { }

            public override string ToString()
            {
                return string.Format("Bones.Count={0}", string.Join(", ", Bones.Count()));
            }
        }

        public IEnumerable<Skeleton> Skeletons { get; internal set; }

        internal SkeletonFile() { }

        public override string ToString()
        {
            return string.Format("Skeletons.Count={0}", Skeletons.Count());
        }

    }
}
