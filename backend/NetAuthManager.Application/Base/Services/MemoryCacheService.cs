using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 内存存储服务
/// </summary>
public class MemoryCacheService : ICacheService, ISingleton
{
    #region 注入与构造

    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    #endregion 注入与构造

    /// <summary>
    /// 设置值对象
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add<V>(string key, V value)
    {
        this.Add(key, value, 7200);
    }

    /// <summary>
    /// 设置值对象
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cacheDurationInSeconds">缓存持续秒数</param>
    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        _memoryCache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheDurationInSeconds)
        });
        //var entry = _memoryCache.CreateEntry(key);
        //entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheDurationInSeconds);
        //entry.SetValue(value);
    }

    /// <summary>
    /// 获取值对象
    /// </summary>
    public V Get<V>(string key)
    {
        object value;
        if (_memoryCache.TryGetValue(key, out value))
        {
            return (V)value;
        }
        return default;
    }

    /// <summary>
    /// 是否包含值
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey<V>(string key)
    {
        return _memoryCache.TryGetValue(key, out object value);
    }

    IEnumerable<string> ICacheService.GetAllKey<V>()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取或创建
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="create"></param>
    /// <param name="cacheDurationInSeconds"></param>
    /// <returns></returns>
    public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds)
    {
        return _memoryCache.GetOrCreate(cacheKey, entry =>
        {
            if (entry == null)
            {
                if (create == null)
                {
                    var value = create();
                    Add(cacheKey, value);
                    return value;
                }
                else return default;
            }
            return (V)entry.Value;
        });
    }

    /// <summary>
    /// 移除键值
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    public void Remove<V>(string key)
    {
        _memoryCache.Remove(key);
    }
}
