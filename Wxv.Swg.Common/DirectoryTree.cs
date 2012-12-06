using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public class DirectoryTree<T>
    {
        public string DirectoryName { get; private set; }

        private List<DirectoryTree<T>> _directories = new List<DirectoryTree<T>>();
        public IEnumerable<DirectoryTree<T>> Directories { get { return _directories; } }

        private List<T> _files = new List<T>();
        public IEnumerable<T> Files { get { return _files; } }

        private DirectoryTree(string directoryName)
        {
            DirectoryName = directoryName;
        }

        private static string NormalizePath(string baseDirectoryPath, string path, string seperator)
        {
            if (!path.StartsWith(baseDirectoryPath, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("path not rooted with baseDirectoryPath", "path");
            path = path.Substring(baseDirectoryPath.Length);
            if (path.EndsWith(seperator, StringComparison.InvariantCultureIgnoreCase))
                path = path.Substring(0, path.Length - seperator.Length);
            return path;
        }

        public static DirectoryTree<T> Get(string baseDirectoryPath, IEnumerable<T> items, Func<T, string> getPath, string seperator = null)
        {
            seperator = seperator ?? Path.DirectorySeparatorChar.ToString();
            baseDirectoryPath = NormalizePath("", baseDirectoryPath, seperator);

            string[] directoryPathNames;
            string directoryPathName;
            if (string.IsNullOrEmpty(baseDirectoryPath))
            {
                directoryPathNames = new string[]{ };
                directoryPathName = string.Empty;
            }
            else
            {
                directoryPathNames = baseDirectoryPath.Split(new[] { seperator }, StringSplitOptions.None);
                directoryPathName = directoryPathNames.Length > 0 ? directoryPathNames.Last() : string.Empty;
            }

            var result = new DirectoryTree<T>(directoryPathName);

            foreach (var item in items)
            {
                var itemPath = NormalizePath(baseDirectoryPath, getPath(item), seperator);
                var itemPathNames = itemPath.Split(new[] { seperator }, StringSplitOptions.None);

                var tree = result;
                for (int i = directoryPathNames.Length; i < (itemPathNames.Length - 1); i++)
                {
                    var directoryName = itemPathNames[i];
                    var childDirectory = tree.Directories.FirstOrDefault(dt => string.Equals(dt.DirectoryName, directoryName, StringComparison.InvariantCultureIgnoreCase));
                    if (childDirectory == null)
                    {
                        childDirectory = new DirectoryTree<T>(directoryName);
                        tree._directories.Add(childDirectory);
                        tree = childDirectory;
                    }
                    else
                        tree = childDirectory;
                }

                tree._files.Add(item);
            }

            return result;
        }

        private void ToString(TextWriter writer, int indent)
        {
            foreach (var directory in Directories)
            {
                writer.WriteLine(new string(' ', indent) + directory.DirectoryName);
                directory.ToString(writer, indent + 2);
            }
            foreach (var file in Files)
                writer.WriteLine(new string(' ', indent) + file.ToString());
        }

        public void ToString(TextWriter writer)
        {
            ToString(writer, 0);
        }

        public override string ToString()
        {
            return DirectoryName;
        }
    }
}
