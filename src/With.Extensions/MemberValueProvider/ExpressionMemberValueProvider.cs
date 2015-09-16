using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns></returns>
        public static MemberValueProvider GetMemberValueProvider()
        {
            // Get arguments
            var objArg = Expression.Parameter(typeof(object), "obj");
            var memberNameArg = Expression.Parameter(typeof(string), "memberName");
            var getValue = Expression.PropertyOrField(objArg, "");

            var getMemberLambda = Expression.Lambda<MemberValueProvider>(
                getValue,
                objArg,
                memberNameArg);

            return getMemberLambda.Compile();
        }
    }
}
