using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Handlers;

/// <summary>
/// 限流属性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RateLimitAttribute : Attribute
{
    /// <summary>
    /// 每分钟限制次数
    /// </summary>
    public int RequestsPerMinute { get; set; }
}
