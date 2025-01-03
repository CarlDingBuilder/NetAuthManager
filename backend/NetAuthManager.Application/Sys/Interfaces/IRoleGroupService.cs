using NetAuthManager.Application.Sys.Params.RoleGroups;
using NetAuthManager.Application.Sys.Results.RoleGroups;
using NetAuthManager.Application.Sys.Results.Roles;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.EntityFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 角色组服务
/// </summary>
public interface IRoleGroupService
{
    /// <summary>
    /// 从 SID 获取角色组，获取不到报错
    /// </summary>
    SysRoleGroup FromSID(string sid);

    /// <summary>
    /// 从 SID 获取用户
    /// </summary>
    public SysRoleGroup TryGetRoleGroupBySID(string sid);

    /// <summary>
    /// 是否是指定角色组成员
    /// 是否是组成员，只要是其中一个组成员就行了
    /// </summary>
    Task<bool> IsRoleGroupMember(string roleCode, string groupCode);

    /// <summary>
    /// 是否是指定角色组成员
    /// </summary>
    bool IsOneRoleGroupMemberUser(string account, List<string> groupCodes);

    /// <summary>
    /// 是否是指定角色组成员
    /// </summary>
    Task<bool> IsRoleGroupSIDMember(string roleCode, string groupSID);

    /// <summary>
    /// 获取角色所属的角色组 SID 列表
    /// </summary>
    Task<List<string>> GetRoleGroupSIDsAsync(List<string> roleSIDs);

    /// <summary>
    /// 获取角色所属的角色组 SID 列表
    /// 仅考虑开启的角色
    /// </summary>
    Task<List<string>> GetRoleGroupSIDsAsync(DefaultDbContext dbContext, List<string> roleSIDs);

    /// <summary>
    /// 获取所属角色组
    /// </summary>
    Task<List<BelongRoleGroupsResult>> GetBelongRoleGroups(RoleGroupCheckParam @param);

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    /// <param name="roleCode"></param>
    /// <returns></returns>
    Task<List<BelongRoleGroupsResult>> GetBelongRoleGroups(string roleCode);

    ///// <summary>
    ///// 获取用户所属角色组
    ///// </summary>
    //List<BelongRoleGroupsResult> GetBelongRoleGroupsByUser(string account);

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    List<SysRoleGroup> GetOwerRoleGroupsByUser(string account);

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    Task<List<SysRoleGroup>> GetOwerRoleGroupsByUserAsync(string account);

    /// <summary>
    /// 获取用户所属角色组下属角色
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task<List<BelongRolesResult>> GetBelongRoleGroupRolesByUserAsync(string account);

    /// <summary>
    /// 获取用户所属角色组下属角色
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    List<BelongRolesResult> GetBelongRoleGroupRolesByUser(string account);

    /// <summary>
    /// 是否角色组成员
    /// </summary>
    Task<IsBelongRoleGroupResult> IsRoleGroupMember(RoleGroupCheckIsMemberParam @param);
}
