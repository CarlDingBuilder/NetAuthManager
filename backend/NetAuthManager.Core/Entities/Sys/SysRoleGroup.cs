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
/// RDM 角色组表
/// </summary>
[Table("SYS_ROLEGROUP")]
public class SysRoleGroup : BaseGuidInfoEntity, IEntityTypeBuilder<SysRoleGroup>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [NotMapped]
    public override string Id { get; set; }

    /// <summary>
    /// 角色组编码
    /// </summary>
    [Key]
    [Column("GROUPCODE"), Required, MaxLength(50)]
    [Comment("角色组编码")]
    public string GroupCode { get; set; }

    /// <summary>
    /// 角色组名称
    /// </summary>
    [Column("GROUPNAME"), Required, MaxLength(50)]
    [Comment("角色组名称")]
    public string GroupName { get; set; }

    /// <summary>
    /// 服务实体编码
    /// </summary>
    [Column("SERVICEENTITY"), MaxLength(50)]
    [Comment("服务实体")]
    public string ServiceEntityCode { get; set; }

    /// <summary>
    /// 服务实体名称
    /// </summary>
    [NotMapped]
    public string ServiceEntityName { get; set; }

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
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysRoleGroup> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        entityBuilder.HasKey(u => u.GroupCode);
        SetGuidColumn(entityBuilder, u => u.SID, "SID");
        SetDefaultCreatedTime(entityBuilder);
    }
}
