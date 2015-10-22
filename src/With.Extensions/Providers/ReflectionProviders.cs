using System;
using System.Reflection;
using With.Helpers;

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
            if (null == ctorInfo) throw new ArgumentNullException("ctorInfo");

            return ctorInfo.Invoke;
        }

        /// <summary>
        /// Returns a value accessor for the specified type and member name (by using PropertyInfo.GetValue or FieldInfo.GetValue).
        /// Properties are given preference over fields.
        /// </summary>
        /// <param name="type">The type containing property/field named 'propertyOrFieldName'</param>
        /// <param name="propertyOrFieldName">The name of a property/field to be accessed</param>
        /// <returns>Accessor of the property/field</returns>
        public static PropertyOrFieldAccessor GetPropertyOrFieldAccessor(Type type, string propertyOrFieldName)
        {
            if (null == type) throw new ArgumentNullException("type");
            if (null == propertyOrFieldName) throw new ArgumentNullException("propertyOrFieldName");

            return obj =>
            {
                var typeInfo = type.GetTypeInfo();

                // Property ?
                var propertyInfo = typeInfo.GetProperty(propertyOrFieldName);
                if (null != propertyInfo)
                    return propertyInfo.GetValue(obj);

                // Field ?
                var fieldInfo = typeInfo.GetField(propertyOrFieldName);
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
