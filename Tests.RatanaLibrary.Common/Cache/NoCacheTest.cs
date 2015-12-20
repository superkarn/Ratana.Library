using NUnit.Framework;
using RatanaLibrary.Common.Cache;
using System;

namespace Tests.RatanaLibrary.Common.Cache
{
    [TestFixture]
    public class NoCacheTest
    {
        [Test]
        [TestCase("NoCacheTest:GetOrAdd:test-key1", "test-value", "test-fake-value")]
        [TestCase("NoCacheTest:GetOrAdd:test-key2", "", "test-fake-value")]
        public void GetOrAdd(string cacheKey, string cacheValue, string fakeValue)
        {
            #region Arrange
            // Set up some variables
            var cache = new NoCache();
            #endregion


            #region Act
            // 1 Try to save cacheValue1 under cacheKey.
            var returnedCachValue1 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue;
            });

            // 2 Try to save cacheValue2 under cacheKey.
            var returnedCachValue2 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return fakeValue;
            });
            #endregion


            #region Assert
            // What we get back should equal what we put in, since there is no cache.
            Assert.AreEqual(cacheValue, returnedCachValue1);
            Assert.AreEqual(fakeValue, returnedCachValue2);
            #endregion
        }

        [Test]
        [TestCase("NoCacheTest:Remove:test-key", "test-value-1", "test-value-2")]
        [TestCase("NoCacheTest:Remove:test-key", "", "test-value-2")]
        [TestCase("", "test-value-1", "test-value-2")]
        public void Remove(string cacheKey, string cacheValue1, string cacheValue2)
        {
            #region Arrange
            // Set up some variables
            var cache = new NoCache();
            #endregion


            #region Act
            // 1 Try to save cacheValue1 under cacheKey.
            var returnedCachValue1 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue1;
            });

            // 2 Remove the cacheKey
            ((ICache)cache).Remove(cacheKey);
            #endregion


            #region Assert
            // There is no cache.  There is nothing to assert.  
            // Just make sure NoCache.Remove() doesn't throw an exception
            #endregion
        }
    }
}
