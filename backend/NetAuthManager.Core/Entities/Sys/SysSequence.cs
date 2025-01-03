using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// 序列表
/// </summary>
[Table("SYS_SEQUENCE")]
public class SysSequence : BaseGuidEntity, IEntityTypeBuilder<SysSequence>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [NotMapped]
    public override string Id { get; set; }

    /// <summary>
    /// 前缀
    /// </summary>
    [Key]
    [Column("PREFIX")]
    [Comment("前缀")]
    [MaxLength(50)]
    public string Prefix { get; set; }

    /// <summary>
    /// 当前值
    /// </summary>
    [Column("CURVALUE"), Required, DefaultValue(0)]
    [Comment("当前值")]
    public int CurValue { get; set; }

    /// <summary>
    /// 激活日期
    /// </summary>
    [Required]
    [Column("ACTIVEDATE")]
    [Comment("激活日期")]
    public DateTime ActiveDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysSequence> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        entityBuilder.HasKey(u => u.Prefix);
        SetDefaultTime(entityBuilder, u => u.ActiveDate, "激活日期");
    }
}
