using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Wxv.Swg.Common;
using Wxv.Swg.Common.Exporters;
using Wxv.Swg.Explorer.UserControls;

namespace Wxv.Swg.Explorer
{
    public partial class MainForm : Form
    {
        private const string Title = "Swg.Explorer";

        private DirectoryTree<TREInfoFile> DirectoryTree { get; set; }

        private IRepository repository;
        private IRepository Repository
        {
            get { return repository; }
            set
            {
                if (repository != null) repository.Dispose();
                repository = value;
            }
        }

        private string DirectoryName { get; set; }
        private bool IsRefreshingData { get; set; }

        private DirectoryTree<TREInfoFile> SelectedDirectoryTree
        {
            get 
            { 
                return treeView.SelectedNode != null
                ? (DirectoryTree<TREInfoFile>)treeView.SelectedNode.Tag
                : null; 
            }
        }

        private TREInfoFile SelectedDirectoryFile
        {
            get
            {
                if (listView.SelectedIndices.Count <= 0)
                    return null ;

                var index = listView.SelectedIndices[0];
                var tif = FilteredItems.ElementAt(index);
                return tif;
            }
        }

        private List<TREInfoFile> filteredItems = new List<TREInfoFile>();
        private List<TREInfoFile> FilteredItems { get { return filteredItems; } }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mediaControl.Visible
                = binaryControl.Visible
                = textureControl.Visible 
                = false;

            mediaControl.Dock
                = binaryControl.Dock
                = textureControl.Dock
                = DockStyle.Fill;

            DirectoryName = Wxv.Swg.Explorer.Properties.Settings.Default.RepositoryDirectoryName;
            if (DirectoryName == null)
            {
            }
            else
            {
                try
                {
                    IRepository repository;
                    DirectoryTree<TREInfoFile> directoryTree;
                    LoadRepository(DirectoryName, out repository, out directoryTree);
                    Repository = repository;
                    DirectoryTree = directoryTree;
                }
                catch
                {
                    DirectoryName = null;
                }
            }

            folderBrowserDialog.SelectedPath =
                DirectoryName == null ? Directory.GetCurrentDirectory() : DirectoryName;

