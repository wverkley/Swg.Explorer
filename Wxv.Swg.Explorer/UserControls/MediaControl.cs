using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;

using Wxv.Swg.Common;

namespace Wxv.Swg.Explorer.UserControls
{
    public partial class MediaControl : ViewerControl
    {
        public MediaControl()
        {
            InitializeComponent();
        }

        public override void InitViewer(TREInfoFile treInfoFile)
        {
            base.InitViewer(treInfoFile);
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            MediaPlayer.Instance.Play(TREInfoFile.Data, 
                string.Equals (TREInfoFile.FileType.Extension, "mp3", StringComparison.InvariantCultureIgnoreCase)
                ? MediaPlayer.Format.MP3
                : MediaPlayer.Format.Wave);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            MediaPlayer.Instance.Stop();
        }
    }
}
