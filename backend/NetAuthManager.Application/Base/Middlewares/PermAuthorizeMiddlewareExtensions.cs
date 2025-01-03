using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Middlewares;

/// <summary>
/// 权限中间件扩展
/// </summary>
public static class PermAuthorizeMiddlewareExtensions
{
    /// <summary>
    /// 使用权限中间件
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UsePermAuthorize(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PermAuthorizeMiddleware>();
    }
}
