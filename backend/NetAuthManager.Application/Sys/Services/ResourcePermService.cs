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
using NetAuthManager.Application.Sys.Results.Users;
using Newtonsoft.Json;
using NetAuthManager.Core.Common.Extended;

namespace NetAuthManager.Application;

/// <summary>
/// 资源权限服务，PermType == "Module"
/// </summary>
public class ResourcePermService : BaseService<SysResourcePerm>, IResourcePermService, ITransient
{
    #region 构造及注入

    private readonly ILogger<ResourcePermService> _logger;

    private readonly IUserLoginService _userLoginService;
    protected readonly IUserService _userService;
    protected readonly IMenuService _menuService;
    protected readonly IRoleService _roleService;
    protected readonly IRoleGroupService _roleGroupService;
    protected readonly IResourcePermCommonService _resourcePermCommonService;

    protected readonly IRepository<SysMenu> _menuRepository;
    protected readonly IRepository<SysResourcePerm> _resourceRepository;
    protected readonly IRepository<SysResourcePermAcl> _resourceAclRepository;
    protected readonly EnumService _enumService;

    public ResourcePermService(ILogger<ResourcePermService> logger, IServiceScopeFactory scopeFactory,
        IMenuService menuService, IRepository<SysMenu> menuRepository, IRoleService roleService, 
        IRoleGroupService roleGroupService, IResourcePermCommonService resourcePermCommonService,
        IRepository<SysResourcePerm> resourceService, IRepository<SysResourcePermAcl> resourceAclService,
        IUserLoginService userLoginService, IUserService userService, EnumService enumService) : base(resourceService, scopeFactory)
    {
        _logger = logger;

        _userLoginService = userLoginService;
        _menuService = menuService;
        _menuRepository = menuRepository;
        _roleService = roleService;
        _roleGroupService = roleGroupService;
        _resourcePermCommonService = resourcePermCommonService;
        _resourceRepository = resourceService;
        _resourceAclRepository = resourceAclService;
        _userService = userService;
        _enumService = enumService;
    }

    #endregion 构造及注入

    /// <summary>
    /// 获取资源树
    /// </summary>
    public async Task<List<ResurceNodeItem>> GetResources(GetResourcesParam param)
    {
        //菜单
        var menus = await _menuService.GetAllMenus(new Params.Menus.GetMenusParam
        {
            PCode = "default"
        });
        var treeMenu = _menuService.BuildMenuTree(menus);
        var treeMenuResource = BuildResource(treeMenu);

        return new List<ResurceNodeItem>
        {
            //菜单资源
            GetMenuRootResurceNode(treeMenuResource),
            //资源库
            //GetRootResurceNode(new List<ResurceNodeItem>())
        };
    }
    protected List<ResurceNodeItem> BuildResource(List<SysMenu> src)
    {
        var list = new List<ResurceNodeItem>();
        foreach (var menu in src)
        {
            if (!menu.IsMenu) continue;
            if (!string.IsNullOrEmpty(menu.ConfigJson))
            {
                var json = JsonConvert.DeserializeObject<JObject>(menu.ConfigJson);
                var meta = json?["meta"] as JToken;
                if (meta != null)
                {
                    var hidden = meta.GetBool("hidden", false);
                    if (hidden) continue;
                }
            }
            list.Add(BuildResource(menu));
        }
        return list;
    }
    protected ResurceNodeItem BuildResource(SysMenu menu)
    {
        //节点
        var model = new ResurceNodeItem(menu);

        //子菜单
        if (menu.Children != null && menu.Children.Count > 0) model.Children = BuildResource(menu.Children);

        return model;
    }

    /// <summary>
    /// 获取资源信息
    /// </summary>
    /// <returns></returns>
    public async Task<ResourceItemDto> GetResourceInfo(string sid, SIDTypeEnum? sidType)
    {
        if (string.IsNullOrEmpty(sid))
        {
            throw new Exception("请提供要访问的资源的 SID！");
        }

        switch (sidType.Value)
        {
            case SIDTypeEnum.MenuSID:
                if (sid == ConstRSIDs.Base.MenuRoot)
                {
                    return GetMenuRootResurceNode();
                }
                else
                {
                    var model = await (from menu in _menuRepository.Entities
                                       where menu.RSID == sid
                                       select menu).FirstOrDefaultAsync();
                    if (model == null)
                        throw new Exception("菜单资源未找到！");

                    return new ResourceItemDto(model);
                }
            case SIDTypeEnum.ResourceSID:
                if (sid == ConstRSIDs.Base.ResourceRoot)
                {
                    return GetRootResurceNode();
                }
                else
                {
                    throw new Exception("待开发！");
                }
            //case SIDType.FileLevelSID:
            //    return await GetFileLevelInfo(sid);
            default:
                throw new Exception("暂不支持其他类型的资源！");
        }
    }

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="sidType"></param>
    /// <returns></returns>
    public async Task<List<ResourcePermItemDto>> GetResourcePerms(string sid, SIDTypeEnum? sidType)
    {
        //对应的模块操作
        var list = await GetResourcePermsInner(sid, sidType);

        //对应的操作权限获取
        var acls = await (from acl in _resourceAclRepository.Entities
                          where acl.SID == sid
                          orderby acl.OrderIndex, acl.Id
                          select acl).ToListAsync();

        var resources = (from res in list select new ResourcePermItemDto(res)
        {
            Roles = GetResourcePermsRoles(res, acls)
        }).ToList();
        return resources;
    }

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<ResourcePermTypeDto>> GetResourcePermTypes()
    {
        return await Task.Run(() =>
        {
            //对应的模块操作
            var permTypes = new List<ResourcePermTypeDto>()
            {
                new ResourcePermTypeDto
                {
                    Label = ConstNames.ResourcePermTypeName.Module,
                    Value = ConstNames.ResourcePermTypeValue.Module
                },
                new ResourcePermTypeDto
                {
                    Label = ConstNames.ResourcePermTypeName.Record,
                    Value = ConstNames.ResourcePermTypeValue.Record
                }
            };

            return permTypes;
        });
    }

    /// <summary>
    /// 获取资源操作项列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<ResourcePermRecordDto>> GetResourcePermLog()
    {
        var logs = await (from entiy in _resourceRepository.Entities
                          orderby entiy.OrderIndex, entiy.Id
                          select new ResourcePermRecordDto
                          {
                              PermName = entiy.PermName,
                              PermDisplayName = entiy.PermDisplayName
                          }).Distinct().ToListAsync();

        return logs;
    }

