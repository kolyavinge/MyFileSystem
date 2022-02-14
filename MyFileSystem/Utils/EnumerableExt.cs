using System;
using System.Collections.Generic;
using System.Text;

namespace MyFileSystem.Utils
{
    public static class EnumerableExt
    {
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
