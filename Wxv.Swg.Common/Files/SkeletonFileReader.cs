using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna;
using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public class SkeletonFileReader : SWGFileReader<SkeletonFile>
    {
        private SkeletonFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            // We dont care about whether its a (S)LOD or it just contains just one skeleton (SKTM)
            var skeletonNodes = string.Equals(iffFile.Root.Type, "SKTM", StringComparison.InvariantCultureIgnoreCase)
                ? new IFFFile.Node[]{ iffFile.Root } 
                : iffFile.Root.Descendents("SKTM").ToArray();
            
            var skeletons = new List<SkeletonFile.Skeleton>();

            foreach (var skeletonNode in skeletonNodes)
            {
                var bonesCount = skeletonNode.Descendents("INFO").First().Data.ReadInt32();
                
                var boneNames = skeletonNode.Descendents("NAME").First().Data.ReadStringList();
                if (boneNames.Count() != bonesCount)
                    throw new IOException("Invalid # of bone names");

                var boneParents = new List<int>();
                using (var stream = new MemoryStream(skeletonNode.Descendents("PRNT").First().Data))
                    for (int i = 0; i < bonesCount; i++)
                        boneParents.Add(stream.ReadInt32());

                var bonePreRotations = new List<Quaternion>();
                using (var stream = new MemoryStream(skeletonNode.Descendents("RPRE").First().Data))
                    for (int i = 0; i < bonesCount; i++)
                        bonePreRotations.Add(stream.ReadQuaternionSingle());

                var bonePostRotations = new List<Quaternion>();
                using (var stream = new MemoryStream(skeletonNode.Descendents("RPST").First().Data))
                    for (int i = 0; i < bonesCount; i++)
                        bonePostRotations.Add(stream.ReadQuaternionSingle());

                var boneOffsets = new List<Vector3>();
                using (var stream = new MemoryStream(skeletonNode.Descendents("BPTR").First().Data))
                    for (int i = 0; i < bonesCount; i++)
                        boneOffsets.Add(stream.ReadVector3Single());

                var bones = new List<SkeletonFile.SkeletonBone>();
                for (int i = 0; i < bonesCount; i++)
                    bones.Add(new SkeletonFile.SkeletonBone
                    {
                        Name = boneNames[i],
                        ParentIndex = boneParents[i],
                        PreRotation = bonePreRotations[i],
                        PostRotation = bonePostRotations[i],
                        Offset = boneOffsets[i],
                    });

                foreach (var bone in bones)
                {
                    if (bone.ParentIndex >= 0)
                        bone.Parent = bones.ElementAt(bone.ParentIndex);
                }

                skeletons.Add(new SkeletonFile.Skeleton
                {
                    Bones = bones
                });
            }

            return new SkeletonFile
            {
                Skeletons = skeletons
            };
        }

        public override SkeletonFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
