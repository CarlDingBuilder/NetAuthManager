using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Results.RoleGroups;

/// <summary>
/// 所属角色返回
/// </summary>
public class BelongRoleGroupsResult
{
    public string GroupCode { get; set; }
    public string GroupName { get; set; }
    public string SID { get; set; }
}
