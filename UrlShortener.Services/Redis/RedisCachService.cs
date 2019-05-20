using System;
using StackExchange.Redis;

namespace UrlShortener.Services.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _cache;
        public RedisCacheService(string cacheConnString)
        {
            _cache = new Lazy<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(cacheConnString)).Value.GetDatabase();
        }

        public void SetCache(string key, string value)
        {
            _cache.StringSet(key, value, TimeSpan.FromDays(7));
        }

        public string GetCache(string key)
        {
            return _cache.StringGet(key).ToString();
        }
    }
}
