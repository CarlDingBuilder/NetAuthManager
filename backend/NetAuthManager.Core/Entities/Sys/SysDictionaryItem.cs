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
/// 数据字典项表
/// </summary>
[Table("SYS_DICTIONARY_ITEM")]
[Comment("数据字典项表")]
public class SysDictionaryItem : BaseGuidInfoEntity, IEntityTypeBuilder<SysDictionaryItem>
{
    /// <summary>
    /// 类型ID
    /// </summary>
    [Column("TYPEID"), Required, MaxLength(50)]
    [Comment("角色编码")]
    public string TypeId { get; set; }

    /// <summary>
    /// 字典项编码
    /// </summary>
    [Column("ITEMCODE"), Required, MaxLength(50)]
    [Comment("字典项编码")]
    public string ItemCode { get; set; }

    /// <summary>
    /// 字典项名称
    /// </summary>
    [Column("ITEMNAME"), Required, MaxLength(50)]
    [Comment("角色名称")]
    public string ItemName { get; set; }

    /// <summary>
    /// 扩展字段01
    /// </summary>
    [Column("EXT01")]
    [Comment("扩展字段01")]
    [MaxLength(50)]
    public string Ext01 { get; set; }

    /// <summary>
    /// 扩展字段02
    /// </summary>
    [Column("EXT02")]
    [Comment("扩展字段02")]
    [MaxLength(50)]
    public string Ext02 { get; set; }

    /// <summary>
    /// 扩展字段03
    /// </summary>
    [Column("EXT03")]
    [Comment("扩展字段03")]
    [MaxLength(50)]
    public string Ext03 { get; set; }

    /// <summary>
    /// 扩展字段04
    /// </summary>
    [Column("EXT04")]
    [Comment("扩展字段04")]
    [MaxLength(50)]
    public string Ext04 { get; set; }

    /// <summary>
    /// 扩展字段05
    /// </summary>
    [Column("EXT05")]
    [Comment("扩展字段05")]
    [MaxLength(50)]
    public string Ext05 { get; set; }

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
    public void Configure(EntityTypeBuilder<SysDictionaryItem> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);
    }
}
