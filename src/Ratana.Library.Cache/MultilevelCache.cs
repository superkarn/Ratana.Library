using System;
using System.Collections.Generic;
using System.Linq;

namespace Ratana.Library.Cache
{
    /// <summary>
    /// An implementation of the IMultilevelCache interface that contains a list of ICache.
    /// Each ICache can save the data at different intervals, with the lowest level having
    /// presumably the fastest retrieval time.
    /// </summary>
    public class MultilevelCache : IMultilevelCache
    {
        /// <summary>
        /// This is the list of caches to be used, 
        /// in the order that was received via the constructor.
        /// </summary>
        public IList<ICache> Caches { get; set; }

        public MultilevelCache(params ICache[] caches)
        {
            this.Caches = caches.ToList();
        }

        T IMultilevelCache.GetOrAdd<T>(string key, Func<T> orAdd, params TimeSpan[] expiration)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            // Validate expiration array.  Make sure each cache level is accounted for.
            if (expiration.Length != this.Caches.Count)
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
            foreach (ICache cache in this.Caches)
            {
                if (cache.TryGet(key, out value))
                {
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
            foreach (ICache cache in this.Caches)
            {
                if (ii >=  cacheLevelNeedingUpdate)
                {
                    break;
                }

                cache.GetOrAdd(key, () => value, expiration[ii]);
                ii++;
            }

            return value;
        }

        T ICache.GetOrAdd<T>(String key, Func<T> orAdd, TimeSpan expiration)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            // If one expiration is passed in, treat it as only one level
            return ((IMultilevelCache)this).GetOrAdd(key, orAdd, expiration);
        }

        void ICache.Remove(String key)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            foreach (ICache cache in this.Caches)
            {
                cache.Remove(key);
            }
        }

        Boolean ICache.TryGet<T>(string key, out T value)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            value = default(T);
            foreach (ICache cache in this.Caches)
            {
                if (cache.TryGet(key, out value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
