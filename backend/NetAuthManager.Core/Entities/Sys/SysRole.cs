using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetAuthManager.Core.Common.Enums;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// RDM 角色表，区别与组织角色
/// </summary>
[Table("SYS_ROLE")]
public class SysRole : BaseGuidInfoEntity, IEntityTypeBuilder<SysRole>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [NotMapped]
    public override string Id { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    [Key]
    [Column("ROLECODE"), Required, MaxLength(50)]
    [Comment("角色编码")]
    public string RoleCode { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    [Column("ROLENAME"), Required, MaxLength(50)]
    [Comment("角色名称")]
    public string RoleName { get; set; }

    /// <summary>
    /// SID
    /// </summary>
    [Column("SID"), Required, MaxLength(36)]
    [Comment("SID")]
    public string SID { get; set; } = GetNewGuid();

    /// <summary>
    /// 排序字段
    /// </summary>
    [Column("ORDERINDEX"), Required, DefaultValue(0)]
    [Comment("排序字段")]
    public int OrderIndex { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Column("ISOPEN")]
    [Comment("是否启用")]
    public bool IsOpen { get; set; } = true;

    /// <summary>
    /// 描述
    /// </summary>
    [Column("DESCRIPTION")]
    [Comment("描述")]
    [MaxLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// 是否系统角色
    /// </summary>
    [NotMapped]
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// 是否是超级管理员角色
    /// </summary>
    [NotMapped]
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// 是否是Everyone角色
    /// </summary>
    [NotMapped]
    public bool IsEveryone { get; set; } = false;

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysRole> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        entityBuilder.HasKey(u => u.RoleCode);
        SetGuidColumn(entityBuilder, u => u.SID, "SID");
        SetDefaultCreatedTime(entityBuilder);
    }
}
