using NUnit.Framework;
using Ratana.Library.Cache;
using System;
using System.Threading;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.Cache
{
    [TestFixture]
    public class NoCacheTest
    {
        [Test]
        [Continuous, Integration]
        [TestCase("NoCacheTest:GetOrAdd:test-key1", "test-value", "test-fake-value")]
        [TestCase("NoCacheTest:GetOrAdd:test-key2", "", "test-fake-value")]
        public void GetOrAdd(string cacheKey, string cacheValue, string fakeValue)
        {
            #region Arrange
            // Set up some variables
            ICache cache = new NoCache();
            #endregion


            #region Act
            // 1 Try to save cacheValue1 under cacheKey.
            // Since there is no cache, cacheValue is returned.
            var returnedCacheValue1 = cache.GetOrAdd(cacheKey, () =>
            {
                return cacheValue;
            });

            // 2 Try to save cacheValue2 under cacheKey.
            // Since there is no cache, fakeValue is returned.
            var returnedCacheValue2 = cache.GetOrAdd(cacheKey, () =>
            {
                return fakeValue;
            });
            #endregion


            #region Assert
            // What we get back should equal what we put in, since there is no cache.
            Assert.AreEqual(cacheValue, returnedCacheValue1);
            Assert.AreEqual(fakeValue, returnedCacheValue2);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("		")]
        public void GetOrAdd_ShouldFailWithNullOrWhiteSpaceKey(string cacheKey)
        {
            #region Arrange 
            // Set up some variables
            ICache cache = new NoCache();
            #endregion


            #region Act & Assert
            // GetOrAdd() should throw ArgumentException because the key is invalid.
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var returnedCacheValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return "cacheValue";
                });
            });

            Assert.AreEqual("key", ex.ParamName);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("NoCacheTest:Remove:test-key", "test-value-1", "test-value-2")]
        [TestCase("NoCacheTest:Remove:test-key", "", "test-value-2")]
        public void Remove(string cacheKey, string cacheValue1, string cacheValue2)
        {
            #region Arrange
            // Set up some variables
            ICache cache = new NoCache();
            #endregion


            #region Act
            // 1 Try to save cacheValue1 under cacheKey.
            var returnedCacheValue1 = cache.GetOrAdd(cacheKey, () =>
            {
                return cacheValue1;
            });

            // 2 Remove the cacheKey
            cache.Remove(cacheKey);

            // 3 Try to get the cached value
            var tryGetResult = cache.TryGet(cacheKey, out string returnedCacheValue2);
            #endregion


            #region Assert
            // tryGetResult should always be true because there is no cache
            Assert.IsTrue(tryGetResult);

            // returnedCacheValue2 should be null because the key isn't in the cache (because there's no cache)
            Assert.AreEqual(default(string), returnedCacheValue2);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("		")]
        public void Remove_ShouldFailWithNullOrWhiteSpaceKey(string cacheKey)
        {
            #region Arrange 
            // Set up some variables
            ICache cache = new NoCache();
            #endregion


            #region Act & Assert
            // GetOrAdd() should throw ArgumentException because the key is invalid.
            var ex = Assert.Throws<ArgumentException>(() =>
            {
               cache.Remove(cacheKey);
            });

            Assert.AreEqual("key", ex.ParamName);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("NoCacheTest:TryGet_NonExistingKey:test-key")]
        public void TryGet_NonExistingKey(string cacheKey)
        {
            #region Arrange
            // Set up some variables
            ICache cache = new NoCache();

            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to get the non existing key
            var tryGetResult = cache.TryGet(cacheKey, out string returnedCacheValue1);
            #endregion


            #region Assert
            // tryGetResult should always be true because there is no cache
            Assert.IsTrue(tryGetResult);

            // returnedCacheValue1 should be null
            Assert.AreEqual(default(string), returnedCacheValue1);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("NoCacheTest:Expiration:test-key", "test-value-1")]
        public void Expiration(string cacheKey, string cacheValue1)
        {
            #region Arrange
            // Set up some variables
            ICache cache = new NoCache();

            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey for 1 ms
            var returnedCacheValue1 = cache.GetOrAdd(cacheKey, () =>
            {
                return cacheValue1;
            }, TimeSpan.FromMilliseconds(1));

            // 2 Wait for the cache to expired
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            // 3 Try to get the cached value
            var tryGetResult = cache.TryGet(cacheKey, out string returnedCacheValue2);
            #endregion


            #region Assert
            // tryGetResult should always be true because there is no cache
            Assert.IsTrue(tryGetResult);

            // returnedCacheValue2 should be null because the key isn't in the cache (because there's no cache)
            Assert.AreEqual(default(string), returnedCacheValue2);
            #endregion
        }
    }
}
