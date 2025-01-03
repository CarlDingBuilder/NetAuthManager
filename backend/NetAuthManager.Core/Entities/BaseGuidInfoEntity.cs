using NetAuthManager.Core.Helper;
using Furion.DatabaseAccessor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// 实体信息基类
/// </summary>
public abstract class BaseGuidInfoEntity : BaseGuidEntity
{
    /// <summary>
    /// 新建人账号
    /// </summary>
    //[Required]
    [Column("CREATOR_ACCOUNT")]
    [Comment("新建人账号")]
    [MaxLength(50)]
    [JsonPropertyOrder(1000)]
    public virtual string CreatorAccount { get; set; } = string.Empty;

    /// <summary>
    /// 新建人姓名
    /// </summary>
    [Column("CREATOR_NAME")]
    [Comment("新建人姓名")]
    [MaxLength(50)]
    [JsonPropertyOrder(1001)]
    public virtual string CreatorName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    [Column("CREATED_TIME")]
    [Comment("创建时间")]
    [JsonPropertyOrder(1002)]
    public virtual DateTime CreatedTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新人账号
    /// </summary>
    [Column("UPDATOR_ACCOUNT")]
    [Comment("更新人账号")]
    [MaxLength(50)]
    [JsonPropertyOrder(1003)]
    public virtual string UpdatorAccount { get; set; }

    /// <summary>
    /// 更新人姓名
    /// </summary>
    [Column("UPDATOR_NAME")]
    [Comment("更新人姓名")]
    [MaxLength(50)]
    [JsonPropertyOrder(1004)]
    public virtual string UpdatorName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("UPDATED_TIME")]
    [Comment("更新时间")]
    [JsonPropertyOrder(1005)]
    public virtual DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public new void Configure<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, DbContext dbContext, Type dbContextLocator) where TEntity : BaseGuidInfoEntity
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);

        SetDefaultCreatedTime(entityBuilder);
    }

    /// <summary>
    /// 设置新建时间默认值
    /// </summary>
    protected void SetDefaultCreatedTime<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : BaseGuidInfoEntity
    {
        SetDefaultTime(entityBuilder, u => u.CreatedTime, "创建时间");
    }
}