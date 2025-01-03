using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Resources;

/// <summary>
/// 资源权限保存参数
/// </summary>
public class ResourcePermAclsSaveParam
{
    /// <summary>
    /// 资源ID
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// 资源类型
    /// </summary>
    public string SIDType { get; set; }

    /// <summary>
    /// 权限项
    /// </summary>
    public List<ResourcePermAclsSaveParam_Item> Acls { get; set; }
}

/// <summary>
/// 权限项
/// </summary>
public class ResourcePermAclsSaveParam_Item
{
    /// <summary>
    /// 资源角色SID
    /// </summary>
    public string SID { get; set; }
    /// <summary>
    /// 资源角色类型
    /// </summary>
    public string SIDType { get; set; }
    /// <summary>
    /// 资源角色名
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// 是否继承的
    /// </summary>
    public bool Inherited { get; set; }
    /// <summary>
    /// 允许的权限
    /// </summary>
    public List<string> AllowPermision { get; set; }
    /// <summary>
    /// 禁止的权限
    /// </summary>
    public List<string> DenyPermision { get; set; }
}
