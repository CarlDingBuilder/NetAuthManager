using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetAuthManager.Application.Sys.Params.RejectReasons;

/// <summary>
/// 退回原因基本参数
/// </summary>
public class RejectReasonBaseParam
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 退回操作类型编码
    /// </summary>
    public string OptTypeCode { get; set; }

    /// <summary>
    /// 退回原因
    /// </summary>
    public string RejectReason { get; set; }

    /// <summary>
    /// 是否关联绩效
    /// </summary>
    public bool IsRelatePerformance { get; set; }

    /// <summary>
    /// 服务实体编码
    /// </summary>
    public string EntityCode { get; set; }

    /// <summary>
    /// 工单编码
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsOpen { get; set; } 
}

/// <summary>
/// 添加参数
/// </summary>
public class RejectReasonAddParam : RejectReasonBaseParam
{
}

/// <summary>
/// 修改参数
/// </summary>
public class RejectReasonModifyParam : RejectReasonBaseParam
{
}

/// <summary>
/// 启用参数
/// </summary>
public class SetRejectReasonIsOpenParam : RejectReasonBaseParam
{
}

/// <summary>
/// 删除参数
/// </summary>
public class RejectReasonDeletesParam
{
    /// <summary>
    /// IDS
    /// </summary>
    public List<string> Ids { get; set; }
}
