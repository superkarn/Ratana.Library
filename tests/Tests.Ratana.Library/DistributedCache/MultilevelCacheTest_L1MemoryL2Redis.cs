using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Ratana.Library.DistributedCache;
using System;
using System.Text;
using System.Threading;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.DistributedCache
{
    class MultilevelCacheTest_L1MemoryL2Redis
    {
        private MultilevelCache _cache;
        
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
            var cacheL2 = new RedisCache(Options.Create(redisCacheOptions));
            
            this._cache = new MultilevelCache(
                cacheL1,
                cacheL2
            );
        }

        #region Get
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetLevel1:test-key1", "test-value-1", "test-value-2")]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetLevel1:test-key2", "", "test-value-2")]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetLevel1:test-key3", "", "")]
        public void GetLevel1(string cacheKey, string cacheValue1Str, string cacheValue2Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = Encoding.UTF8.GetBytes(cacheValue1Str);
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);

            // Set cache values
            this._cache.Caches[0].Set(cacheKey, cacheValue1);
            this._cache.Caches[1].Set(cacheKey, cacheValue2);
            #endregion


            #region Act
            // Get the value for cacheKey
            var returnedCacheValue = this._cache.Get(cacheKey);
            #endregion


            #region Assert
            // returnedCacheValue1 should equal cacheValue1
            // since it should never reached higher level caches
            Assert.AreEqual(cacheValue1, returnedCacheValue);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetLevel2:test-key1", null, "test-value-2")]
        public void GetLevel2_When_Level1_Is_Empty(string cacheKey, string cacheValue1Str, string cacheValue2Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);

            // Set up some variables, but only level 2
            this._cache.Caches[1].Set(cacheKey, cacheValue2);
            #endregion


            #region Act
            // Get the value for cacheKey
            var returnedCacheValue = this._cache.Get(cacheKey);
            #endregion


            #region Assert
            // returnedCacheValue should equal cacheValue2
            // since we set cacheL1 to null
            Assert.AreEqual(cacheValue2, returnedCacheValue);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetLevel2:test-key2")]
        public void GetLevel2_In_Redis_EmptyString_Becomes_Null(string cacheKey)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue = Encoding.UTF8.GetBytes(string.Empty);

            // Set up some variables, but only level 2
            //this._cache.Caches[0].Set(cacheKey, cacheValue1);
            this._cache.Caches[1].Set(cacheKey, cacheValue);
            #endregion


            #region Act
            // Get the value for cacheKey
            var returnedCacheValue = this._cache.Get(cacheKey);
            #endregion


            #region Assert
            // For some reason Empty String in Redis comes back as null
            Assert.IsNull(returnedCacheValue);
            #endregion
        }
        #endregion


        #region GetAsync
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetAsyncLevel1:test-key1", "test-value-1", "test-value-2")]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetAsyncLevel1:test-key2", "", "test-value-2")]
        public void GetAsyncLevel1(string cacheKey, string cacheValue1Str, string cacheValue2Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = Encoding.UTF8.GetBytes(cacheValue1Str);
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);

            // Set cache values
            this._cache.Caches[0].Set(cacheKey, cacheValue1);
            this._cache.Caches[1].Set(cacheKey, cacheValue2);
            #endregion


            #region Act
            // Get the value for cacheKey
            var task = this._cache.GetAsync(cacheKey);
            task.Wait();
            var returnedCacheValue = task.Result;
            #endregion


            #region Assert
            // returnedCacheValue1 should equal cacheValue1
            // since it should never reached higher level caches
            Assert.AreEqual(cacheValue1, returnedCacheValue);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetAsyncLevel2:test-key1", "test-value-1", "test-value-2")]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:GetAsyncLevel2:test-key2", "", "test-value-2")]
        public void GetAsyncLevel2(string cacheKey, string cacheValue1Str, string cacheValue2Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = Encoding.UTF8.GetBytes(cacheValue1Str);
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);

            // Set up some variables, but only level 2
            //this._cache.Caches[0].Set(cacheKey, cacheValue1);
            this._cache.Caches[1].Set(cacheKey, cacheValue2);
            #endregion


            #region Act
            // Get the value for cacheKey
            var task = this._cache.GetAsync(cacheKey);
            task.Wait();
            var returnedCacheValue = task.Result;
            #endregion


            #region Assert
            // returnedCacheValue1 should equal cacheValue1
            // since it should never reached higher level caches
            Assert.AreEqual(cacheValue2, returnedCacheValue);
            #endregion
        }
        #endregion


        #region Refresh
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:Refresh:test-key1")]
        public void Refresh(string cacheKey)
        {
            #region Arrange 
            // Set up some variables
            var cacheL1 = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            MultilevelCache cache = new MultilevelCache(
                cacheL1
            );
            #endregion


            #region Act
            // Call Refresh
            cache.Refresh(cacheKey);
            #endregion


            #region Assert
            // Nothing to check, just make sure there's not exceptions.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:RefreshAsync:test-key1")]
        public void RefreshAsync(string cacheKey)
        {
            #region Arrange 
            // Set up some variables
            var cacheL1 = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            MultilevelCache cache = new MultilevelCache(
                cacheL1
            );
            #endregion


            #region Act
            // Call Refresh
            var task = cache.RefreshAsync(cacheKey);
            task.Wait();
            #endregion


            #region Assert
            // Nothing to check, just make sure there's not exceptions.
            #endregion
        }
        #endregion


        #region Remove
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:Remove:test-key1")]
        public void Remove(string cacheKey)
        {
            #region Arrange 
            const string cacheValueStr = "test-value";

            // Convert string to byte[]
            byte[] cacheValue = Encoding.UTF8.GetBytes(cacheValueStr);
            #endregion


            #region Act
            // Add the key to cache
            this._cache.Set(cacheKey, cacheValue);

            // Make sure the cache is set
            byte[] returnedCacheValue = this._cache.Get(cacheKey);
            Assert.AreEqual(cacheValue, returnedCacheValue);

            // Call Remove
            this._cache.Remove(cacheKey);
            #endregion


            #region Assert
            // Make sure the key is no longer in the cache
            returnedCacheValue = this._cache.Get(cacheKey);
            Assert.IsNull(returnedCacheValue);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:RemoveAsync:test-key1")]
        public void RemoveAsync(string cacheKey)
        {
            #region Arrange 
            const string cacheValueStr = "test-value";

            // Convert string to byte[]
            byte[] cacheValue = Encoding.UTF8.GetBytes(cacheValueStr);
            #endregion


            #region Act
            // Add the key to cache
            this._cache.Set(cacheKey, cacheValue);

            // Make sure the cache is set
            byte[] returnedCacheValue = this._cache.Get(cacheKey);
            Assert.AreEqual(cacheValue, returnedCacheValue);

            // Call Remove
            var task = this._cache.RemoveAsync(cacheKey);
            task.Wait();
            #endregion


            #region Assert
            // Make sure the key is no longer in the cache
            returnedCacheValue = this._cache.Get(cacheKey);
            Assert.IsNull(returnedCacheValue);
            #endregion
        }
        #endregion


        #region Set
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:Set:test-key1", "test-value-1")]
        public void Set(string cacheKey, string cacheValueStr)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue = Encoding.UTF8.GetBytes(cacheValueStr);

            // Make sure the key we're about to test is empty
            this._cache.Remove(cacheKey);
            #endregion


            #region Act
            // Call Set
            this._cache.Set(cacheKey, cacheValue);
            #endregion


            #region Assert
            // Make sure we can get back the value we cached
            Assert.AreEqual(cacheValue, this._cache.Get(cacheKey));

            // Also make sure the inner caches also have the value
            Assert.AreEqual(cacheValue, this._cache.Caches[0].Get(cacheKey));
            Assert.AreEqual(cacheValue, this._cache.Caches[1].Get(cacheKey));
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:Set:test-key1", "")]
        public void Set_EmptyString_Does_Not_Work_As_Expected_In_Redis(string cacheKey, string cacheValueStr)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue = Encoding.UTF8.GetBytes(cacheValueStr);
            #endregion


            #region Act
            // Call Set
            this._cache.Set(cacheKey, cacheValue);
            #endregion


            #region Assert
            // Make sure we can get back the value we cached
            Assert.AreEqual(cacheValue, this._cache.Get(cacheKey));

            // Also make sure the inner caches also have the value
            Assert.AreEqual(cacheValue, this._cache.Caches[0].Get(cacheKey));

            // For some reason Empty String in Redis comes back as null
            Assert.IsNull(this._cache.Caches[1].Get(cacheKey));
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:Set_Null_Should_Throw_Exception:test-key1")]
        public void Set_Null_Should_Throw_Exception(string cacheKey)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue = null;
            #endregion


            #region Act & Assert
            // Call Set
            // Since cacheValue is null, it should throw an exception
            Assert.Throws<ArgumentNullException>(() => this._cache.Set(cacheKey, cacheValue));
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:SetAsync:test-key1", "test-value-1")]
        public void SetAsync(string cacheKey, string cacheValueStr)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue = Encoding.UTF8.GetBytes(cacheValueStr);

            // Make sure the key we're about to test is empty
            this._cache.Remove(cacheKey);
            #endregion


            #region Act
            // Call SetAsync
            var task = this._cache.SetAsync(cacheKey, cacheValue);
            task.Wait();
            #endregion


            #region Assert
            // Make sure we can get back the value we cached
            Assert.AreEqual(cacheValue, this._cache.Get(cacheKey));

            // Also make sure the inner caches also have the value
            Assert.AreEqual(cacheValue, this._cache.Caches[0].Get(cacheKey));
            Assert.AreEqual(cacheValue, this._cache.Caches[1].Get(cacheKey));
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1MemoryL2Redis:SetAsync_Null_Should_Throw_Exception:test-key1")]
        public void SetAsync_Null_Should_Throw_Exception(string cacheKey)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue = null;
            #endregion


            #region Act & Assert
            // Call SetAsync
            // Since cacheValue is null, it should throw an exception
            Assert.Throws<ArgumentNullException>(() =>
            {
                var task = this._cache.SetAsync(cacheKey, cacheValue);
                task.Wait();
            });
            #endregion
        }
        #endregion
    }
}
