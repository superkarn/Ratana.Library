using System;
using Microsoft.Extensions.Caching.Memory;

namespace RatanaLibrary.Cache
{
    /// <summary>
    /// An in-memory implementation of the ICache interface.
    /// </summary>
    public class InMemoryCache : ICache
    {
        private IMemoryCache iMemoryCache;
        private static Object Lock = new Object();

        public InMemoryCache()
        {
            MemoryCacheOptions options = new MemoryCacheOptions();
            this.iMemoryCache = new MemoryCache(options);
        }

        T ICache.GetOrAdd<T>(String key, Func<T> orAdd, TimeSpan expiration)
        {
            T value = default(T);

            // Try to get the item
            // If the item is not found, lock and try again.
            // If it's not found this time, add it to the cache.
            if (!((ICache)this).TryGet(key, out value))
            {
                lock (InMemoryCache.Lock)
                {
                    if (!((ICache)this).TryGet(key, out value))
                    {
                        value = orAdd();

                        using (var entry = this.iMemoryCache.CreateEntry(key))
                        {
                            entry.Value = value;
                            this.iMemoryCache.Set(key, value, DateTime.Now.Add(expiration));
                        }
                    }
                }
            }

            return value;
        }

        void ICache.Remove(String key)
        {
            this.iMemoryCache.Remove(key);
        }



        /// <summary>
        /// Look for the item in the cache.
        /// If the item is found, set the value and return true,
        /// else set the value to default and return false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Boolean ICache.TryGet<T>(String key, out T value)
        {
            return this.iMemoryCache.TryGetValue<T>(key, out value);
        }
    }
}
