using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyFileSystem.Data.Repository;
using MyFileSystem.Model.Utils;
using MyFileSystem.Utils;

namespace MyFileSystem.Model
{
    public class FileSystemItem : NotificationObject
    {
        private readonly IFileSystemRepository _fileSystemRepository;
        private IEnumerable<FileSystemItem> _children;
        private string _name;

        internal uint Id { get; private set; }

        public string Name
        {
            get => _name;
            internal set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Extension => Path.GetExtension(Name);

        public FileSystemItemKind Kind { get; private set; }

        public FileKind FileKind { get; private set; }

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

        public FileSystemItem(
            IFileSystemRepository fileSystemRepository,
            uint id,
            FileSystemItemKind kind,
            string name)
        {
            _fileSystemRepository = fileSystemRepository;
            Id = id;
            Kind = kind;
            Name = name;
            FileKind = ImageFileFormats.Extensions.Contains(Path.GetExtension(Name)) ? FileKind.Image : FileKind.Other;
        }

        internal void AddChildren(IEnumerable<FileSystemItem> items)
        {
            _children = _children.Union(items).ToList();
            RaisePropertyChanged(() => Children);
        }
    }
}
