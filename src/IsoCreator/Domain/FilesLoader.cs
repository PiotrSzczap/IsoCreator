using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IsoCreator.Domain
{
    public class FilesLoader : IFilesLoader
    {
        public IEnumerable<FileContainer> GetFiles(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path), "Path cannot be null or white spaces");
            if (!Directory.Exists(path)) throw new ArgumentException($"Directory {path} does not exist");

            DirectoryInfo directory = new DirectoryInfo(path);
            return directory.GetFiles("*", SearchOption.AllDirectories)
                .Select(t => new FileContainer(t.FullName[path.Length..], t.FullName));

        }
    }
}
