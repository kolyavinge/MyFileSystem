using System.IO;
using System.Windows.Media.Imaging;
using MyFileSystem.Model;
using MyFileSystem.Wpf.Model;
using MyFileSystem.Wpf.Mvvm;

namespace MyFileSystem.Wpf.ViewModel
{
    public class ImageFileViewModel : NotificationObject
    {
        private readonly IFileSystem _fileSystem;
        private bool _isActive;
        private BitmapFrame _imageSource;

        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

        public BitmapFrame ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }

        public ImageFileViewModel(
            IFileSystem fileSystem,
            IFileSystemItemSelectorManager fileSystemItemSelectorManager)
        {
            _fileSystem = fileSystem;
            fileSystemItemSelectorManager.ChangeSelectedItemEvent += OnChangeSelectedItemEvent;
        }

        private void OnChangeSelectedItemEvent(object sender, ChangeSelectedItemEventArgs e)
        {
            IsActive = e.SelectedItem.FileKind == FileKind.Image;
            if (IsActive)
            {
                LoadImage(e.SelectedItem);
            }
        }

        private void LoadImage(FileSystemItem selectedItem)
        {
            var contentBytes = _fileSystem.GetFileContent(selectedItem);
            using (var stream = new MemoryStream(contentBytes))
            {
                ImageSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
    }
}
