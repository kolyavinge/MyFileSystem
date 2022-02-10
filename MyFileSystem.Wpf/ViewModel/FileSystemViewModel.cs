using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Win32;
using MyFileSystem.Model;
using MyFileSystem.Model.Utils;
using MyFileSystem.Wpf.Utils;

namespace MyFileSystem.Wpf.ViewModel
{
    public class FileSystemViewModel : NotificationObject
    {
        private readonly IFileSystem _fileSystem;
        private FileSystemItemViewModel _selectedFileSystemItem;

        public IEnumerable<FileSystemItemViewModel> Root => new[] { new FileSystemItemViewModel(_fileSystem.Root) { IsExpanded = true } };

        public FileSystemItemViewModel SelectedFileSystemItem
        {
            get => _selectedFileSystemItem;
            set
            {
                _selectedFileSystemItem = value;
                RaisePropertyChanged(() => OpenFileCommandEnabled);
                RaisePropertyChanged(() => AddFilesCommandEnabled);
            }
        }

        public ICommand OpenFileCommand => new ActionCommand(OpenFile);

        public ICommand AddFilesCommand => new ActionCommand(AddFiles);

        public bool OpenFileCommandEnabled => SelectedFileSystemItem?.Kind == FileSystemItemKind.File;

        public bool AddFilesCommandEnabled => SelectedFileSystemItem?.Kind == FileSystemItemKind.Directory;

        public FileSystemViewModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private void AddFiles()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                _fileSystem.AddFiles(SelectedFileSystemItem.InnerObject, openFileDialog.FileNames);
            }
        }

        private void OpenFile()
        {
            if (SelectedFileSystemItem?.Kind != FileSystemItemKind.File) return;
            _fileSystem.OpenFile(SelectedFileSystemItem.InnerObject);
        }
    }
}
