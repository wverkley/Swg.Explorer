using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;

using Wxv.Swg.Common.Files;

namespace Wxv.Swg.Common.Exporters
{
    public sealed class ColladaMeshExporter
    {
        public delegate void ExportDDSToPngFileDelegate(byte[] ddsData, string pngFileName);

        public static ExportDDSToPngFileDelegate DefaultExportDDSToPngFileDelegate = null;

        private const string TemplateNameSpace = @"http://www.collada.org/2005/11/COLLADASchema";
        private const string TemplateResourceName = @"Wxv.Swg.Common.Exporters.ColladaExporterTemplate.dae";

        public IRepository Repository { get; private set; }
        public MeshFile MeshFile { get; private set; }
        public ExportDDSToPngFileDelegate ExportDDSToPngFile { get; private set; }

        private XDocument doc;
        private XNamespace TNS;

        public ColladaMeshExporter(IRepository repository, MeshFile meshFile, ExportDDSToPngFileDelegate exportDDSToPngFile = null)
        {
            Repository = repository;
            MeshFile = meshFile;
            ExportDDSToPngFile = exportDDSToPngFile ?? DefaultExportDDSToPngFileDelegate;
            TNS = TemplateNameSpace;
        }

        public ColladaMeshExporter(IRepository repository, byte[] data, ExportDDSToPngFileDelegate exportDDSToPngFile = null)
            : this (repository, new MeshFileReader().Load(data), exportDDSToPngFile)
        {
        }

        private void ExportShaders(string daeFileName)
        {
            var libraryImagesElement = doc
                .Descendants(TNS + "library_images")
                .First();
            var imageElement = libraryImagesElement
                .Descendants(TNS + "image")
                .First();
            imageElement.Remove();

            var libraryMaterialsElement = doc
                .Descendants(TNS + "library_materials")
                .First();
            var materialElement = libraryMaterialsElement
                .Descendants(TNS + "material")
                .First();
            materialElement.Remove();

            var libraryEffectsElement = doc
                .Descendants(TNS + "library_effects")
                .First();
            var effectElement = libraryEffectsElement
                .Descendants(TNS + "effect")
                .First();
            effectElement.Remove();
            
            if (ExportDDSToPngFile == null) return;

            for (int i = 0; i < MeshFile.Geometries.Count(); i++)
            {
                var meshGeometry = MeshFile.Geometries.ElementAt(i);
                var shaderFile = Repository.Load<ShaderFile>(
                    meshGeometry.ShaderFileName,
                    stream => new ShaderFileReader().Load(stream));
                var textureData = Repository.Load<byte[]>(
                    shaderFile.TextureFileName,
                    stream => stream.ReadBytes((int)stream.Length));
                var pngFileName = Path.Combine(
                    Path.GetDirectoryName(daeFileName),  
                    Path.GetFileNameWithoutExtension(shaderFile.TextureFileName) + ".png");
                if (textureData != null)
                    ExportDDSToPngFile(textureData, pngFileName);

                var imageCopy = new XElement(imageElement);
                imageCopy.SetAttributeValue("id", "Image" + i);
                imageCopy.SetAttributeValue("name", "Image" + i);
                imageCopy.Descendants(TNS + "init_from").First().Value = Path.GetFileName(pngFileName);
                libraryImagesElement.Add(imageCopy);

                var materialCopy = new XElement(materialElement);
                materialCopy.SetAttributeValue("id", "Material" + i);
                materialCopy.SetAttributeValue("name", "Material" + i);
                materialCopy.Descendants(TNS + "instance_effect").Attributes("url").First().Value = "#Effect" + i;
                libraryMaterialsElement.Add(materialCopy);

                var effectCopy = new XElement(effectElement);
                effectCopy.SetAttributeValue("id", "Effect" + i);
                var surfaceParamElement = effectCopy.Descendants(TNS + "newparam")
                    .Where(n => n.Attribute("sid") != null && n.Attribute("sid").Value == "Image0-surface")
                    .First();
                surfaceParamElement.Attribute("sid").Value = "Image" + i + "-surface";
                surfaceParamElement.Descendants(TNS + "init_from").First().Value = "Image" + i;
                var samplerParamElement = effectCopy.Descendants(TNS + "newparam")
                    .Where(n => n.Attribute("sid") != null && n.Attribute("sid").Value == "Image0-sampler")
                    .First();
                samplerParamElement.Attribute("sid").Value = "Image" + i + "-sampler";
                samplerParamElement.Descendants(TNS + "source").First().Value = "Image" + i + "-surface";
                effectCopy.Descendants(TNS + "texture").First().Attribute("texture").Value = "Image" + i + "-sampler";

                libraryEffectsElement.Add(effectCopy);
            }
        }

