using NetAuthManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Resources;

/// <summary>
/// 有权限的角色对象，这里的角色是角色、部门、人员的通称
/// </summary>
public class ResourcePermRoleItemDto_RoleItem
{
    /// <summary>
    /// 角色名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 角色SID
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// 角色类型：角色、角色组、用户
    /// </summary>
    public string SIDType { get; set; }

    /// <summary>
    /// 是否继承来的角色
    /// </summary>
    public bool Inherited { get; set; }
}

/// <summary>
/// 资源操作方式包装
/// </summary>
public class ResourcePermRolesBoxResult
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public ResourceItemDto Resource { get; set; }

    /// <summary>
    /// 角色对象
    /// </summary>
    public List<ResourcePermRoleItemDto_RoleItem> Roles { get; set; }
}
