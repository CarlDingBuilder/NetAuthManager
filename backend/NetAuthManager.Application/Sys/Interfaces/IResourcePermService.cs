using NetAuthManager.Application.Sys.Params.Resources;
using NetAuthManager.Application.Sys.Results.Resources;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 菜单相关接口
/// </summary>
public interface IResourcePermService
{
    /// <summary>
    /// 获取资源树
    /// </summary>
    /// <returns></returns>
    Task<List<ResurceNodeItem>> GetResources(GetResourcesParam param);

    /// <summary>
    /// 获取资源角色权限信息
    /// 仅角色信息
    /// </summary>
    /// <param name="sid"></param>
    Task<List<ResourcePermRoleItemDto_RoleItem>> GetResourcePermRoles(string sid);

    /// <summary>
    /// 获取资源信息
    /// </summary>
    /// <returns></returns>
    Task<ResourceItemDto> GetResourceInfo(string sid, Core.Common.Enums.SIDTypeEnum? sidType);

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    Task<List<ResourcePermItemDto>> GetResourcePerms(string sid, Core.Common.Enums.SIDTypeEnum? sidType);

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <returns></returns>
    Task<List<ResourcePermTypeDto>> GetResourcePermTypes();

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <returns></returns>
    Task<List<ResourcePermRecordDto>> GetResourcePermLog();

    /// <summary>
    /// 保存资源操作项列表
    /// </summary>
    /// <param name="param"></param>
    Task SaveResourcePerms(ResourcePermSaveParam param);

    /// <summary>
    /// 获取资源角色权限信息
    /// 仅权限
    /// </summary>
    /// <param name="param"></param>
    Task<List<ResourcePermAclsDto>> GetResourcePermRoleAcls(ResourcePermAclsGetParam param);

    /// <summary>
    /// 添加资源角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task AddResourcePermRole(ResourcePermRoleAddParam param);

    /// <summary>
    /// 删除资源角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task DeleteResourcePermRole(ResourcePermRoleDeleteParam param);

    /// <summary>
    /// 保存资源角色权限列表
    /// </summary>
    /// <param name="param"></param>
    Task SaveAcls(ResourcePermAclsSaveParam param);
}
