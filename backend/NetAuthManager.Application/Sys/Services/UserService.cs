using NetAuthManager.Application.Sys.Mappers;
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
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 用户服务
/// </summary>
public class UserService : BaseService<SysUser>, IUserService, ITransient
{
    #region 构造与注入

    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger,
        IServiceScopeFactory scopeFactory,
        IRepository<SysUser> userRepository) : base(userRepository, scopeFactory)
    {
        _logger = logger;
    }

    #endregion 构造与注入

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public async Task<SysUser> FromAccount(string account)
    {
        return await GetUserInner(account);
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    public async Task<SysUser> FromAccount(DbSet<SysUser> users, string account)
    {
        return await GetUserInner(users, account);
    }

    /// <summary>
    /// 从 SID 获取用户，获取不到报错
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public SysUser FromSID(string sid)
    {
        //用户表
        var user = (from entity in _entityRepository.Entities
                    where entity.SID == sid
                    select entity).FirstOrDefault();
        if (user == null)
            throw new Exception($"系统出现异常，用户 SID={sid} 不存在！");

        ////扩展属性表
        //var usersExt = from userext in UsersExtInstance.Entities
        //               where userext.Account == user.Account
        //               select userext;
        //var userExt = usersExt.FirstOrDefault();

        ////扩展属性
        //user.ExtAttributes = userExt;
        //if (user.ExtAttributes == null) user.ExtAttributes = new SysUserExt();

        return user;
    }

    /// <summary>
    /// 尝试获取用户
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public SysUser TryGetUserBySID(string sid)
    {
        //用户表
        var user = (from entity in _entityRepository.Entities
                    where entity.SID == sid
                    select entity).FirstOrDefault();
        if (user == null)
            return null;

        ////扩展属性表
        //var usersExt = from userext in UsersExtInstance.Entities
        //               where userext.Account == user.Account
        //               select userext;
        //var userExt = usersExt.FirstOrDefault();

        ////扩展属性
        //user.ExtAttributes = userExt;
        //if (user.ExtAttributes == null) user.ExtAttributes = new SysUserExt();

        return user;
    }

    /// <summary>
    /// 获取用户 SID
    /// </summary>
    public async Task<string> GetSID(string account)
    {
        var user = await FromAccount(account);
        return user.SID;
    }

    /// <summary>
    /// 获取用户 SID
    /// </summary>
    public async Task<string> GetSID(DefaultDbContext dbContext, string account)
    {
        var user = await FromAccount(dbContext.Users, account);
        return user.SID;
    }

    #region 内部方法

    /// <summary>
    /// 获取用户
    /// </summary>
    private async Task<SysUser> GetUserInner(string account, bool throwError = true)
    {
        return await GetUserInner(_entityRepository.Entities, account, throwError);
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    private async Task<SysUser> GetUserInner(DbSet<SysUser> users, string account, bool throwError = true)
    {
        //用户表
        var models = from entity in users
                    where entity.Account == account
                    select entity;
        var user = await models.FirstOrDefaultAsync();
        if (user == null)
        {
            if (throwError) throw new Exception($"系统出现异常，用户 {account} 不存在！");
            else return null;
        }

        ////扩展属性表
        //var usersExt = from userext in UsersExtInstance.Entities
        //               where userext.Account == account
        //               select userext;
        //var userExt = usersExt.FirstOrDefault();

        ////扩展属性
        //user.ExtAttributes = userExt;
        //if (user.ExtAttributes == null) user.ExtAttributes = new SysUserExt();

        return user;
    }

    #endregion 内部方法
}
