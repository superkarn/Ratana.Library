using System;

namespace Ratana.Library.Cache
{
    /// <summary>
    /// Provides an easy-to-use generic caching interface.
    /// </summary>
    public interface IMultilevelCache : ICache
    {
        /// <summary>
        /// Get the item based on the key from the cache if it exists.
        /// If not, it calls the orAdd method and saves the result with 
        /// the given expiration and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="orAdd"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        T GetOrAdd<T>(String key, Func<T> orAdd, params TimeSpan[] expiration);
    }
}
