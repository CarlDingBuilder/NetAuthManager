using NetAuthManager.Application.Params.Menus;
using NetAuthManager.Application.Sys.Params.Menus;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Entities;
using System.Collections;

namespace NetAuthManager.Application;

public interface IMenuResourceService
{
    /// <summary>
    /// 获得业务类型全路径
    /// 不包含根目录，即，不包含 BizMoudleRootRsid 对应的节点
    /// </summary>
    /// <param name="rsid"></param>
    /// <returns></returns>
    Task<string> GetFullPath(string rsid);

    /// <summary>
    /// 从全路径转为 RSID
    /// </summary>
    /// <param name="fullPath">资源全路径</param>
    /// <returns></returns>
    Task<string> GetRSID(string fullPath);

    /// <summary>
    /// 获取资源权限项
    /// </summary>
    Task<List<SysResourcePerm>> GetResourcePerms(string rsid);

    /// <summary>
    /// 获取资源权限项
    /// </summary>
    Task<List<string>> GetResourcePermNames(string rsid);

    /// <summary>
    /// 资源业务权限名称获取
    /// </summary>
    /// <param name="rsid"></param>
    /// <returns></returns>
    Task<List<string>> GetResourceBizPermNames(string rsid);

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="permName">权限名称</param>
    /// <returns></returns>
    Task<bool> CheckPermision(string rsid, string permName);

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="permNames">权限名称</param>
    /// <returns></returns>
    Task<List<bool>> CheckPermisions(string rsid, List<string> permNames);

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="perm">权限类型</param>
    /// <returns></returns>
    Task<bool> CheckPermision(string rsid, PermTypeEnum perm);

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="perms">权限类型</param>
    /// <returns></returns>
    Task<List<bool>> CheckPermisions(string rsid, PermTypeEnum[] perms);

    /// <summary>
    /// 获取资源业务权限
    /// </summary>
    Task<Dictionary<string, bool>> GetResourceBizPermisions(string rsid);

    /// <summary>
    /// 获取资源有权限的业务项
    /// </summary>
    Task<string[]> GetResourceBizAllowPermNames(string rsid);

    /// <summary>
    /// 获取当前权限模块有权限的业务项
    /// </summary>
    /// <returns></returns>
    Task<string[]> GetResourceBizAllowPermNames();

    /// <summary>
    /// 权限授权属性
    /// </summary>
    /// <returns></returns>
    public PermAuthorizeAttribute GetMethodPermAuthorize();
}
