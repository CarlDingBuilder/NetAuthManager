using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Results.Roles;

/// <summary>
/// 所属角色返回
/// </summary>
public class BelongRolesResult
{
    public string RoleCode { get; set; }
    public string RoleName { get; set; }
    public string SID { get; set; }
}
