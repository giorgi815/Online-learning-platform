
namespace OLP.Application.Contracts.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan lifetime);
        Task RemoveAsync(string key);
    }
}
