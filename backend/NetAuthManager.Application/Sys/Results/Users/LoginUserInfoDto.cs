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
public class LoginUserInfoDto
{
    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; }
    /// <summary>
    /// 用户账号
    /// </summary>
    public string UserAccount { get; set; }
    /// <summary>
    /// 用户姓名
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; set; }
    /// <summary>
    /// 用户账号
    /// </summary>
    public string HRID { get; set; }
}
