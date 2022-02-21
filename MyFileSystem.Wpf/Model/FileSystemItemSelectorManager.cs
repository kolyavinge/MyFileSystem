using System;
using MyFileSystem.Model;

namespace MyFileSystem.Wpf.Model
{
    public interface IFileSystemItemSelectorManager
    {
        event EventHandler<ChangeSelectedItemEventArgs> ChangeSelectedItemEvent;
        FileSystemItem SelectedItem { get; set; }
    }

    public class FileSystemItemSelectorManager : IFileSystemItemSelectorManager
    {
        private FileSystemItem _selectedItem;

        public event EventHandler<ChangeSelectedItemEventArgs> ChangeSelectedItemEvent;

        public FileSystemItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                ChangeSelectedItemEvent?.Invoke(this, new ChangeSelectedItemEventArgs(_selectedItem));
            }
        }

    }

    public class ChangeSelectedItemEventArgs : EventArgs
    {
        public FileSystemItem SelectedItem { get; }

        public ChangeSelectedItemEventArgs(FileSystemItem selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }
}
