using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using MyFileSystem.Model;
using MyFileSystem.Wpf.Controls;
using MyFileSystem.Wpf.Mvvm;

namespace MyFileSystem.Wpf.ViewModel
{
    public class FileSystemViewModel : NotificationObject
    {
        private readonly IFileSystem _fileSystem;
        private readonly IWindowsManager _windowsManager;
        private readonly IMessageBox _messageBox;
        private readonly FileSystemItemViewModel _rootItem;
        private FileSystemItemViewModel _selectedFileSystemItem;
        private string _newItemName;

        public IEnumerable<FileSystemItemViewModel> Root => new[] { _rootItem };

        public FileSystemItemViewModel SelectedFileSystemItem
        {
            get => _selectedFileSystemItem;
            set
            {
                OffRenameMode();
                _selectedFileSystemItem = value;
                OpenDirectoryOrFileCommand.UpdateCanExecute();
                AddFilesCommand.UpdateCanExecute();
                CreateDirectoryCommand.UpdateCanExecute();
                MoveToDirectoryCommand.UpdateCanExecute();
                DeleteItemCommand.UpdateCanExecute();
            }
        }

        public string NewItemName
        {
            get => _newItemName;
            set
            {
                _newItemName = value;
                RaisePropertyChanged(() => NewItemName);
            }
        }

        public ActionCommand OpenDirectoryOrFileCommand { get; private set; }
        public ActionCommand AddFilesCommand { get; private set; }
        public ActionCommand StartRenameCommand { get; private set; }
        public ActionCommand ApplyRenameCommand { get; private set; }
        public ActionCommand UndoRenameCommand { get; private set; }
        public ActionCommand CreateDirectoryCommand { get; private set; }
        public ActionCommand MoveToDirectoryCommand { get; private set; }
        public ActionCommand DeleteItemCommand { get; private set; }

        public FileSystemViewModel(
            IFileSystem fileSystem,
            IWindowsManager windowsManager,
            IMessageBox messageBox)
        {
            _fileSystem = fileSystem;
            _windowsManager = windowsManager;
            _messageBox = messageBox;
            _rootItem = new FileSystemItemViewModel(_fileSystem.Root) { IsExpanded = true };
            InitCommands();
            OffRenameMode();
        }

        private void InitCommands()
        {
            OpenDirectoryOrFileCommand = new ActionCommand(OpenDirectoryOrFile, () => SelectedFileSystemItem?.Kind == FileSystemItemKind.File);
            AddFilesCommand = new ActionCommand(AddFiles, () => SelectedFileSystemItem?.Kind == FileSystemItemKind.Directory);
            StartRenameCommand = new ActionCommand(StartRename, () => SelectedFileSystemItem != null && SelectedFileSystemItem != _rootItem);
            ApplyRenameCommand = new ActionCommand(ApplyRename, () => SelectedFileSystemItem != null);
            UndoRenameCommand = new ActionCommand(OffRenameMode);
            CreateDirectoryCommand = new ActionCommand(CreateDirectory, () => SelectedFileSystemItem?.Kind == FileSystemItemKind.Directory);
            MoveToDirectoryCommand = new ActionCommand(MoveToDirectory, () => SelectedFileSystemItem != null && SelectedFileSystemItem != _rootItem);
            DeleteItemCommand = new ActionCommand(DeleteItem, () => SelectedFileSystemItem != null && SelectedFileSystemItem != _rootItem);
        }

        private void AddFiles()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                _fileSystem.AddFiles(SelectedFileSystemItem.InnerObject, openFileDialog.FileNames);
                SelectedFileSystemItem.IsExpanded = true;
            }
        }

        private void OpenDirectoryOrFile()
        {
            if (SelectedFileSystemItem == null) return;
            if (SelectedFileSystemItem.Kind == FileSystemItemKind.File) _fileSystem.OpenFile(SelectedFileSystemItem.InnerObject);
            else if (SelectedFileSystemItem.Kind == FileSystemItemKind.Directory) SelectedFileSystemItem.IsExpanded = !SelectedFileSystemItem.IsExpanded;
        }

        private void StartRename()
        {
            NewItemName = SelectedFileSystemItem.Name;
            OnRenameMode();
        }

        private void ApplyRename()
        {
            OffRenameMode();
            _fileSystem.Rename(SelectedFileSystemItem.InnerObject, NewItemName);
        }

        private void CreateDirectory()
        {
            if (SelectedFileSystemItem?.Kind != FileSystemItemKind.Directory) return;
            var newDirectory = _fileSystem.CreateDirectory(SelectedFileSystemItem.InnerObject);
            var newDirectoryItemViewModel = SelectedFileSystemItem.Children.First(x => x.InnerObject == newDirectory);
            SelectedFileSystemItem.IsExpanded = true;
            SelectedFileSystemItem = newDirectoryItemViewModel;
            StartRename();
        }

        private void MoveToDirectory()
        {
            var vm = _windowsManager.ShowDialog<SelectDirectoryViewModel>();
            if (vm.IsOK)
            {
                var itemToMove = SelectedFileSystemItem.InnerObject;
                var parentDirectory = vm.SelectedFileSystemItem.InnerObject;
                _fileSystem.MoveTo(itemToMove, parentDirectory);
            }
        }

        private void DeleteItem()
        {
            var result = _messageBox.Show(
                "Точно удаляем?" + new string(' ', 20),
                "My File System",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question,
                System.Windows.MessageBoxResult.No);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _fileSystem.DeleteItems(new[] { SelectedFileSystemItem.InnerObject });
            }
        }

        private void OnRenameMode()
        {
            SelectedFileSystemItem?.SetRenameModeOn();
        }

        private void OffRenameMode()
        {
            SelectedFileSystemItem?.SetRenameModeOff();
        }
    }
}
