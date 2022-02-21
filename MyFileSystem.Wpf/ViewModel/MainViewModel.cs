using MyFileSystem.Wpf.Mvvm;

namespace MyFileSystem.Wpf.ViewModel
{
    public class MainViewModel : NotificationObject
    {
        public FileSystemViewModel FileSystemViewModel { get; }

        public ImageFileViewModel ImageFileViewModel { get; }

        public MainViewModel(
            FileSystemViewModel fileSystemViewModel,
            ImageFileViewModel imageFileViewModel)
        {
            FileSystemViewModel = fileSystemViewModel;
            ImageFileViewModel = imageFileViewModel;
        }
    }
}
