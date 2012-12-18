using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Caching;

using Wxv.Swg.Common.Files;

namespace Wxv.Swg.Common
{
    public delegate T RepositoryLoadDelegate<T>(string treFileName, string fileName, Stream stream);
    public delegate T RepositorySimpleLoadDelegate<T>(Stream stream);

    public interface IRepositoryTREFile : IDisposable
    {
        string FileName { get; }
        TREFile TREFile { get; }
        Stream Stream { get; }
    }

    public interface IRepository : IDisposable
    {
        string BaseDirectory { get; }
        IEnumerable<IRepositoryTREFile> Files { get; }

        bool Find(string treFileName, string fileName, out IRepositoryTREFile repositoryTREFile, out TREFile.TreInfo treInfo);
        bool Find(string fileName, out IRepositoryTREFile repositoryTREFile, out TREFile.TreInfo treInfo);
        bool Exists(string treFileName, string fileName);
        bool Exists(string fileName);
        T Load<T>(string treFileName, string fileName, RepositoryLoadDelegate<T> load) where T : class;
        T Load<T>(string treFileName, string fileName, RepositorySimpleLoadDelegate<T> load) where T : class;
        T Load<T>(string fileName, RepositorySimpleLoadDelegate<T> load) where T : class;
    }

    public class Repository : IRepository
    {
        private class RepositoryTREFile : IRepositoryTREFile
        {
            public string FileName { get; internal set; }
            public TREFile TREFile { get; internal set; }
            public Stream Stream { get; internal set; }
            public void Dispose()
            {
                Stream.Close();
            }
        }

        public string BaseDirectory { get; private set; }
        public IEnumerable<IRepositoryTREFile> Files { get; private set; }
        private MemoryCache _cache = new MemoryCache(typeof(Repository).FullName);

        private Repository() {}

        public void Dispose()
        {
            foreach (var file in Files)
                file.Dispose();
        }

        public static IRepository Load(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
                throw new ArgumentException("Directory does not exist", "baseDirectory");

            var fileNames = Directory.GetFiles(baseDirectory, "*.tre")
                .Where(fn => Path.GetFileName(fn).StartsWith("data_"))
                .OrderBy(fn => fn)
                .ToList();
            var files = fileNames
                .Select(fn => new { FileName = fn, Stream = File.OpenRead(fn) })
                .Select(f => new RepositoryTREFile
                {
                    FileName = Path.Combine (baseDirectory, Path.GetFileName(f.FileName)),
                    Stream = f.Stream,
                    TREFile = new TREFileReader().Load(f.Stream)
                })
                .ToList();

            return new Repository
            {
                BaseDirectory = baseDirectory,
                Files = files
            };
        }

        public bool Find(string treFileName, string fileName, out IRepositoryTREFile repositoryTREFile, out TREFile.TreInfo treInfo)
        {
            fileName = fileName.Replace(@"\", @"/").ToLower();

            repositoryTREFile = null;
            treInfo = null;

            if (!string.IsNullOrEmpty(treFileName))
                repositoryTREFile = Files
                    .Where(f =>
                        string.Equals(f.FileName, treFileName, StringComparison.InvariantCultureIgnoreCase)
                        && f.TREFile.ContainsInfoFile(fileName))
                    .FirstOrDefault();
            if (repositoryTREFile == null)
                repositoryTREFile = Files
                    .Where(f => f.TREFile.ContainsInfoFile(fileName))
                    .FirstOrDefault();

            if (repositoryTREFile == null)
                return false;

            treInfo = repositoryTREFile.TREFile[fileName];
            return true;
        }

        public bool Find(string fileName, out IRepositoryTREFile repositoryTREFile, out TREFile.TreInfo treInfo)
        {
            return Find(null, fileName, out repositoryTREFile, out treInfo);
        }

        public bool Exists(string treFileName, string fileName)
        {
            IRepositoryTREFile repositoryTREFile;
            TREFile.TreInfo treInfo;
            return Find(treFileName, fileName, out repositoryTREFile, out treInfo);
        }

        public bool Exists(string fileName)
        {
            return Exists(null, fileName);
        }
        
        public T Load<T>(string treFileName, string fileName, RepositoryLoadDelegate<T> load) where T : class
        {
            IRepositoryTREFile repositoryTREFile;
            TREFile.TreInfo treInfo;

            if (!Find(treFileName, fileName, out repositoryTREFile, out treInfo))
                return null;

            string fullFileName = typeof(T).FullName + "-" + repositoryTREFile.FileName + "/" + fileName;

            lock (repositoryTREFile)
            {
                var result = _cache[fullFileName] as T;
                if (result != null)
                    return result;

                using (var stream = treInfo.Open(repositoryTREFile.Stream))
                    result = load(repositoryTREFile.FileName, treInfo.Name, stream);
                if (result != null)
                    _cache[fullFileName] = result;
                return result;
            }
        }

        public T Load<T>(string treFileName, string fileName, RepositorySimpleLoadDelegate<T> load) where T : class
        {
            return Load(treFileName, fileName, (tfn, fn, stream) => load(stream));
        }

        public T Load<T>(string fileName, RepositorySimpleLoadDelegate<T> load) where T : class
        {
            return Load(null, fileName, load);
        }
    }
}
