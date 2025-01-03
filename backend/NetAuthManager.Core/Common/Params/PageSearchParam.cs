using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Params;

/// <summary>
/// 分页查询参数
/// </summary>
public class PageSearchParam : BaseSearchParam
{
    /// <summary>
    /// 当前页
    /// </summary>
    /// <example>1</example>
    public int pageNo { get; set; } = 1;
    /// <summary>
    /// 每页数量
    /// </summary>
    /// <example>15</example>
    public int pageSize { get; set; } = 15;
}
