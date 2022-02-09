using System.Windows;
using MyFileSystem.Wpf.Infrastructure;
using MyFileSystem.Wpf.ViewModel;

namespace MyFileSystem.Wpf.View
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = ServiceContainer.GetService<MainViewModel>();
        }
    }
}
