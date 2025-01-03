using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.EntityFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 用户服务
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户
    /// </summary>
    Task<SysUser> FromAccount(string account);

    /// <summary>
    /// 获取用户
    /// </summary>
    Task<SysUser> FromAccount(DbSet<SysUser> users, string account);

    /// <summary>
    /// 获取用户
    /// </summary>
    SysUser FromSID(string sid);

    /// <summary>
    /// 尝试获取用户
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public SysUser TryGetUserBySID(string sid);

    /// <summary>
    /// 获取用户 SID
    /// </summary>
    Task<string> GetSID(string account);

    /// <summary>
    /// 获取用户 SID
    /// </summary>
    Task<string> GetSID(DefaultDbContext dbContext, string account);
}
