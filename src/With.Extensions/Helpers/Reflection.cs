using System.Reflection;

namespace With.Helpers
{
    /// <summary>
    /// Helpers user to retrieve metadatas on properties or fields
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Recursive function used to retrieve property metadatas
        /// </summary>
        /// <param name="typeInfo">Type owning the property</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Property metadatas (null if not found)</returns>
        public static PropertyInfo GetProperty(this TypeInfo typeInfo, string propertyName)
        {
            var propertyInfo = typeInfo.GetDeclaredProperty(propertyName);
            if (null == propertyInfo && null != typeInfo.BaseType)
                propertyInfo = GetProperty(typeInfo.BaseType.GetTypeInfo(), propertyName);

            return propertyInfo;
        }

        /// <summary>
        /// Recursive function used to retrieve field metadatas
        /// </summary>
        /// <param name="typeInfo">Type owning the field</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>Field metadatas (null if not found)</returns>
        public static FieldInfo GetField(this TypeInfo typeInfo, string fieldName)
        {
            var fieldInfo = typeInfo.GetDeclaredField(fieldName);
            if (null == fieldInfo && null != typeInfo.BaseType)
                fieldInfo = GetField(typeInfo.BaseType.GetTypeInfo(), fieldName);

            return fieldInfo;
        }
    }
}
