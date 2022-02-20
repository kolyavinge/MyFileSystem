using System;
using System.Collections.Generic;
using System.Text;

namespace MyFileSystem.Utils
{
    public static class SetExt
    {
        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                set.Add(item);
            }
        }
    }
}
