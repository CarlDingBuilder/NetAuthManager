using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// 系统菜单权限模块
/// </summary>
[Table("SYS_RESOURCE_PERM_ACL")]
public class SysResourcePermAcl : BaseGuidInfoEntity, IEntityTypeBuilder<SysResourcePermAcl>
{
    /// <summary>
    /// 资源ID
    /// </summary>
    [Column("SID"), Required, MaxLength(50)]
    public string SID { get; set; }

    /// <summary>
    /// 允许的权限名，多个逗号间隔
    /// </summary>
    [Column("ALLOW_PERMS"), MaxLength(1000)]
    public string AllowPerms { get; set; }

    /// <summary>
    /// 禁止的权限名，多个逗号间隔
    /// </summary>
    [Column("DENY_PERMS"), MaxLength(1000)]
    public string DenyPerms { get; set; }

    /// <summary>
    /// 授权角色类型：角色、用户、角色组
    /// </summary>
    [Column("ROLE_TYPE"), MaxLength(50)]
    public string RoleType { get; set; }

    /// <summary>
    /// 授权角色参数
    /// 授权角色编码存储该字段
    /// </summary>
    [Column("ROLE_PARAM1"), MaxLength(200)]
    public string RoleParam1 { get; set; }

    /// <summary>
    /// 授权角色参数
    /// </summary>
    [Column("ROLE_PARAM2"), MaxLength(200)]
    public string RoleParam2 { get; set; }

    /// <summary>
    /// 授权角色参数
    /// </summary>
    [Column("ROLE_PARAM3"), MaxLength(200)]
    public string RoleParam3 { get; set; }

    /// <summary>
    /// 是否继承
    /// </summary>
    [Column("INHERITED"), Required, DefaultValue(false)]
    public bool Inherited { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    [Column("ORDERINDEX"), Required, DefaultValue(0)]
    public int OrderIndex { get; set; }

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysResourcePermAcl> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);
    }
}
