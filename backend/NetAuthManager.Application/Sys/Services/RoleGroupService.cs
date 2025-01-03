using NetAuthManager.Application.Sys.Params.RoleGroups;
using NetAuthManager.Application.Sys.Results.RoleGroups;
using NetAuthManager.Application.Sys.Results.Roles;
using NetAuthManager.Core.Common.Enums;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Expressions;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.Core.Services;
using NetAuthManager.EntityFramework.Core;
using Furion.DatabaseAccessor;
using Furion.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 角色组服务
/// </summary>
public class RoleGroupService : BaseService<SysRoleGroup>, IRoleGroupService, ITransient
{
    #region 构造和注入

    private readonly ILogger<RoleGroupService> _logger;

    private readonly IUserLoginService _userLoginService;
    private readonly IRepository<SysUser> _userRepository;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysRoleMember> _roleMemberRepository;
    private readonly IRepository<SysRoleGroup> _roleGroupRepository;
    private readonly IRepository<SysRoleGroupMember> _roleGroupMemberRepository;
    private readonly IRoleService _roleService;

    public RoleGroupService(ILogger<RoleGroupService> logger, IServiceScopeFactory scopeFactory,
        IRepository<SysRoleGroup> roleGroupRepository, IRepository<SysRoleGroupMember> roleGroupMemberRepository,
        IUserLoginService userLoginService, IRoleService roleService, IRepository<SysUser> userRepository, IRepository<SysRoleMember> roleMemberRepository,
        IRepository<SysRole> roleRepository) : base(roleGroupRepository, scopeFactory)
    {
        _logger = logger;

        _userLoginService = userLoginService;
        _roleGroupRepository = roleGroupRepository;
        _roleGroupMemberRepository = roleGroupMemberRepository;
        _roleService = roleService;
        _userRepository = userRepository;
        _roleMemberRepository = roleMemberRepository;
        _roleRepository = roleRepository;
    }

    #endregion 构造和注入

    /// <summary>
    /// 获取角色所属的角色组 SID 列表
    /// 仅考虑开启的角色
    /// </summary>
    public async Task<List<string>> GetRoleGroupSIDsAsync(List<string> roleSIDs)
    {
        return await GetRoleGroupSIDsAsync(_roleGroupRepository.Entities, _roleGroupMemberRepository.Entities, roleSIDs);
    }

    /// <summary>
    /// 获取角色所属的角色组 SID 列表
    /// 仅考虑开启的角色
    /// </summary>
    public async Task<List<string>> GetRoleGroupSIDsAsync(DefaultDbContext dbContext, List<string> roleSIDs)
    {
        return await GetRoleGroupSIDsAsync(dbContext.RoleGroups, dbContext.RoleGroupMembers, roleSIDs);
    }

    /// <summary>
    /// 获取角色所属的角色组 SID 列表
    /// 仅考虑开启的角色
    /// </summary>
    private async Task<List<string>> GetRoleGroupSIDsAsync(DbSet<SysRoleGroup> roleGroups, DbSet<SysRoleGroupMember> roleGroupMembers, List<string> roleSIDs)
    {
        //返回数据
        var list = await (from roleGroup in roleGroups
                          join roleGroupMember in roleGroupMembers on roleGroup.GroupCode equals roleGroupMember.GroupCode
                          where roleSIDs.Contains(roleGroupMember.SID)
                          select roleGroup.SID).ToListAsync();
        return list;
    }

    /// <summary>
    /// 从 SID 获取用户，获取不到报错
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public SysRoleGroup FromSID(string sid)
    {
        var role = (from entity in _roleGroupRepository.Entities
                    where entity.SID == sid
                    select entity).FirstOrDefault();
        if (role == null)
            throw new Exception($"系统出现异常，角色组 SID={sid} 不存在！");
        return role;
    }

    /// <summary>
    /// 从 SID 获取用户，获取不到报错
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public SysRoleGroup TryGetRoleGroupBySID(string sid)
    {
        var role = (from entity in _roleGroupRepository.Entities
                    where entity.SID == sid
                    select entity).FirstOrDefault();
        return role;
    }

