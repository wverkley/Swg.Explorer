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
    public class SkeletonFileReader : SWGFileReader<SkeletonFile>
    {
        private SkeletonFile Load(IFFFile iffFile)
        {
            if (iffFile == null) throw new ArgumentNullException("iffFile");

            return new SkeletonFile
            {
            };
        }

        public override SkeletonFile Load(Stream stream)
        {
            var iffFile = new IFFFileReader().Load(stream);
            return Load(iffFile);
        }
    }
}
