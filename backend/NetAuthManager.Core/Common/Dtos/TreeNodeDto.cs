using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Dtos;

/// <summary>
/// 树形节点
/// </summary>
public class TreeNodeDto<T>
{
    /// <summary>
    /// 树形节点唯一标识
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 树形节点文本
    /// </summary>
    public string Label => Name;

    /// <summary>
    /// 树形节点文本
    /// </summary>
    public string Text => Name;

    /// <summary>
    /// 树形节点文本
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否叶子节点
    /// </summary>
    public bool Leaf => IsLeaf;

    /// <summary>
    /// 是否叶子节点
    /// </summary>
    public bool IsLeaf { get; set; }

    /// <summary>
    /// 展开
    /// </summary>
    public bool Expanded { get; set; }

    /// <summary>
    /// 是否禁用
    /// </summary>
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// 扩展数据
    /// </summary>
    public T ExtData { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<TreeNodeDto<T>> Children { get; set; }
}

/// <summary>
/// 树形节点
/// </summary>
public class TreeNodeDto : TreeNodeDto<object>
{
}
