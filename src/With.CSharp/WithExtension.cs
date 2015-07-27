using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using With.CSharp;

namespace System
{
    /// <summary>
    /// Provides 'With' method on all classes
    /// </summary>
    public static class WithExtension
    {
        /// <summary>
        /// Instance provider used by the extension to create new instances
        /// </summary>
        public static IInstanceProvider InstanceProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Copy and update extension, used to create a copy of an instance with one field/property updated.
        /// Designed to work on immutable classes with a single constructor.
        /// Names of the constructor's parameters must match names of corresponding public fields/properties (case is ignored).
        /// </summary>
        /// <typeparam name="TType">Type of the class to 'copy and update'</typeparam>
        /// <typeparam name="TField">Type of the field/property to update</typeparam>
        /// <param name="me">Instance to copy and update</param>
        /// <param name="selector">Selector on the field/property to update</param>
        /// <param name="value">New value for the field/property</param>
        /// <returns>Copied instance, with updated field/property</returns>
        public static TType With<TType, TField>(this TType me, Expression<Func<TType, TField>> selector, TField value) 
            where TType : class
        {
            var typeToBuild = typeof(TType);

            // Check if unique constructor is available
            var ctors = typeToBuild.GetConstructors();
            if (1 != ctors.Length)
                throw new InvalidOperationException("Type " + typeToBuild + " must only contain one constructor");

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
                        "Field/property not accessed from instance '{0}'",
                        selector.Parameters[0].Name));

            // Get constructor parameters
            var ctor = ctors[0];
            var ctorParams = ctor.GetParameters();
            
            // Get arguments values
            var arguments = ctorParams.Select((param, index) =>
            {
                if (param.Name.ToLower(CultureInfo.InvariantCulture) == memberExpression.Member.Name.ToLower(CultureInfo.InvariantCulture))
                    return value;

                // Field ?
                var fieldInfo = typeToBuild.GetField(param.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (null != fieldInfo)
                    return fieldInfo.GetValue(me);
                
                // Property ?
                var propertyInfo = typeToBuild.GetProperty(param.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (null != propertyInfo)
                    return propertyInfo.GetValue(me);

                throw new InvalidOperationException(
                    string.Format(
                        "Unable to find a value matching constructor's argument '{0}'", 
                        param.Name));
            }).ToArray();

            return InstanceProvider.Create<TType>(arguments);
        }
    }
}