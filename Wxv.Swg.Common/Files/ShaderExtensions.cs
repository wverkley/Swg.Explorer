using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public static class ShaderExtensions
    {
        public static void ToString(this ShaderFile shaderFile, TextWriter writer)
        {
            writer.WriteLine("MaterialColorAmbient: {0}", shaderFile.MaterialColorAmbient.ToFormatString());
            writer.WriteLine("MaterialColorDiffuse: {0}", shaderFile.MaterialColorDiffuse.ToFormatString());
            writer.WriteLine("MaterialColorSpecular: {0}", shaderFile.MaterialColorSpecular.ToFormatString());
            writer.WriteLine("MaterialColorEmisive: {0}", shaderFile.MaterialColorEmisive.ToFormatString());
            writer.WriteLine("MaterialShininess: {0}", shaderFile.MaterialShininess.ToFormatString());
            writer.WriteLine("TextureFileName: {0}", shaderFile.TextureFileName);
        }
    }
}
