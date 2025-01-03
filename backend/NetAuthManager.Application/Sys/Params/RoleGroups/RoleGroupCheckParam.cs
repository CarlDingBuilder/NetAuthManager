using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Params.RoleGroups;

/// <summary>
/// 角色检查参数
/// </summary>
public class RoleGroupCheckParam
{
    /// <summary>
    /// 角色编码
    /// </summary>
    public string RoleCode { get; set; }
}

/// <summary>
/// 角色检查参数
/// </summary>
public class RoleGroupCheckIsMemberParam : RoleGroupBaseParam
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public string RoleCode { get; set; }
}
