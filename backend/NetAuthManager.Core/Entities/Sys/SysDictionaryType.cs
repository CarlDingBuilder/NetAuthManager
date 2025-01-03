using NetAuthManager.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.FSSC.DBCore.Entity;

/// <summary>
/// 数据字典类型表
/// </summary>
[Table("SYS_DICTIONARY_TYPE")]
[Comment("数据字典类型表")]
public class SysDictionaryType : BaseGuidInfoEntity, IEntityTypeBuilder<SysDictionaryType>
{
    /// <summary>
    /// 业务编码
    /// </summary>
    [Column("TYPECODE"), Required, MaxLength(50)]
    [Comment("角色编码")]
    public string TypeCode { get; set; }

    /// <summary>
    /// 业务名称
    /// </summary>
    [Column("TYPENAME"), Required, MaxLength(50)]
    [Comment("角色名称")]
    public string TypeName { get; set; }

    /// <summary>
    /// 扩展字段名称01
    /// </summary>
    [Column("EXTNAME01")]
    [Comment("扩展字段名称01")]
    [MaxLength(50)]
    public string ExtName01 { get; set; }

    /// <summary>
    /// 扩展字段名称02
    /// </summary>
    [Column("EXTNAME02")]
    [Comment("扩展字段名称02")]
    [MaxLength(50)]
    public string ExtName02 { get; set; }

    /// <summary>
    /// 扩展字段名称03
    /// </summary>
    [Column("EXTNAME03")]
    [Comment("扩展字段名称03")]
    [MaxLength(50)]
    public string ExtName03 { get; set; }

    /// <summary>
    /// 扩展字段名称04
    /// </summary>
    [Column("EXTNAME04")]
    [Comment("扩展字段名称04")]
    [MaxLength(50)]
    public string ExtName04 { get; set; }

    /// <summary>
    /// 扩展字段名称05
    /// </summary>
    [Column("EXTNAME05")]
    [Comment("扩展字段名称05")]
    [MaxLength(50)]
    public string ExtName05 { get; set; }

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
    public void Configure(EntityTypeBuilder<SysDictionaryType> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);
    }
}
