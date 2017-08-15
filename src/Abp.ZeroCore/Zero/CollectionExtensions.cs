using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Abp.Zero
{
    internal static class CollectionExtensions
    {
        public static IList<T> RemoveAll<T>([NotNull] this ICollection<T> source, Func<T, bool> predicate)
        {
            var items = source.Where(predicate).ToList();

            foreach (var item in items)
            {
                source.Remove(item);
            }

            return items;
        }
    }
}
