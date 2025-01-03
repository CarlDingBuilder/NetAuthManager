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
/// RDM 角色成员表，区别与组织角色成员
/// </summary>
[Table("SYS_ROLE_MEMBER")]
public class SysRoleMember : BaseGuidInfoEntity, IEntityTypeBuilder<SysRoleMember>
{
    /// <summary>
    /// 角色编码
    /// </summary>
    [Column("ROLECODE"), Required, MaxLength(50)]
    [Comment("角色编码")]
    public string RoleCode { get; set; }

    /// <summary>
    /// 角色成员类型
    /// </summary>
    [Column("SIDTYPE"), Required, MaxLength(50)]
    [Comment("角色成员类型")]
    public string SIDType { get; set; } = SIDTypeEnum.UserSID.ToString();

    /// <summary>
    /// 角色成员 SID
    /// 对应用户SID
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
    public void Configure(EntityTypeBuilder<SysRoleMember> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);
    }
}
