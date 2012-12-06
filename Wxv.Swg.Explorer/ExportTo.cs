using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Wxv.Xna;
using Wxv.Swg.Common;
using Wxv.Swg.Common.Exporters;

namespace Wxv.Swg.Explorer
{
    public static class ExportTo
    {
        static ExportTo()
        {
            ColladaMeshExporter.DefaultExportDDSToPngFileDelegate =
                (ddsData, pngFileName) =>
                {
                    using (var bitmap = DDSHelper.LoadBitmap(ddsData, withoutTransparency: true))
                        bitmap.Save(pngFileName);
                };

            PngExporter.DefaultExportDDSToPngFileDelegate =
                (ddsData, pngFileName) =>
                {
                    using (var bitmap = DDSHelper.LoadBitmap(ddsData, withoutTransparency: false))
                        bitmap.Save(pngFileName);
                };
        }

        public static void Show(TREInfoFile tif)
        {
            if (tif.Exporters.Count() == 0) 
                return;

            string fileFilter = string.Join("|",
                tif.Exporters
                    .Select(fte => string.Format("{0} Files|*.{1}", fte.Name, fte.Extension))
                    .ToArray());

            using (var saveDialog = new SaveFileDialog
            {
                Title = "Export To",
                Filter = fileFilter,
                InitialDirectory = Wxv.Swg.Explorer.Properties.Settings.Default.SaveAsDirectoryName,
                FileName = Path.GetFileNameWithoutExtension(tif.Path)
            })
            {
                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                var fte = tif.Exporters.FirstOrDefault(fte0 => string.Equals("." + fte0.Extension, Path.GetExtension(saveDialog.FileName)));
                if (fte == null)
                    return;

                fte.Converter(tif.Repository, tif.Data, saveDialog.FileName);

                Wxv.Swg.Explorer.Properties.Settings.Default.RepositoryDirectoryName = Path.GetDirectoryName(saveDialog.FileName);
                Wxv.Swg.Explorer.Properties.Settings.Default.Save();
            }
        }
    }
}
