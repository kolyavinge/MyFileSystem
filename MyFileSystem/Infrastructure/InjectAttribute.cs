using System;

namespace MyFileSystem.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : DependencyInjection.InjectAttribute { }
}
