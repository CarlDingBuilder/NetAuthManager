using NetAuthManager.Core.Entities;
using Mapster.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Resources;

/// <summary>
/// 资源节点数据
/// </summary>
public class ResurceNodeItem : ResourceItemDto
{
    /// <summary>
    /// 子菜单
    /// </summary>
    public List<ResurceNodeItem> Children { get; set; } = new List<ResurceNodeItem>();

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResurceNodeItem() { }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResurceNodeItem(SysMenu menu): base(menu) { }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResurceNodeItem(SysResourcePerm resourcePerm) : base(resourcePerm) { }
}