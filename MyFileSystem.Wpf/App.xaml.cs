using System.Windows;
using MyFileSystem.Wpf.Infrastructure;

namespace MyFileSystem.Wpf
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            DependencyContainer.Dispose();

            base.OnExit(e);
        }
    }
}
