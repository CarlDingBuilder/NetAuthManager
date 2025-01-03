using NetAuthManager.Application.Sys.Params.Resources;
using NetAuthManager.Application.Sys.Results.Resources;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Entities;
using NetAuthManager.EntityFramework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 菜单相关接口
/// </summary>
public interface IResourcePermCommonService
{
    /// <summary>
    /// 获取用户所有权限相关的 SID，包括用户、部门、角色
    /// </summary>
    Task<List<ResourceBaseParam>> GetUserPermSIDs();

    /// <summary>
    /// 获取用户所有权限相关的 SID，包括用户、角色组、角色
    /// </summary>
    Task<List<ResourceBaseParam>> GetUserPermSIDs(string account);

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    Task<List<SysResourcePerm>> GetPermisions(string sid);

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <param name="sids"></param>
    /// <returns></returns>
    Task<List<SysResourcePerm>> GetPermisions(List<string> sids);

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    Task<List<SysResourcePerm>> GetPermisions(DefaultDbContext dbContext, List<string> sids);

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    /// <param name="sids"></param>
    /// <returns></returns>
    Task<List<SysResourcePermAcl>> GetPermisionAcls(List<string> sids);

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    Task<List<SysResourcePermAcl>> GetPermisionAcls(DefaultDbContext dbContext, List<string> sids);

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    Task<List<SysResourcePermAcl>> GetPermisionAcls(string sid);

    /// <summary>
    /// 判断是否有权限
    /// </summary>
    /// <param name="permName"></param>
    /// <param name="perms"></param>
    /// <param name="acls"></param>
    /// <returns></returns>
    bool CheckPermision(string permName, IEnumerable<SysResourcePerm> perms, IEnumerable<SysResourcePermAcl> acls);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    Task<bool> CheckExecutePermision(string sid);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <returns></returns>
    Task<bool> CheckExecutePermision(string sid, string account);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <returns></returns>
    Task<bool> CheckPermision(string sid, PermTypeEnum perm);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <returns></returns>
    Task<bool> CheckPermision(string sid, string account, PermTypeEnum perm);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permName"></param>
    /// <returns></returns>
    Task<bool> CheckPermision(string sid, string permName);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permName"></param>
    /// <returns></returns>
    Task<bool> CheckPermision(string sid, string account, string permName);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    Task<List<bool>> CheckPermision(string sid, List<string> permNames);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    Task<List<bool>> CheckPermision(string sid, string account, List<string> permNames);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    Task<Dictionary<string, bool>> CheckPermisionBack(string sid, List<string> permNames);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    Task<Dictionary<string, bool>> CheckPermisionBack(string sid, string account, List<string> permNames);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="userRoles"></param>
    /// <param name="isAdministrator"></param>
    /// <returns></returns>
    Task<Hashtable> CheckPermisionBack(string sid, List<ResourceBaseParam> userRoles, bool isAdministrator);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    Task<Hashtable> CheckPermisionBackHashtable(string sid, List<string> permNames);

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    Task<Hashtable> CheckPermisionBackHashtable(string sid, string account, List<string> permNames);

    /// <summary>
    /// 类型转换
    /// </summary>
    /// <param name="sidType"></param>
    /// <returns></returns>
    SIDTypeEnum GetSIDType(string sidType);
}
