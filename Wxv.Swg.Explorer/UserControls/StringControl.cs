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
using Wxv.Swg.Common.Files;

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

            var stringsFile = new StringFileReader()
                .Load(treInfoFile.Data);

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
