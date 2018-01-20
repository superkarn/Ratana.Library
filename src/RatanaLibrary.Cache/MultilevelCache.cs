using System;
using System.Collections.Generic;
using System.Linq;

namespace RatanaLibrary.Cache
{
    /// <summary>
    /// An implementation of the ICache interface that does not do anything.
    /// This is useful for testing.
    /// </summary>
    public class MultilevelCache : IMultilevelCache
    {
        /// <summary>
        /// This is the list of caches to be used, 
        /// in the order that was received via the constructor.
        /// </summary>
        protected IList<ICache> Caches { get; set; }

        public MultilevelCache(params ICache[] caches)
        {
            this.Caches = caches.ToList();
        }

        T IMultilevelCache.GetOrAdd<T>(string key, Func<T> orAdd, params TimeSpan[] expiration)
        {
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
            }


            return value;
        }

        T ICache.GetOrAdd<T>(String key, Func<T> orAdd, TimeSpan expiration)
        {
            // If one expiration is passed in, treat it as only one level
            return ((IMultilevelCache)this).GetOrAdd(key, orAdd, expiration);
        }

        void ICache.Remove(String key)
        {
            foreach (ICache cache in this.Caches)
            {
                cache.Remove(key);
            }
        }

        Boolean ICache.TryGet<T>(string key, out T value)
        {
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
