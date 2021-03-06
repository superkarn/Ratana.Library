﻿using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Ratana.Library.Cache;
using System;
using System.Threading;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.Cache
{
    [TestFixture]
    public class InMemoryCacheTest
    {
        private ICache _cache;

        [SetUp]
        public void Initialize()
        {
            this._cache = new InMemoryCache();
        }

        [Test]
        [Continuous, Integration]
        [TestCase("InMemoryCacheTest:GetOrAdd:test-key1", "test-value", "test-fake-value")]
        [TestCase("InMemoryCacheTest:GetOrAdd:test-key2", "", "test-fake-value")]
        public void GetOrAdd(string cacheKey, string cacheValue, string fakeValue)
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
                    return cacheValue;
                });

            // 2 Try to save fakeValue under cacheKey.
            // Since the key already exist, fakeValue is never reached.
            var returnedCacheValue2 = this._cache.GetOrAdd(cacheKey, () =>
                {
                    return fakeValue;
                });
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
        [Continuous, Integration]
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
                });
            });

            Assert.AreEqual("key", ex.ParamName);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("InMemoryCacheTest:Remove:test-key", "test-value-1", "test-value-2")]
        [TestCase("InMemoryCacheTest:Remove:test-key", "", "test-value-2")]
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
            });

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
        [Continuous, Integration]
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
        [TestCase("InMemoryCacheTest:TryGet_NonExistingKey:test-key")]
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
        [TestCase("InMemoryCacheTest:Expiration:test-key", "test-value-1")]
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
            }, TimeSpan.FromMilliseconds(1));

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
