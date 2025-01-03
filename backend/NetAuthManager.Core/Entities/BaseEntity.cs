using Furion.DatabaseAccessor;
using System;
using System.ComponentModel;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// 实体基类
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class BaseEntity<TKey> : EntityBase<TKey>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Comment("主键Id")]
    [Column("GID")]
    [MaxLength(50)]
    public override TKey Id { get; set; }
}