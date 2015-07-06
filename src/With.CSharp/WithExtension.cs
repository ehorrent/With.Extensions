using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using With.CSharp;

namespace System
{
    public static class WithExtension
    {
        public static IInstanceProvider InstanceProvider
        {
            private get;
            set;
        }

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
                        "Field/property not accessed from parameter named '{0}'",
                        selector.Parameters[0].Name));

            // Get constructor parameters
            var ctor = ctors[0];
            var ctorParams = ctor.GetParameters();

            // Get arguments values
            var arguments = ctorParams.Select((param, index) =>
            {
                if (param.Name.ToLower(CultureInfo.InvariantCulture) == memberExpression.Member.Name.ToLower(CultureInfo.InvariantCulture))
                    return (object)value;

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
                        "Unable to find value for constructor argument named [{0}]", 
                        param.Name));
            }).ToArray();

            return InstanceProvider.Create<TType>(arguments);
        }
    }
}