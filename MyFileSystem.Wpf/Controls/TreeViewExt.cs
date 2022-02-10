using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MyFileSystem.Wpf.Controls
{
    public class TreeViewExt : TreeView
    {
        public object SelectedItemBindable
        {
            get { return GetValue(SelectedItemBindableProperty); }
            set { SetValue(SelectedItemBindableProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemBindableProperty =
            DependencyProperty.Register("SelectedItemBindable", typeof(object), typeof(TreeViewExt));

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemBindable = e.NewValue;
            base.OnSelectedItemChanged(e);
        }
    }
}
