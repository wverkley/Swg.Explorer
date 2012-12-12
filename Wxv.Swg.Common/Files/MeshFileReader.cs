using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public class MeshFileReader : SWGFileReader<MeshFile>
    {
        public static MeshFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            var rootGeometryNode = iffFile.Root.Descendents("SPS").Children().First();
            if (rootGeometryNode == null) throw new IOException();

            var geometries = new List<MeshFile.MeshGeometry>();

            var geometryCount = rootGeometryNode.Children("CNT").First().Data.ReadInt32();
            for (int geometryIndex = 0; geometryIndex < geometryCount; geometryIndex++)
            {
                var geometryType = (geometryIndex + 1).ToString().PadLeft(4, '0');

                var geometryNode = rootGeometryNode.Children(geometryType).First();

                var shaderFileName = geometryNode.Descendents("NAME").First().Data.ReadString();

                var vertexNode = geometryNode.Descendents("VTXA").First();

                var numVertexes = vertexNode.Descendents("INFO").First().Data.ReadInt32(4);

                var vertexDataNode = vertexNode.Descendents("DATA").First();
                var bytesPerVertex = vertexDataNode.Data.Length / numVertexes;
                if ((vertexDataNode.Data.Length % numVertexes) != 0) throw new IOException("Invalid mesh geometry");

                var vertexes = new List<MeshFile.MeshVertex>();
                using (var stream = new MemoryStream(vertexDataNode.Data))
                    while (stream.Position < stream.Length)
                    {
                        MeshFile.MeshVertex vertex;
                        int vertexPosition = 0;
                        switch (bytesPerVertex)
                        {
                            case 32:
                            case 40:
                            case 48:
                            case 56:
                            case 64:
                            case 72:
                                vertex = new MeshFile.MeshVertex
                                {
                                    Position = stream.ReadVector3Single(),
                                    Normal = stream.ReadVector3Single()
                                };
                                vertexPosition = 24;
                                break;
                            case 36:
                            case 44:
                            case 52:
                            case 60:
                            case 68:
                                vertex = new MeshFile.MeshVertex
                                {
                                    Position = stream.ReadVector3Single(),
                                    Normal = stream.ReadVector3Single(),
                                    Color = stream.ReadColorARGBByte()
                                };
                                vertexPosition = 28;
                                break;
                            default:
                                throw new IOException("Invalid mesh geometry");
                        }

                        var textureCoords = new Vector2[(bytesPerVertex - vertexPosition) / 8];
                        for (int i = 0; i < textureCoords.Length; i++)
                            textureCoords[i] = stream.ReadVector2Single();
                        vertex.TexCoords = textureCoords;

                        vertexes.Add(vertex);
                    }

                int[] indexes;
                var vertexIndexNode = geometryNode.Descendents("INDX").First();
                using (var stream = new MemoryStream(vertexIndexNode.Data))
                {
                    int numIndexes = stream.ReadInt32();
                    int bytesPerIndex = (vertexIndexNode.Data.Length - 4) / numIndexes;
                    if (((vertexIndexNode.Data.Length - 4) % numIndexes) != 0) throw new IOException("Invalid mesh geometry");

                    indexes = new int[numIndexes];
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        switch (bytesPerIndex)
                        {
                            case 2: indexes[i] = stream.ReadInt16(); break;
                            case 4: indexes[i] = stream.ReadInt32(); break;
                            default: throw new IOException("Invalid mesh geometry");
                        }
                    }
                }

                geometries.Add(new MeshFile.MeshGeometry
                {
                    ShaderFileName = shaderFileName,
                    Vertexes = vertexes,
                    Indexes = indexes
                });
            }

            return new MeshFile
            {
                Geometries = geometries
            };
        }

        public override MeshFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
