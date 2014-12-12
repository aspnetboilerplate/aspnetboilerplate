using System.Collections.Generic;
using System.Linq;

namespace Abp.Extensions
{    /// <summary> 
    /// Extension methods for <see cref="Enumerable"/> class.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type System.String, using the specified separator between each member.
        /// This is a shortcut for string.Join(...)
        /// </summary>
        /// <param name="source">A collection that contains the strings to concatenate.</param>
        /// <param name="seperator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
        /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
        public static string JoinAsString(this IEnumerable<string> source, string seperator)
        {
            return string.Join(seperator, source);
        }

        /// <summary>
        /// Concatenates the members of a collection, using the specified separator between each member.
        /// This is a shortcut for string.Join(...)
        /// </summary>
        /// <param name="source">A collection that contains the objects to concatenate.</param>
        /// <param name="seperator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
        public static string JoinAsString<T>(this IEnumerable<T> source, string seperator)
        {
            return string.Join(seperator, source);
        }
    }
}
