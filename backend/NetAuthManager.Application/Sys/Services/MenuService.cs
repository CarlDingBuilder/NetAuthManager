using NetAuthManager.Application.Params.Menus;
using NetAuthManager.Application.Sys.Mappers;
using NetAuthManager.Application.Sys.Params.Menus;
using NetAuthManager.Application.Sys.Params.Resources;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Common.Extended;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Services;
using NetAuthManager.EntityFramework.Core;
using Furion.DatabaseAccessor;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 菜单服务
/// </summary>
public class MenuService : BaseService<SysMenu>, IMenuService, ITransient
{
    #region 构造与注入

    private const string DefaultPCode = "default";

    private readonly ILogger<MenuService> _logger;

    private readonly IRepository<SysMenu> _menuRepository;
    private readonly IRepository<SysMenuType> _menuTypeRepository;
    private readonly IRepository<SysResourcePerm> _resourceRepository;
    private readonly IRepository<SysResourcePermAcl> _resourceAclRepository;

    private readonly BaseService<SysMenuType> _menuTypeService;
    private readonly IResourcePermCommonService _resourcePermCommonService;
    private readonly IRoleService _roleService;

    private readonly IRegister _routerMapper;

    public MenuService(
        ILogger<MenuService> logger, IServiceScopeFactory scopeFactory, 
        IRepository<SysMenu> menuRepository, IRepository<SysMenuType> menuTypeRepository,
        IRepository<SysResourcePerm> resourceService, IRepository<SysResourcePermAcl> resourceAclService,
        BaseService<SysMenuType> menuTypeService, 
        RouterMapper routerMapper, IResourcePermCommonService resourcePermService, IRoleService roleService) : base(menuRepository, scopeFactory)
    {
        _logger = logger;

        _menuRepository = menuRepository;
        _menuTypeRepository = menuTypeRepository;
        _resourceRepository = resourceService;
        _resourceAclRepository = resourceAclService;

        _menuTypeService = menuTypeService;
        _roleService = roleService;

        _routerMapper = routerMapper;
        _resourcePermCommonService = resourcePermService;
    }

    #endregion 构造与注入

    /// <summary>
    /// 获取菜单类型
    /// </summary>
    public async Task<List<SysMenuType>> GetMenuType()
    {
        var list = new List<SysMenuType>();
        var listData = await _menuTypeService.GetListAsync();
        if (listData != null || listData.Count > 0)
        {
            if (!listData.Any(x => x.PCode == DefaultPCode))
            {
                list.Add(new SysMenuType
                {
                    PCode = DefaultPCode,
                    Name = "默认菜单",
                });
            }
            list.AddRange(listData);
        }
        else
        {
            list.Add(new SysMenuType
            {
                PCode = DefaultPCode,
                Name = "默认菜单",
            });
        }
        return list;
    }

    /// <summary>
    /// 添加菜单类型
    /// </summary>
    public async Task AddMenuType(SysMenuType menuType)
    {
        if (await _menuTypeService.AnyAsync(x => x.Id == menuType.PCode))
            throw new CustomException($"已存在编码为 {menuType.PCode} 的菜单类型！");

        if (await _menuTypeService.AnyAsync(x => x.Id == menuType.Name))
            throw new CustomException($"已存在名称为 {menuType.Name} 的菜单类型！");

        await _menuTypeService.InsertAsync(menuType);
    }

    /// <summary>
    /// 删除菜单类型
    /// </summary>
    public async Task DeleteMenuType(DeleteMenuTypeParam param)
    {
        await _menuTypeService.DeleteByIdAsync(param.PCode);
    }

