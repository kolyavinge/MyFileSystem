﻿using System;
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

        public class AddChildrenEventArgs : EventArgs
        {
            public IEnumerable<FileSystemItem> AddedChildren { get; set; }
        }

        public class DeleteChildrenEventArgs : EventArgs
        {
            public IEnumerable<FileSystemItem> DeletedChildren { get; set; }
        }

        public class ChildrenMoveFromEventArgs : EventArgs
        {
            public IEnumerable<FileSystemItem> MovedChildren { get; set; }
        }

        public class ChildrenMoveToEventArgs : EventArgs
        {
            public IEnumerable<FileSystemItem> MovedChildren { get; set; }
        }

        private readonly IFileSystemRepository _fileSystemRepository;
        private List<FileSystemItem> _children;
        private string _name;

        public event EventHandler<NameChangeEventEventArgs> NameChangeEvent;

        public event EventHandler<AddChildrenEventArgs> AddChildrenEvent;

        public event EventHandler<DeleteChildrenEventArgs> RemoveChildrenEvent;

        public event EventHandler<ChildrenMoveFromEventArgs> ChildrenMoveFromEvent;

        public event EventHandler<ChildrenMoveToEventArgs> ChildrenMoveToEvent;

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
                if (!ChildrenLoaded) LoadChildren();
                return _children;
            }
        }

        internal bool ChildrenLoaded => _children != null;

        public FileSystemItem Parent { get; internal set; }

        internal ushort DataFileNumber { get; }

        public int Depth
        {
            get
            {
                int depth = 0;
                var parent = Parent;
                while (parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }

                return depth;
            }
        }

        internal IEnumerable<FileSystemItem> GetAllChildren()
        {
            return Children.Union(Children.SelectMany(x => x.Children));
        }

        internal FileSystemItem(
            IFileSystemRepository fileSystemRepository,
            uint id,
            FileSystemItemKind kind,
            string name,
            FileSystemItem parent,
            ushort dataFileNumber)
        {
            _fileSystemRepository = fileSystemRepository ?? throw new NullReferenceException();
            Id = id;
            Kind = kind;
            Name = name;
            Parent = parent;
            DataFileNumber = dataFileNumber;
        }

        internal void AddChildren(IEnumerable<FileSystemItem> items)
        {
            items.Each(x => x.Parent = this);
            if (ChildrenLoaded) _children.AddRange(items);
            else LoadChildren();
            AddChildrenEvent?.Invoke(this, new AddChildrenEventArgs { AddedChildren = items });
        }

        internal void DeleteChildren(IEnumerable<FileSystemItem> items)
        {
            if (ChildrenLoaded)
            {
                items.Each(x => _children.Remove(x));
                RemoveChildrenEvent?.Invoke(this, new DeleteChildrenEventArgs { DeletedChildren = items });
            }
        }

        internal void ChildrenMoveFrom(IEnumerable<FileSystemItem> items)
        {
            if (ChildrenLoaded) items.Each(x => _children.Remove(x));
            ChildrenMoveFromEvent?.Invoke(this, new ChildrenMoveFromEventArgs { MovedChildren = items });
        }

        internal void ChildrenMoveTo(IEnumerable<FileSystemItem> items)
        {
            items.Each(x => x.Parent = this);
            if (ChildrenLoaded) _children.AddRange(items);
            else LoadChildren();
            ChildrenMoveToEvent?.Invoke(this, new ChildrenMoveToEventArgs { MovedChildren = items });
        }

        private void LoadChildren()
        {
            if (Kind == FileSystemItemKind.Directory)
            {
                _children = _fileSystemRepository
                    .GetChildren(Id)
                    .Select(poco => FileSystemItemConverter.ToFileSystemItem(_fileSystemRepository, poco, this))
                    .ToList();
            }
            else
            {
                _children = new List<FileSystemItem>();
            }
        }
    }
}
