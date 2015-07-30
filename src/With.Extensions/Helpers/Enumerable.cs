using System.Collections.Generic;

namespace With.Helpers
{
    /// <summary>
    /// Internal extensions on IEnumerable
    /// </summary>
    internal static class Enumerable
    {
        /// <summary>
        /// Concatenate an enumeration with a single item
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate</typeparam>
        /// <param name="source">Sequence to concatenete</param>
        /// <param name="newItem">Item to add</param>
        /// <returns>New sequence with item added</returns>
        internal static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T newItem)
        {
            foreach (var item in source)
            {
                yield return item;
            }
            
            yield return newItem;
        }
    }
}
