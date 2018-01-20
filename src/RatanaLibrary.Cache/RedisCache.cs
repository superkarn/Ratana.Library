using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RatanaLibrary.Cache
{
    /// <summary>
    /// A Redis implementation of the ICache interface, using StackExchange.Redis
    /// </summary>
    public class RedisCache : ICache
    {
        ConnectionMultiplexer redis;
        IDatabase db;

        public RedisCache() : this(new RedisSettings() { Server = "localhost" })
        {
        }

        public RedisCache(RedisSettings settings) : this(new List<RedisSettings> { settings })
        {
        }

        public RedisCache(IList<RedisSettings> settings)
        {
            // create the multiplexer
            try
            {
                this.redis = ConnectionMultiplexer.Connect(
                    String.Join(",", settings.Select(x => x.Server + ":" + x.Port))
                );
            }
            catch (RedisConnectionException)
            {
                // do something here 
                throw;
            }

            // use the database id from the first settings object
            if (settings.FirstOrDefault() != null)
            {
                this.db = this.redis.GetDatabase(settings.FirstOrDefault().DatabaseId);
            }
            // if not available, use default db0
            else
            {
                this.db = this.redis.GetDatabase(0);
            }
        }

        T ICache.GetOrAdd<T>(String key, Func<T> orAdd, TimeSpan expiration)
        {
            T value = default(T);

            // Try to get the item
            // If the item is not found, lock and try again.
            // If it's not found this time, add it to the cache.
            if (!((ICache)this).TryGet(key, out value))
            {
                value = orAdd();

                String json = RedisValue.Null;
                if (value != null)
                {
                    json = JsonConvert.SerializeObject(value);
                }

                this.db.StringSet(key, json, expiration);
            }

            return value;
        }

        void ICache.Remove(String key)
        {
            this.db.KeyDelete(key);
        }

        private String Get(String key)
        {
            return this.db.StringGet(key);
        }

        /// <summary>
        /// Look for the item in the cache.
        /// If the item is found, set the value and return true,
        /// else set the out value to default and return false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Boolean ICache.TryGet<T>(String key, out T value)
        {
            String json = this.Get(key);

            if (json == null)
            {
                value = default(T);
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(json);

            return true;
        }

        public class RedisSettings
        {
            public bool Enabled { get; set; }

            public String Server { get; set; }

            public int Port { get; set; }

            public TimeSpan Retry { get; set; }

            public int DatabaseId { get; set; }

            public int PoolSizeMultiplier { get; set; }
        }
    }
}
