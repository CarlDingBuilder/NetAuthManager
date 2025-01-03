using NetAuthManager.Application.Sys.Params.Resources;
using NetAuthManager.Application.Sys.Params.Roles;
using NetAuthManager.Application.Sys.Results.Roles;
using NetAuthManager.Application.Sys.Results.Users;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Consts;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Expressions;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.Core.Services;
using NetAuthManager.EntityFramework.Core;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 角色服务
/// </summary>
public class RoleService : BaseService<SysRole>, IRoleService, ITransient
{
    #region 构造和注入

    /// <summary>
    /// 系统角色编码列表
    /// </summary>
    private string[] SystemRoleCodes = new string[2] { ConstNames.RoleCode.Administrators, ConstNames.RoleCode.Everyone };

    /// <summary>
    /// 系统角色SID列表
    /// </summary>
    private string[] SystemRoleSIDs = new string[2] { ConstRSIDs.RoleSID.Administrators, ConstRSIDs.RoleSID.Everyone };

    private readonly ILogger<RoleService> _logger;

    private readonly IUserLoginService _userLoginService;
    private readonly IUserService _userService;
    private readonly IRepository<SysUser> _userRepository;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysRoleMember> _roleMemberRepository;
    private readonly IRepository<SysRoleGroupMember> _roleGroupMemberRepository;
    private readonly IRepository<SysResourcePermAcl> _resourceAclRepository;

    public RoleService(ILogger<RoleService> logger, IServiceScopeFactory scopeFactory, IRepository<SysUser> userRepository,
    IRepository<SysRole> roleRepository, IRepository<SysRoleMember> roleMemberRepository, IRepository<SysRoleGroupMember> roleGroupMemberRepository, 
        IUserLoginService userLoginService, IUserService userService, IRepository<SysResourcePermAcl> resourceAclRepository) : base(roleRepository, scopeFactory)
    {
        _logger = logger;

        _userRepository = userRepository;
        _userLoginService = userLoginService;
        _roleRepository = roleRepository;
        _roleMemberRepository = roleMemberRepository;
        _userService = userService;
        _resourceAclRepository = resourceAclRepository;
        _roleGroupMemberRepository = roleGroupMemberRepository;
    }

    #endregion 构造和注入

    /// <summary>
    /// 从 SID 获取用户，获取不到报错
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public SysRole FromSID(string sid)
    {
        if (sid == ConstRSIDs.RoleSID.Administrators)
        {
            return new SysRole()
            {
                SID = ConstRSIDs.RoleSID.Administrators,
                RoleCode = ConstNames.RoleCode.Administrators,
                RoleName = ConstNames.RoleName.Administrators
            };
        }
        if (sid == ConstRSIDs.RoleSID.Everyone)
        {
            return new SysRole()
            {
                SID = ConstRSIDs.RoleSID.Everyone,
                RoleCode = ConstNames.RoleCode.Everyone,
                RoleName = ConstNames.RoleName.Everyone
            };
        }
        var role = (from entity in _roleRepository.Entities
                    where entity.SID == sid
                    select entity).FirstOrDefault();
        if (role == null)
            throw new Exception($"系统出现异常，角色 SID={sid} 不存在！");
        return role;
    }

    /// <summary>
    /// 从 SID 获取用户，获取不到报错
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public SysRole TryGetRoleBySID(string sid)
    {
        if (sid == ConstRSIDs.RoleSID.Administrators)
        {
            return new SysRole()
            {
                SID = ConstRSIDs.RoleSID.Administrators,
                RoleCode = ConstNames.RoleCode.Administrators,
                RoleName = ConstNames.RoleName.Administrators
            };
        }
        if (sid == ConstRSIDs.RoleSID.Everyone)
        {
            return new SysRole()
            {
                SID = ConstRSIDs.RoleSID.Everyone,
                RoleCode = ConstNames.RoleCode.Everyone,
                RoleName = ConstNames.RoleName.Everyone
            };
        }
        var role = (from entity in _roleRepository.Entities
                    where entity.SID == sid
                    select entity).FirstOrDefault();
        return role;
    }

