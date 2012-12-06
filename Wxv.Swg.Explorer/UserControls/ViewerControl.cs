using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Wxv.Swg.Common;

namespace Wxv.Swg.Explorer.UserControls
{
    public class ViewerControl : UserControl
    {
        protected TREInfoFile TREInfoFile { get; private set; }

        public virtual void InitViewer(TREInfoFile treInfoFile)
        {
            TREInfoFile = treInfoFile;
        }
    }
}