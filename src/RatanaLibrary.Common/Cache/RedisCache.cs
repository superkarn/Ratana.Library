using System;

namespace RatanaLibrary.Common.Cache
{
    /// <summary>
    /// A Redis implementation of the ICache interface, using StackExchange.Redis
    /// </summary>
    public class RedisCache : ICache
    {
        public RedisCache()
        {
        }

        public RedisCache(RedisSettings settings)
        {

        }

        T ICache.GetOrAdd<T>(string key, Func<T> orAdd)
        {
            throw new NotImplementedException();
        }

        T ICache.GetOrAdd<T>(string key, Func<T> orAdd, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        void ICache.Remove(string key)
        {
            throw new NotImplementedException();
        }

        public class RedisSettings
        {
            public bool Enabled { get; }

            public string Server { get; }

            public int Port { get; }

            public TimeSpan Retry { get; }

            public int DatabaseId { get; }

            public int PoolSizeMultiplier { get; }
        }
    }
}
