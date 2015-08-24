using System;
using System.Runtime.Caching;

namespace With.ConstructorProvider
{
    /// <summary>
    /// Decorator on an CreateConstructor delegate. 
    /// Use memory cache to store constructors.
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
        public static GetConstructor New(
            GetConstructor getConstructor, 
            MemoryCache memoryCache, 
            CacheItemPolicy cacheItemPolicy)
        {
            if (null == getConstructor) throw new ArgumentNullException("ctorProvider");
            if (null == memoryCache) throw new ArgumentNullException("memoryCache");
            if (null == cacheItemPolicy) throw new ArgumentNullException("cacheItemPolicy");

            return (ctorInfo) =>
            {
                var cacheKey = ctorInfo.MemberType.ToString();
                var cacheEntry = memoryCache.Get(cacheKey);
                if (null != cacheEntry)
                    return (Constructor)cacheEntry;

                var constructor = getConstructor(ctorInfo);
                memoryCache.Add(cacheKey, constructor, cacheItemPolicy);
                return constructor;
            };
        }
    }
}
