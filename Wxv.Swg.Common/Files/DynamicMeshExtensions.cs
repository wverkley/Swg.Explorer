using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public static class DynamicMeshExtensions
    {
        public static void ToString(this DynamicMeshFile dynamicMeshFile, TextWriter writer)
        {
            writer.WriteLine(dynamicMeshFile.ToString());
        }
    }
}
