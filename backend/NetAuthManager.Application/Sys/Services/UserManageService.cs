using NetAuthManager.Application.Sys.Consts;
using NetAuthManager.Application.Sys.Mappers;
using NetAuthManager.Application.Sys.Params;
using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Application.Sys.Results.DataSync;
using NetAuthManager.Application.Sys.Results.Users;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Expressions;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.Core.Services;
using NetAuthManager.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NetAuthManager.Application.Common.Helpers;

namespace NetAuthManager.Application;

/// <summary>
/// 用户服务
/// </summary>
public class UserManageService : BaseService<SysUser>, IUserManageService, ITransient
{
    #region 构造与注入

    private readonly IUserLoginService _userLoginService;
    private readonly ILogger<UserManageService> _logger;
    private readonly IMenuResourceService _menuResourceService;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysRoleMember> _roleMemberRepository;
    private readonly IRepository<SysResourcePermAcl> _resourceAclRepository;

    public UserManageService(ILogger<UserManageService> logger, IMenuResourceService menuResourceService,
        IServiceScopeFactory scopeFactory, IUserLoginService userLoginService,
        IRepository<SysUser> userRepository, 
        IRepository<SysRole> roleRepository, IRepository<SysRoleMember> roleMemberRepository, IRepository<SysResourcePermAcl> resourceAclRepository) : base(userRepository, scopeFactory)
    {
        _logger = logger;
        _userLoginService = userLoginService;
        _roleRepository = roleRepository;
        _menuResourceService = menuResourceService;
        _roleMemberRepository = roleMemberRepository;
        _resourceAclRepository = resourceAclRepository;
    }

    #endregion 构造与注入

    /// <summary>
    /// 获取分页列表
    /// </summary>
    public override async Task<PageResult<SysUser>> GetAllPageList(PageSearchParam param)
    {
        //获取权限
        var perms = await _menuResourceService.GetResourceBizAllowPermNames();

        //权限构建
        var permExpression = ExpressionParserEx<SysUser>.ParserConditionsByPerms(perms, "user");

        //查询条件
        var whereExpression = ExpressionParser<SysUser>.ParserConditions(param.filters, "user");

        var list = await _entityRepository.Where(permExpression).Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        return PageResult<SysUser>.Get(list: list.Items.ToList(), total: list.TotalCount);
    }

