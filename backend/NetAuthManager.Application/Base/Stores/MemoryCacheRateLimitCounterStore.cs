using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Stores;

/// <summary>
/// 内存限制
/// </summary>
public class MemoryCacheRateLimitCounterStore : IRateLimitCounterStore, ITransient
{
    private readonly IMemoryCache _cache;

    public MemoryCacheRateLimitCounterStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 限流头部
    /// </summary>
    private const string CachePrefix = "RateLimit://";

    /// <summary>
    /// 获取限流缓存数据
    /// </summary>
    public async Task<RateLimitCounter> GetAsync(string key, CancellationToken cancellationToken)
    {
        // 模拟异步操作
        await Task.Yield();
        _cache.TryGetValue($"{CachePrefix}{key}", out RateLimitCounter counter);
        return counter;
    }

    /// <summary>
    /// 递增
    /// </summary>
    public async Task<bool> IncrementAsync(string key, CancellationToken cancellationToken)
    {
        // 模拟异步操作
        await Task.Yield();
        var counter = _cache.GetOrCreate($"{CachePrefix}{key}", entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);
            return new RateLimitCounter
            {
                Timestamp = DateTime.UtcNow,
                TotalRequests = 1
            };
        });

        if (counter.Timestamp.AddMinutes(1) >= DateTime.UtcNow)
        {
            counter.TotalRequests++;
            _cache.Set(key, counter, TimeSpan.FromMinutes(1));
            return true;
        }

        return false;
    }
}
