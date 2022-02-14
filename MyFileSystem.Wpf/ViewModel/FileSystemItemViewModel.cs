using System;
using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Model;

namespace MyFileSystem.Wpf.ViewModel
{
    public class FileSystemItemViewModel : ItemViewModel<FileSystemItem>
    {
        private IEnumerable<FileSystemItemViewModel> _children;
        private bool _isExpanded;
        private bool _renameModeOff;
        private bool _renameModeOn;

        public FileSystemItemViewModel(FileSystemItem fileSystemItem) : base(fileSystemItem)
        {
            SetRenameModeOff();
            fileSystemItem.NameChangeEvent += (s, e) =>
            {
                RaisePropertyChanged(() => Name);
                RaisePropertyChanged(() => FileKind);
            };
            if (fileSystemItem.Kind == FileSystemItemKind.Directory)
            {
                SetEventHandlers(fileSystemItem);
            }
        }

        private void SetEventHandlers(FileSystemItem fileSystemItem)
        {
            fileSystemItem.AddChildrenEvent += (s, e) =>
            {
                UpdateChildren(_children.Union(e.AddedChildren.Select(x => new FileSystemItemViewModel(x))));
            };

            fileSystemItem.RemoveChildrenEvent += (s, e) =>
            {
                var removedChildrenSet = new HashSet<FileSystemItem>(e.RemovedChildren);
                UpdateChildren(_children.Where(x => !removedChildrenSet.Contains(x.InnerObject)));
            };

            fileSystemItem.ChildrenMoveFromEvent += (s, e) =>
            {
                var removedChildrenSet = new HashSet<FileSystemItem>(e.MovedChildren);
                UpdateChildren(_children.Where(x => !removedChildrenSet.Contains(x.InnerObject)));
            };

            fileSystemItem.ChildrenMoveToEvent += (s, e) =>
            {
                UpdateChildren(_children.Union(e.MovedChildren.Select(x => new FileSystemItemViewModel(x))));
                IsExpanded = true;
            };
        }

        public string Name => InnerObject.Name;

        public FileSystemItemKind Kind => InnerObject.Kind;

        public FileKind FileKind => InnerObject.FileKind;

        public IEnumerable<FileSystemItemViewModel> Children
        {
            get
            {
                return _children ?? (_children = InnerObject.Children.Select(x => new FileSystemItemViewModel(x)).OrderBy(x => x, DefaultComparer.Instance).ToList());
            }
        }

        private void UpdateChildren(IEnumerable<FileSystemItemViewModel> children)
        {
            if (_children == null) return;
            _children = children.OrderBy(x => x, DefaultComparer.Instance).ToList();
            RaisePropertyChanged(() => Children);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(() => IsExpanded);
            }
        }

        public bool IsRenameModeOn
        {
            get => _renameModeOn;
            private set
            {
                _renameModeOn = value;
                RaisePropertyChanged(() => IsRenameModeOn);
            }
        }

        public bool IsRenameModeOff
        {
            get => _renameModeOff;
            private set
            {
                _renameModeOff = value;
                RaisePropertyChanged(() => IsRenameModeOff);
            }
        }

        public void SetRenameModeOn()
        {
            IsRenameModeOn = true;
            IsRenameModeOff = false;
        }

        public void SetRenameModeOff()
        {
            IsRenameModeOn = false;
            IsRenameModeOff = true;
        }

        class DefaultComparer : IComparer<FileSystemItemViewModel>
        {
            public readonly static DefaultComparer Instance = new DefaultComparer();

            public int Compare(FileSystemItemViewModel x, FileSystemItemViewModel y)
            {
                if (x.Kind == FileSystemItemKind.Directory && x.Kind == y.Kind)
                {
                    return String.Compare(x.Name, y.Name);
                }

                if (x.Kind == FileSystemItemKind.File && x.Kind == y.Kind && x.InnerObject.Extension == y.InnerObject.Extension)
                {
                    return String.Compare(x.Name, y.Name);
                }

                if (x.Kind == FileSystemItemKind.File && x.Kind == y.Kind)
                {
                    return String.Compare(x.InnerObject.Extension, y.InnerObject.Extension);
                }

                if (x.Kind == FileSystemItemKind.Directory)
                {
                    return -1;
                }

                return 1;
            }
        }
    }
}