    /// <summary>
    /// 获取角色成员
    /// </summary>
    public async Task<List<GetRoleUsersItem>> GetRoleUsersAsync(RoleBaseParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        //非系统角色进行校验
        if (SystemRoleCodes.Contains(param.RoleCode))
        {
            return new List<GetRoleUsersItem>();
        }

        if (param.RoleCode != ConstNames.RoleCode.Everyone)
        {
            //角色成员
            var roleMembers = await (from entity in _roleMemberRepository.Entities
                                     where entity.RoleCode == param.RoleCode && entity.SIDType == SIDTypeEnum.UserSID.ToString()
                                     join user in _userRepository.Entities on entity.SID equals user.SID
                                     orderby entity.OrderIndex
                                     select new GetRoleUsersItem
                                     {
                                         UserAccount = user.Account,
                                         UserDisplayName = user.Name
                                     }).ToListAsync();
            return roleMembers;
        }
        else
        {
            var users = await (from user in _userRepository.Entities 
                               select new GetRoleUsersItem
                               {
                                   UserAccount = user.Account,
                                   UserDisplayName = user.Name
                               }).ToListAsync();
            return users;
        }
    }

    /// <summary>
    /// 获取角色成员
    /// </summary>
    public List<GetRoleUsersItem> GetRoleUsers(RoleBaseParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        //非系统角色进行校验
        if (SystemRoleCodes.Contains(param.RoleCode))
        {
            return new List<GetRoleUsersItem>();
        }

        if (param.RoleCode != ConstNames.RoleCode.Everyone)
        {
            //角色成员
            var roleMembers = (from entity in _roleMemberRepository.Entities
                                     where entity.RoleCode == param.RoleCode && entity.SIDType == SIDTypeEnum.UserSID.ToString()
                                     join user in _userRepository.Entities on entity.SID equals user.SID
                                     where user.IsOpen
                                     orderby entity.OrderIndex
                                     select new GetRoleUsersItem
                                     {
                                         UserAccount = user.Account,
                                         UserDisplayName = user.Name
                                     }).ToList();
            return roleMembers;
        }
        else
        {
            var users = (from user in _userRepository.Entities
                               select new GetRoleUsersItem
                               {
                                   UserAccount = user.Account,
                                   UserDisplayName = user.Name
                               }).ToList();
            return users;
        }
    }

    /// <summary>
    /// 获取分页列表，角色用户
    /// </summary>
    public async Task<PageResult<GetRoleUsersItemByRole>> GetRoleUserPageList(PageSearchParam param)
    {
        //公开访问，无需权限
        //查询条件
        var whereExpression = ExpressionParser<GetRoleUsersItemByRole>.ParserConditions(param.filters, "user");

        //返回数据
        var list = await (from entity in _roleMemberRepository.Entities
                          join user in _userRepository.Entities on entity.SID equals user.SID
                          where user.IsOpen
                          orderby entity.OrderIndex
                          select new GetRoleUsersItemByRole
                          {
                              RoleCode = entity.RoleCode,
                              UserAccount = user.Account,
                              UserDisplayName = user.Name
                          })
            .Where(whereExpression)
            .Select(user => new GetRoleUsersItemByRole
            {
                UserAccount = user.UserAccount,
                UserDisplayName = user.UserDisplayName
            })
            .Distinct().OrderBy(param.sorts)
            .ToPagedListAsync(param.pageNo, param.pageSize);
        return PageResult<GetRoleUsersItemByRole>.Get(list: list.Items.ToList(), total: list.TotalCount);
    }

