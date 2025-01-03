using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Middlewares;

/// <summary>
/// 限流中间件扩展
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    /// <summary>
    /// 使用限流中间件
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseRateLimitAuthorize(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}
