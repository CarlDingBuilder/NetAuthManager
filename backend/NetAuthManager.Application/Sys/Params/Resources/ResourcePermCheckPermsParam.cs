using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Resources;

/// <summary>
/// 资源权限检测参数
/// </summary>
public class ResourcePermCheckPermsParam : ResourcePermCheckParam
{
    /// <summary>
    /// 操作名称
    /// </summary>
    public List<string> PermNames { get; set; }
}
