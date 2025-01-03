using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Resources;

/// <summary>
/// 获取资源结构参数
/// </summary>
public class ResourceBaseParam
{
    /// <summary>
    /// 资源 SID
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// 资源 SID 类型
    /// </summary>
    public string SIDType { get; set; }
}
