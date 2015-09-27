using System;
using System.Linq.Expressions;
using System.Reflection;

namespace With.Providers
{
    /// <summary>
    /// Contains providers using expression trees to return compiled lambda expression providers.
    /// </summary>
    public static class ExpressionProviders
    {
        /// <summary>
        /// Creates a constructor method, based on the specified constructor infos.
        /// </summary>
        /// <param name="ctorInfo">Metadatas used to create the constructor</param>
        /// <returns>Corresponding constructor (if existing)</returns>
        public static Constructor BuildConstructor(ConstructorInfo ctorInfo)
        {
            if (null == ctorInfo) throw new ArgumentNullException("ctorInfo");

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

        /// <summary>
        /// Creates a compiled property/field provider for a specified type and member name
        /// </summary>
        /// <param name="type">The type containing property/field named 'propertyOrFieldName'</param>
        /// <param name="propertyOrFieldName">The name of a property/field to be accessed</param>
        /// <returns>Value of the property/field</returns>
        public static PropertyOrFieldProvider BuildPropertyOrFieldProvider(Type type, string propertyOrFieldName)
        {
            if (null == type) throw new ArgumentNullException("type");
            if (null == propertyOrFieldName) throw new ArgumentNullException("propertyOrFieldName");

            // Get arguments
            var objArg = Expression.Parameter(typeof(object), "obj");
            var castedArg = Expression.Convert(objArg, type);
            var getValue = Expression.Convert(
                Expression.PropertyOrField(castedArg, propertyOrFieldName),
                typeof(object));

            var getMemberLambda = Expression.Lambda<PropertyOrFieldProvider>(
                getValue,
                objArg);

            return getMemberLambda.Compile();
        }
    }
}