    /// <summary>
    /// 是否是指定角色成员
    /// </summary>
    /// <param name="account"></param>
    /// <param name="roleCode"></param>
    /// <returns></returns>
    public async Task<bool> IsRoleMember(string account, string roleCode)
    {
        if (string.IsNullOrEmpty(roleCode))
            throw new Exception("角色名称不能为空！");

        //当前角色SID
        if (!SystemRoleCodes.Contains(roleCode))
        {
            var role = (from entity in _roleRepository.Entities
                        where entity.RoleCode == roleCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色 “{roleCode}”！");
        }

        //角色嵌套方法存在问题，目前采用的方法是递归查询，会比较慢
        //因为暂时不会使用角色嵌套功能，暂不做修改
        return await IsRoleMemberInner(account, roleCode);
    }

    /// <summary>
    /// 是否是指定角色成员
    /// </summary>
    /// <param name="account"></param>
    /// <param name="roleSID"></param>
    /// <returns></returns>
    public async Task<bool> IsRoleSIDMember(string account, string roleSID)
    {
        if (string.IsNullOrEmpty(roleSID))
            throw new Exception("角色 SID 不能为空！");

        //当前角色名称
        string roleCode = string.Empty;
        if (!SystemRoleSIDs.Contains(roleSID))
        {
            var role = await (from entity in _roleRepository.Entities
                        where entity.SID == roleSID
                        select entity).FirstOrDefaultAsync();
            if (role == null)
                throw new Exception($"未找到角色 “{roleSID}”！");
            roleCode = role.RoleCode;
        }
        else if (roleSID == ConstRSIDs.RoleSID.Administrators) roleCode = ConstNames.RoleCode.Administrators;
        else if (roleSID == ConstRSIDs.RoleSID.Everyone) roleCode = ConstNames.RoleCode.Everyone;

        return await IsRoleMemberInner(account, roleCode);
    }

    /// <summary>
    /// 是否是管理员
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public async Task<bool> IsAdministrator(string account)
    {
        if (AdminConst.AdminUsers.Contains(account)) return true;

        return await IsRoleMemberInner(account, ConstNames.RoleCode.Administrators);
    }

    /// <summary>
    /// 获取用户所属的角色 SID 列表
    /// 仅启用的角色
    /// </summary>
    public async Task<List<string>> GetRoleSIDsAsync(string account)
    {
        var sids = new List<string>();

        //所有人角色
        //IsRoleMemberInner中已经包含验证
        //sids.Add(ConstRSIDs.RoleSID.Everyone);

        //管理员角色
        if (await IsRoleMemberInner(account, ConstNames.RoleCode.Administrators))
            sids.Add(ConstRSIDs.RoleSID.Administrators);

        //用户
        var listRoleUserMembersTask = TryAsyncDoReturnDelegate<DefaultDbContext, List<SysRoleMember>>(async (dbContext, serviceProvider) =>
        {
            return await (from entity in dbContext.RoleMembers
                          join role in dbContext.Roles on entity.RoleCode equals role.RoleCode
                          where role.IsOpen
                          where entity.SIDType == SIDTypeEnum.UserSID.ToString()
                          select entity).ToListAsync();
        });

        //角色
        var listRoleRoleMembersTask = TryAsyncDoReturnDelegate<DefaultDbContext, List<SysRoleMember>>(async (dbContext, serviceProvider) =>
        {
            return await (from entity in dbContext.RoleMembers
                          join role in dbContext.Roles on entity.RoleCode equals role.RoleCode
                          where role.IsOpen
                          where entity.SIDType == SIDTypeEnum.RoleSID.ToString()
                          select entity).ToListAsync();
        });

        var listRoleUserMembers = await listRoleUserMembersTask;
        var listRoleRoleMembers = await listRoleRoleMembersTask;

        //异步处理结果
        await TryAsyncDoDelegate<DefaultDbContext>(async (dbContext, serviceProvider) =>
        {
            var roles = (from entity in dbContext.Roles select entity).ToList();
            foreach (var role in roles)
            {
                if (await IsRoleMemberInner(dbContext.Users, account, role.RoleCode, listRoleUserMembers, listRoleRoleMembers, roles))
                    sids.Add(role.SID);
            }
        });

        return sids;
    }

    /// <summary>
    /// 是否是指定角色成员
    /// 目前支持账号、部门、角色嵌套
    /// </summary>
    private async Task<bool> IsRoleMemberInner(string account, string roleCode, int deepCount = 0)
    {
        //判断账号
        var user = await _userService.FromAccount(account);
        if (roleCode == ConstNames.RoleCode.Everyone) return true;
        if (roleCode == ConstNames.RoleCode.Administrators && account == "sa") return true;
        var roleUser = await (from entity in _roleMemberRepository.Entities
                        where entity.RoleCode == roleCode &&
                              entity.SID == user.SID &&
                              entity.SIDType == SIDTypeEnum.UserSID.ToString()
                              select entity).FirstOrDefaultAsync();
        if (roleUser != null) return true;

        //判断部门
        //获取所属部门的结构树
        //var task = OUHelper.GetOUTreeByAccount(account);
        //task.Wait();
        //var ous = task.Result;
        //var ousids = ous.Select(ou => ou.SID);
        //var roleOU = (from entity in _roleMemberRepository.Entities
        //              where entity.RoleCode == roleCode &&
        //                    ousids.Contains(entity.SID) &&
        //                    entity.SIDType == SIDTypeEnum.OUSID.ToString()
        //              select entity).FirstOrDefault();
        //if (roleOU != null) return true;

        //判断角色
        //如果角色嵌套存在循环嵌套则会出现死循环
        //为了防止出现这种情况出现，限定读取深度为 5，更深则不读取
        if (deepCount <= 5)
        {
            var roleRoles = await (from entity in _roleMemberRepository.Entities
                             where entity.RoleCode == roleCode &&
                                   entity.SIDType == SIDTypeEnum.RoleSID.ToString()
                                   select entity.SID).ToListAsync();
            var roles = (from entity in _roleRepository.Entities
                         where roleRoles.Contains(entity.SID)
                         select entity).ToList();
            if (roles.Count > 0)
            {
                deepCount++;
                foreach (var role in roles)
                {
                    if (await IsRoleMemberInner(account, role.RoleCode, deepCount)) return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 是否是指定角色成员
    /// 目前支持账号、部门、角色嵌套
    /// </summary>
    private async Task<bool> IsRoleMemberInner(DbSet<SysUser> users, string account, string roleCode,
        List<SysRoleMember> listRoleUserMembers, List<SysRoleMember> listRoleRoleMembers,
        List<SysRole> roles, int deepCount = 0) //List<SysRoleMember> listRoleOUMembers
    {
        //判断账号
        //var task = OUHelper.GetOUTreeByAccount(account);
        var user = await _userService.FromAccount(users, account);
        if (roleCode == ConstNames.RoleCode.Everyone) return true;
        if (roleCode == ConstNames.RoleCode.Administrators && account == "sa") return true;
        var roleUser = (from entity in listRoleUserMembers
                        where entity.RoleCode == roleCode &&
                              entity.SID == user.SID
                        select entity).FirstOrDefault();
        if (roleUser != null) return true;

        //判断角色
        //如果角色嵌套存在循环嵌套则会出现死循环
        //为了防止出现这种情况出现，限定读取深度为 5，更深则不读取
        if (deepCount <= 5)
        {
            var roleRoles = (from entity in listRoleRoleMembers
                             where entity.RoleCode == roleCode
                             select entity.SID).ToList();
            var roleInners = (from entity in roles
                              where roleRoles.Contains(entity.SID)
                              select entity).ToList();
            if (roleInners.Count > 0)
            {
                deepCount++;
                foreach (var role in roleInners)
                {
                    if (await IsRoleMemberInner(users, account, role.RoleCode, listRoleUserMembers, listRoleRoleMembers, roles, deepCount))
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 获取用户所属角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<List<BelongRolesResult>> GetBelongRoles(RoleCheckParam param)
    {
        return await GetBelongRoles(param.Account);
    }

    /// <summary>
    /// 获取用户所属角色
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public async Task<List<BelongRolesResult>> GetBelongRoles(string account)
    {
        var user = await _userService.FromAccount(account);
        var roles = await (from entity in _roleRepository.Entities
                           join roleMember in _roleMemberRepository.Entities on entity.RoleCode equals roleMember.RoleCode
                           where roleMember.SID == user.SID
                           orderby entity.OrderIndex
                           select entity).ToListAsync();

        var newdata = new List<BelongRolesResult>();
        if (roles != null)
        {
            foreach (var item in roles)
            {
                newdata.Add(new BelongRolesResult
                {
                    RoleCode = item.RoleCode,
                    RoleName = item.RoleName,
                    SID = item.SID
                });
            }
        }
        return newdata;
    }

    /// <summary>
    /// 是否是角色成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<IsBelongRoleResult> IsRoleMember(RoleCheckIsMemberParam param)
    {
        return new IsBelongRoleResult
        {
            IsBelong = await IsRoleMember(param.Account, param.RoleCode)
        };
    }

    /// <summary>
    /// 是否是管理员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<IsBelongRoleResult> IsAdministrator(RoleCheckParam param)
    {
        return new IsBelongRoleResult
        {
            IsBelong = await IsAdministrator(param.Account)
        };
    }

    /// <summary>
    /// 是否是管理员登录
    /// </summary>
    public async Task<IsBelongRoleResult> IsAdministratorLogin()
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        return new IsBelongRoleResult
        {
            IsBelong = await IsAdministrator(loginUser.UserAccount)
        };
    }
}
