using NetAuthManager.Application.Common.Helpers;
using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Application.Sys.Results.Users;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.User;
using NetAuthManager.Core.User.Dtos;
using Furion.EventBus;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

public class UserLoginService : IUserLoginService, ITransient
{
    #region 构造与注入

    private readonly IRepository<SysUser> _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cacheService;
    private readonly IRSAService _rsaService;
    public UserLoginService(IHttpContextAccessor httpContextAccessor, IRepository<SysUser> userRepository, ICacheService cacheService, IRSAService rsaService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _rsaService = rsaService;
    }

    #endregion 构造与注入

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginInfo">用户信息</param>
    /// <returns>访问 token 信息</returns>
    public TokenInfoDto Login(LoginInfoParam loginInfo)
    {
        if (string.IsNullOrEmpty(loginInfo.VerificationCode))
            throw new CustomException("验证码不能为空");

        //获取私钥并解密，这里的 loginInfo.Keystore 是缓存私钥的 key
        var keystore = loginInfo.Keystore;
        var verifyCodeKeystore = loginInfo.VerifyCodeKeystore;
        var privateKey = _cacheService.Get<string>(keystore);

        var json = _rsaService.Decrypt(loginInfo.Password, privateKey);
        loginInfo = JsonConvert.DeserializeObject<LoginInfoParam>(json);

        //验证码校验
        var verificationCode = _cacheService.Get<string>(verifyCodeKeystore)?.ToLower();
        loginInfo.VerificationCode = loginInfo.VerificationCode?.ToLower();
        if (!loginInfo.VerificationCode.Equals(verificationCode))
            throw new CustomException("验证码错误");

        return GetToken(loginInfo);
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <param name="loginInfo">用户信息</param>
    /// <returns>访问 token 信息</returns>
    public TokenInfoDto GetToken(GetTokenParam loginInfo)
    {
        //用户信息
        var user = _userRepository.FirstOrDefault(x => x.Account == loginInfo.UserName);
        if (user == null)
            throw new CustomException("用户不存在");

        //授权登录
        if (!Authenticate(loginInfo, user))
            throw new CustomException("用户名或密码错误");

        var tokenInfo = new TokenInfoDto(user.Account, user.Name, user.HRID);

        // 设置Swagger自动登录
        _httpContextAccessor.HttpContext.SigninToSwagger(tokenInfo.Token);

        // 设置刷新Token令牌
        _httpContextAccessor.HttpContext.Response.Headers["access-token"] = tokenInfo.Token;
        _httpContextAccessor.HttpContext.Response.Headers["x-access-token"] = tokenInfo.RefreshToken;

        return tokenInfo;
    }

    /// <summary>
    /// 用户认证
    /// </summary>
    /// <param name="loginInfo">用户信息</param>
    public bool Authenticate(GetTokenParam loginInfo)
    {
        var user = _userRepository.FirstOrDefault(x => x.Account == loginInfo.UserName);
        if (user == null)
            throw new CustomException("用户不存在");
        return Authenticate(loginInfo, user);
    }

    /// <summary>
    /// 用户认证
    /// </summary>
    /// <param name="loginInfo">用户信息</param>
    /// <param name="user">用户实体信息</param>
    private bool Authenticate(GetTokenParam loginInfo, SysUser user)
    {
        if (user == null) return false;

        //认证模式
        var authMode = App.GetConfig<string>("AppSettings:Auth:AuthMode");

        bool success;
        if (string.IsNullOrEmpty(user.Password) && string.IsNullOrEmpty(loginInfo.Password))
        {
            success = true;
        }
        else
        {
            if (!string.IsNullOrEmpty(user.Password)) user.Password = user.Password.TrimEnd();

            //构建md5
            var md5 = MD5Helper.GetMd5(loginInfo.Password);
            if (md5 == user.Password) success = true;
            else success = false;
        }
        return success;
    }

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    public LoginUserInfoDto GetLoginUserInfo()
    {
        var loginUserInfo = LoginUserInfo.GetLoginUser();
        if (loginUserInfo == null) return null;

        var userInfo = new LoginUserInfoDto()
        {
            UserAccount = loginUserInfo.UserAccount,
            UserName = loginUserInfo.UserDisplayName,
            HRID = loginUserInfo.HRID
        };
        return userInfo;
    }
}
