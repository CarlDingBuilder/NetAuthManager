using NetAuthManager.Application.Sys.Params.Roles;
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
/// 角色服务
/// </summary>
public interface IRoleManageService
{
    /// <summary>
    /// 获取角色
    /// </summary>
    Task<PageResult<SysRole>> GetRolePageList(PageSearchParam param);

    /// <summary>
    /// 获取分页列表，仅启用用户
    /// 公开访问，无需权限
    /// </summary>
    Task<PageResult<SysRole>> GetRoleNoEveryonePageList(PageSearchParam param);

    /// <summary>
    /// 获取分页列表
    /// </summary>
    Task<PageResult<SysRole>> GetAllRolePageList(PageSearchParam param);

    /// <summary>
    /// 获取角色
    /// </summary>
    Task<List<SysRole>> GetRoles();

    /// <summary>
    /// 添加角色
    /// </summary>
    Task AddRole(RoleAddParam @param);

    /// <summary>
    /// 重命名角色
    /// </summary>
    Task ModifyRole(RoleModifyParam @param);

    /// <summary>
    /// 设置是否启用
    /// </summary>
    Task SetIsOpen(SetIsOpenParam param);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task DeleteRole(RoleBaseParam @param);

    /// <summary>
    /// 删除多个角色
    /// </summary>
    Task DeleteRoles(RoleDeletesParam @param);

    /// <summary>
    /// 获取角色成员（即BPM安全组成员）
    /// </summary>
    Task<List<RoleMemberResult>> GetRoleMembers(RoleBaseParam @param);

    /// <summary>
    /// 添加角色成员
    /// </summary>
    Task AddRoleMember(RoleMemberAddParam @param);

    /// <summary>
    /// 添加多个角色成员
    /// </summary>
    Task AddRoleMembers(RoleMembersAddParam @param);

    /// <summary>
    /// 删除角色成员
    /// </summary>
    Task DeleteRoleMember(RoleMemberDeleteParam @param);

    /// <summary>
    /// 删除多个角色成员
    /// </summary>
    Task DeleteRoleMembers(RoleMemberDeletesParam @param);

    /// <summary>
    /// 保存角色成员
    /// </summary>
    Task SaveRoleMembers(SaveRoleMembersParam param);
}
