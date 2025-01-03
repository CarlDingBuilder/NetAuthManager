using NetAuthManager.Core.Exceptions;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetAuthManager.Application.Handlers.Models;
using Newtonsoft.Json;
using NetAuthManager.Core.Common.Enums;

namespace NetAuthManager.Application.Handlers;

/// <summary>
/// 权限授权属性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
public class PermAuthorizeAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 权限授权项
    /// </summary>
    public List<PermAuthItem> PermsAuthItems { get; set; }

    /// <summary>
    /// 权限全路径
    /// </summary>
    public string PermFullPath { get { return PermsAuthItems.FirstOrDefault()?.PermFullPath ?? string.Empty; } }

    ///// <summary>
    ///// 权限操作名称
    ///// </summary>
    //public string PermName { get { return PermsAuthItems.FirstOrDefault()?.PermName ?? string.Empty; } }

    /// <summary>
    /// 单权限授权属性定义
    /// </summary>
    /// <param name="PermFullPath">权限资源全路径</param>
    /// <param name="Perm">权限枚举，默认值访问</param>
    /// <param name="PermName">权限名称，存在权限名称时，取权限名称，否则取权限枚举</param>
    public PermAuthorizeAttribute(string PermFullPath, PermTypeEnum Perm = PermTypeEnum.Execute, string PermName = null)
    {
        PermsAuthItems = new List<PermAuthItem>
        {
            new PermAuthItem
            {
                PermFullPath = PermFullPath,
                PermName = PermName ?? Perm.ToString()
            }
        };
    }

    /// <summary>
    /// 多权限授权属性定义
    /// 只要有一个权限项有权限则有权限
    /// </summary>
    /// <param name="Perms">权限项</param>
    public PermAuthorizeAttribute(string Perms)
    {
        if (string.IsNullOrEmpty(Perms)) PermsAuthItems = new List<PermAuthItem>();
        else
        {
            PermsAuthItems = JsonConvert.DeserializeObject<List<PermAuthItem>>(Perms).ToList();
        }
    }

    /// <summary>
    /// 多权限授权属性定义
    /// 只要有一个权限项有权限则有权限
    /// </summary>
    /// <param name="Perms">权限项</param>
    public PermAuthorizeAttribute(List<PermAuthItem> Perms)
    {
        PermsAuthItems = Perms;
    }
}
