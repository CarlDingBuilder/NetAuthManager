using NetAuthManager.Application.Params.Menus;
using NetAuthManager.Application.Sys.Params.Menus;
using NetAuthManager.Core.Entities;
using System.Collections;

namespace NetAuthManager.Application;

public interface IMenuService
{
    /// <summary>
    /// 获取菜单
    /// </summary>
    Task<List<SysMenuType>> GetMenuType();

    /// <summary>
    /// 添加菜单类型
    /// </summary>
    Task AddMenuType(SysMenuType menuType);

    /// <summary>
    /// 删除菜单类型
    /// </summary>
    Task DeleteMenuType(DeleteMenuTypeParam param);

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    /// <returns></returns>
    Task<List<SysMenu>> GetMenus(GetMenusParam param);

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns></returns>
    Task<List<SysMenu>> GetMenuTreeByRsid(string rsid);

    /// <summary>
    /// 构建菜单树
    /// </summary>
    /// <param name="menus"></param>
    /// <param name="pid"></param>
    /// <returns></returns>
    List<SysMenu> BuildMenuTree(List<SysMenu> menus, string pid = null);

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<List<SysMenu>> GetAllMenus(GetMenusParam param);

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    /// <returns></returns>
    Task<List<SysMenu>> GetAllMenus();

    /// <summary>
    /// 保存菜单
    /// </summary>
    /// <returns></returns>
    Task SaveMenu(SysMenu menu);

    /// <summary>
    /// 删除菜单
    /// </summary>
    Task DeleteMenu(DeleteMenuParam param);

    /// <summary>
    /// 获取路由
    /// </summary>
    Task<List<Hashtable>> GetRouters(GetRoutersParam param);

    /// <summary>
    /// 查询所有子结构的菜单项
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="containMe"></param>
    /// <param name="allMenus"></param>
    /// <returns></returns>
    Task<List<SysMenu>> GetConcatMenu(List<string> ids, bool containMe = false, List<SysMenu> allMenus = null);

    /// <summary>
    /// 查询所有子结构的菜单项
    /// </summary>
    /// <param name="sids"></param>
    /// <param name="containMe"></param>
    /// <param name="allMenus"></param>
    /// <returns></returns>
    Task<List<SysMenu>> GetConcatMenuBySIDs(List<string> sids, bool containMe = false, List<SysMenu> allMenus = null);

    /// <summary>
    /// 查询所有父结构的菜单项
    /// </summary>
    Task<SysMenu> GetParentMenuBySIDs(string rsid, List<SysMenu> allMenus = null);

    /// <summary>
    /// 按照全路径获取菜单项
    /// </summary>
    Task<SysMenu> GetMenuByFullName(string fullPath, List<SysMenu> allMenus = null);

    /// <summary>
    /// 查询子菜单项
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="mustHave"></param>
    /// <returns></returns>
    Task<List<SysMenu>> GetChildMenuBySID(string sid, bool mustHave = true);

    /// <summary>
    /// 查询根菜单项
    /// </summary>
    /// <returns></returns>
    Task<List<SysMenu>> GetRootMenu();
}
