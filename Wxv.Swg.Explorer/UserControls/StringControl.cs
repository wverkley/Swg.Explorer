using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Wxv.Swg.Common;

namespace Wxv.Swg.Explorer.UserControls
{
    public partial class StringControl : ViewerControl
    {
        public StringControl()
        {
            InitializeComponent();
        }

        public override void InitViewer(TREInfoFile treInfoFile)
        {
            base.InitViewer(treInfoFile);

            StringFile stringsFile;
            using (var stream = new MemoryStream(treInfoFile.Data))
                stringsFile = StringFile.Load(stream);

            listView.BeginUpdate();
            listView.Items.Clear();
            foreach (var stringItem in stringsFile.Items)
            {
                var listViewItem = new ListViewItem(new string[]
                {
                    stringItem.Id.ToString(),
                    stringItem.Name,
                    stringItem.Value
                })
                {
                    Tag = stringItem
                };

                listView.Items.Add(listViewItem);
            }
            listView.EndUpdate();

        }
    }
}
