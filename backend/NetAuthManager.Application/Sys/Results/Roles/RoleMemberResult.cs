using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Results.Roles;

/// <summary>
/// 角色成员 DTO
/// </summary>
public class RoleMemberResult
{
    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// 成员 SID
    /// </summary>
    public string SID { get; set; }
    /// <summary>
    /// 成员类型
    /// </summary>
    public string SIDType { get; set; }
}
