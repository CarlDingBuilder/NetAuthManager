using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Stores;

/// <summary>
/// 限流记数Store
/// </summary>
public interface IRateLimitCounterStore
{
    /// <summary>
    /// 获取限流缓存数据
    /// </summary>
    Task<RateLimitCounter> GetAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// 递增
    /// </summary>
    Task<bool> IncrementAsync(string key, CancellationToken cancellationToken);
}
