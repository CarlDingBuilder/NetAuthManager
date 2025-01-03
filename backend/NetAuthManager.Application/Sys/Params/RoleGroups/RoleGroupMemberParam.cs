using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Params.RoleGroups;

/// <summary>
/// 角色成员添加参数
/// </summary>
public class RoleGroupMemberAddParam : RoleGroupBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public RoleGroupMember Member { get; set; }
}

/// <summary>
/// 角色成员添加参数
/// </summary>
public class RoleGroupMembersAddParam : RoleGroupBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public List<RoleGroupMember> Members { get; set; }
}

/// <summary>
/// 角色成员删除参数
/// </summary>
public class RoleGroupMemberDeleteParam : RoleGroupBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public RoleGroupMember Member { get; set; }
}

/// <summary>
/// 角色成员删除多个参数
/// </summary>
public class RoleGroupMemberDeletesParam : RoleGroupBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public List<RoleGroupMember> Members { get; set; }
}

/// <summary>
/// 保存角色组成员
/// </summary>
public class SaveRoleGroupMembersParam : RoleGroupBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public List<RoleGroupMember> Members { get; set; }
}