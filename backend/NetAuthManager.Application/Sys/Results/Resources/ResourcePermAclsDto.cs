using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Resources;

/// <summary>
/// 资源操作权限
/// </summary>
public class ResourcePermAclsDto
{
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
    /// 允许权限
    /// </summary>
    public bool AllowPermision { get; set; }
    /// <summary>
    /// 允许权限是否是继承来的
    /// </summary>
    public bool IsAllowInherited { get; set; }
    /// <summary>
    /// 禁止权限
    /// </summary>
    public bool DenyPermision { get; set; }
    /// <summary>
    /// 禁止权限是否是继承来的
    /// </summary>
    public bool IsDenyInherited { get; set; }
}
