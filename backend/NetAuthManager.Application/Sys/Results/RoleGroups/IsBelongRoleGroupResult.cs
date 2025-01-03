using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Results.RoleGroups;

/// <summary>
/// 是否属于角色返回
/// </summary>
public class IsBelongRoleGroupResult
{
    /// <summary>
    /// 是否属于
    /// </summary>
    public bool IsBelong { get; set; }
}
