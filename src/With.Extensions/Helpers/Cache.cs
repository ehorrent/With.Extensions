using System;
using System.Collections.Concurrent;

namespace With.Helpers
{
    /// <summary>
    /// Contains memoization methods.
    /// Already computed results are stored into an internal ConcurrentDictionary.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Memoization on a method with 1 input argument.
        /// Already computed results are stored into an internal ConcurrentDictionary.
        /// </summary>
        /// <typeparam name="TInput">The type of the parameter of the method</typeparam>
        /// <typeparam name="TOutput">Type of the return value</typeparam>
        /// <param name="func">Method to decorate with "memoizer"</param>
        /// <returns>Memoized method</returns>
        public static Func<TInput, TOutput> Memoize<TInput, TOutput>(Func<TInput, TOutput> func)
        {
            if (null == func) throw new ArgumentNullException("func");

            var memoryCache = new ConcurrentDictionary<TInput, TOutput>();

            return (ctorInfo) =>
            {
                TOutput cacheEntry;
                if (memoryCache.TryGetValue(ctorInfo, out cacheEntry))
                    return cacheEntry;

                var result = func(ctorInfo);
                memoryCache[ctorInfo] = result;
                return result;
            };
        }

        /// <summary>
        /// Memoization on a method with 2 input arguments.
        /// Already computed results are stored into an internal ConcurrentDictionary.
        /// </summary>
        /// <typeparam name="TInput1">The type of the first parameter of the method</typeparam>
        /// <typeparam name="TInput2">The type of the second parameter of the method</typeparam>
        /// <typeparam name="TOutput">Type of the return value</typeparam>
        /// <param name="func">Method to decorate with "memoizer"</param>
        /// <returns>Memoized method</returns>
        public static Func<TInput1, TInput2, TOutput> Memoize<TInput1, TInput2, TOutput>(Func<TInput1, TInput2, TOutput> func)
        {
            if (null == func) throw new ArgumentNullException("func");

            var memoryCache = new ConcurrentDictionary<Tuple<TInput1, TInput2>, TOutput>();

            return (arg0, arg1) =>
            {
                TOutput cacheEntry;
                var cacheKey = Tuple.Create(arg0, arg1);
                if (memoryCache.TryGetValue(cacheKey, out cacheEntry))
                    return cacheEntry;

                var result = func(arg0, arg1);
                memoryCache[cacheKey] = result;
                return result;
            };
        }
    }
}
