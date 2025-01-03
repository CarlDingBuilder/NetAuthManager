using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models;

/// <summary>
/// 查询条件
/// </summary>
public class LinqSelectCondition
{
    /// <summary>
    /// 查询字段名称
    /// </summary>
    /// <example>Name</example>
    public string Field { get; set; } = string.Empty;
    /// <summary>
    /// 值
    /// </summary>
    /// <example>丁竹</example>
    public string? Value { get; set; }
    /// <summary>
    /// 值类型
    /// </summary>
    /// <example>DateTime</example>
    public string? Type { get; set; }
    /// <summary>
    /// 查询操作类型，= / like / in / between
    /// </summary>
    /// <example>between</example>
    public string Op { get; set; } = string.Empty;

    [JsonIgnore]
    public LinqSelectOperator Operator {
        get
        {
            switch(Op.ToLower())
            {
                case "like":
                    return LinqSelectOperator.Contains;
                case "in":
                    return LinqSelectOperator.InWithEqual;
                case "between":
                    return LinqSelectOperator.Between;
                case "=":
                default:
                    return LinqSelectOperator.Equal;
            }
        }
    }
}
