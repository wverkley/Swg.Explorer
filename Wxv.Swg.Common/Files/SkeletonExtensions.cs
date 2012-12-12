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
        public static Matrix TransformMatrix(this SkeletonFile.SkeletonBone skeletonBone)
        {
            //Matrix parentMatrix = skeletonBone.Parent != null
            //    ? skeletonBone.Parent.TransformMatrix()
            //    : Matrix.Identity;
            //Quaternion preRotation = skeletonBone.PreRotation;
            //Quaternion postRotation = skeletonBone.PostRotation;
            //Vector offset = skeletonBone.Offset;

            //var localMatrix = Matrix.Rotation(postRotation) * Matrix.Rotation(preRotation);
            //localMatrix.v_3_0 = offset.X;
            //localMatrix.v_3_1 = offset.Y;
            //localMatrix.v_3_2 = offset.Z;

            //var result = parentMatrix * localMatrix;

            ////var rotation = preRotation * postRotation;
            ////var localMatrix =
            ////    Matrix.Rotation(rotation)
            ////    * Matrix.Translation(offset);

            //var result = Matrix.Identity;

            //var p = skeletonBone;
            //while ((p = p.Parent) != null)
            //{
            //    result = Matrix.Identity
            //        * Matrix.Translation(p.Offset)
            //        * Matrix.Rotation(p.PostRotation)
            //        * Matrix.Rotation(p.PreRotation)
            //        * result;
            //}

            //result = result
            //    * Matrix.Translation(skeletonBone.Offset)
            //    * Matrix.Rotation(skeletonBone.PostRotation)
            //    * Matrix.Rotation(skeletonBone.PreRotation)
            //    ;

            //return result;

            var result = Matrix.Identity;

            var p = skeletonBone;
            while ((p = p.Parent) != null)
            {
                result = Matrix.Identity
                    * Matrix.CreateTranslation(p.Offset)
                    //* Matrix.Rotation(p.PostRotation)
                    //* Matrix.Rotation(p.PreRotation)
                    * result;
            }

            result = result
                * Matrix.CreateTranslation(skeletonBone.Offset)
                //* Matrix.Rotation(skeletonBone.PostRotation)
                //* Matrix.Rotation(skeletonBone.PreRotation)
                ;

            return result;
        }

        public static Vector3 Position(this SkeletonFile.SkeletonBone skeletonBone)
        {
            var transformMatrix = TransformMatrix(skeletonBone);
            return Vector3.Transform(Vector3.Zero, transformMatrix);
        }

        public static void ToString(this SkeletonFile.SkeletonBone skeletonBone, TextWriter writer)
        {
            writer.WriteLine(@"    Name={0}, 
    ParentIndex={1}, 
    PreRotation={2}, 
    PostRotation={3}, 
    Offset={4},
    Position={5}", 
               skeletonBone.Name,
               skeletonBone.ParentIndex,
               skeletonBone.PreRotation,
               skeletonBone.PostRotation,
               skeletonBone.Offset,
               skeletonBone.Position());
        }

        
        public static void ToString(this SkeletonFile.Skeleton skeleton, TextWriter writer)
        {
            writer.WriteLine("  Bones.Count(): {0}", skeleton.Bones.Count());
            for (int i = 0; i < skeleton.Bones.Count(); i++)
            {
                var skeletonBone = skeleton.Bones.ElementAt(i);
                writer.WriteLine("  {0}:", i);
                ToString(skeletonBone, writer);
            }
        }

        public static IEnumerable<Vector3> Positions(this SkeletonFile.Skeleton skeleton)
        {
            return skeleton.Bones.Select(b => b.Position()).ToArray();
        }

        public static string PositionsAsString(this SkeletonFile.Skeleton skeleton, bool flipZ = false)
        {
            return string.Join(" ", skeleton.Positions().Select(p =>
                string.Format("{0:0.######} {1:0.######} {2:0.######}", p.X, p.Y, flipZ ? -p.Z : p.Z)));
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
