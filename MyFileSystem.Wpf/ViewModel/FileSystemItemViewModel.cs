using System;
using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Data;
using MyFileSystem.Model;

namespace MyFileSystem.Wpf.ViewModel
{
    public class FileSystemItemViewModel : SimpleViewModel<FileSystemItem>
    {
        public FileSystemItemViewModel(FileSystemItem fileSystemItem) : base(fileSystemItem)
        {
            fileSystemItem.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
        }

        public string Name => InnerObject.Name;

        public FileSystemItemKind Kind => InnerObject.Kind;

        public IEnumerable<FileSystemItemViewModel> Children =>
            InnerObject.Children?.Select(x => new FileSystemItemViewModel(x)).OrderBy(x => x, DefaultComparer.Instance).ToList();

        class DefaultComparer : IComparer<FileSystemItemViewModel>
        {
            public readonly static DefaultComparer Instance = new DefaultComparer();

            public int Compare(FileSystemItemViewModel x, FileSystemItemViewModel y)
            {
                if (x.Kind == y.Kind) return String.Compare(x.Name, y.Name);
                if (x.Kind == FileSystemItemKind.Directory) return -1;
                return 1;
            }
        }
    }
}
