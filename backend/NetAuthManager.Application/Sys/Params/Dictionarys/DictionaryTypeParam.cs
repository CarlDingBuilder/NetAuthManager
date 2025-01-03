using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Dictionarys;

/// <summary>
/// 字典基本参数
/// </summary>
public class DictionaryTypeBaseParam
{
    /// <summary>
    /// 字典ID
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 业务编码
    /// </summary>
    public string TypeCode { get; set; }

    /// <summary>
    /// 业务名称
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 扩展字段名称01
    /// </summary>
    public string ExtName01 { get; set; }

    /// <summary>
    /// 扩展字段名称02
    /// </summary>
    public string ExtName02 { get; set; }

    /// <summary>
    /// 扩展字段名称03
    /// </summary>
    public string ExtName03 { get; set; }

    /// <summary>
    /// 扩展字段名称04
    /// </summary>
    public string ExtName04 { get; set; }

    /// <summary>
    /// 扩展字段名称05
    /// </summary>
    public string ExtName05 { get; set; }

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
/// 字典添加参数
/// </summary>
public class DictionaryTypeAddParam : DictionaryTypeBaseParam
{
}

/// <summary>
/// 字典修改参数
/// </summary>
public class DictionaryTypeModifyParam : DictionaryTypeBaseParam
{
}

/// <summary>
/// 字典启用参数
/// </summary>
public class SetDictionaryTypeIsOpenParam : DictionaryTypeBaseParam
{
}

/// <summary>
/// 字典删除参数
/// </summary>
public class DictionaryTypeDeletesParam
{
    /// <summary>
    /// 字典Id
    /// </summary>
    public List<string> Ids { get; set; }
}
