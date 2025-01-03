using NetAuthManager.Application.Sys.Params.RoleGroups;
using NetAuthManager.Application.Sys.Results.RoleGroups;
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
/// 角色组管理服务
/// </summary>
public interface IRoleGroupManageService
{
    /// <summary>
    /// 获取角色组
    /// </summary>
    Task<PageResult<SysRoleGroup>> GetRoleGroupPageList(PageSearchParam param);

    /// <summary>
    /// 获取分页列表
    /// </summary>
    Task<PageResult<SysRoleGroup>> GetAllRoleGroupPageList(PageSearchParam param);

    /// <summary>
    /// 获取角色组
    /// </summary>
    Task<List<SysRoleGroup>> GetRoleGroups();

    /// <summary>
    /// 获取角色组
    /// </summary>
    Task<List<SysRoleGroup>> GetOwerRoleGroupsAsync();

    /// <summary>
    /// 添加角色组
    /// </summary>
    Task<SysRoleGroup> AddRoleGroup(RoleGroupAddParam @param);

    /// <summary>
    /// 重命名角色组
    /// </summary>
    Task ModifyRoleGroup(RoleGroupModifyParam @param);

    /// <summary>
    /// 设置是否启用
    /// </summary>
    Task SetIsOpen(SetRoleGroupIsOpenParam param);

    /// <summary>
    /// 删除角色组
    /// </summary>
    Task DeleteRoleGroup(RoleGroupBaseParam @param);

    /// <summary>
    /// 删除多个角色组
    /// </summary>
    Task DeleteRoleGroups(RoleGroupDeletesParam @param);

    /// <summary>
    /// 获取角色组成员（即BPM安全组成员）
    /// </summary>
    Task<List<RoleGroupMemberResult>> GetRoleGroupMembers(RoleGroupBaseParam @param);

    /// <summary>
    /// 获取角色组-角色成员
    /// </summary>
    Task<List<RoleGroupRoleItem>> GetRoleGroupRoles(RoleGroupBaseParam param);

    /// <summary>
    /// 添加角色组成员
    /// </summary>
    Task AddRoleGroupMember(RoleGroupMemberAddParam @param);

    /// <summary>
    /// 添加多个角色组成员
    /// </summary>
    Task AddRoleGroupMembers(RoleGroupMembersAddParam @param);

    /// <summary>
    /// 删除角色组成员
    /// </summary>
    Task DeleteRoleGroupMember(RoleGroupMemberDeleteParam @param);

    /// <summary>
    /// 删除多个角色组成员
    /// </summary>
    Task DeleteRoleGroupMembers(RoleGroupMemberDeletesParam @param);

    /// <summary>
    /// 保存角色组成员
    /// </summary>
    Task SaveRoleGroupMembers(SaveRoleGroupMembersParam param);
}
