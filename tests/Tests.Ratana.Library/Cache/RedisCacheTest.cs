using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Ratana.Library.Cache;
using System;
using System.Threading;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.Cache
{
    [TestFixture]
    public class RedisCacheTest
    {
        private ICache _cache;

        [SetUp]
        public void Initialize()
        {
            // Get environment name
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Get Redis info from config
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .Build();

            // Default Redis host:port is localhost:6379
            var host = string.IsNullOrWhiteSpace(config["redis:host"]) ? "localhost" : config["redis:host"];
            int port = 6379;
            try { port = int.Parse(config["redis:port"]); } catch { }
            
            var redisSettings = new RedisCache.RedisSettings()
            {
                Server = host,
                Port = port
            };

            this._cache = new RedisCache(redisSettings);
        }

        [Test]
        [Continuous, Integration]
        [TestCase("RedisCacheTest:GetOrAdd:test-key1", "test-value", "test-fake-value")]
        [TestCase("RedisCacheTest:GetOrAdd:test-key2", "", "test-fake-value")]
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
        public void GetOrAdd_AnonymousType()
        {
            #region Arrange
            // Set up some variables
            var cacheKey = "RedisCacheTest:GetOrAddAnonymousType:test-key";
            var cacheValue = new { Name = "test-name", Value = "test-value" };
            var fakeValue = new { Name = "test-fake-name", Value = "test-fake-value" };

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
            var returnedCacheValue2 = this._cache.GetOrAdd(cacheKey, () =>
            {
                return fakeValue;
            });
            #endregion
            
            #region Assert
            // returnedCacheValue1 should equal cacheValue because the cache is empty.
            Assert.AreEqual(cacheValue, returnedCacheValue1);

            // returnedCacheValue2 should equal cacheValue because the cache exists, 
            // so fakeValue was never returned.
            Assert.AreEqual(cacheValue, returnedCacheValue2);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("RedisCacheTest:Remove:test-key", "test-value-1", "test-value-2")]
        [TestCase("RedisCacheTest:Remove:test-key", "", "test-value-2")]
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

            // 3 Try to get the non existing key
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
        [TestCase("RedisCacheTest:TryGet_NonExistingKey:test-key")]
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
        [TestCase("RedisCacheTest:Expiration:test-key", "test-value-1")]
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
