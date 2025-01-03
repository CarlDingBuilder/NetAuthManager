using NetAuthManager.Application.Sys.Params.Resources;
using NetAuthManager.Application.Sys.Params.Roles;
using NetAuthManager.Application.Sys.Results.Roles;
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
public class RoleManageService : BaseService<SysRole>, IRoleManageService, ITransient
{
    #region 构造和注入

    /// <summary>
    /// 系统角色编码列表
    /// </summary>
    private string[] SystemRoleCodes = new string[2] { ConstNames.RoleCode.Administrators, ConstNames.RoleCode.Everyone };

    /// <summary>
    /// 系统角色名称列表
    /// </summary>
    private string[] SystemRoleNames = new string[2] { ConstNames.RoleName.Administrators, ConstNames.RoleName.Everyone };

    private readonly ILogger<RoleManageService> _logger;

    private readonly IUserLoginService _userLoginService;
    private readonly IUserService _userService;
    private readonly IMenuResourceService _menuResourceService;
    private readonly IRoleService _roleService;
    private readonly IRepository<SysRole> _roleRepository;
    private readonly IRepository<SysRoleMember> _roleMemberRepository;
    private readonly IRepository<SysRoleGroupMember> _roleGroupMemberRepository;
    private readonly IRepository<SysResourcePermAcl> _resourceAclRepository;

    public RoleManageService(ILogger<RoleManageService> logger, IServiceScopeFactory scopeFactory, IRoleService roleService,
        IRepository<SysRole> roleRepository, IRepository<SysRoleMember> roleMemberRepository, IRepository<SysRoleGroupMember> roleGroupMemberRepository,
        IUserLoginService userLoginService, IUserService userService, IRepository<SysResourcePermAcl> resourceAclRepository, IMenuResourceService menuResourceService) : base(roleRepository, scopeFactory)
    {
        _logger = logger;

        _userLoginService = userLoginService;
        _roleRepository = roleRepository;
        _roleMemberRepository = roleMemberRepository;
        _userService = userService;
        _resourceAclRepository = resourceAclRepository;
        _roleGroupMemberRepository = roleGroupMemberRepository;
        _menuResourceService = menuResourceService;
        _roleService = roleService;
    }

    #endregion 构造和注入

    /// <summary>
    /// 获取分页列表，仅启用用户
    /// </summary>
    public async Task<PageResult<SysRole>> GetRolePageList(PageSearchParam param)
    {
        //公开访问，无需权限
        //查询条件
        var whereExpression = ExpressionParser<SysRole>.ParserConditions(param.filters, "user");

        //返回数据
        var list = await _entityRepository.Where(x => x.IsOpen)
            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        var endList = list.Items.ToList();
        foreach(var role in endList)
        {
            role.IsSystem = SystemRoleCodes.Contains(role.RoleCode);
            role.IsAdmin = ConstNames.RoleCode.Administrators == role.RoleCode;
            role.IsEveryone = ConstNames.RoleCode.Everyone == role.RoleCode;
        }
        return PageResult<SysRole>.Get(list: endList, total: list.TotalCount);
    }

