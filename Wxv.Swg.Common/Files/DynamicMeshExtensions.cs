using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public static class DynamicMeshExtensions
    {
        public static string PositionsAsString(this DynamicMeshFile dynamicMeshFile, bool flipZ = false)
        {
            return string.Join(" ", dynamicMeshFile.Vertexes.Select(v =>
            {
                var p = v.Position;
                if (flipZ) p.Z = -p.Z;
                return p.ToFormatString();
            }));
        }

        public static void ToString(this DynamicMeshFile.DynamicMeshBoneWeight dynamicMeshBoneWeight, TextWriter writer)
        {
            writer.WriteLine("    BoneIndex: {0}", dynamicMeshBoneWeight.BoneIndex);
            writer.WriteLine("    Weight: {0}", dynamicMeshBoneWeight.Weight.ToFormatString());
        }

        public static void ToString(this DynamicMeshFile.DynamicMeshVertex dynamicMeshVertex, TextWriter writer)
        {
            writer.WriteLine("  Position: {0}", dynamicMeshVertex.Position.ToFormatString());
            writer.WriteLine("  BoneWeights.Count: {0}", dynamicMeshVertex.BoneWeights.Count());
            for (int i = 0; i < dynamicMeshVertex.BoneWeights.Count(); i++)
            {
                var boneWeight = dynamicMeshVertex.BoneWeights.ElementAt(i);
                writer.WriteLine("  {0}:", i);
                ToString(boneWeight, writer);
            }
        }

        public static void ToString(this DynamicMeshFile.DynamicMeshBlend dynamicMeshBlend, TextWriter writer)
        {
            writer.WriteLine("  ShaderFileName: {0}", dynamicMeshBlend.ShaderFileName);
            writer.WriteLine("  PositionIndexes.Count(): {0}", dynamicMeshBlend.PositionIndexes.Count());
            writer.WriteLine("  NormalIndexes.Count(): {0}", dynamicMeshBlend.NormalIndexes.Count());
            writer.WriteLine("  TexCoords.Count(): {0}", dynamicMeshBlend.TexCoords.Count());
        }

        public static void ToString(this DynamicMeshFile dynamicMeshFile, TextWriter writer)
        {
            writer.WriteLine("SkeletonFileNames: {0}", string.Join (", ", dynamicMeshFile.SkeletonFileNames));
            writer.WriteLine("BoneNames: {0}", string.Join(", ", dynamicMeshFile.BoneNames));
            writer.WriteLine("Vertexes.Count: {0}", dynamicMeshFile.Vertexes.Count());
            //for (int i = 0; i < dynamicMeshFile.Vertexes.Count(); i++)
            //{
            //    var vertex = dynamicMeshFile.Vertexes.ElementAt(i);
            //    writer.WriteLine("{0}:", i);
            //    ToString(vertex, writer);
            //}
            writer.WriteLine("Normals.Count: {0}", dynamicMeshFile.Normals.Count());
            //for (int i = 0; i < dynamicMeshFile.Normals.Count(); i++)
            //{
            //    var normal = dynamicMeshFile.Normals.ElementAt(i);
            //    writer.WriteLine("{0}: {1}", i, normal.ToFormatString());
            //}
            writer.WriteLine("MeshBlends.Count: {0}", dynamicMeshFile.MeshBlends.Count());
            for (int i = 0; i < dynamicMeshFile.MeshBlends.Count(); i++)
            {
                var meshBlend = dynamicMeshFile.MeshBlends.ElementAt(i);
                writer.WriteLine("{0}:", i);
                ToString(meshBlend, writer);
            }
        }
    }
}
