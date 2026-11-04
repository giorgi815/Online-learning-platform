using Microsoft.Extensions.Caching.Distributed;
using OLP.Application.Contracts.Services;
using System.Text.Json;

namespace OLP.Application.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public RedisCacheService(IDistributedCache cache) => _cache = cache;

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);
            return json is null ? default : JsonSerializer.Deserialize<T>(json);
        }

        public Task RemoveAsync(string key) => _cache.RemoveAsync(key);

        public Task SetAsync<T>(string key, T value, TimeSpan lifetime)
            => _cache.SetStringAsync(key, JsonSerializer.Serialize(value),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = lifetime });
    }
}
