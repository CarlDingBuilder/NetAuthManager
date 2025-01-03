using NetAuthManager.Core.Options;
using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.User.Dtos;

/// <summary>
/// 登录信息
/// </summary>
public class TokenInfoDto
{
    /// <summary>
    /// 访问 Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 刷新 Token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// 过期时长
    /// </summary>
    public long ExpiredTime { get; set; }

    /// <summary>
    /// 刷新Token过期时长
    /// </summary>
    public int RefreshExpiredTime { get; set; }

    /// <summary>
    /// Token 信息构造
    /// </summary>
    /// <param name="userAccount"></param>
    /// <param name="userDisplayName"></param>
    /// <param name="hrid"></param>
    public TokenInfoDto(string userAccount, string userDisplayName, string hrid)
    {
        ExpiredTime = App.GetOptions<JWTSettingsOptions>().ExpiredTime ?? 1440;
        RefreshExpiredTime = App.GetOptions<RefreshTokenSettingOptions>().ExpiredTime;
        Token = JWTEncryption.Encrypt(new LoginUserInfo()
        {
            UserAccount = userAccount,
            UserDisplayName = userDisplayName,
            HRID = hrid
        }, ExpiredTime);
        RefreshToken = JWTEncryption.GenerateRefreshToken(Token, RefreshExpiredTime);
    }
}
