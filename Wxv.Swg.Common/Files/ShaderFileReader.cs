using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class ShaderFileReader : SWGFileReader<ShaderFile>
    {
        private ShaderFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            var materialNode = iffFile.Root.Descendents("MATS").First();
            var materialTag = materialNode.Children().First().Descendents("TAG").First().Data.ReadString();

            var materialListNode = materialNode.Children().First().Descendents("MATL").First();
            if (materialListNode.Size != 68) throw new IOException("Expected MATL size to be 68");

            ColorF a,d,s,e;
            Single shininess;
            using (var stream = new MemoryStream(materialListNode.Data))
            {
                a = ColorF.Load(stream);
                d = ColorF.Load(stream);
                s = ColorF.Load(stream);
                e = ColorF.Load(stream);
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

        public override ShaderFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
