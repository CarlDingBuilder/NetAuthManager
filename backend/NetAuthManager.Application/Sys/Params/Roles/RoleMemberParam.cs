using System;
using System.Collections.Generic;
using System.Text;

namespace NetAuthManager.Application.Sys.Params.Roles;

/// <summary>
/// 角色成员添加参数
/// </summary>
public class RoleMemberAddParam : RoleBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public RoleMember Member { get; set; }
}

/// <summary>
/// 角色成员添加参数
/// </summary>
public class RoleMembersAddParam : RoleBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public List<RoleMember> Members { get; set; }
}

/// <summary>
/// 角色成员删除参数
/// </summary>
public class RoleMemberDeleteParam : RoleBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public RoleMember Member { get; set; }
}

/// <summary>
/// 角色成员删除多个参数
/// </summary>
public class RoleMemberDeletesParam : RoleBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public List<RoleMember> Members { get; set; }
}

/// <summary>
/// 保存角色成员
/// </summary>
public class SaveRoleMembersParam : RoleBaseParam
{
    /// <summary>
    /// 角色成员
    /// </summary>
    public List<RoleMember> Members { get; set; }
}