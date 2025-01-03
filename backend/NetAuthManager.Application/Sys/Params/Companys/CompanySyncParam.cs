using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params;

/// <summary>
/// 公司参数
/// </summary>
public class CompanySyncParam
{
    /// <summary>
    /// 公司编码
    /// </summary>
    public string CompanyCode { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    /// 公司简称
    /// </summary>
    public string CompanyShortName { get; set; }

    /// <summary>
    /// 城市名称
    /// </summary>
    public string CityName { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 来源系统
    /// </summary>
    /// <example>FSSC</example>
    public string SourceSystem { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsOpen { get; set; }
}
