using System;
using System.IO;
using System.Reflection;
using MyFileSystem.Infrastructure;

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
            _container.InitFromModules(new CoreInjectModule(dataDirectoryPath), new WpfInjectModule());
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : DependencyInjection.InjectAttribute { }
}
