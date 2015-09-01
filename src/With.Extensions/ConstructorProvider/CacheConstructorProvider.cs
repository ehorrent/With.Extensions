using System;
using System.Collections.Concurrent;

namespace With.ConstructorProvider
{
    /// <summary>
    /// Decorator on an CreateConstructor delegate. 
    /// Use memory cache to store constructors.
    /// </summary>
    public static class CacheConstructorProvider
    {
        private static ConcurrentDictionary<string, Constructor> _memoryCache = new ConcurrentDictionary<string, Constructor>();

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

            return (ctorInfo) =>
            {
                var cacheKey = ctorInfo.DeclaringType.Name;
                var cacheEntry = _memoryCache[cacheKey];
                if (null != cacheEntry)
                    return cacheEntry;

                var constructor = getConstructor(ctorInfo);
                _memoryCache[cacheKey] = constructor;
                return constructor;
            };
        }
    }
}
