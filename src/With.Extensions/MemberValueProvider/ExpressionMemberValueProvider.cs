using System;
using System.Linq.Expressions;

namespace With
{
    /// <summary>
    /// Value provider, using expression trees to create compiled providers
    /// </summary>
    public static class ExpressionMemberValueProvider
    {
        /// <summary>
        /// Creates a compiled member value provider for a specified type
        /// </summary>
        /// <param name="type">The type containing property/field named 'memberName'</param>
        /// <param name="memberName">The name of a property/field to be accessed</param>
        /// <returns>Value of the property/field</returns>
        public static MemberValueProvider Create(Type type, string memberName)
        {
            // Get arguments
            var objArg = Expression.Parameter(typeof(object), "obj");
            var castedArg = Expression.Convert(objArg, type);
            var getValue = Expression.Convert(
                Expression.PropertyOrField(castedArg, memberName),
                typeof(object));
            
            var getMemberLambda = Expression.Lambda<MemberValueProvider>(
                getValue,
                objArg);

            return getMemberLambda.Compile();
        }
    }
}
