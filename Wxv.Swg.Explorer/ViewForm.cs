using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Wxv.Swg.Common;
using Wxv.Swg.Explorer.UserControls;

namespace Wxv.Swg.Explorer
{
    public partial class ViewForm : Form
    {
        private const string Title = "Swg.Explorer";

        private IRepository Repository { get; set; }
        private TREInfoFile TREInfoFile { get; set; }
        private byte[] Data { get; set; }

        public ViewForm()
        {
            InitializeComponent();
        }

        private void AddViewerControl<TViewerControl>(string tagPageText) where TViewerControl : ViewerControl
        {
            var viewerControl = (ViewerControl)typeof(TViewerControl).GetConstructor(Type.EmptyTypes).Invoke(null);
            viewerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            viewerControl.Size = new System.Drawing.Size(730, 447);

            viewerControl.InitViewer(TREInfoFile);

            var tabPage = new TabPage()
            {
                Location = new System.Drawing.Point(4, 22),
                Size = new System.Drawing.Size(736, 453),
                Text = tagPageText,
                UseVisualStyleBackColor = true
            };
            tabPage.Controls.Add(viewerControl);

            tabControl.Controls.Add(tabPage);
        }

        private void ViewForm_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();

            Text = string.Format("{0} - [{1}] ({2})", 
                Title,
                TREInfoFile,
                TREInfoFile.FileType.Name);

            if ((TREInfoFile.FileType.FileView & FileView.IFF) != 0)
                AddViewerControl<IFFControl>("Interchange File Format");
            if ((TREInfoFile.FileType.FileView & FileView.Binary) != 0)
                AddViewerControl<BinaryControl>("Binary");
            if ((TREInfoFile.FileType.FileView & FileView.Texture) != 0)
                AddViewerControl<TextureControl>("Texture");
            if ((TREInfoFile.FileType.FileView & FileView.Text) != 0)
                AddViewerControl<TextControl>("Text");
            if ((TREInfoFile.FileType.FileView & FileView.Media) != 0)
                AddViewerControl<MediaControl>("Media");
            if ((TREInfoFile.FileType.FileView & FileView.String) != 0)
                AddViewerControl<StringControl>("Strings");

            this.ResumeLayout(false);
            this.PerformLayout();

            exportToToolStripMenuItem.Visible = TREInfoFile.Exporters.Count() > 0;
        }

        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewForms.ContainsKey(TREInfoFile.Path))
                viewForms.Remove(TREInfoFile.Path);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs.Show(TREInfoFile);
        }

        private void exportToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTo.Show(TREInfoFile);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static Dictionary<string, ViewForm> viewForms = new Dictionary<string, ViewForm>();

        public static void CloseAll()
        {
            var l = viewForms.Values.ToList();
            foreach (var viewForm in l)
                viewForm.Close();
            viewForms.Clear();
        }

        public static bool Show(IRepository repository, TREInfoFile treInfoFile)
        {
            ViewForm viewForm;
            if (viewForms.TryGetValue(treInfoFile.Path, out viewForm))
            {
                viewForm.Activate();
                return true;
            }
            
            var data = repository.Load<byte[]>(
                treInfoFile.TreFileName, 
                treInfoFile.Path, 
                stream => stream.ReadBytes());
            if (data == null)
                return false;

            viewForm = new ViewForm
            {
                Repository = repository,
                TREInfoFile = treInfoFile,
                Data = data
            };
            viewForms.Add(treInfoFile.Path, viewForm);
            viewForm.Show();
            return true;
        }


    }
}
