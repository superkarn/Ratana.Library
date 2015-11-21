using NUnit.Framework;
using RatanaLibrary.Common.Cache;
using System;

namespace Tests.RatanaLibrary.Common.Cache
{
    [TestFixture]
    public class RedisCacheTest
    {
        [Test]
        [TestCase("test-key1", "test-value", "test-fake-value")]
        [TestCase("test-key2", "", "test-fake-value")]
        public void GetOrAdd(string cacheKey, string cacheValue, string fakeValue)
        {
            #region Arrange
            // Set up some variables
            var cache = new RedisCache(new RedisCache.RedisSettings() { });

            // Make sure the key we're about to test is empty
            ((ICache)cache).Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue;
            });

            // 2 Try to save fakeValue under cacheKey.
            var returnedCachValue2 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return fakeValue;
            });
            #endregion


            #region Assert
            // returnedCachValue1 should equal cacheValue because the cache is empty.
            Assert.AreEqual(cacheValue, returnedCachValue1);

            // returnedCachValue2 should equal cacheValue because the cache exists, 
            // so fakeValue was never returned.
            Assert.AreEqual(cacheValue, returnedCachValue2);
            Assert.AreNotEqual(fakeValue, returnedCachValue2);
            #endregion
        }

        [Test]
        public void GetOrAddAnonymousType()
        {
            #region Arrange
            // Set up some variables
            var cacheKey = "test-key";
            var cacheValue = new { Name = "test-name", Value = "test-value" };
            var fakeValue = new { Name = "test-fake-name", Value = "test-fake-value" };
            var cache = new RedisCache(new RedisCache.RedisSettings() { });

            // Make sure the key we're about to test is empty
            ((ICache)cache).Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue;
            });

            // 2 Try to save fakeValue under cacheKey.
            var returnedCachValue2 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return fakeValue;
            });
            #endregion


            #region Assert
            // returnedCachValue1 should equal cacheValue because the cache is empty.
            Assert.AreEqual(cacheValue, returnedCachValue1);

            // returnedCachValue2 should equal cacheValue because the cache exists, 
            // so fakeValue was never returned.
            Assert.AreEqual(cacheValue, returnedCachValue2);
            Assert.AreNotEqual(fakeValue, returnedCachValue2);
            #endregion
        }

        [Test]
        public void Remove()
        {
            #region Arrange
            // Set up some variables
            var cacheKey = "test-key";
            var cacheValue1 = "test-value-1";
            var cacheValue2 = "test-value-2";
            var cache = new RedisCache(new RedisCache.RedisSettings() { });

            // Make sure the key we're about to test is empty
            ((ICache)cache).Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue1;
            });

            // 2 Remove the cacheKey
            ((ICache)cache).Remove(cacheKey);

            // 3 Try to save cacheValue2 under cacheKey again
            var returnedCachValue2 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue2;
            });
            #endregion


            #region Assert
            // returnedCachValue2 should equal cacheValue2 (but not cacheValue1)
            // because the cache was empty the second time.
            Assert.AreNotEqual(cacheValue1, returnedCachValue2);
            Assert.AreEqual(cacheValue2, returnedCachValue2);
            #endregion
        }
    }
}
