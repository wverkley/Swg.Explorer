using System;
using System.Text;
using System.IO;

using Wxv.Swg.Common.Files;

namespace Wxv.Swg.Common.Exporters
{
    public class StringExporter
    {
        public StringFile StringFile { get; private set; }

        public StringExporter(StringFile stringFile)
        {
            StringFile = stringFile;
        }

        public StringExporter(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
                StringFile = new StringFileReader().Load(data);
        }

        public void Export(string fileName)
        {
            using (var writer = File.CreateText(fileName))
                foreach (var item in StringFile.Items)
                    writer.WriteLine(CsvUtil.Format(new[] { item.Id.ToString(), item.Name, item.Value }));
        }
    }
}