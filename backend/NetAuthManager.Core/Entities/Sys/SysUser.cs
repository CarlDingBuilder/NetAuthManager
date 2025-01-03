using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// 用户表
/// </summary>
[Table("SYS_USER")]
[Comment("用户表")]
public class SysUser : BaseGuidInfoEntity, IEntityTypeBuilder<SysUser>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [NotMapped]
    public override string Id { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    [Key]
    [Column("ACCOUNT")]
    [Comment("账号")]
    [MaxLength(50)]
    public string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Column("PASSWORD")]
    [Comment("密码")]
    [MaxLength(200)]
    public string Password { get; set; }

    /// <summary>
    /// SID
    /// </summary>
    [Column("SID"), Required, MaxLength(50)]
    [Comment("SID")]
    public string SID { get; set; } = GetNewGuid();

    /// <summary>
    /// 员工工号
    /// </summary>
    [Column("HRID")]
    [Comment("员工工号")]
    [MaxLength(50)]
    public string HRID { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [Column("NAME")]
    [Comment("姓名")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [Column("DESCRIPTION")]
    [Comment("描述")]
    [MaxLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// 来源系统
    /// </summary>
    [Column("SOURCESYSTEM")]
    [Comment("来源系统")]
    [MaxLength(50)]
    public string SourceSystem { get; set; }

    /// <summary>
    /// 是否同步
    /// </summary>
    [Column("ISSYNC")]
    [Comment("是否同步")]
    public bool IsSync { get; set; } = false;

    /// <summary>
    /// 是否启用
    /// 仅当账号和员工都启用时，才是有效的用户
    /// </summary>
    [Column("ISOPEN")]
    [Comment("是否启用")]
    public bool IsOpen { get; set; } = true;

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysUser> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        entityBuilder.HasKey(u => u.Account);
        SetGuidColumn(entityBuilder, u => u.SID, "SID");
        SetDefaultCreatedTime(entityBuilder);
    }
}
