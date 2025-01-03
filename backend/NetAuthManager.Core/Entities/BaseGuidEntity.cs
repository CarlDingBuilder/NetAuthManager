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
/// 实体基类
/// </summary>
public abstract class BaseGuidEntity :  BaseEntity<string>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Key]
    //[Required]
    [Column("ID")]
    [Comment("主键Id")]
    [MaxLength(50)]
    [JsonPropertyOrder(0)]
    public override string Id { get; set; }

    /// <summary>
    /// 获取新的 GUID
    /// </summary>
    public static string GetNewGuid(string prefix = null)
    {
        var guid = Guid.NewGuid().ToString().ToUpper();
        if (string.IsNullOrEmpty(prefix)) return guid;
        else return $"{prefix}_{guid}";
    }

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public virtual void Configure<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, DbContext dbContext, Type dbContextLocator) where TEntity : BaseGuidEntity
    {
        SetGuidKey(entityBuilder);
    }

    /// <summary>
    /// 设置 GUID 主键
    /// </summary>
    protected void SetGuidKey<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : BaseGuidEntity
    {
        SetGuidKey(entityBuilder, u => u.Id, "主键Id");
    }

    /// <summary>
    /// 设置 GUID 主键
    /// </summary>
    protected void SetGuidKey<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, Expression<Func<TEntity, object>> expression, string comment) where TEntity : BaseGuidEntity
    {
        entityBuilder.HasKey(expression);
        SetGuidColumn(entityBuilder, expression, comment);
    }

    /// <summary>
    /// 设置 GUID 列值
    /// </summary>
    protected void SetGuidColumn<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, Expression<Func<TEntity, object>> expression, string comment) where TEntity : BaseGuidEntity
    {
        var config = ConfigHelper.GetConnectionConfig();
        if (config.DbType == DbType.SqlServer)
        {
            //SQL Server 默认GUID
            entityBuilder.Property(expression).HasDefaultValueSql("NEWID()").HasComment(comment);
        }
        else if (config.DbType == DbType.Oracle)
        {
            //Oracle 默认GUID
            entityBuilder.Property(expression).HasDefaultValueSql("RAWTOHEX(SYS_GUID())").HasComment(comment);
        }
    }

    /// <summary>
    /// 设置 Decimel
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entityBuilder"></param>
    /// <param name="expression"></param>
    /// <param name="precision"></param>
    /// <param name="scale"></param>
    protected void SetDecimal<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, Expression<Func<TEntity, object>> expression, int precision, int scale) where TEntity : class
    {
        var config = ConfigHelper.GetConnectionConfig();
        if (config.DbType == DbType.SqlServer)
        {
            entityBuilder.Property(expression).HasPrecision(precision, scale);
        }
        else if (config.DbType == DbType.Oracle)
        {
            entityBuilder.Property(expression).HasColumnType($"NUMBER({precision},{scale})");
        }
    }

    /// <summary>
    /// 设置时间默认值
    /// </summary>
    protected void SetDefaultTime<TEntity>(EntityTypeBuilder<TEntity> entityBuilder, Expression<Func<TEntity, object>> expression, string comment) where TEntity : BaseGuidEntity
    {
        var config = ConfigHelper.GetConnectionConfig();
        if (config.DbType == DbType.SqlServer)
        {
            //SQL Server 默认时间
            entityBuilder.Property(expression).HasDefaultValueSql("GETDATE()").HasComment(comment);
        }
        else if (config.DbType == DbType.Oracle)
        {
            //Oracle 默认时间
            entityBuilder.Property(expression).HasDefaultValueSql("CURRENT_TIMESTAMP").HasComment(comment);
        }
    }
}