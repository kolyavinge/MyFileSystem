using System.Windows.Controls;
using MyFileSystem.Wpf.Infrastructure;
using MyFileSystem.Wpf.ViewModel;

namespace MyFileSystem.Wpf.View
{
    public partial class FileSystemView : UserControl
    {
        public FileSystemView()
        {
            InitializeComponent();
            DataContext = DependencyContainer.Resolve<FileSystemViewModel>();
        }
    }
}
