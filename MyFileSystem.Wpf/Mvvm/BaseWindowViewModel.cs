using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSystem.Wpf.Mvvm
{
    public class BaseWindowViewModel : NotificationObject
    {
        public event EventHandler CloseEvent;

        public bool IsOK { get; private set; }

        protected void Close(bool ok)
        {
            IsOK = ok;
            CloseEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