    /// <summary>
    /// 获取分页列表，仅启用用户
    /// 公开访问，无需权限
    /// </summary>
    public async Task<PageResult<SysRole>> GetRoleNoEveryonePageList(PageSearchParam param)
    {
        //查询条件
        var whereExpression = ExpressionParser<SysRole>.ParserConditions(param.filters, "user");

        //返回数据
        var list = await _entityRepository.Where(x => x.IsOpen && x.RoleCode != ConstNames.RoleCode.Everyone)
            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        var endList = list.Items.ToList();
        foreach (var role in endList)
        {
            role.IsSystem = SystemRoleCodes.Contains(role.RoleCode);
            role.IsAdmin = ConstNames.RoleCode.Administrators == role.RoleCode;
        }
        return PageResult<SysRole>.Get(list: endList, total: list.TotalCount);
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    public async Task<PageResult<SysRole>> GetAllRolePageList(PageSearchParam param)
    {
        //获取权限
        var perms = await _menuResourceService.GetResourceBizAllowPermNames();

        //权限构建
        var permExpression = ExpressionParserEx<SysRole>.ParserConditionsByPerms(perms, "user");

        //查询条件
        var whereExpression = ExpressionParser<SysRole>.ParserConditions(param.filters, "user");

        //返回数据
        var list = await _entityRepository
            .Where(permExpression)
            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        var endList = list.Items.ToList();
        foreach (var role in endList)
        {
            role.IsSystem = SystemRoleCodes.Contains(role.RoleCode);
            role.IsAdmin = ConstNames.RoleCode.Administrators == role.RoleCode;
            role.IsEveryone = ConstNames.RoleCode.Everyone == role.RoleCode;
        }
        return PageResult<SysRole>.Get(list: endList, total: list.TotalCount);
    }

    /// <summary>
    /// 获取所有角色
    /// </summary>
    public async Task<List<SysRole>> GetRoles()
    {
        //获取当前登录用户
        var loginUser = _userLoginService.GetLoginUserInfo();
        var entitis = await (from entity in _roleRepository.Entities
                             orderby entity.OrderIndex
                             select entity).ToListAsync();

        var newdata = new List<SysRole>();
        if (await _roleService.IsAdministrator(loginUser.UserAccount))
        {
            if(entitis.All(x => x.RoleCode != ConstNames.RoleCode.Administrators)) newdata.Add(new SysRole
            {
                RoleCode = ConstNames.RoleCode.Administrators,
                RoleName = ConstNames.RoleName.Administrators,
                SID = ConstRSIDs.RoleSID.Administrators,
                IsSystem = true,
                IsAdmin = true,
                IsEveryone = false,
            });
            if (entitis.All(x => x.RoleCode != ConstNames.RoleCode.Everyone)) newdata.Add(new SysRole
            {
                RoleCode = ConstNames.RoleCode.Everyone,
                RoleName = ConstNames.RoleName.Everyone,
                SID = ConstRSIDs.RoleSID.Everyone,
                IsSystem = true,
                IsAdmin = false,
                IsEveryone = true,
            });
        }
        foreach (var item in entitis)
        {
            newdata.Add(new SysRole
            {
                RoleCode = item.RoleCode,
                RoleName = item.RoleName,
                SID = item.SID,
                OrderIndex = item.OrderIndex
            });
        }
        return newdata;
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    public async Task AddRole(RoleAddParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        if (string.IsNullOrEmpty(param.RoleName))
            throw new Exception("角色名称不能为空！");

        //系统角色进行校验
        if (SystemRoleCodes.Contains(param.RoleCode) || SystemRoleNames.Contains(param.RoleName))
            throw new Exception("系统角色不需要创建！");

        var roles = await (from entity in _roleRepository.Entities
                           where entity.RoleCode == param.RoleCode || entity.RoleName == param.RoleName
                           select entity).ToListAsync();

        if (roles.Any(r => r.RoleCode == param.RoleCode))
            throw new Exception($"角色编码 “{param.RoleCode}” 已存在，不能重复新增！");

        if (roles.Any(r => r.RoleName == param.RoleName))
            throw new Exception($"角色 “{param.RoleName}” 已存在，不能重复新增！");

        //角色排序
        var maxOrderIndex = await MaxAsync(x => true, x => x.OrderIndex);
        if (maxOrderIndex <= 0) maxOrderIndex = 0;
        maxOrderIndex = maxOrderIndex + 1;

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //角色信息
        var roleModel = new SysRole()
        {
            RoleCode = param.RoleCode,
            RoleName = param.RoleName,
            OrderIndex = maxOrderIndex,
            IsOpen = param.IsOpen,
            Description = param.Description,
            CreatorAccount = loginUser.UserAccount,
            CreatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName,
            CreatedTime = DateTime.Now,
        };

        //事务执行
        await _roleRepository.InsertNowAsync(roleModel);

        //添加角色成员
        //var members = new List<SysRoleMember>();
        //if (param.Members != null && param.Members.Count > 0)
        //{
        //    foreach (var mer in param.Members)
        //    {
        //        members.Add(new SysRoleMember
        //        {
        //            RoleCode = param.RoleCode,
        //            SID = mer.SID,
        //            SIDType = GetSIDType(mer.SIDType),
        //            OrderIndex = members.Count + 1,
        //            CreatorAccount = loginUser.UserAccount,
        //            CreatorName = loginUser.UserName,
        //            CreatedTime = DateTime.Now,
        //        });
        //    }
        //}

        //事务执行
        //await TryTransDoTask(async (dbContext, serviceProvider) =>
        //{
        //    await _roleRepository.InsertNowAsync(roleModel);
        //    if (members.Count > 0)
        //        await _roleMemberRepository.InsertNowAsync(members);
        //});
    }

    /// <summary>
    /// 重命名角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task ModifyRole(RoleModifyParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        if (string.IsNullOrEmpty(param.RoleName))
            throw new Exception("角色名称不能为空！");

        //系统角色进行校验
        if (SystemRoleCodes.Contains(param.RoleCode) || SystemRoleNames.Contains(param.RoleName))
            throw new Exception("系统角色不能被修改！");

        //角色信息
        var role = (from entity in _roleRepository.Entities
                    where entity.RoleCode == param.RoleCode
                    select entity).FirstOrDefault();
        if (role == null)
            throw new Exception($"未找到角色 “{param.RoleName}（{param.RoleCode}）”！");

        var tempRole = (from entity in _roleRepository.Entities
                        where entity.RoleName == param.RoleName && entity.SID != role.SID
                        select entity).FirstOrDefault();
        if (tempRole != null)
            throw new Exception($"角色 “{param.RoleName}” 已存在！");

        ////角色成员
        //var roleMembers = (from entity in _roleMemberRepository.Entities
        //                   where entity.RoleCode == param.RoleCode
        //                   select entity).ToList();

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        role.RoleName = param.RoleName;
        role.OrderIndex = param.OrderIndex;
        role.IsOpen = param.IsOpen;
        role.Description = param.Description;
        role.UpdatorAccount = loginUser.UserAccount;
        role.UpdatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName;
        role.UpdatedTime = DateTime.Now;
        await _roleRepository.UpdateNowAsync(role);
    }

    /// <summary>
    /// 设置是否启用
    /// </summary>
    public async Task SetIsOpen(SetIsOpenParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        //系统角色进行校验
        if (SystemRoleCodes.Contains(param.RoleCode))
            throw new Exception("系统角色不能被修改！");

        //角色信息
        var role = (from entity in _roleRepository.Entities
                    where entity.RoleCode == param.RoleCode
                    select entity).FirstOrDefault();
        if (role == null)
            throw new Exception($"未找到角色 “{param.RoleName}（{param.RoleCode}）”！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        role.IsOpen = param.IsOpen;
        role.UpdatorAccount = loginUser.UserAccount;
        role.UpdatorName = loginUser.UserName;
        role.UpdatedTime = DateTime.Now;
        await _roleRepository.UpdateNowAsync(role);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRole(RoleBaseParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        //系统角色进行校验
        if (SystemRoleCodes.Contains(param.RoleCode))
            throw new Exception("系统角色不能被删除！");

        var role = (from entity in _roleRepository.Entities
                    where entity.RoleCode == param.RoleCode
                    select entity).FirstOrDefault();
        if (role != null)
        {
            ////删除的角色成员
            //var roleMembers = await (from entity in _roleMemberRepository.Entities
            //                         where entity.RoleCode == param.RoleCode
            //                         select entity).ToListAsync();

            //删除角色对应的授权
            //var acls = await (from entity in _resourceAclRepository.Entities
            //                  where entity.RoleType == SIDType.RoleSID.ToString() && entity.RoleParam1 == role.SID
            //                  select entity).ToListAsync();

            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除角色
                await _roleRepository.DeleteAsync(role);

                //删除角色成员
                await _roleMemberRepository.Where(entity => entity.RoleCode == param.RoleCode).ExecuteDeleteAsync();

                //删除角色组与角色对应关系
                await _roleGroupMemberRepository.Where(entity => entity.SID == role.SID).ExecuteDeleteAsync();

                //删除角色对应的授权
                await _resourceAclRepository.Where(entity => entity.RoleType == Core.Common.Enums.SIDTypeEnum.RoleSID.ToString() && entity.RoleParam1 == role.SID).ExecuteDeleteAsync();

                //if (roleMembers.Count > 0)
                //    await _roleMemberRepository.DeleteNowAsync(roleMembers);
                //if (acls.Count > 0)
                //    await _resourceAclRepository.DeleteNowAsync(acls);
            });
        }
    }

    /// <summary>
    /// 删除多个角色
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRoles(RoleDeletesParam param)
    {
        if (param.RoleCodes == null || param.RoleCodes.Count == 0)
            throw new Exception("角色编码不能为空！");

        foreach (var roleCode in param.RoleCodes)
        {
            //系统角色进行校验
            if (SystemRoleCodes.Contains(roleCode))
                throw new Exception("系统角色不能被删除！");
        }

        var roles = (from entity in _roleRepository.Entities
                     where param.RoleCodes.Contains(entity.RoleCode)
                     orderby entity.OrderIndex
                     select entity).ToList();

        if (roles != null && roles.Count > 0)
        {
            //删除的角色成员
            //var roleMembers = await (from entity in _roleMemberRepository.Entities
            //                         where param.RoleCodes.Contains(entity.RoleCode)
            //                         select entity).ToListAsync();

            //删除角色对应的授权
            var roleSids = roles.Select(r => r.SID).ToList();
            //var acls = await (from entity in _resourceAclRepository.Entities
            //                  where entity.RoleType == Core.Common.Enums.SIDType.RoleSID.ToString() && roleSids.Contains(entity.RoleParam1)
            //                  select entity).ToListAsync();

            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除角色
                await _roleRepository.DeleteAsync(roles);

                //删除的角色组成员
                await _roleMemberRepository.Where(entity => param.RoleCodes.Contains(entity.RoleCode)).ExecuteDeleteAsync();

                //删除角色组与角色对应关系
                await _roleGroupMemberRepository.Where(entity => roleSids.Contains(entity.SID)).ExecuteDeleteAsync();

                //删除角色组对应的授权
                await _resourceAclRepository.Where(entity => entity.RoleType == Core.Common.Enums.SIDTypeEnum.RoleSID.ToString() && roleSids.Contains(entity.RoleParam1)).ExecuteDeleteAsync();

                //if (roleMembers.Count > 0)
                //    await _roleMemberRepository.DeleteAsync(roleMembers);
                //if (acls.Count > 0)
                //    await _resourceAclRepository.DeleteAsync(acls);
            });
        }
    }

    /// <summary>
    /// 获取角色成员
    /// </summary>
    public async Task<List<RoleMemberResult>> GetRoleMembers(RoleBaseParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        //非系统角色进行校验
        if (!SystemRoleCodes.Contains(param.RoleCode))
        {
            var role = (from entity in _roleRepository.Entities
                        where entity.RoleCode == param.RoleCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色 “{param.RoleCode}”！");
        }

        var members = new List<RoleMemberResult>();
        if (param.RoleCode != ConstNames.RoleCode.Everyone)
        {
            //角色成员
            var roleMembers = await (from entity in _roleMemberRepository.Entities
                                     where entity.RoleCode == param.RoleCode
                                     orderby entity.OrderIndex
                                     select entity).ToListAsync();
            foreach (var member in roleMembers)
            {
                members.Add(new RoleMemberResult
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
    /// 添加一个角色成员
    /// </summary>
    public async Task AddRoleMember(RoleMemberAddParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色名称不能为空！");

        if (param.RoleCode == ConstNames.RoleCode.Everyone)
            throw new Exception("Everyone 角色不能添加成员！");

        if (param.Member == null)
            throw new Exception("请提供需要添加的角色成员信息！");

        if (param.RoleCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleRepository.Entities
                        where entity.RoleCode == param.RoleCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色 “{param.RoleCode}”！");
        }

        //添加角色成员
        var roleMembers = await (from entity in _roleMemberRepository.Entities
                                 where entity.RoleCode == param.RoleCode
                                 select entity).ToListAsync();
        if (!roleMembers.Any(m => m.SID == param.Member.SID && m.SIDType == param.Member.SIDType))
        {
            var member = new SysRoleMember
            {
                RoleCode = param.RoleCode,
                SID = param.Member.SID,
                SIDType = param.Member.SIDType
            };
            await _roleMemberRepository.InsertNowAsync(member);
        }
    }

    /// <summary>
    /// 添加多个角色成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task AddRoleMembers(RoleMembersAddParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        if (param.RoleCode == ConstNames.RoleCode.Everyone)
            throw new Exception("Everyone 角色不能添加成员！");

        if (param.Members == null || param.Members.Count == 0)
            throw new Exception("请提供需要添加的角色成员信息！");

        if (param.RoleCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleRepository.Entities
                        where entity.RoleCode == param.RoleCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色 “{param.RoleCode}”！");
        }

        //添加角色成员
        var roleMembers = (from entity in _roleMemberRepository.Entities
                           where entity.RoleCode == param.RoleCode
                           select entity).ToList();

        var addMembers = new List<SysRoleMember>();
        foreach (var member in param.Members)
        {
            if (!roleMembers.Any(m => m.SID == member.SID && m.SIDType == member.SIDType))
            {
                addMembers.Add(new SysRoleMember
                {
                    RoleCode = param.RoleCode,
                    SID = member.SID,
                    SIDType = member.SIDType
                });
            }
        }

        //事务执行
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            await _roleMemberRepository.InsertAsync(addMembers);
        });
    }

    /// <summary>
    /// 删除角色成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRoleMember(RoleMemberDeleteParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色名称不能为空！");

        if (param.Member == null || string.IsNullOrEmpty(param.Member.SID))
            throw new Exception("请提供需要添加的角色成员信息！");

        if (param.RoleCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleRepository.Entities
                        where entity.RoleCode == param.RoleCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色 “{param.RoleCode}”！");
        }

        //添加角色成员
        var roleMembers = await (from entity in _roleMemberRepository.Entities
                                 where entity.RoleCode == param.RoleCode
                                 select entity).ToListAsync();

        var oldEntity = roleMembers.FirstOrDefault(m => m.SID == param.Member.SID && m.SIDType == param.Member.SIDType);
        if (oldEntity != null)
        {
            await _roleMemberRepository.DeleteNowAsync(oldEntity);
        }
    }

    /// <summary>
    /// 删除多个角色成员
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task DeleteRoleMembers(RoleMemberDeletesParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色名称不能为空！");

        if (param.Members == null || param.Members.Count == 0)
            throw new Exception("请提供需要添加的角色成员信息！");

        if (param.RoleCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleRepository.Entities
                        where entity.RoleCode == param.RoleCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色 “{param.RoleCode}”！");
        }

        //角色成员
        var roleMembers = await (from entity in _roleMemberRepository.Entities
                                 where entity.RoleCode == param.RoleCode
                                 select entity).ToListAsync();

        var deleteMembers = new List<SysRoleMember>();
        foreach (var member in param.Members)
        {
            if (string.IsNullOrEmpty(member.SID))
                throw new Exception("请提供需要添加的角色成员信息！");

            var oldEntity = roleMembers.FirstOrDefault(m => m.SID == member.SID && m.SIDType == member.SIDType);
            if (oldEntity != null)
            {
                deleteMembers.Add(oldEntity);
            }
        }
        await TryTransDoTaskAsync(async (dbContext) =>
        {
            await _roleMemberRepository.DeleteAsync(deleteMembers);
        });
    }

    /// <summary>
    /// 保存角色成员
    /// </summary>
    public async Task SaveRoleMembers(SaveRoleMembersParam param)
    {
        if (string.IsNullOrEmpty(param.RoleCode))
            throw new Exception("角色编码不能为空！");

        if (param.RoleCode == ConstNames.RoleCode.Everyone)
            throw new Exception("Everyone 角色不能添加成员！");

        if (param.RoleCode != ConstNames.RoleCode.Administrators)
        {
            var role = (from entity in _roleRepository.Entities
                        where entity.RoleCode == param.RoleCode
                        select entity).FirstOrDefault();
            if (role == null)
                throw new Exception($"未找到角色 “{param.RoleCode}”！");
        }

        //现有的角色成员
        var roleMembers = await (from entity in _roleMemberRepository.Entities
                                 where entity.RoleCode == param.RoleCode
                                 select entity).ToListAsync();

        //构建
        var insertMembers = new List<SysRoleMember>();
        var updateMembers = new List<SysRoleMember>();
        var deleteMembers = new List<SysRoleMember>();
        for (var index = 0; index < param.Members.Count; index++)
        {
            var member = param.Members[index];
            var oldMember = roleMembers.FirstOrDefault(m => m.SID == member.SID && m.SIDType == member.SIDType);
            if (oldMember == null)
            {
                insertMembers.Add(new SysRoleMember
                {
                    RoleCode = param.RoleCode,
                    SID = member.SID,
                    SIDType = member.SIDType,
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
            var newMember = param.Members.FirstOrDefault(m => m.SID == member.SID && m.SIDType == member.SIDType);
            if (newMember == null)
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
    /// 获取成员显示名
    /// </summary>
    private string GetMemberDisplayName(SysRoleMember member)
    {
        var sidType = GetSIDType(member.SIDType);
        switch (sidType)
        {
            case SIDTypeEnum.UserSID:
                var user = _userService.TryGetUserBySID(member.SID);
                if (user != null) return $"{user.Name}({user.Account})";
                else return $"{member.SID}(未知用户)";
            //case Core.Common.Enums.SIDType.OUSID:
            //    var ou = OUHelper.FromSID(member.SID);
            //    return ou.OUFullName;
            default:
                return $"(不支持的类型：{member.SIDType})";
        }
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    private Core.Common.Enums.SIDTypeEnum GetSIDType(string sidType)
    {
        Core.Common.Enums.SIDTypeEnum type;
        if (!System.Enum.TryParse(sidType, out type))
            throw new Exception($"无效的成员类型 “{sidType}”！");
        return type;
    }
}
