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
[Table("SYS_RESOURCE_PERM")]
public class SysResourcePerm : BaseGuidInfoEntity, IEntityTypeBuilder<SysResourcePerm>
{
    /// <summary>
    /// 资源ID
    /// </summary>
    [Column("SID"), Required, MaxLength(50)]
    public string SID { get; set; }

    /// <summary>
    /// 权限名
    /// </summary>
    [Column("PERM_NAME"), Required, MaxLength(100)]
    public string PermName { get; set; }

    /// <summary>
    /// 权限名
    /// </summary>
    [Column("PERM_DISPLAYNAME"), MaxLength(100)]
    public string PermDisplayName { get; set; }

    /// <summary>
    /// 权限类型：模块、记录
    /// </summary>
    [Column("PERM_TYPE"), Required, MaxLength(10)]
    public string PermType { get; set; }

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
    public void Configure(EntityTypeBuilder<SysResourcePerm> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);
    }
}
