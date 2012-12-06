using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Be.Windows.Forms;

namespace Wxv.Swg.Explorer.UserControls
{
    public partial class BinaryControl : ViewerControl
    {
        public BinaryControl()
        {
            InitializeComponent();
        }

        public override void InitViewer(TREInfoFile treInfoFile)
        {
            base.InitViewer(treInfoFile);
            hexBox.ByteProvider = new DynamicByteProvider(treInfoFile.Data);
        }
    }
}
