using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Resources;

/// <summary>
/// 判断资源操作权限
/// </summary>
public class ResourcePermChecksDto
{
    /// <summary>
    /// 权限列表
    /// </summary>
    public Dictionary<string, bool> Perms { get; set; }
}
