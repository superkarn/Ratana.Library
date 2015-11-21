using System;
using System.Runtime.Caching;

namespace RatanaLibrary.Common.Cache
{
    /// <summary>
    /// An in-memory implementation of the ICache interface.
    /// </summary>
    public class InMemoryCache : ICache
    {
        private readonly TimeSpan DEFAULT_CACHE_DURATION = TimeSpan.FromDays(1);

        private static Object Lock = new Object();

        public InMemoryCache()
        {
        }


        T ICache.GetOrAdd<T>(String key, Func<T> orAdd)
        {
            return ((ICache)this).GetOrAdd(key, orAdd, DEFAULT_CACHE_DURATION);
        }

        T ICache.GetOrAdd<T>(String key, Func<T> orAdd, TimeSpan expiration)
        {
            T value = default(T);

            // Try to get the item
            // If the item is not found, lock and try again.
            // If it's not found this time, add it to the cache.
            if (!this.TryGet(key, out value))
            {
                lock(InMemoryCache.Lock)
                {
                    if (!this.TryGet(key, out value))
                    {
                        value = orAdd();
                        MemoryCache.Default.Add(key, value, DateTime.Now.Add(expiration));
                    }
                }
            }

            return value;
        }

        void ICache.Remove(String key)
        {
            MemoryCache.Default.Remove(key);
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
        private Boolean TryGet<T>(String key, out T value)
        {
            if (!MemoryCache.Default.Contains(key))
            {
                value = default(T);
                return false;
            }

            value = (T)MemoryCache.Default.Get(key);

            return true;
        }
    }
}
