using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Wxv.Swg.Common
{
    public static class FileUtilities
    {
        public static string GetActualFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName))
                throw new ArgumentException("File does not exist", "fileName");

            fileName = Directory.GetFiles(Path.GetDirectoryName(fileName), Path.GetFileName(fileName)).FirstOrDefault();

            string path = Path.GetDirectoryName(fileName);
            var directoryNameList = new List<string>();
            DirectoryInfo di;
            while ((di = Directory.GetParent(path)) != null)
            {
                directoryNameList.Insert(0, Path.GetFileName(path));
                path = di.FullName;
            }

            foreach (var dn in directoryNameList)
                path += Path.GetFileName(Directory.GetDirectories(path, dn).FirstOrDefault()) + Path.DirectorySeparatorChar;

            return path + Path.GetFileName(fileName);
        }

        public static string GetRelativePath(string fileName, string baseDirectory)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            fileName = Path.GetFullPath(fileName);
            if (string.IsNullOrEmpty(baseDirectory))
                throw new ArgumentNullException("baseDirectory");
            baseDirectory = Path.GetFullPath(baseDirectory);
            if (!baseDirectory.EndsWith("" + Path.DirectorySeparatorChar))
                baseDirectory += Path.DirectorySeparatorChar;

            return
                new Uri(baseDirectory)
                .MakeRelativeUri(new Uri(fileName))
                .OriginalString
                .Replace(@"/", Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
        }

        public static IEnumerable<string> GetRootPaths(IEnumerable<string> directoryNames)
        {
            if ((directoryNames == null)
             || !directoryNames.Any()
             || directoryNames.Any(dn => string.IsNullOrEmpty(dn)))
                throw new ArgumentNullException("fileNames");

            var fullDirectoryNames = directoryNames.Select(dn => Path.GetFullPath(dn));
            if (fullDirectoryNames.Any(fn => Path.GetPathRoot(fn) == null))
                throw new ArgumentException("all file names must contain a root path", "fileNames");

            // IEnumerable<IGrouping<string, string>>
            var fullDirectoryNamesByRoot = fullDirectoryNames
                .GroupBy(fn => Path.GetPathRoot(fn).ToLower())
                .OrderBy(g => g.Key)
                .ToList();

            var result = new List<string>();
            foreach (var group in fullDirectoryNamesByRoot)
            {
                var rootResult = group.Key;
                int minLength = group.Min(dn => dn.Length);
                for (int i = group.Key.Length; i < minLength; i++)
                {
                    if (!group.All(dn => string.Equals(dn.Substring(0, i), group.First().Substring(0, i), StringComparison.InvariantCultureIgnoreCase)))
                        break;

                    char c = group.First()[i];
                    if (c == Path.DirectorySeparatorChar)
                        rootResult = group.First().Substring(0, i + 1);
                }
                result.Add(rootResult);
            }
            return result;
        }

        public static string SizeToString(Object obj)
        {
            try
            {
                if ((obj != null) && ((obj.GetType() == typeof(Int64)) || (obj.GetType() == typeof(Int32))))
                {
                    string[] unitList = new string[] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
                    Int32 unitIndex = 0;
                    Double byteCount = 0;
                    string byteSuffix = string.Empty;
                    if (obj.GetType() == typeof(Int32))
                    {
                        byteCount = (Int32)obj;
                    }
                    else
                    {
                        byteCount = (Int64)obj;
                    }

                    while ((byteCount > 999) && (unitIndex < unitList.Length))
                    {
                        byteCount /= 1024;
                        unitIndex++;
                    }

                    string returnValue = string.Empty;
                    if ((unitIndex == 0) || (byteCount >= 99.95))
                    {
                        returnValue = byteCount.ToString("F0");
                    }
                    else if (byteCount >= 9.995)
                    {
                        returnValue = byteCount.ToString("F1");
                    }
                    else
                    {
                        returnValue = byteCount.ToString("F2");
                    }

                    return returnValue + " " + unitList[unitIndex];
                }
                else
                {
                    return "0 bytes";
                }
            }
            catch
            {
                return "0 bytes";
            }
        }

        public static Boolean IsAlpha(byte[] data)
        {
            foreach (Byte character in data)
            {
                if ((((character <= 0x2f) || (character >= 0x3a)) && ((character <= 0x40) || (character >= 0x5c))) && (character != 0x20))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsAlphaList(byte[] data)
        {
            if ((data == null) || (data.Length == 0))
            {
                return false;
            }

            foreach (Byte character in data)
            {
                if (((character > 0x01) && (character < 0x2D)) || ((character > 0x39) && (character < 0x41)) || ((character > 0x5A) && (character < 0x5C)) || ((character > 0x5C) && (character < 0x5F)) || ((character > 0x5F) && (character < 0x61)) || (character > 0x7A))
                {
                    return false;
                }
            }
            return true;
        }

        public static UInt32 EndianFlip(UInt32 unsignedInteger)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (UInt32)(((unsignedInteger & 0x000000ff) << 24) + ((unsignedInteger & 0x0000ff00) << 8) + ((unsignedInteger & 0x00ff0000) >> 8) + ((unsignedInteger & 0xff000000) >> 24));
            }
            else
            {
                return unsignedInteger;
            }
        }

        public static Int32 EndianFlip(Int32 signedInteger)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (Int32)(((signedInteger & 0x000000ff) << 24) + ((signedInteger & 0x0000ff00) << 8) + ((signedInteger & 0x00ff0000) >> 8) + ((signedInteger & 0xff000000) >> 24));
            }
            else
            {
                return signedInteger;
            }
        }
    }
}
