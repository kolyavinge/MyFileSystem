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

        public TreeViewExt()
        {
            SelectedItemChanged += OnSelectedItemChanged;
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemBindable = e.NewValue;
        }
    }
}
