using System;
using System.Collections.Immutable;

namespace Abp.ObjectComparators
{
    public interface IObjectComparator
    {
        /// <summary>
        /// Comparator object type
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// List of compare types. For example: ["Equals", "Contains", "StartsWith", "EndsWith"].ToList();
        /// </summary>
        ImmutableList<string> CompareTypes { get; }

        /// <summary>
        /// Compare baseObject and compareObject with given compareType
        /// </summary>
        bool Compare(object baseObject, object compareObject, string compareType);

        /// <summary>
        /// Can compare given type with given compareType parameter
        /// </summary>
        bool CanCompare(Type baseObjectType, string compareType);
    }
}
