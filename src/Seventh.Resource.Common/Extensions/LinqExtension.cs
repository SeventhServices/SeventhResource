using System;
using System.Collections.Generic;

namespace Seventh.Resource.Common.Extensions
{
    public static class LinqExtension
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            foreach (var item in self) action(item);
        }
        

    }
}