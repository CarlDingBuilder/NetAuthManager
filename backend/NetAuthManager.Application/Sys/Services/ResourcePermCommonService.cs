using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using NetAuthManager.Application.Sys.Results.Resources;
using NetAuthManager.Application.Sys.Params.Resources;
using NetAuthManager.Core.Entities;
using Newtonsoft.Json.Linq;
using Mapster.Utils;
using Microsoft.IdentityModel.Logging;
using System.Runtime.CompilerServices;
using NetAuthManager.Core.Services;
using Microsoft.Extensions.Logging;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Consts;
using NetAuthManager.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
//
//using NetAuthManager.Core.User;
//using System.Diagnostics;
//using BPM.Client.Security;

namespace NetAuthManager.Application;

/// <summary>
/// 资源权限服务，PermType == "Module"
/// </summary>
public class ResourcePermCommonService : BaseService<SysResourcePerm>, IResourcePermCommonService, ITransient
{
    #region 构造及注入

    private readonly ILogger<ResourcePermCommonService> _logger;

    private readonly IUserLoginService _userLoginService;
    protected readonly IUserService _userService;
    protected readonly IRoleService _roleService;
    protected readonly IRoleGroupService _roleGroupService;
    protected readonly IRepository<SysResourcePerm> _resourceRepository;
    protected readonly IRepository<SysResourcePermAcl> _resourceAclRepository;

    public ResourcePermCommonService(ILogger<ResourcePermCommonService> logger, IServiceScopeFactory scopeFactory,
        IRoleService roleService, IRoleGroupService roleGroupService,
        IRepository<SysResourcePerm> resourceService, IRepository<SysResourcePermAcl> resourceAclService,
        IUserLoginService userLoginService, IUserService userService) : base(resourceService, scopeFactory)
    {
        _logger = logger;

        _userLoginService = userLoginService;
        _roleService = roleService;
        _roleGroupService = roleGroupService;
        _resourceRepository = resourceService;
        _resourceAclRepository = resourceAclService;
        _userService = userService;
    }

    #endregion 构造及注入

