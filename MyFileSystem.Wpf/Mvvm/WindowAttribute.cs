using System;

namespace MyFileSystem.Wpf.Mvvm
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WindowAttribute : Attribute
    {
        public Type ViewModelType { get; private set; }

        public WindowAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }
}
