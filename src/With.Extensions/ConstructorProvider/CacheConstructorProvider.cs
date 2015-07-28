using System;
using System.Runtime.Caching;

namespace With.ConstructorProvider
{
    /// <summary>
    /// Decorator on an IConstructorProvider. 
    /// Use memory cache to store constructors.
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
        /// Cache item expiration dela
        /// </summary>
        private readonly TimeSpan _cacheEntryDelay;

        /// <summary>
        /// Provides current time
        /// </summary>
        private readonly Func<DateTime> _getCurrentTime;

        /// <summary>
        /// Create an instance of <see cref="T:CacheConstructorProvider"/> type
        /// </summary>
        /// <param name="instanceProvider">Instance to decorate</param>
        /// <param name="memoryCache">Memory cache used to store constructors</param>
        /// <param name="cacheEntryDelay">Cache item expiration delay</param>
        /// <param name="getCurrentTime">Provides current time</param>
        public CacheConstructorProvider(IConstructorProvider instanceProvider, MemoryCache memoryCache, TimeSpan cacheEntryDelay, Func<DateTime> getCurrentTime)
        {
            if (null == instanceProvider) throw new ArgumentNullException("instanceProvider");
            if (null == memoryCache) throw new ArgumentNullException("memoryCache");
            if (null == getCurrentTime) throw new ArgumentNullException("getCurrentTime");

            this._instanceProvider = instanceProvider;
            this._memoryCache = memoryCache;
            this._cacheEntryDelay = cacheEntryDelay;
            this._getCurrentTime = getCurrentTime;
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

            var absoluteExpiration = this._getCurrentTime() + this._cacheEntryDelay;
            var constructor = this._instanceProvider.GetConstructor<T>(constructorSignature);
            this._memoryCache.Add(cacheKey, constructor, absoluteExpiration);
            return constructor;
        }
    }
}
