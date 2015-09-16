using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace With
{
    /// <summary>
    /// Decorator on a constructor provider. 
    /// Use an internal ConcurrentDictionary to store constructors.
    /// </summary>
    public static class CacheConstructorProvider
    {
        /// <summary>
        /// Decorates a constructor provider to use memory cache.
        /// Cache key is just the name of the new type to create.
        /// </summary>
        /// <param name="getConstructor">Delegate to decorate</param>
        /// <returns>New constructor provider</returns>
        public static Func<ConstructorInfo, Constructor> New(Func<ConstructorInfo, Constructor> getConstructor)
        {
            if (null == getConstructor) throw new ArgumentNullException("getConstructor");

            var memoryCache = new ConcurrentDictionary<string, Constructor>();

            return (ctorInfo) =>
            {
                var cacheKey = ctorInfo.DeclaringType.Name;
                Constructor cacheEntry;
                if (memoryCache.TryGetValue(cacheKey, out cacheEntry))
                    return cacheEntry;

                var constructor = getConstructor(ctorInfo);
                memoryCache[cacheKey] = constructor;
                return constructor;
            };
        }
    }
}
