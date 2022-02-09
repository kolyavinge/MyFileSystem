using MyFileSystem.Data;
using MyFileSystem.Data.Poco;
using MyFileSystem.Data.Repository;

namespace MyFileSystem.Model
{
    static class FileSystemItemConverter
    {
        public static FileSystemItem ToFileSystemItem(IFileSystemRepository fileSystemRepository, FileSystemItemPoco poco)
        {
            return new FileSystemItem(fileSystemRepository)
            {
                Id = poco.Id,
                Kind = (FileSystemItemKind)poco.Kind,
                Name = poco.Name
            };
        }
    }
}
