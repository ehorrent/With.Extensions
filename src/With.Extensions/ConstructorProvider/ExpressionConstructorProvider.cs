using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace With.ConstructorProvider
{
    /// <summary>
    /// Constructor provider, using expression trees to create compiled constructors
    /// </summary>
    public static class ExpressionConstructorProvider
    {
        /// <summary>
        /// Provides a constructor, based on the given signature
        /// </summary>
        /// <param name="ctorInfo">Metadatas used to create the constructor</param>
        /// <returns>Corresponding constructor (if existing)</returns>
        public static Constructor CreateConstructor(ConstructorInfo ctorInfo)
        {
            // Get arguments
            var argsExpr = Expression.Parameter(typeof(object[]), "arguments");

            // Get constructor parameters values
            var ctorParameters = ctorInfo.GetParameters();
            var ctorParametersExpr = ctorParameters.Select((param, index) =>
            {
                var arrayAccessExpr = Expression.ArrayAccess(
                    argsExpr,
                    Expression.Constant(index, typeof(int)));

                return Expression.Convert(arrayAccessExpr, param.ParameterType);
            }).ToArray();

            var ctorExpr = Expression.New(ctorInfo, ctorParametersExpr);

            var creatorExpr = Expression.Lambda<Constructor>(
                ctorExpr,
                argsExpr);

            return creatorExpr.Compile();
        }
    }
}