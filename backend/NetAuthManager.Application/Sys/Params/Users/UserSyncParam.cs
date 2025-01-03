using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params;

/// <summary>
/// 用户同步参数
/// </summary>
public class UserSyncParam
{
    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 员工工号
    /// </summary>
    public string HRID { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 来源系统
    /// </summary>
    public string SourceSystem { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsOpen { get; set; }
}