    /// <summary>
    /// 是否是指定角色组成员
    /// </summary>
    /// <param name="roleCode"></param>
    /// <param name="groupCode"></param>
    /// <returns></returns>
    public async Task<bool> IsRoleGroupMember(string roleCode, string groupCode)
    {
        if (string.IsNullOrEmpty(groupCode))
            throw new Exception("角色组编码不能为空！");

        //当前角色组SID
        var group = (from entity in _roleGroupRepository.Entities
                    where entity.GroupCode == groupCode
                    select entity).FirstOrDefault();
        if (group == null)
            throw new Exception($"未找到角色组 “{groupCode}”！");

        //角色组嵌套方法存在问题，目前采用的方法是递归查询，会比较慢
        //因为暂时不会使用角色组嵌套功能，暂不做修改
        return await IsRoleMemberInner(roleCode, groupCode);
    }

    /// <summary>
    /// 是否是指定角色组成员
    /// 是否是组成员，只要是其中一个组成员就行了
    /// </summary>
    /// <param name="account"></param>
    /// <param name="groupCodes"></param>
    /// <returns></returns>
    public bool IsOneRoleGroupMemberUser(string account, List<string> groupCodes)
    {
        if (groupCodes.Count == 0)
            throw new Exception("角色组编码不能为空！");

        //当前角色组SID
        var group = (from entity in _roleGroupRepository.Entities
                     join roleGroupMember in _roleGroupMemberRepository.Entities on entity.GroupCode equals roleGroupMember.GroupCode
                     join role in _roleRepository.Entities on roleGroupMember.SID equals role.SID
                     join roleMember in _roleMemberRepository.Entities on role.RoleCode equals roleMember.RoleCode
                     join user in _userRepository.Entities on roleMember.SID equals user.SID
                     where groupCodes.Contains(entity.GroupCode) && user.Account == account
                     select entity).FirstOrDefault();
        if (group != null) return true;

        //角色组嵌套方法存在问题，目前采用的方法是递归查询，会比较慢
        //因为暂时不会使用角色组嵌套功能，暂不做修改
        return false;
    }

    /// <summary>
    /// 是否是指定角色组成员
    /// </summary>
    /// <param name="roleCode"></param>
    /// <param name="groupSID"></param>
    /// <returns></returns>
    public async Task<bool> IsRoleGroupSIDMember(string roleCode, string groupSID)
    {
        if (string.IsNullOrEmpty(groupSID))
            throw new Exception("角色组 SID 不能为空！");

        //当前角色组组编码
        var group = await (from entity in _roleGroupRepository.Entities
                          where entity.SID == groupSID
                          select entity).FirstOrDefaultAsync();
        if (group == null)
            throw new Exception($"未找到角色组 “{groupSID}”！");

        return await IsRoleMemberInner(roleCode, group.GroupCode);
    }

