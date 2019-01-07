using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
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
    class MultilevelCacheTest_L1Memory
    {
        private MultilevelCache _cache;

        [SetUp]
        public void Initialize()
        {
            // Set up some variables
            var cacheL1 = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            this._cache = new MultilevelCache(
                cacheL1
            );
        }

        #region Get
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1Memory:GetLevel1:test-key1", "test-value-1")]
        [TestCase("MultilevelCacheTest_L1Memory:GetLevel1:test-key2", "")]
        public void GetLevel1(string cacheKey, string cacheValue1Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = Encoding.UTF8.GetBytes(cacheValue1Str);

            this._cache.Caches[0].Set(cacheKey, cacheValue1);
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
        #endregion


        #region GetAsync
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1Memory:GetAsyncLevel1:test-key1", "test-value-1")]
        [TestCase("MultilevelCacheTest_L1Memory:GetAsyncLevel1:test-key2", "")]
        public void GetAsyncLevel1(string cacheKey, string cacheValue1Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = Encoding.UTF8.GetBytes(cacheValue1Str);

            this._cache.Caches[0].Set(cacheKey, cacheValue1);
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
        #endregion


        #region Refresh
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1Memory:Refresh:test-key1")]
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
        [TestCase("MultilevelCacheTest_L1Memory:RefreshAsync:test-key1")]
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
        [TestCase("MultilevelCacheTest_L1Memory:Remove:test-key1")]
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
        [TestCase("MultilevelCacheTest_L1Memory:RemoveAsync:test-key1")]
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

            // Call RemoveAsync
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
        [TestCase("MultilevelCacheTest_L1Memory:Set:test-key1", "test-value-1")]
        [TestCase("MultilevelCacheTest_L1Memory:Set:test-key1", "")]
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
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1Memory:Set_Null_Should_Throw_Exception:test-key1")]
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
        [TestCase("MultilevelCacheTest_L1Memory:SetAsync:test-key1", "test-value-1")]
        [TestCase("MultilevelCacheTest_L1Memory:SetAsync:test-key1", "")]
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
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest_L1Memory:SetAsync_Null_Should_Throw_Exception:test-key1")]
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
