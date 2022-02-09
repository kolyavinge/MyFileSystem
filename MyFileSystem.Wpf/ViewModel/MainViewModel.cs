using MyFileSystem.Model.Utils;

namespace MyFileSystem.Wpf.ViewModel
{
    public class MainViewModel : NotificationObject
    {
        public FileSystemViewModel FileSystemViewModel { get; }

        public MainViewModel(FileSystemViewModel fileSystemViewModel)
        {
            FileSystemViewModel = fileSystemViewModel;
        }
    }
}