    /// <summary>
    /// 获取菜单
    /// </summary>
    public async Task<List<SysMenu>> GetMenus(GetMenusParam param)
    {
        var list = await GetAllMenus(param);
        return BuildMenuTree(list);
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns></returns>
    public async Task<List<SysMenu>> GetMenuTreeByRsid(string rsid)
    {
        var enums = await GetAllMenus(new GetMenusParam {
            PCode= DefaultPCode,
        });
        var menu = enums.Where(x => x.RSID == rsid).FirstOrDefault();
        if (menu == null) throw new CustomException("未找到指定资源ID的菜单项！");

        return BuildMenuTree(enums, menu.PId);
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    /// <param name="menus"></param>
    /// <param name="pid"></param>
    /// <returns></returns>
    public List<SysMenu> BuildMenuTree(List<SysMenu> menus, string pid = null)
    {
        List<SysMenu> list;
        if(string.IsNullOrEmpty(pid))
        {
            list = menus.Where(x => string.IsNullOrEmpty(x.PId)).ToList();
        }
        else
        {
            list = menus.Where(x => x.PId == pid).ToList();
        }
        foreach(var menu in list)
        {
            menu.Children = BuildMenuTree(menus, menu.Id);
        }
        return list;
    }

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<List<SysMenu>> GetAllMenus(GetMenusParam param)
    {
        var list = await _menuRepository.Entities.OrderBy(x => x.OrderIndex)
            .Where(x => x.PCode == param.PCode)
            .ToListAsync();

        return list;
    }

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    public async Task<List<SysMenu>> GetAllMenus()
    {
        return await GetAllMenus(new GetMenusParam
        {
            PCode = DefaultPCode
        });
    }

    /// <summary>
    /// 保存菜单
    /// </summary>
    /// <returns></returns>
    public async Task SaveMenu(SysMenu menu)
    {
        if (string.IsNullOrEmpty(menu.Id))
        {
            //校验
            if (await this.AnyAsync(x => x.PCode == menu.PCode && x.PId == menu.PId && x.Name == menu.Name))
                throw new CustomException($"同级菜单下已经存在名称为 {menu.Name} 的菜单！");

            //校验
            if (await this.AnyAsync(x => x.PCode == menu.PCode && x.PId == menu.PId && x.NameEn == menu.NameEn))
                throw new CustomException($"同级菜单下已经存在英文名称为 {menu.NameEn} 的菜单！");
        }
        else
        {
            //校验
            if (await this.AnyAsync(x => x.PCode == menu.PCode && x.PId == menu.PId && x.Name == menu.Name && x.Id != menu.Id))
                throw new CustomException($"同级菜单下已经存在名称为 {menu.Name} 的菜单！");

            //校验
            if (await this.AnyAsync(x => x.PCode == menu.PCode && x.PId == menu.PId && x.NameEn == menu.NameEn && x.Id != menu.Id))
                throw new CustomException($"同级菜单下已经存在英文名称为 {menu.NameEn} 的菜单！");
        }

        //父主键及层级
        if (string.IsNullOrEmpty(menu.PId))
        {
            menu.MenuLevel = 1;
            menu.PId = null;
        }
        else
        {
            var pmenu = await this.GetFirstAsync(x => x.Id == menu.PId);
            if (pmenu == null)
                throw new CustomException($"父菜单未找到！");

            menu.MenuLevel = pmenu.MenuLevel + 1;
        }

        //标记
        if (menu.Badge == "no") menu.Badge = string.Empty;
        if (string.IsNullOrEmpty(menu.ModuleType)) menu.ModuleType = "1";
        if (string.IsNullOrEmpty(menu.RSID)) menu.RSID = BaseGuidEntity.GetNewGuid();

        //菜单分类
        menu.PCode = menu.PCode ?? DefaultPCode;

        //新增或更新菜单
        await this.InsertOrUpdateAsync(menu);
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    public async Task DeleteMenu(DeleteMenuParam param)
    {
        //获取
        var allMenus = await GetAllMenus();
        var cmenu = allMenus.FirstOrDefault(x => x.Id == param.Id);
        if (cmenu == null) return;
        cmenu.Children = BuildMenuTree(allMenus, param.Id);

        //菜单树结构
        var menus = GetMenuNodes(cmenu, new List<SysMenu>());
        var ids = menus.Select(x => x.Id).ToList();
        var sids = menus.Select(x => x.RSID).ToList();

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            //删除菜单
            await DeleteByIdsAsync(ids.ToArray());

            //删除菜单权限
            await _resourceRepository.Where(x => sids.Contains(x.SID)).ExecuteDeleteAsync();
            await _resourceAclRepository.Where(x => sids.Contains(x.SID)).ExecuteDeleteAsync();
        });
    }
    private List<SysMenu> GetMenuNodes(SysMenu menu, List<SysMenu> list)
    {
        list.Add(menu);
        GetMenuNodes(menu.Children, list);
        return list;
    }
    private void GetMenuNodes(List<SysMenu> menus, List<SysMenu> list)
    {
        if (menus != null)
        {
            foreach(var menu in menus)
            {
                GetMenuNodes(menu, list);
            }
        }
    }

    /// <summary>
    /// 获取路由
    /// </summary>
    public async Task<List<Hashtable>> GetRouters(GetRoutersParam param)
    {
        //暂时仅加载全菜单，菜单权限开发好之后需要修改
        var pcode = DefaultPCode;
        var menus = await GetAllMenus(new GetMenusParam
        {
            PCode = pcode,
        });
        var list = BuildMenuTree(menus);

        //加载权限
        var menuSIDs = menus.Select(m => m.RSID).ToList();

        //操作权限列表
        var permsTask = TryAsyncDoReturnDelegate<DefaultDbContext, List<SysResourcePerm>>(async (dbContext, serviceProvider) =>
        {
            return await _resourcePermCommonService.GetPermisions(dbContext, menuSIDs);
        });
        var aclsTask = TryAsyncDoReturnDelegate<DefaultDbContext, List<SysResourcePermAcl>>(async (dbContext, serviceProvider) =>
        {
            return await _resourcePermCommonService.GetPermisionAcls(dbContext, menuSIDs);
        });
        var userPermTask = TryAsyncDoReturnDelegate<DefaultDbContext, List<ResourceBaseParam>>(async (dbContext, serviceProvider) =>
        {
            return await _resourcePermCommonService.GetUserPermSIDs();
        });
        var perms = await permsTask;
        var acls = await aclsTask;
        var userPerm = await userPermTask;

        //获取当前登录用户
        var isAdministrator = await _roleService.IsAdministratorLogin();

        //构建路由
        var listHashtable = BuildRouter(list, perms, acls, userPerm, isAdministrator.IsBelong);
        return listHashtable;
    }

    /// <summary>
    /// 返回空数组则表示子菜单没有权限
    /// </summary>
    protected List<Hashtable> BuildRouter(List<SysMenu> src, List<SysResourcePerm> perms, List<SysResourcePermAcl> acls, List<ResourceBaseParam> userPerm, bool isAdministrator)
    {
        var list = new List<Hashtable>();
        foreach (var menu in src)
        {
            if (!menu.IsMenu) continue;

            //没权限则为空
            var router = BuildRouter(menu, perms, acls, userPerm, isAdministrator);
            if (router == null) continue;

            list.Add(router);
        }
        return list;
    }
    protected Hashtable BuildRouter(SysMenu menu, List<SysResourcePerm> allPerms, List<SysResourcePermAcl> allAcls, List<ResourceBaseParam> userPerm, bool isAdministrator)
    {
        var model = new Hashtable();
        var meta = new Hashtable();

        //额外参数
        string crsid = menu.RSID;
        JObject config = new JObject();
        if (!string.IsNullOrEmpty(menu.ConfigJson))
        {
            try
            {
                config = JObject.Parse(menu.ConfigJson);
                if (config["meta"] != null)
                {
                    foreach (var metaItem in (config["meta"] as JObject))
                    {
                        if (metaItem.Key == "rsid")
                        {
                            crsid = Convert.ToString(metaItem.Value);
                        }
                        else
                        {
                            meta[metaItem.Key] = metaItem.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model["CONFIGJSON"] = menu.ConfigJson;
                model["CONFIGJSON_ERROR"] = ex.Message;
            }
        }

        //当前菜单访问权限
        var perms = allPerms.Where(p => p.SID == crsid).ToList();
        var permNames = perms.Where(x => x.PermType == ConstNames.ResourcePermTypeValue.Module).Select(x => x.PermName).ToList();
        var acls = allAcls.Where(p => p.SID == crsid).ToList();
        var cacls = (from entity in acls
                     where userPerm.Any(perm => perm.SID == entity.RoleParam1 && perm.SIDType == entity.RoleType)
                     select entity).ToList();

        //检查当前菜单权限
        //有访问权限 Execute 则需要验证，没有则通过验证子节点是否有权限，没有子节点表示有权限
        //资源权限
        var jsonPerm = new Hashtable();
        //jsonPerm["rsid"] = menu.RSID;
        jsonPerm["sid"] = crsid;
        foreach (var permName in permNames)
        {
            jsonPerm[permName] = isAdministrator || _resourcePermCommonService.CheckPermision(permName, perms, cacls);
        }
        if (!isAdministrator)
        {
            var executePermName = PermTypeEnum.Execute.ToString();
            if (permNames.Contains(executePermName))
            {
                if (!Convert.ToBoolean(jsonPerm[executePermName])) return null;
            }
        }

        //当前菜单
        //menu.FormatToHashtable(model);
        model["name"] = menu.NameEn ?? "menu_" + menu.Id;
        model["meta"] = meta;
        meta["title"] = menu.Name;
        meta["icon"] = menu.Icon;
        meta["perm"] = jsonPerm;

        if (string.IsNullOrEmpty(menu.PId))
        {
            model["path"] = menu.Url ?? "/";
            model["component"] = "Layout";
            model["expand"] = !Convert.ToBoolean(menu.IsSpread);
        }
        else
        {
            if (menu.Children.Count > 0)
            {
                model["path"] = menu.Url ?? "/menu" + menu.Id;
                //meta["component"] = "Layout";
                model["expand"] = !Convert.ToBoolean(menu.IsSpread);
            }
            else
            {
                if (menu.NoClosable)
                {
                    meta["noClosable"] = true;
                }
                if (menu.NoKeepAlive)
                {
                    meta["noKeepAlive"] = true;
                }

                meta["urltype"] = menu.UrlType;
                if (menu.UrlType == "Html")
                {
                    model["path"] = "/" + menu.Id;
                    model["component"] = "@/views/other/iframe/view/index";
                    meta["url"] = menu.Id;
                }
                else if (menu.UrlType == "Vue")
                {
                    if (!string.IsNullOrEmpty(menu.NameEn))
                    {
                        model["name"] = menu.NameEn;
                    }
                    model["path"] = menu.Url;
                    var component = Convert.ToString(config["component"]) ?? menu.Url;
                    if (component.StartsWith("@"))
                    {
                        model["component"] = component;
                    }
                    else
                    {
                        model["component"] = "@" + component;
                    }
                    //meta["hidden"] = config["hidden"] != null ? config["component"] : false;
                    //if (!string.IsNullOrEmpty(menu.RSID))
                    //{
                    //    using (BPMConnection cn = new BPMConnection())
                    //    {
                    //        cn.WebOpen();
                    //        BPM.Client.Security.UserResourcePermisionCollection resourcePerms = BPM.Client.Security.UserResource.GetPermisions(cn, menu.RSID);
                    //        BPMObjectNameCollection permNames = new BPMObjectNameCollection();
                    //        foreach (BPM.Client.Security.UserResourcePermision resourcePerm in resourcePerms)
                    //        {
                    //            if (resourcePerm.PermType == UserResourcePermisionType.Module)
                    //                permNames.Add(resourcePerm.PermName);
                    //        }
                    //        //记录工具条上的模块级权限许可情况
                    //        bool[] rvs = BPM.Client.Security.UserResource.CheckPermision(cn, menu.RSID, permNames);
                    //        for (int k = 0; k < permNames.Count; k++)
                    //            jsonPerm[permNames[k]] = rvs[k];
                    //    }
                    //    meta["perm"] = jsonPerm;
                    //}
                }
                else if (menu.UrlType == "Custom")
                {
                    string cusid = Convert.ToString(JObject.Parse(menu.Url)["customid"]);
                    meta["cusid"] = cusid;
                    meta["urltype"] = "Vue";
                    model["path"] = "/" + menu.Id;
                    model["component"] = "@/views/dynamic/page/index";
                }
                else
                {
                    model["path"] = "/404";
                    model["component"] = "@/views/404";
                }
            }
        }
        if (!string.IsNullOrEmpty(menu.Badge))
        {
            meta["dot"] = true;
        }
        //meta["dynamicNewTab"] = true;

        //子菜单
        if (menu.Children != null && menu.Children.Count > 0)
        {
            var children = BuildRouter(menu.Children, allPerms, allAcls, userPerm, isAdministrator);
            if (children.Count == 0)
            {
                return null;
                //if (string.IsNullOrEmpty(Convert.ToString(meta["urltype"])))
                //{
                //    return null;
                //}
            }
            else model["children"] = children;
        }

        return model;
    }

    /// <summary>
    /// 查询所有子结构的菜单项
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="containMe"></param>
    /// <param name="allMenus"></param>
    /// <returns></returns>
    public async Task<List<SysMenu>> GetConcatMenu(List<string> ids, bool containMe = false, List<SysMenu> allMenus = null)
    {
        if (allMenus == null) allMenus = await GetAllMenus();

        //查询子集
        var list = (from menu in allMenus
                    where ids.Contains(menu.PId)
                    select menu).ToList();
        if (list.Count > 0)
        {
            var listChild = await GetConcatMenu(list.Select(q => q.Id).ToList(), allMenus: allMenus);
            if (listChild.Count > 0)
            {
                list.AddRange(listChild);
            }
        }
        //是否包含当前层级
        if (containMe)
        {
            var models = (from menu in allMenus
                          where ids.Contains(menu.Id)
                         select menu).ToList();
            list.AddRange(models);
        }
        return list;
    }

    /// <summary>
    /// 查询所有子结构的菜单项
    /// </summary>
    /// <param name="sids"></param>
    /// <param name="containMe"></param>
    /// <param name="allMenus"></param>
    /// <returns></returns>
    public async Task<List<SysMenu>> GetConcatMenuBySIDs(List<string> sids, bool containMe = false, List<SysMenu> allMenus = null)
    {
        if (allMenus == null) allMenus = await GetAllMenus();

        //查询对应的ID
        var models = (from menu in allMenus
                    where sids.Contains(menu.RSID)
                    select menu).ToList();
        var ids = models.Select(menu => menu.Id).ToList();

        //查询子集
        var list = (from menu in allMenus
                    where ids.Contains(menu.PId)
                    select menu).ToList();
        if (list.Count > 0)
        {
            var listChild = await GetConcatMenu(list.Select(q => q.Id).ToList(), allMenus: allMenus);
            if (listChild.Count > 0)
            {
                list.AddRange(listChild);
            }
        }
        //是否包含当前层级
        if (containMe)
        {
            list.AddRange(models);
        }
        return list;
    }

    /// <summary>
    /// 查询所有父结构的菜单项
    /// </summary>
    public async Task<SysMenu> GetParentMenuBySIDs(string rsid, List<SysMenu> allMenus = null)
    {
        if (allMenus == null) allMenus = await GetAllMenus();

        //查询对应的ID
        var model = (from menu in allMenus
                      where menu.RSID == rsid
                      select menu).FirstOrDefault();
        if (model == null) throw new CustomException("未查询到指定资源ID的菜单项！");

        //父级别
        await BuildParentMenu(model, allMenus);

        return model;
    }

    /// <summary>
    /// 查询所有父结构的菜单项
    /// </summary>
    public async Task BuildParentMenu(SysMenu menu, List<SysMenu> allMenus = null)
    {
        if (allMenus == null) allMenus = await GetAllMenus();

        //查询父菜单
        var pmenu = (from entity in allMenus
                    where menu.PCode == entity.PCode && menu.Id == entity.PId
                    select menu).FirstOrDefault();
        if (pmenu != null)
        {
            menu.Parent = pmenu;
            await BuildParentMenu(pmenu, allMenus);
        }
    }

    /// <summary>
    /// 按照全路径获取菜单项
    /// </summary>
    public async Task<SysMenu> GetMenuByFullName(string fullPath, List<SysMenu> allMenus = null)
    {
        if (allMenus == null) allMenus = await GetAllMenus();

        // 构建队列
        var pathes = fullPath.Split('/');
        var nodeQueue = new Queue();
        foreach (var path in pathes)
        {
            nodeQueue.Enqueue(path);
        }

        // 匹配资源
        var resource = await GetUserResource(nodeQueue, allMenus);
        if (resource == null)
            throw new CustomException("指定的资源全路径不存在");

        return resource;
    }

    /// <summary>
    /// 从名称队列到资源队列的转换
    /// </summary>
    private async Task<SysMenu> GetUserResource(Queue nodeQueue, List<SysMenu> allMenus = null, string parentId = null)
    {
        if (allMenus == null) allMenus = await GetAllMenus();

        //当前节点
        var nodeName = nodeQueue.Dequeue() as string;
        var menu = string.IsNullOrEmpty(parentId) 
            ? allMenus.FirstOrDefault(x => x.PCode == DefaultPCode && string.IsNullOrEmpty(x.PId) && x.Name == nodeName)
            : allMenus.FirstOrDefault(x => x.PCode == DefaultPCode && x.PId == parentId && x.Name == nodeName);

        //还存在元素
        if (nodeQueue.Count == 0) return menu;

        //继续查询
        return await GetUserResource(nodeQueue, allMenus, menu.Id);
    }

    /// <summary>
    /// 查询子菜单项
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="mustHave"></param>
    /// <returns></returns>
    public async Task<List<SysMenu>> GetChildMenuBySID(string sid, bool mustHave = true)
    {
        var menu = await (from entity in _menuRepository.Entities
                          where entity.RSID == sid
                          select entity).FirstOrDefaultAsync();
        if(mustHave)
        {
            if (menu == null)
                throw new Exception("没有找到当前菜单项！");
        }

        if (menu == null)
        {
            return new List<SysMenu>();
        }
        else
        {
            //查询子集
            var list = await (from entity in this._menuRepository.Entities
                              where entity.PId == menu.Id
                              select entity).ToListAsync();
            return list;
        }
    }

    /// <summary>
    /// 查询根菜单项
    /// </summary>
    public async Task<List<SysMenu>> GetRootMenu()
    {
        var list = await (from entity in _menuRepository.Entities
                         where string.IsNullOrEmpty(entity.PId)
                         select entity).ToListAsync();
        return list;
    }
}
