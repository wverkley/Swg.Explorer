using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public static class SkeletonExtensions
    {
        public static Matrix LocalTransformMatrix(this SkeletonFile.SkeletonBone skeletonBone)
        {
            if (skeletonBone == null)
                return Matrix.Identity;

            var matrixPreRotation = Matrix.CreateFromQuaternion(skeletonBone.PreRotation);
            var matrixPostRotation = Matrix.CreateFromQuaternion(skeletonBone.PostRotation);
            var matrixOffset = Matrix.CreateTranslation(skeletonBone.Offset);
            var result = matrixPreRotation * matrixPostRotation * matrixOffset;
            return result;
        }
        
        public static Matrix GlobalTransformMatrix(this SkeletonFile.SkeletonBone skeletonBone)
        {
            if (skeletonBone == null)
                return Matrix.Identity;

            return LocalTransformMatrix(skeletonBone) * GlobalTransformMatrix(skeletonBone.Parent);
        }

        public static Vector3 Position(this SkeletonFile.SkeletonBone skeletonBone, bool flipZ = false)
        {
            var transformMatrix = GlobalTransformMatrix(skeletonBone);
            var result = Vector3.Transform(Vector3.Zero, transformMatrix);
            if (flipZ) result.Z = -result.Z;
            return result;
        }

        public static void ToString(this SkeletonFile.SkeletonBone skeletonBone, TextWriter writer, bool flipZ = false)
        {
            writer.WriteLine(@"    Name={0}, 
    ParentIndex= {1}, 
    PreRotation= {2}, 
    PostRotation={3}, 
    Offset=      {4},
    Position=    {5}", 
               skeletonBone.Name,
               skeletonBone.ParentIndex,
               skeletonBone.PreRotation.ToFormatString(),
               skeletonBone.PostRotation.ToFormatString(),
               skeletonBone.Offset.ToFormatString(),
               skeletonBone.Position(flipZ).ToFormatString());
        }

        
        public static void ToString(this SkeletonFile.Skeleton skeleton, TextWriter writer, bool flipZ = false)
        {
            writer.WriteLine("  Bones.Count(): {0}", skeleton.Bones.Count());
            for (int i = 0; i < skeleton.Bones.Count(); i++)
            {
                var skeletonBone = skeleton.Bones.ElementAt(i);
                writer.WriteLine("  {0}:", i);
                ToString(skeletonBone, writer, flipZ);
            }
        }

        public static IEnumerable<Vector3> Positions(this SkeletonFile.Skeleton skeleton)
        {
            return skeleton.Bones.Select(b => b.Position()).ToArray();
        }

        public static string PositionsAsString(this SkeletonFile.Skeleton skeleton, bool flipZ = false)
        {
            return string.Join(" ", skeleton.Bones.Select(b =>
            {
                var p = b.Position(flipZ);
                return p.ToFormatString();
            }));
        }

        public static IEnumerable<int> Indexes(this SkeletonFile.Skeleton skeleton)
        {
            var result = new List<int>();
            for (int i = 0; i < skeleton.Bones.Count(); i++)
            {
                var bone = skeleton.Bones.ElementAt(i);
                if (bone.ParentIndex >= 0)
                    result.AddRange(new int[] { bone.ParentIndex, i });
            }
            return result;
        }

        public static string IndexesAsString(this SkeletonFile.Skeleton skeleton, bool flipZ = false)
        {
            return string.Join(" ", skeleton.Indexes().Select(i => string.Format("{0}", i)));
        }

        public static void ToString(this SkeletonFile skeletonFile, TextWriter writer)
        {
            writer.WriteLine("Skeletons.Count(): {0}", skeletonFile.Skeletons.Count());
            for (int i = 0; i < skeletonFile.Skeletons.Count(); i++)
            {
                var skeleton = skeletonFile.Skeletons.ElementAt(i);
                writer.WriteLine("{0}:", i);
                ToString(skeleton, writer);
            }
        }

    }
}
