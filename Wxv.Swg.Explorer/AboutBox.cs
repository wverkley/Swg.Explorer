using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Wxv.Swg.Explorer
{
    partial class AboutBox : Form
    {
        private bool isSplash = false;

        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyInfoHelper.AssemblyTitle);
            this.ClientSize = BackgroundImage.Size;
            versionLabel.Text = AssemblyInfoHelper.AssemblyVersion;
        }

        public void EnableSplash()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            okButton.Visible = false;
            isSplash = true;
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            timer.Enabled = isSplash;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            Close();
        }

    }
}
