using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Resources;

/// <summary>
/// 资源权限检测参数
/// </summary>
public class ResourcePermCheckParam
{
    /// <summary>
    /// 资源 SID
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// 账号，当不提供时，默认为当前登录用户
    /// </summary>
    public string Account { get; set; }
}