    /// <summary>
    /// 获取分页列表，仅启用用户
    /// 公开访问，无需权限
    /// </summary>
    public override async Task<PageResult<SysUser>> GetPageList(PageSearchParam param)
    {
        //查询条件
        var whereExpression = ExpressionParser<SysUser>.ParserConditions(param.filters, "user");

        var list = await _entityRepository.Where(x => x.IsOpen)
            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        return PageResult<SysUser>.Get(list: list.Items.ToList(), total: list.TotalCount);
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    public async Task AddUser(UserAddParam param)
    {
        if (string.IsNullOrEmpty(param.Account))
            throw new Exception("用户账号不能为空！");

        if (await AnyAsync(x => x.Account == param.Account))
            throw new Exception($"用户账号 “{param.Account}” 已存在，不能重复新增！");


        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //员工信息
        var roleModel = new SysUser()
        {
            Account = param.Account,
            HRID = param.HRID,
            Name = param.Name,
            IsOpen = param.IsOpen,
            Description = param.Description,
            CreatorAccount = loginUser.UserAccount,
            CreatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName,
            CreatedTime = DateTime.Now,
        };

        //新增用户
        await _entityRepository.InsertNowAsync(roleModel);
    }

    /// <summary>
    /// 重命名用户
    /// </summary>
    public async Task ModifyUser(UserModifyParam param)
    {
        if (string.IsNullOrEmpty(param.Account))
            throw new Exception("用户账号不能为空！");

        //账号信息
        var user = (from entity in _entityRepository.Entities
                    where entity.Account == param.Account
                    select entity).FirstOrDefault();
        if (user == null)
            throw new Exception($"用户账号 “{param.Account}” 不存在，无法修改！");
        
        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        user.Account = param.Account;
        user.IsOpen = param.IsOpen;
        user.Description = param.Description;
        user.UpdatorAccount = loginUser.UserAccount;
        user.UpdatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName;
        user.UpdatedTime = DateTime.Now;
        await _entityRepository.UpdateNowAsync(user);
    }

    /// <summary>
    /// 修改用户密码
    /// </summary>
    public async Task ModifyPassword(UserModifyPasswordParam param)
    {
        if (string.IsNullOrEmpty(param.Account))
            throw new Exception("用户账号不能为空！");

        if (string.IsNullOrEmpty(param.Password))
            throw new Exception("密码不能为空！");

        if (param.Password != param.PasswordAgain)
            throw new Exception("两次输入的密码不一致！");

        //账号信息
        var user = (from entity in _entityRepository.Entities
                    where entity.Account == param.Account
                    select entity).FirstOrDefault();
        if (user == null)
            throw new Exception($"用户账号 “{param.Account}” 不存在，无法修改密码！");

        if (user.IsSync)
            throw new Exception($"用户账号 “{param.Account}” 为同步账号，无法修改密码！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        user.Account = param.Account;
        user.Password = MD5Helper.GetMd5(param.Password);
        user.UpdatorAccount = loginUser.UserAccount;
        user.UpdatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName;
        user.UpdatedTime = DateTime.Now;
        await _entityRepository.UpdateNowAsync(user);
    }

    /// <summary>
    /// 设置用户是否启用
    /// </summary>
    public async Task SetUserIsOpen(SetUserIsOpenParam param)
    {
        if (string.IsNullOrEmpty(param.Account))
            throw new Exception("用户账号不能为空！");

        //账号信息
        var user = (from entity in _entityRepository.Entities
                    where entity.Account == param.Account
                    select entity).FirstOrDefault();
        if (user == null)
            throw new Exception($"用户账号 “{param.Account}” 不存在，无法修改！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        user.IsOpen = param.IsOpen;
        user.UpdatorAccount = loginUser.UserAccount;
        user.UpdatorName = loginUser.UserName;
        user.UpdatedTime = DateTime.Now;
        await _entityRepository.UpdateNowAsync(user);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task DeleteUser(UserBaseParam param)
    {
        if (string.IsNullOrEmpty(param.Account))
            throw new Exception("用户账号不能为空！");

        var user = (from entity in _entityRepository.Entities
                    where entity.Account == param.Account
                    select entity).FirstOrDefault();
        if (user != null)
        {
            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除角色
                await _entityRepository.DeleteAsync(user);

                //删除角色成员
                await _roleMemberRepository.Where(entity => entity.SID == user.SID).ExecuteDeleteAsync();

                //删除角色对应的授权
                await _resourceAclRepository.Where(entity => entity.RoleType == Core.Common.Enums.SIDTypeEnum.UserSID.ToString() && entity.RoleParam1 == user.SID).ExecuteDeleteAsync();
            });
        }
    }

    /// <summary>
    /// 删除多个用户
    /// </summary>
    public async Task DeleteUsers(UserDeletesParam param)
    {
        if (param.Accounts == null || param.Accounts.Count == 0)
            throw new Exception("用户账号不能为空！");

        var users = (from entity in _entityRepository.Entities
                     where param.Accounts.Contains(entity.Account)
                     select entity).ToList();

        if (users != null && users.Count > 0)
        {
            //删除用户对应的授权
            var userSids = users.Select(r => r.SID).ToList();

            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除角色
                await _entityRepository.DeleteAsync(users);

                //删除的角色组成员
                await _roleMemberRepository.Where(entity => userSids.Contains(entity.SID)).ExecuteDeleteAsync();

                //删除角色组对应的授权
                await _resourceAclRepository.Where(entity => entity.RoleType == Core.Common.Enums.SIDTypeEnum.UserSID.ToString() && userSids.Contains(entity.RoleParam1)).ExecuteDeleteAsync();
            });
        }
    }

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<List<UserRoleItem>> GetUserRoles(UserBaseParam param)
    {
        if (string.IsNullOrEmpty(param.Account))
            throw new Exception("用户账号不能为空！");

        //用户
        var user = await (from entity in _entityRepository.Entities
                          where entity.Account == param.Account
                          select entity).FirstOrDefaultAsync();
        if (user == null)
            throw new Exception("用户账号未找到！");

        //现有所属的角色
        var userRoles = new List<UserRoleItem>();
        var roleMembers = await (from entity in _roleMemberRepository.Entities
                                 join role in _roleRepository.Entities on entity.RoleCode equals role.RoleCode into roleGroup
                                 from roleEntity in roleGroup.DefaultIfEmpty()
                                 where entity.SID == user.SID && entity.SIDType == SIDTypeEnum.UserSID.ToString()
                                 select roleEntity).ToListAsync();
        foreach(var roleMember in roleMembers)
        {
            userRoles.Add(new UserRoleItem
            {
                RoleCode = roleMember.RoleCode,
                DisplayName = roleMember.RoleName
            });
        }
        return userRoles;
    }

