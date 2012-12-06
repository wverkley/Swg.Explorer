using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Wxv.Swg.Explorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            var aboutBox = new AboutBox();
            aboutBox.EnableSplash();
            Application.Run(aboutBox);

            Application.Run(new MainForm());
        }
    }
}
