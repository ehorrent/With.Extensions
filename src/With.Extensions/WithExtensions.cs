using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using With.Helpers;
using With.Naming;
using With.Providers;

namespace With
{
    /// <summary>
    /// Create a new instance, using specified constructor's arguments
    /// </summary>
    /// <param name="arguments">Ordered constructor's arguments</param>
    /// <returns>New instance</returns>
    public delegate object Constructor(object[] arguments);

    /// <summary>
    /// Returns a property/field value of the specified object
    /// </summary>
    /// <param name="obj">The object whose property/field value will be returned</param>
    /// <returns>The property/field value of the specified object</returns>
    public delegate object PropertyOrFieldProvider(object obj);

    /// <summary>
    /// Provides 'With' method on all classes
    /// </summary>
    public static class WithExtensions
    {
        /// <summary>
        /// Empty list of KeyValuePair elements
        /// </summary>
        private static readonly IEnumerable<KeyValuePair<string, object>> EmptyList = Enumerable.Empty<KeyValuePair<string, object>>();

        /// <summary>
        /// Static constructor, used to instantiate default providers.
        /// </summary>
        static WithExtensions()
        {
            ConstructorProvider = Cache.Memoize<ConstructorInfo, Constructor>(ExpressionProviders.BuildConstructor);
            PropertyOrFieldProvider = Cache.Memoize<Type, string, PropertyOrFieldProvider>(ExpressionProviders.BuildPropertyOrFieldProvider);
        }

        /// <summary>
        /// Provides constructors used to create new objects
        /// </summary>
        public static Func<ConstructorInfo, Constructor> ConstructorProvider { get; set; }

        /// <summary>
        /// Provides methods used to retrieve field/property values for a specified object
        /// </summary>
        public static Func<Type, string, PropertyOrFieldProvider> PropertyOrFieldProvider { get; set; }

        /// <summary>
        /// Creates a query to copy and update an object.
        /// </summary>
        /// <typeparam name="TSource">Type of the object to 'copy and update'</typeparam>
        /// <typeparam name="TPropertyOrField">Type of the field/property to update</typeparam>
        /// <param name="source">Object to copy and update</param>
        /// <param name="propertyOrFieldSelector">Selector on the field/property to update</param>
        /// <param name="value">New value for the field/property</param>
        /// <returns>Query used to create the new desired object</returns>
        public static CopyUpdateQuery<TSource> With<TSource, TPropertyOrField>(
            this TSource source,
            Expression<Func<TSource, TPropertyOrField>> propertyOrFieldSelector,
            TPropertyOrField value)
            where TSource : class
        {
            // Get field/property name accessed by the selector
            var propertyOrFieldName = GetReturnedPropertyOrFieldName(propertyOrFieldSelector);

            // Create query
            return new CopyUpdateQuery<TSource>(
                source,
                EmptyList.Concat(KeyValuePair.Create(propertyOrFieldName, (object)value)));
        }

        /// <summary>
        /// Creates a query to copy and update an object.
        /// </summary>
        /// <typeparam name="TSource">Type of the object to 'copy and update'</typeparam>
        /// <typeparam name="TPropertyOrField">Type of the field/property to update</typeparam>
        /// <param name="query">Current query to update</param>
        /// <param name="propertyOrFieldSelector">Selector on the field/property to update</param>
        /// <param name="value">New value for the field/property</param>
        /// <returns>Query used to create the new desired object</returns>
        public static CopyUpdateQuery<TSource> With<TSource, TPropertyOrField>(
            this CopyUpdateQuery<TSource> query,
            Expression<Func<TSource, TPropertyOrField>> propertyOrFieldSelector,
            TPropertyOrField value)
            where TSource : class
        {
            // Get field/property name accessed by the selector
            var propertyOrFieldName = GetReturnedPropertyOrFieldName(propertyOrFieldSelector);

            // Create query
            return new CopyUpdateQuery<TSource>(
                query.Source,
                query.PropertyOrFieldValues.Concat(KeyValuePair.Create(propertyOrFieldName, (object)value)));
        }

        /// <summary>
        /// Execute a query to copy and update an object.
        /// </summary>
        /// <typeparam name="TSource">Type of the object to 'copy and update'</typeparam>
        /// <param name="query">Query to execute</param>
        /// <param name="getPropertyOrFieldNameFromArgument">
        /// Returns the field/property name corresponding to a specified argument name.
        /// If not specified, pascal case convention is used.
        /// Only useful if you use a different naming convention for your members ('m_' prefix for example)
        /// </param>
        /// <returns>New object, with updated values</returns>
        public static TSource Create<TSource>(
            this CopyUpdateQuery<TSource> query,
            Func<string, string> getPropertyOrFieldNameFromArgument = null)
            where TSource : class
        {
            getPropertyOrFieldNameFromArgument = getPropertyOrFieldNameFromArgument ?? PascalCase.Convert;

            var typeToBuild = typeof(TSource);

            // Check if unique constructor is available
            var typeInfo = typeToBuild.GetTypeInfo();
            var ctorInfos = typeInfo.DeclaredConstructors;
            if (1 != ctorInfos.Count())
                throw new InvalidOperationException("Type " + typeToBuild + " must only contain one constructor");

            // Get constructor parameters
            var ctorInfo = ctorInfos.First();
            var ctorParams = ctorInfo.GetParameters();

            var updatedValues = query.PropertyOrFieldValues.ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value);

            // Get arguments values
            var arguments = new object[ctorParams.Length];
            for (int index = 0; index < ctorParams.Length; ++index)
            {
                var arg = ctorParams[index];
                var propertyOrFieldName = getPropertyOrFieldNameFromArgument(arg.Name);

                // Update value ?
                object newValue;
                if (updatedValues.TryGetValue(propertyOrFieldName, out newValue))
                {
                    arguments[index] = newValue;
                    continue;
                }

                // Get property/field value
                var valueProvider = PropertyOrFieldProvider(typeToBuild, propertyOrFieldName);
                arguments[index] = valueProvider(query.Source);
            }

            var constructor = ConstructorProvider(ctorInfo);
            return (TSource)constructor(arguments);
        }

        /// <summary>
        /// Retrieve field/property name returned by a lambda expression.
        /// </summary>
        /// <typeparam name="TSource">Type of the object owning the field/property</typeparam>
        /// <typeparam name="TPropertyOrField">Type of the field/property</typeparam>
        /// <param name="selector">Lambda expression to inspect</param>
        /// <returns>Field/property name returned by the lambda expression</returns>
        private static string GetReturnedPropertyOrFieldName<TSource, TPropertyOrField>(Expression<Func<TSource, TPropertyOrField>> selector)
            where TSource : class
        {
            // Check if lambda is valid
            var memberExpression = selector.Body as MemberExpression;
            if (null == memberExpression)
                throw new ArgumentException(
                    string.Format(
                        "Lambda '{0}'is not a member access",
                        selector.Name));

            // Check if lambda is a field/property access
            var isFieldOrPropertyAccess = memberExpression.Member is FieldInfo || memberExpression.Member is PropertyInfo;
            if (!isFieldOrPropertyAccess)
                throw new ArgumentException(
                    string.Format(
                        "Lambda '{0}' is not a field/property access",
                        selector.Name));

            // Check if field/property is accessed from lambda parameter
            if (selector.Parameters[0] != memberExpression.Expression)
                throw new ArgumentException(
                    string.Format(
                        "Field/property not accessed from source '{0}'",
                        selector.Parameters[0].Name));

            return memberExpression.Member.Name;
        }
    }
}
