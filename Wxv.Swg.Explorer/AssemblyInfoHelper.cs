using System;
using System.Reflection;

namespace Wxv.Swg.Explorer
{
    internal sealed class AssemblyInfoHelper
    {
        private static string _assemblyTitle;
        public static string AssemblyTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(_assemblyTitle)) 
                    return _assemblyTitle;

                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                        return _assemblyTitle = titleAttribute.Title;
                }

                return _assemblyTitle = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        private static string _assemblyVersion;
        public static string AssemblyVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(_assemblyVersion)) 
                    return _assemblyVersion;

                return _assemblyVersion = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private static string _assemblyCopyright;
        public static string AssemblyCopyright
        {
            get
            {
                if (!string.IsNullOrEmpty(_assemblyCopyright)) 
                    return _assemblyCopyright;

                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0) 
                    return _assemblyCopyright = "";

                return _assemblyCopyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
    }
}
