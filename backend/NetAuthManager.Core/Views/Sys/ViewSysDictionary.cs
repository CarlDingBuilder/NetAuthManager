using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Views.Sys;

/// <summary>
/// 数据字典视图
/// </summary>
[Comment("数据字典视图")]
[NotMapped]
public class ViewSysDictionary
{
    /// <summary>
    /// 业务编码
    /// </summary>
    [Column("TYPECODE")]
    public string TypeCode { get; set; }

    /// <summary>
    /// 业务名称
    /// </summary>
    [Column("TYPENAME")]
    public string TypeName { get; set; }

    /// <summary>
    /// 字典项编码
    /// </summary>
    [Column("ITEMCODE")]
    public string ItemCode { get; set; }

    /// <summary>
    /// 字典项名称
    /// </summary>
    [Column("ITEMNAME")]
    public string ItemName { get; set; }

    /// <summary>
    /// 扩展字段01
    /// </summary>
    [Column("EXT01")]
    public string Ext01 { get; set; }

    /// <summary>
    /// 扩展字段02
    /// </summary>
    [Column("EXT02")]
    public string Ext02 { get; set; }

    /// <summary>
    /// 扩展字段03
    /// </summary>
    [Column("EXT03")]
    public string Ext03 { get; set; }

    /// <summary>
    /// 扩展字段04
    /// </summary>
    [Column("EXT04")]
    public string Ext04 { get; set; }

    /// <summary>
    /// 扩展字段05
    /// </summary>
    [Column("EXT05")]
    public string Ext05 { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    [Column("ORDERINDEX")]
    public int OrderIndex { get; set; }
}
