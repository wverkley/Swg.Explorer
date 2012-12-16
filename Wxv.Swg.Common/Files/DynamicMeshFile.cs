using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public class DynamicMeshFile : ISWGFile
    {
        public class DynamicMeshBoneWeight
        {
            public int BoneIndex { get; internal set; }
            public Single Weight { get; internal set; }

            public override string ToString()
            {
                return string.Format("BoneIndex={0}, Weight={1}", BoneIndex, Weight.ToFormatString());
            }
        }

        public class DynamicMeshVertex
        {
            public Vector3 Position { get; internal set; }
            public DynamicMeshBoneWeight[] BoneWeights { get; internal set; }

            public override string ToString()
            {
                return string.Format("Position={0}, BoneWeights.Count={1}", Position, BoneWeights.Count());
            }
        }

        public class DynamicMeshTriangle
        {
            public int Group { get; internal set; }
            public int Index0 { get; internal set; }
            public int Index1 { get; internal set; }
            public int Index2 { get; internal set; }

            public override string ToString()
            {
                return string.Format("Group={0}, Index0={1}, Index1={2}, Index2={3}", Group, Index0, Index1, Index2);
            }
        }

        public class DynamicMeshBlend
        {
            public DynamicMeshFile Parent { get; internal set; }

            public string ShaderFileName;
            public IEnumerable<int> PositionIndexes { get; internal set; }
            public IEnumerable<int> NormalIndexes { get; internal set; }
            public IEnumerable<Vector2> TexCoords { get; internal set; }
            public IEnumerable<DynamicMeshTriangle> Triangles { get; internal set; }

            internal DynamicMeshBlend() { }

            public override string ToString()
            {
                return string.Format("ShaderFileName={0}, PositionIndexes.Count()={1}, Triangles.Count()={2}", 
                    ShaderFileName,
                    PositionIndexes.Count(),
                    Triangles.Count());
            }
        }

        public IEnumerable<string> SkeletonFileNames { get; internal set; }
        public IEnumerable<string> BoneNames { get; internal set; }
        public IEnumerable<DynamicMeshVertex> Vertexes { get; internal set; }
        public IEnumerable<Vector3> Normals { get; internal set; }
        public IEnumerable<DynamicMeshBlend> MeshBlends { get; internal set; }

        internal DynamicMeshFile() { }

        public override string ToString()
        {
            return string.Format("SkeletonFileNames={0}", string.Join (", ", SkeletonFileNames));
        }
    }
}
