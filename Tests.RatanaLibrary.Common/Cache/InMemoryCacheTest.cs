using NUnit.Framework;
using RatanaLibrary.Common.Cache;
using System;

namespace Tests.RatanaLibrary.Common.Cache
{
    [TestFixture]
    public class InMemoryCacheTest
    {
        [Test]
        public void GetOrAdd()
        {
            #region Arrange
            // Set up some variables
            var cacheKey = "test-key";
            var cacheValue = "test-value";
            var fakeValue = "test-fake-value";
            var cache = new InMemoryCache();

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
            throw new NotImplementedException();
        }
    }
}
