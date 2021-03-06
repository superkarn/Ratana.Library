﻿using System;

namespace Ratana.Library.Cache
{
    /// <summary>
    /// This interface is deprecated.  Please use Ratana.Library.DistributedCache instead.
    /// The code is kept for reference purposes.
    /// 
    /// Provides an easy-to-use generic caching interface.
    /// </summary>
    public interface ICache
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
        T GetOrAdd<T>(String key, Func<T> orAdd, TimeSpan expiration);

        /// <summary>
        /// Remove the item based on the key.
        /// </summary>
        /// <param name="key"></param>
        void Remove(String key);

        /// <summary>
        /// Try and get a cached item by key.  
        /// If the key is found, set it in value and return true.  
        /// Else return false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Boolean TryGet<T>(String key, out T value);
    }
}
