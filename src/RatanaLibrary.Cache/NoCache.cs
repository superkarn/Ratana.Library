using System;

namespace RatanaLibrary.Cache
{
    /// <summary>
    /// An implementation of the ICache interface that does not do anything.
    /// This is useful for testing.
    /// </summary>
    public class NoCache : ICache
    {
        T ICache.GetOrAdd<T>(String key, Func<T> orAdd, TimeSpan expiration)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            return orAdd();
        }

        void ICache.Remove(String key)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            // Nothing to do
        }

        Boolean ICache.TryGet<T>(string key, out T value)
        {
            // Make sure there is a valid key.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key cannot be null or white space.", "key");
            }

            value = default(T);
            return true;
        }
    }
}
