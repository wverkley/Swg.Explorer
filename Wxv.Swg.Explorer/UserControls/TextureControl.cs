using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Wxv.Swg.Common;
using Wxv.Xna;

namespace Wxv.Swg.Explorer.UserControls
{
    public partial class TextureControl : ViewerControl
    {
        public TextureControl()
        {
            InitializeComponent();
        }

        public override void InitViewer(TREInfoFile treInfoFile)
        {
            base.InitViewer(treInfoFile);

            pictureBox.Image = DDSHelper.LoadBitmap(treInfoFile.Data);
        }
    }
}
