using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Application.Sys.Results.Users;
using NetAuthManager.Core.User.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

public interface IUserLoginService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginInfo">用户信息</param>
    /// <returns>访问 token 信息</returns>
    TokenInfoDto Login(LoginInfoParam loginInfo);

    /// <summary>
    /// 获取访问 token，无须验证码
    /// </summary>
    /// <param name="loginInfo">用户信息</param>
    /// <returns>访问 token 信息</returns>
    TokenInfoDto GetToken(GetTokenParam loginInfo);
    
    /// <summary>
    /// 用户认证
    /// </summary>
    /// <param name="loginInfo">用户信息</param>
    bool Authenticate(GetTokenParam loginInfo);

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    LoginUserInfoDto GetLoginUserInfo();
}
