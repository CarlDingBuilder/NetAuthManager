using NetAuthManager.Application.Base.Consts;
using NetAuthManager.Application.Sys.Params.RoleGroups;
using NetAuthManager.Application.Sys.Results.RoleGroups;
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
using static NetAuthManager.Application.Consts.ConstNames;
using static NetAuthManager.Application.Consts.ConstRSIDs;

namespace NetAuthManager.Application;

/// <summary>
/// 角色组服务
/// </summary>
public class RoleGroupManageService : BaseService<SysRoleGroup>, IRoleGroupManageService, ITransient
{
    #region 构造和注入

    private readonly ILogger<RoleGroupManageService> _logger;

    private readonly IUserLoginService _userLoginService;
    private readonly IRoleService _roleService;
    private readonly IMenuResourceService _menuResourceService;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysRoleGroup> _roleGroupRepository;
    private readonly IRepository<SysRoleGroupMember> _roleGroupMemberRepository;
    private readonly IRepository<SysServiceEntity> _serviceEntityRepository;
    private readonly IRepository<SysResourcePermAcl> _resourceAclRepository;
    private readonly IdentityService _identityService;
    private readonly IRoleGroupService _roleGroupService;

    public RoleGroupManageService(ILogger<RoleGroupManageService> logger, IServiceScopeFactory scopeFactory,
        IMenuResourceService menuResourceService, IRepository<SysRole> roleRepository,
        IRepository<SysRoleGroup> roleGroupRepository, IRepository<SysRoleGroupMember> roleGroupMemberRepository,
        IUserLoginService userLoginService, IRoleService roleService, IRepository<SysResourcePermAcl> resourceAclRepository, 
        IRepository<SysServiceEntity> serviceEntityRepository, IdentityService identityService, IRoleGroupService roleGroupService) : base(roleGroupRepository, scopeFactory)
    {
        _logger = logger;

        _userLoginService = userLoginService;
        _roleRepository = roleRepository;
        _roleGroupRepository = roleGroupRepository;
        _roleGroupMemberRepository = roleGroupMemberRepository;
        _roleService = roleService;
        _resourceAclRepository = resourceAclRepository;
        _serviceEntityRepository = serviceEntityRepository;
        _identityService = identityService;
        _menuResourceService = menuResourceService;
        _roleGroupService = roleGroupService;
    }

    #endregion 构造和注入

