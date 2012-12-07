using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
            var skeletonItems = new List<SkeletonFile.SkeletonFileItem>();

            foreach (var skeletonNode in skeletonNodes)
            {
                var boneNames = skeletonNode.Descendents("NAME").First().Data.ReadStringList();

                var skeletonItem = new SkeletonFile.SkeletonFileItem
                {
                    BoneNames = boneNames
                };
                skeletonItems.Add(skeletonItem);
            }

            return new SkeletonFile
            {
                Items = skeletonItems
            };
        }

        public override SkeletonFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
