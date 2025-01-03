using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Forms;

/// <summary>
/// 表单同步参数
/// </summary>
public class SyncFormParam
{
    /// <summary>
    /// 工单编码
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 工单名称
    /// </summary>
    public string FormName { get; set; }
}
