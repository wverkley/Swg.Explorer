using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public static class SkeletonExtensions
    {
        public static void ToString(this SkeletonFile.SkeletonFileItem skeletonFileItem, TextWriter writer)
        {
            writer.WriteLine("  Bone Names:");
            for (int i = 0; i < skeletonFileItem.BoneNames.Count(); i++) 
                writer.WriteLine("    {0}: {1}", i, skeletonFileItem.BoneNames.ElementAt(i));
        }

        public static void ToString(this SkeletonFile skeletonFile, TextWriter writer)
        {
            writer.WriteLine("Items.Count: {0}", skeletonFile.Items.Count());
            for (int i = 0; i < skeletonFile.Items.Count(); i++)
            {
                var skeletonFileItem = skeletonFile.Items.ElementAt(i);
                writer.WriteLine("{0}:", i);
                ToString(skeletonFileItem, writer);
            }
        }

    }
}
