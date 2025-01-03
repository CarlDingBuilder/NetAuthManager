using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using System.Linq;
using NetAuthManager.Core.User;
using NetAuthManager.Core.Options;

namespace NetAuthManager.Web.Core;

public class JwtHandler : AppAuthorizeHandler
{
    /// <summary>
    /// 重写 Handler 添加自动刷新
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task HandleAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        // 自动刷新Token
        if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext(),
            App.GetOptions<JWTSettingsOptions>().ExpiredTime,
            App.GetOptions<RefreshTokenSettingOptions>().ExpiredTime))
        {
            await AuthorizeHandleAsync(context);
        }
        else
        {
            context.Fail(); // 授权失败
            DefaultHttpContext currentHttpContext = context.GetCurrentHttpContext();
            if (currentHttpContext == null)
                return;
            currentHttpContext.SignoutToSwagger();
        }
    }

    public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false

        return await CheckAuthorzieAsync(httpContext);
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private async Task<bool> CheckAuthorzieAsync(DefaultHttpContext httpContext)
    {
        var tokenHeader = Convert.ToString(httpContext.Request.Headers["Authorization"]);
        if (string.IsNullOrEmpty(tokenHeader)) return false;

        string pattern = "^Bearer (.*?)$";
        if (!Regex.IsMatch(tokenHeader, pattern)) return false;

        string accessToken = Regex.Match(tokenHeader, pattern).Groups[1]?.ToString();
        if (string.IsNullOrEmpty(accessToken)) return false;

        var (isVaild, tokenInfo, validationResult) = JWTEncryption.Validate(accessToken);
        if (!isVaild)
        {
            var newJWTToken = Convert.ToString(httpContext.Response.Headers["access-token"]);
            if (string.IsNullOrEmpty(newJWTToken)) return false;

            (isVaild, tokenInfo, validationResult) = JWTEncryption.Validate(newJWTToken);
            if (!isVaild) return false;
        }

        var claims = validationResult.Claims;
        var loginUserInfo = new LoginUserInfo(claims);
        try
        {
            var userInfo = App.User;
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}
