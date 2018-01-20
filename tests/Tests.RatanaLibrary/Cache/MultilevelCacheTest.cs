using Moq;
using NUnit.Framework;
using RatanaLibrary.Cache;
using System;
using Tests.RatanaLibrary.Attributes;

namespace Tests.RatanaLibrary.Common.Cache
{
    [TestFixture]
    public class MultilevelCacheTest
    {
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:GetOrAdd:test-key1", "test-value", "test-fake-value")]
        [TestCase("MultilevelCacheTest:GetOrAdd:test-key2", "", "test-fake-value")]
        public void GetOrAdd(string cacheKey, string cacheValue, string fakeValue)
        {
            #region Arrange 
            // Set up some variables
            var cacheLevel1 = new Mock<ICache>();
            var cacheLevel2 = new Mock<ICache>();
            var cacheLevel3 = new Mock<ICache>();

            // These caches' GetOrAdd() should return cacheValue
            Func<String> orAdd1 = () => cacheValue;
            cacheLevel1.Setup(x => x.GetOrAdd(cacheKey, orAdd1, TimeSpan.MaxValue)).Returns(cacheValue);
            cacheLevel2.Setup(x => x.GetOrAdd(cacheKey, orAdd1, TimeSpan.MaxValue)).Returns(cacheValue);
            cacheLevel3.Setup(x => x.GetOrAdd(cacheKey, orAdd1, TimeSpan.MaxValue)).Returns(cacheValue);

            // These caches' TryGet() should return cacheValue
            cacheLevel1.Setup(x => x.TryGet(cacheKey, out cacheValue)).Returns(true);
            cacheLevel2.Setup(x => x.TryGet(cacheKey, out cacheValue)).Returns(true);
            cacheLevel3.Setup(x => x.TryGet(cacheKey, out cacheValue)).Returns(true);

            IMultilevelCache cache = new MultilevelCache(
                cacheLevel1.Object,
                cacheLevel2.Object,
                cacheLevel3.Object
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
                TimeSpan.MaxValue,
                TimeSpan.MaxValue,
                TimeSpan.MaxValue);

            // 2 Try to save fakeValue under cacheKey.
            // Since the key already exist, fakeValue is never reached.
            var returnedCachValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return fakeValue;
                },
                TimeSpan.MaxValue,
                TimeSpan.MaxValue,
                TimeSpan.MaxValue);
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("		")]
        public void GetOrAdd_ShouldFailWithNullOrWhiteSpaceKey(string cacheKey)
        {
            #region Arrange 
            // Set up some variables
            var cacheLevel1 = new Mock<ICache>();
            var cacheLevel2 = new Mock<ICache>();
            var cacheLevel3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheLevel1.Object,
                cacheLevel2.Object,
                cacheLevel3.Object
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
                    TimeSpan.MaxValue,
                    TimeSpan.MaxValue,
                    TimeSpan.MaxValue);
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
            var cacheLevel1 = new Mock<ICache>();
            var cacheLevel2 = new Mock<ICache>();
            var cacheLevel3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheLevel1.Object,
                cacheLevel2.Object,
                cacheLevel3.Object
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
                TimeSpan.MaxValue, 
                TimeSpan.MaxValue, 
                TimeSpan.MaxValue);

            // 2 Remove the cacheKey
            cache.Remove(cacheKey);

            // 3 Try to save cacheValue2 under cacheKey again
            var returnedCachValue2 = cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue2;
                },
                TimeSpan.MaxValue,
                TimeSpan.MaxValue,
                TimeSpan.MaxValue);
            #endregion


            #region Assert
            // returnedCachValue2 should equal cacheValue2 (but not cacheValue1)
            // because the cache was empty the second time.
            Assert.AreNotEqual(cacheValue1, returnedCachValue2);
            Assert.AreEqual(cacheValue2, returnedCachValue2);
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
            var cacheLevel1 = new Mock<ICache>();
            var cacheLevel2 = new Mock<ICache>();
            var cacheLevel3 = new Mock<ICache>();

            IMultilevelCache cache = new MultilevelCache(
                cacheLevel1.Object,
                cacheLevel2.Object,
                cacheLevel3.Object
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
    }
}
