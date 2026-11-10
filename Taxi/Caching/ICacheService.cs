namespace Taxi.Caching;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
    void Remove(params string[] keys);
    void Clear();
}