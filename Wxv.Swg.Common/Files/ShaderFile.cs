using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    /// <summary>
    /// Shader file
    /// </summary>
    public class ShaderFile
    {
        public ColorS MaterialColorA { get; internal set; }
        public ColorS MaterialColorD { get; internal set; }
        public ColorS MaterialColorS { get; internal set; }
        public ColorS MaterialColorE { get; internal set; }
        public Single MaterialShininess { get; internal set; }
        public string TextureFileName { get; internal set; }

        public void ToString(TextWriter writer)
        {
            writer.WriteLine("MaterialColorA: {0}", MaterialColorA);
            writer.WriteLine("MaterialColorD: {0}", MaterialColorA);
            writer.WriteLine("MaterialColorS: {0}", MaterialColorA);
            writer.WriteLine("MaterialColorE: {0}", MaterialColorA);
            writer.WriteLine("MaterialShininess: {0}", MaterialColorA);
            writer.WriteLine("TextureFileName: {0}", TextureFileName);
        }

        public override string ToString()
        {
            return string.Format("ShaderFile: {0}", TextureFileName);
        }

        internal ShaderFile() { }

        public static ShaderFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            var materialNode = iffFile.Root.Descendents("MATS").First();
            var materialTag = materialNode.Children().First().Descendents("TAG").First().Data.ReadString();

            var materialListNode = materialNode.Children().First().Descendents("MATL").First();
            if (materialListNode.Size != 68) throw new IOException("Expected MATL size to be 68");

            ColorS a,d,s,e;
            Single shininess;
            using (var stream = new MemoryStream(materialListNode.Data))
            {
                a = ColorS.Load(stream);
                d = ColorS.Load(stream);
                s = ColorS.Load(stream);
                e = ColorS.Load(stream);
                shininess = stream.ReadSingle();
            }

            var texureNode = iffFile
                .Root
                .Descendents("TXMS")
                .Descendents("TXM")
                .FirstOrDefault(node => string.Equals(node.Descendents("DATA").First().Data.ReadString(), "NIAM", StringComparison.InvariantCultureIgnoreCase));
            if (texureNode == null)
                texureNode = iffFile
                    .Root
                    .Descendents("TXMS")
                    .Descendents("TXM")
                    .First();
            var textureFileName = texureNode.Descendents("NAME").First().Data.ReadString(); 

            return new ShaderFile
            {
                MaterialColorA = a,
                MaterialColorD = d,
                MaterialColorS = s,
                MaterialColorE = e,
                MaterialShininess = shininess,
                TextureFileName = textureFileName
            };
        }

        public static ShaderFile Load(Stream stream)
        {
            var iffFile = IFFFile.Load(stream);
            return Load(iffFile);
        }

    }
}
