using NetAuthManager.Application.Consts;
using NetAuthManager.Application.Handlers;
using NetAuthManager.Core.Common.Dtos;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Exceptions;
using NetAuthManager.Core.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetAuthManager.Application;

/// <summary>
/// 用户资源服务
/// </summary>
public class MenuResourceService : IMenuResourceService, ITransient
{
    #region 构造与注入

    private readonly IMenuService _menuService;
    private readonly IRoleService _roleService;
    private readonly IResourcePermService _resourcePermService;
    private readonly IResourcePermCommonService _resourcePermCommonService;

    public MenuResourceService(IResourcePermService resourcePermService, IResourcePermCommonService resourcePermCommonService, IMenuService menuService, IRoleService roleService)
    {
        _resourcePermService = resourcePermService;
        _resourcePermCommonService = resourcePermCommonService;
        _menuService = menuService;
        _roleService = roleService;
    }

    #endregion 构造与注入

    /// <summary>
    /// 业务划分在权限模块中的根节点RSID
    /// </summary>
    public const string MenuRootRsid = ConstRSIDs.Base.MenuRoot;

    /// <summary>
    /// 获得业务类型全路径
    /// 不包含根目录，即，不包含 BizMoudleRootRsid 对应的节点
    /// </summary>
    /// <param name="rsid"></param>
    /// <returns></returns>
    public async Task<string> GetFullPath(string rsid)
    {
        var loginUserInfo = LoginUserInfo.GetLoginUser();
        if (loginUserInfo == null)
            throw new CustomException("获取登录用户异常");

        var fullName = string.Empty;
        var allMenus = await _menuService.GetAllMenus();
        var resource = await _menuService.GetParentMenuBySIDs(rsid, allMenus);
        while (resource != null && rsid != MenuRootRsid)
        {
            if (string.IsNullOrEmpty(fullName))
                fullName = resource.Name;
            else
                fullName = resource.Name + "/" + fullName;

            if (resource.Parent != null)
                resource = resource.Parent;
            else
                resource = null;
        }
        return fullName;
    }

    /// <summary>
    /// 从全路径转为 RSID
    /// </summary>
    /// <param name="fullPath">资源全路径</param>
    /// <returns></returns>
    public async Task<string> GetRSID(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            throw new CustomException("请提供资源全路径");

        var loginUserInfo = LoginUserInfo.GetLoginUser();
        if (loginUserInfo == null)
            throw new CustomException("获取登录用户异常");

        // 匹配资源
        var resource = await _menuService.GetMenuByFullName(fullPath);
        if (resource == null)
            throw new CustomException("指定的资源全路径不存在");

        return resource.RSID;
    }

    /// <summary>
    /// 获取资源权限项
    /// </summary>
    public async Task<List<SysResourcePerm>> GetResourcePerms(string rsid)
    {
        if (string.IsNullOrEmpty(rsid))
            throw new CustomException("请提供资源 RSID");

        var loginUserInfo = LoginUserInfo.GetLoginUser();
        if (loginUserInfo == null)
            throw new CustomException("获取登录用户异常");

        // 匹配资源
        return await _resourcePermCommonService.GetPermisions(rsid);
    }

    /// <summary>
    /// 获取资源权限项
    /// </summary>
    public async Task<List<string>> GetResourcePermNames(string rsid)
    {
        var list = await GetResourcePerms(rsid);
        return list.Select(x => x.PermName).ToList();
    }

    /// <summary>
    /// 资源业务权限名称获取
    /// </summary>
    /// <param name="rsid"></param>
    /// <returns></returns>
    public async Task<List<string>> GetResourceBizPermNames(string rsid)
    {
        var list = await GetResourcePerms(rsid);
        return list.Where(x => x.PermName.StartsWith("$") || 
                               new[] {
                                   PermTypeEnum.Execute.ToString(),
                                   PermTypeEnum.ExecuteAll.ToString(),
                                   PermTypeEnum.ExecuteHandle.ToString()
                               }.Contains(x.PermName)
                         )
            .Select(x => x.PermName).ToList();
    }