        private void ExportGeometries()
        {
            var libraryGeometriesElement = doc
                .Descendants(TNS + "library_geometries")
                .First();
            var geometryElement = libraryGeometriesElement
                .Descendants(TNS + "geometry")
                .First();
            geometryElement.Remove();

            for (int i = 0; i < MeshFile.Geometries.Count(); i++)
            {
                var meshGeometry = MeshFile.Geometries.ElementAt(i);

                var geometryCopy = new XElement(geometryElement);
                geometryCopy.SetAttributeValue("id", "meshGeometry" + i);
                geometryCopy.SetAttributeValue("name", "meshGeometry" + i);

                // rename id and source attributes
                foreach (var idElement in geometryCopy.Descendants().Where(n => n.Attribute("id") != null && n.Attribute("id").Value.StartsWith("meshGeometry0")))
                {
                    string name = idElement.Attribute("id").Value.Replace("meshGeometry0", "meshGeometry" + i);
                    idElement.Attribute("id").Value = name;
                }
                foreach (var sourceElement in geometryCopy.Descendants().Where(n => n.Attribute("source") != null && n.Attribute("source").Value.StartsWith("#meshGeometry0")))
                {
                    string name = sourceElement.Attribute("source").Value.Replace("#meshGeometry0", "#meshGeometry" + i);
                    sourceElement.Attribute("source").Value = name;
                }
                    
                // positions
                var positionElement = geometryCopy
                    .Descendants(TNS + "source")
                    .Where(n => n.Attributes("name").First().Value == "position")
                    .First();
                var positionFloatArrayElement = positionElement.Descendants(TNS + "float_array").First();
                positionFloatArrayElement.Attribute("count").Value = (meshGeometry.Vertexes.Count() * 3).ToString();
                positionFloatArrayElement.Value = meshGeometry.PositionsAsString(flipZ: true);
                var positionFloatAccessorElement = positionElement.Descendants(TNS + "accessor").First();
                positionFloatAccessorElement.SetAttributeValue("count", meshGeometry.Vertexes.Count().ToString());

                // normals
                var normalElement = geometryCopy
                    .Descendants(TNS + "source")
                    .Where(n => n.Attributes("name").First().Value == "normal")
                    .First();
                var normalFloatArrayElement = normalElement.Descendants(TNS + "float_array").First();
                normalFloatArrayElement.Attribute("count").Value = (meshGeometry.Vertexes.Count() * 3).ToString();
                normalFloatArrayElement.Value = meshGeometry.NormalsAsString(flipZ: true);
                var normalFloatAccessorElement = normalElement.Descendants(TNS + "accessor").First();
                normalFloatAccessorElement.SetAttributeValue("count", meshGeometry.Vertexes.Count().ToString());

                // map (texture co-ordinates)
                var mapElement = geometryCopy
                    .Descendants(TNS + "source")
                    .Where(n => n.Attributes("name").First().Value == "map")
                    .First();
                var mapFloatArrayElement = mapElement.Descendants(TNS + "float_array").First();
                mapFloatArrayElement.Attribute("count").Value = (meshGeometry.Vertexes.Count() * 2).ToString();
                mapFloatArrayElement.Value = meshGeometry.TexCoordsAsString(flipV: true);
                var mapFloatAccessorElement = mapElement.Descendants(TNS + "accessor").First();
                mapFloatAccessorElement.SetAttributeValue("count", meshGeometry.Vertexes.Count().ToString());

                // triangles
                var trianglesElement = geometryCopy
                    .Descendants(TNS + "triangles")
                    .First();
                trianglesElement.Attribute("material").Value = "meshGeometry" + i + "-material";
                trianglesElement.Attribute("count").Value = (meshGeometry.Indexes.Count() / 3).ToString();
                trianglesElement.Descendants(TNS + "p").First().Value = meshGeometry.IndexesAsString(reverse: true);

                libraryGeometriesElement.Add(geometryCopy);
            }
        }

        private void ExportVisualScenes()
        {
            var swgMeshNodeElement = doc
                .Descendants(TNS + "node")
                .Where(n => n.Attribute("id") != null && n.Attribute("id").Value == "SWGMesh")
                .First();
            var instanceGeometryElement = swgMeshNodeElement
                .Descendants(TNS + "instance_geometry")
                .First();
            instanceGeometryElement.Remove();

            for (int i = 0; i < MeshFile.Geometries.Count(); i++)
            {
                var instanceGeometryCopy = new XElement(instanceGeometryElement);
                instanceGeometryCopy.SetAttributeValue("url", "#meshGeometry" + i);
                var instanceMaterial = instanceGeometryCopy.Descendants(TNS + "instance_material").First();
                instanceMaterial.Attribute("symbol").Value = "meshGeometry" + i + "-material";
                instanceMaterial.Attribute("target").Value = "#Material" + i;

                swgMeshNodeElement.Add(instanceGeometryCopy);
            }
        }

        public void Export(string daeFileName)
        {
            using (var stream = typeof(ColladaMeshExporter).Assembly.GetManifestResourceStream(TemplateResourceName))
                doc = XDocument.Load(stream);

            ExportShaders(daeFileName);
            ExportGeometries();
            ExportVisualScenes();

            doc.Save(daeFileName);
        }
    }
}
