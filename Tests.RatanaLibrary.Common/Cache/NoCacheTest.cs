using NUnit.Framework;
using RatanaLibrary.Common.Cache;
using System;

namespace Tests.RatanaLibrary.Common.Cache
{
    [TestFixture]
    public class NoCacheTest
    {
        [Test]
        public void GetOrAdd()
        {
            #region Arrange
            // Set up some variables
            var cacheKey = "test-key";
            var cacheValue1 = "test-value-1";
            var cacheValue2 = "test-value-2";
            var cache = new NoCache();
            #endregion


            #region Act
            // 1 Try to save cacheValue1 under cacheKey.
            var returnedCachValue1 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue1;
            });

            // 2 Try to save cacheValue2 under cacheKey.
            var returnedCachValue2 = ((ICache)cache).GetOrAdd(cacheKey, () =>
            {
                return cacheValue2;
            });
            #endregion


            #region Assert
            // What we get back should equal what we put in, since there is no cache.
            Assert.AreEqual(cacheValue1, returnedCachValue1);
            Assert.AreEqual(cacheValue2, returnedCachValue2);
            #endregion
        }

        [Test]
        public void Remove()
        {
            #region Arrange
            // Set up some variables
            var cacheKey = "test-key";
            var cacheValue1 = "test-value-1";
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
