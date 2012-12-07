using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    /// <summary>
    /// Shader file
    /// </summary>
    public class ShaderFile : ISWGFile
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
            writer.WriteLine("MaterialColorD: {0}", MaterialColorD);
            writer.WriteLine("MaterialColorS: {0}", MaterialColorS);
            writer.WriteLine("MaterialColorE: {0}", MaterialColorE);
            writer.WriteLine("MaterialShininess: {0}", MaterialShininess);
            writer.WriteLine("TextureFileName: {0}", TextureFileName);
        }

        public override string ToString()
        {
            return string.Format("ShaderFile: {0}", TextureFileName);
        }

        internal ShaderFile() { }


    }
}
