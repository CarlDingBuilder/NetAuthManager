using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// 菜单类型表
/// </summary>
[Table("SYS_MENUS_TYPE")]
[Comment("菜单类型表")]
public class SysMenuType : BaseGuidInfoEntity, IEntityTypeBuilder<SysMenuType>
{
    /// <summary>
    /// 菜单类型编码
    /// </summary>
    [Key, Required]
    [Column("PCODE")]
    [Comment("菜单类型编码")]
    [MaxLength(50)]
    public override string Id { get; set; }

    /// <summary>
    /// 菜单类型编码
    /// </summary>
    [NotMapped]
    public string PCode { get { return Id; } set { Id = value; } }

    [Column("NAME")]
    [Comment("菜单类型名称")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysMenuType> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        entityBuilder.HasKey(u => u.Id);
    }
}
