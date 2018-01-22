using Moq;
using NUnit.Framework;
using RatanaLibrary.Cache;
using System;
using System.Threading;
using Tests.RatanaLibrary.Attributes;

namespace Tests.RatanaLibrary.Cache
{
    [TestFixture]
    public class MultilevelCacheTest
    {
        private readonly RedisCache.RedisSettings _redisSettings = new RedisCache.RedisSettings()
        {
            Server = "localhost"
        };

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


            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            // 2 Try to save fakeValue under cacheKey.
            // Since the key already exist, fakeValue is never reached.
            var returnedCachValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return fakeValue;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));
            #endregion


            #region Assert
            // returnedCachValue1 should equal cacheValue because the cache was empty.
            Assert.AreEqual(cacheValue, returnedCachValue1);

            // returnedCachValue2 should equal cacheValue because the cache exists, 
            // so fakeValue was never returned.
            Assert.AreEqual(cacheValue, returnedCachValue2);
            Assert.AreNotEqual(fakeValue, returnedCachValue2);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:GetOrAdd_L1InMemoryL2Redis:test-key1", "test-value", "test-fake-value")]
        public void GetOrAdd_L1InMemoryL2Redis(string cacheKey, string cacheValue, string fakeValue)
        {
            #region Arrange 
            // Set up some variables
            ICache cacheL1 = new InMemoryCache();
            ICache cacheL2 = new RedisCache(this._redisSettings);

            IMultilevelCache cache = new MultilevelCache(
                cacheL1,
                cacheL2
            );


            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));
            
            // 2 Try to save fakeValue under cacheKey.
            // Since the key already exist, fakeValue is never reached.
            var returnedCachValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return fakeValue;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            // 2.1 Get the values from cacheL1 and cacheL2
            cacheL1.TryGet(cacheKey, out string returnedCachL1Value);
            cacheL2.TryGet(cacheKey, out string returnedCachL2Value);
            #endregion


            #region Assert
            // returnedCachValue1 should equal cacheValue because the cache was empty.
            Assert.AreEqual(cacheValue, returnedCachValue1);

            // returnedCachValue2 should equal cacheValue because the cache exists, 
            // so fakeValue was never returned.
            Assert.AreEqual(cacheValue, returnedCachValue2);
            Assert.AreNotEqual(fakeValue, returnedCachValue2);

            // returnedCachL1Value adn returnedCachL2Value should match cacheValue
            Assert.AreEqual(cacheValue, returnedCachL1Value);
            Assert.AreEqual(cacheValue, returnedCachL2Value);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:GetOrAdd_L1InMemoryL2Redis_L1ExpiredButNotL2:test-key1", "test-value-1", "test-value-2")]
        public void GetOrAdd_L1InMemoryL2Redis_L1ExpiredButNotL2(string cacheKey, string cacheValue1, string cacheValue2)
        {
            #region Arrange
            // Set up some variables
            ICache cacheL1 = new InMemoryCache();
            ICache cacheL2 = new RedisCache(this._redisSettings);

            IMultilevelCache cache = new MultilevelCache(
                cacheL1,
                cacheL2
            );


            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue1;
                },
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(1));

            // 2 Wait for L1 cache to expire, but not long enough for L2 to expire.
            Thread.Sleep(TimeSpan.FromSeconds(2));

            // 2.1 Get the values from cacheL1 after it has expired
            cacheL1.TryGet(cacheKey, out string returnedCachL1Value);

            // 3 Try to save cacheValue2 under cacheKey
            var returnedCachValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue2;
                },
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(1));

            // 3.1 Get the values from cacheL2
            cacheL2.TryGet(cacheKey, out string returnedCachL2Value);
            #endregion


            #region Assert
            // returnedCachValue1 should equal cacheValue1 because the cache was empty.
            Assert.AreEqual(cacheValue1, returnedCachValue1);

            // returnedCachValue2 should equal cacheValue1 because the cache exists, 
            // so cacheValue2 was never returned.
            Assert.AreEqual(cacheValue1, returnedCachValue2);

            // returnedCachL1Value should be null because its first value had expired
            // TODO this is still broken
            //Assert.AreEqual(null, returnedCachL1Value);

            // returnedCachL2Value should match cacheValue1 because its first value hadn't expired
            Assert.AreEqual(cacheValue1, returnedCachL2Value);
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
            // Set up some variables
            var cacheL1 = new Mock<ICache>();
            var cacheL2 = new Mock<ICache>();
            var cacheL3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
            #endregion


            #region Act & Assert
            // GetOrAdd() should throw ArgumentException because the key is invalid.
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var returnedCachValue1 = cache.GetOrAdd(cacheKey, () =>
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
            // Set up some variables
            var cacheL1 = new Mock<ICache>();
            var cacheL2 = new Mock<ICache>();
            var cacheL3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );


            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue1;
                }, 
                TimeSpan.FromSeconds(1), 
                TimeSpan.FromSeconds(1), 
                TimeSpan.FromSeconds(1));

            // 2 Remove the cacheKey
            cache.Remove(cacheKey);

            // 3 Try to save cacheValue2 under cacheKey again
            var returnedCachValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue2;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));
            #endregion


            #region Assert
            // returnedCachValue2 should equal cacheValue2 (but not cacheValue1)
            // because the cache was empty the second time.
            Assert.AreNotEqual(cacheValue1, returnedCachValue2);
            Assert.AreEqual(cacheValue2, returnedCachValue2);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:Remove_L1InMemoryL2Redis:test-key", "test-value-1", "test-value-2")]
        public void Remove_L1InMemoryL2Redis(string cacheKey, string cacheValue1, string cacheValue2)
        {
            #region Arrange
            // Set up some variables
            ICache cacheL1 = new InMemoryCache();
            ICache cacheL2 = new RedisCache(this._redisSettings);

            IMultilevelCache cache = new MultilevelCache(
                cacheL1,
                cacheL2
            );


            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey.
            //   Since this key is new, the cacheValue should be saved to the cache
            //   and returned to returnedCachValue1;
            var returnedCachValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue1;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            // 2 Remove the cacheKey
            cache.Remove(cacheKey);

            // 3 Try to save cacheValue2 under cacheKey again
            var returnedCachValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue2;
                },
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            // 3.1 Get the values from cacheL1 and cacheL2
            cacheL1.TryGet(cacheKey, out string returnedCachL1Value);
            cacheL2.TryGet(cacheKey, out string returnedCachL2Value);
            #endregion


            #region Assert
            // returnedCachValue2 should equal cacheValue2 (but not cacheValue1)
            // because the cache was empty the second time.
            Assert.AreNotEqual(cacheValue1, returnedCachValue2);
            Assert.AreEqual(cacheValue2, returnedCachValue2);

            // returnedCachL1Value and returnedCachL2Value should match cacheValue2
            Assert.AreEqual(cacheValue2, returnedCachL1Value);
            Assert.AreEqual(cacheValue2, returnedCachL2Value);
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
            // Set up some variables
            var cacheL1 = new Mock<ICache>();
            var cacheL2 = new Mock<ICache>();
            var cacheL3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
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
        [TestCase("MultilevelCacheTest:TryGet_NonExistingKey:test-key")]
        public void TryGet_NonExistingKey(string cacheKey)
        {
            #region Arrange
            // Set up some variables
            var cacheL1 = new Mock<ICache>();
            var cacheL2 = new Mock<ICache>();
            var cacheL3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );

            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to get the non existing key
            var tryGetResult = cache.TryGet(cacheKey, out string returnedCachValue1);
            #endregion


            #region Assert
            // tryGetResult should be false because the key wasn't found
            Assert.IsFalse(tryGetResult);

            // returnedCachValue1 should be null
            Assert.AreEqual(default(string), returnedCachValue1);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:Expiration:test-key", "test-value-1")]
        public void Expiration(string cacheKey, string cacheValue1)
        {
            #region Arrange
            // Set up some variables
            var cacheL1 = new Mock<ICache>();
            var cacheL2 = new Mock<ICache>();
            var cacheL3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );

            // Make sure the key we're about to test is empty
            cache.Remove(cacheKey);
            #endregion


            #region Act
            // 1 Try to save cacheValue under cacheKey for 1 ms
            var returnedCachValue1 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue1;
                },
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1));

            // 2 Wait for the cache to expired
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            // 3 Try to get the cached value
            var tryGetResult = cache.TryGet(cacheKey, out string returnedCachValue2);
            #endregion


            #region Assert
            // tryGetResult should be false because the key wasn't found
            Assert.IsFalse(tryGetResult);

            // returnedCachValue2 should be null
            Assert.AreEqual(default(string), returnedCachValue2);
            #endregion
        }
    }
}
