using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetAuthManager.Application.Sys.Params;

/// <summary>
/// 服务实体基本参数
/// </summary>
public class ServiceEntityBaseParam
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 服务实体编码
    /// </summary>
    public string EntityCode { get; set; }

    /// <summary>
    /// 服务实体名称
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// 来源编码
    /// </summary>
    public string SourceCode { get; set; }
    
    /// <summary>
    /// 系统域名前缀
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// 是否关联绩效
    /// </summary>
    public bool IsRelatePerformance { get; set; }

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
/// 添加参数
/// </summary>
public class ServiceEntityAddParam : ServiceEntityBaseParam
{
}

/// <summary>
/// 修改参数
/// </summary>
public class ServiceEntityModifyParam : ServiceEntityBaseParam
{
}

/// <summary>
/// 启用参数
/// </summary>
public class SetServiceEntityIsOpenParam : ServiceEntityBaseParam
{
}

/// <summary>
/// 删除参数
/// </summary>
public class ServiceEntityDeletesParam
{
    /// <summary>
    /// 实体编码
    /// </summary>
    public List<string> EntityCodes { get; set; }
}
