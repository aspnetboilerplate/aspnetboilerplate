using System.Collections.Generic;

namespace Abp.Utils.Extensions
{
    /// <summary>
    /// Extension methods for Collections.
    /// </summary>
    internal static class CollectionExtensions
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count <= 0;
        }
    }
}