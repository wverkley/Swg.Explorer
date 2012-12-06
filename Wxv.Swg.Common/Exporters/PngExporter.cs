using System;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace Wxv.Swg.Common.Exporters
{
    public class PngExporter
    {
        public delegate void ExportDDSToPngFileDelegate(byte[] ddsData, string pngFileName);

        public static ExportDDSToPngFileDelegate DefaultExportDDSToPngFileDelegate = null;

        public byte[] DdsData { get; private set; }
        public ExportDDSToPngFileDelegate ExportDDSToPngFile { get; private set; }

        public PngExporter(byte[] ddsData, ExportDDSToPngFileDelegate exportDDSToPngFile = null)
        {
            DdsData = ddsData;
            ExportDDSToPngFile = exportDDSToPngFile ?? DefaultExportDDSToPngFileDelegate;
        }

        public void Export(string pngFileName)
        {
            ExportDDSToPngFile(DdsData, pngFileName);
        }
    }
}