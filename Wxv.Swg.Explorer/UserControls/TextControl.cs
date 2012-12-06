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
    public partial class TextControl : ViewerControl
    {
        public TextControl()
        {
            InitializeComponent();
        }

        public override void InitViewer(TREInfoFile treInfoFile)
        {
            base.InitViewer(treInfoFile);

            string text = Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.ASCII, treInfoFile.Data));

            XDocument xmlDoc = null;
            if (text.TrimStart().StartsWith("<"))
            {
                try
                {
                    xmlDoc = XDocument.Parse(text.Trim());
                }
                catch { }
            }

            if (xmlDoc != null)
            {
                var converter = new XmlToRtfConverter();
                var rtfText = converter.Process(xmlDoc, true);
                richTextBox.Rtf = rtfText;
            }
            else
                richTextBox.Text = text;
        }
    }
}
