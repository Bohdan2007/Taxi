using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Taxi.Logging;
using Taxi.Options;

namespace Taxi.Caching;

public class CacheService : ICacheService
{
    private readonly IMemoryCache cache;
    private readonly IOptionsMonitor<CachingOptions> options;
    private readonly ILogger<CacheService> logger;
    private CancellationTokenSource resetTokenSource = new();
    private readonly object resetLock = new();

    public CacheService(IMemoryCache cache, IOptionsMonitor<CachingOptions> options, ILogger<CacheService> logger)
    {
        this.cache = cache;
        this.options = options;
        this.logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
    {
        var settings = options.CurrentValue;

        if (!settings.Enabled)
        {
            logger.LogInformation(LogEvents.CacheDisabled, "[Кеш] Вимкнено в конфігурації — {Key} завантажується з БД", key);
            return await factory();
        }

        if (cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null)
        {
            logger.LogInformation(LogEvents.CacheHit, "[Кеш] Влучання: {Key}", key);
            return cachedValue;
        }

        logger.LogInformation(LogEvents.CacheMiss, "[Кеш] Промах: {Key} — завантаження з БД", key);
        var value = await factory();

        var entryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(settings.DurationSeconds))
            .AddExpirationToken(new CancellationChangeToken(resetTokenSource.Token));

        cache.Set(key, value, entryOptions);
        logger.LogInformation(LogEvents.CacheStored, "[Кеш] Збережено: {Key} на {Seconds} с", key, settings.DurationSeconds);

        return value;
    }

    public void Remove(params string[] keys)
    {
        var distinctKeys = keys.Distinct().ToArray();
        foreach (var key in distinctKeys)
        {
            cache.Remove(key);
        }

        logger.LogInformation(LogEvents.CacheRemoved, "[Кеш] Скинуто: {Keys}", string.Join(", ", distinctKeys));
    }

    public void Clear()
    {
        CancellationTokenSource oldTokenSource;
        lock (resetLock)
        {
            oldTokenSource = resetTokenSource;
            resetTokenSource = new CancellationTokenSource();
        }

        oldTokenSource.Cancel();
        logger.LogInformation(LogEvents.CacheCleared, "[Кеш] Повністю очищено");
    }
}
