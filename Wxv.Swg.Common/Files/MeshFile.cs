using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public class MeshFile : ISWGFile
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
            public Vector3 Position { get; internal set; }
            public Vector3 Normal { get; internal set; }
            public Color Color { get; internal set; }
            public IEnumerable<Vector2> TexCoords { get; internal set; }
            internal MeshVertex() { }

            public override string ToString()
            {
                return string.Format("Position={0}, Normal={1}, Color={2}, TexCoords.Count={3}", Position, Normal, Color, TexCoords.Count());
            }
        }

        public IEnumerable<MeshGeometry> Geometries { get; internal set; }
        internal MeshFile() { }

        public override string ToString()
        {
            return string.Format ("Geomerties.Count={0}", Geometries.Count());
        }
    }
}