    /// <summary>
    /// 是否是指定角色组成员
    /// 目前支持账号、部门、角色组嵌套
    /// </summary>
    private async Task<bool> IsRoleMemberInner(string roleCode, string groupCode)
    {
        var role = await (from  entity in _roleRepository.Entities
                          where entity.RoleCode == roleCode
                          select entity).FirstOrDefaultAsync();
        if (role == null)
            throw new Exception($"角色 {roleCode} 不存在！");

        //判断账号
        var roleUser = await (from entity in _roleGroupMemberRepository.Entities
                        where entity.GroupCode == groupCode &&
                              entity.SID == role.SID
                        select entity).FirstOrDefaultAsync();
        if (roleUser != null) return true;

        return false;
    }

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<List<BelongRoleGroupsResult>> GetBelongRoleGroups(RoleGroupCheckParam param)
    {
        return await GetBelongRoleGroups(param.RoleCode);
    }

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    /// <param name="roleCode"></param>
    /// <returns></returns>
    public async Task<List<BelongRoleGroupsResult>> GetBelongRoleGroups(string roleCode)
    {
        var roleGroups = await (from entity in _roleGroupRepository.Entities
                           join roleMember in _roleGroupMemberRepository.Entities on entity.GroupCode equals roleMember.GroupCode
                           join role in _roleRepository.Entities on roleMember.SID equals role.SID    
                           where role.RoleCode == roleCode
                           select entity).ToListAsync();

        var newdata = new List<BelongRoleGroupsResult>();
        if (roleGroups != null)
        {
            foreach (var item in roleGroups)
            {
                newdata.Add(new BelongRoleGroupsResult
                {
                    GroupCode = item.GroupCode,
                    GroupName = item.GroupName,
                    SID = item.SID
                });
            }
        }
        return newdata;
    }

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public List<BelongRoleGroupsResult> GetBelongRoleGroupsByUser(string account)
    {
        var roleGroups = GetOwerRoleGroupsByUserInner(account).ToList();

        var newdata = new List<BelongRoleGroupsResult>();
        if (roleGroups != null)
        {
            foreach (var item in roleGroups)
            {
                newdata.Add(new BelongRoleGroupsResult
                {
                    GroupCode = item.GroupCode,
                    GroupName = item.GroupName,
                    SID = item.SID
                });
            }
        }
        return newdata;
    }

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public List<SysRoleGroup> GetOwerRoleGroupsByUser(string account)
    {
        return GetOwerRoleGroupsByUserInner(account).ToList();
    }

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public async Task<List<SysRoleGroup>> GetOwerRoleGroupsByUserAsync(string account)
    {
        return await GetOwerRoleGroupsByUserInner(account).ToListAsync();
    }

    /// <summary>
    /// 获取用户所属角色组
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public IQueryable<SysRoleGroup> GetOwerRoleGroupsByUserInner(string account)
    {
        var queryTemp = (from entity in _roleGroupRepository.Entities
                    join roleGroupMember in _roleGroupMemberRepository.Entities on entity.GroupCode equals roleGroupMember.GroupCode
                    join role in _roleRepository.Entities on roleGroupMember.SID equals role.SID
                    join roleMember in _roleMemberRepository.Entities on role.RoleCode equals roleMember.RoleCode
                    join user in _userRepository.Entities on roleMember.SID equals user.SID
                    where user.Account == account
                    select entity.SID);
        var query = (from entity in _roleGroupRepository.Entities
                     where queryTemp.Contains(entity.SID)
                     select entity);
        return query;
    }


    /// <summary>
    /// 获取用户所属角色组下属角色
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public async Task<List<BelongRolesResult>> GetBelongRoleGroupRolesByUserAsync(string account)
    {
        var roles = await GetBelongRoleGroupRolesByUserInner(account).ToListAsync();
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
    /// 获取用户所属角色组下属角色
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public List<BelongRolesResult> GetBelongRoleGroupRolesByUser(string account)
    {
        var roles = GetBelongRoleGroupRolesByUserInner(account).ToList();
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
    private IQueryable<SysRole> GetBelongRoleGroupRolesByUserInner(string account)
    {
        var groupCodes = (from entity in _roleGroupRepository.Entities
                          join roleGroupMember in _roleGroupMemberRepository.Entities on entity.GroupCode equals roleGroupMember.GroupCode
                          join role in _roleRepository.Entities on roleGroupMember.SID equals role.SID
                          join roleMember in _roleMemberRepository.Entities on role.RoleCode equals roleMember.RoleCode
                          join user in _userRepository.Entities on roleMember.SID equals user.SID
                          where user.Account == account
                          select entity.GroupCode).Distinct();
        var roles = (from role in _roleRepository.Entities
                          join roleGroupMember in _roleGroupMemberRepository.Entities on role.SID equals roleGroupMember.SID
                          where groupCodes.Contains(roleGroupMember.GroupCode)
                          select role).Distinct();

        return roles;
    }

    /// <summary>
    /// 是否是角色组成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<IsBelongRoleGroupResult> IsRoleGroupMember(RoleGroupCheckIsMemberParam param)
    {
        return new IsBelongRoleGroupResult
        {
            IsBelong = await IsRoleGroupMember(param.GroupCode, param.RoleCode)
        };
    }
}
