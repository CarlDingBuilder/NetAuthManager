using NetAuthManager.Application.Sys.Params;
using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Application.Sys.Results.DataSync;
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
/// 用户服务
/// </summary>
public interface IUserManageService
{
    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    Task<PageResult<SysUser>> GetAllPageList(PageSearchParam param);

    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    Task<PageResult<SysUser>> GetPageList(PageSearchParam param);

    /// <summary>
    /// 同步用户
    /// </summary>
    Task<SyncResult> SyncUsers(List<UserSyncParam> param);

    /// <summary>
    /// 添加用户
    /// </summary>
    Task AddUser(UserAddParam param);

    /// <summary>
    /// 重命名用户
    /// </summary>
    Task ModifyUser(UserModifyParam param);

    /// <summary>
    /// 修改用户密码
    /// </summary>
    Task ModifyPassword(UserModifyPasswordParam param);

    /// <summary>
    /// 设置用户是否启用
    /// </summary>
    Task SetUserIsOpen(SetUserIsOpenParam param);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task DeleteUser(UserBaseParam param);

    /// <summary>
    /// 删除多个用户
    /// </summary>
    Task DeleteUsers(UserDeletesParam param);

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<List<UserRoleItem>> GetUserRoles(UserBaseParam param);

    /// <summary>
    /// 保存用户角色
    /// </summary>
    Task SaveUserRoles(SaveUserRolesParam param);
}
