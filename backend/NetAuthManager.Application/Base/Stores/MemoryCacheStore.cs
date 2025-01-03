using NetAuthManager.Application.Stores;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Base.Stores;

/// <summary>
/// 内存缓存存储
/// </summary>
public class MemoryCacheStore : IMemoryCacheStore, ITransient
{
    private readonly IMemoryCache _cache;
    public MemoryCacheStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 通用存储头部
    /// </summary>
    private const string CachePrefix = "CommonCache://";

    /// <summary>
    /// 异步获取
    /// </summary>
    public async Task<T> GetAsync<T>(string key)
    {
        // 模拟异步操作
        await Task.Yield();
        _cache.TryGetValue($"{CachePrefix}{key}", out T counter);
        return counter;
    }

    /// <summary>
    /// 获取
    /// </summary>
    public T Get<T>(string key)
    {
        _cache.TryGetValue($"{CachePrefix}{key}", out T counter);
        return counter;
    }

    public async Task SetAsync<T>(string key, T model)
    {
        // 模拟异步操作
        await Task.Yield();
        _cache.Set<T>($"{CachePrefix}{key}", model);
    }

    public void Set<T>(string key, T model)
    {
        _cache.Set<T>($"{CachePrefix}{key}", model);
    }
}
