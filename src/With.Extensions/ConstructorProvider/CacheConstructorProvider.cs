using System;
using System.Collections.Concurrent;

namespace With.ConstructorProvider
{
    /// <summary>
    /// Decorator on a CreateConstructor delegate. 
    /// Use an internal ConcurrentDictionary to store constructors.
    /// </summary>
    public static class CacheConstructorProvider
    {
        /// <summary>
        /// Decorates a CreateConstructor to use memory cache.
        /// Cache key is just the name of the new type to create.
        /// </summary>
        /// <param name="getConstructor">Delegate to decorate</param>
        /// <returns>New constructor</returns>
        public static GetConstructor New(GetConstructor getConstructor)
        {
            if (null == getConstructor) throw new ArgumentNullException("getConstructor");

            var memoryCache = new ConcurrentDictionary<string, Constructor>();

            return (ctorInfo) =>
            {
                var cacheKey = ctorInfo.DeclaringType.Name;
                var cacheEntry = memoryCache[cacheKey];
                if (null != cacheEntry)
                    return cacheEntry;

                var constructor = getConstructor(ctorInfo);
                memoryCache[cacheKey] = constructor;
                return constructor;
            };
        }
    }
}
