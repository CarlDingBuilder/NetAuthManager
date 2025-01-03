using NetAuthManager.Application.Results;
using NetAuthManager.Application.Results.Provider;
using NetAuthManager.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace NetAuthManager.Application.Filters;

public class BaseExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<BaseExceptionFilter> _logger;
    public BaseExceptionFilter(ILogger<BaseExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 异常拦截器
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        // 如果异常在其他地方被标记了处理，那么这里不再处理
        if (!context.ExceptionHandled)
        {
            // 获取控制器信息
            //ControllerActionDescriptor actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            // 获取请求的方法
            //var method = actionDescriptor!.MethodInfo;

            // 获取异常对象
            var exception = context.Exception;

            // 获取调用堆栈信息，提供更加简单明了的调用和异常堆栈
            var stackTrace = EnhancedStackTrace.Current();

            // 记录日志
            if (exception is not CustomException)
            {
                _logger.LogError(exception, "全局错误拦截到了错误信息");
            }

            // 判断如果是 MVC 视图，可以动态添加数据到页面中
            if (context.Result is ViewResult viewResult)
            {
                // MVC 直接返回自定义的错误页面，或者 BadPageResult 类型，如：context.Result = new BadPageResult(StatusCodes.Status500InternalServerError) { }

                //await Task.CompletedTask;

                // 代码参考接口方式
                await base.OnExceptionAsync(context);
            }
            else
            {
                // 代码参考接口方式
                context.Result = new JsonResult(BaseResultProvider.BaseResult(StatusCodes.Status400BadRequest, message: exception.Message, errors: exception.ToString()));
                //await base.OnExceptionAsync(context);
            };
        }

        //标记为已处理
        context.ExceptionHandled = true;
    }
}
