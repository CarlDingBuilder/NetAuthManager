using NetAuthManager.Core.User.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Users;

/// <summary>
/// 登录用户信息
/// </summary>
public class LoginUserBoxDto
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public LoginUserInfoDto User { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public TokenInfoDto Token { get; set; }
}