    /// <summary>
    /// 保存用户角色
    /// </summary>
    public async Task SaveUserRoles(SaveUserRolesParam param)
    {
        if (string.IsNullOrEmpty(param.Account))
            throw new Exception("用户账号不能为空！");

        if (param.RoleCodes.Contains(ConstNames.RoleCode.Everyone))
            throw new Exception("Everyone 角色不能添加成员！");

        //用户
        var user = await (from entity in _entityRepository.Entities
                          where entity.Account == param.Account
                          select entity).FirstOrDefaultAsync();
        if (user == null)
            throw new Exception("用户账号未找到！");

        //角色
        var roles = await (from entity in _roleRepository.Entities
                     where param.RoleCodes.Contains(entity.RoleCode)
                     select entity).ToListAsync();
        foreach(var roleCode in param.RoleCodes)
        {
            if (roleCode == ConstNames.RoleCode.Administrators)
            {
                roles.Add(new SysRole {
                    RoleCode = ConstNames.RoleCode.Administrators,
                    RoleName = ConstNames.RoleName.Administrators
                });
                break;
            }

            var role = roles.FirstOrDefault(x => x.RoleCode == roleCode);
            if (role == null)
                throw new Exception($"未找到角色 “{roleCode}”！");
        }

        //现有所属的角色
        var roleMembers = await (from entity in _roleMemberRepository.Entities
                                 where param.RoleCodes.Contains(entity.RoleCode) && entity.SID == user.SID && entity.SIDType == SIDTypeEnum.UserSID.ToString()
                                 select entity).ToListAsync();

        //构建
        var insertMembers = new List<SysRoleMember>();
        var updateMembers = new List<SysRoleMember>();
        var deleteMembers = new List<SysRoleMember>();
        for (var index = 0; index < param.RoleCodes.Count; index++)
        {
            var role = roles.First(x => x.RoleCode == param.RoleCodes[index]);
            var memberSIDType = SIDTypeEnum.UserSID.ToString();
            var oldMember = roleMembers.FirstOrDefault(entity => entity.RoleCode == role.RoleCode && entity.SID == user.SID && entity.SIDType == memberSIDType);
            if (oldMember == null)
            {
                insertMembers.Add(new SysRoleMember
                {
                    RoleCode = role.RoleCode,
                    SID = user.SID,
                    SIDType = memberSIDType,
                    OrderIndex = index + 1,
                });
            }
            else
            {
                oldMember.OrderIndex = index + 1;
                updateMembers.Add(oldMember);
            }
        }
        foreach (var member in roleMembers)
        {
            if (!param.RoleCodes.Contains(member.RoleCode))
            {
                deleteMembers.Add(member);
            }
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            if (insertMembers.Count > 0) await _roleMemberRepository.InsertAsync(insertMembers);
            if (updateMembers.Count > 0) await _roleMemberRepository.UpdateAsync(updateMembers);
            if (deleteMembers.Count > 0) await _roleMemberRepository.DeleteAsync(deleteMembers);
        });
    }

    /// <summary>
    /// 同步用户
    /// 仅新增、更新，并不删除
    /// </summary>
    public async Task<SyncResult> SyncUsers(List<UserSyncParam> param)
    {
        var guid = Guid.NewGuid().ToString();

        //新增、修改（没有的修改状态，有的修改数据
        var shouldInsertUsers = new List<SysUser>();
        var shouldUpdateUsers = new List<SysUser>();

        var users = await _entityRepository.DetachedEntities.ToListAsync();
        foreach (var item in param)
        {
            var user = users.FirstOrDefault(x => x.Account == item.Account);
            if (user == null)
            {
                if (!shouldInsertUsers.Any(x => x.Account == item.Account))
                {
                    user = item.Adapt<SysUser>();
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        user.Password = user.Password.TrimEnd();
                    }
                    user.IsSync = true;
                    user.CreatorAccount = SysConst.SysAdminAccount;
                    user.CreatorName = SysConst.SysAdminDisplayName;
                    shouldInsertUsers.Add(user);
                }
            }
            else
            {
                user = item.Adapt(user);
                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.Password = user.Password.TrimEnd();
                }
                user.IsSync = true;
                user.UpdatorAccount = SysConst.SysAdminAccount;
                user.UpdatorName = SysConst.SysAdminDisplayName;
                user.UpdatedTime = DateTime.Now;
                shouldUpdateUsers.Add(user);
            }
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            if (shouldInsertUsers.Count > 0) await _entityRepository.InsertAsync(shouldInsertUsers);
            if (shouldUpdateUsers.Count > 0) await _entityRepository.UpdateAsync(shouldUpdateUsers);
        });

        return new SyncResult(guid);
    }
}
