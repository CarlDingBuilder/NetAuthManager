using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Params;

/// <summary>
/// 获取一条数据
/// </summary>
public class GetOneParam
{
    /// <summary>
    /// 数据的主键
    /// </summary>
    /// <example>T001</example>
    public string Id { get; set; }
}

/// <summary>
/// 获取一条数据
/// </summary>
public class GetOneParam<T>
{
    /// <summary>
    /// 数据的主键
    /// </summary>
    /// <example>T001</example>
    public T Id { get; set; }
}
