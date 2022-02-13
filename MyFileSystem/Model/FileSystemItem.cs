using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyFileSystem.Data.Repository;
using MyFileSystem.Utils;

namespace MyFileSystem.Model
{
    public class FileSystemItem
    {
        public class NameChangeEventEventArgs : EventArgs
        {
            public string Name { get; set; }
        }

        public class AddChildrenEventEventArgs : EventArgs
        {
            public IEnumerable<FileSystemItem> NewChildren { get; set; }
        }

        private readonly IFileSystemRepository _fileSystemRepository;
        private List<FileSystemItem> _children;
        private string _name;

        public event EventHandler<NameChangeEventEventArgs> NameChangeEvent;

        public event EventHandler<AddChildrenEventEventArgs> AddChildrenEvent;

        internal uint Id { get; private set; }

        public string Name
        {
            get => _name;
            internal set
            {
                _name = value;
                FileKind = ImageFileFormats.Extensions.Contains(Path.GetExtension(_name)) ? FileKind.Image : FileKind.Other;
                NameChangeEvent?.Invoke(this, new NameChangeEventEventArgs { Name = _name });
            }
        }

        public string Extension => Path.GetExtension(Name);

        public FileSystemItemKind Kind { get; private set; }

        public FileKind FileKind { get; private set; }

        public IEnumerable<FileSystemItem> Children
        {
            get
            {
                if (!AreChildrenLoaded) LoadChildren();
                return _children;
            }
        }

        public bool AreChildrenLoaded
        {
            get
            {
                if (Kind == FileSystemItemKind.Directory) return _children != null;
                else return true;
            }
        }

        private void LoadChildren()
        {
            _children = _fileSystemRepository.GetChildren(Id).Select(x => FileSystemItemConverter.ToFileSystemItem(_fileSystemRepository, x)).ToList();
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
        }

        internal void AddChildren(IEnumerable<FileSystemItem> items)
        {
            if (AreChildrenLoaded) _children.AddRange(items);
            else LoadChildren();
            AddChildrenEvent?.Invoke(this, new AddChildrenEventEventArgs { NewChildren = items });
        }
    }
}
