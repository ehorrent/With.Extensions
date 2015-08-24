using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using With.ConstructorProvider;
using With.Helpers;
using With.Naming;
using With.Query;

namespace With
{
    /// <summary>
    /// Provides 'With' method on all classes
    /// </summary>
    public static class WithExtensions
    {
        /// <summary>
        /// Static constructor, used to instantiate default source provider.
        /// </summary>
        static WithExtensions()
        {
            // Default constructor, using pure reflection
            //// GetConstructor = ctor => ctor.Invoke;

            // For better performances, we put in cache compiled constructors
            GetConstructor = CacheConstructorProvider.New(
                ExpressionConstructorProvider.CreateConstructor,
                MemoryCache.Default,
                new CacheItemPolicy());
        }

        /// <summary>
        /// Constructor provider used by the extension
        /// </summary>
        public static GetConstructor GetConstructor
        {
            get;
            set;
        }

        /// <summary>
        /// Copy and update extension.
        /// </summary>
        /// <typeparam name="TSource">Type of the object to 'copy and update'</typeparam>
        /// <typeparam name="TMember">Type of the field/property to update</typeparam>
        /// <param name="source">Object to copy and update</param>
        /// <param name="memberSelector">Selector on the field/property to update</param>
        /// <param name="memberValue">New value for the field/property</param>
        /// <returns>Query used to create the new desired object</returns>
        public static ICopyUpdateQuery<TSource> With<TSource, TMember>(
            this TSource source, 
            Expression<Func<TSource, TMember>> memberSelector, 
            TMember memberValue)
            where TSource : class
        {
            // Get field/property name accessed by the selector
            var memberName = GetReturnedMemberName(memberSelector);
            
            // Create query
            return new SingleCopyUpdateQuery<TSource>(
                source, 
                KeyValuePair.Create(memberName, (object)memberValue));
        }

        /// <summary>
        /// Copy and update extension, used to allow chaining.
        /// </summary>
        /// <typeparam name="TSource">Type of the object to 'copy and update'</typeparam>
        /// <typeparam name="TMember">Type of the field/property to update</typeparam>
        /// <param name="query">Current query to update</param>
        /// <param name="memberSelector">Selector on the field/property to update</param>
        /// <param name="memberValue">New value for the field/property</param>
        /// <returns>Query used to create the new desired object</returns>
        public static ICopyUpdateQuery<TSource> With<TSource, TMember>(this ICopyUpdateQuery<TSource> query, Expression<Func<TSource, TMember>> memberSelector, TMember memberValue)
            where TSource : class
        {
            // Get field/property name accessed by the selector
            var memberName = GetReturnedMemberName(memberSelector);

            // Create query
            return new CopyUpdateQuery<TSource>(
                query.Source,
                query.MemberValues.Concat(KeyValuePair.Create(memberName, (object)memberValue)));
        }

        /// <summary>
        /// Copy and update an object.
        /// </summary>
        /// <typeparam name="TSource">Type of the object to 'copy and update'</typeparam>
        /// <param name="query">Query to execute</param>
        /// <param name="getMemberNameFromArgument">
        /// Returns the member name corresponding to a given argument name.
        /// If not specified, camel case convention is used.
        /// Only useful if you use a different naming convention for your members ('m_' prefix for example)
        /// </param>
        /// <returns>New object, with updated values</returns>
        public static TSource Create<TSource>(this ICopyUpdateQuery<TSource> query, Func<string, string> getMemberNameFromArgument = null)
            where TSource : class
        {
            getMemberNameFromArgument = getMemberNameFromArgument ?? CamelCase.Convert;

            var typeToBuild = typeof(TSource);

            // Check if unique constructor is available
            var ctorInfos = typeToBuild.GetConstructors();
            if (1 != ctorInfos.Length)
                throw new InvalidOperationException("Type " + typeToBuild + " must only contain one constructor");

            // Get constructor parameters
            var ctorInfo = ctorInfos[0];
            var ctorParams = ctorInfo.GetParameters();

            // Get arguments values
            var arguments = ctorParams.Select((arg, index) =>
            {
                // TODO : can be optimized
                var memberName = getMemberNameFromArgument(arg.Name);
                var newValue = query.MemberValues.Where(keyValue => keyValue.Key == memberName).Select(keyValue => keyValue.Value).FirstOrDefault();
                if (null != newValue)
                    return newValue;

                // Field ?
                var fieldInfo = typeToBuild.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (null != fieldInfo)
                    return fieldInfo.GetValue(query.Source);

                // Property ?
                var propertyInfo = typeToBuild.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (null != propertyInfo)
                    return propertyInfo.GetValue(query.Source);

                throw new InvalidOperationException(
                    string.Format(
                        "Unable to find a value matching constructor argument named '{0}'",
                        arg.Name));
            }).ToArray();

            var constructor = GetConstructor(ctorInfo);
            return (TSource)constructor(arguments);
        }

        /// <summary>
        /// Retrieve member name returned by a lambda expression.
        /// </summary>
        /// <typeparam name="TSource">Type of the object owning the member</typeparam>
        /// <typeparam name="TMember">Type of the member</typeparam>
        /// <param name="selector">Lambda expression to inspect</param>
        /// <returns>Member name returned by the lambda expression (lowered)</returns>
        private static string GetReturnedMemberName<TSource, TMember>(Expression<Func<TSource, TMember>> selector)
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
