using NetAuthManager.Application.Filters;
using NetAuthManager.Application.Handlers;
using NetAuthManager.Application.Handlers.Models;
using NetAuthManager.Application.Results.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Middlewares;

/// <summary>
/// 授权中间件
/// </summary>
public class PermAuthorizeMiddleware// : IMiddleware
{
    //private readonly IConfiguration _configuration;
    private readonly RequestDelegate _next;
    private readonly ILogger<BaseExceptionFilter> _logger;

    public PermAuthorizeMiddleware(RequestDelegate next, ILogger<BaseExceptionFilter> logger)
    {
        //IConfiguration configuration, 
        //this._configuration = configuration;
        this._next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)//, RequestDelegate next)
    {
        var permAttribute = context.GetEndpoint()?.Metadata?.GetMetadata<PermAuthorizeAttribute>();
        if (permAttribute != null)
        {
            try
            {
                var permsAuthItems = permAttribute.PermsAuthItems;

                // 无校验放行
                if (permsAuthItems.Count > 0)
                {
                    // 获取校验权限
                    var canOpt = false;
                    var menuResourceService = context.RequestServices.GetService<IMenuResourceService>();
                    foreach (var perm in permsAuthItems)
                    {
                        //无资源路径跳过
                        if (string.IsNullOrEmpty(perm.PermFullPath)) continue;

                        //资源全路径
                        var rsid = await menuResourceService.GetRSID(perm.PermFullPath);

                        // 判断操作权限
                        canOpt = canOpt || await menuResourceService.CheckPermision(rsid, perm.PermName);
                        if (canOpt) break;
                    }

                    // 校验权限
                    if (canOpt)
                    {
                        await _next(context);
                    }
                    else
                    {
                        // 无权限
                        throw new UnauthorizedAccessException("您没有进行该操作的权限！");
                    }
                }
                else
                {
                    await _next(context);
                }

                //var permissionSettings = configuration.GetSection("PermissionSettings").Get();
                //var permission = permissionSettings?.Permissions?.FirstOrDefault(p => p.PermFullPath == permFullPath);
                //if (permission != null && permission.PermNames.Contains(permName)) {
                //    await next(context);
                //}
                //else
                //{
                //    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                //    await context.Response.WriteAsync("You are not authorized to access this resource.");
                //}
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                //var result = BaseResultProvider.BaseResult(StatusCodes.Status403Forbidden, message: exception.Message, errors: exception.ToString());
                //await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "权限中间件拦截到了错误信息");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                //var result = BaseResultProvider.BaseResult(StatusCodes.Status400BadRequest, message: exception.Message, errors: exception.ToString());
                //await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
        }
        else
        {
            await _next(context);
        }
    }
}