    /// <summary>
    /// 获取用户所有权限相关的 SID，包括用户、部门、角色
    /// 内部访问数据库有多线程处理，不会报错
    /// </summary>
    public async Task<List<ResourceBaseParam>> GetUserPermSIDs()
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        return await GetUserPermSIDs(loginUser.UserAccount);
    }

    /// <summary>
    /// 获取用户所有权限相关的 SID，包括用户、角色组、角色
    /// </summary>
    public async Task<List<ResourceBaseParam>> GetUserPermSIDs(string account)
    {
        if (AdminConst.AdminUsers.Contains(account)) return new List<ResourceBaseParam> { };

        //账号及角色SID
        var roleSIDs = await _roleService.GetRoleSIDsAsync(account);

        //用户
        var userSIDsTask = TryAsyncDoReturnDelegate<DefaultDbContext, string>(async (dbContext, serviceProvider) =>
        {
            return await _userService.GetSID(dbContext, account);
        });

        //根据账号及角色SID，获取角色组SID
        var roleGroupSIDsTask = TryAsyncDoReturnDelegate<DefaultDbContext, List<string>>(async (dbContext, serviceProvider) =>
        {
            return await _roleGroupService.GetRoleGroupSIDsAsync(dbContext, roleSIDs);
        });

        var roleGroupSIDs = await roleGroupSIDsTask;
        var userSid = await userSIDsTask;

        //用户
        var permUser = new ResourceBaseParam
        {
            SID = userSid,
            SIDType = SIDTypeEnum.UserSID.ToString()
        };
        //角色
        var permRoles = roleSIDs.Select(sid => new ResourceBaseParam
        {
            SID = sid,
            SIDType = SIDTypeEnum.RoleSID.ToString()
        });
        //角色组
        var permRoleGroups = roleGroupSIDs.Select(sid => new ResourceBaseParam
        {
            SID = sid,
            SIDType = SIDTypeEnum.RoleGroupSID.ToString()
        });

        var list = new List<ResourceBaseParam>();
        list.Add(permUser);
        list.AddRange(permRoles);
        list.AddRange(permRoleGroups);
        //list.AddRange(permOUs);
        //list.AddRange(permJobLevels);
        //list.AddRange(permLeaderTitles);
        return list;
    }

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public async Task<List<SysResourcePerm>> GetPermisions(string sid)
    {
        //对应的模块操作
        List<SysResourcePerm> resources = await (from res in _resourceRepository.Entities
                                                 where res.SID == sid
                                                 orderby res.OrderIndex, res.Id
                                                 select res).ToListAsync();
        if (resources == null)
            throw new Exception("资源未找到！");
        return resources;
    }

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <param name="sids"></param>
    /// <returns></returns>
    public async Task<List<SysResourcePerm>> GetPermisions(List<string> sids)
    {
        return await GetPermisions(_resourceRepository.Entities, sids);
    }

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    public async Task<List<SysResourcePerm>> GetPermisions(DefaultDbContext dbContext, List<string> sids)
    {
        return await GetPermisions(dbContext.ResourcePerms, sids);
    }
    private async Task<List<SysResourcePerm>> GetPermisions(DbSet<SysResourcePerm> resourcePerms, List<string> sids)
    {
        //对应的模块操作
        List<SysResourcePerm> resources = await (from res in resourcePerms
                                                 where sids.Contains(res.SID)
                                                 orderby res.OrderIndex, res.Id
                                                 select res).ToListAsync();
        return resources;
    }

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    /// <param name="sids"></param>
    /// <returns></returns>
    public async Task<List<SysResourcePermAcl>> GetPermisionAcls(List<string> sids)
    {
        return await GetPermisionAcls(_resourceAclRepository.Entities, sids);
    }

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    public async Task<List<SysResourcePermAcl>> GetPermisionAcls(DefaultDbContext dbContext, List<string> sids)
    {
        return await GetPermisionAcls(dbContext.ResourcePermAcls, sids);
    }

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    private async Task<List<SysResourcePermAcl>> GetPermisionAcls(DbSet<SysResourcePermAcl> resourcePerms, List<string> sids)
    {
        //对应的模块操作
        //权限判断
        var acls = await (from entity in resourcePerms
                          where sids.Contains(entity.SID)
                          select entity).ToListAsync();
        return acls;
    }

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public async Task<List<SysResourcePermAcl>> GetPermisionAcls(string sid)
    {
        //对应的模块操作
        //权限判断
        var acls = await (from entity in _resourceAclRepository.Entities
                          where entity.SID == sid
                          select entity).ToListAsync();
        return acls;
    }

    /// <summary>
    /// 判断是否有权限
    /// </summary>
    /// <param name="permName"></param>
    /// <param name="perms"></param>
    /// <param name="acls"></param>
    /// <returns></returns>
    public bool CheckPermision(string permName, IEnumerable<SysResourcePerm> perms, IEnumerable<SysResourcePermAcl> acls)
    {
        if (!perms.Any())
        {
            return true;
        }
        else if (!perms.Any(p => p.PermName == permName))
        {
            if (permName == PermTypeEnum.Execute.ToString())
            {
                return true;
            }
            else return false;
        }

        //没有配置就是没有权限
        if (acls.Count() == 0) return false;

        var isEveryoneHavePerm = false; //所有人角色具有权限
        var isElseHavePerm = false; //非所有人角色具备权限
        var isElseIsSet = false; //非所有人角色是否有设置
        var groups = acls.GroupBy(acl => acl.RoleParam1);
        foreach (var group in groups)
        {
            //当前有权限
            bool havePermAclInner = false;
            var aclInherited = group.FirstOrDefault(a => a.Inherited);
            var aclNoInherited = group.FirstOrDefault(a => !a.Inherited);

            if (aclInherited != null)
            {
                if (aclInherited.AllowPerms == null) aclInherited.AllowPerms = string.Empty;
                if (aclInherited.DenyPerms == null) aclInherited.DenyPerms = string.Empty;
                if (aclInherited.AllowPerms.Split(',').Contains(permName)) havePermAclInner = true;
                if (aclInherited.DenyPerms.Split(',').Contains(permName)) havePermAclInner = false;
            }
            if (aclNoInherited != null)
            {
                if (aclNoInherited.AllowPerms == null) aclNoInherited.AllowPerms = string.Empty;
                if (aclNoInherited.DenyPerms == null) aclNoInherited.DenyPerms = string.Empty;
                if (aclNoInherited.AllowPerms.Split(',').Contains(permName)) havePermAclInner = true;
                if (aclNoInherited.DenyPerms.Split(',').Contains(permName)) havePermAclInner = false;
            }

            if (ConstRSIDs.RoleSID.Everyone.ToString().Equals(group.Key))
            {
                isEveryoneHavePerm = havePermAclInner;
            }
            else
            {
                isElseIsSet = true;

                //有一个有权限则设置为否
                if (havePermAclInner) isElseHavePerm = true;
            }
        }

        //没有设置其他权限的，按照所有人权限
        if (!isElseIsSet) return isEveryoneHavePerm;

        //设置有权限的，按照设置的权限
        return isElseHavePerm;
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, bool>> CheckPermisionBack(string sid, List<string> permNames)
    {
        var list = await CheckPermision(sid, permNames);
        var dic = new Dictionary<string, bool>();
        for (var i = 0; i < list.Count; i++)
        {
            dic.Add(permNames[i], list[i]);
        }
        return dic;
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, bool>> CheckPermisionBack(string sid, string account, List<string> permNames)
    {
        var list = await CheckPermision(sid, account, permNames);
        var dic = new Dictionary<string, bool>();
        for (var i = 0; i < list.Count; i++)
        {
            dic.Add(permNames[i], list[i]);
        }
        return dic;
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public async Task<bool> CheckExecutePermision(string sid)
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        return await CheckExecutePermision(sid, loginUser.UserAccount);
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <returns></returns>
    public async Task<bool> CheckExecutePermision(string sid, string account)
    {
        return await CheckPermision(sid, account, PermTypeEnum.Execute.ToString());
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckPermision(string sid, PermTypeEnum perm)
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        return await CheckPermision(sid, loginUser.UserAccount, perm);
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckPermision(string sid, string account, PermTypeEnum perm)
    {
        return await CheckPermision(sid, account, perm.ToString());
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permName"></param>
    /// <returns></returns>
    public async Task<bool> CheckPermision(string sid, string permName)
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        return await CheckPermision(permName, loginUser.UserAccount, permName);
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permName"></param>
    /// <returns></returns>
    public async Task<bool> CheckPermision(string sid, string account, string permName)
    {
        //对应的模块操作
        var perms = await (from res in _resourceRepository.Entities
                           where res.SID == sid
                           select res).ToListAsync();

        //获取当前用户角色、角色组、用户的 SID
        var userPerm = await GetUserPermSIDs(account);

        //权限判断
        var acls = await (from entity in _resourceAclRepository.Entities
                          where entity.SID == sid
                          select entity).ToListAsync();
        //权限判断
        acls = acls.Where(entity => userPerm.Any(perm => perm.SID == entity.RoleParam1 && perm.SIDType == entity.RoleType)).ToList();
        return CheckPermision(permName, perms, acls);
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    public async Task<List<bool>> CheckPermision(string sid, List<string> permNames)
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        return await CheckPermision(sid, loginUser.UserAccount, permNames);
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    public async Task<List<bool>> CheckPermision(string sid, string account, List<string> permNames)
    {
        if (permNames == null) return null;

        //对应的模块操作
        var perms = await (from res in _resourceRepository.Entities
                           where res.SID == sid
                           select res).ToListAsync();

        //获取当前用户角色、角色组、用户的 SID
        var userPerm = await GetUserPermSIDs(account);
        var isAdministrator = await _roleService.IsAdministrator(account);

        //权限判断
        var acls = await (from entity in _resourceAclRepository.Entities
                          where entity.SID == sid
                          select entity).ToListAsync();

        acls = (from entity in acls
                where userPerm.Any(perm => perm.SID == entity.RoleParam1 && perm.SIDType == entity.RoleType)
                select entity).ToList();

        var list = permNames.Select(permName => isAdministrator || CheckPermision(permName, perms, acls)).ToList();
        return list;
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    public async Task<Hashtable> CheckPermisionBackHashtable(string sid, List<string> permNames)
    {
        //获取当前登录用户
        var list = await CheckPermision(sid, permNames);
        var hashtable = new Hashtable();
        for (var i = 0; i < permNames.Count; i++)
        {
            hashtable.Add(permNames[i], list[i]);
        }
        return hashtable;
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="account"></param>
    /// <param name="permNames"></param>
    /// <returns></returns>
    public async Task<Hashtable> CheckPermisionBackHashtable(string sid, string account, List<string> permNames)
    {
        //获取当前登录用户
        var list = await CheckPermision(sid, account, permNames);
        var hashtable = new Hashtable();
        for (var i = 0; i < permNames.Count; i++)
        {
            hashtable.Add(permNames[i], list[i]);
        }
        return hashtable;
    }

    /// <summary>
    /// 检查资源对应的操作是否有权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="userRoles"></param>
    /// <param name="isAdministrator"></param>
    /// <returns></returns>
    public async Task<Hashtable> CheckPermisionBack(string sid, List<ResourceBaseParam> userRoles, bool isAdministrator)
    {
        //资源的权限
        var perms = await GetPermisions(sid);
        var acls = await GetPermisionAcls(sid);
        acls = (from entity in acls
                where userRoles.Any(perm => perm.SID == entity.RoleParam1 && perm.SIDType == entity.RoleType)
                select entity).ToList();

        //操作权限
        var jsonPerm = new Hashtable();

        //权限名称
        var permNames = new List<string>();
        foreach (var resourcePerm in perms)
        {
            if (resourcePerm.PermType == ConstNames.ResourcePermTypeValue.Module)
                permNames.Add(resourcePerm.PermName);
        }

        //资源权限
        foreach (var permName in permNames)
        {
            jsonPerm[permName] = isAdministrator || CheckPermision(permName, perms, acls);
        }
        return jsonPerm;
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    /// <param name="sidType"></param>
    /// <returns></returns>
    public SIDTypeEnum GetSIDType(string sidType)
    {
        SIDTypeEnum type;
        if (!System.Enum.TryParse(sidType, out type))
            throw new Exception($"无效的资源类型 “{sidType}”！");
        return type;
    }

    ///// <summary>
    ///// 获取资源有权限的业务项
    ///// </summary>
    //public string[] GetResourceBizAllowPermNames(string rsid)
    //{
    //    // 业务权限名称
    //    var permNames = GetResourceBizPermNames(rsid);

    //    // 判断访问权限
    //    var canArr = CheckPermisions(rsid, permNames);
    //    if (canArr.Length != permNames.Length)
    //        throw new CustomException("校验权限失败！");

    //    // 筛选
    //    return permNames.Where((permName, index) => canArr[index]).ToArray();
    //}

    ///// <summary>
    ///// 获取当前权限模块有权限的业务项
    ///// </summary>
    ///// <returns></returns>
    //public string[] GetResourceBizAllowPermNames()
    //{
    //    //属性
    //    var permAuthorize = GetMethodPermAuthorize();
    //    if (permAuthorize == null) return new string[0];

    //    //权限全路径
    //    var permFullPath = permAuthorize.PermFullPath;
    //    if (string.IsNullOrEmpty(permFullPath)) return new string[0];

    //    //资源全路径
    //    var rsid = GetRSID(permFullPath);
    //    if (string.IsNullOrEmpty(rsid)) return new string[0];

    //    //权限名称列表
    //    return GetResourceBizAllowPermNames(rsid);
    //}

    ///// <summary>
    ///// 权限授权属性
    ///// </summary>
    ///// <returns></returns>
    //public PermAuthorizeAttribute GetMethodPermAuthorize()
    //{
    //    StackTrace stackTrace = new StackTrace();

    //    try
    //    {
    //        var index = 2;
    //        while (index < 15)
    //        {
    //            var method = stackTrace.GetFrame(index).GetMethod();
    //            var perm = method.GetCustomAttribute<PermAuthorizeAttribute>();
    //            if (perm != null)
    //            {
    //                return perm;
    //            }
    //            index++;
    //        }
    //        return null;
    //    }
    //    catch (Exception)
    //    {
    //        return null;
    //    }
    //}

    ///// <summary>
    ///// 资源业务权限名称获取
    ///// </summary>
    ///// <param name="rsid"></param>
    ///// <returns></returns>
    //public string[] GetResourceBizPermNames(string rsid)
    //{
    //    var list = GetResourcePerms(rsid);
    //    return list.Where(x => x.PermName.StartsWith("$") ||
    //                           new[] {
    //                               PermTypeEnum.ExecuteAll.ToString(),
    //                               PermTypeEnum.ExecuteHandle.ToString()
    //                           }.Contains(x.PermName)
    //                     )
    //        .Select(x => x.PermName).ToArray();
    //}

    ///// <summary>
    ///// 获取资源权限项
    ///// </summary>
    //public UserResourcePermisionCollection GetResourcePerms(string rsid)
    //{
    //    if (string.IsNullOrEmpty(rsid))
    //        throw new CustomException("请提供资源 RSID");

    //    var loginUserInfo = LoginUserInfo.GetLoginUser();
    //    if (loginUserInfo == null)
    //        throw new CustomException("获取登录用户异常");

    //    // 匹配资源
    //    using (BPMConnection cn = new BPMConnection())
    //    {
    //        cn.OpenWithToken(loginUserInfo.Token);

    //        var userResource = UserResource.Open(cn, rsid);
    //        if (userResource == null)
    //            throw new CustomException("指定的资源不存在");

    //        // 匹配资源
    //        return UserResource.GetPermisions(cn, rsid);
    //    }
    //}

    ///// <summary>
    ///// 检查授权
    ///// </summary>
    ///// <param name="rsid">资源 RSID</param>
    ///// <param name="permNames">权限名称</param>
    ///// <returns></returns>
    //public bool[] CheckPermisions(string rsid, string[] permNames)
    //{
    //    var loginUserInfo = LoginUserInfo.GetLoginUser();
    //    if (loginUserInfo == null)
    //        throw new CustomException("获取登录用户异常");

    //    using (BPMConnection cn = new BPMConnection())
    //    {
    //        cn.OpenWithToken(loginUserInfo.Token);
    //        return CheckPermisions(cn, rsid, permNames);
    //    }
    //}

}
