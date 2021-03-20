using System.Collections.Generic;

namespace IsoCreator.Domain
{
    public interface IFilesLoader
    {
        IEnumerable<FileContainer> GetFiles(string path);
    }
}
