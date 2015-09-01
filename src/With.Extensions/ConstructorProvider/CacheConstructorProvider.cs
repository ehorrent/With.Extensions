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
        /// <param name="memoryCache">Memory cache used to store created delegates</param>
        /// <param name="cacheItemPolicy">Cache item policy</param>
        /// <returns>New constructor</returns>
        public static GetConstructor New(GetConstructor getConstructor)
        {
            if (null == getConstructor) throw new ArgumentNullException("ctorProvider");

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
