using System;
using System.IO;
using System.Reflection;
using MyFileSystem.Data;
using MyFileSystem.Data.Repository;
using MyFileSystem.Logic;
using MyFileSystem.Model;
using MyFileSystem.Wpf.Controls;
using MyFileSystem.Wpf.Model;
using MyFileSystem.Wpf.Mvvm;
using MyFileSystem.Wpf.ViewModel;

namespace MyFileSystem.Wpf.Infrastructure
{
    public static class DependencyContainer
    {
        private static readonly DependencyInjection.DependencyContainer _container;

        static DependencyContainer()
        {
            _container = new DependencyInjection.DependencyContainer();
            InitContainer();
        }

        public static TDependency Resolve<TDependency>()
        {
            return _container.Resolve<TDependency>();
        }

        public static void Dispose()
        {
            _container.Dispose();
        }

        private static void InitContainer()
        {
            var appDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            var dataDirectoryPath = Path.Combine(appDirectory, "data");

            // core
            _container.Bind<IDataContext>().ToMethod(() => new DataContext(dataDirectoryPath)).ToSingleton();
            _container.Bind<IFileSystemRepository>().ToMethod(() => _container.Resolve<IDataContext>().FileSystemRepository).ToSingleton();
            _container.Bind<IDataFileRepository>().ToMethod(() => _container.Resolve<IDataContext>().DataFileRepository).ToSingleton();
            _container.Bind<ITempFileManager>().ToMethod(() => new TempFileManager(dataDirectoryPath)).ToSingleton();
            _container.Bind<IGetFileLogic, GetFileLogic>().ToSingleton();
            _container.Bind<IAddFilesLogic, AddFilesLogic>().ToSingleton();
            _container.Bind<ICreateDirectoryLogic, CreateDirectoryLogic>().ToSingleton();
            _container.Bind<IDeleteFileSystemItemLogic, DeleteFileSystemItemLogic>().ToSingleton();
            _container.Bind<IFileSystem, FileSystem>().ToSingleton();

            // model
            _container.Bind<IFileSystemItemSelectorManager, FileSystemItemSelectorManager>().ToSingleton();

            // mvvm
            _container.Bind<FileSystemViewModel, FileSystemViewModel>().ToSingleton();
            _container.Bind<SelectDirectoryViewModel, SelectDirectoryViewModel>().ToSingleton();
            _container.Bind<ImageFileViewModel, ImageFileViewModel>().ToSingleton();
            _container.Bind<MainViewModel, MainViewModel>().ToSingleton();
            _container.Bind<IWindowsManager, WindowsManager>().ToSingleton();
            _container.Bind<IMessageBox, MessageBox>().ToSingleton();
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : DependencyInjection.InjectAttribute
    {
    }
}
