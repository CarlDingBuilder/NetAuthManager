using NetAuthManager.Application.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Base.Stores;

public interface IMemoryCacheStore
{
    /// <summary>
    /// 异步获取
    /// </summary>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// 获取
    /// </summary>
    T Get<T>(string key);

    /// <summary>
    /// 异步设置
    /// </summary>
    Task SetAsync<T>(string key, T model);

    /// <summary>
    /// 设置
    /// </summary>
    void Set<T>(string key, T model);
}
