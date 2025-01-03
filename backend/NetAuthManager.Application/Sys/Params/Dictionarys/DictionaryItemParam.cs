using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Dictionarys;

/// <summary>
/// 字典项基本参数
/// </summary>
public class DictionaryItemBaseParam
{
    /// <summary>
    /// 字典项ID
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 字典ID
    /// </summary>
    public string TypeId { get; set; }

    /// <summary>
    /// 字典项编码
    /// </summary>
    public string ItemCode { get; set; }

    /// <summary>
    /// 字典项名称
    /// </summary>
    public string ItemName { get; set; }

    /// <summary>
    /// 扩展字段01
    /// </summary>
    public string Ext01 { get; set; }

    /// <summary>
    /// 扩展字段02
    /// </summary>
    public string Ext02 { get; set; }

    /// <summary>
    /// 扩展字段03
    /// </summary>
    public string Ext03 { get; set; }

    /// <summary>
    /// 扩展字段04
    /// </summary>
    public string Ext04 { get; set; }

    /// <summary>
    /// 扩展字段05
    /// </summary>
    public string Ext05 { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}

/// <summary>
/// 字典项添加参数
/// </summary>
public class DictionaryItemAddParam : DictionaryItemBaseParam
{
}

/// <summary>
/// 字典项修改参数
/// </summary>
public class DictionaryItemModifyParam : DictionaryItemBaseParam
{
}

/// <summary>
/// 字典项启用参数
/// </summary>
public class SetDictionaryItemIsOpenParam : DictionaryItemBaseParam
{
}

/// <summary>
/// 字典项删除参数
/// </summary>
public class DictionaryItemDeletesParam
{
    /// <summary>
    /// 字典项Id
    /// </summary>
    public List<string> Ids { get; set; }
}

/// <summary>
/// 字典项排序
/// </summary>
public class DictionaryItemOrderParam
{
    public List<DictionaryItemOrderParam_Item> Items { get; set; }
}
public class DictionaryItemOrderParam_Item
{
    public string Id { get; set; }
    public int OrderIndex { get; set; }
}