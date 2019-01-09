using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ratana.Library.Cache
{
    /// <summary>
    /// An implementation of the IMultilevelCache interface using Ratana.Library.DistributedCache.MultilevelCache.
    /// that contains a list of ICache.
    /// Each ICache can save the data at different intervals, with the lowest level having
    /// presumably the fastest retrieval time.
    /// </summary>
    public class MultilevelDistributedCache : ICache, IMultilevelCache
    {
        public Ratana.Library.DistributedCache.MultilevelCache Cache
        {
            get => this._cache;
        }

        private Ratana.Library.DistributedCache.MultilevelCache _cache;

        public MultilevelDistributedCache(Ratana.Library.DistributedCache.MultilevelCache cache)
        {
            this._cache = cache;
        }

        T ICache.GetOrAdd<T>(string key, Func<T> orAdd, TimeSpan expiration)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            T value = default(T);

            // Try to get the item
            // If the item is not found, add it to the cache.
            if (!((ICache)this).TryGet(key, out value))
            {
                value = orAdd();

                string json = null;
                if (value != null)
                {
                    json = JsonConvert.SerializeObject(value);
                }

                this._cache.SetString(key, json, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = expiration
                });
            }

            return value;
        }

        T IMultilevelCache.GetOrAdd<T>(string key, Func<T> orAdd, params TimeSpan[] expiration)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            // Validate expiration array.  Make sure each cache level is accounted for.
            if (expiration.Length != this._cache.Caches.Count)
            {
                throw new ArgumentOutOfRangeException(
                    "expiration",
                    "The number of cache expirations must match the number of Caches.");
            }

            // This variable tells us up to which level of the caches need to be updated.
            // The default is 0; meaning the key is found in first level cache, 
            // therefore nothing needs to be updated.
            int cacheLevelNeedingUpdate = 0;

            // Set the value to default.
            T value = default(T);

            // Loop through the caches until the key is found
            foreach (IDistributedCache cache in this._cache.Caches)
            {
                var json = cache.GetString(key);
                if (json != null)
                {
                    value = JsonConvert.DeserializeObject<T>(json);
                    break;
                }

                cacheLevelNeedingUpdate++;
            }

            // If the value is still default, then the key was not found.
            // Call orAdd() to get the value.
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                value = orAdd();
            }

            int ii = 0;
            // Save the value back into the caches that need them.
            // Since the value was not found via TryGet() above for some caches,
            // calling GetOrAdd() on those will trigger their orAdd().
            // Wrap the value in an anonymous function and pass it in as orAdd().
            foreach (IDistributedCache cache in this._cache.Caches)
            {
                if (ii >=  cacheLevelNeedingUpdate)
                {
                    break;
                }

                string json = JsonConvert.SerializeObject(value);

                cache.SetString(key, json, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = expiration[ii]
                });

                ii++;
            }


            return value;
        }

        void ICache.Remove(string key)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            this._cache.Remove(key);
        }

        Boolean ICache.TryGet<T>(string key, out T value)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            var json = this._cache.GetString(key);

            if (json == null)
            {
                value = default(T);
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(json);

            return true;
        }
    }
}
