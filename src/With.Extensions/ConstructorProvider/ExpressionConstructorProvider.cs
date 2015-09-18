using System.Linq.Expressions;
using System.Reflection;

namespace With
{
    /// <summary>
    /// Constructor provider, using expression trees to create compiled constructors
    /// </summary>
    public static class ExpressionConstructorProvider
    {
        /// <summary>
        /// Provides a constructor, based on the specified constructor infos
        /// </summary>
        /// <param name="ctorInfo">Metadatas used to create the constructor</param>
        /// <returns>Corresponding constructor (if existing)</returns>
        public static Constructor Create(ConstructorInfo ctorInfo)
        {
            // Get arguments
            var argsExpr = Expression.Parameter(typeof(object[]), "arguments");

            // Get constructor parameters values
            var ctorParameters = ctorInfo.GetParameters();
            var ctorParametersExpr = new UnaryExpression[ctorParameters.Length];
            for (int i = 0; i < ctorParameters.Length; ++i)
            {
                var arrayAccessExpr = Expression.ArrayAccess(
                    argsExpr,
                    Expression.Constant(i, typeof(int)));

                ctorParametersExpr[i] = Expression.Convert(arrayAccessExpr, ctorParameters[i].ParameterType);
            }

            var ctorExpr = Expression.New(ctorInfo, ctorParametersExpr);

            var creatorExpr = Expression.Lambda<Constructor>(
                ctorExpr,
                argsExpr);

            return creatorExpr.Compile();
        }
    }
}