    /// <summary>
    /// 获取分页列表，仅启用用户
    /// </summary>
    public async Task<PageResult<SysRoleGroup>> GetRoleGroupPageList(PageSearchParam param)
    {
        //公开访问，无需权限

        //查询条件
        var whereExpression = ExpressionParser<SysRoleGroup>.ParserConditions(param.filters, "roleGroup");

        //返回数据
        var list = await (from roleGroup in _roleGroupRepository.Entities
                          join serviceEntity in _serviceEntityRepository.Entities on roleGroup.ServiceEntityCode equals serviceEntity.EntityCode
                          select new SysRoleGroup
                          {
                              GroupCode = roleGroup.GroupCode,
                              GroupName = roleGroup.GroupName,
                              ServiceEntityCode = roleGroup.ServiceEntityCode,
                              ServiceEntityName = serviceEntity.EntityName,
                              SID = roleGroup.SID,
                              OrderIndex = roleGroup.OrderIndex,
                              IsOpen = roleGroup.IsOpen,
                              Description = roleGroup.Description,
                          }).Where(x => x.IsOpen)
                            //.Where(permExpression)
                            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        var endList = list.Items.ToList();
        return PageResult<SysRoleGroup>.Get(list: endList, total: list.TotalCount);
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    public async Task<PageResult<SysRoleGroup>> GetAllRoleGroupPageList(PageSearchParam param)
    {
        //公开访问，无需权限
        //获取权限
        var perms = await _menuResourceService.GetResourceBizAllowPermNames();

        //权限构建
        var permExpression = ExpressionParserEx<SysRoleGroup>.ParserConditionsByPerms(perms, "roleGroup");

        //查询条件
        var whereExpression = ExpressionParser<SysRoleGroup>.ParserConditions(param.filters, "roleGroup");

        //返回数据
        var list = await (from roleGroup in _roleGroupRepository.Entities
                          join serviceEntity in _serviceEntityRepository.Entities on roleGroup.ServiceEntityCode equals serviceEntity.EntityCode into serviceEntityGroup
                          from serviceEntity in serviceEntityGroup.DefaultIfEmpty()
                          select new SysRoleGroup
                          {
                              GroupCode = roleGroup.GroupCode, 
                              GroupName = roleGroup.GroupName, 
                              ServiceEntityCode = roleGroup.ServiceEntityCode, 
                              ServiceEntityName = serviceEntity != null ? serviceEntity.EntityName : null, 
                              SID = roleGroup.SID, 
                              OrderIndex = roleGroup.OrderIndex, 
                              IsOpen = roleGroup.IsOpen, 
                              Description = roleGroup.Description, 
                              CreatorAccount = roleGroup.CreatorAccount, 
                              CreatorName = roleGroup.CreatorName, 
                              CreatedTime = roleGroup.CreatedTime,
                          })
                            .Where(permExpression)
                            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        var endList = list.Items.ToList();
        return PageResult<SysRoleGroup>.Get(list: endList, total: list.TotalCount);
    }

    /// <summary>
    /// 添加角色组
    /// </summary>
    public async Task<SysRoleGroup> AddRoleGroup(RoleGroupAddParam param)
    {
        if (string.IsNullOrEmpty(param.GroupName))
            throw new Exception("角色组名称不能为空！");

        var roles = await  (from entity in _roleGroupRepository.Entities
                     where entity.GroupCode == param.GroupCode || entity.GroupName == param.GroupName
                            select entity).ToListAsync();

        if (roles.Any(r => r.GroupName == param.GroupName))
            throw new Exception($"角色组 “{param.GroupName}” 已存在，不能重复新增！");

        //角色组编码生成
        param.GroupCode = _identityService.GetIdentityNo(ConstIdentityPrefixes.RoleGroup, 10);

        //角色组排序
        var maxOrderIndex = await MaxAsync(x => true, x => x.OrderIndex);
        if (maxOrderIndex <= 0) maxOrderIndex = 0;
        maxOrderIndex = maxOrderIndex + 1;

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //角色组信息
        var roleModel = new SysRoleGroup()
        {
            GroupCode = param.GroupCode,
            GroupName = param.GroupName,
            ServiceEntityCode = param.ServiceEntityCode,
            OrderIndex = maxOrderIndex,
            IsOpen = param.IsOpen,
            Description = param.Description,
            CreatorAccount = loginUser.UserAccount,
            CreatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName,
            CreatedTime = DateTime.Now,
        };

        //事务执行
        await _roleGroupRepository.InsertNowAsync(roleModel);

        //重新查询
        return await _roleGroupRepository.Where(x => x.GroupCode == param.GroupCode).FirstAsync();
    }

    /// <summary>
    /// 重命名角色组
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task ModifyRoleGroup(RoleGroupModifyParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组编码不能为空！");

        if (string.IsNullOrEmpty(param.GroupName))
            throw new Exception("角色组名称不能为空！");

        //角色组信息
        var role = (from entity in _roleGroupRepository.Entities
                    where entity.GroupCode == param.GroupCode
                    select entity).FirstOrDefault();
        if (role == null)
            throw new Exception($"未找到角色组 “{param.GroupName}（{param.GroupCode}）”！");

        var tempRole = (from entity in _roleGroupRepository.Entities
                        where entity.GroupName == param.GroupName && entity.SID != role.SID
                        select entity).FirstOrDefault();
        if (tempRole != null)
            throw new Exception($"角色组 “{param.GroupName}” 已存在！");

        ////角色组成员
        //var roleMembers = (from entity in _roleMemberRepository.Entities
        //                   where entity.RoleCode == param.RoleCode
        //                   select entity).ToList();

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        role.GroupName = param.GroupName;
        role.ServiceEntityCode = param.ServiceEntityCode;
        role.OrderIndex = param.OrderIndex;
        role.IsOpen = param.IsOpen;
        role.Description = param.Description;
        role.UpdatorAccount = loginUser.UserAccount;
        role.UpdatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName;
        role.UpdatedTime = DateTime.Now;
        await _roleGroupRepository.UpdateNowAsync(role);
    }

    /// <summary>
    /// 设置是否启用
    /// </summary>
    public async Task SetIsOpen(SetRoleGroupIsOpenParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组编码不能为空！");

        //角色组信息
        var role = (from entity in _roleGroupRepository.Entities
                    where entity.GroupCode == param.GroupCode
                    select entity).FirstOrDefault();
        if (role == null)
            throw new Exception($"未找到角色组 “{param.GroupName}（{param.GroupCode}）”！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        role.IsOpen = param.IsOpen;
        role.UpdatorAccount = loginUser.UserAccount;
        role.UpdatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName;
        role.UpdatedTime = DateTime.Now;
        await _roleGroupRepository.UpdateNowAsync(role);
    }

    /// <summary>
    /// 获取所有角色组
    /// </summary>
    public async Task<List<SysRoleGroup>> GetRoleGroups()
    {
        //获取当前登录用户
        var entitis = await (from entity in _roleGroupRepository.Entities
                             select entity).OrderBy(x => x.OrderIndex).ToListAsync();

        return entitis;
    }

    /// <summary>
    /// 获取所有角色组
    /// </summary>
    public async Task<List<SysRoleGroup>> GetOwerRoleGroupsAsync()
    {
        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        return await _roleGroupService.GetOwerRoleGroupsByUserAsync(loginUser.UserAccount);
    }

    /// <summary>
    /// 删除角色组
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRoleGroup(RoleGroupBaseParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组编码不能为空！");

        var role = (from entity in _roleGroupRepository.Entities
                    where entity.GroupCode == param.GroupCode
                    select entity).FirstOrDefault();
        if (role != null)
        {
            ////删除的角色组成员
            //var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
            //                         where entity.GroupCode == param.GroupCode
            //                         select entity).ToListAsync();

            ////删除角色组对应的授权
            //var acls = await (from entity in _resourceAclRepository.Entities
            //                  where entity.RoleType == SIDTypeEnum.RoleGroupSID.ToString() && entity.RoleParam1 == role.SID
            //                  select entity).ToListAsync();

            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除角色
                await _roleGroupRepository.DeleteAsync(role);
                //if (roleMembers.Count > 0)
                //    await _roleGroupMemberRepository.DeleteAsync(roleMembers);
                //if (acls.Count > 0)
                //    await _resourceAclRepository.DeleteAsync(acls);


                //删除的角色组成员
                await _roleGroupMemberRepository.Where(entity => entity.GroupCode == param.GroupCode).ExecuteDeleteAsync();

                //删除角色组对应的授权
                await _resourceAclRepository.Where(entity => entity.RoleType == SIDTypeEnum.RoleGroupSID.ToString() && entity.RoleParam1 == role.SID).ExecuteDeleteAsync();
            });
        }
    }

