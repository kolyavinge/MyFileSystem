using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MyFileSystem.Data;
using MyFileSystem.Data.Repository;
using MyFileSystem.Logic;
using MyFileSystem.Model;
using MyFileSystem.Wpf.ViewModel;

namespace MyFileSystem.Wpf.Infrastructure
{
    public static class ServiceContainer
    {
        private static readonly ServiceProvider _serviceProvider;

        static ServiceContainer()
        {
            _serviceProvider = ConfigureServices();
        }

        public static TService GetService<TService>()
        {
            return _serviceProvider.GetService<TService>();
        }

        public static void Dispose()
        {
            _serviceProvider.Dispose();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var appDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            var dataDirectoryPath = Path.Combine(appDirectory, "data");

            // core
            services.AddSingleton<IDataContext, DataContext>(_ => new DataContext(dataDirectoryPath));
            services.AddSingleton<IFileSystemRepository>(provider => provider.GetService<IDataContext>().FileSystemRepository);
            services.AddSingleton<IDataFileRepository>(provider => provider.GetService<IDataContext>().DataFileRepository);
            services.AddSingleton<ITempFileManager, TempFileManager>(_ => new TempFileManager(dataDirectoryPath));
            services.AddSingleton<IGetFileLogic, GetFileLogic>();
            services.AddSingleton<IAddFilesLogic, AddFilesLogic>();
            services.AddSingleton<IFileSystem, FileSystem>();

            // mvvm
            services.AddSingleton<FileSystemViewModel, FileSystemViewModel>();
            services.AddSingleton<MainViewModel, MainViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
