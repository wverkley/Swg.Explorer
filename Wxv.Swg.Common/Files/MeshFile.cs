using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    /// <summary>
    /// Mesh File
    /// </summary>
    public class MeshFile
    {
        public class MeshGeometry
        {
            public string ShaderFileName { get; internal set; } 
            public IEnumerable<MeshVertex> Vertexes { get; internal set; }
            public IEnumerable<int> Indexes { get; internal set; }
            internal MeshGeometry() { }

            public override string ToString()
            {
                return string.Format("ShaderFileName={0}, Vertexes.Count={1}, Indexes.Count={2}", ShaderFileName, Vertexes.Count(), Indexes.Count());
            }
        }

        public class MeshVertex
        {
            public Vector Position { get; internal set; }
            public Vector Normal { get; internal set; }
            public Color Color { get; internal set; }
            public IEnumerable<TextureCoord> TexCoords { get; internal set; }
            internal MeshVertex() { }

            public override string ToString()
            {
                return string.Format("Position={0}, Normal={1}, Color={2}, TexCoords.Count={3}", Position, Normal, Color, TexCoords.Count());
            }
        }

        public IEnumerable<MeshGeometry> Geometries { get; internal set; }
        internal MeshFile() { }

        public static MeshFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            var rootGeometryNode = iffFile.Root.Descendents("SPS").Children().First();
            if (rootGeometryNode == null) throw new IOException();

            var geometries = new List<MeshGeometry>();
            
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

                var vertexes = new List<MeshVertex>();
                using (var stream = new MemoryStream(vertexDataNode.Data))
                while (stream.Position < stream.Length)
                {
                    MeshVertex vertex;
                    int vertexPosition = 0;
                    switch (bytesPerVertex)
                    {
                        case 32:
                        case 40:
                        case 48:
                        case 56:
                        case 64:
                        case 72:
                            vertex = new MeshVertex
                            {
                                Position = Vector.Load(stream),
                                Normal = Vector.Load(stream)
                            };
                            vertexPosition = 24;
                            break;
                        case 36:
                        case 44:
                        case 52:
                        case 60:
                        case 68:
                            vertex = new MeshVertex
                            {
                                Position = Vector.Load(stream),
                                Normal = Vector.Load(stream),
                                Color = Color.Load(stream)
                            };
                            vertexPosition = 28;
                            break;
                        default:
                            throw new IOException("Invalid mesh geometry");
                    }

                    var textureCoords = new TextureCoord[(bytesPerVertex - vertexPosition) / 8];
                    for (int i = 0; i < textureCoords.Length; i++)
                        textureCoords[i] = TextureCoord.Load(stream);
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

                geometries.Add(new MeshGeometry
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

        public static MeshFile Load(Stream stream)
        {
            var iffFile = IFFFile.Load(stream);
            return Load(iffFile);
        }

        public static MeshFile Load(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return Load(stream);
        }

        public override string ToString()
        {
            return string.Format ("Geomerties.Count={0}", Geometries.Count());
        }
    }
}
