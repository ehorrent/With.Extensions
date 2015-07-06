using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace With.CSharp.Factory
{
    public class ExpressionInstanceProvider : IInstanceProviderFactory
    {
        public Func<object[], T> GetProvider<T>(Type[] constructorSignature) where T : class
        {
            // Find constructor with matching argument types
            var ctorInfo = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                CallingConventions.HasThis,
                constructorSignature.ToArray(),
                new ParameterModifier[0]);

            // Convert enumeration to array
            var argsExpr = Expression.Parameter(typeof(object[]), "arguments");

            // Get constructor parameters values
            var ctorParameters = ctorInfo.GetParameters();
            var ctorParametersExpr = ctorParameters.Select((param, index) =>
            {
                var arrayAccessExpr = Expression.ArrayAccess(
                    argsExpr,
                    Expression.Constant(index, typeof(int))
                );

                return Expression.Convert(arrayAccessExpr, param.ParameterType);
            }).ToArray();

            var ctorExpr = Expression.New(ctorInfo, ctorParametersExpr);

            var creatorExpr = Expression.Lambda<Func<object[], T>>(
                ctorExpr,
                argsExpr);

            return creatorExpr.Compile();
        }
    }
}