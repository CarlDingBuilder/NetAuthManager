using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Params;

/// <summary>
/// 分页查询参数
/// </summary>
public class BaseSearchParam
{
    /// <summary>
    /// 查询参数
    /// </summary>
    /// <example>
    /// [
    ///     {
    ///         "field": "Name",
    ///         "value": "丁竹",
    ///         "type": "String",
    ///         "op": "like"
    ///     }
    /// ]
    /// </example>
    public List<LinqSelectCondition> filters { get; set; } = new List<LinqSelectCondition>();
    /// <summary>
    /// 多列排序
    /// </summary>
    /// <example>
    /// [
    ///     {
    ///         "field": "name",
    ///         "order": "asc",
    ///     }
    /// ]
    /// </example>
    public List<LinqOrderCondition> sorts { get; set; } = new List<LinqOrderCondition>();
}
