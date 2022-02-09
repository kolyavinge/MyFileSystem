using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Data;
using MyFileSystem.Data.Repository;
using MyFileSystem.Model.Utils;

namespace MyFileSystem.Model
{
    public class FileSystemItem : NotificationObject
    {
        private readonly IFileSystemRepository _fileSystemRepository;
        private IEnumerable<FileSystemItem> _children;

        internal uint Id { get; set; }

        public string Name { get; set; }

        public FileSystemItemKind Kind { get; set; }

        public IEnumerable<FileSystemItem> Children
        {
            get
            {
                if (Kind == FileSystemItemKind.Directory && _children == null)
                {
                    _children = _fileSystemRepository.GetChildren(Id).Select(x => FileSystemItemConverter.ToFileSystemItem(_fileSystemRepository, x)).ToList();
                }

                return _children;
            }
        }

        public FileSystemItem(IFileSystemRepository fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
        }

        internal void AddChildren(IEnumerable<FileSystemItem> items)
        {
            _children = _children.Union(items).ToList();
            RaisePropertyChanged(() => Children);
        }
    }
}
