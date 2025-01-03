using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Resources;

/// <summary>
/// 资源操作权限历史记录
/// </summary>
public class ResourcePermRecordDto
{
    /// <summary>
    /// 权限名
    /// </summary>
    public string PermName { get; set; }

    /// <summary>
    /// 显示名
    /// </summary>
    public string PermDisplayName { get; set; }
}
