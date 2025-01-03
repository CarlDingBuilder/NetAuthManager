using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Params.Roles;

/// <summary>
/// 角色检查参数
/// </summary>
public class RoleCheckParam
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public string Account { get; set; }
}

/// <summary>
/// 角色检查参数
/// </summary>
public class RoleCheckIsMemberParam : RoleBaseParam
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public string Account { get; set; }
}
