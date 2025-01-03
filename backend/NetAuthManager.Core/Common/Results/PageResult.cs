using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Results;

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="TDto"></typeparam>
public class PageResult<TDto>
{
    public IList<TDto> list { get; set; }
    public int total { get; set; }
    public Dictionary<string, object> sumData { get; set; }
    public Dictionary<string, object> extData { get; set; }

    public PageResult(IList<TDto> list = null, int total = 0, Dictionary<string, object> sumData = null, Dictionary<string, object> extData = null)
    {
        this.list = list ?? new List<TDto>();
        this.total = total;
        this.sumData = sumData;
        this.extData = extData;
    }

    /// <summary>
    /// 分页结果构建
    /// </summary>
    /// <param name="list">数据列</param>
    /// <param name="total"></param>
    /// <param name="sumData"></param>
    /// <returns></returns>
    public static PageResult<TDto> Get(IList<TDto> list = null, int total = 0, Dictionary<string, object> sumData = null, Dictionary<string, object> extData = null)
    {
        return new PageResult<TDto>(list, total, sumData, extData);
    }
}
