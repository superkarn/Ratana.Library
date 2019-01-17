using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Ratana.Library.Cache;
using System;
using System.Threading;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.Cache
{
    [TestFixture]
    public class MultilevelDistributedCacheTest_L1MemoryL2Redis
    {
        private IMultilevelCache _cache;

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

            RedisCacheOptions redisCacheOptions = new RedisCacheOptions();
            redisCacheOptions.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions();
            redisCacheOptions.ConfigurationOptions.EndPoints.Add(host, port);

            var cacheL1 = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheL2 = new Microsoft.Extensions.Caching.Redis.RedisCache(Options.Create(redisCacheOptions));



            this._cache = new MultilevelDistributedCache(new global::Ratana.Library.DistributedCache.MultilevelCache(
                cacheL1,
                cacheL2
            ));
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1InMemoryL2Redis:GetOrAdd:test-key1", "test-value", "test-fake-value")]
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
                },
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));

            // 2 Try to save fakeValue under cacheKey.
            // Since the key already exist, fakeValue is never reached.
            var returnedCacheValue2 = this._cache.GetOrAdd(cacheKey, () =>
                {
                    return fakeValue;
                },
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));

            // 2.1 Get the values from cacheL1 and cacheL2
            var returnedCacheL1Value = JsonConvert.DeserializeObject<string>(
                (this._cache as MultilevelDistributedCache)
                .Cache
                .Caches[0]
                .GetString(cacheKey)
            );

            var returnedCacheL2Value = JsonConvert.DeserializeObject<string>(
                (this._cache as MultilevelDistributedCache)
                .Cache
                .Caches[1]
                .GetString(cacheKey)
            );
            #endregion


            #region Assert
            // returnedCacheValue1 should equal cacheValue because the cache was empty.
            Assert.AreEqual(cacheValue, returnedCacheValue1);

            // returnedCacheValue2 should equal cacheValue because the cache exists, 
            // so fakeValue was never returned.
            Assert.AreEqual(cacheValue, returnedCacheValue2);

            // returnedCacheL1Value and returnedCacheL2Value should match cacheValue
            Assert.AreEqual(cacheValue, returnedCacheL1Value);
            Assert.AreEqual(cacheValue, returnedCacheL2Value);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1InMemoryL2Redis:GetOrAdd_L1ExpiredButNotL2:test-key1", "test-value-1", "test-value-2")]
        public void GetOrAdd_L1ExpiredButNotL2(string cacheKey, string cacheValue1, string cacheValue2)
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
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMinutes(1));

            // 2 Wait for L1 cache to expire, but not long enough for L2 to expire.
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            // 2.1 Get the values from cacheL1 after it has expired
            var returnedCacheL1Value = (this._cache as MultilevelDistributedCache)
                .Cache
                .Caches[0]
                .GetString(cacheKey);

            // 3 Try to save cacheValue2 under cacheKey
            var returnedCacheValue2 = this._cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue2;
                },
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMinutes(1));

            // 3.1 Get the values from cacheL2
            var returnedCacheL2Value = JsonConvert.DeserializeObject<string>(
                (this._cache as MultilevelDistributedCache)
                .Cache
                .Caches[1]
                .GetString(cacheKey)
            );
            #endregion


            #region Assert
            // returnedCacheValue1 should equal cacheValue1 because the cache was empty.
            Assert.AreEqual(cacheValue1, returnedCacheValue1);

            // returnedCacheValue2 should equal cacheValue1 because the cache exists, 
            // so cacheValue2 was never returned.
            Assert.AreEqual(cacheValue1, returnedCacheValue2);

            // returnedCacheL1Value should be null because its first value had expired
            Assert.IsNull(returnedCacheL1Value);

            // returnedCacheL2Value should match cacheValue1 because its first value hadn't expired
            Assert.AreEqual(cacheValue1, returnedCacheL2Value);
            #endregion
        }
               
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1InMemoryL2Redis:Remove:test-key", "test-value-1", "test-value-2")]
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
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));

            // 2 Remove the cacheKey
            this._cache.Remove(cacheKey);

            // 3 Try to save cacheValue2 under cacheKey again
            var returnedCacheValue2 = this._cache.GetOrAdd(cacheKey, () =>
                {
                    return cacheValue2;
                },
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));

            // 3.1 Get the values from cacheL1 and cacheL2
            var returnedCacheL1Value = JsonConvert.DeserializeObject<string>(
                (this._cache as MultilevelDistributedCache)
                .Cache
                .Caches[0]
                .GetString(cacheKey)
            );

            var returnedCacheL2Value = JsonConvert.DeserializeObject<string>(
                (this._cache as MultilevelDistributedCache)
                .Cache
                .Caches[1]
                .GetString(cacheKey)
            );
            #endregion


            #region Assert
            // returnedCacheValue2 should equal cacheValue2 (but not cacheValue1)
            // because the cache was empty the second time.
            Assert.AreNotEqual(cacheValue1, returnedCacheValue2);
            Assert.AreEqual(cacheValue2, returnedCacheValue2);

            // returnedCacheL1Value and returnedCacheL2Value should match cacheValue2
            Assert.AreEqual(cacheValue2, returnedCacheL1Value);
            Assert.AreEqual(cacheValue2, returnedCacheL2Value);
            #endregion
        }
        
    }
}
