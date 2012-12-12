using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;

using Wxv.Swg.Common.Files;

namespace Wxv.Swg.Common.Exporters
{
    public sealed class ColladaSkeletonExporter
    {
        private const string TemplateNameSpace = @"http://www.collada.org/2005/11/COLLADASchema";
        private const string TemplateResourceName = @"Wxv.Swg.Common.Exporters.ColladaSkeletonExporter.dae";

        public IRepository Repository { get; private set; }
        public SkeletonFile SkeletonFile { get; private set; }

        private XDocument doc;
        private XNamespace TNS;

        public ColladaSkeletonExporter(IRepository repository, SkeletonFile skeletonFile)
        {
            Repository = repository;
            SkeletonFile = skeletonFile;
            TNS = TemplateNameSpace;
        }

        public ColladaSkeletonExporter(IRepository repository, byte[] data)
            : this (repository, new SkeletonFileReader().Load(data))
        {
        }

        private void ExportGeometries()
        {
            var libraryGeometriesElement = doc
                .Descendants(TNS + "library_geometries")
                .First();
            var geometryElement = libraryGeometriesElement
                .Descendants(TNS + "geometry")
                .First();

            var skeleton = SkeletonFile.Skeletons.First();

            // positions
            var positionElement = geometryElement
                .Descendants(TNS + "source")
                .Where(n => n.Attributes("name").First().Value == "position")
                .First();
            var positionFloatArrayElement = positionElement.Descendants(TNS + "float_array").First();
            positionFloatArrayElement.Attribute("count").Value = (skeleton.Positions().Count() * 3).ToString();
            positionFloatArrayElement.Value = skeleton.PositionsAsString(flipZ: true);
            var positionFloatAccessorElement = positionElement.Descendants(TNS + "accessor").First();
            positionFloatAccessorElement.SetAttributeValue("count", skeleton.Positions().Count().ToString());

            // triangles
            var linesElement = geometryElement
                .Descendants(TNS + "lines")
                .First();
            linesElement.Attribute("count").Value = (skeleton.Indexes().Count() / 2).ToString();
            linesElement.Descendants(TNS + "p").First().Value = skeleton.IndexesAsString();
        }

        public void Export(string daeFileName)
        {
            using (var stream = typeof(ColladaSkeletonExporter).Assembly.GetManifestResourceStream(TemplateResourceName))
                doc = XDocument.Load(stream);

            ExportGeometries();

            doc.Save(daeFileName);
        }
    }
}
