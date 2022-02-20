using System;
using System.Windows.Input;

namespace MyFileSystem.Wpf.Mvvm
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecuteFunc;

        public event EventHandler CanExecuteChanged;

        public ActionCommand(Action action, Func<bool> canExecuteFunc = null)
        {
            _action = action;
            _canExecuteFunc = canExecuteFunc ?? (() => true);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunc();
        }

        public void UpdateCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
