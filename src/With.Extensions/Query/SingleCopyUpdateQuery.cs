using System;
using System.Collections;
using System.Collections.Generic;

namespace With.Query
{
    /// <summary>
    /// Provides informations to create and update an immutable instance.
    /// Used to update a single field.
    /// </summary>
    /// <typeparam name="T">Type of the instance to copy and update</typeparam>
    internal class SingleCopyUpdateQuery<T> : ICopyUpdateQuery<T>
        where T : class
    {
        /// <summary>
        /// Value to update (key = field/property name)
        /// </summary>
        private readonly KeyValuePair<string, object> _value;

        /// <summary>
        /// Create an instance of <see cref="T:CreateUpdateQuery"/> type
        /// </summary>
        /// <param name="source">Instance to wrap</param>
        /// <param name="memberValue">Value to update (key = field/property name)</param>
        public SingleCopyUpdateQuery(T source, KeyValuePair<string, object> memberValue)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));

            this.Source = source;
            this._value = memberValue;
        }

        /// <summary>
        /// Wrapped instance
        /// </summary>
        public T Source { get; }

        /// <summary>
        /// Values to update (key = field/property name)
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> MemberValues
        {
            get { yield return this._value; }
        }
    }
}