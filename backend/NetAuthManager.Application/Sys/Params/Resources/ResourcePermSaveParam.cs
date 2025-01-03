using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Resources;

/// <summary>
/// 资源权限项保存参数
/// </summary>
public class ResourcePermSaveParam
{
    /// <summary>
    /// SID
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// 权限项
    /// </summary>
    public List<ResourcePermSaveParam_Item> Perms { get; set; }
}

/// <summary>
/// 权限项
/// </summary>
public class ResourcePermSaveParam_Item
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
    /// 排序
    /// </summary>
    public int OrderIndex { get; set; }
}
