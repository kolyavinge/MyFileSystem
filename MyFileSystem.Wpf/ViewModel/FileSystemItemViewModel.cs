using System;
using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Data;
using MyFileSystem.Model;

namespace MyFileSystem.Wpf.ViewModel
{
    public class FileSystemItemViewModel : ItemViewModel<FileSystemItem>
    {
        private bool _isExpanded;
        private bool _renameModeOff;
        private bool _renameModeOn;

        public FileSystemItemViewModel(FileSystemItem fileSystemItem) : base(fileSystemItem)
        {
            fileSystemItem.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
            IsRenameModeOn = false;
            IsRenameModeOff = true;
        }

        public string Name => InnerObject.Name;

        public FileSystemItemKind Kind => InnerObject.Kind;

        public FileKind FileKind => InnerObject.FileKind;

        public IEnumerable<FileSystemItemViewModel> Children =>
            InnerObject.Children?.Select(x => new FileSystemItemViewModel(x)).OrderBy(x => x, DefaultComparer.Instance).ToList();

        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; RaisePropertyChanged(() => IsExpanded); }
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
