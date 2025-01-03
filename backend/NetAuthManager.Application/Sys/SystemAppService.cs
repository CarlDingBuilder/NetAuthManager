using BPM.FSSC.DBCore.Entity;
using NetAuthManager.Application.Params.Menus;
using NetAuthManager.Application.Sys.Params;
using NetAuthManager.Application.Sys.Params.Dictionarys;
using NetAuthManager.Application.Sys.Params.Menus;
using NetAuthManager.Application.Sys.Params.RejectReasons;
using NetAuthManager.Application.Sys.Params.Resources;
using NetAuthManager.Application.Sys.Params.RoleGroups;
using NetAuthManager.Application.Sys.Params.Roles;
using NetAuthManager.Application.Sys.Params.Staffs;
using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Application.Sys.Results.Resources;
using NetAuthManager.Application.Sys.Results.RoleGroups;
using NetAuthManager.Application.Sys.Results.Roles;
using NetAuthManager.Application.Sys.Results.Users;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.Core.Views.Sys;
using Furion.SpecificationDocument;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.ComponentModel;

namespace NetAuthManager.Application;

/// <summary>
/// 系统服务接口
/// </summary>
[ApiDescriptionSettings("SOPSysGroup@2")]
[Route("api/sys")]
public class SystemAppService : IDynamicApiController
{
    #region 构造和注入

    private readonly ISystemService _systemService;
    private readonly IUserLoginService _userLoginService;
    private readonly IMenuService _menuService;
    private readonly IResourcePermService _resourcePermService;
    private readonly IResourcePermCommonService _resourcePermCommonService;
    private readonly IUserManageService _userManageService;
    private readonly IRoleService _roleService;
    private readonly IRoleManageService _roleManageService;
    private readonly IRoleGroupService _roleGroupService;
    private readonly IRoleGroupManageService _roleGroupManageService;
    private readonly IDictionaryTypeService _dictionaryTypeService;
    private readonly IDictionaryItemService _dictionaryItemService;
    private readonly IDictionaryService _dictionaryService;

    public SystemAppService(ISystemService systemService, IUserLoginService userLoginService, IMenuService menuService, IResourcePermService resourcePermService, IResourcePermCommonService resourcePermCommonService,
        IUserManageService userService, 
        IRoleService roleService, IRoleManageService roleManageService, IRoleGroupService roleGroupService, IRoleGroupManageService roleGroupManageService, 
        IDictionaryTypeService dictionaryTypeService, IDictionaryItemService dictionaryItemService, IDictionaryService dictionaryService)
    {
        _systemService = systemService;
        _userLoginService = userLoginService;
        _menuService = menuService;
        _resourcePermService = resourcePermService;
        _resourcePermCommonService = resourcePermCommonService;
        _userManageService = userService;
        _roleService = roleService;
        _roleManageService = roleManageService;
        _roleGroupService = roleGroupService;
        _roleGroupManageService = roleGroupManageService;
        _dictionaryTypeService = dictionaryTypeService;
        _dictionaryItemService = dictionaryItemService;
        _dictionaryService = dictionaryService;
    }

    #endregion 构造和注入

    #region 系统

    /// <summary>
    /// 获取系统描述
    /// </summary>
    [ApiDescriptionSettings(Tag = "系统")]
    public string GetDescription()
    {
        return _systemService.GetDescription();
    }

    #endregion 系统

    #region 接口访问

    /// <summary>
    /// 用于访问 Swagger 文档，检测登陆接口有效性 
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [HttpPost, AllowAnonymous, NonUnify]
    [Route("check")]
    public int Check()
    {
        return 401;
    }

    /// <summary>
    /// 用于访问 Swagger 文档，登陆接口 
    /// </summary>
    /// <param name="auth"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [HttpPost, AllowAnonymous, NonUnify]
    [Route("login")]
    public int Login([FromForm] SpecificationAuth auth)
    {
        //if (auth.UserName != "sa")
        //{
        //    return 401;
        //}
        if (_userLoginService.Authenticate(new GetTokenParam
        {
            UserName = auth.UserName,
            Password = auth.Password,
        }))
        {
            _userLoginService.GetToken(new GetTokenParam
            {
                UserName = auth.UserName,
                Password = auth.Password,
            });
            return 200;
        }
        else
        {
            return 401;
        }
    }

    #endregion 接口访问

    #region 菜单

