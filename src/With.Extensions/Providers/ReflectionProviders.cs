using System;
using System.Reflection;

namespace With.Providers
{
    /// <summary>
    /// Contains providers using pure (and slow) reflection methods to get results.
    /// </summary>
    public static class ReflectionProviders
    {
        /// <summary>
        /// Provides a constructor method, based on the specified constructor infos (by using ConstructorInfo.Invoke method)
        /// </summary>
        /// <param name="ctorInfo">Metadatas used to create the constructor</param>
        /// <returns>Corresponding constructor (if existing)</returns>
        public static Constructor GetConstructor(ConstructorInfo ctorInfo)
        {
            return ctorInfo.Invoke;
        }

        /// <summary>
        /// Returns a value provider for the specified type and member (by using PropertyInfo.GetValue or FieldInfo.GetValue)
        /// </summary>
        /// <param name="type">The type containing property/field named 'memberName'</param>
        /// <param name="propertyOrFieldName">The name of a property/field to be accessed</param>
        /// <returns>Value of the property/field</returns>
        public static PropertyOrFieldProvider GetPropertyOrFieldProvider(Type type, string propertyOrFieldName)
        {
            return obj =>
            {
                var typeInfo = type.GetTypeInfo();

                // Property ?
                var propertyInfo = typeInfo.GetDeclaredProperty(propertyOrFieldName);
                if (null != propertyInfo)
                    return propertyInfo.GetValue(obj);

                // Field ?
                var fieldInfo = typeInfo.GetDeclaredField(propertyOrFieldName);
                if (null != fieldInfo)
                    return fieldInfo.GetValue(obj);

                throw new InvalidOperationException(
                    string.Format(
                        "Unable to find a field/property value for '{0}'",
                        propertyOrFieldName));
            };
        }
    }
}
