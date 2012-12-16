using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public class DynamicMeshFileReader : SWGFileReader<DynamicMeshFile>
    {
        public static DynamicMeshFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            if (!string.Equals(iffFile.Root.Type, "SKMG", StringComparison.InvariantCultureIgnoreCase))
                throw new IOException();

            int skeletonCount;
            int bonesCount;
            int pointsCount;
            int twdtCount;
            int normalsCount;
            int psdtCount;
            int blendsCount;

            var infoNode = iffFile.Root.Children("0004").Children("INFO").First();
            using (var stream = new MemoryStream(infoNode.Data))
            {
                stream.ReadInt32(); // u1?
                stream.ReadInt32(); // u2?
                skeletonCount = stream.ReadInt32();
                bonesCount = stream.ReadInt32();
                pointsCount = stream.ReadInt32();
                twdtCount = stream.ReadInt32();
                normalsCount = stream.ReadInt32();
                psdtCount = stream.ReadInt32();
                blendsCount = stream.ReadInt32();

                stream.ReadInt16(); // u10?
                stream.ReadInt16(); // u11?
                stream.ReadInt16(); // u12? // Something to do with OITL
                stream.ReadInt16(); // u13?
            }

            var skeletonFileNames = iffFile.Root.Children("0004").Children("SKTM").First().Data.ReadStringList();

            var boneNames = iffFile.Root.Children("0004").Children("XFNM").First().Data.ReadStringList();
            if (boneNames.Count() != bonesCount)
                throw new IOException();

            var vertexList = new List<DynamicMeshFile.DynamicMeshVertex>();
            vertexList.AddRange(Enumerable.Range(0, pointsCount).Select(i => new DynamicMeshFile.DynamicMeshVertex()));

            var positionNode = iffFile.Root.Children("0004").Children("POSN").First();
            using (var stream = new MemoryStream(positionNode.Data))
            {
                for (int i = 0; i < pointsCount; i++)
                    vertexList[i].Position = stream.ReadVector3Single();
            }

            var numVertexWeightsNode = iffFile.Root.Children("0004").Children("TWHD").First();
            using (var stream = new MemoryStream(numVertexWeightsNode.Data))
            {
                for (int i = 0; i < pointsCount; i++)
                {
                    int weightsCount = stream.ReadInt32();
                    vertexList[i].BoneWeights = new DynamicMeshFile.DynamicMeshBoneWeight[weightsCount];
                }
            }

            var vertexWeightsNode = iffFile.Root.Children("0004").Children("TWDT").First();
            using (var stream = new MemoryStream(vertexWeightsNode.Data))
            {
                for (int i = 0; i < pointsCount; i++)
                    for (int j = 0; j < vertexList[i].BoneWeights.Count(); j++)
                    {
                        var boneWeight = new DynamicMeshFile.DynamicMeshBoneWeight
                        {
                            BoneIndex = stream.ReadInt32(),
                            Weight = stream.ReadSingle()
                        };
                        vertexList[i].BoneWeights[j] = boneWeight;
                    }
            }

            var normalsNode = iffFile.Root.Children("0004").Children("NORM").First();
            var normalsList = new List<Vector3>();
            using (var stream = new MemoryStream(normalsNode.Data))
            {
                for (int i = 0; i < normalsCount; i++)
                {
                    var v = stream.ReadVector3Single();
                    normalsList.Add(v);
                }
            }

            var psdtNodes = iffFile.Root.Children("0004").Children("PSDT").ToList();
            if (psdtNodes.Count != psdtCount)
                throw new IOException();
            var meshBlends = new List<DynamicMeshFile.DynamicMeshBlend>();
            foreach (var psdtNode in psdtNodes)
            {
                var shaderFileName = psdtNode.Children("NAME").First().Data.ReadString();

                var positionIndexNode = psdtNode.Children("PIDX").First();
                var positionIndexes = new List<int>();
                int numIndexes;
                using (var stream = new MemoryStream(positionIndexNode.Data))
                {
                    numIndexes = stream.ReadInt32();
                    for (int i = 0; i < numIndexes; i++)
                    {
                        var pi = stream.ReadInt32();
                        positionIndexes.Add(pi);
                    }
                }

                var normalIndexNode = psdtNode.Children("NIDX").First();
                var normalIndexes = new List<int>();
                using (var stream = new MemoryStream(normalIndexNode.Data))
                {
                    for (int i = 0; i < numIndexes; i++)
                    {
                        var ni = stream.ReadInt32();
                        normalIndexes.Add(ni);
                    }
                }

                var texureCoordsNode = psdtNode.Children("TCSF").Children("TCSD").First();
                var textureCoords = new List<Vector2>();
                using (var stream = new MemoryStream(texureCoordsNode.Data))
                {
                    for (int i = 0; i < numIndexes; i++)
                    {
                        var tc = stream.ReadVector2Single();
                        textureCoords.Add(tc);
                    }
                }

                var meshBlend = new DynamicMeshFile.DynamicMeshBlend
                {
                    ShaderFileName = shaderFileName,
                    PositionIndexes = positionIndexes.ToArray(),
                    NormalIndexes = normalIndexes.ToArray(),
                    TexCoords = textureCoords.ToArray()
                };
                meshBlends.Add(meshBlend);
            }

            return new DynamicMeshFile
            {
                SkeletonFileNames = skeletonFileNames,
                BoneNames = boneNames,
                Vertexes = vertexList.ToArray(),
                Normals = normalsList.ToArray(),
                MeshBlends = meshBlends.ToArray()
            };
        }

        public override DynamicMeshFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
