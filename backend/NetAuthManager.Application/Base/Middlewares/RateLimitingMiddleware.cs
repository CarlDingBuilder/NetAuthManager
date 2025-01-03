using NetAuthManager.Application.Base.Exceptions;
using NetAuthManager.Application.Filters;
using NetAuthManager.Application.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Middlewares;

/// <summary>
/// 限流中间件
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitCounterStore _counterStore;
    private readonly ILogger<BaseExceptionFilter> _logger;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<BaseExceptionFilter> logger, IRateLimitCounterStore counterStore)
    {
        _next = next;
        _logger = logger;
        _counterStore = counterStore;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var rateLimitAttribute = endpoint.Metadata?.GetMetadata<RateLimitAttribute>();
            if (rateLimitAttribute != null)
            {
                try
                {
                    var key = $"{context.Request.Path}:{context.Request.Method}";
                    var counter = await _counterStore.GetAsync(key, context.RequestAborted);

                    if (counter != null && DateTime.UtcNow < counter.Timestamp.AddMinutes(1) && counter.TotalRequests >= rateLimitAttribute.RequestsPerMinute)
                    {
                        throw new TooManyRequestsException("超出每分钟内最大访问次数");
                    }

                    await _counterStore.IncrementAsync(key, context.RequestAborted);
                }
                catch (TooManyRequestsException exception)
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                    _logger.LogError($"限流中间件拦截，{exception.Message}，IP：{ipAddress}");

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    //var result = BaseResultProvider.BaseResult(StatusCodes.Status403Forbidden, message: exception.Message, errors: exception.ToString());
                    //await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "限流中间件拦截到了错误信息");

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    //var result = BaseResultProvider.BaseResult(StatusCodes.Status400BadRequest, message: exception.Message, errors: exception.ToString());
                    //await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                }
            }
        }

        await _next(context);
    }
}
