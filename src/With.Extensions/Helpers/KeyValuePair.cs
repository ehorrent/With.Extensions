using System.Collections.Generic;

namespace With.Helpers
{
    /// <summary>
    /// Helper class used to create KeyValuePair objects
    /// </summary>
    internal static class KeyValuePair
    {
        /// <summary>
        /// Create helper, just useful if you want use type inference
        /// </summary>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value associated with <paramref name="key"/></param>
        /// <returns>New KeyValuePair instance</returns>
        internal static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}