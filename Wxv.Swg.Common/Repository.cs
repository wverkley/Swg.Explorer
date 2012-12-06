using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Caching;

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
                    TREFile = TREFile.Load(f.Stream)
                })
                .ToList();

            return new Repository
            {
                BaseDirectory = baseDirectory,
                Files = files
            };
        }

        public T Load<T>(string treFileName, string fileName, RepositoryLoadDelegate<T> load) where T : class
        {
            IRepositoryTREFile treFile = null;
            if (string.IsNullOrEmpty(treFileName))
                treFile = Files
                    .Where(f =>
                        string.Equals(f.FileName, treFileName, StringComparison.InvariantCultureIgnoreCase)
                        && f.TREFile.ContainsInfoFile(fileName))
                    .FirstOrDefault();
            if (treFile == null)
                treFile = Files
                    .Where(f => f.TREFile.ContainsInfoFile(fileName))
                    .FirstOrDefault();

            if (treFile == null)
                return null;

            string fullFileName = typeof(T).FullName + "-" + treFile.FileName + "/" + fileName;

            lock (treFile)
            {
                var result = _cache[fullFileName] as T;
                if (result != null)
                    return result;

                var treInfo = treFile.TREFile[fileName];
                using (var stream = treInfo.Open(treFile.Stream))
                    result = load(treFile.FileName, treInfo.Name, stream);
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
