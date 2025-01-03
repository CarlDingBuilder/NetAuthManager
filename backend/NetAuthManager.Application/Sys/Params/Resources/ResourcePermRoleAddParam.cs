using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Resources;

/// <summary>
/// 添加资源角色参数
/// </summary>
public class ResourcePermRoleAddParam : ResourceBaseParam
{
    /// <summary>
    /// 角色SID
    /// </summary>
    public string RoleSID { get; set; }

    /// <summary>
    /// 角色类型
    /// </summary>
    public string RoleSIDType { get; set; }
}
