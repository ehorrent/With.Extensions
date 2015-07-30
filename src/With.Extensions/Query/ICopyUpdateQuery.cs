using System.Collections.Generic;

namespace With.Query
{
    /// <summary>
    /// Provides informations to create and update an object
    /// </summary>
    /// <typeparam name="T">Type of the object to copy and update</typeparam>
    public interface ICopyUpdateQuery<out T>
        where T : class
    {
        /// <summary>
        /// Wrapped instance
        /// </summary>
        T Source { get; }

        /// <summary>
        /// Values to update (key = field/property name)
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> MemberValues { get; }
    }
}