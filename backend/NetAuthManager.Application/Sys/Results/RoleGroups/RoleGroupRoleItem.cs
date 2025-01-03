using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Results.RoleGroups;

/// <summary>
/// 角色成员 DTO
/// </summary>
public class RoleGroupRoleItem
{
    /// <summary>
    /// 角色编码
    /// </summary>
    public string RoleCode { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; }
}