    /// <summary>
    /// 删除多个角色组
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRoleGroups(RoleGroupDeletesParam param)
    {
        if (param.GroupCodes == null || param.GroupCodes.Count == 0)
            throw new Exception("角色组编码不能为空！");

        var roleGroups = (from entity in _roleGroupRepository.Entities
                     where param.GroupCodes.Contains(entity.GroupCode)
                     select entity).ToList();
        if (roleGroups != null && roleGroups.Count > 0)
        {
            ////删除的角色组成员
            //var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
            //                         where param.GroupCodes.Contains(entity.GroupCode)
            //                         select entity).ToListAsync();

            //删除角色组对应的授权
            var roleSids = roleGroups.Select(r => r.SID).ToList();
            //var acls = await (from entity in _resourceAclRepository.Entities
            //                  where entity.RoleType == SIDType.RoleSID.ToString() && roleSids.Contains(entity.RoleParam1)
            //                  select entity).ToListAsync();

            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除角色组
                await _roleGroupRepository.DeleteAsync(roleGroups);

                //if (roleMembers.Count > 0)
                //    await _roleGroupMemberRepository.DeleteAsync(roleMembers);
                //if (acls.Count > 0)
                //    await _resourceAclRepository.DeleteAsync(acls);

                //删除的角色组成员
                await _roleGroupMemberRepository.Where(entity => param.GroupCodes.Contains(entity.GroupCode)).ExecuteDeleteAsync();

                //删除角色组对应的授权
                await _resourceAclRepository.Where(entity => entity.RoleType == SIDTypeEnum.RoleGroupSID.ToString() && roleSids.Contains(entity.RoleParam1)).ExecuteDeleteAsync();
            });
        }
    }

    /// <summary>
    /// 获取角色组成员
    /// </summary>
    public async Task<List<RoleGroupMemberResult>> GetRoleGroupMembers(RoleGroupBaseParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组编码不能为空！");

        //非系统角色组进行校验
        var role = (from entity in _roleGroupRepository.Entities
                    where entity.GroupCode == param.GroupCode
                    select entity).FirstOrDefault();
        if (role == null)
            throw new Exception($"未找到角色组 “{param.GroupCode}”！");

        var members = new List<RoleGroupMemberResult>();
        if (param.GroupCode != ConstNames.RoleCode.Everyone)
        {
            //角色组成员
            var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
                               where entity.GroupCode == param.GroupCode
                                     select entity).ToListAsync();
            foreach (var member in roleMembers)
            {
                members.Add(new RoleGroupMemberResult
                {
                    DisplayName = GetMemberDisplayName(member),
                    SID = member.SID,
                    SIDType = member.SIDType.ToString()
                });
            }
        }

        return members;
    }

    /// <summary>
    /// 获取角色组-角色成员
    /// </summary>
    public async Task<List<RoleGroupRoleItem>> GetRoleGroupRoles(RoleGroupBaseParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组编码不能为空！");

        //非系统角色组进行校验
        var roleGroup = (from entity in _roleGroupRepository.Entities
                    where entity.GroupCode == param.GroupCode
                    select entity).FirstOrDefault();
        if (roleGroup == null)
            throw new Exception($"未找到角色组 “{param.GroupCode}”！");

        var roles = new List<RoleGroupRoleItem>();
        if (param.GroupCode != ConstNames.RoleCode.Everyone)
        {
            //角色组成员
            var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
                                     where entity.GroupCode == param.GroupCode
                                     join role in _roleRepository.Entities on entity.SID equals role.SID
                                     select role).ToListAsync();
            foreach (var member in roleMembers)
            {
                roles.Add(new RoleGroupRoleItem
                {
                    RoleCode = member.RoleCode,
                    RoleName = member.RoleName,
                });
            }
        }

        return roles;
    }

    /// <summary>
    /// 添加一个角色组成员
    /// </summary>
    public async Task AddRoleGroupMember(RoleGroupMemberAddParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组名称不能为空！");

        if (param.GroupCode == ConstNames.RoleCode.Everyone)
            throw new Exception("Everyone 角色组不能添加成员！");

        if (param.Member == null)
            throw new Exception("请提供需要添加的角色组成员信息！");

        if (param.GroupCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleGroupRepository.Entities
                        where entity.GroupCode == param.GroupCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色组 “{param.GroupCode}”！");
        }

        //添加角色组成员
        var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
                           where entity.GroupCode == param.GroupCode
                                 select entity).ToListAsync();
        if (!roleMembers.Any(m => m.SID == param.Member.SID && m.SIDType == param.Member.SIDType))
        {
            var member = new SysRoleGroupMember
            {
                GroupCode = param.GroupCode,
                SID = param.Member.SID
            };
            await _roleGroupMemberRepository.InsertNowAsync(member);
        }
    }

    /// <summary>
    /// 添加多个角色组成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task AddRoleGroupMembers(RoleGroupMembersAddParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组名称不能为空！");

        if (param.GroupCode == ConstNames.RoleCode.Everyone)
            throw new Exception("Everyone 角色组不能添加成员！");

        if (param.Members == null || param.Members.Count == 0)
            throw new Exception("请提供需要添加的角色组成员信息！");

        if (param.GroupCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleGroupRepository.Entities
                        where entity.GroupCode == param.GroupCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色组 “{param.GroupCode}”！");
        }

        //添加角色组成员
        var roleMembers = (from entity in _roleGroupMemberRepository.Entities
                           where entity.GroupCode == param.GroupCode
                           select entity).ToList();

        var addMembers = new List<SysRoleGroupMember>();
        foreach (var member in param.Members)
        {
            if (!roleMembers.Any(m => m.SID == member.SID && m.SIDType == member.SIDType))
            {
                addMembers.Add(new SysRoleGroupMember
                {
                    GroupCode = param.GroupCode,
                    SID = member.SID
                });
            }
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            await _roleGroupMemberRepository.InsertAsync(addMembers);
        });
    }

    /// <summary>
    /// 删除角色组成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRoleGroupMember(RoleGroupMemberDeleteParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组名称不能为空！");

        if (param.Member == null || string.IsNullOrEmpty(param.Member.SID))
            throw new Exception("请提供需要添加的角色组成员信息！");

        if (param.GroupCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleGroupRepository.Entities
                        where entity.GroupCode == param.GroupCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色组 “{param.GroupCode}”！");
        }

        //添加角色组成员
        var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
                           where entity.GroupCode == param.GroupCode
                                 select entity).ToListAsync();

        var oldEntity = roleMembers.FirstOrDefault(m => m.SID == param.Member.SID && m.SIDType == param.Member.SIDType);
        if (oldEntity != null)
        {
            await _roleGroupMemberRepository.DeleteNowAsync(oldEntity);
        }
    }

    /// <summary>
    /// 删除多个角色组成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRoleGroupMembers(RoleGroupMemberDeletesParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组组编码不能为空！");

        if (param.Members == null || param.Members.Count == 0)
            throw new Exception("请提供需要添加的角色组成员信息！");

        if (param.GroupCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleGroupRepository.Entities
                        where entity.GroupCode == param.GroupCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色组 “{param.GroupCode}”！");
        }

        //角色组成员
        var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
                           where entity.GroupCode == param.GroupCode
                                 select entity).ToListAsync();

        var deleteMembers = new List<SysRoleGroupMember>();
        foreach (var member in param.Members)
        {
            if (string.IsNullOrEmpty(member.SID))
                throw new Exception("请提供需要添加的角色组成员信息！");

            var oldEntity = roleMembers.FirstOrDefault(m => m.SID == member.SID && m.SIDType == member.SIDType);
            if (oldEntity != null)
            {
                deleteMembers.Add(oldEntity);
            }
        }
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            await _roleGroupMemberRepository.DeleteAsync(deleteMembers);
        });
    }

    /// <summary>
    /// 保存角色组成员
    /// </summary>
    public async Task SaveRoleGroupMembers(SaveRoleGroupMembersParam param)
    {
        if (string.IsNullOrEmpty(param.GroupCode))
            throw new Exception("角色组编码不能为空！");

        //现有的角色组成员
        var roleMembers = await (from entity in _roleGroupMemberRepository.Entities
                                 where entity.GroupCode == param.GroupCode
                                 select entity).ToListAsync();

        //构建
        var insertMembers = new List<SysRoleGroupMember>();
        var updateMembers = new List<SysRoleGroupMember>();
        var deleteMembers = new List<SysRoleGroupMember>();
        for (var index = 0; index < param.Members.Count; index++)
        {
            var member = param.Members[index];
            var oldMember = roleMembers.FirstOrDefault(m => m.SID == member.SID && m.SIDType.Equals(member.SIDType));
            if (oldMember == null)
            {
                insertMembers.Add(new SysRoleGroupMember
                {
                    GroupCode = param.GroupCode,
                    SID = member.SID,
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
            var newMember = param.Members.FirstOrDefault(m => m.SID == member.SID && m.SIDType.Equals(member.SIDType.ToString()));
            if (newMember == null)
            {
                deleteMembers.Add(member);
            }
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            if (insertMembers.Count > 0) await _roleGroupMemberRepository.InsertAsync(insertMembers);
            if (updateMembers.Count > 0) await _roleGroupMemberRepository.UpdateAsync(updateMembers);
            if (deleteMembers.Count > 0) await _roleGroupMemberRepository.DeleteAsync(deleteMembers);
        });
    }

    /// <summary>
    /// 获取成员显示名
    /// </summary>
    private string GetMemberDisplayName(SysRoleGroupMember member)
    {
        var sidType = GetSIDType(member.SIDType);
        switch (sidType)
        {
            case SIDTypeEnum.RoleSID:
                var role = _roleService.FromSID(member.SID);
                return role.RoleName;
            //case SIDType.OUSID:
            //    var ou = OUHelper.FromSID(member.SID);
            //    return ou.OUFullName;
            default:
                return $"(不支持的类型：{member.SIDType})";
        }
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    private SIDTypeEnum GetSIDType(string sidType)
    {
        SIDTypeEnum type;
        if (!System.Enum.TryParse(sidType, out type))
            throw new Exception($"无效的成员类型 “{sidType}”！");
        return type;
    }
}