    /// <summary>
    /// 获取菜单类型
    /// </summary>
    [ApiDescriptionSettings(Tag = "菜单")]
    [Route("getMenuType")]
    [Description("获取菜单类型")]
    [HttpGet]
    public async Task<List<SysMenuType>> GetMenuType()
    {
        return await _menuService.GetMenuType();
    }

    /// <summary>
    /// 添加菜单类型
    /// </summary>
    [ApiDescriptionSettings(Tag = "菜单")]
    [Route("addMenuType")]
    [Description("添加菜单类型")]
    [HttpPost]
    public async Task AddMenuType([FromBody] SysMenuType param)
    {
        await _menuService.AddMenuType(param);
    }

    /// <summary>
    /// 删除菜单类型
    /// </summary>
    [ApiDescriptionSettings(Tag = "菜单")]
    [Route("deleteMenuType")]
    [Description("删除菜单类型")]
    [HttpPost]
    public async Task DeleteMenuType([FromBody] DeleteMenuTypeParam param)
    {
        await _menuService.DeleteMenuType(param);
    }

    /// <summary>
    /// 获取菜单
    /// </summary>
    [ApiDescriptionSettings(Tag = "菜单")]
    [Route("getMenus")]
    [Description("获取菜单")]
    [HttpGet]
    public async Task<List<SysMenu>> GetMenus([FromQuery] GetMenusParam param)
    {
        return await _menuService.GetMenus(param);
    }

