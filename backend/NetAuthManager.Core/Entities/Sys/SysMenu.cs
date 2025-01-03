using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace NetAuthManager.Core.Entities;

/// <summary>
/// 菜单表
/// </summary>
[Table("SYS_MENUS")]
[Comment("菜单表")]
public class SysMenu : BaseGuidInfoEntity, IEntityTypeBuilder<SysMenu>
{
    /// <summary>
    /// 父ID
    /// </summary>
    [Column("PID")]
    [Comment("父ID")]
    [MaxLength(50)]
    public string PId { get; set; }

    /// <summary>
    /// 用于标识菜单，默认值为：default，如果有多种菜单会使用到这个值
    /// </summary>
    [Column("PCODE")]
    [Comment("菜单分类")]
    [MaxLength(20)]
    public string PCode { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Column("NAME")]
    [Comment("菜单名称")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 菜单英文名称
    /// </summary>
    [Column("NAMEEN")]
    [Comment("菜单英文名称")]
    [MaxLength(80)]
    public string NameEn { get; set; }

    /// <summary>
    /// 菜单地址
    /// </summary>
    [Column("URL")]
    [Comment("菜单地址")]
    [MaxLength(200)]
    public string Url { get; set; }

    /// <summary>
    /// 菜单地址类型
    /// </summary>
    [Column("URLTYPE")]
    [Comment("菜单地址类型")]
    [MaxLength(50)]
    public string UrlType { get; set; }

    /// <summary>
    /// Icon
    /// </summary>
    [Column("ICON")]
    [Comment("图标")]
    [MaxLength(30)]
    public string Icon { get; set; }

    /// <summary>
    /// 是否菜单
    /// </summary>
    [Column("ISMENU")]
    [Comment("是否菜单")]
    public bool IsMenu { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Column("ORDERINDEX")]
    [Comment("排序")]
    public int OrderIndex { get; set; }

    /// <summary>
    /// RSID
    /// </summary>
    [Column("RSID")]
    [Comment("RSID")]
    [MaxLength(50)]
    public string RSID { get; set; }

    /// <summary>
    /// 菜单层级
    /// </summary>
    [Column("MENULEVEL")]
    [Comment("菜单层级")]
    public int MenuLevel { get; set; }

    /// <summary>
    /// 资源名称
    /// </summary>
    [Column("RESOURCENAME")]
    [Comment("资源名称")]
    [MaxLength(50)]
    public string ResourceName { get; set; }

    /// <summary>
    /// 是否展开
    /// </summary>
    [Column("ISSPREAD")]
    [Comment("是否展开")]
    public bool IsSpread { get; set; }

    /// <summary>
    /// 配置JSON
    /// </summary>
    [Column("CONFIGJSON")]
    [Comment("配置JSON")]
    [MaxLength(1000)]
    public string ConfigJson { get; set; }

    /// <summary>
    /// 标记
    /// </summary>
    [Column("BADGE")]
    [Comment("标记")]
    [MaxLength(200)]
    public string Badge { get; set; }

    /// <summary>
    /// 模块类型
    /// </summary>
    [Column("MODULETYPE")]
    [Comment("模块类型")]
    [MaxLength(50)]
    public string ModuleType { get; set; }

    /// <summary>
    /// 不可关闭
    /// </summary>
    [Column("NOCLOSABLE")]
    [Comment("不可关闭")]
    public bool NoClosable { get; set; }

    /// <summary>
    /// 不保存活动
    /// </summary>
    [Column("NOKEEPALIVE")]
    [Comment("不保存活动")]
    public bool NoKeepAlive { get; set; }

    /// <summary>
    /// 父菜单
    /// </summary>
    [NotMapped]
    public SysMenu Parent { get; set; } = null;

    /// <summary>
    /// 子菜单
    /// </summary>
    [NotMapped]
    public List<SysMenu> Children { get; set; } = new List<SysMenu>();

    /// <summary>
    /// 配置数据库实体
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="dbContext"></param>
    /// <param name="dbContextLocator"></param>
    public void Configure(EntityTypeBuilder<SysMenu> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        base.Configure(entityBuilder, dbContext, dbContextLocator);
    }
}
