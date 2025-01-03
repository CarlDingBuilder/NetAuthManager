using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Results.RoleGroups;

/// <summary>
/// 角色成员 DTO
/// </summary>
public class RoleGroupMemberResult
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
