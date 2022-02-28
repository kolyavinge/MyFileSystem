using DependencyInjection;
using MyFileSystem.Wpf.Controls;
using MyFileSystem.Wpf.Model;
using MyFileSystem.Wpf.Mvvm;
using MyFileSystem.Wpf.ViewModel;

namespace MyFileSystem.Wpf.Infrastructure
{
    public class WpfInjectModule : InjectModule
    {
        public override void Init(IBindingProvider bindingProvider)
        {
            // model
            bindingProvider.Bind<IFileSystemItemSelectorManager, FileSystemItemSelectorManager>().ToSingleton();

            // mvvm
            bindingProvider.Bind<FileSystemViewModel, FileSystemViewModel>().ToSingleton();
            bindingProvider.Bind<SelectDirectoryViewModel, SelectDirectoryViewModel>().ToSingleton();
            bindingProvider.Bind<ImageFileViewModel, ImageFileViewModel>().ToSingleton();
            bindingProvider.Bind<MainViewModel, MainViewModel>().ToSingleton();
            bindingProvider.Bind<IWindowsManager, WindowsManager>().ToSingleton();
            bindingProvider.Bind<IMessageBox, MessageBox>().ToSingleton();
        }
    }
}
