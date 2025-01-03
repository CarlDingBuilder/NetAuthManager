using NetAuthManager.Application.Base.Results;
using NetAuthManager.Application.Services;
using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Application.Sys.Results.RSA;
using NetAuthManager.Application.Sys.Results.Users;
using NetAuthManager.Core.User.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 用户登录服务
/// </summary>
[ApiDescriptionSettings("SOPSysGroup@2")]
[Route("api/user")]
public class LoginService : IDynamicApiController
{
    #region 注入与构造

    private readonly IUserLoginService _userLoginService;
    private readonly IRSAService _rsaService;
    private readonly IValidateCodeService _validateCodeService;

    public LoginService(IUserLoginService userService, IRSAService rsaService, IValidateCodeService validateCodeService)
    {
        _userLoginService = userService;
        _rsaService = rsaService;
        _validateCodeService = validateCodeService;
    }

    #endregion 注入与构造

    #region 用户

    /// <summary>
    /// 登录
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [AllowAnonymous]
    [Route("login")]
    public TokenInfoDto Login(LoginInfoParam loginInfo)
    {
        return _userLoginService.Login(loginInfo);
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [AllowAnonymous]
    [HttpPost("getToken")]
    public TokenInfoDto GetToken(GetTokenParam loginInfo)
    {
        return _userLoginService.GetToken(loginInfo);
    }

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Route("getUserInfo")]
    public LoginUserInfoDto GetUserInfo()
    {
        return _userLoginService.GetLoginUserInfo();
    }

    /// <summary>
    /// 获取公钥
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [HttpGet("publicKey")]
    [AllowAnonymous]
    public GetPublicKeyResult GetPublicKey()
    {
        return _rsaService.GetPublicKey();
    }

    /// <summary>
    /// 获取登录验证码
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [HttpGet("getCaptcha")]
    [AllowAnonymous]
    public GetValidateCodeResult GetCaptcha()
    {
        return _validateCodeService.GetValidateCode();
    }

    #endregion
}
