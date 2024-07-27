using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using RapidNoteFinderApi.Interfaces;
using RapidNoteFinderApi.Models;

namespace RapidNoteFinderApi.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private const int EXPIRATION_TIME = 60;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetCacheValueAsync<T>(string key, IEnumerable<T> value)
        {
            var options = new DistributedCacheEntryOptions
            {
                // Optional: Set cache entry options such as expiration
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(RedisCacheService.EXPIRATION_TIME)
            };

            string serializedValue = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedValue, options);
        }

        // Get cache for IEnumerable<T>
        public async Task<IEnumerable<T>> GetCacheValueAsync<T>(string key)
        {
            string serializedValue = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(serializedValue))
            {
                return null; // or throw an exception or return an empty list
            }

            return JsonSerializer.Deserialize<IEnumerable<T>>(serializedValue);
        }

        public async Task DeleteCacheValueAsync(string key)
        {
             await _cache.RemoveAsync(key);
        }
    }
}