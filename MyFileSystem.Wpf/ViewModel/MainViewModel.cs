using MyFileSystem.Infrastructure;
using MyFileSystem.Wpf.Mvvm;

namespace MyFileSystem.Wpf.ViewModel
{
    public class MainViewModel : NotificationObject
    {
        [Inject]
        public FileSystemViewModel FileSystemViewModel { get; set; }

        [Inject]
        public ImageFileViewModel ImageFileViewModel { get; set; }
    }
}
