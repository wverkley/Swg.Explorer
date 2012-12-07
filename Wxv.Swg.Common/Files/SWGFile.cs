using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public interface ISWGFile
    {
    }

    public abstract class SWGFileReader<T> where T : ISWGFile
    {
        public abstract T Load(Stream stream);

        public T Load(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return Load(stream);
        }

        public T Load(string path)
        {
            using (var stream = File.OpenRead(path))
                return Load(stream);
        }
    }
}
