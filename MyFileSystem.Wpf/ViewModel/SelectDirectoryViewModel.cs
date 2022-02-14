using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Model;
using MyFileSystem.Wpf.Mvvm;

namespace MyFileSystem.Wpf.ViewModel
{
    public class SelectDirectoryViewModel : BaseWindowViewModel
    {
        private readonly IFileSystem _fileSystem;

        public IEnumerable<SelectDirectoryItemViewModel> Root =>
            new[] { new SelectDirectoryItemViewModel(_fileSystem.Root) { IsExpanded = true } };

        public SelectDirectoryItemViewModel SelectedFileSystemItem { get; set; }

        public SelectDirectoryViewModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
    }

    public class SelectDirectoryItemViewModel : ItemViewModel<FileSystemItem>
    {
        public string Name => InnerObject.Name;

        public IEnumerable<SelectDirectoryItemViewModel> Children =>
            InnerObject.Children.Where(x => x.Kind == FileSystemItemKind.Directory).Select(x => new SelectDirectoryItemViewModel(x));

        public bool IsExpanded { get; set; }

        public SelectDirectoryItemViewModel(FileSystemItem innerObject) : base(innerObject) { }
    }
}
