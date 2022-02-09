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

        public IEnumerable<FileSystemItemViewModel> Root { get; }

        public FileSystemItemViewModel SelectedFileSystemItem { get; set; }

        public ICommand OpenFileCommand { get; }

        public ICommand AddFilesCommand { get; }

        public FileSystemViewModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Root = new[] { new FileSystemItemViewModel(_fileSystem.Root) };
            OpenFileCommand = new ActionCommand(OpenFile);
            AddFilesCommand = new ActionCommand(AddFiles);
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
            _fileSystem.OpenFile(SelectedFileSystemItem.InnerObject);
        }
    }
}