    /// <summary>
    /// 保存菜单
    /// </summary>
    [ApiDescriptionSettings(Tag = "菜单")]
    [Route("saveMenu")]
    [Description("保存菜单")]
    [HttpPost]
    public async Task SaveMenu([FromBody] SysMenu param)
    {
        await _menuService.SaveMenu(param);
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    [ApiDescriptionSettings(Tag = "菜单")]
    [Route("deleteMenu")]
    [Description("删除菜单")]
    [HttpPost]
    public async Task DeleteMenu([FromBody] DeleteMenuParam param)
    {
        await _menuService.DeleteMenu(param);
    }

    /// <summary>
    /// 获取路由
    /// </summary>
    [ApiDescriptionSettings(Tag = "菜单")]
    [Route("getRouters")]
    [Description("获取路由")]
    [HttpGet]
    public async Task<List<Hashtable>> GetRouters([FromQuery] GetRoutersParam param)
    {
        return await _menuService.GetRouters(param);
    }

    #endregion 菜单

    #region 角色

    /// <summary>
    /// 角色权限全路径
    /// </summary>
    public const string RoleResurceFullPath = "基础管理/角色管理";

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取角色列表")]
    [HttpPost("getRolePageList")]
    public async Task<PageResult<SysRole>> GetRolePageList([FromBody] PageSearchParam param) => await _roleManageService.GetRolePageList(param);

    /// <summary>
    /// 获取角色列表，不包含Everyone
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取角色列表")]
    [HttpPost("getRoleNoEveryonePageList")]
    public async Task<PageResult<SysRole>> GetRoleNoEveryonePageList([FromBody] PageSearchParam param) => await _roleManageService.GetRoleNoEveryonePageList(param);

    /// <summary>
    /// 获取所有角色列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取所有角色列表")]
    [HttpPost("getAllRolePageList")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath)]
    public async Task<PageResult<SysRole>> GetAllRolePageList([FromBody] PageSearchParam param) => await _roleManageService.GetAllRolePageList(param);

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取角色列表")]
    [HttpPost("getRoles")]
    public async Task<List<SysRole>> GetRoles() => await _roleManageService.GetRoles();

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("添加角色")]
    [HttpPost("addRole")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, Perm: PermTypeEnum.New)]
    public async Task AddRole(RoleAddParam param) => await _roleManageService.AddRole(param);

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("修改角色")]
    [HttpPost("editRole")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task EditRole(RoleModifyParam param) => await _roleManageService.ModifyRole(param);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("删除角色")]
    [HttpPost("deleteRole")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteRole(RoleBaseParam param) => await _roleManageService.DeleteRole(param);

    /// <summary>
    /// 删除多个角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("删除多个角色")]
    [HttpPost("deleteRoles")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteRoles(RoleDeletesParam param) => await _roleManageService.DeleteRoles(param);

    /// <summary>
    /// 设置是否启用
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("设置是否启用")]
    [HttpPost("setIsOpen")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task SetIsOpen(Sys.Params.Roles.SetIsOpenParam param) => await _roleManageService.SetIsOpen(param);

    /// <summary>
    /// 获取角色成员列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取角色成员列表")]
    [HttpPost("getRoleMembers")]
    public async Task<List<RoleMemberResult>> GetRoleMembers(RoleBaseParam param) => await _roleManageService.GetRoleMembers(param);

    /// <summary>
    /// 获取角色用户列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取角色用户列表")]
    [HttpPost("getRoleUsers")]
    public async Task<List<GetRoleUsersItem>> GetRoleUsers(RoleBaseParam param) => await _roleService.GetRoleUsersAsync(param);

    /// <summary>
    /// 获取角色用户分页列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取角色用户分页列表")]
    [HttpPost("getRoleUserPageList")]
    public async Task<PageResult<GetRoleUsersItemByRole>> GetRoleUserPageList([FromBody] PageSearchParam param) => await _roleService.GetRoleUserPageList(param);

    /// <summary>
    /// 添加角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("添加角色成员")]
    [HttpPost("addRoleMember")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, PermName: "Assign")]
    public async Task AddRoleMember(RoleMemberAddParam param) => await _roleManageService.AddRoleMember(param);

    /// <summary>
    /// 添加多个角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("添加多个角色成员")]
    [HttpPost("addRoleMembers")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, PermName: "Assign")]
    public async Task AddRoleMembers(RoleMembersAddParam param) => await _roleManageService.AddRoleMembers(param);

    /// <summary>
    /// 删除角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("删除角色成员")]
    [HttpPost("deleteRoleMember")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, PermName: "Assign")]
    public async Task DeleteRoleMember(RoleMemberDeleteParam param) => await _roleManageService.DeleteRoleMember(param);

    /// <summary>
    /// 删除多个角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("删除多个角色成员")]
    [HttpPost("deleteRoleMembers")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, PermName: "Assign")]
    public async Task DeleteRoleMembers(RoleMemberDeletesParam param) => await _roleManageService.DeleteRoleMembers(param);

    /// <summary>
    /// 保存角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("保存角色成员")]
    [HttpPost("saveRoleMembers")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, PermName: "Assign")]
    public async Task SaveRoleMembers(SaveRoleMembersParam param) => await _roleManageService.SaveRoleMembers(param);

    /// <summary>
    /// 获取所属角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("获取所属角色")]
    [HttpPost("getBelongRoles")]
    public async Task<List<BelongRolesResult>> GetBelongRoles(RoleCheckParam param) => await _roleService.GetBelongRoles(param);

    /// <summary>
    /// 是否是角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("是否是角色成员")]
    [HttpPost("isRoleMember")]
    public async Task<IsBelongRoleResult> IsRoleMember(RoleCheckIsMemberParam param) => await _roleService.IsRoleMember(param);

    /// <summary>
    /// 是否是管理员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("是否是管理员")]
    [HttpPost("isAdministrator")]
    public async Task<IsBelongRoleResult> IsAdministrator(RoleCheckParam param) => await _roleService.IsAdministrator(param);

    /// <summary>
    /// 是否是管理员登录
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色")]
    [Description("是否是管理员登录")]
    [HttpPost("isAdministratorLogin")]
    public async Task<IsBelongRoleResult> IsAdministratorLogin() => await _roleService.IsAdministratorLogin();

    #endregion 角色

    #region 角色组

    /// <summary>
    /// 角色组权限全路径
    /// </summary>
    public const string RoleGroupResurceFullPath = "基础管理/角色组管理";

    /// <summary>
    /// 获取角色组列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("获取角色列表")]
    [HttpPost("getRoleGroupPageList")]
    public async Task<PageResult<SysRoleGroup>> GetRoleGroupPageList([FromBody] PageSearchParam param) => await _roleGroupManageService.GetRoleGroupPageList(param);

    /// <summary>
    /// 获取所有角色列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("获取所有角色列表")]
    [HttpPost("getAllRoleGroupPageList")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath)]
    public async Task<PageResult<SysRoleGroup>> GetAllRoleGroupPageList([FromBody] PageSearchParam param) => await _roleGroupManageService.GetAllRoleGroupPageList(param);

    /// <summary>
    /// 获取角色组列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("获取角色组列表")]
    [HttpPost("getRoleGroups")]
    public async Task<List<SysRoleGroup>> GetRoleGroups() => await _roleGroupManageService.GetRoleGroups();

    /// <summary>
    /// 获取所属角色组列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("获取所属角色组列表")]
    [HttpPost("getOwerRoleGroups")]
    public async Task<List<SysRoleGroup>> GetOwerRoleGroups() => await _roleGroupManageService.GetOwerRoleGroupsAsync();

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("添加角色")]
    [HttpPost("addRoleGroup")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, Perm: PermTypeEnum.New)]
    public async Task AddRoleGroup(RoleGroupAddParam param) => await _roleGroupManageService.AddRoleGroup(param);

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("修改角色")]
    [HttpPost("editRoleGroup")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task EditRoleGroup(RoleGroupModifyParam param) => await _roleGroupManageService.ModifyRoleGroup(param);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("删除角色")]
    [HttpPost("deleteRoleGroup")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteRoleGroup(RoleGroupBaseParam param) => await _roleGroupManageService.DeleteRoleGroup(param);

    /// <summary>
    /// 删除多个角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("删除多个角色")]
    [HttpPost("deleteRoleGroups")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteRoleGroups(RoleGroupDeletesParam param) => await _roleGroupManageService.DeleteRoleGroups(param);

    /// <summary>
    /// 设置是否启用
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("设置是否启用")]
    [HttpPost("setRoleGroupIsOpen")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task SetRoleGroupIsOpen(SetRoleGroupIsOpenParam param) => await _roleGroupManageService.SetIsOpen(param);

    /// <summary>
    /// 获取角色组成员列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("获取角色成员列表")]
    [HttpPost("getRoleGroupMembers")]
    public async Task<List<RoleGroupMemberResult>> GetRoleGroupMembers(RoleGroupBaseParam param) => await _roleGroupManageService.GetRoleGroupMembers(param);

    /// <summary>
    /// 获取角色组成员列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("获取角色组角色成员列表")]
    [HttpPost("getRoleGroupRoles")]
    public async Task<List<RoleGroupRoleItem>> GetRoleGroupRoles(RoleGroupBaseParam param) => await _roleGroupManageService.GetRoleGroupRoles(param);

    /// <summary>
    /// 添加角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("添加角色成员")]
    [HttpPost("addRoleGroupMember")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, PermName: "Assign")]
    public async Task AddRoleGroupMember(RoleGroupMemberAddParam param) => await _roleGroupManageService.AddRoleGroupMember(param);

    /// <summary>
    /// 添加多个角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("添加多个角色成员")]
    [HttpPost("addRoleGroupMembers")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, PermName: "Assign")]
    public async Task AddRoleGroupMembers(RoleGroupMembersAddParam param) => await _roleGroupManageService.AddRoleGroupMembers(param);

    /// <summary>
    /// 删除角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("删除角色成员")]
    [HttpPost("deleteRoleGroupMember")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, PermName: "Assign")]
    public async Task DeleteRoleGroupMember(RoleGroupMemberDeleteParam param) => await _roleGroupManageService.DeleteRoleGroupMember(param);

    /// <summary>
    /// 删除多个角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("删除多个角色成员")]
    [HttpPost("deleteRoleGroupMembers")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, PermName: "Assign")]
    public async Task DeleteRoleGroupMembers(RoleGroupMemberDeletesParam param) => await _roleGroupManageService.DeleteRoleGroupMembers(param);

    /// <summary>
    /// 保存角色组成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("保存角色组成员")]
    [HttpPost("saveRoleGroupMembers")]
    [PermAuthorize(PermFullPath: RoleGroupResurceFullPath, PermName: "Assign")]
    public async Task SaveRoleGroupMembers(SaveRoleGroupMembersParam param) => await _roleGroupManageService.SaveRoleGroupMembers(param);

    /// <summary>
    /// 获取所属角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("获取所属角色")]
    [HttpPost("getBelongRoleGroups")]
    public async Task<List<BelongRoleGroupsResult>> GetBelongRoleGroups(RoleGroupCheckParam param) => await _roleGroupService.GetBelongRoleGroups(param);

    /// <summary>
    /// 是否是角色成员
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "角色组")]
    [Description("是否是角色成员")]
    [HttpPost("isRoleGroupMember")]
    public async Task<IsBelongRoleGroupResult> IsRoleGroupMember(RoleGroupCheckIsMemberParam param) => await _roleGroupService.IsRoleGroupMember(param);

    #endregion 角色

    #region 用户

    /// <summary>
    /// 用户权限全路径
    /// </summary>
    public const string UserResurceFullPath = "基础管理/用户管理";

    /// <summary>
    /// 获取用户列表
    /// </summary>
    [ApiDescriptionSettings(Tag = "用户")]
    [Route("getAllUserPageList")]
    [Description("获取用户列表")]
    [HttpPost]
    [PermAuthorize(PermFullPath: UserResurceFullPath)]
    public async Task<PageResult<SysUser>> GetAllUserPageList([FromBody] PageSearchParam param)
    {
        return await _userManageService.GetAllPageList(param);
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    [ApiDescriptionSettings(Tag = "用户")]
    [Route("getUserPageList")]
    [Description("获取用户列表")]
    [HttpPost]
    public async Task<PageResult<SysUser>> GetUserPageList([FromBody] PageSearchParam param)
    {
        return await _userManageService.GetPageList(param);
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("添加用户")]
    [HttpPost("addUser")]
    [PermAuthorize(PermFullPath: UserResurceFullPath, Perm: PermTypeEnum.New)]
    public async Task AddUser(UserAddParam param) => await _userManageService.AddUser(param);

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("修改用户")]
    [HttpPost("editUser")]
    [PermAuthorize(PermFullPath: UserResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task EditUser(UserModifyParam param) => await _userManageService.ModifyUser(param);

    /// <summary>
    /// 修改用户密码
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("修改用户密码")]
    [HttpPost("editPassword")]
    [PermAuthorize(PermFullPath: UserResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task ModifyPassword(UserModifyPasswordParam param) => await _userManageService.ModifyPassword(param);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("删除用户")]
    [HttpPost("deleteUser")]
    [PermAuthorize(PermFullPath: UserResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteUser(UserBaseParam param) => await _userManageService.DeleteUser(param);

    /// <summary>
    /// 删除多个用户
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("删除多个用户")]
    [HttpPost("deleteUsers")]
    [PermAuthorize(PermFullPath: UserResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteUsers(UserDeletesParam param) => await _userManageService.DeleteUsers(param);

    /// <summary>
    /// 设置是否启用
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("设置用户是否启用")]
    [HttpPost("setUserIsOpen")]
    [PermAuthorize(PermFullPath: UserResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task SetUserIsOpen(SetUserIsOpenParam param) => await _userManageService.SetUserIsOpen(param);

    /// <summary>
    /// 获取用户角色列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("获取用户角色列表")]
    [HttpPost("getUserRoles")]
    public async Task<List<UserRoleItem>> GetUserRoles(UserBaseParam param) => await _userManageService.GetUserRoles(param);

    /// <summary>
    /// 用户分配角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "用户")]
    [Description("用户分配角色")]
    [HttpPost("saveUserRoles")]
    [PermAuthorize(PermFullPath: RoleResurceFullPath, PermName: "Assign")]
    public async Task SaveUserRoles(SaveUserRolesParam param) => await _userManageService.SaveUserRoles(param);

    #endregion 用户

    #region 资源

    /// <summary>
    /// 获取资源树
    /// </summary>
    [ApiDescriptionSettings(Tag = "资源")]
    [Route("getResources")]
    [Description("获取资源树")]
    [HttpGet]
    public async Task<List<ResurceNodeItem>> GetResources([FromQuery] GetResourcesParam param)
    {
        return await _resourcePermService.GetResources(param);
    }

    /// <summary>
    /// 获取资源权限项列表
    /// </summary>
    [ApiDescriptionSettings(Tag = "资源")]
    [Route("getResourcePerms")]
    [Description("获取资源权限项列表")]
    [HttpPost]
    public async Task<ResourcePermItemBoxDto> GetResourcePerms([FromBody] ResourceBaseParam param)
    {
        var sidType = _resourcePermCommonService.GetSIDType(param.SIDType);
        var resource = await _resourcePermService.GetResourceInfo(param.SID, sidType);
        var perms = await _resourcePermService.GetResourcePerms(param.SID, sidType);
        var permTypes = await _resourcePermService.GetResourcePermTypes();
        var logs = await _resourcePermService.GetResourcePermLog();
        return new ResourcePermItemBoxDto()
        {
            Resource = resource,
            Perms = perms,
            PermTypes = permTypes,
            PermLogs = logs
        };
    }

    /// <summary>
    /// 保存资源权限项列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("保存资源权限项列表")]
    [HttpPost("saveResourcePerms")]
    public async Task SaveResourcePerms([FromBody] ResourcePermSaveParam param)
    {
        await _resourcePermService.SaveResourcePerms(param);
    }

    /// <summary>
    /// 获取资源权限维护的角色信息：角色、部门、用户
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("获取资源权限维护的角色信息")]
    [HttpPost("getResourcePermRoles")]
    public async Task<ResourcePermRolesBoxResult> GetResourcePermRoles([FromBody] ResourceBaseParam param)
    {
        var resource = await _resourcePermService.GetResourceInfo(param.SID, _resourcePermCommonService.GetSIDType(param.SIDType));
        var roles = await _resourcePermService.GetResourcePermRoles(param.SID);
        var model = new ResourcePermRolesBoxResult()
        {
            Resource = resource,
            Roles = roles
        };
        return model;
    }

    /// <summary>
    /// 获取资源角色权限信息
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("获取资源角色权限信息")]
    [HttpPost("getResourcePermRoleAcls")]
    public async Task<List<ResourcePermAclsDto>> GetResourcePermRoleAcls([FromBody] ResourcePermAclsGetParam param)
    {
        var roles = await _resourcePermService.GetResourcePermRoleAcls(param);
        return roles;
    }

    /// <summary>
    /// 添加资源角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("添加资源角色")]
    [HttpPost("addResourcePermRole")]
    public async Task AddResourcePermRole([FromBody] ResourcePermRoleAddParam param)
    {
        await _resourcePermService.AddResourcePermRole(param);
    }

    /// <summary>
    /// 删除资源角色
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("删除资源角色")]
    [HttpPost("deleteResourcePermRole")]
    public async Task DeleteResourcePermRole([FromBody] ResourcePermRoleDeleteParam param)
    {
        await _resourcePermService.DeleteResourcePermRole(param);
    }

    /// <summary>
    /// 保存资源权限
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("保存资源权限")]
    [HttpPost("saveAcls")]
    public async Task SaveAcls([FromBody] ResourcePermAclsSaveParam param)
    {
        await _resourcePermService.SaveAcls(param);
    }

    /// <summary>
    /// 检查访问权限
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("检查访问权限")]
    [HttpPost("checkExecutePermision")]
    public async Task<ResourcePermCheckDto> CheckExecutePermision([FromBody] ResourcePermCheckParam param)
    {
        var havePerm = await (string.IsNullOrEmpty(param.Account)
            ? _resourcePermCommonService.CheckExecutePermision(param.SID)
            : _resourcePermCommonService.CheckExecutePermision(param.SID, param.Account));
        return new ResourcePermCheckDto
        {
            HavePerm = havePerm
        };
    }

    /// <summary>
    /// 检查操作权限
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("检查操作权限")]
    [HttpPost("checkPermision")]
    public async Task<ResourcePermCheckDto> CheckPermision([FromBody] ResourcePermCheckPermParam param)
    {
        var havePerm = await (string.IsNullOrEmpty(param.Account)
            ? _resourcePermCommonService.CheckPermision(param.SID, param.PermName)
            : _resourcePermCommonService.CheckPermision(param.SID, param.Account, param.PermName));
        return new ResourcePermCheckDto
        {
            HavePerm = havePerm
        };
    }

    /// <summary>
    /// 检查多个操作权限
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "资源")]
    [Description("检查多个操作权限")]
    [HttpPost("checkPermisions")]
    public async Task<ResourcePermChecksDto> CheckPermisions([FromBody] ResourcePermCheckPermsParam param)
    {
        var perms = await (string.IsNullOrEmpty(param.Account)
            ? _resourcePermCommonService.CheckPermisionBack(param.SID, param.PermNames)
            : _resourcePermCommonService.CheckPermisionBack(param.SID, param.Account, param.PermNames));
        return new ResourcePermChecksDto
        {
            Perms = perms
        };
    }

    #endregion 资源

    #region 数据字典

    /// <summary>
    /// 数据字典权限全路径
    /// </summary>
    public const string DictionaryResurceFullPath = "系统管理/数据字典";

    /// <summary>
    /// 获取数据字典列表
    /// </summary>
    [ApiDescriptionSettings(Tag = "数据字典")]
    [Route("getDictionaryList")]
    [Description("获取数据字典类型列表")]
    [HttpPost]
    public async Task<List<ViewSysDictionary>> GetDictionaryList([FromBody] DictionaryParam param)
    {
        return await _dictionaryService.GetList(param.TypeCode, param.Ext01, param.Ext02, param.Ext03, param.Ext04, param.Ext05);
    }

    /// <summary>
    /// 获取数据字典类型列表
    /// </summary>
    [ApiDescriptionSettings(Tag = "数据字典-字典类型")]
    [Route("getDictionaryTypePageList")]
    [Description("获取数据字典类型列表")]
    [HttpPost]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath)]
    public async Task<PageResult<SysDictionaryType>> GetDictionaryTypePageList([FromBody] PageSearchParam param)
    {
        return await _dictionaryTypeService.GetPageList(param);
    }

    /// <summary>
    /// 添加数据字典类型
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典类型")]
    [Description("添加数据字典类型")]
    [HttpPost("addDictionaryType")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.New)]
    public async Task AddDictionaryType(DictionaryTypeAddParam param) => await _dictionaryTypeService.Add(param);

    /// <summary>
    /// 修改数据字典类型
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典类型")]
    [Description("修改数据字典类型")]
    [HttpPost("editDictionaryType")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task EditDictionaryType(DictionaryTypeModifyParam param) => await _dictionaryTypeService.Modify(param);

    /// <summary>
    /// 删除数据字典类型
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典类型")]
    [Description("删除数据字典类型")]
    [HttpPost("deleteDictionaryType")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteDictionaryType(DictionaryTypeBaseParam param) => await _dictionaryTypeService.Delete(param);

    /// <summary>
    /// 删除多个数据字典类型
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典类型")]
    [Description("删除多个数据字典类型")]
    [HttpPost("deleteDictionaryTypes")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteDictionaryTypes(DictionaryTypeDeletesParam param) => await _dictionaryTypeService.Deletes(param);

    /// <summary>
    /// 设置是否启用
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典类型")]
    [Description("设置数据字典类型是否启用")]
    [HttpPost("setDictionaryTypeIsOpen")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task SetDictionaryTypeIsOpen(SetDictionaryTypeIsOpenParam param) => await _dictionaryTypeService.SetIsOpen(param);

    /// <summary>
    /// 获取数据字典项列表
    /// </summary>
    [ApiDescriptionSettings(Tag = "数据字典-字典项")]
    [Route("getDictionaryItemPageList")]
    [Description("获取数据字典项列表")]
    [HttpPost]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath)]
    public async Task<PageResult<SysDictionaryItem>> GetDictionaryItemPageList([FromBody] PageSearchParam param)
    {
        return await _dictionaryItemService.GetPageList(param);
    }

    /// <summary>
    /// 添加数据字典项
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典项")]
    [Description("添加数据字典项")]
    [HttpPost("addDictionaryItem")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.New)]
    public async Task AddDictionaryItem(DictionaryItemAddParam param) => await _dictionaryItemService.Add(param);

    /// <summary>
    /// 修改数据字典项
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典项")]
    [Description("修改数据字典项")]
    [HttpPost("editDictionaryItem")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task EditDictionaryItem(DictionaryItemModifyParam param) => await _dictionaryItemService.Modify(param);

    /// <summary>
    /// 删除数据字典项
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典项")]
    [Description("删除数据字典项")]
    [HttpPost("deleteDictionaryItem")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteDictionaryItem(DictionaryItemBaseParam param) => await _dictionaryItemService.Delete(param);

    /// <summary>
    /// 删除多个数据字典项
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典项")]
    [Description("删除多个数据字典项")]
    [HttpPost("deleteDictionaryItems")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Delete)]
    public async Task DeleteDictionaryItems(DictionaryItemDeletesParam param) => await _dictionaryItemService.Deletes(param);

    /// <summary>
    /// 设置是否启用
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典项")]
    [Description("设置数据字典项是否启用")]
    [HttpPost("setDictionaryItemIsOpen")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Edit)]
    public async Task SetDictionaryItemIsOpen(SetDictionaryItemIsOpenParam param) => await _dictionaryItemService.SetIsOpen(param);

    /// <summary>
    /// 字典项排序
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Tag = "数据字典-字典项")]
    [Description("字典项排序")]
    [HttpPost("orderDictionaryItem")]
    [PermAuthorize(PermFullPath: DictionaryResurceFullPath, Perm: PermTypeEnum.Edit)]
    public void OrderDictionaryItem(DictionaryItemOrderParam param) => _dictionaryItemService.Order(param);

    #endregion 数据字典
}
