using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Be.Windows.Forms;

using Wxv.Swg.Common;
using Wxv.Swg.Common.Files;

namespace Wxv.Swg.Explorer.UserControls
{
    public partial class IFFControl : ViewerControl
    {
        private IFFFile IFFFile { get; set; }
        private bool IsRefreshingData { get; set; }

        public IFFControl()
        {
            InitializeComponent();
        }

        public override void InitViewer(TREInfoFile treInfoFile)
        {
            base.InitViewer(treInfoFile);

            IFFFile = new IFFFileReader().Load(treInfoFile.Data);

            dataSizeLabel.Text = "";
            RefreshTreeView();
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

            var item = (IFFFile.Node) e.Node.Tag;
            if (item == null)
            {
                dataSizeLabel.Text = string.Format("Data Size: {0:N0}", 0);
                hexBox.ByteProvider = new DynamicByteProvider(new byte[] { });
                openButton.Visible = false;
                return;
            }

            dataSizeLabel.Text = string.Format("Data Size: {0:N0}", item.Data.Length);
            hexBox.ByteProvider = new DynamicByteProvider(item.Data);

            try
            {
                string fileName = item.Data.ReadString();
                openButton.Visible = TREInfoFile.Repository.Exists(fileName);
            }
            catch
            {
                openButton.Visible = false;
            }
        }

        private TreeNode RefreshTreeView(TreeNodeCollection parentNodes, IFFFile.Node item, bool isRoot)
        {
            var node = new TreeNode();
            node.Text = item.Type;
            node.Tag = item;
            node.SelectedImageIndex 
                = node.ImageIndex 
                = item.NodeType == IFFFile.NodeType.Form ? 0 : 1;
            
            parentNodes.Add(node);
            foreach (var childItem in item.Children)
                RefreshTreeView(node.Nodes, childItem, false);
            return node;
        }

        private void RefreshTreeView()
        {
            try
            {
                IsRefreshingData = true;

                treeView.BeginUpdate();
                treeView.Nodes.Clear();

                var node = RefreshTreeView(treeView.Nodes, IFFFile.Root, true);
                treeView.SelectedNode = node;
                node.ExpandAll();

                treeView.EndUpdate();
            }
            finally
            {
                IsRefreshingData = false;
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            var node = treeView.SelectedNode;
            if (node == null)
                return;
                
            var item = (IFFFile.Node)node.Tag;
            if (item == null)
                return;

            try
            {
                string fileName = item.Data.ReadString();
                IRepositoryTREFile repositoryTREFile;
                TREFile.TreInfo treInfo;
                if (TREInfoFile.Repository.Find(fileName, out repositoryTREFile, out treInfo))
                {
                    var treInfoFile = new TREInfoFile(TREInfoFile.Repository, repositoryTREFile.FileName, treInfo);
                    ViewForm.Show(TREInfoFile.Repository, treInfoFile);
                }
            }
            catch
            {
            }
        }

    }
}
