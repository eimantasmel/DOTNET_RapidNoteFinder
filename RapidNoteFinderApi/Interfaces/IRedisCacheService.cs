using RapidNoteFinderApi.Models;

namespace RapidNoteFinderApi.Interfaces
{
    public interface IRedisCacheService 
    {
        Task SetCacheValueAsync<T>(string key, IEnumerable<T> value);

        Task<IEnumerable<T>> GetCacheValueAsync<T>(string key);

        public Task DeleteCacheValueAsync(string key);
    }
}