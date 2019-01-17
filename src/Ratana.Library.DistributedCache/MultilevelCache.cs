using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;

namespace Ratana.Library.DistributedCache
{
    /// <summary>
    /// An implementation of the IDistributedCache interface that provides multilevel caching.
    /// When instantiating MultilevelCache, the lowest level cache (e.g. L1) comes first in the parameter list.
    /// 
    /// TODO: there might be an issue with retrieving keys with empty values in Redis
    /// </summary>
    public class MultilevelCache : IDisposable, IDistributedCache
    {
        /// <summary>
        /// This is the list of caches to be used, 
        /// in the order that was received via the constructor.
        /// </summary>
        public IList<IDistributedCache> Caches { get; protected set; }

        public MultilevelCache(params IDistributedCache[] caches)
        {
            this.Caches = caches.ToList();
        }

        /// <summary>
        /// When MultilevelCache is being disposed, make sure to dispose of all caches that are IDisposable.
        /// </summary>
        public void Dispose()
        {
            foreach (IDistributedCache cache in this.Caches)
            {
                if (cache is IDisposable iDisposableCache)
                {
                    iDisposableCache.Dispose();
                }
            }
        }

        public byte[] Get(string key)
        {
            byte[] result = null;

            // Loop through the caches until the key is found
            foreach (IDistributedCache cache in this.Caches)
            {
                result = cache.Get(key);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        // TODO this method is not really async...
        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            byte[] result = null;
            IList<Task<byte[]>> tasks = new List<Task<byte[]>>();
            
            // Loop through each cache in order
            foreach (IDistributedCache cache in this.Caches)
            {
                var cacheTask = cache.GetAsync(key, token);
                cacheTask.Wait(token);
                result = cacheTask.Result;

                // If result is found from this cache, then no need to look further
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        public void Refresh(string key)
        {
            foreach (IDistributedCache cache in this.Caches)
            {
                cache.Refresh(key);
            }
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            foreach (IDistributedCache cache in this.Caches)
            {
                await cache.RefreshAsync(key, token);
            }
        }

        public void Remove(string key)
        {
            foreach (IDistributedCache cache in this.Caches)
            {
                cache.Remove(key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            foreach (IDistributedCache cache in this.Caches)
            {
                await cache.RemoveAsync(key, token);
            }
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            foreach (IDistributedCache cache in this.Caches)
            {
                cache.Set(key, value, options);
            }
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            foreach (IDistributedCache cache in this.Caches)
            {
                await cache.SetAsync(key, value, options, token);
            }
        }
    }
}
