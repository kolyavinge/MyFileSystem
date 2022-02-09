using MyFileSystem.Model.Utils;

namespace MyFileSystem.Wpf.ViewModel
{
    public abstract class SimpleViewModel<TInnerObject> : NotificationObject
    {
        public TInnerObject InnerObject { get; }

        public SimpleViewModel(TInnerObject innerObject)
        {
            InnerObject = innerObject;
        }
    }
}