    ///// <summary>
    ///// 获取资源结构
    ///// 特殊情况：菜单项时，返回所有父级及当期的资源信息
    ///// </summary>
    ///// <returns></returns>
    //public async Task<List<ResourceItemDto>> GetResourceStruct(string sid, SIDType? sidType)
    //{
    //    if (string.IsNullOrEmpty(sid))
    //    {
    //        throw new Exception("请提供要访问的资源的 SID！");
    //    }

    //    switch(sidType.Value)
    //    {
    //        case SIDType.MenuSID:
    //            return await GetMenuStruct(sid);
    //        case SIDType.FileLevelSID:
    //            return await GetFileLevelStruct(sid);
    //        default:
    //            throw new Exception("暂不支持其他类型的资源！");

    //    }
    //}

    /// <summary>
    /// 获取资源角色权限信息
    /// 仅角色信息，这里的角色是广义的角色
    /// </summary>
    /// <param name="sid"></param>
    public async Task<List<ResourcePermRoleItemDto_RoleItem>> GetResourcePermRoles(string sid)
    {
        //对应的操作权限获取
        var acls = await (from acl in _resourceAclRepository.Entities
                          where acl.SID == sid
                          orderby acl.OrderIndex, acl.Id
                          select acl).ToListAsync();

        var list = new List<ResourcePermRoleItemDto_RoleItem>();
        foreach (var acl in acls)
        {
            var lastAcl = list.FirstOrDefault(a => a.SID == acl.RoleParam1);
            if (lastAcl == null)
            {
                list.Add(new ResourcePermRoleItemDto_RoleItem
                {
                    Name = GetMemberDisplayName(acl.RoleParam1, acl.RoleType),
                    SID = acl.RoleParam1,
                    SIDType = acl.RoleType,
                    Inherited = acl.Inherited
                });
            }
            else if (acl.Inherited && !lastAcl.Inherited)
            {
                lastAcl.Inherited = true;
            }
        }

        if (!list.Any(item => item.SID == ConstRSIDs.RoleSID.Administrators))
        {
            list.Insert(0, new ResourcePermRoleItemDto_RoleItem
            {
                Name = ConstNames.RoleCode.Administrators,
                SID = ConstRSIDs.RoleSID.Administrators,
                SIDType = SIDTypeEnum.RoleSID.ToString(),
                Inherited = true
            });
        }
        return list;
    }

    /// <summary>
    /// 保存资源操作项列表
    /// </summary>
    /// <param name="param"></param>
    public async Task SaveResourcePerms(ResourcePermSaveParam param)
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //权限
        var permNames = param.Perms.Select(p => p.PermName).ToList();
        if (permNames.Count != permNames.Distinct().Count())
            throw new Exception("不能存在相同的权限项！");

        //Perm 表
        List<SysResourcePerm> permsDelete; //需要删除的列表
        List<SysResourcePerm> permsUpdate = new List<SysResourcePerm>(); //需要更新的列表
        List<SysResourcePerm> permsInsert = new List<SysResourcePerm>(); //需要新增的列表
        List<SysResourcePerm> permsOld = await (from perm in _resourceRepository.Entities
                                                where perm.SID == param.SID
                                                select perm).ToListAsync(); //现存的列表
        permsDelete = (from perm in permsOld
                       where !permNames.Contains(perm.PermName)
                       select perm).ToList();

        var orderIndex = 0;
        foreach (var perm in param.Perms)
        {
            //排序
            perm.OrderIndex = ++orderIndex;

            var permTemp = permsOld.FirstOrDefault(p => p.PermName == perm.PermName);
            if (permTemp == null)
            {
                //构建插入
                permsInsert.Add(new SysResourcePerm()
                {
                    SID = param.SID,
                    PermName = perm.PermName,
                    PermDisplayName = perm.PermDisplayName,
                    PermType = perm.PermType,
                    OrderIndex = perm.OrderIndex,
                    CreatorAccount = loginUser.UserAccount,
                    CreatorName = loginUser.UserName
                });
            }
            else
            {
                //构建更新
                permTemp.PermDisplayName = perm.PermDisplayName;
                permTemp.PermType = perm.PermType;
                permTemp.OrderIndex = perm.OrderIndex;
                permsUpdate.Add(permTemp);
            }
        }

