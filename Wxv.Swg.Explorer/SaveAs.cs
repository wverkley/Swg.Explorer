using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Wxv.Swg.Common;

namespace Wxv.Swg.Explorer
{
    public static class SaveAs
    {
        public static void Show(TREInfoFile tif)
        {
            using (var saveDialog = new SaveFileDialog
            {
                Title = "Save As",
                Filter = string.Format("{0} Files|*.{1}|All Files|*.*", tif.FileType.Name, tif.FileType.Extension),
                InitialDirectory = Wxv.Swg.Explorer.Properties.Settings.Default.SaveAsDirectoryName,
                FileName = Path.GetFileName(tif.Path)
            })
            {
                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    File.WriteAllBytes(saveDialog.FileName, tif.Data);

                    Wxv.Swg.Explorer.Properties.Settings.Default.RepositoryDirectoryName = Path.GetDirectoryName(saveDialog.FileName);
                    Wxv.Swg.Explorer.Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    MessageBox.Show("Error saving: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
