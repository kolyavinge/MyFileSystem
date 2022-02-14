using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace MyFileSystem.Wpf.Mvvm
{
    public interface IWindowsManager
    {
        TViewModel ShowDialog<TViewModel>() where TViewModel : BaseWindowViewModel;
    }

    public class WindowsManager : IWindowsManager
    {
        public TViewModel ShowDialog<TViewModel>() where TViewModel : BaseWindowViewModel
        {
            var windowType = GetWindowType<TViewModel>();
            var window = (Window)Activator.CreateInstance(windowType);
            var vm = (TViewModel)window.DataContext;
            vm.CloseEvent += (s, e) => window.Close();
            window.ShowDialog();

            return vm;
        }

        private Type GetWindowType<TViewModel>() where TViewModel : BaseWindowViewModel
        {
            return Assembly.GetCallingAssembly()
                .GetTypes()
                .First(t => t.GetCustomAttribute<WindowAttribute>()?.ViewModelType == typeof(TViewModel));
        }
    }
}