        //PermAcl 表
        var permAclsUpdate = new List<SysResourcePermAcl>();
        var acls = await (from acl in _resourceAclRepository.Entities
                          where acl.SID == param.SID
                          select acl).ToListAsync();
        foreach (var acl in acls)
        {
            var oldAllowPerms = acl.AllowPerms ?? string.Empty;
            var oldDenyPerms = acl.DenyPerms ?? string.Empty;
            var permNameAllowPerms = oldAllowPerms.Split(",");
            var permNameDenyPerms = oldDenyPerms.Split(",");
            var newAllowPerms = string.Join(",", permNameAllowPerms.Intersect(permNames));
            var newDenyPerms = string.Join(",", permNameDenyPerms.Intersect(permNames));

            if (oldAllowPerms != newAllowPerms || oldDenyPerms != newDenyPerms)
            {
                acl.AllowPerms = newAllowPerms;
                acl.DenyPerms = newDenyPerms;
                permAclsUpdate.Add(acl);
            }
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            //Perm 表
            if (permsInsert.Count > 0) await _resourceRepository.InsertAsync(permsInsert);
            if (permsDelete.Count > 0) await _resourceRepository.DeleteAsync(permsDelete);
            if (permsUpdate.Count > 0) await _resourceRepository.UpdateAsync(permsUpdate);

            //PermAcl 表，更新没有的
            if (permAclsUpdate.Count > 0) await _resourceAclRepository.UpdateAsync(permAclsUpdate);
        });
    }

    /// <summary>
    /// 获取指定资源的指定角色的权限信息
    /// 仅权限
    /// </summary>
    /// <param name="param"></param>
    public async Task<List<ResourcePermAclsDto>> GetResourcePermRoleAcls(ResourcePermAclsGetParam param)
    {
        //格式化要保存的SID
        //param.RoleSID = FormatAclSID(param.RoleSID, param.RoleSIDType);

        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //对应的模块操作
        var list = await GetResourcePermsInner(param.SID, _resourcePermCommonService.GetSIDType(param.SIDType));

        //对应的操作权限获取
        var acls = await (from acl in _resourceAclRepository.Entities
                          where acl.SID == param.SID && acl.RoleParam1 == param.RoleSID
                          select acl).ToListAsync();

        var resourceAcls = new List<ResourcePermAclsDto>();
        foreach (var resource in list)
        {
            //操作信息
            var perm = new ResourcePermAclsDto()
            {
                PermName = resource.PermName,
                PermDisplayName = resource.PermDisplayName,
                PermType = resource.PermType
            };

            //管理员
            if (param.RoleSID == ConstRSIDs.RoleSID.Administrators)
            {
                perm.AllowPermision = true;
                perm.IsAllowInherited = true;
                perm.DenyPermision = false;
                perm.IsDenyInherited = true;
            }
            else
            {
                //权限信息
                var allowPerms = acls.Where(acl => acl.AllowPerms != null && acl.AllowPerms.Split(",").Contains(resource.PermName));
                var denyPerms = acls.Where(acl => acl.DenyPerms != null && acl.DenyPerms.Split(",").Contains(resource.PermName));
                if (allowPerms.Any(a => a.Inherited))
                {
                    perm.AllowPermision = true;
                    perm.IsAllowInherited = true;
                }
                else if (allowPerms.Any(a => !a.Inherited))
                {
                    perm.AllowPermision = true;
                    perm.IsAllowInherited = false;
                }
                if (denyPerms.Any(a => a.Inherited))
                {
                    perm.DenyPermision = true;
                    perm.IsDenyInherited = true;
                }
                else if (denyPerms.Any(a => !a.Inherited))
                {
                    perm.DenyPermision = true;
                    perm.IsDenyInherited = false;
                }
            }

            resourceAcls.Add(perm);
        }

        return resourceAcls;
    }

    /// <summary>
    /// 添加资源角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task AddResourcePermRole(ResourcePermRoleAddParam param)
    {
        if (param == null)
            throw new Exception("参数不能为空！");

        if (string.IsNullOrEmpty(param.RoleSID))
            throw new Exception("角色 RoleSID 不能为空！");

        if (string.IsNullOrEmpty(param.SID))
            throw new Exception("资源 SID 不能为空！");

        //管理员直接跳过，因为默认认为存在管理员
        if (param.RoleSID == ConstRSIDs.RoleSID.Administrators)
            return;

        //格式化要保存的SID
        //param.RoleSID = FormatAclSID(param.RoleSID, param.RoleSIDType);

        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //检查
        var oldAcls = await (from acl in _resourceAclRepository.Entities
                             where acl.SID == param.SID
                             select acl).ToListAsync();
        if (oldAcls.Any(acl => acl.RoleParam1 == param.RoleSID && acl.RoleType == param.RoleSIDType))
            throw new Exception("已经存在相同的授权对象，无需重复添加！");

        //类型
        var sidType = _resourcePermCommonService.GetSIDType(param.SIDType);

        //排序，不存在添加 1 的情况，如果不存在任何权限时，默认应该有一个管理员的权限，所以从 2 开始
        var maxOrderIndex = oldAcls.Count == 0 ? 1 : oldAcls.Max(acl => acl.OrderIndex);
        var shouldInserts = new List<SysResourcePermAcl>();
        var shouldDeletes = new List<SysResourcePermAcl>();
        var shouldUpdates = new List<SysResourcePermAcl>();
        var entity = new SysResourcePermAcl()
        {
            SID = param.SID,
            AllowPerms = string.Empty,
            DenyPerms = string.Empty,
            RoleType = param.RoleSIDType,
            RoleParam1 = param.RoleSID,
            Inherited = false,
            OrderIndex = (param.RoleSID == ConstRSIDs.RoleSID.Administrators || param.RoleSID == ConstRSIDs.RoleSID.Everyone) ? 1 : ++maxOrderIndex,
            CreatorAccount = loginUser.UserAccount,
            CreatorName = loginUser.UserName
        };
        shouldInserts.Add(entity);

        //如果是菜单的情况，存在层级结构，需要继承
        if (sidType == SIDTypeEnum.MenuSID)
        {
            var parentAcls = new List<SysResourcePermAcl>
            {
                CopyTo(entity)
            };
            await BuildChildMenuAcl(param.SID, parentAcls, shouldInserts, shouldDeletes, shouldUpdates, loginUser);
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            if (shouldInserts.Count > 0)
                await _resourceAclRepository.InsertAsync(shouldInserts);
            if (shouldDeletes.Count > 0)
                await _resourceAclRepository.DeleteAsync(shouldDeletes);
            if (shouldUpdates.Count > 0)
                await _resourceAclRepository.UpdateAsync(shouldUpdates);
        });
    }

    /// <summary>
    /// 删除资源角色，因为从上级继承是不可以删除的，所以删除当前说明上级没有继承下来
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteResourcePermRole(ResourcePermRoleDeleteParam param)
    {
        //格式化要保存的SID
        //param.RoleSID = FormatAclSID(param.RoleSID, param.RoleSIDType);

        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //检查
        var oldAcls = await (from acl in _resourceAclRepository.Entities
                             where acl.SID == param.SID &&
                                   acl.RoleParam1 == param.RoleSID &&
                                   acl.RoleType == param.RoleSIDType
                             select acl).ToListAsync();

        var aclInserts = new List<SysResourcePermAcl>();
        var aclDeletes = new List<SysResourcePermAcl>();
        var aclUpdates = new List<SysResourcePermAcl>();
        aclDeletes.AddRange(oldAcls);

        //继承的资源
        //如果是菜单的情况，存在层级结构，需要继承
        var sidType = _resourcePermCommonService.GetSIDType(param.SIDType);
        if (sidType == SIDTypeEnum.MenuSID)
        {
            //当前菜单的父级菜单
            await DeleteChildMenuRoleAcl(param.SID, param.RoleSID, param.RoleSIDType, null, aclInserts, aclDeletes, aclUpdates, loginUser);
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            if (aclInserts.Count > 0)
                await _resourceAclRepository.InsertAsync(aclInserts);
            if (aclDeletes.Count > 0)
                await _resourceAclRepository.DeleteAsync(aclDeletes);
            if (aclUpdates.Count > 0)
                await _resourceAclRepository.UpdateAsync(aclUpdates);
        });
    }

    /// <summary>
    /// 保存资源角色权限列表
    /// 当前级别的优先级高于父级继承来的
    /// </summary>
    /// <param name="param"></param>
    public async Task SaveAcls(ResourcePermAclsSaveParam param)
    {
        if (string.IsNullOrEmpty(param.SID))
            throw new Exception("请提供需要维护权限的资源 SID");

        //没有任何权限信息直接结束
        if (param.Acls == null || param.Acls.Count == 0) return;

        //格式化要保存的SID
        //FormatAclSID(param.Acls);

        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //当前资源
        var aclInserts = new List<SysResourcePermAcl>();
        var aclDeletes = new List<SysResourcePermAcl>();
        var aclUpdates = new List<SysResourcePermAcl>();

        //更新后的权限
        var aclCurrent = new List<SysResourcePermAcl>();

        //现有权限
        var oldAcls = await (from entity in _resourceAclRepository.Entities
                             where entity.SID == param.SID
                             select entity).ToListAsync();
        foreach (var acl in param.Acls)
        {
            if (acl.AllowPermision == null) acl.AllowPermision = new List<string>();
            if (acl.DenyPermision == null) acl.DenyPermision = new List<string>();

            //现有数据
            var oldCurrentAcls = (from entity in oldAcls
                                  where entity.RoleParam1 == acl.SID && entity.RoleType == acl.SIDType
                                  select entity).ToList();

            //继承来的数据保留
            var oldAclsInherited = oldCurrentAcls.Where(entity => entity.Inherited).ToList();
            aclCurrent.AddRange(oldAclsInherited);

            //去除当前非继承的数据中和继承一致的数据
            foreach (var oldAclInherited in oldAclsInherited)
            {
                var tempAllowPerms = string.IsNullOrEmpty(oldAclInherited.AllowPerms) ? new List<string>() : oldAclInherited.AllowPerms.Split(",").ToList();
                var tempDenyPerms = string.IsNullOrEmpty(oldAclInherited.DenyPerms) ? new List<string>() : oldAclInherited.DenyPerms.Split(",").ToList();
                acl.AllowPermision = acl.AllowPermision.Except(tempAllowPerms).ToList();
                acl.DenyPermision = acl.DenyPermision.Except(tempDenyPerms).ToList();
            }

            //不是继承的数据
            var oldAclsNoInherited = oldCurrentAcls.Where(entity => !entity.Inherited).ToList();

            //继承的数据因为一定不可以修改，所以只考虑往非继承中插入或更新，并不需要判断当前是否继承来的数据
            //插入
            if (oldAclsNoInherited.Count == 0)
            {
                var maxOrderIndex = oldCurrentAcls.Count == 0 ? 1 : oldCurrentAcls.Max(o => o.OrderIndex);
                var entity = new SysResourcePermAcl()
                {
                    SID = param.SID,
                    AllowPerms = string.Join(',', acl.AllowPermision),
                    DenyPerms = string.Join(',', acl.DenyPermision),
                    RoleType = acl.SIDType,
                    RoleParam1 = acl.SID,
                    Inherited = false,
                    OrderIndex = (acl.SID == ConstRSIDs.RoleSID.Administrators || acl.SID == ConstRSIDs.RoleSID.Everyone) ? 1 : ++maxOrderIndex,
                    CreatorAccount = loginUser.UserAccount,
                    CreatorName = loginUser.UserName,
                };
                aclInserts.Add(entity);
                aclCurrent.Add(CopyTo(entity));
            }
            else //更新
            {
                //合并现有的及新的权限到一条数据中
                //对现有数据的更新
                var mainEntity = oldAclsNoInherited[0];
                mainEntity.AllowPerms = string.Join(',', acl.AllowPermision);
                mainEntity.DenyPerms = string.Join(',', acl.DenyPermision);
                aclUpdates.Add(mainEntity);
                aclCurrent.Add(CopyTo(mainEntity));

                //额外的需要被删除
                if (oldAclsNoInherited.Count > 1)
                {
                    for (var i = 1; i < oldAclsNoInherited.Count; i++)
                        aclDeletes.Add(oldAclsNoInherited[i]);
                }
            }
        }

        //继承的资源
        //如果是菜单的情况，存在层级结构，需要继承
        var sidType = _resourcePermCommonService.GetSIDType(param.SIDType);
        if (sidType == SIDTypeEnum.MenuSID)
        {
            //构建子集
            await BuildChildMenuAcl(param.SID, aclCurrent, aclInserts, aclDeletes, aclUpdates, loginUser);
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            if (aclInserts.Count > 0)
                await _resourceAclRepository.InsertAsync(aclInserts);
            if (aclDeletes.Count > 0)
                await _resourceAclRepository.DeleteAsync(aclDeletes);
            if (aclUpdates.Count > 0)
                await _resourceAclRepository.UpdateAsync(aclUpdates);
        });
    }

    #region 私有方法

    /// <summary>
    /// 获取资源操作列表，附带管理员
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="sidType"></param>
    /// <returns></returns>
    private async Task<List<SysResourcePerm>> GetResourcePermsInner(string sid, SIDTypeEnum? sidType)
    {
        //对应的模块操作
        var list = await _resourcePermCommonService.GetPermisions(sid);

        //添加管理权限
        if ((sidType == SIDTypeEnum.MenuSID && sid == ConstRSIDs.Base.MenuRoot) ||
            (sidType == SIDTypeEnum.ResourceSID && sid == ConstRSIDs.Base.ResourceRoot))
        {
            var assignPermName = PermTypeEnum.UserResourceAssignPermision.ToString();
            var writePermName = PermTypeEnum.Write.ToString();
            var assignPermision = list.FirstOrDefault(x => x.PermName == assignPermName); // 资源管理员
            var writePermision = list.FirstOrDefault(x => x.PermName == writePermName); // 授权管理员
            if (assignPermision == null)
            {
                list.Insert(0, new SysResourcePerm()
                {
                    SID = sid,
                    PermName = writePermName,
                    PermDisplayName = _enumService.GetDescription(PermTypeEnum.Write), // 授权管理员
                    PermType = ConstNames.ResourcePermTypeValue.System,
                    OrderIndex = 0,
                });
            }
            if (writePermision == null)
            {
                list.Insert(0, new SysResourcePerm()
                {
                    SID = sid,
                    PermName = assignPermName,
                    PermDisplayName = _enumService.GetDescription(PermTypeEnum.UserResourceAssignPermision), // 资源管理员
                    PermType = ConstNames.ResourcePermTypeValue.System,
                    OrderIndex = 0,
                });
            }
        }
        return list;
    }

    /// <summary>
    /// 获取资源操作权限列表
    /// </summary>
    /// <returns></returns>
    private async Task<List<SysResourcePermAcl>> GetResourcePermAcls()
    {
        //当前菜单当前角色权限
        var acls = await _resourceAclRepository.Entities.ToListAsync();
        return acls;
    }

    /// <summary>
    /// 获取资源角色列表
    /// </summary>
    /// <returns></returns>
    private async Task<List<SysResourcePerm>> GetResourcePerms()
    {
        //当前菜单当前角色
        var perms = await _resourceRepository.Entities.ToListAsync();
        return perms;
    }

    /// <summary>
    /// 获取菜单资源根节点
    /// </summary>
    /// <param name="treeMenuResource"></param>
    /// <returns></returns>
    private ResurceNodeItem GetMenuRootResurceNode(List<ResurceNodeItem> treeMenuResource = null)
    {
        var node = new ResurceNodeItem
        {
            Label = "菜单项",
            SID = ConstRSIDs.Base.MenuRoot,
            SIDType = SIDTypeEnum.MenuSID.ToString(),
            IsRoot = true,
        };
        if (treeMenuResource != null) node.Children = treeMenuResource;
        return node;
    }

    /// <summary>
    /// 获取资源根节点
    /// </summary>
    /// <param name="treeMenuResource"></param>
    /// <returns></returns>
    private ResurceNodeItem GetRootResurceNode(List<ResurceNodeItem> treeMenuResource = null)
    {
        var node = new ResurceNodeItem
        {
            Label = "资源库",
            SID = ConstRSIDs.Base.ResourceRoot,
            SIDType = SIDTypeEnum.ResourceSID.ToString(),
            IsRoot = true,
        };
        if (treeMenuResource != null) node.Children = treeMenuResource;
        return node;
    }

    /// <summary>
    /// 获取资源权限角色信息
    /// 操作权限项+对应角色
    /// </summary>
    /// <returns></returns>
    private List<ResourcePermRoleItemDto_RoleItem> GetResourcePermsRoles(SysResourcePerm resource, List<SysResourcePermAcl> acls)
    {
        var allowPerms = acls.Where(acl => acl.AllowPerms != null && acl.AllowPerms.Split(",").Contains(resource.PermName));
        var denyPerms = acls.Where(acl => acl.DenyPerms != null && acl.DenyPerms.Split(",").Contains(resource.PermName));

        var roles = new List<ResourcePermRoleItemDto_RoleItem>();
        foreach (var allowPerm in allowPerms)
        {
            //存在当前循环操作权限被禁用的被跳过
            if (denyPerms.Any(acl => acl.RoleParam1 == allowPerm.RoleParam1)) continue;

            roles.Add(new ResourcePermRoleItemDto_RoleItem
            {
                Name = GetMemberDisplayName(allowPerm.RoleParam1, allowPerm.RoleType),
                SID = allowPerm.RoleParam1,
                SIDType = allowPerm.RoleType
            });
        }
        if (!roles.Any(item => item.SID == ConstRSIDs.RoleSID.Administrators))
        {
            roles.Insert(0, new ResourcePermRoleItemDto_RoleItem
            {
                Name = ConstNames.RoleName.Administrators,
                SID = ConstRSIDs.RoleSID.Administrators,
                SIDType = SIDTypeEnum.RoleSID.ToString()
            });
        }
        return roles;
    }

    /// <summary>
    /// 删除或更新子菜单的权限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="roleSid"></param>
    /// <param name="roleSidType"></param>
    /// <param name="currentAcl">当前菜单对应角色权限</param>
    /// <param name="aclInserts"></param>
    /// <param name="aclDeletes"></param>
    /// <param name="aclUpdates"></param>
    /// <param name="loginUser"></param>
    /// <param name="allMenus"></param>
    /// <param name="allAcls"></param>
    /// <param name="cmenu"></param>
    private async Task DeleteChildMenuRoleAcl(string sid, string roleSid, string roleSidType,
        SysResourcePermAcl currentAcl,
        List<SysResourcePermAcl> aclInserts,
        List<SysResourcePermAcl> aclDeletes,
        List<SysResourcePermAcl> aclUpdates, 
        LoginUserInfoDto loginUser, List<SysMenu> allMenus = null, List<SysResourcePermAcl> allAcls = null,
        SysMenu cmenu = null)
    {
        //数据初始化
        if (allMenus == null) allMenus = await _menuService.GetAllMenus();
        if (allAcls == null) allAcls = await GetResourcePermAcls();
        if(cmenu == null) cmenu = allMenus.FirstOrDefault(x => x.RSID == sid);
        if (cmenu == null) throw new CustomException("未找到当前菜单！");

        //子层级数据
        var menus = allMenus.Where(x => (sid == ConstRSIDs.Base.MenuRoot) ? string.IsNullOrEmpty(x.PId) : x.PId == cmenu.Id).ToList();

        //更新子集对应的权限
        foreach (var menu in menus)
        {
            //用于继承给下一级
            SysResourcePermAcl currentChildAcl = null;

            //当前菜单当前角色权限
            var oldCurrentAcls = allAcls.Where(entity => entity.SID == menu.RSID && entity.RoleParam1 == roleSid && entity.RoleType == roleSidType).ToList();

            //继承来的数据需要按照当前新的数据作修改
            var oldAclInherited = oldCurrentAcls.FirstOrDefault(entity => entity.Inherited);
            var oldAclNoInherited = oldCurrentAcls.FirstOrDefault(entity => !entity.Inherited);

            //表示上级没有继承来的数据，则删除当前子项的继承来的数据
            if (currentAcl == null)
            {
                if (oldAclInherited != null) aclDeletes.Add(oldAclInherited);
                if (oldAclNoInherited != null) currentChildAcl = oldAclNoInherited;
            }
            else
            {
                if (oldAclInherited != null)
                {
                    oldAclInherited.AllowPerms = currentAcl.AllowPerms;
                    oldAclInherited.DenyPerms = currentAcl.DenyPerms;
                    aclUpdates.Add(oldAclInherited);
                }
                else //应当不会出现这种情况，因为当前有下级不会没有
                {
                    var maxOrderIndex = oldCurrentAcls.Count == 0 ? 1 : oldCurrentAcls.Max(a => a.OrderIndex);
                    oldAclInherited = new SysResourcePermAcl
                    {
                        SID = menu.RSID,
                        AllowPerms = currentAcl.AllowPerms,
                        DenyPerms = currentAcl.DenyPerms,
                        RoleType = roleSidType,
                        RoleParam1 = roleSid,
                        Inherited = true,
                        OrderIndex = (roleSid == ConstRSIDs.RoleSID.Administrators || roleSid == ConstRSIDs.RoleSID.Everyone) ? 1 : ++maxOrderIndex,
                        CreatorName = loginUser.UserName,
                        CreatorAccount = loginUser.UserAccount,
                    };
                    aclInserts.Add(oldAclInherited);
                }

                //合并后继承给下级
                currentChildAcl = AclFormatNewInherited(oldAclInherited, oldAclNoInherited);
            }

            //更新子集的子集
            await DeleteChildMenuRoleAcl(menu.RSID, roleSid, roleSidType, currentChildAcl, aclInserts, aclDeletes, aclUpdates, loginUser, allMenus, allAcls, menu);
        }
    }

    /// <summary>
    /// 构建子菜单权限
    /// 当前级别的优先级高于父级继承来的
    /// </summary>
    private async Task BuildChildMenuAcl(string sid, List<SysResourcePermAcl> parentAcls,
        List<SysResourcePermAcl> aclInserts,
        List<SysResourcePermAcl> aclDeletes,
        List<SysResourcePermAcl> aclUpdates, 
        LoginUserInfoDto loginUser, List<SysMenu> allMenus = null, List<SysResourcePerm> allPerms = null, List<SysResourcePermAcl> allAcls = null)
    {
        //全部数据，这里只适合定量权限的情况，目前仅有菜单权限，如果以后要在菜单权限外，支持自定义权限，这种模式则需要改，否则可能会慢
        if (allMenus == null) allMenus = await _menuService.GetAllMenus();
        if (allPerms == null) allPerms = await GetResourcePerms();
        if (allAcls == null) allAcls = await GetResourcePermAcls();

        //子层级数据
        List<SysMenu> menus; 
        if (sid == ConstRSIDs.Base.MenuRoot)
        {
            menus = allMenus.Where(x => string.IsNullOrEmpty(x.PId)).ToList();
        }
        else
        {
            //当前
            var cmenu = allMenus.First(x => x.RSID == sid);
            menus = allMenus.Where(x => x.PId == cmenu.Id).ToList();
        }

        //将父级的权限重新解析，当前级别的优先级高于父级继承来的
        var formatAcls = parentAcls.Where(entity => entity.Inherited).ToList();
        var aclsNoInherited = parentAcls.Where(entity => !entity.Inherited).ToList();
        foreach (var aclNoInherited in aclsNoInherited)
        {
            var formatAcl = formatAcls.FirstOrDefault(acl => acl.RoleParam1 == aclNoInherited.RoleParam1);
            if (formatAcl == null) formatAcls.Add(aclNoInherited);

            //当前级别的优先级高于父级继承来的
            else AclFormatToInherited(formatAcl, aclNoInherited);
        }

        //获取菜单直属子集
        foreach (var menu in menus)
        {
            //当前要继承给下一级
            var currentAcls = new List<SysResourcePermAcl>();

            //存在的操作项
            var perms = allPerms.Where(x => x.SID == menu.RSID).ToList();

            //现有权限
            var oldAcls = allAcls.Where(entity => entity.SID == menu.RSID).ToList();

            //当前如果没有任何操作项，就不需要添加权限
            //直接继承到下一级
            if (perms.Count == 0)
            {
                if (oldAcls.Count > 0) aclDeletes.AddRange(oldAcls);
                continue;
            }
            var permNames = perms.Select(x => x.PermName).ToList();

            //结合现有权限及父级别重构权限
            foreach (var acl in formatAcls)
            {
                //当前权限
                var cAllowPermNames = (acl.AllowPerms ?? string.Empty).Split(',').Where(permName => permNames.Contains(permName));
                var cDenyPermNames = (acl.DenyPerms ?? string.Empty).Split(',').Where(permName => permNames.Contains(permName));
                var allowPermNameStr = string.Join(",", cAllowPermNames);
                var denyPermNameStr = string.Join(",", cDenyPermNames);

                //当前角色对应的权限
                var oldCurrentAcls = (from entity in oldAcls
                                      where entity.RoleParam1 == acl.RoleParam1 && entity.RoleType == acl.RoleType
                                      select entity).ToList();

                //继承来的数据需要按照当前新的数据作修改
                var oldAclsInherited = oldCurrentAcls.Where(entity => entity.Inherited).ToList();

                //插入或更新继承的数据
                //排序，不存在添加 1 的情况，如果不存在任何权限时，默认应该有一个管理员的权限，所以从 2 开始
                SysResourcePermAcl currentAcl;
                if (oldAclsInherited.Count == 0) //插入
                {
                    var maxOrderIndex = oldAcls.Count == 0 ? 1 : oldAcls.Max(o => o.OrderIndex);
                    currentAcl = new SysResourcePermAcl()
                    {
                        SID = menu.RSID,
                        AllowPerms = allowPermNameStr,
                        DenyPerms = denyPermNameStr,
                        RoleType = acl.RoleType,
                        RoleParam1 = acl.RoleParam1,
                        Inherited = true,
                        OrderIndex = (acl.RoleParam1 == ConstRSIDs.RoleSID.Administrators || acl.RoleParam1 == ConstRSIDs.RoleSID.Everyone) ? 1 : ++maxOrderIndex,
                        CreatorName = loginUser.UserName,
                        CreatorAccount = loginUser.UserAccount,
                    };
                    aclInserts.Add(currentAcl);
                    currentAcls.Add(CopyTo(currentAcl));
                }
                else //更新
                {
                    //合并现有的及新的权限到一条数据中
                    //对现有数据的更新
                    currentAcl = oldAclsInherited[0];
                    currentAcl.AllowPerms = allowPermNameStr;
                    currentAcl.DenyPerms = denyPermNameStr;
                    aclUpdates.Add(currentAcl);
                    currentAcls.Add(CopyTo(currentAcl));

                    //额外的需要被删除
                    if (oldAclsInherited.Count > 1)
                    {
                        for (var i = 1; i < oldAclsInherited.Count; i++)
                            aclDeletes.Add(oldAclsInherited[i]);
                    }
                }

                //不是继承的数据，进行处理，每个角色应该只有一条数据，多余的删除
                var oldAclsNoInherited = oldCurrentAcls.Where(entity => !entity.Inherited).ToList();
                if (oldAclsNoInherited.Count > 0)
                {
                    var oldAclNoInherited = oldAclsNoInherited[0];
                    var oldAllowPermNames = (oldAclNoInherited.AllowPerms ?? string.Empty).Split(',').Where(permName => permNames.Contains(permName));
                    var oldDenyPermNames = (oldAclNoInherited.DenyPerms ?? string.Empty).Split(',').Where(permName => permNames.Contains(permName));
                    var oldAllowPerms = string.Join(",", oldAllowPermNames);
                    var oldDenyPerms = string.Join(",", oldDenyPermNames);
                    oldAclNoInherited.AllowPerms = oldAllowPerms;
                    oldAclNoInherited.DenyPerms = oldDenyPerms;

                    //去除当前非继承的数据中和继承一致的数据
                    AclClearNoInherited(currentAcl, oldAclNoInherited);

                    if (oldAllowPerms != oldAclNoInherited.AllowPerms || oldDenyPerms != oldAclNoInherited.DenyPerms)
                    {
                        aclUpdates.Add(oldAclNoInherited);
                    }
                    currentAcls.Add(CopyTo(oldAclsNoInherited[0]));

                    //额外的需要被删除
                    if (oldAclsNoInherited.Count > 1)
                    {
                        for (var i = 1; i < oldAclsNoInherited.Count; i++)
                            aclDeletes.Add(oldAclsNoInherited[i]);
                    }
                }
            }

            //构建子集的子集
            await BuildChildMenuAcl(menu.RSID, currentAcls, aclInserts, aclDeletes, aclUpdates, loginUser, allMenus: allMenus, allPerms: allPerms, allAcls: allAcls);
        }
    }

    /// <summary>
    /// 继承结果合并到继承中
    /// </summary>
    private void AclFormatToInherited(SysResourcePermAcl aclInherited, SysResourcePermAcl aclNoInherited)
    {
        if (aclNoInherited == null) return;
        else if (aclInherited == null) return;

        //当前级别的优先级高于父级继承来的
        var inheritedAllowPerms = string.IsNullOrEmpty(aclInherited.AllowPerms) ? new List<string>() : aclInherited.AllowPerms.Split(",").ToList();
        var inheritedDenyPerms = string.IsNullOrEmpty(aclInherited.DenyPerms) ? new List<string>() : aclInherited.DenyPerms.Split(",").ToList();
        var noInheritedAllowPerms = string.IsNullOrEmpty(aclNoInherited.AllowPerms) ? new List<string>() : aclNoInherited.AllowPerms.Split(",").ToList();
        var noIeritedDenyPerms = string.IsNullOrEmpty(aclNoInherited.DenyPerms) ? new List<string>() : aclNoInherited.DenyPerms.Split(",").ToList();
        var formatAllowPerms = new List<string>();
        var formatDenyPerms = new List<string>();

        formatAllowPerms.AddRange(noInheritedAllowPerms);
        formatDenyPerms.AddRange(noIeritedDenyPerms);
        foreach (var inheritedAllowPerm in inheritedAllowPerms)
        {
            if (!formatAllowPerms.Contains(inheritedAllowPerm) && !formatDenyPerms.Contains(inheritedAllowPerm))
            {
                formatAllowPerms.Add(inheritedAllowPerm);
            }
        }
        foreach (var inheritedDenyPerm in inheritedDenyPerms)
        {
            if (!formatAllowPerms.Contains(inheritedDenyPerm) && !formatDenyPerms.Contains(inheritedDenyPerm))
            {
                formatDenyPerms.Add(inheritedDenyPerm);
            }
        }

        aclInherited.AllowPerms = string.Join(",", formatAllowPerms);
        aclInherited.DenyPerms = string.Join(",", formatDenyPerms);
    }

    /// <summary>
    /// 继承结果合并为一条新的继承
    /// </summary>
    private SysResourcePermAcl AclFormatNewInherited(SysResourcePermAcl aclInherited, SysResourcePermAcl aclNoInherited)
    {
        if (aclNoInherited == null)
        {
            return CopyTo(aclInherited);
        }
        else if (aclInherited == null)
        {
            return CopyTo(aclNoInherited);
        }

        var formatAcl = CopyTo(aclInherited);

        //当前级别的优先级高于父级继承来的
        var inheritedAllowPerms = string.IsNullOrEmpty(aclInherited.AllowPerms) ? new List<string>() : aclInherited.AllowPerms.Split(",").ToList();
        var inheritedDenyPerms = string.IsNullOrEmpty(aclInherited.DenyPerms) ? new List<string>() : aclInherited.DenyPerms.Split(",").ToList();
        var noInheritedAllowPerms = string.IsNullOrEmpty(aclNoInherited.AllowPerms) ? new List<string>() : aclNoInherited.AllowPerms.Split(",").ToList();
        var noIeritedDenyPerms = string.IsNullOrEmpty(aclNoInherited.DenyPerms) ? new List<string>() : aclNoInherited.DenyPerms.Split(",").ToList();
        var formatAllowPerms = new List<string>();
        var formatDenyPerms = new List<string>();

        formatAllowPerms.AddRange(noInheritedAllowPerms);
        formatDenyPerms.AddRange(noIeritedDenyPerms);
        foreach (var inheritedAllowPerm in inheritedAllowPerms)
        {
            if (!formatAllowPerms.Contains(inheritedAllowPerm) && !formatDenyPerms.Contains(inheritedAllowPerm))
            {
                formatAllowPerms.Add(inheritedAllowPerm);
            }
        }
        foreach (var inheritedDenyPerm in inheritedDenyPerms)
        {
            if (!formatAllowPerms.Contains(inheritedDenyPerm) && !formatDenyPerms.Contains(inheritedDenyPerm))
            {
                formatDenyPerms.Add(inheritedDenyPerm);
            }
        }

        formatAcl.AllowPerms = string.Join(",", formatAllowPerms);
        formatAcl.DenyPerms = string.Join(",", formatDenyPerms);
        return formatAcl;
    }

    /// <summary>
    /// 复制，用于往子资源复制时使用
    /// </summary>
    /// <param name="acl"></param>
    /// <returns></returns>
    private SysResourcePermAcl CopyTo(SysResourcePermAcl acl)
    {
        if (acl == null) return null;
        var formatAcl = new SysResourcePermAcl
        {
            SID = acl.SID,
            AllowPerms = acl.AllowPerms,
            DenyPerms = acl.DenyPerms,
            RoleType = acl.RoleType,
            RoleParam1 = acl.RoleParam1,
            Inherited = true,
            OrderIndex = acl.OrderIndex,
            CreatorName = acl.CreatorName,
            CreatorAccount = acl.CreatorAccount,
        };
        return formatAcl;
    }

    /// <summary>
    /// 从非继承中排除继承的
    /// </summary>
    private void AclClearNoInherited(SysResourcePermAcl aclInherited, SysResourcePermAcl aclNoInherited)
    {
        if (aclNoInherited == null || aclInherited == null) return;

        var tempAllowPerms = string.IsNullOrEmpty(aclInherited.AllowPerms) ? new List<string>() : aclInherited.AllowPerms.Split(",").ToList();
        var tempDenyPerms = string.IsNullOrEmpty(aclInherited.DenyPerms) ? new List<string>() : aclInherited.DenyPerms.Split(",").ToList();

        if (string.IsNullOrEmpty(aclNoInherited.AllowPerms)) aclNoInherited.AllowPerms = string.Empty;
        if (string.IsNullOrEmpty(aclNoInherited.DenyPerms)) aclNoInherited.DenyPerms = string.Empty;
        aclNoInherited.AllowPerms = string.Join(",", aclNoInherited.AllowPerms.Split(",").ToList().Except(tempAllowPerms));
        aclNoInherited.DenyPerms = string.Join(",", aclNoInherited.DenyPerms.Split(",").ToList().Except(tempDenyPerms));
    }

    /// <summary>
    /// 返回 SID 对应的成员名称
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="sidType"></param>
    /// <returns></returns>
    private string GetMemberDisplayName(string sid, string sidType)
    {
        if (sid == null) sid = string.Empty;
        var sidTypeEnum = _resourcePermCommonService.GetSIDType(sidType);
        switch (sidTypeEnum)
        {
            case SIDTypeEnum.RoleGroupSID:
                var roleGroup = _roleGroupService.TryGetRoleGroupBySID(sid);
                if (roleGroup == null) return sid;
                return roleGroup.GroupName;
            case SIDTypeEnum.RoleSID:
                var role = _roleService.TryGetRoleBySID(sid);
                if (role == null) return sid;
                return role.RoleName;
            //case SIDType.OUSID:
            //    var ou = OUHelper.TryGetOUBySID(sid);
            //    if (ou == null) return sid;
            //    return ou.OUFullName;
            //case SIDType.LeaderTitleSID:
            //    return sid.Remove(0, ConstNames.Prefix.UserLeaderTitle.Length);
            //case SIDType.JobLevelSID:
            //    return sid.Remove(0, ConstNames.Prefix.UserJobLevel.Length);
            case SIDTypeEnum.UserSID:
                var user = _userService.TryGetUserBySID(sid);
                if (user == null) return sid;
                return $"{user.Name}({user.Account})";
            default:
                return $"(不支持的类型：{sidType})";
        }
    }

    /// <summary>
    /// 查询父结构的菜单资源项
    /// </summary>
    private List<ResourceItemDto> GetParentMenus(SysMenu menu, List<SysMenu> menus, List<ResourceItemDto> resources = null, bool containMe = false)
    {
        if (resources == null) resources = new List<ResourceItemDto>();
        if (containMe) resources.Insert(0, new ResourceItemDto(menu));

        if (!string.IsNullOrEmpty(menu.PId))
        {
            //查询父级
            var pmenu = (from cmenu in menus
                         where cmenu.Id == menu.PId
                         select cmenu).FirstOrDefault();
            if (pmenu == null) return resources;
            return GetParentMenus(pmenu, menus, resources, true);
        }

        return resources;
    }

    ///// <summary>
    ///// 获取菜单结构
    ///// </summary>
    ///// <param name="sid"></param>
    ///// <returns></returns>
    //private async Task<List<ResourceItemDto>> GetMenuStruct(string sid)
    //{
    //    var menus = await (from menu in _menuRepository.Entities
    //                       orderby menu.OrderIndex, menu.Id
    //                       select menu).ToListAsync();

    //    var model = (from menu in menus
    //                 where menu.RSID == sid
    //                 select menu).FirstOrDefault();
    //    if (model == null)
    //        throw new Exception("菜单资源未找到！");

    //    return GetParentMenus(model, menus, new List<ResourceItemDto>(), true);
    //}

    ///// <summary>
    ///// 格式化角色 SID
    ///// </summary>
    //private void FormatAclSID(ResourcePermAclsSaveParam_Item acl)
    //{
    //    if (acl != null && acl.SID != null)
    //        acl.SID = FormatAclSID(acl.SID, acl.SIDType);
    //}

    ///// <summary>
    ///// 格式化角色 SID
    ///// </summary>
    //private string FormatAclSID(string sid, string sidType)
    //{
    //    if (sid != null && sidType != null)
    //    {
    //        //var sidTypeEnum = GetSIDType(sidType);
    //        //switch (sidTypeEnum)
    //        //{
    //            //case SIDType.JobLevelSID:
    //            //    if (!sid.StartsWith(ConstNames.Prefix.UserJobLevel))
    //            //        sid = $"{ConstNames.Prefix.UserJobLevel}{sid}";
    //            //    break;
    //            //case SIDType.LeaderTitleSID:
    //            //    if (!sid.StartsWith(ConstNames.Prefix.UserLeaderTitle))
    //            //        sid = $"{ConstNames.Prefix.UserLeaderTitle}{sid}";
    //            //    break;
    //            //case SIDType.RoleSID:
    //            //    if (!sid.StartsWith(ConstNames.Prefix.UserJobLevel))
    //            //        sid = $"{ConstNames.Prefix.UserJobLevel}{sid}";
    //            //    break;
    //            //case SIDType.RoleGroupSID:
    //            //    if (!sid.StartsWith(ConstNames.Prefix.UserLeaderTitle))
    //            //        sid = $"{ConstNames.Prefix.UserLeaderTitle}{sid}";
    //            //    break;
    //        //}
    //    }
    //    return sid;
    //}

    ///// <summary>
    ///// 格式化角色 SID
    ///// </summary>
    //private void FormatAclSID(List<ResourcePermAclsSaveParam_Item> acls)
    //{
    //    if (acls != null) acls.ForEach(acl => FormatAclSID(acl));
    //}

    #endregion 私有方法
}
