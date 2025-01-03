using NetAuthManager.Application.Filters;
using NetAuthManager.Core.Common.Exceptions;

//using BPM.FSSC.Core.Helper;
using Furion.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Results.Provider;

/// <summary>
/// 返回结果支持
/// </summary>
[UnifyModel(typeof(BaseResult<>))]
public class BaseResultProvider : IUnifyResultProvider
{
    private readonly ILogger<BaseResultProvider> _logger;
    public BaseResultProvider(ILogger<BaseResultProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
    {
        //错误信息
        var exception = context.Exception;

        //记录错误日志
        _logger.LogError(context.Exception, "OnException：" + exception?.Message);

        //if (context.HttpContext.Request.ContentType == "application/xml")
        //{
        //    return new ContentResult()
        //    {
        //        Content = XmlHelper.Serializer(typeof(BaseResult<object>), BaseResult(
        //            statusCode: metadata.StatusCode,
        //            message: context.Exception?.Message ?? string.Empty
        //        )),
        //        ContentType = "application/xml",
        //        StatusCode = metadata.StatusCode,
        //    };
        //}
        //else
        //{
        //    return new JsonResult(BaseResult(
        //        statusCode: metadata.StatusCode,
        //        errors: metadata.Errors,
        //        message: context.Exception?.Message ?? string.Empty
        //    ));
        //}
        var message = string.Empty;
        if (exception != null)
        {
            if (exception is AggregateException aggregateException)
            {
                message = aggregateException.GetMessage();
            }
            else message = exception.Message;
        }
        return new JsonResult(BaseResult(
            statusCode: metadata.StatusCode,
            errors: metadata.Errors,
            message: message ?? string.Empty
        ));
    }

    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnAuthorizeException(DefaultHttpContext context, ExceptionMetadata metadata)
    {
        //错误信息
        var exception = metadata.Exception;

        //记录错误日志
        _logger.LogError(metadata.Exception, "OnAuthorizeException：" + exception?.Message);

        //if (context.HttpContext.Request.ContentType == "application/xml")
        //{
        //    return new ContentResult()
        //    {
        //        Content = XmlHelper.Serializer(typeof(BaseResult<object>), BaseResult(
        //            statusCode: metadata.StatusCode,
        //            message: context.Exception?.Message ?? string.Empty
        //        )),
        //        ContentType = "application/xml",
        //        StatusCode = metadata.StatusCode,
        //    };
        //}
        //else
        //{
        //    return new JsonResult(BaseResult(
        //        statusCode: metadata.StatusCode,
        //        errors: metadata.Errors,
        //        message: context.Exception?.Message ?? string.Empty
        //    ));
        //}
        var message = string.Empty;
        if (exception != null)
        {
            if (exception is AggregateException aggregateException)
            {
                message = aggregateException.GetMessage();
            }
            else message = exception.Message;
        }
        return new JsonResult(BaseResult(
            statusCode: metadata.StatusCode,
            errors: metadata.Errors,
            message: message ?? string.Empty
        ));
    }

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        //if (context.HttpContext.Request.ContentType == "application/xml")
        //{
        //    return new ContentResult()
        //    {
        //        Content = XmlHelper.Serializer(typeof(BaseResult<object>), BaseResult(
        //            statusCode: StatusCodes.Status200OK,
        //            succeeded: true,
        //            data: data
        //        )),
        //        ContentType = "application/xml",
        //        StatusCode = StatusCodes.Status200OK,
        //    };
        //}
        //else
        //{
        //    return new JsonResult(BaseResult(StatusCodes.Status200OK, true, data));
        //}
        return new JsonResult(BaseResult(StatusCodes.Status200OK, true, data));
    }

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        //记录错误日志
        _logger.LogError("OnValidateFailed：" + metadata.Message ?? metadata.FirstErrorMessage ?? string.Empty);

        //if (context.HttpContext.Request.ContentType == "application/xml")
        //{
        //    return new ContentResult()
        //    {
        //        Content = XmlHelper.Serializer(typeof(BaseResult<object>), BaseResult(
        //            statusCode: metadata.StatusCode ?? StatusCodes.Status400BadRequest,
        //            message: metadata.Message ?? metadata.FirstErrorMessage ?? string.Empty
        //        )),
        //        ContentType = "application/xml",
        //        StatusCode = metadata.StatusCode ?? StatusCodes.Status400BadRequest,
        //    };
        //}
        //else
        //{
        //    return new JsonResult(BaseResult(
        //        statusCode: metadata.StatusCode ?? StatusCodes.Status400BadRequest,
        //        errors: metadata.ValidationResult,
        //        message: metadata.Message ?? metadata.FirstErrorMessage ?? string.Empty
        //    ));
        //}
        return new JsonResult(BaseResult(
            statusCode: metadata.StatusCode ?? StatusCodes.Status400BadRequest,
            errors: metadata.ValidationResult,
            message: metadata.Message ?? metadata.FirstErrorMessage ?? string.Empty
        ));
    }

    /// <summary>
    /// 特定状态码返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="statusCode"></param>
    /// <param name="unifyResultSettings"></param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings)
    {
        // 设置响应状态码
        UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);
        switch (statusCode)
        {
            // 处理 401 状态码
            case StatusCodes.Status401Unauthorized:
                //if (context.Request.ContentType == "application/xml")
                //{
                //    await context.Response.WriteAsJsonAsync(XmlHelper.Serializer(typeof(BaseResult<object>), BaseResult(
                //        statusCode: statusCode,
                //        message: "401 Unauthorized"
                //    )));
                //}
                //else
                //{
                    await context.Response.WriteAsJsonAsync(BaseResult(
                        statusCode: statusCode, 
                        errors: "401 Unauthorized",
                        message: "401 Unauthorized"
                    ),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                //}
                break;
            // 处理 403 状态码
            case StatusCodes.Status403Forbidden:
                //if (context.Request.ContentType == "application/xml")
                //{
                //    await context.Response.WriteAsJsonAsync(XmlHelper.Serializer(typeof(BaseResult<object>), BaseResult(
                //        statusCode: statusCode,
                //        message: "403 Forbidden"
                //    )));
                //}
                //else
                //{
                    await context.Response.WriteAsJsonAsync(BaseResult(
                        statusCode: statusCode,
                        errors: "403 Forbidden",
                        message: "403 Forbidden"
                    ),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                //}
                break;
            default: break;
        }
    }

    /// <summary>
    /// 返回 RESTful 风格结果集
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="succeeded"></param>
    /// <param name="data"></param>
    /// <param name="errors"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static BaseResult BaseResult(int statusCode, bool succeeded = default, object data = default, object errors = default, string message = default)
    {
        if (data != null)
            return new BaseResult<object>
            {
                Code = statusCode,
                Success = succeeded,
                Msg = message,
                Data = data,
                Errors = errors,
                Extras = UnifyContext.Take(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        else
            return new BaseResult
            {
                Code = statusCode,
                Success = succeeded,
                Msg = message,
                Errors = errors,
                Extras = UnifyContext.Take(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
    }
}