            RefreshData();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Repository = null;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                IRepository repository;
                DirectoryTree<TREInfoFile> directoryTree;
                LoadRepository(folderBrowserDialog.SelectedPath, out repository, out directoryTree);
                Repository = repository;
                DirectoryTree = directoryTree;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                MessageBox.Show("Error opening solution", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DirectoryName = folderBrowserDialog.SelectedPath;

            Wxv.Swg.Explorer.Properties.Settings.Default.RepositoryDirectoryName = DirectoryName;
            Wxv.Swg.Explorer.Properties.Settings.Default.Save();

            ViewForm.CloseAll();
            
            RefreshData();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedDirectoryFile != null)
                ViewForm.Show(Repository, SelectedDirectoryFile);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedDirectoryFile != null)
                SaveAs.Show(SelectedDirectoryFile);
        }

        private void exportToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedDirectoryFile != null)
                ExportTo.Show(SelectedDirectoryFile);
        }

        private void treeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (
                //(e.Button == MouseButtons.Left) ||
                 (e.Button == MouseButtons.Right))
            {
                var n = treeView.GetNodeAt(e.X, e.Y);
                if ((n != null) && (n != treeView.SelectedNode))
                    treeView.SelectedNode = n;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (IsRefreshingData)
                return;

            var item = (DirectoryTree<TREInfoFile>) e.Node.Tag;
            if (item == null)
                return;

            //RefreshMenus();
            RefreshUI();
            RefreshSelectedDirectoryFile();
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshUI();
            RefreshSelectedDirectoryFile();
        }

        private void clearFilterButton_Click(object sender, EventArgs e)
        {
            filterTextBox.Text = string.Empty;
        }

        private void listView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                IsRefreshingData = true;

                var tif = FilteredItems.ElementAt(e.ItemIndex);
                e.Item = new ListViewItem(new string[]
                {
                    Path.GetFileName(tif.TreInfo.Name),
                    tif.FileType.Name,
                    FileUtilities.SizeToString(tif.TreInfo.DataSize),
                    Path.GetFileName(tif.TreFileName),
                })
                {
                    Tag = tif,
                    ImageIndex = 2
                };
            }
            catch 
            {
                e.Item = new ListViewItem(new string[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty
                })
                {
                    ImageIndex = 2
                };
            }
            finally
            {
                IsRefreshingData = false;
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedDirectoryFile != null)
                ViewForm.Show(Repository, SelectedDirectoryFile);
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelectedDirectoryFile();
        }

        private void LoadRepository(string directoryName, out IRepository repository, out DirectoryTree<TREInfoFile> directoryTree)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                repository = null;
                directoryTree = null;

                if (string.IsNullOrEmpty(directoryName))
                    return;

                var localRepository = Wxv.Swg.Common.Repository.Load(directoryName);

                IEnumerable<TREInfoFile> treInfoFiles;
                if (localRepository != null)
                    treInfoFiles = localRepository.Files
                        .SelectMany(tf => tf.TREFile.InfoFiles, (tf, tfif) => new TREInfoFile
                        {
                            Repository = localRepository,
                            TreFileName = tf.FileName,
                            TreInfo = tfif
                        })
                        .OrderBy(tif => tif.Path)
                        .ToList();
                else
                    treInfoFiles = new TREInfoFile[] { };

                repository = localRepository;
                directoryTree = DirectoryTree<TREInfoFile>.Get("", treInfoFiles, tif => tif.Path, "/");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void RefreshTitle()
        {
            this.Text =
                Title + " - [" +
                ((("" + DirectoryName) != "") ? DirectoryName : "Untitled")
                + "]";
        }

        private TreeNode RefreshTreeView(TreeNodeCollection parentNodes, DirectoryTree<TREInfoFile> item, bool isRoot)
        {
            var node = new TreeNode();
            node.Text = isRoot ? "Resources" : item.DirectoryName;
            node.Tag = item;
            parentNodes.Add(node);
            foreach (var childDirectory in item.Directories)
                RefreshTreeView(node.Nodes, childDirectory, false);
            return node;
        }

        private void RefreshTreeView()
        {
            try
            {
                IsRefreshingData = true;

                treeView.BeginUpdate();
                treeView.Nodes.Clear();

                if (DirectoryTree != null)
                {
                    var node = RefreshTreeView(treeView.Nodes, DirectoryTree, true);
                    treeView.SelectedNode = node;
                    node.Expand();
                }

                treeView.EndUpdate();
            }
            finally
            {
                IsRefreshingData = false;
            }
        }

        private void RefreshUI()
        {
            try
            {
                IsRefreshingData = true;

                FilteredItems.Clear();
                string[] filter = new string[]{};
                if (SelectedDirectoryTree != null)
                {
                    filter = filterTextBox.Text
                        .Trim()
                        .Split(' ')
                        .Where(fi => !string.IsNullOrWhiteSpace(fi))
                        .ToArray();
                    if (filter.Length > 0)
                    {
                        var items = SelectedDirectoryTree.Files
                            .Where(f => filter.All(fi => Path.GetFileName(f.Path).IndexOf(fi, StringComparison.InvariantCultureIgnoreCase) >= 0))
                            .ToList();
                        FilteredItems.AddRange(items);
                        resultsLabel.Text = string.Format("{0:N0} / {1:N0} Files", FilteredItems.Count, SelectedDirectoryTree.Files.Count());
                    }
                    else
                    {
                        FilteredItems.AddRange(SelectedDirectoryTree.Files);
                        resultsLabel.Text = string.Format("{0:N0} Files", SelectedDirectoryTree.Files.Count());
                    }
                }
                else
                {
                    resultsLabel.Text = "";
                }

                listView.VirtualListSize = FilteredItems.Count;
                listView.Refresh();
                listView.SelectedIndices.Clear();
            }
            finally
            {
                IsRefreshingData = false;
            }
        }

        private void RefreshSelectedDirectoryFile()
        {
            if (SelectedDirectoryFile == null)
            {
                //splitContainerFile.Panel2Collapsed = true;
                mediaControl.Visible
                    = binaryControl.Visible
                    = textureControl.Visible
                    = false;
                selectedDirectoryFileLabel.Text = "";

                listView.ContextMenuStrip =
                previewPanel.ContextMenuStrip = null;
                return;
            }

            exportToToolStripMenuItem.Visible = SelectedDirectoryFile.Exporters.Count() > 0;

            listView.ContextMenuStrip =
            previewPanel.ContextMenuStrip = contextMenuStrip;

            selectedDirectoryFileLabel.Text = SelectedDirectoryFile.ToString();

            var viewerControlShown = false;

            if ((SelectedDirectoryFile.FileType.FileView & FileView.Media) != 0)
            {
                mediaControl.InitViewer(SelectedDirectoryFile);
                viewerControlShown = mediaControl.Visible = true;
            }
            else
                mediaControl.Visible = false;

            if ((SelectedDirectoryFile.FileType.FileView & FileView.Texture) != 0)
            {
                textureControl.InitViewer(SelectedDirectoryFile);
                viewerControlShown = textureControl.Visible = true;
            }
            else
                textureControl.Visible = false;

            if (!viewerControlShown)
            {
                binaryControl.InitViewer(SelectedDirectoryFile);
                viewerControlShown = binaryControl.Visible = true;
            }
            else
                binaryControl.Visible = false;

            splitContainerFile.Panel2Collapsed = !viewerControlShown;
        }

        private void RefreshData()
        {
            RefreshTitle();
            RefreshTreeView();
            //RefreshMenus();
            RefreshUI();
            RefreshSelectedDirectoryFile();
        }



    }
}
