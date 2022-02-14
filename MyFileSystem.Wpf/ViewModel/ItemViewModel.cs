using MyFileSystem.Wpf.Mvvm;

namespace MyFileSystem.Wpf.ViewModel
{
    public abstract class ItemViewModel<TInnerObject> : NotificationObject
    {
        public TInnerObject InnerObject { get; }

        public ItemViewModel(TInnerObject innerObject)
        {
            InnerObject = innerObject;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
    }
}
