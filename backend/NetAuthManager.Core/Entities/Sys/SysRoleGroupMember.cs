using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// RDM 角色组成员表
/// </summary>
[Table("SYS_ROLEGROUP_MEMBER")]
public class SysRoleGroupMember : BaseGuidInfoEntity, IEntityTypeBuilder<SysRoleGroupMember>
{
    /// <summary>
    /// 角色组编码
    /// </summary>
    [Column("GROUPCODE"), Required, MaxLength(50)]
    [Comment("角色编码")]
    public string GroupCode { get; set; }

    /// <summary>
    /// 角色组成员类型
    /// </summary>
    [NotMapped]
    public string SIDType { get; } = Common.Enums.SIDTypeEnum.RoleSID.ToString();

    /// <summary>
    /// 角色成员 SID
    /// </summary>
    [Column("SID"), Required, MaxLength(36)]
    [Comment("角色成员 SID")]
    public string SID { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    [Column("ORDERINDEX"), Required, DefaultValue(0)]
    [Comment("排序字段")]
    public int OrderIndex { get; set; }

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysRoleGroupMember> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);
    }
}
