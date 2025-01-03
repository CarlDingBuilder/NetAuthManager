using NetAuthManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Resources;

/// <summary>
/// 资源操作方式
/// </summary>
public class ResourcePermItemDto
{
    /// <summary>
    /// 资源ID
    /// </summary>
    public string SID { get; set; }
    /// <summary>
    /// 权限名
    /// </summary>
    public string PermName { get; set; }

    /// <summary>
    /// 权限名
    /// </summary>
    public string PermDisplayName { get; set; }

    /// <summary>
    /// 权限类型：模块、记录
    /// </summary>
    public string PermType { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public List<ResourcePermRoleItemDto_RoleItem> Roles { get; set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResourcePermItemDto() { }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResourcePermItemDto(SysResourcePerm resourcePerm)
    {
        this.CopyFrom(resourcePerm);
    }

    /// <summary>
    /// 格式化到子对象
    /// </summary>
    /// <param name="resourcePerm">资源功能</param>
    /// <returns></returns>
    protected virtual void CopyFrom(SysResourcePerm resourcePerm)
    {
        if (resourcePerm == null) return;

        this.SID = resourcePerm.SID;
        this.PermName = resourcePerm.PermName;
        this.PermDisplayName = resourcePerm.PermDisplayName;
        this.PermType = resourcePerm.PermType;
    }
}

/// <summary>
/// 资源操作方式包装
/// </summary>
public class ResourcePermItemBoxDto
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public ResourceItemDto Resource { get; set; }

    /// <summary>
    /// 资源操作方式
    /// </summary>
    public List<ResourcePermItemDto> Perms { get; set; }

    /// <summary>
    /// 资源操作权限类型
    /// </summary>
    public List<ResourcePermTypeDto> PermTypes { get; set; }

    /// <summary>
    /// 资源操作权限记录
    /// </summary>
    public List<ResourcePermRecordDto> PermLogs { get; set; }
}