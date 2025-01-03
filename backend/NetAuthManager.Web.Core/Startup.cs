using Furion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetAuthManager.Application.Results.Provider;
using NetAuthManager.Core.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using NetAuthManager.Application.Filters;
using NetAuthManager.Application.Middlewares;
using NetAuthManager.Core.Entities;
using NetAuthManager.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace NetAuthManager.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConsoleFormatter();
        services.AddConfigurableOptions<RefreshTokenSettingOptions>();
        services.AddJwt<JwtHandler>(enableGlobalAuthorize: true, jwtBearerConfigure: options =>
        {
            // 实现 JWT 身份验证过程控制
            options.Events = new JwtBearerEvents
            {
                // 添加读取 Token 的方式
                OnMessageReceived = context =>
                {
                    var httpContext = context.HttpContext;

                    // 判断请求是否包含 Authorization 参数，如果有就设置给 Token
                    if (httpContext.Request.Query.ContainsKey("access_token"))
                    {
                        var token = httpContext.Request.Query["access_token"];

                        // 设置 Token
                        context.Token = token;
                    }

                    return System.Threading.Tasks.Task.CompletedTask;
                }
            };
        });

        services.AddCorsAccessor();

        services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // 首字母小写(驼峰样式)
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    // 时间格式化
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

                    // 枚举格式化
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());

                    // 忽略循环
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddInjectWithUnifyResult<BaseResultProvider>();

        //例子二：每天创建一个日志文件
        services.AddFileLogging("Logs/{0:yyyy}-{0:MM}-{0:dd}.log", options =>
        {
            options.FileNameRule = fileName =>
            {
                return string.Format(fileName, DateTime.UtcNow);
            };
        });

        //添加基础错误过滤器
        services.AddMvcFilter<BaseExceptionFilter>();

        //缓存服务
        services.AddMemoryCache();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 开启请求日志
        app.UseHttpLogging();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // 添加规范化结果状态码，需要在这里注册
        app.UseUnifyResultStatusCodes();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject(string.Empty);

        app.UseRateLimitAuthorize(); //限流中间件
        app.UsePermAuthorize(); //权限中间件

        //语言设置
        app.UseRequestLocalization(option =>
        {
            option.SetDefaultCulture("zh-CHS").AddSupportedCultures("zh-CHS").AddSupportedUICultures("zh-CHS");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
