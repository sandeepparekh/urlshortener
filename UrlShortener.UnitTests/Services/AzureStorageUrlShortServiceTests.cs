using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using UrlShortener.Models.Azure;
using UrlShortener.Models.Common;
using UrlShortener.Repositories;
using UrlShortener.Services;
using UrlShortener.Services.Azure;
using Xunit;

namespace UrlShortener.UnitTests.Services
{
    public class AzureStorageUrlShortServiceTests
    {
        private IAppSettings _settings;
        private readonly string _alphabets = "mVRrEAkqZ4dDisTbOlGvgCUI56KnXFH37xcjaLoJ0M8Nt9fYehuWzSPwBQ1yp2";

        public AzureStorageUrlShortServiceTests()
        {
            var mockSettings = new Mock<IAppSettings>();
            mockSettings.Setup(s => s.EncodingAlphabet).Returns(_alphabets);
            mockSettings.Setup(s => s.ShortUrlCodeLength).Returns(6);
            _settings = mockSettings.Object;
        }

        [Theory]
        [InlineData("https://localhost/dfedb3af-16c7-4aca-b7e8-3c932a0dbf1f", "TvZzsr", "anon")]
        [InlineData("https://localhost/02205dd4-1c99-4f61-9f39-f53617482f31", "svxLiU", "john")]
        [InlineData("https://localhost/6a800413-b3e5-43eb-9b56-d4663ad2dae1", "sr5BiP", "doe")]
        public async void CreateUrlTest_WhenEntryDoesNotExists(string longUrl, string shortUrlCode, string userId)
        {
            var mockRepo = new Mock<IUrlRepository>();
            var mockLogger = new Mock<ILogger<AzureStorageUrlShortService>>();
            mockRepo.Setup(r => r.GetRedirectOptimizedUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<Url>(null));
            mockRepo.Setup(r => r.CreateUrl(It.IsAny<Url>(), It.IsAny<Url>()))
                .Returns(Task.FromResult(true));

            var service = new AzureStorageUrlShortService(_settings, mockRepo.Object, new Mock<ICacheService>().Object, mockLogger.Object);
            var result = await service.CreateUrl(longUrl, userId);
            Assert.Equal(shortUrlCode, result?.Data);
        }

        [Theory]
        [InlineData("https://localhost/dfedb3af-16c7-4aca-b7e8-3c932a0dbf1f", "TvZzsr", "anon")]
        [InlineData("https://localhost/02205dd4-1c99-4f61-9f39-f53617482f31", "svxLiU", "john")]
        [InlineData("https://localhost/6a800413-b3e5-43eb-9b56-d4663ad2dae1", "sr5BiP", "doe")]
        public async void CreateUrlTest_WhenEntryAlreadyExistsForSameLongUrl(string longUrl, string shortUrlCode, string userId)
        {
            var mockRepo = new Mock<IUrlRepository>();
            var mockLogger = new Mock<ILogger<AzureStorageUrlShortService>>();
            mockRepo.Setup(r => r.GetRedirectOptimizedUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Url("", "", longUrl)));

            var service = new AzureStorageUrlShortService(_settings, mockRepo.Object, new Mock<ICacheService>().Object, mockLogger.Object);
            var result = await service.CreateUrl(longUrl, userId);
            Assert.Equal(shortUrlCode, result?.Data);
        }

        [Theory]
        [InlineData("https://localhost/dfedb3af-16c7-4aca-b7e8-3c932a0dbf1f", "au5W5P", "anon")]
        public async void CreateUrlTest_WhenEntryAlreadyExistsForAnotherLongUrl(string longUrl, string shortUrlCode, string userId)
        {
            var mockRepo = new Mock<IUrlRepository>();
            var mockLogger = new Mock<ILogger<AzureStorageUrlShortService>>();
            mockRepo.SetupSequence(r => r.GetRedirectOptimizedUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult<Url>(null));
            mockRepo.Setup(r => r.CreateUrl(It.IsAny<Url>(), It.IsAny<Url>()))
                .Returns(Task.FromResult(true));

            var service = new AzureStorageUrlShortService(_settings, mockRepo.Object, new Mock<ICacheService>().Object, mockLogger.Object);
            var result = await service.CreateUrl(longUrl, userId);
            Assert.Equal(shortUrlCode, result?.Data);
        }

        [Theory]
        [InlineData("https://localhost/dfedb3af-16c7-4aca-b7e8-3c932a0dbf1f", "Unable to generate a short URL code", "anon")]
        public async void CreateUrlTest_WhenEntryAlreadyExistsForAnotherLongUrl_AndAllReTriesFail(string longUrl, string errorMessage, string userId)
        {
            var mockRepo = new Mock<IUrlRepository>();
            var mockLogger = new Mock<ILogger<AzureStorageUrlShortService>>();
            mockRepo.SetupSequence(r => r.GetRedirectOptimizedUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())))
                .Returns(Task.FromResult(new Url("", "", Guid.NewGuid().ToString())));

            var service = new AzureStorageUrlShortService(_settings, mockRepo.Object, new Mock<ICacheService>().Object, mockLogger.Object);
            var result = await service.CreateUrl(longUrl, userId);
            Assert.Equal(false, result?.Success);
            Assert.Equal(errorMessage, result?.Error);
        }

        [Theory]
        [InlineData("https://localhost/dfedb3af-16c7-4aca-b7e8-3c932a0dbf1f", "TvZzsr", "anon")]
        public async void CreateUrlTest_ThrowsException(string longUrl, string shortUrlCode, string userId)
        {
            string exMsg = "Test Exception";
            var mockRepo = new Mock<IUrlRepository>();
            var mockLogger = new Mock<ILogger<AzureStorageUrlShortService>>();
            mockRepo.Setup(r => r.GetRedirectOptimizedUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception(exMsg));

            var service = new AzureStorageUrlShortService(_settings, mockRepo.Object, new Mock<ICacheService>().Object, mockLogger.Object);
            var result = await service.CreateUrl(longUrl, userId);
            Assert.Equal(exMsg, result?.Error);
        }

        [Fact]
        public async void GetLongUrl_CacheNoneCacheTest()
        {
            var mockRepo = new Mock<IUrlRepository>();
            var mockLogger = new Mock<ILogger<AzureStorageUrlShortService>>();
            var mockCache = new Mock<ICacheService>();
            var service = new AzureStorageUrlShortService(_settings, mockRepo.Object, mockCache.Object, mockLogger.Object);

            var shortUrlCode = "aBcDeF";
            var pk = "aBc";
            var longUrl = "https://example.com";

            mockCache.SetupSequence(c => c.GetCache(shortUrlCode))
                .Returns(longUrl)
                .Returns(string.Empty);

            // get value from cache
            Assert.Equal(longUrl, service.GetLongUrl(shortUrlCode).Result.Data);

            // check if cache set and repo called
            mockRepo.Setup(r => r.GetRedirectOptimizedUrl(pk, shortUrlCode))
                .Returns(Task.FromResult(new Url(pk, shortUrlCode, longUrl)));
            await service.GetLongUrl(shortUrlCode);
            mockRepo.Verify(v => v.GetRedirectOptimizedUrl(pk, shortUrlCode), Times.Once);
            mockCache.Verify(c => c.SetCache(shortUrlCode, longUrl), Times.Once);
        }
    }
}