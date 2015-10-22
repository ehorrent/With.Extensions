using System;
using System.Linq.Expressions;
using System.Reflection;
using With.Helpers;

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

            var ctorParameters = ctorInfo.GetParameters();

            // Get lambda arguments
            var argsExpr = Expression.Parameter(typeof(object[]), "arguments");

            // Check if lambda arguments count == constructor parameters count
            var validateArgsExpr = Expression.IfThen(
                Expression.NotEqual(
                    Expression.Constant(ctorParameters.Length), 
                    Expression.ArrayLength(argsExpr)),
                Expression.Throw(
                    Expression.Constant(new ArgumentOutOfRangeException("arguments"))));

            // Get expression for each constructor argument
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
                Expression.Block(
                    validateArgsExpr, 
                    ctorExpr),
                argsExpr);

            return creatorExpr.Compile();
        }

        /// <summary>
        /// Creates a compiled property/field accessor for a specified type and member name (by using Expression.PropertyOrField)
        /// </summary>
        /// <param name="type">The type containing property/field named 'propertyOrFieldName'</param>
        /// <param name="propertyOrFieldName">The name of a property/field to be accessed</param>
        /// <returns>Accessor of the property/field</returns>
        public static PropertyOrFieldAccessor BuildPropertyOrFieldAccessor(Type type, string propertyOrFieldName)
        {
            if (null == type) throw new ArgumentNullException("type");
            if (null == propertyOrFieldName) throw new ArgumentNullException("propertyOrFieldName");

            // Expression.PropertyOrField is case insensitive so we check manually if property/field exists with right naming
            var typeInfo = type.GetTypeInfo();
            var propertyInfo = typeInfo.GetProperty(propertyOrFieldName);
            var fieldInfo = typeInfo.GetField(propertyOrFieldName);
            if (null == propertyInfo && null == fieldInfo)
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to find a field/property value for '{0}' in type '{1}'",
                        propertyOrFieldName,
                        type.Name));
            
            // Get arguments
            var objArg = Expression.Parameter(typeof(object), "obj");
            var castedArg = Expression.Convert(objArg, type);
            var getValue = Expression.Convert(
                Expression.PropertyOrField(castedArg, propertyOrFieldName),
                typeof(object));
            
            // Create lambda : obj => obj.propertyOrFieldName
            var getMemberLambda = Expression.Lambda<PropertyOrFieldAccessor>(
                getValue,
                objArg);

            return getMemberLambda.Compile();
        }
    }
}
