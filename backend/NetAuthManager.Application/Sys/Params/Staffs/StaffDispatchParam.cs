using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetAuthManager.Application.Sys.Params.Staffs;

/// <summary>
/// 员工派单基本参数
/// </summary>
public class StaffDispatchBaseParam
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// HRID
    /// </summary>
    public string HRID { get; set; }

    /// <summary>
    /// 用户名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 停止派单时间从
    /// </summary>
    public DateTime StopStartTime { get; set; }

    /// <summary>
    /// 停止派单时间至
    /// </summary>
    public DateTime StopEndTime { get; set; }

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
/// 员工派单添加参数
/// </summary>
public class StaffDispatchAddParam : StaffDispatchBaseParam
{
}

/// <summary>
/// 员工派单修改参数
/// </summary>
public class StaffDispatchModifyParam : StaffDispatchBaseParam
{
}

/// <summary>
/// 员工派单启用参数
/// </summary>
public class SetStaffDispatchIsOpenParam : StaffDispatchBaseParam
{
}