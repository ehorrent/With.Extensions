using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace With.CSharp.Factory
{
    public class CacheInstanceProviderFactory : IInstanceProviderFactory
    {
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        private readonly IInstanceProviderFactory _instanceProvider;
        private readonly MemoryCache _memoryCache;
        private readonly TimeSpan _cacheEntryDelay;
        private readonly Func<DateTime> _getCurrentTime;

        public CacheInstanceProviderFactory(IInstanceProviderFactory instanceProvider, MemoryCache memoryCache, TimeSpan cacheEntryDelay, Func<DateTime> getCurrentTime)
        {
            if (null == instanceProvider) throw new ArgumentNullException("instanceProvider");
            if (null == memoryCache) throw new ArgumentNullException("memoryCache");
            if (null == getCurrentTime) throw new ArgumentNullException("getCurrentTime");

            this._instanceProvider = instanceProvider;
            this._memoryCache = memoryCache;
            this._cacheEntryDelay = cacheEntryDelay;
            this._getCurrentTime = getCurrentTime;
        }

        public Func<object[], T> GetProvider<T>(Type[] constructorSignature) where T : class
        {
            object cacheEntry;
            if (this._cache.TryGetValue(typeof(T), out cacheEntry)) //this._memoryCache.Get(cacheKey) as Func<IEnumerable<object>, T>;
                return (Func<object[], T>)cacheEntry;

            var creator = this._instanceProvider.GetProvider<T>(constructorSignature);
            this._cache.Add(typeof(T), creator);
            return creator;
        }
    }
}
