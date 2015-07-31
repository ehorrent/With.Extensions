using System;
using System.Runtime.Caching;

namespace With.ConstructorProvider
{
    /// <summary>
    /// Decorator on an IConstructorProvider. 
    /// Use a memory cache to store constructors.
    /// </summary>
    public class CacheConstructorProvider : IConstructorProvider
    {
        /// <summary>
        /// Instance to decorate
        /// </summary>
        private readonly IConstructorProvider _instanceProvider;

        /// <summary>
        /// Memory cache used to store constructors
        /// </summary>
        private readonly MemoryCache _memoryCache;

        /// <summary>
        /// Set of eviction and expiration details of the cache
        /// </summary>
        private readonly CacheItemPolicy _cacheItemPolicy;

        /// <summary>
        /// Create an instance of <see cref="T:CacheConstructorProvider"/> type
        /// </summary>
        /// <param name="instanceProvider">Instance to decorate</param>
        /// <param name="memoryCache">Memory cache used to store constructors</param>
        /// <param name="cacheItemPolicy">Set of eviction and expiration details of the cache</param>
        public CacheConstructorProvider(IConstructorProvider instanceProvider, MemoryCache memoryCache, CacheItemPolicy cacheItemPolicy)
        {
            if (null == instanceProvider) throw new ArgumentNullException("instanceProvider");
            if (null == memoryCache) throw new ArgumentNullException("memoryCache");

            this._instanceProvider = instanceProvider;
            this._memoryCache = memoryCache;
            this._cacheItemPolicy = cacheItemPolicy;
        }

        /// <summary>
        /// Provides a constructor, based on the given signature
        /// </summary>
        /// <typeparam name="T">Type of the instance to be created by the constructor</typeparam>
        /// <param name="constructorSignature">Constructor's signature</param>
        /// <returns>Corresponding constructor (if existing)</returns>
        public Func<object[], T> GetConstructor<T>(Type[] constructorSignature) where T : class
        {
            var cacheKey = typeof(T).ToString();
            var cacheEntry = this._memoryCache.Get(typeof(T).ToString());
            if (null != cacheEntry)
                return (Func<object[], T>)cacheEntry;

            var constructor = this._instanceProvider.GetConstructor<T>(constructorSignature);
            this._memoryCache.Add(cacheKey, constructor, this._cacheItemPolicy);
            return constructor;
        }
    }
}
