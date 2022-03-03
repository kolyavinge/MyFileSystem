using System.IO;
using System.Reflection;
using MyFileSystem.Infrastructure;

namespace MyFileSystem.Wpf.Infrastructure
{
    public static class DependencyContainer
    {
        private static readonly DependencyInjection.DependencyContainer _container = new();

        static DependencyContainer()
        {
            var appDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            var dataDirectoryPath = Path.Combine(appDirectory, "data");
            _container.InitFromModules(new CoreInjectModule(dataDirectoryPath), new WpfInjectModule());
        }

        public static TDependency Resolve<TDependency>()
        {
            return _container.Resolve<TDependency>();
        }

        public static void Dispose()
        {
            _container.Dispose();
        }
    }
}
