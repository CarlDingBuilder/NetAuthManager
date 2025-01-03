using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Enums;

/// <summary>
/// 对象操作权限
/// 并非所有的对象都有这些权限，可能有其中的0个或者多个，也可能所有操作都不在该范围内，这种情况使用字符串的方式调用鉴权方法
/// </summary>
public enum PermTypeEnum
{
    /// <summary>
    /// 无权限
    /// </summary>
    None,
    /// <summary>
    /// 查看（菜单访问权为改权限）
    /// </summary>
    Execute,
    /// <summary>
    /// 查看所有
    /// </summary>
    ExecuteAll,
    /// <summary>
    /// 查看经办
    /// </summary>
    ExecuteHandle,
    /// <summary>
    /// 新增
    /// </summary>
    New,
    /// <summary>
    /// 编辑
    /// </summary>
    Edit,
    /// <summary>
    /// 删除
    /// </summary>
    Delete,
    /// <summary>
    /// 导入
    /// </summary>
    Import,
    /// <summary>
    /// 导出
    /// </summary>
    Export,
    /// <summary>
    /// 分配资源，资源管理员
    /// </summary>
    [Description("资源管理员")]
    UserResourceAssignPermision,
    /// <summary>
    /// 授权管理员
    /// </summary>
    [Description("授权管理员")]
    Write,
}
