using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public static class MeshExtensions
    {
        public static string PositionsAsString(this MeshFile.MeshGeometry meshGeometry, bool flipZ = false)
        {
            return string.Join(" ", meshGeometry.Vertexes.Select(v =>
                string.Format("{0:0.######} {1:0.######} {2:0.######}", v.Position.X, v.Position.Y, flipZ ? -v.Position.Z : v.Position.Z)));
        }

        public static string NormalsAsString(this MeshFile.MeshGeometry meshGeometry, bool flipZ = false)
        {
            return string.Join(" ", meshGeometry.Vertexes.Select(v =>
                string.Format("{0:0.######} {1:0.######} {2:0.######}", v.Normal.X, v.Normal.Y, flipZ ? -v.Normal.Z : v.Normal.Z)));
        }

        public static string TexCoordsAsString(this MeshFile.MeshGeometry meshGeometry, int index = 0, bool flipV = false)
        {
            return string.Join(" ", meshGeometry.Vertexes.Select(v =>
                string.Format("{0:0.######} {1:0.######}", 
                v.TexCoords.ElementAt(index).X,
                flipV ? (1.0 - v.TexCoords.ElementAt(index).Y) : v.TexCoords.ElementAt(index).Y)));
        }

        public static string TriangleIndexesAsString(this MeshFile.MeshGeometry meshGeometry, bool reverse = false)
        {
            if (reverse)
            {
                var list = new List<int>();
                for (int i = 0; i < meshGeometry.Indexes.Count(); i += 3)
                    list.AddRange(new []
                    {
                        meshGeometry.Indexes.ElementAt(i + 2),
                        meshGeometry.Indexes.ElementAt(i + 1),
                        meshGeometry.Indexes.ElementAt(i + 0)
                    });

                return string.Join(" ", list.Select(i => string.Format("{0}", i)));
            }
            else
                return string.Join(" ", meshGeometry.Indexes.Select(i => string.Format("{0}", i)));
        }

        public static void ToString(this MeshFile.MeshGeometry meshGeometry, TextWriter writer)
        {
            writer.WriteLine("  ShaderFileName:", meshGeometry.ShaderFileName);
            writer.WriteLine("  Vertexes:");
            for (int i = 0; i < meshGeometry.Vertexes.Count(); i++)
                writer.WriteLine("    {0}: {1}", i, meshGeometry.Vertexes.ElementAt(i));
            writer.WriteLine("  Indexes:");
            for (int i = 0; i < meshGeometry.Indexes.Count(); i += 3)
                writer.WriteLine("    {0}: {1}, {2}, {3}",
                    i / 3,
                    meshGeometry.Indexes.ElementAt(i + 0),
                    meshGeometry.Indexes.ElementAt(i + 1),
                    meshGeometry.Indexes.ElementAt(i + 2));
        }

        public static void ToString(this MeshFile meshFile, TextWriter writer)
        {
            writer.WriteLine("Geometries.Count: {0}", meshFile.Geometries.Count());
            for (int i = 0; i < meshFile.Geometries.Count(); i++)
            {
                var geometry = meshFile.Geometries.ElementAt(i);
                writer.WriteLine("{0}:", i);
                ToString(geometry, writer);
            }
        }

    }
}
