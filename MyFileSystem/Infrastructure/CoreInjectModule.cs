using DependencyInjection;
using MyFileSystem.Data;
using MyFileSystem.Data.Repository;
using MyFileSystem.Logic;
using MyFileSystem.Model;

namespace MyFileSystem.Infrastructure
{
    public class CoreInjectModule : InjectModule
    {
        private readonly string _dataDirectoryPath;

        public CoreInjectModule(string dataDirectoryPath)
        {
            _dataDirectoryPath = dataDirectoryPath;
        }

        public override void Init(IBindingProvider bindingProvider)
        {
            bindingProvider.Bind<IDataContext>().ToMethod(provider => new DataContext(_dataDirectoryPath)).ToSingleton();
            bindingProvider.Bind<IFileSystemRepository>().ToMethod(provider => provider.Resolve<IDataContext>().FileSystemRepository).ToSingleton();
            bindingProvider.Bind<IDataFileRepository>().ToMethod(provider => provider.Resolve<IDataContext>().DataFileRepository).ToSingleton();
            bindingProvider.Bind<ITempFileManager>().ToMethod(provider => new TempFileManager(_dataDirectoryPath)).ToSingleton();
            bindingProvider.Bind<IGetFileLogic, GetFileLogic>().ToSingleton();
            bindingProvider.Bind<IAddFilesLogic, AddFilesLogic>().ToSingleton();
            bindingProvider.Bind<ICreateDirectoryLogic, CreateDirectoryLogic>().ToSingleton();
            bindingProvider.Bind<IDeleteFileSystemItemLogic, DeleteFileSystemItemLogic>().ToSingleton();
            bindingProvider.Bind<IFileSystem, FileSystem>().ToSingleton();
        }
    }
}
