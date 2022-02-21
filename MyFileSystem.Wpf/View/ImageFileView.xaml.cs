using System.Windows.Controls;
using MyFileSystem.Wpf.Infrastructure;
using MyFileSystem.Wpf.ViewModel;

namespace MyFileSystem.Wpf.View
{
    public partial class ImageFileView : UserControl
    {
        public ImageFileView()
        {
            InitializeComponent();
            DataContext = ServiceContainer.GetService<ImageFileViewModel>();
        }
    }
}
