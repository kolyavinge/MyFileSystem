using MyFileSystem.Data.Poco;
using MyFileSystem.Data.Repository;

namespace MyFileSystem.Model
{
    static class FileSystemItemConverter
    {
        public static FileSystemItem ToFileSystemItem(IFileSystemRepository fileSystemRepository, FileSystemItemPoco poco, FileSystemItem parent)
        {
            return new FileSystemItem(
                fileSystemRepository,
                poco.Id,
                (FileSystemItemKind)poco.Kind,
                poco.Name,
                parent,
                poco.DataFileNumber
            );
        }
    }
}
