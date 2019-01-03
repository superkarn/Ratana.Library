using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using Ratana.Library.DistributedCache;
using System;
using System.Text;
using System.Threading;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.DistributedCache
{
    [TestFixture]
    public class MultilevelCacheTest
    {
        private MultilevelCache _cache;

        [SetUp]
        public void Initialize()
        {
            // Set up some variables
            var cacheL1 = new Mock<IDistributedCache>();
            var cacheL2 = new Mock<IDistributedCache>();
            var cacheL3 = new Mock<IDistributedCache>();

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
        }

        #region Get
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:GetLevel1:test-key1", "test-value-1", "test-value-2", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetLevel1:test-key2", "", "test-value-2", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetLevel1:test-key3", "", "", "")]
        public void GetLevel1(string cacheKey, string cacheValue1Str, string cacheValue2Str, string cacheValue3Str)
        {
            #region Arrange 
            // Can't use the predefined MultilevelCache in Initialized() because we need to mock the internal caches.

            // Convert string to byte[]
            byte[] cacheValue1 = Encoding.UTF8.GetBytes(cacheValue1Str);
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);
            byte[] cacheValue3 = Encoding.UTF8.GetBytes(cacheValue3Str);

            // Set up some variables
            var cacheL1 = new Mock<IDistributedCache>();
            var cacheL2 = new Mock<IDistributedCache>();
            var cacheL3 = new Mock<IDistributedCache>();

            // Mock Get()
            cacheL1.Setup(x => x.Get(cacheKey)).Returns(cacheValue1);
            cacheL2.Setup(x => x.Get(cacheKey)).Returns(cacheValue2);
            cacheL3.Setup(x => x.Get(cacheKey)).Returns(cacheValue3);

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
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
        [TestCase("MultilevelCacheTest:GetLevel2:test-key1", null, "test-value-2", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetLevel2:test-key2", null, "", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetLevel2:test-key3", null, "", "")]
        public void GetLevel2(string cacheKey, string cacheValue1Str, string cacheValue2Str, string cacheValue3Str)
        {
            #region Arrange 
            // Can't use the predefined MultilevelCache in Initialized() because we need to mock the internal caches.

            // Convert string to byte[]
            byte[] cacheValue1 = null;
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);
            byte[] cacheValue3 = Encoding.UTF8.GetBytes(cacheValue3Str);

            // Set up some variables
            var cacheL1 = new Mock<IDistributedCache>();
            var cacheL2 = new Mock<IDistributedCache>();
            var cacheL3 = new Mock<IDistributedCache>();

            // Mock Get()
            cacheL1.Setup(x => x.Get(cacheKey)).Returns(cacheValue1);
            cacheL2.Setup(x => x.Get(cacheKey)).Returns(cacheValue2);
            cacheL3.Setup(x => x.Get(cacheKey)).Returns(cacheValue3);

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
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
        [TestCase("MultilevelCacheTest:GetLevel3:test-key1", null, null, "test-value-3")]
        [TestCase("MultilevelCacheTest:GetLevel3:test-key2", null, null, "")]
        public void GetLevel3(string cacheKey, string cacheValue1Str, string cacheValue2Str, string cacheValue3Str)
        {
            #region Arrange 
            // Can't use the predefined MultilevelCache in Initialized() because we need to mock the internal caches.

            // Convert string to byte[]
            byte[] cacheValue1 = null;
            byte[] cacheValue2 = null;
            byte[] cacheValue3 = Encoding.UTF8.GetBytes(cacheValue3Str);

            // Set up some variables
            var cacheL1 = new Mock<IDistributedCache>();
            var cacheL2 = new Mock<IDistributedCache>();
            var cacheL3 = new Mock<IDistributedCache>();

            // Mock Get()
            cacheL1.Setup(x => x.Get(cacheKey)).Returns(cacheValue1);
            cacheL2.Setup(x => x.Get(cacheKey)).Returns(cacheValue2);
            cacheL3.Setup(x => x.Get(cacheKey)).Returns(cacheValue3);

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
            #endregion


            #region Act
            // Get the value for cacheKey
            var returnedCacheValue = this._cache.Get(cacheKey);
            #endregion


            #region Assert
            // returnedCacheValue should equal cacheValue3
            // since we set cacheL1 and cacheL2 to null
            Assert.AreEqual(cacheValue3, returnedCacheValue);
            #endregion
        }
        #endregion


        #region GetAsync
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:GetAsyncLevel1:test-key1", "test-value-1", "test-value-2", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetAsyncLevel1:test-key2", "", "test-value-2", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetAsyncLevel1:test-key3", "", "", "")]
        public void GetAsyncLevel1(string cacheKey, string cacheValue1Str, string cacheValue2Str, string cacheValue3Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = Encoding.UTF8.GetBytes(cacheValue1Str);
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);
            byte[] cacheValue3 = Encoding.UTF8.GetBytes(cacheValue3Str);

            // Set up some variables
            var cacheL1 = new Mock<IDistributedCache>();
            var cacheL2 = new Mock<IDistributedCache>();
            var cacheL3 = new Mock<IDistributedCache>();

            // Mock GetAsync()
            cacheL1.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue1);
            cacheL2.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue2);
            cacheL3.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue3);

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
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
        [TestCase("MultilevelCacheTest:GetAsyncLevel2:test-key1", null, "test-value-2", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetAsyncLevel2:test-key2", null, "", "test-value-3")]
        [TestCase("MultilevelCacheTest:GetAsyncLevel2:test-key3", null, "", "")]
        public void GetAsyncLevel2(string cacheKey, string cacheValue1Str, string cacheValue2Str, string cacheValue3Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = null;
            byte[] cacheValue2 = Encoding.UTF8.GetBytes(cacheValue2Str);
            byte[] cacheValue3 = Encoding.UTF8.GetBytes(cacheValue3Str);

            // Set up some variables
            var cacheL1 = new Mock<IDistributedCache>();
            var cacheL2 = new Mock<IDistributedCache>();
            var cacheL3 = new Mock<IDistributedCache>();

            // Mock GetAsync()
            cacheL1.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue1);
            cacheL2.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue2);
            cacheL3.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue3);

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
            #endregion


            #region Act
            // Get the value for cacheKey
            var task = this._cache.GetAsync(cacheKey);
            task.Wait();
            var returnedCacheValue = task.Result;
            #endregion


            #region Assert
            // returnedCacheValue should equal cacheValue2
            // since we set cacheL1 to null
            Assert.AreEqual(cacheValue2, returnedCacheValue);
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:GetAsyncLevel3:test-key1", null, null, "test-value-3")]
        [TestCase("MultilevelCacheTest:GetAsyncLevel3:test-key2", null, null, "")]
        public void GetAsyncLevel3(string cacheKey, string cacheValue1Str, string cacheValue2Str, string cacheValue3Str)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue1 = null;
            byte[] cacheValue2 = null;
            byte[] cacheValue3 = Encoding.UTF8.GetBytes(cacheValue3Str);

            // Set up some variables
            var cacheL1 = new Mock<IDistributedCache>();
            var cacheL2 = new Mock<IDistributedCache>();
            var cacheL3 = new Mock<IDistributedCache>();

            // Mock GetAsync()
            cacheL1.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue1);
            cacheL2.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue2);
            cacheL3.Setup(x => x.GetAsync(cacheKey, default(CancellationToken))).ReturnsAsync(cacheValue3);

            this._cache = new MultilevelCache(
                cacheL1.Object,
                cacheL2.Object,
                cacheL3.Object
            );
            #endregion


            #region Act
            // Get the value for cacheKey
            var task = this._cache.GetAsync(cacheKey);
            task.Wait();
            var returnedCacheValue = task.Result;
            #endregion


            #region Assert
            // returnedCacheValue should equal cacheValue3
            // since we set cacheL1 and cacheL2 to null
            Assert.AreEqual(cacheValue3, returnedCacheValue);
            #endregion
        }
        #endregion


        #region Refresh
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:Refresh:test-key1")]
        public void Refresh(string cacheKey)
        {
            #region Arrange 
            // Nothing to do
            #endregion


            #region Act
            // Call Refresh
            this._cache.Refresh(cacheKey);
            #endregion


            #region Assert
            // Nothing to check, just make sure there's not exceptions.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:RefreshAsync:test-key1")]
        public void RefreshAsync(string cacheKey)
        {
            #region Arrange 
            // Nothing to do
            #endregion


            #region Act
            // Call Refresh
            var task = this._cache.RefreshAsync(cacheKey);
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
        [TestCase("MultilevelCacheTest:Remove:test-key1")]
        public void Remove(string cacheKey)
        {
            #region Arrange 
            // Nothing to do
            #endregion


            #region Act
            // Call Remove
            this._cache.Remove(cacheKey);
            #endregion


            #region Assert
            // Nothing to check, since it's not a real cache
            // just make sure there's not exceptions.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:RemoveAsync:test-key1")]
        public void RemoveAsync(string cacheKey)
        {
            #region Arrange 
            // Nothing to do 
            #endregion


            #region Act
            // Call RemoveAsync
            var task = this._cache.RemoveAsync(cacheKey);
            task.Wait();
            #endregion


            #region Assert
            // Nothing to check, since it's not a real cache
            // just make sure there's not exceptions.
            #endregion
        }
        #endregion


        #region Set
        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:Set:test-key1", "test-value-1")]
        [TestCase("MultilevelCacheTest:Set:test-key1", "")]
        public void Set(string cacheKey, string cacheValueStr)
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
            // Nothing to check, since it's not a real cache
            // just make sure there's not exceptions.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:Set_Null_Should_Throw_Exception:test-key1")]
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
        [TestCase("MultilevelCacheTest:SetAsync:test-key1", "test-value-1")]
        [TestCase("MultilevelCacheTest:SetAsync:test-key1", "")]
        public void SetAsync(string cacheKey, string cacheValueStr)
        {
            #region Arrange 
            // Convert string to byte[]
            byte[] cacheValue = Encoding.UTF8.GetBytes(cacheValueStr);
            #endregion


            #region Act
            // Call SetAsync
            var task = this._cache.SetAsync(cacheKey, cacheValue);
            task.Wait();
            #endregion


            #region Assert
            // Nothing to check, since it's not a real cache
            // just make sure there's not exceptions.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        [TestCase("MultilevelCacheTest:SetAsync_Null_Should_Throw_Exception:test-key1")]
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