    #region 检查权限

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="permName">权限名称</param>
    /// <returns></returns>
    public async Task<bool> CheckPermision(string rsid, string permName)
    {
        return await _resourcePermCommonService.CheckPermision(rsid, permName);
    }

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="permNames">权限名称</param>
    /// <returns></returns>
    public async Task<List<bool>> CheckPermisions(string rsid, List<string> permNames)
    {
        var loginUserInfo = LoginUserInfo.GetLoginUser();
        if (loginUserInfo == null)
            throw new CustomException("获取登录用户异常");

        return await _resourcePermCommonService.CheckPermision(rsid, permNames.ToList());
    }

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="perm">权限类型</param>
    /// <returns></returns>
    public async Task<bool> CheckPermision(string rsid, PermTypeEnum perm)
    {
        var permName = perm.ToString();
        return await _resourcePermCommonService.CheckPermision(rsid, permName);
    }

    /// <summary>
    /// 检查授权
    /// </summary>
    /// <param name="rsid">资源 RSID</param>
    /// <param name="perms">权限类型</param>
    /// <returns></returns>
    public async Task<List<bool>> CheckPermisions(string rsid, PermTypeEnum[] perms)
    {
        var list = perms.Select(p => p.ToString()).ToList();
        return await CheckPermisions(rsid, list);
    }

    /// <summary>
    /// 获取资源业务权限
    /// </summary>
    public async Task<Dictionary<string, bool>> GetResourceBizPermisions(string rsid)
    {
        // 业务权限名称
        var permNames = await GetResourceBizPermNames(rsid);

        // 判断访问权限
        var canArr = await CheckPermisions(rsid, permNames);
        if (canArr.Count != permNames.Count)
            throw new CustomException("校验权限失败！");

        // 字典
        var dictionary = new Dictionary<string, bool>();
        for (var index = 0; index < permNames.Count; index ++)
        {
            dictionary.Add(permNames[index], canArr[index]);
        }

        return dictionary;
    }

    /// <summary>
    /// 获取资源有权限的业务项
    /// </summary>
    public async Task<string[]> GetResourceBizAllowPermNames(string rsid)
    {
        // 业务权限名称
        var permNames = await GetResourceBizPermNames(rsid);

        // 判断访问权限
        var canArr = await CheckPermisions(rsid, permNames);
        if (canArr.Count != permNames.Count)
            throw new CustomException("校验权限失败！");

        // 筛选
        var endPerms = permNames.Where((permName, index) => canArr[index]).ToList();

        // 管理员权限
        var isAdmin = await _roleService.IsAdministratorLogin();
        if (isAdmin.IsBelong)
        {
            var execute = PermTypeEnum.Execute.ToString();
            var executeAll = PermTypeEnum.ExecuteAll.ToString();
            if (!endPerms.Contains(execute)) endPerms.Add(execute);
            if (!endPerms.Contains(executeAll)) endPerms.Add(executeAll);
        }

        return endPerms.ToArray();
    }

    /// <summary>
    /// 获取当前权限模块有权限的业务项
    /// </summary>
    /// <returns></returns>
    public async Task<string[]> GetResourceBizAllowPermNames()
    {
        //属性
        var permAuthorize = GetMethodPermAuthorize();
        if (permAuthorize == null) return new string[0];

        //权限全路径
        var permFullPath = permAuthorize.PermFullPath;
        if (string.IsNullOrEmpty(permFullPath)) return new string[0];

        //资源全路径
        var rsid = await GetRSID(permFullPath);
        if (string.IsNullOrEmpty(rsid)) return new string[0];

        //权限名称列表
        return await GetResourceBizAllowPermNames(rsid);
    }

    /// <summary>
    /// 权限授权属性
    /// </summary>
    /// <returns></returns>
    public PermAuthorizeAttribute GetMethodPermAuthorize()
    {
        StackTrace stackTrace = new StackTrace();

        try
        {
            var index = 2;
            while (index < 20)
            {
                var method = stackTrace.GetFrame(index).GetMethod();
                var perm = method.GetCustomAttribute<PermAuthorizeAttribute>();
                if (perm != null)
                {
                    return perm;
                }
                index++;
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    #endregion 检查权限
}
