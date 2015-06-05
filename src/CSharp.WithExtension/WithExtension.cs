using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    public static class WithExtension
    {
        private static ITypeBuilder _typeBuilder;

        public static void Register(ITypeBuilder builder)
        {
            _typeBuilder = builder;
        }

        public static TType With<TType, TField>(this TType me, Expression<Func<TType, TField>> projection, TField value) 
            where TType : class
        {
            // Check if unique constructor is available
            var ctors = typeof(TType).GetConstructors();
            if (1 != ctors.Length)
                throw new InvalidOperationException("Type must only contain one constructor");

            // Check if lambda is valid
            var memberExpression = projection.Body as MemberExpression;
            if (null == memberExpression)
                throw new ArgumentException("Lambda is not a member access");

            var fieldInfo = memberExpression.Member as FieldInfo;
            if (null == fieldInfo)
                throw new ArgumentException("Lambda is not a field access");
            
            if (projection.Parameters.Count != 1 || projection.Parameters[0] != memberExpression.Expression)
                throw new ArgumentException("Field not accessed from parameter");

            var fieldName = char.ToLowerInvariant(fieldInfo.Name[0]) + fieldInfo.Name.Substring(1);;

            // Get constructor parameters
            var ctor = ctors[0];
            var ctorParams = ctor.GetParameters();

            // Get arguments values
            var fields = typeof (TType).GetFields();
            var arguments = ctorParams.Select((param, index) =>
            {
                if (param.Name == fieldName)
                    return (object)value;

                // Get current field value
                return fields.First(field => field.Name.ToLower() == param.Name.ToLower())
                             .GetValue(me);
            });

            return _typeBuilder.Create<TType>(arguments);
        }
    }
}