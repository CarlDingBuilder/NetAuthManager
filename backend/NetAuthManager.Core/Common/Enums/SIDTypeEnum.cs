using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Enums;

/// <summary>
/// 资源SID类型
/// </summary>
public enum SIDTypeEnum
{
    /// <summary>
    /// 用户
    /// </summary>
    UserSID = 0,

    /// <summary>
    /// 角色
    /// </summary>
    RoleSID = 1,

    /// <summary>
    /// OU
    /// </summary>
    OUSID = 2,

    /// <summary>
    /// 职级
    /// </summary>
    JobLevelSID = 3,

    /// <summary>
    /// 组织角色
    /// </summary>
    OURoleSID = 4,

    /// <summary>
    /// 职位
    /// </summary>
    LeaderTitleSID = 5,

    /// <summary>
    /// 菜单
    /// </summary>
    MenuSID = 6,

    /// <summary>
    /// 文件分级
    /// </summary>
    FileLevelSID = 7,

    /// <summary>
    /// 角色组
    /// </summary>
    RoleGroupSID = 98,

    /// <summary>
    /// 资源
    /// </summary>
    ResourceSID = 99,

    /// <summary>
    /// 未知
    /// </summary>
    Unknow = 999999
}
