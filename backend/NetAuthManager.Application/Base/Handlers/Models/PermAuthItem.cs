using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Handlers.Models;

/// <summary>
/// 权限授权项
/// </summary>
public class PermAuthItem
{
    /// <summary>
    /// 权限资源全路径
    /// </summary>
    public string PermFullPath { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    public string PermName { get; set; }
}
