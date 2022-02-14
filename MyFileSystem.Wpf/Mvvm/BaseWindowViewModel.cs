using System;
using System.Windows.Input;

namespace MyFileSystem.Wpf.Mvvm
{
    public class BaseWindowViewModel : NotificationObject
    {
        public event EventHandler CloseEvent;

        public bool IsOK { get; private set; }

        public ICommand OKCommand => new ActionCommand(OK);

        public ICommand CancelCommand => new ActionCommand(Cancel);

        protected virtual void OK()
        {
            Close(true);
        }

        protected virtual void Cancel()
        {
            Close(false);
        }

        protected void Close(bool ok)
        {
            IsOK = ok;
            CloseEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
