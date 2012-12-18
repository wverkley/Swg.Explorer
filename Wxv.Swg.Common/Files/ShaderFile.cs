using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

namespace Wxv.Swg.Common.Files
{
    public class ShaderFile : ISWGFile
    {
        public Color MaterialColorAmbient { get; internal set; }
        public Color MaterialColorDiffuse { get; internal set; }
        public Color MaterialColorSpecular { get; internal set; }
        public Color MaterialColorEmisive { get; internal set; }
        public Single MaterialShininess { get; internal set; }
        public string TextureFileName { get; internal set; }

        public override string ToString()
        {
            return string.Format("ShaderFile: {0}", TextureFileName);
        }

        internal ShaderFile() { }
    }
}
