using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Ratana.Library.Cache;
using System;
using System.Threading;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.Cache
{
    [TestFixture]
    public class MultilevelCacheTest
    {
        private IMultilevelCache _cache;

        [SetUp]
        public void Initialize()
        {
            // Set up some variables
            var cacheL1 = new Mock<ICache>();
            var cacheL2 = new Mock<ICache>();
            var cacheL3 = new Mock<ICache>();

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:GetOrAdd:test-key1", "test-value", "test-fake-value")]
        [TestCase("MultilevelCacheTest:GetOrAdd:test-key2", "", "test-fake-value")]
        public void GetOrAdd(string cacheKey, string cacheValue, string fakeValue)
        {
            #region Arrange 
            // Set up some variables
            var cacheL1 = new Mock<ICache>();
            var cacheL2 = new Mock<ICache>();
            var cacheL3 = new Mock<ICache>();

            // These caches' GetOrAdd() should return cacheValue
            Func<String> orAdd1 = () => cacheValue;
            cacheL1.Setup(x => x.GetOrAdd(cacheKey, orAdd1, TimeSpan.FromSeconds(1))).Returns(cacheValue);
            cacheL2.Setup(x => x.GetOrAdd(cacheKey, orAdd1, TimeSpan.FromSeconds(1))).Returns(cacheValue);
            cacheL3.Setup(x => x.GetOrAdd(cacheKey, orAdd1, TimeSpan.FromSeconds(1))).Returns(cacheValue);

            // These caches' TryGet() should return cacheValue
            cacheL1.Setup(x => x.TryGet(cacheKey, out cacheValue)).Returns(true);
            cacheL2.Setup(x => x.TryGet(cacheKey, out cacheValue)).Returns(true);
            cacheL3.Setup(x => x.TryGet(cacheKey, out cacheValue)).Returns(true);

            IMultilevelCache cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCacheValue1;
            var returnedCacheValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            // 2 Try to save fakeValue under cacheKey.
            // Since the key already exist, fakeValue is never reached.
            var returnedCacheValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return fakeValue;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));
            #endregion


            #region Assert
            // returnedCacheValue1 should equal cacheValue because the cache was empty.
            Assert.AreEqual(cacheValue, returnedCacheValue1);

            // returnedCacheValue2 should equal cacheValue because the cache exists, 
            // so fakeValue was never returned.
            Assert.AreEqual(cacheValue, returnedCacheValue2);
            #endregion
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("		")]
        public void GetOrAdd_ShouldFailWithNullOrWhiteSpaceKey(string cacheKey)
        {
            #region Arrange 
            // Nothing to do
            #endregion


            #region Act & Assert
            // GetOrAdd() should throw ArgumentException because the key is invalid.
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var returnedCacheValue1 = this._cache.GetOrAdd(cacheKey, () =>
                    {
                        return "cacheValue";
                    },
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1));
            });

            Assert.AreEqual("key", ex.ParamName);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:Remove:test-key", "test-value-1", "test-value-2")]
        [TestCase("MultilevelCacheTest:Remove:test-key", "", "test-value-2")]
        public void Remove(string cacheKey, string cacheValue1, string cacheValue2)
        {
            #region Arrange
            // Make sure the key we're about to test is empty
            this._cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCacheValue1;
            var returnedCacheValue1 = this._cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue1;
                }, 
                TimeSpan.FromSeconds(1), 
                TimeSpan.FromSeconds(1), 
                TimeSpan.FromSeconds(1));

            // 2 Remove the cacheKey
            this._cache.Remove(cacheKey);

            // 3 Try to get the cached value
            var tryGetResult = this._cache.TryGet(cacheKey, out string returnedCacheValue2);
            #endregion


            #region Assert
            // tryGetResult should be false because the key wasn't found
            Assert.IsFalse(tryGetResult);

            // returnedCacheValue2 should be null
            Assert.AreEqual(default(string), returnedCacheValue2);
            #endregion
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("		")]
        public void Remove_ShouldFailWithNullOrWhiteSpaceKey(string cacheKey)
        {
            #region Arrange 
            // Nothing to do
            #endregion


            #region Act & Assert
            // GetOrAdd() should throw ArgumentException because the key is invalid.
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                this._cache.Remove(cacheKey);
            });

            Assert.AreEqual("key", ex.ParamName);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:TryGet_NonExistingKey:test-key")]
        public void TryGet_NonExistingKey(string cacheKey)
        {
            #region Arrange
            // Make sure the key we're about to test is empty
            this._cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to get the non existing key
            var tryGetResult = this._cache.TryGet(cacheKey, out string returnedCacheValue1);
            #endregion


            #region Assert
            // tryGetResult should be false because the key wasn't found
            Assert.IsFalse(tryGetResult);

            // returnedCacheValue1 should be null
            Assert.AreEqual(default(string), returnedCacheValue1);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:Expiration:test-key", "test-value-1")]
        public void Expiration(string cacheKey, string cacheValue1)
        {
            #region Arrange
            // Make sure the key we're about to test is empty
            this._cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey for 1 ms
            var returnedCacheValue1 = this._cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue1;
                },
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1));

            // 2 Wait for the cache to expired
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            // 3 Try to get the cached value
            var tryGetResult = this._cache.TryGet(cacheKey, out string returnedCacheValue2);
            #endregion


            #region Assert
            // tryGetResult should be false because the key wasn't found
            Assert.IsFalse(tryGetResult);

            // returnedCacheValue2 should be null
            Assert.AreEqual(default(string), returnedCacheValue2);
            #endregion
        }
    }
}
