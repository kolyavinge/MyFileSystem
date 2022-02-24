using System;
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
        private double _imageWidth;
        private double _imageHeight;
        private double _viewportWidth;
        private double _viewportHeight;
        private double _scale;

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

        public double ImageWidth
        {
            get => _imageWidth;
            set
            {
                _imageWidth = value;
                RaisePropertyChanged(() => ImageWidth);
            }
        }

        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                RaisePropertyChanged(() => ImageHeight);
            }
        }

        public double ViewportWidth
        {
            get { return _viewportWidth; }
            set
            {
                _viewportWidth = value;
                RaisePropertyChanged(() => ViewportWidth);
            }
        }

        public double ViewportHeight
        {
            get { return _viewportHeight; }
            set
            {
                _viewportHeight = value;
                RaisePropertyChanged(() => ViewportHeight);
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                RaisePropertyChanged(() => Scale);
            }
        }

        public ActionCommand ScaleIncCommand { get; }
        public ActionCommand ScaleDecCommand { get; }
        public ActionCommand FillImageCommand { get; }

        public ImageFileViewModel(
            IFileSystem fileSystem,
            IFileSystemItemSelectorManager fileSystemItemSelectorManager)
        {
            _fileSystem = fileSystem;
            fileSystemItemSelectorManager.ChangeSelectedItemEvent += OnChangeSelectedItemEvent;
            Scale = 100.0;
            ScaleIncCommand = new ActionCommand(ScaleInc);
            ScaleDecCommand = new ActionCommand(ScaleDec);
            FillImageCommand = new ActionCommand(FillImage);
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
                ApplyScale();
            }
        }

        private void ApplyScale()
        {
            ImageWidth = _scale * ImageSource.Width / 100.0;
            ImageHeight = _scale * ImageSource.Height / 100.0;
        }

        private void ScaleInc()
        {
            var newScale = _scale + 10.0;
            if (newScale > 100.0) newScale = 100.0;
            Scale = newScale;
            ApplyScale();
        }

        private void ScaleDec()
        {
            var newScale = _scale - 10.0;
            if (newScale < 10.0) newScale = 10.0;
            Scale = newScale;
            ApplyScale();
        }

        private void FillImage()
        {
            ImageWidth = ViewportWidth;
            ImageHeight = ImageSource.Height * ViewportWidth / ImageSource.Width;
            if (ImageHeight > ViewportHeight)
            {
                ImageWidth = ImageWidth * ViewportHeight / ImageHeight;
                ImageHeight = ViewportHeight;
            }
        }
    }
}
