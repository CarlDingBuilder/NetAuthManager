using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json.Converters;


namespace NetAuthManager.Core.Models;

/// <summary>
/// 排序对象
/// </summary>
public class LinqOrderCondition
{
    /// <summary>
    /// 查询字段名称
    /// </summary>
    /// <example>Name</example>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 排序类型
    /// </summary>
    /// <example>Asc</example>
    [JsonConverter(typeof(StringEnumConverter))]
    public OrderByType Order { get; set; }
}
