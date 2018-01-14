using System;

namespace RatanaLibrary.Cache
{
    public static class CacheExtentions
    {
        private static readonly TimeSpan DEFAULT_CACHE_DURATION = TimeSpan.FromDays(1);

        public static T GetOrAdd<T>(this ICache cache, String key, Func<T> orAdd)
        {
            return cache.GetOrAdd(key, orAdd, DEFAULT_CACHE_DURATION);
        }
    }
}
