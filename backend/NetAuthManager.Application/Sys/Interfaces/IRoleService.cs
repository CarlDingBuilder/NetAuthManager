using NetAuthManager.Application.Sys.Params.Roles;
using NetAuthManager.Application.Sys.Results.Roles;
using NetAuthManager.Application.Sys.Results.Users;
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
public interface IRoleService
{
    /// <summary>
    /// 从 SID 获取角色，获取不到报错
    /// </summary>
    SysRole FromSID(string sid);

    /// <summary>
    /// 从 SID 获取用户
    /// </summary>
    SysRole TryGetRoleBySID(string sid);

    /// <summary>
    /// 获取角色成员
    /// </summary>
    Task<List<GetRoleUsersItem>> GetRoleUsersAsync(RoleBaseParam param);

    /// <summary>
    /// 获取角色成员
    /// </summary>
    List<GetRoleUsersItem> GetRoleUsers(RoleBaseParam param);

    /// <summary>
    /// 获取分页列表，角色用户
    /// </summary>
    Task<PageResult<GetRoleUsersItemByRole>> GetRoleUserPageList(PageSearchParam param);

    /// <summary>
    /// 是否是指定角色成员
    /// </summary>
    Task<bool> IsRoleMember(string account, string roleCode);

    /// <summary>
    /// 是否是指定角色成员
    /// </summary>
    Task<bool> IsRoleSIDMember(string account, string roleSID);

    /// <summary>
    /// 是否是管理员
    /// </summary>
    Task<bool> IsAdministrator(string account);

    /// <summary>
    /// 获取用户所属的角色 SID 列表
    /// 内部已经支持多线程
    /// </summary>
    Task<List<string>> GetRoleSIDsAsync(string account);

    /// <summary>
    /// 获取所属角色
    /// </summary>
    Task<List<BelongRolesResult>> GetBelongRoles(RoleCheckParam @param);

    /// <summary>
    /// 获取用户所属角色
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task<List<BelongRolesResult>> GetBelongRoles(string account);

    /// <summary>
    /// 是否角色成员
    /// </summary>
    Task<IsBelongRoleResult> IsRoleMember(RoleCheckIsMemberParam @param);

    /// <summary>
    /// 是否是管理员
    /// </summary>
    Task<IsBelongRoleResult> IsAdministrator(RoleCheckParam param);

    /// <summary>
    /// 是否是管理员登录
    /// </summary>
    Task<IsBelongRoleResult> IsAdministratorLogin();
}
