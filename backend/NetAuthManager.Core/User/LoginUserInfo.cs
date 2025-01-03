using NetAuthManager.Core.Consts;
using NetAuthManager.Core.Exceptions;
using Furion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.User;

/// <summary>
/// Jwt 用户登录贮存信息模块
/// </summary>
public class LoginUserInfo : Dictionary<string, object>
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public string UserAccount
    {
        get
        {
            object obj;
            this.TryGetValue(ClaimConst.UserAccountColumnName, out obj);
            return Convert.ToString(obj) ?? string.Empty;
        }
        set
        {
            this[ClaimConst.UserAccountColumnName] = value;
        }
    }

    /// <summary>
    /// 用户姓名
    /// </summary>
    public string UserDisplayName
    {
        get
        {
            object obj;
            this.TryGetValue(ClaimConst.UserDisplayNameColumnName, out obj);
            return Convert.ToString(obj) ?? string.Empty;
        }
        set
        {
            this[ClaimConst.UserDisplayNameColumnName] = value;
        }
    }

    /// <summary>
    /// HRID
    /// </summary>
    public string HRID
    {
        get
        {
            object obj;
            this.TryGetValue(ClaimConst.HRIDColumnName, out obj);
            return Convert.ToString(obj) ?? string.Empty;
        }
        set
        {
            this[ClaimConst.HRIDColumnName] = value;
        }
    }

    /// <summary>
    /// 反构建
    /// </summary>
    public LoginUserInfo() { }

    /// <summary>
    /// 反构建
    /// </summary>
    /// <param name="claims"></param>
    public LoginUserInfo(IDictionary<string, object> claims)
    {
        foreach(var claim in claims)
        {
            this.Add(claim.Key, claim.Value);
        }
    }

    /// <summary>
    /// 当前登录用户
    /// </summary>
    public static LoginUserInfo GetLoginUser()
    {
        if (App.User == null) return null;

        var dic = new Dictionary<string, object>();

        var userAccount = App.User.FindFirst(ClaimConst.UserAccountColumnName)?.Value;
        dic.Add(ClaimConst.UserAccountColumnName, userAccount);

        var userDisplayName = App.User.FindFirst(ClaimConst.UserDisplayNameColumnName)?.Value;
        dic.Add(ClaimConst.UserDisplayNameColumnName, userDisplayName);

        var hridColumnName = App.User.FindFirst(ClaimConst.HRIDColumnName)?.Value;
        dic.Add(ClaimConst.HRIDColumnName, hridColumnName);

        return new LoginUserInfo(dic);
    }
}
