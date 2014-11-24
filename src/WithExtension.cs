using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    public static class WithExtension
    {
        private static ITypeBuilder _typeBuilder;

        public static void RegisterBuilder(ITypeBuilder builder)
        {
            _typeBuilder = builder;
        }

        public static TType With<TType, TField>(this TType me, Expression<Func<TType, TField>> projection, TField value) 
            where TType : class
        {
            // Check if unique constructor is available
            var constructors = me.GetType().GetConstructors();
            if (1 != constructors.Length)
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

            var ctorInfo = constructors[0];
            var ctorParams = ctorInfo.GetParameters();
            var param = ctorParams.FirstOrDefault(p => p.Name == fieldName);

            return null;
        }
    }
}