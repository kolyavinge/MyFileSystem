using System.Windows;
using MyFileSystem.Wpf.Infrastructure;
using MyFileSystem.Wpf.Mvvm;
using MyFileSystem.Wpf.ViewModel;

namespace MyFileSystem.Wpf.View
{
    [WindowAttribute(typeof(SelectDirectoryViewModel))]
    public partial class SelectDirectoryView : Window
    {
        public SelectDirectoryView()
        {
            InitializeComponent();
            DataContext = DependencyContainer.Resolve<SelectDirectoryViewModel>();
        }
    }
}
