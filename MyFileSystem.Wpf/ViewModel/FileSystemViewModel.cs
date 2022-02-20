using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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
                RaisePropertyChanged(() => OpenFileCommandEnabled);
                RaisePropertyChanged(() => AddFilesCommandEnabled);
                RaisePropertyChanged(() => CreateDirectoryCommandEnabled);
                RaisePropertyChanged(() => MoveToDirectoryCommandEnabled);
                RaisePropertyChanged(() => DeleteItemCommandEnabled);
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

        public ICommand OpenDirectoryOrFileCommand => new ActionCommand(OpenDirectoryOrFile);

        public ICommand AddFilesCommand => new ActionCommand(AddFiles);

        public ICommand StartRenameCommand => new ActionCommand(StartRename);

        public ICommand ApplyRenameCommand => new ActionCommand(ApplyRename);

        public ICommand UndoRenameCommand => new ActionCommand(UndoRename);

        public ICommand CreateDirectoryCommand => new ActionCommand(CreateDirectory);

        public ICommand MoveToDirectoryCommand => new ActionCommand(MoveToDirectory);

        public ICommand DeleteItemCommand => new ActionCommand(DeleteItem);

        public bool OpenFileCommandEnabled => SelectedFileSystemItem?.Kind == FileSystemItemKind.File;

        public bool AddFilesCommandEnabled => SelectedFileSystemItem?.Kind == FileSystemItemKind.Directory;

        public bool CreateDirectoryCommandEnabled => SelectedFileSystemItem?.Kind == FileSystemItemKind.Directory;

        public bool MoveToDirectoryCommandEnabled => SelectedFileSystemItem != null && SelectedFileSystemItem != _rootItem;

        public bool DeleteItemCommandEnabled => SelectedFileSystemItem != null && SelectedFileSystemItem != _rootItem;

        public FileSystemViewModel(
            IFileSystem fileSystem,
            IWindowsManager windowsManager,
            IMessageBox messageBox)
        {
            _fileSystem = fileSystem;
            _windowsManager = windowsManager;
            _messageBox = messageBox;
            _rootItem = new FileSystemItemViewModel(_fileSystem.Root) { IsExpanded = true };
            OffRenameMode();
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
            if (SelectedFileSystemItem == null) return;
            if (SelectedFileSystemItem == _rootItem) return;
            NewItemName = SelectedFileSystemItem.Name;
            OnRenameMode();
        }

        private void ApplyRename()
        {
            if (SelectedFileSystemItem == null) return;
            OffRenameMode();
            _fileSystem.Rename(SelectedFileSystemItem.InnerObject, NewItemName);
        }

        private void UndoRename()
        {
            OffRenameMode();
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
            if (!DeleteItemCommandEnabled) return; // ??
            var result = _messageBox.Show(
                "Точно удаляем?",
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
